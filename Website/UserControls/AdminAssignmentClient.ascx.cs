using System;
using System.Collections.Generic;
using System.Globalization;
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
    public partial class AdminAssignmentClient : UserControl
    {
        public int TotalAdministrators = 0;
        public Event MyEvent;

        public ObjectId ClientId = ObjectId.GenerateNewId();
        public int SelectedAdminCount = 0;
        public int GroupsAssigned = 0;

        public string loggedInAdminId = "";

        public Client MyClient;

        Utils mUtils = new Utils();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Request["clientId"] != null)
                    if (Request["clientId"].ToString(CultureInfo.CurrentCulture) != "")
                        ClientId = ObjectId.Parse(Request["clientId"]);

                if (Request["groupsassigned"] != null)
                    if (Request["groupsassigned"].ToString(CultureInfo.CurrentCulture) != "")
                        GroupsAssigned = Convert.ToInt16(Request["groupsassigned"].ToString(CultureInfo.CurrentCulture));

                if (Request["loggedInAdminId"] != null)
                    if (Request["loggedInAdminId"] != "")
                        loggedInAdminId = Request["loggedInAdminId"];

                loggedInAdminId = MACSecurity.Security.DecodeAndDecrypt(loggedInAdminId.ToString(), Constants.Strings.DefaultClientId);

                var userIsReadOnly = "";
                if (!String.IsNullOrEmpty(Request["userisreadonly"]))
                    userIsReadOnly = Request["userisreadonly"];

                userIsReadOnly = MACSecurity.Security.DecodeAndDecrypt(userIsReadOnly, loggedInAdminId);

                MyClient = new Client(ClientId.ToString());

                if (IsPostBack)
                {
                    var relationships = new List<Relationship>();

                    if (Session["Relationships"].ToString() != "")
                        relationships = (List<Relationship>)Session["Relationships"];

                    for (var i = 0; i < dlAdministrators.Items.Count; i++)
                    {
                        var currentAdministrator = dlAdministrators.Items[i];

                        var createRelationship = false;
                        if (currentAdministrator.Selected)
                        {
                            SelectedAdminCount++;
                            createRelationship = true;
                        }

                        // Handle the relationships
                        if (currentAdministrator.Value.Trim() == Constants.Strings.DefaultAdminId ||
                            currentAdministrator.Value.Trim() == Constants.Strings.DefaultEmptyObjectId ||
                            currentAdministrator.Value.Trim() == Constants.Strings.DefaultStaticObjectId) continue;
                        var userProfile = new UserProfile(currentAdministrator.Value);

                        mUtils.ManageObjectRelationships_AdminAndClient(ObjectId.Parse(loggedInAdminId), createRelationship, userProfile, MyClient);
                    }

                    MyClient.Update();
                    GetAdministratorList();

                    assignAdministratorsMessage.Visible = true;

                    //ScriptManager.RegisterClientScriptBlock(Page, typeof(Page), "Script", "javascript: callParentMessageUpdate();", true); 
                }
                else
                {
                    assignAdministratorsMessage.Visible = false;
                    GetAdministratorList();
                }

                if (Convert.ToBoolean(userIsReadOnly))
                {
                    btnSaveAdministrator.Visible = false;
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
            systemAdmin.Attributes.Add("title", "Click here to create a new System Administrator");
            systemAdmin.Attributes.Add("onclick", "javascript: navigateCreateNewAdmin('System');");
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

                        var isAdmin = MyClient.Relationships.Find(FindRelationshipById(li.Value));
                        if (isAdmin != null)
                            li.Selected = true;

                        dlAdministrators.Items.Add(li);
                    }
                }
                TotalAdministrators++;
            }

            #endregion

            #region Get Group Administrators next

            var groupAdminList = new MacListAdHoc("UserProfile", "Roles", Constants.Strings.UserRoles.GroupAdmin.Item3, false, "");

            // Here we need to process the xml response into a List collection
            var xmlGroupAdminDoc = groupAdminList.ListXml;
            var xmlGroupAdmins = xmlGroupAdminDoc.GetElementsByTagName("userprofile");

            var groupAdmin = new ListItem {Text = @"Group Administrators", Value = Constants.Strings.DefaultEmptyObjectId};
            groupAdmin.Attributes.Add("class", "ListItemIndentLevel_0");
            groupAdmin.Attributes.Add("title", "Click here to create a new Group Administrator");
            groupAdmin.Attributes.Add("onclick", "javascript: navigateCreateNewAdmin('Group');");
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

                        var isAdmin = MyClient.Relationships.Find(FindRelationshipById(li.Value));
                        if (isAdmin != null)
                            li.Selected = true;

                        dlAdministrators.Items.Add(li);
                    }
                }
                TotalAdministrators++;
            }

            #endregion

            #region Get Client Administrators next

            var clientAdmin = new ListItem {Text = @"Client Administrators", Value = Constants.Strings.DefaultEmptyObjectId};
            clientAdmin.Attributes.Add("class", "ListItemIndentLevel_0");
            clientAdmin.Attributes.Add("title", "Click here to create a new Client Administrator");
            clientAdmin.Attributes.Add("onclick", "javascript: navigateCreateNewAdmin('Client');");
            dlAdministrators.Items.Add(clientAdmin);

            var adminList = new MacListAdHoc("UserProfile", "Roles", Constants.Strings.UserRoles.ClientAdmin.Item3, false, "");

            // Here we need to process the xml response into a List collection
            var xmlClientAdminDoc = adminList.ListXml;
            var xmlClientAdmins = xmlClientAdminDoc.GetElementsByTagName("userprofile");

            foreach (XmlNode currentAdmin in xmlClientAdmins)
            {
                if (currentAdmin.Attributes != null)
                {
                    var adminId = currentAdmin.Attributes["id"].Value;

                    // Get the admin's name
                    var userProfile = new UserProfile(adminId);

                    var adminName = Security.DecodeAndDecrypt(userProfile.FirstName, adminId) + " " + Security.DecodeAndDecrypt(userProfile.LastName, adminId);

                    if (adminName != "!Client Administrator")
                    {
                        var li = new ListItem { Text = adminName, Value = adminId };

                        li.Attributes.Add("class", "ListItemIndentLevel_1");

                        var isAdmin = MyClient.Relationships.Find(FindRelationshipById(li.Value));
                        if (isAdmin != null)
                            li.Selected = true;

                        dlAdministrators.Items.Add(li);
                    }
                }
                TotalAdministrators++;
            }

            #endregion

            // Subtract 1 since we're hiding the system admin
            spanAdministratorCount.InnerHtml = (TotalAdministrators-1) + " Administrators Available";
        }

        public bool IsItemSelected(string currentAdminId)
        {
            foreach(var currentRelationship in MyClient.Relationships)
            {
                if(currentRelationship.MemberId.ToString() == currentAdminId.Trim())
                    return true;
                else
                    return false;
            }
            return false;
        }

        static Predicate<Relationship> FindRelationshipById(string currentAdminId)
        {
            return provider => provider.MemberId == ObjectId.Parse(currentAdminId);
        }
    }
}