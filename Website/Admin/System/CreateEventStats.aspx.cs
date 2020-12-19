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
    public partial class CreateEventStats : Page
    {
        Utils myUtils = new Utils();

        public Random rnd = new Random();

        public UserProfile adminProfile;

        public string adminUserId = "";
        public string adminFirstName = "";
        public string adminLastName = "";

        public DateTime endDate;
        public DateTime startDate;
        public DateTime currentProcessDate;

        public int dateRangeSpan = 7;

        public Double lowerBound = 0;
        public Double upperBound = 99999;

        public EventStat myStats;

        public Int32 AdMessageSent = 0;
        public Int32 AdEnterOtpScreenSent = 0;
        public Int32 AdVerificationScreenSent = 0;
        public Int32 EndUserRegister = 0;
        public Int32 EndUserVerify = 0;
        new public Int32 Events = 0;
        public Int32 OtpSentEmail = 0;
        public Int32 OtpSentSms = 0;
        public Int32 OtpSentVoice = 0;
        public Int32 OtpValid = 0;
        public Int32 OtpInvalid = 0;
        public Int32 OtpExpired = 0;

        public Int32 TotalEndUserEvents = 0;
        public Int32 TotalOtpSentEvents = 0;
        public Int32 TotalOtpValidationEvents = 0;
        public Int32 TotalAdPassEvents = 0;

        public Int32 randomFactor;

        public Double percentFactor;

        public int currentDateIndex = 0;

        public StringBuilder sbResponse = new StringBuilder();

        public bool bShowOutPut = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Deny access if user request not logged in
            var isAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
            if (!isAuthenticated)
            {
                Response.Write("Hello world!");
                Response.End();
            }

            if (Request["userId"] != null)
            {
                adminUserId = Request["userId"].ToString();

                adminUserId = MACSecurity.Security.DecodeAndDecrypt(adminUserId, Constants.Strings.DefaultClientId);

                adminProfile = new UserProfile(adminUserId);

                adminFirstName = MACSecurity.Security.DecodeAndDecrypt(adminProfile.FirstName, adminUserId);
                adminLastName = MACSecurity.Security.DecodeAndDecrypt(adminProfile.LastName, adminUserId);
            }

            statsTable.Visible = false;

            if (Request["su"] != null)
                bShowOutPut = Convert.ToBoolean(Request["su"].ToString());

            if (txtEndDate.Text == "")
            {
                endDate = DateTime.Now;
                txtEndDate.Text = endDate.ToShortDateString();
            }
            else
            {
                endDate = DateTime.Parse(txtEndDate.Text);
            }

            // Calc start date
            if (txtStartDate.Text == "")
            {
                startDate = endDate.AddDays(-dateRangeSpan);
                txtStartDate.Text = startDate.ToShortDateString();
            }
            else
            {
                startDate = DateTime.Parse(txtStartDate.Text);
            }

            TimeSpan dateDiff = endDate.Subtract(startDate);
            dateRangeSpan = Convert.ToInt32(dateDiff.Days);

            if (txtLowerBound.Text == "")
                txtLowerBound.Text = lowerBound.ToString();
            else
                lowerBound = Convert.ToInt32(txtLowerBound.Text);

            if (txtUpperBound.Text == "")
                txtUpperBound.Text = upperBound.ToString();
            else
                upperBound = Convert.ToInt32(txtUpperBound.Text);

            if (IsPostBack) // Run the process
            {
                try
                {
                    // Drop the EventStats collection first
                    myUtils.mongoDBConnectionPool.DropCollection("EventStat");

                    // Get all clients. Loop through each and generate numbers
                    var mongoCollection = myUtils.mongoDBConnectionPool.GetCollection("Client");
                    var clientCollection = mongoCollection.FindAllAs<Client>();

                    var sortBy = SortBy.Ascending("Name");
                    clientCollection.SetSortOrder(sortBy);

                    foreach (Client currentClient in clientCollection)
                    {
                        // Create Client stat collection
                        myStats = new EventStat(currentClient._id);
                        myStats.OwnerId = currentClient._id;
                        myStats.OwnerName = currentClient.Name;
                        myStats.DailyStats = new List<EventStatDay>();

                        // Date range loop
                        for (int i = 0; i < dateRangeSpan; i++ )
                        {
                            AdMessageSent = 0;
                            AdEnterOtpScreenSent = 0;
                            AdVerificationScreenSent = 0;
                            EndUserRegister = 0;
                            EndUserVerify = 0;
                            Events = 0;
                            OtpSentEmail = 0;
                            OtpSentSms = 0;
                            OtpSentVoice = 0;
                            OtpValid = 0;
                            OtpInvalid = 0;
                            OtpExpired = 0;

                            TotalEndUserEvents = 0;
                            TotalOtpSentEvents = 0;
                            TotalOtpValidationEvents = 0;
                            TotalAdPassEvents = 0;

                            currentProcessDate = endDate.AddDays(-i);

                            var tmpResponse = GenerateDayStats(currentClient, currentProcessDate);

                            sbResponse.Append(tmpResponse);
                        }

                        // Update the stats
                        myStats.Create();
                    }

                    divClientsProcessed.InnerHtml = sbResponse.ToString();
 
                    RenderStats();
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch(Exception ex)
                {
                    var errMsg = ex.ToString();
                }

                ClientScript.RegisterStartupScript(typeof(Page), "closePage", "<script type='text/JavaScript'>callParentDocumentFunction();</script>");
            }
            else // 
            {

            }
        }

        private string GenerateDayStats(Client currentClient, DateTime currentDay)
        {
            StringBuilder sbCurrentData = new StringBuilder();

            var eventDetails = "";

            #region Construct stat values

                // User registration is the starting point for our stat formula
                EndUserRegister = Convert.ToInt32(GenerateRandomNumber(lowerBound, upperBound));

                percentFactor = GenerateRandomNumber(0.00, 0.25);
                EndUserVerify = Convert.ToInt32(EndUserRegister * percentFactor);

                TotalEndUserEvents = EndUserRegister + EndUserVerify;

                // OTP Sent Events
                randomFactor = Convert.ToInt32(GenerateRandomNumber(1, 3));
                OtpSentSms = (EndUserRegister / randomFactor) * (Convert.ToInt32(GenerateRandomNumber(0, 5)));

                randomFactor = Convert.ToInt32(GenerateRandomNumber(4, 6));
                OtpSentEmail = (EndUserRegister / randomFactor) * (Convert.ToInt32(GenerateRandomNumber(0, 1.5)));

                randomFactor = Convert.ToInt32(GenerateRandomNumber(7, 9));
                OtpSentVoice = (EndUserRegister / randomFactor) * (Convert.ToInt32(GenerateRandomNumber(0.5, 1.0)));

                TotalOtpSentEvents = OtpSentSms + OtpSentEmail + OtpSentVoice;

                // OTP Validation Events
                OtpInvalid = Convert.ToInt32(TotalOtpSentEvents * 0.05);
                OtpExpired = Convert.ToInt32(TotalOtpSentEvents * 0.01);
                OtpValid = (TotalOtpSentEvents - OtpInvalid) - OtpExpired;
                TotalOtpValidationEvents = OtpInvalid + OtpExpired + OtpValid;

                // AdPass Events
                percentFactor = GenerateRandomNumber(0.00, 0.60);
                AdMessageSent = Convert.ToInt32(TotalOtpSentEvents * percentFactor);

                percentFactor = GenerateRandomNumber(0.00, 0.40);
                AdEnterOtpScreenSent = Convert.ToInt32(AdMessageSent * percentFactor);

                percentFactor = GenerateRandomNumber(0.00, 0.20);
                AdVerificationScreenSent = Convert.ToInt32(AdEnterOtpScreenSent * percentFactor);

                TotalAdPassEvents = AdMessageSent + AdEnterOtpScreenSent + AdVerificationScreenSent;

                Events += EndUserRegister;
                Events += EndUserVerify;
                Events += OtpSentEmail;
                Events += OtpSentSms;
                Events += OtpSentVoice;
                Events += OtpValid;
                Events += OtpInvalid;
                Events += OtpExpired;
                Events += AdMessageSent;
                Events += AdEnterOtpScreenSent;
                Events += AdVerificationScreenSent;

            #endregion

            #region Create the stats instance

            EventStatDay currentStatDay = new EventStatDay();
                currentStatDay.Date = currentDay;
                currentStatDay.AdEnterOtpScreenSent = AdEnterOtpScreenSent;
                currentStatDay.AdMessageSent = AdMessageSent;
                currentStatDay.AdVerificationScreenSent = AdVerificationScreenSent;
                currentStatDay.EndUserRegister = EndUserRegister;
                currentStatDay.EndUserVerify = EndUserVerify;
                currentStatDay.Events = Events;
                currentStatDay.Exceptions = 0;
                currentStatDay.OtpExpired = OtpExpired;
                currentStatDay.OtpInvalid = OtpInvalid;
                currentStatDay.OtpSentEmail = OtpSentEmail;
                currentStatDay.OtpSentSms = OtpSentSms;
                currentStatDay.OtpSentVoice = OtpSentVoice;
                currentStatDay.OtpValid = OtpValid;

                myStats.DailyStats.Add(currentStatDay);

            #endregion

            #region Create output StringBuilder

                if (bShowOutPut)
                {
                    sbCurrentData.Append("<div style='height: 25px; border-bottom: solid 1px #c0c0c0; padding: 5px; overflow-y:auto !important;'>");

                    sbCurrentData.Append("<div style='float: left; width:225px;'>");
                    sbCurrentData.Append(currentClient.Name);
                    sbCurrentData.Append("</div>");

                    sbCurrentData.Append("<div style='float: left; width:100px;'>");
                    sbCurrentData.Append("Date: <span style='color: #ff0000;'>" + currentProcessDate.ToShortDateString() + "</span> ");
                    sbCurrentData.Append("</div>");

                    sbCurrentData.Append("<div style='float: left;'>");
                    sbCurrentData.Append("<span style='color: #c0c0c0;'>|</span> ");
                    sbCurrentData.Append("End User: <span style='color: #ff0000;'>" + myUtils.FormatNumber(TotalEndUserEvents.ToString()) + "</span> <span style='color: #c0c0c0;'>|</span> ");
                    sbCurrentData.Append("OTP Sent: <span style='color: #ff0000;'>" + myUtils.FormatNumber(TotalOtpSentEvents.ToString()) + "</span> <span style='color: #c0c0c0;'>|</span> ");
                    sbCurrentData.Append("OTP Validation: <span style='color: #ff0000;'>" + myUtils.FormatNumber(TotalOtpValidationEvents.ToString()) + "</span> <span style='color: #c0c0c0;'>|</span> ");
                    sbCurrentData.Append("AdPass: <span style='color: #ff0000;'>" + myUtils.FormatNumber(TotalAdPassEvents.ToString()) + "</span> <span style='color: #c0c0c0;'>|</span> ");
                    sbCurrentData.Append("Events: <span style='color: #ff0000;'>" + myUtils.FormatNumber(Events.ToString()) + "</span>");
                    sbCurrentData.Append("</div>");

                    sbCurrentData.Append("</div>");
                }
                else
                {
                    sbCurrentData.Append("<div style='height: 25px; border-bottom: solid 1px #c0c0c0; padding: 5px; overflow-y:auto !important;'>");

                    sbCurrentData.Append("<div style='float: left; width:225px;'>");
                    sbCurrentData.Append(currentClient.Name);
                    sbCurrentData.Append("</div>");

                    sbCurrentData.Append("<div style='float: left; width:100px;'>");
                    sbCurrentData.Append("Date: <span style='color: #ff0000;'>" + currentProcessDate.ToShortDateString() + "</span> ");
                    sbCurrentData.Append("</div>");

                    sbCurrentData.Append("</div>");
                }

                eventDetails += "(Date - " + currentProcessDate.ToShortDateString() + ") ";
                eventDetails += "(End User - " + myUtils.FormatNumber(TotalEndUserEvents.ToString()) + ") ";
                eventDetails += "(OTP Sent - " + myUtils.FormatNumber(TotalOtpSentEvents.ToString()) + ") ";
                eventDetails += "(OTP Valid - " + myUtils.FormatNumber(TotalOtpValidationEvents.ToString()) + ") ";
                eventDetails += "(AdPass - " + myUtils.FormatNumber(TotalAdPassEvents.ToString()) + ") ";
                eventDetails += "(Events - " + myUtils.FormatNumber(Events.ToString()) + ")";

            #endregion

            #region Log the stat event

                // Log the stats change
                var statEvent = new Event
                {
                    ClientId = currentClient._id,
                    UserId = ObjectId.Parse(adminUserId),
                    EventTypeDesc = Constants.TokenKeys.ClientName + currentClient.Name
                                    + Constants.TokenKeys.EventStatDate + currentDay
                                    + Constants.TokenKeys.EventStatDetails + eventDetails
                                    + Constants.TokenKeys.EventGeneratedByName + adminFirstName + " " + adminLastName
                };
                statEvent.Create(Constants.EventLog.System.EventStatsReset, null);

            #endregion

            return sbCurrentData.ToString();
        }

        private Double GenerateRandomNumber(Double lowerBound, Double upperBound)
        {
            Double randomNumber = rnd.NextDouble() * (upperBound - lowerBound) + lowerBound;

            return randomNumber;
        }

        private void RenderStats()
        {
            EndUserRegisterCount.InnerText = myUtils.FormatNumber(EndUserRegister.ToString());
            EndUserVerifyCount.InnerText = myUtils.FormatNumber(EndUserVerify.ToString());
            spanUserEventCount.InnerHtml = myUtils.FormatNumber((EndUserRegister + EndUserVerify).ToString());

            OtpSentEmailCount.InnerText = myUtils.FormatNumber(OtpSentEmail.ToString());
            OtpSentSmsCount.InnerText = myUtils.FormatNumber(OtpSentSms.ToString());
            OtpSentVoiceCount.InnerText = myUtils.FormatNumber(OtpSentVoice.ToString());
            spanOtpSentEventCount.InnerHtml = myUtils.FormatNumber((OtpSentEmail + OtpSentSms + OtpSentVoice).ToString());

            OtpValidCount.InnerText = myUtils.FormatNumber(OtpValid.ToString());
            OtpInvalidCount.InnerText = myUtils.FormatNumber(OtpInvalid.ToString());
            OtpExpiredCount.InnerText = myUtils.FormatNumber(OtpExpired.ToString());
            spanOtpValidationEventCount.InnerHtml = myUtils.FormatNumber((OtpValid + OtpInvalid + OtpExpired).ToString());


            AdMessageSentCount.InnerText = myUtils.FormatNumber(AdMessageSent.ToString());
            AdEnterOtpScreenSentCount.InnerText = myUtils.FormatNumber(AdEnterOtpScreenSent.ToString());
            AdVerificationScreenSentCount.InnerText = myUtils.FormatNumber(AdVerificationScreenSent.ToString());
            spanAdEventCount.InnerHtml = myUtils.FormatNumber((AdMessageSent + AdEnterOtpScreenSent + AdVerificationScreenSent).ToString());

            EventsCount.InnerText = myUtils.FormatNumber(Events.ToString());

            if (EndUserRegisterCount.InnerText == "")
                EndUserRegisterCount.InnerText = "0";

            if (EndUserVerifyCount.InnerText == "")
                EndUserVerifyCount.InnerText = "0";

            if (AdMessageSentCount.InnerText == "")
                AdMessageSentCount.InnerText = "0";

            if (AdEnterOtpScreenSentCount.InnerText == "")
                AdEnterOtpScreenSentCount.InnerText = "0";

            if (AdVerificationScreenSentCount.InnerText == "")
                AdVerificationScreenSentCount.InnerText = "0";

            if (OtpSentEmailCount.InnerText == "")
                OtpSentEmailCount.InnerText = "0";

            if (OtpSentSmsCount.InnerText == "")
                OtpSentSmsCount.InnerText = "0";

            if (OtpSentVoiceCount.InnerText == "")
                OtpSentVoiceCount.InnerText = "0";

            if (OtpValidCount.InnerText == "")
                OtpValidCount.InnerText = "0";

            if (OtpInvalidCount.InnerText == "")
                OtpInvalidCount.InnerText = "0";

            if (OtpExpiredCount.InnerText == "")
                OtpExpiredCount.InnerText = "0";

            if (EventsCount.InnerText == "")
                EventsCount.InnerText = "0";
        }
    }
}