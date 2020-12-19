using System;

namespace TestLib
{
    public class Bill
    {
        public Bill()
        {
            this.DateOfIssue=  DateTime.UtcNow;
            this.DateDueDate = DateTime.UtcNow.AddDays(15);
            this.Balance = 0.00;
            this.Status = "";
            this.Description = "";
            this.Details = "";
            this.Name = "";
            this.BusinessType = "";
        }
        public int InvoiceNumber { get; set; }
        public string Name { get; set; }
        public string BusinessType { get; set; }
        public DateTime DateOfIssue { get; set; }
        public DateTime DateDueDate { get; set; }
        public DateTime DateOfPayment { get; set; }
        public Double Balance { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
    }
}