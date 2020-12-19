using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;
using System.Web;

using MACServices;
using sr = MACServices.Constants.ServiceResponse;
using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using cs = MACServices.Constants.Strings;

using tc = TestLib.TestConstants.TestBank;

public partial class MACUserApps_Web_Tests_MACTestBank_BillPay : System.Web.UI.Page
{
    private const string Test = "MTB_Bill";
    private const string MTB = "MAC Test Bank";
    private const string SelectUser = "Select Account Holder";
    private const string SelectMerchant = "Select Merchant";
    private const string SelectUtility = "Select Utility";
    private const string SelectAccount = "Select Account or Card Name";
    private const string SelectNumber = "Account Number";
    private const string NoBills = "No Bills Due";
    private const string SelectABill = "Select a Bill";
    //private const string EnterCardNumber = "Enter Card Number";

    HiddenField _hiddenW;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Master != null)
        {
            _hiddenW = (HiddenField)Master.FindControl("hiddenW");
            _hiddenW.Value = "54a83ab1ead6362034d04bc6";
        }

        if (IsPostBack) return;
        var cid = GetCidUsingName();
        SendRequestToMacTestBankServer(cid, 
            dk.Request + dk.KVSep + tc.GetBankStatus +
            dk.ItemSep + dk.CID + dk.KVSep + cid);
        fillddlNames();
        fillddlAccountNames();
        btnPayBill.Visible = false;
        btnDisabledPayBill.Visible = true;
    }

    #region Purchasing Button events

    protected void btnPreauth_Click(object sender, EventArgs e)
    {
        AddToLogAndDisplay("btnPreauth");
        lbError.Text = "";
        var user = ddlUserNames4.SelectedItem.Text;
        if (String.IsNullOrEmpty(user) || user == SelectUser)
        {
            lbError.Text = @"Select a user";
            return;
        }
        var accountname = ddlAccountNames4.SelectedItem.Text;
        var accountnumber = ddlAccountNumbers4.SelectedItem.Text;
        if (String.IsNullOrEmpty(accountname) || accountname == SelectAccount)
        {
            if (String.IsNullOrEmpty(accountnumber) || accountnumber == SelectNumber)
            {
                lbError.Text = @"Select an account or account number";
                return;
            }
        }
        double mAmountToPay;
        if (double.TryParse(txtPreauthAmount.Text, out mAmountToPay) == false)
        {
            lbError.Text = @"Invalid amount!";
            return;
        }
        if (mAmountToPay < 1)
        {
            lbError.Text = @"Invalid amount!";
            return;
        }
        var cid = GetCidUsingName();
        var request = dk.Request + dk.KVSep + tc.Preauth +
            dk.ItemSep + dk.CID + dk.KVSep + cid +  
            dk.ItemSep + tc.AccountHolder + dk.KVSep + user +            
            dk.ItemSep + tc.Amount + dk.KVSep + txtPreauthAmount.Text;
        if (String.IsNullOrEmpty(accountname) || accountname == SelectAccount)
            request += dk.ItemSep + tc.AccountNo + dk.KVSep + accountnumber;
        else
            request += dk.ItemSep + tc.AccountName + dk.KVSep + accountname;

        var rply = SendRequestToMacTestBankServer(cid, request);
        if (rply.Item1 == false) return;
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(rply.Item2);
        var elemList = xmlDoc.GetElementsByTagName("macResponse");
        AddToLogAndDisplay(elemList[0].InnerXml);
    }

    protected void btnDefaultPurchases_Click(object sender, EventArgs e)
    {
        AddToLogAndDisplay("btnDefaultPurchases");
        lbError.Text = @"Not implemented";

    }

    protected void btnMakeAPurchase_Click(object sender, EventArgs e)
    {
        AddToLogAndDisplay("btnMakeAPurchase");
        lbError.Text = "";
        var user = ddlUserNames3.SelectedItem.Text;
        if (String.IsNullOrEmpty(user) || user == SelectUser)
        {
            lbError.Text = @"Select a user";
            return;
        }
        var merchant = ddlMerchants.SelectedItem.Text;
        if (String.IsNullOrEmpty(merchant) || merchant == SelectMerchant)
        {
            lbError.Text = @"Select a merchant";
            return;
        }
        var cardnumber = txtCardNumber.Text;
        if (String.IsNullOrEmpty(txtCardNumber.Text))
        {
            lbError.Text = @"Select a card(sub-account) OR enter the card number";
            return;
        }
        double mAmountToPay;
        if (double.TryParse(txtPurchaseAmount.Text, out mAmountToPay) == false)
        {
            lbError.Text = @"Invalid purchase amount!";
            return;
        }
        if (mAmountToPay < 1)
        {
            lbError.Text = @"Invalid purchase amount!";
            return;
        }
        MakePurchase(user, cardnumber, merchant, txtPurchaseAmount.Text);
    }

    private void MakePurchase(string pUser, string pCardNumber, string pMerchant, string pAmount)
    {
        lbError.Text = "";
        AddToLogAndDisplay(tc.User + ":" + pUser + ", " + tc.Merchant + ":" + pMerchant + ", " + tc.CardNumber + ":" + pCardNumber + ", " + tc.Amount + ":" + pAmount);
        var cid = GetCidUsingName();
        var rply = SendRequestToMacTestBankServer(cid,
            dk.Request + dk.KVSep + tc.Purchase +
            dk.ItemSep + dk.CID + dk.KVSep + cid +
            dk.ItemSep + tc.MerchantName + dk.KVSep + pMerchant +
            dk.ItemSep + tc.AccountHolder + dk.KVSep + pUser +
            dk.ItemSep + tc.CardNumber + dk.KVSep + pCardNumber +
            dk.ItemSep + tc.Amount + dk.KVSep + pAmount);
        if (rply.Item1 == false) return;
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(rply.Item2);
        var elemList = xmlDoc.GetElementsByTagName("macResponse");
        AddToLogAndDisplay(elemList[0].InnerXml);
    }

    #endregion

    #region Purchase DDL events
    protected void ddlUserNames3_Selected(object sender, EventArgs e)
    {
        ddlAccountNames3.Items.Clear();
        {
            var li = new ListItem { Text = SelectAccount, Value = SelectAccount };
            ddlAccountNames3.Items.Add(li);
        }
        if (ddlUserNames3.SelectedValue == SelectUser) return;

        var mAccs = GetAccountNamesNumbersByAccountHolderName(ddlUserNames3.SelectedValue);
        if (String.IsNullOrEmpty(mAccs))
        {
            lbError.Text = @"Error: could not get Account Names and numbers by account holder name";
            return;
        }
        var mAccNamesNos = mAccs.Split(Char.Parse(dk.ItemSep));

        foreach (var mAccNameNo in mAccNamesNos)
        {
            var mNN = mAccNameNo.Split(char.Parse(dk.KVSep));
            var li = new ListItem
            {
                Text = mNN[0],   // account name
                Value = mNN[1]   // account number
            };
            ddlAccountNames3.Items.Add(li);
        }
    }

    protected void ddlUserNames4_Selected(object sender, EventArgs e)
    {
        ddlAccountNames4.Items.Clear();
        {
            var li = new ListItem { Text = SelectAccount, Value = SelectAccount };
            ddlAccountNames4.Items.Add(li);
        }
        ddlAccountNumbers4.Items.Clear();
        {
            var li = new ListItem { Text = SelectNumber, Value = SelectNumber };
            ddlAccountNumbers4.Items.Add(li);
        }
        if (ddlUserNames4.SelectedValue == SelectUser) return;

        var mAccs = GetAccountNamesNumbersByAccountHolderName(ddlUserNames4.SelectedValue);
        if (String.IsNullOrEmpty(mAccs))
        {
            lbError.Text = @"Error: could not get Account Names and numbers by account holder name";
            return;
        }
        var mAccNamesNos = mAccs.Split(Char.Parse(dk.ItemSep));

        foreach (var mAccNameNo in mAccNamesNos)
        {
            var mNN = mAccNameNo.Split(char.Parse(dk.KVSep));
            var liName = new ListItem
            {
                Text = mNN[0],   // account name
                Value = mNN[0]
            }; 
            ddlAccountNames4.Items.Add(liName);
            var liNo = new ListItem
            {
                Text = mNN[1],   // account number
                Value = mNN[1]
            };
            ddlAccountNumbers4.Items.Add(liNo);
        }
    }

    protected void ddlAccountNames3_Selected(object sender, EventArgs e)
    {
        if (ddlAccountNames3.SelectedItem.Text == SelectAccount)
            txtCardNumber.Text = "";
        else
            txtCardNumber.Text = ddlAccountNames3.SelectedValue; // account number
    }

    protected string GetAccountNamesNumbersByAccountHolderName(string pAccountHolderName)
    {
        var cid = GetCidUsingName();
        var rply = SendRequestToMacTestBankServer(cid,
            dk.Request + dk.KVSep + "GetAccountNamesNumbers" +
            dk.ItemSep + dk.CID + dk.KVSep + cid +
            dk.ItemSep + "AccountHolder" + dk.KVSep + pAccountHolderName);
        if (rply.Item1 == false) return null;
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(rply.Item2);
        var elemList = xmlDoc.GetElementsByTagName("Details");
        if (elemList.Count < 1) return "";
        AddToLogAndDisplay(elemList[0].InnerXml.Replace(dk.ItemSep, "' "));
        return elemList[0].InnerXml;
    }
    #endregion

    #region Billing Button Events and methods
    protected void btnDefaultBilling_Click(object sender, EventArgs e)
    {
        AddToLogAndDisplay("btnDefaultBilling");
        lbError.Text = "";
        var mAccNames = hiddenAccountHoldersList.Value.Split(char.Parse(dk.ItemSep));
        var mUsers = new List<string>();
        var mUtilities = new List<string>();
        var random = new Random();
        foreach (var mAccName in mAccNames)
        {
            var mNameType = mAccName.Split(char.Parse(dk.KVSep));
            if (mNameType[1] == tc.User)
                mUsers.Add(mNameType[0]);
            else
                if (mNameType[1] == tc.Utility)
                mUtilities.Add(mNameType[0]);
        }
        foreach (var user in mUsers)
        {
            foreach (var mUtility in mUtilities)
            {
                var amount = random.NextDouble() * (525.00 - 150.00) + 150.00;
                CreateABill(user, mUtility, amount.ToString());
            }
        }
    }

    protected void btnGetBills_Click(object sender, EventArgs e)
    {
        AddToLogAndDisplay("btnGetBills");
        lbError.Text = "";
        var user = ddlUserNames2.SelectedItem.Text;
        if (String.IsNullOrEmpty(user) || user == SelectUser)
        {
            lbError.Text = @"You must select a user";
            return;
        }

        // Get accounts for user
        var mAccs = GetAccountNamesNumbersByAccountHolderName(ddlUserNames2.SelectedValue);
        if (String.IsNullOrEmpty(mAccs))
        {
            lbError.Text = @"Error: could not get Account Names and numbers by account holder name";
            return;
        }
        var mAccNamesNos = mAccs.Split(Char.Parse(dk.ItemSep));

        foreach (var mAccNameNo in mAccNamesNos)
        {
            var mNN = mAccNameNo.Split(char.Parse(dk.KVSep));
            var li = new ListItem
            {
                Text = mNN[0],   // account name
                Value = mNN[1]   // account number
            };
            ddlAccountNames5.Items.Add(li);
        }

        var cid = GetCidUsingName();

        // Get bills
        var rply = SendRequestToMacTestBankServer(cid,
            dk.Request + dk.KVSep + tc.GetBills +
            dk.ItemSep + dk.CID + dk.KVSep + cid +
            dk.ItemSep + tc.AccountHolder + dk.KVSep + ddlUserNames2.SelectedItem.Text);
        if (rply.Item1 == false) return;
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(rply.Item2);
        var elemList = xmlDoc.GetElementsByTagName("macResponse");
        AddToLogAndDisplay(elemList[0].InnerXml.Replace("<Bill>", "||<Bill>"));
        elemList = xmlDoc.GetElementsByTagName(tc.Bills);
        if (elemList.Count == 0)
            hiddenBills.Value = "";
        else
            hiddenBills.Value = elemList[0].InnerXml;
        fillBillList();
        btnPayBill.Visible = true;
        btnDisabledPayBill.Visible = false;
    }
    
    protected void btnDisabledPayBill_Click(object sender, EventArgs e)
    {
        lbError.Text = @"You must get the list of bills to pay";
    }

    protected void btnPayBill_Click(object sender, EventArgs e)
    {
        AddToLogAndDisplay("btnPayBill");
        lbError.Text = "";
        if (ddlAccountNames5.SelectedValue == SelectAccount)
        {
            lbError.Text = @"Select an account by name!";
            return;
        }
        if (ddlBillList.SelectedValue == NoBills)
        {
            lbError.Text = @"No bills to pay!";
            return;
        }
        if (ddlBillList.SelectedValue == SelectABill)
        {
            lbError.Text = SelectABill;
            return;
        }
        if (txtAmountToPay.Text.ToLower().Trim() != tc.Full.ToLower())
        {
            double mAmountToPay;
            if (double.TryParse(txtAmountToPay.Text.Replace("$", "").Replace(",", "").Replace(".", ""), out mAmountToPay) == false)
            {
                lbError.Text = @"Invalid amount to pay!";
                return;
            }
            if (mAmountToPay < 1)
            {
                lbError.Text = @"Invalid amount to pay!";
                return;
            }
        }
        else
        {
            txtAmountToPay.Text = tc.Full;
        }
        var cid = GetCidUsingName();
        var rply = SendRequestToMacTestBankServer(cid,
            dk.Request + dk.KVSep + tc.PayBill +
            dk.ItemSep + dk.CID + dk.KVSep + cid +
            dk.ItemSep + tc.InvoiceNumber + dk.KVSep + ddlBillList.SelectedValue +
            dk.ItemSep + tc.AccountHolder + dk.KVSep + ddlUserNames2.SelectedValue +
            dk.ItemSep + tc.AccountName + dk.KVSep + ddlAccountNames5.SelectedItem.Text +
            dk.ItemSep + tc.AccountNo + dk.KVSep + ddlAccountNames5.SelectedValue +
            dk.ItemSep + tc.Amount + dk.KVSep + txtAmountToPay.Text);
        if (rply.Item1 == false) return;
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(rply.Item2);
        var elemList = xmlDoc.GetElementsByTagName("macResponse");
        AddToLogAndDisplay(elemList[0].InnerXml);
    }

    protected void btnCreateBill_Click(object sender, EventArgs e)
    {
        AddToLogAndDisplay("btnCreateBill");
        lbError.Text = "";
        var user = ddlUserNames1.SelectedItem.Text;
        if (String.IsNullOrEmpty(user) || user == SelectUser)
        {
            lbError.Text = @"You must selet someone to bill!";
            return;
        }
        var from = ddlUtilities1.SelectedItem.Text;
        if (String.IsNullOrEmpty(from) || from == SelectUtility)
        {
            lbError.Text = @"You must selet a utility the bill is from!";
            return;
        }
        if (String.IsNullOrEmpty(txtAmount.Text))
        {
            lbError.Text = @"You must enter the acount to bill!";
            return;
        }
        try
        {
            var amount = Convert.ToDouble(txtAmount.Text);
            if (Math.Abs(amount) < 1)
            {
                lbError.Text = @"Invalid amount!";
                return;
            }
        }
        catch (FormatException)
        {
            lbError.Text = @"Invalid amount!";
            return;
        }
        CreateABill(
            ddlUserNames1.SelectedItem.Text,
            ddlUtilities1.SelectedItem.Text,
            txtAmount.Text);
    }
    #endregion

    #region Billing ddl events and methods
    protected void ddlUserNames1_Selected(object sender, EventArgs e)
    {
        ddlAccountNames4.Items.Clear();
        {
            var li = new ListItem { Text = SelectAccount, Value = SelectAccount };
            ddlAccountNames4.Items.Add(li);
        }
        ddlAccountNumbers4.Items.Clear();
        {
            var li = new ListItem { Text = SelectNumber, Value = SelectNumber };
            ddlAccountNumbers4.Items.Add(li);
        }
        if (ddlUserNames4.SelectedValue == SelectUser) return;

        var mAccs = GetAccountNamesNumbersByAccountHolderName(ddlUserNames4.SelectedValue);
        if (String.IsNullOrEmpty(mAccs))
        {
            lbError.Text = @"Error: could not get Account Names and numbers by account holder name";
            return;
        }
        var mAccNamesNos = mAccs.Split(Char.Parse(dk.ItemSep));

        foreach (var mAccNameNo in mAccNamesNos)
        {
            var mNN = mAccNameNo.Split(char.Parse(dk.KVSep));
            var liName = new ListItem
            {
                Text = mNN[0],   // account name
                Value = mNN[0]
            };
            ddlAccountNames4.Items.Add(liName);
            var liNo = new ListItem
            {
                Text = mNN[1],   // account number
                Value = mNN[1]
            };
            ddlAccountNumbers4.Items.Add(liNo);
        }
    }

    private void CreateABill(string pUser, string pMerchant, string pAmount)
    {
        AddToLogAndDisplay(tc.User + dk.KVSep + pUser + ", " + tc.Utility + dk.KVSep + pMerchant + ", " + tc.Amount + dk.KVSep + pAmount);
        var cid = GetCidUsingName();
        var rply = SendRequestToMacTestBankServer(cid,
            dk.Request + dk.KVSep + tc.AddBill +
            dk.ItemSep + dk.CID + dk.KVSep + cid +
            dk.ItemSep + tc.MerchantName + dk.KVSep + pMerchant +
            dk.ItemSep + tc.AccountHolder + dk.KVSep + pUser +
            dk.ItemSep + tc.Amount + dk.KVSep + pAmount);
        if (rply.Item1 == false) return;
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(rply.Item2);
        var elemList = xmlDoc.GetElementsByTagName("macResponse");
        AddToLogAndDisplay(elemList[0].InnerXml
            .Replace(dk.ItemSep, ", ")
            .Replace("<" + sr.Details + ">", dk.ItemSep + "  <" + sr.Details + ">"));
    }
    #endregion

    #region Billing Fill methods
    private void fillBillList()
    {
        ddlBillList.Items.Clear();

        if (hiddenBills.Value.Contains("None"))
        {
            var li = new ListItem { Text = NoBills, Value = NoBills };
            ddlBillList.Items.Add(li);
        }
        {
            var li = new ListItem { Text = SelectABill, Value = SelectABill };
            ddlBillList.Items.Add(li);
        }
        var bills = (hiddenBills.Value.Replace("<Bill>", dk.ItemSep)).Split(char.Parse(dk.ItemSep));
        foreach (var bill in bills)
        {
            if (string.IsNullOrEmpty(bill)) continue;
            var invoiceno = "";
            var invoicests = "";
            var invoicebis = "";
            var invoiceamt = "";
            var invoice = bill
                .Replace("</Bill>", "")
                .Replace("</InvoiceNumber", "")
                .Replace("</Status", "")
                .Replace("</BusinessType", "")
                .Replace("</Name", "")
                .Replace("</BillingDate", "")
                .Replace("</DueDate", "")
                .Replace("><", dk.ItemSep).Replace(">", "").Replace("<", "").Replace("/", "");
            var invoicedetails = invoice.Split(char.Parse(dk.ItemSep));
            foreach (var invoicedetail in invoicedetails)
            {
                if (invoicedetail.StartsWith("InvoiceNumber"))
                    invoiceno = invoicedetail.Replace("InvoiceNumber", "").Trim();
                if (invoicedetail.StartsWith("Status"))
                    invoicests = invoicedetail.Replace("Status", "").Trim();
                if (invoicedetail.StartsWith("Name"))
                    invoicebis = invoicedetail.Replace("Name", "").Trim();
                if (invoicedetail.StartsWith("AmountDue"))
                {
                    invoiceamt = invoicedetail.Replace("AmountDue", "").Trim();
                }
            }
            if (invoicests == "Due")
            {
                var li = new ListItem { Text = invoiceno + dk.KVSep + @" " + invoicebis + @", " + invoiceamt, Value = invoiceno };
                ddlBillList.Items.Add(li);
            }
        }
    }
 
    private void fillddlAccountNames()
    {
        //ddlAccountNames.Items.Clear();
        ddlAccountNames3.Items.Clear();
        ddlAccountNames4.Items.Clear();
        ddlAccountNames5.Items.Clear();
        {
            var li = new ListItem { Text = SelectAccount, Value = SelectAccount };
            //ddlAccountNames.Items.Add(li);
            ddlAccountNames3.Items.Add(li);
            ddlAccountNames4.Items.Add(li);
            ddlAccountNames5.Items.Add(li);
        }
    }

    protected void fillddlNames()
    {
        ddlUserNames1.Items.Clear();
        ddlUserNames2.Items.Clear();
        ddlUserNames3.Items.Clear();
        ddlUserNames4.Items.Clear();
        ddlMerchants.Items.Clear();
        ddlUtilities1.Items.Clear();
        {
            var li = new ListItem { Text = SelectUser, Value = SelectUser };
            ddlUserNames1.Items.Add(li);
            ddlUserNames2.Items.Add(li);
            ddlUserNames3.Items.Add(li);
            ddlUserNames4.Items.Add(li);
        }
        ddlMerchants.Items.Clear();
        {
            var li = new ListItem { Text = SelectMerchant, Value = SelectMerchant };
            ddlMerchants.Items.Add(li);
        }
        ddlUtilities1.Items.Clear();
        {
            var li = new ListItem { Text = SelectUtility, Value = SelectUtility };
            ddlUtilities1.Items.Add(li);
        }
        if (String.IsNullOrEmpty(hiddenAccountHoldersList.Value))
        {
            AddToLogAndDisplay("Account Holders List empty");
            return;
        }
        var AccountHolderNames = hiddenAccountHoldersList.Value.Split(char.Parse(dk.ItemSep));
        foreach (var AccountHolderName in AccountHolderNames)
        {
            var nametype = AccountHolderName.Split(char.Parse(dk.KVSep));
            var li = new ListItem { Text = nametype[0], Value = nametype[0] };
            if (nametype[1] == tc.User)
            {
                ddlUserNames1.Items.Add(li);
                ddlUserNames2.Items.Add(li);
                ddlUserNames3.Items.Add(li);
                ddlUserNames4.Items.Add(li);
            }
            else if (nametype[1] == tc.Merchant)
            {
                ddlMerchants.Items.Add(li);
            }
            else
            {
                ddlUtilities1.Items.Add(li);
            }
        }
    }
    #endregion

    #region Common
    protected string GetCidUsingName()
    {
        var mUtils = new Utils();
        var mClient = mUtils.GetClientUsingClientName(MTB);
        if (mClient == null)
            return cs.DefaultClientId;
        return mClient.ClientId.ToString();
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
        tbLog.Text = newlog.Trim('|').Replace("|", Environment.NewLine);
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

    private Tuple<bool, string> SendRequestToMacTestBankServer(string pClientId, string requestData)
    {
        try
        {
            var dataStream = Encoding.UTF8.GetBytes("data=99" + pClientId.Length + pClientId.ToUpper() + StringToHex(requestData));
            var request = ConfigurationManager.AppSettings[cfg.MacServicesUrl] + TestLib.TestConstants.MacTestBankUrl;
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


            elemList = xmlDoc.GetElementsByTagName("AccountNamesList");
            if (elemList.Count != 0)
            {
                hiddenAccountNamesList.Value = elemList[0].InnerXml;
                fillddlAccountNames();
            }
            return new Tuple<bool, string>(true, xmlDoc.OuterXml);
        }
        catch (Exception ex)
        {
            return new Tuple<bool, string>(false, "Error: " + ex.Message);
        }
    }
    #endregion
}