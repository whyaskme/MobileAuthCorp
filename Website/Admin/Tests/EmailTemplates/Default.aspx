<%@ Page Title="MAC Otp System Administration"
    Language="C#"
    ValidateRequest="false"
    MasterPageFile="~/MasterPages/AdminConsole.master"
    AutoEventWireup="true" 
    CodeFile="Default.aspx.cs"
    Inherits="EmailTemplates.Default" %>

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
            <h3>Email Templates</h3>
        </div>
    </div>

    <div class="row" id="divMsgContainer" runat="server">
        <div class="large-12 columns">
            <div class="alert-box success radius" id="divMsgDetails" runat="server" style="display: ; cursor: pointer;" onclick="javascript: noDisplay();">
                Email successfully sent!
            </div>
        </div>
    </div>
    
    <div style="padding: 0.25rem;"></div>

    <div class="row">
        <div class="large-4 medium-4 small-12 columns">
            <asp:DropDownList ID="dlClients" runat="server" AutoPostBack="False" CssClass="chosen-select">
                <asp:ListItem Value="000000000000000000000000">Select a Client</asp:ListItem>
            </asp:DropDownList>
            <div style="padding: 0.75rem;"></div>
        </div>
        
        <div class="large-12 medium-12 small-12 columns">
            <label>Send To
                <asp:TextBox ID="txtEmailTo" runat="server"></asp:TextBox>
            </label>
        </div>
        <div class="large-12 medium-12 small-12 columns">
            <label>Subject
                <asp:TextBox ID="txtSubject" runat="server"></asp:TextBox>
            </label>
        </div>
        <div class="large-12 medium-12 small-12 columns">
            <label>Message
                <textarea id="txtBody" runat="server" cols="10" rows="2" style="width: 500px;"></textarea>
            </label>
        </div>
        <div class="large-12 medium-12 small-12 columns">
            <input type="submit" value="Send Message" class="button tiny radius" />
        </div>
    </div>

</asp:Content>
