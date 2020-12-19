using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using MACServices;

public partial class UserControls_BillingRender : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public string BillContainerId { get { return divBillDocumentContainer.ID; } set { divBillDocumentContainer.ID = value; } }

    //public string ClientIdLabel { get { return divClientIdLabel.InnerHtml; } set { divClientIdLabel.InnerHtml = value; } }
    public string ClientNameLabel { get { return divClientNameLabel.InnerHtml; } set { divClientNameLabel.InnerHtml = value; } }

    public string ClientLogo { get { return imgOwnerLogo.Src; } set { imgOwnerLogo.Src = value; } }

    public string ClientMemberNumber { get { return txtClientMemberNumber.InnerHtml; } set { txtClientMemberNumber.InnerHtml = value; } }

    public string IncludedInGroupBillLabel { get { return spanIncludedInGroupBillLabel.InnerHtml; } set { spanIncludedInGroupBillLabel.InnerHtml = value; } }

    public string BillNumber { get { return txtBillNumber.InnerHtml; } set { txtBillNumber.InnerHtml = value; } }

    public string DateCreated { get { return txtDateCreated.InnerHtml; } set { txtDateCreated.InnerHtml = value; } }
    public string DateDue { get { return txtDateDue.InnerHtml; } set { txtDateDue.InnerHtml = value; } }
    public string DateSent { get { return txtDateSent.InnerHtml; } set { txtDateSent.InnerHtml = value; } }
    public string DatePaid { get { return txtDatePaid.InnerHtml; } set { txtDatePaid.InnerHtml = value; } }

    // Form labels
    public string LegendMessagingCost { get { return legendMessagingCost.InnerHtml; } set { legendMessagingCost.InnerHtml = value; } }
    public string LegendAdsCost { get { return legendAdsCost.InnerHtml; } set { legendAdsCost.InnerHtml = value; } }
    public string LegendEndUserCost { get { return legendEndUserCost.InnerHtml; } set { legendEndUserCost.InnerHtml = value; } }

    public string LegendTaxRate { get { return legendTaxRate.InnerHtml; } set { legendTaxRate.InnerHtml = value; } }

    // Client Info
    public new string ClientID { get { return txtClientID.InnerHtml; } set { txtClientID.InnerHtml = value; } }
    public string ClientName { get { return txtClientName.InnerHtml; } set { txtClientName.InnerHtml = value; } }
    public string TaxId { get { return txtTaxId.InnerHtml; } set { txtTaxId.InnerHtml = value; } }
    public string Street1 { get { return txtStreet1.InnerHtml; } set { txtStreet1.InnerHtml = value; } }
    public string Street2 { get { return txtStreet2.InnerHtml; } set { txtStreet2.InnerHtml = value; } }
    public string City { get { return txtCity.InnerHtml; } set { txtCity.InnerHtml = value; } }
    public string State { get { return txtState.InnerHtml; } set { txtState.InnerHtml = value; } }
    public string ZipCode { get { return txtZipCode.InnerHtml; } set { txtZipCode.InnerHtml = value; } }
    public string Phone { get { return txtPhone.InnerHtml; } set { txtPhone.InnerHtml = value; } }

    // Otp Charges
    public string OtpEmailCount { get { return spanOtpEmailCount.InnerHtml; } set { spanOtpEmailCount.InnerHtml = value; } }
    public string OtpEmailCost { get { return spanOtpEmailCost.InnerHtml; } set { spanOtpEmailCost.InnerHtml = value; } }
    public string OtpEmailCharge { get { return spanOtpEmailCharge.InnerHtml; } set { spanOtpEmailCharge.InnerHtml = value; } }
    public string OtpSmsCount { get { return spanOtpSmsCount.InnerHtml; } set { spanOtpSmsCount.InnerHtml = value; } }
    public string OtpSmsCost { get { return spanOtpSmsCost.InnerHtml; } set { spanOtpSmsCost.InnerHtml = value; } }
    public string OtpSmsCharge { get { return spanOtpSmsCharge.InnerHtml; } set { spanOtpSmsCharge.InnerHtml = value; } }
    public string OtpVoiceCount { get { return spanOtpVoiceCount.InnerHtml; } set { spanOtpVoiceCount.InnerHtml = value; } }
    public string OtpVoiceCost { get { return spanOtpVoiceCost.InnerHtml; } set { spanOtpVoiceCost.InnerHtml = value; } }
    public string OtpVoiceCharge { get { return spanOtpVoiceCharge.InnerHtml; } set { spanOtpVoiceCharge.InnerHtml = value; } }
    public string OtpTotal { get { return spanOtpTotal.InnerHtml; } set { spanOtpTotal.InnerHtml = value; } }

    // Advertising Charges
    public string AdMessageSentCount { get { return spanAdMessageSentCount.InnerHtml; } set { spanAdMessageSentCount.InnerHtml = value; } }
    public string AdMessageSentCost { get { return spanAdMessageSentCost.InnerHtml; } set { spanAdMessageSentCost.InnerHtml = value; } }
    public string AdMessageSentAmount { get { return spanAdMessageSentAmount.InnerHtml; } set { spanAdMessageSentAmount.InnerHtml = value; } }
    public string AdEnterOtpScreenSentCount { get { return spanAdEnterOtpScreenSentCount.InnerHtml; } set { spanAdEnterOtpScreenSentCount.InnerHtml = value; } }
    public string AdEnterOtpScreenSentCost { get { return spanAdEnterOtpScreenSentCost.InnerHtml; } set { spanAdEnterOtpScreenSentCost.InnerHtml = value; } }
    public string AdEnterOtpScreenSentAmount { get { return spanAdEnterOtpScreenSentAmount.InnerHtml; } set { spanAdEnterOtpScreenSentAmount.InnerHtml = value; } }
    public string AdVerificationScreenSentCount { get { return spanAdVerificationScreenSentCount.InnerHtml; } set { spanAdVerificationScreenSentCount.InnerHtml = value; } }
    public string AdVerificationScreenSentCost { get { return spanAdVerificationScreenSentCost.InnerHtml; } set { spanAdVerificationScreenSentCost.InnerHtml = value; } }
    public string AdVerificationScreenSentAmount { get { return spanAdVerificationScreenSentAmount.InnerHtml; } set { spanAdVerificationScreenSentAmount.InnerHtml = value; } }
    public string AdPassTotal { get { return spanAdPassTotal.InnerHtml; } set { spanAdPassTotal.InnerHtml = value; } }

    // End User Charges
    public string EndUserRegistrationCount { get { return spanEndUserRegistrationCount.InnerHtml; } set { spanEndUserRegistrationCount.InnerHtml = value; } }
    public string EndUserRegistrationCost { get { return spanEndUserRegistrationCost.InnerHtml; } set { spanEndUserRegistrationCost.InnerHtml = value; } }
    public string EndUserRegistrationAmount { get { return spanEndUserRegistrationAmount.InnerHtml; } set { spanEndUserRegistrationAmount.InnerHtml = value; } }
    public string EndUserVerificationCount { get { return spanEndUserVerificationCount.InnerHtml; } set { spanEndUserVerificationCount.InnerHtml = value; } }
    public string EndUserVerificationCost { get { return spanEndUserVerificationCost.InnerHtml; } set { spanEndUserVerificationCost.InnerHtml = value; } }
    public string EndUserVerificationAmount { get { return spanEndUserVerificationAmount.InnerHtml; } set { spanEndUserVerificationAmount.InnerHtml = value; } }
    public string EndUserTotal { get { return spanEndUserTotal.InnerHtml; } set { spanEndUserTotal.InnerHtml = value; } }

    // Miscellaneous Charges
    public bool ShowMiscCharges { get { return divMiscellaneousBilling.Visible; } set { divMiscellaneousBilling.Visible = value; } }
    public string AddendumsContainer { get { return divAddendumsContainer.InnerHtml; } set { divAddendumsContainer.InnerHtml = value; } }
    public string MiscTotal { get { return spanMiscTotal.InnerHtml; } set { spanMiscTotal.InnerHtml = value; } }

    // SubTotal, Tax Rate, Taxes and Total
    public string Subtotal { get { return spanSubtotal.InnerHtml; } set { spanSubtotal.InnerHtml = value; } }
    public string TaxRate { get { return spanTaxRate.InnerHtml; } set { spanTaxRate.InnerHtml = value; } }
    public string SalesTax { get { return spanSalesTax.InnerHtml; } set { spanSalesTax.InnerHtml = value; } }
    public string Total { get { return spanTotal.InnerHtml; } set { spanTotal.InnerHtml = value; } }

    // Foot Notes
    public bool ShowFootNotes { get { return divClientSummaryContainer.Visible; } set { divClientSummaryContainer.Visible = value; } }
    public string FootNotes { get { return divClientSummary.InnerHtml; } set { divClientSummary.InnerHtml = value; } }

    public string PageNumber { get { return divPageNumber.InnerHtml; } set { divPageNumber.InnerHtml = value; } }
}