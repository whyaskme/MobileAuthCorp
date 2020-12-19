using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

using MACSecurity;
using MACServices;

using dk = MACServices.Constants.Dictionary.Keys;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using dv = MACServices.Constants.Dictionary.Values;
using cs = MACServices.Constants.Strings;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using sr = MACServices.Constants.ServiceResponse;

namespace MacOtp
{
    public class MacOtp
    {
        private const string BadEmailAddress = "Error: Invalid Email address: ";
        private const string BadPhoneNumber = "Error: Invalid Phone Number: ";

        public string SendOtpToRegisteredUser(string macUrl, string pClientId, string pGroupId, string pUserLastName, string pUserId, string pTrxType, string pTrxDetails)
        {
            return SendOtpToRegisteredUser(macUrl, pClientId, pGroupId, pUserLastName, pUserId, pTrxType, pTrxDetails, null);
        }
        public string SendOtpToRegisteredUser(string macUrl, string pClientId, string pGroupId, string pUserLastName, string pUserId, string pTrxType, string pTrxDetails, string pAdSelectionDetails)
        {
            var requestData = dk.Request + dk.KVSep + dv.SendOtp +
                dk.ItemSep + dkui.EndUserIpAddress + dk.KVSep + GetUserIp() +
                dk.ItemSep + dk.TrxType + dk.KVSep + pTrxType;

            if (!String.IsNullOrEmpty(pClientId))
                requestData += dk.ItemSep + dk.CID + dk.KVSep + pClientId;
            else // for testing service if client id not included
                pClientId = cs.DefaultClientId;

            if (!String.IsNullOrEmpty(pUserId))
                requestData += dk.ItemSep + dk.UserId + dk.KVSep + pUserId;

            // Group Id if supplied
            if (String.IsNullOrEmpty(pGroupId) == false)
                requestData += dk.ItemSep + dk.GroupId + dk.KVSep + pGroupId;

            // add transaction details if any
            if (String.IsNullOrEmpty(pTrxDetails) == false)
                requestData += dk.ItemSep + dk.TrxDetails + dk.KVSep + StringToHex(pTrxDetails);

            if (!String.IsNullOrEmpty(pAdSelectionDetails))
            {
                if (pAdSelectionDetails.StartsWith(dk.ItemSep))
                    requestData += pAdSelectionDetails;
                else
                    requestData += dk.ItemSep + pAdSelectionDetails;
            }

            return SendRequestToMacOtpServer(macUrl + Constants.ServiceUrls.RequestOtpWebService, pClientId, requestData);
        }

        public string SendOtpToClientUser(string macUrl, string pClientId, string pUserEmail, string pUserPhoneNumber, string pTrxType, string pTrxDetails)
        {
            return SendOtpToClientUser(macUrl, pClientId, pUserEmail, pUserPhoneNumber, pTrxType, pTrxDetails, null);
        }
        public string SendOtpToClientUser(string macUrl, string pClientId, string pUserEmail, string pUserPhoneNumber, string pTrxType, string pTrxDetails, string pAdSelectionDetails)
        {
            if (Regex.IsMatch(pUserEmail, Constants.RegexStrings.EmailAddress) == false)
                return BadEmailAddress + pUserEmail;

            if (Regex.IsMatch(pUserPhoneNumber, Constants.RegexStrings.PhoneNumber) == false)
                return BadPhoneNumber + pUserPhoneNumber;

            var requestData = dk.Request + dk.KVSep + dv.SendOtp +
                              dk.ItemSep + dkui.EndUserIpAddress + dk.KVSep + GetUserIp() +
                              dk.ItemSep + dk.TrxType + dk.KVSep + pTrxType;
            
            if (!String.IsNullOrEmpty(pClientId))
                requestData += dk.ItemSep + dk.CID + dk.KVSep + pClientId;

            if (!String.IsNullOrEmpty(pUserPhoneNumber))
                requestData += dk.ItemSep + dkui.PhoneNumber + dk.KVSep + pUserPhoneNumber;

            if (!String.IsNullOrEmpty(pUserEmail))
                requestData += dk.ItemSep + dkui.EmailAddress + dk.KVSep + pUserEmail;

            // add transaction details if any
            if (String.IsNullOrEmpty(pTrxDetails) == false)
                requestData += dk.ItemSep + dk.TrxDetails + dk.KVSep + StringToHex(pTrxDetails);

            if (!String.IsNullOrEmpty(pAdSelectionDetails))
            {
                if (pAdSelectionDetails.StartsWith(dk.ItemSep))
                    requestData += pAdSelectionDetails;
                else
                    requestData += dk.ItemSep + pAdSelectionDetails;
            }

            return SendRequestToMacOtpServer(macUrl + Constants.ServiceUrls.RequestOtpWebService, pClientId, requestData);
        }

        public string SendOtpToAdminUser(string pMacUrl, string pLogin, string pClientId)
        {
            return SendOtpToAdminUser(pMacUrl, pLogin, pClientId, null);
        }
        public string SendOtpToAdminUser(string macUrl, string login, string clientId, string pComment)
        {
            if (String.IsNullOrEmpty(clientId))
                clientId = Constants.Strings.DefaultClientId;

            var requestData = dk.Request + dk.KVSep + dv.SendOtpAdmin +
                              dk.ItemSep + dk.CID + dk.KVSep + clientId +
                              dk.ItemSep + dk.UserId + dk.KVSep + login +
                              dk.ItemSep + dkui.EndUserIpAddress + dk.KVSep + GetUserIp();

            // add comment if any
            if (String.IsNullOrEmpty(pComment) == false)
                requestData += dk.ItemSep + dk.Comment + dk.KVSep + StringToHex(pComment);

            return SendRequestToMacOtpServer(macUrl + Constants.ServiceUrls.RequestOtpWebService, clientId, requestData);
        }

        public string ResendOtp(string macUrl, string pClientId, string pRequestId)
        {
            var requestData = dk.Request + dk.KVSep + dv.ResendOtp +
                              dk.ItemSep + dk.CID + dk.KVSep + pClientId +
                              dk.ItemSep + dk.RequestId + dk.KVSep + pRequestId +
                              dk.ItemSep + dkui.EndUserIpAddress + dk.KVSep + GetUserIp();

            return SendRequestToMacOtpServer(macUrl + Constants.ServiceUrls.RequestOtpWebService, pClientId, requestData);
        }

        public string CancelOtp(string macUrl, string pClientId, string pRequestId)
        {
            var requestData = dk.Request + dk.KVSep + dv.CancelOtp +
                              dk.ItemSep + dk.CID + dk.KVSep + pClientId +
                              dk.ItemSep + dk.RequestId + dk.KVSep + pRequestId +
                              dk.ItemSep + dkui.EndUserIpAddress + dk.KVSep + GetUserIp();

            return SendRequestToMacOtpServer(macUrl + Constants.ServiceUrls.RequestOtpWebService, pClientId, requestData);
        }

        public string VerifyOtp(string macUrl, string pClientId, string pRequestId, string pOtpCode)
        {
            var requestData = dk.Request + dk.KVSep + dv.VerifyOtp +
                              dk.ItemSep + dk.CID + dk.KVSep + pClientId +
                              dk.ItemSep + dk.RequestId + dk.KVSep + pRequestId +
                              dk.ItemSep + dk.OTP + dk.KVSep + pOtpCode +
                              dk.ItemSep + dkui.EndUserIpAddress + dk.KVSep + GetUserIp();

            return SendRequestToMacOtpServer(macUrl + Constants.ServiceUrls.VerifyOtpWebService, pClientId, requestData);
        }

        public string SendMessageToClientUser(string macUrl, string pClientId, string pUserName, string pUserEmail, string pUserPhoneNumber, string pMessage)
        {
            if (Regex.IsMatch(pUserEmail, Constants.RegexStrings.EmailAddress) == false)
                return BadEmailAddress + pUserEmail;

            if (Regex.IsMatch(pUserPhoneNumber, Constants.RegexStrings.PhoneNumber) == false)
                return BadPhoneNumber + pUserPhoneNumber;

            var requestData = dk.Request + dk.KVSep + dv.SendMessage +
                  dk.ItemSep + dk.CID + dk.KVSep + pClientId +
                  dk.ItemSep + dkui.PhoneNumber + dk.KVSep + pUserPhoneNumber +
                  dk.ItemSep + dkui.EmailAddress + dk.KVSep + pUserEmail +
                  dk.ItemSep + dkui.EndUserIpAddress + dk.KVSep + GetUserIp() +
                  dk.ItemSep + dk.Message + dk.KVSep + StringToHex(pMessage);

            return SendRequestToMacOtpServer(macUrl + Constants.ServiceUrls.RequestOtpWebService, pClientId, requestData);
        }

        public string SendMessageToRegisteredUser(string macUrl, string pClientId, string pGroupId, string pUserId, string pMessage)
        {
            var requestData = dk.Request + dk.KVSep + dv.SendMessage +
                              dk.ItemSep + dk.CID + dk.KVSep + pClientId +
                              dk.ItemSep + dk.UserId + dk.KVSep + pUserId +
                              dk.ItemSep + dkui.EndUserIpAddress + dk.KVSep + GetUserIp() +
                              dk.ItemSep + dk.Message + dk.KVSep + StringToHex(pMessage);

            // Group Id if supplied
            if (String.IsNullOrEmpty(pGroupId) == false)
                requestData += dk.ItemSep + dk.GroupId + dk.KVSep + pGroupId;

            return SendRequestToMacOtpServer(macUrl + Constants.ServiceUrls.RequestOtpWebService, pClientId, requestData);
        }

        public string SendManageRegisteredUser(string macUrl, string pClientId, string pGroupId, string pUserId, string pRequest)
        {
            var requestData = pRequest +
                              dk.ItemSep + dk.CID + dk.KVSep + pClientId +
                              dk.ItemSep + dk.UserId + dk.KVSep + pUserId;

            // Group Id if supplied
            if (String.IsNullOrEmpty(pGroupId) == false)
                requestData += dk.ItemSep + dk.GroupId + dk.KVSep + pGroupId;

            return SendRequestToMacOtpServer(macUrl + Constants.ServiceUrls.EndUserManagementWebService, pClientId, requestData);
        }

        protected string SendRequestToMacOtpServer(string url, string id, string requestData)
        {
            var data = String.Format("data={0}{1}{2}", id.Length, id.ToUpper(), Security.EncryptAndEncode(requestData + dk.ItemSep + dk.API + dk.KVSep + dv.MSAPI, id.ToUpper()));
            try
            {
                var dataStream = Encoding.UTF8.GetBytes(data);

                // Reformat url protocol if running in SSL session
                //url = url.Replace("http://", "https://");

                var request = url;
                var webRequest = WebRequest.Create(request);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.ContentLength = dataStream.Length;
                var newStream = webRequest.GetRequestStream();
                // Send the data.
                newStream.Write(dataStream, 0, dataStream.Length);
                newStream.Close();
                var res = webRequest.GetResponse();
                var response = res.GetResponseStream();
                var xmlDoc = new XmlDocument();
                if (response != null)
                {
                    xmlDoc.Load(response);
                    //error
                    var elemList = xmlDoc.GetElementsByTagName(sr.Error);
                    if (elemList.Count != 0)
                    {
                        return elemList[0].InnerXml;
                        //var mErrorDesc = elemList[0].InnerXml;
                        //if (mErrorDesc.Contains("STOP"))
                        //    return mErrorDesc;
                    }

                    elemList = xmlDoc.GetElementsByTagName(sr.Debug);
                    var debugitems = "";
                    if (elemList.Count != 0)
                        debugitems = dk.ItemSep + elemList[0].InnerXml;
                    elemList = xmlDoc.GetElementsByTagName(sr.Reply);
                    return elemList[0].InnerXml + debugitems;
                }
                return "Error: null response!";
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        protected static string GetUserIp()
        {
            var uip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (String.IsNullOrEmpty(uip))
                uip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            return uip;
        }

        protected string HexToString(String input)
        { // data is encoded in hex, convert it back to a string
            if (String.IsNullOrEmpty(input)) return null;
            try
            {
                var sb = new StringBuilder();
                for (var i = 0; i < input.Length; i += 2)
                {
                    var hs = input.Substring(i, 2);
                    sb.Append(Convert.ToChar(Convert.ToUInt32(hs, 16)));
                }
                return sb.ToString();
            }
            catch
            {
                return null;
            }
        }

        protected static string StringToHex(String input)
        {
            try
            {
                var values = input.ToCharArray();
                var output = new StringBuilder();
                foreach (int value in values.Select(Convert.ToInt32))
                {
                    // Convert the decimal value to a hexadecimal value in string form. 
                    output.Append(String.Format("{0:X}", value));
                }
                return output.ToString();
            }
            catch
            {
                return null;
            }
        }
    } /* end  MacOtp class */
}

