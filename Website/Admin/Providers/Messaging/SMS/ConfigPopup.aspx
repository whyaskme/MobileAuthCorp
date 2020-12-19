<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ConfigPopup.aspx.cs" Inherits="Admin.Providers.Messaging.SMS.ConfigPopup" ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SMS Provider Config</title>
    <link href="/App_Themes/CSS/foundation_menu.css" rel="stylesheet" />
    <script src="/Javascript/MACSystemAdmin-Responsive.js"></script>
    <script src="/Javascript/jquery-1.10.2.js"></script>
    <script type="text/javascript">
        function callParentDocumentFunction() {

            var selectedProviderId = $('#hiddenProviderID').val();
            var providerToAddToHiddenRetryList = $('#spanProviderName').html();

            providerToAddToHiddenRetryList = providerToAddToHiddenRetryList.replace(" Configuration", "");
            providerToAddToHiddenRetryList = providerToAddToHiddenRetryList.replace("SMS", "(Sms)");

            // Enable edit icon
            var editIcon = window.parent.parent.document.getElementById("img_" + providerToAddToHiddenRetryList);
            editIcon.src = "../../Images/icon-edit.png";

            var providerNameLinkSpan = window.parent.parent.document.getElementById("span_" + providerToAddToHiddenRetryList);

            var tmpProviderName = providerToAddToHiddenRetryList;
            tmpProviderName = tmpProviderName.replace(" (Email)", "");
            tmpProviderName = tmpProviderName.replace(" (Sms)", "");
            tmpProviderName = tmpProviderName.replace(" (Verification)", "");
            tmpProviderName = tmpProviderName.replace(" (Voice)", "");

            var tmpLinkSpan = "<span id='span_" + providerToAddToHiddenRetryList + "'>";
            tmpLinkSpan += "<a id='link_" + providerToAddToHiddenRetryList + " (Sms)' href='javascript: editMessageProvider(this, &apos;Sms&apos;,&apos;" + selectedProviderId + "&apos;);'>";
            tmpLinkSpan += tmpProviderName;
            tmpLinkSpan += "</a>";
            tmpLinkSpan += "</span>";

            providerNameLinkSpan.innerHTML = tmpLinkSpan;

            var providerChecked = window.parent.parent.document.getElementById(providerToAddToHiddenRetryList);
            providerChecked.checked = true;

            var hiddenRetryList = window.parent.parent.document.getElementById("hiddenRetryList");
            var listOptions = window.parent.parent.document.getElementById("selectProviderList");

            tmpProviderName += " (Sms)";

            // Check to see if the provider is already in the hidden ad visible retry list. If so, do not add it again.
            // This might be the case when editing an already defined provider
            if (hiddenRetryList.value.indexOf(tmpProviderName) > -1) {
                // Ignore, do not add to retry lists
            }
            else {
                hiddenRetryList.value += "|" + providerToAddToHiddenRetryList;

                var option = document.createElement("option");
                option.text = providerToAddToHiddenRetryList;
                option.value = option.text;

                listOptions.add(option, listOptions.options.length);
            }

            window.parent.parent.hideJQueryDialog();
        }

        function cancelChanges() {
            window.parent.parent.hideJQueryDialog();
        }
    </script>

</head>
    <body style="overflow:auto !important;" onload="callParentDocumentFunction33()">

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

        <div style="padding:.25rem;"></div>

        <div class="row">
            <div class="small-12 columns">
                <label>Url</label>
            </div>
            <div class="small-12 columns">
                <asp:TextBox ID="txtUrl" runat="server"></asp:TextBox>
            </div>
        </div>

        <div class="row">
            <div class="small-12 columns">
               <label><span id="spanPrivateKey" runat="server">Private Key</span></label>
            </div>
            <div class="small-12 columns">
                <asp:TextBox ID="txtSID" runat="server"></asp:TextBox>
            </div>
        </div>

        <div class="row">
            <div class="small-12 columns">
                <label><span id="spanPublicKey" runat="server">Public Key</span></label>
            </div>
            <div class="small-12 columns">
                <asp:TextBox ID="txtAuthToken" runat="server"></asp:TextBox>
            </div>
        </div>

        <div class="row">
            <div class="small-12 columns">
                <label id="lblShortCode" runat="server">Batch Id / Short Code</label>
            </div>
            <div class="small-12 columns">
                <asp:TextBox ID="txtShortCodeFromNumber" runat="server"></asp:TextBox>
            </div>
        </div>

        <div class="row">
            <div class="small-12 columns">
                <label>API Version</label>
            </div>
            <div class="small-12 columns">
                <asp:TextBox ID="txtApiVersion" runat="server"></asp:TextBox>
            </div>
        </div>

        <div class="row">
            <div class="small-12 columns">
                <label>Newline Replacement</label>
            </div>
            <div class="small-12 columns">
                <asp:TextBox ID="txtNewlineReplacement" runat="server">huh</asp:TextBox>
            </div>
        </div>

        <div class="row" style="display: none;">
            <div class="small-12 columns">
                <label>Key</label>
            </div>
            <div class="small-12 columns">
                <asp:TextBox ID="txtKey" runat="server"></asp:TextBox>
            </div>
        </div>

        <div class="row" style="display: none;">
            <div class="small-12 columns">
                <label>Username</label>
            </div>
            <div class="small-12 columns">
                <asp:TextBox ID="txtLoginUsername" runat="server"></asp:TextBox>
            </div>
        </div>

        <div class="row" style="display: none;">
            <div class="small-12 columns">
                <label>Password</label>
            </div>
            <div class="small-12 columns">
                <asp:TextBox ID="txtLoginPassword" runat="server"></asp:TextBox>
            </div>
        </div>

        <div class="row" style="display: none;">
            <div class="small-12 columns">
                <label>Protocol</label>
            </div>
            <div class="small-12 columns">
                <asp:TextBox ID="txtProtocol" runat="server"></asp:TextBox>
            </div>
        </div>

        <div class="row" style="display: none;">
            <div class="small-12 columns">
                <label>Port</label>
            </div>
            <div class="small-12 columns">
                <asp:TextBox ID="txtPort" runat="server"></asp:TextBox>
            </div>
        </div>

        <div class="row" style="display: none;">
            <div class="small-12 columns">
                <label>Server</label>
            </div>
            <div class="small-12 columns">
                <asp:TextBox ID="txtServer" runat="server"></asp:TextBox>
            </div>
        </div>

        <div class="row" style="display: none;">
            <div class="small-12 columns">
                <label>Voice Token</label>
            </div>
            <div class="small-12 columns">
                <asp:TextBox ID="txtVoiceToken" runat="server"></asp:TextBox>
            </div>
        </div>

        <div class="row" style="display: none;">
            <div class="small-12 columns">
                <label>Phone Format</label>
            </div>
            <div class="small-12 columns">
                <asp:TextBox ID="txtPhoneNumberFormat" runat="server"></asp:TextBox>
            </div>
        </div>

        <div class="row" style="display: none;">
            <div class="small-12 columns">
                <label>Provider Charge</label>
            </div>
            <div class="small-12 columns">
                <asp:TextBox ID="txtProviderCharge" runat="server"></asp:TextBox>
            </div>
        </div>

        <div class="row" style="display: none;">
            <div class="small-12 columns">
                <label>Client Charge</label>
            </div>
            <div class="small-12 columns">
                <asp:TextBox ID="txtClientCharge" runat="server"></asp:TextBox>
            </div>
        </div>

        <div class="row">
            <div class="small-12 columns" style="width: 100%; text-align: center;">
                <asp:Button CssClass="tiny button radius" ID="btnSave" runat="server" Text="Save" />
                <input class="tiny button radius" id="btn_cancel" type="button" runat="server" value="Cancel" onclick="javascript: window.parent.parent.hideJQueryDialog();" />
            </div>
            <div class="small-12 columns">
                <input id="hiddenProviderID" type="hidden" runat="server" style="width: 300px;" />
            </div>
        </div>

    </form>
</body>
</html>
