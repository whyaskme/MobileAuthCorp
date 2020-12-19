namespace MACServices
{
    public class Contact
    {
        public Contact()
        {
            Email = string.Empty;
            HomePhone = string.Empty;
            WorkPhone = string.Empty;
            WorkExtension = string.Empty;
            MobilePhone = string.Empty;
        }

        public string Email { get; set; }
        public string HomePhone { get; set; }
        public string WorkPhone { get; set; }
        public string WorkExtension { get; set; }
        public string MobilePhone { get; set; }
    }
}