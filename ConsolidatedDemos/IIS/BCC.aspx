<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BCC.aspx.cs" Inherits="BL1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="css/foundation_menu.css" rel="stylesheet" />
    <title>Credit Card Landing Page</title>
    <style>
        /*html {
            background: url('Images/bg_wave55.jpg') no-repeat top center fixed;
            -webkit-background-size: auto;
            -moz-background-size: auto;
            -o-background-size: auto;
            background-size: auto;
        }*/
        /*html {
            background: rgb(123, 155, 179);
            background: -moz-linear-gradient(90deg, rgb(123, 155, 179) 2%, rgb(255, 255, 255) 55%);
            background: -webkit-linear-gradient(90deg, rgb(123, 155, 179) 2%, rgb(255, 255, 255) 55%);
            background: -o-linear-gradient(90deg, rgb(123, 155, 179) 2%, rgb(255, 255, 255) 55%);
            background: -ms-linear-gradient(90deg, rgb(123, 155, 179) 2%, rgb(255, 255, 255) 55%);
            background: linear-gradient(180deg, rgb(123, 155, 179) 2%, rgb(255, 255, 255) 55%);
        }*/
        /*html {
            background: url('Images/bg-family1.jpg') repeat-x top center fixed;            
            -webkit-background-size: cover;
            -moz-background-size: cover;
            -o-background-size: cover;
            background-size: cover;
        }*/
        html {
            background: url('Images/bg-gradient.jpg') repeat-x top center fixed;
        }
        h3 {
            font-size:2rem;
            color:#160000;
            line-height:2rem;
        }
        h4 {
            font:18px/22px Arial,Helvetica,sans-serif;
        }
        ul {
            line-height:2.25rem;
            list-style:square;
            color:#555;
        }
        li {
            padding-left:1rem;
        }
    </style>
</head>
<body>
    <div class="contain-to-grid sticky" style="background-color:#1b4d8c;height:100px;">
        <nav class="top-bar" style="background-color:#1b4d8c;">
            <ul class="title-area">
                <li class="name">
                    <%--<div id="divLogoMac" runat="server"><a href="#"><img src="Images/logo-bank.png" alt="" style="margin:25px 0 0 10px;" /></a></div>--%>
                    <div><h3 style="margin:33px 0 0 10px;color:#fff;">Online Bank</h3></div>
                </li>
                <!-- Remove the class "menu-icon" to get rid of menu icon. Take out "Menu" to just have icon alone -->
                <!--<li class="toggle-topbar menu-icon"><a href="#"><span>Menu</span></a></li>-->
                <li class="toggle-topbar menu-icon"><a href="#"><img src="GolfShop/img/menu-icon1.png" style="position:relative;top:0;left:0;" title="Menu" /></a></li>
            </ul>

            <section class="top-bar-section hide-for-small">
                <!-- Right Nav Section -->
                <ul class="right" style="margin-top:30px;">
                    <li style="background-color:transparent;"><a href="#" style="background-color:transparent;padding:0 !important;margin:0 !important;"><span style="padding:10px 25px;margin:0 5px 0 0;">HOME</span></a></li>
                    <li style="background-color:transparent;"><a href="#" style="background-color:transparent;padding:0 !important;margin:0 !important;"><span style="padding:10px 25px;margin:0 5px 0 0;">VIEW CARDS</span></a></li>
                    <li style="background-color:transparent;"><a href="#" style="background-color:transparent;padding:0 !important;margin:0 !important;"><span style="padding:10px 25px;margin:0 5px 0 0;">LEARN MORE</span></a></li>
                    <li style="background-color:transparent;"><a href="#" style="background-color:transparent;padding:0 !important;margin:0 !important;"><span style="padding:10px 25px;margin:0 5px 0 0;">MANAGE ACCOUNT</span></a></li>
                </ul>
            </section>
        
        </nav>
    </div>
    <div class="row">
        <div class="large-4 medium-4 small-12 columns" style="text-align:center;color:#222;margin-top:10px;padding:25px 0 0;">
            <img src="Images/creditCard.png" />
        </div>
        <div class="large-8 medium-8 small-12 columns" style="color:#222;margin-top:10px;padding:25px 0 0;">
            <div style="padding:0 1rem;">
                <h3>0% APR Credit Cards‎</h3>
                <p>Apply for a credit card by completing our secure online application.</p>
                <p><a href="#" class="button tiny radius" style="background:#c02525;">Apply Now</a></p>
            </div>
        </div>
    </div>
    <div class="hide-for-small" style="padding:1rem;"></div>
    <div class="row">
        <div class="large-8 medium-8 small-12 columns">
            <div style="border-top:3px solid #d8d8d8;margin:0 auto 1.5rem;width:95%;"></div>
            <div class="hide-for-small" style="padding:0 3.5rem 0   2.5rem;">
                <ul>
                    <li><span style="color:#222;">1% cash back on all purchases.</span></li>
                    <li><span style="color:#222;">2% on groceries and 3% on gas for the first $1,000 in combined purchases per quarter.</span></li>
                    <li><span style="color:#222;">One-time $250 bonus after purchases of $1,500*.</span></li>
                    <%--<li><span style="color:#222;">Select specific features like low intro rates, cash back, and more.</span></li>--%>
                    <li><span style="color:#222;">Get 0% Intro APR on Balance Transfers for 18 months.</span></li>
                    <li><span style="color:#222;">No annual fee.</span></li>
                </ul>
            </div>
            <div class="show-for-small" style="padding:0 0.5rem;">
                <ul>
                    <li><span style="color:#222;">1% cash back on all purchases.</span></li>
                    <li><span style="color:#222;">2% on groceries and 3% on gas for the first $1,000 in combined purchases per quarter.</span></li>
                    <li><span style="color:#222;">One-time $250 bonus after purchases of $1,500*.</span></li>
                    <%--<li><span style="color:#222;">Select specific features like low intro rates, cash back, and more.</span></li>--%>
                    <li><span style="color:#222;">Get 0% Intro APR on Balance Transfers for 18 months.</span></li>
                    <li><span style="color:#222;">No annual fee.</span></li>
                </ul>
            </div>
        </div>
        <div class="large-4 medium-4 small-12 columns panel">
            <h3>Rewards Card</h3>
            <h4>$100 cash rewards</h4>
            <p style="margin-top:1.5rem;">Earn cash back for all of the things that you buy.</p>
            <p>Low introductory APR</p>
            <%--<p>If you're a Bank of America Preferred Rewards client, you could increase that bonus to 25% to 75%.</p>--%>
            <p><a href="#">Learn More ››</a></p>
        </div>
    </div>

    <div class="row hide-for-small">
        <div class="large-12 columns">
            <div style="padding:1rem;margin-bottom:1.5rem;"></div>
        </div>
    </div>

     <div class="row">
        <div class="large-12 columns">
            <div style="font-size:0.75rem;color:#555;margin:0 0 1rem;">
                <script type="text/javascript">
                    <!--
                    var currentDate = new Date();
                    var year = currentDate.getFullYear();
                    document.write("&copy; " + year + " Online Bank. All rights reserved.");
                    //-->
                </script>
            </div>
        </div>
    </div>

</body>
</html>
