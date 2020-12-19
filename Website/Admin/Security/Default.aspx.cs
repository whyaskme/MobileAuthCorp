using System;
using System.Configuration;
using System.Text;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using MACServices;
using dk = MACServices.Constants.Dictionary.Keys;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using sr = MACServices.Constants.ServiceResponse;

namespace Admin.Security
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Utils myUtils = new Utils();

            if (Page.Master == null) return;
            var displayMode = (HiddenField)Page.Master.FindControl("hiddenU");
            var hiddenA = (HiddenField)Page.Master.FindControl("hiddenA");
            var hiddenV = (HiddenField)Page.Master.FindControl("hiddenV");
            var hiddenAE = (HiddenField)Page.Master.FindControl("hiddenAE");

            // Check and update web.config if needed
            var bIsUpdated = myUtils.SetWebConfig("");
            if (bIsUpdated)
            {
                // Automatically click the logo to force a page refresh to load the new config without intervention
                ClientScript.RegisterStartupScript(typeof(Page), "closePage", "<script type='text/JavaScript'>refreshPageAfterConfigUpdate();</script>");
            }

            // Set environment label
            var environmentLabel = myUtils.SetEnvironmentLabel();

            var divRunningEnvironment_Desktop = (HtmlContainerControl)Page.Master.FindControl("divRunningEnvironment_Desktop");
            var divRunningEnvironment_Mobile = (HtmlContainerControl)Page.Master.FindControl("divRunningEnvironment_Mobile");

            divRunningEnvironment_Desktop.InnerHtml = environmentLabel + " Environment";
            divRunningEnvironment_Mobile.InnerHtml = environmentLabel + " Environment";

            var myForm = (HtmlForm)Page.Master.FindControl("formMain");

            var membershipId = "";

            if (Request["debug"] != null)
                hiddenA.Value = Request["debug"].ToLower();

            if (LinkUseExistingCredentials_Desktop != null)
                LinkUseExistingCredentials_Desktop.Visible = Convert.ToBoolean(hiddenA.Value);

            if (LinkUseExistingCredentials_Mobile != null)
                LinkUseExistingCredentials_Mobile.Visible = Convert.ToBoolean(hiddenA.Value);

            if (Request.ServerVariables["SERVER_NAME"] == "localhost")
            {
                // ReSharper disable once PossibleNullReferenceException
                LinkUseExistingCredentials_Desktop.Visible = true;
                // ReSharper disable once PossibleNullReferenceException
                LinkUseExistingCredentials_Mobile.Visible = true;
            }

            try
            {
                if (!IsPostBack)
                {
                    if (Request.ServerVariables["SERVER_NAME"].ToLower() == "localhost")
                        // This is commented out for security reasons.
                        txtUsername_Desktop.Value = "system@mobileauthcorp.com";
                    else
                        txtUsername_Desktop.Value = "";

                    if (Request["uname"] != null)
                    {
                        txtUsername_Desktop.Value = Request["uname"].ToLower();
                        txtUsername_Mobile.Value = Request["uname"].ToLower();
                    }
                }
                else
                {
                    var userLoginName = hiddenAB.Value.Trim();

                    var adminUser = Membership.GetUser(userLoginName);
                    if (adminUser != null)
                    {
                        if (!adminUser.IsApproved)
                        {
                            // Display error message
                            AdminLoginResult_Desktop.InnerHtml = "<span style='color: #ff0000; font-weight: normal;'>ERROR: Cannot log into this account.<br />Please contact an administrator.</span>";
                            var invalidAccountEvent = new Event();

                            invalidAccountEvent.EventTypeDesc += "";
                        }
                        else
                        {
                            if (hiddenAA.Value == "") return;

                            var myMacOtp = new MacOtp.MacOtp();
                            var sReply = myMacOtp.SendOtpToAdminUser(ConfigurationManager.AppSettings[cfg.MacServicesUrl], userLoginName, "");

                            if (sReply.StartsWith("Error"))
                            {
                                hiddenZ.Value = "";

                                txtUsername_Desktop.Value = "";
                                txtUsername_Desktop.Style.Add("border-color", "#ff0000;");
                                txtUsername_Desktop.Focus();
                                btnAdminOtpRequest_Desktop.Value = "Request OTP";

                                // construct a span containing the error message
                                var mTheError = new StringBuilder();
                                mTheError.Append("<span id='spanLoginResultDetails' style='color: #ff0000; font-weight: bold'>");
                                if (sReply.Contains("Blocked") && (sReply.Contains(sr.STOP)))
                                {   //Error: Not sent, Blocked user replied 'STOP' (FromNumber=244-687)
                                    // Isolate the from number (should be the short code) in the error message
                                    var idx = sReply.IndexOf(sr.FromNumber, StringComparison.Ordinal) + sr.FromNumber.Length + 1;
                                    var mReplyNumber = sReply.Substring(idx, sReply.Length - idx - 1);
                                    mTheError.Append("Your OTP text message was blocked.<br />To unblock text OPTIN to ");
                                    mTheError.Append(mReplyNumber);
                                }
                                else
                                {
                                    mTheError.Append(hiddenA.Value.ToLower() == "true" ? sReply : "Invalid User");
                                }
                                mTheError.Append("!</span>");
                                AdminLoginResult_Desktop.InnerHtml = mTheError.ToString();
                            }
                            else
                            {
                                var otpSentTo = "";

                                if (adminUser.ProviderUserKey != null)
                                {
                                    membershipId = adminUser.ProviderUserKey.ToString();

                                    var adminProfile = new UserProfile(membershipId);

                                    var emailToSend = MACSecurity.Security.DecodeAndDecrypt(adminProfile.Contact.Email, membershipId);
                                    var phoneToSend = MACSecurity.Security.DecodeAndDecrypt(adminProfile.Contact.MobilePhone, membershipId);

                                    hiddenZ.Value = "OTP in progress";

                                    otpSentTo = sReply.Contains(sr.DeliveryMethod + "=Sms") ? phoneToSend : emailToSend;
                                }

                                // Redirect Login page
                                var pageAction = "";
                                pageAction += "Action=OtpLoginPending";
                                pageAction += "&UName=" + userLoginName;
                                pageAction += "&OtpSentTo=" + otpSentTo;
                                pageAction += "&DisplayMode=" + displayMode.Value;

                                var mReplyItems = sReply.Split(char.Parse(dk.ItemSep));
                                foreach (var mReplyItem in mReplyItems)
                                {
                                    if (mReplyItem.StartsWith(sr.OTP))
                                    {
                                        pageAction += "&OTP=" + mReplyItem.Replace(sr.OTP + "=", "");
                                    }
                                }

                                if (hiddenV.Value != "")
                                    pageAction += "&bid=" + hiddenV.Value;

                                hiddenAA.Value = pageAction;
                                hiddenAE.Value = pageAction;

                                myForm.Action = "/Admin/Security/Login.aspx?d=" + MACSecurity.Security.EncryptAndEncode(pageAction, membershipId) + membershipId.ToUpper();

                                ClientScript.RegisterStartupScript(typeof(Page), "RedirectLogin", "<script type='text/JavaScript'>redirectLogin();</script>");
                            }
                        }
                    }
                    else
                    {
                        var invalidUserMsg = "<span style='color: #ff0000; font-weight: normal;'>" + userLoginName + " not registered.<br />Please contact an administrator.</span>";

                        AdminLoginResult_Desktop.InnerHtml = invalidUserMsg;
                        AdminLoginResult_Mobile.InnerHtml = invalidUserMsg;
                    }
                }
            }
            catch (Exception ex)
            {
                var mDetails = ex.ToString().Replace(Constants.Dictionary.Keys.ItemSep, "!").Replace(Constants.Dictionary.Keys.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
            }
        }
    }
}