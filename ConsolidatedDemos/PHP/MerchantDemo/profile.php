<?php
  if(isset($_SESSION['mac-cart']['cart'])) 
      $cart_count = count($_SESSION['mac-cart']['cart']);
  else
      $cart_count = 0;   
  $_SESSION['mac-cart']['page-name'] = pathinfo(__FILE__)['basename'];
        
  include 'header_basic.html.php';   // uses $cart_count for cart count display in header
  include_once 'includes/functions.php';
  include_once 'includes/db_class.php';
  include_once 'includes/user_class.php';  
  include_once 'includes/macCurlSubmitter_class.php';
  
  // connect to database
  $user = new ClientUser();

  // validate user in database to see if already exists
  if(!isset($_SESSION['mac-cart']['loggedIn']) || $_SESSION['mac-cart']['loggedIn'] != true) {
      // display login screen if not logged in
      include 'loginform.php';
      exit();
  }
  else if(!isset($formVars['fname'])) {  // not a submit 
      // get user data if logged in and display profile page
  
      $user = new ClientUser();
      $userData = $user->getUserByUserID($_SESSION['mac-cart']['user_id']); // set data for display
      $userData['password'] = '';    // leave password blank on screen, user can change it on this screen
  }
  else {       // is a submit
      // updating user data
      // validate
      $validFields = array('fname' => 'text', 'lname' => 'text', 'password' => 'textSpecial', 'address' => 'text', 
          'city' => 'text', 'state' => 'state', 'zip' => 'zip', 'email' =>'email', 'cell_phone'=>'phone', 
          'primary_account' => 'number',
          'preferred_payment_account'=>'number', 'preferred_payment_type' => 'text', 'verified'=> 'text',
          'preferred_payment_account2'=>'number', 'preferred_payment_type2' => 'text', 'verified2'=> 'text');
      $requiredFields = array('fname', 'lname', 'email', 'cell_phone');
      $returnArray = $user->validateUserFields($formVars,$validFields, $requiredFields);
      $errorArray = $returnArray['errorArray'];  // error messages or empty array
      
      // verify bank account number if updated
      if($formVars['preferred_payment_account_update'] == '1' || $formVars['preferred_payment_account2_update'] == '1') {
          // set up bank api call
           // validate login name
        $requestData = "Request:ValidateLoginName";   // Command to service
        if ($formVars['ClientId'] == null || strlen($formVars['ClientId']) == 0) //Client Id as issued by MAC
            $errorArray[] = "Client ID required!";
        $requestData .= "|LoginName:" . $formVars['email']; // email is login id

        // 99 indicates the data is converted to a hex string (not encrypted)
        $data_before_hex = "Data=99" . strlen($formVars['ClientId']) . strtoupper($formVars['ClientId']) . 
              $requestData;

        $data = "Data=99" . strlen($formVars['ClientId']) . strtoupper($formVars['ClientId']) . 
              strToHex($requestData);
       // echo $data_before_hex;
       // echo '<br />';         
              
       // echo $data;  
       // echo '<br />';

        $mac = new MAC_Auth();
        $response = $mac->doRequest('bank', $data);
        if($response['status'] != 'success' || substr($response['response'], 0, 10) == 'Curl Error') {
            $errorArray[] = $response['response'];
        }
       // var_dump( $response);
       // echo '<br /><br />';
        $xml = array();
        if(empty($errorArray))   {   // curl call is successful
            $cleanedXml = preg_replace('/\s\s+/', ' ',$response['response']);
            $xml = new SimpleXMLElement($cleanedXml);
        //    var_dump( $xml);
            $xmlArray = xmlToArray($xml);
        // look for 'Details' == 'Success: Funds available'
            if(substr($xmlArray['Reply'], 0, 7) != 'Success')  {
               $errorArray[] = 'invalid bank login';
            }
      
            if(empty($errorArray)) {  // valid login
                // get account holder details
                $requestData = "Request:GetAccountDetails";   //Command to service
                if ($formVars['ClientId'] == null || strlen($formVars['ClientId']) == 0) //Client Id as issued by MAC
                   $errorArray[] = "Client ID required!";
                $requestData .= "|AccountHolder:" . $formVars['fname'].' '.$formVars['lname'];

                // 99 indicates the data is converted to a hex string (not encrypted)
                $data_before_hex = "Data=99" . strlen($formVars['ClientId']) . strtoupper($formVars['ClientId']) . 
                      $requestData;

                $data = "Data=99" . strlen($formVars['ClientId']) . strtoupper($formVars['ClientId']) . 
                      strToHex($requestData);
               // echo $data_before_hex;
               // echo '<br />';         
                      
               // echo $data;  
               // echo '<br />';

                $mac = new MAC_Auth();
                $response = $mac->doRequest('bank', $data);
               // var_dump($response);
               // echo "<br /><br />";
                $xml = array();
                if($response['status'] == 'success' && substr($response['response'], 0, 10) != 'Curl Error')   {
                    $cleanedXml = preg_replace('/\s\s+/', ' ',$response['response']);
                    $xml = new SimpleXMLElement($cleanedXml);
                //    var_dump( $xml);
                    $xmlArray = xmlToArray($xml);
                    if(substr($xmlArray['Reply'], 0, 7) != 'Success')  {
                       $errorArray[] = 'invalid bank login';
                    }
                    if(empty($errorArray)) {
                        // search for match on account number
                        $accountOK = false;              // initialize
                        $account2OK = false;
                        $haveValidationError = false;
                        $formVars['primary_account'] = $xmlArray['Details']['Account']['PAN'];
                        foreach($xmlArray['Details']['Account']['SubAccounts']['SubAccount'] as $account) {
                            if($formVars['preferred_payment_account_update'] == '1')  {
                                if($account['Number'] == $formVars['preferred_payment_account']) {
                                   $accountOK = true;
                                   $formVars['preferred_payment_type'] = $account['Name'];
                                   $formVars['verified'] = 'verified';
                                }
                            }
                            if($formVars['preferred_payment_account2_update'] == '1') {
                                if($account['Number'] == $formVars['preferred_payment_account2']) {
                                   $account2OK = true;
                                   $formVars['preferred_payment_type2'] = $account['Name'];
                                   $formVars['verified2'] = 'verified';
                                }
                            }
                        }
                        // if account is being verified and found no match then show as error
                        if($formVars['preferred_payment_account_update'] == '1' && $accountOK != true)  {
                           $formVars['preferred_payment_type'] = '0';
                           $formVars['verified'] = '0';
                           $accountValidationError = 'invalid account number';
                        }
                        if($formVars['preferred_payment_account2_update'] == '1' && $account2OK != true) {
                           $formVars['preferred_payment_type2'] = '0';
                           $formVars['verified2'] = '0';
                           $accountValidationError = 'invalid account number';
                        }
                    }
                } 
                else {
                    $errorArray[] = $response['response'];  // problem with cUrl call
                }
        
            }  // end of valid login
        
        
        }  // end of successful curl
        else {
            $_SESSION['mac-cart']['billing-info']['accounts']['errors'][] = $response['response'];
        }
      }  // end of verify account
 
      // update if no error messages
      if(empty($errorArray)) { 
          foreach($formVars as $field => $value) {
              if(in_array($field, array_keys($validFields)) && $formVars[$field] != '') {
                  $table[$field] = $value;
              }
          }
          $status = $user->saveUser($table, array('user_id' => $_SESSION['mac-cart']['user_id']));     //  save user data
          if($status != true)
             $errorArray[] = 'problem update profile database';
          $errorArray[] = 'profile successfully updated';   
      }
  
      $errorArray[] = $accountValidationError;   // add account validation error to array, if any
      $userData = $formVars;      // set data for display
  
  }  // end of submit
         
    // use $serData from above for form
    include_once 'i_render_profile.html.php';
    include_once 'footer_basic.html';
  
   
?> 
<script>
  $(document).ready(function() {
      // click events
      $('.item-thumb-button').on('click', function(ev) {
            var id = ev.currentTarget.id;
            macCart.addToCart(id);
      });
      
      // indicate updating account number
      $('input[name="preferred_payment_account"]').on('change', function(ev) {
            $('input[name="preferred_payment_account_update"]').val('1');
      });
      // indicate updating account number
      $('input[name="preferred_payment_account2"]').on('change', function(ev) {
            $('input[name="preferred_payment_account2_update"]').val('1');
      });

  
  });


</script>
