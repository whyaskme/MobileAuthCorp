using System;
using System.Collections.Generic;
using MongoDB.Bson;
namespace MACBilling
{
    public class BillBase : BillUtils
    {
        public BillBase()
        {
            _id = ObjectId.GenerateNewId();
            CreatedById = ObjectId.GenerateNewId();

            //OwnerId = ObjectId.Parse(MACServices.Constants.Strings.DefaultEmptyObjectId);
            OwnerId = MACServices.Constants.Strings.DefaultEmptyObjectId;
            OwnerName = "";
            OwnerType = "";

            DateStart = DateTime.UtcNow;
            DateEnd = DateTime.UtcNow;

            DateCreated = DateTime.UtcNow;
            DateDue = DateTime.UtcNow;
            DateSent = DateTime.UtcNow;

            IsPaid = false;

            AdMessageSentCount = 0;
            AdMessageSentCost = 0.00M; // 'M' suffix defines Decimal data type
            AdMessageSentAmount = 0.00M; // 'M' suffix defines Decimal data type

            AdEnterOtpScreenSentCount = 0;
            AdEnterOtpScreenSentCost = 0.00M; // 'M' suffix defines Decimal data type
            AdEnterOtpScreenSentAmount = 0.00M; // 'M' suffix defines Decimal data type

            AdVerificationScreenSentCount = 0;
            AdVerificationScreenSentCost = 0.00M; // 'M' suffix defines Decimal data type
            AdVerificationScreenSentAmount = 0.00M; // 'M' suffix defines Decimal data type

            OtpSentEmailCount = 0;
            OtpSentEmailCost = 0.00M; // 'M' suffix defines Decimal data type
            OtpSentEmailAmount = 0.00M; // 'M' suffix defines Decimal data type

            OtpSentSmsCount = 0;
            OtpSentSmsCost = 0.00M; // 'M' suffix defines Decimal data type
            OtpSentSmsAmount = 0.00M; // 'M' suffix defines Decimal data type

            OtpSentVoiceCount = 0;
            OtpSentVoiceCost = 0.00M; // 'M' suffix defines Decimal data type
            OtpSentVoiceAmount = 0.00M; // 'M' suffix defines Decimal data type

            EndUserRegistrationCount = 0;
            EndUserRegistrationCost = 0.00M; // 'M' suffix defines Decimal data type
            EndUserRegistrationAmount = 0.00M; // 'M' suffix defines Decimal data type

            EndUserVerificationCount = 0;
            EndUserVerificationCost = 0.00M; // 'M' suffix defines Decimal data type
            EndUserVerificationAmount = 0.00M; // 'M' suffix defines Decimal data type

            Addendums = new List<BillAddendum>();
            AddendumsAmount = 0.00M; // 'M' suffix defines Decimal data type

            SubTotal = 0.00M; // 'M' suffix defines Decimal data type
            TaxRate = 0.00M; // 'M' suffix defines Decimal data type
            SalesTax = 0.00M; // 'M' suffix defines Decimal data type
            Total = 0.00M; // 'M' suffix defines Decimal data type
        }

        public ObjectId _id { get; set; }
        public ObjectId CreatedById { get; set; }
        public ObjectId UpdatedById { get; set; }

        //public ObjectId OwnerId { get; set; }
        public string OwnerId { get; set; }
        public string OwnerName { get; set; }
        public string OwnerType { get; set; }

        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateDue { get; set; }
        public DateTime DateSent { get; set; }
        public DateTime DatePaid { get; set; }
        public DateTime DateVoided { get; set; }

        public Boolean IsPaid { get; set; }

        public Int64 AdMessageSentCount { get; set; }
        public Decimal AdMessageSentCost { get; set; }
        public Decimal AdMessageSentAmount { get; set; }

        public Int64 AdEnterOtpScreenSentCount { get; set; }
        public Decimal AdEnterOtpScreenSentCost { get; set; }
        public Decimal AdEnterOtpScreenSentAmount { get; set; }

        public Int64 AdVerificationScreenSentCount { get; set; }
        public Decimal AdVerificationScreenSentCost { get; set; }
        public Decimal AdVerificationScreenSentAmount { get; set; }

        public Int64 OtpSentEmailCount { get; set; }
        public Decimal OtpSentEmailCost { get; set; }
        public Decimal OtpSentEmailAmount { get; set; }

        public Int64 OtpSentSmsCount { get; set; }
        public Decimal OtpSentSmsCost { get; set; }
        public Decimal OtpSentSmsAmount { get; set; }

        public Int64 OtpSentVoiceCount { get; set; }
        public Decimal OtpSentVoiceCost { get; set; }
        public Decimal OtpSentVoiceAmount { get; set; }

        public Int64 EndUserRegistrationCount { get; set; }
        public Decimal EndUserRegistrationCost { get; set; }
        public Decimal EndUserRegistrationAmount { get; set; }

        public Int64 EndUserVerificationCount { get; set; }
        public Decimal EndUserVerificationCost { get; set; }
        public Decimal EndUserVerificationAmount { get; set; }

        public List<BillAddendum> Addendums { get; set; }
        public Decimal AddendumsAmount { get; set; }

        public Decimal SubTotal { get; set; }
        public Decimal TaxRate { get; set; }
        public Decimal SalesTax { get; set; }
        public Decimal Total { get; set; }
    }
}
