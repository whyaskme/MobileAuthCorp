using System;
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
    public class BillClient : BillBase
    {
        public BillClient()
        {
        }

        public BillClient(string dateRange, string clientId, string adminId)
        {
            // Decrypt the adminid
            if (string.IsNullOrEmpty(adminId))
                adminId = Constants.Strings.DefaultAdminId;

            if(adminId.Length > 24)
                adminId = MACSecurity.Security.DecodeAndDecrypt(adminId, Constants.Strings.DefaultClientId);

            var myUtils = new Utils();

            Money tmpCost;
            var decryptedMinimumAmount = 0.00M;

            ClientId = ObjectId.Parse(clientId);
            CreatedById = ObjectId.Parse(adminId);
            DateCreated = DateTime.UtcNow;

// ReSharper disable once UnusedVariable
            var myClient = new Client(clientId);

            var myBillConfig = new BillConfig(clientId, cnt.Common.Client, adminId);

            OwnerId = myBillConfig.OwnerId.ToString();
            OwnerName = myBillConfig.OwnerName;
            OwnerType = myBillConfig.OwnerType;

            var originalOwnerId = myBillConfig.OwnerId;
            var originalOwnerName = myBillConfig.OwnerName;
            var originalOwnerType = myBillConfig.OwnerType;

            Decimal AddendumSubtotal = 0.00M;

// ReSharper disable once UnusedVariable
            var currentDate = DateTime.UtcNow.Date.ToShortDateString();

            var isGroupBilled = false;

            string currentTierTypeName;

            #region Find Addendums

                // Query addendums for current client for processing further down
                var addendumQuery = Query.And(Query.EQ("_t", "BillAddendum"), Query.EQ("OwnerId", ObjectId.Parse(clientId)), Query.EQ("HasBeenAttached", false));
                var addendumResult = myUtils.mongoDBConnectionPool.GetCollection("Billing").FindAs<BillAddendum>(addendumQuery);

            #endregion

            #region Override Client config with Group config if Group member

            // If a member of a group, grab the group's config and override the client with group settings
                if(myBillConfig.IncludeInGroupBill)
                {
                    myBillConfig = new BillConfig(myBillConfig.BillToGroupId.ToString(), cnt.Common.Group, adminId);

                    // This flag indicates no minimum charge processing for client group member
                    isGroupBilled = true;
                }

            #endregion

            #region Set Date Range

                Int16 dateDelta = 0;
                switch (myBillConfig.BillingTerms.Replace(" ", ""))
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

            #region Check Periodic Service Charges

                // Check to see if we have a Monthly Service Charge. Only process this if Client direct billed.
                if (!isGroupBilled)
                {
                    var monthlyServiceCharge = "0.00";

                    if (myBillConfig.MonthlyServiceCharge != "0.00")
                        monthlyServiceCharge = MACSecurity.Security.DecodeAndDecrypt(myBillConfig.MonthlyServiceCharge, myBillConfig.OwnerId.ToString());

                    if (monthlyServiceCharge != "0.00")
                    {
                        BillAddendum monthlyAddendum = new BillAddendum(myBillConfig.OwnerId.ToString());

                        monthlyAddendum.Amount = monthlyServiceCharge;
                        monthlyAddendum.Notes = myBillConfig.BillingCycle + " service charge.";

                        Addendums.Add(monthlyAddendum);
                        AddendumSubtotal += Convert.ToDecimal(monthlyServiceCharge);
                    }
                }

            #endregion

            #region Ads Sent

                // AdMessageSent
                currentTierTypeName = BillConstants.Tiers.AdMessageSent;
                var AdMessageSentRate = FindTierByName(myBillConfig, currentTierTypeName).Split(char.Parse(cnt.Common.ItemSep));
                AdMessageSentCount = GetEventCount(dateRange, clientId, currentTierTypeName);
                foreach (var currentCost in AdMessageSentRate)
                {
                    var currentRateElement = currentCost.Split(char.Parse(cnt.Common.KVSep));
                    var rateCount = Convert.ToInt32(currentRateElement[0]);
                    Decimal rateValue = Convert.ToDecimal(currentRateElement[1]);

                    // Init the cost if it's 0
                    if (AdMessageSentCost == 0)
                        AdMessageSentCost = rateValue;

                    // If the actual event count is greater than the current rate count, set the price according to rate
                    if (AdMessageSentCount > rateCount)
                        AdMessageSentCost = rateValue;
                }
                // Use Money class to prevent rounding errors caused by double, decimal and float data types
                tmpCost = new Money(AdMessageSentCost, CurrencyCodes.USD);
                AdMessageSentAmount = Convert.ToDecimal((AdMessageSentCount * tmpCost.InternalAmount).ToString());


                // AdEnterOtpScreenSent
                currentTierTypeName = BillConstants.Tiers.AdEnterOtpScreenSent;
                var AdEnterOtpScreenSentRate = FindTierByName(myBillConfig, currentTierTypeName).Split(char.Parse(cnt.Common.ItemSep));
                AdEnterOtpScreenSentCount = GetEventCount(dateRange, clientId, currentTierTypeName);
                foreach (var currentCost in AdEnterOtpScreenSentRate)
                {
                    var currentRateElement = currentCost.Split(char.Parse(cnt.Common.KVSep));
                    var rateCount = Convert.ToInt32(currentRateElement[0]);
                    Decimal rateValue = Convert.ToDecimal(currentRateElement[1]);

                    // Init the cost if it's 0
                    if (AdEnterOtpScreenSentCost == 0)
                        AdEnterOtpScreenSentCost = rateValue;

                    // If the actual event count is greater than the current rate count, set the price according to rate
                    if (AdMessageSentCount > rateCount)
                        AdEnterOtpScreenSentCost = rateValue;
                }
                // Use Money class to prevent rounding errors caused by double, decimal and float data types
                tmpCost = new Money(AdEnterOtpScreenSentCost, CurrencyCodes.USD);
                AdEnterOtpScreenSentAmount = Convert.ToDecimal((AdEnterOtpScreenSentCount * tmpCost.InternalAmount).ToString());



                // AdVerificationScreenSent
                currentTierTypeName = BillConstants.Tiers.AdVerificationScreenSent;
                var AdVerificationScreenSentRate = FindTierByName(myBillConfig, currentTierTypeName).Split(char.Parse(cnt.Common.ItemSep));
                AdVerificationScreenSentCount = GetEventCount(dateRange, clientId, currentTierTypeName);
                foreach (var currentCost in AdVerificationScreenSentRate)
                {
                    var currentRateElement = currentCost.Split(char.Parse(cnt.Common.KVSep));
                    var rateCount = Convert.ToInt32(currentRateElement[0]);
                    Decimal rateValue = Convert.ToDecimal(currentRateElement[1]);

                    // Init the cost if it's 0
                    if (AdVerificationScreenSentCost == 0)
                        AdVerificationScreenSentCost = rateValue;

                    // If the actual event count is greater than the current rate count, set the price according to rate
                    if (AdMessageSentCount > rateCount)
                        AdVerificationScreenSentCost = rateValue;
                }
                // Use Money class to prevent rounding errors caused by double, decimal and float data types
                tmpCost = new Money(AdVerificationScreenSentCost, CurrencyCodes.USD);
                AdVerificationScreenSentAmount = Convert.ToDecimal((AdVerificationScreenSentCount * tmpCost.InternalAmount).ToString());

            #endregion

            #region Minimum Advertising Validation

                if (!isGroupBilled) // Only apply minimums to client if direct billed
                {
                    // Check to see if Ads subtotal is less than minimum charge
                    if (myBillConfig.MinimumAdCharge != "0.00")
                        decryptedMinimumAmount = Convert.ToDecimal(MACSecurity.Security.DecodeAndDecrypt(myBillConfig.MinimumAdCharge, myBillConfig.OwnerId.ToString()));

                    var totalAdCharges = AdMessageSentAmount + AdEnterOtpScreenSentAmount + AdVerificationScreenSentAmount;
                    if (totalAdCharges < decryptedMinimumAmount)
                    {
                        BillAddendum myAdvertisingAddendum = new BillAddendum(myBillConfig.OwnerId.ToString());

                        myAdvertisingAddendum._id = ObjectId.GenerateNewId();

                        myAdvertisingAddendum.IsMinimumPriceAdjustment = true;

                        myAdvertisingAddendum.OwnerId = originalOwnerId;
                        myAdvertisingAddendum.OwnerName = originalOwnerName;
                        myAdvertisingAddendum.OwnerType = originalOwnerType;

                        myAdvertisingAddendum.CreatedById = CreatedById;
                        myAdvertisingAddendum.DateCreated = DateTime.UtcNow;

                        myAdvertisingAddendum.Amount = (decryptedMinimumAmount - totalAdCharges).ToString();
                        myAdvertisingAddendum.Notes = FormatMoney(Convert.ToDecimal(decryptedMinimumAmount.ToString())) + " " + myBillConfig.BillingCycle.ToLower() + " Advertising minimum - adj.";
                        myAdvertisingAddendum.HasBeenAttached = false;

                        Addendums.Add(myAdvertisingAddendum);

                        AddendumSubtotal += Convert.ToDecimal((decryptedMinimumAmount - totalAdCharges).ToString());
                    }
                }

            #endregion

            #region Otp Sent Email

                currentTierTypeName = BillConstants.Tiers.OtpSentEmail;
                var otpRateSentEmail = FindTierByName(myBillConfig, currentTierTypeName).Split(char.Parse(cnt.Common.ItemSep));
                OtpSentEmailCount = GetEventCount(dateRange, clientId, currentTierTypeName);
                foreach (var currentCost in otpRateSentEmail)
                {
                    var currentRateElement = currentCost.Split(char.Parse(cnt.Common.KVSep));
                    var rateCount = Convert.ToInt32(currentRateElement[0]);
                    Decimal rateValue = Convert.ToDecimal(currentRateElement[1]);

                    // Init the cost if it's 0
                    if (OtpSentEmailCost == 0)
                        OtpSentEmailCost = rateValue;

                    // If the actual event count is greater than the current rate count, set the price according to rate
                    if (OtpSentEmailCount > rateCount)
                        OtpSentEmailCost = rateValue;
                }
                // Use Money class to prevent rounding errors caused by double, decimal and float data types
                tmpCost = new Money(OtpSentEmailCost, CurrencyCodes.USD);
                OtpSentEmailAmount = Convert.ToDecimal((OtpSentEmailCount * tmpCost.InternalAmount).ToString());

            #endregion

            #region Otp Sent Sms

                currentTierTypeName = BillConstants.Tiers.OtpSentSms;
                var otpRateSentSms = FindTierByName(myBillConfig, currentTierTypeName).Split(char.Parse(cnt.Common.ItemSep));
                OtpSentSmsCount = GetEventCount(dateRange, clientId, currentTierTypeName);
                foreach (var currentCost in otpRateSentSms)
                {
                    var currentRateElement = currentCost.Split(char.Parse(cnt.Common.KVSep));
                    var rateCount = Convert.ToInt32(currentRateElement[0]);
                    Decimal rateValue = Convert.ToDecimal(currentRateElement[1]);

                    // Init the cost if it's 0
                    if (OtpSentSmsCost == 0)
                        OtpSentSmsCost = rateValue;

                    // If the actual event count is greater than the current rate count, set the price according to rate
                    if (OtpSentSmsCount > rateCount)
                        OtpSentSmsCost = rateValue;
                }
                // Use Money class to prevent rounding errors caused by double, decimal and float data types
                tmpCost = new Money(OtpSentSmsCost, CurrencyCodes.USD);
                OtpSentSmsAmount = Convert.ToDecimal((OtpSentSmsCount * tmpCost.InternalAmount).ToString());

            #endregion

            #region Otp sent Voice

                currentTierTypeName = BillConstants.Tiers.OtpSentVoice;
                var otpRateSentVoice = FindTierByName(myBillConfig, currentTierTypeName).Split(char.Parse(cnt.Common.ItemSep));
                OtpSentVoiceCount = GetEventCount(dateRange, clientId, currentTierTypeName);
                foreach (var currentCost in otpRateSentVoice)
                {
                    var currentRateElement = currentCost.Split(char.Parse(cnt.Common.KVSep));
                    var rateCount = Convert.ToInt32(currentRateElement[0]);
                    Decimal rateValue = Convert.ToDecimal(currentRateElement[1]);

                    // Init the cost if it's 0
                    if (OtpSentVoiceCost == 0)
                        OtpSentVoiceCost = rateValue;

                    // If the actual event count is greater than the current rate count, set the price according to rate
                    if (OtpSentVoiceCount > rateCount)
                        OtpSentVoiceCost = rateValue;
                }
                // Use Money class to prevent rounding errors caused by double, decimal and float data types
                tmpCost = new Money(OtpSentVoiceCost, CurrencyCodes.USD);
                OtpSentVoiceAmount = Convert.ToDecimal((OtpSentVoiceCount * tmpCost.InternalAmount).ToString());

            #endregion

            #region Minimum OTP Validation

                if (!isGroupBilled) // Only apply minimums to client if direct billed
                {
                    // Check to see if Otp subtotal is less than minimum charge
                    if (myBillConfig.MinimumOtpCharge != "0.00")
                        decryptedMinimumAmount = Convert.ToDecimal(MACSecurity.Security.DecodeAndDecrypt(myBillConfig.MinimumOtpCharge, myBillConfig.OwnerId.ToString()));

                    var totalOtpCharges = OtpSentEmailAmount + OtpSentSmsAmount + OtpSentVoiceAmount;
                    if (totalOtpCharges < decryptedMinimumAmount)
                    {
                        BillAddendum myOtpAddendum = new BillAddendum(myBillConfig.OwnerId.ToString());

                        myOtpAddendum._id = ObjectId.GenerateNewId();

                        myOtpAddendum.IsMinimumPriceAdjustment = true;

                        myOtpAddendum.OwnerId = originalOwnerId;
                        myOtpAddendum.OwnerName = originalOwnerName;
                        myOtpAddendum.OwnerType = originalOwnerType;

                        myOtpAddendum.CreatedById = CreatedById;
                        myOtpAddendum.DateCreated = DateTime.UtcNow;

                        myOtpAddendum.Amount = (decryptedMinimumAmount - totalOtpCharges).ToString();
                        myOtpAddendum.Notes = FormatMoney(Convert.ToDecimal(decryptedMinimumAmount.ToString())) + " " + myBillConfig.BillingCycle.ToLower() + " OTP minimum - adj.";
                        myOtpAddendum.HasBeenAttached = false;
                            
                        Addendums.Add(myOtpAddendum);

                        AddendumSubtotal += Convert.ToDecimal((decryptedMinimumAmount - totalOtpCharges).ToString());
                    }
                }

            #endregion

            #region End User Registration

                currentTierTypeName = BillConstants.Tiers.EndUserRegister;
                var endUserRateRegistration = FindTierByName(myBillConfig, currentTierTypeName).Split(char.Parse(cnt.Common.ItemSep));
                EndUserRegistrationCount = GetEventCount(dateRange, clientId, currentTierTypeName);
                foreach (var currentCost in endUserRateRegistration)
                {
                    var currentRateElement = currentCost.Split(char.Parse(cnt.Common.KVSep));
                    var rateCount = Convert.ToInt32(currentRateElement[0]);
                    Decimal rateValue = Convert.ToDecimal(currentRateElement[1]);

                    // Init the cost if it's 0
                    if (EndUserRegistrationCost == 0)
                        EndUserRegistrationCost = rateValue;

                    // If the actual event count is greater than the current rate count, set the price according to rate
                    if (EndUserRegistrationCount > rateCount)
                        EndUserRegistrationCost = rateValue;
                }
                // Use Money class to prevent rounding errors caused by double, decimal and float data types
                tmpCost = new Money(EndUserRegistrationCost, CurrencyCodes.USD);
                EndUserRegistrationAmount = Convert.ToDecimal((EndUserRegistrationCount * tmpCost.InternalAmount).ToString());

            #endregion

            #region End User Verification

                currentTierTypeName = BillConstants.Tiers.EndUserVerify;
                var endUserRateVerification = FindTierByName(myBillConfig, currentTierTypeName).Split(char.Parse(cnt.Common.ItemSep));
                EndUserVerificationCount = GetEventCount(dateRange, clientId, currentTierTypeName);
                foreach (var currentCost in endUserRateVerification)
                {
                    var currentRateElement = currentCost.Split(char.Parse(cnt.Common.KVSep));
                    var rateCount = Convert.ToInt32(currentRateElement[0]);
                    Decimal rateValue = Convert.ToDecimal(currentRateElement[1]);

                    // Init the cost if it's 0
                    if (EndUserVerificationCost == 0)
                        EndUserVerificationCost = rateValue;

                    // If the actual event count is greater than the current rate count, set the price according to rate
                    if (EndUserVerificationCount > rateCount)
                        EndUserVerificationCost = rateValue;
                }
                // Use Money class to prevent rounding errors caused by double, decimal and float data types
                tmpCost = new Money(EndUserVerificationCost, CurrencyCodes.USD);
                EndUserVerificationAmount = Convert.ToDecimal((EndUserVerificationCount * tmpCost.InternalAmount).ToString());

            #endregion

            #region Minimum End User Validation

                if (!isGroupBilled) // Only apply minimums to client if direct billed
                {
                    // Check to see if End User Registration subtotal is less than minimum charge
                    if (myBillConfig.MinimumEndUserRegistrationCharge != "0.00")
                        decryptedMinimumAmount = Convert.ToDecimal(MACSecurity.Security.DecodeAndDecrypt(myBillConfig.MinimumEndUserRegistrationCharge, myBillConfig.OwnerId.ToString()));

                    var totalEndUserCharges = EndUserRegistrationAmount + EndUserVerificationAmount;
                    if (totalEndUserCharges < decryptedMinimumAmount)
                    {
                        BillAddendum myEndUserAddendum = new BillAddendum(myBillConfig.OwnerId.ToString());

                        myEndUserAddendum._id = ObjectId.GenerateNewId();

                        myEndUserAddendum.IsMinimumPriceAdjustment = true;

                        myEndUserAddendum.OwnerId = originalOwnerId;
                        myEndUserAddendum.OwnerName = originalOwnerName;
                        myEndUserAddendum.OwnerType = originalOwnerType;

                        myEndUserAddendum.CreatedById = CreatedById;
                        myEndUserAddendum.DateCreated = DateTime.UtcNow;

                        myEndUserAddendum.Amount = (decryptedMinimumAmount - totalEndUserCharges).ToString();
                        myEndUserAddendum.Notes = FormatMoney(Convert.ToDecimal(decryptedMinimumAmount.ToString())) + " " + myBillConfig.BillingCycle.ToLower() + " End User minimum - adj.";
                        myEndUserAddendum.HasBeenAttached = false;
                            
                        Addendums.Add(myEndUserAddendum);

                        AddendumSubtotal += Convert.ToDecimal((decryptedMinimumAmount - totalEndUserCharges).ToString());
                    }
                }

            #endregion

            #region Addendums

                foreach (BillAddendum currentAddendum in addendumResult)
                {
                    // Have to do this. Otherwise, the values get re-encrypted again during the member copy operation!
                    //var tmpAddAmount = MACSecurity.Security.DecodeAndDecrypt(currentAddendum.Amount, clientId);
                    //tmpAddAmount = MACSecurity.Security.DecodeAndDecrypt(tmpAddAmount, clientId);

                    var tmpAddAmount = currentAddendum.Amount;

                    //var tmpAddNotes = MACSecurity.Security.DecodeAndDecrypt(currentAddendum.Notes, clientId);
                    //tmpAddNotes = MACSecurity.Security.DecodeAndDecrypt(tmpAddNotes, clientId);

                    var tmpAddNotes = currentAddendum.Notes;

                    currentAddendum.Amount = tmpAddAmount;
                    currentAddendum.Notes = tmpAddNotes;

                    if (isGroupBilled)
                    {
                        if (tmpAddNotes.Contains("Advertising minimum") || tmpAddNotes.Contains("OTP minimum") || tmpAddNotes.Contains("End User minimum"))
                        {
                            // Don't add automated minimum charge adjustments
                        }
                        else
                        {
                            Addendums.Add(currentAddendum); // This is a manual addendum
                            AddendumSubtotal += Convert.ToDecimal(tmpAddAmount.Replace("$", ""));
                        }
                    }
                    else
                    {
                        Addendums.Add(currentAddendum);
                        AddendumSubtotal += Convert.ToDecimal(tmpAddAmount.Replace("$", ""));
                    }
                }

                // Use Money class to prevent rounding errors caused by double, decimal and float data types
                tmpCost = new Money(AddendumSubtotal, CurrencyCodes.USD);
                AddendumsAmount = Convert.ToDecimal(tmpCost.InternalAmount.ToString());

            #endregion

            #region Subtotal

                // Use Money class to prevent rounding errors caused by double, decimal and float data types
                var totalAdAmount = AdMessageSentAmount + AdEnterOtpScreenSentAmount + AdVerificationScreenSentAmount;
                var totalOtpAmount = OtpSentEmailAmount + OtpSentSmsAmount + OtpSentVoiceAmount;
                var totalEndUserAmount = EndUserRegistrationAmount + EndUserVerificationAmount;

                tmpCost = new Money((totalAdAmount + totalOtpAmount + totalEndUserAmount + AddendumsAmount), CurrencyCodes.USD);
                SubTotal = Convert.ToDecimal(tmpCost.InternalAmount.ToString());

            #endregion

            #region Sales Tax

                // Use Money class to prevent rounding errors caused by double, decimal and float data types
                if (myBillConfig.TaxRate != "0.00")
                    TaxRate = Convert.ToDecimal(MACSecurity.Security.DecodeAndDecrypt(myBillConfig.TaxRate, myBillConfig.OwnerId.ToString()));
                else
                    TaxRate = Convert.ToDecimal(myBillConfig.TaxRate);

                tmpCost = new Money((SubTotal * TaxRate), CurrencyCodes.USD);
                SalesTax = Convert.ToDecimal(tmpCost.InternalAmount.ToString());

            #endregion

            #region Total

                // Use Money class to prevent rounding errors caused by double, decimal and float data types
                tmpCost = new Money((SubTotal + SalesTax), CurrencyCodes.USD);
                Total = Convert.ToDecimal(tmpCost.InternalAmount.ToString());

            #endregion
        }

        private string FindTierByName(BillConfig myBillConfig, string tierName)
        {
            var decodedTierValue = "";

            foreach(BillTier currentTier in myBillConfig.BillingTiers)
            {
                if (currentTier.TierType == tierName)
                    return MACSecurity.Security.DecodeAndDecrypt(currentTier.TierValues, currentTier.OwnerId.ToString());
            }

            return decodedTierValue;
        }

        public ObjectId ClientId { get; set; }

        public Int32 GetEventCount(string dateRange, string clientId, string statName)
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

                    var query = Query.And(Query.EQ("_t", "BillArchive"), Query.EQ("OwnerId", ObjectId.Parse(clientId)));
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

            var myStats = new EventStat(ObjectId.Parse(clientId));

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
