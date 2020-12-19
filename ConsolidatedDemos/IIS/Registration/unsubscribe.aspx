<%@ Page Language="C#" AutoEventWireup="true" CodeFile="unsubscribe.aspx.cs" Inherits="Registration_unsubscribe" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>MAC Unsubscribe</title>

    <link href="../css/Admin.css" rel="stylesheet" />
    <link rel="stylesheet" href="../css/foundation_menu.css" />
    
    <script src="../js/jquery-1.10.2.js"></script>
    <script src="../js/jquery-ui-1-11-0.js"></script>
    <script type="text/javascript" src="../js/Constants.js"></script>
    <script type="text/javascript" src="../js/MACSystemAdmin-Responsive.js"></script>
    <script type="text/javascript" src="../js/jquery.cookie.js"></script>
    <script src="../js/Demo.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="row">
            <div class="large-12 columns">
                <img src="../GolfShop/img/mac-logo_80.png" alt="Mobile Authentication Corporation" style="margin: 0.5rem 0 1.25rem;" />
            </div>
        </div>
        <div class="row" id="messageRow" onclick="javascript: hidePanel();">
            <div class="large-12 columns" style="text-align: center;">
                <div class="panel callout radius" style="margin-bottom: 0.25rem;">
                    <label><asp:Label ID="lblMessage" runat="server" ForeColor="#222" Text="Enter your email address below to remove yourself from <span style='white-space: nowrap;'> the demo system.</span>" /></label>                            
                </div>
            </div>
        </div>
        <!--Unsubscribe-->
        
            <div class="row">
                <div class="large-12 columns">
                    <fieldset><legend><span id="legendLabel" runat="server" class="label_875">Unsubscribe</span></legend>

                        <div id="divUnsubscribe" runat="server">
                            <div class="row">
                                <div class="large-12 columns">
                                    <div style="margin: 0 auto 0.75rem;padding: 0;max-width: 24rem;">
                                        <div runat="server">
                                            <label>
                                                Enter Email
                                                <input id="txtUnsubscribe" lbl="Email" isrequired="true" isvalid="false" min-length="7" max-length="50" matchpattern="^([A-Za-z0-9_\.-]+)@([\da-z\.-]+)\.([A-Za-z\.]{2,6})$" patterndescription="a valid email address" type="text" runat="server" onblur="javascript: validateClientFormField(this);" onkeyup="javascript: validateClientFormField(this);" />
                                            </label>
                                        </div>
                                        <div runat="server" style="text-align: center;">
                                            <asp:Label ID="lbError3" runat="server" ForeColor="Red" Text="" />
                                        </div>
                                        <!-- Button container -->
                                        <div style="text-align: center;margin-top: 0.75rem;">
                                            <asp:Button CssClass="button tiny radius" ID="btnUnsubscribe" Enabled="false" runat="server" Text="Submit" onclick="btnUnsubscribe_Click" />
                                        </div>
                                        <div style="clear: both;"></div>
                                    </div>
                                </div>
                            </div>

                            <!-- successfully unsubscribed div -->
                            <div id="divUnsubscribeMessage" style="position: relative;margin: 0 auto;" runat="server">
                                <div class="row">
                                    <div class="large-12 columns" style="text-align: center;">
                                        <h1 style="font-size: 2.5rem;color: #222;white-space: nowrap;">Thank you!</h1>
                                        <p style="font-size: 1rem;color: #222;line-height: 1.5rem;">You have successfully removed "<span id="deletedEmail" runat="server">xxxx</span>" from the demo system.</p>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="large-12 columns" style="margin-bottom: 1rem;text-align: center;">
                                        <asp:Button CssClass="button tiny radius" ID="btnHome1" runat="server" Text="Home" OnClick="btnHome_Click" />
                                    </div>
                                </div>
                            </div>
                            <!-- end successfully unsubscribed div -->
                        </div>

                    </fieldset>
                </div>
            </div>

            <!-- disclaimer -->
            <%--<div class="row">
                <div class="large-12 columns">
                    <fieldset><legend style="background: none;">MAC Disclosure Statement</legend>
                        <div>
                            <p style="margin-top: 0;font-size:0.75rem;">
                                The information contained in this website is for general information purposes only. While we endeavor to keep the information up to date and correct, we make no representations or warranties of any kind, express or implied, about the completeness, accuracy, reliability, suitability or availability with respect to the website or the information, products, services, or related graphics contained on this website for any purpose. Any reliance you place on such information is therefore strictly at your own risk.
                            </p>
                            <p style="font-size:0.75rem;">
                                In no event will we be liable for any loss or damage including without limitation, indirect or consequential loss or damage, or any loss or damage whatsoever arising from loss of data or profits arising out of, or in connection with, the use of this website.
                            </p>
                            <p style="font-size:0.75rem;">
                                Links to third party websites are not under the control of Mobile Authentication Corporation. The inclusion of any links does not necessarily imply a recommendation or endorse the views expressed within them.
                            </p>
                            <p style="margin-bottom: 0;font-size:0.75rem;">
                                Every effort is made to keep the website up and running smoothly. However, Mobile Authentication Corporation will not be liable for the website being temporarily unavailable due to technical issues beyond our control.
                            </p>
                        </div>
                    </fieldset>
                </div>
            </div>--%>
            <!-- end disclaimer -->
            <div class="row">
                <div class="large-12 columns">
                    <div style="color: #222;text-align: left;font-size: 0.719rem;margin:1rem 0 2.5rem;">
                        <%--&copy; 2014 Scottsdale Golf Shop.--%>
                        <script type="text/javascript">
                            <!--
                            var currentDate = new Date();
                            var year = currentDate.getFullYear();
                            document.write("&copy; " + year + " Mobile Authentication Corporation. All rights reserved.");
                            //-->
                        </script>
                    </div>
                </div>
            </div>
        <!--End Unsubscribe-->

        <input id="hiddenLastName" runat="server" type="hidden" value="" />
        <input id="hiddenLoginName" runat="server" type="hidden" value="" />
        <input id="hiddenTrxDetails" runat="server" type="hidden" value="" />
        <input id="hiddenOTPType" runat="server" type="hidden" value="" />
        <input id="hiddenRequestId" runat="server" type="hidden" value="" />
        <input id="hiddenOTP" runat="server" type="hidden" value="" />
        <input id="hiddenT" runat="server" type="hidden" value="" />
        <input id="hiddenDemo" runat="server" type="hidden" value="" />
        <input id="hiddenCID" runat="server" type="hidden" value="" />
        <input id="hiddenGID" runat="server" type="hidden" value="" />        
        <input id="hiddenO" runat="server" type="hidden" value="" />
        <input id="hiddenMacServicesUrl" runat="server" type="hidden" value="" />
        <input id="hiddenMacbankUrl" runat="server" type="hidden" value="" />
        <input id="hiddenRegisterUrl" runat="server" type="hidden" value="" />
        <input id="hiddenContactMe" runat="server" type="hidden" value="" />
        <!--email validation-->
        <input id="hiddenValidation" runat="server" type="hidden" value="false" />
        
         <!-- Send Email Parameters -->
        <input id="hiddenEmailServer" runat="server" type="hidden" value="false" />
        <input id="hiddenEmailPort" runat="server" type="hidden" value="false" />
        <input id="hiddenEmailToAddress" runat="server" type="hidden" value="false" />
        <input id="hiddenEmailFromAddress" runat="server" type="hidden" value="false" />
        <input id="hiddenEmailLoginUserName" runat="server" type="hidden" value="false" />
        <input id="hiddenEmailPassword" runat="server" type="hidden" value="false" />


    </form>
</body>
</html>
