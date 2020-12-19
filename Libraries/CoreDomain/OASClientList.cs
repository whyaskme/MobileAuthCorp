using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace MACServices
{
    public class OasClientList
    {
        public List<OasClientEvent> EndUserEvents = new List<OasClientEvent>();

        public OasClientList(string clientId)
        {
            _id = ObjectId.GenerateNewId();
            ClientId = ObjectId.Parse(clientId);
            Enabled = true;
            Date = DateTime.UtcNow;
        }

        public ObjectId _id { get; set; }
        public ObjectId ClientId { get; set; }
        public string Name { get; set; }
        public string FullyQualifiedDomainName { get; set; }
        public bool Enabled { get; set; }
        public DateTime Date { get; set; }
    }
}