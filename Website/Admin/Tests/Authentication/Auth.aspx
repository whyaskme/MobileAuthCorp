<%@ Page Title="MAC Otp System Administration"
    Language="C#"
    ValidateRequest="false"
    MasterPageFile="~/MasterPages/AdminConsole.master" 
    AutoEventWireup="true"
    CodeFile="Auth.aspx.cs"
    Inherits="MACUserApps.Web.Tests.Authentication.MacUserAppsWebTestsAuthenticationDefault" %>

<%@ Register Src="~/UserControls/Menu-Test.ascx" TagPrefix="uc1" TagName="MenuTest" %>

<asp:Content ID="BodyContainer" ContentPlaceHolderID="BodyContent" Runat="Server">
    <script>
        $().ready(function () {
            //On tab click, set hidden value 'panelFocus'
            $('#UseTextFilesTab').click(function () {
                $('#panelFocusUsers').val('UseTextFilesTab');
            });
            $('#UseFormTab').click(function () {
                $('#panelFocusUsers').val('UseFormTab');
            });
            //Expand and position current tab on reload
            var currentTab = $('#panelFocusUsers').val();
            if (currentTab == 'UseTextFilesTab') {               // Use files
                $('#' + currentTab + '').click();
                $('html, body').animate({
                    scrollTop: ($('#scroll2').offset().top - 44)
                }, 750, 'easeOutExpo');
            } else if (currentTab == 'UseFormTab') {        // Use form
                $('#' + currentTab + '').click();
                $('html, body').animate({
                    scrollTop: ($('#scroll2').offset().top)
                }, 750, 'easeOutExpo');
            }

            $('#cbAdPassDisable').click(function () {
                if ($('#cbAdPassDisable').is(':checked')) {
                    $('#cbAdPassEnable').prop('checked', false);
                }
            });
            $('#cbAdPassEnable').click(function () {
                if ($('#cbAdPassEnable').is(':checked')) {
                    $('#cbAdPassDisable').prop('checked', false);
                }
            });

            $('#cbXMLDS').click(function () {
                if ($('#cbXMLDS').is(':checked')) {
                    $('#cbXML').prop('checked', false);
                } else {
                    $('#cbXMLDS').prop('checked', true);
                }
            });
            $('#cbXML').click(function () {
                if ($('#cbXML').is(':checked')) {
                    $('#cbXMLDS').prop('checked', false);
                } else {
                    $('#cbXMLDS').prop('checked', true);
                }
            });
        });
    </script>   

    <div class="row">
        <div class="large-3 medium-3 small-12 columns hide-for-small">
            &nbsp;
        </div>
        <div class="large-6 medium-6 small-12 columns">
            <uc1:MenuTest runat="server" ID="MenuTest" />
        </div>
        <div class="large-3 medium-3 small-12 columns hide-for-small">
            &nbsp;
        </div>
    </div>

    <div style="padding: 0.75rem;"></div>
    <div class="row">
        <div class="large-12 medium-12 small-12 columns">
            <h3>End User Authentication</h3>
        </div>
    </div>

    <div class="row" ID="scroll2">
        <div class="large-12 columns">
            
            <dl class="accordion" data-accordion="">
                <dd>
                    <a href="#panel1" id="UseTextFilesTab"><span>Use Text Files</span></a>
                    <div id="panel1" class="content">
                        <div class="row">
                            <div class="large-4 medium-4 small-12 columns">
                                <label>Files
                                    <asp:DropDownList CssClass="chosen-select" ID="ddlTestUserFiles" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlTestUserFiles_Selected" />
                                </label>
                            </div>
                            <div class="large-4 medium-4 small-12 columns">
                                <label>Users
                                    <asp:DropDownList CssClass="chosen-select" ID="ddlEndUserList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlEndUserList_Selected"/>
                                </label>
                            </div>
                            <div class="large-4 medium-4 small-12 columns">
                                <label>Using Secure Trading UserId</label>
                                    <asp:CheckBox ID="cbUsingSTSUserId" runat="server" Text="Check for yes: "/>
                            </div>
                        </div>
                    </div>  
                </dd>
                <dd>
                    <a href="#panel2" id="UseFormTab"><span>Use Form</span></a>
                    <div id="panel2" class="content">
                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns">
                                <label>First Name
                                    <asp:TextBox ID="txtFirstName" runat="server" />
                                </label>
                            </div>
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Last Name (Required if registered)
                                    <asp:TextBox ID="txtLastName" runat="server" />
                                </label>
                            </div>
                        </div>
                         <div class="row">
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Mobile Phone (Required if client managed)
                                    <asp:TextBox ID="txtMPhoneNo" runat="server" />
                                </label>
                            </div>
                            <div class="large-6 medium-6 hidden-for-small columns ">
                                &nbsp;
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-5 medium-5 small-12 columns">
                                <label>Unique Id / Email (Required if client managed)
                                    <asp:TextBox ID="txtUidEmail" runat="server" />
                                </label>
                            </div>
                            <div class="large-2 medium-2 small-12 columns" align="center">
                                <label>&nbsp;</label>
                                <asp:Label runat="server" Text="- OR -" ForeColor="Gray"/>
                            </div>
                            <div class="large-5 medium-5 small-12 columns">
                                <label>Client Generated Unique Id (aka STS)
                                    <asp:TextBox ID="txtSTSUserId" runat="server" />
                                </label>
                            </div>
                        </div>
                    </div>  
                </dd>
            </dl>
        </div>                                       
    </div>
    <div style="padding: 0.5rem;"></div>
    <div class="row">
        <div class="large-6 medium-6 small-12 columns">
            <label><strong>Clients (Required)</strong>
                <asp:DropDownList CssClass="chosen-select" ID="ddlClient" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlClient_Selected" />
            </label>
        </div>
        <div class="large-6 medium-6 small-12 columns">
            <label><asp:Label ID="lbGroup" runat="server" Text="Groups" Font-Bold="true" Visible="false" />
                <asp:DropDownList CssClass="chosen-select" ID="ddlGroups" runat="server"  Visible="false" />
            </label>
        </div>
    </div>
    <div style="padding: 0.5rem;"></div>
    <div class="row">
        <div class="large-6 medium-6 small-12 columns">
            <label><strong>Force Error on request</strong></label>
            <asp:RadioButton ID="rbNone" runat="server" Text="None" GroupName="ForcedErrors" Checked="true" />
            <asp:RadioButton ID="rbNoCID" runat="server" Text="No CID" GroupName="ForcedErrors" />
            <asp:RadioButton ID="rbInvalidCID" runat="server" Text="Inv CID" GroupName="ForcedErrors" />
            <asp:RadioButton ID="rbInvalidGroupId" runat="server" Text="Inv Group Id" GroupName="ForcedErrors" />
            <asp:RadioButton ID="rbNoUserId" runat="server" Text="No User Id" GroupName="ForcedErrors" />
        </div>

        <div class="large-6 medium-6 small-12 columns">              
            <label><strong>Client Managed Requests Only</strong></label>
            <asp:RadioButton ID="rbNoPhone" runat="server" Text="No Phone" GroupName="ForcedErrors" />
            <asp:RadioButton ID="rbInvPhone" runat="server" Text="Invalid Phone" GroupName="ForcedErrors" />
            <asp:RadioButton ID="rbNoEmail" runat="server" Text="No Email" GroupName="ForcedErrors" />
            <asp:RadioButton ID="rbInvalidEmail" runat="server" Text="Invalid Email" GroupName="ForcedErrors" />
        </div>
    </div>

    <div style="padding: 0.5rem;"></div>
    <div class="row">
        <div class="large-7 medium-7 small-12 columns">
            <label><strong>Ad Specifics</strong></label>
            <div style="float:left; position:relative; top:10px; font-size:90%; ">
                Ad Number(1-5):
            </div>
            <div style="float:left;">
                <asp:TextBox ID="txtAdNumber" runat="server" Width="30" 
                    ToolTip="Select ad using Ad Number, this overrides Special Keywords."/>
            </div>
            <div style="float:left; position:relative; top:10px; font-size:90%; ">
                &nbsp;&nbsp;KeyWords:
            </div>
            <div style="float:left;">
                <asp:TextBox ID="txtKeyWords" runat="server" Width="150"
                    ToolTip="Select ad using Special Keywords."/>
            </div>
            <label>&nbsp;&nbsp;<strong>Force End User Options</strong></label>
            <div style="float:left; position:relative; top:10px; font-size:90%; ">
                &nbsp;&nbsp;Opt-out:
                <asp:CheckBox ID="cbAdPassDisable" runat="server" ToolTip="Check to set AdDisable in request."/>
                &nbsp;&nbsp;Opt-in:
                <asp:CheckBox ID="cbAdPassEnable" runat="server" ToolTip="Check to set AdEnable in request."/>
            </div>
        </div>
        <div class="large-5 medium-5 small-12 columns">
            <label><strong>Response Format</strong></label>
            <asp:CheckBox runat="server" ID="cbXMLDS" Text="XML with Delimited Strings(Default)" Checked="true" ToolTip="Check to return delimited strings in XML elements." />
            <asp:CheckBox runat="server" ID="cbXML" Text="XML" ToolTip="Check to return details in XML elements." />
        </div>
    </div>

    <div class="row">
        <div class="large-12 columns">
            <asp:Button CssClass="button tiny radius" ID="btnRegistered" runat="server" Text="Registered" OnClick="btnRegisteredUser_Click" tooltip="Click to use open user to request Otp."/>
            &nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button CssClass="button tiny radius" ID="btnClientManaged" runat="server" Text="Client Managed" OnClick="btnClientManagedUser_Click" tooltip="Click to use Client User to request Otp."/>
        </div>
    </div>
    <div class="row">
        <div class="large-12 columns">
            <asp:Label ID="lbTrxDetails" runat="server" Visible="false" />
        </div>
    </div>
    <div class="row">
        <div class="large-12 columns">
            <asp:Button CssClass="button tiny radius" runat="server" ID="btnClearLog" Text="Clear Logs" OnClick="btnClearLog_Click"/>
            <asp:Label ID="lbError" runat="server" Font-Bold="true" ForeColor="Red" />
        </div>
    </div>
    <div class="row">
        <div class="large-12 columns">
            <asp:TextBox id="tbLog" runat="server" TextMode="MultiLine" Height="300" />
        </div>
    </div>

    <input id="panelFocusUsers" runat="server" type="hidden" value="" />
</asp:Content>
