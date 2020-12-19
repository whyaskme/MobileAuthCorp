using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Xml;

using sr = MACServices.Constants.ServiceResponse;
using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using cs = MACServices.Constants.Strings;
using tc = TestLib.TestConstants.TestBank;
using cr = MACServices.Constants.ReplyServiceRequest;
using su = MACServices.Constants.ServiceUrls;


public partial class CoffeeShop_Default : Page
{
    private const string QS_action = "action";
    private const string QS_config = "cfg";
    private const string CoffeeShopConfigFileName = "CoffeeShopConfig.txt";
    private const string cfgClientName = "clientName";
    private const string cfgMacServicesUrl = "MacServicesUrl";
    //private const string cfgMacBankUrl = "macbankUrl";
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
        hiddenHost.Value = HttpContext.Current.Request.ServerVariables["SERVER_NAME"].ToString();
        hiddenHost.Value = hiddenHost.Value.Replace("http://", "").Replace("https://", "");
        var chparts = hiddenHost.Value.Split('.');
        var demoConfig = chparts[0] + "." + CoffeeShopConfigFileName;

        hiddenT.Value = Request.QueryString[QS_action];

        //this references a hidden span that displays a server IP.
        //var ip = Request.ServerVariables["LOCAL_ADDR"];
        //if (ip.Contains(":") == false)
        //    spanServerIp.InnerHtml = "IP: " + ip;
        
        if (Request.QueryString["msg"] != null)
        {
            lbError.Text = Request["msg"].ToString();
            lbError.Visible = true;
            divErrorMessage.Visible = true;
        }

        var mpath = HttpContext.Current.Server.MapPath(".");
        var mfile = Path.Combine(mpath, demoConfig);
        if (!File.Exists(mfile))
        {
            divHome.Visible = false;
            divShopping.Visible = false;
            divClientAdmin.Visible = true;
            return;
        }

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
                //else if (line.StartsWith(cfgMacBankUrl))
                //    hiddenMacbankUrl.Value = line.Replace(cfgMacBankUrl + ":", "");
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
            clientName.Text = hiddenO.Value;
            macServicesUrl.Text = hiddenMacServicesUrl.Value;
            //macbankUrl.Text = hiddenMacbankUrl.Value;
            registerUrl.Text = hiddenRegisterUrl.Value;
            //MBMessageReplyUrl.Text = hiddenMBReplyServiceUrl.Value;
            txtEmailServer.Text = hiddenEmailServer.Value;
            txtEmailPort.Text = hiddenEmailPort.Value;
            txtEmailLoginUserName.Text = hiddenEmailLoginUserName.Value;
            txtEmailPassword.Text = hiddenEmailPassword.Value;
            txtEmailFromAddress.Text = hiddenEmailToAddress.Value;
            txtEmailToAddress.Text = hiddenEmailFromAddress.Value;

            if (hiddenT.Value == QS_config)
            {
                divHome.Visible = false;
                divShopping.Visible = false;
                divClientAdmin.Visible = true;
            }
            else
            {
                divClientAdmin.Visible = false;
                divHome.Visible = true;
                divShopping.Visible = true;
                if (String.IsNullOrEmpty(hiddenO.Value))
                {
                    lbError.Text += @"Page_Load: No demo name in request";
                    lbError.Visible = true;
                    divErrorMessage.Visible = true;
                    
                    //btnValidateLoginName.Enabled = false; //taken from golfshop demo. Unsure if it's applicable.
                }
            }
        }
        else
        {
            divClientAdmin.Visible = false;
            divHome.Visible = false;
            divShopping.Visible = false;
        }
        if (!String.IsNullOrEmpty(hiddenO.Value)) //(hiddenO.value = client name
            getClientIdFromMacBank(hiddenO.Value);

        divErrorMessage.Visible = false;
    }

    #region Button Event Handlers

    /// <summary> Register from data on form </summary>
    public void btnSaveCfg_Click(object sender, EventArgs e)
    {
        try
        {
            var mpath = HttpContext.Current.Server.MapPath(".");
            var chparts = hiddenHost.Value.Split('.');
            var demoConfig = chparts[0] + "." + CoffeeShopConfigFileName;
            var mfile = Path.Combine(mpath, demoConfig);

            File.WriteAllText(mfile, String.Empty);

            using (var file = new StreamWriter(mfile, true))
            {
                file.WriteLine(cfgClientName + ":" + clientName.Text);
                file.WriteLine(cfgMacServicesUrl + ":" + macServicesUrl.Text);
                //file.WriteLine(cfgMacBankUrl + ":" + macbankUrl.Text);
                file.WriteLine(cfgRegisterUrl + ":" + registerUrl.Text);
                //file.WriteLine(cfgMBReplyServiceURL + ":" + MBMessageReplyUrl.Text);
                // Send Email Config parameters
                file.WriteLine(cfgEmailServer + ":" + txtEmailToAddress.Text);
                file.WriteLine(cfgEmailPort + ":" + txtEmailPort.Text);
                file.WriteLine(cfgEmailLoginUserName + ":" + txtEmailLoginUserName.Text);
                file.WriteLine(cfgEmailPassword + ":" + txtEmailPassword.Text);
                file.WriteLine(cfgEmailToAddress + ":" + txtEmailToAddress.Text);
                file.WriteLine(cfgEmailFromAddress + ":" + txtEmailFromAddress.Text);
            }
        }
        catch (Exception ex)
        {
            var msg = "Exception saving configuration," + ex.Message;
            ScriptManager.RegisterStartupScript(Page, Page.GetType(),
                   "err_msg",
                   "alert(" + msg + ");",
                   true);
            return;
        }
        // display saved message
        ScriptManager.RegisterStartupScript(Page, Page.GetType(),
          "err_msg",
          "alert('Your settings have been saved.');",
          true);

    }

    public void btnCancelCfg_Click(object sender, EventArgs e)
    {
        var RawUrl = Request.RawUrl.Split('?');
        Response.Redirect(RawUrl[0]);
    }

    protected void btnPurchase_Click(object sender, EventArgs e)
    {
        lbError.Text = String.Empty;

        if (String.IsNullOrEmpty(hiddenCID.Value))
        {
            lbError.Text = @"No Client ID!";
            divErrorMessage.Visible = true;
            return;
        }
        
        if (String.IsNullOrEmpty(lbError.Text) == false)
            return;

        // todo: update displayed status and show sending otp

        // Call Request Otp Service
        var sReply = String.Empty;
        try
        {
            var url = hiddenMacServicesUrl.Value + su.RequestOtpWebService;
            var mRequestData = dk.Request + dk.KVSep + dv.SendOtp +
                            dk.ItemSep + dkui.EndUserIpAddress + dk.KVSep + GetUserIp() +
                            dk.ItemSep + dk.TrxType + dk.KVSep + "9" +
                            dk.ItemSep + dk.CID + dk.KVSep + hiddenCID.Value +
                            dk.ItemSep + dk.ReplyUri + dk.KVSep + StringToHex(
                            hiddenMacServicesUrl.Value + TestLib.TestConstants.MacReplyServiceUrl) +
                            dk.ItemSep + dkui.PhoneNumber + dk.KVSep + txtMobilePhone.Value +
                            dk.ItemSep + dkui.EmailAddress + dk.KVSep + txtEmail.Value;
            var rply = ServiceRequest(url, hiddenCID.Value, mRequestData, dv.SendOtp);
            if (rply.Item1 == false)
            {
                lbError.Text = "btnPurchase_Click: " + rply.Item2;
                divErrorMessage.Visible = true;
                return;
            }
            hiddenRequestId.Value = rply.Item2;
        }
        catch (Exception ex)
        {
            lbError.Text = "btnPurchase_Click.Request: Exception " + ex.Message;
            divErrorMessage.Visible = true;
            return;
        }
        if (sReply.StartsWith(sr.Error))
        {   // Transaction rejected
            lbError.Text = sReply;
            divErrorMessage.Visible = true;
            return;
        }

        // todo: update displayed status and show waiting authorization

        var mResult = "Waiting";
        var mWaitUntil = DateTime.Now.AddMinutes(10);
        while (true)
        {
            try
            {
                //Poll GetReply(Test Service) for text reply
                var mUri = hiddenMacServicesUrl.Value + TestLib.TestConstants.MacReplyServiceUrl
                           + "?" + cr.Request + "=" + cr.GetReplyCompletion
                           + "&" + cr.cid + "=" + hiddenCID.Value
                           + "&" + cr.RequestId + "=" + hiddenRequestId.Value;
                var webRequest = (HttpWebRequest) WebRequest.Create(mUri);
                webRequest.MaximumAutomaticRedirections = 4;
                webRequest.MaximumResponseHeadersLength = 4;
                webRequest.Credentials = CredentialCache.DefaultCredentials;
                var res = webRequest.GetResponse();
                var response = res.GetResponseStream();
                var xmlDoc = new XmlDocument();
                if (response != null) xmlDoc.Load(response);
                res.Close();
                if (response != null)
                    response.Close();

                var elemList = xmlDoc.GetElementsByTagName(sr.Error);
                if (elemList.Count != 0)
                {
                    mResult = elemList[0].InnerXml;
                    break;
                }
                elemList = xmlDoc.GetElementsByTagName(sr.Reply);
                if (elemList.Count != 0)
                {
                    if (elemList[0].InnerXml != "Wait")
                    {
                        mResult = elemList[0].InnerXml;
                        break;
                    }
                }
                if (DateTime.Now > mWaitUntil)
                {
                    break;
                }
            }
            catch (Exception ex)
            {
                // check for timeout
                mResult = "Exception: " + ex.Message;
                break;
            }
        }
        if (mResult == "Waiting")
        {
            //mResult = "Timeout waiting on your reply message!";
            mResult = "Timeout waiting on your reply message!";

            hiddenTimeout.Value = "true";
        }
        if (mResult == "Retry limit exceeded! Transaction canceled!")
        {
            hiddenTooManyRetries.Value = "true";
        }
        if (mResult == "Approved")
        {
            var d1 = hiddenDescription1.Value;
            var d2 = hiddenDescription2.Value;
            var d3 = hiddenDescription3.Value;
            var d4 = hiddenDescription4.Value;

            var q1 = hiddenQty1.Value;
            var q2 = hiddenQty2.Value;
            var q3 = hiddenQty3.Value;
            var q4 = hiddenQty4.Value;

            var st1 = hiddenSubtotal1.Value;
            var st2 = hiddenSubtotal2.Value;
            var st3 = hiddenSubtotal3.Value;
            var st4 = hiddenSubtotal4.Value;

            var xq = hiddenTotalQty.Value;
            var xt = hiddenTotalPrice.Value;

            var transaction = new StringBuilder();

            var i = 0;

            if(q1 != "0")
            {
                i++;
                transaction.Append("<strong>Item " + i + ":</strong> <span>" + d1 + "</span><br />");
                transaction.Append("<strong>Qty:</strong> <span>" + q1 + "</span><br />");
                transaction.Append("<strong>Subtotal:</strong> <span>$" + st1 + "</span>");
                transaction.Append("<hr style='margin-top:0.75rem;margin-bottom:0.75rem;border-color:#e3d4c1;' />");
            }
            if (q2 != "0")
            {
                i++;
                transaction.Append("<strong>Item " + i + ":</strong> <span>" + d2 + "</span><br />");
                transaction.Append("<strong>Qty:</strong> <span>" + q2 + "</span><br />");
                transaction.Append("<strong>Subtotal:</strong> <span>$" + st2 + "</span>");
                transaction.Append("<hr style='margin-top:0.75rem;margin-bottom:0.75rem;border-color:#e3d4c1;' />");
            }
            if (q3 != "0")
            {
                i++;
                transaction.Append("<strong>Item " + i + ":</strong> <span>" + d3 + "</span><br />");
                transaction.Append("<strong>Qty:</strong> <span>" + q3 + "</span><br />");
                transaction.Append("<strong>Subtotal:</strong> <span>$" + st3 + "</span>");
                transaction.Append("<hr style='margin-top:0.75rem;margin-bottom:0.75rem;border-color:#e3d4c1;' />");
            }
            if (q4 != "0")
            {
                i++;
                transaction.Append("<strong>Item " + i + ":</strong> <span>" + d4 + "</span><br />");
                transaction.Append("<strong>Qty:</strong> <span>" + q4 + "</span><br />");
                transaction.Append("<strong>Subtotal:</strong> <span>$" + st4 + "</span>");
                transaction.Append("<hr style='margin-top:0.75rem;margin-bottom:0.75rem;border-color:#e3d4c1;' />");
            }

            transaction.Append("<strong>Total Qty:</strong> <span>" + xq + "</span><br />");
            transaction.Append("<strong>Total Price:</strong> <span>$" + xt + "</span><br />");
            //transaction.Append("<hr style='margin-top:0.75rem;margin-bottom:0.75rem;border-color:#e3d4c1;' />");
            transaction.Append("<strong>Confirmation #:</strong> <span id='spanConfirmationNumber'></span>");            
            
            // Add to form
            transactionSummary.InnerHtml = transaction.ToString();
            transaction.Clear();
            hiddenThankYou.Value = "true";
        }
        // todo: Show completion page Display mResult on transaction completion page
    }

    public void btnClear_Click(object sender, EventArgs e)
    {
        hiddenThankYou.Value = "false";
    }

    #endregion

    #region Helper Methods
    private void getClientIdFromMacBank(string pClientName)
    {
        hiddenCID.Value = string.Empty;
        try
        {
            var url = hiddenMacServicesUrl.Value + TestLib.TestConstants.MacTestBankUrl;
            var mRequestData = dk.Request + dk.KVSep + dv.GetClientId
                      + dk.ItemSep + dk.ClientName + dk.KVSep + pClientName;
            var rply = ServiceRequest(url, cs.DefaultClientId, mRequestData, dv.GetClientId);
            if (rply.Item1 == false)
            {
                lbError.Text = "Page_Load.getClientIdFromMacBank: " + rply.Item2;
                divErrorMessage.Visible = true;
                return;
            }
            if (rply.Item2.Contains("Error"))
            {
                lbError.Text = "Page_Load.getClientIdFromMacBank: " + rply.Item2;
                divErrorMessage.Visible = true;
                return;
            }
            hiddenCID.Value = rply.Item2;
        }
        catch (Exception ex)
        {
            lbError.Text = "Page_Load.getClientIdFromMacBank: " + ex.Message;
            divErrorMessage.Visible = true;
        }
    }

    protected static string StringToHex(String input)
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

    protected static string GetUserIp()
    {
        var uip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        if (String.IsNullOrEmpty(uip))
            uip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        return uip;
    }
    #endregion

    #region Service Calls
    protected Tuple<bool, string> ServiceRequest(string pUrl, string pClientId, string requestData, string pRequest)
    {
        try
        {
            var dataStream = Encoding.UTF8.GetBytes("data=99" + pClientId.Length + pClientId.ToUpper() + StringToHex(requestData));
            var webRequest = WebRequest.Create(pUrl);
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

            var elemList = xmlDoc.GetElementsByTagName(sr.Error);
            if (elemList.Count != 0)
                return new Tuple<bool, string>(false, "Error: returned from service" + elemList[0].InnerXml);

            if (pRequest == dv.SendOtp)
            {
                elemList = xmlDoc.GetElementsByTagName(sr.RequestId);
                if (elemList.Count != 0)
                    return new Tuple<bool, string>(true, elemList[0].InnerXml);
            }
            if (pRequest == dv.GetClientId)
            {
                elemList = xmlDoc.GetElementsByTagName(sr.Reply);
                if (elemList.Count != 0)
                {
                    if (elemList[0].InnerXml == sr.Success)
                    {
                        elemList = xmlDoc.GetElementsByTagName(sr.Details);
                        if (elemList.Count != 0)
                            return new Tuple<bool, string>(true, elemList[0].InnerXml);
                    }
                }

            }
            return new Tuple<bool, string>(false, "Error");
        }
        catch (Exception ex)
        {
            return new Tuple<bool, string>(false, "SendRequestToMacTestBankServer. Error: " + ex.Message);
        }
    }
    #endregion

}