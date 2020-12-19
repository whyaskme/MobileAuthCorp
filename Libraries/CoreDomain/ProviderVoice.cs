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
    public class ProviderVoice
    {
        public ProviderVoice()
        {
            var mongoDBConnectionPool = (MongoDatabase)HttpContext.Current.Application[cs.MongoDB];

            var query = Query.EQ("_id", ObjectId.Parse(cs.DefaultVoiceProvider));
            var mongoCollection = mongoDBConnectionPool.GetCollection("TypeDefinitions").Find(query);

            foreach (var doc in mongoCollection)
            {
                foreach (var element in doc.Elements)
                {
                    var elementName = element.Name;
                    switch (elementName)
                    {
                        case "Name":
                            Name = element.Value.ToString();
                            break;
                        case "Enabled":
                            Enabled = Convert.ToBoolean(element.Value);
                            break;
                        case "ApiVersion":
                            ApiVersion = element.Value.ToString();
                            break;
                        case "AuthToken":
                            AuthToken = element.Value.ToString();
                            break;
                        case "FromPhoneNumber":
                            FromPhoneNumber = element.Value.ToString();
                            break;
                        case "ShortCodeFromNumber":
                            ShortCodeFromNumber = element.Value.ToString();
                            break;
                        case "Key":
                            Key = element.Value.ToString();
                            break;
                        case "LoginUsername":
                            LoginUsername = element.Value.ToString();
                            break;
                        case "LoginPassword":
                            LoginPassword = element.Value.ToString();
                            break;
                        case "NewlineReplacement":
                            NewlineReplacement = element.Value.ToString();
                            break;
                        case "Port":
                            Port = element.Value.ToString();
                            break;
                        case "Protocol":
                            Protocol = element.Value.ToString();
                            break;
                        case "RequiresSsl":
                            RequiresSsl = Convert.ToBoolean(element.Value);
                            break;
                        case "CredentialsRequired":
                            CredentialsRequired = Convert.ToBoolean(element.Value);
                            break;
                        case "Server":
                            Server = element.Value.ToString();
                            break;
                        case "Sid":
                            Sid = element.Value.ToString();
                            break;
                        case "VoiceToken":
                            VoiceToken = element.Value.ToString();
                            break;
                        case "PhoneNumberFormat":
                            PhoneNumberFormat = element.Value.ToString();
                            break;
                        case "Url":
                            Url = element.Value.ToString();
                            break;
                    }
                }
            }

            _id = ObjectId.Parse(cs.DefaultVoiceProvider);
            DateCreated = DateTime.UtcNow;
        }

        public ObjectId _id { get; set; }
        public DateTime DateCreated { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public double ProviderCharge { get; set; }
        public double ClientCharge { get; set; }
        public string ApiVersion { get; set; }
        public string AuthToken { get; set; }
        public string FromPhoneNumber { get; set; }
        public string ShortCodeFromNumber { get; set; }
        public string Key { get; set; }
        public string LoginUsername { get; set; }
        public string LoginPassword { get; set; }
        public string NewlineReplacement { get; set; }
        public string Port { get; set; }
        public string Protocol { get; set; }
        public bool RequiresSsl { get; set; }
        public bool CredentialsRequired { get; set; }
        public string Server { get; set; }
        public string Sid { get; set; }
        public string VoiceToken { get; set; }
        public string PhoneNumberFormat { get; set; }
        public string Url { get; set; }
        public string FromEmail { get; set; }

    }
}