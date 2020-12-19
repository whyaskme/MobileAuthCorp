<?php
ini_set('display_errors', 1);
error_reporting(E_ERROR | E_WARNING | E_PARSE);

include_once 'includes/functions.php';

$_SESSION['mac-bank']['loggedIn'] = false;
unset($_SESSION['mac-bank']['user_id']);
unset($_SESSION['mac-bank']['fname']);
unset($_SESSION['mac-bank']['lname']);
unset($_SESSION['mac-bank']['billing-info']);
unset($_SESSION['mac-bank']['formVars']);

ob_clean();    // clear previous head already 'included'
$page = 'main.php';

include $page;


?>
