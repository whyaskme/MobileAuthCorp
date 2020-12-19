using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using MACServices;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;

using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using cfgcs = MACServices.Constants.WebConfig.ConnectionStringKeys;

namespace Admin.Help
{
    public partial class Default : System.Web.UI.Page
    {
        public string loggedInAdminId = "";

        public HelpUtils myHelpUtils = new HelpUtils();
        public Utils myUtils = new Utils();

        HiddenField _hiddenE;
        HiddenField _hiddenH;
        HiddenField _hiddenI;
        HiddenField _hiddenD;
        HiddenField _hiddenAA;
        HiddenField _userIpAddress;
        HiddenField _hiddenUserRole;

        HiddenField _hiddenW;

        protected override void OnPreRender(EventArgs e)
        {
            var y = Regex.Replace(divHelpDetails.InnerHtml, @"<img([\w\W]+?)/>", "<div>$&</div>");

            divHelpDetails.InnerHtml = y;

            base.OnPreRender(e);
        }

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
                    _hiddenW = (HiddenField)Master.FindControl("hiddenW");
                }
            }

            // Page context id
            if (Request["tid"] != null)
            {
                if (Request["tid"] != "")
                {
                    _hiddenW.Value = Request["tid"];
                }
                else
                {
                    if (!IsPostBack)
                    {
                        GetPublicCategories();
                        GetPrivateCategories();
                    }
                }
            }

            if (!String.IsNullOrEmpty(HttpContext.Current.Request["u"]))
                loggedInAdminId = HttpContext.Current.Request["u"];

            if (!String.IsNullOrEmpty(HttpContext.Current.Request["loggedInAdminId"]))
                loggedInAdminId = HttpContext.Current.Request["loggedInAdminId"];

            if (!String.IsNullOrEmpty(HttpContext.Current.Request["userId"]))
                loggedInAdminId = HttpContext.Current.Request["userId"];

            loggedInAdminId = MACSecurity.Security.DecodeAndDecrypt(loggedInAdminId, Constants.Strings.DefaultClientId);

            var userProfile = new UserProfile(loggedInAdminId);

            var userRole = myUtils.GetRoleNameByRoleId(userProfile.Roles[0].ToString());

            if (userRole == "System Administrator")
                btnManageTopics.Visible = true;
            else
                btnManageTopics.Visible = false;

            if (hiddenX.Value != "true")
            {
                if (_hiddenW.Value != "")
                {
                    // Set List Boxes from hidden field
                    HelpTopic myTopic = new HelpTopic(_hiddenW.Value);

                    var itemCount = 0;
                    var itemIndex = 0;

                    GetPublicCategories();
                    GetPrivateCategories();

                    foreach (ListItem item in dlTopics.Items)
                    {
                        if (item.Text == myTopic.Category)
                            itemIndex = itemCount;
                        else
                            itemCount++;

                    }
                    dlTopics.SelectedIndex = itemIndex;

                    divHelpDescription.InnerHtml = "<h1>" + myTopic.Description + "</h1>";

                    var helpDetailsContent = myTopic.Details.ToString();
                    if (helpDetailsContent != "")
                        divHelpDetails.InnerHtml = helpDetailsContent;
                    else
                        divHelpDetails.InnerHtml = "<div style='color: #ff0000; width: 100%; text-align: center; font-weight: normal; padding-top: 50px;'>!!! Needs content !!!</div>";

                    spanTopicId.InnerHtml = dlTopics.SelectedValue;
                    spanTopicCreated.InnerHtml = myTopic.DateCreated.ToLocalTime().ToString();

                    GetPublicSubCategories();
                    GetPrivateSubCategories();

                    itemCount = 0;
                    itemIndex = 0;

                    foreach (ListItem item in dlSubTopics.Items)
                    {
                        if (item.Text == myTopic.SubCategory)
                            itemIndex = itemCount;
                        else
                            itemCount++;

                    }
                    dlSubTopics.SelectedIndex = itemIndex;

                    dlSubTopics.Enabled = true;

                    _hiddenW.Value = "";
                    hiddenX.Value = "true";
                }
            }

            divEditorContainer.Visible = true;

            if (dlTopics.SelectedIndex == 0)
                GetTableOfContents();
        }

        private void GetTableOfContents()
        {
            var totalPageCount = 0;
            var pageNumber = 0;
            var topicNumber = 0;
            var subTopicNumber = 0;
            var topicTitle = "";

            StringBuilder sbTableOfContents = new StringBuilder();

            sbTableOfContents.Append("<table border='1' style='border-bottom: dashed 0px #c0c0c0; width: 100%;' cellspacing='0' cellpadding='0'>");
            sbTableOfContents.Append("  <tr>");
            sbTableOfContents.Append("      <td style='border-bottom: dashed 0px #c0c0c0; padding: 7px; white-space: nowrap;width:2% !important;text-align: right; font-weight: bold;'>");
            sbTableOfContents.Append("&nbsp;");
            sbTableOfContents.Append("      </td>");
            sbTableOfContents.Append("      <td style='border-bottom: dashed 0px #c0c0c0; padding: 7px; white-space: nowrap;width:33% !important;font-weight: bold;'>");
            sbTableOfContents.Append("&nbsp;");
            sbTableOfContents.Append("      </td>");
            sbTableOfContents.Append("      <td style='border-bottom: dashed 0px #c0c0c0; width:65% !important;'>");
            sbTableOfContents.Append("&nbsp;");
            sbTableOfContents.Append("      </td>");
            sbTableOfContents.Append("  </tr>");

            var IsRootTableOfContents = true;
            var IsTopicTableOfContentsInitialized = false;

            var query = Query.EQ("IsTopLevel", true);

            var selectedCategory = dlTopics.SelectedItem.Text;
            switch (selectedCategory)
            {
                case "------------------------ Public ------------------------":
                    // Do nothing
                    break;

                case "Table of Contents":
                    // Do nothing
                    break;

                default:
                    query = Query.EQ("Category", selectedCategory);
                    IsRootTableOfContents = false;
                    break;
            }

            var sortBy = SortBy.Ascending("Category");

            var mainTopics = myHelpUtils.mongoDBConnectionPool.GetCollection("Help").FindAs<HelpTopic>(query).SetSortOrder(sortBy);
            foreach (HelpTopic currentTopic in mainTopics)
            {
                totalPageCount++;
                pageNumber++;
                topicNumber++;
                subTopicNumber = 0;

                topicTitle = currentTopic.Category;

                // Process the loop only once
                if (IsRootTableOfContents) 
                {
                    sbTableOfContents.Append("  <tr>");
                    sbTableOfContents.Append("      <td style='border-bottom: dashed 0px #c0c0c0;width:2% !important;padding: 7px; white-space: nowrap; text-align: right; font-weight: bold;'>");
                    sbTableOfContents.Append(topicNumber + ".  ");
                    sbTableOfContents.Append("      </td>");
                    sbTableOfContents.Append("      <td style='border-bottom: dashed 1px #c0c0c0;width:33% !important;padding: 7px; padding-left: 0; white-space: nowrap; font-weight: bold;'>");
                    sbTableOfContents.Append("              <a href='javascript: NavigateTopic(&quot;" + currentTopic._id + "&quot;);' id='link_isRootTableOfContents_currentTopic'>");
                    sbTableOfContents.Append("                  <span style='font-weight: bold;'>");
                    sbTableOfContents.Append(FormatTopicTitle(topicTitle));
                    sbTableOfContents.Append("                  </span>");
                    sbTableOfContents.Append("              </a>");
                    sbTableOfContents.Append("      </td>");
                    sbTableOfContents.Append("      <td style='border-bottom: dashed 1px #c0c0c0;width:65% !important;'>");
                    sbTableOfContents.Append("&nbsp;");
                    sbTableOfContents.Append("      </td>");
                    sbTableOfContents.Append("  </tr>");

                    // Get sub-topics
                    query = Query.EQ("Category", currentTopic.Category);
                    sortBy = SortBy.Ascending("SubCategory");

                    var subTopics = myHelpUtils.mongoDBConnectionPool.GetCollection("Help").FindAs<HelpTopic>(query).SetSortOrder(sortBy);
                    foreach (HelpTopic currentSubTopic in subTopics)
                    {
                        if (currentSubTopic.SubCategory != "")
                        {
                            totalPageCount++;
                            pageNumber++;
                            subTopicNumber++;
                            topicTitle = currentSubTopic.SubCategory;

                            sbTableOfContents.Append("  <tr>");
                            sbTableOfContents.Append("      <td style='border-bottom: dashed 0px #c0c0c0;width:2% !important; padding: 7px; white-space: nowrap;text-align: right; font-style: italic;'>");
                            sbTableOfContents.Append(" &nbsp;");
                            sbTableOfContents.Append("      </td>");
                            sbTableOfContents.Append("      <td style='color: #808080; border-bottom: dashed 1px #c0c0c0;width:33% !important; padding: 0px; padding-left: 5px; white-space: nowrap;'>");
                            sbTableOfContents.Append("&raquo; ");
                            sbTableOfContents.Append("              <a href='javascript: NavigateTopic(&quot;" + currentSubTopic._id + "&quot;);' id='link_isRootTableOfContents_currentSubTopic'>");
                            sbTableOfContents.Append(FormatTopicTitle(topicTitle));
                            sbTableOfContents.Append("              </a>");
                            sbTableOfContents.Append("      </td>");
                            sbTableOfContents.Append("      <td style='border-bottom: dashed 1px #c0c0c0;width:65% !important;'>");
                            sbTableOfContents.Append("&nbsp;");
                            sbTableOfContents.Append("      </td>");
                            sbTableOfContents.Append("  </tr>");
                        }
                    }
                    dlSubTopics.Enabled = false;
                }
                else
                {
                    if (!IsTopicTableOfContentsInitialized)
                    {
                        sbTableOfContents.Append("  <tr>");
                        sbTableOfContents.Append("      <td style='border-bottom: dashed 0px #c0c0c0;width:2% !important;  padding: 7px; white-space: nowrap;text-align: right; font-weight: bold;'>");
                        sbTableOfContents.Append(topicNumber + ".  ");
                        sbTableOfContents.Append("      </td>");
                        sbTableOfContents.Append("      <td style='border-bottom: dashed 1px #c0c0c0;width:33% !important; padding: 7px; padding-left: 0; white-space: nowrap; font-weight: bold;'>");
                        sbTableOfContents.Append("              <a href='javascript: NavigateTopic(&quot;" + currentTopic._id + "&quot;);' id='link_isNotRootTableOfContents_currentTopic'>");
                        sbTableOfContents.Append("                  <span style='font-weight: bold;'>");
                        sbTableOfContents.Append(FormatTopicTitle(topicTitle));
                        sbTableOfContents.Append("                  </span>");
                        sbTableOfContents.Append("              </a>");
                        sbTableOfContents.Append("      </td>");
                        sbTableOfContents.Append("      <td style='border-bottom: dashed 1px #c0c0c0;width:65% !important;'>");
                        sbTableOfContents.Append("&nbsp;");
                        sbTableOfContents.Append("      </td>");
                        sbTableOfContents.Append("  </tr>");

                        // Get sub-topics
                        query = Query.EQ("Category", currentTopic.Category);
                        sortBy = SortBy.Ascending("SubCategory");

                        var subTopics = myHelpUtils.mongoDBConnectionPool.GetCollection("Help").FindAs<HelpTopic>(query).SetSortOrder(sortBy);
                        foreach (HelpTopic currentSubTopic in subTopics)
                        {
                            if (currentSubTopic.SubCategory != "")
                            {
                                pageNumber++;
                                subTopicNumber++;
                                topicTitle = currentSubTopic.SubCategory;

                                sbTableOfContents.Append("  <tr>");
                                sbTableOfContents.Append("      <td style='border-bottom: dashed 0px #c0c0c0;width:2% !important; padding: 7px; white-space: nowrap;text-align: right; font-style: italic;'>");
                                sbTableOfContents.Append(" &nbsp;");
                                sbTableOfContents.Append("      </td>");
                                sbTableOfContents.Append("      <td style='color: #808080; border-bottom: dashed 1px #c0c0c0;width:33% !important; padding: 0px; padding-left: 5px; white-space: nowrap;'>");
                                sbTableOfContents.Append("&raquo; ");
                                sbTableOfContents.Append("              <a href='javascript: NavigateTopic(&quot;" + currentSubTopic._id + "&quot;);' id='link_isNotRootTableOfContents_currentSubTopic'>");
                                sbTableOfContents.Append(FormatTopicTitle(topicTitle));
                                sbTableOfContents.Append("              </a>");
                                sbTableOfContents.Append("      </td>");
                                sbTableOfContents.Append("      <td style='border-bottom: dashed 1px #c0c0c0;width:65% !important;'>");
                                sbTableOfContents.Append("&nbsp;");
                                sbTableOfContents.Append("      </td>");
                                sbTableOfContents.Append("  </tr>");
                            }
                        }
                        IsTopicTableOfContentsInitialized = true;
                    }
                    dlSubTopics.Enabled = true;
                }
            }

            sbTableOfContents.Append("</table>");

            divHelpDescription.InnerHtml = "<h1>Table of Contents</h1>";
            divHelpDetails.InnerHtml = sbTableOfContents.ToString();

            spanTopicId.InnerHtml = MACServices.Constants.Strings.DefaultEmptyObjectId;
            spanTopicCreated.InnerHtml = DateTime.UtcNow.ToString();

            if (dlTopics.SelectedIndex != 4)
                if (dlSubTopics.SelectedIndex == 0)
                    GetChildTopicContent(dlTopics.SelectedItem.Text);
        }

        private void GetSystemDesign()
        {
            var totalPageCount = 0;
            var pageNumber = 0;

            List<string> lstDesign = new List<string>();

            StringBuilder sbSystemDesign = new StringBuilder();

            var query = Query.EQ("IsPublic", true);
            var sortBy = SortBy.Ascending("SubCategory");

            var systemRequirements = myHelpUtils.mongoDBConnectionPool.GetCollection("Help").FindAs<HelpTopic>(query).SetSortOrder(sortBy);
            foreach (HelpTopic currentTopic in systemRequirements)
            {
                var subCategory = currentTopic.SubCategory;
                if (subCategory.Contains("Design") && currentTopic.Category != "4) System Design")
                    lstDesign.Add(FormatTopicTitle(currentTopic.Category) + "~" + currentTopic.Details.Replace("h2", "h3"));
            }

            lstDesign.Sort();
            foreach (string currentItem in lstDesign)
            {
                pageNumber++;

                var tmpArr = currentItem.Split('~');

                sbSystemDesign.Append("<div style='border-bottom: solid 1px #c0c0c0; padding-bottom: 15px;margin-bottom: 25px;'>");
                sbSystemDesign.Append("<h2>" + pageNumber + ". " + tmpArr[0] + "</h2>");
                sbSystemDesign.Append("<div style='padding-left: 25px; width: 98%;'>" + tmpArr[1] + "</div>");
                sbSystemDesign.Append("</div>");
            }

            divHelpDescription.InnerHtml = "<h1>System Design <span style='font-weight: normal; color: #808080; font-size: 12px;'>(" + totalPageCount + " pages)</span></h1>";
            divHelpDetails.InnerHtml = sbSystemDesign.ToString();

            dlSubTopics.SelectedIndex = 0;
            //dlSubTopics.Enabled = false;
        }

        private void GetSystemRequirements()
        {
            var pageNumber = 0;

            List<string> lstRequirements = new List<string>();

            StringBuilder sbSystemRequirements = new StringBuilder();

            var query = Query.EQ("IsPublic", true);
            var sortBy = SortBy.Ascending("SubCategory");

            var systemRequirements = myHelpUtils.mongoDBConnectionPool.GetCollection("Help").FindAs<HelpTopic>(query).SetSortOrder(sortBy);
            foreach (HelpTopic currentTopic in systemRequirements)
            {
                var subCategory = currentTopic.SubCategory;
                if (subCategory.Contains("Requirements") && currentTopic.Category != "3) System Requirements")
                    lstRequirements.Add(FormatTopicTitle(currentTopic.Category) + "~" + currentTopic.Details.Replace("h2", "h3"));
            }

            lstRequirements.Sort();
            foreach (string currentItem in lstRequirements)
            {
                pageNumber++;

                var tmpArr = currentItem.Split('~');

                sbSystemRequirements.Append("<div style='border-bottom: solid 1px #c0c0c0; padding-bottom: 15px;margin-bottom: 25px;'>");
                sbSystemRequirements.Append("<h2>" + pageNumber + ". " + tmpArr[0] + "</h2>");
                sbSystemRequirements.Append("<div style='padding-left: 25px; width: 98%;'>" + tmpArr[1] + "</div>");
                sbSystemRequirements.Append("</div>");
            }

            divHelpDescription.InnerHtml = "<h1>System Requirements <span style='font-weight: normal; color: #808080; font-size: 12px;'>(" + pageNumber + " pages)</span></h1>";
            divHelpDetails.InnerHtml = sbSystemRequirements.ToString();

            dlSubTopics.SelectedIndex = 0;
            //dlSubTopics.Enabled = false;
        }

        protected void dlTopics_SelectedIndexChanged(object sender, EventArgs e)
        {
            spanTopicId.InnerHtml = "";
            spanTopicCreated.InnerHtml = "";

            dlSubTopics.SelectedIndex = 0;

            if (dlTopics.SelectedIndex > 1)
            {
                if (dlSubTopics.SelectedIndex == 0)
                {
                    switch (dlTopics.SelectedItem.Text)
                    {
                        case "3) System Requirements":
                            GetSystemRequirements();
                            break;

                        case "4) System Design":
                            GetSystemDesign();
                            break;

                        default:
                            GetTableOfContents();
                            GetPublicSubCategories();
                            break;
                    }
                }
                else
                {
                    // Get selected topic 
                    HelpTopic myTopic = new HelpTopic(dlTopics.SelectedValue);

                    divHelpDescription.InnerHtml = "<h1>" + myTopic.Description + "</h1>";

                    // Remove the edit css formatting from the content
                    var helpDetailsContent = myTopic.Details.ToString();
                    if (helpDetailsContent != "")
                        divHelpDetails.InnerHtml = helpDetailsContent;
                    else
                        divHelpDetails.InnerHtml = "<div style='color: #ff0000; width: 100%; text-align: center; font-weight: normal; padding-top: 50px;'>!!! Needs content !!!</div>";

                    // Wrap img tags in div with class 'watermark'
                    var x = divHelpDetails.InnerHtml;
                    var y = Regex.Replace(x, @"<img([\w\W]+?)/>", "<div class='watermark'>$&</div>");

                    divHelpDetails.InnerHtml = y;

                    spanTopicId.InnerHtml = dlTopics.SelectedValue;
                    spanTopicCreated.InnerHtml = myTopic.DateCreated.ToLocalTime().ToString();

                    dlSubTopics.Enabled = true;

                    GetPublicSubCategories();
                }
            }
            else
            {
                GetTableOfContents();
            }
        }

        protected void dlSubTopics_SelectedIndexChanged(object sender, EventArgs e)
        {
            spanTopicId.InnerHtml = "";
            spanTopicCreated.InnerHtml = "";

            if (dlSubTopics.SelectedIndex > 0)
            {
                divEditorContainer.Visible = true;

                // Get selected topic 
                HelpTopic myTopic = new HelpTopic(dlSubTopics.SelectedValue);

                divHelpDescription.InnerHtml = "<h1>" + myTopic.Description + "</h1>";

                // Remove the edit css formatting from the content
                var helpDetailsContent = myTopic.Details.ToString();
                if (helpDetailsContent != "")
                    divHelpDetails.InnerHtml = helpDetailsContent;
                else
                    divHelpDetails.InnerHtml = "<div style='color: #ff0000; width: 100%; text-align: center; font-weight: normal; padding-top: 50px;'>!!! Needs content !!!</div>";

                // Wrap img tags in div with class 'watermark'
                var x = divHelpDetails.InnerHtml;
                var y = Regex.Replace(x, @"<img([\w\W]+?)/>", "<div class='watermark'>$&</div>");

                divHelpDetails.InnerHtml = y;


                spanTopicId.InnerHtml = myTopic._id.ToString();
                spanTopicCreated.InnerHtml = myTopic.DateCreated.ToLocalTime().ToString();
            }
            else
            {
                divEditorContainer.Visible = false;
            }
        }

        private void GetChildTopicContent(string selectedTopic)
        {
            var pageNumber = 1;

            StringBuilder sbResponse = new StringBuilder();

            //divHiddenHelpDescription.InnerHtml = "<h1>" + FormatTopicTitle(selectedTopic) + "</h1>";

            sbResponse.Append(" <div id='divContentDetails' style='border-top: 1px solid #c0c0c0; position: relative; top: 50px; padding-top: 25px;page-break-before: always;'>");

            var query = Query.EQ("Category", selectedTopic);
            var sortBy = SortBy.Ascending("SubCategory");

            var subTopics = myHelpUtils.mongoDBConnectionPool.GetCollection("Help").FindAs<HelpTopic>(query).SetSortOrder(sortBy);
            foreach (HelpTopic currentSubTopic in subTopics)
            {
                if (currentSubTopic.Description != selectedTopic && !currentSubTopic.Description.Contains("Table of Contents"))
                {
                    pageNumber++;

                    sbResponse.Append(" <div id='" + currentSubTopic._id + "' style='border-bottom: 1px solid #c0c0c0; padding-bottom: 15px; margin-bottom: 15px;'>");
                    sbResponse.Append("     <h2>");
                    sbResponse.Append(FormatTopicTitle(currentSubTopic.Description));
                    sbResponse.Append(" <span class='TopicId'>TopicId: " + currentSubTopic._id + "</span>");
                    sbResponse.Append("     </h2>");

                    sbResponse.Append("     <span>" + currentSubTopic.Details + "</span>");
                    sbResponse.Append(" </div>");
                }
            }

            sbResponse.Append(" </div>");

            //divHiddenHelpDetails.InnerHtml = sbResponse.ToString();
            divHelpDetails.InnerHtml += sbResponse.ToString();
        }

        private void GetPublicCategories()
        {
            dlTopics.Items.Clear();

            ListItem rootToC = new ListItem();
            rootToC.Text = "Table of Contents";
            rootToC.Value = MACServices.Constants.Strings.DefaultEmptyObjectId;
            dlTopics.Items.Add(rootToC);

            ListItem publicLi = new ListItem();
            publicLi.Text = "------------------------ Public ------------------------";
            publicLi.Value = "-1";
            dlTopics.Items.Add(publicLi);

            var query = Query.And(Query.EQ("IsTopLevel", true), Query.EQ("IsPublic", true));

            // Don't filter for sysadmin users
            if (_hiddenUserRole.Value == Constants.Roles.SystemAdministrator)
                query = Query.EQ("IsTopLevel", true);

            var sortBy = SortBy.Ascending("Category");

            var topicsResult = myHelpUtils.mongoDBConnectionPool.GetCollection("Help").FindAs<HelpTopic>(query).SetSortOrder(sortBy);
            foreach (HelpTopic currentTopic in topicsResult)
            {
                ListItem li = new ListItem();
                li.Text = currentTopic.Category;
                li.Value = currentTopic._id.ToString();

                dlTopics.Items.Add(li);
            }

            // Show private topics if not sysadmin
            if (_hiddenUserRole.Value != Constants.Roles.SystemAdministrator)
                GetPrivateCategories();
        }

        private void GetPrivateCategories()
        {
            var iCount = 0;
            var query = Query.EQ("Relationships.MemberId", ObjectId.Parse(loggedInAdminId));
            var sortBy = SortBy.Ascending("Category");

            var topicsResult = myHelpUtils.mongoDBConnectionPool.GetCollection("Help").FindAs<HelpTopic>(query).SetSortOrder(sortBy);
            foreach (HelpTopic currentTopic in topicsResult)
            {
                if(iCount == 0)
                {
                    ListItem assignedLi = new ListItem();
                    assignedLi.Text = "---------------------- Assigned ----------------------";
                    assignedLi.Value = "-1";
                    dlTopics.Items.Add(assignedLi);
                    iCount++;
                }

                ListItem li = new ListItem();
                li.Text = currentTopic.Category;
                li.Value = currentTopic._id.ToString();

                dlTopics.Items.Add(li);
            }
        }

        private void GetPublicSubCategories()
        {
            var iItemCount = 0;
            dlSubTopics.Items.Clear();

            ListItem rootLi = new ListItem();
            rootLi.Text = "Sub-Topics";
            rootLi.Value = MACServices.Constants.Strings.DefaultEmptyObjectId;
            dlSubTopics.Items.Add(rootLi);

            ListItem liGeneralSub = new ListItem();
            liGeneralSub.Text = "------------------------ Public ------------------------";
            liGeneralSub.Value = "-1";
            dlSubTopics.Items.Add(liGeneralSub);

            var query = Query.And(Query.EQ("Category", dlTopics.SelectedItem.Text), Query.EQ("IsPublic", true));

            // Don't filter for sysadmin users
            if (_hiddenUserRole.Value == Constants.Roles.SystemAdministrator)
                query = Query.EQ("Category", dlTopics.SelectedItem.Text);

            var sortBy = SortBy.Ascending("SubCategory");

            var topicsResult = myHelpUtils.mongoDBConnectionPool.GetCollection("Help").FindAs<HelpTopic>(query).SetSortOrder(sortBy);
            foreach (HelpTopic currentTopic in topicsResult)
            {
                ListItem li = new ListItem();
                li.Text = currentTopic.SubCategory;
                li.Value = currentTopic._id.ToString();

                dlSubTopics.Items.Add(li);

                iItemCount++;
            }

            // Show private topics if not sysadmin
            if (_hiddenUserRole.Value != Constants.Roles.SystemAdministrator)
                GetPrivateSubCategories();

            // Only enable if we have sub-topics available
            if (dlSubTopics.Items.Count > 3)
                dlSubTopics.Enabled = true;
            else
                dlSubTopics.Enabled = false;
        }

        private void GetPrivateSubCategories()
        {
            var iCount = 0;
            var iItemCount = 0;

            var query = Query.And(Query.EQ("Category", dlTopics.SelectedItem.Text), Query.EQ("Relationships.MemberId", ObjectId.Parse(loggedInAdminId)));
            var sortBy = SortBy.Ascending("SubCategory");

            var topicsResult = myHelpUtils.mongoDBConnectionPool.GetCollection("Help").FindAs<HelpTopic>(query).SetSortOrder(sortBy);
            foreach (HelpTopic currentTopic in topicsResult)
            {
                if (iCount == 0)
                {
                    ListItem assignedLi = new ListItem();
                    assignedLi.Text = "---------------------- Assigned ----------------------";
                    assignedLi.Value = "-1";
                    dlSubTopics.Items.Add(assignedLi);
                    iCount++;
                }

                ListItem li = new ListItem();
                li.Text = currentTopic.SubCategory;
                li.Value = currentTopic._id.ToString();

                dlSubTopics.Items.Add(li);

                iItemCount++;
            }

            if (iItemCount > 0)
                dlSubTopics.Enabled = true;
            else
                dlSubTopics.Enabled = false;
        }

        public string FormatTopicTitle(string topicTitle)
        {
            topicTitle = topicTitle.Replace("1) ", "");
            topicTitle = topicTitle.Replace("2) ", "");
            topicTitle = topicTitle.Replace("3) ", "");
            topicTitle = topicTitle.Replace("4) ", "");
            topicTitle = topicTitle.Replace("5) ", "");
            topicTitle = topicTitle.Replace("6) ", "");
            topicTitle = topicTitle.Replace("7) ", "");
            topicTitle = topicTitle.Replace("8) ", "");
            topicTitle = topicTitle.Replace("9) ", "");
            topicTitle = topicTitle.Replace("10) ", "");

            return topicTitle.Trim();
        }
    }
}