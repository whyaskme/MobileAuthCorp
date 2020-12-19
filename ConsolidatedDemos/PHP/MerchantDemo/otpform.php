<?php
  if(isset($_SESSION['mac-cart']['cart'])) 
      $cart_count = count($_SESSION['mac-cart']['cart']);
  else
      $cart_count = 0;     

    include_once 'includes/functions.php';
    include_once 'includes/macRequest_class.php';
  
     
  if(isset($formVars['refresh']) && $formVars['refresh'] == 'yes') {  // request another OTP code
      $formVars['ClientId'] = $_SESSION['mac-cart']['config']['clientId'];   // load saved clientid
      if(!isset($_SESSION['mac-cart']['loggedIn']) || (isset($_SESSION['mac-cart']['loggedIn']) && $_SESSION['mac-cart']['loggedIn'] != true)) {
          $mac = new MacRequest('clientManaged', $formVars);
      }
      else if(isset($_SESSION['mac-cart']['loggedIn']) && $_SESSION['mac-cart']['loggedIn'] == true) {
          $mac = new MacRequest('registered', $formVars);
      }
      // request a 'resend' of OTP
      $otp = $mac->resendOTP();
      if($otp != true)  {
          $errorArray[] = 'error resending OTP code';
      }
      else if(isset($formVars['refresh']) && $formVars['refresh'] == 'yes') {
         $_SESSION['mac-cart']['billing-info']['info-message'] = 'Authentication Code resent.';
      }
        
       // check status in $_SESSION['mac-cart']['billing-info'] which is set by MacRequest calls
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
  
      
      if(!isset($_SESSION['mac-cart']['otpFormType'])) {
           $_SESSION['mac-cart']['otpFormType'] = $otpFormType = 'otp_purchase'; //  ??
      }
      else  {
          $otpFormType = $_SESSION['mac-cart']['otpFormType'];   // use saved form type (purchase or login)
      }
  }
  else { // initial display of otp form
      // save form data
      $_SESSION['mac-cart']['formVars'] = $formVars;  // use on purchase screen, $formVars will be lost by then
  }
  
  include_once 'header_basic.html.php';   // uses $cart_count
  include 'i_otp_form.html.php';
  include 'footer_basic.html'; 
  
?> 
<script>
  $(document).ready(function() {
      // on load
      $('input[name="otp"]').focus();
      
      $('form').submit(function(ev) {
          $('#form-status').show();
          $('#otp-submit-button').prop('disabled', true);
          setTimeout(function() {    // wait 2 seconds
              return true;
              }, 2000);
      });  
  });


</script>
