<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GroupAssignment.ascx.cs" Inherits="UserControls.GroupAssignment" %>

<link href="/App_Themes/CSS/Admin.css" rel="stylesheet" />
<link href="/App_Themes/CSS/foundation_menu.css" rel="stylesheet" />
<script src="/Javascript/MACSystemAdmin-Responsive.js"></script>

<script type="text/javascript">
    function callParentDocumentFunction() {
        window.parent.parent.hideJQueryDialog();
    }
</script>

<form id="formMain" runat="server">

    <div class="row">
        <div class="large-12 columns">
            <div style="font-size:1rem;font-weight:bold;margin-top:0.75rem;margin-bottom:0.75rem;text-align:left;" id="spanGroupCount" runat="server">0 Groups Available</div>
        </div>
    </div>

    <div class="row">
        <div class="small-12 columns">
            <hr style="margin-top:0;margin-bottom:1rem;" />
        </div>
    </div>

    <div class="row">
        <div class="small-12 columns">
            <div id="assignGroupMessage" runat="server" class="alert-box success radius" onclick="javascript: noDisplay();" style="display: block; cursor: pointer;">
                Client has been successfuly assigned to Group(s)
            </div>
        </div>
    </div>

    <div class="row">
        <div class="small-12 columns">
            <div id="spanGroupAssignmentCount" runat="server" style="font-size: 0.875rem;margin-bottom:0.75rem;text-align:left;">0 Groups Assigned</div>
        </div>
    </div>

    <div class="row">
        <div class="small-12 columns">
            <div id="divGroupsContainer" runat="server" class="MyResourcesContainer"></div>
        </div>
    </div>

    <div style="padding:.5rem;"></div>

    <div class="row">
        <div class="small-12 columns" style="padding-left: 0.9375rem;">
            <%--<input class="tiny button radius" type="button" id="btnClearAssignments" runat="server" onclick="javascript: clearGroupAssignments(this);" value="Clear" style="" />--%>
            <input class="tiny button radius" type="submit" id="btnSaveGroup" runat="server" onclick="javascript: editGroupAssignment(this);" value="Assign to Group(s)" style="" />
            <input class="tiny button radius" type="button" id="btnCloseWindow" runat="server" onclick="javascript: callParentDocumentFunction();" value="Cancel" style="" />
        </div>
    </div>

    <asp:HiddenField ID="hiddenAA" runat="server" Value="" />
    <asp:HiddenField ID="hiddenYs" runat="server" Value="" />

</form>