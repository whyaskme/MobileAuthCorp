using System;
using System.Globalization;
using System.Text;
using System.Web;

using MACServices;

namespace MACAdmin.Clients
{
    public partial class IPAddressAssignmentPopup : System.Web.UI.Page
    {
        Utils myUtils = new Utils();

        Client myClient;

        string LoggedInAdminId = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            // Deny access if user request not logged in
            var isAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
            if (!isAuthenticated)
            {
                Response.Write("Hello world!");
                Response.End();
            }

            var sbResponse = new StringBuilder();

            if (Request["clientId"] != null)
            {
                var clientId = Request["clientId"].ToString(CultureInfo.CurrentCulture);
                myClient = new Client(clientId);
            }

            if (Request["userid"] != null)
                if (Request["userid"] != "")
                    LoggedInAdminId = Request["userid"];

            LoggedInAdminId = MACSecurity.Security.DecodeAndDecrypt(LoggedInAdminId.ToString(), Constants.Strings.DefaultClientId);

            var userIsReadOnly = "";
            if (!String.IsNullOrEmpty(Request["userisreadonly"]))
                userIsReadOnly = Request["userisreadonly"];

            userIsReadOnly = MACSecurity.Security.DecodeAndDecrypt(userIsReadOnly, LoggedInAdminId.ToString());

            if(IsPostBack)
            {
                myClient.AllowedIpList = txtIPAddresses.Text;
                myClient.Update();

                divUpdateMsg.Visible = true;
            }
            else
            {
                txtIPAddresses.Text = myClient.AllowedIpList;

                divUpdateMsg.Visible = false;
            }

            if (Convert.ToBoolean(userIsReadOnly))
            {
                txtIPAddresses.Enabled = false;
                btnUpdate.Visible = false;
                btnCancel.Visible = false;
            }
        }
    }
}