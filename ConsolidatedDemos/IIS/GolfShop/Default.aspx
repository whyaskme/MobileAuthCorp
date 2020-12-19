<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="GolfShop_Default" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml" class="no-js" lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <%--<meta name="viewport" content="width=device-width, initial-scale=1.0" />--%>
    <meta name="viewport" content="width=device-width, maximum-scale=1.0"  />
    <title>Welcome To The Scottsdale Golf Shop</title>
    <link href="../css/jquery-ui-smoothness.css" rel="stylesheet" />
    <link href="../css/Admin.css" rel="stylesheet" />
    <link href="../css/table-style.css" rel="stylesheet" />
    <link href="../css/style.css" rel="stylesheet" />
    <link href="../css/prism.css" rel="stylesheet" />
    <link href="../css/chosen.css" rel="stylesheet" />
    <link rel="stylesheet" href="../css/foundation_golf.css" />
    <script src="../js/vendor/modernizr.js"></script>
    <script src="../js/jquery-1.10.2.js"></script>
    <script src="../js/jquery-ui-1-11-0.js"></script>
    <script src="../js/golfShop_main.js"></script>
    <script src="../js/golfShop_shoppingCart.js"></script>
    <link href='http://fonts.googleapis.com/css?family=Roboto+Slab' rel='stylesheet' type='text/css' />
    <style>
        .disclaimerLink {
            color:#e5dd40 !important;
        }
    </style>
</head>
   <body style="width:100% !important;">
       <!--loading lightbox-->
        <div id="divPleaseWaitProcessing" class="lightbox_background" >
            <div id="divDialogContainer" class="lightbox_content">
                <div id="circularG">
                    <div id="circularG_1" class="circularG"></div>
                    <div id="circularG_2" class="circularG"></div>
                    <div id="circularG_3" class="circularG"></div>
                    <div id="circularG_4" class="circularG"></div>
                    <div id="circularG_5" class="circularG"></div>
                    <div id="circularG_6" class="circularG"></div>
                    <div id="circularG_7" class="circularG"></div>
                    <div id="circularG_8" class="circularG"></div>
                </div>
            </div>
        </div>

        <!--iframe popup lightbox-->
        <div id="divPleaseWaitProcessing_popup" class="lightbox_background_popup"></div>
        <div id="divDialogContainer_popup" class="lightbox_content_popup"></div>

    <div class="contain-to-grid sticky">
      <nav class="top-bar">
        <ul class="title-area">
          <li class="name">
            <a href="#" id="btnTitleAreaHome" onServerClick="btnHome_Click" runat="server" title="Home"><img src="img/golf-logo.png" alt="" style="margin: 13px 0 0 10px;" /></a>
          </li>
          <!-- Remove the class "menu-icon" to get rid of menu icon. Take out "Menu" to just have icon alone -->
          <!--<li class="toggle-topbar menu-icon"><a href="#"><span>Menu</span></a></li>-->
          <li class="toggle-topbar menu-icon"><a href="#"><img src="img/menu-icon1.png" alt="" style="position:relative;top:-8px;left:35px;" title="Menu" /></a></li>
        </ul>

        <section class="top-bar-section">
          <!-- Right Nav Section -->
          <ul class="right">
            <li><a href="http://www.mobileauthcorp.com/" target="_blank" alt="MobileAuthCorp.com">MAC</a></li>
            <%--<li><a href="golf-store.aspx">Shopping</a></li>--%>
            <%--<li><a href="#">View Cart</a></li>
            <li><a href="#">Orders</a></li>
            <li class="active"><a href="#">My Account</a></li>--%>
            <!--<li><a href="myAccount.html">My Account</a></li>
            <li><a href="#">Logout</a></li>-->
            <!--<li><a href="#" style="background:#e57e80;">Logout</a></li>-->
            <!--<li class="has-dropdown">
              <a href="#">Right Button Dropdown</a>
              <ul class="dropdown">
                <li><a href="#">First link in dropdown</a></li>
              </ul>
            </li>-->
          </ul>
        </section>
        
      </nav>
    </div>

    <form id="formMain" runat="server" method="post" onsubmit="javascript: ShowProcessingMessage();">

        <!--login div content-->
        <div id="divLogin" runat="server" style="position: relative;margin: 0 auto;z-index: 0;">
            <div class="row">
                <div class="large-12 columns">
                    <%--<div id="main_image_container" style="margin:25px 0 0.75rem;display:block;width:100%;max-width:970px;height:280px;max-height:280px;background:url('../Images/bg_main.jpg') no-repeat center bottom;">--%>
                    <!-- login container -->
                    <div class="row bg-main-login" id="divLoginContainer" runat="server" style="">
                        <div class="large-8 medium-7 small-12 columns" style="text-align: center;">
                            <%--<h1 class="show-for-small" style="font-family: '    ', serif;font-size:1.7em;color: #fff;line-height:2rem;margin: 1.5rem 0 0;">Welcome to the<br />Scottsdale Golf Shop</h1>
                            <h1 class="show-for-medium" style="font-family: 'Roboto Slab', serif;font-size:2.125em;color: #fff;margin: 3.25rem 0 0;">Welcome to the<br />Scottsdale Golf Shop</h1>
                            <h1 class="hide-for-medium hide-for-small" style="font-family: 'Roboto Slab', serif;font-size:3em;color: #fff;margin: 2.5rem 0 0;">Welcome to the<br />Scottsdale Golf Shop</h1>

                            <h3 class="hide-for-medium hide-for-small" style="color: #fff;padding: 0;margin: 0.875rem 0 1rem !important;font-size: 1.5rem;font-style: italic;">Your No. 1 Source for Everything Golf!</h3>
                            <h3 class="show-for-medium" style="color: #fff;padding: 0;margin: 0.875rem 0 1rem !important;font-size: 1.125rem;font-style: italic;">Your No. 1 Source for Everything Golf!</h3>
                            <h3 class="show-for-small" style="color: #fff;padding: 0;margin: 0.75rem 0 0 !important;font-size: 0.95rem;font-style: italic;">Your No. 1 Source for Everything Golf!</h3>--%>
                        </div>
                        <div class="large-4 medium-5 small-12 columns">
                            <%--<div style="margin: 35px auto 15px;z-index: 0;padding: 0; border: 25px solid #ffffff; border: 25px solid rgba(255, 255, 255, 0.25); -ms-border-radius: 18px; border-radius: 18px; max-width: 24rem;">--%>
                            <div class="align-center-mobile" style="margin: 20px 12px 0;padding: 0; max-width: 24rem;border:1px solid #fff;background: #fff !important;">

                                <div style="margin:0 auto;padding: 0.75rem 1.563rem 0;display: block;height: auto;background: #fff !important;">

                                    <%--<div id="AdminLoginResult_Desktop" runat="server" style="font-size: 1rem;font-weight:bold;color: #222;margin: 0 0 0.5rem;text-align: center;">
                                        Demo Login
                                    </div>--%>

                                    <div runat="server">
                                        <label>
                                            Welcome
                                            <input id="txtLoginName" lbl="Email" isrequired="true" 
                                                isvalid="false" min-length="7" max-length="50" 
                                                matchpattern="^([A-Za-z0-9_\.-]+)@([\da-z\.-]+)\.([A-Za-z\.]{2,6})$" 
                                                patterndescription="a valid email address" type="text" runat="server"
                                                placeholder="Enter User ID" 
                                                onblur="javascript: validateFormFields(this);" 
                                                onkeyup="javascript: validateFormFields(this);"
                                                onchange="javascript: validateFormFields(this);" />
                                        </label>
                                    </div>

                                    <!-- Button container -->
                                    <div style="text-align: center;margin-top: 1rem;">
                                            <asp:Button CssClass="button tiny radius loginButton" runat="server" BackColor="#2d8c36" 
                                            ID="btnValidateLoginName" Text="Login" 
                                            CausesValidaion="false"
                                            OnClick="btnValidateLoginName_Click"
                                             />
                                        <div>
                                            <asp:Button CssClass="registerLink" runat="server" ID="btnToRegister" Text="Register" CausesValidaion="false" Enabled="true" OnClick="btnToRegister_Click"/>
                                            <span style="margin:0.25rem 0;font-size:0.625rem;font-weight:bold;color:#ccc;">|</span>
                                            <asp:Button CssClass="registerLink" runat="server" ID="Button2" Text="Unsubscribe" Enabled="true" OnClick="btnGoToUnsubscribe_Click"/>
                                        </div>
                                        <div class="adSelectMenu" style="margin:15px auto 20px;">
                                            <asp:DropDownList ID="dlAdType" runat="server" CssClass="chosen-select">
                                                <asp:ListItem Value="0">Select Ad</asp:ListItem>
                                                <asp:ListItem Value="1">Random</asp:ListItem>
                                                <asp:ListItem Value="2">Specials</asp:ListItem>
                                                <asp:ListItem Value="3">Tee Times</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                    <div style="clear: both;"></div>

                                </div>
                            </div>

                        </div>
                    </div>
                    <!-- end login container -->
                    <%--</div>--%>

                    <!-- login OTP container -->
                    <div class="row bg-main-otp" id="divLoginOTPContainer" runat="server">
                        <div class="" style="padding:0 !important;position:relative;z-index:100;">
                            <div id="otpMessage" runat="server" style="padding:0.5rem;font-size: 0.8125rem;color:#222;text-align:center;background:#e5dd40 !important;">
                                Please enter the one-time authentication code sent to your mobile phone.
                            </div>
                        </div>
                        <div class="large-8 medium-7 small-12 columns" style="text-align: center;">
                            <div>
                                <%--<div style="margin: 0 auto;z-index: 0;padding: 0; border: 25px solid #ffffff; border: 25px solid rgba(255, 255, 255, 0.25); -ms-border-radius: 18px; border-radius: 18px; max-width: 24rem;">--%>
                                <div style="margin: 20px auto 0;z-index: 0;padding: 0;max-width: 24rem;">
                                    <div style="margin:0 auto;padding:0;z-index: 10;display: block;width: 100%;max-width:335px;max-height: 150px;height: auto;background: #fff !important;text-align: left;">
                                        <div id="divLoginAd" runat="server">
                                            --Ad goes here--
                                        </div>
                                    </div>  
                                </div>
                            </div>
                        </div>
                        <div class="large-4 medium-5 small-12 columns">
                            <div style="padding:0.25rem 0 0;">
                                <%--<div style="margin: 0 auto;z-index: 0;padding: 0; border: 25px solid #ffffff; border: 25px solid rgba(255, 255, 255, 0.25); -ms-border-radius: 18px; border-radius: 18px; max-width: 24rem;">--%>
                                <div style="margin: 20px auto 0;padding: 0;max-width: 24rem;">
                                    <div class="align-center-mobile" style="margin:0 12px;padding: 0.75rem 1.563rem 0;display: block;max-width:335px;height: auto;background: #fff !important;">

                                        <%--<div id="Div2" runat="server" style="font-size: 1rem;font-weight:bold;color: #222;margin: 0 0 0.5rem;text-align: center;">
                                            Demo Login
                                        </div>--%>

                                        <div runat="server">
                                            <label>
                                                Enter Authentication Code
                                                <%--<input id="txtLoginOTP" lbl="Login Authentication Code"
                                                     isrequired="true" isvalid="false"
                                                     min-length="10" max-length="12"
                                                     matchpattern="^\s*(?:\+?(\d{1,3}))?[-. (]*(\d{3})[-. )]*(\d{3})[-. ]*(\d{4})(?: *x(\d+))?\s*$"
                                                     patterndescription="Authentication Code"
                                                     type="text" runat="server"
                                                     onblur="javascript: validateFormFields(this);" 
                                                     onkeyup="javascript: validateFormFields(this);"
                                                     meta:resourcekeyonchange="javascript: validateFormFields(this);" />--%>
                                                <input id="txtLoginOTP" lbl="Login Authentication Code"
                                                     isrequired="true" isvalid="false"
                                                     min-length="10" max-length="12"
                                                     patterndescription="Authentication Code"
                                                     type="text" runat="server" />
                                            </label>
                                        </div>

                                        <!-- Button container -->
                                        <div style="text-align: center;margin-top: 0.75rem;padding-bottom:7px;">
                                            <asp:Button CssClass="button tiny radius loginButton" BackColor="#2d8c36" runat="server" ID="btnLoginOTP" Text="Submit" OnClick="btnVerifyLoginOtp_Click"/>
                                            <asp:Button CssClass="button tiny radius loginButton" BackColor="#2d8c36" runat="server" ID="btnLoginOTPResend" Text="Resend" OnClick="btnResendLoginOtp_Click"/>
                                        
                                        </div>

                                        <div style="clear: both;"></div>

                                    </div>

                                </div>
                            </div>
                        </div>
                    </div>
                    <!-- end login OTP container -->
                    <div class="hide-for-small-down" style="margin-top:0.75rem;">&nbsp;</div>
                    <div class="row">
                        <div class="large-12 columns" style="text-align: center;">
                            <div id="divErrorContainer" runat="server" class="alert-box warning radius">
                                <asp:Label ID="lbError" runat="server" ForeColor="#222" Font-Bold="true" Text="" />
                            </div>
                            <div id="divErrorContainerRegister" runat="server" class="alert-box warning radius" style="margin-bottom: 0.75rem;">
                                <label style="font-weight: bold;color: #222;">Email not found. Enter a valid Email Address or <asp:Button CssClass="registerLink" runat="server" ID="btnToRegister1" Text="Register" CausesValidaion="false" Enabled="true" OnClick="btnToRegister_Click"/> a new user.</label>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="large-12 columns">
                            <div class="panel radius" style="background: #2d521a;margin:0 auto 1rem;border:1px solid #fff;text-align: center;padding: 0.75rem 1rem 1rem;">
                                <p>
                                    <%--<span style="font-size:1rem;line-height: 1.25rem;color: #fff;font-family: 'Roboto Slab', serif;text-rendering: optimizelegibility;">Scottsdale Golf Shop carries a world-class selection of brand name golf equipment, apparel, accessories and gifts for golfers of all ages and abilities. Whatever your golf needs, Scottsdale Golf Shop has you covered.</span>--%>
                                    <span style="font-size:1rem;line-height: 1.25rem;color: #fff;font-family: 'Roboto Slab', serif;text-rendering: optimizelegibility;">World-class golf equipment, apparel, accessories and gifts for golfers of all ages and abilities.</span>
                                </p>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="large-4 medium-4 small-12 columns" style="color:#fff;padding:.75rem 0.75rem 0;text-align: center;">
                            <div style="max-width:300px;margin:0 auto;border:1px solid #fff;">
                                <a onclick="javascript: pleaseLogin();"><img src="img/golf-3.jpg" alt="" /></a>
                                <div style="background:#2d521a;font-size:1.5rem;color:#fff;display:block;height:55px;line-height:55px;text-align: center;">
                                    Clubs
                                </div>
                            </div>
                            <div style="max-width:300px;margin:0 auto;">
                                <p style="padding:0.625rem;text-align:left;font-size:0.813rem;color:#fff;">
                                    Every year, new technology pushes the envelope to help golfers improve their distance, accuracy and consistency. Advance your golf game with the latest drivers, fairway woods, hybrids, irons and putters from the biggest brands!
                                </p>
                            </div>
                        </div>
                        <div class="large-4 medium-4 small-12 columns" style="color:#fff;padding:.75rem 0.75rem 0;">
                            <div style="max-width:300px;margin:0 auto;border:1px solid #fff;">
                                <a onclick="javascript: pleaseLogin();"><img src="img/golf-1.jpg" alt="" /></a>
                                <div style="background:#2d521a;font-size:1.5rem;color:#fff;display:block;height:55px;line-height:55px;text-align: center;">
                                    Apparel
                                </div>
                            </div>
                            <div style="max-width:300px;margin:0 auto;">
                                <p style="padding:0.625rem;text-align:left;font-size:0.813rem;">
                                    Golf shirts, pants, shorts and outerwear are more than just clothing - they're golf equipment, providing moisture management and protection from the elements. Get great golf apparel at great values from the leading brands!
                                </p>
                            </div>
                        </div>
                        <div class="large-4 medium-4 small-12 columns" style="color:#fff;padding:.75rem 0.75rem 0;">
                            <div style="max-width:300px;margin:0 auto;border:1px solid #fff;">
                                <a onclick="javascript: pleaseLogin();"><img src="img/golf-2.jpg" alt="" /></a>
                                <div style="background:#2d521a;font-size:1.5rem;color:#fff;display:block;height:55px;line-height:55px;text-align: center;">
                                    Accessories
                                </div>
                            </div>
                            <div style="max-width:300px;margin:0 auto;">
                                <p style="padding:0.625rem;text-align:left;font-size:0.813rem;">
                                    Scottsdale Golf Shop has a wide range of golf accessories and essential tools to complement and support your golf game. From rangefinders to personalized golf tees, Scottsdale Golf Shop has everything you need.
                                </p>
                            </div>
                        </div>
                    </div>
                    <div class="row" style="display: none;">
                        <div class="large-12 columns" style="text-align: center;">
                            <span id="spanServerIp" runat="server" style="font-size: 0.875rem;color: #fff;">Server: 127.0.0.1</span>
                        </div>
                    </div>
                    <%--<div class="row">
                        <div class="large-12 columns" style="text-align: center;margin-bottom: 0.5rem;">
                            <span style="color: #fff;font-size: 0.5rem;">&#9899;&nbsp;&#9899;&nbsp;&#9899;&nbsp;&#9899;&nbsp;&#9899;&nbsp;</span>
                        </div>
                    </div>--%>
                    <!-- Disclaimer/Copyright -->
                    <div class="row" id="divDisclaimer" style="position:relative;z-index:100 !important;">
                        <div class="large-12 columns">
                            <fieldset style="border-radius: 5px;background-color: #2d521a;"><legend style="background: none;color:#fff;">MAC Disclosure Statement</legend>
                                <div>
                                    <p class="golfDisclaimer" style="margin-top: 0;">
                                        <strong>This demonstration website ("Demo") is the copyrighted work of Mobile Authentication Corporation.</strong>
                                    </p>
                                    <p class="golfDisclaimer">
                                        The information contained in this website is for general information purposes only. While we endeavor to keep the information up to date and correct, we make no representations or warranties of any kind, express or implied, about the completeness, accuracy, reliability, suitability or availability with respect to the website or the information, products, services, or related graphics contained on this website for any purpose. Any reliance you place on such information is therefore strictly at your own risk.
                                    </p>
                                    <p class="golfDisclaimer">
                                        In no event will we be liable for any loss or damage including without limitation, indirect or consequential loss or damage, or any loss or damage whatsoever arising from loss of data or profits arising out of, or in connection with, the use of this website.
                                    </p>
                                    <p class="golfDisclaimer">
                                        Links to third party websites are not under the control of Mobile Authentication Corporation. The inclusion of any links does not necessarily imply a recommendation or endorse the views expressed within them.
                                    </p>
                                    <p class="golfDisclaimer" style="margin-bottom: 0;">
                                        Every effort is made to keep the website up and running smoothly. However, Mobile Authentication Corporation will not be liable for the website being temporarily unavailable due to technical issues beyond our control.
                                    </p>
                                    <%--<div style="padding: 0.5rem;"></div>
                                    <span style="font-size: 1rem;font-weight: bold;color: #fff;">
                                        To unsubscribe, <asp:Button CssClass="registerLink" runat="server" ID="Button1" Text="click here2" Enabled="true" OnClick="btnGoToUnsubscribe_Click"/>
                                    </span>--%>
                                </div>
                            </fieldset>
                        </div>
                    </div>
                    <div class="row">
                        <div class="large-12 columns">
                            <div class="hide-for-small-down" style="color: #fff;text-align: left;line-height:1.25rem;font-size: 0.719rem;margin:0 0 2.5rem;">
                                <script type="text/javascript">
                                    <!--
                                    var currentDate = new Date();
                                    var year = currentDate.getFullYear();
                                    document.write("&copy; " + year + " Mobile Authentication Corporation. All rights reserved.");
                                    //-->
                                </script>
                            </div>
                            <div class="show-for-small-down" style="color: #fff;text-align: center;line-height:1.25rem;font-size: 0.719rem;margin:0 0 2.5rem;">
                                <script type="text/javascript">
                                    <!--
                                    var currentDate = new Date();
                                    var year = currentDate.getFullYear();
                                    document.write("&copy; " + year + " Mobile Authentication Corporation.<br />All rights reserved.");
                                    //-->
                                </script>
                            </div>
                        </div>
                    </div>
                    <!-- Disclaimer/Copyright -->        
                </div>
            </div>
        </div>
        <!--end login div content-->

        

        <!--client admin div-->
        <div id="divClientAdmin" style="position: relative;margin: 0 auto;z-index: 10;" runat="server">
            <div id="divCfgForm" runat="server">
                <div class="row">
                    <div class="large-12 columns">
                        <img src="img/mac-logo_80_white.png" alt="Mobile Authentication Corporation" style="margin: 1.25rem 0 0;" />
                    </div>
                </div>
                <div class="row" id="scroll2">
                    <div class="large-12 columns">
                        <fieldset><legend style="background: none;"><span class="label_875" style="color: #fff !important;">Client Administration</span></legend>
                            <div class="row">
                                <div class="large-6 medium-6 small-12 columns">
                                    <label style="color: #fff;">Client Name
                                        <asp:TextBox ID="clientName" runat="server" />
                                    </label>
                                </div>
                                <div class="large-6 medium-6 small-12 columns">
                                    <label style="color: #fff;">Mac Service URL
                                        <asp:TextBox ID="macServicesUrl" runat="server" />
                                    </label>
                                </div>
                                <%--<div class="large-6 medium-6 small-12 columns">
                                    <label style="color: #fff;">Client ID
                                        <asp:TextBox ID="clientId" runat="server" />
                                    </label>
                                </div>--%>
                            </div>
                            <div class="row">
                                <div class="large-6 medium-6 small-12 columns">
                                  <label style="color: #fff;">MAC Bank URL
                                        <asp:TextBox ID="macbankUrl" runat="server" />
                                    </label>
                                </div>
                                <div class="large-6 medium-6 small-12 columns">
                                    <label style="color: #fff;">Register URL
                                        <asp:TextBox ID="registerUrl" runat="server" />
                                    </label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="large-6 medium-6 small-12 columns">
                                    <label style="color: #fff;">Email Server
                                        <asp:TextBox ID="txtEmailServer" runat="server" />
                                    </label>
                                </div>
                                <div class="large-6 medium-6 small-12 columns">
                                    <label style="color: #fff;">Email Port
                                        <asp:TextBox ID="txtEmailPort" runat="server" />
                                    </label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="large-6 medium-6 small-12 columns">
                                    <label style="color: #fff;">Email Login User Name
                                        <asp:TextBox ID="txtEmailLoginUserName" runat="server" />
                                    </label>
                                </div>
                                <div class="large-6 medium-6 small-12 columns">
                                    <label style="color: #fff;">Email Password
                                        <asp:TextBox ID="txtEmailPassword" runat="server" />
                                    </label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="large-6 medium-6 small-12 columns">
                                    <label style="color: #fff;">Email From Address
                                        <asp:TextBox ID="txtEmailFromAddress" runat="server" />
                                    </label>
                                </div>
                                <div class="large-6 medium-6 small-12 columns">
                                    <label style="color: #fff;">Email To Address
                                        <asp:TextBox ID="txtEmailToAddress" runat="server" />
                                    </label>
                                </div>
                            </div>
                            <div style="padding: 0.25rem;"></div>
                            <div class="row">
                                <div class="large-12 columns">
                                    <asp:Button CssClass="button tiny radius" BackColor="#2d8c36" ID="btnSaveCfg" runat="server" Text="Save" OnClick="btnSaveCfg_Click"/>
                                    <asp:Button CssClass="button tiny radius" BackColor="#2d8c36" ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancelCfg_Click"/>
                                </div>
                            </div>
                        </fieldset>
                    </div>
                </div>
            </div>
            <div style="padding: 0.5rem;"></div>
            <div class="row">
                <div class="large-12 columns">
                    <label><asp:Label ID="Label1" runat="server" ForeColor="Red" Text="" /></label>
                </div>
            </div>
            <div class="row hide-for-small">
                <div class="large-12 columns">
                    <div style="color: #fff;font-size: 0.875rem;">
                        <script type="text/javascript">
                            <!--
    var currentDate = new Date();
    var year = currentDate.getFullYear();
    document.write("&copy; " + year + " Mobile Authentication Corporation.");
    //-->
                        </script>
                    </div>
                </div>
            </div>
            <div class="row show-for-small">
                <div class="large-12 columns">
                    <div style="color: #fff;font-size: 0.875rem;line-height: 1.25rem;text-align: center;">
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
            <input id="panelFocusClients" runat="server" type="hidden" value="" />
        </div>
        <!--end client admin div-->

        <!-- thank you div -->
        <div id="divThankYou" style="position: relative;margin: 1rem auto;z-index: 10;" runat="server">
            <div class="row">
                <div class="large-12 columns">
                    <fieldset>
                        <div class="row">
                            <div class="large-12 columns" style="padding: 0 1rem;text-align: center;">
                                <h1 style="font-size: 2.5rem;color: #fff;white-space: nowrap;">Thank you!</h1>
                                <p style="font-size: 1rem;color: #fff;line-height: 1.5rem;">This completes the demo.</p>
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-12 columns" style="padding: 0 1rem;text-align: center;">
                                <asp:Button CssClass="button tiny radius" BackColor="#2d8c36" ID="btnHome" runat="server" Text="Home" OnClick="btnHome_Click" />
                            </div>
                        </div>
                    </fieldset>
                </div>
            </div>
            <!-- Disclaimer/Copyright -->
            <div class="row">
                <div class="large-12 columns">
                    <fieldset style="border-radius: 5px;background-color: #2d521a;"><legend style="background: none;color:#fff;">MAC Disclosure Statement</legend>
                        <div>
                            <p class="golfDisclaimer" style="margin-top: 0;">
                                <strong>This demonstration website ("Demo") is the copyrighted work of Mobile Authentication Corporation.</strong>
                            </p>
                            <p class="golfDisclaimer">
                                The information contained in this website is for general information purposes only. While we endeavor to keep the information up to date and correct, we make no representations or warranties of any kind, express or implied, about the completeness, accuracy, reliability, suitability or availability with respect to the website or the information, products, services, or related graphics contained on this website for any purpose. Any reliance you place on such information is therefore strictly at your own risk.
                            </p>
                            <p class="golfDisclaimer">
                                In no event will we be liable for any loss or damage including without limitation, indirect or consequential loss or damage, or any loss or damage whatsoever arising from loss of data or profits arising out of, or in connection with, the use of this website.
                            </p>
                            <p class="golfDisclaimer">
                                Links to third party websites are not under the control of Mobile Authentication Corporation. The inclusion of any links does not necessarily imply a recommendation or endorse the views expressed within them.
                            </p>
                            <p class="golfDisclaimer" style="margin-bottom: 0;">
                                Every effort is made to keep the website up and running smoothly. However, Mobile Authentication Corporation will not be liable for the website being temporarily unavailable due to technical issues beyond our control.
                            </p>
                            <%--<div style="padding: 0.5rem;"></div>
                            <span style="font-size: 1rem;font-weight: bold;color: #fff;">To unsubscribe, <asp:Button CssClass="registerLink" runat="server" ID="unregister2" Text="click here3" Enabled="true" OnClick="btnGoToUnsubscribe_Click"/></span>--%>
                        </div>  
                    </fieldset>
                </div>
            </div>
            <!-- Disclaimer/Copyright -->
        </div>

        <!-- feedback div -->
        <div id="divFeedback" style="position: relative;margin: 1rem auto;z-index: 10;" runat="server">
            <div class="row">
                <div class="large-12 columns">
                    <div id="divEmailSendError" runat="server" style="text-align:center;margin:0.5rem 0 0;">
                        <asp:Label ID="lbErrorEmailSend" runat="server" ForeColor="Orange" Text="" />
                    </div>
                    <fieldset>
                        <%--<div class="row">
                            <div class="large-12 columns" style="padding: 0 1rem;text-align: center;">
                                <h1 style="font-size: 2.5rem;color: #fff;white-space: nowrap;">Thank you!</h1>
                                <p style="font-size: 1rem;color: #fff;line-height: 1.5rem;">This completes the demo.</p>
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-12 columns" style="padding: 0 1rem;">
                                <asp:Button CssClass="button tiny radius" BackColor="#2d8c36" ID="btnJustExit" runat="server" Text="Home" OnClick="btnFeedbackSubmit_Click" />
                            </div>
                        </div>
                        <div style="padding: 0.25rem;"></div>--%>
                        <div class="row">
                            <div class="large-12 columns" style="padding: 0 1rem;">
                                <p style="font-size: 1rem;color: #fff;line-height: 1.5rem;">
                                    We would appreciate any feedback you can provide regarding this demo. This information will be used to further enhance and improve user experience.
                                </p>
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-12 columns" style="padding: 0 1rem;">
                                <asp:TextBox id="selectFeedback" TextMode="multiline" Columns="50" Rows="10" runat="server" placeholder="Enter Comments" />
                                     <%--min-length="1" isrequired="true" isvalid="false"
                                     onblur="javascript: validateFormFields(this);" onkeyup="javascript: validateFormFields(this);" onchange="javascript: validateFormFields(this);" />--%>
                            </div>
                        </div>
                        <div style="padding: 0.25rem;"></div>
                        <div class="row">
                            <div class="large-12 columns" style="padding: 0 1rem;">
                                <input type="checkbox" id="chkContact" name="contact" value="" onchange="javascript: contactMe();" />
                                <label style="color: #fff !important">Please contact me with more information</label>
                                <%--<asp:CheckBox CssClass="noMargin" ID="chkContact" runat="server" Text="Please contact me with more information" />--%>
                            </div>
                        </div>
                        <div style="padding: 0.25rem;"></div>
                        <div class="row">
                            <div class="large-12 columns" style="padding: 0 1rem;">                                
                                <asp:Button CssClass="button tiny radius" BackColor="#2d8c36" ID="btnFeedbackSubmit" runat="server" Text="Submit" OnClick="btnFeedbackSubmit_Click" />
                                <asp:Button CssClass="button tiny radius" BackColor="#2d8c36" ID="btnNoThanks" runat="server" Text="No Thanks" OnClick="btnThankYou_Click" />
                            </div>
                        </div>
                    </fieldset>
                </div>
            </div>
            <!-- Disclaimer/Copyright -->
            <div class="row">
                <div class="large-12 columns">
                    <fieldset style="border-radius: 5px;background-color: #2d521a;"><legend style="background: none;color:#fff;">MAC Disclosure Statement</legend>
                        <div>
                            <p class="golfDisclaimer" style="margin-top: 0;">
                                <strong>This demonstration website ("Demo") is the copyrighted work of Mobile Authentication Corporation.</strong>
                            </p>
                            <p class="golfDisclaimer">
                                The information contained in this website is for general information purposes only. While we endeavor to keep the information up to date and correct, we make no representations or warranties of any kind, express or implied, about the completeness, accuracy, reliability, suitability or availability with respect to the website or the information, products, services, or related graphics contained on this website for any purpose. Any reliance you place on such information is therefore strictly at your own risk.
                            </p>
                            <p class="golfDisclaimer">
                                In no event will we be liable for any loss or damage including without limitation, indirect or consequential loss or damage, or any loss or damage whatsoever arising from loss of data or profits arising out of, or in connection with, the use of this website.
                            </p>
                            <p class="golfDisclaimer">
                                Links to third party websites are not under the control of Mobile Authentication Corporation. The inclusion of any links does not necessarily imply a recommendation or endorse the views expressed within them.
                            </p>
                            <p class="golfDisclaimer" style="margin-bottom: 0;">
                                Every effort is made to keep the website up and running smoothly. However, Mobile Authentication Corporation will not be liable for the website being temporarily unavailable due to technical issues beyond our control.
                            </p>
                            <%--<div style="padding: 0.5rem;"></div>
                            <span style="font-size: 1rem;font-weight: bold;color: #fff;">To unsubscribe, <asp:Button CssClass="registerLink" runat="server" ID="unregister3" Text="click here4" Enabled="true" OnClick="btnGoToUnsubscribe_Click"/></span>--%>
                        </div> 
                    </fieldset>
                </div>
            </div>
            <!-- Disclaimer/Copyright -->
        </div>
        <!--end thank you div-->

        <!--begin store container-->
        <div class="row" id="divStoreContainer" style="position: relative;margin: 0 auto;text-align: center;z-index: 10;" runat="server">
            <div class="large-12 columns" style="margin-top: 1.75rem;">
                <div class="alert-box success radius" id="loginMessage" runat="server" style="color:#222;text-align:center;cursor:pointer;" title="Click to close message" onclick="javascript: hideMessage();">
                    <div class="hide-for-small-down">Login Success! Please select items below for purchase.</div>
                    <div class="show-for-small-down">Login Success!<br />Please select items below for purchase.</div>
                </div>
            </div>
            <div class="large-12 columns">    
                <fieldset id="myFieldset" runat="server" style="margin-top:0;"><legend style="background: none;color:#fff;"><span id="legendLabel" runat="server">Golf Equipment</span></legend>
                    <div id="productDisplay" runat="server">
                        <div class="row">
                            <div class="large-3 medium-3 small-12 columns" style="color:#222;">
                                <div style="margin: 0.5rem;background: #fff;">
                                    <img src="img/golf-bag-111203.jpg" style="width:204px;" />
                                    <div style="padding: 0.5rem;font-size: 0.875rem;">
                                        <div style="margin-bottom: 0.5rem;" id="item_decription1">BLUE GOLF BAG</div>
                                        <div style="margin-bottom: 0.5rem;color: #052441;">Item: #<span id="item_number1">111203</span></div>
                                        <span style="margin-top: 0.125rem;font-weight: bold;" id="item_price1">$129.33</span>
                                        <div style="text-align: center;"><a href="#" id="item1" onclick="javascript: btnItem1();" class="item-thumb-button">Add To Cart</a></div>
                                    </div>
                                </div>
                            </div>
                            <div class="large-3 medium-3 small-12 columns" style="color:#222;">
                                <div style="margin: 0.5rem;background: #fff;">
                                    <img src="img/golf-balls-111111.jpg" />
                                    <div style="padding: 0.5rem;font-size: 0.875rem;">
                                        <div style="margin-bottom: 0.5rem;" id="item_decription2">GOLF BALLS</div>
                                        <div style="margin-bottom: 0.5rem;color: #052441;">Item: #<span id="item_number2">111111</span></div>
                                        <span style="margin-top: 0.125rem;font-weight: bold;" id="item_price2">$39.00</span>
                                        <div style="text-align: center;"><a href="#" id="item2" onclick="javascript: btnItem2();" class="item-thumb-button">Add To Cart</a></div>
                                    </div>
                                </div>
                            </div>
                            <div class="large-3 medium-3 small-12 columns" style="color:#222;">
                                <div style="margin: 0.5rem;background: #fff;">
                                    <img src="img/kids-clubs-111200.jpg" />
                                    <div style="padding: 0.5rem;font-size: 0.875rem;">
                                        <div style="margin-bottom: 0.5rem;" id="item_decription3">KIDS GOLF CLUB SET</div>
                                        <div style="margin-bottom: 0.5rem;color: #052441;">Item: #<span id="item_number3">111200</span></div>
                                        <span style="margin-top: 0.125rem;font-weight: bold;" id="item_price3">$69.99</span>
                                        <div style="text-align: center;"><a href="#" id="item3" onclick="javascript: btnItem3();" class="item-thumb-button">Add To Cart</a></div>
                                    </div>
                                </div>
                            </div>
                            <div class="large-3 medium-3 small-12 columns" style="color:#222;">
                                <div style="margin: 0.5rem;background: #fff;">
                                    <img src="img/mens-golf-shoes-111202.jpg" />
                                    <div style="padding: 0.5rem;font-size: 0.875rem;">
                                        <div style="margin-bottom: 0.5rem;" id="item_decription4">MEN'S GOLF SHOES</div>
                                        <div style="margin-bottom: 0.5rem;color: #052441;">Item: #<span id="item_number4">111202</span></div>
                                        <span style="margin-top: 0.125rem;font-weight: bold;" id="item_price4">$89.99</span>
                                        <div style="text-align: center;"><a href="#" id="item4" onclick="javascript: btnItem4();" class="item-thumb-button">Add To Cart</a></div>
                                    </div>
                                </div>
                            </div> 
                        </div>

                        <div style="padding: 0.5rem;"></div>

                        <div class="row">
                            <div class="large-3 medium-3 small-12 columns" style="color:#222;">
                                <div style="margin: 0.5rem;background: #fff;">
                                    <img src="img/tees-111170.jpg" />
                                    <div style="padding: 0.5rem;font-size: 0.875rem;">
                                        <div style="margin-bottom: 0.5rem;" id="item_decription5">TEE PACKAGE</div>
                                        <div style="margin-bottom: 0.5rem;color: #052441;">Item: #<span id="item_number5">111170</span></div>
                                        <span style="margin-top: 0.125rem;font-weight: bold;" id="item_price5">$7.98</span>
                                        <div style="text-align: center;"><a href="#" id="item5" onclick="javascript: btnItem5();" class="item-thumb-button">Add To Cart</a></div>
                                    </div>
                                </div>
                            </div>
                            <div class="large-3 medium-3 small-12 columns" style="color:#222;">
                                <div style="margin: 0.5rem;background: #fff;">
                                    <img src="img/water-bottle-111300.jpg" />
                                    <div style="padding: 0.5rem;font-size: 0.875rem;">
                                        <div style="margin-bottom: 0.5rem;" id="item_decription6">KEEP COOL BOTTLE</div>
                                        <div style="margin-bottom: 0.5rem;color: #052441;">Item: #<span id="item_number6">111300</span></div>
                                        <span style="margin-top: 0.125rem;font-weight: bold;" id="item_price6">$12.00</span>
                                        <div style="text-align: center;"><a href="#" id="item6" onclick="javascript: btnItem6();" class="item-thumb-button">Add To Cart</a></div>
                                    </div>
                                </div>
                            </div>
                            <div class="large-3 medium-3 small-12 columns" style="color:#222;">
                                <div style="margin: 0.5rem;background: #fff;">
                                    <img src="img/womens-hat-111130.jpg" />
                                    <div style="padding: 0.5rem;font-size: 0.875rem;">
                                        <div style="margin-bottom: 0.5rem;" id="item_decription7">RED HAT</div>
                                        <div style="margin-bottom: 0.5rem;color: #052441;">Item: #<span id="item_number7">111130</span></div>
                                        <span style="margin-top: 0.125rem;font-weight: bold;" id="item_price7">$14.50</span>
                                        <div style="text-align: center;"><a href="#" id="item7" onclick="javascript: btnItem7();" class="item-thumb-button">Add To Cart</a></div>
                                    </div>
                                </div>
                            </div>
                            <div class="large-3 medium-3 small-12 columns" style="color:#222;">
                                <div style="margin: 0.5rem;background: #fff;">
                                    <img src="img/womens-visor-111118.jpg" />
                                    <div style="padding: 0.5rem;font-size: 0.875rem;">
                                        <div style="margin-bottom: 0.5rem;" id="item_decription8">LADY'S GOLF VISOR</div>
                                        <div style="margin-bottom: 0.5rem;color: #052441;">Item: #<span id="item_number8">111118</span></div>
                                        <span style="margin-top: 0.125rem;font-weight: bold;" id="item_price8">$19.99</span>
                                        <div style="text-align: center;"><a href="#" id="item8" onclick="javascript: btnItem8();" class="item-thumb-button">Add To Cart</a></div>
                                    </div>
                                </div>
                            </div> 
                        </div>
                    </div>

                    <div id="tableContainerPurchase" runat="server">
                        <div style="padding: 0.75rem;" id="tableSpacer" runat="server"></div>
                         <div class="row">
                            <div class="large-12 columns" style="text-align: right;padding-right: 1.5rem;">
                                <table style="width: 100%;border-collapse: collapse;margin-bottom:0;margin-left: .5rem;"><thead><tr><th>Item #</th><th>Description</th><th>Quantity</th><th style="text-align: right;">Subtotal</th></tr></thead>
                                    <tbody>
                                        <tr id="item_display1" runat="server">
                                            <td style="width:100px;" class="small-only-text-left"><strong class="show-for-small-down">Item #</strong><span id="item_number1_display" runat="server"></span></td>
                                            <td><strong class="show-for-small-down">Description</strong><span id="item_decription1_display" runat="server"></span></td>
                                            <td style="width:45px;text-align: center;" class="small-only-text-left"><strong class="show-for-small-down">Quantity</strong><span id="item_quantity1" runat="server">0</span></td>
                                            <td style="width:155px;text-align: right;" class="small-only-text-left"><strong class="show-for-small-down">Subtotal</strong><span id="item_subtotal1" runat="server">0.00</span></td>
                                        </tr>
                                        <tr id="item_display2" runat="server">
                                            <td style="width:100px;" class="small-only-text-left"><strong class="show-for-small-down">Item #</strong><span id="item_number2_display" runat="server"></span></td>
                                            <td><strong class="show-for-small-down">Description</strong><span id="item_decription2_display" runat="server"></span></td>
                                            <td style="width:45px;text-align: center;" class="small-only-text-left"><strong class="show-for-small-down">Quantity</strong><span id="item_quantity2" runat="server">0</span></td>
                                            <td style="width:155px;text-align: right;" class="small-only-text-left"><strong class="show-for-small-down">Subtotal</strong><span id="item_subtotal2" runat="server">0.00</span></td>
                                        </tr>
                                        <tr id="item_display3" runat="server">
                                            <td style="width:100px;" class="small-only-text-left"><strong class="show-for-small-down">Item #</strong><span id="item_number3_display" runat="server"></span></td>
                                            <td><strong class="show-for-small-down">Description</strong><span id="item_decription3_display" runat="server"></span></td>
                                            <td style="width:45px;text-align: center;" class="small-only-text-left"><strong class="show-for-small-down">Quantity</strong><span id="item_quantity3" runat="server">0</span></td>
                                            <td style="width:155px;text-align: right;" class="small-only-text-left"><strong class="show-for-small-down">Subtotal</strong><span id="item_subtotal3" runat="server">0.00</span></td>
                                        </tr>
                                        <tr id="item_display4" runat="server">
                                            <td style="width:100px;" class="small-only-text-left"><strong class="show-for-small-down">Item #</strong><span id="item_number4_display" runat="server"></span></td>
                                            <td><strong class="show-for-small-down">Description</strong><span id="item_decription4_display" runat="server"></span></td>
                                            <td style="width:45px;text-align: center;" class="small-only-text-left"><strong class="show-for-small-down">Quantity</strong><span id="item_quantity4" runat="server">0</span></td>
                                            <td style="width:155px;text-align: right;" class="small-only-text-left"><strong class="show-for-small-down">Subtotal</strong><span id="item_subtotal4" runat="server">0.00</span></td>
                                        </tr>
                                        <tr id="item_display5" runat="server">
                                            <td style="width:100px;" class="small-only-text-left"><strong class="show-for-small-down">Item #</strong><span id="item_number5_display" runat="server"></span></td>
                                            <td><strong class="show-for-small-down">Description</strong><span id="item_decription5_display" runat="server"></span></td>
                                            <td style="width:45px;text-align: center;" class="small-only-text-left"><strong class="show-for-small-down">Quantity</strong><span id="item_quantity5" runat="server">0</span></td>
                                            <td style="width:155px;text-align: right;" class="small-only-text-left"><strong class="show-for-small-down">Subtotal</strong><span id="item_subtotal5" runat="server">0.00</span></td>
                                        </tr>
                                        <tr id="item_display6" runat="server">
                                            <td style="width:100px;" class="small-only-text-left"><strong class="show-for-small-down">Item #</strong><span id="item_number6_display" runat="server"></span></td>
                                            <td><strong class="show-for-small-down">Description</strong><span id="item_decription6_display" runat="server"></span></td>
                                            <td style="width:45px;text-align: center;" class="small-only-text-left"><strong class="show-for-small-down">Quantity</strong><span id="item_quantity6" runat="server">0</span></td>
                                            <td style="width:155px;text-align: right;" class="small-only-text-left"><strong class="show-for-small-down">Subtotal</strong><span id="item_subtotal6" runat="server">0.00</span></td>
                                        </tr>
                                        <tr id="item_display7" runat="server">
                                            <td style="width:100px;" class="small-only-text-left"><strong class="show-for-small-down">Item #</strong><span id="item_number7_display" runat="server"></span></td>
                                            <td><strong class="show-for-small-down">Description</strong><span id="item_decription7_display" runat="server"></span></td>
                                            <td style="width:45px;text-align: center;" class="small-only-text-left"><strong class="show-for-small-down">Quantity</strong><span id="item_quantity7" runat="server">0</span></td>
                                            <td style="width:155px;text-align: right;" class="small-only-text-left"><strong class="show-for-small-down">Subtotal</strong><span id="item_subtotal7" runat="server">0.00</span></td>
                                        </tr>
                                        <tr id="item_display8" runat="server">
                                            <td style="width:100px;" class="small-only-text-left"><strong class="show-for-small-down">Item #</strong><span id="item_number8_display" runat="server"></span></td>
                                            <td><strong class="show-for-small-down">Description</strong><span id="item_decription8_display" runat="server"></span></td>
                                            <td style="width:45px;text-align: center;" class="small-only-text-left"><strong class="show-for-small-down">Quantity</strong><span id="item_quantity8" runat="server">0</span></td>
                                            <td style="width:155px;text-align: right;" class="small-only-text-left"><strong class="show-for-small-down">Subtotal</strong><span id="item_subtotal8" runat="server">0.00</span></td>
                                        </tr>
                                    </tbody>
                                    <tfoot style="text-align:right;" id="totalContainer">
                                        <tr>
                                            <td colspan="4">
                                                <div class="show-for-small" style="border-top:none;"></div>
                                                <div id="scrollHere" style="text-align: right;" class="small-only-text-left">
                                                    <strong>Total:</strong> $<span id="totalPrice" runat="server">0.00</span>
                                                </div>
                                            </td>
                                        </tr>
                                    </tfoot>
                                </table>
                            </div>
                        </div>
                    </div>
                    <!-- end tableContainerPurchase -->

                    <!-- error message -->
                    <div class="row">
                        <div class="large-12 columns" style="text-align: center;padding: 0.5rem;margin-top: 0.5rem;">
                            <asp:Label ID="lbError2" runat="server" ForeColor="Orange" Text="" />
                        </div>
                    </div>

                    <!--Unsubscribe-->
                    <div id="divUnsubscribe" runat="server">
                        <div class="row">
                            <div class="large-12 columns" style="text-align: center;">
                                <div style="color: #fff;">
                                    Enter your email address below to remove yourself from the demo system.
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-12 columns" style="padding: 0 1rem;">
                                <div style="margin: 35px auto 15px;z-index: 0;padding: 0; border: 25px solid #ffffff; border: 25px solid rgba(255, 255, 255, 0.25); -ms-border-radius: 18px; border-radius: 18px; max-width: 24rem;">
                                    <div style="margin:0 auto;padding: 0.5rem 1.563rem 0;z-index: 10;display: block;height: auto;background: #fff !important;text-align: left;">
                                        <div runat="server">
                                            <label>
                                                Enter Email
                                                <input id="txtUnsubscribe" lbl="Email" isrequired="true" isvalid="false" min-length="7" max-length="50" 
                                                    matchpattern="^([A-Za-z0-9_\.-]+)@([\da-z\.-]+)\.([A-Za-z\.]{2,6})$" patterndescription="a valid email address" type="text" runat="server" 
                                                    onblur="javascript: validateFormFields(this);" 
                                                    onkeyup="javascript: validateFormFields(this);" 
                                                    onchange="javascript: validateFormFields(this);" />
                                            </label>
                                        </div>
                                        <div runat="server" style="text-align: center;">
                                            <asp:Label ID="lbError3" runat="server" ForeColor="Red" Text="" />
                                        </div>
                                        <!-- Button container -->
                                        <div style="text-align: center;margin-top: 0.75rem;">
                                            <asp:Button CssClass="button tiny radius" BackColor="#2d8c36" ID="btnUnsubscribe" Enabled="false" runat="server" Text="Submit" onclick="btnUnsubscribe_Click" />
                                        </div>
                                        <div style="clear: both;"></div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <!--End Unsubscribe-->

                    <!-- successfully unsubscribed div -->
                    <div id="divUnsubscribeMessage" style="position: relative;margin: 1rem auto;z-index: 10;" runat="server">
                        <div class="row">
                            <div class="large-12 columns" style="padding: 0 1rem;text-align: center;">
                                <h1 style="font-size: 2.5rem;color: #fff;white-space: nowrap;">Thank you!</h1>
                                <p style="font-size: 1rem;color: #fff;line-height: 1.5rem;">You have successfully removed "<span id="deletedEmail" runat="server">xxxx</span>" from the demo system.</p>
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-12 columns" style="padding: 0 1rem;">
                                <asp:Button CssClass="button tiny radius" BackColor="#2d8c36" ID="btnHome1" runat="server" Text="Home" OnClick="btnHome_Click" />
                            </div>
                        </div>
                    </div>

                    <div style="padding: 0.5rem;" class="hide-for-small"></div>

                    <!-- purchase and clear  buttons container-->
                    <div class="row" id="buttonContainerPurchase" runat="server">
                        <div class="large-12 columns">
                            <div class="text-right small-only-text-center">
                                <a href="#"id="btnClear" onclick="javascript: clearTotal();" class="button tiny radius loginButton" style="background-color: #2d8c36;">Clear</a>
                                <asp:Button CssClass="button tiny radius loginButton" BackColor="#2d8c36" runat="server" ID="btnPurchase" Text="Purchase" OnClick="btnPurchase_Click"/>
                            </div>
                        </div>
                    </div>

                    <!--ad and otp container-->
                    <div class="row" id="adOTPContainer" runat="server" style="margin: 0 0 0.55rem;display: block;">
                        <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 1.5rem;">
                            <div style="margin: 0 auto;z-index: 0;padding: 0; border: 25px solid #ffffff; border: 25px solid rgba(255, 255, 255, 0.25); -ms-border-radius: 18px; border-radius: 18px; max-width: 24rem;">
                                <div style="margin:0 auto;padding:0;z-index: 10;display: block;max-width:335px;width: 100%;max-height: 150px;height: auto;background: #fff !important;text-align: left;">
                                    <div id="AdDiv" runat="server">
                                        Ad goes here
                                    </div>
                                </div>  
                            </div>
                        </div>
                        <div class="large-6 medium-6 small-12 columns">
                            <div style="margin: 0 auto;z-index: 0;padding: 0; border: 25px solid #ffffff; border: 25px solid rgba(255, 255, 255, 0.25); -ms-border-radius: 18px; border-radius: 18px; max-width: 24rem;">
                                <div style="margin:0 auto;padding: 0.5rem 1.563rem 0;z-index: 10;display: block;height: auto;background: #fff !important;text-align: left;min-height:150px;">
                                    <div id="Div1" runat="server" style="font-size: 1rem;font-weight:bold;color: #222;margin: 0.5rem 0 0.75rem;">
                                        Enter Verification Code
                                    </div>
                                    <div runat="server">
                                        <label>
                                        <input id="txtOtp" type="text" runat="server" value="" onclick="javascript: this.value = ''" style="margin-bottom: 0.5rem;" />
                                        </label>
                                    </div>
                                    <!-- enter otp button container -->
                                    <div style="text-align: center;margin-top: 0.75rem;">
                                        <asp:Button CssClass="button tiny radius loginButton" BackColor="#2d8c36" runat="server" ID="btnVerifyOtp" Text="Submit" OnClick="btnVerifyOtp_Click"/>
                                        <asp:Button CssClass="button tiny radius loginButton" BackColor="#2d8c36" runat="server" ID="btnResendOtp" Text="Resend" OnClick="btnResendOtp_Click"/>
                                    </div>
                                    <div style="clear: both;"></div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <!-- End ad and otp container-->

                    <div class="row" style="display: none;">
                        <div class="large-12 columns" style="text-align: center;">
                            <span id="serverDisplay" runat="server" style="font-size: 0.875rem;color: #fff;">Server: 127.0.0.1</span>
                        </div>
                    </div>
                </fieldset>

                <%--<div style="padding: 0.5rem;"></div>--%>
                <!-- Disclaimer/Copyright -->
                <div id="divDisclaimerOTP" class="row">
                    <div class="large-12 columns" style="text-align: left;">
                        <fieldset style="border-radius: 5px;background-color: #2d521a;"><legend style="background: none;color:#fff;">MAC Disclosure Statement</legend>
                            <div>
                                <p class="golfDisclaimer" style="margin-top: 0;">
                                    <strong>This demonstration website ("Demo") is the copyrighted work of Mobile Authentication Corporation.</strong>
                                </p>
                                <p class="golfDisclaimer">
                                    The information contained in this website is for general information purposes only. While we endeavor to keep the information up to date and correct, we make no representations or warranties of any kind, express or implied, about the completeness, accuracy, reliability, suitability or availability with respect to the website or the information, products, services, or related graphics contained on this website for any purpose. Any reliance you place on such information is therefore strictly at your own risk.
                                </p>
                                <p class="golfDisclaimer">
                                    In no event will we be liable for any loss or damage including without limitation, indirect or consequential loss or damage, or any loss or damage whatsoever arising from loss of data or profits arising out of, or in connection with, the use of this website.
                                </p>
                                <p class="golfDisclaimer">
                                    Links to third party websites are not under the control of Mobile Authentication Corporation. The inclusion of any links does not necessarily imply a recommendation or endorse the views expressed within them.
                                </p>
                                <p class="golfDisclaimer" style="margin-bottom: 0;">
                                    Every effort is made to keep the website up and running smoothly. However, Mobile Authentication Corporation will not be liable for the website being temporarily unavailable due to technical issues beyond our control.
                                </p>
                                <%--<div style="padding: 0.5rem;"></div>
                                <span style="font-size: 1rem;font-weight: bold;color: #fff;">To unsubscribe, <asp:Button CssClass="registerLink" runat="server" ID="btnGoToUnsubscribe2" Text="click here1" Enabled="true" OnClick="btnGoToUnsubscribe_Click"/></span>--%>
                            </div> 
                        </fieldset>
                    </div>
                </div>
                <!-- Disclaimer/Copyright -->
            </div>
        </div>
        <!--end store container-->      
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
        <input id="hiddenHost" runat="server" type="hidden" value="" />
        <!--email validation-->
        <input id="hiddenValidation" runat="server" type="hidden" value="false" />
        <input id="hiddenFeedbackValidation" runat="server" type="hidden" value="false" />
        <input id="hiddenEmailSendError" runat="server" type="hidden" value="false" />
        
         <!-- Send Email Parameters -->
        <input id="hiddenEmailServer" runat="server" type="hidden" value="false" />
        <input id="hiddenEmailPort" runat="server" type="hidden" value="false" />
        <input id="hiddenEmailToAddress" runat="server" type="hidden" value="false" />
        <input id="hiddenEmailFromAddress" runat="server" type="hidden" value="false" />
        <input id="hiddenEmailLoginUserName" runat="server" type="hidden" value="false" />
        <input id="hiddenEmailPassword" runat="server" type="hidden" value="false" />

        <div id="divHiddenFields" style="visibility: hidden;"> 
            <input type="hidden" id="hiddenPANList" value="" runat="server" />
            <input type="hidden" id="hiddenAccountHoldersList" value="" runat="server" />
            <input type="hidden" id="hiddenAccountNamesList" value="" runat="server" />
            <input type="hidden" id="hiddenLoginNamesList" value=""  runat="server" />
            <input type="hidden" id="hiddenBills" value="" runat="server" />
        </div>
    </form>

    <!--bg layer-->
    <div id="bg" class="row" style="position: absolute;top:55px;background: #091c0b;opacity: .9;z-index: -1;border-radius: 5px;">
        &nbsp;
    </div>
    
    <script src="../js/vendor/jquery.js"></script>
    <script src="../js/foundation.min.js"></script>
    <script>
        $(document).foundation();

        $(document).ready(function () {
            bgResize();

            $("#chkContact label").css("color", "white !important");

            for (var i = 1; i < 9; i++) {
                var x = $("#item_number" + i + "_display").html();
                if (x == "") {
                    $("#item_display" + i).hide();
                }
            }

            // validate feedback textarea input
            $("#btnFeedbackSubmit").attr("disabled", "true");
            $("#selectFeedback").keyup(function () {
                if ($(this).val() != "") {
                    $("#btnFeedbackSubmit").removeAttr("disabled");
                } else {
                    $("#btnFeedbackSubmit").attr("disabled", true);
                }
            });

        });

        var store = $("#myFieldset").is(":visible");
        var OTP = $("#divDisclaimerOTP").is(":visible");

        if (store == true) {
            //alert("store");
            var h = $("#serverContainer").innerHeight();

            $('#bg').css('height', h + 45);

            var w = $('#bg').width() / 2;
            var xpos = $(window).innerWidth() / 2 - w;
            $('#bg').css('margin-left', xpos);

        } else if (OTP == true) {
            var disclaimerOffset = $("#divDisclaimerOTP").offset();
            var h = disclaimerOffset.top() - 25;
            alert(h);

            $('#bg').css('height', h);

            var w = $('#bg').width() / 2;
            var xpos = $(window).innerWidth() / 2 - w;
            $('#bg').css('margin-left', xpos);

        } else {
            var h = $(document).height();
            $('#bg').css('height', h);

            var w = $('#bg').width() / 2;
            var xpos = $(window).innerWidth() / 2 - w;
            $('#bg').css('margin-left', xpos);
        }


        $(window).resize(function () {
            var store = $("#myFieldset").is(":visible");

            if (store == true) {
                var h = $("#serverContainer").innerHeight();

                $('#bg').css('height', h + 45);

                var w = $('#bg').width() / 2;
                var xpos = $(window).innerWidth() / 2 - w;
                $('#bg').css('margin-left', xpos);
            } else {
                var h = $(document).height();
                $('#bg').css('height', h);

                var w = $('#bg').width() / 2;
                var xpos = $(window).innerWidth() / 2 - w;
                $('#bg').css('margin-left', xpos);
            }
        });
        
    </script>
    <script src="../js/foundation.min.js"></script>
    <script src="../js/chosen.jquery.js"></script>
    <script src="../js/prism.js"></script>
    <script type="text/javascript">
        $('.chosen-select').chosen({ disable_search_threshold: 28 });
        var config = {
            '.chosen-select': {},
            '.chosen-select-deselect': { allow_single_deselect: true },
            '.chosen-select-no-single': { disable_search_threshold: 10 },
            '.chosen-select-no-results': { no_results_text: 'Oops, nothing found!' },
            '.chosen-select-width': { width: "95%" }
        }
        for (var selector in config) {
            $(selector).chosen(config[selector]);
        }
        function contactMe() {
            var contactMe = $("#chkContact").prop("checked");
            if (contactMe == true) {
                $("#hiddenContactMe").val(" - Information Requested!");
            }
            else {
                $("#hiddenContactMe").val("");
            }
        }
    </script>

  </body>
</html>


