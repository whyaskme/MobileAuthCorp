using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.Security;

using MACServices;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using cfgcs = MACServices.Constants.WebConfig.ConnectionStringKeys;
using cs = MACServices.Constants.Strings;

namespace MasterPages
{
    public partial class AdminConsole : System.Web.UI.MasterPage
    {
        public bool Debug = Convert.ToBoolean(ConfigurationManager.AppSettings[cfg.Debug]);
        public bool ShowTestMenu = Convert.ToBoolean(ConfigurationManager.AppSettings["ShowTestMenu"]);
        public string CurrentPage = HttpContext.Current.Request.ServerVariables["Url"].ToLower();

        public Utils myUtils = new Utils();

        public Stopwatch ProcessTimer = new Stopwatch();

        protected void Page_Init(object sender, EventArgs e)
        {
            divReadOnlyStatus.Visible = false;

            ProcessTimer.Start();

            if (hiddenB.Value == "")
                hiddenB.Value = cs.DefaultGroupId;

            if (hiddenD.Value == "")
                hiddenD.Value = cs.DefaultClientId;

            if (hiddenE.Value == "")
                hiddenE.Value = cs.DefaultAdminId;

            DebugInfo.Visible = false;

            if (Request["debug"] != null)
                hiddenA.Value = Request["debug"].ToLower();

            if (Request["bid"] != null)
                hiddenV.Value = Request["bid"].ToLower();

            if (HttpContext.Current.Request.ServerVariables["SERVER_NAME"] == "localhost")
                hiddenA.Value = "true";

            if (Convert.ToBoolean(hiddenA.Value))
                ShowDebugInfo();

            divServiceResponseMessage.Text = Request["msg"] != null ? Request["msg"].ToString(CultureInfo.CurrentCulture) : "";

            switch (CurrentPage)
            {
                case "/admin/clients/default.aspx":
                    menuClients.Attributes.Add("class", "active");
                    break;
                case "/admin/billing/default.aspx":
                    menuBilling.Attributes.Add("class", "active");
                    break;
                case "/admin/billing/client/default.aspx":
                    menuBilling.Attributes.Add("class", "active");
                    break;
                case "/admin/billing/group/default.aspx":
                    menuBilling.Attributes.Add("class", "active");
                    break;
                case "/admin/groups/default.aspx":
                    menuGroups.Attributes.Add("class", "active");
                    break;
                case "/admin/help/default.aspx":
                    menuHelp.Attributes.Add("class", "active");
                    break;
                case "/admin/users/myaccount/default.aspx":
                    lnkMyAccount.Attributes.Add("class", "active");
                    break;
                case "/admin/reports/default.aspx":
                    menuReports.Attributes.Add("class", "active");
                    break;
                case "/admin/reports/events/eventsdefault.aspx":
                    menuReports.Attributes.Add("class", "active");
                    break;
                case "/admin/reports/operations/opsdefault.aspx":
                    menuReports.Attributes.Add("class", "active");
                    break;
                case "/admin/reports/clients/default.aspx":
                    menuReports.Attributes.Add("class", "active");
                    break;
                case "/admin/users/default.aspx":
                    menuUsers.Attributes.Add("class", "active");
                    break;
            }

            var isAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
            if (!isAuthenticated)
            {
                divLoginMyAccountControlsContainer_Desktop.Visible = false;
                divLoginMyAccountControlsContainer_Mobile.Visible = false;

                divMenuBar.Visible = false;

                lnkMyAccount.Visible = false;
                lnkLogoff.Visible = false;

                menuGroups.Visible = false;
                menuClients.Visible = false;
                menuUsers.Visible = false;
                menuReports.Visible = false;
                menuTests.Visible = false;
                menuHelp.Visible = false;

                Session.Abandon();

                if (CurrentPage != "/admin/security/default.aspx" && CurrentPage != "/admin/security/login.aspx" && CurrentPage != "/404.aspx")
                {
                    Response.Redirect("~/Admin/Security/Default.aspx");
                    Response.End();
                }
            }
            else
            {
                SetHiddenFields();

                SiteLogo.PostBackUrl = "/Admin/";

                var decryptedUserId = MACSecurity.Security.DecodeAndDecrypt(hiddenE.Value, Constants.Strings.DefaultClientId);
                var decryptedUserName = MACSecurity.Security.DecodeAndDecrypt(hiddenG.Value, decryptedUserId);

                // Check to see if the user is still enabled and valid. This is to terminate any disabled user mid-session
                var adminUser = Membership.GetUser(decryptedUserName);
                if (adminUser != null)
                {
                    UserProfile myProfile = new UserProfile(decryptedUserId);

                    divMemberRole.InnerHtml = MACSecurity.Security.DecodeAndDecrypt(hiddenL.Value, decryptedUserId);

                    var userFirstName = MACSecurity.Security.DecodeAndDecrypt(myProfile.FirstName, decryptedUserId);
                    var userLastName = MACSecurity.Security.DecodeAndDecrypt(myProfile.LastName, decryptedUserId);

                    divMemberName.InnerHtml = userFirstName + " " + userLastName;

                    //hiddenF.Value = myProfile.IsReadOnly.ToString();

                    if (Convert.ToBoolean(MACSecurity.Security.DecodeAndDecrypt(hiddenF.Value, decryptedUserId)))
                        divReadOnlyStatus.Visible = true;
                    else
                        divReadOnlyStatus.Visible = false;

                    if (!adminUser.IsApproved)
                    {
                        // Redirect and log user out
                        Response.Redirect("~/Admin/Security/Logoff.aspx");
                    }
                    else
                    {
                        divLoginMyAccountControlsContainer_Desktop.Visible = true;
                        divLoginMyAccountControlsContainer_Mobile.Visible = true;

                        divMenuBar.Visible = true;

                        lnkMyAccount.Visible = true;
                        lnkLogoff.Visible = true;

                        if (ShowTestMenu)
                            menuTests.Visible = true;

                        switch (MACSecurity.Security.DecodeAndDecrypt(hiddenL.Value, decryptedUserId))
                        {
                            case Constants.Roles.SystemAdministrator:
                                menuGroups.Visible = true;
                                menuClients.Visible = true;
                                menuUsers.Visible = true;
                                menuReports.Visible = true;
                                menuHelp.Visible = true;
                                menuTests.Visible = true;
                                menuBilling.Visible = true;
                                DebugInfo.Visible = true;
                                ShowDebugInfo();
                                break;

                            case Constants.Roles.GroupAdministrator:
                                menuGroups.Visible = true;
                                menuClients.Visible = true;
                                menuUsers.Visible = false;
                                menuReports.Visible = true;
                                menuHelp.Visible = true;
                                menuTests.Visible = false;
                                menuBilling.Visible = true;
                                DebugInfo.Visible = false;
                                break;

                            case Constants.Roles.ClientAdministrator:
                                menuGroups.Visible = false;
                                menuClients.Visible = true;
                                menuUsers.Visible = false;
                                menuReports.Visible = true;
                                menuHelp.Visible = true;
                                menuTests.Visible = false;
                                menuBilling.Visible = true;
                                DebugInfo.Visible = false;
                                break;

                            case Constants.Roles.AccountingUser:
                                menuGroups.Visible = false;
                                menuClients.Visible = false;
                                menuUsers.Visible = false;
                                menuReports.Visible = false;
                                menuHelp.Visible = true;
                                menuTests.Visible = false;
                                menuBilling.Visible = true;
                                DebugInfo.Visible = false;
                                // Modify the logo link for these users
                                SiteLogo.PostBackUrl += "Billing/";
                                break;

                            case Constants.Roles.ClientUser:
                                menuGroups.Visible = false;
                                menuClients.Visible = false;
                                menuUsers.Visible = false;
                                menuReports.Visible = false;
                                menuHelp.Visible = true;
                                menuTests.Visible = false;
                                menuBilling.Visible = true;
                                DebugInfo.Visible = false;
                                // Modify the logo link for these users
                                SiteLogo.PostBackUrl += "Billing/";
                                break;

                            case Constants.Roles.GroupUser:
                                menuGroups.Visible = false;
                                menuClients.Visible = false;
                                menuUsers.Visible = false;
                                menuReports.Visible = false;
                                menuHelp.Visible = true;
                                menuTests.Visible = false;
                                menuBilling.Visible = true;
                                DebugInfo.Visible = false;
                                // Modify the logo link for these users
                                SiteLogo.PostBackUrl += "Billing/";
                                break;

                            case Constants.Roles.OperationsUser:
                                menuGroups.Visible = false;
                                menuClients.Visible = false;
                                menuUsers.Visible = false;
                                menuReports.Visible = false;
                                menuHelp.Visible = false;
                                menuTests.Visible = false;
                                menuBilling.Visible = false;
                                DebugInfo.Visible = false;
                                spanMyAccountContainer1.Visible = false;
                                spanMyAccountContainer2.Visible = false;
                                // Modify the logo link for these users
                                SiteLogo.PostBackUrl += "";
                                break;
                                
                            case Constants.Roles.ViewOnlyUser:
                                menuGroups.Visible = true;
                                menuClients.Visible = true;
                                menuUsers.Visible = true;
                                menuReports.Visible = true;
                                menuHelp.Visible = true;
                                menuTests.Visible = true;
                                menuBilling.Visible = true;
                                DebugInfo.Visible = false;
                                // Modify the logo link for these users
                                SiteLogo.PostBackUrl += "";
                                break;

                            default:
                                menuGroups.Visible = false;
                                menuClients.Visible = false;
                                menuUsers.Visible = false;
                                menuReports.Visible = false;
                                menuHelp.Visible = false;
                                menuTests.Visible = false;
                                menuBilling.Visible = true;
                                DebugInfo.Visible = false;
                                break;
                        }
                    }
                    //var totalProcessTime = ProcessTimer.Elapsed;
                    GetAppLastModifiedDateVersion();

                    if (Convert.ToBoolean(MACSecurity.Security.DecodeAndDecrypt(hiddenF.Value, decryptedUserId)))
                    {
                        menuTests.Visible = false;
                    }
                }
            }

            var environmentLabel = myUtils.SetEnvironmentLabel();

            divRunningEnvironment_Desktop.InnerHtml = environmentLabel + " Environment";
            divRunningEnvironment_Mobile.InnerHtml = environmentLabel + " Environment";
        }

        public void SetHiddenFields()
        {
            // Read the user values from the authentication cookie and validate
            var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie == null)
            {
                var logoutEvent = new Event();
                logoutEvent.Create();

                FormsAuthentication.SignOut();
                Session.Abandon();

                // clear authentication cookie
                var cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "")
                {
                    Expires = DateTime.UtcNow.AddYears(-1)
                };
                Response.Cookies.Add(cookie1);

                // clear session cookie (not necessary for your current problem but i would recommend you do it anyway)
                var cookie2 = new HttpCookie("ASP.NET_SessionId", "") { Expires = DateTime.UtcNow.AddYears(-1) };
                Response.Cookies.Add(cookie2);

                hiddenF.Value = "false";
                hiddenD.Value = "";
                hiddenE.Value = "";
                hiddenG.Value = "";
                hiddenH.Value = "";
                hiddenI.Value = "";
                hiddenJ.Value = "";
                hiddenK.Value = "";
                hiddenL.Value = "";
                hiddenV.Value = "";

                Response.Redirect("~/Default.aspx");
                Response.End();
            }
            else
            {
                var ticket = FormsAuthentication.Decrypt(authCookie.Value);

                if (ticket == null) return;
                var userData = ticket.UserData.Split('|');
                foreach (var userField in userData)
                {
                    var userFieldData = userField.Split('=');
                    var fieldName = userFieldData[0];
                    var fieldValue = userFieldData[1];

                    switch (fieldName)
                    {
                        case "hiddenV":
                            hiddenV.Value = fieldValue;
                            break;
                        case "hiddenD":
                            hiddenD.Value = fieldValue;
                            break;
                        case "hiddenE":
                            hiddenE.Value = fieldValue;
                            break;
                        case "hiddenF":
                            hiddenF.Value = fieldValue;
                            break;
                        case "hiddenG":
                            hiddenG.Value = fieldValue;
                            break;
                        case "hiddenH":
                            hiddenH.Value = fieldValue;
                            break;
                        case "hiddenI":
                            hiddenI.Value = fieldValue;
                            break;
                        case "hiddenJ":
                            hiddenJ.Value = fieldValue;
                            break;
                        case "hiddenK":
                            hiddenK.Value = fieldValue;
                            break;
                        case "hiddenL":
                            hiddenL.Value = fieldValue;
                            break;
                    }
                }

                hiddenM.Value = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
        }

        public void ShowDebugInfo()
        {
            string serverName;
            string serverPort;

            DebugInfo.Visible = true;
            var mDbConnectionString = ConfigurationManager.ConnectionStrings[cfgcs.MongoServer].ConnectionString;
            if (mDbConnectionString.Contains("@"))
            {
                var arrDbConnStr = mDbConnectionString.Split('@');
                var arrDbServer = arrDbConnStr[1].Split(':');
                var arrDbPortAndName = arrDbServer[1].Split('/');

                serverName = arrDbServer[0];
                serverPort = arrDbPortAndName[0];
            }
            else
            {
                var arrDbConnStr = mDbConnectionString.Replace("mongodb://", "").Split('/');
                var arrDbServer = arrDbConnStr[0].Split(':');

                serverName = arrDbServer[0];
                serverPort = arrDbServer[1];
            }

            var sbConnectionInfo = new StringBuilder();

            sbConnectionInfo.Append("Svr: ");
            sbConnectionInfo.Append(HttpContext.Current.Request.ServerVariables["LOCAL_ADDR"]);
            sbConnectionInfo.Append(",&nbsp;&nbsp;DB&nbsp;&raquo;&nbsp;");
            sbConnectionInfo.Append(serverName + ":");
            sbConnectionInfo.Append(serverPort);
            sbConnectionInfo.Append(" (<span style='font-size: 0.75rem;color: #f00;'>" + 
                ConfigurationManager.AppSettings[cfg.MongoDbName] + "</span>)");

            var lbt = ConfigurationManager.AppSettings[cfg.LoopBackTest];
            if (Debug)
            {
                sbConnectionInfo.Append("<br />(Debug)");

                // show loopback config
                if (!String.IsNullOrEmpty(lbt))
                {
                    sbConnectionInfo.Append(" (" + cfg.LoopBackTest + "=");
                    sbConnectionInfo.Append(lbt);
                    sbConnectionInfo.Append(")");
                }
            }
            else // not debug
            {
                // show loopback config if not disabled
                if (!String.IsNullOrEmpty(lbt))
                {
                    if (lbt != cfg.Disabled)
                    {
                        sbConnectionInfo.Append(" (" + cfg.LoopBackTest + "=");
                        sbConnectionInfo.Append(lbt);
                        sbConnectionInfo.Append(")");
                    }
                }
            }
            ServerInfo.InnerHtml = sbConnectionInfo.ToString();
            PerformanceInfo.InnerHtml = "Page time: " + ProcessTimer.ElapsedMilliseconds + "ms.";
        }

        public void GetAppLastModifiedDateVersion()
        {
            var assemblyPath = HttpContext.Current.Server.MapPath("\\Bin\\MACServices.dll");

            var appYear = DateTime.UtcNow.Year;

            var dt = System.IO.File.GetLastWriteTime(assemblyPath);
            var lastModifiedDate = dt.ToString();

            var sbFooterInfo = new StringBuilder();
            var sbFooterInfoMobile = new StringBuilder();

            sbFooterInfo.Append("<div class='large-6 medium-6 small-12 columns copyright' style='margin-bottom:1rem;'>");
            sbFooterInfo.Append("&copy; 2013-" + appYear + " Mobile Authentication Corporation. All rights reserved.");            
            sbFooterInfo.Append("</div>");
            sbFooterInfo.Append("<div class='large-6 medium-6 small-12 columns copyright' style='margin-bottom:1rem;text-align:right !important;'>");
            sbFooterInfo.Append("Last modified - " + lastModifiedDate);
            sbFooterInfo.Append("</div>");

            sbFooterInfoMobile.Append("<div class='large-6 medium-6 small-12 columns copyright' style='margin:1rem 0 0;'>");
            sbFooterInfoMobile.Append("Last modified - " + lastModifiedDate);
            sbFooterInfoMobile.Append("</div>");
            sbFooterInfoMobile.Append("<div class='large-6 medium-6 small-12 columns copyright' style='margin:0.5rem 0;'>");
            sbFooterInfoMobile.Append("&copy; 2013-" + appYear + "<br />Mobile Authentication Corporation.<br />All rights reserved.");
            sbFooterInfoMobile.Append("</div>");

            divFooterCopyright_Desktop.InnerHtml = sbFooterInfo.ToString();
            divFooterCopyright_Mobile.InnerHtml = sbFooterInfoMobile.ToString();
        }
    }
}
