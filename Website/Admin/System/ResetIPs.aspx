<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ResetIPs.aspx.cs" Inherits="Reset.IPs" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Reset IP Addresses</title>
    <link href="/App_Themes/CSS/foundation_menu.css" rel="stylesheet" />
    <script src="/Javascript/MACSystemAdmin-Responsive.js"></script>
    <script type="text/javascript">
        function callParentDocumentFunction() {
            window.parent.parent.hideJQueryDialog();
        }
    </script>
</head>
<body>
    <%--<form id="formMain" runat="server" method="post" onsubmit="javascript: ShowProcessingMessage();">--%>
    <form id="formMain" runat="server" method="post">

        <div class="row" style="position: relative; top: 10px; margin-bottom: 10px;">
            <div class="large-12 medium-12 small-12 columns" style="width: 100%; text-align: right;">
                <a href="javascript: NavigateTopicPopup('54ee66f6b5655a1fd4adc17e');" id="link_help">Help?</a>
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
                <input id="btnUpdate" type="submit" runat="server" value="Update" class="button tiny radius" onclick="javascript: validateIPAddresses();" />
                <input id="btnCancel" type="button" runat="server" value="Cancel" class="button tiny radius" onclick="javascript: callParentDocumentFunction();" />
            </div>
        </div>
    </form>
</body>
</html>
