using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.UI;

using MACBilling;
using MACSecurity;
using MACServices;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

using cfg = MACServices.Constants.WebConfig;

namespace System
{
    public partial class CreateMessageTemplate : Page
    {
        Utils myUtils = new Utils();

        public string adminUserId = "";
        public string adminFirstName = "";
        public string adminLastName = "";

        public StringBuilder sbResponse = new StringBuilder();

        public UserProfile adminProfile;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Deny access if user request not logged in
            var isAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
            if (!isAuthenticated)
            {
                Response.Write("Hello world!");
                Response.End();
            }

            divDetailInfo.Visible = false;
            divFromInfo.Visible = false;
            btnCreate.Visible = false;

            if (Request["userId"] != null)
            {
                adminUserId = Request["userId"].ToString();

                adminUserId = MACSecurity.Security.DecodeAndDecrypt(adminUserId, Constants.Strings.DefaultClientId);

                adminProfile = new UserProfile(adminUserId);

                adminFirstName = MACSecurity.Security.DecodeAndDecrypt(adminProfile.FirstName, adminUserId);
                adminLastName = MACSecurity.Security.DecodeAndDecrypt(adminProfile.LastName, adminUserId);
            }

            if (IsPostBack) // Run the process
            {
                try
                {
                    switch(dlMessageType.Value)
                    {
                        case "Email":
                            link_help.HRef = "javascript: NavigateTopicPopup('5547fa74a6e10b18d47f6e85');";
                            break;

                        case "Sms":
                            link_help.HRef = "javascript: NavigateTopicPopup('5547fa95a6e10b18d47f6e88');";
                            break;

                        case "Voice":
                            link_help.HRef = "javascript: NavigateTopicPopup('5547fab4a6e10b18d47f6e8b');";
                            break;

                        default:
                            link_help.HRef = "javascript: NavigateTopicPopup('553ff491a6e10b006c0cd085');";
                            break;
                    }

                    var mongoCollection = myUtils.mongoDBConnectionPool.GetCollection("TypeDefinitions");

                    switch(hiddenAA.Value)
                    {
                        case "GetLastTemplateNumber":
                            // Get all templates. Loop through each and determine last highest number for message type
                            // Use this to increment the MessageClass number
                            var IsProcessed = false;

                            if (dlMessageType.SelectedIndex > 0)
                            {
                                var addendumQuery = Query.Matches("MessageClass", dlMessageType.Value + "-.*");
                                var sortBy = SortBy.Descending("MessageClass");

                                var templateResult = myUtils.mongoDBConnectionPool.GetCollection("TypeDefinitions").FindAs<DocumentTemplate>(addendumQuery).SetSortOrder(sortBy);
                                foreach (DocumentTemplate currentTemplate in templateResult)
                                {
                                    if (!IsProcessed)
                                    {
                                        var tmpVal = currentTemplate.MessageClass.Split('-');
                                        spanClassName.InnerHtml = dlMessageType.Value + "-" + (Convert.ToInt16(tmpVal[1])+1);
                                        IsProcessed = true;
                                    }
                                }

                                divDetailInfo.Visible = true;
                                btnCreate.Visible = true;

                                // Only show from info is email template
                                if(dlMessageType.SelectedIndex == 1)
                                    divFromInfo.Visible = true;
                                else
                                    divFromInfo.Visible = false;

                                //// Validate document templates. This will add any that are missing
                                //var validatedTemplates = myUtils.ValidateDocumentTemplates(myClient.DocumentTemplates);
                                //DocumentTemplates = (List<DocumentTemplate>)validatedTemplates[1];

                                //// If there were any missing, persist the updated collection to the db for next use
                                //if ((bool)validatedTemplates[0] == true)
                                //    this.Update();
                            }
                            else
                            {
                                divDetailInfo.Visible = false;
                                btnCreate.Visible = false;
                                spanClassName.InnerHtml = "";
                                txtDesc.Text = "";
                                txtDetails.Value = "";
                                txtFromAddress.Text = "";
                                txtFromName.Text = "";
                            }
                            break;

                        case "SaveTemplate":
                            DocumentTemplate myTemplate = new DocumentTemplate();

                            myTemplate.MessageClass = spanClassName.InnerHtml;
                            myTemplate.MessageDesc = txtDesc.Text;
                            myTemplate.MessageFormat = txtDetails.Value;

                            if (dlMessageType.SelectedIndex == 1)
                            {
                                myTemplate.MessageFromAddress = txtFromAddress.Text;
                                myTemplate.MessageFromName = txtFromName.Text;
                            }

                            // Insert the new template into the db
                            var templateCol = myUtils.mongoDBConnectionPool.GetCollection("TypeDefinitions");
                            templateCol.Insert(myTemplate);

                            btnCancel.Disabled = true;
                            btnCreate.Disabled = true;

                            // Add the template to all clients
                            var clients = myUtils.mongoDBConnectionPool.GetCollection("Client").FindAllAs<Client>();
                            foreach (Client currentClient in clients)
                            {
                                currentClient.DocumentTemplates.Add(myTemplate);
                                currentClient.Update();

                                // Log event
                                //Event myEvent = new Event();
                            }

                            ClientScript.RegisterStartupScript(typeof(Page), "closePage", "<script type='text/JavaScript'>callParentDocumentFunction();</script>");
                            break;

                        default:

                            break;
                    }
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch(Exception ex)
                {
                    var errMsg = ex.ToString();
                }
            }
            else // 
            {

            }
        }
    }
}