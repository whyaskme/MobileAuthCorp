<?php
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
  
  $returnArray = $mac->getServerClientId($_SESSION['mac-bank']['config']['defaultClientId'], $_SESSION['mac-bank']['config']['clientName']);
  if($returnArray['status'] != true)
     $errorArray[] = 'error retrieving default client id';
     
  if(!empty($returnArray['errors'])) 
     $errorArray = $returnArray['errors'];   
  
  // no error - display page   
  if(empty($errorArray)) {
     // set up data for listing registered user accounts in select list   
      if(isset($returnArray['serverClientId'])) { 
          $serverClientId = $returnArray['serverClientId'];
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
	  $status = write_ini_file($outArray, '../clientidbank.txt');
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
   $iniArray = read_ini_file('../clientidbank.txt');
   if($iniArray === false) {
	   $errorArray[] = '>>> error reading client id file';
   }
   else  {
	   $_SESSION['mac-bank']['config'] = $iniArray['config'];      // set session varibles with the client id and other config data
   } 
   
   // get all account
   $formVars['ClientId'] = $_SESSION['mac-bank']['config']['clientId'];
   $mac = new MacRequest(null, $formVars);
   $returnArray = $mac->getAllAccounts();
   if($returnArray['status'] != true)
      $errorArray[] = $returnArray['errors'][0];  // TODO - merge arrays
   else  {   // success
      $returnArray['messages'] = array();   // don't need a success message here   
      $returnArray['response']['PANList'] = explode('|', $returnArray['response']['PANList']);
      $returnArray['response']['AccountHoldersList'] = explode('|', $returnArray['response']['AccountHoldersList']);
      $returnArray['response']['AccountNamesList'] = explode('|', $returnArray['response']['AccountNamesList']);
      $returnArray['response']['LoginNamesList'] = explode('|', $returnArray['response']['LoginNamesList']);
   }   
   
   
  // display client id and name
  include_once 'header_basic.html.php';   // uses $clientId, clientName
  include 'i_clientid_form.html.php';  
    
?> 
