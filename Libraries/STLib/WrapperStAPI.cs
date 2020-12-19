using System;
using System.Net;
using System.Text;
using System.Configuration;
using System.IO;
using Newtonsoft.Json.Linq;
using strt = STLib.STConstants.ResponsesTemplates;
using cfg = STLib.STConstants.Config;

namespace STLib
{
    public class WrapperStAPI
    {
        #region methods
        public string stapiPrePaidWidthdrawal(StiWrapperState pState)
        {
            return mRequest("stapiSubmitPrePaidWidthdrawal", pState);
        }
        public string stapiSubmitPrePaidWidthdrawal(StiWrapperState pState)
        {
            return mRequest("stapiSubmitPrePaidWidthdrawal", pState);
        }

        public string stapiPreCheckSiteValidatePlayerRequest(StiWrapperState pState)
        {
            return mRequest("auth/stapiPreCheckSiteValidatePlayerRequest", pState);
        }

        public string stapiRegisterPlayer(StiWrapperState pState)
        {
            return mRequest("stapiRegisterPlayer", pState);
        }

        public string stapiSubmitPlayerKBA(StiWrapperState pState)
        {
            return mRequest("stapiSubmitPlayerKBA", pState);
        }

        public string stapiSelfExcludePlayer(StiWrapperState pState)
        {
            return mRequest("auth/stapiSelfExcludePlayer", pState);
        }

        public string stapiModifyPlayerRequest(StiWrapperState pState)
        {
            return mRequest("stapiModifyPlayerRequest", pState);
        }

        public string stapiSubmitModifiedPlayerKBA(StiWrapperState pState)
        {
            return mRequest("stapiSubmitModifiedPlayerKBA", pState);
        }

        public string stapiPreCheckSiteCardRequest(StiWrapperState pState)
        {
            return mRequest("stapiPreCheckSiteCardRequest", pState);
        }

        public string stapiPreCheckSiteModifyCardRequest(StiWrapperState pState)
        {
            return mRequest("stapiPreCheckSiteModifyCardRequest", pState);
        }

        public string stapiPreCheckSiteDeleteSpecifiedCard(StiWrapperState pState)
        {
            return mRequest("stapiPreCheckSiteDeleteSpecifiedCard", pState);
        }

        public string stapiPreCheckSiteAccountRequest(StiWrapperState pState)
        {
            return mRequest("stapiPreCheckSiteAccountRequest", pState);
        }

        public string stapiPreCheckSiteModifyAccountRequest(StiWrapperState pState)
        {
            return mRequest("stapiPreCheckSiteModifyAccountRequest", pState);
        }

        public string stapiRegisterPrePaidAccount(StiWrapperState pState)
        {
            return mRequest("stapiRegisterPrePaidAccount", pState);
        }

        public string stapiSubmitPrePaidIdentityAnswers(StiWrapperState pState)
        {
            return mRequest("stapiSubmitPrePaidIdentityAnswers", pState);
        }

        public string stapiPrePaidRegisterAndLoad(StiWrapperState pState)
        {
            return mRequest("stapiPrePaidRegisterAndLoad", pState);
        }

        public string stapiGetPrePaidAccountBalance(StiWrapperState pState)
        {
            return mRequest("stapiGetPrePaidAccountBalance", pState);
        }

        public string stapiGetPrePaidAccountCVV2(StiWrapperState pState)
        {
            return mRequest("stapiGetPrePaidAccountCVV2", pState);
        }

        public string stapiGetPrePaidAccountStatus(StiWrapperState pState)
        {
            return mRequest("stapiGetPrePaidAccountStatus", pState);
        }

        public string stapiUpdatePrePaidAccountStatus(StiWrapperState pState)
        {
            return mRequest("stapiUpdatePrePaidAccountStatus", pState);
        }

        public string stapiSubmitDepositRequest(StiWrapperState pState)
        {
            return mRequest("stapiSubmitDepositRequest", pState);
        }

        public string stapiSubmitDepositRequestWithGeoComplyWithOut3DSecurity(StiWrapperState pState)
        {
            return mRequest("stapiSubmitDepositRequestWithGeoComplyWithOut3DSecurity", pState);
        }

        public string stapiSubmitDepositRequestWith3DSecurity(StiWrapperState pState)
        {
            return mRequest("stapiSubmitDepositRequestWith3DSecurity", pState);
        }

        public string stapiSubmitDepositRequestWithOut3DSecurity(StiWrapperState pState)
        {
            return mRequest("stapiSubmitDepositRequestWithOut3DSecurity", pState);
        }

        public string stapiAuthDepositRequest(StiWrapperState pState)
        {
            return mRequest("stapiAuthDepositRequest", pState);
        }

        public string stapiSubmitDepositAccountRequest(StiWrapperState pState)
        {
            return mRequest("stapiSubmitDepositAccountRequest", pState);
        }
        public string stapiubmitPrePaidDeposit(StiWrapperState pState)
        {
            return mRequest("stapiubmitPrePaidDeposit", pState);
        }

        public string stapiSubmitPrePaidDeposit(StiWrapperState pState)
        {
            return mRequest("stapiSubmitPrePaidDeposit", pState);
        }

        public string stapiLoadPrePaidFunds(StiWrapperState pState)
        {
            return mRequest("stapiLoadPrePaidFunds", pState);
        }

        public string stapiWithdrawal(StiWrapperState pState)
        {
            return mRequest("stapiWithdrawal", pState);
        }

        public string stapiAccountWithdrawal(StiWrapperState pState)
        {
            return mRequest("stapiAccountWithdrawal", pState);
        }

        public string stapiSubmitReversal(StiWrapperState pState)
        {
            return mRequest("stapiSubmitReversal", pState);
        }

        public string stapiSubmitPrePaidWithdrawal(StiWrapperState pState)
        {
            return mRequest("stapiSubmitPrePaidWithdrawal", pState);
        }

        public string stapiSubmitRefund(StiWrapperState pState)
        {
            return mRequest("stapiSubmitRefund", pState);
        }

        public string stapiGetTransactionDetails(StiWrapperState pState)
        {
            return mRequest("stapiGetTransactionDetails", pState);
        }

        public string stapiUpdateTransaction(StiWrapperState pState)
        {
            return mRequest("stapiUpdateTransaction", pState);
        }

        public string stapiGetRegisteredCards(StiWrapperState pState)
        {
            return mRequest("stapiGetRegisteredCards", pState);
        }

        public string stapiGetRegisteredAccounts(StiWrapperState pState)
        {
            return mRequest("stapiGetRegisteredAccounts", pState);
        }

        public string stapiGetPrePaidAccountHolderInfo(StiWrapperState pState)
        {
            return mRequest("stapiGetPrePaidAccountHolderInfo", pState);
        }

        public string stapiGetPlayerSSN(StiWrapperState pState)
        {
            return mRequest("stapiGetPlayerSSN", pState);
        }

        public string stapiUpdatePlayerSSN(StiWrapperState pState)
        {
            return mRequest("stapiUpdatePlayerSSN", pState);
        }

        public string stapiAddSelfExcludePlayer(StiWrapperState pState)
        {
            return mRequest("stapiAddSelfExcludePlayer", pState);
        }

        #endregion

        public string mRequest(string pMethod, StiWrapperState pState)
        {

            var mMACUtils = new MACUtils();
            dynamic mJsonData = JObject.Parse(pState.stRequestJson);
            string msiteUsername = mJsonData.siteUsername.ToString();
            if (msiteUsername.Contains("Loopback") == false)
            {
                { // log request being issued to ST-1
                    var mEvent = new StiEvent();
                    mEvent.macType = STConstants.EventTypes.WrapperRequestST + " - " + pState.methodName;
                    mEvent.macData = pState.stRequestJson;
                    mEvent.stOperatorId = pState.stOperatorId;
                    mEvent.stSiteId = pState.stSiteId;
                    mEvent.macClientId = pState.macClientId;
                    pState.Events.Add(mEvent);
                    pState.Save(pState);
                }
                var mStiOperations = new StiOperations();
                // issue request to ST-1
                var stResponse = mStiOperations.stapiRequest(pMethod, pState.stRequestJson);
                {// log response
                    var mEvent = new StiEvent();
                    mEvent.macType = STConstants.EventTypes.STResponseWrapper + " - " + pState.methodName;
                    mEvent.stJsonData = pState.stRequestJson;
                    mEvent.stOperatorId = pState.stOperatorId;
                    mEvent.stSiteId = pState.stSiteId;
                    mEvent.macClientId = pState.macClientId;
                    pState.Events.Add(mEvent);
                    pState.Save(pState);
                }
                return stResponse;
            }
            pState.stProcess = "Loopback";
            var sbResponse = new StringBuilder();
            try
            {
                {
                    var mEvent = new StiEvent();
                    mEvent.macType = STConstants.EventTypes.WrapperRequestLB + " - " + pState.methodName;
                    mEvent.macData = pState.stRequestJson;
                    mEvent.stOperatorId = pState.stOperatorId;
                    mEvent.stSiteId = pState.stSiteId;
                    mEvent.macClientId = pState.macClientId;
                    pState.Events.Add(mEvent);
                    pState.Save(pState);
                }
                var mMethod = pMethod;
                if (pMethod.StartsWith("auth")) mMethod = pMethod.Replace("auth/", "");
                var mUrl = ConfigurationManager.AppSettings[cfg.StLoopbackServiceUrl]
                    + String.Format("?data={0}:{1}:{2}", mMethod, pState._id.ToString(), mMACUtils.StringToHex(pState.stRequestJson.ToString()));
                var request = mUrl;
                var webRequest = WebRequest.Create(request);
                webRequest.Method = "Get";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                var res = webRequest.GetResponse();
                var response = res.GetResponseStream();
                if (response == null)
                {
                    var error = strt.SystemErrorTemplates
                    .Replace("[EC]", "ERR_LOP_90020")
                    .Replace("[ET]", "Null response from loopback")
                    .Replace("[DT]", mMACUtils.getSTFormatedDate());
                    return error;
                }
                using (var reader = new StreamReader(response, Encoding.UTF8))
                {
                    var hexdata = reader.ReadToEnd();
                    sbResponse.Append(mMACUtils.HexToString(hexdata));
                }
            }
            catch (Exception ex)
            {
                var error = strt.SystemErrorTemplates
                .Replace("[EC]", "ERR_LOP_90021")
                .Replace("[ET]", "Exception calling loopback:" + ex.Message)
                .Replace("[DT]", mMACUtils.getSTFormatedDate());
                return error;
            }
            {
                var mEvent = new StiEvent();
                mEvent.macType = STConstants.EventTypes.LoopbackResponse + " - " + pState.methodName;
                mEvent.stJsonData = sbResponse.ToString();
                mEvent.stOperatorId = pState.stOperatorId;
                mEvent.stSiteId = pState.stSiteId;
                mEvent.macClientId = pState.macClientId;
                pState.Events.Add(mEvent);
                pState.Save(pState);
            }
            return sbResponse.ToString();
        }
    }

    public class JSONRequest
    {
        public string operatorId { get; set; }
        public string siteId { get; set; }
        public string siteUsername { get; set; }
        public string sitePwd { get; set; }
        public string geoComplyEncryptedPacket { get; set; }
        public string userName { get; set; }
        public string ipAddress { get; set; }
        public JSONPlayer playerDetails { get; set; }
    }

    public class RegisterPlayer
    {
        public string operatorId { get; set; }
        public string siteId { get; set; }
        public string siteUsername { get; set; }
        public string sitePwd { get; set; }
        public string geoComplyEncryptedPacket { get; set; }
        public string userName { get; set; }
        public string ipAddress { get; set; }
        public JSONPlayer playerDetails { get; set; }
    }

    public class JSONPlayer
    {
        public string userName { get; set; }
        public string firstName { get; set; }
        public string middleInitial { get; set; }
        public string lastName { get; set; }
        public string gender { get; set; }
        public string dob { get; set; }
        public string emailAddress { get; set; }
        public string playerAddress1 { get; set; }
        public string playerAddress2 { get; set; }
        public string city { get; set; }
        public string county { get; set; }
        public string state { get; set; }
        public string zipCode { get; set; }
        public string country { get; set; }
        public string mobileNo { get; set; }
        public string landLineNo { get; set; }
        public string ssn { get; set; }
        public string dlNumber { get; set; }
        public string dlIssuingState { get; set; }
        public string ipAddress { get; set; }
    }
}
