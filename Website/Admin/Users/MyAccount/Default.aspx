<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/AdminConsole.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Admin.Users.MyAccount.Default" %>

<%@ Register Src="~/UserControls/ManagedResources.ascx" TagPrefix="uc1" TagName="ManagedResources" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ScriptContainer" Runat="Server">
    <style type="text/css">
        .highlight {
            padding:0.125rem 0.375rem;background: #248beb;color: #fff;border-radius: 3px;
        }
    </style>
    <script>
        $(document).ready(function() {
            //On tab click, set hidden value 'panelFocus'
            $('#clientTab1').click(function () {
                $('#panelFocusMyAccount').val('clientTab1');
            });
            $('#clientTab2').click(function () {
                $('#panelFocusMyAccount').val('clientTab2');
            });

            //Expand and position current tab on reload
            var currentTab = $('#panelFocusMyAccount').val();
            if (currentTab == '') {
                $('#clientTab1').click();
                $('html, body').animate({
                    scrollTop: ($('#scroll2').offset().top - 46)
                }, 750, 'easeOutExpo');
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
            }

            //Highlight 'My Account' menu item
            $('#myAccountId1').addClass('highlight');
            $('#myAccountId2').addClass('highlight');

        });
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BodyContent" Runat="Server">
    <div class="row" id="scroll2">
        <div class="large-12 columns">
            <dl class="accordion" data-accordion="">
                <dd>
                    <a href="#panel1" id="clientTab1"><span id="h3NameTitle" runat="server">My Registration</span></a>
                    <div id="panel1" class="content">
                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns">
                                <span class="title_875rem">Registration: </span><span id="spanDateRegistered" style="font-size:0.875rem;" runat="server"></span>
                            </div>
                            <div class="large-6 medium-6 small-12 columns">
                                <span class="title_875rem">Last Login: </span><span id="spanLastLogin" style="font-size:0.875rem;" runat="server"></span>
                            </div>
                        </div>
                        <div style="padding: 0.25rem;"></div>
                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns">
                                <span id="span1" runat="server"><asp:CheckBox ID="chkIsApproved" runat="server" Enabled="false" Text="Enabled" /></span>
                            </div>
                            <div class="large-6 medium-6 small-12 columns">
                                <span id="spanIsLockedOut" runat="server"><asp:CheckBox ID="chkIsLockedOut" runat="server" Enabled="false" Text="Locked out" /></span>
                            </div>
                        </div>
                        <div style="padding: .5rem;"></div>
                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns">
                                <label>First Name</label>
                                <asp:TextBox ID="txtFirstName" runat="server"></asp:TextBox>
                            </div>
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Last Name</label>
                                <asp:TextBox ID="txtLastName" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Email</label>
                                <asp:TextBox ID="txtEmail" runat="server"></asp:TextBox>
                            </div>
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Mobile Phone</label>
                                <asp:TextBox ID="txtPhone" runat="server"></asp:TextBox>
                            </div>
                        </div>
<%--                        <div class="row">
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
                                <label>Username (Read Only)</label>
                                <asp:TextBox ID="txtUsername" runat="server" Enabled="false"></asp:TextBox>
                            </div>
                            <div id="divPasswordControls" runat="server" class="large-6 medium-6 small-12 columns">
                                <label>Password</label>
                                <input id="txtPassword" type="text" runat="server" value="" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-12 columns">
                                <asp:Button ID="btnRegister" CssClass="tiny button radius" runat="server" Text="Update" OnClick="btnRegister_Click" />
                            </div>
                        </div>
                    </div>
                </dd>
<%--                <dd>
                    <a href="#panel2" id="clientTab2"><span id="Span2" runat="server">Resources</span></a>
                    <div id="panel2" class="content">
                        <div class="row">
                            <div class="large-12 columns">
                                <uc1:ManagedResources runat="server" ID="ManagedResources" />
                            </div>
                        </div>
                    </div>
                </dd>--%>
            </dl>
        </div>
    </div>

    <input id="panelFocusMyAccount" runat="server" type="hidden" value="" />
    <input id="hiddenRefreshTimerPaused" type="hidden" value="false" />
    <input id="hiddenRefreshEventTypeList" type="hidden" value="true" />

</asp:Content>



