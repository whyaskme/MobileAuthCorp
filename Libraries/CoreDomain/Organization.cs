using System;
using MongoDB.Bson;

namespace MACServices
{
    public class Organization
    {
        public Organization()
        {
            TaxId = String.Empty;
            Street1 = String.Empty;
            Street2 = String.Empty;
            Unit = String.Empty;
            City = String.Empty;
            State = ObjectId.Parse(Constants.Strings.DefaultEmptyObjectId);
            Zipcode = String.Empty;
            Country = String.Empty;
            Email = String.Empty;
            Phone = String.Empty;
            Extension = String.Empty;
            AdminNotificationProvider = new ProviderEmail();
            PrimaryAdminId = ObjectId.Parse(Constants.Strings.DefaultEmptyObjectId);
        }

        public string TaxId { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string Unit { get; set; }
        public string City { get; set; }
        public ObjectId State { get; set; }
        public string Zipcode { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Extension { get; set; }
        public ProviderEmail AdminNotificationProvider { get; set; }
        public ObjectId PrimaryAdminId { get; set; }
    }
}