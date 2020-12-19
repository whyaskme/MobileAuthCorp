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
     private $requestUrl  = 'Otp/RequestOTP.asmx/WsRequestOtp';
     private $verifyUrl = 'Otp/ValidateOTP.asmx/WsValidateOtp';
     private $testBankUrl = 'Test/MacTestBank.asmx/WsMacTestBank';


    public function __construct($connectDelay=null, $attempts=null, $processDelay=null) {
        // set the url's using saved config data
        $this->requestUrl  = $_SESSION['mac-cart']['config']['requestOtpUrl'];
        $this->verifyUrl = $_SESSION['mac-cart']['config']['verifyOtpUrl'];
        $this->testBankUrl = $_SESSION['mac-cart']['config']['macbankUrl'];
        
        if(!isset($connectDelay) || $connectDelay == null || $connectDelay == '')
            $this->connectDelay = 12;      // default
        else
           $this->connectDelay = $connectDelay;

        if(!isset($attempts) || $attempts == null || $attempts == '')
            $this->attempts = 1;          // default
        else
           $this->attempts = $attempts;

        if(!isset($processDaelay) || $processDelay == null || $processDelay == '')
            $this->processDelay = 8;      // default 
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
		 
	    if($requestType == 'requestOtp')
		    $url = $this->requestUrl;
	    else if($requestType == 'verifyOtp')
		    $url = $this->verifyUrl;
        else if($requestType == 'bank')
            $url = $this->testBankUrl;    
            
	 
	    $response = $this->doCurl($url, $input);
        if($response == null || $response == false || strlen($response) < 1) // unexpected response from PRC
		   return (array('status' => 'error'));
		else 
           return (array('status' => 'success', 'response' => $response));
    }		   
 
  }   // end of class definition


?>