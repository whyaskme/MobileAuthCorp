using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Timers;
using System.Web;
using System.Web.UI.WebControls;

using MACSecurity;
using MACServices;

using dk = MACServices.Constants.Dictionary.Keys;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using dv = MACServices.Constants.Dictionary.Values;
using sr = MACServices.Constants.ServiceResponse;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;

namespace Admin.Tests.AWS
{
    public partial class EndUserRegister : System.Web.UI.Page
    {
        public List<ListItem> ClientList = new List<ListItem>();

        public string ClientId = "";
        public string ClientName = "";

        public const string QS_action = "action";
        public const string QS_cid = "cid";
        public const string QS_groupid = "groupid";
        public const string QS_lastname = "lastname";
        public const string QS_userid = "userId";

        public bool UseStaggeredRequestTimers;
        public int iWaitSecsBeforeRequest;

        public bool bAutoOtpRequest = false;

        public int MinWaitSecs = Convert.ToInt16(ConfigurationManager.AppSettings["MinWaitSecs"]);
        public int MaxWaitSecs = Convert.ToInt16(ConfigurationManager.AppSettings["MaxWaitSecs"]);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["UseStaggeredRequestTimers"]))
                iWaitSecsBeforeRequest = new Random().Next(MinWaitSecs, MaxWaitSecs);
            else
                iWaitSecsBeforeRequest = 0;

            rbClientRestricted.Enabled = true;
            rbGroupRestricted.Enabled = true;
            rbOpen.Enabled = true;

            rbClientRestricted.Checked = false;
            rbGroupRestricted.Checked = false;
            rbOpen.Checked = false;

            // This gets populated only once
            if (Application["ClientList"] == null)
                GetClientList();

            if (Request["autorequest"] != null)
            {
                if(Request["autorequest"].ToString() == "true")
                    bAutoOtpRequest = true;
                else
                    bAutoOtpRequest = false;
            }

            // Just testing
            bAutoOtpRequest = true;

            var randomFirstName = Guid.NewGuid();
            var randomLastName = Guid.NewGuid();

            txtFirstName.Text = @"First-" + randomFirstName.ToString().Replace("-", "").Substring(0, new Random().Next(5, 10));
            txtLastName.Text = @"Last-" + randomLastName.ToString().Replace("-", "").Substring(0, new Random().Next(5, 10));

            txtMPhoneNo.Text = new Random().Next(201, 989) + @"-" + new Random().Next(100, 999) + @"-" + new Random().Next(1000, 9999);
            txtEmailAdr.Text = txtFirstName.Text.ToLower() + @"." + txtLastName.Text.ToLower() + @"@mobileauthcorp.com";

            if (!IsPostBack)
            {
                PopulateClientListBox();

                ddlGroups.Visible = false;

                var selectedIndex = new Random().Next(1, ddlClient.Items.Count);

                ddlClient.SelectedIndex = selectedIndex;
            }
            else
            {
                if (ddlClient.SelectedIndex > 0)
                    GetGroups(ddlClient.SelectedValue);
                else
                    ddlGroups.Visible = false;
            }

            if (!ddlGroups.Visible)
            {
                if (ddlClient.SelectedItem.Text.Contains("(Open)"))
                    rbOpen.Checked = true;
                else
                    rbClientRestricted.Checked = true;
            }

            ClientId = ddlClient.SelectedValue;
            ClientName = ddlClient.SelectedItem.Text;

            if (hiddenFormSubmitted.Value == "false")
            {
                btnRegisterEndUser_Click(sender, e);

                hiddenFormSubmitted.Value = "true";
            }
        }

        public void PopulateClientListBox()
        {
            ddlClient.Items.Clear();

            foreach (var currentItem in (List<ListItem>)Application["ClientList"])
            {
                ddlClient.Items.Add(currentItem);
            }
        }

        public void GetClientList()
        {
            var clientArray= new ArrayList();

            var clientList = new MacList("", "Client", "", "_id,Name,OpenAccessServicesEnabled");

            foreach (var currentClient in clientList.ListItems)
            {
                var clientName = currentClient.Attributes["Name"];
                if (currentClient.Attributes["OpenAccessServicesEnabled"] == "true")
                    clientName += " (Open)";

                clientArray.Add(clientName + char.Parse(dk.ItemSep) + currentClient.Value);
            }
            clientArray.Sort();

            foreach (var currentClient in clientArray)
            {
                var tmpClient = currentClient.ToString().Split(char.Parse(dk.ItemSep));
                var clientName = tmpClient[0];
                var clientId = tmpClient[1];

                var li = new ListItem {Text = clientName, Value = clientId};

                ClientList.Add(li);
            }
            Application["ClientList"] = ClientList;
        }

        /// <summary> Register from data on form </summary>
        public void btnRegisterEndUser_Click(object sender, EventArgs e)
        {
            // Group Id if Group is selected
            string mGroupId = null;

            if (ddlGroups.Visible)
                mGroupId = ddlGroups.SelectedValue;

            var rd = CreateRequestDataFromForm(ClientId, mGroupId);

            if (String.IsNullOrEmpty(rd)) return;

            var myReg = new MacRegistration.MacRegistration();
            var reply = myReg.StsRegisterEndUser(ConfigurationManager.AppSettings[cfg.MacServicesUrl], ClientId, rd);

            if (reply.Contains("Reply=Registered"))
            {
                reply = "<div style='width: 100%; text-align: center; position: relative; top: 20px; color: #000000;'>Registered</div>";

                if(bAutoOtpRequest)
                {
                    var mRedirect = new StringBuilder();
                    mRedirect.Append("OTP-Request.aspx");
                    mRedirect.Append("?" + QS_action + "=" + "sendotp");
                    mRedirect.Append("&" + QS_cid + "=" + ClientId);
                    if (String.IsNullOrEmpty(hiddenB.Value) == false)
                        mRedirect.Append("&" + QS_groupid + "=" + hiddenB.Value);
                    mRedirect.Append("&" + QS_lastname + "=" + hiddenlastname.Value);
                    mRedirect.Append("&" + QS_userid + "=" + hiddenE.Value);

                    // Redirect to Otp Request with browser timer
                    Response.AddHeader("REFRESH", iWaitSecsBeforeRequest.ToString() + ";URL=" + mRedirect.ToString());
                    Response.End();
                }
            }
            else
                reply = "<div style='width: 100%; text-align: center; position: relative; top: 20px; color: #ff0000;'>" + reply + "</div>";

            rbClientRestricted.Checked = false;
            rbGroupRestricted.Checked = false;
            rbOpen.Checked = false;
        }

        public void btnSendOTP_Click(object sender, EventArgs e)
        {
            var mRedirect = new StringBuilder();
            mRedirect.Append("OTP-Request.aspx");
            mRedirect.Append("?" + QS_action + "=" + "sendotp");
            mRedirect.Append("&" + QS_cid + "=" + ClientId);
            if (String.IsNullOrEmpty(hiddenB.Value) == false)
                mRedirect.Append("&" + QS_groupid + "=" + hiddenB.Value);
            mRedirect.Append("&" + QS_lastname + "=" + hiddenlastname.Value);
            mRedirect.Append("&" + QS_userid + "=" + hiddenE.Value);
            Response.Redirect(mRedirect.ToString(), true);
            Response.End();
        }

        #region Helpers

        private void GetGroups(string clientId)
        {
            ddlGroups.Items.Clear();
            ddlGroups.Visible = false;

            if (ddlClient.SelectedIndex > 0)
            {
                var currentClient = new Client(clientId);
                foreach (var myRelationship in currentClient.Relationships.Where(myRelationship => myRelationship.MemberType == "Group"))
                {
                    var currentGroup = new Group(myRelationship.MemberId.ToString());
                    var groupItem = new ListItem
                    {
                        Text = currentGroup.Name,
                        Value = currentGroup._id.ToString()
                    };

                    ddlGroups.Items.Add(groupItem);
                    ddlGroups.Visible = true;

                    rbGroupRestricted.Checked = true;
                }
            }
        }

        private string CreateRequestDataFromForm(string pClientid, string pGroupId)
        {
            var rd = new StringBuilder();

            rd.Append(dk.CID + dk.KVSep + pClientid);

            if (rbClientRestricted.Checked)
            {
                rd.Append(dk.ItemSep + dk.RegistrationType + dk.KVSep + dv.ClientRegister);
            }
            else if (rbOpen.Checked)
            {
                rd.Append(dk.ItemSep + dk.RegistrationType + dk.KVSep + dv.OpenRegister);
            }
            else if (rbGroupRestricted.Checked)
            {
                if (String.IsNullOrEmpty(pGroupId))
                {
                    lbError.Text = @"First Name required!";
                    return null;
                }
                hiddenB.Value = pGroupId;
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
            hiddenlastname.Value = txtLastName.Text;
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
            // save incase request OTP is cliecked by tester after registration
            hiddenE.Value = Security.GetHashString(txtLastName.Text.ToLower() + GenerateUid(txtFirstName.Text + txtLastName.Text).ToLower()).ToUpper();
            rd.Append(dk.ItemSep + dk.UserId + dk.KVSep + hiddenE.Value);
            return rd.ToString();
        }

        public string GenerateUid(string pFullName)
        {
            return StringToHex("STS") + StringToHex(pFullName);
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
            tbLog.InnerHtml = "";
        }

        //private void AddToLogAndDisplay(string textToAdd)
        //{
        //    tbLog.InnerHtml = textToAdd;
        //}

        #endregion
    }
}

