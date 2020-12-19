using System;
using System.Collections.Generic;
using cts = MACBilling.BillConstants;
using MACServices;
using MongoDB.Bson;

namespace MACBilling
{
    public class BillConfig : BillUtils
    {
        public BillConfig(string ownerId, string ownerType, string adminId)
        {
            try
            {
                // Read in the object data from the db and return the populated config
// ReSharper disable once RedundantThisQualifier
                var myConfig = (BillConfig)this.Read(ownerId, BillConstants.Common.BillConfig);

                if (myConfig == null)
                {
                    // Create empty config
                    _id = ObjectId.GenerateNewId();

                    AllowedIpList = String.Empty;

                    ObjectType = "BillConfig";

                    Sid = _id.ToString();

                    OwnerId = ObjectId.Parse(ownerId);
                    OwnerType = ownerType;

                    Organization = new Organization();

                    // Lookup Client Organization if OwnerType = Client
                    if (ownerType == "Client")
                    {
                        Client myClient = new Client(ownerId);
                        Organization.Street1 = MACSecurity.Security.EncryptAndEncode(myClient.Organization.Street1, myClient.ClientId.ToString());
                        Organization.Street2 = MACSecurity.Security.EncryptAndEncode(myClient.Organization.Street2, myClient.ClientId.ToString());
                        Organization.City = MACSecurity.Security.EncryptAndEncode(myClient.Organization.City, myClient.ClientId.ToString());

                        Organization.State = myClient.Organization.State;

                        Organization.Zipcode = MACSecurity.Security.EncryptAndEncode(myClient.Organization.Zipcode, myClient.ClientId.ToString());
                        Organization.Email = MACSecurity.Security.EncryptAndEncode(myClient.Organization.Email, myClient.ClientId.ToString());
                        Organization.Phone = MACSecurity.Security.EncryptAndEncode(myClient.Organization.Phone, myClient.ClientId.ToString());

                        Organization.PrimaryAdminId = myClient.Organization.PrimaryAdminId;

                        Organization.Extension = MACSecurity.Security.EncryptAndEncode(myClient.Organization.Extension, myClient.ClientId.ToString());

                        Organization.AdminNotificationProvider = myClient.Organization.AdminNotificationProvider;
                        Organization.Country = MACSecurity.Security.EncryptAndEncode(myClient.Organization.Country, myClient.ClientId.ToString());

                        Organization.TaxId = MACSecurity.Security.EncryptAndEncode(myClient.Organization.TaxId, myClient.ClientId.ToString());
                    }
                    else
                    {
                        Organization.Street1 = MACSecurity.Security.EncryptAndEncode("Not specified", ownerId);
                        Organization.Street2 = MACSecurity.Security.EncryptAndEncode("Not specified", ownerId);
                        Organization.City = MACSecurity.Security.EncryptAndEncode("Not specified", ownerId);

                        Organization.State = ObjectId.Parse(Constants.Strings.DefaultEmptyObjectId);

                        Organization.Zipcode = MACSecurity.Security.EncryptAndEncode("Not specified", ownerId);
                        Organization.Email = MACSecurity.Security.EncryptAndEncode("Not specified", ownerId);
                        Organization.Phone = MACSecurity.Security.EncryptAndEncode("Not specified", ownerId);

                        Organization.PrimaryAdminId = ObjectId.Parse(Constants.Strings.DefaultEmptyObjectId);

                        Organization.Extension = MACSecurity.Security.EncryptAndEncode("Not specified", ownerId);
                        Organization.Country = MACSecurity.Security.EncryptAndEncode("Not specified", ownerId);
                        Organization.TaxId = MACSecurity.Security.EncryptAndEncode("Not specified", ownerId);
                    }

                    IncludeInGroupBill = false;
                    BillToGroupId = ObjectId.Parse(BillConstants.Common.DefaultEmptyObjectId);

                    DateCreated = DateTime.UtcNow;
                    CreatedById = ObjectId.Parse(adminId);

                    switch (OwnerType)
                    {
                        case BillConstants.Common.Group:
                            var myGroup = new Group(OwnerId.ToString());
                            OwnerName = myGroup.Name;
                            break;

                        default:
                            var myClient = new Client(OwnerId.ToString());
                            OwnerName = myClient.Name;
                            break;
                    }

                    BillingCycle = cts.Common.Monthly;
                    BillingTerms = cts.Common.Net30;
                    BillingSendTo = "";

                    ExternallySettledBy = "";

                    PaymentMethod = new BillPaymentACH();
                    PaymentGateway = new BillPaymentGateway();

                    ItemizedAdvertisingCharges = false;

                    BillingTiers = new List<BillTier>();

                    BillTier billTier;

                    billTier = new BillTier(OwnerId.ToString(), BillConstants.Tiers.AdMessageSent, BillConstants.Tiers.DefaultRates);
                    BillingTiers.Add(billTier);

                    billTier = new BillTier(OwnerId.ToString(), BillConstants.Tiers.AdEnterOtpScreenSent, BillConstants.Tiers.DefaultRates);
                    BillingTiers.Add(billTier);

                    billTier = new BillTier(OwnerId.ToString(), BillConstants.Tiers.AdVerificationScreenSent, BillConstants.Tiers.DefaultRates);
                    BillingTiers.Add(billTier);

                    billTier = new BillTier(OwnerId.ToString(), BillConstants.Tiers.EndUserRegister, BillConstants.Tiers.DefaultRates);
                    BillingTiers.Add(billTier);

                    billTier = new BillTier(OwnerId.ToString(), BillConstants.Tiers.EndUserVerify, BillConstants.Tiers.DefaultRates);
                    BillingTiers.Add(billTier);

                    billTier = new BillTier(OwnerId.ToString(), BillConstants.Tiers.OtpSentEmail, BillConstants.Tiers.DefaultRates);
                    BillingTiers.Add(billTier);

                    billTier = new BillTier(OwnerId.ToString(), BillConstants.Tiers.OtpSentSms, BillConstants.Tiers.DefaultRates);
                    BillingTiers.Add(billTier);

                    billTier = new BillTier(OwnerId.ToString(), BillConstants.Tiers.OtpSentVoice, BillConstants.Tiers.DefaultRates);
                    BillingTiers.Add(billTier);

                    MinimumOtpCharge = MACSecurity.Security.EncryptAndEncode("0.00", ownerId);
                    MinimumAdCharge = MACSecurity.Security.EncryptAndEncode("0.00", ownerId);
                    MinimumEndUserRegistrationCharge = MACSecurity.Security.EncryptAndEncode("0.00", ownerId);

                    MonthlyServiceCharge = MACSecurity.Security.EncryptAndEncode("0.00", ownerId);

                    NotifyUserIds = "";

                    TaxRate = MACSecurity.Security.EncryptAndEncode(BillConstants.Tiers.DefaultTaxRate, ownerId);

// ReSharper disable once RedundantThisQualifier
                    this.Create(this);
                }
                else
                {
                    _id = myConfig._id;

                    AllowedIpList = myConfig.AllowedIpList ?? "localhost" + MACServices.Constants.Common.ItemSep + "127.0.0.1";

                    ObjectType = "BillConfig";

                    Sid = myConfig.Sid;

                    OwnerId = myConfig.OwnerId;
                    OwnerName = myConfig.OwnerName;
                    OwnerType = myConfig.OwnerType;

                    Organization = myConfig.Organization;

                    IncludeInGroupBill = myConfig.IncludeInGroupBill;
                    BillToGroupId = myConfig.BillToGroupId;

                    ItemizedAdvertisingCharges = myConfig.ItemizedAdvertisingCharges;

                    DateCreated = myConfig.DateCreated;
                    CreatedById = myConfig.CreatedById;

                    DateUpdated = myConfig.DateUpdated;
                    UpdatedById = myConfig.UpdatedById;

                    BillingCycle = myConfig.BillingCycle;
                    BillingTerms = myConfig.BillingTerms;
                    BillingSendTo = myConfig.BillingSendTo;

                    ExternallySettledBy = myConfig.ExternallySettledBy;

                    PaymentMethod = myConfig.PaymentMethod;
                    PaymentGateway = myConfig.PaymentGateway;

                    BillingTiers = myConfig.BillingTiers;
                    // Have to do this. Otherwise, the values get re-encrypted again during the member copy operation!
                    BillingTiers = new List<BillTier>();
                    foreach (var currentTier in myConfig.BillingTiers)
                    {
                        var tierType = currentTier.TierType;

                        var tierTEMPValue = MACSecurity.Security.DecodeAndDecrypt(currentTier.TierValues, OwnerId.ToString());
                        var tierValue = MACSecurity.Security.DecodeAndDecrypt(tierTEMPValue, OwnerId.ToString());

                        BillTier myTier = new BillTier(OwnerId.ToString(), tierType, tierValue);
                        BillingTiers.Add(myTier);
                    }

                    MinimumOtpCharge = myConfig.MinimumOtpCharge;
                    MinimumAdCharge = myConfig.MinimumAdCharge;
                    MinimumEndUserRegistrationCharge = myConfig.MinimumEndUserRegistrationCharge;

                    MonthlyServiceCharge = myConfig.MonthlyServiceCharge;

                    if (myConfig.NotifyUserIds == null)
                        NotifyUserIds = "";
                    else
                        NotifyUserIds = myConfig.NotifyUserIds;

                    TaxRate = myConfig.TaxRate;
                }
            }
            catch(Exception ex)
            {
// ReSharper disable once UnusedVariable
                var exMsg = ex.ToString();
            }
        }

        public ObjectId _id { get; set; }

        public string Sid { get; set; }

        public string AllowedIpList { get; set; }

        public ObjectId OwnerId { get; set; }
        public string OwnerName { get; set; }
        public string OwnerType { get; set; }

        public string ObjectType { get; set; }

        public Organization Organization { get; set; }

        public string BillingCycle { get; set; }
        public string BillingTerms { get; set; }
        public string BillingSendTo { get; set; }

        public ObjectId BillToGroupId { get; set; }
        public bool IncludeInGroupBill { get; set; }

        public DateTime DateCreated { get; set; }
        public ObjectId CreatedById { get; set; }

        public DateTime DateUpdated { get; set; }
        public ObjectId UpdatedById { get; set; }

        public string ExternallySettledBy { get; set; }

        public string MinimumOtpCharge { get; set; }
        public string MinimumAdCharge { get; set; }
        public string MinimumEndUserRegistrationCharge { get; set; }

        public string MonthlyServiceCharge { get; set; }

        public string NotifyUserIds { get; set; }

        public bool ItemizedAdvertisingCharges { get; set; }

        public string TaxRate { get; set; }

        public List<BillTier> BillingTiers { get; set; }

        public BillPaymentGateway PaymentGateway { get; set; }
        public BillPaymentMethod PaymentMethod { get; set; }
    }
}
