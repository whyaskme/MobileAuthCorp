using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;

using MACServices;
using sr = MACServices.Constants.ServiceResponse;
using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using cs = MACServices.Constants.Strings;

using tc = TestLib.TestConstants.TestBank;

namespace MACUserApps.Web.Tests.MACTestBank
{
    public partial class MacUserAppsWebTestsMacTestBankMacTestBank : System.Web.UI.Page
    {
        private const string Test = "MTB";
        private const string MTB = "MAC Test Bank";
        public static string MacServicesUrl = ConfigurationManager.AppSettings[cfg.MacServicesUrl];
        private const string SelectPan = "Select PAN";
        private const string SelectHolder = "Select Account Holder";
        private const string SelectAccountName = "Select Account Name";
        private const string SelectAccountNumber = "Select Account Number";
        private const string SelectLogin = "Select Login";

        HiddenField _hiddenW;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Master != null)
            {
                _hiddenW = (HiddenField)Master.FindControl("hiddenW");
                _hiddenW.Value = "54a83a27ead6362034d04bc0";
            }

            if (IsPostBack) return;
            var cid = GetCidUsingName();
            SendRequestToMacTestBankServer(cid, 
                dk.Request + dk.KVSep + tc.GetBankStatus +
                dk.ItemSep + dk.CID + dk.KVSep + cid);
            fillddlPanList();
            fillddlAccountHolderNames();
            fillddlAccountNames();
            fillddlLoginNames();
        }

        #region Button Events

        protected void btnGetAccountBalance_Click(object sender, EventArgs e)
        {
            AddToLogAndDisplay("btnGetAccountBalance");
            lbError.Text = "";
            var cid = GetCidUsingName();
            var mRequest = dk.Request + dk.KVSep + "GetAccountBalance";
            if (ddlAccountHolderNames3.SelectedItem.Text != SelectHolder)
            {
                mRequest += dk.ItemSep + "AccountHolder" + dk.KVSep + ddlAccountHolderNames3.SelectedValue;
            }
            else if (ddlPanList3.SelectedItem.Text != SelectPan)
            {
                mRequest += dk.ItemSep + "AccountNo" + dk.KVSep + ddlPanList3.SelectedValue;
            }
            else
            {
                lbError.Text = @"Select an account holder or PAN!";
                return;
            }
            if (ddlAccountNames3.SelectedItem.Text == SelectAccountName)
            {
                lbError.Text = @"Select an account name!";
                return;
            }
            mRequest += dk.ItemSep + "AccountName" + dk.KVSep + ddlAccountNames3.SelectedValue +
                        dk.ItemSep + dk.CID + dk.KVSep + cid;

            var rply = SendRequestToMacTestBankServer(cid, mRequest);
            if (rply.Item1 == false) return;
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(rply.Item2);
            var elemList = xmlDoc.GetElementsByTagName("macResponse");
            AddToLogAndDisplay(elemList[0].InnerXml.Replace(char.Parse(dk.ItemSep), ' '));
        }

        protected void btnDeleteAccount_Click(object sender, EventArgs e)
        {
            AddToLogAndDisplay("btnDeleteAccount");
            lbError.Text = "";
            var cid = GetCidUsingName();
            var mRequest = dk.Request + dk.KVSep + tc.DeleteUserAccount;
            if (ddlAccountHolderNames1.SelectedItem.Text != SelectHolder)
            {
                mRequest += dk.ItemSep + tc.AccountHolder + dk.KVSep + ddlAccountHolderNames1.SelectedValue;
            }
            else if (ddlPanList1.SelectedItem.Text != SelectPan)
            {
                mRequest += dk.ItemSep + tc.AccountNo + dk.KVSep + ddlPanList1.SelectedValue;
            }
            else
            {
                lbError.Text = @"You must select an account holder or PAN!";
                return;
            }
            mRequest += dk.ItemSep + dk.CID + dk.KVSep + cid;

            var rply = SendRequestToMacTestBankServer(cid, mRequest);
            if (rply.Item1 == false) return;
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(rply.Item2);
            var elemList = xmlDoc.GetElementsByTagName("macResponse");
            ParseAndDisplay(elemList[0].InnerXml);
        }

        protected void btnAccountDetails_Click(object sender, EventArgs e)
        {
            AddToLogAndDisplay("btnAccountDetails");
            lbError.Text = "";
            var cid = GetCidUsingName();
            var mRequest = dk.Request + dk.KVSep + tc.GetAccountDetails;
            if (ddlAccountHolderNames1.SelectedItem.Text != SelectHolder)
            {
                mRequest += dk.ItemSep + tc.AccountHolder + dk.KVSep + ddlAccountHolderNames1.SelectedValue;
            }
            else if (ddlPanList1.SelectedItem.Text != SelectPan)
            {
                mRequest += dk.ItemSep + tc.AccountNo + dk.KVSep + ddlPanList1.SelectedValue;
            }
            else
            {
                lbError.Text = @"You must select an account holder or PAN!";
                return;
            }
            mRequest += dk.ItemSep + dk.CID + dk.KVSep + cid;

            var rply = SendRequestToMacTestBankServer(cid, mRequest); 
            if (rply.Item1 == false) return;
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(rply.Item2);
            var elemList = xmlDoc.GetElementsByTagName("macResponse");
            ParseAndDisplay(elemList[0].InnerXml);
        }
        
        protected void btnAccountLog_Click(object sender, EventArgs e)
        {
            AddToLogAndDisplay("btnAccountLog");
            lbError.Text = "";
            var ddlAccountHolderNames1Index = ddlAccountHolderNames1.SelectedIndex;
            var ddlPanList1Index = ddlPanList1.SelectedIndex;
            var cid = GetCidUsingName();
            var mRequest = dk.Request + dk.KVSep + "GetAccountLog" +
                            dk.ItemSep + dk.CID + dk.KVSep + cid;
            if (ddlAccountHolderNames1.SelectedItem.Text != SelectHolder)
            {
                mRequest += dk.ItemSep + "AccountHolder" + dk.KVSep + ddlAccountHolderNames1.SelectedValue;
            }
            else if (ddlPanList1.SelectedItem.Text != SelectPan)
            {
                mRequest += dk.ItemSep + "AccountNo" + dk.KVSep + ddlPanList1.SelectedValue;
            }
            else 
            {
                lbError.Text = @"You must select an account holder or PAN!";
                return;
            }

            var rply = SendRequestToMacTestBankServer(cid, mRequest);
            if (rply.Item1 == false) return;
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(rply.Item2);
            var elemList = xmlDoc.GetElementsByTagName("macResponse");
            ParseAndDisplay(elemList[0].InnerXml);
            ddlAccountHolderNames1.SelectedIndex = ddlAccountHolderNames1Index;
            ddlPanList1.SelectedIndex = ddlPanList1Index;
        }
        
        protected void btnAdjustAccount_Click(object sender, EventArgs e)
        {
            AddToLogAndDisplay("btnAdjustAccount");
            lbError.Text = "";
            var button = sender as Button;
            var mRequest = dk.Request + dk.KVSep + "CreditAccount";
            if (button == null) return;
            if (button.Text.ToLower().Contains("debit"))
                mRequest = dk.Request + dk.KVSep + "DebitAccount";

            var mAccountName = ddlAccountNames.SelectedValue;
            if (mAccountName == SelectAccountName)
            {
                lbError.Text = @"You must select an account name!";
                return;
            }
            AddToLogAndDisplay(button.Text + "Adjust Account");

            lbError.Text = "";
            if (ddlAccountHolderNames2.SelectedItem.Text != SelectHolder)
            {
                mRequest += dk.ItemSep + "AccountHolder" + dk.KVSep + ddlAccountHolderNames2.SelectedValue;
            }
            else if (ddlPanList2.SelectedItem.Text != SelectPan)
            {
                mRequest += dk.ItemSep + "AccountNo" + dk.KVSep + ddlPanList2.SelectedValue;
            }
            else
            {
                lbError.Text = @"You must select an account holder or PAN!";
                return;
            }
            var cid = GetCidUsingName();
            const int amount = 1500;
            mRequest += dk.ItemSep + "AccountName" + dk.KVSep + mAccountName +
                        dk.ItemSep + "Amount" + dk.KVSep + amount +
                        dk.ItemSep + dk.CID + dk.KVSep + cid;

            var rply = SendRequestToMacTestBankServer(cid, mRequest);
            if (rply.Item1 == false) return;
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(rply.Item2);
            var elemList = xmlDoc.GetElementsByTagName("macResponse");
            AddToLogAndDisplay(elemList[0].InnerXml);
        }

        protected void btnMoveFunds_Click(object sender, EventArgs e)
        {
            AddToLogAndDisplay("btnMoveFunds");
            lbError.Text = "";
            var request = dk.Request + dk.KVSep + "MoveFunds";

            // from account
            if (ddlFromAccountHolders.SelectedItem.Text == SelectHolder)
            {
                lbError.Text = @"Select a 'From' account holder";
                return;
            }
            if (ddlFromAccountNo.SelectedItem.Text != SelectAccountNumber)
            {
                request += dk.ItemSep + "FromAccountNo" + dk.KVSep + ddlFromAccountNo.SelectedValue;
            }
            else if (ddlFromAccountNames.SelectedItem.Text != SelectAccountName)
            {
                request += dk.ItemSep + "FromAccountHolder" + dk.KVSep + ddlFromAccountHolders.SelectedValue +
                            dk.ItemSep + "FromAccountName" + dk.KVSep + ddlFromAccountNames.SelectedValue;
            }
            else
            {
                lbError.Text = @"Select a 'From' account name or number";
                return;
            }
            // to account
            if (ddlToAccountHolders.SelectedItem.Text == SelectHolder)
            {
                lbError.Text = @"Select a 'To' account holder";
                return;
            }
            if (ddlToAccountNo.SelectedItem.Text != SelectAccountNumber)
            {
                request += dk.ItemSep + "ToAccountNo" + dk.KVSep + ddlToAccountNo.SelectedValue;
            }
            else if (ddlToAccountNames.SelectedItem.Text != SelectAccountName)
            {
                request += dk.ItemSep + "ToAccountHolder" + dk.KVSep + ddlToAccountHolders.SelectedValue +
                            dk.ItemSep + "ToAccountName" + dk.KVSep + ddlToAccountNames.SelectedValue;
            }
            else
            {
                lbError.Text = @"Select a 'to' account name or number";
                return;
            }
            // Amount
            if (String.IsNullOrEmpty(txtAmountToMove.Text))
            {
                lbError.Text = @"You must enter the amount";
                txtAmountToMove.Text = "";
                return;
            }
            double amount = 0;
            try
            {
                amount = double.Parse(txtAmountToMove.Text);
            }
            catch
            {
                lbError.Text = @"Invalid amount: " + amount.ToString();
                txtAmountToMove.Text = "";
                return;
            }
            var cid = GetCidUsingName();
            request += dk.ItemSep + "Amount" + dk.KVSep + txtAmountToMove.Text +
                dk.ItemSep + dk.CID + dk.KVSep + cid;
            AddToLogAndDisplay(request.Replace(dk.ItemSep, ", "));
            var rply = SendRequestToMacTestBankServer(cid, request);
            if (rply.Item1 == false) return;
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(rply.Item2);
            var elemList = xmlDoc.GetElementsByTagName("macResponse");
            AddToLogAndDisplay(elemList[0].InnerXml.Replace(dk.ItemSep, ", "));
        }

        protected void btnValidateLoginName_Click(object sender, EventArgs e)
        {
            AddToLogAndDisplay("btnValidateLoginName");
            lbError.Text = "";
            if (String.IsNullOrEmpty(txtLoginName.Text))
            {
                lbError.Text = @"You must enter a login name!";
                return;
            }
            var cid = GetCidUsingName();
            var rply = SendRequestToMacTestBankServer(cid,
                dk.Request + dk.KVSep + "ValidateLoginName" +
                dk.ItemSep + dk.CID + dk.KVSep + cid +
                dk.ItemSep + "LoginName" + dk.KVSep + txtLoginName.Text);
            if (rply.Item1 == false) return;
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(rply.Item2);
            var elemList = xmlDoc.GetElementsByTagName("macResponse");
            AddToLogAndDisplay(elemList[0].InnerXml.Replace(dk.ItemSep, ", "));
        }

        protected void btnDeleteAccount1_Click(object sender, EventArgs e)
        {
            AddToLogAndDisplay("btnDeleteAccount");
            lbError.Text = "";
            if (String.IsNullOrEmpty(txtLoginName.Text))
            {
                lbError.Text = @"You must enter a login name!";
                return;
            }
            var cid = GetCidUsingName();
            var rply = SendRequestToMacTestBankServer(cid,
                dk.Request + dk.KVSep + tc.DeleteUserAccount +
                dk.ItemSep + dk.CID + dk.KVSep + cid +
                dk.ItemSep + tc.LoginName + dk.KVSep + txtLoginName.Text);
            if (rply.Item1 == false) return;
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(rply.Item2);
            var elemList = xmlDoc.GetElementsByTagName("macResponse");
            AddToLogAndDisplay(elemList[0].InnerXml.Replace(dk.ItemSep, ", "));
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            lbError.Text = "";
            AddToLogAndDisplay("Reset/Delete all accounts clicked");
            var cid = GetCidUsingName();
            var rply = SendRequestToMacTestBankServer(cid, 
                dk.Request + dk.KVSep + "DeleteAllAccounts" +
                dk.ItemSep + dk.CID + dk.KVSep + cid);
            if (rply.Item1 == false) return;
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(rply.Item2);
            var elemList = xmlDoc.GetElementsByTagName("macResponse");
            if (elemList.Count != 0)
            {
                AddToLogAndDisplay(elemList[0].InnerXml);
            }
        }

        protected void btnUnassignAllAccounts_Click(object sender, EventArgs e)
        {
            lbError.Text = "";
            AddToLogAndDisplay("Unassign all accounts clicked");
            var cid = GetCidUsingName();
            var rply = SendRequestToMacTestBankServer(cid, 
                dk.Request + dk.KVSep + "UnassignAllAccounts" +
                dk.ItemSep + dk.CID + dk.KVSep + cid);
            if (rply.Item1 == false) return;
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(rply.Item2);
            var elemList = xmlDoc.GetElementsByTagName("macResponse");
            if (elemList.Count != 0)
            {
                AddToLogAndDisplay(elemList[0].InnerXml);
            }
        }
                
        protected void btnGetClientNameUsingId_Click(object sender, EventArgs e)
        {
            lbError.Text = "";
            AddToLogAndDisplay("btnGetClientNameUsingId_Click");
            var rply = SendRequestToMacTestBankServer(cs.DefaultClientId,
                dk.Request + dk.KVSep + dv.GetClientName +
                dk.ItemSep + dk.CID + dk.KVSep + txtCid.Text);
            if (rply.Item1 == false) return;
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(rply.Item2);
            var elemList = xmlDoc.GetElementsByTagName("macResponse");
            if (elemList.Count != 0)
            {
                elemList = xmlDoc.GetElementsByTagName(sr.Details);
                txtCName.Text = elemList[0].InnerXml;
                AddToLogAndDisplay(txtCName.Text);
            }
        }

        protected void btnGetClientIdUsingName_Click(object sender, EventArgs e)
        {
            lbError.Text = "";
            AddToLogAndDisplay("btnGetClientIdUsingName_Click");
            var rply = SendRequestToMacTestBankServer(cs.DefaultClientId,
                dk.Request + dk.KVSep + dv.GetClientId +
                dk.ItemSep + dk.ClientName + dk.KVSep + txtCName.Text);

            if (rply.Item1 == false) return;
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(rply.Item2);
            var elemList = xmlDoc.GetElementsByTagName("macResponse");
            if (elemList.Count != 0)
            {
                elemList = xmlDoc.GetElementsByTagName(sr.Details);
                txtCid.Text = elemList[0].InnerXml;
                AddToLogAndDisplay(txtCid.Text);
            }
        }

        protected void btnClearLog_Click(object sender, EventArgs e)
        {
            Session["LogText"] = "";
            AddToLogAndDisplay("btnClearLog");
            lbError.Text = "";
        }

        #endregion

        #region Drop Down List events and methods

        protected void ddlLoginNames_Changed(object sender, EventArgs e)
        {
            if (ddlLoginNames.SelectedValue == SelectLogin)
            {
                txtLoginName.Text = "";
                return;
            }
            txtLoginName.Text = ddlLoginNames.SelectedValue;
        }

        protected void ddlToAccountHolders_Changed(object sender, EventArgs e)
        {
            {
                ddlToAccountNames.Items.Clear();
                var li = new ListItem();
                li.Text = li.Value = SelectAccountName;
                ddlToAccountNames.Items.Add(li);
            }
            {
                ddlToAccountNo.Items.Clear();
                var li = new ListItem();
                li.Text = li.Value = SelectAccountNumber;
                ddlToAccountNo.Items.Add(li);
            }
            if (ddlToAccountHolders.SelectedValue == SelectHolder) return;

            // get account names and numbers for to account holder
            var mAccs = GetAccountNamesNumbersByAccountHolderName(ddlToAccountHolders.SelectedValue);
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
                ddlToAccountNames.Items.Add(liName);
                var liNo = new ListItem
                {
                    Text = mNN[1],   // account number
                    Value = mNN[1]
                };
                ddlToAccountNo.Items.Add(liNo);
            }
        }

        protected void ddlFromAccountHolders_Changed(object sender, EventArgs e)
        {
            {
                ddlFromAccountNames.Items.Clear();
                var li = new ListItem();
                li.Text = li.Value = SelectAccountName;
                ddlFromAccountNames.Items.Add(li);
            }
            {
                ddlFromAccountNo.Items.Clear();
                var li = new ListItem();
                li.Text = li.Value = SelectAccountNumber;
                ddlFromAccountNo.Items.Add(li);
            }
            if (ddlFromAccountHolders.SelectedValue == SelectHolder) return;

            // get account names and numbers for to account holder
            var mAccs = GetAccountNamesNumbersByAccountHolderName(ddlFromAccountHolders.SelectedValue);
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
                ddlFromAccountNames.Items.Add(liName);
                var liNo = new ListItem
                {
                    Text = mNN[1],   // account number
                    Value = mNN[1]
                };
                ddlFromAccountNo.Items.Add(liNo);
            }
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

        #region Fill Drop Down List methods
        private void fillddlPanList()
        {
            ddlPanList1.Items.Clear();
            ddlPanList2.Items.Clear();
            ddlPanList3.Items.Clear();
            {
                var li = new ListItem { Text = SelectPan, Value = SelectPan };
                ddlPanList1.Items.Add(li);
                ddlPanList2.Items.Add(li);
                ddlPanList3.Items.Add(li);
            }
            if (String.IsNullOrEmpty(hiddenPANList.Value))
            {
                AddToLogAndDisplay("PAN List empty");
                return;
            }
            var mPans = hiddenPANList.Value.Split(char.Parse(dk.ItemSep));
            foreach (string mPan in mPans)
            {
                var li = new ListItem { Text = mPan, Value = mPan };
                ddlPanList1.Items.Add(li);
                ddlPanList2.Items.Add(li);
                ddlPanList3.Items.Add(li);
            }
        }

        private void fillddlAccountHolderNames()
        {
            ddlAccountHolderNames1.Items.Clear();
            ddlAccountHolderNames2.Items.Clear();
            ddlAccountHolderNames3.Items.Clear();
            ddlFromAccountHolders.Items.Clear();
            ddlToAccountHolders.Items.Clear();
            {
                var li = new ListItem { Text = SelectHolder, Value = SelectHolder };
                ddlFromAccountHolders.Items.Add(li);
                ddlToAccountHolders.Items.Add(li);
                ddlAccountHolderNames1.Items.Add(li);
                ddlAccountHolderNames2.Items.Add(li);
                ddlAccountHolderNames3.Items.Add(li);
            }
            if (String.IsNullOrEmpty(hiddenAccountHoldersList.Value))
            {
                AddToLogAndDisplay("Account Holders List empty");
                return;
            }
            var AccountHolderNames = hiddenAccountHoldersList.Value.Split(char.Parse(dk.ItemSep));
            foreach (var AccountHolderName in AccountHolderNames)
            {
                var nametype = AccountHolderName.Split(':');
                var li = new ListItem { Text = nametype[0] + @" (" + nametype[1] + @")", Value = nametype[0] };
                ddlFromAccountHolders.Items.Add(li);
                ddlToAccountHolders.Items.Add(li);
                ddlAccountHolderNames1.Items.Add(li);
                ddlAccountHolderNames2.Items.Add(li);
                ddlAccountHolderNames3.Items.Add(li);
            }
        }
        
        private void fillddlLoginNames()
        {
            ddlLoginNames.Items.Clear();
            {
                var li = new ListItem { Text = SelectLogin, Value = SelectLogin };
                ddlLoginNames.Items.Add(li);
            }
            if (String.IsNullOrEmpty(hiddenLoginNamesList.Value))
            {
                AddToLogAndDisplay("Login Names List empty");
                return;          
            }

            var mNames = hiddenLoginNamesList.Value.Split(char.Parse(dk.ItemSep));
            foreach (var mName in mNames)
            {
                var li = new ListItem { Text = mName, Value = mName };
                ddlLoginNames.Items.Add(li);
            }
        }

        private void fillddlAccountNames()
        {
            ddlAccountNames.Items.Clear();
            ddlAccountNames3.Items.Clear();
            {
                var li = new ListItem { Text = SelectAccountName, Value = SelectAccountName };
                ddlAccountNames.Items.Add(li);
                ddlAccountNames3.Items.Add(li);
            }
            {
                var li = new ListItem { Text = SelectAccountNumber, Value = SelectAccountNumber };
                ddlFromAccountNo.Items.Add(li);
                ddlToAccountNo.Items.Add(li);
            }
            if (String.IsNullOrEmpty(hiddenAccountNamesList.Value))
            {
                AddToLogAndDisplay("Account Names List empty");
                return;
            }

            var mAccNames = hiddenAccountNamesList.Value.Split(char.Parse(dk.ItemSep));
            foreach (var mAccName in mAccNames)
            {
                var li = new ListItem { Text = mAccName, Value = mAccName };
                ddlAccountNames.Items.Add(li);
                ddlAccountNames3.Items.Add(li);
            }
        }
        #endregion

        #region Common Methods
        protected string GetCidUsingName()
        {
            var mUtils = new Utils();
            var mClient = mUtils.GetClientUsingClientName(MTB);
            if (mClient == null)
                return cs.DefaultClientId;
            return mClient.ClientId.ToString();
        }

        private void ParseAndDisplay(string xml)
        {
            var txt = xml.Replace("|", ", ");
            txt = txt.Replace("<Reply>", "|<Reply>").Replace("<Details>", "|<Details>");
            txt = txt.Replace("<Details><Account>", "<Details>|<Account>");
            txt = txt.Replace("<SubAccounts>", "|<SubAccounts>").Replace("</SubAccounts>", "|<SubAccounts>");
            txt = txt.Replace("<SubAccount>", "|  <SubAccount>");
            txt = txt.Replace("<ActivityLog>", "|<ActivityLog>").Replace("<LogEntry>", "|  <LogEntry>");
            txt = txt.Replace("</ActivityLog>", "|</ActivityLog>|").Replace("</Details>", "|</Details>|");
            AddToLogAndDisplay(txt);
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
                var request = MacServicesUrl + TestLib.TestConstants.MacTestBankUrl;
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
                elemList = xmlDoc.GetElementsByTagName("TotalAccounts");
                if (elemList.Count != 0)
                    lbTotalNumberOfAccounts.Text = elemList[0].InnerXml;
                elemList = xmlDoc.GetElementsByTagName("AssignedAccounts");
                if (elemList.Count != 0)
                    lbNumberOfAccountsAssigned.Text = elemList[0].InnerXml;
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
    }
}