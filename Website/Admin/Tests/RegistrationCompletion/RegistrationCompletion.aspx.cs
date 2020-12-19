using System;
using System.Configuration;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using MACServices;
using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using sr = MACServices.Constants.ServiceResponse;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;

namespace MACUserApps.Web.Tests.RegistrationCompletion
{
    public partial class MacUserAppsWebTestsRegistrationCompletionRegistrationCompletion : System.Web.UI.Page
    {
        protected const string LogText = "LogText";
        protected const string Test = "RegComp";

        HiddenField _hiddenD;
        HiddenField _hiddenP;
        HiddenField _hiddenQ;
        HiddenField _hiddenR;
        HiddenField _hiddenN;
        HiddenField _hiddenS;
        HiddenField _hiddenO;

        // This page gets loaded from the registration email
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.Master != null)
            {
                _hiddenD = (HiddenField)Page.Master.FindControl("hiddenD");
                _hiddenN = (HiddenField)Page.Master.FindControl("hiddenN");
                _hiddenP = (HiddenField)Page.Master.FindControl("hiddenP");
                _hiddenQ = (HiddenField)Page.Master.FindControl("hiddenQ");
                _hiddenR = (HiddenField)Page.Master.FindControl("hiddenR");
                _hiddenS = (HiddenField)Page.Master.FindControl("hiddenS");
                _hiddenO = (HiddenField)Page.Master.FindControl("hiddenO");
            }

            if (IsPostBack) return;
            try
            {
                var mUtils = new Utils();

                var data = Request.QueryString["id"];
                if (String.IsNullOrEmpty(data))
                {

                    lbError.Text = @"No request data!";
                    return;
                }
                Session[LogText] = "";

                var myData = new Dictionary<string, string>();
                if (data.StartsWith("99"))
                {  // hex encoded
                    var requestData = data.Substring(2, data.Length - 2); // dump the 99 from front
                    // isloate ID from data
                    var request = mUtils.GetIdDataFromRequest(requestData);
                    // parse string(data) and add to the dictionary
                    if (mUtils.ParseIntoDictionary(
                        mUtils.HexToString(request.Item2), myData, char.Parse(dk.KVSep)) == false)
                    {
                        lbError.Text = @"Corrupt or bad request data!";
                        return;
                    }
                }
                else
                {   // encrypted request data
                    var request = mUtils.GetIdDataFromRequest(data);
                    if (String.IsNullOrEmpty(request.Item1))
                    {
                        lbError.Text = @"Corrupt or bad request data!";
                        return;
                    }

                    // decrypt, parse string and add to the dictionary
                    if (mUtils.DecryptAndParseRequestData(request.Item1, request.Item2, myData, char.Parse(dk.KVSep)) == false)
                    {
                        lbError.Text = @"Corrupt or bad request data!";
                        return;
                    }
                }
                _hiddenD.Value = myData[dk.CID];
                _hiddenN.Value = myData[dk.RegistrationType];
                _hiddenP.Value = myData[dk.UserId];
                _hiddenQ.Value = myData[dkui.FirstName];
                _hiddenR.Value = myData[dkui.LastName];
                _hiddenO.Value = myData[dk.ClientName];
                _hiddenS.Value = "";

                AddToLogAndDisplay(dk.RegistrationType + ":" + _hiddenN.Value);
                AddToLogAndDisplay(dk.ClientName + ":" + _hiddenO.Value);
                AddToLogAndDisplay(dk.UserId + ":" + _hiddenP.Value);
                AddToLogAndDisplay(dkui.FirstName + ":" + _hiddenQ.Value);
                AddToLogAndDisplay(dkui.LastName + ":" + _hiddenR.Value);

                lbUserName.Text = _hiddenQ.Value;
                lbUserName.Text += @" " + _hiddenR.Value;
                lbClientName.Text = lbClientName1.Text = _hiddenO.Value + @"'s";
                var mReg = new MacRegistration.MacRegistration();
                var reply = mReg.CompleteRegistration(
                    ConfigurationManager.AppSettings[cfg.MacServicesUrl], 
                    dv.RequestRegistrationOtp, 
                    _hiddenP.Value,
                    _hiddenD.Value, 
                    _hiddenN.Value, 
                    null, 
                    null);

                AddToLogAndDisplay(reply);
                if (reply.StartsWith(sr.Error))
                {
                    lbError.Text = reply;
                    lbError.Visible = true;
                }
                if (!reply.Contains(sr.RequestId))
                {
                    lbError.Text = reply;
                    lbError.Visible = true;
                }
                else
                {
                    _hiddenS.Value = String.Empty;
                    var mReplys = reply.Split(char.Parse(dk.ItemSep));
                    foreach (var item in mReplys)
                    {
                        if (item.StartsWith(sr.RequestId))
                        {
                            _hiddenS.Value = item.Replace(sr.RequestId + "=", "");
                        }
                        else if (item.StartsWith(sr.Debug))
                        {
                            txtOtp.Text = item.Replace(sr.Debug + "=" + sr.OTP + dk.KVSep, "");
                        }
                        
                    }
                    if (String.IsNullOrEmpty(_hiddenS.Value))
                    {
                        lbError.Text = @"Did not get " + sr.RequestId + @" in response!";
                        lbError.Visible = true;
                    }

                }
            }
            catch (Exception ex) {
                AddToLogAndDisplay("Exception:" + ex.Message);
            }
        }

        protected void btnSubmit_Clicked(object sender, EventArgs e)
        {
            var mUtils = new Utils();
            AddToLogAndDisplay("OTP value:" + txtOtp.Text.Trim());
            if ((txtOtp.Text.Trim().Contains("Enter") || String.IsNullOrWhiteSpace(txtOtp.Text.Trim())))
            {
                lbError.Text = @"You must enter a Valid OTP!";
                lbError.Visible = true;
                return;
            }
            var mReg = new MacRegistration.MacRegistration();
            var mReply = mReg.CompleteRegistration(
                ConfigurationManager.AppSettings[cfg.MacServicesUrl],
                dv.VerifyOtp,
                _hiddenP.Value,
                _hiddenD.Value,
                _hiddenN.Value,
                _hiddenS.Value,
                txtOtp.Text.Trim());

            AddToLogAndDisplay(mReply);

            var rtn = mReply.Split(char.Parse(dk.ItemSep));
            if (rtn[0].Contains(sr.Validated))
            {
                Response.Redirect("RCComplete.aspx?Id=" +
                        mUtils.StringToHex(dk.ClientName + dk.KVSep + _hiddenO.Value +
                               dk.ItemSep + dkui.FirstName + dk.KVSep + _hiddenQ.Value +
                               dk.ItemSep + dkui.LastName + dk.KVSep + _hiddenR.Value +
                               dk.ItemSep + "Caller" + dk.KVSep + "RegistrationCompletion"));
            }
            else
            {
                AddToLogAndDisplay(mReply);
                lbError.Text = rtn[0].Replace("<" + sr.Action + ">", "").Replace("</" + sr.Action + ">", "");

            }
        }

        protected void btnResend_Clicked(object sender, EventArgs e)
        {
            var mReg = new MacRegistration.MacRegistration();
            var reply = mReg.CompleteRegistration(
                ConfigurationManager.AppSettings[cfg.MacServicesUrl], 
                dv.ResendOtp, _hiddenP.Value,
                _hiddenD.Value, 
                _hiddenN.Value, 
                _hiddenS.Value, 
                null);

            AddToLogAndDisplay(reply);
        }

        protected void btnCancel_Clicked(object sender, EventArgs e)
        {
            var mReg = new MacRegistration.MacRegistration();
            var reply = mReg.CompleteRegistration(
                ConfigurationManager.AppSettings[cfg.MacServicesUrl], 
                dv.CancelRegistration, 
                _hiddenP.Value,
                _hiddenD.Value, 
                _hiddenN.Value, 
                _hiddenS.Value, 
                null);

            AddToLogAndDisplay(reply);
        }
        
        private void AddToLogAndDisplay(string textToAdd)
        {
            var newlog = String.Format("{0}|{1}.{2}", Session[LogText], Test, textToAdd);
            Session[LogText] = newlog;
            tbLog.Text = newlog.Replace("|", Environment.NewLine);
        }
    }
}