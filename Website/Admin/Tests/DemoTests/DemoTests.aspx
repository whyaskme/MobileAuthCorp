<%@ Page Title="MAC Otp System Administration"
    Language="C#"
    ValidateRequest="false"
    MasterPageFile="~/MasterPages/AdminConsole.master" 
    AutoEventWireup="true"
    CodeFile="DemoTests.aspx.cs" 
    Inherits="Admin_Tests_DemoTests_DemoTests" %>

<%@ Register Src="~/UserControls/Menu-Test.ascx" TagPrefix="uc1" TagName="MenuTest" %>

<asp:Content ID="BodyContainer" ContentPlaceHolderID="BodyContent" Runat="Server">
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
    <div style="padding: 0.25rem;"></div>

    <div class="row">
        <div class="large-12 columns">
            <fieldset><legend>Call Demo Registration</legend>
                  <div class="row">
                    <div class="large-8 medium-8 small-12 columns" style="margin-bottom: 0.125rem;">
                        <label>Demo URL</label>
                        <asp:TextBox ID="txtDemoUrl" runat="server" />
                    </div>
                    <div class="large-4 medium-4 small-12 columns hidden-for-small hidden-for-medium">
                        &nbsp;
                    </div>
                </div>              

                <div class="row">
                    <div class="large-4 medium-4 small-12 columns" style="margin-bottom: 0.125rem;">
                        <asp:DropDownList CssClass="chosen-select" ID="ddlClient" runat="server" />
                    </div>
                    <div class="large-6 medium-6 small-12 columns hidden-for-small hidden-for-medium">
                        &nbsp;
                    </div>
                </div>
                <div style="padding: 0.25rem;"></div>
                <div class="row">
                    <div class="large-12 columns">
                        <!-- Note: The ID of the following buttons are used in query string whne the DemoRegistration.aspx is called (do not change) -->
                        <asp:Button CssClass="button tiny radius" runat="server" ID="btnDemoReg" Text="MAC Demo Reg Form" OnClick="btnRegDemo_Click"/>
                        <asp:Button CssClass="button tiny radius" runat="server" ID="btnDemoCfg" Text="MAC Demo Reg Config" OnClick="btnRegDemo_Click"/>
                    </div>
                </div>
            </fieldset>
        </div>
    </div>
    <div style="padding: 0.25rem;"></div>
    <div class="row">
        <div class="large-12 columns">
            <div class="row">
                <asp:Button CssClass="button tiny radius" runat="server" ID="Button3" Text="Clear the log window" OnClick="btnClearLog_Click"/>
                <asp:Label ID="lbError" runat="server" Font-Bold="false" ForeColor="Red" Text="" />
            </div>

            <div class="row">
                <asp:TextBox id="tbLog" runat="server" TextMode="MultiLine" Height="350" />
            </div>
        </div>
    </div>
</asp:Content>
