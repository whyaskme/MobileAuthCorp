using System;
using System.Text;
using System.Net;
using System.Web;
using System.Web.UI;

using MACSecurity;
using MACServices;

using dk = MACServices.Constants.Dictionary.Keys;

namespace MACAdmin.Reporting
{
    public partial class EventPopup : Page
    {
        Utils mUtils = new Utils();
        public StringBuilder sbResponse = new StringBuilder();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Deny access if user request not logged in
            var isAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
            if (!isAuthenticated)
            {
                Response.Write("Hello world!");
                Response.End();
            }

            int eventCount = 0;

            if (Request["eventIds"] != null)
                hiddenEventId.Value = Request["eventIds"].ToString();

            try
            {
                var selectedEvents = hiddenEventId.Value.Split('|');
                foreach (var selectedEventId in selectedEvents)
                {
                    if (!String.IsNullOrEmpty(selectedEventId))
                    {
                        eventCount++;

                        var currentEvent = new Event(selectedEventId);

                        sbResponse.Append("<div style='border: solid 1px #808080; padding-top: 15px; margin-bottom: 15px;' id='divEvent_" + selectedEventId + "'>");
                        sbResponse.Append("<table style='width: 100%;'>");

                        sbResponse.Append("     <tr>");
                        sbResponse.Append("         <td colspan='2'>");
                        sbResponse.Append(
                            "             <div style='float: left; color: #808080; margin-right: 5px; border: solid 0px #ff0000; min-width: 60px; text-align: right;'>EventId:</div><div style='float: left;'>" +
                            selectedEventId + "</div>");
                        sbResponse.Append("         </td>");
                        sbResponse.Append("     </tr>");

                        sbResponse.Append("     <tr>");
                        sbResponse.Append("         <td style='width: 50%;'>");
                        //row 1
                        sbResponse.Append(
                            "             <div style='float: left; color: #808080; margin-right: 5px; border: solid 0px #ff0000; min-width: 60px; text-align: right;'>Date:</div><div style='float: left;'>" +
                            currentEvent.Date + "</div>");
                        sbResponse.Append("         </td>");

                        var adminFirstName = "Unknown";
                        var adminLastName = "User";

                        if (currentEvent.UserId.ToString() != Constants.Strings.DefaultEmptyObjectId)
                        {
                            var userId = currentEvent.UserId.ToString();
                            var userProfile = new UserProfile(userId);

                            if (!String.IsNullOrEmpty(userProfile.FirstName))
                                adminFirstName = Security.DecodeAndDecrypt(userProfile.FirstName, userId);

                            if (!String.IsNullOrEmpty(userProfile.LastName))
                                adminLastName = Security.DecodeAndDecrypt(userProfile.LastName, userId);
                        }

                        sbResponse.Append("         <td style='width: 50%;'>");
                        sbResponse.Append(
                            "             <div style='float: left; color: #808080; margin-right: 5px; border: solid 0px #ff0000; min-width: 60px; text-align: right;'>User:</div><div style='float: left;'>" +
                            adminFirstName + " " + adminLastName + "</div>");
                        sbResponse.Append("         </td>");
                        sbResponse.Append("     </tr>");
                        //row 2
                        sbResponse.Append("     <tr>");
                        sbResponse.Append("         <td>");
// ReSharper disable once RedundantAssignment
                        var userLocation = @"Need to Geo Locate User IP";

                        var userIp = currentEvent.UserIpAddress.Trim();
                        if (userIp == "::1")
                        {
                            userIp = "127.0.0.1";
                            userLocation = "localhost";
                        }
                        else
                        {
                            userLocation = mUtils.GeoLocationByUserIp(userIp);
                        }
                        sbResponse.Append(
                            "             <div style='float: left; color: #808080; margin-right: 5px; border: solid 0px #ff0000; min-width: 60px; text-align: right;'>IP:</div><div style='float: left;'>" +
                            userIp + "</div>");
                        sbResponse.Append("         </td>");
                        sbResponse.Append("         <td>");
                        sbResponse.Append(
                            "             <div style='float: left; color: #808080; margin-right: 5px; border: solid 0px #ff0000; min-width: 60px; text-align: right;'>Geo:</div><div style='float: left;'>" +
                            userLocation + "</div>");
                        sbResponse.Append("         </td>");
                        sbResponse.Append("     </tr>");
                        //row 3
                        sbResponse.Append("     <tr>");
                        sbResponse.Append("         <td>");
                        var clientName = Constants.Strings.DefaultClientName;

                        var clientId = currentEvent.ClientId.ToString();
                        if (clientId != Constants.Strings.DefaultEmptyObjectId)
                        {
                            var currentClient = mUtils.GetClientUsingClientId(clientId);

                            if (currentClient != null)
                                clientName = currentClient.Name;
                            else
                                clientName = "Unknown Client";
                        }
                        sbResponse.Append(
                            "             <div style='float: left; color: #808080; margin-right: 5px; border: solid 0px #ff0000; min-width: 60px; text-align: right;'>Client:</div><div style='float: left;'>" +
                            clientName + "</div>");
                        sbResponse.Append("         </td>");
                        sbResponse.Append("         <td>");
                        sbResponse.Append(
                            "             <div style='float: left; color: #808080; margin-right: 5px; border: solid 0px #ff0000; min-width: 60px; text-align: right;'>Group:</div><div style='float: left;'>Undetermined</div>");
                        sbResponse.Append("         </td>");
                        sbResponse.Append("     </tr>");
                        //row 4
                        sbResponse.Append("     <tr>");
                        sbResponse.Append("         <td colspan='2'>");
                        sbResponse.Append(
                            "             <div style='float: left; color: #808080; margin-right: 5px; border: solid 0px #ff0000; min-width: 60px; text-align: right;'>Event:</div><div style='float: left;'>" +
                            currentEvent.EventTypeName + "</div>");
                        sbResponse.Append("         </td>");
                        sbResponse.Append("     </tr>");
                        //row 5
                        sbResponse.Append("     <tr>");
                        sbResponse.Append(
                            "         <td colspan='2' style='word-break: break-all; border-top: solid 1px #c0c0c0; padding: 10px;'>");
                        var eventDetails = currentEvent.EventTypeDesc;
                        if (!String.IsNullOrEmpty(currentEvent.EventTypeName))
                        {
                            if (eventDetails.StartsWith("<?xml version="))
                            {
                                eventDetails = (WebUtility.HtmlEncode(eventDetails)).Replace("&gt;&lt;",
                                    "&gt;<br />&lt;");
                            }
                            else
                            {
                                eventDetails = eventDetails.Replace(currentEvent.EventTypeName + " - ", "");
                            }
                        }
                        sbResponse.Append(eventDetails);
                        sbResponse.Append("         </td>");
                        sbResponse.Append("     </tr>");
                        sbResponse.Append("</table>");

                        sbResponse.Append("</div>");
                    }
                }
                divEvents.InnerHtml = sbResponse.ToString();

                spanProviderName.InnerHtml = "&nbsp;&nbsp;&nbsp;Viewing " + eventCount + " Events";
            }
            catch (Exception ex)
            {
                var mDetails = ex.ToString().Replace(dk.ItemSep, "!").Replace(dk.KVSep, ";") +
                               Constants.TokenKeys.ClientName + "NA";
                var exceptionEvent = new Event
                {
                    EventTypeDesc = Constants.TokenKeys.ExceptionDetails + mDetails
                };
                exceptionEvent.Create(Constants.EventLog.Exceptions.General, null);
            }
        }

        public void btnSendEmail_Click(object sender, EventArgs e)
        {
            lbError.Text = "";

            var mSubject = "Event";
            if (!String.IsNullOrEmpty(txtSubject.Value))
                mSubject = "Events - " + txtSubject.Value;
            
            if (String.IsNullOrEmpty(txtEmail.Value))
            {
                // Commented for security reasons.
                lbError.Text = ""; // @"To Email address required!";
                return;
            }

            var rtn = mUtils.SendGenericEmail(Constants.Strings.DefaultClientId, "Client", Constants.Strings.DefaultFromEmail, txtEmail.Value.Trim(), mSubject, sbResponse.ToString(), true);
            if (rtn == false)
            {
                lbError.Text =
                    @"Unable to send email containing the selected events. Contact your system administrator.";
                return;
            }
            ClientScript.RegisterStartupScript(typeof(Page), "closePage", "<script type='text/JavaScript'>callParentDocumentFunction();</script>");
        }
    }
}