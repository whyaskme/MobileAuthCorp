using cts = MACBilling.BillConstants;
namespace MACBilling
{
    public class BillPaymentACH : BillPaymentMethod
    {
        public BillPaymentACH()
        {
            Type = BillConstants.PaymentMethod.ACH;
            //RoutingNumber = "";
            //AccountNumber = "";
        }

        //public string RoutingNumber { get; set; }
        //public string AccountNumber { get; set; }
    }
}
