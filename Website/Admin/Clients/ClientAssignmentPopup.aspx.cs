using System;
using System.Web;

namespace MACAdmin.Clients
{
    public partial class ClientAssignmentPopup : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Deny access if user request not logged in
            var isAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
            if (!isAuthenticated)
            {
                Response.Write("Hello world!");
                Response.End();
            }
        }
    }
}