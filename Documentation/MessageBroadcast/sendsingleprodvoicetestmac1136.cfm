<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<title>Test Add To Queue</title>
</head>

<body>

	<!--- generate signature string --->
	<cffunction name="generateSignature" returnformat="json" access="remote" output="false">
		<cfargument name="method" required="true" type="string" default="GET" hint="Method" />
                 
		<cfset dateTimeString = GetHTTPTimeString(Now())>
		<!--- Create a canonical string to send --->
		<cfset cs = "#arguments.method#\n\n\n#dateTimeString#\n\n\n">
		<cfset signature = createSignature(cs,variables.secretKey)>
		
		<cfset retval.SIGNATURE = signature>
		<cfset retval.DATE = dateTimeString>
		
		<cfreturn retval>
	</cffunction>
	
	<!--- Encrypt with HMAC SHA1 algorithm--->
	<cffunction name="HMAC_SHA1" returntype="binary" access="private" output="false" hint="NSA SHA-1 Algorithm">
	   <cfargument name="signKey" type="string" required="true" />
	   <cfargument name="signMessage" type="string" required="true" />
	
	   <cfset var jMsg = JavaCast("string",arguments.signMessage).getBytes("iso-8859-1") />
	   <cfset var jKey = JavaCast("string",arguments.signKey).getBytes("iso-8859-1") />
	   <cfset var key = createObject("java","javax.crypto.spec.SecretKeySpec") />
	   <cfset var mac = createObject("java","javax.crypto.Mac") />
	
	   <cfset key = key.init(jKey,"HmacSHA1") />
	   <cfset mac = mac.getInstance(key.getAlgorithm()) />
	   <cfset mac.init(key) />
	   <cfset mac.update(jMsg) />
	
	   <cfreturn mac.doFinal() />
	</cffunction>
	
	
	<cffunction name="createSignature" returntype="string" access="public" output="false">
	    <cfargument name="stringIn" type="string" required="true" />
		<cfargument name="scretKey" type="string" required="true" />
		<!--- Replace "\n" with "chr(10) to get a correct digest --->
		<cfset var fixedData = replace(arguments.stringIn,"\n","#chr(10)#","all")>
		<!--- Calculate the hash of the information --->
		<cfset var digest = HMAC_SHA1(scretKey,fixedData)>
		<!--- fix the returned data to be a proper signature --->
		<cfset var signature = ToBase64("#digest#")>
		
		<cfreturn signature>
	</cffunction>
    
	<cfset variables.accessKey = 'B32EB51E57CC116F4BF7' />
    <cfset variables.secretKey = '5E1/1BD3A4805cE3c223e6AB3Ca7414+02089F86' />
   
 	<cfset variables.sign = generateSignature("POST") /> <!---Verb must match that of request type--->
	<cfset datetime = variables.sign.DATE>
    <cfset signature = variables.sign.SIGNATURE>
    
    <cfset authorization = variables.accessKey & ":" & signature>
    
    <cfdump var="#datetime#"> 
    <BR />
    <cfdump var="#signature#"> 
    <BR />
    <cfdump var="#authorization#"> 
    <BR />
    <cfoutput>
        variables.accessKey = #variables.accessKey#
        <BR />
        variables.secretKey = #variables.secretKey#
        <BR />
    </cfoutput>
    
    <!--- The method="XXX" used in the http request must match the method used to generate key in the generateSignature() function--->
 	<cfhttp url="http://ebm.messagebroadcast.com/webservice/ebm/pdc/addtorealtime" method="POST" result="returnStruct" >
		
       <!--- Required components --->
    
	   <!--- By default EBM API will return json or XML --->
	   <cfhttpparam name="Accept" type="header" value="application/json" />    	
	   <cfhttpparam type="header" name="datetime" value="#datetime#" />
       <cfhttpparam type="header" name="authorization" value="#authorization#" /> 
  	   
       <!--- If DebugAPI is greater than 0 and is specidfied then use Dev DB for testing--->
       <!--- If in debug mode make sure you are using the correct credentials--->
       <cfhttpparam type="formfield" name="DebugAPI" value="0" />
       
       <!--- Batch Id controls which pre-defined campaign to run --->
       <cfhttpparam type="formfield" name="inpBatchId" value="1136" />
       
       <!--- Contact string--->
       <cfhttpparam type="formfield" name="inpContactString" value="Put a phone number here" />
       
       <!--- 1=voice 2=email 3=SMS--->
       <cfhttpparam type="formfield" name="inpContactTypeId" value="1" />
       
       
       
       <!--- Optional Components --->
       
       <!--- Custom data element for PDC Batch Id 1136 --->
       <cfhttpparam type="formfield" name="inpCode" value="12345678" />
                 
    </cfhttp>
    
    returnStruct<BR />                        
    <cfdump var="#returnStruct#">  
    
    <BR />
    
    <cfoutput>
    	#returnStruct.Filecontent#                      
    </cfoutput>
 
</body>
</html>