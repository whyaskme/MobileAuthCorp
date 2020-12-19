<?php
  if(isset($_SESSION['mac-cart']['cart'])) 
      $cart_count = count($_SESSION['mac-cart']['cart']);
  else
      $cart_count = 0;     
 
  include_once 'includes/functions.php';
  include_once 'includes/macRequest_class.php';
  
 // $errorArray = array();
   
  // sending validate otp request
  if(isset($formVars['action']) && $formVars['action'] == 'validate_otp') {  // authorizing payment
      if(!isset($_SESSION['mac-cart']['loggedIn']) || (isset($_SESSION['mac-cart']['loggedIn']) && $_SESSION['mac-cart']['loggedIn'] != true)){             

          $holdOtp = $formVars['otp'];
          $formVars = $_SESSION['mac-cart']['formVars'];  // restore formVars with purchase data
          $formVars['ClientId'] = $_SESSION['mac-cart']['config']['clientId'];   // load saved clientid
          $formVars['otpCode'] = $holdOtp;

          // validate otp if no errors
          $mac = new MacRequest('clientManaged', $formVars);
          if(empty($errorArray)) {
              $otp = $mac->validateOTP($_SESSION['mac-cart']['otp']['RequestId']);
              if($otp != true)  {         // set messages for redisplay of opt screen
                 $errorArray[] = 'error validating OTP code';
                 $_SESSION['mac-cart']['billing-info']['info-message'] = "<strong>Order Total:</strong> $".$_SESSION['mac-cart']['billing-info']['order_total'];

              }  
          }
          
      }
      else if(isset($_SESSION['mac-cart']['loggedIn']) && $_SESSION['mac-cart']['loggedIn'] == true){             

          $holdOtp = $formVars['otp'];
          $formVars = $_SESSION['mac-cart']['formVars'];  // restore formVars with purchase data
          $formVars['ClientId'] = $_SESSION['mac-cart']['config']['clientId'];   // load saved clientid
          $formVars['otpCode'] = $holdOtp;

          // validate otp if no errors
          $mac = new MacRequest('registered', $formVars);
          if(empty($errorArray)) {
              $otp = $mac->validateOTP($_SESSION['mac-cart']['otp']['RequestId']);
              if($otp != true)
                 $errorArray[] = 'error validating OTP code';   
          }
          
      }
      // error messages  - red
      if(isset($_SESSION['mac-cart']['billing-info']['errors']) && !empty($_SESSION['mac-cart']['billing-info']['errors'])) {
          $errorArray[] = $_SESSION['mac-cart']['billing-info']['errors'][0];  // TODO - merge arrays
      }
      // success message -  green
      $messageArray = array();
      if(empty($errorArray)) {
          if(isset($_SESSION['mac-cart']['billing-info']['messages']) && !empty($_SESSION['mac-cart']['billing-info']['messages'])) 
              $messageArray = $_SESSION['mac-cart']['billing-info']['messages'];
      }
      // info messages - orange
      $infoArray = array();
      if(isset($_SESSION['mac-cart']['billing-info']['info-message']) && !empty($_SESSION['mac-cart']['billing-info']['info-message'])) 
          $infoArray[] = $_SESSION['mac-cart']['billing-info']['info-message'];
  
                           
      // display OTP screen
      if(!empty($errorArray)) {    // for test, bypass errors - >> change back to if(!empty... when opt validate works
          $otpFormType = $_SESSION['mac-cart']['otpFormType'];  // use the form type already set up
          include 'otpform.php'; // errors redisplay otp screen
      }
      else {
          $formVars['action'] = 'buy';
          include 'purchase.php'; // ok - proceed to purchase confirmation
      }
      
  }
  
  // sending validate otp from login
  else if(isset($formVars['action']) && $formVars['action'] == 'validate_otp_login') {  
      if(!isset($_SESSION['mac-cart']['loggedIn']) || (isset($_SESSION['mac-cart']['loggedIn']) && $_SESSION['mac-cart']['loggedIn'] != true)){             

          $holdOtp = $formVars['otp'];
          $formVars = $_SESSION['mac-cart']['formVars'];  // restore formVars with purchase data
          $formVars['ClientId'] = $_SESSION['mac-cart']['config']['clientId'];     // load saved clientid
          $formVars['ClientName'] = $_SESSION['mac-cart']['clientname']; // load saved clientid
          $formVars['otpCode'] = $holdOtp;

          // request otp if no errors
          $mac = new MacRequest('registered', $formVars);
          if(empty($errorArray)) {
              $otp = $mac->validateOTP($_SESSION['mac-cart']['otp']['RequestId']);
              if($otp != true)
                 $errorArray[] = 'error validating OTP code';
              else { // setup successful login
                 // set up user variables for this login session
                 $_SESSION['mac-cart']['loggedIn'] = true;
                 $_SESSION['mac-cart']['user_id'] = $formVars['user_id'];
                 $_SESSION['mac-cart']['fname'] = $_SESSION['mac-cart']['billing-info']['fname'];
                 $_SESSION['mac-cart']['lname'] = $_SESSION['mac-cart']['billing-info']['lname'];

                 // get account details
                 $mac = new MacRequest('registered', $formVars);
                 $status = $mac->getAccountDetailsRegistered();
                 if($status != true)
                    $errorArray[] = 'error retrieving account details';
                     
                 if(!empty($_SESSION['mac-cart']['billing-info']['errors'])) 
                    $errorArray = $_SESSION['mac-cart']['billing-info']['errors'];   
          
                 // go to page user was on when logging in
                 if($_SESSION['mac-cart']['page-name'] != 'checkout.php') {
                     $gotoPage = $_SESSION['mac-cart']['page-name']; 
                     include "$gotoPage";
                     exit(); 
                 }
                 else { // unless it was checkout page - then go to Verify page
                     // set up data for listing registered user accounts in select list   
                     if(isset($_SESSION['mac-cart']['billing-info']['accounts'])) { 
                         $myAccounts = $_SESSION['mac-cart']['billing-info']['accounts']['Account']['SubAccounts']['SubAccount'];
                     }
                     else {
                         $myAccounts = array();
                     }
                      
                     // display verify page 
                     $formVars['action'] = 'verify';
                     include 'verify.php'; 
                     exit();
                 }
              }      
          }
          
      }
      else {
          $errorArray[] = 'already logged in';
      }
      // error messages  - red
      if(isset($returnArray['errors']) && !empty($returnArray['errors'])) {
          $errorArray[] = $returnArray['errors'][0];  // TODO - merge arrays
      }
      // success message -  green
      $messageArray = array();
      if(empty($errorArray)) {
          $messageArray = $returnArray['messages'];
      }
      // info messages - orange
      $infoArray = array();
      if(!empty($returnArray['info-message'])) {
         $infoArray[] = $returnArray['info-message'];
      }
              
              
      // display OTP screen
      if(!empty($errorArray)) {    // for test, bypass errors - >> change back to if(!empty... when opt validate works
          $otpFormType = $_SESSION['mac-cart']['otpFormType'];  // use the form type already set up
          include 'otpform.php'; // errors redisplay otp screen
      }
    
  }   
    
  unset($_SESSION['mac-cart']['formVarrs']);
          
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

