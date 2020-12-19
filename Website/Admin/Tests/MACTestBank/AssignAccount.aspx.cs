using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;
using System.Web;

using MACServices;
using cs = MACServices.Constants.Strings;
using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using sr = MACServices.Constants.ServiceResponse;

using tc = TestLib.TestConstants.TestBank;

public partial class MACUserApps_Web_Tests_MACTestBank_AssignAccount : System.Web.UI.Page
{
    private const string Test = "MTB_Asgn";
    private const string MTB = "MAC Test Bank";
    private const string SelectClient = "Select Client";
    private const string NoClient = "No Client";
    private const string SelectFile = "Select File";
    private const string NoTestFiles = "No Test Files";

    HiddenField _hiddenW;

    private const string UIDS = cs.Email + dk.KVSep + dv.ClientRegister +
                                dk.ItemSep + tc.PAN + dk.KVSep + dv.ClientRegister +
                                dk.ItemSep + "Prepaid Account" + dk.KVSep + dv.ClientRegister +
                                dk.ItemSep + "Credit Card" + dk.KVSep + dv.OpenRegister +
                                dk.ItemSep + "Debit Card" + dk.KVSep + dv.OpenRegister +
                                dk.ItemSep + "Client Credit Card" + dk.KVSep + dv.ClientRegister +
                                dk.ItemSep + "Client Debit Card" + dk.KVSep + dv.ClientRegister;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Master != null)
        {
            _hiddenW = (HiddenField)Master.FindControl("hiddenW");
            _hiddenW.Value = "54a83a43ead6362034d04bc2";
        }

        if (IsPostBack) return;
        var cid = GetCidUsingName();
        SendRequestToMacTestBankServer(cid, 
            dk.Request + dk.KVSep + tc.GetBankStatus + 
            dk.ItemSep + dk.CID + dk.KVSep + cid);
        FillddlTestUserFiles();
        FillddlUID();
        GetClients();
    }

    #region Button event handlers

    protected void btnUseSelectedFile1_Click(object sender, EventArgs e)
    {
        var mButton = (Button)sender;
        AddToLogAndDisplay("UseSelectedFile:" + mButton.Text);
        var cid = GetCidUsingName();
        lbError.Text = "";

        if (ddlTestUserFiles1.SelectedValue == SelectFile)
        {
            lbError.Text = @"Select a file.";
            return;
        }

        var filePath = Path.Combine(HttpContext.Current.Server.MapPath("../" + 
            TestLib.TestConstants.UserTestFiles), 
            ddlTestUserFiles1.SelectedValue);

        AddToLogAndDisplay(filePath);
        // Read the file and display it line by line.
        var mFile = new StreamReader(filePath);
        string line;
        while ((line = mFile.ReadLine()) != null)
        {

            if (String.IsNullOrWhiteSpace(line))
                continue;
            if (line.StartsWith(tc.MACBank) == false)
                continue;
            string request;

            if (mButton.Text.Contains("Unregister"))
                request = dk.Request + dk.KVSep + tc.DeleteUserAccount;
            else if (mButton.Text.Contains("Only"))
                request = dk.Request + dk.KVSep + tc.AssignAccount;
            else
                request = dk.Request + dk.KVSep + tc.AssignAndReg;

            request += dk.ItemSep + "Default" + dk.KVSep + "True" + line.Replace(tc.MACBank, "");

            request += dk.ItemSep + dk.CID + dk.KVSep + cid;

            AddToLogAndDisplay(request.Replace("|", ", "));

            var rply = SendRequestToMacTestBankServer(cid, request);
            if (rply.Item1)
            {
                var xmlDoc = new XmlDocument();
                if (rply.Item2 != null) xmlDoc.LoadXml(rply.Item2);
                var elemList = xmlDoc.GetElementsByTagName("macResponse");
                if (elemList.Count != 0)
                    AddToLogAndDisplay(elemList[0].InnerXml);
            }
            else
            {
                AddToLogAndDisplay(rply.Item2);
                lbError.Text = "";
            }
        }
        mFile.Close();
        SendRequestToMacTestBankServer(cid, 
            dk.Request + dk.KVSep + tc.GetBankStatus +
            dk.ItemSep + dk.CID + dk.KVSep + cid);
    }

    protected void btnAssignUser_Click(object sender, EventArgs e)
    {
        var clickedButton = sender as Button;
        if (clickedButton == null) return;
        AddToLogAndDisplay(clickedButton.ID);
        lbError.Text = "";

        var mRequest = CreateRequestDataFromForm();
        if (String.IsNullOrEmpty(mRequest))
            return;

        mRequest += dk.ItemSep + tc.Type + dk.KVSep + tc.User;
        mRequest += dk.ItemSep + tc.AccountName + dk.KVSep + ddlUID.SelectedItem.Text;

        if (clickedButton.ID == "btnRegAndAssign")
            mRequest = dk.Request + dk.KVSep + tc.AssignAndReg + mRequest;
        else
            mRequest = dk.Request + dk.KVSep + tc.AssignAccount + mRequest;

        var rply = SendRequestToMacTestBankServer(lbClientId.Text, mRequest);
        if (rply.Item1)
        {
            var xmlDoc = new XmlDocument();
            if (rply.Item2 != null) xmlDoc.LoadXml(rply.Item2);
            var elemList = xmlDoc.GetElementsByTagName("macResponse");
            AddToLogAndDisplay(elemList[0].InnerXml.Replace(dk.ItemSep, ", "));
        }
        else
        {
            AddToLogAndDisplay(rply.Item2);
            lbError.Text = "";
        }
    }

    protected void btnDeleteAccount_Click(object sender, EventArgs e)
    {
        AddToLogAndDisplay("btnDeleteAccount_Click");
        lbError.Text = "";

        var cid = GetCidUsingName();
        if (String.IsNullOrEmpty(txtFirstName.Text))
        {
            lbError.Text = @"First name required!";
            return;
        }

        if (String.IsNullOrEmpty(txtLastName.Text))
        {
            lbError.Text = @"Last name required!";
            return;
        }
        if (String.IsNullOrEmpty(txtEmailAdr.Text))
        {
            lbError.Text = @"Email address required!";
            return;
        }
        var mRequest = dk.Request + dk.KVSep + tc.DeleteUserAccount +
                       dk.ItemSep + dkui.FirstName + dk.KVSep + txtFirstName.Text.Trim() +
                       dk.ItemSep + dkui.LastName + dk.KVSep + txtLastName.Text.Trim() +
                       dk.ItemSep + dkui.EmailAddress + dk.KVSep + txtEmailAdr.Text.Trim() +
                       dk.ItemSep + tc.Type + dk.KVSep + tc.User +
                       dk.ItemSep + dk.CID + dk.KVSep + cid;

        var rply = SendRequestToMacTestBankServer(cid, mRequest);
        if (rply.Item1)
        {
            var xmlDoc = new XmlDocument();
            if (rply.Item2 != null) xmlDoc.LoadXml(rply.Item2);
            var elemList = xmlDoc.GetElementsByTagName("macResponse");
            AddToLogAndDisplay(elemList[0].InnerXml.Replace(dk.ItemSep, ", "));
        }
        else
        {
            AddToLogAndDisplay(rply.Item2);
            lbError.Text = "";
        }
    }

    protected void btnAssign_Click(object sender, EventArgs e)
    {
        var button = sender as Button;
        if (button == null) return;
        AddToLogAndDisplay("btn" + button.Text);
        lbError.Text = "";
        var mType = button.Text;
        var mShortName = txtMShortName.Text;
        var mPhoneNumber = txtMPhone.Text;
        var mEmail = txtMEmail.Text;
        var mName = txtMName.Text;
        if (string.IsNullOrEmpty(mShortName))
        {
            lbError.Text = @"Enter " + mType + @" short name!";
            return;
        }
        if (string.IsNullOrEmpty(mPhoneNumber))
        {
            lbError.Text = @"Enter " + mType + @" phone number!";
            return;
        }
        if (string.IsNullOrEmpty(mName))
        {
            lbError.Text = @"Enter " + mType + @" mName!";
            return;
        }
        if (string.IsNullOrEmpty(mEmail))
        {
            lbError.Text = @"Enter " + mType + @" email address!";
            return;
        }
        var cid = GetCidUsingName();

        var rply = SendRequestToMacTestBankServer(cid,
            dk.Request + dk.KVSep + tc.AssignAccount +
            dk.ItemSep + dk.CID + dk.KVSep + cid +
            dk.ItemSep + "Default" + dk.KVSep + "True" +
            dk.ItemSep + tc.Type + dk.KVSep + mType +
            dk.ItemSep + mType + dk.KVSep + mShortName +
            dk.ItemSep + tc.MerchantName + dk.KVSep + mName +
            dk.ItemSep + dkui.PhoneNumber + dk.KVSep + mPhoneNumber +
            dk.ItemSep + dkui.EmailAddress + dk.KVSep + mEmail);
        if (rply.Item1)
        {
            var xmlDoc = new XmlDocument();
            if (rply.Item2 != null) xmlDoc.LoadXml(rply.Item2);
            var elemList = xmlDoc.GetElementsByTagName("macResponse");
            AddToLogAndDisplay(elemList[0].InnerXml);
        }
        else
        {
            AddToLogAndDisplay(rply.Item2);
            lbError.Text = "";
        }
        SendRequestToMacTestBankServer(cid, 
            dk.Request + dk.KVSep + "GetBankStatus" +
            dk.ItemSep + dk.CID + dk.KVSep + cid);
    }

    #endregion

    #region Service call
    private Tuple<bool, string> SendRequestToMacTestBankServer(string pClientId, string requestData)
    {
        try
        {
            var dataStream = Encoding.UTF8.GetBytes("data=99" + pClientId.Length + pClientId.ToUpper() + StringToHex(requestData));
            var request = ConfigurationManager.AppSettings[cfg.MacServicesUrl] + 
                TestLib.TestConstants.MacTestBankUrl;
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
            var elemList = xmlDoc.GetElementsByTagName(sr.Error);
            if (elemList.Count != 0)
            {
                lbError.Text = String.Format("Error: returned from service {0}", elemList[0].InnerXml);
                return new Tuple<bool, string>(false, elemList[0].InnerXml);
            }
            elemList = xmlDoc.GetElementsByTagName("PANList");
            if (elemList.Count != 0)
            {
                hiddenPANList.Value = elemList[0].InnerXml;
            }
            elemList = xmlDoc.GetElementsByTagName("AccountHoldersList");
            if (elemList.Count != 0)
            {
                hiddenAccountHoldersList.Value = elemList[0].InnerXml;
            }
            elemList = xmlDoc.GetElementsByTagName("AccountNamesList");
            if (elemList.Count != 0)
            {
                hiddenAccountNamesList.Value = elemList[0].InnerXml;
            }
            elemList = xmlDoc.GetElementsByTagName("LoginNamesList");
            if (elemList.Count != 0)
            {
                hiddenLoginNamesList.Value = elemList[0].InnerXml;
            }
            return new Tuple<bool, string>(true, xmlDoc.OuterXml);
        }
        catch (Exception ex)
        {
            return new Tuple<bool, string>(false, "Error: " + ex.Message);
        }
    }
    #endregion

    #region Helper Methods
    protected string GetCidUsingName()
    {
        var mUtils = new Utils();
        var mClient = mUtils.GetClientUsingClientName(MTB);
        if (mClient == null)
            return cs.DefaultClientId;
        return mClient.ClientId.ToString();
    }

    private string CreateRequestDataFromForm()
    {
        var rd = new StringBuilder();

        if (ddlClient.SelectedValue == SelectClient)
        {
            lbError.Text = @"Select a Client!";
            return null;
        }
        rd.Append(dk.ItemSep + dk.ClientName + dk.KVSep + ddlClient.SelectedItem.Text.Replace("(Open)", "").Trim());
            rd.Append(dk.ItemSep + dk.CID + dk.KVSep + ddlClient.SelectedValue);
            lbClientId.Text = ddlClient.SelectedValue;

        if (rbClientRestricted.Checked)
            rd.Append(dk.ItemSep + dk.RegistrationType + dk.KVSep + dv.ClientRegister);
        else if (rbOpen.Checked)
            rd.Append(dk.ItemSep + dk.RegistrationType + dk.KVSep + dv.OpenRegister);
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
        if (String.IsNullOrEmpty(txtPhoneNumber.Text))
        {
            lbError.Text = @"Mobile Phone Required!";
            return null;
        }
        rd.Append(dk.ItemSep + dkui.PhoneNumber + dk.KVSep + txtPhoneNumber.Text);
        if (String.IsNullOrEmpty(txtEmailAdr.Text))
        {
            lbError.Text = @"email address required!";
            return null;
        }
        rd.Append(dk.ItemSep + dkui.EmailAddress + dk.KVSep + txtEmailAdr.Text);

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

        if (!String.IsNullOrEmpty(txtUnit.Text))
            rd.Append(dk.ItemSep + dkui.Unit + dk.KVSep + txtUnit.Text);

        if (!String.IsNullOrEmpty(txtCity.Text))
            rd.Append(dk.ItemSep + dkui.City + dk.KVSep + txtCity.Text);

        if (!String.IsNullOrEmpty(txtState.Text))
            rd.Append(dk.ItemSep + dkui.State + dk.KVSep + txtState.Text);

        if (!String.IsNullOrEmpty(txtZipCode.Text))
            rd.Append(dk.ItemSep + dkui.ZipCode + dk.KVSep + txtZipCode.Text);

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

    private void GetClients()
    {
        try
        {
            var url = ConfigurationManager.AppSettings[cfg.MacServicesUrl] +
                      TestLib.TestConstants.GetTestClientsInfoUrl;
            var request = WebRequest.Create(url);
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

    private void FillddlUID()
    {
        ddlUID.Items.Clear();
        var mUids = UIDS.Split(char.Parse(dk.ItemSep));
        foreach (var mUID in mUids)
        {
            var nametyp = mUID.Split(char.Parse(dk.KVSep));
            var li = new ListItem { Text = nametyp[0], Value = nametyp[1] };
            ddlUID.Items.Add(li);
        }
    }

    private void FillddlTestUserFiles()
    {
        ddlTestUserFiles1.Items.Clear();
        //ddlTestUserFiles2.Items.Clear();
        // get list of test files
        var path = HttpContext.Current.Server.MapPath("../");
        var folder = Path.Combine(path, TestLib.TestConstants.UserTestFiles);
        var d = new DirectoryInfo(folder);
        var Files = d.GetFiles("*.txt"); //Getting Text files

        if (!Files.Any())
        {
            var li = new ListItem();
            li.Text = li.Value = NoTestFiles;
            ddlTestUserFiles1.Items.Add(li);
            //ddlTestUserFiles2.Items.Add(li);
            return;
        }
        {
            var li = new ListItem();
            li.Text = li.Value = SelectFile;
            ddlTestUserFiles1.Items.Add(li);
            //ddlTestUserFiles2.Items.Add(li);
        }
        foreach (var file in Files)
        {
            var li = new ListItem();
            li.Text = li.Value = file.Name;
            ddlTestUserFiles1.Items.Add(li);
            //ddlTestUserFiles2.Items.Add(li);
        }
    }

    protected void UseSelectedFile1_Click(object sender, EventArgs e)
    {
        AddToLogAndDisplay("UseSelectedFile");
        var cid = GetCidUsingName();
        lbError.Text = "";
        if (ddlTestUserFiles1.SelectedValue == SelectFile)
        {
            lbError.Text = @"Select a file.";
            return;
        }

        var filePath = Path.Combine(HttpContext.Current.Server.MapPath("../" + TestLib.TestConstants.UserTestFiles), ddlTestUserFiles1.SelectedValue);
        AddToLogAndDisplay(filePath);
        // Read the file and display it line by line.
        var mFile = new StreamReader(filePath);
        string line;
        while ((line = mFile.ReadLine()) != null)
        {
            AddToLogAndDisplay(line.Replace(dk.ItemSep, ", "));
            if (String.IsNullOrWhiteSpace(line))
                continue;
            if (line.StartsWith(tc.MACBank) == false)
                continue;

            var request = dk.Request + dk.KVSep + tc.AssignAccount +
                dk.ItemSep + dk.CID + dk.KVSep + cid +
                dk.ItemSep + "Default" + dk.KVSep + "True" +
                line.Replace(tc.MACBank, "");

            AddToLogAndDisplay(request.Replace(dk.ItemSep, ", "));

            var rply = SendRequestToMacTestBankServer(cid, request);
            if (rply.Item1)
            {
                var xmlDoc = new XmlDocument();
                if (rply.Item2 != null) xmlDoc.LoadXml(rply.Item2);
                var elemList = xmlDoc.GetElementsByTagName("macResponse");
                if (elemList.Count != 0)
                    AddToLogAndDisplay(elemList[0].InnerXml);
            }
            else
            {
                AddToLogAndDisplay(rply.Item2);
                lbError.Text = "";
            }
        }
        mFile.Close();
        SendRequestToMacTestBankServer(cid, 
            dk.Request + dk.KVSep + tc.GetBankStatus +
            dk.ItemSep + dk.CID + dk.KVSep + cid);
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