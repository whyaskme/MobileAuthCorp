using System;
using System.Configuration;
using System.Web;

using MACServices;
using dk = MACServices.Constants.Dictionary.Keys;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using sr = MACServices.Constants.ServiceResponse;

namespace MACAdmin.Otp
{
    public partial class RequestOtpPopup : System.Web.UI.Page
    {
        private string _clientId = "";
        private string _clientName = "";
        private string _phoneToSend = "";
        private string _emailToSend = "";
        private string _loggedInAdminId = "";
        private string _userName = "";

        private UserProfile _myProfile;

        private Utils myUtils = new Utils();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Deny access if user request not logged in
            var isAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
            if (!isAuthenticated)
            {
                Response.Write("Hello world!");
                Response.End();
            }

            if (!IsPostBack)
            {
                try
                {
                    _loggedInAdminId = Request.QueryString["loggedInAdminId"] ?? "52aa5bfe675c9b04c00e02bf";

                    _loggedInAdminId = MACSecurity.Security.DecodeAndDecrypt(_loggedInAdminId, Constants.Strings.DefaultClientId);

                    _myProfile = new UserProfile(_loggedInAdminId);

                    _userName = myUtils.GetUserNameByUserId(_loggedInAdminId);
                    _phoneToSend = MACSecurity.Security.DecodeAndDecrypt(_myProfile.Contact.MobilePhone, _loggedInAdminId);
                    _emailToSend = MACSecurity.Security.DecodeAndDecrypt(_myProfile.Contact.Email, _loggedInAdminId);

                    _clientId = Request.QueryString["clientId"] ?? "52aa5bfe675c9b04c00e02bf";

                    var myClient = new Client(_clientId);
                    _clientName = myClient.Name;

                    hiddenD.Value = _clientId;
                    hiddenO.Value = _clientName;

                    var myMacotp = new MacOtp.MacOtp();
                    var sReply = myMacotp.SendOtpToAdminUser
                    (
                        ConfigurationManager.AppSettings[cfg.MacServicesUrl],
                        _userName.Trim(),
                        _clientId
                    );

                    var mReplyItems = sReply.Split(char.Parse(dk.ItemSep));
                    foreach (var mReplyItem in mReplyItems)
                    {
                        if (mReplyItem.StartsWith(sr.RequestId))
                            hiddenRequestID.Value = mReplyItem.Replace(sr.RequestId + "=", "");

                        // Pass the encrypted OTP to the hiddenfield for QA testing
                        if (mReplyItem.StartsWith("OTP"))
                            hiddenAD.Value = MACSecurity.Security.EncryptAndEncode(mReplyItem.Replace("OTP=", ""), Constants.Strings.DefaultClientId);

                        if (mReplyItem.StartsWith(sr.DeliveryMethod))
                        {
                            var deliveryType = mReplyItem.Replace(sr.DeliveryMethod + "=", "");
                            switch (deliveryType)
                            {
                                case Constants.Strings.Email:
                                    spanOtpMessage.InnerHtml = "Enter the Otp we sent to your<br />Email at (" + _emailToSend + ")";
                                    break;
                                case Constants.Strings.Sms:
                                    spanOtpMessage.InnerHtml = "Enter the Otp we sent to your<br />Phone at (" + _phoneToSend + ")";
                                    break;
                                case Constants.Strings.Voice:
                                    spanOtpMessage.InnerHtml = "Enter the Otp we sent to your<br />Phone at (" + _phoneToSend + ")";
                                    break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    var mDetails = ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                    var exceptionEvent = new Event
                    {
                        EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                    };
                    exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                    divServiceResponse.InnerHtml = ex.Message;
                }
            }
            else
            {
                try
                {
                    var myOtpCode = txtOtp.Text;
                    var myMacotp = new MacOtp.MacOtp();
                    var sReply = myMacotp.VerifyOtp(
                        ConfigurationManager.AppSettings[cfg.MacServicesUrl], 
                        hiddenD.Value, 
                        hiddenRequestID.Value, 
                        myOtpCode);

                    string validationResult;

                    if (sReply.Contains("Validated"))
                        validationResult = "<div class='alert-box success radius'>OTP validated!</div>";
                    else
                        validationResult = "<div class='alert-box alert radius'>OTP (Invalidated)</div>";

                    divServiceResponse.InnerHtml = validationResult;
                }
                catch (Exception ex)
                {
                    var mDetails = ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                    var exceptionEvent = new Event
                    {
                        EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                    };
                    exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                    divServiceResponse.InnerHtml = ex.Message;
                }
            }
        }
    }
}