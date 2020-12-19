<?php
session_start();
ini_set('display_errors',1); 
error_reporting(E_ERROR | E_WARNING | E_PARSE);       // for production
//error_reporting(E_ALL);                             // for test

// get current page URL
//$currentURL = 'http';
//if ($_SERVER["HTTPS"] == "on") {$currentURL .= "s";}
//$currentURL .= "://";
$currentURL = '';
if ($_SERVER["SERVER_PORT"] != "80") {
	$currentURL .= $_SERVER["SERVER_NAME"].":".$_SERVER["SERVER_PORT"].$_SERVER["REQUEST_URI"];
} else {
	$currentURL .= $_SERVER["SERVER_NAME"].$_SERVER["REQUEST_URI"];
}
// remove this line
//$currentURL = 'https://qa.otp-ap.us/iis/GolfShop/Default.aspx';

$myURL = strtok($currentURL, ".");

// This is to determine if http/https precedes the URL
// remove the first 7 characters
//$newVar = substr($myURL, 7);
$protocol = substr($myURL, 0, 5);
if($protocol == 'http:') {
	$myEnvironment = substr($myURL, 7);
} else	if($protocol == 'https') {
	$myEnvironment = substr($myURL, 8);
} else {
	$myEnvironment = $protocol;
}

switch ($myEnvironment) {
	case "qa":
		// add code here
		break;
	case "test":
		// add code here
		break;
	case "demo":
		// add code here
		break;
	case "www":
		// add code here
		break;
	case "test":
		// add code here
		break;
	case "test":
		// add code here
		break;
	
	default:
        //echo "Your favorite color is neither red, blue, or green!";
        echo "";
}

include 'includes/iniFiles.php';

// get the most recently saved client id and other config data, if the session variable is not set
if(!isset($_SESSION['mac-cart']['config']) ||  $_SESSION['mac-cart']['config'] == '') {
   $iniArray = read_ini_file('../clientidcart.txt');
   if($iniArray === false) {
       $errorArray[] = '>>> error reading client id file';
   }
   else  {
       $_SESSION['mac-cart']['config'] = $iniArray['config'];      // set session varibles with the client id and other config data
   }
}

$page['main'] = 'main.php';
$page['mac_main'] = 'main.php';
$page['products'] = 'products.php';
$page['cart'] = 'cart.php';
$page['order'] = 'order.php';
$page['checkout'] = 'checkout.php';
$page['about'] = 'about.php';
$page['verify'] = 'verify.php';
$page['loginform'] = 'loginform.php';
$page['login'] = 'login.php';
$page['logout'] = 'logout.php';
$page['profile'] = 'profile.php';
$page['auth'] = 'verify.php';
$page['otpform'] = 'otpform.php';
$page['validate_otp'] = 'validate_otp.php';
$page['validate_otp_login'] = 'validate_otp.php';
$page['buy'] = 'purchase.php';
$page['printer_friendly'] = 'purchase_print.php';
$page['showclientid'] = 'clientid.php';
$page['saveclientid'] = 'clientid.php';

$formVars = '';
foreach($_REQUEST as $param => $value) {
    $formVars[$param] = stripslashes(trim($value));
}

// turn on or off debuggin with additional GET parameter '&debugrequest=do' or '&debugrequest=dont'
// - after turning it on, it will stay on until turned off with '&debugrequest=dont'
if(isset($formVars['debugrequest'])) {
    if($formVars['debugrequest'] == 'do'){                    
        $_SESSION['mac-cart']['debug'] = true;
    }
    if($formVars['debugrequest'] == 'dont') {
        unset($_SESSION['mac-cart']['debug']);
    }
}
if(isset($formVars['action'])) {
    if($page[$formVars['action']] != null) {
        if(file_exists($page[$formVars['action']]))  {
            include $page[$formVars['action']];  
        }
        else  {
            echo 'page not available';
        }
    }
    else  {
        include $page['main'];
    }
}
else {
    include $page['main'];
}
?>
