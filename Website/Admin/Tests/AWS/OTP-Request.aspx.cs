using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using MACServices;
using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using cs = MACServices.Constants.Strings;
using sr = MACServices.Constants.ServiceResponse;

namespace Admin.Tests.AWS
{
    public partial class Default : System.Web.UI.Page
    {
        public static string MacServicesUrl = ConfigurationManager.AppSettings[cfg.MacServicesUrl];
        public const string Cmu = "ClientManaged";
        public const string Cru = "Registered";
        public const string TrxType = "1";
        public const string TestEmailServer = "@MacTest.com";

        private const string QS_action = "action";
        private const string QS_cid = "cid";
        private const string QS_groupId = "groupid";
        private const string QS_lastname = "lastname";
        private const string QS_userId = "userId";

        public bool UseStaggeredRequestTimers;
        public int iWaitSecsBeforeRequest;

        public int MinWaitSecs = Convert.ToInt16(ConfigurationManager.AppSettings["MinWaitSecs"]);
        public int MaxWaitSecs = Convert.ToInt16(ConfigurationManager.AppSettings["MaxWaitSecs"]);

        public Utils mUtils = new Utils();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["UseStaggeredRequestTimers"]))
                iWaitSecsBeforeRequest = new Random().Next(MinWaitSecs, MaxWaitSecs);
            else
                iWaitSecsBeforeRequest = 0;

            if (IsPostBack) return;
            lbError.Text = "";
            lbState.Text = @"State:";

            // web config error don't populate dropdown list so this test will not run
            if (string.IsNullOrEmpty(lbError.Text))
            {
                var li0 = new ListItem();
                li0.Text = li0.Value = @"Select a request type";
                ddlType.Items.Add(li0);

                var li1 = new ListItem();
                li1.Text = li1.Value = Cmu;
                ddlType.Items.Add(li1);

                var li2 = new ListItem();
                li2.Text = li2.Value = Cru;
                ddlType.Items.Add(li2);
            }

            // check the config
            var mInitUserClientIdList = bool.Parse(ConfigurationManager.AppSettings[cfg.InitUserClientIdList]);
            if (!mInitUserClientIdList)
            {
                // do not run
                lbError.Text = @"InitUserClientIdList not set to true in web.config";
            }
            var mLoopBackTest = ConfigurationManager.AppSettings[cfg.LoopBackTest];
            if (mLoopBackTest != dv.NoSend)
            {
                // do not run
                lbError.Text += @"<br/>LoopBackTest in web.config must be set to 'NoSend'";
            }
            var mDebug = bool.Parse(ConfigurationManager.AppSettings[cfg.Debug]);
            if (!mDebug)
            {
                // do not run
                lbError.Text +=
                    @"<br/>Debug in web.config must be set to 'true' or RequestOTP will not return the OTP code";
            }
            //var mDebugLogRequests = bool.Parse(ConfigurationManager.AppSettings[cfg.DebugLogRequests]);
            //lbState.Text += @" DebugLogRequests=" + mDebugLogRequests.ToString();

            //var mDebugLogResponses = bool.Parse(ConfigurationManager.AppSettings[cfg.DebugLogResponses]);
            //lbState.Text += @", DebugLogRequests=" + mDebugLogResponses.ToString();

            //var mUseTestAds = bool.Parse(ConfigurationManager.AppSettings[cfg.UseTestAds]);
            //lbState.Text += @", UseTestAds=" + mUseTestAds.ToString();

            // Do need to see this
            lbState.Text = "";

            // Did End-User Register call this test?
            if (!String.IsNullOrEmpty(Request.QueryString[QS_action]))
            {
                var cid = Request.QueryString[QS_cid].ToString();
                if (String.IsNullOrEmpty(cid))
                {
                    lbError.Text += @"<br/>Client Id not sent in Query String, quit!'";
                    return;
                }
                var userid = Request.QueryString[QS_userId].ToString();
                if (String.IsNullOrEmpty(userid))
                {
                    lbError.Text += @"<br/>User Id no sent in Query String, quit!";
                    return;
                }
                var lastname = Request.QueryString[QS_lastname].ToString();
                if (String.IsNullOrEmpty(lastname))
                {
                    lbError.Text += @"<br/>last name no sent in Query String, quit!";
                    return;
                }
                var groupId = Request.QueryString[QS_groupId];

                var myMacotp = new MacOtp.MacOtp();
                var reply = myMacotp.SendOtpToRegisteredUser(
                    ConfigurationManager.AppSettings[cfg.MacServicesUrl],
                    cid, groupId, lastname, userid, TrxType, null, null);
                if (reply.Contains("Error"))
                {
                    lbError.Text = reply;
                    return;
                }

                // Handle in timed event here
                var mRedirect = new StringBuilder();
                mRedirect.Append("OTP-Validation.aspx?cid=" + cid);

                var tmpReply = reply.Split('|');

                foreach (var currentItem in tmpReply)
                {
                    if (currentItem.Contains("GroupId:"))
                    {
                        var tmpVal = currentItem.Split(':');
                        // only put it in the redirect string once
                        if (mRedirect.ToString().ToLower().Contains("groupid=") == false)
                            mRedirect.Append("&groupid=" + tmpVal[1]);
                    }
                    if (currentItem.Contains("RequestId:"))
                    {
                        var tmpVal = currentItem.Split(':');
                        // only put it in the redirect string once
                        if (mRedirect.ToString().ToLower().Contains("requestid=") == false)
                            mRedirect.Append("&requestid=" + tmpVal[1]);
                    }
                    if (currentItem.Contains("OTP:"))
                    {
                        var tmpVal = currentItem.Split(':');
                        // only put it in the redirect string once
                        if (mRedirect.ToString().ToLower().Contains("otp=") == false)
                            mRedirect.Append("&otp=" + tmpVal[1]);
                    }
                }

                // Redirect to Otp Validation with browser timer
                Response.AddHeader("REFRESH", iWaitSecsBeforeRequest.ToString() + ";URL=" + mRedirect.ToString());
                Response.End();
            }
        }

        protected void ddlType_Changed(object sender, EventArgs e)
        {
            if (ddlType.SelectedIndex <= 0) return;
            // Code that runs on application startup builds the User Client List

            if (Application[Constants.Application.Startup.UserClientList] == null)
                Application[Constants.Application.Startup.UserClientList] = mUtils.GetUsersAndClientIds();

            if (Application[Constants.Application.Startup.UserClientList] != null)
            {
                var rnd = new Random();

                var mType = ddlType.SelectedItem.Text;

                var uc = Application[Constants.Application.Startup.UserClientList].ToString();
                var usersandclientids = uc.Split(char.Parse(Constants.Dictionary.Keys.ItemSep));

                var count = usersandclientids.Count();
                var num = rnd.Next(0, count - 1);

                var mUser = usersandclientids[num];

                lbSelectedUser.Text = mUser;

                var userparms = mUser.Split(':');
                var mData = new Dictionary<string, string>();
                foreach (string userpram in userparms)
                {
                   var kv = userpram.Split('=');
                    if (kv[0] == "EmailAddress")
                    {
                        if (kv[1].Contains("@") == false)
                        {
                            mData.Add(kv[0], kv[1] + TestEmailServer);
                        }
                        else
                        {
                            mData.Add(kv[0], kv[1]);
                        }
                    }
                    else
                    {
                        mData.Add(kv[0], kv[1]);
                    }
                }

                try
                {
                    var myMacotp = new MacOtp.MacOtp();
                    var reply = mType != Cru 
                        ? 
                        myMacotp.SendOtpToClientUser(MacServicesUrl, // client managed
                        mData["CID"],           // clientId
                        mData["EmailAddress"],  // End User's email address
                        mData["PhoneNumber"],   // End User's phone number
                        TrxType, null, null)          // Transaction Type & transaction Details
                        : 
                        myMacotp.SendOtpToRegisteredUser(MacServicesUrl, // Registered
                        mData["CID"],           // clientId
                        null,                   // groupId
                        mData["LastName"],      // End User's last name
                        mData["EmailAddress"],  // End User's email address as Unique identifier 
                        TrxType, null, null);         // Transaction Type & transaction Details
                    if (reply.Contains("Error"))
                    {
                        lbError.Text = reply;
                        return;
                    }
                    var mRedirect = new StringBuilder();
                    mRedirect.Append("OTP-Validation.aspx?cid=" + mData["CID"]);

                    var tmpReply = reply.Split('|');
                    foreach (var currentItem in tmpReply)
                    {
                        if (currentItem.Contains("GroupId:"))
                        {
                            var tmpVal = currentItem.Split(':');
                            // only put it in the redirect string once
                            if (mRedirect.ToString().ToLower().Contains("groupid=") == false)
                                mRedirect.Append("&groupid=" + tmpVal[1]);
                        }
                        if (currentItem.Contains("RequestId:"))
                        {
                            var tmpVal = currentItem.Split(':');
                            // only put it in the redirect string once
                            if (mRedirect.ToString().ToLower().Contains("requestid=") == false)
                                mRedirect.Append("&requestid=" + tmpVal[1]);
                        }
                        if (currentItem.Contains("OTP:"))
                        {
                            var tmpVal = currentItem.Split(':');
                            // only put it in the redirect string once
                            if (mRedirect.ToString().ToLower().Contains("otp=") == false)
                                mRedirect.Append("&otp=" + tmpVal[1]);
                        }
                    }
                    // Redirect to Otp Validation
                    Response.Redirect(mRedirect.ToString(), true);
                    Response.End();
                }
                catch (Exception ex)
                {
                    lbError.Text = ex.Message;
                }
            }
            else
            {
                Response.Write("Please select a request type!");
            }
        }
    }
}