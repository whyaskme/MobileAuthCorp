namespace MACServices
{
    public class EmailTemplate
    {
        public EmailTemplate(string templateType)
        {
            Type = 0;
            MessageDesc = templateType;
            MessageSubject = "";
            MessageBody = "";
            MessageClosing = "";
            MessageFromEmail = "admin@mobileauthcorp.com";
            MessageFromName = "Mobile Authentication Corporation";
        }

        public int Type { get; set; }
        public string MessageDesc { get; set; }
        public string MessageSubject { get; set; }
        public string MessageBody { get; set; }
        public string MessageClosing { get; set; }
        public string MessageFromEmail { get; set; }
        public string MessageFromName { get; set; }
    }
}