<?php
  if($otpFormType == 'otp_login') {
	  $otpAction = 'validate_otp_login';
      $infoArray[] = 'Account Login:';
	  $otpSubmitButton = 'Continue Login';
  }
  else if($otpFormType == 'otp_transfer') {
	  $otpAction = 'validate_otp_transfer';
      $infoArray[] = 'Transaction Verification and Authorization:';
	  $otpSubmitButton  = 'Continue transfer';
  }
  else if($otpFormType == 'otp_deposit') {
	  $otpAction = 'validate_otp_deposit';
      $infoArray[] = 'Transaction Verification and Authorization:';
	  $otpSubmitButton  = 'Continue deposit';
  }
  else if($otpFormType == 'otp_withdraw') {
	  $otpAction = 'validate_otp_withdraw';
      $infoArray[] = 'Transaction Verification and Authorization:';
	  $otpSubmitButton  = 'Continue withdrawal';
  }
  else if($otpFormType == 'otp_paybill') {
      $otpAction = 'validate_otp_paybill';
      $infoArray[] = 'Transaction Verification and Authorization:';
      $otpSubmitButton  = 'Continue bill pay';
  }
?>
<?php 
global $debugOtp;
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

            <div class="container"> 
                <div class="col-md-8">
                    <!--<div style="margin-top: 0;color: #fff;font-size: 3.5rem;font-weight:600;text-align: center;">Online Bank Demo<br /><a href="http://www.mobileauthcorp.com/" target="_blank">Learn More</a></div>-->
                    <div class="hidden-sm hidden-xs" style="margin-top: 0;color: #fff;font-size: 5.5rem;font-weight:600;text-align: center;">Online Banking</div>

                    <div class="row">    
                        <div class="col-md-12">
                            <?php
                            // add the Ad if one exists  - not working yet
                            //   if(isset($_SESSION['mac-bank']['otp']['ad']) && !empty($_SESSION['mac-bank']['otp']['ad'])) {
                            //      echo $_SESSION['mac-bank']['otp']['ad'];
                            //   } 
                            ?>

                            <div id="infoMessage" style="display: none;margin: 10px;text-align: center;">
                                <?php if(isset($infoArray) && !empty($infoArray)) {
                                    echo '<script type="text/javascript">$("#infoMessage").show()</script>';
                                    $counter = 0;
                                    $otpLabel = '';
                                    foreach($infoArray as $msg) {
                                        if($counter > 0)
                                            $otpLabel .= '<br />';
                                            $otpLabel .= $msg;
                                            $counter++;
                                        }
                                    }
                                ?>
                            </div>

                            <div id="successMessage" style="display: none;margin: 10px;text-align: center;" class="alert alert-success">
                                <?php  if(isset($messageArray) && !empty($messageArray)) {
                                    echo '<script type="text/javascript">$("#successMessage").show()</script>';
                                    $counter = 0;
                                    foreach($messageArray as $msg) {
                                        if($counter > 0)
                                            echo '<br />';
                                            echo $msg;
                                            $counter++;
                                        }
                                    }    
                                ?>
                            </div>

                            <div id="errorMessage" style="display: none;margin: 10px;text-align: center;" class="alert alert-danger">
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
                </div>

                <div class="col-md-4" style="margin-top:1rem;padding-left:0;padding-right:0;">
                    <form action="index.php?action=<?= $otpAction?>" method="post" name="otp_form" id="otp-form">
                        <div style="margin: 0 auto;z-index: 0;padding: 0; border: 25px solid #ffffff;max-width:335px; border: 25px solid rgba(255, 255, 255, 0.25); -ms-border-radius: 18px; border-radius: 18px;">
                            <div style="margin:0 auto;padding:0;z-index: 10;display: block;max-width:335px;width: 100%;max-height: 195px;height: auto;background: #fff !important;text-align: left;">
                                <div id="divLoginContainer">
                                    <div style="padding: 1rem 1rem 0;">
                                        <label>
                                            <?= $otpLabel ?>
                                            <input name="otpCode" type="text" id="otpCode" size="30" maxlength="50" value="<?php echo $debugOtp; ?>" class="form-control" placeholder="Enter Authentication Code"  autocomplete="on" style="margin-top: 5px;border-radius:0 !important;"/>
                                        </label>
                                    </div>
                                    <div style="text-align: center;padding:0.5rem 0 1.5rem;">
                                        <button type="submit" id="otp-submit-button" name="Submit"  class="btn btn-default">Submit</button>
                                        <button type="button" id="refresh-otp" name="SubmitRefresh"  class="btn btn-default">Resend</button>
                                    </div>
                                    <div style="clear: both;"></div>
                                </div>
                            </div>
                        </div>
                    </form>

                    <!--<div style="padding: 0.5rem;"></div>-->

                    <div style="padding:0 25px 25px;text-align: center;">
                        <?php
                            if(isset($_SESSION['mac-bank']['otp']['ad']) && !empty($_SESSION['mac-bank']['otp']['ad'])) {
                                echo $_SESSION['mac-bank']['otp']['ad'];
                            }
                        ?>
                    </div>
                </div>

            </div>
        </div>
    </div>
</div>