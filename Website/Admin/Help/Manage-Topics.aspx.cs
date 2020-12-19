using System;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

using MACServices;
using MongoDB.Bson;
using MongoDB.Driver.Builders;

using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using cfgcs = MACServices.Constants.WebConfig.ConnectionStringKeys;

namespace Admin.Help
{
    public partial class ManageTopics : System.Web.UI.Page
    {
        public HelpUtils myUtils = new HelpUtils();
        public Utils coreUtils = new Utils();

        HiddenField _hiddenE;
        HiddenField _hiddenH;
        HiddenField _hiddenI;
        HiddenField _hiddenD;
        HiddenField _hiddenAA;
        HiddenField _userIpAddress;
        HiddenField _hiddenUserRole;

        StringBuilder sbDocumentTemplate = new StringBuilder();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Deny access if user request not logged in
            var isAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
            if (!isAuthenticated)
            {
                Response.Write("Hello world!");
                Response.End();
            }

            if (Page.Master != null)
            {
                if (Master != null)
                {
                    _hiddenE = (HiddenField)Page.Master.FindControl("hiddenE");
                    _hiddenH = (HiddenField)Page.Master.FindControl("hiddenH");
                    _hiddenI = (HiddenField)Page.Master.FindControl("hiddenI");
                    _hiddenD = (HiddenField)Page.Master.FindControl("hiddenD");
                    _hiddenAA = (HiddenField)Page.Master.FindControl("hiddenT");
                    _hiddenUserRole = (HiddenField)Page.Master.FindControl("hiddenL");
                    _userIpAddress = (HiddenField)Master.FindControl("hiddenM");
                }
            }

            //sbDocumentTemplate.Append("<link href='../../App_Themes/CSS/Admin.css' rel='stylesheet' />");
            sbDocumentTemplate.Append("<table class='HelpTable_Edit'>");
            sbDocumentTemplate.Append("	<tbody>");
            sbDocumentTemplate.Append("		<tr>");
            sbDocumentTemplate.Append("			<td class='HelpTableMainImageContainer_Edit' colspan='2' style='text-align: left !important;'>");
            sbDocumentTemplate.Append("                Insert header image if needed or delete this text...");
            sbDocumentTemplate.Append("			</td>");
            sbDocumentTemplate.Append("		</tr>");

            // Content row
            sbDocumentTemplate.Append("		<tr>");
            sbDocumentTemplate.Append("			<td class='HelpTableLeftColumnContainer_Edit' style='text-align: right !important; vertical-align: top !important;'>");
            sbDocumentTemplate.Append("                Insert callout image if needed or delete this text...");
            sbDocumentTemplate.Append("			</td>");
            sbDocumentTemplate.Append("			<td class='HelpTableRightTextColContainer_Edit' style='white-space: normal !important;'>");
            sbDocumentTemplate.Append("				<h3>Section Title</h3>");
            sbDocumentTemplate.Append("             <p>");
            sbDocumentTemplate.Append("					Section content...");
            sbDocumentTemplate.Append("				</p>");
            sbDocumentTemplate.Append("			</td>");
            sbDocumentTemplate.Append("		</tr>");

            // Content row
            sbDocumentTemplate.Append("		<tr>");
            sbDocumentTemplate.Append("			<td class='HelpTableLeftColumnContainer_Edit' style='text-align: right !important; vertical-align: top !important;'>");
            sbDocumentTemplate.Append("                Insert callout image if needed or delete this text...");
            sbDocumentTemplate.Append("			</td>");
            sbDocumentTemplate.Append("			<td class='HelpTableRightTextColContainer_Edit' style='white-space: normal !important;'>");
            sbDocumentTemplate.Append("				<h3>Section Title</h3>");
            sbDocumentTemplate.Append("             <p>");
            sbDocumentTemplate.Append("					Section content...");
            sbDocumentTemplate.Append("				</p>");
            sbDocumentTemplate.Append("			</td>");
            sbDocumentTemplate.Append("		</tr>");

            // Content row
            sbDocumentTemplate.Append("		<tr>");
            sbDocumentTemplate.Append("			<td class='HelpTableLeftColumnContainer_Edit' style='text-align: right !important; vertical-align: top !important;'>");
            sbDocumentTemplate.Append("                Insert callout image if needed or delete this text...");
            sbDocumentTemplate.Append("			</td>");
            sbDocumentTemplate.Append("			<td class='HelpTableRightTextColContainer_Edit' style='white-space: normal !important;'>");
            sbDocumentTemplate.Append("				<h3>Section Title</h3>");
            sbDocumentTemplate.Append("             <p>");
            sbDocumentTemplate.Append("					Section content...");
            sbDocumentTemplate.Append("				</p>");
            sbDocumentTemplate.Append("			</td>");
            sbDocumentTemplate.Append("		</tr>");

            sbDocumentTemplate.Append("	</tbody>");
            sbDocumentTemplate.Append("</table>");

            divMessageContainer.Visible = false;

            chkIsPublic.Checked = true;

            btnUpdateTopics.Text = "Save";

            if(!IsPostBack)
            {
                // Get general topics
                GetCategories();
            }
            else
            {
                if (spanTopicId.InnerHtml != "")
                {
                    HelpTopic topicObject = new HelpTopic(spanTopicId.InnerHtml);

                    // Clear existing relationships
                    if (topicObject.Relationships != null)
                    {
                        topicObject.Relationships.Clear();
                        topicObject.Update(topicObject);
                    }

                    if (hiddenSelectedUserIds.Value.Contains("|"))
                    {
                        var selectedUserIds = hiddenSelectedUserIds.Value.Split('|');
                        foreach (var selectedId in selectedUserIds)
                        {
                            if (selectedId != "")
                            {
                                // Create the relationship
                                Relationship myRelationship = new Relationship();
                                myRelationship.MemberId = ObjectId.Parse(selectedId);
                                myRelationship.MemberHierarchy = "Help";
                                myRelationship.MemberType = "User";

                                topicObject.Relationships.Add(myRelationship);
                            }
                        }

                        topicObject.IsPublic = false;
                        topicObject.Update(topicObject);

                        chkIsPublic.Checked = false;
                        btnAssignAccess.Disabled = false;

                        hiddenSelectedUserIds.Value = "";
                    }
                }
                //else
                //{
                //    HelpTopic topicObject = new HelpTopic("");
                //    topicObject.Category = dlTopics.SelectedValue;
                //    topicObject.SubCategory = "";
                //    topicObject.Description = "";
                //    topicObject.Details = "";
                //    topicObject.IsPublic = true;
                //    topicObject.IsTopLevel = false;
                    
                //    topicObject.Create(topicObject);
                //}
            }

            if (dlTopics.SelectedIndex > 0)
            {
                divEditorContainer.Visible = true;
            }
            else
            {
                divEditorContainer.Visible = false;
            }
        }

        protected void dlTopics_SelectedIndexChanged(object sender, EventArgs e)
        {
            spanTopicId.InnerHtml = "";
            spanTopicCreated.InnerHtml = "";

            dlSubTopics.SelectedIndex = 0;

            if (dlTopics.SelectedIndex > 0)
            {
                switch(dlTopics.SelectedIndex)
                {
                    case 0:
                        btnUpdateTopics.Text = "Save";
                        txtSummary.Value = "";
                        CKEditor1.Text = ""; //sbDocumentTemplate.ToString();
                        dlSubTopics.SelectedIndex = 0;
                        dlSubTopics.Enabled = false;
                        break;

                    case 1: // Create new topic
                        btnUpdateTopics.Text = "Save";
                        txtSummary.Value = "";
                        CKEditor1.Text = ""; //sbDocumentTemplate.ToString();
                        dlSubTopics.SelectedIndex = 0;
                        dlSubTopics.Enabled = false;
                        break;

                    default:
                        btnUpdateTopics.Text = "Update";

                        // Get selected topic 
                        HelpTopic myTopic = new HelpTopic(dlTopics.SelectedValue);

                        chkIsPublic.Checked = myTopic.IsPublic;

                        if (myTopic.IsPublic)
                            btnAssignAccess.Disabled = true;
                        else
                            btnAssignAccess.Disabled = false;

                        txtSummary.Value = myTopic.Description;
                        CKEditor1.Text = myTopic.Details;

                        spanTopicId.InnerHtml = dlTopics.SelectedValue;
                        spanTopicCreated.InnerHtml = myTopic.DateCreated.ToLocalTime().ToString();

                        dlSubTopics.Enabled = true;
                        break;
                }

                //CKEditor1.Height = 500;

                GetSubCategories();
            }
        }

        protected void dlSubTopics_SelectedIndexChanged(object sender, EventArgs e)
        {
            spanTopicId.InnerHtml = "";
            spanTopicCreated.InnerHtml = "";

            if (dlSubTopics.SelectedIndex > 0)
            {
                switch (dlSubTopics.SelectedIndex)
                {
                    case 0: // Do nothing
                        break;

                    case 1: // Create new sub-topic
                        btnUpdateTopics.Text = "Save";
                        txtSummary.Value = "";
                        CKEditor1.Text = ""; //sbDocumentTemplate.ToString();
                        break;

                    default:
                        btnUpdateTopics.Text = "Update";
                        // Get selected topic 
                        HelpTopic myTopic = new HelpTopic(dlSubTopics.SelectedValue);

                        chkIsPublic.Checked = myTopic.IsPublic;

                        if (myTopic.IsPublic)
                            btnAssignAccess.Disabled = true;
                        else
                            btnAssignAccess.Disabled = false;

                        txtSummary.Value = myTopic.Description;
                        CKEditor1.Text = myTopic.Details;

                        spanTopicId.InnerHtml = myTopic._id.ToString();
                        spanTopicCreated.InnerHtml = myTopic.DateCreated.ToLocalTime().ToString();

                        dlSubTopics.Enabled = true;
                        break;
                }
            }
        }

        protected void SaveTopic(object sender, EventArgs e)
        {
            if (spanTopicId.InnerHtml != "")
                btnUpdateTopics.Text = "Update";

            switch (btnUpdateTopics.Text)
            {
                case "Save":
                    HelpTopic myTopic = new HelpTopic("");

                    switch (dlTopics.SelectedIndex)
                    {
                        case 0:
                            // Do nothing
                            break;
                        case 1:
                            myTopic.IsTopLevel = true;
                            myTopic.Category = txtSummary.Value;
                            myTopic.SubCategory = "";
                            myTopic.Description = "";
                            myTopic.Details = "";
                            break;
                        default:
                            myTopic.IsTopLevel = true;
                            myTopic.Category = dlTopics.SelectedItem.Text;
                            myTopic.SubCategory = "";
                            myTopic.Description = "";
                            myTopic.Details = "";
                            break;
                    }

                    switch (dlSubTopics.SelectedIndex)
                    {
                        case 0:
                            // Do nothing
                            break;
                        case 1:
                            myTopic.IsTopLevel = false;
                            myTopic.Category = dlTopics.SelectedItem.Text;
                            myTopic.SubCategory = txtSummary.Value;
                            myTopic.Description = "";
                            myTopic.Details = "";
                            break;
                        default:
                            myTopic.IsTopLevel = false;
                            myTopic.Category = dlTopics.SelectedItem.Text;
                            myTopic.SubCategory = dlSubTopics.SelectedItem.Text;
                            myTopic.Description = "";
                            myTopic.Details = "";
                            break;
                    }

                    myTopic.IsPublic = chkIsPublic.Checked;

                    if (myTopic.IsPublic)
                        btnAssignAccess.Disabled = true;
                    else
                        btnAssignAccess.Disabled = false;

                    myTopic.Description = txtSummary.Value;
                    myTopic.Details = CKEditor1.Text;

                    myTopic.Create(myTopic);

                    divMessageContainer.Visible = true;
                    updateMessage.InnerHtml = "Successfully created new help topic";

                    if (dlTopics.SelectedIndex == 1) // Just created a new topic. Only refresh the sub list
                    {
                        GetCategories();
                        dlTopics.SelectedItem.Text = txtSummary.Value;
                    }

                    if (dlSubTopics.SelectedIndex == 1) // Just created a new sub-topic. Only refresh the sub list
                    {
                        GetSubCategories();
                        dlSubTopics.SelectedItem.Text = txtSummary.Value;
                    }
                    break;

                case "Update":
                    myTopic = new HelpTopic(spanTopicId.InnerHtml);

                    myTopic.IsPublic = chkIsPublic.Checked;

                    if (myTopic.IsPublic)
                        btnAssignAccess.Disabled = true;
                    else
                        btnAssignAccess.Disabled = false;

                    myTopic.Description = txtSummary.Value;
                    myTopic.Details = CKEditor1.Text;

                    myTopic.Update(myTopic);

                    divMessageContainer.Visible = true;
                    updateMessage.InnerHtml = "Successfully updated help topic";
                    break;
            }
        }

        protected void CancelUpdate(object sender, EventArgs e)
        {

        }

        private void GetCategories()
        {
            dlTopics.Items.Clear();

            ListItem rootLi = new ListItem();
            rootLi.Text = "Select a Help Topic";
            rootLi.Value = MACServices.Constants.Strings.DefaultEmptyObjectId;
            dlTopics.Items.Add(rootLi);

            ListItem createLi = new ListItem();
            createLi.Text = "!Create New Topic";
            createLi.Value = MACServices.Constants.Strings.DefaultStaticObjectId;
            dlTopics.Items.Add(createLi);

            // Do NOT qualify with this on management page Query.EQ("IsPublic", true));
            // Otherwise it will not show up in topic management, duh!
            var query = Query.EQ("IsTopLevel", true);
            // Filter furthur based on role

            var sortBy = SortBy.Ascending("Category");

            var topicsResult = myUtils.mongoDBConnectionPool.GetCollection("Help").FindAs<HelpTopic>(query).SetSortOrder(sortBy);
            foreach (HelpTopic currentTopic in topicsResult)
            {
                ListItem li = new ListItem();
                li.Text = currentTopic.Category;
                li.Value = currentTopic._id.ToString();

                dlTopics.Items.Add(li);
            }
        }

        private void GetSubCategories()
        {
            dlSubTopics.Items.Clear();

            ListItem rootLi = new ListItem();
            rootLi.Text = "Sub-Topics";
            rootLi.Value = MACServices.Constants.Strings.DefaultEmptyObjectId;
            dlSubTopics.Items.Add(rootLi);

            ListItem createLi = new ListItem();
            createLi.Text = "!Create New Sub-Topic";
            createLi.Value = MACServices.Constants.Strings.DefaultStaticObjectId;
            dlSubTopics.Items.Add(createLi);

            // Do NOT qualify with this on management page Query.EQ("IsPublic", true));
            // Otherwise it will not show up in topic management, duh!
            var query = Query.EQ("Category", dlTopics.SelectedItem.Text);

            var sortBy = SortBy.Ascending("SubCategory");

            var topicsResult = myUtils.mongoDBConnectionPool.GetCollection("Help").FindAs<HelpTopic>(query).SetSortOrder(sortBy);
            foreach (HelpTopic currentTopic in topicsResult)
            {
                ListItem li = new ListItem();
                li.Text = currentTopic.SubCategory;
                li.Value = currentTopic._id.ToString();

                dlSubTopics.Items.Add(li);
            }
        }
    }
}