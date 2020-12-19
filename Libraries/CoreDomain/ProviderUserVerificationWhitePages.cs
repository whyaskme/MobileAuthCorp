namespace MACServices
{
    public class UserVerificationWhitePages : VerificationProvider
    {
        public UserVerificationWhitePages()
        {
            Name = "White Pages";
            ApiKey = "defa7058bc71ada1b60000216700c1d2";
            ApiVersion = "1.1";
            BaseUrl = "http://proapi.whitepages.com";
            SearchType = "reverse_phone";
            ProxyHost = "http://proxy.mobileauthcorp.com";
            ProxyPort = "80";
        }
    }
}