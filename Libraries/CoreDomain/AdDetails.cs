using System;

namespace MACServices
{
    public class AdDetails
    {
        public AdDetails()
        {
            Status = "New";
            AdServerUrl = "";
            OptOut = "";
            AdClientId = "";
            CampaignId = "";
            Age = "";
            City = "";
            Ethnicity = "";
            Gender = "";
            Homeowner = "";
            HouseholdIncome = "";
            MaritalStatus = "";
            SpecificKeywords = "";
            State = "";
            Type = "";
            ResponseStatus = "";
            EnterOTPAd = "";
            MessageAd = "";
            VerificationAd = "";
            ResponseTime = TimeSpan.Zero;
            Status = "";
            AdNumber = "";
            AdServerDomain = "";
        }
        // request data
        public string Status { get; set; }
        public string AdServerUrl { get; set; }
        public string OptOut { get; set; }
        public string AdClientId { get; set; }
        public string CampaignId { get; set; }
        public string AdNumber { get; set; }
        // user profile data (optional)
        public string Age { get; set; }
        public string City { get; set; }
        public string Ethnicity { get; set; }
        public string Gender { get; set; }
        public string Homeowner { get; set; }
        public string HouseholdIncome { get; set; }
        public string MaritalStatus { get; set; }
        public string SpecificKeywords { get; set; }
        public string State { get; set; }
        public string Type { get; set; }
        public string AdServerDomain { get; set; }
        // Response data
        public string ResponseStatus { get; set; }
        public TimeSpan ResponseTime { get; set; }
        public string EnterOTPAd { get; set; }
        public string MessageAd { get; set; }
        public string VerificationAd { get; set; }
    }
}
