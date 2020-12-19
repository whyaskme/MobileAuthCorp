using cts = MACBilling.BillConstants;
namespace MACBilling
{
    public class BillPaymentManualCheck : BillPaymentMethod
    {
        public BillPaymentManualCheck()
        {
            Type = BillConstants.PaymentMethod.ManualCheck;
        }
    }
}
