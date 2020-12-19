using System;
using System.IO;
using System.Net;
using System.Text;
using System.Configuration;

using cfg = STLib.STConstants.Config;

namespace STLib
{
    public class StiOperations
    {
        public string stapiPreCheckSiteValidatePlayerRequest(string operationData)
        {
            return stapiRequest("auth/stapiPreCheckSiteValidatePlayerRequest", operationData);
        }
        public string stapiRegisterPlayer(string operationData)
        {
            return stapiRequest("stapiRegisterPlayer", operationData);
        }
        public string stapiSubmitPlayerKBA(string operationData)
        {
            return stapiRequest("stapiSubmitPlayerKBA", operationData);
        }
        public string stapiSelfExcludePlayer(string operationData)
        {
            return stapiRequest("auth/stapiSelfExcludePlayer", operationData);
        }
        public string stapiModifyPlayerRequest(string operationData)
        {
            return stapiRequest("stapiModifyPlayerRequest", operationData);
        }
        public string stapiSubmitModifiedPlayerKBA(string operationData)
        {
            return stapiRequest("stapiSubmitModifiedPlayerKBA", operationData);
        }
        public string stapiPreCheckSiteCardRequest(string operationData)
        {
            return stapiRequest("stapiPreCheckSiteCardRequest", operationData);
        }
        public string stapiPreCheckSiteModifyCardRequest(string operationData)
        {
            return stapiRequest("stapiPreCheckSiteModifyCardRequest", operationData);
        }
        public string stapiPreCheckAccountRequest(string operationData)
        {
            return stapiRequest("stapiPreCheckAccountRequest", operationData);
        }
        public string stapiPreCheckModifyAccountRequest(string operationData)
        {
            return stapiRequest("stapiPreCheckModifyAccountRequest", operationData);
        }
        public string stapiRegisterPrePaidAccount(string operationData)
        {
            return stapiRequest("stapiRegisterPrePaidAccount", operationData);
        }
        public string stapiSubmitPrePaidIdentiityAnswers(string operationData)
        {
            return stapiRequest("stapiSubmitPrePaidIdentiityAnswers", operationData);
        }
        public string stapiPrePaidRegisterAndLoad(string operationData)
        {
            return stapiRequest("stapiPrePaidRegisterAndLoad", operationData);
        }
        public string stapiGetPrePaidAccountBalance(string operationData)
        {
            return stapiRequest("stapiGetPrePaidAccountBalance", operationData);
        }
        public string stapiGetPrePaidAccountCVV2(string operationData)
        {
            return stapiRequest("stapiGetPrePaidAccountCVV2", operationData);
        }
        public string stapiGetPrePaidAccountStatus(string operationData)
        {
            return stapiRequest("stapiGetPrePaidAccountStatus", operationData);
        }
        public string stapiSubmitDepositRequest(string operationData)
        {
            return stapiRequest("stapiSubmitDepositRequest", operationData);
        }
        public string stapiAuthDepositRequest(string operationData)
        {
            return stapiRequest("stapiAuthDepositRequest", operationData);
        }
        public string stapiSubmitDepositAccountRequest(string operationData)
        {
            return stapiRequest("stapiSubmitDepositAccountRequest", operationData);
        }
        public string stapiSubmitPrePaidDeposit(string operationData)
        {
            return stapiRequest("stapiSubmitPrePaidDeposit", operationData);
        }
        public string stapiLoadPrePaidFunds(string operationData)
        {
            return stapiRequest("stapiLoadPrePaidFunds", operationData);
        }
        public string stapiWithdrawal(string operationData)
        {
            return stapiRequest("stapiWithdrawal", operationData);
        }
        public string stapiAccountWithdrawal(string operationData)
        {
            return stapiRequest("stapiAccountWithdrawal", operationData);
        }
        public string stapiSubmitPrePaidWithdrawal(string operationData)
        {
            return stapiRequest("stapiSubmitPrePaidWithdrawal", operationData);
        }
        public string stapiSubmitRefund(string operationData)
        {
            return stapiRequest("stapiSubmitRefund", operationData);
        }
        public string stapiGetTransactionDetails(string operationData)
        {
            return stapiRequest("stapiGetTransactionDetails", operationData);
        }
        public string stapiUpdateTransaction(string operationData)
        {
            return stapiRequest("stapiUpdateTransaction", operationData);
        }
        public string stapiGetRegisteredCards(string operationData)
        {
            return stapiRequest("stapiGetRegisteredCards", operationData);
        }
        public string stapiGetRegisteredAccounts(string operationData)
        {
            return stapiRequest("stapiGetRegisteredAccounts", operationData);
        }
        public string stapiGetPrePaidAccountHolderInfo(string operationData)
        {
            return stapiRequest("stapiGetPrePaidAccountHolderInfo", operationData);
        }
        public string stapiGetPlayerSSN(string operationData)
        {
            return stapiRequest("stapiGetPlayerSSN", operationData);
        }
        public string stapiUpdatePlayerSSN(string operationData)
        {
            return stapiRequest("stapiUpdatePlayerSSN", operationData);
        }
        public string stapiAddSelfExcludePlayer(string operationData)
        {
            return stapiRequest("stapiAddSelfExcludePlayer", operationData);
        }
        public string stapiPreCheckSiteAccountRequest(string operationData)
        {
            return stapiRequest("stapiPreCheckSiteAccountRequest", operationData);
        }
        public string stapiUpdatePrePaidAccountStatus(string operationData)
        {
            return stapiRequest("stapiUpdatePrePaidAccountStatus", operationData);
        }
        public string stapiPreCheckSiteModifyAccountRequest(string operationData)
        {
            return stapiRequest("stapiPreCheckSiteModifyAccountRequest", operationData);
        }
        public string stapiSubmitPrePaidIdentityAnswers(string operationData)
        {
            return stapiRequest("stapiSubmitPrePaidIdentityAnswers", operationData);
        }
        public string stapiSubmitReversal(string operationData)
        {
            return stapiRequest("stapiSubmitReversal", operationData);
        }





        public string stapiRequest(string pMethod, string pRequestData)
        {
            var sbResponse1 = new StringBuilder();
            var dataStream = Encoding.UTF8.GetBytes(pRequestData.ToString());
            var mUrl = ConfigurationManager.AppSettings[cfg.STServicesUrl] + pMethod + ".htm";
            try
            {
                var myHttpWebRequest = (HttpWebRequest)WebRequest.Create(mUrl);
                myHttpWebRequest.Method = "POST";

                // Set the content type of the data being posted.
                myHttpWebRequest.ContentType = "application/json;charset=utf-8";
                myHttpWebRequest.ContentLength = dataStream.Length;

                var newStream = myHttpWebRequest.GetRequestStream();
                newStream.Write(dataStream, 0, dataStream.Length);
                newStream.Close();

                using (var response = (HttpWebResponse)myHttpWebRequest.GetResponse())
                {
                    var header = response.GetResponseStream();
                    if (header == null) throw new Exception("stapiRequest returned null header!");
                    var encode = Encoding.GetEncoding("utf-8");
                    var readStream = new StreamReader(header, encode);
                    var mResponseData1 = readStream.ReadToEnd();
                    sbResponse1.Append(mResponseData1);
                    response.Close();
                    readStream.Close();
                }
            }
            catch (Exception ex)
            {
                var errMsg = ex.ToString();
                sbResponse1.Append("Exception: " + errMsg);
            }
            return sbResponse1.ToString();
        }
    }

}
