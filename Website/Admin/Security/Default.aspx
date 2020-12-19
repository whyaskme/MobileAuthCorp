<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/AdminConsole.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Admin.Security.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ScriptContainer" runat="Server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="BodyContent" runat="Server">

    <script type="text/javascript">
        function refreshPageAfterConfigUpdate() {
            // Click the logo to force a page refresh after loading new web.config
            var siteLogo = document.getElementById("SiteLogo");
            siteLogo.click();
        }
    </script>

    <!--show background-->
    <div class="hide-for-small">
        <div style="background: #ccc url(/Images/bg_wave10.jpg) top center repeat-x; height: 575px;">
            <div class="row hide-for-small" style="margin-top: 0;">
                <div class="large-12 columns">
                    <div style="font: bold 1.75rem 'Helvetica Neue', 'Helvetica', Helvetica, Arial, 'Open Sans', sans-serif; color: #fff; text-align: center; padding: 2.5rem 0;">One-Time Password (OTP)</div>
                </div>
            </div>
            <div class="row" style="margin-top: 0;">
                <div class="large-3 medium-3 small-12 columns hide-for-small">
                    &nbsp;
                </div>
                <div class="large-6 medium-6 small-12 columns">
                    <div style="margin: 0 auto;z-index: 0;padding: 0; border: 25px solid #ffffff; border: 25px solid rgba(255, 255, 255, 0.25); -ms-border-radius: 18px; border-radius: 18px; max-width: 24rem;">

                        <div style="margin:0 auto;padding: 1.563rem; z-index: 10; display: block; width: 100%; height: auto; background: #fff !important;">

                            <div id="AdminLoginResult_Desktop" runat="server" style="font-size: 1rem;font-weight:bold;color: #222;margin: 0 0 0.938rem;text-align: center;">
                                Admin Login
                            </div>

                            <!-- Login Name container -->
                            <div runat="server">
                                <label>
                                    Login Name
                                    <input id="txtUsername_Desktop" type="text" runat="server" autocomplete="off" value="" onclick="javascript: this.value = ''" />
                                </label>
                            </div>
                            <!-- Login Name container -->


                            <!-- Button container -->
                            <div style="border: solid 0 #ff0000; width: 100%; text-align: center;">
                                <input id="btnAdminOtpRequest_Desktop" type="button" onclick="javascript: submitOtpRequest('Desktop');" runat="server" value="Request OTP" class="button tiny radius" />
                            </div>
                            <!-- Button container -->


                            <div style="text-align: center;margin: 0.75rem 0 0;">
                                <a id="LinkUseExistingCredentials_Desktop" runat="server" href="javascript: useExistingCredentials('Desktop');">
                                    Use Existing Credentials
                                </a>
                            </div>

                            <div style="clear: both;"></div>

                        </div>

                    </div>
                </div>
                <div class="large-3 medium-3 small-12 columns hide-for-small">
                    &nbsp;
                </div>
            </div>
        </div>
    </div>
    <!--end show background-->

    <!--show background mobile-->
    <div class="show-for-small">
        <div style="background: #ccc url(/Images/bg_mobile.jpg) top center no-repeat; height: 575px;">
            <div class="row show-for-small" style="margin-top: 5px;">
                <div class="large-12 columns">
                    <div style="font: bold 1.5rem 'Helvetica Neue', 'Helvetica', Helvetica, Arial, 'Open Sans', sans-serif; color: #fff; text-align: center; padding: 2.5rem 0 1.35rem;">One-Time Password (OTP)</div>
                </div>
            </div>
            <div class="row" style="margin-top: 15px;">
                <div class="large-3 medium-3 small-12 columns hide-for-small">
                    &nbsp;
                </div>
                <div class="large-6 medium-6 small-12 columns">
                    <div style="margin: 0 auto; z-index: 0; padding: 0; border: 25px solid rgba(255, 255, 255, 0.25); border-radius: 18px;max-width:24rem;">

                        <div style="margin: 0 auto; padding: 25px; z-index: 10; display: block; width: 100%; height: auto; background: #fff !important;">

                            <div id="AdminLoginResult_Mobile" runat="server" style="font-size: 1rem;font-weight:bold;color: #222;margin: 0 0 0.938rem;text-align: center;">
                                Admin Login
                            </div>

                            <!-- Login Name container -->
                            <div runat="server">
                                <label>
                                    Login Name
                                    <input id="txtUsername_Mobile" type="text" runat="server" autocomplete="off" value="" onclick="javascript: this.value = ''" />
                                </label>
                            </div>
                            <!-- Login Name container -->


                            <!-- Button container -->
                            <div style="border: solid 0 #ff0000; width: 100%; text-align: center;">
                                <input id="btnAdminOtpRequest_Mobile" type="button" onclick="javascript: submitOtpRequest('Mobile');" runat="server" value="Request OTP" class="button tiny radius" />
                            </div>
                            <!-- Button container -->


                            <div style="text-align: center;margin: 0 0 0.313rem;">
                                <a id="LinkUseExistingCredentials_Mobile" runat="server" href="javascript: useExistingCredentials('Mobile');">
                                    Use Existing Credentials
                                </a>
                            </div>

                            <div id="divIsMobile">&nbsp;</div>

                            <div style="clear: both;"></div>

                        </div>
                    </div>
                </div>
                <div class="large-3 medium-3 small-12 columns hide-for-small">
                    &nbsp;
                </div>
            </div>
        </div>
    </div>
    <!--end show background mobile-->

    <input type="hidden" id="hiddenY" runat="server" value="" />
    <input type="hidden" id="hiddenZ" runat="server" value="" />
    <input type="hidden" id="hiddenAA" runat="server" value="" />

    <input type="hidden" id="hiddenAB" runat="server" value="" />
    <input type="hidden" id="hiddenAC" runat="server" value="" />

</asp:Content>

