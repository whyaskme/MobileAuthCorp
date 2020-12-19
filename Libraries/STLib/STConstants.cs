using MAC_sr = MACServices.Constants.ServiceResponse;
using MAC_dk = MACServices.Constants.Dictionary.Keys;
using MAC_dv = MACServices.Constants.Dictionary.Values;
using MAC_dkui = MACServices.Constants.Dictionary.Userinfo;
using MAC_cs = MACServices.Constants.Strings;
using MAC_svcs = MACServices.Constants.ServiceUrls;
using MAC_regX = MACServices.Constants.RegexStrings;
using MAC_cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using MAC_DocTp = MACServices.Constants.DocumentTemplateReplacementTokens;

namespace STLib
{
    public static class STConstants
    {
        public static class Strings
        {
            public const string DefaultSTPlayerId = "000000000000000000000";
        }

        public static class Config
        {
            public const string StWrapperServiceUrl = "StWrapperServiceUrl";
            public const string STServicesUrl = "STServicesUrl";
            public const string MacServicesUrl = "MacServicesUrl";
            public const string StLoopbackServiceUrl = "StLoopbackServiceUrl";
            public const string GetMACClientIdBySTSiteId = "/AdminServices/ClientServices.asmx/WsGetMACClientIdBySTSiteId";
        }

        public static class MACInfoCodes
        {
            public const string PlayerDeleted   = "INFO_MAC_10021";
            public const string EnterOTP        = "INFO_MAC_20021";
            public const string Re_enter        = "INFO_MAC_30021";
        }
        public static class MACInfoMessages
        {
            public const string PlayerDeleted  = "Player deleted";
            public const string EnterOTP       = "Please enter OTP!";
            public const string Re_enter       = "OTP Invalid, Please re-enter!";
        }

        public static class MACErrorCodes
        {
            public const string NoCreateInResponse = "ERR_MAC_10045";
            public const string RegError           = "ERR_MAC_20045";
            public const string SendOTPError       = "ERR_MAC_30045"; //error message returned for MAC
            public const string NoRequestId        = "ERR_MAC_40045";
            public const string InvalidResponse    = "ERR_MAC_50045";
            public const string Exception          = "ERR_MAC_60045";
            public const string NoClientForSiteId  = "ERR_MAC_70045";
            public const string NostPlayerId       = "ERR_MAC_80045";
            public const string DeletePlayerError  = "ERR_MAC_90045";
            public const string NoRequestToken     = "ERR_MAC_10045";
            public const string InvalidWrapperId   = "ERR_MAC_11045";
            public const string VerifyOTPError     = "ERR_MAC_12045"; //error message returned for MAC
            public const string OTPDisabled        = "ERR_MAC_13045";
            public const string OTPTimeout         = "ERR_MAC_14045";
            public const string OTPInactive        = "ERR_MAC_15045";
            public const string InvalidRequest     = "ERR_MAC_16045";
            public const string ResendOTPError     = "ERR_MAC_17045"; //error message returned for MAC
            public const string NoPlayerDetails    = "ERR_MAC_18045";
            public const string NoPlayerNotRegisteredWithMAC = "ERR_MAC_19045";
        
        }

        public static class MACErrorMessages
        {
            public const string NoCreateInResponse = "Could not register player, wasn't created!";
            public const string InvalidResponse = "invalid response from ST!";
            public const string Exception = "Exception.";
            public const string NoClientForSiteId = "System Error - Could not find MAC Client using Site Id.";
            public const string NostPlayerId = "No Player Id in request! ";
            public const string NoRequestToken = "No Reference Number in request! ";
            public const string InvalidWrapperId = "Invalid Wrapper State Id! ";
            public const string OTPDisabled = "Invalid OTP, Too many retries! ";
            public const string OTPInactive = "OTP inactive! ";
            public const string OTPTimeout = "OTP Timed out! ";
            public const string OTPInvalidRequest = "System Error - Invalid request! ";
            public const string NoPlayerDetails = "No Player Details";
        }

        public static class EventTypes
        {
            public const string OperRequestWrapper = "Oper Request Wrapper";
            public const string WrapperResponesOper = "Wrapper Response Oper";

            public const string WrapperRequestST = "Wrapper Request ST";
            public const string WrapperRequestLB = "Wrapper Request Loopback";
            public const string STResponseWrapper = "ST Response Wrapper";

            public const string MACResponseWrapper = "MAC Response Wrapper";
            public const string WrapperRequestMAC = "Wrapper Request MAC";
            public const string LoopbackResponse = "Loopback Respones";
        }


        public static class MACConstantsToWrapperConstants
        {
            // MAC Dictionary Keys
            public const string ItemSep = MAC_dk.ItemSep;       // Pipe(|) used as item seperator
            public const string KVSep = MAC_dk.KVSep;
            public const string CID = MAC_dk.CID;
            public const string API = MAC_dk.API;
            public const string UserId = MAC_dk.UserId;
            public const string RegistrationType = MAC_dk.RegistrationType;
            // MAC Dictionary Values
            public const string EndUserRegister = MAC_dv.EndUserRegister;
            public const string SendOtp = MAC_dv.SendOtp;
            public const string VerifyOtp = MAC_dv.VerifyOtp;
            public const string ResendOtp = MAC_dv.ResendOtp;
            public const string CheckEndUserRegistration = MAC_dv.CheckEndUserRegistration;
            public const string DeleteEndUser = MAC_dv.DeleteEndUser;
            public const string TrxType = MAC_dk.TrxType;
            public const string TrxDetails = MAC_dk.TrxDetails;
            public const string GetClientId = MAC_dv.GetClientId;
            public const string ClientRegister = MAC_dv.ClientRegister;
            // MAC Service responses
            public const string Reply = MAC_sr.Reply;
            public const string Request = MAC_sr.Request;
            public const string Error = MAC_sr.Error;
            public const string Debug = MAC_sr.Debug;
            public const string RID = MAC_sr.RequestId;
            public const string EnterOTPAd = MAC_sr.EnterOTPAd;
            public const string TLM = MAC_sr.TLM; // time to live minutes
            public const string ClientName = MAC_sr.ClientName;
            public const string OTPRetriesMax = MAC_sr.OTPRetriesMax;
            public const string OTPExpiredTime = MAC_sr.OTPExpiredTime;
            public const string RequestId = MAC_sr.RequestId;
            public const string OTP = MAC_sr.OTP;
            public const string Details = MAC_sr.Details;
            public const string Validated = MAC_sr.Validated;
            public const string Invalid = MAC_sr.Invalid;
            public const string Timeout = MAC_sr.Timeout;
            public const string Disabled = MAC_sr.Disabled;
            public const string Resent = MAC_sr.Resent;
            public const string Inactive = MAC_sr.Inactive;
            public const string Sent = MAC_sr.Sent;
            // MAC String constants
            public const string DefaultClientId = MAC_cs.DefaultClientId;

            public const string VerifyOtpWebService = MAC_svcs.VerifyOtpWebService;
            public const string SecureTraidingRegisterUserWebService = MAC_svcs.SecureTraidingRegisterUserWebService;
            public const string RequestOtpWebService = MAC_svcs.RequestOtpWebService;
            public const string MacClientServices = MAC_svcs.MacClientServices;
            public const string MacEndUserManagement = MAC_svcs.EndUserManagementWebService;
            
            // Player information
            public const string FirstName = MAC_dkui.FirstName;
            public const string LastName = MAC_dkui.LastName;
            public const string PhoneNumber = MAC_dkui.PhoneNumber;
            public const string EmailAddress = MAC_dkui.EmailAddress;
            public const string MiddleName = MAC_dkui.MiddleName;
            public const string Suffix = MAC_dkui.Suffix;
            public const string DOB = MAC_dkui.DOB;
            public const string SSN4 = MAC_dkui.SSN4;
            public const string Street = MAC_dkui.Street;
            public const string Street2 = MAC_dkui.Street2;
            public const string Unit = MAC_dkui.Unit;
            public const string City = MAC_dkui.City;
            public const string State = MAC_dkui.State;
            public const string ZipCode = MAC_dkui.ZipCode;
            public const string DriverLic = MAC_dkui.DriverLic;
            public const string DriverLicSt = MAC_dkui.DriverLicSt;
            public const string Country = MAC_dkui.Country;
            public const string EndUserIpAddress = MAC_dkui.EndUserIpAddress;

            public const string regX_EmailAddress = MAC_regX.EmailAddress;
            public const string regX_PhoneNumber = MAC_regX.PhoneNumber;
            // MAC Document Message Templates
            public const int Generic = MAC_DocTp.Generic;
            public const int Authentication = MAC_DocTp.Authentication;
            public const int TransactionVerification = MAC_DocTp.TransactionVerification;
            public const int RegistrationOTP = MAC_DocTp.RegistrationOTP;
            public const int Notification = MAC_DocTp.Notification;
        }

        public static class ResponsesTemplates
        {
            public const string MACSuccessTemplates =
            "{"
                + "\"returnCode\":\"[EC]\","
                + "\"returnMsg\":\"Success – [ET].\","
                + "\"stTimeStamp\":\"[DT]\""
            + "}";

            public const string SystemErrorTemplates =
            "{"
                + "\"returnCode\":\"[EC]\","
                + "\"returnMsg\":\"System Error – [ET]. Please contact Secure Trading Inc. Support.\","
                + "\"stTimeStamp\":\"[DT]\""
            + "}";
        }

    }

}
