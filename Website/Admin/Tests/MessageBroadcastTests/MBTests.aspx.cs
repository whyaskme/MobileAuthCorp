using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;

using MACServices;
using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using sr = MACServices.Constants.ServiceResponse;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using cs = MACServices.Constants.Strings;
using mbrsp = MACServices.Constants.MessageBroadcast.ResponseKeys;

public partial class Admin_Tests_MessageBroadcastTests_MBTests : System.Web.UI.Page
{
    private const string Test = "MBT";
    private const string ProviderName = "MessageBroadcast";

    HiddenField _hiddenW;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Master != null)
        {
            _hiddenW = (HiddenField)Master.FindControl("hiddenW");
            _hiddenW.Value = "54a83c16ead6362034d04bd1";
        }
    }

    #region Bottons

    protected void btnCallService_Click(object sender, EventArgs e)
    {
        lbError.Text = "";
        var mUtils = new Utils();
        AddToLogAndDisplay("btnSend_Click: Number=" + txtPhoneNumber.Text);
        if (String.IsNullOrEmpty(txtPhoneNumber.Text))
        {
            lbError.Text = @"Phone Number required!";
            return;
        }
        if (mUtils.ValidatePhoneNumber(txtPhoneNumber.Text) == false)
        {
            lbError.Text = @"Invalid Phone Number!";
            return;
        }
        if (String.IsNullOrEmpty(txtMessage.Text))
        {
            lbError.Text = @"Test Message (Text) required!";
            return;
        }

        var mMessageDeliveryServiceUrl = ConfigurationManager.AppSettings["MessageBroadcastAPIService"];
        var mLoopBackState = ConfigurationManager.AppSettings[cfg.LoopBackTest];
        var mData = new Dictionary<string, string>();
        if (cbDeliverVoice.Checked)
            mData.Add(dk.Request, ProviderName + " (Voice)");
        else
            mData.Add(dk.Request, ProviderName + " (Sms)");

        mData.Add(dk.CID, cs.DefaultClientId);      // MAC Default Client Id
        //mData.Add(dk.OtpId, pOtp._id.ToString());

        mData.Add(dkui.PhoneNumber, txtPhoneNumber.Text);
        mData.Add(dk.Message, mUtils.StringToHex(txtMessage.Text));

        // If loopback set pass additional data to delivery service
        if ((mLoopBackState != null) && (mLoopBackState != cfg.Disabled))
        {
            mData.Add(dk.LoopBackTest, cfg.NoSend);
            mData.Add(dk.Name, cs.DefaultAdminName); // Admin Login or Send
        }

        //===== Call the service ===========================================
        var rtn = mUtils.ServiceRequest(mMessageDeliveryServiceUrl, cs.DefaultClientId, mData);
        AddToLogAndDisplay("-------------");
        AddToLogAndDisplay(rtn.Item2);
        AddToLogAndDisplay("-------------");
    }

    protected void btnUseAPI_Click(object sender, EventArgs e)
    {
        lbError.Text = "";
        var mUtils = new Utils();
        AddToLogAndDisplay("btnIssueWebRequestXML_Click: Number=" + txtPhoneNumber.Text);
        if (String.IsNullOrEmpty(txtPhoneNumber.Text))
        {
            lbError.Text = @"Phone Number required!";
            return;
        }
        if (mUtils.ValidatePhoneNumber(txtPhoneNumber.Text) == false)
        {
            lbError.Text = @"Invalid Phone Number!";
            return;
        }
        if (String.IsNullOrEmpty(txtMessage.Text))
        {
            lbError.Text = @"Test Message (Text) required!";
            return;
        }
        const string mAuthToken = "B32EB51E57CC116F4BF7";
        const string mMBSID = "5E1/1BD3A4805cE3c223e6AB3Ca7414+02089F86";
        const string mURL = "http://ebmapi.messagebroadcast.com/webservice/ebm/pdc/addtorealtime/";

        var MyDateTime = DateTime.UtcNow.ToString();

        var stringToSign = "POST" + "\n\n\n" + MyDateTime + "\n\n\n";
        // private key from Sid properity
        var signature = HmacSha1SignRequest(mMBSID, stringToSign);

        var sbRestRequest = new StringBuilder();
        if (cbNoReply.Checked)
        {
            sbRestRequest.Append("inpBatchId=1530"); //1134 pure sms inpCustomSMS=Text
            sbRestRequest.Append("&inpContactString=" + txtPhoneNumber.Text);
            sbRestRequest.Append("&inpContactTypeId=3");
            sbRestRequest.Append("&inpCustomSMS=" + txtMessage.Text);
        }
        else
        {
            
        }

        AddToLogAndDisplay("Request dataStream[" + sbRestRequest + "]");

        var doc = new XmlDocument();
        try
        {
            var myHttpWebRequest = (HttpWebRequest)WebRequest.Create(mURL);
            myHttpWebRequest.Method = "POST";
            var dataStream = Encoding.UTF8.GetBytes(sbRestRequest.ToString());
            // Set the content type of the data being posted.
            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
            myHttpWebRequest.ContentLength = dataStream.Length;
            myHttpWebRequest.Headers["datetime"] = MyDateTime;
            myHttpWebRequest.Headers["Authorization"] = mAuthToken + ":" + signature;
            //Just add a header value -  name="Accept" type="header" value="application/xml"
            myHttpWebRequest.Accept = "application/xml";
            myHttpWebRequest.SendChunked = true;

            var newStream = myHttpWebRequest.GetRequestStream();
            newStream.Write(dataStream, 0, dataStream.Length);
            newStream.Close();

            var mResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
            var content = mResponse.GetResponseStream();

            if (content != null)
            {
                using (var reader = new StreamReader(content))
                {
                    doc.LoadXml(reader.ReadToEnd());
                }
            }
            else
            {
                AddToLogAndDisplay("No content"); 
                AddToLogAndDisplay("-------------");
                return;
            }
            AddToLogAndDisplay(doc.OuterXml.ToString().Replace("><", ">|<"));
        }
        catch (Exception ex)
        {
            AddToLogAndDisplay(ex.Message);
        }
        AddToLogAndDisplay("-------------");
        AddToLogAndDisplay("-------------");
    }
    
    #endregion

    #region Helpers

    protected void ParceMBResponseJSON(string pResponse, Dictionary<string, string>pDict)
    {
        const string COL = "\"COLUMNS\":[";
        const string DAT = "\"DATA\":[[";
        const string END = "]]";
        const string colnames = mbrsp.RXRESULTCODE + "," + mbrsp.INPBATCHID + "," + mbrsp.CURRTZVOICE + "," +
                                mbrsp.COUNTQUEUEDUPVOICE + "," + mbrsp.COUNTQUEUEDUPEMAIL + "," +
                                mbrsp.COUNTQUEUEDUPSMS + "," + mbrsp.CURRTZ + "," + mbrsp.DISTRIBUTIONID + "," +
                                mbrsp.BLOCKEDBYDNC + "," + mbrsp.SMSINIT + "," + mbrsp.TYPE + "," +
                                mbrsp.MESSAGE + "," + mbrsp.ERRMESSAGE + "," + mbrsp.END;
        //AddToLogAndDisplay(pResponse);
        try
        {
            string mCol1;
            string mData;
            if (pResponse.Contains(COL))
            {
                var x = pResponse.IndexOf(COL, StringComparison.Ordinal);
                var y = pResponse.IndexOf(DAT, StringComparison.Ordinal);
                //var z = pResponse.IndexOf(END, StringComparison.Ordinal);
                mCol1 = pResponse.Substring(x + COL.Length, y - 12);
                mData = pResponse.Replace(COL + mCol1, "");
                mCol1 = mCol1.Replace("\"", "").Replace("]", "," + mbrsp.END).Trim(',');
                mData = mData.Replace(DAT, "").Replace(END, ",0,0,");
            }
            else
            {
                // only data in response
                // set column names
                mCol1 = colnames;
                mData = pResponse.Replace(DAT, "").Replace(END, ",0,0,");
            }
            var mCols = mCol1.Split(',');
            AddToLogAndDisplay(mCol1);
            mData = mData.TrimEnd('}');
            mData = mData.TrimStart('{');
            AddToLogAndDisplay(mData);
            var values = new StringBuilder();
            while (mData.Length != 0)
            {
                string value;
                int j;

                if (mData.StartsWith("\""))
                {
                    mData = mData.TrimStart('\"'); // remove quote from start
                    if (mData.Length == 0)
                    {
                        break;
                    }
                    if (mData.StartsWith(","))
                    {   // null value
                        values.Append(" |"); // add null value to string
                    }
                    else
                    {
                        // Text in field
                        j = mData.IndexOf("\"", StringComparison.Ordinal); // index of ending quite
                        value = mData.Substring(0, j); // get text
                        values.Append(value + "|"); // add to values string
                        mData = mData.Replace(value + "\"", "");
                    }
                }
                else
                {
                    j = mData.IndexOf(",", StringComparison.Ordinal);
                    value = mData.Substring(0, j);
                    values.Append(value + "|");
                    mData = mData.Substring(j, mData.Length - value.Length);
                }
                mData = mData.TrimStart(','); // remove comma from start
            }
            var mDatas = (values.ToString()).Split('|');
            for (var i = 0; i < mCols.Count(); ++i)
                pDict.Add(mCols[i], mDatas[i]);
        }
        catch
        {
            AddToLogAndDisplay("--- exception ---");
        }
    }

    protected string HmacSha1SignRequest(string privateKey, string valueToHash)
    {
        var encoding = new ASCIIEncoding();
        var keyByte = encoding.GetBytes(privateKey);
        var hmacsha1 = new HMACSHA1(keyByte);
        var messageBytes = encoding.GetBytes(valueToHash);
        var hashmessage = hmacsha1.ComputeHash(messageBytes);
        var hashedValue = Convert.ToBase64String(hashmessage); // convert to base64
        return hashedValue;
    }

    protected void btnClearLog_Click(object sender, EventArgs e)
    {
        Session["LogText"] = "";
        AddToLogAndDisplay("btnClearLog");
    }

    protected void AddToLogAndDisplay(string textToAdd)
    {
        var newlog = String.Format("{0}|{1} - {2}", Session["LogText"], Test, textToAdd);
        Session["LogText"] = newlog;
        tbLog.Text = newlog.Replace("|", Environment.NewLine);
    }
    
    #endregion
}