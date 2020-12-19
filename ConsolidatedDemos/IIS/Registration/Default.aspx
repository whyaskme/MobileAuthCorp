<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Registration_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>MAC Registration</title>
    <%--<link href="../../css/jquery-ui-smoothness.css" rel="stylesheet" />
    <link href="../../css/jquery-ui-smoothness.css" rel="stylesheet" />--%>
    
    <%--<script src="../../js/jquery.timer.js"></script>
    <script src="../../js/jquery.validate.js"></script>--%>

    <link href="../css/Admin.css" rel="stylesheet" />
    <%--<link href="../../css/table-style.css" rel="stylesheet" />--%>

    <%--<link rel="stylesheet" href="../../css/style.css" />
    <link rel="stylesheet" href="../../css/prism.css" />
    <link rel="stylesheet" href="../../css/chosen.css" />--%>

    <%--<link rel="shortcut icon" href="../../Images/favicon.ico" />--%>
    <link rel="stylesheet" href="../css/foundation_menu.css" />
    
    <script src="../js/jquery-1.10.2.js"></script>
    <script src="../js/jquery-ui-1-11-0.js"></script>
    <script type="text/javascript" src="../js/Constants.js"></script>
    <script type="text/javascript" src="../js/golfShop_main.js"></script>
    <script type="text/javascript" src="../js/jquery.cookie.js"></script>
    <script src="../js/golfShop_shoppingCart.js"></script>
</head>
    <body>
    <form id="form1" runat="server">
        <div>
            <div id="divRegForm" runat="server">
                <div class="row">
                    <div class="large-12 columns">
                        <img src="../GolfShop/img/mac-logo_80.png" alt="Mobile Authentication Corporation" style="margin: 0.5rem 0 1.25rem;" />
                    </div>
                </div>
                <div class="row" id="messageRow" onclick="javascript: hidePanel();">
                    <div class="large-12 columns" style="text-align: center;">
                        <div class="panel callout radius" style="margin-bottom: 0.25rem;">
                            <label><asp:Label ID="lblMessage" runat="server" ForeColor="#222" Text="" /></label>                            
                        </div>
                    </div>
                </div>
                <div class="row" id="scroll2">
                    <div class="large-12 columns">
                        <fieldset><legend><span class="label_875"><asp:Label ID="registrationMessage" runat="server" Text="" /></span></legend>
                            <div class="row">
                                <div class="large-12 columns" style="margin-bottom: 0.75rem;">
                                    <label id="registrationError" runat="server" style="color:#f00;"></label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="large-6 medium-6 small-12 columns">
                                    <label title="Please enter your First Name.">First Name *</label>
                                        <%--<asp:TextBox ID="txtFirstName" runat="server" />--%>
                                        <input id="txtFirstName" lbl="First Name" isrequired="true" isvalid="false" min-length="1" max-length="75" 
                                            matchpattern="^([A-Za-z\d\s.,!-_#()]+)$" patterndescription="A-Z, a-z, 0-9, space, .,!-_#()" type="text" runat="server" 
                                            onblur="javascript: validateFormFields(this);" 
                                            onkeyup="javascript: validateFormFields(this);"
                                            onchange="javascript: validateFormFields(this);" />
                                </div>
                                <div class="large-6 medium-6 small-12 columns">
                                    <label title="Please enter your Last Name.">Last Name *</label>
                                        <%--<asp:TextBox ID="txtLastName" runat="server" />--%>
                                        <input id="txtLastName" lbl="Last Name" isrequired="true" isvalid="false" min-length="1" max-length="75" 
                                            matchpattern="^([A-Za-z\d\s.,!-_#()]+)$" patterndescription="A-Z, a-z, 0-9, space, .,!-_#()" type="text" runat="server" 
                                            onblur="javascript: validateFormFields(this);" 
                                            onkeyup="javascript: validateFormFields(this);"
                                            onchange="javascript: validateFormFields(this);" />
                                </div>
                            </div>
                            <div class="row">
                                <div class="large-6 medium-6 small-12 columns">
                                    <label title="Please enter the name of your Company.">Company *</label>
                                        <input id="txtCompany" lbl="Company" isrequired="true" isvalid="false" min-length="1" max-length="75" 
                                            matchpattern="^([A-Za-z\d\s.,!-_#()]+)$" patterndescription="A-Z, a-z, 0-9, space, .,!-_#()" type="text" runat="server" 
                                            onblur="javascript: validateFormFields(this);" 
                                            onkeyup="javascript: validateFormFields(this);"
                                            onchange="javascript: validateFormFields(this);" />
                                        <%--<asp:TextBox ID="txtCompany" runat="server" />--%>
                                </div>
                                <div class="large-6 medium-6 small-12 columns">
                                    <label title="Please enter your Job Title.">Job Title *</label>
                                        <input id="txtJobTitle" lbl="Job Title" isrequired="true" isvalid="false" min-length="1" max-length="75" 
                                            matchpattern="^([A-Za-z\d\s.,!-_#()]+)$" patterndescription="A-Z, a-z, 0-9, space, .,!-_#()" type="text" runat="server" 
                                            onblur="javascript: validateFormFields(this);" 
                                            onkeyup="javascript: validateFormFields(this);"
                                            onchange="javascript: validateFormFields(this);" />
                                        <%--<asp:TextBox ID="txtJobTitle" runat="server" />--%>
                                </div>
                            </div>
                            <div class="row">
                                <div class="large-6 medium-6 small-12 columns">
                                    <label title="Please enter a valid mobile phone number that can receive text messages.">Mobile Phone *</label>
                                        <%--<asp:TextBox ID="txtMPhoneNo" runat="server" />--%>
                                        <input id="txtMPhoneNo" lbl="Mobile Phone" isrequired="true" isvalid="false" min-length="10" max-length="12" 
                                            matchpattern="^\s*(?:\+?(\d{1,3}))?[-. (]*(\d{3})[-. )]*(\d{3})[-. ]*(\d{4})(?: *x(\d+))?\s*$" patterndescription="###-###-####" type="text" runat="server" 
                                            onblur="javascript: validateFormFields(this);" 
                                            onkeyup="javascript: validateFormFields(this);"
                                            onchange="javascript: validateFormFields(this);" />
                                </div>
                                <div class="large-6 medium-6 small-12 columns">
                                    <label title="Please enter a valid Email Address.">Email *</label>
                                        <%--<asp:TextBox ID="txtEmailAdr" runat="server" />--%>
                                        <input id="txtEmailAdr" lbl="Email" isrequired="true" isvalid="false" min-length="6" max-length="50" 
                                            matchpattern="^([A-Za-z0-9_\.-]+)@([\da-z\.-]+)\.([A-Za-z\.]{2,6})$" patterndescription="a valid email address" type="text" runat="server" 
                                            onblur="javascript: validateFormFields(this);" 
                                            onkeyup="javascript: validateFormFields(this);"
                                            onchange="javascript: validateFormFields(this);" />
                                </div>
                            </div>

                            <%--error message--%>
                            <div class="row">
                                <div class="large-12 columns">
                                    <label><asp:Label ID="lbError" runat="server" ForeColor="Red" Text="" /></label>
                                </div>
                            </div>

                            <div style="padding: 0.25rem;"></div>
                            <div class="row">
                                <div class="large-3 medium-3 small-12 columns">
                                    <asp:Image ID="Image1" runat="server" CssClass="captchaImage captchaResize" ImageUrl="CImage.aspx"/>
                                    <div style="padding: 0.5rem;"></div>
                                    <label>Enter Image Code *
                                    <%--<asp:TextBox ID="txtimgcode" runat="server"></asp:TextBox>--%>
                                    <input type="text" id="txtimgcode" runat="server" onchange="javascript: acceptTermsAndConditions();" onfocus="javascript: acceptTermsAndConditions();" onblur="javascript: acceptTermsAndConditions();" onkeypress="javascript: acceptTermsAndConditions();" onforminput="javascript: acceptTermsAndConditions();" />
                                    </label>
                                    <div style="padding: 0.125rem;"></div>
                                </div>
                                <div class="large-9 medium-9 small-12 columns hide-for-small">
                                    &nbsp;                       
                                </div>
                            </div>
                        
                            <div style="padding: 0.25rem;"></div>
                            <div class="row">
                                <div class="large-12 columns">
                                    <input id="chkTermsAndConditions" type="checkbox" value="" onchange="javascript: acceptTermsAndConditions();" /> <label>I have read and agree to the <a href="http://www.mobileauthcorp.com/MAC-Terms-and-Conditions-43458.pdf" target="_blank" style="white-space: nowrap;">Terms and Conditions</a></label>
                                </div>
                            </div>

                            <%--captcha error message--%>
                            <%--<div class="row">
                                <div class="large-12 columns">
                                    <label><asp:Label ID="lblmsg" runat="server" ForeColor="Red" Text="asdas" /></label>
                                </div>
                            </div>--%>

                            <div style="padding: 0.75rem;"></div>
                            <div class="row">
                                <div class="large-12 columns">
                                    <%--<asp:Button CssClass="button tiny radius" ID="btnRegisterEndUser" runat="server" Text="Register" CausesValidaion="false" OnClientClick="validateRegistrationForm();" OnClick="btnRegisterEndUser_Click" tooltip=""/>--%>
                                    <%--<input class="button tiny radius" type="button" id="btnRegisterEndUser" runat="server" value="Register" onclick="validateRegistrationForm();" title=""/>--%>
                                    <asp:Button CssClass="button tiny radius spacerRight125" ID="btnRegisterEndUser" runat="server" Text="Register" CausesValidaion="false" OnClientClick="validateRegistrationForm();" />
                                    <asp:Button CssClass="button tiny radius" ID="btnTestNavBack" runat="server" Text="Cancel" OnClick="btnTestNavBack_Click" />
                                </div>
                            </div>

                            <div style="padding: 0.75rem;"></div>
                            <div class="row">
                                <div class="large-12 columns">
                                    <label style="font-size: 0.625rem;">* Required Field</label>
                                </div>
                            </div>
                        </fieldset>
                    </div>
                </div>
            </div>

            <%--Text Notifications--%>
            <div id="divStopReset" runat="server">  
                <div class="row">
                    <div class="large-12 columns">
                        <img src="../GolfShop/img/mac-logo_80.png" alt="Mobile Authentication Corporation" style="margin: 0.5rem 0 1.25rem;" />
                    </div>
                </div>
                <%--<div class="row" id="messageRowStopReset" onclick="javascript: hidePanel();">
                    <div class="large-12 columns" style="text-align: center;">
                        <div class="panel callout radius" style="margin-bottom: 0.25rem;">
                            <label>Please submit this form to reset STOP</label>
                        </div>
                    </div>
                </div>--%>
                <div class="row" id="scroll3">
                    <div class="large-12 columns">
                        <fieldset><legend><span class="label_875"><asp:Label ID="stopResetMessage" runat="server" Text="Text Notifications" /></span></legend>
                            <div class="row">
                                <div class="large-12 columns" style="margin-bottom: 0.25rem;">
                                    <label id="stopResetError" runat="server" style="color:#f00;"></label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="large-12 columns" style="margin-bottom:0.25rem;">
                                    <p style="margin-top:0;"><label>User <strong id="userID" runat="server">UserID</strong> has requested to <strong>STOP</strong> text notifications.</label></p>
                                    <p style="margin-bottom:0;"><label>Please text <strong>OPTIN</strong> to <strong id="textNotification" runat="server">43458</strong> to reactivate.</label></p>
                                </div>
                            </div>
                            <%--<div class="row">
                                <div class="large-6 medium-6 small-12 columns" style="margin-bottom:0.25rem;">
                                    <label id="rs_firstName" runat="server" style="font-weight:bold;">First Name</label>
                                </div>
                                <div class="large-6 medium-6 small-12 columns" style="margin-bottom:0.25rem;">
                                    <label id="rs_lastName" runat="server" style="font-weight:bold;">Last Name</label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="large-6 medium-6 small-12 columns" style="margin-top:0.25rem;margin-bottom:0.25rem;">
                                    <label id="rs_company" runat="server" style="font-weight:bold;">Company</label>
                                </div>
                                <div class="large-6 medium-6 small-12 columns" style="margin-top:0.25rem;margin-bottom:0.25rem;">
                                    <label id="rs_jobTitle" runat="server" style="font-weight:bold;">Job Title</label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="large-6 medium-6 small-12 columns" style="margin-top:0.25rem;margin-bottom:0.25rem;">
                                    <label id="rs_mobilePhone" runat="server" style="font-weight:bold;">Mobile Phone</label>
                                </div>
                                <div class="large-6 medium-6 small-12 columns" style="margin-top:0.25rem;margin-bottom:0.25rem;">
                                    <label id="rs_email" runat="server" style="font-weight:bold;">Email</label>
                                </div>
                            </div>

                            <div style="padding: 0.5rem;"></div>--%>

                            <%--error message--%>
                            <%--<div class="row">
                                <div class="large-12 columns">
                                    <label><asp:Label ID="lberror1" runat="server" ForeColor="Red" Text="" /></label>
                                </div>
                            </div>

                            <div style="padding: 0.25rem;"></div>
                            <div class="row">
                                <div class="large-3 medium-3 small-12 columns">
                                    <asp:Image ID="Image2" runat="server" CssClass="captchaImage captchaResize" ImageUrl="CImage.aspx"/>
                                    <div style="padding: 0.5rem;"></div>
                                    <label>Enter Image Code *
                                    <input type="text" id="txtimgcode1" runat="server" onchange="javascript: acceptTermsAndConditions();" onfocus="javascript: acceptTermsAndConditions();" onblur="javascript: acceptTermsAndConditions();" onkeypress="javascript: acceptTermsAndConditions();" onforminput="javascript: acceptTermsAndConditions();" />
                                    </label>
                                    <div style="padding: 0.125rem;"></div>
                                </div>
                                <div class="large-9 medium-9 small-12 columns hide-for-small">
                                    &nbsp;                       
                                </div>
                            </div>
                        
                            <div style="padding: 0.25rem;"></div>
                            <div class="row">
                                <div class="large-12 columns">
                                    <input id="chkTermsAndConditions1" type="checkbox" value="" onchange="javascript: acceptTermsAndConditions();" /> <label>I have read and agree to the <a href="http://www.mobileauthcorp.com/MAC-Terms-and-Conditions-43458.pdf" target="_blank" style="white-space: nowrap;">Terms and Conditions</a></label>
                                </div>
                            </div>--%>

                            <div style="padding: 0.75rem;"></div>
                            <div class="row">
                                <div class="large-12 columns">
                                    <%--<asp:Button CssClass="button tiny radius" ID="btnSubmitResetStop" runat="server" Text="Submit" CausesValidaion="false" OnClientClick="validateRegistrationForm();" />--%>
                                    <asp:Button CssClass="button tiny radius" ID="Button3" runat="server" Text="Back" OnClick="btnTestNavBack_Click" />
                                </div>
                            </div>

                            <%--<div style="padding: 0.75rem;"></div>
                            <div class="row">
                                <div class="large-12 columns">
                                    <label style="font-size: 0.625rem;">* Required Field</label>
                                </div>
                            </div>--%>
                        </fieldset>
                    </div>
                </div>
            </div>
            <%--End Text Notifications--%>
                
            <div id="divCfgForm" runat="server">  
                <div class="row">
                    <div class="large-12 columns">
                        <img src="../GolfShop/img/mac-logo_80.png" alt="Mobile Authentication Corporation" style="margin: 0.5rem 0;" />
                    </div>
                </div>
                <div class="row" id="scroll4">
                    <div class="large-12 columns">
                        <fieldset><legend><span class="label_875">Client Administration</span></legend>
                            <div class="row">
                                <div class="large-6 medium-6 small-12 columns">
                                    <label>MAC Test Bank URL
                                        <asp:TextBox ID="txtMacBankUrl" runat="server" />
                                    </label>
                                </div>
                                <div class="large-6 medium-6 small-12 columns">
                                    <label>RegistrationType
                                        <asp:TextBox ID="txtRegType" runat="server" />
                                    </label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="large-6 medium-6 small-12 columns">
                                    <label>Email Server
                                        <asp:TextBox ID="txtEmailServer" runat="server" />
                                    </label>
                                </div>
                                <div class="large-6 medium-6 small-12 columns">
                                    <label>Email Port
                                        <asp:TextBox ID="txtEmailPort" runat="server" />
                                    </label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="large-6 medium-6 small-12 columns">
                                    <label>Email Login User Name
                                        <asp:TextBox ID="txtEmailLoginUserName" runat="server" />
                                    </label>
                                </div>
                                <div class="large-6 medium-6 small-12 columns">
                                    <label>Email Password
                                        <asp:TextBox ID="txtEmailPassword" runat="server" />
                                    </label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="large-6 medium-6 small-12 columns">
                                    <label>Email From Address
                                        <asp:TextBox ID="txtEmailFromAddress" runat="server" />
                                    </label>
                                </div>
                                <div class="large-6 medium-6 small-12 columns">
                                    <label>Email To Address
                                        <asp:TextBox ID="txtEmailToAddress" runat="server" />
                                    </label>
                                </div>
                            </div>

                            <div style="padding: 0.25rem;"></div>
                            <div class="row">
                                <div class="large-12 columns">
                                    <asp:Button CssClass="button tiny radius spacerRight125" ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" tooltip=""/>
                                    <asp:Button CssClass="button tiny radius" ID="btnBackCfg" runat="server" Text="Cancel" OnClick="btnTestNavBack_Click" /> 
                                </div>
                            </div>
                        </fieldset>
                    </div>
                </div>
            </div>

            <!-- unregister -->
            <div id="divUnregister" runat="server">  
                <div class="row">
                    <div class="large-12 columns">
                        <img src="../GolfShop/img/mac-logo_80.png" alt="Mobile Authentication Corporation" style="margin: 0.5rem 0;" />
                    </div>
                </div>
                <div class="row" id="scroll5">
                    <div class="large-12 columns">
                        <fieldset><legend><span id="legendLabel" runat="server" class="label_875">Unsubscribe</span></legend>

                            <div id="divUnsubscribe" runat="server">
                                <div class="row">
                                    <div class="large-12 columns">
                                        <div style="margin: 0 auto 0.75rem;padding: 0;max-width: 24rem;">
                                            <div runat="server">
                                                <label>
                                                    Enter Email
                                                    <input id="txtUnsubscribe" lbl="Email" isrequired="true" isvalid="false" min-length="7" max-length="50" 
                                                        matchpattern="^([A-Za-z0-9_\.-]+)@([\da-z\.-]+)\.([A-Za-z\.]{2,6})$" patterndescription="a valid email address" type="text" runat="server" 
                                                        onblur="javascript: validateUnsubscribeFormFields(this);" 
                                                        onkeyup="javascript: validateUnsubscribeFormFields(this);"
                                                        onchange="javascript: validateUnsubscribeFormFields(this);" />
                                                </label>
                                            </div>
                                            <div runat="server" style="text-align: center;">
                                                <asp:Label ID="lbError3" runat="server" ForeColor="Red" Text="" />
                                            </div>
                                            <!-- Button container -->
                                            <div style="text-align: center;margin-top: 0.75rem;">
                                                <asp:Button CssClass="button tiny radius spacerRight125" ID="btnUnsubscribe" Enabled="false" runat="server" Text="Submit" OnClick="btnUnsubscribe_Click" />
                                                <asp:Button CssClass="button tiny radius" ID="btnToRegister" runat="server" Text="Cancel" OnClick="btnHome_Click" />
                                            </div>
                                            <div style="clear: both;"></div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <!-- successfully unsubscribed div -->
                            <div id="divUnsubscribeMessage" style="position: relative;margin: 0 auto;" runat="server">
                                <div class="row">
                                    <div class="large-12 columns" style="text-align: center;">
                                        <h1 style="font-size: 2.5rem;color: #222;white-space: nowrap;">Success!</h1>
                                        <p style="line-height: 1.5rem;"><label style="color: #222;">You have successfully removed <strong><span id="deletedEmail" runat="server">xxxx</span></strong> from the demo system.</label></p>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="large-12 columns" style="margin-bottom: 1rem;text-align: center;">
                                        <asp:Button CssClass="button tiny radius" ID="btnHome1" runat="server" Text="Home" OnClick="btnHome_Click" />
                                    </div>
                                </div>
                            </div>
                            <!-- end successfully unsubscribed div -->

                        </fieldset>
                    </div>
                </div>
            </div>
            <!-- end unregister -->
        
            <!--disclaimer-->
            <%--<div class="row">
                <div class="large-12 columns">
                    <fieldset><legend>Disclaimer</legend>
                        <div class="copyright" style="line-height: 0.875rem;">
                                <strong>MAC Disclosure Statement Short Code Text Messages</strong><br />
                                You understand and agree that by registering you are opting-in to receive a One-Time Password (OTP) via "text" or "voice" message. Opt-out via "Reply Text" is not available. If you decide you no longer want to subscribe "Opt-out" click here: Demo Options, fill out the Demo Options form and click the "Remove Me" button to unsubscribe.
                            <div style="padding: 0.25rem;"></div>
                                You may receive ads in the body of the OTP "text" message or on the Enter OTP page. You may opt-out of receiving the ads by clicking here: Demo Options, fill out the Demo Options form and click the "STOP Ads" button to cancel receipt of ads.
                            <div style="padding: 0.25rem;"></div>
                                If you have any questions regarding the terms and conditions or the OTP messaging options described above, please email <a href="mailto:info@mobileauthcorp.com" class="copyright">info@mobileauthcorp.com</a> or call 1-800-939-2980.
                            <div style="padding: 0.25rem;"></div>
                        </div>
                    </fieldset>
                </div>
            </div>--%>
            <%--<div class="row">
                <div class="large-12 columns">
                    <fieldset><legend>MAC Disclosure Statement</legend>
                        <div class="copyright" style="line-height: 1.125rem;">
                            <strong>Program Sponsor:</strong> Welcome to MAC Alerts and Notifications!<br />
                            <strong>Service Description:</strong> MAC sends One-Time Passwords to you as enhanced security when you use debit, credit or gift cards to make purchases.<br />
                            <strong>Frequency:</strong> Msg frequency depends upon user.<br />
                            <strong>Customer Support:</strong> Text <strong>HELP</strong> to 43458 or 62667 for assistance.<br />
                            <strong>Opt Out Info:</strong> Text <strong>STOP</strong> to 43458 or 62667 to cancel.<br />
                            <strong>Additional Carrier Costs:</strong> Msg & Data Rates May Apply.<br />
                            <strong>Participating Carriers:</strong> ACG, ALLTEL AWCC, AT&T Mobility, Boost, Cincinnati Bell, Cricket, Google Voice, Metro PCS, Rural Carrier Groups, Sprint, Tier 2/3 Carrier Group, T-Mobile, U.S. Cellular, Verizon Wireless, Virgin Mobile.<br />
                            <div style="padding: 0.5rem;"></div>
                            <strong>Terms and Conditions:</strong> <a href="http://www.otp-ap.com/MAC-Terms-and-Conditions.pdf" target="_blank">www.mobileauthcorp.com/terms</a><br />
                            <strong>Privacy Policy:</strong> <a href="http://www.otp-ap.com/MAC-Privacy-Policy.pdf" target="_blank">www.mobileauthcorp.com/privacy</a>
                            <%--<div style="padding: 0.5rem;"></div>
                            <span style="font-size: 1rem;font-weight: bold;">To unsubscribe, <a href="#" onServerClick="btnGoToUnsubscribe_Click" runat="server" style="font-size: 1rem;font-weight: bold;">click here</a></span>--%>
                        <%--</div>
                    </fieldset>
                </div>
            </div>--%>
            <div class="row hide-for-small-down">
                <div class="large-12 columns">
                    <fieldset><legend>Disclosure</legend>
                        <div class="registrationDisclaimer" style="line-height: 1.125rem;">
                            <div style="padding:0 0 0.5rem;">
                                <strong>This demonstration website ("Demo") is the copyrighted work of Mobile Authentication Corporation (MAC).</strong>
                            </div>
                            <strong>MAC Alerts and Notifications</strong><br />
                            MAC sends One-Time Passwords as enhanced security when you use debit, credit or gift cards to make purchases. 
                            Text <strong>STOP</strong> to 43458<%-- or 62667--%> to cancel. 
                            Text <strong>HELP</strong> to 43458<%-- or 62667--%> for assistance. 
                            For additional information, please call 1-844-427-0411 or email <a href="mailto:info@mobileauthcorp.com">info@mobileauthcorp.com</a>. 
                            Message frequency depends on user. Message and data rates may apply. Participating carriers include: ACG, ALLTEL AWCC, AT&amp;T Mobility, Boost, Cincinnati Bell, Cricket, Google Voice, Metro PCS, Nextel, Rural Carrier Groups, Sprint, Tier 2/3 Carrier Group, T-Mobile, U.S. Cellular, Verizon Wireless and Virgin Mobile.
                            <div style="padding: 0.25rem;"></div>
                            <strong>Terms and Conditions:</strong> <a href="http://www.mobileauthcorp.com/MAC-Terms-and-Conditions-43458.pdf" style="color:#0362a6 !important;" target="_blank">www.mobileauthcorp.com/terms</a><br />
                            <strong>Privacy Policy:</strong> <a href="http://www.mobileauthcorp.com/MAC-Privacy-Policy.pdf" style="color:#0362a6 !important;" target="_blank">www.mobileauthcorp.com/privacy</a>
                        </div>
                    </fieldset>
                </div>
            </div>

            <div class="row show-for-small-down">
                <div class="large-12 columns">
                    <fieldset><legend>Disclosure</legend>
                        <div class="registrationDisclaimer" style="line-height: 1.125rem;">
                            <div style="padding:0 0 0.5rem;">
                                <strong>This demonstration website ("Demo") is the copyrighted work of Mobile Authentication Corporation (MAC).</strong>
                            </div>
                            <strong>MAC Alerts and Notifications</strong><br />
                            MAC sends One-Time Passwords as enhanced security when you use debit, credit or gift cards to make purchases. 
                            <div style="padding: 0.25rem;"></div>
                            Text <strong>STOP</strong> to 43458<%-- or 62667--%> to cancel.<br />
                            Text <strong>HELP</strong> to 43458<%-- or 62667--%> for assistance.<br />
                            <div style="padding: 0.25rem;"></div> 
                            For additional information,<br />
                            please call 1-844-427-0411<br />
                            or email <a href="mailto:info@mobileauthcorp.com">info@mobileauthcorp.com</a>. 
                            <div style="padding: 0.25rem;"></div>
                            Message frequency depends on user. Message and data rates may apply. Participating carriers include: ACG, ALLTEL AWCC, AT&amp;T Mobility, Boost, Cincinnati Bell, Cricket, Google Voice, Metro PCS, Nextel, Rural Carrier Groups, Sprint, Tier 2/3 Carrier Group, T-Mobile, U.S. Cellular, Verizon Wireless and Virgin Mobile.
                            <div style="padding: 0.25rem;"></div>
                            <strong>Terms and Conditions:</strong><br />
                            <a href="http://www.mobileauthcorp.com/MAC-Terms-and-Conditions-43458.pdf" style="color:#0362a6 !important;" target="_blank">www.mobileauthcorp.com/terms</a><br />
                            <div style="padding: 0.25rem;"></div>
                            <strong>Privacy Policy:</strong><br />
                            <a href="http://www.mobileauthcorp.com/MAC-Privacy-Policy.pdf" style="color:#0362a6 !important;" target="_blank">www.mobileauthcorp.com/privacy</a>
                        </div>
                    </fieldset>
                </div>
            </div>
          
            <div style="padding: 0.5rem;"></div>

            <div class="row hide-for-small">
                <div class="large-12 columns">
                    <div class="copyright" style="font-size: 0.719rem;">
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
            <div class="row show-for-small">
                <div class="large-12 columns">
                    <div class="copyright" style="font-size: 0.719rem;">
                        <script type="text/javascript">
                            <!--
                            var currentDate = new Date();
                            var year = currentDate.getFullYear();
                            document.write("&copy; " + year + " Mobile Authentication Corporation." + "<br />" + "All rights reserved.");
                            //-->
                        </script>
                    </div>
                </div>
            </div>
            
            <input id="hiddenLastName" runat="server" type="hidden" value="" />
            <input id="hiddenLoginName" runat="server" type="hidden" value="" />

            <input id="panelFocusClients" runat="server" type="hidden" value="" />
            <input id="hiddenCID" runat="server" type="hidden" value="" />
            <input id="hiddenDemo" runat="server" type="hidden" value="" />
            <input id="hiddenT" runat="server" type="hidden" value="" />
            <input id="hiddenE" runat="server" type="hidden" value="" />
            <input id="hiddenShortCode" runat="server" type="hidden" value="" />
            <input id="hiddenCallerURL" runat="server" type="hidden" value="" />
            <input id="hiddenMACBankUrl" runat="server" type="hidden" value="" />
            <input id="hiddenRegistrationType" runat="server" type="hidden" value="" />
            <input id="hiddenLN" runat="server" type="hidden" value="" />
            <input id="hiddenHost" runat="server" type="hidden" value="" />
            
            <input id="hiddenAA" runat="server" type="hidden" value="" />
            <input id="hiddenValidation" runat="server" type="hidden" value="false" />
            
            <!-- Send Email Parameters --> 
            <input id="hiddenEmailServer" runat="server" type="hidden" value="false" />
            <input id="hiddenEmailPort" runat="server" type="hidden" value="false" />
            <input id="hiddenEmailToAddress" runat="server" type="hidden" value="false" />
            <input id="hiddenEmailFromAddress" runat="server" type="hidden" value="false" />
            <input id="hiddenEmailLoginUserName" runat="server" type="hidden" value="false" />
            <input id="hiddenEmailPassword" runat="server" type="hidden" value="false" />

        </div>
    </form>
    <script>
        // dynamic registration label
        var demoName = $("#hiddenDemo").val();
        $("#registrationMessage").html(demoName + " Registration");
        $("#lblMessage").html("Please complete the registration form to access the <strong>" + demoName + "</strong> demo.");

        // disable register button
        $("#btnRegisterEndUser").attr("disabled", true);

    </script>
</body>
</html>

