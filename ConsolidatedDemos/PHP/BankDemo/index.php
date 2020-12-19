<?php
session_start();
ini_set('display_errors',1); 
error_reporting(E_ERROR | E_WARNING | E_PARSE);

include 'includes/iniFiles.php';

// get the most recently saved client id and other config data, if the session variable is not set
if(!isset($_SESSION['mac-bank']['config']) ||  $_SESSION['mac-bank']['config'] == '') {
   $iniArray = read_ini_file('../clientidbank.txt');
   if($iniArray === false) {
       $errorArray[] = '>>> error reading client id file';
   }
   else  {
       $_SESSION['mac-bank']['config'] = $iniArray['config'];      // set session varibles with the client id and other config data
   }
}

$page['main'] = 'main.php';
$page['accounts'] = 'accounts.php';
$page['log'] = 'log.php';
$page['bills'] = 'bills.php';
$page['paybill'] = 'bills.php';
$page['paybillcomplete'] = 'bills.php';
$page['transfer'] = 'transfer.php';
$page['transferfunds'] = 'transfer.php';
$page['funds'] = 'funds.php';
$page['depositwithdraw'] = 'funds.php';
$page['loginform'] = 'loginform.php';
$page['login'] = 'login.php';
$page['logout'] = 'logout.php';
$page['otpform'] = 'otpform.php';
$page['validate_otp'] = 'validate_otp.php';
$page['validate_otp_login'] = 'validate_otp.php';
$page['validate_otp_transfer'] = 'validate_otp.php';
$page['validate_otp_deposit'] = 'validate_otp.php';
$page['validate_otp_withdraw'] = 'validate_otp.php';
$page['validate_otp_paybill'] = 'validate_otp.php';
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
        $_SESSION['mac-bank']['debug'] = true;
    }
    if($formVars['debugrequest'] == 'dont') {
        unset($_SESSION['mac-bank']['debug']);
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

