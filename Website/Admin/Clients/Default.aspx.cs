using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

using MongoDB.Bson;

using MACSecurity;
using MACServices;

using dk = MACServices.Constants.Dictionary.Keys;
using cs = MACServices.Constants.Strings;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;

namespace Admin.Clients
{
    public partial class Default : System.Web.UI.Page
    {
        HelpUtils helpUtils = new HelpUtils();

        // This is all being converted to ajax
        public static string MacServicesUrl = ConfigurationManager.AppSettings[cfg.MacServicesUrl];

        public string loggedInUserId = "";
        public string loggedInUserIsReadOnly = "";
        public string loggedInUserFirstName = "";
        public string loggedInUserLastName = "";

        public bool isNewClient = false;

        HtmlForm _myForm { get; set; }

        HiddenField _hiddenF;
        HiddenField _hiddenE;
        HiddenField _hiddenH;
        HiddenField _hiddenI;
        HiddenField _hiddenD;
        HiddenField _hiddenT;
        HiddenField _userIpAddress;
        HiddenField _hiddenUserRole;

        HiddenField _hiddenW;

        private TextBox _updateMsg;

        Panel _bodyContainer;

        public Event Event = new Event();

        public Utils MUtils = new Utils();

        public string CurrentPageState = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentPageState = "Page_Load";

            btnCancelClientUpdate.Disabled = true;

            // Set alert boxes
            updateMessage.Visible = false;
            updateMessage.Attributes.Add("class", "alert-box success radius");

            if (Page.Master != null)
            {
                if (Master != null)
                {
                    _updateMsg = (TextBox)Master.FindControl("divServiceResponseMessage");

                    _myForm = (HtmlForm)Page.Master.FindControl("formMain");

                    _hiddenF = (HiddenField)Page.Master.FindControl("hiddenF");
                    _hiddenE = (HiddenField)Page.Master.FindControl("hiddenE");
                    _hiddenH = (HiddenField)Page.Master.FindControl("hiddenH");
                    _hiddenI = (HiddenField)Page.Master.FindControl("hiddenI");
                    _hiddenD = (HiddenField)Page.Master.FindControl("hiddenD");
                    _hiddenT = (HiddenField)Page.Master.FindControl("hiddenT");
                    _hiddenUserRole = (HiddenField)Page.Master.FindControl("hiddenL");
                    _userIpAddress = (HiddenField)Master.FindControl("hiddenM");

                    _hiddenW = (HiddenField)Master.FindControl("hiddenW");
                    _hiddenW.Value = "5490bfa2ead63627d88e3e20";

                    _bodyContainer = (Panel)Master.FindControl("BodyContainer");
                    _bodyContainer.Height = 1000;
                }
            }

            if (Request["uid"] != null)
                _hiddenE.Value = Request["uid"].ToString(CultureInfo.CurrentCulture);

            if (Request["fname"] != null)
                _hiddenH.Value = Request["fname"].ToString(CultureInfo.CurrentCulture);

            if (Request["lname"] != null)
                _hiddenI.Value = Request["lname"].ToString(CultureInfo.CurrentCulture);

            if (Request["panelFocus"] != null)
                panelFocusClients.Value = Request["panelFocus"].ToString(CultureInfo.CurrentCulture);

            loggedInUserId = MACSecurity.Security.DecodeAndDecrypt(_hiddenE.Value, Constants.Strings.DefaultClientId);
            loggedInUserIsReadOnly = MACSecurity.Security.DecodeAndDecrypt(_hiddenF.Value, loggedInUserId);
            loggedInUserFirstName = MACSecurity.Security.DecodeAndDecrypt(_hiddenH.Value, loggedInUserId);
            loggedInUserLastName = MACSecurity.Security.DecodeAndDecrypt(_hiddenI.Value, loggedInUserId);

            #region Set date range

            var endDate = DateTime.UtcNow;
            var ts = new TimeSpan(7, 0, 0, 0, 0);
            var startDate = endDate.Add(-ts);

            if (popupDatepickerStartDate.Value == "")
                popupDatepickerStartDate.Value = startDate.ToString(CultureInfo.CurrentCulture);

            if (popupDatepickerEndDate.Value == "")
                popupDatepickerEndDate.Value = endDate.ToString(CultureInfo.CurrentCulture);

            #endregion

            Session["ClientXML"] = "";

            if (dlClients.SelectedIndex > 0)
                GetHiddenClientList(dlClients.SelectedItem.Text);

            assignmentMenu.SelectedIndex = 0;

            if (!IsPostBack)
            {
                GetStateList();
                GetClientList();

                if (Request[Constants.Dictionary.Keys.CID] != null)
                {
                    dlClients.SelectedValue = Request[dk.CID].ToString(CultureInfo.CurrentCulture);

                    panelFocusClients.Value = "Tab_EventReporting";

                    GetPopulatedClientFromWebService();

                    btnClientActions.Value = @"Update";
                    btnClientActions.Visible = true;
                }
            }
            else
            {
                dlClients.Visible = true;
                txtClientID.Visible = true;
                btnRequestOtp.Visible = true;

                switch (_hiddenT.Value)
                {
                    case "CancelUpdateClient":
                        ResetForm();
                        _hiddenT.Value = "";
                        btnClientActions.Value = @"Create New";
                        dlClients.SelectedIndex = 0;
                        _hiddenD.Value = cs.DefaultStaticObjectId;
                        spanOrganization.InnerHtml = "New Client Name";
                        break;

                    case "CreateNewClient":
                        ResetForm();

                        btnClientActions.Value = @"Save";
                        btnClientActions.Visible = true;
                        btnCancelClientUpdate.Disabled = false;

                        GetClientList();

                        dlClients.SelectedIndex = 0;
                        dlClients.Visible = false;

                        _hiddenD.Value = cs.DefaultStaticObjectId;
                        spanOrganization.InnerHtml = "New Client Name";
                        btnRequestOtp.Visible = false;
                        GetGlobalProviders(true);
                        break;

                    case "SaveNewClient":
                        isNewClient = true;

                        SaveClient("");

                        GetPopulatedClientFromWebService();

                        btnCancelClientUpdate.Disabled = false;
                        _hiddenT.Value = "";
                        _hiddenD.Value = cs.DefaultStaticObjectId;
                        spanOrganization.InnerHtml = "New Client Name";
                        btnRequestOtp.Visible = true;

                        _hiddenT.Value = "";
                        break;

                    case "UpdateExistingClient":

                        var providersToDelete = hiddenProvidersToDelete.Value;

                        SaveClient(dlClients.SelectedValue);

                        // Reset hidden providers
                        hiddenProvidersToDelete.Value = "";
                        hiddenRetryList.Value = "";

                        // Reset session variable so it will populate with update info
                        //Session["ClientXML"] = "";

                        GetPopulatedClientFromWebService();

                        btnCancelClientUpdate.Disabled = false;
                        _hiddenT.Value = "";
                        break;

                    default:
                        btnRequestOtp.Visible = true;

                        if (dlClients.SelectedIndex > 0)
                            GetPopulatedClientFromWebService();

                        btnClientActions.Value = @"Update";
                        btnClientActions.Visible = true;
                        btnCancelClientUpdate.Disabled = false;
                        break;
                }
            }

            if (dlClients.SelectedIndex == 0 && hiddenClientList.Value == "")
                btnClientActions.Value = "Create New";

            hiddenClientList.Value = hiddenClientList.Value.Replace(dlClients.SelectedItem.Text + "|", "");

            GetToolTips();

            if (Convert.ToBoolean(loggedInUserIsReadOnly))
            {
                //selectProviderList.Enabled = false;

                link_resetRetryList.Visible = false;

                btnClientActions.Visible = false;
                btnCancelClientUpdate.Visible = false;
            }
        }

        public void GetPopulatedClientFromWebService()
        {
            var cDoc = new XmlDocument();

            Session["OriginalClient"] = new Client(dlClients.SelectedValue);

            // Client Xml to session
            ClientXmlToSession(dlClients.SelectedValue);

            cDoc.LoadXml(Session["ClientXML"].ToString());

            var myXml = cDoc.DocumentElement;

            // Set field values
            if (myXml != null)
            {
                var singleId = myXml.SelectSingleNode("_id");
                if (singleId != null)
                    txtClientID.Value = WebUtility.HtmlDecode(singleId.InnerText.Trim());

                var singleNode = myXml.SelectSingleNode("Name");
                if (singleNode != null)
                    txtClientName.Value = WebUtility.HtmlDecode(singleNode.InnerText.Trim());

                txtClientName.Value = txtClientName.Value.Replace("&quot;", "\"");

                txtClientName.Attributes.Add("style", "border-color: #b3b3b3;background-color: #ffffff;background: url('/Images/green-checkmark-symbol.png') no-repeat right center;");

                Session["CurrentClientName"] = txtClientName.Value.Trim();

                #region Set Client logo

                    imgOwnerLogo.Src = myXml.SelectSingleNode("OwnerLogoUrl").InnerText;

                #endregion

                #region Document Templates

                var sbXmlTemplates = new StringBuilder();

                    sbXmlTemplates.Append("<documents>");
                    var documentTemplatesCollection = cDoc.GetElementsByTagName("DocumentTemplates");
                    foreach (XmlElement templateElement in documentTemplatesCollection)
                    {
                        var messageClass = templateElement.ChildNodes[2].InnerText;
                        var messageDesc = templateElement.ChildNodes[3].InnerText;
                        var messageFormat = templateElement.ChildNodes[4].InnerText;
                        var messageFromAddress = templateElement.ChildNodes[5].InnerText;
                        var messageFromName = templateElement.ChildNodes[6].InnerText;

                        sbXmlTemplates.Append("<documenttemplates messageclass='" + messageClass + "' messagedesc='" + messageDesc + "'>");

                        sbXmlTemplates.Append("<messageformat>");
                        sbXmlTemplates.Append("<![CDATA[");

                        var messageBody = messageFormat.Trim();
                        messageBody = messageBody.Replace("A generic message body should be passed here...", "[Details]");
                        messageBody = messageBody.Replace("MAC", "[ClientName]");

                        sbXmlTemplates.Append(messageBody);

                        sbXmlTemplates.Append("]]></messageformat>");

                        sbXmlTemplates.Append("<messagefromaddress>" + messageFromAddress + "</messagefromaddress>");
                        sbXmlTemplates.Append("<messagefromname>" + messageFromName + "</messagefromname>");

                        sbXmlTemplates.Append("</documenttemplates>");
                    }
                    sbXmlTemplates.Append("</documents>");

                    var documentTemplates = myXml.SelectNodes("DocumentTemplates");
                    if (documentTemplates != null)
                        ParseDocumentXmlIntoListBoxes(documentTemplates);

                    // Pass xml docs into hidden field for postback processing
                    hiddenTemplatesXml.Value = sbXmlTemplates.ToString();

                #endregion

                var xmlNode4 = myXml.SelectSingleNode("clientId");
                if (xmlNode4 != null)
                    txtClientID.Value = xmlNode4.InnerText;

                var xmlNode = myXml.SelectSingleNode("Enabled");
                if (xmlNode != null)
                    chkClientEnabled.Checked = Boolean.Parse(xmlNode.InnerText.Trim());

                if (!chkClientEnabled.Checked)
                {
                    chkClientEnabled.ForeColor = Color.Red;
                    chkClientEnabled.Text = @"<span style='color: #ff0000;'>Enable Account</span>";
                }
                else
                {
                    chkClientEnabled.ForeColor = Color.Green;
                    chkClientEnabled.Text = @"Enable Account";
                }

                var xmlCreateDate = myXml.SelectSingleNode("DateCreated");
                if (xmlCreateDate != null)
                {
                    var convertedDate = DateTime.Parse(xmlCreateDate.InnerText.Trim());
                    var createdInfoLink = "<a href='javascript: ";

                    const string createdDateDetails = "&quot;Created info...&quot;";

                    createdInfoLink += "alert(" + createdDateDetails + ");";
                    createdInfoLink += "' style='font-size: 0.875rem !important;'>";
                    createdInfoLink += "Created: ";
                    createdInfoLink += "</a>";

                    clientCreatedDate.InnerHtml = createdInfoLink + convertedDate;
                }

                var xmlModifiedDate = myXml.SelectSingleNode("DateModified");
                if (xmlModifiedDate != null)
                {
                    var convertedDate = DateTime.Parse(xmlModifiedDate.InnerText.Trim());
                    var modifiedInfoLink = "<a href='javascript: ";

                    const string modifiedDateDetails = "&quot;Modified info...&quot;";

                    modifiedInfoLink += "alert(" + modifiedDateDetails + ");";
                    modifiedInfoLink += "' style='font-size: 0.875rem !important;'>";
                    modifiedInfoLink += "Modified: ";
                    modifiedInfoLink += "</a>";

                    clientLastModifiedDate.InnerHtml = modifiedInfoLink + convertedDate;
                }

                var node = myXml.SelectSingleNode("OpenAccessServicesEnabled");
                if (node != null)
                    chkOpenAccessServicesEnabled.Checked = Boolean.Parse(node.InnerText.Trim());

                // This is the flaf we will use to determine if the OAS service status has been changed on the client when posting back updates
                hiddenChkOpenRegistrationStatus.Value = chkOpenAccessServicesEnabled.Checked.ToString();

                // Client Organization
                myXml = (XmlElement)myXml.SelectSingleNode("Organization");
                if (myXml != null)
                {
                    var singleNode1 = myXml.SelectSingleNode("TaxId");
                    if (singleNode1 != null)
                        txtTaxId.Text = singleNode1.InnerText;

                    var xmlNode1 = myXml.SelectSingleNode("Street1");
                    if (xmlNode1 != null)
                        txtStreet1.Text = WebUtility.HtmlDecode(xmlNode1.InnerText.Trim());

                    var node1 = myXml.SelectSingleNode("Street2");
                    if (node1 != null)
                        txtStreet2.Text = WebUtility.HtmlDecode(node1.InnerText.Trim());

                    var selectSingleNode2 = myXml.SelectSingleNode("Unit");
                    if (selectSingleNode2 != null)
                        txtUnit.Text = WebUtility.HtmlDecode(selectSingleNode2.InnerText.Trim());

                    var singleNode2 = myXml.SelectSingleNode("City");
                    if (singleNode2 != null)
                        txtCity.Text = WebUtility.HtmlDecode(singleNode2.InnerText.Trim());

                    var xmlNode2 = myXml.SelectSingleNode("State");
                    if (xmlNode2 != null)
                        dlStates.SelectedValue = xmlNode2.InnerText;

                    txtZipCode.Value = myXml.SelectSingleNode("Zipcode").InnerText;
                    txtZipCode.Attributes.Add("style", "border-color: #b3b3b3;background-color: #ffffff;background: url('/Images/green-checkmark-symbol.png') no-repeat right center;");

                    txtEmail.Value = WebUtility.HtmlDecode(myXml.SelectSingleNode("Email").InnerText.Trim());
                    txtEmail.Attributes.Add("style", "border-color: #b3b3b3;background-color: #ffffff;background: url('/Images/green-checkmark-symbol.png') no-repeat right center;");

                    txtPhone.Value = myXml.SelectSingleNode("Phone").InnerText;
                    txtPhone.Attributes.Add("style", "border-color: #b3b3b3;background-color: #ffffff;background: url('/Images/green-checkmark-symbol.png') no-repeat right center;");

                    spanManageAdminNotificationProvider.InnerHtml = myXml.SelectSingleNode("AdminNotificationProvider").SelectSingleNode("Name").InnerText;
                }
            }

            // Otp settings
            myXml = cDoc.DocumentElement;
            myXml = (XmlElement)myXml.SelectSingleNode("OtpSettings");
            dlOtpLength.SelectedIndex = int.Parse(myXml.SelectSingleNode("Length").InnerText.Trim()) - 4;
            dlMaxRetries.SelectedValue = myXml.SelectSingleNode("MaxRetries").InnerText;
            dlTimeout.SelectedValue = myXml.SelectSingleNode("Timeout").InnerText;

            for (var i = 0; i < 3; i++)
            {
                dlCharacterset.Items[i].Selected = false;
                dlCharacterset.Items[i].Attributes.Add("style", "color: #808080; background: none;");
            }

            var otpcs = myXml.SelectSingleNode("Characterset").InnerText;
            if (MUtils.HasLowerCase(otpcs))
                dlCharacterset.Items[0].Selected = true;

            if (MUtils.HasUpperCase(otpcs))
                dlCharacterset.Items[1].Selected = true;

            if (MUtils.HasNumeric(otpcs))
                dlCharacterset.Items[2].Selected = true;

            // Need to parse the delimited string and add items as required
            selectProviderList.Items.Clear();
            hiddenRetryList.Value = "";
            hiddenProvidersToDelete.Value = "";

            var otpPl = myXml.SelectSingleNode("ProviderList").InnerText;
            if (otpPl.IndexOf("|", StringComparison.Ordinal) > -1)
            {
                var arrProviderRetryList = otpPl.Split(char.Parse(Constants.Dictionary.Keys.ItemSep));
                foreach (var t in arrProviderRetryList)
                {
                    if (!String.IsNullOrEmpty(t))
                    {
                        var providerItem = new ListItem();
                        providerItem.Attributes.Add("id", t);
                        providerItem.Text = t;
                        providerItem.Value = t;

                        if (!Convert.ToBoolean(loggedInUserIsReadOnly))
                            providerItem.Attributes.Add("onclick", "deleteFromRetryList(this)");

                        selectProviderList.Items.Add(providerItem);
                        hiddenRetryList.Value += t + char.Parse(Constants.Dictionary.Keys.ItemSep);
                    }
                }
            }
            else
            {
                var providerItem = new ListItem();
                providerItem.Attributes.Add("id", otpPl);
                providerItem.Text = otpPl;
                providerItem.Value = otpPl;

                if (!Convert.ToBoolean(loggedInUserIsReadOnly))
                    providerItem.Attributes.Add("onclick", "deleteFromRetryList(this)");

                selectProviderList.Items.Add(providerItem);

                hiddenRetryList.Value = otpPl;
            }

            // Client VerificationProvider
            myXml = cDoc.DocumentElement;
            myXml = (XmlElement)myXml.SelectSingleNode("VerificationProviders");
            if (myXml != null)
            {
                // This is not needed since it is all handled inside the popup
            }

            var myMessageProviders = new MessageProvider();

            // Provider info
            var emailProviders = cDoc.SelectNodes("Client/MessageProviders/EmailProviders");
            foreach (XmlNode pNode in emailProviders)
            {
                var myProvider = new ProviderEmail
                {
                    Name = pNode.SelectSingleNode("Name").InnerText,
                    _id = ObjectId.Parse(pNode.SelectSingleNode("_id").InnerText.Trim()),
                    Enabled = Boolean.Parse(pNode.SelectSingleNode("Enabled").InnerText.Trim()),
                    ProviderCharge = Double.Parse(pNode.SelectSingleNode("ProviderCharge").InnerText.Trim()),
                    ClientCharge = Double.Parse(pNode.SelectSingleNode("ClientCharge").InnerText.Trim()),
                    FromEmail = pNode.SelectSingleNode("FromEmail").InnerText,
                    LoginUserName = pNode.SelectSingleNode("LoginUserName").InnerText,
                    LoginPassword = pNode.SelectSingleNode("LoginPassword").InnerText,
                    Port = pNode.SelectSingleNode("Port").InnerText.Trim(),
                    NewlineReplacement = pNode.SelectSingleNode("NewlineReplacement").InnerText,
                    RequiresSsl = Boolean.Parse(pNode.SelectSingleNode("RequiresSsl").InnerText.Trim()),
                    CredentialsRequired = Boolean.Parse(pNode.SelectSingleNode("CredentialsRequired").InnerText.Trim()),
                    IsBodyHtml = Boolean.Parse(pNode.SelectSingleNode("IsBodyHtml").InnerText.Trim()),
                    Server = pNode.SelectSingleNode("Server").InnerText,
                    OtpSubject = pNode.SelectSingleNode("OtpSubject").InnerText,
                    NotficationSubject = pNode.SelectSingleNode("NotficationSubject").InnerText,
                    AdminNotificationOnFailure = Boolean.Parse(pNode.SelectSingleNode("AdminNotificationOnFailure").InnerText.Trim())
                };
                myMessageProviders.EmailProviders.Add(myProvider);
            }
            var smsProviders = cDoc.SelectNodes("Client/MessageProviders/SmsProviders");
            foreach (XmlNode pNode in smsProviders)
            {
                var myProvider = new ProviderSms
                {
                    Name = pNode.SelectSingleNode("Name").InnerText,
                    _id = ObjectId.Parse(pNode.SelectSingleNode("_id").InnerText.Trim()),
                    Enabled = Boolean.Parse(pNode.SelectSingleNode("Enabled").InnerText.Trim()),
                    ProviderCharge = Double.Parse(pNode.SelectSingleNode("ProviderCharge").InnerText.Trim()),
                    ClientCharge = Double.Parse(pNode.SelectSingleNode("ClientCharge").InnerText.Trim()),
                    Url = pNode.SelectSingleNode("Url").InnerText,
                    Sid = pNode.SelectSingleNode("Sid").InnerText,
                    AuthToken = pNode.SelectSingleNode("AuthToken").InnerText,
                    ShortCodeFromNumber = pNode.SelectSingleNode("ShortCodeFromNumber").InnerText,
                    ApiVersion = pNode.SelectSingleNode("ApiVersion").InnerText,
                    Key = pNode.SelectSingleNode("Key").InnerText,
                    Server = pNode.SelectSingleNode("Server").InnerText,
                    Port = pNode.SelectSingleNode("Port").InnerText,
                    Protocol = pNode.SelectSingleNode("Protocol").InnerText,
                    RequiresSsl = Boolean.Parse(pNode.SelectSingleNode("RequiresSsl").InnerText.Trim()),
                    LoginUsername = pNode.SelectSingleNode("LoginUsername").InnerText,
                    LoginPassword = pNode.SelectSingleNode("LoginPassword").InnerText,
                    NewlineReplacement = pNode.SelectSingleNode("NewlineReplacement").InnerText,
                    PhoneNumberFormat = pNode.SelectSingleNode("PhoneNumberFormat").InnerText,
                    VoiceToken = pNode.SelectSingleNode("VoiceToken").InnerText
                };
                myProvider.Name = pNode.SelectSingleNode("Name").InnerText;
                myMessageProviders.SmsProviders.Add(myProvider);
            }

            var voiceProviders = cDoc.SelectNodes("Client/MessageProviders/VoiceProviders");
            foreach (XmlNode pNode in voiceProviders)
            {
                var myProvider = new ProviderVoice
                {
                    Name = pNode.SelectSingleNode("Name").InnerText,
                    _id = ObjectId.Parse(pNode.SelectSingleNode("_id").InnerText.Trim()),
                    Enabled = Boolean.Parse(pNode.SelectSingleNode("Enabled").InnerText.Trim()),
                    ProviderCharge = Double.Parse(pNode.SelectSingleNode("ProviderCharge").InnerText.Trim()),
                    ClientCharge = Double.Parse(pNode.SelectSingleNode("ClientCharge").InnerText.Trim()),
                    ApiVersion = pNode.SelectSingleNode("ApiVersion").InnerText,
                    FromPhoneNumber = pNode.SelectSingleNode("FromPhoneNumber").InnerText,
                    Key = pNode.SelectSingleNode("Key").InnerText,
                    LoginUsername = pNode.SelectSingleNode("LoginUsername").InnerText,
                    LoginPassword = pNode.SelectSingleNode("LoginPassword").InnerText,
                    Port = pNode.SelectSingleNode("Port").InnerText,
                    Protocol = pNode.SelectSingleNode("Protocol").InnerText,
                    RequiresSsl = Boolean.Parse(pNode.SelectSingleNode("RequiresSsl").InnerText.Trim()),
                    Server = pNode.SelectSingleNode("Server").InnerText,
                    Sid = pNode.SelectSingleNode("Sid").InnerText,
                    VoiceToken = pNode.SelectSingleNode("VoiceToken").InnerText
                };
                myProvider.VoiceToken = pNode.SelectSingleNode("VoiceToken").InnerText;
                myMessageProviders.VoiceProviders.Add(myProvider);
            }

            Session["MessageProviders"] = myMessageProviders;

            // Client's myRelationships
            var iNumberOfAdministratorsAssigned = 0;
            Session["ClientAdministrators"] = "";

            var iNumberOfGroupsAssigned = 0;

            var myRelationships = new List<Relationship>();
            var relationships = cDoc.GetElementsByTagName("Relationships");
            foreach (var myRelationship in from XmlNode rNode in relationships
                let selectSingleNode = rNode.SelectSingleNode("MemberId")
                where selectSingleNode != null
                let singleNode = rNode.SelectSingleNode("MemberType")
                where singleNode != null
                let xmlNode = rNode.SelectSingleNode("MemberHierarchy")
                where xmlNode != null
                select new Relationship
            {
                MemberId = ObjectId.Parse(selectSingleNode.InnerXml),
                MemberType = singleNode.InnerXml,
                MemberHierarchy = xmlNode.InnerXml
            })
                {
                myRelationships.Add(myRelationship);

                switch (myRelationship.MemberType)
                {
                    case "Group":
                        if (myRelationship.MemberHierarchy == "Member")
                            iNumberOfGroupsAssigned++;
                        break;
                    case "Administrator":
                        if (myRelationship.MemberHierarchy == "Member")
                            iNumberOfAdministratorsAssigned++;
                        break;
                }
            }
                
            adminCount.InnerHtml = iNumberOfAdministratorsAssigned.ToString(CultureInfo.CurrentCulture);
            groupCount.InnerHtml = iNumberOfGroupsAssigned.ToString(CultureInfo.CurrentCulture);

            Session["Relationships"] = myRelationships;

            GetGlobalProviders(false);
        }

        public List<DocumentTemplate> ParseHiddenTemplateXmlIntoTemplateCollection()
        {
            var templateResponse = new List<DocumentTemplate>();

            var templateXmlToProcess = new XmlDocument();
            templateXmlToProcess.LoadXml(hiddenTemplatesXml.Value.ToString());

            XmlNodeList parentNode = templateXmlToProcess.GetElementsByTagName("documenttemplate");
            foreach (XmlNode childNode in parentNode)
            {
                var newTemplate = new DocumentTemplate();

                newTemplate.MessageClass = childNode["messageclass"].InnerText;
                newTemplate.MessageDesc = childNode["messagedesc"].InnerText;
                newTemplate.MessageFormat = childNode["messageformat"].InnerText;
                newTemplate.MessageFromAddress = childNode["messagefromaddress"].InnerText;
                newTemplate.MessageFromName = childNode["messagefromname"].InnerText;

                templateResponse.Add(newTemplate);
            }

            return templateResponse;
        }

        public void ParseDocumentXmlIntoListBoxes(XmlNodeList documentTemplates)
        {
            var rootItemIndex = 0;

            var childItemIndentLevel = "20px";

            //var documentTemplateCollection = new ArrayList();

            var emailTemplates = new ArrayList();
            var htmlTemplates = new ArrayList();
            var smsTemplates = new ArrayList();
            var voiceTemplates = new ArrayList();

            var emailInitialized = false;
            var htmlInitialized = false;
            var smsInitialized = false;
            var voiceInitialized = false;

            dlMessageTemplates.Items.Clear();

            var defaultItem = new ListItem { Text = @"Choose a Template", Value = @"Choose a Template" };
            defaultItem.Attributes.Add("id", Constants.Strings.DefaultEmptyObjectId);

            dlMessageTemplates.Items.Insert(rootItemIndex, defaultItem);

            rootItemIndex++;

            var selectionText = "";
            var selectionValue = "";

            var emailRoot = new ListItem();
            var htmlRoot = new ListItem();
            var smsRoot = new ListItem();
            var voiceRoot = new ListItem();

            foreach (XmlElement currentTemplate in documentTemplates)
            {
                var tmpClassNode = currentTemplate.SelectNodes("MessageClass");

                var templateClass = tmpClassNode[0].InnerText;

                var tmpClassName = templateClass.Split('-');
                var templateType = tmpClassName[0];

                var itemAttributes = "";

                // Set the root template class items in list
                switch (templateType)
                {
                    case "Email":
                        if (!emailInitialized)
                        {
                            emailRoot.Text = templateType;
                            emailRoot.Value = templateType;

                            emailRoot.Attributes.Add("style", "font-weight: bold;");

                            emailInitialized = true;
                        }
                        foreach (XmlNode currentNode in currentTemplate)
                        {
                            if (currentNode.Name == "MessageDesc")
                            {
                                // Build the current item's attribute collection
                                foreach (XmlElement itemAttribute in currentTemplate.ChildNodes)
                                    itemAttributes += itemAttribute.Name + "=" + itemAttribute.InnerText + "&";

                                selectionText = currentTemplate.ChildNodes[3].InnerText;
                                selectionValue = currentTemplate.ChildNodes[2].InnerText;

                                emailTemplates.Add(selectionText + "^" + selectionValue + "^" + itemAttributes);
                            }
                        }
                        break;

                    case "Html":
                        if (!htmlInitialized)
                        {
                            htmlRoot.Text = templateType;
                            htmlRoot.Value = templateType;

                            htmlRoot.Attributes.Add("style", "font-weight: bold;");

                            htmlInitialized = true;
                        }
                        foreach (XmlNode currentNode in currentTemplate)
                        {
                            if (currentNode.Name == "MessageDesc")
                            {
                                // Build the current item's attribute collection
                                foreach (XmlElement itemAttribute in currentTemplate.ChildNodes)
                                    itemAttributes += itemAttribute.Name + "=" + itemAttribute.InnerText + "&";

                                selectionText = currentTemplate.ChildNodes[3].InnerText;
                                selectionValue = currentTemplate.ChildNodes[2].InnerText;

                                htmlTemplates.Add(selectionText + "^" + selectionValue + "^" + itemAttributes);
                            }
                        }
                        break;

                    case "Sms":
                        if (!smsInitialized)
                        {
                            smsRoot.Text = templateType;
                            smsRoot.Value = templateType;

                            smsRoot.Attributes.Add("style", "font-weight: bold;");

                            smsInitialized = true;
                        }
                        foreach (XmlNode currentNode in currentTemplate)
                        {
                            if (currentNode.Name == "MessageDesc")
                            {
                                // Build the current item's attribute collection
                                foreach (XmlElement itemAttribute in currentTemplate.ChildNodes)
                                    itemAttributes += itemAttribute.Name + "=" + itemAttribute.InnerText + "&";

                                selectionText = currentTemplate.ChildNodes[3].InnerText;
                                selectionValue = currentTemplate.ChildNodes[2].InnerText;

                                smsTemplates.Add(selectionText + "^" + selectionValue + "^" + itemAttributes);
                            }
                        }
                        break;

                    case "Voice":
                        if (!voiceInitialized)
                        {
                            voiceRoot.Text = templateType;
                            voiceRoot.Value = templateType;

                            voiceRoot.Attributes.Add("style", "font-weight: bold;");

                            voiceInitialized = true;
                        }
                        foreach (XmlNode currentNode in currentTemplate)
                        {
                            if (currentNode.Name == "MessageDesc")
                            {
                                // Build the current item's attribute collection
                                foreach (XmlElement itemAttribute in currentTemplate.ChildNodes)
                                    itemAttributes += itemAttribute.Name + "=" + itemAttribute.InnerText + "&";

                                selectionText = currentTemplate.ChildNodes[3].InnerText;
                                selectionValue = currentTemplate.ChildNodes[2].InnerText;

                                voiceTemplates.Add(selectionText + "^" + selectionValue + "^" + itemAttributes);
                            }
                        }
                        break;
                }
            }

            if (emailInitialized)
            {
                emailRoot.Attributes.Add("id", emailRoot.Value);
                emailRoot.Attributes.Add("isroottype", "true");

                dlMessageTemplates.Items.Insert(rootItemIndex, emailRoot);

                rootItemIndex++;

                // Sort the item list
                emailTemplates.Sort();

                // Populate the child elements
                foreach (var templateItem in emailTemplates)
                {
                    var tmpItem = templateItem.ToString().Split('^');

                    var templateLi = new ListItem();
                    templateLi.Text = tmpItem[0];
                    templateLi.Value = tmpItem[1];
                    templateLi.Attributes.Add("id", templateLi.Value);

                    // Process attributes if available
                    if (tmpItem[2] != null)
                    {
                        var tmpAttributes = tmpItem[2].Split('&');
                        foreach (var currentAttribute in tmpAttributes)
                        {
                            if (!String.IsNullOrEmpty(currentAttribute))
                            {
                                var attributeDetails = currentAttribute.Split('=');
                                templateLi.Attributes.Add(attributeDetails[0], attributeDetails[1]);
                                templateLi.Attributes.Add("isroottype", "false");
                            }
                        }
                    }

                    templateLi.Attributes.Add("style", "padding-left: " + childItemIndentLevel + ";");

                    dlMessageTemplates.Items.Insert(rootItemIndex, templateLi);
                    rootItemIndex++;
                }
            }

            if (htmlInitialized)
            {
                htmlRoot.Attributes.Add("id", htmlRoot.Value);
                htmlRoot.Attributes.Add("isroottype", "true");

                dlMessageTemplates.Items.Insert(rootItemIndex, htmlRoot);

                rootItemIndex++;

                // Sort the item list
                htmlTemplates.Sort();
                // Populate the child elements
                foreach (var templateItem in htmlTemplates)
                {
                    var tmpItem = templateItem.ToString().Split('^');

                    var templateLi = new ListItem();
                    templateLi.Text = tmpItem[0];
                    templateLi.Value = tmpItem[1];
                    templateLi.Attributes.Add("id", templateLi.Value);

                    // Process attributes if available
                    if (tmpItem[2] != null)
                    {
                        var tmpAttributes = tmpItem[2].Split('&');
                        foreach (var currentAttribute in tmpAttributes)
                        {
                            if (!String.IsNullOrEmpty(currentAttribute))
                            {
                                var attributeDetails = currentAttribute.Split('=');
                                templateLi.Attributes.Add(attributeDetails[0], attributeDetails[1]);
                                templateLi.Attributes.Add("isroottype", "false");
                            }
                        }
                    }

                    templateLi.Attributes.Add("style", "padding-left: " + childItemIndentLevel + ";");

                    dlMessageTemplates.Items.Insert(rootItemIndex, templateLi);
                    rootItemIndex++;
                }
            }

            if (smsInitialized)
            {
                smsRoot.Attributes.Add("id", smsRoot.Value);
                smsRoot.Attributes.Add("isroottype", "true");

                dlMessageTemplates.Items.Insert(rootItemIndex, smsRoot);

                rootItemIndex++;

                // Sort the item list
                smsTemplates.Sort();

                // Populate the child elements
                foreach (var templateItem in smsTemplates)
                {
                    var tmpItem = templateItem.ToString().Split('^');

                    var templateLi = new ListItem();
                    templateLi.Text = tmpItem[0];
                    templateLi.Value = tmpItem[1];
                    templateLi.Attributes.Add("id", templateLi.Value);

                    // Process attributes if available
                    if (tmpItem[2] != null)
                    {
                        var tmpAttributes = tmpItem[2].Split('&');
                        foreach (var currentAttribute in tmpAttributes)
                        {
                            if (!String.IsNullOrEmpty(currentAttribute))
                            {
                                var attributeDetails = currentAttribute.Split('=');
                                templateLi.Attributes.Add(attributeDetails[0], attributeDetails[1]);
                                templateLi.Attributes.Add("isroottype", "false");
                            }
                        }
                    }

                    templateLi.Attributes.Add("style", "padding-left: " + childItemIndentLevel + ";");

                    dlMessageTemplates.Items.Insert(rootItemIndex, templateLi);
                    rootItemIndex++;
                }
            }

            if (voiceInitialized)
            {
                voiceRoot.Attributes.Add("id", voiceRoot.Value);
                voiceRoot.Attributes.Add("isroottype", "true");

                dlMessageTemplates.Items.Insert(rootItemIndex, voiceRoot);

                rootItemIndex++;

                // Sort the item list
                voiceTemplates.Sort();

                // Populate the child elements
                foreach (var templateItem in voiceTemplates)
                {
                    var tmpItem = templateItem.ToString().Split('^');

                    var templateLi = new ListItem();
                    templateLi.Text = tmpItem[0];
                    templateLi.Value = tmpItem[1];
                    templateLi.Attributes.Add("id", templateLi.Value);

                    // Process attributes if available
                    if (tmpItem[2] != null)
                    {
                        var tmpAttributes = tmpItem[2].Split('&');
                        foreach (var currentAttribute in tmpAttributes)
                        {
                            if (!String.IsNullOrEmpty(currentAttribute))
                            {
                                var attributeDetails = currentAttribute.Split('=');
                                templateLi.Attributes.Add(attributeDetails[0], attributeDetails[1]);
                                templateLi.Attributes.Add("isroottype", "false");
                            }
                        }
                    }

                    templateLi.Attributes.Add("style", "padding-left: " + childItemIndentLevel + ";");

                    dlMessageTemplates.Items.Insert(rootItemIndex, templateLi);
                    rootItemIndex++;
                }
            }
        }

        public void ResetForm()
        {
            spanManageAdminNotificationProvider.InnerHtml = "Manage";

            hiddenRetryList.Value = "";
            hiddenProvidersToDelete.Value = "";

            dlOtpLength.SelectedIndex = 6;

            chkOpenAccessServicesEnabled.Checked = false;
            chkOpenAccessServicesEnabled.Text = @"Enable Open Service?";
            chkOpenAccessServicesEnabled.ForeColor = Color.Gray;

            txtClientID.Value = Constants.Strings.DefaultStaticObjectId;
            _hiddenD.Value = txtClientID.Value;

            txtClientID.Visible = false;

            for (var i = 0; i < 3; i++)
            {
                dlCharacterset.Items[i].Selected = false;
            }

            dlCharacterset.Items[2].Selected = true;

            if (Request.ServerVariables["SERVER_NAME"].ToLower() == "localhost")
            {
                chkClientEnabled.Checked = true;

                txtClientName.Value = @"New Client Name?";
                txtTaxId.Text = @"123-456-789";
                txtStreet1.Text = @"321 My Street";
                txtStreet2.Text = "";
                txtUnit.Text = @"#201";
                txtCity.Text = @"Scottsdale";
                dlStates.SelectedIndex = 3;
                txtZipCode.Value = @"85257";
                txtEmail.Value = @"admin@mycompany.com";
                txtPhone.Value = @"555-555-1212";

                dlTimeout.SelectedIndex = 4;
                dlMaxRetries.SelectedIndex = 1;
                dlOtpLength.SelectedIndex = 2;
            }
            else
            {
                txtClientName.Value = "";
                chkClientEnabled.Checked = true;
                txtTaxId.Text = "";
                txtStreet1.Text = "";
                txtStreet2.Text = "";
                txtUnit.Text = "";
                txtCity.Text = "";
                dlStates.SelectedIndex = 0;
                txtZipCode.Value = "";
                txtEmail.Value = "";
                txtPhone.Value = "";

                dlTimeout.SelectedIndex = 4;
                dlMaxRetries.SelectedIndex = 1;
                dlOtpLength.SelectedIndex = 2;
            }

            selectProviderList.Items.Clear();
        }

        public void SaveClient(string clientId)
        {
            try
            {

                #region Create Client instance and configure base properties

                    if (clientId == cs.DefaultEmptyObjectId)
                        clientId = "";

                    var myClient = new Client(clientId);

                    if (clientId != "" && clientId != cs.DefaultEmptyObjectId)
                    {
                        myClient._id = ObjectId.Parse(txtClientID.Value.Trim());
                    }
                    else
                        myClient._id = ObjectId.GenerateNewId();

                    myClient.ClientId = myClient._id;

                    myClient.DateModified = DateTime.UtcNow;

                #endregion

                #region Organization Info

                    myClient.Name = txtClientName.Value.Trim().Replace("\"", "&quot;");
                    myClient.Enabled = chkClientEnabled.Checked;
                    //myClient.OpenRegistrationEnabled = chkOpenRegistrationEnabled.Checked;

                    myClient.Organization.TaxId = txtTaxId.Text.Trim();
                    myClient.Organization.Street1 = txtStreet1.Text.Trim();
                    myClient.Organization.Street2 = txtStreet2.Text.Trim();
                    myClient.Organization.Unit = txtUnit.Text.Trim();
                    myClient.Organization.City = txtCity.Text.Trim();
                    myClient.Organization.State = ObjectId.Parse(dlStates.SelectedValue);
                    myClient.Organization.Zipcode = txtZipCode.Value.Trim();
                    myClient.Organization.Email = txtEmail.Value.Trim();
                    myClient.Organization.Phone = txtPhone.Value.Trim();
                    //myClient.Organization.Extension = txtExtension.Text.Trim();

                    if (Session["AdminNotificationProvider"] != null)
                        if (Session["AdminNotificationProvider"].ToString() != "")
                            myClient.Organization.AdminNotificationProvider = (ProviderEmail)Session["AdminNotificationProvider"];

                #endregion

                #region Process Relationships

                    myClient.Relationships.Clear();

                    // This needs to come from session from the group popup
                    if (Session["Relationships"] != null)
                    {
                        if (Session["Relationships"].ToString() != "")
                        {
                            var myNewRelationships = (List<Relationship>)Session["Relationships"];

                            foreach (var member in myNewRelationships)
                            {
                                switch (member.MemberType)
                                {
                                    case "Administrator":
                                        // Add the currently logged in user as the first administrator for this client
                                        var adminRelationship = new Relationship
                                        {
                                            MemberId = member.MemberId,
                                            MemberType = "Administrator",
                                            MemberHierarchy = "Member"
                                        };

                                        myClient.Relationships.Add(adminRelationship);

                                        var adminProfile = new UserProfile(member.MemberId.ToString());

                                        var adminFirstName = Security.DecodeAndDecrypt(adminProfile.FirstName, member.MemberId.ToString());
                                        var adminLastName = Security.DecodeAndDecrypt(adminProfile.LastName, member.MemberId.ToString());
                                        var roleId = adminProfile.Roles[0].ToString();

                                        // Look up role name by roleid and use in event logging below
                                        var roleName = MUtils.GetRoleNameByRoleId(roleId);

                                        // Log the admin user that created this Client
                                        var adminEvent = new Event
                                        {
                                            ClientId = myClient._id,
                                            UserId = ObjectId.Parse(loggedInUserId),
                                            EventTypeDesc = Constants.TokenKeys.UserRole + roleName
                                                            + Constants.TokenKeys.ClientName + myClient.Name
                                                            + Constants.TokenKeys.EventGeneratedByName + adminFirstName + " " + adminLastName
                                        };
                                        adminEvent.Create(Constants.EventLog.Assignments.AdminAssignedToClient, null);
                                        break;

                                    case "Group":
                                        // Add the Client to the Group's relationships
                                        var parentGroup = new Group(member.MemberId.ToString());

                                        // Remove all previous Group relationships if there are any
                                        parentGroup.Relationships.RemoveAll(FindRelationshipByObjectId(myClient._id));

                                        var parentRelationship = new Relationship
                                        {
                                            MemberId = myClient._id,
                                            MemberType = "Client",
                                            MemberHierarchy = "Member"
                                        };

                                        parentGroup.Relationships.Add(parentRelationship);
                                        parentGroup.Update();

                                        // Log the Client > Group assignment
                                        var groupEvent = new Event
                                        {
                                            ClientId = myClient._id,
                                            UserId = ObjectId.Parse(loggedInUserId),
                                            EventTypeDesc = Constants.TokenKeys.GroupName + parentGroup.Name
                                                            + Constants.TokenKeys.ClientName + myClient.Name
                                                            + Constants.TokenKeys.EventGeneratedByName + _hiddenH.Value + " " + _hiddenI.Value
                                        };
                                        groupEvent.Create(Constants.EventLog.Assignments.ClientAssignedToGroup, null);

                                        // Add the group relationship to the client
                                        var groupRelationship = new Relationship
                                        {
                                            MemberId = parentGroup._id,
                                            MemberType = "Group",
                                            MemberHierarchy = "Member"
                                        };
                                        myClient.Relationships.Add(groupRelationship);

                                        break;
                                }
                            }
                        }
                    }

                #endregion

                #region Process Otp settings

                    if (Session["VerificationProviders"] != null)
                        myClient.VerificationProviders = (List<VerificationProvider>)Session["VerificationProviders"];
                    else
                        myClient.VerificationProviders = new List<VerificationProvider>();

                    //myClient.RegistrationCompletionUrl = txtRegistrationCompletionUrl.Text.Trim();
                    //myClient.AuthorizedDomain = txtAuthorizedDomain.Text.Trim();

                    // Otp settings
                    myClient.OtpSettings.Length = Convert.ToInt16(dlOtpLength.SelectedValue);

                    // This should be set from the multi-select list box
                    var selectedCharacters = dlCharacterset.Items.Cast<ListItem>().Where(li => li.Selected).Aggregate("", (current, li) => current + li.Value);

                    myClient.OtpSettings.Characterset = selectedCharacters;
                    myClient.OtpSettings.Timeout = Convert.ToInt16(dlTimeout.SelectedValue);
                    myClient.OtpSettings.MaxRetries = Convert.ToInt16(dlMaxRetries.SelectedValue);

                    #endregion

                #region Process OAS service request if needed

                    // If chkOpenAccessServicesEnabled then call the open web service
                    myClient.OpenAccessServicesEnabled = chkOpenAccessServicesEnabled.Checked;
                    //if (hiddenChkOpenRegistrationStatus.Value.ToLower() != myClient.OpenAccessServicesEnabled.ToString().ToLower())
                    if (myClient.OpenAccessServicesEnabled)
                    {
                        // Call OAS service since this state HAS been changed
                        var oasRegistrationResults = RegisterOpenAccessServiceClient(myClient.OpenAccessServicesEnabled, myClient.AuthorizedDomain);

                        if(oasRegistrationResults.Contains("Error"))
                        {
                            // Stop processing update and reflect the error in UI
                            updateMessage.InnerHtml = "There was an error registering Client in AOS service call";
                            updateMessage.Attributes.Add("class", "alert-box warning radius");
                            updateMessage.Visible = true;
                            return;
                        }
                    }

                #endregion

                #region Delete Providers if specified

                    var arrProvidersToDelete = hiddenProvidersToDelete.Value.IndexOf("|", StringComparison.Ordinal) > -1 ? hiddenProvidersToDelete.Value.Split('|') : new[] { hiddenProvidersToDelete.Value };

                #endregion

                #region Handle messaging providers config

                    // Handle ProviderSms Providers
                    if (myClient.MessageProviders.SmsProviders != null)
                    {
                        if (myClient.MessageProviders.SmsProviders.Count > 0)
                        {
                            var smsProviders = myClient.MessageProviders.SmsProviders.ToList();
                            var tmpSmsList = (from currentProvider in smsProviders let providerName = currentProvider.Name.Replace("&nbsp;", "") + " (" + Constants.Strings.Sms + ")" where !arrProvidersToDelete.Contains(providerName) select currentProvider).ToList();
                            myClient.MessageProviders.SmsProviders = tmpSmsList;
                        }
                    }

                    // Handle ProviderEmail Providers
                    if (myClient.MessageProviders.EmailProviders != null)
                    {
                        if (myClient.MessageProviders.EmailProviders.Count > 0)
                        {
                            var emailProviders = myClient.MessageProviders.EmailProviders.ToList();
                            var tmpEmailList = (from currentProvider in emailProviders let providerName = currentProvider.Name.Replace("&nbsp;", "") + " (" + Constants.Strings.Email + ")" where !arrProvidersToDelete.Contains(providerName) select currentProvider).ToList();
                            myClient.MessageProviders.EmailProviders = tmpEmailList;
                        }
                    }

                    // Handle ProviderVoice Providers
                    if (myClient.MessageProviders.VoiceProviders != null)
                    {
                        if (myClient.MessageProviders.VoiceProviders.Count > 0)
                        {
                            var voiceProviders = myClient.MessageProviders.VoiceProviders.ToList();
                            var tmpVoiceList = (from currentProvider in voiceProviders let providerName = currentProvider.Name.Replace("&nbsp;", "") + " (" + Constants.Strings.Voice + ")" where !arrProvidersToDelete.Contains(providerName) select currentProvider).ToList();
                            myClient.MessageProviders.VoiceProviders = tmpVoiceList;
                        }
                    }

                #endregion

                #region Process Provider Retry List

                    var retryListLength = hiddenRetryList.Value.Length;
                    var leadingCharacter = "";
                    var trailingCharacter = "";

                    // Sanitize leading character. Remove "|" if it's first
                    if (hiddenRetryList.Value.Length > 0)
                        leadingCharacter = hiddenRetryList.Value.Substring(0, 1);

                    if (leadingCharacter == "|")
                        myClient.OtpSettings.ProviderList = hiddenRetryList.Value.Substring(1, (retryListLength-1));
                    else
                        myClient.OtpSettings.ProviderList = hiddenRetryList.Value;

                    // Sanitize trailing character. Remove "|" if it's last
                    if (myClient.OtpSettings.ProviderList.Length > 0)
                        trailingCharacter = myClient.OtpSettings.ProviderList.Substring(retryListLength - 1);

                    if (trailingCharacter == "|")
                        myClient.OtpSettings.ProviderList = myClient.OtpSettings.ProviderList.Substring(0, myClient.OtpSettings.ProviderList.Length-1);

                #endregion

                #region Create or Update the client

                    if (String.IsNullOrEmpty(clientId))
                    {
                        // Add the currently logged in user as the first administrator for this client
                        var adminRelationship = new Relationship
                        {
                            MemberId = ObjectId.Parse(loggedInUserId),
                            MemberType = "Administrator",
                            MemberHierarchy = "Member"
                        };

                        myClient.Relationships.Add(adminRelationship);

                        // Log the admin user that created this Client
                        var createEvent = new Event
                        {
                            ClientId = myClient._id,
                            UserId = ObjectId.Parse(loggedInUserId),
                            EventTypeDesc = Constants.TokenKeys.UserRole + _hiddenUserRole.Value
                                            + Constants.TokenKeys.ClientName + myClient.Name
                                            + Constants.TokenKeys.EventGeneratedByName + _hiddenH.Value + " " + _hiddenI.Value
                        };
                        createEvent.Create(Constants.EventLog.Assignments.AdminAssignedToClient, null);

                        #region Process Document Templates

                            myClient.DocumentTemplates = MUtils.GetDefaultDocumentTemplates(false);

                        #endregion

                        // Create default provider and retry list
                        var defaultProvider = new ProviderSms();
                        myClient.MessageProviders.SmsProviders.Add(defaultProvider);

                        myClient.OtpSettings.ProviderList = defaultProvider.Name + " (Sms)";

                        myClient.Create();

                        _updateMsg.Text = @"Client (Created) - " + myClient.Name + "";

                        var clientEvent = new Event
                        {
                            ClientId = myClient._id,
                            UserId = ObjectId.Parse(loggedInUserId),
                            EventTypeDesc = Constants.TokenKeys.UserRole + _hiddenUserRole.Value
                                            + Constants.TokenKeys.ClientName + myClient.Name
                                            + Constants.TokenKeys.EventGeneratedByName + _hiddenH.Value + " " + _hiddenI.Value
                        };
                        clientEvent.Create(Constants.EventLog.Client.Created, null);
                    }
                    else
                    {
                        var sDifferences = MUtils.GetObjectDifferences(Session["OriginalClient"], myClient);

                        var changeEvent = new Event
                        {
                            ClientId = myClient._id,
                            UserId = ObjectId.Parse(loggedInUserId),
                            EventTypeDesc = Constants.TokenKeys.ObjectPropertiesUpdated + sDifferences
                                            + Constants.TokenKeys.ClientName + myClient.Name
                                            + Constants.TokenKeys.EventGeneratedByName + _hiddenH.Value + " " + _hiddenI.Value
                        };
                        changeEvent.Create(Constants.EventLog.Client.Updated, null);

                        if (myClient.DocumentTemplates.Count < 1)
                            myClient.DocumentTemplates = MUtils.GetDefaultDocumentTemplates(false);
                        else
                            myClient.DocumentTemplates = ParseHiddenTemplateXmlIntoTemplateCollection();
                        
                        myClient.Update();

                        isNewClient = false;

                        _updateMsg.Text = @"Client (Updated) - " + myClient.Name + "";
                    }

                #endregion

                GetClientList();

                // The first item is Select a Client
                if (dlClients.Items.Count > 1)
                    dlClients.SelectedValue = myClient._id.ToString();

                Session["ClientXML"] = "";

                GetPopulatedClientFromWebService();

                btnClientActions.Value = @"Update";

                updateMessage.Visible = true;
                updateMessage.Attributes.Add("class", "alert-box success radius");
                updateMessage.InnerHtml = "Successfully updated " + dlClients.SelectedItem.Text;
            }
            catch(Exception ex)
            {
                var mDetails = ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + dlClients.SelectedItem.Text;
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                updateMessage.Visible = true;
                updateMessage.Attributes.Add("class", "alert-box alert radius");
                updateMessage.InnerHtml = exceptionEvent.EventTypeDesc;
                return;
            }
            finally
            {

            }
        }

        public string RegisterOpenAccessServiceClient(bool clientEnabled, string AuthorizedDomain)
        {
            try
            {
                var macServicesUrl = ConfigurationManager.AppSettings["MacServicesUrl"];
                var myReg = new MacRegistration.MacRegistration();
                var oasServiceType = clientEnabled ? "RegisterOasClient" : "DisableOasClient";
                var reply = myReg.RegisterOasClient(
                    macServicesUrl,
                    dlClients.SelectedValue,
                    Constants.Dictionary.Keys.Request + Constants.Dictionary.Keys.KVSep + oasServiceType +
                    Constants.Dictionary.Keys.ItemSep + Constants.Dictionary.Keys.Name + Constants.Dictionary.Keys.KVSep + txtClientName.Value +
                    Constants.Dictionary.Keys.ItemSep + Constants.Dictionary.Keys.FullyQualifiedDomainName + Constants.Dictionary.Keys.KVSep + AuthorizedDomain +
                    Constants.Dictionary.Keys.ItemSep + Constants.Dictionary.Keys.CID + Constants.Dictionary.Keys.KVSep + dlClients.SelectedValue);

                //switch (reply.Replace("", ""))
                //{ //Error=RegisterOasClient Invalid client ID 530F6E8E675C9B1854A6970B
                //    case "OAS Client registered":
                //        // Do nothing, this was successful
                //        break;
                //}

                return reply;
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch(Exception ex)
            {
                var mDetails = ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + dlClients.SelectedItem.Text;
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                updateMessage.Visible = true;
                updateMessage.Attributes.Add("class", "alert-box alert radius");
                updateMessage.InnerHtml = exceptionEvent.EventTypeDesc;
                return ex.ToString();
            }
        }

        static Predicate<ProviderEmail> FindEmailProviderByName(string providerName)
        {
            return provider => provider.Name == providerName;
        }

        static Predicate<ProviderSms> FindSmsProviderByName(string providerName)
        {
            return provider => provider.Name == providerName;
        }

        static Predicate<ProviderVoice> FindVoiceProviderByName(string providerName)
        {
            return provider => provider.Name == providerName;
        }

        static Predicate<Relationship> FindRelationshipByObjectId(ObjectId objectId)
        {
            return relationship => relationship.MemberId == objectId;
        }

        public void GetGlobalProviders(bool newClient)
        {
            var sbResponse = new StringBuilder();
            var doc = new XmlDocument();
            MessageProvider clientsMessageProviders;
            // get current client message providers from session
            try
            {
                if (Session["MessageProviders"] == null)
                    clientsMessageProviders = new MessageProvider();
                else
                    clientsMessageProviders = (MessageProvider)Session["MessageProviders"];
            }
            catch (Exception ex)
            {
                var mDetails = ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + dlClients.SelectedItem.Text;
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                updateMessage.Visible = true;
                updateMessage.Attributes.Add("class", "alert-box alert radius");
                updateMessage.InnerHtml = exceptionEvent.EventTypeDesc;
                return;
            }
            // get Available Message Providers from session or Client Service
            try
            {
                if (Session["AvailableMessageProviders"] == null)
                {
                    if (GetAvailMessageProviderFromService() == false)
                    {
                        Session["AvailableMessageProviders"] = null;
                        if (Master == null)
                        {
                            _updateMsg.Text = @"getGlobalProviders: failed!";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var mDetails = ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + dlClients.SelectedItem.Text;
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                if (GetAvailMessageProviderFromService() == false)
                {
                    updateMessage.Visible = true;
                    updateMessage.Attributes.Add("class", "alert-box alert radius");
                    updateMessage.InnerHtml = exceptionEvent.EventTypeDesc;
                    return;
                }

            }

            var o = Session["AvailableMessageProviders"];
            if (o != null)
                doc.LoadXml(o.ToString());

            // ---- ProviderEmail Providers -------------------
            var providers = doc.GetElementsByTagName("EmailProviders");
            if (providers.Count != 0)
            {
                foreach (XmlNode pNode in providers)
                {
                    var globalEmailProviderName = pNode.SelectSingleNode("Name").InnerText.Replace(" ", "_").Trim();
                    var emailProviderId = pNode.SelectSingleNode("_id").InnerText;

                    sbResponse.Append("<div id='divEmailProviders'>");
                    var clientEmailProvider = clientsMessageProviders.EmailProviders.Find(FindEmailProviderByName(globalEmailProviderName.Trim()));
                    if (clientEmailProvider != null)  // Provider is Selected
                    {
                        emailProviderId = clientEmailProvider._id.ToString();

                        // Checkbox
                        if (Convert.ToBoolean(loggedInUserIsReadOnly))
                        {
                            sbResponse.Append("<input id='" + globalEmailProviderName + " (Email)' type='checkbox' checked='true' disabled='disabled' onclick='javascript: toggleProviderSelected(this, &apos;Email&apos;, &apos;" + emailProviderId + "&apos;);' />");

                            // Edit icon
                            sbResponse.Append("&nbsp;<a href='javascript: editMessageProvider(&quot;" + emailProviderId + "&quot;, &quot;Email&quot;);'>");
                            sbResponse.Append("<img id='img_" + globalEmailProviderName + " (Email)' src='../../Images/icon-edit.png' border='0' class='editIcon' onclick='javascript: editMessageProvider(&apos;" + emailProviderId + "&apos;, &apos;Email&apos;);' />");
                            sbResponse.Append("</a>");

                            // Provider name
                            sbResponse.Append("&nbsp;<span id='span_" + globalEmailProviderName + " (Email)' style='color: #c0c0c0;'>");
                            sbResponse.Append(globalEmailProviderName);
                            sbResponse.Append("&nbsp;</span>");
                        }
                        else
                        {
                            sbResponse.Append("<input id='" + globalEmailProviderName + " (Email)' type='checkbox' checked='true' onclick='javascript: toggleProviderSelected(this, &apos;Email&apos;, &apos;" + emailProviderId + "&apos;);' />");

                            // Edit icon
                            sbResponse.Append("&nbsp;<a href='javascript: editMessageProvider(&quot;" + emailProviderId + "&quot;, &quot;Email&quot;);'>");
                            sbResponse.Append("<img id='img_" + globalEmailProviderName + " (Email)' src='../../Images/icon-edit.png' border='0' class='editIcon' onclick='javascript: editMessageProvider(&apos;" + emailProviderId + "&apos;, &apos;Email&apos;);' />");
                            sbResponse.Append("</a>");

                            // Provider name
                            sbResponse.Append("&nbsp;<span id='span_" + globalEmailProviderName + " (Email)'>");
                            sbResponse.Append("<a id='link_" + globalEmailProviderName + " (Email)' href='javascript: addToProvidersList(&quot;" + globalEmailProviderName + " (Email)&quot;);'>");
                            sbResponse.Append(globalEmailProviderName);
                            sbResponse.Append("</a>");
                            sbResponse.Append("&nbsp;</span>");
                        }
                    }
                    else // Provider Not selected
                    {
                        // Checkbox
                        if (Convert.ToBoolean(loggedInUserIsReadOnly))
                        {
                            sbResponse.Append("<input id='" + globalEmailProviderName + " (Email)' type='checkbox' disabled='disabled' onclick='javascript: toggleProviderSelected(this, &apos;Email&apos;, &apos;" + emailProviderId + "&apos;);' />");

                            // Edit icon
                            sbResponse.Append("&nbsp;<img id='img_" + globalEmailProviderName + " (Email)' src='../../Images/icon-edit-disabled.png' border='0' class='editIcon' />");

                            // Provider name
                            sbResponse.Append("&nbsp;<span id='span_" + globalEmailProviderName + " (Email)' style='color: #c0c0c0; font-size: 13px;'>");
                            sbResponse.Append(globalEmailProviderName);
                            sbResponse.Append("</span>");
                        }
                        else
                        {
                            sbResponse.Append("<input id='" + globalEmailProviderName + " (Email)' type='checkbox' onclick='javascript: toggleProviderSelected(this, &apos;Email&apos;, &apos;" + emailProviderId + "&apos;);' />");

                            // Edit icon
                            sbResponse.Append("&nbsp;<img id='img_" + globalEmailProviderName + " (Email)' src='../../Images/icon-edit-disabled.png' border='0' class='editIcon' />");

                            // Provider name
                            sbResponse.Append("&nbsp;<span id='span_" + globalEmailProviderName + " (Email)' style='color: #c0c0c0; font-size: 13px;'>");
                            sbResponse.Append(globalEmailProviderName);
                            sbResponse.Append("</span>");
                        }
                    }
                    sbResponse.Append("</div>");
                }
                // Add controls to form
                divEmailProviders.InnerHtml = sbResponse.ToString();
                sbResponse.Clear();
            }

            // ---- ProviderSms Providers -------------------
            providers = doc.GetElementsByTagName("SmsProviders");
            if (providers.Count != 0)
            {
                foreach (XmlNode pNode in providers)
                {
                    var globalSmsProviderName = pNode.SelectSingleNode("Name").InnerText.Replace(" ", "_").Trim();
                    var SmsProviderId = pNode.SelectSingleNode("_id").InnerText;

                    sbResponse.Append("<div id='divSmsProviders'>");
                    var clientSmsProvider = clientsMessageProviders.SmsProviders.Find(FindSmsProviderByName(globalSmsProviderName.Trim()));
                    if (clientSmsProvider != null)  // Provider is Selected
                    {
                        SmsProviderId = clientSmsProvider._id.ToString();

                        // Checkbox
                        if (Convert.ToBoolean(loggedInUserIsReadOnly))
                        {
                            sbResponse.Append("<input id='" + globalSmsProviderName + " (Sms)' type='checkbox' checked='true' disabled='disabled' onclick='javascript: toggleProviderSelected(this, &apos;Sms&apos;, &apos;" + SmsProviderId + "&apos;);' />");

                            // Edit icon
                            sbResponse.Append("&nbsp;<a href='javascript: editMessageProvider(&quot;" + SmsProviderId + "&quot;, &quot;Sms&quot;);'>");
                            sbResponse.Append("<img id='img_" + globalSmsProviderName + " (Sms)' src='../../Images/icon-edit.png' border='0' class='editIcon' onclick='javascript: editMessageProvider(&apos;" + SmsProviderId + "&apos;, &apos;Sms&apos;);' />");
                            sbResponse.Append("</a>");

                            // Provider name
                            sbResponse.Append("&nbsp;<span id='span_" + globalSmsProviderName + " (Sms)' style='color: #c0c0c0;'>");
                            sbResponse.Append(globalSmsProviderName);
                            sbResponse.Append("&nbsp;</span>");
                        }
                        else
                        {
                            sbResponse.Append("<input id='" + globalSmsProviderName + " (Sms)' type='checkbox' checked='true' onclick='javascript: toggleProviderSelected(this, &apos;Sms&apos;, &apos;" + SmsProviderId + "&apos;);' />");

                            // Edit icon
                            sbResponse.Append("&nbsp;<a href='javascript: editMessageProvider(&quot;" + SmsProviderId + "&quot;, &quot;Sms&quot;);'>");
                            sbResponse.Append("<img id='img_" + globalSmsProviderName + " (Sms)' src='../../Images/icon-edit.png' border='0' class='editIcon' onclick='javascript: editMessageProvider(&apos;" + SmsProviderId + "&apos;, &apos;Sms&apos;);' />");
                            sbResponse.Append("</a>");

                            // Provider name
                            sbResponse.Append("&nbsp;<span id='span_" + globalSmsProviderName + " (Sms)'>");
                            sbResponse.Append("<a id='link_" + globalSmsProviderName + " (Sms)' href='javascript: addToProvidersList(&quot;" + globalSmsProviderName + " (Sms)&quot;);'>");
                            sbResponse.Append(globalSmsProviderName);
                            sbResponse.Append("</a>");
                            sbResponse.Append("&nbsp;</span>");
                        }
                    }
                    else // Provider Not selected
                    {
                        // Checkbox
                        if (Convert.ToBoolean(loggedInUserIsReadOnly))
                        {
                            sbResponse.Append("<input id='" + globalSmsProviderName + " (Sms)' type='checkbox' disabled='disabled' onclick='javascript: toggleProviderSelected(this, &apos;Sms&apos;, &apos;" + SmsProviderId + "&apos;);' />");

                            // Edit icon
                            sbResponse.Append("&nbsp;<img id='img_" + globalSmsProviderName + " (Sms)' src='../../Images/icon-edit-disabled.png' border='0' class='editIcon' />");

                            // Provider name
                            sbResponse.Append("&nbsp;<span id='span_" + globalSmsProviderName + " (Sms)' style='color: #c0c0c0; font-size: 13px;'>");
                            sbResponse.Append(globalSmsProviderName);
                            sbResponse.Append("</span>");
                        }
                        else
                        {
                            sbResponse.Append("<input id='" + globalSmsProviderName + " (Sms)' type='checkbox' onclick='javascript: toggleProviderSelected(this, &apos;Sms&apos;, &apos;" + SmsProviderId + "&apos;);' />");

                            // Edit icon
                            sbResponse.Append("&nbsp;<img id='img_" + globalSmsProviderName + " (Sms)' src='../../Images/icon-edit-disabled.png' border='0' class='editIcon' />");

                            // Provider name
                            sbResponse.Append("&nbsp;<span id='span_" + globalSmsProviderName + " (Sms)' style='color: #c0c0c0; font-size: 13px;'>");
                            sbResponse.Append(globalSmsProviderName);
                            sbResponse.Append("</span>");
                        }
                    }
                    sbResponse.Append("</div>");
                }
                // Add controls to form
                divSmsProviders.InnerHtml = sbResponse.ToString();
                sbResponse.Clear();
            }

            // ---- ProviderVoice Providers -------------------
            providers = doc.GetElementsByTagName("VoiceProviders");
            if (providers.Count == 0) return;
            foreach (XmlNode pNode in providers)
            {
                var globalVoiceProviderName = pNode.SelectSingleNode("Name").InnerText.Replace(" ", "_").Trim();
                var VoiceProviderId = pNode.SelectSingleNode("_id").InnerText;

                sbResponse.Append("<div id='divVoiceProviders'>");
                var clientVoiceProvider = clientsMessageProviders.VoiceProviders.Find(FindVoiceProviderByName(globalVoiceProviderName.Trim()));
                if (clientVoiceProvider != null)  // Provider is Selected
                {
                    VoiceProviderId = clientVoiceProvider._id.ToString();

                    // Checkbox
                    if (Convert.ToBoolean(loggedInUserIsReadOnly))
                    {
                        sbResponse.Append("<input id='" + globalVoiceProviderName + " (Voice)' type='checkbox' checked='true' disabled='disabled' onclick='javascript: toggleProviderSelected(this, &apos;Voice&apos;, &apos;" + VoiceProviderId + "&apos;);' />");

                        // Edit icon
                        sbResponse.Append("&nbsp;<a href='javascript: editMessageProvider(&quot;" + VoiceProviderId + "&quot;, &quot;Voice&quot;);'>");
                        sbResponse.Append("<img id='img_" + globalVoiceProviderName + " (Voice)' src='../../Images/icon-edit.png' border='0' class='editIcon' onclick='javascript: editMessageProvider(&apos;" + VoiceProviderId + "&apos;, &apos;Voice&apos;);' />");
                        sbResponse.Append("</a>");

                        // Provider name
                        sbResponse.Append("&nbsp;<span id='span_" + globalVoiceProviderName + " (Voice)' style='color: #c0c0c0;'>");
                        sbResponse.Append(globalVoiceProviderName);
                        sbResponse.Append("&nbsp;</span>");
                    }
                    else
                    {
                        sbResponse.Append("<input id='" + globalVoiceProviderName + " (Voice)' type='checkbox' checked='true' onclick='javascript: toggleProviderSelected(this, &apos;Voice&apos;, &apos;" + VoiceProviderId + "&apos;);' />");

                        // Edit icon
                        sbResponse.Append("&nbsp;<a href='javascript: editMessageProvider(&quot;" + VoiceProviderId + "&quot;, &quot;Voice&quot;);'>");
                        sbResponse.Append("<img id='img_" + globalVoiceProviderName + " (Voice)' src='../../Images/icon-edit.png' border='0' class='editIcon' onclick='javascript: editMessageProvider(&apos;" + VoiceProviderId + "&apos;, &apos;Voice&apos;);' />");
                        sbResponse.Append("</a>");

                        // Provider name
                        sbResponse.Append("&nbsp;<span id='span_" + globalVoiceProviderName + " (Voice)'>");
                        sbResponse.Append("<a id='link_" + globalVoiceProviderName + " (Voice)' href='javascript: addToProvidersList(&quot;" + globalVoiceProviderName + " (Voice)&quot;);'>");
                        sbResponse.Append(globalVoiceProviderName);
                        sbResponse.Append("</a>");
                        sbResponse.Append("&nbsp;</span>");
                    }
                }
                else // Provider Not selected
                {
                    // Checkbox
                    if (Convert.ToBoolean(loggedInUserIsReadOnly))
                    {
                        sbResponse.Append("<input id='" + globalVoiceProviderName + " (Voice)' type='checkbox' disabled='disabled' onclick='javascript: toggleProviderSelected(this, &apos;Voice&apos;, &apos;" + VoiceProviderId + "&apos;);' />");

                        // Edit icon
                        sbResponse.Append("&nbsp;<img id='img_" + globalVoiceProviderName + " (Voice)' src='../../Images/icon-edit-disabled.png' border='0' class='editIcon' />");

                        // Provider name
                        sbResponse.Append("&nbsp;<span id='span_" + globalVoiceProviderName + " (Voice)' style='color: #c0c0c0; font-size: 13px;'>");
                        sbResponse.Append(globalVoiceProviderName);
                        sbResponse.Append("</span>");
                    }
                    else
                    {
                        sbResponse.Append("<input id='" + globalVoiceProviderName + " (Voice)' type='checkbox' onclick='javascript: toggleProviderSelected(this, &apos;Voice&apos;, &apos;" + VoiceProviderId + "&apos;);' />");

                        // Edit icon
                        sbResponse.Append("&nbsp;<img id='img_" + globalVoiceProviderName + " (Voice)' src='../../Images/icon-edit-disabled.png' border='0' class='editIcon' />");

                        // Provider name
                        sbResponse.Append("&nbsp;<span id='span_" + globalVoiceProviderName + " (Voice)' style='color: #c0c0c0; font-size: 13px;'>");
                        sbResponse.Append(globalVoiceProviderName);
                        sbResponse.Append("</span>");
                    }
                }
                sbResponse.Append("</div>");
            }
            // Add controls to form
            divVoiceProviders.InnerHtml = sbResponse.ToString();
            sbResponse.Clear();
        }

        protected void btnClientActions_Click(object sender, EventArgs e)
        {
            switch (btnClientActions.Value)
            {
                case "Create New":
                    GetGlobalProviders(true);

                    // Set ClientActions button
                    btnClientActions.Value = @"Save";
                    btnClientActions.Visible = true;
                    btnCancelClientUpdate.Disabled = false;
                    break;

                case "Save":
                    SaveClient("");
                    btnCancelClientUpdate.Disabled = false;
                    break;

                case "Update":
                    SaveClient(dlClients.SelectedValue);
                    btnCancelClientUpdate.Disabled = false;
                    break;
            }
        }

        protected void dlClients_SelectedIndexChanged(object sender, EventArgs e)
        {
            _updateMsg.Text = "";

            //ResetSessionObjects();

            switch (dlClients.SelectedValue)
            {
                case cs.DefaultEmptyObjectId: // Select a Client
                    _hiddenD.Value = cs.DefaultStaticObjectId;
                    hiddenProvidersToDelete.Value = "";
                    spanOrganization.InnerHtml = "Organization";
                    ResetForm();
                    btnClientActions.Visible = true;
                    Tab_EventReporting.Disabled = true;
                    break;

                default:
                    btnClientActions.Value = @"Update";
                    _hiddenD.Value = dlClients.SelectedValue;
                    hiddenProvidersToDelete.Value = "";
                    spanOrganization.InnerHtml = dlClients.SelectedItem.ToString();
                    Tab_EventReporting.Disabled = false;
                    break;
            }
        }

        public void GetStateList()
        {
            var stateList = new MacList("", "TypeDefinitions", "State", "_id,Name");
            foreach (var li in stateList.ListItems.Select(item => new ListItem { Text = item.Attributes["Name"], Value = item.Attributes["_id"] }))
            {
                dlStates.Items.Add(li);
            }
        }

        public void GetClientList()
        {
            dlClients.Items.Clear();

            var clientList = new MacList("", "Client", "", "_id,Name");
            foreach (var li in clientList.ListItems.Select(item => new ListItem { Text = item.Attributes["Name"].Replace("&quot;", "\"").Trim(), Value = item.Attributes["_id"].Trim() }))
            {
                if (li.Text != "")
                {
                    dlClients.Items.Add(li);
                    hiddenClientList.Value += li.Text + Constants.Common.ItemSep;
                }
            }

            var li0 = new ListItem { Text = @"Select a Client (" + (dlClients.Items.Count) + @")", Value = Constants.Strings.DefaultEmptyObjectId };
            dlClients.Items.Insert(0, li0);
        }

        public void GetHiddenClientList(string currentSelectedClient)
        {
            // Clear this out for the current data load
            hiddenClientList.Value = "";

            var clientList = new MacList("", "Client", "", "_id,Name");
            foreach (var li in clientList.ListItems.Select(item => new ListItem { Text = item.Attributes["Name"].Replace("&quot;", "\"").Trim(), Value = item.Attributes["_id"].Trim() }))
            {
                if (li.Text.Trim().ToLower() != "" && !String.Equals(li.Text.Trim(), currentSelectedClient.Trim(), StringComparison.CurrentCultureIgnoreCase))
                {
                    hiddenClientList.Value += li.Text + Constants.Common.ItemSep;
                }
            }
        }

        //public void ResetSessionObjects()
        //{
        //    hiddenProvidersToDelete.Value = "";

        //    Session["MessageProviders"] = new MessageProvider();
        //    Session["Relationships"] = "";
        //    Session["ClientAdministrators"] = "";
        //    Session["ClientXML"] = "";
        //}

        private void ClientXmlToSession(string clientId)
        {
            // Resetting the session since this is proving problematic at the moment
            Session["ClientXML"] = null;

            try
            {
                if (Session["ClientXML"] == null || Session["ClientXML"].ToString() == "")
                {
                    // first time
                    if (GetClientDataFromService()) return;
                    {
                        if (Master == null) return;
                        _updateMsg.Text = @"GetPopulatedClientFromWebService: failed!";
                    }
                }
                else // the session varible has data
                {   // is it for the same client??
                    var doc = new XmlDocument();
                    doc.LoadXml(Session["ClientXML"].ToString());
                    XmlNode cNode = doc.DocumentElement;
                    // Set field values
                    if (cNode != null)
                    {
                        var selectSingleNode = cNode.SelectSingleNode("_id");
                        if (selectSingleNode == null || (selectSingleNode.InnerText == clientId)) return;
                    }
                    // different client
                    if (GetClientDataFromService()) return;
                    if (Master == null) return;
                    _updateMsg.Text = @"GetPopulatedClientFromWebService: failed!";
                }
            }
            catch(Exception ex)
            {
                if (GetClientDataFromService() == false)
                {
                    var mDetails = ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + dlClients.SelectedItem.Text;
                    var exceptionEvent = new Event
                    {
                        EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                    };
                    exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                    updateMessage.Visible = true;
                    updateMessage.Attributes.Add("class", "alert-box alert radius");
                    updateMessage.InnerHtml = exceptionEvent.EventTypeDesc;
                    return;
                }
            }
        }

        private bool GetClientDataFromService()
        {
            var reply = ClientServicesRequest(loggedInUserId, Constants.Dictionary.Values.GetClient, dlClients.SelectedValue);
            if (reply.Item1 == false)
            {
                if (Master == null) return false;
                _updateMsg.Text = @"GetPopulatedClientFromWebService: parse response failed (" + reply.Item2 + @")";
                return false;
            }
            // isolate data from key
            var ret = MUtils.GetIdDataFromRequest(reply.Item2);
            if (ret.Item1 == null)
            {
                if (Master == null) return false;
                _updateMsg.Text = @"GetPopulatedClientFromWebService: parse reply failed!";
                return false;
            }
            // deserialize into xml document
            var cDoc = new XmlDocument();
            try
            {
                var serData = Security.DecodeAndDecrypt(ret.Item2, ret.Item1);
                // test if data can be converted into an xml document
                serData = serData.Replace("<Name>", "<Name><![CDATA[");
                serData = serData.Replace("</Name>", "]]></Name>");

                cDoc.LoadXml(serData);
                Session["ClientXML"] = serData;
                return true;
            }
            catch (Exception ex)
            {
                var mDetails = ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + dlClients.SelectedItem.Text;
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                updateMessage.Visible = true;
                updateMessage.Attributes.Add("class", "alert-box alert radius");
                updateMessage.InnerHtml = exceptionEvent.EventTypeDesc;
                return false;
            }
        }

        private bool GetAvailMessageProviderFromService()
        {
            // Get all available providers
            //var myData = new Dictionary<string, string>();
            var reply = ClientServicesRequest(loggedInUserId, Constants.Dictionary.Values.GetAvailableMessageProviders, null);
            if (reply.Item1 == false)
            {
                if (Master == null) return false;
                _updateMsg.Text = @"getGlobalProviders: parse response failed (" + reply.Item2 + @")";
                return false;
            }
            // isolate data from key
            var ret = MUtils.GetIdDataFromRequest(reply.Item2);
            if (ret.Item1 == null)
            {
                if (Master == null) return false;
                _updateMsg.Text = @"getGlobalProviders: parse response failed!";
                return false;
            }
            var doc = new XmlDocument();
            try
            {
                var serData = Security.DecodeAndDecrypt(ret.Item2, ret.Item1);
                // test if data can be converted into an xml document
                doc.LoadXml(serData);
                Session["AvailableMessageProviders"] = serData;
                return true;
            }
            catch (Exception ex)
            {
                var mDetails = ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + dlClients.SelectedItem.Text;
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                updateMessage.Visible = true;
                updateMessage.Attributes.Add("class", "alert-box alert radius");
                updateMessage.InnerHtml = exceptionEvent.EventTypeDesc;
                return false;

                //_updateMsg.Text = @"getGlobalProviders.Exception: decrypt or load xml failed (" + ex.Message + @")";
            }
            //return false;
        }

        private Tuple<bool, string> ClientServicesRequest(string id, string pRequest, string pCid)
        {
            var requestData = Constants.Dictionary.Keys.Request + Constants.Dictionary.Keys.KVSep + pRequest;
            if (!String.IsNullOrEmpty(pCid))
                requestData += Constants.Dictionary.Keys.ItemSep + Constants.Dictionary.Keys.CID + Constants.Dictionary.Keys.KVSep + pCid;
            else
                requestData += Constants.Dictionary.Keys.ItemSep + Constants.Dictionary.Keys.CID + Constants.Dictionary.Keys.KVSep + id;

            requestData += Constants.Dictionary.Keys.ItemSep + Constants.Dictionary.Keys.LoggedInAdminId + Constants.Dictionary.Keys.KVSep + loggedInUserId;
            requestData += Constants.Dictionary.Keys.ItemSep + Constants.Dictionary.Keys.LoggedInAdminIpAddress + Constants.Dictionary.Keys.KVSep + _userIpAddress.Value;

            var myData = String.Format("Data={0}{1}{2}",
                /*0*/id.Length,
                /*1*/id.ToUpper(),
                /*2*/Security.EncryptAndEncode(requestData, id.ToUpper()));
            /*2Security.EncryptAndEncode(requestData, pCID.ToUpper()));*/
            try
            {
                var dataStream = Encoding.UTF8.GetBytes(myData);
                var request = MacServicesUrl + "/AdminServices/ClientServices.asmx/WsClientServices";
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

                var elemList = xmlDoc.GetElementsByTagName("Error");
                if (elemList.Count != 0)
                {
                    if (elemList.Count > 0)
                    {
                        return new Tuple<bool, string>(false,
                            String.Format("Error: returned from service {0}", elemList[0].InnerXml));
                    }
                }
                else
                {
                    elemList = xmlDoc.GetElementsByTagName("Reply");
                    return new Tuple<bool, string>(true, elemList[0].InnerXml);
                }
            }
            catch (Exception ex)
            {
                var mDetails = ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") + Constants.TokenKeys.ClientName + dlClients.SelectedItem.Text;
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);

                updateMessage.Visible = true;
                updateMessage.Attributes.Add("class", "alert-box alert radius");
                updateMessage.InnerHtml = exceptionEvent.EventTypeDesc;
                return new Tuple<bool, string>(false, "Exception: " + ex.Message);
            }
            return null;
        }

        public static void SortListControl(HtmlSelect control, bool isAscending)
        {
            List<ListItem> collection;

            if (isAscending)
                collection = control.Items.Cast<ListItem>()
                    .Select(x => x)
                    .OrderBy(x => x.Text)
                    .ToList();
            else
                collection = control.Items.Cast<ListItem>()
                    .Select(x => x)
                    .OrderByDescending(x => x.Text)
                    .ToList();

            control.Items.Clear();

            foreach (ListItem item in collection)
                control.Items.Add(item);
        }

        static public string StructureXmlDocToString(XmlDocument doc)
        {
            var sb = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace
            };
            using (var writer = XmlWriter.Create(sb, settings))
            {
                doc.Save(writer);
            }
            return sb.ToString();
        }

        private void GetToolTips()
        {
            // Handle accordian headers
            Tab_Organization.Attributes.Add("title", helpUtils.GetToolTips("5491ed80ead6361c58f5ce03")); // Organization
            Tab_EventReporting.Attributes.Add("title", helpUtils.GetToolTips("5491ee32ead6361c58f5ce08")); // Event Reporting
            Tab_Assignments.Attributes.Add("title", helpUtils.GetToolTips("5491edcbead6361c58f5ce07")); // Assignments
            Tab_MessageProviders.Attributes.Add("title", helpUtils.GetToolTips("5491ee58ead6361c58f5ce09")); // Messaging Providers
            Tab_OTPSettings.Attributes.Add("title", helpUtils.GetToolTips("5491ee7dead6361c58f5ce0a")); // OTP Settings
            Tab_Advertising.Attributes.Add("title", helpUtils.GetToolTips("5491edabead6361c58f5ce05")); // Advertising
            //clientTab7.Attributes.Add("title", helpUtils.GetToolTips("5491eebeead6361c58f5ce0b")); // User Verification
        }
    }
}