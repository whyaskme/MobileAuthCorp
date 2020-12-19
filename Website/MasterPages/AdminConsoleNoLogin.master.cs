using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Web;
using System.Web.Configuration;

using MACServices;

using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using cfgcs = MACServices.Constants.WebConfig.ConnectionStringKeys;

namespace MasterPages
{
    public partial class AdminConsole : System.Web.UI.MasterPage
    {
        public bool Debug = Convert.ToBoolean(ConfigurationManager.AppSettings["Debug"]);
        public bool ShowTestMenu = Convert.ToBoolean(ConfigurationManager.AppSettings["ShowTestMenu"]);
        public string CurrentPage = HttpContext.Current.Request.ServerVariables["Url"].ToLower();
        public Stopwatch ProcessTimer;

        public Utils myUtils = new Utils();

        protected void Page_Init(object sender, EventArgs e)
        {
            if (hiddenB.Value == "")
                hiddenB.Value = Constants.Strings.DefaultGroupId;

            if (hiddenD.Value == "")
                hiddenD.Value = Constants.Strings.DefaultClientId;

            if (hiddenE.Value == "")
                hiddenE.Value = Constants.Strings.DefaultAdminId;
            divServiceResponseMessage.Text = Request["msg"] != null
                ? Request["msg"].ToString(CultureInfo.CurrentCulture)
                : "";

        }
    }
}
