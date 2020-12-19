using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using MACServices;

public partial class Default : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Page.Master == null) return;
        var displayMode = (HiddenField)Page.Master.FindControl("hiddenU");

        //if (!Convert.ToBoolean(Application["WebConfigSet"]))
        //{
        //    // Set web config variables for environment
        //    Utils myUtils = new Utils();
        //    myUtils.SetWebConfig();
        //}

        //Response.Redirect("~/Admin/Default.aspx");
        //Response.End();

    }
}