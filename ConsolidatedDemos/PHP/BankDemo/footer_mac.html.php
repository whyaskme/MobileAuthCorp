<?php 
   //$requestUrl = $_SESSION['mac-bank']['config']['registerUrl'];  //  build url if not logged in
   //$params = '?action=unreg&demo='.urlencode($_SESSION['mac-bank']['config']['clientName']);
   //$params .= '&cid='.$_SESSION['mac-bank']['config']['clientId'];
   //$unRegisterUrl .= $requestUrl.$params;
   
   //$params = '?action=reg&demo='.urlencode($_SESSION['mac-bank']['config']['clientName']);
   //$params .= '&cid='.$_SESSION['mac-bank']['config']['clientId'];
   //$registerUrl .= $requestUrl.$params;
   
   // get current page URL
   $pageURL = 'http';
   if ($_SERVER["HTTPS"] == "on") {$pageURL .= "s";}
   $pageURL .= "://";
   if ($_SERVER["SERVER_PORT"] != "80") {
   	$pageURL .= $_SERVER["SERVER_NAME"].":".$_SERVER["SERVER_PORT"].$_SERVER["REQUEST_URI"];
   } else {
   	$pageURL .= $_SERVER["SERVER_NAME"].$_SERVER["REQUEST_URI"];
   }
   
   $fromURL = strtok($pageURL, "?");
   
   $requestUrl = $_SESSION['mac-bank']['config']['registerUrl'];  //  build url if not logged in
   $params = '?action=unreg&demo='.urlencode($_SESSION['mac-bank']['config']['clientName']);
   $params .= '&cid='.$_SESSION['mac-bank']['config']['clientId'];
   $params .= '&fromUrl='.$fromURL;
   $unRegisterUrl = $requestUrl.$params;

?>
    <div class="cboth"></div>
    <div class="container">
        <div id="footer_txt" style="border:1px solid #c9c9c9;background:#fff !important;">
	        <div class="row">
		        <div class="col-md-4 col-xs-12" style="padding-right:0;">
                    <div class="col-left-txt" id="col1">
			            <h4>Overview</h4>
			            <p>
                            Mobile Authentication Corp. offers two robust solutions: Secure User Authentication and Transaction Verification for Online Retailer Payments, and Online Banking for Financial Institutions.    
                        </p>
                        <p>
                            <a href="http://www.mobileauthcorp.com/" target="_blank">Learn More</a>
                        </p>
                    </div>
                </div>
        
                <div class="col-md-4 col-xs-12" style="padding-right:0;">
                    <div class="col-center-txt" id="col2">
	                    <h4>Privacy and Security</h4>
			            <p>
                            To review our "Terms and Conditions" and "Privacy Policy", please click the links below.
                        </p>
		                <p>
                            <a href="http://www.mobileauthcorp.com/MAC-Terms-and-Conditions-43458.pdf" target="_blank">Terms and Conditions</a><br />
                            <a href="http://www.mobileauthcorp.com/MAC-Privacy-Policy.pdf" target="_blank">Privacy Policy</a>
                        </p>
                    </div>
                </div>
        
                <div class="col-md-4 col-xs-12" style="padding-right:0;">
                    <div class="col-right-txt" id="col3">
                        <h4>Contact Us</h4>
                        <p>
                            8777 E. Via de Ventura
                            <br />
                            Suite 280
                            <br />
                            Scottsdale, Arizona 85258
                        </p>
                        <p>
                            1-844-427-0411
                            <br />
                            <a href="mailto:info@mobileauthcorp.com">info@mobileauthcorp.com</a>
                        </p>
                    </div>
		        </div>
	            <div class="cboth"></div>
	        </div>
        </div>
    </div>

    <!--<div style="padding:0.5rem;"></div>-->
    <div style="text-align:center;margin:1rem 0.5rem;">
        <span style="font-size: 1rem;color:#ccc;margin:0;">&#9899;</span>
        <span style="font-size: 1rem;color:#ccc;margin:0;">&#9899;</span>
        <span style="font-size: 1rem;color:#ccc;margin:0;">&#9899;</span>
        <span style="font-size: 1rem;color:#ccc;margin:0;">&#9899;</span>
        <span style="font-size: 1rem;color:#ccc;margin:0;">&#9899;</span>
    </div>
    <div class="container">
        <div id="footer-disclaimer" style="border:1px solid #c9c9c9;">
            <fieldset style="background-color: #f0f0f0;padding:1.5rem;">
                <p style="margin-top:0;">
                    <strong>MAC Disclosure Statement</strong>
                </p>
                <p>
                    The information contained in this website is for general information purposes only.
                    While we endeavor to keep the information up to date and correct, we make no representations or warranties of any kind, express or implied, about the completeness, accuracy, reliability,
                    suitability or availability with respect to the website or the information, products, services, or related graphics contained on this website for any purpose. Any reliance you place on
                    such information is therefore strictly at your own risk.
                </p>
                <p>
                    In no event will we be liable for any loss or damage including without limitation,
                    indirect or consequential loss or damage, or any loss or damage whatsoever arising from loss of data or profits arising out of, or in connection with, the use of this website.
                </p>
                <p>
                    Links to third party websites are not under the control of Mobile Authentication Corporation.
                    The inclusion of any links does not necessarily imply a recommendation or endorse the views expressed within them.
                </p>
                <p>
                    Every effort is made to keep the website up and running smoothly.
                    However, Mobile Authentication Corporation will not be liable for the website being temporarily unavailable due to technical issues beyond our control.
                </p>
                <p>To unregister <a href="<?= $unRegisterUrl ?>" style="font-size: 1.25rem;">click here</a></p>
            </fieldset>
        </div>
        <div id="footer-wrap2_in">
            <div class="hidden-sm hidden-xs" style="padding:1rem 1rem 1.5rem;font-size: 1.125rem;color: #999;">
                <script type="text/javascript">
                    <!--
                    var currentDate = new Date();
                    var year = currentDate.getFullYear();
                    document.write("&copy; " + year + " Mobile Authentication Corporation. All rights reserved.");
                    //-->
                </script>
            </div>
            <div class="visible-sm visible-xs" style="padding:1rem 1rem 1.5rem;text-align:center;font-size: 1.125rem;color: #999;">
                <script type="text/javascript">
                    <!--
                    var currentDate = new Date();
                    var year = currentDate.getFullYear();
                    document.write("&copy; " + year + " Mobile Authentication Corporation.<br />All rights reserved.");
                    //-->
                </script>
            </div>
        </div>
    </div>
