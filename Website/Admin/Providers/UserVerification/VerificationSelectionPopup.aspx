<%@ Page Language="C#" AutoEventWireup="true" CodeFile="VerificationSelectionPopup.aspx.cs" Inherits="Admin.Providers.UserVerification.VerificationSelectionPopup" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Clients Assigned to Group</title>

    <link href="/App_Themes/CSS/Admin.css" rel="stylesheet" />
    <link href="/App_Themes/CSS/foundation_menu.css" rel="stylesheet" />
    <script src="/Javascript/MACSystemAdmin-Responsive.js"></script>

</head>
<body style="overflow:auto !important;">
    
    <form id="form1" runat="server">
        <div style="display:block;width:100%;margin:0 auto;">

            <div class="row">
                <div class="small-12 columns">
                    <div style="margin:1rem 0 0;text-align:left;">
                        <span id="spanProviderName" runat="server"><strong>User Verification Providers</strong></span>
                        <hr />
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="small-12 columns">
                    <div id="assignVerificationProvidersMessage" runat="server" class="alert-box success radius" onclick="javascript: noDisplay();" style="display: block; cursor: pointer;">
                        Successfuly assigned to User Verification provider(s)
                    </div>
                </div>
            </div>

            <div style="padding: 0.5rem;"></div>

            <div style="display: block;width: 12.188rem;margin:0 auto;">
                <div id="divVerificationContainer" runat="server"></div>
            </div>

            <div style="padding: 0.125rem;"></div>

            <div style="text-align:center;">
                <input class="button tiny radius" id="btn_save" runat="server" type="submit" value="Save" />
                <%--<input class="button tiny radius" type="button" value="Cancel" onclick="javascript: callParentDocumentFunction();" />--%>
            </div>

            <div id="divDlProvidersContainer" style="visibility: hidden;">
                <asp:DropDownList ID="dlGlobalVerificationProviders" runat="server"></asp:DropDownList>
            </div>

            <asp:HiddenField ID="hiddenSelectedProviderIds" runat="server" Value="" />
        </div>
    </form>
</body>
</html>
