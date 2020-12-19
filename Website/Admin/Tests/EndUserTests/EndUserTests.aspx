<%@ Page Title="MAC Otp System Administration"
    Language="C#"
    ValidateRequest="false"
    MasterPageFile="~/MasterPages/AdminConsole.master" 
    AutoEventWireup="true" 
    CodeFile="EndUserTests.aspx.cs" 
    Inherits="Admin_Tests_EndUserTests_EndUserTests" %>

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
            <h3>End User</h3>
        </div>
    </div>

    <div id="panel2" class="content">
        <div class="row">
            <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.5rem;">
                <label><asp:Label ID="Label1" runat="server" Text="" />
                    <asp:DropDownList CssClass="chosen-select" ID="ddlClient" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlClient_Selected" />                                
                </label>
            </div>
            <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.5rem;">
                <label><asp:Label ID="lbGroup" runat="server" Text="" />
                    <asp:DropDownList CssClass="chosen-select" ID="ddlGroups" runat="server" />                                
                </label>
            </div>
        </div>
    </div>

    <div class="row">
        <div class="large-6 medium-6 small-12 columns">
            <label>Last Name (Required)
                <asp:TextBox ID="txtLastName" runat="server" />
            </label>
        </div>
        <div class="large-6 medium-6 small-12 columns">
            <label>Email / Unique Id (Required)
                <asp:TextBox ID="txtEmailAdr" runat="server" />
            </label>
        </div>
    </div>
    <div class="row">
        <div class="large-6 medium-6 small-12 columns">
            <label>Client Generated Unique Id (aka STS)
                <asp:TextBox ID="txtSTSUserId" runat="server" />
            </label>
        </div>
        <div class="large-6 medium-6 small-12 hidden-for-small columns">
            &nbsp;
        </div>
    </div>
    <div class="row">
        <div class="large-12 columns">
            <asp:Button CssClass="button tiny radius" ID="btnComputeUserId" runat="server" Text="Compute UserId" 
                OnClick="btnComputeUserId_Click"/>
            <asp:Button CssClass="button tiny radius" ID="btnCheckEndUserReg" runat="server" Text="Check End User Registration" 
                OnClick="btnCheckEndUserReg_Click"/>
            <asp:Button CssClass="button tiny radius" runat="server" ID="btnClearLog" Text="Clear" 
                            OnClick="btnClearLog_Click"/>
        </div>
    </div>
    
    <div class="row">
        <div class="large-12 columns">
            <label><asp:Label ID="lbError" runat="server" Font-Bold="false" ForeColor="Red" /></label>
        </div>
    </div>    
    <div class="row">
        <div class="large-12 columns">
            <asp:TextBox id="tbLog" runat="server" TextMode="MultiLine" Height="350" />
        </div>
    </div>

</asp:Content>