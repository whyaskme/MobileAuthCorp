using System;
using MongoDB.Bson;

namespace STLib
{
    public class StiEvent : MongoDBData
    {
        public StiEvent()
        {
            _id = ObjectId.GenerateNewId();
            _t = "Event";

            // MAC Info
            macStateId = ObjectId.GenerateNewId();
            macClientId = "";
            macType = "";
            macData = "";

            // Secure Trading Info
            stOperatorId = "";
            stSiteId = "";
            stToken = "";
            stJsonData = "";
            stReturnCode = "";
            stResponseJson = "";
            stRecordStatus = "";
            stPlayerId = "";
            stPlayerIp = "";
            stTimeStamp = DateTime.UtcNow;
        }
        // Wrapper
        public ObjectId _id { get; set; }
        public string _t { get; set; }
        public ObjectId macStateId { get; set; }
        public string macType { get; set; }
        public string macData { get; set; }
        public string macClientId { get; set; }

        // Secure Trading Info
        public string stOperatorId { get; set; }
        public string stSiteId { get; set; }
        public string stToken { get; set; }
        public string stJsonData { get; set; }
        public string stReturnCode { get; set; }
        public string stResponseJson { get; set; }
        public string stRecordStatus { get; set; }
        public DateTime stTimeStamp { get; set; }
        public string stPlayerId { get; set; }
        public string stPlayerIp { get; set; }

    }
}
