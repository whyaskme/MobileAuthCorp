using System;
using MongoDB.Bson;

namespace MACBilling
{
    public static class BillConstants
    {
        public static class TokenKeys
        {
            public const string ItemSep = "|";          // Pipe(|) used as item seperator
            public const string KVSep = ":";            // Colon(:) used as Key/value seperator
            public static string BillId = ItemSep + "BillId" + KVSep;
            public static string BillTotal = ItemSep + "BillTotal" + KVSep;

            public static string BillAddendumAmount = ItemSep + "BillAddendumAmount" + KVSep;
            public static string BillAddendumNotes = ItemSep + "BillAddendumNotes" + KVSep;

            public static string ClientName = ItemSep + "ClientName" + KVSep;
            public static string EventGeneratedByName = ItemSep + "EventGeneratedByName" + KVSep;
            public static string ExceptionDetails = "ExceptionDetails" + KVSep;
            public static string FailureDetails = ItemSep + "FailureDetails" + KVSep;
            public static string ObjectPropertiesUpdated = ItemSep + "ObjectPropertiesUpdated" + KVSep;
            public static string SentToAddress = ItemSep + "SentToAddress" + KVSep;
            public static string UpdatedValues = ItemSep + "UpdatedValues" + KVSep;
        }

        public static class Common
        {
            public const string DefaultEmptyObjectId = "000000000000000000000000";
            public const string DefaultStaticObjectId = "111111111111111111111111";

            public const string ItemSep = "|";
            public const string KVSep = ":";
            public const string True = "True";
            public const string False = "False";
            public const string Disabled = "Disabled";
            public const string Weekly = "Weekly";
            public const string Monthly = "Monthly";
            public const string Quarterly = "Quarterly";
            public const string Annually = "Annually";

            // Payment Terms
            public const string DueUponReceipt = "Due Upon Receipt";
            public const string Net10 = "Net 10";
            public const string Net15 = "Net 15";
            public const string Net30 = "Net 30";
            public const string Adv2Percent15Net30 = "2% 15, net 30";
            public const string Adv4Percent7Net30 = "4% 7, net 30";

            public const string DefaultAdminId = "5387d81e1c863364a8292fc5";
            public const string BillConfig = "BillConfig";
            public const string Client = "Client";
            public const string Group = "Group";
        }

        public static class PaymentMethod
        {
            public const string None = "BillPaymentNone";
            public const string ACH = "BillPaymentACH";
            public const string CreditCard = "BillPaymentCreditCard";
            public const string ManualCheck = "BillPaymentManualCheck";
            public const string WireTransfer = "BillPaymentWireTransfer";
        }

        public static class Tiers
        {
            public const string DefaultRates = "0" + Common.KVSep + "0.10" + Common.ItemSep + "10000" + Common.KVSep + "0.09" + Common.ItemSep + "25000" + Common.KVSep + "0.08";
            public const string DefaultTaxRate = "0.00";

            public const string AdMessageSent = "AdMessageSent";
            public const string AdEnterOtpScreenSent = "AdEnterOtpScreenSent";
            public const string AdVerificationScreenSent = "AdVerificationScreenSent";

            public const string AdClicked = "AdsClicked";
            public const string EndUserRegister = "EndUserRegister";
            public const string EndUserVerify = "EndUserVerify";
            public const string OtpSentEmail = "OtpSentEmail";
            public const string OtpSentSms = "OtpSentSms";
            public const string OtpSentVoice = "OtpSentVoice";
            public const string TaxRate = "TaxRate";
        }

        public static class DeliveryMethod
        {
            public static Tuple<string, ObjectId> Email = new Tuple<string, ObjectId>("Email", ObjectId.Parse("52a9ff62675c9b04c077107f"));
            public static Tuple<string, ObjectId> Sms = new Tuple<string, ObjectId>("Sms", ObjectId.Parse("52ade07f2b0b6d13f88c4d99"));
            public static Tuple<string, ObjectId> Voice = new Tuple<string, ObjectId>("Voice", ObjectId.Parse("52ade0802b0b6d13f88c4da0"));
        }

        public static class EventLog
        {
            #region Event class range initialization

                private const Int32 EventLogExceptionStartRange = 2000;
                private const Int32 EventLogBillingStartRange = 15000;
                private const Int32 EventLogBillingAddendumStartRange = 15500;

            #endregion

            #region Event Types

                public static class Bill
                {
                    public static Tuple<int, string, string> Base = new Tuple<int, string, string>(EventLogBillingStartRange, "Billing (Base)", "Base Billing Event Class");
                    public static Tuple<int, string, string> Created = new Tuple<int, string, string>(Base.Item1 + 1, "Billing (Created)", "Bill ([" + TokenKeys.BillId.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Created. Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> Updated = new Tuple<int, string, string>(Created.Item1 + 1, "Billing (Updated)", "Bill ([" + TokenKeys.BillId.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Updated. [" + TokenKeys.ObjectPropertiesUpdated.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "].  Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> Saved = new Tuple<int, string, string>(Updated.Item1 + 1, "Billing (Saved)", "Bill ([" + TokenKeys.BillId.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Saved. Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> Sent = new Tuple<int, string, string>(Saved.Item1 + 1, "Billing (Sent)", "Bill ([" + TokenKeys.BillId.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Sent. Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> Printed = new Tuple<int, string, string>(Sent.Item1 + 1, "Billing (Printed)", "Bill ([" + TokenKeys.BillId.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Printed. Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> Paid = new Tuple<int, string, string>(Printed.Item1 + 1, "Billing (Paid)", "Bill ([" + TokenKeys.BillId.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Paid. Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> Voided = new Tuple<int, string, string>(Paid.Item1 + 1, "Billing (Voided)", "Bill ([" + TokenKeys.BillId.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Voided. Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                }

                public static class Addendum
                {
                    public static Tuple<int, string, string> Base = new Tuple<int, string, string>(EventLogBillingAddendumStartRange, "Billing Addendum (Base)", "Base Bill Addendum Event Class");
                    public static Tuple<int, string, string> Created = new Tuple<int, string, string>(Base.Item1 + 1, "Billing Addendum (Created)", "Bill Addendum ([" + TokenKeys.BillId.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Created. Amount: $[" + TokenKeys.BillAddendumAmount.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] - Notes: [" + TokenKeys.BillAddendumNotes.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]. Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> Updated = new Tuple<int, string, string>(Created.Item1 + 1, "Billing Addendum (Updated)", "Bill Addendum ([" + TokenKeys.BillId.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Updated. [" + TokenKeys.ObjectPropertiesUpdated.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "].  Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> Attached = new Tuple<int, string, string>(Updated.Item1 + 1, "Billing Addendum (Attached)", "Bill Addendum ([" + TokenKeys.BillId.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Attached to bill ([" + TokenKeys.BillId.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> Voided = new Tuple<int, string, string>(Attached.Item1 + 1, "Billing Addendum (Voided)", "Bill Addendum ([" + TokenKeys.BillId.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                }

                public static class Exceptions
                {
                    public static Tuple<int, string, string> Base = new Tuple<int, string, string>(EventLogExceptionStartRange, "Exceptions (Base)", "Base Exception Event Class");
                    public static Tuple<int, string, string> General = new Tuple<int, string, string>(Base.Item1 + 1, "Exceptions (General)", "[" + TokenKeys.ExceptionDetails.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                }

            #endregion
        }

        #region Web Config
        public static class WebConfig
        {
            public static class ConnectionStringKeys
            {
                public const string MongoServer = "MongoServer";
            }

            public static class AppSettingsKeys
            {
                public const string MongoDbName = "MongoDbName";
            }
        }
        #endregion

    }
}
