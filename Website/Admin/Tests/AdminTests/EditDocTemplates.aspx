<%@ Page Title="MAC Otp System Administration"
    Language="C#"
    ValidateRequest="false"
    MasterPageFile="~/MasterPages/AdminConsole.master" 
    AutoEventWireup="true"
    CodeFile="EditDocTemplates.aspx.cs"
    Inherits="Admin_Tests_AdminOTPTests_EditDocTemplates" %>

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
    <div style="padding: 0.5rem;"></div>
    <div class="row">
        <div class="large-6 medium-6 small-12 columns">
            <label>Clients
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
        <div class="large-12 columns">
            <asp:Button CssClass="button tiny radius" ID="btnGetDocTemplates" runat="server" Text="Get Templates" OnClick="btnGetDocTemplates_Click" tooltip="Get Document Templates from client"/>
        </div>
    </div>
    <!-- Document Template table -->
    <div style="padding: 0.5rem;"></div>
    <div class="row">
        <div class="large-12 columns">
            <asp:Table ID="tbDocs" runat="server" Width="100%"></asp:Table>
        </div>
    </div>    
    <div style="padding: 0.5rem;"></div>
    <div class="row">
        <div class="large-12 columns">
            <asp:Label ID="lbError" runat="server" Font-Bold="true" ForeColor="Red" />
        </div>
    </div>
    <div class="row">
        <div class="large-12 columns">
            <asp:TextBox id="tbLog" runat="server" TextMode="MultiLine" Height="300" />
        </div>
    </div>
    <div>
        <input id="hiddenTemplates" runat="server" type="hidden" value="" />
    </div>
</asp:Content>

