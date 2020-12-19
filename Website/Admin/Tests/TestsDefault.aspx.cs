using System;
using System.Configuration;
using System.Web.UI.WebControls;

using MongoDB.Bson;
using MACServices;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;

namespace MACUserApps.Web.Tests
{
    public partial class MacUserAppsWebTestsDefault : System.Web.UI.Page
    {
        HiddenField _hiddenE;
        HiddenField _hiddenL;
        HiddenField _hiddenD;

        HiddenField _hiddenW;

        TextBox _serviceMessage;

        public static string MyRoles = "";
        Utils mUtils = new Utils();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.Master != null)
            {
                _hiddenE = (HiddenField)Page.Master.FindControl("hiddenE");
                _hiddenL = (HiddenField)Page.Master.FindControl("hiddenL");
                _hiddenD = (HiddenField)Page.Master.FindControl("hiddenD");
                _serviceMessage = (TextBox)Page.Master.FindControl("divServiceResponseMessage");

                _hiddenW = (HiddenField)Master.FindControl("hiddenW");
                _hiddenW.Value = "5490c024ead63627d88e3e25";
            }
        }
        
        protected void btnDBBackup_Click(object sender, EventArgs e)
        {
            var sourceDatabase = ConfigurationManager.AppSettings[cfg.MongoDbName];
            var targetDatabase = "System_Backup_" + sourceDatabase + "_" + DateTime.UtcNow.ToString("MM-dd-yyyy-hh-mm-ss-tt");

            mUtils.CopyDatabaseToBackupDatabase(sourceDatabase, targetDatabase);

            var dbEvent = new Event();
            dbEvent.UserId = ObjectId.Parse(_hiddenE.Value);
            dbEvent.ClientId = ObjectId.Parse(_hiddenD.Value);

            var tokens = "";
            tokens += Constants.TokenKeys.EventGeneratedByName + "!System Administrator";
            tokens += Constants.TokenKeys.DatabaseSource + sourceDatabase;
            tokens += Constants.TokenKeys.DatabaseTarget + targetDatabase;

            dbEvent.Create(Constants.EventLog.System.DatabaseBackupCopyCompleted, tokens);

            _serviceMessage.Text = @"Data successfully Backed up to " + targetDatabase + @"!";
        }

        protected void btnDeleteAdminRelationships_Click(object sender, EventArgs e)
        {
            mUtils.DeleteAdminRelationshipsFromClientsAndGroups();

            var dbEvent = new Event();
            dbEvent.UserId = ObjectId.Parse(_hiddenE.Value);
            dbEvent.ClientId = ObjectId.Parse(_hiddenD.Value);

            var tokens = "";
            tokens += Constants.TokenKeys.EventGeneratedByName + "!System Administrator";

            dbEvent.Create(Constants.EventLog.Assignments.AdminRelationshipsRemoved, tokens);

            _serviceMessage.Text = @"Successfully deleted all admin relationships!";
        }

        public void ResetSessionObjects()
        {
            Session["MessageProviders"] = new MessageProvider();
            Session["Relationships"] = "";
            Session["ClientAdministrators"] = "";
            Session["ClientXML"] = "";
        }
    }
}