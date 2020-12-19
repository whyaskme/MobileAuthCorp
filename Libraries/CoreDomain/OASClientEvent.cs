using System;

namespace MACServices
{
    public class OasClientEvent
    {
        public OasClientEvent(string eventDetails)
        {
            Details = eventDetails;
            Date = DateTime.UtcNow;
        }

        public DateTime Date { get; set; }
        public string Details { get; set; }
    }
}