using System;
using System.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

using MACSecurity;

using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using cfgcs = MACServices.Constants.WebConfig.ConnectionStringKeys;

namespace MACServices
{
    public class ProviderBase : PropertySecurity
    {
        public ProviderBase()
        {
            _id = ObjectId.GenerateNewId();
            Name = String.Empty;
            Enabled = true;
            EncKey = _id.ToString();
            DateCreated = DateTime.UtcNow;
        }

        // Base properties
        public ObjectId _id { get; set; }
        public string EncKey { get; set; }
        public DateTime DateCreated { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }

        // Overflow properties
        private string p1;
        public string P1 { get { return p1; } set { p1 = Security.EncryptAndEncode(value, EncKey); } }

        private string p2;
        public string P2 { get { return p2; } set { p2 = Security.EncryptAndEncode(value, EncKey); } }

        private string p3;
        public string P3 { get { return p3; } set { p3 = Security.EncryptAndEncode(value, EncKey); } }

        private string p4;
        public string P4 { get { return p4; } set { p4 = Security.EncryptAndEncode(value, EncKey); } }
    }
}
