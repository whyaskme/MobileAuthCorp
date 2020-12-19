using System;
using MongoDB.Bson;

namespace TestLib
{
    public class BankSubAccount
    {
        public BankSubAccount()
        {
            _id = ObjectId.GenerateNewId();
            Enabled = true;
            DateCreated = DateTime.UtcNow;
            Type = false;
            Name = "";
            Number = "";
            Usage = "";
            SecurityCode = "";
            Balance = 0.00;
            BillingZipCode = "";
        }

        public ObjectId _id { get; set; }
        public bool Enabled { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime Expires { get; set; }
        public DateTime DateLastAccessed { get; set; }
        public bool Type { get; set; } // true=Credit, false=debit(can't go negitive)
        public String Institution { get; set; }
        public String Name { get; set; }
        public String Number { get; set; }
        public String Usage { get; set; }
        public String SecurityCode { get; set; }
        public Double Balance { get; set; }
        public Double Limit { get; set; } // Credit Limit, Debit maximum balance
        public String BillingZipCode { get; set; }
    }
}