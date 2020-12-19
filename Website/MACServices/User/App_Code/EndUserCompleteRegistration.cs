using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Web.Services;
using System.Xml;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

using MACServices;
using MACSecurity;
using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using sr = MACServices.Constants.ServiceResponse;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using es = MACServices.Constants.EventStats;
using cs = MACServices.Constants.Strings;

[WebService(Namespace = "http://mobileauthcorp.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

[System.Web.Script.Services.ScriptService]
public class EndUserCompleteRegistration: WebService
{

    private const string mSvcName = "EndUserCompleteRegistration";
    private const string mLogId = "CR";
    public Object PropertyValue { get; set; }
    public virtual Type PropertyType { get; set; }


    [WebMethod]
    public XmlDocument WsEndUserCompleteRegistration(string data)
    {
        var mUtils = new Utils();

         Tuple<string, string> request;
        // request data Dictionary
         var myData = new Dictionary<string, string> { { dk.ServiceName, mSvcName } };

        // start the XML response
        var myResponse = new StringBuilder();
        mUtils.InitializeXmlResponse(myResponse);

        if (data.StartsWith("99"))
        {
            var requestData = data.Substring(2, data.Length - 2); // dump the 99 from front
            // isloate ID from data
            request = mUtils.GetIdDataFromRequest(requestData);

            // parse string(data) and add to the dictionary
            if (mUtils.ParseIntoDictionary(mUtils.HexToString(request.Item2), myData, char.Parse(dk.KVSep)) == false)
                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                    request.Item1, "Corrupt request!" + Environment.NewLine + data, "99");
        }
        else
        {   // Encrypted
            request = mUtils.GetIdDataFromRequest(data);
            if (String.IsNullOrEmpty(request.Item1))
                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                    null, request.Item2 + Environment.NewLine + data, "1");

            if (mUtils.DecryptAndParseRequestData(request.Item1, request.Item2, myData, char.Parse(dk.KVSep)) == false)
                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                    null, "Corrupt request!" + Environment.NewLine + data, "2");
        }

        // log request if debug set in web.config
        var eid = mUtils.LogRequest(myData, data, mLogId);

        if (!myData.ContainsKey(dk.Request))
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, 
                null, "Request type required!", "3");

        // id must be valid client
        var myClient = mUtils.ValidateClient(myData[dk.CID]);
        if (myClient == null)
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                        null, "Corrupt request data!", "2");
        myData.Remove(dk.ClientName);
        myData.Add(dk.ClientName, myClient.Name);

        #region Request Registration OTP
        if (myData[dk.Request].Contains(dv.RequestRegistrationOtp))
        { // errors numbered 20-29
            try
            {
                var mEndUserEvent = new EndUserEvent(myData[dk.Request]);
                var myEndUser = mUtils.GetEndUserByHashedUserId(myData[dk.UserId].Trim());
                if (myEndUser == null)
                    return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                        request.Item1, "Invalid end user", "21");

                if (myEndUser.State == Constants.EndUserStates.WaitingOtp)
                    return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                        request.Item1, "OTP Sent", " Check your text messages");

                //==== send OTP to end user's mobile phone ==================================
                var myOtp = new Otp(myData[dk.CID])
                {
                    UserId = myEndUser.HashedUserId,
                    ToPhone = myEndUser.Phone,
                    UserName = myEndUser.FirstName,
                    ToEmail = myEndUser.Email,
                    UserIpAddress = "N/A",
                    Name = "End User Registration",
                    TrxType = 3
                };

                // send the message
                var otpReturn = mUtils.SendMessageToEndUser(myClient, myOtp);
                if (otpReturn.Item1 == false)
                {
                    return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                        myData[dk.CID], myOtp.ErrorMsg, "23");
                }
                // need more time to account for the email delivery
                myOtp.EndOfLife = DateTime.UtcNow.AddDays(10);
                myEndUser.State = Constants.EndUserStates.WaitingOtp;
                mEndUserEvent.Details = mSvcName + ", OTP Sent(" + myOtp.DeliveryMethod + "), New State=" + myEndUser.State;
                myEndUser.EndUserEvents.Add(mEndUserEvent);
                myEndUser.Update();
                var otpEvent = new Event
                {
                    EventTypeDesc = "OTP Sent(" + myOtp.DeliveryMethod + ")",
                    //ClientId = myClient.ClientId,
                    //UserId = myEndUser._id
                };
                myOtp.CodeHistory.Add(otpEvent);
                // Write the OTP to the database
                mUtils.ObjectCreate(myOtp);
                // reply must have the work RequestId to get correctly processed by the user interface(web page)
                myResponse.Append("<" + sr.RequestId + ">" + myOtp._id + "</" + sr.RequestId + ">");
                // Details
                myResponse.Append("<" + sr.Details + ">");
                {
                    myResponse.Append(sr.Request + dk.KVSep + myData[dk.Request]);
                    myResponse.Append(dk.ItemSep + sr.Action + dk.KVSep + sr.Sent);
                    myResponse.Append(dk.ItemSep + dk.ClientName + dk.KVSep + myClient.Name);
                    myResponse.Append(dk.ItemSep + sr.DeliveryMethod + dk.KVSep + myOtp.DeliveryMethod);
                    myResponse.Append(dk.ItemSep + sr.TLM + dk.KVSep + myClient.OtpSettings.Timeout);
                    myResponse.Append(dk.ItemSep + dk.OTPRetriesMax + dk.KVSep + myOtp.ValidationRetries);
                    myResponse.Append(dk.ItemSep + dk.OTPExpiredTime + dk.KVSep + myOtp.EndOfLife);
                }
                myResponse.Append("</" + sr.Details + ">");

                // Debug or QA User
                if (Convert.ToBoolean(ConfigurationManager.AppSettings[cfg.Debug]) ||
                    (Security.DecodeAndDecrypt(myOtp.UserName, myOtp.UserId) == cs.QAUserFirstName))
                { // if debug is set or End User's First name is the QA User
                    //   return debug node containing requestId and the OTP, Debug Node
                    //   User for load testing and Automated QA Testing
                    myResponse.Append("<" + sr.Debug + ">");
                    {
                        myResponse.Append(sr.OTP + dk.KVSep + myOtp.Code);
                    }
                    myResponse.Append("</" + sr.Debug + ">");
                }
                var myReply = mUtils.FinalizeXmlResponse(myResponse, mLogId);
                return myReply;
            }
            catch (Exception ex)
            {
                var mDetails = ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                return mUtils.FinalizeXmlResponseWithError(
                    mSvcName + "." + myData[dk.Request] + "Exception: " + ex.Message, mLogId);
            }
        }
        #endregion

        #region Verify Registration OTP
        if (myData[dk.Request].Contains(dv.VerifyOtp))
        { // errors numbered 10-19
            if (!myData.ContainsKey(dk.RegistrationType))
                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                    myClient._id.ToString(), "requestData missing RegistrationType", "10");

            if (!myData.ContainsKey(dk.RequestId))
                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                    myClient._id.ToString(), "requestData missing RequestId", "11");

            if (!myData.ContainsKey(dk.OTP))
                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                    myClient._id.ToString(), "requestData missing Otp", "12");

            if (String.IsNullOrEmpty(myData[dk.OTP]))
                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                    myClient._id.ToString(), "OTP is null", "12");

            // get Otp using the Request Id
            var myOtp = mUtils.GetOtpUsingRequestId(myData[dk.RequestId]);
            if (myOtp == null)
                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                    myClient._id.ToString(), "Invalid request Id[" + myData[dk.RequestId] + "], " + eid, "13");

            var otpEvent = new Event
            {
                EventTypeDesc = myData[dk.Request] + "(" + myData[dk.OTP] + ")",
                ClientId = myClient.ClientId,
            };
            // is the Otp still active
            if (myOtp.Active == false)
            {
                myResponse.Append("<" + sr.Reply + ">" + Constants.ServiceResponse.Inactive + "</" + sr.Reply + ">");
                myResponse.Append("<" + sr.Details + ">Marked inactive</" + sr.Details + ">");
            }
            try
            {
                // Has the OTP timed out
                var myNow = DateTime.UtcNow;
                var myTill = myOtp.EndOfLife;
                if (myNow > myTill)
                {  //yes the OTP has timed out
                    otpEvent.EventTypeDesc += "Timeout";
                    myResponse.Append("<" + sr.Reply + ">" + Constants.ServiceResponse.Timeout + "</" + sr.Reply + ">");
                    myResponse.Append("<" + sr.Details + ">" + otpEvent.EventTypeDesc + "</" + sr.Details + ">");
                }
                else
                {   // validate the OTP
                    if (myData[dk.OTP] == myOtp.Code)
                    {

                        // ReSharper disable once UnusedVariable
                        var myStat1 = new EventStat(myClient._id, myClient.Name, es.OtpValid, 1);

                        otpEvent.EventTypeDesc += " is validated";
                        if (myData[dk.RegistrationType].Contains(dv.Open))
                        {
                            // send activate end user to open service
                            var myReq = new Dictionary<string, string> {{dk.Request, dv.ActivateEndUser}};
                            var ret = mUtils.ServiceRequest(
                                ConfigurationManager.AppSettings[cfg.MacOpenServicesUrl] +
                                Constants.ServiceUrls.MacOpenEndUserServices,
                                request.Item1, myReq);
                            if (ret.Item1 == false)
                                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                                    myData[dk.CID], ret.Item2, "15");
                            myOtp.Active = false; // set the OTP so it can't be used again
                            otpEvent.EventTypeDesc += " " + myData[dk.Request] + ":" + ret.Item2;
                        }
                        else
                        {
                            // check if user exists
                            var myEndUser = mUtils.GetEndUserByHashedUserId(myData[dk.UserId]);
                            if (myEndUser == null)
                            {
                                myOtp.Active = false; // set the OTP so it can't be used again
                                otpEvent.EventTypeDesc += ", could not get End User by Id: " + myData[dk.UserId];
                                myOtp.CodeHistory.Add(otpEvent);
                                myOtp.Update();
                                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                                    myData[dk.CID], otpEvent.EventTypeDesc, "16");
                            }
                            // Activate end user
                            myEndUser.Active = true;
                            myEndUser.State = Constants.EndUserStates.Registered;

                            var euEvent = new EndUserEvent(mSvcName + 
                                                    ", New State=" + myEndUser.State +
                                                    ", Ad Opt-Out=" + myEndUser.OtpOutAd);
                            myEndUser.EndUserEvents.Add(euEvent);
                            myEndUser.Update();
                            myOtp.Active = false; // set the OTP so it can't be used again
                            otpEvent.EventTypeDesc += " user activated";
                        }
                        // ReSharper disable once UnusedVariable
                        var myStat2 = new EventStat(myClient._id, myClient.Name, es.EndUserRegister, 1);

                        myResponse.Append("<" + sr.Reply + ">" + Constants.ServiceResponse.Validated + "</" + sr.Reply + ">");
                        myResponse.Append("<" + sr.Details + ">" + otpEvent.EventTypeDesc + "</" + sr.Details + ">");
                    }
                    else
                    {   // OTP was invalid check the retry count
                        // ReSharper disable once UnusedVariable
                        var myStat = new EventStat(myClient._id, myClient.Name, es.OtpInvalid, 1);

                        if (myOtp.ValidationCount < myOtp.ValidationRetries)
                        {
                            myOtp.ValidationCount++;
                            myResponse.Append("<" + sr.Reply + ">" + Constants.ServiceResponse.Invalid + "</" + sr.Reply + ">");
                            myResponse.Append("<" + sr.Details + ">" + otpEvent.EventTypeDesc + ", Retry count: " + myOtp.ValidationCount + "</" + sr.Details + ">");
                        }
                        else
                        {   // too many retries
                            otpEvent.EventTypeDesc += Constants.ServiceResponse.Invalid + ", Too many retries!";
                            myOtp.Active = false; // set so OTP can't be used again
                            if (myData[dk.RegistrationType].Contains(dv.Open))
                            {
                                // send Delete end user to open service
                                var myReq = new Dictionary<string, string> {{dk.Request, dv.DeleteEndUser}};
                                mUtils.ServiceRequest(
                                    ConfigurationManager.AppSettings[cfg.MacOpenServicesUrl] +
                                    Constants.ServiceUrls.MacOpenEndUserServices,
                                    request.Item1, myReq);
                            }
                            else
                            { // delete end user document
                                var myEndUser = mUtils.GetEndUserByHashedUserId(myData[dk.UserId]);
                                if (myEndUser != null) mUtils.DeleteEndUser(myEndUser);
                            }
                            myOtp.Active = false; // set so Otp can't be used again
                            otpEvent.EventTypeDesc += " " + Constants.ServiceResponse.Disabled;
                            myResponse.Append("<" + sr.Reply + ">" + Constants.ServiceResponse.Disabled + "</" + sr.Reply + ">");
                            myResponse.Append("<" + sr.Details + ">" + otpEvent.EventTypeDesc + "</" + sr.Details + ">");
                        }
                    }
                }
                myOtp.CodeHistory.Add(otpEvent);
                myOtp.Update();
                return mUtils.FinalizeXmlResponse(myResponse, mLogId);
            }
            catch (Exception ex)
            {
                var mDetails = ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";");
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                if (myData.ContainsKey(dk.ClientName))
                    exceptionEvent.EventTypeDesc += Constants.TokenKeys.ClientName + myData[dk.ClientName];
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                return mUtils.FinalizeXmlResponseWithError(
                    mSvcName + "." + myData[dk.Request] + "Exception: " + ex.Message, mLogId);
            }
        }
        #endregion

        #region Resend Registration OTP

        if (myData[dk.Request].Contains(dv.ResendOtp))
        {
            try
            {
                if (!myData.ContainsKey(dk.RequestId))
                    return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                        myData[dk.CID], "requestData missing RequestId", "30");

                // get OTP object using the Request Id
                var myOtp = mUtils.GetOtpUsingRequestId(myData[dk.RequestId]);
                if (myOtp == null)
                    return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                        myData[dk.CID], "Invalid request Id[" + myData[dk.RequestId] + "]", "31");

                if (myOtp.Active == false)
                    return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                        myData[dk.CID], "Disabled can not resend", "32");

                var otpEvent = new Event
                {
                    EventTypeDesc = myData[dk.Request],
                    ClientId = myClient.ClientId,
                };
                // Has the OTP timed out
                var myNow = DateTime.UtcNow;
                var myTill = myOtp.EndOfLife;
                if (myNow > myTill)
                {  //yes the OTP has timed out
                    myOtp.Active = false;
                    otpEvent.EventTypeDesc += Constants.ServiceResponse.Timeout;
                    myResponse.Append("<" + sr.Reply + ">" + Constants.ServiceResponse.Timeout + "</" + sr.Reply + ">");
                    myResponse.Append("<" + sr.Details + ">" + otpEvent.EventTypeDesc + "</" + sr.Details + ">");
                }
                else if (myOtp.ValidationCount < myOtp.ValidationRetries)
                {
                    myOtp.ValidationCount++;
                    // reset the life span
                    var timeout = myClient.OtpSettings.Timeout;
                    if (timeout == 0) timeout = 3;
                    myOtp.EndOfLife = DateTime.UtcNow.AddMinutes(timeout);

                    myOtp.TrxType = 99;
                    var ret1 = mUtils.SendMessageToEndUser(myClient, myOtp);
                    if (ret1.Item1 == false)
                        return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                            myData[dk.CID], myOtp.ErrorMsg, "33");

                    myResponse.Append("<" + sr.Reply + ">" + Constants.ServiceResponse.Resent + "</" + sr.Reply + ">");
                    myResponse.Append("<" + sr.RequestId + ">" + myOtp._id + "</" + sr.RequestId + ">");
                    myResponse.Append("<" + sr.DeliveryMethod + ">" + myOtp._id + "</" + sr.DeliveryMethod + ">");
                    myResponse.Append("<" + sr.Details + ">" + otpEvent.EventTypeDesc + "</" + sr.Details + ">");
                }
                else
                { // too many retries
                    myOtp.Active = false; // set so OTP can't be used again
                    otpEvent.EventTypeDesc += Constants.ServiceResponse.Invalid + " too many retries";
                    myOtp.Active = false;
                    myResponse.Append("<" + sr.Reply + ">" + Constants.ServiceResponse.Invalid + "</" + sr.Reply + ">");
                    myResponse.Append("<" + sr.Details + ">" + otpEvent.EventTypeDesc + "</" + sr.Details + ">");
                }
                // Update the OTP
                mUtils.ObjectUpdate(myOtp, myOtp._id.ToString());
                return mUtils.FinalizeXmlResponse(myResponse, mLogId);
            }
            catch (Exception ex)
            {
                var mDetails = ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";");
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                if (myData.ContainsKey(dk.ClientName))
                    exceptionEvent.EventTypeDesc += Constants.TokenKeys.ClientName + myData[dk.ClientName];
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                return mUtils.FinalizeXmlResponseWithError(
                    mSvcName + "." + myData[dk.Request] + "Exception: " + ex.Message, mLogId);
            }
        }
        #endregion

        #region Cancel Registration
        if (myData[dk.Request].Contains(dv.CancelRegistration))
        { // errors numbered 40-49
            try
            {
                var otpEvent = new Event { EventTypeDesc = myData[dk.Request] };
                if (myData[dk.RegistrationType] == dv.OpenRegister)
                {
                    // send delete end user registration to open service
                    var myReq = new Dictionary<string, string> {{dk.Request, dv.DeleteEndUser}};
                    var ret = mUtils.ServiceRequest(
                        ConfigurationManager.AppSettings[cfg.MacOpenServicesUrl] +
                        Constants.ServiceUrls.MacOpenEndUserServices,
                        request.Item1,
                        myReq);
                    if (ret.Item1 == false)
                        otpEvent.EventTypeDesc = Constants.ServiceResponse.Error + " OAS failed to delete EndUser: " + ret.Item1;
                    else
                        otpEvent.EventTypeDesc = Constants.ServiceResponse.Error + " OAS Deleted EndUser: " + ret.Item1;
                }
                else // GroupRegister or ClientRegister
                {
                    // check if user exists
                    var myEndUser = mUtils.GetEndUserByHashedUserId(myData[dk.UserId]);
                    //var myEndUser = mUtils.GetEndUser(request.Item1, myClient.ClientId.ToString(), null);
                    if (myEndUser == null)
                    {
                        otpEvent.EventTypeDesc = Constants.ServiceResponse.Error + " No EndUser to deleted!";
                    }
                    else
                    {
                        var rtn = mUtils.DeleteEndUser(myEndUser);
                        if (String.IsNullOrEmpty(rtn))
                            otpEvent.EventTypeDesc = Constants.ServiceResponse.Error + " EndUser deleted!";
                        else
                            otpEvent.EventTypeDesc = Constants.ServiceResponse.Error + " Could not delete EndUser: " + rtn;
                    }
                }
                myResponse.Append("<" + sr.Reply + ">" + Constants.ServiceResponse.Error + "</" + sr.Reply + ">");
                myResponse.Append("<" + sr.Details + ">" + otpEvent.EventTypeDesc + "</" + sr.Details + ">");
            }
            catch (Exception ex)
            {
                var mDetails = ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";");
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                if (myData.ContainsKey(dk.ClientName))
                    exceptionEvent.EventTypeDesc += Constants.TokenKeys.ClientName + myData[dk.ClientName];
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                return mUtils.FinalizeXmlResponseWithError(
                    mSvcName + "." + myData[dk.Request] + "Exception: " + ex.Message, mLogId);
            }
        }
        #endregion
        
        return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                    null, "Invalid request[" + myData[dk.Request] + "]", null);
    }
}
