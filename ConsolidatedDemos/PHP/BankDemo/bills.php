<?php
  $path = pathinfo(__FILE__);    
  $_SESSION['mac-bank']['page-name'] = $path['basename'];

  include_once 'includes/functions.php';
  include_once 'includes/macRequest_class.php';

  $myAccounts = array();     // initialize array - used by page renderer, filled when logged in
  $myBills  = array();
   
  // if logged in show account details, else home page text
  // validate billing information
  if(isset($_SESSION['mac-bank']['loggedIn']) && $_SESSION['mac-bank']['loggedIn'] == true) {
      $formVars['ClientId'] = $_SESSION['mac-bank']['config']['clientId'];
      $formVars['user_id'] = $_SESSION['mac-bank']['user_id'];
      $formVars['fname'] = $_SESSION['mac-bank']['fname'];
      $formVars['lname'] = $_SESSION['mac-bank']['lname'];
                                                      
     if($formVars['action'] == 'paybill') {
          // validate input
          $selected_account = $formVars['accountName'];
          if(!isset($formVars['accountName']) || $formVars['accountName'] == '') {
              $errorArray[] = 'Please select an account';
          }
          if(strtolower($formVars['payAmount']) == 'full') {
              $formVars['payAmount'] = $formVars['billAmount'];  // full means pay whole amount
          } 
          $payAmount = save_money_amount($formVars['payAmount']);
          $billAmount = save_money_amount($formVars['billAmount']);
          if($payAmount == '' || $payAmount <= 0) {
              $errorArray[] = 'Please enter pay amount';
          }
          if($payAmount > $billAmount) {
              $errorArray[] = 'Pay amount is greater than bill amount';
          }
          if(empty($errorArray)) {  // ok to start pyment
            $mac = new MacRequest('registered', $formVars);
            $returnArray = $mac->requestOTP('auth');
            if($returnArray['status'] != true)
               $errorArray[] = $returnArray['errors'][0];  // TODO - merge arrays
                 
            if(empty($errorArray)) {
               $messageArray[] = 'Transaction: billpay';
               $messageArray[] = 'Invoice: '.$formVars['invoice'];
               $messageArray[] = 'Amount: '.$formVars['payAmount'];
                
               $_SESSION['mac-bank']['otpFormType'] = $otpFormType = 'otp_paybill';
                     
               include 'otpform.php';
               exit();
            }
          }
     }
     

      // get account details 
      $mac = new MacRequest('registered', $formVars);
      $returnArray = $mac->getAccountDetailsRegistered();
      if($returnArray['status'] == true)  {
          $returnArray['messages'] = array();   // don't need a success message here   
         // set up data for listing registered user accounts in select list   
          if(isset($returnArray['accounts'])) {   // set up account list for display
              $myAccounts = $returnArray['accounts']['Account']['SubAccounts']['SubAccount'];
          }
          else {
              $myAccounts = array();
          }

          $returnArray = $mac->getBills();
          if($returnArray['status'] == true) {
              // set up data for listing bills in table   
              if(isset($returnArray['accounts'])) {   // set up account list for display
                  $myBills = $returnArray['accounts']['Bills']['Bill'];
              }
              else {
                  $myBills = array();
              }
          }
          if($formVars['action'] == 'bills') {  // menu clock got us here
              $returnArray['messages'] = array();  // clear out success messages
          }
      }
             
     // error messages  - red
     if(isset($returnArray['errors']) && !empty($returnArray['errors'])) {
        $errorArray[] = $returnArray['errors'][0];  // TODO - merge arrays
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
  include 'i_render_bills.html.php';
  include 'footer_mac.html.php';
  
?> 
<script>
  $(document).ready(function() {
      
      // click events
      $('.pay-bill-button').click(function(ev) {
          // gather bill pay data from grid
           var target = ev.currentTarget.parentNode;
           var invoice = $(target).find('.bill-invoice').text();
           var billAmount = $(target).find('.bill-amount').text();
           var payAmount = $('input[name="pay_bill_amount"]').val();
           var accountName = $('#account_affected').val();
           var url = "index.php?action=paybill&invoice="+invoice+"&billAmount="+billAmount+"&accountName="+accountName+"&payAmount="+payAmount;
           window.location = url;
     });
      
  });

</script>
