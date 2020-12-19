<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ConfigPopup.aspx.cs" Inherits="MACAdmin.Clients.Providers.Advertising.ConfigPopup" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Client Advertising Settings</title>
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

            //alert("updateProvider");
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
            <div class="large-12 medium-12 small-12 columns">
                <asp:CheckBox ID="chkAdPassEnabled" runat="server" Text="AdPass enabled?" AutoPostBack="True" />
            </div>
        </div>

        <div class="row">
            <div class="large-6 medium-6 small-12 columns">
                <label>Provider Name
                <asp:TextBox runat="server" ID="txtProviderName" />
                </label>
            </div>
            <div class="large-6 medium-6 small-12 columns">
                <label>Ad Client Id
                <asp:TextBox runat="server" ID="txtAdClientId" />
                </label>
            </div>
        </div>

        <div class="row">
            <div class="large-6 medium-6 small-12 columns">
                <label>Api Url
                <asp:TextBox runat="server" ID="txtApiUrl" />
                </label>
            </div>
            <div class="large-6 medium-6 small-12 columns">
                <label>Api Key
                <asp:TextBox runat="server" ID="txtApiKey" />
                </label>
            </div>
        </div>

        <div class="row">
            <div class="large-6 medium-6 small-12 columns">
                <label>Api Username
                <asp:TextBox runat="server" ID="txtApiUserName" />
                </label>
            </div>
            <div class="large-6 medium-6 small-12 columns">
                <label>Api Password
                <asp:TextBox runat="server" ID="txtApiPassword" />
                </label>
            </div>
        </div>

        <div class="row">
            <div class="large-6 medium-6 small-12 columns">
                <label>Extra Property 1
                <asp:TextBox runat="server" ID="txtP1" />
                </label>
            </div>
            <div class="large-6 medium-6 small-12 columns">
                <label>Extra Property 2
                <asp:TextBox runat="server" ID="txtP2" />
                </label>
            </div>
        </div>

        <div class="row">
            <div class="large-6 medium-6 small-12 columns">
                <label>Extra Property 3
                <asp:TextBox runat="server" ID="txtP3" />
                </label>
            </div>
            <div class="large-6 medium-6 small-12 columns">
                <label>Extra Property 4
                <asp:TextBox runat="server" ID="txtP4" />
                </label>
            </div>
        </div>

        <div class="row">
            <div class="large-12 columns">
                <%--<asp:Button CssClass="button tiny radius" runat="server" ID="btnSave" Text="Save" />--%>
                <input id="btnSave" type="button" runat="server" class="button tiny radius" value="Save" onclick="javascript: updateProvider();" />
            </div>
        </div>

        <input id="hiddenAA" type="hidden" runat="server" value="" />

    </form>

</body>
</html>
