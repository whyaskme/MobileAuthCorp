using System;
using System.Web.UI;
using MACServices;

namespace Admin.Tests.AWS
{
    public partial class AwsHealthCheck : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //var myIp = Request.ServerVariables["LOCAL_ADDR"];

            //spanServerIP.InnerHtml = myIp;

            //var healtcheckEvent = new Event();

            //var tokens = "";
            //tokens += Constants.TokenKeys.ServerIpAddress + myIp.Replace(":", "-"); // Do this because the : in ther ip is a constant separator and will not get logged!

            //healtcheckEvent.Create(Constants.EventLog.System.AwsHealthCheck, tokens);
        }
    }
}