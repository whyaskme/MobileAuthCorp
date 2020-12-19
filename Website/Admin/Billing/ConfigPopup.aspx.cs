using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using MACServices;
using cs = MACServices.Constants.Strings;

using MongoDB.Bson;

namespace MACBilling
{
    public partial class ConfigPopup : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var loggedInAdminId = "";
            if (!String.IsNullOrEmpty(HttpContext.Current.Request["loggedInAdminId"]))
                loggedInAdminId = HttpContext.Current.Request["loggedInAdminId"];

            loggedInAdminId = MACSecurity.Security.DecodeAndDecrypt(loggedInAdminId, Constants.Strings.DefaultClientId);

            var ownerId = "";
            if (!String.IsNullOrEmpty(HttpContext.Current.Request["ownerId"]))
                ownerId = HttpContext.Current.Request["ownerId"];

            hiddenOwnerId.Value = ownerId;

            var configType = "";
            if (!String.IsNullOrEmpty(HttpContext.Current.Request["configType"]))
                configType = HttpContext.Current.Request["configType"];

            var userIsReadOnly = "";
            if (!String.IsNullOrEmpty(Request["userisreadonly"]))
                userIsReadOnly = Request["userisreadonly"];

            userIsReadOnly = MACSecurity.Security.DecodeAndDecrypt(userIsReadOnly, loggedInAdminId);

            hiddenOwnerType.Value = configType;

            chkIncludeInGroupBill.Enabled = false;

            BillPaymentACH myACHPaymentMethod;
            BillPaymentCreditCard myCreditCardPaymentMethod;
            BillPaymentManualCheck myManualCheckPaymentMethod;
            BillPaymentWireTransfer myWireTransferPaymentMethod;

            Client myClient = new Client("");
            Group myGroup = new Group("");

            BillConfig myBillConfig;

            try
            {
                billingAssignmentsFrame.Src = "/Admin/Billing/UserPopupAssignment.aspx?ownerId=" + ownerId + "&ownerType=" + configType;

                myBillConfig = new BillConfig(ownerId, configType, loggedInAdminId);

                // Check to see if the client is a group member. If so, enable the checkbox to allow include in group billing.
                chkIncludeInGroupBill.Enabled = false;

                var GroupMembershipCount = 0;

                //divSettingDisabledText.Visible = false;

                switch (configType)
                {
                    case "Client":
                        ArrayList myAssignedGroups = new ArrayList();

                        myClient = new Client(ownerId);
                        spanBillingConfig.InnerHtml = myClient.Name + " (<span style='color: #ff0000;'>Client</span>) Billing Configuration";
                        foreach (var currRelationship in myClient.Relationships)
                        {
                            if (currRelationship.MemberType == "Group")
                            {
                                Group currentGroup = new Group(currRelationship.MemberId.ToString());

                                myAssignedGroups.Add(currentGroup.Name + "|" + currentGroup._id.ToString());

                                chkIncludeInGroupBill.Enabled = true;
                                spanIncludeInBillGroup.Style.Add("color", "#000000");
                                GroupMembershipCount++;
                                continue;
                            }
                        }
                        myAssignedGroups.Sort();
                        foreach (var currentGroup in myAssignedGroups)
                        {
                            var tmpVal = currentGroup.ToString().Split('|');

                            ListItem groupItem = new ListItem();
                            groupItem.Text = tmpVal[0];
                            groupItem.Value = tmpVal[1];

                            if (groupItem.Value == myBillConfig.BillToGroupId.ToString())
                                groupItem.Selected = true;

                            dlGroupList.Items.Add(groupItem);
                        }
                        break;

                    case "Group":
                        myGroup = new Group(ownerId);
                        spanBillingConfig.InnerHtml = myGroup.Name + " (<span style='color: #ff0000;'>Group</span>) Billing Configuration";

                        hiddenOwnerType.Value = "Group";
                        //divSettingDisabledText.Visible = true;
                        break;
                }

                if (IsPostBack)
                {
                    myBillConfig = new BillConfig(ownerId, configType, loggedInAdminId);

                    myBillConfig.DateUpdated = DateTime.UtcNow;
                    myBillConfig.UpdatedById = ObjectId.Parse(loggedInAdminId);

                    // Group billing has been disabled and need to reset config property
                    if (!chkIncludeInGroupBill.Checked)
                    {
                        myBillConfig.BillToGroupId = ObjectId.Parse(Constants.Strings.DefaultEmptyObjectId);
                        myBillConfig.IncludeInGroupBill = false;
                    }
                    else
                    {
                        if (configType == "Client")
                            myBillConfig.IncludeInGroupBill = chkIncludeInGroupBill.Checked;

                        if (dlGroupList.SelectedIndex > -1)
                            myBillConfig.BillToGroupId = ObjectId.Parse(dlGroupList.SelectedValue);
                    }

                    if (txtMinimumOtpCharge.Text == "")
                        txtMinimumOtpCharge.Text = "0.00";

                    if (txtMinimumAdCharge.Text == "")
                        txtMinimumAdCharge.Text = "0.00";

                    if (txtMinimumEndUserRegistrationCharge.Text == "")
                        txtMinimumEndUserRegistrationCharge.Text = "0.00";

                    if (textMonthlyServiceCharge.Text == "")
                        textMonthlyServiceCharge.Text = "0.00";

                    if (txtExternallySettledBy.Text == "")
                        txtExternallySettledBy.Text = "Not specified";

                    if (txtTaxRate.Text == "")
                        txtTaxRate.Text = "0.00";

                    myBillConfig.MinimumOtpCharge = MACSecurity.Security.EncryptAndEncode(txtMinimumOtpCharge.Text, ownerId);
                    myBillConfig.MinimumAdCharge = MACSecurity.Security.EncryptAndEncode(txtMinimumAdCharge.Text, ownerId);
                    myBillConfig.MinimumEndUserRegistrationCharge = MACSecurity.Security.EncryptAndEncode(txtMinimumEndUserRegistrationCharge.Text, ownerId);
                    myBillConfig.MonthlyServiceCharge = MACSecurity.Security.EncryptAndEncode(textMonthlyServiceCharge.Text, ownerId);
                    myBillConfig.ExternallySettledBy = MACSecurity.Security.EncryptAndEncode(txtExternallySettledBy.Text, ownerId);
                    myBillConfig.TaxRate = MACSecurity.Security.EncryptAndEncode(txtTaxRate.Text, ownerId);

                    myBillConfig.BillingCycle = dlBillingCycle.SelectedValue;
                    myBillConfig.BillingTerms = dlBillingTerms.SelectedValue;

                    #region Payment Gateway

                        myBillConfig.PaymentGateway.RequiresSsl = chkRequiresSsl.Checked;
                        myBillConfig.PaymentGateway.GatewayName = MACSecurity.Security.EncryptAndEncode(txtGatewayName.Text, ownerId);
                        myBillConfig.PaymentGateway.ApiVersion = MACSecurity.Security.EncryptAndEncode(txtApiVersion.Text, ownerId);
                        myBillConfig.PaymentGateway.ApiKey = MACSecurity.Security.EncryptAndEncode(txtApiKey.Text, ownerId);
                        myBillConfig.PaymentGateway.LoginUsername = MACSecurity.Security.EncryptAndEncode(txtLoginUsername.Text, ownerId);
                        myBillConfig.PaymentGateway.LoginPassword = MACSecurity.Security.EncryptAndEncode(txtLoginPassword.Text, ownerId);
                        myBillConfig.PaymentGateway.Protocol = MACSecurity.Security.EncryptAndEncode(txtProtocol.Text, ownerId);
                        myBillConfig.PaymentGateway.Url = MACSecurity.Security.EncryptAndEncode(txtUrl.Text, ownerId);

                    #endregion

                    #region Payment Methods

                        switch (dlPaymentMethod.SelectedIndex)
                        {
                            case 0:
                                myBillConfig.PaymentMethod = new BillPaymentNone();
                                break;

                            case 1:
                                myACHPaymentMethod = new BillPaymentACH();
                                myACHPaymentMethod.CreatedById = ObjectId.Parse(loggedInAdminId);
                                myACHPaymentMethod.AccountNumber = MACSecurity.Security.EncryptAndEncode(txtACHAccountNumber.Text, ownerId);
                                myACHPaymentMethod.InstitutionName = MACSecurity.Security.EncryptAndEncode(txtACHInstitutionName.Text, ownerId);
                                myACHPaymentMethod.RoutingNumber = MACSecurity.Security.EncryptAndEncode(txtACHRoutingNumber.Text, ownerId);

                                myBillConfig.PaymentMethod = myACHPaymentMethod;
                                break;

                            case 2:
                                myCreditCardPaymentMethod = new BillPaymentCreditCard();
                                myCreditCardPaymentMethod.CardType = MACSecurity.Security.EncryptAndEncode(dlCreditCardType.SelectedItem.Text, ownerId);
                                myCreditCardPaymentMethod.CreatedById = ObjectId.Parse(loggedInAdminId);

                                myCreditCardPaymentMethod.CardholderName = MACSecurity.Security.EncryptAndEncode(txtCardHolderName.Text, ownerId);
                                myCreditCardPaymentMethod.CardNumber = MACSecurity.Security.EncryptAndEncode(txtCardNumber.Text, ownerId);
                                myCreditCardPaymentMethod.CCVNumber = MACSecurity.Security.EncryptAndEncode(txtCCVNumber.Text, ownerId);
                                myCreditCardPaymentMethod.Expires = MACSecurity.Security.EncryptAndEncode(txtCardExpires.Text, ownerId);

                                myCreditCardPaymentMethod.BillingStreet1 = MACSecurity.Security.EncryptAndEncode(txtBillingStreet1.Text, ownerId);
                                myCreditCardPaymentMethod.BillingStreet2 = MACSecurity.Security.EncryptAndEncode(txtBillingStreet2.Text, ownerId);
                                myCreditCardPaymentMethod.BillingCity = MACSecurity.Security.EncryptAndEncode(txtBillingCity.Text, ownerId);
                                myCreditCardPaymentMethod.BillingZipCode = MACSecurity.Security.EncryptAndEncode(txtBillingZipcode.Text, ownerId);

                                myCreditCardPaymentMethod.BillingState = dlStates.SelectedValue;

                                myBillConfig.PaymentMethod = myCreditCardPaymentMethod;
                                break;

                            case 3:
                                myManualCheckPaymentMethod = new BillPaymentManualCheck();
                                myManualCheckPaymentMethod.CreatedById = ObjectId.Parse(loggedInAdminId);
                                myBillConfig.PaymentMethod = myManualCheckPaymentMethod;
                                break;

                            case 4:
                                myWireTransferPaymentMethod = new BillPaymentWireTransfer();
                                myWireTransferPaymentMethod.CreatedById = ObjectId.Parse(loggedInAdminId);
                                myWireTransferPaymentMethod.AccountNumber = MACSecurity.Security.EncryptAndEncode(txtWireInstitutionName.Text, ownerId);
                                myWireTransferPaymentMethod.InstitutionName = MACSecurity.Security.EncryptAndEncode(txtWireRoutingNumber.Text, ownerId);
                                myWireTransferPaymentMethod.RoutingNumber = MACSecurity.Security.EncryptAndEncode(txtAccountNumber.Text, ownerId);

                                myBillConfig.PaymentMethod = myWireTransferPaymentMethod;
                                break;

                            default:
                                BillPaymentACH myDefaultPaymentMethod = new BillPaymentACH();
                                myDefaultPaymentMethod.CreatedById = ObjectId.Parse(loggedInAdminId);
                                myDefaultPaymentMethod.AccountNumber = MACSecurity.Security.EncryptAndEncode(txtACHAccountNumber.Text, ownerId);
                                myDefaultPaymentMethod.InstitutionName = MACSecurity.Security.EncryptAndEncode(txtACHInstitutionName.Text, ownerId);
                                myDefaultPaymentMethod.RoutingNumber = MACSecurity.Security.EncryptAndEncode(txtACHRoutingNumber.Text, ownerId);

                                myBillConfig.PaymentMethod = myDefaultPaymentMethod;
                                break;
                        }

                    #endregion

                    #region Pricing Tiers

                        if (!radioAdSimpleCharge.Checked)
                            myBillConfig.ItemizedAdvertisingCharges = true;
                        else
                            myBillConfig.ItemizedAdvertisingCharges = false;

                        // Clear the existing members and repopulate from the form
                        List<BillTier> myNewTiers = new List<BillTier>();

                        BillTier myTier = new BillTier(ownerId, "OtpSentEmail", OtpSentEmail.Text);
                        myNewTiers.Add(myTier);

                        myTier = new BillTier(ownerId, "OtpSentSms", OtpSentSms.Text);
                        myNewTiers.Add(myTier);

                        myTier = new BillTier(ownerId, "OtpSentVoice", OtpSentVoice.Text);
                        myNewTiers.Add(myTier);

                        myTier = new BillTier(ownerId, "AdMessageSent", AdMessageSent.Text);
                        myNewTiers.Add(myTier);

                        myTier = new BillTier(ownerId, "AdEnterOtpScreenSent", AdEnterOtpScreenSent.Text);
                        myNewTiers.Add(myTier);

                        myTier = new BillTier(ownerId, "AdVerificationScreenSent", AdVerificationScreenSent.Text);
                        myNewTiers.Add(myTier);

                        myTier = new BillTier(ownerId, "EndUserRegister", EndUserRegister.Text);
                        myNewTiers.Add(myTier);

                        myTier = new BillTier(ownerId, "EndUserVerify", EndUserVerify.Text);
                        myNewTiers.Add(myTier);

                        myBillConfig.BillingTiers = myNewTiers;

                    #endregion

                    #region Organization Info

                        myBillConfig.Organization.TaxId = MACSecurity.Security.EncryptAndEncode(txtOrgTaxId.Text, ownerId);
                        myBillConfig.Organization.Street1 = MACSecurity.Security.EncryptAndEncode(txtOrgStreet1.Text, ownerId);
                        myBillConfig.Organization.Street2 = MACSecurity.Security.EncryptAndEncode(txtOrgStreet2.Text, ownerId);
                        myBillConfig.Organization.City = MACSecurity.Security.EncryptAndEncode(txtOrgCity.Text, ownerId);
                        myBillConfig.Organization.State = ObjectId.Parse(dlOrgStates.SelectedValue);
                        myBillConfig.Organization.Zipcode = MACSecurity.Security.EncryptAndEncode(txtOrgZip.Text, ownerId);
                        myBillConfig.Organization.Phone = MACSecurity.Security.EncryptAndEncode(txtOrgPhone.Text, ownerId);

                    #endregion

                    myBillConfig.UpdateConfig(myBillConfig, myBillConfig.OwnerId.ToString());

                    // Compare objects to determine values that were changed
                    //Utils utility = new Utils();
                    //var differences = utility.GetObjectDifferences(myBillConfig, myBillConfig);

                    // Log the changes
                    //var providerEvent = new Event
                    //{
                    //    ownerId = myClient._id,
                    //    UserId = ObjectId.Parse(loggedInAdminId),
                    //    EventTypeDesc = Constants.TokenKeys.ProviderName + updatedProvider.Name
                    //                    + Constants.TokenKeys.ProviderType + "Email"
                    //                    + Constants.TokenKeys.ProviderChangedValues + differences
                    //                    + Constants.TokenKeys.ClientName + myClient.Name
                    //};

                    //providerEvent.Create(Constants.EventLog.Providers.Updated, null);

                    ClientScript.RegisterStartupScript(typeof(Page), "closePage", "<script type='text/JavaScript'>callParentDocumentFunction();</script>");
                }
                else
                {
                    GetStateList();

                    if (!myBillConfig.ItemizedAdvertisingCharges)
                        radioAdSimpleCharge.Checked = true;
                    else
                        radioAdItemizedCharges.Checked = true;

                    foreach (var currentTier in myBillConfig.BillingTiers)
                    {
                        var tierType = currentTier.TierType;
                        var tierValue = MACSecurity.Security.DecodeAndDecrypt(currentTier.TierValues, ownerId);

                        var currentControl = (TextBox)this.Form.FindControl(tierType);

                        if (currentControl != null)
                            currentControl.Text = tierValue;
                    }

                    #region General Settings

                        dlBillingCycle.SelectedValue = myBillConfig.BillingCycle;
                        dlBillingTerms.SelectedValue = myBillConfig.BillingTerms;

                        if (configType == "Client")
                            chkIncludeInGroupBill.Checked = myBillConfig.IncludeInGroupBill;
                        else
                            divIncludeInGroupBill.Visible = false;

                        // Minimum OTP Charge
                        if (myBillConfig.MinimumOtpCharge != "0.00")
                            txtMinimumOtpCharge.Text = MACSecurity.Security.DecodeAndDecrypt(myBillConfig.MinimumOtpCharge, ownerId);
                        else
                            txtMinimumOtpCharge.Text = myBillConfig.MinimumOtpCharge;

                        if (txtMinimumOtpCharge.Text == "")
                            txtMinimumOtpCharge.Text = "0.00";

                        // Minimum Ad Charge
                        if (myBillConfig.MinimumAdCharge != "0.00")
                            txtMinimumAdCharge.Text = MACSecurity.Security.DecodeAndDecrypt(myBillConfig.MinimumAdCharge, ownerId);
                        else
                            txtMinimumAdCharge.Text = myBillConfig.MinimumAdCharge;

                        if (txtMinimumAdCharge.Text == "")
                            txtMinimumAdCharge.Text = "0.00";

                        // Minimum End-User Registration Charge
                        if (myBillConfig.MinimumEndUserRegistrationCharge != "0.00")
                            txtMinimumEndUserRegistrationCharge.Text = MACSecurity.Security.DecodeAndDecrypt(myBillConfig.MinimumEndUserRegistrationCharge, ownerId);
                        else
                            txtMinimumEndUserRegistrationCharge.Text = myBillConfig.MinimumEndUserRegistrationCharge;

                        if (txtMinimumEndUserRegistrationCharge.Text == "")
                            txtMinimumEndUserRegistrationCharge.Text = "0.00";

                        // Monthly Service Charge
                        if (myBillConfig.MonthlyServiceCharge != "0.00")
                            textMonthlyServiceCharge.Text = MACSecurity.Security.DecodeAndDecrypt(myBillConfig.MonthlyServiceCharge, ownerId);
                        else
                            textMonthlyServiceCharge.Text = myBillConfig.MonthlyServiceCharge;

                        if (textMonthlyServiceCharge.Text == "")
                            textMonthlyServiceCharge.Text = "0.00";


                        // Externally Settled By
                        if (myBillConfig.ExternallySettledBy != null)
                        {
                            if (myBillConfig.ExternallySettledBy != "")
                                txtExternallySettledBy.Text = MACSecurity.Security.DecodeAndDecrypt(myBillConfig.ExternallySettledBy, ownerId);
                            else
                                txtExternallySettledBy.Text = myBillConfig.ExternallySettledBy;
                        }
                        else
                            txtExternallySettledBy.Text = "Not specified";

                        // Tax Rate
                        if (myBillConfig.TaxRate != "0.00")
                            txtTaxRate.Text = MACSecurity.Security.DecodeAndDecrypt(myBillConfig.TaxRate, ownerId);
                        else
                            txtTaxRate.Text = myBillConfig.TaxRate;

                        if (txtTaxRate.Text == "")
                            txtTaxRate.Text = "0.00";

                    #endregion

                    #region Organization Info

                        txtOrgName.Text = myBillConfig.OwnerName;

                        txtOrgTaxId.Text = myBillConfig.Organization.TaxId;
                        if (txtOrgTaxId.Text == "")
                            txtOrgTaxId.Text = "Not specified";
                        else
                            txtOrgTaxId.Text = MACSecurity.Security.DecodeAndDecrypt(myBillConfig.Organization.TaxId, ownerId);

                        txtOrgStreet1.Text = myBillConfig.Organization.Street1;
                        if (txtOrgStreet1.Text == "")
                            txtOrgStreet1.Text = "Not specified";
                        else
                            txtOrgStreet1.Text = MACSecurity.Security.DecodeAndDecrypt(myBillConfig.Organization.Street1, ownerId);

                        txtOrgStreet2.Text = myBillConfig.Organization.Street2;
                        if (txtOrgStreet2.Text == "")
                            txtOrgStreet2.Text = "Not specified";
                        else
                            txtOrgStreet2.Text = MACSecurity.Security.DecodeAndDecrypt(myBillConfig.Organization.Street2, ownerId);

                        txtOrgCity.Text = myBillConfig.Organization.City;
                        if (txtOrgCity.Text == "")
                            txtOrgCity.Text = "Not specified";
                        else
                            txtOrgCity.Text = MACSecurity.Security.DecodeAndDecrypt(myBillConfig.Organization.City, ownerId);

                        if (myBillConfig.Organization.State.ToString() != MACServices.Constants.Strings.DefaultEmptyObjectId)
                            dlOrgStates.SelectedValue = myBillConfig.Organization.State.ToString();
                        else
                            dlOrgStates.SelectedIndex = 0;

                        txtOrgZip.Text = myBillConfig.Organization.Zipcode;
                        if (txtOrgZip.Text == "")
                            txtOrgZip.Text = "Not specified";
                        else
                            txtOrgZip.Text = MACSecurity.Security.DecodeAndDecrypt(myBillConfig.Organization.Zipcode, ownerId);

                        txtOrgPhone.Text = myBillConfig.Organization.Phone;
                        if (txtOrgPhone.Text == "")
                            txtOrgPhone.Text = "Not specified";
                        else
                            txtOrgPhone.Text = MACSecurity.Security.DecodeAndDecrypt(myBillConfig.Organization.Phone, ownerId);

                    #endregion

                    #region Payment Gateway

                        chkRequiresSsl.Checked = myBillConfig.PaymentGateway.RequiresSsl;

                        if (myBillConfig.PaymentGateway.GatewayName != "")
                            txtGatewayName.Text = MACSecurity.Security.DecodeAndDecrypt(myBillConfig.PaymentGateway.GatewayName, ownerId);

                        if (myBillConfig.PaymentGateway.ApiVersion != "")
                            txtApiVersion.Text = MACSecurity.Security.DecodeAndDecrypt(myBillConfig.PaymentGateway.ApiVersion, ownerId);

                        if (myBillConfig.PaymentGateway.ApiKey != "")
                            txtApiKey.Text = MACSecurity.Security.DecodeAndDecrypt(myBillConfig.PaymentGateway.ApiKey, ownerId);

                        if (myBillConfig.PaymentGateway.LoginUsername != "")
                            txtLoginUsername.Text = MACSecurity.Security.DecodeAndDecrypt(myBillConfig.PaymentGateway.LoginUsername, ownerId);

                        if (myBillConfig.PaymentGateway.LoginPassword != "")
                            txtLoginPassword.Text = MACSecurity.Security.DecodeAndDecrypt(myBillConfig.PaymentGateway.LoginPassword, ownerId);

                        if (myBillConfig.PaymentGateway.Protocol != "")
                            txtProtocol.Text = MACSecurity.Security.DecodeAndDecrypt(myBillConfig.PaymentGateway.Protocol, ownerId);

                        if (myBillConfig.PaymentGateway.Url != "")
                            txtUrl.Text = MACSecurity.Security.DecodeAndDecrypt(myBillConfig.PaymentGateway.Url, ownerId);

                    #endregion

                    #region Payment Methods

                        switch (myBillConfig.PaymentMethod.Type)
                        {
                            case BillConstants.PaymentMethod.None:
                                dlPaymentMethod.SelectedIndex = 0;
                                break;

                            case BillConstants.PaymentMethod.ACH:
                                dlPaymentMethod.SelectedIndex = 1;

                                if (myBillConfig.PaymentMethod.InstitutionName != "")
                                    txtACHInstitutionName.Text = MACSecurity.Security.DecodeAndDecrypt(myBillConfig.PaymentMethod.InstitutionName, ownerId);

                                if (myBillConfig.PaymentMethod.RoutingNumber != "")
                                    txtACHRoutingNumber.Text = MACSecurity.Security.DecodeAndDecrypt(myBillConfig.PaymentMethod.RoutingNumber, ownerId);

                                if (myBillConfig.PaymentMethod.AccountNumber != "")
                                    txtACHAccountNumber.Text = MACSecurity.Security.DecodeAndDecrypt(myBillConfig.PaymentMethod.AccountNumber, ownerId);
                                break;

                            case BillConstants.PaymentMethod.CreditCard:
                                dlPaymentMethod.SelectedIndex = 2;

                                dlCreditCardType.SelectedValue = myBillConfig.PaymentMethod.Type;

                                if (myBillConfig.PaymentMethod.BillingStreet1 != "")
                                    txtBillingStreet1.Text = MACSecurity.Security.DecodeAndDecrypt(myBillConfig.PaymentMethod.BillingStreet1, ownerId);

                                if (myBillConfig.PaymentMethod.BillingStreet2 != "")
                                    txtBillingStreet2.Text = MACSecurity.Security.DecodeAndDecrypt(myBillConfig.PaymentMethod.BillingStreet2, ownerId);

                                if (myBillConfig.PaymentMethod.BillingCity != "")
                                    txtBillingCity.Text = MACSecurity.Security.DecodeAndDecrypt(myBillConfig.PaymentMethod.BillingCity, ownerId);

                                if (myBillConfig.PaymentMethod.BillingZipCode != "")
                                    txtBillingZipcode.Text = MACSecurity.Security.DecodeAndDecrypt(myBillConfig.PaymentMethod.BillingZipCode, ownerId);

                                dlStates.SelectedValue = myBillConfig.PaymentMethod.BillingState;

                                if (myBillConfig.PaymentMethod.CardholderName != "")
                                    txtCardHolderName.Text = MACSecurity.Security.DecodeAndDecrypt(myBillConfig.PaymentMethod.CardholderName, ownerId);

                                if (myBillConfig.PaymentMethod.CardNumber != "")
                                    txtCardNumber.Text = MACSecurity.Security.DecodeAndDecrypt(myBillConfig.PaymentMethod.CardNumber, ownerId);

                                if (myBillConfig.PaymentMethod.CCVNumber != "")
                                    txtCCVNumber.Text = MACSecurity.Security.DecodeAndDecrypt(myBillConfig.PaymentMethod.CCVNumber, ownerId);

                                if (myBillConfig.PaymentMethod.Expires != "")
                                    txtCardExpires.Text = MACSecurity.Security.DecodeAndDecrypt(myBillConfig.PaymentMethod.Expires, ownerId);
                                break;

                            case BillConstants.PaymentMethod.ManualCheck:
                                dlPaymentMethod.SelectedIndex = 3;
                                break;

                            case BillConstants.PaymentMethod.WireTransfer:
                                dlPaymentMethod.SelectedIndex = 4;

                                if (myBillConfig.PaymentMethod.InstitutionName != "")
                                    txtWireInstitutionName.Text = MACSecurity.Security.DecodeAndDecrypt(myBillConfig.PaymentMethod.InstitutionName, ownerId);

                                if (myBillConfig.PaymentMethod.RoutingNumber != "")
                                    txtWireRoutingNumber.Text = MACSecurity.Security.DecodeAndDecrypt(myBillConfig.PaymentMethod.RoutingNumber, ownerId);

                                if (myBillConfig.PaymentMethod.AccountNumber != "")
                                    txtAccountNumber.Text = MACSecurity.Security.DecodeAndDecrypt(myBillConfig.PaymentMethod.AccountNumber, ownerId);
                                break;

                            default:
                                dlPaymentMethod.SelectedIndex = 1;

                                if (myBillConfig.PaymentMethod.InstitutionName != "")
                                    txtACHInstitutionName.Text = MACSecurity.Security.DecodeAndDecrypt(myBillConfig.PaymentMethod.InstitutionName, ownerId);

                                if (myBillConfig.PaymentMethod.RoutingNumber != "")
                                    txtACHRoutingNumber.Text = MACSecurity.Security.DecodeAndDecrypt(myBillConfig.PaymentMethod.RoutingNumber, ownerId);

                                if (myBillConfig.PaymentMethod.AccountNumber != "")
                                    txtACHAccountNumber.Text = MACSecurity.Security.DecodeAndDecrypt(myBillConfig.PaymentMethod.AccountNumber, ownerId);
                                break;
                        }

                    #endregion

                    divGroupSelection.Visible = false;

                    if (GroupMembershipCount > 0)
                    {
                        divGroupSelection.Visible = true;
                        divIncludeInGroupBill.Visible = true;
                        spanIncludeInBillGroup.InnerHtml = "Include in Group billing?";
                        chkIncludeInGroupBill.Enabled = true;

                        // Attach client-side click event handler to checkbox. Display assigned groups if checked
                        chkIncludeInGroupBill.Attributes.Add("onclick", "javascript: GetGroupMembership(this.checked);");
                    }
                    else
                    {
                        if (configType == "Client")
                        {
                            chkIncludeInGroupBill.Checked = myBillConfig.IncludeInGroupBill;
                            spanIncludeInBillGroup.InnerHtml = "Cannot include in Group billing. Not a member of a Group.";
                            chkIncludeInGroupBill.Enabled = false;
                        }
                        else
                        {
                            divIncludeInGroupBill.Visible = false;
                        }
                    }

                    CheckEmptyFields();

                    divConfigWarning_1.Visible = false;
                    divConfigWarning_2.Visible = false;

                    // Warn the admin that a default config is being used
                    if (myBillConfig.UpdatedById.ToString() == Constants.Strings.DefaultEmptyObjectId)
                    {
                        divConfigWarning_1.Visible = true;
                        divConfigWarning_2.Visible = true;

                        divConfigWarningText_1.InnerHtml = "!!! Default Billing Configuration !!!";
                        divConfigWarningText_2.InnerHtml = "!!! Default Billing Configuration !!!";
                    }

                    // Disable the controls because they don't apply
                    if (myClient.AdEnabled)
                    {
                        txtMinimumAdCharge.Enabled = true;
                        spanMinimumAdCharge.InnerHtml = "Minimum Ad Charge";
                    }
                    else
                    {
                        txtMinimumAdCharge.Text = "0.00";
                        txtMinimumAdCharge.Enabled = false;
                        spanMinimumAdCharge.InnerHtml = "Minimum Ad Charge Disabled - Client not Ad enabled";
                    }

                    if (myBillConfig.IncludeInGroupBill)
                    {
                        // Disable to eliminate confusion. Controls are set in the assigned group's config
                        myGroup = new Group(myBillConfig.BillToGroupId.ToString());

                        HideConfigControls(myGroup.Name);
                    }
                    //else
                    //    divGeneralSettingsEnabledMessage.Visible = false;
                }
            }
            // ReSharper disable once UnusedVariable
            catch(Exception ex)
            {
                var errMsg = ex.ToString();
            }

            if(Convert.ToBoolean(userIsReadOnly))
            {
                btnSave.Visible = false;
            }
        }

        private void HideConfigControls(string ownerName)
        {
            //panelGeneralSettings.Visible = false;
            //panelPaymentGateway.Visible = false;
            //panelPaymentMethod.Visible = false;
            //panelPricingTiers.Visible = false;
        }

        private void CheckEmptyFields()
        {
            // Payment Gateway
            if (txtGatewayName.Text == "")
                txtGatewayName.Text = "payments.bofa.com";

            if(txtApiVersion.Text == "")
                txtApiVersion.Text = "1.0.0";

            if(txtApiKey.Text == "")
                txtApiKey.Text = "5E1/1BD3A4805cE3c223e6AB3Ca7414+02089F86";

            if(txtLoginUsername.Text == "")
                txtLoginUsername.Text = "macservices";

            if(txtLoginPassword.Text == "")
                txtLoginPassword.Text = "!macotp#";

            if(txtProtocol.Text == "")
                txtProtocol.Text = "http";

            if(txtUrl.Text == "")
                txtUrl.Text = "http://payments.bofa.com";

            // ACH
            if(txtACHInstitutionName.Text == "")
                txtACHInstitutionName.Text = "Bank of America";

            if(txtACHRoutingNumber.Text == "")
                txtACHRoutingNumber.Text = "12340987456";

            if(txtACHAccountNumber.Text == "")
                txtACHAccountNumber.Text = "1234567890";

            // Credit Card
            if(txtCardHolderName.Text == "")
                txtCardHolderName.Text = "MAC User";

            if(txtCardNumber.Text == "")
                txtCardNumber.Text = "1234 1234 1234 1234";

            if(txtCCVNumber.Text == "")
                txtCCVNumber.Text = "012";

            if(txtCardExpires.Text == "")
                txtCardExpires.Text = "12/2016";

            if(txtBillingStreet1.Text == "")
                txtBillingStreet1.Text = "123 My Street";

            if(txtBillingStreet2.Text == "")
                txtBillingStreet2.Text = "#218";

            if(txtBillingCity.Text == "")
                txtBillingCity.Text = "Scottsdale";

            if(txtBillingZipcode.Text == "")
                txtBillingZipcode.Text = "Az";

            // Bank Wire
            if(txtWireInstitutionName.Text == "")
                txtWireInstitutionName.Text = "Bank of America";

            if(txtWireRoutingNumber.Text == "")
                txtWireRoutingNumber.Text = "12340987456";

            if(txtAccountNumber.Text == "")
                txtAccountNumber.Text = "1234567890";
        }

        Predicate<Relationship> FindRelationshipByMemberType(string memberType)
        {
            return relationship => relationship.MemberType == memberType;
        }

        public void GetStateList()
        {
            var stateList = new MacList("", "TypeDefinitions", "State", "_id,Name");
            foreach (var li in stateList.ListItems.Select(item => new ListItem { Text = item.Attributes["Name"], Value = item.Attributes["_id"] }))
            {
                dlOrgStates.Items.Add(li);
                dlStates.Items.Add(li);
            }
        }
    }
}