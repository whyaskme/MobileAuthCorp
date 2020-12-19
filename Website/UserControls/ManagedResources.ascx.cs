using System;
using System.Globalization;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using MACServices;

namespace UserControls
{
    public partial class ManagedResources : UserControl
    {
        public HiddenField hiddenE;
        public HiddenField hiddenG;
        public HiddenField hiddenH;
        public HiddenField hiddenI;
        public HiddenField hiddenJ;
        public HiddenField hiddenK;
        public HiddenField hiddenL;
        public Panel PnlManagedResources;
        public int IndexNumber;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.Master != null)
            {
                hiddenE = (HiddenField)Page.Master.FindControl("hiddenE");
                hiddenG = (HiddenField)Page.Master.FindControl("hiddenG");
                hiddenH = (HiddenField)Page.Master.FindControl("hiddenH");
                hiddenI = (HiddenField)Page.Master.FindControl("hiddenI");
                hiddenJ = (HiddenField)Page.Master.FindControl("hiddenJ");
                hiddenK = (HiddenField)Page.Master.FindControl("hiddenK");
                hiddenL = (HiddenField)Page.Master.FindControl("hiddenL");
            }

            PnlManagedResources = (Panel)Parent.FindControl("pnlManagedResources");

            GetMyGroupList();
            GetMyGroupClientList();
            GetMyStandaloneClientList();
            //GetMyLast5EventsList();
        }

        private void GetMyGroupList()
        {
            int TotalGroups = 0;

            // Get available groups and subgroups
            var groupList = new MacListAdHoc("Group", "GroupType", "RootGroup", true, "Relationships,MacOasServicesUrl");
            //var groupList = new MacListAdHoc("Group", "Relationships.MemberId", hiddenE.Value, true, "Relationships,MacOasServicesUrl");

            // Here we need to process the xml response into a List collection
            var xmlDoc = groupList.ListXml;
            var xmlGroups = xmlDoc.GetElementsByTagName("group");

            var renderData = "&nbsp;&nbsp;My Groups (???)";

            foreach (XmlNode currentGroup in xmlGroups)
            {
                renderData += NewRenderMethod(currentGroup, xmlDoc);
                TotalGroups++;
            }

            renderData = renderData.Replace("(???)", "(" + TotalGroups + ")");
            divMyGroups.InnerHtml = renderData;
        }

        public string NewRenderMethod(XmlNode groupDoc, XmlDocument masterGroups)
        {
            string groupName = "";
            string groupId = "";
            string groupLevel = "";

            string groupEnabled = "";
            string groupMacOasServicesUrl = "";

            var parentDoc = new XmlDocument();

            string parentId = "";

            var sbListItems = new StringBuilder();

            if (groupDoc.Attributes != null)
            {
                groupName = groupDoc.Attributes["name"].Value;
                groupId = groupDoc.Attributes["id"].Value;
                groupLevel = groupDoc.Attributes["heirarchy"].Value;

                groupEnabled = groupDoc.Attributes["enabled"].Value;
                parentId = groupDoc.Attributes["parentid"].Value;

                groupMacOasServicesUrl = groupDoc.ChildNodes[1].InnerText;

                var xmlNodeList = groupDoc.SelectNodes("relationships/administrator");
                if (xmlNodeList != null)
                {
                    var groupAdministratorCount = xmlNodeList.Count;
                    var selectNodes = groupDoc.SelectNodes("relationships/client");
                    if (selectNodes != null)
                    {
                        var groupClientCount = selectNodes.Count;

                        // Add item to the new group control
                        sbListItems.Append("<div");
                        sbListItems.Append(" onclick='javascript: setActiveGroup(this);'");
                        sbListItems.Append(" id='" + groupId + "'");
                        sbListItems.Append(" groupname='" + groupName.Replace("'", "&apos;") + "'");
                        sbListItems.Append(" class='ListItemIndentLevel_" + groupLevel + "'");
                        sbListItems.Append(" enabled='" + groupEnabled + "'");
                        sbListItems.Append(" macoasservicesurl='" + groupMacOasServicesUrl + "'");
                        sbListItems.Append(" administratorcount='" + groupAdministratorCount.ToString(CultureInfo.CurrentCulture) + "'");
                        sbListItems.Append(" clientcount='" + groupClientCount.ToString(CultureInfo.CurrentCulture) + "'");
                        sbListItems.Append(" parentid='" + parentId + "'");
                        sbListItems.Append(" indexnumber='" + IndexNumber + "'");
                        IndexNumber++;
                    }
                }

                if (!Convert.ToBoolean(groupEnabled))
                    sbListItems.Append(" style='color: #ff0000;'");

                sbListItems.Append(">");
                sbListItems.Append("<span id='spanGroupName_" + groupId + "' onclick='javascript: navigateToGroup(this);'>" + groupLevel + @") " + groupName + "</span>");
            }

            sbListItems.Append("</div>");

            return sbListItems.ToString();
        }

        private void GetMyGroupClientList()
        {
            
        }

        private void GetMyStandaloneClientList()
        {
            var itemCount = 0;
            var userId = "";
            var collectionLabel = "My ";

            if (hiddenE.Value != null)
                userId = hiddenE.Value;

            // Check which page we are on. This will tells us which UserId to use
            var currentPage = Request.ServerVariables["Url"];
            if (currentPage != "")
            {
                // Set the userId from the dropdown list
                var dlUsers = (DropDownList)Parent.FindControl("dlUsers");
                if (dlUsers != null)
                {
                    userId = dlUsers.SelectedValue;

                    if (dlUsers.SelectedItem != null)
                    {
                        //var arrSelectedName = dlUsers.SelectedItem.Text.Split(' ');
                        collectionLabel = "";
                    }
                }
            }

            var clientList = new MacListAdHoc("Client", "Relationships.MemberId", userId, false, "");

            // Here we need to process the xml response into a List collection
            var xmlDoc = clientList.ListXml;
            var xmlClients = xmlDoc.GetElementsByTagName("client");

            foreach (XmlNode currentClient in xmlClients)
            {
                itemCount++;

                if (currentClient.Attributes != null)
                {
                    var clientName = "(SC) " + currentClient.Attributes[0].Value; // Stand Alone Client
                    var clientId = currentClient.Attributes[1].Value;

                    var li = new ListItem {Text = clientName, Value = clientId};

                    li.Attributes.Add("class", "ListItemIndentLevel_1");
                    li.Attributes.Add("onclick", "javascript: navigateToClient('" + li.Value + "');");
                    lstMyClients.Items.Add(li);
                }
            }

            var li2 = new ListItem
            {
                Text = collectionLabel + @" Clients (" + itemCount + @") - (GC)=Group - (SC)=Standalone",
                Value = Constants.Strings.DefaultEmptyObjectId
            };
            li2.Attributes.Add("class", "ListItemIndentLevel_0");
            lstMyClients.Items.Insert(0, li2);
        }

        private void GetMyLast5EventsList()
        {
            //lstMyLast5Events.Items.Clear();

            var itemCount = 0;
            var userId = "";
            var collectionLabel = "My ";

            if (hiddenE.Value != null)
                userId = hiddenE.Value;

            // Check which page we are on. This will tells us which UserId to use
            var currentPage = Request.ServerVariables["Url"];
            if (currentPage != "")
            {
                // Set the userId from the dropdown list
                var dlUsers = (DropDownList)Parent.FindControl("dlUsers");
                if (dlUsers != null)
                {
                    userId = dlUsers.SelectedValue;

                    if (dlUsers.SelectedItem != null)
                    {
                        //var arrSelectedName = dlUsers.SelectedItem.Text.Split(' ');
                        collectionLabel = "";
                    }
                }
            }

            var last5EventsList = new MacListAdHoc("Event", "UserId", userId, false, "EventTypeDesc,Date");

            // Here we need to process the xml response into a List collection
            var xmlDoc = last5EventsList.ListXml;
            var xmlLast5Events = xmlDoc.GetElementsByTagName("event");

            foreach (XmlNode currentEvent in xmlLast5Events)
            {
                itemCount++;

                if (itemCount < 6)
                {
                    var eventDate = currentEvent.ChildNodes[0].InnerText;

                    //var convertedDate = DateTime.SpecifyKind(
                    //    DateTime.Parse(eventDate),
                    //    DateTimeKind.Utc);

                    var eventDesc = currentEvent.ChildNodes[1].InnerText;
                    if (currentEvent.Attributes == null) continue;
                    var eventId = currentEvent.Attributes[1].Value;

                    var li = new ListItem {Text = eventDate + @" - " + eventDesc, Value = eventId};

                    li.Attributes.Add("class", "ListItemIndentLevel_1");
                    li.Attributes.Add("onclick", "javascript: navigateToEvent('" + li.Value + "');");
                    li.Attributes.Add("title", eventDesc);
                    //lstMyLast5Events.Items.Add(li);
                }
                else
                    break;
            }

            var li2 = new ListItem { Text = collectionLabel + @" Last (5) Events - Show Local Time", Value = Constants.Strings.DefaultEmptyObjectId };
            li2.Attributes.Add("class", "ListItemIndentLevel_0");
            li2.Attributes.Add("onclick", "javascript: showEventsLocalTime();");
            //lstMyLast5Events.Items.Insert(0, li2);
        }
    }
}