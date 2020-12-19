<?php

 include_once 'includes/functions.php';
 include_once 'includes/macCurlSubmitter_class.php';
 
class MacRequest {
    
    public $formVars;
    public $requestType;
    public $userType;    // 'clientManaged', or 'registered'
    
    function __construct($userType, $formVars) {
       if($userType == '')
          $this->userType = 'default';
       else    
          $this->userType = $userType;
       $this->formVars = $formVars; 
       
       // initialize error, message response array
       $this->returnStatus['status'] = null;     // true or false
       $this->returnStatus['errors'] = array();   // error messages
       $this->returnStatus['messages'] = array(); // response messages - successful transaction, etc
       $this->returnStatus['info-message'] = array();  // info messages
    }
       

    // Client Managed User - will be logged in to shopping site - may not registered at bank             
    function preAuth() {
       // preauth first
        $requestData = "Request:Preauth";   //Command to service
        if ($this->formVars['ClientId'] == null || strlen($this->formVars['ClientId']) == 0) //Client Id as issued by MAC
           $this->returnStatus['errors'][] = "Client ID required!";
        $requestData .= "|CID:" . $this->formVars['ClientId'];
        
        //  Holder plus account Name,  or subaccount, or PAN
        if(isset($this->formVars['fname']) && strlen($this->formVars['fname']) > 0
           && isset($this->formVars['lname']) && strlen($this->formVars['lname']) > 0
           && isset($this->formVars['preferred_payment_type']) && strlen($this->formVars['preferred_payment_type']) > 0) {
            $requestData .= "|AccountHolder:" .$this->formVars['fname']. ' '.$this->formVars['lname'];
            $requestData .= "|AccountName:" . $this->formVars['preferred_payment_type'];  
        }
        else if(isset($this->formVars['preferred_payment_account']) && strlen($this->formVars['preferred_payment_account']) > 0) {
            $requestData .= "|AccountNo:" .preg_replace('/[^\d]/', '', $this->formVars['preferred_payment_account']);
        }                            
        $requestData .= "|Amount:" . $this->formVars['order_total'];

        // 99 indicates the data is converted to a hex string (not encrypted)
        $data_before_hex = "Data=99" . strlen($this->formVars['ClientId']) . strtoupper($this->formVars['ClientId']) . 
              
        $data = "Data=99" . strlen($this->formVars['ClientId']) . strtoupper($this->formVars['ClientId']) . 
              strToHex($requestData);

        $mac = new MAC_Auth();
        $response = $mac->doRequest('bank', $data);
        // dump if debug is on -- url parameter '&debugrequest=do', turn off with '&debugrequest=dont'
        if (isset($_SESSION['mac-bank']['debug']) && $_SESSION['mac-bank']['debug'] == true)  {      
           var_dump('Data before hex: ',$data_before_hex);  
           var_dump('<br/>');
           var_dump('Data after hex: ', $data);
           var_dump('<br/>');
           var_dump('Response: ', $response);
        }
        $xml = array();
        $xmlArray = array();
        if(is_array($response) && substr($response['response'], 0, 5) == '<?xml') {
            $cleanedXml = preg_replace('/\s\s+/', ' ',$response['response']);
            $xml = new SimpleXMLElement($cleanedXml);
            $xmlArray = xmlToArray($xml);  
        }  
     
        if($response['status'] == 'success' && substr($response['response'], 0, 10) != 'Curl Error' && !isset($xmlArray['Error']))   {
           if(substr($xmlArray['Reply'], 0, 7) == 'Success') {
                if(strtolower(substr($xmlArray['Details'], 0, 24)) != 'funds available') {
                   $this->returnStatus['errors'][] = $xmlArray['Details'];
                }
                else  {
                    $this->returnStatus['messages'][] = $xmlArray['Details'];
                }
           }
           else {
               if(isset($xmlArray['Details']))
                  $this->returnStatus['errors'][] = substr($xmlArray['Details'], 0, 50);
           }
        }
        else if(isset($xmlArray['Error'])) {
            $this->returnStatus['errors'][] = $xmlArray['Error'];
        }              
        else {
            $this->returnStatus['errors'][] = $response['response'];
        }
 
        if(empty($this->returnStatus['errors'])) 
            $this->returnStatus['status'] = true;
        else
            $this->returnStatus['status'] = false;
        return $this->returnStatus;    
        // look for Success: Funds available
   }
  
   // request otp for otpType - 'login', or 'auth'          
   function requestOTP($otpType) {
       $requestData = "Request:SendOtp"; // Command to service
       if ($this->formVars['ClientId'] == null || strlen($this->formVars['ClientId']) == 0) //Client Id as issued by MAC
           $this->returnStatus['errors'][] = "Client ID required!";
       $requestData .= "|CID:" . $this->formVars['ClientId'];

       if (isset($this->formVars['pGroupId']) && $this->formVars['pGroupId'] != null && strlen($this->formVars['pGroupId']) != 0) // Optional if client request is restricted to a group
          $requestData .= "|GroupId:" . $this->formVars['pGroupId'];

       if($this->userType == 'clientManaged') {
           if ($this->formVars['cell_phone'] == null || strlen($this->formVars['cell_phone']) == 0)
              $this->returnStatus['errors'][] = "End User's Phone Number required!";
           $requestData .= "|PhoneNumber:" . preg_replace('/[^\d]/', '', $this->formVars['cell_phone']);
             
           if ($this->formVars['email'] == null || strlen($this->formVars['email']) == 0)
               $this->returnStatus['errors'][] = "End User's Email Address required!";
           $requestData .= "|EmailAddress:" . $this->formVars['email'];
       }
       else if($this->userType == 'registered') {
           if($this->formVars['lname'] == null)
               $this->returnStatus['errors'][] = 'Last Name required!';
           if($this->formVars['user_id'] == null)  // email
               $this->returnStatus['errors'][] = 'Email required!';
           $userIdBeforeMd5 = "|UserId:" . strtolower($this->formVars['lname']).strtolower($this->formVars['user_id']);   
           $requestData .= "|UserId:" . strtoupper(md5(strtolower($this->formVars['lname']).strtolower($this->formVars['user_id'])));   
       }
       
       if($otpType == 'login')          
           $requestData .= "|TrxType:" . '1';  // login otp
       else if($otpType == 'auth')    
           $requestData .= "|TrxType:" . '2';  // auth otp
       
       // parse and send transaction details
       $transDetail = '';
       $counter = 1;
       foreach($this->formVars as $field => $value) {
         if(substr($field, 0, 19) == 'product_description')   // avoid description, use number
             continue;
         if(substr($field, 0, 8) == 'product_') {
             if($counter % 2 == 0) {
                 $transDetail .= ' $'.$value.'|';
             }
             else {
                 $transDetail .= "$value";
             }
         $counter++;                 // count only products and order totals from array
         }
         if(substr($field, 0, 6) == 'order_') {
             if($field == 'order_product_total')
                $transDetail .= "subtotal $$value|";
             else if($field == 'order_shipping_total')   
                $transDetail .= "shipping $$value|";
             else if($field == 'order_total')   
                $transDetail .= "total $$value|";
             $counter++;
         }
       }
       $transDetail = rtrim($transDetail, '|');      // get rid of last | separator

       if ($transDetail != '')
          $requestData .= "|TrxDetails:" . strToHex($transDetail);
          
       $requestData .= "|API:PHP";  
    
        // 99 indicates the data is converted to a hex string (not encrypted)
        $data_before_hex = "Data=99" . strlen($this->formVars['ClientId']) . strtoupper($this->formVars['ClientId']) . 
              $requestData;

        $data = "Data=99" . strlen($this->formVars['ClientId']) . strtoupper($this->formVars['ClientId']) . 
              strToHex($requestData);

        $mac = new MAC_Auth();
        $response = $mac->doRequest('requestOtp', $data);
        
        // dump if debug is on -- url parameter '&debugrequest=do', turn off with '&debugrequest=dont'
        if (isset($_SESSION['mac-bank']['debug']) && $_SESSION['mac-bank']['debug'] == true)  {      
           var_dump('Data before hex: ',$data_before_hex);  
           var_dump('<br/>');
           var_dump('Data after hex: ', $data);
           var_dump('<br/>');
           var_dump('Response: ', $response);
           exit();
        }
        $xml = array();
        $xmlArray = array();
        if(is_array($response) && substr($response['response'], 0, 5) == '<?xml') {
            $cleanedXml = preg_replace('/\s\s+/', ' ',$response['response']);
            $xml = new SimpleXMLElement($cleanedXml);
            $xmlArray = xmlToArray($xml);  
        }  
     
        if($response['status'] == 'success' && substr($response['response'], 0, 10) != 'Curl Error' && !isset($xmlArray['Error']))   {
            if(substr($xmlArray['Reply'], 0, 9) == 'RequestId'){     // success
                $_SESSION['mac-bank']['otp']['RequestId'] =  $xmlArray['RequestId'];
                if($this->userType == 'clientManaged') {
                    $holdPhone = preg_replace('/[^\d]/', '', $this->formVars['cell_phone']);
                    $holdPhone = "(***) ***-**".substr($holdPhone, 8, 2);
                //    $this->returnStatus['messages'][] = 'enter the TCode texted to '.$holdPhone.' &nbsp;(<em>code valid for 3 minutes</em>)';
                    $this->returnStatus['messages'][] = 'Enter the Authentication Code sent to your mobile phone.';
                }
                else if($this->userType == 'registered') {
                //    $this->returnStatus['messages'][] = 'enter the TCode texted to your registered cellphone &nbsp;(<em>code valid for 3 minutes</em>)';
                	$this->returnStatus['messages'][] = 'Enter the Authentication Code sent to your mobile phone.';
                }
                if(isset($xmlArray['EnterOTPAd'])) {
                    $ad = hexToStr($xmsArray['EnterOTPAd']);
                }
                else if(($adPos = strpos($xmlArray['Reply'], '|EnterOTPAd:')) > 0) {
                    $ad = hexToStr(substr($xmlArray['Reply'], $adPos + 4));
                }
                $_SESSION['mac-bank']['otp']['ad'] = $ad;    
            }    
            else { 
                $this->returnStatus['errors'][] = strip_tags(substr($response['response'], 0, 50));
            }
        }
        else if(isset($xmlArray['Error'])) {
            $this->returnStatus['errors'][] = $xmlArray['Error'];
        }              
        else {
            $this->returnStatus['errors'][] = strip_tags($response['response']);
        }
        
        if(empty($this->returnStatus['errors'])) 
            $this->returnStatus['status'] = true;
        else
            $this->returnStatus['status'] = false;
        return $this->returnStatus;    
       // var_dump( $response);
       // echo '<br /><br />';
   }
    
    // resend otp          
   function resendOTP($requestId) {
       $requestData = "Request:ResendOtp"; // Command to service
       if ($this->formVars['ClientId'] == null || strlen($this->formVars['ClientId']) == 0) //Client Id as issued by MAC
           $this->returnStatus['errors'][] = "Client ID required!";
       $requestData .= "|CID:" . $this->formVars['ClientId'];

       if (!isset($requestId) || $requestId == null)
           $this->returnStatus['errors'][] = 'Request Id from OTP is missing!';
       $requestData .= "|RequestId:" . $requestId;   
      
       $requestData .= "|API:PHP";   
       
        // 99 indicates the data is converted to a hex string (not encrypted)
        $data_before_hex = "Data=99" . strlen($this->formVars['ClientId']) . strtoupper($this->formVars['ClientId']) . 
              $requestData;

        $data = "Data=99" . strlen($this->formVars['ClientId']) . strtoupper($this->formVars['ClientId']) . 
              strToHex($requestData);

        $mac = new MAC_Auth();
        $response = $mac->doRequest('requestOtp', $data);
        // dump if debug is on -- url parameter '&debugrequest=do', turn off with '&debugrequest=dont'
        if (isset($_SESSION['mac-bank']['debug']) && $_SESSION['mac-bank']['debug'] == true)  {      
           var_dump('Data before hex: ',$data_before_hex);  
           var_dump('<br/>');
           var_dump('Data after hex: ', $data);
           var_dump('<br/>');
           var_dump('Response: ', $response);
           exit();
        }
        $xml = array();
        $xmlArray = array();
        if(is_array($response) && substr($response['response'], 0, 5) == '<?xml') {
            $cleanedXml = preg_replace('/\s\s+/', ' ',$response['response']);
            $xml = new SimpleXMLElement($cleanedXml);
            $xmlArray = xmlToArray($xml);  
        }  
        if($response['status'] == 'success' && substr($response['response'], 0, 10) != 'Curl Error' && !isset($xmlArray['Error']))   {
           if(substr($xmlArray['Reply'], 0, 6) != 'Resent') {
           		// DO NOT DELETE! The code below displays information in error format for development purposes.
                //$this->returnStatus['errors'][] = $xmlArray['Details'];
                
           		// This code is in place temporarily to display successful resent message.
           		$this->returnStatus['messages'][] = 'Authentication Code resent.';
            }
            else  {
                $_SESSION['mac-bank']['otp']['RequestId'] =  $xmlArray['RequestId'];
                if($this->userType == 'clientManaged') {
                    $holdPhone = preg_replace('/[^\d]/', '', $this->returnStatus['cell_phone']);
                    $holdPhone = "(***) ***-**".substr($holdPhone, 8, 2);
                    $this->returnStatus['messages'][] = 'enter the TCode texted to '.$holdPhone.' &nbsp;(<em>code valid for 3 minutes</em>)';
                }
                else if($this->userType == 'registered') {
                    $this->returnStatus['messages'][] = 'enter the TCode texted to your registered cellphone &nbsp;(<em>code valid for 3 minutes</em>)';
                }
            }
        }
        else if(isset($xmlArray['Error'])) {
            if(strpos($xmlArray['Error'], 'Resend OTP Disabled') > 0)
                $xmlArray['Error'] = $xmlArray['Error'] . '-> Please restart your transaction.';
            $this->returnStatus['errors'][] = $xmlArray['Error'];
        }              
        else {
            $this->returnStatus['errors'][] = $response['response'];
        }
        
        if(empty($this->returnStatus['errors'])) 
            $this->returnStatus['status'] = true;
        else
            $this->returnStatus['status'] = false;
        return $this->returnStatus;    
    }  
                     
   // validate the user entered otp, need the saved requestId from the requestOTP() call to match up for validation.   
   function validateOTP($requestId) {
       $requestData = "Request:VerifyOtp"; // Command to service
       if ($this->formVars['ClientId'] == null || strlen($this->formVars['ClientId']) == 0) //Client Id as issued by MAC
           $this->returnStatus['errors'][] = "Client ID required!";
       $requestData .= "|CID:" . $this->formVars['ClientId'];
       
       if (!isset($requestId) || $requestId == null)
           $this->returnStatus['errors'][] = 'Request Id from OTP is missing';
       $requestData .= "|RequestId:" . $requestId;   
          
       if (!isset($this->formVars['otpCode']) || $this->formVars['otpCode'] == '')
           $this->returnStatus['errors'][] = 'Please enter OTP code';
       $requestData .= "|OTP:" . $this->formVars['otpCode'];
       
       // $requestData .= "|EndUserIpAddress:72.215.204.74";
       
       $requestData .= "|API:PHP";   
                     
        // 99 indicates the data is converted to a hex string (not encrypted)
        $data_before_hex = "Data=99" . strlen($this->formVars['ClientId']) . $this->formVars['ClientId'] . 
              strtoupper($requestData);

        $data = "Data=99" . strlen($this->formVars['ClientId']) . strtoupper($this->formVars['ClientId']) . 
              strToHex($requestData);

        $mac = new MAC_Auth();
        $response = $mac->doRequest('verifyOtp', $data);
        // dump if debug is on -- url parameter '&debugrequest=do', turn off with '&debugrequest=dont'
        if (isset($_SESSION['mac-bank']['debug']) && $_SESSION['mac-bank']['debug'] == true)  {      
           var_dump('Data before hex: ',$data_before_hex);  
           var_dump('<br/>');
           var_dump('Data after hex: ', $data);
           var_dump('<br/>');
           var_dump('Response: ', $response);
           exit();
        }
        $xml = array();
        $xmlArray = array();
        if(is_array($response) && substr($response['response'], 0, 5) == '<?xml') {
            $cleanedXml = preg_replace('/\s\s+/', ' ',$response['response']);
            $xml = new SimpleXMLElement($cleanedXml);
            $xmlArray = xmlToArray($xml);  
        }  
     
        if($response['status'] == 'success' && substr($response['response'], 0, 10) != 'Curl Error' && !isset($xmlArray['Error']))   {
            if($xmlArray['Reply'] == 'Validated') {           //  success ???
                $this->returnStatus['messages'][] = substr($xmlArray['Details'], 0, 70);
            }
            else { 
                if(isset($xmlArray['Details'])) {
                    $this->returnStatus['errors'][] = $xmlArray['Reply'];
                    $this->returnStatus['errors'][] = str_replace('|', ' ', substr($xmlArray['Details'], 0, 140));    
                }
                else if(isset($xmlArray['Error'])) 
                    $this->returnStatus['errors'][] = $xmlArray['Error'];
                else     
                    $this->returnStatus['errors'][] = str_replace('|', ' ', strip_tags(substr($response['response'], 0, 140)));
            }
        }
        else if(isset($xmlArray['Error'])) {
            $this->returnStatus['errors'][] = $xmlArray['Error'];
        }              
        else {
            $this->returnStatus['errors'][] = strip_tags($response['response']);
        }
        
        if(empty($this->returnStatus['errors'])) 
            $this->returnStatus['status'] = true;
        else
            $this->returnStatus['status'] = false;
        return $this->returnStatus;    
       // var_dump( $response);
       // echo '<br /><br />';
   }
     
         // get account holder details
   function getAccountDetails() {      
        $requestData = "Request:GetAccountDetails";   //Command to service
        if ($this->formVars['ClientId'] == null || strlen($this->formVars['ClientId']) == 0) //Client Id as issued by MAC
           $this->returnStatus['errors'][] = "Client ID required!";
        $requestData .= "|AccountHolder:" . $this->formVars['fname'].' '.$this->formVars['lname'];

        // 99 indicates the data is converted to a hex string (not encrypted)
        $data_before_hex = "Data=99" . strlen($this->formVars['ClientId']) . strtoupper($this->formVars['ClientId']) . 
              $requestData;

        $data = "Data=99" . strlen($this->formVars['ClientId']) . strtoupper($this->formVars['ClientId']) . 
              strToHex($requestData);

        $mac = new MAC_Auth();
        $response = $mac->doRequest('bank', $data);
        // dump if debug is on -- url parameter '&debugrequest=do', turn off with '&debugrequest=dont'
        if (isset($_SESSION['mac-bank']['debug']) && $_SESSION['mac-bank']['debug'] == true)  {      
           var_dump('Data before hex: ',$data_before_hex);  
           var_dump('<br/>');
           var_dump('Data after hex: ', $data);
           var_dump('<br/>');
           var_dump('Response: ', $response);
        }
        $xml = array();
        $xmlArray = array();
        if(is_array($response) && substr($response['response'], 0, 5) == '<?xml') {
            $cleanedXml = preg_replace('/\s\s+/', ' ',$response['response']);
            $xml = new SimpleXMLElement($cleanedXml);
            $xmlArray = xmlToArray($xml);  
        }  
     
        if($response['status'] == 'success' && substr($response['response'], 0, 10) != 'Curl Error' && !isset($xmlArray['Error']))   {
           if(substr($xmlArray['Reply'], 0, 7) != 'Success') {
                $this->returnStatus['errors'][] = $xmlArray['Details'];
            }
            else if(!is_array($xmlArray['Details']) && substr($xmlArray['Details'], 0, 8) != 'Success:') {
                $this->returnStatus['errors'][] = $xmlArray['Details'];
            }
            else {
                $this->returnStatus['messages'][] = 'Success';
                $this->returnStatus['accounts'] = $xmlArray['Details'];
            }                  
        }
        else if(isset($xmlArray['Error'])) {
            $this->returnStatus['errors'][] = $xmlArray['Error'];
        }              
        else {
            $this->returnStatus['errors'][] = $response['response'];
        }
      
        if(empty($this->returnStatus['errors'])) 
            $this->returnStatus['status'] = true;
        else
            $this->returnStatus['status'] = false;
        return $this->returnStatus;    
   }
        
    // Registered User - does not need to be logged in to shopping site - must be registered at bank
    function validateLogin() {
        // validage login name
        $requestData = "Request:ValidateLoginName";   //Command to service
        if ($this->formVars['ClientId'] == null || strlen($this->formVars['ClientId']) == 0) //Client Id as issued by MAC
            $this->returnStatus['errors'][] = "Client ID required!";
        $requestData .= "|LoginName:" . $this->formVars['user_id'];

        // 99 indicates the data is converted to a hex string (not encrypted)
        $data_before_hex = "Data=99" . strlen($this->formVars['ClientId']) . strtoupper($this->formVars['ClientId']) . 
              $requestData;

        $data = "Data=99" . strlen($this->formVars['ClientId']) . strtoupper($this->formVars['ClientId']) . 
              strToHex($requestData);

        $mac = new MAC_Auth();
        $response = $mac->doRequest('bank', $data);
        // dump if debug is on -- url parameter '&debugrequest=do', turn off with '&debugrequest=dont'
        if (isset($_SESSION['mac-bank']['debug']) && $_SESSION['mac-bank']['debug'] == true)  {      
           var_dump('Data before hex: ',$data_before_hex);  
           var_dump('<br/>');
           var_dump('Data after hex: ', $data);
           var_dump('<br/>');
           var_dump('Response: ', $response);
        }
        $xml = array();
        $xmlArray = array();
        if(is_array($response) && substr($response['response'], 0, 5) == '<?xml') {
            $cleanedXml = preg_replace('/\s\s+/', ' ',$response['response']);
            $xml = new SimpleXMLElement($cleanedXml);
            $xmlArray = xmlToArray($xml);  
        }  
     
        if($response['status'] == 'success' && substr($response['response'], 0, 10) != 'Curl Error' && !isset($xmlArray['Error']))   {
           if(substr($xmlArray['Reply'], 0, 7) != 'Success') {
                $this->returnStatus['errors'][] = $xmlArray['Details'];
            }
            else  {
                $holdReply = explode('|', $xmlArray['Details']);
                $this->returnStatus['pan'] = $holdReply[0];
                $holdName = explode(' ', $holdReply[1]);
                $this->returnStatus['fname'] = $holdName[0];
                $this->returnStatus['lname'] = $holdName[1];
                $this->returnStatus['messages'][] = $xmlArray['Details'];
            }
        }
        else if(isset($xmlArray['Error'])) {
            $this->returnStatus['errors'][] = $xmlArray['Error'];
        }              
        else {
            $this->returnStatus['errors'][] = $response['response'];
        }
        
        if(empty($this->returnStatus['errors'])) 
            $this->returnStatus['status'] = true;
        else
            $this->returnStatus['status'] = false;
        return $this->returnStatus;    
    }  
                
         
            
    // account details
    function getAccountDetailsRegistered() {        
        $requestData = "Request:GetAccountDetails";   //Command to service
        if ($this->formVars['ClientId'] != null || strlen($this->formVars['ClientId']) > 0)  //Client Id as issued by MAC
           $clientId =  $this->formVars['ClientId'];
        else     
           $this->returnStatus['errors'][] = "Client ID required!";

        if(empty($_SESSION['mac-bank']['billing']['errors']))
           $requestData .= "|ClientId:" . $clientId;
           
        $requestData .= "|AccountHolder:" . $this->formVars['fname'].' '.$this->formVars['lname'];

        // 99 indicates the data is converted to a hex string (not encrypted)
        $data_before_hex = "Data=99" . strlen($cientId) . strtoupper($clientId) . 
           $requestData;

        $data = "Data=99" . strlen($clientId) . strtoupper($clientId) . 
           strToHex($requestData);

        $mac = new MAC_Auth();
        $response = $mac->doRequest('bank', $data);
        // dump if debug is on -- url paramete lEtsdEbUg=on, turn off lEtsdEbUg=off
        if (isset($_SESSION['mac-bank']['debug']) && $_SESSION['mac-bank']['debug'] == true)  {      
           var_dump('Data before hex: ',$data_before_hex);  
           var_dump('<br/>');
           var_dump('Data after hex: ', $data);
           var_dump('<br/>');
           var_dump('Response: ', $response);
        }
        $xml = array();
        $xmlArray = array();
        if(is_array($response) && substr($response['response'], 0, 5) == '<?xml') {
            $cleanedXml = preg_replace('/\s\s+/', ' ',$response['response']);
            $xml = new SimpleXMLElement($cleanedXml);
            $xmlArray = xmlToArray($xml);  
        }  
     
        if($response['status'] == 'success' && substr($response['response'], 0, 10) != 'Curl Error' && !isset($xmlArray['Error']))   {
           if(substr($xmlArray['Reply'], 0, 7) != 'Success') {
                $this->returnStatus['errors'][] = $xmlArray['Details'];
            }
            else if(!is_array($xmlArray['Details']) && substr($xmlArray['Details'], 0, 8) != 'Success:') {
                $this->returnStatus['errors'][] = $xmlArray['Details'];
            }
            else {
                $this->returnStatus['messages'][] = 'Success';
                $this->returnStatus['accounts'] = $xmlArray['Details'];
            }                  
        }
        else if(isset($xmlArray['Error'])) {
            $this->returnStatus['errors'][] = $xmlArray['Error'];
        }              
        else {
            $this->returnStatus['errors'][] = $response['response'];
        }
        
        if(empty($this->returnStatus['errors'])) 
            $this->returnStatus['status'] = true;
        else
            $this->returnStatus['status'] = false;
        return $this->returnStatus;    
    }
        
    function depositWithdraw($transaction) {        
        // set transaction type
       if($transaction == 'deposit')
          $requestData = "Request:CreditAccount";   // Command to service
       else if($transaction == 'withdraw') 
          $requestData = "Request:DebitAccount";
             
       if ($this->formVars['ClientId'] == null || strlen($this->formVars['ClientId']) == 0) //Client Id as issued by MAC
          $this->returnStatus['errors'][] = "Client ID required!";
       $requestData .= "|CID:" . $this->formVars['ClientId'];

       $requestData .= "|AccountHolder:" . $this->formVars['fname'].' '.$this->formVars['lname'];
       $requestData .= "|AccountName:" . $this->formVars['account_affected'];    
       $requestData .= "|Amount:" . preg_replace("/[^0-9\.]/", "", $this->formVars['transfer_amount']);
           // 99 indicates the data is converted to a hex string (not encrypted)
        $data_before_hex = "Data=99" . strlen($this->formVars['ClientId']) . strtoupper($this->formVars['ClientId']) . 
              $requestData;
              
        $data = "Data=99" . strlen($this->formVars['ClientId']) . strtoupper($this->formVars['ClientId']) . 
              strToHex($requestData);

        $mac = new MAC_Auth();
        $response = $mac->doRequest('bank', $data);
        // dump if debug is on -- url paramete lEtsdEbUg=on, turn off lEtsdEbUg=off
        if (isset($_SESSION['mac-bank']['debug']) && $_SESSION['mac-bank']['debug'] == true)  {      
           var_dump('Data before hex: ',$data_before_hex);  
           var_dump('<br/>');
           var_dump('Data after hex: ', $data);
           var_dump('<br/>');
           var_dump('Response: ', $response);
        }
        $xml = array();
        $xmlArray = array();
        if(is_array($response) && substr($response['response'], 0, 5) == '<?xml') {
            $cleanedXml = preg_replace('/\s\s+/', ' ',$response['response']);
            $xml = new SimpleXMLElement($cleanedXml);
            $xmlArray = xmlToArray($xml);  
        }  
        
        if($response['status'] == 'success' && substr($response['response'], 0, 10) != 'Curl Error'  && !isset($xmlArray['Error']))   {
            $cleanedXml = preg_replace('/\s\s+/', ' ',$response['response']);
            $xml = new SimpleXMLElement($cleanedXml);
        //    var_dump( $xml);
            $xmlArray = xmlToArray($xml);
            if(substr($xmlArray['Reply'], 0, 7) == 'Success') {
                $this->returnStatus['messages'][] = 'Success';
                $this->returnStatus['messages'][] = $xmlArray['Details'];
            }
            else
                 $this->returnStatus['errors'][] = $xmlArray['Details'];
        } 
        else if(isset($xmlArray['Error'])) {
            $this->returnStatus['errors'][] = $xmlArray['Error'];
        }              
        else {
            $this->returnStatus['errors'][] = $response['response'];
        }
        
        if(empty($this->returnStatus['errors'])) 
            $this->returnStatus['status'] = true;
        else
            $this->returnStatus['status'] = false;
        return $this->returnStatus;    
    }     
    
    
    function moveFunds() {        
        // move funds
       $requestData = "Request:MoveFunds";   //Command to service
       if ($this->formVars['ClientId'] == null || strlen($this->formVars['ClientId']) == 0) //Client Id as issued by MAC
          $this->returnStatus['errors'][] = "Client ID required!";
       $requestData .= "|CID:" . $this->formVars['ClientId'];

       if(isset($this->formVars['account_from']) && $this->formVars['account_from'] != '') {
          $requestData .= "|FromAccountHolder:" . $this->formVars['fname'].' '.$this->formVars['lname']; 
          $requestData .= "|FromAccountName:" . $this->formVars['account_from'];  // name of account type  
       }
       //  ?? may need account numbe ??
       $useAccountToNumber = $this->formVars['account_to_number'];     // set up account to number
       $tempAccountToNumber = preg_replace('/[^\d]/', '', $this->formVars['account_to_number']);
       if(is_number($tempAccountToNumber) && strlen($tempAccountToNumber) == 16)    // if account Number use this one
          $useAccountToNumber = $tempAccountToNumber;          // if its an account Number use this one, else its an account Name
       if(isset($this->formVars['account_to_name']) && $this->formVars['account_to_name'] != '') {
          $requestData .= "|ToAccountHolder:" . trim($this->formVars['account_to_name']);
          $requestData .= "|ToAccountName:" . $useAccountToNumber;    
       }
       $requestData .= "|Amount:" . preg_replace("/[^0-9\.]/", "", $this->formVars['transfer_amount']);

        // 99 indicates the data is converted to a hex string (not encrypted)
        $data_before_hex = "Data=99" . strlen($this->formVars['ClientId']) . strtoupper($this->formVars['ClientId']) . 
              $requestData;
              
        $data = "Data=99" . strlen($this->formVars['ClientId']) . strtoupper($this->formVars['ClientId']) . 
              strToHex($requestData);

        $mac = new MAC_Auth();
        $response = $mac->doRequest('bank', $data);
        // dump if debug is on -- url paramete lEtsdEbUg=on, turn off lEtsdEbUg=off
        if (isset($_SESSION['mac-bank']['debug']) && $_SESSION['mac-bank']['debug'] == true)  {      
           var_dump('Data before hex: ',$data_before_hex);  
           var_dump('<br/>');
           var_dump('Data after hex: ', $data);
           var_dump('<br/>');
           var_dump('Response: ', $response);
        }
        $xml = array();
        $xmlArray = array();
        if(is_array($response) && substr($response['response'], 0, 5) == '<?xml') {
            $cleanedXml = preg_replace('/\s\s+/', ' ',$response['response']);
            $xml = new SimpleXMLElement($cleanedXml);
            $xmlArray = xmlToArray($xml);  
        }  
        
        if($response['status'] == 'success' && substr($response['response'], 0, 10) != 'Curl Error'  && !isset($xmlArray['Error']))   {
            $cleanedXml = preg_replace('/\s\s+/', ' ',$response['response']);
            $xml = new SimpleXMLElement($cleanedXml);
        //    var_dump( $xml);
            $xmlArray = xmlToArray($xml);
            if(substr($xmlArray['Reply'], 0, 7) == 'Success') {
                $this->returnStatus['messages'][] = 'Success';
                $this->returnStatus['messages'][] = $xmlArray['Details'];
            }
            else
                 $this->returnStatus['errors'][] = $xmlArray['Details'];
        } 
        else if(isset($xmlArray['Error'])) {
            $this->returnStatus['errors'][] = $xmlArray['Error'];
        }              
        else {
            $this->returnStatus['errors'][] = $response['response'];
        }
        
        if(empty($this->returnStatus['errors'])) 
            $this->returnStatus['status'] = true;
        else
            $this->returnStatus['status'] = false;
        return $this->returnStatus;    
    } 

    
    
 
    function purchase() {
         // move funds
       $requestData = "Request:Purchase";   //Command to service
       if ($this->formVars['ClientId'] == null || strlen($this->formVars['ClientId']) == 0) //Client Id as issued by MAC
           $this->returnStatus['errors'][] = "Client ID required!";
       $requestData .= "|CID:" . $this->formVars['ClientId'];
       $requestData .= "|AccountHolder:" . $this->formVars['fname'].' '.$this->formVars['lname'];

       if(isset($this->formVars['preferred_payment_type']) && $this->formVars['preferred_payment_type'] != '') {
          $requestData .= "|AccountName:" . $this->formVars['preferred_payment_type'];    
       }
       else if(isset($this->formVars['preferred_payment_account']) && $this->formVars['preferred_payment_account'] != '') {
            $requestData .= "|CardNumber:" .preg_replace('/[^\d]/', '', $this->formVars['preferred_payment_account']);
       }
       $requestData .= "|MerchantName:" . "Kohl's";
       $requestData .= "|Amount:" . $this->formVars['order_total'];

       // parse and send transaction details
       $transDetail = '';
       $counter = 1;
       foreach($this->formVars as $field => $value) {
         if(substr($field, 0, 8) == 'product_') {
             $transDetail .= " $value";
             $remainder = $counter % 3;
             if($remainder == 0) {          // if on 3rd item we are done with a product
                 $transDetail .= '|';
             }
             else if($remainder % 2 == 1) {  // if on next after 3rd then add a comma
                 $transDetail .= ',';
             }
             $counter++;                 // count only products and order totals from array
         }
         if(substr($field, 0, 6) == 'order_') {
             $transDetail .= substr($field, 6). " $value|"; 
             $counter++;
         }
       }
       $transDetail = rtrim($transDetail, '|');      // get rid of last | separator

       if ($transDetail != '')
          $requestData .= "|TrxDetails:" . strToHex($transDetail);

        // 99 indicates the data is converted to a hex string (not encrypted)
        $data_before_hex = "Data=99" . strlen($this->formVars['ClientId']) . strtoupper($this->formVars['ClientId']) . 
              $requestData;
              
        $data = "Data=99" . strlen($this->formVars['ClientId']) . strtoupper($this->formVars['ClientId']) . 
              strToHex($requestData);

        $mac = new MAC_Auth();
        $response = $mac->doRequest('bank', $data);
        // dump if debug is on -- url paramete lEtsdEbUg=on, turn off lEtsdEbUg=off
        if (isset($_SESSION['mac-bank']['debug']) && $_SESSION['mac-bank']['debug'] == true)  {      
           var_dump('Data before hex: ',$data_before_hex);  
           var_dump('<br/>');
           var_dump('Data after hex: ', $data);
           var_dump('<br/>');
           var_dump('Response: ', $response);
        }
        $xml = array();
        $xmlArray = array();
        if(is_array($response) && substr($response['response'], 0, 5) == '<?xml') {
            $cleanedXml = preg_replace('/\s\s+/', ' ',$response['response']);
            $xml = new SimpleXMLElement($cleanedXml);
            $xmlArray = xmlToArray($xml);  
        }  
        
        if($response['status'] == 'success' && substr($response['response'], 0, 10) != 'Curl Error'  && !isset($xmlArray['Error']))   {
            if(substr($xmlArray['Reply'], 0, 7) == 'Success') {
                $this->returnStatus['messages'][] = 'Success';
                $this->returnStatus['messages'][] = $xmlArray['Details']['Transaction'];
            }
            else
                 $this->returnStatus['errors'][] = $xmlArray['Details'];
        } 
        else if(isset($xmlArray['Error'])) {
            $this->returnStatus['errors'][] = $xmlArray['Error'];
        }              
        else {
            $this->returnStatus['errors'][] = $response['response'];
        }
      
        if(empty($this->returnStatus['errors'])) 
            $this->returnStatus['status'] = true;
        else
            $this->returnStatus['status'] = false;
        return $this->returnStatus;    
    } 
 
     function getBills() {
         // move funds
       $requestData = "Request:GetBills";   //Command to service
       if ($this->formVars['ClientId'] == null || strlen($this->formVars['ClientId']) == 0) //Client Id as issued by MAC
           $this->returnStatus['errors'][] = "Client ID required!";
       $requestData .= "|CID:" . $this->formVars['ClientId'];
       $requestData .= "|AccountHolder:" . $this->formVars['fname'].' '.$this->formVars['lname'];

        // 99 indicates the data is converted to a hex string (not encrypted)
        $data_before_hex = "Data=99" . strlen($this->formVars['ClientId']) . strtoupper($this->formVars['ClientId']) . 
              $requestData;
              
        $data = "Data=99" . strlen($this->formVars['ClientId']) . strtoupper($this->formVars['ClientId']) . 
              strToHex($requestData);

        $mac = new MAC_Auth();
        $response = $mac->doRequest('bank', $data);
        // dump if debug is on -- url paramete lEtsdEbUg=on, turn off lEtsdEbUg=off
        if (isset($_SESSION['mac-bank']['debug']) && $_SESSION['mac-bank']['debug'] == true)  {      
           var_dump('Data before hex: ',$data_before_hex);  
           var_dump('<br/>');
           var_dump('Data after hex: ', $data);
           var_dump('<br/>');
           var_dump('Response: ', $response);
        }
        $xml = array();
        $xmlArray = array();
        if(is_array($response) && substr($response['response'], 0, 5) == '<?xml') {
            $cleanedXml = preg_replace('/\s\s+/', ' ',$response['response']);
            $xml = new SimpleXMLElement($cleanedXml);
            $xmlArray = xmlToArray($xml);  
        }  
        
        if($response['status'] == 'success' && substr($response['response'], 0, 10) != 'Curl Error'  && !isset($xmlArray['Error']))   {
           if(substr($xmlArray['Reply'], 0, 7) != 'Success') {
                $this->returnStatus['errors'][] = $xmlArray['Details'];
            }
            else if(is_array($xmlArray['Account']['Bills']) && is_array($xmlArray['Account']['Bills'])) {
                 
                /*      //  test data
                $xmlArray['Account']['Bills']= array();
                $xmlArray['Account']['Bills'][] = array('InvoiceNumber'=>'10','Status'=>'Due','BusinessType'=>'Utility','Name'=>'ComEd',          'BillingDate'=>'8/14/2014 12:01 AM','AmountDue'=>'$1,200','DueDate'=>'8/16/2014 12:01 AM');
                $xmlArray['Account']['Bills'][] = array('InvoiceNumber'=>'11','Status'=>'Due','BusinessType'=>'Utility','Name'=>'North Shore Gas','BillingDate'=>'8/14/2014 12:01 AM','AmountDue'=>'$135.00','DueDate'=>'8/16/2014 12:01 AM');
                $xmlArray['Account']['Bills'][] = array('InvoiceNumber'=>'12','Status'=>'Due','BusinessType'=>'Utility','Name'=>'Verizon',        'BillingDate'=>'8/14/2014 12:01 AM','AmountDue'=>'$189.00','DueDate'=>'8/16/2014 12:01 AM');
                */   // end test
                                
                $this->returnStatus['accounts'] = $xmlArray['Account'];
            }
        } 
        else if(isset($xmlArray['Error'])) {
            $this->returnStatus['errors'][] = $xmlArray['Error'];
        }              
        else {
            $this->returnStatus['errors'][] = $response['response'];
        }
      
        if(empty($this->returnStatus['errors'])) 
            $this->returnStatus['status'] = true;
        else
            $this->returnStatus['status'] = false;
        return $this->returnStatus;    
    } 
 
     function payBill() {
         // move funds
       $requestData = "Request:PayBill";   //Command to service
       if ($this->formVars['ClientId'] == null || strlen($this->formVars['ClientId']) == 0) //Client Id as issued by MAC
           $this->returnStatus['errors'][] = "Client ID required!";
       $requestData .= "|CID:" . $this->formVars['ClientId'];
       $requestData .= "|InvoiceNumber:" . $this->formVars['invoice'];
       $requestData .= "|AccountHolder:" . $this->formVars['fname'].' '.$this->formVars['lname'];
       $requestData .= "|AccountName:" . $this->formVars['accountName'];
       $requestData .= "|Amount:" . preg_replace("/[^0-9\.]/", "", $this->formVars['payAmount']);

        // 99 indicates the data is converted to a hex string (not encrypted)
        $data_before_hex = "Data=99" . strlen($this->formVars['ClientId']) . strtoupper($this->formVars['ClientId']) . 
              $requestData;
              
        $data = "Data=99" . strlen($this->formVars['ClientId']) . strtoupper($this->formVars['ClientId']) . 
              strToHex($requestData);

        $mac = new MAC_Auth();
        $response = $mac->doRequest('bank', $data);
        // dump if debug is on -- url paramete lEtsdEbUg=on, turn off lEtsdEbUg=off
        if (isset($_SESSION['mac-bank']['debug']) && $_SESSION['mac-bank']['debug'] == true)  {      
           var_dump('Data before hex: ',$data_before_hex);  
           var_dump('<br/>');
           var_dump('Data after hex: ', $data);
           var_dump('<br/>');
           var_dump('Response: ', $response);
        }
        $xml = array();
        $xmlArray = array();
        if(is_array($response) && substr($response['response'], 0, 5) == '<?xml') {
            $cleanedXml = preg_replace('/\s\s+/', ' ',$response['response']);
            $xml = new SimpleXMLElement($cleanedXml);
            $xmlArray = xmlToArray($xml);  
        }  
        
        if($response['status'] == 'success' && substr($response['response'], 0, 10) != 'Curl Error'  && !isset($xmlArray['Error']))   {
           if(substr($xmlArray['Reply'], 0, 7) != 'Success') {
                $this->returnStatus['errors'][] = $xmlArray['Details'];
            }
            else {
                $this->returnStatus['messages'][] = 'Success';
                $this->returnStatus['messages'][] = $xmlArray['Details'];
            }
        } 
        else if(isset($xmlArray['Error'])) {
            $this->returnStatus['errors'][] = $xmlArray['Error'];
        }              
        else {
            $this->returnStatus['errors'][] = $response['response'];
        }
      
        if(empty($this->returnStatus['errors'])) 
            $this->returnStatus['status'] = true;
        else
            $this->returnStatus['status'] = false;
        return $this->returnStatus;    
    } 
    
         // get account holder details
   function getAllAccounts() {      
        $requestData = "Request:GetBankStatus";   //Command to service
        if ($this->formVars['ClientId'] == null || strlen($this->formVars['ClientId']) == 0) //Client Id as issued by MAC
           $this->returnStatus['errors'][] = "Client ID required!";

        // 99 indicates the data is converted to a hex string (not encrypted)
        $data_before_hex = "Data=99" . strlen($this->formVars['ClientId']) . strtoupper($this->formVars['ClientId']) . 
              $requestData;

        $data = "Data=99" . strlen($this->formVars['ClientId']) . strtoupper($this->formVars['ClientId']) . 
              strToHex($requestData);

        $mac = new MAC_Auth();
        $response = $mac->doRequest('bank', $data);
        // dump if debug is on -- url paramete lEtsdEbUg=on, turn off lEtsdEbUg=off
        if (isset($_SESSION['mac-bank']['debug']) && $_SESSION['mac-bank']['debug'] == true)  {      
           var_dump('Data before hex: ',$data_before_hex);  
           var_dump('<br/>');
           var_dump('Data after hex: ', $data);
           var_dump('<br/>');
           var_dump('Response: ', $response);
        }
        $xml = array();
        $xmlArray = array();
        if(is_array($response) && substr($response['response'], 0, 5) == '<?xml') {
            $cleanedXml = preg_replace('/\s\s+/', ' ',$response['response']);
            $xml = new SimpleXMLElement($cleanedXml);
            $xmlArray = xmlToArray($xml);  
        }  
     
        if($response['status'] == 'success' && substr($response['response'], 0, 10) != 'Curl Error' && !isset($xmlArray['Error']))   {
           if(substr($xmlArray['Reply'], 0, 7) != 'Success') {
                $this->returnStatus['errors'][] = $xmlArray['Details'];
            }
            else {
                $this->returnStatus['messages'][] = 'Success';
                $this->returnStatus['response'] = $xmlArray;
            }                  
        }
        else if(isset($xmlArray['Error'])) {
            $this->returnStatus['errors'][] = $xmlArray['Error'];
        }              
        else {
            $this->returnStatus['errors'][] = $response['response'];
        }
      
        if(empty($this->returnStatus['errors'])) 
            $this->returnStatus['status'] = true;
        else
            $this->returnStatus['status'] = false;
        return $this->returnStatus;    
   }
    
   // get clientId from server for the ClientName
   function getServerClientId($defaultClientId=null, $clientName=null ) {      
        if($defaultClientId != null)   {
            $useClientId = $defaultClientId;
        }
        else{
            $useClientId = "530f6e8e675c9b1854a6970b";     
        }
        if($clientName != null) {
            $useClientName = $clientName;
        }
        else {
            $useClientName = "MAC Test Bank";
        }
        $requestData = "Request:GetClientId";   //Command to service
        $requestData .= "|CID:$useClientId";
        $requestData .= "|ClientName:$useClientName";

        // 99 indicates the data is converted to a hex string (not encrypted)
        $data_before_hex = "Data=99" . strlen($useClientId) . strtoupper($useClientId) . 
              $requestData;

        $data = "Data=99" . strlen($useClientId) . strtoupper($useClientId) . 
              strToHex($requestData);

        $mac = new MAC_Auth();
        $response = $mac->doRequest('bank', $data);
        // dump if debug is on -- url parameter '&debugrequest=do', turn off with '&debugrequest=dont'
        if (isset($_SESSION['mac-cart']['debug']) && $_SESSION['mac-cart']['debug'] == true)  {      
           var_dump('Data before hex: ',$data_before_hex);  
           var_dump('<br/>');
           var_dump('Data after hex: ', $data);
           var_dump('<br/>');
           var_dump('Response: ', $response);
           exit();
        }
        $xml = array();
        $xmlArray = array();
        if(is_array($response) && substr($response['response'], 0, 5) == '<?xml') {
            $cleanedXml = preg_replace('/\s\s+/', ' ',$response['response']);
            $xml = new SimpleXMLElement($cleanedXml);
            $xmlArray = xmlToArray($xml);  
        }  
     
        if($response['status'] == 'success' && substr($response['response'], 0, 10) != 'Curl Error' && !isset($xmlArray['Error']))   {
           if(substr($xmlArray['Reply'], 0, 7) != 'Success') {
                $this->returnStatus['errors'][] = $xmlArray['Details'];
            }
            else {
                $this->returnStatus['messages'][] = 'Success';
                $this->returnStatus['serverClientId'] = $xmlArray['Details'];
            }                  
        }
        else if(isset($xmlArray['Error'])) {
            $this->returnStatus['errors'][] = $xmlArray['Error'];
        }              
        else {
            $this->returnStatus['errors'][] = $response['response'];
        }
        
        if(empty($this->returnStatus['errors'])) 
            $this->returnStatus['status'] = true;
        else
            $this->returnStatus['status'] = false;
        return $this->returnStatus;    
   }
         
}  // end of class   
  
?>
