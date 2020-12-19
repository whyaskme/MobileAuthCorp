using System;
using MongoDB.Bson;

namespace MACBilling
{
    public class BillArchive : BillUtils
    {
        public BillArchive()
        {
            _id = ObjectId.GenerateNewId();
            ForMonthYear = DateTime.Now.Month + "/" + DateTime.Now.Year;
            DateCreated = DateTime.UtcNow;
            Amount = 0.00M;
            IsPaid = false;
        }

        //public BillArchive(string ClientId, string MonthYear)
        //{
        //    ReadBillForMonth(ClientId, MonthYear);
        //}

        public BillArchive(string billId)
        {
            var myBillArchive = (BillArchive)Read(billId, "BillArchive");

            if (myBillArchive != null)
            {
                ForMonthYear = myBillArchive.ForMonthYear;
                DateCreated = myBillArchive.DateCreated;
                DateDue = myBillArchive.DateDue;
                Amount = myBillArchive.Amount;
                DateSent = myBillArchive.DateSent;
                DatePaid = myBillArchive.DatePaid;
                DateVoided = myBillArchive.DateVoided;
                OwnerId = myBillArchive.OwnerId;
                OwnerName = myBillArchive.OwnerName;
                OwnerType = myBillArchive.OwnerType;
                CreatedById = myBillArchive.CreatedById;
                UpdatedById = myBillArchive.UpdatedById;
                IsPaid = myBillArchive.IsPaid;

                BillDetails = myBillArchive.BillDetails;
            }
        }

        public ObjectId _id { get; set; }

        public string ForMonthYear { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateDue { get; set; }
        public Decimal Amount { get; set; }

        public DateTime DateSent { get; set; }
        public DateTime DatePaid { get; set; }
        public DateTime DateVoided { get; set; }

        //public ObjectId OwnerId { get; set; }
        public String OwnerId { get; set; }
        public String OwnerName { get; set; }
        public String OwnerType { get; set; }

        public ObjectId CreatedById { get; set; }
        public ObjectId UpdatedById { get; set; }

        public Boolean IsPaid { get; set; }

        public string BillDetails { get; set; }
    }
}
