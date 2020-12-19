<%@ Page Title="MAC Otp System Administration"
    Language="C#"
    ValidateRequest="false"
    MasterPageFile="~/MasterPages/AdminConsole.master"
    AutoEventWireup="true" 
    CodeFile="RegisterUsersInFile.aspx.cs"
    Inherits="MACUserApps.Web.Tests.EndUserRegistration.MacUserAppsWebTeStsEndUserRegistrationRegisterUsersInFile" %>

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
            <h3>File Registration</h3>
        </div>
    </div>

    <div style="padding: 0.25rem;"></div>
    <div id="divDataForm" runat="server">
        <div class="row">
            <div class="large-12 columns">
                <fieldset><legend></legend>
                    <div class="row">
                        <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.5rem;">
                            <label><asp:Label ID="Label1" runat="server" Text="" Font-Bold="false" />
                                <asp:DropDownList CssClass="chosen-select" ID="ddlClient" runat="server" AutoPostBack="true"  OnSelectedIndexChanged="ddlClient_Selected" />
                            </label>
                        </div>
                        <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.5rem;">
                            <label><asp:Label ID="lbGroup" runat="server" Text="Groups:" Font-Bold="false" />
                                <asp:DropDownList CssClass="chosen-select" ID="ddlGroups" runat="server" />
                            </label>
                        </div>
                    </div>
                    <div style="padding: 0.5rem;"></div>
                    <div class="row">
                        <div class="large-12 columns">
                            <label><strong>Registration Type</strong></label>
                                <asp:RadioButton id="rbOpen" Text="&nbsp;Open (any client)" GroupName="type" runat="server" />&nbsp;
                                <asp:RadioButton id="rbClientRestricted" Text="&nbsp;Restricted to Client" Checked="True" GroupName="type" runat="server" />&nbsp;
                                <asp:RadioButton id="rbGroupRestricted" Text="&nbsp;Restricted to Group" GroupName="type" runat="server" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="large-4 medium-4 small-12 columns" style="margin-bottom: 0.5rem;">
                            <asp:FileUpload CssClass="button tiny radius" id="FileUploadControl" runat="server" />
                        </div>
                        <div class="large-8 medium-8 small-12 columns hidden-for-medium hidden-for-small" style="margin-bottom: 0.5rem;">
                            &nbsp;
                        </div>
                    </div>
                    <div class="row">
                        <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.5rem;">
                            <asp:Button CssClass="button tiny radius" runat="server" id="btnProcessFile" text="Process File" onclick="btnProcessFile_Click" />
                        </div>
                    </div>
                </fieldset>
            </div>
        </div>
        <div style="padding: 0.75rem;"></div>
        <div class="row">
            <div class="large-12 columns">
                <asp:Button CssClass="button tiny radius" runat="server" ID="btnClearLog" Text="Clear Logs" OnClick="btnClearLog_Click"/>
                <asp:Label ID="lbError" runat="server" Font-Bold="true" ForeColor="Red" />
            </div>
        </div>
        <div class="row">
            <div class="large-12 columns">
                <asp:TextBox id="tbLog" runat="server" TextMode="MultiLine" Height="350" />
            </div>
        </div>
    </div>
</asp:Content>