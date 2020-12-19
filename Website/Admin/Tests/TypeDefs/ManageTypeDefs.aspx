<%@ Page Title="MAC Otp System Administration"
    Language="C#"
    ValidateRequest="false"
    MasterPageFile="~/MasterPages/AdminConsole.master"
    AutoEventWireup="true"
    CodeFile="ManageTypeDefs.aspx.cs"
    Inherits="MACUserApps.Web.Tests.TypeDefs.MacUserAppsWebTestsTypeDefsManageTypeDefs" %>

<%@ Register Src="~/UserControls/Menu-Test.ascx" TagPrefix="uc1" TagName="MenuTest" %>


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
            <h3>Manage Type Definitions</h3>
        </div>
    </div>

    <div class="row">
        <div class="large-6 medium-6 small-12 columns">
            <label>
                <asp:DropDownList CssClass="chosen-select" id="ddlSType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="dldSType_Changed">
                    <asp:ListItem>Select Type</asp:ListItem>
               </asp:DropDownList>
            </label>
        </div>
        <div class="large-6 medium-6 small-12 columns">
            <label><asp:Label ID="lbItem" runat="server" Text="" Font-Bold="false" Visible="false" />
                <asp:DropDownList CssClass="chosen-select" id="ddlSItem" runat="server" Visible="false" AutoPostBack="true" OnSelectedIndexChanged="dldSItem_Changed">
                    <asp:ListItem>Select Item</asp:ListItem>
                </asp:DropDownList>
            </label>
        </div>
    </div>    
    <div style="padding: 0.25rem;"></div>
    <div class="row">
        <div class="large-12 columns">
            <asp:HiddenField ID="hiddenT" runat="server" />
            <asp:HiddenField ID="HiddenChangeList" runat="server" />
            <asp:Label ID="lbError" runat="server" Font-Bold="false" ForeColor="Red" />
            <asp:Label ID="lbResult" runat="server" Font-Bold="false" ForeColor="Green" />
        </div>
    </div>
    <div style="padding: 0.25rem;"></div>
    <div class="row">
        <div class="large-12 columns">
            <div id="divtable" runat="server"></div>
            <div id="divaction" runat="server" style="visibility: hidden"></div>
        </div>
    </div>       
</asp:Content>