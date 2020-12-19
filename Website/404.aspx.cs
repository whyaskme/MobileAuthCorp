using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using cnt = MACBilling.BillConstants;

public partial class Default : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Page.Master == null) return;
        var displayMode = (HiddenField)Page.Master.FindControl("hiddenU");

        if(!IsPostBack)
        {
            // We need to rethink this. This is vulnerable to injection attacks.
            //if(Request["errmsg"] != null)
            //{
            //    div404Details_Desktop.InnerHtml = "<h1>Oooops, something went wrong!</h1>" + Request["errmsg"];
            //    div404Details_Mobile.InnerHtml = "<h1>Oooops, something went wrong!</h1>" + Request["errmsg"];
            //}
        }
        else
        {

        }
    }
}