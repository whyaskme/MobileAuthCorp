using System;

namespace MACServices
{
    public class OtpSettings
    {
        public OtpSettings()
        {
            Length = 6;
            Header = String.Empty;
            Characterset = "0123456789";
            Label = String.Empty;
            MaxRetries = 3;
            Trailer = String.Empty;
            ProviderList = String.Empty;
            Timeout = 2;
            EndUserRegistrationText = "Your registration Otp";
        }

        public int Length { get; set; }
        public string Header { get; set; }
        public string Characterset { get; set; }
        public string Label { get; set; }
        public string Trailer { get; set; }
        public int MaxRetries { get; set; }
        public int Timeout { get; set; }
        public string ProviderList { get; set; }
        public string EndUserRegistrationText { get; set; }
    }
}