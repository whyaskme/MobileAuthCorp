<%@ Page Title="MAC Otp System Administration" 
    Language="C#"
    ValidateRequest="false" 
    MasterPageFile="~/MasterPages/AdminConsole.master"
    AutoEventWireup="true" 
    CodeFile="EndUserManagement.aspx.cs"
    Inherits="MACUserApps_Web_Tests_EndUserManagement_EndUserManagement" %>

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
            <h3>End User Management</h3>
        </div>
    </div>

    <div style="padding: 0.25rem;"></div>
    <div id="divDataForm" runat="server">
        <div class="row">
            <div class="large-12 columns">
                <fieldset><legend></legend>
                    <div class="row">
                        <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.5rem;">
                            <label><asp:Label ID="Label1" runat="server" Text="" />
                                <asp:DropDownList CssClass="chosen-select" ID="ddlClient" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlClient_Selected" />                                
                            </label>
                        </div>
                        <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.5rem;">
                            <label><asp:Label ID="lbGroup" runat="server" Text="" Font-Bold="false" Visible="false" />
                                <asp:DropDownList CssClass="chosen-select" ID="ddlGroups" runat="server"  Visible="false" />                                
                            </label>
                        </div>
                    </div>
                    <div class="row">
                        <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.5rem;">
                            <label><asp:Label ID="Label2" runat="server" Text="" />  
                                <asp:DropDownList CssClass="chosen-select" ID="ddlTestUserFiles" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlTestUserFiles_Selected" />                                
                            </label>
                        </div>
                        <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.5rem;">
                            <label>
                                <asp:DropDownList CssClass="chosen-select" ID="ddlEndUserList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlEndUserList_Selected"  />
                            </label>
                        </div>
                    </div>
                    <div style="padding: 0.5rem;"></div>
                    <div class="row">
                        <div class="large-4 medium-4 small-12 columns" style="margin-bottom: 0.5rem;">
                            <asp:Button CssClass="button tiny radius" ID="btnDeactivate" runat="server" Text="Deactivate" OnClick="btnDeactivateEndUser_Click" tooltip="Click to deactivate selected end user."/>
                            <asp:Button CssClass="button tiny radius" ID="btnActivate" runat="server" Text="Activate" OnClick="btnActivateEndUser_Click" tooltip="Click to activate selected end user."/>
                        </div>
                        <div class="large-4 medium-4 small-12 columns" style="margin-bottom: 0.5rem;">
                            <asp:Button CssClass="button tiny radius" ID="btnDelete" runat="server" Text="Delete" OnClick="btnDeleteEndUser_Click" tooltip="Click to deactivate selected end user."/>
                        </div>
                        <div class="large-4 medium-4 small-12 columns" style="margin-bottom: 0.5rem;">
                            <asp:Button CssClass="button tiny radius" id="btnAdPassEnable" Text="Enable Ads" runat="server" OnClick="btnAdPassEnable_Click" tooltip="Click to set Opt-In to Ads for selected end user."/>
                            <asp:Button CssClass="button tiny radius" id="btnAdPassDisable" Text="Disable Ads" runat="server" OnClick="btnAdPassDisable_Click" tooltip="Click to set Opt-Out to Ads for selected end user."/>
                        </div>
                    </div>                   
                    <div class="row">
                        <div class="large-12 columns">
                            <hr />
                        </div>
                    </div>
                    <div class="row">
                        <div class="large-6 medium-6 small-12 columns">
                            <label>
                                <asp:TextBox runat="server" ID="txtPhoneNo" Text="Phone Number" />
                            </label>
                        </div>
                        <div class="large-6 medium-6 small-12 columns hide-for-small">
                            &nbsp;
                        </div>
                    </div>
                    <div class="row">
                        <div class="large-12 columns" style="margin-bottom: 0.5rem;">
                            <asp:Button CssClass="button tiny radius" ID="btnChangeUserInfo" runat="server" Text="Change" OnClick="btnChangeUserInfo_Click" />
                        </div>
                    </div>
                </fieldset>
            </div>
        </div>        
        <div class="row">
            <div class="large-12 columns" style="margin-bottom: 0.5rem;">
                <asp:Label ID="lbError" runat="server" Font-Bold="true" ForeColor="Red" Text="" />
            </div>
        </div>
        <div class="row">
            <div class="large-12 columns" style="margin-bottom: 0.5rem;">
                <asp:TextBox id="tbLog" runat="server" TextMode="MultiLine" Height="350" />
            </div>
        </div>
    </div>

</asp:Content>