<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GroupClientsPopup.aspx.cs" Inherits="MACAdmin.Clients.GroupAssignmentPopup" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Clients Assigned to Group</title>
    <link href="/App_Themes/CSS/Admin.css" rel="stylesheet" />
    <link href="/App_Themes/CSS/foundation_menu.css" rel="stylesheet" />
    <script src="/Javascript/MACSystemAdmin-Responsive.js"></script>
    <script type="text/javascript">
        function callParentDocumentFunction() {
            window.parent.parent.hideJQueryDialog();
        }

        function navigateToClient(selectedClientId)
        {
            window.parent.parent.navigateToClientFromPopup(selectedClientId);
        }

    </script>
</head>
<body>
    <div style="padding: 0.5rem;"></div>
    <div class="row">
        <div class="large-12 columns">
            <div id="divGroupClientsHeader" runat="server" class="title_875rem"><span id="spanClientCount" runat="server">0</span>&nbsp;<span id="spanGroupName" runat="server">???</span> Clients</div>
            <div style="padding: 0.5rem;"></div>
            <hr style="margin: 0;" />
        </div>
    </div>
    <div style="padding: 0.625rem;"></div>
    <div class="row">
        <div class="large-12 columns">
            <div id="divGroupClientsContainer" runat="server"></div>
        </div>
    </div>
    <div style="padding: 0.5rem;"></div>
    <div class="row">
        <div class="large-12 columns">
            <div class="button tiny radius" id="closeIframe" runat="server" onclick="javascript: callParentDocumentFunction();">Cancel</div>
        </div>
    </div>
</body>
</html>
