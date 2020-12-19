using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

using dk = MACServices.Constants.Common;
using cs = MACServices.Constants.Strings;

namespace MACServices
{
    public class EventStat
    {
        public EventStat(ObjectId ownerId)
        {
            // ownerId to help with billing
            OwnerId = ownerId;

// ReSharper disable once RedundantCast
            var myStat = (EventStat)Read();

            if (myStat != null)
                DailyStats = myStat.DailyStats;
        }

        public EventStat(ObjectId ownerId, string ownerName, string statName, int incrementValue)
        {
            if (ownerId.ToString() == Constants.Strings.DefaultEmptyObjectId) return;

            try
            {
                OwnerId = ownerId;
                OwnerName = ownerName;

                StatCollection = MyDB.GetCollection("EventStat");

// ReSharper disable once RedundantCast
                var mySats = (EventStat)Read();
                if (mySats == null)
                {
                    var myNewStat = new EventStatDay();
                    foreach (var property in myNewStat.GetType().GetProperties())
                    {
                        if (statName == property.Name)
                        {
                            property.SetValue(myNewStat, incrementValue);

                            myNewStat.Events += 1;

                            DailyStats = new List<EventStatDay>();
                            DailyStats.Add(myNewStat);

                            Create();
                            return;
                        }
                    }
                }
                else
                {
                    DailyStats = mySats.DailyStats;

                    var dayIndex = 0;
                    var hasTodayStats = false;

                    foreach (var currentDayStat in DailyStats)
                    {
                        // Only update Today's stats
                        if (currentDayStat.Date.ToShortDateString() == CurrentDate.ToShortDateString())
                        {
                            hasTodayStats = true;
                            foreach (PropertyInfo property in currentDayStat.GetType().GetProperties())
                            {
                                if (statName == property.Name)
                                {
                                    var statQuery = Query.EQ("OwnerId", OwnerId);
                                    var statSortBy = SortBy.Ascending("OwnerId");

                                    var updateElementPath = "DailyStats." + dayIndex + "." + property.Name;
                                    var updateEventPath = "DailyStats." + dayIndex + ".Events";
                                    var statUpdate = MongoDB.Driver.Builders.Update.Inc(updateElementPath, incrementValue).Inc(updateEventPath, 1);

                                    StatCollection.FindAndModify(statQuery, statSortBy, statUpdate, false);

                                    return;
                                }
                            }
                        }
                        dayIndex++;
                    }

                    if (!hasTodayStats)
                    {
                        var myNewStat = new EventStatDay();
                        foreach (var property in myNewStat.GetType().GetProperties())
                        {
                            if (statName == property.Name)
                            {
                                property.SetValue(myNewStat, incrementValue);

                                myNewStat.Events += 1;

                                DailyStats.Add(myNewStat);

                                Update();
                                return;
                            }
                        }
                    }
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
            }
        }

        public ObjectId _id { get; set; }
        public ObjectId OwnerId { get; set; }
        public string OwnerName { get; set; }

        private DateTime CurrentDate = DateTime.UtcNow.Date;
        
        public List<EventStatDay> DailyStats { get; set; }

        private MongoDatabase MyDB = (MongoDatabase)HttpContext.Current.Application[cs.MongoDB];
        private MongoCollection StatCollection { get; set; }

        public EventStat Read()
        {
            try
            {
                StatCollection = MyDB.GetCollection("EventStat");

                var query = Query.EQ("OwnerId", OwnerId);
                var myEventStatOwner = StatCollection.FindOneAs<EventStat>(query);

                return myEventStatOwner;
            }
            catch (Exception ex)
            {
// ReSharper disable once UnusedVariable
                var errMsg = ex.Message;
            }
            return null;
        }

        public void Create()
        {
            try
            {
                StatCollection.Insert(this);
            }
            catch (Exception ex)
            {
// ReSharper disable once UnusedVariable
                var errMsg = ex.Message;
            }
        }

        public void Update()
        {
            try
            {
                // Use strongly typed query and update
                var statQuery = Query<EventStat>.EQ(i => i.OwnerId, OwnerId);
                var statSortBy = SortBy.Descending("_id");
                var statUpdate = Update<EventStat>.Set(i => i.DailyStats, DailyStats);

                if (MyDB == null)
                    MyDB = (MongoDatabase)HttpContext.Current.Application[cs.MongoDB];

                if (StatCollection == null)
                    StatCollection = MyDB.GetCollection("EventStat");

                StatCollection.FindAndModify(statQuery, statSortBy, statUpdate);
            }
            catch (Exception ex)
            {
// ReSharper disable once UnusedVariable
                var errMsg = ex.Message;
            }
        }
    }
}
