<?php
ini_set('display_errors', 1);
error_reporting(E_ERROR | E_WARNING | E_PARSE);

    if(isset($_SESSION['mac-cart']['cart'])) 
       $cart_count = count($_SESSION['mac-cart']['cart']);
    else
       $cart_count = 0;     
    include_once 'includes/functions.php';
    include_once 'includes/validate_class.php';
    include_once 'includes/macRequest_class.php';

    // initialize SESSION
    unset($_SESSION['mac-cart']['user_id']);
    unset($_SESSION['mac-cart']['fname']);
    unset($_SESSION['mac-cart']['lname']);
    unset($_SESSION['mac-cart']['billing-info']);
    unset($_SESSION['mac-cart']['formVars']);
    
    // set up Registration URL if needed
    $requestUrl = $_SESSION['mac-cart']['config']['registerUrl'];

     //  user login
    $validFields = array('user_id' => 'email');
    $requiredFields = array('user_id');
    $val = new Validate();
    $returnArray = $val->validateUserFields($formVars,$validFields, $requiredFields);
    $errorArray = $returnArray['errorArray'];  // error messages or empty array

    // login if no errors
    if(empty($errorArray)) {
        
        $formVars['ClientId'] = $_SESSION['mac-cart']['config']['clientId'];   // load saved clientid
        $mac = new MacRequest('registered', $formVars);
        
        $status = $mac->validateLogin();
        if($status != true)
          $errorArray[] = 'error logging in';
               
        // request otp if no errors   
        if(empty($errorArray)) {
            // set up formVars with info from validateLogin above
            $formVars['lname'] = $_SESSION['mac-cart']['billing-info']['lname'];
            $formVars['fname'] = $_SESSION['mac-cart']['billing-info']['fname'];
           // request otp and send to enter otp screen
            $mac = new MacRequest('registered', $formVars);
            $otp = $mac->requestOTP('login');
            if($otp != true)
                 $errorArray[] = 'error requesting OTP code';
            
            if(empty($errorArray)) {
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
              
 
                // go to otp form
                $_SESSION['mac-cart']['otpFormType'] = $otpFormType = 'otp_login'; // use to customize the otp form
                include 'otpform.php';
                exit();
            }             
                
       }  
    }
    
    // check status in $_SESSION['mac-cart']['billing-info'] which is set by MacRequest calls
    // error messages  - red
    if(isset($_SESSION['mac-cart']['billing-info']['errors']) && !empty($_SESSION['mac-cart']['billing-info']['errors'])) {
        if($_SESSION['mac-cart']['billing-info']['status'] == false && substr($_SESSION['mac-cart']['billing-info']['errors'][0], 0, 25) == "ValidateLoginName Failed:")   {
            $_SESSION['mac-cart']['billing-info']['errors'][0] .= ' ...Re-enter the correct Login or Register.';
            $showRegister = "noclass";  // override class to show the Register button
        }
        $errorArray[] = $_SESSION['mac-cart']['billing-info']['errors'][0];  // TODO - merge arrays
        
        // Check if error(s) contains stop notification
        foreach($errorArray as $item) {
            if (strpos($item, "Blocked user replied 'STOP'") !== FALSE) {
                
                // User ID
                $userID = $formVars['user_id'];
                
                // get the short code
                $pieces = explode("=", $item);
                $shortCode = rtrim($pieces[1], ")");
                
                // redirect URL to notification instructions
                $params = '?action=STOP&userID='.$userID.'&shortCode='.$shortCode;
                //$params = '?action=STOP&shortCode='.$shortCode;                    
                $myNewUrl = $requestUrl.$params;
                
                header('Location: '.$myNewUrl);
                exit();
                
            }
        }
        
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
  
           
    if(!empty($errorArray)) {
         $userID = $formVars['user_id'];  // save for redisplay with errors
         $page = 'loginform.php';
    }
    else  {
         if($gotoPage != null) {
             $page = $gotoPage;
         }
         else if($gotoPage == null)  {
             ob_clean();
             $page = 'main.php';
         }
    }
         
     include $page;       

?>