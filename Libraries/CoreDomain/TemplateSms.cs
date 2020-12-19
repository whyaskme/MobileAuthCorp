namespace MACServices
{
    public class SmsTemplate
    {
        public SmsTemplate(string templateType)
        {
            Type = 0;
            MessageDesc = templateType;
            MessageHeader = "";
            MessageLabel = "";
            MessageText = "";
            MessageTrailer = "";
        }

        public int Type { get; set; }
        public string MessageDesc { get; set; }
        public string MessageHeader { get; set; }
        public string MessageLabel { get; set; }
        public string MessageText { get; set; }
        public string MessageTrailer { get; set; }
    }
}