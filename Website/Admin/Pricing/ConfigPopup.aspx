<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ConfigPopup.aspx.cs" Inherits="Admin.Providers.Messaging.Email.ConfigPopup" ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Email Provider Config</title>
    <link href="/App_Themes/CSS/foundation_menu.css" rel="stylesheet" />
    <script src="/Javascript/MACSystemAdmin-Responsive.js"></script>
    <script type="text/javascript">
        function callParentDocumentFunction()
        {
            window.parent.parent.hideJQueryDialog();
            //alert("Update parent button label");
        }
    </script>
</head>
    <body style="overflow:auto !important;">

    <form id="form1" runat="server">
        <div class="row">
            <div class="large-12 columns">
                <h3 style="font-size:1rem;font-weight:bold;margin-top:0.5rem;" id="spanProviderName" runat="server"></h3>
            </div>
        </div>

        <div class="row">
            <div class="small-12 columns">
                <hr style="margin-top:0;margin-bottom:1rem;" />
            </div>
        </div>

        <div class="row">
            <div class="small-12 columns">
                <asp:CheckBox ID="chkCredentialsRequired" runat="server" Text="&nbsp;&nbsp;Credentials Required?" />
            </div>
            <div class="small-12 columns">
                <asp:CheckBox ID="chkRequiresSsl" runat="server" Text="&nbsp;&nbsp;Requires SSL?" />
            </div>
            <div class="small-12 columns">
                <asp:CheckBox ID="chkIsBodyHtml" runat="server" Text="&nbsp;&nbsp;Html Body?" />
            </div>
            <div class="small-12 columns">
                <asp:CheckBox ID="chkNotifyAdminOnFailure" runat="server" Text="&nbsp;&nbsp;Notify on Failure?" />
            </div>
        </div>

        <div style="padding:.25rem;"></div>

        <div class="row">
            <div class="small-12 columns">
                <label>Name</label>
            </div>
            <div class="small-12 columns">
                <asp:TextBox ID="txtName" runat="server"></asp:TextBox>
            </div>
        </div>

        <div class="row">
            <div class="small-12 columns">
                <label>From Email</label>
            </div>
            <div class="small-12 columns">
                <asp:TextBox ID="txtFromEmail" runat="server"></asp:TextBox>
            </div>
        </div>

        <div class="row">
            <div class="small-12 columns">
                <label>UserName</label>
            </div>
            <div class="small-12 columns">
                <asp:TextBox ID="txtLoginUserName" runat="server"></asp:TextBox>                
            </div>
        </div>

        <div class="row">
            <div class="small-12 columns">
                <label>Password</label>
            </div>
            <div class="small-12 columns">
                <asp:TextBox ID="txtLoginPassword" runat="server"></asp:TextBox>
            </div>
        </div>

        <div class="row">
            <div class="small-12 columns">
                <label>Server</label>
            </div>
            <div class="small-12 columns">
                <asp:TextBox ID="txtServer" runat="server"></asp:TextBox>
            </div>
        </div>

        <div class="row">
            <div class="small-12 columns">
                <label>Port</label>
            </div>
            <div class="small-12 columns">
                <asp:TextBox ID="txtPort" runat="server"></asp:TextBox>
            </div>
        </div>

        <div class="row">
            <div class="small-12 columns">
                <label>Newline Replacement</label>
            </div>
            <div class="small-12 columns">
                <asp:TextBox ID="txtNewlineReplacement" runat="server"></asp:TextBox>
            </div>
        </div>

        <div class="row">
            <div class="small-12 columns">
                <label>&nbsp;</label>
            </div>
        </div>

        <div class="row">
            <div class="small-12 columns">
                <asp:Button CssClass="tiny button radius" ID="btnSave" runat="server" Text="Save" />
                <input class="tiny button radius" id="btn_cancel" type="button" value="Cancel" onclick="javascript: callParentDocumentFunction();" />
            </div>
            <div class="small-12 columns">
                <input id="hiddenProviderID" type="hidden" runat="server" style="width: 300px;" />
            </div>
        </div>        

    </form>
</body>
</html>
