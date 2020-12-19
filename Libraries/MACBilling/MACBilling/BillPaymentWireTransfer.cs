using cts = MACBilling.BillConstants;
namespace MACBilling
{
    public class BillPaymentWireTransfer : BillPaymentMethod
    {
        public BillPaymentWireTransfer()
        {
            Type = BillConstants.PaymentMethod.WireTransfer;
            //RoutingNumber = "";
            //AccountNumber = "";
        }

        //public string RoutingNumber { get; set; }
        //public string AccountNumber { get; set; }
    }
}
