<%@ Page Title="MAC Otp System Administration"
    Language="C#" 
    ValidateRequest="false"
    MasterPageFile="~/MasterPages/AdminConsole.master"
    AutoEventWireup="true" 
    CodeFile="IssueRequest.aspx.cs"
    Inherits="MACUserApps_Web_Tests_IssueRequest_IssueRequest" %>

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
            <h3>Issue Request</h3>
        </div>
    </div>

    <div id="divDataForm" runat="server">
        <div class="row">
            <div class="large-12 columns">
                <fieldset><legend></legend>
                    <div class="row">
                        <div class="large-6 medium-6 small-12 columns">
                            <label><asp:Label ID="Label11" runat="server" Text="Last Name" />
                                <asp:TextBox runat="server" ID="txtLastName" />                                                                
                            </label>
                        </div>
                        <div class="large-6 medium-6 small-12 columns">
                            <label><asp:Label ID="Label12" runat="server" Text="Unique Identifier" />
                                <asp:TextBox runat="server" ID="txtUID" />                                                                
                            </label>
                        </div>
                    </div>
                    <div class="row">
                        <div class="large-6 medium-6 small-12 columns">
                            <label>Hash #
                                <asp:TextBox runat="server" ID="txtUserid" />
                            </label>
                        </div>
                        <div class="large-6 medium-6 small-12 columns">
                            &nbsp;
                        </div>
                    </div>
                    <div class="row">
                        <div class="large-12 columns">
                            <asp:Button CssClass="button tiny radius" runat="server" ID="btnHash" Text="Generate Hash" OnClick="btnHash_Clicked"/>
                        </div>
                    </div>
                </fieldset>
            </div>
        </div>
        <div class="row">
            <div class="large-12 columns">
                <fieldset><legend>&nbsp;</legend>
                    <div class="row">
                        <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.75rem;">
                            <label>OTP Services
                                <asp:DropDownList CssClass="chosen-select" runat="server" ID="ddlOtpServiceNames" AutoPostBack="True" OnSelectedIndexChanged="ddlOtpServiceNames_Changed"/>                         
                            </label>
                        </div>
                        <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.75rem;">
                            <label>User Services
                                <asp:DropDownList CssClass="chosen-select" runat="server" ID="ddlUserServiceNames" AutoPostBack="True" OnSelectedIndexChanged="ddlUserServiceNames_Changed"/>
                            </label>
                        </div>
                    </div>
                    <div class="row">
                        <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.75rem;">
                            <label>OAS Services
                                <asp:DropDownList CssClass="chosen-select" runat="server" ID="ddlOASServiceNames" AutoPostBack="True" OnSelectedIndexChanged="ddlOASServiceNames_Changed"/>
                            </label>
                        </div>
                        <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.75rem;">
                            <label>Admin Services
                                <asp:DropDownList CssClass="chosen-select" runat="server" ID="ddlAdminServiceNames" AutoPostBack="True" OnSelectedIndexChanged="ddlAdminServiceNames_Changed"/>
                            </label>
                        </div>
                    </div>
                    <div class="row">
                        <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.75rem;">
                            <label>Event Services
                                <asp:DropDownList CssClass="chosen-select" runat="server" ID="ddlEventServiceNames" AutoPostBack="True" OnSelectedIndexChanged="ddlEventServiceNames_Changed"/>
                            </label>
                        </div>
                        <div class="large-6 medium-6 small-12 columns style="margin-bottom: 0.75rem;"">
                            <label>Test Services
                                <asp:DropDownList CssClass="chosen-select" runat="server" ID="ddlTestServiceNames" AutoPostBack="True" OnSelectedIndexChanged="ddlTestServiceNames_Changed"/>
                            </label>
                        </div>
                    </div>
                    <div style="padding: 0.5rem;"></div>
                    <div class="row">
                        <div class="large-12 columns">
                            <label>Request Message
                                <asp:TextBox id="txtRequest" runat="server" TextMode="MultiLine" Height="125" />
                            </label>
                        </div>
                    </div>
                    <div class="row">
                        <div class="large-12 columns">
                            <label>
                                <asp:Button CssClass="button tiny radius" ID="btnClearText" runat="server" Text="Clear Text" OnClick="btnClearText_Click" tooltip="Request data is in clear text."/>
                                <asp:Button CssClass="button tiny radius" ID="btnEncodedEncrypted" runat="server" Text="Encoded or Encryped" OnClick="btnEncodedEncrypted_Click" tooltip="Request data is either Hex Encoded or AES encrypted."/>
                            </label>
                        </div>
                    </div>
                </fieldset>
            </div>
        </div>        
        <div class="row">
            <div class="large-12 columns">
                <label>
                    <asp:Label ID="lbError" runat="server" Font-Bold="false" ForeColor="Red" Text="" />
                </label>
            </div>
        </div>
        <div style="padding: 0.5rem;"></div>
        <div class="row">
            <div class="large-12 columns" style="margin-bottom: 0.25rem;">
                    <asp:Button CssClass="button tiny radius" ID="btnClearLog" runat="server" Text="Clear Log" OnClick="btnClearLog_Click"/>
            </div>
        </div>
        <div class="row">
            <div class="large-12 columns">
                    <asp:TextBox id="tbLog" runat="server" TextMode="MultiLine" Height="350" />
            </div>
        </div>
    </div>
        <%--<asp:Label ID="Label3" runat="server" Text="Request data format ?" />--%>

</asp:Content>
