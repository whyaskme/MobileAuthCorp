<?php
  if(isset($_SESSION['mac-bank']['cart'])) 
      $cart_count = count($_SESSION['mac-bank']['cart']);
  else
      $cart_count = 0;     
  $path = pathinfo(__FILE__);    
  $_SESSION['mac-bank']['page-name'] = $path['basename'];
 
  include_once 'includes/functions.php';
  include_once 'includes/db_class.php';
  include_once 'includes/product_class.php';
  include_once 'includes/user_class.php';
  include_once 'includes/macRequest_class.php';
  
  // get personal data
  if(!empty($formVars)) {
     foreach($formVars as $key=>$value){
       $_SESSION['mac-bank']['billing-info'][$key] = trim(stripslashes($value));      
     }
  }                          
   
  // get product data
  $pd = new ProductClass();
  $products = '';
 
  // rebuild product order from cart contents - cannot update on this screen
  if(isset($_SESSION['mac-bank']['cart'])) {
      foreach($_SESSION['mac-bank']['cart'] as $index => $productInfo) {
          $condition['product_number'][] = $productInfo['product_number'];
      }
      $products = $pd->getProducts($condition);
      
      // initialize totals array
      $product_totals = array('order_product_total' => 0, 'order_shipping_am' => 0, 'order_total' => 0);
      
      foreach($products as $pindex => $product) {
          $prodNum = $product['product_number'];
          $qty = 0;
          foreach($_SESSION['mac-bank']['cart'] as $index => $cartProduct) {
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
  
  // verify billing input
  if(isset($formVars['action']) && $formVars['action'] == 'verify') {   // is a submit from checkout page
    
      // start with clean 'billing-info'
      $_SESSION['mac-bank']['billing-info'] = array();
      if(!empty($formVars)) {
         foreach($formVars as $key=>$value){
           $_SESSION['mac-bank']['billing-info'][$key] = trim(stripslashes($value));      
         }
      }                          
      
      // validate billing information from checkout page entered by client managed user
      if(!isset($_SESSION['mac-bank']['loggedIn']) || (isset($_SESSION['mac-bank']['loggedIn']) && $_SESSION['mac-bank']['loggedIn'] != true)) {
          $validFields = array('fname' => 'text', 'lname' => 'text',  'address' => 'text', 'city' => 'text', 'state' => 'state', 'zip' => 'zip', 'email' => 'email', 'cell_phone' => 'phone', 'primary_account' => 'number', 'preferred_payment_account' => 'creditcard');
          $requiredFields = array('fname', 'lname', 'email', 'cell_phone', 'preferred_payment_account');
          $user = new ClientUser();
          $returnArray = $user->validateUserFields($formVars,$validFields, $requiredFields);
          $errorArray = $returnArray['errorArray'];  // error messages or empty array     
      }
      
      // get account information for logged in registered user
      else if(isset($_SESSION['mac-bank']['loggedIn']) && $_SESSION['mac-bank']['loggedIn'] == true) {
          $mac = new MacRequest('registered', $formVars);
          $status = $mac->getAccountDetailsRegistered();
          if($status != true)
             $errorArray[] = 'error retrieving account details';
      }
 
      if(!empty($errorArray)) {    // go back to checkout with error mesages
          ob_clean();
          include 'checkout.php';
          exit();
      }
  }

  // sending auth request
  else if(isset($formVars['action']) && $formVars['action'] == 'auth') {  // authorizing payment
      if(!isset($_SESSION['mac-bank']['loggedIn']) || (isset($_SESSION['mac-bank']['loggedIn']) && $_SESSION['mac-bank']['loggedIn'] != true)) {
          $mac = new MacRequest('clientManaged', $formVars);
      }
          
      else if(isset($_SESSION['mac-bank']['loggedIn']) && $_SESSION['mac-bank']['loggedIn'] == true) {
          $mac = new MacRequest('registered', $formVars);
          // grab the selected payment type, - in csse of redisplay with errors
          if(isset($formVars['preferred_payment_type']) && $formVars['preferred_payment_type'] != '')
             $selected_preferred_payment_type = $formVars['preferred_payment_type'];
          else
             $selected_preferred_payment_type = '';
      }

      // preauth if no errors
      if(empty($errorArray)) {
          $authenticated = $mac->preAuth();
          if($authenticated != true)
             $errorArray[] = 'error authorizing account';
      }
         
      // request otp if no errors
      if(empty($errorArray)) {
          $otp = $mac->requestOTP('auth');
          if($otp != true)
             $errorArray[] = 'error requesting OTP code';   
      }
      
      // display OTP screen for next step - purchase
      if(empty($errorArray)) {
          // check status in $_SESSION['mac-bank']['billing-info'] which is set by MacRequest calls
          // error messages  - red
          if(isset($_SESSION['mac-bank']['billing-info']['errors']) && !empty($_SESSION['mac-bank']['billing-info']['errors'])) {
              $errorArray[] = $_SESSION['mac-bank']['billing-info']['errors'][0];  // TODO - merge arrays
          }
          // success message -  green
          $messageArray = array();
          if(empty($errorArray)) {
              $messageArray = $_SESSION['mac-bank']['billing-info']['messages'];
          }
          // info messages - orange
          $infoArray = array();
          $infoArray[] = $_SESSION['mac-bank']['billing-info']['info-message'];
      
          $_SESSION['mac-bank']['otpFormType'] = $otpFormType = 'otp_purchase';
          include 'otpform.php';
          exit();
      }
  }
  
  // set up data for listing registered user accounts in select list   
  if(isset($_SESSION['mac-bank']['billing-info']['accounts'])) { 
      $myAccounts = $_SESSION['mac-bank']['billing-info']['accounts']['Account']['SubAccounts']['SubAccount'];
  }
  else {
      $myAccounts = array();
  }

  
      // check status in $_SESSION['mac-bank']['billing-info'] which is set by MacRequest calls
      // error messages  - red
      if(isset($_SESSION['mac-bank']['billing-info']['errors']) && !empty($_SESSION['mac-bank']['billing-info']['errors'])) {
          $errorArray[] = $_SESSION['mac-bank']['billing-info']['errors'][0];  // TODO - merge arrays
      }
      // success message -  green
      $messageArray = array();
      if(empty($errorArray)) {
          $messageArray = $_SESSION['mac-bank']['billing-info']['messages'];
      }
      // info messages - orange
      $infoArray = array();
      $infoArray[] = $_SESSION['mac-bank']['billing-info']['info-message'];
      
  
 
  // i_render_verify.html.php has two sections - one for 'client managed', one for 'registered' user
  include_once 'header_basic.html.php';     // uses $cart_count
  include 'i_render_verify.html.php';       // uses $authenticated
  include 'footer_mac.html,php';
?> 
<script>
  $(document).ready(function() {
          
     // xxxx
     $('input[name="modify_billing"]').click(function(ev) {
           window.location = 'index.php?action=checkout';
     });  
          
     $('input[name="authenticate_order"]').click(function(ev) {
           macCart.prepareAuthenticate(ev);
           return false;
     });  
          
  /*   $('form').on('submit',  function(ev) {
         var status = macCart.prepareAuthenticate(ev);
         return false;
     });       */
  });


</script>

