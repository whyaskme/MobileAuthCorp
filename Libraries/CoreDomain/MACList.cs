using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using System.Web;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using cfgcs = MACServices.Constants.WebConfig.ConnectionStringKeys;
using cs = MACServices.Constants.Strings;

namespace MACServices
{
    public class MacList
    {
        public MacList(string clientId, string collectionName, string objectType, string attributes)
        {
            var attributeArray = new string[0];

            if (attributes.IndexOf(",", StringComparison.Ordinal) > -1)
                attributeArray = attributes.Split(',');

            ListItems = new List<ListItem>();

            // Get all available providers
            var mongoDBConnectionPool = (MongoDatabase)HttpContext.Current.Application[cs.MongoDB];

            if (!String.IsNullOrEmpty(clientId))
            {
                var query = Query.EQ("clientId", ObjectId.Parse(clientId));
                var mongoCollection = mongoDBConnectionPool.GetCollection(collectionName).Find(query).SetFields(Fields.Include("_id", "Name"));
                var sortOrder = new[] {"Name"};
                mongoCollection.SetSortOrder(sortOrder);

                foreach (var doc in mongoCollection)
                {
                    var item = new ListItem {Value = doc[0].ToString(), Text = doc[1].ToString()};

                    if (attributeArray.Length > 0)
                    {
                        foreach (var element in doc.Elements.Where(element => attributeArray.Contains(element.Name)))
                        {
                            item.Attributes.Add(element.Name, element.Value.ToString());
                        }
                    }
                    ListItems.Add(item);
                }
            }
            else
            {
                var mongoCollection = mongoDBConnectionPool.GetCollection(collectionName).FindAll();
                if (!String.IsNullOrEmpty(objectType))
                {
                    var query = Query.EQ("_t", objectType);
                    mongoCollection =
                        mongoDBConnectionPool.GetCollection(collectionName).Find(query).SetFields(Fields.Include("_id", "Name"));
                }
                var sortOrder = new[] {"Name"};
                mongoCollection.SetSortOrder(sortOrder);

                foreach (var doc in mongoCollection)
                {
                    var item = new ListItem {Value = doc[0].ToString(), Text = doc[1].ToString()};

                    if (attributeArray.Length > 0)
                    {
                        foreach (var element in doc.Elements.Where(element => attributeArray.Contains(element.Name)))
                        {
                            item.Attributes.Add(element.Name, element.Value.ToString());
                        }
                    }
                    ListItems.Add(item);
                }
            }
        }

        public List<ListItem> ListItems { get; set; }
    }
}