<%@ Page Title="" Language="C#" 
    MasterPageFile="~/MasterPages/AdminConsole.master" 
    AutoEventWireup="true" 
    CodeFile="~/Admin/Reports/Operations/OpsDefault.aspx.cs" 
    Inherits="Admin.Reports.Operations.OpsDefault" %>

<%@ Register Src="~/UserControls/Menu-Reports.ascx" TagPrefix="uc1" TagName="MenuReports" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server" />
<asp:Content ID="Content2" ContentPlaceHolderID="ScriptContainer" Runat="Server" />
<asp:Content ID="Content3" ContentPlaceHolderID="BodyContent" Runat="Server" >
    <script type="text/javascript">

        $(document).ready(function() {
            $().ready(function () {

            });
        });
        function FileSelected() {
            var listToValidate = $("#dlResultsTabFiles")[0];
            $("#hiddenResultsFileName").val(listToValidate.options[listToValidate.selectedIndex].text);
            $("#hiddenResultsTestId").val(listToValidate.options[listToValidate.selectedIndex].value);
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
    <div style="padding: 0.5rem 0 1.25rem;" id="scroll2"></div>
    <div class="row">
        <div class="large-4 medium-4 small-12 columns" style="margin-bottom: 1rem;">
            <label>Systems Under Test
                <asp:DropDownList runat="server" id="dlResultsTabSystemsUnderTest"  />
            </label>
        </div>                         
        <div class="large-3 medium-3 small-12 columns" style="margin-bottom: 1rem;">
            <label>Start Date
                <input type="text" id="txtResultsStartDate" runat="server" style="width:200px;" />
            </label>
        </div>                            
        <div class="large-3 medium-3 small-12 columns" style="margin-bottom: 1rem;">
            <label>End Date
                <input type="text" id="txtResultsEndDate" runat="server" style="width:200px;" />
            </label>
        </div>
        <div class="large-2 medium-2 small-12 columns">
            <label>Retrive Test Runs</label>
            <asp:Button runat="server" ID="btnRetrieTestRuns" Text="Go" OnClick="btnRetrieveTestRuns_Click"/>
        </div>
    </div>
    <div class="row">
        <div class="large-8 medium-8 small-12 columns"> 
            <label>Test Runs
                <asp:DropDownList runat="server" id="dlResultsTabTestRuns"
                    AutoPostBack="True" OnSelectedIndexChanged="dlResultsTabTestRuns_Changed" />
            </label>
        </div>
        <div class="large-4 medium-4 small-12 columns" style="margin-bottom: 1rem;">
            <label>&nbsp;</label>
            <asp:Label Id="lbErrorTestResults" runat="server" ForeColor="Red" />
        </div>
    </div>
    <div class="row">
        <div class="large-3 medium-3 small-12 columns"> 
            <label>Test Name
                <input type="text" id="txtResultsTestName" runat="server" />
            </label>
        </div>
        <div class="large-3 medium-3 small-12 columns"> 
            <label>Test Script
                <input type="text" id="txtResultsTestScript" runat="server" />
            </label>
        </div>
        <div class="large-3 medium-3 small-12 columns"> 
            <label>Overall Results
                <input type="text" id="txtResultsOverall" runat="server" />
            </label>
        </div>                            
        <div class="large-3 medium-3 small-12 columns">
            <label>&nbsp;</label>
            <asp:CheckBox runat="server" ID="cbResultsEmailSent" Text="Message Sent to admin(s)"/>
        </div>
    </div>
    <div class="row">
        <div class="large-12 medium-12 small-12 columns" runat="server" id="divFiles">
            &nbsp;
        </div>
    </div>
    <div class="row">
        <div class="large-1 medium-1 small-12 columns">
            <asp:Label runat="server" ID="lbViewFileError" ForeColor="Red" />
        </div>
    </div>
    
    

    <input id="hiddenResultsFileName" runat="server" type="hidden" value="" />
    <input id="hiddenResultsTestId" runat="server" type="hidden" value="" />
    <input id="hiddenFileList" runat="server" type="hidden" value="" />
    <input id="panelFocusUsers" runat="server" type="hidden" value="" />
</asp:Content>