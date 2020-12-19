using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Web;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using cfgcs = MACServices.Constants.WebConfig.ConnectionStringKeys;
using cs = MACServices.Constants.Strings;

namespace MACServices
{
    public class MacListAdHoc
    {
        public MacListAdHoc(string collectionName, string queryField, string queryValue, bool bReturnDecendants, string attributes)
        {
            // Prepend "_id,Name,Enabled". This ensures all objects will contain id, name and enabled properties. No need to pass in from callee.
            if (attributes == "")
                attributes = "_id,Name,Enabled";
            else
                attributes = "_id,Name,Enabled," + attributes;
            // Get all available providers

            SbResponse = new StringBuilder();

            SbResponse.Append("<" + collectionName.ToLower() + "s>");

            // Process data
            ProcessRootItems(collectionName, queryField, queryValue, bReturnDecendants, attributes);

            // End element tag
            SbResponse.Append("</" + collectionName.ToLower() + "s>");

            ListXml = new XmlDocument();
            ListXml.LoadXml(SbResponse.ToString());

        }
        private MongoDatabase mongoDBConnectionPool = (MongoDatabase)HttpContext.Current.Application[cs.MongoDB];

        public string[] ArrAttributes = { "EMPTY" };
        public string[] ArrDecendantIDs = { "EMPTY" };
        public int ItemCount;
        public int ParentLevelNumber;
        public StringBuilder SbResponse = new StringBuilder();
        public string[] SortOrder = null;

        public MongoClient MongoClient { get; set; }
        public MongoServer MongoServer { get; set; }
        private MongoDatabase MongoDatabase { get; set; }
        public MongoCursor MongoCollection { get; set; }

        public XmlDocument ListXml { get; set; }

        public void ProcessRootItems(string collectionName, string queryField, string queryValue,
            bool bReturnDecendants, string attributes)
        {
            var mUtils = new Utils();
            ItemCount++;

            var attributeArray = new string[0];

            if (attributes.IndexOf(",", System.StringComparison.Ordinal) > -1)
                attributeArray = attributes.Split(',');

            var query = Query.EQ(queryField, queryValue);

            var isObjectId = mUtils.IsValueObjectId(queryValue);
            if (isObjectId)
                query = Query.EQ(queryField, ObjectId.Parse(queryValue));

            MongoCollection = mongoDBConnectionPool.GetCollection(collectionName).Find(query);

            if (attributeArray.Length > 0)
            {
                // * indicates return all fields
                if (attributeArray[attributeArray.Length-1] != "*")
                    MongoCollection = mongoDBConnectionPool.GetCollection(collectionName).Find(query).SetFields(attributeArray);
            }

            switch (collectionName)
            {
                case "Event":
                    SortOrder = new[] { "Date" };
                    break;
                default:
                    SortOrder = new[] { "Name" };
                    break;
            }

            MongoCollection.SetSortOrder(SortOrder);

            foreach (BsonDocument doc in MongoCollection)
            {
                var itemName = GetDocElementValueByName("name", doc);
                var itemId = GetDocElementValueByName("_id", doc);
                var itemEnabled = GetDocElementValueByName("enabled", doc);

                SbResponse.Append("<" + collectionName.ToLower() +
                                  " name='" + SanitizeXml(itemName) + "'" +
                                  " id='" + itemId + "'" +
                                  " heirarchy='" + ParentLevelNumber + "'" +
                                  " enabled='" + itemEnabled + "'" +
                                  " parentid='parentId???'" +
                                  " >");

                if (attributeArray.Length > 0)
                {
                    foreach (var element in doc.Elements.Where(element => attributeArray.Contains(element.Name)))
                    {
                        if (element.Name.ToLower() == "relationships")
                        {
                            SbResponse.Append("<" + element.Name.ToLower().Replace("_", "") + ">");

                            var relationships = element.Value.ToString();
                            var sanitizedRelationships =
                                relationships.Replace("\"", "").Replace("[", "").Replace("]", "");

                            var arrRelationships = sanitizedRelationships.Split('}');

                            foreach (var t in arrRelationships)
                            {
                                var currentRelationship =
                                    t.Replace("{ ", "")
                                        .Replace("ObjectId(", "")
                                        .Replace(")", "")
                                        .Replace(" : ", ":");

                                if (currentRelationship == "") continue;
                                if (currentRelationship.StartsWith(", "))
                                    currentRelationship =
                                        currentRelationship.Substring(2, currentRelationship.Length - 2).Trim();

                                var arrRelationshipElements = currentRelationship.Split(',');

                                //var memberEnabled = arrRelationshipElements[0].Trim().Replace("Enabled:", "");
                                var memberId = arrRelationshipElements[1].Trim().Replace("MemberId:", "");
                                var memberType = arrRelationshipElements[2].Trim().Replace("MemberType:", "");
                                var memberHierarchy = arrRelationshipElements[3].Trim().Replace("MemberHierarchy:", "");

                                switch (memberType)
                                {
                                    case "Group":
                                        switch (memberHierarchy)
                                        {
                                            case "Child":
                                                ProcessDecendants(1, collectionName, "_id", memberId, true, attributes);
                                                break;

                                            case "Parent":
                                                SbResponse.Replace("parentId???", memberId);
                                                break;
                                        }
                                        break;

                                    case "Administrator":
                                        SbResponse.Append("<" + memberType.ToLower() + " id='" + memberId + "' />");
                                        break;

                                    case "Client":
                                        SbResponse.Append("<" + memberType.ToLower() + " id='" + memberId + "' />");
                                        break;
                                }
                            }

                            SbResponse.Append("</" + element.Name.ToLower().Replace("_", "") + ">");
                        }
                        else
                        {
                            // Don't include id or name as they are defined as attributes in the element definition tag
                            if (element.Name.ToLower() == "_id" || element.Name.ToLower() == "name" || element.Name.ToLower() == "enabled") continue;
                            SbResponse.Append("<" + element.Name.ToLower().Replace("_", "") + ">");
                            // Wrap in CData so we don't have xml parsing issues with entity characters
                            SbResponse.Append(SanitizeXml(element.Value.ToString()));
                            SbResponse.Append("</" + element.Name.ToLower().Replace("_", "") + ">");
                        }
                    }
                    ItemCount++;
                }
                SbResponse.Append("</" + collectionName.ToLower() + ">");
            }
        }

        public void ProcessDecendants(int heirarchyNumber, string collectionName, string queryField, string queryValue, bool bReturnDecendants, string attributes)
        {
            var mUtils = new Utils();
            ItemCount++;

            var attributeArray = new string[0];

            if (attributes.IndexOf(",", System.StringComparison.Ordinal) > -1)
                attributeArray = attributes.Split(',');

            var query = Query.EQ(queryField, queryValue);

            var isObjectId = mUtils.IsValueObjectId(queryValue);
            if (isObjectId)
                query = Query.EQ(queryField, ObjectId.Parse(queryValue));

            MongoCollection = mongoDBConnectionPool.GetCollection(collectionName).Find(query);
            if (attributeArray.Length > 0)
            {
                // * indicates return all fields
                if (attributeArray[attributeArray.Length - 1] != "*")
                    MongoCollection = mongoDBConnectionPool.GetCollection(collectionName).Find(query).SetFields(attributeArray);
            }

            SortOrder = new[] { "Name" };
            MongoCollection.SetSortOrder(SortOrder);

            foreach (BsonDocument doc in MongoCollection)
            {
                var itemName = GetDocElementValueByName("name", doc);
                var itemId = GetDocElementValueByName("_id", doc);
                var itemEnabled = GetDocElementValueByName("enabled", doc);

                SbResponse.Append("<" + collectionName.ToLower() +
                                  " name='" + SanitizeXml(itemName) + "'" +
                                  " id='" + itemId + "'" +
                                  " heirarchy='" + heirarchyNumber + "'" +
                                  " enabled='" + itemEnabled + "'" +
                                  " parentid='parentId???'" +
                                  " >");

                if (attributeArray.Length > 0)
                {
                    foreach (var element in doc.Elements.Where(element => attributeArray.Contains(element.Name)))
                    {
                        if (element.Name.ToLower() == "relationships")
                        {
                            SbResponse.Append("<" + element.Name.ToLower().Replace("_", "") + ">");

                            var relationships = element.Value.ToString();
                            var sanitizedRelationships =
                                relationships.Replace("\"", "").Replace("[", "").Replace("]", "");

                            var arrRelationships = sanitizedRelationships.Split('}');

                            foreach (var t in arrRelationships)
                            {
                                var currentRelationship =
                                    t.Replace("{ ", "")
                                        .Replace("ObjectId(", "")
                                        .Replace(")", "")
                                        .Replace(" : ", ":");

                                if (currentRelationship == "") continue;
                                if (currentRelationship.StartsWith(", "))
                                    currentRelationship =
                                        currentRelationship.Substring(2, currentRelationship.Length - 2).Trim();

                                var arrRelationshipElements = currentRelationship.Split(',');

                                //var memberEnabled = arrRelationshipElements[0].Trim().Replace("Enabled:", "");
                                var memberId = arrRelationshipElements[1].Trim().Replace("MemberId:", "");
                                var memberType = arrRelationshipElements[2].Trim().Replace("MemberType:", "");
                                var memberHierarchy =
                                    arrRelationshipElements[3].Trim().Replace("MemberHierarchy:", "");

                                switch (memberType)
                                {
                                    case "Group":

                                        switch (memberHierarchy)
                                        {
                                            case "Child":
                                                ProcessDecendants((heirarchyNumber + 1), collectionName, "_id", memberId, true, attributes);
                                                break;

                                            case "Parent":
                                                SbResponse.Replace("parentId???", memberId);
                                                break;
                                        }
                                        break;

                                    case "Administrator":
                                        SbResponse.Append("<" + memberType.ToLower() +
                                                            " id='" + memberId + "'" +
                                                          " />");
                                        break;

                                    case "Client":
                                        SbResponse.Append("<" + memberType.ToLower() +
                                                            " id='" + memberId + "'" +
                                                          " />");
                                        break;
                                }
                            }

                            SbResponse.Append("</" + element.Name.ToLower().Replace("_", "") + ">");
                        }
                        else
                        {
                            // Don't include id or name as they are defined as attributes in the element definition tag
                            if (element.Name.ToLower() == "_id" || element.Name.ToLower() == "name" || element.Name.ToLower() == "enabled") continue;
                            SbResponse.Append("<" + element.Name.ToLower().Replace("_", "") + ">");
                            SbResponse.Append(SanitizeXml(element.Value.ToString()));
                            SbResponse.Append("</" + element.Name.ToLower().Replace("_", "") + ">");
                        }
                    }
                    ItemCount++;
                }
                SbResponse.Append("</" + collectionName.ToLower() + ">");
            }
        }

        public string SanitizeXml(string inputString)
        {
            return WebUtility.HtmlEncode(inputString);
        }

        public string GetDocElementValueByName(string elementName, BsonDocument doc)
        {
            var elementValue = "";

            foreach (var docElement in doc.Elements)
            {
                var currentDocElement = docElement.Name.ToLower();

                if (currentDocElement == elementName.ToLower())
                    elementValue = docElement.Value.ToString();
            }

            return elementValue;
        }
    }
}