using System;
using System.Web;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

using dk = MACServices.Constants.Dictionary.Keys;
using es = MACServices.Constants.EventStats;
using cs = MACServices.Constants.Strings;

namespace MACServices
{
    public class Event
    {
        public Event()
        {
            myDB = (MongoDatabase)HttpContext.Current.Application[cs.MongoDB];

            _id = ObjectId.GenerateNewId();
            ClientId = ObjectId.Parse(Constants.Strings.DefaultEmptyObjectId);
            Date = DateTime.UtcNow;
            EventTypeId = 0;
            EventTypeName= "";
            EventTypeDesc = "";
            UserId = ObjectId.Parse(Constants.Strings.DefaultEmptyObjectId);
            UserIpAddress = "";
        }

        public Event(string eventId)
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
        private MongoDatabase myDB { get; set; }

        public void Create()
        {
            try
            {
                var dbCollection = myDB.GetCollection("Event");
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
        }

        public void Create(Tuple<int, string, string> pEventDetails, string details)
        {
            if (HttpContext.Current != null) 
            { 
                UserIpAddress = String.IsNullOrEmpty(HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]) 
                    ? HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] 
                    : HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            }

            if (UserIpAddress == "::1")
                UserIpAddress = "localhost";

            EventTypeId = pEventDetails.Item1;
            EventTypeName = pEventDetails.Item2;

            if (string.IsNullOrEmpty(details))
            {
                // Why is the separator being replaced with a semi-colon? Because the display method in admin splits on a colon
                if (EventTypeName.Contains("Request") || EventTypeName.Contains("Response"))
                    EventTypeDesc = String.IsNullOrEmpty(EventTypeDesc) ? pEventDetails.Item3 : ReplaceTokens(pEventDetails.Item3, EventTypeDesc.Replace(":", ";"));
                else
                    EventTypeDesc = String.IsNullOrEmpty(EventTypeDesc) ? pEventDetails.Item3 : ReplaceTokens(pEventDetails.Item3, EventTypeDesc);
            }
            else
            {
                // Why is the separator being replaced with a semi-colon? Because the display method in admin splits on a colon
                if (EventTypeName.Contains("Request") || EventTypeName.Contains("Response"))
                    EventTypeDesc = String.IsNullOrEmpty(details) ? pEventDetails.Item3 : ReplaceTokens(pEventDetails.Item3, details.Replace(":", ";"));
                else
                    EventTypeDesc = String.IsNullOrEmpty(details) ? pEventDetails.Item3 : ReplaceTokens(pEventDetails.Item3, details);
            }
            try
            {
                var dbCollection = myDB.GetCollection("Event");
                dbCollection.Insert(this);
                // Update the exception event stats if an exception event
                if ((EventTypeId >= Constants.EventLogExceptionStartRange) && (EventTypeId <= Constants.EventLogGroupStartRange - 1))
                {
                    var mUtils = new Utils();
                    // ReSharper disable once UnusedVariable
                    var myStat = new EventStat(ClientId, mUtils.GetClientNameUsingId(ClientId.ToString()), es.Exceptions, 1);
                }
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
        }

        public string ReplaceTokens(string tokenizedFormat, string replacementValue)
        {
            if (replacementValue.Contains(Constants.TokenKeys.ItemSep))
            {
                var splitTokens = replacementValue.Split(char.Parse(Constants.TokenKeys.ItemSep));
                foreach (var currentToken in splitTokens)
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
            Event myEvent = null;
            try
            {
                var query = Query.EQ("_id", ObjectId.Parse(eventId));

                myDB = (MongoDatabase)HttpContext.Current.Application[cs.MongoDB];

                MongoCollection mongoCollection = myDB.GetCollection("Event");
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
            return myEvent;
        }
    }
}