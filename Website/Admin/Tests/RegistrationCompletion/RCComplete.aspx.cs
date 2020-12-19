using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using dkui = MACServices.Constants.Dictionary.Userinfo;

namespace MACUserApps.Web.Tests.RegistrationCompletion
{
    public partial class RcComplete : System.Web.UI.Page
    {
        protected const string LogText = "LogText";
        protected const string Test = "RCComplete";
        HiddenField _hiddenO;
        HiddenField _hiddenQ;
        HiddenField _hiddenR;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.Master != null)
            {
                _hiddenO = (HiddenField)Page.Master.FindControl("hiddenO");
                _hiddenQ = (HiddenField)Page.Master.FindControl("hiddenQ");
                _hiddenR = (HiddenField)Page.Master.FindControl("hiddenR");
            }
            try
            {
                var data = Request.QueryString["id"];
                if (String.IsNullOrEmpty(data))
                {

                    lbError.Text = @"No request data!";
                    return;
                }
                var mUtils = new MACServices.Utils();
                var myData = new Dictionary<string, string>();
                var requestData = mUtils.HexToString(data);
                // parse string(data) and add to the dictionary
                if (mUtils.ParseIntoDictionary(requestData, myData, char.Parse(dk.KVSep)) == false)
                {
                    lbError.Text = @"Invalid request data!";
                    return;
                }

                _hiddenQ.Value = myData[dkui.FirstName];
                _hiddenR.Value = myData[dkui.LastName];
                _hiddenO.Value = myData[dk.ClientName];

                lbUserName.Text = _hiddenQ.Value + @" " + _hiddenR.Value;
                lbClientName1.Text = lbClientName.Text = _hiddenO.Value;
                AddToLogAndDisplay(dk.ClientName + ":" + _hiddenO.Value);
                AddToLogAndDisplay(dkui.FirstName + ":" + _hiddenQ.Value);
                AddToLogAndDisplay(dkui.LastName + ":" + _hiddenR.Value);
                AddToLogAndDisplay("CalledBy" + ":" + myData["Caller"]);
            }
            catch (Exception ex)
            {
                AddToLogAndDisplay(ex.ToString());
            }
        }

        protected void lnkbtnExit_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Default.aspx");
        }

        private void AddToLogAndDisplay(string textToAdd)
        {
            var newlog = String.Format("{0}|{1}.{2}", Session[LogText], Test, textToAdd);
            Session[LogText] = newlog;
            tbLog.Text = newlog.Replace("|", Environment.NewLine);
        }


    }
}