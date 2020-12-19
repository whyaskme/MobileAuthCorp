using System;
using System.Globalization;
using System.Text;
using System.Web;

using MACServices;

namespace MACAdmin.Clients
{
    public partial class GroupAssignmentPopup : System.Web.UI.Page
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

            var clientCount = 0;
            var sbResponse = new StringBuilder();

            if (Request["gid"] != null)
            {
                var groupId = Request["gid"].ToString(CultureInfo.CurrentCulture);
                var myGroup = new Group(groupId);

                spanGroupName.InnerHtml = myGroup.Name;

                foreach (var currentClientRelationship in myGroup.Relationships)
                {
                    if (currentClientRelationship.MemberType == "Client")
                    {
                        clientCount++;

                        // Lookup the current clients info
                        var currentClient = new Client(currentClientRelationship.MemberId.ToString());

                        sbResponse.Append("<a href='javascript: navigateToClient(&apos;" + currentClientRelationship.MemberId + "&apos;);'>");
                        sbResponse.Append("<div id='divClient_" + currentClientRelationship.MemberId + "' class='button radius tiny' style='width: 100%;'>");
                        sbResponse.Append(currentClient.Name);
                        sbResponse.Append("</div>");
                        sbResponse.Append("</a><br />");
                    }
                }
                spanClientCount.InnerHtml = clientCount.ToString(CultureInfo.CurrentCulture);
                divGroupClientsContainer.InnerHtml = sbResponse.ToString();
            }
        }
    }
}