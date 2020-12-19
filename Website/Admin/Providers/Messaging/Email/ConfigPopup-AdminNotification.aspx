<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ConfigPopup-AdminNotification.aspx.cs" Inherits="Admin.Providers.Messaging.Email.ConfigPopupAdminNotification" ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Email Provider Config</title>
    <link href="/App_Themes/CSS/Admin.css" rel="stylesheet" />
    <link href="/App_Themes/CSS/foundation_menu.css" rel="stylesheet" />
    <script src="/Javascript/MACSystemAdmin-Responsive.js"></script>

    <script type="text/javascript">
        function callParentDocumentFunction() {
            window.parent.parent.hideJQueryDialog();
        }
    </script>
</head>
    <body style="overflow-y: auto;">

    <form id="form1" runat="server">
        <div class="row">
            <div class="large-12 columns">
                <div style="padding: 0.5rem 0 0;">
                    <h1 class="title" id="spanProviderName" runat="server"></h1>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="large-12 columns">
                <hr style="margin: 0.25rem 0 1rem;" />
            </div>
        </div>
        <div class="row">
            <div class="large-6 medium-6 small-12 columns">
                <asp:CheckBox ID="chkCredentialsRequired" runat="server" Text="&nbsp;&nbsp;Credentials Required?" />
            </div>
            <div class="large-6 medium-6 small-12 columns">
                <asp:CheckBox ID="chkIsBodyHtml" runat="server" Text="&nbsp;&nbsp;Html Body?" />
            </div>
        </div>
        <div class="row">
            <div class="large-6 medium-6 small-12 columns">
                <asp:CheckBox ID="chkRequiresSsl" runat="server" Text="&nbsp;&nbsp;Requires SSL?" />
            </div>
            <div class="large-6 medium-6 small-12 columns">
                <asp:CheckBox ID="chkNotifyAdminOnFailure" runat="server" Text="&nbsp;&nbsp;Notify on Failure?" />
            </div>
        </div>
        <div style="padding: 0.25rem;"></div>
        <div class="row">
            <div class="large-6 medium-6 small-12 columns">
                <label>Name</label>
                <asp:TextBox ID="txtName" runat="server"></asp:TextBox>
            </div>
            <div class="large-6 medium-6 small-12 columns">
                <label>From Email</label>
                <asp:TextBox ID="txtFromEmail" runat="server"></asp:TextBox>
            </div>
        </div>
        <div class="row">
            <div class="large-6 medium-6 small-12 columns">
                <label>UserName</label>
                <asp:TextBox ID="txtLoginUserName" runat="server"></asp:TextBox>
            </div>
            <div class="large-6 medium-6 small-12 columns">
                <label>Password</label>
                <asp:TextBox ID="txtLoginPassword" runat="server"></asp:TextBox>
            </div>
        </div>
        <div class="row">
            <div class="large-6 medium-6 small-12 columns">
                <label>Server</label>
                <asp:TextBox ID="txtServer" runat="server"></asp:TextBox>
            </div>
            <div class="large-6 medium-6 small-12 columns">
                <label>Port</label>
                <asp:TextBox ID="txtPort" runat="server"></asp:TextBox>
            </div>
        </div>
        <div class="row">
            <div class="large-6 medium-6 small-12 columns">
                <label>Newline Replacement</label>
                <asp:TextBox ID="txtNewlineReplacement" runat="server"></asp:TextBox>
            </div>
            <div class="large-6 medium-6 small-12 columns">
                &nbsp;
            </div>
        </div>
        <div class="row">
            <div class="large-12 columns" style="width: 100%; text-align: center;">
                <asp:Button CssClass="tiny button radius" ID="btnSave" runat="server" Text="Save" />
                <input class="tiny button radius" id="btn_cancel" type="button" runat="server" value="Cancel" onclick="javascript: callParentDocumentFunction();" />
            </div>
        </div>

        <input id="hiddenProviderID" type="hidden" runat="server" style="width: 300px;" />

    </form>
</body>
</html>
