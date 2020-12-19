<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GLS.aspx.cs" Inherits="GLS" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" class="no-js" lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Welcome To The Golf Shop</title>
    <link rel="stylesheet" href="css/foundation_golf.css" />
    <link href="css/MAC-golf.css" rel="stylesheet" />
    <link href="css/table-style.css" rel="stylesheet" />
    <script src="js/vendor/modernizr.js"></script>
    <script src="js/Demo.js"></script>
    <link href='http://fonts.googleapis.com/css?family=Roboto+Slab' rel='stylesheet' type='text/css' />

</head>
  <body style="">

    <div class="contain-to-grid sticky">
      <nav class="top-bar">
        <ul class="title-area">
          <li class="name">
            <a href="default.aspx" title="Home"><img src="GolfShop/img/golf-logo.png" alt="" style="margin: 13px 0 0 10px;" /></a>
          </li>
           <!-- Remove the class "menu-icon" to get rid of menu icon. Take out "Menu" to just have icon alone -->
          <!--<li class="toggle-topbar menu-icon"><a href="#"><span>Menu</span></a></li>-->
          <li class="toggle-topbar menu-icon"><a href="#"><img src="GolfShop/img/menu-icon1.png" style="position:relative;top:-8px;left:35px;" title="Menu" /></a></li>
        </ul>

        <section class="top-bar-section">
          <!-- Right Nav Section -->
          <ul class="right">
            <li><a href="http://www.mobileauthcorp.com/" target="_blank">MAC</a></li>
            <%--<li class="active"><a href="#">Shopping</a></li>--%>
            <%--<li><a href="#">View Cart</a></li>
            <li><a href="#">Orders</a></li>
            <li><a href="#">My Account</a></li>--%>
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
    
    <!--login div content-->
    <%--<div id="divStore" runat="server">--%>

    <div class="row" id="divStoreContainer" style="position: relative;margin: 0 auto;text-align: center;z-index: 10;">
        <div class="large-12 columns">    
            <fieldset id="myFieldset" style="margin-top:1.5rem;padding:0.25rem 1.125rem;"><%--<legend style="background: none;color:#fff;"><span>Specials</span></legend>--%>
                <!-- ad specials div -->
                <div id="divAdSpecials" style="position: relative;margin: 1rem auto;padding:0.5rem;z-index: 10;" runat="server">
                    <div class="row">
                        <div class="large-12 columns">
                            <h1 style="color: #fff;font-size: 36px;font-weight: bold;"><em>Scottsdale Golf Shop Current Specials.</em></h1>
                        </div>
                    </div>
                    <div style="padding: 0.5rem;"></div>
                    <div class="row">
                        <div class="large-4 medium-4 small-12 columns" style="padding:0.5rem;">
                            <div class="specials">
                                <a href="#"><img src="GolfShop/img/specials/special1.png" style="width: 100%;max-width: 270px;" /></a>
                            </div>
                        </div>
                        <div class="large-4 medium-4 small-12 columns" style="padding:0.5rem;">
                            <div class="specials">
                                <a href="#"><img src="GolfShop/img/specials/special2.png" style="width: 100%;max-width: 270px;" /></a>
                            </div>
                        </div>
                        <div class="large-4 medium-4 small-12 columns" style="padding:0.5rem;">
                            <div class="specials">
                                <a href="#"><img src="GolfShop/img/specials/special3.png" style="width: 100%;max-width: 270px;" /></a>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="large-4 medium-4 small-12 columns" style="padding:0.5rem;">
                            <div class="specials">
                                <a href="#"><img src="GolfShop/img/specials/special4.png" style="width: 100%;max-width: 270px;" /></a>
                            </div>
                        </div>
                        <div class="large-4 medium-4 small-12 columns" style="padding:0.5rem;">
                            <div class="specials">
                                <a href="#"><img src="GolfShop/img/specials/special5.png" style="width: 100%;max-width: 270px;" /></a>
                            </div>
                        </div>
                        <div class="large-4 medium-4 small-12 columns" style="padding:0.5rem;">
                            <div class="specials">
                                <a href="#"><img src="GolfShop/img/specials/special6.png" style="width: 100%;max-width: 270px;" /></a>
                            </div>
                        </div>
                    </div>
                    <div style="padding: 0.5rem;"></div>
                </div>
                <!--end ad specials div-->
            </fieldset>
        </div>
    </div>

    <div id="bg" class="row" style="position: absolute;top:55px;background: #091c0b;opacity: .75;z-index: 1;border-radius: 5px;">
        &nbsp;
    </div>    
    
    <script src="js/vendor/jquery.js"></script>
    <script src="js/foundation.min.js"></script>
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

