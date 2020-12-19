using System;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.UI;

using MACServices;
using MACSecurity;

using MongoDB.Bson;
using MongoDB.Driver;

public partial class MACAdmin_System_DBDropCollections : Page
{
    Utils mUtils = new Utils();

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

        var defaultClientId = Constants.Strings.DefaultClientId;

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
            string currentOperationalDatabase =
                ConfigurationManager.AppSettings[Constants.WebConfig.AppSettingsKeys.MongoDbName];
            var targetDatabase = "System_Backup_" + currentOperationalDatabase + "_" +
                                 DateTime.Now.ToString("MM-dd-yyyy-hh-mm-ss-tt");
            mUtils.CopyDatabaseToBackupDatabase(currentOperationalDatabase, targetDatabase);

            // Process the collections to be dropped
            var collectionsToDrop = hiddenItemsToDropIds.Value.Split(char.Parse(Constants.Common.ItemSep));

            foreach (var currentItem in collectionsToDrop)
            {
                if (currentItem != "")
                {
                    var collectionName = currentItem.Replace("chk_", "");

                    mongoDBConnectionPool.DropCollection(collectionName);

                    var dbEvent = new Event
                    {
                        UserId = ObjectId.Parse(loggedInAdminId),
                        ClientId = ObjectId.Parse(defaultClientId)
                    };

                    var replacementTokens = Constants.TokenKeys.EventGeneratedByName + loggedInAdminName;
                    replacementTokens += Constants.TokenKeys.DatabaseCollectionName + collectionName;

                    dbEvent.Create(Constants.EventLog.System.DatabaseCollectionDropped, replacementTokens);
                }
            }

            mUtils.ResetSystemData(loggedInAdminId, loggedInAdminName, loggedInAdminName);

            //ShowDBCollections();

            ClientScript.RegisterStartupScript(typeof (Page), "closePage",
                "<script type='text/JavaScript'>callParentDocumentFunction();</script>");
        }
        else
        {
            ShowDBCollections();
        }
    }

    public void ShowDBCollections()
    {
        var mongoDBConnectionPool = (MongoDatabase)HttpContext.Current.Application[Constants.Strings.MongoDB];
        var collectionCounter = 0;

        var sbMandatoryDrops = new StringBuilder();
        sbMandatoryDrops.Append("The collections: ");

        var sbCollections = new StringBuilder();
        var dbCollections = mongoDBConnectionPool.GetCollectionNames();

        foreach (var currentCollection in dbCollections)
        {
            switch (currentCollection)
            {
                case "Client":
                    sbMandatoryDrops.Append(currentCollection + ", ");
                    break;

                case "Event":
                    sbMandatoryDrops.Append(currentCollection + ", ");
                    break;

                case "Group":
                    sbMandatoryDrops.Append(currentCollection + ", ");
                    break;

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
                    sbMandatoryDrops.Append(currentCollection + ", ");
                    break;

                case "UserProfile":
                    sbMandatoryDrops.Append(currentCollection + ", ");
                    break;

                default:
                    collectionCounter++;

                    sbCollections.Append("<div id='collection_" + currentCollection + "' style='width: 150px; float: left; border: solid 0 #ffff00; margin-right: 15px;'>");

                    sbCollections.Append("<div style='float: left; border: solid 0px #00ff00; width: auto; margin-right: 0px;'>");

                    sbCollections.Append("<input id='chk_" + currentCollection + "' type='checkbox' onchange='javascript: setCollectionIdsToDrop(this);' style='position: relative; top: 0px;' />");
                    sbCollections.Append("</div>");

                    sbCollections.Append("<div style='float: left; border: solid 0px #0000ff; width: auto; padding-left: 3px; font-size: 12px;'>");
                    sbCollections.Append(collectionCounter + ") " + currentCollection);

                    var itemCount = mongoDBConnectionPool.GetCollection(currentCollection).Count();
                    sbCollections.Append(" (" + mUtils.FormatNumber(itemCount.ToString()) + ")");

                    sbCollections.Append("</div>");

                    sbCollections.Append("</div>");
                    break;
            }
        }

        divMandatoryDrops.InnerHtml = sbMandatoryDrops.ToString().Substring(0, sbMandatoryDrops.ToString().Length - 2);
        divMandatoryDrops.InnerHtml += " are automatically deleted.";

        divDBCollectionsToDrop.InnerHtml = sbCollections.ToString();
    }
}