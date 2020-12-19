<%@ Page Title="MAC Otp System Administration"
    Language="C#"
    ValidateRequest="false"
    MasterPageFile="~/MasterPages/AdminConsole.master" 
    AutoEventWireup="true"
    CodeFile="Done.aspx.cs"
    Inherits="MACUserApps.Web.Tests.Authentication.MacUserAppsWebTestsAuthenticationDone" %>

<asp:Content ID="BodyContainer" ContentPlaceHolderID="BodyContent" Runat="Server">
    <div class="row"> 
        <div class="large-12 columns">
            <div class="title_875rem" style="margin-bottom: 1rem;">** Otp verified **</div>
            <asp:Button ID="btnDoItAgain" CssClass="button tiny radius" runat="server" Text="Do It Again" OnClick="btnDoItAgain_Click" />
            <div style="padding:0.25rem;"></div>
            <asp:TextBox id="tbLog" runat="server" TextMode="MultiLine" Height="500" Width="100%" />
        </div>
    </div>
</asp:Content>
