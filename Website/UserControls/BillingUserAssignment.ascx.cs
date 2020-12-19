using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

using MACBilling;
using MACSecurity;
using MACServices;


using dk = MACServices.Constants.Dictionary.Keys;

namespace UserControls
{
    public partial class BillingUserAssignment : UserControl
    {
        public int TotalAdministrators = 0;
        public int TotalUsers = 0;

        public Event MyEvent;

        public ObjectId OwnerId = ObjectId.GenerateNewId();
        public string OwnerType = "";

        public int SelectedAdminCount = 0;
        public int GroupsAssigned = 0;

        public ObjectId LoggedInAdminId = ObjectId.GenerateNewId();

        public BillConfig myBillConfig;

        public Utils mUtils = new Utils();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Request["ownerId"] != null)
                    if (Request["ownerId"].ToString(CultureInfo.CurrentCulture) != "")
                        OwnerId = ObjectId.Parse(Request["ownerId"]);

                if (Request["ownerType"] != null)
                    if (Request["ownerType"].ToString(CultureInfo.CurrentCulture) != "")
                        OwnerType = Request["ownerType"];

                if (Request["groupsassigned"] != null)
                    if (Request["groupsassigned"].ToString(CultureInfo.CurrentCulture) != "")
                        GroupsAssigned = Convert.ToInt16(Request["groupsassigned"].ToString(CultureInfo.CurrentCulture));

                if (Request["loggedInAdminId"] != null)
                    if (Request["loggedInAdminId"] != "")
                        LoggedInAdminId = ObjectId.Parse(Request["loggedInAdminId"]);

                myBillConfig = new BillConfig(OwnerId.ToString(), OwnerType, LoggedInAdminId.ToString());

                if (IsPostBack)
                {
                    myBillConfig.NotifyUserIds = "";

                    // Read the selected users
                    foreach (ListItem currentItem in dlAdministrators.Items)
                    {
                        if (currentItem.Selected)
                            myBillConfig.NotifyUserIds += currentItem.Value + "|";
                    }
                    myBillConfig.UpdateConfig(myBillConfig, myBillConfig.OwnerId.ToString());

                    GetUserList();

                    assignAdministratorsMessage.Visible = true;
                }
                else
                {
                    assignAdministratorsMessage.Visible = false;
                    GetUserList();
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

        public void GetUserList()
        {
            var i = 0;

            dlAdministrators.Items.Clear();

            #region Get Client Administrators next

                var clientAdmins = mUtils.GetOwnerUsers(OwnerId.ToString(), OwnerType, "Administrator");
                foreach (string currentUser in clientAdmins)
                {
                    if(i == 0)
                    {
                        var clientAdmin = new ListItem { Text = OwnerType + " Administrators", Value = Constants.Strings.DefaultEmptyObjectId };
                        clientAdmin.Attributes.Add("class", "ListItemIndentLevel_0");
                        dlAdministrators.Items.Add(clientAdmin);
                    }

                    var tmpVal = currentUser.Split('|');
                    var userName = tmpVal[0];
                    var userId = tmpVal[1];

                    var li = new ListItem { Text = userName, Value = userId };

                    if (IsUserSelected(userId))
                        li.Selected = true;

                    li.Attributes.Add("class", "ListItemIndentLevel_1");

                    dlAdministrators.Items.Add(li);
                    TotalAdministrators++;
                    i++;
                }

            #endregion

            #region Get Client Users next

                i = 0;

                var clientUsers = mUtils.GetOwnerUsers(OwnerId.ToString(), OwnerType, "User");
                foreach (string currentUser in clientUsers)
                {
                    if (i == 0)
                    {
                        var clientUser = new ListItem { Text = OwnerType + " Users", Value = Constants.Strings.DefaultEmptyObjectId };
                        clientUser.Attributes.Add("class", "ListItemIndentLevel_0");
                        dlAdministrators.Items.Add(clientUser);
                    }

                    var tmpVal = currentUser.Split('|');
                    var userName = tmpVal[0];
                    var userId = tmpVal[1];

                    var li = new ListItem { Text = userName, Value = userId };

                    if (IsUserSelected(userId))
                        li.Selected = true;

                    li.Attributes.Add("class", "ListItemIndentLevel_1");

                    dlAdministrators.Items.Add(li);
                    TotalUsers++;
                    i++;
                }

            #endregion

            spanAdministratorCount.InnerHtml = (TotalAdministrators) + " Admins and " + TotalUsers + " Users Available";
        }

        public bool IsUserSelected(string currentUserId)
        {
            var userIsSelected = false;

            if(!string.IsNullOrEmpty(myBillConfig.NotifyUserIds))
            {
                if(myBillConfig.NotifyUserIds.Contains("|"))
                {
                    var tmpVal = myBillConfig.NotifyUserIds.Split('|');
                    foreach(string currentUser in tmpVal)
                    {
                        if (currentUser == currentUserId)
                            return true;
                    }
                }
            }
            return userIsSelected;
        }

        static Predicate<Relationship> FindRelationshipById(string currentAdminId)
        {
            return provider => provider.MemberId == ObjectId.Parse(currentAdminId);
        }
    }
}