using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Web.Services;
using System.Xml;

using MACServices;
using sr = MACServices.Constants.ServiceResponse;
using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using es = MACServices.Constants.EventStats;

[WebService(Namespace = "http://mobileauthcorp.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

[System.Web.Script.Services.ScriptService]
public class StsEndUserRegistration : WebService
{
    public Object PropertyValue { get; set; }
    public virtual Type PropertyType { get; set; }

    private const string mSvcName = "StsEndUserRegistration";
    private const string mLogId = "SR";

    [WebMethod]
    public XmlDocument WsStsEndUserRegistration(string data)
    {
        Tuple<string, string> request;
        // request data Dictionary
        var myData = new Dictionary<string, string>();

        var mUtils = new Utils();
        // start the XML response
        var myResponse = new StringBuilder();
        mUtils.InitializeXmlResponse(myResponse);

        #region Decrypt/decode request

        if (data.StartsWith("99"))
        {
            var requestData = data.Substring(2, data.Length - 2); // dump the 99 from front

            // isloate ID from data
            request = mUtils.GetIdDataFromRequest(requestData);

            // parse string(data) and add to the dictionary
            var Data = mUtils.HexToString(request.Item2);
            if (mUtils.ParseIntoDictionary(Data, myData, char.Parse(dk.KVSep)) == false)
                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                    request.Item1, "Corrupt request data!" + Environment.NewLine + data, null);
        }
        else
        {
            //==== Encrypted data ======================================
            // isloate ID from data
            request = mUtils.GetIdDataFromRequest(data);

            // decrypt, parse string and add to the dictionary
            if (mUtils.DecryptAndParseRequestData(request.Item1, request.Item2, myData, char.Parse(dk.KVSep)) == false)
                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                    request.Item1, "Corrupt request data!" + Environment.NewLine + data, null);
        }

        #endregion

        myData.Remove(dk.ServiceName);
        myData.Add(dk.ServiceName, mSvcName);

        // log request if debug set in web.config
        var eid = mUtils.LogRequest(myData, data, mLogId);

        if (!myData.ContainsKey(dk.Request))
            return mUtils.FinalizeXmlResponseWithError("Request type required, EID:" + eid, mLogId);

        Client myClient = null;
        if (myData.ContainsKey(dk.CID))
        {
            myClient = mUtils.GetClientUsingClientId(myData[dk.CID]);
            if (myClient == null)
                return mUtils.FinalizeXmlResponseWithError("Invalid CID:" + myData[dk.CID], mLogId);
            myData.Remove(dk.ClientName);
            myData.Add(dk.ClientName, myClient.Name);
        }
        else if (myData.ContainsKey(dk.ClientName))
        {
            myClient = mUtils.GetClientUsingClientName(myData[dk.ClientName]);
            if (myClient == null)
                return mUtils.FinalizeXmlResponseWithError("Invalid Client name:" + myData[dk.ClientName], mLogId);
            myData.Remove(dk.CID);
            //myData.Add(dk.CID, myClient.ClientId.ToString());
        }
        if (myClient == null) return mUtils.EmptyXml();

        #region Operational Test Ping
        if (myData[dk.Request] == dv.Ping)
        {
            myResponse.Append("<" + sr.Reply + ">" + sr.Success + "</" + sr.Reply + ">");
            return mUtils.FinalizeXmlResponse(myResponse, mLogId);
        }
        #endregion

        //Check Ip
        var mResult = mUtils.CheckClientIp(myClient);
        if (mResult.Item1 == false)
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, myData[dk.CID], mResult.Item2, null);

        #region STS Register End User

        if (myData[dk.Request] == dv.EndUserRegister)
        {
            // Check if the minimun user information was included in request
            var ret = mUtils.Check4MininumEndUserInfo(myData);
            if (ret.Item1 == false)
                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                    myClient.ClientId.ToString(), ret.Item2, null);

            if (!myData[dk.Request].Contains(dv.Register))
                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                    myClient.ClientId.ToString(), "Invalid request[" + myData[dk.Request] + "], " + eid, null);

            var myEndUser = new EndUser();
            if (myData[dk.RegistrationType] == dv.OpenRegister)
            {
                if (myClient.OpenAccessServicesEnabled == false)
                    return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                        myClient.ClientId.ToString(), "Invalid registration request[" + myData[dk.RegistrationType] + "], " + eid, null);

                //==== Open Registration Request =====================================
                // forward end user registration request to OAS 
                ret = mUtils.ServiceRequest(
                    ConfigurationManager.AppSettings[cfg.MacOpenServicesUrl] +
                    Constants.ServiceUrls.MacOpenEndUserServices,
                    myClient.ClientId.ToString(), myData);
                if (ret.Item1 == false)
                    return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                        myClient.ClientId.ToString(), ret.Item2, null);

                myResponse.Append("<" + sr.Reply + ">" + ret.Item2 + "</" + sr.Reply + ">");
                myResponse.Append("<" + sr.UserId + ">" + myEndUser.HashedUserId + "</" + sr.UserId + ">");

                // ReSharper disable once UnusedVariable
                var myStat = new EventStat(myClient._id, myClient.Name, es.EndUserRegister, 1);

                return mUtils.FinalizeXmlResponse(myResponse, mLogId);
            }
            //======== Group Registration =====================
            if (myData[dk.RegistrationType] == dv.GroupRegister)
            {
                if (myData.ContainsKey(dk.GroupId))
                {
                    var mGroup = mUtils.GetGroupUsingGroupId(myData[dk.GroupId]);
                    if (mGroup == null)
                    {
                        myResponse.Append("Invalid Group Id:" + myData[dk.GroupId] + "</" + sr.Reply + ">");
                        return mUtils.FinalizeXmlResponse(myResponse, mLogId);
                    }
                }
                else if (myData.ContainsKey(dk.GroupName))
                {
                    var mGroup = mUtils.GetGroupUsingGroupName(myData[dk.GroupName]);
                    if (mGroup == null)
                    {
                        myResponse.Append("Invalid Group name:" + myData[dk.GroupName] + "</" + sr.Reply + ">");
                        return mUtils.FinalizeXmlResponse(myResponse, mLogId);
                    }
                    myData.Add(dk.GroupId, mGroup._id.ToString());
                }
            }

            //--------- Client and Group registration -------------------------
            var rtnHash = mUtils.GetHashedIdBasedOnRegistrationType(myData);
            if (rtnHash.Item1 == false)
                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                    myClient.ClientId.ToString(), rtnHash.Item2, null);

            // check if user exists
            var endUser = mUtils.GetEndUserByHashedUserId(rtnHash.Item2);
            if (endUser != null)
                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                    myClient.ClientId.ToString(), "End User Exists", null);

            // Fill new end user document with user information
            ret = mUtils.PopulateEndUserObject(myEndUser, myData);
            if (ret.Item1 == false)
                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                    myClient.ClientId.ToString(), ret.Item2, null);

            //==== Create the end user document =========================================        
            myEndUser.State = Constants.EndUserStates.Registered;
            myEndUser.Active = true;
            var mRegistrationEvent =
                new EndUserEvent(mSvcName +
                                 ":" + myEndUser.RegistrationType +
                                 ", State=" + myEndUser.State +
                                 ", Ad Opt-Out=" + myEndUser.OtpOutAd);
            myEndUser.EndUserEvents.Add(mRegistrationEvent);
            var mHashedUserId = myEndUser.HashedUserId;
            mUtils.ObjectCreate(myEndUser);
            myResponse.Append("<" + sr.Reply + ">" + sr.Registered + "</" + sr.Reply + ">");
            myResponse.Append("<" + sr.UserId + ">" + myEndUser.HashedUserId + "</" + sr.UserId + ">");
            myResponse.Append("<" + sr.Details + ">" + mRegistrationEvent.Details + "</" + sr.Details + ">");

            // ReSharper disable once UnusedVariable
            var myRegStat = new EventStat(myClient._id, myClient.Name, es.EndUserRegister, 1);

            Thread.Sleep(100);
            var ckEndUser = mUtils.GetEndUserByHashedUserId(mHashedUserId);
            if (ckEndUser == null)
                Thread.Sleep(1000);

            return mUtils.FinalizeXmlResponse(myResponse, mLogId);
        }

        #endregion

        #region CancelRegistration

        if (myData[dk.Request] == dv.CancelRegistration)
        {
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName + "." + myData[dk.Request],
                myData[dk.CID], "Request not implemented!", null);
        }

        #endregion

        #region Delete Registered User

        if (myData[dk.Request].Contains(dv.DeleteEndUser))
        {

            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName + "." + myData[dk.Request],
                myData[dk.CID], "Request not implemented!", null);
        }

        #endregion

        return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
            null, "Invalid request[" + myData[dk.Request] + "], " + eid, null);
    }
}