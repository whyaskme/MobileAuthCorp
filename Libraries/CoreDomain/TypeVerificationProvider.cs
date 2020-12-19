using System;
using MongoDB.Bson;

namespace MACServices
{
    public class UserVerificationProvider
    {
        public UserVerificationProvider(string description)
        {
            _id = ObjectId.GenerateNewId();
            Name = String.Empty;
            Enabled = true;

            ApiKey = String.Empty;
            ApiVersion = String.Empty;
            BaseUrl = String.Empty;
            Login = String.Empty;
            Password = String.Empty;
            ProcessingParameters = String.Empty;
            Protocol = String.Empty;
            ProxyHost = String.Empty;
            ProxyPort = String.Empty;
            RequestParameters = String.Empty;
            SearchType = String.Empty;
            ServiceName = String.Empty;
        }

        public ObjectId _id { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }

        public string ApiKey { get; set; }
        public string ApiVersion { get; set; }
        public string BaseUrl { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string ProcessingParameters { get; set; }
        public string Protocol { get; set; }
        public string ProxyHost { get; set; }
        public string ProxyPort { get; set; }
        public string RequestParameters { get; set; }
        public string SearchType { get; set; }
        public string ServiceName { get; set; }
    }
}