<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GroupManagement.ascx.cs" Inherits="UserControls.GroupManagement" %>

<link href="/App_Themes/CSS/Admin.css" rel="stylesheet" />
<link href="/App_Themes/CSS/foundation_menu.css" rel="stylesheet" />
<script src="/Javascript/MACSystemAdmin-Responsive.js"></script>
<script>
    $(document).ready(function () {
        $('#copyrightContainer').hide();
        $('#clearSelectionControl').hide();
    });

</script>

<form id="formMain_Group">

    <div style="padding: 0.5rem;"></div>
    <div class="row">
        <div class="large-12 columns">
            <label>
                <span id="h2PageTitle" runat="server"></span>
                <span id="Span1" runat="server">Please select a group to view/edit it's properties or click "New Group".</span>
                <span id="clearSelectionControl" style="float:right;"> <a href="#" onclick="javascript: resetGroupControlsContainer();" id="link_resetGroupControls">[Clear Selection]</a></span>
            </label>
        </div>
    </div>

<%--    <div style="padding:0.125rem"></div>--%>
    <div class="row">
        <div class="large-12 columns">
            <div style="display: block;height: 10.15rem;overflow-y: auto;border: 1px solid #b3b3b3;font-size: .8125rem;" id="GroupControlsContainer" runat="server"></div>
        </div>
    </div>

    <div style="padding: 0.5rem;"></div>

    <div class="row" id="scroll2">
        <div class="large-12 columns">
            <div class="alert-box success radius" id="divGroupManagementMessage" runat="server" style="display: none;cursor: pointer;" onclick="JavaScript: noDisplay();"></div>
            <div class="alert-box success radius" id="divAssignedAdminsMessage" runat="server" style="display: none;cursor: pointer;" onclick="JavaScript: noDisplay();">hello</div>
            <div class="alert-box warning radius" id="divDisabledGroupMessageText" style="display: none;cursor: pointer;" onclick="JavaScript: noDisplay();">
                This sub-group has been disabled. It's parent-group must be re-enabled first.
            </div>
        </div>
    </div>

    <div class="row" style="position: relative; top: -15px;">
        <div class="large-12 columns">
            <div id="GroupDisplay" style="margin-top: 1rem;">

                <%--<div id="divGroupList" runat="server">Group List</div>--%>

                <div style="font-weight: bold; font-size: 14px; color: #c0c0c0; text-align: center;">Group Details</div>

                <fieldset style="margin-top: 15px;">
                <div class="row">
                    <div class="large-12 medium-12 small-12 columns" style="text-align: right; z-index: 1000;">
                        <a href="javascript: NavigateTopicPopup('5490bf7eead63627d88e3e1f');" id="link_help_groupDetails">Help?</a>
                    </div>
                    <div class="large-12 medium-12 small-12 columns" style="position: relative; top: -25px;">
                        <table style="width: 100%; height: 175px; border: none; border-bottom: solid 1px #c0c0c0;">
                            <tr>
                                <td style="white-space: nowrap; width: 300px; color: #808080; text-align: center; vertical-align: middle; border: none;">
                                    <a href="javascript: UploadGroupLogo();" id="link_uploadGroupLogo">
                                        <img id="imgOwnerLogo" runat="server" src="/Images/OwnerLogos/!Empty-Placeholder.png" class="OwnerLogo" />
                                    </a>
                                    <br /><br />
                                    Max width allowed: 175px
                                </td>
                                <td style="white-space: nowrap; text-align: center; vertical-align: middle; border: none;">
                                    <span style="color: #808080;">GroupID:</span> <span id="spanGroupID" runat="server"></span>
                                </td>
                                <td style="white-space: nowrap; text-align: center; vertical-align: middle; border: none;">
                                    <span id="spanMembershipStats" runat="server" style="color: #808080;">
                                        Administrators: <span id="adminCount">0</span>
                                        Clients: <span id="clientCount">0</span>
                                    </span>
                                </td>
                            </tr>
                        </table>
                    </div>     
                </div>

                <div class="row">
                    <div class="large-6 medium-6 small-12 columns">
                        <div id="divGroupName" runat="server">
                            <label id="lblGroupName"><span id="spanGroupName">Group Name</span></label>
                            <input id="txtGroupName" type="text" runat="server" disabled lbl="Group Name" isrequired="true" isvalid="false" min-length="3" max-length="50" 
                                matchpattern="^([A-Za-z\d\s.,!-_#()]+)$" patterndescription="A-Z, a-z, 0-9, space, .,!-_#()" onfocus="javascript: validateFormFields(this);"  
                                onblur="javascript: validateFormFields(this);" 
                                onkeyup="javascript: validateFormFields(this);"
                                onchange="javascript: validateFormFields(this);" />
                        </div>
                    </div>
                    <div class="large-6 medium-6 small-12 columns">
                        <div id="divMacOasUrl" runat="server">
                            <label>MAC OAS Url</label>
                            <input type="text" id="txtMACOASServicesUrl" disabled runat="server" value="http://???" />
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div id="divDisabledGroupContainer">
                        <div class="large-9 medium-8 small-8 columns">
                            <div>
                                <input class="tiny button radius" runat="server" style="width: 125px;" id="btnCreateRootGroup" type="button" onclick="javascript: clearGroupUpdate();" value="New Group" />
                                <div class="show-for-small"></div>
                                <input class="tiny button radius" runat="server" style="width: 125px;" id="btnCreateSubRootGroup" disabled="disabled" type="button" onclick="javascript: addSubGroup();" value="New Sub Group" />
                                <div class="show-for-small"></div>
                                <input class="tiny button radius" runat="server" style="width: 125px;" id="btnEnableGroup" disabled="disabled" type="button" onclick="javascript: enableGroup(this);" value="Disable" />
                                <div class="show-for-small"></div>
                                <input class="tiny button radius" runat="server" style="width: 125px;" id="btnSelectedAdministrators" disabled="disabled" type="button" onclick="javascript: popupGroupAdministrators();" value="Admins" />
                            </div>
                        </div>
                        <div class="large-3 medium-4 small-4 columns">
                            <div id="divButtonControls" class="text-right small-only-text-left">
                                <input class="tiny button radius" style="width: 75px;" id="btnSaveGroup" runat="server" disabled="disabled" type="button" onclick="javascript: setPageAction();" value="Save" />
                                <input class="tiny button radius" style="width: 75px;" id="btnCancelGroup" runat="server" disabled="disabled" type="button" onclick="javascript: cancelPageAction();" value="Cancel" />
                            </div>
                        </div>
                    </div>
                </div>
                </fieldset>
            </div>
        </div>
    </div>

    <div style="padding: 0.5rem;"></div>

    <div id="copyrightContainerGroups">
        <div class="row hide-for-small">
            <div class="large-12 columns">
                <div class="copyright">
                    <script type="text/javascript">
                        <!--
                        var currentDate = new Date();
                        var year = currentDate.getFullYear();
                        document.write("&copy; " + year + " Mobile Authentication Corporation. All rights reserved.");
                        //-->
                    </script>
                </div>
            </div>
        </div>
        <div class="row show-for-small">
            <div class="large-12 columns">
                <div class="copyright">
                    <script type="text/javascript">
                        <!--
                        var currentDate = new Date();
                        var year = currentDate.getFullYear();
                        document.write("&copy; " + year + " Mobile Authentication Corporation." + "<br />" + "All rights reserved.");
                        //-->
                    </script>
                </div>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="large-12 columns">
            <!--admin popup value-->
            <asp:HiddenField ID="adminsAssigned" runat="server" Value="" />

            <asp:HiddenField ID="hiddenAA" runat="server" Value="" />
            <asp:HiddenField ID="hiddenSelectedParentGroup" runat="server" Value="" />
            <asp:HiddenField ID="hiddenSelectedIndexNumber" runat="server" Value="" />
            <asp:HiddenField ID="hiddenSelectedItemTopPosition" runat="server" Value="" />
        </div>
    </div>
                                                                                                                                                                                        
</form>