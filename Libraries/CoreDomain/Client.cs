using System;
using System.Collections.Generic;
using System.Globalization;
using MongoDB.Bson;

using delim = MACServices.Constants.Common;

namespace MACServices
{
    public class Client : Base
    {
        public Client(string clientId)
        {
            Utils myUtils = new Utils();

            Enabled = true;

            if (String.IsNullOrEmpty(clientId) || clientId == Constants.Strings.DefaultEmptyObjectId || clientId == Constants.Strings.DefaultStaticObjectId)
            {
                _id = ObjectId.GenerateNewId();
                ClientId = _id;
                DateCreated = DateTime.UtcNow;
                DateModified = DateTime.UtcNow;
                MessageProviders = new MessageProvider();
                VerificationProviders = new List<VerificationProvider>();
                Organization = new Organization();
                OtpSettings = new OtpSettings();
                DocumentTemplates = new List<DocumentTemplate>();
                OpenAccessServicesEnabled = false;
                InMaintenance = false;
                ClientAppUrl = "/LandingSites/";
                RegistrationCompletionUrl = "/Registration-Complete.html";
                AuthorizedDomain = "mobileauthcorp.com";
                AdEnabled = true;
                EndUserAdOptOutEnabled = true;
                AdProviders = new List<ProviderAdvertising>();

                AllowedIpList = String.Empty;

                OwnerLogoUrl = Constants.Common.EmptyOwnerLogoUrl;

                SecureTradingSiteId = String.Empty;
            }
            else
            {
                _id = ObjectId.Parse(clientId);
                ClientId = ObjectId.Parse(clientId);

                // Read in the object data from the db and return the populated object
                var myClient = (Client) Read();
                if (myClient == null) return;

                // This is to populate these members that don't have these properties already
                DateCreated = String.IsNullOrEmpty(myClient.DateCreated.ToString(CultureInfo.CurrentCulture)) ? DateTime.UtcNow : myClient.DateCreated;
                DateModified = String.IsNullOrEmpty(myClient.DateModified.ToString(CultureInfo.CurrentCulture)) ? DateTime.UtcNow : myClient.DateModified;

                Enabled = myClient.Enabled;
                Name = myClient.Name;
                Relationships = myClient.Relationships;
                MessageProviders = myClient.MessageProviders;
                VerificationProviders = myClient.VerificationProviders;
                Organization = myClient.Organization;
                OtpSettings = myClient.OtpSettings;

                DocumentTemplates = myClient.DocumentTemplates;

                OpenAccessServicesEnabled = myClient.OpenAccessServicesEnabled;
                InMaintenance = myClient.InMaintenance;
                ClientAppUrl = myClient.ClientAppUrl;
                RegistrationCompletionUrl = ClientAppUrl + RegistrationCompletionUrl;
                AuthorizedDomain = myClient.AuthorizedDomain;
                AdEnabled = myClient.AdEnabled;
                EndUserAdOptOutEnabled = myClient.EndUserAdOptOutEnabled;

                AdProviders = myClient.AdProviders ?? new List<ProviderAdvertising>();
                AllowedIpList = myClient.AllowedIpList;

                OwnerLogoUrl = myClient.OwnerLogoUrl ?? Constants.Common.EmptyOwnerLogoUrl;

                if (myClient.SecureTradingSiteId == null)
                    SecureTradingSiteId = "";
                else
                    SecureTradingSiteId = myClient.SecureTradingSiteId;
            }
        }

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool AdEnabled { get; set; }
        public bool EndUserAdOptOutEnabled { get; set; }
        public string AuthorizedDomain { get; set; }
        public string ClientAppUrl { get; set; }
        public List<DocumentTemplate> DocumentTemplates { get; set; }
        public bool Enabled { get; set; }
        public bool InMaintenance { get; set; }
        public MessageProvider MessageProviders { get; set; }
        public bool OpenAccessServicesEnabled { get; set; }
        public Organization Organization { get; set; }
        public OtpSettings OtpSettings { get; set; }
        public string RegistrationCompletionUrl { get; set; }
        public List<VerificationProvider> VerificationProviders { get; set; }
        public List<ProviderAdvertising> AdProviders { get; set; }
        public string AllowedIpList { get; set; }
        public string OwnerLogoUrl { get; set; }
        public string SecureTradingSiteId { get; set; }
    }
}