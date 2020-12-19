<?php
// set field values for display
  if(is_array($userData) && !empty($userData)) {
	foreach($userData as $field => $value) {
		$theField = $field;
		$$theField = $value;
	}
  }
  $showHideBankForm = 'hide-form';  // initialize show class for which form to show
  $showHideCartForm = 'hide-form';
?>

<link href="css/mac.css" rel="stylesheet" type="text/css" />

<div id="columnMain">
    <p class="page-header">MAC Test - Enter Billing Information<span class="align-right">
      <input id="modify-order" name="modify_order" type="button" value="Modify Order" /></span></p>
<div class="container-fluid"> 
  <div class="row">   
   <div class="col-md-7">
   <h4>Order Details</h4>
   <table id="cart-content" class="cart-content cart">
     <tr class="">
       <th class="foto">Product Number</th>
       <th class="removable-column foto">Description</th>
       <th class="foto">Unit Price</th>
       <th class="foto">Qty</th>
       <th class="text-align-right foto">Total Price</th>
     </tr>
     <?php
        if(!empty($products)) {
            foreach($products as $index => $product) {
                echo  '
    <tr id="cart'.$product['product_number'].'" class="cart-row">
     <td  class="foto">'.
      $product['product_number'].
     '</td>
     <td class="removable-column foto">'.
      $product['description'].
     '</td>
     <td  class="foto"> $<span class="cart-product-price">'.
      $product['price'].'</span>
     </td>
     <td class="foto" >'
       .$product['qty'].
     '</td> 
	 <td class="foto">
	  <span class="cart-total-product-price align-right">'.$product['total_price'].'</span>
	 </td>
      
    </tr>';
      } 
    }
    else  {
       echo '<tr class="cart-row"><td>Cart is empty</td></tr>';
    }
    // output totals
    echo '
      <tr><td colspan="5"><span id="empty-message-cell"></span></td></tr>
      <tr>
       <td class="">&nbsp;</td>
	   <td class="removable-column">&nbsp;</td>
       <td colspan="2" class="total-amt-container" style="border-top-style: solid; border-top-width: thin;">Product Total</td>
       <td class="total-amt-container"  style="border-top-style: solid; border-top-width: thin;"><span class="cart-order-product-total align-right">$'.$product_totals['order_product_total'].'</span></td>
     </tr>
      <tr><td colspan="5"><span id="empty-message-cell"></span></td></tr>
      <tr>
       <td  class="">&nbsp;</td>
	   <td class="removable-column">&nbsp;</td>
       <td colspan="2" class="total-amt-container">Shipping Amt</td>
       <td class="total-amt-container"><span class="cart-order-shipping-total align-right">$'.$product_totals['order_shipping_amt'].'</span></td>
     </tr>
      <tr><td colspan="5"><span id="empty-message-cell"></span></td></tr>
      <tr>
       <td  class="">&nbsp;</td>
	   <td class="removable-column">&nbsp;</td>
       <td colspan="2" class="total-amt-container">Order Total</td>
       <td class="total-amt-container"><span class="cart-order-total align-right">$'.$product_totals['order_total'].'</span></td>
     </tr>	 
	 ';

    ?>
   </table>
   </div> <!-- end of col -->
<div class="col-md-5">
<div id="billing-info">
<h4 >Billing Information</h4>
<div id="infoMessage" style="margin-top: 8px;" class="msg-orange">
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
</span></div>
<div id="successMessage" style="margin-top: 8px;" class="msg-green">
<?php if(isset($messageArray) && !empty($messageArray)) {
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
<?php 
if(!isset($_SESSION['mac-cart']['loggedIn']) || (isset($_SESSION['mac-cart']['loggedIn']) && $_SESSION['mac-cart']['loggedIn'] != true)) { ?>
<div id="billing-form-container-cart-user" >
<form id="billing_information_cart" name="billing_information" method="POST" action="index.php?action=verify" enctype="multipart/form-data" class="form-horizontal" role="form">
  <div id="error_msg" class="error-msg-red"></div>
  <div class="form-group">
    <label class="col-sm-4 control-label" for="fname">first name<span class="error-msg-red">*</span></label>
    <div class="col-sm-8">
      <input id="fname" name="fname" class="form-control" type="text" placeholder="enter first name" value="<?= $fname?>" />
    </div>
  </div>
  <div class="form-group">
    <label class="col-sm-4 control-label" for="lname"> last name<span class="error-msg-red">*</span></label>
    <div class="col-sm-8">
    <input id="lname" name="lname" class="form-control" type="text" placeholder="enter last name"  value="<?= $lname?>"  />
    </div>
  </div>
  <div class="form-group">
    <label class="col-sm-4 control-label" for="address">address</label>
    <div class="col-sm-8">
    <input id="address" name="address" class="form-control" type="text" placeholder="enter address"  value="<?= $address?>" />
    </div>
  </div>
  <div class="form-group">
    <label class="col-sm-4 control-label" for="bill_state">city</label>
    <div class="col-sm-8">
    <input id="city" name="city" class="form-control" type="text" placeholder="enter city"  value="<?= $city?>"  />
    </div>
  </div>
  <div class="form-group">
    <label class="col-sm-4 control-label" for="state">state</label>
    <div class="col-sm-8">
    <input id="bill_state" name="state" class="form-control" type="text" maxlength="2" placeholder="enter state"  value="<?= $state?>" />
    </div>
  </div>
  <div class="form-group">
    <label class="col-sm-4 control-label" for="zip">zip</label>
    <div class="col-sm-8">
    <input id="zip" name="zip" class="form-control" type="text" placeholder="enter zip"  value="<?= $zip?>" />
    </div>
  </div>
  <div class="form-group">
    <label class="col-sm-4 control-label" for="name">email<span class="error-msg-red">*</span></label>
    <div class="col-sm-8">
    <input id="email" name="email" class="form-control" type="email" placeholder="enter email"  value="<?= $email?>" />
    </div>
  </div>
  <div class="form-group">
    <label class="col-sm-4 control-label" for="cell_phone">cell phone<span class="error-msg-red">*</span></label>
    <div class="col-sm-8">
   <input id="cell_phone" name="cell_phone" class="form-control" type="text" placeholder="enter phone" value="<?= $cell_phone?>" />
    </div>
  </div>
  <div class="form-group">
    <label class="col-sm-4 control-label" for="preferred_payment_account">card number<span class="error-msg-red">*</span></label>
    <div class="col-sm-8">
      <input id="preferred_payment_account" name="preferred_payment_account" class="form-control my-form-control-border" type="text" placeholder="enter credit card" value="<?= $preferred_payment_account?>" />
    </div>
  </div>
  <div class="form-group">
    <label class="col-sm-4 control-label" for="decurity_code">security code<span class="error-msg-red">*</span></label>
    <div class="col-sm-8">
      <input id="security_code" name="security_code" class="form-control my-form-control-border" type="text" placeholder="enter security code" maxlength="3" value="<?= $security_code?>" />
    </div>
  </div>

 <div style="margin-top: 10px;"><span class="align-right"><input name="verify_order" type="submit" value=" validate " /></span>
    <input type="hidden" name="user_type" id="user-type" value="clientManaged"/>
    <input type="hidden" name="ClientId" id="ClientId" value="53a9a3d8faba33196cbabdf5"/> <!-- was 538ecc2c1c86333658d0806c -->
</div>
 </form>
 </div>
<?php } else if(isset($_SESSION['mac-cart']['loggedIn']) && $_SESSION['mac-cart']['loggedIn'] == true) { ?> 
 <div id="billing-form-container-bank-user" >
 <form id="billing_information_bank" name="billing_information_bank" method="POST" action="index.php?action=verify" enctype="multipart/form-data" role="form">
  <div id="error_msg" class="error-msg-red"></div>
  <div class="form-group">
    <label class="col-sm-4 control-label" for="user_id">User ID<span class="error-msg-red">*</span></label>
    <div class="col-sm-8">
      <input id="user_id" name="user_id" class="form-control" type="text" placeholder="enter email" value="<?= $user_id?>" />
    </div>
  </div>
  <div class="form-group">
    <label class="col-sm-4 control-label" for="fname">first name<span class="error-msg-red">*</span></label>
    <div class="col-sm-8">
      <input id="fname" name="fname" class="form-control" type="text" placeholder="enter first name" value="<?= $fname?>" />
    </div>
  </div>
  <div class="form-group">
    <label class="col-sm-4 control-label" for="lname"> last name<span class="error-msg-red">*</span></label>
    <div class="col-sm-8">
    <input id="lname" name="lname" class="form-control" type="text" placeholder="enter last name"  value="<?= $lname?>"  />
    </div>
  </div>

 <div style="margin-top: 10px;"><span class="align-right"><input name="verify_order" type="submit" value="&nbsp;verify&nbsp;" /></span>
   <input type="hidden" name="user-type" id="user-type2" value="registered"/>
   <input type="hidden" name="ClientId" id="ClientId" value="53a9a3d8faba33196cbabdf5"/> <!-- was 538ecc2c1c86333658d0806c -->
 </div>
 </form>
 </div>
 <?php } ?>
 
 <div style="margin-top: 10px;"><span id="status_messages" class="align-right"></span></div>
</div> 
 </div>  <!-- end of col -->
 </div>  <!-- end of row -->
 </div>  <!-- end of container-fluid --> 
</div>
