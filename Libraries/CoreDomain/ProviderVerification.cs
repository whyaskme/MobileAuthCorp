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
    public class ProviderVerification : ProviderBase
    {
        public ProviderVerification()
        {
            ApiKey = String.Empty;
            ApiVersion = String.Empty;
            ChargeTiers = new PricingTierCharge(_id.ToString());
            LoginPassword = String.Empty;
            LoginUsername = String.Empty;
            ProcessingParameters = String.Empty;
            Protocol = String.Empty;
            ProxyHost = String.Empty;
            ProxyPort = String.Empty;
            RequestParameters = String.Empty;
            SearchType = String.Empty;
            ServiceName = String.Empty;
            Url = String.Empty;
        }

        private string apikey;
        public string ApiKey { get { return apikey; } set { apikey = Security.EncryptAndEncode(value, EncKey); } }

        private string apiversion;
        public string ApiVersion { get { return apiversion; } set { apiversion = Security.EncryptAndEncode(value, EncKey); } }

        public PricingTierCharge ChargeTiers { get; set; }

        private string loginpassword;
        public string LoginPassword { get { return loginpassword; } set { loginpassword = Security.EncryptAndEncode(value, EncKey); } }

        private string loginusername;
        public string LoginUsername { get { return loginusername; } set { loginusername = Security.EncryptAndEncode(value, EncKey); } }

        private string processingparameters;
        public string ProcessingParameters { get { return processingparameters; } set { processingparameters = Security.EncryptAndEncode(value, EncKey); } }

        private string protocol;
        public string Protocol { get { return protocol; } set { protocol = Security.EncryptAndEncode(value, EncKey); } }

        private string proxyhost;
        public string ProxyHost { get { return proxyhost; } set { proxyhost = Security.EncryptAndEncode(value, EncKey); } }

        private string proxyport;
        public string ProxyPort { get { return proxyport; } set { proxyport = Security.EncryptAndEncode(value, EncKey); } }

        private string requestparameters;
        public string RequestParameters { get { return requestparameters; } set { requestparameters = Security.EncryptAndEncode(value, EncKey); } }

        private string searchtype;
        public string SearchType { get { return searchtype; } set { searchtype = Security.EncryptAndEncode(value, EncKey); } }

        private string servicename;
        public string ServiceName { get { return servicename; } set { servicename = Security.EncryptAndEncode(value, EncKey); } }

        private string url;
        public string Url { get { return url; } set { url = Security.EncryptAndEncode(value, EncKey); } }
    }
}
