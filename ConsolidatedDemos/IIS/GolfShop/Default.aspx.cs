using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Xml;
using System.Net.Mail;
using System.Configuration;
using System.Text.RegularExpressions;

using MACSecurity;
using sr = MACServices.Constants.ServiceResponse;
using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using cs = MACServices.Constants.Strings;
using tc = TestLib.TestConstants.TestBank;

public partial class GolfShop_Default : Page
{
    //private const string QS_demo = "demo";
    private const string QS_action = "action";
    private const string QS_config = "cfg";
    private const string GolfShopConfigFileName = "GolfShopConfig.txt";
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
        hiddenHost.Value = HttpContext.Current.Request.ServerVariables["SERVER_NAME"].ToString();
        hiddenHost.Value = hiddenHost.Value.Replace("http://", "").Replace("https://", "");
        var chparts = hiddenHost.Value.Split('.');
        var demoConfig = chparts[0] + "." + GolfShopConfigFileName;

        var ip = Request.ServerVariables["LOCAL_ADDR"];
        if (ip.Contains(":") == false)
            spanServerIp.InnerHtml = "IP: " + ip; //this references a hidden span that displays a server IP.

        getAdSelectionDetailsFromForm();

        txtLoginName.Focus();
        divLoginOTPContainer.Visible = false;
        hiddenT.Value = Request.QueryString[QS_action];
        divStoreContainer.Visible = false;
        divUnsubscribe.Visible = false;
        divUnsubscribeMessage.Visible = false;
        adOTPContainer.Visible = false;
        divFeedback.Visible = false;
        divThankYou.Visible = false;
        //divAdSpecials.Visible = false;

        divErrorContainer.Visible = false;
        divErrorContainerRegister.Visible = false;

        if (Request.QueryString["regEmail"] != null)
        {
            // replace ~ with @ symbol in hex encoded email value
            var hexEmail = Request["regEmail"].ToString();
            var unHexEmail = hexEmail.Replace("~", "@");

            //txtLoginName.Value = Request["regEmail"].ToString();
            txtLoginName.Value = unHexEmail;
            txtLoginName.Focus();
            //lbError.Visible = true;
        }

        if (Request.QueryString["msg"] != null)
        {
            lbError.Text = Request["msg"].ToString();
            lbError.Visible = true;
            divErrorContainer.Visible = true;
        }

        var mpath = HttpContext.Current.Server.MapPath(".");
        var mfile = Path.Combine(mpath, demoConfig);
        if (!File.Exists(mfile))
        {
            divLogin.Visible = false;
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
            clientName.Text = hiddenO.Value;
            macServicesUrl.Text = hiddenMacServicesUrl.Value;
            macbankUrl.Text =hiddenMacbankUrl.Value;
            registerUrl.Text = hiddenRegisterUrl.Value;
            txtEmailServer.Text = hiddenEmailServer.Value;
            txtEmailPort.Text = hiddenEmailPort.Value;
            txtEmailLoginUserName.Text = hiddenEmailLoginUserName.Value;
            txtEmailPassword.Text = hiddenEmailPassword.Value;
            txtEmailFromAddress.Text = hiddenEmailToAddress.Value;
            txtEmailToAddress.Text = hiddenEmailFromAddress.Value;

            if (hiddenT.Value == QS_config)
            {
                divLogin.Visible = false;
                divClientAdmin.Visible = true;
            }
            else
            {
                divClientAdmin.Visible = false;
                divLogin.Visible = true;
                if (String.IsNullOrEmpty(hiddenMacbankUrl.Value))
                {
                    lbError.Text += @"Page_Load: No demo name in request";
                    divErrorContainer.Visible = true;
                    btnValidateLoginName.Enabled = false;
                }
            }
        }
        else
        {
            divClientAdmin.Visible = false;
            divLogin.Visible = false;
        }
        if (!String.IsNullOrEmpty(hiddenO.Value))
            getClientIdFromMacBank(hiddenO.Value);
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
                lbError.Text = "Page_Load.getClientIdFromMacBank: " + rply.Item2;
                divErrorContainer.Visible = true;
                return;
            }
            if (rply.Item2.Contains("Error"))
            {
                lbError.Text = "Page_Load.getClientIdFromMacBank: " + rply.Item2;
                divErrorContainer.Visible = true;
                return; 
            }
            hiddenCID.Value = rply.Item2;
        }
        catch (Exception ex)
        {
            lbError.Text = "Page_Load.getClientIdFromMacBank: " + ex.Message;
            divErrorContainer.Visible = true;
        }
    }

    #region Button Event Handlers

    /// <summary> Register from data on form </summary>
    public void btnSaveCfg_Click(object sender, EventArgs e)
    {
        try
        {
            var mpath = HttpContext.Current.Server.MapPath(".");
            var chparts = hiddenHost.Value.Split('.');
            var demoConfig = chparts[0] + "." + GolfShopConfigFileName;
            var mfile = Path.Combine(mpath, demoConfig);

            File.WriteAllText(mfile, String.Empty);

            using (var file = new StreamWriter(mfile, true))
            {
                file.WriteLine(cfgClientName + ":" + clientName.Text);
                file.WriteLine(cfgMacServicesUrl + ":" + macServicesUrl.Text);
                file.WriteLine(cfgMacBankUrl + ":" + macbankUrl.Text);
                file.WriteLine(cfgRegisterUrl + ":" + registerUrl.Text);

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

    

    public void btnUnsubscribe_Click(object sender, EventArgs e)
    {


    }

    public void btnFeedbackSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            var contactRequest = hiddenContactMe.Value;
            var subject = "Scottsdale Golf Shop Demo Submission";
            if (contactRequest != "") {
                subject += " - Information Requested!";
                contactRequest = "Yes";
            } else {
                contactRequest = "No";
            }                

            var body = "";
            body += @"The following information was submitted to the <strong>Scottsdale Golf Shop</strong> demo system: ||";

            body += @"Information Requested? " + contactRequest + "|";
            body += @"Last Name: " + hiddenLastName.Value + "|";
            body += @"Email: " + hiddenLoginName.Value + "|";
            //body += @"Demo: " + hiddenDemo.Value + "|";
            body += @"Client Name: " + hiddenO.Value + "|";
            body += @"Client ID: " + hiddenCID.Value + "|";
            if (hiddenGID.Value != "")
            {
                body += @"Group ID: " + hiddenGID.Value + "|";
            }
            else
            {
                body += @"Group ID: <em>Not provided</em> |";
            }
            body += @"MAC Services URL: " + hiddenMacServicesUrl.Value + "|";
            body += @"MAC Bank URL: " + hiddenMacbankUrl.Value + "|";
            body += @"Register URL: " + hiddenRegisterUrl.Value + "|";
            
            body += @"Purchase details: " + hiddenTrxDetails.Value + "|";
            body += @"Ad Type: " + dlAdType.SelectedItem.Text + "|";
            body += @"Comments: |" + selectFeedback.Text.Replace(Environment.NewLine, "|") + "|";

            SendEmail(subject, body);

            divStoreContainer.Visible = false;
            adOTPContainer.Visible = false;
            divFeedback.Visible = false;
            divClientAdmin.Visible = false;
            divFeedback.Visible = false;
            divLogin.Visible = false;
            divThankYou.Visible = true;
            //return;

            if (hiddenEmailSendError.Value == "true")
            {
                divEmailSendError.Visible = true;
                lbErrorEmailSend.Text = "Unable to send Email! Please retry or call 1-844-427-0411.";
                divFeedback.Visible = true;
                divThankYou.Visible = false;
            }

        }
// ReSharper disable once EmptyGeneralCatchClause
        catch
        {
            //lbError.Text = "btnFeedbackSubmit_Click: " + ex.Message;
        }
    }

    public void btnThankYou_Click(object sender, EventArgs e)
    {
        divStoreContainer.Visible = false;
        adOTPContainer.Visible = false;
        divFeedback.Visible = false;
        divClientAdmin.Visible = false;
        divFeedback.Visible = false;
        divLogin.Visible = false;
        divThankYou.Visible = true;
    }

    public void btnHome_Click(object sender, EventArgs e)
    {
        if (Request.QueryString["msg"] != null)
        {
            var x = Request.RawUrl.Split('?');
            Log("btnHome_Clicka: " + x[0]);
            Response.Redirect(x[0]);
        }
        Log("btnHome_Clickb: " + Request.RawUrl);
        Response.Redirect(Request.RawUrl);
    }

    public void btnCancelCfg_Click(object sender, EventArgs e)
    {
        var RawUrl = Request.RawUrl.Split('?');
        Log("btnCancelCfg_Click: " + RawUrl[0]);
        Response.Redirect(RawUrl[0]);
    }

    protected void btnPurchase_Click(object sender, EventArgs e)
    {
        if (String.IsNullOrEmpty(hiddenTrxDetails.Value))
        {
            lbError2.Text = @"There are no items to purchase!";
            lbError2.Visible = true;
            loginMessage.Visible = false;
            return;
        }

        if (String.IsNullOrEmpty(hiddenTrxDetails.Value))
        {
            lbError2.Text = @"You must select something to buy!";
            loginMessage.Visible = false;
            lbError2.Visible = true;
            return;
        }
        if (String.IsNullOrEmpty(hiddenLastName.Value))
        {
            lbError2.Text = @"No last name!";
            loginMessage.Visible = false;
            lbError2.Visible = true;
            return;
        }
        if (String.IsNullOrEmpty(hiddenLoginName.Value))
        {
            lbError2.Text = @"No Email Address!";
            loginMessage.Visible = false;
            lbError2.Visible = true;
            return;
        }
        if (String.IsNullOrEmpty(hiddenCID.Value))
        {
            lbError2.Text = @"No Client ID!";
            loginMessage.Visible = false;
            lbError2.Visible = true;
            return;
        }

        var mTrxDetailsForRequest = "";
        try
        {
            var mItems = hiddenTrxDetails.Value.Split(char.Parse(dk.ItemSep));
            foreach (var mItem in mItems)
            {
                if (mItem.StartsWith("Total"))
                {
                    totalPrice.InnerText = mItem.Replace("Total", "").Trim();
                    mTrxDetailsForRequest += dk.ItemSep + "Total  $" + mItem.Replace("Total", "").Trim();
                    continue;
                }
                var mvalue = mItem.Split(char.Parse(dk.KVSep));
                if (string.IsNullOrEmpty(mTrxDetailsForRequest))
                    mTrxDetailsForRequest = mvalue[1] + "(" + mvalue[2] + ")" + "  " + mvalue[3];
                else
                    mTrxDetailsForRequest += dk.ItemSep + mvalue[1] + "(" + mvalue[2] + ")" + "  " + mvalue[3];

                switch (mvalue[0])
                {
                    case "1":
                        item_number1_display.InnerText = mvalue[1];
                        item_quantity1.InnerText = mvalue[2];
                        item_subtotal1.InnerText = mvalue[3];
                        item_decription1_display.InnerText = mvalue[4];
                        break;
                    case "2":
                        item_number2_display.InnerText = mvalue[1];
                        item_quantity2.InnerText = mvalue[2];
                        item_subtotal2.InnerText = mvalue[3];
                        item_decription2_display.InnerText = mvalue[4];
                        break;
                    case "3":
                        item_number3_display.InnerText = mvalue[1];
                        item_quantity3.InnerText = mvalue[2];
                        item_subtotal3.InnerText = mvalue[3];
                        item_decription3_display.InnerText = mvalue[4];
                        //item_display3.Style["display"] = "block";
                        break;
                    case "4":
                        item_number4_display.InnerText = mvalue[1];
                        item_quantity4.InnerText = mvalue[2];
                        item_subtotal4.InnerText = mvalue[3];
                        item_decription4_display.InnerText = mvalue[4];
                        break;
                    case "5":
                        item_number5_display.InnerText = mvalue[1];
                        item_quantity5.InnerText = mvalue[2];
                        item_subtotal5.InnerText = mvalue[3];
                        item_decription5_display.InnerText = mvalue[4];
                        break;
                    case "6":
                        item_number6_display.InnerText = mvalue[1];
                        item_quantity6.InnerText = mvalue[2];
                        item_subtotal6.InnerText = mvalue[3];
                        item_decription6_display.InnerText = mvalue[4];
                        break;
                    case "7":
                        item_number7_display.InnerText = mvalue[1];
                        item_quantity7.InnerText = mvalue[2];
                        item_subtotal7.InnerText = mvalue[3];
                        item_decription7_display.InnerText = mvalue[4];
                        break;
                    case "8":
                        item_number8_display.InnerText = mvalue[1];
                        item_quantity8.InnerText = mvalue[2];
                        item_subtotal8.InnerText = mvalue[3];
                        item_decription8_display.InnerText = mvalue[4];
                        break;
                } // end switch

            }
        }
        catch (Exception ex)
        {
            lbError2.Text = @"btnPurchase_Click: Exception filling purchase list: " + ex.Message;
            lbError2.Visible = true;
            return;
        }
        // Call Request Otp Service
        var sReply = String.Empty;
        try
        {
            var mUserId = (Security.GetHashString(hiddenLastName.Value.ToLower() + hiddenLoginName.Value.ToLower())).ToUpper();
            hiddenOTPType.Value = "2";                      //Transaction verification
            var myMacotp = new MacOtp.MacOtp();
            sReply = myMacotp.SendOtpToRegisteredUser(
                            hiddenMacServicesUrl.Value,
                            hiddenCID.Value,
                            hiddenGID.Value,
                            hiddenLastName.Value,
                            mUserId,
                            hiddenOTPType.Value,
                            mTrxDetailsForRequest,
                            null);
        }
        catch (Exception ex)
        {
            lbError2.Text = "btnPurchase_Click.Request: Exception " + ex.Message;
            lbError2.Visible = true;
        }

        try
        {
            if (sReply.StartsWith(sr.Error))
            {
                lbError2.Text = sReply;
                lbError2.Visible = true;
                return;
            }


            var values = sReply.Split(char.Parse(dk.ItemSep));
            foreach (var value in values)
            {
                if (value.StartsWith(sr.RequestId))
                    hiddenRequestId.Value = value.Replace(sr.RequestId + dk.KVSep, "");
                if (value.StartsWith(sr.OTP))
                    txtOtp.Value = hiddenOTP.Value = value.Replace(sr.OTP + dk.KVSep, "");
                if (value.StartsWith(sr.EnterOTPAd))
                {
                    var mAd = HexToString(value.Replace(sr.EnterOTPAd + dk.KVSep, ""));
                    AdDiv.InnerHtml = mAd;
                } 
                //if (value.StartsWith(sr.ContentAd))
                //{
                //    var mAd = HexToString(value.Replace(sr.ContentAd + dk.KVSep, ""));
                //    AdDiv.InnerHtml = mAd;
                //}
            }
            if (string.IsNullOrEmpty(hiddenRequestId.Value))
            {
                lbError2.Text = @"No RequestId in response!";
                lbError2.Visible = true;
                return;
            }
            // show OTP div
            lbError2.Visible = false;
            productDisplay.Visible = false;
            legendLabel.InnerHtml = "Purchase Summary";
            buttonContainerPurchase.Visible = false;
            tableSpacer.Visible = false;
            divThankYou.Visible = false;
            loginMessage.Visible = false;
            adOTPContainer.Visible = true;

            // focus otp input field "txtOtp"
            txtOtp.Focus();

        }
        catch (Exception ex)
        {
            lbError2.Text = "btnPurchase_Click.Response: Exception " + ex.Message;
            lbError2.Visible = true;
        }
    }

    protected void btnVerifyOtp_Click(object sender, EventArgs e)
    {
        var myOtp = txtOtp.Value.Trim();
        if (String.IsNullOrEmpty(myOtp))
        {
            lbError2.Text = @"You must enter an OTP!";
            lbError2.Visible = true;
            return;
        }

        var sReply = String.Empty;
        try
        {
            var myMacotp = new MacOtp.MacOtp();
            sReply = myMacotp.VerifyOtp(
                hiddenMacServicesUrl.Value,
                hiddenCID.Value,
                hiddenRequestId.Value,
                myOtp);

        }
        catch (Exception ex)
        {
            lbError2.Text = "btnVerifyOtp_Click:Request:Exception," + ex.Message;
            lbError2.Visible = true;
        }

        try
        {
            if (sReply.Contains(sr.Validated) == false)
            {
                lbError2.Text = @"Invalid OTP. Please retry.";
                lbError2.Visible = true;
                return;
            }
            // disable enter OTP div
            //adOTPContainer.Visible = false;
            //tableContainerPurchase.Visible = false;
            divStoreContainer.Visible = false;
            divThankYou.Visible = false;

            // enable done div
            divFeedback.Visible = true;
            selectFeedback.Focus();
        }
        catch (Exception ex)
        {
            lbError2.Text = "btnVerifyOtp_Click.Respone:Exception," + ex.Message;
            lbError2.Visible = true;
        }
    }

    public void btnResendOtp_Click(object sender, EventArgs e)
    {
// ReSharper disable once NotAccessedVariable
        var sReply = String.Empty;
        try
        {
            var myMacotp = new MacOtp.MacOtp();
// ReSharper disable once RedundantAssignment
            sReply = myMacotp.ResendOtp(
                hiddenMacServicesUrl.Value,
                hiddenCID.Value,
                hiddenRequestId.Value);
        }
        catch (Exception ex)
        {
            lbError2.Text = "btnResendOtp_Click:Exception," + ex.Message;
            lbError2.Visible = true;
        }
        // need to check error in reply
    }

    protected void btnValidateLoginName_Click(object sender, EventArgs e)
    {
        lbError.Text = "";
        divErrorContainer.Visible = false;
        if (String.IsNullOrEmpty(txtLoginName.Value))
        {
            lbError.Text = @"Please enter a valid Email Address!";
            divErrorContainer.Visible = true;
            return;
        }

        if (String.IsNullOrEmpty(hiddenCID.Value))
        {
            lbError.Text = @"btnVerifyLoginOtp_Click: No Client ID.";
            divErrorContainer.Visible = true;
            return;
        }

        bool isEmail = Regex.IsMatch(txtLoginName.Value, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);

        if (!isEmail)
        {
            lbError.Text = @"Please enter a valid Email Address!";
            divErrorContainer.Visible = true;
            return;
        }

        try
        {
            Log("btnValidateLoginName." + tc.ValidateLoginName + ".Request: " + txtLoginName.Value);
            var rply = SendRequestToMacTestBankServer(hiddenCID.Value,
                dk.Request + dk.KVSep + tc.ValidateLoginName +
                dk.ItemSep + dk.CID + dk.KVSep + hiddenCID.Value +
                dk.ItemSep + tc.LoginName + dk.KVSep + txtLoginName.Value, tc.ValidateLoginName);
            if (rply.Item1 == false)
            {
                lbError.Text = rply.Item2;
                divErrorContainer.Visible = true;
                return;
            }
            Log("btnValidateLoginName." + tc.ValidateLoginName + ".Response: " + rply.Item2);
            if (rply.Item2.Contains("Error"))
            {
                btnToRegister.Enabled = true;
                //lbLoginError.Text = "Enter correct login name<br/>or click the 'To Register' button.";
                //lbError.Text = "Enter a valid login name or 'Register' a new user.";
                //lbError.Text = "Email not found. Enter a valid Email Address or Register a new user.";
                //lbError.Text = "Email not found. Enter a valid Email Address or register a new user.";
                divErrorContainer.Visible = false;
                divErrorContainerRegister.Visible = true;
                return;
            }
            hiddenLoginName.Value = txtLoginName.Value;
            hiddenLastName.Value = rply.Item2;
        }
        catch (Exception ex)
        {
            Log("btnValidateLoginName." + tc.ValidateLoginName + ".Request Exception" + ex.Message);
            lbError.Text = "btnValidateLoginName_Click.SendRequestToMacTestBankServer: Exception " + ex.Message;
            lbError.Visible = true;
        }

        // request OTP for login
        // Call Request Otp Service
        var sReply = String.Empty;
        try
        {
            var mAdSelectionDetails = getAdSelectionDetailsFromForm();

            Log("btnValidateLoginName.RequestOTP.Request: " + mAdSelectionDetails);


            var mUserId =
                (Security.GetHashString(hiddenLastName.Value.ToLower() + hiddenLoginName.Value.ToLower())).ToUpper();
            hiddenOTPType.Value = "1"; //User Authentication
            var myMacotp = new MacOtp.MacOtp();
            sReply = myMacotp.SendOtpToRegisteredUser(
                hiddenMacServicesUrl.Value,
                hiddenCID.Value,
                hiddenGID.Value,
                hiddenLastName.Value,
                mUserId,
                hiddenOTPType.Value,
                null,
                mAdSelectionDetails);
        }
        catch (Exception ex)
        {
            Log("btnValidateLoginName.RequestOTP.Request Exception: " + ex.Message);
            lbError.Text = "btnValidateLoginName_Click.SendOtpToRegisteredUser: Exception," + ex.Message;
            lbError.Visible = true;
        }

        try 
        {
            Log("btnValidateLoginName.RequestOTP.Response: " + sReply);
            if(sReply.Contains("STOP"))
            {
                lbError.Text = "";

                var myString = sReply;
                var subStrings = myString.Split('=');
                var shortCode = subStrings[1].Remove(subStrings[1].IndexOf(')'));

                var url = hiddenRegisterUrl.Value +
                    "?action=STOP" +
                    "&userID=" + hiddenLoginName.Value +
                    "&shortCode=" + shortCode;

                Response.Redirect(url, false);
            }

            if (sReply.StartsWith(sr.Error))
            {
                lbError.Text = "btnValidateLoginName_Click.Response1: " + sReply;
                lbError.Visible = true;
                divErrorContainer.Visible = true;
                return;
            }

            var values = sReply.Split(char.Parse(dk.ItemSep));
            // set ad container visibility to false. Will only display if sr.EnterOTPAd value exists. 
            divLoginAd.Visible = false;
            foreach (var value in values)
            {
                if (value.StartsWith(sr.RequestId))
                    hiddenRequestId.Value = value.Replace(sr.RequestId + dk.KVSep, "");
                if (value.StartsWith(sr.OTP))
                    txtLoginOTP.Value = hiddenOTP.Value = value.Replace(sr.OTP + dk.KVSep, "");
                if (value.StartsWith(sr.EnterOTPAd))
                {
                    var mAd = HexToString(value.Replace(sr.EnterOTPAd + dk.KVSep, ""));
                    divLoginAd.InnerHtml = mAd;
                    divLoginAd.Visible = true;
                }
            }
            if (string.IsNullOrEmpty(hiddenRequestId.Value))
            {
                lbError.Text = @"btnValidateLoginName_Click.Response2: No RequestId in response!";
                lbError.Visible = true;
                divErrorContainer.Visible = true;
                return;
            }
            // show login OTP div
            lbError.Visible = false;
            divLoginContainer.Visible = false;
            divErrorContainer.Visible = false;
            divLoginOTPContainer.Visible = true;
            txtLoginOTP.Focus();
        }
        catch (Exception ex)
        {
            lbError.Text = "btnValidateLoginName_Click.Response2: Exception," + ex.Message;
            lbError.Visible = true;
        }
    }

    protected void btnVerifyLoginOtp_Click(object sender, EventArgs e)
    {
        var myOtp = txtLoginOTP.Value.Trim();
        if (String.IsNullOrEmpty(myOtp))
        {
            otpMessage.InnerText = @"You must enter an OTP!";
            otpMessage.Visible = true;
            return;
        }
        var sReply = String.Empty;
        try
        {
            var myMacotp = new MacOtp.MacOtp();
            sReply = myMacotp.VerifyOtp(
                hiddenMacServicesUrl.Value,
                hiddenCID.Value,
                hiddenRequestId.Value,
                myOtp);
        }
        catch (Exception ex)
        {
            lbError2.Text = "btnVerifyOtp_Click.Request: Exception," + ex.Message;
            lbError2.Visible = true;
        }

        try
        {
            if (sReply.Contains(sr.Validated) == false)
            {
                otpMessage.InnerText = @"Invalid OTP. Please retry.";
                otpMessage.Visible = true;
                return;
            }

            // change to shopping div
            divLogin.Visible = false;
            divClientAdmin.Visible = false;
            divThankYou.Visible = false;
            divStoreContainer.Visible = true;

        }
        catch (Exception ex)
        {
            lbError2.Text = "btnVerifyOtp_Click.Response: Exception," + ex.Message;
            lbError2.Visible = true;
        }
    }

    protected void btnResendLoginOtp_Click(object sender, EventArgs e)
    {
// ReSharper disable once NotAccessedVariable
        var sReply = String.Empty;
        try
        {
            var myMacotp = new MacOtp.MacOtp();
// ReSharper disable once RedundantAssignment
            sReply = myMacotp.ResendOtp(
                hiddenMacServicesUrl.Value,
                hiddenCID.Value,
                hiddenRequestId.Value);

            otpMessage.InnerText = "Please enter the one-time authentication code sent to your mobile phone.";
            otpMessage.Visible = true;
        }
        catch (Exception ex)
        {
            otpMessage.InnerText = "btnResendOtp_Click.Request: " + ex.Message;
            otpMessage.Visible = true;
        }

        // need to check error in reply
    }
    
    public void btnToRegister_Click(object sender, EventArgs e)
    {
        lbError.Text = "";
        try
        {
            var currentUrl = HttpContext.Current.Request.Url.ToString();
            Log("btnToRegister_Click(currentUrl)" + currentUrl);
            var url = hiddenRegisterUrl.Value +
                "?action=reg" +
                "&demo=" + hiddenO.Value +
                "&cid=" + hiddenCID.Value +
                "&fromUrl=" + currentUrl;
            Log("btnToRegister_Click(url)" + url);
            Response.Redirect(url, false);
        }
        catch (Exception ex)
        {
            Log("btnToRegister_Click.exception: " + ex.Message);
            lbError2.Text = "btnToRegister_Click.Response.Redirect: Exception," + ex.Message;
            lbError2.Visible = true;
        }
    }

    public void btnGoToUnsubscribe_Click(object sender, EventArgs e)
    {
        lbError.Text = "";
        try
        {
            var currentUrl = HttpContext.Current.Request.Url.ToString();
            Log("btnGoToUnsubscribe_Click(currentUrl)" + currentUrl);
            var url = hiddenRegisterUrl.Value +
                "?action=unreg" +
                "&demo=" + hiddenO.Value +
                "&cid=" + hiddenCID.Value +
                "&fromUrl=" + currentUrl;
            Log("btnGoToUnsubscribe_Click(url)" + url);
            Response.Redirect(url, false);
            //Response.End();
        }
        catch (Exception ex)
        {
            Log("btnGoToUnsubscribe_Click.exception: " + ex.Message);
            lbError2.Text = "btnGoToUnsubscribe_Click.Response.Redirect: Exception," + ex.Message;
            lbError2.Visible = true;
        }
    }

    //public void btnGoToUnsubscribe_Click(object sender, EventArgs e)
    //{
    //    var url = hiddenRegisterUrl.Value +
    //    "?action=unreg" +
    //    "&demo=" + hiddenO.Value +
    //    "&cid=" + hiddenCID.Value;
    //    Response.Redirect(url, false);
    //}

    #endregion

    #region Service Calls
    protected Tuple<bool, string> SendRequestToMacTestBankServer(string pClientId, string requestData, string pRequest)
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
            return new Tuple<bool, string>(false, "SendRequestToMacTestBankServer. Error: " + ex.Message);
        }
    }
    #endregion

    #region Helpers

    protected string getAdSelectionDetailsFromForm()
    {
        var mAdSelDetails = "";

        // get ad number from ddl
        var mAdSpecificKeywords = dlAdType.SelectedItem.Text;
        var mAdNumber = dlAdType.SelectedValue;
        if (!String.IsNullOrEmpty(mAdNumber))
        {
            int mAdNo;
            if (int.TryParse(mAdNumber, out mAdNo))
            {
                if (mAdNo > 1)
                {
                    if (!String.IsNullOrEmpty(mAdSelDetails))
                        mAdSelDetails += dk.ItemSep;
                    mAdSelDetails += dk.Ads.AdNumber + dk.KVSep + mAdNumber +
                                     dk.ItemSep + dk.Ads.SpecificKeywords + dk.KVSep + StringToHex(mAdSpecificKeywords);
                }
            }
        }
        // other ad selection details go here
        return mAdSelDetails;
    }

    protected string SendEmail(string pSubject, string pBody)
    {
        // Do not wait for smtp to send the email, start a thread
        //try
        //{
        //    var myThread = new Thread(() => ThreadedSendEmail(pSubject, pBody));
        //    myThread.Start();
        //    return "Sent";
        //}
        //catch (Exception ex)
        //{
        //    return "SendEmail Failed: " + ex.Message;
        //}
        try
        {
            //Set up message
            var message = new MailMessage { From = new MailAddress(hiddenEmailFromAddress.Value) };
            message.To.Add(new MailAddress(hiddenEmailToAddress.Value));

            // I want to be copied on this so I know what's going on
            message.CC.Add(new MailAddress("jbaranauskas@mobileauthcorp.com"));

            message.Subject = pSubject;
            message.IsBodyHtml = true;
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
            return "Sent";
        }
        // ReSharper disable once EmptyGeneralCatchClause
        catch (Exception ex)
        {
            //lbError.Text = "ThreadedSendEmail: SendEmail Failed: " + ex.InnerException;
            hiddenEmailSendError.Value = "true";
            return "SendEmail Failed: " + ex.Message;
        }
    }

    /// <summary> Send ProviderEmail Thread </summary>
    //protected void ThreadedSendEmail(string pSubject, string pBody)
    //{
    //    try
    //    {
    //        //Set up message
    //        var message = new MailMessage { From = new MailAddress(hiddenEmailFromAddress.Value) };
    //        message.To.Add(new MailAddress(hiddenEmailToAddress.Value));

    //        // I want to be copied on this so I know what's going on
    //        message.CC.Add(new MailAddress("jbaranauskas@mobileauthcorp.com"));

    //        message.Subject = pSubject;
    //        message.IsBodyHtml = true;
    //        message.Body = pBody.Replace("|", "<br />");
    //        message.Priority = MailPriority.High;
     
    //        // setup Smtp Client
    //        var smtp = new SmtpClient
    //        {
    //            Port = int.Parse(hiddenEmailPort.Value),
    //            Host = hiddenEmailServer.Value,
    //            EnableSsl = false,
    //            UseDefaultCredentials = true
    //        };
    //        smtp.Credentials = new NetworkCredential(hiddenEmailLoginUserName.Value, hiddenEmailPassword.Value);
    //        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
    //        smtp.Send(message);
    //    }
    //    // ReSharper disable once EmptyGeneralCatchClause
    //    catch (Exception ex)
    //    {
    //        //lbError.Text = "ThreadedSendEmail: SendEmail Failed: " + ex.InnerException;

    //        hiddenEmailSendError.Value = "true";

    //        //lbError.Visible = true;
    //        //divErrorContainer.Visible = true;
    //    }
    //}

    protected string HexToString(String input)
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

    protected void Log(string pEntry)
    {
        var mLogging = ConfigurationManager.AppSettings["Logging"];
        if (!String.IsNullOrEmpty(mLogging))
        {
            if (mLogging.ToLower() == "true")
            {
                const string mLogDir = "c:/temp";
                const string mFile = "golfshoplog.txt";
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