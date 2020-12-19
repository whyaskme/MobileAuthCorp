
<%@ Page Title="MAC Otp System Administration" Language="C#" 
    MasterPageFile="~/MasterPages/AdminConsole.master" 
    AutoEventWireup="true" 
    CodeFile="ClientTests.aspx.cs" 
    Inherits="MACUserApps.Web.Tests.Client.MacUserAppsWebTestsclientclientTests" 
    validateRequest="false" %>

<%@ Register Src="~/UserControls/Menu-Test.ascx" TagPrefix="uc1" TagName="MenuTest" %>

<asp:Content ID="BodyContainer" ContentPlaceHolderID="BodyContent" Runat="Server">
    <script>
        $().ready(function () {
            //On tab click, set hidden value 'panelFocus'
            $('#IpFilterTab').click(function () {
                $('#panelFocusUsers').val('IpFilterTab');
            });
            $('#UseFormTab').click(function () {
                $('#panelFocusUsers').val('ClientMgmtTab');
            });
            //Expand and position current tab on reload
            var currentTab = $('#panelFocusUsers').val();
            if (currentTab == 'IpFilterTab') {               // Use files
                $('#' + currentTab + '').click();
                $('html, body').animate({
                    scrollTop: ($('#scroll2').offset().top - 44)
                }, 750, 'easeOutExpo');
            } else if (currentTab == 'ClientMgmtTab') {        // Use form
                $('#' + currentTab + '').click();
                $('html, body').animate({
                    scrollTop: ($('#scroll2').offset().top)
                }, 750, 'easeOutExpo');
            }
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
            <h3>Client</h3>
        </div>
    </div>

    <div class="row" ID="scroll2">
        <div class="large-12 columns">
            <dl class="accordion" data-accordion="">
                <dd>
                    <a href="#panel1" id="IpFilterTab"><span>IP Filtering</span></a>
                    <div id="panel1" class="content">
                        
                        <div>
        <asp:Label ID="Label2" runat="server" Text="Client:" />
        &nbsp;
        <asp:DropDownList ID="ddlClient" runat="server" Width="50%" AutoPostBack="true" OnSelectedIndexChanged="ddlClient_Selected" />
        &nbsp;&nbsp;
        <asp:Label ID="Label3" runat="server" Text="Groups:" Font-Bold="true" Visible="false" />
        &nbsp;
        <asp:DropDownList ID="ddlGroups" runat="server"  Visible="false" />
        127.0.0.1&nbsp;192.168.0.5
        <br />
        range&nbsp;192.168.0.0-10
        <br />
        multiple&nbsp;Ip&nbsp;192.168.0.5|1.1.1.1|127.0.0.1
        <br />
        range&nbsp;and&nbsp;Ip&nbsp;192.168.0.0-10|127.0.0.1
        <br />
        <asp:TextBox runat="server" ID="TextBox1" Width="25%" />
        <asp:TextBox runat="server" ID="TextBox2" Width="70%" />
        <asp:Button ID="Button2" runat="server" Text="Check Ip" OnClick="btnCheckIp_Click" />
                        </div>
                    </div>  
                </dd>
                 <dd>
                    <a href="#panel2" id="ClientMgmtTab"><span>Client Management</span></a>
                    <div id="panel2" class="content">
                    <div>  
        <br />
        <asp:Button ID="btnDisableClient" runat="server" Text="Disable" OnClick="btnDisableClient_Click" />
        &nbsp;&nbsp;
        <asp:Button ID="btnEnableClient" runat="server" Text="Enable" OnClick="btnEnableClient_Click" />
        &nbsp;&nbsp;
        <asp:Button ID="btnDeleteClient" runat="server" Text="Delete" OnClick="btnDeleteClient_Click" />
        &nbsp;&nbsp;
        <asp:Button ID="btnShowClientData" runat="server" Text="Show Cliect Data" OnClick="btnShowClientData_Click" />
        <br /><br />

                    </div>
                    </div>   
                </dd>
            </dl>
        </div>
    </div>
    <div>
        <asp:Label ID="lbError" runat="server" Font-Bold="true" ForeColor="Red" Text="Test" />
        <br />
        <asp:TextBox id="tbLog" runat="server" TextMode="MultiLine" Height="395" Width="100%" />
    </div>
        <br />
    <div id="divClientData">
        <asp:TextBox id="tbClientData" runat="server" TextMode="MultiLine" Height="98%" Width="100%" />
    </div>
</asp:Content>
