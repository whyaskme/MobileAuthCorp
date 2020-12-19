using System;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.UI;

using MACServices;
using MACSecurity;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

public partial class MACAdmin_System_DeleteTestData : Page
{
    readonly Utils mUtils = new Utils();

    protected void Page_Load(object sender, EventArgs e)
    {
        // Deny access if user request not logged in
        var isAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
        if (!isAuthenticated)
        {
            Response.Write("Hello world!");
            Response.End();
        }

        var mongoDBConnectionPool = (MongoDatabase)HttpContext.Current.Application[Constants.Strings.MongoDB];

        var loggedInAdminId = Constants.Strings.DefaultEmptyObjectId;
        var loggedInAdminName = "";

        if (Request["userId"] != null)
        {
            loggedInAdminId = Request["userId"].ToString();

            loggedInAdminId = Security.DecodeAndDecrypt(loggedInAdminId, Constants.Strings.DefaultClientId);

            UserProfile adminProfile = new UserProfile(loggedInAdminId);

            var firstName = Security.DecodeAndDecrypt(adminProfile.FirstName, loggedInAdminId);
            var lastName = Security.DecodeAndDecrypt(adminProfile.LastName, loggedInAdminId);
            loggedInAdminName = firstName + " " + lastName;
        }

        if (IsPostBack)
        {
            // Backup the current database before resetting system
            if (chkBackUpDB.Checked)
            {
                string currentOperationalDatabase = ConfigurationManager.AppSettings[Constants.WebConfig.AppSettingsKeys.MongoDbName];
                var targetDatabase = "System_Backup_" + currentOperationalDatabase + "_" + DateTime.Now.ToString("MM-dd-yyyy-hh-mm-ss-tt");
                mUtils.CopyDatabaseToBackupDatabase(currentOperationalDatabase, targetDatabase);
            }

            // Process the collections to be dropped
            var updateMsg = "Successfully deleted test data and created indexes for ";

            var collectionsToDrop = hiddenItemsToDropIds.Value.Split(char.Parse(Constants.Common.ItemSep));
            foreach (var currentItem in collectionsToDrop)
            {
                var collectionNameSuffix = "";

                if (currentItem != "")
                {
                    var collectionName = currentItem.Replace("chk_", "");

                    switch (collectionName)
                    {
                        case "Accounts_Merchant": // Delete all Merchant accounts
                            collectionName = "Accounts";
                            collectionNameSuffix = " - Merchant";
                            updateMsg += "Merchant Accounts, ";
                            var merchantQuery = Query.EQ("Type", "Merchant");
                            mongoDBConnectionPool.GetCollection("Accounts").Remove(merchantQuery);
                            break;

                        case "Accounts_User": // Delete all user accounts
                            collectionName = "Accounts";
                            collectionNameSuffix = "  - User";
                            updateMsg += "User Accounts, ";
                            var userQuery = Query.EQ("Type", "User");
                            mongoDBConnectionPool.GetCollection("Accounts").Remove(userQuery);
                            break;

                        case "Accounts_Utility": // Delete all utility accounts
                            collectionName = "Accounts";
                            collectionNameSuffix = " - Utility";
                            updateMsg += "Utility Accounts, ";
                            var utilityQuery = Query.EQ("Type", "Utility");
                            mongoDBConnectionPool.GetCollection("Accounts").Remove(utilityQuery);
                            break;

                        default: // Else, drop the actual collection
                            updateMsg += collectionName + ", ";
                            mongoDBConnectionPool.DropCollection(collectionName);

                            // Reset error stats for all clients
                            if (currentItem.Replace("chk_", "") == "Event")
                            {
                                var eventStatCollection = mongoDBConnectionPool.GetCollection("EventStat");

                                var eventStatList = eventStatCollection.FindAllAs<EventStat>();
                                foreach (var currentEventStat in eventStatList)
                                {
                                    // Loop through all event days and reset error stats
                                    foreach (EventStatDay currentDay in currentEventStat.DailyStats)
                                    {
                                        currentDay.Exceptions = 0;
                                    }
                                    currentEventStat.Update();
                                }
                            }
                            break;
                    }

                    string[] collectionsToManage = { collectionName };
                    mUtils.CreateDatabaseIndexes(collectionsToManage);

                    var dbEvent = new Event
                    {
                        UserId = ObjectId.Parse(loggedInAdminId),
                        ClientId = ObjectId.Parse(Constants.Strings.DefaultClientId)
                    };

                    var replacementTokens = Constants.TokenKeys.EventGeneratedByName + loggedInAdminName;
                    replacementTokens += Constants.TokenKeys.DatabaseCollectionName + collectionName + collectionNameSuffix;

                    dbEvent.Create(Constants.EventLog.System.DatabaseCollectionDropped, replacementTokens);
                }
            }

            updateMsg = updateMsg.Substring(0, updateMsg.Length - 2) + ".";

            ClientScript.RegisterStartupScript(typeof(Page), "closePage", "<script type='text/JavaScript'>callParentDocumentFunction('" + updateMsg + "');</script>");
        }
        else
        {
            ShowDBCollections();
        }
    }

    public void ShowDBCollections()
    {
// ReSharper disable once NotAccessedVariable
        var collectionCounter = 0;

        var sbCollections = new StringBuilder();
        var mongoDBConnectionPool = (MongoDatabase)HttpContext.Current.Application[Constants.Strings.MongoDB];
        var dbCollections = mongoDBConnectionPool.GetCollectionNames();

        foreach (var currentCollection in dbCollections)
        {
            int itemCount;
            switch (currentCollection)
            {
                case "Accounts":
                    // Merchant
                    sbCollections.Append("<div id='collection_" + currentCollection + "' style='width: 150px; float: left; border: solid 0 #ffff00; margin-right: 15px; margin-bottom: 5px;'>");
                    sbCollections.Append("  <div style='float: left; border: solid 0px #00ff00; width: auto;'>");
                    sbCollections.Append("      <input id='chk_" + currentCollection + "_Merchant' type='checkbox' onclick='javascript: setCollectionIdsToDrop(this);' style='position: relative; top: 0px; margin-right: 3px;' />");
                    sbCollections.Append("  </div>");

                    sbCollections.Append("  <div style='font-size: 12px; float: left; border: solid 0px #0000ff; width: auto; padding-left: 3px;'>");
                    sbCollections.Append("      Merch Acct");

                    var merchantCountQuery = Query.EQ("Type", "Merchant");
                    itemCount = Convert.ToInt32(mongoDBConnectionPool.GetCollection("Accounts").Find(merchantCountQuery).Count());

                    if (itemCount > 0)
                        sbCollections.Append(" (" + mUtils.FormatNumber(itemCount.ToString()) + ")");
                    else
                        sbCollections.Append(" (" + itemCount.ToString() + ")");

                    sbCollections.Append("  </div>");
                    sbCollections.Append("</div>");

                    // User
                    sbCollections.Append("<div id='collection_" + currentCollection + "' style='width: 150px; float: left; border: solid 0 #ffff00; margin-right: 15px; margin-bottom: 5px;'>");
                    sbCollections.Append("  <div style='float: left; border: solid 0px #00ff00; width: auto;'>");
                    sbCollections.Append("      <input id='chk_" + currentCollection + "_User' type='checkbox' onclick='javascript: setCollectionIdsToDrop(this);' style='position: relative; top: 0px; margin-right: 3px;' />");
                    sbCollections.Append("  </div>");

                    sbCollections.Append("  <div style='font-size: 12px; float: left; border: solid 0px #0000ff; width: auto; padding-left: 3px;'>");
                    sbCollections.Append("      User Acct");

                    var userCountQuery = Query.EQ("Type", "User");
                    itemCount = Convert.ToInt32(mongoDBConnectionPool.GetCollection("Accounts").Find(userCountQuery).Count());

                    if (itemCount > 0)
                        sbCollections.Append(" (" + mUtils.FormatNumber(itemCount.ToString()) + ")");
                    else
                        sbCollections.Append(" (" + itemCount.ToString() + ")");

                    sbCollections.Append("  </div>");
                    sbCollections.Append("</div>");

                    // Utility
                    sbCollections.Append("<div id='collection_" + currentCollection + "' style='width: 150px; float: left; border: solid 0 #ffff00; margin-right: 15px; margin-bottom: 5px;'>");
                    sbCollections.Append("  <div style='float: left; border: solid 0px #00ff00; width: auto;'>");
                    sbCollections.Append("      <input id='chk_" + currentCollection + "_Utility' type='checkbox' onclick='javascript: setCollectionIdsToDrop(this);' style='position: relative; top: 0px; margin-right: 3px;' />");
                    sbCollections.Append("  </div>");

                    sbCollections.Append("  <div style='font-size: 12px; float: left; border: solid 0px #0000ff; width: auto; padding-left: 3px;'>");
                    sbCollections.Append("      Utility Acct");

                    var utilityCountQuery = Query.EQ("Type", "Utility");
                    itemCount = Convert.ToInt32(mongoDBConnectionPool.GetCollection("Accounts").Find(utilityCountQuery).Count());

                    if (itemCount > 0)
                        sbCollections.Append(" (" + mUtils.FormatNumber(itemCount.ToString()) + ")");
                    else
                        sbCollections.Append(" (" + itemCount.ToString() + ")");

                    sbCollections.Append("  </div>");
                    sbCollections.Append("</div>");
                    break;

                case "Client":
                    // Do nothing
                    break;

                case "Group":
                    // Do nothing
                    break;

                //case "Billing":
                //    // Do nothing
                //    break;

                case "system.indexes":
                    // Do nothing
                    break;

                case "system.users":
                    // Do nothing
                    break;

                case "system.js":
                    // Do nothing
                    break;

                case "Roles":
                    // Do nothing
                    break;

                case "TypeDefinitions":
                    // Do nothing
                    break;

                case "Users":
                    // Do nothing
                    break;

                case "UsersInRoles":
                    // Do nothing
                    break;

                case "UserProfile":
                    // Do nothing
                    break;

                default:
                    collectionCounter++;

                    sbCollections.Append("<div id='collection_" + currentCollection + "' style='width: 150px; float: left; border: solid 0 #ffff00; margin-right: 15px; margin-bottom: 5px;'>");

                    sbCollections.Append("  <div style='float: left; border: solid 0px #00ff00; width: auto;'>");

                    sbCollections.Append("      <input id='chk_" + currentCollection + "' type='checkbox' onclick='javascript: setCollectionIdsToDrop(this);' style='position: relative; top: 0px; margin-right: 3px;' />");
                    sbCollections.Append("  </div>");

                    sbCollections.Append("  <div style='font-size: 12px; float: left; border: solid 0px #0000ff; width: auto; padding-left: 3px;'>");
                    sbCollections.Append(currentCollection);

                    itemCount = Convert.ToInt32(mongoDBConnectionPool.GetCollection(currentCollection).Count());

                    if (itemCount > 0)
                        sbCollections.Append(" (" + mUtils.FormatNumber(itemCount.ToString()) + ")");
                    else
                        sbCollections.Append(" (" + itemCount.ToString() + ")");

                    sbCollections.Append("  </div>");

                    sbCollections.Append("</div>");
                    break;
            }
        }

        divDBCollectionsToDrop.InnerHtml = sbCollections.ToString();
    }
}