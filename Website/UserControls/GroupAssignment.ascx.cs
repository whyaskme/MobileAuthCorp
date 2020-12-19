using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

using MACServices;
using MongoDB.Bson;
using dk = MACServices.Constants.Dictionary.Keys;
using mUtils = MACServices.Utils;

namespace UserControls
{
    public partial class GroupAssignment : UserControl
    {
        Int16 _totalGroupsAvailable;
        Int16 _totalGroupsAssigned;

        public Client myClient;

        protected void Page_Load(object sender, EventArgs e)
        {
            var loggedInUserId = "";
            var clientId = ObjectId.GenerateNewId();
            var selectedGroupCount = 0;
            var Utils = new mUtils();

            try
            {
                if (Request["clientId"] != null)
                    if (Request["clientId"] != "")
                        clientId = ObjectId.Parse(Request["clientId"]);

                if (Request["loggedInUserId"] != null)
                    if (Request["loggedInUserId"] != "")
                        loggedInUserId = Request["loggedInUserId"];

                loggedInUserId = MACSecurity.Security.DecodeAndDecrypt(loggedInUserId, Constants.Strings.DefaultClientId);

                var userIsReadOnly = "";
                if (!String.IsNullOrEmpty(Request["userisreadonly"]))
                    userIsReadOnly = Request["userisreadonly"];

                userIsReadOnly = MACSecurity.Security.DecodeAndDecrypt(userIsReadOnly, loggedInUserId);

                myClient = new Client(clientId.ToString());

                if (IsPostBack)
                {
                    // Delete al group relationships from the client
                    myClient.Relationships.RemoveAll(FindRelationshipByMemberType("Group"));

                    // Find all groups that have a relationship with the client and delete each one
                    var groupsWithClient = new MacListAdHoc("Group", "Relationships.MemberId", clientId.ToString(), false, "Relationships");

                    // Here we need to process the xml response into a List collection
                    var xmlGroupDoc = groupsWithClient.ListXml;
                    var xmlGroups = xmlGroupDoc.GetElementsByTagName("relationships");
                    foreach (XmlElement currentGroup in xmlGroups)
                    {
                        var groupName = currentGroup.ParentNode.Attributes[0].Value;
                        var groupId = currentGroup.ParentNode.Attributes[1].Value;

                        var currentDeleteGroup = new Group(groupId);
                        currentDeleteGroup.Relationships.RemoveAll(FindRelationshipByMemberId(clientId));
                        currentDeleteGroup.Update();
                    }

                    // Loop through all the selected groups and create the client/group relationship.
                    var groupsContainer = hiddenYs.Value.Split(',');
                    foreach (var currentGroupId in groupsContainer)
                    {
                        if (currentGroupId != "")
                        {
                            var myGroup = new Group(currentGroupId);

                            Utils.ManageObjectRelationships_GroupAndClient(ObjectId.Parse(loggedInUserId), true, myClient, myGroup);
                            selectedGroupCount++;
                        }
                    }

                    assignGroupMessage.Visible = true;
                }
                else
                {
                    assignGroupMessage.Visible = false;
                }

                GetGroupList(myClient);

                if (Convert.ToBoolean(userIsReadOnly))
                {
                    btnSaveGroup.Visible = false;
                }
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

        static Predicate<Relationship> FindRelationshipByMemberType(string memberType)
        {
            return relationship => relationship.MemberType == memberType;
        }

        static Predicate<Relationship> FindRelationshipByMemberId(ObjectId memberId)
        {
            return relationship => relationship.MemberId == memberId;
        }

        private void GetGroupList(Client myClient)
        {
            // Get available groups and subgroups
            var groupList = new MacListAdHoc("Group", "GroupType", "RootGroup", true, "Relationships,MacOasServicesUrl");

            // Here we need to process the xml response into a List collection
            var xmlDoc = groupList.ListXml;
            var xmlGroups = xmlDoc.GetElementsByTagName("group");

            var renderData = xmlGroups.Cast<XmlNode>().Aggregate("", (current, currentGroup) => current + RenderGroups(myClient, currentGroup, xmlDoc));

            spanGroupCount.InnerHtml = _totalGroupsAvailable + " Groups Available";
            divGroupsContainer.InnerHtml = renderData;
        }

        public string RenderGroups(Client myClient, XmlNode groupDoc, XmlDocument masterGroups)
        {
            var currentGroupIds = "?";

            currentGroupIds = myClient.Relationships.Aggregate("", (current, currentRelationship) => current + (currentRelationship.MemberId.ToString() + "|"));

            var sbListItems = new StringBuilder();

            if (groupDoc.Attributes != null)
            {
                var groupName = groupDoc.Attributes["name"].Value;
                var groupId = groupDoc.Attributes["id"].Value;
                var groupLevel = groupDoc.Attributes["heirarchy"].Value;

                var groupEnabled = groupDoc.Attributes["enabled"].Value;
                var parentId = groupDoc.Attributes["parentid"].Value;

                var groupMacOasServicesUrl = groupDoc.ChildNodes[1].InnerText;

                var xmlNodeList = groupDoc.SelectNodes("relationships/administrator");
                if (xmlNodeList != null)
                {
                    var groupAdministratorCount = xmlNodeList.Count;
                    var selectNodes = groupDoc.SelectNodes("relationships/client");
                    if (selectNodes != null)
                    {
                        var groupClientCount = selectNodes.Count;

                        _totalGroupsAvailable++;

                        // Add item to the new group control
                        sbListItems.Append("<div");
                        sbListItems.Append(" onclick='javascript: selectGroupToList(this);'");
                        sbListItems.Append(" id='" + groupId + "'");
                        sbListItems.Append(" groupname='" + groupName.Replace("'", "&apos;") + "'");
                        sbListItems.Append(" class='ListItemIndentLevel_" + groupLevel + "'");
                        sbListItems.Append(" enabled='" + groupEnabled + "'");
                        sbListItems.Append(" macoasservicesurl='" + groupMacOasServicesUrl + "'");
                        sbListItems.Append(" administratorcount='" + groupAdministratorCount.ToString(CultureInfo.CurrentCulture) + "'");
                        sbListItems.Append(" clientcount='" + groupClientCount.ToString(CultureInfo.CurrentCulture) + "'");
                        sbListItems.Append(" parentid='" + parentId + "'");

                        if (currentGroupIds.Contains(groupId))
                        {
                            sbListItems.Append(" isselected='true'");
                            sbListItems.Append(" style='background-color: #accee5;color: #222;'");
                            _totalGroupsAssigned++;
                        }
                        else
                            sbListItems.Append(" isselected='false'");
                    }
                }

                if (!Convert.ToBoolean(groupEnabled))
                    sbListItems.Append(" style='color: #ff0000;'");

                sbListItems.Append(">");
                sbListItems.Append("<span id='spanGroupName_" + groupId + "' onclick='javascript: navigateToGroup(this);'>" + groupLevel + @") " + groupName + "</span>");
            }

            sbListItems.Append("</div>");

            if (_totalGroupsAssigned == 0)
                spanGroupAssignmentCount.InnerHtml = "0 groups currently assigned";
            else if (_totalGroupsAssigned == 1)
                spanGroupAssignmentCount.InnerHtml = "1 group currently assigned";
            else
                spanGroupAssignmentCount.InnerHtml = _totalGroupsAssigned + " groups currently assigned";

            return sbListItems.ToString();
        }

        public string SanitizeXmlValue(string inputString)
        {
            var cleanedString = inputString;
            cleanedString = cleanedString.Replace("'", "&apos;");

            return cleanedString;
        }
    }
}