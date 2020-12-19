using System;
using System.Text;

namespace UserControls
{
    public partial class NavBreadCrumb : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var tmpUrl = "";
            var currentUrl = Request.ServerVariables["URL"];
            var sbNavBreadCrumb = new StringBuilder();

            if (currentUrl.ToLower() != "/admin/default.aspx" && currentUrl.ToLower() != "/admin/security/default.aspx" && currentUrl.ToLower() != "/admin/security/login.aspx")
            {
                var urlArray = currentUrl.Split('/');
                for (var i = 0; i < urlArray.Length - 1; i++)
                {
                    if (urlArray[i] != "")
                    {
                        tmpUrl += "/" + urlArray[i];

                        if (i < urlArray.Length - 2)
                        {
                            sbNavBreadCrumb.Append("<a href='" + tmpUrl + "'>" + urlArray[i].Replace("Admin", "Home") + "</a>");
                            sbNavBreadCrumb.Append("<span style='margin:5px;color: #c1c1c1;position: relative; top: -2px;padding:0 0.5em;'>&raquo;</span>");
                        }
                        else
                            sbNavBreadCrumb.Append("<span style='font-size: 13px;color: #808080;'>" + urlArray[i] + "</span>");
                    }
                }

                sbNavBreadCrumb.Append("<span id='spanTabName' runat='server'></span>");

                divNavBreadCrumb.InnerHtml = sbNavBreadCrumb.ToString();
            }
            else
                divNavBreadCrumb.InnerHtml = "";
        }
    }
}