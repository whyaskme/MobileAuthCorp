using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using MACSecurity;
using MACServices;
using MongoDB.Bson;
using dk = MACServices.Constants.Dictionary.Keys;

namespace UserControls
{
    public partial class AdminAssignmentGroup : UserControl
    {
        int _iTotalAdministrators;

        ObjectId groupId = ObjectId.GenerateNewId();
        string loggedInAdminId = "";
        int _selectedAdminCount;

        Group _myGroup;

        Utils mUtils = new Utils();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Request["groupID"] != null)
                    if (Request["groupID"]!= "")
                        groupId = ObjectId.Parse(Request["groupID"]);

                if (Request["loggedInAdminId"] != null)
                    if (Request["loggedInAdminId"] != "")
                        loggedInAdminId = Request["loggedInAdminId"];

                loggedInAdminId = MACSecurity.Security.DecodeAndDecrypt(loggedInAdminId, Constants.Strings.DefaultClientId);

                var userIsReadOnly = "";
                if (!String.IsNullOrEmpty(Request["userisreadonly"]))
                    userIsReadOnly = Request["userisreadonly"];

                userIsReadOnly = MACSecurity.Security.DecodeAndDecrypt(userIsReadOnly, loggedInAdminId);

                _myGroup = new Group(groupId.ToString());

                divGroupTitle.InnerHtml = _myGroup.Name + " Administrators";

                Session["Relationships"] = _myGroup.Relationships;

                if (IsPostBack)
                {
                    var relationships = new List<Relationship>();

                    if (Session["Relationships"].ToString() != "")
                        relationships = (List<Relationship>)Session["Relationships"];

                    for (var i = 0; i < dlAdministrators.Items.Count; i++)
                    {
                        var currentAdmin = dlAdministrators.Items[i];

                        if (currentAdmin.Value == Constants.Strings.DefaultEmptyObjectId) continue;

                        var createRelationship = false;
                        if (currentAdmin.Selected)
                        {
                            _selectedAdminCount++;
                            createRelationship = true;
                        }

                        // Handle the relationships
                        if (currentAdmin.Value.Trim() != Constants.Strings.DefaultAdminId)
                        {
                            var userProfile = new UserProfile(currentAdmin.Value);
                            mUtils.ManageObjectRelationships_AdminAndGroup(ObjectId.Parse(loggedInAdminId), createRelationship, userProfile, _myGroup);
                        }
                    }
                    ScriptManager.RegisterClientScriptBlock(Page, typeof(Page), "Script", "javascript: callParentDocumentFunction();", true); 
                }
                else
                {
                    GetAdministratorList();
                }

                if (Convert.ToBoolean(userIsReadOnly))
                {
                    btnSaveAdministrator.Visible = false;
                    btnCloseWindow.Visible = false;
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

        public void GetAdministratorList()
        {
            dlAdministrators.Items.Clear();

            #region Get System Administrators first

            var systemAdminList = new MacListAdHoc("UserProfile", "Roles", Constants.Strings.UserRoles.SystemAdmin.Item3, false, "");

            // Here we need to process the xml response into a List collection
            var xmlSystemAdminDoc = systemAdminList.ListXml;
            var xmlSystemAdmins = xmlSystemAdminDoc.GetElementsByTagName("userprofile");

            var systemAdmin = new ListItem {Text = @"System Administrators", Value = Constants.Strings.DefaultEmptyObjectId};
            systemAdmin.Attributes.Add("class", "ListItemIndentLevel_0");
            systemAdmin.Attributes.Add("systemrootitem", "true");

            dlAdministrators.Items.Add(systemAdmin);

            foreach (XmlNode currentAdmin in xmlSystemAdmins)
            {
                if (currentAdmin.Attributes != null)
                {
                    var adminId = currentAdmin.Attributes["id"].Value;

                    // Get the admin's name
                    var userProfile = new UserProfile(adminId);

                    var adminName = Security.DecodeAndDecrypt(userProfile.FirstName, adminId) + " " + Security.DecodeAndDecrypt(userProfile.LastName, adminId);

                    if (adminName != "!System Administrator")
                    {
                        var li = new ListItem { Text = adminName, Value = adminId };

                        li.Attributes.Add("class", "ListItemIndentLevel_1");
                        li.Attributes.Add("systemchilditem", "true");

                        var bAdminIsAssigned = IsItemSelected(li.Value);
                        if (bAdminIsAssigned)
                            li.Selected = true;

                        dlAdministrators.Items.Add(li);
                    }
                }
                _iTotalAdministrators++;
            }

            #endregion

            #region Get Group Administrators next

            var groupAdminList = new MacListAdHoc("UserProfile", "Roles", Constants.Strings.UserRoles.GroupAdmin.Item3, false, "");

            // Here we need to process the xml response into a List collection
            var xmlGroupAdminDoc = groupAdminList.ListXml;
            var xmlGroupAdmins = xmlGroupAdminDoc.GetElementsByTagName("userprofile");

            var groupAdmin = new ListItem {Text = @"Group Administrators", Value = Constants.Strings.DefaultEmptyObjectId};
            groupAdmin.Attributes.Add("class", "ListItemIndentLevel_0");
            groupAdmin.Attributes.Add("grouprootitem", "true");
            dlAdministrators.Items.Add(groupAdmin);

            foreach (XmlNode currentAdmin in xmlGroupAdmins)
            {
                if (currentAdmin.Attributes != null)
                {
                    var adminId = currentAdmin.Attributes["id"].Value;

                    // Get the admin's name
                    var userProfile = new UserProfile(adminId);

                    var adminName = Security.DecodeAndDecrypt(userProfile.FirstName, adminId) + " " + Security.DecodeAndDecrypt(userProfile.LastName, adminId);

                    if (adminName != "!Group Administrator")
                    {
                        var li = new ListItem { Text = adminName, Value = adminId };

                        li.Attributes.Add("class", "ListItemIndentLevel_1");
                        li.Attributes.Add("groupchilditem", "true");

                        var bAdminIsAssigned = IsItemSelected(li.Value);
                        if (bAdminIsAssigned)
                            li.Selected = true;

                        dlAdministrators.Items.Add(li);
                    }
                }
                _iTotalAdministrators++;
            }

            #endregion

            #region Get Client Administrators next

                // Do NOT show client admins since this a group assignment popup. Client admins cannot administer groups!

            #endregion

            // Subtract 1 since we're hiding the system admin
            spanAdministratorCount.InnerHtml = "Available Administrators: " + (_iTotalAdministrators - 1);
        }

        public bool IsItemSelected(string currentAdminId)
        {
            if (Session["Relationships"] == null) return false;
            if (Session["Relationships"].ToString() == "") return false;
            var selectedGroupAdministrators = (List<Relationship>)Session["Relationships"];

            return selectedGroupAdministrators.Any(administrator => administrator.MemberId.ToString().Contains(currentAdminId));
        }
    }
}