using System;
using System.Globalization;
using System.Web;
using System.Web.UI;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

using MACServices;

namespace Admin.Providers.Messaging.SMS
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

            var originalProvider = myClient.MessageProviders.SmsProviders.Find(FindProviderById(hiddenProviderID.Value));
            var updatedProvider = new ProviderSms();

            if (IsPostBack)
            {
                if (originalProvider != null)
                {
                    // Remove the existing instance as it will be replaced below
                    myClient.MessageProviders.SmsProviders.Remove(originalProvider);

                    updatedProvider.Name = spanProviderName.InnerHtml.Replace("SMS", "").Replace("Configuration", "").Trim();
                    updatedProvider.ApiVersion = txtApiVersion.Text;
                    updatedProvider.AuthToken = txtAuthToken.Text;
                    updatedProvider.ClientCharge = Convert.ToDouble(txtClientCharge.Text);
                    updatedProvider.Key = txtKey.Text;
                    updatedProvider.LoginPassword = txtLoginPassword.Text;
                    updatedProvider.LoginUsername = txtLoginUsername.Text;
                    updatedProvider.NewlineReplacement = txtNewlineReplacement.Text;
                    updatedProvider.PhoneNumberFormat = txtPhoneNumberFormat.Text;
                    updatedProvider.Port = txtPort.Text;
                    updatedProvider.Protocol = txtProtocol.Text;
                    updatedProvider.ProviderCharge = Convert.ToDouble(txtProviderCharge.Text);
                    updatedProvider.Server = txtServer.Text;
                    updatedProvider.ShortCodeFromNumber = txtShortCodeFromNumber.Text;
                    updatedProvider.Sid = txtSID.Text;
                    updatedProvider.Url = txtUrl.Text;
                    updatedProvider.VoiceToken = txtVoiceToken.Text;
                }
                else
                {
                    updatedProvider = GetDefaultProvider();
                }

                // Add the new provider to the client
                myClient.MessageProviders.SmsProviders.Add(updatedProvider);

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
                                    + Constants.TokenKeys.ProviderType + "SMS"
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
                    spanProviderName.InnerHtml = originalProvider.Name + " SMS Configuration";
                    if (originalProvider.Name.Contains("MessageBroadcast"))
                    {
                        spanPrivateKey.InnerHtml = "Private Key";
                        spanPublicKey.InnerHtml = "Public Key";
                        //lblShortCode.InnerHtml = "Batch ID";
                    }
                    //else
                    //    lblShortCode.InnerHtml = "Short Code";

                    txtUrl.Text = originalProvider.Url;
                    txtSID.Text = originalProvider.Sid;
                    txtAuthToken.Text = originalProvider.AuthToken;
                    txtShortCodeFromNumber.Text = originalProvider.ShortCodeFromNumber;
                    txtApiVersion.Text = originalProvider.ApiVersion;
                    txtNewlineReplacement.Text = originalProvider.NewlineReplacement;
                    txtKey.Text = originalProvider.Key;
                    txtLoginUsername.Text = originalProvider.LoginUsername;
                    txtLoginPassword.Text = originalProvider.LoginPassword;
                    txtProtocol.Text = originalProvider.Protocol;
                    txtPort.Text = originalProvider.Port.ToString(CultureInfo.CurrentCulture);
                    txtServer.Text = originalProvider.Server;
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
                txtUrl.Enabled = false;
                txtSID.Enabled = false;
                txtAuthToken.Enabled = false;
                txtShortCodeFromNumber.Enabled = false;
                txtApiVersion.Enabled = false;
                txtNewlineReplacement.Enabled = false;

                btnSave.Visible = false;
                btn_cancel.Visible = true;
            }
        }

        private ProviderSms GetDefaultProvider()
        {
            var mongoDBConnectionPool = (MongoDatabase)HttpContext.Current.Application[Constants.Strings.MongoDB];
            MongoCollection mongoCollection = mongoDBConnectionPool.GetCollection("TypeDefinitions");

            var query = Query.EQ("_id", ObjectId.Parse(hiddenProviderID.Value));

            var smsProvider = mongoCollection.FindOneAs<ProviderSms>(query);

            spanProviderName.InnerHtml = smsProvider.Name + " SMS Configuration";

            if (smsProvider.Name.Contains("MessageBroadcast"))
            {
                spanPrivateKey.InnerHtml = "Private Key";
                spanPublicKey.InnerHtml = "Public Key";
            }

            txtUrl.Text = smsProvider.Url;
            txtSID.Text = smsProvider.Sid;
            txtAuthToken.Text = smsProvider.AuthToken;
            txtShortCodeFromNumber.Text = smsProvider.ShortCodeFromNumber;
            txtApiVersion.Text = smsProvider.ApiVersion;
            txtNewlineReplacement.Text = smsProvider.NewlineReplacement;
            txtKey.Text = smsProvider.Key;
            txtLoginUsername.Text = smsProvider.LoginUsername;
            txtLoginPassword.Text = smsProvider.LoginPassword;
            txtProtocol.Text = smsProvider.Protocol;
            txtPort.Text = smsProvider.Port.ToString(CultureInfo.CurrentCulture);
            txtServer.Text = smsProvider.Server;
            txtVoiceToken.Text = smsProvider.VoiceToken;
            txtPhoneNumberFormat.Text = smsProvider.PhoneNumberFormat;
            txtClientCharge.Text = smsProvider.ClientCharge.ToString(CultureInfo.CurrentCulture);
            txtProviderCharge.Text = smsProvider.ProviderCharge.ToString(CultureInfo.CurrentCulture);

            return smsProvider;
        }

        static Predicate<ProviderSms> FindProviderById(string providerId)
        {
            return provider => provider._id == ObjectId.Parse(providerId);
        }
    }
}