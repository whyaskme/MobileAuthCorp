<%@ Page Title="MAC Otp System Administration"
    Language="C#"
    ValidateRequest="false"
    MasterPageFile="~/MasterPages/AdminConsole.master"
    AutoEventWireup="true" 
    CodeFile="txDone.aspx.cs"
    Inherits="MACUserApps.Web.Tests.TrxVerification.MacUserAppsWebTestsTrxVerificationTxDone" %>

<asp:Content ID="BodyContainer" ContentPlaceHolderID="BodyContent" Runat="Server">
    <div> 
        <b>** Otp verified **</b>
        <br /><br />
        <asp:Button ID="btnDoItAgain" runat="server" Text="Do It Again" OnClick="btnDoItAgain_Click" />
        <br />
        <asp:TextBox id="tbLog" runat="server" TextMode="MultiLine" Height="500" Width="95%" />
    </div>
</asp:Content>