using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;

using MACSecurity;
using MACServices;
using dk = MACServices.Constants.Dictionary.Keys;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using dv = MACServices.Constants.Dictionary.Values;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;

using Newtonsoft.Json;

namespace MACUserApps.Web.Tests.UserVerification
{
    public partial class MacUserAppsWebTestsUserVerificationUserVerification : System.Web.UI.Page
    {
        private const string Test = "UserVer";
        private const string PathTestUsers = "~/MACUserApps/Web/Tests/UserTestFiles/TestUsers.txt";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                AddToLogAndDisplay("btnUserVerification Click");
                try
                {
                    var request = WebRequest.Create(ConfigurationManager.AppSettings[cfg.MacServicesUrl] +
                                            TestLib.TestConstants.GetTestClientsInfoUrl);
                    request.Method = "Post";
                    request.ContentLength = 0;
                    var res = request.GetResponse();
                    var response = res.GetResponseStream();
                    var xmlDoc = new XmlDocument();
                    if (response != null) xmlDoc.Load(response);
                    var elemList = xmlDoc.GetElementsByTagName("Client");

                    foreach (XmlNode myClientNode in elemList)
                    {
                        var li = new ListItem
                        {
                            Text = myClientNode.ChildNodes[0].InnerXml,
                            Value = myClientNode.ChildNodes[1].InnerXml
                        };
                        ddlClient.Items.Add(li);
                    }
                }
                catch
                {
                    lbError.Text = @"GetClientsService service error";
                    lbError.Visible = true;
                }
            }
        }

        public void lbFormFill_Click(object sender, EventArgs e)
        {
            ClearForm();
            string testUsersFile = Server.MapPath(PathTestUsers);
            if (String.IsNullOrEmpty(txtTestUser.Text) == false)
            {
                if (File.Exists(testUsersFile) == false)
                {
                    lbError.Text = @" No TestUser File, I can't go on!";
                    lbError.Visible = true;
                    return;
                }
                string line;
                var file = new StreamReader(testUsersFile);
                while ((line = file.ReadLine()) != null)
                {
                    if (line.StartsWith(txtTestUser.Text + ":"))
                        break;
                }
                file.Close();
                if (String.IsNullOrEmpty(line)) return;

                line = line.Replace(txtTestUser.Text + ":", "");
                var flds = line.Split('|');
                foreach(var fld in flds)
                {
                    var kv = fld.Split('=');
                    switch(kv[0])
                    {
                        case dkui.FirstName:
                            txtFirstName.Text = kv[1];
                            break;
                        case dkui.LastName:
                            txtLastName.Text = kv[1];
                            break;
                        case dkui.PhoneNumber:
                            txtMPhoneNo.Text = kv[1];
                            break;
                        case dkui.EmailAddress:
                            txtEmailAdr.Text = kv[1];
                            break;
                        case dkui.DOB:
                            txtDOB.Text = kv[1];
                            break;
                        case dkui.SSN4:
                            txtSSN4.Text = kv[1];
                            break;
                        case dkui.Street:
                            txtAdr.Text = kv[1];
                            break;
                        case dkui.Unit:
                            txtUnit.Text = kv[1];
                            break;
                        case dkui.City:
                            txtCity.Text = kv[1];
                            break;
                        case dkui.State:
                            txtState.Text = kv[1];
                            break;
                        case dkui.ZipCode:
                            txtZipCode.Text = kv[1];
                            break;
                        case dkui.DriverLic:
                            txtDriverLic.Text = kv[1];
                            break;
                        case dkui.DriverLicSt:
                            txtDriverLicSt.Text = kv[1];
                            break;
                        case dkui.UID:
                            txtUid.Text = kv[1];
                            break;
                    }
                }
                txtTestUser.Text = "";
            }
        }

        private void ClearForm()
        {
            txtFirstName.Text = txtLastName.Text = txtMPhoneNo.Text = txtEmailAdr.Text = "";
            txtDOB.Text = txtSSN4.Text = txtAdr.Text = txtUnit.Text = txtCity.Text = "";
            txtState.Text = txtZipCode.Text = txtDriverLic.Text = txtDriverLicSt.Text = txtUid.Text = "";
        }

        public void ddlClient_Selected(object sender, EventArgs e)
        {

        }

        protected void btnUseLNS_Click(object sender, EventArgs e)
        {
            Go(Constants.LexisNexis.UserVerificationLexisNexisService);
        }

        protected void btnUseWPP_Click(object sender, EventArgs e)
        {
            Go(Constants.WhitePagesPro.UserVerificationWhitePagesService);
        }

        protected void Go(string service)
        {
            var cd = ddlClient.SelectedValue.Split('|');
            var cid = cd[0];

            lbError.Visible = false;
            lbError.Text = "";
            var rd = new Dictionary<string, string>();
            if (String.IsNullOrEmpty(txtFirstName.Text))
            {
                lbError.Text = @"First Name required!";
                lbError.Visible = true;
                return;
            }
            rd.Add(dkui.FirstName, txtFirstName.Text);
            if (String.IsNullOrEmpty(txtLastName.Text))
            {
                lbError.Text = @"Last Name required!";
                lbError.Visible = true;
                return;
            }
            rd.Add(dkui.LastName, txtLastName.Text);
            if (String.IsNullOrEmpty(txtMPhoneNo.Text))
            {
                lbError.Text = @"Mobile Phone Required!";
                lbError.Visible = true;
                return;
            }
            rd.Add(dkui.PhoneNumber, txtMPhoneNo.Text);
            if (String.IsNullOrEmpty(txtEmailAdr.Text))
            {
                lbError.Text = @"email address required!";
                lbError.Visible = true;
                return;
            }
            rd.Add(dkui.EmailAddress, txtEmailAdr.Text);

            if (!String.IsNullOrEmpty(txtDOB.Text))
                rd.Add(dkui.DOB, txtDOB.Text);

            if (!String.IsNullOrEmpty(txtSSN4.Text))
                rd.Add(dkui.SSN4, txtSSN4.Text);

            if (!String.IsNullOrEmpty(txtUid.Text))
                rd.Add(dkui.UID, txtUid.Text);

            if (!String.IsNullOrEmpty(txtAdr.Text))
                rd.Add(dkui.Street, txtAdr.Text);

            if (!String.IsNullOrEmpty(txtUnit.Text))
                rd.Add(dkui.Unit, txtUnit.Text);

            if (!String.IsNullOrEmpty(txtCity.Text))
                rd.Add(dkui.City, txtCity.Text);

            if (!String.IsNullOrEmpty(txtState.Text))
                rd.Add(dkui.State, txtState.Text);

            if (!String.IsNullOrEmpty(txtZipCode.Text))
                rd.Add(dkui.ZipCode, txtZipCode.Text);

            if (!String.IsNullOrEmpty(txtDriverLic.Text))
            {
                rd.Add(dkui.DriverLic, txtDriverLic.Text);
                rd.Add(dkui.DriverLicSt, txtDriverLicSt.Text);
            }
            SendRequestToLn(ConfigurationManager.AppSettings["MacServicesUrl"] + service, cid, rd);
        }

        private void SendRequestToLn(string url, string cid, Dictionary<string, string> dict)
        {
            var id = cid.ToUpper();
            var data = String.Format("data={0}{1}{2}", id.Length, id, Security.EncryptAndEncode(
                JsonConvert.SerializeObject(dict), 
                id));
            try
            {
                byte[] dataStream = Encoding.UTF8.GetBytes(data);
                var request = url;
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
                AddToLogAndDisplay("|" + System.Xml.Linq.XDocument.Parse(xmlDoc.OuterXml));
                ProcessReqponse(xmlDoc);
            }
            catch (Exception ex)
            {
                AddToLogAndDisplay(ex.Message);
            }
        }

        private void ProcessReqponse(XmlDocument xmlDoc)
        {
            var nl = xmlDoc.GetElementsByTagName("ComprehensiveVerificationIndex");
            txtComprehensiveVerificationIndex.Text = nl.Count < 1 ? @"?" : nl[0].InnerText;

            nl = xmlDoc.GetElementsByTagName("AdditionalScore1");
            txtAdditionalScore1.Text = nl.Count < 1 ? @"?" : nl[0].InnerText;

            nl = xmlDoc.GetElementsByTagName("AdditionalScore2");
            txtAdditionalScore2.Text = nl.Count < 1 ? @"?" : nl[0].InnerText;

            nl = xmlDoc.GetElementsByTagName("NameAddressSSNSummary");
            txtNameAddressSSNSummary.Text = nl.Count < 1 ? @"?" : nl[0].InnerText;

            nl = xmlDoc.GetElementsByTagName("DOBMatchLevel");
            txtDOBMatchLevel.Text = nl.Count < 1 ? @"?" : nl[0].InnerText;

            nl = xmlDoc.GetElementsByTagName("NameAddressPhone");
            if (nl.Count < 1)
                txtNameAddressPhoneSummary.Text = @"?";
            else
            {
                XmlNode node = nl[0].FirstChild;
                txtNameAddressPhoneSummary.Text = node.InnerText;
            }

            var ri = "";
            nl = xmlDoc.GetElementsByTagName("RiskIndicator");
            foreach (XmlNode n in nl)
            {
                XmlElement xmlElement = n["RiskCode"];
                if (xmlElement == null) continue;
                var rc = xmlElement.InnerText;
                XmlElement element = n["Description"];
                if (element == null) continue;
                var rd = element.InnerText;
                ri += rc + ") " + rd + Environment.NewLine;
            }
            txtRiskIndicators.Text = ri;
        }

        private void AddToLogAndDisplay(string textToAdd)
        {
            var newlog = String.Format("{0}|{1}.{2}", Session["LogText"], Test, textToAdd);
            Session["LogText"] = newlog;
            tbLog.Text = newlog.Replace("|", Environment.NewLine);
        }

        public void btnExit_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Default.aspx");
        }
    }
}