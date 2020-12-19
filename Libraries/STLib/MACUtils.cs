using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using MACSecurity;
using stc = STLib.STConstants.MACConstantsToWrapperConstants;
using et = STLib.STConstants.EventTypes;
using strt = STLib.STConstants.ResponsesTemplates;
using ic = STLib.STConstants.MACInfoCodes;
using im = STLib.STConstants.MACInfoMessages;
using cfg = STLib.STConstants.Config;


namespace STLib
{
    public class MACUtils
    {
        #region MAC Service Call Methods
        public bool GetMACClientIdUsingOperatorIdAndSiteId(StiWrapperState pStiWrapperState, string pOperatorId, string pSiteId)
        {
            var stiServiceUrl =  ConfigurationManager.AppSettings[cfg.MacServicesUrl] + cfg.GetMACClientIdBySTSiteId;
            try
            {
                var request = (HttpWebRequest) WebRequest.Create(stiServiceUrl);
                var postData = "data=" + pOperatorId + "|" + pSiteId;
                var data = Encoding.ASCII.GetBytes(postData);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                var response = (HttpWebResponse) request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd().ToString();
                var myDoc = new XmlDocument();
                myDoc.LoadXml(responseString);
                var elemList = myDoc.GetElementsByTagName("clientid");
                if (elemList.Count != 0)
                {
                    pStiWrapperState.macClientId = elemList[0].InnerXml;
                    elemList = myDoc.GetElementsByTagName("clientname");
                    if (elemList.Count != 0)
                    {
                        pStiWrapperState.macClientName = elemList[0].InnerXml;
                    }
                    return true;
                }

            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch(Exception ex)
            {
                var errMsg = ex.ToString();
            }
            return false;
        }

        public Tuple<bool, string> MACRegisterPlayer(StiWrapperState pStiWrapperState, string pMethodName)
        {
            var mMACRequest = new StringBuilder();
            mMACRequest.Append(stc.Request + stc.KVSep + stc.EndUserRegister);
            mMACRequest.Append(stc.ItemSep + stc.CID + stc.KVSep + pStiWrapperState.macClientId);
            mMACRequest.Append(stc.ItemSep + stc.RegistrationType + stc.KVSep + stc.ClientRegister);

            try
            {
                dynamic mResponseData = JObject.Parse(pStiWrapperState.stResponseJson);
                JObject playerDetails = mResponseData.playerDetails;
                var Property = playerDetails.Property("userName");
                if (Property == null)
                    return new Tuple<bool, string>(false, "MACRegisterPlayer, invalid no userName");
                string userName = Property.Value.ToString();
                if (String.IsNullOrEmpty(userName))
                    return new Tuple<bool, string>(false, "MACRegisterPlayer, invalid userName");
                mMACRequest.Append(stc.ItemSep + stc.UserId + stc.KVSep + userName);

                Property = playerDetails.Property("firstName");
                if (Property == null)
                    return new Tuple<bool, string>(false, "MACRegisterPlayer, invalid no first name");
                string fn = Property.Value.ToString();
                if (String.IsNullOrEmpty(fn))
                    return new Tuple<bool, string>(false, "MACRegisterPlayer, invalid first name");
                mMACRequest.Append(stc.ItemSep + stc.FirstName + stc.KVSep + fn);

                Property = playerDetails.Property("lastName");
                if (Property == null)
                    return new Tuple<bool, string>(false, "MACRegisterPlayer, invalid no last name");
                string ln = Property.Value.ToString();
                if (String.IsNullOrEmpty(ln))
                    return new Tuple<bool, string>(false, "MACRegisterPlayer, invalid last name");
                mMACRequest.Append(stc.ItemSep + stc.LastName + stc.KVSep + ln);

                Property = playerDetails.Property("mobileNo");
                if (Property == null) 
                    return new Tuple<bool, string>(false, "MACRegisterPlayer, invalid no mobile No");
                Tuple<bool, string> rtn = CheckAndEditPhoneNumber(Property.Value.ToString());
                if (rtn.Item1 == false)
                    return rtn;
                mMACRequest.Append(stc.ItemSep + stc.PhoneNumber + stc.KVSep + rtn.Item2);

                Property = playerDetails.Property("emailAddress");
                if (Property == null) 
                    return new Tuple<bool, string>(false, "MACRegisterPlayer, invalid no email Address");
                var em = Property.Value.ToString();
                if (ValidateEmailAddress(em) == false)
                    return new Tuple<bool, string>(false, "MACRegisterPlayer, invalid email address");
                mMACRequest.Append(stc.ItemSep + stc.EmailAddress + stc.KVSep + em);

                Property = playerDetails.Property("middleInitial");
                if (Property != null)
                    mMACRequest.Append(stc.ItemSep + stc.MiddleName + stc.KVSep + Property.Value.ToString());

                Property = playerDetails.Property("dob");
                if (Property != null)
                    mMACRequest.Append(stc.ItemSep + stc.DOB + stc.KVSep + Property.Value.ToString());

                Property = playerDetails.Property("playerAddress1");
                if (Property != null)
                    mMACRequest.Append(stc.ItemSep + stc.Street + stc.KVSep + Property.Value.ToString());

                Property = playerDetails.Property("playerAddress2");
                if (Property != null)
                    mMACRequest.Append(stc.ItemSep + stc.Street2 + stc.KVSep + Property.Value.ToString());

                Property = playerDetails.Property("city");
                if (Property != null)
                    mMACRequest.Append(stc.ItemSep + stc.City + stc.KVSep + Property.Value.ToString());

                Property = playerDetails.Property("ssn");
                if (Property != null)
                    mMACRequest.Append(stc.ItemSep + stc.SSN4 + stc.KVSep + Property.Value.ToString());

                Property = playerDetails.Property("state");
                if (Property != null)
                    mMACRequest.Append(stc.ItemSep + stc.State + stc.KVSep + Property.Value.ToString());

                Property = playerDetails.Property("zipCode");
                if (Property != null)
                    mMACRequest.Append(stc.ItemSep + stc.ZipCode + stc.KVSep + Property.Value.ToString());

                Property = playerDetails.Property("dlNumber");
                if (Property != null)
                    mMACRequest.Append(stc.ItemSep + stc.DriverLic + stc.KVSep + Property.Value.ToString());

                Property = playerDetails.Property("dlIssuingState");
                if (Property != null)
                    mMACRequest.Append(stc.ItemSep + stc.DriverLicSt + stc.KVSep + Property.Value.ToString());

              mMACRequest.Append(stc.ItemSep + stc.API + stc.KVSep + "STW");
            }
            catch (Exception ex)
            {
                return new Tuple<bool, string>(false, "MACRegisterPlayer, Exception: " + ex.Message);
            }
            return SendRequestToMac(pStiWrapperState, stc.SecureTraidingRegisterUserWebService, mMACRequest.ToString(), pMethodName + " - " + stc.EndUserRegister);
        }

        public Tuple<bool, string> MACRequestOTP_UnregisteredPlayer(StiWrapperState pStiWrapperState, string pMethodName)
        {
            string mMACRequest;
            try
            {
                dynamic mResponseData = JObject.Parse(pStiWrapperState.stRequestJson);
                Tuple<bool, string> rtn = CheckAndEditPhoneNumber(mResponseData.playerDetails.mobileNo.ToString());
                if (rtn.Item1 == false)
                    return rtn;
                var mn = rtn.Item2;
                var em = mResponseData.playerDetails.emailAddress.ToString();
                if (ValidateEmailAddress(em) == false)
                    return new Tuple<bool, string>(false, "invalid email address");

                mMACRequest = stc.Request + stc.KVSep + stc.SendOtp
                    + stc.ItemSep + stc.CID + stc.KVSep + pStiWrapperState.macClientId
                    + stc.ItemSep + stc.PhoneNumber + stc.KVSep + mn
                    + stc.ItemSep + stc.EmailAddress + stc.KVSep + em
                    + stc.ItemSep + stc.EndUserIpAddress + stc.KVSep + pStiWrapperState.stPlayerIp
                    + stc.ItemSep + stc.TrxType + stc.KVSep + "3"
                    + stc.ItemSep + stc.API + stc.KVSep + "STW";
            }
            catch (Exception ex)
            {
                return new Tuple<bool, string>(false, "Exception: " + ex.Message);
            }
            return SendRequestToMac(pStiWrapperState, stc.RequestOtpWebService, mMACRequest, pMethodName + " - " + stc.SendOtp);
        }

        public Tuple<bool, string> MACRequestOTP_RegisteredPlayer(StiWrapperState pStiWrapperState, string pMethodName, int pType, String pDetails)
        {
            var mMACRequest = stc.Request + stc.KVSep + stc.SendOtp
                              + stc.ItemSep + stc.CID + stc.KVSep + pStiWrapperState.macClientId
                              + stc.ItemSep + stc.UserId + stc.KVSep + pStiWrapperState.stUserName
                              + stc.ItemSep + stc.EndUserIpAddress + stc.KVSep + pStiWrapperState.stPlayerIp
                              + stc.ItemSep + stc.TrxType + stc.KVSep + pType;

            if (!String.IsNullOrEmpty(pDetails))
                mMACRequest += stc.ItemSep + stc.TrxDetails + stc.KVSep + StringToHex(pDetails);

            mMACRequest += stc.ItemSep + stc.API + stc.KVSep + "STW";

            return SendRequestToMac(pStiWrapperState, stc.RequestOtpWebService, mMACRequest, pMethodName + " - " + stc.SendOtp);
        }

        public Tuple<bool, string> MACVerifyOTP(StiWrapperState pStiWrapperState, string pMethodName)
        {
            var mMACRequest = stc.Request + stc.KVSep + stc.VerifyOtp
                  + stc.ItemSep + stc.CID + stc.KVSep + pStiWrapperState.macClientId
                  + stc.ItemSep + stc.RequestId + stc.KVSep + pStiWrapperState.macRequestId
                  + stc.ItemSep + stc.OTP + stc.KVSep + pStiWrapperState.macOTP
                  //+ stc.ItemSep + stc.EndUserIpAddress + stc.KVSep + pStiWrapperState.stPlayerIp
                  + stc.ItemSep + stc.API + stc.KVSep + "STW";
            return SendRequestToMac(pStiWrapperState, stc.VerifyOtpWebService, mMACRequest, pMethodName + " - " + stc.VerifyOtp);
        }

        public Tuple<bool, string> MACResendOTP(StiWrapperState pStiWrapperState, string pMethodName)
        {
            var mMACRequest = stc.Request + stc.KVSep + stc.ResendOtp
                              + stc.ItemSep + stc.CID + stc.KVSep + pStiWrapperState.macClientId
                              + stc.ItemSep + stc.RequestId + stc.KVSep + pStiWrapperState.macRequestId
                              + stc.ItemSep + stc.EndUserIpAddress + stc.KVSep + pStiWrapperState.stPlayerIp
                              + stc.ItemSep + stc.API + stc.KVSep + "STW";
            return SendRequestToMac(pStiWrapperState, stc.VerifyOtpWebService, mMACRequest, pMethodName + " - " + stc.ResendOtp);
        }

        public Tuple<bool, string> MACEndUserRegisteredByUserName(StiWrapperState pStiWrapperState, string pMethodName)
        {
            var mMACRequest = stc.Request + stc.KVSep + stc.CheckEndUserRegistration
                              + stc.ItemSep + stc.CID + stc.KVSep + pStiWrapperState.macClientId
                              + stc.ItemSep + stc.UserId + stc.KVSep + pStiWrapperState.stUserName
                              + stc.ItemSep + stc.API + stc.KVSep + "STW";
            return SendRequestToMac(pStiWrapperState, stc.MacEndUserManagement, mMACRequest, pMethodName + " - " + stc.CheckEndUserRegistration);
        }

        public Tuple<bool, string> MACDeleteEndUserByUserId(StiWrapperState pStiWrapperState, string pMethodName)
        {
            var mMACRequest = stc.Request + stc.KVSep + stc.DeleteEndUser
                              + stc.ItemSep + stc.CID + stc.KVSep + pStiWrapperState.macClientId
                              + stc.ItemSep + stc.UserId + stc.KVSep + pStiWrapperState.stUserName
                              + stc.ItemSep + stc.API + stc.KVSep + "STW";
            return SendRequestToMac(pStiWrapperState, stc.MacEndUserManagement, mMACRequest, pMethodName + " - " + stc.DeleteEndUser);
        }

        protected Tuple<bool, string> SendRequestToMac(StiWrapperState pStiWrapperState, string pServiceUrl, String pRequestData, string pMethodName)
        {
            var mUrl = ConfigurationManager.AppSettings[cfg.MacServicesUrl] + pServiceUrl;
            EventLogMACRequest(pStiWrapperState, mUrl + pServiceUrl, pRequestData);
            var data = String.Format("data={0}{1}{2}", pStiWrapperState.macClientId.Length, pStiWrapperState.macClientId.ToUpper(),
                Security.EncryptAndEncode(pRequestData, pStiWrapperState.macClientId.ToUpper()));
            var ReturnJson = new JObject();
            try
            {
                var dataStream = Encoding.UTF8.GetBytes(data);
                var request = mUrl;
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
                    return new Tuple<bool, string>(false, "no response from MAC service!");

                xmlDoc.Load(response);
                var elemList = xmlDoc.GetElementsByTagName(stc.Error);
                if (elemList.Count != 0)
                    return new Tuple<bool, string>(false, elemList[0].InnerXml);

                if (pMethodName.Contains("CkEndUserReg"))
                {
                    bool Registered = false;
                    elemList = xmlDoc.GetElementsByTagName("Groups");
                    if (elemList.Count != 0)
                    {
                        if (elemList[0].InnerXml != "None")
                            Registered = true;
                    }
                    elemList = xmlDoc.GetElementsByTagName("Client");
                    if (elemList.Count != 0)
                    {
                        if (elemList[0].InnerXml != "None")
                            Registered = true;
                    }
                    elemList = xmlDoc.GetElementsByTagName("Open");
                    if (elemList.Count != 0)
                    {
                        if (elemList[0].InnerXml != "Not Open Registered")
                            Registered = true;
                    }
                    return Registered ? new Tuple<bool, string>(true, "Registered") : new Tuple<bool, string>(false, "Not registered");
                }



                elemList = xmlDoc.GetElementsByTagName(stc.Reply);
                if (elemList.Count != 0)
                {
                    var mReply = elemList[0].InnerXml;
                    if (mReply.Contains(stc.ItemSep) == false)
                    {
                        ReturnJson.Add(stc.Reply, mReply);
                    }
                    else
                    {  // compound string with multiple items seperated by ItemSep
                        // parse into Json
                        var mFields = mReply.Split(char.Parse(stc.ItemSep));
                        if (mFields.Any())
                        {
                            foreach (var field in mFields)
                            {
                                if (field.Contains(stc.KVSep) == false) continue;
                                var keyValue = field.Split(char.Parse(stc.KVSep));
                                var key = keyValue[0];
                                if (String.IsNullOrEmpty(key)) continue;
                                var value = field.Replace(key + stc.KVSep, "");
                                ReturnJson.Remove(stc.Reply);
                                ReturnJson.Add(key, value.Trim());
                            }
                        }
                    }
                    // requestId as seperate element
                    elemList = xmlDoc.GetElementsByTagName(stc.RequestId);
                    if (elemList.Count != 0)
                    {
                        ReturnJson.Remove(stc.RequestId);
                        ReturnJson.Add(stc.RequestId, elemList[0].InnerXml);
                    }
                    elemList = xmlDoc.GetElementsByTagName(stc.Debug);
                    if (elemList.Count != 0)
                    {
                        var debugotp = elemList[0].InnerXml;
                        if (debugotp.Contains(stc.OTP + ":"))
                        {
                            ReturnJson.Remove(stc.OTP);
                            ReturnJson.Add(stc.OTP, debugotp.Replace(stc.OTP + ":", ""));
                        }
                    }
                }
                elemList = xmlDoc.GetElementsByTagName(stc.Details);
                if (elemList.Count != 0)
                { // compound string with multiple items seperated by ItemSep
                    // parse into Json
                    var details = elemList[0].InnerXml;
                    var mDetails = details.Split(char.Parse(stc.ItemSep));
                    if (mDetails.Any())
                    {
                        foreach (var mDetail in mDetails)
                        {
                            if (mDetail.Contains(stc.KVSep) == false) continue;
                            var keyValue = mDetail.Split(char.Parse(stc.KVSep));
                            var key = keyValue[0];
                            if (String.IsNullOrEmpty(key)) continue;
                            var value = mDetail.Replace(key + stc.KVSep, "");
                            ReturnJson.Remove(key);
                            ReturnJson.Add(key, value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new Tuple<bool, string>(false, "SendRequestToMac.Exception: " + ex.Message);
            }
            var rtn = JsonConvert.SerializeObject(ReturnJson);
            EventLogMACResponse(pStiWrapperState, rtn);
            return new Tuple<bool, string>(true, rtn);
        }

        #endregion

        #region Wrapper State Methods

        public StiWrapperState NewWrapperState(string mRequestData, string pMethodName)
        {
            dynamic myRequestJson = JObject.Parse(mRequestData);
            var mStiWrapperState = new StiWrapperState(String.Empty);
            mStiWrapperState._t = "WrapperState";
            mStiWrapperState.methodName = pMethodName;
            var OIDProperty = myRequestJson.operatorId;
            mStiWrapperState.stOperatorId = OIDProperty;
            var SIDProperty = myRequestJson.siteId;
            mStiWrapperState.stSiteId = SIDProperty;
            mStiWrapperState.stRequestJson = mRequestData;
            //todo: mStiWrapperState.stTimeStamp = Convert.ToDateTime(stTimeStamp);
            return mStiWrapperState;
        }

        #endregion

        #region EventLog Methods

        public void EventLogOperRequest(StiWrapperState pStiWrapperState)
        {
            var mEvent = new StiEvent();
            mEvent.macType = et.OperRequestWrapper + " - " + pStiWrapperState.methodName;
            mEvent.stJsonData = pStiWrapperState.stRequestJson;
            mEvent.stOperatorId = pStiWrapperState.stOperatorId;
            mEvent.stSiteId = pStiWrapperState.stSiteId;
            mEvent.macClientId = pStiWrapperState.macClientId;
            pStiWrapperState.Events.Add(mEvent);
            pStiWrapperState.Save(pStiWrapperState);
        }

        public void EventLogMACRequest(StiWrapperState pStiWrapperState, String pUrl, String pRequestData)
        {
            var mEvent = new StiEvent();
            mEvent.macType = et.WrapperRequestMAC + " - " + pStiWrapperState.methodName;
            mEvent.macData = "Url:" + pUrl + "!Data:" + pRequestData;
            mEvent.stOperatorId = pStiWrapperState.stOperatorId;
            mEvent.stSiteId = pStiWrapperState.stSiteId;
            mEvent.macClientId = pStiWrapperState.macClientId;
            pStiWrapperState.Events.Add(mEvent);
            pStiWrapperState.Save(pStiWrapperState);
        }

        public void EventLogMACResponse(StiWrapperState pStiWrapperState, String pResponseData)
        {
            var mEvent = new StiEvent();
            mEvent.macType = et.MACResponseWrapper + " - " + pStiWrapperState.methodName;
            mEvent.macData = pResponseData;
            mEvent.stOperatorId = pStiWrapperState.stOperatorId;
            mEvent.stSiteId = pStiWrapperState.stSiteId;
            mEvent.macClientId = pStiWrapperState.macClientId;
            pStiWrapperState.Events.Add(mEvent);
            pStiWrapperState.Save(pStiWrapperState);
        }

        //public void xEventLogSTRequest(StiWrapperState pStiWrapperState)
        //{
        //    var mEvent = new StiEvent();
        //    if (pStiWrapperState.stProcess == "ST")
        //        mEvent.macType = et.WrapperRequestST + " - " + pStiWrapperState.methodName;
        //    else
        //        mEvent.macType = et.WrapperRequestLB + " - " + pStiWrapperState.methodName;
        //    mEvent.macType = et.WrapperRequestST + " - " + pStiWrapperState.methodName;
        //    mEvent.macData = pStiWrapperState.stRequestJson;  
        //    mEvent.stOperatorId = pStiWrapperState.stOperatorId;
        //    mEvent.stSiteId = pStiWrapperState.stSiteId;
        //    mEvent.macClientId = pStiWrapperState.macClientId;
        //    pStiWrapperState.Events.Add(mEvent);
        //    pStiWrapperState.Save(pStiWrapperState);
        //}

        //public void xEventLogSTResponse(StiWrapperState pStiWrapperState)
        //{
        //    var mEvent = new StiEvent();
        //    if (pStiWrapperState.stProcess == "ST")
        //        mEvent.macType = et.STResponseWrapper + " - " + pStiWrapperState.methodName;
        //    else
        //        mEvent.macType = et.LoopbackResponse + " - " + pStiWrapperState.methodName;
        //    mEvent.stJsonData = pStiWrapperState.stRequestJson;
        //    mEvent.stOperatorId = pStiWrapperState.stOperatorId;
        //    mEvent.stSiteId = pStiWrapperState.stSiteId;
        //    mEvent.macClientId = pStiWrapperState.macClientId;
        //    pStiWrapperState.Events.Add(mEvent);
        //    pStiWrapperState.Save(pStiWrapperState);
        //}

        #endregion

        #region Operator Response Methods

        public string SendOperatorResponse_PlayerDeleted(String pCode, String pText)
        {
            return strt.MACSuccessTemplates
                .Replace("[EC]", pCode)
                .Replace("[ET]", pText)
                .Replace("[DT]", getSTFormatedDate());
        }

        public string SendOperatorResponse_SystemError(StiWrapperState pStiWrapperState, String pErrorCode, String pErrorText)
        {
            var Data = strt.SystemErrorTemplates
                .Replace("[EC]", pErrorCode)
                .Replace("[ET]", pErrorText)
                .Replace("[DT]", getSTFormatedDate());

            if (pStiWrapperState != null)
            {
                var mEvent = new StiEvent
                {
                    macStateId = pStiWrapperState._id,
                    macType = et.WrapperResponesOper,
                    stOperatorId = pStiWrapperState.stOperatorId,
                    stSiteId = pStiWrapperState.stSiteId,
                    macClientId = pStiWrapperState.macClientId,
                    stJsonData = pStiWrapperState.stRequestJson,
                    macData = Data
                };
                pStiWrapperState.Events.Add(mEvent);
                pStiWrapperState.Save(pStiWrapperState);
            }
            return Data;
        }

        public string SendOperatorResponse_RequestOTP(StiWrapperState pStiWrapperState)
        {
            var RequestOTPResponse =
                "{"
                + "\"returnCode\":\"" + ic.EnterOTP + "\","
                + "\"returnMsg\":\"" + im.EnterOTP + "\","
                + "\"responseToken\":\"" + pStiWrapperState._id + "\","
                + "\"stTimeStamp\":\"" + getSTFormatedDate() + "\"";
            
            if (String.IsNullOrEmpty(pStiWrapperState.macOTP) == false)
                RequestOTPResponse += ", \"" + stc.OTP + "\":\" " + pStiWrapperState.macOTP.Trim() + "\"";

            if (String.IsNullOrEmpty(pStiWrapperState.macEnterOtpAd) == false)
                RequestOTPResponse += ",\"" + stc.EnterOTPAd + "\":\" " + pStiWrapperState.macEnterOtpAd.Trim() + "\"";

            RequestOTPResponse += "}";

            var mEvent = new StiEvent
            {
                macStateId = pStiWrapperState._id,
                macType = et.WrapperResponesOper,
                stOperatorId = pStiWrapperState.stOperatorId,
                stSiteId = pStiWrapperState.stSiteId,
                macClientId = pStiWrapperState.macClientId,
                stJsonData = RequestOTPResponse,
            };
            pStiWrapperState.Events.Add(mEvent);
            pStiWrapperState.Save(pStiWrapperState);
            return RequestOTPResponse;
        }

        public string SendOperatorResponse_ReenterOTP(StiWrapperState pStiWrapperState, String pErrorMessage)
        {
            var RequestOTPResponse =
                           "{"
                           + "\"responseToken\":\"" + pStiWrapperState._id.ToString() + "\","
                           + "\"returnCode\":\"" + ic.Re_enter + "\","
                           + "\"returnMsg\":\"" + im.Re_enter + "\","
                           + "\"stTimeStamp\":\"" + getSTFormatedDate() + "\",";

            if (String.IsNullOrEmpty(pStiWrapperState.macEnterOtpAd) == false)
                RequestOTPResponse += "\"" + stc.EnterOTPAd + "\":\" " + pStiWrapperState.macEnterOtpAd + "\",";

            RequestOTPResponse += "\"playerDetails\":"
                    + "{"
                        + "\"operatorId\":\"" + pStiWrapperState.stOperatorId + "\","
                        + "\"siteId\":\"" + pStiWrapperState.stSiteId + "\","
                        + "\"stPlayerId\":\"" + pStiWrapperState.stPlayerId + "\","
                        + "\"stReferenceNo\":\"" + pStiWrapperState.stReferenceNumber + "\""
                    + "}"
                + "}";
            var mEvent = new StiEvent
            {
                macStateId = pStiWrapperState._id,
                macType = et.WrapperResponesOper,
                stOperatorId = pStiWrapperState.stOperatorId,
                stSiteId = pStiWrapperState.stSiteId,
                macClientId = pStiWrapperState.macClientId,
                stJsonData = RequestOTPResponse,
            };
            pStiWrapperState.Events.Add(mEvent);
            pStiWrapperState.Save(pStiWrapperState);
            return RequestOTPResponse;
        }

        public string SendOperatorResponse_OTPError(StiWrapperState pStiWrapperState, string pErrorCode, String pErrorMessage)
        {

           var mResponse = "{"
               + "\"returnCode\":\"" + pErrorCode + "\","
               + "\"returnMsg\":\"" + pErrorMessage + "\","
               + "\"stTimeStamp\":\"" + getSTFormatedDate() + "\""
           + "}";
            var mEvent = new StiEvent
            {
                macStateId = pStiWrapperState._id,
                macType = et.WrapperResponesOper,
                stOperatorId = pStiWrapperState.stOperatorId,
                stSiteId = pStiWrapperState.stSiteId,
                macClientId = pStiWrapperState.macClientId,
                stJsonData = mResponse,
            };
            pStiWrapperState.Events.Add(mEvent);
            pStiWrapperState.Save(pStiWrapperState);
            return mResponse;
        }

        public string SendOperatorResponse_StSuccessResponse(StiWrapperState pStiWrapperState)
        {
            // send ST's Success response save 
            var mStSuccessResponse = pStiWrapperState.stResponseJson;
            var mEvent = new StiEvent
            {
                macStateId = pStiWrapperState._id,
                macType = et.WrapperResponesOper,
                stOperatorId = pStiWrapperState.stOperatorId,
                stSiteId = pStiWrapperState.stSiteId,
                macClientId = pStiWrapperState.macClientId,
                stJsonData = mStSuccessResponse,
            };
            pStiWrapperState.Events.Add(mEvent);
            pStiWrapperState.Save(pStiWrapperState);
            return mStSuccessResponse;
        }

        #endregion

        #region Helper Methods

        private Tuple<bool, string> CheckAndEditPhoneNumber(string pPhoneNumber)
        {
            if (String.IsNullOrEmpty(pPhoneNumber))
                return new Tuple<bool, string>(false, "EditPhoneNumber1, invalid mobile No(null)");

            if (Regex.IsMatch(pPhoneNumber, stc.regX_PhoneNumber) == false)
                return new Tuple<bool, string>(false, "EditPhoneNumber2, invalid mobile No(format)," + pPhoneNumber);

            // remove formatting characters
            var mn = pPhoneNumber.Replace("-", "").Replace(".", "").Replace("(", "").Replace(")", "").Replace(" ", "");
            if (mn.Length < 10)
                return new Tuple<bool, string>(false, "EditPhoneNumber3, invalid mobile No(length)," + mn);

            // get the last 10 digits (area code 3, exchange 3, number 4
            mn = mn.Substring(mn.Length - 10, 10);

            // is it for a valid area code
            if (ValidAreaCode(mn.Substring(0,3)) == false)
                return new Tuple<bool, string>(false, "EditPhoneNumber4, invalid mobile No(area code)," + mn.Substring(0, 3));
            return new Tuple<bool, string>(true, mn);
        }
        
        public string GetCurrentMethod()
        {
            var st = new StackTrace();
            var sf = st.GetFrame(1);
            return sf.GetMethod().Name;
        }

        public string getSTFormatedDate()
        {
            /*
           ISO Date Time Format:
           Complete date plus hours, minutes, seconds and a decimal fraction of a second
              YYYY-MM-DDThh:mm:ss.sTZD (eg 1997-07-16T19:20:30.45+01:00)
            where:
                 YYYY = four-digit year
                 MM   = two-digit month (01=January, etc.)
                 DD   = two-digit day of month (01 through 31)
                 hh   = two digits of hour (00 through 23) (am/pm NOT allowed)
                 mm   = two digits of minute (00 through 59)
                 ss   = two digits of second (00 through 59)
                 s    = one or more digits representing a decimal fraction of a second
                 TZD  = time zone designator (Z or +hh:mm or -hh:mm)
            */
            return DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz");
        }

        public string StringToHex(String input)
        {
            if (String.IsNullOrEmpty(input)) return string.Empty;
            try
            {
                var values = input.ToCharArray();
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
                return string.Empty;
            }
        }

        public string HexToString(String input)
        { // data is encoded in hex, convert it back to a string
            if (String.IsNullOrEmpty(input)) return string.Empty;
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
                return string.Empty;
            }
        }

        protected bool ValidAreaCode(string pMobilePhoneNumber)
        {
            //todo: add arrea code check here
            return true;
        }

        public bool ValidateEmailAddress(string pEmailAddress)
        {
            return Regex.IsMatch(pEmailAddress, stc.regX_EmailAddress);
        }

        #endregion
    }
}
