<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UsersPopup.aspx.cs" Inherits="UsersPopup" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Clients Assigned to Group</title>
    <link href="/App_Themes/CSS/Admin.css" rel="stylesheet" />
    <link href="/App_Themes/CSS/foundation_menu.css" rel="stylesheet" />
    <script src="/Javascript/MACSystemAdmin-Responsive.js"></script>
    <script type="text/javascript">

        function callParentDocumentFunction() {

            var selectedUserIds = document.getElementById("hiddenSelectedUserIds").value;

            // Pass selected values to parent document for update
            window.parent.parent.document.getElementById("hiddenSelectedUserIds").value = selectedUserIds;

            //alert(window.parent.parent.document.getElementById("hiddenSelectedUserIds").value);

            window.parent.parent.hideJQueryDialog();
        }

    </script>
</head>
    <body style="overflow:auto !important;">

    <form id="form1" runat="server">

        <div class="row">
            <div class="large-12 columns">
                <h3 style="font-size:1rem;font-weight:bold;margin-top:0.5rem;" id="spanTopicName" runat="server"></h3>
            </div>
        </div>

        <div style="padding: 0.5rem;"></div>

        <div class="row">
            <div class="large-12 columns" style="border: solid 0px #ff0000; height: 500px;">
                <select id="dlUserList" style="height: 450px;" multiple="true" runat="server"></select>
            </div>
        </div>

        <div class="row">
            <div class="large-12 columns" style="text-align: center;">
                <asp:Button CssClass="tiny button radius" ID="btnSave" runat="server" Text="Save" />
                <input class="tiny button radius" id="btn_cancel" type="button" value="Cancel" onclick="javascript: window.parent.parent.hideJQueryDialog();" />
            </div>
        </div>

        <input id="hiddenSelectedUserIds" runat="server" type="hidden" value="" />

    </form>

</body>
</html>
