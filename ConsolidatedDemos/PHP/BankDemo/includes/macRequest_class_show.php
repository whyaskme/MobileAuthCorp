<?php
  
 include_once 'includes/functions.php';                // generic utility functions
 include_once 'includes/macCurlSubmitter_class.php';   // perform call to OTP server
 
class MacRequest {
    
    public $formVars;     //  input values for request
    public $requestType;  //  client managed, or registered user
    public $userType;     // 'clientManaged', or 'registered'
    public $returnStatus; // otp request results
    
    // data fields needed for building request are passed in as an associative array in $formVars
    //  when object is instantiated
    //  array keys are the field names needed by the request, not all are required, depends on request
    //  'clientid' => value
    //  'fname' => first name
    //  'lname' => last name
    //  'pGroupId' => value
    //  'email'   => email address
    //  'cell_phone' => phone to receive OTP code
    //  'user_id' => user id, usually email address
    //  'preferred_payment_type' => name of account (not numbet, eq. 'PrePaid Account')
    //  'otpCode' => value (OTP entered by user )
    // fields for transactions - for purchase transactions
    //  'product_description1' => value
    //  'product_number1' => value
    //  'product_total1' => value
    //   ...
    //  'product_descriptionN' => value
    //  'product_numberN' => value
    //  'product_totalN' => value
    //  'order_product_total' => value
    //  'order_shipping_total' => value
    //  'order_total'          => value
    
    function __construct($userType, $formVars) {
       if($userType == '')
          $this->userType = 'default';
       else    
          $this->userType = $userType;
       $this->formVars = $formVars; 
       
       // initialize error, message response array
       // thie array returns the restults from each request call
       $this->returnStatus['status'] = null;           // true or false
       $this->returnStatus['errors'] = array();        // error messages
       $this->returnStatus['messages'] = array();      // response messages - successful transaction, etc
       $this->returnStatus['info-message'] = array();  // info messages
    }

   // request otp for otpType - 'login', or 'auth' type of transaction         
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
               $this->returnStatus['errors'][] = "End User email address required!";
           $requestData .= "|EmailAddress:" . $this->formVars['email'];
       }
       else if($this->userType == 'registered') {
           if($this->formVars['lname'] == null)
               $this->returnStatus['errors'][] = 'Last name required';
           if($this->formVars['user_id'] == null)  // email
               $this->returnStatus['errors'][] = 'email required';
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
                }
                else if($this->userType == 'registered') {
                //    $this->returnStatus['messages'][] = 'enter the TCode texted to your registered cellphone &nbsp;(<em>code valid for 3 minutes</em>)';
                }
                if(isset($xmlArray['Ad'])) {
                    $ad = hexToStr($xmsArray['Ad']);
                }
                else if(($adPos = strpos($xmlArray['Reply'], '|Ad:')) > 0) {
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
   }
    
   // resend the last otp, need the saved requestId from the requestOTP() call to match up for resending.   
   function resendOTP($requestId) {
       $requestData = "Request:ResendOtp"; // Command to service
       if ($this->formVars['ClientId'] == null || strlen($this->formVars['ClientId']) == 0) //Client Id as issued by MAC
           $this->returnStatus['errors'][] = "Client ID required!";
       $requestData .= "|CID:" . $this->formVars['ClientId'];

       if (!isset($requestId) || $requestId == null)
           $this->returnStatus['errors'][] = 'Request Id from OTP is missing';
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
                $this->returnStatus['errors'][] = $xmlArray['Details'];
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
                $xmlArray['Error'] = $xmlArray['Error'] . '-> Please restart your login.';
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
   function validateOTP(4requestId) {
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
   }
     
}       
  
?>
