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
          
                   
     if($formVars['action'] == 'transferfunds') {
          // validate input
          $selected_from_account = $formVars['account_from'];
          $saveAcctNum = trim(str_replace('-','',$formVars['account_to_number']));
          //if(!isset($formVars['account_from']) || $formVars['account_from'] == ''
          //   || !isset($formVars['account_to_name']) || $formVars['account_to_name'] == ''
          //   || !isset($formVars['account_to_number']) || $saveAcctNum == '') {

          //    $errorArray[] = 'Please enter account information below';
          //}
          if(!isset($formVars['account_from']) || $formVars['account_from'] == '') {
              $errorArray[] = 'Please select a <strong>Transfer From Account</strong>';
          }
          if(!isset($formVars['transfer_amount']) || $formVars['transfer_amount'] == '') {
              $errorArray[] = 'Please enter an <strong>Amount</strong>';
          }
          if(!isset($formVars['account_to_name']) || $formVars['account_to_name'] == '') {
              $errorArray[] = 'Please enter the name of the <strong>Transfer To Account Holder</strong>';
          }
          if(!isset($formVars['account_to_number']) || $saveAcctNum == '') {
              $errorArray[] = 'Please enter the <strong>Transfer To Account Number</strong>';
          }
          
          else if(empty($errorArray)) {  // ok to start transfer
            $mac = new MacRequest('registered', $formVars);
            $returnArray = $mac->requestOTP('auth');
            if($returnArray['status'] != true)
               $errorArray[] = $returnArray['errors'][0];  // TODO - merge arrays
                 
            if(empty($errorArray)) {
               $messageArray[] = 'Transaction: transfer';
               $messageArray[] = 'From Account: '.$formVars['account_from'];
               $messageArray[] = 'To Account Name: '.$formVars['account_to_name'];
               $messageArray[] = 'To Account Number: '.$formVars['account_to_number'];
               $messageArray[] = 'Amount: '.$formVars['transfer_amount'];

               $_SESSION['mac-bank']['otpFormType'] = $otpFormType = 'otp_transfer';
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
     if(!empty($saveReturnArray['info-message'])) {
        $infoArray[] = $saveReturnArray['info-message'];
     }
                    
  }
  else {
      include 'loginform.php';
      exit();
  }
   
  include_once 'header_basic.html.php';   // uses $myAccounts
  include 'i_render_transfer.html.php';
  include 'footer_mac.html.php';
    
?> 
<script>
  $(document).ready(function() {
      // click events
      // format mask for credit card entry
     new Formatter(document.getElementById('account_to_number'), {
        'pattern': '{{9999}}-{{9999}}-{{9999}}-{{9999}}',
        'persistent' : true
     });
      
  });

</script>
