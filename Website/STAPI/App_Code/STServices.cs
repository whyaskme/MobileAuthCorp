using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using Newtonsoft.Json.Linq;

using STLib;
using stc = STLib.STConstants.MACConstantsToWrapperConstants;
using et = STLib.STConstants.EventTypes;
using ec = STLib.STConstants.MACErrorCodes;
using ic = STLib.STConstants.MACInfoCodes;
using im = STLib.STConstants.MACInfoMessages;
using em = STLib.STConstants.MACErrorMessages;
using cfg = STLib.STConstants.Config;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.ComponentModel.ToolboxItem(false)]
[ScriptService]

public class STServices : WebService {
    // Default constructor
    public STServices () {}

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string stapiPreCheckSiteValidatePlayerRequest()// done
    {
        string jsonRequest;

        // Read input json from post stream
        if (HttpContext.Current.Request.InputStream.CanSeek)
            HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            jsonRequest = reader.ReadToEnd();
        // Read input json from post stream

        dynamic operationData = JObject.Parse(jsonRequest);

        var mWrapperStAPI = new WrapperStAPI();
        var mMACUtils = new MACUtils();

        var mMethodName = mMACUtils.GetCurrentMethod();
        var mStiWrapperState = mMACUtils.NewWrapperState(jsonRequest, mMethodName);

        // get client id for site from MAC Service
        if (mMACUtils.GetMACClientIdUsingOperatorIdAndSiteId(mStiWrapperState, operationData.operatorId.ToString(),
                operationData.siteId.ToString()) == false)
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoClientForSiteId, em.NoClientForSiteId);

        ////mMACUtils.EventLogOperRequest(mStiWrapperState);
        try
        {
            mStiWrapperState.stResponseJson = mWrapperStAPI.stapiPreCheckSiteValidatePlayerRequest(mStiWrapperState);
            return mStiWrapperState.stResponseJson;
        }
        catch (Exception ex)
        {
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        }
    }

    #region --------- Card --------------------------------

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string stapiPreCheckSiteCardRequest() // done
    {
        string jsonRequest;

        // Read input json from post stream
        if (HttpContext.Current.Request.InputStream.CanSeek)
            HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            jsonRequest = reader.ReadToEnd();
        // Read input json from post stream

        dynamic operationData = JObject.Parse(jsonRequest);

        var mWrapperStAPI = new WrapperStAPI();
        var mMACUtils = new MACUtils();

        var mMethodName = mMACUtils.GetCurrentMethod();
        var mStiWrapperState = mMACUtils.NewWrapperState(jsonRequest, mMethodName);

        ////mMACUtils.EventLogOperRequest(mStiWrapperState);
        // get client id for site
        if (mMACUtils.GetMACClientIdUsingOperatorIdAndSiteId(mStiWrapperState,
                operationData.operatorId.ToString(),
                operationData.siteId.ToString()) == false)
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoClientForSiteId, em.NoClientForSiteId);

        try
        {
            mStiWrapperState.stResponseJson = mWrapperStAPI.stapiPreCheckSiteCardRequest(mStiWrapperState);
            return mStiWrapperState.stResponseJson;
        }
        catch (Exception ex)
        {
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string stapiPreCheckSiteModifyCardRequest()
    {
        string jsonRequest;

        // Read input json from post stream
        if (HttpContext.Current.Request.InputStream.CanSeek)
            HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            jsonRequest = reader.ReadToEnd();
        // Read input json from post stream

        dynamic operationData = JObject.Parse(jsonRequest);

        var mWrapperStAPI = new WrapperStAPI();
        var mMACUtils = new MACUtils();

        var mMethodName = mMACUtils.GetCurrentMethod();
        var mStiWrapperState = mMACUtils.NewWrapperState(jsonRequest, mMethodName);

        // get client id for site from MAC Service
        if (mMACUtils.GetMACClientIdUsingOperatorIdAndSiteId(mStiWrapperState, operationData.operatorId.ToString(),
                operationData.siteId.ToString()) == false)
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoClientForSiteId, em.NoClientForSiteId);

        ////mMACUtils.EventLogOperRequest(mStiWrapperState);
        try
        {
            mStiWrapperState.stResponseJson = mWrapperStAPI.stapiPreCheckSiteModifyCardRequest(mStiWrapperState);
            return mStiWrapperState.stResponseJson;
        }
        catch (Exception ex)
        {
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string stapiGetRegisteredCards()
    {
        string jsonRequest;

        // Read input json from post stream
        if (HttpContext.Current.Request.InputStream.CanSeek)
            HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            jsonRequest = reader.ReadToEnd();
        // Read input json from post stream

        dynamic operationData = JObject.Parse(jsonRequest);

        var mWrapperStAPI = new WrapperStAPI();
        var mMACUtils = new MACUtils();

        var mMethodName = mMACUtils.GetCurrentMethod();
        var mStiWrapperState = mMACUtils.NewWrapperState(jsonRequest, mMethodName);

        // get client id for site from MAC Service
        if (mMACUtils.GetMACClientIdUsingOperatorIdAndSiteId(mStiWrapperState, operationData.operatorId.ToString(),
                operationData.siteId.ToString()) == false)
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoClientForSiteId, em.NoClientForSiteId);

        ////mMACUtils.EventLogOperRequest(mStiWrapperState);
        try
        {
            mStiWrapperState.stResponseJson = mWrapperStAPI.stapiGetRegisteredCards(mStiWrapperState);
            return mStiWrapperState.stResponseJson;
        }
        catch (Exception ex)
        {
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string stapiPreCheckSiteDeleteSpecifiedCard()
    {
        string jsonRequest;

        // Read input json from post stream
        if (HttpContext.Current.Request.InputStream.CanSeek)
            HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            jsonRequest = reader.ReadToEnd();
        // Read input json from post stream

        dynamic operationData = JObject.Parse(jsonRequest);

        var mWrapperStAPI = new WrapperStAPI();
        var mMACUtils = new MACUtils();

        var mMethodName = mMACUtils.GetCurrentMethod();
        var mStiWrapperState = mMACUtils.NewWrapperState(jsonRequest, mMethodName);

        // get client id for site from MAC Service
        if (mMACUtils.GetMACClientIdUsingOperatorIdAndSiteId(mStiWrapperState, operationData.operatorId.ToString(),
                operationData.siteId.ToString()) == false)
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoClientForSiteId, em.NoClientForSiteId);

        ////mMACUtils.EventLogOperRequest(mStiWrapperState);
        try
        {
            mStiWrapperState.stResponseJson = mWrapperStAPI.stapiAddSelfExcludePlayer(mStiWrapperState);
            return mStiWrapperState.stResponseJson;
        }
        catch (Exception ex)
        {
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        }
    }
    #endregion

    #region --------- OTP ---------------------------------

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string stapiValidateRegistrationOtp() // done
    {
        string jsonRequest;

        // Read input json from post stream
        if (HttpContext.Current.Request.InputStream.CanSeek)
            HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            jsonRequest = reader.ReadToEnd();
        // Read input json from post stream

        dynamic operationData = JObject.Parse(jsonRequest);

        var mWrapperStAPI = new WrapperStAPI();
        var mMACUtils = new MACUtils();

        var mMethodName = mMACUtils.GetCurrentMethod();

        try
        {
            var mStiWrapperState = new StiWrapperState(operationData.requestToken.ToString());
            if (mStiWrapperState.IsValid == false)
                return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.InvalidWrapperId, em.InvalidWrapperId);

            ////mMACUtils.EventLogOperRequest(mStiWrapperState);

            // get OTP entered by Player
            mStiWrapperState.macOTP = operationData.OTP.ToString().Trim();

            //---- send verify OTP request to MAC Otp System ----
            var mVerifyOTPReply = mMACUtils.MACVerifyOTP(mStiWrapperState, mMethodName);
            if (mVerifyOTPReply.Item1 == false)
                return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.VerifyOTPError, mVerifyOTPReply.Item2);

            var mResponse = JObject.Parse(mVerifyOTPReply.Item2);
            var mReplyProperty = mResponse.Property(stc.Reply);
            var mReply = mReplyProperty.Value.ToString();
            switch (mReply)
            {
                case stc.Validated:
                    mStiWrapperState.stResponseJson = mWrapperStAPI.stapiRegisterPlayer(mStiWrapperState);

                    dynamic mSTResponseData = JObject.Parse(mStiWrapperState.stResponseJson);
                    string mReturnCode = mSTResponseData.returnCode.ToString();
                    switch (mReturnCode)
                    {
                        case "INFO_PFO_00023": //Player successfully registered.
                            // player userName to state, used ad unique identifier in MAC End User Registration
                            string mPlayerUserName = mSTResponseData.playerDetails.userName.ToString();
                            if (String.IsNullOrEmpty(mPlayerUserName))
                                return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoPlayerDetails, ec.NoPlayerDetails);
                            mStiWrapperState.stUserName = mPlayerUserName;

                            //---- Delete End User if exists
                            mMACUtils.MACDeleteEndUserByUserId(mStiWrapperState, mMethodName);

                            //---- Register Player with MAC System ---
                            var regReply = mMACUtils.MACRegisterPlayer(mStiWrapperState, mMethodName);
                            if (regReply.Item1 == false)
                                return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.RegError,
                                    regReply.Item2);
                            return mMACUtils.SendOperatorResponse_StSuccessResponse(mStiWrapperState);

                        case "INFO_PFO_00021": //Please answer personal verification questions(kycDetails)
                            mSTResponseData.Remove("responseToken");
                            mSTResponseData.Add("responseToken", mStiWrapperState._id.ToString());
                            mStiWrapperState.macNextMethod = "SubmitPlayerKBA";
                            mStiWrapperState.Save(mStiWrapperState);
                            return mStiWrapperState.stResponseJson;

                        case "ERR_PFO_00001":
                        //Validation Failure - Field validation failure. Please see validationErrors for more details.
                        case "ERR_PFO_00045":
                            //System Error – Existing player check. Please contact Secure Trading Inc. Support.
                            mStiWrapperState.Delete(mStiWrapperState);
                            return mStiWrapperState.stResponseJson;

                        default:
                            if (mReturnCode.StartsWith("ERR_LOP"))
                                return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, mReturnCode,
                                    mSTResponseData.returnMsg);

                            //System Error – Existing player check. Please contact Secure Trading Inc. Support.
                            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.InvalidResponse,
                                em.InvalidResponse);
                    }

                case stc.Invalid:
                    return mMACUtils.SendOperatorResponse_ReenterOTP(mStiWrapperState, mVerifyOTPReply.Item2);

                case stc.Disabled:
                    return mMACUtils.SendOperatorResponse_OTPError(mStiWrapperState, ec.OTPDisabled, em.OTPDisabled);

                case stc.Inactive:
                    return mMACUtils.SendOperatorResponse_OTPError(mStiWrapperState, ec.OTPInactive, em.OTPInactive);

                case stc.Timeout:
                    return mMACUtils.SendOperatorResponse_OTPError(mStiWrapperState, ec.OTPTimeout, ec.OTPTimeout);
                
                default:
                    //System Error – Existing player check. Please contact Secure Trading Inc. Support.
                    return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState,
                        ec.InvalidResponse, mMethodName + ":" + em.InvalidResponse);
            }
        }
        catch (Exception ex)
        {
            return mMACUtils.SendOperatorResponse_SystemError(null, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string stapiResendOtp() // done
    {
        string jsonRequest;

        // Read input json from post stream
        if (HttpContext.Current.Request.InputStream.CanSeek)
            HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            jsonRequest = reader.ReadToEnd();
        // Read input json from post stream

        dynamic operationData = JObject.Parse(jsonRequest);

        var mMACUtils = new MACUtils();
        var mMethodName = mMACUtils.GetCurrentMethod();

        try
        {
            var mStiWrapperState = new StiWrapperState(operationData.requestToken.ToString());
            if (mStiWrapperState.IsValid == false)
                return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.InvalidWrapperId, em.InvalidWrapperId);

            ////mMACUtils.EventLogOperRequest(mStiWrapperState);

            //---- send resendOTP request to MAC ----
            var ReqResentOTPReply = mMACUtils.MACResendOTP(mStiWrapperState, mMethodName);
            if (ReqResentOTPReply.Item1 == false)
                return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.ResendOTPError, ReqResentOTPReply.Item2);

            var mResuestOTPResponse = JObject.Parse(ReqResentOTPReply.Item2);
            var RIDProperty = mResuestOTPResponse.Property(stc.RequestId);
            if (RIDProperty == null)
                return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoRequestId, ReqResentOTPReply.Item2);
            return mMACUtils.SendOperatorResponse_RequestOTP(mStiWrapperState);
        }
        catch (Exception ex)
        {
            return mMACUtils.SendOperatorResponse_SystemError(null, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string stapiValidateAuthenticationOtp() // done
    {
        string jsonRequest;

        // Read input json from post stream
        if (HttpContext.Current.Request.InputStream.CanSeek)
            HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            jsonRequest = reader.ReadToEnd();
        // Read input json from post stream

        dynamic operationData = JObject.Parse(jsonRequest);

        var mMACUtils = new MACUtils();
        var mMethodName = mMACUtils.GetCurrentMethod();

        try
        {
            var mStiWrapperState = new StiWrapperState(operationData.requestToken.ToString());
            if (mStiWrapperState.IsValid == false)
                return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState,
                    ec.InvalidWrapperId, em.InvalidWrapperId);

            ////mMACUtils.EventLogOperRequest(mStiWrapperState);

            // get OTP entered by Player
            mStiWrapperState.macOTP = operationData.OTP.ToString().Trim();

            //---- send verify OTP request to MAC Otp System ----
            var mVerifyOTPReply = mMACUtils.MACVerifyOTP(mStiWrapperState, mMethodName);
            if (mVerifyOTPReply.Item1 == false)
                return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.VerifyOTPError, mVerifyOTPReply.Item2);

            var mResponse = JObject.Parse(mVerifyOTPReply.Item2);
            var mReplyProperty = mResponse.Property(stc.Reply);
            var mReply = mReplyProperty.Value.ToString();
            switch (mReply)
            {
                case stc.Validated: // on Authorization OTP just return ST's response from stapiPreCheckSiteValidatePlayerRequest Request
                    return mStiWrapperState.stResponseJson;

                case stc.Invalid:
                    return mMACUtils.SendOperatorResponse_ReenterOTP(mStiWrapperState, mVerifyOTPReply.Item2);

                case stc.Disabled:
                    return mMACUtils.SendOperatorResponse_OTPError(mStiWrapperState, ec.OTPDisabled, em.OTPDisabled);

                case stc.Inactive:
                    return mMACUtils.SendOperatorResponse_OTPError(mStiWrapperState, ec.OTPInactive, em.OTPInactive);

                case stc.Timeout:
                    return mMACUtils.SendOperatorResponse_OTPError(mStiWrapperState, ec.OTPTimeout, ec.OTPTimeout);

                default:
                    //System Error – Existing player check. Please contact Secure Trading Inc. Support.
                    return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState,
                        ec.InvalidResponse, mMethodName + ":" + em.InvalidResponse);
            }
        }
        catch (Exception ex)
        {
            return mMACUtils.SendOperatorResponse_SystemError(null, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string stapiValidateAuthorizationOtp() // done
    {
        string jsonRequest;

        // Read input json from post stream
        if (HttpContext.Current.Request.InputStream.CanSeek)
            HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            jsonRequest = reader.ReadToEnd();
        // Read input json from post stream

        dynamic operationData = JObject.Parse(jsonRequest);

        var mWrapperStAPI = new WrapperStAPI();
        var mMACUtils = new MACUtils();

        var mMethodName = mMACUtils.GetCurrentMethod();

        try
        {
            string mRequestToken = operationData.requestToken.ToString();
            var mStiWrapperState = new StiWrapperState(mRequestToken);
            if (mStiWrapperState.IsValid == false)
                return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState,
                    ec.InvalidWrapperId, em.InvalidWrapperId);
            //mMACUtils.EventLogOperRequest(mStiWrapperState);

            // get OTP entered by Player
            string mOTP = operationData.OTP.ToString();
            mStiWrapperState.macOTP = mOTP.Trim();

            //---- send verify OTP request to MAC Otp System ----
            var mVerifyOTPReply = mMACUtils.MACVerifyOTP(mStiWrapperState, mMethodName);
            if (mVerifyOTPReply.Item1 == false)
                return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState,
                    ec.VerifyOTPError, mVerifyOTPReply.Item2);

            var mResponse = JObject.Parse(mVerifyOTPReply.Item2);
            var mReplyProperty = mResponse.Property(stc.Reply);
            var mReply = mReplyProperty.Value.ToString();
            switch (mReply)
            {
                case stc.Validated:
                    //switch (mStiWrapperState.methodName)
                    //{
                    //    case "stapiPrePaidRegisterAndLoad":
                    //        mStiWrapperState.stResponseJson = mWrapperStAPI.stapiPrePaidRegisterAndLoad(mStiWrapperState);
                    //        break;
                    //    case "stapiSubmitDepositRequest":
                    //        mStiWrapperState.stResponseJson = mWrapperStAPI.stapiSubmitDepositRequest(mStiWrapperState);
                    //        break;
                    //    case "stapiSubmitDepositAccountRequest":
                    //        mStiWrapperState.stResponseJson = mWrapperStAPI.stapiSubmitDepositAccountRequest(mStiWrapperState);
                    //        break;
                    //    case "stapiSubmitPrePaidDeposit":
                    //        mStiWrapperState.stResponseJson = mWrapperStAPI.stapiSubmitPrePaidDeposit(mStiWrapperState);
                    //        break;
                    //    case "stapiLoadPrePaidFunds":
                    //        mStiWrapperState.stResponseJson = mWrapperStAPI.stapiLoadPrePaidFunds(mStiWrapperState);
                    //        break;
                    //    case "stapiWithdrawal":
                    //        mStiWrapperState.stResponseJson = mWrapperStAPI.stapiWithdrawal(mStiWrapperState);
                    //        break;
                    //    case "stapiAccountWithdrawal":
                    //        mStiWrapperState.stResponseJson = mWrapperStAPI.stapiAccountWithdrawal(mStiWrapperState);
                    //        break;
                    //    case "stapiSubmitPrePaidWithdrawal":
                    //        mStiWrapperState.stResponseJson = mWrapperStAPI.stapiSubmitPrePaidWithdrawal(mStiWrapperState);
                    //        break;
                    //    case "stapiSubmitRefund":
                    //        mStiWrapperState.stResponseJson = mWrapperStAPI.stapiSubmitRefund(mStiWrapperState);
                    //        break;
                    //    default: 
                    //        return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState,
                    //                ec.InvalidResponse, mMethodName + ":" + em.InvalidResponse);
                    //}
                    dynamic mSTResponseData = JObject.Parse(mStiWrapperState.stResponseJson);
                    string returnCode = mSTResponseData.returnCode.ToString();
                    switch (returnCode)
                    {
                        case "INFO_PFO_00023": //Load PrePaid Funds successfully
                        case "INFO_PFO_00112": //Authorization Failure
                            return mMACUtils.SendOperatorResponse_StSuccessResponse(mStiWrapperState);

                        default:
                            if (returnCode.StartsWith("ERR_LOP"))
                                return mStiWrapperState.stResponseJson;

                            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState,
                                ec.InvalidResponse, em.InvalidResponse);
                    }

                case stc.Invalid:
                    return
                        mMACUtils.SendOperatorResponse_ReenterOTP(mStiWrapperState, mVerifyOTPReply.Item2);

                case stc.Disabled:
                    return
                        mMACUtils.SendOperatorResponse_OTPError(mStiWrapperState,
                            ec.OTPDisabled, em.OTPDisabled);

                case stc.Inactive:
                    return
                        mMACUtils.SendOperatorResponse_OTPError(mStiWrapperState,
                            ec.OTPInactive, em.OTPInactive);

                case stc.Timeout:
                    return
                        mMACUtils.SendOperatorResponse_OTPError(mStiWrapperState,
                            ec.OTPTimeout, ec.OTPTimeout);

                default:
                    //System Error – Existing player check. Please contact Secure Trading Inc. Support.
                    return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState,
                        ec.InvalidResponse, mMethodName + ":" + em.InvalidResponse);

            }
        }
        catch (Exception ex)
        {
            return
                mMACUtils.SendOperatorResponse_SystemError(null,
                    ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        }
    }
    #endregion

    #region --------- Player ------------------------------

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string stapiPlayerLogin() // OTP Enabled status done
    {
        string jsonRequest;

        // Read input json from post stream
        if (HttpContext.Current.Request.InputStream.CanSeek)
            HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            jsonRequest = reader.ReadToEnd();
        // Read input json from post stream

        dynamic operationData = JObject.Parse(jsonRequest);

        var mWrapperStAPI = new WrapperStAPI();
        var mMACUtils = new MACUtils();

        var mMethodName = mMACUtils.GetCurrentMethod();
        var mStiWrapperState = mMACUtils.NewWrapperState(jsonRequest, mMethodName);

        // player userName to Wrapper state, used to send otp to registered end user
        mStiWrapperState.stUserName = operationData.userName.ToString();
        // get client id to wrapper state, used in MAC Service requests
        if (mMACUtils.GetMACClientIdUsingOperatorIdAndSiteId(mStiWrapperState, operationData.operatorId.ToString(),
                                        operationData.siteId.ToString()) == false)
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoClientForSiteId, em.NoClientForSiteId);

        //mMACUtils.EventLogOperRequest(mStiWrapperState);
        try
        {
            mStiWrapperState.stResponseJson = mWrapperStAPI.stapiPreCheckSiteValidatePlayerRequest(mStiWrapperState);

            dynamic mResponseJson = JObject.Parse(mStiWrapperState.stResponseJson);
            string returnCode = mResponseJson.returnCode.ToString();
            switch (returnCode)
            {
                case "INFO_PFO_00002":
                case "INFO_PFO_00041":
                case "INFO_PFO_00040":
                    // check if end user is registered in MAC System
                    var EndUserRegisteredReply = mMACUtils.MACEndUserRegisteredByUserName(mStiWrapperState, mMethodName);
                    if (EndUserRegisteredReply.Item1 == false)
                        return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoPlayerNotRegisteredWithMAC, EndUserRegisteredReply.Item2);

                    // Player is registered with MAC, send authorization OTP
                    //---- send registration OTP message to registered end user (Player) ----
                    var ReqRequestOTPReply = mMACUtils.MACRequestOTP_RegisteredPlayer(mStiWrapperState, mMethodName, stc.Authentication, null);
                    if (ReqRequestOTPReply.Item1 == false)
                        return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.SendOTPError, ReqRequestOTPReply.Item2);

                    var mResuestOTPResponse = JObject.Parse(ReqRequestOTPReply.Item2);
                    var RIDProperty = mResuestOTPResponse.Property(stc.RequestId);
                    if (RIDProperty == null)
                        return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoRequestId, ReqRequestOTPReply.Item2);
                    // request id to state
                    mStiWrapperState.macRequestId = RIDProperty.Value.ToString();
                    // OTP to state if included in response
                    var OTPProperty = mResuestOTPResponse.Property(stc.OTP);
                    if (OTPProperty != null) //check if OTPProperty exists
                        mStiWrapperState.macOTP = OTPProperty.Value.ToString();
                    // EnterOTPAd to state if included in response
                    var EnterOTPAdProperty = mResuestOTPResponse.Property(stc.EnterOTPAd);
                    if (EnterOTPAdProperty != null) //check if EnterOTPAd exists
                        mStiWrapperState.macEnterOtpAd = EnterOTPAdProperty.Value.ToString();
                    return mMACUtils.SendOperatorResponse_RequestOTP(mStiWrapperState);

            }
            // anything else just return ST's response
            return mStiWrapperState.stResponseJson;
        }
        catch (Exception ex)
        {
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string stapiRegisterPlayer() // OTP Enabled status done
    {
        string jsonRequest;

        // Read input json from post stream
        if (HttpContext.Current.Request.InputStream.CanSeek)
            HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            jsonRequest = reader.ReadToEnd();
        // Read input json from post stream

        dynamic operationData = JObject.Parse(jsonRequest);

        var mMACUtils = new MACUtils();
        var mMethodName = mMACUtils.GetCurrentMethod();
        var mStiWrapperState = mMACUtils.NewWrapperState(jsonRequest, mMethodName);

        //mMACUtils.EventLogOperRequest(mStiWrapperState);
        // get client id for site from MAC Service
        if (mMACUtils.GetMACClientIdUsingOperatorIdAndSiteId(mStiWrapperState,
                operationData.operatorId.ToString(),
                operationData.siteId.ToString()) == false)
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoClientForSiteId, em.NoClientForSiteId);
        try
        {
            //---- send registration OTP message to Player to confirm valid mobile phone ----
            var ReqRequestOTPReply = mMACUtils.MACRequestOTP_UnregisteredPlayer(mStiWrapperState, mMethodName);
            if (ReqRequestOTPReply.Item1 == false)
                return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.SendOTPError, ReqRequestOTPReply.Item2);

            var mResuestOTPResponse = JObject.Parse(ReqRequestOTPReply.Item2);
            var RIDProperty = mResuestOTPResponse.Property(stc.RequestId);
            if (RIDProperty == null)
                return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoRequestId, ReqRequestOTPReply.Item2);
            // request id to state
            mStiWrapperState.macRequestId = RIDProperty.Value.ToString();
            // otp to state if included in response
            var OTPProperty = mResuestOTPResponse.Property(stc.OTP);
            if (OTPProperty != null) //check if OTPProperty exists
                mStiWrapperState.macOTP = OTPProperty.Value.ToString();
            // EnterOTPAd to state if included in response
            var EnterOTPAdProperty = mResuestOTPResponse.Property(stc.EnterOTPAd);
            if (EnterOTPAdProperty != null) //check if EnterOTPAd exists
                mStiWrapperState.macEnterOtpAd = EnterOTPAdProperty.Value.ToString();
            return mMACUtils.SendOperatorResponse_RequestOTP(mStiWrapperState);
        }
        catch (Exception ex)
        {
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string stapiSubmitPlayerKBA() // done
    {
        string jsonRequest;

        // Read input json from post stream
        if (HttpContext.Current.Request.InputStream.CanSeek)
            HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            jsonRequest = reader.ReadToEnd();
        // Read input json from post stream

        dynamic operationData = JObject.Parse(jsonRequest);

        var mWrapperStAPI = new WrapperStAPI();
        var mMACUtils = new MACUtils();

        var mMethodName = mMACUtils.GetCurrentMethod();

        try
        {
            var mStiWrapperState = new StiWrapperState(operationData.requestToken.ToString());
            if (mStiWrapperState.IsValid == false)
                return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.InvalidWrapperId, em.InvalidWrapperId);
            //mMACUtils.EventLogOperRequest(mStiWrapperState);

            mStiWrapperState.stResponseJson = mWrapperStAPI.stapiSubmitPlayerKBA(mStiWrapperState);
            return mStiWrapperState.stResponseJson;
        }
        catch (Exception ex)
        {
            return mMACUtils.SendOperatorResponse_SystemError(null, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string stapiSelfExcludePlayer()
    {
        string jsonRequest;

        // Read input json from post stream
        if (HttpContext.Current.Request.InputStream.CanSeek)
            HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            jsonRequest = reader.ReadToEnd();
        // Read input json from post stream

        dynamic operationData = JObject.Parse(jsonRequest);

        var mWrapperStAPI = new WrapperStAPI();
        var mMACUtils = new MACUtils();

        var mMethodName = mMACUtils.GetCurrentMethod();
        var mStiWrapperState = mMACUtils.NewWrapperState(jsonRequest, mMethodName);

        // get client id for site from MAC Service
        if (mMACUtils.GetMACClientIdUsingOperatorIdAndSiteId(mStiWrapperState, operationData.operatorId.ToString(),
                operationData.siteId.ToString()) == false)
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoClientForSiteId, em.NoClientForSiteId);

        //mMACUtils.EventLogOperRequest(mStiWrapperState);
        try
        {
            mStiWrapperState.stResponseJson = mWrapperStAPI.stapiSelfExcludePlayer(mStiWrapperState);
            return mStiWrapperState.stResponseJson;
        }
        catch (Exception ex)
        {
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string stapiModifyPlayerRequest()
    {
        string jsonRequest;

        // Read input json from post stream
        if (HttpContext.Current.Request.InputStream.CanSeek)
            HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            jsonRequest = reader.ReadToEnd();
        // Read input json from post stream

        dynamic operationData = JObject.Parse(jsonRequest);

        var mWrapperStAPI = new WrapperStAPI();
        var mMACUtils = new MACUtils();

        var mMethodName = mMACUtils.GetCurrentMethod();
        var mStiWrapperState = mMACUtils.NewWrapperState(jsonRequest, mMethodName);

        // get client id for site from MAC Service
        if (mMACUtils.GetMACClientIdUsingOperatorIdAndSiteId(mStiWrapperState, operationData.operatorId.ToString(),
                operationData.siteId.ToString()) == false)
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoClientForSiteId, em.NoClientForSiteId);

        //mMACUtils.EventLogOperRequest(mStiWrapperState);
        try
        {
            mStiWrapperState.stResponseJson = mWrapperStAPI.stapiModifyPlayerRequest(mStiWrapperState);
            return mStiWrapperState.stResponseJson;
        }
        catch (Exception ex)
        {
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string stapiSubmitModifiedPlayerKBA()
    {
        string jsonRequest;

        // Read input json from post stream
        if (HttpContext.Current.Request.InputStream.CanSeek)
            HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            jsonRequest = reader.ReadToEnd();
        // Read input json from post stream

        dynamic operationData = JObject.Parse(jsonRequest);

        var mWrapperStAPI = new WrapperStAPI();
        var mMACUtils = new MACUtils();

        var mMethodName = mMACUtils.GetCurrentMethod();
        var mStiWrapperState = mMACUtils.NewWrapperState(jsonRequest, mMethodName);

        // get client id for site from MAC Service
        if (mMACUtils.GetMACClientIdUsingOperatorIdAndSiteId(mStiWrapperState, operationData.operatorId.ToString(),
                operationData.siteId.ToString()) == false)
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoClientForSiteId, em.NoClientForSiteId);

        //mMACUtils.EventLogOperRequest(mStiWrapperState);
        try
        {
            mStiWrapperState.stResponseJson = mWrapperStAPI.stapiSubmitModifiedPlayerKBA(mStiWrapperState);
            return mStiWrapperState.stResponseJson;
        }
        catch (Exception ex)
        {
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string stapiGetPlayerSSN() // done
    {
        string jsonRequest;

        // Read input json from post stream
        if (HttpContext.Current.Request.InputStream.CanSeek)
            HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            jsonRequest = reader.ReadToEnd();
        // Read input json from post stream

        dynamic operationData = JObject.Parse(jsonRequest);

        var mWrapperStAPI = new WrapperStAPI();
        var mMACUtils = new MACUtils();

        var mMethodName = mMACUtils.GetCurrentMethod();
        var mStiWrapperState = mMACUtils.NewWrapperState(jsonRequest, mMethodName);

        // get client id for site from MAC Service
        if (mMACUtils.GetMACClientIdUsingOperatorIdAndSiteId(mStiWrapperState, operationData.operatorId.ToString(),
                operationData.siteId.ToString()) == false)
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoClientForSiteId, em.NoClientForSiteId);

        //mMACUtils.EventLogOperRequest(mStiWrapperState);
        try
        {
            mStiWrapperState.stResponseJson = mWrapperStAPI.stapiGetPlayerSSN(mStiWrapperState);
            return mStiWrapperState.stResponseJson;
        }
        catch (Exception ex)
        {
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string stapiUpdatePlayerSSN()
    {
        string jsonRequest;

        // Read input json from post stream
        if (HttpContext.Current.Request.InputStream.CanSeek)
            HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            jsonRequest = reader.ReadToEnd();
        // Read input json from post stream

        dynamic operationData = JObject.Parse(jsonRequest);

        var mWrapperStAPI = new WrapperStAPI();
        var mMACUtils = new MACUtils();

        var mMethodName = mMACUtils.GetCurrentMethod();
        var mStiWrapperState = mMACUtils.NewWrapperState(jsonRequest, mMethodName);

        // get client id for site from MAC Service
        if (mMACUtils.GetMACClientIdUsingOperatorIdAndSiteId(mStiWrapperState, operationData.operatorId.ToString(),
                operationData.siteId.ToString()) == false)
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoClientForSiteId, em.NoClientForSiteId);

        //mMACUtils.EventLogOperRequest(mStiWrapperState);
        try
        {
            mStiWrapperState.stResponseJson = mWrapperStAPI.stapiUpdatePlayerSSN(mStiWrapperState);
            return mStiWrapperState.stResponseJson;
        }
        catch (Exception ex)
        {
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string stapiAddSelfExcludePlayer()
    {
        string jsonRequest;

        // Read input json from post stream
        if (HttpContext.Current.Request.InputStream.CanSeek)
            HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            jsonRequest = reader.ReadToEnd();
        // Read input json from post stream

        dynamic operationData = JObject.Parse(jsonRequest);

        var mWrapperStAPI = new WrapperStAPI();
        var mMACUtils = new MACUtils();

        var mMethodName = mMACUtils.GetCurrentMethod();
        var mStiWrapperState = mMACUtils.NewWrapperState(jsonRequest, mMethodName);

        // get client id for site from MAC Service
        if (mMACUtils.GetMACClientIdUsingOperatorIdAndSiteId(mStiWrapperState, operationData.operatorId.ToString(),
                operationData.siteId.ToString()) == false)
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoClientForSiteId, em.NoClientForSiteId);

        //mMACUtils.EventLogOperRequest(mStiWrapperState);
        try
        {
            mStiWrapperState.stResponseJson = mWrapperStAPI.stapiAddSelfExcludePlayer(mStiWrapperState);
            return mStiWrapperState.stResponseJson;
        }
        catch (Exception ex)
        {
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        }
    }
    #endregion

    #region --------- Account -----------------------------

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string stapiRegisterPrePaidAccount()
    {
        string jsonRequest;

        // Read input json from post stream
        if (HttpContext.Current.Request.InputStream.CanSeek)
            HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            jsonRequest = reader.ReadToEnd();
        // Read input json from post stream

        dynamic operationData = JObject.Parse(jsonRequest);

        var mWrapperStAPI = new WrapperStAPI();
        var mMACUtils = new MACUtils();

        var mMethodName = mMACUtils.GetCurrentMethod();
        var mStiWrapperState = mMACUtils.NewWrapperState(jsonRequest, mMethodName);

        //mMACUtils.EventLogOperRequest(mStiWrapperState);
        // get client id for site from MAC Service
        if (mMACUtils.GetMACClientIdUsingOperatorIdAndSiteId(mStiWrapperState,
                operationData.operatorId.ToString(),
                operationData.siteId.ToString()) == false)
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoClientForSiteId, em.NoClientForSiteId);
        try
        {
            return mWrapperStAPI.stapiRegisterPrePaidAccount(mStiWrapperState);
        }
        catch (Exception ex)
        {
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string stapiPreCheckSiteAccountRequest()
    {
        string jsonRequest;

        // Read input json from post stream
        if (HttpContext.Current.Request.InputStream.CanSeek)
            HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            jsonRequest = reader.ReadToEnd();
        // Read input json from post stream

        dynamic operationData = JObject.Parse(jsonRequest);

        var mWrapperStAPI = new WrapperStAPI();
        var mMACUtils = new MACUtils();

        var mMethodName = mMACUtils.GetCurrentMethod();
        var mStiWrapperState = mMACUtils.NewWrapperState(jsonRequest, mMethodName);

        // get client id for site from MAC Service
        if (mMACUtils.GetMACClientIdUsingOperatorIdAndSiteId(mStiWrapperState, operationData.operatorId.ToString(),
                operationData.siteId.ToString()) == false)
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoClientForSiteId, em.NoClientForSiteId);

        //mMACUtils.EventLogOperRequest(mStiWrapperState);
        try
        {
            mStiWrapperState.stResponseJson = mWrapperStAPI.stapiPreCheckSiteAccountRequest(mStiWrapperState);
            return mStiWrapperState.stResponseJson;
        }
        catch (Exception ex)
        {
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string stapiPreCheckSiteModifyAccountRequest()
    {
        string jsonRequest;

        // Read input json from post stream
        if (HttpContext.Current.Request.InputStream.CanSeek)
            HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            jsonRequest = reader.ReadToEnd();
        // Read input json from post stream

        dynamic operationData = JObject.Parse(jsonRequest);

        var mWrapperStAPI = new WrapperStAPI();
        var mMACUtils = new MACUtils();

        var mMethodName = mMACUtils.GetCurrentMethod();
        var mStiWrapperState = mMACUtils.NewWrapperState(jsonRequest, mMethodName);

        // get client id for site from MAC Service
        if (mMACUtils.GetMACClientIdUsingOperatorIdAndSiteId(mStiWrapperState, operationData.operatorId.ToString(),
                operationData.siteId.ToString()) == false)
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoClientForSiteId, em.NoClientForSiteId);

        //mMACUtils.EventLogOperRequest(mStiWrapperState);
        try
        {
            mStiWrapperState.stResponseJson = mWrapperStAPI.stapiPreCheckSiteModifyAccountRequest(mStiWrapperState);
            return mStiWrapperState.stResponseJson;
        }
        catch (Exception ex)
        {
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string stapiSubmitPrePaidIdentityAnswers()
    {
        string jsonRequest;

        // Read input json from post stream
        if (HttpContext.Current.Request.InputStream.CanSeek)
            HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            jsonRequest = reader.ReadToEnd();
        // Read input json from post stream

        dynamic operationData = JObject.Parse(jsonRequest);

        var mWrapperStAPI = new WrapperStAPI();
        var mMACUtils = new MACUtils();

        var mMethodName = mMACUtils.GetCurrentMethod();
        var mStiWrapperState = mMACUtils.NewWrapperState(jsonRequest, mMethodName);

        // get client id for site from MAC Service
        if (mMACUtils.GetMACClientIdUsingOperatorIdAndSiteId(mStiWrapperState, operationData.operatorId.ToString(),
                operationData.siteId.ToString()) == false)
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoClientForSiteId, em.NoClientForSiteId);

        //mMACUtils.EventLogOperRequest(mStiWrapperState);
        try
        {
            mStiWrapperState.stResponseJson = mWrapperStAPI.stapiSubmitPrePaidIdentityAnswers(mStiWrapperState);
            return mStiWrapperState.stResponseJson;
        }
        catch (Exception ex)
        {
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string stapiPrePaidRegisterAndLoad() // OTP enabled not done
    {
        string jsonRequest;

        // Read input json from post stream
        if (HttpContext.Current.Request.InputStream.CanSeek)
            HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            jsonRequest = reader.ReadToEnd();
        // Read input json from post stream

        dynamic operationData = JObject.Parse(jsonRequest);

        var mWrapperStAPI = new WrapperStAPI();
        var mMACUtils = new MACUtils();

        var mMethodName = mMACUtils.GetCurrentMethod();
        var mStiWrapperState = mMACUtils.NewWrapperState(jsonRequest, mMethodName);

        // get client id for site from MAC Service
        if (mMACUtils.GetMACClientIdUsingOperatorIdAndSiteId(mStiWrapperState, operationData.operatorId.ToString(),
                operationData.siteId.ToString()) == false)
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoClientForSiteId, em.NoClientForSiteId);

        //mMACUtils.EventLogOperRequest(mStiWrapperState);
        try
        {
            mStiWrapperState.stResponseJson = mWrapperStAPI.stapiPrePaidRegisterAndLoad(mStiWrapperState);
            return mStiWrapperState.stResponseJson;
        }
        catch (Exception ex)
        {
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string stapiGetPrePaidAccountBalance()
    {
        string jsonRequest;

        // Read input json from post stream
        if (HttpContext.Current.Request.InputStream.CanSeek)
            HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            jsonRequest = reader.ReadToEnd();
        // Read input json from post stream

        dynamic operationData = JObject.Parse(jsonRequest);

        var mWrapperStAPI = new WrapperStAPI();
        var mMACUtils = new MACUtils();

        var mMethodName = mMACUtils.GetCurrentMethod();
        var mStiWrapperState = mMACUtils.NewWrapperState(jsonRequest, mMethodName);

        // get client id for site from MAC Service
        if (mMACUtils.GetMACClientIdUsingOperatorIdAndSiteId(mStiWrapperState, operationData.operatorId.ToString(),
                operationData.siteId.ToString()) == false)
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoClientForSiteId, em.NoClientForSiteId);

        //mMACUtils.EventLogOperRequest(mStiWrapperState);
        try
        {
            mStiWrapperState.stResponseJson = mWrapperStAPI.stapiGetPrePaidAccountBalance(mStiWrapperState);
            return mStiWrapperState.stResponseJson;
        }
        catch (Exception ex)
        {
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string stapiGetPrePaidAccountCVV2()
    {
        string jsonRequest;

        // Read input json from post stream
        if (HttpContext.Current.Request.InputStream.CanSeek)
            HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            jsonRequest = reader.ReadToEnd();
        // Read input json from post stream

        dynamic operationData = JObject.Parse(jsonRequest);

        var mWrapperStAPI = new WrapperStAPI();
        var mMACUtils = new MACUtils();

        var mMethodName = mMACUtils.GetCurrentMethod();
        var mStiWrapperState = mMACUtils.NewWrapperState(jsonRequest, mMethodName);

        // get client id for site from MAC Service
        if (mMACUtils.GetMACClientIdUsingOperatorIdAndSiteId(mStiWrapperState, operationData.operatorId.ToString(),
                operationData.siteId.ToString()) == false)
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoClientForSiteId, em.NoClientForSiteId);

        //mMACUtils.EventLogOperRequest(mStiWrapperState);
        try
        {
            mStiWrapperState.stResponseJson = mWrapperStAPI.stapiGetPrePaidAccountCVV2(mStiWrapperState);
            return mStiWrapperState.stResponseJson;
        }
        catch (Exception ex)
        {
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string stapiGetPrePaidAccountStatus()
    {
        string jsonRequest;

        // Read input json from post stream
        if (HttpContext.Current.Request.InputStream.CanSeek)
            HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            jsonRequest = reader.ReadToEnd();
        // Read input json from post stream

        dynamic operationData = JObject.Parse(jsonRequest);

        var mWrapperStAPI = new WrapperStAPI();
        var mMACUtils = new MACUtils();

        var mMethodName = mMACUtils.GetCurrentMethod();
        var mStiWrapperState = mMACUtils.NewWrapperState(jsonRequest, mMethodName);

        // get client id for site from MAC Service
        if (mMACUtils.GetMACClientIdUsingOperatorIdAndSiteId(mStiWrapperState, operationData.operatorId.ToString(),
                operationData.siteId.ToString()) == false)
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoClientForSiteId, em.NoClientForSiteId);

        //mMACUtils.EventLogOperRequest(mStiWrapperState);
        try
        {
            mStiWrapperState.stResponseJson = mWrapperStAPI.stapiSubmitDepositRequest(mStiWrapperState);
            return mStiWrapperState.stResponseJson;
        }
        catch (Exception ex)
        {
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string stapiUpdatePrePaidAccountStatus()
    {
        string jsonRequest;

        // Read input json from post stream
        if (HttpContext.Current.Request.InputStream.CanSeek)
            HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            jsonRequest = reader.ReadToEnd();
        // Read input json from post stream

        dynamic operationData = JObject.Parse(jsonRequest);

        var mWrapperStAPI = new WrapperStAPI();
        var mMACUtils = new MACUtils();

        var mMethodName = mMACUtils.GetCurrentMethod();
        var mStiWrapperState = mMACUtils.NewWrapperState(jsonRequest, mMethodName);

        // get client id for site from MAC Service
        if (mMACUtils.GetMACClientIdUsingOperatorIdAndSiteId(mStiWrapperState, operationData.operatorId.ToString(),
                operationData.siteId.ToString()) == false)
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoClientForSiteId, em.NoClientForSiteId);

        //mMACUtils.EventLogOperRequest(mStiWrapperState);
        try
        {
            mStiWrapperState.stResponseJson = mWrapperStAPI.stapiUpdatePrePaidAccountStatus(mStiWrapperState);
            return mStiWrapperState.stResponseJson;
        }
        catch (Exception ex)
        {
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string stapiGetRegisteredAccounts() // done
    {
        string jsonRequest;

        // Read input json from post stream
        if (HttpContext.Current.Request.InputStream.CanSeek)
            HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            jsonRequest = reader.ReadToEnd();
        // Read input json from post stream

        dynamic operationData = JObject.Parse(jsonRequest);

        var mWrapperStAPI = new WrapperStAPI();
        var mMACUtils = new MACUtils();

        var mMethodName = mMACUtils.GetCurrentMethod();
        var mStiWrapperState = mMACUtils.NewWrapperState(jsonRequest, mMethodName);

        // get client id for site from MAC Service
        if (mMACUtils.GetMACClientIdUsingOperatorIdAndSiteId(mStiWrapperState, operationData.operatorId.ToString(),
                operationData.siteId.ToString()) == false)
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoClientForSiteId, em.NoClientForSiteId);

        //mMACUtils.EventLogOperRequest(mStiWrapperState);
        try
        {
            mStiWrapperState.stResponseJson = mWrapperStAPI.stapiGetRegisteredAccounts(mStiWrapperState);
            return mStiWrapperState.stResponseJson;
        }
        catch (Exception ex)
        {
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string stapiGetPrePaidAccountHolderInfo()
    {
        string jsonRequest;

        // Read input json from post stream
        if (HttpContext.Current.Request.InputStream.CanSeek)
            HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            jsonRequest = reader.ReadToEnd();
        // Read input json from post stream

        dynamic operationData = JObject.Parse(jsonRequest);

        var mWrapperStAPI = new WrapperStAPI();
        var mMACUtils = new MACUtils();

        var mMethodName = mMACUtils.GetCurrentMethod();
        var mStiWrapperState = mMACUtils.NewWrapperState(jsonRequest, mMethodName);

        // get client id for site from MAC Service
        if (mMACUtils.GetMACClientIdUsingOperatorIdAndSiteId(mStiWrapperState, operationData.operatorId.ToString(),
                operationData.siteId.ToString()) == false)
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoClientForSiteId, em.NoClientForSiteId);

        //mMACUtils.EventLogOperRequest(mStiWrapperState);
        try
        {
            mStiWrapperState.stResponseJson = mWrapperStAPI.stapiGetPrePaidAccountHolderInfo(mStiWrapperState);
            return mStiWrapperState.stResponseJson;
        }
        catch (Exception ex)
        {
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        }
    }
    #endregion

    #region ---------- Deposits & Loads -------------------

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string stapiSubmitDepositRequest() // OTP Enabled, status Not done
    {
        string jsonRequest;

        // Read input json from post stream
        if (HttpContext.Current.Request.InputStream.CanSeek)
            HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            jsonRequest = reader.ReadToEnd();
        // Read input json from post stream

        dynamic operationData = JObject.Parse(jsonRequest);

        var mWrapperStAPI = new WrapperStAPI();
        var mMACUtils = new MACUtils();

        var mMethodName = mMACUtils.GetCurrentMethod();
        var mStiWrapperState = mMACUtils.NewWrapperState(jsonRequest, mMethodName);

        // get client id for site from MAC Service
        if (mMACUtils.GetMACClientIdUsingOperatorIdAndSiteId(mStiWrapperState, operationData.operatorId.ToString(),
                operationData.siteId.ToString()) == false)
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoClientForSiteId, em.NoClientForSiteId);

        //mMACUtils.EventLogOperRequest(mStiWrapperState);
        try
        {
            mStiWrapperState.stResponseJson = mWrapperStAPI.stapiSubmitDepositRequest(mStiWrapperState);
            return mStiWrapperState.stResponseJson;
        }
        catch (Exception ex)
        {
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string stapiAuthDepositRequest()
    {
        string jsonRequest;

        // Read input json from post stream
        if (HttpContext.Current.Request.InputStream.CanSeek)
            HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            jsonRequest = reader.ReadToEnd();
        // Read input json from post stream

        dynamic operationData = JObject.Parse(jsonRequest);

        var mWrapperStAPI = new WrapperStAPI();
        var mMACUtils = new MACUtils();

        var mMethodName = mMACUtils.GetCurrentMethod();
        var mStiWrapperState = mMACUtils.NewWrapperState(jsonRequest, mMethodName);

        // get client id for site from MAC Service
        if (mMACUtils.GetMACClientIdUsingOperatorIdAndSiteId(mStiWrapperState, operationData.operatorId.ToString(),
                operationData.siteId.ToString()) == false)
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoClientForSiteId, em.NoClientForSiteId);

        //mMACUtils.EventLogOperRequest(mStiWrapperState);
        try
        {
            mStiWrapperState.stResponseJson = mWrapperStAPI.stapiAuthDepositRequest(mStiWrapperState);
            return mStiWrapperState.stResponseJson;
        }
        catch (Exception ex)
        {
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        }

        // the following logic is to enable OTP for stapiAuthDepositRequest
        //string jsonRequest;

        //// Read input json from post stream
        //if (HttpContext.Current.Request.InputStream.CanSeek)
        //    HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        //using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
        //    jsonRequest = reader.ReadToEnd();
        //// Read input json from post stream

        //dynamic operationData = JObject.Parse(jsonRequest);

        //var mMACUtils = new MACUtils();
        //var mMethodName = mMACUtils.GetCurrentMethod();
        //var mStiWrapperState = mMACUtils.NewWrapperState(jsonRequest, mMethodName);

        //// get client id for site from MAC Service
        //if (mMACUtils.GetMACClientIdUsingOperatorIdAndSiteId(mStiWrapperState, operationData.operatorId.ToString(),
        //        operationData.siteId.ToString()) == false)
        //    return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoClientForSiteId, em.NoClientForSiteId);
        ////mMACUtils.EventLogOperRequest(mStiWrapperState, mMethodName);

        //mStiWrapperState.stTransactionRequest = mMethodName.Replace("stapi", "");
        //// player userName to Wrapper state, used to send otp to registered end user
        //mStiWrapperState.stUserName = operationData.depositDetails.userName.ToString();
        //// get transaction details from request json
        //mStiWrapperState.stTransactionType = operationData.depositDetails.transactionType.ToString();
        //mStiWrapperState.stTransactionAmount = operationData.depositDetails.amount.ToString();
        //string currencySymbol = operationData.depositDetails.currency.ToString();
        //string accountType = operationData.depositDetails.accountType.ToString();
        //// note: changes WrapperState get saved later in MACRequestOTP_RegisteredPlayer

        //var pTransactionDetails = String.Format("{0} Account {1} for {2}{3}",
        //            accountType,
        //            mStiWrapperState.stTransactionType,
        //            GetCurrencySymbolsUsingCurrencyCode(currencySymbol),
        //            mStiWrapperState.stTransactionAmount);
        //try
        //{
        //    //---- send Authorization OTP message to Player's mobile phone ----
        //    var ReqRequestOTPReply = mMACUtils.MACRequestOTP_RegisteredPlayer(mStiWrapperState, mMethodName, stc.TransactionVerification, pTransactionDetails);
        //    if (ReqRequestOTPReply.Item1 == false)
        //        return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.SendOTPError, ReqRequestOTPReply.Item2);

        //    var mResuestOTPResponse = JObject.Parse(ReqRequestOTPReply.Item2);
        //    var RIDProperty = mResuestOTPResponse.Property(stc.RequestId);
        //    if (RIDProperty == null)
        //        return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoRequestId, ReqRequestOTPReply.Item2);
        //    // request id to state
        //    mStiWrapperState.macRequestId = RIDProperty.Value.ToString();
        //    // otp to state if included in response
        //    var OTPProperty = mResuestOTPResponse.Property(stc.OTP);
        //    if (OTPProperty != null) //check if OTPProperty exists
        //        mStiWrapperState.macOTP = OTPProperty.Value.ToString();
        //    // EnterOTPAd to state if included in response
        //    var EnterOTPAdProperty = mResuestOTPResponse.Property(stc.EnterOTPAd);
        //    if (EnterOTPAdProperty != null) //check if EnterOTPAd exists
        //        mStiWrapperState.macEnterOtpAd = EnterOTPAdProperty.Value.ToString();
        //    return mMACUtils.SendOperatorResponse_RequestOTP(mStiWrapperState);
        //}
        //catch (Exception ex)
        //{
        //    return mMACUtils.SendOperatorResponse_SystemError(null, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        //}
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string stapiSubmitDepositAccountRequest() // OTP Enabled, status Not Tested
    {
        string jsonRequest;

        // Read input json from post stream
        if (HttpContext.Current.Request.InputStream.CanSeek)
            HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            jsonRequest = reader.ReadToEnd();
        // Read input json from post stream

        dynamic operationData = JObject.Parse(jsonRequest);
        var mMACUtils = new MACUtils();

        var mMethodName = mMACUtils.GetCurrentMethod();
        var mStiWrapperState = mMACUtils.NewWrapperState(jsonRequest, mMethodName);

        // get client id for site from MAC Service
        if (mMACUtils.GetMACClientIdUsingOperatorIdAndSiteId(mStiWrapperState, operationData.operatorId.ToString(),
                operationData.siteId.ToString()) == false)
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoClientForSiteId, em.NoClientForSiteId);
        //mMACUtils.EventLogOperRequest(mStiWrapperState);
        mStiWrapperState.stTransactionRequest = mMethodName.Replace("stapi", "");
        // player userName to Wrapper state, used to send otp to registered end user
        mStiWrapperState.stUserName = operationData.depositDetails.userName.ToString();
        // get transaction details from request json
        mStiWrapperState.stTransactionType = operationData.depositDetails.transactionType.ToString();
        mStiWrapperState.stTransactionAmount = operationData.depositDetails.amount.ToString();
        string currencySymbol = operationData.depositDetails.currency.ToString();
        string accountType = operationData.depositDetails.accountType.ToString();
        // note: changes WrapperState get saved later in MACRequestOTP_RegisteredPlayer

        var pTransactionDetails = String.Format("{0} Account {1} for {2}{3}",
                    accountType,
                    mStiWrapperState.stTransactionType,
                    GetCurrencySymbolsUsingCurrencyCode(currencySymbol),
                    mStiWrapperState.stTransactionAmount);
        try
        {
            //---- send Authorization OTP message to Player's mobile phone ----
            var ReqRequestOTPReply = mMACUtils.MACRequestOTP_RegisteredPlayer(mStiWrapperState, mMethodName, stc.TransactionVerification, pTransactionDetails);
            if (ReqRequestOTPReply.Item1 == false)
                return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.SendOTPError, ReqRequestOTPReply.Item2);

            var mResuestOTPResponse = JObject.Parse(ReqRequestOTPReply.Item2);
            var RIDProperty = mResuestOTPResponse.Property(stc.RequestId);
            if (RIDProperty == null)
                return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoRequestId, ReqRequestOTPReply.Item2);
            // request id to state
            mStiWrapperState.macRequestId = RIDProperty.Value.ToString();
            // otp to state if included in response
            var OTPProperty = mResuestOTPResponse.Property(stc.OTP);
            if (OTPProperty != null) //check if OTPProperty exists
                mStiWrapperState.macOTP = OTPProperty.Value.ToString();
            // EnterOTPAd to state if included in response
            var EnterOTPAdProperty = mResuestOTPResponse.Property(stc.EnterOTPAd);
            if (EnterOTPAdProperty != null) //check if EnterOTPAd exists
                mStiWrapperState.macEnterOtpAd = EnterOTPAdProperty.Value.ToString();
            return mMACUtils.SendOperatorResponse_RequestOTP(mStiWrapperState);
        }
        catch (Exception ex)
        {
            return mMACUtils.SendOperatorResponse_SystemError(null, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string stapiSubmitPrePaidDeposit() // OTP Enabled, status Not tested
    {
        string jsonRequest;

        // Read input json from post stream
        if (HttpContext.Current.Request.InputStream.CanSeek)
            HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            jsonRequest = reader.ReadToEnd();
        // Read input json from post stream

        dynamic operationData = JObject.Parse(jsonRequest);

        var mMACUtils = new MACUtils();
        var mMethodName = mMACUtils.GetCurrentMethod();
        var mStiWrapperState = mMACUtils.NewWrapperState(jsonRequest, mMethodName);

        // get client id for site from MAC Service
        if (mMACUtils.GetMACClientIdUsingOperatorIdAndSiteId(mStiWrapperState, operationData.operatorId.ToString(),
                operationData.siteId.ToString()) == false)
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoClientForSiteId, em.NoClientForSiteId);
        //mMACUtils.EventLogOperRequest(mStiWrapperState);
        mStiWrapperState.stTransactionRequest = mMethodName.Replace("stapi", "");
        // player userName to Wrapper state, used to send otp to registered end user
        mStiWrapperState.stUserName = operationData.depositDetails.userName.ToString();
        // get transaction details from request json
        mStiWrapperState.stTransactionType = operationData.depositDetails.transactionType.ToString();
        mStiWrapperState.stTransactionAmount = operationData.depositDetails.amount.ToString();
        string currencySymbol = operationData.depositDetails.currency.ToString();
        string accountType = operationData.depositDetails.accountType.ToString();
        // note: changes WrapperState get saved later in MACRequestOTP_RegisteredPlayer

        var pTransactionDetails = String.Format("{0} Account {1} for {2}{3}",
                    accountType,
                    mStiWrapperState.stTransactionType,
                    GetCurrencySymbolsUsingCurrencyCode(currencySymbol),
                    mStiWrapperState.stTransactionAmount);
        try
        {
            //---- send Authorization OTP message to Player's mobile phone ----
            var ReqRequestOTPReply = mMACUtils.MACRequestOTP_RegisteredPlayer(mStiWrapperState, mMethodName, stc.TransactionVerification, pTransactionDetails);
            if (ReqRequestOTPReply.Item1 == false)
                return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.SendOTPError, ReqRequestOTPReply.Item2);

            var mResuestOTPResponse = JObject.Parse(ReqRequestOTPReply.Item2);
            var RIDProperty = mResuestOTPResponse.Property(stc.RequestId);
            if (RIDProperty == null)
                return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoRequestId, ReqRequestOTPReply.Item2);
            // request id to state
            mStiWrapperState.macRequestId = RIDProperty.Value.ToString();
            // otp to state if included in response
            var OTPProperty = mResuestOTPResponse.Property(stc.OTP);
            if (OTPProperty != null) //check if OTPProperty exists
                mStiWrapperState.macOTP = OTPProperty.Value.ToString();
            // EnterOTPAd to state if included in response
            var EnterOTPAdProperty = mResuestOTPResponse.Property(stc.EnterOTPAd);
            if (EnterOTPAdProperty != null) //check if EnterOTPAd exists
                mStiWrapperState.macEnterOtpAd = EnterOTPAdProperty.Value.ToString();
            return mMACUtils.SendOperatorResponse_RequestOTP(mStiWrapperState);
        }
        catch (Exception ex)
        {
            return mMACUtils.SendOperatorResponse_SystemError(null, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string stapiLoadPrePaidFunds()  // OTP Enabled, status loopback
    {
        string jsonRequest;

        // Read input json from post stream
        if (HttpContext.Current.Request.InputStream.CanSeek)
            HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        // Read input json from post stream
        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            jsonRequest = reader.ReadToEnd();

        // parse json input string into Json Object
        dynamic operationData = JObject.Parse(jsonRequest);
        
        var mMACUtils = new MACUtils();
        var mMethodName = mMACUtils.GetCurrentMethod();
        var mStiWrapperState = mMACUtils.NewWrapperState(jsonRequest, mMethodName);
        // get client id to wrapper state, used in MAC Service requests
        if (mMACUtils.GetMACClientIdUsingOperatorIdAndSiteId(mStiWrapperState,
            operationData.operatorId.ToString(), operationData.siteId.ToString()) == false)
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoClientForSiteId, em.NoClientForSiteId);
        //mMACUtils.EventLogOperRequest(mStiWrapperState);
        
        // create transaction details string for request json
        mStiWrapperState.stTransactionRequest = mMethodName.Replace("stapi", "");
        // player userName to Wrapper state, used to send otp to registered end user
        mStiWrapperState.stUserName = operationData.depositDetails.userName.ToString();
        mStiWrapperState.stTransactionType = operationData.depositDetails.transactionType.ToString();
        mStiWrapperState.stTransactionAmount = operationData.depositDetails.amount.ToString(); 
        string currencySymbol = operationData.depositDetails.currency.ToString();
        string accountType = operationData.depositDetails.accountType.ToString();
        // note: changes WrapperState get saved later in MACRequestOTP_RegisteredPlayer

        var pTransactionDetails = String.Format("{0} Account {1} for {2}{3}",
                    accountType,
                    mStiWrapperState.stTransactionType,
                    GetCurrencySymbolsUsingCurrencyCode(currencySymbol),
                    mStiWrapperState.stTransactionAmount);
        try
        {
            //---- send Authorization OTP message to Player's mobile phone ----
            var ReqRequestOTPReply = mMACUtils.MACRequestOTP_RegisteredPlayer(mStiWrapperState, mMethodName, stc.TransactionVerification, pTransactionDetails);
            if (ReqRequestOTPReply.Item1 == false)
                return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.SendOTPError, ReqRequestOTPReply.Item2);

            var mResuestOTPResponse = JObject.Parse(ReqRequestOTPReply.Item2);
            var RIDProperty = mResuestOTPResponse.Property(stc.RequestId);
            if (RIDProperty == null)
                return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoRequestId, ReqRequestOTPReply.Item2);
            // request id to state
            mStiWrapperState.macRequestId = RIDProperty.Value.ToString();
            // otp to state if included in response
            var OTPProperty = mResuestOTPResponse.Property(stc.OTP);
            if (OTPProperty != null) //check if OTPProperty exists
                mStiWrapperState.macOTP = OTPProperty.Value.ToString();
            // EnterOTPAd to state if included in response
            var EnterOTPAdProperty = mResuestOTPResponse.Property(stc.EnterOTPAd);
            if (EnterOTPAdProperty != null) //check if EnterOTPAd exists
                mStiWrapperState.macEnterOtpAd = EnterOTPAdProperty.Value.ToString();
            return mMACUtils.SendOperatorResponse_RequestOTP(mStiWrapperState);
        }
        catch (Exception ex)
        {
            return mMACUtils.SendOperatorResponse_SystemError(null, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        }
    }
    #endregion

    #region ---------- Withdrawals & Refund & Reversal ---------------

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string stapiWithdrawal() // OTP Enabled, status Not tested
    {
        string jsonRequest;

        // Read input json from post stream
        if (HttpContext.Current.Request.InputStream.CanSeek)
            HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            jsonRequest = reader.ReadToEnd();

        dynamic operationData = JObject.Parse(jsonRequest);
        var mMACUtils = new MACUtils();

        var mMethodName = mMACUtils.GetCurrentMethod();
        var mStiWrapperState = mMACUtils.NewWrapperState(jsonRequest, mMethodName);

        // get client id for site from MAC Service
        if (mMACUtils.GetMACClientIdUsingOperatorIdAndSiteId(mStiWrapperState, operationData.operatorId.ToString(),
                operationData.siteId.ToString()) == false)
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoClientForSiteId, em.NoClientForSiteId);
        //mMACUtils.EventLogOperRequest(mStiWrapperState);
 
        // create transaction details string for request json
        mStiWrapperState.stTransactionRequest = mMethodName.Replace("stapi", "");
        // player userName to Wrapper state, used to send otp to registered end user
        mStiWrapperState.stUserName = operationData.withdrawalDetails.userName.ToString();
        mStiWrapperState.stTransactionType = operationData.withdrawalDetails.transactionType.ToString();
        mStiWrapperState.stTransactionAmount = operationData.withdrawalDetails.amount.ToString();
        string currencySymbol = operationData.withdrawalDetails.currency.ToString();
        string accountType = operationData.withdrawalDetails.accountType.ToString();
        // note: changes WrapperState get saved later in MACRequestOTP_RegisteredPlayer

        var pTransactionDetails = String.Format("{0} Account {1} for {2}{3}",
                    accountType,
                    mStiWrapperState.stTransactionType,
                    GetCurrencySymbolsUsingCurrencyCode(currencySymbol),
                    mStiWrapperState.stTransactionAmount);
        try
        {
            //---- send Authorization OTP message to Player's mobile phone ----
            var ReqRequestOTPReply = mMACUtils.MACRequestOTP_RegisteredPlayer(mStiWrapperState, mMethodName, stc.TransactionVerification, pTransactionDetails);
            if (ReqRequestOTPReply.Item1 == false)
                return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.SendOTPError, ReqRequestOTPReply.Item2);

            var mResuestOTPResponse = JObject.Parse(ReqRequestOTPReply.Item2);
            var RIDProperty = mResuestOTPResponse.Property(stc.RequestId);
            if (RIDProperty == null)
                return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoRequestId, ReqRequestOTPReply.Item2);
            // request id to state
            mStiWrapperState.macRequestId = RIDProperty.Value.ToString();
            // otp to state if included in response
            var OTPProperty = mResuestOTPResponse.Property(stc.OTP);
            if (OTPProperty != null) //check if OTPProperty exists
                mStiWrapperState.macOTP = OTPProperty.Value.ToString();
            // EnterOTPAd to state if included in response
            var EnterOTPAdProperty = mResuestOTPResponse.Property(stc.EnterOTPAd);
            if (EnterOTPAdProperty != null) //check if EnterOTPAd exists
                mStiWrapperState.macEnterOtpAd = EnterOTPAdProperty.Value.ToString();
            return mMACUtils.SendOperatorResponse_RequestOTP(mStiWrapperState);
        }
        catch (Exception ex)
        {
            return mMACUtils.SendOperatorResponse_SystemError(null, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string stapiAccountWithdrawal() // OTP Enabled, status Not tested
    {
        string jsonRequest;

        // Read input json from post stream
        if (HttpContext.Current.Request.InputStream.CanSeek)
            HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            jsonRequest = reader.ReadToEnd();
        // Read input json from post stream

        dynamic operationData = JObject.Parse(jsonRequest);

        var mMACUtils = new MACUtils();
        var mMethodName = mMACUtils.GetCurrentMethod();
        var mStiWrapperState = mMACUtils.NewWrapperState(jsonRequest, mMethodName);

        // get client id for site from MAC Service
        if (mMACUtils.GetMACClientIdUsingOperatorIdAndSiteId(mStiWrapperState, operationData.operatorId.ToString(),
                operationData.siteId.ToString()) == false)
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoClientForSiteId, em.NoClientForSiteId);
        //mMACUtils.EventLogOperRequest(mStiWrapperState);

        // create transaction details string for request json
        mStiWrapperState.stTransactionRequest = mMethodName.Replace("stapi", "");
        // player userName to Wrapper state, used to send otp to registered end user
        mStiWrapperState.stUserName = operationData.withdrawalDetails.userName.ToString();
        mStiWrapperState.stTransactionType = operationData.withdrawalDetails.transactionType.ToString();
        mStiWrapperState.stTransactionAmount = operationData.withdrawalDetails.amount.ToString();
        string currencySymbol = operationData.withdrawalDetails.currency.ToString();
        string accountType = operationData.withdrawalDetails.accountType.ToString();
        // note: changes WrapperState get saved later in MACRequestOTP_RegisteredPlayer

        var pTransactionDetails = String.Format("{0} Account {1} for {2}{3}",
                    accountType,
                    mStiWrapperState.stTransactionType,
                    GetCurrencySymbolsUsingCurrencyCode(currencySymbol),
                    mStiWrapperState.stTransactionAmount);
        try
        {
            //---- send Authorization OTP message to Player's mobile phone ----
            var ReqRequestOTPReply = mMACUtils.MACRequestOTP_RegisteredPlayer(mStiWrapperState, mMethodName, stc.TransactionVerification, pTransactionDetails);
            if (ReqRequestOTPReply.Item1 == false)
                return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.SendOTPError, ReqRequestOTPReply.Item2);

            var mResuestOTPResponse = JObject.Parse(ReqRequestOTPReply.Item2);
            var RIDProperty = mResuestOTPResponse.Property(stc.RequestId);
            if (RIDProperty == null)
                return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoRequestId, ReqRequestOTPReply.Item2);
            // request id to state
            mStiWrapperState.macRequestId = RIDProperty.Value.ToString();
            // otp to state if included in response
            var OTPProperty = mResuestOTPResponse.Property(stc.OTP);
            if (OTPProperty != null) //check if OTPProperty exists
                mStiWrapperState.macOTP = OTPProperty.Value.ToString();
            // EnterOTPAd to state if included in response
            var EnterOTPAdProperty = mResuestOTPResponse.Property(stc.EnterOTPAd);
            if (EnterOTPAdProperty != null) //check if EnterOTPAd exists
                mStiWrapperState.macEnterOtpAd = EnterOTPAdProperty.Value.ToString();
            return mMACUtils.SendOperatorResponse_RequestOTP(mStiWrapperState);
        }
        catch (Exception ex)
        {
            return mMACUtils.SendOperatorResponse_SystemError(null, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string stapiSubmitPrePaidWithdrawal() // OTP Enabled, status Not done
    {
        string jsonRequest;

        // Read input json from post stream
        if (HttpContext.Current.Request.InputStream.CanSeek)
            HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            jsonRequest = reader.ReadToEnd();
        // Read input json from post stream

        dynamic operationData = JObject.Parse(jsonRequest);

        var mMACUtils = new MACUtils();

        var mMethodName = mMACUtils.GetCurrentMethod();
        var mStiWrapperState = mMACUtils.NewWrapperState(jsonRequest, mMethodName);
        // get client id to wrapper state, used in MAC Service requests
        if (mMACUtils.GetMACClientIdUsingOperatorIdAndSiteId(mStiWrapperState, operationData.operatorId.ToString(),
                                        operationData.siteId.ToString()) == false)
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoClientForSiteId, em.NoClientForSiteId);
        //mMACUtils.EventLogOperRequest(mStiWrapperState);
        
        mStiWrapperState.stTransactionRequest = mMethodName.Replace("stapi", "");
        // player userName to Wrapper state, used to send otp to registered end user
        mStiWrapperState.stUserName = operationData.withdrawalDetails.userName.ToString();
        mStiWrapperState.stTransactionType = operationData.withdrawalDetails.transactionType.ToString();
        mStiWrapperState.stTransactionAmount = operationData.withdrawalDetails.amount.ToString();
        string currencySymbol = operationData.withdrawalDetails.currency.ToString();
        string accountType = operationData.withdrawalDetails.accountType.ToString();
        // note: changes WrapperState get saved later in MACRequestOTP_RegisteredPlayer

        var pTransactionDetails = String.Format("{0} Account {1} for {2}{3}",
                    accountType,
                    mStiWrapperState.stTransactionType,
                    GetCurrencySymbolsUsingCurrencyCode(currencySymbol),
                    mStiWrapperState.stTransactionAmount);
        try
        {
            //---- send Authorization OTP message to Player's mobile phone ----
            var ReqRequestOTPReply = mMACUtils.MACRequestOTP_RegisteredPlayer(mStiWrapperState, mMethodName, stc.TransactionVerification, pTransactionDetails);
            if (ReqRequestOTPReply.Item1 == false)
                return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.SendOTPError, ReqRequestOTPReply.Item2);

            var mResuestOTPResponse = JObject.Parse(ReqRequestOTPReply.Item2);
            var RIDProperty = mResuestOTPResponse.Property(stc.RequestId);
            if (RIDProperty == null)
                return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoRequestId, ReqRequestOTPReply.Item2);
            // request id to state
            mStiWrapperState.macRequestId = RIDProperty.Value.ToString();
            // otp to state if included in response
            var OTPProperty = mResuestOTPResponse.Property(stc.OTP);
            if (OTPProperty != null) //check if OTPProperty exists
                mStiWrapperState.macOTP = OTPProperty.Value.ToString();
            // EnterOTPAd to state if included in response
            var EnterOTPAdProperty = mResuestOTPResponse.Property(stc.EnterOTPAd);
            if (EnterOTPAdProperty != null) //check if EnterOTPAd exists
                mStiWrapperState.macEnterOtpAd = EnterOTPAdProperty.Value.ToString();
            return mMACUtils.SendOperatorResponse_RequestOTP(mStiWrapperState);
        }
        catch (Exception ex)
        {
            return mMACUtils.SendOperatorResponse_SystemError(null, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        }
    }
    
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string stapiSubmitRefund()  // status Not tested
    {
        var mMACUtils = new MACUtils();
        var mWrapperStAPI = new WrapperStAPI();
        string jsonRequest;


        StiWrapperState mStiWrapperState = null;
        // Read input json from post stream
        if (HttpContext.Current.Request.InputStream.CanSeek)
            HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            jsonRequest = reader.ReadToEnd();
        
        var mMethodName = mMACUtils.GetCurrentMethod();
        mStiWrapperState = mMACUtils.NewWrapperState(jsonRequest, mMethodName);
        string operatorId;
        string siteId;
        // Read input json from post stream
        try
        {
            dynamic operationData = JObject.Parse(jsonRequest);
            operatorId = operationData.operatorId.ToString();
            siteId = operationData.siteId.ToString();

        }
        catch (Exception ex)
        {
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.InvalidRequest, ex.Message);
        }

        // get client id for site from MAC Service
        if (mMACUtils.GetMACClientIdUsingOperatorIdAndSiteId(mStiWrapperState, operatorId, siteId) == false)
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoClientForSiteId, em.NoClientForSiteId);

        //mMACUtils.EventLogOperRequest(mStiWrapperState);
        try
        {
            mStiWrapperState.stResponseJson = mWrapperStAPI.stapiSubmitRefund(mStiWrapperState);
            return mStiWrapperState.stResponseJson;
        }
        catch (Exception ex)
        {
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string stapiSubmitReversal()  // status Not tested
    {
        string jsonRequest;

        // Read input json from post stream
        if (HttpContext.Current.Request.InputStream.CanSeek)
            HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            jsonRequest = reader.ReadToEnd();
        // Read input json from post stream

        dynamic operationData = JObject.Parse(jsonRequest);

        var mWrapperStAPI = new WrapperStAPI();
        var mMACUtils = new MACUtils();

        var mMethodName = mMACUtils.GetCurrentMethod();
        var mStiWrapperState = mMACUtils.NewWrapperState(jsonRequest, mMethodName);

        // get client id for site from MAC Service
        if (mMACUtils.GetMACClientIdUsingOperatorIdAndSiteId(mStiWrapperState, operationData.operatorId.ToString(),
                operationData.siteId.ToString()) == false)
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoClientForSiteId, em.NoClientForSiteId);

        //mMACUtils.EventLogOperRequest(mStiWrapperState);
        try
        {
            mStiWrapperState.stResponseJson = mWrapperStAPI.stapiSubmitReversal(mStiWrapperState);
            return mStiWrapperState.stResponseJson;
        }
        catch (Exception ex)
        {
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        }
    }
    #endregion

    #region ---------- Transaction ------------------------

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string stapiGetTransactionDetails() // done
    {
        string jsonRequest;

        // Read input json from post stream
        if (HttpContext.Current.Request.InputStream.CanSeek)
            HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            jsonRequest = reader.ReadToEnd();
        // Read input json from post stream

        dynamic operationData = JObject.Parse(jsonRequest);

        var mWrapperStAPI = new WrapperStAPI();
        var mMACUtils = new MACUtils();

        var mMethodName = mMACUtils.GetCurrentMethod();
        var mStiWrapperState = mMACUtils.NewWrapperState(jsonRequest, mMethodName);

        // get client id for site from MAC Service
        if (mMACUtils.GetMACClientIdUsingOperatorIdAndSiteId(mStiWrapperState, operationData.operatorId.ToString(),
                operationData.siteId.ToString()) == false)
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoClientForSiteId, em.NoClientForSiteId);

        //mMACUtils.EventLogOperRequest(mStiWrapperState);
        try
        {
            mStiWrapperState.stResponseJson = mWrapperStAPI.stapiGetTransactionDetails(mStiWrapperState);
            return mStiWrapperState.stResponseJson;
        }
        catch (Exception ex)
        {
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string stapiUpdateTransaction()
    {
        string jsonRequest;

        // Read input json from post stream
        if (HttpContext.Current.Request.InputStream.CanSeek)
            HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            jsonRequest = reader.ReadToEnd();
        // Read input json from post stream

        dynamic operationData = JObject.Parse(jsonRequest);

        var mWrapperStAPI = new WrapperStAPI();
        var mMACUtils = new MACUtils();

        var mMethodName = mMACUtils.GetCurrentMethod();
        var mStiWrapperState = mMACUtils.NewWrapperState(jsonRequest, mMethodName);

        // get client id for site from MAC Service
        if (mMACUtils.GetMACClientIdUsingOperatorIdAndSiteId(mStiWrapperState, operationData.operatorId.ToString(),
                operationData.siteId.ToString()) == false)
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoClientForSiteId, em.NoClientForSiteId);

        //mMACUtils.EventLogOperRequest(mStiWrapperState);
        try
        {
            mStiWrapperState.stResponseJson = mWrapperStAPI.stapiUpdateTransaction(mStiWrapperState);
            return mStiWrapperState.stResponseJson;
        }
        catch (Exception ex)
        {
            return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
        }
    }
    #endregion

    //[WebMethod]
    //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    //public string stapiDeletePlayer()
    //{
    //    string jsonRequest;

    //    // Read input json from post stream
    //    if (HttpContext.Current.Request.InputStream.CanSeek)
    //        HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);

    //    using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
    //        jsonRequest = reader.ReadToEnd();
    //    // Read input json from post stream

    //    dynamic operationData = JObject.Parse(jsonRequest);

    //    var mWrapperStAPI = new WrapperStAPI();
    //    var mMACUtils = new MACUtils();

    //    var mMethodName = mMACUtils.GetCurrentMethod();
    //    var mStiWrapperState = mMACUtils.NewWrapperState(jsonRequest, mMethodName);


    //    //var mWrapperStAPI = new WrapperStAPI();
    //    //var mMACUtils = new MACUtils();

    //    //var mMethodName = mMACUtils.GetCurrentMethod();

    //    //var mySerializedRequest = JsonConvert.SerializeObject(operationData);

    //    //var mStiWrapperState = mMACUtils.NewWrapperState(mySerializedRequest, mMethodName);

    //    //// get client id for site
    //    //if (mMACUtils.GetMACClientIdUsingOperatorIdAndSiteId(mStiWrapperState, operationData.operatorId.ToString(), operationData.siteId.ToString()) == false)
    //    //    return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NoClientForSiteId, em.NoClientForSiteId);

    //    ////mMACUtils.EventLogOperRequest(mStiWrapperState, mMethodName);

    //    //try
    //    //{
    //    //    var OTPProperty = operationData.userName;
    //    //    if (OTPProperty == null) //is player Id in request
    //    //        return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.NostPlayerId, em.NostPlayerId);
    //    //    mStiWrapperState.stPlayerId = OTPProperty;

    //    //    var ReqDeleteEndUserReply = mMACUtils.MACDeleteEndUserByUserId(mStiWrapperState, mMethodName);
    //    //    if (ReqDeleteEndUserReply.Item1 == false)
    //    //        return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.DeletePlayerError, ReqDeleteEndUserReply.Item2);
    //    //    return mMACUtils.SendOperatorResponse_PlayerDeleted(ic.PlayerDeleted, im.PlayerDeleted);
    //    //}
    //    //catch (Exception ex)
    //    //{
    //    //    return mMACUtils.SendOperatorResponse_SystemError(mStiWrapperState, ec.Exception, em.Exception + mMethodName + ":" + ex.ToString());
    //    //}

    //    return "";
    //}

    #region Helper Methods
    static string GetCurrencySymbolsUsingCurrencyCode(string pCurrencyCode)
    {
        if (String.IsNullOrEmpty(pCurrencyCode)) return "$";
        var symbols = new Dictionary<string, string>();
        foreach (var ci in CultureInfo.GetCultures(CultureTypes.AllCultures))
        {
            try
            {
                var ri = new RegionInfo(ci.Name);
                symbols[ri.ISOCurrencySymbol] = ri.CurrencySymbol;
            }
            catch
            {
                return "$";
            }
        }
        try
        {
            return symbols[pCurrencyCode];
        }
        catch (Exception)
        {
            return "$";
        }
    }
    #endregion

}
