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
        public Utils myUtils = new Utils();

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
                if (string.IsNullOrEmpty(txtSecureTradingOperatorId.Text.Trim()) && string.IsNullOrEmpty(txtSecureTradingSiteId.Text.Trim()))
                {
                    // Go ahead and update to empty. 
                    myClient.SecureTradingSiteId = "";
                    myClient.Update();

                    updateMessage.Visible = true;

                    updateMessage.Style.Add("background-color", "#43ac6a;");
                    updateMessage.Style.Add("border-color", "#43ac6a;");

                    updateMessage.InnerHtml = "SecureTrading SiteId set to 'Empty' for " + myClient.Name;
                }
                else
                {
                    // Check to see if Operator|SiteId is already assigned to another client
                    var tmpSiteId = myUtils.GetMACClientIdBySTSiteId(txtSecureTradingOperatorId.Text.Trim(), txtSecureTradingSiteId.Text.Trim());

                    if (String.IsNullOrEmpty(tmpSiteId))
                    {
                        myClient.SecureTradingSiteId = txtSecureTradingOperatorId.Text + "|" + txtSecureTradingSiteId.Text;
                        myClient.Update();

                        updateMessage.Visible = true;

                        updateMessage.Style.Add("background-color", "#43ac6a;");
                        updateMessage.Style.Add("border-color", "#43ac6a;");

                        updateMessage.InnerHtml = "SecureTrading SiteId (" + txtSecureTradingSiteId.Text + ") added to " + myClient.Name;
                    }
                    else
                    {
                        var tmpVal = tmpSiteId.Split('|');
                        var tmpClientName = tmpVal[1];

                        updateMessage.Visible = true;

                        updateMessage.Style.Add("background-color", "#ff0000;");
                        updateMessage.Style.Add("border-color", "#ff0000;");

                        updateMessage.InnerHtml = "SecureTrading SiteId (<b>" + txtSecureTradingSiteId.Text + "</b>) is already being used by client (<b>" + tmpClientName + "</b>).";
                        updateMessage.InnerHtml += "&nbsp;Please try another SiteId.";

                        //txtSecureTradingSiteId.Text = "";
                        txtSecureTradingSiteId.Focus();
                    }
                }
            }
            else
            {
                var tmpOperatorId = "";
                var tmpSiteId = "";

                if (myClient.SecureTradingSiteId.Contains("|"))
                {
                    var tmpVal = myClient.SecureTradingSiteId.Split('|');
                    tmpOperatorId = tmpVal[0];
                    tmpSiteId = tmpVal[1];
                }
                else
                {
                    tmpSiteId = myClient.SecureTradingSiteId;
                }

                txtSecureTradingOperatorId.Text = tmpOperatorId;
                txtSecureTradingSiteId.Text = tmpSiteId;
            }
        }
    }
}