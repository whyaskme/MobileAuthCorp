<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/AdminConsole.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">

    <script type="text/javascript" src="/JavaScript/jquery-1.10.2.js"></script>
    <script type="text/javascript" src="/JavaScript/jquery-ui-1-11-0.js"></script>
    <script type="text/javascript" src="/JavaScript/jquery.timer.js"></script>

    <script type="text/javascript" src="/JavaScript/Queue.js"></script>

    <script type="text/javascript" src="https://www.google.com/jsapi"></script>

     <script>

         google.load("visualization", "1", { packages: ["corechart"] });
         google.setOnLoadCallback(drawChart);

         var chartTimeInterval = 60; // Seconds

         var chartIntervalArray = [];

        // Initialize the array
        resetIntervalData();

         // create a new interval queue
         var intervalQueue = new Queue();

         // Core
         var prevGroupsCount = 0;
         var prevClientsCount = 0;
         var prevEventsCount = 0;
         var prevErrorsCount = 0;

         var currGroupsCount = 0;
         var currClientsCount = 0;
         var currEventsCount = 0;
         var currExceptionsCount = 0;

         var groupsPerSecond = 0;
         var clientsPerSecond = 0;
         var eventsPerSecond = 0;
         var errorsPerSecond = 0;

         // User
         var prevSysAdminsCount = 0;
         var prevGroupAdminsCount = 0;
         var prevClientAdminsCount = 0;
         var prevUsersCount = 0;

         var currSysAdminsCount = 0;
         var currGroupAdminsCount = 0;
         var currClientAdminsCount = 0;
         var currUsersCount = 0;

         var currOtpSentCount = 0;
         var currOtpValidCount = 0;
         var currOtpInvalidCount = 0;
         var currAdsSentCount = 0;

         var usersSysAdminPerSecond = 0;
         var usersGroupAdminPerSecond = 0;
         var usersClientAdminPerSecond = 0;
         var usersPerSecond = 0;

         // Otp
         var prevOtpSentCount = 0;
         var prevOtpValidCount = 0;
         var prevOtpInvalidCount = 0;
         var prevAdsSentCount = 0;

         var currOtpSentCount = 0;
         var currOtpValidCount = 0;
         var currOtpInvalidCount = 0;
         var currAdsSentCount = 0;

         var otpSentPerSecond = 0;
         var otpValidPerSecond = 0;
         var otpInvalidPerSecond = 0;
         var adsSentPerSecond = 0;

         var response;

         var eventTimer;
         function startSystemEventTimer() {

             // Reinit timer
             if (eventTimer != null)
                 eventTimer = null;

             eventTimer = $.timer(function ()
             {
                 document.getElementById("legendCoreStats").innerHTML = "Core<span><img src='/Images/progress-bar-round.gif' style='margin: 0 0 0 0.25rem;height: 16px; width: 16px;' /></span>";
                 document.getElementById("legendUserStats").innerHTML = "Accounts<span><img src='/Images/progress-bar-round.gif' style='margin: 0 0 0 0.25rem;height: 16px; width: 16px;' /></span>";
                 document.getElementById("legendOtpStats").innerHTML = "OTP<span><img src='/Images/progress-bar-round.gif' style='margin: 0 0 0 0.25rem;height: 16px; width: 16px;' /></span>";

                 GetSystemStats();
             });
             eventTimer.once(1000);
         }

         function resetIntervalData()
         {
             var cid = $('#dlClients').val();
             var uid = $('#hiddenE').val();

             // Update hyperlinks to event logs
             $('#hrefEvents').attr("href", "/Admin/Reports/Events/EventsDefault.aspx?type=Events&uid=" + uid + "&cid=" + cid);
             $('#hrefExceptions').attr("href", "/Admin/Reports/Events/EventsDefault.aspx?type=Exceptions&uid=" + uid + "&cid=" + cid);
             $('#hrefUsers').attr("href", "/Admin/Reports/Events/EventsDefault.aspx?type=EndUsers&uid=" + uid + "&cid=" + cid);
             $('#hrefOtpSent').attr("href", "/Admin/Reports/Events/EventsDefault.aspx?type=OtpSent&uid=" + uid + "&cid=" + cid);
             $('#hrefOtpValid').attr("href", "/Admin/Reports/Events/EventsDefault.aspx?type=OtpValid&uid=" + uid + "&cid=" + cid);
             $('#hrefOtpAds').attr("href", "/Admin/Reports/Events/EventsDefault.aspx?type=AdsSent&uid=" + uid + "&cid=" + cid);
             $('#hrefOtpInvalid').attr("href", "/Admin/Reports/Events/EventsDefault.aspx?type=OtpInvalid&uid=" + uid + "&cid=" + cid);

             // Initialize the array
             chartIntervalArray = [];
             for (var i = 0; i < chartTimeInterval; i++) {
                 chartIntervalArray[i] = ['.', 0, 0, 0, 0, 0];
             }

             // Core
             document.getElementById("legendCoreStats").innerHTML = "Core";
             $('#spanGroupCount').html("<span style='color: #0362a6;'>-</span>");
             $('#spanClientCount').html("<span style='color: #0362a6;'>-</span>");
             $('#spanEventCount').html("<span style='color: #0362a6;'>-</span>");
             $('#spanExceptionCount').html("<span style='color: #0362a6;'>-</span>");

             // Users
             document.getElementById("legendUserStats").innerHTML = "Accounts";
             $('#spanSystemAdminCount').html("<span style='color: #0362a6;'>-</span>");
             $('#spanGroupAdminCount').html("<span style='color: #0362a6;'>-</span>");
             $('#spanClientAdminCount').html("<span style='color: #0362a6;'>-</span>");
             $('#spanEndUserCount').html("<span style='color: #0362a6;'>-</span>");

             // OTPs
             document.getElementById("legendOtpStats").innerHTML = "OTP";
             $('#spanOtpSentCount').html("<span style='color: #0362a6;'>-</span>");
             $('#spanOtpValidCount').html("<span style='color: #0362a6;'>-</span>");
             $('#spanOtpInvalidCount').html("<span style='color: #0362a6;'>-</span>");
             $('#spanAdsSentCount').html("<span style='color: #0362a6;'>-</span>");

             // Reset per-second counters
            $('#spanEventCountValue').html("<span style=''>-</span>");
            $('#spanExceptionCountValue').html("<span style=''>-</span>");
            $('#spanEndUserCountValue').html("<span style=''>-</span>");
            $('#spanOtpSentCountValue').html("<span style=''>-</span>");
            $('#spanOtpValidCountValue').html("<span style=''>-</span>");
            $('#spanOtpInvalidCountValue').html("<span style=''>-</span>");
            $('#spanAdsSentCountValue').html("<span style=''>-</span>");

            //startSystemEventTimer();
         }

         function GetSystemStats() {

             var dateRange = "";
             var ownerId = new Date().toDateString();

             if (document.getElementById("dlDateRange") != null) {
                 var selectedDateRange = document.getElementById("dlDateRange");
                 dateRange = selectedDateRange.options[selectedDateRange.selectedIndex].text;
             }

             if (document.getElementById("dlClients") != null) {
                 var selectedClient = document.getElementById("dlClients");
                 ownerId = selectedClient.options[selectedClient.selectedIndex].value;
             }

             var webMethod = "/MACServices/AdminServices/SystemStats.asmx/WsSystemStats";

             var parameters = "dateRange=" + dateRange;
             parameters += "&ownerId=" + ownerId;

             $.post(webMethod, parameters, statsResult);
         }

         function statsResult(response)
         {
             prevEventsCount = parseInt($('#spanEventCount').text().replace(",", "").replace(",", "").replace("-", ""));
             prevErrorsCount = parseInt($('#spanExceptionCount').text().replace(",", "").replace(",", "").replace("-", ""));

             // Previous User Counts
             prevUsersCount = parseInt($('#spanEndUserCount').text().replace(",", "").replace(",", "").replace("-", ""));

             // Previous Otp Counts
             prevOtpSentCount = parseInt($('#spanOtpSentCount').text().replace(",", "").replace(",", "").replace("-", ""));
             prevOtpValidCount = parseInt($('#spanOtpValidCount').text().replace(",", "").replace(",", "").replace("-", ""));
             prevOtpInvalidCount = parseInt($('#spanOtpInvalidCount').text().replace(",", "").replace(",", "").replace("-", ""));
             prevAdsSentCount = parseInt($('#spanAdsSentCount').text().replace(",", "").replace(",", "").replace("-", ""));

             // Process service response
             var serviceResponse = $.parseJSON(response.childNodes[0].textContent);

             // Current Core Counts
             currGroupsCount = serviceResponse.Groups;
             currClientsCount = serviceResponse.Clients;
             currEventsCount = serviceResponse.Events;
             currExceptionsCount = serviceResponse.Exceptions;

             // Current User Counts
             currSysAdminsCount = serviceResponse.SystemAdmins;
             currGroupAdminsCount = serviceResponse.GroupAdmins;
             currClientAdminsCount = serviceResponse.ClientAdmins;
             currUsersCount = serviceResponse.EndUsers;

             // Current Otp Counts
             currOtpSentCount = serviceResponse.OtpSent;
             currOtpValidCount = serviceResponse.OtpValid;
             currOtpInvalidCount = serviceResponse.OtpInvalid;
             currAdsSentCount = serviceResponse.AdsSent;

             currEventsCount = parseInt(currUsersCount) + parseInt(currOtpSentCount) + parseInt(currOtpValidCount) + parseInt(currOtpInvalidCount) + parseInt(currAdsSentCount) + parseInt(currExceptionsCount);

             // Core
             document.getElementById("legendCoreStats").innerHTML = "Core";
             $('#spanGroupCount').html("<span style='color: #0362a6; text-decoration: underline;'>" + numberWithCommas(currGroupsCount) + "</span>");
             $('#spanClientCount').html("<span style='color: #0362a6; text-decoration: underline;'>" + numberWithCommas(currClientsCount) + "</span>");
             $('#spanEventCount').html("<span style='color: #0362a6; text-decoration: underline;'>" + numberWithCommas(currEventsCount) + "</span>");
             $('#spanExceptionCount').html("<span style='color: #0362a6; text-decoration: underline;'>" + numberWithCommas(currExceptionsCount) + "</span>");

             // Users
             document.getElementById("legendUserStats").innerHTML = "Accounts";
             $('#spanSystemAdminCount').html("<span style='color: #0362a6; text-decoration: underline;'>" + numberWithCommas(currSysAdminsCount) + "</span>");
             $('#spanGroupAdminCount').html("<span style='color: #0362a6; text-decoration: underline;'>" + numberWithCommas(currGroupAdminsCount) + "</span>");
             $('#spanClientAdminCount').html("<span style='color: #0362a6; text-decoration: underline;'>" + numberWithCommas(currClientAdminsCount) + "</span>");
             $('#spanEndUserCount').html("<span style='color: #0362a6; text-decoration: underline;'>" + numberWithCommas(currUsersCount) + "</span>");

             // OTPs
             document.getElementById("legendOtpStats").innerHTML = "OTP";
             $('#spanOtpSentCount').html("<span style='color: #0362a6; text-decoration: underline;'>" + numberWithCommas(currOtpSentCount) + "</span>");
             $('#spanOtpValidCount').html("<span style='color: #0362a6; text-decoration: underline;'>" + numberWithCommas(currOtpValidCount) + "</span>");
             $('#spanOtpInvalidCount').html("<span style='color: #0362a6; text-decoration: underline;'>" + numberWithCommas(currOtpInvalidCount) + "</span>");
             $('#spanAdsSentCount').html("<span style='color: #0362a6; text-decoration: underline;'>" + numberWithCommas(currAdsSentCount) + "</span>");

             // Per second counts
             eventsPerSecond = parseInt(currEventsCount) - parseInt(prevEventsCount);
             errorsPerSecond = parseInt(currExceptionsCount) - parseInt(prevErrorsCount);

             usersPerSecond = parseInt(currUsersCount) - parseInt(prevUsersCount);

             otpSentPerSecond = parseInt(currOtpSentCount) - parseInt(prevOtpSentCount);
             otpValidPerSecond = parseInt(currOtpValidCount) - parseInt(prevOtpValidCount);
             otpInvalidPerSecond = parseInt(currOtpInvalidCount) - parseInt(prevOtpInvalidCount);
             adsSentPerSecond = parseInt(currAdsSentCount) - parseInt(prevAdsSentCount);

             //alert("errorsPerSecond - " + errorsPerSecond);

             if (parseInt(eventsPerSecond) > 0)
                 $('#spanEventCountValue').html("<span style=''>" + numberWithCommas(eventsPerSecond) + "</span>");
             else
                 $('#spanEventCountValue').html("<span style=''>0</span>");

             if (parseInt(errorsPerSecond) > 0) {
                 if (errorsPerSecond > 0)
                     $('#spanExceptionCountValue').html("<span style='color: #ff0000;'>" + numberWithCommas(errorsPerSecond) + "</span>");
                 else
                     $('#spanExceptionCountValue').html("<span style=''>" + numberWithCommas(errorsPerSecond) + "</span>");
             }
             else
                 $('#spanExceptionCountValue').html("<span style=''>0</span>");

             if (parseInt(eventsPerSecond) > 0)
                 $('#spanEndUserCountValue').html("<span style=''>" + numberWithCommas(usersPerSecond) + "</span>");
             else
                 $('#spanEndUserCountValue').html("<span style=''>0</span>");

             if (parseInt(usersPerSecond) > 0)
                 $('#spanOtpSentCountValue').html("<span style=''>" + numberWithCommas(otpSentPerSecond) + "</span>");
             else
                 $('#spanOtpSentCountValue').html("<span style=''>0</span>");

             if (parseInt(otpValidPerSecond) > 0)
                 $('#spanOtpValidCountValue').html("<span style=''>" + numberWithCommas(otpValidPerSecond) + "</span>");
             else
                 $('#spanOtpValidCountValue').html("<span style=''>0</span>");

             if (parseInt(otpInvalidPerSecond) > 0)
             {
                 if (otpInvalidPerSecond > 0)
                     $('#spanOtpInvalidCountValue').html("<span style='color: #ff0000;'>" + numberWithCommas(otpInvalidPerSecond) + "</span>");
                 else
                     $('#spanOtpInvalidCountValue').html("<span style=''>" + numberWithCommas(otpInvalidPerSecond) + "</span>");
             }
             else
                 $('#spanOtpInvalidCountValue').html("<span style=''>0</span>");

             if (parseInt(adsSentPerSecond) > 0)
                 $('#spanAdsSentCountValue').html("<span style=''>" + numberWithCommas(adsSentPerSecond) + "</span>");
             else
                 $('#spanAdsSentCountValue').html("<span style=''>0</span>");

             // Update stats array
             updateChartArrayData(false);

             drawChart();

             startSystemEventTimer();
         }

         function drawChart(startTimeChartLabel, endTimeChartLabel)
         {
            var currDate = new Date();
            var currHours = currDate.getHours();
            var currMinutes = currDate.getMinutes();

            var DayTimeIndicator = "am";
            if (currHours > 12) {
                currHours -= 12; // Convert to 12 hour clock
                DayTimeIndicator = "pm";
            }

            var startMinutes = (currDate.getMinutes() + 0);
            if (parseInt(startMinutes) < 10)
                startMinutes = "0" + startMinutes;

            var endMinutes = (currDate.getMinutes() - 1);
            if (parseInt(endMinutes) < 10)
                endMinutes = "0" + endMinutes;

            var startTimeChartLabel = currHours + ":" + startMinutes + DayTimeIndicator;
            var endTimeChartLabel = currHours + ":" + endMinutes + DayTimeIndicator;

             var chartData = [

                  ['Time', 'Users', 'Sent', 'Valid', 'Ads', 'Errors'],
                  [startTimeChartLabel, usersPerSecond, otpSentPerSecond, otpValidPerSecond, adsSentPerSecond, errorsPerSecond],

                  // These are reverse order so they will stream left to right
                  chartIntervalArray[59],
                  chartIntervalArray[58],
                  chartIntervalArray[57],
                  chartIntervalArray[56],
                  chartIntervalArray[55],
                  chartIntervalArray[54],
                  chartIntervalArray[53],
                  chartIntervalArray[52],
                  chartIntervalArray[51],
                  chartIntervalArray[50],
                  chartIntervalArray[49],
                  chartIntervalArray[48],
                  chartIntervalArray[47],
                  chartIntervalArray[46],
                  chartIntervalArray[45],
                  chartIntervalArray[44],
                  chartIntervalArray[43],
                  chartIntervalArray[42],
                  chartIntervalArray[41],
                  chartIntervalArray[40],
                  chartIntervalArray[39],
                  chartIntervalArray[38],
                  chartIntervalArray[37],
                  chartIntervalArray[36],
                  chartIntervalArray[35],
                  chartIntervalArray[34],
                  chartIntervalArray[33],
                  chartIntervalArray[32],
                  chartIntervalArray[31],
                  chartIntervalArray[30],
                  chartIntervalArray[29],
                  chartIntervalArray[28],
                  chartIntervalArray[27],
                  chartIntervalArray[26],
                  chartIntervalArray[25],
                  chartIntervalArray[24],
                  chartIntervalArray[23],
                  chartIntervalArray[22],
                  chartIntervalArray[21],
                  chartIntervalArray[20],
                  chartIntervalArray[19],
                  chartIntervalArray[18],
                  chartIntervalArray[17],
                  chartIntervalArray[16],
                  chartIntervalArray[15],
                  chartIntervalArray[14],
                  chartIntervalArray[13],
                  chartIntervalArray[12],
                  chartIntervalArray[11],
                  chartIntervalArray[10],
                  chartIntervalArray[9],
                  chartIntervalArray[8],
                  chartIntervalArray[7],
                  chartIntervalArray[6],
                  chartIntervalArray[5],
                  chartIntervalArray[4],
                  chartIntervalArray[3],
                  chartIntervalArray[2],
                  chartIntervalArray[1],
                  chartIntervalArray[0] 
             ];

            var data = google.visualization.arrayToDataTable(chartData);

            var options = {
                title: '',
                backgroundColor: '#f8f8f8',
                lineWidth: 0.75,
                //curveType: 'function', // This curves the lines, doesn't look right
                is3D: true,
                 
                fontName: 'Helvetica Neue, Helvetica, Arial, sans-serif',
                fontSize: 12,
                bold: false,
                italic: false,
                color: '#871b47',     // The color of the text.
                auraColor: '#d799ae', // The color of the text outline.
                opacity: 0.8 ,         // The transparency of the text.
                colors: ['#0362a6', '#f26522', '#005e20', '#808080', '#ff0000']

            };

             var chart = new google.visualization.LineChart(document.getElementById('chart_lines'));

             chart.draw(data, options);
         }

         function updateChartArrayData(initChartData)
         {
            if(!initChartData)
            {
                // FIFO  structure
                if (chartIntervalArray.length <= chartTimeInterval) {

                    // Removes the first element of the array,
                    chartIntervalArray.shift();

                    // Add latest stats
                    chartIntervalArray.push([".", usersPerSecond, otpSentPerSecond, otpValidPerSecond, adsSentPerSecond, errorsPerSecond]);
                }
            }
         }

 </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ScriptContainer" Runat="Server">
    <script>
        $(document).ready(function () {

            startSystemEventTimer();

            //On tab click, set hidden value 'panelFocus'
            $('#clientTab1').click(function () {
                $('#panelFocusMyAccount').val('clientTab1');
            });
            $('#clientTab2').click(function () {
                $('#panelFocusMyAccount').val('clientTab2');
            });

            //Expand and position current tab on reload
            var currentTab = $('#panelFocusMyAccount').val();
            if (currentTab == '') {
                $('#clientTab1').click();
                $('html, body').animate({
                    scrollTop: ($('#scroll2').offset().top - 46)
                }, 750, 'easeOutExpo');
            } else if (currentTab == 'clientTab1') {
                $('#' + currentTab + '').click();
                $('html, body').animate({
                    scrollTop: ($('#scroll2').offset().top - 46)
                }, 750, 'easeOutExpo');
            } else if (currentTab == 'clientTab2') {
                $('#' + currentTab + '').click();
                $('html, body').animate({
                    scrollTop: ($('#scroll2').offset().top)
                }, 750, 'easeOutExpo');
            }
        });
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BodyContent" Runat="Server">
    <div style="padding: 0.25rem;"></div>

    <div class="row">
        <div class="large-12 columns">
            <div class="alert-box success radius" id="updateMessage" runat="server" style="cursor: pointer;" onclick="javascript: noDisplay();">
                Update message...
            </div>
        </div>
    </div>

    <div class="row" id="scroll2" runat="server">
        <div class="large-12 columns">
            <dl class="accordion" data-accordion="">

                <dd id="SystemManagementPanel" runat="server">
                    <a id="clientTab2" href="#panel2"><span id="systemManagement" runat="server">System Management</span></a>
                    <div id="panel2" class="content">

                        <div class="row" style="margin-bottom: 0.75rem; position: relative; top: -40px; margin-bottom: -40px;">
                            <div class="large-12 medium-12 small-12 columns" style="width: 100%; text-align: right;">
                                <a href="javascript: NavigateTopicPopup('54fe2090ea6a5700ccbd085d');" id="link_help_systemManagement">Help?</a>
                            </div>
                        </div>

                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns">
                                <select id="systemManagementFunctions" class="chosen-select" runat="server" name="systemManagement" onchange="javascript: systemManagementSelection();">
                                    <option value="pleaseSelect">Please Select</option>
                                    <option value="Change Environment Settings">Change Environment Settings</option>
                                    <option value="Create EventStats">Create Event Stats</option>
                                    <option value="Create Indexes">Create Indexes</option>
                                    <option value="Create Message Template">Create Message Template</option>
                                    <option value="Backup">Database Backup</option>
                                    <option value="Reset">Database Reset</option>
                                    <option value="Restore">Database Restore</option>
                                    <option value="Delete Relationships">Delete Relationships</option>
                                    <option value="Delete Test Data">Delete Test Data</option>
                                    <option value="Reset IPs">Update Allowed IPs</option>
                                    <option value="Reset Ad Providers">Update Ad Providers</option>
                                    <option value="Reset Message Providers">Update Message Providers</option>
                                </select>
                            </div>
                            <div class="large-6 medium-6 small-12 columns">

                            </div>
                        </div>
                    </div>
                </dd>

                <dd>
                    <a id="clientTab1" href="#panel1"><span id="systemOverview" runat="server">System Statistics</span></a>
                    <div id="panel1" class="content">


                        <div class="row" style="margin-bottom: 0.75rem; position: relative; top: -40px; margin-bottom: -40px;">
                            <div class="large-12 medium-12 small-12 columns" style="width: 100%; text-align: right;">
                                <a href="javascript: NavigateTopicPopup('54984e2bead6361ac8883c00');" id="link_help_systemStatistics">Help?</a>
                            </div>
                        </div>

                        <div class="row">

                            <div class="large-6 medium-6 small-12 columns" style="margin-bottom:0.25rem;">
                                <select id="dlDateRange" runat="server" class="chosen-select" onchange="javascript: resetIntervalData();">
                                    <option value="0">All Time</option>
                                    <option value="1">Today</option>
                                    <option value="2">Yesterday</option>
                                    <option value="3">This Week</option>
                                    <option value="4">This Month</option>
                                    <option value="5">This Quarter</option>
                                    <option value="6">This Year</option>
                                </select>
                            </div>

                            <div class="large-6 medium-6 small-12 columns" style="margin-bottom:0.25rem;">
                                <select id="dlClients" runat="server" class="chosen-select" onchange="javascript: resetIntervalData();">
                                    <option value="000000000000000000000000">All Clients</option>
                                </select>
                            </div>
                        </div>

                        <div id="divDateRange" runat="server"><span id="spanStartDate" runat="server" style="font-size: 12px;"></span> <span id="span1" runat="server" style="font-size: 12px;">thru</span> <span id="spanEndDate" runat="server" style="font-size: 12px;"></span></div>

                        <div style="padding: 0.25rem;"></div>

                        <div class="row">
                            <div class="large-4 medium-12 small-12 columns">
                                <fieldset style="padding: 0.5rem 1rem 1rem;margin: 0.25rem 0 0.5rem;"><legend id="legendCoreStats" class="StatLabel" style="font-weight: normal;">Core</legend>
                                    
                                    <div class="row">
                                        <div class="large-8 medium-8 small-8 columns">
                                            <span class="StatLabel" style="color: #b0b0b0;">Items</span>
                                        </div>
                                        <div class="large-4 medium-4 small-4 columns" style="text-align: right;">
                                            <span class="StatLabel" style="color: #b0b0b0;">Per sec</span>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="large-8 medium-8 small-8 columns">
                                            <a id="hrefGroups" href="/Admin/Groups/Default.aspx">
                                                <div>
                                                    <label class="StatLabel">Groups:
                                                        <span id="spanGroupCount" runat="server" class="StatCount"><img src="/Images/progress-bar-round.gif" style="height: 16px; width: 16px;" /></span>
                                                    </label>
                                                </div>
                                            </a>
                                        </div>
                                        <div class="large-4 medium-4 small-4 columns" style="text-align: right;">
                                            <label class="StatLabel" id="spanGroupCountValue" style="color: #b0b0b0;">-</label>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="large-8 medium-8 small-8 columns">
                                            <a id="hrefClients" href="/Admin/Clients/Default.aspx">
                                                <div>
                                                    <label class="StatLabel">Clients:
                                                        <span id="spanClientCount" runat="server" class="StatCount"><img src="/Images/progress-bar-round.gif" style="height: 16px; width: 16px;" /></span>
                                                    </label>
                                                </div>
                                            </a>
                                        </div>
                                        <div class="large-4 medium-4 small-4 columns" style="text-align: right;">
                                            <label class="StatLabel" id="spanClientCountValue" style="color: #b0b0b0;">-</label>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="large-8 medium-8 small-8 columns">
                                            <a id="hrefEvents" href="/Admin/Reports/Events/EventsDefault.aspx?type=Events">
                                                <div>
                                                    <label class="StatLabel">Events:
                                                        <span id="spanEventCount" runat="server" class="StatCount"><img src="/Images/progress-bar-round.gif" style="height: 16px; width: 16px;" /></span>
                                                    </label>
                                                </div>
                                            </a>
                                        </div>
                                        <div class="large-4 medium-4 small-4 columns" style="text-align: right;">
                                            <label class="StatLabel" id="spanEventCountValue">0</label>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="large-8 medium-8 small-8 columns">
                                            <a id="hrefExceptions" href="/Admin/Reports/Events/EventsDefault.aspx?type=Exceptions">
                                                <div>
                                                    <label class="StatLabel">Errors:
                                                        <span id="spanExceptionCount" runat="server" class="StatCount"><img src="/Images/progress-bar-round.gif" style="height: 16px; width: 16px;" /></span>
                                                    </label>
                                                </div>
                                            </a>
                                        </div>
                                        <div class="large-4 medium-4 small-4 columns" style="text-align: right;">
                                            <label class="StatLabel" id="spanExceptionCountValue">0</label>
                                        </div>
                                    </div>

                                </fieldset>
                            </div>

                            <div class="large-4 medium-12 small-12 columns">
                                <fieldset style="padding: 0.5rem 1rem 1rem;margin: 0.25rem 0 0.5rem;"><legend id="legendUserStats" class="StatLabel" style="font-weight: normal;">Accounts</legend>

                                    <div class="row">
                                        <div class="large-8 medium-8 small-8 columns">
                                            <span class="StatLabel" style="color: #b0b0b0;">Items</span>
                                        </div>
                                        <div class="large-4 medium-4 small-4 columns" style="text-align: right;">
                                            <span class="StatLabel" style="color: #b0b0b0;">Per sec</span>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="large-8 medium-8 small-8 columns">
                                            <a id="hrefSystemAdmins" href="/Admin/Users/Default.aspx?AdminType=System">
                                                <div>
                                                    <label class="StatLabel">System Admins:
                                                        <span id="spanSystemAdminCount" runat="server" class="StatCount"><img src="/Images/progress-bar-round.gif" style="height: 16px; width: 16px;" /></span>
                                                    </label>
                                                </div>
                                            </a>
                                        </div>
                                        <div class="large-4 medium-4 small-4 columns" style="text-align: right;">
                                             <label class="StatLabel" id="spanSystemAdminCountValue" style="color: #b0b0b0;">-</label>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="large-8 medium-8 small-8 columns">
                                            <a id="hrefGroupAdmins" href="/Admin/Users/Default.aspx?AdminType=Group">
                                                <div>
                                                    <label class="StatLabel">Group Admins:
                                                        <span id="spanGroupAdminCount" runat="server" class="StatCount"><img src="/Images/progress-bar-round.gif" style="height: 16px; width: 16px;" /></span>
                                                    </label>
                                                </div>
                                            </a>
                                        </div>
                                        <div class="large-4 medium-4 small-4 columns" style="text-align: right;">
                                             <label class="StatLabel" id="spanGroupAdminCountValue" style="color: #b0b0b0;">-</label>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="large-8 medium-8 small-8 columns">
                                            <a id="hrefClientAdmins" href="/Admin/Users/Default.aspx?AdminType=Client">
                                                <div>
                                                    <label class="StatLabel">Client Admins:
                                                        <span id="spanClientAdminCount" runat="server" class="StatCount"><img src="/Images/progress-bar-round.gif" style="height: 16px; width: 16px;" /></span>
                                                    </label>
                                                </div>
                                            </a>
                                        </div>
                                        <div class="large-4 medium-4 small-4 columns" style="text-align: right;">
                                             <label class="StatLabel" id="spanClientAdminCountValue" style="color: #b0b0b0;">-</label>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="large-8 medium-8 small-8 columns">
                                            <a id="hrefUsers" href="/Admin/Reports/Events/EventsDefault.aspx?type=EndUsers">
                                                <div>
                                                    <label class="StatLabel">End Users:
                                                            <span id="spanEndUserCount" runat="server" class="StatCount" style="white-space: nowrap !important;"><img src="../Images/progress-bar-round.gif" style="height: 16px; width: 16px;" /></span>
                                                    </label>
                                                </div>
                                            </a>
                                        </div>
                                        <div class="large-4 medium-4 small-4 columns" style="text-align: right;">
                                             <label class="StatLabel" id="spanEndUserCountValue">0</label>
                                        </div>
                                    </div>
                                    
                                </fieldset>
                            </div>     
                                       
                            <div class="large-4 medium-12 small-12 columns">
                                <fieldset style="padding: 0.5rem 1rem 1rem;margin: 0.25rem 0 0.5rem;"><legend id="legendOtpStats" class="StatLabel" style="font-weight: normal;">OTP</legend>

                                    <div class="row">
                                        <div class="large-8 medium-8 small-8 columns">
                                            <span class="StatLabel" style="color: #b0b0b0;">Items</span>
                                        </div>
                                        <div class="large-4 medium-4 small-4 columns" style="text-align: right;">
                                            <span class="StatLabel" style="color: #b0b0b0;">Per sec</span>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="large-8 medium-8 small-8 columns">
                                            <a id="hrefOtpSent" href="/Admin/Reports/Events/EventsDefault.aspx?type=OtpSent">
                                                <div>
                                                    <label class="StatLabel">Sent:
                                                        <span id="spanOtpSentCount" runat="server" class="StatCount"><img src="/Images/progress-bar-round.gif" style="height: 16px; width: 16px;" /></span>
                                                    </label>
                                                </div>
                                            </a>
                                        </div>
                                        <div class="large-4 medium-4 small-4 columns" style="text-align: right;">
                                             <label class="StatLabel" id="spanOtpSentCountValue">0</label>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="large-8 medium-8 small-8 columns">
                                            <a id="hrefOtpValid" href="/Admin/Reports/Events/EventsDefault.aspx?type=OtpValid">
                                                <div>
                                                    <label class="StatLabel">Valid:
                                                        <span id="spanOtpValidCount" runat="server" class="StatCount"><img src="/Images/progress-bar-round.gif" style="height: 16px; width: 16px;" /></span>
                                                    </label>
                                                </div>
                                            </a>
                                        </div>
                                        <div class="large-4 medium-4 small-4 columns" style="text-align: right;">
                                             <label class="StatLabel" id="spanOtpValidCountValue">0</label>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="large-8 medium-8 small-8 columns">
                                            <a id="hrefOtpInvalid" href="/Admin/Reports/Events/EventsDefault.aspx?type=OtpInvalid">
                                                <div>
                                                    <label class="StatLabel">Invalid:
                                                        <span id="spanOtpInvalidCount" runat="server" class="StatCount"><img src="/Images/progress-bar-round.gif" style="height: 16px; width: 16px;" /></span>
                                                    </label>
                                                </div>
                                            </a>
                                        </div>
                                        <div class="large-4 medium-4 small-4 columns" style="text-align: right;">
                                             <label class="StatLabel" id="spanOtpInvalidCountValue">0</label>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="large-8 medium-8 small-8 columns">
                                            <a id="hrefOtpAds" href="/Admin/Reports/Events/EventsDefault.aspx?type=AdsSent">
                                                <div>
                                                    <label class="StatLabel">Ads:
                                                        <span id="spanAdsSentCount" runat="server" class="StatCount"><img src="/Images/progress-bar-round.gif" style="height: 16px; width: 16px;" /></span>
                                                    </label>
                                                </div>
                                            </a>
                                        </div>
                                        <div class="large-4 medium-4 small-4 columns" style="text-align: right;">
                                             <label class="StatLabel" id="spanAdsSentCountValue">0</label>
                                        </div>
                                    </div>
                                  
                                </fieldset>
                            </div>
                        </div>
                    </div>

                    <div class="row" id="divGoogleChart" style="position: relative; top: 0px;">
                        <div class="large-12 medium-12 small-12 columns" style="margin-bottom:0.25rem; width: 100%; text-align: center; padding-bottom: 0px;">
                            <div id="chart_lines" style="position: relative; top: -5px; width: 100%; height: 250px; border: solid 0px #ff0000; background-color: transparent;"></div>
                        </div>
                    </div>

                </dd>
            </dl>

        </div>
    </div>
    <input id="panelFocusMyAccount" runat="server" type="hidden" value="" />
</asp:Content>

