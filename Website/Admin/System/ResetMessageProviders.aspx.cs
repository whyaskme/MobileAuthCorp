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
    public partial class ResetMessageProviders : Page
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

            dlProviders.Visible = false;

            divEmailProvider.Visible = false;
            divSmsProvider.Visible = false;
            divVoiceProvider.Visible = false;

            btnUpdate.Disabled = true;

            spanProviderId.InnerHtml = "";

            if (Request["userId"] != null)
            {
                adminUserId = Request["userId"].ToString();

                adminUserId = Security.DecodeAndDecrypt(adminUserId, Constants.Strings.DefaultClientId);

                adminProfile = new UserProfile(adminUserId);

                adminFirstName = Security.DecodeAndDecrypt(adminProfile.FirstName, adminUserId);
                adminLastName = Security.DecodeAndDecrypt(adminProfile.LastName, adminUserId);
            }

            switch(dlProviderTypes.SelectedIndex)
            {
                case 0: // Noting selected
                    link_help.HRef = "javascript: NavigateTopicPopup('54ee670db5655a1fd4adc17f');";
                    break;

                case 1: // Email
                    link_help.HRef = "javascript: NavigateTopicPopup('5548150aa6e10b18d4499828');";
                    break;

                case 2: // Sms
                    link_help.HRef = "javascript: NavigateTopicPopup('5548151ca6e10b18d449982a');";
                    break;

                case 3: // Voice
                    link_help.HRef = "javascript: NavigateTopicPopup('55481536a6e10b18d449982c');";
                    break;
            }

            if (IsPostBack) // Run the process
            {
                switch (hiddenAA.Value)
                {
                    case "":
                        if (dlProviderTypes.SelectedIndex > 0)
                        {
                            dlProviders.Visible = true;
                            GetDefaultProviders(dlProviderTypes.SelectedValue);
                        }
                        break;

                    case "GetProviderDetails":
                        dlProviders.Visible = true;
                        spanProviderId.InnerHtml = hiddenSelectedProviderId.Value;
                        GetDefaultProviderDetails(hiddenSelectedProviderId.Value);
                        hiddenSelectedProviderId.Value = "";
                        break;

                    case "UpdateProvider":
                        switch (dlProviderTypes.SelectedValue)
                        {
                            case "Email":
                                myNewEmailProvider = new ProviderEmail();
                                SetProviderPropertiesFromForm();
                                UpdateClientProviders();
                                myUtils.ObjectUpdate(myNewEmailProvider, myNewEmailProvider._id.ToString());
                                break;

                            case "Sms":
                                myNewSmsProvider = new ProviderSms();
                                SetProviderPropertiesFromForm();
                                UpdateClientProviders();
                                myUtils.ObjectUpdate(myNewSmsProvider, myNewSmsProvider._id.ToString());
                                break;

                            case "Voice":
                                myNewVoiceProvider = new ProviderVoice();
                                SetProviderPropertiesFromForm();
                                UpdateClientProviders();
                                myUtils.ObjectUpdate(myNewVoiceProvider, myNewVoiceProvider._id.ToString());
                                break;
                        }

                        hiddenAA.Value = "";

                        ClientScript.RegisterStartupScript(typeof(Page), "closePage", "<script type='text/JavaScript'>callParentDocumentFunction();</script>");
                        break;
                }
            }
            else // 
            {
                var sbResponse = new StringBuilder();
            }
        }

        private void UpdateClientProviders()
        {
            GetDefaultProviderDetails(hiddenSelectedProviderId.Value);

            var mongoCollection = myUtils.mongoDBConnectionPool.GetCollection("Client");
            var clientCollection = mongoCollection.FindAllAs<Client>();

            var clientCount = clientCollection.Count();
            var currentIndex = 0;

            foreach (Client currentClient in clientCollection)
            {
                currentIndex++;

                switch(dlProviderTypes.SelectedValue)
                {
                    case "Email":
                        foreach (ProviderEmail currentEmailProvider in currentClient.MessageProviders.EmailProviders)
                        {
                            var sDifferences = myUtils.GetObjectDifferences(currentEmailProvider, myNewEmailProvider);

                            if (currentEmailProvider._id.ToString() == hiddenSelectedProviderId.Value)
                            {
                                // This is the one we want to compare and decide if we need to update it
                                currentEmailProvider.AdminNotificationOnFailure = myNewEmailProvider.AdminNotificationOnFailure;
                                currentEmailProvider.ClientCharge = myNewEmailProvider.ClientCharge;
                                currentEmailProvider.CredentialsRequired = myNewEmailProvider.CredentialsRequired;
                                currentEmailProvider.Enabled = myNewEmailProvider.Enabled;
                                currentEmailProvider.FromEmail = myNewEmailProvider.FromEmail;
                                currentEmailProvider.IsBodyHtml = myNewEmailProvider.IsBodyHtml;
                                currentEmailProvider.LoginPassword = myNewEmailProvider.LoginPassword;
                                currentEmailProvider.LoginUserName = myNewEmailProvider.LoginUserName;
                                currentEmailProvider.Name = myNewEmailProvider.Name;
                                currentEmailProvider.NewlineReplacement = myNewEmailProvider.NewlineReplacement;
                                currentEmailProvider.NotficationSubject = myNewEmailProvider.NotficationSubject;
                                currentEmailProvider.OtpSubject = myNewEmailProvider.OtpSubject;
                                currentEmailProvider.Port = myNewEmailProvider.Port;
                                currentEmailProvider.ProviderCharge = myNewEmailProvider.ProviderCharge;
                                currentEmailProvider.RequiresSsl = myNewEmailProvider.RequiresSsl;
                                currentEmailProvider.Server = myNewEmailProvider.Server;

                                currentClient.Update();

                                // Log the provider changes
                                var providerEvent = new Event
                                {
                                    ClientId = currentClient._id,
                                    UserId = ObjectId.Parse(adminUserId),
                                    EventTypeDesc = Constants.TokenKeys.ClientName + currentClient.Name
                                                    + Constants.TokenKeys.ProviderChangedValues + sDifferences
                                                    + Constants.TokenKeys.ProviderName + currentEmailProvider.Name
                                                    + Constants.TokenKeys.ProviderType + dlProviderTypes.SelectedValue
                                                    + Constants.TokenKeys.EventGeneratedByName + adminFirstName + " " + adminLastName
                                };
                                providerEvent.Create(Constants.EventLog.Providers.AllReset, null);
                            }
                            else
                            {
                                // Ignore
                            }
                        }
                        break;

                    case "Sms":
                        foreach (ProviderSms currentSmsProvider in currentClient.MessageProviders.SmsProviders)
                        {
                            var sDifferences = myUtils.GetObjectDifferences(currentSmsProvider, myNewSmsProvider);

                            if (currentSmsProvider._id.ToString() == hiddenSelectedProviderId.Value)
                            {
                                // This is the one we want to compare and decide if we need to update it
                                currentSmsProvider.ApiVersion = myNewSmsProvider.ApiVersion;
                                currentSmsProvider.AuthToken = myNewSmsProvider.AuthToken;
                                currentSmsProvider.ClientCharge = myNewSmsProvider.ClientCharge;
                                currentSmsProvider.CredentialsRequired = myNewSmsProvider.CredentialsRequired;
                                currentSmsProvider.Enabled = myNewSmsProvider.Enabled;
                                currentSmsProvider.Key = myNewSmsProvider.Key;
                                currentSmsProvider.LoginPassword = myNewSmsProvider.LoginPassword;
                                currentSmsProvider.LoginUsername = myNewSmsProvider.LoginUsername;
                                currentSmsProvider.Name = myNewSmsProvider.Name;
                                currentSmsProvider.NewlineReplacement = myNewSmsProvider.NewlineReplacement;
                                currentSmsProvider.PhoneNumberFormat = myNewSmsProvider.PhoneNumberFormat;
                                currentSmsProvider.Port = myNewSmsProvider.Port;
                                currentSmsProvider.Protocol = myNewSmsProvider.Protocol;
                                currentSmsProvider.ProviderCharge = myNewSmsProvider.ProviderCharge;
                                currentSmsProvider.RequiresSsl = myNewSmsProvider.RequiresSsl;
                                currentSmsProvider.Server = myNewSmsProvider.Server;

                                // Only update shortcode if it's not custom defined by comparison to original default value
                                // If they are the same, this indicates a default code and need to update accordingly
                                if (currentSmsProvider.ShortCodeFromNumber == myDefaultSmsProvider.ShortCodeFromNumber)
                                {
                                    // Update shortcode from new settings
                                    currentSmsProvider.ShortCodeFromNumber = myNewSmsProvider.ShortCodeFromNumber;
                                }
                                else
                                {
                                    // If they are not the same, this indicates a custom code. Leave shortcode intact.
                                }

                                currentSmsProvider.Sid = myNewSmsProvider.Sid;
                                currentSmsProvider.Url = myNewSmsProvider.Url;
                                currentSmsProvider.VoiceToken = myNewSmsProvider.VoiceToken;

                                currentClient.Update();

                                // Log the provider changes
                                var providerEvent = new Event
                                {
                                    ClientId = currentClient._id,
                                    UserId = ObjectId.Parse(adminUserId),
                                    EventTypeDesc = Constants.TokenKeys.ClientName + currentClient.Name
                                                    + Constants.TokenKeys.ProviderChangedValues + sDifferences
                                                    + Constants.TokenKeys.ProviderName + currentSmsProvider.Name
                                                    + Constants.TokenKeys.ProviderType + dlProviderTypes.SelectedValue
                                                    + Constants.TokenKeys.EventGeneratedByName + adminFirstName + " " + adminLastName
                                };
                                providerEvent.Create(Constants.EventLog.Providers.AllReset, null);
                            }
                            else
                            {
                                // Ignore
                            }
                        }
                        break;

                    case "Voice":
                        foreach (ProviderVoice currentVoiceProvider in currentClient.MessageProviders.VoiceProviders)
                        {
                            var sDifferences = myUtils.GetObjectDifferences(currentVoiceProvider, myNewVoiceProvider);

                            if (currentVoiceProvider._id.ToString() == hiddenSelectedProviderId.Value)
                            {
                                // This is the one we want to compare and decide if we need to update it
                                currentVoiceProvider.ApiVersion = myNewVoiceProvider.ApiVersion;
                                currentVoiceProvider.AuthToken = myNewVoiceProvider.AuthToken;
                                currentVoiceProvider.ClientCharge = myNewVoiceProvider.ClientCharge;
                                currentVoiceProvider.CredentialsRequired = myNewVoiceProvider.CredentialsRequired;
                                currentVoiceProvider.Enabled = myNewVoiceProvider.Enabled;
                                currentVoiceProvider.FromEmail = myNewVoiceProvider.FromEmail;
                                currentVoiceProvider.FromPhoneNumber = myNewVoiceProvider.FromPhoneNumber;
                                currentVoiceProvider.Key = myNewVoiceProvider.Key;
                                currentVoiceProvider.LoginPassword = myNewVoiceProvider.LoginPassword;
                                currentVoiceProvider.LoginUsername = myNewVoiceProvider.LoginUsername;
                                currentVoiceProvider.Name = myNewVoiceProvider.Name;
                                currentVoiceProvider.NewlineReplacement = myNewVoiceProvider.NewlineReplacement;
                                currentVoiceProvider.PhoneNumberFormat = myNewVoiceProvider.PhoneNumberFormat;
                                currentVoiceProvider.Port = myNewVoiceProvider.Port;
                                currentVoiceProvider.Protocol = myNewVoiceProvider.Protocol;
                                currentVoiceProvider.ProviderCharge = myNewVoiceProvider.ProviderCharge;
                                currentVoiceProvider.RequiresSsl = myNewVoiceProvider.RequiresSsl;
                                currentVoiceProvider.Server = myNewVoiceProvider.Server;

                                // Only update shortcode if it's not custom defined by comparison to original default value
                                // If they are the same, this indicates a default code and need to update accordingly
                                if (currentVoiceProvider.ShortCodeFromNumber == myDefaultVoiceProvider.ShortCodeFromNumber)
                                {
                                    // Update shortcode from new settings
                                    currentVoiceProvider.ShortCodeFromNumber = myNewVoiceProvider.ShortCodeFromNumber;
                                }
                                else
                                {
                                    // If they are not the same, this indicates a custom code. Leave shortcode intact.
                                }

                                currentVoiceProvider.Sid = myNewVoiceProvider.Sid;
                                currentVoiceProvider.Url = myNewVoiceProvider.Url;
                                currentVoiceProvider.VoiceToken = myNewVoiceProvider.VoiceToken;

                                currentClient.Update();

                                // Log the provider changes
                                var providerEvent = new Event
                                {
                                    ClientId = currentClient._id,
                                    UserId = ObjectId.Parse(adminUserId),
                                    EventTypeDesc = Constants.TokenKeys.ClientName + currentClient.Name
                                                    + Constants.TokenKeys.ProviderChangedValues + sDifferences
                                                    + Constants.TokenKeys.ProviderName + currentVoiceProvider.Name
                                                    + Constants.TokenKeys.ProviderType + dlProviderTypes.SelectedValue
                                                    + Constants.TokenKeys.EventGeneratedByName + adminFirstName + " " + adminLastName
                                };
                                providerEvent.Create(Constants.EventLog.Providers.AllReset, null);
                            }
                            else
                            {
                                // Ignore
                            }
                        }
                        break;
                }
            }
        }

        private void GetDefaultProviders(string ProviderType)
        {
            var defaultListItemText = "";
            switch(ProviderType)
            {
                case "Email":
                    defaultListItemText = "Select an " + ProviderType + " provider";
                    break;

                case "Sms":
                    defaultListItemText = "Select an " + ProviderType + " provider";
                    break;

                case "Voice":
                    defaultListItemText = "Select a " + ProviderType + " provider";
                    break;
            }

            dlProviders.Items.Clear();

            ListItem defaultListItem = new ListItem();
            defaultListItem.Text = defaultListItemText;
            defaultListItem.Value = "-1";

            dlProviders.Items.Add(defaultListItem);

            // Get list of providers for specified type
            var objectType = "Provider" + ProviderType;

            var query = Query.EQ("_t", objectType);
            var sortBy = SortBy.Ascending("Name");

            switch (ProviderType)
            {
                case "Email":
                    var myDefaultEmailProviders = myUtils.mongoDBConnectionPool.GetCollection("TypeDefinitions").FindAs<ProviderEmail>(query).SetSortOrder(sortBy);
                    foreach (ProviderEmail currentProvider in myDefaultEmailProviders)
                    {
                        ListItem currentLi = new ListItem();
                        currentLi.Text = currentProvider.Name;
                        currentLi.Value = currentProvider._id.ToString();

                        dlProviders.Items.Add(currentLi);
                    }
                    break;

                case "Sms":
                    var myDefaultSmsProviders = myUtils.mongoDBConnectionPool.GetCollection("TypeDefinitions").FindAs<ProviderSms>(query).SetSortOrder(sortBy);
                    foreach (ProviderSms currentProvider in myDefaultSmsProviders)
                    {
                        ListItem currentLi = new ListItem();
                        currentLi.Text = currentProvider.Name;
                        currentLi.Value = currentProvider._id.ToString();

                        dlProviders.Items.Add(currentLi);
                    }
                    break;

                case "Voice":
                    var myDefaultVoiceProviders = myUtils.mongoDBConnectionPool.GetCollection("TypeDefinitions").FindAs<ProviderVoice>(query).SetSortOrder(sortBy);
                    foreach (ProviderVoice currentProvider in myDefaultVoiceProviders)
                    {
                        ListItem currentLi = new ListItem();
                        currentLi.Text = currentProvider.Name;
                        currentLi.Value = currentProvider._id.ToString();

                        dlProviders.Items.Add(currentLi);
                    }
                    break;
            }
        }

        private void GetDefaultProviderDetails(string providerId)
        {
            try
            {
                btnUpdate.Disabled = false;

                var query = Query.EQ("_id", ObjectId.Parse(providerId));

                switch (dlProviderTypes.SelectedValue)
                {
                    case "Email":
                        myDefaultEmailProvider = myUtils.mongoDBConnectionPool.GetCollection("TypeDefinitions").FindOneAs<ProviderEmail>(query);
                        defaultProviderId = myDefaultEmailProvider._id;
                        RenderSelectedProvider();
                        break;

                    case "Sms":
                        myDefaultSmsProvider = myUtils.mongoDBConnectionPool.GetCollection("TypeDefinitions").FindOneAs<ProviderSms>(query);
                        defaultProviderId = myDefaultSmsProvider._id;
                        RenderSelectedProvider();
                        break;

                    case "Voice":
                        myDefaultVoiceProvider = myUtils.mongoDBConnectionPool.GetCollection("TypeDefinitions").FindOneAs<ProviderVoice>(query);
                        defaultProviderId = myDefaultVoiceProvider._id;
                        RenderSelectedProvider();
                        break;
                }
            }
            catch(Exception ex)
            {
                var errMsg = ex.ToString();
            }
        }

        private void RenderSelectedProvider()
        {
            ResetForm();

            switch (dlProviderTypes.SelectedValue)
            {
                case "Email":
                    divEmailProvider.Visible = true;

                    chkEmailCredentialsRequired.Checked = myDefaultEmailProvider.CredentialsRequired;
                    chkEmailRequiresSsl.Checked = myDefaultEmailProvider.RequiresSsl;
                    chkEmailIsBodyHtml.Checked = myDefaultEmailProvider.IsBodyHtml;
                    chkEmailNotifyAdminOnFailure.Checked = myDefaultEmailProvider.AdminNotificationOnFailure;

                    txtEmailName.Text = myDefaultEmailProvider.Name;
                    txtEmailFromEmail.Text = myDefaultEmailProvider.FromEmail;
                    txtEmailLoginUsername.Text = myDefaultEmailProvider.LoginUserName;
                    txtEmailLoginPassword.Text = myDefaultEmailProvider.LoginPassword;
                    txtEmailServer.Text = myDefaultEmailProvider.Server;
                    txtEmailPort.Text = myDefaultEmailProvider.Port;
                    txtEmailNewlineReplacement.Text = myDefaultEmailProvider.NewlineReplacement;
                    break;

                case "Sms":
                    divSmsProvider.Visible = true;

                    txtSmsUrl.Text = myDefaultSmsProvider.Url;
                    txtSmsSID.Text = myDefaultSmsProvider.Sid;
                    txtSmsAuthToken.Text = myDefaultSmsProvider.AuthToken;
                    txtSmsShortCodeFromNumber.Text = myDefaultSmsProvider.ShortCodeFromNumber;
                    txtSmsApiVersion.Text = myDefaultSmsProvider.ApiVersion;
                    txtSmsNewlineReplacement.Text = myDefaultSmsProvider.NewlineReplacement;
                    txtSmsKey.Text = myDefaultSmsProvider.Key;
                    txtSmsLoginUsername.Text = myDefaultSmsProvider.LoginUsername;
                    txtSmsLoginPassword.Text = myDefaultSmsProvider.LoginPassword;
                    txtSmsProtocol.Text = myDefaultSmsProvider.Protocol;
                    txtSmsPort.Text = myDefaultSmsProvider.Port;
                    txtSmsServer.Text = myDefaultSmsProvider.Server;
                    txtSmsVoiceToken.Text = myDefaultSmsProvider.VoiceToken;
                    txtSmsPhoneNumberFormat.Text = myDefaultSmsProvider.PhoneNumberFormat;
                    txtSmsProviderCharge.Text = myDefaultSmsProvider.ProviderCharge.ToString();
                    txtSmsClientCharge.Text = myDefaultSmsProvider.ClientCharge.ToString();
                    break;

                case "Voice":
                    divVoiceProvider.Visible = true;

                    chkVoiceRequiresSsl.Checked = myDefaultVoiceProvider.RequiresSsl;

                    txtVoiceName.Text = myDefaultVoiceProvider.Name;
                    txtVoiceApiVersion.Text = myDefaultVoiceProvider.ApiVersion;
                    txtVoiceFromPhoneNumber.Text = myDefaultVoiceProvider.FromPhoneNumber;
                    txtVoiceKey.Text = myDefaultVoiceProvider.Key;
                    txtVoiceLoginUsername.Text = myDefaultVoiceProvider.LoginUsername;
                    txtVoiceLoginPassword.Text = myDefaultVoiceProvider.LoginPassword;
                    txtVoicePort.Text = myDefaultVoiceProvider.Port;
                    txtVoiceProtocol.Text = myDefaultVoiceProvider.Protocol;
                    txtVoiceServer.Text = myDefaultVoiceProvider.Server;
                    txtVoiceSID.Text = myDefaultVoiceProvider.Sid;
                    txtVoiceVoiceToken.Text = myDefaultVoiceProvider.VoiceToken;
                    txtVoiceNewlineReplacement.Text = myDefaultVoiceProvider.NewlineReplacement;
                    txtVoicePhoneNumberFormat.Text = myDefaultVoiceProvider.PhoneNumberFormat;
                    txtVoiceProviderCharge.Text = myDefaultVoiceProvider.ProviderCharge.ToString();
                    txtVoiceClientCharge.Text = myDefaultVoiceProvider.ClientCharge.ToString();
                    break;
            }
        }

        private void SetProviderPropertiesFromForm()
        {
            switch (dlProviderTypes.SelectedValue)
            {
                case "Email":
                    myNewEmailProvider.CredentialsRequired = chkEmailCredentialsRequired.Checked;
                    myNewEmailProvider.RequiresSsl = chkEmailRequiresSsl.Checked;
                    myNewEmailProvider.IsBodyHtml = chkEmailIsBodyHtml.Checked;
                    myNewEmailProvider.AdminNotificationOnFailure = chkEmailNotifyAdminOnFailure.Checked;

                    myNewEmailProvider.Name = txtEmailName.Text;
                    myNewEmailProvider.FromEmail = txtEmailFromEmail.Text;
                    myNewEmailProvider.LoginUserName = txtEmailLoginUsername.Text;
                    myNewEmailProvider.LoginPassword = txtEmailLoginPassword.Text;
                    myNewEmailProvider.Server = txtEmailServer.Text;
                    myNewEmailProvider.Port = txtEmailPort.Text;
                    myNewEmailProvider.NewlineReplacement = txtEmailNewlineReplacement.Text;
                    break;

                case "Sms":
                    myNewSmsProvider.Url = txtSmsUrl.Text;
                    myNewSmsProvider.Sid = txtSmsSID.Text;
                    myNewSmsProvider.AuthToken = txtSmsAuthToken.Text;
                    myNewSmsProvider.ShortCodeFromNumber = txtSmsShortCodeFromNumber.Text;
                    myNewSmsProvider.ApiVersion = txtSmsApiVersion.Text;
                    myNewSmsProvider.NewlineReplacement = txtSmsNewlineReplacement.Text;
                    myNewSmsProvider.Key = txtSmsKey.Text;
                    myNewSmsProvider.LoginUsername = txtSmsLoginUsername.Text;
                    myNewSmsProvider.LoginPassword = txtSmsLoginPassword.Text;
                    myNewSmsProvider.Protocol = txtSmsProtocol.Text;
                    myNewSmsProvider.Port = txtSmsPort.Text;
                    myNewSmsProvider.Server = txtSmsServer.Text;
                    myNewSmsProvider.VoiceToken = txtSmsVoiceToken.Text;
                    myNewSmsProvider.PhoneNumberFormat = txtSmsPhoneNumberFormat.Text;

                    if (txtSmsProviderCharge.Text != "")
                        myNewSmsProvider.ProviderCharge = Convert.ToDouble(txtSmsProviderCharge.Text);
                    else
                        myNewSmsProvider.ProviderCharge = 0.00;

                    if (txtSmsClientCharge.Text != "")
                        myNewSmsProvider.ClientCharge = Convert.ToDouble(txtSmsClientCharge.Text);
                    else
                        myNewSmsProvider.ClientCharge = 0.00;
                    break;

                case "Voice":
                    myNewVoiceProvider.RequiresSsl = chkVoiceRequiresSsl.Checked;

                    myNewVoiceProvider.Name = txtVoiceName.Text;
                    myNewVoiceProvider.ApiVersion = txtVoiceApiVersion.Text;
                    myNewVoiceProvider.FromPhoneNumber = txtVoiceFromPhoneNumber.Text;
                    myNewVoiceProvider.Key = txtVoiceKey.Text;
                    myNewVoiceProvider.LoginUsername = txtVoiceLoginUsername.Text;
                    myNewVoiceProvider.LoginPassword = txtVoiceLoginPassword.Text;
                    myNewVoiceProvider.Port = txtVoicePort.Text;
                    myNewVoiceProvider.Protocol = txtVoiceProtocol.Text;
                    myNewVoiceProvider.Server = txtVoiceServer.Text;
                    myNewVoiceProvider.Sid = txtVoiceSID.Text;
                    myNewVoiceProvider.VoiceToken = txtVoiceVoiceToken.Text;
                    myNewVoiceProvider.NewlineReplacement = txtVoiceNewlineReplacement.Text;
                    myNewVoiceProvider.PhoneNumberFormat = txtVoicePhoneNumberFormat.Text;

                    if (txtVoiceProviderCharge.Text != "")
                        myNewVoiceProvider.ProviderCharge = Convert.ToDouble(txtVoiceProviderCharge.Text);
                    else
                        myNewVoiceProvider.ProviderCharge = 0.00;

                    if (txtVoiceClientCharge.Text != "")
                        myNewVoiceProvider.ClientCharge = Convert.ToDouble(txtVoiceClientCharge.Text);
                    else
                        myNewVoiceProvider.ClientCharge = 0.00;
                    break;
            }
        }

        public void ResetForm()
        {
            // Email
            chkEmailCredentialsRequired.Checked = false;
            chkEmailRequiresSsl.Checked = false;
            chkEmailIsBodyHtml.Checked = false;
            chkEmailNotifyAdminOnFailure.Checked = false;

            txtEmailName.Text = "";
            txtEmailFromEmail.Text = "";
            txtEmailLoginUsername.Text = "";
            txtEmailLoginPassword.Text = "";
            txtEmailServer.Text = "";
            txtEmailPort.Text = "";
            txtEmailNewlineReplacement.Text = "";

            // Smsm
            txtSmsUrl.Text = "";
            txtSmsSID.Text = "";
            txtSmsAuthToken.Text = "";
            txtSmsShortCodeFromNumber.Text = "";
            txtSmsApiVersion.Text = "";
            txtSmsNewlineReplacement.Text = "";
            txtSmsKey.Text = "";
            txtSmsLoginUsername.Text = "";
            txtSmsLoginPassword.Text = "";
            txtSmsProtocol.Text = "";
            txtSmsPort.Text = "";
            txtSmsServer.Text = "";
            txtSmsVoiceToken.Text = "";
            txtSmsPhoneNumberFormat.Text = "";
            txtSmsProviderCharge.Text = "";
            txtSmsClientCharge.Text = "";

            // Voice
            chkVoiceRequiresSsl.Checked = false;
            txtVoiceName.Text = "";
            txtVoiceApiVersion.Text = "";
            txtVoiceFromPhoneNumber.Text = "";
            txtVoiceKey.Text = "";
            txtVoiceLoginUsername.Text = "";
            txtVoiceLoginPassword.Text = "";
            txtVoicePort.Text = "";
            txtVoiceProtocol.Text = "";
            txtVoiceServer.Text = "";
            txtVoiceSID.Text = "";
            txtVoiceVoiceToken.Text = "";
            txtVoiceNewlineReplacement.Text = "";
            txtVoicePhoneNumberFormat.Text = "";
            txtVoiceProviderCharge.Text = "";
            txtVoiceClientCharge.Text = "";
        }

        protected void dlProviderTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            dlProviders.SelectedIndex = 0;

            btnUpdate.Disabled = true;

            if (dlProviderTypes.SelectedIndex > 0)
            {
                dlProviders.Visible = true;

                GetDefaultProviders(dlProviderTypes.SelectedValue);
            }
            else
            {
                dlProviders.Visible = false;
            }
        }
    }
}