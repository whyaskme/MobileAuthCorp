using System;
using System.Web;
using System.Web.UI;
using MACServices;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace MACAdmin.Providers.UserVerification
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

            var userVerificationProvider = new VerificationProvider();

            var providerName = HttpContext.Current.Request["providername"];

            var userIsReadOnly = "";
            if (!String.IsNullOrEmpty(Request["userisreadonly"]))
                userIsReadOnly = Request["userisreadonly"];

            switch (providerName)
            {
                case "Lexis Nexus":
                    userVerificationProvider = new UserVerificationLexisNexis();
                    break;

                case "White Pages":
                    userVerificationProvider = new UserVerificationWhitePages();
                    break;
            }

            if (IsPostBack)
            {
                #region Postback
                userVerificationProvider.Name = txtName.Text;
                userVerificationProvider.ApiKey = txtApiKey.Text;
                userVerificationProvider.ApiVersion = txtApiVersion.Text;
                userVerificationProvider.BaseUrl = txtBaseUrl.Text;
                userVerificationProvider.Login = txtLogin.Text;
                userVerificationProvider.Password = txtPassword.Text;
                userVerificationProvider.SearchType = txtSearchType.Text;
                userVerificationProvider.Protocol = txtProtocol.Text;
                userVerificationProvider.ProxyHost = txtProxyHost.Text;
                userVerificationProvider.ProxyPort = txtProxyPort.Text;
                userVerificationProvider.RequestParameters = txtRequestParameters.Text;
                userVerificationProvider.ServiceName = txtServiceName.Text;
                userVerificationProvider.ProcessingParameters = txtProcessingParameters.Text;
            
                Session["VerificationProviders"] = userVerificationProvider;

                ClientScript.RegisterStartupScript(typeof(Page), "closePage", "<script type='text/JavaScript'>window.close();</script>");
                #endregion
            }
            else
            {
                #region !Postback
                var mongoDBConnectionPool = (MongoDatabase)HttpContext.Current.Application[Constants.Strings.MongoDB];
                MongoCollection mongoCollection = mongoDBConnectionPool.GetCollection("TypeDefinitions");

                // Get the selected global provider settings. These are default and should be customized and saved in the client.
                var providerId = ObjectId.GenerateNewId();
                if (!String.IsNullOrEmpty(HttpContext.Current.Request["providerID"]))
                    providerId = ObjectId.Parse(HttpContext.Current.Request["providerID"]);

                if (!String.IsNullOrEmpty(HttpContext.Current.Request["clientId"]))
                {
                    // Get available ProviderEmail provider config
                    var query = Query.EQ("_id", providerId);
                    var userVerificationProviders = mongoCollection.FindAs<UserVerificationProvider>(query);
                    if (userVerificationProviders != null)
                    {
                        foreach (var verificationProvider in userVerificationProviders)
                        {
                            spanProviderName.InnerHtml = verificationProvider.Name + " User Verification Configuration";
                            txtName.Text = userVerificationProvider.Name;
                            txtApiKey.Text = userVerificationProvider.ApiKey;
                            txtApiVersion.Text = userVerificationProvider.ApiVersion;
                            txtBaseUrl.Text = userVerificationProvider.BaseUrl;
                            txtLogin.Text = userVerificationProvider.Login;
                            txtPassword.Text = userVerificationProvider.Password;
                            txtSearchType.Text = userVerificationProvider.SearchType;
                            txtProtocol.Text = userVerificationProvider.Protocol;
                            txtProxyHost.Text = userVerificationProvider.ProxyHost;
                            txtProxyPort.Text = userVerificationProvider.ProxyPort;
                            txtRequestParameters.Text = userVerificationProvider.RequestParameters;
                            txtServiceName.Text = userVerificationProvider.ServiceName;
                            txtProcessingParameters.Text = userVerificationProvider.ProcessingParameters;
                        }
                    }
                }
                else
                {
                    spanProviderName.InnerHtml = userVerificationProvider.Name + " User Verification Configuration";
                    txtName.Text = userVerificationProvider.Name;
                    txtApiKey.Text = userVerificationProvider.ApiKey;
                    txtApiVersion.Text = userVerificationProvider.ApiVersion;
                    txtBaseUrl.Text = userVerificationProvider.BaseUrl;
                    txtLogin.Text = userVerificationProvider.Login;
                    txtPassword.Text = userVerificationProvider.Password;
                    txtSearchType.Text = userVerificationProvider.SearchType;
                    txtProtocol.Text = userVerificationProvider.Protocol;
                    txtProxyHost.Text = userVerificationProvider.ProxyHost;
                    txtProxyPort.Text = userVerificationProvider.ProxyPort;
                    txtRequestParameters.Text = userVerificationProvider.RequestParameters;
                    txtServiceName.Text = userVerificationProvider.ServiceName;
                    txtProcessingParameters.Text = userVerificationProvider.ProcessingParameters;
                }
                #endregion
            }

            if(Convert.ToBoolean(userIsReadOnly))
            {

            }
        }
    }
}