using System;
using System.Web;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using cfgcs = MACServices.Constants.WebConfig.ConnectionStringKeys;
using cs = MACServices.Constants.Strings;

namespace MACServices
{
    public class ProviderEmail
    {
        public ProviderEmail()
        {
            var mongoDBConnectionPool = (MongoDatabase)HttpContext.Current.Application[cs.MongoDB];

            var query = Query.EQ("_id", ObjectId.Parse(cs.DefaultEmailProvider));
            var mongoCollection = mongoDBConnectionPool.GetCollection("TypeDefinitions").Find(query);

            foreach (var doc in mongoCollection)
            {
                foreach (var element in doc.Elements)
                {
                    string elementName = element.Name;
                    switch (elementName)
                    {
                        case "Name":
                            Name = element.Value.ToString();
                            break;
                        case "Enabled":
                            Enabled = Convert.ToBoolean(element.Value);
                            break;
                        case "OtpSubject":
                            OtpSubject = element.Value.ToString();
                            break;
                        case "NotficationSubject":
                            NotficationSubject = element.Value.ToString();
                            break;
                        case "FromEmail":
                            FromEmail = element.Value.ToString();
                            break;
                        case "LoginUserName":
                            LoginUserName = element.Value.ToString();
                            break;
                        case "LoginPassword":
                            LoginPassword = element.Value.ToString();
                            break;
                        case "Server":
                            Server = element.Value.ToString();
                            break;
                        case "Port":
                            Port = element.Value.ToString();
                            break;
                        case "NewlineReplacement":
                            NewlineReplacement = element.Value.ToString();
                            break;
                        case "RequiresSsl":
                            RequiresSsl = Convert.ToBoolean(element.Value);
                            break;
                        case "CredentialsRequired":
                            CredentialsRequired = Convert.ToBoolean(element.Value);
                            break;
                        case "IsBodyHtml":
                            IsBodyHtml = Convert.ToBoolean(element.Value);
                            break;
                        case "ProviderCharge":
                            ProviderCharge = Convert.ToDouble(element.Value.ToString());
                            break;
                        case "ClientCharge":
                            ClientCharge = Convert.ToDouble(element.Value.ToString());
                            break;
                        case "AdminNotificationOnFailure":
                            AdminNotificationOnFailure = Convert.ToBoolean(element.Value);
                            break;
                    }
                }
            }

            // Default Object
            _id = ObjectId.Parse(cs.DefaultEmailProvider);
            DateCreated = DateTime.UtcNow;
        }

        public ObjectId _id { get; set; }
        public DateTime DateCreated { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public double ProviderCharge { get; set; }
        public double ClientCharge { get; set; }
        public string FromEmail { get; set; }
        public string LoginUserName { get; set; }
        public string LoginPassword { get; set; }
        public string Port { get; set; }
        public string NewlineReplacement { get; set; }
        public bool RequiresSsl { get; set; }
        public bool CredentialsRequired { get; set; }
        public bool IsBodyHtml { get; set; }
        public string Server { get; set; }
        public string OtpSubject { get; set; }
        public string NotficationSubject { get; set; }
        public bool AdminNotificationOnFailure { get; set; }
    }
}