<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BillingUserAssignment.ascx.cs" Inherits="UserControls.BillingUserAssignment" %>

<link href="/App_Themes/CSS/Admin.css" rel="stylesheet" />
<link href="/App_Themes/CSS/foundation_menu.css" rel="stylesheet" />
<script src="/Javascript/MACSystemAdmin-Responsive.js"></script>

<script>

    $(document).ready(function () {

    });

    function AssignUsersToBillNotify() {
        var selectedUserId = "WTF???";
        alert("selectedUserId - " + selectedUserId);
    }

</script>

<form id="formMain" runat="server">

    <div class="row">
        <div class="large-12 columns">
            <span style="" id="spanAdministratorCount" runat="server"></span>
        </div>
    </div>

    <div class="row">
        <div class="small-12 columns">
            <hr style="margin-top:0;margin-bottom:1rem;" />
        </div>
    </div>

    <div class="row">
        <div class="small-12 columns">
            <div id="assignAdministratorsMessage" runat="server" class="alert-box success radius" onclick="javascript: noDisplay();" style="display: block; cursor: pointer;">
                User have been successfully assigned
            </div>
        </div>
    </div>

    <div class="row">
        <div class="small-12 columns">
            <span style="font-size: 0.875rem;">To select multiple Users, hold down "Ctrl" and click.</span>
        </div>
    </div>

    <div style="padding:.25rem;"></div>

    <div class="row">
        <div class="small-12 columns">
            <select id="dlAdministrators" style="height:20rem;" multiple="true" runat="server"></select>
        </div>
    </div>

    <div class="row">
        <div class="small-12 columns">
            <input class="tiny button radius" type="submit" id="btnSaveAdministrator" runat="server" value="Assign" onclick="javascript: callParentMessageUpdate();" />
        </div>
    </div>

    <asp:HiddenField ID="hiddenAA" runat="server" Value="" />

</form>