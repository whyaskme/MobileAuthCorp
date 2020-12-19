using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;
using System.Web;

using MACSecurity;
using MACServices;
using dk = MACServices.Constants.Dictionary.Keys;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using dv = MACServices.Constants.Dictionary.Values;
using sr = MACServices.Constants.ServiceResponse;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;

using tc = TestLib.TestConstants.TestBank;

public partial class MACUserApps_Web_Tests_EndUserManagement_EndUserManagement : System.Web.UI.Page
{
    private const string Test = "Mgmt";
    //public static string MacServicesUrl = ConfigurationManager.AppSettings[cfg.MacServicesUrl];
    private const string SelectUser = "Select User";
    private const string NoTestFiles = "No Test Files";
    private const string SelectFile = "Select File";
    private const string SelectClient = "Select Client";
    private const string NoClient = "No Clients";
    private const string SelectGroup = "Select Group";
    private const string NoGroups = "No Groups";

    private Utils mUtils = new Utils();

    private HiddenField _hiddenC;
    HiddenField _hiddenW;

    protected void Page_Load(object sender, EventArgs e)
    {
        Session["LogText"] = "";
        if (Page.Master != null)
        {
            _hiddenC = (HiddenField)Page.Master.FindControl("hiddenC");

            _hiddenW = (HiddenField)Master.FindControl("hiddenW");
            _hiddenW.Value = "54a83b32ead6362034d04bca";
        }

        if (IsPostBack) return;
        AddToLogAndDisplay("btnAuth_Click");
        GetClients();
        SetGroups();
        FillddlTestUserFiles();
        {
            var li1 = new ListItem { Text = SelectUser, Value = SelectUser };
            ddlEndUserList.Items.Add(li1);
        }
    }

    #region Button event handlers
    protected void btnDeactivateEndUser_Click(object sender, EventArgs e)
    {
        lbError.Text = "";
        AddToLogAndDisplay("btnDeactivateEndUser");
        SendToEndUserManagementService(dk.Request + dk.KVSep + dv.DeactivateEndUser);
    }
    protected void btnActivateEndUser_Click(object sender, EventArgs e)
    {
        lbError.Text = "";
        AddToLogAndDisplay("btnActivateEndUser");
        SendToEndUserManagementService(dk.Request + dk.KVSep + dv.ActivateEndUser);
    }
    protected void btnAdPassEnable_Click(object sender, EventArgs e)
    {
        lbError.Text = "";
        AddToLogAndDisplay("btnAdPassEnable");
        SendToEndUserManagementService(dk.Request + dk.KVSep + dv.SetAdPassOption + dk.ItemSep + dk.AdPassOption + dk.KVSep + dv.AdEnable);
    }
    protected void btnAdPassDisable_Click(object sender, EventArgs e)
    {
        lbError.Text = "";
        AddToLogAndDisplay("btnAdPassDisable");
        SendToEndUserManagementService(dk.Request + dk.KVSep + dv.SetAdPassOption + dk.ItemSep + dk.AdPassOption + dk.KVSep + dv.AdDisable);
    }

    protected void btnChangeUserInfo_Click(object sender, EventArgs e)
    {
        lbError.Text = "";
        AddToLogAndDisplay("btnChangeUserInfo");
        var infoToChange = "";
        if (String.IsNullOrEmpty(txtPhoneNo.Text) == false)
            infoToChange = dk.ItemSep + dkui.PhoneNumber + dk.KVSep + txtPhoneNo.Text;

        if (String.IsNullOrEmpty(infoToChange))
        {
            lbError.Text = @"Nothing to change";
            return;
        }
        SendToEndUserManagementService(
            dk.Request + dk.KVSep + dv.UpdateEndUser + infoToChange);
    }

    protected void btnDeleteEndUser_Click(object sender, EventArgs e)
    {
        lbError.Text = "";
        AddToLogAndDisplay("btnDeleteEndUser_Click");
        SendToEndUserManagementService( dk.Request + dk.KVSep + dv.DeleteEndUser);
    }

    #endregion

    private void SendToEndUserManagementService(string pRequest)
    {
        // client Id
        var cid = GetSelectedClientId(ddlClient.SelectedValue);
        if (string.IsNullOrEmpty(cid))
        {
            lbError.Text = @"You must select a client!";
            return;
        } 

        if (ddlEndUserList.SelectedItem.Text == SelectUser)
        {
            lbError.Text = @"You must select a user!";
            return;
        }

        // Group Id if Group is selected
        var groupId = GetSelectedGroupId();

        // Get end user's last name
        var lastname = GetUserInfo(dkui.LastName, ddlEndUserList.SelectedItem.Value);

        // End user User Id
        var UserId = GetUserInfo(dk.UserId, ddlEndUserList.SelectedItem.Value);
        if (String.IsNullOrEmpty(UserId))
        {
            var uid = GetUserInfo(dkui.UID, ddlEndUserList.SelectedItem.Value);
            if (String.IsNullOrEmpty(uid))
                uid = GetUserInfo(dkui.EmailAddress, ddlEndUserList.SelectedItem.Value);
            UserId = (Security.GetHashString(lastname.ToLower() + uid.ToLower())).ToUpper();
        }

        var req = String.Format("|---------|Url={0}|CID={1}|GroupId={2}|UserId={3}|Request={4}|---------",
                        ConfigurationManager.AppSettings[cfg.MacServicesUrl], cid, groupId, UserId, pRequest);
        AddToLogAndDisplay(req);

        try
        {
            var myMacotp = new MacOtp.MacOtp();
            var sReply = myMacotp.SendManageRegisteredUser(ConfigurationManager.AppSettings[cfg.MacServicesUrl],
                cid, groupId, UserId, pRequest);
            AddToLogAndDisplay(sReply);
        }
        catch (Exception ex)
        {
            var exs = ex.ToString();
            AddToLogAndDisplay("EndUserManagement: Exception:" + exs);
            lbError.Text = ex.Message;
        }
    }

    #region helpers

    private string GetSelectedGroupId()
    {
        if (ddlGroups.Visible)
        {
            if (ddlGroups.SelectedValue != NoGroups)
            {
                if (ddlGroups.SelectedValue != SelectGroup)
                {
                    var gd = ddlGroups.SelectedValue.Split('|');
                    return gd[0];
                }
            }
        }
        return null;
    }

    /// <summary> Get selected Client Id </summary>
    private string GetSelectedClientId(string pClientDetails)
    {
        if (pClientDetails == SelectClient)
        {
            lbError.Text = @"You must select a client!";
            return null;
        }
        var cd = pClientDetails.Split(char.Parse(dk.ItemSep));
        return cd[0];
    }

    /// <summary> Get end user's last name </summary>
    private string GetUserInfo(string pKey, string pUserDetails)
    {
        var mDetails = pUserDetails.Split(char.Parse(dk.ItemSep));
        if (mDetails.Any())
        {
            foreach (var mDetail in mDetails)
            {
                if (mDetail.StartsWith(pKey))
                {
                    return mDetail.Replace(pKey + dk.KVSep, "");
                }
            }
        }
        return null;
    }

    private void FillddlTestUserFiles()
    {
        ddlTestUserFiles.Items.Clear();
        // get list of test files
        var path = HttpContext.Current.Server.MapPath("../");
        var folder = Path.Combine(path, TestLib.TestConstants.UserTestFiles);
        var d = new DirectoryInfo(folder);
        var Files = d.GetFiles("*.txt"); //Getting Text files

        if (!Files.Any())
        {
            var li = new ListItem();
            li.Text = li.Value = NoTestFiles;
            ddlTestUserFiles.Items.Add(li);
            return;
        }
        {
            var li = new ListItem();
            li.Text = li.Value = SelectFile;
            ddlTestUserFiles.Items.Add(li);
        }
        foreach (var file in Files)
        {
            var li = new ListItem();
            li.Text = li.Value = file.Name;
            ddlTestUserFiles.Items.Add(li);
        }
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

    public void ddlClient_Selected(object sender, EventArgs e)
    {
        lbError.Text = "";
        SetGroups();
        AddToLogAndDisplay("ClientSelected: " + ddlClient.SelectedItem + " " + ddlClient.SelectedItem.Value);

    }
    
    private void SetGroups()
    {

        lbGroup.Visible = false;
        ddlGroups.Visible = false;

        ddlGroups.Items.Clear();
        var values = ddlClient.SelectedValue.Split(Char.Parse(dk.ItemSep));
        if (values.Count() < 2)
        {
            ddlGroups.Items.Add(NoGroups);
            _hiddenC.Value = "";
            return;
        }
        var skipcid = true; // first entry is always the clientId
        foreach (var group in values)
        {
            var grouplist = new ListItem();
            if (skipcid)
            {
                grouplist.Text = grouplist.Value = SelectGroup;
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

    public void ddlEndUserList_Selected(object sender, EventArgs e)
    {
        AddToLogAndDisplay("Selected User Details: " + ddlEndUserList.SelectedValue.Replace(dk.ItemSep, ", "));
    }

    public void ddlTestUserFiles_Selected(object sender, EventArgs e)
    {
        if (ddlTestUserFiles.SelectedItem.Text == SelectFile)
            return;
        if (ddlTestUserFiles.SelectedItem.Text == NoTestFiles)
            return;

        var path = HttpContext.Current.Server.MapPath("../");
        var folder = Path.Combine(path, TestLib.TestConstants.UserTestFiles);

        var filePath = Path.Combine(folder, ddlTestUserFiles.SelectedItem.Text.Trim());
        if (File.Exists(filePath) == false)
        {
            lbError.Text = @"No file at " + filePath;
            return;
        }
        ddlEndUserList.Items.Clear();
        var firstuser = true;
        // process the file
        string line;
        var mData = new Dictionary<string, string>();
        // Read the file and display it line by line.
        var mFile = new StreamReader(filePath);
        while ((line = mFile.ReadLine()) != null)
        {
            if (line.StartsWith(tc.MACBank))
            {
                mData.Clear();
                var rtn = mUtils.ParseIntoDictionary(line.Replace(tc.MACBank + dk.ItemSep, ""), mData, char.Parse(dk.KVSep));
                if (rtn == false) continue;
                if (mData[tc.Type] == tc.User)
                {
                    if (firstuser)
                    {
                        firstuser = false;
                        var li1 = new ListItem { Text = SelectUser, Value = SelectUser };
                        ddlEndUserList.Items.Add(li1);
                    }
                    {
                        var li1 = new ListItem { Text = mData[dkui.FirstName] + @" " + mData[dkui.LastName], Value = line };
                        ddlEndUserList.Items.Add(li1);
                    }
                }
            }
        }
        mFile.Close();
    }

    public void lnkbtnExit_Click(object sender, EventArgs e)
    {
        Response.Redirect("../Default.aspx");
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

    private void AddToLogAndDisplay(string textToAdd)
    {
        var newlog = String.Format("{0}|{1}.{2}", Session["LogText"], Test, textToAdd);
        Session["LogText"] = newlog;
        tbLog.Text = newlog.Replace("|", Environment.NewLine);
    }
    #endregion
    
}