using cts = MACBilling.BillConstants;
namespace MACBilling
{
    public class BillPaymentNone : BillPaymentMethod
    {
        public BillPaymentNone()
        {
            Type = BillConstants.PaymentMethod.None;
        }
    }
}
