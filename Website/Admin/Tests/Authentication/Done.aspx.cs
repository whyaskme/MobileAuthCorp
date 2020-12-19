using System;

namespace MACUserApps.Web.Tests.Authentication
{
    public partial class MacUserAppsWebTestsAuthenticationDone : System.Web.UI.Page
    {
        private static string Test = "Done";
        protected void Page_Load(object sender, EventArgs e)
        {
            AddToLogAndDisplay("Done");
        }

        protected void btnDoItAgain_Click(object sender, EventArgs e)
        {
            Response.Redirect("Auth.aspx");
        }

        private void AddToLogAndDisplay(string textToAdd)
        {
            var newlog = String.Format("{0}|{1}.{2}", Session["LogText"], Test, textToAdd);
            Session["LogText"] = newlog;
            tbLog.Text = newlog.Replace("|", Environment.NewLine);
        }

    }
}