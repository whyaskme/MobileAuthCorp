<?php 
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

$requestUrl = $_SESSION['mac-cart']['config']['registerUrl'];  //  build url if not logged in
$params = '?action=unreg&demo='.urlencode($_SESSION['mac-cart']['config']['clientName']);
$params .= '&cid='.$_SESSION['mac-cart']['config']['clientId'];
$params .= '&fromUrl='.$fromURL;
$unRegisterUrl = $requestUrl.$params;


   //$requestUrl = $_SESSION['mac-cart']['config']['registerUrl'];  //  build url if not logged in
   //$params = '?action=unreg&demo='.urlencode($_SESSION['mac-cart']['config']['clientName']);
   //$params .= '&cid='.$_SESSION['mac-cart']['config']['clientId'];
   //$unRegisterUrl .= $requestUrl.$params;

?>
<style>
    p {
        margin-top:1.5rem;
    }
</style>
<div class="cboth"></div>
<div id="footer_txt" style="padding-bottom:2.5rem;">
	<div class="row wrap">
		<div id="column_left" class="col-sm-4 col-left-txt">
			<h3>Who We Are</h3>
			<p>Scottsdale   Golf Store is the premier online shopping experience for golfers of all   ages and levels of experience. Shop our top selections for the very   best deals on golf clubs, equipment, apparel, and accessories.</p>
            <p>Because   we are committed to providing the utmost in online shopping security,   this site is powered by MAC - delivering a one-time password to your   mobile phone via text message to validate your identity at the time of purchase. Learn more about this innovative security technology at <a href="http://www.mobileauthcorp.com/" target="_blank">www.mobileauthcorp.com</a></p>
        </div>
        
        <div id="column_center" class="col-sm-4 col-center-txt">
			<h3>Join our Mailing List</h3>
			<p>Sign   up to receive email updates on new product announcements, gift ideas,   special promotions, sales and more. &nbsp;When signing up for our mailing   list, you will also have the option to sign up to receive emails from our suppliers and affiliates.</p>
        </div>
        
        <div id="column_right" class="col-sm-4 col-right-txt">
			<h3>Contact Us			</h3>
            <p>
                8777 E. Via da Ventura
                <br/>
                Suite 280
                <br />
                Scottsdale, Arizona 85258
            </p>
			<p>(480) 939-2980<br/>
			    <a href="#">info@scottsdalegolf.com</a>
            </p>
		</div>
	    
	    <div class="cboth"></div>
	</div>
</div>
<div id="footer-wrap2" style="border-top:1px solid #d2d2d2;">
    <div id="footer-disclaimer" style="padding:2.5rem;">
        <fieldset>
                <h3 style="font-size: 1.75rem;">MAC Disclosure Statement</h3>
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
            <span style="font-size: 1.5rem; font-weight: bold;">To unsubscribe, <a href="<?= $unRegisterUrl ?>" style="font-size: 1.5rem; font-weight: bold;">click here</a></span>
        </fieldset>
    </div>
    <div id="footer_foto">
        
    </div>   
</div>    
<div style="margin-top: 1rem;font-size:12px;color: #222; ">
        <script type="text/javascript">
            <!--
    var currentDate = new Date();
    var year = currentDate.getFullYear();
    document.write("&copy; " + year + " MAC Online Merchant Demo. All rights reserved.");
    //-->
        </script>
    </div>
    <script>
        $(document).ready(function () {
            var l = $("#column_left").height();
            $("#column_center").height(l);
            $("#column_right").height(l);
        });
    </script>    

