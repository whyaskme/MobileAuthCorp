using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;

using cs = MACServices.Constants.Strings;
using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using sr = MACServices.Constants.ServiceResponse;

public partial class Admin_Tests_DemoTests_DemoTests : System.Web.UI.Page
{
    private const string Test = "Demo";
    private const string DemoUrl = "http://localhost:8080/DemoRegistration/DemoRegistration.aspx";
    private const string SelectClient = "Select Client";
    private const string NoClient = "No Client";
    private const string QS_demo = "demo";
    private const string QS_cid = "cid";
    private const string QS_action = "action";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack) return;
        txtDemoUrl.Text = DemoUrl;
        GetClients();
    }
    
    protected void btnRegDemo_Click(object sender, EventArgs e)
    {
        var clickedButton = sender as Button;
        if (clickedButton == null) return;
        AddToLogAndDisplay(clickedButton.ID);
        lbError.Text = "";
        var action = clickedButton.ID.Replace("btnDemo", "").ToLower();

        if (ddlClient.SelectedItem.Text == SelectClient)
        {
            lbError.Text = @"Select a demo client!";
            return;
        }
        var cid = ddlClient.SelectedValue;
        var qstring = txtDemoUrl.Text +
            "?" + QS_action + "=" + action +
            "&" + QS_demo + "=Test" +
            "&" + QS_cid + "=" + cid;
        Response.Redirect(qstring, false);
    }

    #region helpers

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
            var elemList = xmlDoc.GetElementsByTagName(sr.Error);
            if (elemList.Count != 0)
            {
                lbError.Text = String.Format("Error: returned from GetClients service {0}", elemList[0].InnerXml);
                return;
            }

            ddlClient.Items.Clear();
            elemList = xmlDoc.GetElementsByTagName(dv.Client);
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

    protected void btnClearLog_Click(object sender, EventArgs e)
    {
        Session["LogText"] = "";
        AddToLogAndDisplay("btnClearLog");
    }

    private void AddToLogAndDisplay(string textToAdd)
    {
        var newlog = String.Format("{0}|{1}.{2}", Session["LogText"], Test, textToAdd.Replace("&apos;", "'"));
        Session["LogText"] = newlog;
        tbLog.Text = newlog.Replace("|", Environment.NewLine);
    }

    public static string StringToHex(String input)
    {
        try
        {
            var values = input.ToCharArray();
            var output = new StringBuilder();
            foreach (var value in values.Select(Convert.ToInt32))
            {
                // Convert the decimal value to a hexadecimal value in string form. 
                output.Append(String.Format("{0:X}", value));
            }
            return output.ToString();
        }
        catch
        {
            return null;
        }
    }
    #endregion

}