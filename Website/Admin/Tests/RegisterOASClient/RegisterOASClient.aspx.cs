using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web.UI.WebControls;
using System.Xml;

using dk = MACServices.Constants.Dictionary.Keys;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using dv = MACServices.Constants.Dictionary.Values;
using sr = MACServices.Constants.ServiceResponse;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;

namespace MACUserApps.Web.Tests.RegisterOASClient
{
    public partial class MacUserAppsWebTestsRegisterOasClientRegisterOasClient : System.Web.UI.Page
    {
        public static string Test = "OAS";

        private const string SelectClient = "Select Client";
        private const string NoClient = "No Clients";

        public static string MyWho = "Test";

        HiddenField _hiddenW;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.Master != null)
            {
                _hiddenW = (HiddenField)Master.FindControl("hiddenW");
                _hiddenW.Value = "54a83b60ead6362034d04bcb";
            }

            Session["LogText"] = "";
            if (!IsPostBack)
            {
                GetClients();
                SetGroups();
                ddlGroups.Visible = false;
            }
        }

        public void ddlClient_Selected(object sender, EventArgs e)
        {
            SetGroups();
        }


        private void GetClients()
        {
            try
            {
                var request = WebRequest.Create(ConfigurationManager.AppSettings[cfg.MacServicesUrl] + TestLib.TestConstants.GetTestClientsInfoUrl);
                request.Method = "Post";
                request.ContentLength = 0;
                var res = request.GetResponse();
                var response = res.GetResponseStream();
                var xmlDoc = new XmlDocument();
                if (response != null) xmlDoc.Load(response);
                var elemList = xmlDoc.GetElementsByTagName(sr.Error);
                if (elemList.Count != 0)
                {
                    lbError.Text = String.Format("Error: returned from GetClients service {0}", elemList[0].InnerXml);
                    return;
                }

                ddlClient.Items.Clear();
                elemList = xmlDoc.GetElementsByTagName("Client");
                if (elemList.Count != 0)
                {
                    var li1 = new ListItem
                    {
                        Text = SelectClient,
                        Value = SelectClient
                    };
                    ddlClient.Items.Add(li1);
                    foreach (XmlNode myClientNode in elemList)
                    {
                        var li = new ListItem
                        {
                            Text = myClientNode.ChildNodes[0].InnerXml,
                            Value = myClientNode.ChildNodes[1].InnerXml
                        };
                        var gelemList = myClientNode.ChildNodes[2].ChildNodes;
                        foreach (XmlNode gnode in gelemList)
                        {
                            li.Value += dk.ItemSep + gnode.ChildNodes[0].InnerText;
                        }
                        ddlClient.Items.Add(li);
                    }
                }
                else
                {
                    var li0 = new ListItem
                    {
                        Text = NoClient,
                        Value = NoClient
                    };
                    ddlClient.Items.Add(li0);
                }
            }
            catch (Exception ex)
            {
                AddToLogAndDisplay(ex.ToString());
                lbError.Text = @"GetClients service error";
            }
        }


        private void SetGroups()
        {
            ddlGroups.Items.Clear();
            string[] values = ddlClient.SelectedValue.Split(char.Parse(dk.ItemSep));
            if (values[0] == SelectClient)
            {
                tbClientID.Text = "";
                tbFullyQualifiedDomainName.Text = "";
                return;
            }
            tbClientID.Text = values[0];
            var cname = ddlClient.SelectedItem.Text;
            var idx = ddlClient.SelectedItem.Text.IndexOf("(", System.StringComparison.Ordinal);
            if (idx > 0)
                cname = ddlClient.SelectedItem.Text.Substring(0, idx);
            tbFullyQualifiedDomainName.Text = Uri.EscapeDataString(@"www." + cname.Replace(" ", "") + @".com");

            if (values.Count() < 2)
            {
                ddlGroups.Items.Add("No groups");
                return;
            }
            var first = true;

            foreach (var group in values)
            {
                if (first)
                {
                    first = false;
                    ddlGroups.Items.Add("None");
                }
                else
                {
                    ddlGroups.Items.Add(group);
                }
            }
        }

        protected void btnRequest_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            var text = btn.Text;
            var cname = ddlClient.SelectedItem.Text;
            var idx = ddlClient.SelectedItem.Text.IndexOf("(", System.StringComparison.Ordinal);
            if (idx > 0)
                cname = ddlClient.SelectedItem.Text.Substring(0, idx);
            AddToLogAndDisplay(ConfigurationManager.AppSettings[cfg.MacOpenServicesUrl]);

            var mReq = string.Format(dk.Request + ":{0}|" + dk.Name + ":{1}|" + dk.FullyQualifiedDomainName + ":{2}|" + dk.CID + ":{3}",
                text,
                cname,
                tbFullyQualifiedDomainName.Text,
                tbClientID.Text);
 
            AddToLogAndDisplay(mReq.Replace(dk.ItemSep, ", "));

            var myReg = new MacRegistration.MacRegistration();
            var reply = myReg.RegisterOasClient(
                ConfigurationManager.AppSettings[cfg.MacOpenServicesUrl],
                tbClientID.Text, mReq);

            AddToLogAndDisplay(reply);
        }
   
        private void AddToLogAndDisplay(string textToAdd)
        {
            var newlog = String.Format("{0}|{1}.{2}", Session["LogText"], Test, textToAdd);
            Session["LogText"] = newlog;
            tbLog.Text = newlog.Replace("|", Environment.NewLine);
        }
    }
}