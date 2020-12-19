using System;
using System.Configuration;
using System.Net;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;

using MACServices;
using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using sr = MACServices.Constants.ServiceResponse;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;

public partial class MACUserApps_Web_Tests_IssueRequest_IssueRequest : System.Web.UI.Page
{
    HiddenField _hiddenT;
    private const string Test = "IssueRequest";
    private const string SelectService = "Select Service";
    private const string OTPService = "Otp";
    private const string OTPServiceNames = SelectService +
                                           dk.ItemSep + "RequestOtp" +
                                           dk.ItemSep + "ValidateOtp" +
                                           dk.ItemSep + "OpenClientServices" +
                                           dk.ItemSep + "OpenEndUserServices";

    private const string OASService = "OAS";
    private const string OASServiceNames = SelectService +
                                       dk.ItemSep + "OpenClientServices" +
                                       dk.ItemSep + "OpenEndUserServices";

    private const string UserService = "User";
    private const string UserServiceNames = SelectService +
                                       dk.ItemSep + "EndUserManagement" +
                                       dk.ItemSep + "EndUserRegistration" +
                                       dk.ItemSep + "StsEndUserRegistration" +
                                       dk.ItemSep + "EndUserFileRegistration";

    private const string AdminService = "AdminServices";
    private const string AdminServiceNames = SelectService +
                                    dk.ItemSep + "ClientServices" +
                                    dk.ItemSep + "GetGroupInfo" +
                                    dk.ItemSep + "ManageTypeDefsService" +
                                    dk.ItemSep + "SystemStats" +
                                    dk.ItemSep + "UsageBilling";

    private const string EventService = "Event";
    private const string EventServiceNames = SelectService +
                                   dk.ItemSep + "EventHistory" +
                                   dk.ItemSep + "EventHistory1" +
                                   dk.ItemSep + "EventHistoryStats";

    private const string TestService = "Test";
    private const string TestServiceNames = SelectService +
                               dk.ItemSep + "GetClients" +
                               dk.ItemSep + "GetTestLoginInfo" +
                               dk.ItemSep + "MacTestBank" +
                               dk.ItemSep + "RegisterClients" +
                               dk.ItemSep + "RegisterProviders";

    HiddenField _hiddenW;

    private Utils mUtils = new Utils();
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Page.Master != null)
        {
            _hiddenT = (HiddenField) Page.Master.FindControl("hiddenT");

            _hiddenW = (HiddenField)Master.FindControl("hiddenW");
            _hiddenW.Value = "54a83c00ead6362034d04bd0";
        }

        if (IsPostBack) return;
        Populate_ddlServiceNames();
        _hiddenT.Value = "";
    }

    protected void btnHash_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtLastName.Text))
        {
            lbError.Text = @"Last Name Required!";
            return;
        }
        if (string.IsNullOrEmpty(txtUID.Text))
        {
            lbError.Text = @"Unique Identifier Required!";
            return;
        }
        var userid = MACSecurity.Security.GetHashString(
            txtLastName.Text.ToLower() + txtUID.Text.ToLower());
        txtUserid.Text = userid.ToUpper();
        AddToLogAndDisplay(txtUserid.Text);
    }

    protected void btnClearText_Click(object sender, EventArgs e)
    {
        AddToLogAndDisplay("btnClearText_Click [" + _hiddenT.Value.Replace(dk.ItemSep, ", ") + "]");
        if (_hiddenT.Value.Contains("Select Service"))
        {
            lbError.Text = @"Select a service";
            return;
        }
        var serviceUrl = GetServiceUrl();
        if (String.IsNullOrEmpty(txtRequest.Text))
        {
            lbError.Text = @"Reauest Data required!";
            return; 
        }
        if (txtRequest.Text.StartsWith(dk.Request) == false)
        {
            lbError.Text = @"Reauest Data invalid!";
            return; 
        }
        var data = "data=99" +
                   Constants.Strings.DefaultClientId.Length.ToString() +
                   Constants.Strings.DefaultClientId.ToUpper() +
                   mUtils.StringToHex(txtRequest.Text);
        IssueRequest(serviceUrl, data);
    }

    protected void btnEncodedEncrypted_Click(object sender, EventArgs e)
    {
        AddToLogAndDisplay("btnEncodedEncrypted [" + _hiddenT.Value.Replace(dk.ItemSep, ", ") + "]");
        if (_hiddenT.Value.Contains("Select Service"))
        {
            lbError.Text = @"Select a service";
            return;
        }
        var serviceUrl = GetServiceUrl();
        if (String.IsNullOrEmpty(txtRequest.Text))
        {
            lbError.Text = @"Reauest Data required!";
            return;
        }
        if (txtRequest.Text.StartsWith(dk.Request))
        {
            lbError.Text = @"Reauest Data invalid!";
            return;
        }
        var data = txtRequest.Text.Trim();
        if (data.StartsWith("99"))
        { // hex encoded
            var requestData = data.Substring(2, data.Length - 2); // dump the 99 from front

            // isloate key from data
            var request = mUtils.GetIdDataFromRequest(requestData);

            // parse string(data) and add to the dictionary
            AddToLogAndDisplay(mUtils.HexToString(request.Item2));
        }
        else
        {   // encrypted request data
            var request = mUtils.GetIdDataFromRequest(data);
            if (String.IsNullOrEmpty(request.Item1))
            {
                lbError.Text = @"btnEncodedEncrypted Parse error!";
                return;
            }
            // decrypt, parse string and add to the dictionary
            try
            {
                var mRequestData = MACSecurity.Security.DecodeAndDecrypt(request.Item2, request.Item1);
                AddToLogAndDisplay("|Request Data|" + mRequestData + "|");
            }
            catch (Exception ex)
            {
                lbError.Text = @"btnEncodedEncrypted " + ex.Message;
                // ReSharper disable once RedundantToStringCall
                AddToLogAndDisplay("Exception|" + ex.ToString() + "|");
                return;
            }
        }

        IssueRequest(serviceUrl, "data=" + txtRequest.Text.Trim());   
    }

    private void IssueRequest(string pUrl, string pRequest)
    {
        AddToLogAndDisplay("|Url|" + pUrl + "|RequestData|" + pRequest);
        try
        {
            var dataStream = Encoding.UTF8.GetBytes(pRequest);
            var request = pUrl;
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
            var elemList = xmlDoc.GetElementsByTagName("macResponse");
            AddToLogAndDisplay(elemList[0].InnerXml);
        }
        catch (Exception ex)
        {
            AddToLogAndDisplay(ex.ToString());
            lbError.Text = @"Service error";
        }
    }

    private string GetServiceUrl()
    {
        var baseUrl = ConfigurationManager.AppSettings[cfg.MacServicesUrl];

        if (_hiddenT.Value.Contains("OAS"))
            baseUrl = ConfigurationManager.AppSettings[cfg.MacOpenServicesUrl];
        var mServiceNameAndProject = _hiddenT.Value.Split(char.Parse(dk.ItemSep));
        string mServiceName;
        string mProject;
        try
        {
            mServiceName = mServiceNameAndProject[0];
            mProject = mServiceNameAndProject[1];
        }
        catch (Exception ex)
        {
            lbError.Text = @"GetServiceUrl" + ex.Message;
            return null;
        }
        var mServiceUrl = baseUrl +
            @"/" + mProject +
            @"/" + mServiceName + ".asmx" +
            @"/" + "Ws" + mServiceName;
        AddToLogAndDisplay("Service Url: " + mServiceUrl);
        return mServiceUrl;
    }

    protected void btnClearLog_Click(object sender, EventArgs e)
    {
        Session["LogText"] = "";
        AddToLogAndDisplay("btnClearLog");
    }

    #region Select Service
    protected void ddlOtpServiceNames_Changed(object sender, EventArgs e)
    {
        AddToLogAndDisplay("ddlOtpServiceNames_Changed [" + ddlOtpServiceNames.SelectedValue.Replace(dk.ItemSep, ", ") + "]");
        _hiddenT.Value = ddlOtpServiceNames.SelectedValue;
    }

    protected void ddlUserServiceNames_Changed(object sender, EventArgs e)
    {
        AddToLogAndDisplay("ddlUserServiceNames_Changed [" + ddlUserServiceNames.SelectedValue.Replace(dk.ItemSep, ", ") + "]");
        _hiddenT.Value = ddlUserServiceNames.SelectedValue;
    }

    protected void ddlOASServiceNames_Changed(object sender, EventArgs e)
    {
        AddToLogAndDisplay("ddlOASServiceNames_Changed [" + ddlOASServiceNames.SelectedValue.Replace(dk.ItemSep, ", ") + "]");
        _hiddenT.Value = ddlOASServiceNames.SelectedValue;
    }

    protected void ddlAdminServiceNames_Changed(object sender, EventArgs e)
    {
        AddToLogAndDisplay("ddlAdminServiceNames_Changed [" + ddlAdminServiceNames.SelectedValue.Replace(dk.ItemSep, ", ") + "]");
        _hiddenT.Value = ddlAdminServiceNames.SelectedValue;
    }

    protected void ddlEventServiceNames_Changed(object sender, EventArgs e)
    {
        AddToLogAndDisplay("ddlEventServiceNames_Changed [" + ddlEventServiceNames.SelectedValue.Replace(dk.ItemSep, ", ") + "]");
        _hiddenT.Value = ddlEventServiceNames.SelectedValue;
    }

    protected void ddlTestServiceNames_Changed(object sender, EventArgs e)
    {
        AddToLogAndDisplay("ddlTestServiceNames_Changed [" + ddlTestServiceNames.SelectedValue.Replace(dk.ItemSep, ", ") + "]");
        _hiddenT.Value = ddlTestServiceNames.SelectedValue;
    }

    #endregion

    #region helpers
    private void Populate_ddlServiceNames()
    {
        ddlOtpServiceNames.Items.Clear();
        var OtpServices = OTPServiceNames.Split(char.Parse(dk.ItemSep));
        foreach (var servicename in OtpServices)
        {
            var li = new ListItem { Text = servicename, Value = servicename + dk.ItemSep + OTPService };
            ddlOtpServiceNames.Items.Add(li);
        }

        ddlUserServiceNames.Items.Clear();
        var UserServices = UserServiceNames.Split(char.Parse(dk.ItemSep));
        foreach (var servicename in UserServices)
        {
            var li = new ListItem { Text = servicename, Value = servicename + dk.ItemSep + UserService };
            ddlUserServiceNames.Items.Add(li);
        }

        ddlOASServiceNames.Items.Clear();
        var OasServices = OASServiceNames.Split(char.Parse(dk.ItemSep));
        foreach (var servicename in OasServices)
        {
            var li = new ListItem { Text = servicename, Value = servicename + dk.ItemSep + OASService };
            ddlOASServiceNames.Items.Add(li);
        }

        ddlAdminServiceNames.Items.Clear();
        var AdminServices = AdminServiceNames.Split(char.Parse(dk.ItemSep));
        foreach (var servicename in AdminServices)
        {
            var li = new ListItem { Text = servicename, Value = servicename + dk.ItemSep + AdminService };
            ddlAdminServiceNames.Items.Add(li);
        }

        ddlEventServiceNames.Items.Clear();
        var EventServices = EventServiceNames.Split(char.Parse(dk.ItemSep));
        foreach (var servicename in EventServices)
        {
            var li = new ListItem { Text = servicename, Value = servicename + dk.ItemSep + EventService };
            ddlEventServiceNames.Items.Add(li);
        }

        ddlTestServiceNames.Items.Clear();
        var TestServices = TestServiceNames.Split(char.Parse(dk.ItemSep));
        foreach (var servicename in TestServices)
        {
            var li = new ListItem { Text = servicename, Value = servicename + dk.ItemSep + TestService };
            ddlTestServiceNames.Items.Add(li);
        }
    }

    private void AddToLogAndDisplay(string textToAdd)
    {
        var newlog = String.Format("{0}|{1} - {2}", Session["LogText"], Test, textToAdd);
        Session["LogText"] = newlog;
        tbLog.Text = newlog.Replace("|", Environment.NewLine);
    }
    #endregion
}