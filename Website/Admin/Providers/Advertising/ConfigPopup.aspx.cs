using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

using MongoDB.Bson;
using MongoDB.Driver;

using MACSecurity;
using MACServices;

using dk = MACServices.Constants.Dictionary.Keys;
using cs = MACServices.Constants.Strings;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;

namespace MACAdmin.Clients.Providers.Advertising
{
    public partial class ConfigPopup : Page
    {
        public MongoClient MyMongoClient;
        public MongoServer MyMongoServer;

        public Client myClient;

        public string adClientId;
        public string adApiKey;
        public string adClientUserName;
        public string adClientPassword;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Deny access if user request not logged in
            var isAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
            if (!isAuthenticated)
            {
                Response.Write("Hello world!");
                Response.End();
            }

// ReSharper disable once NotAccessedVariable
            var loggedInAdminId = "";
            if (!String.IsNullOrEmpty(Request["loggedInAdminId"]))
// ReSharper disable once RedundantAssignment
                loggedInAdminId = Request["loggedInAdminId"];
            

            var clientId = "";
            if (!String.IsNullOrEmpty(Request["clientId"]))
                clientId = Request["clientId"];

            var userId = "";
            if (!String.IsNullOrEmpty(Request["userid"]))
                userId = Request["userid"];

            var userIsReadOnly = "";
            if (!String.IsNullOrEmpty(Request["userisreadonly"]))
                userIsReadOnly = Request["userisreadonly"];

            var decryptedUserId = MACSecurity.Security.DecodeAndDecrypt(userId, Constants.Strings.DefaultClientId);

            userIsReadOnly = Security.DecodeAndDecrypt(userIsReadOnly, decryptedUserId);

            myClient = new Client(clientId);

            updateMessage.Visible = false;

            if(IsPostBack)
            {
                updateMessage.Visible = true;

                if (Convert.ToBoolean(chkAdPassEnabled.Checked))
                {
                    if (myClient.AdProviders == null || myClient.AdProviders.Count < 1)
                    {
                        var myCampaign = new ProviderAdvertising();

                        var xmlDoc = GetAdPassClientInfo(myClient.Name);

                        adClientId = xmlDoc.GetElementsByTagName("clientid")[0].InnerText;
                        adApiKey = xmlDoc.GetElementsByTagName("apikey")[0].InnerText;
                        adClientUserName = xmlDoc.GetElementsByTagName("username")[0].InnerText;
                        adClientPassword = xmlDoc.GetElementsByTagName("password")[0].InnerText;

                        myCampaign.AdClientId = adClientId;
                        myCampaign.ApiKey = adApiKey;
                        myCampaign.UserName = adClientUserName;
                        myCampaign.Password = adClientPassword;

                        myClient.AdProviders = new List<ProviderAdvertising>();
                        myClient.AdProviders.Add(myCampaign);
                    }

                    if (hiddenAA.Value == "UpdateProvider")
                    {
                        myClient.AdProviders[0].Name = txtProviderName.Text;
                        myClient.AdProviders[0].AdClientId = txtAdClientId.Text;
                        myClient.AdProviders[0].ApiKey = txtApiKey.Text;
                        myClient.AdProviders[0].Password = txtApiPassword.Text;
                        myClient.AdProviders[0].UserName = txtApiUserName.Text;
                        myClient.AdProviders[0].ApiUrl = txtApiUrl.Text;

                        myClient.AdProviders[0].P1 = txtP1.Text;
                        myClient.AdProviders[0].P2 = txtP2.Text;
                        myClient.AdProviders[0].P3 = txtP3.Text;
                        myClient.AdProviders[0].P4 = txtP4.Text;

                        updateMessage.InnerHtml = "Advertising provider updated";
                    }
                    else
                        updateMessage.InnerHtml = "Enabled advertising provider";

                    myClient.AdEnabled = true;
                    myClient.Update();
                }
                else
                {
                    myClient.AdEnabled = false;
                    myClient.AdProviders.Clear();
                    myClient.Update();

                    updateMessage.InnerHtml = "Disabled advertising provider";
                }
            }

            SetAdvertisingDisplay(myClient.AdEnabled);

            if (Convert.ToBoolean(userIsReadOnly))
            {
                chkAdPassEnabled.Enabled = false;

                txtProviderName.Enabled = false;
                txtAdClientId.Enabled = false;
                txtApiUrl.Enabled = false;
                txtApiKey.Enabled = false;
                txtApiUserName.Enabled = false;
                txtApiPassword.Enabled = false;
                txtP1.Enabled = false;
                txtP2.Enabled = false;
                txtP3.Enabled = false;
                txtP4.Enabled = false;

                btnSave.Visible = false;
            }
        }

        private XmlDocument GetAdPassClientInfo(string clientName)
        {
            //var clientId = "";
            var xmlDoc = new XmlDocument();

            // Call AdPass to get their clientId and update accordingly
            var requestUrl = "http://localhost:8080/macservices/AdminServices/AdPassServices.asmx/WsGetAdPassClientInfoByName";

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(requestUrl);

                var postData = "clientName=" + myClient.Name;
                var data = Encoding.ASCII.GetBytes(postData);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd().ToString();

                xmlDoc.LoadXml(responseString);
            }
            catch(Exception ex)
            {
                var errMsg = ex.ToString();
            }

            return xmlDoc;
        }

        private void SetAdvertisingDisplay(bool isEnabled)
        {
            if (isEnabled)
            {
                if (myClient.AdProviders == null || myClient.AdProviders.Count < 1)
                {
                    var myCampaign = new ProviderAdvertising();

                    myClient.AdProviders = new List<ProviderAdvertising>();
                    myClient.AdProviders.Add(myCampaign);
                    myClient.Update();
                }

                chkAdPassEnabled.Checked = true;
                btnSave.Disabled = false;

                txtProviderName.Enabled = true;
                txtAdClientId.Enabled = true;
                txtApiUrl.Enabled = true;
                txtApiKey.Enabled = true;
                txtApiUserName.Enabled = true;
                txtApiPassword.Enabled = true;
                txtP1.Enabled = true;
                txtP2.Enabled = true;
                txtP3.Enabled = true;
                txtP4.Enabled = true;

                txtProviderName.Text = myClient.AdProviders[0].Name;
                txtAdClientId.Text = myClient.AdProviders[0].AdClientId;
                txtApiKey.Text = myClient.AdProviders[0].ApiKey;
                txtApiPassword.Text = myClient.AdProviders[0].Password;
                txtApiUserName.Text = myClient.AdProviders[0].UserName;
                txtApiUrl.Text = myClient.AdProviders[0].ApiUrl;

                txtP1.Text = myClient.AdProviders[0].P1;
                txtP2.Text = myClient.AdProviders[0].P2;
                txtP3.Text = myClient.AdProviders[0].P3;
                txtP4.Text = myClient.AdProviders[0].P4;
            }
            else
            {
                chkAdPassEnabled.Checked = false;
                btnSave.Disabled = true;

                txtProviderName.Enabled = false;
                txtAdClientId.Enabled = false;
                txtApiUrl.Enabled = false;
                txtApiKey.Enabled = false;
                txtApiUserName.Enabled = false;
                txtApiPassword.Enabled = false;
                txtP1.Enabled = false;
                txtP2.Enabled = false;
                txtP3.Enabled = false;
                txtP4.Enabled = false;

                txtProviderName.Text = "";
                txtAdClientId.Text = "";
                txtApiKey.Text = "";
                txtApiPassword.Text = "";
                txtApiUserName.Text = "";
                txtApiUrl.Text = "";

                txtP1.Text = "";
                txtP2.Text = "";
                txtP3.Text = "";
                txtP4.Text = "";
            }
        }
    }
}