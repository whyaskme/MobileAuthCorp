<?php
// set field values for display
	foreach($userData as $field => $value) {
		$theField = $field;
		$$theField = $value;
	}
?>
<div id="columnMain">
    <p class="page-header">MAC Test - User Profile</p>
<div class="container-fluid"> 
  <div class="row">   
  <form id="profile" name="profile" method="POST" enctype="multipart/form-data" class="form-horizontal" role="form" action="index.php?action=profile">
<div class="col-md-10">
<div id="errorMessage" class="error-msg-red">
<?php
 if(!empty($errorArray)) {
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
<div id="billing-info">
  <div class="form-group squish-form">
    <label class="col-sm-4 control-label" for="fname">first name</label>
    <div class="col-sm-8">
      <input id="fname" name="fname" class="form-control" type="text" placeholder="enter first name"  value="<?= $fname?>" />
    </div>
  </div>
  <div class="form-group squish-form">
    <label class="col-sm-4 control-label" for="lname"> last name</label>
    <div class="col-sm-8">
    <input id="lname" name="lname" class="form-control" type="text" placeholder="enter last name"    value="<?= $lname?>" />
    </div>
  </div>
  <div class="form-group squish-form">
    <label class="col-sm-4 control-label" for="address">address</label>
    <div class="col-sm-8">
    <input id="address" name="address" class="form-control" type="text" placeholder="enter address"    value="<?= $address?>"  />
    </div>
  </div>
  <div class="form-group squish-form">
    <label class="col-sm-4 control-label" for="state">city</label>
    <div class="col-sm-8">
    <input id="city" name="city" class="form-control" type="text" placeholder="enter city"    value="<?= $city?>" />
    </div>
  </div>
  <div class="form-group squish-form">
    <label class="col-sm-4 control-label" for="state">state</label>
    <div class="col-sm-8">
    <input id="state" name="state" class="form-control" type="text" placeholder="enter state"    value="<?= $state?>" maxlength="2"/>
    </div>
  </div>
  <div class="form-group squish-form">
    <label class="col-sm-4 control-label" for="zipe">zip</label>
    <div class="col-sm-8">
    <input id="zip" name="zip" class="form-control" type="text" placeholder="enter zip"    value="<?= $zip?>" />
    </div>
  </div>
  <div class="form-group squish-form">
    <label class="col-sm-4 control-label" for="password">change password</label>
    <div class="col-sm-8">
    <input id="password" name="password" class="form-control" type="password" placeholder="enter new password"    value="<?= $password?>" />
    </div>
  </div>
  <div class="form-group squish-form">
    <label class="col-sm-4 control-label" for="password2">re-type new password</label>
    <div class="col-sm-8">
    <input id="password2" name="password2" class="form-control" type="password" placeholder="retype new password"    value="<?= $password2?>" />
    </div>
  </div>
  <div class="form-group squish-form">
    <label class="col-sm-4 control-label" for="name">email</label>
    <div class="col-sm-8">
    <input id="email" name="email" class="form-control" type="text" placeholder="enter email"   value="<?= $email?>" />
    </div>
  </div>
  <div class="form-group squish-form">
    <label class="col-sm-4 control-label" for="cell_name">phone</label>
    <div class="col-sm-8">
   <input id="cell_phone" name="cell_phone" class="form-control" type="text" placeholder="enter phone"   value="<?= $cell_phone?>"  />
    </div>
  </div>
  <div class="form-group squish-form">
    <label class="col-sm-4 control-label phone-margin-top" for="payment_type">payment account 1</label>
    <div class="col-sm-8">
      <label class="col-sm-1 control-label less-col-padding-1" for="preferred_payment_account">acct#</label>
      <div class="col-sm-5 less-col-padding-10">
        <input name="preferred_payment_account" type="text" id="textfield" placeholder="enter account number" class="form-control less-col-padding-6 text_10" value="<?= $preferred_payment_account?>"/><input name="preferred_payment_account_update" type="hidden" value="" />
      </div>
      <label class="col-sm-1 control-label less-col-padding-1" for="preferred_payment_type">type</label>
      <div class="col-sm-4 less-col-padding-10">
      <input name="preferred_payment_type" type="text" id="verified"  class="form-control less-col-padding-6 text_10" value="<?= $preferred_payment_type?>" readonly="readonly"/>
      </div>
      <div class="col-sm-1 less-col-padding-4">
        <input name="verified" type="text" id="textfield2"  class="form-control less-col-padding-1 text_10 msg-green" value="<?= $verified ?>" readonly="readonly"/>
      </div>
    </div>
  </div>
 <div class="form-group squish-form">
    <label class="col-sm-4 control-label phone-margin-top" for="payment_type">payment account 2</label>
    <div class="col-sm-8">
      <label class="col-sm-1 control-label less-col-padding-1" for="preferred_payment_account2">acct#</label>
      <div class="col-sm-5 less-col-padding-10">
        <input name="preferred_payment_account2" type="text" id="textfield" placeholder="enter account number" class="form-control less-col-padding-6 text_10" value="<?= $preferred_payment_account2?>"/><input name="preferred_payment_account2_update" type="hidden" value="" />
      </div>
      <label class="col-sm-1 control-label less-col-padding-1" for="preferred_payment_type2">type</label>
      <div class="col-sm-4 less-col-padding-10">
      <input name="preferred_payment_type2" type="text" id="textfield"  class="form-control less-col-padding-6 text_10" value="<?= $preferred_payment_type2?>" readonly="readonly"/>
      </div>
      <div class="col-sm-1 less-col-padding-4">
        <input name="verified2" type="text" id="verfied2"  class="form-control less-col-padding-1 text_10 msg-green" value="<?= $verified2 ?>" readonly="readonly"/>
      </div>
    </div>
    </div>
  </div><div style="margin-top: 10px;"><span class="align-right"><input name="update_profile" type="submit" value="update profile" /></span>
   <input type="hidden" name="ClientId" id="ClientId" value="53a9a3d8faba33196cbabdf5"/> <!-- was 538ecc2c1c86333658d0806c -->
 </div>
 <div style="margin-top: 10px;"><span id="status_messages" class="align-right"></span></div>
</div> 
 </div>  <!-- end of col -->
 </form>
 </div>  <!-- end of row -->
 <div class="row">
   <div id="auth-status" class="error-msg-red">&nbsp;
   </div>
  </div>  <!-- end of row -->
 </div>  <!-- end of container-fluid --> 
</div>
