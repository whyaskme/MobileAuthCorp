using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

using MACServices;
using MACSecurity;

using MongoDB.Bson;
using MongoDB.Driver;

public partial class UsersPopup : Page
{
    public MongoClient MyMongoClient;
    public MongoServer MyMongoServer;
    //private MongoDatabase MyMongoDatabase;

    public HelpTopic myTopic;

    protected void Page_Load(object sender, EventArgs e)
    {
        // Deny access if user request not logged in
        var isAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
        if (!isAuthenticated)
        {
            Response.Write("Hello world!");
            Response.End();
        }

        var loggedInuserId = "";
        if (!String.IsNullOrEmpty(Request["loggedInuserId"]))
            loggedInuserId = Request["loggedInuserId"];

        var topicId = "";
        if (!String.IsNullOrEmpty(Request["topicId"]))
            topicId = Request["topicId"];

        myTopic = new HelpTopic(topicId);

        spanTopicName.InnerHtml = "Grant access to " + myTopic.Category + " help topic";

        if (myTopic.SubCategory != "")
            spanTopicName.InnerHtml = "Grant access to " + myTopic.SubCategory + " help topic";

        if (IsPostBack)
        {
            foreach(ListItem li in dlUserList.Items)
            {
                if(li.Selected)
                {
                    hiddenSelectedUserIds.Value += li.Value + "|";
                }
            }

            // Compare objects to determine values that were changed
            //Utils utility = new Utils();
            //var differences = utility.GetObjectDifferences(originalProvider, updatedProvider);

            //// Log the changes
            //var providerEvent = new Event
            //{
            //    ClientId = myClient._id,
            //    UserId = ObjectId.Parse(loggedInAdminId),
            //    EventTypeDesc = Constants.TokenKeys.ProviderName + updatedProvider.Name
            //                    + Constants.TokenKeys.ProviderType + "Email"
            //                    + Constants.TokenKeys.ProviderChangedValues + differences
            //                    + Constants.TokenKeys.ClientName + myClient.Name
            //};

            //providerEvent.Create(Constants.EventLog.Providers.Updated, null);

            ClientScript.RegisterStartupScript(typeof(Page), "closePage", "<script type='text/JavaScript'>callParentDocumentFunction();</script>");
        }
        else
        {
            // Fetch all registered users
            var userProfileCollection = new MacListAdHoc("UserProfile", "_t", "UserProfile", false, "");

            // Here we need to process the xml response into a List collection
            var xmlSystemUsersDoc = userProfileCollection.ListXml;
            var xmlSystemUsers = xmlSystemUsersDoc.GetElementsByTagName("userprofile");

            foreach (XmlNode currentUser in xmlSystemUsers)
            {
                if (currentUser.Attributes != null)
                {
                    var userId = currentUser.Attributes["id"].Value;

                    var userProfile = new UserProfile(userId);
                    var userName = Security.DecodeAndDecrypt(userProfile.FirstName, userId) + " " + Security.DecodeAndDecrypt(userProfile.LastName, userId);

                    var li = new ListItem { Text = userName, Value = userId };

                    var hasAccess = myTopic.Relationships.Find(FindRelationshipById(li.Value));
                    if (hasAccess != null)
                        li.Selected = true;

                    dlUserList.Items.Add(li);
                }
            }
        }
    }

    static Predicate<Relationship> FindRelationshipById(string currentAdminId)
    {
        return provider => provider.MemberId == ObjectId.Parse(currentAdminId);
    }
}