<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/AdminConsole.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Admin.Users.Default" %>

<%@ Register Src="~/UserControls/ManagedResources.ascx" TagPrefix="uc1" TagName="ManagedResources" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ScriptContainer" Runat="Server">

    <script src="/Javascript/jquery-1.10.2.js"></script>

    <script>
        $().ready(function () {

            //On tab click, set hidden value 'panelFocus'
            $('#clientTab1').click(function () {
                $('#panelFocusUsers').val('clientTab1');
            });

            //Expand and position current tab on reload
            var currentTab = $('#panelFocusUsers').val();
            if (currentTab == '') {
                $('#clientTab1').click();
                $('html, body').animate({
                    scrollTop: ($('#scroll2').offset().top - 46)
                }, 750, 'easeOutExpo');
            } else if (currentTab == 'clientTab1') {
                $('#' + currentTab + '').click();
                $('html, body').animate({
                    scrollTop: ($('#scroll2').offset().top -46)
                }, 750, 'easeOutExpo');
            }
        });

        function SetPageAction()
        {
            var selectedUserIndex = $('#dlUsers')[0].selectedIndex;
            switch(selectedUserIndex)
            {
                case 0:
                    document.getElementById("hiddenAA").value = "CreateUser";
                    break;

                default:
                    document.getElementById("hiddenAA").value = "UpdateUser";
                    break;
            }

            var selectedUserRole = $('#dlRolesAssigned')[0];
            if (selectedUserRole.selectedIndex > 0) {
                document.getElementById("formMain").submit();
                return true;
            }
            else
            {
                alert("Please select a user role before continuing.");
                return false;
            }
        }

        function selectRole()
        {
            $("#dlRolesAssigned option[id=role1]").attr("selected", true);
            $("#dlRolesAssigned_chosen").trigger("chosen:updated");
        }

   </script>
</asp:Content>
<asp:Content ID="BodyContainer" ContentPlaceHolderID="BodyContent" Runat="Server">
    <div class="row">
        <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.25rem;">
            <asp:DropDownList CssClass="chosen-select" ID="dlUserRoles" runat="server" AutoPostBack="True" OnSelectedIndexChanged="dlUserRoles_SelectedIndexChanged">
                <asp:ListItem Value="000000000000000000000000">Select a Role</asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="large-6 medium-6 small-12 columns">
            <asp:DropDownList CssClass="chosen-select" ID="dlUsers" runat="server" AutoPostBack="True" Visible="true" OnSelectedIndexChanged="dlUsers_SelectedIndexChanged">
                <asp:ListItem Value="000000000000000000000000">Select a User</asp:ListItem>
            </asp:DropDownList>
        </div>
    </div>

    <div style="padding: 0.5rem;"></div>
    <div class="row">
        <div class="large-12 columns">
            <div class="alert-box success radius" id="userUpdateMessage" runat="server" style="cursor: pointer;" onclick="javascript: noDisplay();">
                User settings have been saved!
            </div>
        </div>
    </div>

    <div class="row" id="scroll2">
        <div class="large-12 columns">
            <dl class="accordion" data-accordion="">
                <dd>
                    <a href="#panel1" id="clientTab1"><span>User Registration</span></a>
                    <div id="panel1" class="content">

                        <div class="row" style="margin-bottom: 0.75rem; position: relative; top: -40px; margin-bottom: -40px;">
                            <div class="large-12 medium-12 small-12 columns" style="width: 100%; text-align: right;">
                                <a href="javascript: NavigateTopicPopup('5490bfb6ead63627d88e3e21');" id="link_help_userRegistration">Help?</a>
                            </div>
                        </div>

                        <div id="pnlRegistration" runat="server">
                            <div class="row">                            
                                <div class="large-6 medium-6 small-12 columns">
                                    <a href="#" onclick="javascript:simulateClick();">link</a>
                                    <label>User: <span id="h3NameTitle" runat="server"></span></label>
                                </div>
                                <div class="large-6 medium-6 small-12 columns">
                                    <div style="display: block;width: 1.5rem;float: left;">
                                        <label>ID:</label>
                                    </div>
                                    <div style="display: block;height: 25px; width: 300px; border: solid 0px #ff0000; float: left;position: relative;top: -12px;">                                
                                        <asp:TextBox CssClass="transparentTextBox" ID="txtUserID" runat="server" Width="100%" BorderStyle="None" Font-Bold="false"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div style="padding: 0.125rem;"></div>
                            <div class="row">                            
                                <div class="large-6 medium-6 small-12 columns">
                                    <div style="white-space: nowrap;"><label>Registered: <span style="font-size: 0.875rem;" id="spanDateRegistered" runat="server"></span></label></div>
                                </div>
                                <div class="large-6 medium-6 small-12 columns">
                                    <div style="white-space: nowrap;"><label>Last login: <span style="font-size: 0.875rem;" id="spanLastLogin" runat="server"></span></label></div>
                                </div>
                            </div>
                            <div style="padding: 0.25rem;"></div>
                            <div class="row">                            
                                <div class="large-6 medium-6 small-12 columns">
                                    <span id="span1" runat="server"><asp:CheckBox Text="Enabled" ID="chkIsApproved" runat="server" /></span>
                                </div>
                                <div class="large-6 medium-6 small-12 columns">
                                    <span id="spanIsLockedOut" runat="server"><asp:CheckBox Text="Locked" ID="chkIsLockedOut" runat="server" /></span>
                                </div>
                            </div>
                            <div class="row">                            
                                <div class="large-12 columns">
                                    <hr style="margin: 0.5rem 0 1rem;" />
                                </div>
                            </div>
                            
                            <div class="row" id="divRolesAssignedList" runat="server">   
                                <div style="padding: 0.125rem;"></div>                         
                                <div class="large-6 medium-6 small-12 columns">
                                    <div id="divRolesAssigned" runat="server">
                                        <label>Role Assignment</label>
                                        <asp:DropDownList CssClass="chosen-select" ID="dlRolesAssigned" runat="server" AutoPostBack="false">
                                            <asp:ListItem Value="000000000000000000000000">Select a Role</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="large-6 medium-6 small-12 columns">
                                    <div id="div1" runat="server" style="position: relative; top: 25px; border: solid 0px #ff0000;">
                                        <asp:CheckBox ID="chkUserReadOnly" Text="User is Read Only?" runat="server" />
                                    </div>
                                </div>
                            </div>
                            <div style="padding: 0.5rem;"></div>
                            <div class="row">                            
                                <div class="large-6 medium-6 small-12 columns"> 
                                    <label id="lblFirstName"><span id="spanFirstName">First Name</span>
                                        <input id="txtFirstName" lbl="First Name" isrequired="true" isvalid="false" min-length="3" max-length="25" 
                                            matchpattern="^([A-Za-z\d\s.,!-_#()]+)$" patterndescription="A-Z, a-z, 0-9, space, .,!-_#()" type="text" runat="server" 
                                            onblur="javascript: validateFormFields(this);" 
                                            onkeyup="javascript: validateFormFields(this);"
                                            onchange="javascript: validateFormFields(this);" />
                                    </label>                    
                                </div>
                                <div class="large-6 medium-6 small-12 columns">
                                    <label id="lblLastName"><span id="spanLastName">Last Name</span>
                                        <input id="txtLastName" lbl="Last Name" isrequired="true" isvalid="false" min-length="3" max-length="25" 
                                            matchpattern="^([A-Za-z\d\s.,!-_#()]+)$" patterndescription="A-Z, a-z, 0-9, space, .,!-_#()" type="text" runat="server" 
                                            onblur="javascript: validateFormFields(this);" 
                                            onkeyup="javascript: validateFormFields(this);"
                                            onchange="javascript: validateFormFields(this);" />
                                    </label> 
                                </div>
                            </div>
                            <div class="row">                            
                                <div class="large-6 medium-6 small-12 columns">   
                                    <label id="lblEmail"><span id="spanEmail">Email Address</span>
                                        <input id="txtEmail" lbl="Email Address" isrequired="true" isvalid="false" min-length="7" max-length="50" 
                                            matchpattern="^([a-z0-9_\.-]+)@([\da-z\.-]+)\.([a-z\.]{2,6})$" patterndescription="a valid email address" type="text" runat="server" 
                                            onblur="javascript: validateFormFields(this);" 
                                            onkeyup="javascript: validateFormFields(this);"
                                            onchange="javascript: validateFormFields(this);" />
                                    </label>                  
                                </div>
                                <div class="large-6 medium-6 small-12 columns">
                                    <label id="lblPhone"><span id="spanPhone">Phone</span>
                                        <input id="txtPhone" lbl="Phone Number" isrequired="true" isvalid="false" min-length="10" max-length="12"
                                            matchpattern="^\s*(?:\+?(\d{1,3}))?[-. (]*(\d{3})[-. )]*(\d{3})[-. ]*(\d{4})(?: *x(\d+))?\s*$" patterndescription="###-###-####" type="text" runat="server" 
                                            onblur="javascript: validateFormFields(this);" 
                                            onkeyup="javascript: validateFormFields(this);"
                                            onchange="javascript: validateFormFields(this);" />
                                    </label>
                                </div>
                            </div>
<%--                            <div class="row">                            
                                <div class="large-6 medium-6 small-12 columns">
                                    <label>Security Question</label>
                                    <asp:TextBox ID="txtSecurityQuestion" runat="server"></asp:TextBox>                            
                                </div>
                                <div class="large-6 medium-6 small-12 columns">
                                    <label>Security Answer</label>
                                    <asp:TextBox ID="txtSecurityAnswer" runat="server"></asp:TextBox>
                                </div>
                            </div>--%>
                            <div class="row">                            
                                <div class="large-6 medium-6 small-12 columns">  
                                    <label id="lblUsername"><span id="spanUsername" runat="server">Username</span>
                                        <input id="txtUsername" lbl="Username" isrequired="true" isvalid="false" min-length="7" max-length="50" 
                                            matchpattern="^([A-Za-z\d\s.,!-_#()]+)$" patterndescription="A-Z, a-z, 0-9, space, .,!-_#()" type="text" runat="server" 
                                            onblur="javascript: validateFormFields(this);" 
                                            onkeyup="javascript: validateFormFields(this);"
                                            onchange="javascript: validateFormFields(this);" />
                                    </label>                      
                                </div>
                                <div id="divPasswordControls" runat="server" class="large-6 medium-6 small-12 columns">
                                    <label id="lblPassword"><span id="spanPassword">Password</span>
                                        <input id="txtPassword" lbl="Password" isrequired="true" isvalid="false" min-length="4" max-length="50" 
                                            matchpattern="^([A-Za-z\d\s.,!-_#()]+)$" patterndescription="A-Z, a-z, 0-9, space, .,!-_#()" type="text" runat="server" 
                                            onblur="javascript: validateFormFields(this);" 
                                            onkeyup="javascript: validateFormFields(this);"
                                            onchange="javascript: validateFormFields(this);" />
                                    </label>  
                                </div>
                            </div>
                            <div class="row">
                                <div class="large-12 columns">
                                    <input class="tiny button radius" type="button" id="btnSelectedGroups" runat="server" onclick="javascript: popupGroupAssignment();" value="0 Groups Assigned" />
                                    <%--<asp:Button CssClass="tiny button radius" ID="btnRegister" runat="server" Text="Register Admin User" OnClick="btnRegister_Click" />--%>
                                    <input type="button" id="btnSaveUser" runat="server" class="tiny button radius" onclick="javascript: SetPageAction();" value="Save User" />
                                </div>
                            </div>
                        </div>
                    </div>
                </dd>
            </dl>
        </div>
    </div>

    <input id="hiddenSelectedUserId" runat="server" type="hidden" value="" />
    <input id="panelFocusUsers" runat="server" type="hidden" value="" />
    <input id="hiddenRefreshTimerPaused" type="hidden" value="false" />
    <input id="hiddenRefreshEventTypeList" type="hidden" value="true" />

    <asp:HiddenField ID="hiddenAA" runat="server" />

</asp:Content>

