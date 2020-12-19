using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Security;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

using cs = MACServices.Constants.Strings;

namespace MongoDB.Web.Providers
{
    public class MongoDbRoleProvider : RoleProvider
    {
        private MongoCollection _rolesMongoCollection;
        private MongoCollection _usersInRolesMongoCollection;

        public MongoDbRoleProvider()
        {
            ApplicationName = ConfigurationManager.AppSettings["MongoDbName"];

            var mongoDBConnectionPool = (MongoDatabase)HttpContext.Current.Application[cs.MongoDB];
            _rolesMongoCollection = mongoDBConnectionPool.GetCollection("Roles");
            _usersInRolesMongoCollection = mongoDBConnectionPool.GetCollection("UsersInRoles");
        }

        public override sealed string ApplicationName { get; set; }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            // Make sure each role exists
            foreach (var roleName in roleNames.Where(roleName => !RoleExists(roleName)))
            {
                throw new ProviderException(String.Format("The role '{0}' was not found.", roleName));
            }

            foreach (var userName in usernames)
            {
                var membershipUser = Membership.GetUser(userName );

                if (membershipUser == null)
                {
                    throw new ProviderException(String.Format("The user '{0}' was not found.", userName ));
                }

                foreach (var roleName in roleNames)
                {
                    if (IsUserInRole(userName , roleName))
                    {
                        throw new ProviderException(String.Format("The user '{0}' is already in role '{1}'.", userName , roleName));
                    }

                    var bsonDocument = new BsonDocument
                    {
                        { "ApplicationName", ApplicationName },
                        { "Role", roleName },
                        { "Username", userName }
                    };

                    _usersInRolesMongoCollection.Insert(bsonDocument);
                }
            }
        }

        public override void CreateRole(string roleName)
        {
            var query = Query.And(Query.EQ("ApplicationName", ApplicationName), Query.EQ("Role", roleName));
            
            if (_rolesMongoCollection.FindAs<BsonDocument>(query).Any())
            {
                throw new ProviderException(String.Format("The role '{0}' already exists.", roleName));
            }

            var bsonDocument = new BsonDocument
            {
                { "ApplicationName", ApplicationName },
                { "Role", roleName }
            };

            _rolesMongoCollection.Insert(bsonDocument);
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            if (!RoleExists(roleName))
            {
                throw new ProviderException(String.Format("The role '{0}' was not found.", roleName));
            }
            
            var query = Query.And(Query.EQ("ApplicationName", ApplicationName), Query.EQ("Role", roleName));

            if (throwOnPopulatedRole && _usersInRolesMongoCollection.FindAs<BsonDocument>(query).Any())
            {
                throw new ProviderException("This role cannot be deleted because there are users present in it.");
            }

            _usersInRolesMongoCollection.Remove(query);
            _rolesMongoCollection.Remove(query);

            return true;
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            if (!RoleExists(roleName))
            {
                throw new ProviderException(String.Format("The role '{0}' was not found.", roleName));
            }

            var query = Query.And(Query.EQ("ApplicationName", ApplicationName), Query.EQ("Role", roleName));
            return _usersInRolesMongoCollection.FindAs<BsonDocument>(query).ToList().Select(bsonDocument => bsonDocument["Username"].AsString).ToArray();
        }

        public override string[] GetAllRoles()
        {
            var query = Query.EQ("ApplicationName", ApplicationName);
            return _rolesMongoCollection.FindAs<BsonDocument>(query).ToList().Select(bsonDocument => bsonDocument["Role"].AsString).ToArray();
        }

        public override string[] GetRolesForUser(string userName )
        {
            //var query = Query.And(Query.EQ("ApplicationName", ApplicationName), Query.EQ("Username", userName ));
            var query = Query.EQ("Username", userName );
            return _usersInRolesMongoCollection.FindAs<BsonDocument>(query).ToList().Select(bsonDocument => bsonDocument["Role"].AsString).ToArray();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            var query = Query.And(Query.EQ("ApplicationName", ApplicationName), Query.EQ("Role", roleName));
            return _usersInRolesMongoCollection.FindAs<BsonDocument>(query).ToList().Select(bsonDocument => bsonDocument["Username"].AsString).ToArray();
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            ApplicationName = config["applicationName"] ?? HostingEnvironment.ApplicationVirtualPath;
            
            var mongoDBConnectionPool = (MongoDatabase)HttpContext.Current.Application[cs.MongoDB];
            _rolesMongoCollection = mongoDBConnectionPool.GetCollection(config["collection"] ?? "Roles");
            _usersInRolesMongoCollection = mongoDBConnectionPool.GetCollection("UsersInRoles");

            _rolesMongoCollection.EnsureIndex("ApplicationName");
            _rolesMongoCollection.EnsureIndex("ApplicationName", "Role");
            _usersInRolesMongoCollection.EnsureIndex("ApplicationName", "Role");
            _usersInRolesMongoCollection.EnsureIndex("ApplicationName", "Username");
            _usersInRolesMongoCollection.EnsureIndex("ApplicationName", "Role", "Username");

            base.Initialize(name, config);
        }

        public override bool IsUserInRole(string userName , string roleName)
        {
            var query = Query.And(Query.EQ("ApplicationName", ApplicationName), Query.EQ("Role", roleName), Query.EQ("Username", userName ));
            return _usersInRolesMongoCollection.FindAs<BsonDocument>(query).Any();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            foreach (var userName in usernames)
            {
                foreach (var roleName in roleNames)
                {
                    var query = Query.And(Query.EQ("ApplicationName", ApplicationName), Query.EQ("Role", roleName), Query.EQ("Username", userName ));
                    _usersInRolesMongoCollection.Remove(query);
                }
            }
        }

        public override bool RoleExists(string roleName)
        {
            //var query = Query.And(Query.EQ("ApplicationName", ApplicationName), Query.EQ("Role", roleName));
            var query = Query.EQ("Role", roleName);
            return _rolesMongoCollection.FindAs<BsonDocument>(query).Any();
        }
    }
}
