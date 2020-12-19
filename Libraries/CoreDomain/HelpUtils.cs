using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.UI.WebControls;
using System.Web;

using MACServices;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using cfgcs = MACServices.Constants.WebConfig.ConnectionStringKeys;
using cs = MACServices.Constants.Strings;

namespace MACServices
{
    public class HelpUtils
    {
        public MongoClient mongoClient;
        public MongoServer mongoServer;
        public MongoDatabase mongoDBConnectionPool;
        public MongoCollection mongoCollection;

        public HelpUtils()
        {
            // Setup the db connection if needed
            if (HttpContext.Current.Application[cs.MongoDBDocServer] == null)
                HttpContext.Current.Application[cs.MongoDBDocServer] = CreateApplicationDBConnectionPool();

            mongoDBConnectionPool = (MongoDatabase)HttpContext.Current.Application[cs.MongoDBDocServer];        
        }

        public MongoDatabase CreateApplicationDBConnectionPool()
        {
            // Parse dbserver credentials
            string[] dbCredentials;
            // Parse dbserver settings
            string[] dbServerPort;
            var dbConnectionString = ConfigurationManager.ConnectionStrings[cfgcs.DocumentationServer].ConnectionString;

            var connectionMode = ConfigurationManager.AppSettings[cs.ConnectionMode];
            var readPreference = ConfigurationManager.AppSettings[cs.ReadPreference];
            var writeConcern = ConfigurationManager.AppSettings[cs.WriteConcern];

            var mongoClientSettings = new MongoClientSettings();

            var dbConnectionTimeout = new TimeSpan(0, 0, 0, Convert.ToInt16(ConfigurationManager.AppSettings[cs.ConnectionTimeoutSeconds]), 0);
            mongoClientSettings.ConnectTimeout = dbConnectionTimeout;

            mongoClientSettings.MinConnectionPoolSize = Convert.ToInt16(ConfigurationManager.AppSettings[cs.MinDBConnections]);
            mongoClientSettings.MaxConnectionPoolSize = Convert.ToInt16(ConfigurationManager.AppSettings[cs.MaxDBConnections]);

            if (dbConnectionString.Contains(cs.ReplicaSetFlag))
            {
                // MongoS service connecting to a ReplicaSet running on AWS cluster
                const string dbServer = "localhost";
                const string dbPort = "27019";

                mongoClientSettings.Server = new MongoServerAddress(dbServer, Convert.ToInt16(dbPort));
            }
            else
            {
                var dbConnectionSettings = dbConnectionString.Split('@');

                // Server credentials
                dbCredentials = dbConnectionSettings[0].Split(':');
                var dbUserName = dbCredentials[1].Replace("//", "");
                var dbPassword = dbCredentials[2];

                // Server settings
                dbServerPort = dbConnectionSettings[1].Split(':');
                var dbServer = dbServerPort[0];
                var dbPort = dbServerPort[1];

                mongoClientSettings.Credentials = new[] { MongoCredential.CreateMongoCRCredential(ConfigurationManager.AppSettings[Constants.WebConfig.AppSettingsKeys.MongoDbDocumentDBName], dbUserName, dbPassword) };
                mongoClientSettings.Server = new MongoServerAddress(dbServer, Convert.ToInt16(dbPort));
            }

            switch (connectionMode)
            {
                case cs.Automatic:
                    mongoClientSettings.ConnectionMode = ConnectionMode.Automatic;
                    break;
                case cs.Direct:
                    mongoClientSettings.ConnectionMode = ConnectionMode.Direct;
                    break;
                case cs.ReplicaSet:
                    mongoClientSettings.ConnectionMode = ConnectionMode.ReplicaSet;
                    break;
                case cs.ShardRouter:
                    mongoClientSettings.ConnectionMode = ConnectionMode.ShardRouter;
                    break;
            }

            switch (readPreference)
            {
                case cs.Nearest:
                    mongoClientSettings.ReadPreference = ReadPreference.Nearest;
                    break;
                case cs.Primary:
                    mongoClientSettings.ReadPreference = ReadPreference.Primary;
                    break;
                case cs.PrimaryPreferred:
                    mongoClientSettings.ReadPreference = ReadPreference.PrimaryPreferred;
                    break;
                case cs.Secondary:
                    mongoClientSettings.ReadPreference = ReadPreference.Secondary;
                    break;
                case cs.SecondaryPreferred:
                    mongoClientSettings.ReadPreference = ReadPreference.SecondaryPreferred;
                    break;
            }

            switch (writeConcern)
            {
                case cs.Acknowledged:
                    mongoClientSettings.WriteConcern = WriteConcern.Acknowledged;
                    break;
                case cs.Unacknowledged:
                    mongoClientSettings.WriteConcern = WriteConcern.Unacknowledged;
                    break;
            }

            mongoClient = new MongoClient(mongoClientSettings);
            mongoServer = mongoClient.GetServer();
            mongoDBConnectionPool = mongoServer.GetDatabase(ConfigurationManager.AppSettings[Constants.WebConfig.AppSettingsKeys.MongoDbDocumentDBName]);

            return mongoDBConnectionPool;
        }

        public string GetToolTips(string topicId)
        {
            var toolTipText = "Tooltip not available";

            try
            {
                MongoDatabase mongoDBConnectionPool = (MongoDatabase)HttpContext.Current.Application[cs.MongoDBDocServer];

                var topicQuery = Query.EQ("_id", ObjectId.Parse(topicId));
                MongoCollection helpCollection = mongoDBConnectionPool.GetCollection("Help");
                var topicDetails = helpCollection.FindOneAs<HelpTopic>(topicQuery);
                toolTipText = topicDetails.Description;
            }
            catch (Exception ex)
            {
                var errMsg = ex.ToString();
            }

            return toolTipText;
        }
    }
}
