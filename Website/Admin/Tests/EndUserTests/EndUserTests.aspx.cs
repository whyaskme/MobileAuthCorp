using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;

using MACSecurity;
using MACServices;
using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using sr = MACServices.Constants.ServiceResponse;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;

public partial class Admin_Tests_EndUserTests_EndUserTests : System.Web.UI.Page
{
    private const string Test = "User";
    private const string SelectClient = "Select Client";
    private const string NoClient = "No Clients";
    private const string SelectGroup = "Select Group";
    private const string NotSelected = "Not Selected";

    HiddenField _hiddenW;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Master != null)
        {
            _hiddenW = (HiddenField)Master.FindControl("hiddenW");
            _hiddenW.Value = "54a83cf2ead6362034d04bd5";
        }

        if (!IsPostBack)
        {
            GetClients();
            SetGroups();
            lbGroup.Visible = false;
            ddlGroups.Visible = false;
        }
    }

    public void btnComputeUserId_Click(object sender, EventArgs e)
    {
        if (String.IsNullOrEmpty(txtLastName.Text))
        {
            lbError.Text = @"Last Name required!";
            return;
        }
        if (String.IsNullOrEmpty(txtEmailAdr.Text))
        {
            lbError.Text = @"Email or Unique Id required!";
            return;
        }
        var mSelected = "Open ";

        var userid = (Security.GetHashString(txtLastName.Text.ToLower() + txtEmailAdr.Text.ToLower())).ToUpper();
        // Is it a client userid
        if (ddlClient.SelectedItem.Text != SelectClient)
        {
            if ((ddlGroups.SelectedItem.Text != SelectGroup) && (ddlGroups.SelectedItem.Text != NotSelected))
            {
                mSelected = "Selected Group " + ddlGroups.SelectedItem.Text +" Id:" + ddlGroups.SelectedValue;
                userid = Security.GetHashString(userid + ddlGroups.SelectedValue);
            }
            else
            {
                mSelected = "Selected Client " + ddlClient.SelectedItem.Text + " Id:" + ddlClient.SelectedValue;
                userid = Security.GetHashString(userid + ddlClient.SelectedValue);
            }
        } 
        AddToLogAndDisplay(mSelected + " User Id: " + userid);
    }

    public void btnCheckEndUserReg_Click(object sender, EventArgs e)
    {
        string userid;
        if (String.IsNullOrEmpty(txtSTSUserId.Text))
        {
            if (String.IsNullOrEmpty(txtLastName.Text))
            {
                lbError.Text = @"Last Name required!";
                return;
            }
            if (String.IsNullOrEmpty(txtEmailAdr.Text))
            {
                lbError.Text = @"Email or Unique Id required!";
                return;
            }
            userid = (Security.GetHashString(txtLastName.Text.ToLower() + txtEmailAdr.Text.ToLower())).ToUpper();
        }
        else
        {
            userid = txtSTSUserId.Text;
        }

        var cid = Constants.Strings.DefaultClientId;

        var mRequest = dk.Request + dk.KVSep + dv.CheckEndUserRegistration +
                        dk.ItemSep + dk.UserId + dk.KVSep + userid +
                        dk.ItemSep + dk.CID + dk.KVSep + cid;

        var url = ConfigurationManager.AppSettings[cfg.MacServicesUrl] +
                                  Constants.ServiceUrls.EndUserManagementWebService; 
        try
        {
            var dataStream = Encoding.UTF8.GetBytes("data=99" + cid.Length + cid.ToUpper() + StringToHex(mRequest));
            var webRequest = WebRequest.Create(url);
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
            var elemList = xmlDoc.GetElementsByTagName("macResponse");
            AddToLogAndDisplay(elemList[0].InnerXml.Replace("><", ">|<"));
        }
        catch (Exception ex)
        {
            AddToLogAndDisplay(ex.ToString());
            lbError.Text = @"Service error";
        }

    }

    public void ddlClient_Selected(object sender, EventArgs e)
    {
        lbError.Text = "";
        SetGroups();
    }

    #region Helpers
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
        lbGroup.Visible = false;
        ddlGroups.Visible = false;

        ddlGroups.Items.Clear();
        var values = ddlClient.SelectedValue.Split(char.Parse(dk.ItemSep));
        //if (values.Count() < 2)
        //{
        //    ddlGroups.Items.Add(NoGroups);
        //    _hiddenC.Value = "";
        //    return;
        //}
        var skipcid = true; // first entry is always the clientId
        foreach (var group in values)
        {
            var grouplist = new ListItem();
            if (skipcid)
            {
                grouplist.Text = grouplist.Value = NotSelected;
                skipcid = false;
            }
            else
            {
                lbGroup.Visible = true;
                ddlGroups.Visible = true;
                var name_id = group.Split('=');
                grouplist.Text = name_id[0];
                grouplist.Value = name_id[1];
            }
            ddlGroups.Items.Add(grouplist);
        }
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

    protected void btnClearLog_Click(object sender, EventArgs e)
    {
        Session["LogText"] = "";
        AddToLogAndDisplay("btnClearLog");
    }
    private void AddToLogAndDisplay(string textToAdd)
    {
        var newlog = String.Format("{0}|{1}.{2}", Session["LogText"], Test, textToAdd);
        Session["LogText"] = newlog;
        tbLog.Text = newlog.Replace("|", Environment.NewLine);
    }
    #endregion
}