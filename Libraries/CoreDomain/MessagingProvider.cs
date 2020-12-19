using System.Collections.Generic;

namespace MACServices
{
    public class MessageProvider
    {
        public MessageProvider()
        {
            EmailProviders = new List<ProviderEmail>();
            SmsProviders = new List<ProviderSms>();
            VoiceProviders = new List<ProviderVoice>();
        }

        #region Base Messaging Properties

        public List<ProviderEmail> EmailProviders { get; set; }
        public List<ProviderSms> SmsProviders { get; set; }
        public List<ProviderVoice> VoiceProviders { get; set; }

        #endregion
    }
}