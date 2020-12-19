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

        var mainForm = document.getElementById("formMain");

        $(document).ready(function () {


        });

        function NavigateBillType(billType) {
            var mainForm = document.getElementById("formMain");

            switch (billType)
            {
                case 1:
                    ShowProcessingMessage();
                    mainForm.action = "/Admin/Billing/Client/Default.aspx";
                    mainForm.submit();
                    break;

                case 2:
                    ShowProcessingMessage();
                    mainForm.action = "/Admin/Billing/Group/Default.aspx";
                    mainForm.submit();
                    break;
            }
        }

    </script>

    <div class="row">
        <div class="large-4 medium-4 small-12 columns" style="padding-bottom: 0.25rem;">
            &nbsp;
        </div>
        <div class="large-4 medium-4 small-12 columns" style="padding-bottom: 0.25rem;">
            <select id="dlBillingCategory" class="chosen-select" runat="server" name="dlBillingCategory" onchange="javascript: NavigateBillType(this.selectedIndex);">
                <option value="Select" selected="selected">Select a Bill Type</option>
                <option value="Client">Client</option>
                <option value="Group">Group</option>
            </select>
        </div>
        <div class="large-4 medium-4 small-12 columns" style="padding-bottom: 0.25rem;">
            &nbsp;
        </div>
    </div>

    <div style="padding: 0.75rem;"></div>

    <input id="hiddenAA" runat="server" type="hidden" value="" />
    <input id="hiddenSerializedBill" runat="server" type="hidden" value="" />
    <input id="hiddenY" runat="server" type="hidden" value="" />

</asp:Content>
