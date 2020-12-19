using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;

using MACServices;

using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using cfgcs = MACServices.Constants.WebConfig.ConnectionStringKeys;
using cs = MACServices.Constants.Strings;

namespace MasterPages
{
    public partial class AdminConsoleBlank : System.Web.UI.MasterPage
    {
        public bool Debug = Convert.ToBoolean(ConfigurationManager.AppSettings["Debug"]);
        public bool ShowTestMenu = Convert.ToBoolean(ConfigurationManager.AppSettings["ShowTestMenu"]);
        public string CurrentPage = HttpContext.Current.Request.ServerVariables["Url"].ToLower();
        public Stopwatch ProcessTimer = new Stopwatch();

        public Utils myUtils = new Utils();

        protected void Page_Init(object sender, EventArgs e)
        {
            ProcessTimer.Start();

            if (hiddenB.Value == "")
                hiddenB.Value = cs.DefaultGroupId;

            if (hiddenD.Value == "")
                hiddenD.Value = cs.DefaultClientId;

            if (hiddenE.Value == "")
                hiddenE.Value = cs.DefaultAdminId;

            if (Request["debug"] != null)
                hiddenA.Value = Request["debug"].ToLower();

            if (Request["bid"] != null)
                hiddenV.Value = Request["bid"].ToLower();

            if (HttpContext.Current.Request.ServerVariables["SERVER_NAME"] == "localhost")
                hiddenA.Value = "true";

            divServiceResponseMessage.Text = Request["msg"] != null ? Request["msg"].ToString(CultureInfo.CurrentCulture) : "";

            var isAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
            if (!isAuthenticated)
            {
                Session.Abandon();

                if (CurrentPage != "/admin/security/default.aspx" && CurrentPage != "/admin/security/login.aspx")
                {
                    Response.Redirect("~/Admin/Security/Default.aspx");
                    Response.End();
                }
            }
            else
            {
                SetHiddenFields();
            }

            //GetAppLastModifiedDateVersion();
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

        public void GetAppLastModifiedDateVersion()
        {
            //var assemblyPath = HttpContext.Current.Server.MapPath("\\Bin\\MACServices.dll");

            //var appYear = DateTime.UtcNow.Year;

            //DateTime dt = System.IO.File.GetLastWriteTime(assemblyPath);
            //var lastModifiedDate = dt.ToString();

            //StringBuilder sbFooterInfo = new StringBuilder();

            //sbFooterInfo.Append("<table style='width: 100%; position: relative; top: 25px;'>");
            //sbFooterInfo.Append("   <tr>");
            //sbFooterInfo.Append("       <td style='white-space: nowrap; border: none; border-top: solid 1px #c0c0c0; padding-top: 25px; color: #808080;'>");
            //sbFooterInfo.Append("           Last modified - " + lastModifiedDate);
            //sbFooterInfo.Append("       </td>");
            //sbFooterInfo.Append("       <td style='width: 100%; border: none; border-top: solid 1px #c0c0c0; padding-top: 25px; color: #808080;'>");
            //sbFooterInfo.Append("           &nbsp;");
            //sbFooterInfo.Append("       </td>");
            //sbFooterInfo.Append("       <td style='white-space: nowrap; border: none; border-top: solid 1px #c0c0c0; padding-top: 25px; color: #808080;'>");
            //sbFooterInfo.Append("           &copy; 2013-" + appYear + " Mobile Authentication Corporation. All rights reserved.");
            //sbFooterInfo.Append("       </td>");
            //sbFooterInfo.Append("   </tr>");
            //sbFooterInfo.Append("</table>");

            //var footerInfo = "&copy; 2013-" + appYear + " Mobile Authentication Corporation. All rights reserved. Last modified - " + lastModifiedDate + "";

            //divFooterCopyright_Desktop.InnerHtml = sbFooterInfo.ToString();
            //divFooterCopyright_Mobile.InnerHtml = sbFooterInfo.ToString();
        }
    }
}
