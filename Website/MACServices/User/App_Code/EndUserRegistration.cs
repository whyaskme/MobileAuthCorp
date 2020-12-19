using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.Services;
using System.Xml;

using MACSecurity;
using MACServices;
using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using sr = MACServices.Constants.ServiceResponse;

[WebService(Namespace = "http://mobileauthcorp.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

[System.Web.Script.Services.ScriptService]
public class EndUserRegistration : WebService
{

    public Object PropertyValue { get; set; }
    public virtual Type PropertyType { get; set; }
    private const string mSvcName = "EndUserRegistration";
    private const string mLogId = "SR";

    [WebMethod]
    public XmlDocument WsEndUserRegistration(string data)
    {
        var mUtils = new Utils();

        Tuple<string, string> request;
        // request data Dictionary
        var myData = new Dictionary<string, string> { { dk.ServiceName, mSvcName } };

        // start the XML response
        var myResponse = new StringBuilder();
        mUtils.InitializeXmlResponse(myResponse);

        #region Decrypt/decode Request data
        if (data.StartsWith("99"))
        {
            var requestData = data.Substring(2, data.Length - 2); // dump the 99 from front

            // isloate ID from data
            request = mUtils.GetIdDataFromRequest(requestData);

            // parse string(data) and add to the dictionary
            if (mUtils.ParseIntoDictionary(mUtils.HexToString(request.Item2), myData, char.Parse(dk.KVSep)) == false)
                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                    request.Item1, "Corrupt or bad request data! + Environment.NewLine + data", "98");
        }
        else
        {   //==== Encrypted data ======================================
            // isloate ID from data
            request = mUtils.GetIdDataFromRequest(data);

            // decrypt, parse string and add to the dictionary
            if (mUtils.DecryptAndParseRequestData(request.Item1, request.Item2, myData, char.Parse(dk.KVSep)) == false)
                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                    request.Item1, "Corrupt or bad request data!" + Environment.NewLine + data, "99");
        }
        #endregion

        // log request if debug set in web.config
        var eid = mUtils.LogRequest(myData, data, mLogId);

        // id must be valid client
        var myClient = mUtils.GetClientUsingClientId(myData[dk.CID]);
        if (myClient == null) return mUtils.EmptyXml();
        if (myData.ContainsKey(dk.ClientName) == false)
            myData.Add(dk.ClientName, myClient.Name);

        //Check Ip
        var mResult = mUtils.CheckClientIp(myClient);
        if (mResult.Item1 == false)
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, myData[dk.CID], mResult.Item2, null);

        // Check if the minimun user information was included in request
        var ret = mUtils.Check4MininumEndUserInfo(myData);
        if (ret.Item1 == false)
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, 
                myClient.ClientId.ToString(), ret.Item2, "2");

        if (myData[dk.Request] != dv.EndUserRegister)
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                myClient.ClientId.ToString(), "Invalid request[" + myData[dk.Request] + "], " + eid, null);

        //==== Registration Request ==============================================
        var myEndUser = new EndUser();
        if (myData[dk.RegistrationType] == dv.OpenRegister)
        {   //==== Open Registration Request =====================================
            if (myClient.OpenAccessServicesEnabled == false)
                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                    myClient.ClientId.ToString(), "Invalid registration request[" + myData[dk.RegistrationType] + "], " + eid, null);

            // forward end user registration request to OAS 
            ret = mUtils.ServiceRequest(
                ConfigurationManager.AppSettings[cfg.MacOpenServicesUrl] + 
                Constants.ServiceUrls.MacOpenEndUserServices,
                myClient.ClientId.ToString(), myData);
            if (ret.Item1 == false)
                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, 
                    myClient.ClientId.ToString(), ret.Item2, "5");

            myResponse.Append("<" + sr.Reply + ">" + ret.Item2 + "</" + sr.Reply + ">");
            return mUtils.FinalizeXmlResponse(myResponse, mLogId);
        }
        //==== Client or Group Restricted Registration  =====================================
        var rtn = mUtils.GetHashedIdBasedOnRegistrationType(myData);
        if (rtn.Item1 == false)
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                myClient.ClientId.ToString(), rtn.Item2, "6");
        // check if user exists
        var endUser = mUtils.GetEndUserByHashedUserId(rtn.Item2);
        if (endUser != null)
        {
            if (endUser.State == Constants.EndUserStates.Registered)
            {
                if (endUser.Active != true)
                {
                    var mEuEvent = new EndUserEvent("Already registered, reactivate");
                    endUser.EndUserEvents.Add(mEuEvent);
                    endUser.Active = true;
                    myEndUser.Update();
                }
                myResponse.Append("<" + sr.Reply + ">" + Constants.ServiceResponse.Registered + "</" + sr.Reply + ">");
                return mUtils.FinalizeXmlResponse(myResponse, mLogId);
            }
            // End user is not fully registered
            endUser.State = Constants.EndUserStates.FailedReg;
            var eventText = mSvcName +
                            "; Received a new registration request (" +
                            myData[dk.Request] +
                            ") for this end user " +
                            "before the end user finished the first request." +
                            " The new request superciedes the previous request.";
            endUser.HashedUserId = endUser.HashedUserId + "-99";
            var mEndUserEvent = new EndUserEvent(eventText);
            endUser.EndUserEvents.Add(mEndUserEvent);
            endUser.Update();
            var myFailedEvent = new Event
            {
                ClientId = myClient._id,
                EventTypeDesc = "Registration (" + endUser.State + ") " + eventText
            };
            myFailedEvent.Create();
        }

        // Fill new end user document with user information
        ret = mUtils.PopulateEndUserObject(myEndUser, myData);
        if (ret.Item1 == false)
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                myClient.ClientId.ToString(), ret.Item2, "7");

        //==== Create the end user document =========================================        
        var mCreateEvent = new EndUserEvent(" Registration." + mSvcName + ":" + myEndUser.RegistrationType);
        myEndUser.EndUserEvents.Add(mCreateEvent);
        myEndUser.Create();

        //==== End User Verification check ============================================
        #region Verification
        if (myClient.VerificationProviders != null)
        {
            if (myClient.VerificationProviders.Count() != 0)
            {

                foreach (var mVerificationProvider in myClient.VerificationProviders)
                {
                    myData.Remove(dk.VerificationProviderName);
                    myData.Add(dk.VerificationProviderName, mVerificationProvider.Name);

                    var mVerificationEvent = new EndUserEvent("")
                    {
                        Details = "Verify using " + mVerificationProvider.Name + "["
                    };
                    var configKey = "UserVerification" + mVerificationProvider.Name.Replace(" ", "") + "Service";
                    var mVerificationProviderUrl = "";
                    try
                    {
                        // get the base URL from config
                        var baseUrl = ConfigurationManager.AppSettings["MacServicesUrl"];
                        if (!String.IsNullOrEmpty(baseUrl))
                        {
                            // get service URL from web.config
                            var serviceUrl = ConfigurationManager.AppSettings[configKey];
                            if (!String.IsNullOrEmpty(serviceUrl))
                                mVerificationProviderUrl = baseUrl + serviceUrl;
                        }
                    }
                    catch
                    {
                        mVerificationProviderUrl = null;
                    }
                    if (String.IsNullOrEmpty(mVerificationProviderUrl) == false)
                    {
                        if (mVerificationProvider.Enabled == false)
                        {
                            mVerificationEvent.Details += "Disabled]";
                            myEndUser.EndUserEvents.Add(mVerificationEvent);
                            myEndUser.Update();
                        }
                        else
                        {
                            try
                            {
                                // call the verification service
                                ret = mUtils.ServiceRequest(mVerificationProviderUrl, myClient.ClientId.ToString(),
                                    myData);
                                // add the retrun details to the end user verification event
                                mVerificationEvent.Details += ret.Item2 + "] ";

                                myEndUser.EndUserEvents.Add(mVerificationEvent);
                                myEndUser.Update();
                                if (ret.Item2.Contains("Continue Verification")) continue;
                                if (!ret.Item2.Contains("More User Information")) continue;

                                // tell the UI what the Verification Provider wants
                                myResponse.Append("<VerificationProvider>" + mVerificationProvider.Name +
                                                  "</VerificationProvider>");
                                myResponse.Append("<" + sr.Reply + ">" + ret.Item2 + "</" + sr.Reply + ">");
                                return mUtils.FinalizeXmlResponse(myResponse, mLogId);
                            }
                            catch (Exception ex)
                            {
                                mVerificationEvent.Details += "Call to verification provider failed], " + ex.Message;
                                myEndUser.EndUserEvents.Add(mVerificationEvent);
                                myEndUser.Update();

                                var mDetails = ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";");
                                var exceptionEvent = new Event
                                {
                                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                                };
                                if (myData.ContainsKey(dk.ClientName))
                                    exceptionEvent.EventTypeDesc += Constants.TokenKeys.ClientName + myData[dk.ClientName];
                                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
                            }
                        }
                    }
                    else
                    {
                        mVerificationEvent.Details += "Config Error]";
                        myEndUser.EndUserEvents.Add(mVerificationEvent);
                        myEndUser.Update();
                        //var mEvent = new Event
                        //{
                        //    //Name = "Configuration error",
                        //    //Details = "Could not get config setting using key: " + configKey
                        //    //EventTypeId = Constants.EventLog.Exceptions.BaseId,
                        //    //EventTypeDesc = Constants.EventLog.Exceptions.BaseValue.Replace("[ExceptionDetails]", "Could not get config setting using key: " + configKey)
                        //};
                        //mEvent.Create();
                    }
                } //end foreach
            }
            else
            {
                var mVerificationEvent = new EndUserEvent("") { Details = "No Verification Providers configured." };
                myEndUser.EndUserEvents.Add(mVerificationEvent);
                myEndUser.Update();
            }
        }
        else
        {
            var mVerificationEvent = new EndUserEvent("") {Details = "No Verification Providers configured."};
            myEndUser.EndUserEvents.Add(mVerificationEvent);
            myEndUser.Update();
        }
        #endregion

        //==== Send Registration Completion email =================================
        var mSendEmailEvent = new EndUserEvent(mSvcName + ".SendEmail");
        var emailret = SendCompletionEmail(myClient, myEndUser, myData);
        if (emailret.Item1 == false)
        {
            mSendEmailEvent.Details = mSendEmailEvent.Details + ", failed to send email," + emailret.Item2;
            myEndUser.EndUserEvents.Add(mSendEmailEvent);
            myEndUser.HashedUserId = myEndUser.HashedUserId + "-98";
            myEndUser.State = mSendEmailEvent.Details;
            myEndUser.Update();
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                myClient.ClientId.ToString(), emailret.Item2, "10");
        }
        myEndUser.State = Constants.EndUserStates.WaitingEmailSent;
        mSendEmailEvent.Details = mSendEmailEvent.Details +
                                  ", " + emailret.Item2 +
                                  ", New State=" + myEndUser.State +
                                  ", Ad Opt-Out=" + myEndUser.OtpOutAd;
        myEndUser.EndUserEvents.Add(mSendEmailEvent);
        //==== Registration complete Reply =========================================
        myEndUser.Update();
        myResponse.Append("<" + sr.Reply + ">" + emailret.Item2 + "</" + sr.Reply + ">");
        return mUtils.FinalizeXmlResponse(myResponse, mLogId);
    }

    #region Call Send Email
    // send end use email
    protected static Tuple<bool, string> SendCompletionEmail(Client pClient, EndUser pEndUser, Dictionary<String, String>pData)
    {
        var mUtils = new Utils();

        var myUrlData = dk.Request + dk.KVSep + pData[dk.Request] +
                        dk.ItemSep + dk.CID + dk.KVSep + pClient.ClientId +
                        dk.ItemSep + dk.ClientName + dk.KVSep + pClient.Name +
                        dk.ItemSep + dk.RegistrationType + dk.KVSep + pData[dk.RegistrationType] +
                        dk.ItemSep + dkui.FirstName + dk.KVSep +
                            Security.DecodeAndDecrypt(pEndUser.FirstName, pEndUser.HashedUserId) +
                        dk.ItemSep + dkui.LastName + dk.KVSep +
                            Security.DecodeAndDecrypt(pEndUser.LastName, pEndUser.HashedUserId) +
                        dk.ItemSep + dk.UserId + dk.KVSep + pEndUser.HashedUserId;

        // construct the full Url
        var myFullUrl = String.Format("<a href='{0}?id={1}{2}{3}'>click here</a>",
            /*0*/mUtils.HexToString(pData[dk.EmailLandingPage]),
            /*1*/"99" + pEndUser.HashedUserId.Length,
            /*2*/pEndUser.HashedUserId,
            /*3*/mUtils.StringToHex(myUrlData));
        
        if (pClient.Organization.AdminNotificationProvider == null)
            return new Tuple<bool, string>(false, "Admin Notification Provider not configured!" + pClient.Name);

        //var toAdr = Security.DecodeAndDecrypt(pEndUser.Email, pEndUser.HashedUserId);
        return mUtils.SendEmail(pClient.Organization.AdminNotificationProvider,
            Security.DecodeAndDecrypt(pEndUser.Email, pEndUser.HashedUserId),
            pClient.Name + " Registration", // Subject
            Security.DecodeAndDecrypt(pEndUser.FirstName, pEndUser.HashedUserId) + 
            ",| To complete your registration " + myFullUrl + "|Thank You,|" + pClient.Name,
            pEndUser.HashedUserId);
    }
    #endregion
}
