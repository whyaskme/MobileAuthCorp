using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using cfgcs = MACServices.Constants.WebConfig.ConnectionStringKeys;
using cs = MACServices.Constants.Strings;

namespace MACServices
{
    public class EventList
    {
        public EventList(string clientId, string eventTypeName, int startRecordNumber, int numberOfRecords, string sortField, string sortDirection, string startDate, string endDate, string attributes)
        {
            if (String.IsNullOrEmpty(clientId))
                clientId = "";

            if (clientId == Constants.Strings.DefaultEmptyObjectId)
                clientId = "";

            if (String.IsNullOrEmpty(eventTypeName))
                eventTypeName = "";

            if (startRecordNumber < 1)
                startRecordNumber = 0;

            if (numberOfRecords < 1)
                numberOfRecords = 10;

            if (String.IsNullOrEmpty(sortField))
                sortField = "Date";

            if (String.IsNullOrEmpty(sortDirection))
                sortDirection = "descending";

            sortDirection = sortDirection.ToLower();

            var sd = String.IsNullOrEmpty(startDate) ? DateTime.UtcNow.AddDays(-30) : DateTime.Parse(startDate);
            var ed = String.IsNullOrEmpty(endDate) ? DateTime.UtcNow.AddDays(1) : DateTime.Parse(endDate);

            var result = DateTime.Compare(sd, ed);
            if (result == 0)
            {
                ed = ed.AddDays(1); // start date is the same as end date
                ed = ed.AddSeconds(-1);
            }
            else if (result > 0)
                sd = ed.AddDays(-1); // start date is greater than end date

            var attributeArray = new string[0];
            List<string> attribList = new List<string>();

            if (attributes.IndexOf(",", StringComparison.Ordinal) > -1)
            {
                attributeArray = attributes.Split(',');
                foreach(var currAttrib in attributeArray)
                {
                    attribList.Add(currAttrib);
                }
            }

            var dbCollection = "Event";

            var query = Query.Null;

            switch(eventTypeName)
            {
                case "AdsSent":
                    dbCollection = "Otp";
                    if (!String.IsNullOrEmpty(clientId) && clientId != cs.DefaultEmptyObjectId)
                    {
                        query = Query.And(
                            Query.GT("CodeHistory.Date", sd),
                            Query.LT("CodeHistory.Date", ed),
                            Query.EQ("ClientId", ObjectId.Parse(clientId)),
                            Query.EQ("AdDetails.Status", "Ad included in message")
                            );
                    }
                    else
                    {
                        query = Query.And(
                            Query.GT("CodeHistory.Date", sd),
                            Query.LT("CodeHistory.Date", ed),
                            Query.EQ("AdDetails.Status", "Ad included in message")
                            );
                    }
                    break;

                case "OtpSent":
                    dbCollection = "Otp";
                    
                    // Add additional attributes
                    attribList.Add("AdDetails");
                    attribList.Add("CodeHistory");

                    if (!String.IsNullOrEmpty(clientId) && clientId != cs.DefaultEmptyObjectId)
                    {
                        query = Query.And(
                            Query.GT("CodeHistory.Date", sd),
                            Query.LT("CodeHistory.Date", ed),
                            Query.EQ("ClientId", ObjectId.Parse(clientId)),
                            Query.EQ("CodeHistory.EventTypeId", 3003)
                            );
                    }
                    else
                    {
                        query = Query.And(
                            Query.GT("CodeHistory.Date", sd),
                            Query.LT("CodeHistory.Date", ed),
                            Query.EQ("CodeHistory.EventTypeId", 3003)
                            );
                    }
                    break;

                case "OtpValid":
                    dbCollection = "Otp";
                    if (!String.IsNullOrEmpty(clientId) && clientId != cs.DefaultEmptyObjectId)
                    {
                        query = Query.And(
                            Query.GT("CodeHistory.Date", sd),
                            Query.LT("CodeHistory.Date", ed),
                            Query.EQ("ClientId", ObjectId.Parse(clientId)),
                            Query.EQ("CodeHistory.EventTypeId", 3005)
                            );
                    }
                    else
                    {
                        query = Query.And(
                            Query.GT("CodeHistory.Date", sd),
                            Query.LT("CodeHistory.Date", ed),
                            Query.EQ("CodeHistory.EventTypeId", 3005)
                            );
                    }
                    break;

                case "OtpInvalid":
                    dbCollection = "Otp";
                    break;

                case "Users":
                    dbCollection = "EndUser";
                    break;

                default:
                    //dbCollection = "Event";
                    if (!String.IsNullOrEmpty(clientId) && !String.IsNullOrEmpty(eventTypeName))
                    {
                        query = Query.And(
                            Query.GT("Date", sd),
                            Query.LT("Date", ed),
                            Query.EQ("ClientId", ObjectId.Parse(clientId)),
                            Query.EQ("EventTypeName", eventTypeName)
                            );
                    }
                    else if (!String.IsNullOrEmpty(clientId))
                    {
                        query = Query.And(
                            Query.GT("Date", sd),
                            Query.LT("Date", ed),
                            Query.EQ("ClientId", ObjectId.Parse(clientId))
                            );
                    }
                    else if (!String.IsNullOrEmpty(eventTypeName))
                    {
                        query = Query.And(
                            Query.GT("Date", sd),
                            Query.LT("Date", ed),
                            Query.EQ("EventTypeName", eventTypeName)
                            );
                    }
                    else
                    {
                        query = Query.And(Query.GT("Date", sd), Query.LT("Date", ed));
                    }
                    break;
            }

            ListItems = new List<ListItem>();

            var db = (MongoDatabase)HttpContext.Current.Application[cs.MongoDB];

            MongoCursor<BsonDocument> mongoCollection;

            int eventCount;

            eventCount = Convert.ToInt32(db.GetCollection(dbCollection).Find(query).Count());
            mongoCollection = db.GetCollection(dbCollection).Find(query).SetSkip(startRecordNumber).SetLimit(numberOfRecords).SetSortOrder(sortDirection.ToLower() == "descending" ? SortBy.Descending(sortField) : SortBy.Ascending(sortField)).SetFields(attribList.ToArray()); // attributeArray

            if (mongoCollection == null) return;

            // Process the query response into a return list
            switch (eventTypeName)
            {
                case "AdsSent":

                    break;

                case "OtpSent":
                    foreach (var doc in mongoCollection)
                    {
                        var item = new ListItem { Value = eventTypeName, Text = eventTypeName };

                        item.Attributes.Add("EventCount", eventCount.ToString(CultureInfo.CurrentCulture));

                        foreach (var element in doc.Elements)
                        {
                            switch (element.Name)
                            {
                                case "_id":
                                    item.Attributes.Add(element.Name, element.Value.ToString());
                                    break;

                                case "AdDetails":
                                    item.Attributes.Add("", element.Value.ToString());
                                    break;

                                case "CodeHistory":
                                    item.Attributes.Add(element.Name, element.Value.ToString());
                                    break;

                                case "Date":
                                    item.Attributes.Add("date", element.Value.ToString());
                                    break;

                                case "EventTypeId":
                                    item.Attributes.Add(element.Name, element.Value.ToString());
                                    break;

                                case "EventTypeName":
                                    item.Attributes.Add(element.Name, element.Value.ToString());
                                    break;

                                case "EventTypeDesc":
                                    item.Attributes.Add(element.Name, element.Value.ToString());
                                    break;
                            }
                        }
                        ListItems.Add(item);
                    }
                    break;

                case "OtpValid":

                    break;

                case "OtpInvalid":

                    break;

                case "Users":

                    break;

                default:
                    //dbCollection = "Event";
                    foreach (var doc in mongoCollection)
                    {
                        var item = new ListItem { Value = doc[0].ToString(), Text = doc[1].ToString() };

                        item.Attributes.Add("EventCount", eventCount.ToString(CultureInfo.CurrentCulture));

                        if (attributeArray.Length > 0)
                        {
                            foreach (var element in doc.Elements.Where(element => attributeArray.Contains(element.Name)))
                            {
                                item.Attributes.Add(element.Name, element.Value.ToString());
                            }
                        }

                        ListItems.Add(item);
                    }
                    break;
            }
        }

        public List<ListItem> ListItems { get; set; }
    }
}