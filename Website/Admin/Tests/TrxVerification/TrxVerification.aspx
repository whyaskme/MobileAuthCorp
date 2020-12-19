<%@ Page Title="MAC Otp System Administration"
    Language="C#"
    ValidateRequest="false"
    MasterPageFile="~/MasterPages/AdminConsole.master"
    AutoEventWireup="true" 
    CodeFile="TrxVerification.aspx.cs"
    Inherits="MACUserApps.Web.Tests.TrxVerification.MacUserAppsWebTestsTrxVerificationTrxVerification" %>

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
        function handleClick() {
            var total = 0;
            var value;
            if ($('#cbHat').is(':checked')) {
                value = $('#lbHat').text();
                total = total + Number(value.replace(/[^0-9\.]+/g, ""));
            }
            if ($('#cbJacket').is(':checked')) {
                value = $('#lbJacket').text();
                total = total + Number(value.replace(/[^0-9\.]+/g, ""));
            }
            if ($('#cbShoes').is(':checked')) {
                value = $('#lbShoes').text();
                total = total + Number(value.replace(/[^0-9\.]+/g, ""));
            }
            if ($('#cbShirt').is(':checked')) {
                value = $('#lbShirt').text();
                total = total + Number(value.replace(/[^0-9\.]+/g, ""));
            }
            $('#lbTotal').html("$" + total);
        }

    </script>
    <div style="padding: 0.25rem;"></div>
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
    <div style="padding: 0.5rem;"></div>
    <div class="row">
        <div class="large-12 medium-12 small-12 columns">
            <h3>Transaction Verification & Authorization</h3>
        </div>
    </div>
    <div class="row">
        <div class="large-12 columns">
            <dl class="accordion" data-accordion="">
                <dd> <!-- Select User from File -->
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
                <dd> <!-- Enter User in form -->
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
            <div style="padding: 0.5rem;"></div>
            <div class="row">
                <div class="large-6 medium-6 small-12 columns">
                    <label><asp:Label ID="Label1" runat="server" Text="Client:" />
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
                <div class="large-12 medium-12 small-12 columns">
                    <h4>Items to purchase, Pick at least one item</h4>
                </div>
            </div>
            <div class="row">
                <div class="large-6 medium-6 small-12 columns" style="border:  1px solid; padding: 0;">
                    <asp:Table ID="t1" runat="server">
                        <asp:TableRow ID="tr1" runat="server" >
                            <asp:TableCell ID="tc1" runat="server" Width="100" >
                                <asp:CheckBox ID="cbHat" runat="server" Text="&nbsp;Hat" onclick='handleClick();' />
                            </asp:TableCell>
                            <asp:TableCell ID="TableCell1" runat="server"  HorizontalAlign="Right"  >
                                <asp:Label ID="lbHat" runat="server" Text="$7.99" />
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="TableRow1" runat="server" >
                            <asp:TableCell ID="TableCell2" runat="server" >
                                <asp:CheckBox ID="cbJacket" runat="server" Text="&nbsp;Jacket" onclick='handleClick();' />
                            </asp:TableCell>
                            <asp:TableCell ID="TableCell3" runat="server"  HorizontalAlign="Right" >
                                <asp:Label ID="lbJacket" runat="server" Text="$150.98"  />
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="TableRow2" runat="server" >
                            <asp:TableCell ID="TableCell4" runat="server" >
                                <asp:CheckBox ID="cbShoes" runat="server" Text="&nbsp;Shoes" onclick='handleClick();' />
                            </asp:TableCell>
                            <asp:TableCell ID="TableCell5" runat="server" Width="100" HorizontalAlign="Right" >
                                <asp:Label ID="lbShoes" runat="server" Text="$98.99" />
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="TableRow3" runat="server" >
                            <asp:TableCell ID="TableCell6" runat="server" >
                                <asp:CheckBox ID="cbSocks" runat="server" Text="&nbsp;Socks" onclick='handleClick();' />
                            </asp:TableCell>
                            <asp:TableCell ID="TableCell7" runat="server"  HorizontalAlign="Right" >
                                <asp:Label ID="lbSocks" runat="server" Text="$5.99" />
                            </asp:TableCell>  
                        </asp:TableRow>
                        <asp:TableRow ID="TableRow4" runat="server" >
                            <asp:TableCell ID="TableCell8" runat="server" >
                                <asp:CheckBox ID="cbShirt" runat="server" Text="&nbsp;Shirt" onclick='handleClick();' />
                            </asp:TableCell>
                            <asp:TableCell ID="TableCell9" runat="server"  HorizontalAlign="Right" >
                                <asp:Label ID="lbShirt" runat="server" Text="$14.99" />
                            </asp:TableCell>  
                        </asp:TableRow>
                        <asp:TableRow ID="trCoupon" runat="server" Visible="false" >
                                <asp:TableCell ID="TableCell14" runat="server" >
                                <asp:Label ID="lbCoupon" runat="server" Text="" />
                            </asp:TableCell>
                            <asp:TableCell ID="TableCell15" runat="server"  HorizontalAlign="Right" Font-Bold="false">
                                <asp:Label ID="lbCouponValue" runat="server" Text="$0.00" />
                            </asp:TableCell>  
                        </asp:TableRow>
                        <asp:TableRow ID="TableRow6" runat="server" >
                                <asp:TableCell ID="TableCell12" runat="server" >
                                <asp:Label ID="lbShipping" runat="server" Text="Shipping" />
                            </asp:TableCell>
                            <asp:TableCell ID="TableCell13" runat="server"  HorizontalAlign="Right" Font-Bold="false">
                                <asp:Label ID="lbShippingCost" runat="server" Text="$0.00" />
                            </asp:TableCell>  
                        </asp:TableRow>
                        <asp:TableRow ID="TableRow5" runat="server" >
                            <asp:TableCell ID="TableCell10" runat="server" >
                                <asp:Label ID="l10" runat="server" Text="Total" Font-Bold="true"/>
                            </asp:TableCell>
                            <asp:TableCell ID="TableCell11" runat="server"  HorizontalAlign="Right" Font-Bold="false" Font-Overline="true" >
                                <asp:Label ID="lbTotal" runat="server" />
                            </asp:TableCell>  
                        </asp:TableRow>
                    </asp:Table>
                </div>
                <div class="large-6 medium-6 small-12 columns hidden-for-small">
                    &nbsp;
                </div>
            </div>
            <div style="padding: 0.5rem;"></div>
            <div class="row">
                <div class="large-4 medium-4 small-12 columns">
                    <div style="float:left; position:relative; top:10px; font-size:90%; ">
                        Ad Number(1-5):&nbsp;
                    </div>
                    <div style="float:left;">
                        <asp:TextBox ID="txtAdNumber" runat="server" Width="30"/>
                    </div>
                </div>
                <div class="large-8 medium-8 small-12 columns">
                    <label>Response Format</label>
                    <asp:CheckBox runat="server" ID="cbXMLDS" Text="Return XML with Delimited Strings(Default)" Checked="true"/>
                    <asp:CheckBox runat="server" ID="cbXML" Text="Return XML"/>
                </div>
            </div>
            <div class="row">
                <div class="large-12 columns">
                    <asp:Button CssClass="button tiny radius" ID="xbtnClientManaged" runat="server" Text="Client Managed" 
                        OnClick="btnClientManagedUser_Click" tooltip="Click to use Client User to request Otp."/>
                    <asp:Button CssClass="button tiny radius" ID="xbtnRegistered" runat="server" Text="Registered" 
                        OnClick="btnRegisteredUser_Click" tooltip="Click to use open user to request Otp."/>
                </div>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="large-12 columns">
            <asp:Button CssClass="button tiny radius" runat="server" ID="btnClearLog" Text="Clear Logs" OnClick="btnClearLog_Click"/>
            <asp:Label ID="lbError" runat="server" ForeColor="Red" />
        </div>
    </div>   
    <div class="row">
        <div class="large-12 columns">
            <asp:TextBox id="tbLog" runat="server" TextMode="MultiLine" Height="350" />
        </div>
    </div>    
    <input id="panelFocusUsers" runat="server" type="hidden" value="" />
</asp:Content>
