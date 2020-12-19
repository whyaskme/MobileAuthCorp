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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

using MACServices;

using cnt = MACBilling.BillConstants;

namespace MACBilling
{
    public partial class AddendumPopup : Page
    {
        public string loggedInAdminId = "";
        public string ownerId = "";
        public string ownerName = "";
        public string configType = "";
        public string billId = "";

        public Utils myUtils = new Utils();
        public BillUtils myBillUtils = new BillUtils();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Deny access if user request not logged in
            var isAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
            if (!isAuthenticated)
            {
                Response.Write("Hello world!");
                Response.End();
            }

            if (!String.IsNullOrEmpty(HttpContext.Current.Request["loggedInAdminId"]))
                loggedInAdminId = HttpContext.Current.Request["loggedInAdminId"];

            loggedInAdminId = MACSecurity.Security.DecodeAndDecrypt(loggedInAdminId, Constants.Strings.DefaultClientId);

            if (!String.IsNullOrEmpty(HttpContext.Current.Request["clientId"]))
                ownerId = HttpContext.Current.Request["clientId"];

            if (!String.IsNullOrEmpty(HttpContext.Current.Request["configType"]))
                configType = HttpContext.Current.Request["configType"];

            if (!String.IsNullOrEmpty(HttpContext.Current.Request["billId"]))
                billId = HttpContext.Current.Request["billId"];

            var userIsReadOnly = "";
            if (!String.IsNullOrEmpty(Request["userisreadonly"]))
                userIsReadOnly = Request["userisreadonly"];

            userIsReadOnly = MACSecurity.Security.DecodeAndDecrypt(userIsReadOnly, loggedInAdminId);

            BillAddendum myAddendum = new BillAddendum();

            if (IsPostBack)
            {
                switch(hiddenAA.Value)
                {
                    case "Update":
                        // Update the selected addendum
                        myAddendum = new BillAddendum(ownerId, dlAddendums.SelectedValue);

                        myAddendum.Amount = txtAmount.Value.Replace("$","");
                        myAddendum.Notes = txtNotes.Value;

                        myAddendum.Update(myAddendum, myAddendum._id.ToString());

                        ClientScript.RegisterStartupScript(typeof(Page), "closePage", "<script type='text/JavaScript'>callParentDocumentFunction();</script>");
                        break;

                    case "Delete":
                        myAddendum.DeleteAddendum(dlAddendums.SelectedValue);

                        myAddendum.Amount = "";
                        myAddendum.Notes = "";

                        dlAddendums.SelectedIndex = 0;

                        ClientScript.RegisterStartupScript(typeof(Page), "closePage", "<script type='text/JavaScript'>callParentDocumentFunction();</script>");
                        break;

                    case "Create":
                        if (configType == "Client")
                        {
                            Client myClient = new Client(ownerId);
                            ownerName = myClient.Name;
                        }
                        else
                        {
                            Group myGroup = new Group(ownerId);
                            ownerName = myGroup.Name;
                        }

                        myAddendum.OwnerId = ObjectId.Parse(ownerId);
                        myAddendum.OwnerType = configType;
                        myAddendum.OwnerName = ownerName;

                        spanAddendum.InnerHtml = "Create " + ownerName + " (" + configType + ") Billing Addendum";

                        myAddendum.CreatedById = ObjectId.Parse(loggedInAdminId);

                        UserProfile adminProfile = new UserProfile(loggedInAdminId);

                        var adminFirstName = MACSecurity.Security.DecodeAndDecrypt(adminProfile.FirstName, adminProfile._id.ToString());
                        var adminLastName = MACSecurity.Security.DecodeAndDecrypt(adminProfile.LastName, adminProfile._id.ToString());
                        var adminFullName = adminFirstName + " " + adminLastName;

                        var myBillUtils = new BillUtils();

                        var addendumNotes = txtNotes.Value;
                        if (String.IsNullOrEmpty(addendumNotes))
                            addendumNotes = "Notes not specified";

                        myAddendum.Notes = addendumNotes;

                        var addendumAmount = txtAmount.Value.Replace("$","");
                        if (String.IsNullOrEmpty(addendumAmount))
                            addendumAmount = "0.00";

                        myAddendum.Amount = addendumAmount;

                        myBillUtils.Create(myAddendum);

                        // Log the Addendum
                        //var billAddendumEvent = new Event
                        //{
                        //    ClientId = ObjectId.Parse(ownerId),
                        //    UserId = ObjectId.Parse(loggedInAdminId),
                        //    EventTypeDesc = cnt.TokenKeys.EventGeneratedByName + adminFullName
                        //                    + cnt.TokenKeys.ClientName + ownerName
                        //                    + cnt.TokenKeys.BillId + billId
                        //                    + cnt.TokenKeys.BillAddendumAmount + MACSecurity.Security.DecodeAndDecrypt(myAddendum.Amount, myAddendum.OwnerId.ToString())
                        //                    + cnt.TokenKeys.BillAddendumNotes + MACSecurity.Security.DecodeAndDecrypt(myAddendum.Notes, myAddendum.OwnerId.ToString())
                        //};

                        var billAddendumEvent = new Event
                        {
                            ClientId = ObjectId.Parse(ownerId),
                            UserId = ObjectId.Parse(loggedInAdminId),
                            EventTypeDesc = cnt.TokenKeys.EventGeneratedByName + adminFullName
                                            + cnt.TokenKeys.ClientName + ownerName
                                            + cnt.TokenKeys.BillId + billId
                                            + cnt.TokenKeys.BillAddendumAmount + myAddendum.Amount
                                            + cnt.TokenKeys.BillAddendumNotes + myAddendum.Notes
                        };

                        billAddendumEvent.Create(cnt.EventLog.Addendum.Created, null);

                        ClientScript.RegisterStartupScript(typeof(Page), "closePage", "<script type='text/JavaScript'>callParentDocumentFunction();</script>");
                        break;
                }

                if (dlAddendums.SelectedIndex > 0)
                {
                    // Get selected addendum details
                    myAddendum = new BillAddendum(ownerId, dlAddendums.SelectedValue);

                    var tmpAddAmount = myAddendum.Amount;
                    var tmpAddNotes = myAddendum.Notes;

                    var addendumAmount = myBillUtils.FormatMoney(Convert.ToDecimal(tmpAddAmount));
                    if (addendumAmount.Contains("(") || addendumAmount.Contains("("))
                    {
                        addendumAmount = addendumAmount.Replace("(", "-");
                        addendumAmount = addendumAmount.Replace(")", "");
                    }

                    txtAmount.Value = addendumAmount.Replace("$", "");
                    txtNotes.Value = tmpAddNotes;

                    btnDelete.Disabled = false;
                }
                else
                {
                    btnDelete.Disabled = true;
                }
            }
            else
            {
                GetAddendums("");
            }

            if (Convert.ToBoolean(userIsReadOnly))
            {
                btnDelete.Visible = false;
                btnSave.Visible = false;
            }
        }

        public void GetAddendums(string selectedAddendumId)
        {
            int i = 0;

            dlAddendums.Items.Clear();

            ListItem defaultLi = new ListItem();
            defaultLi.Text = "Select an addendum to update it";
            defaultLi.Value = MACServices.Constants.Strings.DefaultEmptyObjectId;

            dlAddendums.Items.Add(defaultLi);

            var addendumQuery = Query.And(Query.EQ("_t", "BillAddendum"), Query.EQ("OwnerId", ObjectId.Parse(ownerId)), Query.EQ("HasBeenAttached", false));
            var sortBy = SortBy.Descending("DateCreated");

            var addendumResult = myUtils.mongoDBConnectionPool.GetCollection("Billing").FindAs<BillAddendum>(addendumQuery).SetSortOrder(sortBy);
            foreach (BillAddendum currentAddendum in addendumResult)
            {
                i++;

                // Have to do this. Otherwise, the values get re-encrypted again during the member copy operation!
                //var tmpAddAmount = MACSecurity.Security.DecodeAndDecrypt(currentAddendum.Amount, currentAddendum.OwnerId.ToString());
                //tmpAddAmount = MACSecurity.Security.DecodeAndDecrypt(tmpAddAmount, currentAddendum.OwnerId.ToString());

                //var tmpAddNotes = MACSecurity.Security.DecodeAndDecrypt(currentAddendum.Notes, currentAddendum.OwnerId.ToString());
                //tmpAddNotes = MACSecurity.Security.DecodeAndDecrypt(tmpAddNotes, currentAddendum.OwnerId.ToString());

                var tmpAddAmount = currentAddendum.Amount;

                var tmpAddNotes = currentAddendum.Notes;

                var addendumAmount = myBillUtils.FormatMoney(Convert.ToDecimal(tmpAddAmount));
                if(addendumAmount.Contains("(") || addendumAmount.Contains("("))
                {
                    addendumAmount = addendumAmount.Replace("(", "-");
                    addendumAmount = addendumAmount.Replace(")", "");
                }

                ListItem li = new ListItem();
                if (currentAddendum._id.ToString() == selectedAddendumId)
                    li.Selected = true;

                var truncateText = "...";

                var maxLength = 30;
                if (tmpAddNotes.Length < maxLength)
                {
                    maxLength = tmpAddNotes.Length;
                    truncateText = "";
                }

                li.Text = i + ") " + currentAddendum.DateCreated.ToString() + ": " + addendumAmount + " - " + tmpAddNotes.Substring(0, maxLength) + truncateText;
                li.Value = currentAddendum._id.ToString();

                dlAddendums.Items.Add(li);
            }

            if(dlAddendums.Items.Count < 2)
                divAddendumCollection.Visible = false;
            else
                divAddendumCollection.Visible = true;
        }
        protected void dlAddendums_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(dlAddendums.SelectedIndex > 0)
                btnSave.Value = "Update";
            else
                btnSave.Value = "Create";
        }
}
}