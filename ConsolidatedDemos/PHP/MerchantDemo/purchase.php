<?php
  if(isset($_SESSION['mac-cart']['cart'])) 
      $cart_count = count($_SESSION['mac-cart']['cart']);
  else
      $cart_count = 0;     
  $path = pathinfo(__FILE__);    
  $_SESSION['mac-cart']['page-name'] = $path['basename'];
 
  include_once 'includes/functions.php';
  include_once 'includes/product_class.php';
  include_once 'includes/user_class.php';
  include_once 'includes/macRequest_class.php';
  
  $errorArray = array();

  // get product data
  $pd = new ProductClass();
  $products = '';
 
  // rebuild product order from cart contents - cannot update on this screen
  if(isset($_SESSION['mac-cart']['cart'])) {
      foreach($_SESSION['mac-cart']['cart'] as $index => $productInfo) {
          $condition['product_number'][] = $productInfo['product_number'];
      }
      $products = $pd->getProducts($condition);
      $order_product_total = 0;
      
      // initialize totals array
      $product_totals = array('order_product_total' => 0, 'order_shipping_am' => 0, 'order_total' => 0);
      
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
  else {    // no cart data
      $products = null;
      $product_totals = null;
      $errorArray[] = 'cart is empty';
  }

  if(isset($formVars['action']) && $formVars['action'] == 'buy') {  // make a purchase
     
      $formVars = $_SESSION['mac-cart']['formVars'];  // restore formVars with purchase data
      $formVars['ClientId'] = $_SESSION['mac-cart']['config']['clientId'];     // load saved clientid
      $formVars['ClientName'] = $_SESSION['mac-cart']['config']['clientName']; // load saved clientid
     
      if(isset($_SESSION['mac-cart']['loggedIn']) && $_SESSION['mac-cart']['loggedIn'] == true) {
          // grab the selected payment type,
          if(isset($formVars['preferred_payment_type']) && $formVars['preferred_payment_type'] != '')
             $selected_preferred_payment_type = $formVars['preferred_payment_type'];
          else
             $selected_preferred_payment_type = '';

          $mac = new MacRequest('registered', $formVars);
          $purchased = $mac->purchase();
          if($purchased != true)
             $errorArray[] = 'error making purchase';
      }
      else {
          $mac = new MacRequest('clientmanaged', $formVars);
          $purchased = $mac->purchase();
          if($purchased != true)
             $errorArray[] = 'error making purchase';
          
      }
  }
     
  
  // error messages
  if(isset($_SESSION['mac-cart']['billing-info']['errors']) && !empty($_SESSION['mac-cart']['billing-info']['errors'])) {
      $errorArray[] = $_SESSION['mac-cart']['billing-info']['errors'][0];  // TODO - merge arrays
  }
  
  if(!empty($errorArray)) {
     //   ob_clean();
      $formVars['action'] = 'redisplay';
      include 'verify.php';
      exit();
  }
  
  // save order to array
  $billingArray = array();
  $billingArray = $_SESSION['mac-cart']['billing-info'];
  
  if(isset($billingArray['preferred_payment_account']))    // adjust account
     $billingArray['account'] = $billingArray['preferred_payment_account'];
  else if(isset($billingArray['preferred_payment_type']))
     $billingArray['account'] = $billingArray['preferred_payment_type']; 
  $billingArray['order_date'] = time();   
       
  // success message s
  $messageArray = array();
  $messageArray = $_SESSION['mac-cart']['billing-info']['messages'];
  
  // save for re-print
  $_SESSION['mac-cart']['save-order']['products'] = $products;
  $_SESSION['mac-cart']['save-order']['product_totals'] = $product_totals;
  $_SESSION['mac-cart']['save-order']['billingArray'] = $billingArray;
  $_SESSION['mac-cart']['save-order']['errorArray'] = $errorArray;
  $_SESSION['mac-cart']['save-order']['messageArray'] = $messageArray;
  
  // i_render_verify.html.php has two sections - one for 'client managed', one for 'registered' user
  ob_clean();
  include_once 'header_basic.html.php';     // uses $cart_count
  include 'i_render_order.html.php';       // uses $products, product_totals, billingArray, errorArray, messageArray
  include 'footer_basic.html';
  
  unset($_SESSION['mac-cart']['cart']);    // clear cart after purchase
  unset($_SESSION['mac-cart']['formVars']); // clear saved formVars
?> 
<script>
  $(document).ready(function() {
          
     // xxxx
     $('#printer-friendly').click(function(ev) {
           window.open('index.php?action=printer_friendly', '_blank');
     });  
          
  });


</script>

