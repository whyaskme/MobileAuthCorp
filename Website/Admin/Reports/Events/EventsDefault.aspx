<%@ Page Title="" Language="C#" 
    MasterPageFile="~/MasterPages/AdminConsole.master" 
    AutoEventWireup="true" 
    CodeFile="~/Admin/Reports/Events/EventsDefault.aspx.cs" 
    Inherits="Admin.Reports.Events.EventsDefault" %>

<%@ Register Src="~/UserControls/EventTypes.ascx" TagPrefix="uc1" TagName="EventTypes" %>
<%@ Register Src="~/UserControls/Menu-Reports.ascx" TagPrefix="uc1" TagName="MenuReports" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ScriptContainer" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BodyContent" Runat="Server">
    <script type="text/javascript">

        $(document).ready(function () {
            eventHistoryTimerEnabled = true;
            getEventHistory();


            $('#btnDetails').prop('disabled', true);
            $('#btnPause').prop('disabled', false);
            $('#save').prop('disabled', true);

            $('#btnNext').prop('disabled', true);
            $('#btnLast').prop('disabled', true);

            $('#btnEditClient').hide();

            // Check to see if there are parameters in the request. If so, process accordingly
            var currentWindowLocation = window.location.toString();

            if (currentWindowLocation.indexOf('?') > -1)
            {
                //alert("Here 1...");

                var urlElements = currentWindowLocation.split('?');
                var urlItems = "";

                if (urlElements[1].indexOf('&') > -1)
                {
                    urlItems = urlElements[1].split('&');
                    for (var i = 0; i < urlItems.length; i++)
                    {
                        if (urlItems[i].indexOf('=') > -1)
                        {
                            var urlParams = urlItems[i].split('=');

                            var urlParamName = urlParams[0];
                            var urlParamValue = urlParams[1];

                            switch(urlParamName.toLowerCase())
                            {
                                case "cid":
                                    $('#btnEditClient').hide();
                                    break;

                                case "type":
                                    toggleSearchDisplay();
                                    switch (urlParamValue.toLowerCase())
                                    {
                                        case "endusers":
                                            //alert("Here...");
                                            //$('#dlEventTypes').select(3);
                                            //$('#dlEventTypes>option:eq(3)').prop('selected', true);
                                            //document.getElementById("dlEventTypes").selectedIndex = 5;

                                            //$('#dlEventTypes').prop('selectedIndex', 3);

                                            //var el = document.getElementById('dlEventTypes')[0];
                                            //alert(el.selectedIndex);

                                            //el.selectedIndex = 6;

                                            //alert(el.selectedIndex);

                                            //$('#dlEventTypes_chosen').val("EndUsers");

                                            //// Update the eventype list display
                                            //$('#dlEventTypes').trigger('chosen:updated');
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    //alert("Here - " + urlElements.length);

                    for (var i = 0; i < urlElements.length; i++)
                    {
                        if (urlElements[i].indexOf('=') > -1)
                        {
                            //alert("Here - " + urlElements.length);

                            var urlParams = urlElements[i].split('=');

                            var urlParamName = urlParams[0];
                            var urlParamValue = urlParams[1];

                            switch (urlParamName.toLowerCase()) {
                                case "cid":
                                    $('#btnEditClient').hide();
                                    break;

                                case "type":
                                    toggleSearchDisplay();
                                    switch (urlParamValue.toLowerCase())
                                    {
                                        case "exceptions":

                                            //alert("Here 123");

                                            //$('#dlEventTypes').select(20);
                                            //$('#dlEventTypes>option:eq(20)').prop('selected', true);

                                            $("#dlEventTypes option[id=Failures (General):]").attr("selected", true);

                                            //$('#dlEventTypes_chosen').val("Failures (General):");

                                            // Update the eventype list display
                                            $('#dlEventTypes').trigger('chosen:updated');
                                            break;

                                        default:

                                            break;
                                    }
                                    break;
                            }
                        }
                    }
                }
            }

            //if (currentWindowLocation.indexOf("cid=") > -1)
            //    $('#btnEditClient').hide();
            //else
            //    $('#btnEditClient').hide();

        });
        $(function () {
            $('#popupDatepickerStartDate').datepicker();
            $('#popupDatepickerEndDate').datepicker();
        });

        function navigateClient()
        {
            var cid = $('#dlClients').val();
            var uid = $('#hiddenE').val();

            var url = "/Admin/Clients/Default.aspx?uid=" + uid + "&cid=" + cid;
            //alert("url - " + url);
            window.location = url;
        }
    </script>

    <div style="padding: 0.25rem;"></div>

    <div class="row">
        <div class="large-3 medium-3 small-12 columns hide-for-small">
            &nbsp;
        </div>
        <div class="large-6 medium-6 small-12 columns">
            <uc1:MenuReports runat="server" ID="MenuReports" />
        </div>
        <div class="large-3 medium-3 small-12 columns hide-for-small">
            &nbsp;
        </div>
    </div>

    <div style="padding: 0.25rem;"></div>
    <div class="row" id="scroll2" style="border-top: solid 1px #c0c0c0; position: relative; top: 25px; padding-top: 25px; margin-bottom: 25px;">
        <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.25rem;">
            <select id="dlClients" class="chosen-select" runat="server" onchange="javascript: switchClient();">
                <option value="000000000000000000000000">Select a Client</option>
            </select>
        </div>
        <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.50rem;">
                <input class="button tiny radius" style="width: 80px;" id="btnEditClient" runat="server" type="button" value="Edit" onclick="javascript: navigateClient();" />  
        </div>
    </div>
    <div style="padding: 0.5rem;"></div>
    <div class="row">
        <div class="large-12 columns">
            <span><a href="#" id="displaySearchSettings" onclick="javascript: toggleSearchDisplay();">[+] Show Settings</a></span>
        </div>
    </div>
    <div style="padding: 0.25rem;"></div>
    <div class="row">
        <div class="large-12 columns">
            <div id="searchSettingsDisplay" style="display: none;">
                <fieldset id="myFieldset">
                    <div class="row">
                        <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.25rem;">
                            <uc1:EventTypes runat="server" ID="EventTypes" />
                        </div>
                        <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.25rem;">
                            <select id="dlRecordsPerPage" name="mySelect" class="chosen-select">
                                <option value="5">5 Records Per Page</option>
                                <option value="10">10 Records Per Page</option>
                                <option selected value="25">25 Records Per Page</option>
                                <option value="50">50 Records Per Page</option>
                                <option value="100">100 Records Per Page</option>
                                <option value="250">250 Records Per Page</option>
                                <option value="500">500 Records Per Page</option>
                                <option value="750">750 Records Per Page</option>
                                <option value="1000">1000 Records Per Page</option>
                            </select>
                        </div>
                    </div>
                    <div class="row">                    
                        <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.25rem;">
                            <asp:DropDownList CssClass="chosen-select" ID="dlSortField" runat="server" AutoPostBack="False">
                                <asp:ListItem>Sort By</asp:ListItem>
                                <asp:ListItem>Date</asp:ListItem>
                                <asp:ListItem>Details</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.25rem;">
                            <asp:DropDownList CssClass="chosen-select" ID="dlSortOrder" runat="server" AutoPostBack="False">
                                <asp:ListItem Value="Ascending">Ascending</asp:ListItem>
                                <asp:ListItem Value="Descending" Selected="True">Descending</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div style="padding: 0.5rem;"></div>
                    <div class="row">                    
                        <div class="large-6 medium-6 small-12 columns">
                            <label>Start Date
                                <input type="text" id="popupDatepickerStartDate" runat="server" />
                            </label>
                        </div>
                        <div class="large-6 medium-6 small-12 columns">
                            <label>End Date
                                <input type="text" id="popupDatepickerEndDate" runat="server" />
                            </label>
                        </div>
                    </div>
                </fieldset>
            </div>
        </div>
    </div>
    <div style="padding: 0.25rem;"></div>
    <div class="row">
        <div class="large-6 medium-6 small-12 columns">
            <div class="hide-for-small" style="display: block; height: 26px;"></div>

            <div class="title_875rem" id="refreshMessage" style="color: #222;display:block;height:16px;float:left;">Loading...</div> <img src="/Images/progress-bar-round.gif" id="refreshIndicator" style="height: 16px; width: 16px;display:none;" />

            <div class="show-for-small" style="display: block; height: 0.75rem;"></div>
        </div>
        <div class="large-6 medium-6 small-12 columns" style="text-align: right;">
            <input class="button tiny radius" runat="server" disabled style="width: 80px;margin:0 0 0.75rem;" id="btnPause" type="button" value="Pause" onclick="javascript: pauseRefresh(false);" />
            <input class="button tiny radius" runat="server" disabled style="width: 80px;margin:0 0 0.75rem;" id="btnDetails" type="button" value="Details" onclick="javascript: showEventDetailsPopup(false);" />
            <input class="button tiny radius" runat="server" disabled style="width: 80px;margin:0 0 0.75rem;" id="save" type="button" value="Export" onclick="javascript: exportEvents();" />
        </div>
    </div>
    <div class="row">
        <div class="large-12 columns">
            <table style='width: 100%;border-collapse: collapse;margin-bottom:0;border:1px solid #ccc;'><thead><tr><th>Row</th><th>Date</th><th>Details</th></tr></thead>
                <tbody id='eventHistoryTBody'>
                </tbody>
            </table>

        </div>
    </div>
    <div class="row">
        <div class="large-6 medium-6 small-12 columns">
            <div style="font-size:0.875rem;color:#4d4d4d;margin-top: 0.25rem;">
                Page
                <span id="CurrentPageNumber">0</span>
                of
                <span id="TotalPageNumbers">0</span>
            </div>
        </div>
        <div class="large-6 medium-6 small-12 columns">
            <div style="margin-top: 0.5rem;text-align: right;">
                <input class="button tiny radius" style="width: 80px;" disabled id="btnFirst" type="button" value="First" onclick="javascript: navigatePages('first');" />
                <input class="button tiny radius" style="width: 80px;" disabled id="btnPrevious" type="button" value="Previous" onclick="javascript: navigatePages('previous');" />
                <input class="button tiny radius" style="width: 80px;" disabled id="btnNext" type="button" value="Next" onclick="javascript: navigatePages('next');" />
                <input class="button tiny radius" style="width: 80px;" disabled id="btnLast" type="button" value="Last" onclick="javascript: navigatePages('last');" />         
            </div>
        </div>
        <input id="hiddenRefreshTimerPaused" type="hidden" value="false" />
        <input id="hiddenRefreshEventTypeList" type="hidden" value="true" />
        <input id="hiddenSelectedEventIds" type="hidden" />
    </div>
    
</asp:Content>