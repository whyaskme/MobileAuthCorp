<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Menu-Reports.ascx.cs" Inherits="UserControls_Menu_Test" %>

<script>
    $(document).ready(function () {

        var testList = document.getElementById("reportList");

        $(".chosen-select").chosen({ disable_search_threshold: 100 });

        // Set selected listitem by quierystring if present
        if (window.location.toString().indexOf("?i=") > -1)
        {
            var tmpVal = window.location.toString().split('=');
            var selectedIndex = parseInt(tmpVal[1]);
            testList.selectedIndex = selectedIndex;
            $("#reportList").trigger("chosen:updated");
        }

        $(function pageRedirect() {
            //ShowProcessingMessage();
            // bind change event to select
            $('#reportList').on('change', function () {
                var url = $(this).val(); // get selected value

                //alert();

                if (url != "-1") {
                    var myForm = document.getElementById("formMain");
                    myForm.action = url; // redirect
                    myForm.submit();
                }

                return false;
            });
        });

    });
</script>

<select name="reportSelect" id="reportList" class="chosen-select" onchange="javascript: pageRedirect();">
    <option id="Select" value="/Admin/Reports/Default.aspx">Select a Report</option>     
    <option id="ReportEvents" value="/Admin/Reports/Events/EventsDefault.aspx?i=1">System Events</option>
    <option id="ReportOperationalTest" value="/Admin/Reports/Operations/OpsDefault.aspx?i=2">Operational Test Results</option>
</select>