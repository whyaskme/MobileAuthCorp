using cts = MACBilling.BillConstants;
namespace MACBilling
{
    public class BillPaymentGateway
    {
        public BillPaymentGateway()
        {
            GatewayName = "";
            ApiVersion = "";
            ApiKey = "";
            LoginUsername = "";
            LoginPassword = "";
            Protocol = "";
            RequiresSsl = false;
            Url = "";
        }

        public string GatewayName { get; set; }
        public string ApiVersion { get; set; }
        public string ApiKey { get; set; }
        public string LoginUsername { get; set; }
        public string LoginPassword { get; set; }        
        public string Protocol { get; set; }
        public bool RequiresSsl { get; set; }
        public string Url { get; set; }
    }
}
