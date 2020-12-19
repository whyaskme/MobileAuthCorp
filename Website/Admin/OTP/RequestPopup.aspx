<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RequestPopup.aspx.cs" Inherits="MACAdmin.Otp.RequestOtpPopup" ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Otp Request</title>
    <link href="/App_Themes/CSS/Admin.css" rel="stylesheet" />
    <link href="/App_Themes/CSS/foundation_menu.css" rel="stylesheet" />
    <script src="/Javascript/MACSystemAdmin-Responsive.js"></script>
    <script type="text/javascript">
        function callParentDocumentFunction() {
            window.parent.parent.hideJQueryDialog();
        }
    </script>
</head>
    <body>
    <form id="form1" runat="server">
        <div class="row">
            <div class="large-12 columns">
                <div style="padding: 0.5rem 0 0;">
                    <h1 class="title" id="spanProviderName" runat="server">Request Otp Service</h1>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="large-12 columns">
                <hr style="margin: 0.25rem 0 1rem;" />
            </div>
        </div>
        <div class="row">
            <div class="large-12 columns">
                <div id="divServiceResponse" runat="server"></div>
            </div>
        </div>
        <div class="row">
            <div class="large-12 columns">
                <div id="spanOtpMessage" runat="server">Otp sent to your cell phone</div>
            </div>
        </div>
        <div style="padding: 0.125rem;"></div>
        <div class="row">
            <div class="large-12 columns">
                <asp:TextBox ID="txtOtp" runat="server"></asp:TextBox>
            </div>
        </div>
        <div class="row">
            <div class="large-12 columns">
                <input class="tiny button radius" id="btn_validate" type="submit" value="Validate" />
                <input class="tiny button radius" id="btn_cancel" type="button" value="Cancel" onclick="javascript: callParentDocumentFunction();" />
            </div>
        </div>

        <asp:HiddenField ID="hiddenD" runat="server" />
        <asp:HiddenField ID="hiddenO" runat="server" />
        <asp:HiddenField ID="hiddenRequestID" runat="server" />
        <input type="hidden" id="hiddenAD" runat="server" value="" />
    </form>
</body>
</html>
