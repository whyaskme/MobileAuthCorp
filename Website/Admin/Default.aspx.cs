using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

using MongoDB.Bson;

using MACServices;
using MACSecurity;

using sc = MACServices.Constants.Strings;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using cfghost = MACServices.Constants.WebConfig.HostInfo;

public partial class Default : System.Web.UI.Page
{
    HiddenField _hiddenE;
    HiddenField _hiddenF;
    HiddenField _hiddenL;
    HiddenField _hiddenD;

    HiddenField _hiddenH;
    HiddenField _hiddenI;

    HiddenField _hiddenW;

    TextBox _serviceMessage;

    public static string MyRoles = "";

    public static DateTime startDate = DateTime.UtcNow;
    public static DateTime endDate = DateTime.UtcNow.AddDays(1);
    public static ObjectId ownerId;

    public string adminId = "";
    public string adminFirstName = "";
    public string adminLastName = "";
    public string adminIsReadOnly = "";

    public string clientId = "";

    Utils mUtils = new Utils();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Page.Master != null)
        {
            _hiddenE = (HiddenField)Page.Master.FindControl("hiddenE");

            _hiddenF = (HiddenField)Page.Master.FindControl("hiddenF");

            _hiddenL = (HiddenField)Page.Master.FindControl("hiddenL");
            _hiddenD = (HiddenField)Page.Master.FindControl("hiddenD");
            _serviceMessage = (TextBox)Page.Master.FindControl("divServiceResponseMessage");

            _hiddenH = (HiddenField)Page.Master.FindControl("hiddenH");
            _hiddenI = (HiddenField)Page.Master.FindControl("hiddenI");

            _hiddenW = (HiddenField)Master.FindControl("hiddenW");
            _hiddenW.Value = "54fe2090ea6a5700ccbd085d";
        }

        adminId = Security.DecodeAndDecrypt(_hiddenE.Value, Constants.Strings.DefaultClientId).Trim();
        adminIsReadOnly = Security.DecodeAndDecrypt(_hiddenF.Value, adminId);

        adminFirstName = Security.DecodeAndDecrypt(_hiddenH.Value, adminId);
        adminLastName = Security.DecodeAndDecrypt(_hiddenI.Value, adminId);

        clientId = Security.DecodeAndDecrypt(_hiddenD.Value, adminId).Trim();

        updateMessage.Visible = false;

        spanStartDate.InnerHtml = startDate.ToShortDateString();
        spanEndDate.InnerHtml = endDate.ToShortDateString();

        divDateRange.Visible = false;

        if(IsPostBack)
        {
            switch(systemManagementFunctions.Value)
            {
                case "Backup":
                    DBBackup();
                    break;
                case "Reset":
                    // Defined in js
                    break;
                case "Restore":
                    // Defined in js
                    break;
                case "Delete Relationships":
                    DeleteAllRelationships();
                    break;
                case "Delete Test Data":
                    // Defined in js
                    break;
                case "Create Indexes":
                    CreateIndexes();
                    break;
            }
        }

        if(dlClients.SelectedIndex == 0)
            GetClientList();

        // Remove destructive items from management list if not localhost
        var currentHost = Request.ServerVariables[cfghost.RequestVariables.ServerName].ToString();

        switch (currentHost)
        {
            case Constants.WebConfig.HostInfo.Host.Localhost:
                // Do nothing
                break;

            case Constants.WebConfig.HostInfo.Host.LocalhostChris:
                // Do nothing
                break;

            case Constants.WebConfig.HostInfo.Host.LocalhostGeneric:
                // Do nothing
                break;

            case Constants.WebConfig.HostInfo.Host.LocalhostJoe:
                // Do nothing
                break;

            case Constants.WebConfig.HostInfo.Host.LocalhostTerry:
                // Do nothing
                break;

            case Constants.WebConfig.HostInfo.Host.QA:
                // Do nothing
                break;

            case Constants.WebConfig.HostInfo.Host.Test:
                // Do nothing
                break;

            default:
                ListItem li1 = new ListItem();
                li1.Text = "Database Reset";
                li1.Value = "Reset";

                systemManagementFunctions.Items.Remove(li1);

                ListItem li2 = new ListItem();
                li2.Text = "Create Event Stats";
                li2.Value = "Create EventStats";

                systemManagementFunctions.Items.Remove(li2);

                ListItem li3 = new ListItem();
                li3.Text = "Delete Relationships";
                li3.Value = "Delete Relationships";

                systemManagementFunctions.Items.Remove(li3);

                ListItem li4 = new ListItem();
                li4.Text = "Delete Test Data";
                li4.Value = "Delete Test Data";

                systemManagementFunctions.Items.Remove(li4);

                ListItem li5 = new ListItem();
                li5.Text = "Change Environment Settings";
                li5.Value = "Change Environment Settings";

                systemManagementFunctions.Items.Remove(li5);
                break;
        }

        //if(!currentHost.Contains("localhost"))
        //{
        //    ListItem li1 = new ListItem();
        //    li1.Text = "Database Reset";
        //    li1.Value = "Reset";

        //    systemManagementFunctions.Items.Remove(li1);

        //    ListItem li2 = new ListItem();
        //    li2.Text = "Create Event Stats";
        //    li2.Value = "Create EventStats";

        //    systemManagementFunctions.Items.Remove(li2);

        //    ListItem li3 = new ListItem();
        //    li3.Text = "Delete Relationships";
        //    li3.Value = "Delete Relationships";

        //    systemManagementFunctions.Items.Remove(li3);

        //    ListItem li4 = new ListItem();
        //    li4.Text = "Delete Test Data";
        //    li4.Value = "Delete Test Data";

        //    systemManagementFunctions.Items.Remove(li4);

        //    ListItem li5 = new ListItem();
        //    li5.Text = "Change Environment Settings";
        //    li5.Value = "Change Environment Settings";

        //    systemManagementFunctions.Items.Remove(li5);
        //}

        switch (_hiddenL.Value)
        {
            case Constants.Roles.SystemAdministrator:
                break;

            case Constants.Roles.GroupAdministrator:
                break;

            case Constants.Roles.ClientAdministrator:
                break;

            case Constants.Roles.AccountingUser:
                break;

            case Constants.Roles.ClientUser:
                break;

            case Constants.Roles.GroupUser:
                break;

            case Constants.Roles.OperationsUser:
                scroll2.Visible = false;
                updateMessage.Visible = true;
                updateMessage.InnerHtml = "Successful application test and database connectivity.";

                var tokens = "";
                tokens += Constants.TokenKeys.EventGeneratedByName + adminFirstName + " " + adminLastName;

                Event testEvent = new Event();
                testEvent.UserId = ObjectId.Parse(_hiddenE.Value);
                testEvent.Create(Constants.EventLog.System.OperationalTest, tokens);
                break;

            case Constants.Roles.ViewOnlyUser:
                break;

            default:

                break;
        }
        var isAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
        if (isAuthenticated)
        {
            if (string.IsNullOrEmpty(adminIsReadOnly))
                adminIsReadOnly = "true";

            if (Convert.ToBoolean(adminIsReadOnly))
            {
                SystemManagementPanel.Visible = false;
            }
        }        
    }

    public void GetClientList()
    {
        dlClients.Items.Clear();

        var clientList = new MacList("", "Client", "", "_id,Name");
        foreach (var li in clientList.ListItems.Select(item => new ListItem { Text = item.Attributes["Name"].Replace("&quot;", "\"").Trim(), Value = item.Attributes["_id"].Trim() }))
        {
            if (li.Text != "")
            {
                dlClients.Items.Add(li);
            }
        }

        var li0 = new ListItem { Text = @"All Clients (" + (dlClients.Items.Count) + @")", Value = Constants.Strings.DefaultEmptyObjectId };
        dlClients.Items.Insert(0, li0);
    }

    protected void DBBackup()
    {
        var sourceDatabase = ConfigurationManager.AppSettings[cfg.MongoDbName];
        var targetDatabase = "System_Backup_" + sourceDatabase + "_" + DateTime.UtcNow.ToString("MM-dd-yyyy-hh-mm-ss-tt");

        mUtils.CopyDatabaseToBackupDatabase(sourceDatabase, targetDatabase);

        var dbEvent = new Event
        {
            UserId = ObjectId.Parse(adminId),
            ClientId = ObjectId.Parse(clientId),
            EventTypeDesc = Constants.TokenKeys.EventGeneratedByName + "!System Administrator"
                            + Constants.TokenKeys.DatabaseSource + sourceDatabase
                            + Constants.TokenKeys.DatabaseTarget + targetDatabase
        };
        dbEvent.Create(Constants.EventLog.System.DatabaseBackupCopyCompleted, null);

        updateMessage.Visible = true;
        updateMessage.InnerHtml = @"Data successfully Backed up to " + targetDatabase + @"!";
    }

    protected void DeleteAdminRelationships_Click(object sender, EventArgs e)
    {
        mUtils.DeleteAdminRelationshipsFromClientsAndGroups();
        var dbEvent = new Event
        {
            UserId = ObjectId.Parse(adminId),
            ClientId = ObjectId.Parse(clientId),
            EventTypeDesc = Constants.TokenKeys.EventGeneratedByName + "!System Administrator"
        };
        dbEvent.Create(Constants.EventLog.Assignments.AdminRelationshipsRemoved, null);

        updateMessage.Visible = true;
        updateMessage.InnerHtml = @"Successfully deleted all admin relationships!";
    }

    protected void DeleteAllRelationships()
    {
        mUtils.DeleteAllRelationships();
        var dbEvent = new Event
        {
            UserId = ObjectId.Parse(adminId),
            ClientId = ObjectId.Parse(clientId),
            EventTypeDesc = Constants.TokenKeys.EventGeneratedByName + "!System Administrator"
        };
        dbEvent.Create(Constants.EventLog.Assignments.AllRelationshipsRemoved, null);

        updateMessage.Visible = true;
        updateMessage.InnerHtml = @"Successfully deleted all admin relationships!";
    }

    protected void CreateIndexes()
    {
        string[] collectionsToManage = { "Accounts", "Billing", "Client", "EndUser", "Event", "EventStat", "Group", "OasClientList", "Otp", "UserProfile" };

        mUtils.CreateDatabaseIndexes(collectionsToManage);

        var updateMsg = "Successfully created indexes for ";
        foreach (var currentCollection in collectionsToManage)
        {
            updateMsg += currentCollection + ", ";
        }

        updateMessage.Visible = true;
        updateMessage.InnerHtml = updateMsg.Substring(0, updateMsg.Length - 2) + ".";
    }
}