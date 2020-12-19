using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.UI.WebControls;
using System.Xml;

using dk = MACServices.Constants.Dictionary.Keys;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using dv = MACServices.Constants.Dictionary.Values;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using tc = TestLib.TestConstants.TestBank;

namespace MACUserApps.Web.Tests.EndUserRegistration
{
    public partial class MacUserAppsWebTeStsEndUserRegistrationRegisterUsersInFile : System.Web.UI.Page
    {
        private const string Test = "FileReg"; 

        private const string NotSelected = "Not Selected";
        private const string SelectClient = "Select Client";
        private const string NoGroups = "No Groups";

        HiddenField _hiddenW;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Master != null)
            {
                _hiddenW = (HiddenField)Master.FindControl("hiddenW");
                _hiddenW.Value = "54a83af2ead6362034d04bc8";
            }

            if (!IsPostBack)
            {
                var request = WebRequest.Create(ConfigurationManager.AppSettings[cfg.MacServicesUrl] + "/Test/GetClients.asmx/WsGetClients");
                request.Method = "Post";
                request.ContentLength = 0;
                var res = request.GetResponse();
                var response = res.GetResponseStream();
                var xmlDoc = new XmlDocument();
                if (response != null) xmlDoc.Load(response);
                ddlClient.Items.Clear();
                {
                    var li = new ListItem();
                    li.Text = li.Value = SelectClient;
                    ddlClient.Items.Add(li);
                }
                var elemList = xmlDoc.GetElementsByTagName("Client");

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
                        li.Value += @"|" + gnode.ChildNodes[0].InnerText;
                    }
                    ddlClient.Items.Add(li);
                }
                //lbGroup.Visible = false;
                ddlGroups.Visible = false;
                SetGroups();
            }
        }

        public void ddlClient_Selected(object sender, EventArgs e)
        {
            lbError.Text = "";
            SetGroups();
            AddToLogAndDisplay("ClientSelected: " + ddlClient.SelectedItem + " " + ddlClient.SelectedItem.Value);
            rbClientRestricted.Checked = false;
            rbGroupRestricted.Checked = false;
            rbOpen.Checked = false;
        }

        private void SetGroups()
        {
            if (ddlClient.SelectedItem.Text.Contains("(Open)"))
                rbOpen.Enabled = true;
            else
                rbOpen.Enabled = false;

            ddlGroups.Visible = false;
            lbGroup.Visible = false;

            ddlGroups.Items.Clear();
            var values = ddlClient.SelectedValue.Split('|');
            if (values.Count() < 2)
            {
                ddlGroups.Items.Add(NoGroups);
                rbGroupRestricted.Enabled = false;
                return;
            }
            var skipcid = true; // first entry is always the clientId
            rbGroupRestricted.Enabled = true;
            foreach (var group in values)
            {
                var grouplist = new ListItem();
                if (skipcid)
                {
                    grouplist.Text = grouplist.Value = NotSelected;
                    skipcid = false;
                }
                else
                {
                    ddlGroups.Visible = true;
                    lbGroup.Visible = true;
                    var name_id = group.Split('=');
                    grouplist.Text = name_id[0];
                    grouplist.Value = name_id[1];
                }
                ddlGroups.Items.Add(grouplist);
            }
        }

        protected void btnProcessFile_Click(object sender, EventArgs e)
        {
            if (FileUploadControl.HasFile)
            {
                FileUploadControl.Enabled = false;
                btnProcessFile.Enabled = false;
                try
                {
                    var path = Server.MapPath("~/");
                    var registrationFileUploadSubFolder = ConfigurationManager.AppSettings[cfg.RegistrationFileUploadSubFolder];
                    var uploadFolderOnServer = path + registrationFileUploadSubFolder;
                    Session["UploadFolder"] = uploadFolderOnServer;
                    var filename = Path.GetFileName(FileUploadControl.FileName);
                    Session["FileName"] = filename;
                    AddToLogAndDisplay("Upload folder:" + uploadFolderOnServer);
                    AddToLogAndDisplay("Upload file:" + filename);
                    AddToLogAndDisplay("Uploading");
                    FileUploadControl.SaveAs(Server.MapPath("~/") + registrationFileUploadSubFolder + "/" + filename);
                    AddToLogAndDisplay("Upload status: File uploaded!");
                }
                catch (Exception ex)
                {
                    AddToLogAndDisplay("Upload status: The file could not be uploaded. The following error occured: " + ex.Message);
                    return;
                }
                try
                {
                    var myReg = new MacRegistration.MacRegistration();
                    var values = ddlClient.SelectedValue.Split(char.Parse(dk.ItemSep));
                    var cid = values[0];

                    var groupid = "";
                    
                    var  regType = dv.ClientRegister;
                    if (rbOpen.Checked)
                        regType = dv.OpenRegister;
                    else if (rbGroupRestricted.Checked)
                    {
                        regType = dv.GroupRegister;
                        if (ddlGroups.SelectedValue == NotSelected)
                        {
                            lbError.Text = @"You selected 'Register with Group' but you did not pick a group!";
                            return;
                        }
                        values = ddlGroups.SelectedValue.Split(char.Parse(dk.ItemSep));
                        groupid = values[0];

                    }
                    var uploadFolder = Session["UploadFolder"].ToString();
                    var o = Session["FileName"];
                    if (o != null)
                    {
                        var fileName = o.ToString();
                        var fileType = Path.GetExtension(fileName).Trim('.');

                        var reply = myReg.EndUserFileRegistration(
                            /*1*/ConfigurationManager.AppSettings[cfg.MacServicesUrl],
                                /*2*/cid,
                                /*3*/groupid,
                                /*4*/regType,
                                /*5*/fileType,
                                /*6*/uploadFolder,
                                /*7*/fileName);
                        AddToLogAndDisplay(reply.Replace("><", ">|<"));
                    }
                }
                catch (Exception ex)
                {
                    AddToLogAndDisplay("|Exception calling service:" + ex.Message);
                }
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
