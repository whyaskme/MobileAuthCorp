using System.Collections.Generic;

namespace MACServices
{
    public class DocumentTemplates
    {
        public DocumentTemplates()
        {
            Email = new List<EmailTemplate>();
            Html = new List<HtmlTemplate>();
            Sms = new List<SmsTemplate>();
            Voice = new List<VoiceTemplate>();
        }

        public List<EmailTemplate> Email { get; set; }
        public List<HtmlTemplate> Html { get; set; }
        public List<SmsTemplate> Sms { get; set; }
        public List<VoiceTemplate> Voice { get; set; }
    }
}