using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace MACServices
{
    public class UserProfile : Base
    {
        public UserProfile(string userId)
        {
            //var decryptedUserId = MACSecurity.Security.DecodeAndDecrypt(_hiddenE.Value, Constants.Strings.DefaultClientId);

            //if (Convert.ToBoolean(MACSecurity.Security.DecodeAndDecrypt(_hiddenF.Value, decryptedUserId)))

            if (string.IsNullOrEmpty(userId) || userId == Constants.Strings.DefaultEmptyObjectId)
            {
                UserId = ObjectId.Parse(Constants.Strings.DefaultEmptyObjectId);
                Prefix = String.Empty;
                FirstName = String.Empty;
                MiddleName = String.Empty;
                LastName = String.Empty;
                Suffix = String.Empty;
                DateOfBirth = "1/1/1900";
                Roles = new List<ObjectId>();
                Contact = new Contact();
                Address = new Address();
                Relationships = new List<Relationship>();
                FirstTimeCarrierInfoSent = false;
                IsReadOnly = false;
            }
            else
            {
                _id = ObjectId.Parse(userId);
                UserId = ObjectId.Parse(userId);

                // Read in the object data from the db and return the populated object
                var myProfile = (UserProfile) Read();

                if (myProfile != null)
                {
                    Prefix = myProfile.Prefix;
                    FirstName = myProfile.FirstName;
                    MiddleName = myProfile.MiddleName;
                    LastName = myProfile.LastName;
                    Suffix = myProfile.Suffix;
                    DateOfBirth = myProfile.DateOfBirth;
                    Roles = myProfile.Roles;
                    Contact = myProfile.Contact;
                    Address = myProfile.Address;
                    Relationships = myProfile.Relationships;
                    FirstTimeCarrierInfoSent = myProfile.FirstTimeCarrierInfoSent;
                    IsReadOnly = myProfile.IsReadOnly;
                }
            }
        }

        public ObjectId UserId { get; set; }
        public string Prefix { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string DateOfBirth { get; set; }

        // Property collections
        public List<ObjectId> Roles { get; set; }
        public Contact Contact { get; set; }
        public Address Address { get; set; }
        public bool FirstTimeCarrierInfoSent { get; set; }    // false send carrier info on first text message, true = don't send
        public bool IsReadOnly { get; set; }
    }
}