using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;
using System.Web.Services;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

using Newtonsoft.Json;

using MACServices;
using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using cs = MACServices.Constants.Strings;
using sr = MACServices.Constants.ServiceResponse;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using cfgcs = MACServices.Constants.WebConfig.ConnectionStringKeys;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

[System.Web.Script.Services.ScriptService]
public class ClientServices: WebService {

    private const string mSvcName = "ClientServices";
    private const string mLogId = "CS";

    [WebMethod]
    public XmlDocument WsClientServices(string data)
    {
        var mUtils = new Utils();
        var myData = new Dictionary<string, string> { { dk.ServiceName, mSvcName }, { "Debug", "true" } };

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
                    null, "request Data (Corrupt)" + Environment.NewLine + data, "99");
        }
        else
        {   // encrypted request data

            //var request = mUtils.GetIdDataFromRequest(data);
            //if (mUtils.DecryptAndParseRequestData(request.Item1, request.Item2, myData, char.Parse(dk.KVSep)) == false)
            //    return mUtils.EventLogError_FinalizeXmlResponse(myData[dk.ServiceName],
            //        null, "Corrupt or bad request data!" + Environment.NewLine + data, "1");



            var request = mUtils.GetIdDataFromRequest(data);

            if (String.IsNullOrEmpty(request.Item1))
                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                    null, request.Item2 + Environment.NewLine + data, "1");

            // decrypt, parse string and add to the dictionary
            if (mUtils.DecryptAndParseRequestData(request.Item1, request.Item2, myData, char.Parse(dk.KVSep)) ==
                false)
                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName,
                    null, "request Data (Corrupt)" + Environment.NewLine + data, "2");
        }
        #endregion

        // log request if debug set in web.config
        var eid = mUtils.LogRequest(myData, data, mLogId);

        #region Operational Test Ping
        if (myData[dk.Request] == dv.Ping)
        {
            myResponse.Append("<" + sr.Reply + ">" + sr.Success + "</" + sr.Reply + ">");
            return mUtils.FinalizeXmlResponse(myResponse, mLogId);
        }
        #endregion

        Client myClient;
        //==== Process Request ====================================================
        switch (myData[dk.Request])
        {
            #region GetClient
            case dv.GetClient:
                //Client myClient = null;
                // get the Client by Id or name
                try
                {
                    if (myData.ContainsKey(dk.CID))
                    {
                        // This doesn't work when we add new members to the client. This should instantiate a new client from the class since it pulls from the db anyways.
                        //myClient = mUtils.GetClientUsingClientId(myData[dk.CID]);

                        myClient = new Client(myData[dk.CID]);
                    }
                    else
                    {
                        myClient = mUtils.GetClientUsingClientName(myData[dk.ClientName]);
                        if (myClient == null)
                            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName + "." + myData[dk.Request],
                                myData[dk.CID], "Failed using name: " + myData[dk.ClientName], "4");
                    }

                }
                catch (Exception ex)
                {
                    var mDetails = ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                    var exceptionEvent = new Event
                    {
                        EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                    };
                    exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                    return mUtils.EventLogError_FinalizeXmlResponse(mSvcName + "." + myData[dk.Request],
                        myData[dk.CID], " Exception" + ex.Message, "5");
                }
                // 1) serialize client object to Json
                // 2) Clean string replacing BSON ObjectIDs and ISODate items
                // 3) Deserialize to XmlNode with Client root
                // 4) Encrypt the response
                // 5) Wrap the response in Xml Reply
                // 6) return xml response
                try
                {
                    // Cast the sms provider port property to string
                    //myClient.MessageProviders.EmailProviders

                    var clientXml = JsonConvert.DeserializeXmlNode(
                        // clean the serialized data
                        myClient.ToJson().Replace("ObjectId(\"", "\"").Replace("\")", "\"").Replace("ISODate(\"", "\""), 
                        "Client");

                    var myClientId = myClient.ClientId.ToString().ToUpper();
                    var cdata = clientXml.OuterXml;
                    var edata = MACSecurity.Security.EncryptAndEncode(cdata, myClientId);

                    myResponse.Append("<" + sr.Reply + ">" + myClientId.Length + myClientId + edata + "</" + sr.Reply + ">");
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

                    return mUtils.EventLogError_FinalizeXmlResponse(mSvcName + "." + myData[dk.Request],
                       myData[dk.CID], " Serialize: Exception:" + ex.Message, "6");
                }
            #endregion

            #region Get Available Message Providers
            case dv.GetAvailableMessageProviders:

                var allMessageProvider = new MessageProvider();
                // 1) Get the Available ProviderEmail, ProviderSms and ProviderVoice Message Provider from database
                // 2) serialize Message Provider object to Json
                // 3) Clean string replacing BSON ObjectIDs and ISODate items
                // 4) Deserialize to XmlNode with Client root
                // 5) Encrypt the response
                // 6) Wrap the response in Xml Reply
                // 7) return xml response
                try
                {
                    var mongoDBConnectionPool = (MongoDatabase)HttpContext.Current.Application[cs.MongoDB];
                    MongoCollection mongoCollection = mongoDBConnectionPool.GetCollection("TypeDefinitions");

                    // --------- available ProviderEmail providers -------------
                    var query = Query.EQ("_t", "ProviderEmail");
                    var globalEmailProviders = mongoCollection.FindAs<ProviderEmail>(query);
                    var sortOrder = new[] { "Name" };
                    globalEmailProviders.SetSortOrder(sortOrder);
                    allMessageProvider.EmailProviders = new List<ProviderEmail>();
                    foreach (var ep in globalEmailProviders)
                    {
                        allMessageProvider.EmailProviders.Add(ep);
                    }

                    //  --------- available ProviderSms Providers -----------------
                    query = Query.EQ("_t", "ProviderSms");
                    var globalSmsProviders = mongoCollection.FindAs<ProviderSms>(query);
                    sortOrder = new[] { "Name" };
                    globalSmsProviders.SetSortOrder(sortOrder);
                    allMessageProvider.SmsProviders = new List<ProviderSms>();
                    foreach (var sp in globalSmsProviders)
                    {
                        allMessageProvider.SmsProviders.Add(sp);
                    }

                    // --------- available ProviderVoice providers -----------------
                    query = Query.EQ("_t", "ProviderVoice");
                    var globalVoiceProviders = mongoCollection.FindAs<ProviderVoice>(query);
                    sortOrder = new[] { "Name" };
                    globalVoiceProviders.SetSortOrder(sortOrder);
                    allMessageProvider.VoiceProviders = new List<ProviderVoice>();
                    foreach (var vp in globalVoiceProviders)
                    {
                        allMessageProvider.VoiceProviders.Add(vp);
                    }
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

                    return mUtils.EventLogError_FinalizeXmlResponse(mSvcName + "." + myData[dk.Request],
                        myData[dk.CID], "Exception:" + ex.Message, "13");
                }
                // serialize and build Xml reply
                try
                {
                    var clientXml = JsonConvert.DeserializeXmlNode(
                        // clean the serialized data
                        allMessageProvider.ToJson().Replace("ObjectId(\"", "\"").Replace("\")", "\"").Replace("ISODate(\"", "\""),
                        "MessageProvider");
                    var resp = (myData[dk.CID].ToUpper().Length).ToString(CultureInfo.CurrentCulture) +
                                  myData[dk.CID].ToUpper() +
                                  MACSecurity.Security.EncryptAndEncode(clientXml.OuterXml, myData[dk.CID].ToUpper());
                    myResponse.Append("<" + sr.Reply + ">" + resp + "</" + sr.Reply + ">");
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

                    return mUtils.EventLogError_FinalizeXmlResponse(mSvcName + "." + myData[dk.Request],
                        myData[dk.CID], "Serializer.Exception:" + ex.Message, "14");
                }
                //break;
            #endregion

            #region Get Client Id
            case dv.GetClientId: // using client name
                if (myData.ContainsKey(dk.ClientName))
                {
                    myClient = mUtils.GetClientUsingClientName(myData[dk.ClientName]);
                    if (myClient == null)
                        return mUtils.EventLogError_FinalizeXmlResponse(mSvcName + "." + myData[dk.Request],
                                    null, "No client with name: " + myData[dk.ClientName], "15");

                    myResponse.Append("<" + sr.Reply + ">" + myClient.ClientId + "</" + sr.Reply + ">");
                    return mUtils.FinalizeXmlResponse(myResponse, mLogId);
                }
                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName + "." + myData[dk.Request],
                        null, myData[dk.Request] + ":Client name required!" , "16");
            #endregion

            #region Get Client Name
            case dv.GetClientName:
                if (myData.ContainsKey(dk.CID))
                {
                    myClient = mUtils.GetClientUsingClientId(myData[dk.CID]);
                    if (myClient == null)
                        return mUtils.EventLogError_FinalizeXmlResponse(mSvcName + "." + myData[dk.Request],
                                    null, "No client with id: " + myData[dk.CID], "15");

                    myResponse.Append("<" + sr.Reply + ">" + myClient.Name + "</" + sr.Reply + ">");
                    return mUtils.FinalizeXmlResponse(myResponse, mLogId);
                }
                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName + "." + myData[dk.Request],
                        null, myData[dk.Request] + ":Client Id (CID) required!" , "16");
            #endregion

            default:
                return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, null,
                    "Invalid request[" + myData[dk.Request] + "], " + eid, null);
        }
    }


    [WebMethod]
    public XmlDocument WsGetMACClientWithSTSiteIds()
    {
        var mUtils = new Utils();
        // start the XML response
        var myResponse = new StringBuilder();
        mUtils.InitializeXmlResponse(myResponse);
        var mClients = mUtils.GetMACClientWithSTSiteIds();
        if (mClients.Any())
        {
            myResponse.Append("<STClients>");
            foreach (var mClient in mClients)
            {
                if (String.IsNullOrEmpty(mClient.SecureTradingSiteId))
                {
                    mClient.SecureTradingSiteId = String.Empty;
                    mClient.Update();
                    continue;

                }
                if (mClient.SecureTradingSiteId.Contains("|"))
                {
                    var tmpVal = mClient.SecureTradingSiteId.Split('|');
                    myResponse.Append("<STClient");
                    myResponse.Append(" ClientId=\"" + mClient.ClientId.ToString() + "\"");
                    myResponse.Append(" ClientName=\"" + mClient.Name + "\"");
                    myResponse.Append(" OperatorId=\"" + tmpVal[0] + "\"");
                    myResponse.Append(" SiteId=\"" + tmpVal[1] + "\"");
                    myResponse.Append(" />");
                }
            }
            myResponse.Append("</STClients>");
        }
        else
        {
            myResponse.Append("<" + sr.Error + ">" + "No Clients Configured with ST Operator and Site Ids" + "</" + sr.Error + ">");
        }

        return mUtils.FinalizeXmlResponse(myResponse, mLogId);
    }

    [WebMethod]
    public XmlDocument WsGetMACClientIdBySTSiteId(string data)
    {
        
        var mUtils = new Utils();

        // start the XML response
        var myResponse = new StringBuilder();
        mUtils.InitializeXmlResponse(myResponse);

        var operatorId = "";
        var siteId = "";

        if(data.Contains("|"))
        {
            var tmpVal = data.Split('|');

            operatorId = tmpVal[0];
            siteId = tmpVal[1];
        }

        try
        {
            if (string.IsNullOrEmpty(data))
            {
                myResponse.Append("<msg>You must provide a Secure Trading OperatorId and SiteId seperated by a pipe character to recieve a validation response.</msg>");
                return mUtils.FinalizeXmlResponse(myResponse, mLogId);
            }
            else
            {
                // Check to see if SiteId is already assigned to another client
                //var tmpSiteId = mUtils.GetMACClientIdBySTOperatorIdAndSiteId(operatorId, siteId);
                var tmpSiteId = mUtils.GetMACClientIdBySTSiteId(operatorId, siteId);
                if (String.IsNullOrEmpty(tmpSiteId))
                {
                    myResponse.Append("<msg>SecureTrading OperatorId (" + operatorId + ") and SiteId (" + siteId + ") is available for Client assignment.</msg>");
                }
                else
                {
                    var tmpVal = tmpSiteId.Split('|');
                    myResponse.Append("<clientid>" + tmpVal[0] + "</clientid>");
                    myResponse.Append("<clientname>" + tmpVal[1] + "</clientname>");
                }
            }
        }
        catch(Exception ex)
        {
            myResponse.Append("<exception>" + ex.ToString() + "</exception>");
        }

        return mUtils.FinalizeXmlResponse(myResponse, mLogId);
    }
}
