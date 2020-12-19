using System;
using System.Text;
using System.Web;
using System.Web.UI;

using MACServices;

using MongoDB.Bson;
using MongoDB.Driver.Builders;

using cnt = MACBilling.BillConstants;

namespace MACBilling
{
    public partial class HistoryPopup : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Deny access if user request not logged in
            var isAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
            if (!isAuthenticated)
            {
                Response.Write("Hello world!");
                Response.End();
            }

            var myUtils = new Utils();
            var billUtils = new BillUtils();

            var loggedInAdminId = "";
            if (!String.IsNullOrEmpty(HttpContext.Current.Request["loggedInAdminId"]))
                loggedInAdminId = HttpContext.Current.Request["loggedInAdminId"];

            loggedInAdminId = MACSecurity.Security.DecodeAndDecrypt(loggedInAdminId, Constants.Strings.DefaultClientId);

            var ownerId = "";
            if (!String.IsNullOrEmpty(HttpContext.Current.Request["ownerId"]))
                ownerId = HttpContext.Current.Request["ownerId"];

            var configType = "";
            if (!String.IsNullOrEmpty(HttpContext.Current.Request["configType"]))
                configType = HttpContext.Current.Request["configType"];

            var billId = "";
            if (!String.IsNullOrEmpty(HttpContext.Current.Request["billId"]))
                billId = HttpContext.Current.Request["billId"];

            var ownerName = "";

            if(configType == "Client")
            {
                var myClient = new Client(ownerId);
                ownerName = myClient.Name;
            }
            else
            {
                var myGroup = new Group(ownerId);
                ownerName = myGroup.Name;
            }

            spanHistory.InnerHtml = ownerName + " Billing History";

            var adminProfile = new UserProfile(loggedInAdminId);

            var adminFirstName = MACSecurity.Security.DecodeAndDecrypt(adminProfile.FirstName, adminProfile._id.ToString());
            var adminLastName = MACSecurity.Security.DecodeAndDecrypt(adminProfile.LastName, adminProfile._id.ToString());
            var adminFullName = adminFirstName + " " + adminLastName;

            var sbResponse = new StringBuilder();

            sbResponse.Append("<div style='width: 100%; height: 20px; border: solid 0px #ff0000;'>");
            sbResponse.Append("     <div style='float: left; width: 165px; border: solid 0px #ff0000; color: #808080;'>Created</div>");
            sbResponse.Append("     <div style='float: left; width: 165px; border: solid 0px #ff0000; color: #808080;'>Due</div>");
            sbResponse.Append("     <div style='float: left; width: 100px; border: solid 0px #ff0000; color: #808080;'>Amount</div>");
            sbResponse.Append("     <div style='float: left; width: 80px; border: solid 0px #ff0000; color: #808080;'>Status</div>");
            sbResponse.Append("</div>");

            var billQuery = Query.Null;
            var sortBy = SortBy.Descending("DateCreated");

            if (IsPostBack)
            {
                billQuery = Query.And(Query.EQ("OwnerId", ownerId), Query.EQ("HasBeenAttached", false));
            }
            else
            {

                billQuery = Query.EQ("OwnerId", ownerId);

                var billHistory = myUtils.mongoDBConnectionPool.GetCollection("Archive").FindAs<BillArchive>(billQuery).SetSortOrder(sortBy);
                foreach (BillArchive billArchive in billHistory)
                {
                    sbResponse.Append("<a href='javascript: viewBillDetails(&quot;" + billArchive._id + "&quot;)' id='link_viewBillDetails'>");
                    sbResponse.Append("<div style='width: 100%; height: 20px; border: solid 0px #ff0000; margin-top: 10px; margin-bottom: 10px; padding-bottom: 23px; border-bottom: solid 1px #e6e6e6;'>");
                    sbResponse.Append("     <div style='float: left; width: 165px; border: solid 0px #ff0000;'>" + billArchive.DateCreated.ToLocalTime() + "</div>");
                    sbResponse.Append("     <div style='float: left; width: 165px; border: solid 0px #ff0000;'>" + billArchive.DateDue.ToLocalTime() + "</div>");
                    sbResponse.Append("     <div style='float: left; width: 100px; border: solid 0px #ff0000;'>" + billUtils.FormatMoney(billArchive.Amount) + "</div>");

                    var billStatus = "<span style=''>Not paid</span>";

                    switch (billArchive.IsPaid)
                    {
                        case true:
                            billStatus = "<span style='color: #00ff00;'>Paid</span>";
                            break;

                        case false:
                            if (billArchive.DateDue < DateTime.UtcNow)
                                billStatus = "<span style='color: #ff0000;'>Overdue!</span>";
                            break;
                    }
                    sbResponse.Append("     <div style='float: left; width: 80px; border: solid 0px #ff0000;'>" + billStatus + "</div>");
                    sbResponse.Append("</div>");
                    sbResponse.Append("</a>");
                }

                divBillingHistory.InnerHtml = sbResponse.ToString();
            }
        }
    }
}