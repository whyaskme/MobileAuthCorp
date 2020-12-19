using System;
using System.Configuration;
using System.Net;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;

using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using sr = MACServices.Constants.ServiceResponse;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;

public partial class MACUserApps_Web_Tests_LogTesting_EventLogTests : System.Web.UI.Page
{
    private const string Test = "LogTesting";
    private const string SelectException = "Select Exception";

    private const string ExceptionList = SelectException + dk.KVSep + SelectException +
                                         dk.ItemSep + "System.NullReferenceException" + dk.KVSep + "NullReference" +
                                         dk.ItemSep + "System.Collections.Generic.KeyNotFoundException" + dk.KVSep +
                                         "KeyNotFoundException" +
                                         dk.ItemSep + "System.Net.Sockets.SocketException" + dk.KVSep +
                                         "SocketException" +
                                         dk.ItemSep + "System.IO.IOException" + dk.KVSep + "IOException" +
                                         dk.ItemSep + "MongoDB.Driver.MongoConnectionException" + dk.KVSep +
                                         "MongoConnectionException" +
                                         dk.ItemSep + "System.Net.WebException" + dk.KVSep + "WebException" +
                                         dk.ItemSep + "System.IndexOutOfRangeException" + dk.KVSep +
                                         "IndexOutOfRangeException" +
                                         dk.ItemSep + "Unhandled" + dk.KVSep + "Unhandled";

    HiddenField _hiddenW;
        
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Master != null)
        {
            _hiddenW = (HiddenField)Master.FindControl("hiddenW");
            _hiddenW.Value = "54a83c4dead6362034d04bd3";
        }

        if (IsPostBack) return;

        ddlExceptions.Items.Clear();
        var exceptions = ExceptionList.Split(char.Parse(dk.ItemSep));
        foreach (var exception in exceptions)
        {
            var namevalue = exception.Split(char.Parse(dk.KVSep));
            var li = new ListItem {Text = namevalue[0], Value = namevalue[1]};
            ddlExceptions.Items.Add(li);
        }
    }

    protected void btnExecute_Click(object sender, EventArgs e)
    {
        if (ddlExceptions.SelectedValue == SelectException)
        {
            lbError.Text = @"Select an exception";
            return;
        }

        try
        {
            var dataStream = Encoding.UTF8.GetBytes("data=" + ddlExceptions.SelectedValue);
            var request = ConfigurationManager.AppSettings[cfg.MacServicesUrl] + "/Test/EventLogTests.asmx/WsEventLogTests";
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
    private void AddToLogAndDisplay(string textToAdd)
    {
        var newlog = String.Format("{0}|{1} - {2}", Session["LogText"], Test, textToAdd);
        Session["LogText"] = newlog;
        tbLog.Text = newlog.Replace("|", Environment.NewLine);
    }
}