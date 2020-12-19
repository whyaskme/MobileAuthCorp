using System;
using System.Collections.Generic;
using System.Globalization;

using MongoDB.Bson;

using MACSecurity;
using delim = MACServices.Constants.Common;

namespace MACServices
{
    public class PricingTierCharge
    {
        public PricingTierCharge(string ownerId)
        {
            ClientId = ownerId;

            AdSent = "0" + delim.KVSep + "0.05" + delim.ItemSep;

            AdClickThru = "0" + delim.KVSep + "0.05" + delim.ItemSep;
            AdUsed = "0" + delim.KVSep + "0.05" + delim.ItemSep;

            EndUserRegistration = "0" + delim.KVSep + "0.05" + delim.ItemSep;
            EndUserVerification = "0" + delim.KVSep + "0.05" + delim.ItemSep;

            OtpSent = "0" + delim.KVSep + "0.05" + delim.ItemSep;
        }

        public string ClientId { get; set; }

        private string adsent;
        public string AdSent { get { return adsent; } set { adsent = Security.EncryptAndEncode(value, ClientId); } }

        private string adclickthru;
        public string AdClickThru { get { return adclickthru; } set { adclickthru = Security.EncryptAndEncode(value, ClientId); } }

        private string adused;
        public string AdUsed { get { return adused; } set { adused = Security.EncryptAndEncode(value, ClientId); } }

        private string enduserregistration;
        public string EndUserRegistration { get { return enduserregistration; } set { enduserregistration = Security.EncryptAndEncode(value, ClientId); } }

        private string enduserverification;
        public string EndUserVerification { get { return enduserverification; } set { enduserverification = Security.EncryptAndEncode(value, ClientId); } }

        private string otpsent;
        public string OtpSent { get { return otpsent; } set { otpsent = Security.EncryptAndEncode(value, ClientId); } }
    }
}
