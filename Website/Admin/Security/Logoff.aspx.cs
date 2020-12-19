using System;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;

using MongoDB.Bson;

using MACServices;

public partial class Logoff : System.Web.UI.Page
{
    public string loggedInAdminUserName = "";
    public string loggedInAdminFirstName = "";
    public string loggedInAdminLastName = "";
    public string loggedInAdminId = "";
    public string loggedInClientId = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Page.Master != null)
        {
            var hiddenE = (HiddenField)Page.Master.FindControl("hiddenE");
            var hiddenG = (HiddenField)Page.Master.FindControl("hiddenG");
            var hiddenH = (HiddenField)Page.Master.FindControl("hiddenH");
            var hiddenI = (HiddenField)Page.Master.FindControl("hiddenI");
            var hiddenJ = (HiddenField)Page.Master.FindControl("hiddenJ");
            var hiddenK = (HiddenField)Page.Master.FindControl("hiddenK");
            var hiddenL = (HiddenField)Page.Master.FindControl("hiddenL");
            var hiddenD = (HiddenField)Page.Master.FindControl("hiddenD");

            var redirectMsg = "";
            if(Request["msg"] != null)
                if (Request["msg"] != "")
                    redirectMsg = "?msg=" + Request["msg"];

            loggedInAdminId = MACSecurity.Security.DecodeAndDecrypt(hiddenE.Value, Constants.Strings.DefaultClientId);
            loggedInAdminUserName = MACSecurity.Security.DecodeAndDecrypt(hiddenG.Value, loggedInAdminId);

            loggedInAdminFirstName = MACSecurity.Security.DecodeAndDecrypt(hiddenH.Value, loggedInAdminId);
            loggedInAdminLastName = MACSecurity.Security.DecodeAndDecrypt(hiddenI.Value, loggedInAdminId);

            loggedInClientId = MACSecurity.Security.DecodeAndDecrypt(hiddenD.Value, loggedInAdminId);

            var logoffEvent = new Event();

            if (string.IsNullOrEmpty(loggedInAdminId))
                loggedInAdminId = "true";

            var tokens = "";

            if (loggedInAdminId != "")
            {
                logoffEvent.UserId = ObjectId.Parse(loggedInAdminId);

                if (loggedInAdminId != "")
                    logoffEvent.ClientId = ObjectId.Parse(loggedInClientId);

                tokens += Constants.TokenKeys.UserRole + hiddenL.Value.Replace("User ", "");
                tokens += Constants.TokenKeys.UserFullName + loggedInAdminFirstName + " " + loggedInAdminLastName;

                logoffEvent.Create(Constants.EventLog.Security.Logout.Succeeded, tokens);
            }
            else
            {
                logoffEvent.Create(Constants.EventLog.Security.Logout.Succeeded, tokens);
            }

            FormsAuthentication.SignOut();
            Session.Abandon();

            // Read the user values from the authentication cookie and validate
            var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];

            // clear MAC_R1 cookie
            var cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "") { Expires = DateTime.UtcNow.AddYears(-1) };
            Response.Cookies.Add(cookie1);

            // clear session cookie (not necessary for your current problem but i would recommend you do it anyway)
            var cookie2 = new HttpCookie("ASP.NET_SessionId", "") { Expires = DateTime.UtcNow.AddYears(-1) };
            Response.Cookies.Add(cookie2);

            hiddenE.Value = "";
            hiddenG.Value = "";
            hiddenH.Value = "";
            hiddenI.Value = "";
            hiddenJ.Value = "";
            hiddenK.Value = "";
            hiddenL.Value = "";

            var redirectUrl = "~/" + redirectMsg;

            if (Request.ServerVariables["HTTP_REFERER"].ToLower().Contains("/admin/"))
                redirectUrl = "/Admin/Security/" + redirectMsg;

            Response.Redirect(redirectUrl);
            Response.End();
        }
        Response.End();
    }
}