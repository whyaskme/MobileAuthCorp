using System;
using System.Configuration;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Web.Services;
using System.Xml;
using System.Web;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

using MACServices;
using MACSecurity;
using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using sc = MACServices.Constants.Strings;
using sr = MACServices.Constants.ServiceResponse;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using cfgcs = MACServices.Constants.WebConfig.ConnectionStringKeys;

[WebService(Namespace = "http://mobileauthcorp.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

[System.Web.Script.Services.ScriptService]
public class EndUserManagement : WebService
{
    private const string mSvcName = "EndUserManagement";
    private const string mLogId = "UM";

    public Object PropertyValue { get; set; }
    public virtual Type PropertyType { get; set; }


    [WebMethod]
    public XmlDocument WsEndUserManagement(string data)
    {
        var mUtils = new Utils();

        Tuple<string, string> request;
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
                    request.Item1, "Corrupt request data!" + Environment.NewLine + data, "99");
        }
        else
        {   //==== Encrypted data ======================================
            // isloate ID from data
            request = mUtils.GetIdDataFromRequest(data);

            // decrypt, parse string and add to the dictionary
            if (mUtils.DecryptAndParseRequestData(request.Item1, request.Item2, myData, char.Parse(dk.KVSep)) == false)
                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                    request.Item1, "Corrupt request data!" + Environment.NewLine + data, "1");
        }
        #endregion

        // log request if debug set in web.config
        var eid = mUtils.LogRequest(myData, data, mLogId);

        if (myData.ContainsKey(dk.Request) == false)
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, null, " No Request", "1");

        // valid client
        if (myData.ContainsKey(dk.CID) == false)
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                    request.Item1, "CID required!" + Environment.NewLine + data, "3");
        var myClient = mUtils.GetClientUsingClientId(myData[dk.CID]);
        if (myClient == null) 
            return mUtils.EmptyXml();
        if (myData.ContainsKey(dk.ClientName) == false)
            myData.Add(dk.ClientName, myClient.Name);

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

        try
        {
            #region Check End User Registration
            if (myData[dk.Request] == dv.CheckEndUserRegistration)
            {
                if (myData.ContainsKey(dk.UserId))
                {
                    myResponse.Append("<Groups>");
                    myResponse.Append(CheckIfRegisteredWithGroups(myData[dk.UserId]));
                    myResponse.Append("</Groups>");

                    myResponse.Append("<Clients>");
                    myResponse.Append(CheckIfRegisteredWithClients(myData[dk.UserId]));
                    myResponse.Append("</Clients>");

                    myResponse.Append("<Open>");
                    myResponse.Append(CheckIfOpenRegistered(myData[dk.UserId]));
                    myResponse.Append("</Open>");
                    return mUtils.FinalizeXmlResponse(myResponse, mLogId);
                }
            }
            #endregion

            #region Activate End User
            if (myData[dk.Request] == dv.ActivateEndUser)
            {
                #region Send request to OAS if end user not in local database
                EndUser myEndUser;
                // is this a group request (request data conatins a group Id)
                if (myData.ContainsKey(dk.GroupId))
                {
                    myEndUser =
                        mUtils.GetEndUserByHashedUserId(Security.GetHashString(myData[dk.UserId] + myData[dk.GroupId]));
                }
                else
                {
                    myEndUser =
                        mUtils.GetEndUserByHashedUserId(Security.GetHashString(myData[dk.UserId] + myData[dk.CID]));
                }
                if (myEndUser == null)
                {   // User record not in local database send request to OAS service
                    var ret = mUtils.ServiceRequest(
                        ConfigurationManager.AppSettings[cfg.MacOpenServicesUrl] +
                        Constants.ServiceUrls.MacOpenEndUserServices, myData[dk.CID], myData);
                    if (ret.Item1 == false)
                        return mUtils.FinalizeXmlResponseWithError(ret.Item2, mLogId);

                    // End User was in OAS database
                    if (ret.Item2.Contains(dk.ItemSep))
                    {
                        var replydetails = ret.Item2.Split(char.Parse(dk.ItemSep));
                        myResponse.Append("<" + sr.Details + ">" + replydetails[1] + "</" + sr.Details + ">");
                        myResponse.Append("<" + sr.Reply + ">" + replydetails[0] + "</" + sr.Reply + ">");
                        //string xx = myResponse.ToString();
                        return mUtils.FinalizeXmlResponse(myResponse, mLogId);
                    }
                    myResponse.Append("<" + sr.Reply + ">" + ret.Item2 + "</" + sr.Reply + ">");
                    return mUtils.FinalizeXmlResponse(myResponse, mLogId);
                }
                #endregion

                // user is in local database
                //if (myEndUser.ClientId != myClient.ClientId)
                //    return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                //                    myData[dk.CID], "Not user's client", null);
                if (myEndUser.Active)
                {
                    myResponse.Append("<" + sr.Reply + ">" + Constants.ServiceResponse.Activated + "</" + sr.Reply + ">");
                    return mUtils.FinalizeXmlResponse(myResponse, mLogId);
                }
                myEndUser.Active = true;

                var myEvent = new EndUserEvent(mSvcName + "." + myData[dk.Request]);
                myEndUser.EndUserEvents.Add(myEvent);
                //mUtils.ObjectUpdate(myEndUser, myEndUser._id.ToString());
                myResponse.Append("<" + sr.Reply + ">" + Constants.ServiceResponse.Activated + "</" + sr.Reply + ">");
                return mUtils.FinalizeXmlResponse(myResponse, mLogId);
            }
            #endregion

            #region Deactivate End User
            if (myData[dk.Request] == dv.DeactivateEndUser)
            {
                #region Send request to OAS if end user not in local database
                EndUser myEndUser;
                // is this a group request (request data conatins a group Id)
                if (myData.ContainsKey(dk.GroupId))
                {
                    myEndUser =
                        mUtils.GetEndUserByHashedUserId(Security.GetHashString(myData[dk.UserId] + myData[dk.GroupId]));
                }
                else
                {
                    myEndUser =
                        mUtils.GetEndUserByHashedUserId(Security.GetHashString(myData[dk.UserId] + myData[dk.CID]));
                }
                if (myEndUser == null)
                {   // User record not in local database send request to OAS service
                    var ret = mUtils.ServiceRequest(
                        ConfigurationManager.AppSettings[cfg.MacOpenServicesUrl] +
                        Constants.ServiceUrls.MacOpenEndUserServices,
                                        myData[dk.CID], myData);
                    if (ret.Item1 == false)
                        return mUtils.FinalizeXmlResponseWithError(ret.Item2, mLogId);

                    // End User was in OAS database
                    if (ret.Item2.Contains(dk.ItemSep))
                    {
                        var replydetails = ret.Item2.Split(char.Parse(dk.ItemSep));
                        myResponse.Append("<Details>" + replydetails[1] + "</Details>");
                        myResponse.Append("<" + sr.Reply + ">" + replydetails[0] + "</" + sr.Reply + ">");
                        //string xx = myResponse.ToString();
                        return mUtils.FinalizeXmlResponse(myResponse, mLogId);
                    }
                    myResponse.Append("<" + sr.Reply + ">" + ret.Item2 + "</" + sr.Reply + ">");
                    return mUtils.FinalizeXmlResponse(myResponse, mLogId);
                }
                #endregion

                // user is in local database
                //if (myEndUser.ClientId != myClient.ClientId)
                //    return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                //                    myData[dk.CID], "Not user's client", null);
                
                if (myEndUser.Active == false)
                {
                    myResponse.Append("<" + sr.Reply + ">" + Constants.ServiceResponse.Deactivated + "</" + sr.Reply + ">");
                    return mUtils.FinalizeXmlResponse(myResponse, mLogId);
                }
                myEndUser.Active = false;

                var myEvent = new EndUserEvent(mSvcName + "." + myData[dk.Request]);
                myEndUser.EndUserEvents.Add(myEvent);
                mUtils.ObjectUpdate(myEndUser, myEndUser._id.ToString());
                myResponse.Append("<" + sr.Reply + ">" + Constants.ServiceResponse.Deactivated + "</" + sr.Reply + ">");
                return mUtils.FinalizeXmlResponse(myResponse, mLogId);
            }
            #endregion

            #region Update End User

            if (myData[dk.Request] == dv.UpdateEndUser)
            {
                #region Send request to OAS if end user not in local database

                EndUser myEndUser;
                // is this a group request (request data conatins a group Id)
                if (myData.ContainsKey(dk.GroupId))
                {
                    myEndUser =
                        mUtils.GetEndUserByHashedUserId(Security.GetHashString(myData[dk.UserId] + myData[dk.GroupId]));
                }
                else
                {
                    myEndUser =
                        mUtils.GetEndUserByHashedUserId(Security.GetHashString(myData[dk.UserId] + myData[dk.CID]));
                }
                if (myEndUser == null)
                {
                    // User record not in local database send request to OAS service
                    var ret = mUtils.ServiceRequest(
                        ConfigurationManager.AppSettings[cfg.MacOpenServicesUrl] +
                        Constants.ServiceUrls.MacOpenEndUserServices,
                        myData[dk.CID], myData);
                    if (ret.Item1 == false)
                        return mUtils.FinalizeXmlResponseWithError(ret.Item2, mLogId);

                    // End User was in OAS database
                    if (ret.Item2.Contains(dk.ItemSep))
                    {
                        var replydetails = ret.Item2.Split(char.Parse(dk.ItemSep));
                        myResponse.Append("<Details>" + replydetails[1] + "</Details>");
                        myResponse.Append("<" + sr.Reply + ">" + replydetails[0] + "</" + sr.Reply + ">");
                        //string xx = myResponse.ToString();
                        return mUtils.FinalizeXmlResponse(myResponse, mLogId);
                    }
                    myResponse.Append("<" + sr.Reply + ">" + ret.Item2 + "</" + sr.Reply + ">");
                    return mUtils.FinalizeXmlResponse(myResponse, mLogId);
                }

                #endregion

                // user is in local database
                //if (myEndUser.ClientId != myClient.ClientId)
                //    return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, myData[dk.CID], "Not user's client", null);
                var mUpdate  = new StringBuilder();
                mUpdate.Append("Updated:");
                if (myData.ContainsKey(dkui.PhoneNumber))
                {
                    // todo: Need to match old phone number
                    var mNewPhone = myData[dkui.PhoneNumber];
                    if (mUtils.ValidatePhoneNumber(mNewPhone) == false)
                        return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                            myData[dk.CID], dv.Update + ", invalid phone number: " + mNewPhone, "17");
                    myEndUser.Phone = Security.EncryptAndEncode(mUtils.PhoneJustNumbers(mNewPhone), myEndUser.HashedUserId);
                    mUpdate.Append("," + dkui.PhoneNumber + "=" + mUtils.PhoneJustNumbers(mNewPhone));
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
                var euEvent = new EndUserEvent(mSvcName + ": " + myData[dk.Request] + ", " + mUpdate.ToString());
                myEndUser.EndUserEvents.Add(euEvent);
                myEndUser.Update();

                myResponse.Append("<" + sr.Reply + ">" + Constants.ServiceResponse.Success + "</" + sr.Reply + ">");
                myResponse.Append("<" + sr.Details + ">Updated, " + mUpdate.ToString() + "</" + sr.Details + ">");
                return mUtils.FinalizeXmlResponse(myResponse, mLogId);
            }

            #endregion

            #region Delete End User
            if (myData[dk.Request] == dv.DeleteEndUser)
            {
                #region Send request to OAS if end user not in local database
                EndUser myEndUser;
                // is this a group request (request data conatins a group Id)
                if (myData.ContainsKey(dk.GroupId))
                {
                    myEndUser =
                        mUtils.GetEndUserByHashedUserId(Security.GetHashString(myData[dk.UserId] + myData[dk.GroupId]));
                }
                else
                {
                    myEndUser =
                        mUtils.GetEndUserByHashedUserId(Security.GetHashString(myData[dk.UserId] + myData[dk.CID]));
                }
                if (myEndUser == null)
                {   // User record not in local database send request to OAS service
                    var ret = mUtils.ServiceRequest(
                        ConfigurationManager.AppSettings[cfg.MacOpenServicesUrl] +
                        Constants.ServiceUrls.MacOpenEndUserServices,
                                        myData[dk.CID], myData);
                    if (ret.Item1 == false)
                        return mUtils.FinalizeXmlResponseWithError(ret.Item2, mLogId);

                    // End User was in OAS database
                    if (ret.Item2.Contains(dk.ItemSep))
                    {
                        var replydetails = ret.Item2.Split(char.Parse(dk.ItemSep));
                        myResponse.Append("<" + sr.Details + ">" + replydetails[1] + "</" + sr.Details + ">");
                        myResponse.Append("<" + sr.Reply + ">" + replydetails[0] + "</" + sr.Reply + ">");
                        //string xx = myResponse.ToString();
                        return mUtils.FinalizeXmlResponse(myResponse, mLogId);
                    }
                    myResponse.Append("<" + sr.Reply + ">" + ret.Item2 + "</" + sr.Reply + ">");
                    return mUtils.FinalizeXmlResponse(myResponse, mLogId);
                }
                //if (myEndUser.ClientId != myClient.ClientId)
                //    return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                //                    myData[dk.CID], "Not user's client", null);
                #endregion
                // user is in local database
                mUtils.DeleteEndUser(myEndUser);
                myResponse.Append("<" + sr.Reply + ">" + Constants.ServiceResponse.Deleted + "</" + sr.Reply + ">");
                return mUtils.FinalizeXmlResponse(myResponse, mLogId);
            }
            #endregion

            #region Ad Pass
            if (myData[dk.Request] == dv.SetAdPassOption)
            {
                #region Send request to OAS if end user not in local database
                EndUser myEndUser;
                // is this a group request (request data conatins a group Id)
                if (myData.ContainsKey(dk.GroupId))
                {
                    myEndUser =
                        mUtils.GetEndUserByHashedUserId(Security.GetHashString(myData[dk.UserId] + myData[dk.GroupId]));
                }
                else
                {
                    myEndUser =
                        mUtils.GetEndUserByHashedUserId(Security.GetHashString(myData[dk.UserId] + myData[dk.CID]));
                }
                if (myEndUser == null)
                {   // User record not in local database send request to OAS service
                    var ret = mUtils.ServiceRequest(
                        ConfigurationManager.AppSettings[cfg.MacOpenServicesUrl] +
                        Constants.ServiceUrls.MacOpenEndUserServices, myData[dk.CID], myData);
                    if (ret.Item1 == false)
                        return mUtils.FinalizeXmlResponseWithError(ret.Item2, mLogId);

                    // End User was in OAS database
                    if (ret.Item2.Contains(dk.ItemSep))
                    {
                        var replydetails = ret.Item2.Split(char.Parse(dk.ItemSep));
                        myResponse.Append("<" + sr.Details + ">" + replydetails[1] + "</" + sr.Details + ">");
                        myResponse.Append("<" + sr.Reply + ">" + replydetails[0] + "</" + sr.Reply + ">");
                        //string xx = myResponse.ToString();
                        return mUtils.FinalizeXmlResponse(myResponse, mLogId);
                    }
                    myResponse.Append("<" + sr.Reply + ">" + ret.Item2 + "</" + sr.Reply + ">");
                    return mUtils.FinalizeXmlResponse(myResponse, mLogId);
                }
                #endregion
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
                    myResponse.Append("<" + sr.Reply + ">" + Constants.ServiceResponse.Success + "</" + sr.Reply + ">");
                    return mUtils.FinalizeXmlResponse(myResponse, mLogId);
                }
            }
            #endregion

            #region Reset SMS Stop
            //if (myData[dk.Request] == dv.ResetSMSStop)
            //{
            //    return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
            //                  myData[dk.CID], "Request Not Implemented[" + myData[dk.Request] + "], " + eid, null);
        
            //}
            #endregion

            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                        myData[dk.CID], "Invalid request[" + myData[dk.Request] + "], " + eid, null);
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

            return mUtils.FinalizeXmlResponseWithError(mSvcName +
                    myData[dk.CID] + String.Format("Exception processing request[{0}] {1}", myData[dk.Request], ex.Message), mLogId);
        }
    }

    protected string CheckIfRegisteredWithGroups(string UserId)
    {
        var mongoDBConnectionPool = (MongoDatabase)HttpContext.Current.Application[sc.MongoDB];
        var myUtils = new Utils();
        // get list of client Ids
        var theList = new StringBuilder();
        try
        {
            // Get available Clients
            var query = Query.EQ("_t", "Group");
            var mongoCollection = mongoDBConnectionPool.GetCollection("Group").Find(query).SetFields(Fields.Include("_id", "Name"));
            var sortOrder = new[] { "Name" };
            mongoCollection.SetSortOrder(sortOrder);

            var items = mongoCollection.ToList();
            foreach (var item in items)
            {
                var id = item[0].ToString();
                var name = item[1];
                if (name != Constants.Strings.DefaultClientName)
                {
                    var mUserid = Security.GetHashString(UserId + id);
                    var mEndUser = myUtils.GetEndUserByHashedUserId(mUserid);
                    if (mEndUser != null)
                        theList.Append("<Group>" + name + "</Group>");
                }
            }
        }
        catch
        {
            return @"Group Error!";
        }

        if (String.IsNullOrEmpty(theList.ToString()) == false)
        {
            return theList.ToString();
        }
        return "None";
    }

    protected string CheckIfRegisteredWithClients(string UserId)
    {
        var myUtils = new Utils();
        var mongoDBConnectionPool = (MongoDatabase)HttpContext.Current.Application[sc.MongoDB];
        // get list of client Ids
        var theList = new StringBuilder();
        try
        {
            // Get available Clients
            var query = Query.EQ("_t", "Client");
            var mongoCollection = mongoDBConnectionPool.GetCollection("Client").Find(query).SetFields(Fields.Include("_id", "Name"));
            var sortOrder = new[] {"Name"};
            mongoCollection.SetSortOrder(sortOrder);

            var clientIdList = mongoCollection.ToList();
            foreach (var vClient in clientIdList)
            {
                var cid = vClient[0].ToString();
                var cname = vClient[1];
                if (cname != Constants.Strings.DefaultClientName)
                {
                    var mUserid = Security.GetHashString(UserId + cid);
                    var mEndUser = myUtils.GetEndUserByHashedUserId(mUserid);
                    if (mEndUser != null)
                        theList.Append("<Client>" + cname + "</Client>");
                }
            }
        }
        catch
        {
            return @"Client Error!";
        }

        return String.IsNullOrEmpty(theList.ToString()) == false ? theList.ToString() : "None";
    }

    protected string CheckIfOpenRegistered(string UserId)
    {
        // call open service
        var mUtils = new Utils();
        var myOpenRequest = new Dictionary<string, string>
                            {
                                {dk.UserId, UserId},
                                {dk.Request, dv.GetEndUserInfo}
                            };
        var endUserInfo = mUtils.ServiceRequest(
                ConfigurationManager.AppSettings[cfg.MacOpenServicesUrl] +
                Constants.ServiceUrls.MacOpenEndUserServices,
            Constants.Strings.DefaultClientId, myOpenRequest);

        return endUserInfo.Item1 == false ? "Not Open Registered" : "Open Registered";
    }
}