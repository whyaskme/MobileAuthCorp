using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Threading;
using System.Net.Mail;

using MACSecurity;
using sr = MACServices.Constants.ServiceResponse;
using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using cs = MACServices.Constants.Strings;
using tc = TestLib.TestConstants.TestBank;

public partial class Registration_unsubscribe : System.Web.UI.Page
{

    //private const string QS_demo = "demo";
    private const string QS_action = "action";
    private const string QS_config = "cfg";
    private const string demoConfig = "demoConfig.txt";
    private const string cfgClientName = "clientName";
    private const string cfgMacServicesUrl = "MacServicesUrl";
    private const string cfgMacBankUrl = "macbankUrl";
    private const string cfgRegisterUrl = "registerUrl";

    // Send Email Config parameters
    private const string cfgEmailServer = "EmailServer";
    private const string cfgEmailPort = "EmailPort";
    private const string cfgEmailLoginUserName = "EmailLoginUserName";
    private const string cfgEmailPassword = "EmailPassword";
    private const string cfgEmailToAddress = "EmailToAddress";
    private const string cfgEmailFromAddress = "EmailFromAddress";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack) return;
        //spanServerIp.InnerHtml = "Server: " + Request.ServerVariables["LOCAL_ADDR"];
        hiddenT.Value = Request.QueryString[QS_action];
        divUnsubscribe.Visible = true;
        divUnsubscribeMessage.Visible = false;;

        if (Request.QueryString["msg"] != null)
        {
            lbError3.Text = Request["msg"].ToString();
            lbError3.Visible = true;
        }

        var mpath = HttpContext.Current.Server.MapPath(".");
        var mfile = Path.Combine(mpath, demoConfig);
        if (File.Exists(mfile))
        {
            string line;

            // Read the file and display it line by line.
            var file = new StreamReader(mfile);
            while ((line = file.ReadLine()) != null)
            {
                if (line.Length < 5) continue;
                if (line.StartsWith("#")) continue;

                if (line.StartsWith(cfgClientName))
                    hiddenO.Value = line.Replace(cfgClientName + ":", "");
                 else if (line.StartsWith(cfgMacServicesUrl))
                    hiddenMacServicesUrl.Value = line.Replace(cfgMacServicesUrl + ":", "");
                else if (line.StartsWith(cfgMacBankUrl))
                    hiddenMacbankUrl.Value = line.Replace(cfgMacBankUrl + ":", "");
                else if (line.StartsWith(cfgRegisterUrl))
                    hiddenRegisterUrl.Value = line.Replace(cfgRegisterUrl + ":", "");

                    // Read send email parameters
                else if (line.StartsWith(cfgEmailServer))
                    hiddenEmailServer.Value = line.Replace(cfgEmailServer + ":", "");
                else if (line.StartsWith(cfgEmailPort))
                    hiddenEmailPort.Value = line.Replace(cfgEmailPort + ":", "");
                else if (line.StartsWith(cfgEmailLoginUserName))
                    hiddenEmailLoginUserName.Value = line.Replace(cfgEmailLoginUserName + ":", "");
                else if (line.StartsWith(cfgEmailPassword))
                    hiddenEmailPassword.Value = line.Replace(cfgEmailPassword + ":", "");
                else if (line.StartsWith(cfgEmailToAddress))
                    hiddenEmailToAddress.Value = line.Replace(cfgEmailToAddress + ":", "");
                else if (line.StartsWith(cfgEmailFromAddress))
                    hiddenEmailFromAddress.Value = line.Replace(cfgEmailFromAddress + ":", "");
            }

            file.Close();
            // move values to text boxes on form
            //clientName.Text = hiddenO.Value;
            //macServicesUrl.Text = hiddenMacServicesUrl.Value;
            //macbankUrl.Text = hiddenMacbankUrl.Value;
            //registerUrl.Text = hiddenRegisterUrl.Value;
            //txtEmailServer.Text = hiddenEmailServer.Value;
            //txtEmailPort.Text = hiddenEmailPort.Value;
            //txtEmailLoginUserName.Text = hiddenEmailLoginUserName.Value;
            //txtEmailPassword.Text = hiddenEmailPassword.Value;
            //txtEmailFromAddress.Text = hiddenEmailToAddress.Value;
            //txtEmailToAddress.Text = hiddenEmailFromAddress.Value;

            //if (hiddenT.Value == QS_config)
            //{
            //    divLogin.Visible = false;
            //    divClientAdmin.Visible = true;
            //}
            //else
            //{
            //    divClientAdmin.Visible = false;
            //    divLogin.Visible = true;
            //    if (String.IsNullOrEmpty(hiddenMacbankUrl.Value))
            //    {
            //        lbError.Text += @"No demo name in request";
            //        btnValidateLoginName.Enabled = false;
            //    }
            //}
        }
        //else
        //{
        //    divClientAdmin.Visible = false;
        //    divLogin.Visible = false;
        //}
        if (!String.IsNullOrEmpty(hiddenO.Value))
            getClientIdFromMacBank(hiddenO.Value);
    }

    public void btnUnsubscribe_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtUnsubscribe.Value))
        {
            lbError3.Text = "Login ID required.";
            return;
        }
        //validate user & get last name
        try
        {
            var rply = SendRequestToMacTestBankServer(hiddenCID.Value,
                dk.Request + dk.KVSep + tc.ValidateLoginName +
                dk.ItemSep + dk.CID + dk.KVSep + hiddenCID.Value +
                dk.ItemSep + tc.LoginName + dk.KVSep + txtUnsubscribe.Value, tc.ValidateLoginName);
            if (rply.Item1 == false)
            {
                lbError3.Text = rply.Item2;
                return;
            }
            if (rply.Item2.Contains("Error"))
            {
                lbError3.Text = "User not found";
                return;
            }
            hiddenLoginName.Value = txtUnsubscribe.Value;
            hiddenLastName.Value = rply.Item2;
        }
        catch (Exception ex)
        {
            lbError3.Text = "Page_Load.getClientIdFromMacBank: " + ex.Message;
            return;
        }
        //delete account using last name & email
        try
        {
            var rply = SendRequestToMacTestBankServer(cs.DefaultClientId,
                dk.Request + dk.KVSep + tc.DeleteUserAccount +
                dk.ItemSep + dkui.LastName + dk.KVSep + hiddenLastName.Value +
                dk.ItemSep + dkui.EmailAddress + dk.KVSep + hiddenLoginName.Value +
                dk.ItemSep + dk.CID + dk.KVSep + hiddenCID.Value, null);

            if (rply.Item1 == false)
            {
                lbError3.Text = "Page_Load.getClientIdFromMacBank: " + rply.Item2;
                return;
            }
            if (rply.Item2.Contains("Error"))
            {
                lbError3.Text = "Page_Load.getClientIdFromMacBank: " + rply.Item2;
                return;
            }
            hiddenCID.Value = rply.Item2;
        }
        catch (Exception ex)
        {
            lbError3.Text = "Page_Load.getClientIdFromMacBank: " + ex.Message;
        }

        lbError3.Text = "";
        legendLabel.InnerHtml = "Thank You";
        deletedEmail.InnerHtml = hiddenLoginName.Value;
        divUnsubscribe.Visible = false;
        divUnsubscribeMessage.Visible = true;

    }

    private Tuple<bool, string> SendRequestToMacTestBankServer(string pClientId, string requestData, string pRequest)
    {
        try
        {
            var dataStream = Encoding.UTF8.GetBytes("data=99" + pClientId.Length + pClientId.ToUpper() + StringToHex(requestData));
            var request = hiddenMacbankUrl.Value;
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

            res.Close();
            if (response != null)
                response.Close();

            var elemList = xmlDoc.GetElementsByTagName(sr.Reply);
            if (elemList.Count == 0)
                return new Tuple<bool, string>(false, "Error: returned from service" + elemList[0].InnerXml);

            if (elemList[0].InnerXml.Contains(sr.Error))
            {
                elemList = xmlDoc.GetElementsByTagName(sr.Details);
                if (elemList.Count != 0)
                    return new Tuple<bool, string>(true, "Error: " + elemList[0].InnerXml);

                return new Tuple<bool, string>(false, "Error");
            }
            if (pRequest == tc.ValidateLoginName)
            {
                elemList = xmlDoc.GetElementsByTagName("LastName");
                if (elemList.Count != 0)
                    return new Tuple<bool, string>(true, elemList[0].InnerXml);
                return new Tuple<bool, string>(true, elemList[0].InnerXml);
            }

            elemList = xmlDoc.GetElementsByTagName(sr.Details);
            if (elemList.Count != 0)
                return new Tuple<bool, string>(true, elemList[0].InnerXml);

            return new Tuple<bool, string>(false, "Error");
        }
        catch (Exception ex)
        {
            return new Tuple<bool, string>(false, "Error: " + ex.Message);
        }
    }

    private void getClientIdFromMacBank(string pClientName)
    {
        try
        {
            var rply = SendRequestToMacTestBankServer(cs.DefaultClientId,
                dk.Request + dk.KVSep + dv.GetClientId +
                dk.ItemSep + dk.ClientName + dk.KVSep + pClientName, dv.GetClientId);
            if (rply.Item1 == false)
            {
                lbError3.Text = "Page_Load.getClientIdFromMacBank: " + rply.Item2;
                return;
            }
            if (rply.Item2.Contains("Error"))
            {
                lbError3.Text = "Page_Load.getClientIdFromMacBank: " + rply.Item2;
                return;
            }
            hiddenCID.Value = rply.Item2;
        }
        catch (Exception ex)
        {
            lbError3.Text = "Page_Load.getClientIdFromMacBank: " + ex.Message;
        }
    }

    public void btnHome_Click(object sender, EventArgs e)
    {
        if (Request.QueryString["msg"] != null)
        {
            var x = Request.RawUrl.Split('?');
            Response.Redirect(x[0]);
        }
        Response.Redirect(Request.RawUrl);
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
}