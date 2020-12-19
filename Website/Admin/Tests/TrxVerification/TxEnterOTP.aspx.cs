using System;
using System.Configuration;
using System.Xml;

using MACServices;
using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using sr = MACServices.Constants.ServiceResponse;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;

namespace MACUserApps.Web.Tests.TrxVerification
{
    public partial class MacUserAppsWebTestsTrxVerificationTxEnterOtp : System.Web.UI.Page
    {
        private const string Test = "TxEnterOTP";
        readonly Utils mUtils = new Utils();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lbCID.Text = Request.QueryString[dk.CID];
                lbGID.Text = Request.QueryString[dk.GroupId];
                lbRID.Text = Request.QueryString[sr.RequestId];
                tbOtp.Text = Request.QueryString[sr.OTP];
                AdDiv.InnerHtml = mUtils.HexToString(Request.QueryString[sr.EnterOTPAd]);
                //AdDivConAd.InnerHtml = mUtils.HexToString(Request.QueryString[sr.ContentAd]);
                AddToLogAndDisplay(Test + ":" +
                    dk.ItemSep + dk.CID + dk.KVSep + lbCID.Text +
                    dk.ItemSep + dk.GroupId + dk.KVSep + lbGID.Text +
                    dk.ItemSep + dk.RequestId + dk.KVSep + lbRID.Text
                    );
            }
        }

        public void btnSubmit_Click(object sender, EventArgs e)
        {
            lbError.Text = "";
            var myOtp = tbOtp.Text.Trim();
            if (String.IsNullOrEmpty(myOtp))
            {
                lbError.Text = @"You must enter an Otp";
                return;
            }
            AddToLogAndDisplay("btnSubmit: Otp Submited:" + lbRID.Text + "/" + myOtp);
            try
            {

                var myMacotp = new MacOtp.MacOtp();
                var sReply = myMacotp.VerifyOtp(
                    ConfigurationManager.AppSettings[cfg.MacServicesUrl],
                    lbCID.Text, lbRID.Text, myOtp);
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
                            Response.Redirect("txDone.aspx", false);
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
                        Response.Redirect("txDone.aspx", false);
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
            lbError.Text = "";
            AddToLogAndDisplay("btnResend: Resend Otp requested");
            try
            {
                var myMacotp = new MacOtp.MacOtp();
                var sReply = myMacotp.ResendOtp(
                    ConfigurationManager.AppSettings[cfg.MacServicesUrl],
                    lbCID.Text, lbRID.Text);

                AddToLogAndDisplay(sReply);
            }
            catch (Exception ex)
            {
                AddToLogAndDisplay("btnResend Exception:" + ex);
                lbError.Text = ex.Message;
            }
        }

        protected void btnClearLog_Click(object sender, EventArgs e)
        {
            Session["LogText"] = "";
            AddToLogAndDisplay("btnClearLog");
        }
        private void AddToLogAndDisplay(string textToAdd)
        {
            var newlog = String.Format("{0}|{1}.{2}", Session["LogText"], Test, textToAdd);
            Session["LogText"] = newlog;
            tbLog.Text = newlog.Replace("|", Environment.NewLine);
        }
    }
}