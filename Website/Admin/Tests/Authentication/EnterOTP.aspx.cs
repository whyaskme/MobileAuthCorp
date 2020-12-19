using System;
using System.Configuration;
using System.Web.UI.WebControls;
using System.Net;
using System.Text;
using System.Xml;
using System.Linq;

using MACServices;
using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using sr = MACServices.Constants.ServiceResponse;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using str = MACServices.Constants.Strings;

namespace MACUserApps.Web.Tests.Authentication
{
    public partial class MacUserAppsWebTestsAuthenticationEnterOtp : System.Web.UI.Page
    {
        private const string Test = "Auth.EnterOtp";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lbCID.Text = Request.QueryString[dk.CID];
                lbGID.Text = Request.QueryString[dk.GroupId];
                lbRID.Text = Request.QueryString[sr.RequestId];
                tbOtp.Text = Request.QueryString[sr.OTP];
                var mUtils = new Utils();
                // stuff the Ad into the page
                AdDiv.InnerHtml = mUtils.HexToString(Request.QueryString[sr.EnterOTPAd]);
                //AdDiv.InnerHtml = mUtils.HexToString(Request.QueryString[sr.ContentAd]);
                AddToLogAndDisplay(Test + ":" +
                    dk.ItemSep + dk.CID + dk.KVSep + lbCID.Text +
                    dk.ItemSep + dk.GroupId + dk.KVSep + lbGID.Text +
                    dk.ItemSep + dk.RequestId + dk.KVSep + lbRID.Text
                    );
            }
        }

        public void btnSubmitAsReply_Click(object sender, EventArgs e)
        {
            lbError.Text = "";
            var clickedButton = sender as Button;
            if (clickedButton == null) return;
            var mRID = lbRID.Text;
            // test invalid 
            switch (clickedButton.ID)
            {
                case @"btnReplyBadID":
                    mRID = str.DefaultClientId;
                    break;
            }
            var myOtp = tbOtp.Text.Trim();
            if (String.IsNullOrEmpty(myOtp))
            {
                lbError.Text = @"You must enter an OTP";
                return;
            }
            // submit to message reply service
            var mRequest =  dk.Request + dk.KVSep + dv.VerifyOtp +
                            dk.ItemSep + dk.RequestId + dk.KVSep + mRID +
                            dk.ItemSep + dk.OTP + dk.KVSep + myOtp;
            try
            {
                var dataStream = Encoding.UTF8.GetBytes("data=" + StringToHex(mRequest));
                var request = ConfigurationManager.AppSettings[cfg.MacServicesUrl] + Constants.ServiceUrls.MessageBroadcastReplyService;
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
                if (response != null)
                {
                    xmlDoc.Load(response);
                    //error
                    var elemList = xmlDoc.GetElementsByTagName(sr.Error);
                    if (elemList.Count != 0)
                    {
                        AddToLogAndDisplay("Error: " + elemList[0].InnerXml);
                        lbError.Text = elemList[0].InnerXml;
                        return;
                    }

                    elemList = xmlDoc.GetElementsByTagName(sr.Debug);
                    if (elemList.Count != 0)
                        AddToLogAndDisplay("Debug items: " + elemList[0].InnerXml);

                    elemList = xmlDoc.GetElementsByTagName(sr.Reply);
                    if (elemList.Count != 0)
                    {
                        AddToLogAndDisplay("Error: " + elemList[0].InnerXml);
                        Response.Redirect("Done.aspx", false);
                    }
                }
                AddToLogAndDisplay("Error: null response!");
            }
            catch (Exception ex)
            {
                AddToLogAndDisplay("Error: " + ex.Message);
            }
        }

        public void btnSubmit_Click(object sender, EventArgs e)
        {
            var clickedButton = sender as Button;
            if (clickedButton == null) return;
            var mCID = lbCID.Text;
            var mRID = lbRID.Text;
            // test invalid 
            switch (clickedButton.ID)
            {
                case @"btnSubmitBadRID":
                    mRID = str.DefaultClientId;
                    break;
                case @"btnSubmitBadCID":
                    mCID = mRID;
                    break;
                case @"btnSubmitWrongCID":
                    mCID = str.DefaultClientId;
                    break;
            }
            var myOtp = tbOtp.Text.Trim();
            if (String.IsNullOrEmpty(myOtp))
            {
                lbError.Text = @"You must enter an OTP";
                return;
            }
            lbError.Text = "";
            Session[sr.OTP] = myOtp;

            AddToLogAndDisplay("--- " + clickedButton.ID + " ---|CID:" + mCID + 
                ", RID:"+ mRID + 
                ", OTP:" + myOtp);
            try
            {
                var myMacotp = new MacOtp.MacOtp();
                var sReply = myMacotp.VerifyOtp(
                    ConfigurationManager.AppSettings[cfg.MacServicesUrl], mCID, mRID, myOtp);
                if (cbXML.Checked)
                {
                    AddToLogAndDisplay(sReply.Replace("><", ">|<"));
                    var xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(sReply);
                    var elemList = xmlDoc.GetElementsByTagName(sr.Reply);
                    if (elemList.Count != 0)
                    {
                        if (elemList[0].InnerXml == sr.Validated)
                        {
                            Response.Redirect("Done.aspx", false);
                        }
                        else
                        {
                            lbError.Text = elemList[0].InnerXml;
                            return;
                        }
                    }
                }
                else
                {
                    AddToLogAndDisplay(sReply);
                    if (sReply.Contains(sr.Validated))
                        Response.Redirect("Done.aspx", false);
                }

                lbError.Text = sReply;
            }
            catch (Exception ex)
            {
                AddToLogAndDisplay("btnSubmit Exception:" + ex);
                lbError.Text = ex.Message;
            }
        }

        public void btnResend_Click(object sender, EventArgs e)
        {
            var clickedButton = sender as Button;
            if (clickedButton == null) return;
            AddToLogAndDisplay(clickedButton.ID);
            var mCID = lbCID.Text;
            var mRID = lbRID.Text;
            // test invalid 
            if (clickedButton.ID == @"btnResendBadRID") mRID = str.DefaultClientId;
            else if (clickedButton.ID == @"btnResendBadCID") mCID = mRID;
            else if (clickedButton.ID == @"btnResendWrongCID") mCID = str.DefaultClientId;
            lbError.Text = "";
            AddToLogAndDisplay("--- " + clickedButton.ID + " ---|CID:" + mCID + ", RID:" + mRID);
            try
            {
                var myMacotp = new MacOtp.MacOtp();
                var sReply = myMacotp.ResendOtp(ConfigurationManager.AppSettings[cfg.MacServicesUrl], mCID, mRID);
                if (sReply.Contains("Error"))
                {
                    lbError.Text = sReply;
                }
                AddToLogAndDisplay("Reply:" + sReply);
            }
            catch (Exception ex)
            {
                lbError.Text = @"Exception";
                AddToLogAndDisplay(lbError.Text + "|" + ex);
            } 
        }

        public void btnRestart_Click(object sender, EventArgs e)
        {
            Session["LogText"] = "";
            Session[dk.CID] = "";
            Session[sr.RequestId] = "";
            Session[sr.OTP] = "";
            AddToLogAndDisplay("Restart");
            Response.Redirect("Auth.aspx", false);
        }
        protected void btnClearLog_Click(object sender, EventArgs e)
        {
            Session["LogText"] = "";
            AddToLogAndDisplay("btnClearLog");
        }
        private void AddToLogAndDisplay(string textToAdd)
        {
            var newlog = String.Format("{0}|{1} - {2}", Session["LogText"], Test, textToAdd);
            Session["LogText"] = newlog;
            tbLog.Text = newlog.Replace("|", Environment.NewLine);
        }

        public void btnExit_Click(object sender, EventArgs e)
        {
            Session[dk.CID] = "";
            Session[sr.RequestId] = "";
            Session[sr.OTP] = "";
            Response.Redirect("../Default.aspx");
        }

        protected static string StringToHex(String input)
        {
            try
            {
                var values = input.ToCharArray();
                var output = new StringBuilder();
                foreach (int value in values.Select(Convert.ToInt32))
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
}