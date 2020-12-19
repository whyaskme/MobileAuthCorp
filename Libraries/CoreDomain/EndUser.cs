using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace MACServices
{
    public class EndUser : Base
    {
        public List<EndUserEvent> EndUserEvents = new List<EndUserEvent>();

        public EndUser()
        {
            _id = ObjectId.GenerateNewId();
            DateRegistered = DateTime.UtcNow;
            ClientId = ObjectId.Parse(Constants.Strings.DefaultEmptyObjectId);
            Active = false;
            HashedUserId = String.Empty;
            FirstTimeCarrierInfoSent = false;    // false = send carrier info, true = don't send
            Email = String.Empty;
            Phone = String.Empty;
            State = String.Empty;
            RegistrationType = String.Empty;
            NotifyOpts = String.Empty;
            Prefix = String.Empty;
            FirstName = String.Empty;
            MiddleName = String.Empty;
            LastName = String.Empty;
            Suffix = String.Empty;
            DateOfBirth = "1/1/1900";
            OtpOutAd = false;
            Address = new Address();
            PH = String.Empty;
            EH = String.Empty;
            Relationships = new List<Relationship>();
        }
        public DateTime DateRegistered { get; set; }
        public string HashedUserId { get; set; }
        public bool FirstTimeCarrierInfoSent { get; set; }
        public string Prefix { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string DateOfBirth { get; set; }
        public string RegistrationType { get; set; }
        public string NotifyOpts { get; set; }
        public bool Active { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool OtpOutAd { get; set; }
        public string State { get; set; }
        public string PH { get; set; }
        public string EH { get; set; }
        public Address Address { get; set; }
    }
}