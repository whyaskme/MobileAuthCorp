using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using MACServices;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Admin.Providers.UserVerification
{
    public partial class VerificationSelectionPopup : Page
    {
        public StringBuilder sbResponse = new StringBuilder();

        public string userIsReadOnly = "";

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

            var mongoDBConnectionPool = (MongoDatabase)HttpContext.Current.Application[Constants.Strings.MongoDB];
            var clientId = "";
            if (!String.IsNullOrEmpty(HttpContext.Current.Request["clientId"]))
                clientId = HttpContext.Current.Request["clientId"];
            
            if (!String.IsNullOrEmpty(Request["userisreadonly"]))
                userIsReadOnly = Request["userisreadonly"];

            var myClient = new Client(clientId);

            if (IsPostBack)
            {
                // Clear all verification providers before reprocessing
                myClient.VerificationProviders.Clear();

                MongoCollection mongoCollection = mongoDBConnectionPool.GetCollection("TypeDefinitions");

                // Loop through the selected providers and set in the session accordingly
                if (hiddenSelectedProviderIds.Value.IndexOf("|", StringComparison.Ordinal) > -1)
                {
                    var selectedProviders = hiddenSelectedProviderIds.Value.Split('|');
                    foreach (var currentProviderId in selectedProviders)
                    {
                        if (currentProviderId != "")
                        {
                            var cleanedProviderId = currentProviderId.Replace("chk_", "");

                            // Get the current provider's info
                            var query = Query.EQ("_id", ObjectId.Parse(cleanedProviderId));
                            var userVerificationProviders = mongoCollection.FindAs<VerificationProvider>(query);
                            foreach (VerificationProvider currentProvider in userVerificationProviders)
                            {
                                switch (currentProvider.Name)
                                {
                                    case "Lexis Nexis":
                                        currentProvider.Name = "Lexis Nexis";
                                        currentProvider.ApiKey = "n/a";
                                        currentProvider.ApiVersion = "1.65";
                                        currentProvider.BaseUrl = "https://wsonline.seisint.com/WsIdentity/InstantID";
                                        currentProvider.Login = "MACXML";
                                        currentProvider.Password = "up83525H";
                                        currentProvider.Protocol = "SOAP";
                                        currentProvider.ProxyHost = "http://proxy.mobileauthcorp.com";
                                        currentProvider.ProxyPort = "80";
                                        currentProvider.RequestParameters = "GLBPurpose=5|DLPurpose=3|UseDOBFilter=1|DOBRadius=3";
                                        currentProvider.ProcessingParameters = "";
                                        currentProvider.SearchType = "";
                                        currentProvider.ServiceName = "";
                                        break;

                                    case "White Pages":
                                        currentProvider.Name = "White Pages";
                                        currentProvider.ApiKey = "defa7058bc71ada1b60000216700c1d2";
                                        currentProvider.ApiVersion = "1.1";
                                        currentProvider.BaseUrl = "http://proapi.whitepages.com";
                                        currentProvider.Login = "";
                                        currentProvider.Password = "";
                                        currentProvider.Protocol = "";
                                        currentProvider.ProxyHost = "http://proxy.mobileauthcorp.com";
                                        currentProvider.ProxyPort = "80";
                                        currentProvider.SearchType = "reverse_phone";
                                        currentProvider.ServiceName = "";
                                        break;
                                }
                                // Add the current provider to the client
                                myClient.VerificationProviders.Add(currentProvider);
                            }
                        }
                    }
                }

                myClient.Update();

                assignVerificationProvidersMessage.Visible = true;
                assignVerificationProvidersMessage.InnerHtml = "User Verification providers assigned";

                // Log the changes
                //var providerEvent = new Event
                //{
                //    ClientId = myClient._id,
                //    UserId = ObjectId.Parse(loggedInAdminId),
                //    EventTypeDesc = Constants.TokenKeys.ProviderName + updatedProvider.Name
                //                    + Constants.TokenKeys.ProviderType + "User Verification"
                //                    + Constants.TokenKeys.ProviderChangedValues + differences
                //                    + Constants.TokenKeys.ClientName + myClient.Name
                //};

                //providerEvent.Create(Constants.EventLog.Providers.Updated, null);
            }
            else
            {
                assignVerificationProvidersMessage.Visible = false;
            }

            divVerificationContainer.InnerHtml = "";

            GetProviders(myClient);

            if(Convert.ToBoolean(userIsReadOnly))
            {
                btn_save.Visible = false;
            }
        }

        private void GetProviders(Client myClient)
        {
            var mongoDBConnectionPool = (MongoDatabase)HttpContext.Current.Application[Constants.Strings.MongoDB];

            divVerificationContainer.InnerHtml = "";

            sbResponse.Clear();
            dlGlobalVerificationProviders.Items.Clear();

            MongoCollection mongoCollection = mongoDBConnectionPool.GetCollection("TypeDefinitions");

            // Get all default providers
            var query = Query.EQ("_t", "VerificationProvider");
            var userVerificationProviders = mongoCollection.FindAs<VerificationProvider>(query);
            foreach (VerificationProvider currentProvider in userVerificationProviders)
            {
                // Loop through the default providers and if one is client selected, set display accordingly
                ListItem li = new ListItem();
                li.Text = currentProvider.Name;
                li.Value = currentProvider._id.ToString();
                dlGlobalVerificationProviders.Items.Add(li);
            }

            int iCurrentIndex = 0;

            string[] arrClientSelectedProviders = new string[1];

            if (myClient.VerificationProviders != null)
            {
                arrClientSelectedProviders = new string[myClient.VerificationProviders.Count];
                foreach (VerificationProvider selectedProvider in myClient.VerificationProviders)
                {
                    arrClientSelectedProviders[iCurrentIndex] = selectedProvider.Name;
                    iCurrentIndex++;
                }
            }

            foreach (ListItem li in dlGlobalVerificationProviders.Items)
            {
                if (arrClientSelectedProviders.Contains(li.Text))
                {
                    sbResponse.Append("         <div id='divProviderContainer_" + li.Value + "'>");

                    if (Convert.ToBoolean(userIsReadOnly))
                        sbResponse.Append("             <input id='chk_" + li.Value + "' onclick='javascript: setSelectedVerificationProviders(this);' disabled='disabled' type='checkbox' checked='checked' />");
                    else
                        sbResponse.Append("             <input id='chk_" + li.Value + "' onclick='javascript: setSelectedVerificationProviders(this);' type='checkbox' checked='checked' />");

                    sbResponse.Append("             <span class='title_875rem' style='position:relative;top: -1px;'>");
                    sbResponse.Append(li.Text);
                    sbResponse.Append("             </span>");
                    sbResponse.Append("         </div>");

                    hiddenSelectedProviderIds.Value = "chk_" + li.Value + "|";

                }
                else
                {
                    sbResponse.Append("         <div id='divProviderContainer_" + li.Value + "'>");

                    if (Convert.ToBoolean(userIsReadOnly))
                        sbResponse.Append("             <input id='chk_" + li.Value + "' onclick='javascript: setSelectedVerificationProviders(this);' disabled='disabled' type='checkbox' />");
                    else
                        sbResponse.Append("             <input id='chk_" + li.Value + "' onclick='javascript: setSelectedVerificationProviders(this);' type='checkbox' />");

                    sbResponse.Append("             <span class='title_875rem' style='position:relative;top: -1px;'>");
                    sbResponse.Append(li.Text);
                    sbResponse.Append("             </span>");
                    sbResponse.Append("         </div>");

                }
            }

            divVerificationContainer.InnerHtml = sbResponse.ToString();
        }

// ReSharper disable once UnusedMember.Local
        static Predicate<ProviderEmail> FindProviderById(string providerId)
        {
            return provider => provider._id == ObjectId.Parse(providerId);
        }
    }
}