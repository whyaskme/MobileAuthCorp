using cts = MACBilling.BillConstants;
namespace MACBilling
{
    public class BillPaymentCreditCard : BillPaymentMethod
    {
        public BillPaymentCreditCard()
        {
            Type = BillConstants.PaymentMethod.CreditCard;
            //CardType = "";
            //CardholderName = "";
            //CardNumber = "";
            //CCVNumber = "";
            //Expires = "";

            //BillingStreet1 = "";
            //BillingStreet2 = "";
            //BillingCity = "";
            //BillingState = "";
            //BillingZipCode = "";
        }

        //public string CardType { get; set; }
        //public string CardholderName { get; set; }
        //public string CardNumber { get; set; }
        //public string CCVNumber { get; set; }
        //public string Expires { get; set; }

        //public string BillingStreet1 { get; set; }
        //public string BillingStreet2 { get; set; }
        //public string BillingCity { get; set; }
        //public string BillingState { get; set; }
        //public string BillingZipCode { get; set; }
    }
}
