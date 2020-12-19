<%@ Page Language="C#" AutoEventWireup="true" CodeFile="LP-test.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="css/foundation_menu.css" rel="stylesheet" />
    <title>Advertisement Landing Page</title>
</head>
<body>
    <div class="contain-to-grid sticky" style="background-color:transparent;height:100px;">
        <nav class="top-bar" style="background-color:transparent;">
            <ul class="title-area">
                <li class="name">
                    <div id="logoAvenueB" style="display:block;"><a href="#"><img src="Images/logo-avenueB.png" alt="" style="margin:33px 0 0 10px;" /></a></div>
                    <div id="logoMac" style="display:none;"><a href="#"><img src="Images/logo-mac.png" alt="" style="margin:25px 0 0;" /></a></div>
                    <div id="logoSts" style="display:none;"><a href="#"><img src="Images/logo-sts.png" alt="" style="margin:36px 0 0;" /></a></div>
                    <div id="logotns" style="display:none;"><a href="#"><img src="Images/logo-tns.png" alt="" style="margin:25px 0 0;" /></a></div>
                </li>
                <!-- Remove the class "menu-icon" to get rid of menu icon. Take out "Menu" to just have icon alone -->
                <!--<li class="toggle-topbar menu-icon"><a href="#"><span>Menu</span></a></li>-->
                <li class="toggle-topbar menu-icon"><a href="#"><img src="GolfShop/img/menu-icon1.png" style="position:relative;top:25px;left:0;" title="Menu" /></a></li>
            </ul>

            <section class="top-bar-section hide-for-small">
                <!-- Right Nav Section -->
                <ul class="right" style="margin-top:35px;">
                    <li style="background-color:transparent;"><a href="#" target="_blank" style="background-color:transparent;padding:0 !important;margin:0 !important;"><span style="padding:10px 25px;margin:0 5px 0 0;">HOME</span></a></li>
                    <li style="background-color:transparent;"><a href="#" target="_blank" style="background-color:transparent;padding:0 !important;margin:0 !important;"><span style="padding:10px 25px;margin:0 5px 0 0;">ABOUT</span></a></li>
                    <li style="background-color:transparent;"><a href="#" target="_blank" style="background-color:transparent;padding:0 !important;margin:0 !important;"><span style="padding:10px 25px;margin:0 5px 0 0;">BLOG</span></a></li>
                    <li style="background-color:transparent;"><a href="#" target="_blank" style="background-color:transparent;padding:0 !important;margin:0 !important;"><span style="padding:10px 25px;margin:0 5px 0 0;">CONTACT</span></a></li>
                </ul>
            </section>
        
        </nav>
    </div>


    <div class="row">
        <div class="large-3 medium-2 hide-for-small columns">&nbsp;</div>
        <div class="large-6 medium-8 small-12 columns" style="text-align:center;color:#fff;margin-top:35px;padding:25px 0;border-bottom:1px solid #fff;border-top:1px solid #fff;">
            <%--<div id="discount25" class="keywordTitle">25% Discount from <span id="client-Discount25">Avenue B</span></div>--%>
            <div id="specialOffer" class="keywordTitle">Special Offer from <span id="client-specialOffer">Avenue B</span>!</div>
        </div>
        <div class="large-3 medium-2 hide-for-small columns">&nbsp;</div>
    </div>
    <div style="padding:1rem;"></div>
    <div class="row">
        <div class="large-12 columns">
            <p class="content">
                Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas molestie metus et lacus dapibus dictum. Ut neque purus, sollicitudin non ante ac, malesuada condimentum libero. Nunc hendrerit augue vel tempor posuere. Cras consectetur vel odio scelerisque placerat. Vestibulum non aliquet orci.
            </p>
            <p style="text-align:center;">
                <a href="#" class="button tiny radius">Read More</a>
            </p>
        </div>
    </div>

    <%--<div class="row" style="position:absolute;left:50%;top:575px;">
        <div class="large-4 medium-4 small-12 columns">
            <div style="border:1px solid #999;display:block;height:155px;">

            </div>
        </div>
        <div class="large-4 medium-4 small-12 columns">
            <div style="border:1px solid #999;display:block;height:155px;">

            </div>
        </div>
        <div class="large-4 medium-4 small-12 columns">
            <div style="border:1px solid #999;display:block;height:155px;">

            </div>
        </div>
    </div>--%>

</body>
</html>
