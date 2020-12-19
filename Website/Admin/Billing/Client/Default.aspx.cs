using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

using MACBilling;
using MACServices;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

using Newtonsoft.Json;

using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;

using cnt = MACBilling.BillConstants;
using cs = MACServices.Constants.Strings;

public partial class Default : System.Web.UI.Page
{
    public HtmlForm _myForm { get; set; }

    HiddenField _hiddenF;
    HiddenField _hiddenE;
    HiddenField _hiddenH;
    HiddenField _hiddenI;
    HiddenField _hiddenD;
    HiddenField _hiddenT;
    HiddenField _userIpAddress;
    HiddenField _hiddenUserRole;

    HiddenField _hiddenV;

    HiddenField _hiddenW;

    Utils mUtils = new Utils();
    BillUtils myBillUtils = new BillUtils();

    string userIsReadOnly = "";
    string userId = "";
    string userFirstName = "";
    string userLastName = "";
    //string clientId = "";
    //string action = "";
    string userRole = "";
    //string userIpAddress = "";

    Client myClient;

    MongoDatabase mongoDBConnectionPool = (MongoDatabase)HttpContext.Current.Application[cs.MongoDB];
    //MongoCollection mongoCollection;

    public string parentContainerId = "";
    public Decimal ClientRate = 0.00M;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Page.Master != null)
        {
            if (Master != null)
            {
                _myForm = (HtmlForm)Page.Master.FindControl("formMain");

                _hiddenF = (HiddenField)Page.Master.FindControl("hiddenF");
                _hiddenE = (HiddenField)Page.Master.FindControl("hiddenE");
                _hiddenH = (HiddenField)Page.Master.FindControl("hiddenH");
                _hiddenI = (HiddenField)Page.Master.FindControl("hiddenI");
                _hiddenD = (HiddenField)Page.Master.FindControl("hiddenD");
                _hiddenT = (HiddenField)Page.Master.FindControl("hiddenT");
                _hiddenUserRole = (HiddenField)Page.Master.FindControl("hiddenL");
                _userIpAddress = (HiddenField)Master.FindControl("hiddenM");

                _hiddenV = (HiddenField)Master.FindControl("hiddenV");

                _hiddenW = (HiddenField)Master.FindControl("hiddenW");
                _hiddenW.Value = "54e77340b5655a0a4c4e89a5";
            }
        }

        userId = MACSecurity.Security.DecodeAndDecrypt(_hiddenE.Value, Constants.Strings.DefaultClientId);
        userIsReadOnly = MACSecurity.Security.DecodeAndDecrypt(_hiddenF.Value, userId);
        userFirstName = MACSecurity.Security.DecodeAndDecrypt(_hiddenH.Value, userId);
        userLastName = MACSecurity.Security.DecodeAndDecrypt(_hiddenI.Value, userId);
        userRole = MACSecurity.Security.DecodeAndDecrypt(_hiddenUserRole.Value, userId);

        if (Request["pa"] != null)
            hiddenAA.Value = Request["pa"];

        if (Request["bid"] != null)
        {
            _hiddenV.Value = Request["bid"];
            hiddenAA.Value = "ViewBillDetails";
        }

        if (Request["billId"] != null)
        {
            _hiddenV.Value = Request["billId"];
        }

        divBillDocumentContainer.Visible = false;
        btnBillConfig.Enabled = false;

        btnBillSave.Enabled = true;
        btnBillHistory.Enabled = false;
        btnBillSend.Enabled = false;
        //btnBillPrint.Enabled = false;

        var selectedOwnerId = dlClients.SelectedValue;// hiddenselectedOwnerId.Value;
        if (selectedOwnerId == cs.DefaultEmptyObjectId)
        {
            return;
        }
        else if (selectedOwnerId != "-1")
        {
            myClient = new Client(selectedOwnerId);

            divBillDocumentContainer.Visible = true;
            btnBillConfig.Enabled = true;

            GetBillDetails(selectedOwnerId);
        }

        divUpdateMsg.Visible = false;
        updateMessage.InnerHtml = "";

        if (IsPostBack)
        {
            // Save the bill info
            switch (hiddenAA.Value)
            {
                case "SaveBill":
                    var myBill = (new JavaScriptSerializer()).Deserialize<BillClient>(hiddenSerializedBill.Value);

                    myBill._id = ObjectId.Parse(_hiddenV.Value);
                    myBill.ClientId = ObjectId.Parse(selectedOwnerId);

                    foreach (var currAddendum in myBill.Addendums)
                    {
                        if (currAddendum.IsMinimumPriceAdjustment)
                        {
                            // Do nothing. This is a non-persisted minimum price adjustment addendum
                        }
                        else
                        {
                            // Update status of addendum
                            currAddendum._id = ObjectId.Parse(currAddendum.AddendumId);
                            currAddendum.AttachedToBillId = myBill._id;
                            currAddendum.DateAttached = DateTime.UtcNow;
                            currAddendum.HasBeenAttached = true;
                            currAddendum.CreatedById = ObjectId.Parse(_hiddenE.Value);

                            // Update the addendum in the db
                            currAddendum.UpdateAddendum(currAddendum, currAddendum._id.ToString());
                        }
                    }

                    var myArchive = new BillArchive();

                    myArchive.ForMonthYear = myBill.DateStart.ToString("MM/yyyy");

                    myArchive.OwnerId = myBill.ClientId.ToString();

                    var myJsonBill = (new JavaScriptSerializer()).Serialize(myBill);

                    myArchive._id = myBill._id;

                    //myArchive.BillDetails = MACSecurity.Security.EncryptAndEncode(myJsonBill, myBill.ClientId.ToString());
                    myArchive.BillDetails = myJsonBill;
                    //myArchive.BillDetails = myJsonBill;

                    myArchive.Amount = myBill.Total;

                    myArchive.CreatedById = ObjectId.Parse(_hiddenE.Value);

                    myArchive.DateCreated = DateTime.UtcNow;
                    myArchive.DateDue = myBill.DateDue;

                    //myArchive.OwnerId = ObjectId.Parse(selectedOwnerId);
                    myArchive.OwnerId = selectedOwnerId;
                    myArchive.OwnerName = myClient.Name;
                    myArchive.OwnerType = "Client";

                    myArchive.Archive(myArchive);

                    //var adminProfile = new UserProfile(_hiddenE.Value);
                    //var adminFirstName = MACSecurity.Security.DecodeAndDecrypt(adminProfile.FirstName, _hiddenE.Value);
                    //var adminLastName = MACSecurity.Security.DecodeAndDecrypt(adminProfile.LastName, _hiddenE.Value);

                    // Log the billing event
                    var myBillEvent = new Event();
                    myBillEvent.EventTypeDesc = Constants.TokenKeys.BillAmount + "<a href='/Admin/Billing/Client/Default.aspx?bid=" + myBill._id + "&pa=ViewBillDetails' id='link_viewBillDetails'>" + myBillUtils.FormatMoney(Convert.ToDecimal(myBill.Total.ToString())) + "</a>";
                    myBillEvent.EventTypeDesc += Constants.TokenKeys.ClientName + myArchive.OwnerName;
                    myBillEvent.EventTypeDesc += Constants.TokenKeys.EventGeneratedByName + (userFirstName + " " + userLastName);

                    myBillEvent.EventTypeName = Constants.EventLog.Billing.Created.Item2;
                    myBillEvent.EventTypeId = Constants.EventLog.Billing.Created.Item1;
                    myBillEvent.ClientId = ObjectId.Parse(selectedOwnerId);

                    myBillEvent.Create(Constants.EventLog.Billing.Created, null);

                    divUpdateMsg.Visible = true;
                    updateMessage.InnerHtml = "Successfully created and archived bill";

                    btnBillSave.Enabled = false;
                    btnBillHistory.Enabled = true;
                    btnBillSend.Enabled = true;
                    //btnBillPrint.Enabled = true;

                    hiddenAA.Value = "";
                    _hiddenV.Value = "";
                    break;

                case "ViewBillDetails":
                    // Decrypt and rehydrate bill for ui
                    //var myBillArchive = new BillArchive(_hiddenV.Value);

                    //var myClient = new Client(myBillArchive.OwnerId.ToString());

                    //var decryptBillDetails = MACSecurity.Security.DecodeAndDecrypt(myBillArchive.BillDetails, myClient._id.ToString());

                    //var myOldBill = (new JavaScriptSerializer()).Deserialize<BillClient>(decryptBillDetails);

                    //myOldBill._id = ObjectId.Parse(_hiddenV.Value);
                    //myOldBill.ClientId = ObjectId.Parse(selectedOwnerId);

                    //spanOrganization.InnerHtml = "Bill Details";

                    //renderBillCurrent(myClient, myOldBill, false);

                    //hiddenAA.Value = "";
                    //_hiddenV.Value = "";

                    //btnBillSave.Enabled = false;
                    break;
            }
        }
        else
        {
            GetClientList();
        }

        if (Convert.ToBoolean(userIsReadOnly))
        {
            btnBillSave.Visible = false;
            btnBillPay.Visible = false;
            btnBillVoid.Visible = false;
            btnBillBack.Visible = false;
        }
    }

    public void GetBillDetails(string selectedOwnerId)
    {
        // Check to see if there is any billing history
        btnBillHistory.Enabled = false;

        var archiveQuery = Query.And(Query.EQ("_t", "BillArchive"), Query.EQ("OwnerId", selectedOwnerId));
        var archiveSortBy = SortBy.Descending("DateCreated");

        var billHistory = mongoDBConnectionPool.GetCollection("Archive").FindAs<BillArchive>(archiveQuery).SetSortOrder(archiveSortBy);
        foreach (BillArchive billArchive in billHistory)
        {
            btnBillHistory.Enabled = true;
            btnBillSend.Enabled = true;
            //btnBillPrint.Enabled = true;
        }

        UserControls_BillingRender myBillingRender;

        BillUtils myBillUtils = new BillUtils();
        Utils myUtils = new Utils();

        Client myClient = new Client(selectedOwnerId);

        BillClient myClientBill = new BillClient(dlDateRange.SelectedItem.Text, selectedOwnerId, _hiddenE.Value);

        BillConfig myClientBillConfig = new BillConfig(selectedOwnerId, "Client", "");

        var miscTotal = 0M;

        var iCount = 1;
        var groupAssignmentCount = 0;

        #region Client Bill

            myBillingRender = (UserControls_BillingRender)Page.LoadControl("~/UserControls/BillingRender.ascx");

            myBillingRender.ClientLogo = myClient.OwnerLogoUrl;
            myBillingRender.ClientID = myClient._id.ToString();

            myBillingRender.BillNumber = myClientBill._id.ToString();

            #region Set Labels

                //myBillingRender.ClientIdLabel = "ClientId";
                myBillingRender.ClientNameLabel = "Client";

                // Set form labels
                myBillingRender.LegendAdsCost = "Price";
                myBillingRender.LegendEndUserCost = "Price";
                myBillingRender.LegendMessagingCost = "Price";
                myBillingRender.LegendTaxRate = "<em>Tax Rate</em>";

            #endregion

            #region General Bill Info

                myBillingRender.ID = ObjectId.GenerateNewId().ToString();

                //myBillingRender.BillNumber = myClientBill._id.ToString();
                _hiddenV.Value = myClientBill._id.ToString();  //myBillingRender.BillNumber;

                // Set each bill's container ID to hyperlink the Client Members legend
                myBillingRender.BillContainerId = "divBillDocumentContainer_" + myClientBill._id.ToString();  //myBillingRender.BillNumber;

                parentContainerId = myBillingRender.BillContainerId;

                myBillingRender.ClientMemberNumber = "";

                myBillingRender.DateCreated = myClientBill.DateCreated.ToString();
                myBillingRender.DateDue = myClientBill.DateDue.ToString();
                myBillingRender.DateSent = "Not Sent";

                if (myClientBill.DatePaid.ToString() == "1/1/0001 12:00:00 AM")
                    myBillingRender.DatePaid = "Not Paid";
                else
                    myBillingRender.DatePaid = myClientBill.DatePaid.ToString();

            #endregion

            #region Organization Info

                //myBillingRender.ClientID = selectedOwnerId;

                myBillingRender.ClientName = myClient.Name;

                if (myClientBillConfig.Organization.TaxId != "")
                    myBillingRender.TaxId = MACSecurity.Security.DecodeAndDecrypt(myClientBillConfig.Organization.TaxId, selectedOwnerId);
                else
                    myBillingRender.TaxId = "Not specified";

                if (myClientBillConfig.Organization.Phone != "")
                    myBillingRender.Phone = MACSecurity.Security.DecodeAndDecrypt(myClientBillConfig.Organization.Phone, selectedOwnerId);
                else
                    myBillingRender.Phone = "Not specified";

                if (myClientBillConfig.Organization.Street1 != "")
                    myBillingRender.Street1 = MACSecurity.Security.DecodeAndDecrypt(myClientBillConfig.Organization.Street1, selectedOwnerId);
                else
                    myBillingRender.Street1 = "Not specified";

                if (myClientBillConfig.Organization.Street2 != "")
                    myBillingRender.Street2 = MACSecurity.Security.DecodeAndDecrypt(myClientBillConfig.Organization.Street2, selectedOwnerId);
                else
                    myBillingRender.Street2 = "Not specified";

                if (myClientBillConfig.Organization.City != "")
                    myBillingRender.City = MACSecurity.Security.DecodeAndDecrypt(myClientBillConfig.Organization.City, selectedOwnerId);
                else
                    myBillingRender.City = "Not specified";

                // Need to lookup state by id
                if (myClientBillConfig.Organization.State.ToString() != MACServices.Constants.Strings.DefaultEmptyObjectId)
                    myBillingRender.State = myUtils.GetStateById(myClientBillConfig.Organization.State.ToString());
                else
                    myBillingRender.State = "Not specified";

                if (myClientBillConfig.Organization.Zipcode != "")
                    myBillingRender.ZipCode = MACSecurity.Security.DecodeAndDecrypt(myClientBillConfig.Organization.Zipcode, selectedOwnerId);
                else
                    myBillingRender.ZipCode = "Not specified";


            #endregion

            #region Otp Charges

                // Email
                // Reset ClientRate
                ClientRate = 0.00M;
                myBillingRender.OtpEmailCount = myBillUtils.FormatNumber(myClientBill.OtpSentEmailCount);
                var otpRateSentEmail = FindTierByName(myClientBillConfig, BillConstants.Tiers.OtpSentEmail).Split(char.Parse(cnt.Common.ItemSep));
                foreach (var currentCost in otpRateSentEmail)
                {
                    var currentRateElement = currentCost.Split(char.Parse(cnt.Common.KVSep));
                    var rateCount = Convert.ToInt32(currentRateElement[0]);
                    Decimal rateValue = Convert.ToDecimal(currentRateElement[1]);

                    if (ClientRate == 0.00M)
                        ClientRate = rateValue;

                    // If the actual event count is greater than the current rate count, set the price according to rate
                    if (Convert.ToInt32(myBillingRender.OtpEmailCount.Replace(",", "")) > rateCount)
                        ClientRate = rateValue;
                }
                myBillingRender.OtpEmailCost = myBillUtils.FormatCost(ClientRate);
                myBillingRender.OtpEmailCharge = myBillUtils.FormatMoney(myClientBill.OtpSentEmailAmount);

                // SMS
                // Reset ClientRate
                ClientRate = 0.00M;
                myBillingRender.OtpSmsCount = myBillUtils.FormatNumber(myClientBill.OtpSentSmsCount);
                var otpRateSentSms = FindTierByName(myClientBillConfig, BillConstants.Tiers.OtpSentSms).Split(char.Parse(cnt.Common.ItemSep));
                foreach (var currentCost in otpRateSentSms)
                {
                    var currentRateElement = currentCost.Split(char.Parse(cnt.Common.KVSep));
                    var rateCount = Convert.ToInt32(currentRateElement[0]);
                    Decimal rateValue = Convert.ToDecimal(currentRateElement[1]);

                    if (ClientRate == 0.00M)
                        ClientRate = rateValue;

                    // If the actual event count is greater than the current rate count, set the price according to rate
                    if (Convert.ToInt32(myBillingRender.OtpSmsCount.Replace(",","")) > rateCount)
                        ClientRate = rateValue;
                }
                myBillingRender.OtpSmsCost = myBillUtils.FormatCost(ClientRate);
                myBillingRender.OtpSmsCharge = myBillUtils.FormatMoney(myClientBill.OtpSentSmsAmount);
                

                // Voice
                // Reset ClientRate
                ClientRate = 0.00M;
                myBillingRender.OtpVoiceCount = myBillUtils.FormatNumber(myClientBill.OtpSentVoiceCount);
                var otpRateSentVoice = FindTierByName(myClientBillConfig, BillConstants.Tiers.OtpSentVoice).Split(char.Parse(cnt.Common.ItemSep));
                foreach (var currentCost in otpRateSentVoice)
                {
                    var currentRateElement = currentCost.Split(char.Parse(cnt.Common.KVSep));
                    var rateCount = Convert.ToInt32(currentRateElement[0]);
                    Decimal rateValue = Convert.ToDecimal(currentRateElement[1]);

                    if (ClientRate == 0.00M)
                        ClientRate = rateValue;

                    // If the actual event count is greater than the current rate count, set the price according to rate
                    if (Convert.ToInt32(myBillingRender.OtpVoiceCount.Replace(",", "")) > rateCount)
                        ClientRate = rateValue;
                }
                myBillingRender.OtpVoiceCost = myBillUtils.FormatCost(ClientRate);
                myBillingRender.OtpVoiceCharge = myBillUtils.FormatMoney(myClientBill.OtpSentVoiceAmount);
                

                myBillingRender.OtpTotal = myBillUtils.FormatMoney((myClientBill.OtpSentEmailAmount + myClientBill.OtpSentSmsAmount + myClientBill.OtpSentVoiceAmount));

            #endregion

            #region Advertising Charges

                // OTP screen
                // Reset ClientRate
                ClientRate = 0.00M;
                myBillingRender.AdEnterOtpScreenSentCount = myBillUtils.FormatNumber(myClientBill.AdEnterOtpScreenSentCount);
                var adOtpScreen = FindTierByName(myClientBillConfig, BillConstants.Tiers.OtpSentVoice).Split(char.Parse(cnt.Common.ItemSep));
                foreach (var currentCost in adOtpScreen)
                {
                    var currentRateElement = currentCost.Split(char.Parse(cnt.Common.KVSep));
                    var rateCount = Convert.ToInt32(currentRateElement[0]);
                    Decimal rateValue = Convert.ToDecimal(currentRateElement[1]);

                    if (ClientRate == 0.00M)
                        ClientRate = rateValue;

                    // If the actual event count is greater than the current rate count, set the price according to rate
                    if (Convert.ToInt32(myBillingRender.AdEnterOtpScreenSentCount.Replace(",", "")) > rateCount)
                        ClientRate = rateValue;
                }
                myBillingRender.AdEnterOtpScreenSentCost = myBillUtils.FormatCost(ClientRate);
                myBillingRender.AdEnterOtpScreenSentAmount = myBillUtils.FormatMoney(myClientBill.AdEnterOtpScreenSentAmount);

                // Ad message
                // Reset ClientRate
                ClientRate = 0.00M;
                myBillingRender.AdMessageSentCount = myBillUtils.FormatNumber(myClientBill.AdMessageSentCount);
                var adMessage = FindTierByName(myClientBillConfig, BillConstants.Tiers.OtpSentVoice).Split(char.Parse(cnt.Common.ItemSep));
                foreach (var currentCost in adMessage)
                {
                    var currentRateElement = currentCost.Split(char.Parse(cnt.Common.KVSep));
                    var rateCount = Convert.ToInt32(currentRateElement[0]);
                    Decimal rateValue = Convert.ToDecimal(currentRateElement[1]);

                    if (ClientRate == 0.00M)
                        ClientRate = rateValue;

                    // If the actual event count is greater than the current rate count, set the price according to rate
                    if (Convert.ToInt32(myBillingRender.AdMessageSentCount.Replace(",", "")) > rateCount)
                        ClientRate = rateValue;
                }
                myBillingRender.AdMessageSentCost = myBillUtils.FormatCost(ClientRate);
                myBillingRender.AdMessageSentAmount = myBillUtils.FormatMoney(myClientBill.AdMessageSentAmount);

                // Verification screen
                // Reset ClientRate
                ClientRate = 0.00M;
                myBillingRender.AdVerificationScreenSentCount = myBillUtils.FormatNumber(myClientBill.AdVerificationScreenSentCount);
                var adVerification = FindTierByName(myClientBillConfig, BillConstants.Tiers.OtpSentVoice).Split(char.Parse(cnt.Common.ItemSep));
                foreach (var currentCost in adVerification)
                {
                    var currentRateElement = currentCost.Split(char.Parse(cnt.Common.KVSep));
                    var rateCount = Convert.ToInt32(currentRateElement[0]);
                    Decimal rateValue = Convert.ToDecimal(currentRateElement[1]);

                    if (ClientRate == 0.00M)
                        ClientRate = rateValue;

                    // If the actual event count is greater than the current rate count, set the price according to rate
                    if (Convert.ToInt32(myBillingRender.AdVerificationScreenSentCount.Replace(",", "")) > rateCount)
                        ClientRate = rateValue;
                }
                myBillingRender.AdVerificationScreenSentCost = myBillUtils.FormatCost(ClientRate);
                myBillingRender.AdVerificationScreenSentAmount = myBillUtils.FormatMoney(myClientBill.AdVerificationScreenSentAmount);

                myBillingRender.AdPassTotal = myBillUtils.FormatMoney((myClientBill.AdEnterOtpScreenSentAmount + myClientBill.AdMessageSentAmount + myClientBill.AdVerificationScreenSentAmount));

            #endregion

            #region End User Charges

                // Registration
                // Reset ClientRate
                ClientRate = 0.00M;
                myBillingRender.EndUserRegistrationCount = myBillUtils.FormatNumber(myClientBill.EndUserRegistrationCount);
                var endUserRegistration = FindTierByName(myClientBillConfig, BillConstants.Tiers.OtpSentVoice).Split(char.Parse(cnt.Common.ItemSep));
                foreach (var currentCost in endUserRegistration)
                {
                    var currentRateElement = currentCost.Split(char.Parse(cnt.Common.KVSep));
                    var rateCount = Convert.ToInt32(currentRateElement[0]);
                    Decimal rateValue = Convert.ToDecimal(currentRateElement[1]);

                    if (ClientRate == 0.00M)
                        ClientRate = rateValue;

                    // If the actual event count is greater than the current rate count, set the price according to rate
                    if (Convert.ToInt32(myBillingRender.EndUserRegistrationCount.Replace(",", "")) > rateCount)
                        ClientRate = rateValue;
                }
                myBillingRender.EndUserRegistrationCost = myBillUtils.FormatCost(ClientRate);
                myBillingRender.EndUserRegistrationAmount = myBillUtils.FormatMoney(myClientBill.EndUserRegistrationAmount);

                // Verification
                // Reset ClientRate
                ClientRate = 0.00M;
                myBillingRender.EndUserVerificationCount = myBillUtils.FormatNumber(myClientBill.EndUserVerificationCount);
                var endUserVerification = FindTierByName(myClientBillConfig, BillConstants.Tiers.OtpSentVoice).Split(char.Parse(cnt.Common.ItemSep));
                foreach (var currentCost in endUserVerification)
                {
                    var currentRateElement = currentCost.Split(char.Parse(cnt.Common.KVSep));
                    var rateCount = Convert.ToInt32(currentRateElement[0]);
                    Decimal rateValue = Convert.ToDecimal(currentRateElement[1]);

                    if (ClientRate == 0.00M)
                        ClientRate = rateValue;

                    // If the actual event count is greater than the current rate count, set the price according to rate
                    if (Convert.ToInt32(myBillingRender.EndUserVerificationCount.Replace(",", "")) > rateCount)
                        ClientRate = rateValue;
                }
                myBillingRender.EndUserVerificationCost = myBillUtils.FormatCost(ClientRate);
                myBillingRender.EndUserVerificationAmount = myBillUtils.FormatMoney(myClientBill.EndUserVerificationAmount);

                myBillingRender.EndUserTotal = myBillUtils.FormatMoney((myClientBill.EndUserRegistrationAmount + myClientBill.EndUserVerificationAmount));

            #endregion

            #region Miscellaneous Charges

                myBillingRender.ShowMiscCharges = false;

                miscTotal = 0M;

                iCount = 1;

                if (myClientBill.Addendums.Count > 0)
                {
                    StringBuilder sbAddendums = new StringBuilder();

                    myBillingRender.ShowMiscCharges = true;

                    sbAddendums.Append("<table style='border: solid 0px #0000ff;'>");

                    foreach (BillAddendum myAddendum in myClientBill.Addendums)
                    {
                        //var tmpAddAmount = MACSecurity.Security.DecodeAndDecrypt(myAddendum.Amount, myAddendum.OwnerId.ToString()).Replace("$", "");
                        var tmpAddAmount = myAddendum.Amount;

                        miscTotal += Convert.ToDecimal(tmpAddAmount);

                       // var tmpAddNotes = MACSecurity.Security.DecodeAndDecrypt(myAddendum.Notes, myAddendum.OwnerId.ToString());
                        var tmpAddNotes = myAddendum.Notes;

                        sbAddendums.Append("<tr style='border: solid 0px #0000ff;'>");

                        sbAddendums.Append("    <td class='tdMisc50' style='width: 50%;'>");
                        sbAddendums.Append("        <div>");
                        sbAddendums.Append("            <span style='color: #808080;'>" + iCount + ".</span> ");
                        sbAddendums.Append("            <span style='font-weight: normal;'>" + myAddendum.OwnerType + " - " + myAddendum.OwnerName + "</span>");
                        sbAddendums.Append("        </div>");
                        sbAddendums.Append("        <span runat='server' class='billingData small-only-text-center' style='position: relative; left: 16px;'>" + tmpAddNotes + "</span>");
                        sbAddendums.Append("    </td>");

                        if (tmpAddAmount.Contains("-"))
                        {
                            sbAddendums.Append("    <td class='tdMisc25'><span runat='server' class='billingData small-only-text-center'>-" + myBillUtils.FormatMoney(Convert.ToDecimal(tmpAddAmount.Replace("-", ""))) + "</span></td>");
                            sbAddendums.Append("    <td class='tdMisc25'><span runat='server' class='billingData small-only-text-center' style='color: #808080;'>-</span></td>");
                        }
                        else
                        {
                            sbAddendums.Append("    <td class='tdMisc25'><span runat='server' class='billingData small-only-text-center' style='color: #808080;'>-</span></td>");
                            sbAddendums.Append("    <td class='tdMisc25'><span runat='server' class='billingData small-only-text-center'>" + myBillUtils.FormatMoney(Convert.ToDecimal(tmpAddAmount)) + "</span></td>");
                        }

                        sbAddendums.Append("</tr>");

                        iCount++;
                    }

                    sbAddendums.Append("</table>");

                    myBillingRender.MiscTotal = myBillUtils.FormatMoney(miscTotal);

                    myBillingRender.AddendumsContainer = sbAddendums.ToString();
                }

            #endregion

            #region SubTotal, Tax Rate, Taxes and Total

                myBillingRender.Subtotal = myBillUtils.FormatMoney(myClientBill.SubTotal);

                myBillingRender.TaxRate = myClientBill.TaxRate.ToString();
                myBillingRender.SalesTax = myBillUtils.FormatMoney(myClientBill.SalesTax);

                myBillingRender.Total = myBillUtils.FormatMoney(myClientBill.Total);

                if (myClientBill.Total > 0.00M)
                    btnBillSave.Enabled = true;

            #endregion

            #region Footnotes

                // This is only applicable to group billing
                //StringBuilder sbFootnotes = new StringBuilder();
                //sbFootnotes.Append("<div id='divClientSummaryContainer' style='border-bottom: solid 0px #c0c0c0; position: relative; top: 25px; padding-bottom: 10px; margin-bottom: 25px; font-weight: bold;'>" + myClientBill.ClientBills.Count.ToString() + " Client Clients Summary</div>");

                //var clientCount = 1;

                // Need to parse this from the Client properties.

                //foreach (BillClient currentClientBill in myClientBill.ClientBills)
                //{
                //    Client myClient = new Client(currentClientBill.ClientId.ToString());

                //    sbFootnotes.Append("<div style='border-bottom: solid 0px #c0c0c0; font-weight: normal; margin-bottom: 10px;'>");

                //    sbFootnotes.Append("<div style='float: left; position: relative; left: 0px; border: solid 0px #ff0000; width: 45px; text-align: left; margin-right: 3px;'>pg " + (clientCount+1) + "</div>");

                //    sbFootnotes.Append("<a href='#divBillDocumentContainer_" + currentClientBill._id + "'>");
                //    sbFootnotes.Append("<div style='float: left;'>");
                //    sbFootnotes.Append(myClient.Name);
                //    sbFootnotes.Append("</div>");
                //    sbFootnotes.Append("</a>");

                //    myClientBillConfig = new BillConfig(currentClientBill.ClientId.ToString(), "Client", "");

                //    Client myAssignedClient;

                //    if (myClientBillConfig.BillToClientId.ToString() == selectedOwnerId)
                //    {
                //        myAssignedClient = new Client(myClientBillConfig.BillToClientId.ToString());

                //        sbFootnotes.Append("&nbsp;&nbsp;-&nbsp;&nbsp;<span style='color: #ff0000;'>Included (" + myBillUtils.FormatMoney(currentClientBill.Total) + ") in this Client bill.</span>");
                //    }
                //    else if (myClientBillConfig.IncludeInClientBill)
                //    {
                //        myAssignedClient = new Client(myClientBillConfig.BillToClientId.ToString());

                //        sbFootnotes.Append("&nbsp;&nbsp;-&nbsp;&nbsp;<span style=''>Charged (" + myBillUtils.FormatMoney(currentClientBill.Total) + ") to the (<span style='font-weight: bold;'>" + myAssignedClient.Name + "</span>) Client bill.</span>");
                //    }
                //    else
                //    {
                //        sbFootnotes.Append("&nbsp;&nbsp;-&nbsp;&nbsp;Direct billed (" + myBillUtils.FormatMoney(currentClientBill.Total) + ") to client.");
                //    }

                //    sbFootnotes.Append("</div>");

                //    clientCount++;
                //}


                //myBillingRender.FootNotes = sbFootnotes.ToString();

            #endregion

            myBillingRender.PageNumber = "<span style='font-weight: bold;'>pg 1</span> of 1"; // +(myClientBill.ClientBills.Count + 1);

            StringBuilder sbBillLabel = new StringBuilder();

            if (myClientBillConfig.IncludeInGroupBill)
            {
                Group myGroup = new Group(myClientBillConfig.BillToGroupId.ToString());

                sbBillLabel.Append("Included in " + myGroup.Name + "'s group billing. Group pricing applied."); // <a href='javascript: updateGroupBillConfig(&quot;" + myGroup._id.ToString() + "&quot;,&quot;Pricing&quot;);'>Update Group Pricing.</a>");

                //sbBillLabel.Append("<span style='padding-left: 15px;'><a href='javascript: NavigateTopicPopup(&quot;54addabeb5655a6098e911e2&quot;);'>Help?</a></span>");

                myBillingRender.LegendAdsCost = "Group Price";
                myBillingRender.LegendEndUserCost = "Group Price";
                myBillingRender.LegendMessagingCost = "Group Price";
                myBillingRender.LegendTaxRate = "<span style='font-style: italic;'>Group Tax Rate</span>";
            }
            else
            {
                sbBillLabel.Append("Direct client billing.");

                // Check to see if groups are available for assignment
                foreach (Relationship currRelationship in myClient.Relationships)
                {
                    // If groups are available, update the label so it's obvious
                    if (currRelationship.MemberType == "Group")
                        groupAssignmentCount++;
                }

                if (groupAssignmentCount > 0)
                    sbBillLabel.Append(" Client may be assigned to 1 of " + groupAssignmentCount + " groups for group billing.");

                //sbBillLabel.Append("<span style='padding-left: 5px;'><a href='javascript: NavigateTopicPopup(&quot;54ad9c13b5655a5cf4f68b89&quot;);'>Help?</a></span>");

                myBillingRender.LegendAdsCost = "Client Price";
                myBillingRender.LegendEndUserCost = "Client Price";
                myBillingRender.LegendMessagingCost = "Client Price";
                myBillingRender.LegendTaxRate = "<span style='font-style: italic;'>Client Tax Rate</span>";
            }

            btnBillConfig.Enabled = true;

            var configType = "Client";

            // Override client config with group config if indicated
            if (myClientBillConfig.IncludeInGroupBill)
            {
                myClientBillConfig = new BillConfig(myClientBillConfig.BillToGroupId.ToString(), "Group", "");
                configType = "Group";
            }

            // Warn the admin that a default config is being used
            if (myClientBillConfig.UpdatedById.ToString() == Constants.Strings.DefaultEmptyObjectId)
            {
                sbBillLabel.Append("<div style='color: #ff0000; position: relative; top: 5px; margin-bottom: 15px;'>");
                sbBillLabel.Append("!!! Default (" + configType + ") configuration !!!");

                if (configType == "Group")
                {
                    sbBillLabel.Append("&nbsp;&nbsp;<a href='javascript: updateGroupBillConfig(&quot;" + myClientBillConfig.OwnerId.ToString() + "&quot;,&quot;General&quot;);'>Update Group Bill Config</a>");
                }

                sbBillLabel.Append("</div>");
            }

            myBillingRender.IncludedInGroupBillLabel = sbBillLabel.ToString();

            // Add the control to the form
            panelBilling.Controls.Add(myBillingRender);

        #endregion

        hiddenSerializedBill.Value = (new JavaScriptSerializer()).Serialize(myClientBill);
    }

    private string FindTierByName(BillConfig myBillConfig, string tierName)
    {
        var decodedTierValue = "";

        foreach (BillTier currentTier in myBillConfig.BillingTiers)
        {
            if (currentTier.TierType == tierName)
                return MACSecurity.Security.DecodeAndDecrypt(currentTier.TierValues, currentTier.OwnerId.ToString());
        }

        return decodedTierValue;
    }

    public void GetClientList()
    {
        dlClients.Items.Clear();

        // If loggedin user is not system admin or accounting user, filter client list to only clients granted access
        if (userRole != Constants.Roles.SystemAdministrator && userRole != Constants.Roles.AccountingUser)
        {
            List<string> userClients = mUtils.GetClientsForUser(userRole);

            foreach (string userClient in userClients)
            {
                var tmpUser = userClient.Split('|');

                ListItem li = new ListItem();
                li.Text = tmpUser[0];
                li.Value = tmpUser[1];

                dlClients.Items.Add(li);
            }
        }
        else
        {
            var clientList = new MacList("", "Client", "", "_id,Name");
            foreach (var li in clientList.ListItems.Select(item => new ListItem { Text = item.Attributes["Name"].Replace("&quot;", "\"").Trim(), Value = item.Attributes["_id"].Trim() }))
            {
                if (li.Text != "")
                    dlClients.Items.Add(li);
            }
        }

        var li0 = new ListItem { Text = @"All Clients (" + (dlClients.Items.Count) + @")", Value = MACServices.Constants.Strings.DefaultEmptyObjectId };
        dlClients.Items.Insert(0, li0);
    }

    protected void dlBillingCategory_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    protected void dlBillingFrequency_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    protected void dlClients_SelectedIndexChanged(object sender, EventArgs e)
    {
        //SetUserControls();
    }

    protected void dlBillingType_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
}