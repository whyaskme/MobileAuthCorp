using System;
using System.Configuration;
//using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;
using System.Web;

using MACSecurity;
using dk = MACServices.Constants.Dictionary.Keys;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using dv = MACServices.Constants.Dictionary.Values;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using tc = TestLib.TestConstants.TestBank;

namespace MACUserApps.Web.Tests.EndUserRegistration
{
    public partial class MacUserAppsWebTeStsEndUserRegistrationRegister : System.Web.UI.Page
    {
        private const string Test = "Reg"; 
        //private const string NotSelected = "Not Selected";
        //private const string SelectUser = "Select User";
        //private const string NoTestFiles = "No Test Files";
        //private const string SelectFile = "Select File";
        private const string SelectClient = "Select Client";
        private const string NoClient = "No Client";
        private const string SelectGroup = "Select Group";
        private const string NoGroups = "No Groups";

        private HiddenField _hiddenD;
        private HiddenField _hiddenO;
        private HiddenField _hiddenC;
        private HiddenField _hiddenQ;
        private HiddenField _hiddenJ;

        HiddenField _hiddenW;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.Master != null)
            {
                _hiddenD = (HiddenField)Page.Master.FindControl("hiddenD");
                _hiddenO = (HiddenField)Page.Master.FindControl("hiddenO");
                _hiddenC = (HiddenField)Page.Master.FindControl("hiddenC");
                _hiddenQ = (HiddenField)Page.Master.FindControl("hiddenQ");
                _hiddenJ = (HiddenField)Page.Master.FindControl("hiddenJ");

                if (Master != null)
                {
                    _hiddenW = (HiddenField) Master.FindControl("hiddenW");
                    _hiddenW.Value = "54a83addead6362034d04bc7"; // End User - User Interface Registration
                }
            }
            if (!IsPostBack)
            {
                Session["LogText"] = "";
                divDataForm.Visible = true;
                divEmailSent.Visible = false;
                if (IsPostBack) return;
                GetClients();
                SetGroups();
                //FillddlTestUserFiles();
            }
        }
        
        /// <summary>  </summary>
        public void btnRegisterEndUser_Click(object sender, EventArgs e)
        {
            _hiddenO.Value = ddlClient.SelectedItem.Text.Replace("(Open)", "").Trim();
            _hiddenJ.Value = txtEmailAdr.Text;
            _hiddenQ.Value = txtFirstName.Text;

            var cid = GetClientIdFromddlClient();
            if (String.IsNullOrEmpty(cid))
            {
                lbError.Text = @"You must select a client!";
                return;
            }

            // Group Id if Group is selected
            var mGroupId = GetSelectedGroupId();

            AddToLogAndDisplay("btnRegisterEndUser_Click: Register " + txtFirstName.Text + " " + txtLastName.Text);
            // process form

            var rd = CreateRequestDataFromForm(cid, mGroupId);
            if (String.IsNullOrEmpty(rd)) return;

            // link to email landing page, user gets an email and clicks on this link
            var current = HttpContext.Current.Request.Url.AbsoluteUri;
            var offset = current.IndexOf("EndUserRegistration", StringComparison.CurrentCulture);
            var path = current.Substring(0, offset);
            var emaillandingpage = path + "RegistrationCompletion/RegistrationCompletion.aspx";
            AddToLogAndDisplay("emaillandingpage:" + emaillandingpage);
            rd += dk.ItemSep + dk.EmailLandingPage + dk.KVSep + StringToHex(emaillandingpage);

            var myReg = new MacRegistration.MacRegistration();
            var reply = myReg.RegisterEndUser(ConfigurationManager.AppSettings[cfg.MacServicesUrl], cid, rd);
            AddToLogAndDisplay(reply);


            if (reply.Contains("Error") == false)
            {
                divDataForm.Visible = false;
                divEmailSent.Visible = true;
                lbFirstName.Text = _hiddenQ.Value;
                if (_hiddenO.Value.EndsWith("'s"))
                    lbClientName.Text = _hiddenO.Value;
                else if (_hiddenO.Value.EndsWith("s"))
                    lbClientName.Text = _hiddenO.Value + @"'";
                else
                    lbClientName.Text = _hiddenO.Value + @"'s";
                lbEmailAddress.Text = _hiddenJ.Value;
            }
        }
        
        #region Helpers

        private string GetClientIdFromddlClient()
        {
            if (ddlClient.SelectedValue == SelectClient)
                return null;
            try
            {
                var values = ddlClient.SelectedValue.Split(char.Parse(dk.ItemSep));
                return values[0];
            }
            catch
            {
                return null;
            }
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

            // do not send ads to this user
            if (rdAdPassDisable.Checked)
                rd.Append(dk.ItemSep + dk.AdPassOption + dk.KVSep + dv.AdDisable);

            if (cbText.Checked)
            {
                if (cbEmail.Checked)
                    // Any
                    rd.Append(dk.ItemSep + dk.NotificationOption + dk.KVSep + dv.Any);
                else
                    // text only
                    rd.Append(dk.ItemSep + dk.NotificationOption + dk.KVSep + dv.Text);
            }
            else
            {       // email only
                    rd.Append(dk.ItemSep + dk.NotificationOption + dk.KVSep + dv.Email);
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

            if (String.IsNullOrEmpty(txtUid.Text))
                rd.Append(dk.ItemSep + dk.UserId + dk.KVSep + GenerateUID(txtLastName.Text, txtEmailAdr.Text));
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

        private string GenerateUID(string p1, string p2)
        {
            return Security.GetHashString(p1.ToLower() + p2.ToLower()).ToUpper();
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
            _hiddenD.Value = ddlClient.SelectedValue;
            if (ddlClient.SelectedItem.Text.Contains("(Open)"))
                rbOpen.Enabled = true;
            else
                rbOpen.Enabled = false;
            _hiddenO.Value = ddlClient.SelectedItem.Text.Replace("(Open)", "").Trim();

            lbGroup.Visible = false;
            ddlGroups.Visible = false;

            ddlGroups.Items.Clear();
            var values = ddlClient.SelectedValue.Split('|');
            if (values.Count() < 2)
            {
                ddlGroups.Items.Add(NoGroups);
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
                    lbGroup.Visible = true;
                    ddlGroups.Visible = true;
                    var name_id = group.Split('=');
                    grouplist.Text = name_id[0];
                    grouplist.Value = name_id[1];
                }
                ddlGroups.Items.Add(grouplist);
            }
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


