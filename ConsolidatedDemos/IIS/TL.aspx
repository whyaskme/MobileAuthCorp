<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TL.aspx.cs" Inherits="TL1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="css/foundation_menu.css" rel="stylesheet" />
    <title>General Advertisement Landing Page</title>
    <style>
        html {
            background: url('Images/bg_world70.jpg') no-repeat top center fixed !important;
            -webkit-background-size: auto !important;
            -moz-background-size: auto !important;
            -o-background-size: auto !important;
            background-size: auto !important;
        }
    </style>
</head>
<body>
    <div class="contain-to-grid sticky" style="background-color:transparent;height:100px;">
        <nav class="top-bar" style="background-color:transparent;">
            <ul class="title-area">
                <li class="name">
                    <div id="divLogoAvenueB" runat="server"><a href="#"><img src="Images/logo-avenueB.png" alt="" style="margin:33px 0 0 10px;" /></a></div>
                    <div id="divLogoMac" runat="server"><a href="#"><img src="Images/logo-mac.png" alt="" style="margin:25px 0 0 10px;" /></a></div>
                    <div id="divLogoSts" runat="server"><a href="#"><img src="Images/logo-sts.png" alt="" style="margin:36px 0 0 10px;" /></a></div>
                    <div id="divLogotns" runat="server"><a href="#"><img src="Images/logo-tns.png" alt="" style="margin:25px 0 0 10px;" /></a></div>
                    <div id="divLogoGolfShop" runat="server"><a href="#"><img src="Images/logo-golfShop.png" alt="" style="margin:28px 0 0 10px;" /></a></div>
                    <div id="divLogoGolfStore" runat="server"><a href="#"><img src="Images/logo-golfStore.png" alt="" style="margin:28px 0 0 10px;" /></a></div>
                </li>
                <!-- Remove the class "menu-icon" to get rid of menu icon. Take out "Menu" to just have icon alone -->
                <!--<li class="toggle-topbar menu-icon"><a href="#"><span>Menu</span></a></li>-->
                <li class="toggle-topbar menu-icon"><a href="#"><img src="GolfShop/img/menu-icon1.png" style="position:relative;top:25px;left:0;" title="Menu" /></a></li>
            </ul>

            <section class="top-bar-section hide-for-small">
                <!-- Right Nav Section -->
                <ul class="right" style="margin-top:35px;">
                    <li style="background-color:transparent;"><a href="#" style="background-color:transparent;padding:0 !important;margin:0 !important;"><span style="padding:10px 25px;margin:0 5px 0 0;">HOME</span></a></li>
                    <li style="background-color:transparent;"><a href="#" style="background-color:transparent;padding:0 !important;margin:0 !important;"><span style="padding:10px 25px;margin:0 5px 0 0;">ABOUT</span></a></li>
                    <li style="background-color:transparent;"><a href="#" style="background-color:transparent;padding:0 !important;margin:0 !important;"><span style="padding:10px 25px;margin:0 5px 0 0;">BLOG</span></a></li>
                    <li style="background-color:transparent;"><a href="#" style="background-color:transparent;padding:0 !important;margin:0 !important;"><span style="padding:10px 25px;margin:0 5px 0 0;">CONTACT</span></a></li>
                </ul>
            </section>
        
        </nav>
    </div>


    <div class="row">
        <div class="large-2 medium-2 hide-for-small columns">&nbsp;</div>
        <div class="large-8 medium-8 small-12 columns" style="text-align:center;color:#fff;margin-top:35px;padding:25px 0;border-bottom:1px solid #fff;border-top:1px solid #fff;">
            <div id="divDiscount25" runat="server" class="keywordTitle"><img src="Images/25discount.png" alt="" /></div>
            <div id="divSpecialOffer" runat="server" class="keywordTitle"><img src="Images/specialOffer.png" alt="" /></div>
            <div id="divGeneric" runat="server" class="keywordTitle"><span id="spanClientGeneric" runat="server">x</span> Landing Page <span id="spanTypeNum" runat="server"></span></div>
        </div>
        <div class="large-2 medium-2 hide-for-small columns">&nbsp;</div>
    </div>
    <div style="padding:1rem;"></div>
    <div class="row">
        <div class="large-12 columns">
            <p id="profileAvenueB" runat="server" class="content">
               <strong>Avenue B</strong> Consulting, Inc. is a management consulting firm focusing on payment systems products, services and technologies. The company offers a variety of services tailored to the payment system environment including business and strategic planning, new product development, product market validation, business and IT assessments, and competitive analyses.
            </p>
            <p id="profileMac" runat="server" class="content">
                <strong>Mobile Authentication Corp.</strong> is the Owner of the Exclusive Patent Rights for the US Financial Services Industry to U.S. Patent #6,993,658 “The Use of Personal Communication Devices for User Authentication” used for SMS Out-of-Band, One-Time Password (OTP) generation to secure Retail Merchant Payments and Online Banking on the Internet, on Mobile Wallets, and at the Physical Point-of-Contact.
            </p>
            <p id="profileTns" runat="server" class="content">
                <strong>TNS</strong> has been delivering industry-leading solutions for the payments, financial and telecommunications industries since 1990. TNS is the preferred supplier of networking, integrated data and voice services to many leading organizations in the global payments and financial communities, as well as a provider of extensive telecommunications network solutions to service providers
            </p>
            <p id="profileSts" runat="server" class="content">
                <strong>STS</strong> is one of the world’s leading independent payment service providers. We help online businesses succeed through cutting-edge technology, world-leading expertise and a culture dedicated to trust.
            </p>
            <p id="profileGolfShop" runat="server" class="content">
                <strong>Scottsdale Golf Shop</strong> carries a world-class selection of brand name golf equipment, apparel, accessories and gifts for golfers of all ages and abilities. Whatever your golf needs, Scottsdale Golf Shop has you covered.
            </p>
            <p id="profileGolfStore" runat="server" class="content">
                <strong>Scottsdale Golf Sore</strong> carries a world-class selection of brand name golf equipment, apparel, accessories and gifts for golfers of all ages and abilities. Whatever your golf needs, Scottsdale Golf Shop has you covered.
            </p>
            <p style="text-align:center;">
                <a href="#" class="button tiny radius">Read More</a>
            </p>
            <p>&nbsp;</p>
        </div>
    </div>

     <%--<div id="boxes" class="row">
        <div class="large-4 medium-4 small-12 columns">
            <div style="border:1px solid #999;display:block;height:155px;">
                sdg
            </div>
        </div>
        <div class="large-4 medium-4 small-12 columns">
            <div style="border:1px solid #999;display:block;height:155px;">
                sdg
            </div>
        </div>
        <div class="large-4 medium-4 small-12 columns">
            <div style="border:1px solid #999;display:block;height:155px;">
                sgd
            </div>
        </div>
    </div>--%>

</body>
</html>
