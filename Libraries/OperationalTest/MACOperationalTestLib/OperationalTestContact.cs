using System;
using MongoDB.Bson;

namespace MACOperationalTestLib
{
    public class OperationalTestContact
    {
        public OperationalTestContact()
        {
            _id = ObjectId.GenerateNewId();
            _t = "OperationalTestContact";
            DateLastUpdate = DateTime.UtcNow;
            eFirstName = String.Empty;
            eLastName = String.Empty;
            eEmailAddress = String.Empty;
            ePhoneNumber = String.Empty;
            SendSMS = true;
            SendEmail = false;
        }
        public ObjectId _id { get; set; }
        public string _t { get; set; }
        public DateTime DateLastUpdate { get; set; }
        public string eFirstName { get; set; }
        public string eLastName { get; set; }
        public string eEmailAddress { get; set; }
        public string ePhoneNumber { get; set; }
        public bool SendSMS { get; set; }
        public bool SendEmail { get; set; }
    }
}
