using System;
using System.Globalization;
using System.Web;
using System.Web.UI;
using MACServices;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Admin.Providers.Messaging.Email
{
    public partial class ConfigPopup : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Deny access if user request not logged in
            var isAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
            if (!isAuthenticated)
            {
                Response.Write("Hello world!");
                Response.End();
            }

            var loggedInAdminId = "";
            if (!String.IsNullOrEmpty(HttpContext.Current.Request["loggedInAdminId"]))
                loggedInAdminId = HttpContext.Current.Request["loggedInAdminId"];

            var providerId = ObjectId.GenerateNewId();
            if (!String.IsNullOrEmpty(HttpContext.Current.Request["providerID"]))
                providerId = ObjectId.Parse(HttpContext.Current.Request["providerID"]);

            hiddenProviderID.Value = providerId.ToString();

            var clientId = "";
            if (!String.IsNullOrEmpty(HttpContext.Current.Request["clientId"]))
                clientId = HttpContext.Current.Request["clientId"];

            var myClient = new Client(clientId);

            var originalProvider = myClient.MessageProviders.EmailProviders.Find(FindProviderById(hiddenProviderID.Value));
            var updatedProvider = new ProviderEmail();

            if (IsPostBack)
            {
                if (originalProvider != null)
                {
                    // Remove the existing instance as it will be replaced below
                    myClient.MessageProviders.EmailProviders.Remove(originalProvider);

                    updatedProvider.Name = spanProviderName.InnerHtml.Replace("Email", "").Replace("Configuration", "").Trim();
                    updatedProvider.CredentialsRequired = chkCredentialsRequired.Checked;
                    updatedProvider.FromEmail = txtFromEmail.Text;
                    updatedProvider.IsBodyHtml = chkIsBodyHtml.Checked;
                    updatedProvider.LoginPassword = txtLoginPassword.Text;
                    updatedProvider.LoginUserName = txtLoginUserName.Text;
                    updatedProvider.NewlineReplacement = txtNewlineReplacement.Text;
                    updatedProvider.Port = txtPort.Text;
                    updatedProvider.RequiresSsl = chkRequiresSsl.Checked;
                    updatedProvider.Server = txtServer.Text;
                    updatedProvider.AdminNotificationOnFailure = chkNotifyAdminOnFailure.Checked;
                }
                else
                {
                    updatedProvider = GetDefaultProvider();
                }

                // Add the new provider to the client
                myClient.MessageProviders.EmailProviders.Add(updatedProvider);

                myClient.Update();

                // Compare objects to determine values that were changed
                Utils utility = new Utils();
                var differences = utility.GetObjectDifferences(originalProvider, updatedProvider);

                // Log the changes
                var providerEvent = new Event
                {
                    ClientId = myClient._id,
                    UserId = ObjectId.Parse(loggedInAdminId),
                    EventTypeDesc = Constants.TokenKeys.ProviderName + updatedProvider.Name
                                    + Constants.TokenKeys.ProviderType + "Email"
                                    + Constants.TokenKeys.ProviderChangedValues + differences
                                    + Constants.TokenKeys.ClientName + myClient.Name
                };

                providerEvent.Create(Constants.EventLog.Providers.Updated, null);

                ClientScript.RegisterStartupScript(typeof(Page), "closePage", "<script type='text/JavaScript'>callParentDocumentFunction();</script>");
            }
            else
            {
                // Check to see if the passed in hiddenProviderID already exists in the client. If so, display that provider's settings
                if (originalProvider != null)
                {
                    spanProviderName.InnerHtml = originalProvider.Name + " Email Configuration";

                    txtName.Text = originalProvider.Name;
                    txtFromEmail.Text = originalProvider.FromEmail;
                    txtLoginUserName.Text = originalProvider.LoginUserName;
                    txtLoginPassword.Text = originalProvider.LoginPassword;
                    txtServer.Text = originalProvider.Server;
                    txtPort.Text = originalProvider.Port.ToString(CultureInfo.CurrentCulture);
                    txtNewlineReplacement.Text = originalProvider.NewlineReplacement;
                    chkRequiresSsl.Checked = originalProvider.RequiresSsl;
                    chkCredentialsRequired.Checked = originalProvider.CredentialsRequired;
                    chkIsBodyHtml.Checked = originalProvider.IsBodyHtml;
                    chkNotifyAdminOnFailure.Checked = originalProvider.AdminNotificationOnFailure;
                }
                else // If we're here, it means the client doesn't have this provider type defined yet.
                {
                    GetDefaultProvider();
                }
            }
        }

        private ProviderEmail GetDefaultProvider()
        {
            var mongoDBConnectionPool = (MongoDatabase)HttpContext.Current.Application[Constants.Strings.MongoDB];
            MongoCollection mongoCollection = mongoDBConnectionPool.GetCollection("TypeDefinitions");

            var query = Query.EQ("_id", ObjectId.Parse(hiddenProviderID.Value));

            var emailProvider = mongoCollection.FindOneAs<ProviderEmail>(query);

            spanProviderName.InnerHtml = emailProvider.Name + " Email Configuration";

            txtName.Text = emailProvider.Name;
            txtFromEmail.Text = emailProvider.FromEmail;
            txtLoginUserName.Text = emailProvider.LoginUserName;
            txtLoginPassword.Text = emailProvider.LoginPassword;
            txtServer.Text = emailProvider.Server;
            txtPort.Text = emailProvider.Port.ToString(CultureInfo.CurrentCulture);
            txtNewlineReplacement.Text = emailProvider.NewlineReplacement;
            chkRequiresSsl.Checked = emailProvider.RequiresSsl;
            chkCredentialsRequired.Checked = emailProvider.CredentialsRequired;
            chkIsBodyHtml.Checked = emailProvider.IsBodyHtml;
            chkNotifyAdminOnFailure.Checked = emailProvider.AdminNotificationOnFailure;

            return emailProvider;
        }

        static Predicate<ProviderEmail> FindProviderById(string providerId)
        {
            return provider => provider._id == ObjectId.Parse(providerId);
        }
    }
}