using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Text;
using System.Web.Services;
using System.Xml;
using System.Threading;

using Twilio;

using MACSecurity;
using MACServices;
using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using sr = MACServices.Constants.ServiceResponse;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;

[WebService(Namespace = "http://mobileauthcorp.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

[System.Web.Script.Services.ScriptService]
// ReSharper disable once InconsistentNaming
public class TwilioAPIService : WebService
{
    private const string mSvcName = "TwilioAPIService";
    private const string mLogId = "TW";

    [WebMethod]
    // ReSharper disable once InconsistentNaming
    public XmlDocument WsTwilioAPIService(string data)
    {
        var mUtils = new Utils();

        Dictionary<string, string> myData = null;
        // start the XML response
        var myResponse = new StringBuilder();
        mUtils.InitializeXmlResponse(myResponse);

        var request = mUtils.GetIdDataFromRequest(data);
        if (String.IsNullOrEmpty(request.Item1))
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, null, request.Item2, null);

        try
        {
            var sd = Security.DecodeAndDecrypt(request.Item2, request.Item1);
            myData = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(sd);
            myData.Remove(dk.ServiceName);
            myData.Add(dk.ServiceName, mSvcName);
        }
        catch (Exception ex)
        {
            var mDetails = mSvcName + " " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
            var exceptionEvent = new Event
            {
                EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
            };
            exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
            return mUtils.FinalizeXmlResponseWithError(mSvcName + "." + myData[dk.CID] + ex.Message, mLogId);
        }
        // log request if debug set in web.config
        var eid = mUtils.LogRequest(myData, data, mLogId);

        if (!myData.ContainsKey(dk.Request))
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, null, "requestData (no Request)", null);

        if (!myData.ContainsKey(dk.CID))
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, null, "requestData (no CID)", null);

        var myClient = mUtils.GetClientUsingClientId(myData[dk.CID]);
        if (myClient == null)
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, null, "Invalid ClientId", null);
        myData.Add(dk.ClientName, myClient.Name);

        if (myData[dk.Request] == dv.Ping)
        {
            myResponse.Append("<" + sr.Reply + ">" + sr.Success + "</" + sr.Reply + ">");
            return mUtils.FinalizeXmlResponse(myResponse, mLogId);
        }

        if (!myData.ContainsKey(dk.UserId))
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, null, "requestData (no UserId)", null);

        if (!myData.ContainsKey(dkui.PhoneNumber))
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, null, "requestData (no ToPhone)", null);

        if (!myData.ContainsKey(dk.Message))
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, null, "requestData (no Message)", null);

        if (myData.ContainsKey(dk.LoopBackTest))
        {
            switch (myData[dk.LoopBackTest])
            {
                case cfg.NoSend:
                    myResponse.Append("<" + sr.Reply+ ">" + cfg.LoopBackTest + ":" +
                                      cfg.NoSend + "</" + sr.Reply+ ">");
                    return mUtils.FinalizeXmlResponse(myResponse, mLogId);
                case cfg.StartThread:
                case cfg.LoopBackReplyAtAPI: // not supported by Twillio just use thread to verify opt
                case cfg.LoopBackReplyAtGateway: // not supported by Twillio just use thread to verify opt
                    try
                    {
                        var mLoopBackThread =
                            new Thread(() => LoopBackThread(myData[dk.CID], myData[dk.RequestId], myData[dk.OTP]))
                            {
                                IsBackground = true
                            };
                        mLoopBackThread.Start();
                        myResponse.Append("<" + sr.Reply+ ">LoopBackThread Started</" +
                                          sr.Reply+ ">");
                        return mUtils.FinalizeXmlResponse(myResponse, mLogId);
                    }
                    catch (Exception ex)
                    {
                        var mDetails = mSvcName + "." + dk.LoopBackTest + " " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                        var exceptionEvent = new Event
                        {
                            EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                        };
                        exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                        return
                            mUtils.FinalizeXmlResponseWithError(mSvcName + ".LoopBackThread" + ex, mLogId);
                    }
            }
        }
        // Not Loop Back send to Twilio
        #region SMS
        if (myData[dk.Request].Contains(Constants.Strings.Sms))
        {
            foreach(var myProvider in myClient.MessageProviders.SmsProviders)
            {
                if (myData[dk.Request].Contains(myProvider.Name) == false) continue;
                try
                {
                    // if the user id is null then the phone number is not encrypted
                    var mToPhoneNumber = myData[dkui.PhoneNumber];
                    if (!String.IsNullOrEmpty(myData[dk.UserId]))
                        mToPhoneNumber = Security.DecodeAndDecrypt(myData[dkui.PhoneNumber], myData[dk.UserId]);
                    //var message = HttpUtility.UrlEncode(mUtils.HexToString(myData[dk.Message]).Replace("|", Environment.NewLine));
                    var message = mUtils.HexToString(myData[dk.Message]).Replace("|", Environment.NewLine);

                    var twilio = new TwilioRestClient(myProvider.Sid, myProvider.AuthToken);
                    var reply1 = twilio.SendMessage(
                        // TODO: encrypt the pProvider.ShortCodeFromNumber
                        /* From phone number */ myProvider.ShortCodeFromNumber,
                        // /* From phone number */ Security.DecodeAndDecrypt(myProvider.ShortCodeFromNumber, myProvider._id.ToString(),
                        /* To phone number */   mToPhoneNumber.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", ""),
                        /* text message */      message);
                    myResponse.Append("<" + sr.Reply+ ">" + reply1.Status + "</" + sr.Reply+ ">");
                    return mUtils.FinalizeXmlResponse(myResponse, mLogId);
                }
                catch (Exception ex)
                {
                    var mDetails = mSvcName + ".Request "+ ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                    var exceptionEvent = new Event
                    {
                        EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                    };
                    exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                    return mUtils.FinalizeXmlResponseWithError(mSvcName + "." + myData[dk.CID] + ex, mLogId);
                }
            }
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                        myData[dk.CID], "No match for SMS provider", null);
        }
        #endregion

        #region Voice
        if (myData[dk.Request].Contains(Constants.Strings.Voice))
        {
            ////var mToPhoneNumber = Security.DecodeAndDecrypt(myData[dkui.PhoneNumber], myData[dk.UserId]);

            //foreach (var myProvider in myClient.MessageProviders.VoiceProviders)
            //{
            //    return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
            //        myData[dk.CID], "VIOCE NOT IMPLEMENTED", null);
            //}
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                myData[dk.CID], "No match for Voice provider", null);
        }
        #endregion

        return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                null, "Invalid request[" + myData[dk.Request] + "], " + eid, null);
    }

    protected void LoopBackThread(string pCid, string pRID, string pOtpCode)
    {
        try
        {
            var mUtils = new Utils();

            var requestData = dk.Request + dk.KVSep + dv.VerifyOtp +
                   dk.ItemSep + dk.CID + dk.KVSep + pCid +
                   dk.ItemSep + dk.RequestId + dk.KVSep + pRID +
                   dk.ItemSep + dk.OTP + dk.KVSep + pOtpCode;

            var dataStream =
                Encoding.UTF8.GetBytes("data=99" + pCid.Length + pCid.ToUpper() +
                                       mUtils.StringToHex(requestData));

            var webRequest = WebRequest.Create(ConfigurationManager.AppSettings[cfg.MacServicesUrl] +
                                               Constants.ServiceUrls.VerifyOtpWebService);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = dataStream.Length;
            var newStream = webRequest.GetRequestStream();
            // Send the data.
            newStream.Write(dataStream, 0, dataStream.Length);
            newStream.Close();
            var res = webRequest.GetResponse();
            var response = res.GetResponseStream();
            var xmlDoc = new XmlDocument();
            if (response != null) xmlDoc.Load(response);
        }
        catch (Exception ex)
        {
            var mDetails = mSvcName + ".LoopBackThread" + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
            var exceptionEvent = new Event
            {
                EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
            };
            exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
        }
    }
}
