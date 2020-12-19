using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Xml;
using System.Threading;
using System.Net.Mail;
using System.Web.Services;
using System.Configuration;

using sr = MACServices.Constants.ServiceResponse;
using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using cs = MACServices.Constants.Strings;

using tc = TestLib.TestConstants.TestBank;

public partial class Registration_Default : Page
{
    private const string QS_demo = "demo";
    private const string QS_cid = "cid";
    private const string QS_action = "action";
    private const string QS_shortCode = "shortCode";
    private const string QS_userID = "userID";
    private const string QS_config = "cfg";
    private const string QS_reg = "reg";
    private const string QS_unreg = "unreg";
    private const string QS_LN = "ln";

    private const string QS_fromUrl = "fromUrl";

    private const string RegistrationConfigFileName = "RegConfig.txt";
    private const string CfgRegistrationType = "RegistrationType";
    private const string CfgmacbankUrl = "macbankUrl";

    // Send Email Config parameters
    private const string cfgEmailServer = "EmailServer";
    private const string cfgEmailPort = "EmailPort";
    private const string cfgEmailLoginUserName = "EmailLoginUserName";
    private const string cfgEmailPassword = "EmailPassword";
    private const string cfgEmailToAddress = "EmailToAddress";
    private const string cfgEmailFromAddress = "EmailFromAddress";

    protected void Page_Load(object sender, EventArgs e)
    {

        if (IsPostBack)
        {
            if (hiddenAA.Value == "registerUser")
                btnRegisterEndUser_Click();
        }
        else
        {
            // Determine if registering or unsubscribing
            string regAction = Request.QueryString[QS_action];
            if (regAction == "reg")
                txtFirstName.Focus();
            else if (regAction == "cfg")
            {
                txtMacBankUrl.Focus();
                btnBackCfg.Visible = false;
            }
            else
                txtUnsubscribe.Focus();            
            
            // get referrer URL and remove any parameters
            string referrerUrl = Request.QueryString[QS_fromUrl];
            if (referrerUrl != null)
            {
                hiddenCallerURL.Value = referrerUrl;
            }
            else
                hiddenCallerURL.Value = "";

            Log("Page_Load.hiddenCallerURL.Value: " + hiddenCallerURL.Value);

            hiddenHost.Value = HttpContext.Current.Request.ServerVariables["SERVER_NAME"].ToString();
            Log("Page_Load.hiddenHost.Value: " + hiddenHost.Value);

            hiddenHost.Value = hiddenHost.Value.Replace("http://", "").Replace("https://", "");
            Log("Page_Load.hiddenHost.Value2: " + hiddenHost.Value);

            var chparts = hiddenHost.Value.Split('.');
            var demoConfig = chparts[0] + "." + RegistrationConfigFileName;
            Log("Page_Load.demoConfig: " + demoConfig);

            hiddenT.Value = Request.QueryString[QS_action];
            Log("Page_Load.hiddenT.Value: " + hiddenT.Value);
            hiddenE.Value = Request.QueryString[QS_userID];
            Log("Page_Load.hiddenE.Value: " + hiddenE.Value);
            hiddenShortCode.Value = Request.QueryString[QS_shortCode];
            Log("Page_Load.hiddenShortCode.Value: " + hiddenShortCode.Value);

            if (String.IsNullOrEmpty(hiddenT.Value))
            {
                hiddenT.Value = String.Empty;
                divCfgForm.Visible = false;
                divRegForm.Visible = false;
                divUnregister.Visible = false;
                divStopReset.Visible = false;
                //if (Request.UrlReferrer != null)
                //{
                //    hiddenCallerURL.Value = Request.UrlReferrer.ToString();
                //    Response.Redirect(hiddenCallerURL.Value, true);
                //}
                return;
            }

            if (String.IsNullOrEmpty(Request.QueryString[QS_LN]))
                hiddenLN.Value = "NA";
            else
                hiddenLN.Value = HexToString(Request.QueryString[QS_LN]);

            hiddenMACBankUrl.Value = string.Empty;
            var mpath = HttpContext.Current.Server.MapPath(".");
            var mfile = Path.Combine(mpath, demoConfig);

            if (!File.Exists(mfile))
            {
                divRegForm.Visible = false;
                divCfgForm.Visible = true;
                divUnregister.Visible = false;
                divStopReset.Visible = false;
                return;
            }

            if (File.Exists(mfile))
            {
                // Hide registration form and show config form
                string line;

                // Read the file and display it line by line.
                var file = new StreamReader(mfile);
                while ((line = file.ReadLine()) != null)
                {
                    if (line.Length < 5) continue; // line too short, ignore
                    if (line.StartsWith("#")) continue; //comment line, ignore
                    // fill in form fields based of what line startswith
                    if (line.StartsWith(CfgmacbankUrl))
                        hiddenMACBankUrl.Value = line.Replace(CfgmacbankUrl + ":", "");
                    if (line.StartsWith(CfgRegistrationType))
                        hiddenRegistrationType.Value = line.Replace(CfgRegistrationType + ":", "");

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
                txtMacBankUrl.Text = hiddenMACBankUrl.Value;
                txtRegType.Text = hiddenRegistrationType.Value;

                // Email settings
                txtEmailServer.Text = hiddenEmailServer.Value;
                txtEmailPort.Text = hiddenEmailPort.Value;
                txtEmailLoginUserName.Text = hiddenEmailLoginUserName.Value;
                txtEmailPassword.Text = hiddenEmailPassword.Value;
                txtEmailToAddress.Text = hiddenEmailToAddress.Value;
                txtEmailFromAddress.Text = hiddenEmailFromAddress.Value;
            }
            if (hiddenT.Value == QS_config)
            {
                divRegForm.Visible = false;
                divCfgForm.Visible = true;
                divUnregister.Visible = false;
                divStopReset.Visible = false;
            }
            else if (hiddenT.Value == QS_reg)
            {
                divRegForm.Visible = true;
                divCfgForm.Visible = false;
                divUnregister.Visible = false;
                divStopReset.Visible = false;
                divStopReset.Visible = false;

                hiddenDemo.Value = Request.QueryString[QS_demo];
                if (String.IsNullOrEmpty(hiddenDemo.Value))
                {
                    lbError.Text += @" No demo name in request";
                    //btnRegisterEndUser.Enabled = false;
                }
                hiddenCID.Value = Request.QueryString[QS_cid];
                if (String.IsNullOrEmpty(hiddenCID.Value))
                {
                    lbError.Text += @" No cid in request";
                    //btnRegisterEndUser.Enabled = false;
                }
            }
            else if (hiddenT.Value == QS_unreg)
            {
                divRegForm.Visible = false;
                divCfgForm.Visible = false;
                divUnregister.Visible = true;
                divUnsubscribe.Visible = true;
                divUnsubscribeMessage.Visible = false;
                divStopReset.Visible = false;

                hiddenCID.Value = Request.QueryString[QS_cid];
                if (String.IsNullOrEmpty(hiddenCID.Value))
                {
                    lbError.Text += @" No cid in request";
                    //btnRegisterEndUser.Enabled = false;
                }
            }
            else if (hiddenT.Value == "STOP")
            {
                divRegForm.Visible = false;
                divCfgForm.Visible = false;
                divUnregister.Visible = false;
                divUnsubscribe.Visible = false;
                divUnsubscribeMessage.Visible = false;
                userID.InnerText = hiddenE.Value;
                textNotification.InnerText = hiddenShortCode.Value;
                divStopReset.Visible = true;
            }
            else
            {
                return;
            }
            // Caller
            //if (Request.UrlReferrer != null)
            //{
            //    string testURL = Request.UrlReferrer.ToString();
            //    if (testURL.Contains("?"))
            //    {
            //        string[] splitURL = testURL.Split('?');
            //        hiddenCallerURL.Value = splitURL[0];
            //    }
            //    else
            //    {
            //        hiddenCallerURL.Value = Request.UrlReferrer.ToString();
            //    }
            //}

        }
    }

    #region Button event handlers
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
            Log("btnUnsubscribe_Click." + tc.ValidateLoginName + ".Request: " + txtUnsubscribe.Value);
            var rply = SendRequestToMacTestBankServer(hiddenCID.Value,
                dk.Request + dk.KVSep + tc.ValidateLoginName +
                dk.ItemSep + dk.CID + dk.KVSep + hiddenCID.Value +
                dk.ItemSep + tc.LoginName + dk.KVSep + txtUnsubscribe.Value, tc.ValidateLoginName);

            Log("btnUnsubscribe_Click." + tc.ValidateLoginName + ".Reponse: " + rply.Item1 + "," + rply.Item2);
            if (rply.Item1 == false)
            {
                lbError3.Text = rply.Item2;
                return;
            }
            if (rply.Item2.Contains("Error"))
            {
                //lbLoginError.Text = "Enter correct login name<br/>or click the 'To Register' button.";
                lbError3.Text = "Email address not found.";
                return;
            }
            hiddenLoginName.Value = txtUnsubscribe.Value;
            hiddenLastName.Value = rply.Item2;
        }
        catch (Exception ex)
        {
            Log("btnUnsubscribe_Click." + tc.ValidateLoginName + ".Exception: " + ex.Message);
            lbError3.Text = "Exception: " + ex.Message;
            return;
        }
        //delete account using last name & email
        try
        {
            Log("btnUnsubscribe_Click." + tc.DeleteUserAccount + ".Request: " + txtUnsubscribe.Value);
            var rply = SendRequestToMacTestBankServer(cs.DefaultClientId,
                dk.Request + dk.KVSep + tc.DeleteUserAccount +
                dk.ItemSep + tc.LoginName + dk.KVSep + txtUnsubscribe.Value +
                dk.ItemSep + dk.CID + dk.KVSep + hiddenCID.Value, null);

            Log("btnUnsubscribe_Click." + tc.DeleteUserAccount + ".Reponse: " + rply.Item1 + "," + rply.Item2);
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

        }
        catch (Exception ex)
        {
            Log("btnUnsubscribe_Click." + tc.DeleteUserAccount + ".Exception: " + ex.Message);
            lbError3.Text = "Exception: " + ex.Message;
        }

        lbError3.Text = "";
        legendLabel.InnerHtml = "Success";
        deletedEmail.InnerHtml = hiddenLoginName.Value;
        divUnregister.Visible = true;
        divUnsubscribe.Visible = false;
        divUnsubscribeMessage.Visible = true;

    }

    //public void btnToRegister_Click(object sender, EventArgs e)
    //{
    //    lbError3.Text = "";
    //    if (String.IsNullOrEmpty(txtLoginName.Value))
    //    {
    //        lbError3.Text = @"You must enter a login name!";
    //        return;
    //    }
    //    var url = hiddenRegisterUrl.Value +
    //        "?action=reg" +
    //        "&demo=" + hiddenO.Value +
    //        "&cid=" + hiddenCID.Value +
    //        "&ln=" + StringToHex(txtLoginName.Value);
    //    Response.Redirect(url, false);
    //}

    /// <summary> Register from data on form </summary>
    //public void btnRegisterEndUser_Click(object sender, EventArgs e)
    [WebMethod]
    public void btnRegisterEndUser_Click()
    {
        try
        {
            Log("btnRegisterEndUser_Click");
            var validated = hiddenValidation.Value.ToString();

            if (validated == "false")
            {
                lbError.Text = @"Could not validate form!";
                return;
            }

            if (String.IsNullOrEmpty(txtEmailAdr.Value))
            {
                lbError.Text = @"Email address required";
                return;
            }

            if (hiddenLN.Value != "NA")
            {
                if (txtEmailAdr.Value.ToLower() != hiddenLN.Value.ToLower())
                {
                    lbError.Text = @"Invalid login name!";
                    return;
                }
            }

            lbError.Text = String.Empty;
            // captcha code
            // Check if special qa user(s) check if code beging with QA
            if (txtFirstName.Value == "QAUser")
            {
                if (!txtimgcode.Value.Contains("QA"))
                {
                    lbError.Text = @"Re-enter Image Code";
                    return;
                }
            }
            else
            {
                //********** uncomment this! ********** 
                if (txtimgcode.Value != Session["CaptchaImageText"].ToString())
                {
                    lbError.Text = @"Re-enter Image Code";
                    return;
                }
                //********** uncomment this! **********
            }
            Log("btnRegisterEndUser_Click2");
            lbError.Text = String.Empty;
            // captch a code
            //if (txtimgcode.Text != Session["CaptchaImageText"].ToString())
            //{
            //    lbError.Text = @"Image code is incorrect. ";
            //    lbError.Text = "";
            //    return;
            //}
            txtimgcode.Value = "";

            //
            if (String.IsNullOrEmpty(hiddenMACBankUrl.Value))
            {
                lbError.Text += @"config error 1";
                return;
            }
            Log("btnRegisterEndUser_Click.hiddenMACBankUrl.Value: " + hiddenMACBankUrl.Value);
            if (String.IsNullOrEmpty(hiddenRegistrationType.Value))
            {
                lbError.Text += @"config error 2";
                return;
            }
            Log("btnRegisterEndUser_Click.hiddenRegistrationType.Value: " + hiddenRegistrationType.Value);
            var regtype = dv.OpenRegister;
            if (hiddenRegistrationType.Value.ToLower().Contains("client"))
                regtype = dv.ClientRegister;
            else if (hiddenRegistrationType.Value.ToLower().Contains("group"))
                regtype = "GroupRegister";

            if (String.IsNullOrEmpty(hiddenCID.Value))
            {
                lbError.Text += @"config error 3";
                return;
            }

            var mRequestInfo = dk.Request + dk.KVSep + tc.AssignAndReg +
                               dk.ItemSep + dk.CID + dk.KVSep + hiddenCID.Value +
                               dk.ItemSep + dkui.FirstName + dk.KVSep + txtFirstName.Value +
                               dk.ItemSep + dkui.LastName + dk.KVSep + txtLastName.Value +
                               dk.ItemSep + dkui.PhoneNumber + dk.KVSep + txtMPhoneNo.Value +
                               dk.ItemSep + dkui.EmailAddress + dk.KVSep + txtEmailAdr.Value +
                               dk.ItemSep + tc.Type + dk.KVSep + tc.User +
                               dk.ItemSep + dk.RegistrationType + dk.KVSep + regtype +
                               dk.ItemSep + dk.NotificationOption + dk.KVSep + dv.Email;

            var mRequest = mRequestInfo.Trim();
            Log("btnRegisterEndUser_Click." + tc.AssignAndReg + ".Request: " + mRequest);
            var reply = SendRequestToMacTestBankServer(hiddenCID.Value, mRequest, null);
            Log("btnRegisterEndUser_Click." + tc.AssignAndReg + ".Response: " + reply);
            if (reply.Item1 == false)
            {
                Log(reply.Item2);
                //lbError.Text = reply.Item2;
                lbError.Text = "Unable to register due to a client configuration error.<br />Please contact the system administrator.";
                return;
            }
            // successful
                
            // If the browser is not connected stop all response processing.
            if (!Response.IsClientConnected)
            {
                Log("the browser is not connected stop all response processing");
                Response.End();
            }

            // send email to info@mobileauthcorp.com
            var subject = "Demo Registration: " + hiddenDemo.Value;
            var body = "";
            body += @"The following information was submitted to the <strong>" + hiddenDemo.Value + "</strong> demo registration system: ||";
            body += @"<strong>Registration Form Field Values</strong>" + "|";
            body += @"First Name: " + txtFirstName.Value + "|";
            body += @"Last Name: " + txtLastName.Value + "|";
            if (txtCompany.Value != "")
            {
                body += @"Company: " + txtCompany.Value + "|";
            }
            else
            {
                body += @"Company: <em>Not provided</em> |";
            }
            if (txtJobTitle.Value != "")
            {
                body += @"Job Title: " + txtJobTitle.Value + "|";
            }
            else
            {
                body += @"Job Title: <em>Not provided</em> |";
            }
            body += @"Mobile Phone: " + txtMPhoneNo.Value + "|";
            body += @"Email: " + txtEmailAdr.Value + "||";
            // hidden fields
            body += @"<strong>Hidden Form Field Values</strong>" + "|";
            body += @"Client ID: " + hiddenCID.Value + "|";
            body += @"Caller URL: " + hiddenCallerURL.Value + "|";
            body += @"MAC Bank URL: " + hiddenMACBankUrl.Value + "|";
            body += @"Registration Type: " + hiddenRegistrationType.Value + "|";

            SendEmail(subject, body);

            var hexEmail = txtEmailAdr.Value.Replace("@", "~");                

            var url = hiddenCallerURL.Value +
                //"?action=registered&regEmail=" + txtEmailAdr.Value + "&msg=Registration complete. Please login using your registered Email Address.",
                "?action=registered&regEmail=" + hexEmail + "&msg=Registration complete. Please login using your registered Email Address.";
            Log("btnRegisterEndUser_Click.Redirect: " + url);
            Response.Redirect(url, false);
            //Response.End();
        }
        catch (Exception ex)
        {
            Log("btnRegisterEndUser_Click.Redirect.Exception: " + ex.ToString());
            lbError.Text = "btnRegisterEndUser_Click: " + ex.Message;
        }
    }

    public void btnTestNavBack_Click(object sender, EventArgs e)
    {

        if (!Response.IsClientConnected)
        {
            Log("btnTestNavBack_Click.!Response.IsClientConnected");
            Response.End();
        }
        Log("btnTestNavBack_Click.!Response.Redirect: " + hiddenCallerURL.Value);
        Response.Redirect(hiddenCallerURL.Value, false);
        Response.End();
    }

    public void btnSave_Click(object sender, EventArgs e)
    {
        var mpath = HttpContext.Current.Server.MapPath(".");
        var chparts = hiddenHost.Value.Split('.');
        var ConfigFileName = chparts[0] + "." + RegistrationConfigFileName;
        var mfile = Path.Combine(mpath, ConfigFileName);
        
        File.WriteAllText(mfile, String.Empty);

        using (var file = new StreamWriter(mfile, true))
        {
            file.WriteLine(CfgmacbankUrl + ":" + txtMacBankUrl.Text);
            file.WriteLine(CfgRegistrationType + ":" + txtRegType.Text);

            // Send Email Config parameters
            file.WriteLine(cfgEmailServer + ":" + txtEmailServer.Text);
            file.WriteLine(cfgEmailPort + ":" + txtEmailPort.Text);
            file.WriteLine(cfgEmailLoginUserName + ":" + txtEmailLoginUserName.Text);
            file.WriteLine(cfgEmailPassword + ":" + txtEmailPassword.Text);
            file.WriteLine(cfgEmailToAddress + ":" + txtEmailToAddress.Text);
            file.WriteLine(cfgEmailFromAddress + ":" + txtEmailFromAddress.Text);

        }
        // display saved message
        ScriptManager.RegisterStartupScript(Page, Page.GetType(),
            "err_msg",
            "alert('Your settings have been saved.');",
            true);
    }
    #endregion

    #region Service Calls
    private Tuple<bool, string> SendRequestToMacTestBankServer(string pClientId, string requestData, string pRequest)
    {
        try
        {
            var dataStream = Encoding.UTF8.GetBytes("data=99" + pClientId.Length + pClientId.ToUpper() + StringToHex(requestData));
            var request = hiddenMACBankUrl.Value;
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

            var userExists = xmlDoc.InnerXml.ToString();
            if (userExists.Contains("Already an account with login"))
                return new Tuple<bool, string>(false, registrationError.InnerText = "An account with email " + txtEmailAdr.Value + " already exists.");

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
                    return new Tuple<bool, string>(false, "Error: " + elemList[0].InnerXml);

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
    #endregion

    public void btnHome_Click(object sender, EventArgs e)
    {
        //if (Request.QueryString["msg"] != null)
        //{
        //    var x = Request.RawUrl.Split('?');
        //    Response.Redirect(x[0]);
        //}
        if (!Response.IsClientConnected)
            Response.End();
        Response.Redirect(hiddenCallerURL.Value, false);
        Response.End();
        //Response.Redirect(Request.RawUrl);
        //Response.Redirect("../GolfShop/default.aspx");
    }


    #region Helpers

    public string SendEmail(string pSubject, string pBody)
    {
        // Do not wait for smtp to send the email, start a thread
        try
        {
            var myThread = new Thread(() => ThreadedSendEmail(pSubject, pBody));
            myThread.Start();
            return "Sent";
        }
        catch (Exception ex)
        {
            return "Failed: " + ex.Message;
        }
    }

    /// <summary> Send ProviderEmail Thread </summary>
    private void ThreadedSendEmail(string pSubject, string pBody)
    {
        try
        {
            //Set up message
            var message = new MailMessage { From = new MailAddress(hiddenEmailFromAddress.Value) };
            message.To.Add(new MailAddress(hiddenEmailToAddress.Value));
            message.Subject = pSubject;
            message.IsBodyHtml = true;
            // form fields
            

            message.Body = pBody.Replace("|", "<br />");
            message.Priority = MailPriority.High;

            // setup Smtp Client
            var smtp = new SmtpClient
            {
                Port = int.Parse(hiddenEmailPort.Value),
                Host = hiddenEmailServer.Value,
                EnableSsl = false,
                UseDefaultCredentials = true
            };
            smtp.Credentials = new NetworkCredential(hiddenEmailLoginUserName.Value, hiddenEmailPassword.Value);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Send(message);
        }
        // ReSharper disable once EmptyGeneralCatchClause
        catch
        {
            /* ignore */
        }
    }

    public string HexToString(String input)
    { // data is encoded in hex, convert it back to a string
        if (String.IsNullOrEmpty(input)) return null;
        try
        {
            var sb = new StringBuilder();
            for (var i = 0; i < input.Length; i += 2)
            {
                var hs = input.Substring(i, 2);
                sb.Append(Convert.ToChar(Convert.ToUInt32(hs, 16)));
            }
            return sb.ToString();
        }
        catch
        {
            return null;
        }
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


    protected void Log(string pEntry)
    {
        var mLogging = ConfigurationManager.AppSettings["Logging"];
        if (!String.IsNullOrEmpty(mLogging))
        {
            if (mLogging.ToLower() == "true")
            {
                const string mLogDir = "c:/temp";
                const string mFile = "Registrationlog.txt";
                if (!Directory.Exists(mLogDir))
                {
                    Directory.CreateDirectory(mLogDir);
                }
                using (var file = new StreamWriter(Path.Combine(mLogDir, mFile), true))
                {
                    file.WriteLine(pEntry);
                }
            }
        }
    }

    #endregion
}