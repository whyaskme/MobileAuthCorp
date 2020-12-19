namespace MACServices
{
    public class VoiceTemplate
    {
        public VoiceTemplate(string templateType)
        {
            Type = 0;
            MessageDesc = templateType;
            MessageVoiceText = "";
            MessageFromNumber = "1.480.939.2980";
            MessageFromName = "Mobile Authentication Corporation";
        }

        public int Type { get; set; }
        public string MessageDesc { get; set; }
        public string MessageVoiceText { get; set; }
        public string MessageFromNumber { get; set; }
        public string MessageFromName { get; set; }
    }
}