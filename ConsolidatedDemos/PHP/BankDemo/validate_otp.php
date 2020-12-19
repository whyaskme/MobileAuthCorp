<?php
  if(isset($_SESSION['mac-bank']['cart'])) 
      $cart_count = count($_SESSION['mac-bank']['cart']);
  else
      $cart_count = 0;     
 
  include_once 'includes/functions.php';
  include_once 'includes/macRequest_class.php';
  
  $errorArray = array();
  if($formVars['otpCode'] == '' || !is_number($formVars['otpCode']))
     //$errorArray[] = 'invalid code, must be a number';
      $errorArray[] = 'Invalid code!';
 
  if(empty($errorArray)) {  
      // sending validate otp request
      if(isset($formVars['action']) && $formVars['action'] == 'validate_otp_transfer') {  // authorizing payment
          if(!isset($_SESSION['mac-bank']['loggedIn']) || (isset($_SESSION['mac-bank']['loggedIn']) && $_SESSION['mac-bank']['loggedIn'] != true)){             
              //$errorArray[] = 'not logged in';
              $errorArray[] = 'User is not logged in!';
              
          }
          else if(isset($_SESSION['mac-bank']['loggedIn']) && $_SESSION['mac-bank']['loggedIn'] == true){  
          
              $holdOtp = $formVars['otpCode'];           
              $formVars = $_SESSION['mac-bank']['formVars'];  // restore formVars with purchase data
              $formVars['ClientId'] = $_SESSION['mac-bank']['config']['clientId'];   // load saved clientid
              $formVars['otpCode'] = $holdOtp;

              // validate otp if no errors
              $mac = new MacRequest('registered', $formVars);
              if(empty($errorArray)) {
                  $returnArray = $mac->validateOTP($_SESSION['mac-bank']['otp']['RequestId']);
              }
              if($returnArray['status'] == true) { // ok to do move funds
                  $mac = new MacRequest('registered', $formVars);  // clears messages
                  $returnArray = $mac->moveFunds();
                  if($returnArray['status'] != true)
                     $errorArray[] = $returnArray['errors'][0];
                
                  $saveReturnArray = $returnArray;   // save for 'transfer.php'  
                  $formVars['action'] = 'transfer';   // return to form to display status
                  include 'transfer.php';
                  exit();   
              }
              
              
          }
          
      }

      // sending validate otp request for deposit or withdrawal
      if(isset($formVars['action']) &&
          ( $formVars['action'] == 'validate_otp_deposit' || $formVars['action'] == 'validate_otp_withdraw')) {  // authorizing payment
          if(!isset($_SESSION['mac-bank']['loggedIn']) || (isset($_SESSION['mac-bank']['loggedIn']) && $_SESSION['mac-bank']['loggedIn'] != true)){             
              $errorArray[] = 'User is not logged in!';
              
          }
          else if(isset($_SESSION['mac-bank']['loggedIn']) && $_SESSION['mac-bank']['loggedIn'] == true){  
          
              $holdOtp = $formVars['otpCode'];           
              $formVars = $_SESSION['mac-bank']['formVars'];  // restore formVars with purchase data
              $formVars['ClientId'] = $_SESSION['mac-bank']['config']['clientId'];   // load saved clientid
              $formVars['otpCode'] = $holdOtp;

              // validate otp if no errors
              $mac = new MacRequest('registered', $formVars);
              if(empty($errorArray)) {
                  $returnArray = $mac->validateOTP($_SESSION['mac-bank']['otp']['RequestId']);
              }
              if($returnArray['status'] == true) { // ok to do transaction
                  $mac = new MacRequest('registered', $formVars);  // clears messages
                  $returnArray = $mac->depositWithdraw($formVars['transaction']);
                  if($returnArray['status'] != true)
                     $errorArray[] = $returnArray['errors'][0];  // TODO - merge arrays
                  $saveReturnArray = $returnArray;   // save for 'funds.php'  
                  $formVars['action'] = 'funds';  // return to form to display status
                  include 'funds.php';
                  exit();   
              }
              
              
          }
          
      }
      
       // sending validate otp request for billpay
      if(isset($formVars['action']) && $formVars['action'] == 'validate_otp_paybill') {  // authorizing payment
          if(!isset($_SESSION['mac-bank']['loggedIn']) || (isset($_SESSION['mac-bank']['loggedIn']) && $_SESSION['mac-bank']['loggedIn'] != true)){             
              $errorArray[] = 'User is not logged in!';
              
          }
          else if(isset($_SESSION['mac-bank']['loggedIn']) && $_SESSION['mac-bank']['loggedIn'] == true){  
          
              $holdOtp = $formVars['otpCode'];           
              $formVars = $_SESSION['mac-bank']['formVars'];  // restore formVars with purchase data
              $formVars['ClientId'] = $_SESSION['mac-bank']['config']['clientId'];   // load saved clientid
              $formVars['otpCode'] = $holdOtp;

              // validate otp if no errors
              $mac = new MacRequest('registered', $formVars);
              if(empty($errorArray)) {
                  $returnArray = $mac->validateOTP($_SESSION['mac-bank']['otp']['RequestId']);
              }
              if($returnArray['status'] == true) { // ok to do transaction
                  $mac = new MacRequest('registered', $formVars);  // clears messages
                  $returnArray = $mac->payBill();
                  if($returnArray['status'] != true)
                     $errorArray[] = $returnArray['errors'][0];  // TODO - merge arrays
                  $saveReturnArray = $returnArray;   // save for 'bills.php'  
                  $formVars['action'] = 'paybillcomplete';  // return to form to display status
                  include 'bills.php';
                  exit();   
              }
              
              
          }
          
      }
     
      // sending validate otp from login
      else if(isset($formVars['action']) && $formVars['action'] == 'validate_otp_login') {  
          if(!isset($_SESSION['mac-bank']['loggedIn']) || (isset($_SESSION['mac-bank']['loggedIn']) && $_SESSION['mac-bank']['loggedIn'] != true)){             

              $formVars['ClientId'] = $_SESSION['mac-bank']['config']['clientId'];   // load saved clientid
              // request otp 
              $mac = new MacRequest('registered', $formVars);
              if(empty($errorArray)) {
                  $returnArray = $mac->validateOTP($_SESSION['mac-bank']['otp']['RequestId']);
                  if($returnArray['status'] == true) {   // setup successful login
                     // indicated logged in
                     $_SESSION['mac-bank']['loggedIn'] = true;
              
                     // go to page user was on when logging in
                      if(isset($_SESSION['mac-bank']['page-name']) && $_SESSION['mac-bank']['page-name'] != '')  {
                          $gotoPage = $_SESSION['mac-bank']['page-name']; 
                          if($gotoPage == 'main.php') {
                              $gotoPage = 'accounts.php'; // change to go directly to Accounts page after login from home page
                          }
                     }
                     else {
                         $gotoPage = 'main.php'; // change to go directly to Accounts page after login from home page
                     }  
                     include "$gotoPage";
                     exit(); 
                    }      
              }
              
          }
          else {
              $errorArray[] = 'User is already logged in!';
          }
        
      }   
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
      $otpFormType = $_SESSION['mac-bank']['otpFormType'];  // use the form type already set up
      include 'otpform.php'; // errors redisplay otp screen
  }
  else {
   //   $formVars['action'] = 'buy';
   //   include 'purchase.php'; // ok - proceed to purchase confirmation
  }
  
  unset($_SESSION['mac-bank']['formVarrs']);
          
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

