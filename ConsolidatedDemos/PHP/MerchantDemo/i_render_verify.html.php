<?php
    //default   
	$submitButtonText = 'purchase';
	$submitAction = 'auth';
?>	  
<div id="columnMain">
    <p class="page-header">MAC Test - Verify Billing Information<span class="align-right">
    <input id="modify-billing" name="modify_billing" type="button" value="<< modify billing" /></span></p>
<div class="container-fluid"> 
  <div class="row">   
  <form id="billing_information" name="billing_information" method="POST" enctype="multipart/form-data" class="form-horizontal" role="form" action="index.php?action=<?= $submitAction ?>">
   <div class="col-md-7">
   <h4>Order Details</h4>
    <table id="cart-content" class="cart-content cart">
     <tr class="">
       <th class="foto">product number</th><th class="removable-column foto">description</th><th class="foto">unit price</th><th class="foto">qty</th><th class="text-align-right foto">total price</th>
     </tr>
    <?php
        if(!empty($products)) {
			$counter = 1;
            foreach($products as $index => $product) {
                echo  '
    <tr id="cart'.$product['product_number'].'" class="cart-row">
     <td  class="foto">
      <input type="text" name="product_number'.$counter.'" class="form-control squish-form my-form-control" size="2" style="text-align: left;" readonly="readonly" value="'.$product['product_number'].
     '"</td>
     <td class="removable-column foto">
	 <input type="text" name="product_description'.$counter.'" class="form-control squish-form my-form-control" size="6" style="text-align: right;" readonly="readonly" value="'.$product['description'].'"/></td>
     <td class="foto" > $<span class="cart-product-price">'.
      $product['price'].'</span>
     </td>
     <td class="foto" >'
       .$product['qty'].
     '</td> 
	 <td class="foto">
	  <span class="cart-total-product-price align-right"><input type="text" name="product_total'.$counter.'" class="form-control squish-form my-form-control" size="6" style="text-align: right;" readonly="readonly" value="'.$product['total_price'].'"/></span>

    </td>
      
    </tr>';
	  $counter++;
      } 
    }
    else  {
       echo '<tr class="cart-row"><td>cart is empty</td></tr>';
    }
    // output totals
    echo '
      <tr><td colspan="5"><span id="empty-message-cell"></span></td></tr>
      <tr>
       <td class="">&nbsp;</td>
	   <td class="removable-column">&nbsp;</td>
       <td colspan="2" class="total-amt-container" style="border-top-style: solid; border-top-width: thin;">product total</td>
       <td class="total-amt-container"  style="border-top-style: solid; border-top-width: thin;"><span class="cart-order-product-total align-right"><input type="text" name="order_product_total" class="form-control squish-form my-form-control" size="6" style="text-align: right;" readonly="readonly" value="'.$product_totals['order_product_total'].'"/></span></td>
     </tr>
      <tr><td colspan="5"><span id="empty-message-cell"></span></td></tr>
      <tr>
       <td  class="">&nbsp;</td>
	   <td class="removable-column">&nbsp;</td>
       <td colspan="2" class="total-amt-container">shipping amt</td>
       <td class="total-amt-container"><span class="cart-order-shipping-total align-right"><input type="text" name="order_shipping_total" class="form-control squish-form my-form-control" size="6" style="text-align: right;" readonly="readonly" value="'.$product_totals['order_shipping_amt'].'"/></span></td>
     </tr>
      <tr><td colspan="5"><span id="empty-message-cell"></span></td></tr>
      <tr>
       <td  class="">&nbsp;</td>
	   <td class="removable-column">&nbsp;</td>
       <td colspan="2" class="total-amt-container">order total</td>
       <td class="total-amt-container"><span class="cart-order-total align-right"><input type="text" name="order_total" class="form-control squish-form my-form-control" size="6" style="text-align: right;" readonly="readonly" value="'.$product_totals['order_total'].'"/></span></td>
     </tr>	 
	 ';

    ?>
</table>
</div> <!-- end of col -->
<div class="col-md-5">
<div id="billing-info">
<h4>Billing Information</h4>
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

<?php 
if(!isset($_SESSION['mac-cart']['loggedIn']) || (isset($_SESSION['mac-cart']['loggedIn']) && $_SESSION['mac-cart']['loggedIn'] != true)) { ?>
  <div class="form-group squish-form">
    <label class="col-sm-4 control-label" for="fname">first name</label>
    <div class="col-sm-8">
      <input id="fname" name="fname" class="form-control my-form-control" type="text" placeholder="enter first name" required value="<?= $formVars['fname']?>"  readonly="readonly"/>
    </div>
  </div>
  <div class="form-group squish-form">
    <label class="col-sm-4 control-label" for="lname"> last name</label>
    <div class="col-sm-8">
    <input id="lname" name="lname" class="form-control my-form-control" type="text" placeholder="enter last name"  required  value="<?= $formVars['lname']?>"   readonly="readonly"/>
    </div>
  </div>
  <div class="form-group squish-form">
    <label class="col-sm-4 control-label" for="address">address</label>
    <div class="col-sm-8">
    <input id="address" name="address" class="form-control my-form-control" type="text" placeholder="enter address"  required  value="<?= $formVars['address']?>"  readonly="readonly" />
    </div>
  </div>
  <div class="form-group squish-form">
    <label class="col-sm-4 control-label" for="state">city</label>
    <div class="col-sm-8">
    <input id="city" name="city" class="form-control my-form-control" type="text" placeholder="enter city"  required  value="<?= $formVars['city']?>"   readonly="readonly"/>
    </div>
  </div>
  <div class="form-group squish-form">
    <label class="col-sm-4 control-label" for="state">state</label>
    <div class="col-sm-8">
    <input id="state" name="state" class="form-control my-form-control" type="text" placeholder="enter state"  required  value="<?= $formVars['state']?>"  readonly="readonly"/>
    </div>
  </div>
  <div class="form-group squish-form">
    <label class="col-sm-4 control-label" for="zip">zip</label>
    <div class="col-sm-8">
    <input id="zip" name="zip" class="form-control my-form-control" type="text" placeholder="enter zip"  required  value="<?= $formVars['zip']?>"  readonly="readonly"/>
    </div>
  </div>
  <div class="form-group squish-form">
    <label class="col-sm-4 control-label" for="email">email</label>
    <div class="col-sm-8">
    <input id="email" name="email" class="form-control my-form-control" type="email" placeholder="enter email"  value="<?= $formVars['email']?>"  readonly="readonly"/>
    </div>
  </div>
  <div class="form-group squish-form">
    <label class="col-sm-4 control-label" for="cell_phone">phone</label>
    <div class="col-sm-8">
   <input id="phone" name="cell_phone" class="form-control my-form-control" type="text" placeholder="enter phone"  value="<?= $formVars['cell_phone']?>"  readonly="readonly"/>
    </div>
  </div>
  <div class="form-group squish-form">
    <label class="col-sm-4 control-label" for="preferred_payment_account">credit card</label>
    <div class="col-sm-8">
      <input id="preferred_payment_account" name="preferred_payment_account" class="form-control my-form-control" type="text"  required  value="<?= $formVars['preferred_payment_account']?>"  />
    </div>
  </div>

    <input type="hidden" name="user_type" id="user-type" value="clientManaged"/>
<?php } else if(isset($_SESSION['mac-cart']['loggedIn']) && $_SESSION['mac-cart']['loggedIn'] == true) { ?> 
  <div class="form-group">
    <label class="col-sm-4 control-label" for="user_id">user id</label>
    <div class="col-sm-8">
      <input id="user_id" name="user_id" class="form-control my-form-control" type="text" placeholder="enter user id" value="<?= $formVars['user_id']?>"   readonly="readonly"/>
    </div>
  </div>
  <div class="form-group squish-form">
    <label class="col-sm-4 control-label" for="fname">first name</label>
    <div class="col-sm-8">
      <input id="fname" name="fname" class="form-control my-form-control" type="text" placeholder="enter first name" required value="<?= $formVars['fname']?>"  readonly="readonly"/>
    </div>
  </div>
  <div class="form-group squish-form">
    <label class="col-sm-4 control-label" for="lname"> last name</label>
    <div class="col-sm-8">
    <input id="lname" name="lname" class="form-control my-form-control" type="text" placeholder="enter last name"  required  value="<?= $formVars['lname']?>"   readonly="readonly"/>
    </div>
  </div>
  <div class="form-group squish-form">
      <?php
	    if(!empty($myAccounts)) {
            echo '<label class="col-sm-4 control-label" for="payment_type">payment options</label>
                 <div class="col-sm-8">
                 <select id="preferred-payment-type" name="preferred_payment_type" class="form-control">';
            if($selected_preferred_payment_type == null) {    // if null then show 'select..' message
                echo '<option value="" selected="selected">select payment type</option>';
            }
			$selected = 'selected="selected"';
		    foreach($myAccounts as $account) {
               $account = (array)$account;
			   $doSelected = '';
			   if($selected_preferred_payment_type != '' && $selected_preferred_payment_type == $account['Name'])
			      $doSelected = $selected;
			   $formattedNumber = substr($account['Number'], 0, 4).'-'.substr($account['Number'], 4, 4).'-'.substr($account['Number'], 8, 4).'-'.substr($account['Number'], 12, 4);  
			   echo "<option value='$account[Name]' $doSelected>$formattedNumber : $account[Name]</option>";
		   }
           echo '</select>';
		}
	  ?>	

    </div>
   <input type="hidden" name="user_type" id="user-type" value="registered"/>
   <?php } ?>
  </div>
 <div style="margin-top: 10px;"><span class="align-right">
 <!--<input name="authenticate_order" type="button" value="authenticate_ajax" />--></span>
 <span class="align-right">
   <input name="authenticate_order2" type="submit" value="<?= $submitButtonText ?>" />
 </span>
   <input type="hidden" name="ClientId" id="ClientId" value="53a9a3d8faba33196cbabdf5"/> <!-- was 538ecc2c1c86333658d0806c -->
 </div>
 <div style="margin-top: 10px;"><span id="status_messages" class="align-right"></span></div>
</div> 
 </form>
 </div>  <!-- end of row -->
 <div class="row">
   <div id="auth-status" class="error-msg-red">&nbsp;
   </div>
  </div>  <!-- end of row -->
 </div>  <!-- end of container-fluid --> 
</div>
