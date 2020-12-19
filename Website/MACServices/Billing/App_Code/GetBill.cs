using System;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Xml;

using MongoDB.Driver.Builders;

using MACServices;
using MACBilling;

using dk = MACServices.Constants.Dictionary.Keys;

/// <summary>
/// Summary description for GetBill
/// </summary>
[WebService(Namespace = "http://mobileauthcorp.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

[System.Web.Script.Services.ScriptService]
public class GetBill : WebService {

    bool HasBill = false;

    Decimal AdMessageSentCount = 0.00M;
    Decimal AdMessageSentCost = 0.00M;
    Decimal AdMessageSentAmount = 0.00M;
    Decimal AdEnterOtpScreenSentCount = 0.00M;
    Decimal AdEnterOtpScreenSentCost = 0.00M;
    Decimal AdEnterOtpScreenSentAmount = 0.00M;
    Decimal AdVerificationScreenSentCount = 0.00M;
    Decimal AdVerificationScreenSentCost = 0.00M;
    Decimal AdVerificationScreenSentAmount = 0.00M;
    Decimal OtpSentEmailCount = 0.00M;
    Decimal OtpSentEmailCost = 0.00M;
    Decimal OtpSentEmailAmount = 0.00M;
    Decimal OtpSentSmsCount = 0.00M;
    Decimal OtpSentSmsCost = 0.00M;
    Decimal OtpSentSmsAmount = 0.00M;
    Decimal OtpSentVoiceCount = 0.00M;
    Decimal OtpSentVoiceCost = 0.00M;
    Decimal OtpSentVoiceAmount = 0.00M;
    Decimal EndUserRegistrationCount = 0.00M;
    Decimal EndUserRegistrationCost = 0.00M;
    Decimal EndUserRegistrationAmount = 0.00M;
    Decimal EndUserVerificationCount = 0.00M;
    Decimal EndUserVerificationCost = 0.00M;
    Decimal EndUserVerificationAmount = 0.00M;

    Decimal price = 0.00M;

    Decimal AdsSentCount = 0.00M;
    Decimal AdsSentAmount = 0.00M;

    Decimal AdsClickedCount = 0.00M;
    Decimal AdsClickedAmount = 0.00M;

    BillUtils myBillUtils = new BillUtils();

    Utils mUtils = new Utils();

    StringBuilder sbResponse = new StringBuilder();
    StringBuilder sbClientData = new StringBuilder();

    BillClient myClientBill = new BillClient();
    BillGroup myGroupBill = new BillGroup();

    String selectedOwnerId = "";

    Boolean IsClientBilledToGroup = false;

    public GetBill () {}

    [WebMethod]
    public XmlDocument WsGetBill(string ownerId, string ownerType, string billDate)
    {
        var billMonth = DateTime.Now.Month.ToString();
        var billYear = DateTime.Now.Year.ToString();

        string renderResponse;

        if (string.IsNullOrEmpty(ownerId))
            ownerId = "53a9a7a4b5655a165cb03392"; // < Best Buy MACServices.Constants.Strings.DefaultClientId;

        // This let's us maintain the groupid when when processing a group bill
        selectedOwnerId = ownerId;

        if(string.IsNullOrEmpty(ownerType))
            ownerType = "Client";

        // Set defaults if empty or null
        if(!string.IsNullOrEmpty(billDate))
        {
            var tmpDate = billDate.Split('/');
            billMonth = tmpDate[0];
            billYear = tmpDate[1];
        }

        if (billMonth.Length == 1) // Must include a leading "0" if length == 1
            billMonth = "0" + billMonth;

        billDate = billMonth + "/" + billYear;

        var ownerName = "";
        switch(ownerType)
        {
            case "Client":
                Client myClient = new Client(ownerId);
                ownerName = myClient.Name;
                break;

            case "Group":
                Group myGroup = new Group(ownerId);
                ownerName = myGroup.Name;
                break;
        }

        mUtils.InitializeXmlResponse(sbResponse);

        try
        {
            sbResponse.Append("<Reply>");

            //var billQuery = Query.And(
            //    Query.EQ("_t", "BillArchive"), 
            //    Query.EQ("OwnerId", ObjectId.Parse(ownerId)), 
            //    Query.EQ("OwnerType", ownerType),
            //    Query.EQ("ForMonthYear", billDate)
            //    );

            var billQuery = Query.And(
                Query.EQ("_t", "BillArchive"),
                Query.EQ("OwnerId", ownerId),
                Query.EQ("OwnerType", ownerType),
                Query.EQ("ForMonthYear", billDate)
                );

            var sortBy = SortBy.Descending("DateCreated");

            var billHistory = mUtils.mongoDBConnectionPool.GetCollection("Archive").FindAs<BillArchive>(billQuery).SetSortOrder(sortBy);
            foreach (BillArchive billArchive in billHistory)
            {
                // We have a bill ready. Need to decrypt and convert JSon to xml for the service response.
                //var archiveData = MACSecurity.Security.DecodeAndDecrypt(billArchive.BillDetails, ownerId);
                var archiveData = billArchive.BillDetails;

                switch (ownerType)
                {
                    case "Client":
                        renderResponse = RenderArchiveXml(ownerName, ownerId, ownerType, archiveData);
                        sbResponse.Append(renderResponse);
                        break;

                    case "Group":
                        renderResponse = RenderArchiveXml(ownerName, ownerId, ownerType, archiveData);
                        sbResponse.Append(renderResponse);

                        myGroupBill = (new JavaScriptSerializer()).Deserialize<BillGroup>(archiveData);

                        if (myGroupBill.ClientBills.Count > 0)
                        {
                            sbClientData.Append("<Clients>");
                            foreach (BillClient currentClientBill in myGroupBill.ClientBills)
                            {
                                archiveData = (new JavaScriptSerializer()).Serialize(currentClientBill);

                                renderResponse = RenderArchiveXml(currentClientBill.OwnerName, currentClientBill.OwnerId.ToString(), "Client", archiveData);
                                sbClientData.Append(renderResponse);
                            }
                            sbClientData.Append("</Clients>");
                        }
                        break;
                }
            }

            if(!HasBill)
            {
                sbResponse.Append("<billarchive status='Bill not generated yet for " + ownerName + " with the specified date " + billDate + "' />");
            }
        }
// ReSharper disable once EmptyGeneralCatchClause
// ReSharper disable once UnusedVariable
        catch (Exception ex)
        {
            var errMsg = ex.ToString();
        }

        // Add client data
        sbResponse.Append(sbClientData.ToString());

        sbResponse.Append("</Reply>");

        var rsp = mUtils.FinalizeXmlResponse(sbResponse, "GB");
        return rsp;
    }

    private string RenderArchiveXml(string ownerName, string ownerId, string ownerType, string archiveData)
    {
        StringBuilder sbBillRender = new StringBuilder();
        StringBuilder sbBillSummary = new StringBuilder();
        StringBuilder sbBillDetails = new StringBuilder();
        StringBuilder sbBillAddendums = new StringBuilder();

        switch(ownerType)
        {
            case "Client":
                BillConfig clientBillConfig = new BillConfig(ownerId, "Client", Constants.Strings.DefaultAdminId);

                var assignedGroupName = "Unknown";
                var includeInGroupBill = clientBillConfig.IncludeInGroupBill;
                var billToGroupId = clientBillConfig.BillToGroupId.ToString();

                if (includeInGroupBill)
                {
                    Group clientGroup = new Group(billToGroupId);
                    assignedGroupName = clientGroup.Name;

                    if (billToGroupId == selectedOwnerId)
                    {
                        IsClientBilledToGroup = true;
                        sbBillRender.Append("<" + ownerType + " name='" + ownerName.Replace("'", "&apos;") + "' id='" + ownerId + "' directbilled='false' includedinthisbill='" + includeInGroupBill.ToString().ToLower() + "'>");
                    }
                    else
                    {
                        sbBillRender.Append("<" + ownerType + " name='" + ownerName.Replace("'", "&apos;") + "' id='" + ownerId + "' directbilled='false' includedinthisbill='false' billedtogroupname='" + assignedGroupName + "' billedtogroupid='" + billToGroupId + "'>");
                    }
                }
                else
                    sbBillRender.Append("<" + ownerType + " name='" + ownerName.Replace("'", "&apos;") + "' id='" + ownerId + "' directbilled='true' includedinthisbill='false'>");
                break;

            case "Group":
                sbBillRender.Append("<" + ownerType + " name='" + ownerName.Replace("'", "&apos;") + "' id='" + ownerId + "' directbilled='true' includedinthisbill='true'>");
                IsClientBilledToGroup = true;
                break;
        }

        if (IsClientBilledToGroup)
        {
            myClientBill = (new JavaScriptSerializer()).Deserialize<BillClient>(archiveData);

            #region Process properties into xml structure

                foreach (var property in myClientBill.GetType().GetProperties())
                {
                    var propName = property.Name;
                    var propValue = property.GetValue(myClientBill);

                    // Convert to xml
                    switch (propName)
                    {
                        case "_id":
                            // Ignore this property
                            break;

                        case "ClientId":
                            // Ignore this property
                            break;

                        case "CreatedById":
                            // Ignore this property
                            break;

                        case "UpdatedById":
                            // Ignore this property
                            break;

                        case "OwnerId":
                            // Ignore this property
                            break;

                        case "OwnerName":
                            // Ignore this property
                            break;

                        case "OwnerType":
                            // Ignore this property
                            break;

                        case "DateStart":
                            // Ignore this property
                            break;

                        case "DateEnd":
                            // Ignore this property
                            break;

                        case "DateCreated":
                            sbBillSummary.Append("<" + propName + ">" + propValue + "</" + propName + ">");
                            break;

                        case "DateDue":
                            sbBillSummary.Append("<" + propName + ">" + propValue + "</" + propName + ">");
                            break;

                        case "DateSent":
                            // Ignore this property
                            break;

                        case "DatePaid":
                            // Ignore this property
                            break;

                        case "DateVoided":
                            // Ignore this property
                            break;

                        case "Addendums":
                            sbBillAddendums.Append("<Addendums count='" + myBillUtils.FormatNumber(myClientBill.Addendums.Count) + "' amount='" + myBillUtils.FormatMoney(myClientBill.AddendumsAmount) + "'>");
                            // Decrypt addendums
                            foreach (BillAddendum currentAddendum in myClientBill.Addendums)
                            {
                                sbBillAddendums.Append("<Addendum>");

                                //var addendumAmount = MACSecurity.Security.DecodeAndDecrypt(currentAddendum.Amount, currentAddendum.OwnerId.ToString());
                                //var tmpAmount = MACSecurity.Security.DecodeAndDecrypt(addendumAmount, currentAddendum.OwnerId.ToString());
                                sbBillAddendums.Append("     <Amount>" + myBillUtils.FormatMoney(Convert.ToDecimal(currentAddendum.Amount)) + "</Amount>");

                                //var addendumNotes = MACSecurity.Security.DecodeAndDecrypt(currentAddendum.Notes, currentAddendum.OwnerId.ToString());
                                //var tmpNotes = MACSecurity.Security.DecodeAndDecrypt(addendumNotes, currentAddendum.OwnerId.ToString());
                                sbBillAddendums.Append("     <Notes>" + currentAddendum.Notes + "</Notes>");

                                sbBillAddendums.Append("</Addendum>");
                            }
                            sbBillAddendums.Append("</Addendums>");
                            break;

                        case "AddendumsAmount":
                            // Ignore this property
                            break;

                        case "AdMessageSentCount":
                            AdMessageSentCount += Convert.ToInt32(propValue);
                            AdsSentCount += Convert.ToInt32(propValue);
                            break;

                        case "AdMessageSentCost":
                            AdMessageSentCost += Convert.ToDecimal(propValue);
                            break;

                        case "AdMessageSentAmount":
                            AdMessageSentAmount += Convert.ToDecimal(propValue);
                            AdsSentAmount += Convert.ToDecimal(propValue);
                            break;

                        case "AdEnterOtpScreenSentCount":
                            AdEnterOtpScreenSentCount += Convert.ToInt32(propValue);
                            AdsSentCount += Convert.ToInt32(propValue);
                            break;

                        case "AdEnterOtpScreenSentCost":
                            AdEnterOtpScreenSentCost += Convert.ToDecimal(propValue);
                            break;

                        case "AdEnterOtpScreenSentAmount":
                            AdEnterOtpScreenSentAmount += Convert.ToDecimal(propValue);
                            AdsSentAmount += Convert.ToDecimal(propValue);
                            break;

                        case "AdVerificationScreenSentCount":
                            AdVerificationScreenSentCount += Convert.ToInt32(propValue);
                            AdsSentCount += Convert.ToInt32(propValue);
                            break;

                        case "AdVerificationScreenSentCost":
                            AdVerificationScreenSentCost += Convert.ToDecimal(propValue);
                            break;

                        case "AdVerificationScreenSentAmount":
                            AdVerificationScreenSentAmount += Convert.ToDecimal(propValue);
                            AdsSentAmount += Convert.ToDecimal(propValue);
                            break;

                        case "OtpSentEmailCount":
                            OtpSentEmailCount += Convert.ToInt32(propValue);
                            break;

                        case "OtpSentEmailCost":
                            OtpSentEmailCost += Convert.ToDecimal(propValue);
                            break;

                        case "OtpSentEmailAmount":
                            OtpSentEmailAmount += Convert.ToDecimal(propValue);
                            break;

                        case "OtpSentSmsCount":
                            OtpSentSmsCount += Convert.ToInt32(propValue);
                            break;

                        case "OtpSentSmsCost":
                            OtpSentSmsCost += Convert.ToDecimal(propValue);
                            break;

                        case "OtpSentSmsAmount":
                            OtpSentSmsAmount += Convert.ToDecimal(propValue);
                            break;

                        case "OtpSentVoiceCount":
                            OtpSentVoiceCount += Convert.ToInt32(propValue);
                            break;

                        case "OtpSentVoiceCost":
                            OtpSentVoiceCost += Convert.ToDecimal(propValue);
                            break;

                        case "OtpSentVoiceAmount":
                            OtpSentVoiceAmount += Convert.ToDecimal(propValue);
                            break;

                        case "EndUserRegistrationCount":
                            EndUserRegistrationCount += Convert.ToInt32(propValue);
                            break;

                        case "EndUserRegistrationCost":
                            EndUserRegistrationCost += Convert.ToDecimal(propValue);
                            break;

                        case "EndUserRegistrationAmount":
                            EndUserRegistrationAmount += Convert.ToDecimal(propValue);
                            break;

                        case "EndUserVerificationCount":
                            EndUserVerificationCount += Convert.ToInt32(propValue);
                            break;

                        case "EndUserVerificationCost":
                            EndUserVerificationCost += Convert.ToDecimal(propValue);
                            break;

                        case "EndUserVerificationAmount":
                            EndUserVerificationAmount += Convert.ToDecimal(propValue);
                            break;

                        case "SubTotal":
                            sbBillSummary.Append("<" + propName + ">" + myBillUtils.FormatMoney(Convert.ToDecimal(propValue)) + "</" + propName + ">");
                            break;

                        case "TaxRate":
                            sbBillSummary.Append("<" + propName + ">" + Convert.ToDecimal(propValue) + "</" + propName + ">");
                            break;

                        case "SalesTax":
                            sbBillSummary.Append("<" + propName + ">" + myBillUtils.FormatMoney(Convert.ToDecimal(propValue)) + "</" + propName + ">");
                            break;

                        case "Total":
                            sbBillSummary.Append("<" + propName + ">" + myBillUtils.FormatMoney(Convert.ToDecimal(propValue)) + "</" + propName + ">");
                            break;

                        default:
                            //sbResponse.Append("<" + propName + ">" + propValue + "</" + propName + ">");
                            break;
                    }
                }

            #endregion

            // Reset for next pass
            IsClientBilledToGroup = false;

            sbBillRender.Append(sbBillSummary.ToString());

            sbBillDetails.Append("<Details>");

            #region Otp Info

                sbBillDetails.Append("<OTPSent>");

                if (OtpSentSmsCount != 0)
                    price = (Convert.ToDecimal(OtpSentSmsAmount) / Convert.ToDecimal(OtpSentSmsCount));
                sbBillDetails.Append("     <Sms count='" + myBillUtils.FormatNumber(Convert.ToDecimal(OtpSentSmsCount)) + "' price='" + myBillUtils.FormatMoney(price) + "' amount='" + myBillUtils.FormatMoney(Convert.ToDecimal(OtpSentSmsAmount)) + "' />");

                price = 0.00M;
                if (OtpSentEmailCount != 0)
                    price = (Convert.ToDecimal(OtpSentEmailAmount) / Convert.ToDecimal(OtpSentEmailCount));
                sbBillDetails.Append("     <Email count='" + myBillUtils.FormatNumber(Convert.ToDecimal(OtpSentEmailCount)) + "' price='" + myBillUtils.FormatMoney(price) + "' amount='" + myBillUtils.FormatMoney(Convert.ToDecimal(OtpSentEmailAmount)) + "' />");

                price = 0.00M;
                if (OtpSentVoiceCount != 0)
                    price = (Convert.ToDecimal(OtpSentVoiceAmount) / Convert.ToDecimal(OtpSentVoiceCount));
                sbBillDetails.Append("     <Voice count='" + myBillUtils.FormatNumber(Convert.ToDecimal(OtpSentVoiceCount)) + "' price='" + myBillUtils.FormatMoney(price) + "' amount='" + myBillUtils.FormatMoney(Convert.ToDecimal(OtpSentVoiceAmount)) + "' />");

                sbBillDetails.Append("</OTPSent>");

            #endregion

            #region Ad Info

                sbBillDetails.Append("<Ads>");

                price = 0.00M;
                if (AdsSentCount != 0)
                    price = (Convert.ToDecimal(AdsSentAmount) / Convert.ToDecimal(AdsSentCount));
                sbBillDetails.Append("    <Sent count='" + myBillUtils.FormatNumber(AdsSentCount) + "' price='" + myBillUtils.FormatMoney(price) + "' amount='" + myBillUtils.FormatMoney(AdsSentAmount) + "' />");

                price = 0.00M;
                if (AdsClickedCount != 0)
                    price = (Convert.ToDecimal(AdsClickedAmount) / Convert.ToDecimal(AdsClickedCount));
                sbBillDetails.Append("    <Clicked count='" + myBillUtils.FormatNumber(AdsClickedCount) + "' price='" + myBillUtils.FormatMoney(price) + "' amount='" + myBillUtils.FormatMoney(AdsClickedAmount) + "' />");

                sbBillDetails.Append("</Ads>");

            #endregion

            #region End User Info

                sbBillDetails.Append("<EndUser>");

                price = 0.00M;
                if (EndUserRegistrationCount != 0)
                    price = (Convert.ToDecimal(EndUserRegistrationAmount) / Convert.ToDecimal(EndUserRegistrationCount));
                sbBillDetails.Append("    <Registrations count='" + myBillUtils.FormatNumber(EndUserRegistrationCount) + "' price='" + myBillUtils.FormatMoney(price) + "' amount='" + myBillUtils.FormatMoney(EndUserRegistrationAmount) + "' />");

                price = 0.00M;
                if (EndUserVerificationCount != 0)
                    price = (Convert.ToDecimal(EndUserVerificationAmount) / Convert.ToDecimal(EndUserVerificationCount));
                sbBillDetails.Append("    <Verifications count='" + myBillUtils.FormatNumber(EndUserVerificationCount) + "' price='" + myBillUtils.FormatMoney(price) + "' amount='" + myBillUtils.FormatMoney(EndUserVerificationAmount) + "' />");

                sbBillDetails.Append("</EndUser>");

            #endregion

            // Add addendums
            sbBillDetails.Append(sbBillAddendums.ToString());

            sbBillDetails.Append("</Details>");
        }

        sbBillDetails.Append("</" + ownerType + ">");

        sbBillRender.Append(sbBillDetails.ToString());

        HasBill = true;

        return sbBillRender.ToString();
    }
}
