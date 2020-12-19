using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;

using MACServices;

using MongoDB.Bson;

using cnt = MACBilling.BillConstants;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;

namespace MACBilling
{
    public partial class SendToPopup : Page
    {
        public Utils myUtils = new Utils();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Deny access if user request not logged in
            var isAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
            if (!isAuthenticated)
            {
                Response.Write("Hello world!");
                Response.End();
            }

            var FromAddress = ConfigurationManager.AppSettings[cfg.FromAddress];
            
            var loggedInAdminId = "";
            if (!String.IsNullOrEmpty(HttpContext.Current.Request["loggedInAdminId"]))
                loggedInAdminId = HttpContext.Current.Request["loggedInAdminId"];

            loggedInAdminId = MACSecurity.Security.DecodeAndDecrypt(loggedInAdminId, Constants.Strings.DefaultClientId);

            var clientId = "";
            if (!String.IsNullOrEmpty(HttpContext.Current.Request["clientId"]))
                clientId = HttpContext.Current.Request["clientId"];

            var configType = "";
            if (!String.IsNullOrEmpty(HttpContext.Current.Request["configType"]))
                configType = HttpContext.Current.Request["configType"];

            if (!String.IsNullOrEmpty(HttpContext.Current.Request["billId"]))
                hiddenV.Value = HttpContext.Current.Request["billId"];

            var myClient = new Client(clientId);
            var myBillConfig = new BillConfig(clientId, configType, loggedInAdminId);

            var billSentTo = "";

            spanSendTo.InnerHtml = "Send " + myClient.Name + "'s Bill To";

            var adminProfile = new UserProfile(loggedInAdminId);

            var adminFirstName = MACSecurity.Security.DecodeAndDecrypt(adminProfile.FirstName, adminProfile._id.ToString());
            var adminLastName = MACSecurity.Security.DecodeAndDecrypt(adminProfile.LastName, adminProfile._id.ToString());
            var adminFullName = adminFirstName + " " + adminLastName;

            List<string> notifyUsers = new List<string>();

            if (!string.IsNullOrEmpty(myBillConfig.NotifyUserIds))
            {
                if(myBillConfig.NotifyUserIds.Contains("|"))
                {
                    var arrUserIds = myBillConfig.NotifyUserIds.Split('|');
                    foreach(string currentUserId in arrUserIds)
                    {
                        if (currentUserId != "")
                        {
                            UserProfile myProfile = new UserProfile(currentUserId);
                            var userEmail = MACSecurity.Security.DecodeAndDecrypt(myProfile.Contact.Email, currentUserId);
                            notifyUsers.Add(userEmail);
                        }
                    }
                }
                else
                {
                    notifyUsers.Add(MACServices.Constants.Strings.DefaultAccountingEmail);
                }
            }
            else
            {
                notifyUsers.Add(MACServices.Constants.Strings.DefaultAccountingEmail);
            }

            var myBillUtils = new BillUtils();

            if (IsPostBack)
            {
                var currentServer = "http://" + Request.ServerVariables["SERVER_NAME"];

                var MessageSubject = "";
                var MessageBody = "";

                if (Notes.Value.Length > 0)
                    MessageBody += "<p>" + Notes.Value + "</p>";

                // Send bill now
                foreach (var currEmailAddress in notifyUsers)
                {
                    billSentTo += currEmailAddress + ", ";
                    MessageSubject = myClient.Name + ", your current MAC bill is ready for review";

                    MessageBody = "<p>Please click the link below to log into your account and view the current bill for (" + myClient.Name + ").</p>";
                    MessageBody += "<p><a href='" + currentServer + "/Admin/Security/Default.aspx?uname=" + currEmailAddress.Trim() + "&bid=" + hiddenV.Value + "&pa=ViewBillDetails' id='link_login'>Log In</a></p>";

                    myUtils.SendGenericEmail(clientId, configType, MACServices.Constants.Strings.DefaultFromEmail, currEmailAddress.Trim(), MessageSubject, MessageBody, true);
                }

                var myBillArchive = new BillArchive(hiddenV.Value);
                //myBillArchive._id = ObjectId.Parse(hiddenV.Value);

                //// Need to rehydrate bill and update date sent property before resaving the archive
                //var tmpBill = MACSecurity.Security.DecodeAndDecrypt(myBillArchive.BillDetails, myBillArchive.OwnerId.ToString());

                //var myBill = (new JavaScriptSerializer()).Deserialize<BillClient>(tmpBill);
                //myBill._id = myBillArchive._id;
                //myBill.DateSent = DateTime.UtcNow;

                //// Update the archive
                //var tmpArchiveDetails = (new JavaScriptSerializer()).Serialize(myBill);
                //myBillArchive.BillDetails = MACSecurity.Security.EncryptAndEncode(tmpArchiveDetails, myBillArchive.OwnerId.ToString());
                //myBillArchive.DateSent = DateTime.UtcNow;
                //myBillArchive.UpdateArchive(myBillArchive, hiddenV.Value);

                // Log the billing event
                var myBillUtil = new BillUtils();

                var myBillEvent = new Event();
                myBillEvent.EventTypeDesc = Constants.TokenKeys.BillAmount + "<a href='/Admin/Billing/Client/Default.aspx?bid=" + hiddenV.Value + "&pa=ViewBillDetails' id='link_viewBillDetails'>" + myBillUtil.FormatMoney(Convert.ToDecimal(myBillArchive.Amount.ToString())) + "</a>";
                myBillEvent.EventTypeDesc += Constants.TokenKeys.ClientName + myBillConfig.OwnerName;
                myBillEvent.EventTypeDesc += Constants.TokenKeys.BillSentTo + billSentTo;
                myBillEvent.EventTypeDesc += Constants.TokenKeys.EventGeneratedByName + (adminFirstName + " " + adminLastName);

                myBillEvent.EventTypeName = Constants.EventLog.Billing.Sent.Item2;
                myBillEvent.EventTypeId = Constants.EventLog.Billing.Sent.Item1;
                myBillEvent.ClientId = myBillConfig.OwnerId;

                myBillEvent.Create(Constants.EventLog.Billing.Sent, null);

                ClientScript.RegisterStartupScript(typeof(Page), "closePage", "<script type='text/JavaScript'>callParentDocumentFunction();</script>");
            }
            else
            {
                foreach(string currentEmail in notifyUsers)
                {
                    txtEmailAdddresses.Value += currentEmail + ",";
                }
                var tmpEmails = txtEmailAdddresses.Value;

                // Remove last comma
                txtEmailAdddresses.Value = tmpEmails.Substring(0, tmpEmails.Length-1);
            }
        }
    }
}