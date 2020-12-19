using System;
using MongoDB.Bson;

using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using cfgcs = MACServices.Constants.WebConfig.ConnectionStringKeys;
using cs = MACServices.Constants.Strings;

namespace MACServices
{
    public class ProviderAdvertising
    {
        public ProviderAdvertising()
        {
            _id = ObjectId.GenerateNewId();
            DateCreated = DateTime.UtcNow;

            Name = "SecureAds";
            AdClientId = "30880828-adcd-a4cb-3eea-08d1bc2aa550"; // This is temp. Each client will have their own unique values
            ApiKey = "1106fdc1-1b91-71c4-296e-08d1b88891e5";
            ApiUrl = "api.authenticationads.com/Ad.svc";
            UserName = "macapi";
            Password = "mac123!";
            P1 = "";
            P2 = "";
            P3 = "";
            P4 = "";
        }

        public ObjectId _id { get; set; }
        public DateTime DateCreated { get; set; }

        public string Name { get; set; }
        public string AdClientId { get; set; }
        public string ApiKey { get; set; }
        public string ApiUrl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string P1 { get; set; }
        public string P2 { get; set; }
        public string P3 { get; set; }
        public string P4 { get; set; }
    }
}
