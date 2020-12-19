<%@ Page Title="MAC Otp System Administration"
    Language="C#" 
    ValidateRequest="false"
    MasterPageFile="~/MasterPages/AdminConsole.master"
    AutoEventWireup="true" 
    CodeFile="RegisterOasClient.aspx.cs"
    Inherits="MACUserApps.Web.Tests.RegisterOASClient.MacUserAppsWebTestsRegisterOasClientRegisterOasClient" %>

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
            <h3>Open Register Client</h3>
        </div>
    </div>

    <div style="padding: 0.25rem;"></div>
    <div class="row">
        <div class="large-12 columns">
            <fieldset><legend></legend>
                <div class="row">
                    <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.5rem;">
                        <label><asp:Label ID="Label3" runat="server" Text="" />
                            <asp:DropDownList CssClass="chosen-select" ID="ddlClient" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlClient_Selected" />
                        </label>
                    </div>
                    <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.5rem;">
                        <label><asp:Label ID="Label4" runat="server" Text="Groups" Visible="false" />
                            <asp:DropDownList CssClass="chosen-select" ID="ddlGroups" runat="server"  Visible="false" />
                        </label>
                    </div>
                </div>
                <div class="row">
                    <div class="large-6 medium-6 small-12 columns">
                        <label><asp:Label ID="L1" runat="server" Text="Client ID" />
                            <asp:TextBox ID="tbClientID" runat="server" />                            
                        </label>
                    </div>
                    <div class="large-6 medium-6 small-12 columns">
                        <label><asp:Label ID="Label2" runat="server" Text="Fully Qualified Domain Name" />
                            <asp:TextBox ID="tbFullyQualifiedDomainName" runat="server" />                                                            
                        </label>
                    </div>
                </div>
                <div style="padding: 0.25rem;"></div>
                <div class="row">
                    <div class="large-12 columns">
                        <label>
                            <asp:Button CssClass="button tiny radius" ID="btnRegister" runat="server" Text="Register" OnClick="btnRequest_Click" />
                            <asp:Button CssClass="button tiny radius" ID="btnDisable" runat="server" Text="Disable" OnClick="btnRequest_Click" />
                            <asp:Button CssClass="button tiny radius" ID="btnEnable" runat="server" Text="Enable" OnClick="btnRequest_Click" />
                            <asp:Button CssClass="button tiny radius" ID="btnDelete" runat="server" Text="Delete" OnClick="btnRequest_Click" />
                        </label>
                    </div>
                </div>
            </fieldset>
        </div>
    </div>
    <div class="row">
        <div class="large-12 columns" style="margin-bottom: 0.5rem;">
            <label>
                <asp:Label ID="lbError" runat="server" ForeColor="Red" Text="Test" />
            </label>
        </div>
    </div>
    <div class="row">
        <div class="large-12 columns" style="margin-bottom: 0.5rem;">
            <label>
                <asp:TextBox id="tbLog" runat="server" TextMode="MultiLine" Height="350" />
            </label>
        </div>
    </div>

</asp:Content>
