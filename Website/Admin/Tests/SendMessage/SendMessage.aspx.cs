using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
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

using tc = TestLib.TestConstants;

namespace MACUserApps.Web.Tests.SendMessage
{
    public partial class MacUserAppsWebTestsSendMessageSendMessage : System.Web.UI.Page
    {
        private const string Test = "SndMsg";
        private const string SelectUser = "Select User";
        private const string NoTestFiles = "No Test Files";
        private const string SelectFile = "Select File";
        private const string SelectClient = "Select Client";
        private const string NoClient = "No Clients";
        private const string SelectGroup = "Select Group";
        private const string NoGroups = "No Groups";

        private HiddenField _hiddenC;

        HiddenField _hiddenW;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.Master != null)
            {
                _hiddenC = (HiddenField) Page.Master.FindControl("hiddenC");
                if (Master != null)
                {
                    _hiddenW = (HiddenField) Master.FindControl("hiddenW");
                    _hiddenW.Value = "54a83bc5ead6362034d04bce";
                }
            }

            if (!IsPostBack)
            {
                if (IsPostBack) return;
                GetClients();
                SetGroups();
                FillddlTestUserFiles();
            }
        }

        #region button events
        /// <summary> Client managed end user (closed) have to supply phone number and email </summary>
        public void btnClientManagedUser_Click(object sender, EventArgs e)
        {
            lbError.Text = "";
            AddToLogAndDisplay("btnClientManagedUser");

            if (String.IsNullOrEmpty(tbMessage.Text))
            {
                lbError.Text = @"You must enter a message!";
                return;
            }
            var msg = tbMessage.Text.Trim().Replace(Environment.NewLine, "|");

            string mPhoneNum;
            string mEmailAdr;
            switch (panelFocusUsers.Value)
            {
                case "UseTextFilesTab":
                    if (ddlEndUserList.SelectedItem.Text == SelectUser)
                    {
                        lbError.Text = @"You must select a user!";
                        return;
                    }
                    // End user info
                    mPhoneNum = GetUserInfo(dkui.PhoneNumber, ddlEndUserList.SelectedItem.Value);
                    mEmailAdr = GetUserInfo(dkui.EmailAddress, ddlEndUserList.SelectedItem.Value);
                    break;
                case "UseFormTab":
                    lbError.Text = String.Empty;
                    if (String.IsNullOrEmpty(txtFirstName.Text))
                        lbError.Text += @"First name, ";
                    if (String.IsNullOrEmpty(txtLastName.Text))
                        lbError.Text += @"Last name, ";
                    if (String.IsNullOrEmpty(txtMPhoneNo.Text))
                    {
                        lbError.Text += @"Phone number, ";
                    }
                    mPhoneNum = txtMPhoneNo.Text;
                    if (String.IsNullOrEmpty(txtUidEmail.Text))
                    {
                        lbError.Text += @"Email address, ";
                    }
                    mEmailAdr = txtUidEmail.Text;
                    if (String.IsNullOrEmpty(lbError.Text) == false)
                    {
                        lbError.Text += @"Is Required!";
                        return;
                    }
                    break;
                default:
                    lbError.Text += @"Select an input method!";
                    return;
            }

            // client Id
            var mCid = GetSelectedClientId(ddlClient.SelectedValue);
            if (string.IsNullOrEmpty(mCid)) return;

            AddToLogAndDisplay(
                String.Format(
                    "|---------|Url:{0}|CID:{1}|Name:{2}|email:{3}|phone:{4}|Message:{5}|---------",
                    ConfigurationManager.AppSettings[cfg.MacServicesUrl], 
                    mCid,
                    txtFirstName.Text + " " + txtLastName.Text,
                    mEmailAdr,
                    mPhoneNum,
                    msg));
            try
            {
                var myMacotp = new MacOtp.MacOtp();
                var response = myMacotp.SendMessageToClientUser(
                    ConfigurationManager.AppSettings[cfg.MacServicesUrl],
                    mCid,
                    txtFirstName.Text + " " + txtLastName.Text,
                    mEmailAdr,
                    mPhoneNum,
                    msg);

                ProcessReply(response, mCid, null);
            }
            catch (Exception ex)
            {
                AddToLogAndDisplay("btnClientLogin: Exception:" + ex);
                lbError.Text = ex.Message;
            }
        }

        /// <summary>  Registered end user to create hash of user's email and phone number ti get UID </summary>
        public void btnRegisteredUser_Click(object sender, EventArgs e)
        {
            lbError.Text = "";
            if (String.IsNullOrEmpty(tbMessage.Text))
            {
                lbError.Text = @"You must enter a message!";
                return;
            }
            var mMsg = tbMessage.Text.Trim().Replace(Environment.NewLine, "|");

            if (ddlEndUserList.SelectedItem.Text == SelectUser)
            {
                lbError.Text = @"You must select a user!";
                return;  
            }
   
            // client Id
            var mCid = GetSelectedClientId(ddlClient.SelectedValue);
            if (string.IsNullOrEmpty(mCid)) return;

            // Group Id if Group is selected
            var mGroupId = GetSelectedGroupId();

            string lastname;
            string UserId;
            switch (panelFocusUsers.Value)
            {
                case "UseTextFilesTab":
                    if (ddlEndUserList.SelectedItem.Text == SelectUser)
                    {
                        lbError.Text = @"You must select a user!";
                        return;
                    }
                    lastname = GetUserInfo(dkui.LastName, ddlEndUserList.SelectedItem.Value);
                    if (cbUsingSTSUserId.Checked)
                    {
                        UserId = GetUserInfo(dk.UserId, ddlEndUserList.SelectedItem.Value);
                        if (String.IsNullOrEmpty(UserId))
                        {
                            var uid = GetUserInfo(dkui.UID, ddlEndUserList.SelectedItem.Value);
                            if (String.IsNullOrEmpty(uid))
                                uid = GetUserInfo(dkui.EmailAddress, ddlEndUserList.SelectedItem.Value);
                            UserId = (Security.GetHashString(lastname.ToLower() + uid.ToLower())).ToUpper();
                        }
                    }
                    else
                    {
                        var uid = GetUserInfo(dkui.UID, ddlEndUserList.SelectedItem.Value);
                        if (String.IsNullOrEmpty(uid))
                            uid = GetUserInfo(dkui.EmailAddress, ddlEndUserList.SelectedItem.Value);
                        UserId = (Security.GetHashString(lastname.ToLower() + uid.ToLower())).ToUpper();
                    }
                    break;
                case "UseFormTab":
                    lbError.Text = String.Empty;
                    if (String.IsNullOrEmpty(txtFirstName.Text))
                        lbError.Text += @"First name, ";
                    if (String.IsNullOrEmpty(txtLastName.Text))
                        lbError.Text += @"Last name, ";
                    if (String.IsNullOrEmpty(txtMPhoneNo.Text))
                        lbError.Text += @"Phone number, ";
                    if (String.IsNullOrEmpty(txtUidEmail.Text))
                        if (String.IsNullOrEmpty(txtSTSUserId.Text))
                            lbError.Text += @"Unique ID / Email or Client Generated Id, ";
                    if (String.IsNullOrEmpty(lbError.Text) == false)
                    {
                        lbError.Text += @"Is Required!";
                        return;
                    }
                    lastname = txtLastName.Text;
                    if (String.IsNullOrEmpty(txtSTSUserId.Text))
                        UserId = (Security.GetHashString(lastname.ToLower() + txtUidEmail.Text.ToLower())).ToUpper();
                    else
                        UserId = txtSTSUserId.Text;
                    break;
                default:
                    lbError.Text += @"Select an input method!";
                    return;
            }

            AddToLogAndDisplay(String.Format("|---------|Url:{0}|CID:{1}|GroupId{2}|UserId{3}|msg:{4}|-----------",
                ConfigurationManager.AppSettings[cfg.MacServicesUrl], 
                mCid, 
                mGroupId, 
                UserId, 
                mMsg)
                );

            try
            {
                var myMacotp = new MacOtp.MacOtp();
                var response = myMacotp.SendMessageToRegisteredUser(
                    ConfigurationManager.AppSettings[cfg.MacServicesUrl], 
                    mCid, 
                    mGroupId, 
                    UserId, 
                    mMsg);

                ProcessReply(response, mCid, mGroupId);
            }
            catch (Exception ex)
            {
                AddToLogAndDisplay("btnRegisteredUser: Exception:" + ex);
                lbError.Text = ex.Message;
            }
        }

        private void ProcessReply(string sReply, string pCid, string pGroupId)
        {
            if (cbXML.Checked)
            {
                AddToLogAndDisplay(sReply.Replace("><", ">|<"));
            }
            else
            {
                AddToLogAndDisplay(sReply);
                if (sReply.StartsWith(sr.Error))
                {
                    lbError.Text = sReply;
                    return;
                }
            }
        }
        #endregion

        #region Helper Methods

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
        
        public void ddlClient_Selected(object sender, EventArgs e)
        {
            SetGroups();
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
            AddToLogAndDisplay("Selected User Details: " + ddlEndUserList.SelectedValue.Replace("|", ", "));
        }

        public void ddlTestUserFiles_Selected(object sender, EventArgs e)
        {
            var mUtils = new Utils();
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
            var mUserCount = 0;
            // process the file
            string line;
            var mData = new Dictionary<string, string>();
            // Read the file and display it line by line.
            var mFile = new StreamReader(filePath);
            while ((line = mFile.ReadLine()) != null)
            {
                line = line.Trim();
                if (line.Length < 5) continue;
                if (line.StartsWith("#")) continue;

                if (line.StartsWith(tc.File.LineDirectives.RequestOTP))
                {
                    mData.Clear();
                    var rtn =
                        mUtils.ParseIntoDictionary(line.Replace(tc.File.LineDirectives.RequestOTP + dk.ItemSep, ""),
                            mData, char.Parse(dk.KVSep));
                    if (rtn == false)
                    {
                        AddToLogAndDisplay("Bad Line:|" + line);
                        continue;
                    }
                    if (firstuser)
                    {
                        firstuser = false;
                        var li1 = new ListItem { Text = SelectUser, Value = SelectUser };
                        ddlEndUserList.Items.Add(li1);
                    }
                    {
                        var li1 = new ListItem
                        {
                            Text = mData[dkui.FirstName] + @" " + mData[dkui.LastName],
                            Value = line
                        };
                        ddlEndUserList.Items.Add(li1);
                        ++mUserCount;
                    }
                }
            }
            mFile.Close();
            if (mUserCount != 1)
                ddlEndUserList.SelectedIndex = 0;
            else
                ddlEndUserList.SelectedIndex = 1;
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

        private void FillddlTestUserFiles()
        {
            ddlTestUserFiles.Items.Clear();
            // get list of test files
            var path = HttpContext.Current.Server.MapPath("../");
            var folder = Path.Combine(path, "UserTestFiles");
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

        public void btnExit_Click(object sender, EventArgs e)
        {
            Session["LogText"] = "";
            Response.Redirect("../Default.aspx");
        }

        #endregion
    }
}