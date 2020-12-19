<?php
  if(isset($_SESSION['mac-cart']['cart'])) 
      $cart_count = count($_SESSION['mac-cart']['cart']);
  else
      $cart_count = 0;     
  $path = pathinfo(__FILE__);    
  $_SESSION['mac-cart']['page-name'] = $path['basename'];
 
  include_once 'includes/functions.php';
  include_once 'includes/product_class.php';
  include_once 'includes/macRequest_class.php';
  
  // initialize 
  if(isset($formVars['action']) && $formVars['action'] == 'checkout')
      $errorArray = array();
      
  // get product data
  $pd = new ProductClass();
  $products = '';
 
  // display order data from cart - cannot update on this screen
  if(isset($_SESSION['mac-cart']['cart']) && count($_SESSION['mac-cart']['cart']) > 0) {
      foreach($_SESSION['mac-cart']['cart'] as $index => $productInfo) {
          $condition['product_number'][] = $productInfo['product_number'];
      }
      $products = $pd->getProducts($condition);
      $order_product_total = 0;
      
      // initialize totals array
      $product_totals = array('order_product_total' => 0, 'order_shipping_am' => 0, 'order_total' => 0);
      $order_product_total = 0;   // initialize
      
      foreach($products as $pindex => $product) {
          $prodNum = $product['product_number'];
          $qty = 0;
          foreach($_SESSION['mac-cart']['cart'] as $index => $cartProduct) {
              if($cartProduct['product_number'] == $prodNum)
                 $qty = $cartProduct['qty'];
          }
          $products[$pindex]['qty'] = $qty;
          $total = ($products[$pindex]['price']*100) * $qty;
          $products[$pindex]['total_price'] = number_format(($total/100), 2, '.', ',');
          
          // accumulate order total
          $total_price = $total/100;
          $order_product_total += $total_price;
      }
      // set order totals
      $order_shipping_amt = round($order_product_total * .1, 2);
      $order_total = $order_product_total + $order_shipping_amt;
     
      // format numbers for display 
      $product_totals['order_product_total'] = number_format($order_product_total, 2, '.', ',');
      $product_totals['order_shipping_amt'] = number_format($order_shipping_amt, 2, '.', ',');
      $product_totals['order_total'] = number_format($order_total, 2, '.', ',');
                                            
   }  
   else {
       $errorArray[] = 'empty cart - cannot checkout';
       ob_clean();
       include 'cart.php';
       exit();
   }
   
   // checkout submit
   if(isset($formVars['action']) && $formVars['action'] == 'checkout') {   // is a submit from checkout
      // validate billing information
      if(isset($_SESSION['mac-cart']['loggedIn']) && $_SESSION['mac-cart']['loggedIn'] == true) {
          $formVars['ClientId'] = $_SESSION['mac-cart']['config']['clientId'];     // load saved clientid
          $formVars['ClientName'] = $_SESSION['mac-cart']['config']['clientName']; // load saved clientid
          $formVars['user_id'] = $_SESSION['mac-cart']['user_id'];
          $formVars['fname'] = $_SESSION['mac-cart']['fname'];
          $formVars['lname'] = $_SESSION['mac-cart']['lname'];
          
          // get account details
          $mac = new MacRequest('registered', $formVars);
          $status = $mac->getAccountDetailsRegistered();
          if($status != true)
             $errorArray[] = 'Error retrieving account details';
             
          if(!empty($_SESSION['mac-cart']['billing-info']['errors'])) 
             $errorArray = $_SESSION['mac-cart']['billing-info']['errors'];   
          
          // no error - display page   
          if(empty($errorArray)) {
             // set up data for listing registered user accounts in select list   
              if(isset($_SESSION['mac-cart']['billing-info']['accounts'])) { 
                  $myAccounts = $_SESSION['mac-cart']['billing-info']['accounts']['Account']['SubAccounts']['SubAccount'];
              }
              else {
                  $myAccounts = array();
              }
              
              // i_render_verify.html.php has two sections - one for 'client managed', one for 'registered' user
              include_once 'header_basic.html.php';     // uses $cart_count
              include 'i_render_verify.html.php';       // uses $authenticated
              include 'footer_basic.html';
              exit();
          }
  

      }
      else {
        if(!isset($formVars['fname']) && isset($_SESSION['mac-cart']['billing-info']) && !empty($_SESSION['mac-cart']['billing-info'])) {
           $formVars = array();
           foreach($_SESSION['mac-cart']['billing-info'] as $field => $value) { 
               $formVars[$field] = $value;   
           } 
           unset($_SESSION['mac-cart']['billing-info']);     
        }
     }
   }  
   
   // not a submit - redisplay billing info if redisplaying from verify screen  - for errors
   else if(!isset($formVars['fname']) && isset($_SESSION['mac-cart']['billing-info']) && !empty($_SESSION['mac-cart']['billing-info'])) {
       $formVars = array();
       foreach($_SESSION['mac-cart']['billing-info'] as $field => $value) { 
           $formVars[$field] = $value;   
       } 
       unset($_SESSION['mac-cart']['billing-info']);
   } 
   
   // display Guest message if not logged in
  if(!isset($_SESSION['mac-cart']['loggedIn']) || (isset($_SESSION['mac-cart']['loggedIn']) && $_SESSION['mac-cart']['loggedIn'] != true)) {
      $infoArray = array();
      $infoArray[] = '<span class="glyphicon glyphicon-arrow-right"></span> proceed as a "Guest", or login to "My Account"';
  }
         
  // set userData for form
  $userData = $formVars;
  
  include_once'header_basic.html.php';     // uses $cart_count
  include 'i_render_checkout.html.php';
  include 'footer_basic.html';
?> 
<script>
  $(document).ready(function() {
      
      // format mask for credit card entry
     new Formatter(document.getElementById('preferred_payment_account'), {
        'pattern': '{{9999}}-{{9999}}-{{9999}}-{{9999}}',
        'persistent' : true
     });
      
      
     // back to cart link
     $('input[name="modify_order"]').click(function(ev) {
           window.location = 'index.php?action=cart';
     }); 
     
     $('#guest-user-link').click(function(ev) {
         $('#billing-form-container-bank-user').hide();
         $('#billing-form-container-cart-user').show();
         macCart.setUserType('guest');
     });
     $('#bank-user-link').click(function(ev) {
         $('#billing-form-container-cart-user').hide();
         $('#billing-form-container-bank-user').show();
         macCart.setUserType('bank');
     });
     $('#cart-user-link').click(function(ev) {
         $('#billing-form-container-bank-user').hide();
         $('#billing-form-container-cart-user').show();
         macCart.setUserType('cart');
     });
     
     
           
  });


</script>
