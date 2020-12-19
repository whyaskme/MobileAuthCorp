<?php
    // set form values if redisplaying due to errors.
	if(isset($useridSaved)) {
		$user_id= $useridSaved;
	}

?>
<div id="columnMain">
<blockquote>
  <p style="font-weight: bold;">Password Reset Request -- please enter your user ID...</p>
  <p style="margin-bottom: 20px;">We will email a secure link for the password reset screen to the email address on file for your user id.</p>
  <form action="index.php?action=forgot" method="post" name="forgot" id="forgot">
    <table width="566" border="0">
      <tr>
        <td width="143" style="padding-left: 10px;" class="border-bottom-style"><div align="right"><font size="2">User ID </font></div></td>
        <td width="10">&nbsp;</td>
        <td width="399"><font size="2">
          <input name="userid" type="text" id="userid" size="30" maxlength="50" value="<?php echo $user_id; ?>"/>
        </font></td>
      </tr>
      <tr>
        <td class="border-bottom-style"><div align="right" ><font size="2">enter characters from image</font></div></td>
        <td>&nbsp;</td>
        <td><span style="padding-top: 20px;">
          <input name="imagetext" type="text" id="imagetext" value="" size="30" maxlength="6" />
          &nbsp;&nbsp;<img src="include/random_image.php" alt="random image" style="vertical-align: middle; border-style: solid; border-color: blue;" /></span></td>
      </tr>
      <tr>
        <td><font size="2">
          <input type="submit" name="Submit" value="Submit" />
          </font></td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
      </tr>
    </table>
  </form>
  <table width="341" border="0">
    <tr>
      <td width="167"><font size="2"><a href="index.php?action=loginform">Returning User Login</a> </font></td>
      <td width="164"><font size="2"><a href="index.php?action=registerform">New User Registration</a> </font></td>
    </tr>
    <tr>
      <td><span class="error-msg-red"><font size="2">
<?php
 if(!empty($errorArray)) {
     foreach($errorArray as $msg) {
         echo $msg."<br/>";
     }
 }
 ?>
      </font></span></td>
      <td>&nbsp;</td>
    </tr>
  </table>
</blockquote>
</div>