<%@ Page Title="MAC Otp System Administration"
    Language="C#"
    ValidateRequest="false"
    MasterPageFile="~/MasterPages/AdminConsole.master"
    AutoEventWireup="true"
    CodeFile="TxEnterOtp.aspx.cs"
    Inherits="MACUserApps.Web.Tests.TrxVerification.MacUserAppsWebTestsTrxVerificationTxEnterOtp" %>

<%@ Register Src="~/UserControls/Menu-Test.ascx" TagPrefix="uc1" TagName="MenuTest" %>

<asp:Content ID="BodyContainer" ContentPlaceHolderID="BodyContent" Runat="Server">
    <script>
        $().ready(function () {
            $('#cbXMLDS').click(function () {
                if ($('#cbXMLDS').is(':checked')) {
                    $('#cbXML').prop('checked', false);
                } else {
                    $('#cbXMLDS').prop('checked', true);
                }
            });
            $('#cbXML').click(function () {
                if ($('#cbXML').is(':checked')) {
                    $('#cbXMLDS').prop('checked', false);
                } else {
                    $('#cbXMLDS').prop('checked', true);
                }
            });
        });
    </script>
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
    <div style="padding: 0.25rem;"></div>
    <!-------- Bill Payment ---------------------------------------------------------->
    <div class="row">
        <div class="large-12 columns">
            <fieldset><legend>Enter OTP</legend>
                <div class="row">
                    <div class="large-6 medium-6 small-12 columns">
                        <div style="margin-bottom: 1rem;width: 100%;" id="AdDiv" runat="server">
                            Ad goes here
                        </div>
                    </div>
                    <div class="large-6 medium-6 small-12 columns">
                        <div class="row">
                            <div class="large-12 columns">
                                <div><label><strong>CID:</strong> <asp:Label ID="lbCID" runat="server" Font-Underline="false" /></label></div>
                                <div><label><strong>GID:</strong> <asp:Label ID="lbGID" runat="server" Font-Underline="false" /></label></div>
                                <div><label><strong>RID:</strong> <asp:Label ID="lbRID" runat="server" Font-Underline="false" /></label></div>
                            </div>
                        </div>
                        <div style="padding: 0.25rem;"></div>
                        <div class="row">
                            <div class="large-12 columns">
                                <label><asp:Label ID="l1" runat="server" Text="Enter Otp:" />
                                    <asp:TextBox ID="tbOtp" runat="server" />
                                </label>
                            </div>
                        </div>
                        <div class="large-12 columns">
                            <label>Response Format</label>
                            <asp:CheckBox runat="server" ID="cbXMLDS" Text="Return XML with Delimited Strings(Default)" Checked="true"/>
                            <asp:CheckBox runat="server" ID="cbXML" Text="Return XML"/>
                        </div>
                        <div class="row">
                            <div class="large-12 columns">
                                <asp:Button CssClass="button tiny radius" ID="btnSubmit" runat="server" Text="Submit" toolTip="Click to submit Otp for verification." OnClick="btnSubmit_Click" />
                                <asp:Button CssClass="button tiny radius" ID="btnResend" runat="server" Text="Resend" toolTip="Click to have server resend the otp" OnClick="btnResend_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </fieldset>
        </div>
    </div>
    <div style="padding: 0.5rem;"></div>
    <div class="row">
        <div class="large-12 columns">
            <asp:Button CssClass="button tiny radius" runat="server" ID="btnClearLog" Text="Clear Logs" OnClick="btnClearLog_Click"/>
            <asp:Label ID="lbError" runat="server" ForeColor="Red" />
        </div>
    </div>
    <div class="row">
        <div class="large-12 columns">
            <label>
                <asp:TextBox id="tbLog" runat="server" TextMode="MultiLine" Height="350" />
            </label>
        </div>
    </div>
</asp:Content>