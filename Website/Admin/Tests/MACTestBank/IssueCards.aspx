
<%@ Page Title="MAC Test Bank"
    Language="C#"
    ValidateRequest="false"
    MasterPageFile="~/MasterPages/AdminConsole.master" 
    AutoEventWireup="true" 
    CodeFile="IssueCards.aspx.cs" 
    Inherits="Admin_Tests_MACTestBank_IssueCards" %>

<%@ Register Src="~/UserControls/Menu-Test.ascx" TagPrefix="uc1" TagName="MenuTest" %>


<asp:Content ID="BodyContainer" ContentPlaceHolderID="BodyContent" Runat="Server">
    <script src="TestBank.js"></script>

    <div class="row">
        <div class="large-3 medium-3 small-12 columns hide-for-small">
            &nbsp;
        </div>
        <div class="large-6 medium-6 small-12 columns">
            <uc1:MenuTest runat="server" ID="MenuTest" />
        </div>
        <div class="large-3 medium-3 small-12 columns hide-for-small">
            &nbsp;
        </div>
    </div>

    <div style="padding: 0.75rem;"></div>
    <div class="row">
        <div class="large-12 medium-12 small-12 columns">
            <h3>Issue Cards</h3>
        </div>
    </div>

    <div class="row">
        <div class="large-12 columns">
            <fieldset><legend></legend>
                <div class="row">
                    <div class="large-4 medium-4 small-12 columns">
                        <asp:DropDownList CssClass="chosen-select" ID="ddlAccountHolderList" runat="server" />
                    </div>
                </div>
                <div style="padding: 0.875rem;"></div>
                <div class="row">
                    <div class="large-6 medium-6 small-12 columns">
                        <label>Card Name
                            <asp:TextBox ID="txtCardName" runat="server" />
                        </label>
                    </div>
                    <div class="large-6 medium-6 small-12 columns">
                        <label>Card Type
                            <asp:TextBox ID="txtCardType" runat="server" />
                        </label>
                    </div>
                </div>
                <div class="row">
                    <div class="large-6 medium-6 small-12 columns">
                        <label>Limit
                            <asp:TextBox ID="txtLimit" runat="server" />
                        </label>
                    </div>
                    <div class="large-6 medium-6 small-12 columns">
                        <label>Usage
                            <asp:TextBox ID="txtUsage" runat="server" />
                        </label>
                    </div>
                </div>
               
                <div class="row">
                    <div class="large-12 columns">
                        <asp:Button CssClass="button tiny radius" runat="server" ID="btnIssueCard" Text="Issue Card" OnClick="btnAssignUser_Click" />
                    </div>
                </div>
            </fieldset>
        </div>
    </div>

    <div style="padding: 0.5rem;"></div>
    <div class="row">
        <div class="large-12 columns">
            <asp:Button CssClass="button tiny radius" runat="server" ID="btnClearLog" Text="Clear Logs" OnClick="btnClearLog_Click"/>
        </div>
    </div>
    <div class="row">
        <div class="large-12 columns">
            <asp:Label ID="lbError" runat="server" ForeColor="Red" />
        </div>
    </div>
    <div class="row">
        <div class="large-12 columns">
            <asp:TextBox id="tbLog" runat="server" TextMode="MultiLine" Height="300" />
        </div>
    </div>
    <div id="divHiddenFields" style="visibility: hidden;">
        <input type="hidden" id="hiddenPANList" value="" runat="server" />
        <input type="hidden" id="hiddenAccountHoldersList" value="" runat="server" />
        <input type="hidden" id="hiddenAccountNamesList" value="" runat="server" />
        <input type="hidden" id="hiddenLoginNamesList" value=""  runat="server" />
        <input type="hidden" id="hiddenBills" value="" runat="server" />
    </div>
    <script>
        $(document).ready(function () {
            var defaultMessage1 = $('#ddlTestUserFiles1_chosen a span').html();
            if (defaultMessage1 == 'Select File') {
                $('#ddlTestUserFiles1_chosen a span').html('Select a File To Assign');
            }
            var defaultMessage2 = $('#ddlEndUserList2_chosen a span').html();
            if (defaultMessage2 == 'Select an Option') {
                $('#ddlEndUserList2_chosen a span').html('Select User');
            }
            $('#menuTests').addClass('active');
        });
    </script>
</asp:Content>
