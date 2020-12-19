using System;

namespace TestLib
{
    public class AccountActivity
    {
        public AccountActivity()
        {
            Date = DateTime.UtcNow;
            Details = "";
        }
        public DateTime Date { get; set; }
        public string Details { get; set; }
    }
}