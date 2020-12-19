<?php
session_start();  
ini_set('display_errors',1); 
error_reporting(E_ERROR | E_WARNING | E_PARSE);

// allow requests to this file only if ajax call
if(!(isset($_SERVER['HTTP_X_REQUESTED_WITH']) && $_SERVER['HTTP_X_REQUESTED_WITH'] == 'XMLHttpRequest') )
    exit();
 
// set default timezone
date_default_timezone_set('America/Chicago');     // default for all date/time operation     
    
session_start();
require_once 'includes/product_class.php';

    $docroot = $_SERVER['DOCUMENT_ROOT'];
    $httproot = $_SERVER['HTTP_HOST'];
    
//  get POST parameters, clean them up-- ajax call should all be POST's, except for 'doc_open', which is not really an ajax call
$formVars = array();
if(!empty($_POST)) {
  foreach($_POST AS $key=>$value){
    $formVars[$key] = trim(stripslashes($value));
  }
}

// handle all GETS and POSTS - from AJAX calls, and doc_open
switch($formVars['type']) {
  
   // cart data will be saved in SESSION variable, and will be available until SESSION disappears, or user clears it, or order is completed 
   case 'add_to_cart': 
    
      $table = array();      // initialize
      $saveItemIndex = -1;   
      $msg = '';
      $validFields = array('product_number');
      foreach($formVars as $name => $value) {
          if(in_array($name, $validFields)) 
               $table[$name] = $value;        // hold item info in array
      }
      if(!empty($table)) { 
          // check to see if item already in cart
          if(isset($_SESSION['mac-cart']['cart']) && !empty($_SESSION['mac-cart']['cart']))  {
              foreach($_SESSION['mac-cart']['cart'] as $index => $item) {
                  if($item['product_number'] == $table['product_number']) {
                      $saveItemIndex = $index;  // save index of already added item
                  }
              }
          }
          // if not already in cart, just add it
          if($saveItemIndex == -1) {
             $holdArray = array('product_number' => $table['product_number'], 'qty' => 1); // prepare cart entry
             $_SESSION['mac-cart']['cart'][] = $holdArray;   // newly added item
             $msg = 'added';
          }
          else {   // if already in cart, increment qty
             $_SESSION['mac-cart']['cart'][$saveItemIndex]['qty']++; // update qty 
             $msg = 'updated';
          }
              
          echo json_encode(array('status'=> true,  'data' => $_SESSION['mac-cart'], 'message' => $msg));
      }
      else
          echo json_encode(array('status' => false, 'data' => $table, 'message' => 'error adding to cart'));
      break;
      
   case 'remove_from_cart':
   
      $table = array();
      $errorFlag = true;   // initialize
      
      $validFields = array('id');
      foreach($formVars as $name => $value) {
          if(in_array($name, $validFields)) 
               $table[$name] = $value;
      }
      if($table['id'] == '') {                                                                     
          unset($_SESSION['mac-cart']['cart']);
          $errorFlag = false;
      }
      else if(strlen($table['id']) > 0) {
          foreach($_SESSION['mac-cart']['cart'] as $index => $theCart) {
              if($theCart['product_number'] == substr($table['id'], 4)) {
                  unset($_SESSION['mac-cart']['cart'][$index]);
                  $errorFlag = false;
              }
          }
      }
      else {
          $errorFlag = true;
      }
      if(empty($_SESSION['mac-cart']['cart'])) {  // if cart is empty then delete it
         unset($_SESSION['mac-cart']['cart']);
      }
      if($errorFlag == false)
          echo json_encode(array('status' => true, 'data' => $table, 'message' => 'cart successfully removed'));
      else
          echo json_encode(array('status' => false, 'data' => $table, 'message' => 'problem removing from cart'));
      break;
      
   case 'save_cart':
      
      // save qty into SESSION
      $errorFlag = true;
      foreach($_SESSION['mac-cart']['cart'] as $index => $theCart) {
          if($theCart['product_number'] == $formVars['product_number']) {
              $_SESSION['mac-cart']['cart'][$index]['qty'] = $formVars['qty'];
              $errorFlas = false;  
          }
      }
      if($errorFlag == false)
          echo json_encode(array('status' => true,  'message' => 'cart successfully updated'));
      else
          echo json_encode(array('status' => false,  'message' => 'problem updating cart'));
      break;
      
   case 'save_temp_validation_data':   // save to SESSION, for later use
   
      $errorFlag = true;
      foreach($formVars as $field => $value) {
          $_SESSION['mac-cart']['validation-data'][$field] = trim($value);
          $errorFlag = false;
      }
      $savedCart = $_SESSION['mac-cart']['validation-data'];
      
      if($errorFlag == false)
          echo json_encode(array('status' => true, 'data' => null , 'message' => 'validation data successfully saved'));
      else
          echo json_encode(array('status' => false, 'data' =>  null, 'message' => 'problem saving validation data'));
      break;
                
    case 'set_user_type':
      $_SESSION['mac-cart']['user-type'] = $formVars['user_type'];
      echo json_encode(array('status' => true, 'data' => array('user_type' => $_SESSION['mac-cart']['user-type']), 'message' => 'success setting user type')); 
      break;        
       
             
 
   default:
      echo json_encode(array('status' => false, 'message' => 'invalid request'));
      break;
         
   
}
      

?>
