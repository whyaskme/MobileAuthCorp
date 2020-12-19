using System;

namespace MACServices
{
    public class EndUserEvent
    {
        public EndUserEvent(string eventDetails)
        {
            Details = eventDetails;
            Date = DateTime.UtcNow;
        }

        public DateTime Date { get; set; }
        public string Details { get; set; }
    }
}