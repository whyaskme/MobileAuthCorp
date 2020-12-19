<%@ Page Title="MAC Otp System Administration"
    Language="C#"
    ValidateRequest="false" 
    MasterPageFile="~/MasterPages/AdminConsole.master"
    AutoEventWireup="true" 
    CodeFile="STSRegister.aspx.cs"
    Inherits="Admin.Tests.SecureTradingService.MacUserAppsWebTestsSecureTradingServiceStsRegister" %>

<%@ Register Src="~/UserControls/Menu-Test.ascx" TagPrefix="uc1" TagName="MenuTest" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ScriptContainer" Runat="Server">
    <script>
        $().ready(function () {
            //On tab click, set hidden value 'panelFocus'
            $('#clientTab1').click(function () {
                $('#panelFocusClients').val('clientTab1');
            });
            $('#clientTab2').click(function () {
                $('#panelFocusClients').val('clientTab2');
            });
            $('#clientTab3').click(function () {
                $('#panelFocusClients').val('clientTab3');
            });

            //Expand and position current tab on reload
            var currentTab = $('#panelFocusClients').val();
            if (currentTab == '') {

            } else if (currentTab == 'clientTab1') {
                $('#' + currentTab + '').click();
                $('html, body').animate({
                    scrollTop: ($('#scroll2').offset().top - 46)
                }, 750, 'easeOutExpo');
            } else if (currentTab == 'clientTab2') {
                $('#' + currentTab + '').click();
                $('html, body').animate({
                    scrollTop: ($('#scroll2').offset().top)
                }, 750, 'easeOutExpo');
            } else if (currentTab == 'clientTab3') {
                $('#' + currentTab + '').click();
                $('html, body').animate({
                    scrollTop: ($('#scroll2').offset().top + 46)
                }, 750, 'easeOutExpo');
            }
        });
   </script>
</asp:Content>

<asp:Content ID="BodyContainer" ContentPlaceHolderID="BodyContent" Runat="Server">

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
            <h3>Secure Trading Registration & Bill Numbers</h3>
        </div>
    </div>

    <div id="divDataForm" runat="server">
        <div class="row" id="scroll2">
            <div class="large-12 columns">
                <dl class="accordion" data-accordion="">
                    <dd>
                        <a id="clientTab1" runat="server" href="#panel1"><span id="clients">File Registration</span></a>
                        <div id="panel1" class="content">
                            <div class="row">
                                <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.5rem;">
                                    <label>Files
                                        <asp:DropDownList CssClass="chosen-select" ID="ddlTestUserFiles" runat="server" />
                                    </label>
                                </div>
                                <div class="large-6 medium-6 small-12 columns hide-for-small" style="margin-bottom: 0.5rem;">
                                    &nbsp;
                                </div>
                            </div>
                            <div style="padding: 0.25rem;"></div>
                            <div class="row">
                                <div class="large-12 columns" style="margin-bottom: 0.5rem;">
                                    <label>
                                        <asp:Button CssClass="button tiny radius" ID="btnRegisterTestUsers" runat="server" Text="Register" OnClick="btnFileRegisterTestUsers_Click" />
                                    </label>
                                </div>
                            </div>
                        </div>
                    </dd>
                    <dd>
                        <a id="clientTab2" runat="server" href="#panel2"><span id="files">Form Registration</span></a>
                        <div id="panel2" class="content">
                            <div class="row">
                                <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.5rem;">
                                    <label><asp:Label ID="Label1" runat="server" Text="Clients" />
                                        <asp:DropDownList CssClass="chosen-select" ID="ddlClient0" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlClient_Selected0" />                                
                                    </label>
                                </div>
                                <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.5rem;">
                                    <label><asp:Label ID="lbGroup0" runat="server" Text="Groups" />
                                        <asp:DropDownList CssClass="chosen-select" ID="ddlGroups0" runat="server" />                                
                                    </label>
                                </div>
                            </div>
                            <div style="padding: 0.375rem;"></div>
                            <div class="row">
                                <div class="large-6 medium-6 small-12 columns" >
                                    <label><strong>End User as</strong></label>
                                    <asp:RadioButton id="rbClientRestricted" Text="&nbsp;Restricted to Client" Checked="True" GroupName="type" runat="server"/><br />
                                    <asp:RadioButton id="rbGroupRestricted" Text="&nbsp;Restricted to Group" GroupName="type" runat="server"/><br />
                                    <asp:RadioButton id="rbOpen" Text="&nbsp;Open (any client)" GroupName="type" runat="server"/>
                                </div>
                                <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.5rem;">
                                    <label><strong>Notification Preference</strong></label>
                                    <asp:CheckBox ID="xcbText" runat="server" Text="&nbsp;Text Message" /><br />
                                    <asp:CheckBox ID="xcbEmail" runat="server" Text="&nbsp;Email" Checked="true" />
                                </div>
                            </div>
                            <div class="row">
                                <div class="large-1 medium-1 small-12 columns">
                                    <label>Prefix
                                        <asp:TextBox ID="txtNamePrefix" runat="server" />
                                    </label>
                                </div>
                                <div class="large-3 medium-3 small-12 columns">
                                    <label>First Name *
                                        <asp:TextBox ID="txtFirstName" runat="server" />
                                    </label>
                                </div>
                                <div class="large-3 medium-3 small-12 columns">
                                    <label>Middle Name
                                        <asp:TextBox ID="txtMiddleName" runat="server" />
                                    </label>
                                </div>
                                <div class="large-4 medium-4 small-12 columns">
                                    <label>Last Name *
                                        <asp:TextBox ID="txtLastName" runat="server" />
                                    </label>
                                </div>
                                <div class="large-1 medium-1 small-12 columns">
                                    <label>Suffix
                                        <asp:TextBox ID="txtNameSuffix" runat="server" />
                                    </label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="large-6 medium-6 small-12 columns">
                                    <label>Mobile Phone (Required)
                                        <asp:TextBox ID="txtMPhoneNo" runat="server" />
                                    </label>
                                </div>
                                <div class="large-6 medium-6 small-12 columns">
                                    <label>Email (Required)
                                        <asp:TextBox ID="txtEmailAdr" runat="server" />
                                    </label>
                                </div>
                            </div>

                            <div class="row">
                                <div class="large-6 medium-6 small-12 columns">
                                    <label>Unique ID
                                        <asp:TextBox ID="txtUid" runat="server" />
                                    </label>
                                </div>
                               <div class="large-6 medium-6 small-12 columns  hidden-for-medium hidden-for-small">
                                    &nbsp;
                                </div>
                            </div>

                            <div class="row">
                                <div class="large-4 medium-4 small-12 columns">
                                    <label>Address
                                        <asp:TextBox ID="txtAdr" runat="server" />
                                    </label>
                                </div>
                                <div class="large-4 medium-4 small-12 columns">
                                    <label>Address 2
                                        <asp:TextBox ID="txtAdr2" runat="server" />
                                    </label>
                                </div>
                                <div class="large-4 medium-4 small-12 columns">
                                    <label>Suite #
                                        <asp:TextBox ID="txtUnit" runat="server" />
                                    </label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="large-6 medium-6 small-12 columns">
                                    <label>City
                                        <asp:TextBox ID="txtCity" runat="server" />
                                    </label>
                                </div>
                                <div class="large-6 medium-6 small-12 columns">
                                    <label>State
                                        <asp:TextBox ID="txtState" runat="server" />
                                    </label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="large-6 medium-6 small-12 columns">
                                    <label>Zip Code
                                        <asp:TextBox ID="txtZipCode" runat="server"/>
                                    </label>
                                </div>
                                <div class="large-6 medium-6 small-12 columns">
                                    <label>Country
                                        <asp:TextBox ID="txtCountry" runat="server"/>
                                    </label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="large-6 medium-6 small-12 columns">
                                    <label>Date of Birth
                                        <asp:TextBox ID="txtDOB" runat="server" />
                                    </label>
                                </div>
                                <div class="large-6 medium-6 small-12 columns">
                                    <label>Last 4 Digits of SSN
                                        <asp:TextBox ID="txtSSN4" runat="server" />
                                    </label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="large-6 medium-6 small-12 columns">
                                    <label>Driver's License Number
                                        <asp:TextBox ID="txtDriverLic" runat="server" />
                                    </label>
                                </div>
                                <div class="large-6 medium-6 small-12 columns">
                                    <label>State Issued
                                        <asp:TextBox ID="txtDriverLicSt" runat="server" />
                                    </label>
                                </div>
                            </div>

                            <div style="padding: 0.25rem;"></div>
                            <div class="row">
                                <div class="large-12 columns">
                                    <asp:Button CssClass="button tiny radius" ID="btnRegisterEndUser" 
                                        runat="server" Text="Register" OnClick="btnRegisterEndUser_Click" tooltip="Click to register the selected end user."/>
                                    <asp:Button CssClass="button tiny radius" ID="btnValidate" 
                                        runat="server" Text="Validate" OnClick="btnValidate_Click" tooltip="Phone and Email format."/>
                                </div>
                            </div>
                        </div>
                    </dd>
                    
                     <dd>
                        <a id="clientTab3" runat="server" href="#panel3"><span id="UB">Usage / Billing</span></a>
                        <div id="panel3" class="content">
                            <div class="row">
                                <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.5rem;">
                                    <label><asp:Label ID="Label2" runat="server" Text="Clients" />
                                        <asp:DropDownList CssClass="chosen-select" ID="ddlClient1" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlClient_Selected1" />                                
                                    </label>
                                </div>
                                <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.5rem;">
                                    <label><asp:Label ID="lbGroup1" runat="server" Text="Groups" />
                                        <asp:DropDownList CssClass="chosen-select" ID="ddlGroups1" runat="server" />                                
                                    </label>
                                </div>
                            </div>
                            <div style="padding: 0.375rem;"></div>
                            <div class="row">
                                <div class="large-12 medium-12 small-12 columns" >
                                    <label>Bill Date(MM/YYYY)</label>
                                    <div style="float:left; margin-right: 150px;">
                                        <asp:TextBox runat="server" ID="txtMMYYYY" Width="100" />
                                    </div>
                                    <div style="float:left;">
                                        <asp:CheckBox runat="server" ID="cbDummyBilling" Text="Get Dummy Billing Numbers"/>
                                    </div>
                                    <div style="clear:both"></div>
                                </div>
                            </div>
<%--                            <div style="padding: 0.25rem;"></div>
                            <div class="row">
                                <div class="large-12 medium-12 small-12 columns">
                                        <asp:Button CssClass="button tiny radius" ID="btnGetBillingNumbers" runat="server" Text="Get Billing Numbers" 
                                            OnClick="btnGetBillingNumbers_Click"
                                            tooltip="Click to get usage numbers and amount of bill."/>
                                </div>
                            </div>
--%>
                        </div>
                    </dd>

                </dl>
            </div>
        </div>
    </div>
    <div style="padding: 0.5rem;"></div>
    <div class="row">
        <div class="large-12 columns">
            <asp:Button CssClass="button tiny radius" runat="server" ID="btnClearLog" Text="Clear" OnClick="btnClearLog_Click"/>
            <label><asp:Label ID="lbError" runat="server" ForeColor="Red" Text="" /></label>
        </div>
    </div>
    <div class="row">
        <div class="large-12 columns">
            <label>
                <asp:TextBox id="tbLog" runat="server" TextMode="MultiLine" Height="350" />
            </label>
        </div>
    </div>
    <input id="panelFocusClients" runat="server" type="hidden" value="" />

</asp:Content>

