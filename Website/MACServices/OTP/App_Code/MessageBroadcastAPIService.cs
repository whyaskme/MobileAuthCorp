using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Xml;
using System.Threading;


using MACSecurity;
using MACServices;
using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using sr = MACServices.Constants.ServiceResponse;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using mbrsp = MACServices.Constants.MessageBroadcast.ResponseKeys;
using cr = MACServices.Constants.ReplyServiceRequest;

[WebService(Namespace = "http://mobileauthcorp.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

[System.Web.Script.Services.ScriptService]
public class MessageBroadcastAPIService : WebService
{
    private const string mSvcName = "MessageBroadcastAPIService";
    private const string mLogId = "MB";

    [WebMethod]
    public XmlDocument WsMessageBroadcastAPIService(string data)
    {
        var mUtils = new Utils();

        Dictionary<string, string> myData;
        // start the XML response
        var myResponse = new StringBuilder();
        mUtils.InitializeXmlResponse(myResponse);

        #region decryp, parse and log request
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

            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, null, ex.Message, null);
        }

        // log request if debug set in web.config
        var eid = mUtils.LogRequest(myData, data, mLogId);
        #endregion

        #region Validate Request

        if (!myData.ContainsKey(dk.CID))
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, null, "requestData (no CID), EID:" + eid, null);

        if (!myData.ContainsKey(dk.Request))
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, null, "requestData (no Request), EID:" + eid, null);

        var myClient = mUtils.GetClientUsingClientId(myData[dk.CID]);
        if (myClient == null)
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, null, "Invalid ClientId, EID:" + eid, null);
        myData.Remove(dk.ClientName);
        myData.Add(dk.ClientName, myClient.Name);

        if (myData[dk.Request] == dv.Ping)
        {
            myResponse.Append("<" + sr.Reply + ">" + sr.Success + "</" + sr.Reply + ">");
            return mUtils.FinalizeXmlResponse(myResponse, mLogId);
        }

        if (!myData.ContainsKey(dkui.PhoneNumber))
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, null, "requestData (no ToPhone), EID:" + eid, null);

        if (!myData.ContainsKey(dk.Message))
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, null, "requestData (no Message), EID:" + eid, null);

        #endregion

        // get log settings from config
        var mServiceLogSettings = ConfigurationManager.AppSettings[cfg.DebugLogRequests];

        #region LoopBack
        if (myData.ContainsKey(dk.LoopBackTest) && (myData[dk.LoopBackTest] != cfg.LoopBackReplyAtGateway))
        {
            switch (myData[dk.LoopBackTest])
            {
                    // todo: add loopback by gateway
                case cfg.NoSend:
                    myResponse.Append("<" + sr.Reply+ ">" + cfg.LoopBackTest + ":" + cfg.NoSend + "</" + sr.Reply+ ">");
                    return mUtils.FinalizeXmlResponse(myResponse, mLogId);

                case cfg.LoopBackReplyAtAPI:
                    if (myData.ContainsKey(dk.EnableReply))
                    {   // Yes, do loopback reply
                        try
                        {
                            var mLoopBackThread =
                                new Thread(
                                    () => LoopBackReplyThread(myData[dk.CID], myData[dk.RequestId], myData[dk.OTP]))
                                {
                                    IsBackground = true
                                };
                            mLoopBackThread.Start();
                            myResponse.Append("<" + sr.Reply + ">LoopBackReplyThread Started</" +
                                              sr.Reply + ">");
                            return mUtils.FinalizeXmlResponse(myResponse, mLogId);
                        }
                        catch (Exception ex)
                        {
                            var mDetails = mSvcName + "." + dk.LoopBackTest + " " +
                                           ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") +
                                           Constants.TokenKeys.ClientName + "NA";
                            var exceptionEvent = new Event
                            {
                                EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                            };
                            exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                            return
                                mUtils.FinalizeXmlResponseWithError(mSvcName + ".LoopBackThread" + ex, mLogId);
                        }
                    }
                    // Reply not enables, assume nosend 
                    myResponse.Append("<" + sr.Reply + ">" + cfg.LoopBackTest + ":" + cfg.NoSend + "</" + sr.Reply + ">");
                    return mUtils.FinalizeXmlResponse(myResponse, mLogId);
                case cfg.StartThread:
                    try
                    {
                        var mLoopBackThread =
                            new Thread(
                                () => LoopBackVerifyOtpThread(myData[dk.CID], myData[dk.RequestId], myData[dk.OTP]))
                            {
                                IsBackground = true
                            };
                        mLoopBackThread.Start();
                        myResponse.Append("<" + sr.Reply + ">LoopBackVerifyOtpThread Started</" +
                                            sr.Reply + ">");
                        return mUtils.FinalizeXmlResponse(myResponse, mLogId);
                    }
                    catch (Exception ex)
                    {
                        var mDetails = mSvcName + "." + dk.LoopBackTest + " " +
                                        ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") +
                                        Constants.TokenKeys.ClientName + "NA";
                        var exceptionEvent = new Event
                        {
                            EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                        };
                        exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                        return
                            mUtils.FinalizeXmlResponseWithError(mSvcName + ".LoopBackThread" + ex, mLogId);
                    }
            } // end switch
        }
        #endregion
        
        // Not Loop Back send to Message Broadcast
        #region SMS
        if (myData[dk.Request].Contains(Constants.Strings.Sms))
        {
            // find message broadcast object in client's message providers
            foreach (var myProvider in myClient.MessageProviders.SmsProviders)
            {
                if (myData[dk.Request].Contains(myProvider.Name) == false) continue;
                //------------------------------------------------------------------- 
                var MyDateTime = DateTime.UtcNow.ToString();

                var stringToSign = "POST" + "\n\n\n" + MyDateTime + "\n\n\n";
                // private key from Sid properity
                var signature = HmacSha1SignRequest(myProvider.Sid, stringToSign);
                var phoneNumber = myData[dkui.PhoneNumber];
                if (myData.ContainsKey(dk.UserId))
                {
                    if (!String.IsNullOrEmpty(myData[dk.UserId]))
                        phoneNumber = Security.DecodeAndDecrypt(myData[dkui.PhoneNumber], myData[dk.UserId]);
                }
                var mTextMessage = mUtils.HexToString(myData[dk.Message]).Replace("|", "\n");
                var sbRestRequest = new StringBuilder();
                // Batch Id from provider object
                sbRestRequest.Append("inpBatchId=" + myProvider.ShortCodeFromNumber);
                // Mobile Phone Number
                sbRestRequest.Append("&inpContactString=" + phoneNumber);
                // Contact type single mobile phone
                sbRestRequest.Append("&inpContactTypeId=3");
                // The test message
                sbRestRequest.Append("&inpA=" + Server.UrlEncode(mTextMessage));
                // Carrier required t&c for initial text message to user
                sbRestRequest.Append("&inpB=" + Server.UrlEncode(Constants.MessageBroadcast.RequestKeys.CarrierFirstTimeInfo));
                //Time zone required to get correct error response on invalid phone numbers
                sbRestRequest.Append("&inpTimeZone=Mountain");

                // is this request using the reply process to validate the OTP?
                if (myData.ContainsKey(dk.EnableReply))
                {   // Yes, include additional parameters in request
                    // correlation number is the client id and the request id separated by a colon
                    sbRestRequest.Append("&inpId=" + myData[dk.CID] + ":" + myData[dk.RequestId]);
                    sbRestRequest.Append("&inpDomain=" + HttpUtility.UrlEncode(ConfigurationManager.AppSettings[cfg.MessageBroadcastReplyDomain]));
                    // Reply Uri is Message Broadcast Reply Service
                    sbRestRequest.Append("&inpPath=" + HttpUtility.UrlEncode("MacServices" + Constants.ServiceUrls.MessageBroadcastReplyService));
                    // Loopback indicator
                    if ((myData.ContainsKey(dk.LoopBackTest))
                        && (myData[dk.LoopBackTest] == cfg.LoopBackReplyAtGateway))
                        sbRestRequest.Append("&inplb=1");
                    else
                        sbRestRequest.Append("&inplb=0");
                }

                var dataStream = Encoding.UTF8.GetBytes(sbRestRequest.ToString());
                // request to event log
                if (mServiceLogSettings.Contains(mLogId) && (mServiceLogSettings.Contains("API")))
                {
                    var mEventRequest = new Event
                    {
                        EventTypeName = myProvider.Name + " request",
                        EventTypeDesc = "URL[" + myProvider.Url + 
                        "]..." +
                        "myHttpWebRequest[" +
                        "Method=POST," +
                        "ContentType=application/x-www-form-urlencoded,"+
                        "ContentLength=" + dataStream.Length + "," +
                        "Headers['datetime']=" + MyDateTime + "," +
                        "Headers['Authorization']=" + myProvider.AuthToken + ":" + signature + ", " +
                        "Accept=application/xml]..." +
                        "Data[" + (sbRestRequest.ToString()) + "]"
                    };
                    mEventRequest.Create();
                }
                // Send the request to Message Broadcast
                try
                {
                    var myHttpWebRequest = (HttpWebRequest)WebRequest.Create(myProvider.Url);
                    myHttpWebRequest.Method = "POST";
                    // Set the content type of the data being posted.
                    myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
                    myHttpWebRequest.ContentLength = dataStream.Length;
                    myHttpWebRequest.Headers["datetime"] = MyDateTime;
                    myHttpWebRequest.Headers["Authorization"] = myProvider.AuthToken + ":" + signature;
                    // request XML response
                    myHttpWebRequest.Accept = "application/xml";
                    var newStream = myHttpWebRequest.GetRequestStream();
                    newStream.Write(dataStream, 0, dataStream.Length);
                    newStream.Close();

                    string mResponseData;
                    using (var response = (HttpWebResponse)myHttpWebRequest.GetResponse())
                    {
                        var header = response.GetResponseStream();
                        if (header == null)
                        {
                            myResponse.Append("<" + sr.Error + ">" + "Error: Null response" + "</" + sr.Error + ">");
                            return mUtils.FinalizeXmlResponse(myResponse, mLogId);
                        }
                        var encode = Encoding.GetEncoding("utf-8");
                        var readStream = new StreamReader(header, encode);
                        mResponseData = readStream.ReadToEnd();
                        response.Close();
                        readStream.Close();
                    }
                    //  log the response, if web.config () contains this service's code and the log API code
                    if (mServiceLogSettings.Contains(mLogId) && (mServiceLogSettings.Contains("API")))
                    {
                        var mEventResponse = new Event
                        {
                            EventTypeName = myProvider.Name = " response",
                            EventTypeDesc = (mResponseData.ToString()).Replace("&", "&amp;")
                        };
                        mEventResponse.Create();
                    }

                    var xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(mResponseData);
                    var elemList = xmlDoc.GetElementsByTagName(mbrsp.BLOCKEDBYDNC);
                    var mBLOCKEDBYDNCValue = elemList[0].InnerXml;
                    elemList = xmlDoc.GetElementsByTagName(mbrsp.SMSINIT);
                    var mSMSINITValue = elemList[0].InnerXml;
                    elemList = xmlDoc.GetElementsByTagName(mbrsp.MESSAGE);
                    var mMESSAGEValue = elemList[0].InnerXml;
                    elemList = xmlDoc.GetElementsByTagName(mbrsp.ERRMESSAGE);
                    var mERRMESSAGEValue = elemList[0].InnerXml;
                    elemList = xmlDoc.GetElementsByTagName(mbrsp.SMSDEST);
                    var mSHORTCODE = elemList[0].InnerXml;

                    if (mBLOCKEDBYDNCValue == "1")
                        myResponse.Append("<" + sr.Error + ">" + "Error: Not sent, Blocked user replied '" + sr.STOP +
                                          "' (" + sr.FromNumber + "=" + mSHORTCODE + ")</" + sr.Error + ">");
                    else if (mSMSINITValue == "0")
                        myResponse.Append("<" + sr.Error + ">" + "Error: Not sent, " +
                                          mMESSAGEValue + " (" + mERRMESSAGEValue + ")</" + sr.Error + ">");
                    else
                        myResponse.Append("<" + sr.Reply + ">" + sr.Sent + "</" + sr.Reply + ">");
                    return mUtils.FinalizeXmlResponse(myResponse, mLogId);
                }
                catch (Exception ex)
                {
                    var mDetails = mSvcName + ".webRequest " +
                                   ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") +
                                   Constants.TokenKeys.ClientName + myData[dk.CID];
                    var exceptionEvent = new Event
                    {
                        EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                    };
                    exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                    return
                        mUtils.FinalizeXmlResponseWithError(
                            mSvcName + "." + myData[dk.CID] + ex.Message, mLogId);
                }
            }

            return mUtils.FinalizeXmlResponse(myResponse, mLogId);
        }
        #endregion

        #region Voice
        if (myData[dk.Request].Contains(Constants.Strings.Voice))
        {
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                myData[dk.CID], "No match for Voice provider", null);
        }
        #endregion

        return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
            null, "Invalid request: " + myData[dk.Request] + "], " + eid, null);
    }

    protected string HmacSha1SignRequest(string privateKey, string valueToHash)
    {
        var encoding = new ASCIIEncoding();

        var keyByte = encoding.GetBytes(privateKey);
        var hmacsha1 = new HMACSHA1(keyByte);

        var messageBytes = encoding.GetBytes(valueToHash);
        var hashmessage = hmacsha1.ComputeHash(messageBytes);
        var hashedValue = Convert.ToBase64String(hashmessage); // convert to base64

        return hashedValue;
    }


    #region Loopback Methods

    protected bool xStartLoopBackOtpThread(Dictionary<string, string>pData, StringBuilder pResponse)
    {
        var mLoopBackThread =
            new Thread(
                () => LoopBackVerifyOtpThread(pData[dk.CID], pData[dk.RequestId], pData[dk.OTP]))
            {
                IsBackground = true
            };
        mLoopBackThread.Start();
        pResponse.Append("<" + sr.Reply + ">LoopBackVerifyOtpThread Started</" +
                            sr.Reply + ">");
        return true;
    }
    protected void LoopBackVerifyOtpThread(string pCid, string pRID, string pOtpCode)
    {
        Thread.Sleep(2 * 1000);
        try
        {
            var mUtils = new Utils();
            var mUri = ConfigurationManager.AppSettings[cfg.MacServicesUrl] +
                       Constants.ServiceUrls.VerifyOtpWebService;

            var requestData = dk.Request + dk.KVSep + dv.VerifyOtp +
                              dk.ItemSep + dk.CID + dk.KVSep + pCid +
                              dk.ItemSep + dk.RequestId + dk.KVSep + pRID +
                              dk.ItemSep + dk.OTP + dk.KVSep + pOtpCode;

            var dataStream =
                Encoding.UTF8.GetBytes("data=99" + pCid.Length + pCid.ToUpper() +
                                       mUtils.StringToHex(requestData));

            var webRequest = WebRequest.Create(mUri);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = dataStream.Length;
            var newStream = webRequest.GetRequestStream();
            // Send the data.
            newStream.Write(dataStream, 0, dataStream.Length);
            newStream.Close();
            var res = webRequest.GetResponse();
            res.GetResponseStream();
        }
        catch (Exception ex)
        {
            var mDetails = mSvcName + ".LoopBackVerifyOtpThread " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") +
                           Constants.TokenKeys.ClientName + "NA";
            var exceptionEvent = new Event
            {
                EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
            };
            exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
        }
    }
    #endregion

    #region LoopBackReplyThread
    protected void LoopBackReplyThread(string pCid, string pRID, string pOtpCode)
    {
        Thread.Sleep(2 * 1000);

        var mUri = ConfigurationManager.AppSettings[cfg.MacServicesUrl]
                    + Constants.ServiceUrls.MessageBroadcastReplyService
                    + "?" + cr.Request +"=" + dv.VerifyOtp 
                    + "&" + cr.inpId + "=" + pCid + ":" + pRID 
                    + "&" + cr.text + "=" + pOtpCode
                    + "&API=LB";

        var request = (HttpWebRequest)WebRequest.Create(mUri);
        //request.Method = "GET";
        request.MaximumAutomaticRedirections = 4;
        request.MaximumResponseHeadersLength = 4;
        // Set credentials to use for this request.
        request.Credentials = CredentialCache.DefaultCredentials;
        var response = (HttpWebResponse)request.GetResponse();
        // Get the stream associated with the response.
        response.GetResponseStream();
        response.Close();
    }
    #endregion
}