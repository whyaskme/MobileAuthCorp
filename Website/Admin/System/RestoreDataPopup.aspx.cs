using System;
using System.Collections;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.UI;

using MACServices;
using cfg = MACServices.Constants.WebConfig;

using MongoDB.Driver;

namespace Backups
{
    public partial class RestoreDataPopup : Page
    {
        Utils mUtils = new Utils();

        string originalDatabase = "";
        string backupDatabase = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            // Deny access if user request not logged in
            var isAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
            if (!isAuthenticated)
            {
                Response.Write("Hello world!");
                Response.End();
            }

            if (IsPostBack) // Run the restoration process
            {
                originalDatabase = ConfigurationManager.AppSettings[cfg.AppSettingsKeys.MongoDbName].ToString();
                backupDatabase = hiddenSelectedDatabaseBackup.Value;

                mUtils.RestoreDatabaseFromBackupDatabase(backupDatabase);

                ClientScript.RegisterStartupScript(typeof(Page), "closePage", "<script type='text/JavaScript'>finishRestoreCleanup('" + backupDatabase + "');</script>");
            }
            else // Show available directories to restore from
            {
                var sbResponse = new StringBuilder();
                var backupCount = 0;

                var dbName = ConfigurationManager.AppSettings[cfg.AppSettingsKeys.MongoDbName];

                var mongoClient = new MongoClient(ConfigurationManager.ConnectionStrings[cfg.ConnectionStringKeys.MongoServer].ConnectionString);
                var mongoServer = mongoClient.GetServer();
                var databaseCollection = mongoServer.GetDatabaseNames();

                var databaseNames = new ArrayList();

                foreach (var currentDbName in databaseCollection)
                {
                    if (currentDbName.Contains("System_Backup"))
                        databaseNames.Add(currentDbName);
                }

                databaseNames.Reverse();

                foreach (var currentDbName in databaseNames)
                {
                    var tmpDbData = currentDbName.ToString().Split('_');
                    var tmpDbName = tmpDbData[tmpDbData.Length - 1].Split('-');

                    var backupDate = tmpDbName[0] + "/" + tmpDbName[1] + "/" + tmpDbName[2];
                    var backupTime = tmpDbName[3] + ":" + tmpDbName[4] + ":" + tmpDbName[5];
                    var backupButtonLabel = backupDate + " - " + backupTime + " " + tmpDbName[6];

                    var backupDatabaseName = "";

                    // Compare the bu db name with the current connected db
                    for (var i = 2; i < tmpDbData.Length - 1; i++)
                    {
                        backupDatabaseName += tmpDbData[i] + "_";
                    }
                    backupDatabaseName = backupDatabaseName.Substring(0, backupDatabaseName.Length - 1);

                    if (backupDatabaseName == dbName)
                    {
                        var sTitle = "";

                        backupCount++;

                        var currentDatabase = mongoServer.GetDatabase(currentDbName.ToString());
                        var collectionNames = currentDatabase.GetCollectionNames();

                        foreach(var currentCollection in collectionNames)
                        {
                            var itemCount = currentDatabase.GetCollection(currentCollection).Count();
                            sTitle += "" + currentCollection + " (" + mUtils.FormatNumber(itemCount.ToString()) + ") : ";
                        }
                        sTitle = sTitle.Substring(0, sTitle.Length - 2);

                        sbResponse.Append("<input id='" + currentDbName + "' title='" + sTitle + "' onclick='javascript: setDirectoryToRestore(this);' type='button' style='margin-bottom: 0.125rem;width: 100%;padding: 0.25rem 0;' class='tiny button radius' value='" + backupButtonLabel + "' />");
                    }
                }

                divBackupDirectoryContainer.InnerHtml = sbResponse.ToString();
                spanBackups.InnerHtml = "Restore System Data: <span style='white-space: nowrap;'>(" + backupCount + ") Backups Available</span>";
            }
        }
    }
}