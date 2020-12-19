using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using MongoDB.Bson;

using MACSecurity;
using MACServices;
using dk = MACServices.Constants.Dictionary.Keys;
using MUtils = MACServices.Utils;

namespace UserControls
{
    public partial class GroupManagement : UserControl
    {
        HelpUtils helpUtils = new HelpUtils();

        public HtmlForm MyForm;

        public TextBox ServiceResponseMessage;
        public HiddenField hiddenD;
        public HiddenField hiddenF;
        public HiddenField hiddenE;
        public HiddenField hiddenG;
        public HiddenField hiddenH;
        public HiddenField hiddenI;
        public HiddenField hiddenJ;
        public HiddenField hiddenK;
        public HiddenField hiddenL;
        public HiddenField hiddenA;

        public Group MyGroup;
        public Group MyParentGroup = new Group("");

        public int TotalGroups;
        public int TotalAdministrators;
        public int TotalClients;
        public int ParentCount;
        public int DecendantCount;
        public bool IsParent = true;
        public int SelectIndex;
        public int IndexNumber;

        public string decryptedUserId = "";
        public string AdminFullName = "???";

        public UserProfile AdminProfile = new UserProfile("");

        Utils mUtils = new Utils();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.Master != null)
            {
                MyForm = (HtmlForm)Page.Master.FindControl("formMain");

                ServiceResponseMessage = (TextBox)Page.Master.FindControl("div_serviceResponseMessage");

                hiddenD = (HiddenField)Page.Master.FindControl("hiddenD");
                hiddenF = (HiddenField)Page.Master.FindControl("hiddenF");
                hiddenE = (HiddenField)Page.Master.FindControl("hiddenE");
                hiddenG = (HiddenField)Page.Master.FindControl("hiddenG");
                hiddenH = (HiddenField)Page.Master.FindControl("hiddenH");
                hiddenI = (HiddenField)Page.Master.FindControl("hiddenI");
                hiddenJ = (HiddenField)Page.Master.FindControl("hiddenJ");
                hiddenK = (HiddenField)Page.Master.FindControl("hiddenK");
                hiddenL = (HiddenField)Page.Master.FindControl("hiddenL");
                hiddenA = (HiddenField)Page.Master.FindControl("hiddenA");

                var bodyContainer = (Panel)Page.Master.FindControl("BodyContainer");
                bodyContainer.Height = 550;
            }

            decryptedUserId = MACSecurity.Security.DecodeAndDecrypt(hiddenE.Value, Constants.Strings.DefaultClientId);

            GetToolTips();

            TotalAdministrators = 0;
            TotalClients = 0;

            try
            {
                var  statusMsg = divGroupManagementMessage;

                if (IsPostBack)
                {
                    #region Handle Postback

                    var myEvent = new Event();

                    switch (hiddenAA.Value)
                    {
                        case "EnableGroup":
                            // Update the selected Group
                            MyGroup = new Group(hiddenSelectedParentGroup.Value) { Enabled = true };
                            MyGroup.Update();

                            AdminProfile = new UserProfile(decryptedUserId);
                            AdminFullName = Security.DecodeAndDecrypt(AdminProfile.FirstName, decryptedUserId) + " " + Security.DecodeAndDecrypt(AdminProfile.LastName, decryptedUserId);

                            myEvent._id = ObjectId.GenerateNewId();
                            myEvent.UserId = ObjectId.Parse(decryptedUserId);
                            myEvent.EventTypeDesc = Constants.TokenKeys.ParentGroupName + MyGroup.Name;
                            myEvent.EventTypeDesc += Constants.TokenKeys.EventGeneratedByName + AdminFullName;

                            myEvent.Create(Constants.EventLog.Group.ParentGroup.Enabled, null);

                            statusMsg.InnerHtml = myEvent.EventTypeDesc;
                            statusMsg.Style["display"] = "block";

                            // Disable any decendents as well
                            foreach (var currentRelationship in MyGroup.Relationships)
                            {
                                switch (currentRelationship.MemberType)
                                {
                                    case "Group":
                                        if (currentRelationship.MemberHierarchy == "Child")
                                            EditDecendants(MyGroup._id, currentRelationship.MemberId, MyGroup.Enabled, AdminFullName);
                                        break;
                                }
                            }

                            GetGroupList();

                            hiddenAA.Value = "";
                            break;

                        case "DisableGroup":
                            // Update the selected Group
                            MyGroup = new Group(hiddenSelectedParentGroup.Value) {Enabled = false};
                            MyGroup.Update();

                            AdminProfile = new UserProfile(decryptedUserId);
                            AdminFullName = Security.DecodeAndDecrypt(AdminProfile.FirstName, decryptedUserId) + " " + Security.DecodeAndDecrypt(AdminProfile.LastName, decryptedUserId);

                            myEvent._id = ObjectId.GenerateNewId();
                            myEvent.UserId = ObjectId.Parse(decryptedUserId);
                            myEvent.EventTypeDesc += Constants.TokenKeys.ParentGroupName + MyGroup.Name;
                            myEvent.EventTypeDesc += Constants.TokenKeys.EventGeneratedByName + AdminFullName;
                            myEvent.Create(Constants.EventLog.Group.ParentGroup.Disabled, null);

                            statusMsg.InnerHtml = myEvent.EventTypeDesc;
                            statusMsg.Style["display"] = "block";

                            // Disable any decendents as well
                            foreach (var currentRelationship in MyGroup.Relationships)
                            {
                                switch (currentRelationship.MemberType)
                                {
                                    case "Group":
                                        if (currentRelationship.MemberHierarchy == "Child")
                                            EditDecendants(MyGroup._id, currentRelationship.MemberId, MyGroup.Enabled, AdminFullName);
                                        break;
                                }
                            }

                            GetGroupList();

                            hiddenAA.Value = "";
                            break;

                        case "CreateGroup":
                            MyGroup = new Group("")
                            {
                                _id = ObjectId.GenerateNewId(),
                                Name = txtGroupName.Value,
                                GroupType = "RootGroup",
                                MacOasServicesUrl = txtMACOASServicesUrl.Value,
                                CreatedById = ObjectId.Parse(decryptedUserId),
                            };

                            AdminProfile = new UserProfile(decryptedUserId);
                            AdminFullName = Security.DecodeAndDecrypt(AdminProfile.FirstName, decryptedUserId) + " " + Security.DecodeAndDecrypt(AdminProfile.LastName, decryptedUserId);

                            MyGroup.Create();

                            #region Add the new Group to the Parent Group's relationships node if specified

                                if (hiddenSelectedParentGroup.Value != "")
                                {
                                    IsParent = false;
                                    MyParentGroup = new Group(hiddenSelectedParentGroup.Value);

                                    MyGroup.GroupType = "ChildGroup";

                                    mUtils.ManageObjectRelationships_GroupAndGroup(ObjectId.Parse(decryptedUserId), true, MyGroup, MyParentGroup);
                                }

                            #endregion

                            if (IsParent)
                            {
                                myEvent._id = ObjectId.GenerateNewId();
                                myEvent.UserId = ObjectId.Parse(decryptedUserId);
                                myEvent.EventTypeDesc = Constants.TokenKeys.ParentGroupName + MyGroup.Name;
                                myEvent.EventTypeDesc += Constants.TokenKeys.EventGeneratedByName + AdminFullName;

                                myEvent.Create(Constants.EventLog.Group.ParentGroup.Created, null);

                                statusMsg.InnerHtml = myEvent.EventTypeDesc;
                                statusMsg.Style["display"] = "block";
                            }
                            else
                            {
                                myEvent._id = ObjectId.GenerateNewId();
                                myEvent.UserId = ObjectId.Parse(decryptedUserId);
                                myEvent.EventTypeDesc = Constants.TokenKeys.EventGeneratedByName + AdminFullName;
                                myEvent.EventTypeDesc += Constants.TokenKeys.SubGroupName + MyGroup.Name;
                                myEvent.EventTypeDesc += Constants.TokenKeys.ParentGroupName + MyParentGroup.Name;

                                myEvent.Create(Constants.EventLog.Group.ChildGroup.Created, null);

                                statusMsg.InnerHtml = myEvent.EventTypeDesc;
                                statusMsg.Style["display"] = "block";
                            }

                            #region Create administrator relationship

                                mUtils.ManageObjectRelationships_AdminAndGroup(ObjectId.Parse(decryptedUserId), true, AdminProfile, MyGroup);

                            #endregion

                            hiddenAA.Value = "";

                            // Reset fields
                            txtGroupName.Value = MyGroup.Name;
                            txtGroupName.Disabled = false;
                            txtGroupName.Focus();

                            txtMACOASServicesUrl.Value = MyGroup.MacOasServicesUrl;
                            txtMACOASServicesUrl.Disabled = false;
                            break;

                        case "UpdateGroup":

                            string enabledState;

                            MyGroup = new Group(hiddenSelectedParentGroup.Value);

                            foreach (var relationship in MyGroup.Relationships)
                            {
                                switch (relationship.MemberType)
                                {
                                    case "Administrator":
                                        TotalAdministrators++;
                                        break;

                                    case "Client":
                                        TotalClients++;
                                        break;
                                }
                            }

                            var btnGroupEnabledState = btnEnableGroup.Value;
                            var groupEnabled = true;
                            if (btnGroupEnabledState == "Disable")
                            {
                                enabledState = "Enabled";
                            }
                            else
                            {
                                groupEnabled = false;
                                enabledState = "Disabled";
                            }

                            statusMsg.InnerHtml = "Group (" + enabledState + ") - The parent-group (" + MyGroup.Name + ")";
                            statusMsg.Style["display"] = "block";

                            // Update the selected Group
                            MyGroup.Name = txtGroupName.Value;
                            MyGroup.Enabled = groupEnabled;
                            MyGroup.MacOasServicesUrl = txtMACOASServicesUrl.Value;

                            AdminProfile = new UserProfile(decryptedUserId);
                            AdminFullName = Security.DecodeAndDecrypt(AdminProfile.FirstName, decryptedUserId) + " " + Security.DecodeAndDecrypt(AdminProfile.LastName, decryptedUserId);

                            // Log the differences
                            Group originalGroup = new Group(spanGroupID.InnerText);
                            var sDifferences = mUtils.GetObjectDifferences(originalGroup, MyGroup);

                            var changeEvent = new Event {UserId = ObjectId.Parse(decryptedUserId)};
                            changeEvent.EventTypeDesc = Constants.TokenKeys.EventGeneratedByName + AdminFullName;
                            changeEvent.EventTypeDesc += Constants.TokenKeys.ObjectPropertiesUpdated + sDifferences;

                            if (hiddenSelectedParentGroup.Value != "")
                            {
                                myEvent._id = ObjectId.GenerateNewId();
                                myEvent.UserId = ObjectId.Parse(decryptedUserId);
                                myEvent.EventTypeDesc = Constants.TokenKeys.ParentGroupName + MyGroup.Name;
                                myEvent.EventTypeDesc += Constants.TokenKeys.EventGeneratedByName + AdminFullName;
                                myEvent.EventTypeDesc += Constants.TokenKeys.ObjectPropertiesUpdated + sDifferences;

                                myEvent.Create(Constants.EventLog.Group.ParentGroup.Updated, null);
                            }
                            else
                            {
                                myEvent._id = ObjectId.GenerateNewId();
                                myEvent.UserId = ObjectId.Parse(decryptedUserId);
                                myEvent.EventTypeDesc = Constants.TokenKeys.EventGeneratedByName + AdminFullName;
                                myEvent.EventTypeDesc += Constants.TokenKeys.SubGroupName + MyGroup.Name;
                                myEvent.EventTypeDesc += Constants.TokenKeys.ParentGroupName + MyParentGroup.Name;
                                myEvent.EventTypeDesc += Constants.TokenKeys.ObjectPropertiesUpdated + sDifferences;

                                myEvent.Create(Constants.EventLog.Group.ChildGroup.Updated, null);
                            }

                            var tmpVal = myEvent.EventTypeDesc.Split(':');

                            statusMsg.InnerHtml = tmpVal[0].Replace(" Changes", "");
                            statusMsg.Style["display"] = "block";

                            // Enable/Disable any decendents as well
                            foreach (var currentRelationship in MyGroup.Relationships)
                            {
                                switch (currentRelationship.MemberType)
                                {
                                    case "Administrator":
                                        if (currentRelationship.MemberId.ToString() != Constants.Strings.DefaultEmptyObjectId)
                                        {
                                            // Create the relationships here
                                            var adminProfile = new UserProfile(currentRelationship.MemberId.ToString());

                                            var assignedAdminName = Security.DecodeAndDecrypt(adminProfile.FirstName, currentRelationship.MemberId.ToString()) + " " + Security.DecodeAndDecrypt(adminProfile.LastName, currentRelationship.MemberId.ToString());
                                            var userRole = mUtils.GetRoleNameByRoleId(adminProfile.Roles[0].ToString());

                                            var adminRelationship = new Relationship
                                            {
                                                MemberId = MyGroup._id,
                                                MemberType = "Group",
                                                MemberHierarchy = "Administrator"
                                            };

                                            adminProfile.Relationships.Add(adminRelationship);
                                            adminProfile.Name = assignedAdminName;
                                            adminProfile.Update();

                                            // Log the event
                                            var adminEvent = new Event();
                                            adminEvent._id = ObjectId.GenerateNewId();
                                            myEvent.UserId = ObjectId.Parse(decryptedUserId);
                                            adminEvent.EventTypeDesc = Constants.TokenKeys.UserRole + userRole;
                                            adminEvent.EventTypeDesc += Constants.TokenKeys.GroupName + MyGroup.Name;
                                            adminEvent.EventTypeDesc += Constants.TokenKeys.EventGeneratedByName + AdminFullName;

                                            adminEvent.Create(Constants.EventLog.Assignments.AdminAssignedToGroup, null);
                                        }
                                        break;

                                    case "Group":
                                        if (currentRelationship.MemberHierarchy == "Child")
                                            EditDecendants(MyGroup._id, currentRelationship.MemberId, MyGroup.Enabled, AdminFullName);
                                        break;
                                }
                            }

                            MyGroup.Update();

                            GetGroupList();

                            spanMembershipStats.InnerHtml = "Administrators: <span style='color: #ff0000;'>" + TotalAdministrators + "</span>&nbsp;&nbsp;&nbsp;&nbsp;Clients: <span style='color: #ff0000;'>" + TotalClients + "</span>";
                            hiddenAA.Value = "";

                            // Reset fields
                            spanGroupID.InnerHtml = MyGroup._id.ToString();

                            btnCreateRootGroup.Disabled = true;
                            btnCreateSubRootGroup.Disabled = false;
                            btnEnableGroup.Disabled = false;
                            btnSelectedAdministrators.Disabled = false;
                            btnSaveGroup.Disabled = false;
                            btnCancelGroup.Disabled = false;

                            txtGroupName.Value = MyGroup.Name;
                            txtGroupName.Disabled = false;
                            txtGroupName.Focus();

                            txtMACOASServicesUrl.Value = MyGroup.MacOasServicesUrl;
                            txtMACOASServicesUrl.Disabled = false;
                            break;
                    }
                    #endregion
                }
                else
                {
                    txtGroupName.Value = "";
                    txtMACOASServicesUrl.Value = ConfigurationManager.AppSettings["DefaultMacOasServicesUrl"];
                }
            }
            catch (Exception ex)
            {
                var mDetails = ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                var exceptionEvent = new Event
                {
                    _id = ObjectId.GenerateNewId(),
                    UserId = ObjectId.Parse(decryptedUserId),
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
            }

            if (Request["indexnumber"] != null)
                hiddenSelectedIndexNumber.Value = Request["indexnumber"].ToString(CultureInfo.CurrentCulture);

            if (Request["gid"] != null)
            {
                hiddenSelectedParentGroup.Value = Request["gid"].ToString(CultureInfo.CurrentCulture);
                var myGroup = new Group(hiddenSelectedParentGroup.Value);

                Session["OriginalGroup"] = myGroup;

                btnEnableGroup.Value = myGroup.Enabled ? "Disable" : "Enable";

                txtGroupName.Value = myGroup.Name;
                txtMACOASServicesUrl.Value = myGroup.MacOasServicesUrl;

                btnSaveGroup.Value = "Update";
            }

            GetGroupList();

            if (Convert.ToBoolean(MACSecurity.Security.DecodeAndDecrypt(hiddenF.Value, decryptedUserId)))
            {
                btnCreateRootGroup.Visible = false;
                btnCreateSubRootGroup.Visible = false;
                btnEnableGroup.Visible = false;
                btnSelectedAdministrators.Visible = false;
                btnSaveGroup.Visible = false;
                btnCancelGroup.Visible = false;
            }
        }

        public void EditDecendants(ObjectId parentId, ObjectId decendantId, bool decendantEnabled, string adminName)
        {
            try
            {
                var myEvent = new Event();

                var enabledState = "Disabled";

                if (decendantEnabled)
                    enabledState = "Enabled";

                var decendantGroup = new Group(decendantId.ToString())
                {
                    GroupType = "ClientGroup",
                    Enabled = decendantEnabled
                };
                decendantGroup.Update();

                myEvent._id = ObjectId.GenerateNewId();
                myEvent.UserId = ObjectId.Parse(decryptedUserId);
                myEvent.EventTypeDesc = Constants.TokenKeys.ParentGroupName + MyGroup.Name;
                myEvent.EventTypeDesc += Constants.TokenKeys.SubGroupName + decendantGroup.Name;
                myEvent.EventTypeDesc += Constants.TokenKeys.EventGeneratedByName + AdminFullName;

                myEvent.Create(enabledState == "Disabled" ? Constants.EventLog.Group.ChildGroup.Disabled : Constants.EventLog.Group.ChildGroup.Enabled, null);

                // Edit any decendents of the current item
                foreach (var currentRelationship in decendantGroup.Relationships)
                {
                    switch (currentRelationship.MemberType)
                    {
                        case "Administrator":
                            // Create the relations ships here
                            var adminProfile = new UserProfile(currentRelationship.MemberId.ToString());

                            var assignedAdminName = Security.DecodeAndDecrypt(adminProfile.FirstName, currentRelationship.MemberId.ToString()) + " " + Security.DecodeAndDecrypt(adminProfile.LastName, currentRelationship.MemberId.ToString());
                            var userRole = mUtils.GetRoleNameByRoleId(adminProfile.Roles[0].ToString());

                            var adminRelationship = new Relationship
                            {
                                MemberId = MyGroup._id,
                                MemberType = "Group",
                                MemberHierarchy = "Administrator"
                            };

                            adminProfile.Relationships.Add(adminRelationship);
                            adminProfile.Name = assignedAdminName;
                            adminProfile.Update();

                            // Log the event
                            var adminEvent = new Event();

                            adminEvent._id = ObjectId.GenerateNewId();
                            myEvent.UserId = ObjectId.Parse(decryptedUserId);
                            adminEvent.EventTypeDesc = Constants.TokenKeys.UserRole + userRole;
                            adminEvent.EventTypeDesc += Constants.TokenKeys.GroupName + decendantGroup.Name;
                            adminEvent.EventTypeDesc += Constants.TokenKeys.EventGeneratedByName + assignedAdminName;
                            adminEvent.Create(Constants.EventLog.Assignments.AdminAssignedToGroup, null);
                            break;

                        case "Group":
                            if (currentRelationship.MemberHierarchy == "Child")
                                EditDecendants(decendantGroup._id, currentRelationship.MemberId, decendantGroup.Enabled, AdminFullName);
                            break;
                    }
                }
                //else
                //{

                //}
            }
            catch(Exception ex)
            {
                var mDetails = ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + "NA";
                var exceptionEvent = new Event
                {
                    _id = ObjectId.GenerateNewId(),
                    UserId = ObjectId.Parse(decryptedUserId),
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
            }
        }

        public void GetGroupList()
        {
            TotalGroups = 0;
            IndexNumber = 0;

            var processTimer = Stopwatch.StartNew();

            // Get available groups and subgroups
            var groupList = new MacListAdHoc("Group", "GroupType", "RootGroup", true, "Relationships,OwnerLogoUrl,MacOasServicesUrl");

            processTimer.Stop();
            var xmlListProcessTime = processTimer.ElapsedMilliseconds.ToString(CultureInfo.CurrentCulture);

            processTimer = Stopwatch.StartNew();

            var xmlDoc = groupList.ListXml;
            var xmlGroups = xmlDoc.GetElementsByTagName("group");

            var renderData = "";

            foreach (XmlNode currentGroup in xmlGroups)
            {
                renderData += NewRenderMethod(currentGroup, xmlDoc);
                TotalGroups++;
            }

            GroupControlsContainer.InnerHtml = renderData;

            h2PageTitle.InnerHtml = TotalGroups + " Groups available.";

            var groupListProcessTime = processTimer.ElapsedMilliseconds.ToString(CultureInfo.CurrentCulture);
            var totalTime = Convert.ToInt32(xmlListProcessTime) + Convert.ToInt32(groupListProcessTime);
        }

        public string NewRenderMethod(XmlNode groupDoc, XmlDocument masterGroups)
        {
            var sbListItems = new StringBuilder();

            if (groupDoc.Attributes != null)
            {
                var groupName = groupDoc.Attributes["name"].Value;
                var groupId = groupDoc.Attributes["id"].Value;
                var groupLevel = groupDoc.Attributes["heirarchy"].Value;

                //divGroupList.InnerHtml += groupName + "<br />";

                var groupEnabled = groupDoc.Attributes["enabled"].Value;
                var parentId = groupDoc.Attributes["parentid"].Value;

                var groupMacOasServicesUrl = groupDoc.ChildNodes[1].InnerText;

                var ownerLogoUrl = "";
                if (groupDoc.ChildNodes[2] != null)
                    ownerLogoUrl = groupDoc.ChildNodes[2].InnerText;
                else
                {
                    // Update the OwnerLogoUrl property if null or empty
                    Group myGroup = new Group(groupDoc.Attributes["id"].Value);
                    myGroup.OwnerLogoUrl = Constants.Common.EmptyOwnerLogoUrl;
                    myGroup.Update();
                    ownerLogoUrl = Constants.Common.EmptyOwnerLogoUrl;
                }

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
                        sbListItems.Append(" ownerlogourl='" + ownerLogoUrl + "'");
                        sbListItems.Append(" macoasservicesurl='" + groupMacOasServicesUrl + "'");
                        sbListItems.Append(" administratorcount='" + groupAdministratorCount.ToString(CultureInfo.CurrentCulture) + "'");
                        sbListItems.Append(" clientcount='" + groupClientCount.ToString(CultureInfo.CurrentCulture) + "'");
                        sbListItems.Append(" endusercount='0'");
                        sbListItems.Append(" otpsentcount='0'");
                        sbListItems.Append(" parentid='" + parentId + "'");

                        if (hiddenSelectedIndexNumber.Value != "")
                        {
                            sbListItems.Append(hiddenSelectedParentGroup.Value == groupId
                                ? " isselected='true'"
                                : " isselected='false'");
                        }
                        else
                        {
                            sbListItems.Append(" isselected='false'");
                        }

                        sbListItems.Append(" indexnumber='" + IndexNumber + "'");
                        IndexNumber++;
                    }
                }

                if (!Convert.ToBoolean(groupEnabled))
                    sbListItems.Append(" style='color: #ff0000;'");

                sbListItems.Append(">");
                sbListItems.Append("<span id='spanGroupName_" + groupId + "'>" + groupLevel + @") " + groupName + "</span>");
            }

            sbListItems.Append("</div>");

            return sbListItems.ToString();
        }

        public string SanitizeXmlValue(string inputString)
        {
            var cleanedString = inputString;
            cleanedString = cleanedString.Replace("'", "&apos;");

            return cleanedString;
        }

        public void GetToolTips()
        {
            txtGroupName.Attributes.Add("title", helpUtils.GetToolTips("5494a36bead6360ffcd07de5"));
            txtMACOASServicesUrl.Attributes.Add("title", helpUtils.GetToolTips("5494ac61ead6360ffcd7801e"));
        }

        static Predicate<Relationship> FindRelationshipByObjectId(ObjectId objectId)
        {
            return relationship => relationship.MemberId == objectId;
        }

    }
}