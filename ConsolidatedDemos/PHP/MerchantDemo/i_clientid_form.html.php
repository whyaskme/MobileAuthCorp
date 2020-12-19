<div id="columnMain">
    <p class="page-header"><span>Change Client Name, Id</span></p>
<div class="container-fluid"> 
  <div class="row">   
   <div class="col-md-11">
 <div id="successMessage" style="margin-top: 8px;" class="msg-green">
<?php  if(isset($messageArray) && !empty($messageArray)) {
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
    
  <form action="index.php?action=saveclientid" method="post" name="client_form" id="client-form">
    <table width="100%" border="0">
     
      <?php
      if(!is_array($iniArray) || !isset($iniArray['config'])) {
          $iniArray = array();         // set empty input screen
          $iniArray['config'] = array();
          $iniArray['config']['clientName'] = '';
          $iniArray['config']['clientId'] = '';
          $iniArray['config']['requestOtpUrl'] = '';
          $iniArray['config']['verifyOtpUrl'] = '';
          $iniArray['config']['macbankUrl'] = '';
          $iniArray['config']['registerUrl'] = '';
      }
	  foreach($iniArray['config'] as $field => $value) {
          if($field == 'PHPSESSID' || $field == 'useServerClientId'   // dont allow display or update of this variable, not sure how it gets into the config file
             || $field == 'Submit' || $field == 'action' || $field == 'serverClientId')
             continue;

          echo "<tr><td width='22%' style='padding-left: 10px;' class='border-bottom-style'><div align='right'>$field</div></td>
        <td width='1%' >&nbsp;</td>
        <td width='76%' >
          <input name='$field' type='text' id='client-name' size='60'  placeholder='enter $field' value='$value' />
          &nbsp;</td></tr>";
	  }
      echo "<td width='20%' style='padding-left: 10px;' class='border-bottom-style'><div align='right'><input name='useServerClientId' type='checkbox' value='1' checked /> - use Server Client Id</div></td>
        <td width='4%' >&nbsp;</td>
        <td width='76%' >
          <input name='serverClientId' type='text' id='serverClientId' size='60'  value='$serverClientId' />
          &nbsp;</td>";
	  ?>
      
      <tr>
        <td colspan="3">&nbsp;</td>
      </tr>
      <tr>
        <td colspan="3">
          <input type="submit" id="save-client-button" name="Submit" value="Save Client Settings" />
</td>
      </tr>
      </table>
  </form>
    </div>   <!-- end of col-md-n -->
   </div>   <!-- end of row -->
  </div>  <!-- end of container-fluid-->
</div>