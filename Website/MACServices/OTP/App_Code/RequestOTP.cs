using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web.Security;
using System.Web.Services;

using MongoDB.Bson;
using MongoDB.Web.Providers;

using MACSecurity;
using MACServices;

using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using sr = MACServices.Constants.ServiceResponse;
using cs = MACServices.Constants.Strings;

[WebService(Namespace = "http://mobileauthcorp.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

[System.Web.Script.Services.ScriptService]
public class RequestOtp : WebService
{
    private const string mSvcName = "RequestOtp";
    private const string mLogId = "RO";

    [WebMethod]
    public XmlDocument WsRequestOtp(string data)
    {
        var mUtils = new Utils();

        // request data Dictionary
        var myData = new Dictionary<string, string> { { dk.ServiceName, mSvcName } };

        // start the XML response
        var myResponse = new StringBuilder();
        mUtils.InitializeXmlResponse(myResponse);

        #region Decode or Decrypt request and Log
        if (data.StartsWith("99"))
        { // hex encoded
            var requestData = data.Substring(2, data.Length - 2); // dump the 99 from front

            // isloate key from data
            var request = mUtils.GetIdDataFromRequest(requestData);

            // parse string(data) and add to the dictionary
            if (mUtils.ParseIntoDictionary(mUtils.HexToString(request.Item2), myData, char.Parse(dk.KVSep)) ==
                false)
                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                    null, "requestData (Corrupt)" + Environment.NewLine + data, "99");
        }
        else
        {   // encrypted request data
            var request = mUtils.GetIdDataFromRequest(data);

            if (String.IsNullOrEmpty(request.Item1))
                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                    null, request.Item2 + Environment.NewLine + data, "1");

            // decrypt, parse string and add to the dictionary
            if (mUtils.DecryptAndParseRequestData(request.Item1, request.Item2, myData, char.Parse(dk.KVSep)) ==
                false)
                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                    null, "requestData (Corrupt)" + Environment.NewLine + data, "2");
        }

        // log request if debug set in web.config
        var eid = mUtils.LogRequest(myData, data, mLogId);
        #endregion

        #region Validate Request
        if (!myData.ContainsKey(dk.CID))
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, null, "requestData (no CID)", "3");

        if (myData.ContainsKey(dk.Request) == false)
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, myData[dk.CID], "requestData (Corrupt)", "5");

        #endregion

        #region Get Client
        var myClient = mUtils.GetClientUsingClientId(myData[dk.CID]);
        if (myClient == null)
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,  null, "Invalid Client Id", "4");

        if (myData.ContainsKey(dk.ClientName) == false)
            myData.Add(dk.ClientName, myClient.Name);

        #endregion

        #region Operational Test Ping
        if (myData[dk.Request] == dv.Ping)
        {
            myResponse.Append("<" + sr.Reply + ">" + sr.Success +  "</" + sr.Reply + ">");
            return mUtils.FinalizeXmlResponse(myResponse, mLogId);
        }
        #endregion

        #region Get Debug, LoopBackTest settings from config
        //string mLBT;
        try
        {
            var mLBT = ConfigurationManager.AppSettings[cfg.LoopBackTest].ToString();
            myData.Add(dk.LoopBackTest, mLBT);
        }
        catch
        {
            myData.Add(dk.LoopBackTest, cfg.Disabled);
        }
        bool mDebug;
        try
        {
            mDebug = Convert.ToBoolean(ConfigurationManager.AppSettings[cfg.Debug]);
        }
        catch
        {
            mDebug = false;
        }

        #endregion

        #region Send Otp Admin
        if (myData[dk.Request] == dv.SendOtpAdmin)
        {
            try
            { // Create Otp Object
                var mySendOtpAdminEvent = new Event();

                var maxRetries = myClient.OtpSettings.MaxRetries;
                if (maxRetries == 0) maxRetries = 3;
                var myOtp = new Otp(myData[dk.CID])
                {
                    Name = dv.Admin + " login",
                    ValidationRetries = maxRetries
                };

                var myProvider = new MongoDbMembershipProvider();
                int count;
                var toEmail = "";
                var toPhone = "";
                var userFullName = "";
                var userFirstName = "";

                //var myUserCollection = myProvider.FindUsersByEmail(myData[dk.UserId].Trim(), 0, 1, out count);\
                var myUserCollection = myProvider.FindUsersByName(myData[dk.UserId].Trim(), 0, 1, out count);

                foreach (MembershipUser myUser in myUserCollection)
                {
                    if (myUser.ProviderUserKey != null)
                    {
                        var userProfile = new UserProfile(myUser.ProviderUserKey.ToString());
                        myOtp.UserOtpOutAd = true; // no ads to admin
                        myOtp.UserId = myUser.ProviderUserKey.ToString();
                        myOtp.ToPhone = userProfile.Contact.MobilePhone;
                        myOtp.ToEmail = userProfile.Contact.Email;
                        myOtp.FirstTimeCarrierInfoSent = userProfile.FirstTimeCarrierInfoSent;
                        if (userProfile.FirstTimeCarrierInfoSent == false)
                        {
                            userProfile.FirstTimeCarrierInfoSent = true;
                            userProfile.Update();
                        }
                        userFirstName = userFullName = Security.DecodeAndDecrypt(userProfile.FirstName, myUser.ProviderUserKey.ToString());
                        userFullName += " ";
                        userFullName += Security.DecodeAndDecrypt(userProfile.LastName, myUser.ProviderUserKey.ToString());

                        toPhone = Security.DecodeAndDecrypt(myOtp.ToPhone, myUser.ProviderUserKey.ToString());
                        toEmail = Security.DecodeAndDecrypt(myOtp.ToEmail, myUser.ProviderUserKey.ToString());
                    }
                    // include any comments
                    if (myData.ContainsKey(dk.Comment)) myOtp.TrxDetail = mUtils.HexToString(myData[dk.Comment]);

                    myOtp.TrxType = 5; // use Admin login document template

                    //======== Send OTP =====================================================================
                    if (userFirstName == cs.QAUserFirstName)
                    {
                        myOtp.Code = mUtils.GenerateOtpCode(myClient);
                        myOtp.DeliveryMethod = cs.AdminQAUserDeliveryMethod;
                    }
                    else
                    {
                        var ret1 = mUtils.SendMessageToEndUser(myClient, myOtp);
                        if (ret1.Item1 == false)
                            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                                myData[dk.CID], "Error: " + ret1.Item2, null);
                    }
                    mySendOtpAdminEvent.EventTypeDesc = Constants.TokenKeys.EndUserId + userFullName
                                        + Constants.TokenKeys.DeliveryMethod + myOtp.DeliveryMethod
                                        + Constants.TokenKeys.ClientName + myData[dk.ClientName];

                    if (myOtp.DeliveryMethod.ToLower() == "email")
                        mySendOtpAdminEvent.EventTypeDesc += Constants.TokenKeys.SentToAddress + toEmail;
                    else
                    {
                        mySendOtpAdminEvent.EventTypeDesc += Constants.TokenKeys.SentToAddress + toPhone;
                    }

                    mySendOtpAdminEvent.EventTypeDesc += Constants.TokenKeys.RequestId + myOtp._id.ToString();

                    mySendOtpAdminEvent.ClientId = ObjectId.Parse(myData[dk.CID]);
                    if (myUser.ProviderUserKey != null)
                        mySendOtpAdminEvent.UserId = ObjectId.Parse(myUser.ProviderUserKey.ToString());

                    if (myData[dk.CID] == Constants.Strings.DefaultClientId) // This is a console login request
                        mySendOtpAdminEvent.Create(Constants.EventLog.Otp.RequestAdmin, null);
                    else // This is a Client OTP test
                        mySendOtpAdminEvent.Create(Constants.EventLog.Otp.RequestClientTest, null);

                    // change password
                    myUser.ChangePassword(myUser.GetPassword(), myOtp.Code);

                    myOtp.CodeHistory.Add(mySendOtpAdminEvent);

                    //========  Write the Otp object to the database =======================================
                    mUtils.ObjectCreate(myOtp);

                    //========  Create the response ========================================================
                    myResponse.Append("<" + sr.Reply + ">" +
                        dk.ItemSep + sr.RequestId + "=" + myOtp._id +
                        dk.ItemSep + sr.DeliveryMethod + "=" + myOtp.DeliveryMethod +
                        dk.ItemSep + sr.OTP + "=" + myOtp.Code +
                        "</" + sr.Reply + ">");

                    return mUtils.FinalizeXmlResponse(myResponse, mLogId);
                }
            }
            catch (Exception ex)
            {
                var mDetails = mSvcName + "." + dv.SendOtpAdmin + " " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";");
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                if (myData.ContainsKey(dk.ClientName))
                    exceptionEvent.EventTypeDesc += Constants.TokenKeys.ClientName + myData[dk.ClientName];
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
                // return error to requester
                return mUtils.FinalizeXmlResponseWithError(mSvcName + "." + myData[dk.CID] + "12", "TW");
            }
        }
        #endregion  
        
        #region Check Ip
        var mResult = mUtils.CheckClientIp(myClient);
        if (mResult.Item1 == false)
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, myData[dk.CID], mResult.Item2, null);
        #endregion

        #region Send OTP to End User

        if (myData[dk.Request] == dv.SendOtp)
        {
            try
            {
                // create an Otp event and attached it to the OTP object
                var mSendOtpEvent = new Event { ClientId = myClient.ClientId };

                //------ if group included in request data, check group status -----
                if (myData.ContainsKey(dk.GroupId))
                {
                    Group myGroup = null;
                    // does client belong to the group
                    if (
                        myClient.Relationships.Where(mRelationship => mRelationship.MemberType == "Group")
                            .Any(mRelationship => mRelationship.MemberId.ToString() == myData[dk.GroupId]))
                    {
                        myGroup = mUtils.GetGroupUsingGroupId(myData[dk.GroupId]);
                    }
                    if (myGroup == null)
                        return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                            myData[dk.CID], "Invalid GroupId " + myData[dk.GroupId], "6");

                    if (myGroup.Enabled == false)
                        return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                            myData[dk.CID], "Group disabled " + myData[dk.GroupId], "7");
                }

                var maxRetries = myClient.OtpSettings.MaxRetries;
                if (maxRetries == 0) maxRetries = 3;
                var myOtp = new Otp(myData[dk.CID]) { Name = "Send", ValidationRetries = maxRetries };

                // Move the AdPass request details from otp request to OTP object
                mUtils.SetAdSelectionSpecifics(myClient, myOtp, myData);
                
                // Save Reply Url in OTP object if in the input parameters
                if (myData.ContainsKey(dk.ReplyUri))
                    myOtp.ReplyUri = myData[dk.ReplyUri];

                // transaction details, text appended to text message after the Otp
                if (myData.ContainsKey(dk.TrxDetails))
                    myOtp.TrxDetail = mUtils.HexToString(myData[dk.TrxDetails]);

                // Transaction type, used to select document template
                if (myData.ContainsKey(dk.TrxType))
                {
                    try
                    {
                        myOtp.TrxType = Convert.ToInt32(myData[dk.TrxType]);
                    }
                    catch
                    {
                        myOtp.TrxType = 0;
                        mSendOtpEvent.EventTypeDesc += "invalid " + dk.TrxType + ", "; 
                    }
                }
                else
                {
                    myOtp.TrxType = 0;
                    mSendOtpEvent.EventTypeDesc += "no " + dk.TrxType + " in request, "; 
                }

                string requestType;
                if (myData.ContainsKey(dk.UserId))
                {
                    // Registered end user
                    string mHashedId;
                    // is this a group request (request data conatins a group Id
                    if (myData.ContainsKey(dk.GroupId))
                    {
                        requestType = cs.Otp.RequestTypeGroupRestricted;
                        mHashedId = Security.GetHashString(myData[dk.UserId] + myData[dk.GroupId]);
                    }
                    else
                    {
                        requestType = cs.Otp.RequestTypeClientGroupRestricted;
                        mHashedId = Security.GetHashString(myData[dk.UserId] + myData[dk.CID]);

                    }
                    var myEndUser = mUtils.GetEndUserByHashedUserId(mHashedId);
                    if (myEndUser == null)
                    {   //------- Open Check -----------------------------------------------
                        // if not registered with this client or group
                        // check if this client is able to access the open registered users
                        if (myClient.OpenAccessServicesEnabled == false)
                            return mUtils.FinalizeXmlResponseWithError("Unregistered End User", mLogId);

                        requestType = cs.Otp.RequestTypeOpen;
                        var myOpenRequest = new Dictionary<string, string>
                        {
                            {dk.UserId, myData[dk.UserId]},
                            {dk.Request, dv.GetEndUserInfo}
                        };
                        var endUserInfo = mUtils.ServiceRequest(
                            ConfigurationManager.AppSettings[cfg.MacOpenServicesUrl] +
                            Constants.ServiceUrls.MacOpenEndUserServices,
                            myData[dk.CID], myOpenRequest);
                        if (endUserInfo.Item1 == false)
                            return mUtils.FinalizeXmlResponseWithError(endUserInfo.Item2, mLogId);

                        // if true (user record found) the user record is encrypted.
                        var reply = Security.DecodeAndDecrypt(endUserInfo.Item2, myData[dk.CID]);
                        var results = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(reply);
                        //result contains all the info in the end user record
                        myOtp.UserOtpOutAd = results.OtpOutAd;
                        myOtp.UserId = results.HashedUserId;
                        myOtp.UserName = results.FirstName;
                        myOtp.ToPhone = results.Phone;
                        myOtp.ToEmail = results.Email;
                        myOtp.FirstTimeCarrierInfoSent = results.FirstTimeCarrierInfoSent;
                    }
                    else
                    {
                        // user found in this database
                        if (myEndUser.Active == false)
                            return mUtils.FinalizeXmlResponseWithError("End User (Inactive)", mLogId);

                        myOtp.UserId = myEndUser.HashedUserId;
                        mSendOtpEvent.UserId = myEndUser._id;
                        myOtp.UserOtpOutAd = myEndUser.OtpOutAd;
                        myOtp.ToPhone = myEndUser.Phone;
                        myOtp.ToEmail = myEndUser.Email;
                        myOtp.UserName = myEndUser.FirstName;
                        myOtp.FirstTimeCarrierInfoSent = myEndUser.FirstTimeCarrierInfoSent;
                    }

                    // user is registered so the AdPassOption if present over rides end user setting
                    if (myData.ContainsKey(dk.AdPassOption))
                    {
                        if (myClient.AdEnabled)
                        {
                            // enable ad in request over rides end user setting
                            if (myData[dk.AdPassOption] == dv.AdEnable)
                                myOtp.UserOtpOutAd = false;
                            else if (myData[dk.AdPassOption] == dv.AdDisable)
                                myOtp.UserOtpOutAd = true;
                        }
                    }
                }
                else // no UserId supplied, must be a request to send the Otp to a Client managed user
                {
                    requestType = cs.Otp.RequestTypeClientManaged;
                    myOtp.UserName = String.Empty;
                    if (myData.ContainsKey(dkui.EmailAddress) && myData.ContainsKey(dkui.PhoneNumber))
                    {
                        myOtp.UserId = ""; // no end user id
                        myOtp.ToEmail = myData[dkui.EmailAddress];
                        if (mUtils.ValidateEmailAddress(myOtp.ToEmail) == false)
                            return mUtils.FinalizeXmlResponseWithError("Invalid email: " + myOtp.ToEmail, mLogId);

                        myOtp.ToPhone = myData[dkui.PhoneNumber];
                        if (mUtils.ValidatePhoneNumber(myOtp.ToPhone) == false)
                            return mUtils.FinalizeXmlResponseWithError("invalid phone number: " + myOtp.ToPhone, mLogId);
                    }
                    else
                    {
                        return mUtils.FinalizeXmlResponseWithError("Requires user's email and phone number", mLogId);
                    }
                    //  and the Ad opt-out option
                    if (myData.ContainsKey(dk.AdPassOption))
                    {
                        if (myClient.AdEnabled)
                        {
                            if (myData[dk.AdPassOption] == dv.AdDisable)
                                myOtp.UserOtpOutAd = true;
                            else
                                myOtp.UserOtpOutAd = false;
                        }
                    }
                }
                myOtp.RequestType = requestType;
                mSendOtpEvent.EventTypeDesc = Constants.TokenKeys.RequestId + myOtp._id.ToString()
                                + Constants.TokenKeys.DeliveryMethod + myOtp.DeliveryMethod 
                                +  " (" + myData[dk.LoopBackTest] + ") "
                                + Constants.TokenKeys.RequestType + requestType
                                + Constants.TokenKeys.ClientName + myData[dk.ClientName];
                mSendOtpEvent.EventTypeId = Constants.EventLog.Otp.Sent.Item1;
                mSendOtpEvent.EventTypeName = Constants.EventLog.Otp.Sent.Item2;

                //======== Send OTP ===================================================================
                var ret = mUtils.SendMessageToEndUser(myClient, myOtp);
                if (ret.Item1 == false)
                {
                    if (ret.Item2.Contains(sr.STOP) == false)
                        return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, myData[dk.CID], ret.Item2, null);

                    // set end user status so next text message contains the carrier information.
                    UpdateEndUserIfFirstSMS(myOtp, myData[dk.CID], dv.False);

                    // return error and short code in details node
                    myResponse.Append("<" + sr.Error + ">" + (ret.Item2) + "</" + sr.Error + ">");
                    //isolate shortcode -- "Error: Not sent, Blocked user replied 'STOP' (FromNumber=123456)"
                    const string sckey = sr.FromNumber + "=";
                    if (ret.Item2.Contains(sckey))
                    {
                        var parts = ret.Item2.Split('(');
                        foreach (var part in parts)
                        {
                            if (part.StartsWith(sckey))
                            {
                                var subparts = part.Split(')');
                                foreach (var subpart in subparts)
                                {
                                    if (subpart.StartsWith(sckey))
                                    {
                                        myResponse.Append("<" + sr.Details + ">");
                                        myResponse.Append(subpart.Replace(sckey, ""));
                                        myResponse.Append("</" + sr.Details + ">");
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                    }
                    var rtn = mUtils.FinalizeXmlResponse(myResponse, mLogId);
                    return rtn;
                }

                //======== Update EndUser if this is not client managed and the 1st sms message sent
                if (myOtp.DeliveryMethod.Contains(cs.Sms) && myOtp.RequestType != cs.Otp.RequestTypeClientManaged)
                {
                    if (myOtp.FirstTimeCarrierInfoSent == false)
                        mSendOtpEvent.EventTypeDesc += UpdateEndUserIfFirstSMS(myOtp, myData[dk.CID], dv.True);
                }
                myOtp.CodeHistory.Add(mSendOtpEvent);
                //========  Write the Otp object to the database =======================================
                mUtils.ObjectCreate(myOtp);

                //========  Create the response ========================================================
                {  // Format response as XML containing delimited string (default)
                    // Reply
                    myResponse.Append("<" + sr.Reply + ">");
                    {
                        // return the request Id
                        myResponse.Append(dk.RequestId + dk.KVSep + myOtp._id.ToString());

                        // If the ad service was called and returned ads, include the ads (hex encoded)
                        if (String.IsNullOrEmpty(myOtp.AdDetails.EnterOTPAd) == false)
                            myResponse.Append(dk.ItemSep + sr.EnterOTPAd + dk.KVSep +
                                              mUtils.StringToHex(myOtp.AdDetails.EnterOTPAd));

                        //if (String.IsNullOrEmpty(myOtp.AdDetails.VerificationAd) == false)
                        //    myResponse.Append(dk.ItemSep + sr.ContentAd + dk.KVSep + mUtils.StringToHex(myOtp.AdDetails.VerificationAd));
                    }
                    myResponse.Append("</" + sr.Reply + ">");

                    // RequestId
                    myResponse.Append("<" + sr.RequestId + ">" + myOtp._id.ToString() + "</" + sr.RequestId + ">");

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
                    
                    if (mDebug ||
                        (myData[dk.LoopBackTest] != cfg.Disabled) ||
                        (!String.IsNullOrEmpty(myOtp.UserName) &&
                        (Security.DecodeAndDecrypt(myOtp.UserName, myOtp.UserId) == cs.QAUserFirstName))
                        )
                    {   // if debug is true
                        // or loopback is not Disabled (web.Config)
                        // or End User's First name is the QA User
                        // then return Debug Node containing the OTP code.
                        myResponse.Append("<" + sr.Debug + ">");
                        myResponse.Append(sr.OTP + dk.KVSep + myOtp.Code);
                        myResponse.Append("</" + sr.Debug + ">");
                    }
                    var rtn = mUtils.FinalizeXmlResponse(myResponse, mLogId);
                    return rtn;
                }
            }
            catch (Exception ex)
            {
                var mDetails = mSvcName + "." + dv.SendOtp + " " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";");
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                if (myData.ContainsKey(dk.ClientName))
                    exceptionEvent.EventTypeDesc += Constants.TokenKeys.ClientName + myData[dk.ClientName];
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
                // return error to requester
                return mUtils.FinalizeXmlResponseWithError(mSvcName + "." + myData[dk.CID] +"[" + eid + "]", mLogId);
            }
        }
        #endregion

        #region Resend Otp
        if (myData[dk.Request] == dv.ResendOtp)
        { //------------ ResendOtp Otp ----------------
            try
            {
                if (!myData.ContainsKey(dk.RequestId))
                    return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                        myData[dk.CID], "Request Data missing RequestId", "30");

                // get OTP object using the Request Id
                var myOtp = mUtils.GetOtpUsingRequestId(myData[dk.RequestId]);
                if (myOtp == null)
                    return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                        myData[dk.CID], "Invalid request Id[" + myData[dk.RequestId] + "]", "31");

                // has to be from the same client/Group that requested the OTP
                if (!String.Equals(myOtp.ClientId.ToString(), myData[dk.CID], StringComparison.CurrentCultureIgnoreCase))
                    return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                        myData[dk.CID], "Client Id (Invalid for this OTP)", "32");

                var mResendOtpEvent = new Event
                {
                    EventTypeDesc = mSvcName,
                    ClientId = myClient.ClientId,

                };

                if (myOtp.Active == false)
                { // not active
                    mResendOtpEvent.EventTypeDesc += " Disabled can not resend";
                    myOtp.CodeHistory.Add(mResendOtpEvent);
                    mUtils.ObjectUpdate(myOtp, myOtp._id.ToString());
                    return mUtils.FinalizeXmlResponseWithError(mSvcName + ", Disabled can not resend", mLogId);
                }

                // Has the OTP timed out
                var myNow = DateTime.UtcNow;
                var myTill = myOtp.EndOfLife;
                if (myNow > myTill)
                { // timed out
                    mResendOtpEvent.EventTypeDesc += " Timeout can not resend";
                    myOtp.CodeHistory.Add(mResendOtpEvent);
                    myOtp.Active = false; // set so OTP can't be used again
                    mUtils.ObjectUpdate(myOtp, myOtp._id.ToString());
                    return mUtils.FinalizeXmlResponseWithError(mSvcName + ", Timeout can not resend", mLogId);
                }

                if (myOtp.ValidationCount >= myOtp.ValidationRetries)
                { // too many retries
                    mResendOtpEvent.EventTypeDesc += " Too Many Retries can not resend";
                    myOtp.CodeHistory.Add(mResendOtpEvent);
                    myOtp.Active = false; // set so OTP can't be used again
                    mUtils.ObjectUpdate(myOtp, myOtp._id.ToString());
                    return mUtils.FinalizeXmlResponseWithError(mSvcName + ", Too Many Retries can not resend", mLogId);
                }
                myOtp.ValidationCount++;

                // reset the life span
                var timeout = myClient.OtpSettings.Timeout;
                if (timeout == 0) timeout = 3;
                myOtp.EndOfLife = DateTime.UtcNow.AddMinutes(timeout);
                myOtp.TrxType = 99; // resend, set bypass message construction
                var ret1 = mUtils.SendMessageToEndUser(myClient, myOtp);
                if (ret1.Item1 == false)
                { // failed to send
                    mResendOtpEvent.EventTypeDesc += " " + myOtp.ErrorMsg;
                    myOtp.CodeHistory.Add(mResendOtpEvent);
                    mUtils.ObjectUpdate(myOtp, myOtp._id.ToString());
                    return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                        myData[dk.CID], mResendOtpEvent.EventTypeDesc, "35");
                }
                // no errors
                mResendOtpEvent.EventTypeDesc = Constants.TokenKeys.RequestType + myOtp.Name
                                + mResendOtpEvent.EventTypeDesc
                                + Constants.TokenKeys.DeliveryMethod + myOtp.DeliveryMethod
                                + " (" + myData[dk.LoopBackTest] + ") "
                                + Constants.TokenKeys.ClientName + myData[dk.ClientName];

                myOtp.CodeHistory.Add(mResendOtpEvent);
                // Write the Otp object to the database
                mUtils.ObjectUpdate(myOtp, myOtp._id.ToString());

                //========  Create the response ========================================================
                {  // Format as XML containing delimited string
                    // Reply 
                    myResponse.Append("<" + sr.Reply + ">");
                    {
                        // return the request Id
                        myResponse.Append(dk.RequestId + dk.KVSep + myOtp._id.ToString());

                        // If the ad service was called and returned ads, include the ads (hex encoded)
                        if (String.IsNullOrEmpty(myOtp.AdDetails.EnterOTPAd) == false)
                            myResponse.Append(dk.ItemSep + sr.EnterOTPAd + dk.KVSep +
                                              mUtils.StringToHex(myOtp.AdDetails.EnterOTPAd));

                        //if (String.IsNullOrEmpty(myOtp.AdDetails.VerificationAd) == false)
                        //    myResponse.Append(dk.ItemSep + sr.ContentAd + dk.KVSep + mUtils.StringToHex(myOtp.AdDetails.VerificationAd));
                    }
                    myResponse.Append("</" + sr.Reply + ">");

                    // RequestId
                    myResponse.Append("<" + dk.RequestId + ">" + myOtp._id.ToString() + "</" + dk.RequestId + ">");

                    // Details
                    myResponse.Append("<" + sr.Details + ">");
                    {
                        myResponse.Append(sr.Request + dk.KVSep + myData[dk.Request]);
                        myResponse.Append(dk.ItemSep + sr.Action + dk.KVSep + sr.Resent);
                        myResponse.Append(dk.ItemSep + dk.ClientName + dk.KVSep + myClient.Name);
                        myResponse.Append(dk.ItemSep + sr.DeliveryMethod + dk.KVSep + myOtp.DeliveryMethod);
                        myResponse.Append(dk.ItemSep + sr.TLM + dk.KVSep + myClient.OtpSettings.Timeout);
                        myResponse.Append(dk.ItemSep + dk.OTPRetriesMax + dk.KVSep + myOtp.ValidationRetries);
                        myResponse.Append(dk.ItemSep + dk.OTPExpiredTime + dk.KVSep + myOtp.EndOfLife);
                    }
                    myResponse.Append("</" + sr.Details + ">");

                    // Debug mode or loopback or QA User
                    if (mDebug ||
                        (myData[dk.LoopBackTest] != cfg.Disabled) ||
                        (!String.IsNullOrEmpty(myOtp.UserName) &&
                        (Security.DecodeAndDecrypt(myOtp.UserName, myOtp.UserId) == cs.QAUserFirstName))
                        )
                    {   // if debug is true
                        // or loopback
                        // or End User's First name is the QA User
                        //  return Debug Node containing requestId and the OTP code.
                        //  User for load testing and Automated QA Testing
                        myResponse.Append("<" + sr.Debug + ">");
                        {
                            myResponse.Append(sr.OTP + dk.KVSep + myOtp.Code);
                        }
                        myResponse.Append("</" + sr.Debug + ">");
                    }
                    var rtn = mUtils.FinalizeXmlResponse(myResponse, mLogId);
                    return rtn;
                }
            }
            catch (Exception ex)
            {
                var mDetails = mSvcName + "." + dv.ResendOtp + " " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";");
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                if (myData.ContainsKey(dk.ClientName))
                    exceptionEvent.EventTypeDesc += Constants.TokenKeys.ClientName + myData[dk.ClientName];
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
                // return error to requester
                return mUtils.FinalizeXmlResponseWithError(mSvcName + "." + myData[dk.CID] + "36", mLogId);
            }
        }

        #endregion

        #region Cancel Otp

        if (myData[dk.Request] == dv.CancelOtp)
        {
            // ------------- Cancel Otp -------------------
            try
            {
                if (myData.ContainsKey(dk.RequestId))
                {
                    var mCancelOtpEvent = new Event
                    {
                        EventTypeDesc = Constants.TokenKeys.RequestType + "Cancel Otp",
                        ClientId = myClient.ClientId
                    };

                    // get Otp object using the Request Id
                    var myOtp = mUtils.GetOtpUsingRequestId(myData[dk.RequestId]);
                    if (myOtp != null)
                    {
                        // Was Otp validated or did user retry validation too many times
                        if (myOtp.Active == false)
                        {
                            myOtp.Active = false; // set so Otp can't be used again
                            mCancelOtpEvent.Create(Constants.EventLog.Otp.Cancelled, "");
                            myOtp.CodeHistory.Add(mCancelOtpEvent);

                            // Update the Otp object in the database
                            mUtils.ObjectUpdate(myOtp, myOtp._id.ToString());
                        }
                    }
                    mCancelOtpEvent.EventTypeDesc += Constants.TokenKeys.ClientName + myData[dk.ClientName];
                }
                myResponse.Append("<" + sr.Reply + ">" + Constants.ServiceResponse.Canceled + "</" + sr.Reply + ">");
                return mUtils.FinalizeXmlResponse(myResponse, mLogId);
            }
            catch (Exception ex)
            {
                var mDetails = mSvcName + "." + dv.CancelOtp + " " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";");
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                if (myData.ContainsKey(dk.ClientName))
                    exceptionEvent.EventTypeDesc += Constants.TokenKeys.ClientName + myData[dk.ClientName];
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
                // return error to requestor
                return mUtils.FinalizeXmlResponseWithError(mSvcName + "." + myData[dk.CID] + "40", mLogId);
            }
        }

        #endregion

        #region SendMessage

        if (myData[dk.Request] == dv.SendMessage)
        { // ------------- Send Message -------------------
            try
            {
                var mSendMsgEvent = new Event();
                // Create Otp Object to use for sending the message
                var myOtp = new Otp(myData[dk.CID]);
                // decode message from request
                if (myData.ContainsKey(dk.Message))
                    myOtp.Message = mUtils.HexToString(myData[dk.Message]);

                // Move the AdPass request details from otp request to OTP object
                mUtils.SetAdSelectionSpecifics(myClient, myOtp, myData);

                if (myData.ContainsKey(dk.UserId))
                {
                    //------------ Registered end user -----------------
                    myOtp.Name = "Registered";
                    // Registered end user
                    string mHashedId;
                    // is this a group request (request data conatins a group Id
                    if (myData.ContainsKey(dk.GroupId))
                    {
                        mHashedId = Security.GetHashString(myData[dk.UserId] + myData[dk.GroupId]);
                    }
                    else
                    {
                        mHashedId = Security.GetHashString(myData[dk.UserId] + myData[dk.CID]);
                    }
                    var myEndUser = mUtils.GetEndUserByHashedUserId(mHashedId);
                    if (myEndUser == null)
                    {
                        //------- Open Check -----------------------------------------------
                        // if not registered with this client or group
                        // check if this client is able to access the open registered users
                        if (myClient.OpenAccessServicesEnabled == false)
                            return mUtils.FinalizeXmlResponseWithError("Unregistered End User", mLogId);

                        var myOpenRequest = new Dictionary<string, string>
                        {
                            {dk.UserId, myData[dk.UserId]},
                            {dk.Request, dv.GetEndUserInfo}
                        };

                        var endUserInfo = mUtils.ServiceRequest(
                            ConfigurationManager.AppSettings[cfg.MacOpenServicesUrl] +
                            Constants.ServiceUrls.MacOpenEndUserServices,
                            myData[dk.CID], myOpenRequest);
                        if (endUserInfo.Item1 == false)
                            return mUtils.FinalizeXmlResponseWithError("Unregistered End User|" + endUserInfo.Item2, mLogId);

                        // if true (user record found) the user record is encrypted.
                        var reply = Security.DecodeAndDecrypt(endUserInfo.Item2, myData[dk.CID]);
                        var results = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(reply);
                        //result contains all the info in the end user record
                        myOtp.UserOtpOutAd = results.OtpOutAd;
                        myOtp.UserId = results.HashedUserId;
                        myOtp.UserName = results.FirstName;
                        myOtp.ToPhone = results.Phone;
                        myOtp.ToEmail = results.Email;
                        myOtp.FirstTimeCarrierInfoSent = results.FirstTimeCarrierInfoSent;
                    }
                    else
                    {
                        // user found in this database
                        if (myEndUser.Active == false)
                            return mUtils.FinalizeXmlResponseWithError("End User (Inactive)", mLogId);

                        myOtp.UserId = myEndUser.HashedUserId;
                        mSendMsgEvent.UserId = myEndUser._id;
                        myOtp.UserOtpOutAd = myEndUser.OtpOutAd;
                        myOtp.ToPhone = myEndUser.Phone;
                        myOtp.ToEmail = myEndUser.Email;
                        myOtp.UserName = myEndUser.FirstName;
                        myOtp.FirstTimeCarrierInfoSent = myEndUser.FirstTimeCarrierInfoSent;
                    }
                }
                else // no UserId supplied, must be a request to send the OTP to a Client managed user
                {
                    myOtp.UserId = ""; // no end user id
                    if (myData.ContainsKey(dkui.EmailAddress) && myData.ContainsKey(dkui.PhoneNumber))
                    {
                        myOtp.Name = "Client Managed";
                        myOtp.UserName = String.Empty;
                        myOtp.ToEmail = myData[dkui.EmailAddress];
                        if (mUtils.ValidateEmailAddress(myOtp.ToEmail) == false)
                            return mUtils.FinalizeXmlResponseWithError(myOtp.Name + ", invalid email: " + myOtp.ToEmail, mLogId);

                        myOtp.ToPhone = myData[dkui.PhoneNumber];
                        if (mUtils.ValidatePhoneNumber(myOtp.ToPhone) == false)
                            if (mUtils.ValidatePhoneNumber(myOtp.ToPhone) == false)
                                return mUtils.FinalizeXmlResponseWithError(myOtp.Name + ", invalid phone number: " + myOtp.ToPhone, mLogId);
                        // Since the user is not registered the client must provide:
                        //  the 1st time flag 
                        if (myData.ContainsKey(dk.SendCarrierInfo) || myData[dk.SendCarrierInfo] == dv.False)
                            myOtp.FirstTimeCarrierInfoSent = false;
                        else
                            myOtp.FirstTimeCarrierInfoSent = true;
                    }
                    else
                    {
                        return mUtils.FinalizeXmlResponseWithError(myOtp.Name + ", Requires user's email and phone number", mLogId);
                    }
                }
                myOtp.TrxDetail = mUtils.HexToString(myData[dk.Message]); // the message
                myOtp.TrxType = 0; // Use Generic message template

                //======== Send Message ===============================================================
                var ret = mUtils.SendMessageToEndUser(myClient, myOtp);
                if (ret.Item1 == false)
                    return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, myData[dk.CID], ret.Item2, null);

                mSendMsgEvent.EventTypeDesc = Constants.TokenKeys.RequestType + myOtp.Name
                                + mSendMsgEvent.EventTypeDesc
                              + Constants.TokenKeys.ClientName + myData[dk.ClientName];

                //======== Update EndUser if this is not client managed and the 1st sms message sent
                if (myOtp.DeliveryMethod.Contains(cs.Sms) && myOtp.RequestType != cs.Otp.RequestTypeClientManaged)
                {
                    if (myOtp.FirstTimeCarrierInfoSent == false)
                        mSendMsgEvent.EventTypeDesc += UpdateEndUserIfFirstSMS(myOtp, myData[dk.CID], dv.True);
                }

                //========  Create the response ========================================================
                myResponse.Append("<" + sr.Reply + ">" + Constants.ServiceResponse.Sent + "</" + sr.Reply + ">");
                // include ads if present in otp object
                if (String.IsNullOrEmpty(myOtp.AdDetails.EnterOTPAd) == false)
                    myResponse.Append("<" + sr.EnterOTPAd + ">" + mUtils.StringToHex(myOtp.AdDetails.EnterOTPAd) + "</" + sr.EnterOTPAd + ">");
                //if (String.IsNullOrEmpty(myOtp.AdDetails.VerificationAd) == false)
                //    myResponse.Append(dk.ItemSep + sr.ContentAd + dk.KVSep + mUtils.StringToHex(myOtp.AdDetails.VerificationAd));

                // Details
                myResponse.Append("<" + sr.Details + ">");
                {
                    myResponse.Append(sr.Request + dk.KVSep + myData[dk.Request]);
                    myResponse.Append(dk.ItemSep + sr.Action + dk.KVSep + sr.Sent);
                    myResponse.Append(dk.ItemSep + dk.ClientName + dk.KVSep + myClient.Name);
                    myResponse.Append(dk.ItemSep + sr.DeliveryMethod + dk.KVSep + myOtp.DeliveryMethod);
                }
                myResponse.Append("</" + sr.Details + ">");
                return mUtils.FinalizeXmlResponse(myResponse, mLogId);
            }
            catch (Exception ex)
            {
                var mDetails = mSvcName + "." + dv.SendMessage + " " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";");
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                if (myData.ContainsKey(dk.ClientName))
                    exceptionEvent.EventTypeDesc += Constants.TokenKeys.ClientName + myData[dk.ClientName];
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
                // return error to requester
                return mUtils.FinalizeXmlResponseWithError(mSvcName + "." + myData[dk.CID] + "58", mLogId);
            }
        }
        #endregion

        return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
            null, " Invalid request[" + myData[dk.Request] + "], " + eid, null);
    }

    protected string UpdateEndUserIfFirstSMS(Otp pOtp, String pCID, string pNewCondition)
    {
        var mUtils = new Utils();
        if (pOtp.RequestType == cs.Otp.RequestTypeOpen)
        {
            // Open so ask the OAS Service to update the user record
            var myOpenRequest = new Dictionary<string, string>
            {
                {dk.UserId, pOtp.UserId},
                {dk.Request, dv.UpdateEndUser},
                {dk.SetFirstTimeCarrierInfoSent, pNewCondition}
            };
            var oasResponse = mUtils.ServiceRequest(
                ConfigurationManager.AppSettings[cfg.MacOpenServicesUrl] +
                Constants.ServiceUrls.MacOpenEndUserServices,
                pCID, myOpenRequest);
            if (oasResponse.Item1 == false)
                return oasResponse.Item2;

            //// if true (user record found) the user record is encrypted.
            //var reply = Security.DecodeAndDecrypt(oasResponse.Item2, pCID);
            //var results = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(reply);
            return @"End User Updated!"; 
        }

        // not open so user shouldbe in local database
        var myEndUser = mUtils.GetEndUserByHashedUserId(pOtp.UserId);
        if (myEndUser == null) return " could not update end user ";
        myEndUser.FirstTimeCarrierInfoSent = true;
        myEndUser.Update();
        return @"End User Updated!";
    }
}
