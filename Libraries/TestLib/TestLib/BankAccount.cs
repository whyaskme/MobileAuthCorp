using System;
using System.Collections.Generic;
using MongoDB.Bson;

//using MACServices;

namespace TestLib
{
    public class BankAccount
    {
        public BankAccount(string pAccNum, string pPan)
        {
            _id = ObjectId.GenerateNewId();
            _t = "Account";
            Type = "";
            Name = "";
            Prefix = "";
            FirstName = "";
            MiddleName = "";
            LastName = "";
            Email = "";
            MobilePhone = "";
            LoginName = "";
            Assigned = true;
            Assigned = false;
            AccountNumber = pAccNum;
            PAN = pPan;
            Balance = 0.00;
            DateCreated = DateTime.UtcNow;
            Address = new Address();
            SubAccounts = new List<BankSubAccount>();
            Bills = new List<Bill>();
            ActivityLog = new List<AccountActivity>();
        }

        public ObjectId _id { get; set; }
        public string _t { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Prefix { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string LoginName { get; set; }
        public string Email { get; set; }
        public string MobilePhone { get; set; }
        public Address Address { get; set; }
        public string Type { get; set; }
        public bool Assigned { get; set; }
        public bool Hold { get; set; }
        public string AccountNumber { get; set; }
        public string PAN { get; set; }
        public Double Balance { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public DateTime DateAssigned { get; set; }
        public List<BankSubAccount> SubAccounts { get; set; }
        public List<Bill> Bills { get; set; }
        public List<AccountActivity> ActivityLog { get; set; }
    }
}