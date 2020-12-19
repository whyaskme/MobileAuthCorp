<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BCL.aspx.cs" Inherits="BL1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="css/foundation_menu.css" rel="stylesheet" />
    <title>Car Loan Landing Page</title>
    <style>
        /*html {
            background: url('Images/bg_wave55.jpg') no-repeat top center fixed;
            -webkit-background-size: auto;
            -moz-background-size: auto;
            -o-background-size: auto;
            background-size: auto;
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
                    <li style="background-color:transparent;"><a href="#" style="background-color:transparent;padding:0 !important;margin:0 !important;"><span style="padding:10px 25px;margin:0 5px 0 0;">RATES &AMP; OPTIONS</span></a></li>
                    <li style="background-color:transparent;"><a href="#" style="background-color:transparent;padding:0 !important;margin:0 !important;"><span style="padding:10px 25px;margin:0 5px 0 0;">CALCULATORS</span></a></li>
                    <li style="background-color:transparent;"><a href="#" style="background-color:transparent;padding:0 !important;margin:0 !important;"><span style="padding:10px 25px;margin:0 5px 0 0;">SERVICES</span></a></li>
                </ul>
            </section>
        
        </nav>
    </div>

    <div class="row">
        <div class="large-4 medium-4 small-12 columns" style="text-align:center;color:#222;margin-top:10px;padding:25px 0 0;">
            <img src="Images/car-loan.jpg" />
        </div>
        <div class="large-8 medium-8 small-12 columns" style="color:#222;margin-top:10px;padding:25px 0 0;">
            <div style="padding:0 1rem;">
                <h3>2.49% APR Auto Loans</h3>
                <p>Great rates with no money down and low monthly payments.</p>
                <p><a href="#" class="button tiny radius" style="background:#c02525;">Learn More</a></p>
            </div>
        </div>
    </div>
    <div class="hide-for-small" style="padding:1rem;"></div>
    <div class="row">
        <div class="large-8 medium-8 small-12 columns">
            <div style="border-top:3px solid #d8d8d8;margin:0 auto 1.5rem;width:95%;"></div>
            <div class="hide-for-small" style="padding:0 2.5rem;">
                <ul>
                    <li><span style="color:#222;">Competitive auto loan rates.</span></li>
                    <li><span style="color:#222;">Existing customers get 15% rate discounts for auto purchases or refinances.</span></li>
                    <li><span style="color:#222;">Same great rates on new and used cars.</span></li>
                    <li><span style="color:#222;">Finance up to 100% with no down payment.</span></li>
                    <li><span style="color:#222;">Automatically deduct payments from your account for additional rate savings.</span></li>
                    <li><span style="color:#222;">Easy-to-use online tools.</span></li>
                </ul>
            </div>
            <div class="show-for-small" style="padding:0 0.5rem;">
                <ul>
                    <li><span style="color:#222;">Competitive auto loan rates.</span></li>
                    <li><span style="color:#222;">Existing customers get 15% rate discounts for auto purchases or refinances.</span></li>
                    <li><span style="color:#222;">Same great rates on new and used cars.</span></li>
                    <li><span style="color:#222;">Finance up to 100% with no down payment.</span></li>
                    <li><span style="color:#222;">Automatically deduct payments from your account for additional rate savings.</span></li>
                    <li><span style="color:#222;">Easy-to-use online tools.</span></li>
                </ul>
            </div>
        </div>
        <div class="large-4 medium-4 small-12 columns panel" style="background:#d8edfe !important;">
            <h3 style="font-size:1.5rem;">Rates As Low As 1.99%</h3>
            <p>As an Online Bank customer, you can receive interest rate discounts on a new auto purchase or refinance loan.</p>
            <p>Whether you are buying a new or used car, Online Bank has the right auto financing solutions for you.</p>
            <p><a href="#">Apply Now ››</a></p>
        </div>
    </div>

    <div class="row hide-for-small">
        <div class="large-12 columns">
            <div style="padding:1rem;margin-bottom:1.5rem;border-bottom:1px dotted #ccc;"></div>
        </div>
    </div>

     <div class="row">
        <div class="large-12 columns">
            <div style="font-size:0.75rem;margin:0 0 1rem;">
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
