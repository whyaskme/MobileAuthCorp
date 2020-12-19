<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ResetAdProviders.aspx.cs" Inherits="Reset.ResetAdProviders" %>

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

        <div id="divAdProvider" runat="server" class="row">
            <div class="large-12 columns" style="font-size: 13px; margin-bottom: 10px;">
                Only fields with specified values will be updated. If the filed is blank, it will be ignored.
            </div>
            <div class="large-12 columns">
                <div id="divProviderContainer" runat="server">

                    <div class="row">
                        <div class="small-12 columns">
                            <label>Provider Name</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtProviderName" runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row">
                        <div class="small-12 columns">
                            <label>Ad ClientId</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtAdClientId" runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row">
                        <div class="small-12 columns">
                            <label>API Key</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtAPIKey" runat="server"></asp:TextBox>                
                        </div>
                    </div>

                    <div class="row">
                        <div class="small-12 columns">
                            <label>API Url</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtAPIUrl" runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row">
                        <div class="small-12 columns">
                            <label>Username</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtAPIUsername" runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row">
                        <div class="small-12 columns">
                            <label>Password</label>
                        </div>
                        <div class="small-12 columns">
                            <asp:TextBox ID="txtAPIPassword" runat="server"></asp:TextBox>
                        </div>
                    </div>

                </div>
            </div>
        </div>

        <div style="padding: 0.5rem;"></div>
        <div class="row">
            <div class="large-12 columns">
                <input id="btnUpdate" type="submit" runat="server" value="Update" class="button tiny radius" />
                <input id="btnCancel" type="button" runat="server" value="Cancel" class="button tiny radius" onclick="javascript: callParentDocumentFunction();" />
            </div>
        </div>

        <input id="hiddenAA" runat="server" type="hidden" value="" />
        <input id="hiddenSelectedProviderId" runat="server" type="hidden" value="" />

    </form>
</body>
</html>
