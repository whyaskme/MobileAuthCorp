using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;
using MACSecurity;
using MACServices;
using Newtonsoft.Json;
using System.Web;

using dk = MACServices.Constants.Dictionary.Keys;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;

namespace MACUserApps.Web.Tests.Client
{
    public partial class MacUserAppsWebTestsclientclientTests : System.Web.UI.Page
    {
        //public static string MacServicesUrl = ConfigurationManager.AppSettings[cfg.MacServicesUrl];
        public static string MyId = ConfigurationManager.AppSettings["MacServicesUrl"];
        private const string SelectClient = "Select Client";
        private const string NoClient = "No Clients";
        private const string SelectGroup = "Select Group";
        private const string NoGroups = "No Groups";

        private HiddenField _hiddenC;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.Master != null)
            {
                _hiddenC = (HiddenField)Page.Master.FindControl("hiddenC");
            }
            if (!IsPostBack)
            {
                var myIP = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (String.IsNullOrEmpty(myIP))
                    myIP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                //txtIP.Text = myIP;
                AddToLogAndDisplay("btnClientTests_Click");
                GetClients();
                SetGroups();
            }
        }

        public void ddlClient_Selected(object sender, EventArgs e)
        {
            SetGroups();
            AddToLogAndDisplay("ddlClient_Selected: " + ddlClient.SelectedValue);
            GetClient();
        }


        private void GetClients()
        {
            try
            {
                var request = WebRequest.Create(ConfigurationManager.AppSettings[cfg.MacServicesUrl] +
                                TestLib.TestConstants.GetTestClientsInfoUrl);
                request.Method = "Post";
                request.ContentLength = 0;
                var res = request.GetResponse();
                var response = res.GetResponseStream();
                var xmlDoc = new XmlDocument();
                if (response != null) xmlDoc.Load(response);
                var elemList = xmlDoc.GetElementsByTagName("Error");
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
                            li.Value += @"|" + gnode.ChildNodes[0].InnerText;
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
            if (ddlClient.SelectedValue == SelectClient)
                return;
            ddlGroups.Items.Clear();
            var values = ddlClient.SelectedValue.Split('|');
            if (values.Count() < 2)
            {
                ddlGroups.Items.Add(NoGroups);
                return;
            }
            // first value is the client id
            var first = true;
            foreach (var group in values)
            {
                // replace client is with "none"
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

        private string GetSelectedClientId()
        {
            var idGroups = ddlClient.SelectedValue.Split(':');
            return idGroups[0];
        }

        protected void GetClient()
        {
            AddToLogAndDisplay("GetClient:" + ddlClient.SelectedItem.Text);
            var clientName = ddlClient.SelectedItem.Text;
            var clientId = GetSelectedClientId();

            var myData = new Dictionary<string, string> {{"_id", clientId}, {"Name", clientName}};
            // make the request
            var result = ClientServicesRequest(clientId, "GetClient", clientName, JsonConvert.SerializeObject(myData));
            // isloate the id from the data
            var mUtils = new Utils();
            var ret = mUtils.GetIdDataFromRequest(result);
            if (ret.Item1 == null)
            {
                AddToLogAndDisplay("GetClient: Error");
                Session["ClientData"] = "";
            }
            else
            {
                AddToLogAndDisplay("GetClient: ID=" + ret.Item1);
                Session["ClientData"] = Security.DecodeAndDecrypt(ret.Item2, ret.Item1);
                if (clientName.Trim().StartsWith("Mobile Authentication"))
                {
                    btnDisableClient.Visible = false;
                    btnEnableClient.Visible = false;
                    btnDeleteClient.Visible = false;
                } else
                {
                    btnDisableClient.Visible = true;
                    btnEnableClient.Visible = true;
                    btnDeleteClient.Visible = true;
                }
            }
            btnShowClientData.Visible = true;
        }

        protected void btnCheckIp_Click(object sender, EventArgs e)
        {
            lbError.Text = "";
            var mUtils = new Utils();
            //if (String.IsNullOrEmpty(txtIP.Text))
            //{
            //    lbError.Text = @"Enter an IP to check";
            //    return;
            //}
            if (ddlClient.SelectedItem.Text != SelectClient)
            {
                var mClient = mUtils.GetClientUsingClientName(ddlClient.SelectedItem.Text);
                if (mClient == null)
                {
                    lbError.Text = @"Could not get client";
                    return;            
                }
                lbError.Text = @"Not implemented";
                return;
            }

            //// client not supplied, check IP against entered list
            //if (String.IsNullOrEmpty(txtIPList.Text))
            //{
            //    lbError.Text = @"Enter a list to check";
            //    return;
            //}
            //var mResult = mUtils.CheckIpList(txtIP.Text, txtIPList.Text);
            //if (mResult.Item1 == false)
            //    lbError.Text = @"No match ";
            //AddToLogAndDisplay("Ip[" + txtIP.Text + "] result " + mResult.Item2);

        }

        protected void btnShowClientData_Click(object sender, EventArgs e)
        {
            if (Session["ClientData"] == null)
            {
                tbClientData.Text = @"No Client Selected!";
                return;
            }
            var data = Session["ClientData"].ToString();
            tbClientData.Text = System.Xml.Linq.XDocument.Parse(data).ToString();
        }

        protected void ddlCD_Selected(object sender, EventArgs e)
        {
            //tbCDData.Text = ddlCD.SelectedValue;
        }

        protected void btnCD_Update_Clicked(object sender, EventArgs e)
        {
            AddToLogAndDisplay("btnCD_Update_Clicked: removed");
        }

        protected void btnCreateClient_Click(object sender, EventArgs e)
        {
            AddToLogAndDisplay("btnCreateClient: removed");
        }

        protected void btnUpdateClient_Click(object sender, EventArgs e)
        {
            AddToLogAndDisplay("btnUpdateClient:" + ddlClient.SelectedItem.Text);
        }

        protected void btnDisableClient_Click(object sender, EventArgs e)
        {
            AddToLogAndDisplay("btnDisableClient:" + ddlClient.SelectedItem.Text);
            var clientName = ddlClient.SelectedItem.Text;
            var clientId = GetSelectedClientId();

            var myData = new Dictionary<string, string> {{"_id", clientId}, {"Name", clientName}};

            var result = ClientServicesRequest(clientId, "DisableClient", clientName, JsonConvert.SerializeObject(myData));
            AddToLogAndDisplay(result);
        }

        protected void btnEnableClient_Click(object sender, EventArgs e)
        {
            AddToLogAndDisplay("btnEnableClient:" + ddlClient.SelectedItem.Text);
            var clientName = ddlClient.SelectedItem.Text;
            var clientId = GetSelectedClientId();

            var myData = new Dictionary<string, string> {{"_id", clientId}, {"Name", clientName}};

            var result = ClientServicesRequest(clientId, "EnableClient", clientName, JsonConvert.SerializeObject(myData));
            AddToLogAndDisplay(result);

        }
        protected void btnDeleteClient_Click(object sender, EventArgs e)
        {
            AddToLogAndDisplay("btnDeleteClient:" + ddlClient.SelectedItem.Text);
            var clientName = ddlClient.SelectedItem.Text;
            var clientId = GetSelectedClientId();

            var myData = new Dictionary<string, string> {{"_id", clientId}, {"Name", clientName}};

            string result = ClientServicesRequest(clientId, "DeleteClient", clientName, JsonConvert.SerializeObject(myData));
            AddToLogAndDisplay(result);

        }

        private string ClientServicesRequest(string pId, string pRequest, string pClientName, String pSerDict)
        {
            var myData = String.Format("Data={0}{1}{2}", pId.Length, pId.ToUpper(), Security.EncryptAndEncode(
                "Request=" + pRequest + "|ClientName=" + pClientName + "|Dict=" + pSerDict, pId.ToUpper()));
            try
            {
                var dataStream = Encoding.UTF8.GetBytes(myData);
                var request = ConfigurationManager.AppSettings[cfg.MacServicesUrl] +
                                            TestLib.TestConstants.ClientServicesUrl;
                var webRequest = WebRequest.Create(request);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.ContentLength = dataStream.Length;
                var newStream = webRequest.GetRequestStream();
                // Send the data.
                newStream.Write(dataStream, 0, dataStream.Length);
                newStream.Close();
                var res = webRequest.GetResponse();
                var response = res.GetResponseStream();
                var xmlDoc = new XmlDocument();
                if (response != null) xmlDoc.Load(response);
                var elemList = xmlDoc.GetElementsByTagName("Error");
                if (elemList.Count != 0)
                {
                    return String.Format("Error: returned from service {0}", elemList[0].InnerXml);
                }
                elemList = xmlDoc.GetElementsByTagName("Reply");
                return elemList[0].InnerXml;
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        public void btnExit_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Default.aspx");
        }

        private void AddToLogAndDisplay(string textToAdd)
        {
            var newlog = String.Format("{0}|ClientTests.{1}", Session["LogText"], textToAdd);
            Session["LogText"] = newlog;
            tbLog.Text = newlog.Replace("|", Environment.NewLine);
        }
    }
}