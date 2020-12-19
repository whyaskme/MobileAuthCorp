<%@ Page Language="C#" AutoEventWireup="true" CodeFile="HistoryPopup.aspx.cs" Inherits="MACBilling.HistoryPopup" ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Email Provider Config</title>

    <link href="/App_Themes/CSS/foundation_menu.css" rel="stylesheet" />
    <link href="../../App_Themes/CSS/Billing.css" rel="stylesheet" />

    <script src="/Javascript/MACSystemAdmin-Responsive.js"></script>
    <script src="/Javascript/jquery-1.10.2.js"></script>
    <script type="text/javascript">
        function callParentDocumentFunction()
        {
            window.parent.parent.hideJQueryDialog();

            var parentForm = window.parent.parent.document.getElementById("formMain");
            parentForm.submit();
        }

        function viewBillDetails(billId)
        {
            //alert("billId - " + billId);

            window.parent.parent.hideJQueryDialog();

            var parentAction = window.parent.parent.document.getElementById("hiddenAA");
            parentAction.value = "ViewBillDetails";

            var parentBillId = window.parent.parent.document.getElementById("hiddenV");
            parentBillId.value = billId;

            var parentForm = window.parent.parent.document.getElementById("formMain");
            parentForm.submit();
        }

        function cancelChanges()
        {
            window.parent.parent.hideJQueryDialog();
        }
    </script>
</head>
    <body style="overflow:auto !important;">

    <form id="form1" runat="server">

        <div class="row">
            <div class="large-12 columns">
                <h3 style="font-size:1rem;font-weight:bold;margin-top:0.5rem;" id="spanHistory" runat="server">Billing History</h3>
            </div>
        </div>

        <div class="row">
            <div class="small-12 columns">
                <hr style="margin-top:0;margin-bottom:1rem;" />
            </div>
        </div>

        <div style="padding:.25rem;"></div>

        <div id="panel3" class="content">

            <div id="divBillingHistory" runat="server" class="row" style="padding-left: 15px; padding-right: 15px;"></div>

            <div class="row" style="width: 100%; text-align: center; padding: 15px;">
                <input id="btnCancel" type="button" onclick="javascript: cancelChanges();" value="Close" class="button tiny radius" />          
            </div>

        </div>     

    </form>
</body>
</html>
