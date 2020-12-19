<?php
  $path = pathinfo(__FILE__);    
  $_SESSION['mac-bank']['page-name'] = $path['basename'];

  include_once 'includes/functions.php';
  include_once 'includes/macRequest_class.php';

  $myAccounts = array();     // initialize array - used by page renderer, filled when logged in
   
  // if logged in show account details, else home page text
  // validate billing information
  if(isset($_SESSION['mac-bank']['loggedIn']) && $_SESSION['mac-bank']['loggedIn'] == true) {
      $formVars['ClientId'] = $_SESSION['mac-bank']['config']['clientId'];
      $formVars['user_id'] = $_SESSION['mac-bank']['user_id'];
      $formVars['fname'] = $_SESSION['mac-bank']['fname'];
      $formVars['lname'] = $_SESSION['mac-bank']['lname'];
                                                      
      // get account details 
      $mac = new MacRequest('registered', $formVars);
      $returnArray = $mac->getAccountDetailsRegistered();
      if($returnArray['status'] != true)
          $errorArray[] = $returnArray['errors'][0];  // TODO - merge arrays
      else   {   // success
          $returnArray['messages'] = array();   // don't need a success message here   
         // set up data for listing registered user accounts in select list   
          if(isset($returnArray['accounts'])) {   // set up account list for display
              $myAccounts = $returnArray['accounts']['Account']['SubAccounts']['SubAccount'];
              $primAccount = $returnArray['accounts']['Account'];
          }
          else {
              $myAccounts = array();
              $primAccount = array();
          }
      }
          
             
     if($formVars['action'] == 'depositwithdraw') {
          // validate input
          $selected_account = $formVars['account_affected'];
          if(!isset($formVars['account_affected']) || $formVars['account_affected'] == '') {
              $errorArray[] = 'Please select an <strong>Account</strong>';
          }
          if(!isset($formVars['transaction']) || $formVars['transaction'] == '') {
              $errorArray[] = 'Please select a <strong>Transaction</strong>';
          }
          if(!isset($formVars['transfer_amount']) || $formVars['transfer_amount'] == '') {
              $errorArray[] = 'Please enter an <strong>Amount</strong>';
          }
          else if(empty($errorArray)) {  // ok to start transfer
            $mac = new MacRequest('registered', $formVars);
            $returnArray = $mac->requestOTP('auth');
            if($returnArray['status'] != true)
               $errorArray[] = $returnArray['errors'][0];  // TODO - merge arrays
                 
            if(empty($errorArray)) {
               $messageArray[] = 'Transaction: '.$formVars['transaction'];
               $messageArray[] = 'Account: '.$formVars['account_affected'];
               $messageArray[] = 'Amount: '.$formVars['transfer_amount'];
                    
               if($formVars['transaction'] == 'deposit')
                  $_SESSION['mac-bank']['otpFormType'] = $otpFormType = 'otp_deposit';
               else if($formVars['transaction'] == 'withdraw')
                  $_SESSION['mac-bank']['otpFormType'] = $otpFormType = 'otp_withdraw';
                     
               include 'otpform.php';
               exit();
            }
          }
     }
     // error messages  - red
     if(isset($saveReturnArray['errors']) && !empty($saveReturnArray['errors'])) {
        $errorArray[] = $saveReturnArray['errors'][0];  // TODO - merge arrays
     }
     // success message -  green
     $messageArray = array();
     if(empty($errorArray)) {
        $messageArray = $saveReturnArray['messages'];
     }
     // info messages - orange
     $infoArray = array();
     $infoArray[] = $saveReturnArray['info-message'];
              
  }
  else {
      include 'loginform.php';
      exit();
  }
   
  include_once 'header_basic.html.php';   // uses $myAccounts
  include 'i_render_funds.html.php';
  include 'footer_mac.html.php';
  
?> 
<script>
  $(document).ready(function() {
      // click events
      
  });

</script>
