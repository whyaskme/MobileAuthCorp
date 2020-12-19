<?php
    // set form values if redisplaying due to errors.
	if(!empty($errorArray)) {
		foreach($formVars as $field => $value) {
			$theField = $field;
			$$theField = $value;
		}
	}
	
	// initialize
	$registerClass = "hide-form"; //  initially hide the Register button
	$registerClass = "";          //   always show the Register button
	if(isset($showRegister) && $showRegister != '') {  // set in login.php, which can load this screen
	   $registerClass = $showRegister;
	}
	
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
	
	$requestUrl = $_SESSION['mac-bank']['config']['registerUrl'];
	$params = '?action=reg&demo='.urlencode($_SESSION['mac-bank']['config']['clientName']);
	$params .= '&cid='.$_SESSION['mac-bank']['config']['clientId'];
	$params .= '&fromUrl='.$fromURL;
	$registerUrl = $requestUrl.$params;
    //echo "<script type='text/javascript'>alert('$requestUrl');</script>";
?>
<script type="text/javascript">
    $(document).ready(function () {
        $("#shadow").hide();
        $("#mobileMenu").hide();
    });
</script>

<!--loading lightbox-->
<div id="divPleaseWaitProcessing" class="lightbox_background" >
    <div id="divDialogContainer" class="lightbox_content">
        <div id="circularG">
            <div id="circularG_1" class="circularG"></div>
            <div id="circularG_2" class="circularG"></div>
            <div id="circularG_3" class="circularG"></div>
            <div id="circularG_4" class="circularG"></div>
            <div id="circularG_5" class="circularG"></div>
            <div id="circularG_6" class="circularG"></div>
            <div id="circularG_7" class="circularG"></div>
            <div id="circularG_8" class="circularG"></div>
        </div>
    </div>
</div>

<!--iframe popup lightbox-->
<div id="divPleaseWaitProcessing_popup" class="lightbox_background_popup"></div>
<div id="divDialogContainer_popup" class="lightbox_content_popup"></div>

<div id="columnMain" style="background: url('images/bg-main-435.jpg') repeat-x top center;min-height:435px;">
    <div class="main-content" style="min-height: 180px;">
        <div class="container-fluid">
            <div class="container" style="margin-top:1rem;">    
                <div class="col-md-12">
                    <div id="errorMessage" style="display: none;margin-bottom:0 !important;text-align:center;" class="alert alert-danger">
                        <?php if(isset($errorArray) && !empty($errorArray)) {
                            echo '<script type="text/javascript">$("#errorMessage").show()</script>';
                            $counter = 0;
                            foreach($errorArray as $msg) {
                                if($counter > 0)
                                    echo '<br />';
                                    echo $msg;
		                            $counter++;
                                }
                                }
                        ?>
                    </div>
                </div>
            </div>

            <div class="container"> 
                <div class ="col-md-8">
                    <!--<h4>Learn more about One-Time Passwords - <a href="http://www.mobileauthcorp.com/" target="_blank">Click Here</a></h4>
                    <p>MAC offers a secure, true User Authentication and Payment Transaction Verification solution for ALL payment verticals by delivering a One-Time Password to each customer via their mobile phone using an SMS text message.</p>-->
                    <!--<div style="margin-top: 0;color: #fff;font-size: 5.5rem;font-weight:600;text-align: center;">Online Bank Demo<br /><a href="http://www.mobileauthcorp.com/" target="_blank">Learn More</a></div>-->
                    <div class="hidden-sm hidden-xs" style="margin-top: 20px;color: #fff;font-size: 5.5rem;font-weight:600;text-align: center;">Online Banking</div>
                    <div class="visible-sm visible-xs" style="margin-top: 10px;color: #fff;font-size: 4.5rem;font-weight:600;text-align: center;line-height: 4.5rem;">Online Banking</div>
                </div>
                <div class="col-md-4" style="padding-left:0;padding-right:0;margin-top: 20px;">
                    <div style="margin: 0 auto 15px;z-index: 0;padding: 0; border: 25px solid #ffffff;max-width:335px; border: 25px solid rgba(255, 255, 255, 0.25); -ms-border-radius: 18px; border-radius: 18px;">
                        <div style="margin:0 auto;padding:0;z-index: 10;display: block;max-width:335px;width: 100%;max-height: 195px;height: auto;background: #fff !important;text-align: left;">
                            <div id="divLoginContainer">
                                <div style="padding: 1rem 1rem 0;">
                                    <label>
                                        Account Login
                                        <input name="user_id" type="text" id="user_id" size="40" maxlength="50" value="<?= isset($userID) ? $userID : ''; ?>" class="form-control" placeholder="Enter Email Address"  autocomplete="on" style="margin-top: 10px;border-radius:0 !important;"/>
                                    </label>
                                </div>
                                <div style="text-align: center;margin:0;">
                                    <button type="button" id="login-submit-button" name="login-submit-button" value="Login" class="btn btn-default">Login</button> &nbsp;&nbsp;
                                    <div style="padding:0.5rem 0;background:#ccc;margin:1rem 0 0;" title="Demo Registration">New User?  <a href="<?= $registerUrl ?>" >Register</a></div>
                                </div>
                                <div style="clear: both;"></div>
                            </div>
                        </div>
                    </div>                    
                </div>
            </div>
        </div>
    </div>
</div>