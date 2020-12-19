using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;

using MACServices;
using cs = MACServices.Constants.Strings;
using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using sr = MACServices.Constants.ServiceResponse;

using tc = TestLib.TestConstants.TestBank;


public partial class Admin_Tests_MACTestBank_IssueCards : System.Web.UI.Page
{
    private static string Test = "MTB_IssueCard";
    private const string MTB = "MAC Test Bank";
    private const string SelectUser = "Select User";

    HiddenField _hiddenW;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Master != null)
        {
            _hiddenW = (HiddenField)Master.FindControl("hiddenW");
            _hiddenW.Value = "54a83a64ead6362034d04bc4";
        }

        if (IsPostBack) return;
        AddToLogAndDisplay("Entry");
        var cid = GetCidUsingName();
        SendRequestToMacTestBankServer(cid, 
            dk.Request + dk.KVSep + tc.GetBankStatus +
            dk.ItemSep + dk.CID + dk.KVSep + cid);
        FillddlAccountHolderList();
    }
    protected void btnAssignUser_Click(object sender, EventArgs e)
    {
        var mAccountHolder = ddlAccountHolderList.SelectedValue;
        if (mAccountHolder == SelectUser)
        {
            lbError.Text = @"Select an Account Holder";
            return;
        }
        if (String.IsNullOrEmpty(txtCardName.Text))
        {
            lbError.Text = @"Enter a Card Name";
            return;
        }
        if (String.IsNullOrEmpty(txtCardType.Text))
        {
            lbError.Text = @"Enter Card type (" + tc.Credit + @" or Debit)";
            return;
        }
        if (txtCardType.Text != tc.Credit)
        {
            if (txtCardType.Text == @"Debit")
            {
                lbError.Text = @"Enter Card type (" + tc.Credit + @" or Debit)";
                return;
            }
        }
        if (String.IsNullOrEmpty(txtLimit.Text) == false)
        {
            if (IsNumberic(txtLimit.Text) == false)
            {
                lbError.Text = @"Limit must be numeric!";
                return;
            }
        }
        var cid = GetCidUsingName();
        var mRequest = dk.Request + dk.KVSep + tc.IssueCard +
                       dk.ItemSep + dk.CID + dk.KVSep + cid +
                       dk.ItemSep + tc.AccountHolder + dk.KVSep + mAccountHolder +
                       dk.ItemSep + tc.CardName + dk.KVSep + txtCardName.Text +
                       dk.ItemSep + tc.Type + dk.KVSep + txtCardType.Text;
        if (string.IsNullOrEmpty(txtUsage.Text) == false)
            mRequest += dk.ItemSep + tc.Usage + dk.KVSep + txtUsage.Text;
        if (string.IsNullOrEmpty(txtLimit.Text) == false)
            mRequest += dk.ItemSep + tc.Limit + dk.KVSep + txtLimit.Text;
        AddToLogAndDisplay("btnAssignUser: " + mRequest.Replace(dk.ItemSep, ", "));
        var reply = SendRequestToMacTestBankServer(cid, mRequest);
        AddToLogAndDisplay("btnAssignUser: " + reply);
    }

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

    protected string GetCidUsingName()
    {
        var mUtils = new Utils();
        var mClient = mUtils.GetClientUsingClientName(MTB);
        if (mClient == null)
            return cs.DefaultClientId;
        return mClient.ClientId.ToString();
    }
    
    protected void FillddlAccountHolderList()
    {
        ddlAccountHolderList.Items.Clear();
        {
            var li = new ListItem { Text = SelectUser, Value = SelectUser };
            ddlAccountHolderList.Items.Add(li);
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
                ddlAccountHolderList.Items.Add(li);
            }
        }
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

    private bool IsNumberic(string pNum)
    {
        int n;
        return int.TryParse(pNum, out n);
    }
}