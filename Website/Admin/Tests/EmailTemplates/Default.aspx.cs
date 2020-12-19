using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
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
using MongoDB.Driver.Builders;

using MACServices;
using MACSecurity;

using cfg = MACServices.Constants.WebConfig;

namespace EmailTemplates
{
    public partial class Default : System.Web.UI.Page
    {
        private HiddenField _hiddenD;
        private HiddenField _hiddenO;
        private HiddenField _hiddenC;
        private HiddenField _hiddenQ;
        private HiddenField _hiddenJ;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Deny access if user request not logged in
            var isAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
            if (!isAuthenticated)
            {
                Response.Write("Hello world!");
                Response.End();
            }

            Utils myUtils = new Utils();

            if (Page.Master != null)
            {
                _hiddenD = (HiddenField)Page.Master.FindControl("hiddenD");
                _hiddenO = (HiddenField)Page.Master.FindControl("hiddenO");
                _hiddenC = (HiddenField)Page.Master.FindControl("hiddenC");
                _hiddenQ = (HiddenField)Page.Master.FindControl("hiddenQ");
                _hiddenJ = (HiddenField)Page.Master.FindControl("hiddenJ");
            }

            divMsgContainer.Visible = false;

            if(IsPostBack)
            {
                if (dlClients.SelectedIndex == 1)
                {
                    var mongoCollection = myUtils.mongoDBConnectionPool.GetCollection("Client");
                    var clientCollection = mongoCollection.FindAllAs<Client>();

                    foreach (Client currentClient in clientCollection)
                    {
                        var rtn = myUtils.SendGenericEmail(currentClient._id.ToString(), "Client", Constants.Strings.DefaultFromEmail, txtEmailTo.Text, txtSubject.Text, txtBody.Value, true);
                        if (rtn == false)
                        {
                            //var tmpVal = "Something went wrong";
                        }
                    }
                }
                else
                {
                    var rtn = myUtils.SendGenericEmail(dlClients.SelectedValue, "Client", Constants.Strings.DefaultFromEmail, txtEmailTo.Text, txtSubject.Text, txtBody.Value, true);
                }

                divMsgContainer.Visible = true;
            }
            else
            {
                GetClientList();

                txtEmailTo.Text = _hiddenJ.Value;
                txtSubject.Text = "Test email templates";
                txtBody.Value = "Is the template formatting displaying all custom elements correctly?";
            }
        }

        public void GetClientList()
        {
            dlClients.Items.Clear();

            var clientList = new MacList("", "Client", "", "_id,Name");
            foreach (var li in clientList.ListItems.Select(item => new ListItem { Text = item.Attributes["Name"].Replace("&quot;", "\"").Trim(), Value = item.Attributes["_id"].Trim() }))
            {
                if (li.Text != "")
                {
                    dlClients.Items.Add(li);
                }
            }

            var li0 = new ListItem { Text = @"Select a Client tmplate (" + (dlClients.Items.Count) + @")", Value = Constants.Strings.DefaultEmptyObjectId };
            dlClients.Items.Insert(0, li0);

            var li1 = new ListItem { Text = @"Send all Client templates" };
            dlClients.Items.Insert(1, li1);
        }
    }
}


