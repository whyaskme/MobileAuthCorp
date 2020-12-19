<?php

  /*
  * MAC Auth class definition
  */

  class MAC_Auth {

     private $host;
     private $port = 80;
     private $attempts;
     private $connectDelay;
     private $processDelay;
	// private $baseUrl = 'http://corp.mobileauthcorp.com/macservices/';
     private $requestUrl  = 'http://corp.mobileauthcorp.com/macservices/Otp/RequestOTP.asmx/WsRequestOtp';
     private $verifyUrl = 'http://corp.mobileauthcorp.com/macservices/Otp/ValidateOTP.asmx/WsValidateOtp';
     private $testBankUrl = 'http://corp.mobileauthcorp.com/macservices/Test/MacTestBank.asmx/WsMacTestBank';


    public function __construct($connectDelay=null, $attempts=null, $processDelay=null) {
        // set the url's using saved config data
        if(isset($_SESSION['mac-cart']['config']['requestOtpUrl']) && $_SESSION['mac-cart']['config']['requestOtpUrl'] != null) 
           $this->requestUrl  = $_SESSION['mac-cart']['config']['requestOtpUrl'];
        if(isset($_SESSION['mac-cart']['config']['verifyOtpUrl']) && $_SESSION['mac-cart']['config']['verifyOtpUrl'] != null) 
           $this->verifyUrl = $_SESSION['mac-cart']['config']['verifyOtpUrl'];
        if(isset($_SESSION['mac-cart']['config']['macbankUrl']) && $_SESSION['mac-cart']['config']['macbankUrl'] != null) 
           $this->testBankUrl = $_SESSION['mac-cart']['config']['macbankUrl'];
        
        // total connection time allowed
        if(!isset($connectDelay) || $connectDelay == null || $connectDelay == '')
            $this->connectDelay = 12;      // default
        else
           $this->connectDelay = $connectDelay;
                
        // number of retry call attempts   
        if(!isset($attempts) || $attempts == null || $attempts == '')
            $this->attempts = 2;          // default
        else
           $this->attempts = $attempts;
         
        // process time delay   
        if(!isset($processDelay) || $processDelay == null || $processDelay == '')
            $this->processDelay = 12;      // default 
        else
           $this->processDelay = $processDelay;

    }

    private function doCurl($url, $input) {
           
            $ch = curl_init();

            curl_setopt($ch, CURLOPT_HTTPHEADER, array ("Content-Type: application/x-www-form-urlencoded", "Expires: Mon, 26 Jul 1997 05:00:00 GMT"));    //  text/xml for production
            curl_setopt($ch, CURLOPT_URL, $url);
            curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
            curl_setopt($ch, CURLOPT_HEADER, false);
            curl_setopt($ch, CURLOPT_POST, true);
            curl_setopt($ch, CURLOPT_POSTFIELDS, $input);
            curl_setopt($ch, CURLOPT_FRESH_CONNECT, false);  
            curl_setopt($ch, CURLOPT_CONNECTTIMEOUT, $this->connectDelay);
            curl_setopt($ch, CURLOPT_TIMEOUT, $this->processDelay);

            // attempt to send request
            $attempts_counter = 0;
            while ($attempts_counter < $this->attempts) {
                $attempts_counter++;
                $data = curl_exec($ch);
                if($data === false) continue;  // try to send again
                else break;
            }
            if($data == false) {
                if(curl_error($ch)) {
                    return 'Curl Error: '.curl_error($ch);
                }
            }

            curl_close($ch);

            return $data;
    }

	public function doRequest($requestType, $input) {	  
		 
        // determine which URL based on parameter 
	    if($requestType == 'requestOtp')
		    $url = $this->requestUrl;
	    else if($requestType == 'verifyOtp')
		    $url = $this->verifyUrl;
        else if($requestType == 'bank')
            $url = $this->testBankUrl;    
            
        // send the request    
	    $response = $this->doCurl($url, $input);
	    
	    // check if $response contains <debug></debug> tags
	    $doc = new DOMDocument;
	    $doc->loadXML($response);
	    
	    $debugValue = $doc->getElementsByTagName('Debug')->item(0)->nodeValue;
	    
	    if($debugValue != "" || $debugValue != null) {
	    	global $debugOtp;
	    	$debugOtp = substr($debugValue, 4, 16);
	    	// set debugOtp as global variable
	    	//$GLOBALS['debugOtp'] = $debugOtp;
	    }
	    
        if($response == null || $response == false || strlen($response) < 1) // unexpected response from PRC
		   return (array('status' => 'error'));
		else 
           return (array('status' => 'success', 'response' => $response));
    }		   
 
  }   // end of class definition


?>