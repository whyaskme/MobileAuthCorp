using System;
using System.Globalization;
using System.Web;
using System.Web.UI;
using MACServices;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Admin.Providers.Messaging.Voice
{
    public partial class ConfigPopup : Page
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

            var providerId = ObjectId.GenerateNewId();
            if (!String.IsNullOrEmpty(HttpContext.Current.Request["providerID"]))
                providerId = ObjectId.Parse(HttpContext.Current.Request["providerID"]);

            hiddenProviderID.Value = providerId.ToString();

            var clientId = "";
            if (!String.IsNullOrEmpty(HttpContext.Current.Request["clientId"]))
                clientId = HttpContext.Current.Request["clientId"];

            var userIsReadOnly = "";
            if (!String.IsNullOrEmpty(Request["userisreadonly"]))
                userIsReadOnly = Request["userisreadonly"];

            userIsReadOnly = MACSecurity.Security.DecodeAndDecrypt(userIsReadOnly, loggedInAdminId);

            var myClient = new Client(clientId);

            var originalProvider = myClient.MessageProviders.VoiceProviders.Find(FindProviderById(hiddenProviderID.Value));
            var updatedProvider = new ProviderVoice();

            if (IsPostBack)
            {
                if (originalProvider != null)
                {
                    // Remove the existing instance as it will be replaced below
                    myClient.MessageProviders.VoiceProviders.Remove(originalProvider);

                    updatedProvider.Name = spanProviderName.InnerHtml.Replace("Voice", "").Replace("Configuration", "").Trim();
                    updatedProvider.ApiVersion = txtApiVersion.Text;
                    updatedProvider.ClientCharge = Convert.ToDouble(txtClientCharge.Text);
                    updatedProvider.FromPhoneNumber = txtFromPhoneNumber.Text;
                    updatedProvider.Key = txtKey.Text;
                    updatedProvider.LoginPassword = txtLoginPassword.Text;
                    updatedProvider.LoginUsername = txtLoginUsername.Text;
                    updatedProvider.PhoneNumberFormat = txtPhoneNumberFormat.Text;
                    updatedProvider.Port = txtPort.Text;
                    updatedProvider.Protocol = txtProtocol.Text;
                    updatedProvider.ProviderCharge = Convert.ToDouble(txtProviderCharge.Text);
                    updatedProvider.RequiresSsl = chkRequiresSsl.Checked;
                    updatedProvider.Server = txtServer.Text;
                    updatedProvider.Sid = txtSID.Text;
                    updatedProvider.VoiceToken = txtVoiceToken.Text;
                }
                else
                {
                    updatedProvider = GetDefaultProvider();
                }

                // Add the new provider to the client
                myClient.MessageProviders.VoiceProviders.Add(updatedProvider);

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
                                    + Constants.TokenKeys.ProviderType + "Voice"
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
                    spanProviderName.InnerHtml = originalProvider.Name + " Voice Configuration";

                    txtName.Text = originalProvider.Name;
                    txtApiVersion.Text = originalProvider.ApiVersion;
                    txtFromPhoneNumber.Text = originalProvider.FromPhoneNumber;
                    txtKey.Text = originalProvider.Key;
                    txtLoginUsername.Text = originalProvider.LoginUsername;
                    txtLoginPassword.Text = originalProvider.LoginPassword;
                    txtPort.Text = originalProvider.Port.ToString(CultureInfo.CurrentCulture);
                    txtProtocol.Text = originalProvider.Protocol;
                    chkRequiresSsl.Checked = originalProvider.RequiresSsl;
                    txtServer.Text = originalProvider.Server;
                    txtSID.Text = originalProvider.Sid;
                    txtVoiceToken.Text = originalProvider.VoiceToken;
                    txtPhoneNumberFormat.Text = originalProvider.PhoneNumberFormat;
                    txtClientCharge.Text = originalProvider.ClientCharge.ToString(CultureInfo.CurrentCulture);
                    txtProviderCharge.Text = originalProvider.ProviderCharge.ToString(CultureInfo.CurrentCulture);
                }
                else // If we're here, it means the client doesn't have this provider type defined yet.
                {
                    GetDefaultProvider();
                }
            }

            if (Convert.ToBoolean(userIsReadOnly))
            {
                txtApiVersion.Enabled = false;
                txtFromPhoneNumber.Enabled = false;
                txtKey.Enabled = false;
                txtLoginUsername.Enabled = false;
                txtLoginPassword.Enabled = false;
                txtServer.Enabled = false;

                chkRequiresSsl.Enabled = false;

                btnSave.Visible = false;
                btn_cancel.Visible = true;
            }
        }

        private ProviderVoice GetDefaultProvider()
        {
            var mongoDBConnectionPool = (MongoDatabase)HttpContext.Current.Application[Constants.Strings.MongoDB];

            var mongoCollection = mongoDBConnectionPool.GetCollection("TypeDefinitions");

            var query = Query.EQ("_id", ObjectId.Parse(hiddenProviderID.Value));

            var voiceProvider = mongoCollection.FindOneAs<ProviderVoice>(query);

            spanProviderName.InnerHtml = voiceProvider.Name + " Voice Configuration";

            txtName.Text = voiceProvider.Name;
            txtApiVersion.Text = voiceProvider.ApiVersion;
            txtFromPhoneNumber.Text = voiceProvider.FromPhoneNumber;
            txtKey.Text = voiceProvider.Key;
            txtLoginUsername.Text = voiceProvider.LoginUsername;
            txtLoginPassword.Text = voiceProvider.LoginPassword;
            txtPort.Text = voiceProvider.Port.ToString(CultureInfo.CurrentCulture);
            txtProtocol.Text = voiceProvider.Protocol;
            chkRequiresSsl.Checked = voiceProvider.RequiresSsl;
            txtServer.Text = voiceProvider.Server;
            txtSID.Text = voiceProvider.Sid;
            txtVoiceToken.Text = voiceProvider.VoiceToken;
            txtPhoneNumberFormat.Text = voiceProvider.PhoneNumberFormat;
            txtClientCharge.Text = voiceProvider.ClientCharge.ToString(CultureInfo.CurrentCulture);
            txtProviderCharge.Text = voiceProvider.ProviderCharge.ToString(CultureInfo.CurrentCulture);

            return voiceProvider;
        }

        static Predicate<ProviderVoice> FindProviderById(string providerId)
        {
            return provider => provider._id == ObjectId.Parse(providerId);
        }
    }
}