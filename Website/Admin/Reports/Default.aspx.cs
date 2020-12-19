using System;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using MACServices;

namespace Admin.Reports
{
    public partial class Default : System.Web.UI.Page
    {
        HiddenField _hiddenW;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Master != null)
            {
                // ReSharper disable once UnusedVariable
                var oPanel = (Panel)Master.FindControl("pnlEventHistory");
                var bodyContainer = (Panel)Master.FindControl("BodyContainer");
                bodyContainer.Height = 1200;

                _hiddenW = (HiddenField)Master.FindControl("hiddenW");
                _hiddenW.Value = "5490bfc4ead63627d88e3e22";
            }

        }
    }
}