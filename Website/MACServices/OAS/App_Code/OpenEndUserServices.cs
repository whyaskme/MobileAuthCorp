using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web.Services;
using System.Threading;

using MACSecurity;
using MACServices;
using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using sr = MACServices.Constants.ServiceResponse;
using dkui = MACServices.Constants.Dictionary.Userinfo;

[WebService(Namespace = "http://mobileauthcorp.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

[System.Web.Script.Services.ScriptService]
public class OpenEndUserServices : WebService
{
    private const string mSvcName = "OpenEndUserServices";
    private const string mLogId = "OE";

    [WebMethod]
    public XmlDocument WsOpenEndUserServices(string data)
    {
        var mUtils = new Utils();

        Dictionary<string, string> myData;
        // start the XML response
        
        var myResponse = new StringBuilder();
        mUtils.InitializeXmlResponse(myResponse);

        #region Decrypt and log request
        var request = mUtils.GetIdDataFromRequest(data);
        if (String.IsNullOrEmpty(request.Item1))
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, null, request.Item2 + Environment.NewLine + data, null);

        try
        {
            var sd = Security.DecodeAndDecrypt(request.Item2, request.Item1);
            myData = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(sd);
            myData.Remove(dk.ServiceName);
            myData.Add(dk.ServiceName, mSvcName);
        }
        catch (Exception ex)
        {
            var mDetails = mSvcName + ".decrypt" + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
            var exceptionEvent = new Event
            {
                EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
            };
            exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

            return mUtils.FinalizeXmlResponseWithError(
                mSvcName + exceptionEvent.EventTypeDesc + Environment.NewLine + data, mLogId);
        }

        // log request if debug set in web.config
        var eid = mUtils.LogRequest(myData, data, mLogId);

        #endregion

        #region Validate request

        if (!myData.ContainsKey(dk.Request))
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, null, "Request type required!" + eid, null);
        
        if (!myData.ContainsKey(dk.UserId))
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, request.Item1,
                "no end user id in requestData", null);
        #endregion

        #region Register End User
        if (myData[dk.Request].Contains(dv.Register))
        {
            try
            {
                var myOasClient = mUtils.GetOasClientByClientId(myData[dk.CID]);
                if (myOasClient == null)
                    return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, request.Item1, "Invalid OASClient " + myData[dk.CID] + ", " + eid, null);

                var ret = mUtils.Check4MininumEndUserInfo(myData);
                if (ret.Item1 == false)
                    return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, request.Item1, ret.Item2, null);

                if (myData.ContainsKey(dk.UserId) == false)
                {   // no User Id supplied crete one using last name and Unique Identifier
                    if (!myData.ContainsKey(dkui.UID))
                        myData.Add(dkui.UID, myData[dkui.EmailAddress]);

                    myData.Add(dk.UserId,
                                    Security.GetHashString(
                                        myData[dkui.LastName].ToLower() +   // user's last name
                                        myData[dkui.UID].ToLower()          // unique Identifier (normailly email address)
                                    ).ToUpper()
                                );
                }
                // check if user exists
                var endUser = mUtils.GetEndUserByHashedUserId(myData[dk.UserId]);
                if (endUser == null)
                {
                    var myEndUser = new EndUser();
                    ret = mUtils.PopulateEndUserObject(myEndUser, myData);
                    if (ret.Item1 == false)
                        return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, request.Item1,
                            ret.Item2, null);
                    myEndUser.Active = true;
                    myEndUser.State = Constants.EndUserStates.Registered;
                    var euEvent = new EndUserEvent(mSvcName +
                            ":" + myEndUser.RegistrationType +
                            ", State=" + myEndUser.State +
                            ", Ad Opt-Out=" + myEndUser.OtpOutAd);   
                    myEndUser.EndUserEvents.Add(euEvent);
                    myEndUser.Create();

                    var mHashedUserId = myEndUser.HashedUserId;
                    Thread.Sleep(100);
                    // check to see if database indexing has completed
                    var ckEndUser = mUtils.GetEndUserByHashedUserId(mHashedUserId);
                    if (ckEndUser == null) Thread.Sleep(1000);
                } 
                else if (endUser.Active == false)
                {
                    var euEvent = new EndUserEvent(mSvcName + ": Activated");
                    endUser.EndUserEvents.Add(euEvent);
                    endUser.Update();
                }
                myResponse.Append("<" + sr.Reply + ">" + Constants.ServiceResponse.Registered + "</" + sr.Reply + ">");
                return mUtils.FinalizeXmlResponse(myResponse, mLogId);
            }
            catch (Exception ex)
            {
                var mDetails = mSvcName + dv.Register + " " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";");
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                if (myData.ContainsKey(dk.ClientName))
                    exceptionEvent.EventTypeDesc += Constants.TokenKeys.ClientName + myData[dk.ClientName];
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null); 

                return mUtils.FinalizeXmlResponseWithError(
                    mSvcName + "." + myData[dk.Request] + exceptionEvent.EventTypeDesc, mLogId);
            }
        }
        #endregion

        #region Get End User Info

        if (myData[dk.Request] == dv.GetEndUserInfo)
        {
            try
            {
                var myEndUser = mUtils.GetEndUserByHashedUserId(myData[dk.UserId]);
                if (myEndUser == null)
                    return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, request.Item1, "Unregistered End User",
                        "5");

                if (myEndUser.Active == false)
                {
                    myResponse.Append("<" + sr.Reply + ">" + Constants.ServiceResponse.Inactive + "</" + sr.Reply +
                                      ">");
                }
                else
                {
                    var userinfo = Newtonsoft.Json.JsonConvert.SerializeObject(myEndUser);
                    var reply = Security.EncryptAndEncode(userinfo, request.Item1);
                    myResponse.Append("<" + sr.Reply + ">" + reply + "</" + sr.Reply + ">");
                }
                return mUtils.FinalizeXmlResponse(myResponse, mLogId);
            }
            catch (Exception ex)
            {
                var mDetails = mSvcName + dv.GetEndUserInfo + " " +
                               ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";");
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                if (myData.ContainsKey(dk.ClientName))
                    exceptionEvent.EventTypeDesc += Constants.TokenKeys.ClientName + myData[dk.ClientName];
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                return mUtils.FinalizeXmlResponseWithError(
                    mSvcName + "." + myData[dk.Request] + exceptionEvent.EventTypeDesc, mLogId);
            }
        }

        #endregion

        #region Activate End User
        if (myData[dk.Request] == dv.ActivateEndUser)
        {
            try {
                var myEndUser = mUtils.GetEndUserByHashedUserId(myData[dk.UserId]);
                if (myEndUser == null)
                    return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, null, "Unregistered End User", null);
                
                if (!myEndUser.Active)
                {
                    var euEvent = new EndUserEvent(mSvcName + ": Enabled");
                    myEndUser.EndUserEvents.Add(euEvent);
                    myEndUser.Active = true;
                    myEndUser.Update();
                }
                myResponse.Append("<" + sr.Details + ">End User Enable</" + sr.Details + ">");
                myResponse.Append("<" + sr.Reply + ">" + Constants.ServiceResponse.Success + "</" + sr.Reply + ">");
                return mUtils.FinalizeXmlResponse(myResponse, mLogId);
            }
            catch (Exception ex)
            {
                var mDetails = mSvcName + dv.Enable + " " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";");
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                if (myData.ContainsKey(dk.ClientName))
                    exceptionEvent.EventTypeDesc += Constants.TokenKeys.ClientName + myData[dk.ClientName];
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                return mUtils.FinalizeXmlResponseWithError(
                    mSvcName + "." + myData[dk.Request] + exceptionEvent.EventTypeDesc, mLogId);
            }
        }
        #endregion

        #region Deactivate End User
        if (myData[dk.Request] == dv.DeactivateEndUser)
        {
            try {
                var myEndUser = mUtils.GetEndUserByHashedUserId(myData[dk.UserId]);
                if (myEndUser == null)
                    return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, null, "Unregistered End User", null);
                if (myEndUser.Active)
                {
                    var euEvent = new EndUserEvent(mSvcName + ": Disabled");
                    myEndUser.EndUserEvents.Add(euEvent);
                    myEndUser.Active = false;
                    myEndUser.Update();
                }
                myResponse.Append("<" + sr.Details + ">End User Disable</" + sr.Details + ">");
                myResponse.Append("<" + sr.Reply + ">" + Constants.ServiceResponse.Success + "</" + sr.Reply + ">");
                return mUtils.FinalizeXmlResponse(myResponse, mLogId);
            }
            catch (Exception ex)
            {
                var mDetails = mSvcName + dv.Disable + " " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";");
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                if (myData.ContainsKey(dk.ClientName))
                    exceptionEvent.EventTypeDesc += Constants.TokenKeys.ClientName + myData[dk.ClientName];
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                return mUtils.FinalizeXmlResponseWithError(
                    mSvcName + "." + myData[dk.Request] + exceptionEvent.EventTypeDesc, mLogId);
            }
        }
        #endregion

        #region Update
        if (myData[dk.Request] == dv.UpdateEndUser)
        {
            try
            {
                var myEndUser = mUtils.GetEndUserByHashedUserId(myData[dk.UserId]);
                if (myEndUser == null)
                    return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, null, "Unregistered End User", null);

                var mUpdate = new StringBuilder();
                if (myData.ContainsKey(dkui.PhoneNumber))
                {
                    // todo: Need to match old phone number
                    var mNewPhone = myData[dkui.PhoneNumber];
                    if (mUtils.ValidatePhoneNumber(mNewPhone) == false)
                        return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                            myData[dk.CID], dv.Update + ", invalid phone number: " + mNewPhone, "17");
                    myEndUser.Phone = Security.EncryptAndEncode(mNewPhone, myEndUser.HashedUserId);
                    mUpdate.Append("," + dkui.PhoneNumber + "=" + mNewPhone);
                }

                if (myData.ContainsKey(dkui.DOB))
                {
                    myEndUser.DateOfBirth = Security.EncryptAndEncode(myData[dkui.DOB], myEndUser.HashedUserId);
                    mUpdate.Append("," + dkui.DOB + "=" + myData[dkui.DOB]);
                }
                if (myData.ContainsKey(dkui.Street))
                {
                    myEndUser.Address.Street1 = Security.EncryptAndEncode(myData[dkui.Street], myEndUser.HashedUserId);
                    mUpdate.Append("," + dkui.Street + "=" + myData[dkui.Street]);
                }
                if (myData.ContainsKey(dkui.Street2))
                {
                    myEndUser.Address.Street2 = Security.EncryptAndEncode(myData[dkui.Street2], myEndUser.HashedUserId);
                    mUpdate.Append("," + dkui.Street2 + "=" + myData[dkui.Street2]);
                }
                if (myData.ContainsKey(dkui.Unit))
                {
                    myEndUser.Address.Unit = Security.EncryptAndEncode(myData[dkui.Unit], myEndUser.HashedUserId);
                    mUpdate.Append("," + dkui.Unit + "=" + myData[dkui.Unit]);
                }
                if (myData.ContainsKey(dkui.City))
                {
                    myEndUser.Address.City = Security.EncryptAndEncode(myData[dkui.City], myEndUser.HashedUserId);
                    mUpdate.Append("," + dkui.City + "=" + myData[dkui.City]);
                }
                if (myData.ContainsKey(dkui.State))
                {
                    myEndUser.Address.State = Security.EncryptAndEncode(myData[dkui.State], myEndUser.HashedUserId);
                    mUpdate.Append("," + dkui.State + "=" + myData[dkui.State]);
                }
                if (myData.ContainsKey(dkui.ZipCode))
                {
                    myEndUser.Address.Zipcode = Security.EncryptAndEncode(myData[dkui.ZipCode], myEndUser.HashedUserId);
                    mUpdate.Append("," + dkui.ZipCode + "=" + myData[dkui.ZipCode]);
                }
                if (myData.ContainsKey(dkui.Country))
                {
                    myEndUser.Address.Country = Security.EncryptAndEncode(myData[dkui.Country], myEndUser.HashedUserId);
                    mUpdate.Append("," + dkui.Country + "=" + myData[dkui.Country]);
                }
                if (myData.ContainsKey(dk.SetFirstTimeCarrierInfoSent))
                {
                    if (myData[dk.SetFirstTimeCarrierInfoSent] == dv.True)
                    {
                        myEndUser.FirstTimeCarrierInfoSent = true;
                    }
                    else
                    {
                        myEndUser.FirstTimeCarrierInfoSent = false;
                    }
                    mUpdate.Append("," + dk.SetFirstTimeCarrierInfoSent + "=" + myEndUser.FirstTimeCarrierInfoSent);
                }
                // Log the event
                var euEvent = new EndUserEvent(mSvcName + ": " + myData[dk.Request] + ", " + mUpdate.ToString());
                myEndUser.EndUserEvents.Add(euEvent);
                myEndUser.Update();

                myResponse.Append("<" + sr.Reply + ">" + Constants.ServiceResponse.Success + "</" + sr.Reply + ">");
                myResponse.Append("<" + sr.Details + ">" + dv.UpdateEndUser + ", " + mUpdate.ToString() + "</" + sr.Details + ">");
                return mUtils.FinalizeXmlResponse(myResponse, mLogId);
            }
            catch (Exception ex)
            {
                var mDetails = mSvcName + dv.UpdateEndUser + " " +
                               ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";");
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                if (myData.ContainsKey(dk.ClientName))
                    exceptionEvent.EventTypeDesc += Constants.TokenKeys.ClientName + myData[dk.ClientName];
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                return mUtils.FinalizeXmlResponseWithError(
                    mSvcName + "." + myData[dk.Request] + exceptionEvent.EventTypeDesc, mLogId);
            }

        }
        #endregion

        #region Delete End User
        if (myData[dk.Request] == dv.DeleteEndUser)
        {
            try
            {
                var myEndUser = mUtils.GetEndUserByHashedUserId(myData[dk.UserId]);
                if (myEndUser == null)
                    return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, null, "Unregistered End User", null);
                mUtils.DeleteEndUser(myEndUser);
                myResponse.Append("<" + sr.Reply + ">" + Constants.ServiceResponse.Deleted + "</" + sr.Reply + ">");
                return mUtils.FinalizeXmlResponse(myResponse, mLogId);
            }
            catch (Exception ex)
            {
                var mDetails = mSvcName + dv.Disable + " " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";");
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                if (myData.ContainsKey(dk.ClientName))
                    exceptionEvent.EventTypeDesc += Constants.TokenKeys.ClientName + myData[dk.ClientName];
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                return mUtils.FinalizeXmlResponseWithError(
                    mSvcName + "." + myData[dk.Request] + exceptionEvent.EventTypeDesc, mLogId);
            }
        }
        #endregion

        #region Ad Pass
        if (myData[dk.Request] == dv.SetAdPassOption)
        {
            try {
                var myEndUser = mUtils.GetEndUserByHashedUserId(myData[dk.UserId]);
                if (myEndUser == null)
                    return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, null, "Unregistered End User",
                        null);
                if (myData.ContainsKey(dk.AdPassOption))
                {
                    switch (myData[dk.AdPassOption])
                    {
                        case dv.AdEnable:
                            myEndUser.OtpOutAd = false;
                            break;
                        case dv.AdDisable:
                            myEndUser.OtpOutAd = true;
                            break;
                    }
                    var euEvent = new EndUserEvent(mSvcName + ": Ad Pass " + myData[dk.AdPassOption]);
                    myEndUser.EndUserEvents.Add(euEvent);
                    myEndUser.Update();
                    myResponse.Append("<" + sr.Details + ">" + euEvent.Details + "</" + sr.Details + ">");
                    myResponse.Append("<" + sr.Reply + ">" + Constants.ServiceResponse.Success +
                                      "</" + sr.Reply + ">");
                    return mUtils.FinalizeXmlResponse(myResponse, mLogId);
                }
            }
            catch (Exception ex)
            {
                var mDetails = mSvcName + dv.SetAdPassOption + " " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";");
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                if (myData.ContainsKey(dk.ClientName))
                    exceptionEvent.EventTypeDesc += Constants.TokenKeys.ClientName + myData[dk.ClientName];
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                return mUtils.FinalizeXmlResponseWithError(
                    mSvcName + "." + myData[dk.Request] + exceptionEvent.EventTypeDesc, mLogId);
            }
        }

        #region ADSTOP / ADENABLE
        if ((myData[dk.Request] == dv.ADSTOP.ToLower()) || (myData[dk.Request] == dv.ADENABLE.ToLower()))
        {
            try
            {
                var mEndUser = mUtils.GetEndUserByHashedUserId(myData[dk.UserId]);
                if (mEndUser == null)
                    return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, null, "Unregistered End User", null);
                if (myData[dk.Request] == dv.ADSTOP.ToLower())
                    mEndUser.OtpOutAd = false;
                if (myData[dk.Request] == dv.ADENABLE.ToLower())
                    mEndUser.OtpOutAd = true;
                var euEvent = new EndUserEvent(mSvcName + ": AdPass " + myData[dk.Request]);
                mEndUser.EndUserEvents.Add(euEvent);
                mEndUser.Update();
                myResponse.Append("<" + sr.Reply + ">" 
                    + Constants.ServiceResponse.Success 
                    + "</" + sr.Reply + ">");
                return mUtils.FinalizeXmlResponse(myResponse, mLogId);
            }
            catch (Exception ex)
            {
                var mDetails = mSvcName + dv.SetAdPassOption + " " +
                               ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";");
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                if (myData.ContainsKey(dk.ClientName))
                    exceptionEvent.EventTypeDesc += Constants.TokenKeys.ClientName + myData[dk.ClientName];
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                return mUtils.FinalizeXmlResponseWithError(
                    mSvcName + "." + myData[dk.Request] + exceptionEvent.EventTypeDesc, mLogId);
            }
        }
        #endregion

        #endregion

        #region Invalid request
        return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, null, 
            "Invalid request[" + myData[dk.Request] + "], " + eid, null);
        #endregion
    }
}
