using System;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

using MACServices;
using dk = MACServices.Constants.Dictionary.Keys;
using cs = MACServices.Constants.Strings;

namespace UserControls
{
    public partial class GroupSelection : UserControl
    {
        public int TotalGroups = 0;
        public Group MyGroup;
        public Event MyEvent;
        public int ParentCount = 0;
        public int DecendantCount = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (IsPostBack)
                {
                    var selectedGroupId = dlMasterGroups.Items[dlMasterGroups.SelectedIndex].Value;
                    var selectedGroupName = dlMasterGroups.Items[dlMasterGroups.SelectedIndex].Text;

                    Session["SelectedGroup"] = selectedGroupName + "|" + selectedGroupId;
                }

                GetGroupList();
            }
            catch (Exception ex)
            {
                var mDetails = ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
            }
        }

        public void GetGroupList()
        {
            dlMasterGroups.Items.Clear();

            // Get available groups and subgroups
            var groupList = new MacListAdHoc("Group", "GroupType", "RootGroup", true, "Relationships,MacOasServicesUrl");

            // Here we need to process the xml response into a List collection
            var xmlDoc = groupList.ListXml;
            var xmlGroups = xmlDoc.GetElementsByTagName("group");

            foreach (XmlNode currentGroup in xmlGroups)
            {
                if (currentGroup.Attributes != null)
                {
                    var groupName = currentGroup.Attributes[0].Value;
                    var groupId = currentGroup.Attributes[1].Value;
                    var groupLevel = currentGroup.Attributes[2].Value;

                    var groupEnabled = currentGroup.ChildNodes[1].InnerText;
                    var groupMacoasServicesUrl = currentGroup.ChildNodes[2].InnerText;

                    var li = new ListItem {Text = groupLevel + @") " + groupName, Value = groupId};

                    li.Attributes.Add("Enabled", groupEnabled);
                    li.Attributes.Add("MacOasServicesUrl", groupMacoasServicesUrl);

                    var adminDoc = new XmlDocument();
                    adminDoc.LoadXml(currentGroup.OuterXml);

                    // Get admin count
                    var xmlAdministrators = adminDoc.GetElementsByTagName("administrator");
                    var groupAdministratorCount = xmlAdministrators.Cast<XmlNode>().Count(currentAdministrator => currentAdministrator.Attributes != null && groupId == currentAdministrator.Attributes["parentid"].Value.Trim());
                    li.Attributes.Add("AdministratorCount", groupAdministratorCount.ToString(CultureInfo.CurrentCulture));

                    // Get Client count
                    var xmlClients = adminDoc.GetElementsByTagName("client");
                    var groupClientCount = xmlClients.Cast<XmlNode>().Count(currentClient => currentClient.Attributes != null && groupId == currentClient.Attributes["parentid"].Value.Trim());
                    li.Attributes.Add("ClientCount", groupClientCount.ToString(CultureInfo.CurrentCulture));

                    li.Attributes.Add("class", "ListItemIndentLevel_" + groupLevel);

                    if (!Convert.ToBoolean(groupEnabled))
                        li.Attributes.Add("style", "color: #ff0000");

                    li.Attributes.Add("ondblclick", "javascript: setActiveGroup(this);");

                    dlMasterGroups.Items.Add(li);
                }

                TotalGroups++;
            }

            spanGroupCount.InnerHtml = TotalGroups + " Groups Available";
        }
    }
}