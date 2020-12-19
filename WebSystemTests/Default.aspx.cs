using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

using MongoDB.Bson;

using MACSecurity;

using cs = MACServices.Constants.Strings;
using dk = MACServices.Constants.Dictionary.Keys;
using dkui = MACServices.Constants.Dictionary.Userinfo;

using MACOperationalTestLib;

public partial class _Default : Page
{
    public const string PS = "Please Select";
    public const string None = "None";
    public const string New = "New";
    public const string Update = "Update";

    public const string ST = "Select a Test";

    public string mappedPath = "";
    public string ScriptsDir = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        mappedPath = Server.MapPath("~");
        ScriptsDir = mappedPath + "\\" + ConfigurationManager.AppSettings["RegressionTestScriptsDir"];

        //if (IsPostBack) return;

        // Get environments to test from config
        ListItem defaultItem = new ListItem();
        defaultItem.Text = "Select a Target Server to test";
        defaultItem.Value = defaultItem.Text;

        dlTargetServers.Items.Add(defaultItem);

        string[] serversToTest = ConfigurationManager.AppSettings["TargetServers"].Split('|');
        foreach(var currentServerConfig in serversToTest)
        {
            var tmpVal = currentServerConfig.Split('~');

            var targetAppServer = tmpVal[0].Trim();
            var targetDatabaseServer = tmpVal[1].Trim();
            var targetDatabaseName = tmpVal[2].Trim();

            ListItem currentItem = new ListItem();
            currentItem.Text = targetAppServer;
            currentItem.Value = currentItem.Text;

            // Add attributes
            currentItem.Attributes.Add("targetAppServer", targetAppServer);
            currentItem.Attributes.Add("targetDatabaseServer", targetDatabaseServer);

            if (targetAppServer == "localhost")
            {
                // Figure out which localhost and append the database name accordingly
                var mComputer = Environment.MachineName.ToUpper();
                switch (mComputer.ToLower())
                {
                    case "chrismiller":
                        targetDatabaseName += "_Chris";
                        break;
                    case "terryshotbox":
                        targetDatabaseName += "_Terry";
                        break;
                    case "lenovo-pc":
                        targetDatabaseName += "_Joe";
                        break;
                }
            }

            currentItem.Attributes.Add("targetDatabaseName", targetDatabaseName);

            dlTargetServers.Items.Add(currentItem);
        }

        ListItem liUsersDefault = new ListItem();
        liUsersDefault.Text = "Select a Test User";
        liUsersDefault.Value = liUsersDefault.Text;

        dlTestUsers.Items.Add(liUsersDefault);

        btnSystemUnderTestSaveUpdate.Text = "New";
        btnSystemUnderTestDelete.Visible = false;
        btnSystemRefresh.Enabled = false;

        txtRunId.InnerHtml = DateTime.Now.ToString();

        if(!IsPostBack)
        {
            UpdateTestSystemForm();
            GetApplicationsToTest();
        }
    }

    protected void GetApplicationsToTest()
    {
        StringBuilder sbApps = new StringBuilder();

        var appCount = 0;

        if (!IsPostBack)
        {
            sbApps.Append("    <div id='div_ApplicationsToTest' style='border-bottom: dashed 0px #c0c0c0; padding: 0; padding-left: 5px; padding-bottom: 15px; margin-bottom: 15px;'>");

            sbApps.Append("    <div id='div_Select_All_Applications' style='border: solid 0px #ff0000; padding: 0; padding-left: 0px; height: 25px;'>");
            sbApps.Append("      <span>");
            sbApps.Append("            <input id='chk_Select_All_Applications' onclick='javascript: SelectAllApplications();' style='margin-right: 5px;' type='checkbox' value='Select all Applications' />");
            sbApps.Append("      <span>");
            sbApps.Append("      <span style='font-weight: bold; position: relative; top: -2px;'>Select all Applications</span>");
            sbApps.Append("    </div>");

            var directories = Directory.GetDirectories(ScriptsDir);
            foreach (var di in directories)
            {
                var tmpDir = di.Split('\\');
                var dirName = tmpDir[tmpDir.Length - 1].Replace("_", " ");

                appCount++;

                sbApps.Append("    <div id='div_" + dirName + "' style='border: solid 0px #ff0000; padding: 0; padding-left: 20px; height: 25px;'>");
                sbApps.Append("      <span>");
                sbApps.Append("            <input id='chk_" + dirName.Replace(" ", "_") + "' onclick='javascript: GetApplicationAreas(&quot;" + dirName.Replace(" ", "_") + "&quot;);' style='margin-right: 5px; height: 25px; padding: 0;' type='checkbox' />");
                sbApps.Append("      <span>");

                sbApps.Append("      <span style='position: relative; top: -8px;'>" + appCount + ". " + dirName + "</span>");
                sbApps.Append("    </div>");
            }

            sbApps.Append("    </div>");

            divApplicationsToTest.InnerHtml = sbApps.ToString();
    }
    }

    //--------------- Operational Test ---------------------------------
    #region Operational Test Button Events

    protected void btnUpdateTestSystemConfig_Click(object sender, EventArgs e)
    {
        var mOtUtils = new OperationalTestUtils();
        var pOpTestSystem = mOtUtils.Read();
        //check if anything was updated in the Test System's settings
        var isChanged = false;
        if (pOpTestSystem.TestSystemHost != txtTestSystemHost.Value)
        {
            isChanged = true;
            pOpTestSystem.TestSystemHost = txtTestSystemHost.Value;
        }
        if (pOpTestSystem.TestSystemIp != txtTestSystemIP.Value)
        {
            isChanged = true;
            pOpTestSystem.TestSystemIp = txtTestSystemIP.Value;
        }
        if (pOpTestSystem.RunInterval.ToString() != txtTestSystemRunInterval.Value)
        {
            int newinterval;
            if (int.TryParse(txtTestSystemRunInterval.Value, out newinterval))
            {
                isChanged = true;
                pOpTestSystem.RunInterval = newinterval;
            }
            else
            {
                txtTestSystemRunInterval.Value = pOpTestSystem.RunInterval.ToString();
            }
        }
        if (pOpTestSystem.ResultsLookupDays.ToString() != txtResultsLookupDays.Value)
        {
            int newvalue;
            if (int.TryParse(txtResultsLookupDays.Value, out newvalue))
            {
                isChanged = true;
                pOpTestSystem.ResultsLookupDays = newvalue;
            }
            else
            {
                txtResultsLookupDays.Value = pOpTestSystem.RunInterval.ToString();
            }
        }

        // Check if Email Settings changed
        if (Security.DecodeAndDecrypt(pOpTestSystem.EmailSettings.eEmailHost, cs.DefaultClientId) != txtTestSystemEmailServer.Value)
        {
            isChanged = true;
            pOpTestSystem.EmailSettings.eEmailHost = Security.EncryptAndEncode(txtTestSystemEmailServer.Value, cs.DefaultClientId);
        }
        if (pOpTestSystem.EmailSettings.EmailPort.ToString() != txtTestSystemEmailPort.Value)
        {
            isChanged = true;
            int mPort;
            if (int.TryParse(txtTestSystemEmailPort.Value, out mPort) == false)
            {
                lbError.Text = @"Invalid email port, must be a number!";
                return;
            }
            pOpTestSystem.EmailSettings.EmailPort = mPort;
        }
        if (cbTestSystemEmailEnableSsl.Checked != pOpTestSystem.EmailSettings.EmailEnableSsl)
        {
            isChanged = true;
            if (cbTestSystemEmailEnableSsl.Checked)
                pOpTestSystem.EmailSettings.EmailEnableSsl = true;
            else
                pOpTestSystem.EmailSettings.EmailEnableSsl = false;
        }
        if (cbTestSystemEmailUseDefaultCredentials.Checked != pOpTestSystem.EmailSettings.EmailUseDefaultCredentials)
        {
            isChanged = true;
            if (cbTestSystemEmailUseDefaultCredentials.Checked)
                pOpTestSystem.EmailSettings.EmailUseDefaultCredentials = true;
            else
                pOpTestSystem.EmailSettings.EmailUseDefaultCredentials = false;
        }

        if (Security.DecodeAndDecrypt(pOpTestSystem.EmailSettings.eEmailLoginUserName, cs.DefaultClientId) != txtTestSystemEmailLogin.Value)
        {
            isChanged = true;
            pOpTestSystem.EmailSettings.eEmailLoginUserName = Security.EncryptAndEncode(txtTestSystemEmailLogin.Value, cs.DefaultClientId);

        }
        if (Security.DecodeAndDecrypt(pOpTestSystem.EmailSettings.eEmailLoginPassword, cs.DefaultClientId) != txtTestSystemEmailPWD.Value)
        {
            isChanged = true;
            pOpTestSystem.EmailSettings.eEmailLoginPassword = Security.EncryptAndEncode(txtTestSystemEmailPWD.Value, cs.DefaultClientId);

        }
        if (pOpTestSystem.EmailSettings.EmailFromAddress != txtTestSystemEmailFromAddress.Value)
        {
            isChanged = true;
            pOpTestSystem.EmailSettings.EmailFromAddress = txtTestSystemEmailFromAddress.Value;
        }
        if (pOpTestSystem.EmailSettings.EmailBodyTemplate != txtTestSystemEmailBodyTemplate.Value)
        {
            isChanged = true;
            pOpTestSystem.EmailSettings.EmailBodyTemplate = txtTestSystemEmailBodyTemplate.Value;
        }
        if (pOpTestSystem.EmailSettings.EmailSubject != txtTestSystemEmailSubject.Value)
        {
            isChanged = true;
            pOpTestSystem.EmailSettings.EmailSubject = txtTestSystemEmailSubject.Value;
        }

        // check if Text(SMS) settings changed
        if (pOpTestSystem.SmsSettings.URL != txtSMSURL.Value)
        {
            isChanged = true;
            pOpTestSystem.SmsSettings.URL = txtSMSURL.Value;
        }
        if (Security.DecodeAndDecrypt(pOpTestSystem.SmsSettings.eBatchId, cs.DefaultClientId) != txtSMSBatchId.Value)
        {
            isChanged = true;
            pOpTestSystem.SmsSettings.eBatchId = Security.EncryptAndEncode(txtSMSBatchId.Value, cs.DefaultClientId);
        }
        if (Security.DecodeAndDecrypt(pOpTestSystem.SmsSettings.eSid, cs.DefaultClientId) != txtSMSSid.Value)
        {
            isChanged = true;
            pOpTestSystem.SmsSettings.eSid = Security.EncryptAndEncode(txtSMSSid.Value, cs.DefaultClientId);
        }
        if (Security.DecodeAndDecrypt(pOpTestSystem.SmsSettings.eAuthToken, cs.DefaultClientId) != txtSMSAuthToken.Value)
        {
            isChanged = true;
            pOpTestSystem.SmsSettings.eAuthToken = Security.EncryptAndEncode(txtSMSAuthToken.Value, cs.DefaultClientId);
        }
        if (pOpTestSystem.SmsSettings.TextMessageTemplate != txtSMSMessageTemplate.Value)
        {
            isChanged = true;
            pOpTestSystem.SmsSettings.TextMessageTemplate = txtSMSMessageTemplate.Value;
        }

        // If anything changed update settings in database
        if (isChanged)
        {
            pOpTestSystem.LastUpdate = DateTime.UtcNow;
            UpdateOperationalTestObject(pOpTestSystem);
        }
    }

    protected void btnSystemRefresh_Click(object sender, EventArgs e)
    {
        UpdateTestSystemForm();
    }

    // ======= System Under Test Configuration ==================================
    protected void btnSystemUnderTestSaveUpdate_Click(object sender, EventArgs e)
    {
        var mButton = (Button)sender;

        lbError.Text = lbErrSUT.Text = String.Empty;
        if (String.IsNullOrEmpty(txtSystemUnderTestName.Value))
        {
            lbError.Text = lbErrSUT.Text = @"System under test name required!";
            return;
        }
        // Is this a new or update
        if (mButton.Text == New)
        {
            // check if system under test name is unique
            var mSUT = dlSystemsUnderTest.Items.FindByText(txtSystemUnderTestName.Value.Trim());
            if (mSUT != null)
            {
                lbError.Text = lbErrSUT.Text = @"Name must be unique";
                return;
            }
        }
        if (dlSystemUnderTestContactList.SelectedItem.Text == None)
        {
            if ((String.IsNullOrEmpty(txtContactFirstName.Value)) ||
                (String.IsNullOrEmpty(txtContactLastName.Value)))
            {
                lbError.Text = lbErrSUT.Text = @"Need contact information!";
                return;
            }
            //Add Contact info to New System Under Test
            var errormsg = String.Empty;
            if (String.IsNullOrEmpty(txtSystemUnderTestName.Value.Trim()))
                errormsg = @"System under test name, ";
            if (String.IsNullOrEmpty(txtContactFirstName.Value.Trim()))
                errormsg += "First name, ";
            if (String.IsNullOrEmpty(txtContactLastName.Value.Trim()))
                errormsg += "Last name, ";
            if (String.IsNullOrEmpty(txtContactMobilePhone.Value.Trim()))
                errormsg += "Phone Number, ";
            if (String.IsNullOrEmpty(txtContactEmailaddress.Value.Trim()))
                errormsg += "Email, ";
            if ((cbContactEmail.Checked == false) && (cbContactSMS.Checked == false))
                errormsg += "Contact Method";

            if (!String.IsNullOrEmpty(errormsg))
            {
                lbError.Text = lbErrSUT.Text = errormsg + @"required!";
                return;
            }
            var mNewSUT = new OperationalTestSystemUnderTest { SystemName = txtSystemUnderTestName.Value.Trim() };
            var mNewContact = new OperationalTestContact
            {
                eFirstName =
                    Security.EncryptAndEncode(txtContactFirstName.Value.Replace(" ", ""), cs.DefaultClientId),
                eLastName = Security.EncryptAndEncode(txtContactLastName.Value.Replace(" ", ""), cs.DefaultClientId),
                eEmailAddress =
                    Security.EncryptAndEncode(txtContactEmailaddress.Value.Replace(" ", ""), cs.DefaultClientId),
                ePhoneNumber =
                    Security.EncryptAndEncode(txtContactMobilePhone.Value.Replace(" ", ""), cs.DefaultClientId),
                SendSMS = cbContactSMS.Checked,
                SendEmail = cbContactEmail.Checked
            };
            mNewSUT.ContactList.Add(mNewContact);
            AddSystemUnderTest(mNewSUT);
            txtSystemUnderTestName.Value = String.Empty;
            // clear contact form                
            txtContactFirstName.Value = String.Empty;
            txtContactLastName.Value = String.Empty;
            txtContactMobilePhone.Value = String.Empty;
            txtContactEmailaddress.Value = String.Empty;
            cbContactEmail.Checked = false;
            cbContactSMS.Checked = false;
            SetDDLFromOperationalTestObject(null);
            dlSystemsUnderTest.Focus();

            return;
        }
        // add contacts to Operational Test Contact List
        foreach (ListItem mContact in dlSystemUnderTestContactList.Items)
        {
            // var mContactName = mContact.Text;
            var mContactDetails = mContact.Value.Split(char.Parse(dk.ItemSep));
            if (mContactDetails.Count() == 4)
            {
                var mNewContact = new OperationalTestContact
                {
                    DateLastUpdate = DateTime.UtcNow
                };
                foreach (var mContactItem in mContactDetails)
                {
                    if (mContactItem.StartsWith(dkui.FirstName))
                        mNewContact.eFirstName = Security.EncryptAndEncode(mContactItem.Replace(dkui.FirstName, ""), cs.DefaultClientId);
                    if (mContactItem.StartsWith(dkui.LastName))
                        mNewContact.eLastName = Security.EncryptAndEncode(mContactItem.Replace(dkui.LastName, ""), cs.DefaultClientId);
                    if (mContactItem.StartsWith(dkui.PhoneNumber))
                        mNewContact.ePhoneNumber = Security.EncryptAndEncode(mContactItem.Replace(dkui.PhoneNumber, ""), cs.DefaultClientId);
                    if (mContactItem.StartsWith(dkui.EmailAddress))
                        mNewContact.eEmailAddress = Security.EncryptAndEncode(mContactItem.Replace(dkui.EmailAddress, ""), cs.DefaultClientId);
                    if (mContactItem.StartsWith("SMSContact"))
                        mNewContact.SendSMS = mContactItem.ToLower().Contains("true");
                    if (mContactItem.StartsWith("EmailContact"))
                        mNewContact.SendSMS = mContactItem.ToLower().Contains("true");
                }
                AddContactToSystemUnderTest(txtSystemUnderTestName.Value.Trim(), mNewContact);
            }
        }
        SetDDLFromOperationalTestObject(null);
        dlSystemsUnderTest.Focus();
        txtSystemUnderTestName.Value = String.Empty;
    }

    protected void btnSystemUnderTestCancel_Click(object sender, EventArgs e)
    {
        lbErrorTestConfiguration.Text = lbError.Text = string.Empty;
        ClearSystemUnderTestSelection();
        dlSystemsUnderTest.Focus();
    }

    protected void btnSaveUpdateTest_Click(object sender, EventArgs e)
    {
        var mButton = (Button)sender;

        lbErrorTestConfiguration.Text = lbError.Text = string.Empty;
        var mSUTName = dlSystemsUnderTest.SelectedItem.Text;
        if (mSUTName == PS)
        {
            lbErrorTestConfiguration.Text = lbError.Text = @"<br />Select a system under test then retry!";
            return;
        }
        if (mSUTName == None)
        {
            lbErrorTestConfiguration.Text = lbError.Text = @"<br />Create a system under test first!";
            return;
        }
        if (String.IsNullOrEmpty(txtTestName.Value))
        {
            lbErrorTestConfiguration.Text = lbError.Text = @"<br />Test Name required!";
            return;
        }
        if (String.IsNullOrEmpty(txtTestScript.Value))
        {
            lbErrorTestConfiguration.Text = lbError.Text = @"<br />Test Script required!";
            return;
        }
        var mv = GetConfigurationVariableList("~");
        var mTestConfig = new OperationalTestConfig
        {
            eTestName = Security.EncryptAndEncode(txtTestName.Value.Trim(), cs.DefaultClientId),
            eTestScript = Security.EncryptAndEncode(txtTestScript.Value.Trim(), cs.DefaultClientId),
            eTestCommandLineVariables = Security.EncryptAndEncode(mv, cs.DefaultClientId)
        };
        txtTestScript.Value = String.Empty;
        txtTestVariableName.Value = String.Empty;
        txtTestVariableValue.Value = String.Empty;
        txtTestVariableList.Text = String.Empty;
        if (mButton.Text == New)
        {
            var item = dlTests.Items.FindByText(txtTestName.Value);
            if (item != null)
            {
                lbErrorTestConfiguration.Text =
                    lbError.Text = @"<br />Test Name [" + txtTestName.Value + @"] must be unique!";
                txtTestName.Value = String.Empty;
                return;
            }
        }
        txtTestName.Value = String.Empty;
        lbErrorTestConfiguration.Text = lbError.Text =
            AddOperationalTestConfigToSystemUnderTest(mSUTName, mTestConfig);
    }

    protected void btnVariableAdd_Click(object sender, EventArgs e)
    {
        lbErrorConfigurationVariable.Text = lbError.Text = string.Empty;
        if (String.IsNullOrEmpty(txtTestName.Value.Trim()))
        {
            lbErrorConfigurationVariable.Text = lbError.Text = @"<br />Enter Test name first!";
            return;
        }
        // Check if Variable Name entered and is unique and is valid(no colons)
        if (String.IsNullOrEmpty(txtTestVariableName.Value.Trim()))
        {
            lbErrorConfigurationVariable.Text = lbError.Text = @"<br />Variable Name required!";
            return;
        }
        var VNameUpperCase = txtTestVariableName.Value.Trim().Replace(" ", "").ToUpper();
        if (VNameUpperCase.Contains(":"))
        {
            lbErrorConfigurationVariable.Text = lbError.Text = @"<br />Invalid Variable Name, contains a colon!";
            return;
        }
        // check if in hidden ddl
        var item = dlTextVariableList.Items.FindByText(VNameUpperCase);
        if (item != null)
        {
            lbErrorConfigurationVariable.Text =
                lbError.Text = @"<br />Variable Name [" + VNameUpperCase + @"] must be unique!";
            return;
        }
        // Check if Variable value entered and is valid(no colons)
        if (String.IsNullOrEmpty(txtTestVariableValue.Value.Trim()))
        {
            lbErrorConfigurationVariable.Text = lbError.Text = @"<br />Variable value required!";
            return;
        }
        var mValue = txtTestVariableValue.Value.Trim();
        if (txtTestVariableValue.Value.Contains(":"))
        {
            lbErrorConfigurationVariable.Text = lbError.Text = @"<br />Invalid Variable value, contains a colon!";
            return;
        }
        // add to var list
        var newVar = new ListItem
        {
            Text = VNameUpperCase,
            Value = mValue
        };
        dlTextVariableList.Items.Add(newVar);
        txtTestVariableList.Text = GetConfigurationVariableList(Environment.NewLine);

    }
    
    protected void btnVariableDelete_Click(object sender, EventArgs e)
    {
        lbErrorConfigurationVariable.Text = lbError.Text = string.Empty;
        if (String.IsNullOrEmpty(txtTestName.Value.Trim()))
        {
            lbErrorConfigurationVariable.Text = lbError.Text = @"<br />Enter Test name first!";
            return;
        }
        // Check if Variable Name entered and is unique and is valid(no colons)
        if (String.IsNullOrEmpty(txtTestVariableName.Value.Trim()))
        {
            lbErrorConfigurationVariable.Text = lbError.Text = @"<br />Enter variable Name to delete!";
            return;
        }
        var VNameUpperCase = txtTestVariableName.Value.Trim().Replace(" ", "").ToUpper();
        // check if in hidden ddl
        var item = dlTextVariableList.Items.FindByText(VNameUpperCase);
        if (item == null)
        {
            lbErrorConfigurationVariable.Text =
                lbError.Text = @"<br />Variable Name mismatch [" + VNameUpperCase + @"]!";
            return;
        }
        dlTextVariableList.Items.Remove(item);
        txtTestVariableList.Text = GetConfigurationVariableList(Environment.NewLine);
    }

    protected void btnDeleteTest_Click(object sender, EventArgs e)
    {
        lbError.Text = lbErrSUT.Text = String.Empty;
        var mSystemUnderTestName = dlSystemsUnderTest.SelectedItem.Text;
        if (mSystemUnderTestName == PS || mSystemUnderTestName == None)
        {
            lbError.Text = lbErrSUT.Text = @"System Under Test not selected!";
            return;
        }
        var mTestName = dlTests.SelectedItem.Text;
        if (mTestName == PS || mTestName == None)
        {
            lbError.Text = lbErrSUT.Text = @"Test not selected!";
            return;
        }
        txtTestName.Value = String.Empty;
        txtTestScript.Value = String.Empty;
        txtTestVariableName.Value = String.Empty;
        txtTestVariableValue.Value = String.Empty;
        txtTestVariableList.Text = String.Empty;
        lbErrorTestConfiguration.Text = lbError.Text =
            DeleteOperationalTestConfigFromSystemUnderTest(mSystemUnderTestName, mTestName);
    }

    protected void btnSystemUnderTestDelete_Click(object sender, EventArgs e)
    {
        lbErrorTestConfiguration.Text = lbError.Text = string.Empty;
        var mSUTName = dlSystemsUnderTest.SelectedItem.Text;
        DeleteSystemUnderTestByName(mSUTName);
    }

    protected void btnAddUpdateContact_Click(object sender, EventArgs e)
    {
        lbErrorTestConfiguration.Text = lbError.Text = lbErrSUT.Text = String.Empty;
        var mButton = (Button)sender;

        var pSystemUnderTestName = txtSystemUnderTestName.Value.Trim();
        var errormsg = String.Empty;
        if (String.IsNullOrEmpty(txtSystemUnderTestName.Value.Trim()))
            errormsg = @"System under test name, ";
        if (String.IsNullOrEmpty(txtContactFirstName.Value.Trim()))
            errormsg += "First name, ";
        if (String.IsNullOrEmpty(txtContactLastName.Value.Trim()))
            errormsg += "Last name, ";
        if (String.IsNullOrEmpty(txtContactMobilePhone.Value.Trim()))
            errormsg += "Phone Number, ";
        if (String.IsNullOrEmpty(txtContactEmailaddress.Value.Trim()))
            errormsg += "Email, ";

        if (!String.IsNullOrEmpty(errormsg))
        {
            lbError.Text = lbErrSUT.Text = errormsg + @"required!";
            return;
        }
        var mNewContact = new OperationalTestContact();
        mNewContact.eFirstName = Security.EncryptAndEncode(txtContactFirstName.Value.Replace(" ", ""), cs.DefaultClientId);
        mNewContact.eLastName = Security.EncryptAndEncode(txtContactLastName.Value.Replace(" ", ""), cs.DefaultClientId);
        mNewContact.eEmailAddress = Security.EncryptAndEncode(txtContactEmailaddress.Value, cs.DefaultClientId);
        mNewContact.ePhoneNumber = Security.EncryptAndEncode(txtContactMobilePhone.Value, cs.DefaultClientId);
        mNewContact.SendSMS = cbContactSMS.Checked;
        mNewContact.SendEmail = cbContactEmail.Checked;

        if (mButton.Text == New)
            AddContactToSystemUnderTest(pSystemUnderTestName, mNewContact);
        else
            UpdateContactForSystemUnderTest(pSystemUnderTestName, mNewContact);
        ClearSystemUnderTestSelection();
        SetDDLFromOperationalTestObject(null);
    }

    protected void btnDeleteContact_Click(object sender, EventArgs e)
    {
        lbErrorTestConfiguration.Text = lbError.Text = string.Empty;
        // contact name to delete
        var mContactToDelete = dlSystemUnderTestContactList.SelectedItem.Text;
        if (mContactToDelete == None) return;
        if (mContactToDelete == PS) return;

        DeleteContactByName(txtSystemUnderTestName.Value, mContactToDelete);
        ClearSystemUnderTestSelection();
        SetDDLFromOperationalTestObject(null);
    }

    #endregion

    #region Operational Test DDL Events

    protected void dlSystemUnderTestContact_Changed(object sender, EventArgs e)
    {
        lbErrorTestConfiguration.Text = lbError.Text = string.Empty;
        var mContactName = dlSystemUnderTestContactList.SelectedItem.Text;
        var mContactValue = dlSystemUnderTestContactList.SelectedValue;
        // no contact selected
        txtContactFirstName.Value = String.Empty;
        txtContactLastName.Value = String.Empty;
        txtContactMobilePhone.Value = String.Empty;
        txtContactEmailaddress.Value = String.Empty;
        cbContactEmail.Checked = false;
        cbContactSMS.Checked = false;

        if (mContactName == PS)
        {
            btnAddContact.Visible = true;
            btnDeleteContact.Visible = false;
            btnUpdateContact.Visible = false;
            return;
        }
        if (String.IsNullOrEmpty(mContactValue))
        {
            lbError.Text = lbErrSUT.Text = @"Invalid Contact Details for " + mContactName + @"!";
            return;
        }
        btnAddContact.Visible = false;
        btnDeleteContact.Visible = true;
        btnUpdateContact.Visible = true;
        var mContactDetails = mContactValue.Split(char.Parse(dk.ItemSep));
        foreach (var mContactDetail in mContactDetails)
        {
            if (mContactDetail.StartsWith(dkui.FirstName))
            {
                txtContactFirstName.Value = mContactDetail.Replace(dkui.FirstName, "");
                continue;
            }
            if (mContactDetail.StartsWith(dkui.LastName))
            {
                txtContactLastName.Value = mContactDetail.Replace(dkui.LastName, "");
                continue;
            }
            if (mContactDetail.StartsWith(dkui.PhoneNumber))
            {
                txtContactMobilePhone.Value = mContactDetail.Replace(dkui.PhoneNumber, "");
                continue;
            }
            if (mContactDetail.StartsWith(dkui.EmailAddress))
            {
                txtContactEmailaddress.Value = mContactDetail.Replace(dkui.EmailAddress, "");
                continue;
            }
            if (mContactDetail.StartsWith("SMSContact"))
            {
                if (mContactDetail.ToLower().Contains("true"))
                    cbContactSMS.Checked = true;
                else
                    cbContactSMS.Checked = false;
                continue;
            }
            if (mContactDetail.StartsWith("EmailContact"))
            {
                if (mContactDetail.ToLower().Contains("true"))
                    cbContactEmail.Checked = true;
                else
                    cbContactEmail.Checked = false;
            }
        }
    }

    protected void dlTests_Changed(object sender, EventArgs e)
    {
        lbErrorTestConfiguration.Text = lbError.Text = string.Empty;

        if (dlTests.SelectedItem.Text == PS || dlTests.SelectedItem.Text == None)
        {
            txtTestName.Value = String.Empty;
            txtTestScript.Value = String.Empty;
            txtTestVariableList.Text = String.Empty;
            btnSaveUpdateTest.Text = New;
            btnDeleteTest.Visible = false;
            dlTextVariableList.Items.Clear();
        }
        else
        {
            var mOperationalTest = GetOperationalTestObject();
            foreach (var mSUT in mOperationalTest.SystemsUnderTestList)
            {
                // first find the system under test object
                if (mSUT.SystemName == dlSystemsUnderTest.SelectedItem.Text)
                {
                    if (mSUT.TestConfigList.Any())
                    {
                        foreach (var mTestConfig in mSUT.TestConfigList)
                        {
                            if (Security.DecodeAndDecrypt(mTestConfig.eTestName, cs.DefaultClientId) == dlTests.SelectedItem.Text)
                            {
                                btnSaveUpdateTest.Text = Update;
                                btnDeleteTest.Visible = true;
                                txtTestName.Value = Security.DecodeAndDecrypt(mTestConfig.eTestName, cs.DefaultClientId);
                                txtTestScript.Value = Security.DecodeAndDecrypt(mTestConfig.eTestScript, cs.DefaultClientId);
                                dlTextVariableList.Items.Clear();
                                if (!String.IsNullOrEmpty(mTestConfig.eTestCommandLineVariables))
                                {
                                    var mv = Security.DecodeAndDecrypt(mTestConfig.eTestCommandLineVariables, cs.DefaultClientId);
                                    if (!String.IsNullOrEmpty(mv))
                                    {
                                        var mVariables = mv.Split('~');
                                        foreach (var mVariable in mVariables)
                                        {
                                            var NameValue = mVariable.Split(':');
                                            var mLi = new ListItem { Text = NameValue[0], Value = NameValue[1] };
                                            dlTextVariableList.Items.Add(mLi);
                                        }
                                        txtTestVariableList.Text = mv.Replace("~", Environment.NewLine);
                                    }
                                }
                            }
                        }
                    }
                    break;
                }
            }
        }
    }

    protected void dlSystemsUnderTest_Changed(object sender, EventArgs e)
    {
        lbErrorTestConfiguration.Text = lbError.Text = string.Empty;
        var mSUTName = dlSystemsUnderTest.SelectedItem.Text;
        var mSUTValue = dlSystemsUnderTest.SelectedValue;
        if (mSUTName == PS)
        {
            btnSystemUnderTestSaveUpdate.Text = New;
            btnSystemUnderTestDelete.Visible = false;

            // hide test configuration form
            configurationsDisplay.Visible = false;
            divConfigTestsRow.Visible = false;
            divConfigDotsRow.Visible = false;
            btnDeleteTest.Visible = false;
            btnSaveUpdateTest.Text = New;

            dlTests.Items.Clear();
            dlSystemUnderTestContactList.Items.Clear();
            {
                var mLi = new ListItem();
                mLi.Text = mLi.Value = None;
                dlSystemUnderTestContactList.Items.Add(mLi);
                btnAddContact.Visible = false;
                btnDeleteContact.Visible = false;
                btnUpdateContact.Visible = false;
            }
            ClearSystemUnderTestSelection();

            btnDeleteContact.Visible = false;
            btnUpdateContact.Visible = false;
            systemsDisplay.Visible = true;
            return;
        }

        PopulateTestsDDL(null);
        PopulateContactDDL(mSUTValue);
        txtSystemUnderTestName.Value = mSUTName;
        btnSystemUnderTestSaveUpdate.Text = Update;
        btnSystemUnderTestDelete.Visible = true;
        // Show test configuration form
        configurationsDisplay.Visible = true;
        divConfigTestsRow.Visible = true;
        divConfigDotsRow.Visible = true;
        btnSaveUpdateTest.Text = New;
    }

    #endregion

    #region Operational Test Helper Methods
    
    protected void ClearSystemUnderTestSelection()
    {
        // no contact selected
        txtContactFirstName.Value = String.Empty;
        txtContactLastName.Value = String.Empty;
        txtContactMobilePhone.Value = String.Empty;
        txtContactEmailaddress.Value = String.Empty;
        cbContactEmail.Checked = false;
        cbContactSMS.Checked = false;
        txtSystemUnderTestName.Value = String.Empty;
    }
    
    protected void UpdateTestSystemForm()
    {
        var mOpTestSystem = GetOperationalTestObject();
        if (mOpTestSystem == null)
        {
            lbError.Text = @"Unable to get Operational Test documnet from database, Please retry!";
            btnUpdateTestSystemConfig.Visible = false;
            btnSystemRefresh.Visible = true;
            return;
        }
        lbError.Text = String.Empty;
        btnUpdateTestSystemConfig.Visible = true;
        btnSystemRefresh.Visible = false;

        txtTestSystemName.Value = mOpTestSystem.TestSystemName;
        txtTestSystemRunInterval.Value = mOpTestSystem.RunInterval.ToString();
        txtResultsLookupDays.Value = mOpTestSystem.ResultsLookupDays.ToString();
        txtTestSystemLastRun.Value = mOpTestSystem.LastTimeTestRan.ToString();
        txtTestSystemIP.Value = mOpTestSystem.TestSystemIp;
        txtTestSystemHost.Value = mOpTestSystem.TestSystemHost;
        txtTestSystemCountSystemsUnderTest.Value = mOpTestSystem.SystemsUnderTestList.Count().ToString();
        // email settings
        txtTestSystemEmailServer.Value = Security.DecodeAndDecrypt(mOpTestSystem.EmailSettings.eEmailHost, cs.DefaultClientId);
        txtTestSystemEmailPort.Value = mOpTestSystem.EmailSettings.EmailPort.ToString();
        cbTestSystemEmailEnableSsl.Checked = mOpTestSystem.EmailSettings.EmailEnableSsl;
        cbTestSystemEmailUseDefaultCredentials.Checked = mOpTestSystem.EmailSettings.EmailUseDefaultCredentials;
        txtTestSystemEmailLogin.Value = Security.DecodeAndDecrypt(mOpTestSystem.EmailSettings.eEmailLoginUserName, cs.DefaultClientId);
        txtTestSystemEmailPWD.Value = Security.DecodeAndDecrypt(mOpTestSystem.EmailSettings.eEmailLoginPassword, cs.DefaultClientId);
        txtTestSystemEmailFromAddress.Value = mOpTestSystem.EmailSettings.EmailFromAddress;
        txtTestSystemEmailSubject.Value = mOpTestSystem.EmailSettings.EmailSubject;
        txtTestSystemEmailBodyTemplate.Value = mOpTestSystem.EmailSettings.EmailBodyTemplate;
        // Test(sms) settings
        lbSMSProviderName.Text = mOpTestSystem.SmsSettings.ProviderName;
        txtSMSURL.Value = mOpTestSystem.SmsSettings.URL;
        txtSMSBatchId.Value = Security.DecodeAndDecrypt(mOpTestSystem.SmsSettings.eBatchId, cs.DefaultClientId);
        txtSMSSid.Value = Security.DecodeAndDecrypt(mOpTestSystem.SmsSettings.eSid, cs.DefaultClientId);
        txtSMSAuthToken.Value = Security.DecodeAndDecrypt(mOpTestSystem.SmsSettings.eAuthToken, cs.DefaultClientId);
        txtSMSMessageTemplate.Value = mOpTestSystem.SmsSettings.TextMessageTemplate;
        SetDDLFromOperationalTestObject(mOpTestSystem);
    }

    protected void SetDDLFromOperationalTestObject(OperationalTest pOpTestSystem)
    {
        var mOpTestSystem = pOpTestSystem;
        if (pOpTestSystem == null)
        {
            mOpTestSystem = GetOperationalTestObject();
        }
        // hide test configuration form
        configurationsDisplay.Visible = false;
        divConfigTestsRow.Visible = false;
        divConfigDotsRow.Visible = false;
        btnDeleteTest.Visible = false;
        btnSaveUpdateTest.Text = New;

        dlSystemUnderTestContactList.Items.Clear();
        dlSystemsUnderTest.Items.Clear();
        dlTests.Items.Clear();
        {
            var mLi = new ListItem();
            mLi.Text = mLi.Value = None;
            dlSystemsUnderTest.Items.Add(mLi);
            dlSystemUnderTestContactList.Items.Add(mLi);
            dlTests.Items.Add(mLi);

            btnAddContact.Visible = false;
            btnDeleteContact.Visible = false;
            btnUpdateContact.Visible = false;
        }
        if (mOpTestSystem.SystemsUnderTestList.Any())
        {
            dlSystemsUnderTest.Items.Clear();
            {
                var mLi = new ListItem();
                mLi.Text = mLi.Value = PS;
                dlSystemsUnderTest.Items.Add(mLi);
            }
            foreach (var mSUT in mOpTestSystem.SystemsUnderTestList)
            {
                var mLi = new ListItem { Text = mSUT.SystemName };
                //Contacts
                if (!mSUT.ContactList.Any())
                {
                    mLi.Value = None;
                    btnAddContact.Visible = false;
                }
                else
                {
                    foreach (var mContact in mSUT.ContactList)
                    {
                        var fn = Security.DecodeAndDecrypt(mContact.eFirstName, cs.DefaultClientId);
                        var ln = Security.DecodeAndDecrypt(mContact.eLastName, cs.DefaultClientId);
                        var em = Security.DecodeAndDecrypt(mContact.eEmailAddress, cs.DefaultClientId);
                        var pn = Security.DecodeAndDecrypt(mContact.ePhoneNumber, cs.DefaultClientId);
                        mLi.Value += @"~"
                                    + dkui.FirstName + fn.Replace(" ", "") + dk.ItemSep
                                    + dkui.LastName + ln.Replace(" ", "") + dk.ItemSep
                                    + dkui.EmailAddress + em + dk.ItemSep
                                    + dkui.PhoneNumber + pn +
                                    dk.ItemSep + @"SMSContact" + mContact.SendSMS +
                                    dk.ItemSep + @"EmailContact" + mContact.SendEmail;
                    }
                    mLi.Value = mLi.Value.Trim('~');
                    btnAddContact.Visible = true;
                }
                dlSystemsUnderTest.Items.Add(mLi);
            }
        }
    }

    protected string PopulateTestsDDL(OperationalTest pOpTestSystem)
    {
        if (pOpTestSystem == null)
        {
            pOpTestSystem = GetOperationalTestObject();
            if (pOpTestSystem == null)
            {
                return @"Could not get OperationalTest, retry!";
            }
        }
        btnDeleteTest.Visible = false;
        dlTests.Items.Clear();
        {
            var mLi = new ListItem();
            mLi.Text = mLi.Value = None;
            dlTests.Items.Add(mLi);
        }

        foreach (var mSUT in pOpTestSystem.SystemsUnderTestList)
        {
            // first find the system under test object
            if (mSUT.SystemName == dlSystemsUnderTest.SelectedItem.Text)
            {
                if (mSUT.TestConfigList.Any())
                {
                    dlTests.Items.Clear();
                    {
                        var mLi = new ListItem();
                        mLi.Text = mLi.Value = PS;
                        dlTests.Items.Add(mLi);
                    }
                    foreach (OperationalTestConfig mTestConfig in mSUT.TestConfigList)
                    {
                        var mLi = new ListItem
                        {
                            Text = Security.DecodeAndDecrypt(mTestConfig.eTestName, cs.DefaultClientId),
                            Value = mTestConfig._id.ToString()
                        };
                        dlTests.Items.Add(mLi);
                    }
                }
                break;
            }
        }
        return null;
    }

    protected void PopulateContactDDL(String pContactList)
    {
        dlSystemUnderTestContactList.Items.Clear();
        {
            var mLi = new ListItem();
            mLi.Text = mLi.Value = None;
            dlSystemUnderTestContactList.Items.Add(mLi);
            btnAddContact.Visible = false;
            btnDeleteContact.Visible = false;
            btnUpdateContact.Visible = false;
            btnAddContact.Visible = true;
        }
        // no contact selected
        txtContactFirstName.Value = String.Empty;
        txtContactLastName.Value = String.Empty;
        txtContactMobilePhone.Value = String.Empty;
        txtContactEmailaddress.Value = String.Empty;
        cbContactEmail.Checked = false;
        cbContactSMS.Checked = false;
        //no contacts to parse
        if (pContactList.Contains(dk.ItemSep) == false)
        {
            lbErrSUT.Text = @"debug no contacts to parse";
            return;
        }
        dlSystemUnderTestContactList.Items.Clear();
        {
            var mLi = new ListItem();
            mLi.Text = mLi.Value = PS;
            dlSystemUnderTestContactList.Items.Add(mLi);
            btnAddContact.Visible = true;
        }
        dlSystemUnderTestContactList.ClearSelection();
        dlSystemUnderTestContactList.Items.FindByValue(PS).Selected = true;
        var mContacts = pContactList.Split('~');
        // loop through contacts
        foreach (var mContact in mContacts)
        {
            var mContactDetails = mContact.Split(char.Parse(dk.ItemSep));
            // loop through details
            var mContactFirstName = String.Empty;
            var mContactLastName = String.Empty;
            foreach (var mContactDetail in mContactDetails)
            {
                if (mContactDetail.StartsWith(dkui.FirstName))
                {
                    mContactFirstName = mContactDetail.Replace(dkui.FirstName, "");
                    continue;
                }
                if (mContactDetail.StartsWith(dkui.LastName))
                {
                    mContactLastName = mContactDetail.Replace(dkui.LastName, "");
                }
            }
            var mLi = new ListItem
            {
                Text = mContactFirstName + @" " + mContactLastName,
                Value = mContact
            };
            dlSystemUnderTestContactList.Items.Add(mLi);
        }
    }

    protected void FillContactForm(string pContactDetails)
    {
        var mContactDetails = pContactDetails.Split(char.Parse(dk.ItemSep));
        foreach (var mContactDetail in mContactDetails)
        {
            if (mContactDetail.StartsWith(dkui.FirstName))
            {
                txtContactFirstName.Value = mContactDetail.Replace(dkui.FirstName, "");
                continue;
            }
            if (mContactDetail.StartsWith(dkui.LastName))
            {
                txtContactLastName.Value = mContactDetail.Replace(dkui.LastName, "");
                continue;
            }
            if (mContactDetail.StartsWith(dkui.PhoneNumber))
            {
                txtContactMobilePhone.Value = mContactDetail.Replace(dkui.PhoneNumber, "");
                continue;
            }
            if (mContactDetail.StartsWith(dkui.EmailAddress))
            {
                txtContactEmailaddress.Value = mContactDetail.Replace(dkui.EmailAddress, "");
                continue;
            }
            if (mContactDetail.StartsWith("SMSContact"))
            {
                if (mContactDetail.ToLower().Contains("true"))
                    cbContactSMS.Checked = true;
                else
                    cbContactSMS.Checked = false;
                continue;
            }
            if (mContactDetail.StartsWith("EmailContact"))
            {
                if (mContactDetail.ToLower().Contains("true"))
                    cbContactEmail.Checked = true;
                else
                    cbContactEmail.Checked = false;
            }
        }
    }

    protected string GetConfigurationVariableList(string pDelim)
    {
        var mVarlist = "";
        foreach (ListItem mVar in dlTextVariableList.Items)
        {
            if (String.IsNullOrEmpty(mVarlist))
                mVarlist += mVar.Text + @":" + mVar.Value;
            else
                mVarlist += pDelim + mVar.Text + @":" + mVar.Value;
        }
        return mVarlist;
    }

    #endregion

    #region Operational Test Service Calls
    protected void AddSystemUnderTest(OperationalTestSystemUnderTest pNewSystemUnderTest)
    {
        var mOpUtils = new OperationalTestUtils();
        mOpUtils.AddSystemUnderTest(pNewSystemUnderTest);
    }

    protected void AddContactToSystemUnderTest(String pSystemUnderTestName, OperationalTestContact pNewContact)
    {
        var mOpUtils = new OperationalTestUtils();
        mOpUtils.AddContactToSystemUnderTest(pSystemUnderTestName, pNewContact);
    }

    protected void UpdateContactForSystemUnderTest(String pSystemUnderTestName, OperationalTestContact pNewContact)
    {
        var mOpUtils = new OperationalTestUtils();
        mOpUtils.UpdateContactForSystemUnderTest(pSystemUnderTestName, pNewContact);
    }

    protected void DeleteContactByName(String pSystemUnderTestName, String pContactNameToDelete)
    {
        var mOpUtils = new OperationalTestUtils();
        mOpUtils.DeleteContactFromSystemUnderTest(pSystemUnderTestName, pContactNameToDelete);
    }

    protected string AddOperationalTestConfigToSystemUnderTest(String pSUTName, OperationalTestConfig pTestConfig)
    {
        var mOpUtils = new OperationalTestUtils();
        mOpUtils.AddTestFromSystemUnderTest(pSUTName, pTestConfig);
        SetDDLFromOperationalTestObject(null);
        dlSystemsUnderTest.Focus();
        return string.Empty;
    }

    protected string DeleteOperationalTestConfigFromSystemUnderTest(String pSystemUnderTestName, String pTestName)
    {
        var mOpUtils = new OperationalTestUtils();
        mOpUtils.DeleteTestFromSystemUnderTest(pSystemUnderTestName, pTestName);
        SetDDLFromOperationalTestObject(null);
        dlSystemsUnderTest.Focus();
        return string.Empty;
    }

    protected OperationalTest GetOperationalTestObject()
    {
        try
        {
            var mOtUtils = new OperationalTestUtils();
            return mOtUtils.Read();
        }
        catch (Exception ex)
        {
            lbError.Text = ex.Message;
        }
        return null;
    }

    protected bool UpdateOperationalTestObject(OperationalTest pObject)
    {
        try
        {
            var mOtUtils = new OperationalTestUtils();
            var rtn = mOtUtils.Update(pObject);
            if (String.IsNullOrEmpty(rtn)) return true;
            lbError.Text = rtn;
        }
        catch (Exception ex)
        {
            lbError.Text = ex.Message;
        }
        return false;
    }

    protected bool DeleteSystemUnderTestByName(string mSUTName)
    {
        try
        {
            var mOtUtils = new OperationalTestUtils();
            var rtn = mOtUtils.DeleteSystemUnderTestByName(mSUTName);
            if (String.IsNullOrEmpty(rtn)) return true;
            lbError.Text = rtn;
        }
        catch (Exception ex)
        {
            lbError.Text = ex.Message;
        }
        return false;
    }
    #endregion
}