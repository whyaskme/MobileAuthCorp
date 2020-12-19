<?php
ini_set('display_errors', 1);
error_reporting(E_ERROR | E_WARNING | E_PARSE);
    include_once 'includes/functions.php';
    include_once 'includes/validate_class.php';
    include_once 'includes/macRequest_class.php';

    // initialize SESSION
    unset($_SESSION['mac-bank']['user_id']);
    unset($_SESSION['mac-bank']['fname']);
    unset($_SESSION['mac-bank']['lname']);
    unset($_SESSION['mac-bank']['billing-info']);
    unset($_SESSION['mac-bank']['formVars']);
    
    // get current page URL
    $pageURL = 'http';
    if ($_SERVER["HTTPS"] == "on") {$pageURL .= "s";}
    $pageURL .= "://";
    if ($_SERVER["SERVER_PORT"] != "80") {
    	$pageURL .= $_SERVER["SERVER_NAME"].":".$_SERVER["SERVER_PORT"].$_SERVER["REQUEST_URI"];
    } else {
    	$pageURL .= $_SERVER["SERVER_NAME"].$_SERVER["REQUEST_URI"];
    }
    
    $fromURL = strtok($pageURL, "?");
    
    // set up Registration URL if needed
    $requestUrl = $_SESSION['mac-bank']['config']['registerUrl'];
    
    // reset registration link['mac-bank']['config']['clientId']
    $requestUrl = $_SESSION['mac-bank']['config']['registerUrl'];  //  build url if not logged in
    $params = '?action=reg&demo='.urlencode($_SESSION['mac-bank']['config']['clientName']);
    $params .= '&cid='.$_SESSION['mac-bank']['config']['clientId'];
    $params .= '&fromUrl='.$fromURL;
    $registerUrl = $requestUrl.$params;

     //  user login
    $validFields = array('user_id' => 'email');
    $requiredFields = array('user_id');
    $user = new Validate();
    $returnArray = $user->validateUserFields($formVars,$validFields, $requiredFields);
    $errorArray = $returnArray['errorArray'];  // error messages or empty array

    // login if no errors
    if(empty($errorArray)) {
        
        $formVars['ClientId'] = $_SESSION['mac-bank']['config']['clientId'];   // load saved clientid
        $mac = new MacRequest('registered', $formVars);
        
        $returnArray = $mac->validateLogin();
               
        // request otp if no errors   
        if($returnArray['status'] == true) {
            
            // save user login information
            $_SESSION['mac-bank']['fname'] =  $returnArray['fname'];
            $_SESSION['mac-bank']['lname'] =  $returnArray['lname'];
            $_SESSION['mac-bank']['user_id'] = $formVars['user_id'];
                       
            // set up formVars with info from validateLogin above
            $formVars['lname'] = $_SESSION['mac-bank']['lname'];
            $formVars['fname'] = $_SESSION['mac-bank']['fname'];
            
           // request otp and send to enter otp screen
            $mac = new MacRequest('registered', $formVars);
            $returnArray = $mac->requestOTP('login');
            
            if($returnArray['status'] != true)
               $errorArray[] = $returnArray['errors'][0];  // TODO - merge arrays
            
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
                    $myNewUrl = $requestUrl.$params;
                    
                    header('Location: '.$myNewUrl);
                    exit();
                    
                }
            }
            
            
            if(empty($errorArray)) {
                // error messages  - red
                if(isset($returnArray['errors'][0]) && !empty($returnArray['errors'])) {
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
 
                // go to otp form
                $_SESSION['mac-bank']['otpFormType'] = $otpFormType = 'otp_login'; // use to customize the otp form
                include 'otpform.php';
                exit();
            }
                
       }  
    }
    
    // check status in $returnArray is set by MacRequest calls above
    // error messages  - red
    if(isset($returnArray['errors']) && !empty($returnArray['errors'])) {
        if($returnArray['status'] == false && substr($returnArray['errors'][0], 0, 25) == "ValidateLoginName Failed:")   {
            //$returnArray['errors'][0] .= ' ...Re-enter the correct Login or Register.';
            $returnArray['errors'][0] .= ' Please enter a valid login or register a new user.';
            $showRegister = "noclass";  // override class to show the Register button
        }
        $errorArray[] = $returnArray['errors'][0];  // TODO - merge arrays
    }
    // success message -  green
    $messageArray = array();
    if(empty($errorArray)) {
        $messageArray = $returnArray['messages'];
    }
    // info messages - orange
    $infoArray = array();
    $infoArray[] = $returnArray['info-message'];
           
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