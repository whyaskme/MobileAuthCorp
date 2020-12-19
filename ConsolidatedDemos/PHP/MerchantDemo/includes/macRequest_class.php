<?php

 include_once 'includes/functions.php';
 include_once 'includes/macCurlSubmitter_class.php';
 
class MacRequest {
    
    public $formVars;
    public $requestType;
    public $userType;    // 'clientManaged', or 'registered'
    
    function __construct($userType, $formVars) {
       $this->userType = $userType;
       $this->formVars = $formVars; 
       // initialize error, message holder
       $_SESSION['mac-cart']['billing-info']['errors'] = array();
       $_SESSION['mac-cart']['billing-info']['messages'] = array();
       $_SESSION['mac-cart']['billing-info']['info-message'] = array();
    }
       

    // Client Managed User - will be logged in to shopping site - may not registered at bank             
    function preAuth() {
       // preauth first
        $requestData = "Request:Preauth";   //Command to service
        if ($this->formVars['ClientId'] == null || strlen($this->formVars['ClientId']) == 0) //Client Id as issued by MAC
           $_SESSION['mac-cart']['billing-info']['errors'][] = "Client ID required!";
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
           if(substr($xmlArray['Reply'], 0, 7) == 'Success') {
                if(strtolower(substr($xmlArray['Details'], 0, 15)) != 'funds available' && strtolower(substr($xmlArray['Details'], 0, 11)) != 'under limit') {
                   $_SESSION['mac-cart']['billing-info']['errors'][] = $xmlArray['Details'];
                }
                else  {
                    $_SESSION['mac-cart']['billing-info']['messages'][] = $xmlArray['Details'];
                }
           }
           else {
               if(isset($xmlArray['Details']))
                  $_SESSION['mac-cart']['billing-info']['errors'][] = substr($xmlArray['Details'], 0, 50);
           }
        }
        else if(isset($xmlArray['Error'])) {
            $_SESSION['mac-cart']['billing-info']['errors'][] = $xmlArray['Error'];
        }              
        else {
            $_SESSION['mac-cart']['billing-info']['errors'][] = $response['response'];
        }
 
        if(empty($_SESSION['mac-cart']['billing-info']['errors']))
            return true;
        else
            return false;
        // look for Success: Funds available
   }
  
   // request otp for otpType - 'login', or 'auth'          
   function requestOTP($otpType) {
       $requestData = "Request:SendOtp"; // Command to service
       if ($this->formVars['ClientId'] == null || strlen($this->formVars['ClientId']) == 0) //Client Id as issued by MAC
           $_SESSION['mac-cart']['billing-info']['errors'][] = "Client ID required!";
       $requestData .= "|CID:" . $this->formVars['ClientId'];

       if (isset($this->formVars['pGroupId']) && $this->formVars['pGroupId'] != null && strlen($this->formVars['pGroupId']) != 0) // Optional if client request is restricted to a group
          $requestData .= "|GroupId:" . $this->formVars['pGroupId'];

       if($this->userType == 'clientManaged') {
           if ($this->formVars['cell_phone'] == null || strlen($this->formVars['cell_phone']) == 0)
              $_SESSION['mac-cart']['billing-info']['errors'][] = "End User's Phone Number required!";
           $requestData .= "|PhoneNumber:" . preg_replace('/[^\d]/', '', $this->formVars['cell_phone']);
             
           if ($this->formVars['email'] == null || strlen($this->formVars['email']) == 0)
               $_SESSION['mac-cart']['billing-info']['errors'][] = "End User email address required!";
           $requestData .= "|EmailAddress:" . $this->formVars['email'];
       }
       else if($this->userType == 'registered') {
           if($this->formVars['lname'] == null)
               $_SESSION['mac-cart']['billing-info']['errors'][] = 'Last name required';
           if($this->formVars['user_id'] == null)  // email
               $_SESSION['mac-cart']['billing-info']['errors'][] = 'email required';
           $userIdBeforeMd5 = "|UserId:" . strtolower($this->formVars['lname']).strtolower($this->formVars['user_id']);   
           $requestData .= "|UserId:" . strtoupper(md5(strtolower($this->formVars['lname']).strtolower($this->formVars['user_id'])));   
       }
       
       if($otpType == 'login')          
           $requestData .= "|TrxType:" . '1';  // auth request, get OTP
       else if($otpType == 'auth')    
           $requestData .= "|TrxType:" . '2';  // auth request, get OTP
       
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
            if(substr($xmlArray['Reply'], 0, 9) == 'RequestId'){     // success
                $_SESSION['mac-cart']['otp']['RequestId'] =  $xmlArray['RequestId'];
                if($this->userType == 'clientManaged') {
                    $holdPhone = preg_replace('/[^\d]/', '', $this->formVars['cell_phone']);
                    $holdPhone = "(***) ***-**".substr($holdPhone, 8, 2);
                    //$_SESSION['mac-cart']['billing-info']['messages'][] = 'enter the TCode texted to '.$holdPhone.' &nbsp;(<em>code valid for 3 minutes</em>)';
                    $_SESSION['mac-cart']['billing-info']['messages'][] = 'Enter the Authentication Code sent to your mobile phone.';
                }
                else if($this->userType == 'registered') {
                    //$_SESSION['mac-cart']['billing-info']['messages'][] = 'enter the TCode texted to your registered cellphone &nbsp;(<em>code valid for 3 minutes</em>)';
                	$_SESSION['mac-cart']['billing-info']['messages'][] = 'Enter the Authentication Code sent to your mobile phone.';
                }
                if(isset($xmlArray['EnterOTPAd'])) {
                    $ad = hexToStr($xmsArray['EnterOTPAd']);
                }
                else if(($adPos = strpos($xmlArray['Reply'], '|EnterOTPAd:')) > 0) {
                    $ad = hexToStr(substr($xmlArray['Reply'], $adPos + 4));
                }
                $_SESSION['mac-cart']['otp']['ad'] = $ad;    
            }    
            else { 
                $_SESSION['mac-cart']['billing-info']['errors'][] = strip_tags(substr($response['response'], 0, 50));
            }
        }
        else if(isset($xmlArray['Error'])) {
            $_SESSION['mac-cart']['billing-info']['errors'][] = $xmlArray['Error'];
        }              
        else {
            $_SESSION['mac-cart']['billing-info']['errors'][] = strip_tags($response['response']);
        }
        
        if(empty($_SESSION['mac-cart']['billing-info']['errors']))
            return true;
        else
            return false;
       // var_dump( $response);
       // echo '<br /><br />';
   }
    
    // resend otp          
   function resendOTP() {
       $requestData = "Request:ResendOtp"; // Command to service
       if ($this->formVars['ClientId'] == null || strlen($this->formVars['ClientId']) == 0) //Client Id as issued by MAC
           $_SESSION['mac-cart']['billing-info']['errors'][] = "Client ID required!";
       $requestData .= "|CID:" . $this->formVars['ClientId'];

       if (!isset($_SESSION['mac-cart']['otp']['RequestId']) || $_SESSION['mac-cart']['otp']['RequestId'] == null)
           $_SESSION['mac-cart']['billing-info']['errors'][] = 'Request Id from OTP is missing';
       $requestData .= "|RequestId:" . $_SESSION['mac-cart']['otp']['RequestId'];   
      
       $requestData .= "|API:PHP";   
       
        // 99 indicates the data is converted to a hex string (not encrypted)
        $data_before_hex = "Data=99" . strlen($this->formVars['ClientId']) . strtoupper($this->formVars['ClientId']) . 
              $requestData;

        $data = "Data=99" . strlen($this->formVars['ClientId']) . strtoupper($this->formVars['ClientId']) . 
              strToHex($requestData);

        $mac = new MAC_Auth();
        $response = $mac->doRequest('requestOtp', $data);
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
        	// need to add logic if status = success, return message "OTP has been resent"
           if(substr($xmlArray['Reply'], 0, 6) != 'Resent') {
                //$_SESSION['mac-cart']['billing-info']['errors'][] = $xmlArray['Details'];
           	$_SESSION['mac-cart']['billing-info']['messages'][] = 'Authentication Code resent.';
            }
            else  {
                $_SESSION['mac-cart']['otp']['RequestId'] =  $xmlArray['RequestId'];
                if($this->userType == 'clientManaged') {
                    $holdPhone = preg_replace('/[^\d]/', '', $_SESSION['mac-cart']['billing-info']['cell_phone']);
                    $holdPhone = "(***) ***-**".substr($holdPhone, 8, 2);
                    $_SESSION['mac-cart']['billing-info']['messages'][] = 'enter the TCode texted to '.$holdPhone.' &nbsp;(<em>code valid for 3 minutes</em>)';
                }
                else if($this->userType == 'registered') {
                    $_SESSION['mac-cart']['billing-info']['messages'][] = 'enter the TCode texted to your registered cellphone &nbsp;(<em>code valid for 3 minutes</em>)';
                }
            }
        }
        else if(isset($xmlArray['Error'])) {
            if(strpos($xmlArray['Error'], 'Resend OTP Disabled') > 0)
                $xmlArray['Error'] = $xmlArray['Error'] . '-> Please View Cart to restart your purchase';
            $_SESSION['mac-cart']['billing-info']['errors'][] = $xmlArray['Error'];
        }              
        else {
            $_SESSION['mac-cart']['billing-info']['errors'][] = $response['response'];
        }
        
        if(empty($_SESSION['mac-cart']['billing-info']['errors']))
            return true;
        else
            return false;
    }  
                     
      
   function validateOTP() {
       $requestData = "Request:VerifyOtp"; // Command to service
       if ($this->formVars['ClientId'] == null || strlen($this->formVars['ClientId']) == 0) //Client Id as issued by MAC
           $_SESSION['mac-cart']['billing-info']['errors'][] = "Client ID required!";
       $requestData .= "|CID:" . $this->formVars['ClientId'];
       
       if (!isset($_SESSION['mac-cart']['otp']['RequestId']) || $_SESSION['mac-cart']['otp']['RequestId'] == null)
           $_SESSION['mac-cart']['billing-info']['errors'][] = 'Request Id from OTP is missing';
       $requestData .= "|RequestId:" . $_SESSION['mac-cart']['otp']['RequestId'];   
          
       if (!isset($this->formVars['otpCode']) || $this->formVars['otpCode'] == '')
           $_SESSION['mac-cart']['billing-info']['errors'][] = 'Please enter OTP code';
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
            if($xmlArray['Reply'] == 'Validated') {           //  success ???
                $_SESSION['mac-cart']['billing-info']['messages'][] = substr($xmlArray['Details'], 0, 70);
            }
            else { 
                if(isset($xmlArray['Error'])) 
                    $_SESSION['mac-cart']['billing-info']['errors'][] = $xmlArray['Error'];
                else if(isset($xmlArray['Details']))
                    $_SESSION['mac-cart']['billing-info']['errors'][] = substr($xmlArray['Details'], 0, 80);    
                else     
                    $_SESSION['mac-cart']['billing-info']['errors'][] = strip_tags(substr($response['response'], 0, 80));
            }
        }
        else if(isset($xmlArray['Error'])) {
            $_SESSION['mac-cart']['billing-info']['errors'][] = $xmlArray['Error'];
        }              
        else {
            $_SESSION['mac-cart']['billing-info']['errors'][] = strip_tags($response['response']);
        }
        
        if(empty($_SESSION['mac-cart']['billing-info']['errors']))
            return true;
        else
            return false;
       // var_dump( $response);
       // echo '<br /><br />';
   }
     
         // get account holder details
   function getAccountDetails() {      
        $requestData = "Request:GetAccountDetails";   //Command to service
        if ($this->formVars['ClientId'] == null || strlen($this->formVars['ClientId']) == 0) //Client Id as issued by MAC
           $_SESSION['mac-cart']['billing-info']['errors'][] = "Client ID required!";
        $requestData .= "|AccountHolder:" . $this->formVars['fname'].' '.$this->formVars['lname'];

        // 99 indicates the data is converted to a hex string (not encrypted)
        $data_before_hex = "Data=99" . strlen($this->formVars['ClientId']) . strtoupper($this->formVars['ClientId']) . 
              $requestData;

        $data = "Data=99" . strlen($this->formVars['ClientId']) . strtoupper($this->formVars['ClientId']) . 
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
                $_SESSION['mac-cart']['billing-info']['errors'][] = $xmlArray['Details'];
            }
            else if(!is_array($xmlArray['Details']) && substr($xmlArray['Details'], 0, 8) != 'Success:') {
                $_SESSION['mac-cart']['billing-info']['errors'][] = $xmlArray['Details'];
            }
            else {
                $_SESSION['mac-cart']['billing-info']['messages'][] = 'Success';
                $_SESSION['mac-cart']['billing-info']['accounts'] = $xmlArray['Details'];
            }                  
        }
        else if(isset($xmlArray['Error'])) {
            $_SESSION['mac-cart']['billing-info']['errors'][] = $xmlArray['Error'];
        }              
        else {
            $_SESSION['mac-cart']['billing-info']['errors'][] = $response['response'];
        }
      
        if(empty($_SESSION['mac-cart']['billing-info']['errors']))
            return true;
        else
            return false;
   }
        
    // Registered User - does not need to be logged in to shopping site - must be registered at bank
    function validateLogin() {
        // validage login name
        $requestData = "Request:ValidateLoginName";   //Command to service
        if ($this->formVars['ClientId'] == null || strlen($this->formVars['ClientId']) == 0) //Client Id as issued by MAC
            $_SESSION['mac-cart']['billing-info']['errors'][] = "Client ID required!";
        $requestData .= "|LoginName:" . $this->formVars['user_id'];

        // 99 indicates the data is converted to a hex string (not encrypted)
        $data_before_hex = "Data=99" . strlen($this->formVars['ClientId']) . strtoupper($this->formVars['ClientId']) . 
              $requestData;

        $data = "Data=99" . strlen($this->formVars['ClientId']) . strtoupper($this->formVars['ClientId']) . 
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
                $_SESSION['mac-cart']['billing-info']['errors'][] = $xmlArray['Details'];
            }
            else  {
                $holdReply = explode('|', $xmlArray['Details']);
                $_SESSION['mac-cart']['billing-info']['pan'] = $holdReply[0];
                $holdName = explode(' ', $holdReply[1]);
                $_SESSION['mac-cart']['billing-info']['fname'] = $holdName[0];
                $_SESSION['mac-cart']['billing-info']['lname'] = $holdName[1];
                $_SESSION['mac-cart']['billing-info']['messages'][] = $xmlArray['Details'];
            }
        }
        else if(isset($xmlArray['Error'])) {
            $_SESSION['mac-cart']['billing-info']['errors'][] = $xmlArray['Error'];
        }              
        else {
            $_SESSION['mac-cart']['billing-info']['errors'][] = $response['response'];
        }
        
        if(empty($_SESSION['mac-cart']['billing-info']['errors']))
            return true;
        else
            return false;
    }  
                
         
            
    // account details
    function getAccountDetailsRegistered() {        
        $requestData = "Request:GetAccountDetails";   //Command to service
        if ($this->formVars['ClientId'] != null || strlen($this->formVars['ClientId']) > 0)  //Client Id as issued by MAC
           $clientId =  $this->formVars['ClientId'];
        else     
           $_SESSION['mac-cart']['billing-info']['errors'][] = "Client ID required!";

        if(empty($_SESSION['mac-cart']['billing']['errors']))
           $requestData .= "|ClientId:" . $clientId;
           
        $requestData .= "|AccountHolder:" . $this->formVars['fname'].' '.$this->formVars['lname'];

        // 99 indicates the data is converted to a hex string (not encrypted)
        $data_before_hex = "Data=99" . strlen($cientId) . strtoupper($clientId) . 
           $requestData;

        $data = "Data=99" . strlen($clientId) . strtoupper($clientId) . 
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
                $_SESSION['mac-cart']['billing-info']['errors'][] = $xmlArray['Details'];
            }
            else if(!is_array($xmlArray['Details']) && substr($xmlArray['Details'], 0, 8) != 'Success:') {
                $_SESSION['mac-cart']['billing-info']['errors'][] = $xmlArray['Details'];
            }
            else {
                $_SESSION['mac-cart']['billing-info']['messages'][] = 'Success';
                $_SESSION['mac-cart']['billing-info']['accounts'] = $xmlArray['Details'];
            }                  
        }
        else if(isset($xmlArray['Error'])) {
            $_SESSION['mac-cart']['billing-info']['errors'][] = $xmlArray['Error'];
        }              
        else {
            $_SESSION['mac-cart']['billing-info']['errors'][] = $response['response'];
        }
        
        if(empty($_SESSION['mac-cart']['billing-info']['errors']))
            return true;
        else
            return false;
    }
        
    function moveFunds() {        
        // move funds
       $requestData = "Request:MoveFunds";   //Command to service
       if ($this->formVars['ClientId'] == null || strlen($this->formVars['ClientId']) == 0) //Client Id as issued by MAC
          $_SESSION['mac-cart']['billing-info']['errors'][] = "Client ID required!";
       $requestData .= "|CID:" . $this->formVars['ClientId'];

       if(isset($this->formVars['preferred_payment_type']) && $this->formVars['preferred_payment_type'] != '') {
          $requestData .= "|FromAccountHolder:" . $this->formVars['fname'].' '.$this->formVars['lname'];
          $requestData .= "|FromAccountName:" . $this->formVars['preferred_payment_type'];    
       }
       else if(isset($this->formVars['preferred_payment_account']) && $this->formVars['preferred_payment_account'] != '') {
            $requestData .= "|FromAccountNo:" .preg_replace('/[^\d]/', '', $this->formVars['preferred_payment_account']);
       }
       $requestData .= "|ToAccountHolder:" . "Kohl's";
       $requestData .= "|ToAccountName:" . "6601000000001305";    
       $requestData .= "|Amount:" . $this->formVars['order_total'];

       // parse and send transaction details
       $transDetail = '';
       $counter = 1;
       foreach($this->formVars as $field => $value) {
         if(substr($field, 0, 8) == 'product_') {
             $transDetail .= " $value";
             if($counter % 2 == 0)
                 $transDetail .= '|';
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
        
        if($response['status'] == 'success' && substr($response['response'], 0, 10) != 'Curl Error'  && !isset($xmlArray['Error']))   {
            $cleanedXml = preg_replace('/\s\s+/', ' ',$response['response']);
            $xml = new SimpleXMLElement($cleanedXml);
        //    var_dump( $xml);
            $xmlArray = xmlToArray($xml);
            if(substr($xmlArray['Reply'], 0, 7) == 'Success') {
                $_SESSION['mac-cart']['billing-info']['messages'][] = 'Success';
                $_SESSION['mac-cart']['billing-info']['messages'][] = $xmlArray['Details'];
            }
            else
                 $_SESSION['mac-cart']['billing-info']['errors'][] = $xmlArray['Details'];
        } 
        else if(isset($xmlArray['Error'])) {
            $_SESSION['mac-cart']['billing-info']['errors'][] = $xmlArray['Error'];
        }              
        else {
            $_SESSION['mac-cart']['billing-info']['errors'][] = $response['response'];
        }
        
        if(empty($_SESSION['mac-cart']['billing-info']['errors']))
            return true;
        else
            return false;
    } 
 
    function purchase() {
         // move funds
       $requestData = "Request:Purchase";   //Command to service
       if ($this->formVars['ClientId'] == null || strlen($this->formVars['ClientId']) == 0) //Client Id as issued by MAC
           $_SESSION['mac-cart']['billing-info']['errors'][] = "Client ID required!";
       $requestData .= "|CID:" . $this->formVars['ClientId'];
       $requestData .= "|AccountHolder:" . $this->formVars['fname'].' '.$this->formVars['lname'];

       if(isset($this->formVars['preferred_payment_type']) && $this->formVars['preferred_payment_type'] != '') {
          $requestData .= "|AccountName:" . $this->formVars['preferred_payment_type'];    
       }
       else if(isset($this->formVars['preferred_payment_account']) && $this->formVars['preferred_payment_account'] != '') {
            $requestData .= "|CardNumber:" .preg_replace('/[^\d]/', '', $this->formVars['preferred_payment_account']);
       }
       $requestData .= "|MerchantName:" . $this->formVars['ClientName'];
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
        
        if($response['status'] == 'success' && substr($response['response'], 0, 10) != 'Curl Error'  && !isset($xmlArray['Error']))   {
            if(substr($xmlArray['Reply'], 0, 7) == 'Success') {
                $_SESSION['mac-cart']['billing-info']['messages'][] = 'Success';
                $_SESSION['mac-cart']['billing-info']['messages'][] = $xmlArray['Details']['Transaction'];
            }
            else
                 $_SESSION['mac-cart']['billing-info']['errors'][] = $xmlArray['Details'];
        } 
        else if(isset($xmlArray['Error'])) {
            $_SESSION['mac-cart']['billing-info']['errors'][] = $xmlArray['Error'];
        }              
        else {
            $_SESSION['mac-cart']['billing-info']['errors'][] = $response['response'];
        }
      
        if(empty($_SESSION['mac-cart']['billing-info']['errors']))
            return true;
        else
            return false;
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
            $useClientName = "Scottsdale Golf Store";
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
                $_SESSION['mac-cart']['billing-info']['errors'][] = $xmlArray['Details'];
            }
            else {
                $_SESSION['mac-cart']['billing-info']['messages'][] = 'Success';
                $_SESSION['mac-cart']['billing-info']['serverClientId'] = $xmlArray['Details'];
            }                  
        }
        else if(isset($xmlArray['Error'])) {
            $_SESSION['mac-cart']['billing-info']['errors'][] = $xmlArray['Error'];
        }              
        else {
            $_SESSION['mac-cart']['billing-info']['errors'][] = $response['response'];
        }
      
        if(empty($_SESSION['mac-cart']['billing-info']['errors']))
            return true;
        else
            return false;
   }

           
}  // end of class   
  
?>
