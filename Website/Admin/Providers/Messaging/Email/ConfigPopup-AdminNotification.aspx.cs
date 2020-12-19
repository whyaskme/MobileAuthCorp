using System;
using System.Globalization;
using System.Web;
using System.Web.UI;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

using MACServices;
using cs = MACServices.Constants.Strings;

namespace Admin.Providers.Messaging.Email
{
    public partial class ConfigPopupAdminNotification : Page
    {
        public string loggedInUserId = "";
        public string loggedInUserIsReadOnly = "";
        public string loggedInUserFirstName = "";
        public string loggedInUserLastName = "";

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

            loggedInAdminId = MACSecurity.Security.DecodeAndDecrypt(loggedInAdminId, Constants.Strings.DefaultClientId);

// ReSharper disable once NotAccessedVariable
            var providerId = ObjectId.GenerateNewId();
            if (!String.IsNullOrEmpty(HttpContext.Current.Request["providerID"]))
// ReSharper disable once RedundantAssignment
                providerId = ObjectId.Parse(HttpContext.Current.Request["providerID"]);

            hiddenProviderID.Value = cs.DefaultEmailProvider;

            var clientId = "";
            if (!String.IsNullOrEmpty(HttpContext.Current.Request["clientId"]))
                clientId = HttpContext.Current.Request["clientId"];

            var userIsReadOnly = "";
            if (!String.IsNullOrEmpty(Request["userisreadonly"]))
                userIsReadOnly = Request["userisreadonly"];

            userIsReadOnly = MACSecurity.Security.DecodeAndDecrypt(userIsReadOnly, loggedInAdminId);

            var myClient = new Client(clientId);

            var originalProvider = myClient.Organization.AdminNotificationProvider;
            var updatedProvider = new ProviderEmail();

            if (IsPostBack)
            {
                if (originalProvider != null)
                {
                    updatedProvider.Name = spanProviderName.InnerHtml.Replace("Email", "").Replace("Configuration", "").Trim();
                    updatedProvider.CredentialsRequired = chkCredentialsRequired.Checked;
                    updatedProvider.FromEmail = txtFromEmail.Text;
                    updatedProvider.IsBodyHtml = chkIsBodyHtml.Checked;
                    updatedProvider.LoginPassword = txtLoginPassword.Text;
                    updatedProvider.LoginUserName = txtLoginUserName.Text;
                    updatedProvider.NewlineReplacement = txtNewlineReplacement.Text;
                    updatedProvider.NotficationSubject = "MAC Notification(To be set in client config)";
                    updatedProvider.OtpSubject = "MAC Subject(To be set in client config)";
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
                myClient.Organization.AdminNotificationProvider = updatedProvider;

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
                                    + Constants.TokenKeys.ProviderType + "Admin Notification"
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

            if (Convert.ToBoolean(userIsReadOnly))
            {
                txtFromEmail.Enabled = false;
                txtName.Enabled = false;
                txtLoginUserName.Enabled = false;
                txtLoginPassword.Enabled = false;
                txtServer.Enabled = false;
                txtPort.Enabled = false;
                txtNewlineReplacement.Enabled = false;

                chkCredentialsRequired.Enabled = false;
                chkRequiresSsl.Enabled = false;
                chkIsBodyHtml.Enabled = false;
                chkNotifyAdminOnFailure.Enabled = false;

                btnSave.Visible = false;
                btn_cancel.Visible = true;
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