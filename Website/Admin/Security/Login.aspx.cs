using System;
using System.Text;
using System.Web;
using System.Configuration;
using System.Web.Security;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using MACServices;
using MACSecurity;
using MongoDB.Bson;

using dk = MACServices.Constants.Dictionary.Keys;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;

namespace Admin.Security
{
    public partial class Login : System.Web.UI.Page
    {
        HiddenField hiddenU;
        HiddenField hiddenV;

        public string userRegisteredState = "Not registered";
        public string sentTo = "";
        public string otpSent = "";

        Utils mUtils = new Utils();

        protected void Page_Load(object sender, EventArgs e)
        {
            var requestType = "Password";
            var pageAction = "";

            if (Master != null)
                if (Page.Master != null)
                {
                    hiddenU = (HiddenField)Page.Master.FindControl("hiddenU");
                    hiddenV = (HiddenField)Page.Master.FindControl("hiddenV");
                }

            try
            {
                if(Request["d"] != null)
                {
                    // Encrypted request
                    var requestRawData = Request["d"];
                    var requestData = requestRawData.Substring(0, requestRawData.Length - 24);

                    // UserId (ObjectId = 24 chars)  is tacked onto the end of the requestring as uppercase. Convert tolower after substring
                    var requestUserId = requestRawData.Substring(requestRawData.Length - 24).ToLower();

                    var requestParams = MACSecurity.Security.DecodeAndDecrypt(requestData, requestUserId);

                    var parseParams = requestParams.Split('&');
                    foreach (var paramSet in parseParams)
                    {
                        var tmpVal = paramSet.Split('=');
                        var paramKey = tmpVal[0];
                        var paramValue = tmpVal[1];

                        switch (paramKey.ToLower())
                        {
                            case "uname":
                                hiddenAB.Value = MACSecurity.Security.EncryptAndEncode(paramValue.Trim(), Constants.Strings.DefaultClientId);
                                break;

                            case "displaymode":
                                hiddenU.Value = paramValue.Trim();
                                break;

                            case "bid":
                                hiddenV.Value = paramValue.Trim();
                                break;

                            case "action":
                                pageAction = paramValue.Trim();
                                break;

                            case "otp":
                                //otpSent = MACSecurity.Security.EncryptAndEncode(paramValue.Trim(), Constants.Strings.DefaultClientId);
                                hiddenAD.Value = MACSecurity.Security.EncryptAndEncode(paramValue.Trim(), Constants.Strings.DefaultClientId);
                                break;

                            case "otpsentto":
                                sentTo = paramValue.Trim();
                                sentTo = "*** - *** - **" + sentTo.Substring(sentTo.Length - 2);
                                break;
                        }
                    }
                }
                else if (Request["Action"] != null)
                {
                    // This is "Use existing credentials" login
                    hiddenAB.Value = Request["UName"].Trim();
                    hiddenU.Value = Request["DisplayMode"].Trim();
                    hiddenV.Value = Request["bid"].Trim();
                    pageAction = Request["Action"].Trim();
                }

                #region Update message controls

                // If Desktop is null, we are working in mobile environment
                var updateMessageContainer = (HtmlContainerControl)AdminLoginResult_Desktop;
                if (hiddenU.Value == Constants.Strings.DisplayMobile)
                    updateMessageContainer = AdminLoginResult_Mobile;

                #endregion

                #region Password/OTP controls
                
                // If Desktop is null, we are working in mobile environment
                var userPasswordField = txtPassword_Desktop;
                if (hiddenU.Value == Constants.Strings.DisplayMobile)
                    userPasswordField = txtPassword_Mobile;

                #endregion

                #region User Name controls

                // If Desktop is null, we are working in mobile environment
                var requestButton = btnAdminOtpRequest_Desktop;
                if (hiddenU.Value == Constants.Strings.DisplayMobile)
                    requestButton = btnAdminOtpRequest_Mobile;

                #endregion

                userPasswordField.Focus();

                switch (pageAction.ToLower())
                {
                    case "useexistingcredentials":
                        updateMessageContainer.InnerHtml = "<span style='font-weight: normal;'>Enter existing password for<br /><span style='color: #0362a6; font-weight: normal;'>" + hiddenAB.Value + "</span></span>";
                        requestButton.Value = "Submit";
                        break;

                    case "otploginpending":
                        updateMessageContainer.InnerHtml = "<span style='font-weight: normal;'>OTP sent to (<span style='color: #0362a6; font-weight: normal;'>" + sentTo + "</span>)</span>";
                        requestButton.Value = "Submit";
                        requestType = "OTP";
                        break;
                }

                if (!IsPostBack)
                {

                }
                else
                {
                    var userLoginName = "";// hiddenAB.Value.Trim();

                    switch (pageAction.ToLower())
                    {
                        case "useexistingcredentials":
                            userLoginName = hiddenAB.Value.Trim();
                            break;

                        case "otploginpending":
                            userLoginName = MACSecurity.Security.DecodeAndDecrypt(hiddenAB.Value.Trim(), Constants.Strings.DefaultClientId);
                            break;
                    }

                    var userPassword = userPasswordField.Value.Trim();

                    var loginEvent = new Event { _id = ObjectId.GenerateNewId() };

                    // Bypass until I can resolve MongoDB version issue
                    //Response.Redirect("/Admin/", true);
                    //Response.End();

                    var validUser = Membership.GetUser(userLoginName);

                    if (validUser != null)
                    {
                        userRegisteredState = "Registered";

                        if (Membership.ValidateUser(userLoginName, userPassword))
                        {
                            if (validUser.ProviderUserKey != null)
                            {
                                var userId = validUser.ProviderUserKey.ToString();
                                var userProfile = new UserProfile(userId);

                                // todo: try catch around this so we know if there is a mismatch on what is in constants and the database
                                var userRole = mUtils.GetRoleNameByRoleId(userProfile.Roles[0].ToString());

                                var sbUserData = new StringBuilder();

                                // Encrypted hidden fields
                                sbUserData.Append("hiddenE=" + MACSecurity.Security.EncryptAndEncode(userId, Constants.Strings.DefaultClientId) + "|");
                                sbUserData.Append("hiddenG=" + MACSecurity.Security.EncryptAndEncode(userLoginName, userId) + "|");
                                sbUserData.Append("hiddenF=" + MACSecurity.Security.EncryptAndEncode(userProfile.IsReadOnly.ToString(), userId) + "|");
                                sbUserData.Append("hiddenH=" + userProfile.FirstName + "|");
                                sbUserData.Append("hiddenI=" + userProfile.LastName + "|");
                                sbUserData.Append("hiddenJ=" + userProfile.Contact.Email + "|");
                                sbUserData.Append("hiddenK=" + userProfile.Contact.MobilePhone + "|");
                                sbUserData.Append("hiddenL=" + MACSecurity.Security.EncryptAndEncode(userRole, userId) + "|");
                                sbUserData.Append("hiddenD=" + MACSecurity.Security.EncryptAndEncode(Constants.Strings.DefaultClientId, userId));

                                const int timeoutMinutes = 10080; // One week in minutes

                                var cookieIssued = DateTime.UtcNow;
                                var cookieExpires = DateTime.UtcNow.AddMinutes(timeoutMinutes);

                                var ticket = new FormsAuthenticationTicket(
                                    1,
                                    userLoginName,
                                    cookieIssued,
                                    cookieExpires,
                                    true,
                                    sbUserData.ToString(), // Un-encrypted hidden fields
                                    FormsAuthentication.FormsCookiePath);

                                // Encrypt the ticket.
                                var encTicket = FormsAuthentication.Encrypt(ticket);

                                // Create the cookie.
                                var sessionCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);

                                // Set the cookie domain to match the <httpCookies httpOnlyCookies="true" domain="polaris.mobileauthcorp.net" /> in web.config
                                // If you don't do this, you will not be able to authenticate and lock yourself out!
                                var currentHost = HttpContext.Current.Request.ServerVariables["SERVER_NAME"].ToString();
                                sessionCookie.Domain = currentHost;
                                sessionCookie.Expires = cookieExpires;
                                sessionCookie.Shareable = false;
                                sessionCookie.HttpOnly = true;
                                //sessionCookie.Secure = true;

                                Response.Cookies.Add(sessionCookie);

                                // Log the login event
                                loginEvent.UserId = ObjectId.Parse(userId);
                                loginEvent.ClientId = ObjectId.Parse(Constants.Strings.DefaultClientId);

                                var tokens = "";
                                tokens += Constants.TokenKeys.UserRole + userRole;
                                tokens += Constants.TokenKeys.UserFullName + MACSecurity.Security.DecodeAndDecrypt(userProfile.FirstName, userId) + " " + MACSecurity.Security.DecodeAndDecrypt(userProfile.LastName, userId);

                                loginEvent.Create(Constants.EventLog.Security.Login.Succeeded, tokens);

                                var redirectUrl = "/Admin";
                                switch (userRole)
                                {
                                    case Constants.Roles.SystemAdministrator:
                                        redirectUrl += "/";
                                        break;

                                    case Constants.Roles.GroupAdministrator:
                                        redirectUrl += "/Groups/";
                                        break;

                                    case Constants.Roles.ClientAdministrator:
                                        redirectUrl += "/Clients/";
                                        break;

                                    case Constants.Roles.AccountingUser:
                                        redirectUrl += "/Billing/";
                                        break;

                                    case Constants.Roles.ClientUser:
                                        redirectUrl += "/Billing/";
                                        break;

                                    case Constants.Roles.GroupUser:
                                        redirectUrl += "/Billing/";
                                        break;

                                    case Constants.Roles.OperationsUser:
                                        redirectUrl += "/";
                                        break;

                                    case Constants.Roles.ViewOnlyUser:
                                        redirectUrl += "/Users/MyAccount/";
                                        break;

                                    default:
                                        redirectUrl += "/Users/MyAccount/";
                                        break;
                                }

                                if (hiddenV.Value != "")
                                    redirectUrl = "/Admin/Billing/Client/Default.aspx?bid=" + hiddenV.Value + "&pa=ViewBillDetails";

                                Response.Redirect(redirectUrl, true);
                                Response.End();
                            }
                        }
                    }


                    // If we reach here, the user's credentials were invalid 
                    switch(userRegisteredState)
                    {
                        case "Not registered":
                            updateMessageContainer.InnerHtml = "<span style='color: #ff0000; font-weight: normal;'>" + userLoginName + " not registered.<br />Please contact an administrator.</span>";
                            break;

                        case "Registered":
                            updateMessageContainer.InnerHtml = "<span style='color: #ff0000; font-weight: bold;'>Invalid " + requestType.Replace("Password", "OTP") + ".<br />Please try again.</span>";
                            break;
                    }

                    loginEvent.Create(Constants.EventLog.Security.Login.Failed, Constants.TokenKeys.UserName + userLoginName + Constants.TokenKeys.Password + userPassword);

                    hiddenZ.Value = "";
                    hiddenAA.Value = "";

                    txtPassword_Desktop.Attributes.Add("style", "border-color: #ff0000;");
                    txtPassword_Desktop.Focus();

                    txtPassword_Mobile.Attributes.Add("style", "border-color: #ff0000;");
                }
            }
            catch (Exception ex)
            {
                // Do not log this since it is generated as "Thread aborted" as a result of response.redirect

                if (ex.Message.ToLower() !=
                    "unable to evaluate expression because the code is optimized or a native frame is on top of the call stack.")
                {
                    if (ex.Message.StartsWith("Thread") == false)
                    {
                        var mDetails = ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") +
                                       Constants.TokenKeys.ClientName + "NA";
                        var exceptionEvent = new Event
                        {
                            EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                        };
                        exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                        AdminLoginResult_Desktop.InnerHtml = ex.Message;
                    }
                }
            }
        }
    }
}