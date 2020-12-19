<%@ Page Title="MAC Test Bank"
    Language="C#"
    ValidateRequest="false"
    MasterPageFile="~/MasterPages/AdminConsole.master"
    AutoEventWireup="true" 
    CodeFile="MACTestBank.aspx.cs" 
    Inherits="MACUserApps.Web.Tests.MACTestBank.MacUserAppsWebTestsMacTestBankMacTestBank" %>

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
            $('#Tab4').click(function () {
                $('#panelFocusUsers').val('Tab4');
            });
            $('#Tab5').click(function () {
                $('#panelFocusUsers').val('Tab5');
            });
            $('#Tab6').click(function () {
                $('#panelFocusUsers').val('Tab6');
            });
            //Expand and position current tab on reload
            var currentTab = $('#panelFocusUsers').val();
            if (currentTab == 'Tab1') {                         // Account Summary
                $('#' + currentTab + '').click();
                $('html, body').animate({
                    scrollTop: ($('#scroll2').offset().top - 40)
                }, 750, 'easeOutExpo');
            } else if (currentTab == 'Tab2') {                  // Account Balance
                $('#' + currentTab + '').click();
                $('html, body').animate({
                    scrollTop: ($('#scroll2').offset().top + 20)
                }, 750, 'easeOutExpo');
            } else if (currentTab == 'Tab3') {                 // Account Adjustment
                $('#' + currentTab + '').click();
                $('html, body').animate({
                    scrollTop: ($('#scroll2').offset().top + 65)
                }, 750, 'easeOutExpo');
            } else if (currentTab == 'Tab4') {                  // Transfer Funds
                $('#' + currentTab + '').click();
                $('html, body').animate({
                    scrollTop: ($('#scroll2').offset().top + 110)
                }, 750, 'easeOutExpo');
            } else if (currentTab == 'Tab5') {                 // Login Information
                $('#' + currentTab + '').click();
                $('html, body').animate({
                    scrollTop: ($('#scroll2').offset().top + 145)
                }, 750, 'easeOutExpo');
            } else if (currentTab == 'Tab6') {                 // Client Mgmt
                $('#' + currentTab + '').click();
                $('html, body').animate({
                    scrollTop: ($('#scroll2').offset().top + 155)
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
            <h3>Bank Management</h3>
        </div>
    </div>

    <div class="row" runat="server" ID="divBank">
        <div class="large-12 columns">
            <fieldset><legend></legend>
                <div class="row">
                    <div class="large-6 medium-6 small-12 columns">
                        <asp:Label ID="Label1" runat="server" Text="Total Accounts:" />
                        <asp:Label ID="lbTotalNumberOfAccounts" runat="server" Text="0" />
                    </div>
                    <div class="large-6 medium-6 small-12 columns">
                        <asp:Label ID="Label2" runat="server" Text="Accounts Assigned:" />
                        <asp:Label ID="lbNumberOfAccountsAssigned" runat="server" Text="0" />
                    </div>
                </div>
            </fieldset>
        </div>
    </div>

    <div class="row" ID="scroll2">
        <div class="large-12 columns">
            <dl class="accordion" data-accordion="">
                
<!-- Tab1 Account Management -->
                <dd>
                    <a href="#panel1" id="Tab1"><span>Account Management</span></a>
                    <div id="panel1" class="content">

                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Account Holder
                                    <asp:DropDownList CssClass="chosen-select" runat="server" 
                                        ID="ddlAccountHolderNames1" />
                                </label>
                            </div>
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Primary Account Number (PAN)
                                    <asp:DropDownList CssClass="chosen-select" runat="server" 
                                        ID="ddlPanList1" />
                                </label>
                            </div>
                        </div>
                        <div style="padding: 0.5rem;"></div>
                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns">
                                <asp:Button CssClass="button tiny radius" runat="server" ID="btnAccountDetails" Text="Details" OnClick="btnAccountDetails_Click"/>
                                <asp:Button CssClass="button tiny radius" runat="server" ID="btnAccountLog" Text="Log" OnClick="btnAccountLog_Click"/>
                            </div>                         
                            <div class="large-6 medium-6 small-12 columns">
                                <asp:Button CssClass="button tiny radius" runat="server" ID="btnDeleteAccount" Text="Delete Account" OnClick="btnDeleteAccount_Click" ToolTip="Delete Account, deletes the account and unregisters the end user" />
                            </div>
                        </div>
                    </div>
                </dd>
                
<!-- Tab2 Account Balance -->
                <dd>
                    <a href="#panel2" id="Tab2"><span>Account Balance</span></a>
                    <div id="panel2" class="content">
                                              
                       <div class="row">
                        <div class="large-6 medium-6 small-12 columns" style="padding-bottom: 0.75rem;">
                            <label>Account Holder
                                <asp:DropDownList CssClass="chosen-select" runat="server" ID="ddlAccountHolderNames3" />
                            </label>
                        </div>
                        <div class="large-6 medium-6 small-12 columns" style="padding-bottom: 0.75rem;">
                            <label>Primary Account Number (PAN)
                                <asp:DropDownList CssClass="chosen-select" runat="server" ID="ddlPanList3" />
                            </label>
                        </div>
                    </div>
                    <div class="row">
                        <div class="large-6 medium-6 small-12 columns" style="padding-bottom: 0.75rem;">
                            <label>Account Name
                                <asp:DropDownList CssClass="chosen-select" runat="server" ID="ddlAccountNames3" />
                            </label>
                        </div>
                    </div>
                    <div style="padding: 0.5rem;"></div>
                        <div class="row">
                            <div class="large-12 columns">
                                <asp:Button CssClass="button tiny radius" runat="server" ID="btnGetAccountBalance" Text="Get" OnClick="btnGetAccountBalance_Click"/>
                            </div>
                        </div>
                    </div>
                </dd>

<!-- Tab3 Account Adjustment -->
                <dd>
                    <a href="#panel3" id="Tab3"><span>Account Adjustment</span></a>
                    <div id="panel3" class="content">

                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns" style="padding-bottom: 0.75rem;">
                                <label>Account Holder
                                    <asp:DropDownList CssClass="chosen-select" runat="server" ID="ddlAccountHolderNames2" />
                                </label>
                            </div>
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Primary Account Number (PAN)
                                    <asp:DropDownList CssClass="chosen-select" runat="server" ID="ddlPanList2" />
                                </label>
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns" style="padding-bottom: 0.75rem;">
                                <label>Account Name
                                    <asp:DropDownList CssClass="chosen-select" runat="server" ID="ddlAccountNames" />
                                </label>
                            </div>
                            <div class="large-6 medium-6 small-12 columns hide-for-small">
                                &nbsp;
                            </div>
                        </div>
                        <div style="padding: 0.5rem;"></div>
                        <div class="row">
                            <div class="large-12 columns">
                                <asp:Button CssClass="button tiny radius" runat="server" ID="btnCredit" Text="Credit" ToolTip="Deposit" OnClick="btnAdjustAccount_Click"/>
                                <asp:Button CssClass="button tiny radius" runat="server" ID="btnDebit" Text="Debit" ToolTip="Withdrawl" OnClick="btnAdjustAccount_Click"/>
                            </div>
                        </div>
                    </div>
                </dd>

<!-- Tab4 Transfer Funds -->
                <dd>
                    <a href="#panel4" id="Tab4"><span>Transfer Funds</span></a>
                    <div id="panel4" class="content">

                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Amount
                                    <asp:TextBox ID="txtAmountToMove" runat="server" />
                                </label>
                            </div>
                            <div class="large-6 medium-6 small-12 columns hide-for-small">
                                &nbsp;
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-12 columns panel radius" style="padding:0.5rem 1rem 0.25rem">
                                <div class="row">
                                    <div class="large-6 medium-6 small-12 columns" style="padding-bottom: 0.75rem;">
                                        <label>From Account Holder
                                            <asp:DropDownList CssClass="chosen-select" runat="server" 
                                                ID="ddlFromAccountHolders" AutoPostBack="true" OnSelectedIndexChanged="ddlFromAccountHolders_Changed"/>
                                        </label>
                                    </div>
                                    <div class="large-6 medium-6 small-12 columns hidden-for-small hidden-for-medium">
                                        &nbsp;
                                    </div>
                                </div>                    
                                <div class="row">
                                    <div class="large-6 medium-6 small-12 columns" style="padding-bottom: 0.75rem;">
                                        <label>From Account Name
                                            <asp:DropDownList CssClass="chosen-select" runat="server" 
                                                ID="ddlFromAccountNames" />
                                        </label>
                                    </div>
                                    <div class="large-6 medium-6 small-12 columns">
                                        <label>From Account Number                                                        
                                            <asp:DropDownList CssClass="chosen-select" runat="server" 
                                                ID="ddlFromAccountNo" />
                                        </label>
                                    </div>
                                </div>
                                <div style="padding:0.25rem;"></div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-12 columns panel radius" style="padding:0.5rem 1rem 0.25rem">
                                <div class="row">
                                    <div class="large-6 medium-6 small-12 columns" style="padding-bottom: 0.75rem;">
                                        <label>To Account Holder
                                            <asp:DropDownList CssClass="chosen-select" runat="server" 
                                                ID="ddlToAccountHolders" AutoPostBack="true" OnSelectedIndexChanged="ddlToAccountHolders_Changed"/>
                                        </label>
                                    </div>
                                    <div class="large-6 medium-6 small-12 columns hidden-for-medium hidden-for-small">
                                        &nbsp;
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="large-6 medium-6 small-12 columns" style="padding-bottom: 0.75rem;">
                                        <label>To Account Name                                                        
                                            <asp:DropDownList CssClass="chosen-select" runat="server" 
                                                ID="ddlToAccountNames" />
                                        </label>
                                    </div>
                                    <div class="large-6 medium-6 small-12 columns">
                                        <label>To Account Number                                                        
                                            <asp:DropDownList CssClass="chosen-select" runat="server" 
                                                    ID="ddlToAccountNo" />
                                        </label>
                                    </div>

                                </div>
                                <div style="padding:0.25rem;"></div>
                            </div>

                        </div>
                        <div class="row">
                            <div class="large-12 columns">
                                <asp:Button CssClass="button tiny radius" runat="server" 
                                    ID="btnMoveFunds" Text="Transfer" OnClick="btnMoveFunds_Click"/>
                            </div>
                        </div>
                    </div>
                </dd>

<!-- Tab5 Login Information -->
                <dd>
                    <a href="#panel5" id="Tab5"><span>Login Information</span></a>
                    <div id="panel5" class="content">
                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Enter Login Name
                                    <asp:TextBox ID="txtLoginName" runat="server" />
                                </label>
                            </div>
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Select Login Name
                                    <asp:DropDownList CssClass="chosen-select" runat="server" ID="ddlLoginNames" AutoPostBack="true" OnSelectedIndexChanged="ddlLoginNames_Changed"/>   
                                </label>
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns">
                                <asp:Button CssClass="button tiny radius" runat="server" ID="btnValidateLoginName" Text="Validate Login Name" OnClick="btnValidateLoginName_Click"/>
                            </div>
                            <div class="large-6 medium-6 small-12 columns">
                                <asp:Button CssClass="button tiny radius" runat="server" ID="btnDeleteAccount1" Text="Delete Account using Login" OnClick="btnDeleteAccount1_Click" 
                                    ToolTip="Delete Account, deletes the account and unregisters the end user" />
                            </div>
                        </div>
                    </div>
                </dd>

<!-- Tab6 Client Mgmt -->
                <dd>
                    <a href="#panel6" id="Tab6"><span>Client Mgmt</span></a>
                    <div id="panel6" class="content">
                        
                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Client Name
                                    <asp:TextBox ID="txtCName" runat="server" />
                                </label>
                            </div>
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Client Id
                                    <asp:TextBox ID="txtCid" runat="server" />
                                </label>
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns">
                                <asp:Button CssClass="button tiny radius" runat="server" ID="btnGetClientIdUsingName" Text="Get Client Id using name" OnClick="btnGetClientIdUsingName_Click"/>
                            </div>
                            <div class="large-6 medium-6 small-12 columns">
                                <asp:Button CssClass="button tiny radius" runat="server" ID="btnGetClientNameUsingId" Text="Get Client Name using Id" OnClick="btnGetClientNameUsingId_Click"/>
                            </div>
                        </div>
                    </div>
                </dd>
            </dl>
        </div>
    </div>
    <div style="padding: 0.75rem;"></div>
    <div class="row">
        <div class="large-12 columns">
            <asp:Button CssClass="button tiny radius" runat="server" ID="btnClearLog" Text="Clear Log Window" OnClick="btnClearLog_Click"/>
            <asp:Label ID="lbError" runat="server" ForeColor="Red" Text="" />
        </div>
    </div>
    <div class="row">
        <div class="large-12 columns">
            <asp:TextBox id="tbLog" runat="server" TextMode="MultiLine" Height="350" />
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
            $('#menuTests').addClass('active');
        });
    </script>
    
</asp:Content>
