using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

using MACServices;
using MACSecurity;

using cfg = MACServices.Constants.WebConfig;

namespace Reset
{
    public partial class ResetAdProviders : Page
    {
        public Utils myUtils = new Utils();

        public ObjectId defaultProviderId = ObjectId.Empty;
        public ObjectId newProviderId = ObjectId.Empty;

        public ProviderEmail myDefaultEmailProvider;
        public ProviderSms myDefaultSmsProvider;
        public ProviderVoice myDefaultVoiceProvider;

        public ProviderEmail myNewEmailProvider;
        public ProviderSms myNewSmsProvider;
        public ProviderVoice myNewVoiceProvider;

        public UserProfile adminProfile;

        public string adminFirstName = "";
        public string adminLastName = "";

        public Event providerEvent = new Event();

        public string adminUserId = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            // Deny access if user request not logged in
            var isAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
            if (!isAuthenticated)
            {
                Response.Write("Hello world!");
                Response.End();
            }

            if (Request["userId"] != null)
            {
                adminUserId = Request["userId"].ToString();

                adminUserId = Security.DecodeAndDecrypt(adminUserId, Constants.Strings.DefaultClientId);

                adminProfile = new UserProfile(adminUserId);

                adminFirstName = Security.DecodeAndDecrypt(adminProfile.FirstName, adminUserId);
                adminLastName = Security.DecodeAndDecrypt(adminProfile.LastName, adminUserId);
            }

            if (IsPostBack) // Run the process
            {
                UpdateClientProviders();

                hiddenAA.Value = "";

                ClientScript.RegisterStartupScript(typeof(Page), "closePage", "<script type='text/JavaScript'>callParentDocumentFunction();</script>");
            }
            else // 
            {
                var myProvider = new ProviderAdvertising();

                txtProviderName.Text = myProvider.Name;
                txtAdClientId.Text = myProvider.AdClientId;
                txtAPIKey.Text = myProvider.ApiKey;
                txtAPIUrl.Text = myProvider.ApiUrl;
                txtAPIUsername.Text = myProvider.UserName;
                txtAPIPassword.Text = myProvider.Password;
            }
        }

        private void UpdateClientProviders()
        {
            var mongoCollection = myUtils.mongoDBConnectionPool.GetCollection("Client");
            var clientCollection = mongoCollection.FindAllAs<Client>();

            foreach (Client currentClient in clientCollection)
            {
                if(currentClient.AdProviders != null)
                {
                    foreach(ProviderAdvertising currentProvider in currentClient.AdProviders)
                    {
                        if (txtAdClientId.Text != "")
                            currentProvider.AdClientId = txtAdClientId.Text;

                        if (txtAPIKey.Text != "")
                            currentProvider.ApiKey = txtAPIKey.Text;

                        if (txtAPIUrl.Text != "")
                            currentProvider.ApiUrl = txtAPIUrl.Text;

                        if (txtProviderName.Text != "")
                            currentProvider.Name = txtProviderName.Text;

                        if (txtAPIUsername.Text != "")
                            currentProvider.UserName = txtAPIUsername.Text;

                        if (txtAPIPassword.Text != "")
                            currentProvider.Password = txtAPIPassword.Text;

                        //currentProvider.P1 = "";
                        //currentProvider.P2 = "";
                        //currentProvider.P3 = "";
                        //currentProvider.P4 = "";
                    }
                    currentClient.Update();
                }

                // Log the provider changes
                var providerEvent = new Event
                {
                    ClientId = currentClient._id,
                    UserId = ObjectId.Parse(adminUserId),
                    EventTypeDesc = Constants.TokenKeys.ClientName + currentClient.Name
                                    + Constants.TokenKeys.ProviderChangedValues + "ProviderUrl"
                                    + Constants.TokenKeys.ProviderName + "SecureAds"
                                    + Constants.TokenKeys.ProviderType + "Advertising"
                                    + Constants.TokenKeys.EventGeneratedByName + adminFirstName + " " + adminLastName
                };
                providerEvent.Create(Constants.EventLog.Providers.AllReset, null);
            }
        }
    }
}