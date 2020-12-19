﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ConfigPopup.aspx.cs" Inherits="Admin.Providers.Messaging.Email.ConfigPopup" ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Email Provider Config</title>
    <link href="/App_Themes/CSS/foundation_menu.css" rel="stylesheet" />
    <script src="/Javascript/MACSystemAdmin-Responsive.js"></script>
    <script src="/Javascript/jquery-1.10.2.js"></script>
    <script type="text/javascript">

        function callParentDocumentFunction() {
            var selectedProviderId = $('#hiddenProviderID').val();
            var providerToAddToHiddenRetryList = $('#spanProviderName').html();

            providerToAddToHiddenRetryList = providerToAddToHiddenRetryList.replace(" Configuration", "");
            providerToAddToHiddenRetryList = providerToAddToHiddenRetryList.replace("Email", "(Email)");
            providerToAddToHiddenRetryList = providerToAddToHiddenRetryList;

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
            tmpLinkSpan += "<a id='link_" + providerToAddToHiddenRetryList + " (Voice)' href='javascript: editMessageProvider(this, &apos;Voice&apos;,&apos;" + selectedProviderId + "&apos;);'>";
            tmpLinkSpan += tmpProviderName;
            tmpLinkSpan += "</a>";
            tmpLinkSpan += "</span>";

            providerNameLinkSpan.innerHTML = tmpLinkSpan;

            var providerChecked = window.parent.parent.document.getElementById(providerToAddToHiddenRetryList);
            providerChecked.checked = true;

            var hiddenRetryList = window.parent.parent.document.getElementById("hiddenRetryList");
            var listOptions = window.parent.parent.document.getElementById("selectProviderList");

            tmpProviderName += " (Email)";

            // Check to see if the provider is already in the hidden ad visible retry list. If so, do not add it again.
            // This might be the case when editing an already defined provider
            if (hiddenRetryList.value.indexOf(tmpProviderName) > -1)
            {
                // Ignore, do not add to retry lists
            }
            else
            {
                hiddenRetryList.value += "|" + providerToAddToHiddenRetryList;

                var option = document.createElement("option");
                option.text = providerToAddToHiddenRetryList;
                option.value = option.text;

                listOptions.add(option, listOptions.options.length);
            }

            window.parent.parent.hideJQueryDialog();
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
            <div class="large-12 medium-12 small-12 columns">
                <table style="width: 100%; border: none; padding: 0px;">
                    <tr>
                        <td style="width: 50%; padding: 0px;">
                            <asp:CheckBox ID="chkCredentialsRequired" runat="server" Text="Credentials Required?" />
                        </td>
                        <td style="width: 50%; padding: 0px;">
                            <asp:CheckBox ID="chkRequiresSsl" runat="server" Text="Requires SSL?" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 50%; padding: 0px;">
                            <asp:CheckBox ID="chkIsBodyHtml" runat="server" Text="Html Body?" />
                        </td>
                        <td style="width: 50%; padding: 0px;">
                            <asp:CheckBox ID="chkNotifyAdminOnFailure" runat="server" Text="Notify on Failure?" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>

        <div class="row" style="display: none;">
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
                <asp:TextBox ID="txtLoginUsername" runat="server"></asp:TextBox>                
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