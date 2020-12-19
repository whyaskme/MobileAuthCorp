<?php
  if(isset($_SESSION['mac-bank']['cart'])) 
      $cart_count = count($_SESSION['mac-bank']['cart']);
  else
      $cart_count = 0;     
  $path = pathinfo(__FILE__);    
  $_SESSION['mac-bank']['page-name'] = $path['basename'];

  include_once 'includes/functions.php';
  include_once 'includes/macRequest_class.php';

  $myAccounts = array();     // initialize array - used by page renderer, filled when logged in
   
  // if logged in show account details, else home page text
  // validate billing information
  if(isset($_SESSION['mac-bank']['loggedIn']) && $_SESSION['mac-bank']['loggedIn'] == true) {
      $formVars['user_id'] = $_SESSION['mac-bank']['user_id'];
      $formVars['fname'] = $_SESSION['mac-bank']['fname'];
      $formVars['lname'] = $_SESSION['mac-bank']['lname'];
      $formVars['ClientId'] = $_SESSION['mac-bank']['config']['clientId'];   // load saved clientid
      
      // get account details 
      $mac = new MacRequest('registered', $formVars);
      $returnArray = $mac->getAccountDetailsRegistered();
      if($returnArray['status'] != true)
          $errorArray[] = $returnArray['errors'][0];  // TODO - merge arrays
         
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

      // no error - display page   
      if(empty($errorArray)) {
         // set up data for listing registered user accounts in select list   
          if(isset($returnArray['accounts'])) { 
              $myAccounts = $returnArray['accounts']['Account']['SubAccounts']['SubAccount'];
              $primAccount = $returnArray['accounts']['Account'];
              $transLog = $returnArray['accounts']['Account']['ActivityLog']['LogEntry']; 
          }
          else {
              $myAccounts = array();
              $primAccount = array();
              $transLog = array();
          }
      }
     include_once 'header_basic.html.php';   // uses $cart_count
     include 'i_render_accounts.html.php';
     include 'footer_mac.html.php';
  }
  else { // not logged in yet
     include_once 'header_basic.html.php';   // uses $cart_count
     include 'i_login_form.html.php';
     include 'footer_mac.html.php';
  }
  
   
?> 
<script>
  $(document).ready(function() {
      // click events
  });

</script>
