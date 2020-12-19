using System;
using System.Web;

public partial class CreateTypeDefs : System.Web.UI.Page
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

        if(!IsPostBack)
        {
            var mUtils = new MACServices.Utils();
            mUtils.CreateTypeDefinitions();
        }

    }
}