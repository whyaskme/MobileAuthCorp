<%@ Page Title="MAC Otp System Administration"
    Language="C#"
    ValidateRequest="false"
    MasterPageFile="~/MasterPages/AdminConsoleNoLogin.master"
    AutoEventWireup="true"
    CodeFile="RegistrationCompletion.aspx.cs"
    Inherits="MACUserApps.Web.Tests.RegistrationCompletion.MacUserAppsWebTestsRegistrationCompletionRegistrationCompletion" %>

<asp:Content ID="BodyContainer" ContentPlaceHolderID="BodyContent" Runat="Server">
    <div style="padding: 0.75rem;"></div>
    <div class="row">
        <div class="large-12 medium-12 small-12 columns">
            <h3>Registration Completion</h3>
        </div>
    </div>
    <div class="row">
        <div class="large-12 medium-12 small-12 columns">
            <asp:Label ID="lbUserName" runat="server" />
            ,
        </div>
    </div>
    <div style="padding: 0.75rem;"></div>
    <div class="row">
        <div class="large-12 medium-12 small-12 columns">
            This is the continuation of the 
            <asp:Label Id="lbClientName" runat="server" Text="ClientName" Font-Underline="true" />
             One-Time Password registration process.
        </div>
    </div>
    <div style="padding: 0.75rem;"></div>
    <div class="row">
        <div class="large-12 medium-12 small-12 columns">
            A One-Time Password "OTP" was sent to your registered mobile phone.
        </div>
    </div>
    <div class="row">
        <div class="large-12 medium-12 small-12 columns">
            When yor receive the text message enter the "OTP" in the following box:
        </div>
    </div>
    <div class="row">
        <div class="large-12 medium-12 small-12 columns">
            <asp:TextBox ID="txtOtp" runat="server" Width="100" />
        </div>
    </div>
    <div class="row">
        <div class="large-12 medium-12 small-12 columns">
            Then click
            <asp:LinkButton ID="lnkbtnSubmit" runat="server" Text="HERE" Font-Bold="true" Font-Underline="true" OnClick="btnSubmit_Clicked" />
            to complete your registration. 
        </div>
    </div>    
    <div style="padding: 0.75rem;"></div>
    <div class="row">
        <div class="large-12 medium-12 small-12 columns">
            Thank you,
            <asp:Label Id="lbClientName1" runat="server" Text="ClientName"/>
        </div>
    </div>
    <div style="padding: 0.75rem;"></div>  
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
</asp:Content>
