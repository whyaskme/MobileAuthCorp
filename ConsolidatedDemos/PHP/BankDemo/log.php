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
              if(isset($transLog['Date'])) {
                   $newLog = array();
                   $newLog[] = $transLog;  // fix array with one log entry
                   $transLog = $newLog;
           
              }
          }
          else {
              $myAccounts = array();
              $primAccount = array();
              $transLog = array();
          }
      }
     include_once 'header_basic.html.php';   // uses $cart_count
     include 'i_render_log.html.php';
     include 'footer_mac.html.php';
  }
  else { // not logged in yet
     include_once 'header_basic.html.php';   // uses $cart_count
     include 'i_login_form.html.php';
     include 'footer_mac.html.php';
  }
  
   
  
 if(!empty($myAccounts))
     $json_myAccounts = json_encode($myAccounts);
 else
     $json_myAccounts = '';
?> 
<script>
  $(document).ready(function() {
      // click events
      var myAccounts = <?php echo $json_myAccounts ?>;
      
    $('#account_review').change(function(ev) {
          var outRow= '';
          $('#display_details .insert-detail-rows h4 .trans-button-container').show();
          $('#display_details .insert-detail-rows h4').nextAll().remove();  // clear detail area
          var acct = $('#account_review').val();  // get selected account
          
          for(var i=0; i<myAccounts.length; i++) {
              if(acct == myAccounts[i]['Name']) {
                  for(var Name in myAccounts[i]) {
                      outRow +=  '<div class="row indent-text-10"><div class="col-sm-4 bold-text">'+Name+'</div><div class="col-sm-6 indent-text-10">'+myAccounts[i][Name]+'</div></div>';
                  }
                  $('#display_details .insert-detail-rows').append(outRow);
                  outRow = '';
              }
          }
      });
      
      $("#transaction_log .row:even").css("background-color", 'WhiteSmoke');
                
      
  });

</script>
