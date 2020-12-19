using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Xml;

using MACSecurity;
using MACServices;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Newtonsoft.Json;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

// [System.Web.Script.Services.ScriptService]
public class GetUsers : System.Web.Services.WebService {

    public GetUsers () {}

    [WebMethod]
    public XmlDocument WsGetUsers(string systemUnderTest)
    {
        MongoClient mongoClient;
        MongoServer mongoServer;
        MongoDatabase mongoDBDatabase;
        MongoCursor MongoCollection;
        MongoCursor mongoProfileCollection;

        var targetAppServer = "";
        var targetDatabaseServer = "";
        var targetDatabaseName = "";

        var sbResponse = new StringBuilder();
        var xmlResponse = new XmlDocument();

        try
        {
            if (string.IsNullOrEmpty(systemUnderTest))
                systemUnderTest = "localhost";

            string[] serversToTest = ConfigurationManager.AppSettings["TargetServers"].Split('|');
            foreach (var currentServerConfig in serversToTest)
            {
                var tmpVal = currentServerConfig.Split('~');

                targetAppServer = tmpVal[0].Trim();
                targetDatabaseServer = tmpVal[1].Trim();
                targetDatabaseName = tmpVal[2].Trim();

                if (targetAppServer == systemUnderTest)
                {
                    switch (systemUnderTest)
                    {
                        case "localhost":
                            if (targetAppServer == "localhost")
                            {
                                // Figure out which localhost and append the database name accordingly
                                var mComputer = Environment.MachineName.ToUpper();
                                switch (mComputer.ToLower())
                                {
                                    case "chrismiller":
                                        targetDatabaseName += "_Chris";
                                        break;
                                    case "terryshotbox":
                                        targetDatabaseName += "_Terry";
                                        break;
                                    case "lenovo-pc":
                                        targetDatabaseName += "_Joe";
                                        break;
                                }
                            }
                            break;
                    }

                    var dbConnectionSettings = targetDatabaseServer.Split('@');

                    // Server credentials
                    var dbCredentials = dbConnectionSettings[0].Split(':');
                    var dbUserName = dbCredentials[1].Replace("//", "");
                    var dbPassword = dbCredentials[2];

                    // Server settings
                    var dbServerPort = dbConnectionSettings[1].Split(':');
                    var dbServer = dbServerPort[0];
                    var dbPort = dbServerPort[1];

                    // Connect to the database and get users
                    var mongoClientSettings = new MongoClientSettings();
                    
                    mongoClientSettings.Credentials = new[] { MongoCredential.CreateMongoCRCredential(targetDatabaseName, dbUserName, dbPassword) };

                    mongoClientSettings.Server = new MongoServerAddress(dbServer, Convert.ToInt16(dbPort));

                    mongoClient = new MongoClient(mongoClientSettings);

                    mongoServer = mongoClient.GetServer();
                    mongoDBDatabase = mongoServer.GetDatabase(targetDatabaseName);

                    sbResponse.Append("<serviceresponse databaseserver='" + dbServer + "' databasename='" + targetDatabaseName + "'>");

                    sbResponse.Append(" <users>");

                    var query = Query.EQ("IsApproved", true);
                    var sortBy = SortBy.Ascending("Username");
                    MongoCollection = mongoDBDatabase.GetCollection("Users").Find(query);//.SetSortOrder(sortBy);
                    foreach (BsonDocument doc in MongoCollection)
                    {
                        var adminId = GetDocElementValueByName("_id", doc);
                        var adminUserName = GetDocElementValueByName("Username", doc);

                        // Get user profile for these values
                        var profileQuery = Query.EQ("UserId", ObjectId.Parse(adminId));
                        mongoProfileCollection = mongoDBDatabase.GetCollection("UserProfile").Find(profileQuery);
                        foreach (BsonDocument profileDoc in mongoProfileCollection)
                        {
                            var adminFirstName = GetDocElementValueByName("FirstName", profileDoc);
                            adminFirstName = MACSecurity.Security.DecodeAndDecrypt(adminFirstName, adminId);

                            var adminLastName = GetDocElementValueByName("LastName", profileDoc);
                            adminLastName = MACSecurity.Security.DecodeAndDecrypt(adminLastName, adminId);

                            var adminName = adminFirstName + " " + adminLastName;

                            var roleId = GetDocElementValueByName("Roles", profileDoc).Replace("[", "").Replace("]", "");

                            var roleQuery = Query.EQ("_id", ObjectId.Parse(roleId.Trim()));
                            var mongoCollection = mongoDBDatabase.GetCollection("Roles");
                            var myRole = mongoCollection.FindOneAs<UserRole>(roleQuery);

                            var adminRole = "Role not assigned";

                            if (myRole != null)
                                adminRole = myRole.Role;

                            sbResponse.Append("<user name='" + adminName + "' username='" + adminUserName + "' role='" + adminRole + "' />");
                        }
                    }
                    sbResponse.Append(" </users>");
                }
            }
        }
        catch (Exception ex)
        {
            sbResponse.Append(ex.ToString());
        }

        sbResponse.Append("</serviceresponse>");

        xmlResponse.LoadXml(sbResponse.ToString());

        return xmlResponse;

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
