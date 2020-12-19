namespace MACServices
{
    public class HtmlTemplate
    {
        public HtmlTemplate(string templateType)
        {
            Type = 0;
            MessageDesc = templateType;
            MessageTitle = "";
            MessageKeywords = "";
            MessageDescription = "";
            MessageBody = "";
        }

        public int Type { get; set; }
        public string MessageDesc { get; set; }
        public string MessageTitle { get; set; }
        public string MessageKeywords { get; set; }
        public string MessageDescription { get; set; }
        public string MessageBody { get; set; }
    }
}