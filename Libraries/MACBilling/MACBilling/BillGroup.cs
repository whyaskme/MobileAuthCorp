using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

using MACServices;

using cnt = MACBilling.BillConstants;
using cs = MACServices.Constants.Strings;

using Useful.Money;

namespace MACBilling
{
    public class BillGroup : BillBase
    {
        string billAction = "Direct billed";
        string billActionDetails = "to client";

        public Decimal AddendumSubtotal = 0.00M;

        BillUtils myBillUtils = new BillUtils();

        public BillGroup()
        {
        }

        public BillGroup(string dateRange, string groupId, string adminId)
        {
            // Decrypt the adminid
            adminId = MACSecurity.Security.DecodeAndDecrypt(adminId, Constants.Strings.DefaultClientId);

            //GroupId = ObjectId.Parse(groupId);
            GroupId = groupId;

            Group myGroup = new Group(groupId);

            OwnerId = GroupId;
            OwnerName = myGroup.Name;
            OwnerType = "Group";

            var myUtils = new Utils();

            var myGroupBillConfig = new BillConfig(groupId, cnt.Common.Group, adminId);

            if (myGroupBillConfig.TaxRate != "0.00")
                TaxRate = Convert.ToDecimal(MACSecurity.Security.DecodeAndDecrypt(myGroupBillConfig.TaxRate, groupId));
            else
                TaxRate = Convert.ToDecimal(myGroupBillConfig.TaxRate);

            Money tmpCost;
            var decryptedMinimumAmount = 0.00M;

            CreatedById = ObjectId.Parse(adminId);
            DateCreated = DateTime.UtcNow;

            ClientSummary = new List<string>();

            #region Set Date Range

                Int16 dateDelta = 0;
                switch (myGroupBillConfig.BillingTerms.Replace(" ", ""))
                {
                    case "DueUponReceipt":
                        dateDelta = 0;
                        break;

                    case "Net10":
                        dateDelta = 10;
                        break;

                    case "Net15":
                        dateDelta = 15;
                        break;

                    case "Net30":
                        dateDelta = 30;
                        break;

                    case "2%15,net30":
                        dateDelta = 30;
                        break;

                    case "4%7,net30":
                        dateDelta = 30;
                        break;
                }

                DateTime dueDate = DateCreated.AddDays(dateDelta);

                DateDue = dueDate;

            #endregion

            ClientsInGroup = 0;
            ClientsGroupBilled = 0;
            ClientsDirectBilled = 0;

            // Loop through all assigned Clients
            ClientBills = new List<BillClient>();

            #region Process Client Members

                foreach (var currentClientRelationship in myGroup.Relationships)
                {
                    if (currentClientRelationship.MemberType == "Client")
                    {
                        ClientsInGroup++;

                        Client myClient = new Client(currentClientRelationship.MemberId.ToString());

                        BillClient myClientBill = new BillClient(dateRange, currentClientRelationship.MemberId.ToString(), CreatedById.ToString());

                        BillConfig myClientBillConfig = new BillConfig(currentClientRelationship.MemberId.ToString(), "Client", CreatedById.ToString());

                        // Set ownership info
                        OwnerId = myClient._id.ToString();
                        OwnerName = myClient.Name;
                        OwnerType = "Client";

                        if (myClientBillConfig.IncludeInGroupBill)
                        {
                            if (myClientBillConfig.BillToGroupId == ObjectId.Parse(groupId))
                            {
                                billAction = "Included";
                                billActionDetails = "in this group bill";

                                ClientsGroupBilled++;

                                AdMessageSentCount += myClientBill.AdMessageSentCount;
                                AdMessageSentCost += myClientBill.AdMessageSentCost;
                                AdMessageSentAmount += myClientBill.AdMessageSentAmount;

                                AdEnterOtpScreenSentCount += myClientBill.AdEnterOtpScreenSentCount;
                                AdEnterOtpScreenSentCost += myClientBill.AdEnterOtpScreenSentCost;
                                AdEnterOtpScreenSentAmount += myClientBill.AdEnterOtpScreenSentAmount;

                                AdVerificationScreenSentCount += myClientBill.AdVerificationScreenSentCount;
                                AdVerificationScreenSentCost += myClientBill.AdVerificationScreenSentCost;
                                AdVerificationScreenSentAmount += myClientBill.AdVerificationScreenSentAmount;

                                OtpSentEmailCount += myClientBill.OtpSentEmailCount;
                                OtpSentEmailCost += myClientBill.OtpSentEmailCost;
                                OtpSentEmailAmount += myClientBill.OtpSentEmailAmount;

                                OtpSentSmsCount += myClientBill.OtpSentSmsCount;
                                OtpSentSmsCost += myClientBill.OtpSentSmsCost;
                                OtpSentSmsAmount += myClientBill.OtpSentSmsAmount;

                                OtpSentVoiceCount += myClientBill.OtpSentVoiceCount;
                                OtpSentVoiceCost += myClientBill.OtpSentVoiceCost;
                                OtpSentVoiceAmount += myClientBill.OtpSentVoiceAmount;

                                EndUserRegistrationCount += myClientBill.EndUserRegistrationCount;
                                EndUserRegistrationCost += myClientBill.EndUserRegistrationCost;
                                EndUserRegistrationAmount += myClientBill.EndUserRegistrationAmount;

                                EndUserVerificationCount += myClientBill.EndUserVerificationCount;
                                EndUserVerificationCost += myClientBill.EndUserVerificationCost;
                                EndUserVerificationAmount += myClientBill.EndUserVerificationAmount;

                                // Check to see if this client is group billed. If so, see if the client has any addendums and attach them to the group's addendums.
                                if (myClientBill.Addendums.Count > 0)
                                {
                                    foreach (BillAddendum myClientAddendum in myClientBill.Addendums)
                                    {
                                        // Add the included client's addendum to the group's addendums
                                        Addendums.Add(myClientAddendum);

                                        // Update addendums total and bill subtotals
                                        //var tmpAddAmount = MACSecurity.Security.DecodeAndDecrypt(myClientAddendum.Amount, myClientAddendum.OwnerId.ToString());
                                        var tmpAddAmount = myClientAddendum.Amount;

                                        AddendumsAmount += Convert.ToDecimal(tmpAddAmount.Replace("$", ""));
                                    }
                                }

                                AddendumSubtotal += myClientBill.AddendumsAmount;

                                SubTotal += myClientBill.SubTotal;

                                Total += myClientBill.Total;
                            }
                            else
                            {
                                billAction = "Charged";
                                billActionDetails = "to the (" + myGroupBillConfig.OwnerName + ") group bill";
                            }
                        }
                        else
                        {
                            ClientsDirectBilled++;
                        }

                        ClientBills.Add(myClientBill);

                        ClientSummary.Add(myClient.Name + " - " + billAction + " (" + myBillUtils.FormatMoney(myClientBill.Total) + ") " + billActionDetails + ".");
                    }
                }

            #endregion

            #region Find Addendums

                // Query addendums for current client and attach if any found not attached already
                var addendumQuery = Query.And(Query.EQ("_t", "BillAddendum"), Query.EQ("OwnerId", ObjectId.Parse(groupId)), Query.EQ("HasBeenAttached", false));

                var addendumResult = myUtils.mongoDBConnectionPool.GetCollection("Billing").FindAs<BillAddendum>(addendumQuery);
                foreach (BillAddendum currentAddendum in addendumResult)
                {
                    // Have to do this. Otherwise, the values get re-encrypted again during the member copy operation!
                    //var tmpAddAmount = MACSecurity.Security.DecodeAndDecrypt(currentAddendum.Amount, groupId);
                    //tmpAddAmount = MACSecurity.Security.DecodeAndDecrypt(tmpAddAmount, groupId);

                    var tmpAddAmount = currentAddendum.Amount;

                    //var tmpAddNotes = MACSecurity.Security.DecodeAndDecrypt(currentAddendum.Notes, groupId);
                    //tmpAddNotes = MACSecurity.Security.DecodeAndDecrypt(tmpAddNotes, groupId);

                    var tmpAddNotes = currentAddendum.Notes;

                    currentAddendum.Amount = tmpAddAmount;
                    currentAddendum.Notes = tmpAddNotes;

                    Addendums.Add(currentAddendum);
                    AddendumSubtotal += Convert.ToDecimal(tmpAddAmount.Replace("$", ""));
                }

                // Check to see if we have a Monthly Service Charge.
                var monthlyServiceCharge = "0.00";

                if (myGroupBillConfig.MonthlyServiceCharge != "0.00")
                    monthlyServiceCharge = MACSecurity.Security.DecodeAndDecrypt(myGroupBillConfig.MonthlyServiceCharge, myGroupBillConfig.OwnerId.ToString());

                if (monthlyServiceCharge != "0.00")
                {
                    BillAddendum monthlyAddendum = new BillAddendum(myGroupBillConfig.OwnerId.ToString());

                    monthlyAddendum.OwnerId = myGroupBillConfig.OwnerId;
                    monthlyAddendum.OwnerName = myGroupBillConfig.OwnerName;
                    monthlyAddendum.OwnerType = "Group";

                    monthlyAddendum.Amount = monthlyServiceCharge;
                    monthlyAddendum.Notes = myGroupBillConfig.BillingCycle + " service charge.";

                    Addendums.Add(monthlyAddendum);
                    AddendumSubtotal += Convert.ToDecimal(monthlyServiceCharge);
                }

                // Use Money class to prevent rounding errors caused by double, decimal and float data types
                tmpCost = new Money(AddendumSubtotal, CurrencyCodes.USD);
                AddendumsAmount += Convert.ToDecimal(tmpCost.InternalAmount.ToString());

                SubTotal += AddendumsAmount;

                #endregion

            #region Check for Minimum Charges

                // Check to see if OTP subtotal is less than minimum charge
                if (myGroupBillConfig.MinimumOtpCharge != "0.00")
                    decryptedMinimumAmount = Convert.ToDecimal(MACSecurity.Security.DecodeAndDecrypt(myGroupBillConfig.MinimumOtpCharge, myGroupBillConfig.OwnerId.ToString()));

                var totalOtpCharges = OtpSentEmailAmount + OtpSentSmsAmount + OtpSentVoiceAmount;
                if (totalOtpCharges < decryptedMinimumAmount)
                {
                    BillAddendum myAdvertisingAddendum = new BillAddendum(myGroupBillConfig.OwnerId.ToString());

                    myAdvertisingAddendum._id = ObjectId.GenerateNewId();

                    myAdvertisingAddendum.IsMinimumPriceAdjustment = true;

                    myAdvertisingAddendum.OwnerId = myGroupBillConfig.OwnerId;
                    myAdvertisingAddendum.OwnerName = myGroupBillConfig.OwnerName;
                    myAdvertisingAddendum.OwnerType = myGroupBillConfig.OwnerType;

                    myAdvertisingAddendum.CreatedById = CreatedById;
                    myAdvertisingAddendum.DateCreated = DateTime.UtcNow;

                    myAdvertisingAddendum.Amount = (decryptedMinimumAmount - totalOtpCharges).ToString();
                    myAdvertisingAddendum.Notes = FormatMoney(Convert.ToDecimal(decryptedMinimumAmount.ToString())) + " " + myGroupBillConfig.BillingCycle.ToLower() + " OTP minimum - adj.";
                    myAdvertisingAddendum.HasBeenAttached = false;

                    Addendums.Add(myAdvertisingAddendum);

                    AddendumSubtotal += Convert.ToDecimal((decryptedMinimumAmount - totalOtpCharges).ToString());
                    AddendumsAmount += Convert.ToDecimal((decryptedMinimumAmount - totalOtpCharges).ToString());

                    SubTotal += Convert.ToDecimal((decryptedMinimumAmount - totalOtpCharges).ToString());
                }

                // Check to see if Ads subtotal is less than minimum charge
                if (myGroupBillConfig.MinimumAdCharge != "0.00")
                    decryptedMinimumAmount = Convert.ToDecimal(MACSecurity.Security.DecodeAndDecrypt(myGroupBillConfig.MinimumAdCharge, myGroupBillConfig.OwnerId.ToString()));

                var totalAdCharges = AdMessageSentAmount + AdEnterOtpScreenSentAmount + AdVerificationScreenSentAmount;
                if (totalAdCharges < decryptedMinimumAmount)
                {
                    BillAddendum myAdvertisingAddendum = new BillAddendum(myGroupBillConfig.OwnerId.ToString());

                    myAdvertisingAddendum._id = ObjectId.GenerateNewId();

                    myAdvertisingAddendum.IsMinimumPriceAdjustment = true;

                    myAdvertisingAddendum.OwnerId = myGroupBillConfig.OwnerId;
                    myAdvertisingAddendum.OwnerName = myGroupBillConfig.OwnerName;
                    myAdvertisingAddendum.OwnerType = myGroupBillConfig.OwnerType;

                    myAdvertisingAddendum.CreatedById = CreatedById;
                    myAdvertisingAddendum.DateCreated = DateTime.UtcNow;

                    myAdvertisingAddendum.Amount = (decryptedMinimumAmount - totalAdCharges).ToString();
                    myAdvertisingAddendum.Notes = FormatMoney(Convert.ToDecimal(decryptedMinimumAmount.ToString())) + " " + myGroupBillConfig.BillingCycle.ToLower() + " Advertising minimum - adj.";
                    myAdvertisingAddendum.HasBeenAttached = false;

                    Addendums.Add(myAdvertisingAddendum);

                    AddendumSubtotal += Convert.ToDecimal((decryptedMinimumAmount - totalAdCharges).ToString());
                    AddendumsAmount += Convert.ToDecimal((decryptedMinimumAmount - totalAdCharges).ToString());

                    SubTotal += Convert.ToDecimal((decryptedMinimumAmount - totalAdCharges).ToString());
                }

                // Check to see if End User subtotal is less than minimum charge
                if (myGroupBillConfig.MinimumEndUserRegistrationCharge != "0.00")
                    decryptedMinimumAmount = Convert.ToDecimal(MACSecurity.Security.DecodeAndDecrypt(myGroupBillConfig.MinimumEndUserRegistrationCharge, myGroupBillConfig.OwnerId.ToString()));

                var totalEndUserCharges = EndUserRegistrationAmount + EndUserVerificationAmount;
                if (totalEndUserCharges < decryptedMinimumAmount)
                {
                    BillAddendum myAdvertisingAddendum = new BillAddendum(myGroupBillConfig.OwnerId.ToString());

                    myAdvertisingAddendum._id = ObjectId.GenerateNewId();

                    myAdvertisingAddendum.IsMinimumPriceAdjustment = true;

                    myAdvertisingAddendum.OwnerId = myGroupBillConfig.OwnerId;
                    myAdvertisingAddendum.OwnerName = myGroupBillConfig.OwnerName;
                    myAdvertisingAddendum.OwnerType = myGroupBillConfig.OwnerType;

                    myAdvertisingAddendum.CreatedById = CreatedById;
                    myAdvertisingAddendum.DateCreated = DateTime.UtcNow;

                    myAdvertisingAddendum.Amount = (decryptedMinimumAmount - totalEndUserCharges).ToString();
                    myAdvertisingAddendum.Notes = FormatMoney(Convert.ToDecimal(decryptedMinimumAmount.ToString())) + " " + myGroupBillConfig.BillingCycle.ToLower() + " End User minimum - adj.";
                    myAdvertisingAddendum.HasBeenAttached = false;

                    Addendums.Add(myAdvertisingAddendum);

                    AddendumSubtotal += Convert.ToDecimal((decryptedMinimumAmount - totalEndUserCharges).ToString());
                    AddendumsAmount += Convert.ToDecimal((decryptedMinimumAmount - totalEndUserCharges).ToString());

                    SubTotal += Convert.ToDecimal((decryptedMinimumAmount - totalEndUserCharges).ToString());
                }

            #endregion

            var otpSubTotal = OtpSentEmailAmount + OtpSentSmsAmount + OtpSentVoiceAmount;
            var adSubTotal = AdEnterOtpScreenSentAmount + AdMessageSentAmount + AdVerificationScreenSentAmount;
            var enduserSubTotal = EndUserRegistrationAmount + EndUserVerificationAmount;
            var addendumSubTotal = AddendumSubtotal;

            var tmpSubTotal = otpSubTotal + adSubTotal + enduserSubTotal + addendumSubTotal;

            SubTotal = tmpSubTotal;

            var tmpTaxes = tmpSubTotal * TaxRate;

            SalesTax = tmpTaxes;
            Total = (tmpSubTotal + tmpTaxes);
        }

        //public ObjectId GroupId { get; set; }
        public string GroupId { get; set; }

        public Int16 ClientsInGroup { get; set; }
        public Int16 ClientsGroupBilled { get; set; }
        public Int16 ClientsDirectBilled { get; set; }

        public List<BillClient> ClientBills { get; set; }
        public List<string> ClientSummary { get; set; }

// ReSharper disable once UnusedMember.Local
        private string FindTierByName(BillConfig myGroupBillConfig, string tierName)
        {
            var decodedTierValue = "";

            foreach (BillTier currentTier in myGroupBillConfig.BillingTiers)
            {
                if (currentTier.TierType == tierName)
                    return MACSecurity.Security.DecodeAndDecrypt(currentTier.TierValues, currentTier.OwnerId.ToString());
            }

            return decodedTierValue;
        }

        public Int32 GetEventCount(string dateRange, string groupId, string statName)
        {
            Int32 eventCount = 0;

            DateTime _startDate = DateTime.UtcNow;
            DateTime _endDate = DateTime.UtcNow.AddDays(1);

            DateTime lastBillDate = DateTime.UtcNow.AddDays(-3650); // Subtract 10 years to initialize

            if (String.IsNullOrEmpty(dateRange))
                dateRange = "All Time";

            switch (dateRange)
            {
                case "All Time":
                    _startDate = DateTime.UtcNow.AddYears(-10);
                    break;
                case "Current (Pending) Charges":
                    // Need to get date of last generated bill
                    var mongoDBConnectionPool = (MongoDatabase)HttpContext.Current.Application[cs.MongoDB];
                    MongoCollection mongoCollection = mongoDBConnectionPool.GetCollection("Archive");

                    var query = Query.And(Query.EQ("_t", "BillArchive"), Query.EQ("OwnerId", ObjectId.Parse(groupId)));
                    var sortBy = SortBy.Descending("DateCreated");

                    var billList = mongoCollection.FindAs<BillArchive>(query).SetSortOrder(sortBy);

                    var iCount = 0;
                    foreach (var archiveBill in billList)
                    {
                        if (iCount == 0)
                            lastBillDate = archiveBill.DateCreated;

                        iCount++;
                    }

                    var lastBillDateTicks = lastBillDate.Ticks;
                    var currentTicks = DateTime.UtcNow.Ticks;
                    var ticksDelta = currentTicks - lastBillDateTicks;

                    _startDate = DateTime.UtcNow.AddTicks(-ticksDelta);
                    break;
                case "Today":
                    _startDate = DateTime.UtcNow.AddDays(-1);
                    break;
                case "Yesterday":
                    _startDate = DateTime.UtcNow.AddDays(-2);
                    _endDate = DateTime.UtcNow.AddDays(-1);
                    break;
                case "This Week":
                    _startDate = DateTime.UtcNow.AddDays(-7);
                    break;
                case "This Month":
                    _startDate = DateTime.UtcNow.AddDays(-30);
                    break;
                case "This Quarter":
                    _startDate = DateTime.UtcNow.AddDays(-90);
                    break;
                case "This Year":
                    _startDate = DateTime.UtcNow.AddDays(-365);
                    break;
            }

            var myStats = new EventStat(ObjectId.Parse(groupId));

            if (myStats.DailyStats != null)
            {
                if (myStats.DailyStats.Count > 0)
                {
                    foreach (EventStatDay currentDayStat in myStats.DailyStats)
                    {
                        foreach (PropertyInfo property in currentDayStat.GetType().GetProperties())
                        {
                            if (property.Name == statName)
                            {
                                if (currentDayStat.Date >= _startDate && currentDayStat.Date <= _endDate)
                                    eventCount += Convert.ToInt32(property.GetValue(currentDayStat, null));
                            }
                        }
                    }
                }
            }

            return eventCount;
        }

        public Decimal GetTaxRate(string organizationState)
        {
            return 0.075M;
        }

    }
}
