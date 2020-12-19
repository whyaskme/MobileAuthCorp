<%@ Page Title="MAC Otp System Administration"
    Language="C#"
    ValidateRequest="false"
    MasterPageFile="~/MasterPages/AdminConsole.master"
    AutoEventWireup="true" 
    CodeFile="SendMessage.aspx.cs"
    Inherits="MACUserApps.Web.Tests.SendMessage.MacUserAppsWebTestsSendMessageSendMessage" %>

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
            <h3>Send Message</h3>
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
                                    <asp:DropDownList CssClass="chosen-select" ID="ddlTestUserFiles" runat="server" 
                                        AutoPostBack="true" OnSelectedIndexChanged="ddlTestUserFiles_Selected" />
                                </label>
                            </div>
                            <div class="large-4 medium-4 small-12 columns">
                                <label>Users
                                    <asp:DropDownList CssClass="chosen-select" ID="ddlEndUserList" runat="server" 
                                        AutoPostBack="true" OnSelectedIndexChanged="ddlEndUserList_Selected"/>
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
    <div class="row">
        <div class="large-12 columns">
            <div class="row">
                <div class="large-6 medium-6 small-12 columns">
                    <label>Clients (Required)
                        <asp:DropDownList CssClass="chosen-select" ID="ddlClient" runat="server" 
                            AutoPostBack="true" OnSelectedIndexChanged="ddlClient_Selected" />
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
                <div class="large-12 columns">
                    <label>Message</label>
                    <asp:TextBox id="tbMessage" runat="server" TextMode="MultiLine" Height="150"/>
                </div>
            </div>
            <div class="row">
                <div class="large-12 medium-12 small-12 columns">
                    <label>Response Format</label>
                    <asp:CheckBox runat="server" ID="cbXMLDS" Text="Return XML with Delimited Strings(Default)" Checked="true"/>
                    <asp:CheckBox runat="server" ID="cbXML" Text="Return XML"/>
                </div>
            </div>  

            <div class="row">
                <div class="large-12 columns">
                    <asp:Button CssClass="button tiny radius" ID="btnClientManagedUser" runat="server" Text="Client Managed" OnClick="btnClientManagedUser_Click" tooltip="Click to use Client User to request Otp."/>        
                    <asp:Button CssClass="button tiny radius" ID="btnRegisteredUser" runat="server" Text="Registered" OnClick="btnRegisteredUser_Click" tooltip="Click to use open user to request Otp."/>
                </div>
            </div>
        </div>
    </div>
    <div style="padding: 0.25rem;"></div>
    <div class="row">
        <div class="large-12 columns">
            <asp:Button CssClass="button tiny radius" runat="server" ID="btnClearLog" Text="Clear" OnClick="btnClearLog_Click"/>
            <asp:Label ID="lbError" runat="server" Font-Bold="true" ForeColor="Red" />
        </div>
    </div>
    <div class="row">
        <div class="large-12 columns">
            <asp:TextBox id="tbLog" runat="server" TextMode="MultiLine" Height="350" />
        </div>
    </div>
    <input id="panelFocusUsers" runat="server" type="hidden" value="" />
</asp:Content>
