<%@ Page ValidateRequest="false"
    Title="MAC Test Bank"
    Language="C#"
    MasterPageFile="~/MasterPages/AdminConsole.master" 
    AutoEventWireup="true" 
    CodeFile="BillPay.aspx.cs" 
    Inherits="MACUserApps_Web_Tests_MACTestBank_BillPay" %>


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
                    scrollTop: ($('#scroll2').offset().top - 40)
                }, 750, 'easeOutExpo');
            } else if (currentTab == 'Tab2') {
                $('#' + currentTab + '').click();
                $('html, body').animate({
                    scrollTop: ($('#scroll2').offset().top + 10)
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
            <h3>Billing, Payments & Purchase</h3>
        </div>
    </div>

    <div class="row" ID="scroll2">
        <div class="large-12 columns">
                        
            <dl class="accordion" data-accordion="">
                <dd>
                    <a href="#panel1" id="Tab1"><span>Bill Pay</span></a>
                    <div id="panel1" class="content">

                            <!-- ---------- Create a Bill ----------------------- -->
                            <div style="padding: 0.5rem;"></div>
                            <div class="row">
                                <div class="large-12 columns panel radius">
                                    <div style="padding: 0.5rem;"></div>
                                    <div class="row">
                                        <div class="large-12 columns">
                                            <label>Create A Bill</label>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.25rem;">
                                            <asp:DropDownList CssClass="chosen-select" runat="server" ID="ddlUserNames1" AutoPostBack="True" OnSelectedIndexChanged="ddlUserNames1_Selected" />
                                        </div>
                                        <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.25rem;">
                                            <asp:DropDownList CssClass="chosen-select" runat="server" ID="ddlUtilities1" />
                                        </div>
                                    </div>
                                    <div style="padding: 0.25rem;"></div>
                                    <div class="row">
                                        <div class="large-6 medium-6 small-12 columns">
                                            <label>Amount of bill
                                                <asp:TextBox runat="server" ID="txtAmount" />
                                            </label>
                                        </div>
                                        <div class="large-6 medium-6 small-12 columns hide-for-small">
                                            &nbsp;
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="large-12 columns">
                                            <asp:Button CssClass="button tiny radius" runat="server" ID="btnSendBill" Text="Create" OnClick="btnCreateBill_Click" />
                                        </div>
                                    </div>
                                    <div style="padding: 0.5rem;"></div>
                                </div>
                            </div>
                            <!-- ---------- Get Bills ----------------------- -->
                            <div class="row">
                                <div class="large-12 columns panel radius">
                                    <div style="padding: 0.5rem;"></div>
                                    <div class="row">
                                        <div class="large-6 medium-6 small-12 columns">
                                            <label>Get Bills for a User
                                                <asp:DropDownList CssClass="chosen-select" runat="server" ID="ddlUserNames2" />
                                            </label>
                                        </div>
                                        <div class="large-6 medium-6 small-12 columns hide-for-small">
                                            &nbsp;
                                        </div>
                                    </div>
                                    <div style="padding: 0.5rem;"></div>
                                    <div class="row">
                                        <div class="large-12 columns">
                                            <asp:Button CssClass="button tiny radius" runat="server" ID="btnGetBills" Text="Get Bills" OnClick="btnGetBills_Click" /> 
                                        </div>
                                    </div>
                                    <div style="padding: 0.875rem;"></div>
                                    <!-- ---------- Pay Bill ----------------------- -->
                                    <div class="row">
                                        <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.25rem;">
                                            <label>Bill to pay
                                                <asp:DropDownList CssClass="chosen-select" runat="server" ID="ddlBillList" />
                                            </label>
                                        </div>
                                        <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.25rem;">
                                            <label>Enter Amount to pay
                                                <asp:TextBox runat="server" ID="txtAmountToPay" />
                                            </label>
                                        </div>
                                    </div>

                                    <div class="row">
                                        <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.25rem;">
                                            <label>Pay with this account
                                                <asp:DropDownList CssClass="chosen-select" runat="server" ID="ddlAccountNames5" />
                                            </label>
                                        </div>
                                        <div class="large-6 medium-6 small-12 columns hide-for-small">
                                            &nbsp;
                                        </div>
                                    </div>
                                    <div style="padding: 0.5rem;"></div>
                                    <div class="row">
                                        <div class="large-12 columns">
                                            <asp:Button CssClass="button tiny radius" runat="server" ID="btnDisabledPayBill" Text="Disabled Pay a Bill" OnClick="btnDisabledPayBill_Click"/> 
                                            <asp:Button CssClass="button tiny radius" runat="server" ID="btnPayBill" Text="Pay a Bill" OnClick="btnPayBill_Click" />
                                        </div>
                                    </div>
                                    <div style="padding: 0.5rem;"></div>
                                </div>
                            </div>
                        </div>

                </dd>
                <dd>
                    <a href="#panel2" id="Tab2"><span>Purchase</span></a>
                    <div id="panel2" class="content">

                        <div class="row">
                            <div class="large-12 columns">
                                <label>
                                    <asp:Button CssClass="button tiny radius" runat="server" 
                                        ID="btnDefaultPurchases" Text="Default Purchases" OnClick="btnDefaultPurchases_Click" 
                                        ToolTip="Creates 1 purchase for every merchant for each user." />
                                </label>
                            </div>
                        </div>
                        <!-- Pre-Authorize Form -->
                        <fieldset><legend>Pre-Authorize</legend>
                            <div class="row">
                                <div class="large-12 columns panel radius">
                                    <div style="padding: 0.5rem;"></div>
                                    <div class="row">
                                        <div class="large-6 medium-6 small-12 columns">
                                            <label>
                                                <asp:DropDownList CssClass="chosen-select" runat="server" 
                                                    ID="ddlUserNames4" AutoPostBack="True" OnSelectedIndexChanged="ddlUserNames4_Selected"/>  
                                            </label>
                                        </div>
                                    </div>
                                    <div style="padding: 0.5rem;"></div>
                                    <div class="row">
                                        <div class="large-6 medium-6 small-12 columns">
                                            <label>
                                                <asp:DropDownList CssClass="chosen-select" runat="server" 
                                                    ID="ddlAccountNames4" />
                                            </label>
                                        </div>
                                        <div class="large-6 medium-6 small-12 columns">
                                            <label>
                                                <asp:DropDownList CssClass="chosen-select" runat="server" 
                                                    ID="ddlAccountNumbers4" />
                                            </label>
                                        </div>
                                    </div>
                                    <div style="padding: 0.5rem;"></div>          
                                    <div class="row">
                                        <div class="large-6 medium-6 small-12 columns">
                                            <label>Pre-Auth Amount
                                                <asp:TextBox runat="server" ID="txtPreauthAmount" />
                                            </label>
                                        </div>
                                        <div class="large-6 medium-6 small-12 columns hidden-for-medium hidden-for-small">
                                            &nbsp;
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="large-6 medium-6 small-12 columns">
                                            <asp:Button CssClass="button tiny radius" runat="server" 
                                                ID="btnPreauth" Text="Preauth" OnClick="btnPreauth_Click" />
                                        </div>
                                        <div class="large-6 medium-6 small-12 columns hidden-for-small hidden-for-medium">
                                            &nbsp;
                                        </div>
                                    </div>
                                    <div style="padding: 0.5rem;"></div>
                                </div>
                            </div>
                        </fieldset>
                        <fieldset><legend>Purchase</legend>
                            <div class="row">
                                <div class="large-12 columns panel radius">
                                    <div style="padding: 0.5rem;"></div>
                                    <div class="row">
                                        <div class="large-6 medium-6 small-12 columns">
                                            <asp:DropDownList CssClass="chosen-select" runat="server" 
                                                ID="ddlUserNames3" AutoPostBack="True" OnSelectedIndexChanged="ddlUserNames3_Selected"/>  
                                        </div>
                                    </div>
                                    <div style="padding: 0.5rem;"></div>
                                    <div class="row">
                                        <div class="large-6 medium-6 small-12 columns">
                                            <asp:DropDownList CssClass="chosen-select" runat="server" 
                                                ID="ddlAccountNames3" AutoPostBack="True" OnSelectedIndexChanged="ddlAccountNames3_Selected" />
                                        </div>

                                        <div class="large-6 medium-6 small-12 columns">
                                            <asp:TextBox runat="server" ID="txtCardNumber" Text="Enter Card Number" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="large-6 medium-6 small-12 columns">
                                            <label>Purchase Amount
                                                <asp:TextBox runat="server" ID="txtPurchaseAmount" />
                                            </label>
                                        </div>
                                        <div class="large-6 medium-6 small-12 columns hidden-for-medium hidden-for-small">
                                            &nbsp;
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="large-6 medium-6 small-12 columns">
                                            <label>Merchant
                                                <asp:DropDownList CssClass="chosen-select" runat="server" 
                                                    ID="ddlMerchants" />
                                            </label>
                                        </div>

                                        <div class="large-6 medium-6 small-12 columns hidden-for-medium hidden-for-small">
                                            &nbsp;
                                        </div>
                                    </div>
                                    <div style="padding: 0.5rem;"></div>
                                    <div class="row">
                                        <div class="large-6 medium-6 small-12 columns">
                                            <asp:Button CssClass="button tiny radius" runat="server" 
                                                ID="btnMakeAPurchase" Text="Purchase" OnClick="btnMakeAPurchase_Click" />
                                        </div>
                                        <div class="large-6 medium-6 small-12 columns hidden-for-small hidden-for-medium">
                                            &nbsp;
                                        </div>
                                    </div>
                                    <div style="padding: 0.5rem;"></div>
                                </div>
                            </div>
                        </fieldset>
                    </div>
                </dd>
            </dl>
        </div>
    </div>                
    <div class="row">
        <div class="large-12 columns">
            &nbsp;&nbsp;&nbsp;
            <asp:Button CssClass="button tiny radius" runat="server" 
                ID="btnClearLog" Text="Clear Logs" OnClick="btnClearLog_Click"/>
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
</asp:Content>