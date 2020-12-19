namespace MACServices
{
    public class UserVerificationLexisNexis : VerificationProvider
    {
        public UserVerificationLexisNexis()
        {
            Name = "Lexis Nexis";
            ApiKey = "n/a";
            ApiVersion = "1.65";
            BaseUrl = "https://wsonline.seisint.com/WsIdentity/InstantID";
            Login = "MACXML";
            Password = "up83525H";
            Protocol = "SOAP";
            ProxyHost = "http://proxy.mobileauthcorp.com";
            ProxyPort = "80";
            RequestParameters = "GLBPurpose=5|DLPurpose=3|UseDOBFilter=1|DOBRadius=3";
            ProcessingParameters = "";
        }
    }
}