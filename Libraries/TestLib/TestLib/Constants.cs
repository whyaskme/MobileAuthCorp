
namespace TestLib
{
    public static class TestConstants
    {
        public const string UserTestFiles = "UserTestFiles";
        public const string GetTestClientsInfoUrl = "/Test/GetClients.asmx/WsGetClients";
        public const string MacTestBankUrl = "/Test/MacTestBank.asmx/WsMacTestBank";
        public const string ClientServicesUrl = "/AdminServices/ClientServices.asmx/WsClientServices";
        public const string MacReplyServiceUrl = "/Test/GetReply.asmx/WsGetReply";
        public static class File
        {
            public static class LineDirectives
            {
                public const string STSRegister = "STSRegister";
                public const string MACBank = "MACBank";
                public const string RequestOTP = "RequestOTP";
            }
        }
        public class TestBank
        {
            // database collections names
            public const string Bank = "Bank";
            public const string Accounts = "Accounts";

            // known names
            public const string MACBank = "MACBank";
            public const string MACBank_BIN = "66010";
            public const string IssueCard = "IssueCard";
            public const string CancelCards = "CancelCards";
            public const string AssignAccount = "AssignAccount";
            public const string AssignAndReg = "AssignAndReg";
            public const string DeleteUserAccount = "DeleteUserAccount";
            public const string GetBankStatus = "GetBankStatus";
            public const string CreateAccounts = "CreateAccounts";
            public const string AccountHold = "AccountHold";
            public const string ResetAssignedAccounts = "ResetAssignedAccounts";
            public const string UnassignAllAccounts = "UnassignAllAccounts";
            public const string GetAccountDetails = "GetAccountDetails";
            public const string GetAccountBalance = "GetAccountBalance";
            public const string GetAccountLog = "GetAccountLog";
            public const string Preauth = "Preauth";
            public const string Debit = "Debit";
            public const string Credit = "Credit";
            public const string MoveFunds = "MoveFunds";
            public const string ValidateLoginName = "ValidateLoginName";
            public const string GetSubAccStatus = "GetSubAccStatus";
            public const string DebitAccount = "DebitAccount";
            public const string CreditAccount = "CreditAccount";
            public const string DeleteAccount = "DeleteAccount";
            public const string Purchase = "Purchase";
            public const string PurchaseReply = "PurchaseReply";
            public const string TestPurchaseReply = "TestPurchaseReply";
            public const string GetAccountNamesNumbers = "GetAccountNamesNumbers";
            public const string AddBill = "AddBill";
            public const string PayBill = "PayBill";
            public const string GetBills = "GetBills";
            public const string GetMerchantList = "GetMerchantList";
            public const string Full = "Full";
            public const string Type = "Type";
            public const string User = "User";
            public const string Merchant = "Merchant";
            public const string Utility = "Utility";

            public const string PAN = "PAN";
            public const string AccountNo = "AccountNo";
            public const string CardNumber = "CardNumber";
            public const string CardName = "CardName";
            public const string AccountHolder = "AccountHolder";
            public const string MerchantName = "MerchantName";
            public const string AccountName = "AccountName";
            public const string UserIndex = "UserIndex";
            public const string Subaccount = "Subaccount";
            public const string Number = "Number";
            public const string Info = "Info";
            public const string Amount = "Amount";
            public const string FromAccountHolder = "FromAccountHolder";
            public const string FromAccountName = "FromAccountName";
            public const string FromAccountNo = "FromAccountNo";
            public const string ToAccountHolder = "ToAccountHolder";
            public const string ToAccountNo = "ToAccountNo";
            public const string ToAccountName = "ToAccountName";
            public const string Status = "Status";
            public const string Balance = "Balance";
            public const string ExpirationDate = "ExpirationDate";
            public const string Usage = "Usage";
            public const string UsageAccount = "Account";
            public const string UsageGroup = "Group";
            public const string UsageOpen = "Open";
            public const string UsageClient = "Client";
            public const string SubAccount = "SubAccount";
            public const string LastAccessed = "LastAccessed";
            public const string Bills = "Bills";
            public const string Bill = "Bill";
            public const string Name = "Name";
            public const string InvoiceNumber = "InvoiceNumber";
            public const string BusinessType = "BusinessType";
            public const string BillingDate = "BillingDate";
            public const string AmountDue = "AmountDue";
            public const string LastPayment = "LastPayment";
            public const string DueDate = "DueDate";
            public const string Description = "Description";
            public const string Account = "Account";
            public const string Paid = "Paid";
            public const string Updated = "Updated";

            public const string TotalAccounts = "TotalAccounts";
            public const string AssignedAccounts = "AssignedAccounts";
            public const string PANList = "PANList";
            public const string AccountHoldersList = "AccountHoldersList";
            public const string AccountNamesList = "AccountNamesList";
            public const string LoginNamesList = "LoginNamesList";
            public const string Success = "Success";
            public const string Details = "Details";
            public const string ActivityLog = "ActivityLog";
            public const string LogEntry = "LogEntry";
            public const string Limit = "Limit";
            public const string Enabled = "Enabled";
            public const string LoginName = "LoginName";
        }

        public class TestAds
        {
            public const string CreateAd = "CreateAd";
            public const string GetAd = "GetAd";
            public const string AID = "AID";
            public const string Name = "Name";
            public const string Catagory = "Catagory";
            public const string Desc = "Desc";
            public const string TextAd = "TextAd";
            public const string PageAd = "PageAd";

            public const string AdClientId = "AdClientId";
            public const string AdCampaignId = "AdCampaignId";
            public const string AdType = "AdType";



            public class TestCampaigns
            {
                public class MACOnlineBank
                {
                    public const string AdClientId = "336e5f5c-67fd-4813-a529-10ad1125e48f";
                    public class AdCampaign1
                    {
                        public const string CampaignName = "MAC Online Bank Campaign 1";
                        public const string CampaignId = "ed3f576d-23ec-49b0-9695-a45960354ad6";  
                    }

                    public class AdCampaign2
                    {
                        public const string CampaignName = "MAC Online Bank Campaign 1";
                        public const string CampaignId = "81554ddf-1961-48de-af14-a93d532d63cb";
                    }
                }
                public class MACOnlineMerchant
                {
                    public const string AdClientId = "bb246fbb-ec75-4304-bb47-6333a2a5a4af";
                    public class AdCampaign1
                    {
                        public const string CampaignName = "MAC Online Merchant Campaign 1";
                        public const string CampaignId = "3fd7e13b-a4e5-4d67-897d-c8f6c68400f4";
                    }

                    public class AdCampaign2
                    {
                        public const string CampaignName = "MAC Online Merchant Campaign 2";
                        public const string CampaignId = "a41edb58-637d-4809-ab3e-a37931e7835a";
                    }
                }
            }

            public static class Ads
            {
                public const string GolfTestAdsFolder = "ads/golf/";
                public const string BankTestAdsFolder = "ads/bank/";
                public const string TestTestAdsFolder = "ads/test/";
                public const string TestAdsImageFileType = ".jpg";
                public const string DefaultAdText = "Click the following link. ";
                // Demo Ads for Golf, Bank and Test
                public const string HostName = "[HostName]";
                public const string ClientKey = "[ClientKey]";
                public const string AdId = "[AdId]";
                public const string AdLink = "[AdLink]";
                public const string AdImage = "[AdImage]";
                public const string AWSAdServerDomain = "www.otp-ap.us";
                public const string DemoAdServerDomain = "www.otp-ap.com";
                public const string adDiv =
                    @"<div data-ad-id='AdId" + AdId + "'>" +
                    @"<a target='_blank' href='" + AdLink + "'>" +
                    @"<img src='" + HostName + "/" + AdImage + "' style='max-width: 335px !important;width: 100% !important;' border='0'>" +
                    @"</a>" +
                    @"</div>";

                // --------- Golf ads ---------------------------------------------------
                // the following is used by the test ad service seperate the test clients
                public const string GolfClientWord = "golf";
                //the following is used by the demo 404 page (redirection process) to create the "AdClient" value in the Request string
                public const string GolfShopClientKey = "G";    //Golf Shop
                public const string GolfStoreClientKey = "H";   //Golf Store
                //the following is used by the test ad service to construct the ad image file name
                public const string GolfShopAdClient = "golfshop";    //Golf Shop
                public const string GolfStoreAdClient = "golfstore";   //Golf Store
                // Keywords used by the test ad service to select the Ad
                public const string GolfAdSpecificKeywordSpecials = "Specials";
                public const string GolfAdSpecificKeywordTeeTimes = "Tee Times";
                public const string GDN = "/GolfShop/";
                //the following is used by the test ad service as test before the ad link
                public const string GolfSpecialAdText = "Checkout our specials. ";
                public const string GolfTeeTimeAdText = "Book tee times. ";
                //used by the demo 404 page (redirection process) to redirect the request to the Golf Landing page
                //public const string GolfAdLandingPage = HostName + "/" + ClientKey + "_GL";
                //the following is used
                //    by the test ad service to create the img reference
                //    by the demo 404 page (redirection process) to create the "AdType" in the Request string
                public const string AdId_GS = "GS";     //Golf Specials
                public const string AdId_TT = "TT";     //Golf Tee Times
                public const string AdId_G1 = "G1";     //Golf ad 1
                public const string AdId_G2 = "G2";     //Golf ad 2
                public const string AdId_G3 = "G3";     //Golf ad 3
                public const string AdId_G4 = "G4";     //Golf ad 4
                public const string AdId_G5 = "G5";     //Golf ad 5

                // ----- Bank Ads ----------------------------------------------------
                // the following is used by the test ad service seperate the test clients
                public const string BankAdClientKey = "bank";
                //the following is used by the demo 404 page (redirection process) to create the "AdClient" value in the Request string
                public const string FirstBankClientKey = "B";   //MAC Test Bank
                public const string SecondBankClientKey = "C";  //MAC Test Bank2
                //the following is used by the test ad service to construct the ad image file name
                public const string FirstBankAdClient = "mactestbank";
                public const string SecondBankAdClient = "mactestbank2";
                // Keywords used by the test ad service to select the Ad
                public const string BankAdSpecificKeywordAutoLoans = "Auto Loans";
                public const string BankAdSpecificKeywordCreditCards = "Credit Cards";
                public const string BDN = "/bank/";
                //the following is used by the test ad service as test before the ad link
                public const string BankAutoLoansAdText = "Checkout our auto loans. ";
                public const string BankCardAdText = "Get the right card for you. ";
                //used by the demo 404 page (redirection process) to redirect the request to the Bank Landing page
                //public const string BankAdLandingPage = HostName + "/" + ClientKey + "_BL";
                //the following is used
                //    by the test ad service to create the img reference
                //    by the demo 404 page (redirection process) to create the "AdType" in the Request string
                public const string AdId_AL = "AL";    //Auto Loan
                public const string AdId_CC = "CC";    //Bank Credit card
                public const string AdId_B1 = "B1";     //Bank ad 1
                public const string AdId_B2 = "B2";     //Bank ad 2
                public const string AdId_B3 = "B3";     //Bank ad 3
                public const string AdId_B4 = "B4";     //Bank ad 4
                public const string AdId_B5 = "B5";     //Bank ad 5

                // ------ Test Ads ------------------------------------------------------
                // the following is used by the test ad service seperate the test clients
                public const string TestAdClientKey = "test";
                //the following is used by the demo 404 page (redirection process) to create the "AdClient" value in the Request string
                public const string AvenueBClientKey = "A"; //Avenue B
                public const string TNSClientKey = "T";     //MC/TNS
                public const string STSClientKey = "S";     //STS
                public const string MACClientKey = "M";     //MAC Test
                //the following is used by the test ad service to construct the ad image file name
                public const string AvenueBAdClient = "avenueb";
                public const string TNSAdClient = "tns";
                public const string STSAdClient = "sts";
                public const string MACAdClient = "mac";
                public const string DefaultAdClient = "default";
                // Keywords used by the test ad service to select the Ad
                public const string TestAdSpecificKeywordDiscount = "Discounts";
                public const string TestAdSpecificKeywordSpecials = "Specials";
                public const string TDN = "/Test/";
                //the following is used by the test ad service as text before the ad link
                public const string TestDiscountAdText = "Discounts";
                public const string TestSpecialAdText = "Specials";
                //used by the demo 404 page (redirection process) to redirect the request to the Golf Landing page
                //public const string TestAdLandingPage = HostName + "/" + ClientKey + "_TL";
                //the following is used
                //    by the test ad service to create the img reference
                //    by the demo 404 page (redirection process) to create the "AdType" in the Request string
                public const string AdId_TD = "TD";     //Test Discount
                public const string AdId_TS = "TS";     //Test Specials
                public const string AdId_T1 = "T1";     //Test ad 1
                public const string AdId_T2 = "T2";     //Test ad 2
                public const string AdId_T3 = "T3";     //Test ad 3
                public const string AdId_T4 = "T4";     //Test ad 4
                public const string AdId_T5 = "T5";     //Test ad 5
            }
        }
    }
}
