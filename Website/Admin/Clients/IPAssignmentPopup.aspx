<%@ Page Language="C#" AutoEventWireup="true" CodeFile="IPAssignmentPopup.aspx.cs" Inherits="MACAdmin.Clients.IPAddressAssignmentPopup" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Clients Assigned IP Addresses</title>
    <link href="../../App_Themes/CSS/Admin.css" rel="stylesheet" />
    <link href="../../App_Themes/CSS/foundation_menu.css" rel="stylesheet" />
    <script src="../../Javascript/MACSystemAdmin-Responsive.js"></script>
    <script type="text/javascript">
        function callParentDocumentFunction() {
            window.parent.parent.hideJQueryDialog();
        }

        function navigateToClient(selectedClientId)
        {
            window.parent.parent.navigateToClientFromPopup(selectedClientId);
        }

        function validateIPAddresses()
        {
            var ipAddresses = document.getElementById("txtIPAddresses").value;

            if (ipAddresses == "") {
                //alert("You must specify at least 1 IP address. Otherwise, calls to the system will fail!");
                ipAddresses = "localhost|127.0.0.1";
            }
            else
            {
                document.getElementById("formMain").submit();
            }
        }

    </script>
</head>
<body>
    <%--<form id="formMain" runat="server" method="post" onsubmit="javascript: ShowProcessingMessage();">--%>
    <form id="formMain" runat="server" method="post">
        <div style="padding: 0.625rem;"></div>
        <div class="row" id="divUpdateMsg" runat="server">
            <div class="large-12 columns">
                <div class="alert-box success radius" id="clientVerificationMessage" style="display: ; cursor: pointer;" onclick="javascript: noDisplay();">
                    Client allowed IP adress settings have been saved!
                </div>
            </div>
        </div>
        <div class="row">
            <div class="large-12 columns">
                <div id="divIPAddressesContainer" runat="server">
                    <label id="lblIPAddresses"><span id="spanEmail">IP Addresses (may be single, pipe "|" delimited or range)</span>
                        <asp:TextBox ID="txtIPAddresses" runat="server"></asp:TextBox>
                    </label>
                </div>
            </div>
        </div>
        <div style="padding: 0.5rem;"></div>
        <div class="row">
            <div class="large-12 columns">
                <input id="btnUpdate" type="button" runat="server" value="Update" class="button tiny radius" onclick="javascript: validateIPAddresses();" />
                <input id="btnCancel" type="button" runat="server" value="Cancel" class="button tiny radius" onclick="javascript: callParentDocumentFunction();" />
            </div>
        </div>
        <input id="hiddenAA" runat="server" type="hidden" value="" />
    </form>
</body>
</html>
