using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;
using System.Web;
using System.Collections.Generic;

using MACSecurity;
using MACServices;
using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using cs = MACServices.Constants.Strings;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using sr = MACServices.Constants.ServiceResponse;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;

using tc = TestLib.TestConstants;

namespace MACUserApps.Web.Tests.Authentication
{
    public partial class MacUserAppsWebTestsAuthenticationDefault : System.Web.UI.Page
    {
        private const string Test = "Auth";
        private const string SelectUser = "Select User";
        private const string NoTestFiles = "No Test Files";
        private const string SelectFile = "Select File";
        private const string SelectClient = "Select Client";
        private const string NoClient = "No Clients";
        private const string SelectGroup = "Select Group";
        private const string NoGroups = "No Groups";
        private const string EnterOTPPage = "EnterOtp.aspx";

        HiddenField _hiddenW;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Master != null)
            {
                _hiddenW = (HiddenField)Master.FindControl("hiddenW");
                _hiddenW.Value = "54a83b84ead6362034d04bcc";
            }

            Session["LogText"] = "";
            if (IsPostBack) return;
            GetClients();
            SetGroups();
            FillddlTestUserFiles();
        }

        #region Buttons

        /// <summary>Client managed end user (closed) have to supply phone number and email</summary>
        public void btnClientManagedUser_Click(object sender, EventArgs e)
        {
            lbError.Text = "";
            AddToLogAndDisplay("btnClientManagedUser:" + panelFocusUsers.Value);

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

            // transaction type and details
            const string trxtype = "1";
            var trxdetails = String.Empty;

            var mAdDetails = getAdDetailsFromForm();

            var mReq = String.Format(
                    "|Url={0}|CID={1}|email={2}|phone={3}|trxtype={4}|trxdetails={5}|AdDetails={6}",
                    ConfigurationManager.AppSettings[cfg.MacServicesUrl],
                    mCid,
                    mEmailAdr,
                    mPhoneNum,
                    trxtype,
                    trxdetails,
                    mAdDetails);
            AddToLogAndDisplay("|--- Request ---" + mReq + "|---------");

            try
            {
                var myMacotp = new MacOtp.MacOtp();
                // force error set
                var sReply = myMacotp.SendOtpToClientUser(
                    ConfigurationManager.AppSettings[cfg.MacServicesUrl], 
                    mCid, mEmailAdr, mPhoneNum, trxtype, trxdetails, mAdDetails);
                ProcessReply(sReply, mCid, null);
            }
            catch (Exception ex)
            {
                AddToLogAndDisplay("btnClientLogin: Exception:" + ex);
                lbError.Text = ex.Message;
            }
        }

        /// <summary>Registered end user to create hash of user's email and phone number to get UID</summary>
        public void btnRegisteredUser_Click(object sender, EventArgs e)
        {
            var mButton = (Button) sender;
            AddToLogAndDisplay(mButton.Text);

            lbError.Text = "";
            AddToLogAndDisplay("btnRegisteredUser:" + panelFocusUsers.Value);

            // client Id
            var mCid = GetSelectedClientId(ddlClient.SelectedValue);
            if (string.IsNullOrEmpty(mCid))
            {
                lbError.Text = @"You must select a client!";
                return;
            }

            // Group Id if Group is selected
            var groupId = GetSelectedGroupId();

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
                    if (mButton.Text != @"Registered")
                    {
                        if (String.IsNullOrEmpty(txtMPhoneNo.Text))
                            lbError.Text += @"Phone number, ";
                    }
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

            // transaction type and details
            const string trxtype = "1";
            var trxdetails = String.Empty;

            var mAdDetails = getAdDetailsFromForm();

            if (rbInvalidCID.Checked)
            {
                mCid = cs.DefaultStaticObjectId;
            }
            else if (rbNoCID.Checked)
            {
                mCid = null;
            }
            else if (rbInvalidGroupId.Checked)
            {
                groupId = cs.DefaultStaticObjectId;
            }
            else if (rbNoUserId.Checked)
            {
                UserId = null;
            }

            var mReq =
                    String.Format(
                        "|Url={0}|CID={1}|GroupId={2}|Name={3}|UID={4}|trxtype={5}|trxdetails={6}|AdDetails={7}|------------",
                        /*0*/ConfigurationManager.AppSettings[cfg.MacServicesUrl],
                        /*1*/mCid,
                        /*2*/groupId,
                        /*3*/lastname,
                        /*4*/UserId,
                        /*5*/trxtype,
                        /*6*/trxdetails,
                        /*8*/mAdDetails);

            AddToLogAndDisplay("|--- Request ---" + mReq + "|---------");
            try
            {
                var myMacotp = new MacOtp.MacOtp();
                var sReply = myMacotp.SendOtpToRegisteredUser(
                    ConfigurationManager.AppSettings[cfg.MacServicesUrl],
                    mCid,
                    groupId,
                    lastname,
                    UserId,
                    trxtype,
                    trxdetails,
                    mAdDetails);

                ProcessReply(sReply, mCid, groupId);
            }
            catch (Exception ex)
            {
                AddToLogAndDisplay("btnRegisteredUser: Exception:" + ex);
                lbError.Text = ex.Message;
            }  
        }

        private void ProcessReply(string sReply, string pCid, string pGroupId)
        {
            var mEnterOTPad = "";
            var mConad = "";
            var mOtp = "";
            var mRid = "";
            if (cbXML.Checked)
            {
                AddToLogAndDisplay(sReply.Replace("><", ">|<"));
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(sReply);
                var elemList = xmlDoc.GetElementsByTagName(sr.RequestId);
                if (elemList.Count != 0)
                    mRid = elemList[0].InnerXml;
                elemList = xmlDoc.GetElementsByTagName(sr.Debug);
                if (elemList.Count != 0)
                    mOtp = elemList[0].InnerXml;
                elemList = xmlDoc.GetElementsByTagName(sr.EnterOTPAd);
                //if (elemList.Count != 0)
                //    mEnterOTPad = elemList[0].InnerXml;
                //elemList = xmlDoc.GetElementsByTagName(sr.ContentAd);
                if (elemList.Count != 0)
                    mConad = elemList[0].InnerXml;
            }
            else
            {
                AddToLogAndDisplay(sReply);
                if (sReply.StartsWith(sr.Error))
                {
                    lbError.Text = sReply;
                    return;
                }

                if (String.IsNullOrEmpty(pCid))
                {
                    lbError.Text = @"Process response did not get CID";
                    return;
                }

                var values = sReply.Split(char.Parse(dk.ItemSep));


                foreach (var value in values)
                {
                    if (value.StartsWith(sr.RequestId))
                        mRid = value.Replace(sr.RequestId + dk.KVSep, "");
                    if (value.StartsWith(sr.OTP))
                        mOtp = value.Replace(sr.OTP + dk.KVSep, "");
                    if (value.StartsWith(sr.EnterOTPAd))
                        mEnterOTPad = value.Replace(sr.EnterOTPAd + dk.KVSep, "");
                    //if (value.StartsWith(sr.ContentAd))
                    //    mConad = value.Replace(sr.EnterOTPAd + dk.KVSep, "");
                }
          
            }
            if (string.IsNullOrEmpty(mRid))
            {
                lbError.Text = @"No RequestId in response!";
                return;
            }
            var redirect = EnterOTPPage + "?" + dk.CID + "=" + pCid +
                           "&" + sr.RequestId + "=" + mRid;

            if (String.IsNullOrEmpty(mOtp) == false)
                redirect += "&" + sr.OTP + "=" + mOtp;

            if (String.IsNullOrEmpty(mEnterOTPad) == false)
                redirect += "&" + sr.EnterOTPAd + "=" + mEnterOTPad;

            //if (String.IsNullOrEmpty(mConad) == false)
            //    redirect += "&" + sr.ContentAd + "=" + mConad;

            if (String.IsNullOrEmpty(pGroupId) == false)
                redirect += "&" + dk.GroupId + "=" + pGroupId;

            if (String.IsNullOrEmpty(pGroupId) == false)
                redirect += "&" + dk.GroupId + "=" + pGroupId;

            AddToLogAndDisplay(redirect);
            Response.Redirect(redirect, false);
        }
        #endregion

        #region Selected
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
                        var li1 = new ListItem {Text = SelectUser, Value = SelectUser};
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

        public void ddlEndUserList_Selected(object sender, EventArgs e)
        {
            AddToLogAndDisplay("Selected User Details: " + ddlEndUserList.SelectedValue.Replace(dk.ItemSep, ", "));
            //btnClientManaged.Enabled = true;
            //btnRegistered.Enabled = true;
        }

        public void ddlClient_Selected(object sender, EventArgs e)
        {
            SetGroups();
        }
        #endregion

        #region Helper Methods

        private string getAdDetailsFromForm()
        {
            var mAdDetails = String.Empty;
            if (!String.IsNullOrEmpty(txtAdNumber.Text))
            {
                int mAdNo;
                if (int.TryParse(txtAdNumber.Text, out mAdNo))
                {
                    if ((mAdNo > 0) || (mAdNo < 6))
                         mAdDetails = dk.Ads.AdNumber + dk.KVSep + txtAdNumber.Text;
                }
            } // ad number overrides keywords
            else if (!String.IsNullOrEmpty(txtKeyWords.Text))
            {
                mAdDetails = dk.Ads.SpecificKeywords + dk.KVSep + StringToHex(txtKeyWords.Text);
            }

            if (cbAdPassEnable.Checked)
            {
                return dk.AdPassOption + dk.KVSep + dv.AdEnable + dk.ItemSep + mAdDetails;
            }

            if (cbAdPassDisable.Checked)
            {
                return dk.AdPassOption + dk.KVSep + dv.AdDisable + dk.ItemSep + mAdDetails;
            }
            return mAdDetails;
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
        protected string GetUserInfo(string pKey, string pUserDetails)
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
                        var gd = ddlGroups.SelectedValue.Split(char.Parse(dk.ItemSep));
                        return gd[0];
                    }
                }
            }
            return null;
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
        
        protected string StringToHex(String input)
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
}