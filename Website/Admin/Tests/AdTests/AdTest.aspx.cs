using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;
using System.Web;
using System.ServiceModel;
using System.Diagnostics;


using MACServices;
//using MACServices.SecureAdsService;
using MACServices.SecureAd;

using sr = MACServices.Constants.ServiceResponse;
using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using str = MACServices.Constants.Strings;

using ta = TestLib.TestConstants.TestAds;

public partial class MACUserApps_Web_Tests_AdTest_AdTest : System.Web.UI.Page
{
    private const string Test = "ADT";
    private const string SelectClient = "Select Client";
    private const string NoClient = "No Clients";
    private const string SelectGroup = "Select Group";
    private const string NoGroups = "No Groups";
    private const string NoTestFiles = "No Test Files";
    private const string SelectFile = "Select File";
    private const string None = "None";

    private HiddenField _hiddenC;

    HiddenField _hiddenW;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Page.Master != null)
        {
            _hiddenC = (HiddenField)Page.Master.FindControl("hiddenC");
            if (Master != null)
            {
                _hiddenW = (HiddenField)Master.FindControl("hiddenW");
                _hiddenW.Value = "54a83bd5ead6362034d04bcf";
            }
        }

        if (IsPostBack) return;
        rbNone.Attributes.Add("onclick", "javascript:RequestUsageChange()");
        rbUseCampaignId.Attributes.Add("onclick", "javascript:RequestUsageChange()");
        rbUseCampaignName.Attributes.Add("onclick", "javascript:RequestUsageChange()");
        rbUseProfileData.Attributes.Add("onclick", "javascript:RequestUsageChange()");
        rbUseKeywords.Attributes.Add("onclick", "javascript:RequestUsageChange()");

        GetClients();
        SetTestAdsNumber();
        FillddlSpecialKeywords();
        FillddlTestFiles();
    }


    #region Button event handlers
    protected void btnShowTestAd_Click(object sender, EventArgs e)
    {
        AddToLogAndDisplay("btnShowTestAd_Click");
        lbError.Text = "";
        var mClientName = ddlClient2.SelectedItem.Text;
        if (String.IsNullOrEmpty(mClientName))
        {
            lbError.Text = @"Select a client!";
            return;
        }
        if (mClientName == NoClient)
        {
            lbError.Text = @"No clients!";
            return;
        }
        if (mClientName == SelectClient)
        {
            lbError.Text = @"Select a client!";
            return;
        }

        string mTargetedAd = String.Empty;
        if (ddlSpeKeywords.SelectedItem.Text != None)
        {
            var Keyword = ddlSpeKeywords.SelectedItem.Text.Split('(');
            mTargetedAd = dk.ItemSep + dk.Ads.SpecificKeywords + dk.KVSep + StringToHex(Keyword[0].Trim());
        }
        else if (ddlAdNumber.SelectedItem.Text != None)
        {
            mTargetedAd = dk.ItemSep + dk.Ads.AdNumber + dk.KVSep + ddlAdNumber.Text;
        }

        var Url = ConfigurationManager.AppSettings[cfg.MacServicesUrl] +
                               Constants.ServiceUrls.MacTestAdService;
        var mRequestData = dk.Request + dk.KVSep + "GetAd" +
                           dk.ItemSep + dk.ClientName + dk.KVSep + mClientName +
                           mTargetedAd;

        // send to Mac Test Ad service
        var reply = new XmlDocument();
        try
        {
            var dataStream =
                Encoding.UTF8.GetBytes("data=99" + str.DefaultClientId.Length + str.DefaultClientId.ToUpper() +
                                       StringToHex(mRequestData));
            var mRequest = Url;
            var webRequest = WebRequest.Create(mRequest);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = dataStream.Length;
            var newStream = webRequest.GetRequestStream();
            // Send the data.
            newStream.Write(dataStream, 0, dataStream.Length);
            newStream.Close();
            var res = webRequest.GetResponse();
            var response = res.GetResponseStream();

            if (response != null) reply.Load(response);
        }
        catch (Exception ex)
        {
            AddToLogAndDisplay("GetTestAdFromTestAdService.Exception: " + ex.Message);
        }
        var elemList = reply.GetElementsByTagName(sr.Error);
        if (elemList.Count != 0)
        {
            lbError.Text = @"Error " + elemList[0].InnerXml;
            elemList = reply.GetElementsByTagName(sr.Details);
            if (elemList.Count != 0)
            {
                var mDetails = HexToString(elemList[0].InnerXml);
                AddToLogAndDisplay(lbError.Text + "|" + mDetails);
            }
            else
                AddToLogAndDisplay(lbError.Text + " No details");
            return;
        }
        elemList = reply.GetElementsByTagName(str.AdMessage);
        if (elemList.Count != 0)
        {
            var adtxt = HexToString(elemList[0].InnerXml);
            AddToLogAndDisplay(str.AdMessage + ": " + adtxt);

            var httpoffset = adtxt.ToLower().IndexOf("http", StringComparison.Ordinal);
            if (httpoffset > 1)
                lbMesage.Text = adtxt.Substring(0, httpoffset - 1);
            lkMessage.Text = adtxt.Substring(httpoffset, adtxt.Length - httpoffset);
        }
        elemList = reply.GetElementsByTagName(str.AdEnterOtp);
        if (elemList.Count != 0)
        {
            AddToLogAndDisplay(str.AdEnterOtp + ": " + HexToString(elemList[0].InnerXml));
            divAdEnterOtpScreenSent.InnerHtml = HexToString(elemList[0].InnerXml);
        }
        elemList = reply.GetElementsByTagName(str.AdVerification);
        if (elemList.Count != 0)
        {
            AddToLogAndDisplay(str.AdVerification + ": " + HexToString(elemList[0].InnerXml));
            divAdVerificationScreenSent.InnerHtml = HexToString(elemList[0].InnerXml);
        }
    }

    protected void btnGetAdFromSA_Click(object sender, EventArgs e)
    {
        lbError.Text = "";
        var mButton = (Button)sender;
        AddToLogAndDisplay(mButton.Text);

        // clear ad displays before request
        lbMesage.Text = "";
        divAdEnterOtpScreenSent.InnerHtml = "";
        divAdVerificationScreenSent.InnerHtml = "";

        if (ddlTestFiles.Text == NoTestFiles)
        {
            lbError.Text = @"No text files to select!";
            return;
        }
        if (ddlTestFiles.Text == SelectFile)
        {
            lbError.Text = @"Select a file!";
            return;
        }
        if (ddlTestFiles.Text.EndsWith(".txt") != true)
        {
            lbError.Text = @"File type error!";
            return;
        }

        var stopwatch = new Stopwatch();
        try
        {
            AddToLogAndDisplay("Url[" + lbUrl.Text + "]");
            AddToLogAndDisplay("apiAuthen[ApiKey:" + lbAPIKey.Text + ", UserName:" + lbAPIUserName.Text + ", Password:" + lbAPIPassword.Text + "]");

            stopwatch.Start();              // time the request
            
            var request = new GetAdRequest {ClientId = lbClientId.Text};

            if (rbUseCampaignId.Checked)
            {
                AddToLogAndDisplay("Request client id & Campaign Id[ClientId:" + lbClientId.Text + ", Campaign Id" + lbCampaignId.Text + "]");
                request.CampaignId = lbCampaignId.Text;
            }
            else if (rbUseCampaignName.Checked)
            {
                AddToLogAndDisplay("Request client id & Campaign Name[ClientId:" + lbClientId.Text + ", Campaign Name" + lbCampaignName.Text + "]");
                request.CampaignName = lbCampaignName.Text;
            }
            else if (rbUseProfileData.Checked)
            {
                var mProfileData = GetProfileData();
                AddToLogAndDisplay("Request client id & ProfileData[ClientId:" + lbClientId.Text + ", ProfileData" + mProfileData + "]");

                //add filter profile in here
                
            }
            else if (rbUseKeywords.Checked)
            {
                var mKeywords = GetKeywords();
                AddToLogAndDisplay("Request client id & keywords[ClientId:" + lbClientId.Text + ", Keywords" + mKeywords + "]");
                
                //request.CampaignProfiling = new CampaignProfiling { Keywords =  mKeywords };
            }
            else
            {
                AddToLogAndDisplay("Request client id[ClientId:" + lbClientId.Text + "]");
            }
            var apiAuthen = new ApiAuthen
            {
                ApiKey = lbAPIKey.Text,
                UserName = lbAPIUserName.Text,
                Password = lbAPIPassword.Text
            };

            var adresponse = ExecuteService<IAd, GetAdResponse>(lbUrl.Text, service => service.GetAd(apiAuthen, request));

            if (!adresponse.Success)
            {
                var errors = adresponse.Errors;
                foreach (var err in errors)
                    lbError.Text += err + @", ";
                return;
            }
            
            if (adresponse.MobileMesasge != null)
            {
                var AdMessageSent = adresponse.MobileMesasge.Message;
                if (String.IsNullOrEmpty(AdMessageSent))
                {
                    lbMesage.Text = @"No MobileMesasge Ad returned!";
                    AddToLogAndDisplay(lbMesage.Text);
                }
                else
                {
                    lbMesage.Text = AdMessageSent;
                    AddToLogAndDisplay("AdMessageSent: " + lbMesage.Text);
                }
            }
            else
            {
                lbMesage.Text = @"No Message ad returned!";
                AddToLogAndDisplay(lbMesage.Text);
            }
            if (adresponse.EnterOtpScreen != null)
            {
                var AdEnterOtpScreenSent = adresponse.EnterOtpScreen.ContentHtml.ToString();
                AdEnterOtpScreenSent = AdEnterOtpScreenSent.Replace(@"\", "/").Trim();
                divAdEnterOtpScreenSent.InnerHtml = AdEnterOtpScreenSent;
                AddToLogAndDisplay("Content Ad: " + AdEnterOtpScreenSent + "|");

                //if (contentAd.Contains(str.adcoupondata))
                //{
                //    var couponstrt = contentAd.IndexOf(str.adcoupondata, StringComparison.Ordinal) + str.adcoupondata.Length + 1;
                //    var couponend = contentAd.IndexOf("' ", couponstrt, StringComparison.Ordinal);
                //    AdMessageDiv.InnerHtml += "<br /><br />Coupon details: " + contentAd.Substring(couponstrt, couponend - couponstrt);
                //}
            }
            else
            {
                divAdEnterOtpScreenSent.InnerHtml = "No EnterOtpScreen ad returned!";
                AddToLogAndDisplay(" No EnterOtpScreen ad returned!");
            }
            if (adresponse.VerificationPage != null)
            {
                var AdVerificationScreenSent = adresponse.VerificationPage.ContentHtml.ToString();
                AdVerificationScreenSent = AdVerificationScreenSent.Replace(@"\", "/").Trim();
                divAdVerificationScreenSent.InnerHtml = AdVerificationScreenSent;
                AddToLogAndDisplay("VerificationPage Ad: " + AdVerificationScreenSent + "|");
            }
            else
            {
                divAdVerificationScreenSent.InnerHtml = "No VerificationPage ad returned!";
                AddToLogAndDisplay(" No VerificationPage ad returned!");
            }
            // close the channel
            //myChannelFactory.Close();
            stopwatch.Stop();
            AddToLogAndDisplay("ET: " + stopwatch.Elapsed);
        }
        catch (Exception ex)
        {
            lbError.Text = ex.Message;
            AddToLogAndDisplay(ex.ToString());
        }
    }

    protected TResult ExecuteService<TService, TResult>(string url, Func<TService, TResult> funcExec)
    {
        var mAdServerRequestType = ConfigurationManager.AppSettings["AdServerRequestType"];
        if (mAdServerRequestType == null || mAdServerRequestType.ToString() == "http")
        {
            var myBinding = new BasicHttpBinding();
            var myEndpoint = new EndpointAddress("http://" + url);
            using (var myChannelFactory = new ChannelFactory<TService>(myBinding, myEndpoint))
            {
                var client = myChannelFactory.CreateChannel();

                return funcExec(client);
            }
        }
        else
        {
            var myBinding = new BasicHttpsBinding();
            var myEndpoint = new EndpointAddress("https://" + url);
            using (var myChannelFactory = new ChannelFactory<TService>(myBinding, myEndpoint))
            {
                var client = myChannelFactory.CreateChannel();

                return funcExec(client);
            }
        }
    }
    
    protected string GetProfileData()
    {
        return null;
    }

    protected string GetKeywords()
    {
        return null;
    }

    protected string GetElementValues(XmlDocument pXDoc, string pElement)
    {
        var elemList = pXDoc.GetElementsByTagName(pElement);
        if (elemList.Count == 0)
        {
            lbError.Text = @"Error: no " + pElement + @" element";
            return null;
        }
        return elemList[0].InnerXml;
    }

    protected void btnInitAdProvider_Click(object sender, EventArgs e)
    {
        // Terry !
        //var mUtils = new Utils();
        //AddToLogAndDisplay("btnInitAdProvider_Click");
        //var cid = GetSelectedClientId(ddlInitClient.SelectedValue);
        //if (string.IsNullOrEmpty(cid)) return;
        //var mClient = mUtils.GetClientUsingClientId(cid);
        //var mAdCampaign = new AdCampaign();
        //if (String.IsNullOrEmpty(txtInitAdClientId.Text.Trim()))
        //{
        //    mAdCampaign.AdClientId = string.Empty;
        //    mAdCampaign.CampaignId = string.Empty;
        //    mAdCampaign.CampaignName = string.Empty;
        //}
        //else
        //{
        //    if (String.IsNullOrEmpty(txtInitCampaignId.Text.Trim()) == false)
        //    {
        //        mAdCampaign.AdClientId = txtInitAdClientId.Text.Trim();
        //        mAdCampaign.CampaignId = txtInitCampaignId.Text.Trim();
        //    }
        //}
        
        //mClient.AdProviders.Add(mAdCampaign);
        //mClient.Update();

        //AddToLogAndDisplay("Client Updated:|AdClientId=" + mAdCampaign.AdClientId + "|CampaignId=" + mAdCampaign.CampaignId);
    }

    protected void lkMessage_Clicked(object sender, EventArgs e)
    {
        Response.Redirect(lkMessage.Text);
        Response.Close();
    }

    #endregion

    //public XmlDocument xGetTestAdFromTestAdService(string pClientName, string pAdNumber)
    //{
    //    var Url = ConfigurationManager.AppSettings[cfg.MacServicesUrl] +
    //                    Constants.ServiceUrls.MacTestAdService;
    //    var mRequestData = dk.Request + dk.KVSep + "GetAd" +
    //                       dk.ItemSep + dk.ClientName + dk.KVSep + pClientName +
    //                       dk.ItemSep + dk.Ads.AdNumber + dk.KVSep + pAdNumber;

    //    // send to Mac Test Ad service
    //    var xmlDoc = new XmlDocument();
    //    try
    //    {
    //        var dataStream =
    //            Encoding.UTF8.GetBytes("data=99" + str.DefaultClientId.Length + str.DefaultClientId.ToUpper() +
    //                                   StringToHex(mRequestData));
    //        var mRequest = Url;
    //        var webRequest = WebRequest.Create(mRequest);
    //        webRequest.Method = "POST";
    //        webRequest.ContentType = "application/x-www-form-urlencoded";
    //        webRequest.ContentLength = dataStream.Length;
    //        var newStream = webRequest.GetRequestStream();
    //        // Send the data.
    //        newStream.Write(dataStream, 0, dataStream.Length);
    //        newStream.Close();
    //        var res = webRequest.GetResponse();
    //        var response = res.GetResponseStream();

    //        if (response != null) xmlDoc.Load(response);
    //    }
    //    catch (Exception ex)
    //    {
    //        AddToLogAndDisplay("GetTestAdFromTestAdService.Exception: " + ex.Message);
    //    }
    //    return xmlDoc;
    //}

    #region Common Test Methods
    protected void SetTestAdsNumber()
    {
        ddlAdNumber.Items.Clear();

        {
            var li = new ListItem();
            li.Text = li.Value = None;
            ddlAdNumber.Items.Add(li);
        }
        for (var x = 1; x < 6; ++x)
        {
            var mLi = new ListItem();
            mLi.Text = mLi.Value = x.ToString();
            ddlAdNumber.Items.Add(mLi);
        }
    }

    /// <summary> Get selected Client Id </summary>
    private string GetSelectedClientId(string pClientDetails)
    {
        if (pClientDetails == SelectClient)
        {
            lbError.Text = @"You must select a client!";
            return null;
        }
        var cd = pClientDetails.Split(char.Parse(dk.ItemSep));
        return cd[0];
    }

    /// <summary> get client list </summary>
    private void GetClients()
    {
        try
        {
            var url = ConfigurationManager.AppSettings[cfg.MacServicesUrl] +
                      TestLib.TestConstants.GetTestClientsInfoUrl;
            var request = WebRequest.Create(url);
            request.Method = "Post";
            request.ContentLength = 0;
            var res = request.GetResponse();
            var response = res.GetResponseStream();
            var xmlDoc = new XmlDocument();
            if (response != null) xmlDoc.Load(response);
            var elemList = xmlDoc.GetElementsByTagName(sr.Error);
            if (elemList.Count != 0)
            {
                lbError.Text = String.Format("Error: returned from GetClients service {0}", elemList[0].InnerXml);
                return;
            }

            ddlInitClient.Items.Clear();
            elemList = xmlDoc.GetElementsByTagName("Client");
            if (elemList.Count != 0)
            {
                var li1 = new ListItem
                {
                    Text = SelectClient,
                    Value = SelectClient
                };
                ddlInitClient.Items.Add(li1);
                ddlClient2.Items.Add(li1);
                foreach (XmlNode myClientNode in elemList)
                {
                    var li = new ListItem
                    {
                        Text = myClientNode.ChildNodes[0].InnerXml,
                        Value = myClientNode.ChildNodes[1].InnerXml
                    };
                    var gelemList = myClientNode.ChildNodes[2].ChildNodes;
                    foreach (XmlNode gnode in gelemList)
                    {
                        li.Value += dk.ItemSep + gnode.ChildNodes[0].InnerText;
                    }
                    ddlInitClient.Items.Add(li);
                    ddlClient2.Items.Add(li);
                }
            }
            else
            {
                var li0 = new ListItem
                {
                    Text = NoClient,
                    Value = NoClient
                };
                ddlInitClient.Items.Add(li0);
                ddlClient2.Items.Add(li0);
            }
        }
        catch (Exception ex)
        {
            AddToLogAndDisplay(ex.ToString());
            lbError.Text = @"GetClients service error";
        }
    }
    
    public void ddlGetClient_Selected(object sender, EventArgs e)
    {
        SetGetGroups();
        // GetIdsForClient(ddlGetClient.SelectedItem.Text, txtGetAdClientId, txtGetCampaignId);
    }

    public void ddlClient2_Selected(object sender, EventArgs e)
    {
        //SetGetGroups();
        // GetIdsForClient(ddlGetClient.SelectedItem.Text, txtGetAdClientId, txtGetCampaignId);
    }

    private void SetGetGroups()
    {
        //lbGetGroup.Visible = false;
        //ddlGetGroup.Visible = false;

        //ddlGetGroup.Items.Clear();
        //var values = ddlGetClient.SelectedValue.Split(Char.Parse(dk.ItemSep));
        //if (values.Count() < 2)
        //{
        //    ddlGetGroup.Items.Add(NoGroups);
        //    _hiddenC.Value = "";
        //    return;
        //}
        //var skipcid = true; // first entry is always the clientId
        //foreach (var group in values)
        //{
        //    var grouplist = new ListItem();
        //    if (skipcid)
        //    {
        //        grouplist.Text = grouplist.Value = SelectGroup;
        //        skipcid = false;
        //    }
        //    else
        //    {
        //        lbGetGroup.Visible = true;
        //        ddlGetGroup.Visible = true;
        //        var name_id = group.Split('=');
        //        grouplist.Text = name_id[0];
        //        grouplist.Value = name_id[1];
        //    }
        //    ddlGetGroup.Items.Add(grouplist);
        //}
    }

    private void GetIdsForClient(String pClientName, TextBox AdClientId, TextBox CampaignId)
    {
        if (pClientName.Contains("Golf"))
        {
            AdClientId.Text = ta.TestCampaigns.MACOnlineMerchant.AdClientId;
            if (pClientName.Contains("Shop"))
            {
                CampaignId.Text = ta.TestCampaigns.MACOnlineMerchant.AdCampaign1.CampaignId;
            }
            else
            {
                CampaignId.Text = ta.TestCampaigns.MACOnlineMerchant.AdCampaign2.CampaignId;
            }
        }
        else if (pClientName.Contains("MAC Test Bank"))
        {
            AdClientId.Text = ta.TestCampaigns.MACOnlineBank.AdClientId;
            CampaignId.Text = ta.TestCampaigns.MACOnlineBank.AdCampaign1.CampaignId;
        }
        else if (pClientName.Contains("TNS"))
        {
            AdClientId.Text = ta.TestCampaigns.MACOnlineBank.AdClientId;
            CampaignId.Text = ta.TestCampaigns.MACOnlineBank.AdCampaign2.CampaignId;
        }
        else
        {
            AdClientId.Text = String.Empty;
            CampaignId.Text = String.Empty;
        }
    }

    #endregion

    #region Test Files Methods
    protected void FillddlSpecialKeywords()
    {
        ddlSpeKeywords.Items.Clear();
        // get list of test files
        var path = HttpContext.Current.Server.MapPath(".");
        var mPathToFile = Path.Combine(path, "AdTestFiles", "SpecialKeywordsList.txt");
        {
            var li = new ListItem();
            li.Text = li.Value = None;
            ddlSpeKeywords.Items.Add(li);
        }
        if (File.Exists(mPathToFile) == false)
        {
            AddToLogAndDisplay("No Special Keywords List file @ " + mPathToFile);
            return;
        }
        try
        {
            string line;
            // Read the file and display it line by line.
            var file = new StreamReader(mPathToFile);
            while((line = file.ReadLine()) != null)
            {
                if (line.StartsWith("#")) continue;
                if (line.StartsWith("*")) continue;
                {
                    var li = new ListItem();
                    li.Text = li.Value = line.Trim();
                    ddlSpeKeywords.Items.Add(li);
                }
            }
            file.Close();
        }
        catch (Exception ex)
        {
            lbError.Text = @"The file could not be read!";
            AddToLogAndDisplay(ex.Message);
        }
    }

    protected void FillddlTestFiles()
    {
        ddlTestFiles.Items.Clear();
        // get list of test files
        var path = HttpContext.Current.Server.MapPath(".");
        var folder = Path.Combine(path, "AdTestFiles");
        var d = new DirectoryInfo(folder);
        var Files = d.GetFiles("*.txt"); //Getting Text files

        if (!Files.Any())
        {
            var li = new ListItem();
            li.Text = li.Value = NoTestFiles;
            ddlTestFiles.Items.Add(li);
            return;
        }
        {
            var li = new ListItem();
            li.Text = li.Value = SelectFile;
            ddlTestFiles.Items.Add(li);
        }
        foreach (var file in Files)
        {
            var li = new ListItem();
            li.Text = li.Value = file.Name;
            ddlTestFiles.Items.Add(li);
        }
    }
    public void ddlTestFile_Selected(object sender, EventArgs e)
    {
        rbUseCampaignId.Checked = false;
        rbUseCampaignName.Checked = false;
        rbUseProfileData.Checked = false;
        rbUseKeywords.Checked = false;
        rbNone.Checked = true;
        divSettings.Visible = false;
        divButton.Visible = false;

        if (ddlTestFiles.Text == SelectFile) return;
        if (ddlTestFiles.Text == NoTestFiles) return;
        divSettings.Visible = true;
        divButton.Visible = true;

        var mFolder = HttpContext.Current.Server.MapPath(".");
        var mPathToFile = Path.Combine(mFolder, "AdTestFiles", ddlTestFiles.Text);
        var mAdDoc = new XmlDocument();
        try
        {
            using (var sr = new StreamReader(mPathToFile))
            {
                var line = sr.ReadToEnd();
                AddToLogAndDisplay("|" + line);
                mAdDoc.LoadXml(line);
            }
        }
        catch (Exception ex)
        {
            lbError.Text = @"The file could not be read!";
            AddToLogAndDisplay(ex.Message);
            return;
        }

        if (String.IsNullOrEmpty(lbClientName.Text = GetElementValues(mAdDoc, "ClientName"))) return;
        if (String.IsNullOrEmpty(lbClientId.Text = GetElementValues(mAdDoc, "ClientId"))) return;
        if (String.IsNullOrEmpty(lbUrl.Text = GetElementValues(mAdDoc, "Url"))) return;
        if (String.IsNullOrEmpty(lbAPIKey.Text = GetElementValues(mAdDoc, "APIKey"))) return;
        if (String.IsNullOrEmpty(lbAPIUserName.Text = GetElementValues(mAdDoc, "APIUserName"))) return;
        if (String.IsNullOrEmpty(lbAPIPassword.Text = GetElementValues(mAdDoc, "APIPassword"))) return;

        //lbUserInfo.Text = GetElementValue(mAdDoc, "UserName");
        //lbUserInfo.Text += @":" + GetElementValue(mAdDoc, "UserPassword");

        if (String.IsNullOrEmpty(lbCampaignName.Text = GetElementValues(mAdDoc, "CampaignName"))) return;
        if (String.IsNullOrEmpty(lbCampaignId.Text = GetElementValues(mAdDoc, "CampaignId"))) return;
        if (String.IsNullOrEmpty(txtPossibleKeywords.Text = GetElementValues(mAdDoc, "CampaignKeywords")))
            lbError.Text = "";
    }

    #endregion

    #region Helper methods

    public void ddlInitClient_Selected(object sender, EventArgs e)
    {
        SetInitGroups();

        GetIdsForClient(ddlInitClient.SelectedItem.Text, txtInitAdClientId, txtInitCampaignId);
        /*
            MAC Online Bank ClientId 336e5f5c-67fd-4813-a529-10ad1125e48f
                Campaign ID
                Campaign 1 ed3f576d-23ec-49b0-9695-a45960354ad6
                Campaign 2 81554ddf-1961-48de-af14-a93d532d63cb
 
            MAC Online Merchant bb246fbb-ec75-4304-bb47-6333a2a5a4af
                Campaign ID
                Campaign 1 3fd7e13b-a4e5-4d67-897d-c8f6c68400f4
                Campaign 2 a41edb58-637d-4809-ab3e-a37931e7835a
 
        */
        //var mClientName = ddlInitClient.SelectedItem.Text;
        //if (mClientName.Contains("Golf"))
        //{
        //    txtInitAdClientId.Text = ta.TestCampaigns.MACOnlineMerchant.AdClientId;
        //    if (mClientName.Contains("Shop"))
        //    {
        //        txtInitCampaignId.Text = ta.TestCampaigns.MACOnlineMerchant.AdCampaign1.CampaignId;
        //    }
        //    else
        //    {
        //        txtInitCampaignId.Text = ta.TestCampaigns.MACOnlineMerchant.AdCampaign2.CampaignId;
        //    }
        //}
        //else if (mClientName.Contains("MAC Test Bank"))
        //{
        //    txtInitAdClientId.Text = ta.TestCampaigns.MACOnlineBank.AdClientId;
        //    txtInitCampaignId.Text = ta.TestCampaigns.MACOnlineBank.AdCampaign1.CampaignId;
        //}
        //else if (mClientName.Contains("TNS"))
        //{
        //    txtInitAdClientId.Text = ta.TestCampaigns.MACOnlineBank.AdClientId;
        //    txtInitCampaignId.Text = ta.TestCampaigns.MACOnlineBank.AdCampaign2.CampaignId;
        //}
        //else
        //{
        //    txtInitAdClientId.Text = String.Empty;
        //    txtInitCampaignId.Text = String.Empty;
        //}

    }
    
    private void SetInitGroups()
    {
        lbInitGroup.Visible = false;
        ddlInitGroup.Visible = false;

        ddlInitGroup.Items.Clear();
        var values = ddlInitClient.SelectedValue.Split(Char.Parse(dk.ItemSep));
        if (values.Count() < 2)
        {
            ddlInitGroup.Items.Add(NoGroups);
            _hiddenC.Value = "";
            return;
        }
        var skipcid = true; // first entry is always the clientId
        foreach (var group in values)
        {
            var grouplist = new ListItem();
            if (skipcid)
            {
                grouplist.Text = grouplist.Value = SelectGroup;
                skipcid = false;
            }
            else
            {
                lbInitGroup.Visible = true;
                ddlInitGroup.Visible = true;
                var name_id = group.Split('=');
                grouplist.Text = name_id[0];
                grouplist.Value = name_id[1];
            }
            ddlInitGroup.Items.Add(grouplist);
        }
    }

    protected void btnClearLog_Click(object sender, EventArgs e)
    {
        Session["LogText"] = "";
        AddToLogAndDisplay("btnClearLog");
    }

    protected void AddToLogAndDisplay(string textToAdd)
    {
        var newlog = String.Format("{0}|{1}.{2}", Session["LogText"], Test, textToAdd.Replace("&apos;", "'"));
        Session["LogText"] = newlog;
        tbLog.Text = newlog.Trim('|').Replace("|", Environment.NewLine);
    }

    protected string StringToHex(String input)
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

    #endregion

}