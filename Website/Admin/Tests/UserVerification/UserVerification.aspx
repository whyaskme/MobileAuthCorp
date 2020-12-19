<%@ Page Title="MAC Otp System Administration"
    Language="C#"
    ValidateRequest="false"
    MasterPageFile="~/MasterPages/AdminConsole.master"
    AutoEventWireup="true"
    CodeFile="UserVerification.aspx.cs"
    Inherits="MACUserApps.Web.Tests.UserVerification.MacUserAppsWebTestsUserVerificationUserVerification" %>

<%@ Register Src="~/UserControls/Menu-Test.ascx" TagPrefix="uc1" TagName="MenuTest" %>


<asp:Content ID="BodyContainer" ContentPlaceHolderID="BodyContent" Runat="Server">
    <div style="padding: 0.25rem;"></div>
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
    <div class="row">
        <div class="large-12 columns">
            <fieldset><legend>Verify End User</legend>
                <div class="row">
                    <div class="large-6 medium-6 small-12 columns">
                        <label><asp:Label ID="Label1" runat="server" Text="Client:" />
                            <asp:DropDownList CssClass="chosen-select" ID="ddlClient" runat="server" />
                        </label>
                    </div>
                    <div class="large-6 medium-6 small-12 columns">
                        <label><asp:Label id="l1" Text="Users" runat="server" />
                            <asp:TextBox ID="txtTestUser" runat="server" Text="- None -" />
                        </label>
                    </div>
                </div>
                <div class="row">
                    <div class="large-12 columns">
                        <asp:LinkButton ID="lbFormFill" runat="server" Text="Fill Form" Font-Bold="true" Font-Underline="true" OnClick="lbFormFill_Click" />
                    </div>
                </div>
                <div style="padding: 0.75rem;"></div>
                <div class="row">
                    <div class="large-6 medium-6 small-12 columns">
                        <label>First Name
                            <asp:TextBox ID="txtFirstName" runat="server"/>
                        </label>
                    </div>
                    <div class="large-6 medium-6 small-12 columns">
                        <label>Last Name
                            <asp:TextBox ID="txtLastName" runat="server"/>
                        </label>
                    </div>
                </div>
                <div class="row">
                    <div class="large-6 medium-6 small-12 columns">
                        <label>Address
                            <asp:TextBox ID="txtAdr" runat="server" />
                        </label>
                    </div>
                    <div class="large-6 medium-6 small-12 columns">
                        <label>Suite #
                            <asp:TextBox ID="txtUnit" runat="server" />
                        </label>
                    </div>
                </div>
                <div class="row">
                    <div class="large-6 medium-6 small-12 columns">
                        <label>City
                            <asp:TextBox ID="txtCity" runat="server" />
                        </label>
                    </div>
                    <div class="large-6 medium-6 small-12 columns">
                        <label>State
                            <asp:TextBox ID="txtState" runat="server" />
                        </label>
                    </div>
                </div>
                <div class="row">
                    <div class="large-6 medium-6 small-12 columns">
                        <label>Zip Code
                            <asp:TextBox ID="txtZipCode" runat="server"/>
                        </label>
                    </div>
                    <div class="large-6 medium-6 small-12 columns">
                        &nbsp;
                    </div>
                </div>
                <div style="padding: 0.75rem;"></div>
                <div class="row">
                    <div class="large-6 medium-6 small-12 columns">
                        <label>Date of Birth
                            <asp:TextBox ID="txtDOB" runat="server" />
                        </label>
                    </div>
                    <div class="large-6 medium-6 small-12 columns">
                        <label>Last 4 of SSN
                            <asp:TextBox ID="txtSSN4" runat="server" />
                        </label>
                    </div>
                </div>
                <div class="row">
                    <div class="large-6 medium-6 small-12 columns">
                        <label>Driver's License Number
                            <asp:TextBox ID="txtDriverLic" runat="server" />
                        </label>
                    </div>
                    <div class="large-6 medium-6 small-12 columns">
                        <label>State Issued
                            <asp:TextBox ID="txtDriverLicSt" runat="server" />
                        </label>
                    </div>
                </div>
                <div class="row">
                    <div class="large-6 medium-6 small-12 columns">
                        <label>Mobile Phone
                            <asp:TextBox ID="txtMPhoneNo" runat="server" />
                        </label>
                    </div>
                    <div class="large-6 medium-6 small-12 columns">
                        <label>Email
                            <asp:TextBox ID="txtEmailAdr" runat="server"  />
                        </label>
                    </div>
                </div>
                <div class="row">
                    <div class="large-6 medium-6 small-12 columns">
                        <label>Unique User Id
                            <asp:TextBox ID="txtUid" runat="server" />
                        </label>
                    </div>
                    <div class="large-6 medium-6 small-12 columns">
                        &nbsp;
                    </div>
                </div>
                <div class="row">
                    <div class="large-12 columns">
                        <asp:Button CssClass="button tiny radius" ID="btnUseLNS" runat="server" Text="Use LexisNexis" OnClick="btnUseLNS_Click" />
                        <asp:Button CssClass="button tiny radius" ID="btnUseWPP" runat="server" Text="Use White Pages" OnClick="btnUseWPP_Click" />
                    </div>
                </div>
            </fieldset>
        </div>
    </div>
    <div class="row">
        <div class="large-12 columns">
            <asp:Label ID="lbError" runat="server" Font-Bold="false" ForeColor="Red" Visible="false" />
        </div>
    </div>
    
    <div class="row">
        <div class="large-12 columns">
            <asp:TextBox id="tbLog" runat="server" TextMode="MultiLine" Height="350" />
        </div>
    </div>
    <div>
        Name Address SSN Summary:
        <asp:TextBox ID="txtNameAddressSSNSummary" runat="server" Width="20" BorderStyle="None" Font-Underline="true" />
        &nbsp;
        Additional Score1:
        <asp:TextBox ID="txtAdditionalScore1" runat="server" Width="20" BorderStyle="None"  Font-Underline="true"/>
        &nbsp;
        Additional Score2:
        <asp:TextBox ID="txtAdditionalScore2" runat="server" Width="20" BorderStyle="None"  Font-Underline="true"/>
        <br />
        Name Address Phone Summary:
        <asp:TextBox ID="txtNameAddressPhoneSummary" runat="server" Width="20"  BorderStyle="None"  Font-Underline="true"/>
        Date of Birth Match Level:
        <asp:TextBox ID="txtDOBMatchLevel" runat="server" Width="20" BorderStyle="None"  Font-Underline="true"/>
        <br />
        Comprehensive Verification Index:
        <asp:TextBox ID="txtComprehensiveVerificationIndex" runat="server" Width="20" BorderStyle="None"  Font-Underline="true"/>
        <br />
        Risk Indicators
        <br />
        <asp:TextBox ID="txtRiskIndicators" runat="server" TextMode="MultiLine" Height="150" Width="700" />

    </div>
</asp:Content>