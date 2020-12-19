using MongoDB.Bson;
using ot = MACOperationalTestLib.OperationalTestConstants;
using cs = MACServices.Constants.Strings;

namespace MACOperationalTestLib
{
    public class OperationalTestSmsSettings
    {
        public OperationalTestSmsSettings()
        {
            _id = ObjectId.GenerateNewId();
            _t = "OperationalTestSmsSettings";
            ProviderName = "Message Broadcast";
            URL = "http://ebmapi.messagebroadcast.com/webservice/ebm/pdc/addtorealtime/";
            eSid = MACSecurity.Security.EncryptAndEncode("5E1/1BD3A4805cE3c223e6AB3Ca7414+02089F86", cs.DefaultClientId);
            eAuthToken = MACSecurity.Security.EncryptAndEncode("B32EB51E57CC116F4BF7", cs.DefaultClientId);
            eBatchId = MACSecurity.Security.EncryptAndEncode("1530", cs.DefaultClientId);
            TextMessageTemplate = "Alert! Operational Test Failure on " + ot.ReplacementKeys.SUT;

        }
        public ObjectId _id { get; set; }
        public string _t { get; set; }
        public string ProviderName { get; set; }
        public string URL { get; set; }
        public string eSid { get; set; }
        public string eAuthToken { get; set; }
        public string eBatchId { get; set; }
        public string TextMessageTemplate { get; set; }
    }
}
