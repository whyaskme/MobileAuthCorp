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

    Group myGroup;

    MongoDatabase mongoDBConnectionPool = (MongoDatabase)HttpContext.Current.Application[cs.MongoDB];

    public string parentContainerId = "";
    public Decimal groupRate = 0.00M;

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
                _hiddenW.Value = "54e77356b5655a0a4c4e89a6";
            }
        }

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

        var selectedGroupId = hiddenY.Value;
        if (selectedGroupId != "")
        {
            myGroup = new Group(selectedGroupId);

            divBillDocumentContainer.Visible = true;
            btnBillConfig.Enabled = true;

            GetBillDetails(selectedGroupId);
        }

        divUpdateMsg.Visible = false;
        updateMessage.InnerHtml = "";

        if (IsPostBack)
        {
            // Save the bill info
            switch (hiddenAA.Value)
            {
                case "SaveBill":
                    var myBill = (new JavaScriptSerializer()).Deserialize<BillGroup>(hiddenSerializedBill.Value);

                    myBill._id = ObjectId.Parse(_hiddenV.Value);
                    //myBill.GroupId = ObjectId.Parse(selectedGroupId);
                    myBill.GroupId = selectedGroupId;

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

                    myArchive.OwnerId = myBill.GroupId;

                    var myJsonBill = (new JavaScriptSerializer()).Serialize(myBill);

                    myArchive._id = myBill._id;

                    //myArchive.BillDetails = MACSecurity.Security.EncryptAndEncode(myJsonBill, myBill.GroupId.ToString());
                    myArchive.BillDetails = myJsonBill;
                    //myArchive.BillDetails = myJsonBill;

                    myArchive.Amount = myBill.Total;

                    myArchive.CreatedById = ObjectId.Parse(_hiddenE.Value);

                    myArchive.DateCreated = DateTime.UtcNow;
                    myArchive.DateDue = myBill.DateDue;

                    //myArchive.OwnerId = ObjectId.Parse(selectedGroupId);
                    myArchive.OwnerId = selectedGroupId;
                    myArchive.OwnerName = myGroup.Name;
                    myArchive.OwnerType = "Group";

                    myArchive.Archive(myArchive);

                    var adminProfile = new UserProfile(_hiddenE.Value);
                    var adminFirstName = MACSecurity.Security.DecodeAndDecrypt(adminProfile.FirstName, _hiddenE.Value);
                    var adminLastName = MACSecurity.Security.DecodeAndDecrypt(adminProfile.LastName, _hiddenE.Value);

                    // Log the billing event
                    var myBillEvent = new Event();
                    myBillEvent.EventTypeDesc = Constants.TokenKeys.BillAmount + "<a href='/Admin/Billing/Group/Default.aspx?bid=" + myBill._id + "&pa=ViewBillDetails' id='link_viewBillDetails'>" + myBillUtils.FormatMoney(Convert.ToDecimal(myBill.Total.ToString())) + "</a>";
                    myBillEvent.EventTypeDesc += Constants.TokenKeys.ClientName + myArchive.OwnerName;
                    myBillEvent.EventTypeDesc += Constants.TokenKeys.EventGeneratedByName + (adminFirstName + " " + adminLastName);

                    myBillEvent.EventTypeName = Constants.EventLog.Billing.Created.Item2;
                    myBillEvent.EventTypeId = Constants.EventLog.Billing.Created.Item1;
                    myBillEvent.ClientId = ObjectId.Parse(selectedGroupId);

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
                    //myOldBill.ClientId = ObjectId.Parse(selectedGroupId);

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

        }

        GetGroupList(selectedGroupId);

        var decryptedUserId = MACSecurity.Security.DecodeAndDecrypt(_hiddenE.Value, Constants.Strings.DefaultClientId);

        if (Convert.ToBoolean(MACSecurity.Security.DecodeAndDecrypt(_hiddenF.Value, decryptedUserId)))
        {
            btnBillSave.Visible = false;
            btnBillPay.Visible = false;
            btnBillVoid.Visible = false;
            btnBillBack.Visible = false;
        }
    }

    public void GetBillDetails(string selectedGroupId)
    {
        // Check to see if there is any billing history
        btnBillHistory.Enabled = false;

        var archiveQuery = Query.And(Query.EQ("_t", "BillArchive"), Query.EQ("OwnerId", selectedGroupId));
        var archiveSortBy = SortBy.Descending("DateCreated");

        var billHistory = mongoDBConnectionPool.GetCollection("Archive").FindAs<BillArchive>(archiveQuery).SetSortOrder(archiveSortBy);
        foreach (BillArchive billArchive in billHistory)
        {
            btnBillHistory.Enabled = true;
            btnBillSend.Enabled = true;
            //btnBillPrint.Enabled = true;
        }

        var i = 0;

        UserControls_BillingRender myBillingRender;

        BillUtils myBillUtils = new BillUtils();
        Utils myUtils = new Utils();

        Group myGroup = new Group(selectedGroupId);

        BillGroup myGroupBill = new BillGroup(dlDateRange.SelectedItem.Text, selectedGroupId, _hiddenE.Value);
        BillConfig myGroupBillConfig = new BillConfig(selectedGroupId, "Group", "");
        BillConfig myClientBillConfig;

        var miscTotal = 0M;

        var iCount = 1;

        #region Group Bill

            myBillingRender = (UserControls_BillingRender)Page.LoadControl("~/UserControls/BillingRender.ascx");

            myBillingRender.ClientLogo = myGroup.OwnerLogoUrl;
            myBillingRender.ClientID = myGroup._id.ToString();

            myBillingRender.BillNumber = myGroupBill._id.ToString();

            #region Set Labels

                //myBillingRender.ClientIdLabel = "GroupId";
                myBillingRender.ClientNameLabel = "Group";

                // Set form labels
                myBillingRender.LegendAdsCost = "Group Price";
                myBillingRender.LegendEndUserCost = "Group Price";
                myBillingRender.LegendMessagingCost = "Group Price";
                myBillingRender.LegendTaxRate = "<em>Group Tax Rate</em>";

                myBillingRender.IncludedInGroupBillLabel = "Master Group Bill for " + myGroup.Name + " - " + (myGroupBill.ClientBills.Count+1) + " pages";

            #endregion

            #region General Bill Info

                myBillingRender.ID = ObjectId.GenerateNewId().ToString();

                //myBillingRender.BillNumber = myGroupBill._id.ToString();
                _hiddenV.Value = myGroupBill._id.ToString();  //myBillingRender.BillNumber;

                // Set each bill's container ID to hyperlink the Client Members legend
                myBillingRender.BillContainerId = "divBillDocumentContainer_" + myGroupBill._id.ToString();  //myBillingRender.BillNumber;

                parentContainerId = myBillingRender.BillContainerId;

                myBillingRender.ClientMemberNumber = "";

                myBillingRender.DateCreated = myGroupBill.DateCreated.ToString();
                myBillingRender.DateDue = myGroupBill.DateDue.ToString();
                myBillingRender.DateSent = "Not Sent";

                if (myGroupBill.DatePaid.ToString() == "1/1/0001 12:00:00 AM")
                    myBillingRender.DatePaid = "Not Paid";
                else
                    myBillingRender.DatePaid = myGroupBill.DatePaid.ToString();

            #endregion

            #region Organization Info

                //myBillingRender.ClientID = selectedGroupId;

                myBillingRender.ClientName = myGroup.Name;

                if (myGroupBillConfig.Organization.TaxId != "")
                    myBillingRender.TaxId = MACSecurity.Security.DecodeAndDecrypt(myGroupBillConfig.Organization.TaxId, selectedGroupId);
                else
                    myBillingRender.TaxId = "Not specified";

                if (myGroupBillConfig.Organization.Phone != "")
                    myBillingRender.Phone = MACSecurity.Security.DecodeAndDecrypt(myGroupBillConfig.Organization.Phone, selectedGroupId);
                else
                    myBillingRender.Phone = "Not specified";

                if (myGroupBillConfig.Organization.Street1 != "")
                    myBillingRender.Street1 = MACSecurity.Security.DecodeAndDecrypt(myGroupBillConfig.Organization.Street1, selectedGroupId);
                else
                    myBillingRender.Street1 = "Not specified";

                if (myGroupBillConfig.Organization.Street2 != "")
                    myBillingRender.Street2 = MACSecurity.Security.DecodeAndDecrypt(myGroupBillConfig.Organization.Street2, selectedGroupId);
                else
                    myBillingRender.Street2 = "Not specified";

                if (myGroupBillConfig.Organization.City != "")
                    myBillingRender.City = MACSecurity.Security.DecodeAndDecrypt(myGroupBillConfig.Organization.City, selectedGroupId);
                else
                    myBillingRender.City = "Not specified";

                // Need to lookup state by id
                if (myGroupBillConfig.Organization.State.ToString() != MACServices.Constants.Strings.DefaultEmptyObjectId)
                    myBillingRender.State = myUtils.GetStateById(myGroupBillConfig.Organization.State.ToString());
                else
                    myBillingRender.State = "Not specified";

                if (myGroupBillConfig.Organization.Zipcode != "")
                    myBillingRender.ZipCode = MACSecurity.Security.DecodeAndDecrypt(myGroupBillConfig.Organization.Zipcode, selectedGroupId);
                else
                    myBillingRender.ZipCode = "Not specified";


            #endregion

            #region Otp Charges

                // Email
                // Reset groupRate
                groupRate = 0.00M;
                myBillingRender.OtpEmailCount = myBillUtils.FormatNumber(myGroupBill.OtpSentEmailCount);
                var otpRateSentEmail = FindTierByName(myGroupBillConfig, BillConstants.Tiers.OtpSentEmail).Split(char.Parse(cnt.Common.ItemSep));
                foreach (var currentCost in otpRateSentEmail)
                {
                    var currentRateElement = currentCost.Split(char.Parse(cnt.Common.KVSep));
                    var rateCount = Convert.ToInt32(currentRateElement[0]);
                    Decimal rateValue = Convert.ToDecimal(currentRateElement[1]);

                    if (groupRate == 0.00M)
                        groupRate = rateValue;

                    // If the actual event count is greater than the current rate count, set the price according to rate
                    if (Convert.ToInt32(myBillingRender.OtpEmailCount.Replace(",", "")) > rateCount)
                        groupRate = rateValue;
                }
                myBillingRender.OtpEmailCost = myBillUtils.FormatCost(groupRate);
                myBillingRender.OtpEmailCharge = myBillUtils.FormatMoney(myGroupBill.OtpSentEmailAmount);

                // SMS
                // Reset groupRate
                groupRate = 0.00M;
                myBillingRender.OtpSmsCount = myBillUtils.FormatNumber(myGroupBill.OtpSentSmsCount);
                var otpRateSentSms = FindTierByName(myGroupBillConfig, BillConstants.Tiers.OtpSentSms).Split(char.Parse(cnt.Common.ItemSep));
                foreach (var currentCost in otpRateSentSms)
                {
                    var currentRateElement = currentCost.Split(char.Parse(cnt.Common.KVSep));
                    var rateCount = Convert.ToInt32(currentRateElement[0]);
                    Decimal rateValue = Convert.ToDecimal(currentRateElement[1]);

                    if (groupRate == 0.00M)
                        groupRate = rateValue;

                    // If the actual event count is greater than the current rate count, set the price according to rate
                    if (Convert.ToInt32(myBillingRender.OtpSmsCount.Replace(",","")) > rateCount)
                        groupRate = rateValue;
                }
                myBillingRender.OtpSmsCost = myBillUtils.FormatCost(groupRate);
                myBillingRender.OtpSmsCharge = myBillUtils.FormatMoney(myGroupBill.OtpSentSmsAmount);
                

                // Voice
                // Reset groupRate
                groupRate = 0.00M;
                myBillingRender.OtpVoiceCount = myBillUtils.FormatNumber(myGroupBill.OtpSentVoiceCount);
                var otpRateSentVoice = FindTierByName(myGroupBillConfig, BillConstants.Tiers.OtpSentVoice).Split(char.Parse(cnt.Common.ItemSep));
                foreach (var currentCost in otpRateSentVoice)
                {
                    var currentRateElement = currentCost.Split(char.Parse(cnt.Common.KVSep));
                    var rateCount = Convert.ToInt32(currentRateElement[0]);
                    Decimal rateValue = Convert.ToDecimal(currentRateElement[1]);

                    if (groupRate == 0.00M)
                        groupRate = rateValue;

                    // If the actual event count is greater than the current rate count, set the price according to rate
                    if (Convert.ToInt32(myBillingRender.OtpVoiceCount.Replace(",", "")) > rateCount)
                        groupRate = rateValue;
                }
                myBillingRender.OtpVoiceCost = myBillUtils.FormatCost(groupRate);
                myBillingRender.OtpVoiceCharge = myBillUtils.FormatMoney(myGroupBill.OtpSentVoiceAmount);
                

                myBillingRender.OtpTotal = myBillUtils.FormatMoney((myGroupBill.OtpSentEmailAmount + myGroupBill.OtpSentSmsAmount + myGroupBill.OtpSentVoiceAmount));

            #endregion

            #region Advertising Charges

                // OTP screen
                // Reset groupRate
                groupRate = 0.00M;
                myBillingRender.AdEnterOtpScreenSentCount = myBillUtils.FormatNumber(myGroupBill.AdEnterOtpScreenSentCount);
                var adOtpScreen = FindTierByName(myGroupBillConfig, BillConstants.Tiers.OtpSentVoice).Split(char.Parse(cnt.Common.ItemSep));
                foreach (var currentCost in adOtpScreen)
                {
                    var currentRateElement = currentCost.Split(char.Parse(cnt.Common.KVSep));
                    var rateCount = Convert.ToInt32(currentRateElement[0]);
                    Decimal rateValue = Convert.ToDecimal(currentRateElement[1]);

                    if (groupRate == 0.00M)
                        groupRate = rateValue;

                    // If the actual event count is greater than the current rate count, set the price according to rate
                    if (Convert.ToInt32(myBillingRender.AdEnterOtpScreenSentCount.Replace(",", "")) > rateCount)
                        groupRate = rateValue;
                }
                myBillingRender.AdEnterOtpScreenSentCost = myBillUtils.FormatCost(groupRate);
                myBillingRender.AdEnterOtpScreenSentAmount = myBillUtils.FormatMoney(myGroupBill.AdEnterOtpScreenSentAmount);

                // Ad message
                // Reset groupRate
                groupRate = 0.00M;
                myBillingRender.AdMessageSentCount = myBillUtils.FormatNumber(myGroupBill.AdMessageSentCount);
                var adMessage = FindTierByName(myGroupBillConfig, BillConstants.Tiers.OtpSentVoice).Split(char.Parse(cnt.Common.ItemSep));
                foreach (var currentCost in adMessage)
                {
                    var currentRateElement = currentCost.Split(char.Parse(cnt.Common.KVSep));
                    var rateCount = Convert.ToInt32(currentRateElement[0]);
                    Decimal rateValue = Convert.ToDecimal(currentRateElement[1]);

                    if (groupRate == 0.00M)
                        groupRate = rateValue;

                    // If the actual event count is greater than the current rate count, set the price according to rate
                    if (Convert.ToInt32(myBillingRender.AdMessageSentCount.Replace(",", "")) > rateCount)
                        groupRate = rateValue;
                }
                myBillingRender.AdMessageSentCost = myBillUtils.FormatCost(groupRate);
                myBillingRender.AdMessageSentAmount = myBillUtils.FormatMoney(myGroupBill.AdMessageSentAmount);

                // Verification screen
                // Reset groupRate
                groupRate = 0.00M;
                myBillingRender.AdVerificationScreenSentCount = myBillUtils.FormatNumber(myGroupBill.AdVerificationScreenSentCount);
                var adVerification = FindTierByName(myGroupBillConfig, BillConstants.Tiers.OtpSentVoice).Split(char.Parse(cnt.Common.ItemSep));
                foreach (var currentCost in adVerification)
                {
                    var currentRateElement = currentCost.Split(char.Parse(cnt.Common.KVSep));
                    var rateCount = Convert.ToInt32(currentRateElement[0]);
                    Decimal rateValue = Convert.ToDecimal(currentRateElement[1]);

                    if (groupRate == 0.00M)
                        groupRate = rateValue;

                    // If the actual event count is greater than the current rate count, set the price according to rate
                    if (Convert.ToInt32(myBillingRender.AdVerificationScreenSentCount.Replace(",", "")) > rateCount)
                        groupRate = rateValue;
                }
                myBillingRender.AdVerificationScreenSentCost = myBillUtils.FormatCost(groupRate);
                myBillingRender.AdVerificationScreenSentAmount = myBillUtils.FormatMoney(myGroupBill.AdVerificationScreenSentAmount);

                myBillingRender.AdPassTotal = myBillUtils.FormatMoney((myGroupBill.AdEnterOtpScreenSentAmount + myGroupBill.AdMessageSentAmount + myGroupBill.AdVerificationScreenSentAmount));

            #endregion

            #region End User Charges

                // Registration
                // Reset groupRate
                groupRate = 0.00M;
                myBillingRender.EndUserRegistrationCount = myBillUtils.FormatNumber(myGroupBill.EndUserRegistrationCount);
                var endUserRegistration = FindTierByName(myGroupBillConfig, BillConstants.Tiers.OtpSentVoice).Split(char.Parse(cnt.Common.ItemSep));
                foreach (var currentCost in endUserRegistration)
                {
                    var currentRateElement = currentCost.Split(char.Parse(cnt.Common.KVSep));
                    var rateCount = Convert.ToInt32(currentRateElement[0]);
                    Decimal rateValue = Convert.ToDecimal(currentRateElement[1]);

                    if (groupRate == 0.00M)
                        groupRate = rateValue;

                    // If the actual event count is greater than the current rate count, set the price according to rate
                    if (Convert.ToInt32(myBillingRender.EndUserRegistrationCount.Replace(",", "")) > rateCount)
                        groupRate = rateValue;
                }
                myBillingRender.EndUserRegistrationCost = myBillUtils.FormatCost(groupRate);
                myBillingRender.EndUserRegistrationAmount = myBillUtils.FormatMoney(myGroupBill.EndUserRegistrationAmount);

                // Verification
                // Reset groupRate
                groupRate = 0.00M;
                myBillingRender.EndUserVerificationCount = myBillUtils.FormatNumber(myGroupBill.EndUserVerificationCount);
                var endUserVerification = FindTierByName(myGroupBillConfig, BillConstants.Tiers.OtpSentVoice).Split(char.Parse(cnt.Common.ItemSep));
                foreach (var currentCost in endUserVerification)
                {
                    var currentRateElement = currentCost.Split(char.Parse(cnt.Common.KVSep));
                    var rateCount = Convert.ToInt32(currentRateElement[0]);
                    Decimal rateValue = Convert.ToDecimal(currentRateElement[1]);

                    if (groupRate == 0.00M)
                        groupRate = rateValue;

                    // If the actual event count is greater than the current rate count, set the price according to rate
                    if (Convert.ToInt32(myBillingRender.EndUserVerificationCount.Replace(",", "")) > rateCount)
                        groupRate = rateValue;
                }
                myBillingRender.EndUserVerificationCost = myBillUtils.FormatCost(groupRate);
                myBillingRender.EndUserVerificationAmount = myBillUtils.FormatMoney(myGroupBill.EndUserVerificationAmount);

                myBillingRender.EndUserTotal = myBillUtils.FormatMoney((myGroupBill.EndUserRegistrationAmount + myGroupBill.EndUserVerificationAmount));

            #endregion

            #region Miscellaneous Charges

                myBillingRender.ShowMiscCharges = false;

                miscTotal = 0M;

                iCount = 1;

                if (myGroupBill.Addendums.Count > 0)
                {
                    StringBuilder sbAddendums = new StringBuilder();

                    myBillingRender.ShowMiscCharges = true;

                    sbAddendums.Append("<table style='border: solid 0px #0000ff;'>");

                    foreach (BillAddendum myAddendum in myGroupBill.Addendums)
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

                    //sbAddendums.Append("<tr style='border: solid 0px #0000ff;'>");
                    //sbAddendums.Append("<td>");
                    //sbAddendums.Append("Client summary here...");
                    //sbAddendums.Append("</td>");
                    //sbAddendums.Append("</tr>");

                    sbAddendums.Append("</table>");

                    myBillingRender.AddendumsContainer = sbAddendums.ToString();
                }
                myBillingRender.MiscTotal = myBillUtils.FormatMoney(myGroupBill.AddendumSubtotal);

            #endregion

            #region SubTotal, Tax Rate, Taxes and Total

                myBillingRender.Subtotal = myBillUtils.FormatMoney(myGroupBill.SubTotal);

                myBillingRender.TaxRate = myGroupBill.TaxRate.ToString();
                myBillingRender.SalesTax = myBillUtils.FormatMoney(myGroupBill.SalesTax);

                myBillingRender.Total = myBillUtils.FormatMoney(myGroupBill.Total);

                if (myGroupBill.Total > 0.00M)
                    btnBillSave.Enabled = true;

            #endregion

            #region Footnotes

                StringBuilder sbFootnotes = new StringBuilder();
                sbFootnotes.Append("<div id='divClientSummaryContainer' style='border-bottom: solid 0px #c0c0c0; position: relative; top: 25px; padding-bottom: 10px; margin-bottom: 25px; font-weight: bold;'>" + myGroupBill.ClientBills.Count.ToString() + " Group Clients Summary</div>");

                var clientCount = 1;

                // Need to parse this from the group properties.

                foreach (BillClient currentClientBill in myGroupBill.ClientBills)
                {
                    Client myClient = new Client(currentClientBill.ClientId.ToString());

                    sbFootnotes.Append("<div style='border-bottom: solid 0px #c0c0c0; font-weight: normal; margin-bottom: 10px;'>");

                    sbFootnotes.Append("<div style='float: left; position: relative; left: 0px; border: solid 0px #ff0000; width: 45px; text-align: left; margin-right: 3px;'>pg " + (clientCount+1) + "</div>");

                    sbFootnotes.Append("<a href='#divBillDocumentContainer_" + currentClientBill._id + "' id='link_currentClientBill'>");
                    sbFootnotes.Append("<div style='float: left;'>");
                    sbFootnotes.Append(myClient.Name);
                    sbFootnotes.Append("</div>");
                    sbFootnotes.Append("</a>");

                    myClientBillConfig = new BillConfig(currentClientBill.ClientId.ToString(), "Client", "");

                    Group myAssignedGroup;

                    if (myClientBillConfig.BillToGroupId.ToString() == selectedGroupId)
                    {
                        myAssignedGroup = new Group(myClientBillConfig.BillToGroupId.ToString());

                        sbFootnotes.Append("&nbsp;&nbsp;-&nbsp;&nbsp;<span style='color: #ff0000;'>Included (" + myBillUtils.FormatMoney(currentClientBill.Total) + ") in this group bill.</span>");
                    }
                    else if (myClientBillConfig.IncludeInGroupBill)
                    {
                        myAssignedGroup = new Group(myClientBillConfig.BillToGroupId.ToString());

                        sbFootnotes.Append("&nbsp;&nbsp;-&nbsp;&nbsp;<span style=''>Charged (" + myBillUtils.FormatMoney(currentClientBill.Total) + ") to the (<span style='font-weight: bold;'>" + myAssignedGroup.Name + "</span>) group bill.</span>");
                    }
                    else
                    {
                        sbFootnotes.Append("&nbsp;&nbsp;-&nbsp;&nbsp;Direct billed (" + myBillUtils.FormatMoney(currentClientBill.Total) + ") to client.");
                    }

                    sbFootnotes.Append("</div>");

                    clientCount++;
                }


                myBillingRender.FootNotes = sbFootnotes.ToString();

            #endregion

            myBillingRender.PageNumber = "<span style='font-weight: bold;'>pg 1</span> of " + (myGroupBill.ClientBills.Count + 1);

            // Add the control to the form
            panelBilling.Controls.Add(myBillingRender);

        #endregion

        #region Client Bills

            foreach (BillClient currentClientBill in myGroupBill.ClientBills)
            {
                // Get a new instance of BillingChild
                myBillingRender = (UserControls_BillingRender)Page.LoadControl("~/UserControls/BillingRender.ascx");

                myClientBillConfig = new BillConfig(currentClientBill.ClientId.ToString(), "Client", "");

                var billingType = "Direct billed to Client";
                if (myClientBillConfig.IncludeInGroupBill)
                {
                    // Set form labels
                    myBillingRender.LegendAdsCost = "Group Price";
                    myBillingRender.LegendEndUserCost = "Group Price";
                    myBillingRender.LegendMessagingCost = "Group Price";
                    myBillingRender.LegendTaxRate = "<em>Group Tax Rate</em>";

                    myGroup = new Group(myClientBillConfig.BillToGroupId.ToString());

                    if (myClientBillConfig.BillToGroupId.ToString() == selectedGroupId)
                    {
                        billingType = "<span style='color: #ff0000;'>Included in this group bill</span>";
                    }
                    else
                        billingType = "Charged to (<span style='font-weight: bold;'>" + myGroup.Name + "</span>) group bill";
                }

                myBillingRender.IncludedInGroupBillLabel = billingType;

                #region General Bill Info

                    myBillingRender.ID = ObjectId.GenerateNewId().ToString();
                    //myBillingRender.BillNumber = currentClientBill._id.ToString();

                    // Set each bill's container ID to hyperlink the Client Members legend
                    myBillingRender.BillContainerId = "divBillDocumentContainer_" + currentClientBill._id.ToString();  //myBillingRender.BillNumber;

                    myBillingRender.ClientMemberNumber = "Client Member #" + (i + 1) + " - <a href='#divSubTotalTaxesTotalBilling' id='link_backToTop'>Back to Top</a>";

                    myBillingRender.DateCreated = currentClientBill.DateCreated.ToString();
                    myBillingRender.DateDue = currentClientBill.DateDue.ToString();
                    myBillingRender.DateSent = currentClientBill.DateSent.ToString();

                    //myBillingRender.DatePaid = currentClientBill.DatePaid.ToString();
                    if (currentClientBill.DatePaid.ToString() == "1/1/0001 12:00:00 AM")
                        myBillingRender.DatePaid = "Not Paid";
                    else
                        myBillingRender.DatePaid = currentClientBill.DatePaid.ToString();

                #endregion

                #region Organization Info

                    //myBillingRender.ClientID = ObjectId.GenerateNewId().ToString();

                    Client myClient = new Client(currentClientBill.ClientId.ToString());

                    myBillingRender.ClientID = myClient._id.ToString();

                    myBillingRender.BillNumber = currentClientBill._id.ToString();

                    myBillingRender.ClientLogo = myClient.OwnerLogoUrl;

                    myBillingRender.ClientName = myClient.Name;

                    if (myClient.Organization.TaxId != "")
                        myBillingRender.TaxId = MACSecurity.Security.DecodeAndDecrypt(myClientBillConfig.Organization.TaxId, currentClientBill.ClientId.ToString());
                    else
                        myBillingRender.TaxId = "Not specified";

                    if (myClient.Organization.Phone != "")
                        myBillingRender.Phone = MACSecurity.Security.DecodeAndDecrypt(myClientBillConfig.Organization.Phone, currentClientBill.ClientId.ToString());
                    else
                        myBillingRender.Phone = "Not specified";

                    if (myClient.Organization.Street1 != "")
                        myBillingRender.Street1 = MACSecurity.Security.DecodeAndDecrypt(myClientBillConfig.Organization.Street1, currentClientBill.ClientId.ToString());
                    else
                        myBillingRender.Street1 = "Not specified";

                    if (myClient.Organization.Street2 != "")
                        myBillingRender.Street2 = MACSecurity.Security.DecodeAndDecrypt(myClientBillConfig.Organization.Street2, currentClientBill.ClientId.ToString());
                    else
                        myBillingRender.Street2 = "Not specified";

                    if (myClient.Organization.City != "")
                        myBillingRender.City = MACSecurity.Security.DecodeAndDecrypt(myClientBillConfig.Organization.City, currentClientBill.ClientId.ToString());
                    else
                        myBillingRender.City = "Not specified";

                    // Need to lookup state by id
                    if (myClient.Organization.State.ToString() != MACServices.Constants.Strings.DefaultEmptyObjectId)
                        myBillingRender.State = myUtils.GetStateById(myClientBillConfig.Organization.State.ToString());
                    else
                        myBillingRender.State = "Not specified";

                    if (myClient.Organization.Zipcode != "")
                        myBillingRender.ZipCode = MACSecurity.Security.DecodeAndDecrypt(myClientBillConfig.Organization.Zipcode, currentClientBill.ClientId.ToString());
                    else
                        myBillingRender.ZipCode = "Not specified";

                #endregion

                #region Otp Charges

                    myBillingRender.OtpEmailCharge = myBillUtils.FormatMoney(currentClientBill.OtpSentEmailAmount);
                    myBillingRender.OtpEmailCost = myBillUtils.FormatCost(currentClientBill.OtpSentEmailCost);
                    myBillingRender.OtpEmailCount = myBillUtils.FormatNumber(currentClientBill.OtpSentEmailCount);

                    myBillingRender.OtpSmsCharge = myBillUtils.FormatMoney(currentClientBill.OtpSentSmsAmount);
                    myBillingRender.OtpSmsCost = myBillUtils.FormatCost(currentClientBill.OtpSentSmsCost);
                    myBillingRender.OtpSmsCount = myBillUtils.FormatNumber(currentClientBill.OtpSentSmsCount);

                    myBillingRender.OtpVoiceCharge = myBillUtils.FormatMoney(currentClientBill.OtpSentVoiceAmount);
                    myBillingRender.OtpVoiceCost = myBillUtils.FormatCost(currentClientBill.OtpSentVoiceCost);
                    myBillingRender.OtpVoiceCount = myBillUtils.FormatNumber(currentClientBill.OtpSentVoiceCount);

                    myBillingRender.OtpTotal = myBillUtils.FormatMoney((currentClientBill.OtpSentEmailAmount + currentClientBill.OtpSentSmsAmount + currentClientBill.OtpSentVoiceAmount));

                #endregion

                #region Advertising Charges

                myBillingRender.AdEnterOtpScreenSentAmount = myBillUtils.FormatMoney(currentClientBill.AdEnterOtpScreenSentAmount);
                myBillingRender.AdEnterOtpScreenSentCost = myBillUtils.FormatCost(currentClientBill.AdEnterOtpScreenSentCost);
                myBillingRender.AdEnterOtpScreenSentCount = myBillUtils.FormatNumber(currentClientBill.AdEnterOtpScreenSentCount);

                myBillingRender.AdMessageSentAmount = myBillUtils.FormatMoney(currentClientBill.AdMessageSentAmount);
                myBillingRender.AdMessageSentCost = myBillUtils.FormatCost(currentClientBill.AdMessageSentCost);
                myBillingRender.AdMessageSentCount = myBillUtils.FormatNumber(currentClientBill.AdMessageSentCount);

                myBillingRender.AdVerificationScreenSentAmount = myBillUtils.FormatMoney(currentClientBill.AdVerificationScreenSentAmount);
                myBillingRender.AdVerificationScreenSentCost = myBillUtils.FormatCost(currentClientBill.AdVerificationScreenSentCost);
                myBillingRender.AdVerificationScreenSentCount = myBillUtils.FormatNumber(currentClientBill.AdVerificationScreenSentCount);

                myBillingRender.AdPassTotal = myBillUtils.FormatMoney((currentClientBill.AdEnterOtpScreenSentAmount + currentClientBill.AdMessageSentAmount + currentClientBill.AdVerificationScreenSentAmount));

                #endregion

                #region End User Charges

                myBillingRender.EndUserRegistrationAmount = myBillUtils.FormatMoney(currentClientBill.EndUserRegistrationAmount);
                myBillingRender.EndUserRegistrationCost = myBillUtils.FormatCost(currentClientBill.EndUserRegistrationCost);
                myBillingRender.EndUserRegistrationCount = myBillUtils.FormatNumber(currentClientBill.EndUserRegistrationCount);

                myBillingRender.EndUserVerificationAmount = myBillUtils.FormatMoney(currentClientBill.EndUserVerificationAmount);
                myBillingRender.EndUserVerificationCost = myBillUtils.FormatCost(currentClientBill.EndUserVerificationCost);
                myBillingRender.EndUserVerificationCount = myBillUtils.FormatNumber(currentClientBill.EndUserVerificationCount);

                myBillingRender.EndUserTotal = myBillUtils.FormatMoney((currentClientBill.EndUserRegistrationAmount + currentClientBill.EndUserVerificationAmount));

                #endregion

                #region Miscellaneous Charges

                    myBillingRender.ShowMiscCharges = false;

                    miscTotal = 0M;

                    iCount = 1;

                    if (currentClientBill.Addendums.Count > 0)
                    {
                        StringBuilder sbAddendums = new StringBuilder();

                        myBillingRender.ShowMiscCharges = true;

                        sbAddendums.Append("<table style='border: solid 0px #0000ff;'>");

                        foreach (BillAddendum myAddendum in currentClientBill.Addendums)
                        {
                            var encKey = myAddendum.OwnerId.ToString();

                            //var tmpAddAmount = MACSecurity.Security.DecodeAndDecrypt(myAddendum.Amount, encKey).Replace("$", "");
                            var tmpAddAmount = myAddendum.Amount;

                            miscTotal += Convert.ToDecimal(tmpAddAmount);

                            //var tmpAddNotes = MACSecurity.Security.DecodeAndDecrypt(myAddendum.Notes, encKey);
                            var tmpAddNotes = myAddendum.Notes;

                            sbAddendums.Append("<tr style='border: solid 0px #0000ff;'>");

                            sbAddendums.Append("    <td class='tdMisc50' style='width: 50%;'>");
                            sbAddendums.Append("            <span style='color: #808080;'>" + iCount + ".</span> ");
                            sbAddendums.Append("            <span runat='server' class='billingData small-only-text-center' style='position: relative; left: 0px;'>" + tmpAddNotes + "</span>");
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

                        myBillingRender.AddendumsContainer = sbAddendums.ToString();
                    }

                    myBillingRender.MiscTotal = myBillUtils.FormatMoney(miscTotal);

                #endregion

                #region SubTotal, Tax Rate, Taxes and Total

                    myBillingRender.TaxRate = currentClientBill.TaxRate.ToString();
                    myBillingRender.SalesTax = myBillUtils.FormatMoney(currentClientBill.SalesTax);
                    myBillingRender.Subtotal = myBillUtils.FormatMoney(currentClientBill.SubTotal);
                    myBillingRender.Total = myBillUtils.FormatMoney(currentClientBill.Total);

                #endregion

                myBillingRender.ShowFootNotes = false;
                myBillingRender.PageNumber = "<span style='font-weight: bold;'>pg " + (i + 2) + "</span> of " + (myGroupBill.ClientBills.Count + 1);

                // Add the BillingChild to the form
                panelBilling.Controls.Add(myBillingRender);

                i++;
            }

        #endregion

        hiddenSerializedBill.Value = (new JavaScriptSerializer()).Serialize(myGroupBill);
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

    public void GetGroupList(string selectedGroupId)
    {
        var groupCount = 0;

        dlGroups.Items.Clear();

        // Get available groups and subgroups
        var groupList = new MacListAdHoc("Group", "GroupType", "RootGroup", true, "Relationships,MacOasServicesUrl");

        // Here we need to process the xml response into a List collection
        var xmlDoc = groupList.ListXml;
        var xmlGroups = xmlDoc.GetElementsByTagName("group");

        foreach (XmlNode currentGroup in xmlGroups)
        {
            if (currentGroup.Attributes != null)
            {
                var groupName = currentGroup.Attributes[0].Value;
                var groupId = currentGroup.Attributes[1].Value;
                var groupLevel = currentGroup.Attributes[2].Value;

                var groupEnabled = currentGroup.Attributes[3].Value;

                var li = new ListItem { Text = groupLevel + @") " + groupName, Value = groupId };

                if (groupId == selectedGroupId)
                    li.Selected = true;

                li.Attributes.Add("class", "ListItemIndentLevel_" + groupLevel);

                if (!Convert.ToBoolean(groupEnabled))
                    li.Attributes.Add("style", "color: #ff0000");

                li.Attributes.Add("ondblclick", "javascript: setActiveGroup(this.value);");

                dlGroups.Items.Add(li);

                groupCount++;
            }
        }

        divGroupCount.InnerHtml = groupCount + " Groups Available";
    }
}