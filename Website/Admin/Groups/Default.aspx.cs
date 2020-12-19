using System;
using System.Web;
using System.Web.UI.WebControls;

namespace Admin.Groups
{
    public partial class Default : System.Web.UI.Page
    {
        HiddenField _hiddenW;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Deny access if user request not logged in
            var isAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
            if (!isAuthenticated)
            {
                Response.Write("Hello world!");
                Response.End();
            }

            if (Page.Master != null)
            {
                if (Master != null)
                {
                    _hiddenW = (HiddenField)Master.FindControl("hiddenW");
                    _hiddenW.Value = "5490bf7eead63627d88e3e1f";
                }
            }
        }
    }
}