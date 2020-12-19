using System.Security.AccessControl;
using MongoDB.Bson;
using ot = MACOperationalTestLib.OperationalTestConstants;
using cs = MACServices.Constants.Strings;

namespace MACOperationalTestLib
{
    public class OperationalTestEmailSettings
    {
        public OperationalTestEmailSettings()
        {
            _id = ObjectId.GenerateNewId();
            _t = "OperationalTestEmailSettings";
            EmailPort = 25;
            eEmailHost = MACSecurity.Security.EncryptAndEncode("services.mobileauthcorp.com", cs.DefaultClientId);
            eEmailLoginPassword = MACSecurity.Security.EncryptAndEncode("jumP4000c", cs.DefaultClientId);
            eEmailLoginUserName = MACSecurity.Security.EncryptAndEncode("info@services.mobileauthcorp.com", cs.DefaultClientId);
            EmailFromAddress = "info@services.mobileauthcorp.com"; 
            EmailUseDefaultCredentials = true;
            EmailEnableSsl = false;
            EmailSubject = "Alert! Operational Test Failure on " + ot.ReplacementKeys.SUT;
            EmailBodyTemplate = "Test Run Results:|" + ot.ReplacementKeys.RESULTSSTATEMENT + "|Do not reply to this email.||Automated Test System" + ot.ReplacementKeys.TESTSYSTEM;
        }
        public ObjectId _id { get; set; }
        public string _t { get; set; }
        public int EmailPort { get; set; }
        public string eEmailHost { get; set; }
        public bool EmailEnableSsl { get; set; }
        public bool EmailUseDefaultCredentials { get; set; }
        public string eEmailLoginUserName { get; set; }
        public string eEmailLoginPassword { get; set; }
        public string EmailFromAddress { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBodyTemplate { get; set; }

    }
}
