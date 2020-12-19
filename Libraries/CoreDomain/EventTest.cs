using System;
using System.Configuration;
using System.Web;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

using dk = MACServices.Constants.Dictionary.Keys;

namespace MACServices
{
    public class EventTest
    {
        // This is the instance based version
        public EventTest()
        {
            _id = ObjectId.GenerateNewId();
            ClientId = ObjectId.Parse(Constants.Strings.DefaultEmptyObjectId);
            Date = DateTime.UtcNow;
            EventTypeId = 0;
            EventTypeName= "";
            EventTypeDesc = "";
            UserId = ObjectId.Parse(Constants.Strings.DefaultEmptyObjectId);
            UserIpAddress = "";
        }

        public EventTest(string eventId)
        {

            var myEvent = Fetch(eventId);

            ClientId = myEvent.ClientId;
            Date = myEvent.Date;
            EventTypeId = myEvent.EventTypeId;
            EventTypeName = myEvent.EventTypeName;
            EventTypeDesc = myEvent.EventTypeDesc;
            UserId = myEvent.UserId;
            UserIpAddress = myEvent.UserIpAddress;
        }

        public ObjectId _id { get; set; }
        public ObjectId ClientId { get; set; }
        public DateTime Date { get; set; }
        public Int32 EventTypeId { get; set; }
        public string EventTypeName { get; set; }
        public string EventTypeDesc { get; set; }
        public ObjectId UserId { get; set; }
        public string UserIpAddress { get; set; }

        public void Create()
        {
            var mongoClient = new MongoClient(ConfigurationManager.ConnectionStrings[Constants.WebConfig.ConnectionStringKeys.MongoServer].ConnectionString);
            var server = mongoClient.GetServer();
            var db = server.GetDatabase(ConfigurationManager.AppSettings[Constants.WebConfig.AppSettingsKeys.MongoDbName]);

            try
            {
                var dbCollection = db.GetCollection("Event");
                dbCollection.Insert(this);
            }
            catch (Exception ex)
            {
                // TODO: need to not log another exception if this is an event for an exception
                var mDetails = ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
            }
            finally
            {
                server.Disconnect(); // http://stackoverflow.com/questions/14495975/why-is-it-recommended-not-to-close-a-mongodb-connection-anywhere-in-node-js-code
            }
        }

        public void Create(Tuple<int, string, string> pEventDetails, string details)
        {
            //if (HttpContext.Current != null) 
            //{ 
            //    UserIpAddress = String.IsNullOrEmpty(HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]) 
            //        ? HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] 
            //        : HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            //}
            EventTypeId = pEventDetails.Item1;
            EventTypeName = pEventDetails.Item2;

            if (string.IsNullOrEmpty(details))
            {
                // Why is the separator being replaced with a semi-colon?
                if (EventTypeName.Contains("Request") || EventTypeName.Contains("Response"))
                    EventTypeDesc = String.IsNullOrEmpty(EventTypeDesc) ? pEventDetails.Item3 : ReplaceTokens(pEventDetails.Item3, EventTypeDesc.Replace(":", ";"));
                else
                    EventTypeDesc = String.IsNullOrEmpty(EventTypeDesc) ? pEventDetails.Item3 : ReplaceTokens(pEventDetails.Item3, EventTypeDesc);
            }
            else
            {
                // Why is the separator being replaced with a semi-colon?
                if (EventTypeName.Contains("Request") || EventTypeName.Contains("Response"))
                    EventTypeDesc = String.IsNullOrEmpty(details) ? pEventDetails.Item3 : ReplaceTokens(pEventDetails.Item3, details.Replace(":", ";"));
                else
                    EventTypeDesc = String.IsNullOrEmpty(details) ? pEventDetails.Item3 : ReplaceTokens(pEventDetails.Item3, details);
            }
            if (EventTypeDesc.Contains("Exception"))
            {
                var txt = _id.ToString();
            }

            var mongoClient = new MongoClient(ConfigurationManager.ConnectionStrings[Constants.WebConfig.ConnectionStringKeys.MongoServer].ConnectionString);
            var server = mongoClient.GetServer();
            var db = server.GetDatabase(ConfigurationManager.AppSettings[Constants.WebConfig.AppSettingsKeys.MongoDbName]);

            try
            {
                var dbCollection = db.GetCollection("Event");
                dbCollection.Insert(this);

                EventStat myStat = new EventStat("Core", "Events", 1);
            }
            catch (Exception ex)
            {
                // TODO: need to not log another exception if this is an event for an exception
                var mDetails = ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
                EventStat myStat = new EventStat("Core", "Exceptions", 1);
            }
            finally
            {
                server.Disconnect(); // http://stackoverflow.com/questions/14495975/why-is-it-recommended-not-to-close-a-mongodb-connection-anywhere-in-node-js-code
            }
        }

        public string ReplaceTokens(string tokenizedFormat, string replacementValue)
        {
            if (replacementValue.Contains(Constants.TokenKeys.ItemSep))
            {
                var splitTokens = replacementValue.Split(char.Parse(Constants.TokenKeys.ItemSep));
                foreach (string currentToken in splitTokens)
                {
                    if (currentToken.Contains(Constants.TokenKeys.KVSep))
                    {
                        var splitToken = currentToken.Split(char.Parse(Constants.TokenKeys.KVSep));
                        var tokenKey = "[" + splitToken[0] + "]";
                        var tokenValue = splitToken[1];

                        tokenizedFormat = tokenizedFormat.Replace(tokenKey, tokenValue);
                    }
                }
            }
            else if (replacementValue.Contains(Constants.TokenKeys.KVSep))
            {
                var splitToken = replacementValue.Split(char.Parse(Constants.TokenKeys.KVSep));
                var tokenKey = "[" + splitToken[0] + "]";
                var tokenValue = splitToken[1];

                tokenizedFormat = tokenizedFormat.Replace(tokenKey, tokenValue);
            }
            else
            {
                return replacementValue;
            }
            return tokenizedFormat;
        }

        public Event Fetch(string eventId)
        {
            var mongoClient = new MongoClient(ConfigurationManager.ConnectionStrings[Constants.WebConfig.ConnectionStringKeys.MongoServer].ConnectionString);
            var server = mongoClient.GetServer();
            var db = server.GetDatabase(ConfigurationManager.AppSettings[Constants.WebConfig.AppSettingsKeys.MongoDbName]);

            Event myEvent = null;

            try
            {
                var query = Query.EQ("_id", ObjectId.Parse(eventId));

                MongoCollection mongoCollection = db.GetCollection("Event");
                myEvent = mongoCollection.FindOneAs<Event>(query);
            }
            catch (Exception ex)
            {
                var mDetails = ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
            }
            finally
            {
                server.Disconnect(); // http://stackoverflow.com/questions/14495975/why-is-it-recommended-not-to-close-a-mongodb-connection-anywhere-in-node-js-code
            }

            return myEvent;
        }
    }
}