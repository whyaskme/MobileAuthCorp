using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;

using MACServices;
using cs = MACServices.Constants.Strings;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

[ScriptService]

public class SystemStats : WebService {

    // ReSharper disable once EmptyConstructor
    public SystemStats () {}

    [WebMethod]
    //[ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)] // Specify response format
    public string WsSystemStats(string dateRange, string ownerId)
    {
        Utils mUtils = new Utils();

        DateTime _startDate = DateTime.UtcNow;
        DateTime _endDate = DateTime.UtcNow.AddDays(1);
        ObjectId _ownerId;

        var mongoDBConnectionPool = (MongoDatabase)HttpContext.Current.Application[cs.MongoDB];

        if (String.IsNullOrEmpty(dateRange))
            dateRange = "All Time";

        switch(dateRange)
        {
            case "All Time":
                _startDate = DateTime.UtcNow.AddYears(-10);
                break;
            case "Today":
                _startDate = DateTime.UtcNow.AddDays(-1);
                break;
            case "Yesterday":
                _startDate = DateTime.UtcNow.AddDays(-2);
                _endDate = DateTime.UtcNow.AddDays(-1);
                break;
            case "This Week":
                _startDate = DateTime.UtcNow.AddDays(-7);
                break;
            case "This Month":
                _startDate = DateTime.UtcNow.AddDays(-30);
                break;
            case "This Quarter":
                _startDate = DateTime.UtcNow.AddDays(-90);
                break;
            case "This Year":
                _startDate = DateTime.UtcNow.AddDays(-365);
                break;
        }

        if (String.IsNullOrEmpty(ownerId))
            _ownerId = ObjectId.Parse(Constants.Strings.DefaultEmptyObjectId);
        else
            _ownerId = ObjectId.Parse(ownerId);

        var stats = new Dictionary<string, string>();

        string myJsonString;

        try
        {
            // Delivery
            var OtpSentEmail = 0;
            var OtpSentSms = 0;
            var OtpSentVoice = 0;

            // Validation
            var OtpValid = 0;
            var OtpInvalid = 0;
            var OtpExpired = 0;

            var EndUserRegister = 0;
            var EndUserVerify = 0;

            //var AdsSent = 0;
            var MessageAd = 0;
            var AdEnterOtpScreenSent = 0;
            var VerificationAd = 0;
            var AdsClicked = 0;

            var Events = 0;
            var Exceptions = 0;

            // Process stats
            MongoCollection statCollection = mongoDBConnectionPool.GetCollection("EventStat");
            var statsQuery = Query.Null;

            var statList = statCollection.FindAllAs<EventStat>().ToList<EventStat>();

            // Get select client stats
            if (_ownerId.ToString() != Constants.Strings.DefaultEmptyObjectId)
            {
                statList.Clear();

                statsQuery = Query.EQ("OwnerId", _ownerId);
                var myStats = statCollection.FindOneAs<EventStat>(statsQuery);

                statList.Add(myStats);
            }

            foreach (var currentStat in statList)
            {
                foreach (var currentDayStat in currentStat.DailyStats)
                {
                    if (currentDayStat.Date >= _startDate.Date && currentDayStat.Date <= _endDate.Date)
                    {
                        foreach (var property in currentDayStat.GetType().GetProperties())
                        {
                            switch (property.Name)
                            {
                                case "OtpSentEmail":
                                    OtpSentEmail += (Int32)property.GetValue(currentDayStat);
                                    break;
                                case "OtpSentSms":
                                    OtpSentSms += (Int32)property.GetValue(currentDayStat);
                                    break;
                                case "OtpSentVoice":
                                    OtpSentVoice += (Int32)property.GetValue(currentDayStat);
                                    break;
                                case "OtpValid":
                                    OtpValid += (Int32)property.GetValue(currentDayStat);
                                    break;
                                case "OtpInvalid":
                                    OtpInvalid += (Int32)property.GetValue(currentDayStat);
                                    break;
                                case "OtpExpired":
                                    OtpExpired += (Int32)property.GetValue(currentDayStat);
                                    break;
                                case "EndUserRegister":
                                    EndUserRegister += (Int32)property.GetValue(currentDayStat);
                                    break;
                                case "EndUserVerify":
                                    EndUserVerify += (Int32)property.GetValue(currentDayStat);
                                    break;
                                case "AdMessageSent":
                                    MessageAd += (Int32)property.GetValue(currentDayStat); 
                                    break;
                                case "AdEnterOtpScreenSent":
                                    AdEnterOtpScreenSent += (Int32)property.GetValue(currentDayStat);
                                    break;
                                case "AdVerificationScreenSent":
                                    VerificationAd += (Int32)property.GetValue(currentDayStat);
                                    break;
                                case "AdsClicked":
                                    AdsClicked += (Int32)property.GetValue(currentDayStat);
                                    break;
                                case "Events":
                                    Events += (Int32)property.GetValue(currentDayStat);
                                    break;
                                case "Exceptions":
                                    Exceptions += (Int32)property.GetValue(currentDayStat);
                                    break;
                            }
                        }
                    }
                }
            }

            var groups = Convert.ToInt32(mongoDBConnectionPool.GetCollection("Group").Count());
            stats.Add("Groups", groups.ToString());

            var clients = Convert.ToInt32(mongoDBConnectionPool.GetCollection("Client").Count());
            stats.Add("Clients", clients.ToString());

            var adminQuery = Query.EQ("Roles", ObjectId.Parse("5285be322b0b6d1ac4a542ca"));
            var systemAdmins = Convert.ToInt32(mongoDBConnectionPool.GetCollection("UserProfile").Find(adminQuery).Count());
            stats.Add("SystemAdmins", systemAdmins.ToString());

            adminQuery = Query.EQ("Roles", ObjectId.Parse("5285be642b0b6d1ac4a542ce"));
            var groupAdmins = Convert.ToInt32(mongoDBConnectionPool.GetCollection("UserProfile").Find(adminQuery).Count());
            stats.Add("GroupAdmins", groupAdmins.ToString());

            adminQuery = Query.EQ("Roles", ObjectId.Parse("5285be8d2b0b6d1ac4a542d2"));
            var clientAdmins = Convert.ToInt32(mongoDBConnectionPool.GetCollection("UserProfile").Find(adminQuery).Count());
            stats.Add("ClientAdmins", clientAdmins.ToString());

            stats.Add("EndUsers", EndUserRegister.ToString());

            stats.Add("Events", Events.ToString());
            stats.Add("Exceptions", Exceptions.ToString());

            var tmpSent = (OtpSentEmail + OtpSentSms + OtpSentVoice);
            var tmpInvalid = (tmpSent - OtpValid);

            stats.Add("OtpSent", tmpSent.ToString());

            stats.Add("OtpValid", OtpValid.ToString());
            stats.Add("OtpInvalid", tmpInvalid.ToString());

            stats.Add("AdsSent", MessageAd.ToString()); //(MessageAd + AdEnterOtpScreenSent + VerificationAd).ToString()); //AdsSent.ToString());

            myJsonString = (new JavaScriptSerializer()).Serialize(stats);

        }
        // ReSharper disable once EmptyGeneralCatchClause
        catch (Exception ex)
        {
            myJsonString = ex.Message;
        }
        return myJsonString;
    }
}
