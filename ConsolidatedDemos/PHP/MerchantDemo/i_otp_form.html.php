<?php
  if($otpFormType == 'otp_login') {
	  $otpAction = 'validate_otp_login';
	  $otpSubmitButton = 'Submit';
  }
  else if($otpFormType == 'otp_purchase') {
	  $otpAction = 'validate_otp';
	  $otpSubmitButton  = 'Submit';
  }
?>
<div id="columnMain">
    <p class="page-header"><span>Enter Authentication Code</span></p>
<div class="container-fluid"> 
  <div class="row">   
   <div class="col-md-7">
<?php
// add the Ad if one exists  - not working yet
  if(isset($_SESSION['mac-cart']['otp']['ad']) && !empty($_SESSION['mac-cart']['otp']['ad'])) {
      echo $_SESSION['mac-cart']['otp']['ad'];
   }   
?>  
<div id="infoMessage" style="margin-top: 8px;" >
  <?php if(isset($infoArray) && !empty($infoArray) && $infoArray[0] != null) {
   $counter = 0;
   foreach($infoArray as $msg) {
         if($counter > 0)
            echo '<br />';
         echo $msg;
         $counter++;
     }
 }
?>
</div>
<div id="successMessage" style="margin-top: 8px;" class="msg-green">
<?php /* if(isset($messageArray) && !empty($messageArray)) {
   $counter = 0;
   foreach($messageArray as $msg) {
         if($counter > 0)
            echo '<br />';
         echo $msg;
         $counter++;
     }
 }   */
?>
</div>
<div id="errorMessage" style="margin-top: 8px;" class="error-msg-red">
<?php if(isset($errorArray) && !empty($errorArray)) {
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
<?php 
global $debugOtp;
?>
    
  <form action="index.php?action=<?= $otpAction?>" method="post" name="otp_form" id="otp-form">
    <table width="100%" border="0">
      <tr>
        <td width="20%" style="padding-left: 10px;" class="border-bottom-style"><div align="right">Code: </div></td>
        <td width="4%" >&nbsp;</td>
        <td width="76%" >
          <input name="otp" type="text" id="otp" size="10" maxlength="10" value="<?php echo $debugOtp; ?>" placeholder="Enter Authentication Code" autocomplete="off"/>
          &nbsp;<img id="refresh-otp" class="text-bg-hilite" src="images/arrow_refresh.png" width="30" height="30" alt="refresh otp" /> <span style="font-style:italic; font-weight:200;">Resend</span></td>
      </tr>
      <tr>
        <td colspan="3">&nbsp;</td>
      </tr>
      <tr>
        <td colspan="3">
          <input type="submit" id="otp-submit-button" name="Submit" value="<?= $otpSubmitButton ?>" class="btn btn-default" />
          <input type="hidden" name="ClientId" id="ClientId" value="53a9a3d8faba33196cbabdf5"/> <!-- was 538ecc2c1c86333658d0806c -->
          &nbsp;<span id="form-status" style="display:none;">please wait&nbsp;&nbsp;<img src="images/wait_snake_red.gif" alt="" /></span></td>
      </tr>
      </table>
  </form>
   </div>  <!-- end of container-fluid-->
  </div>   <!-- end of row -->
 </div>   <!-- end of col-md-n -->
</div>