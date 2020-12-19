<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ChangeEnvironmentSettings.aspx.cs" Inherits="Reset.IPs" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Reset IP Addresses</title>
    <link href="/App_Themes/CSS/jquery-ui-smoothness.css" rel="stylesheet" />
    
    <script src="/JavaScript/jquery-1.10.2.js"></script>
    <script src="/JavaScript/jquery-ui-1-11-0.js"></script>
    <script src="/JavaScript/jquery.timer.js"></script>
    <script src="/JavaScript/jquery.validate.js"></script>
    <script src="/Javascript/jquery.printElement.js"></script>

    <%--<link href="/App_Themes/CSS/Admin.css" rel="stylesheet" />--%>
    <link href="/App_Themes/CSS/table-style.css" rel="stylesheet" />

    <link rel="stylesheet" href="/App_Themes/CSS/style.css" />
    <link rel="stylesheet" href="/App_Themes/CSS/prism.css" />
    <link rel="stylesheet" href="/App_Themes/CSS/chosen.css" />

    <link rel="shortcut icon" href="/Images/favicon.ico" />
    <link rel="stylesheet" href="/App_Themes/CSS/foundation_menu.css" />

    <link href="/App_Themes/CSS/Admin.css" rel="stylesheet" />

    <script type="text/javascript" src="/Javascript/Constants.js"></script>
    <script type="text/javascript" src="/Javascript/MACSystemAdmin-Responsive.js"></script>
    <script type="text/javascript" src="/Javascript/jquery.cookie.js"></script>

    <script src="/JavaScript/foundation.min.js"></script>
    <script src="/JavaScript/chosen.jquery.js" type="text/javascript"></script>
    <script src="/JavaScript/prism.js" type="text/javascript" charset="utf-8"></script>

    <script type="text/javascript">
        function callParentDocumentFunction() {

            window.parent.parent.ShowProcessingMessage();
            window.parent.parent.hideJQueryDialog();
            window.parent.parent.resubmitFormFromChild();
        }

        function cancelUpdate()
        {
            window.parent.parent.hideJQueryDialog();
        }
    </script>
</head>
<body>
    <%--<form id="formMain" runat="server" method="post" onsubmit="javascript: ShowProcessingMessage();">--%>
    <form id="formMain" runat="server" method="post">
        <div style="padding: 0.625rem;"></div>

        <div class="row" style="margin-bottom: 0.75rem; position: relative; top: -5px; margin-bottom: -30px;">
            <div class="large-12 medium-12 small-12 columns" style="width: 100%; text-align: right;">
                <a href="javascript: NavigateTopicPopup('5548035fa6e10b18d4fd4a86');" id="link_help_miscCharges">Help?</a>
            </div>
        </div>

        <div class="row">
            <div class="large-12 columns">
                <div id="divIPAddressesContainer" runat="server">
                    <label id="lblIPAddresses">Select a Target Environment
                        <asp:DropDownList ID="dlEnvironments" runat="server" CssClass="chosen-select">
                            <asp:ListItem Value="localhost-generic">Office - LocalHost (Generic)</asp:ListItem>
                            <asp:ListItem Value="localhost-chris">Office - LocalHost (Chris)</asp:ListItem>
                            <asp:ListItem Value="localhost-joe">AWS: Requires VPN - LocalHost (Joe)</asp:ListItem>
                            <asp:ListItem Value="localhost-terry">Office - LocalHost (Terry)</asp:ListItem>
                            <asp:ListItem Value="test-integration.mobileauthcorp.net">AWS: Requires VPN - Test-Integration (test-integration.mobileauthcorp.net)</asp:ListItem>
                            <asp:ListItem Value="test-load.mobileauthcorp.net">AWS: Requires VPN - Test-Load (test-load.mobileauthcorp.net)</asp:ListItem>
                            <asp:ListItem Value="qa.mobileauthcorp.net">AWS: Requires VPN - QA (qa.mobileauthcorp.net)</asp:ListItem>
                            <asp:ListItem Value="production-staging.mobileauthcorp.net">AWS: Requires VPN - Production Staging (production-staging.mobileauthcorp.net)</asp:ListItem>
                            <asp:ListItem Value="www.mobileauthcorp.net">AWS: Requires VPN - Production (www.mobileauthcorp.net)</asp:ListItem>
                            <asp:ListItem Value="demo.mobileauthcorp.net">AWS: Requires VPN - Demo (demo.mobileauthcorp.net)</asp:ListItem>
                    </asp:DropDownList>
                    </label>
                </div>
            </div>
        </div>
        <div style="padding: 0.5rem;"></div>
        <div class="row">
            <div class="large-12 columns">
                <input id="btnUpdate" type="submit" runat="server" value="Update" class="button tiny radius" />
                <input id="btnCancel" type="button" runat="server" value="Cancel" class="button tiny radius" onclick="javascript: cancelUpdate();" />
            </div>
        </div>
    </form>
</body>
</html>
