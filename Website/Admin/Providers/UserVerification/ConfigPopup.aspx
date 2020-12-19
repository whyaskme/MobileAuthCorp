<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ConfigPopup.aspx.cs" Inherits="MACAdmin.Providers.UserVerification.ConfigPopup" ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>User Verification Provider Config</title>
    <link href="/App_Themes/CSS/Admin.css" rel="stylesheet" />
    <script src="/Javascript/MACSystemAdmin-Responsive.js"></script>
    <script type="text/javascript">
        function callParentDocumentFunction() {
            window.parent.parent.hideJQueryDialog();
        }
    </script>
</head>
<body style="background-image: url('/Images/Backgrounds/MAC-Color-Bar-BG-Large.jpg'); background-repeat: no-repeat;">

    <form id="form1" runat="server" style="position: relative; top: -10px;">
        <h2 id="spanProviderName" runat="server" style="position: relative; left: 0; padding-left: 15px;"></h2>

        <div style="width: 100%; color: #ffffff;">&nbsp;</div>

        <div class="divFormControl" style="width: 50%">
                <span class="divFormLabel" style="width: 120px;">Name</span>
            <asp:TextBox ID="txtName" runat="server"></asp:TextBox>
        </div>

        <div class="divFormControl" style="width: 50%">
                <span class="divFormLabel" style="width: 120px;">ApiKey</span>
            <asp:TextBox ID="txtApiKey" runat="server"></asp:TextBox>
        </div>

        <div class="divFormControl" style="width: 50%">
                <span class="divFormLabel" style="width: 120px;">APIVersion</span>
            <asp:TextBox ID="txtApiVersion" runat="server"></asp:TextBox>
        </div>

        <div class="divFormControl" style="width: 50%">
                <span class="divFormLabel" style="width: 120px;">BaseUrl</span>
            <asp:TextBox ID="txtBaseUrl" runat="server"></asp:TextBox>
        </div>

        <div class="divFormControl" style="width: 50%">
                <span class="divFormLabel" style="width: 120px;">Login</span>
            <asp:TextBox ID="txtLogin" runat="server"></asp:TextBox>
        </div>

        <div class="divFormControl" style="width: 50%">
                <span class="divFormLabel" style="width: 120px;">Password</span>
            <asp:TextBox ID="txtPassword" runat="server"></asp:TextBox>
        </div>

        <div class="divFormControl" style="width: 50%">
                <span class="divFormLabel" style="width: 120px;">Processing Params</span>
            <asp:TextBox ID="txtProcessingParameters" runat="server"></asp:TextBox>
        </div>

        <div class="divFormControl" style="width: 50%">
                <span class="divFormLabel" style="width: 120px;">Protocol</span>
            <asp:TextBox ID="txtProtocol" runat="server"></asp:TextBox>
        </div>

        <div class="divFormControl" style="width: 50%">
                <span class="divFormLabel" style="width: 120px;">Proxy Host</span>
            <asp:TextBox ID="txtProxyHost" runat="server"></asp:TextBox>
        </div>

        <div class="divFormControl" style="width: 50%">
                <span class="divFormLabel" style="width: 120px;">Proxy Port</span>
            <asp:TextBox ID="txtProxyPort" runat="server"></asp:TextBox>
        </div>

        <div class="divFormControl" style="width: 50%">
                <span class="divFormLabel" style="width: 120px;">Request Params</span>
            <asp:TextBox ID="txtRequestParameters" runat="server"></asp:TextBox>
        </div>

        <div class="divFormControl" style="width: 50%">
                <span class="divFormLabel" style="width: 120px;">Search Type</span>
            <asp:TextBox ID="txtSearchType" runat="server"></asp:TextBox>
        </div>

        <div class="divFormControl" style="width: 50%">
                <span class="divFormLabel" style="width: 120px;">Service Name</span>
            <asp:TextBox ID="txtServiceName" runat="server"></asp:TextBox>
        </div>

        <div class="divFormControl" style="width: 50%">
                <span class="divFormLabel" style="width: 120px;">&nbsp;</span>
        </div>

        <div style="color: #ffffff;">&nbsp;</div>
        <div style="width: 100%; text-align: center;  position: relative; top: 15px; padding-top: 15px;">
            <asp:Button ID="btnSave" runat="server" Text="Save" />&nbsp;<input type="button" id="btn_cancel" value="Cancel" onclick="javascript: callParentDocumentFunction();" />
        </div>
    </form>
</body>
</html>
