using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;

using MACSecurity;
using MACServices;
using dk = MACServices.Constants.Dictionary.Keys;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using dv = MACServices.Constants.Dictionary.Values;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using sr = MACServices.Constants.ServiceResponse;

namespace Admin.Tests.SecureTradingService
{
    public partial class MacUserAppsWebTestsSecureTradingServiceStsRegister : System.Web.UI.Page
    {
        private const string SelectClient = "Select Client";
        private const string NoTestFiles = "No Test Files";
        private const string SelectFile = "Select File";
        private const string NoClient = "No Clients";
        private const string SelectGroup = "Select Group";
        private const string NoGroups = "No Groups";

        HiddenField _hiddenD;
        HiddenField _hiddenO;
        HiddenField _hiddenC;
        
        HiddenField _hiddenW;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.Master != null)
            {
                _hiddenD = (HiddenField)Page.Master.FindControl("hiddenD");
                _hiddenO = (HiddenField)Page.Master.FindControl("hiddenO");
                _hiddenC = (HiddenField)Page.Master.FindControl("hiddenC");

                if (Master != null)
                {
                    _hiddenW = (HiddenField) Master.FindControl("hiddenW");
                    _hiddenW.Value = "54a83b22ead6362034d04bc9";
                }
            }
            if (!IsPostBack)
            {
                GetClients();
                SetGroups0();
                lbGroup0.Visible = false;
                ddlGroups0.Visible = false;
                SetGroups1();
                lbGroup1.Visible = false;
                ddlGroups1.Visible = false;
                FillddlTestUserFiles();
            }
        }

        public void ddlClient_Selected0(object sender, EventArgs e)
        {
            lbError.Text = "";
            SetGroups0();
            AddToLogAndDisplay("ClientSelected: " + ddlClient0.SelectedItem + " " + ddlClient0.SelectedItem.Value);
        }
        public void ddlClient_Selected1(object sender, EventArgs e)
        {
            lbError.Text = "";
            SetGroups1();
            AddToLogAndDisplay("ClientSelected: " + ddlClient1.SelectedItem + " " + ddlClient1.SelectedItem.Value);
        }

        #region Buttons
        /// <summary> Register from data on form </summary>
        public void btnRegisterEndUser_Click(object sender, EventArgs e)
        {
            AddToLogAndDisplay("btnRegisterEndUser_Click: Register " + txtFirstName.Text + " " + txtLastName.Text);
            
            var cid = GetClientIdFromddlClient0();

            // Group Id if Group is selected
            var mGroupId = GetSelectedGroupId0();

            var rd = CreateRequestDataFromForm(cid, mGroupId);
            if (String.IsNullOrEmpty(rd)) return;

            var myReg = new MacRegistration.MacRegistration();
            var reply = myReg.StsRegisterEndUser(ConfigurationManager.AppSettings[cfg.MacServicesUrl], cid, rd);
            AddToLogAndDisplay(reply);
        }

        /// <summary> Register from a file </summary>
        public void btnFileRegisterTestUsers_Click(object sender, EventArgs e)
        {
            var btn = (Button) sender;
            AddToLogAndDisplay(btn.Text + @"(" + btn.ID + @") clicked");
            var fileselected = ddlTestUserFiles.SelectedItem.Text;
            if (fileselected == NoTestFiles)
            {
                lbError.Text = @"No test file";
                return;
            }
            if (fileselected == SelectFile)
            {
                lbError.Text = @"You must select a test file";
                return;
            }

            var mData = new System.Collections.Generic.Dictionary<string, string>();
            var path = HttpContext.Current.Server.MapPath("../");
            var folder = Path.Combine(path, TestLib.TestConstants.UserTestFiles);

            var filePath = Path.Combine(folder, fileselected);
            if (File.Exists(filePath) == false)
            {
                lbError.Text = @"No file at " + filePath;
                return;
            }
            AddToLogAndDisplay("Read file @" + filePath);
            {
                var mUtils = new Utils();
                string line;
                // Read the file and display it line by line.
                var mFile = new StreamReader(filePath);
                while ((line = mFile.ReadLine()) != null)
                {
                    if (String.IsNullOrEmpty(line)) continue;
                    if (line.StartsWith(TestLib.TestConstants.File.LineDirectives.STSRegister) == false) continue;
                    line = line.Replace(TestLib.TestConstants.File.LineDirectives.STSRegister + dk.ItemSep, "");
                    mData.Clear();
                    if (mUtils.ParseIntoDictionary(line, mData, char.Parse(dk.KVSep)) == false)
                    {
                        AddToLogAndDisplay("STSRegister.ParseIntoDictionary: bad request data!");
                        AddToLogAndDisplay(line.Replace(dk.ItemSep, ", "));
                        continue;
                    }
                    AddToLogAndDisplay(line.Replace(dk.ItemSep, ", "));
                    if (!mData.ContainsKey(dk.RegistrationType))
                    {
                        AddToLogAndDisplay("Bad line, no RegistrationType!");
                        continue;
                    }
 
                    // client name required
                    if (!mData.ContainsKey(dk.ClientName))
                    {
                        AddToLogAndDisplay("Bad line, no ClientName!");
                        continue;
                    }
                    // get client id by name
                    var cid = "";
                    foreach (ListItem li in ddlClient0.Items)
                    {

                        if (li.Text.Replace("(Open)", "").Trim() == mData[dk.ClientName])
                        {
                            var value = li.Value;
                            var x = value.Split(char.Parse(dk.ItemSep));
                            cid = x[0];
                            break;
                        }
                    }
                    if (String.IsNullOrEmpty(cid))
                    {
                        AddToLogAndDisplay("Could not find" + mData[dk.ClientName] + " in client list");
                        continue;
                    }
                    // CREATE REQUEST DATA STRING FROM DICTIONARY
                    var rd = new StringBuilder();
                    rd.Append(dk.CID + dk.KVSep + cid);
                    if (!mData.ContainsKey(dkui.FirstName))
                    {
                        AddToLogAndDisplay("Bad line, First Name");
                        continue;
                    }
                    rd.Append(dk.ItemSep + dkui.FirstName + dk.KVSep + mData[dkui.FirstName]);

                    if (!mData.ContainsKey(dkui.LastName))
                    {
                        AddToLogAndDisplay("Bad line, Last Name");
                        continue;
                    }
                    rd.Append(dk.ItemSep + dkui.LastName + dk.KVSep + mData[dkui.LastName]);

                    if (!mData.ContainsKey(dkui.PhoneNumber))
                    {
                        AddToLogAndDisplay("Bad line, no Mobile Phone");
                        continue;
                    }
                    rd.Append(dk.ItemSep + dkui.PhoneNumber + dk.KVSep + mData[dkui.PhoneNumber]);

                    if (!mData.ContainsKey(dkui.EmailAddress))
                    {
                        AddToLogAndDisplay("Bad line, no email address");
                        continue;
                    }
                    rd.Append(dk.ItemSep + dkui.EmailAddress + dk.KVSep + mData[dkui.EmailAddress]);

                    rd.Append(dk.ItemSep + dk.RegistrationType + dk.KVSep + mData[dk.RegistrationType]);

                    if (mData.ContainsKey(dk.UserId))
                        rd.Append(dk.ItemSep + dk.UserId + dk.KVSep + mData[dk.UserId]);
                    else
                        rd.Append(dk.ItemSep + dk.UserId + dk.KVSep +
                            (Security.GetHashString(mData[dkui.LastName].ToLower() + mData[dkui.EmailAddress].ToLower()).ToUpper()));

                    if (mData.ContainsKey(dk.GroupName))
                        rd.Append(dk.ItemSep + dk.GroupName + dk.KVSep + mData[dk.GroupName]);

                    var requestdata = rd.ToString();

                    var myReg = new MacRegistration.MacRegistration();
                    var reply = myReg.StsRegisterEndUser(ConfigurationManager.AppSettings[cfg.MacServicesUrl], cid, requestdata);
                    AddToLogAndDisplay(reply);
                    if (reply.Contains("server returned an error")) break;
                }
                mFile.Close();
            }
        }

        //public void btnGetBillingNumbers_Click(object sender, EventArgs e)
        //{
        //    AddToLogAndDisplay("btnGetBillingNumbers_Click:");
        //    if (String.IsNullOrEmpty(txtMMYYYY.Text))
        //    {
        //        lbError.Text = @"Date of bill required!";
        //        return;
        //    }

        //    var cid = GetClientIdFromddlClient1();
        //    if (String.IsNullOrEmpty(cid))
        //    {
        //        lbError.Text = @"Select a client!";
        //        return;
        //    }

        //    // Group Id if Group is selected
        //    var mGroupId = GetSelectedGroupId1();

        //    var mRequest = dk.Request + dk.KVSep + dv.GetUsageBillingForMonth +
        //                   dk.ItemSep + dk.CID + dk.KVSep + cid +
        //                   dk.ItemSep + dk.BillDate + dk.KVSep + txtMMYYYY.Text;
        //    if (string.IsNullOrEmpty(mGroupId) == false)
        //        mRequest += dk.ItemSep + dk.GroupId + dk.KVSep + mGroupId;

        //    if (cbDummyBilling.Checked)
        //        mRequest += dk.ItemSep + dk.DummyBilling + dk.KVSep + "True";
            
        //    mRequest += dk.ItemSep + dk.API + dk.KVSep + dv.Test;

        //    var mResponse = IssueRequest(
        //        ConfigurationManager.AppSettings[cfg.MacServicesUrl].ToString() + Constants.ServiceUrls.MacUsageBilling,
        //        cid, mRequest);

        //    AddToLogAndDisplay(mResponse);
        //}

        public void btnValidate_Click(object sender, EventArgs e)
        {
            var mUtils = new Utils();
            if (String.IsNullOrEmpty(txtMPhoneNo.Text) == false)
            {
                var justNumbers = mUtils.PhoneJustNumbers(txtMPhoneNo.Text);
                var mReply = mUtils.ValidatePhoneNumber(txtMPhoneNo.Text);
                if (mReply == false)
                    AddToLogAndDisplay("Invalid Phone Number[" + txtMPhoneNo.Text + "], just numbers[" + justNumbers + "]");
                else
                    AddToLogAndDisplay("Valid Phone Number[" + txtMPhoneNo.Text + "], just numbers[" + justNumbers + "]");
            }

            if (String.IsNullOrEmpty(txtEmailAdr.Text) == false)
            {
                var mReply = mUtils.ValidateEmailAddress(txtEmailAdr.Text);
                if (mReply == false)
                    AddToLogAndDisplay("Invalid email address[" + txtEmailAdr.Text + "]");
                else
                    AddToLogAndDisplay("Valid email address[" + txtEmailAdr.Text + "]");
            }
        }

        //protected bool ValidateEmailAddress(string pEmailAddress)
        //{
        //    const string regXEmailAddress1 = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

        //    return Regex.IsMatch(pEmailAddress, regXEmailAddress1);
        //}

        //protected bool ValidatePhoneNumber(string pPhoneNumber)
        //{
        //    const string regXPhoneNumber1 = @"^\(?\d{3}\)?-? *\d{3}-? *-?\d{4}";
        //    // 7 or 10 digit number, with extensions allowed, delimiters are spaces, dashes, or periods and extention:
        //    const string regXPhoneNumber2 = @"^(?:(?:\+?1\s*(?:[.-]\s*)?)?(?:\(\s*([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9])\s*\)|([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9]))\s*(?:[.-]\s*)?)?([2-9]1[02-9]|[2-9][02-9]1|[2-9][02-9]{2})\s*(?:[.-]\s*)?([0-9]{4})(?:\s*(?:#|x\.?|ext\.?|extension)\s*(\d+))?$";

        //    // without the extension section
        //    const string regXPhoneNumber3 = @"^(?:(?:\+?1\s*(?:[.-]\s*)?)?(?:(\s*([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9]‌​)\s*)|([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9]))\s*(?:[.-]\s*)?)?([2-9]1[02-‌​9]|[2-9][02-9]1|[2-9][02-9]{2})\s*(?:[.-]\s*)?([0-9]{4})$";

        //    //10 digit accepts () around area code, and dosen't allow preceeding 1 as country code
        //    const string regXPhoneNumber4 = @"(?:(?:(\s*\(?([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9])\s*)|([2-9]1[02-9]|[2‌​-9][02-8]1|[2-9][02-8][02-9]))\)?\s*(?:[.-]\s*)?)([2-9]1[02-9]|[2-9][02-9]1|[2-9]‌​[02-9]{2})\s*(?:[.-]\s*)?([0-9]{4})";


        //    if (Regex.IsMatch(pPhoneNumber, regXPhoneNumber2) == false) return false;
        //    var tmp = pPhoneNumber.Replace(".", "")
        //        .Replace("-", "")
        //        .Replace(" ", "")
        //        .Replace("(", "")
        //        .Replace(")", "");
        //    AddToLogAndDisplay("Just Numbers[" + tmp + "]");
        //    if (tmp.Length != 10) return false;
        //    return true;
        //}

        #endregion

        #region Helpers

        //private string IssueRequest(string pUrl, string pCid, string pRequest)
        //{
        //    try
        //    {
        //        var dataStream = Encoding.UTF8.GetBytes("data=99" + pCid.Length + pCid + StringToHex(pRequest));
        //        var request = pUrl;
        //        var webRequest = WebRequest.Create(request);
        //        webRequest.Method = "POST";
        //        webRequest.ContentType = "application/x-www-form-urlencoded";
        //        webRequest.ContentLength = dataStream.Length;
        //        var newStream = webRequest.GetRequestStream();
        //        // Send the data.
        //        newStream.Write(dataStream, 0, dataStream.Length);
        //        newStream.Close();
        //        var res = webRequest.GetResponse();
        //        var response = res.GetResponseStream();
        //        var xmlDoc = new XmlDocument();
        //        if (response != null) xmlDoc.Load(response);
        //        var elemList = xmlDoc.GetElementsByTagName("macResponse");
        //        return elemList[0].InnerXml;
        //    }
        //    catch (Exception ex)
        //    {
        //        AddToLogAndDisplay(ex.ToString());
        //        lbError.Text = @"Service error";
        //        return null;
        //    }
        //}

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

                ddlClient0.Items.Clear();
                ddlClient1.Items.Clear();
                elemList = xmlDoc.GetElementsByTagName("Client");
                if (elemList.Count != 0)
                {
                    var li1 = new ListItem
                    {
                        Text = SelectClient,
                        Value = SelectClient
                    };
                    ddlClient0.Items.Add(li1);
                    ddlClient1.Items.Add(li1);
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
                        ddlClient0.Items.Add(li);
                        ddlClient1.Items.Add(li);
                    }
                }
                else
                {
                    var li0 = new ListItem
                    {
                        Text = NoClient,
                        Value = NoClient
                    };
                    ddlClient0.Items.Add(li0);
                    ddlClient1.Items.Add(li0);
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

        private string GetSelectedGroupId0()
        {
            if (ddlGroups0.Visible)
            {
                if (ddlGroups0.SelectedValue != NoGroups)
                {
                    if (ddlGroups0.SelectedValue != SelectGroup)
                    {
                        var gd = ddlGroups0.SelectedValue.Split(char.Parse(dk.ItemSep));
                        return gd[0];
                    }
                }
            }
            return null;
        }

        private string GetSelectedGroupId1()
        {
            if (ddlGroups1.Visible)
            {
                if (ddlGroups1.SelectedValue != NoGroups)
                {
                    if (ddlGroups1.SelectedValue != SelectGroup)
                    {
                        var gd = ddlGroups1.SelectedValue.Split(char.Parse(dk.ItemSep));
                        return gd[0];
                    }
                }
            }
            return null;
        }

        private void SetGroups0()
        {
            _hiddenD.Value = ddlClient0.SelectedValue;
            if (ddlClient0.SelectedItem.Text.Contains("(Open)"))
                rbOpen.Enabled = true;
            else
                rbOpen.Enabled = false;
            _hiddenO.Value = ddlClient0.SelectedItem.Text.Replace("(Open)", "").Trim();

            lbGroup0.Visible = false;
            ddlGroups0.Visible = false;

            ddlGroups0.Items.Clear();
            var values = ddlClient0.SelectedValue.Split('|');
            if (values.Count() < 2)
            {
                ddlGroups0.Items.Add(NoGroups);
                rbGroupRestricted.Enabled = false;
                _hiddenC.Value = "";
                return;
            }
            var skipcid = true; // first entry is always the clientId
            rbGroupRestricted.Enabled = true;
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
                    lbGroup0.Visible = true;
                    ddlGroups0.Visible = true;
                    var name_id = group.Split('=');
                    grouplist.Text = name_id[0];
                    grouplist.Value = name_id[1];
                }
                ddlGroups0.Items.Add(grouplist);
            }
        }

        private void SetGroups1()
        {
            _hiddenD.Value = ddlClient1.SelectedValue;
            _hiddenO.Value = ddlClient1.SelectedItem.Text.Replace("(Open)", "").Trim();

            lbGroup1.Visible = false;
            ddlGroups1.Visible = false;

            ddlGroups1.Items.Clear();
            var values = ddlClient1.SelectedValue.Split('|');
            if (values.Count() < 2)
            {
                ddlGroups1.Items.Add(NoGroups);
                rbGroupRestricted.Enabled = false;
                _hiddenC.Value = "";
                return;
            }
            var skipcid = true; // first entry is always the clientId
            rbGroupRestricted.Enabled = true;
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
                    lbGroup1.Visible = true;
                    ddlGroups1.Visible = true;
                    var name_id = group.Split('=');
                    grouplist.Text = name_id[0];
                    grouplist.Value = name_id[1];
                }
                ddlGroups1.Items.Add(grouplist);
            }
        }
        
        private string GetClientIdFromddlClient0()
        {
            if (ddlClient0.SelectedValue == SelectClient)
                return null;
            try
            {
                var values = ddlClient0.SelectedValue.Split(char.Parse(dk.ItemSep));
                return values[0];
            }
            catch
            {
                return null;             
            }
        }
        
        private string GetClientIdFromddlClient1()
        {
            if (ddlClient1.SelectedValue == SelectClient)
                return null;
            try
            {
                var values = ddlClient1.SelectedValue.Split(char.Parse(dk.ItemSep));
                return values[0];
            }
            catch
            {
                return null;
            }
        }

        private string CreateRequestDataFromForm(string pClientid, string pGroupId)
        {
            var rd = new StringBuilder();

            rd.Append(dk.CID + dk.KVSep + pClientid);

            if (rbClientRestricted.Checked)
                rd.Append(dk.ItemSep + dk.RegistrationType + dk.KVSep + dv.ClientRegister);
            else if (rbOpen.Checked)
                rd.Append(dk.ItemSep + dk.RegistrationType + dk.KVSep + dv.OpenRegister);
            else if (rbGroupRestricted.Checked)
            {
                if (String.IsNullOrEmpty(pGroupId))
                {
                    lbError.Text = @"First Name required!";
                    return null;
                }
                rd.Append(dk.ItemSep + dk.RegistrationType + dk.KVSep + dv.GroupRegister);
                rd.Append(dk.ItemSep + dk.GroupId + dk.KVSep + pGroupId);
            }
            else
            {
                lbError.Text = @"Registration type required!";
                return null;
            }

            if (String.IsNullOrEmpty(txtFirstName.Text))
            {
                lbError.Text = @"First Name required!";
                return null;
            }
            rd.Append(dk.ItemSep + dkui.FirstName + dk.KVSep + txtFirstName.Text);
            if (String.IsNullOrEmpty(txtLastName.Text))
            {
                lbError.Text = @"Last Name required!";
                return null;
            }
            rd.Append(dk.ItemSep + dkui.LastName + dk.KVSep + txtLastName.Text);
            if (String.IsNullOrEmpty(txtMPhoneNo.Text))
            {
                lbError.Text = @"Mobile Phone Required!";
                return null;
            }
            rd.Append(dk.ItemSep + dkui.PhoneNumber + dk.KVSep + txtMPhoneNo.Text);
            if (String.IsNullOrEmpty(txtEmailAdr.Text))
            {
                lbError.Text = @"email address required!";
                return null;
            }
            rd.Append(dk.ItemSep + dkui.EmailAddress + dk.KVSep + txtEmailAdr.Text);
            // user id
            if (String.IsNullOrEmpty(txtUid.Text))
                rd.Append(dk.ItemSep + dk.UserId + dk.KVSep + 
                    Security.GetHashString(txtLastName.Text.ToLower() + txtEmailAdr.Text.ToLower()).ToUpper());
            else
                rd.Append(dk.ItemSep + dk.UserId + dk.KVSep + txtUid.Text);

            if (!String.IsNullOrEmpty(txtNamePrefix.Text))
                rd.Append(dk.ItemSep + dkui.Prefix + dk.KVSep + txtNamePrefix.Text);

            if (!String.IsNullOrEmpty(txtMiddleName.Text))
                rd.Append(dk.ItemSep + dkui.MiddleName + dk.KVSep + txtMiddleName.Text);

            if (!String.IsNullOrEmpty(txtNameSuffix.Text))
                rd.Append(dk.ItemSep + dkui.Suffix + dk.KVSep + txtNameSuffix.Text);

            if (!String.IsNullOrEmpty(txtDOB.Text))
            {
                DateTime dt;
                if (DateTime.TryParse(txtDOB.Text, out dt))
                    rd.Append(dk.ItemSep + dkui.DOB + dk.KVSep + dt.ToShortDateString());
            }

            if (!String.IsNullOrEmpty(txtSSN4.Text))
                rd.Append(dk.ItemSep + dkui.SSN4 + dk.KVSep + txtSSN4.Text);

            if (!String.IsNullOrEmpty(txtAdr.Text))
                rd.Append(dk.ItemSep + dkui.Street + dk.KVSep + txtAdr.Text);

            if (!String.IsNullOrEmpty(txtAdr2.Text))
                rd.Append(dk.ItemSep + dkui.Street2 + dk.KVSep + txtAdr2.Text);

            if (!String.IsNullOrEmpty(txtUnit.Text))
                rd.Append(dk.ItemSep + dkui.Unit + dk.KVSep + txtUnit.Text);

            if (!String.IsNullOrEmpty(txtCity.Text))
                rd.Append(dk.ItemSep + dkui.City + dk.KVSep + txtCity.Text);

            if (!String.IsNullOrEmpty(txtState.Text))
                rd.Append(dk.ItemSep + dkui.State + dk.KVSep + txtState.Text);

            if (!String.IsNullOrEmpty(txtZipCode.Text))
                rd.Append(dk.ItemSep + dkui.ZipCode + dk.KVSep + txtZipCode.Text);

            if (!String.IsNullOrEmpty(txtCountry.Text))
                rd.Append(dk.ItemSep + dkui.Country + dk.KVSep + txtCountry.Text);

            if (!String.IsNullOrEmpty(txtDriverLic.Text))
            {
                if (String.IsNullOrEmpty(txtDriverLicSt.Text))
                {
                    lbError.Text = @"State required!";
                    return null;
                }
                rd.Append(dk.ItemSep + dkui.DriverLic + dk.KVSep + txtDriverLic.Text);
                rd.Append(dk.ItemSep + dkui.DriverLicSt + dk.KVSep + txtDriverLicSt.Text);
            }

            return rd.ToString();
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
            var newlog = String.Format("{0}|STS: {1}", Session["LogText"], textToAdd);
            Session["LogText"] = newlog;
            tbLog.Text = newlog.Replace("|", Environment.NewLine);
        }
        #endregion
    }
}

