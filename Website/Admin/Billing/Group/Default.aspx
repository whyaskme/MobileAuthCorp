<%@ Page Title="MAC Otp System Administration"
    Language="C#"
    ValidateRequest="false"
    MasterPageFile="~/MasterPages/AdminConsole.master"
    AutoEventWireup="true"
    CodeFile="Default.aspx.cs"
    Inherits="Default" %>

<%@ Register Src="~/UserControls/BillingRender.ascx" TagPrefix="uc1" TagName="BillingRender" %>


<asp:Content ID="BodyContainer" ContentPlaceHolderID="BodyContent" Runat="Server">

    <link href="../../../App_Themes/CSS/Admin.css" rel="stylesheet" />
    <link href="../../../App_Themes/CSS/billing-table-style.css" rel="stylesheet" />

    <script src="/Javascript/jquery-1.10.2.js"></script>
    <script src="/Javascript/jquery-migrate-1.0.0.js"></script>
    <script src="/Javascript/jquery.printElement.js"></script>

    <script>
            var userIsReadOnly = false;

            var mainForm = document.getElementById("formMain");

            $(document).ready(function () {

                userIsReadOnly = document.getElementById("hiddenF").value.toString().toLowerCase();
                //alert("userIsReadOnly - " + userIsReadOnly);
            });

            function RemoveStyleSheets() {
                $('link[rel=stylesheet]').remove();

                // Attach the css we need
                $('head').append('<link rel="stylesheet" href="/App_Themes/CSS/Admin.css" type="text/css" />');
                $('head').append('<link rel="stylesheet" href="/App_Themes/CSS/print.css" type="text/css" />');
            }

            function PrintGroupBill() {
                var groupName = document.getElementById("txtClientName").innerHTML;

                try {
                    $('#panelBilling').printElement
                        (
                            {
                                pageTitle: 'MAC Billing for Client ' + groupName,
                                printMode: 'iframe', //'iframe', 'popup'
                                overrideElementCSS: ['/App_Themes/CSS/Admin.css', { href: '/App_Themes/CSS/Admin.css', media: 'print' }, '/App_Themes/CSS/print.css', { href: '/App_Themes/CSS/print.css', media: 'print' }],

                                iframeElementOptions:
                                {
                                    styleToAdd: 'position:absolute;width:0px;height:0px;bottom:0px;'
                                },

                                printBodyOptions:
                                {
                                    styleToAdd: 'padding:0px;margin:0px;color:#FFFFFF !important;'
                                }
                            }
                        );
                }
                catch (err) {
                    alert(err.message);
                }
            }

            function setActiveGroup(selectedGroupId) {
                $('#hiddenY').val(selectedGroupId);
                $('#formMain').submit();
            }

            function NavigateBillType(billType) {
                var mainForm = document.getElementById("formMain");

                if (billType == 0) {
                    ShowProcessingMessage();
                    mainForm.action = "/Admin/Billing/Client/Default.aspx";
                    mainForm.submit();
                }
            }

            function GroupBillConfig() {

                var loggedInAdminId = document.getElementById("hiddenE").value;

                var selectedClient = $('#dlGroups')[0];
                var clientId = selectedClient.options[selectedClient.selectedIndex].value;

                var configType = $('#dlBillingCategory').val();

                var targetUrl = "/Admin/Billing/ConfigPopup.aspx?userisreadonly=" + userIsReadOnly + "&loggedInAdminId=" + loggedInAdminId + "&ownerId=" + clientId + "&configType=Group";
                showJQueryDialog(targetUrl, popupWindowWidth, popupWindowHeight);
            }

            function GroupBillAddendum() {

                var loggedInAdminId = document.getElementById("hiddenE").value;

                var selectedGroup = $('#dlGroups')[0];
                var groupId = selectedGroup.options[selectedGroup.selectedIndex].value;
                var billId = document.getElementById("txtBillNumber").innerHTML;

                var popupWindowWidth = 575;
                var popupWindowHeight = 375;

                var configType = $('#dlBillingCategory').val();

                var targetUrl = "/Admin/Billing/AddendumPopup.aspx?userisreadonly=" + userIsReadOnly + "&loggedInAdminId=" + loggedInAdminId + "&clientId=" + groupId + "&billId=" + billId + "&configType=" + configType;
                showJQueryDialog(targetUrl, popupWindowWidth, popupWindowHeight);

            }

            function SaveGroupBill() {
                $('#hiddenAA').val("SaveBill");
                $('#formMain').submit();
            }

            function ShowGroupBillHistory() {
                var loggedInAdminId = document.getElementById("hiddenE").value;

                var selectedGroup = $('#dlGroups')[0];
                var groupId = selectedGroup.options[selectedGroup.selectedIndex].value;

                var targetUrl = "/Admin/Billing/HistoryPopup.aspx?loggedInAdminId=" + loggedInAdminId + "&ownerId=" + groupId + "&configType=Group";
                showJQueryDialog(targetUrl, popupWindowWidth, popupWindowHeight);

                return false;
            }

            function EmailGroupBill() {

                var loggedInAdminId = document.getElementById("hiddenE").value;

                var selectedGroup = $('#dlGroups')[0];
                var groupId = selectedGroup.options[selectedGroup.selectedIndex].value;
                var billId = document.getElementById("txtBillNumber").innerHTML;

                var configType = $('#dlBillingCategory').val();

                var targetUrl = "/Admin/Billing/SendToPopup.aspx?loggedInAdminId=" + loggedInAdminId + "&clientId=" + groupId + "&billId=" + billId + "&configType=Group";
                showJQueryDialog(targetUrl, popupWindowWidth, popupWindowHeight);
            }

            function PayGroupBill() {
                alert("billPay");
            }

            function VoidGroupBill() {
                alert("billVoid");
            }

            function ResetGroupBill() {
                $('#formMain').submit();
            }
    </script>

    <div class="row">
        <div class="large-4 medium-4 small-12 columns" style="padding-bottom: 0.25rem;">
            <select id="dlBillingCategory" class="chosen-select" runat="server" name="dlBillingCategory" onchange="javascript: NavigateBillType(this.selectedIndex);">
                <option value="Client">Client</option>
                <option value="Group" selected="selected">Group</option>
            </select>
        </div>
        <div class="large-4 medium-4 small-12 columns" style="padding-bottom: 0.25rem;">
            <asp:DropDownList ID="dlDateRange" runat="server" AutoPostBack="True" Enabled="false" CssClass="chosen-select">
                <asp:ListItem Value="0">All Time</asp:ListItem>
                <asp:ListItem Value="1" Selected="True">Current (Pending) Charges</asp:ListItem>
                <asp:ListItem Value="2">Today</asp:ListItem>
                <asp:ListItem Value="3">Yesterday</asp:ListItem>
                <asp:ListItem Value="4">This Week</asp:ListItem>
                <asp:ListItem Value="5">This Month</asp:ListItem>
                <asp:ListItem Value="6">This Quarter</asp:ListItem>
                <asp:ListItem Value="7">This Year</asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="large-4 medium-4 small-12 columns" style="padding-bottom: 0.25rem;">
            &nbsp;
        </div>
    </div>

    <div style="padding: 0.5rem;"></div>

    <div class="row">
        <div class="large-12 medium-12 small-12 columns" style="padding-bottom: 0.25rem;">
            <div id="divGroupCount" runat="server">0 Groups</div>
            <select id="dlGroups" multiple="true" runat="server" style="width: 100%; height: 150px;"></select>
        </div>
    </div>

    <div id="divBillDocumentContainer" runat="server">

        <!-- Billing Actions -->
        <div class="row">
            <div class="large-4 medium-4 small-12 columns" style="padding-bottom: 0.25rem;">
                <asp:Button ID="btnBillHistory" runat="server" Text="History" Enabled="true" OnClientClick="javascript: ShowGroupBillHistory(); return false;" CssClass="button tiny radius" />
                <asp:Button ID="btnBillSend" runat="server" Text="Email" Enabled="true" OnClientClick="javascript: EmailGroupBill(); return false;" CssClass="button tiny radius" />
                <asp:Button ID="btnBillPrint" runat="server" Text="Print" Enabled="true" OnClientClick="javascript: PrintGroupBill(); return false;" CssClass="button tiny radius" />
            </div>
            <div class="large-4 medium-4 small-12 columns" style="padding-bottom: 0.25rem;">
                <asp:Button ID="btnBillConfig" runat="server" Text="Config" Enabled="false" OnClientClick="javascript: GroupBillConfig(); return false;" CssClass="button tiny radius" />
                <asp:Button ID="btnBillAddAddendum" runat="server" Text="Misc" Enabled="true" OnClientClick="javascript: GroupBillAddendum(); return false;" CssClass="button tiny radius" />
                <asp:Button ID="btnBillSave" runat="server" Text="Save" Enabled="false" OnClientClick="javascript: SaveGroupBill(); return false;" CssClass="button tiny radius" />
            </div>
            <div class="large-4 medium-4 small-12 columns" style="padding-bottom: 0.25rem;">
                <asp:Button ID="btnBillPay" runat="server" Text="Pay" Enabled="false" OnClientClick="javascript: PayGroupBill(); return false;" CssClass="button tiny radius" />
                <asp:Button ID="btnBillVoid" runat="server" Text="Void" Enabled="false" OnClientClick="javascript: VoidGroupBill(); return false;" CssClass="button tiny radius" />
                <asp:Button ID="btnBillBack" runat="server" Text="Back" Enabled="false" OnClientClick="javascript: ResetGroupBill(); return false;" CssClass="button tiny radius" />
            </div>
        </div>
        <!-- Billing Actions -->


        <div class="row" id="divUpdateMsg" runat="server" style="margin-bottom: -25px;">
            <div class="large-12 columns">
                <div class="alert-box success radius" id="updateMessage" runat="server" style="cursor: pointer;" onclick="javascript: noDisplay();">
                    Update message...
                </div>
            </div>
        </div>

        <div class="row">

            <div class="large-12 medium-12 small-12 columns">
                <asp:Panel ID="panelBilling" runat="server"></asp:Panel>
            </div>

        </div>

    </div>

    <div style="padding: 0.75rem;"></div>

    <input id="hiddenAA" runat="server" type="hidden" value="" />
    <input id="hiddenSerializedBill" runat="server" type="hidden" value="" />
    <input id="hiddenY" runat="server" type="hidden" value="" />

</asp:Content>
