using System;
//using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Web;
using System.Web.UI;
using System.IO;

using dk = MACServices.Constants.Dictionary.Keys;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using dv = MACServices.Constants.Dictionary.Values;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using sr = MACServices.Constants.ServiceResponse;

using tc = TestLib.TestConstants.TestBank;
/*
 * expecting a request query string of:
 * http://..../DemoRegistration.ascx?demo=MACDemox&cid=1234567890123456789
 * where:
 * demo value is the name of the calling demo (MACDemo1,MACDemo2,MACDemo3,etc)
 * cid value is the client Id assigned to the client assigned/configured in the OTP system for the calling demo
 * */

namespace OtpApDemos
{

    public partial class DemoRegistration : System.Web.UI.Page
    {
        private const string QS_demo = "demo";
        private const string QS_cid = "cid";
        private const string QS_action = "action";
        private const string QS_config = "cfg";
        private const string QS_reg = "reg";

        private const string demoConfig = "demoRegConfig.txt";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            //var qs = Request.QueryString;
            hiddenT.Value = Request.QueryString[QS_action];
            if (String.IsNullOrEmpty(hiddenT.Value))
                hiddenT.Value = String.Empty;

            hiddenMACBankUrl.Value = string.Empty;
            string mpath = HttpContext.Current.Server.MapPath(".");
            string mfile = Path.Combine(mpath, demoConfig);
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
                    if (line.StartsWith("macbankUrl"))
                        txtMacBankUrl.Text = hiddenMACBankUrl.Value = line.Replace("macbankUrl:", "");
                    if (line.StartsWith("RegistrationType"))
                        txtRegType.Text = hiddenRegistrationType.Value = line.Replace("RegistrationType:", "");
                }
                file.Close();
            }
            if (hiddenT.Value == QS_config)
            {
                divRegForm.Visible = false;
                divCfgForm.Visible = true;
            }
            else if (hiddenT.Value == QS_reg)
            {
                divCfgForm.Visible = false;
                divRegForm.Visible = true;
                hiddenDemo.Value = Request.QueryString[QS_demo];
                if (String.IsNullOrEmpty(hiddenDemo.Value))
                {
                    lbError.Text += @"No demo name in request";
                    btnRegisterEndUser.Enabled = false;
                }
                hiddenCID.Value = Request.QueryString[QS_cid];
                if (String.IsNullOrEmpty(hiddenCID.Value))
                {
                    lbError.Text += @"No cid in request";
                    btnRegisterEndUser.Enabled = false;
                }
            }
            else
            {
                divCfgForm.Visible = false;
                divRegForm.Visible = false;
            }
            // Caller
            if (Request.UrlReferrer != null)
            {
                hiddenCallerURL.Value = Request.UrlReferrer.ToString();
            }
        }

        //public void btnTest_Click(object sender, EventArgs e)
        //{
        // verify captcha
        //if (String.IsNullOrEmpty(myId.InnerHtml))
        //{
        //    lbError.Text += @"Please select the checkbox to verify that you are human.";
        //    return;
        //}
        //if (String.IsNullOrEmpty(myText.Value))
        //{
        //    lbError.Text += @"Cool.";
        //    return;
        //}
        //else
        //{
        //    lbError.Text += @"You are a bot. This form will not be submitted";
        //    return;
        //}
        //if (String.IsNullOrEmpty(myText.Value))
        //{
        //    var hiddenCaptchaTextField = false;
        //}
        //else
        //{
        //    hiddenCaptchaTextField = true;
        //}
        //lbError.Text += myText.Value.ToString() + "hello";
        //}

        /// <summary> Register from data on form </summary>
        public void btnRegisterEndUser_Click(object sender, EventArgs e)
        {
            lbError.Text = String.Empty;
            if (String.IsNullOrEmpty(hiddenMACBankUrl.Value))
            {
                lbError.Text += @"No MAC Bank Url configured!";
                return;
            }

            if (String.IsNullOrEmpty(hiddenRegistrationType.Value))
            {
                lbError.Text += @"No Registration type configured!";
                return;
            }
            string regtype = dv.OpenRegister;
            if (hiddenRegistrationType.Value.ToLower().Contains("client"))
                regtype = dv.ClientRegister;
            else if (hiddenRegistrationType.Value.ToLower().Contains("group"))
                regtype = "GroupRegister";

            if (String.IsNullOrEmpty(hiddenCID.Value))
            {
                lbError.Text += @"No Client id!";
                return;
            }

            // security check leave blank field
            //if (String.IsNullOrEmpty(txtZipcode.Text) == false)
            //    return;
            if (String.IsNullOrEmpty(txtFirstName.Text))
                lbError.Text += @"First Name, ";
            if (String.IsNullOrEmpty(txtLastName.Text))
                lbError.Text += @"Last Name, ";
            if (String.IsNullOrEmpty(txtEmailAdr.Text))
                lbError.Text += @"Email Address, ";
            if (String.IsNullOrEmpty(txtMPhoneNo.Text))
                lbError.Text += @"Mobile Phone, ";
            if (String.IsNullOrEmpty(lbError.Text) == false)
            {
                lbError.Text += @"Required!";
                return;
            }
            string mRequest = dk.Request + dk.KVSep + tc.AssignAndReg +
                            dk.ItemSep + dk.CID + dk.KVSep + hiddenCID.Value +
                            dk.ItemSep + dkui.FirstName + dk.KVSep + txtFirstName.Text +
                            dk.ItemSep + dkui.LastName + dk.KVSep + txtLastName.Text +
                            dk.ItemSep + dkui.PhoneNumber + dk.KVSep + txtMPhoneNo.Text +
                            dk.ItemSep + dkui.EmailAddress + dk.KVSep + txtEmailAdr.Text +
                            dk.ItemSep + tc.Type + dk.KVSep + tc.User +
                            dk.ItemSep + dk.RegistrationType + dk.KVSep + regtype +
                            dk.ItemSep + dk.NotificationOption + dk.KVSep + dv.Email;

            if (SendRequestToMacTestBankServer(hiddenMACBankUrl.Value, hiddenCID.Value, mRequest))
            {  // successful

                // If the browser is not connected
                // stop all response processing.
                if (!Response.IsClientConnected)
                    Response.End();
                Response.Redirect(hiddenCallerURL.Value + "?action=registered", false);
                Response.End();
            }
            // verify captcha
            //if (String.IsNullOrEmpty(myId.InnerHtml))
            //{
            //    lbError.Text += @"Please select the checkbox to verify that you are human.";
            //    return;
            //}
            //if (String.IsNullOrEmpty(myText.Value))
            //{
            //    lbError.Text += @"Cool.";
            //    return;
            //}
            //else
            //{
            //    lbError.Text += @"You are a bot. This form will not be submitted";
            //    return;
            //}
        }

        public void btnTestNavBack_Click(object sender, EventArgs e)
        {
            if (!Response.IsClientConnected)
                Response.End();
            Response.Redirect(hiddenCallerURL.Value + "?action=abort", false);
            Response.End();
        }

        public void btnSave_Click(object sender, EventArgs e)
        {
            string mpath = HttpContext.Current.Server.MapPath(".");
            string mfile = Path.Combine(mpath, demoConfig);

            File.WriteAllText(mfile,
                    @"macbankUrl:" + txtMacBankUrl.Text +
                    Environment.NewLine +
                    @"RegistrationType:" + txtRegType.Text
                );

            // display saved message
            ScriptManager.RegisterStartupScript(Page, Page.GetType(),
              "err_msg",
              "alert('Your settings have been saved.');",
              true);
        }

        private bool SendRequestToMacTestBankServer(string MACBankUrl, string pCID, string requestData)
        {
            try
            {
                var dataStream = Encoding.UTF8.GetBytes("data=99" + pCID.Length + pCID.ToUpper() + StringToHex(requestData));
                var webRequest = WebRequest.Create(MACBankUrl);
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
                    return false;
                }
                elemList = xmlDoc.GetElementsByTagName(sr.Reply);
                if (elemList.Count != 0)
                    lbError.Text = String.Format(elemList[0].InnerXml);

                return true;
            }
            catch (Exception ex)
            {
                lbError.Text = ex.Message;
                return false;
            }
        }


        #region Helpers

        public static string StringToHex(String input)
        {
            try
            {
                var values = input.ToCharArray();
                var output = new StringBuilder();
                //foreach (var value in values.Select(Convert.ToInt32))
                //{
                //    // Convert the decimal value to a hexadecimal value in string form. 
                //    output.Append(String.Format("{0:X}", value));
                //}
                return output.ToString();
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
