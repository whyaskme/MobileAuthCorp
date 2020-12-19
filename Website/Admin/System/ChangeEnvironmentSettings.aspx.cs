using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;

using MACBilling;
using MACSecurity;
using MACServices;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

using cfg = MACServices.Constants.WebConfig;

using cfghost = MACServices.Constants.WebConfig.HostInfo;

namespace Reset
{
    public partial class IPs : Page
    {
        Utils myUtils = new Utils();

        public UserProfile adminProfile;

        public string adminUserId = "";
        public string adminFirstName = "";
        public string adminLastName = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            // Deny access if user request not logged in
            var isAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
            if (!isAuthenticated)
            {
                Response.Write("Hello world!");
                Response.End();
            }

            if (IsPostBack) // Run the process
            {
                try
                {
                    Application["WebConfigSet"] = myUtils.SetWebConfig(dlEnvironments.SelectedValue);
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch(Exception ex)
                {
                    var errMsg = ex.ToString();
                }

                ClientScript.RegisterStartupScript(typeof(Page), "closePage", "<script type='text/JavaScript'>callParentDocumentFunction();</script>");
            }
            else // 
            {
                var webConfiguration = WebConfigurationManager.OpenWebConfiguration("~");

                var currentEnvironment = webConfiguration.AppSettings.Settings["ConfigName"].Value.ToString();

                //Response.Write(currentEnvironment);
                //Response.End();

                switch(currentEnvironment)
                {
                    case cfghost.Host.LocalhostGeneric:
                        dlEnvironments.SelectedIndex = 0;
                        break;

                    case cfghost.Host.LocalhostChris:
                        dlEnvironments.SelectedIndex = 1;
                        break;

                    case cfghost.Host.LocalhostJoe:
                        dlEnvironments.SelectedIndex = 2;
                        break;

                    case cfghost.Host.LocalhostTerry:
                        dlEnvironments.SelectedIndex = 3;
                        break;

                    case cfghost.Host.Test:
                        dlEnvironments.SelectedIndex = 4;
                        break;

                    case cfghost.Host.TestIntegration:
                        dlEnvironments.SelectedIndex = 5;
                        break;

                    case cfghost.Host.QA:
                        dlEnvironments.SelectedIndex = 6;
                        break;

                    case cfghost.Host.ProductionStaging:
                        dlEnvironments.SelectedIndex = 7;
                        break;

                    case cfghost.Host.Production:
                        dlEnvironments.SelectedIndex = 8;
                        break;

                    case cfghost.Host.Demo:
                        dlEnvironments.SelectedIndex = 9;
                        break;
                }
            }
        }
    }
}