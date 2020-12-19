<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ConfigPopup.aspx.cs" Inherits="MACAdmin.Clients.Providers.Advertising.ConfigPopup" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Secure Trading Config</title>
    <link href="/App_Themes/CSS/Admin.css" rel="stylesheet" />
    <link href="/App_Themes/CSS/foundation_menu.css" rel="stylesheet" />
    <script src="/Javascript/MACSystemAdmin-Responsive.js"></script>
    <script type="text/javascript">
        function callParentDocumentFunction() {
            window.parent.parent.hideJQueryDialog();
        }

        function updateProvider()
        {
            document.getElementById("hiddenAA").value = "UpdateProvider";
            document.getElementById("form1").submit();
        }

    </script>
</head>
    <body style="overflow:auto !important;">

    <form id="form1" runat="server">

        <div class="row">
            <div style="padding: 0.5rem;"></div>
            <div class="large-12 columns">
                <div class="alert-box success radius" id="updateMessage" runat="server" style="cursor: pointer;" onclick="javascript: noDisplay();">
                    Update message...
                </div>
            </div>
        </div>

        <div class="row">
            <div class="large-6 medium-6 small-12 columns">
                <label>OperatorId
                <asp:TextBox runat="server" ID="txtSecureTradingOperatorId" Width="100" />
                </label>
                <label>SiteId
                <asp:TextBox runat="server" ID="txtSecureTradingSiteId" Width="100" />
                </label>
            </div>
            <div class="large-6 medium-6 small-12 columns">
                &nbsp;
            </div>
        </div>

        <div class="row">
            <div class="large-12 columns">
                <input id="btnSave" type="button" runat="server" class="button tiny radius" value="Save" onclick="javascript: updateProvider();" />
            </div>
        </div>

        <input id="hiddenAA" type="hidden" runat="server" value="" />

    </form>

</body>
</html>
