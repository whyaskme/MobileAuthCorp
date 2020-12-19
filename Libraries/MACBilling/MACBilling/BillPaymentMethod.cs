using System;
using cts = MACBilling.BillConstants;
using MongoDB.Bson;

namespace MACBilling
{
    public class BillPaymentMethod
    {
        public BillPaymentMethod()
        {
            _id = ObjectId.GenerateNewId();
            Type = BillConstants.PaymentMethod.None;

            DateCreated = DateTime.UtcNow;

            InstitutionName = "";

            RoutingNumber = "";
            AccountNumber = "";

            CardType = "";
            CardholderName = "";
            CardNumber = "";
            CCVNumber = "";
            Expires = "";

            BillingStreet1 = "";
            BillingStreet2 = "";
            BillingCity = "";
            BillingState = "";
            BillingZipCode = "";
        }

        public ObjectId _id { get; set; }
        public string Type { get; set; }

        public DateTime DateCreated { get; set; }
        public ObjectId CreatedById { get; set; }

        public string InstitutionName { get; set; }

        public string RoutingNumber { get; set; }
        public string AccountNumber { get; set; }

        public string CardType { get; set; }
        public string CardholderName { get; set; }
        public string CardNumber { get; set; }
        public string CCVNumber { get; set; }
        public string Expires { get; set; }

        public string BillingStreet1 { get; set; }
        public string BillingStreet2 { get; set; }
        public string BillingCity { get; set; }
        public string BillingState { get; set; }
        public string BillingZipCode { get; set; }
    }
}
