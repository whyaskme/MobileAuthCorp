using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Xml;

using MongoDB.Driver;
using MongoDB.Driver.Builders;

using MACServices;
using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using sr = MACServices.Constants.ServiceResponse;

[WebService(Namespace = "http://mobileauthcorp.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

[System.Web.Script.Services.ScriptService]
public class OpsTestServices : WebService 
{
    private const string mSvcName = "OpsTestServices";
    private const string mLogId = "OT";

    [WebMethod]
    public XmlDocument WsOpsTestServices(string data)
    {
        var mUtils = new Utils();

        var myData = new Dictionary<string, string> { { dk.ServiceName, mSvcName } };

        // start the XML response
        var myResponse = new StringBuilder();
        mUtils.InitializeXmlResponse(myResponse);

        var myrequestData = data;

        var requestWasFrom = HttpContext.Current.Request.ServerVariables["HTTP_HOST"];

        var request = mUtils.GetIdDataFromRequest(myrequestData);
        if (String.IsNullOrEmpty(request.Item1))
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, request.Item1, "Corrupt data" + Environment.NewLine + data, null);

        // decrypt and parse request data
        if (mUtils.DecryptAndParseRequestData(request.Item1, request.Item2, myData, char.Parse(dk.KVSep)) == false)
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, request.Item1, "Corrupt data" + Environment.NewLine + data, null);

        if (!myData.ContainsKey(dk.Request))
            return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, request.Item1, "Corrupt data" + Environment.NewLine + data, null);

        // log request if debug set in web.config
        var eid = mUtils.LogRequest(myData, data, mLogId);

        if (myData[dk.Request] == "GetOpts")
        {
            var mongoClient =
                new MongoClient(ConfigurationManager.ConnectionStrings["OperationalTestServer"].ConnectionString);
            var server = mongoClient.GetServer();
            var db = server.GetDatabase(ConfigurationManager.AppSettings["MongoDbOperationalTestDBName"]);
            try
            {
                var query = Query.EQ("_t", "OperationalTest");
                var mongoCollection = db.GetCollection("OperationalTest");
  //              return mongoCollection.FindOneAs<MACOperationalTestLib.OperationalTest>(query);
            }
            catch (Exception ex)
            {
                var error = ex.ToString();
                return null;
            }
            finally
            {
                server.Disconnect();
            }
            
        }

        if (myData[dk.Request] == "UpdateOpts")
        {

        }


        return mUtils.EventLogError_FinalizeXmlResponse(mSvcName, request.Item1,
            "Invalid request[" + myData[dk.Request] + "], " + eid, null);
    }
}

