<?php
  if(isset($_SESSION['mac-bank']['cart'])) 
      $cart_count = count($_SESSION['mac-bank']['cart']);
  else
      $cart_count = 0;     

    include_once 'includes/functions.php';
    include_once 'includes/macRequest_class.php';
  
  // save form data - if first time load right after a form submit
  ///  - don't save if redsiplaying due to errors, or refresh - trying to save the original transaction submit formVars
  if(!isset($formVars['refresh']) || $formVars['refresh'] != 'yes' || !empty($errorArray)) 
      $_SESSION['mac-bank']['formVars'] = $formVars;  // save it - $formVars will be lost   
     
  if(isset($formVars['refresh']) && $formVars['refresh'] == 'yes') {  // request another OTP code
      $formVars['ClientId'] = $_SESSION['mac-bank']['config']['clientId'];   // load saved clientid
      if(!isset($_SESSION['mac-bank']['loggedIn']) || (isset($_SESSION['mac-bank']['loggedIn']) && $_SESSION['mac-bank']['loggedIn'] != true)) {
          $mac = new MacRequest('clientManaged', $formVars);
      }
      else if(isset($_SESSION['mac-bank']['loggedIn']) && $_SESSION['mac-bank']['loggedIn'] == true) {
          $mac = new MacRequest('registered', $formVars);
      }
      // request a 'resend' of OTP based on the saved RequestId
      $returnArray = $mac->resendOTP($_SESSION['mac-bank']['otp']['RequestId']);
      if($returnArray['status'] == true && isset($formVars['refresh']) && $formVars['refresh'] == 'yes') {
         //$returnArray['info-message'] = 'OTP Code Resent';
      }
        
       // check status in $_SESSION['mac-bank']['billing-info'] which is set by MacRequest calls
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
      
      $otpFormType = $_SESSION['mac-bank']['otpFormType'];   // use saved form type (bank transaction or login)
  }
  
  include_once 'header_basic.html.php';  
  include 'i_otp_form.html.php';
  include 'footer_mac.html.php'; 
  
?> 
<script>
  $(document).ready(function() {
      // on load
      $('input[name="otpCode"]').focus();
      
      $('form').submit(function(ev) {
          //$('#form-status').show();
          ShowProcessingMessage();
          $('#otp-submit-button').prop('disabled', true);
          setTimeout(function() {    // wait 2 seconds
              return true;
              }, 2000);
      });  
  });


</script>
