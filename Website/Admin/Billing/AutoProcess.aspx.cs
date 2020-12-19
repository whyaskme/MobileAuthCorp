using System;
using System.Web;
using System.Web.Script.Serialization;

using MACBilling;
using MACServices;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

using dk = MACBilling.BillConstants;
using cs = MACServices.Constants.Strings;

public partial class Admin_Billing_AutoProcess : System.Web.UI.Page
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

        var billCount = 0;
        var iCount = 0;

        Decimal billCollectionTotal = 0.00M;

        var lastDayOfMonth = DateTime.DaysInMonth(DateTime.UtcNow.Year, DateTime.UtcNow.Month);
        var todayDate = DateTime.UtcNow.Date.Day;

        var lastBillDate = DateTime.UtcNow;
        BillClient myBill;

        var mUtils = new BillUtils();

        // Only process billing if last day of month
        // This value change is only for testing
        todayDate = lastDayOfMonth;
        if (todayDate == lastDayOfMonth)
        {
            var mongoDBConnectionPool = (MongoDatabase)HttpContext.Current.Application[cs.MongoDB];

            MongoCollection clientCollection = mongoDBConnectionPool.GetCollection("Client");
            MongoCollection billCollection = mongoDBConnectionPool.GetCollection("Archive");

            var query = Query.EQ("_t", "Client");
            var sortBy = SortBy.Ascending("Name");

            var clientList = clientCollection.FindAs<Client>(query).SetSortOrder(sortBy);
            foreach (Client currentClient in clientList)
            {
                // Reset lastBillDate for current iteration
                lastBillDate = DateTime.UtcNow;

                // First, get last bill date to calculate current charges
                query = Query.And(Query.EQ("_t", "BillArchive"), Query.EQ("OwnerId", currentClient._id));
                sortBy = SortBy.Descending("DateCreated");

                var billList = billCollection.FindAs<BillArchive>(query).SetSortOrder(sortBy);
                foreach (var archiveBill in billList)
                {
                    if (iCount == 0)
                        lastBillDate = archiveBill.DateCreated;

                    iCount++;
                }

                // Calculate pending charges since lastBillDate
                myBill = new BillClient("Current (Pending) Charges", currentClient._id.ToString(), cs.DefaultClientAdminId);

                // Only continue processing if the total is greater than $0.00. No need to process if there's nothing to process
                if(myBill.Total > 0)
                {
                    billCount++;

                    billCollectionTotal += Convert.ToDecimal(myBill.Total.ToString());

                    // Process any addendums if present
                    foreach (BillAddendum currAddendum in myBill.Addendums)
                    {
                        currAddendum._id = ObjectId.Parse(currAddendum.AddendumId);
                        currAddendum.AttachedToBillId = myBill._id;
                        currAddendum.DateAttached = DateTime.UtcNow;
                        currAddendum.HasBeenAttached = true;

                        currAddendum.OwnerId = currentClient._id;
                        currAddendum.CreatedById = ObjectId.Parse(cs.DefaultClientAdminId);

                        // Update the addendum in the db
                        currAddendum.UpdateAddendum(currAddendum, currAddendum._id.ToString());
                    }

                    BillArchive myArchive = new BillArchive();
                    //myArchive.OwnerId = myBill.ClientId;
                    myArchive.OwnerId = myBill.ClientId.ToString();

                    var myJsonBill = (new JavaScriptSerializer()).Serialize(myBill);

                    myArchive._id = myBill._id;

                    myArchive.BillDetails = MACSecurity.Security.EncryptAndEncode(myJsonBill, myBill.ClientId.ToString());

                    myArchive.Amount = myBill.Total;

                    myArchive.CreatedById = ObjectId.Parse(cs.DefaultClientAdminId);
                    myArchive.UpdatedById = ObjectId.Parse(cs.DefaultClientAdminId);

                    myArchive.DateCreated = DateTime.UtcNow.AddDays(0); // AddDays just for dev testing
                    myArchive.DateSent = DateTime.UtcNow;

                    // Need to double-check the date due
                    myArchive.DateDue = myBill.DateDue;

                    //myArchive.OwnerId = currentClient._id;
                    myArchive.OwnerId = currentClient._id.ToString();
                    myArchive.OwnerName = currentClient.Name;
                    myArchive.OwnerType = "Client";

                    // Just testing
                    //myArchive.Archive(myArchive);

                    var adminProfile = new UserProfile(cs.DefaultSystemAdminId);
                    var adminFirstName = MACSecurity.Security.DecodeAndDecrypt(adminProfile.FirstName, cs.DefaultSystemAdminId);
                    var adminLastName = MACSecurity.Security.DecodeAndDecrypt(adminProfile.LastName, cs.DefaultSystemAdminId);

                    // Log the billing event
                    var myBillEvent = new Event();
                    myBillEvent.EventTypeDesc = Constants.TokenKeys.BillAmount + "<a href='/Admin/Billing/Client/Default.aspx?bid=" + myBill._id + "&pa=ViewBillDetails'>" + mUtils.FormatMoney(Convert.ToDecimal(myBill.Total.ToString())) + "</a>";
                    myBillEvent.EventTypeDesc += Constants.TokenKeys.ClientName + myArchive.OwnerName;
                    myBillEvent.EventTypeDesc += Constants.TokenKeys.EventGeneratedByName + (adminFirstName + " " + adminLastName);

                    myBillEvent.EventTypeName = Constants.EventLog.Billing.Created.Item2;
                    myBillEvent.EventTypeId = Constants.EventLog.Billing.Created.Item1;
                    myBillEvent.ClientId = currentClient._id;

                    myBillEvent.Create(Constants.EventLog.Billing.Created, null);

                    // Now figure out who to send the bill to from the client config and send it
                    BillConfig myConfig = new BillConfig(currentClient._id.ToString(), "Client", cs.DefaultClientAdminId);
                    var sendToEmails = myConfig.BillingSendTo;

                    if(sendToEmails.Length > 0)
                    {
                        if(sendToEmails.Contains(","))
                        {
                            var tmpEmails = sendToEmails.Split(',');
                            foreach(var currEmail in tmpEmails)
                            {

                            }
                        }
                        else
                        {

                        }
                    }
                    else
                    {

                    }

                    foreach(var currentEmail in myConfig.BillingSendTo)
                    {

                    }

                    // And process payment and mark as paid

                    Response.Write("<br />" + billCount + ") " + currentClient.Name + "'s bill for " + mUtils.FormatMoney(Convert.ToDecimal(myBill.Total.ToString())) + " due on " + myBill.DateDue);
                    Response.Flush();
                }
                else
                {
                    billCount++;

                    Response.Write("<br /><span style='color: #ff0000;'>" + billCount + ") " + currentClient.Name + " has no current bill</span>");
                    Response.Flush();
                }
            }
        }
        else
        {
            
        }

        Response.Write("<hr /><b>Finished processing " + billCount + " bills for a total of " + mUtils.FormatMoney(Convert.ToDecimal(billCollectionTotal.ToString())) + "</b>");
    }
}