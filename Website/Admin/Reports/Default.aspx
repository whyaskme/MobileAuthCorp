<%@ Page Title="" Language="C#" 
    MasterPageFile="~/MasterPages/AdminConsole.master" 
    AutoEventWireup="true" 
    CodeFile="Default.aspx.cs" 
    Inherits="Admin.Reports.Default" %>

<%--<%@ Register Src="~/UserControls/EventTypes.ascx" TagPrefix="uc1" TagName="EventTypes" %>--%>
<%@ Register Src="~/UserControls/Menu-Reports.ascx" TagPrefix="uc1" TagName="MenuReports" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ScriptContainer" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BodyContent" Runat="Server">
    <script type="text/javascript">

        $(document).ready(function () {
            //eventHistoryTimerEnabled = true;
            //getEventHistory();
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
                var urlElements = currentWindowLocation.split('?');
                var urlItems = "";

                if (urlElements[1].indexOf('&') > -1) {

                    urlItems = urlElements[1].split('&');

                    for (var i = 0; i < urlItems.length; i++) {
                        if (urlItems[i].indexOf('=') > -1) {
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

                            //alert("urlParamName - " + urlParamName + ", urlParamValue - " + urlParamValue);
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

</asp:Content>