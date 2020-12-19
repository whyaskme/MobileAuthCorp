<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ResetMessageProviders.aspx.cs" Inherits="Reset.ResetMessageProviders" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Reset IP Providers</title>
    <link href="../../App_Themes/CSS/foundation_menu.css" rel="stylesheet" />
    <link href="../../App_Themes/CSS/chosen.css" rel="stylesheet" />

    <script src="../../Javascript/MACSystemAdmin-Responsive.js"></script>
    <script src="../../JavaScript/foundation.min.js"></script>
    <script src="../../JavaScript/chosen.jquery.js" type="text/javascript"></script>
    <script src="../../JavaScript/prism.js" type="text/javascript" charset="utf-8"></script>

    <script type="text/javascript">
        function callParentDocumentFunction() {
            window.parent.parent.hideJQueryDialog();
        }

        function SetSelectedProvider()
        {
            var providerList = document.getElementById("dlProviders");
            var selectedProviderId = providerList.options[providerList.selectedIndex].value;

            document.getElementById("hiddenSelectedProviderId").value = selectedProviderId;
            document.getElementById("hiddenAA").value = "GetProviderDetails";
            document.getElementById("formMain").submit();
        }

        function SubmitUpdate()
        {
            var providerList = document.getElementById("dlProviders");
            var selectedProviderId = providerList.options[providerList.selectedIndex].value;

            document.getElementById("hiddenSelectedProviderId").value = selectedProviderId;
            document.getElementById("hiddenAA").value = "UpdateProvider";
            document.getElementById("formMain").submit();
        }
    </script>
</head>
<body>
    <%--<form id="formMain" runat="server" method="post" onsubmit="javascript: ShowProcessingMessage();">--%>
    <form id="formMain" runat="server" method="post">

        <div class="row" style="position: relative; top: 10px; margin-bottom: 25px;">
            <div class="large-12 medium-12 small-12 columns" style="width: 100%; text-align: right;">
                <a href="javascript: NavigateTopicPopup('54ee670db5655a1fd4adc17f');" runat="server" id="link_help">Help?</a>
            </div>
        </div>

        <div class="row">
            <div class="large-12 columns">
                <div id="div1" runat="server">
                    <asp:DropDownList ID="dlProviderTypes" runat="server" AutoPostBack="true" CssClass="chosen-select" OnSelectedIndexChanged="dlProviderTypes_SelectedIndexChanged">
                        <asp:ListItem Text="Select a Provider Type" Value="Select a Provider Type" />
                        <asp:ListItem Text="Email Provider" Value="Email" />
                        <asp:ListItem Text="Sms Provider" Value="Sms" />
                        <asp:ListItem Text="Voice Provider" Value="Voice" />
                    </asp:DropDownList>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="large-12 columns">
                <div id="div2" runat="server">
                    <select id="dlProviders" runat="server" class="chosen-select" onchange="javascript: SetSelectedProvider();"></select>
                </div>
                <div id="div4" runat="server">
                    <label>ProviderId:
                        <span id="spanProviderId" runat="server" style="font-weight: bold;">ProviderId</span>
                    </label>
                </div>
            </div>
        </div>

        <div style="padding: 0.625rem;"></div>

        <div id="divEmailProvider" runat="server" class="row">
            <div class="large-12 columns">
                <div id="divProviderContainer" runat="server">

                    <div class="row">
                        <div class="large-12 medium-12 small-12 columns">
                            <table style="width: 100%; border: none; padding: 0px;">
                                <tr>
                                    <td style="width: 50%; padding: 0px;">
                                        <asp:CheckBox ID="chkEmailCredentialsRequired" runat="server" Text="Credentials Required?" />
                                    </td>
                                    <td style="width: 50%; padding: 0px;">
                                        <asp:CheckBox ID="chkEmailRequiresSsl" runat="server" Text="Requires SSL?" />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="width: 50%; padding: 0px;">
                                        <asp:CheckBox ID="chkEmailIsBodyHtml" runat="server" Text="Html Body?" />
                                    </td>
                                    <td style="width: 50%; padding: 0px;">
                                        <asp:CheckBox ID="chkEmailNotifyAdminOnFailure" runat="server" Text="Notify on Failure?" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>

                    <div class="row">
                        <div class="small-12 columns">
                            <label>Name</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtEmailName" runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row">
                        <div class="small-12 columns">
                            <label>From Email</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtEmailFromEmail" runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row">
                        <div class="small-12 columns">
                            <label>UserName</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtEmailLoginUsername" runat="server"></asp:TextBox>                
                        </div>
                    </div>

                    <div class="row">
                        <div class="small-12 columns">
                            <label>Password</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtEmailLoginPassword" runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row">
                        <div class="small-12 columns">
                            <label>Server</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtEmailServer" runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row">
                        <div class="small-12 columns">
                            <label>Port</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtEmailPort" runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row">
                        <div class="small-12 columns">
                            <label>Newline Replacement</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtEmailNewlineReplacement" runat="server"></asp:TextBox>
                        </div>
                    </div>   

                </div>
            </div>
        </div>

        <div id="divSmsProvider" runat="server" class="row">
            <div class="large-12 columns">
                <div id="div3" runat="server">

                    <div class="row">
                        <div class="small-12 columns">
                            <label>Url</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtSmsUrl" runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row">
                        <div class="small-12 columns">
                           <label><span id="spanPrivateKey" runat="server">Private Key</span></label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtSmsSID" runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row">
                        <div class="small-12 columns">
                            <label><span id="spanPublicKey" runat="server">Public Key</span></label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtSmsAuthToken" runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row">
                        <div class="small-12 columns">
                            <label>From Number (Short Code)</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtSmsShortCodeFromNumber" runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row" style="">
                        <div class="small-12 columns">
                            <label>API Version</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtSmsApiVersion" runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row">
                        <div class="small-12 columns">
                            <label>Newline Replacement</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtSmsNewlineReplacement" runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row" style="display: none;">
                        <div class="small-12 columns">
                            <label>Key</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtSmsKey" runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row" style="display: none;">
                        <div class="small-12 columns">
                            <label>Username</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtSmsLoginUsername" runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row" style="display: none;">
                        <div class="small-12 columns">
                            <label>Password</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtSmsLoginPassword" runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row" style="display: none;">
                        <div class="small-12 columns">
                            <label>Protocol</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtSmsProtocol" runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row" style="display: none;">
                        <div class="small-12 columns">
                            <label>Port</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtSmsPort" runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row" style="display: none;">
                        <div class="small-12 columns">
                            <label>Server</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtSmsServer" runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row" style="display: none;">
                        <div class="small-12 columns">
                            <label>Voice Token</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtSmsVoiceToken" runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row" style="display: none;">
                        <div class="small-12 columns">
                            <label>Phone Format</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtSmsPhoneNumberFormat" runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row" style="display: none;">
                        <div class="small-12 columns">
                            <label>Provider Charge</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtSmsProviderCharge" runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row" style="display: none;">
                        <div class="small-12 columns">
                            <label>Client Charge</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtSmsClientCharge" runat="server"></asp:TextBox>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="divVoiceProvider" runat="server" class="row">
            <div class="large-12 columns">
                <div id="div5" runat="server">

                    <div class="row">
                        <div class="small-12 columns">
                            <asp:CheckBox ID="chkVoiceRequiresSsl" runat="server" Text="Requires SSL?" />
                        </div>
                    </div>

                    <div class="row">
                        <div class="small-12 columns">
                            <label>Name</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtVoiceName" runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row">
                        <div class="small-12 columns">
                            <label>API Version</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtVoiceApiVersion" runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row">
                        <div class="small-12 columns">
                            <label>From Phone Number</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtVoiceFromPhoneNumber" runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row" style="display: none;">
                        <div class="small-12 columns">
                            <label>Key</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtVoiceKey" runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row">
                        <div class="small-12 columns">
                            <label>Username</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtVoiceLoginUsername" runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row">
                        <div class="small-12 columns">
                            <label>Password</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtVoiceLoginPassword" runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row" style="display: none;">
                        <div class="small-12 columns">
                            <label>Port</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtVoicePort" runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row" style="display: none;">
                        <div class="small-12 columns">
                            <label>Protocol</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtVoiceProtocol" runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row">
                        <div class="small-12 columns">
                            <label>Server</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtVoiceServer" runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row" style="display: none;">
                        <div class="small-12 columns">
                            <label>Sid</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtVoiceSID" runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row" style="display: none;">
                        <div class="small-12 columns">
                            <label>Voice Token</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtVoiceVoiceToken" runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row" style="display: none;">
                        <div class="small-12 columns">
                            <label>Newline Replacement</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtVoiceNewlineReplacement" runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row" style="display: none;">
                        <div class="small-12 columns">
                            <label>Phone Number Format</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtVoicePhoneNumberFormat" runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row" style="display: none;">
                        <div class="small-12 columns">
                            <label>Provider Charge</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtVoiceProviderCharge" runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row" style="display: none;">
                        <div class="small-12 columns">
                            <label>Client Charge</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtVoiceClientCharge" runat="server"></asp:TextBox>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div style="padding: 0.5rem;"></div>
        <div class="row">
            <div class="large-12 columns">
                <input id="btnUpdate" type="button" runat="server" disabled="disabled" value="Update" class="button tiny radius" onclick="javascript: SubmitUpdate();" />
                <input id="btnCancel" type="button" runat="server" value="Cancel" class="button tiny radius" onclick="javascript: callParentDocumentFunction();" />
            </div>
        </div>

        <input id="hiddenAA" runat="server" type="hidden" value="" />
        <input id="hiddenSelectedProviderId" runat="server" type="hidden" value="" />

    </form>
</body>
</html>
