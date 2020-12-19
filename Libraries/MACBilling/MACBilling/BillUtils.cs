using System;
using System.Web;
//using System.Web.Script.Serialization;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

using cnt = MACBilling.BillConstants;
using cs = MACServices.Constants.Strings;

namespace MACBilling
{
    public class BillUtils
    {
// ReSharper disable once EmptyConstructor
        public BillUtils() { }

        String collectionName = "Billing";

        public object Create(object myObject)
        {
            try
            {
                var mongoDBConnectionPool = (MongoDatabase)HttpContext.Current.Application[cs.MongoDB];
                var dbCollection = mongoDBConnectionPool.GetCollection(collectionName);
                dbCollection.Insert(myObject);
            }
// ReSharper disable once EmptyGeneralCatchClause
            catch (Exception ex)
            {
// ReSharper disable once UnusedVariable
                var errMsg = ex.ToString();
            }
            return this;
        }

        public object Archive(object myObject)
        {
            collectionName = "Archive";

            try
            {
                var mongoDBConnectionPool = (MongoDatabase)HttpContext.Current.Application[cs.MongoDB];
                var dbCollection = mongoDBConnectionPool.GetCollection(collectionName);
                dbCollection.Insert(myObject);
            }
// ReSharper disable once EmptyGeneralCatchClause
            catch (Exception ex)
            {
// ReSharper disable once UnusedVariable
                var errMsg = ex.ToString();
            }
            return this;
        }

        public BillArchive GetBillArchiveForMonth(string clientid, string MonthYear)
        {
            try
            {
                var mongoDBConnectionPool = (MongoDatabase) HttpContext.Current.Application[cs.MongoDB];

                var archiveQuery = Query.And(Query.EQ("_t", "BillArchive"), 
                    Query.EQ("OwnerId", ObjectId.Parse(clientid.Trim())),
                    Query.EQ("ForMonthYear", MonthYear.Trim())
                    );
                MongoCollection archiveCollection = mongoDBConnectionPool.GetCollection("Archive");
                var myBillArchive = archiveCollection.FindOneAs<BillArchive>(archiveQuery);
                return myBillArchive;
            }
            catch
            {
                return null;
            }
        }


        public Object Read(string objectId, string objectType)
        {
            try
            {
                var mongoDBConnectionPool = (MongoDatabase)HttpContext.Current.Application[cs.MongoDB];

                switch (objectType.ToLower())
                {
                    case "billaddendum":
                        var addendumQuery = Query.And(Query.EQ("OwnerId", ObjectId.Parse(objectId)), Query.EQ("_t", "BillAddendum"));
                        MongoCollection addendumCollection = mongoDBConnectionPool.GetCollection("Billing");
                        var myBillAddendum = addendumCollection.FindOneAs<BillAddendum>(addendumQuery);
                        return myBillAddendum;

                    case "billarchive":
                        var archiveQuery = Query.And(Query.EQ("_id", ObjectId.Parse(objectId)), Query.EQ("_t", "BillArchive"));
                        MongoCollection archiveCollection = mongoDBConnectionPool.GetCollection("Archive");
                        var myBillArchive = archiveCollection.FindOneAs<BillArchive>(archiveQuery);
                        return myBillArchive;

                    case "billconfig":
                        var configQuery = Query.And(Query.EQ("OwnerId", ObjectId.Parse(objectId)), Query.EQ("ObjectType", "BillConfig"));
                        MongoCollection configCollection = mongoDBConnectionPool.GetCollection("Billing");
                        var myBillConfig = configCollection.FindOneAs<BillConfig>(configQuery);
                        return myBillConfig;

                    case "billclient":
                        var clientQuery = Query.And(Query.EQ("OwnerId", ObjectId.Parse(objectId)), Query.EQ("_t", "BillClient"));
                        MongoCollection clientCollection = mongoDBConnectionPool.GetCollection("Billing");
                        var myBillClient = clientCollection.FindOneAs<BillClient>(clientQuery);
                        return myBillClient;

                    case "billgroup":
                        var groupQuery = Query.And(Query.EQ("OwnerId", ObjectId.Parse(objectId)), Query.EQ("_t", "BillGroup"));
                        MongoCollection groupCollection = mongoDBConnectionPool.GetCollection("Billing");
                        var myBillGroup = groupCollection.FindOneAs<BillGroup>(groupQuery);
                        return myBillGroup;
                }
            }
            catch (Exception ex)
            {
// ReSharper disable once UnusedVariable
                var errMsg = ex.ToString();
            }
            return null;
        }

        public BillAddendum ReadAddendum(string addendumId)
        {
            try
            {
                var mongoDBConnectionPool = (MongoDatabase)HttpContext.Current.Application[cs.MongoDB];

                var addendumQuery = Query.And(Query.EQ("_id", ObjectId.Parse(addendumId)), Query.EQ("_t", "BillAddendum"));
                MongoCollection addendumCollection = mongoDBConnectionPool.GetCollection("Billing");
                var myBillAddendum = addendumCollection.FindOneAs<BillAddendum>(addendumQuery);
                return myBillAddendum;
            }
            catch (Exception ex)
            {
// ReSharper disable once UnusedVariable
                var errMsg = ex.ToString();
            }
            return null;
        }

        public string Update(Object objectToUpdate, string objectId)
        {
            var updateMessage = "update succeeded";

            try
            {
                var mongoDBConnectionPool = (MongoDatabase)HttpContext.Current.Application[cs.MongoDB];

                var mongoCollection = mongoDBConnectionPool.GetCollection("Billing");
                //var query = Query.EQ("_id", ObjectId.Parse(objectId));
                //var sortBy = SortBy.Descending("_id");

                mongoCollection.Save(objectToUpdate);

                //mongoCollection.FindAndRemove(query, sortBy);
                //mongoCollection.Insert(objectToUpdate);
            }
            catch (Exception ex)
            {
// ReSharper disable once UnusedVariable
                var errMsg = ex.ToString();
            }
            return updateMessage;
        }

        public string DeleteAddendum(string addendumId)
        {
            var updateMessage = "update succeeded";

            try
            {
                var mongoDBConnectionPool = (MongoDatabase)HttpContext.Current.Application[cs.MongoDB];

                var mongoCollection = mongoDBConnectionPool.GetCollection("Billing");
                var query = Query.EQ("_id", ObjectId.Parse(addendumId));
                var sortBy = SortBy.Descending("_id");

                mongoCollection.FindAndRemove(query, sortBy);
            }
            catch (Exception ex)
            {
// ReSharper disable once UnusedVariable
                var errMsg = ex.ToString();
            }
            return updateMessage;
        }

        public string UpdateArchive(Object objectToUpdate, string objectId)
        {
            var updateMessage = "update succeeded";

            try
            {
                var mongoDBConnectionPool = (MongoDatabase)HttpContext.Current.Application[cs.MongoDB];

                var mongoCollection = mongoDBConnectionPool.GetCollection("Archive");
                //var query = Query.EQ("_id", ObjectId.Parse(objectId));
                //var sortBy = SortBy.Descending("_id");

                mongoCollection.Save(objectToUpdate);

                //mongoCollection.FindAndRemove(query, sortBy);
                //mongoCollection.Insert(objectToUpdate);
            }
            catch (Exception ex)
            {
// ReSharper disable once UnusedVariable
                var errMsg = ex.ToString();
            }
            return updateMessage;
        }

        public string UpdateAddendum(Object objectToUpdate, string objectId)
        {
            var updateMessage = "update succeeded";

            try
            {
                var mongoDBConnectionPool = (MongoDatabase)HttpContext.Current.Application[cs.MongoDB];

                var mongoCollection = mongoDBConnectionPool.GetCollection("Billing");
                //var query = Query.And(Query.EQ("_t", "BillAddendum"), Query.EQ("_id", ObjectId.Parse(objectId)));
                //var sortBy = SortBy.Descending("_id");

                mongoCollection.Save(objectToUpdate);

                //mongoCollection.FindAndRemove(query, sortBy);
                //mongoCollection.Insert(objectToUpdate);
            }
            catch (Exception ex)
            {
// ReSharper disable once UnusedVariable
                var errMsg = ex.ToString();
            }
            return updateMessage;
        }

        public string UpdateConfig(BillConfig objectToUpdate, string ownerId)
        {
            var updateMessage = "update succeeded";

            try
            {
                var mongoDBConnectionPool = (MongoDatabase)HttpContext.Current.Application[cs.MongoDB];

                var mongoCollection = mongoDBConnectionPool.GetCollection("Billing");
                //var query = Query.And(Query.EQ("OwnerId", ObjectId.Parse(ownerId)), Query.EQ("ObjectType", "BillConfig"));
                //var sortBy = SortBy.Descending("_id");

                mongoCollection.Save(objectToUpdate);

                //mongoCollection.FindAndRemove(query, sortBy);
                //mongoCollection.Insert(objectToUpdate);
            }
            catch (Exception ex)
            {
// ReSharper disable once UnusedVariable
                var errMsg = ex.ToString();
            }
            return updateMessage;
        }

        public string FormatCost(Decimal moneyAmount)
        {
            var decimalPlaces = 2;

            if (moneyAmount.ToString().IndexOf(".", StringComparison.Ordinal) > -1)
            {
                var tmpVal = moneyAmount.ToString().Split('.');

                decimalPlaces = tmpVal[1].ToString().Length;
            }

            // Force at least 2 decimal places
            if (decimalPlaces < 2)
                decimalPlaces = 2;

            var formattedMoney = moneyAmount.ToString("C" + decimalPlaces.ToString());

            return formattedMoney;
        }

        public string FormatMoney(Decimal moneyAmount)
        {
            var formattedMoney = moneyAmount.ToString("C2");

            return formattedMoney;
        }

        public string FormatNumber(Decimal numberAmount)
        {
            var formattedNumber = numberAmount.ToString("#,###,##0.#####");

            return formattedNumber;
        }
    }
}
