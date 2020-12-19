using System;
using System.Text;
using System.Xml;
using System.Web.Services;
using System.Web;

using MongoDB.Driver;
using MongoDB.Driver.Builders;

using MACServices;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using cfgcs = MACServices.Constants.WebConfig.ConnectionStringKeys;

/// <summary>
/// Summary description for EventHistoryStats
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class EventHistoryStats : WebService {

    [WebMethod]
    public XmlDocument WsEventHistoryStats(string clientId, string objectType, DateTime startDate, DateTime endDate)
    {
        var mUtils = new Utils();

        if (startDate == null) throw new ArgumentNullException("startDate");
        if (endDate == null) throw new ArgumentNullException("endDate");

        var sbResponse = new StringBuilder();

        endDate = endDate.AddDays(1);

        var span = new TimeSpan(30, 0, 0, 0, 0);

        startDate = endDate.Subtract(span);

        mUtils.InitializeXmlResponse(sbResponse);

        try
        {
            var mongoDBConnectionPool = (MongoDatabase)HttpContext.Current.Application[Constants.Strings.MongoDB];

            // Get stats for date range for each event type
            var typeDefs = mUtils.GetArrayOfTypeDefs();
            foreach (string t in typeDefs)
            {
                if (t == null) continue;
                var eventType = t;

                var query = Query.And(Query.Matches("ObjectType", eventType),
                    Query.GT("Date", startDate),
                    Query.LT("Date", endDate));

                var eventCount = Convert.ToInt32(mongoDBConnectionPool.GetCollection("Event").Find(query).Count());

                sbResponse.Append("<" + eventType.ToLower().Replace(" ", "") + ">");
                sbResponse.Append(eventCount);
                sbResponse.Append("</" + eventType.ToLower().Replace(" ", "") + ">");
            }

            // Get distinct Otp.CodeHistory.Details from Otp collection
            var otpEvents = mUtils.GetArrayOfDistinctOtpEvents();

            foreach (string t in otpEvents)
            {
                if (t == null) continue;
                var eventType = t;

                var query = Query.And(Query.Matches("ObjectType", eventType),
                    Query.GT("Date", startDate),
                    Query.LT("Date", endDate));

                var eventCount = Convert.ToInt32(mongoDBConnectionPool.GetCollection("Event").Find(query).Count());

                sbResponse.Append("<" + eventType.ToLower().Replace(" ", "") + ">");
                sbResponse.Append(eventCount);
                sbResponse.Append("</" + eventType.ToLower().Replace(" ", "") + ">");
            }

            var rsp = mUtils.FinalizeXmlResponse(sbResponse, String.Empty);
            return rsp;
        }
        catch(Exception ex)
        {
            return mUtils.FinalizeXmlResponseWithError(String.Format("Exception: {0}", ex.Message), String.Empty);
        }
    }
    
}
