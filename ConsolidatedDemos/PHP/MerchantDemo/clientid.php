<?php
  if(isset($_SESSION['mac-cart']['cart'])) 
      $cart_count = count($_SESSION['mac-cart']['cart']);
  else
      $cart_count = 0;     
  $path = pathinfo(__FILE__);    
  $_SESSION['mac-cart']['page-name'] = $path['basename'];

  include_once 'includes/functions.php';
  include_once 'includes/macRequest_class.php';

  $errorArray = array();
  $messageArray = array();
  $clientArray = array();
  $clientId = '';
  $clientName = '';
  $outArray = array();
  
  // get server client id          // get account details
  $mac = new MacRequest(null, null);
  
  $status = $mac->getServerClientId($_SESSION['mac-cart']['config']['defaultClientId'], $_SESSION['mac-cart']['config']['clientName']);
  if($status != true)
     $errorArray[] = 'error retrieving default client id';
     
  if(!empty($_SESSION['mac-cart']['billing-info']['errors'])) 
     $errorArray = $_SESSION['mac-cart']['billing-info']['errors'];   
  
  // no error - display page   
  if(empty($errorArray)) {
     // set up data for listing registered user accounts in select list   
      if(isset($_SESSION['mac-cart']['billing-info']['serverClientId'])) { 
          $serverClientId = $_SESSION['mac-cart']['billing-info']['serverClientId'];
      }
      else {
          $serverClientId = '';
      }
  }

  
  // save the new client namd and id
  if(isset($formVars['action']) && $formVars['action'] == 'saveclientid') { 
      foreach($formVars as $field => $value ) {
          if($field == 'Submit' || $field == 'action' || $field == 'serverClientId' )
             continue;
          $outArray['config'][$field] = $value;
      } 
      // use server client id ?
      if(isset($formVars['useServerClientId']) && $formVars['useServerClientId'] == '1') {   // checkbox is checked
          $outArray['config']['clientId'] = $serverClientId;     // use default, else use entered id
      }    
	  $status = write_ini_file($outArray, '../clientidcart.txt');
	  if($status === false) {
		  $errorArray[] = '>>> error writing clientid file';
	  }
	  else {
		  $messageArray[] = 'success saving clientid';
	  }
      if(!empty($errorArray))   { 
     // redisplay input on validiation errors
          $clientName = $formVars['clientname'];
          $clientId = $formVars['clientid'];

          include_once 'header_basic.html.php';   // uses $clientId, clientName
          include 'i_clientid_form.html.php';
          exit();
      }
      
  }
   
   // get the most recently saved client id and name
   $iniArray = read_ini_file('../clientidcart.txt');
   if($iniArray === false) {
	   $errorArray[] = '>>> error reading client id file';
   }
   else  {
	   $_SESSION['mac-cart']['config'] = $iniArray['config'];      // set session varibles with the client id and other config data
   }
   
  // display client id and name
  include_once 'header_basic.html.php';   // uses $clientId, clientName
  include 'i_clientid_form.html.php';  
    
?> 
