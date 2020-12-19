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
    
    $requestUrl = $_SESSION['mac-cart']['config']['registerUrl']; 
    $params = '?action=reg&demo='.urlencode($_SESSION['mac-cart']['config']['clientName']);
    $params .= '&cid='.$_SESSION['mac-cart']['config']['clientId'];
    $params .= '&fromUrl='.$fromURL;
    $registerUrl = $requestUrl.$params;
    //echo "<script type='text/javascript'>alert('$registerUrl');</script>";
?>

<div id="columnMain">
    <p class="page-header"><span>Registered User Login</span></p>
<div class="container-fluid"> 
  <div class="row">   
   <div class="col-md-7">
    <table style="width:60%;" border="0">
      <tr>
        <td style="padding-left: 10px;width:23%;" class="border-bottom-style">
            <div><label style="font-size:1.25rem;">User ID: </label></div>
        </td>
        <td style="width:3%;">&nbsp;</td>

        <td style="width:74%;">
          <input name="user_id" type="text" id="user_id" size="30" maxlength="50" value="<?= $userID ?>" placeholder=""  autocomplete="off"/>
        </td>
      </tr>
      <tr style="display: none;">
        <td style="padding-left: 10px;" class="border-bottom-style">
            <div style="text-align:right;"><label style="font-size:1.25rem;">Password</label></div>
        </td>
        <td>&nbsp;</td>
        <td>
          <input name="password" type="password" id="password" size="30" maxlength="35" autocomplete="off"/>
        </td>
      </tr>
      <tr>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
      </tr>
      <tr>
        <td colspan="3">
            <button type="button" id="login-submit-button" name="login-submit-button" value="Login" class="btn btn-default" >Login</button>
            &nbsp;
            <span id="form-status" style="display:none;">please wait&nbsp;&nbsp;<img src="images/wait_snake_red.gif" alt="" /></span>
        </td>
      </tr>
      <tr>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
      </tr>
      <tr>
        <td colspan="3">
            <span style="padding: 0.5rem;" title="New users must register before they can use this demo, thank you.">New User?  <a href="<?= $registerUrl ?>" >Register</a></span>       
        </td>
      </tr>
    </table>
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
</div> <!-- end of error -->
   </div>  <!-- end of container-fluid-->
  </div>   <!-- end of row -->
 </div>   <!-- end of col-md-n -->
</div>