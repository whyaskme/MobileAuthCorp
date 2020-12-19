using System;

namespace MACServices
{
    public class Address
    {
        public Address()
        {
            Country = String.Empty;
            Street1 = String.Empty;
            Street2 = String.Empty;
            Unit = String.Empty;
            City = String.Empty;
            State = String.Empty;
            Zipcode = String.Empty;
        }

        public string Country { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string Unit { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }
    }
}