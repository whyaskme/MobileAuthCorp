using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Xml;

using MACSecurity;
using MACServices;
using dk = MACServices.Constants.Dictionary.Keys;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using dv = MACServices.Constants.Dictionary.Values;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using sr = MACServices.Constants.ServiceResponse;

namespace MacRegistration
{
    public class MacRegistration
    {
        public string RegisterEndUser(string macServicesUrl, string pCid, string userInfo)
        {
            if (String.IsNullOrEmpty(pCid))
                return "Error: Client Id required!";

            if (!userInfo.Contains(dk.RegistrationType))
                return "Error: Registration Type required!";

            if (!userInfo.Contains(dkui.FirstName))
                return "Error: User's first name required!";

            if (!userInfo.Contains(dkui.LastName))
                return "Error: User's last name required!";

            if (!userInfo.Contains(dkui.PhoneNumber))
                return "Error: User's phone number required!";

            if (!userInfo.Contains(dkui.EmailAddress))
                return "Error: User's EmailvAddress required!";

            return SendRequestToMacRegistrationServer(
                macServicesUrl + Constants.ServiceUrls.RegisteredUserWebService, 
                pCid,
                dk.Request + dk.KVSep + dv.EndUserRegister + dk.ItemSep + userInfo);
        }
        public string DeactivateEndUser(string macUrl, string pCid, string pEndUserLastName, String pEndUserUniqueIdentifier)
        {
            if (String.IsNullOrEmpty(pCid))
                return "Error: Client Id is required!";

            if (String.IsNullOrEmpty(pEndUserLastName))
                return "Error: User's last name is required!";

            if (String.IsNullOrEmpty(pEndUserUniqueIdentifier))
                return "Error: User's unique identifier is required!";

            return SendRequestToMacRegistrationServer(
                macUrl + Constants.ServiceUrls.EndUserManagementWebService, 
                pCid,
                dk.Request + dk.KVSep + dv.DeactivateEndUser +
                dk.ItemSep + dk.CID + dk.KVSep + pCid +
                dk.ItemSep + dk.UserId + dk.KVSep + Security.GetHashString(
                                        pEndUserLastName.ToLower() + 
                                        pEndUserUniqueIdentifier.ToLower()).ToUpper());
        }
        public string ActivateEndUser(string macUrl, string pCid, string pEndUserLastName, String pEndUserUniqueIdentifier)
        {
            if (String.IsNullOrEmpty(pCid))
                return "Error: Client Id required!";

            if (String.IsNullOrEmpty(pEndUserUniqueIdentifier))
                return "Error: User's Unique Identifier required!";

            if (String.IsNullOrEmpty(pEndUserLastName))
                return "Error: User's Last Name required!";

            return SendRequestToMacRegistrationServer(
                macUrl + Constants.ServiceUrls.EndUserManagementWebService, 
                pCid,
                dk.Request + dk.KVSep + dv.ActivateEndUser +            
                dk.ItemSep + dk.CID + dk.KVSep + pCid +
                dk.ItemSep + dk.UserId + dk.KVSep + Security.GetHashString(
                                pEndUserLastName.ToLower() + pEndUserUniqueIdentifier.ToLower()).ToUpper());
        }
        public string CompleteRegistration(string macUrl, string request, string pUserId, string pCid, string pRegType, string p1, string p2)
        {
            return SendRequestToMacRegistrationServer(
                macUrl + Constants.ServiceUrls.EndUserCompleteRegistrationWebService,
                pCid,
                dk.Request+ dk.KVSep + request +
                dk.ItemSep + dk.UserId + dk.KVSep + pUserId +
                dk.ItemSep + dk.CID + dk.KVSep + pCid +
                dk.ItemSep + dk.RegistrationType + dk.KVSep + pRegType +
                dk.ItemSep + dk.RequestId + dk.KVSep + p1 +
                dk.ItemSep + dk.OTP + dk.KVSep + p2);
        }

        public string EndUserFileRegistration(string macUrl, string pCid, string pGid, string rt, string fileType, string path, string fileName)
        {
            if (String.IsNullOrEmpty(pCid))
                return "Error: Client Id required!";

            if (String.IsNullOrEmpty(fileName))
                return "Error: File Name required!";

            if (String.IsNullOrEmpty(fileType))
                return "Error: File type required!";

            if (String.IsNullOrEmpty(path))
                return "Error: File path required!";

            // validate
            if (rt.ToLower().Contains("open") == false)
                if (rt.ToLower().Contains("group") == false)
                    if (rt.ToLower().Contains("client") == false)
                        return "Error: invalid registration type";

            var userInfo = dk.Request + dk.KVSep + dv.FileRegister +
                           dk.ItemSep + dk.CID + dk.KVSep + pCid;

            if (rt == dv.GroupRegister)
            {
                if (String.IsNullOrEmpty(pGid))
                    return "Error: no group";
                userInfo += dk.ItemSep + dk.GroupId + dk.KVSep + pGid;
            }

            userInfo += dk.ItemSep + dk.RegistrationType + dk.KVSep + rt +
                           dk.ItemSep + dk.FileName + dk.KVSep + fileName +
                           dk.ItemSep + dk.FileType + dk.KVSep + fileType +
                           dk.ItemSep + dk.UploadFolder + dk.KVSep + StringToHex(path);
            
            return SendRequestToMacRegistrationServer(
                macUrl + Constants.ServiceUrls.EndUserFileRegistrationWebService, pCid, userInfo);
        }

        public string StsRegisterEndUser(string macServicesUrl, string pCid, string userInfo)
        {
            if (String.IsNullOrEmpty(pCid))
                return "Error: Client Id required!";

            if (!userInfo.Contains(dk.RegistrationType))
                return "Error: Registration Type required!";

            if (!userInfo.Contains(dkui.FirstName))
                return "Error: User's first name required!";

            if (!userInfo.Contains(dkui.LastName))
                return "Error: User's last name required!";

            if (!userInfo.Contains(dkui.PhoneNumber))
                return "Error: User's phone number required!";

            if (!userInfo.Contains(dkui.EmailAddress))
                return "Error: User's email address required!";

            if (!userInfo.Contains(dk.UserId))
                return "Error: User Id required!";

            return SendRequestToMacRegistrationServer(
                macServicesUrl + Constants.ServiceUrls.SecureTraidingRegisterUserWebService,
                pCid,
                dk.Request + dk.KVSep + dv.EndUserRegister + dk.ItemSep + userInfo);
        }

        private string SendRequestToMacRegistrationServer(string url, string id, String requestData)
        {
            var data = String.Format("data={0}{1}{2}", id.Length, id.ToUpper(), Security.EncryptAndEncode(requestData + dk.ItemSep + dk.API + dk.KVSep + dv.MSAPI, id.ToUpper()));
            try
            {
                var dataStream = Encoding.UTF8.GetBytes(data);
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
                if (response == null)
                    return "Error=NoResponse";
                xmlDoc.Load(response);
                var elemList = xmlDoc.GetElementsByTagName(sr.Error);
                if (elemList.Count != 0)
                    return "Error=" + elemList[0].InnerXml;

                var rtn = "";
                elemList = xmlDoc.GetElementsByTagName(sr.Reply);
                if (elemList.Count != 0)
                    rtn += dk.ItemSep + "Reply=" + elemList[0].InnerXml;

                elemList = xmlDoc.GetElementsByTagName("Action");
                if (elemList.Count != 0)
                    rtn += dk.ItemSep + "Action=" + elemList[0].InnerXml;

                elemList = xmlDoc.GetElementsByTagName(sr.RequestId);
                if (elemList.Count != 0)
                    rtn += dk.ItemSep + "RequestId=" + elemList[0].InnerXml;

                elemList = xmlDoc.GetElementsByTagName(sr.Details);
                if (elemList.Count != 0)
                    rtn += dk.ItemSep + "Details=" + elemList[0].InnerXml;

                elemList = xmlDoc.GetElementsByTagName(sr.Debug);
                if (elemList.Count != 0)
                {
                    rtn += dk.ItemSep + "Debug=" + elemList[0].InnerXml;
                }

                return rtn.Trim(char.Parse(dk.ItemSep));
            }
            catch (Exception ex)
            {
                return "Error=" + ex.Message;
            }
        }

        public string RegisterOasClient(string macServicesUrl, string pCid, string registrationType)
        {
            return SendRequestToMacRegistrationServer(
                macServicesUrl + Constants.ServiceUrls.MacOpenClientServices,
                pCid,
                registrationType);
        }

        #region Helper methods
        public string StringToHex(String input)
        {
            try
            {
                char[] values = input.ToCharArray();
                var output = new StringBuilder();
                foreach (var value in values.Select(Convert.ToInt32))
                {
                    // This eliminates linebreak/carriage return issues
                    if (value < 32) continue;
                    if (value > 126) continue;
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
        #endregion
    }
}
