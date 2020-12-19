<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/AdminConsoleBlank.master" EnableViewState="true" AutoEventWireup="true" CodeFile="ClientUsers.aspx.cs" Inherits="Admin.Users.ClientUsers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ScriptContainer" Runat="Server">
    <script>
        $().ready(function () {
        });
   </script>
</asp:Content>
<asp:Content ID="BodyContainer" ContentPlaceHolderID="BodyContent" Runat="Server">

    <div class="row">
        <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.25rem;">
            <asp:DropDownList CssClass="chosen-select" ID="dlUsers" runat="server" AutoPostBack="true" Visible="true" OnSelectedIndexChanged="dlUsers_SelectedIndexChanged">
                <asp:ListItem Value="000000000000000000000000">Select a User</asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="large-6 medium-6 small-12 columns">
            &nbsp;
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

    <div id="pnlRegistration" runat="server">
        <div class="row">                            
            <div class="large-6 medium-6 small-12 columns">
                <div style="white-space: nowrap;"><label>Registered: <span style="font-size: 0.875rem;" id="spanDateRegistered" runat="server"></span></label></div>
            </div>
            <div class="large-6 medium-6 small-12 columns">
                <div style="white-space: nowrap;"><label>Last login: <span style="font-size: 0.875rem;" id="spanLastLogin" runat="server"></span></label></div>
            </div>
        </div>
        <div class="row">                            
            <div class="large-12 columns">
                <hr style="margin: 0.5rem 0 1rem;" />
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
        <div class="row">
            <div class="large-12 columns">
                <asp:Button CssClass="tiny button radius" ID="btnRegister" runat="server" Enabled="true" Text="Register" OnClick="btnRegister_Click" />
                <asp:Button CssClass="tiny button radius" ID="btnRemove" runat="server" Enabled="false" Text="Remove" OnClick="btnRemove_Click" />
                <asp:Button CssClass="tiny button radius" ID="btnCancel" runat="server" Enabled="false" Text="Cancel" OnClick="btnCancel_Click" />
            </div>
        </div>
    </div>

    <input id="hiddenD" runat="server" type="hidden" value="" /> 

    <asp:HiddenField ID="hiddenSelectedUserId" runat="server" />
    <asp:HiddenField ID="hiddenAA" runat="server" />

</asp:Content>

