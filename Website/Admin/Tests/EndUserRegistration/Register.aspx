<%@ Page Title="MAC Otp System Administration"
    Language="C#"
    ValidateRequest="false"
    MasterPageFile="~/MasterPages/AdminConsole.master"
    AutoEventWireup="true" 
    CodeFile="Register.aspx.cs"
    Inherits="MACUserApps.Web.Tests.EndUserRegistration.MacUserAppsWebTeStsEndUserRegistrationRegister" %>

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
            <h3>User Interface Registration</h3>
        </div>
    </div>

    <div style="padding: 0.25rem;"></div>
        <div class="row">
            <div class="large-12 columns">
                <div id="divEmailSent" runat="server" style="width: 100%; padding-top: 10px;display:block !important;">
                    <br />
                    <asp:Label ID="lbFirstName" runat="server" Text="f" font-Bold="false" />
                    ,<br />
                    Thank you for registering to use <asp:Label ID="lbClientName" runat="server" Text="c" /> One-Time Password security.
                    <br /><br />
                    Please check your email at <asp:Label ID="lbEmailAddress" runat="server" Text="e" Font-Bold="true" /> and complete the registration process.
                    <br />
                    <asp:LinkButton ID="lnkbtnExit" runat="server" Text="Click Here" Font-Bold="true" Font-Underline="true" OnClick="lnkbtnExit_Click" /> to exit.
                    <br /><br />
                </div>
                <div id="divDataForm" runat="server" >
                    <div class="row">
                        <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.5rem;">
                            <label>
                                <asp:DropDownList CssClass="chosen-select" ID="ddlClient" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlClient_Selected" />
                            </label>
                        </div>
                        <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.5rem;">
                            <label><asp:Label ID="lbGroup" runat="server" Text="Groups" />
                                <asp:DropDownList CssClass="chosen-select" ID="ddlGroups" runat="server" />
                            </label>
                        </div>
                    </div>
                    <div style="padding: 0.5rem;"></div>
                    <div class="row">
                        <div class="large-4 medium-4 small-12 columns" style="margin-bottom: 0.75rem;">
                            <label><strong>Registration Options</strong></label><br />
                                <asp:RadioButton id="rbClientRestricted" Text="&nbsp;Restricted to Client" Checked="True" GroupName="type" runat="server"/><br />
                                <asp:RadioButton id="rbGroupRestricted" Text="&nbsp;Restricted to Group" GroupName="type" runat="server"/><br />
                                <asp:RadioButton id="rbOpen" Text="&nbsp;Open (any client)" GroupName="type" runat="server"/>
                        </div>
                        <div class="large-4 medium-4 small-12 columns" style="margin-bottom: 0.75rem;">
                            <label><strong>Ad Pass Options</strong></label><br />
                                <asp:RadioButton id="rdAdPassEnable" Text="&nbsp;Enable Ads" Checked="True" GroupName="AdCtrl" runat="server"/><br />
                                <asp:RadioButton id="rdAdPassDisable" Text="&nbsp;Disable Ads" GroupName="AdCtrl" runat="server"/>
                        </div>
                        <div class="large-4 medium-4 small-12 columns" style="margin-bottom: 0.75rem;">
                            <label><strong>Notifications Options</strong></label><br />
                                <asp:CheckBox ID="cbText" runat="server" Text="&nbsp;Text Message" /><br />
                                <asp:CheckBox ID="cbEmail" runat="server" Text="&nbsp;Email" Checked="true" />
                        </div>
                    </div>

                    <div class="row">
                        <div class="large-1 medium-1 small-12 columns">
                            <label>Prefix
                                <asp:TextBox ID="txtNamePrefix" runat="server" />
                            </label>
                        </div>
                        <div class="large-3 medium-3 small-12 columns">
                            <label>First Name *
                                <asp:TextBox ID="txtFirstName" runat="server" />
                            </label>
                        </div>
                        <div class="large-3 medium-3 small-12 columns">
                            <label>Middle Name
                                <asp:TextBox ID="txtMiddleName" runat="server" />
                            </label>
                        </div>
                        <div class="large-4 medium-4 small-12 columns">
                            <label>Last Name *
                                <asp:TextBox ID="txtLastName" runat="server" />
                            </label>
                        </div>
                        <div class="large-1 medium-1 small-12 columns">
                            <label>Suffix
                                <asp:TextBox ID="txtNameSuffix" runat="server" />
                            </label>
                        </div>
                    </div>
                    <div class="row">
                        <div class="large-6 medium-6 small-12 columns">
                            <label>Mobile Phone (Required)
                                <asp:TextBox ID="txtMPhoneNo" runat="server" />
                            </label>
                        </div>
                        <div class="large-6 medium-6 small-12 columns">
                            <label>Email (Required)
                                <asp:TextBox ID="txtEmailAdr" runat="server" />
                            </label>
                        </div>
                    </div>

                    <div class="row">
                        <div class="large-6 medium-6 small-12 columns">
                            <label>Unique ID
                                <asp:TextBox ID="txtUid" runat="server" />
                            </label>
                        </div>
                        <div class="large-6 medium-6 small-12 columns  hidden-for-medium hidden-for-small">
                            &nbsp;
                        </div>
                    </div>

                    <div class="row">
                        <div class="large-4 medium-4 small-12 columns">
                            <label>Address
                                <asp:TextBox ID="txtAdr" runat="server" />
                            </label>
                        </div>
                        <div class="large-4 medium-4 small-12 columns">
                            <label>Address 2
                                <asp:TextBox ID="txtAdr2" runat="server" />
                            </label>
                        </div>
                        <div class="large-4 medium-4 small-12 columns">
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
                            <label>Country
                                <asp:TextBox ID="txtCountry" runat="server"/>
                            </label>
                        </div>
                    </div>
                    <div class="row">
                        <div class="large-6 medium-6 small-12 columns">
                            <label>Date of Birth
                                <asp:TextBox ID="txtDOB" runat="server" />
                            </label>
                        </div>
                        <div class="large-6 medium-6 small-12 columns">
                            <label>Last 4 Digits of SSN
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
                        <div class="large-12 columns">
                            <label>
                                <asp:Button CssClass="button tiny radius" ID="btnRegisterEndUser" runat="server" Text="Register" OnClick="btnRegisterEndUser_Click" tooltip="Click to register the selected end user."/>
                            </label>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="large-12 columns">
                <label>
                    <asp:Button CssClass="button tiny radius" runat="server" ID="btnClearLog" Text="Clear Logs" OnClick="btnClearLog_Click"/>
                    <br />
                    <asp:Label ID="lbError" runat="server" ForeColor="Red" Text="" />
                </label>
            </div>
        </div>
        <div class="row">
            <div class="large-12 columns">
                <asp:TextBox id="tbLog" runat="server" TextMode="MultiLine" Height="350" />
            </div>
        </div>
</asp:Content>
