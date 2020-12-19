using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Profile;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

using cs = MACServices.Constants.Strings;

namespace MongoDB.Web.Providers
{
    public class MongoDbProfileProvider : ProfileProvider
    {
        private MongoCollection _mongoCollection;

        public MongoDbProfileProvider()
        {
            ApplicationName = ConfigurationManager.AppSettings["MongoDbName"];

            var mongoDBConnectionPool = (MongoDatabase)HttpContext.Current.Application[cs.MongoDB];
            _mongoCollection = mongoDBConnectionPool.GetCollection("Profiles");
        }

        public override sealed string ApplicationName { get; set; }

        public override int DeleteInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            var query = Query.And(Query.EQ("ApplicationName", ApplicationName), Query.LTE("LastActivityDate", userInactiveSinceDate));

            if (authenticationOption != ProfileAuthenticationOption.All)
            {
                query = Query.And(query, Query.EQ("IsAnonymous", authenticationOption == ProfileAuthenticationOption.Anonymous));
            }

            return (int)_mongoCollection.Remove(query).DocumentsAffected;
        }

        public override int DeleteProfiles(string[] usernames)
        {
            var query = Query.And(Query.EQ("ApplicationName", ApplicationName), Query.In("Username", new BsonArray(usernames)));
            return (int)_mongoCollection.Remove(query).DocumentsAffected;
        }

        public override int DeleteProfiles(ProfileInfoCollection profiles)
        {
            return DeleteProfiles(profiles.Cast<ProfileInfo>().Select(profile => profile.UserName).ToArray());
        }

        public override ProfileInfoCollection FindInactiveProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
        {
            return GetProfiles(authenticationOption, usernameToMatch, userInactiveSinceDate, pageIndex, pageSize, out totalRecords);
        }

        public override ProfileInfoCollection FindProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            return GetProfiles(authenticationOption, usernameToMatch, null, pageIndex, pageSize, out totalRecords);
        }

        public override ProfileInfoCollection GetAllInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
        {
            return GetProfiles(authenticationOption, null, userInactiveSinceDate, pageIndex, pageSize, out totalRecords);
        }

        public override ProfileInfoCollection GetAllProfiles(ProfileAuthenticationOption authenticationOption, int pageIndex, int pageSize, out int totalRecords)
        {
            return GetProfiles(authenticationOption, null, null, pageIndex, pageSize, out totalRecords);
        }

        public override int GetNumberOfInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            var query = GetQuery(authenticationOption, null, userInactiveSinceDate);
            return (int)_mongoCollection.Count(query);
        }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection)
        {
            var settingsPropertyValueCollection = new SettingsPropertyValueCollection();
            
            if (collection.Count < 1)
            {
                return settingsPropertyValueCollection;
            }

            var userName = (string)context["UserName"];

            if(String.IsNullOrWhiteSpace(userName ))
            {
                return settingsPropertyValueCollection;
            }

            var query = Query.And(Query.EQ("ApplicationName", ApplicationName), Query.EQ("Username", userName ));
            //var bsonDocument = _mongoCollection.FindOneAs<BsonDocument>(query);

            foreach (var settingsPropertyValue in from SettingsProperty settingsProperty in collection select new SettingsPropertyValue(settingsProperty))
            {
                settingsPropertyValueCollection.Add(settingsPropertyValue);
            }

            var update = Update.Set("LastActivityDate", DateTime.UtcNow);
            _mongoCollection.Update(query, update);

            return settingsPropertyValueCollection;
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            ApplicationName = ConfigurationManager.AppSettings["MongoDbName"];

            var mongoDBConnectionPool = (MongoDatabase)HttpContext.Current.Application[cs.MongoDB];
            _mongoCollection = mongoDBConnectionPool.GetCollection("Profiles");

            _mongoCollection.EnsureIndex("ApplicationName");
            _mongoCollection.EnsureIndex("ApplicationName", "IsAnonymous");
            _mongoCollection.EnsureIndex("ApplicationName", "IsAnonymous", "LastActivityDate");
            _mongoCollection.EnsureIndex("ApplicationName", "IsAnonymous", "LastActivityDate", "Username");
            _mongoCollection.EnsureIndex("ApplicationName", "IsAnonymous", "Username");
            _mongoCollection.EnsureIndex("ApplicationName", "LastActivityDate");
            _mongoCollection.EnsureIndex("ApplicationName", "Username");
            _mongoCollection.EnsureIndex("ApplicationName", "Username", "IsAnonymous");

            base.Initialize(name, config);
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
        {
            var userName = (string)context["UserName"];
            var isAuthenticated = (bool)context["IsAuthenticated"];

            if (String.IsNullOrWhiteSpace(userName ) || collection.Count < 1)
            {
                return;
            }

            var values = (from SettingsPropertyValue settingsPropertyValue in collection where settingsPropertyValue.IsDirty where isAuthenticated || (bool) settingsPropertyValue.Property.Attributes["AllowAnonymous"] select settingsPropertyValue).ToDictionary(settingsPropertyValue => settingsPropertyValue.Name, settingsPropertyValue => settingsPropertyValue.PropertyValue);

            var query = Query.And(Query.EQ("ApplicationName", ApplicationName), Query.EQ("Username", userName ));
            var bsonDocument = _mongoCollection.FindOneAs<BsonDocument>(query) ?? new BsonDocument
            {
                { "ApplicationName", ApplicationName },
                { "Username", userName }
            };

            var mergeDocument = new BsonDocument
            {
                { "LastActivityDate", DateTime.UtcNow },
                { "LastUpdatedDate", DateTime.UtcNow }
            };

            mergeDocument.AddRange(values as IDictionary<string, object>);
            bsonDocument.Merge(mergeDocument);

            _mongoCollection.Save(bsonDocument);
        }

        #region Private Methods

        private ProfileInfoCollection GetProfiles(ProfileAuthenticationOption authenticationOption, string usernameToMatch, DateTime? userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
        {
            var query = GetQuery(authenticationOption, usernameToMatch, userInactiveSinceDate);

            //var query = Query.EQ("Username", usernameToMatch);

            totalRecords = (int)_mongoCollection.Count(query);

            var profileInfoCollection = new ProfileInfoCollection();

            foreach (var bsonDocument in _mongoCollection.FindAs<BsonDocument>(query).SetSkip(pageIndex * pageSize).SetLimit(pageSize))
            {
                profileInfoCollection.Add(ToProfileInfo(bsonDocument));
            }

            return profileInfoCollection;
        }

        private IMongoQuery GetQuery(ProfileAuthenticationOption authenticationOption, string usernameToMatch, DateTime? userInactiveSinceDate)
        {
            var query = Query.EQ("ApplicationName", ApplicationName);
            
            if (authenticationOption != ProfileAuthenticationOption.All)
            {
                query = Query.And(query, Query.EQ("IsAnonymous", authenticationOption == ProfileAuthenticationOption.Anonymous));
            }

            if(!String.IsNullOrWhiteSpace(usernameToMatch))
            {
                query = Query.And(query, Query.Matches("Username", usernameToMatch));
            }

            if(userInactiveSinceDate.HasValue)
            {
                query = Query.And(query, Query.LTE("LastActivityDate", userInactiveSinceDate));
            }

            return query;
        }

        private static ProfileInfo ToProfileInfo(BsonDocument bsonDocument)
        {
            //return new ProfileInfo(bsonDocument["Username"].AsString, bsonDocument["IsAnonymous"].AsBoolean, bsonDocument["LastActivityDate"].ToUniversalTime(), bsonDocument["LastUpdatedDate"].ToUniversalTime(), 0);
            return new ProfileInfo(bsonDocument["Username"].AsString, bsonDocument["IsAnonymous"].AsBoolean, bsonDocument["LastActivityDate"].ToUniversalTime(), bsonDocument["LastUpdatedDate"].ToUniversalTime(), 0);
        }

        #endregion
    }
}