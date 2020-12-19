<%@ Page Title="MAC Test Bank"
    Language="C#"
    ValidateRequest="false"
    MasterPageFile="~/MasterPages/AdminConsole.master" 
    AutoEventWireup="true" 
    CodeFile="AssignAccount.aspx.cs" 
    Inherits="MACUserApps_Web_Tests_MACTestBank_AssignAccount" %>

<%@ Register Src="~/UserControls/Menu-Test.ascx" TagPrefix="uc1" TagName="MenuTest" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ScriptContainer" Runat="Server">
    <script>
        $().ready(function () {
            //On tab click, set hidden value 'panelFocus'
            $('#Tab1').click(function () {
                $('#panelFocusUsers').val('Tab1');
            });
            $('#Tab2').click(function () {
                $('#panelFocusUsers').val('Tab2');
            });
            $('#Tab3').click(function () {
                $('#panelFocusUsers').val('Tab3');
            });

            //Expand and position current tab on reload
            var currentTab = $('#panelFocusUsers').val();
            if (currentTab == 'Tab1') {
                $('#' + currentTab + '').click();
                $('html, body').animate({
                    scrollTop: ($('#scroll2').offset().top - 30)
                }, 750, 'easeOutExpo');
            } else if (currentTab == 'Tab2') {
                $('#' + currentTab + '').click();
                $('html, body').animate({
                    scrollTop: ($('#scroll2').offset().top + 15)
                }, 750, 'easeOutExpo');
            } else if (currentTab == 'Tab3') {
                $('#' + currentTab + '').click();
                $('html, body').animate({
                    scrollTop: ($('#scroll2').offset().top + 50)
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
        <div class="large-3 medium-3 small-12 columns hidden-for-medium hidden-for-small">
            &nbsp;
        </div>
    </div>

    <div style="padding: 0.75rem;"></div>
    <div class="row">
        <div class="large-12 medium-12 small-12 columns">
            <h3>User Account Management</h3>
        </div>
    </div>

    <div class="row" ID="scroll2">
        <div class="large-12 columns">
            <dl class="accordion" data-accordion="">
                <dd>
                    <a href="#panel1" id="Tab1"><span>Account Management using a file</span></a>
                    <div id="panel1" class="content">
                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns">
                                <asp:DropDownList ID="ddlTestUserFiles1" CssClass="chosen-select" runat="server"  Width="200" />
                            </div>
                            <div class="large-6 medium-6 small-12 columns hidden-for-small hidden-for-medium">
                                &nbsp;
                            </div>
                        </div>
                        <div style="padding: 0.5rem;"></div>
                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns">
                                <asp:Button CssClass="button tiny radius" runat="server" ID="btnAssignAccount" Text="Assign Account Only" OnClick="btnUseSelectedFile1_Click" ToolTip="Create an account at the MAC Test Bank" />
                                <asp:Button CssClass="button tiny radius" runat="server" ID="btnAssignAndReg" Text="Assign Account & Register" OnClick="btnUseSelectedFile1_Click" ToolTip="Create an account at the MAC Test Bank and Register with OTP system." />
                            </div>                         
                            <div class="large-6 medium-6 small-12 columns">
                                <asp:Button CssClass="button tiny radius" runat="server" ID="btnUnregister" Text="Delete Account" OnClick="btnUseSelectedFile1_Click" ToolTip="Unregister deletes the account and unregisters the end user" />
                            </div>
                        </div>
                    </div>
                </dd>
                <dd>
                    <a href="#panel2" id="Tab2"><span>Utility or Merchant using form</span></a>
                    <div id="panel2" class="content">
                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Name
                                    <asp:TextBox ID="txtMName" runat="server" />
                                </label>
                            </div>
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Short Name
                                    <asp:TextBox ID="txtMShortName" runat="server" />
                                </label>
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Mobile Phone
                                    <asp:TextBox ID="txtMPhone" runat="server" />
                                </label>
                            </div>
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Email
                                    <asp:TextBox ID="txtMEmail" runat="server" />
                                </label>
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-12 columns">
                                <asp:Button CssClass="button tiny radius" runat="server" 
                                    ID="btnAssignMerchant" Text="Merchant" OnClick="btnAssign_Click"/>
                                <asp:Button CssClass="button tiny radius" runat="server" 
                                    ID="btnAssignUtility" Text="Utility" OnClick="btnAssign_Click"/>
                            </div>
                        </div>
                    </div>
                </dd>
                <dd>
                    <a href="#panel3" id="Tab3"><span>User Account Menagement using form</span></a>
                    <div id="panel3" class="content">
                        <div class="row">
                            <div class="large-4 medium-4 small-12 columns" style="margin-bottom: 0.125rem;">
                                <asp:DropDownList CssClass="chosen-select" ID="ddlClient" runat="server" />
                            </div>
                            <div class="large-6 medium-6 small-12 columns">
                                <label>
                                    <asp:Label ID="lbClientId" runat="server" />
                                </label>
                            </div>
                        </div>
                        <div style="padding: 0.5rem;"></div>
                            <div class="row">
                                <div class="large-4 medium-4 small-12 columns" style="margin-bottom: 0.75rem;">
                                    <label><strong>Registration Options</strong></label><br />
                                        <asp:RadioButton id="rbClientRestricted" Text="&nbsp;Restricted to Client" Checked="True" GroupName="type" runat="server"/><br />
                                        <asp:RadioButton id="rbOpen" Text="&nbsp;Open (any client)" GroupName="type" runat="server"/>
                                </div>
                                <div class="large-4 medium-4 small-12 columns" style="margin-bottom: 0.75rem;">
                                    <label><strong>Ad Pass Options</strong></label><br />
                                        <asp:RadioButton id="rdAdPassEnable" Text="&nbsp;Enable Ads" Checked="True" GroupName="AdCtrl" runat="server"/><br />
                                        <asp:RadioButton id="rdAdPassDisable" Text="&nbsp;Disable Ads" GroupName="AdCtrl" runat="server"/>
                                </div>
                                <div class="large-4 medium-4 small-12 columns" style="margin-bottom: 0.75rem;">
                                    <label><strong>Notifications Options</strong></label><br />
                                        <asp:CheckBox ID="cbText" runat="server" Text="&nbsp;Text Message" /><br />
                                        <asp:CheckBox ID="cbEmail" runat="server" Text="&nbsp;Email" Checked="true" />
                                </div>
                            </div>
                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns">
                                <label>First Name
                                    <asp:TextBox ID="txtFirstName" runat="server" />
                                </label>
                            </div>
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Last Name
                                    <asp:TextBox ID="txtLastName" runat="server" />
                                </label>
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Mobile Phone
                                    <asp:TextBox ID="txtPhoneNumber" runat="server" />
                                </label>
                            </div>
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Email
                                    <asp:TextBox ID="txtEmailAdr" runat="server" />
                                </label>
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Unique Identifier
                                    <asp:DropDownList CssClass="chosen-select" runat="server" ID="ddlUID" ToolTip="If Registering, Email is default"/>
                                </label>
                            </div>
                            <div class="large-6 medium-6 small-12 columns hidden-for-small">
                                &nbsp;
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Address
                                    <asp:TextBox ID="txtAdr" runat="server" />
                                </label>
                            </div>
                            <div class="large-6 medium-6 small-12 columns">
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
                                <label>State (Code)
                                    <asp:TextBox ID="txtState" runat="server" ToolTip="AL,AK,AZ,AZ,AR,CA, etc." />
                                </label>
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Zip Code
                                    <asp:TextBox ID="txtZipCode" runat="server" />
                                </label>
                            </div>
                            <div class="large-6 medium-6 small-12 columns hidden-for-small">
                                &nbsp;
                            </div>
                        </div>                
                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Last 4 Digits of SSN
                                    <asp:TextBox ID="txtSSN4" runat="server" />
                                </label>
                            </div>
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Date of Birth
                                    <asp:TextBox ID="txtDOB" runat="server" ToolTip="mm/dd/yyyy"/>
                                </label>
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Driver's License Number
                                    <asp:TextBox ID="txtDriverLic" runat="server"  />                        
                                </label>
                            </div>
                            <div class="large-6 medium-6 small-12 columns">
                                <label>State Issued (Code)
                                    <asp:TextBox ID="txtDriverLicSt" runat="server" ToolTip="AL,AK,AZ,AZ,AR,CA, etc." />
                                </label>
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns">
                                <asp:Button CssClass="button tiny radius" runat="server" 
                                    ID="btnAssignOnly" Text="Assign Account Only" OnClick="btnAssignUser_Click" ToolTip="Create an account at the MAC Test Bank" />
                                <asp:Button CssClass="button tiny radius" runat="server" 
                                    ID="btnRegAndAssign" Text="Assign Account & Register" OnClick="btnAssignUser_Click" ToolTip="Create an account at the MAC Test Bank and Register with OTP system." />
                            </div>
                            <div class="large-6 medium-6 small-12 columns">
                                <asp:Button CssClass="button tiny radius" runat="server" 
                                    ID="btnDeleteAccount" Text="Delete Account" OnClick="btnDeleteAccount_Click" ToolTip="Delete Account (unregister user if registered)" />
                            </div>
                        </div>
                    </div>
                </dd>
            </dl>
        </div>
    </div>

    <div style="padding: 0.5rem;"></div>
    <div class="row">
        <div class="large-12 columns">
            <asp:Button CssClass="button tiny radius" runat="server" ID="btnClearLog" Text="Clear Logs" OnClick="btnClearLog_Click"/>
            <asp:Label ID="lbError" runat="server" ForeColor="Red" />
        </div>
    </div>
    <div class="row">
        <div class="large-12 columns">
            <asp:TextBox id="tbLog" runat="server" TextMode="MultiLine" Height="300" />
        </div>
    </div>
    <div id="divHiddenFields" style="visibility: hidden;">
        <input type="hidden" id="hiddenPANList" value="" runat="server" />
        <input type="hidden" id="hiddenAccountHoldersList" value="" runat="server" />
        <input type="hidden" id="hiddenAccountNamesList" value="" runat="server" />
        <input type="hidden" id="hiddenLoginNamesList" value=""  runat="server" />
        <input type="hidden" id="hiddenBills" value="" runat="server" />
        <input id="panelFocusUsers" runat="server" type="hidden" value="" />
    </div>
    <script>
        $(document).ready(function () {
            var defaultMessage1 = $('#ddlTestUserFiles1_chosen a span').html();
            if (defaultMessage1 == 'Select File') {
                $('#ddlTestUserFiles1_chosen a span').html('Select a File To Assign');
            }
            var defaultMessage2 = $('#ddlEndUserList2_chosen a span').html();
            if (defaultMessage2 == 'Select an Option') {
                $('#ddlEndUserList2_chosen a span').html('Select User');
            }
            $('#menuTests').addClass('active');
        });
    </script>

</asp:Content>
