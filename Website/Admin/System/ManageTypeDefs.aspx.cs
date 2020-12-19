using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;

using MACSecurity;
using cs = MACServices.Constants.Strings;

namespace MACUserApps.Web.Tests.TypeDefs
{
    public partial class MacUserAppsWebTestsTypeDefsManageTypeDefs : System.Web.UI.Page
    {

        public static string MacServicesUrl = ConfigurationManager.AppSettings["MacServicesUrl"];
        private const string ManageTypeDefsService = "/AdminServices/ManageTypeDefsService.asmx/WsManageTypeDefsService";

        protected void Page_Load(object sender, EventArgs e)
        {
            // Deny access if user request not logged in
            var isAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
            if (!isAuthenticated)
            {
                Response.Write("Hello world!");
                Response.End();
            }

            if (!IsPostBack)
            {
                ddlSType.Items.Clear();
                var ln = new ListItem();
                ln.Text = ln.Value = @"Select Type";
                ddlSType.Items.Add(ln);

                var result = SendRequestToManageTypeDefsService("Request:GetListOfTypes");
      
                var typelist = result.GetElementsByTagName("TypeDefinition");
                foreach(XmlNode type in typelist)
                {
                    var li = new ListItem();
                    li.Text = li.Value = type.InnerText;
                    ddlSType.Items.Add(li);
                }
            } 
            else
            {
                if (String.IsNullOrEmpty(hiddenT.Value) == false)
                {
                    if (String.IsNullOrEmpty(HiddenChangeList.Value.Trim('|')) == false)
                    {
                        var sbError = new StringBuilder();
                        sbError.Append(hiddenT.Value + ": ");
                        if ((hiddenT.Value == "NewTypeByName") || (hiddenT.Value == "UpdateTypeByName"))
                        {
                            var sbRequest = new StringBuilder();
                            sbRequest.Append("Request:" + hiddenT.Value);
                            sbRequest.Append("|TypeDef:" + ddlSType.SelectedValue);
                            sbRequest.Append("|TypeDefName:" + ddlSItem.SelectedValue);
                            // cleanup changedfield list
                            string[] changedfields = (HiddenChangeList.Value.Trim('|')).Split('|');
                            // add to request to service
                            foreach (string fieldtochange in changedfields)
                                sbRequest.Append("|" + fieldtochange);
                            // update the TypeDefination by name, returns the updated list
                            var result = SendRequestToManageTypeDefsService(sbRequest.ToString());
                            var elemList = result.GetElementsByTagName("Error");
                            if (elemList.Count != 0)
                            {
                                sbError.Append(String.Format("Error: returned from service {0}", elemList[0].InnerXml));
                                lbError.Text = sbError.ToString();
                                lbError.Visible = true;
                                lbResult.Visible = false;
                            }
                            else
                            {
                                // refresh Session variable 
                                Session["TypeDefList"] = result.OuterXml;

                                lbResult.Text = @"TypeDefination" + ddlSType.SelectedValue +
                                                @"." + ddlSItem.SelectedValue + @" action successful.";
                                lbResult.Visible = true;
                                lbError.Visible = false;
                            }
                        }
                        else if (hiddenT.Value.ToLower() != "cancel")
                        {
                            sbError.Append("Request error[" + hiddenT.Value + "]");
                            lbError.Text = sbError.ToString();
                            lbError.Visible = true;
                            lbResult.Visible = false;
                        }
                    }
                    // clear request
                    hiddenT.Value = "";
                    HiddenChangeList.Value = "";
                    // clear selected list and hide
                    //lbItem.Visible = false;
                    ddlSItem.Visible = false;
                    ddlSItem.Items.Clear();
                    var ln = new ListItem();
                    ln.Text = ln.Value = @"Select Item";
                    ddlSItem.Items.Add(ln);
                    // reset typedef ddl to select
                    ddlSType.SelectedValue = @"Select Type";
                    // clear table
                    divtable.InnerHtml = "";
                }
            }
        }

        public void dldSType_Changed(object sender, EventArgs e)
        {
            divtable.InnerHtml = divaction.InnerHtml = " ";
            if (ddlSType.SelectedValue == "Select Type")
            {
                //lbItem.Visible = false;
                ddlSItem.Visible = false;
                ddlSItem.Items.Clear();
                var ln = new ListItem();
                ln.Text = ln.Value = @"???";
                ddlSItem.Items.Add(ln);
            }
            else
            {
                //lbItem.Visible = true;
                ddlSItem.Visible = true;
                ddlSItem.Items.Clear();
                var ln = new ListItem();
                ln.Text = ln.Value = @"Select Item";
                ddlSItem.Items.Add(ln);
                lbResult.Text = "";

                var result = SendRequestToManageTypeDefsService(
                    "Request:GetListByName|Name:" + ddlSType.SelectedValue);
                Session["TypeDefList"] = result.OuterXml;

                var typelist = result.GetElementsByTagName(ddlSType.SelectedValue);
                foreach(XmlNode type in typelist)
                {
                    var li = new ListItem();
                    if (type.Attributes != null)
                        li.Text = li.Value = 
                            // get name in the clear
                            Security.DecodeAndDecrypt(type.Attributes["Name"].Value, cs.DefaultEmptyObjectId);
                    ddlSItem.Items.Add(li);
                }
            }
        }

        public void dldSItem_Changed(object sender, EventArgs e)
        {
            if (ddlSItem.SelectedValue != "Select Item")
            {
                var sTbl = new StringBuilder();
                var sAct = new StringBuilder();
                var ss = Session["TypeDefList"].ToString();
                var myXml = new XmlDocument();
                myXml.LoadXml(ss);

                XmlNodeList typelist = myXml.GetElementsByTagName(ddlSType.SelectedValue);
                foreach (XmlNode type in typelist)
                {
                    // find the typeDefination.Name that was selected
                    if (type.Attributes != null && ddlSItem.SelectedItem.Text == 
                        // get the name in the clear
                        Security.DecodeAndDecrypt(type.Attributes["Name"].Value, cs.DefaultEmptyObjectId))
                    {
                        Session["ItemDisplayed"] = type.InnerXml;
                        sTbl.Append("<table class='gridtable'>");
                        var atts = type.Attributes;

                        // loop through attributes create labels and textbox for each
                        foreach (XmlAttribute att in atts)
                        {
                            // get the value in the clear
                            var value = Security.DecodeAndDecrypt(att.Value, cs.DefaultEmptyObjectId);
                            sTbl.Append("    <tr>");
                            sTbl.Append("        <td>");
                            sTbl.Append("             " + att.Name);
                            sTbl.Append("        </td>");
                            sTbl.Append("        <td>");
                            sTbl.Append("             <input id='txt" + att.Name +
                                        "' style='width:300px' type='text' onkeyup='javascript: TypeDef_onchangeHandler(this);' value='" + value + "' />");
                            sTbl.Append("        </td>");
                            sTbl.Append("    </tr>");
                        }
                        sTbl.Append("</table>");
                        divtable.InnerHtml = sTbl.ToString();

                        sAct.Append("<br />");
                        sAct.Append("    Action:   ");
                        sAct.Append("<input type='submit' id='TypeDefs_btnUpdate' runat='server' value='Update' onclick='javascript: TypeDefs_btnUpdate_onclickHandler();' />");
                        sAct.Append("   ");
                        sAct.Append("<input type='submit' id='TypeDefs_btnCreateNew' runat='server' value='Create New' onclick='javascript: TypeDefs_btnCreateNew_onclickHandler();' />");
                        sAct.Append("   ");
                        sAct.Append("<input type='submit' id='TypeDefs_btnCancel' runat='server' value='Cancel' onclick='javascript: TypeDefs_btnCancel_onclickHandler();' />");
                        divaction.InnerHtml = sAct.ToString();
                    }
                }
            }
        }

        private XmlDocument SendRequestToManageTypeDefsService(string request)
        {
            var id = cs.DefaultClientId.ToUpper();
            var data = String.Format("data={0}{1}{2}", id.Length, id, Security.EncryptAndEncode(request, id));
            try
            {
                byte[] dataStream = Encoding.UTF8.GetBytes(data);
                WebRequest webRequest = WebRequest.Create(MacServicesUrl + ManageTypeDefsService);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.ContentLength = dataStream.Length;
                Stream newStream = webRequest.GetRequestStream();
                // Send the data.
                newStream.Write(dataStream, 0, dataStream.Length);
                newStream.Close();
                WebResponse res = webRequest.GetResponse();
                Stream response = res.GetResponseStream();
                var xmlDoc = new XmlDocument();
                if (response != null) xmlDoc.Load(response);
                return xmlDoc;
            }
            catch (Exception ex)
            {
                var xmlDoc = new XmlDocument();
                lbError.Text = ex.Message;
                lbError.Visible = true;
                return xmlDoc;
            }
        }

        public void btnExit_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Default.aspx");
        }
    }
}