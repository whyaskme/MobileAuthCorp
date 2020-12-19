using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Xml;
using System.ServiceModel;

using Microsoft.Web.Administration;

using KellermanSoftware.CompareNetObjects;
using MACServices.SecureAd;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Newtonsoft.Json;

using MACSecurity;
using cs = MACServices.Constants.Strings;
using dtr = MACServices.Constants.DocumentTemplateReplacementTokens;
using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using regX = MACServices.Constants.RegexStrings;
using tokenKeys = MACServices.Constants.TokenKeys;
using cfgcs = MACServices.Constants.WebConfig.ConnectionStringKeys;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using es = MACServices.Constants.EventStats;
using sr = MACServices.Constants.ServiceResponse;
using delmeth = MACServices.Constants;

using cfghost = MACServices.Constants.WebConfig.HostInfo;

namespace MACServices
{
    public class Utils
    {
        #region System Management Methods

        public void ResetIPAddresses()
        {

        }

        public void ResetProviders(string providerType)
        {

        }

        #endregion

        public bool EmailIsValid(string emailaddress)
        {
            return Regex.IsMatch(emailaddress, @"\A[a-z0-9]+([-._][a-z0-9]+)*@([a-z0-9]+(-[a-z0-9]+)*\.)+[a-z]{2,4}\z")
                && Regex.IsMatch(emailaddress, @"^(?=.{1,64}@.{4,64}$)(?=.{6,100}$).*");
        }

        #region Web Config Updates

            public bool SetWebConfig(string targetEnvironment)
            {
                var bIsUpdated = false;

                var currentHost = HttpContext.Current.Request.ServerVariables["SERVER_NAME"].ToString();

                // Switch the config to thwe passed in value
                if (!String.IsNullOrEmpty(targetEnvironment))
                    currentHost = targetEnvironment;

                var currentWebConfigPath = new StringBuilder(HttpContext.Current.Server.MapPath("~"));
                currentWebConfigPath.Append(@"web.config");

                var sourceConfigPath = new StringBuilder(HttpContext.Current.Server.MapPath("~"));
                sourceConfigPath.Append(@"WebConfigs");

                var mComputer = Environment.MachineName.ToUpper();

                try
                {
                    switch (currentHost)
                    {
                        case cfghost.Host.Localhost:
                            switch (mComputer.ToLower())
                            {
                                case "chrismiller":
                                    sourceConfigPath.Append(@"\web-localhost-Chris.config");
                                    break;
                                case "terryshotbox":
                                    sourceConfigPath.Append(@"\web-localhost-Terry.config");
                                    break;
                                case "lenovo-pc":
                                    sourceConfigPath.Append(@"\web-localhost-Joe.config");
                                    break;
                            }
                            break;
                        case cfghost.Host.Corp:
                            sourceConfigPath.Append(@"\web-corp.config");
                            break;
                        case cfghost.Host.Demo:
                            sourceConfigPath.Append(@"\web-demo.config");
                            break;
                        case cfghost.Host.QA:
                            sourceConfigPath.Append(@"\web-qa.config");
                            break;
                        case cfghost.Host.Test:
                            sourceConfigPath.Append(@"\web-test.config");
                            break;
                        case cfghost.Host.ProductionStaging:
                            sourceConfigPath.Append(@"\web-production-staging.config");
                            break;
                        case cfghost.Host.Production:
                            sourceConfigPath.Append(@"\web-production.config");
                            break;
                    }

                    // Compare configs. If not matched, then overwrite
                    var f1Lines = File.ReadAllLines(currentWebConfigPath.ToString());
                    var f2Lines = File.ReadAllLines(sourceConfigPath.ToString());

                    var configDifferences = f1Lines
                         .Where((line, index) => index >= f2Lines.Length || line != f2Lines[index])
                         .Select((line, index) => line).ToList();

                    if (configDifferences.Count > 0)
                    {
                        File.Copy(sourceConfigPath.ToString(), currentWebConfigPath.ToString(), true); // overwrite = true

                        bIsUpdated = true;

                        // Ensure indexes
                        string[] collectionsToIndex = { "Accounts", "Billing", "Client", "EndUser", "Event", "EventStat", "Group", "OasClientList", "Otp", "UserProfile" };
                        CreateDatabaseIndexes(collectionsToIndex);

                        // Recycle the AppPool to force recompile
                        RecycleAppPools();
                    }
                    return bIsUpdated;
                }
                catch
                {
                    return bIsUpdated;
                }
            }

            public static void RecycleAppPools()
            {
                try
                {
                    var serverManager = new ServerManager();
                    var appPools = serverManager.ApplicationPools;
                    foreach (var ap in appPools)
                    {
                        ap.Recycle();
                    }
                }
                catch(Exception ex)
                {
                    var errMsg = ex.ToString();
                }
            } 

            public string SetEnvironmentLabel()
            {
                var webConfiguration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                var currentEnvironment = webConfiguration.AppSettings.Settings["ConfigName"].Value;

                return currentEnvironment;
            }

        #endregion

        #region Mongodb Connection Pool Methods

            public MongoClient mongoClient;
            public MongoServer mongoServer;
            public MongoDatabase mongoDBConnectionPool;

            public Utils()
            {
                if (HttpContext.Current != null)
                {
                    if (HttpContext.Current.Application[cs.MongoDB] == null)
                        HttpContext.Current.Application[cs.MongoDB] = CreateApplicationDBConnectionPool();
                    mongoDBConnectionPool = (MongoDatabase)HttpContext.Current.Application[cs.MongoDB];
                }
                else
                {   // No HttpContext, threads have no HttpContext
                    mongoDBConnectionPool = CreateApplicationDBConnectionPool();
                }
            }

            public MongoDatabase CreateApplicationDBConnectionPool()
            {
                // Parse dbserver credentials
                string[] dbCredentials;
                // Parse dbserver settings
                string[] dbServerPort;
                var dbConnectionString = ConfigurationManager.ConnectionStrings[cfgcs.MongoServer].ConnectionString;

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

                    //readPreference = cs.SecondaryPreferred;

                    //mongoClientSettings.ReplicaSetName = ConfigurationManager.AppSettings[cs.ReplicaSetName];

                    //List<MongoServerAddress> ReplicaSet = new List<MongoServerAddress>();
                    //MongoServerAddress mongoDb1 = new MongoServerAddress(cs.MongoDbSvr1, Convert.ToInt16(27017));
                    //MongoServerAddress mongoDb2 = new MongoServerAddress(cs.MongoDbSvr2, Convert.ToInt16(27027));
                    //MongoServerAddress mongoDb3 = new MongoServerAddress(cs.MongoDbSvr3, Convert.ToInt16(27037));

                    //ReplicaSet.Add(mongoDb1);
                    //ReplicaSet.Add(mongoDb2);
                    //ReplicaSet.Add(mongoDb3);

                    //mongoClientSettings.Servers = ReplicaSet;
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

                    //var credentials = MongoCredential.CreateScramSha1Credential("admin", "skMongo", ("my password"));

                    mongoClientSettings.Credentials = new[] { MongoCredential.CreateMongoCRCredential(ConfigurationManager.AppSettings[Constants.WebConfig.AppSettingsKeys.MongoDbName], dbUserName, dbPassword) };
                    mongoClientSettings.Server = new MongoServerAddress(dbServer, Convert.ToInt32(dbPort));
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
                mongoDBConnectionPool = mongoServer.GetDatabase(ConfigurationManager.AppSettings[Constants.WebConfig.AppSettingsKeys.MongoDbName]);

                return mongoDBConnectionPool;
            }

        #endregion

        #region Mongodb Methods

            public List<string> GetUserRoles()
            {
                var userRoles = new List<string>();
                try
                {
                    MongoCollection mongoCollection = mongoDBConnectionPool.GetCollection("Roles");

                    var query = Query.EQ("ApplicationName", "mac_r1");
                    var sortBy = SortBy.Ascending("SortOrder");
                    var rolesList = mongoCollection.FindAs<UserRole>(query).SetSortOrder(sortBy);

                    foreach (UserRole currentRole in rolesList)
                    {
                        userRoles.Add(currentRole.SortOrder + "|" + currentRole.Role + "|" + currentRole._id);
                    }
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch (Exception ex)
                {
                    // ReSharper disable once UnusedVariable
                    var errMsg = ex.ToString();
                }
                return userRoles;
            }

            public string GetMACClientIdBySTSiteId(string stOperatorId, string stSiteId)
            {
                var responseClientId = "";
                var queryValues = stOperatorId + "|" + stSiteId;

                try
                {
                    MongoCollection mongoCollection = mongoDBConnectionPool.GetCollection("Client");
                    var query = Query.EQ("SecureTradingSiteId", queryValues);
                    //var query = Query.And(Query.EQ("_t", objectType), Query.EQ("Name", name));

                    var myClient = mongoCollection.FindOneAs<Client>(query);
                    if (myClient != null)
                        responseClientId = myClient._id + "|" + myClient.Name;
                }
                catch(Exception ex)
                {
                    responseClientId = ex.ToString();
                }
                return responseClientId;
            }

            public List<Client> GetMACClientWithSTSiteIds()
            {
                try
                {
                    MongoCollection mongoCollection = mongoDBConnectionPool.GetCollection("Client");
                    var query = Query.And(Query.NE("SecureTradingSiteId", ""), Query.Exists("SecureTradingSiteId"));
                    var mClientList = mongoCollection.FindAs<Client>(query).ToList();
                    return mClientList;
                }
                catch (Exception ex)
                {
                    var opps = ex.ToString();
                }
                return null;
            }

            public List<string> GetOwnerUsers(string ownerId, string ownerType, string userType)
            {
                var ownerUsers = new List<string>();
                try
                {
                    if (ownerType == "Group")
                    {
                        var myGroup = new Group(ownerId);
                        foreach (Relationship currRelationship in myGroup.Relationships)
                        {
                            if (currRelationship.MemberType == userType && currRelationship.MemberHierarchy == "Member")
                            {
                                // Decrypt user profile info
                                var userId = currRelationship.MemberId;
                                var userProfile = new UserProfile(userId.ToString());
                                var userFullName = Security.DecodeAndDecrypt(userProfile.FirstName, userProfile.UserId.ToString()) + " " + Security.DecodeAndDecrypt(userProfile.LastName, userProfile.UserId.ToString());

                                ownerUsers.Add(userFullName + "|" + userId);
                            }
                        }
                    }
                    else
                    {
                        var myClient = new Client(ownerId);
                        foreach (Relationship currRelationship in myClient.Relationships)
                        {
                            if (currRelationship.MemberType == userType && currRelationship.MemberHierarchy == "Member")
                            {
                                // Decrypt user profile info
                                var userId = currRelationship.MemberId;
                                var userProfile = new UserProfile(userId.ToString());
                                var userFullName = Security.DecodeAndDecrypt(userProfile.FirstName, userProfile.UserId.ToString()) + " " + Security.DecodeAndDecrypt(userProfile.LastName, userProfile.UserId.ToString());

                                ownerUsers.Add(userFullName + "|" + userId);
                            }
                        }
                    }
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch(Exception ex)
                {
    // ReSharper disable once UnusedVariable
                    var errMsg = ex.ToString();
                }
                ownerUsers.Sort();

                return ownerUsers;
            }

            public string GetLoggedInUserPropertiesFromCookies(string propertyName)
            {
                string propertyValue = "Property not found";

                // Read the user values from the authentication cookie and validate
                var authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authCookie == null)
                {
                    return propertyValue;
                }
                var ticket = FormsAuthentication.Decrypt(authCookie.Value);

                if (ticket == null) return propertyValue;
                var userData = ticket.UserData.Split('|');
                foreach (var userField in userData)
                {
                    var userFieldData = userField.Split('=');
                    var fieldName = userFieldData[0].Replace("hidden", "");
                    var fieldValue = userFieldData[1];

                    if (fieldName.ToLower() == propertyName.ToLower())
                        return fieldValue;
                }
                return propertyValue;
            }

            public List<string> GetClientsForUser(string userId)
            {
                List<string> userClients = new List<string>();

                try
                {
                    MongoCollection mongoCollection = mongoDBConnectionPool.GetCollection("Client");

                    var query = Query.EQ("Relationships.MemberId", ObjectId.Parse(userId));
                    var clientList = mongoCollection.FindAs<Client>(query);

                    foreach (Client userClient in clientList)
                    {
                        userClients.Add(userClient.Name + "|" + userClient._id);
                    }
                }
    // ReSharper disable once EmptyGeneralCatchClause
                catch
                {

                }
                userClients.Sort();

                return userClients;
            }

            public string GetUsersAndClientIds()
            {
                try
                {
                    var init = Convert.ToBoolean(ConfigurationManager.AppSettings[cfg.InitUserClientIdList]);
                    if (init == false)
                    {
                        return "None" + dk.ItemSep + "None";
                    }
                }
                catch
                {
                    return "None" + dk.ItemSep + "None";
                }

                var mUsersAnsClientIds = "";

                if (Convert.ToBoolean(ConfigurationManager.AppSettings[cfg.Debug]) == false)
                    return "None" + dk.ItemSep + "None";
                try
                {
                    MongoCollection mongoCollection = mongoDBConnectionPool.GetCollection("EndUser");
                    // get list of end users
                    var query = Query.EQ("_t", "EndUser");
                    var endUserList = mongoCollection.FindAs<EndUser>(query).SetLimit(Constants.Application.Startup.UserClientCount);

                    foreach (var eu in endUserList)
                    {
                        foreach (var re in eu.Relationships)
                        {
                            if (re.MemberType == "Client")
                            {
                                var mLastName = Security.DecodeAndDecrypt(eu.LastName, eu.HashedUserId);
                                var mEmail = Security.DecodeAndDecrypt(eu.Email, eu.HashedUserId);
                                var mPhone = Security.DecodeAndDecrypt(eu.Phone, eu.HashedUserId);
                                var mType = eu.RegistrationType;
                                mUsersAnsClientIds += dk.ItemSep + dkui.LastName + "=" + mLastName +
                                                      dk.KVSep + dkui.EmailAddress + "=" + mEmail +
                                                      dk.KVSep + dkui.PhoneNumber + "=" + mPhone +
                                                      dk.KVSep + dk.CID + "=" + re.MemberId +
                                                      dk.KVSep + dk.RegistrationType + "=" + mType;
                                break;
                            }
                        }
                    }
                }
                catch
                {
                    return "None" + dk.ItemSep + "None";
                }
                return mUsersAnsClientIds.Trim(char.Parse(dk.ItemSep));
            }

            public string GetStateById(string stateId)
            {
                var stateName = "";
                try
                {
                    var query = Query.EQ("_id", ObjectId.Parse(stateId));

                    var mongoCollection = mongoDBConnectionPool.GetCollection("TypeDefinitions").Find(query).SetFields(Fields.Include("Name"));
                    foreach (var doc in mongoCollection)
                    {
                        stateName = GetDocElementValueByName("Name", doc);
                    }
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                {

                }
                return stateName;
            }

            public string GetUserIdByUserName(string emailAddress)
            {
                var userId = "";
                try
                {
                    var query = Query.EQ("EmailLower", emailAddress.ToLower());

                    var mongoCollection = mongoDBConnectionPool.GetCollection("Users").Find(query).SetFields(Fields.Include("_id"));
                    foreach (var doc in mongoCollection)
                    {
                        userId = GetDocElementValueByName("_id", doc);
                    }
                }
    // ReSharper disable once EmptyGeneralCatchClause
                catch
                {

                }
                return userId;
            }

            public string GetUserNameByUserId(string userId)
            {
                var userName = "";
                try
                {
                    var query = Query.EQ("_id", ObjectId.Parse(userId));

                    var mongoCollection = mongoDBConnectionPool.GetCollection("Users").Find(query).SetFields(Fields.Include("Username"));
                    foreach (var doc in mongoCollection)
                    {
                        userName = GetDocElementValueByName("Username", doc);
                    }
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                {

                }
                return userName;
            }

            public string GetUsersAndClientIds2()
            {
                try
                {
                    var init = Convert.ToBoolean(ConfigurationManager.AppSettings[cfg.InitUserClientIdList]);
                    if (init == false) return "None" + dk.ItemSep + "None";
                }
                catch
                {
                    return "None" + dk.ItemSep + "None";
                }

                var mUsersAnsClientIds = "";

                if (Convert.ToBoolean(ConfigurationManager.AppSettings[cfg.Debug]) == false)
                    return "None" + dk.ItemSep + "None";
                try
                {
                    var rdm = new Random();

                    var query = Query.EQ("_t", "EndUser");

                    var mRegisteredUserCount = unchecked((int)mongoDBConnectionPool.GetCollection("EndUser").Count());

                    for (var x = 0; x < Constants.Application.Startup.UserClientCount; ++x)
                    {
                        var startRecordNumber = rdm.Next(1, mRegisteredUserCount - 1);
                        var endUserList = mongoDBConnectionPool.GetCollection("EndUser").FindAs<EndUser>(query).SetSkip(startRecordNumber).SetLimit(1);
                        foreach (var eu in endUserList)
                        {
                            foreach (var re in eu.Relationships)
                            {
                                if (re.MemberType == "Client")
                                {
                                    var mLastName = Security.DecodeAndDecrypt(eu.LastName, eu.HashedUserId);
                                    var mEmail = Security.DecodeAndDecrypt(eu.Email, eu.HashedUserId);
                                    var mPhone = Security.DecodeAndDecrypt(eu.Phone, eu.HashedUserId);
                                    var mType = eu.RegistrationType;
                                    mUsersAnsClientIds += dk.ItemSep + dkui.LastName + "=" + mLastName +
                                                          dk.KVSep + dkui.EmailAddress + "=" + mEmail +
                                                          dk.KVSep + dkui.PhoneNumber + "=" + mPhone +
                                                          dk.KVSep + dk.CID + "=" + re.MemberId +
                                                          dk.KVSep + dk.RegistrationType + "=" + mType;
                                    break;
                                }
                            } // end for each relationship
                        }
                    } // end for loop
                }
                catch
                {
                    return "None" + dk.ItemSep + "None";
                }
                return mUsersAnsClientIds.Trim(char.Parse(dk.ItemSep));
            }

            public string ObjectCreate(Object newObject)
            {
                var collectionName = newObject.GetType().ToString().Replace("MACServices.", "");

                const string createMessage = "create succeeded";

                try
                {
                    var dbCollection = mongoDBConnectionPool.GetCollection(collectionName);
                    dbCollection.Insert(newObject);
                }
                catch (Exception ex)
                {
                    var mDetails = "ObjectCreate(" + collectionName + ") " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                    var exceptionEvent = new Event
                    {
                        EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                    };
                    exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
                }
                return createMessage;
            }

            public string GetRoleNameByRoleId(string roleId)
            {
                string roleName;

                if (String.IsNullOrEmpty(roleId)) return "Role name required";

                UserRole myRole;
                try
                {
                    var query = Query.EQ("_id", ObjectId.Parse(roleId.Trim()));
                    var mongoCollection = mongoDBConnectionPool.GetCollection("Roles");
                    myRole = mongoCollection.FindOneAs<UserRole>(query);

                    roleName = myRole.Role;
                }
                catch
                {
                    return "Role name not found (Exception)";
                }
                return roleName;
            }

            public Otp GetOtpUsingRequestId(string pRequestId)
            {
                if (String.IsNullOrEmpty(pRequestId)) return null;

                Otp myOtp;

                try
                {
                    var query = Query.EQ("_id", ObjectId.Parse(pRequestId.Trim()));
                    var mongoCollection = mongoDBConnectionPool.GetCollection("Otp");
                    myOtp = mongoCollection.FindOneAs<Otp>(query);
                }
                catch
                {
                    myOtp = null;
                }
                return myOtp;
            }

            public Relationship GetRelationshipUsingName(string groupName)
            {
                if (String.IsNullOrEmpty(groupName)) return null;

                Relationship myMember;

                try
                {
                    var query = Query.EQ("Name", groupName);
                    var mongoCollection = mongoDBConnectionPool.GetCollection("Group");
                    myMember = mongoCollection.FindOneAs<Relationship>(query);
                }
                catch
                {
                    myMember = null;
                }
                return myMember;
            }

            public Client GetClientUsingClientName(string pClientName)
            {
                if (String.IsNullOrEmpty(pClientName)) return null;

                Client myClient;
                try
                {
                    var query = Query.EQ("Name", pClientName);
                    var mongoCollection = mongoDBConnectionPool.GetCollection("Client");
                    myClient = mongoCollection.FindOneAs<Client>(query);
                }
                catch (Exception ex)
                {
                    var mDetails = "GetClientUsingClientName(" + pClientName + ") " + 
                        ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                    var exceptionEvent = new Event
                    {
                        EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                    };
                    exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
                    myClient = null;
                }
                return myClient;
            }

            public string GetClientNameUsingId(string pCID)
            {
                try
                {
                    var query = Query.EQ("_id", ObjectId.Parse(pCID.Trim()));
                    var mongoCollection = mongoDBConnectionPool.GetCollection("Client").Find(query).SetFields(Fields.Include("Name"));
                    foreach (var doc in mongoCollection)
                    {
                        return GetDocElementValueByName("Name", doc);
                    }
                }
                catch (Exception ex)
                {
                    var mDetails = "GetClientNameUsingId(" + pCID + ") " + 
                        ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                    var exceptionEvent = new Event
                    {
                        EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                    };
                    exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
                }
                return string.Empty;
            }

            public string GetClientIdUsingName(String pClientName)
            {
                try
                {
                    var query = Query.EQ("Name", pClientName);
                    var mongoCollection =
                        mongoDBConnectionPool.GetCollection("Client").Find(query).SetFields(Fields.Include("ClientId"));
                    foreach (var doc in mongoCollection)
                    {
                        return GetDocElementValueByName("ClientId", doc);
                    }
                }
                catch (Exception ex)
                {
                    var mDetails = "GetClientIdUsingName(" + pClientName + ") " + 
                        ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                    var exceptionEvent = new Event
                    {
                        EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                    };
                    exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
                }
                return string.Empty;
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

            public Client GetClientUsingClientId(string pClientId)
            {
                if (String.IsNullOrEmpty(pClientId)) return null;

                Client myClient;

                try
                {
                    var query = Query.EQ("_id", ObjectId.Parse(pClientId.Trim()));
                    var mongoCollection = mongoDBConnectionPool.GetCollection("Client");
                    myClient = mongoCollection.FindOneAs<Client>(query);
                }
                catch (Exception ex)
                {
                    var mDetails = "GetClientUsingClientId(" + pClientId + ") " + 
                        ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                    var exceptionEvent = new Event
                    {
                        EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                    };
                    exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
                    myClient = null;
                }
                return myClient;
            }

            public Group GetGroupUsingGroupName(string pGroupName)
            {
                if (String.IsNullOrEmpty(pGroupName)) return null;

                Group myGroup;

                try
                {
                    var query = Query.EQ("Name", pGroupName);
                    var mongoCollection = mongoDBConnectionPool.GetCollection("Group");
                    myGroup = mongoCollection.FindOneAs<Group>(query);
                }
                catch
                {
                    myGroup = null;
                }
                return myGroup;
            }

            public Group GetGroupUsingGroupId(string groupId)
            {
                if (String.IsNullOrEmpty(groupId)) return null;

                Group myGroup;

                try
                {
                    var query = Query.EQ("_id", ObjectId.Parse(groupId.Trim()));
                    var mongoCollection = mongoDBConnectionPool.GetCollection("Group");
                    myGroup = mongoCollection.FindOneAs<Group>(query);
                }
                catch
                {
                    myGroup = null;
                }
                return myGroup;
            }

            public Object ObjectRead(string objectId, string objectType, string responseType)
            {
                try
                {
                    MongoCollection mongoCollection;

                    var query = Query.EQ("_id", ObjectId.Parse(objectId.Trim()));

                    switch (objectType.ToLower())
                    {
                        case "client":
                            mongoCollection = mongoDBConnectionPool.GetCollection("Client");
                            var myClient = mongoCollection.FindOneAs<Client>(query);
                            return myClient;

                        case "administrator":
                            mongoCollection = mongoDBConnectionPool.GetCollection("UserProfile");
                            query = Query.EQ("UserId", ObjectId.Parse(objectId.Trim()));
                            var myAdminProfile = mongoCollection.FindOneAs<UserProfile>(query);
                            return myAdminProfile;

                        case "enduser":
                            mongoCollection = mongoDBConnectionPool.GetCollection("EndUser");
                            var myEndUser = mongoCollection.FindOneAs<EndUser>(query);
                            return myEndUser;

                        //case "event":
                        //    mongoCollection = mongoDBConnectionPool.GetCollection("Event");
                        //    var myEvent = mongoCollection.FindOneAs<Event>(query);
                        //    return myEvent;

                        case "group":
                            mongoCollection = mongoDBConnectionPool.GetCollection("Group");
                            var myGroup = mongoCollection.FindOneAs<Group>(query);
                            return myGroup;

                        case "userprofile":
                            mongoCollection = mongoDBConnectionPool.GetCollection("UserProfile");
                            query = Query.EQ("UserId", ObjectId.Parse(objectId.Trim()));
                            var myProfile = mongoCollection.FindOneAs<UserProfile>(query);
                            return myProfile;
                    }
                }
                catch (Exception ex)
                {
                    var mDetails = "ObjectRead(" + objectId + ", " + objectType + ", " + responseType + ") " + 
                        ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                    var exceptionEvent = new Event
                    {
                        EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                    };
                    exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
                    return ex;
                }
                return null;
            }

            public string ObjectUpdate(Object objectToUpdate, string objectId)
            {
                var updateMessage = "update succeeded";

                var mongoCollection = mongoDBConnectionPool.GetCollection("Client");
                //var query = Query.EQ("_id", ObjectId.Parse(objectId.Trim()));
                //var sortBy = SortBy.Descending("_id");
                var objectType = "";
                try
                {
                    objectType = objectToUpdate.GetType().ToString().ToLower().Replace("macservices.", "");

                    switch (objectType)
                    {
                        case "client":
                            mongoCollection = mongoDBConnectionPool.GetCollection("Client");
                            break;

                        case "enduser":
                            mongoCollection = mongoDBConnectionPool.GetCollection("EndUser");
                            break;

                        case "userprofile":
                            mongoCollection = mongoDBConnectionPool.GetCollection("UserProfile");
                            break;

                        case "group":
                            mongoCollection = mongoDBConnectionPool.GetCollection("Group");
                            break;

                        case "otp":
                            mongoCollection = mongoDBConnectionPool.GetCollection("Otp");
                            break;

                        case "oasclientlist":
                            mongoCollection = mongoDBConnectionPool.GetCollection("OasClientList");
                            break;

                        case "country":
                            mongoCollection = mongoDBConnectionPool.GetCollection("TypeDefinitions");
                            break;

                        case "registrationtype":
                            mongoCollection = mongoDBConnectionPool.GetCollection("TypeDefinitions");
                            break;

                        case "state":
                            mongoCollection = mongoDBConnectionPool.GetCollection("TypeDefinitions");
                            break;

                        case "Sms":
                            mongoCollection = mongoDBConnectionPool.GetCollection("TypeDefinitions");
                            break;

                        case "email":
                            mongoCollection = mongoDBConnectionPool.GetCollection("TypeDefinitions");
                            break;

                        case "voice":
                            mongoCollection = mongoDBConnectionPool.GetCollection("TypeDefinitions");
                            break;

                        case "userverificationprovider":
                            mongoCollection = mongoDBConnectionPool.GetCollection("TypeDefinitions");
                            break;

                        case "provideremail":
                            mongoCollection = mongoDBConnectionPool.GetCollection("TypeDefinitions");
                            break;

                        case "providersms":
                            mongoCollection = mongoDBConnectionPool.GetCollection("TypeDefinitions");
                            break;

                        case "providervoice":
                            mongoCollection = mongoDBConnectionPool.GetCollection("TypeDefinitions");
                            break;

                        case "verificationprovider":
                            mongoCollection = mongoDBConnectionPool.GetCollection("TypeDefinitions");
                            break;
                    }

                    mongoCollection.Save(objectToUpdate);

                    //mongoCollection.FindAndRemove(query, sortBy);
                    //mongoCollection.Insert(objectToUpdate);
                }
                catch (Exception ex)
                {
                    var mDetails = "ObjectUpdate(" + objectType + ", " + objectId + ") " + 
                        ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                    var exceptionEvent = new Event
                    {
                        EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                    };
                    exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
                    updateMessage = "update failed exception: " + ex.Message;
                }
                return updateMessage;
            }

            public string DeleteOasClient(String id)
            {
                if (String.IsNullOrEmpty(id)) return "Client Id required";

                var rtn = "OAS Client deleted";

                try
                {
                    var query = Query.EQ("clientId", ObjectId.Parse(id.Trim()));
                    var oasClientCollection = mongoDBConnectionPool.GetCollection<OasClientList>("OasClientList");
                    var sortBy = SortBy.Descending("clientId");
                    oasClientCollection.FindAndRemove(query, sortBy);
                }
                catch (Exception ex)
                {
                    var mDetails = "DeleteOasClient(" + id + ") " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                    var exceptionEvent = new Event
                    {
                        EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                    };
                    exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
                    rtn = "Delete failed because " + ex.Message;
                }
                return rtn;
            }

        #endregion

        #region MongoDB TypeDefinitions Methods

        public void GetListByTypeDefName(String typeDefName, StringBuilder sbResponse)
        {
            try
            {
                var query = Query.EQ("_t", typeDefName);
                var myList = mongoDBConnectionPool.GetCollection("TypeDefinitions").Find(query);
                sbResponse.Append("<" + typeDefName + "s>");
                foreach (var myItem in myList)
                {
                    sbResponse.Append("<" + typeDefName);
                    foreach (var y in myItem.Select(myDe => (myDe.ToString()).Split('=')).Select(nv => " " +
                                                                                                       nv[0] +
                                                                                                       "=\"" +
                                                                                                       Security.EncryptAndEncode(nv[1], Constants.Strings.DefaultEmptyObjectId) +
                                                                                                       "\""))
                    {
                        sbResponse.Append(y);
                    }
                    sbResponse.Append(" />");
                }
                sbResponse.Append("</" + typeDefName + "s>");
            }
            catch (Exception ex)
            {
                var mDetails = "GetListByTypeDefName(" + typeDefName + ") " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                sbResponse.Append("<Error>GetListByTypeDefName exception " + ex.Message + "</Error>");
            }
        }

        public void TypeDefsSetNewObjectId(Object currentObject, string objectType)
        {
            switch (objectType)
            {
                case "Country":
                    {
                        var myObject = (Country)currentObject;
                        myObject._id = ObjectId.GenerateNewId();
                    }
                    break;

                case "RegistrationType":
                    {
                        var myObject = (RegistrationType)currentObject;
                        myObject._id = ObjectId.GenerateNewId();
                    }
                    break;

                case "State":
                    {
                        var myObject = (State)currentObject;
                        myObject._id = ObjectId.GenerateNewId();
                    }
                    break;

                case "ProviderSms":
                    {
                        var myObject = (ProviderSms)currentObject;
                        myObject._id = ObjectId.GenerateNewId();
                    }
                    break;

                case "ProviderEmail":
                    {
                        var myObject = (ProviderEmail)currentObject;
                        myObject._id = ObjectId.GenerateNewId();
                    }
                    break;

                case "ProviderVoice":
                    {
                        var myObject = (ProviderVoice)currentObject;
                        myObject._id = ObjectId.GenerateNewId();
                    }
                    break;

                case "VerificationProviders":
                    {
                        var myObject = (VerificationProvider)currentObject;
                        myObject._id = ObjectId.GenerateNewId();
                    }
                    break;
            }
        }

        public string[] GetArrayOfTypeDefs()
        {
            string[] myTypeDefs;
            try
            {
                var mongoCollection = mongoDBConnectionPool.GetCollection("TypeDefinitions");
                myTypeDefs = mongoCollection.Distinct<string>("_t").ToArray();
            }
            catch
            {
                myTypeDefs = null;
            }
            return myTypeDefs;
        }

        public string[] GetArrayOfDistinctOtpEvents()
        {
            string[] myTypeDefs;
            try
            {
                var mongoCollection = mongoDBConnectionPool.GetCollection("Otp");
                myTypeDefs = mongoCollection.Distinct<string>("CodeHistory.Details").ToArray();
            }
            catch
            {
                myTypeDefs = null;
            }
            return myTypeDefs;
        }

        public Object GetTypeDefByTypeAndName(string objectType, string name)
        {
            var mongoCollection = mongoDBConnectionPool.GetCollection("TypeDefinitions");
            var query = Query.And(Query.EQ("_t", objectType), Query.EQ("Name", name));
            try
            {
                switch (objectType)
                {
                    case "Country":
                        return mongoCollection.FindOneAs<Country>(query);

                    case "RegistrationType":
                        return mongoCollection.FindOneAs<RegistrationType>(query);

                    case "State":
                        return mongoCollection.FindOneAs<State>(query);

                    case "ProviderSms":
                        return mongoCollection.FindOneAs<ProviderSms>(query);

                    case "ProviderEmail":
                        return mongoCollection.FindOneAs<ProviderEmail>(query);

                    case "ProviderVoice":
                        return mongoCollection.FindOneAs<ProviderVoice>(query);

                    case "VerificationProviders":
                        return mongoCollection.FindOneAs<VerificationProvider>(query);

                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                var mDetails = "GetTypeDefByTypeAndName(" + objectType + ", " + name +") " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                return ex.Message;
            }
        }

        public string UpdateTypeDef(Object objectToUpdate, string objectId, string objectType)
        {
            const string createMessage = "Update succeeded";
            //var query = Query.EQ("_id", ObjectId.Parse(objectId.Trim()));
            //var sortBy = SortBy.Descending("_id");
            try
            {
                MongoCollection mongoCollection;
                switch (objectType)
                {
                    case "Country":
                        mongoCollection = mongoDBConnectionPool.GetCollection<Country>("TypeDefinitions");
                        break;
                    case "RegistrationType":
                        mongoCollection = mongoDBConnectionPool.GetCollection<RegistrationType>("TypeDefinitions");
                        break;

                    case "State":
                        mongoCollection = mongoDBConnectionPool.GetCollection<State>("TypeDefinitions");
                        break;

                    case "ProviderSms":
                        mongoCollection = mongoDBConnectionPool.GetCollection<ProviderSms>("TypeDefinitions");
                        break;

                    case "ProviderEmail":
                        mongoCollection = mongoDBConnectionPool.GetCollection<ProviderEmail>("TypeDefinitions");
                        break;

                    case "ProviderVoice":
                        mongoCollection = mongoDBConnectionPool.GetCollection<ProviderVoice>("TypeDefinitions");
                        break;

                    case "VerificationProviders":
                        mongoCollection = mongoDBConnectionPool.GetCollection<VerificationProvider>("TypeDefinitions");
                        break;

                    default:
                        return "UpdateTypeDef: Error: Invalid type:" + objectType;
                }

                mongoCollection.Save(objectToUpdate);

                //mongoCollection.FindAndRemove(query, sortBy);
                //mongoCollection.Insert(objectToUpdate);
            }
            catch (Exception ex)
            {
                var mDetails = "UpdateTypeDef( ," + objectId + ", " + objectType + ") " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                return "UpdateTypeDef.Exception:" + ex.Message;
            }
            return createMessage;
        }

        public string CreateTypeDef(Object newObject, string objectType)
        {
            try
            {
                MongoCollection mongoCollection;
                switch (objectType)
                {
                    case "Country":
                        mongoCollection = mongoDBConnectionPool.GetCollection<Country>("TypeDefinitions");
                        break;
                    case "RegistrationType":
                        mongoCollection = mongoDBConnectionPool.GetCollection<RegistrationType>("TypeDefinitions");
                        break;
                    case "State":
                        mongoCollection = mongoDBConnectionPool.GetCollection<State>("TypeDefinitions");
                        break;
                    case "ProviderAdvertising":
                        mongoCollection = mongoDBConnectionPool.GetCollection<ProviderAdvertising>("TypeDefinitions");
                        break;
                    case "ProviderSms":
                        mongoCollection = mongoDBConnectionPool.GetCollection<ProviderSms>("TypeDefinitions");
                        break;
                    case "ProviderEmail":
                        mongoCollection = mongoDBConnectionPool.GetCollection<ProviderEmail>("TypeDefinitions");
                        break;
                    case "ProviderVoice":
                        mongoCollection = mongoDBConnectionPool.GetCollection<ProviderVoice>("TypeDefinitions");
                        break;
                    case "VerificationProviders":
                        mongoCollection = mongoDBConnectionPool.GetCollection<VerificationProvider>("TypeDefinitions");
                        break;
                    default:
                        return "CreateTypeDef Failed Invalid type:" + objectType;
                }
                mongoCollection.Insert(newObject);
                return "CreateTypeDef Update successful";
            }
            catch (Exception ex)
            {
                var mDetails = "CreateTypeDef( ," + objectType + ") " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                return "CreateTypeDef Failed Exception: " + ex.Message;
            }
        }

        #endregion

        #region Geo Location Services

        public string GeoLocationByUserIp(string userIp)
        {
            var stateName = "Unknown";
            var cityName = "Unknown";
            var zipCode = "Unknown";

            try
            {
                var request = "http://freegeoip.net/xml/" + userIp;
                var webRequest = WebRequest.Create(request);
                webRequest.Method = "GET";

                var res = webRequest.GetResponse();
                var response = res.GetResponseStream();
                var xmlDoc = new XmlDocument();
                if (response != null)
                {
                    xmlDoc.Load(response);

                    stateName = xmlDoc.ChildNodes[1].ChildNodes[4].InnerText;
                    cityName = xmlDoc.ChildNodes[1].ChildNodes[5].InnerText;
                    zipCode = xmlDoc.ChildNodes[1].ChildNodes[6].InnerText;
                }

                return cityName + ", " + stateName + " " + zipCode;
            }
            catch (Exception ex)
            {
                return "Error: GeoLocationByUserIp(" + userIp + ") " + ex.Message;
            }
        }

        #endregion

        #region Default data creation method to initialize the app on an empty db

        public void CreateDocumentTemplates()
        {
            var mClientNameReplacement = dtr.ClientName; //Normal Client name replacement
            var mClientEmailReplacement = "admin@" + dtr.ClientName + ".com";

            MongoCollection mongoCollection = mongoDBConnectionPool.GetCollection("TypeDefinitions");

            #region Email Templates

            var docTemplate = new DocumentTemplate("Generic")
            {
                MessageClass = cs.Email + "-" + dtr.Generic,
                MessageFormat = mClientNameReplacement + "'s Generic subject~" +
                                dtr.FirstName + "," +
                                dtr.NL + dtr.NL +
                                dtr.DETAILS +
                                dtr.NL + dtr.NL +
                                dtr.AD,
                MessageFromAddress = mClientEmailReplacement,
                MessageFromName = mClientNameReplacement
            };

            mongoCollection.Insert(docTemplate);

            docTemplate = new DocumentTemplate("Authentication")
            {
                MessageClass = cs.Email + "-" + dtr.Authentication,
                MessageFormat = mClientNameReplacement + " Authentication~" +
                                  dtr.FirstName + "," +
                                  dtr.NL + dtr.NL +
                                  "Your Authentication code is:" + dtr.OTP +
                                  dtr.NL + dtr.NL +
                                  dtr.AD,
                MessageFromAddress = mClientEmailReplacement,
                MessageFromName = mClientNameReplacement
            };

            mongoCollection.Insert(docTemplate);

            docTemplate = new DocumentTemplate("Transaction Verification")
            {
                MessageClass = cs.Email + "-" + dtr.TransactionVerification,
                MessageFormat = mClientNameReplacement + " Transaction Verification~" +
                                  dtr.FirstName + "," +
                                  dtr.NL + dtr.NL +
                                  "Your Transaction Authorization code is:" + dtr.OTP +
                                  dtr.NL + dtr.NL +
                                  dtr.DETAILS +
                                  dtr.NL + dtr.NL +
                                  dtr.AD,
                MessageFromAddress = mClientEmailReplacement,
                MessageFromName = mClientNameReplacement
            };

            mongoCollection.Insert(docTemplate);

            docTemplate = new DocumentTemplate("Registration OTP")
            {
                MessageClass = cs.Email + "-" + dtr.RegistrationOTP,
                MessageFormat = mClientNameReplacement + " Registration Completion~" +
                                   "Hello " + dtr.FirstName + "," +
                                   dtr.NL + dtr.NL +
                                   "To complete your registration enter the verification code" + dtr.OTP +
                                   dtr.NL + "Thank you," +
                                   dtr.NL + mClientNameReplacement +
                                   dtr.NL + dtr.NL +
                                   dtr.AD,
                MessageFromAddress = mClientEmailReplacement,
                MessageFromName = mClientNameReplacement
            };

            mongoCollection.Insert(docTemplate);

            docTemplate = new DocumentTemplate("Notification")
            {
                MessageClass = cs.Email + "-" + dtr.Notification,
                MessageFormat = mClientNameReplacement + " Notification~" +
                                   "Hello " + dtr.FirstName + "," +
                                   dtr.NL + dtr.NL +
                                   dtr.DETAILS +
                                   dtr.NL + "Thank you," +
                                   dtr.NL + mClientNameReplacement +
                                   dtr.NL + dtr.NL +
                                   dtr.AD,
                MessageFromAddress = mClientEmailReplacement,
                MessageFromName = mClientNameReplacement
            };

            mongoCollection.Insert(docTemplate);

            docTemplate = new DocumentTemplate("Admin Login")
            {
                MessageClass = cs.Email + "-" + dtr.AdminLogin,
                MessageFormat = "Login~OTP:" + dtr.OTP + dtr.NL + dtr.NL,
                MessageFromAddress = mClientEmailReplacement,
                MessageFromName = mClientNameReplacement
            };

            mongoCollection.Insert(docTemplate);

            docTemplate = new DocumentTemplate("Transaction Verification - Reply")
            {
                MessageClass = cs.Email + "-" + dtr.TransactionVerificationReply,
                MessageFormat = "[ClientName] Transaction Verification~[FirstName],||Please reply with your Transaction Verification code [OTP]|[DETAILS]||[AD]",
                MessageFromAddress = mClientEmailReplacement,
                MessageFromName = mClientNameReplacement
            };

            mongoCollection.Insert(docTemplate);

            #endregion

            #region HTML Templates

            //docTemplate = new DocumentTemplate("Registration Form")
            //{
            //    MessageClass = "HTML-0",
            //    MessageFormat = GetRegistrationForm(),
            //    MessageFromAddress = "",
            //    MessageFromName = ""
            //};
            //documentTemplates.Add(docTemplate);

            #endregion

            #region Sms Templates

            docTemplate = new DocumentTemplate("Generic")
            {
                MessageClass = cs.Sms + "-" + dtr.Generic,
                MessageFormat = mClientNameReplacement + "," +
                                dtr.NL +
                                dtr.DETAILS +
                                dtr.NL +
                                dtr.AD,
                MessageFromAddress = "",
                MessageFromName = ""
            };

            mongoCollection.Insert(docTemplate);

            docTemplate = new DocumentTemplate("Authentication")
            {
                MessageClass = cs.Sms + "-" + dtr.Authentication,
                MessageFormat = mClientNameReplacement + "," +
                                dtr.NL +
                                "Authentication code:" + dtr.OTP +
                                dtr.NL + dtr.NL +
                                dtr.AD,
                MessageFromAddress = "",
                MessageFromName = ""
            };

            mongoCollection.Insert(docTemplate);

            docTemplate = new DocumentTemplate("Transaction Verification")
            {
                MessageClass = cs.Sms + "-" + dtr.TransactionVerification,
                MessageFormat = mClientNameReplacement + "," +
                                    dtr.NL +
                                    "Authorization code:" + dtr.OTP +
                                    dtr.NL + "----" +dtr.NL +
                                    dtr.DETAILS +
                                    dtr.NL +
                                    dtr.AD,
                MessageFromAddress = "",
                MessageFromName = ""
            };

            mongoCollection.Insert(docTemplate);

            docTemplate = new DocumentTemplate("Registration OTP")
            {
                MessageClass = cs.Sms + "-" + dtr.RegistrationOTP,
                MessageFormat = mClientNameReplacement + "," +
                            dtr.NL +
                            "Registration code:" + dtr.OTP +
                            dtr.NL +
                            dtr.AD,
                MessageFromAddress = "",
                MessageFromName = ""
            };

            mongoCollection.Insert(docTemplate);

            docTemplate = new DocumentTemplate("Notification")
            {
                MessageClass = cs.Sms + "-" + dtr.Notification,
                MessageFormat = mClientNameReplacement + "," +
                                dtr.NL +
                                dtr.DETAILS +
                                dtr.NL +
                                dtr.AD,
                MessageFromAddress = "",
                MessageFromName = ""
            };

            mongoCollection.Insert(docTemplate);

            docTemplate = new DocumentTemplate("Admin Login")
            {
                MessageClass = cs.Sms + "-" + dtr.AdminLogin,
                MessageFormat = mClientNameReplacement + "," +
                                    dtr.NL +
                                    "Login code:" + dtr.OTP +
                                    dtr.NL +
                                    dtr.DETAILS,
                MessageFromAddress = "",
                MessageFromName = ""
            };

            mongoCollection.Insert(docTemplate);

            docTemplate = new DocumentTemplate("OptIn")
            {
                MessageClass = cs.Sms + "-6",
                MessageFormat = mClientNameReplacement + "," +
                                    dtr.NL +
                                    "Login code:" + dtr.OTP +
                                    dtr.NL +
                                    dtr.DETAILS,
                MessageFromAddress = "",
                MessageFromName = ""
            };

            mongoCollection.Insert(docTemplate);

            docTemplate = new DocumentTemplate("OptOut")
            {
                MessageClass = cs.Sms + "-7",
                MessageFormat = mClientNameReplacement + "," +
                                    dtr.NL +
                                    "Login code:" + dtr.OTP +
                                    dtr.NL +
                                    dtr.DETAILS,
                MessageFromAddress = "",
                MessageFromName = ""
            };

            mongoCollection.Insert(docTemplate);

            docTemplate = new DocumentTemplate("Help")
            {
                MessageClass = cs.Sms + "-8",
                MessageFormat = mClientNameReplacement + "," +
                                    dtr.NL +
                                    "Login code:" + dtr.OTP +
                                    dtr.NL +
                                    dtr.DETAILS,
                MessageFromAddress = "",
                MessageFromName = ""
            };

            mongoCollection.Insert(docTemplate);

            docTemplate = new DocumentTemplate("Transaction Verification - Reply")
            {
                MessageClass = cs.Sms + "-" + dtr.TransactionVerificationReply,
                MessageFormat = "[ClientName],|Reply with [OTP] to authorize|[DETAILS]|[AD]",
                MessageFromAddress = "",
                MessageFromName = ""
            };

            mongoCollection.Insert(docTemplate);

            #endregion

            #region Voice Templates

            docTemplate = new DocumentTemplate("Generic")
            {
                MessageClass = cs.Voice + "-" + dtr.Generic,
                MessageFormat = "Hello " + dtr.FirstName + "," +
                                dtr.NL + dtr.NL +
                                "This is a message from " + mClientNameReplacement +
                                dtr.NL + dtr.NL +
                                dtr.DETAILS +
                                dtr.NL + dtr.NL +
                                dtr.AD,
                MessageFromAddress = "",
                MessageFromName = ""
            };

            mongoCollection.Insert(docTemplate);

            docTemplate = new DocumentTemplate("Authentication")
            {
                MessageClass = cs.Voice + "-" + dtr.Authentication,
                MessageFormat = "Hello " + dtr.FirstName + "," +
                                dtr.NL + dtr.NL +
                                "please enter the one time password into the log in form" +
                                dtr.NL + dtr.NL +
                                "The one time password is" +
                                dtr.OTP +
                                ",again the password is " +
                                dtr.OTP +
                                dtr.NL + dtr.NL +
                                dtr.AD,
                MessageFromAddress = "",
                MessageFromName = ""
            };

            mongoCollection.Insert(docTemplate);

            docTemplate = new DocumentTemplate("Transaction Verification")
            {
                MessageClass = cs.Voice + "-" + dtr.TransactionVerification,
                MessageFormat = "Hello " + dtr.FirstName + "," +
                                    dtr.NL + dtr.NL +
                                    "the transaction Details are " +
                                    dtr.DETAILS +
                                    " if correct enter the verification code," +
                                    dtr.OTP +
                                    dtr.NL + dtr.NL +
                                    "again the code is " +
                                    dtr.OTP +
                                    dtr.NL + dtr.NL +
                                    dtr.AD,
                MessageFromAddress = "",
                MessageFromName = ""
            };

            mongoCollection.Insert(docTemplate);

            docTemplate = new DocumentTemplate("Registration OTP")
            {
                MessageClass = cs.Voice + "-" + dtr.RegistrationOTP,
                MessageFormat = "Hello " + dtr.FirstName + "," +
                                dtr.NL + dtr.NL +
                                "To complete your registration enter the verification code " +
                                dtr.OTP +
                                dtr.NL + dtr.NL +
                                "again the code is " +
                                dtr.OTP +
                                dtr.NL + dtr.NL +
                                dtr.AD,
                MessageFromAddress = "",
                MessageFromName = ""
            };

            mongoCollection.Insert(docTemplate);

            docTemplate = new DocumentTemplate("Notification")
            {
                MessageClass = cs.Voice + "-" + dtr.Notification,
                MessageFormat = "Hello " + dtr.FirstName + "," +
                                dtr.NL + dtr.NL +
                                mClientNameReplacement + "would like you to know" +
                                dtr.DETAILS +
                                dtr.NL + dtr.NL +
                                "Thank you" +
                                dtr.NL + dtr.NL +
                                dtr.AD,
                MessageFromAddress = "",
                MessageFromName = ""
            };

            mongoCollection.Insert(docTemplate);

            docTemplate = new DocumentTemplate("Admin Login")
            {
                MessageClass = cs.Voice + "-" + dtr.AdminLogin,
                MessageFormat = "To login enter " + dtr.OTP +
                                dtr.NL + dtr.NL +
                                "again enter " + dtr.OTP +
                                dtr.NL + dtr.NL +
                                dtr.AD,
                MessageFromAddress = "",
                MessageFromName = ""
            };

            mongoCollection.Insert(docTemplate);

            docTemplate = new DocumentTemplate("Transaction Verification - Reply")
            {
                MessageClass = cs.Voice + "-" + dtr.TransactionVerificationReply,
                MessageFormat = "Hello [FirstName],||the transaction Details are [DETAILS] if correct enter on your phone the verification code,[OTP]||again the code is [OTP]||[AD]",
                MessageFromAddress = "",
                MessageFromName = ""
            };

            mongoCollection.Insert(docTemplate);

            #endregion
        }

        /// <summary> </summary>
        public List<DocumentTemplate> GetDefaultDocumentTemplates(bool pIsDefaultClient)
        {
            List<DocumentTemplate> documentTemplates = new List<DocumentTemplate>();

            MongoCollection mongoCollection = mongoDBConnectionPool.GetCollection("TypeDefinitions");

            var query = Query.EQ("_t", "DocumentTemplate");
            var sortBy = SortBy.Ascending("MessageClass");

            var defaultTemplates = mongoCollection.FindAs<DocumentTemplate>(query).SetSortOrder(sortBy);

            foreach (DocumentTemplate defaultTemplate in defaultTemplates)
            {
                documentTemplates.Add(defaultTemplate);
            }

            return documentTemplates;
        }

        //public List<DocumentTemplate> ValidateDocumentTemplates(List<DocumentTemplate> documentTemplates)
        public Object[] ValidateDocumentTemplates(List<DocumentTemplate> documentTemplates)
        {
            bool hasAddedTemplate = false;

            //CreateDocumentTemplates();

            // Get global templates to validate client's template collection
            List<DocumentTemplate> globalTemplates = GetDefaultDocumentTemplates(false);
            foreach (DocumentTemplate globalTemplate in globalTemplates)
            {
                var hasTemplate = documentTemplates.Find(FindDocumentTemplateByMessageClass(globalTemplate.MessageClass));
                if (hasTemplate == null)
                {
                    DocumentTemplate newTemplate = new DocumentTemplate(globalTemplate.MessageClass);

                    // Add the missing template
                    documentTemplates.Add(newTemplate);
                    hasAddedTemplate = true;
                }
            }

            Object[] myArray = new Object[2];

            myArray[0] = hasAddedTemplate;
            myArray[1] = documentTemplates;

            return myArray;
        }

        static Predicate<DocumentTemplate> FindDocumentTemplateByMessageClass(string messageClass)
        {
            return DocumentTemplate => DocumentTemplate.MessageClass == messageClass;
        }

        /// <summary> </summary>
        public void CopyDatabaseToBackupDatabase(string sourceDatabase, string targetDatabase)
        {
            var mMongoClient = new MongoClient(ConfigurationManager.ConnectionStrings[cfgcs.MongoServer].ConnectionString);

            try
            {
                var dbServer = mMongoClient.Settings.Server.Host + ":" + mMongoClient.Settings.Server.Port;

                var command = new CommandDocument
                {
                    { "copydb", 1 },
                    { "fromhost", dbServer },
                    { "fromdb", sourceDatabase },
                    { "todb", targetDatabase }
                };

                var mMongoDatabase = mMongoClient.GetServer().GetDatabase("admin");
                mMongoDatabase.RunCommand(command);

                // Do not run this method. It deletes the current backup. Need to revisit logic
                CleanBackupDatabases();
            }
            catch (Exception ex)
            {
                var mDetails = "CopyDatabaseToBackupDatabase(" + sourceDatabase + ", " + targetDatabase + ")" + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
            }
        }
        /// <summary> </summary>
        public void CleanBackupDatabases()
        {
            var mMongoClient = new MongoClient(ConfigurationManager.ConnectionStrings[cfgcs.MongoServer].ConnectionString);
            var mMongoServer = mMongoClient.GetServer();
            var databaseCollection = mMongoServer.GetDatabaseNames();

            var databaseNames = new ArrayList();
            var backupDatabaseName = "";

            foreach (var currentDbName in databaseCollection)
            {
                if (currentDbName.Contains("System_Backup_" + ConfigurationManager.AppSettings[cfg.MongoDbName]))
                {
                    var tmpDbData = currentDbName.Split('_');
                    for (int i = 2; i < tmpDbData.Length - 1; i++)
                    {
                        backupDatabaseName += tmpDbData[i] + "_";
                    }
                    backupDatabaseName = backupDatabaseName.Substring(0, backupDatabaseName.Length - 1);

                    if (currentDbName.Contains(ConfigurationManager.AppSettings[cfg.MongoDbName]))
                        databaseNames.Add(currentDbName);
                }
            }

            // Enumerate through the backup databases and delete anything over the set amount in web.config
            var backupCount = 0;
            var dbsInServer = databaseNames.Count;

            if (dbsInServer > cs.MaxNumberOfBackupsToKeep)
            {
                foreach (var currentDb in databaseNames)
                {
                    if (backupCount >= cs.MaxNumberOfBackupsToKeep)
                    {
                        mMongoServer.DropDatabase(currentDb.ToString());

                        var replacementTokens = Constants.TokenKeys.EventGeneratedByName + "!System Administrator";
                        replacementTokens += Constants.TokenKeys.DatabaseTarget + currentDb;

                        var dbEvent = new Event();
                        dbEvent.Create(Constants.EventLog.System.DatabaseBackupDropped, replacementTokens);
                    }
                    backupCount++;
                }
            }

            // Reverse sort
            databaseNames.Reverse();
        }

        /// <summary> </summary>
        public void RestoreDatabaseFromBackupDatabase(string backupDatabaseName)
        {
            // ReSharper disable once ObjectCreationAsStatement
            mongoClient = new MongoClient(ConfigurationManager.ConnectionStrings[cfgcs.MongoServer].ConnectionString);
            var mMongoServer = mongoClient.GetServer();
            // ReSharper disable once UnusedVariable
            var mongoDatabase = mMongoServer.GetDatabase(backupDatabaseName);
            var dbServer = mongoClient.Settings.Server.Host;

            try
            {
                // Drop the current database prior to copy due to intact indexes in the target db causing an exception
                var dbToDrop = ConfigurationManager.AppSettings[cfg.MongoDbName].ToString();
                mMongoServer.DropDatabase(dbToDrop);

                var command = new CommandDocument
                {
                    { "copydb", 1 },
                    { "fromhost", dbServer },
                    { "fromdb", backupDatabaseName },
                    { "username", "macservices" },
                    { "todb", dbToDrop }
                };

                var mMongoDatabase = mongoClient.GetServer().GetDatabase("admin");
                mMongoDatabase.RunCommand(command);

                var replacementTokens = Constants.TokenKeys.EventGeneratedByName + "!System Administrator";
                replacementTokens += Constants.TokenKeys.DatabaseSource + backupDatabaseName;
                replacementTokens += Constants.TokenKeys.DatabaseTarget + dbToDrop;

                var dbEvent = new Event();
                dbEvent.Create(Constants.EventLog.System.DatabaseRestored, replacementTokens);
            }
            catch (Exception ex)
            {
                var mDetails = "RestoreDatabaseFromBackupDatabase(" + backupDatabaseName + ") " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
            }
        }

        /// <summary> </summary>
        public void ResetSystemData(string userId, string userName, string userFullName)
        {
            if (userId == "")
                userId = Constants.Strings.DefaultAdminId;

            var adminProfile = new UserProfile(userId);

            var firstName = Security.DecodeAndDecrypt(adminProfile.FirstName, userId);
            var lastName = Security.DecodeAndDecrypt(adminProfile.LastName, userId);
            var adminFullName = firstName + " " + lastName;

            try
            {
                foreach (var collectionName in Constants.Strings.ArrCollectionsToDelete)
                {
                    switch (collectionName)
                    {
                        case "Archive":
                            mongoDBConnectionPool.DropCollection(collectionName);
                            break;

                        case "Billing":
                            mongoDBConnectionPool.DropCollection(collectionName);
                            break;

                        case "Client":
                            mongoDBConnectionPool.DropCollection(collectionName);
                            break;

                        case "Event":
                            mongoDBConnectionPool.DropCollection(collectionName);
                            break;

                        case "Group":
                            mongoDBConnectionPool.DropCollection(collectionName);
                            break;

                        case "Roles":
                            // Leave this intact
                            break;

                        case "TypeDefinitions":
                            // Leave this intact
                            break;

                        case "Users":
                            mongoDBConnectionPool.DropCollection(collectionName);
                            break;

                        case "UserProfile":
                            mongoDBConnectionPool.DropCollection(collectionName);
                            break;
                    }

                    var dbEvent = new Event
                    {
                        UserId = ObjectId.Parse(userId.Trim()),
                        ClientId = ObjectId.Parse(cs.DefaultClientId)
                    };

                    var replacementTokens = Constants.TokenKeys.EventGeneratedByName + adminFullName;
                    replacementTokens += Constants.TokenKeys.DatabaseCollectionName + collectionName;

                    dbEvent.Create(Constants.EventLog.System.DatabaseCollectionDropped, replacementTokens);
                }

                // Create the default admin, group and client for MAC
                var newAdminProfile = CreateDefaultMacRootAdmin(Constants.Strings.DefaultAdminId);
                var newGroup = CreateDefaultMacGroup(newAdminProfile);
                var newClient = CreateDefaultMacClient(newGroup, newAdminProfile);

                // Group membership
                var clientRelationship = new Relationship
                {
                    MemberId = newClient._id,
                    MemberHierarchy = "Administrator",
                    MemberType = "Client"
                };

                newAdminProfile.Relationships.Add(clientRelationship);
                newAdminProfile.Update();

                if (Convert.ToBoolean(ConfigurationManager.AppSettings["Debug"]))
                    CreateAditionalAdministrators(newGroup, newClient);
            }
            catch (Exception ex)
            {
                var mDetails = "ResetSystemData(" + userId + ", " + userName + ", " + userFullName + ") " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
            }
        }

        public void CreateAditionalAdministrators(Group defaultGroup, Client defaultClient)
        {
            try
            {
                var myAdminEvent = new Event();
                var newAdminProfile = new UserProfile("");

                myAdminEvent.ClientId = defaultClient._id;

                var directoryPath = HttpContext.Current.Server.MapPath("/");
                directoryPath += "Admin\\System\\Administrators.txt";

                using (var reader = new StreamReader(directoryPath))
                {
                    string currentAdminUser;
                    while ((currentAdminUser = reader.ReadLine()) != null)
                    {
                        var tmpItem = currentAdminUser.Split(char.Parse(tokenKeys.ItemSep));

                        var newUserId = "";
                        var adminFirstName = tmpItem[0].Replace("FirstName:", "");
                        var adminLastName = tmpItem[1].Replace("LastName:", "");
                        var adminEmail = tmpItem[2].Replace("Email:", "");
                        var adminMobilePhone = tmpItem[3].Replace("MobilePhone:", "");
                        var adminSecurityQuestion = tmpItem[4].Replace("SecurityQuestion:", "");
                        var adminSecurityAnswer = tmpItem[5].Replace("SecurityAnswer:", "");
                        var adminUserName = tmpItem[6].Replace("UserName:", "");
                        var adminPassword = tmpItem[7].Replace("Password:", "");
                        var roleName = tmpItem[8].Replace("RoleName:", "");
                        var roleId = tmpItem[9].Replace("RoleId:", "");

                        MembershipCreateStatus createStatus;
                        var newUser = Membership.CreateUser(adminUserName, adminPassword, adminEmail, adminSecurityQuestion, adminSecurityAnswer, true, out createStatus);
                        switch (createStatus)
                        {
                            case MembershipCreateStatus.Success:

                                if (newUser != null)
                                {
                                    if (newUser.ProviderUserKey != null)
                                        newUserId = newUser.ProviderUserKey.ToString();
                                }

                                // Add the user's profile
                                newAdminProfile = new UserProfile("")
                                {
                                    _id = ObjectId.Parse(newUserId.Trim()),
                                    UserId = ObjectId.Parse(newUserId.Trim()),
                                    Name = adminFirstName + " " + adminLastName,
                                    Prefix = Security.EncryptAndEncode("", newUserId),
                                    FirstName = Security.EncryptAndEncode(adminFirstName, newUserId),
                                    MiddleName = Security.EncryptAndEncode("", newUserId),
                                    LastName = Security.EncryptAndEncode(adminLastName, newUserId),
                                    Suffix = Security.EncryptAndEncode("", newUserId),
                                    DateOfBirth = Security.EncryptAndEncode("1/1/1900", newUserId)
                                };

                                newAdminProfile.Roles.Add(ObjectId.Parse(roleId.Trim()));

                                // Look up role name by roleid and use in event logging below
                                roleName = GetRoleNameByRoleId(roleId);

                                newAdminProfile.Contact.Email = Security.EncryptAndEncode(adminEmail, newUserId);
                                newAdminProfile.Contact.HomePhone = Security.EncryptAndEncode("", newUserId);
                                newAdminProfile.Contact.MobilePhone = Security.EncryptAndEncode(adminMobilePhone, newUserId);
                                newAdminProfile.Contact.WorkPhone = Security.EncryptAndEncode("480-939-2980", newUserId);
                                newAdminProfile.Contact.WorkExtension = Security.EncryptAndEncode("", newUserId);

                                newAdminProfile.Address.Street1 = Security.EncryptAndEncode("8777 E Via De Ventura", newUserId);
                                newAdminProfile.Address.Street2 = Security.EncryptAndEncode("", newUserId);
                                newAdminProfile.Address.Unit = Security.EncryptAndEncode("#280", newUserId);
                                newAdminProfile.Address.City = Security.EncryptAndEncode("Scottsdale", newUserId);
                                newAdminProfile.Address.State = Security.EncryptAndEncode("AZ", newUserId);
                                newAdminProfile.Address.Zipcode = Security.EncryptAndEncode("85258", newUserId);

                                // Only add this relationship if the admin is system or group admin
                                if (roleId == Constants.Strings.UserRoles.SystemAdmin.Item3 || roleId == Constants.Strings.UserRoles.GroupAdmin.Item3)
                                {
                                    // Create the group relationship inside the admin profile
                                    var groupRelationship = new Relationship
                                    {
                                        MemberId = defaultGroup._id,
                                        MemberHierarchy = "Administrator",
                                        MemberType = "Group"
                                    };
                                    newAdminProfile.Relationships.Add(groupRelationship);
                                }

                                // Create the client relationship inside the admin profile
                                var clientRelationship = new Relationship
                                {
                                    MemberId = defaultClient._id,
                                    MemberHierarchy = "Administrator",
                                    MemberType = "Client"
                                };
                                newAdminProfile.Relationships.Add(clientRelationship);

                                newAdminProfile.Create();

                                myAdminEvent = new Event {
                                    UserId = ObjectId.Parse(newUserId.Trim()),
                                    EventTypeDesc = Constants.TokenKeys.UserRole + roleName.Replace("User ", "")
                                            + Constants.TokenKeys.UserFullName + newAdminProfile.Name
                                            + Constants.TokenKeys.UpdatedByLoggedinAdminFullName + "!System Administrator"
                                 };
                                myAdminEvent.Create(Constants.EventLog.Registration.AdminUser.Created, null);
                                break;
                            case MembershipCreateStatus.DuplicateUserName:
                                myAdminEvent = new Event
                                {
                                    EventTypeDesc = Constants.TokenKeys.DuplicateUserName + adminUserName
                                };
                                myAdminEvent.Create(Constants.EventLog.Registration.AdminUser.DuplicateUserName, null);
                                break;
                            case MembershipCreateStatus.DuplicateEmail:
                                myAdminEvent = new Event
                                {
                                    EventTypeDesc = Constants.TokenKeys.DuplicateEmail + adminUserName
                                };
                                myAdminEvent.Create(Constants.EventLog.Registration.AdminUser.DuplicateEmail, null);  
                                break;
                            case MembershipCreateStatus.InvalidEmail:
                                myAdminEvent = new Event
                                {
                                    EventTypeDesc = Constants.TokenKeys.InvalidEmail + adminUserName
                                };
                                myAdminEvent.Create(Constants.EventLog.Registration.AdminUser.InvalidEmail, null); 
                                break;
                            case MembershipCreateStatus.InvalidAnswer:
                                myAdminEvent = new Event
                                {
                                    EventTypeDesc = Constants.TokenKeys.InvalidSecurityAnswer + adminUserName
                                };
                                myAdminEvent.Create(Constants.EventLog.Registration.AdminUser.InvalidSecurityAnswer, null); 
                                break;
                            case MembershipCreateStatus.InvalidPassword:
                                myAdminEvent = new Event
                                {
                                    EventTypeDesc = Constants.TokenKeys.InvalidPassword + adminUserName
                                };
                                myAdminEvent.Create(Constants.EventLog.Registration.AdminUser.InvalidPassword, null);
                                break;
                            default:
                                myAdminEvent = new Event
                                {
                                    EventTypeDesc = Constants.TokenKeys.UnknownRegistrationError + adminUserName
                                };
                                myAdminEvent.Create(Constants.EventLog.Registration.AdminUser.UnknownRegistrationError, null);
                                break;
                        }

                        // Log admin assignment event
                        var adminFullName = Security.DecodeAndDecrypt(newAdminProfile.FirstName, newAdminProfile._id.ToString());
                        adminFullName += " " + Security.DecodeAndDecrypt(newAdminProfile.LastName, newAdminProfile._id.ToString());

                        #region Create the Group relationship

                        // Only add this relationship if the admin is system or group admin
                        if (roleId == Constants.Strings.UserRoles.SystemAdmin.Item3 || roleId == Constants.Strings.UserRoles.GroupAdmin.Item3)
                        {
                            var adminGroupRelationship = new Relationship
                            {
                                MemberId = newAdminProfile.UserId,
                                MemberType = "Administrator",
                                MemberHierarchy = "Member"
                            };
                            defaultGroup.Relationships.Add(adminGroupRelationship);
                            defaultGroup.Update();

                            myAdminEvent = new Event { ClientId = defaultClient._id, UserId = ObjectId.Parse(newUserId.Trim()) };

                            var Tokens = Constants.TokenKeys.UserRole + roleName;
                            Tokens += Constants.TokenKeys.GroupName + defaultGroup.Name;
                            Tokens += Constants.TokenKeys.EventGeneratedByName + adminFullName;

                            myAdminEvent.Create(Constants.EventLog.Assignments.AdminAssignedToGroup, Tokens);
                        }

                        #endregion

                        #region Create the Client relationship

                        var adminClientRelationship = new Relationship
                        {
                            MemberId = newAdminProfile.UserId,
                            MemberType = "Administrator",
                            MemberHierarchy = "Member"
                        };
                        defaultClient.Relationships.Add(adminClientRelationship);
                        defaultClient.Update();

                        // Log admin assignment event
                        myAdminEvent = new Event { 
                            ClientId = defaultClient._id,
                            UserId = ObjectId.Parse(newUserId.Trim()),
                            EventTypeDesc = Constants.TokenKeys.UserRole + roleName
                            + Constants.TokenKeys.ClientName + defaultClient.Name
                            + Constants.TokenKeys.EventGeneratedByName + adminFullName
                        };
                        myAdminEvent.Create(Constants.EventLog.Assignments.AdminAssignedToClient, null);

                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                var mDetails = "CreateAditionalAdministrators() " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
            }
        }

        public UserProfile CreateDefaultMacRootAdmin(string defaultAdminUserId)
        {
            var myAdminEvent = new Event();
            var newUserId = "";

            MembershipCreateStatus createStatus;
            var newUserProfile = new UserProfile("");

            var newUser = Membership.CreateUser(Constants.Strings.DefaultAdminUserName, "!MACOTP#", Constants.Strings.DefaultAdminEmail, "Favorite company", "MobileAuthCorp", true, out createStatus);

            switch (createStatus)
            {
                case MembershipCreateStatus.Success:

                    if (newUser != null)
                    {
                        if (newUser.ProviderUserKey != null)
                            newUserId = newUser.ProviderUserKey.ToString();

                        myAdminEvent.UserId = ObjectId.Parse(defaultAdminUserId.Trim());
                    }

                    // Add the user's profile
                    newUserProfile = new UserProfile("")
                    {
                        _id = ObjectId.Parse(newUserId.Trim()),
                        UserId = ObjectId.Parse(newUserId.Trim()),
                        Name = "!System Administrator ",
                        Prefix = Security.EncryptAndEncode("", newUserId),
                        FirstName = Security.EncryptAndEncode("!System", newUserId),
                        MiddleName = Security.EncryptAndEncode("", newUserId),
                        LastName = Security.EncryptAndEncode("Administrator", newUserId),
                        Suffix = Security.EncryptAndEncode("", newUserId),
                        DateOfBirth = Security.EncryptAndEncode("1/1/1900", newUserId)
                    };

                    var roleId = ObjectId.Parse(cs.UserRoles.SystemAdmin.Item3); // System Administrator
                    newUserProfile.Roles.Add(roleId);

                    newUserProfile.Contact.Email = Security.EncryptAndEncode("system@mobileauthcorp.com", newUserId);
                    newUserProfile.Contact.HomePhone = Security.EncryptAndEncode("", newUserId);
                    newUserProfile.Contact.MobilePhone = Security.EncryptAndEncode(Constants.Strings.DefaultAdminMobilePhone, newUserId);
                    newUserProfile.Contact.WorkPhone = Security.EncryptAndEncode("480-939-2980", newUserId);
                    newUserProfile.Contact.WorkExtension = Security.EncryptAndEncode("", newUserId);

                    newUserProfile.Address.Street1 = Security.EncryptAndEncode("8777 E Via De Ventura", newUserId);
                    newUserProfile.Address.Street2 = Security.EncryptAndEncode("", newUserId);
                    newUserProfile.Address.Unit = Security.EncryptAndEncode("#280", newUserId);
                    newUserProfile.Address.City = Security.EncryptAndEncode("Scottsdale", newUserId);
                    newUserProfile.Address.State = Security.EncryptAndEncode("AZ", newUserId);
                    newUserProfile.Address.Zipcode = Security.EncryptAndEncode("85258", newUserId);

                    newUserProfile.Create();

                    myAdminEvent.EventTypeDesc = Constants.TokenKeys.UserRole + Constants.Strings.UserRoles.SystemAdmin.Item2
                                        + Constants.TokenKeys.UserFullName + newUserProfile.Name.Trim()
                                        + Constants.TokenKeys.UpdatedByLoggedinAdminFullName + newUserProfile.Name.Trim();

                    myAdminEvent.Create(Constants.EventLog.Registration.AdminUser.Created, null);
                    break;
                case MembershipCreateStatus.DuplicateUserName:
                    myAdminEvent = new Event
                    {
                        EventTypeDesc = Constants.TokenKeys.DuplicateUserName + Constants.Strings.DefaultAdminUserName
                    };
                    myAdminEvent.Create(Constants.EventLog.Registration.AdminUser.DuplicateUserName, null); 
                    break;
                case MembershipCreateStatus.DuplicateEmail:
                    myAdminEvent = new Event
                    {
                        EventTypeDesc = Constants.TokenKeys.DuplicateEmail + Constants.Strings.DefaultAdminUserName
                    };
                    myAdminEvent.Create(Constants.EventLog.Registration.AdminUser.DuplicateEmail, null); 
                    break;
                case MembershipCreateStatus.InvalidEmail:
                    myAdminEvent = new Event
                    {
                        EventTypeDesc = Constants.TokenKeys.InvalidEmail + Constants.Strings.DefaultAdminUserName
                    };
                    myAdminEvent.Create(Constants.EventLog.Registration.AdminUser.InvalidEmail, null); 
                    break;
                case MembershipCreateStatus.InvalidAnswer:
                    myAdminEvent = new Event
                    {
                        EventTypeDesc = Constants.TokenKeys.InvalidSecurityAnswer + Constants.Strings.DefaultAdminUserName
                    };
                    myAdminEvent.Create(Constants.EventLog.Registration.AdminUser.InvalidSecurityAnswer, null); 
                    break;
                case MembershipCreateStatus.InvalidPassword:
                    myAdminEvent = new Event
                    {
                        EventTypeDesc = Constants.TokenKeys.InvalidPassword + Constants.Strings.DefaultAdminUserName
                    };
                    myAdminEvent.Create(Constants.EventLog.Registration.AdminUser.InvalidPassword, null); 
                    break;
                default:
                    myAdminEvent = new Event
                    {
                        EventTypeDesc = Constants.TokenKeys.DuplicateUserName + Constants.Strings.DefaultAdminUserName
                    };
                    myAdminEvent.Create(Constants.EventLog.Registration.AdminUser.DuplicateUserName, null); 
                    break;
            }
            return newUserProfile;
        }

        public Group CreateDefaultMacGroup(UserProfile newAdminProfile)
        {
            var defaultGroup = new Group { Name = "!MAC Default Group", GroupType = "RootGroup", MacOasServicesUrl = "http://localhost:8080/MACServices", Enabled = true };
            //var tokens = "";

            try
            {
                // Decided not to do this since default sys admin will always have direct access to everything anyways
                // Create the admin relationship as owner of the new group
                //var newRelationship = new Relationship
                //{
                //    MemberId = newAdminProfile.UserId,
                //    MemberType = "Administrator",
                //    MemberHierarchy = "Member"
                //};

                defaultGroup._id = ObjectId.Parse(cs.DefaultGroupId);
                defaultGroup.Name = cs.DefaultGroupName;

                //defaultGroup.Relationships.Add(newRelationship);
                defaultGroup.Create();

                var adminFullName = Security.DecodeAndDecrypt(newAdminProfile.FirstName, newAdminProfile._id.ToString());
                adminFullName += " " + Security.DecodeAndDecrypt(newAdminProfile.LastName, newAdminProfile._id.ToString());

                // Log the group creation event
                var groupEvent = new Event
                {
                    ClientId = ObjectId.Parse(cs.DefaultClientId),
                    UserId = newAdminProfile._id
                };

                var Tokens = Constants.TokenKeys.ParentGroupName + defaultGroup.Name;
                Tokens += Constants.TokenKeys.EventGeneratedByName + adminFullName;
                Tokens += Constants.TokenKeys.GroupName + defaultGroup.Name;

                groupEvent.Create(Constants.EventLog.Group.ParentGroup.Created, Tokens);

                // Log admin assignment event
                var adminEvent = new Event
                {
                    ClientId = ObjectId.Parse(cs.DefaultClientId),
                    UserId = newAdminProfile._id
                };

                var roleName = "(End User)";
                switch (newAdminProfile.Roles[0].ToString())
                {
                    case Constants.Strings.DefaultSystemAdminId:
                        roleName = Constants.Roles.SystemAdministrator;
                        break;
                    case Constants.Strings.DefaultGroupAdminId:
                        roleName = Constants.Roles.GroupAdministrator;
                        break;
                    case Constants.Strings.DefaultClientAdminId:
                        roleName = Constants.Roles.ClientAdministrator;
                        break;
                }

                adminEvent.EventTypeDesc = Constants.TokenKeys.UserRole + roleName
                        + Constants.TokenKeys.ClientName + Constants.Strings.DefaultClientName
                        + Constants.TokenKeys.EventGeneratedByName + adminFullName
                        + Constants.TokenKeys.GroupName + defaultGroup.Name;

                adminEvent.Create(Constants.EventLog.Assignments.AdminAssignedToGroup, null);
            }
            catch (Exception ex)
            {
                var mDetails = "CreateDefaultMacGroup() " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
            }

            return defaultGroup;
        }

        public Client CreateDefaultMacClient(Group defaultGroup, UserProfile newAdminProfile)
        {
            var defaultMacClient = new Client("")
            {
                _id = ObjectId.Parse(cs.DefaultClientId),
                Name = "!MAC Default Client",
                ClientId = ObjectId.Parse(cs.DefaultClientId),
                Organization =
                    new Organization
                    {
                        TaxId = "",
                        Street1 = "8777 E. Via De Ventura",
                        Street2 = "",
                        Unit = "#230",
                        City = "Scottsdale",
                        State = ObjectId.Parse(cs.DefaultStateId), // AZ
                        Zipcode = "85257",
                        Country = "US",
                        Email = Constants.Strings.DefaultAdminEmail,
                        Phone = Constants.Strings.DefaultAdminMobilePhone,
                        Extension = ""
                    },
                RegistrationCompletionUrl = "MACClientApp",
                DocumentTemplates = GetDefaultDocumentTemplates(true)
            };

            try
            {

                // Add the current admin as the default administrator. 
                defaultMacClient.Organization.PrimaryAdminId = newAdminProfile.UserId;
                defaultMacClient.Organization.AdminNotificationProvider = new ProviderEmail();

                defaultMacClient.OtpSettings = new OtpSettings
                {
                    Length = 4,
                    EndUserRegistrationText = "Your registration Otp"
                };

                defaultMacClient.VerificationProviders = new List<VerificationProvider>();
                defaultMacClient.MessageProviders = new MessageProvider();

                // Add the default ProviderSms provider
                var defaultSms = new ProviderSms();

                defaultMacClient.MessageProviders.SmsProviders.Add(defaultSms);
                defaultMacClient.OtpSettings.ProviderList = defaultSms.Name + " (Sms)|";

                // Add the default ProviderEmail provider
                var defaultEmail = new ProviderEmail();
                defaultMacClient.MessageProviders.EmailProviders.Add(defaultEmail);
                defaultMacClient.OtpSettings.ProviderList += defaultEmail.Name + " (Email)|";

                defaultMacClient.ClientAppUrl = ConfigurationManager.AppSettings["DefaultClientAppUrl"];

                var adminFullName = Security.DecodeAndDecrypt(newAdminProfile.FirstName, newAdminProfile._id.ToString());
                adminFullName += " " + Security.DecodeAndDecrypt(newAdminProfile.LastName, newAdminProfile._id.ToString());

                // Log the client creation event
                var clientEvent = new Event { 
                    ClientId = defaultMacClient._id, 
                    UserId = newAdminProfile.UserId,
                    EventTypeDesc = Constants.TokenKeys.ClientName + Constants.Strings.DefaultClientName
                                        + Constants.TokenKeys.EventGeneratedByName + adminFullName
                };
                clientEvent.Create(Constants.EventLog.Client.Created, null);

                var adminEvent = new Event { ClientId = defaultMacClient._id, UserId = newAdminProfile.UserId };

                var roleName = "(End User)";
                switch (newAdminProfile.Roles[0].ToString())
                {
                    case Constants.Strings.DefaultSystemAdminId:
                        roleName = Constants.Roles.SystemAdministrator;
                        break;
                    case Constants.Strings.DefaultGroupAdminId:
                        roleName = Constants.Roles.GroupAdministrator;
                        break;
                    case Constants.Strings.DefaultClientAdminId:
                        roleName = Constants.Roles.ClientAdministrator;
                        break;
                }

                adminEvent.EventTypeDesc = Constants.TokenKeys.UserRole + roleName
                                                + Constants.TokenKeys.ClientName + Constants.Strings.DefaultClientName
                                                + Constants.TokenKeys.EventGeneratedByName + adminFullName;

                adminEvent.Create(Constants.EventLog.Assignments.AdminAssignedToClient, null);

                #region Create relationships

                // Decided not to do this since default system admin will always have access to everything
                // Add the currently logged in user as the first administrator for this client
                //var adminRelationship = new Relationship
                //{
                //    MemberId = newAdminProfile.UserId,
                //    MemberType = "Administrator",
                //    MemberHierarchy = "Member"
                //};
                //defaultMacClient.Relationships.Add(adminRelationship);

                // Assign the default client to the default group
                var groupRelationship = new Relationship
                {
                    MemberId = defaultGroup._id,
                    MemberType = "Group",
                    MemberHierarchy = "Member"
                };
                defaultMacClient.Relationships.Add(groupRelationship);

                // Log the client assignment to group event
                var assignmentEvent = new Event { 
                            ClientId = defaultMacClient._id, 
                            UserId = newAdminProfile.UserId,
                            EventTypeDesc = Constants.TokenKeys.ClientName + Constants.Strings.DefaultClientName
                                            + Constants.TokenKeys.EventGeneratedByName + adminFullName
                                            + Constants.TokenKeys.GroupName + defaultGroup.Name
                        };
                assignmentEvent.Create(Constants.EventLog.Assignments.ClientAssignedToGroup, null);


                // Assign the default client to the default group
                var parentGroupMembership = new Relationship
                {
                    MemberId = defaultMacClient._id,
                    MemberType = "Client",
                    MemberHierarchy = "Member"
                };
                defaultGroup.Relationships.Add(parentGroupMembership);
                defaultGroup.Update();

                #endregion

                defaultMacClient.Create();
            }
            catch (Exception ex)
            {
                var mDetails = "CreateDefaultMacClient() " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
            }

            return defaultMacClient;
        }

        #endregion

        #region Object Operations

            public ComparisonResult ShowObjectDifferences(Object originalObject, Object updatedObject)
            {
                ComparisonConfig compareConfig = new ComparisonConfig();
                compareConfig.CompareChildren = true;
                compareConfig.ComparePrivateFields = false;
                compareConfig.MaxDifferences = 1000;
                compareConfig.ExpectedName = "Original";
                compareConfig.ActualName = "New";

                CompareLogic comparison = new CompareLogic();
                comparison.Config = compareConfig;

                return comparison.Compare(originalObject, updatedObject);
            }

            public string GetObjectDifferences(Object originalObject, Object updatedObject)
            {
                string diffString;

                ComparisonConfig compareConfig = new ComparisonConfig();
                compareConfig.CompareChildren = true;
                compareConfig.ComparePrivateFields = false;
                compareConfig.MaxDifferences = 1000;
                compareConfig.ExpectedName = "Original";
                compareConfig.ActualName = "New";

                CompareLogic comparison = new CompareLogic();
                comparison.Config = compareConfig;

                ComparisonResult differencesResult = comparison.Compare(originalObject, updatedObject);

                // Have to do this since the token replacement function in utils treats these as delimiters
                diffString = differencesResult.DifferencesString.ToString().Replace(":", "-");

                diffString = diffString.Replace("Begin Differences ", "");
                diffString = diffString.Replace("End Differences (Maximum of 1000 differences shown).", "");

                return diffString;
            }

            public void DeleteTestData(string[] collectionsToDrop)
            {
                try
                {
                    foreach (var currentCollection in collectionsToDrop)
                    {
                        mongoDBConnectionPool.DropCollection(currentCollection);
                    }
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                    // ignore
                }
            }

            public void CreateDatabaseIndexes(string[] collectionsToIndex)
            {
                bool bBackgroundIndex = Convert.ToBoolean(ConfigurationManager.AppSettings[cs.BackgroundIndex]);
                bool bSparseIndex = Convert.ToBoolean(ConfigurationManager.AppSettings[cs.SparseIndex]);

                try
                {
                    foreach (var currentCollection in collectionsToIndex)
                    {
                        var dbCollection = mongoDBConnectionPool.GetCollection(currentCollection);

                        switch (currentCollection)
                        {
                            case "Accounts":
                                // Not needed since _id is indexed by default
                                //dbCollection.EnsureIndex(IndexKeys.Descending("_id"), IndexOptions.SetName("AccountId").SetBackground(bBackgroundIndex).SetSparse(bSparseIndex));
                                break;

                            case "Billing":
                                dbCollection.EnsureIndex(IndexKeys.Descending("OwnerId"), IndexOptions.SetName("OwnerId").SetBackground(bBackgroundIndex).SetSparse(bSparseIndex));
                                dbCollection.EnsureIndex(IndexKeys.Descending("OwnerId").Ascending("_t"), IndexOptions.SetName("OwnerId and _t").SetBackground(bBackgroundIndex).SetSparse(bSparseIndex));
                                dbCollection.EnsureIndex(IndexKeys.Descending("OwnerId").Ascending("OwnerType"), IndexOptions.SetName("OwnerId and Type").SetBackground(bBackgroundIndex).SetSparse(bSparseIndex));
                                break;

                            case "Client":
                                dbCollection.EnsureIndex(IndexKeys.Descending("ClientId"), IndexOptions.SetName("ClientId").SetBackground(bBackgroundIndex).SetSparse(bSparseIndex));
                                dbCollection.EnsureIndex(IndexKeys.Descending("Name"), IndexOptions.SetName("Name").SetBackground(bBackgroundIndex).SetSparse(bSparseIndex));
                                dbCollection.EnsureIndex(IndexKeys.Descending("DocumentTemplates.MessageClass"), IndexOptions.SetName("DocumentTemplates").SetBackground(bBackgroundIndex).SetSparse(bSparseIndex));
                                break;

                            case "EndUser":
                                dbCollection.EnsureIndex(IndexKeys.Descending("HashedUserId"), IndexOptions.SetName("HashedUserId").SetBackground(bBackgroundIndex).SetSparse(bSparseIndex));
                                break;

                            case "Event":
                                dbCollection.EnsureIndex(IndexKeys.Descending("Date"), IndexOptions.SetName("Date").SetBackground(bBackgroundIndex).SetSparse(bSparseIndex));
                                dbCollection.EnsureIndex(IndexKeys.Descending("Date").Ascending("ClientId"), IndexOptions.SetName("Date, ClientId").SetBackground(bBackgroundIndex).SetSparse(bSparseIndex));
                                dbCollection.EnsureIndex(IndexKeys.Descending("Date").Ascending("ClientId").Ascending("EventTypeId"), IndexOptions.SetName("Date, ClientId, EventTypeId").SetBackground(bBackgroundIndex).SetSparse(bSparseIndex));
                                dbCollection.EnsureIndex(IndexKeys.Descending("EventTypeId"), IndexOptions.SetName("EventTypeId").SetBackground(bBackgroundIndex).SetSparse(bSparseIndex));
                                break;

                            case "EventStat":
                                dbCollection.EnsureIndex(IndexKeys.Descending("OwnerId"), IndexOptions.SetName("OwnerId").SetUnique(true).SetBackground(bBackgroundIndex).SetSparse(bSparseIndex));
                                break;

                            case "Group":
                                dbCollection.EnsureIndex(IndexKeys.Descending("Name").Ascending("Relationships.MemberId"), IndexOptions.SetName("Name, ParentId").SetBackground(bBackgroundIndex).SetSparse(bSparseIndex));
                                break;

                            case "OasClientList":
                                dbCollection.EnsureIndex(IndexKeys.Descending("ClientId"), IndexOptions.SetName("ClientId").SetBackground(bBackgroundIndex).SetSparse(bSparseIndex));
                                dbCollection.EnsureIndex(IndexKeys.Descending("Name"), IndexOptions.SetName("Name").SetBackground(bBackgroundIndex).SetSparse(bSparseIndex));
                                break;

                            case "Otp":
                                //dbCollection.EnsureIndex(IndexKeys.Descending("CodeHistory.Date").Descending("ClientId"), IndexOptions.SetName("Date, ClientId").SetBackground(bBackgroundIndex).SetSparse(bSparseIndex));
                                //dbCollection.EnsureIndex(IndexKeys.Descending("CodeHistory.Date").Descending("CodeHistory.EventTypeId"), IndexOptions.SetName("CodeHistory.EventTypeId").SetBackground(bBackgroundIndex).SetSparse(bSparseIndex));
                                //dbCollection.EnsureIndex(IndexKeys.Descending("DeliveryMethodId"), IndexOptions.SetName("DeliveryMethodId").SetBackground(bBackgroundIndex).SetSparse(bSparseIndex));
                                //dbCollection.EnsureIndex(IndexKeys.Descending("DeliveryMethodId").Descending("ClientId"), IndexOptions.SetName("DeliveryMethodId, ClientId").SetBackground(bBackgroundIndex).SetSparse(bSparseIndex));
                                //dbCollection.EnsureIndex(IndexKeys.Descending("CodeHistory.Date").Descending("DeliveryMethodId").Descending("ClientId"), IndexOptions.SetName("Date, DeliveryMethodId, ClientId").SetBackground(bBackgroundIndex).SetSparse(bSparseIndex));
                                break;

                            case "UserProfile":
                                dbCollection.EnsureIndex(IndexKeys.Descending("Roles"), IndexOptions.SetName("Roles").SetBackground(bBackgroundIndex).SetSparse(bSparseIndex));
                                break;
                        }
                    }
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                    // ignore
                }
            }

            public void DeleteAdminRelationshipsFromClientsAndGroups()
            {
                // Handle Clients
                var mongoCollection = mongoDBConnectionPool.GetCollection("Client");
                var clientCollection = mongoCollection.FindAllAs<Client>();

                foreach (var currentClient in clientCollection)
                {
                    currentClient.Relationships.RemoveAll(FindRelationshipByMemberType("Administrator"));
                    currentClient.Update();
                }

                // Handle Groups
                mongoCollection = mongoDBConnectionPool.GetCollection("Group");
                var groupCollection = mongoCollection.FindAllAs<Group>();
                foreach (var currentGroup in groupCollection)
                {
                    currentGroup.Relationships.RemoveAll(FindRelationshipByMemberType("Administrator"));
                    currentGroup.Update();
                }
            }

            public void DeleteAllRelationships()
            {
                // Handle Clients
                var mongoCollection = mongoDBConnectionPool.GetCollection("Client");
                var clientCollection = mongoCollection.FindAllAs<Client>();
                foreach (Client currentClient in clientCollection)
                {
                    currentClient.Relationships = new List<Relationship>();
                    currentClient.Update();
                }

                mongoCollection = mongoDBConnectionPool.GetCollection("UserProfile");
                var profileCollection = mongoCollection.FindAllAs<UserProfile>();
                foreach (UserProfile currentProfile in profileCollection)
                {
                    currentProfile.Relationships = new List<Relationship>();
                    currentProfile.Update();
                }

                mongoCollection = mongoDBConnectionPool.GetCollection("Group");
                var groupCollection = mongoCollection.FindAllAs<Group>();
                foreach (Group currentGroup in groupCollection)
                {
                    var myNewRelationships = new List<Relationship>();

                    foreach(Relationship myRelationship in currentGroup.Relationships)
                    {
                        if (myRelationship.MemberType == "Group")
                            myNewRelationships.Add(myRelationship);
                    }

                    currentGroup.Relationships = myNewRelationships;
                    currentGroup.Update();
                }

                // Handle Administrators
                //DeleteAdminRelationshipsFromClientsAndGroups();
            }

            //public void ManageObjectRelationships_UserAndHelpTopic(ObjectId loggedInAdminId, bool createRelationship, UserProfile userProfile, HelpTopic topicObject)
            //{
            //    try
            //    {
            //        var memberType = "User";
            //        var memberHierarchy = "Help";

            //        var currentAdminId = userProfile._id;
            //        var currentClientId = topicObject._id;

            //        var myEvent = new Event { UserId = loggedInAdminId, ClientId = currentClientId };

            //        // Have to do this as it is getting reset when updating
            //        userProfile.Name = Security.DecodeAndDecrypt(userProfile.FirstName, currentAdminId.ToString()) + " " + Security.DecodeAndDecrypt(userProfile.LastName, currentAdminId.ToString());

            //        var userRole = GetRoleNameByRoleId(userProfile.Roles[0].ToString());

            //        var tokens = "";
            //        //tokens += Constants.TokenKeys.UserRole + userRole;
            //        //tokens += Constants.TokenKeys.EventGeneratedByName + userProfile.Name;
            //        //tokens += Constants.TokenKeys.ClientName + topicObject.Name;

            //        #region Remove the relationships between the objects

            //        // Remove the Topic from the User Profile
            //        foreach (var currRelationship in userProfile.Relationships.ToList())
            //        {
            //            if (currRelationship.MemberId == topicObject._id)
            //            {
            //                if (currRelationship.MemberType == memberType)
            //                {
            //                    if (currRelationship.MemberHierarchy == memberHierarchy)
            //                    {
            //                        userProfile.Relationships.Remove(currRelationship);
            //                        userProfile.Update();

            //                        myEvent._id = ObjectId.GenerateNewId();
            //                        myEvent.Create(Constants.EventLog.Assignments.ClientRemovedFromUser, tokens);
            //                    }
            //                }
            //            }
            //        }

            //        // Remove the User from the Topic
            //        foreach (var currRelationship in topicObject.Relationships)
            //        {
            //            if (currRelationship.MemberId == userProfile._id)
            //            {
            //                if (currRelationship.MemberType == memberType)
            //                {
            //                    if (currRelationship.MemberHierarchy == memberHierarchy)
            //                    {
            //                        //topicObject.Relationships.Remove(currRelationship);
            //                        //topicObject.Update();

            //                        myEvent._id = ObjectId.GenerateNewId();
            //                        myEvent.Create(Constants.EventLog.Assignments.UserRemovedFromClient, tokens);
            //                    }
            //                }
            //            }
            //        }

            //        #endregion

            //        if (createRelationship)
            //        {
            //            #region Create the relationships between the objects if specified

            //            // Add the User to the Topic
            //            var adminToGroupRelationship = new Relationship
            //            {
            //                MemberId = currentAdminId,
            //                MemberType = memberType,
            //                MemberHierarchy = memberHierarchy
            //            };
            //            //topicObject.Relationships.Add(adminToGroupRelationship);
            //            //topicObject.Update();

            //            myEvent._id = ObjectId.GenerateNewId();
            //            myEvent.Create(Constants.EventLog.Assignments.UserAssignedToClient, tokens);

            //            // Add the Client to the User
            //            var groupToAdminRelationship = new Relationship
            //            {
            //                MemberId = currentClientId,
            //                MemberType = memberType,
            //                MemberHierarchy = memberHierarchy
            //            };
            //            userProfile.Relationships.Add(groupToAdminRelationship);
            //            userProfile.Update();

            //            myEvent._id = ObjectId.GenerateNewId();
            //            myEvent.Create(Constants.EventLog.Assignments.ClientAssignedToUser, tokens);

            //            #endregion
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        var mDetails = "ManageObjectRelationships_UserAndTopic() " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
            //        var exceptionEvent = new Event
            //        {
            //            EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
            //        };
            //        exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
            //    }
            //}

            public void ManageObjectRelationships_UserAndClient(ObjectId loggedInAdminId, bool createRelationship, UserProfile userProfile, Client clientObject)
            {
                try
                {
                    //var iCount = 0;
                    var memberType = "User";
                    //var memberHierarchy = "Client";

                    var currentAdminId = userProfile._id;
                    var currentClientId = clientObject._id;

                    var myEvent = new Event { UserId = loggedInAdminId, ClientId = currentClientId };

                    // Have to do this as it is getting reset when updating
                    userProfile.Name = Security.DecodeAndDecrypt(userProfile.FirstName, currentAdminId.ToString()) + " " + Security.DecodeAndDecrypt(userProfile.LastName, currentAdminId.ToString());

                    var userRole = GetRoleNameByRoleId(userProfile.Roles[0].ToString());

                    var tokens = "";
                    tokens += Constants.TokenKeys.UserRole + userRole;
                    tokens += Constants.TokenKeys.EventGeneratedByName + userProfile.Name;
                    tokens += Constants.TokenKeys.ClientName + clientObject.Name;

                    #region Remove the relationships between the objects

                    // Remove the Client from the User Profile
                    foreach (var currRelationship in userProfile.Relationships.ToList())
                    {
                        if(currRelationship.MemberId == clientObject._id)
                        {
                            if (currRelationship.MemberType == "User")
                            {
                                if (currRelationship.MemberHierarchy == "Client")
                                {
                                    userProfile.Relationships.Remove(currRelationship);
                                    userProfile.Update();

                                    myEvent._id = ObjectId.GenerateNewId();
                                    myEvent.Create(Constants.EventLog.Assignments.ClientRemovedFromUser, tokens);
                                }
                            }
                        }
                    }

                    // Remove the User from the Client
                    foreach (var currRelationship in clientObject.Relationships)
                    {
                        if (currRelationship.MemberId == userProfile._id)
                        {
                            if (currRelationship.MemberType == "User")
                            {
                                if (currRelationship.MemberHierarchy == "Member")
                                {
                                    clientObject.Relationships.Remove(currRelationship);
                                    clientObject.Update();

                                    myEvent._id = ObjectId.GenerateNewId();
                                    myEvent.Create(Constants.EventLog.Assignments.UserRemovedFromClient, tokens);
                                }
                            }
                        }
                    }

                    #endregion

                    if (createRelationship)
                    {
                        #region Create the relationships between the objects if specified

                        // Add the User to the Client
                        var adminToGroupRelationship = new Relationship
                        {
                            MemberId = currentAdminId,
                            MemberType = memberType,
                            MemberHierarchy = "Member"
                        };
                        clientObject.Relationships.Add(adminToGroupRelationship);
                        clientObject.Update();

                        myEvent._id = ObjectId.GenerateNewId();
                        myEvent.Create(Constants.EventLog.Assignments.UserAssignedToClient, tokens);

                        // Add the Client to the User
                        var groupToAdminRelationship = new Relationship
                        {
                            MemberId = currentClientId,
                            MemberType = memberType,
                            MemberHierarchy = "Client"
                        };
                        userProfile.Relationships.Add(groupToAdminRelationship);
                        userProfile.Update();

                        myEvent._id = ObjectId.GenerateNewId();
                        myEvent.Create(Constants.EventLog.Assignments.ClientAssignedToUser, tokens);

                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    var mDetails = "ManageObjectRelationships_UserAndClient() " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                    var exceptionEvent = new Event
                    {
                        EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                    };
                    exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
                }
            }

            public void ManageObjectRelationships_AdminAndClient(ObjectId loggedInAdminId, bool createRelationship, UserProfile adminProfile, Client clientObject)
            {
                try
                {
                    var memberType = "Administrator";
                    var currentAdminId = adminProfile._id;
                    var currentClientId = clientObject._id;

                    var myEvent = new Event { UserId = loggedInAdminId, ClientId = currentClientId };

                    //var loggedInAdminProfile = new UserProfile(loggedInAdminId.ToString());
                    //var loggedInAdminName = Security.DecodeAndDecrypt(loggedInAdminProfile.FirstName, loggedInAdminId.ToString()) + " " + Security.DecodeAndDecrypt(loggedInAdminProfile.LastName, loggedInAdminId.ToString());

                    // Have to do this as it is getting reset when updating
                    adminProfile.Name = Security.DecodeAndDecrypt(adminProfile.FirstName, currentAdminId.ToString()) + " " + Security.DecodeAndDecrypt(adminProfile.LastName, currentAdminId.ToString());

                    var adminRole = GetRoleNameByRoleId(adminProfile.Roles[0].ToString());
                    if (adminRole == "(Client User)")
                        memberType = "User";

                    var tokens = "";
                    tokens += Constants.TokenKeys.UserRole + adminRole;
                    tokens += Constants.TokenKeys.EventGeneratedByName + adminProfile.Name;
                    tokens += Constants.TokenKeys.ClientName + clientObject.Name;

                    #region Remove the relationships between the objects

                    // Remove the Client from the Admin
                    var itemsAffected = adminProfile.Relationships.RemoveAll(FindRelationshipByMemberId(currentClientId));
                    if (itemsAffected > 0)
                    {
                        adminProfile.Update();

                        myEvent._id = ObjectId.GenerateNewId();
                        myEvent.Create(Constants.EventLog.Assignments.ClientRemovedFromAdmin, tokens);
                    }

                    // Remove the Admin from the Client
                    itemsAffected = clientObject.Relationships.RemoveAll(FindRelationshipByMemberId(currentAdminId));
                    if (itemsAffected > 0)
                    {
                        clientObject.Update();

                        myEvent._id = ObjectId.GenerateNewId();
                        myEvent.Create(Constants.EventLog.Assignments.AdminRemovedFromClient, tokens);
                    }

                    #endregion

                    if (createRelationship)
                    {
                        #region Create the relationships between the objects if specified

                        // Add the Admin to the Client
                        var adminToGroupRelationship = new Relationship
                        {
                            MemberId = currentAdminId,
                            MemberType = memberType,
                            MemberHierarchy = "Member"
                        };
                        clientObject.Relationships.Add(adminToGroupRelationship);
                        clientObject.Update();

                        myEvent._id = ObjectId.GenerateNewId();
                        myEvent.Create(Constants.EventLog.Assignments.AdminAssignedToClient, tokens);

                        // Add the Client to the Admin
                        var groupToAdminRelationship = new Relationship
                        {
                            MemberId = currentClientId,
                            MemberType = memberType,
                            MemberHierarchy = "Client"
                        };
                        adminProfile.Relationships.Add(groupToAdminRelationship);
                        adminProfile.Update();

                        myEvent._id = ObjectId.GenerateNewId();
                        myEvent.Create(Constants.EventLog.Assignments.ClientAssignedToAdmin, tokens);

                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    var mDetails = "ManageObjectRelationships_AdminAndClient() " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                    var exceptionEvent = new Event
                    {
                        EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                    };
                    exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
                }
            }

            public void ManageObjectRelationships_AdminAndGroup(ObjectId loggedInAdminId, bool createRelationship, UserProfile adminProfile, Group groupObject)
            {
                try
                {
                    var currentAdminId = adminProfile._id;
                    var groupId = groupObject._id;

                    var myEvent = new Event { UserId = loggedInAdminId, ClientId = groupId };

                    //var loggedInAdminProfile = new UserProfile(loggedInAdminId.ToString());
                    //var loggedInAdminName = Security.DecodeAndDecrypt(loggedInAdminProfile.FirstName, loggedInAdminId.ToString()) + " " + Security.DecodeAndDecrypt(loggedInAdminProfile.LastName, loggedInAdminId.ToString());

                    // Have to do this as it is getting reset when updating
                    adminProfile.Name = Security.DecodeAndDecrypt(adminProfile.FirstName, currentAdminId.ToString()) + " " + Security.DecodeAndDecrypt(adminProfile.LastName, currentAdminId.ToString());

                    var adminRole = GetRoleNameByRoleId(adminProfile.Roles[0].ToString());

                    var tokens = "";
                    tokens += Constants.TokenKeys.UserRole + adminRole;
                    tokens += Constants.TokenKeys.EventGeneratedByName + adminProfile.Name;
                    tokens += Constants.TokenKeys.GroupName + groupObject.Name;

                    #region Remove the relationships between the objects

                    // Remove the Group from the Admin
                    //var adminRelationship = new Relationship
                    //{
                    //    Enabled = true,
                    //    MemberId = currentAdminId,
                    //    MemberType = "Group",
                    //    MemberHierarchy = "Administrator"
                    //};
                    var itemsAffected = adminProfile.Relationships.RemoveAll(FindRelationshipByMemberId(groupId));
                    if (itemsAffected > 0)
                    {
                        adminProfile.Update();

                        myEvent._id = ObjectId.GenerateNewId();
                        myEvent.Create(Constants.EventLog.Assignments.GroupRemovedFromAdmin, tokens);
                    }

                    // Remove the Admin from the Group
                    //var groupRelationship = new Relationship
                    //{
                    //    MemberId = currentAdminId,
                    //    MemberType = "Administrator",
                    //    MemberHierarchy = "Member"
                    //};
                    itemsAffected = groupObject.Relationships.RemoveAll(FindRelationshipByMemberId(currentAdminId));
                    if (itemsAffected > 0)
                    {
                        groupObject.Update();

                        myEvent._id = ObjectId.GenerateNewId();
                        myEvent.Create(Constants.EventLog.Assignments.AdminRemovedFromGroup, tokens);
                    }

                    #endregion

                    if (createRelationship)
                    {
                        #region Create the relationships between the objects if specified

                        // Add the Admin to the Group
                        var adminToGroupRelationship = new Relationship
                        {
                            MemberId = currentAdminId,
                            MemberType = "Administrator",
                            MemberHierarchy = "Member"
                        };
                        groupObject.Relationships.Add(adminToGroupRelationship);
                        groupObject.Update();

                        myEvent._id = ObjectId.GenerateNewId();
                        myEvent.Create(Constants.EventLog.Assignments.AdminAssignedToGroup, tokens);

                        // Add the Group to the Admin
                        var groupToAdminRelationship = new Relationship
                        {
                            MemberId = groupId,
                            MemberType = "Administrator",
                            MemberHierarchy = "Group"
                        };
                        adminProfile.Relationships.Add(groupToAdminRelationship);
                        adminProfile.Update();

                        myEvent._id = ObjectId.GenerateNewId();
                        myEvent.Create(Constants.EventLog.Assignments.GroupAssignedToAdmin, tokens);

                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    var mDetails = "ManageObjectRelationships_AdminAndGroup() " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                    var exceptionEvent = new Event
                    {
                        EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                    };
                    exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
                }
            }

            public void ManageObjectRelationships_GroupAndClient(ObjectId loggedInAdminId, bool createRelationship, Client myClient, Group myGroup)
            {
                try
                {
                    var myClientId = myClient._id;
                    var myGroupId = myGroup._id;
                
                    var myEvent = new Event { UserId = loggedInAdminId };
                    myEvent.ClientId = myClient._id;

                    var loggedInAdminProfile = new UserProfile(loggedInAdminId.ToString());
                    //var loggedInAdminName = Security.DecodeAndDecrypt(loggedInAdminProfile.FirstName, loggedInAdminId.ToString()) + " " + Security.DecodeAndDecrypt(loggedInAdminProfile.LastName, loggedInAdminId.ToString());

                    // Have to do this as it is getting reset when updating
                    loggedInAdminProfile.Name = Security.DecodeAndDecrypt(loggedInAdminProfile.FirstName, loggedInAdminId.ToString()) + " " + Security.DecodeAndDecrypt(loggedInAdminProfile.LastName, loggedInAdminId.ToString());

                    var adminRole = GetRoleNameByRoleId(loggedInAdminProfile.Roles[0].ToString());

                    var tokens = "";
                    tokens += Constants.TokenKeys.UserRole + adminRole;
                    tokens += Constants.TokenKeys.EventGeneratedByName + loggedInAdminProfile.Name;
                    tokens += Constants.TokenKeys.GroupName + myGroup.Name;
                    tokens += Constants.TokenKeys.ClientName + myClient.Name;

                    #region Remove the relationships between the objects

                        // Remove Group from Client
                        var itemsAffected = myClient.Relationships.RemoveAll(FindRelationshipByMemberId(myGroupId));
                        if (itemsAffected > 0)
                        {
                            myClient.Update();
                            myEvent._id = ObjectId.GenerateNewId();
                            myEvent.Create(Constants.EventLog.Assignments.GroupRemovedFromClient, tokens);
                        }

                        // Remove Client from Group
                        itemsAffected = myGroup.Relationships.RemoveAll(FindRelationshipByMemberId(myClientId));
                        if (itemsAffected > 0)
                        {
                            myGroup.Update();
                            myEvent._id = ObjectId.GenerateNewId();
                            myEvent.Create(Constants.EventLog.Assignments.ClientRemovedFromGroup, tokens);
                        }

                    #endregion

                    if (createRelationship)
                    {
                        #region Create the relationships between the objects if specified

                            // Add Client to Group
                            var relationshipClientToGroup = new Relationship
                            {
                                Enabled = true,
                                MemberId = myClient._id,
                                MemberType = "Client",
                                MemberHierarchy = "Member"
                            };
                            myGroup.Relationships.Add(relationshipClientToGroup);
                            myGroup.Update();

                            myEvent._id = ObjectId.GenerateNewId();
                            myEvent.Create(Constants.EventLog.Assignments.ClientAssignedToGroup, tokens);

                            // Add Group to Client
                            var relationshipGroupToClient = new Relationship
                            {
                                MemberId = myGroupId,
                                MemberType = "Group",
                                MemberHierarchy = "Member"
                            };
                            myClient.Relationships.Add(relationshipGroupToClient);

                            myClient.DateModified = DateTime.UtcNow;
                            myClient.Update();

                            //myEvent._id = ObjectId.GenerateNewId();
                            //myEvent.Create(Constants.EventLog.Assignments.GroupAssignedToClient, tokens);

                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    var mDetails = "ManageObjectRelationships_GroupAndClient() " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                    var exceptionEvent = new Event
                    {
                        EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                    };
                    exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
                }
            }

            public void ManageObjectRelationships_GroupAndGroup(ObjectId loggedInAdminId, bool createRelationship, Group myGroup, Group myParentGroup)
            {
                try
                {
                    var myGroupId = myGroup._id;
                    var myParentGroupId = myParentGroup._id;

                    var myEvent = new Event { UserId = loggedInAdminId };
                    //myEvent.ClientId = groupId;

                    var loggedInAdminProfile = new UserProfile(loggedInAdminId.ToString());
                    //var loggedInAdminName = Security.DecodeAndDecrypt(loggedInAdminProfile.FirstName, loggedInAdminId.ToString()) + " " + Security.DecodeAndDecrypt(loggedInAdminProfile.LastName, loggedInAdminId.ToString());

                    // Have to do this as it is getting reset when updating
                    loggedInAdminProfile.Name = Security.DecodeAndDecrypt(loggedInAdminProfile.FirstName, loggedInAdminId.ToString()) + " " + Security.DecodeAndDecrypt(loggedInAdminProfile.LastName, loggedInAdminId.ToString());

                    var adminRole = GetRoleNameByRoleId(loggedInAdminProfile.Roles[0].ToString());

                    var tokens = "";
                    tokens += Constants.TokenKeys.UserRole + adminRole;
                    tokens += Constants.TokenKeys.EventGeneratedByName + loggedInAdminProfile.Name;
                    tokens += Constants.TokenKeys.GroupName + myGroup.Name;

                    #region Remove the relationships between the objects

                    // Remove myGroup from myParentGroup
                    var itemsAffected = myParentGroup.Relationships.RemoveAll(FindRelationshipByMemberId(myGroupId));
                    if (itemsAffected > 0)
                    {
                        myParentGroup.Update();

                        myEvent._id = ObjectId.GenerateNewId();
                        myEvent.Create(Constants.EventLog.Assignments.SubGroupRemovedFromParentGroup, tokens);
                    }

                    // Remove myParentGroup from myGroup
                    itemsAffected = myGroup.Relationships.RemoveAll(FindRelationshipByMemberId(myParentGroupId));
                    if (itemsAffected > 0)
                    {
                        myGroup.Update();

                        myEvent._id = ObjectId.GenerateNewId();
                        myEvent.Create(Constants.EventLog.Assignments.AdminRemovedFromGroup, tokens);
                    }

                    #endregion

                    if (createRelationship)
                    {
                        #region Create the relationships between the objects if specified

                        // Add myGroup to myParentGroup
                        var childRelationship = new Relationship
                        {
                            Enabled = true,
                            MemberId = myGroupId,
                            MemberType = "Group",
                            MemberHierarchy = "Child"
                        };
                        myParentGroup.Relationships.Add(childRelationship);
                        myParentGroup.Update();

                        // Add myParentGroup to myGroup
                        var parentRelationship = new Relationship
                        {
                            MemberId = myParentGroupId,
                            MemberType = "Group",
                            MemberHierarchy = "Parent"
                        };
                        myGroup.Relationships.Add(parentRelationship);
                        myGroup.Update();

                        // Add the administrator to the group
                        var adminToGroupRelationship = new Relationship
                        {
                            //MemberId = currentAdminId,
                            MemberType = "Administrator",
                            MemberHierarchy = "Member"
                        };
                        myGroup.Relationships.Add(adminToGroupRelationship);
                        myGroup.Update();

                        myEvent._id = ObjectId.GenerateNewId();
                        myEvent.Create(Constants.EventLog.Assignments.AdminAssignedToGroup, tokens);

                        // Add the group to the administrator
                        var groupToAdminRelationship = new Relationship
                        {
                            MemberId = myGroup._id,
                            MemberType = "Administrator",
                            MemberHierarchy = "Group"
                        };
                        loggedInAdminProfile.Relationships.Add(groupToAdminRelationship);
                        loggedInAdminProfile.Update();

                        myEvent._id = ObjectId.GenerateNewId();
                        myEvent.Create(Constants.EventLog.Assignments.GroupAssignedToAdmin, tokens);

                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    var mDetails = "ManageObjectRelationships_GroupAndGroup() " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                    var exceptionEvent = new Event
                    {
                        EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                    };
                    exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
                }
            }

            Predicate<Relationship> FindRelationshipByMemberType(string memberType)
            {
                return relationship => relationship.MemberType == memberType;
            }

            static Predicate<Relationship> FindRelationshipByMemberId(ObjectId memberId)
            {
                return relationship => relationship.MemberId == memberId;
            }

// ReSharper disable once UnusedMember.Local
            static Predicate<Relationship> FindRelationshipByMemberIdAndMembershipType(ObjectId memberId, string membershipType, string memberHierarchy)
            {
// ReSharper disable once UnusedVariable
                var x1 = membershipType;
// ReSharper disable once UnusedVariable
                var x2 = memberHierarchy;
                return relationship => relationship.MemberId == memberId; // && relationship.MemberType == membershipType && relationship.MemberHierarchy;
            }

        #endregion

        #region XML Response Methods

        public static Stopwatch ProcessTimer;

        /// <summary> </summary>
        public string LowerCaseXmlTags(string xml)
        {
            return Regex.Replace(
                xml,
                @"<[^<>]+>",
                m => m.Value.ToLower(),
                RegexOptions.Multiline | RegexOptions.Singleline);
        }

        public void InitializeXmlResponse(StringBuilder sbResponse)
        {
            sbResponse.Clear();

            sbResponse.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");

            ProcessTimer = Stopwatch.StartNew();

            sbResponse.Append("<macResponse totalProcessTime=''>");
            sbResponse.Append("<calledMethod>");
            var st = new StackTrace();
            var sf = st.GetFrame(1);

            var currentMethodName = sf.GetMethod();

            var callingMethod = currentMethodName.ToString().Replace("System.Xml.XmlDocument", "");
            var iStartIndex = callingMethod.IndexOf('(');

            callingMethod = callingMethod.Substring(0, iStartIndex) + "()";

            sbResponse.Append(callingMethod.Trim());
            sbResponse.Append("</calledMethod>");
        }

        public XmlDocument FinalizeXmlResponseWithError(string errorMsg, string pServiceCode)
        {
            var myResponse = new StringBuilder();
            InitializeXmlResponse(myResponse);
            myResponse.Append("<" + sr.Error + ">" + sr.Error + ", " + SanitizeXmlString(errorMsg) + "</" + sr.Error + ">");
            return FinalizeXmlResponse(myResponse, pServiceCode);
        }

        public XmlDocument FinalizeXmlResponse(StringBuilder sbResponse, string pServiceCode)
        {
            var myxml = new XmlDocument();


            sbResponse.Append("</macResponse>");

            ProcessTimer.Stop();
            sbResponse.Replace("totalProcessTime=''", "totalProcessTime='" + ProcessTimer.ElapsedMilliseconds + "ms'");
            
            // should service response be logged ??
            var mLogResp = ConfigurationManager.AppSettings[cfg.DebugLogResponses];  
            // all disabled
            if ((String.IsNullOrEmpty(mLogResp)) || (mLogResp == "false") || String.IsNullOrEmpty(pServiceCode))
            {
                myxml.LoadXml(SanitizeXmlString(sbResponse.ToString()));
                return myxml;
            }
            if ((mLogResp == "true") || (mLogResp.Contains(pServiceCode)) || pServiceCode == "LE")
            {
                var mLogEvent = new Event
                {
                    EventTypeName = Constants.Strings.ServiceLog,
                    ClientId = ObjectId.Parse(cs.DefaultClientId),
                    UserId = ObjectId.Parse(cs.DefaultEmptyObjectId),
                    EventTypeDesc = SanitizeXmlString(sbResponse.ToString()) + Environment.NewLine
                };
                if (HttpContext.Current != null)
                {
                    mLogEvent.UserIpAddress =
                        HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (String.IsNullOrEmpty(mLogEvent.UserIpAddress))
                        mLogEvent.UserIpAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
                try
                {
                    mLogEvent.Create();
                }
                catch (Exception ex)
                {
                    var mDetails = "FinalizeXmlResponse() " +
                                    ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") +
                                    Constants.TokenKeys.ClientName + "NA";
                    var exceptionEvent = new Event
                    {
                        EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                    };
                    exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
                }
            }
            myxml.LoadXml(SanitizeXmlString(sbResponse.ToString()));
            return myxml;
        }

        public XmlDocument EmptyXml()
        {
            return new XmlDocument();
        }

        public string SanitizeString(string response)
        {
            var cleanedString = response;
            cleanedString = cleanedString.Replace(" & ", " and ");
            cleanedString = cleanedString.Replace("'", "&apos;");

            return cleanedString;
        }

        public string SanitizeXmlString(string response)
        {
            var cleanedString = response;
            cleanedString = cleanedString.Replace("&", "&amp;").Replace(Environment.NewLine, "");
            return cleanedString;
        }

        // Error exit to log system event
        public XmlDocument EventLogError_FinalizeXmlResponse(String pServiceName, String pCid, string pErrorToLog, string xpErrorNumber)
        {
            var myResponse = new StringBuilder();
            myResponse.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            myResponse.Append("<macResponse>");
            myResponse.Append("<calledMethod>" + pServiceName + "()</calledMethod>");
            myResponse.Append("<Error>" + sr.Error + ", " + SanitizeXmlString(pErrorToLog) + "</Error>");
            myResponse.Append("</macResponse>");
            var mLogEvent = new Event
            {
                EventTypeName = "Failures (General): ",
                EventTypeDesc = SanitizeXmlString(myResponse.ToString())
            };
            try
            {
                mLogEvent.ClientId = ObjectId.Parse(pCid.Trim());
            }
            catch
            {
                mLogEvent.ClientId = ObjectId.Parse(cs.DefaultClientId);
            }
            if (HttpContext.Current != null)
            {
                mLogEvent.UserIpAddress =
                    HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (String.IsNullOrEmpty(mLogEvent.UserIpAddress))
                    mLogEvent.UserIpAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            try
            {
                mLogEvent.Create();
            }
            catch (Exception ex)
            {
                var mDetails = "FinalizeXmlResponse() " +
                                ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") +
                                Constants.TokenKeys.ClientName + "NA";
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.EventLog.Failures.General + mDetails
                };
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
            }
            var myxml = new XmlDocument();
            myxml.LoadXml(SanitizeXmlString(myResponse.ToString()));
            return myxml;
        }
        #endregion

        #region String Methods

        public bool ValidateEmailAddress(string pEmailAddress)
        {
            return Regex.IsMatch(pEmailAddress, regX.EmailAddress);
        }

        public bool ValidatePhoneNumber(string pPhoneNumber)
        {
            if (Regex.IsMatch(pPhoneNumber, regX.PhoneNumber) == false) return false;
            var tmp = pPhoneNumber.Replace(".", "")
                .Replace("-", "")
                .Replace(" ", "")
                .Replace("(", "")
                .Replace(")", "");
            return tmp.Length == 10;
        }

        public bool Contains(string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }

        public string ReplaceTokens(string tokenizedFormat, string replacementValue)
        {
            if (replacementValue.Contains(char.Parse(tokenKeys.ItemSep)))
            {
                var splitTokens = replacementValue.Split(char.Parse(tokenKeys.ItemSep));
                foreach (string currentToken in splitTokens)
                {
                    if (currentToken.Contains(char.Parse(tokenKeys.KVSep)))
                    {
                        var splitToken = currentToken.Split(char.Parse(tokenKeys.KVSep));
                        var tokenKey = "[" + splitToken[0] + "]";
                        var tokenValue = splitToken[1];

                        tokenizedFormat = tokenizedFormat.Replace(tokenKey, tokenValue);
                    }
                }
            }
            else if (replacementValue.Contains(char.Parse(tokenKeys.KVSep)))
            {
                var splitToken = replacementValue.Split(char.Parse(tokenKeys.KVSep));
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

        #endregion

        #region Client Management Methods

        public Tuple<bool, string> CheckClientIp(Client pClient)
        {
            var myIP = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (String.IsNullOrEmpty(myIP))
                myIP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            if (String.IsNullOrEmpty(pClient.AllowedIpList))
                return new Tuple<bool, string>(true, "No AllowedIpList");

            return CheckIpList(myIP, pClient.AllowedIpList);
        }

        public Tuple<bool, string> CheckIpList(string pIP, string pAllowedIpList)
        {

            if (pIP.Contains(":"))
                pIP = "127.0.0.1";
            else
                pIP = pIP.Trim();

            // check if allowed Ip configured, if not disabled
            var mIpList = pAllowedIpList.Split(char.Parse(dk.ItemSep));
            foreach (var mItem in mIpList)
            {
                // remove any leading and trailing spaces
                var mIpFromList = mItem.Trim();
                if (String.IsNullOrEmpty(mIpFromList)) continue;
                if (pIP == mIpFromList) return new Tuple<bool, string>(true, mIpFromList);
                // is this a range of Ips
                if (mIpFromList.Contains("-"))
                {
                    // islate the last octet of 
                    var x = mIpFromList.LastIndexOf('.') + 1;
                    var mStartofIP = mIpFromList.Substring(0, x - 1);
                    var mlastOc = mIpFromList.Substring(x, mIpFromList.Length - x);
                    if (mlastOc.Contains("-") == false)
                        return new Tuple<bool, string>(false, "Range error:" + mIpFromList);
                    var mRange = mlastOc.Split('-');
                    int y;
                    if (int.TryParse(mRange[0], out x) == false)
                        return new Tuple<bool, string>(false, "Range parseing error:" + mIpFromList);
                    if (int.TryParse(mRange[1], out y) == false)
                        return new Tuple<bool, string>(false, "Range parseing error:" + mIpFromList);
                    // check if valid range
                    for (; x < y+1; ++x)
                    {
                        var mIpToCheck = mStartofIP + "." + x.ToString();
                        if (mIpToCheck == pIP)
                            return new Tuple<bool, string>(true, mIpToCheck);
                    }
                }
            }
            return new Tuple<bool, string>(false, "No match!");
        }


        public Client ValidateClient(string clientId)
        {
            try
            {
                var myClient = new Client(clientId);
                if (myClient.Enabled)
                    return myClient;
            }
            catch
            {
                return null;
            }

            // No client entry or client is disabled
            return null;
        }

        public OasClientList GetOasClientByClientId(string clientId)
        {
            try
            {
                var mongoCollection = mongoDBConnectionPool.GetCollection("OasClientList");
                var query = Query.EQ("ClientId", ObjectId.Parse(clientId.Trim()));
                return mongoCollection.FindOneAs<OasClientList>(query);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region File and Path Handling Methods

        public void ProcessClientApiApp(Client myClient, bool bNewClient, string oldClientName)
        {
            var cleanedClientNamePath = SantizeDirectoryFilePath(myClient.Name, "-");

            var filePath = HttpContext.Current.Server.MapPath(".");

            filePath = filePath.Replace(@"MACAdmin\Clients", @"ClientApps");

            filePath = filePath.Replace(@"MACUserApps\Web\Tests", @"ClientApps");
            //MACUserApps\Web\Tests

            if (bNewClient)
            {
                myClient.ClientAppUrl = filePath + @"\" + cleanedClientNamePath;

                // Create a directory for the new Client
                Directory.CreateDirectory(myClient.ClientAppUrl);

                // Copy root html files and web.config
                var defaultsPath = filePath.Replace(@"ClientApps", @"!Defaults") + @"\ClientApps";

                var files = Directory.GetFiles(defaultsPath);
                foreach (var file in files)
                {
                    // Copy each file to the new client directory
                    var fileName = file.Split('\\');
                    var clientFileName = myClient.ClientAppUrl + @"\" + fileName[fileName.Length - 1];
                    File.Copy(file, clientFileName, true);

                    UpdatePlaceholderTokens(clientFileName, "[ClientNameToken]", myClient.Name);
                    UpdatePlaceholderTokens(clientFileName, "[ClientIDToken]", myClient._id.ToString());
                    UpdatePlaceholderTokens(clientFileName, "[RegistrationCompletionUrl]",
                        myClient.RegistrationCompletionUrl);
                }

                // Process subdirectories and files
                //var directories = Directory.GetDirectories(filePath + @"\!Defaults");
                var directories = Directory.GetDirectories(defaultsPath);
                foreach (var directory in directories)
                {
                    // Create each subdirectory
                    var tmpArray = directory.Split('\\');
                    var subDirectory = myClient.ClientAppUrl + @"\" + tmpArray[tmpArray.Length - 1];
                    Directory.CreateDirectory(subDirectory);

                    files = Directory.GetFiles(directory);
                    foreach (var file in files)
                    {
                        // Copy each file to the new client directory
                        var fileName = file.Split('\\');
                        var clientFileName = myClient.ClientAppUrl + @"\" + fileName[fileName.Length - 2] + @"\" +
                                                fileName[fileName.Length - 1];
                        File.Copy(file, clientFileName, true);

                        UpdatePlaceholderTokens(clientFileName, "[ClientNameToken]", myClient.Name);
                        UpdatePlaceholderTokens(clientFileName, "[ClientIDToken]", myClient._id.ToString());
                        UpdatePlaceholderTokens(clientFileName, "[RegistrationCompletionUrl]",
                            myClient.RegistrationCompletionUrl);
                    }
                }
            }
            else
            {
                // Update the Client API path and app
                var tmpArray = myClient.ClientAppUrl.Split('/');
                var tmpVal = tmpArray.Where(t => t != "").Aggregate("/", (current, t) => current + (t + "/"));

                var sourcePath = HttpContext.Current.Server.MapPath(tmpVal).Replace("\\MACAdmin\\Clients", "");

                var arrClientAppPath = sourcePath.Split('\\');
                var targetPath = "";

                for (var i = 0; i < arrClientAppPath.Length - 2; i++)
                {
                    targetPath += arrClientAppPath[i] + "\\";
                }

                targetPath += cleanedClientNamePath;
                targetPath = SantizeDirectoryFilePath(targetPath, "-");

                // Only move this directory if it's not our default mac client
                if (sourcePath.Contains("!Defaults") == false)
                {
                    // Update permissions for move operation
                    SetDirectoryAndFilePermissions(sourcePath);

                    // Move the current directory and files to complete "renaming" process
                    Directory.Move(sourcePath, targetPath);

                    // Update the html with the new client name
                    UpdatePlaceholderTokens(targetPath, cleanedClientNamePath, SantizeDirectoryFilePath(oldClientName, " "));
                }
            }

            // Update the path to url spec
            myClient.ClientAppUrl = "/clientapps/" + SantizeDirectoryFilePath(cleanedClientNamePath, "-");
        }

        public string UpdatePlaceholderTokens(string filePath, string placeholderTokenName, string replacementString)
        {
            return ReplaceTextInFile(filePath, placeholderTokenName, replacementString);
        }

        public void SetDirectoryAndFilePermissions(string currentDirectory)
        {
            // Recursively process subdirectories and files
            foreach (
                var currentWorkingDirectory in
                    Directory.GetDirectories(currentDirectory, "*.*", SearchOption.AllDirectories)
                        .Select(subdir => new DirectoryInfo(subdir)))
            {
                currentWorkingDirectory.Attributes = FileAttributes.Normal;
            }
        }

        public string ReplaceTextInFile(string filePath, string searchText, string replaceText)
        {
            var arrFilePath = filePath.Split('.');
            var fileExtension = arrFilePath[arrFilePath.Length - 1];

            try
            {
                switch (fileExtension)
                {
                    case "config":
                        // Do nothing. This is an image
                        break;
                    case "jpg":
                        // Do nothing. This is an image
                        break;
                    case "jpeg":
                        // Do nothing. This is an image
                        break;
                    case "png":
                        // Do nothing. This is an image
                        break;
                    default:
                        using (var reader = new StreamReader(filePath))
                        {
                            var content = reader.ReadToEnd();
                            reader.Close();

                            content = content.Replace(searchText, replaceText);

                            using (var writer = new StreamWriter(filePath))
                            {
                                writer.Write(content);
                                writer.Close();
                            }
                        }
                        break;
                }
                return "Operation succeeded";
            }
            catch (Exception ex)
            {
                var mDetails = "ReplaceTextInFile()" + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                return ex.Message;
            }
        }

        public string SantizeDirectoryFilePath(string input, string replacementCharacter)
        {
            var cleanedString = input;
            cleanedString = cleanedString.Replace("'", "");
            cleanedString = cleanedString.Replace(" ", replacementCharacter);
            cleanedString = cleanedString.Replace("&", "and");
            cleanedString = cleanedString.Replace("!", "");
            cleanedString = cleanedString.Replace("?", "");
            cleanedString = cleanedString.Replace("(", "");
            cleanedString = cleanedString.Replace(")", "");
            cleanedString = cleanedString.Replace("#", "");

            return cleanedString;
        }

        #endregion

        #region OTP Methods
        /// <summary> Create and send a message to end user </summary>
        public Tuple<bool, string> SendMessageToEndUser(Client pClient, Otp pOtp)
        {
            try //set Otp life time
            {
                pOtp.EndOfLife = DateTime.UtcNow.AddMinutes(pClient.OtpSettings.Timeout);
                // range check 2 to 30 minutes
                if (pClient.OtpSettings.Timeout < 1) // min ever is 2 minutes
                    pOtp.EndOfLife = DateTime.UtcNow.AddMinutes(3);
                else if (pClient.OtpSettings.Timeout > 30) //max ever is 30 minutes
                    pOtp.EndOfLife = DateTime.UtcNow.AddMinutes(3);
            }
            catch
            {
                pOtp.EndOfLife = DateTime.UtcNow.AddMinutes(3);
            }

            pOtp.ProviderTryList = pClient.OtpSettings.ProviderList;
            if (String.IsNullOrEmpty(pOtp.ProviderTryList))
            {
                // Log the failure and retry event
                var retryEvent = new Event
                {
                    ClientId = pClient.ClientId,
                    EventTypeDesc = Constants.TokenKeys.FailureDetails + "Empty provider list in OtpSettings"
                };
                retryEvent.Create(Constants.EventLog.Otp.ProviderFailedTryNextProvider, null);
                return new Tuple<bool, string>(false, retryEvent.EventTypeDesc);
            }

            while (String.IsNullOrEmpty(pOtp.ProviderTryList) == false)
            {
                var myProviderName = GetProviderNameFromList(pOtp);
                if (String.IsNullOrEmpty(myProviderName)) continue;

                // get LoopBack setting from config
                var mLoopBackState = ConfigurationManager.AppSettings[cfg.LoopBackTest];

                if ((myProviderName.Contains(cs.Sms)) || (myProviderName.Contains(cs.Voice)))
                {   //---------- Provider SMS or Voice ------------------
                    pOtp.DeliveryMethod = myProviderName.Contains(cs.Sms) ? cs.Sms : cs.Voice;

                    switch (pOtp.DeliveryMethod)
                    {
                        case cs.Sms:
                            pOtp.DeliveryMethodId = delmeth.DeliveryMethod.Sms.Item2;
                            break;

                        case cs.Voice:
                            pOtp.DeliveryMethodId = delmeth.DeliveryMethod.Voice.Item2;
                            break;
                    }


                    //========= Construct Message  ======================================
                    var rtn = ConstructMessage(pClient, pOtp, mLoopBackState);
                    if (rtn.Item1 == false) return rtn;
                    try
                    {   // Call service to send OTP message
                        pOtp.ProvidersName = myProviderName.
                                        Replace("(" + cs.Sms + ")", "").
                                        Replace("(" + cs.Voice + ")", "").Replace(" ", "");
                        var mMessageDeliveryServiceUrl = ConfigurationManager.AppSettings[pOtp.ProvidersName + "APIService"];
                        if (!String.IsNullOrEmpty(mMessageDeliveryServiceUrl))
                        {
                            var mData = new Dictionary<string, string>
                            {
                                {dk.CID, pClient.ClientId.ToString()},
                                {dk.Request, myProviderName},
                                {dkui.PhoneNumber, pOtp.ToPhone},
                                {dk.RequestId, pOtp._id.ToString()},
                                {dk.Message, StringToHex(pOtp.Message)}
                            };
                            if (String.IsNullOrEmpty(pOtp.UserId) == false)
                                mData.Add(dk.UserId, pOtp.UserId);
                            // Never use reply process of loopback an OTP being sent for Admin Login
                            if (pOtp.Name.Contains(dv.Admin) == false)
                            {
                                // including Reply Uri tells Provider service
                                // to use the OTP Reply process
                                if (String.IsNullOrEmpty(pOtp.ReplyUri) == false)
                                    mData.Add(dk.EnableReply, "True");
                                // If loopback set or user's name is QAUser,
                                // pass additional data to delivery service
                                if ((mLoopBackState != null) && (mLoopBackState != cfg.Disabled))
                                {
                                    mData.Add(dk.LoopBackTest, mLoopBackState);
                                    mData.Add(dk.Name, pOtp.Name); // Admin Login or Send
                                    mData.Add(dk.OTP, pOtp.Code);
                                } 
                                else if (!String.IsNullOrEmpty(pOtp.UserName))
                                {
                                    if (Security.DecodeAndDecrypt(pOtp.UserName, pOtp.UserId) == cs.QAUserFirstName)
                                    {
                                        // QAUser do not send OTP message
                                        mData.Add(dk.LoopBackTest, cfg.NoSend);
                                        mData.Add(dk.Name, pOtp.Name);
                                        mData.Add(dk.OTP, pOtp.Code);
                                    }
                                }
                            }
                            //===== Call the service ===========================================
                            rtn = ServiceRequest(mMessageDeliveryServiceUrl, pClient.ClientId.ToString(), mData);
                            pOtp.ProvidersReply += rtn.Item2 + " ";
                            if (rtn.Item1) // true message sent
                            {
// ReSharper disable once UnusedVariable
                                var myStat = new EventStat(pClient._id, pClient.Name, es.OtpSent + pOtp.DeliveryMethod, 1);
                                return rtn; // OTP successfully sent by service
                            }
                            // error returned from service
                            // if blocked because of user replied "Stop" return error
                            if (rtn.Item2.Contains(sr.STOP))
                                return rtn;

                            var retryEvent = new Event
                            {
                                ClientId = pClient.ClientId,
                                EventTypeDesc = Constants.TokenKeys.MessageProviderCurrent + myProviderName
                                                + Constants.TokenKeys.DeliveryMethodCurrent + pOtp.DeliveryMethod
                                                + Constants.TokenKeys.MessageProviderNext + pOtp.ProviderTryList
                                                + Constants.TokenKeys.DeliveryMethodNext + "Unknown"
                                                + Constants.TokenKeys.FailureDetails + pOtp.ProvidersReply
                            };
                            retryEvent.Create(Constants.EventLog.Otp.ProviderFailedTryNextProvider, null);
                        }
                        else
                        {
                            // Log the configuration error and try next message delivery service
                            var retryEvent = new Event
                            {
                                ClientId = pClient.ClientId,
                                EventTypeDesc = Constants.TokenKeys.MessageProviderCurrent + myProviderName
                                                + Constants.TokenKeys.DeliveryMethodCurrent + pOtp.DeliveryMethod
                                                + Constants.TokenKeys.MessageProviderNext + pOtp.ProviderTryList
                                                + Constants.TokenKeys.DeliveryMethodNext + "Unknown"
                                                + Constants.TokenKeys.FailureDetails + pOtp.ProvidersReply +
                                                ", No appSettings for " + mMessageDeliveryServiceUrl
                            };
                            retryEvent.Create(Constants.EventLog.Otp.ProviderFailedTryNextProvider, null);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Error log the error and try next provicer in provider list
                        var mDetails = "SendMessageToEndUser(Sms Voice) " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                        var exceptionEvent = new Event
                        {
                            EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                        };
                        exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
                    }
                }
                else if (myProviderName.Contains(cs.Email))
                {   //---------- ProviderEmail ------------------
                    pOtp.DeliveryMethod = cs.Email;
                    pOtp.DeliveryMethodId = delmeth.DeliveryMethod.Email.Item2;

                    //========= Construct Message  ======================================
                    var rtn = ConstructMessage(pClient, pOtp, mLoopBackState);
                    if (rtn.Item1 == false) return rtn; // error constructing message
                    try
                    {   // split subject and message formats
                        var mEmail = pOtp.Message.Split('~');
                        pOtp.ProvidersName = myProviderName.Replace("(" + cs.Email + ")", "").Trim();
                        foreach (var myProvider in pClient.MessageProviders.EmailProviders)
                        {
                            if (myProvider.Name != pOtp.ProvidersName) continue;
                            if (!myProvider.Enabled) continue;
                            var toEmail = Security.DecodeAndDecrypt(pOtp.ToEmail, pOtp.UserId);

                            var mLoopback = ConfigurationManager.AppSettings[cfg.LoopBackTest];
                            if (!String.IsNullOrEmpty(mLoopback))
                            {
                                if (mLoopback != cfg.Disabled)
                                    return new Tuple<bool, string>(true, "Loopback set to " + mLoopback);
                            }

                            //var emailTemplate = GetEmailTemplate("", "");

                            // Not loopback, send otp via email
                            rtn = SendEmail(myProvider,
                                /*ToAddress*/toEmail,
                                /*Subject*/mEmail[0],
                                /*Body*/ mEmail[1],
                                /*Not registration*/ null);

                            pOtp.ProvidersReply += rtn.Item2 + " ";
                            // on error try next provider in try list
                            if (!rtn.Item1) continue;

                            // on success, return to caller
                            if (rtn.Item1) return rtn;

                            // Log the failure and retry event
                            var retryEvent = new Event
                            {
                                ClientId = pClient.ClientId,
                                EventTypeDesc = Constants.TokenKeys.MessageProviderCurrent + myProviderName
                                                + Constants.TokenKeys.DeliveryMethodCurrent + pOtp.DeliveryMethod
                                                + Constants.TokenKeys.MessageProviderNext + pOtp.ProviderTryList
                                                + Constants.TokenKeys.DeliveryMethodNext + "Unknown"
                                                + Constants.TokenKeys.FailureDetails + pOtp.ProvidersReply
                            };
                            retryEvent.Create(Constants.EventLog.Otp.ProviderFailedTryNextProvider, null);
                        }
                    }
                    catch (Exception ex)
                    {
                        var mDetails = "SendMessageToEndUser(email) " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                        var exceptionEvent = new Event
                        {
                            EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                        };
                        exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
                    }
                }
                else
                {
                    var exceptionEvent = new Event();
                    exceptionEvent.Create(Constants.EventLog.Exceptions.General, "Invalid Provider[" + myProviderName + "] ");
                }

            } // end while
            const string err1 = "All providers failed to send message";
            // Log the failure and retry event
            var retryEvent2 = new Event
            {
                ClientId = pClient.ClientId,
                EventTypeDesc = Constants.TokenKeys.MessageProviderCurrent + "Unknown"
                                + Constants.TokenKeys.DeliveryMethodCurrent + pOtp.DeliveryMethod
                                + Constants.TokenKeys.MessageProviderNext + "Unknown"
                                + Constants.TokenKeys.DeliveryMethodNext + "Unknown"
                                + Constants.TokenKeys.FailureDetails + pOtp.ProvidersReply
            };
            retryEvent2.Create(Constants.EventLog.Otp.ProviderFailedTryNextProvider, null);

            return new Tuple<bool, string>(false, err1);
        }

        /// <summary> Generate an OTP based on the Client's OTP Settings</summary>
        public string GenerateOtpCode(Client pClient)
        {
            var chars = pClient.OtpSettings.Characterset;
            var otpCodeLength = pClient.OtpSettings.Length;
            var random = new Random();
            var otpCode = new string(
                Enumerable.Repeat(chars, otpCodeLength)
                    .Select(s =>
                        s[random.Next(s.Length)])
                    .ToArray());
            return otpCode;
        }

        /// <summary> Get the first provider's name from the provider list in th OTP Object</summary>
        private string GetProviderNameFromList(Otp pOtp)
        {
            try
            {
                var providers = pOtp.ProviderTryList.Trim(char.Parse(dk.ItemSep)).Split(char.Parse(dk.ItemSep));
                if (!providers.Any())
                {
                    return pOtp.ProviderTryList = string.Empty;
                }
                var providerToTry = providers[0];
                // remove the provider name from the start of the retry list
                var len = providerToTry.Length;
                pOtp.ProviderTryList = pOtp.ProviderTryList.Remove(0, len);
                pOtp.ProviderTryList = pOtp.ProviderTryList.TrimStart(char.Parse(dk.ItemSep));
                return providerToTry;
            }
            catch
            {
                return String.Empty;
            }
        }

        /// <summary> Construct the message based on the template </summary>
        public Tuple<bool, string> ConstructMessage(Client pClient, Otp pOtp, string pLoopBackState)
        {
            // is this a resend
            if (pOtp.TrxType == 99)
            {   //yes, do not reconstruct the message
                return new Tuple<bool, string>(true, pOtp.Message);
            }
            var mDocumentTemplate =
                GetDocumentTemplateByDeliveryMethodAndType(pClient, pOtp.DeliveryMethod, pOtp.TrxType.ToString());
            if (mDocumentTemplate == null)
                return new Tuple<bool, string>(false, "No document template for " + pOtp.DeliveryMethod + "[" + pOtp.TrxType.ToString() + "]");

            // Start the message from the template
            pOtp.Message = mDocumentTemplate.MessageFormat;

            // does the template specify a place for the client's name
            if (pClient.Name == cs.DefaultClientName)
                pOtp.Message = pOtp.Message.Replace(dtr.ClientName, "System");
            else
                pOtp.Message = pOtp.Message.Replace(dtr.ClientName, pClient.Name);

            // does the template specify a place for the End User's first name
            if (String.IsNullOrEmpty(pOtp.UserName))
            {
                pOtp.Message = pOtp.Message.Replace(dtr.FirstName, "Valued User");
            }
            else
            {
                pOtp.Message = pOtp.Message.Replace(dtr.FirstName,
                    Security.DecodeAndDecrypt(pOtp.UserName, pOtp.UserId));
            }

            // does the template specify a place for an OTP
            if (pOtp.Message.Contains(dtr.OTP))
                pOtp.Message = pOtp.Message.Replace(dtr.OTP, pOtp.Code = GenerateOtpCode(pClient));

            // does the template specify a place for the message details
            if (pOtp.Message.Contains(dtr.DETAILS))
            {
                // are there details to include
                pOtp.Message = !String.IsNullOrEmpty(pOtp.TrxDetail) ? pOtp.Message.Replace(dtr.DETAILS, pOtp.TrxDetail) : pOtp.Message.Replace(dtr.NL + dtr.DETAILS, "");
            }
            // does the template specify a place for an Ad
            if (pOtp.Message.Contains(dtr.AD))
            {
                // does the client configured to send Ads
                // and does the user get Ads
                if (pClient.AdEnabled)
                {
                    if (pOtp.UserOtpOutAd)
                    {   // no ad for message
                        pOtp.Message = pOtp.Message.Replace(dtr.NL + dtr.AD, "").Replace(dtr.AD, "");
                        pOtp.AdDetails.Status += " User Opt-out set.";
                    }
                    else
                    {
                        GetAds(pClient, pOtp);
                        // Delivery method is SMS
                        if (pOtp.DeliveryMethod == cs.Sms)
                        {
                            // if this is the 1st text message being sent to this user 
                            // or the 1st text message sent after a STOP/OPTIN
                            // don't send the ad link
                            if (pOtp.FirstTimeCarrierInfoSent == false)
                            {
                                pOtp.AdDetails.MessageAd = null;
                                pOtp.AdDetails.Status += "First message to user, no ad.";
                            }
                        }

                        if (String.IsNullOrEmpty(pOtp.AdDetails.MessageAd))
                        {
                            pOtp.Message = pOtp.Message.Replace(dtr.NL + dtr.AD, "").Replace(dtr.AD, "");
                            if (pOtp.FirstTimeCarrierInfoSent)
                                pOtp.AdDetails.Status += "No ad for message.";
                        }
                        else
                        {
                            pOtp.Message = pOtp.Message.Replace(dtr.AD, pOtp.AdDetails.MessageAd);
                            pOtp.AdDetails.Status += "Ad included in message.";
                        }
                    }
                }
                else
                {
                    pOtp.Message = pOtp.Message.Replace(dtr.NL + dtr.AD, "").Replace(dtr.AD, "");
                    pOtp.AdDetails.Status += "Client not enabled.";
                }
            }
            // save the completed message in the OTP Object
            return new Tuple<bool, string>(true, pOtp.Message);
        }

        /// <summary>Get Message Template from the Client Object </summary>
        public DocumentTemplate GetDocumentTemplateByDeliveryMethodAndType(Client pClient, string pDeliveryMethod, string pType)
        {
            DocumentTemplate mDocumentTemplate = null;
            foreach (var mTemplate in pClient.DocumentTemplates)
            {
                if (mTemplate.MessageClass.Contains(pDeliveryMethod))
                {
                    // ReSharper disable once SpecifyACultureInStringConversionExplicitly
                    if (mTemplate.MessageClass.EndsWith(pType))
                    {
                        return mTemplate;
                    }
                    if (mTemplate.MessageClass.EndsWith("0"))
                    {
                        mDocumentTemplate = mTemplate;
                    }
                }
            }
            return mDocumentTemplate;
        }

        public void UpdateOtpHistory(Otp myOtp, Event _mOtpVerifyEvent, string pServiceName)
        {
            try
            {
                var otpQuery = Query.EQ("_id", myOtp._id);
                var otpSortBy = SortBy.Ascending("_id");

                var bsonUpdate = _mOtpVerifyEvent.ToBsonDocument();
                var otpUpdate = Update.AddToSet("CodeHistory", bsonUpdate)
                    .Set("Active", myOtp.Active)
                    .Set("ValidationCount", myOtp.ValidationCount);

                var MyDB = (MongoDatabase)HttpContext.Current.Application[cs.MongoDB];
                MongoCollection otpCollection = MyDB.GetCollection("Otp");

                // ReSharper disable once UnusedVariable
                var otpUpdateStatus = otpCollection.FindAndModify(otpQuery, otpSortBy, otpUpdate, true);
            }
            catch (Exception ex)
            {
                var mDetails = pServiceName + "." + dv.VerifyOtp + " " +
                               ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";");
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
            }
        }
        #endregion

        #region AdPass Methods

        public void SetAdSelectionSpecifics(Client pClient, Otp pOtp, Dictionary<String, String> pData)
        {
            if (pData.ContainsKey(dk.Ads.AdNumber))
                pOtp.AdDetails.AdNumber = pData[dk.Ads.AdNumber];
            else
            {
                if (pData.ContainsKey(dk.TrxType))
                    pOtp.AdDetails.AdNumber = pData[dk.TrxType].ToString();
            }
            if (pData.ContainsKey(dk.Ads.Age))
                pOtp.AdDetails.Age = pData[dk.Ads.Age];
            if (pData.ContainsKey(dk.Ads.City))
                pOtp.AdDetails.City = pData[dk.Ads.City];
            if (pData.ContainsKey(dk.Ads.Ethnicity))
                pOtp.AdDetails.Ethnicity = pData[dk.Ads.Ethnicity];
            if (pData.ContainsKey(dk.Ads.Gender))
                pOtp.AdDetails.Gender = pData[dk.Ads.Gender];
            if (pData.ContainsKey(dk.Ads.Homeowner))
                pOtp.AdDetails.Homeowner = pData[dk.Ads.Homeowner];
            if (pData.ContainsKey(dk.Ads.HouseholdIncome))
                pOtp.AdDetails.HouseholdIncome = pData[dk.Ads.HouseholdIncome];
            if (pData.ContainsKey(dk.Ads.MaritalStatus))
                pOtp.AdDetails.MaritalStatus = pData[dk.Ads.MaritalStatus];
            if (pData.ContainsKey(dk.Ads.SpecificKeywords))
                pOtp.AdDetails.SpecificKeywords = pData[dk.Ads.SpecificKeywords];
            if (pData.ContainsKey(dk.Ads.State))
                pOtp.AdDetails.State = pData[dk.Ads.State];
            if (pData.ContainsKey(dk.Ads.Type))
                pOtp.AdDetails.Type = pData[dk.Ads.Type];
        }

        /// <summary> Call the Secure Ads server and process the response </summary>
        public void GetAds(Client pClient, Otp pOtp)
        {
            if (pClient.AdProviders.Count == 0)
            {
                pOtp.AdDetails.Status += "No AdProviders configured for this Client.";
                return;
            }
            var mAdProvider = pClient.AdProviders[0];
            if (mAdProvider.Name == "SecureAds")
                GetAdsFromSecureAds(pClient, pOtp);
            else
                GetAdFromTestAdService(pClient, pOtp);
        }

        #region Secure Ads
        protected void GetAdsFromSecureAds(Client pClient, Otp pOtp)
        {
            // get log settings from config
            var mServiceLogSettings = ConfigurationManager.AppSettings[cfg.DebugLogRequests];

            var mAdProvider = pClient.AdProviders[0];
            var mError = String.Empty;
            if (String.IsNullOrEmpty(mAdProvider.AdClientId))
                mError += "AdClientId, ";
            if (String.IsNullOrEmpty(mAdProvider.ApiUrl))
                mError += "ApiUrl, ";
            if (String.IsNullOrEmpty(mAdProvider.UserName))
                mError += "UserName, ";
            if (String.IsNullOrEmpty(mAdProvider.Password))
                mError += "Password, ";
            if (String.IsNullOrEmpty(mError) == false)
            {
                pOtp.AdDetails.Status += " AdProvider Config error no[" + mError + "], ";
                return;
            }

            // Get Ads from Secure Ads Server
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // record what was used for the ad request
            pOtp.AdDetails.AdServerUrl = mAdProvider.ApiUrl;
            pOtp.AdDetails.AdClientId = mAdProvider.AdClientId;
            try
            {
                //Assume clientid only request
                var request = new GetAdRequest { ClientId = mAdProvider.AdClientId };

                // Any keywords, use keywords
                if (!String.IsNullOrEmpty(pOtp.AdDetails.SpecificKeywords))
                    // use keyword request
                    request.CampaignProfiling = new CampaignProfiling { Keywords = pOtp.AdDetails.SpecificKeywords };
                else
                {
                    // Any profile data passed in Request OTP
                    var mProfile = GetAdProfileForSecureAds(pOtp);
                    if (String.IsNullOrEmpty(mProfile))
                    {
                        // use profile request
                        //Todo: add filter profile to following request
                        request = new GetAdRequest { ClientId = mAdProvider.AdClientId };
                    }
                }

                // Request Ads from secure ads
                pOtp.AdDetails.Status += @"GetAdsFromSecureAds.Request";
                var apiAuthen = new ApiAuthen
                {
                    ApiKey = mAdProvider.ApiKey,
                    UserName = mAdProvider.UserName,
                    Password = mAdProvider.Password
                };
                if (mServiceLogSettings.Contains("SA") && (mServiceLogSettings.Contains("API")))
                {
                    var mEventRequest = new Event
                    {
                        EventTypeName = mAdProvider.Name + "request",
                        EventTypeDesc = "ApiUrl[" + mAdProvider.ApiUrl + "]..." +
                        "ApiKey=" + mAdProvider.ApiKey + "," +
                        "UserName=" + mAdProvider.UserName + "," +
                        "Password=" + mAdProvider.Password + "," +
                        "request=" + request
                    };
                    mEventRequest.Create();
                }
                // Get response from Secure Ads
                var adresponse = ExecuteService<IAd, GetAdResponse>(mAdProvider.ApiUrl, service => service.GetAd(apiAuthen, request));


                // Check Response
                if (!adresponse.Success)
                {
                    // error response, no ads
                    var errors = adresponse.Errors;
                    foreach (var err in errors)
                        pOtp.AdDetails.Status += err + @", ";

                    if (mServiceLogSettings.Contains("SA") && (mServiceLogSettings.Contains("API")))
                    {
                        var mEventRequest = new Event
                        {
                            EventTypeName = mAdProvider.Name + "response",
                            EventTypeDesc = "error: " + pOtp.AdDetails.Status
                        };
                        mEventRequest.Create();
                    }
                    return;
                }

                // Not error response, Get ads that where returned
                if (adresponse.MobileMesasge != null)
                {
                    pOtp.AdDetails.MessageAd = adresponse.MobileMesasge.Message;
                    if (String.IsNullOrEmpty(pOtp.AdDetails.MessageAd))
                        pOtp.AdDetails.Status += @", Empty message ad returned!";
                    else
                    {
                        // ReSharper disable once UnusedVariable
                        var adSent = new EventStat(pClient._id, pClient.Name, es.AdMessageSent, 1);
                    }
                }
                else
                {
                    pOtp.AdDetails.Status += @", No message ad returned!";
                }
                if (adresponse.EnterOtpScreen != null)
                {
                    //pOtp.AdDetails.EnterOTPAd = (adresponse.EnterOtpScreen.ContentHtml).Replace(@"\", "/").Trim();
                    //These values (VerificationAd and EnterOTPAd) are reversed by Secure Ads.
                    //pOtp.AdDetails.VerificationAd = (adresponse.VerificationPage.ContentHtml).Trim();
                    //pOtp.AdDetails.EnterOTPAd = (adresponse.EnterOtpScreen.ContentHtml).Trim();
                    pOtp.AdDetails.VerificationAd = (adresponse.EnterOtpScreen.ContentHtml).Replace(@"\", "/").Trim();
                    
                    
                    // ReSharper disable once UnusedVariable
                    var adSent = new EventStat(pClient._id, pClient.Name, es.AdEnterOtpScreenSent, 1);
                }
                else
                    pOtp.AdDetails.Status += @", No EnterOTPAd ad returned!";

                if (adresponse.VerificationPage != null)
                {
                    //pOtp.AdDetails.VerificationAd = (adresponse.VerificationPage.ContentHtml).Replace(@"\", "/").Trim();
                    //These values (VerificationAd and EnterOTPAd) are reversed by Secure Ads.
                    //pOtp.AdDetails.EnterOTPAd = (adresponse.EnterOtpScreen.ContentHtml).Trim();
                    //pOtp.AdDetails.VerificationAd = (adresponse.VerificationPage.ContentHtml).Trim();
                    pOtp.AdDetails.EnterOTPAd = (adresponse.VerificationPage.ContentHtml).Replace(@"\", "/").Replace(Environment.NewLine,"").Trim();
                    var hexValue = StringToHex(pOtp.AdDetails.EnterOTPAd);
                    int count = hexValue.Length;
                    var stringValue = HexToString(hexValue);
                    
                    // ReSharper disable once UnusedVariable
                    var adSent = new EventStat(pClient._id, pClient.Name, es.AdVerificationScreenSent, 1);
                }
                else
                    pOtp.AdDetails.Status += @", No VerificationAd ad returned!";

                stopwatch.Stop();
                // record the response time
                pOtp.AdDetails.ResponseTime = stopwatch.Elapsed;

                if (mServiceLogSettings.Contains("SA") && (mServiceLogSettings.Contains("API")))
                {
                    var mEventRequest = new Event
                    {
                        EventTypeName = mAdProvider.Name + "response",
                        EventTypeDesc = "Success: " + pOtp.AdDetails.Status
                        + "MsgAd=" + pOtp.AdDetails.MessageAd
                        + "EnterOTPAd=" + StringToHex(pOtp.AdDetails.EnterOTPAd)
                        + "VerAd" + StringToHex(pOtp.AdDetails.VerificationAd)
                    };
                    mEventRequest.Create();
                }
            }
            catch (Exception ex)
            {
                pOtp.AdDetails.Status += @", Exception:" + ex.Message;
                pOtp.AdDetails.MessageAd = pOtp.AdDetails.EnterOTPAd = String.Empty;
                stopwatch.Stop();
                pOtp.AdDetails.ResponseTime = stopwatch.Elapsed;
                var mDetails = "GetAdForOtp.GetAdsFromSecureAds() " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";");
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                exceptionEvent.EventTypeDesc += Constants.TokenKeys.ClientName + pClient.Name;
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
            }
        }

        protected TResult ExecuteService<TService, TResult>(string url, Func<TService, TResult> funcExec)
        {
            var mAdServerRequestType = ConfigurationManager.AppSettings["AdServerRequestType"];
            if (mAdServerRequestType == null || mAdServerRequestType.ToString() == "http")
            {
                var myBinding = new BasicHttpBinding();
                var myEndpoint = new EndpointAddress("http://" + url); 
                using (var myChannelFactory = new ChannelFactory<TService>(myBinding, myEndpoint))
                {
                    var client = myChannelFactory.CreateChannel();

                    return funcExec(client);
                }
            }
            else
            {
                var myBinding = new BasicHttpsBinding();
                var myEndpoint = new EndpointAddress("https://" + url);
                using (var myChannelFactory = new ChannelFactory<TService>(myBinding, myEndpoint))
                {
                    var client = myChannelFactory.CreateChannel();

                    return funcExec(client);
                }
            }
        }

        protected string GetAdProfileForSecureAds(Otp pOtp)
        {
            const string sep = ", ";
            const string eq = "=";
            var mProfileData = "Profile";
            if (!String.IsNullOrEmpty(pOtp.AdDetails.AdNumber))
                mProfileData += sep + dk.Ads.AdNumber + eq + pOtp.AdDetails.AdNumber;
            if (!String.IsNullOrEmpty(pOtp.AdDetails.Age))
                mProfileData += sep + dk.Ads.Age + eq + pOtp.AdDetails.Age;
            if (!String.IsNullOrEmpty(pOtp.AdDetails.City))
                mProfileData += sep + dk.Ads.City + eq + pOtp.AdDetails.City;
            if (!String.IsNullOrEmpty(pOtp.AdDetails.Ethnicity))
                mProfileData += sep + dk.Ads.Ethnicity + eq + pOtp.AdDetails.Ethnicity;
            if (!String.IsNullOrEmpty(pOtp.AdDetails.Gender))
                mProfileData += sep + dk.Ads.Gender + eq + pOtp.AdDetails.Gender;
            if (!String.IsNullOrEmpty(pOtp.AdDetails.Homeowner))
                mProfileData += sep + dk.Ads.Homeowner + eq + pOtp.AdDetails.Homeowner;
            if (!String.IsNullOrEmpty(pOtp.AdDetails.HouseholdIncome))
                mProfileData += sep + dk.Ads.HouseholdIncome + eq + pOtp.AdDetails.HouseholdIncome;
            if (!String.IsNullOrEmpty(pOtp.AdDetails.MaritalStatus))
                mProfileData += sep + dk.Ads.MaritalStatus + eq + pOtp.AdDetails.MaritalStatus;
            if (!String.IsNullOrEmpty(pOtp.AdDetails.State))
                mProfileData += sep + dk.Ads.State + eq + pOtp.AdDetails.State;
            return mProfileData;
        }

        #endregion

        #region Test Ad Service

        public void GetAdFromTestAdService(Client pClient, Otp pOtp)
        {
            pOtp.AdDetails.Status += @"GetAdFromTestAdService(Request), ";
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            pOtp.AdDetails.AdServerUrl = ConfigurationManager.AppSettings[cfg.MacServicesUrl] +
                            Constants.ServiceUrls.MacTestAdService;
            var mRequestData = dk.Request + dk.KVSep + "GetAd" +
                               dk.ItemSep + dk.ClientName + dk.KVSep + pClient.Name +
                               getAdSelectionDetailsFromOtpForTestServer(pOtp);
            // send to Mac Test Ad service
            try
            {
                var dataStream =
                    Encoding.UTF8.GetBytes("data=99" + cs.DefaultClientId.Length + cs.DefaultClientId.ToUpper() +
                                           StringToHex(mRequestData));
                var mRequest = pOtp.AdDetails.AdServerUrl;
                var webRequest = WebRequest.Create(mRequest);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.ContentLength = dataStream.Length;
                var newStream = webRequest.GetRequestStream();
                // Send the data.
                newStream.Write(dataStream, 0, dataStream.Length);
                newStream.Close();
                var res = webRequest.GetResponse();
                var response = res.GetResponseStream();
                var xmlDoc = new XmlDocument();
                if (response != null) xmlDoc.Load(response);

                var elemList = xmlDoc.GetElementsByTagName(sr.Error);
                if (elemList.Count != 0)
                {
                    stopwatch.Stop();
                    pOtp.AdDetails.Status += ", " + elemList[0].InnerXml;
                    pOtp.AdDetails.MessageAd = pOtp.AdDetails.EnterOTPAd = String.Empty;
                    return;
                }
                elemList = xmlDoc.GetElementsByTagName(cs.AdMessage);
                if (elemList.Count != 0)
                {
                    pOtp.AdDetails.MessageAd = HexToString(elemList[0].InnerXml);
                    // ReSharper disable once UnusedVariable
                    var adSent = new EventStat(pClient._id, pClient.Name, es.AdMessageSent, 1);
                }

                elemList = xmlDoc.GetElementsByTagName(cs.AdEnterOtp);
                if (elemList.Count != 0)
                {
                    pOtp.AdDetails.EnterOTPAd = HexToString(elemList[0].InnerXml);
                    // ReSharper disable once UnusedVariable
                    var adSent = new EventStat(pClient._id, pClient.Name, es.AdEnterOtpScreenSent, 1);
                }

                elemList = xmlDoc.GetElementsByTagName(cs.AdVerification);
                if (elemList.Count != 0)
                {
                    pOtp.AdDetails.VerificationAd = HexToString(elemList[0].InnerXml);
                    // ReSharper disable once UnusedVariable
                    var adSent = new EventStat(pClient._id, pClient.Name, es.AdVerificationScreenSent, 1);
                }

                stopwatch.Stop();
                // record the response time
                pOtp.AdDetails.ResponseTime = stopwatch.Elapsed;
            }
            catch (Exception ex)
            {
                pOtp.AdDetails.Status += @"Exception:" + ex.Message;
                pOtp.AdDetails.MessageAd = pOtp.AdDetails.EnterOTPAd = String.Empty;
                stopwatch.Stop();
                pOtp.AdDetails.ResponseTime = stopwatch.Elapsed;
                var mDetails = "GetAdForOtp.GetAdFromTestAdService() " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";");
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                exceptionEvent.EventTypeDesc += Constants.TokenKeys.ClientName + pClient.Name;
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
            }
        }

        private string getAdSelectionDetailsFromOtpForTestServer(Otp pOtp)
        {
            var mAdSelectionDetails = "";
            if (!String.IsNullOrEmpty(pOtp.AdDetails.AdNumber))
                mAdSelectionDetails += dk.ItemSep + dk.Ads.AdNumber + dk.KVSep + pOtp.AdDetails.AdNumber;
            if (!String.IsNullOrEmpty(pOtp.AdDetails.Age))
                mAdSelectionDetails += dk.ItemSep + dk.Ads.Age + dk.KVSep + pOtp.AdDetails.Age;
            if (!String.IsNullOrEmpty(pOtp.AdDetails.City))
                mAdSelectionDetails += dk.ItemSep + dk.Ads.City + dk.KVSep + pOtp.AdDetails.City;
            if (!String.IsNullOrEmpty(pOtp.AdDetails.Ethnicity))
                mAdSelectionDetails += dk.ItemSep + dk.Ads.Ethnicity + dk.KVSep + pOtp.AdDetails.Ethnicity;
            if (!String.IsNullOrEmpty(pOtp.AdDetails.Gender))
                mAdSelectionDetails += dk.ItemSep + dk.Ads.Gender + dk.KVSep + pOtp.AdDetails.Gender;
            if (!String.IsNullOrEmpty(pOtp.AdDetails.Homeowner))
                mAdSelectionDetails += dk.ItemSep + dk.Ads.Homeowner + dk.KVSep + pOtp.AdDetails.Homeowner;
            if (!String.IsNullOrEmpty(pOtp.AdDetails.HouseholdIncome))
                mAdSelectionDetails += dk.ItemSep + dk.Ads.HouseholdIncome + dk.KVSep + pOtp.AdDetails.HouseholdIncome;
            if (!String.IsNullOrEmpty(pOtp.AdDetails.MaritalStatus))
                mAdSelectionDetails += dk.ItemSep + dk.Ads.MaritalStatus + dk.KVSep + pOtp.AdDetails.MaritalStatus;
            if (!String.IsNullOrEmpty(pOtp.AdDetails.SpecificKeywords))
                mAdSelectionDetails += dk.ItemSep + dk.Ads.SpecificKeywords + dk.KVSep + pOtp.AdDetails.SpecificKeywords;
            if (!String.IsNullOrEmpty(pOtp.AdDetails.State))
                mAdSelectionDetails += dk.ItemSep + dk.Ads.State + dk.KVSep + pOtp.AdDetails.State;
            if (!String.IsNullOrEmpty(pOtp.AdDetails.Type))
                mAdSelectionDetails += dk.ItemSep + dk.Ads.Type + dk.KVSep + pOtp.AdDetails.Type;
            return mAdSelectionDetails;
        }

        #endregion

        #endregion

        #region Data handling

        public Tuple<String, String> GetIdDataFromRequest(string pData)
        {
            if (String.IsNullOrEmpty(pData))
                return new Tuple<String, String>(String.Empty, "Utils.GetIdDataFromRequest: No data!");
            try
            {
                // expected format: length of id(2 characters), followed by the ID followed by the data
                // get length of ID
                int keyLength;
                if (int.TryParse(pData.Substring(0, 2), out keyLength) == false)
                    return new Tuple<String, String>(String.Empty, "Utils.GetIdDataFromRequest: Corrupt or bad request data!");
                // isolate ID
                var id = pData.Substring(2, keyLength);
                var skip = id.Length + 2;
                // isolate data
                var data = pData.Substring(skip, pData.Length - skip);
                return new Tuple<String, String>(id, data);
            }
            catch
            {
                return new Tuple<String, String>(String.Empty, "Utils.GetIdDataFromRequest: Corrupt or bad request data!");
            }
        }

        public bool ParseIntoDictionary(string pData, Dictionary<string, string> pDictionary, char pSubSplitChar)
        {
            // parse string into dictionary: format Pipe(|) delimited fields
            // fields are Subsplit character dlimited key value pairs
            // Example: input string = "Field1=ValField3=Value3" Subsplit character = '='
            //          output = Dictionary<string, string> {"Field1","Value1","Field2","Value2","Field3","Value2"}
            try
            {
                // Parse data
                var ui = pData.Split(char.Parse(dk.ItemSep));
                // Parse into dictionary
                foreach (var field in ui)
                {
                    if (field.Contains(pSubSplitChar) == false) return false;
                    var keyValue = field.Split(pSubSplitChar);
                    var key = keyValue[0];
                    if (String.IsNullOrEmpty(key)) return false;
                    var value = field.Replace(key + pSubSplitChar, "");
                    //if (keyValue[1].Length > 0)
                    //    value = keyValue[1];
                    pDictionary.Remove(key);
                    pDictionary.Add(key, value);
                }
                return true;
            }
            catch (Exception ex)
            {
                var mDetails = "ParseIntoDictionary(); " + StringToHex(pData) + " ; " 
                    + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
                return false;
            }
        }

        public bool DecryptAndParseRequestData(string pClientId, string prequestData,
            Dictionary<string, string> pDictionary, char pSubSplitChar)
        {
            try
            {
                // first decrypt data
                var myrequestData = Security.DecodeAndDecrypt(prequestData, pClientId);
                // then parse into dictionary
                return ParseIntoDictionary(myrequestData, pDictionary, pSubSplitChar);
            }
            catch (Exception ex)
            {
                var mDetails = "DecryptAndParseRequestData() " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                pDictionary.Add("Exception", ex.Message);
                return false;
            }
        }

        public string HexToString(String pInput)
        { // data is encoded in hex, convert it back to a string
            if (String.IsNullOrEmpty(pInput)) return string.Empty;

            var hexInput = pInput.Trim();
            try
            {
                var sb = new StringBuilder();
                for (var i = 0; i < hexInput.Length; i += 2)
                {
                    var hs = hexInput.Substring(i, 2);
                    sb.Append(Convert.ToChar(Convert.ToUInt32(hs, 16)));
                }
                return sb.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        public string StringToHex(String input)
        {
            if (String.IsNullOrEmpty(input)) return string.Empty;
            try
            {
                var values = input.ToCharArray();
                var output = new StringBuilder();
                foreach (var value in values.Select(Convert.ToInt32))
                {
                    // This eliminates linebreak/carriage return issues
                    if (value < 32) continue;
                        if(value > 126) continue;
                    // Convert the decimal value to a hexadecimal value in string form. 
                    output.Append(String.Format("{0:X}", value));
                }
                return output.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        #endregion

        #region MongoDB pre-populate collections for system initialization

        public void CreateTypeDefinitions()
        {
            var oDefs = new TypeDefinitions();

            var filePath = new StringBuilder(HttpContext.Current.Server.MapPath("."));
            //todo move path to constants
            filePath.Append("\\MACServices\\ClassLibraries\\CoreDomain\\TypeDefinitionsText");

            var files = Directory.GetFiles(filePath.ToString(), "*.txt", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var arrFileType = file.Split('\\');
                var fileType = arrFileType[arrFileType.Length - 1].Replace(".txt", "");

                using (var reader = new StreamReader(file))
                {
                    string description;
                    while ((description = reader.ReadLine()) != null)
                    {
                        switch (fileType)
                        {
                            case "Countries":
                                mongoDBConnectionPool.GetCollection<Country>("TypeDefinitions").Insert(new Country(description));
                                break;

                            case "States":
                                mongoDBConnectionPool.GetCollection<State>("TypeDefinitions").Insert(new State(description));
                                break;

                            case "Registration":
                                mongoDBConnectionPool.GetCollection<RegistrationType>("TypeDefinitions")
                                    .Insert(new RegistrationType(description));
                                break;

                            case "VerificationProviders":
                                //mongoDBConnectionPool.GetCollection<VerificationProvider>("TypeDefinitions").Insert(new VerificationProvider(_description));
                                break;

                            //case "ProviderVoice":
                            //    mongoDBConnectionPool.GetCollection<ProviderVoice>("TypeDefinitions").Insert(new VerificationProvider(_description));
                            //    break;
                        }
                    }
                }
            }

            ObjectCreate(oDefs);
        }

        #endregion

        #region List box data

        public string GetEventTypes(string clientId, DateTime startDate, DateTime endDate)
        {
            var sbResponse = new StringBuilder();

            // Get distinct EventTypes for selection
            var eventCollection = mongoDBConnectionPool.GetCollection("Event");
            var eventTypeList = new List<string>();

            if (clientId != "" && clientId != Constants.Strings.DefaultEmptyObjectId)
            {
                var distinctEventTypesQuery = Query.And(
                    Query.GT("Date", startDate),
                    Query.LT("Date", endDate),
                    Query.EQ("ClientId", ObjectId.Parse(clientId.Trim()))
                    );

                var eventTypeResult = eventCollection.Distinct("EventTypeName", distinctEventTypesQuery);
                eventTypeList.AddRange(from et in eventTypeResult where et.ToString() != "BsonNull" && et.ToString() != "" select et.ToString());
            }
            else
            {
                var distinctEventTypesQuery = Query.And(
                    Query.GT("Date", startDate),
                    Query.LT("Date", endDate)
                    );

                var eventTypeResult = eventCollection.Distinct("EventTypeName", distinctEventTypesQuery);
                eventTypeList.AddRange(from et in eventTypeResult where et.ToString() != "BsonNull" && et.ToString() != "" select et.ToString());
            }

            eventTypeList.Sort();

            sbResponse.Append("<eventtypes>");
            foreach (var eventType in eventTypeList)
            {
                sbResponse.Append("<eventtype>");
                sbResponse.Append(eventType);
                sbResponse.Append("</eventtype>");
            }
            sbResponse.Append("</eventtypes>");

            return sbResponse.ToString();
        }

        public List<ListItem> GetStateListItems()
        {
            var mongoCollection = mongoDBConnectionPool.GetCollection("TypeDefinitions").FindAll();
            return
                mongoCollection.Select(doc => new ListItem { Value = doc[0].ToString(), Text = doc[1].ToString() })
                    .ToList();
        }

        #endregion

        #region EndUser Management

        /// <summary> </summary>
        public Tuple<bool, string> GetHashedIdBasedOnRegistrationType(Dictionary<string, string> pData)
        {
            string mHashedId;
            if (pData.ContainsKey(dk.RegistrationType) == false)
                return new Tuple<bool, string>(false, pData[dk.RegistrationType] + " Key not in Dictionary");
            switch (pData[dk.RegistrationType])
            {
                case dv.GroupRegister:
                    if (pData.ContainsKey(dk.GroupId) == false)
                    {
                        if (pData.ContainsKey(dk.GroupName) == false)
                            return new Tuple<bool, string>(false, "Request Data missing group id or group name");

                        var mGroup = GetGroupUsingGroupName(pData[dk.GroupName]);
                        if (mGroup == null)
                            return new Tuple<bool, string>(false, "Could not get group by name " + pData[dk.GroupName]);

                        pData.Add(dk.GroupId, mGroup.Name);
                    }
                    mHashedId = Security.GetHashString(pData[dk.UserId] + pData[dk.GroupId]);
                    break;
                case dv.ClientRegister:
                    mHashedId = Security.GetHashString(pData[dk.UserId] + pData[dk.CID]);
                    break;
                case dv.OpenRegister:
                    mHashedId = pData[dk.UserId];
                    break;
                default:
                    return new Tuple<bool, string>(false, "Invalid Registration Type " + pData[dk.RegistrationType]);
            }
            return new Tuple<bool, string>(true, mHashedId);
        }

        /// <summary> </summary>
        public EndUser GetEndUserByHashedUserId(string pHashedUserId)
        {
            if (String.IsNullOrEmpty(pHashedUserId)) return null;

            EndUser myEndUser;

            try
            {
                var query = Query.EQ("HashedUserId", pHashedUserId);
                var mongoCollection = mongoDBConnectionPool.GetCollection("EndUser");
                myEndUser = mongoCollection.FindOneAs<EndUser>(query);
            }
            catch
            {
                myEndUser = null;
            }
            return myEndUser;
        }

        public EndUser GetEndUserByHashedPhoneNumber(string pHashedPhoneNumber)
        {
            if (String.IsNullOrEmpty(pHashedPhoneNumber)) return null;

            EndUser myEndUser;

            try
            {
                var query = Query.EQ("Phone", pHashedPhoneNumber);
                var mongoCollection = mongoDBConnectionPool.GetCollection("EndUser");
                myEndUser = mongoCollection.FindOneAs<EndUser>(query);
            }
            catch
            {
                myEndUser = null;
            }
            return myEndUser;   
        }

        /// <summary>  </summary>
        public string DeleteEndUser(EndUser pEndUser)
        {
            if (pEndUser == null) return "EndUser object required";

            try
            {
                var query = Query.EQ("_id", pEndUser._id);
                var mongoCollection = mongoDBConnectionPool.GetCollection("EndUser");
                mongoCollection.Remove(query);
                return null;
            }
            catch (Exception ex)
            {
                var mDetails = "DeleteEndUser() " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                return ex.Message;
            }
        }

        /// <summary>  </summary>
        public Tuple<bool, string> Check4MininumEndUserInfo(Dictionary<string, string> pData)
        {
            var errorlist = string.Empty;
            if (!pData.ContainsKey(dk.Request))
                errorlist += "Request type required!";

            // required
            if (pData.ContainsKey(dkui.PhoneNumber) == false)
                errorlist += ", End User's Phone Number is required!";
            if (ValidatePhoneNumber(pData[dkui.PhoneNumber]) == false)
                errorlist += ", End User's Phone Number is invalid!";

            // required
            if (pData.ContainsKey(dkui.EmailAddress) == false)
                errorlist += ", End User's email is required!";
            if (ValidateEmailAddress(pData[dkui.EmailAddress]) == false)
                errorlist += ", End User's email is invalid!";

            // required
            if (pData.ContainsKey(dkui.FirstName) == false)
                errorlist += ", User's first name required!";

            // required
            if (pData.ContainsKey(dkui.LastName) == false)
                errorlist += ", User's last name required!";

            // if unique Id not supplied use email address
            if (!pData.ContainsKey(dkui.UID))
                pData.Add(dkui.UID, pData[dkui.EmailAddress]);

            if (String.IsNullOrEmpty(errorlist))
                return new Tuple<bool, string>(true, "");
            return new Tuple<bool, string>(false, errorlist);
        }

        public String PhoneJustNumbers(string pPhoneNumber)
        {
            return pPhoneNumber.Replace(".", "").Replace("-", "").Replace(" ", "").Replace("(", "").Replace(")", "").Replace("+", "");
        }

        /// <summary> </summary>
        public Tuple<bool, string> PopulateEndUserObject(EndUser pEndUser, Dictionary<string, string> pData)
        {
            try
            {
                if (pData[dk.RegistrationType] == dv.OpenRegister)
                {
                    pEndUser.HashedUserId = pData[dk.UserId]; //UserId computed in OpenEndUserService
                }
                else if (pData[dk.RegistrationType] == dv.GroupRegister)
                {
                    pEndUser.HashedUserId = Security.GetHashString(pData[dk.UserId] + pData[dk.GroupId]);
                    var mGroupRelationship = new Relationship
                    {
                        MemberType = "Group",
                        MemberHierarchy = "Member",
                        MemberId = ObjectId.Parse(pData[dk.GroupId].Trim())
                    };
                    pEndUser.Relationships.Add(mGroupRelationship);
                }
                else
                {
                    pEndUser.HashedUserId = Security.GetHashString(pData[dk.UserId] + pData[dk.CID]);
                }
                pEndUser.ClientId = ObjectId.Parse(pData[dk.CID].Trim());
                pEndUser.Phone = Security.EncryptAndEncode(PhoneJustNumbers(pData[dkui.PhoneNumber]), pEndUser.HashedUserId);
                pEndUser.PH = Security.GetHashString(PhoneJustNumbers(pData[dkui.PhoneNumber]));
                pEndUser.Email = Security.EncryptAndEncode(pData[dkui.EmailAddress], pEndUser.HashedUserId);
                pEndUser.EH = Security.GetHashString(pData[dkui.EmailAddress].ToLower());
                pEndUser.RegistrationType = pData[dk.Request];
                var mClientRelationship = new Relationship
                {
                    MemberType = "Client",
                    MemberHierarchy = "Member",
                    MemberId = pEndUser.ClientId
                };
                pEndUser.Relationships.Add(mClientRelationship);

                // check Ad opt-out option
                if (pData.ContainsKey(dk.AdPassOption))
                    if (pData[dk.AdPassOption] == dv.AdDisable)
                        pEndUser.OtpOutAd = true;

                if (pData.ContainsKey(dk.NotificationOption))
                    pEndUser.NotifyOpts = pData[dk.NotificationOption];

                pEndUser.FirstName = Security.EncryptAndEncode(pData[dkui.FirstName], pEndUser.HashedUserId);
                pEndUser.Name = pData[dkui.FirstName];

                if (pData.ContainsKey(dkui.MiddleName))
                {
                    pEndUser.MiddleName = Security.EncryptAndEncode(pData[dkui.MiddleName], pEndUser.HashedUserId);
                    pEndUser.Name = pEndUser.Name + " " + pData[dkui.MiddleName];
                }

                pEndUser.LastName = Security.EncryptAndEncode(pData[dkui.LastName], pEndUser.HashedUserId);
                pEndUser.Name = pEndUser.Name + " " + pData[dkui.LastName];

                if (pData.ContainsKey(dkui.Prefix))
                {
                    pEndUser.Prefix = Security.EncryptAndEncode(pData[dkui.Prefix], pEndUser.HashedUserId);
                    pEndUser.Name = pData[dkui.Prefix] + " " + pEndUser.Name;
                }

                if (pData.ContainsKey(dkui.Suffix))
                {
                    pEndUser.Suffix = Security.EncryptAndEncode(pData[dkui.Suffix], pEndUser.HashedUserId);
                    pEndUser.Name += " " + pData[dkui.Suffix];
                }

                pEndUser.Name = Security.EncryptAndEncode(pEndUser.Name.Trim(), pEndUser.HashedUserId);

                if (pData.ContainsKey(dkui.DOB))
                    pEndUser.DateOfBirth = pData[dkui.DOB];

                if (pData.ContainsKey(dkui.Street))
                    pEndUser.Address.Street1 = Security.EncryptAndEncode(pData[dkui.Street], pEndUser.HashedUserId);

                if (pData.ContainsKey(dkui.Street2))
                    pEndUser.Address.Street2 = pData[dkui.Street2];

                if (pData.ContainsKey(dkui.Unit))
                    pEndUser.Address.Unit = Security.EncryptAndEncode(pData[dkui.Unit], pEndUser.HashedUserId);

                if (pData.ContainsKey(dkui.City))
                    pEndUser.Address.City = Security.EncryptAndEncode(pData[dkui.City], pEndUser.HashedUserId);

                if (pData.ContainsKey(dkui.State))
                    pEndUser.Address.State = Security.EncryptAndEncode(pData[dkui.State], pEndUser.HashedUserId);

                if (pData.ContainsKey(dkui.ZipCode))
                    pEndUser.Address.Zipcode = Security.EncryptAndEncode(pData[dkui.ZipCode], pEndUser.HashedUserId);

                if (pData.ContainsKey(dkui.Country))
                    pEndUser.Address.Country = Security.EncryptAndEncode(pData[dkui.Country], pEndUser.HashedUserId);

                //if (pData.ContainsKey(dkui.DriverLic))
                //    pEndUser.DriverLic += dk.ItemSep + pData[dkui.DriverLic];

                //if (pData.ContainsKey(dkui.DriverLicSt))
                //    pEndUser.DriverLicSt += dk.ItemSep + pData[dkui.DriverLicSt];

                if (String.IsNullOrEmpty(pEndUser.DateOfBirth) == false)
                    pEndUser.DateOfBirth = Security.EncryptAndEncode(pEndUser.DateOfBirth, pEndUser.HashedUserId);

                // register end user into client database
                if (pData.ContainsKey(dk.RegistrationType))
                    pEndUser.RegistrationType = pData[dk.RegistrationType];

                return new Tuple<bool, string>(true, "End user object Populated");
            }
            catch (Exception ex)
            {
                return new Tuple<bool, string>(false, "Utils.PopulateEndUserObject: Exception " + ex.Message);
            }
        }

        #endregion

        #region Service Methods

        public bool IsDebugSet()
        {
            var mLogReq = ConfigurationManager.AppSettings[cfg.Debug];
            if (String.IsNullOrEmpty(mLogReq)) return false;
            return mLogReq.ToLower() == "false";
        }

        /// <summary> </summary>
        public string LogRequest(Dictionary<string, string> pData, string pRequestData, string pServiceCode)
        {
            if (String.IsNullOrEmpty(pServiceCode)) return "";
            var mServiceLogSettings = ConfigurationManager.AppSettings[cfg.DebugLogRequests];
            // not in config
            if (String.IsNullOrEmpty(mServiceLogSettings)) return "";
            // all disabled
            if (mServiceLogSettings == "false") return "";
            // All enabled
            if (mServiceLogSettings != "true")
            {   // Enabled by service code ?
                if (mServiceLogSettings.Contains(pServiceCode) == false) return "";
            }
            var mServiceName = Constants.Strings.NotSpecified;
            var mCid = cs.DefaultClientId;
            if (pData != null)
            {
                if (pData.ContainsKey(dk.ServiceName))
                {
                    mServiceName = pData[dk.ServiceName];
                    if (pData.ContainsKey(dk.Request))
                        mServiceName += "." + pData[dk.Request];
                }
                if (pData.ContainsKey(dk.CID))
                {
                    mCid = pData[dk.CID];
                }
            }
            var mEvent = new Event
            {
                EventTypeName = Constants.Strings.ServiceLog,
                //UserId = ObjectId.Parse(mUserId.Trim()),
                EventTypeDesc = mServiceName + ": " + Environment.NewLine + "Data=" + pRequestData + Environment.NewLine
            };
            try
            {
                mEvent.ClientId = ObjectId.Parse(mCid.Trim());
            }
            catch
            {
                mEvent.ClientId = ObjectId.Parse(cs.DefaultClientId);
            }

            if (HttpContext.Current != null)
            {
                mEvent.UserIpAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (String.IsNullOrEmpty(mEvent.UserIpAddress))
                    mEvent.UserIpAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            if (pRequestData.StartsWith("99"))
            {
                // hex encoded
                var requestData = pRequestData.Substring(2, pRequestData.Length - 2); // dump the 99 from front
                // isloate key from data
                var request = GetIdDataFromRequest(requestData);
                if (String.IsNullOrEmpty(request.Item1))
                    mEvent.EventTypeDesc += request.Item2;
                else
                    mEvent.EventTypeDesc += HexToString(request.Item2);
            }
            else if (pRequestData.StartsWith(mEvent.ClientId.ToString().Length.ToString() + mEvent.ClientId))
            {
                // encrypted
                var request = GetIdDataFromRequest(pRequestData);
                if (String.IsNullOrEmpty(request.Item1))
                    mEvent.EventTypeDesc += request.Item2;
                else
                    mEvent.EventTypeDesc += Security.DecodeAndDecrypt(request.Item2, request.Item1);
            }
            else
            {
                mEvent.EventTypeDesc = mServiceName + ": " + Environment.NewLine + pRequestData + Environment.NewLine;
            }
            try
            {
                mEvent.Create();
            }
            catch (Exception ex)
            {
                var mDetails = "LogRequest() " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";");
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                if (pData != null)
                {
                    if (pData.ContainsKey(dk.ClientName))
                        exceptionEvent.EventTypeDesc += Constants.TokenKeys.ClientName + pData[dk.ClientName];
                }
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
            }

            var mEmailLogReq = ConfigurationManager.AppSettings[cfg.EmailServiceLog];
            if (!String.IsNullOrEmpty(mEmailLogReq))
            {
                if (mEmailLogReq.Contains(mServiceName))
                {
                    // call service here
                }

            }
            return mEvent._id.ToString();
        }

        /// <summary> </summary>
        public Tuple<bool, string> ServiceRequest(string url, string pId, Dictionary<String, String> pData)
        {
            try
            {
                // use string builder to be thread safe
                var data = String.Format("data={0}{1}{2}",
                    /*0*/pId.Length,
                    /*1*/pId,
                    /*2*/Security.EncryptAndEncode(JsonConvert.SerializeObject(pData), pId));

                // make web service call
                var dataStream = Encoding.UTF8.GetBytes(data);
                var webRequest = WebRequest.Create(url);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.ContentLength = dataStream.Length;
                var newStream = webRequest.GetRequestStream();
                // Send the data.
                newStream.Write(dataStream, 0, dataStream.Length);
                newStream.Close();
                var res = webRequest.GetResponse();
                var response = res.GetResponseStream();
                var xmlDoc = new XmlDocument();
                if (response != null) xmlDoc.Load(response);
                var elemList = xmlDoc.GetElementsByTagName("Error");
                if (elemList.Count != 0)
                    return new Tuple<bool, string>(false, elemList[0].InnerXml);
                elemList = xmlDoc.GetElementsByTagName("Reply");
                if (elemList.Count != 0)
                {
                    var reply = elemList[0].InnerXml;
                    // are there details
                    elemList = xmlDoc.GetElementsByTagName("Details");
                    if (elemList.Count != 0)
                    {// return reply and details
                        return new Tuple<bool, string>(true, reply + "|" + elemList[0].InnerXml);
                    }
                    // return just reply
                    return new Tuple<bool, string>(true, reply);
                }
                return new Tuple<bool, string>(false, "invalid response");
            }
            catch (Exception ex)
            {
                var mDetails = "ServiceRequest() " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";");
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                if (pData.ContainsKey(dk.ClientName))
                    exceptionEvent.EventTypeDesc += Constants.TokenKeys.ClientName + pData[dk.ClientName];

                return new Tuple<bool, string>(false, "ServiceRequest(" + url + "):" + ex.Message);
            }
        }

        /// <summary> </summary>
        public Tuple<bool, string> ThreadedSendRequestToOas(string pRequest, string pId, string pData)
        {
            //// use string builder to be thread safe
            // get base Url for MAC Open Service
            var fullUrlToService = new StringBuilder();
            try
            {
                if (pRequest.ToLower().Contains("register"))
                {
                    // registration request service url
                    fullUrlToService.Append(ConfigurationManager.AppSettings[cfg.MacOpenServicesUrl] +
                        Constants.ServiceUrls.MacOpenEndUserServices);
                }
                else if (pRequest.ToLower().Contains("enduser"))
                {
                    fullUrlToService.Append(ConfigurationManager.AppSettings[cfg.MacOpenServicesUrl] +
                        Constants.ServiceUrls.MacOpenEndUserServices);
                }
                else
                {
                    return new Tuple<bool, string>(false, "SendRequestToOAS: Request error:" + pRequest);
                }
            }
            catch
            {
                return new Tuple<bool, string>(false,
                    "SendRequestToOAS: Configuration error: MACOpenServiceUrl missing in web.config");
            }
            // make web service call
            var data = "data=" + pData;
            var dataStream = Encoding.UTF8.GetBytes(data);
            var webRequest = WebRequest.Create(fullUrlToService.ToString());
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = dataStream.Length;
            var newStream = webRequest.GetRequestStream();
            // Send the data.
            newStream.Write(dataStream, 0, dataStream.Length);
            newStream.Close();
            var res = webRequest.GetResponse();
            var response = res.GetResponseStream();
            var xmlDoc = new XmlDocument();
            if (response != null) xmlDoc.Load(response);
            var elemList = xmlDoc.GetElementsByTagName("Error");
            if (elemList.Count != 0)
                return new Tuple<bool, string>(false, elemList[0].InnerXml);
            elemList = xmlDoc.GetElementsByTagName("Reply");
            return new Tuple<bool, string>(true, elemList[0].InnerXml);
        }

        #endregion

        #region String functions

        /// <summary> </summary>
        public bool IsValueObjectId(string queryValue)
        {
            try
            {
                ObjectId.Parse(queryValue);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary> </summary>
        public bool HasLowerCase(string evalString)
        {
            return !string.IsNullOrEmpty(evalString) && Regex.IsMatch(evalString, "[a-z]");
        }

        /// <summary> </summary>
        public bool HasUpperCase(string evalString)
        {
            return !string.IsNullOrEmpty(evalString) && Regex.IsMatch(evalString, "[A-Z]");
        }

        /// <summary> </summary>
        public bool HasNumeric(string evalString)
        {
            return !string.IsNullOrEmpty(evalString) && Regex.IsMatch(evalString, "[0-9]");
        }

        /// <summary> </summary>
        public string FormatNumber(string formatValue)
        {
            int inputNumber = Convert.ToInt32(formatValue);
            var formattedNumber = String.Format("{0:##,####,####}", inputNumber);

            return formattedNumber;
        }

        #endregion

        #region Send Email

        /// <summary> </summary>
        public ProviderEmail GetMacEmailProvider(string clientId)
        {
            if (clientId == null) return null;

            var mongoCollection = mongoDBConnectionPool.GetCollection("TypeDefinitions");
            try
            {
                var query = Query.EQ("_id", "52a9ff62675c9b04c077107d");
                return mongoCollection.FindOneAs<ProviderEmail>(query);
            }
            catch
            {
                return null;
            }
        }

        public string GetEmailTemplate(string ownerId, string ownerType)
        {
            if (String.IsNullOrEmpty(ownerId))
                ownerId = Constants.Strings.DefaultClientId;

            if (String.IsNullOrEmpty(ownerType))
                ownerType = "Client";

            var currentServer = "http://" + HttpContext.Current.Request.ServerVariables["SERVER_NAME"];

            var sbEmailTemplate = new StringBuilder();

            var ownerName = "Mobile Authentication Corporation";
            var ownerPhone = "1.480.939.2980";
            var ownerImagePath = "/Images/OwnerLogos/!Empty-Placeholder.png";

            switch (ownerType)
            {
                case "Client":
                    var myClient = new Client(ownerId);
                    ownerName = myClient.Name;
                    ownerPhone = myClient.Organization.Phone;
                    ownerImagePath = myClient.OwnerLogoUrl;
                    break;

                case "Group":
                    var myGroup = new Group(ownerId);
                    ownerName = myGroup.Name;
                    ownerPhone = myGroup.Organization.Phone;
                    ownerImagePath = myGroup.OwnerLogoUrl;
                    break;
            }

            sbEmailTemplate.Append("<!DOCTYPE html>");
            sbEmailTemplate.Append("<html>");
            sbEmailTemplate.Append("    <head>");
            sbEmailTemplate.Append("        <title>MAC Email Template</title>");
            sbEmailTemplate.Append("    </head>");
            sbEmailTemplate.Append("    <body>");

            // Main body/table wrapper
            sbEmailTemplate.Append("            <table border='0' cellpadding='0' cellspacing='0' height='100%' width='100%' id='bodyTable'>");
            sbEmailTemplate.Append("                <tr>");
            sbEmailTemplate.Append("                    <td align='center' valign='top'>");

            // Actual content
            sbEmailTemplate.Append("                    <table style='border:1px solid #c0c0c0; width:700px;'>");
            sbEmailTemplate.Append("                        <tr>");
            sbEmailTemplate.Append("                            <td style='width: 200px; padding: 25px; white-space: nowrap;'>");

            // Logo
            if (ownerImagePath.Contains("!Empty-Placeholder"))
                sbEmailTemplate.Append("                                <div style='font-family: Helvetica, Arial, sans-serif; font-weight: bold;font-size: 16px;'>" + ownerName + "</div>");
            else
                sbEmailTemplate.Append("                                <img src='" + currentServer + ownerImagePath + "' width='175' />");

            sbEmailTemplate.Append("                            </td>");
            sbEmailTemplate.Append("                            <td colspan='2' style='width: 100%; text-align: right; padding: 25px; white-space: nowrap;'>");
            sbEmailTemplate.Append("                                <span style='font-family: Helvetica, Arial, sans-serif; font-weight: bold;font-size: 12px;'>[DocTitle]</span>");
            sbEmailTemplate.Append("                                <br />");
            sbEmailTemplate.Append("                                <span style='text-align: center; font-family: Helvetica, Arial, sans-serif; font-size: 12px;'>[Date]</span>");
            sbEmailTemplate.Append("                            </td>");
            sbEmailTemplate.Append("                        </tr>");

            sbEmailTemplate.Append("                        <tr>");
            sbEmailTemplate.Append("                            <td colspan='3' style='border-top: solid 1px #c0c0c0; border-bottom: solid 1px #c0c0c0; padding: 25px;'>");
            sbEmailTemplate.Append("                                <span style='font-family: Helvetica, Arial, sans-serif; font-size: 12px;'>[MessageBody]</span>");
            sbEmailTemplate.Append("                            </td>");
            sbEmailTemplate.Append("                        </tr>");
            sbEmailTemplate.Append("                        <tr>");
            sbEmailTemplate.Append("                            <td colspan='3' style='padding: 25px; padding-bottom: 50px; padding-right: 50px; text-align: left;'>");
            sbEmailTemplate.Append("                                <p style='font-family: Helvetica, Arial, sans-serif;font-size: 12px;'>");
            sbEmailTemplate.Append("                                    <strong>Sincerely,</strong><br />");

            // Name
            sbEmailTemplate.Append(ownerName + "                        <br />");

            // Phone number
            sbEmailTemplate.Append(ownerPhone + "                       <br />");

            sbEmailTemplate.Append("                                </p>");
            sbEmailTemplate.Append("                            </td>");
            sbEmailTemplate.Append("                        </tr>");
            sbEmailTemplate.Append("                        <tr>");
            sbEmailTemplate.Append("                            <td colspan='3' style='border-top: solid 1px #c0c0c0; padding: 15px; text-align: right;'>");
            sbEmailTemplate.Append("                                <span style='font-family: Helvetica, Arial, sans-serif; font-size: 12px; color: #999;'>&copy; 2013-" + DateTime.Now.Year + " " + ownerName + ". All rights reserved.</span>");
            sbEmailTemplate.Append("                            </td>");
            sbEmailTemplate.Append("                        </tr>");
            sbEmailTemplate.Append("                    </table>");

            // Close the wrapper
            sbEmailTemplate.Append("                </td>");
            sbEmailTemplate.Append("            </tr>");
            sbEmailTemplate.Append("        </table>");

            sbEmailTemplate.Append("    </body>");
            sbEmailTemplate.Append("</html>");

            return sbEmailTemplate.ToString();
        }

        public bool SendGenericEmail(string ownerId, string ownerType, string FromAddress, string ToAddress, string Subject, string Body, bool IsBodyHtml)
        {
            if(FromAddress == "")
                FromAddress = ConfigurationManager.AppSettings[cfg.FromAddress];

            var emailTemplate = GetEmailTemplate(ownerId, ownerType);

            try
            {
                //Set up message
                var message = new MailMessage { From = new MailAddress(FromAddress) };
                message.To.Add(new MailAddress(ToAddress));
                message.Subject = Subject;
                message.IsBodyHtml = IsBodyHtml;

                var messageBody = emailTemplate; // sbEmailTemplate.Replace("[DocTitle]", Subject);
                messageBody = messageBody.Replace("[DocTitle]", Subject);
                messageBody = messageBody.Replace("[Date]", DateTime.UtcNow.ToLocalTime().ToString());
                messageBody = messageBody.Replace("[MessageBody]", Body.Replace("|", IsBodyHtml ? "<br />" : Environment.NewLine));

                message.Body = messageBody.ToString();

                message.Priority = MailPriority.High;

                message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                // setup Smtp Client
                var smtp = new SmtpClient
                {
                    Port = Convert.ToInt16(ConfigurationManager.AppSettings[cfg.Port]),
                    Host = ConfigurationManager.AppSettings[cfg.Host],
                    EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings[cfg.EnableSsl]),
                    UseDefaultCredentials =
                        Convert.ToBoolean(ConfigurationManager.AppSettings[cfg.UseDefaultCredentials]),
                    Credentials =
                        new NetworkCredential(ConfigurationManager.AppSettings[cfg.LoginUserName],
                            ConfigurationManager.AppSettings[cfg.LoginPassword]),
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                smtp.Send(message);

                return true;
            }
// ReSharper disable once EmptyGeneralCatchClause
            catch(Exception ex)
            {
                var mDetails = "SendGenericEmail() " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
            }
            return false;
        }

        /// <summary>
        ///     SendEmail starts a thread that does the smtp send
        /// </summary>
        /// <param name="pProvider">Selected ProviderEmail Provider from Client</param>
        /// <param name="pToAddress">User'semail address</param>
        /// <param name="pSubject">ProviderEmail Subject</param>
        /// <param name="pBody">Message</param>
        /// <param name="pHashedUserId">Hashed end user id if email registration, Changes End user state</param>
        /// <returns></returns>
        public Tuple<bool, string> SendEmail(ProviderEmail pProvider, string pToAddress, string pSubject, string pBody, string pHashedUserId)
        {
            // Do not wait for smtp to send the email, start a thread
            try
            {
                var myThread = new Thread(() => ThreadedSendEmail(
                    /*1*/pProvider.Server,
                    /*2*/Convert.ToInt16(pProvider.Port),
                    /*3*/pProvider.LoginUserName,
                    /*4*/pProvider.LoginPassword,
                    /*5*/pProvider.FromEmail,
                    /*6*/pToAddress,
                    /*7*/pSubject,
                    /*8*/pBody,
                    /*9*/pProvider.RequiresSsl,
                    /*10*/pProvider.IsBodyHtml,
                    /*11*/pProvider.CredentialsRequired,
                    /*12*/pProvider.AdminNotificationOnFailure,
                    /*13*/pHashedUserId));

                myThread.Start();

                //var queuedEvent = new Event();

                ////queuedEvent.UserId = mEndUser._id;
                ////myEvent.ClientId = ObjectId.Parse(_hiddenD.Value);

                //var replacementTokens = Constants.TokenKeys.DeliveryMethod + tokenKeys.Email;
                //replacementTokens += Constants.TokenKeys.SentToAddress + pToAddress;

                //queuedEvent.Create(Constants.EventLog.Otp.Queued, replacementTokens);

                return new Tuple<bool, string>(true, "Email Queued");
            }
            catch (Exception ex)
            {
                var mDetails = "SendEmail() " + ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                return new Tuple<bool, string>(false,
                    String.Format("Email.Server:{0} Exception:{1}", pProvider.Server, ex.Message));
            }
        }

        /// <summary> Send ProviderEmail Thread </summary>
        private void ThreadedSendEmail(
            /*1*/ string pServer,
            /*2*/int pPort,
            /*3*/string pLoginUserName,
            /*4*/string pLoginPassword,
            /*5*/string pFromAddress,
            /*6*/string pToAddress,
            /*7*/string pSubject,
            /*8*/string pBody,
            /*9*/bool pEnableSsl,
            /*10*/bool pIsBodyHtml,
            /*11*/bool pUseDefaultCredentials,
            /*12*/bool pAdminNotificationOnFailure,
            /*13*/String pHashedUserId)
        {
            var mUtils = new Utils();
            EndUser mEndUser = null;
            if (pHashedUserId != null)
            {
                for (var x = 0; x < 5; ++x)
                {
                    mEndUser = mUtils.GetEndUserByHashedUserId(pHashedUserId);
                    if (mEndUser != null) break;
                    Thread.Sleep(50);
                }
            }

            var mEndUserEmailThreadEvent = new EndUserEvent("");
            try
            {
                //Set up message
                var message = new MailMessage { From = new MailAddress(pFromAddress) };
                message.To.Add(pToAddress); //one or more to email addresses seperated by commas
                message.Subject = pSubject;
                message.IsBodyHtml = pIsBodyHtml;
                message.Body = pBody.Replace("|", pIsBodyHtml ? "<br />" : Environment.NewLine);
                message.Priority = MailPriority.High;
                if (pAdminNotificationOnFailure)
                    message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                // setup Smtp Client
                var smtp = new SmtpClient
                {
                    Port = pPort,
                    Host = pServer,
                    EnableSsl = pEnableSsl,
                    UseDefaultCredentials = pUseDefaultCredentials
                };

                //var smtp = new SmtpClient
                //{
                //    Port = pPort,
                //    Host = pServer,
                //    EnableSsl = pEnableSsl
                //};

                if (pUseDefaultCredentials)
                    smtp.Credentials = new NetworkCredential(pLoginUserName, pLoginPassword);

                //smtp.Credentials = new NetworkCredential(pLoginUserName, pLoginPassword);

                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                // Joe - Security testing
                //smtp.Send(message);

                //if the end user id was passed
                if (pHashedUserId == null) return;
                // and its valid
                if (mEndUser == null) return;
                // and the end user's state is waiting for email to be sent
                if (mEndUser.State != Constants.EndUserStates.WaitingEmailSent) return;
                // its a send registration email so update the end user state waiting on user
                mEndUser.State = Constants.EndUserStates.WaitingCompletion;
                mEndUserEmailThreadEvent.Details = "ThreadedSendEmail.Registration, New State=" + mEndUser.State;
                mEndUser.EndUserEvents.Add(mEndUserEmailThreadEvent);
                mUtils.ObjectUpdate(mEndUser, mEndUser._id.ToString());
            }
            catch (Exception ex)
            {
                var mDetails =
                    Constants.TokenKeys.ExceptionDetails +
                    "Error in Utils.ThreadedSendEmail() " +
                    ex.Message.Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") +
                    " Server(" + pServer + ") Port(" + pPort + ")" +
                    dk.ItemSep +
                    Constants.TokenKeys.ClientName + "NA";

                var exceptionEvent = new Event();
                if (mEndUser != null)
                {
                    exceptionEvent.UserId = mEndUser._id;
                    mEndUserEmailThreadEvent.Details = exceptionEvent.EventTypeDesc;
                    mEndUser.EndUserEvents.Add(mEndUserEmailThreadEvent);
                    mUtils.ObjectUpdate(mEndUser, mEndUser._id.ToString());
                }
                mEndUserEmailThreadEvent.Details += Constants.TokenKeys.ClientName + "NA" + mDetails + ex.ToString();
                mUtils.ObjectCreate(exceptionEvent);
                //exceptionEvent.Create(Constants.EventLog.Exceptions.General, mDetails);
            }
        }

        #endregion

        #region Cookie methods

        /// <summary> </summary>
        public void CreateCookie(string cookieName, string cookieValue, DateTime cookieExpires)
        {
            var myCookie = new HttpCookie(cookieName) { Value = cookieValue, Expires = cookieExpires };

            HttpContext.Current.Response.Cookies.Add(myCookie);
        }

        /// <summary> </summary>
        public string ReadCookie(string cookieName)
        {
            var myCookie = HttpContext.Current.Request.Cookies[cookieName];
            Debug.Assert(myCookie != null, "myCookie != null");
            return myCookie.Value;
        }

        /// <summary> </summary>
        public void UpdateCookie()
        {
        }

        /// <summary> </summary>
        public void DeleteCookie()
        {
        }

        #endregion
    }
}
