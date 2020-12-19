<?php
ini_set('display_errors', 1);
error_reporting(E_ERROR | E_WARNING | E_PARSE);

if(isset($_SESSION['mac-cart']['cart'])) 
   $cart_count = count($_SESSION['mac-cart']['cart']);
else
   $cart_count = 0;     
include_once 'includes/functions.php';

$_SESSION['mac-cart']['loggedIn'] = false;
unset($_SESSION['mac-cart']['user_id']);
unset($_SESSION['mac-cart']['fname']);
unset($_SESSION['mac-cart']['lname']);
unset($_SESSION['mac-cart']['billing-info']);
unset($_SESSION['mac-cart']['formVars']);
unset($_SESSION['mac-cart']['cart']);

ob_clean();    // clear previous head already 'included'
$page = 'main.php';

include $page;


?>
