<%@ Page Language="C#" AutoEventWireup="true" CodeFile="golf-store.aspx.cs" Inherits="GolfShop_golf_store" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" class="no-js" lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Welcome To The Golf Shop</title>
    <link rel="stylesheet" href="../css/foundation_golf.css" />
    <link href="../css/table-style.css" rel="stylesheet" />
    <script src="../js/vendor/modernizr.js"></script>
    <script src="../js/Demo.js"></script>
    <link href='http://fonts.googleapis.com/css?family=Roboto+Slab' rel='stylesheet' type='text/css' />

</head>
  <body style="">

    <div class="contain-to-grid sticky">
      <nav class="top-bar">
        <ul class="title-area">
          <li class="name">
            <a href="default.aspx" title="Home"><img src="img/golf-logo.png" alt="" style="margin: 13px 0 0 10px;" /></a>
          </li>
           <!-- Remove the class "menu-icon" to get rid of menu icon. Take out "Menu" to just have icon alone -->
          <!--<li class="toggle-topbar menu-icon"><a href="#"><span>Menu</span></a></li>-->
          <li class="toggle-topbar menu-icon"><a href="#"><img src="img/menu-icon1.png" style="position:relative;top:-8px;left:35px;" title="Menu" /></a></li>
        </ul>

        <section class="top-bar-section">
          <!-- Right Nav Section -->
          <ul class="right">
            <li><a href="http://www.mobileauthcorp.com/" target="_blank">MAC</a></li>
          </ul>
        </section>
        
      </nav>
    </div>
    
    <!--login div content-->
    <%--<div id="divStore" runat="server">--%>

    <div class="row" id="divStoreContainer" style="position: relative;margin: 0 auto;text-align: center;z-index: 10;">
        <div class="large-12 columns">
            <div style="padding: 0.5rem;"></div>
    
            <fieldset id="myFieldset"><legend style="background: none;color:#fff;"><span>Golf Equipment</span></legend>
                <div id="productDisplay">
                    <div class="row">
                        <div class="large-3 medium-3 small-12 columns" style="color:#222;">
                            <div style="margin: 0.5rem;background: #fff;">
                                <img src="img/golf-bag-111203.jpg" style="width:204px;" />
                                <div style="padding: 0.5rem;font-size: 0.875rem;">
                                    <div style="margin-bottom: 0.5rem;" id="item-decription1">BLUE GOLF BAG</div>
                                    <div style="margin-bottom: 0.5rem;color: #052441;">Item: #<span id="item-number1">111203</span></div>
                                    <span style="margin-top: 0.125rem;font-weight: bold;" id="item-price1">$129.33</span>
                                    <div style="text-align: center;"><a href="#" id="item1" onclick="javascript: btnItem1();" class="item-thumb-button">Add To Cart</a></div>
                                </div>
                            </div>
                        </div>
                        <div class="large-3 medium-3 small-12 columns" style="color:#222;">
                            <div style="margin: 0.5rem;background: #fff;">
                                <img src="img/golf-balls-111111.jpg" />
                                <div style="padding: 0.5rem;font-size: 0.875rem;">
                                    <div style="margin-bottom: 0.5rem;" id="item-decription2">GOLF BALLS</div>
                                    <div style="margin-bottom: 0.5rem;color: #052441;">Item: #<span id="item-number2">111111</span></div>
                                    <span style="margin-top: 0.125rem;font-weight: bold;" id="item-price2">$39.00</span>
                                    <div style="text-align: center;"><a href="#" id="item2" onclick="javascript: btnItem2();" class="item-thumb-button">Add To Cart</a></div>
                                </div>
                            </div>
                        </div>
                        <div class="large-3 medium-3 small-12 columns" style="color:#222;">
                            <div style="margin: 0.5rem;background: #fff;">
                                <img src="img/kids-clubs-111200.jpg" />
                                <div style="padding: 0.5rem;font-size: 0.875rem;">
                                    <div style="margin-bottom: 0.5rem;" id="item-decription3">KIDS GOLF CLUB SET</div>
                                    <div style="margin-bottom: 0.5rem;color: #052441;">Item: #<span id="item-number3">111200</span></div>
                                    <span style="margin-top: 0.125rem;font-weight: bold;" id="item-price3">$69.99</span>
                                    <div style="text-align: center;"><a href="#" id="item3" onclick="javascript: btnItem3();" class="item-thumb-button">Add To Cart</a></div>
                                </div>
                            </div>
                        </div>
                        <div class="large-3 medium-3 small-12 columns" style="color:#222;">
                            <div style="margin: 0.5rem;background: #fff;">
                                <img src="img/mens-golf-shoes-111202.jpg" />
                                <div style="padding: 0.5rem;font-size: 0.875rem;">
                                    <div style="margin-bottom: 0.5rem;" id="item-decription4">MEN'S GOLF SHOES</div>
                                    <div style="margin-bottom: 0.5rem;color: #052441;">Item: #<span id="item-number4">111202</span></div>
                                    <span style="margin-top: 0.125rem;font-weight: bold;" id="item-price4">$89.99</span>
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
                                    <div style="margin-bottom: 0.5rem;" id="item-decription5">TEE PACKAGE</div>
                                    <div style="margin-bottom: 0.5rem;color: #052441;">Item: #<span id="item-number5">111170</span></div>
                                    <span style="margin-top: 0.125rem;font-weight: bold;" id="item-price5">$7.98</span>
                                    <div style="text-align: center;"><a href="#" id="item5" onclick="javascript: btnItem5();" class="item-thumb-button">Add To Cart</a></div>
                                </div>
                            </div>
                        </div>
                        <div class="large-3 medium-3 small-12 columns" style="color:#222;">
                            <div style="margin: 0.5rem;background: #fff;">
                                <img src="img/water-bottle-111300.jpg" />
                                <div style="padding: 0.5rem;font-size: 0.875rem;">
                                    <div style="margin-bottom: 0.5rem;" id="item-decription6">KEEP COOL BOTTLE</div>
                                    <div style="margin-bottom: 0.5rem;color: #052441;">Item: #<span id="item-number6">111300</span></div>
                                    <span style="margin-top: 0.125rem;font-weight: bold;" id="item-price6">$12.00</span>
                                    <div style="text-align: center;"><a href="#" id="item6" onclick="javascript: btnItem6();" class="item-thumb-button">Add To Cart</a></div>
                                </div>
                            </div>
                        </div>
                        <div class="large-3 medium-3 small-12 columns" style="color:#222;">
                            <div style="margin: 0.5rem;background: #fff;">
                                <img src="img/womens-hat-111130.jpg" />
                                <div style="padding: 0.5rem;font-size: 0.875rem;">
                                    <div style="margin-bottom: 0.5rem;" id="item-decription7">RED HAT</div>
                                    <div style="margin-bottom: 0.5rem;color: #052441;">Item: #<span id="item-number7">111130</span></div>
                                    <span style="margin-top: 0.125rem;font-weight: bold;" id="item-price7">$14.50</span>
                                    <div style="text-align: center;"><a href="#" id="item7" onclick="javascript: btnItem7();" class="item-thumb-button">Add To Cart</a></div>
                                </div>
                            </div>
                        </div>
                        <div class="large-3 medium-3 small-12 columns" style="color:#222;">
                            <div style="margin: 0.5rem;background: #fff;">
                                <img src="img/womens-visor-111118.jpg" />
                                <div style="padding: 0.5rem;font-size: 0.875rem;">
                                    <div style="margin-bottom: 0.5rem;" id="item-decription8">LADY'S GOLF VISOR</div>
                                    <div style="margin-bottom: 0.5rem;color: #052441;">Item: #<span id="item-number8">111118</span></div>
                                    <span style="margin-top: 0.125rem;font-weight: bold;" id="item-price8">$19.99</span>
                                    <div style="text-align: center;"><a href="#" id="item8" onclick="javascript: btnItem8();" class="item-thumb-button">Add To Cart</a></div>
                                </div>
                            </div>
                        </div> 
                    </div>
                </div>

                <div style="padding: 0.75rem;" id="tableSpacer"></div>

                 <div class="row">
                    <div class="large-12 columns" style="text-align: right;padding-right: 1.5rem;">
                        <table style="width: 100%;border-collapse: collapse;margin-bottom:0;margin-left: .5rem;"><thead><tr><th>Item #</th><th>Description</th><th>Quantity</th><th style="text-align: right;">Subtotal</th></tr></thead>
                            <tbody>
                                <tr id="item-display1" style="display: none;">
                                    <td style="width:100px;" class="small-only-text-left" id="item-number1-display"></td>
                                    <td id="item-decription1-display"></td>
                                    <td style="width:45px;text-align: center;" class="small-only-text-left" id="item-quantity1">0</td>
                                    <td style="width:155px;text-align: right;" class="small-only-text-left" id="item-subtotal1">0.00</td>
                                </tr>
                                <tr id="item-display2" style="display: none;">
                                    <td style="width:100px;" class="small-only-text-left" id="item-number2-display"></td>
                                    <td id="item-decription2-display"></td>
                                    <td style="width:45px;text-align: center;" class="small-only-text-left" id="item-quantity2">0</td>
                                    <td style="width:155px;text-align: right;" class="small-only-text-left" id="item-subtotal2">0.00</td>
                                </tr>
                                <tr id="item-display3" style="display: none;">
                                    <td style="width:100px;" class="small-only-text-left" id="item-number3-display"></td>
                                    <td id="item-decription3-display"></td>
                                    <td style="width:45px;text-align: center;" class="small-only-text-left" id="item-quantity3">0</td>
                                    <td style="width:155px;text-align: right;" class="small-only-text-left" id="item-subtotal3">0.00</td>
                                </tr>
                                <tr id="item-display4" style="display: none;">
                                    <td style="width:100px;" class="small-only-text-left" id="item-number4-display"></td>
                                    <td id="item-decription4-display"></td>
                                    <td style="width:45px;text-align: center;" class="small-only-text-left" id="item-quantity4">0</td>
                                    <td style="width:155px;text-align: right;" class="small-only-text-left" id="item-subtotal4">0.00</td>
                                </tr>
                                <tr id="item-display5" style="display: none;">
                                    <td style="width:100px;" class="small-only-text-left" id="item-number5-display"></td>
                                    <td id="item-decription5-display"></td>
                                    <td style="width:45px;text-align: center;" class="small-only-text-left" id="item-quantity5">0</td>
                                    <td style="width:155px;text-align: right;" class="small-only-text-left" id="item-subtotal5">0.00</td>
                                </tr>
                                <tr id="item-display6" style="display: none;">
                                    <td style="width:100px;" class="small-only-text-left" id="item-number6-display"></td>
                                    <td id="item-decription6-display"></td>
                                    <td style="width:45px;text-align: center;" class="small-only-text-left" id="item-quantity6">0</td>
                                    <td style="width:155px;text-align: right;" class="small-only-text-left" id="item-subtotal6">0.00</td>
                                </tr>
                                <tr id="item-display7" style="display: none;">
                                    <td style="width:100px;" class="small-only-text-left" id="item-number7-display"></td>
                                    <td id="item-decription7-display"></td>
                                    <td style="width:45px;text-align: center;" class="small-only-text-left" id="item-quantity7">0</td>
                                    <td style="width:155px;text-align: right;" class="small-only-text-left" id="item-subtotal7">0.00</td>
                                </tr>
                                <tr id="item-display8" style="display: none;">
                                    <td style="width:100px;" class="small-only-text-left" id="item-number8-display"></td>
                                    <td id="item-decription8-display"></td>
                                    <td style="width:45px;text-align: center;" class="small-only-text-left" id="item-quantity8">0</td>
                                    <td style="width:155px;text-align: right;" class="small-only-text-left" id="item-subtotal8">0.00</td>
                                </tr>
                            </tbody>
                            <tfoot style="text-align:right;" id="totalContainer">
                                <tr>
                                    <td colspan="4">
                                        <div class="show-for-small" style="border-top:;"></div>
                                        <div id="scrollHere" style="text-align: right;" class="small-only-text-left">
                                            <strong>Total:</strong> $<span id="totalPrice">0.00</span>
                                        </div>
                                    </td>
                                </tr>
                            </tfoot>
                        </table>
                    </div>
                </div>

                <div style="padding: 1rem;"></div>

                <!--purchase and clear buttons container-->
                <div class="row" id="buttonContainerPurchase">
                    <div class="large-12 columns" style="text-align: right;padding-right: 1.5rem;">
                        <a href="#" onclick="javascript: clearTotal();" class="button tiny radius" style="background-color: #2d8c36;">Clear</a>
                        <a href="#" onclick="btnPurchase_Click" class="button tiny radius" style="background-color: #2d8c36;">Purchase</a>
                    </div>
                </div>

                <!--add and otp container-->
                <div class="row" id="adOTPContainer" style="margin: 0 0 0.55rem;display: none;">
                    <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 1.5rem;" id="contentAd">
                        <a href="#"><img src="img/ads/golf-ad.jpg" alt="" /></a>
                    </div>
                    <div class="large-6 medium-6 small-12 columns">
                        <div style="margin: 0 auto;z-index: 0;padding: 0; border: 25px solid #ffffff; border: 25px solid rgba(255, 255, 255, 0.25); -ms-border-radius: 18px; border-radius: 18px; max-width: 24rem;">

                            <div style="margin:0 auto;padding: 0.5rem 1.563rem 0;z-index: 10;display: block;height: auto;background: #fff !important;text-align: left;">

                                <div id="AdminLoginResult_Desktop" runat="server" style="font-size: 1rem;font-weight:bold;color: #222;margin: 0.5rem 0 0.75rem;">
                                    Enter OTP
                                </div>

                                <div runat="server">
                                    <label>
                                    <input id="txtUsername" type="text" runat="server" value="" onclick="javascript: this.value = ''" style="margin-bottom: 0.5rem;" />
                                    </label>
                                </div>

                                <!-- enter otp button container -->
                                <div style="text-align: center;margin-top: 0.75rem;">
                                    <input id="btnAdminOtpRequest_Desktop" type="button" onclick="javascript: submitOtpRequest('Desktop');" runat="server" value="Enter" class="button tiny radius" style="background-color: #2d8c36;" />
                                </div>


                                <div style="clear: both;"></div>

                            </div>

                        </div>
                    </div>
                </div>

                <div class="row" style="display: none;">
                    <div class="large-12 columns" style="text-align: center;">
                        <span id="spanServerIp" runat="server" style="font-size: 0.875rem;color: #fff;">Server: 127.0.0.1</span>
                    </div>
                </div>
            </fieldset>
        </div>
    </div>

    <div id="bg" class="row" style="position: absolute;top:55px;background: #091c0b;opacity: .75;z-index: 1;border-radius: 5px;">
        &nbsp;
    </div>    
    
    <script src="../js/vendor/jquery.js"></script>
    <script src="../js/foundation.min.js"></script>
    <script>
        $(document).foundation();

        $(document).ready(function () {
            bgResize();
        });

        var h = $(document).height();
        $('#bg').css('height', h);

        var w = $('#bg').width()/2;
        var xpos = $(window).innerWidth()/2 - w;
        $('#bg').css('margin-left', xpos);

        $(window).resize(function() {
            var h = $(document).height();
            $('#bg').css('height', h);

            var w = $('#bg').width()/2;
            var xpos = $(window).innerWidth()/2 - w;
            $('#bg').css('margin-left', xpos);
        });
    </script>
  </body>
</html>


