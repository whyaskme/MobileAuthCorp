using System;

namespace MACUserApps.Web.Tests.TrxVerification
{
    public partial class MacUserAppsWebTestsTrxVerificationTxDone : System.Web.UI.Page
    {
        private static string Test = "TxDone";
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["CID"] = "";
            Session["RequestId"] = "";
            AddToLogAndDisplay("Done");
        }

        protected void btnDoItAgain_Click(object sender, EventArgs e)
        {
            Response.Redirect("TrxVerification.aspx");
        }

        private void AddToLogAndDisplay(string textToAdd)
        {
            var newlog = String.Format("{0}|{1}.{2}", Session["LogText"], Test, textToAdd);
            Session["LogText"] = newlog;
            tbLog.Text = newlog.Replace("|", Environment.NewLine);
        }

    }
}