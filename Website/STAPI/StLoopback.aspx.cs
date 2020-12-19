using System;
using System.Linq;
using Newtonsoft.Json.Linq;

using STLib;
using strt = STLib.STConstants.ResponsesTemplates;
using cfg = STLib.STConstants.Config;

public partial class StLoopback : System.Web.UI.Page
{
    MACUtils mMACUtils = new MACUtils();
    protected void Page_Load(object sender, EventArgs e)
    {
        var QS = Request.QueryString["Data"];
        if (String.IsNullOrEmpty(QS))
        {
            var error = strt.SystemErrorTemplates
                .Replace("[EC]", "ERR_LOP_90001")
                .Replace("[ET]", "Loopback no QueryString")
                .Replace("[DT]", mMACUtils.getSTFormatedDate());
            Response.Write(mMACUtils.StringToHex(error));
            Response.End();
        }
        if (!QS.Contains(":"))
        {
            var error = strt.SystemErrorTemplates
                .Replace("[EC]", "ERR_LOP_90002")
                .Replace("[ET]", "Loopback QueryString error")
                .Replace("[DT]", mMACUtils.getSTFormatedDate());
            Response.Write(mMACUtils.StringToHex(error));
            Response.End();
        }
        var qssplit = QS.Split(':');
        if (qssplit.Count() != 3)
        {
            var error = strt.SystemErrorTemplates
                .Replace("[EC]", "ERR_LOP_90003")
                .Replace("[ET]", "Loopback QueryString 3 fields error")
                .Replace("[DT]", mMACUtils.getSTFormatedDate());
            Response.Write(mMACUtils.StringToHex(error));
            Response.End();
        }
        string mTestCase;
        var pMethod = qssplit[0];
        var pStateId = qssplit[1];
        var jsonInHex = qssplit[2];
        var pJson = mMACUtils.HexToString(jsonInHex);
        dynamic mJsonData = JObject.Parse(pJson);

        // isolate the test case number
        string msiteUsername = mJsonData.siteUsername.ToString();
        mTestCase = msiteUsername.Replace("Loopback Test Case=", "");

        var mJson = string.Empty;
        #region service methods
/*
1 stapiRegisterPlayer
2 stapiPlayerLogin
3 stapiPreCheckSiteValidatePlayerRequest
4 stapiPreCheckSiteCardRequest 
5 DeleteRegisteredCard 
6 stapiRegisterPrePaidAccount
7 stapiLoadPrePaidFunds
8 stapiSubmitPrePaidDeposit
9 stapiSubmitDepositAccountRequest
10 stapiSubmitPrePaidWithdrawal
11 stapiGetTransactionDetails
12 stapiUpdateTransaction 
13 stapiGetRegisteredAccounts
14 stapiGetRegisteredCards
15 stapiGetPlayerSSN
16 stapiUpdatePlayerSSN
17 stapiGetPrePaidAccountHolderInfo
18 stapiGetPrePaidAccountBalance
19 stapiGetPrePaidAccountCVV2
20 stapiPreCheckSiteAccountRequest
21 stapiPrePaidRegisterAndLoad
22 stapiUpdatePrePaidAccountStatus
23 stapiSubmitDepositRequest
24 stapiAccountWithdrawal
25 stapiSelfExcludePlayer
26 stapiModifyPlayerRequest
27 stapiAuthDepositRequest
28 stapiSubmitRefund
29 stapiAddSelfExcludePlayer
30 stapiWithdrawal
31 stapiSubmitReversal
*/
        switch (pMethod)
        {
            case "stapiRegisterPlayer": // 1
                switch (mTestCase)
                {
                    case "1":
                        mJson = stapiRegisterPlayerTestCase1(pJson, pStateId); // no questions
                        break;
                    case "2":
                        mJson = stapiRegisterPlayerTestCase2(pJson, pStateId); // ask questions
                        break;
                    default:
                        mJson = stapiRegisterPlayerTestCase3(pJson, pStateId);
                        break;
                }
                break;

            case "stapiPreCheckSiteValidatePlayerRequest": //2 & 3
                switch (mTestCase) 
                {
                    //Note: this is a little different only test case 4 will return and error response
                    //      this allows 2 step flows to get to the second step
                    case "1":
                    case "2":
                    case "3":
                        mJson = stapiPreCheckSiteValidatePlayerRequestTestCase1(pJson, pStateId);
                        break;
                    default:
                        mJson = stapiPreCheckSiteValidatePlayerRequestTestCase2(pJson, pStateId);
                       break;
                }
                break;

            case "stapiPreCheckSiteCardRequest": // 4
                switch (mTestCase)
                {
                    case "1": //successful response
                        mJson = stapiPreCheckSiteCardRequestTestCase1(pJson, pStateId);
                        break;
                    default: //error Authorization failure
                        mJson = stapiPreCheckSiteCardRequestTestCase2(pJson, pStateId);
                        break;
                }
                break;

            // DeleteRegisteredCard 5 uses stapiPreCheckSiteModifyCardRequest

            case "stapiRegisterPrePaidAccount": // 6
                switch (mTestCase)
                {
                    case "1": //successful response
                        mJson = stapiRegisterPrePaidAccountTestCase1(pJson, pStateId);
                        break;
                    case "2": //ask questions response
                        mJson = stapiRegisterPrePaidAccountTestCase2(pJson, pStateId);
                        break;
                    default: //error Authorization failure
                        mJson = stapiRegisterPrePaidAccountTestCase3(pJson, pStateId);
                        break;
                }
                break;

            case "stapiLoadPrePaidFunds": // 7
                switch (mTestCase)
                {
                    case "1": //successful response
                        mJson = stapiLoadPrePaidFundsTestCase1(pJson, pStateId);
                        break;
                    default: //error Authorization failure
                        mJson = stapiLoadPrePaidFundsTestCase2(pJson, pStateId);
                        break;
                }
                break;

            case "stapiSubmitPrePaidDeposit": // 8
                switch (mTestCase)
                {
                    case "1": //successful response
                        mJson = stapiSubmitPrePaidDepositTestCase1(pJson, pStateId);
                        break;
                    default: //error Authorization failure
                        mJson = stapiSubmitPrePaidDepositTestCase2(pJson, pStateId);
                        break;
                }
                break;

            case "stapiSubmitDepositAccountRequest": // 9
                switch (mTestCase)
                {
                    case "1": //successful response
                        mJson = stapiSubmitDepositAccountRequestTestCase1(pJson, pStateId);
                        break;
                    default: //error Authorization failure
                        mJson = stapiSubmitDepositAccountRequestTestCase2(pJson, pStateId);
                        break;
                }
                break;

            case "stapiSubmitPrePaidWithdrawal": // 10
                switch (mTestCase)
                {
                    case "1":
                        mJson = stapiSubmitPrePaidWithdrawalTestCase1(pJson, pStateId);
                        break;
                    default:
                        mJson = stapiSubmitPrePaidWithdrawalTestCase2(pJson, pStateId);
                       break;
                }
                break;

            case "stapiGetTransactionDetails": // 11
                switch (mTestCase)
                {
                    case "1": //successful response
                        mJson = stapiGetTransactionDetailsTestCase1(pJson, pStateId);
                        break;
                    default: //error Authorization failure
                        mJson = stapiGetTransactionDetailsTestCase2(pJson, pStateId);
                        break;
                }
                break;

            case "stapiUpdateTransaction": //12
                switch (mTestCase)
                {
                    case "1": //successful response
                        mJson = stapiUpdateTransactionTestCase1(pJson, pStateId);
                        break;
                    default: //error Authorization failure
                        mJson = stapiUpdateTransactionTestCase2(pJson, pStateId);
                        break;
                }
                break;

            case "stapiGetRegisteredAccounts": //13
                switch (mTestCase)
                {
                    case "1": //successful response
                        mJson = stapiGetRegisteredAccountsTestCase1(pJson, pStateId);
                        break;
                    default: //error Authorization failure
                        mJson = stapiGetRegisteredAccountsTestCase2(pJson, pStateId);
                        break;
                }
                break;

            case "stapiGetRegisteredCards": // 14
                switch (mTestCase)
                {
                    case "1": //successful response
                        mJson = stapiGetRegisteredCardsTestCase1(pJson, pStateId);
                        break;
                    default: //error Authorization failure
                        mJson = stapiGetRegisteredCardsTestCase2(pJson, pStateId);
                        break;
                }
                break;

            case "stapiGetPlayerSSN": // 15
                switch (mTestCase)
                {
                    case "1": //successful response
                        mJson = stapiGetPlayerSSNTestCase1(pJson, pStateId);
                        break;
                    default: //error Authorization failure
                        mJson = stapiGetPlayerSSNTestCase2(pJson, pStateId);
                        break;
                }
                break;

            case "stapiUpdatePlayerSSN":  // 16
                switch (mTestCase)
                {
                    case "1": //successful response
                        mJson = stapiUpdatePlayerSSNTestCase1(pJson, pStateId);
                        break;
                    default: //error Authorization failure
                        mJson = stapiUpdatePlayerSSNTestCase2(pJson, pStateId);
                        break;
                }
                break;

            case "stapiGetPrePaidAccountHolderInfo":  // 17
                switch (mTestCase)
                {
                    case "1": //successful response
                        mJson = stapiGetPrePaidAccountHolderInfoTestCase1(pJson, pStateId);
                        break;
                    default: //error Authorization failure
                        mJson = stapiGetPrePaidAccountHolderInfoTestCase2(pJson, pStateId);
                        break;
                }
                break;

            case "stapiGetPrePaidAccountBalance": // 18
                switch (mTestCase)
                {
                    case "1": //successful response
                        mJson = stapiGetPrePaidAccountBalanceTestCase1(pJson, pStateId);
                        break;
                    default: //error Authorization failure
                        mJson = stapiGetPrePaidAccountBalanceTestCase2(pJson, pStateId);
                        break;
                }
                break;

            case "stapiGetPrePaidAccountCVV2": // 19
                switch (mTestCase)
                {
                    case "1": //successful response
                        mJson = stapiGetPrePaidAccountCVV2TestCase1(pJson, pStateId);
                        break;
                    default: //error Authorization failure
                        mJson = stapiGetPrePaidAccountCVV2TestCase2(pJson, pStateId);
                        break;
                }
                break;

            case "stapiPreCheckSiteAccountRequest": // 20
                switch (mTestCase)
                {
                    case "1": //successful response
                        mJson = stapiPreCheckSiteAccountRequestTestCase1(pJson, pStateId);
                        break;
                    default: //error Authorization failure
                        mJson = stapiPreCheckSiteAccountRequestTestCase2(pJson, pStateId);
                        break;
                }
                break;

            case "stapiPrePaidRegisterAndLoad": // 21
                switch (mTestCase)
                {
                    case "1": //successful response
                        mJson = stapiPrePaidRegisterAndLoadTestCase1(pJson, pStateId);
                        break;
                    default: //error Authorization failure
                        mJson = stapiPrePaidRegisterAndLoadTestCase2(pJson, pStateId);
                        break;
                }
                break;

            case "stapiUpdatePrePaidAccountStatus": // 22
                switch (mTestCase)
                {
                    case "1": //successful response
                        mJson = stapiUpdatePrePaidAccountStatusTestCase1(pJson, pStateId);
                        break;
                    default: //error Authorization failure
                        mJson = stapiUpdatePrePaidAccountStatusTestCase2(pJson, pStateId);
                        break;
                }
                break;

            case "stapiSubmitDepositRequest": // 23
                switch (mTestCase)
                {
                    case "1": //successful response
                        mJson = stapiSubmitDepositRequestTestCase1(pJson, pStateId);
                        break;
                    default: //error Authorization failure
                        mJson = stapiSubmitDepositRequestTestCase2(pJson, pStateId);
                        break;
                }
                break;

            case "stapiAccountWithdrawal": // 24
                switch (mTestCase)
                {
                    case "1": //successful response
                        mJson = stapiAccountWithdrawalTestCase1(pJson, pStateId);
                        break;
                    default: //error Authorization failure
                        mJson = stapiAccountWithdrawalTestCase2(pJson, pStateId);
                        break;
                }
                break;

            case "stapiSelfExcludePlayer": // 25
                switch (mTestCase)
                {
                    case "1": //successful response
                        mJson = stapiSelfExcludePlayerTestCase1(pJson, pStateId);
                        break;
                    default: //error Authorization failure
                        mJson = stapiSelfExcludePlayerTestCase2(pJson, pStateId);
                        break;
                }
                break;

            case "stapiModifyPlayerRequest": // 26
                switch (mTestCase)
                {
                    case "1": //successful response
                        mJson = stapiModifyPlayerRequestTestCase1(pJson, pStateId);
                        break;
                    default: //error Authorization failure
                        mJson = stapiModifyPlayerRequestTestCase2(pJson, pStateId);
                        break;
                }
                break;

            case "stapiAuthDepositRequest": // 27
                switch (mTestCase)
                {
                    case "1": //successful response
                        mJson = stapiAuthDepositRequestTestCase1(pJson, pStateId);
                        break;
                    default: //error Authorization failure
                        mJson = stapiAuthDepositRequestTestCase2(pJson, pStateId);
                        break;
                }
                break;

            case "stapiSubmitRefund": // 28
                switch (mTestCase)
                {
                    case "1": //successful response
                        mJson = stapiSubmitRefundTestCase1(pJson, pStateId);
                        break;
                    default: //error Authorization failure
                        mJson = stapiSubmitRefundTestCase2(pJson, pStateId);
                        break;
                }
                break;

            case "stapiAddSelfExcludePlayer": // 29
                switch (mTestCase)
                {
                    case "1": //successful response
                        mJson = stapiAddSelfExcludePlayerTestCase1(pJson, pStateId);
                        break;
                    default: //error Authorization failure
                        mJson = stapiAddSelfExcludePlayerTestCase2(pJson, pStateId);
                        break;
                }
                break;

            case "stapiWithdrawal": // 30
                switch (mTestCase)
                {
                    case "1": //successful response
                        mJson = stapiWithdrawalTestCase1(pJson, pStateId);
                        break;
                    default: //error Authorization failure
                        mJson = stapiWithdrawalTestCase2(pJson, pStateId);
                        break;
                }
                break;

            case "stapiSubmitReversal": // 31
                switch (mTestCase)
                {
                    case "1":
                        mJson = stapiSubmitReversalTestCase1(pJson, pStateId);
                        break;
                    default:
                        mJson = stapiSubmitReversalTestCase2(pJson, pStateId);
                       break;
                }
                break;

            case "stapiSubmitPlayerKBA":
                switch (mTestCase)
                {
                    case "1":
                        mJson = stapiSubmitPlayerKBATestCase1(pJson, pStateId);
                        break;
                    default:
                        mJson = stapiSubmitPlayerKBATestCase2(pJson, pStateId);
                       break;
                }
                break;

            case "stapiSubmitModifiedPlayerKBA":
                switch (mTestCase)
                {
                    case "1": //successful response
                        mJson = stapiSubmitModifiedPlayerKBATestCase1(pJson, pStateId);
                        break;
                    default: //error Authorization failure
                        mJson = stapiSubmitModifiedPlayerKBATestCase2(pJson, pStateId);
                        break;
                }
                break;

            case "stapiPreCheckSiteModifyCardRequest":
                switch (mTestCase)
                {
                    case "1": //successful response
                        mJson = stapiPreCheckSiteModifyCardRequestTestCase1(pJson, pStateId);
                        break;
                    default: //error Authorization failure
                        mJson = stapiPreCheckSiteModifyCardRequestTestCase2(pJson, pStateId);
                        break;
                }
                break;

            case "stapiPreCheckSiteModifyAccountRequest":
                switch (mTestCase)
                {
                    case "1": //successful response
                        mJson = stapiPreCheckSiteModifyAccountRequestTestCase1(pJson, pStateId);
                        break;
                    default: //error Authorization failure
                        mJson = stapiPreCheckSiteModifyAccountRequestTestCase2(pJson, pStateId);
                        break;
                }
                break;

            //case "stapiSubmitPrePaidIdentityAnswers":
            //    switch (mTestCase)
            //    {
            //        case "1": //successful response
            //            mJson = stapiSubmitPrePaidIdentityAnswersTestCase1(pJson, pStateId);
            //            break;
            //        default: //error Authorization failure
            //            mJson = stapiSubmitPrePaidIdentityAnswersTestCase2(pJson, pStateId);
            //            break;
            //    }
            //    break;

            default:
                var error = strt.SystemErrorTemplates
                    .Replace("[EC]", "ERR_LOP_90004")
                    .Replace("[ET]", "Loopback invalid method error:" + pMethod)
                    .Replace("[DT]", mMACUtils.getSTFormatedDate());
                Response.Write(mMACUtils.StringToHex(error));
                Response.End();
                break;
        }
        #endregion
        #region Check response
        try
        {
            dynamic test = JObject.Parse(mJson);
        }
        catch (Exception ex)
        {
            var errorJson =
                "{"
                + "\"returnCode\":\"ERR_LOP_90005\","
                + "\"returnMsg\":\"Bad response json " + pMethod + "[" + ex.Message + "]\","
                + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
                + "\"recordStatus\":\"Error\""
                + "}";
            Response.Write(mMACUtils.StringToHex(errorJson));
            Response.End();
        }
        #endregion
        Response.Write(mMACUtils.StringToHex(mJson));
        Response.End();
    }

    #region Json Response Builders for test cases
    private string stapiUpdatePlayerSSNTestCase1(string pJson, String pState)
    {
        dynamic mJsonData = JObject.Parse(pJson);
        string userName = mJsonData.userName.ToString();
        string operatorId = mJsonData.operatorId.ToString();
        string siteId = mJsonData.siteId.ToString();

        var mJson =
        "{"
        + "\"returnCode\":\"INFO_PFO_00023\","
        + "\"returnMsg\":\"Request processed successfully(loopback - UpdatePlayerSSN).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
        + "\"recordStatus\":\"Updated\","
        + "\"ssnDetails\":"
            +"{"
                + "\"operatorId\":\"" + operatorId + "\","
                + "\"siteId\":\"" + siteId + "\","
                + "\"userName\":\"" + userName + "\","
                + "\"ssn\":\"000000000"
            +"}"
        +"}";
        return mJson;
    }
    private string stapiUpdatePlayerSSNTestCase2(string pJson, String pState)
    {
        var mJson =
        "{"
        + "\"returnCode\":\"ERR_PFO_00001\","
        + "\"returnMsg\":\"Validation Failure - Field validation failure(loopback - UpdatePlayerSSN). Please see validation Errors for more details.\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
        + "\"recordStatus\":\"Failed\","
        + "\"validationErrors\":"
            + "["
                + "{"
                + "\"validationError\":\"Invalid operatorId.\","
                + "\"validationField\":\"operatorId\""
                + "}"
            + "]"
        + "}";
        return mJson;
    }

    private string stapiGetPlayerSSNTestCase1(string pJson, String pState)
    {
        dynamic mJsonData = JObject.Parse(pJson);
        string userName = mJsonData.userName.ToString();
        string operatorId = mJsonData.operatorId.ToString();
        string siteId = mJsonData.siteId.ToString();

        var mDatabase = new MongoDBData();
        var mPlayer = mDatabase.GetPlayerByUsername(userName);

        var mJson =
        "{"
        + "\"returnCode\":\"INFO_PFO_00023\","
        + "\"returnMsg\":\"Request processed successfully(loopback - GetPlayerSSN).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
        + "\"ssnDetails\":"
            + "{"
            + "\"operatorId\":\"" + operatorId + "\","
            + "\"siteId\":\"" + siteId + "\","
            + "\"userName\":\"" + userName + "\","
            + "\"ssn\":\"" + mPlayer.ssn + "\""
            + "}"
        + "}";
        return mJson;
    }
    private string stapiGetPlayerSSNTestCase2(string pJson, String pState)
    {
        var mJson =
        "{"
        + "\"returnCode\":\"ERR_PFO_00001\","
        + "\"returnMsg\":\"Validation Failure - Field validation failure(loopback - GetPlayerSSN).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
        + "\"recordStatus\":\"Failed\","
        + "\"validationErrors\":"
            + "["
                + "{"
                + "\"validationError\":\"Invalid operatorId.\","
                + "\"validationField\":\"operatorId\""
                + "}"
            + "]"
        + "}";
        return mJson;
    }

    private string stapiGetRegisteredAccountsTestCase1(string pJson, String pState)
    {
        dynamic mJsonData = JObject.Parse(pJson);
        string mOperatorId = mJsonData.operatorId.ToString();
        string mSiteId = mJsonData.siteId.ToString();
        string mUserName = mJsonData.userName.ToString();

        var mJson =
        "{"
        + "\"returnCode\":\"INFO_PFO_00023\","
        + "\"returnMsg\":\"Request processed successfully(loopback - GetRegisteredAccounts).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
        + "\"operatorId\":\"" + mOperatorId + "\","
        + "\"siteId\":\"" + mSiteId + "\","
        + "\"userName\":\"" + mUserName + "\","
        + "\"accountList\":"
            + "["
                + "{"
                + "\"bankAccountType\":\"SAVINGS\","
                + "\"accountNumber\":\"51############01\","
                + "\"accountToken\":\"gCIxtcEQSdV38bO7Lav_J50eq-\","
                + "\"bankName\":\"FIRSTBANK\""
                + "}"
            + "]"
        + "}";
        return mJson;
    }
    private string stapiGetRegisteredAccountsTestCase2(string pJson, String pState)
    {
        var mJson =
        "{"
        + "\"returnCode\":\"ERR_PFO_00001\","
        + "\"returnMsg\":\"Validation Failure - Field validation failure(loopback - GetRegisteredAccounts).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
        + "\"recordStatus\":\"Failed\","
        + "\"validationErrors\":"
            + "["
                + "{"
                + "\"validationError\":\"Invalid siteUsername.\","
                + "\"validationField\":\"siteUsername\""
                + "}"
            + "]"
        + "}";
        return mJson;
    }

    private string stapiGetPrePaidAccountHolderInfoTestCase1(string pJson, String pState)
    {
        /*
        { 
            "operatorId":"000", 
            "siteId":"000", 
            "siteUsername":"123789456245", "sitePwd":")(*&^%", "playerDetails": 
 	        { 
 	            "userName":"johnsmith1", 
 	            "connectionToken":"6789gfgdfhgdfgdfghhtr4", 
 	            "sessionToken":"200446",  	"prePaidAccountDetails": 
 	 	        { 
 	 	            "prePaidAccountToken":"IpFlmsW9vWXpH3DAVsMi-g=" 
 	 	        } 
 	        } 
        } 
         * */
        dynamic mJsonData = JObject.Parse(pJson);
        string mOperatorId = mJsonData.operatorId.ToString();
        string mSiteId = mJsonData.siteId.ToString();
        string userName = mJsonData.playerDetails.userName.ToString();

        var mMd = new MongoDBData();
        var mPlayer = mMd.GetPlayerByUsername(userName);

        var mJson =
        "{"
        + "\"returnCode\":\"INFO_PFO_00023\","
        + "\"returnMsg\":\"Request processed successfully(loopback - GetPrePaidAccountHolderInfo).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
        + "\"prePaidAccountDetails\":"
            + "{"
            + "\"operatorId\":\"" + mOperatorId + "\","
            + "\"siteId\":\"" + mSiteId + "\","
            + "\"prePaidAccountNumber\":\"" + mPlayer.PPANumber + "\","
            + "\"prePaidAccountStatus\":\"" + "Active" + "\","
            + "\"prePaidAccountReason\":\"\","
            + "\"prePaidAccountHolderDetails\":"
                + "{"
                + "\"firstName\":\"" + mPlayer.firstName + "\","
                + "\"lastName\":\"" + mPlayer.lastName + "\","
                + "\"streetAddress1\":\"" + mPlayer.playerAddress1 + "\","
                + "\"streetAddress2\":\"" + mPlayer.playerAddress2 + "\","
                + "\"city\":\"" + mPlayer.city + "\","
                + "\"stateCode\":\"\","
                + "\"zipCode\":\"" + mPlayer.zipCode + "\","
                + "\"countryCode\":\"" + "US" + "\","
                + "\"dob\":\"" + mPlayer.dob + "\","
                + "\"eveningPhone\":\"" + mPlayer.landLineNo + "\","
                + "\"dayPhone\":\"" + mPlayer.mobileNo + "\""
                + "}"
            + "}"
        + "}";
        return mJson;
    }
    private string stapiGetPrePaidAccountHolderInfoTestCase2(string pJson, String pState)
    {
        var mJson =
        "{"
        + "\"returnCode\":\"ERR_PFO_00001\","
        + "\"returnMsg\":\"Validation Failure - Field validation failure(loopback - GetPrePaidAccountHolderInfo).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
        + "\"recordStatus\":\"Failed\","
        + "\"validationErrors\":"
            + "["
                + "{"
                + "\"validationField\":\"siteUsername\","
                + "\"validationError\":\"Invalid siteUsername.\""
                + "}"
            + "]"
        + "}";
        return mJson;
    }

    private string stapiSubmitRefundTestCase1(string pJson, String pState)
    {
        /*
        { 
            "operatorId":"000", 
            "siteId":"000", 
            "siteUsername":"123456789545", 
            "sitePwd":")*(&^%$#", 
            "refundDetails": 
 	        { 
 	            "userName":"johnsmith1", 
 	            "transactionType":"REFUND", 
 	            "depositSTTransactionNo":"1234567890", 
 	            "expiryDate":"", 
                "accountType":"ECOM", 
                "currency":"USD", 
 	            "amount":"1000" 
 	        } 
        } 
        */


        dynamic mJsonData = JObject.Parse(pJson);
        string mOperatorId = mJsonData.operatorId.ToString();
        string mSiteId = mJsonData.siteId.ToString();
        string mUserName = mJsonData.refundDetails.userName.ToString();
        string mTransactionType = mJsonData.refundDetails.transactionType.ToString();
        string mAmount = mJsonData.refundDetails.amount.ToString();
        string mCurrency = mJsonData.refundDetails.currency.ToString();

        var mJson =
        "{"
        + "\"returnCode\":\"INFO_PFO_00023\","
        + "\"returnMsg\":\"Request processed successfully(loopback - GetPrePaidAccountHolderInfo).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
        + "\"recordStatus\":\"Created\","
        + "\"refundDetails\":"
        + "{"
            + "\"operatorId\":\"" + mOperatorId + "\","
            + "\"siteId\":\"" + mSiteId + "\","
            + "\"userName\":\"" + mUserName + "\","
            + "\"transactionType\":\"" + mTransactionType + "\","
            + "\"amount\":\"" + mAmount + "\","
            + "\"currency\":\"" + mCurrency + "\","
            + "\"authCode\":\"TEST REFUND ACCEPTED\","
            + "\"orderReference\":\"1234\","
            + "\"transactionDate\":\"" + mMACUtils.getSTFormatedDate() + "\","
            + "\"stTransactionNo\":\"" + "1000574052" + "\","
            + "\"accountType\":\"" + "ECOM" + "\","
	        + "\"cardDetails\":"
		    + "{"
                    + "\"cardNumber\":\"411111######0211\","
                    + "\"cardToken\":\"fsfkgbgbdgjkbdfkgjbkdjfbgldfg=\","
                    + "\"cardType\":\"VISA\""
		        + "},"
		        + "\"securityResponses\":"
		        + "{"
		            + "\"securityResponseCVV2\":\"2\","
		            + "\"securityResponseFirstLineofAddress\":\"1\","
		            + "\"securityResponsePostCode\":\"1\""
		        + "},"
		        + "\"additionalResponses\":"
			    + "["
				    + "{"
				    + "\"source\":\"Gateway\","
				    + "\"details\":"
					    + "{"
					        + "\"tid\":\"27882788\","
					        + "\"requestReference\":\"X8899996\","
					        + "\"acquirerResponseCode\":\"00\","
					        + "\"settleDueDate\":\"\","
					        + "\"settleStatus\":\"\","
					        + "\"dccEnabled\":\"0\","
					        + "\"message\":\"Ok\","
					        + "\"errorCode\":\"0\","
					        + "\"issuer\":\"Secure Trading Test Issuer1\","
					        + "\"issuerCountry\":\"US\","
					        + "\"merchantCountryiso2a\":\"GB\","
					        + "\"liveStatus\":\"false\","
					        + "\"merchantName\":\"Wipro Test Account\","
					        + "\"merchantNumber\":\"00000000\""
					    + "}"
				    + "}"
			    + "]"
	        + "}"
        + "}";
        return mJson;
    }
    private string stapiSubmitRefundTestCase2(string pJson, String pState)
    {
        var mJson =
        "{"
        + "\"returnCode\":\"ERR_PFO_00001\","
        + "\"returnMsg\":\"Validation Failure - Field validation failure(loopback - GetPrePaidAccountHolderInfo).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
        + "\"recordStatus\":\"Failed\","
        + "\"validationErrors\":"
            + "["
                + "{"
                + "\"validationError\":\"Invalid operatorId.\","
                + "\"validationField\":\"operatorId\""
                + "}"
            + "]"
        + "}";
        return mJson;
    }

    private string stapiGetTransactionDetailsTestCase1(string pJson, String pState)
    {
        dynamic mJsonData = JObject.Parse(pJson);
        string mOperatorId = mJsonData.operatorId.ToString();
        string mSiteId = mJsonData.siteId.ToString();
        string mOrderReference = mJsonData.orderReference.ToString();
        string mStTransactionNo = mJsonData.stTransactionNo.ToString();

        var mJson =
        "{"
        + "\"returnCode\":\"INFO_PFO_00023\","
        + "\"returnMsg\":\"Request processed successfully(loopback - GetTransactionDetails).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
        + "\"transactionDetails\":"
            + "{"
            + "\"operatorId\":\"" + mOperatorId + "\","
            + "\"siteId\":\"" + mSiteId + "\","
            + "\"stTransactionNo\":\"" + mStTransactionNo + "\","
            + "\"orderReference\":\"" + mOrderReference + "\","
            + "\"transactionDate\":\"" + mMACUtils.getSTFormatedDate() + "\","
            + "\"settledDate\":\"" + mMACUtils.getSTFormatedDate() + "\","
            + "\"status\":\"Pending Settlement\","
            + "\"currency\":\"USD\","
            + "\"amount\":\"1000\""
            + "}"
        + "}";
        return mJson;
    }
    private string stapiGetTransactionDetailsTestCase2(string pJson, String pState)
    {
        var mJson =
            "{"
            + "\"returnCode\":\"ERR_PFO_00001\","
            + "\"returnMsg\":\"Validation Failure - Field validation failure(loopback - GetTransactionDetails).\","
            + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
            + "\"recordStatus\":\"Failed\","
            + "\"validationErrors\":"
                + "["
                    + "{"
                    + "\"validationError\":\"Invalid siteUsername.\","
                    + "\"validationField\":\"siteUsername\""
                    + "}"
                + "]"
            + "}";
        return mJson;
    }

    private string stapiSubmitPrePaidDepositTestCase1(string pJson, String pState)
    {
        /* request json
        "{"
            + "\"operatorId\":\"" + hiddenOperatorId.Value + "\","
            + "\"siteId\":\"" + hiddenSiteId.Value + "\","
            + "\"siteUsername\":\"" + GetSiteUserNameOrLoopbackTestCase() + "\","
            + "\"sitePwd\":\"" + txtSiteUserPassword.Value + "\","
            + "\"geoComplyEncryptedPacket\":\"" + txtGeoInfo.Value + "\","
            + "\"depositDetails\":"
            + "{"
                + "\"userName\":\"" + txtPlayerUsername.Value + "\","
                + "\"connectionToken\":\"" + txtPlayerConnectionToken.Value + "\","
                + "\"sessionToken\":\"" + txtPlayerSessionToken.Value + "\","
                + "\"transactionType\":\"DEPOSIT\","
                + "\"orderReference\":\"1234\","
                + "\"accountType\":\"" + txtPCIAccountType.Value + "\","
                + "\"prePaidAccountToken\":\"" + "1234" + "\","             //todo add to player object and PCI form
                + "\"currency\":\"" + "USD" + "\","                        //todo add to player object and PCI form
                + "\"amount\":\"" + "1000" + "\"" 
            + "}"
        + "}";
         * */
        dynamic mJsonData = JObject.Parse(pJson);
        string operatorId = mJsonData.operatorId.ToString();
        string siteId = mJsonData.siteId.ToString();
        string userName = mJsonData.depositDetails.userName.ToString();
        string transactionType = mJsonData.depositDetails.transactionType.ToString();
        string accountType = mJsonData.depositDetails.accountType.ToString();
        string amount = mJsonData.depositDetails.amount.ToString();
        string currency = mJsonData.depositDetails.currency.ToString();
        string orderReference = mJsonData.depositDetails.orderReference.ToString();
        string prePaidAccountToken = mJsonData.depositDetails.prePaidAccountToken.ToString();

        var mJson =
        "{"
        + "\"recordStatus\":\"Created\","
        + "\"returnCode\":\"INFO_PFO_00023\","
        + "\"returnMsg\":\"Request processed successfully(loopback - SubmitPrePaidDepositTest).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
        + "\"depositDetails\":"
            + "{"
            + "\"userName\":\"" + userName + "\","
            + "\"transactionType\":\"" + transactionType + "\","
            + "\"stTransactionNo\":\"0000000000\","
            + "\"operatorId\":\"" + operatorId + "\","
            + "\"siteId\":\"" + siteId + "\","
            + "\"accountType\":\"" + accountType + "\","
            + "\"amount\":\"" + amount + "\","
            + "\"currency\":\"" + currency + "\","
            + "\"orderReference\":\"" + orderReference + "\","
            + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
            + "\"transactionDate\":\"" + mMACUtils.getSTFormatedDate() + "\","
            + "\"receiptHtml\":\"<value>\","
            + "\"prePaidAccountDetails\":"
                + "{"
                + "\"prePaidAccountNumber\":\"51############01\","
                + "\"prePaidAccountToken\":\"" + prePaidAccountToken + "\","
                + "\"prePaidAccountBalance\":\"" + "100000" + "\""
                + "}"
            + "}"
        + "}";
        return mJson;
    }
    private string stapiSubmitPrePaidDepositTestCase2(string pJson, String pState)
    {
        var mJson =
            "{"
            + "\"returnCode\":\"ERR_PFO_00001\","
            + "\"returnMsg\":\"Validation Failure - Field validation failure(loopback - SubmitPrePaidDepositTest).\","
            + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
            + "\"recordStatus\":\"Failed\","
            + "\"validationErrors\":"
                + "["
                    + "{"
                    + "\"validationError\":\"Invalid operatorId.\","
                    + "\"validationField\":\"operatorId\""
                    + "}"
                + "]"
            + "}";
        return mJson;
    }

    private string stapiAddSelfExcludePlayerTestCase1(string pJson, String pState)
    {
        /* input json
        { 
            "operatorId":"000", 
            "siteId":"000", 
            "siteUsername":"123456789545", 
            "sitePwd":")*(&^%$#", 
            "exclusionDetails": 
            { 
                "userName":"johnsmith1",  
                "fromDate":"03/26/2012", 
                "toDate":"06/05/2012", 
                "requestHelp":"Y", 
                "sessionToken":"233900", 
                "connectionToken":"653be279f22dede9310237db59525bd778523e429b7cdd965f092bba916a0ca1" 
            } 
        } 
        */
        var mJson =
        "{"
        + "\"returnCode\":\"INFO_PFO_00004\","
        + "\"returnMsg\":\"Self Exclusion setup success(loopback - AddSelfExcludePlayer).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
        + "\"recordStatus\":\"Created\""
        + "}";
        return mJson;
    }
    private string stapiAddSelfExcludePlayerTestCase2(string pJson, String pState)
    {
        var mJson =
        "{"
        + "\"returnCode\":\"ERR_PFO_00005\","
        + "\"returnMsg\":\"Invalid UserName(loopback - AddSelfExcludePlayer).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\""
        + "}";
        return mJson;
    }

    private string stapiUpdateTransactionTestCase1(string pJson, String pState)
    {
        dynamic mJsonData = JObject.Parse(pJson);
        string mOperatorId = mJsonData.operatorId.ToString();
        string mSiteId = mJsonData.siteId.ToString();
        string mOrderReference = mJsonData.orderReference.ToString();
        string mStTransactionNo = mJsonData.stTransactionNo.ToString();
        string mStatus = mJsonData.status.ToString();

        var mJson =
        "{"
        + "\"returnCode\":\"INFO_PFO_00023\","
        + "\"returnMsg\":\"Request processed successfully(loopback - UpdateTransaction).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
        + "\"recordStatus\":\"Created\","
        + "\"transactionDetails\":"
            + "{"
            + "\"operatorId\":\"" + mOperatorId + "\","
            + "\"siteId\":\"" + mSiteId + "\","
            + "\"stTransactionNo\":\"" + mStTransactionNo + "\","
            + "\"orderReference\":\"" + mOrderReference + "\","
            + "\"transactionDate\":\"" + mMACUtils.getSTFormatedDate() + "\","
            + "\"settledDate\":\"" + mMACUtils.getSTFormatedDate() + "\","
            + "\"status\":\"" + mStatus + "\","
            + "\"currency\":\"USD\","
            + "\"amount\":\"1000\""
            + "}"
        + "}";
        return mJson;
    }
    private string stapiUpdateTransactionTestCase2(string pJson, String pState)
    {
        var mJson =
            "{"
            + "\"returnCode\":\"ERR_PFO_00001\","
            + "\"returnMsg\":\"Validation Failure - Field validation failure(loopback - UpdateTransaction).\","
            + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
            + "\"recordStatus\":\"Failed\","
            + "\"validationErrors\":"
                + "["
                    + "{"
                    + "\"validationError\":\"Invalid siteUsername.\","
                    + "\"validationField\":\"siteUsername\""
                    + "}"
                + "]"
            + "}";
        return mJson;
    }

    private string stapiSubmitPrePaidWithdrawalTestCase1(string pJson, String pState)
    {
        dynamic mJsonData = JObject.Parse(pJson);
        string mOperatorId = mJsonData.operatorId.ToString();
        string mSiteId = mJsonData.siteId.ToString();
        string mUserName = mJsonData.withdrawalDetails.userName.ToString();
        string mTransactionType = mJsonData.withdrawalDetails.transactionType.ToString();
        string mAmount = mJsonData.withdrawalDetails.amount.ToString();
        string mCurrency = mJsonData.withdrawalDetails.currency.ToString();
        string mOrderReference = mJsonData.withdrawalDetails.orderReference.ToString();
        string mAccountType = mJsonData.withdrawalDetails.accountType.ToString();

        var mJson =
        "{"
        + "\"returnCode\":\"INFO_PFO_00023\","
        + "\"returnMsg\":\"Request processed successfully(loopback - PrePaidWithdrawal).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
        + "\"recordStatus\":\"Created\","
        + "\"withdrawalDetails\":"
        + "{"
            + "\"operatorId\":\"" + mOperatorId + "\","
            + "\"siteId\":\"" + mSiteId + "\","
            + "\"userName\":\"" + mUserName + "\","
            + "\"transactionType\":\"" + mTransactionType + "\","
            + "\"stTransactionNo\":\"1234567890\","
            + "\"orderReference\":\"" + mOrderReference + "\","
            + "\"transactionDate\":\"" + mMACUtils.getSTFormatedDate() + "\","
            + "\"currency\":\"" + mCurrency + "\","
            + "\"amount\":\"" + mAmount + "\","
            + "\"accountType\":\"" + mAccountType + "\","
            + "\"prePaidAccountDetails\":"
                + "{"
                    + "\"prePaidAccountNumber\":\"51############01\","
                    + "\"prePaidAccountToken\":\"" + mSiteId + "\","
                    + "\"prePaidAccountBalance\":\"10000\""
                + "}"
            + "}"
        + "}";
        return mJson;
    }
    private string stapiSubmitPrePaidWithdrawalTestCase2(string pJson, String pState)
    {
        var mJson =
            "{"
            + "\"returnCode\":\"ERR_PFO_00001\","
            + "\"returnMsg\":\"Validation Failure - Field validation failure(loopback - PrePaidWithdrawal).\","
            + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
            + "\"recordStatus\":\"Failed\","
            + "\"validationErrors\":"
                + "["
                    + "{"
                    + "\"validationError\":\"Invalid siteUsername.\","
                    + "\"validationField\":\"siteUsername\""
                    + "}"
                + "]"
            + "}";
        return mJson;
    }

    private string stapiSubmitDepositAccountRequestTestCase1(string pJson, String pState)
    {
        dynamic mJsonData = JObject.Parse(pJson);
        string mOperatorId = mJsonData.operatorId.ToString();
        string mSiteId = mJsonData.siteId.ToString();
        string mUserName = mJsonData.depositDetails.userName.ToString();
        string mTransactionType = mJsonData.depositDetails.transactionType.ToString();
        string mAmount = mJsonData.depositDetails.amount.ToString();
        string mCurrency = mJsonData.depositDetails.currency.ToString();
        string mOrderReference = mJsonData.depositDetails.orderReference.ToString();
        string mAccountType = mJsonData.depositDetails.accountType.ToString();
        /* good response
        { 
            "recordStatus":"Created", 
            "returnCode":"INFO_PFO_00023", 
            "returnMsg":"Request processed successfully", "depositDetails": 
            { 
                "operatorId":"000", 	 
                "siteId":"000", 
                "authCode":"TEST", 
                "accountType":"ACH", 
                "amount":"100000", 
                "currency":"USD", 
                "orderReference":"111", 
                "stTransactionNo":"1000574129", 
                "transactionDate":"2014-06-05T16:23:59.658+05:30", 
                "transactionType":"DEPOSIT", "userName":"Johnsmith1", 
                "accountDetails": 
                { 
                    "accountNumber":"51############01", 
                    "accountToken":"dsfbksdfbksjfbgjsfg=", 
                    "abaNumber":"11#####11", 
                    "bankAccountType":"SAVINGS" 
                }, 
                "atmVerifyDetails": 
                { 
                    "atmVerifyDesc":"P70:VALIDATED", 
                    "transactionType":"ACCOUNTCHECK", 
                    "atmVerifyResult":"PASS" 
                }, 
                    "securityResponses": 
                { 
                    "securityResponseCVV2":"2", 
                    "securityResponseFirstLineofAddress":"1", 
                    "securityResponsePostCode":"1" 
                }, 
                    "additionalResponses": 
                [ 
                    { 
                        "source":"Gateway",  	 	
                        "details": 
                        { 
                            "requestReference":"X8899996", 
                            "acquirerResponseCode":"A01", 
                            "settleDueDate":"", 
                            "settleStatus":"", 
                            "acquirerResponseMessage":"APPROVED", 
                            "message":"Ok", 
                            "errorCode":"0", 
                            "liveStatus":"true", 
                            "merchantName":"Wipro Test Account", 
                            "merchantNumber":"00000000"                     
                        } 
                        }, 
                            { 
                        "source":"atmVerify", 
                        "details": 
                        { 
                            "requestReference":"X9614674", 
                            "orderReference":"578954", 
                            "settleDueDate":"", 
                            "settleStatus":"0", 
                            "message":"Ok", 
                            "errorCode":"0", 
                            "authCode":"TEST"  
                        } 
                    }
                ]	
 
            }
        }
        */

        var mJson =
        "{"
            + "\"recordStatus\":\"Created\","
            + "\"returnCode\":\"INFO_PFO_00023\","
            + "\"returnMsg\":\"Request processed successfully(loopback - SubmitDepositAccountRequest).\","
            + "\"depositDetails\":"
            + "{"
                + "\"operatorId\":\"" + mOperatorId + "\","
                + "\"siteId\":\"" + mSiteId + "\","
                + "\"authCode\":\"TEST\","
                + "\"accountType\":\"" + mAccountType + "\","
                + "\"amount\":\"" + mAmount + "\","
                + "\"currency\":\"" + mCurrency + "\","
                + "\"orderReference\":\"" + mOrderReference + "\","
                + "\"stTransactionNo\":\"1000574129\","
                + "\"transactionDate\":\"" + mMACUtils.getSTFormatedDate() + "\","
                + "\"transactionType\":\"" + mTransactionType + "\","
                + "\"userName\":\"" + mUserName + "\","
                + "\"accountDetails\":"
                + "{"
                    + "\"accountNumber\":\"51############01\","
                        + "\"accountToken\":\"dsfbksdfbksjfbgjsfg=\","
                        + "\"abaNumber\":\"11#####11\","
                        + "\"bankAccountType\":\"SAVINGS\""
                + "}"
            + "}"
        + "}";


                //    + "},"
                //    + "\"atmVerifyDetails\":"
                //    + "{"
                //        + "\"atmVerifyDesc\":\"P70:VALIDATED\","
                //        + "\"transactionType\":\"ACCOUNTCHECK\","
                //        + "\"atmVerifyResult\":\"PASS\""
                //     + "},"
                //    + "\"securityResponses\":"
                //    + "{"
                //        + "\"securityResponseCVV2\":\"2\","
                //        + "\"securityResponseFirstLineofAddress\":\"1\","
                //        + "\"securityResponsePostCode\":\"1\""
                //    + "},"
                //    + "\"additionalResponses\":"
                //    + "["
                //        + "{"
                //            + "\"source\":\"Gateway\","
                //            + "\"details\":"
                //            + "{"
                //                    + "\"requestReference\":\"X8899996\","
                //                    + "\"acquirerResponseCode\":\"A01\","
                //                    + "\"settleDueDate\":\"\","
                //                    + "\"settleStatus\":\"\","
                //                    + "\"acquirerResponseMessage\":\"APPROVED\","
                //                    + "\"message\":\"Ok\","
                //                    + "\"errorCode\":\"0\","
                //                    + "\"liveStatus\":\"true\","
                //                    + "\"merchantName\":\"Wipro Test Account\","
                //                    + "\"merchantNumber\":\"00000000\""
                //            + "}"
                //        + "},"
                //        + "{"
                //            + "\"source\":\"atmVerify\","
                //            + "\"details\":"
                //            + "{"
                //                + "\"requestReference\":\"X9614674\","
                //                + "\"orderReference\":\"578954\","
                //                + "\"settleDueDate\":\"\","
                //                + "\"settleStatus\":\"0\","
                //                + "\"message\":\"Ok\","
                //                + "\"errorCode\":\"0\","
                //                + "\"authCode\":\"TEST\""
                //            + "}"
                //        + "}"

                //    + "]"
                //+ "}"
        //    + "}"
        //+ "}";
        return mJson;
    }
    private string stapiSubmitDepositAccountRequestTestCase2(string pJson, String pState)
    {
        var mJson =
        "{"
        + "\"returnCode\":\"ERR_PFO_00001\","
        + "\"returnMsg\":\"Validation Failure - Field validation failure(loopback - SubmitDepositAccountRequest).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
        + "\"recordStatus\":\"Failed\","
        + "\"validationErrors\":"
            + "["
                + "{"
                + "\"validationError\":\"Invalid operatorId.\","
                + "\"validationField\":\"operatorId\""
                + "}"
            + "]"
        + "}";
        return mJson;
    }

    private string stapiAccountWithdrawalTestCase1(string pJson, String pState)
    {
        /* input json
        { 
            "operatorId":"000", 
            "siteId":"000", 
            "siteUsername":"457985621458", 
            "sitePwd":"(*^*%^$#", 
            "withdrawalDetails":  
            { 
                "userName":"johnsmith1", 
                "transactionType":"WITHDRAWAL", 
                "orderReference":"3213444", 
                "accountToken":"7DHV_UZt3v30RXZXkv2D_e87y0cppadz_wBnM0=",
                "accountType":"ACH", 
                "currency":"USD", 
                "amount":"1000" 
            } 
        }
         */  
        dynamic mJsonData = JObject.Parse(pJson);
        string mOperatorId = mJsonData.operatorId.ToString();
        string mSiteId = mJsonData.siteId.ToString();
        string mUserName = mJsonData.withdrawalDetails.userName.ToString();
        string mTransactionType = mJsonData.withdrawalDetails.transactionType.ToString();
        string mAmount = mJsonData.withdrawalDetails.amount.ToString();
        string mCurrency = mJsonData.withdrawalDetails.currency.ToString();
        string mOrderReference = mJsonData.withdrawalDetails.orderReference.ToString();
        string mAccountType = mJsonData.withdrawalDetails.accountType.ToString();

        var mJson =
        "{"
        + "\"recordStatus\":\"Created\","
        + "\"returnCode\":\"INFO_PFO_00023\","
        + "\"returnMsg\":\"Request processed successfully(loopback - AccountWithdrawal).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
        + "\"withdrawalDetails\":"
            + "{"
            + "\"accountType\":\"" + mAccountType + "\","
            + "\"authCode\":\"TEST\","
            + "\"amount\":\"" + mAmount + "\","
            + "\"currency\":\"" + mCurrency + "\","
            + "\"operatorId\":\"" + mOperatorId + "\","
            + "\"orderReference\":\"" + mOrderReference + "\","
            + "\"siteId\":\"" + mSiteId + "\","
            + "\"stTransactionNo\":\"1000574130\","
            + "\"transactionDate\":\"2014-06-05T16:26:48.144+05:30\","
            + "\"transactionType\":\"" + mTransactionType + "\","
            + "\"userName\":\"" + mUserName + "\","
            + "\"accountDetails\":"
            + "{"
                + "\"accountNumber\":\"51############01\","
                + "\"accountToken\":\"fsfkgbgbdgjkbdfkgjbkdjfbgldfg=\","
                + "\"abaNumber\":\"11#####11\","
                + "\"bankAccountType\":\"SAVINGS\""
            + "},"
            + "\"securityResponses\":"
            + "{"
                + "\"securityResponseCVV2\":\"2\","
                + "\"securityResponseFirstLineofAddress\":\"1\","
                + "\"securityResponsePostCode\":\"1\""
            + "},"
            + "\"additionalResponses\":"
                + "["
                    + "{"
                    + "\"source\":\"Gateway\","
                    + "\"details\":"
                        + "{"
                            + "\"requestReference\":\"X8899996\","
                            + "\"acquirerResponseCode\":\"A01\","
                            + "\"acquirerResponseMessage\":\"APPROVED\","
                            + "\"settleDueDate\":\"\","
                            + "\"settleStatus\":\"\","
                            + "\"dccEnabled\":\"0\","
                            + "\"message\":\"Ok\","
                            + "\"liveStatus\":\"true\","
                            + "\"errorCode\":\"0\","
                            + "\"merchantName\":\"Wipro Test Account\","
                            + "\"merchantNumber\":\"00000000\""
                        + "}"
                    + "}"
                + "]"
            + "}"
        + "}";
        return mJson;
    }
    private string stapiAccountWithdrawalTestCase2(string pJson, String pState)
    {
        var mJson =
        "{"
        + "\"returnCode\":\"ERR_PFO_00001\","
        + "\"returnMsg\":\"Validation Failure - Field validation failure(loopback - AccountWithdrawal).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
        + "\"recordStatus\":\"Failed\","
        + "\"validationErrors\":"
            + "["
                + "{"
                + "\"validationError\":\"Invalid siteUsername.\","
                + "\"validationField\":\"siteUsername\""
                + "}"
            + "]"
        + "}";
        return mJson;
    }

    private string stapiWithdrawalTestCase1(string pJson, String pState)
    {
        dynamic mJsonData = JObject.Parse(pJson);
        string mOperatorId = mJsonData.operatorId.ToString();
        string mSiteId = mJsonData.siteId.ToString();
        string mUserName = mJsonData.withdrawalDetails.userName.ToString();
        string mTransactionType = mJsonData.withdrawalDetails.transactionType.ToString();
        string mAmount = mJsonData.withdrawalDetails.amount.ToString();
        string mCurrency = mJsonData.withdrawalDetails.currency.ToString();
        string mOrderReference = mJsonData.withdrawalDetails.orderReference.ToString();
        string mAccountType = mJsonData.withdrawalDetails.accountType.ToString();

        var mJson =
        "{"
        + "\"recordStatus\":\"Created\","
        + "\"returnCode\":\"INFO_PFO_00023\","
        + "\"returnMsg\":\"Request processed successfully(loopback - Withdrawal).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
        + "\"withdrawalDetails\":"
        + "{"
            + "\"operatorId\":\"" + mOperatorId + "\","
            + "\"siteId\":\"" + mSiteId + "\","
            + "\"userName\":\"" + mUserName + "\","
            + "\"transactionType\":\"" + mTransactionType + "\","
            + "\"amount\":\"" + mAmount + "\","
            + "\"currency\":\"" + mCurrency + "\","
            + "\"authCode\":\"TEST REFUND ACCEPTED\","
            + "\"orderReference\":\"" + mOrderReference + "\","
            + "\"transactionDate\":\"" + mMACUtils.getSTFormatedDate() + "\","
            + "\"stTransactionNo\":\"1000574052\","
            + "\"accountType\":\"" + mAccountType + "\","
            + "\"cardDetails\":"
            + "{"
                + "\"cardNumber\":\"411111######0211\","
                + "\"cardToken\":\"fsfkgbgbdgjkbdfkgjbkdjfbgldfg=\","
                + "\"cardType\":\"VISA\""
            + "},"
            + "\"securityResponses\":"
            + "{"
                + "\"securityResponseCVV2\":\"2\","
                + "\"securityResponseFirstLineofAddress\":\"1\","
                + "\"securityResponsePostCode\":\"1\""
            + "},"
            + "\"additionalResponses\":"
                + "["
                    + "{"
                    + "\"source\":\"Gateway\","
                    + "\"details\":"
                        + "{"
                            + "\"tid\":\"27882788\","
                            + "\"requestReference\":\"X8899996\","
                            + "\"acquirerResponseCode\":\"00\","
                            + "\"settleDueDate\":\"\","
                            + "\"settleStatus\":\"\","
                            + "\"dccEnabled\":\"0\","
                            + "\"liveStatus\":\"false\","
                            + "\"message\":\"Ok\","
                            + "\"errorCode\":\"0\","
                            + "\"issuer\":\"Secure Trading Test Issuer1\","
                            + "\"issuerCountry\":\"US\","
                            + "\"merchantCountryiso2a\":\"GB\","
                            + "\"merchantName\":\"Wipro Test Account\","
                            + "\"merchantNumber\":\"00000000\""
                        + "}"
                    + "}"
                + "]"
            + "}"
        + "}";
        return mJson;
    }
    private string stapiWithdrawalTestCase2(string pJson, String pState)
    {
        var mJson =
        "{"
        + "\"returnCode\":\"ERR_PFO_00001\","
        + "\"returnMsg\":\"Validation Failure - Field validation failure(loopback - Withdrawal).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
        + "\"recordStatus\":\"Failed\","
        + "\"validationErrors\":"
            + "["
                + "{"
                + "\"validationError\":\"Invalid operatorId.\","
                + "\"validationField\":\"operatorId\""
                + "}"
            + "]"
        + "}";
        return mJson;
    }

    private string stapiAuthDepositRequestTestCase1(string pJson, String pState)
    {
        dynamic mJsonData = JObject.Parse(pJson);
        string mOperatorId = mJsonData.operatorId.ToString();
        string mSiteId = mJsonData.siteId.ToString();
        // todo get player document 

        var mJson =
        "{"
        + "\"recordStatus\":\"Created\","
        + "\"returnCode\":\"INFO_PFO_00023\","
        + "\"returnMsg\":\"Request processed successfully(loopback - AuthDepositRequest).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
        + "\"depositDetails\":"
            + "{"
            + "\"operatorId\":\"" + mOperatorId + "\","
            + "\"siteId\":\"" + mSiteId + "\","
            + "\"accountType\":\"Ecom\","
            + "\"amount\":\"5000\","
            + "\"authCode\":\"TEST\","
            + "\"currency\":\"USD\","
            + "\"stTransactionNo\":\"0000000000\","
            + "\"transactionDate\":\"2014-06-04T16:25:51.149+05:30\","
            + "\"transactionType\":\"DEPOSIT\","
            + "\"userName\":\"Johnsmith1\","
            + "\"orderReference\":\"1234\","
            + "\"cardDetails\":"
                + "{"
                + "\"cardNumber\":\"411111######0211\","
                + "\"cardToken\":\"fsfkgbgbdgjkbdfkgjbkdjfbgldfg=\","
                + "\"cardType\":\"VISA\""
                + "},"
                + "\"threeDSecureDetails\":"
                + "{"
                + "\"eciFlag\":\"05\","
                + "\"enrolled\":\"Y\","
                + "\"paresStatus\":\"Y\""
                + "},"
                + "\"securityResponses\":"
                + "{"
                + "\"securityResponseCVV2\":\"2\","
                + "\"securityResponseFirstLineofAddress\":\"1\","
                + "\"securityResponsePostCode\":\"1\""
                + "},"
                + "\"additionalResponses\":"
                    + "["
                        + "{"
                        + "\"source\":\"Gateway\","
                        + "\"details\":"
                            + "{"
                            + "\"tid\":\"27882788\","
                            + "\"requestReference\":\"X8899996\","
                            + "\"cavv\":\"Q0FWVkNBVlZDQVZWQ0FWVkNBVlY=\","
                            + "\"xid\":\"UVBSNjNhUU5JSXZmazRTY2I3MGY=\","
                            + "\"acquirerResponseCode\":\"00\","
                            + "\"settleDueDate\":\"\","
                            + "\"settleStatus\":\"\","
                            + "\"dccEnabled\":\"\","
                            + "\"message\":\"Ok\","
                            + "\"errorCode\":\"\","
                            + "\"issuer\":\"Secure Trading Test Issuer1\","
                            + "\"issuerCountry\":\"US\","
                            + "\"merchantCountryiso2a\":\"GB\","
                            + "\"liveStatus\":\"false\","
                            + "\"merchantName\":\"Wipro Test Account\","
                            + "\"merchantNumber\":\"00000000\""
                            + "}"
                        + "},"
                        + "{"
                        + "\"source\":\" threeDSecure\","
                        + "\"details\":"
                            + "{"
                            + "\"signVerification\":\"Y\","
                            + "\"threeDTransactionType\":\"C\""
                            + "}"
                        + "}"
                    + "]"
            + "}"
        + "}";
        return mJson;
    }
    private string stapiAuthDepositRequestTestCase2(string pJson, String pState)
    {
        var mJson =
            "{"
            + "\"returnCode\":\"ERR_PFO_00001\","
            + "\"returnMsg\":\"Validation Failure - Field validation failure(loopback - AuthDepositRequest).\","
            + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
            + "\"recordStatus\":\"Failed\","
            + "\"validationErrors\":"
                + "["
                    + "{"
                    + "\"validationError\":\"Invalid operatorId.\","
                    + "\"validationField\":\"operatorId\""
                    + "}"
                + "]"
            + "}";
        return mJson;
    }

    private string stapiSubmitDepositRequestTestCase1(string pJson, String pState)
    {
        dynamic mJsonData = JObject.Parse(pJson);
        string mOperatorId = mJsonData.depositDetails.operatorId.ToString();
        string mSiteId = mJsonData.depositDetails.siteId.ToString();

        string mUserName = mJsonData.depositDetails.userName.ToString();
        string mTransactionType = mJsonData.depositDetails.transactionType.ToString();
        string mAmount = mJsonData.depositDetails.amount.ToString();
        string mCurrency = mJsonData.depositDetails.currency.ToString();
        string mOrderReference = mJsonData.depositDetails.orderReference.ToString();
        string mStTransactionNo = mJsonData.depositDetails.stTransactionNo.ToString();
        string mAccountType = mJsonData.depositDetails.accountType.ToString();

        var mJson =
        "{"
        + "\"returnCode\":\"INFO_PFO_00026\","
        + "\"returnMsg\":\"The card is enrolled for 3-D Secure(loopback - SubmitDepositRequest).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
        + "\"recordStatus\":\"Not Created\","
        + "\"depositDetails\":"
            + "{"
            + "\"operatorId\":\"" + mOperatorId + "\","
            + "\"siteId\":\"" + mSiteId + "\","
            + "\"userName\":\"" + mUserName + "\","
            + "\"transactionType\":\"" + mTransactionType + "\","
            + "\"stTransactionNo\":\"" + mStTransactionNo + "\","
            + "\"orderReference\":\"" + mOrderReference + "\","
            + "\"transactionDate\":\"" + mMACUtils.getSTFormatedDate() + "\","
            + "\"currency\":\"" + mCurrency + "\","
            + "\"amount\":\"" + mAmount + "\","
            + "\"accountType\":\"" + mAccountType + "\","
            + "\"threeDSecureDetails\":"
                + "{"
                + "\"threeDAuth\":\"Y\","                                   //todo: ??? - This attribute is not in the request.
                + "\"threeDTransactionType\":\"C\","                        //todo: ??? - This attribute is not in the request.
                + "\"acsurl\":\"https://www.example.com/service?parater=value\","
                + "\"eciFlag\":\"01\","                                     //todo: ??? - This attribute is not in the request.
                + "\"enrolled\":\"Y\","                                     //todo: ??? - This attribute is not in the request.
                + "\"md\":\"fgkfjhb4387bjhfgbjh\","                         //todo: ??? - This attribute is not in the request.
                + "\"pareq\":\"kfgbkgbiubfgib32489234jbkgfbfg743rjbfgdfg"               //todo: ??? - This attribute is not in the request.
                + "},"
                + "\"additionalResponses\":"
                + "["
                    + "{"
                    + "\"source\":\"Gateway\","
                    + "\"details\":"
                        + "{"
                        + "\"liveStatus\":\"false"
                        + "}"
                    + "}"
                + "]"
            + "}"
        + "}";
        return mJson;
    }
    private string stapiSubmitDepositRequestTestCase2(string pJson, String pState)
    {
        var mJson =
            "{"
            + "\"returnCode\":\"ERR_PFO_00001\","
            + "\"returnMsg\":\"Validation Failure - Field validation failure(loopback - SubmitDepositRequest).\","
            + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
            + "\"recordStatus\":\"Failed\","
            + "\"validationErrors\":"
                + "["
                    + "{"
                    + "\"validationError\":\"Invalid operatorId.\","
                    + "\"validationField\":\"operatorId\""
                    + "}"
                + "]"
            + "}";
        return mJson;
    }

    private string stapiRegisterPrePaidAccountTestCase1(string pJson, String pState)
    {
        dynamic mJsonData = JObject.Parse(pJson);
        string operatorId = mJsonData.operatorId.ToString();
        string siteId = mJsonData.siteId.ToString();
        string userName = mJsonData.userName.ToString();
        var mJson =
            "{"
            + "\"returnCode\":\"INFO_PFO_00042\","
            + "\"returnMsg\":\"Pre Paid Account registered successfully(loopback - RegisterPrePaidAccount).\","
            + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
            + "\"recordStatus\":\"Created\","
            + "\"prePaidAccountRegistrationDetails\":"
                + "{"
                    + "\"operatorId\":\"" + operatorId + "\","
                    + "\"siteId\":\"" + siteId + "\","
                    + "\"userName\":\"" + userName + "\","
                    + "\"prePaidAccountNumber\":\"" + Create_prePaidAccountNumber() + "\","
                    + "\"prePaidAccountToken\":\"" + Create_prePaidAccountToken(pState) + "\","
                    + "\"virtualCardNumber\":\"" + Create_virtualCardNumber() + "\","
                    + "\"idCheckStatus\":\"" + "PASSED" + "\","
                    + "\"supportPhoneNumber\":\"" + "1234567895" + "\","
                    + "\"expireDate\":\"" + "12/2016" + "\","
                    + "\"receiptHtml\":\"" + "<div>Receipt</div>" + "\""
                + "}"
            + "}";

        return mJson;
    }
    private string stapiRegisterPrePaidAccountTestCase2(string pJson, String pState)
    {
        /*
         * { "responseToken":"JK_RhLhyzWCxAwd7h17jXOohkTwTyzC1TCDgiqHAhrQ=", "returnCode":"INFO_PFO_00043", "returnMsg":"Please answer the identity verification questions.", "stTimeStamp":"2014-06-20T12:13:51.808+05:30", "identityCheckDetails": { "identityCheckID":"000000000000", "questions": [ { "questionPrompt":"<question>", "questionType":"<questiontype>", "answers": [ "<answer>", "<answer>", "<answer>", "<answer>" ] }, { "questionPrompt":"<question>", "questionType":"<questiontype>", "answers": [ "<answer>", "<answer>", "<answer>", "<answer>" ] }, { "questionPrompt":"<question>", "questionType":"time.at.current.address", "answers": [ "<answer>", "<answer>", "<answer>", "<answer>", "<answer>", "<answer>" ] }, { "questionPrompt":"<question>", "questionType":"<questiontype>", "answers": [ "<answer>", "<answer>", "<answer>", "<answer>" ] } ] } }
         * */

        var mJson =
            "{"
            + "\"responseToken\":\"JK_RhLhyzWCxAwd7h17jXOohkTwTyzC1TCDgiqHAhrQ=\","
            + "\"returnCode\":\"INFO_PFO_00043\","
            + "\"returnMsg\":\"Please answer the identity verification questions.\","
            + "\"stTimeStamp\":\"2014-06-20T12:13:51.808+05:30\","
            + "\"identityCheckDetails\":"
                + "{"
                + "\"identityCheckID\":\"000000000000\","
                + "\"questions\":"
                    + "["
                        + "{"
                        + "\"questionPrompt\":\"<question>\","
                        + "\"questionType\":\"<questiontype>\","
                        + "\"answers\":"
                            + "["
                            + "\"<answer>\","
                            + "\"<answer>\","
                            + "\"<answer>\","
                            + "\"<answer>"
                            + "]"
                        + "},"
                        + "{"
                        + "\"questionPrompt\":\"<question>\","
                        + "\"questionType\":\"<questiontype>\","
                        + "\"answers\":"
                            + "["
                            + "\"<answer>\","
                            + "\"<answer>\","
                            + "\"<answer>\","
                            + "\"<answer>"
                            + "]"
                        + "},"
                        + "{"
                        + "\"questionPrompt\":\"<question>\","
                        + "\"questionType\":\"time.at.current.address\","
                        + "\"answers\":"
                            + "[ "
                            + "\"<answer>\","
                            + "\"<answer>\","
                            + "\"<answer>\","
                            + "\"<answer>\","
                            + "\"<answer>\","
                            + "\"<answer>"
                            + "]"
                        + "},"
                        + "{"
                        + "\"questionPrompt\":\"<question>\","
                        + "\"questionType\":\"<questiontype>\","
                        + "\"answers\":"
                            + "["
                            + "\"<answer>\","
                            + "\"<answer>\","
                            + "\"<answer>\","
                            + "\"<answer>"
                            + "]"
                        + "}"
                    + "]"
                + "}"
            + "}";
        return mJson;
    }
    private string stapiRegisterPrePaidAccountTestCase3(string pJson, string pStateId)
    {
        var mJson =
        "{"
        + "\"returnCode\":\"ERR_PFO_00001\","
        + "\"returnMsg\":\"Validation Failure - Field validation failure(loopback - RegisterPrePaidAccount).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
        + "\"recordStatus\":\"Failed\","
        + "\"validationErrors\":"
            + "["
                + "{"
                + "\"validationError\":\"Invalid OperatorId.\","
                + "\"validationField\":\"OperatorId\""
                + "}"
            + "]"
        + "}";
        return mJson;
    }

    private string stapiSubmitReversalTestCase1(string pJson, String pState)
    {
        /* input json
        { 
            "operatorId":"000", 
            "siteId":"000", 
            "siteUsername":"123456789545", 
            "sitePwd":")*(&^%$#", 
            "reversalDetails": 
            { 
                "userName":"johnsmith1", 
                "transactionType":"REVERSAL", 
                "depositSTTransactionNo":"1234567890", 
                "accountType":"ECOM", 
                "currency":"USD", 
                "amount":"1000"  
            } 
        } 
        */

        /*
        { 
            "returnCode":"INFO_PFO_00023", 
            "returnMsg":"Request processed successfully", 
            "stTimeStamp":"2014-06-05T13:19:27.099+05:30", 
            "recordStatus":"Created", 
            "reversalDetails": 
            { 
                "operatorId":"554", 
                "siteId":"593", 
                "userName":"Emith123", 
                "transactionType":"REVERSAL", 
                "amount":"100004", 
                "currency":"USD", 
                "authCode":"TEST REFUND ACCEPTED", 
                "orderReference":"1234", 
                "transactionDate":"2014-06-05T13:20:01.850+05:30", 
                "stTransactionNo":"1000574052",
                "accountType":"ECOM",
                "cardDetails": 
                { 
                    "cardNumber":"411111######0211", 
                    "cardToken":"fsfkgbgbdgjkbdfkgjbkdjfbgldfg=", 
                    "cardType":"VISA" 
                }, 
                "securityResponses": 
                { 
                        "securityResponseCVV2":"2", 
                        "securityResponseFirstLineofAddress":"1" 
                }, 
                "additionalResponses": 
                [ 
                    { 
                        "source":"Gateway",
                        "details": 
                        { 
                            "liveStatus":"false" 	 
                        } 
                    } 
                ] 
            } 
        } 
        */
        dynamic mJsonData = JObject.Parse(pJson);
        string operatorId = mJsonData.operatorId.ToString();
        string siteId = mJsonData.siteId.ToString();
        string userName = mJsonData.reversalDetails.userName.ToString();

        var mJson =
        "{"
            + "\"returnCode\":\"INFO_PFO_00023\","
            + "\"returnMsg\":\"Request processed successfully\","
            + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
            + "\"recordStatus\":\"Created\","
            + "\"reversalDetails\":"
            + "{"
                + "\"operatorId\":\"" + operatorId + "\","
                + "\"siteId\":\"" + siteId + "\","
                + "\"userName\":\"" + userName + "\","
                + "\"transactionType\":\"REVERSAL\","
                + "\"amount\":\"1000\"," 
                + "\"currency\":\"USD\"," 
                + "\"authCode\":\"TEST REFUND ACCEPTED\","
                + "\"orderReference\":\"1234\"," 
                + "\"transactionDate\":\"2014-06-05T13:20:01.850+05:30\"," 
                + "\"stTransactionNo\":\"1000574052\","
                + "\"accountType\":\"ECOM\""
            + "},"
            + "\"cardDetails\":"
            + "{"
                + "\"cardNumber\":\"411111######0211\","
                + "\"cardToken\":\"fsfkgbgbdgjkbdfkgjbkdjfbgldfg=\","
                + "\"cardType\":\"VISA\""
            + "},"
            + "\"securityResponses\":" 
            + "{"
                + "\"securityResponseCVV2\":\"2\","
                + "\"securityResponseFirstLineofAddress\":\"1\""
            + "},"
            + "\"additionalResponses\":"
            + "["
                + "{" 
                    + "\"source\":\"Gateway\","
                    + "\"details\":"
                    + "{" 
                        + "\"liveStatus\":\"false\""
                    + "}"
                + "}"
            + "]"
        + "}";
        return mJson;
    }
    private string stapiSubmitReversalTestCase2(string pJson, String pState)
    {
        /*
        { 
        "returnCode":"ERR_PFO_00178", 
        "returnMsg":"Site authentication failure.", 
        "stTimeStamp":"2000-12-31T23:59:59.999+01:00" 
        } 
        */
        var mJson =
            "{"
            + "\"returnCode\":\"ERR_PFO_00178\", "
            + "\"returnMsg\":\"Site authentication failure.\","
            + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\""
            + "}";
        return mJson;
    }

    private string stapiPrePaidRegisterAndLoadTestCase1(string pJson, String pState)
    {
        /*
        { 
            "returnCode": "INFO_PFO_00023", 
            "returnMsg": "Request processed successfully.", 
            "stTimeStamp": "2014-06-05T13:44:44.902+05:30", 
            "prePaidAccountRegistrationResponse":  
            { 
                "returnCode":"INFO_PFO_00042",
                "returnMsg":"Pre Paid Account registered successfully.", 
                "stTimeStamp":"2012-08-13T11:54:21.804+05:30", 
                "recordStatus":"Created", 
                "prePaidAccountRegistrationDetails": 
                { 
                    "operatorId":"000", 
                    "siteId":"000", 
                    "userName":"Johnsmith1", 
                    "prePaidAccountNumber":"51############01",
                    "prePaidAccountToken":"IpFlsWGtfoP379vDAVsMig=", 
                    "virtualCardNumber":"0000000000000000", 
                    "idCheckStatus":"PASSED", 
                    "supportPhoneNumber":"1234567895", 
                    "expiryDate":"mm/yyyy", 
                    "receiptHtml":"<value>" 
                } 
            }, 
            "prePaidAccountLoadResponse":  
            { 
                "returnCode":"INFO_PFO_00023", 
                "returnMsg":"Request processed successfully.", 
                "stTimeStamp":"2000-12-31T23:59:59.999+01:00",
                "prePaidAccountLoadingDetails": 
                { 
                    "operatorId":"000", 
                    "siteId":"000", 
                    "userName":"johnsmith1", 
                    "prePaidAccountNumber":"51############01", 
                    "prePaidAccountToken": "djkfsfjgkbfg=", 
                    "prePaidAccountBalance":"10000", 
                    "loadAmount":"1000", 
                    "currency": "USD", 
                    "receiptHtml":"<value>" 
                } 
            } 	
        } 	
        */
        dynamic mJsonData = JObject.Parse(pJson);
        string operatorId = mJsonData.operatorId.ToString();
        string siteId = mJsonData.siteId.ToString();
        string userName = mJsonData.userName.ToString();

        var mJson =
        "{"
            + "\"returnCode\":\"INFO_PFO_00023\","
            + "\"returnMsg\":\"Request processed successfully\","
            + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\""
            + "\"prePaidAccountRegistrationResponse\":"
            + "{"
            + "\"returnCode\":\"INFO_PFO_00042\","
            + "\"returnMsg\":\"Pre Paid Account registered successfully.\"," 
            + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\""
            + "\"recordStatus\":\"Created\","
            + "\"prePaidAccountRegistrationDetails\":"
            + "{"
                + "\"operatorId\":\"" + operatorId + "\","
                + "\"siteId\":\"" + siteId + "\","
                + "\"userName\":\"" + userName + "\","
                + "\"prePaidAccountNumber\":\"51############01\","
                + "\"prePaidAccountToken\":\"IpFlsWGtfoP379vDAVsMig=\"," 
                + "\"virtualCardNumber\":\"0000000000000000\"," 
                + "\"idCheckStatus\":\"PASSED\","
                + "\"supportPhoneNumber\":\"1234567895\"," 
                + "\"expiryDate\":\"mm/yyyy\"," 
                + "\"receiptHtml\":\"<value>\""
            + "},"
            + "\"prePaidAccountLoadResponse\":" 
            + "{"
                + "\"returnCode\":\"INFO_PFO_00023\","  
                + "\"returnMsg\":\"Request processed successfully.\"," 
                + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\""
                + "\"prePaidAccountLoadingDetails\":"
                + "{" 
                + "\"operatorId\":\"" + operatorId + "\","
                + "\"siteId\":\"" + siteId + "\","
                + "\"userName\":\"" + userName + "\"," 
                + "\"prePaidAccountNumber\":\"51############01\"," 
                + "\"prePaidAccountToken\":\"djkfsfjgkbfg=\"," 
                + "\"prePaidAccountBalance\":\"10000\"," 
                + "\"loadAmount\":\"1000\"," 
                + "\"currency\":\"USD\","
                + "\"receiptHtml\":\"<value>\""
                + "}"
            + "}"	
        + "}";
        return mJson;
    }
    private string stapiPrePaidRegisterAndLoadTestCase2(string pJson, String pState)
    {
        dynamic mJsonData = JObject.Parse(pJson);
        string operatorId = mJsonData.operatorId.ToString();
        string siteId = mJsonData.siteId.ToString();
        /*
        { 
        "recordStatus":"Failed", 
        "returnCode":"ERR_PFO_00306", 
        "returnMsg":"Cannot Register Pre Paid Account", 
            "stTimeStamp":"2014-06-19T13:49:26.331+05:30", 
            "prePaidAccountDetails": 
 	        { 
 	            "idCheckStatus":"HARD FAIL", 
 	            "operatorId":"698", 
 	            "siteId":"783" 
 	        } 
        } 
        */
        var mJson =
            "{"
                + "\"recordStatus\":\"Failed\","
                + "\"returnCode\":\"ERR_PFO_00306\","
                + "\"returnMsg\":\"Cannot Register Pre Paid Account\","
                + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\""
                + "\"prePaidAccountDetails\":"
                + "{"
                    + "\"idCheckStatus\":\"HARD FAIL\","
                    + "\"operatorId\":\"" + operatorId + "\","
                    + "\"siteId\":\"" + siteId + "\""
                + "}"
            + "}";
        return mJson;
    }

    private string stapiUpdatePrePaidAccountStatusTestCase1(string pJson, String pState)
    {
        /* input json
            { 
                "operatorId":"000", 
                "siteId":"000", 
                "siteUsername":"123789456245", 
                "sitePwd":")(*&^%",
                "playerDetails": 
 	            { 
 	                "userName":"johnsmith1", 
 	                "connectionToken":"6789gfgdfhgdfgdfghhtr4", 
 	                "sessionToken":"200446",  	
                    "prePaidAccountDetails": 
 	 	            { 
 	 	                "prePaidAccountToken":"IpFlmsW9vWXpH3DAVsMi-g=", 
 	 	                "status":"Active"      
 	 	            } 
 	            } 
            } 

         */

        
        dynamic mJsonData = JObject.Parse(pJson);
        string operatorId = mJsonData.operatorId.ToString();
        string siteId = mJsonData.siteId.ToString();
        /*
        { 
            "returnCode":"INFO_PFO_00023", 
            "returnMsg":"Request processed successfully.", 
            "stTimeStamp":"2012-08-13T11:54:21.804+05:30", 
            "recordStatus":"Updated", 
            "prePaidAccountDetails ": 
 	        { 
 	            "operatorId":"000", 
 	            "siteId":"000", 
 	            "prePaidAccountToken ":"IpFlmsWvWXpH3DAVsMi-g=", 
 	            "status":"Active" 
 	        } 
        } 
        */
        var mJson =
        "{"
            + "\"returnCode\":\"INFO_PFO_00023\","
            + "\"returnMsg\":\"Request processed successfully.\","
            + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
            + "\"recordStatus\":\"Updated\","
            + "\"prePaidAccountDetails\":"
            + "{"
                + "\"operatorId\":\"" + operatorId + "\","
                + "\"siteId\":\"" + siteId + "\","
                + "\"prePaidAccountToken\":\"IpFlmsWvWXpH3DAVsMi-g=\","
                + "\"status\":\"Active\""
            + "}"
        + "}";
        return mJson;
    }
    private string stapiUpdatePrePaidAccountStatusTestCase2(string pJson, String pState)
    {
        var mJson =
        "{"
        + "\"returnCode\":\"ERR_PFO_00001\","
        + "\"returnMsg\":\"Validation Failure - Field validation failure(loopback - GetPrePaidAccountCVV2).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
        + "\"recordStatus\":\"Failed\","
        + "\"validationErrors\":"
            + "["
                + "{"
                + "\"validationField\":\"siteUsername\","
                + "\"validationError\":\"Invalid siteUsername.\""
                + "}"
            + "]"
        + "}";
        return mJson;
    }

    private string stapiGetPrePaidAccountCVV2TestCase1(string pJson, String pState)
    {
        dynamic mJsonData = JObject.Parse(pJson);
        string mOperatorId = mJsonData.operatorId.ToString();
        string mSiteId = mJsonData.siteId.ToString();

        var mJson =
        "{"
        + "\"returnCode\":\"INFO_PFO_00023\","
        + "\"returnMsg\":\"Request processed successfully(loopback - GetPrePaidAccountCVV2).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
        + "\"prePaidAccountDetails\":"
            + "{"
            + "\"operatorId\":\"" + mOperatorId + "\","
            + "\"siteId\":\"" + mSiteId + "\","
            + "\"virtualCardNumber\":\"000000######0000\","
            + "\"CVV2\":\"121\""
            + "}"
        + "}";
        return mJson;
    }
    private string stapiGetPrePaidAccountCVV2TestCase2(string pJson, String pState)
    {
        var mJson =
        "{"
        + "\"returnCode\":\"ERR_PFO_00001\","
        + "\"returnMsg\":\"Validation Failure - Field validation failure(loopback - GetPrePaidAccountCVV2).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
        + "\"recordStatus\":\"Failed\","
        + "\"validationErrors\":"
            + "["
                + "{"
                + "\"validationField\":\"siteUsername\","
                + "\"validationError\":\"Invalid siteUsername.\""
                + "}"
            + "]"
        + "}";
        return mJson;
    }

    private string stapiPreCheckSiteModifyAccountRequestTestCase1(string pJson, String pState)
    {
        dynamic mJsonData = JObject.Parse(pJson);
        string mOperatorId = mJsonData.operatorId.ToString();
        string mSiteId = mJsonData.siteId.ToString();

        var mJson =
        "{"
            + "\"returnCode\":\"INFO_PFO_00035\","
            + "\"returnMsg\":\"Account successfully deleted(loopback - PreCheckModifyAccountRequest).\","
            + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
            + "\"recordStatus\":\"Deleted\","
            + "\"accountDetails\":"
            + "{"
            + "\"operatorId\":\"" + mOperatorId + "\","
            + "\"siteId\":\"" + mSiteId + "\","
            + "\"accountToken\":\"IpFlmsWGtfoP379vWXpH3DAVsMi-g=\""
            + "}"
        + "}";
        return mJson;
    }
    private string stapiPreCheckSiteModifyAccountRequestTestCase2(string pJson, String pState)
    {
        var mJson =
        "{"
        + "\"returnCode\":\"ERR_PFO_00178\","
        + "\"returnMsg\":\"Site authentication failure(loopback - PreCheckModifyAccountRequest).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\""
        + "}";
        return mJson;
    }

    private string stapiSubmitPrePaidIdentityAnswersTestCase1(string pJson, String pState)
    {
        dynamic mJsonData = JObject.Parse(pJson);
        string mOperatorId = mJsonData.operatorId.ToString();
        string mSiteId = mJsonData.siteId.ToString();

        var mJson =
        "{"
            + "\"returnCode\":\"INFO_PFO_00035\","
            + "\"returnMsg\":\"Account successfully deleted(loopback - PreCheckModifyAccountRequest).\","
            + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
            + "\"recordStatus\":\"Deleted\","
            + "\"accountDetails\":"
            + "{"
            + "\"operatorId\":\"" + mOperatorId + "\","
            + "\"siteId\":\"" + mSiteId + "\","
            + "\"accountToken\":\"IpFlmsWGtfoP379vWXpH3DAVsMi-g=\""
            + "}"
        + "}";
        return mJson;
    }

    private string stapiPreCheckSiteAccountRequestTestCase1(string pJson, String pState)
    {
        /*
         * { "operatorId":"000", "siteId":"000", "siteUsername":"123789456245", "sitePwd":")(*&^%", "userName":"johnsmith1", "accountType":"ACH", "connectionToken":"65325bd778523e429b7cdd965f092bba916a0ca1", "sessionToken":"200446", "atmVerify":"Y", "accountDetails": { "bankName":"FIRSTBANK", "bankAccountType":"SAVINGS", "accountNumber":"12121212", "abaNumber":"111111111" } }
         * */

        dynamic mJsonData = JObject.Parse(pJson);
        string mOperatorId = mJsonData.operatorId.ToString();
        string mSiteId = mJsonData.siteId.ToString();
        string mAccountType = mJsonData.accountType.ToString();

        var mJson =
        "{"
        + "\"returnCode\":\"INFO_PFO_00033\","
        + "\"returnMsg\":\"Account successfully registered(loopback - PreCheckAccountRequest).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
        + "\"recordStatus\":\"Created\","
        + "\"accountRegistrationDetails\":"
            + "{"
                + "\"operatorId\":\"" + mOperatorId + "\","
                + "\"siteId\":\"" + mSiteId + "\","
                + "\"accountType\":\"" + mAccountType + "\","
                + "\"authCode\":\"TEST\""
            + "},"
            + "\"accountDetails\":"
            + "{"
                + "\"accountNumber\":\"51############01\","
                + "\"abaNumber\":\"11#####11\","
                + "\"accountToken\":\"IpFlmsWGtfoP379vWkHv_9FQtH3DAVsMi-\","
                + "\"bankAccountType\":\"SAVINGS\""
            + "},"
            + "\"atmVerifyDetails\":"
            + "{"
                + "\"atmVerifyDesc\":\"P70:VALIDATED\","
                + "\"transactionType\":\"ACCOUNTCHECK\","
                + "\"atmVerifyResult\":\"PASS\""
            + "},"
            + "\"securityResponses\":"
            + "{"
                + "\"securityResponseCVV2\":\"2\","
                + "\"securityResponseFirstLineofAddress\":\"1\""
            + "},"
            + "\"additionalResponses\":"
                + "["
                    + "{"
                    + "\"source\":\"atmVerify\","
                    + "\"details\":"
                        + "{"
                        + "\"orderReference\":\"578954\","
                        + "\"requestReference\":\"X9614674\","
                        + "\"merchantName\":\"Wipro Test Account\","
                        + "\"settleDueDate\":\"\","
                        + "\"liveStatus\":\"false\","
                        + "\"settleStatus\":\"0\","
                        + "\"message\":\"Ok\","
                        + "\"errorCode\":\"0\""
                        + "}"
                    + "}"
                + "]"
        + "}";
        return mJson;
    }
    private string stapiPreCheckSiteAccountRequestTestCase2(string pJson, String pState)
    {
        var mJson =
        "{"
        + "\"returnCode\":\"ERR_PFO_00178\","
        + "\"returnMsg\":\"Site authentication failure(loopback - PreCheckAccountRequest).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\""
        + "}";
        return mJson;
    }

    

    private string stapiSubmitModifiedPlayerKBATestCase1(string pJson, String pState)
    {
        dynamic mJsonData = JObject.Parse(pJson);
        string operatorId = mJsonData.operatorId.ToString();
        string siteId = mJsonData.siteId.ToString();
        string userName = mJsonData.siteId.ToString();

        var mJson =
        "{"
        + "\"returnCode\":\"INFO_PFO_00023\","
        + "\"returnMsg\":\"Request processed successfully(loopback - SubmitModifiedPlayerKBA).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
        + "\"recordStatus\":\"Updated\","
        + "\"playerDetails\":"
            + "{"
            + "\"returnCode\":\"INFO_PFO_00009\","
            + "\"returnMsg\":\"Player details successfully modified.\","
            + "\"operatorId\":\"" + operatorId + "\","
            + "\"siteId\":\"" + siteId + "\","
            + "\"userName\":\"" + userName + "\","
            + "\"firstName\":\"John\","
            + "\"middleInitial\":\"M\","
            + "\"lastName\":\"Smith\","
            + "\"gender\":\"MALE\","
            + "\"dob\":\"02/01/1969\","
            + "\"emailAddress\":\"john.smith@example.com\","
            + "\"playerAddress1\":\"123 LORDS STREET\","
            + "\"playerAddress2\":\"123 SECOND STREET\","
            + "\"city\":\"ANNEMANIE\","
            + "\"county\":\"RUSSELL\","
            + "\"state\":\"ALABAMA\","
            + "\"zipCode\":\"36721\","
            + "\"country\":\"UNITED STATES\","
            + "\"mobileNo\":\"\","
            + "\"landLineNo\":\"+1-1234567895\","
            + "\"ssn\":\"123456789\","
            + "\"dlNumber\":\"\","
            + "\"dlIssuingState\":\"\","
            + "\"stPlayerId\":\"1113116000020000188888\","
            + "\"stReferenceNo\":\"111311600002000028888\","
            + "\"sessionToken\":\"200446"
            + "}"
        + "}";
        return mJson;
    }
    private string stapiSubmitModifiedPlayerKBATestCase2(string pJson, String pState)
    {
        var mJson =
            "{"
            + "\"returnCode\":\"ERR_PFO_00178\","
            + "\"returnMsg\":\"Site authentication failure(loopback - SubmitModifiedPlayerKBA).\","
            + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\""
            + "}";
        return mJson;
    }

    private string stapiModifyPlayerRequestTestCase1(string pJson, String pState)
    {
        var mJson =
        "{"
        + "\"responseToken\":\"PGNo_g5lOybKWmYDd1znS3BPCXrmPCdRvXg6vnAU-us=\","
        + "\"returnCode\":\"INFO_PFO_00021\","
        + "\"returnMsg\":\"Please answer personal verification questions(loopback - ModifyPlayerRequest).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
        + "\"kycDetails\":"
            + "{"
            + "\"interactiveQueryId\":\"0\","
            + "\"transactionKey\":\"0000000000000000000000000\","
            + "\"question\":"
                + "{"
                + "\"interactiveQueryId\":\"1\","
                + "\"question\":"
                    + "["
                        + "{"
                        + "\"questionText\":\"<question>\","
                        + "\"questionTextId\":\"null\","
                        + "\"questionTokens\":\"null\","
                        + "\"choiceCode\":\"null\","
                        + "\"questionId\":\"    1\","
                        + "\"answerChoice\":"
                            + "["
                                + "{"
                                + "\"value\":\"<answer>\","
                                + "\"answerId\":\"1\""
                                + "},"
                                + "{"
                                + "\"value\":\"<answer>\","
                                + "\"answerId\":\"2\""
                                + "},"
                                + "{"
                                + "\"value\":\"<answer>\","
                                + "\"answerId\":\"3\""
                                + "},"
                                + "{"
                                + "\"value\":\"<answer>\","
                                + "\"answerId\":\"4\""
                                + "},"
                                + "{"
                                + "\"value\":\"<answer>\","
                                + "\"answerId\":\"5\""
                                + "}"
                            + "]"
                        + "},"
                        + "{"
                        + "\"questionText\":\"<question>\","
                        + "\"questionTextId\":\"null\","
                        + "\"questionTokens\":\"null\","
                        + "\"choiceCode\":\"null\","
                        + "\"questionId\":\"2\","
                        + "\"answerChoice\":"
                            + "["
                                + "{"
                                + "\"value\":\"<answer>\","
                                + "\"answerId\":\"1\""
                                + "},"
                                + "{"
                                + "\"value\":\"<answer>\","
                                + "\"answerId\":\"2\""
                                + "},"
                                + "{"
                                + "\"value\":\"<answer>\","
                                + "\"answerId\":\"3\""
                                + "},"
                                + "{"
                                + "\"value\":\"<answer>\","
                                + "\"answerId\":\"4\""
                                + "},"
                                + "{"
                                + "\"value\":\"<answer>\","
                                + "\"answerId\":\"5\""
                                + "}"
                            + "]"
                        + "},"
                        + "{"
                        + "\"questionText\":\"question\","
                        + "\"questionTextId\":\"null\","
                        + "\"questionTokens\":\"null\","
                        + "\"choiceCode\":\"null\","
                        + "\"questionId\":\"3\","
                        + "\"answerChoice\":"
                            + "["
                                + "{"
                                + "\"value\":\"<answer>\","
                                + "\"answerId\":\"1\""
                                + "},"
                                + "{"
                                + "\"value\":\"<answer>\","
                                + "\"answerId\":\"2\""
                                + "},"
                                + "{"
                                + "\"value\":\"<answer>\","
                                + "\"answerId\":\"3\""
                                + "},"
                                + "{"
                                + "\"value\":\"<answer>\","
                                + "\"answerId\":\"4\""
                                + "},"
                                + "{"
                                + "\"value\":\"<answer>\","
                                + "\"answerId\":\"5\""
                                + "}"
                            + "]"
                        + "},"
                        + "{"
                        + "\"questionText\":\"<question>\","
                        + "\"questionTextId\":\"null\","
                        + "\"questionTokens\":\"null\","
                        + "\"choiceCode\":\"null\","
                        + "\"questionId\":\"4\","
                        + "\"answerChoice\":"
                            + "["
                                + "{"
                                + "\"value\":\"<answer>\","
                                + "\"answerId\":\"1\""
                                + "},"
                                + "{"
                                + "\"value\":\"<answer>\","
                                + "\"answerId\":\"2\""
                                + "},"
                                + "{"
                                + "\"value\":\"<answer>\","
                                + "\"answerId\":\"3\""
                                + "},"
                                + "{"
                                + "\"value\":\"<answer>\","
                                + "\"answerId\":\"4\""
                                + "},"
                                + "{"
                                + "\"value\":\"<answer>\","
                                + "\"answerId\":\"5\""
                                + "}"
                            + "]"
                        + "}"
                    + "]"
                + "}"
            + "}"
        + "}";
        return mJson;
    }
    private string stapiModifyPlayerRequestTestCase2(string pJson, String pState)
    {
        var mJson =
            "{"
            + "\"returnCode\":\"ERR_PFO_00178\","
            + "\"returnMsg\":\"Site authentication failure(loopback - ModifyPlayerRequest).\","
            + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\""
            + "}";
        return mJson;
    }

    private string stapiSelfExcludePlayerTestCase1(string pJson, String pState)
    {
        /* input json
        { 
            "operatorId":"000", 
            "siteId":"000", 
            "siteUsername":"123456789545", 
            "sitePwd":")*(&^%$#",
            "userName":"johnsmith1",
            "requestToken":"KLvYTGHhvmIoys6qF_HVH_sJbJXJ5ECfpDnzwRVtpWx=",  
            "exclusionDetails": 
            { 
                "fromDate":"03/26/2012", 
                "toDate":"06/05/2012", 
                "requestHelp":"Y", 
                "requestExtension":"N", 
                "screenShown":"Y"
            } 
        } 
        */
        dynamic mJsonData = JObject.Parse(pJson);
        string userName = mJsonData.userName.ToString();

        var mJson =
        "{"
            + "\"returnCode\":\"INFO_PFO_00002\","
            + "\"returnMsg\":\"Player successfully validated(loopback - SelfExcludePlayer).\","
            + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
            + "\"validationDetails\":"
            + "{"
                + "\"userName\":\"" + userName + "\","
                + "\"stPlayerId\":\"126232200000000198832\","
                + "\"stReferenceNo\":\"126232200000000278285\","
                + "\"connectionToken\":\"653be279f3ed965f092bba916a0ca1\","
                + "\"sessionToken\":\"233936\","
                + "\"games\":"
                    + "["
                        + "{"
                            + "\"gameId\":3,"
                            + "\"gameName\":\"Lottery\","
                            + "\"gameStartTime\":\"02:00\","
                            + "\"gameEndTime\":\"07:59\""
                        + "}"
                    + "]"
                + "}"
        + "}";
        return mJson;
    }
    private string stapiSelfExcludePlayerTestCase2(string pJson, String pState)
    {
        var mJson =
        "{"
        + "\"returnCode\":\"ERR_PFO_00040\","
        + "\"returnMsg\":\"Invalid toDate(loopback - SelfExcludePlayer).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\""
        + "}";
        return mJson;
    }

    private string stapiGetRegisteredCardsTestCase1(string pJson, String pState)
    {
        dynamic mJsonData = JObject.Parse(pJson);
        string mOperatorId = mJsonData.operatorId.ToString();
        string mSiteId = mJsonData.siteId.ToString();
        string mUserName = mJsonData.userName.ToString();

        var mJson =
        "{"
        + "\"returnCode\":\"INFO_PFO_00023\","
        + "\"returnMsg\":\"Request processed successfully(loopback - GetRegisteredCards).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
        + "\"operatorId\":\"" + mOperatorId + "\","
        + "\"siteId\":\"" + mSiteId + "\","
        + "\"userName\":\"" + mUserName + "\","
        + "\"cardList\":"
            + "["
                + "{"
                    + "\"cardType\":\"MASTERCARD\","
                    + "\"cardNumber\":\"540000######0000\","
                    + "\"cardToken\":\"gCIxtcEQSdV38bO7Lav_J50eq-\","
                    + "\"expiryDate\":\"03/2012\""
                + "},"
                + "{"
                    + "\"cardType\":\"VISA\","
                    + "\"cardNumber\":\"400000######0000\","
                    + "\"cardToken\":\"2c_4zmohdUqm3H2RjA7K8O3P58DWi4kL_8\","
                    + "\"expiryDate\":\"03/2013\""
                + "}"
            + "]"
        + "}";
        return mJson;
    }
    private string stapiGetRegisteredCardsTestCase2(string pJson, String pState)
    {
        var mJson =
        "{"
        + "\"returnCode\":\"ERR_PFO_00001\","
        + "\"returnMsg\":\"Validation Failure - Field validation failure(loopback - GetRegisteredCards).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
        + "\"recordStatus\":\"Failed\","
        + "\"validation Errors\":"
            + "["
                + "{"
                + "\"validationError\":\"Invalid siteUsername.\","
                + "\"validationField\":\"siteUsername\""
                + "}"
            + "]"
        + "}";
        return mJson;
    }

    private string stapiPreCheckSiteCardRequestTestCase1(string pJson, String pState)
    {

        /* the request json
         * {
         * "operatorId":"188",
         * "siteId":"227",
         * "siteUsername":"Loopback Test Case=1",
         * "sitePwd":"@8cIa6NBTdTZjzb7","userName":"Terry155015165044",
         * "accountType":"ECOM",
         * "connectionToken":"653be279f3ed965f092bba916a0ca1",
         * "sessionToken":"233936",
         *  "cardDetails":
         *  {
         *      "nameOnCard":"Terry Davis",
         *      "cardType":"Visa",
         *      "cardNumber":"4000000000000077",
         *      "cardToken":"",
         *      "startDate":"01/2015",
         *      "expiryDate":"12/2016",
         *      "issueNumber":"21",
         *      "defaultCard":"Y",
         *      "cvv":"123",
         *      "cardBillingInfo":
         *      {
         *          "playerAddress1":"10091 E. Buckskin Trail",
         *          "playerAddress2":"Home",
         *          "city":"JEFFERSON CITY",
         *          "county":"UNITED STATES",
         *          "state":"MISSOURI",
         *          "zipCode":"65109",
         *          "country":"UNITED STATES"
         *      }
         *  }
         * }
         */
        dynamic mJsonData = JObject.Parse(pJson);
        string mOperatorId = mJsonData.operatorId.ToString();
        string mSiteId = mJsonData.siteId.ToString();
        string mCardType = mJsonData.cardDetails.cardType.ToString();
        string mStartDate = mJsonData.cardDetails.startDate.ToString();
        string mCardExpDate = mJsonData.cardDetails.expiryDate.ToString();
        string mCardNumber = mJsonData.cardDetails.cardNumber.ToString();
        string mIssueNumber = mJsonData.cardDetails.issueNumber.ToString();

        var mJson =
        "{"
            + "\"returnCode\":\"INFO_PFO_00016\","
            + "\"returnMsg\":\"Card successfully registered(loopback - PreCheckSiteCardRequest).\","
            + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
            + "\"recordStatus\":\"Created\","
            + "\"cardRegistrationDetails\":"
            + "{"
                + "\"operatorId\":\"" + mOperatorId + "\","
                + "\"siteId\":\"" + mSiteId + "\","
                + "\"cardDetails\":"
                + "{"
                    + "\"cardNumber\":\"" + mCardNumber + "\","
                    + "\"cardToken\":\"" + Create_CardToken() + "\","
                    + "\"cardType\":\"" + mCardType + "\","
                    + "\"expiryDate\":\"" + mCardExpDate + "\","
                    + "\"issueNumber\":\"" + mIssueNumber + "\","
                    + "\"startDate\":\"" + mStartDate + "\""
                + "}"
            + "}"
        + "}";
        return mJson;
    }
    private string stapiPreCheckSiteCardRequestTestCase2(string pJson, String pState)
    {
        var mJson =
        "{"
        + "\"returnCode\":\"ERR_PFO_00178\","
        + "\"returnMsg\":\"Site authentication failure(loopback - PreCheckSiteCardRequest).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\""
        + "}";
        return mJson;
    }

    private string stapiPreCheckSiteValidatePlayerRequestTestCase1(string pJson, string pStateId)
    {
        /*
        {
         "operatorId":"188",
         "siteId":"227",
         "siteUsername":"Loopback Test Case=1",
         "sitePwd":"@8cIa6NBTdTZjzb7",
         "userName":"Terry155015165044",
         "ipAddress":"184.182.215.167",
         "geoComplyEncryptedPacket":"ZsUiDymAiyVr/aQxwqC60c50qCfhJ9WPvZo3TrNAmXxD20onJILaqkmK+CGEDzr7tveVE=",
         "deviceFingerPrint":" "}
         * */

        dynamic mJsonData = JObject.Parse(pJson);
        string mUserName = mJsonData.userName.ToString();

        var mJson =
        "{"
            + "\"returnCode\":\"INFO_PFO_00002\","
            + "\"returnMsg\":\"Player successfully validated(loopback - PreCheckSiteValidatePlayerRequest).\","
            + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
            + "\"validationDetails\":"
            + "{"
                + "\"userName\":\"" + mUserName + "\","
                + "\"stPlayerId\":\"126232200000000198832\","
                + "\"stReferenceNo\":\"126232200000000278285\","
                + "\"connectionToken\":\"653be279f3ed965f092bba916a0ca1\","
                + "\"sessionToken\":\"233936\","
                + "\"games\":"
                + "["
                    + "{"
                        + "\"gameId\":3,"
                        + "\"gameName\":\"Lottery\","
                        + "\"gameStartTime\":\"02:00\","
                        + "\"gameEndTime\":\"07:59\""
                    + "}"
                + "]"
            + "}"
        + "}";
        return mJson;


        //If the Secure Trading application identifies that the player is accessing the gaming site for the first time after Self Exclusion period has expired,
        //then the following response is sent to the Operator platform.

        //var mJson =
        //"{"
        //+ "\"returnCode\":\"ERR_PFO_00001\","
        //+ "\"returnMsg\":\"Player was previously Self Excluded, please display the screen with the option to continue Self Exclusion.\","
        //+ "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
        //+ "\"responseToken\":\"KLvYTGHhvmIoys6qF_HVH_sJbJXJ5ECfpDnzwRVtpWx="
        //+ "}";
        //return mJson;


    }
    private string stapiPreCheckSiteValidatePlayerRequestTestCase2(string pJson, string pStateId)
    {
        var mJson =
        "{"
        + "\"returnCode\":\"ERR_PFO_00178\","
        + "\"returnMsg\":\"Site authentication failure(loopback - PreCheckSiteValidatePlayerRequest).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\""
        + "}";
        return mJson;

        //In the event that the Player attempts to login during an active self exclusion period, the following error response is returned from the Secure Trading application:

        //var mJson =
        //"{"
        //+ "\"returnCode\":\"ERR_PFO_00008\","
        //+ "\"returnMsg\":\"Player is self-excluded from the system until {DATE}.\","
        //+ "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + ""
        //+ "}";
        //return mJson;

    }

    private string stapiRegisterPlayerTestCase1(string pJson, string pStateId)
    {
        dynamic mJsonData = JObject.Parse(pJson);
        string mOperatorId = mJsonData.operatorId.ToString();
        string mSiteId = mJsonData.siteId.ToString();
        string userName = mJsonData.playerDetails.userName.ToString();
        var mDb = new MongoDBData();
        var mPlayer = mDb.GetPlayerByUsername(userName);
        var mJson =
        "{"
        + "\"returnCode\":\"INFO_PFO_00023\","
        + "\"returnMsg\":\"Request processed successfully(loopback - RegisterPlayer).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
        + "\"recordStatus\":\"Created\","
        + "\"playerDetails\":"
            + "{"
            + "\"returnCode\":\"INFO_PFO_00007\","
            + "\"returnMsg\":\"Player registered successfully.\","
            + "\"operatorId\":\"" + mOperatorId + "\","
            + "\"siteId\":\"" + mSiteId + "\","
            + "\"userName\":\"" + mPlayer.userName + "\","
            + "\"firstName\":\"" + mPlayer.firstName + "\","
            + "\"lastName\":\"" + mPlayer.lastName + "\","
            + "\"gender\":\"" + mPlayer.gender + "\","
            + "\"dob\":\"" + mPlayer.dob + "\","
            + "\"emailAddress\":\"" + mPlayer.emailAddress + "\","
            + "\"playerAddress1\":\"" + mPlayer.playerAddress1 + "\","
            + "\"city\":\"" + mPlayer.city + "\","
            + "\"state\":\"" + mPlayer.state + "\","
            + "\"country\":\"" + mPlayer.country + "\","
            + "\"county\":\"" + mPlayer.county + "\","
            + "\"zipCode\":\"" + mPlayer.zipCode + "\","
            + "\"mobileNo\":\"" + mPlayer.mobileNo + "\","
            + "\"landLineNo\":\"" + mPlayer.landLineNo + "\","
            + "\"ssn\":\"" + mPlayer.ssn + "\","
            + "\"dlNumber\":\"" + mPlayer.dlNumber + "\","
            + "\"dlIssuingState\":\"" + mPlayer.dlIssuingState + "\","
            + "\"stPlayerId\":\"111311600000000180370\","
            + "\"stReferenceNo\":\"111311600000000226149\""
            + "}"
        + "}";
        return mJson;
    }
    private string stapiRegisterPlayerTestCase2(string pJson, string pStateId)
    {
        var mJson =
       "{"
           + "\"responseToken\":\"" + pStateId + "\","
           + "\"returnCode\":\"INFO_PFO_00021\","
           + "\"returnMsg\":\"Please answer personal verification questions\","
           + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
           + "\"kycDetails\":"
            + "{"
                + "\"interactiveQueryId\":0,"
                + "\"transactionKey\":\"0000000000000000000000000\","
                + "\"question\":"
                + "{"
                + "\"interactiveQueryId\":1,"
                + "\"question\":"
                + "["
                + "{"
                + "\"questionText\":\"Is this really you?\","
                + "\"questionTextId\":null,"
                + "\"questionTokens\":null,"
                + "\"choiceCode\":null,"
                + "\"questionId\":1,"
                + "\"answerChoice\":"
                + "["
                + "{"
                + "\"value\":\"returnCode:INFO_PFO_00023, playerDetails.returnCode:INFO_PFO_00007, Player registered successfully.\","
                + "\"answerId\":1"
                + "},"
                + "{"
                +
                "\"value\":\"returnCode:INFO_PFO_00023, playerDetails.returnCode:ERR_PFO_00072, KYC Check Failure - Player failed KYC Check.\","
                + "\"answerId\":2"
                + "},"
                + "{"
                +
                "\"value\":\"returnCode:INFO_PFO_00023, playerDetails.returnCode:ERR_PFO_00072, KYC Check Failure - Player failed KYC Check.\","
                + "\"answerId\":3"
                + "},"
                + "{"
                +
                "\"value\":\"returnCode:INFO_PFO_00023, playerDetails.returnCode:INFO_PFO_00007, Player registered successfully.\","
                + "\"answerId\":4"
                + "},"
                + "{"
                +
                "\"value\":\"returnCode:ERR_PFO_00083, System Error – Unable to save Player identification details\","
                + "\"answerId\":5"
                + "}"
                + "]"
                + "},"
                + "{"
                + "\"questionText\":\"Unuser in loopback, just pick one\","
                + "\"questionTextId\":null,"
                + "\"questionTokens\":null,"
                + "\"choiceCode\":null,"
                + "\"questionId\":2,"
                + "\"answerChoice\":"
                + "["
                + "{"
                + "\"value\":\"One\","
                + "\"answerId\":1"
                + "},"
                + "{"
                + "\"value\":\"Two\","
                + "\"answerId\":2"
                + "},"
                + "{"
                + "\"value\":\"Three\","
                + "\"answerId\":3"
                + "},"
                + "{"
                + "\"value\":\"Four\","
                + "\"answerId\":4"
                + "},"
                + "{"
                + "\"value\":\"Five\","
                + "\"answerId\":5"
                + "}"
                + "]"
                + "},"
                + "{"
                + "\"questionText\":\"Not used in loopback, just pick one\","
                + "\"questionTextId\":null,"
                + "\"questionTokens\":null,"
                + "\"choiceCode\":null,"
                + "\"questionId\":3,"
                + "\"answerChoice\":"
                + "["
                + "{"
                + "\"value\":\"One\","
                + "\"answerId\":1"
                + "},"
                + "{"
                + "\"value\":\"Two\","
                + "\"answerId\":2"
                + "},"
                + "{"
                + "\"value\":\"Three\","
                + "\"answerId\":3"
                + "},"
                + "{"
                + "\"value\":\"Four\","
                + "\"answerId\":4"
                + "},"
                + "{"
                + "\"value\":\"Five\","
                + "\"answerId\":5"
                + "}"
                + "]"
                + "},"
                + "{"
                + "\"questionText\":\"Not used in loopback, just pick one\","
                + "\"questionTextId\":null,"
                + "\"questionTokens\":null,"
                + "\"choiceCode\":null,"
                + "\"questionId\":4,"
                + "\"answerChoice\":"
                + "["
                + "{"
                + "\"value\":\"One\","
                + "\"answerId\":1"
                + "},"
                + "{"
                + "\"value\":\"Two\","
                + "\"answerId\":3"
                + "},"
                + "{"
                + "\"value\":\"Three\","
                + "\"answerId\":3"
                + "},"
                + "{"
                + "\"value\":\"Four\","
                + "\"answerId\":4"
                + "},"
                + "{"
                + "\"value\":\"Five\","
                + "\"answerId\":5"
                + "}"
                + "]"
                + "}"
                + "]"
                + "}"
            + "}"
       + "}";
        return mJson;
    }
    private string stapiRegisterPlayerTestCase3(string pJson, string pStateId)
    {
        var mJson =
        "{"
        + "\"returnCode\":\"ERR_PFO_00001\","
        + "\"returnMsg\":\"Validation Failure - Field validation failure(loopback - RegisterPlayer).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
        + "\"recordStatus\":\"Failed\","
        + "\"validationErrors\":"
            + "["
                + "{"
                + "\"validationError\":\"Invalid OperatorId.\","
                + "\"validationField\":\"OperatorId\""
                + "}"
            + "]"
        + "}";
        return mJson;
    }

    private string stapiSubmitPlayerKBATestCase1(string pJson, string pStateId)
    {
        var mDatabase = new MongoDBData();
        var mState = mDatabase.Read(pStateId);
        var mPlayer = mDatabase.GetPlayerByUsername(mState.stUserName);

        dynamic mJsonData = JObject.Parse(pJson);
        string mOperatorId = mJsonData.playerDetails.operatorId.ToString();
        string mSiteId = mJsonData.playerDetails.siteId.ToString();

        var mJson =
        "{"
        + "\"returnCode\":\"INFO_PFO_00023\","
        + "\"returnMsg\":\"Request processed successfully(loopback - SubmitPlayerKBA).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
        + "\"recordStatus\":\"Created\","
        + "\"playerDetails\":"
            + "{"
            + "\"returnCode\":\"INFO_PFO_00007\","
            + "\"returnMsg\":\"Player registered successfully.\","
            + "\"operatorId\":\"" + mOperatorId + "\","
            + "\"siteId\":\"" + mSiteId + "\","
            + "\"userName\":\"" + mPlayer.userName + "\","
            + "\"firstName\":\"" + mPlayer.firstName + "\","
            + "\"middleInitial\":\"" + mPlayer.middleInitial + "\","
            + "\"lastName\":\"" + mPlayer.lastName + "\","
            + "\"gender\":\"" + mPlayer.gender + "\","
            + "\"dob\":\"" + mPlayer.dob + "\","
            + "\"emailAddress\":\"" + mPlayer.emailAddress + "\","
            + "\"playerAddress1\":\"" + mPlayer.playerAddress1 + "\","
            + "\"playerAddress2\":\"" + mPlayer.playerAddress2 + "\","
            + "\"city\":\"" + mPlayer.city + "\","
            + "\"state\":\"" + mPlayer.state + "\","
            + "\"country\":\"" + mPlayer.country + "\","
            + "\"county\":\"" + mPlayer.county + "\","
            + "\"zipCode\":\"" + mPlayer.zipCode + "\","
            + "\"mobileNo\":\"" + mPlayer.mobileNo + "\","
            + "\"landLineNo\":\"" + mPlayer.landLineNo + "\","
            + "\"ssn\":\"" + mPlayer.ssn + "\","
            + "\"dlNumber\":\"" + mPlayer.dlNumber + "\","
            + "\"dlIssuingState\":\"" + mPlayer.dlIssuingState + "\","
            + "\"stPlayerId\":\"" + mPlayer.stiPlayerId + "\","
            + "\"stReferenceNo\":\"" + mPlayer.stiReferenceNumber + "\""
            + "}"
        + "}";
        return mJson;
    }
    private string stapiSubmitPlayerKBATestCase2(string pJson, string pStateId)
    {
        var mJson =
        "{"
        + "\"returnCode\":\"ERR_PFO_00083\","
        + "\"returnMsg\":\"System Error – Unable to save Player identification details.(loopback - SubmitPlayerKBA).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
        + "\"recordStatus\":\"Failed\""
        + "}";
        return mJson;
    }

    private string stapiLoadPrePaidFundsTestCase1(string pJson, string pStateId)
    {
        /* Request Json
         * {
         * "operatorId":"188",
         * "siteId":"227",
         * "siteUsername":"Loopback Test Case=1",
         * "sitePwd":"@8cIa6NBTdTZjzb7",
         * "geoComplyEncryptedPacket":"ZsUiDymAiyVr/aQxwqC60c50qCfhJ9WPvZo3TrNAmXxD20onJILaqkmK+CGEDzr7tveVE=",
         * "depositDetails":
         * {
             * "userName":"Terry155015165044",
             * "connectionToken":"653be279f3ed965f092bba916a0ca1",
             * "sessionToken":"233936",
             * "orderReference":"1234",
             * "accountType":"ECOM",
             * "currency":"USD",
             * "transactionType":"DEPOSIT",
             * "threeDFlag":"FALSE",
             * "amount":"1000",
             * "cardDetails":
             * {
                 * "nameOnCard":"Terry Davis",
                 * "cardType":"Visa",
                 * "cardNumber":"4000000000000077",
                 * "cardToken":"null",
                 * "startDate":"01/2015",
                 * "expiryDate":"12/2016",
                 * "issueNumber":"21",
                 * "defaultCard":"Y",
                 * "cvv":"123",
                 * "cardBillingInfo":
                 * {
                     * "playerAddress1":"10091 E. Buckskin Trail",
                     * "playerAddress2":"Home",
                     * "city":"JEFFERSON CITY",
                     * "county":"UNITED STATES",
                     * "state":"MISSOURI",
                     * "zipCode":"65109",
                     * "country":"UNITED STATES"
                 * }
             * }
           * }
        * }

        */
        dynamic mJsonData = JObject.Parse(pJson);
        string operatorId = mJsonData.operatorId.ToString();
        string siteId = mJsonData.siteId.ToString();
        string amount = mJsonData.depositDetails.amount.ToString();
        string currency = mJsonData.depositDetails.currency.ToString();
        var prePaidAccountNumber = Create_prePaidAccountNumber();
        var prePaidAccountToken = Create_prePaidAccountToken(pStateId);
        var prePaidAccountBalance = "10000";

        var mJson =
        "{"
        + "\"returnCode\":\"INFO_PFO_00023\","
        + "\"returnMsg\":\"Request processed successfully(loopback - LoadPrePaidFund).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
        + "\"prePaidAccountRegistrationResponse\":"
            + "{"
            + "\"operatorId\":\"" + operatorId + "\","
            + "\"siteId\":\"" + siteId + "\","
            + "\"prePaidAccountNumber\":\"" + prePaidAccountNumber + "\","
            + "\"prePaidAccountToken\":\"" + prePaidAccountToken + "\","
            + "\"prePaidAccountBalance\":\"" + prePaidAccountBalance + "\","
            + "\"loadAmount\":\"" + amount + "\","
            + "\"currency\":\"" + currency + "\","
            + "\"receiptHtml\":\"<Value>\""
            + "}"
        + "}";
        return mJson;
    }
    private string stapiLoadPrePaidFundsTestCase2(string pJson, string pStateId)
    {
        var mJson =
        "{"
        + "\"returnCode\":\"ERR_PFO_00112\","
        + "\"returnMsg\":\"Authorization Failure(loopback - LoadPrePaidFund).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\""
        + "}";
        return mJson;
    }

    private string stapiPreCheckSiteModifyCardRequestTestCase1(string pJson, string pStateId)
    {
        dynamic mJsonData = JObject.Parse(pJson);
        string mOperatorId = mJsonData.operatorId.ToString();
        string mSiteId = mJsonData.siteId.ToString();
        string mCardToken = mJsonData.cardDetails.cardToken.ToString();
        string deleteFlag = mJsonData.cardDetails.deleteFlag.ToString();
        if (deleteFlag == "N")
        {
            string mCardType = mJsonData.cardDetails.cardType.ToString();
            string mStartDate = mJsonData.cardDetails.startDate.ToString();
            string mCardExpDate = mJsonData.cardDetails.expiryDate.ToString();
            string mCardNumber = mJsonData.cardDetails.cardNumber.ToString();
            string mIssueNumber = mJsonData.cardDetails.issueNumber.ToString();
            var mJson =
                "{"
                + "\"returnCode\":\"INFO_PFO_00017\","
                + "\"returnMsg\":\"Card details successfully modified(loopback - PreCheckSiteModifyCardRequest).\","
                + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
                + "\"recordStatus\":\"Updated\","
                + "\"cardRegistrationDetails\":"
                + "{"
                + "\"operatorId\":\"" + mOperatorId + "\","
                + "\"siteId\":\"" + mSiteId + "\","
                + "\"cardDetails\":"
                + "{"
                + "\"cardNumber\":\"" + mCardNumber + "\","
                + "\"cardToken\":\"" + mCardToken + "\","
                + "\"cardType\":\"" + mCardType + "\","
                + "\"expiryDate\":\"" + mCardExpDate + "\","
                + "\"issueNumber\":\"" + mIssueNumber + "\","
                + "\"startDate\":\"" + mStartDate + "\""
                + "}"
                + "}"
                + "}";
            return mJson;
        }
        else // delete card
        {
            var mJson =
                "{"
                + "\"returnCode\":\"INFO_PFO_00038\","
                + "\"returnMsg\":\"Card details successfully deleted.(loopback - PreCheckSiteModifyCardRequest)\","
                + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
                + "\"cardToken\":\"" + mCardToken + "\","
                + "\"recordStatus\":\"Deleted\""
                + "}";
            return mJson;
            
        }
    }
    private string stapiPreCheckSiteModifyCardRequestTestCase2(string pJson, string pStateId)
    {
        var mJson =
        "{"
        + "\"returnCode\":\"ERR_PFO_00178\","
        + "\"returnMsg\":\"Site authorization Failure(loopback - PreCheckSiteModifyCardRequest).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\""
        + "}";
        return mJson;
    }
   
    private string stapiGetPrePaidAccountBalanceTestCase1(string pJson, string pStateId)
    {
        

        /* Response Json
         * {
         * "returnCode":"INFO_PFO_00023",
         * "returnMsg":"Request processed successfully.", 
         * "stTimeStamp":"2012-08-13T11:54:21.804+05:30", 
         * "prePaidAccountDetails":
         * { 
         * " prePaidAccountToken":"000", 
         * " currency":"USD", 
         * " prePaidAccountBalance":"10000" 
         * }
         * 
         * */
        var currency = "USD";
        var prePaidAccountToken = "1234567890";
        var prePaidAccountBalance = "10000";
        var mJson =
        "{"
        + "\"returnCode\":\"INFO_PFO_00023\","
        + "\"returnMsg\":\"Request processed successfully (loopback - stapiGetPrePaidAccountBalance).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
        + "\"prePaidAccountDetails\":"
            + "{"
            + "\"prePaidAccountToken\":\"" + prePaidAccountToken + "\","
            + "\"currency\":\"" + currency + "\","
            + "\"prePaidAccountBalance\":\"" + prePaidAccountBalance + "\""
            + "}"
        + "}";
        return mJson;
    }
    private string stapiGetPrePaidAccountBalanceTestCase2(string pJson, string pStateId)
    {
        /* Error Response Json
         * { "returnCode":"ERR_PFO_00001", "returnMsg":"Validation Failure - Field validation failure. Please see validationErrors for more details.", "stTimeStamp":"2013-04-09T17:18:10.277+05:30", "recordStatus":"Failed", "validationErrors": [ { "validationField":"siteUsername","validationError":"Invalid siteUserName." } ] }
         * */
        var mJson =
        "{"
        + "\"returnCode\":\"ERR_PFO_00001\","
        + "\"returnMsg\":\"Validation Failure - Field validation failure(loopback - stapiGetPrePaidAccountBalance).\","
        + "\"stTimeStamp\":\"" + mMACUtils.getSTFormatedDate() + "\","
        + "\"recordStatus\":\"Failed\","
        + "\"validationErrors\":"
            + "["
                + "{"
                + "\"validationError\":\"Invalid OperatorId.\","
                + "\"validationField\":\"OperatorId\""
                + "}"
            + "]"
        + "}";
        return mJson;
    }


    
    #endregion

    #region helper methods

    private string Create_CardToken()
    {
        //todo:
        return "lD6WMk_0pcR-dKOyXw4V6KXqART9_G4=";
    }

    private string Create_prePaidAccountNumber()
    {
        //todo:
        return "51############01";
    }

    private string Create_prePaidAccountToken(string pStateId)
    {
        //todo:
        return "IpFlsWGtfoP379vDAVsMi-g=";
    }

    private string Create_virtualCardNumber()
    {
        //todo:
        return "0000000000000000";
    }
    #endregion
}
