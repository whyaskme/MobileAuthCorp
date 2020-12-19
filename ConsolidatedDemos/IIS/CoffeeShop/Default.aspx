<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="CoffeeShop_Default" %>

<!DOCTYPE html>

<html class="no-js" lang="en">
  <head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Coffee Shop Demo : Mobile Authentication Corporation</title>
    <link href="css/table-style.css" rel="stylesheet" />
    <link href="css/loading.css" rel="stylesheet" />
    <link href="foundation/css/foundation.css" rel="stylesheet" />
    <link href="css/main.css" rel="stylesheet" />
    <script src="js/jquery-1.10.2.js"></script>
  </head>
<body style="width:100% !important;">

    <!--loading lightbox-->
    <div id="divPleaseWaitProcessing" class="lightbox_background">        
        <%--<div style="font-size:5.5rem;color:#fff;text-align:center;position:relative;z-index:11000;margin:5.5rem auto;border:1px solid #fff;">Processing...</div>--%>
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

    <%--error display--%>
    <div id="divErrorMessage" runat="server" class="errorMessage" onclick="javascript: hideError();" title="Click to hide error">
        <asp:Label ID="lbError" runat="server" ForeColor="#401712" Text="" />
    </div>

    <!--item added to cart-->
    <div id="divItemAddedToCartDisplay" runat="server" class="itemAddedToCart">
        <div class="row">
            <div class="large-4 medium-4 small-12 columns small-only-text-center">
                <img id="imgItemAddedToCartDisplay" src="Images/" alt="" style="width:150px;border:1px solid #e3d4c1;" />
                <div style="margin-bottom:10px;"></div>
            </div>
            <div class="large-8 medium-8 small-12 columns small-only-text-center">
                <h3 style="font-size:0.875rem;">Item added to cart (Total: <span id="spanAddItemCartTotal">1</span>)</h3>
                <span id="spanAddItemTitle"></span><br />
                Qty: <span id="spanAddItemQty">1</span><br />
                Price: $<span id="spanAddItemPrice">19.99</span>
                <div style="margin-top:0.813rem">
                    <a href="#" class="button tiny radius" style="width:75px;" onclick="javascript:hideItemAddedToCartDisplay();">Close</a>
                    <a href="#" class="button tiny radius" style="width:75px;padding:0.625rem 0 0.6875rem;margin:0 auto;" onclick="javascript:showCart(); hideAddedToCartDisplay();">Cart</a>
                </div>
            </div>
        </div>
    </div>

    <%--header spacer--%>
    <div style="background-color:#73593f;">
        <div class="row hide-for-small-down" style="display:block;height:65px;">
            <div class="large-6 medium-6 small-12 columns">
                <div class="small-only-text-center" style="font-family: 'ReklameScript-RegularDEMO';font-weight:bold;font-size:3rem;color:#F6F3EC;line-height:58px !important;"><span style="cursor:pointer;" title="Store" onclick="javascript:showHomePage(); hideItemAddedToCartDisplay();">Coffee Shop</span></div>
            </div>
            <div class="large-6 medium-6 small-12 columns small-only-text-center" style="text-align:right;">
                <div style="margin:18px 0 0;padding:3px 5px;display:block;width:170px;height:28px;font-size:0.875rem;background: #c97625 url('Images/shoppingCart28.png') no-repeat top left;position:absolute;right:0px;">
                    Items: <span id="spanCartItemsDesktop" runat="server">0</span>
                </div>
            </div>
        </div>
        <div class="row show-for-small-down" style="display:block;height:125px;">
            <div class="large-6 medium-6 small-12 columns">
                <div class="small-only-text-center" style="font-family: 'ReklameScript-RegularDEMO';font-weight:bold;font-size:3rem;color:#F6F3EC;"><span style="cursor:pointer;" title="Store" onclick="javascript:showHomePage(); hideItemAddedToCartDisplay();">Coffee Shop</span></div>
            </div>
            <div class="large-6 medium-6 small-12 columns small-only-text-center">
                <div style="margin:5px auto;padding:3px 5px;display:block;width:170px;height:28px;font-size:0.875rem;background: #c97625 url('Images/shoppingCart28.png') no-repeat top left;text-align:right;">
                    Items: <span id="spanCartItemsMobile">0</span>
                </div>
            </div>
        </div>
    </div>

    <%--navigation--%>
    <div class="contain-to-grid sticky" style="background-color:#080300 !important;border-bottom:3px solid #c97625 !important;">
        <nav class="top-bar" data-topbar="" style="background-color:#080300 !important;">
            <ul class="title-area">
                <li class="name">
                    
                </li>
                <!-- Remove the class "menu-icon" to get rid of menu icon. Take out "Menu" to just have icon alone -->
                <!--<li class="toggle-topbar menu-icon"><a href="#"><span>Menu</span></a></li>-->
                <li class="toggle-topbar menu-icon"><a href="#"><img src="Images/menu-icon1.png" alt="" style="position:relative;top:-8px;left:35px;" title="Menu" /></a></li>
            </ul>

            <section class="top-bar-section">
                <!-- Right Nav Section -->
                <ul class="right">
                    <li id="liMenuHome" class="active"><a href="#" onclick="javascript:showHomePage(); hideItemAddedToCartDisplay();">Store</a></li>
                    <li class="hide-for-small-down" style="height:45px;width:1px;line-height:45px;color:#73593F;">&nbsp;</li>
                    <li id="liMenuCart"><a href="#" onclick="javascript:showCart(); hideItemAddedToCartDisplay();">Cart</a></li>
                    <li class="hide-for-small-down" style="height:45px;width:1px;line-height:45px;color:#73593F;">&nbsp;</li>
                    <li id="liMenuContact"><a href="http://www.mobileauthcorp.com/contact.html" target="_blank" title="MobileAuthCorp.com">Contact</a></li>
                </ul>
            </section>
        
        </nav>
    </div>

    <%--<form id="form1" runat="server">--%>
    <form id="formMain" runat="server" method="post" onsubmit="javascript: ShowProcessing();">
        <div class="wrapper">

            <%--main image--%>
            <div id="divHome" runat="server" class="mainImage">
                &nbsp;
            </div>

            <%--products--%>
            <div id="divShopping" runat="server" style="display:block;background-color:#f6f3ec;border-top:3px solid #e3d4c1;">
                <div style="padding:0.5rem;"></div>
                <div class="row">
                    <div class="large-3 medium-6 small-12 columns">
                        <div style="margin:0 auto 1rem;display:block;max-width:250px;width:100%;height:400px;text-align:center;background:#fff;border:1px solid #e3d4c1;">
                            <img id="imgItem1" src="Images/coffee1.jpg" alt="" style="margin-top:1rem;" /><br />
                            <div style="margin:1rem auto 0.375rem;font-size:0.75rem;font-weight:bold;">
                                <span id="spanItem1Title">Newman's Own Coffee</span>
                            </div>
                            <span style="color:#e3d4c1;">&#8226;&nbsp;&#8226;&nbsp;&#8226;&nbsp;&#8226;&nbsp;&#8226;</span>
                            <div style="margin:0.5rem auto 1.25rem;font-size:1rem;font-weight:bold;color:#d19056;">
                                <span id="spanItem1Price">$15.95</span>
                            </div>
                            <a href="#" onclick="javascript: item1AddedToCart(); itemAddedToCartDisplay();"><img src="Images/buttonAddToCart.png" alt="" /></a>
                        </div>
                    </div>
                    <div class="large-3 medium-6 small-12 columns">
                        <div style="margin:0 auto 1rem;display:block;max-width:250px;width:100%;height:400px;text-align:center;background:#fff;border:1px solid #e3d4c1;">
                            <img id="imgItem2" src="Images/coffee2.jpg" alt="" style="margin-top:1rem;" /><br />
                            <div style="margin:1rem auto 0.375rem;font-size:0.75rem;font-weight:bold;">
                                <span id="spanItem2Title">Pete's Coffee</span>
                            </div>
                            <span style="color:#e3d4c1;">&#8226;&nbsp;&#8226;&nbsp;&#8226;&nbsp;&#8226;&nbsp;&#8226;</span>
                            <div style="margin:0.5rem auto 1.25rem;font-size:1rem;font-weight:bold;color:#d19056;">
                                <span id="spanItem2Price">$16.95</span>
                            </div>
                            <a href="#" onclick="javascript: item2AddedToCart(); itemAddedToCartDisplay();"><img src="Images/buttonAddToCart.png" alt="" /></a>
                        </div>
                    </div>
                    <div class="large-3 medium-6 small-12 columns">
                        <div style="margin:0 auto 1rem;display:block;max-width:250px;width:100%;height:400px;text-align:center;background:#fff;border:1px solid #e3d4c1;">
                            <img id="imgItem3" src="Images/coffee3.jpg" alt="" style="margin-top:1rem;" /><br />
                            <div style="margin:1rem auto 0.375rem;font-size:0.75rem;font-weight:bold;">
                                <span id="spanItem3Title">Starbucks Sumatra Coffee</span>
                            </div>
                            <span style="color:#e3d4c1;">&#8226;&nbsp;&#8226;&nbsp;&#8226;&nbsp;&#8226;&nbsp;&#8226;</span>
                            <div style="margin:0.5rem auto 1.25rem;font-size:1rem;font-weight:bold;color:#d19056;">
                                <span id="spanItem3Price">$14.95</span>
                            </div>
                            <a href="#" onclick="javascript: item3AddedToCart(); itemAddedToCartDisplay();"><img src="Images/buttonAddToCart.png" alt="" /></a>
                        </div>
                    </div>
                    <div class="large-3 medium-6 small-12 columns">
                        <div style="margin:0 auto 1rem;display:block;max-width:250px;width:100%;height:400px;text-align:center;background:#fff;border:1px solid #e3d4c1;">
                            <img id="imgItem4" src="Images/coffee4.jpg" alt="" style="margin-top:1rem;" /><br />
                            <div style="margin:1rem auto 0.375rem;font-size:0.75rem;font-weight:bold;">
                                <span id="spanItem4Title">Dunkin Donuts Coffee</span>
                            </div>
                            <span style="color:#e3d4c1;">&#8226;&nbsp;&#8226;&nbsp;&#8226;&nbsp;&#8226;&nbsp;&#8226;</span>
                            <div style="margin:0.5rem auto 1.25rem;font-size:1rem;font-weight:bold;color:#d19056;">
                                <span id="spanItem4Price">$12.95</span>
                            </div>
                            <a href="#" onclick="javascript: item4AddedToCart(); itemAddedToCartDisplay();"><img src="Images/buttonAddToCart.png" alt="" /></a>
                        </div>
                    </div>
                </div>
                <div style="padding:0.5rem;"></div>
            </div>

            <%--cart--%>
            <div id="divCart" runat="server">
                <div class="row">
                    <div class="large-12 columns">
                        <div style="padding:0.5rem;"></div>
                        <table style="width: 100%;border-collapse: collapse;margin-bottom:0;margin-left: .5rem;">
                            <thead id="cartDisplayHeader" style="display:none;"><tr><th>Product</th><th>Description</th><th>Price</th><th>Qty</th><th style="text-align: right;">Subtotal</th></tr></thead>
                            <tbody>
                                <tr id="item1_cartDisplay" style="display:none;" runat="server">
                                    <td style="width:170px;" class="small-text-center"><img id="imgCartItem1" src="Images/coffee1small.jpg" alt="Newman's Own Coffee" style="" /></td>
                                    <td><strong class="show-for-small-down">Description</strong> <span id="item1_cartDiscription" runat="server">Newman's Own Coffee</span><br /><a href="#" style="font-size:0.813rem;" title="Remove item from cart" onclick="javascript:item1DeletedFromCart();">[Remove]</a><div class="show-for-small-down" style="padding:0.25rem;"></div></td>
                                    <td style="width:75px;text-align: center;"><strong class="show-for-small-down">Price </strong><span id="item1_cartPrice" runat="server">$15.95</span><div class="show-for-small-down" style="padding:0.25rem;"></div></td>
                                    <td style="width:45px;text-align: center;"><strong class="show-for-small-down">Qty </strong><span id="item1_cartQty" runat="server">0</span><div class="show-for-small-down" style="padding:0.25rem;"></div></td>
                                    <td style="width:75px;text-align: right;" class="small-text-center"><strong class="show-for-small-down">Subtotal</strong>$<span id="item1_cartSubtotal" runat="server">0.00</span><div class="show-for-small-down" style="padding:0.25rem;"></div></td>
                                </tr>
                                <tr id="item2_cartDisplay" style="display:none;" runat="server">
                                    <td style="width:170px;" class="small-text-center"><img id="imgCartItem2" src="Images/coffee2small.jpg" alt="Pete's Coffee" style="" /></td>
                                    <td><strong class="show-for-small-down">Description</strong> <span id="item2_cartDiscription" runat="server">Pete's Coffee</span><br /><a href="#" style="font-size:0.813rem;" title="Remove item from cart" onclick="javascript:item2DeletedFromCart();">[Remove]</a><div class="show-for-small-down" style="padding:0.25rem;"></div></td>
                                    <td style="width:75px;text-align: center;"><strong class="show-for-small-down">Price </strong>$<span id="item2_cartPrice" runat="server">16.95</span><div class="show-for-small-down" style="padding:0.25rem;"></div></td>
                                    <td style="width:45px;text-align: center;"><strong class="show-for-small-down">Qty </strong><span id="item2_cartQty" runat="server">0</span><div class="show-for-small-down" style="padding:0.25rem;"></div></td>
                                    <td style="width:75px;text-align: right;" class="small-text-center"><strong class="show-for-small-down">Subtotal</strong>$<span id="item2_cartSubtotal" runat="server">0.00</span><div class="show-for-small-down" style="padding:0.25rem;"></div></td>
                                </tr>
                                <tr id="item3_cartDisplay" style="display:none;" runat="server">
                                    <td style="width:170px;" class="small-text-center"><img id="imgCartItem3" src="Images/coffee3small.jpg" alt="Starbucks Sumatra Coffee" style="" /></td>
                                    <td><strong class="show-for-small-down">Description</strong> <span id="item3_cartDiscription" runat="server">Starbucks Sumatra Coffee</span><br /><a href="#" style="font-size:0.813rem;" title="Remove item from cart" onclick="javascript:item3DeletedFromCart();">[Remove]</a><div class="show-for-small-down" style="padding:0.25rem;"></div></td>
                                    <td style="width:75px;text-align: center;"><strong class="show-for-small-down">Price </strong>$<span id="item3_cartPrice" runat="server">14.95</span><div class="show-for-small-down" style="padding:0.25rem;"></div></td>
                                    <td style="width:45px;text-align: center;"><strong class="show-for-small-down">Qty </strong><span id="item3_cartQty" runat="server">0</span><div class="show-for-small-down" style="padding:0.25rem;"></div></td>
                                    <td style="width:75px;text-align: right;" class="small-text-center"><strong class="show-for-small-down">Subtotal</strong>$<span id="item3_cartSubtotal" runat="server">0.00</span><div class="show-for-small-down" style="padding:0.25rem;"></div></td>
                                </tr>
                                <tr id="item4_cartDisplay" style="display:none;" runat="server">
                                    <td style="width:170px;" class="small-text-center"><img id="imgCartItem4" src="Images/coffee4small.jpg" alt="Dunkin Donuts Coffee" style="" /></td>
                                    <td><strong class="show-for-small-down">Description</strong> <span id="item4_cartDiscription" runat="server">Dunkin Donuts Coffee</span><br /><a href="#" style="font-size:0.813rem;" title="Remove item from cart" onclick="javascript:item4DeletedFromCart();">[Remove]</a><div class="show-for-small-down" style="padding:0.25rem;"></div></td>
                                    <td style="width:75px;text-align: center;"><strong class="show-for-small-down">Price </strong>$<span id="item4_cartPrice" runat="server">12.95</span><div class="show-for-small-down" style="padding:0.25rem;"></div></td>
                                    <td style="width:45px;text-align: center;"><strong class="show-for-small-down">Qty </strong><span id="item4_cartQty" runat="server">0</span><div class="show-for-small-down" style="padding:0.25rem;"></div></td>
                                    <td style="width:75px;text-align: right;" class="small-text-center"><strong class="show-for-small-down">Subtotal</strong>$<span id="item4_cartSubtotal" runat="server">0.00</span><div class="show-for-small-down" style="padding:0.25rem;"></div></td>
                                </tr>
                            </tbody>
                            <tfoot style="text-align:right;" id="totalContainer">
                                <tr>
                                    <td colspan="5">
                                        <div id="totalPriceBorder" class="show-for-small-down totalPriceBorder"></div>
                                        <div id="scrollHere" style="text-align: right;" class="small-only-text-center">
                                            <strong>Total:</strong> $<span id="totalPriceDisplay" runat="server">0.00</span>
                                        </div>
                                        <div class="show-for-small-down" style="margin-top:0.5rem;"></div>
                                    </td>
                                </tr>
                            </tfoot>
                        </table>
                        <div style="text-align:right;margin-top:1rem;" class="small-only-text-center">
                            <asp:Button CssClass="button tiny radius" Width="75px" runat="server" ID="btnClear" Text="Clear" OnClick="btnClear_Click" />
                            <a href="#" class="button tiny radius" style="width:75px;margin-right:auto;margin-left:auto;padding-left:0;padding-right:0;" onclick="javascript:showPurchaseForm();">Checkout</a>
                            <%--<asp:Button CssClass="button tiny radius" Width="75px" runat="server" ID="btnPurchase" Text="Checkout" />--%>
                        </div>
                    </div>
                </div>
            </div>

            <%--purchase form--%>
            <div id="divPurchaseForm" runat="server">
                <div style="padding:0.5rem;"></div>
                <div class="row purchaseForm">
                    <div class="large-12 columns">
                        <div style="padding:0.5rem;"></div>
                        <div class="row">
                            <div class="large-12 columns">
                                <h3 style="font-size:1.25rem;">Purchase Form</h3>
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns">
                                <label>First Name *</label>
                                <input type="text" id="txtFirstName" runat="server" />
                            </div>
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Last Name *</label>
                                <input type="text" id="txtLastName" runat="server" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Address 1</label>
                                <input type="text" id="txtAddress1" runat="server" />
                            </div>
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Address 2</label>
                                <input type="text" id="txtAddress2" runat="server" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns">
                                <label>City</label>
                                <input type="text" id="txtCity" runat="server" />
                            </div>
                            <div class="large-6 medium-6 small-12 columns">
                                <label>State</label>
                                <input type="text" id="txtState" runat="server" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Zip Code</label>
                                <input type="text" id="txtZipCode" runat="server" />
                            </div>
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Country</label>
                                <input type="text" id="txtCountry" runat="server" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Mobile Phone *</label>
                                <input type="text" id="txtMobilePhone" runat="server" />
                            </div>
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Email *</label>
                                <input type="text" id="txtEmail" runat="server" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Credit Card *</label>
                                <input type="text" id="txtCreditCard" runat="server" placeholder="Enter credit card number" />
                            </div>
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Exp Date *</label>
                                <input type="text" id="txtExpDate" runat="server" placeholder="MMYY" />
                            </div>
                        </div>
                        <div style="padding:0.25rem;"></div>
                        <div class="row">
                            <div class="large-12 columns small-only-text-center">
                                <a href="#" class="button tiny radius" style="width:75px;padding-left:0;padding-right:0;margin:0 auto;" onclick="javascript:showCart(); hide();">Cancel</a>
                                <asp:Button CssClass="button tiny radius" Width="75px" runat="server" ID="btnPurchase" Text="Purchase" OnClick="btnPurchase_Click"/>
                            </div>
                        </div>
                        <div style="padding:0.25rem;"></div>
                    </div>
                </div>
                <div style="padding:0.875rem;"></div>
            </div>

            <!--client admin div-->
            <div id="divClientAdmin" style="position: relative;margin: 0 auto;z-index: 10;" runat="server">
                <div id="divCfgForm" runat="server">
                    <div class="row" id="scroll2">
                        <div class="large-12 columns">
                            <fieldset style="padding:0.5rem 1rem 1rem;border-radius: 5px;border-color:#e3d4c1;"><legend style="background: none;font-size:0.875rem;color:#444;"><span>Client Administration</span></legend>
                                <div class="row">
                                    <div class="large-6 medium-6 small-12 columns">
                                        <label style="color: #444;">Client Name
                                            <asp:TextBox ID="clientName" runat="server" />
                                        </label>
                                    </div>
                                    <div class="large-6 medium-6 small-12 columns">
                                        <label style="color: #444;">Mac Service URL
                                            <asp:TextBox ID="macServicesUrl" runat="server" />
                                        </label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="large-6 medium-6 small-12 columns">
                                        <label style="color: #444;">Register URL
                                            <asp:TextBox ID="registerUrl" runat="server" />
                                        </label>
                                    </div>
                                    <div class="large-6 medium-6 small-12 columns hidden-for-small">
                                        &nbsp;
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="large-6 medium-6 small-12 columns">
                                        <label style="color: #444;">Email Server
                                            <asp:TextBox ID="txtEmailServer" runat="server" />
                                        </label>
                                    </div>
                                    <div class="large-6 medium-6 small-12 columns">
                                        <label style="color: #444;">Email Port
                                            <asp:TextBox ID="txtEmailPort" runat="server" />
                                        </label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="large-6 medium-6 small-12 columns">
                                        <label style="color: #444;">Email Login User Name
                                            <asp:TextBox ID="txtEmailLoginUserName" runat="server" />
                                        </label>
                                    </div>
                                    <div class="large-6 medium-6 small-12 columns">
                                        <label style="color: #444;">Email Password
                                            <asp:TextBox ID="txtEmailPassword" runat="server" />
                                        </label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="large-6 medium-6 small-12 columns">
                                        <label style="color: #444;">Email From Address
                                            <asp:TextBox ID="txtEmailFromAddress" runat="server" />
                                        </label>
                                    </div>
                                    <div class="large-6 medium-6 small-12 columns">
                                        <label style="color: #444;">Email To Address
                                            <asp:TextBox ID="txtEmailToAddress" runat="server" />
                                        </label>
                                    </div>
                                </div>
                                <div style="padding: 0.25rem;"></div>
                                <div class="row">
                                    <div class="large-12 columns small-only-text-center">
                                        <asp:Button CssClass="button tiny radius" ID="btnSaveCfg" Width="75px" runat="server" Text="Save" OnClick="btnSaveCfg_Click"/>
                                        <asp:Button CssClass="button tiny radius" ID="btnCancel" Width="75px" runat="server" Text="Cancel" OnClick="btnCancelCfg_Click"/>
                                    </div>
                                </div>
                            </fieldset>
                        </div>
                    </div>
                </div>
                <div style="padding: 0.5rem;"></div>
                <input id="panelFocusClients" runat="server" type="hidden" value="" />
            </div>
            <!--end client admin div-->

            <%--processing message--%>
            <div id="divProcessing" runat="server">
                <div style="padding:0.5rem;"></div>
                <div class="row purchaseForm">
                    <div class="large-12 columns">
                        <div class="row">
                            <div class="large-12 columns" style="text-align:center;padding:1rem 1rem 2.5rem;">
                                <h3 style="margin:0.75rem 0 0;font-size:1.25rem;">Processing...</h3>
                                <span style="font-size:0.875rem;">Your order has been submitted and is waiting for authorization.</span>
                            </div>
                        </div>
                    </div>
                </div>
                <div style="padding:0.875rem;"></div>
            </div>

            <%--thank you display--%>
            <div id="divThankYou" runat="server">
                <div style="padding:0.5rem;"></div>
                <div class="row purchaseForm">
                    <div class="large-12 columns">
                        <div class="row">
                            <div class="large-12 columns" style="text-align:center;padding:1rem 1rem 2.5rem;">
                                <img src="Images/check.png" alt="" /><br />
                                <h3 style="margin:0.75rem 0 0;font-size:1.25rem;">Payment Successful!</h3>
                                <span style="font-size:0.875rem;">Your order has been processed. Here are the transaction details:</span>
                                <div id="transactionSummary" runat="server" style="max-width:425px;margin:1rem auto 1.25rem;padding:1rem 1rem 1.25rem;font-size:0.813rem;text-align:left;line-height:1.5rem;background:#F6F3EC;border:1px solid #e3d4c1;">
                                    
                                </div>
                                <asp:Button CssClass="button tiny radius" Width="75px" runat="server" ID="btnHome" Text="Home" OnClick="btnClear_Click" />
                            </div>
                        </div>
                    </div>
                </div>
                <div style="padding:0.875rem;"></div>
            </div>

            <%--timeout display--%>
            <div id="divTimeout" runat="server">
                <div style="padding:0.5rem;"></div>
                <div class="row purchaseForm">
                    <div class="large-12 columns">
                        <div class="row">
                            <div class="large-12 columns" style="text-align:center;padding:1rem 1rem;">
                                <h3 style="margin:0.75rem 0 0;font-size:1.25rem;">Payment Request Timeout!</h3>
                                <div style="margin-bottom:1rem;font-size:0.875rem;">The system has timed out and your order was not processed.</div>
                                <%--<asp:Button CssClass="button tiny radius" Width="75px" runat="server" ID="Button1" Text="Home" OnClick="btnClear_Click" />--%>
                                <input type="button" class="button tiny radius" style="width:75px;" value="Home" onclick="javascript: showHomePage();" />
                            </div>
                        </div>
                    </div>
                </div>
                <div style="padding:0.875rem;"></div>
            </div>

            <%--too many retries display--%>
            <div id="divTooManyRetries" runat="server">
                <div style="padding:0.5rem;"></div>
                <div class="row purchaseForm">
                    <div class="large-12 columns">
                        <div class="row">
                            <div class="large-12 columns" style="text-align:center;padding:1rem 1rem;">
                                <h3 style="margin:0.75rem 0 0;font-size:1.25rem;">Retry Limit Exceeded!</h3>
                                <div style="margin-bottom:1rem;font-size:0.875rem;">An invalid authentication code was submitted 3 or more times.<br />This purchase has been terminated.</div>
                                <%--<asp:Button CssClass="button tiny radius" Width="75px" runat="server" ID="Button1" Text="Home" OnClick="btnClear_Click" />--%>
                                <input type="button" class="button tiny radius" style="width:75px;" value="Home" onclick="javascript: showHomePage();" />
                            </div>
                        </div>
                    </div>
                </div>
                <div style="padding:0.875rem;"></div>
            </div>

            <!-- Disclaimer/Copyright -->
            <div id="divDisclaimer" style="display: block;">
                <div style="text-align:center;color:#e3d4c1;">&#8226;&nbsp;&#8226;&nbsp;&#8226;&nbsp;&#8226;&nbsp;&#8226;</div>
                <div class="row">
                    <div class="large-12 columns" style="text-align: left;">
                        <fieldset style="padding:0.5rem 1rem 1rem;border-radius: 5px;border-color:#e3d4c1;"><legend style="background: none;font-size:0.875rem;color:#444;">MAC Disclosure Statement</legend>
                            <div class="disclosure">
                                <p>
                                    <strong>This demonstration website ("Demo") is the copyrighted work of Mobile Authentication Corporation.</strong><br />
                                    The information contained in this website is for general information purposes only. While we endeavor to keep the information up to date and correct, we make no representations or warranties of any kind, express or implied, about the completeness, accuracy, reliability, suitability or availability with respect to the website or the information, products, services, or related graphics contained on this website for any purpose. Any reliance you place on such information is therefore strictly at your own risk.
                                </p>
                                <p>
                                    In no event will we be liable for any loss or damage including without limitation, indirect or consequential loss or damage, or any loss or damage whatsoever arising from loss of data or profits arising out of, or in connection with, the use of this website.
                                </p>
                                <p>
                                    Links to third party websites are not under the control of Mobile Authentication Corporation. The inclusion of any links does not necessarily imply a recommendation or endorse the views expressed within them.
                                </p>
                                <p style="margin-bottom: 0;">
                                    Every effort is made to keep the website up and running smoothly. However, Mobile Authentication Corporation will not be liable for the website being temporarily unavailable due to technical issues beyond our control.
                                </p>
                            </div> 
                        </fieldset>
                    </div>
                </div>
            </div>
            <!-- Disclaimer/Copyright -->

            <%--do not remove! used to keep footer stuck to bottom  --%>
            <div class="push"></div>
        </div>
        <div id="divFooter" class="footer">
            <div style="padding:0.5rem;"></div>
            <div class="row">
                <div class="large-6 medium-6 small-12 columns">
                    <div class="hide-for-small-down" style="font-size:0.75rem;color:#e3d4c1;">
                        <script type="text/javascript">
                            <!--
                            var currentDate = new Date();
                            var year = currentDate.getFullYear();
                            document.write("&copy; " + year + " Mobile Authentication Corporation. All rights reserved.");
                            //-->
                        </script>
                    </div>
                    <div class="show-for-small-down" style="font-size:0.75rem;text-align:center;color:#e3d4c1;">
                        <script type="text/javascript">
                            <!--
                            var currentDate = new Date();
                            var year = currentDate.getFullYear();
                            document.write("&copy; " + year + " Mobile Authentication Corporation.<br />All rights reserved.");
                            //-->
                        </script>
                    </div>
                </div>
                <div class="large-6 medium-6 small-12 columns">
                    <div class="hide-for-small-down" style="text-align:right;">
                        <%--<a href="docs/MAC-Terms-and-Conditions.pdf" target="_blank" style="font-size:0.75rem;color:#C97625 !important;">Disclaimer</a>--%>
                        <a href="docs/MAC-Terms-and-Conditions.pdf" target="_blank" class="disclaimer">Disclaimer</a>
                    </div>
                    <div class="show-for-small-down" style="text-align:center;padding:0.25rem 0 0;">
                        <a href="docs/MAC-Terms-and-Conditions.pdf" target="_blank" class="disclaimer">Disclaimer</a>
                    </div>
                </div>
            </div>
        </div>

        <%--hidden fields--%>
        <input id="hiddenHost" runat="server" type="hidden" value="" />
        <input id="hiddenCountOfItemsInCart" runat="server" type="hidden" value="" />
        <input id="hiddenTrxDetails" runat="server" type="hidden" value="" />
        <input id="hiddenCID" runat="server" type="hidden" value="" />
        <input id="hiddenGID" runat="server" type="hidden" value="" />
        <input id="hiddenClientName" runat="server" type="hidden" value="" />
        <%--<input id="hiddenMacRequestOTPServiceUrl" runat="server" type="hidden" value="" />--%>
        <%--<input id="hiddenMacbankUrl" runat="server" type="hidden" value="" />--%>
        <input id="hiddenRegisterUrl" runat="server" type="hidden" value="" />
        <input id="hiddenRequestId" runat="server" type="hidden" value="" />

        <input id="hiddenO" runat="server" type="hidden" value="" />
        <input id="hiddenOTP" runat="server" type="hidden" value="" />
        <input id="hiddenOTPType" runat="server" type="hidden" value="" />
        <input id="hiddenMacServicesUrl" runat="server" type="hidden" value="" />
        <input id="hiddenT" runat="server" type="hidden" value="" />

        <!-- Send Email Parameters -->
        <input id="hiddenEmailServer" runat="server" type="hidden" value="false" />
        <input id="hiddenEmailPort" runat="server" type="hidden" value="false" />
        <input id="hiddenEmailToAddress" runat="server" type="hidden" value="false" />
        <input id="hiddenEmailFromAddress" runat="server" type="hidden" value="false" />
        <input id="hiddenEmailLoginUserName" runat="server" type="hidden" value="false" />
        <input id="hiddenEmailPassword" runat="server" type="hidden" value="false" />

        <%--shopping cart values--%>
        <input id="hiddenDescription1" runat="server" type="hidden" value="" />
        <input id="hiddenDescription2" runat="server" type="hidden" value="" />
        <input id="hiddenDescription3" runat="server" type="hidden" value="" />
        <input id="hiddenDescription4" runat="server" type="hidden" value="" />
        <input id="hiddenQty1" runat="server" type="hidden" value="" />
        <input id="hiddenQty2" runat="server" type="hidden" value="" />
        <input id="hiddenQty3" runat="server" type="hidden" value="" />
        <input id="hiddenQty4" runat="server" type="hidden" value="" />
        <input id="hiddenSubtotal1" runat="server" type="hidden" value="" />
        <input id="hiddenSubtotal2" runat="server" type="hidden" value="" />
        <input id="hiddenSubtotal3" runat="server" type="hidden" value="" />
        <input id="hiddenSubtotal4" runat="server" type="hidden" value="" />
        <input id="hiddenTotalQty" runat="server" type="hidden" value="" />
        <input id="hiddenTotalPrice" runat="server" type="hidden" value="" />
        <input id="hiddenTimeout" runat="server" type="hidden" value="false" />
        <input id="hiddenTooManyRetries" runat="server" type="hidden" value="false" />
        <input id="hiddenThankYou" runat="server" type="hidden" value="false" />
    </form>    
    <script src="js/cart.js"></script>
    <script src="foundation/js/vendor/jquery.js"></script>
    <script src="foundation/js/vendor/modernizr.js"></script>
    <script src="foundation/js/foundation.min.js"></script>
    <script>
        $(document).foundation();

        $("#btnPurchase").attr("disabled", true);

        $(window).resize(function () {
            resize();
        });
    </script>
</body>
</html>
