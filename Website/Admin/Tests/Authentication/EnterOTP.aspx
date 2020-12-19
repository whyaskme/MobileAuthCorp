<%@ Page Title="MAC Otp System Administration"
    Language="C#"
    ValidateRequest="false"
    MasterPageFile="~/MasterPages/AdminConsole.master" 
    AutoEventWireup="true" 
    CodeFile="EnterOtp.aspx.cs"
    Inherits="MACUserApps.Web.Tests.Authentication.MacUserAppsWebTestsAuthenticationEnterOtp" %>

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

    function writeAdCookie() {
        var adcoupondata = $("#adURL").attr("adcoupondata");
        $.cookie('adcouponcookie', adcoupondata, { expires: 7, path: '/' });
    }
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
                                <label>Submit</label>
                                <asp:Button CssClass="button tiny radius" ID="btnSubmit" runat="server" Text="Submit" toolTip="Submit Otp for verification." OnClick="btnSubmit_Click" />
                                <asp:Button CssClass="button tiny radius" ID="btnSubmitBadRID" runat="server" Text="BadRID" toolTip="Submit Otp for verification with bad RID." OnClick="btnSubmit_Click" />
                                <asp:Button CssClass="button tiny radius" ID="btnSubmitBadCID" runat="server" Text="BadCID" toolTip="Submit Otp for verification with bad Client Id." OnClick="btnSubmit_Click" />
                                <asp:Button CssClass="button tiny radius" ID="btnSubmitWrongCID" runat="server" Text="WrongCID" toolTip="Submit Otp for verification with Wrong Client Id." OnClick="btnSubmit_Click" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-12 columns">
                                <label>Resend</label>
                                <asp:Button CssClass="button tiny radius" ID="btnResend" runat="server" Text="Resend" toolTip="Resend OTP." OnClick="btnResend_Click" />
                                <asp:Button CssClass="button tiny radius" ID="btnResendBadRID" runat="server" Text="BadRID"  toolTip="Request Resend with bad request id." OnClick="btnResend_Click" />
                                <asp:Button CssClass="button tiny radius" ID="btnResendBadCID" runat="server" Text="BadCID"  toolTip="Request Resend with bad Client Id." OnClick="btnResend_Click" />
                                <asp:Button CssClass="button tiny radius" ID="btnResendWrongCID" runat="server" Text="WrongCID" toolTip="Request Resend with Wrong Client Id." OnClick="btnResend_Click" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-12 columns">
                                <label>Submit to Reply Service</label>
                                <asp:Button CssClass="button tiny radius" ID="btnSubmitAsReply" runat="server" Text="Reply" toolTip="Submit to Message Reply Service" OnClick="btnSubmitAsReply_Click" />
                                <asp:Button CssClass="button tiny radius" ID="btnReplyBadID" runat="server" Text="BadRID"  toolTip="Submit to Message Reply Service with bad id" OnClick="btnSubmitAsReply_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </fieldset>
        </div>
    </div>
    <div class="row">
        <div class="large-12 columns">
             <asp:Button CssClass="button tiny radius" ID="btnRestart" runat="server" Text="Restart" toolTip="Click to have server send another Otp" OnClick="btnRestart_Click" />
            <asp:Button CssClass="button tiny radius" runat="server" ID="btnClearLog" Text="Clear Logs" OnClick="btnClearLog_Click"/>
            <asp:Label ID="lbError" runat="server" Font-Bold="false" ForeColor="Red" />
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