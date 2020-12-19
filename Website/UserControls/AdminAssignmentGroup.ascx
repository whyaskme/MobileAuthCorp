<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AdminAssignmentGroup.ascx.cs" Inherits="UserControls.AdminAssignmentGroup" %>

<link href="/App_Themes/CSS/Admin.css" rel="stylesheet" />
<link href="/App_Themes/CSS/foundation_menu.css" rel="stylesheet" />
<script src="/Javascript/MACSystemAdmin-Responsive.js"></script>

<script type="text/javascript">
    function callParentDocumentFunction() {
        window.parent.parent.hideJQueryDialog();
    }

    function callAdminFunction() {

        var selectedAdminCount = 0;
        var adminList = document.getElementById("dlAdministrators");

        for (var i = 0; i < adminList.options.length; i++) {
            if (adminList.options[i].selected) {
                if (adminList.options[i].value == "000000000000000000000000") {
                    selectedAdminCount = selectedAdminCount;
                } else {
                    selectedAdminCount++;
                }
            }
        }

        alert(window.parent.parent.location);

        //window.parent.parent.assignAdministrators(selectedAdminCount);
    }
</script>

<form id="formMain" runat="server">

    <div class="row">
        <div class="large-12 columns">
            <h1 style="font-size:1rem;margin-top:0.5rem; font-weight: bold;" id="divGroupTitle" runat="server">Group Title</h1>
        </div>
    </div>

    <div class="row">
        <div class="small-12 columns">
            <hr style="margin-top:0;margin-bottom:0.5rem;" />
        </div>
    </div>

    <div class="row">
        <div class="small-12 columns">
            <span id="spanAdministratorCount" runat="server" style="font-size: 0.813rem;">Available Administrators: 0</span>
        </div>
    </div>

    <div style="padding: 0.25rem;"></div>

    <div class="row">
        <div class="small-12 columns" id="divInstructions" runat="server">
            <div style="font-size: 0.813rem;">Hold down "Ctrl" and click to select multiple Administrators.</div>
        </div>
    </div>

    <div style="padding: 0.25rem;"></div>

    <div class="row">
        <div class="small-12 columns">
            <%--<select id="dlAdministrators" multiple="true" runat="server" style="height: 10rem;font-size: 0.813rem;" onchange="javascript: clearAdministratorUpdate();"></select>--%>
            <select id="dlAdministrators" multiple="true" runat="server" style="height: 13rem;font-size: 0.813rem;"></select>
        </div>
    </div>

    <%--<div class="row" id="confirmMessage" style="display: none;">
        <div class="small-12 columns">
            <span>Are you sure you want to assign these administrators? <input type="button" value="Assign" class="button tiny radius" /></span>
        </div>
    </div>--%>

    <div class="row">
        <div class="small-12 columns">
            <%--<input class="tiny button radius" type="button" id="btnSaveAdministrator" runat="server" onclick="javascript: callAdminFunction();" value="Save" style="" />--%>
            <input class="tiny button radius" type="submit" id="btnSaveAdministrator" runat="server" value="Save" style="" />
            <input class="tiny button radius" type="button" id="btnCloseWindow" runat="server" onclick="javascript: callParentDocumentFunction();" value="Cancel" style="" />
        </div>
    </div>

    <asp:HiddenField ID="hiddenAA" runat="server" Value="" />

</form>