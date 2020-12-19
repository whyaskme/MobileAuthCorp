<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GroupSelection.ascx.cs" Inherits="UserControls.GroupSelection" %>

<link href="/App_Themes/CSS/Admin.css" rel="stylesheet" />
<link href="/App_Themes/CSS/foundation_menu.css" rel="stylesheet" />
<script src="/Javascript/MACSystemAdmin-Responsive.js"></script>

<form id="formMain" runat="server" style="position: relative; top: 0; padding: 0;">

    <asp:Panel ID="pnlGroupManagement" runat="server" CssClass="divFormControlsContainer">

        <h3 id="spanGroupCount" runat="server">0 Groups Available</h3>

        <div id="divGroupManagementMessage" runat="server"></div>

        <div class="divFormControl" style="width: 100%; height: auto; text-align: center; margin-bottom: 15px; ">
            <div style="width: 100%; text-align: center;  position: relative; top: -15px;">
                Double-click a group node to select it.
            </div>
            <div class="divFormControl" style="width: 100%; text-align: center; height: 168px; margin-bottom: 0; overflow-x: hidden; overflow-y: auto; border: solid 0 #c0c0c0;">
                <select id="dlMasterGroups" multiple="true" runat="server" style="width: 75%; height: 100%;"></select>
            </div>
        </div>

    </asp:Panel>

    <asp:HiddenField ID="hiddenAA" runat="server" Value="" />

</form>