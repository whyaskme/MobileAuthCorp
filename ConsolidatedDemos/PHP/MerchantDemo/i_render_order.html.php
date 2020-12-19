<div id="columnMain">
    <p class="page-header">MAC Test - Order Confirmation<span id="printer-friendly" class="align-right text-bg-simple">Printer Friendly</span> </p>
<div class="container-fluid"> 
  <div class="row">   
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
     <td class="foto" >'.
      $product['product_number'].
     '</td>
     <td class="removable-column foto">'.$product['description'].'</td>
     <td class="foto" > $<span class="cart-product-price">'.
      $product['price'].'</span>
     </td>
     <td  class="foto">'
       .$product['qty'].
     '</td> 
	 <td class="foto">
	  <span class="cart-total-product-price align-right">'.$product['total_price'].'</span>

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
       <td class="total-amt-container"  style="border-top-style: solid; border-top-width: thin;"><span class="cart-order-product-total align-right">'.$product_totals['order_product_total'].'</span></td>
     </tr>
      <tr><td colspan="5"><span id="empty-message-cell"></span></td></tr>
      <tr>
       <td  class="">&nbsp;</td>
	   <td class="removable-column">&nbsp;</td>
       <td colspan="2" class="total-amt-container">shipping amt</td>
       <td class="total-amt-container"><span class="cart-order-shipping-total align-right">'.$product_totals['order_shipping_amt'].'</span></td>
     </tr>
      <tr><td colspan="5"><span id="empty-message-cell"></span></td></tr>
      <tr>
       <td  class="">&nbsp;</td>
	   <td class="removable-column">&nbsp;</td>
       <td colspan="2" class="total-amt-container">order total</td>
       <td class="total-amt-container"><span class="cart-order-total align-right">'.$product_totals['order_total'].'</span></td>
     </tr>';	 
?>
</table>
</div> <!-- end of col -->
<div class="col-md-5">
<div id="billing-info">
<h4>Billing Confirmation</h4>
 <div class="billing-content">
  <div class="row">
    <div class="col-sm-4 bold-text" >name:</div>
    <div class="col-sm-8"><?= $billingArray['fname'] . ' ' . $billingArray['lname'] ?></div>
  </div>  
  <div class="row">
    <div class="col-sm-4 bold-text">address:</div>
    <div class="col-sm-8"><?= $billingArray['address'] ?></div>
  </div>  
  <div class="row">
    <div class="col-sm-4 bold-text" for="address">city, st zip:</div>
    <div class="col-sm-8"><?= $billingArray['city'] . ' ' . $billingArray['state'] .' ' .$billingArray['zip'] ?></div>
  </div>  
  <div class="row">
    <div class="col-sm-4 bold-text">account:</div>
    <div class="col-sm-8"><?= $billingArray['account'] ?></div>
  </div>

 </div>
 </div>  <!-- end of col -->
<div id="errorMessage"  style="margin-top: 15px;" class="error-msg-red">
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
<div id="successMessage"  style="margin-top: 15px;" class="msg-green">
<?php   /*  if(isset($messageArray) && !empty($messageArray)) {
   $counter = 0;
   foreach($messageArray as $msg) {
         if($counter > 0)
            echo '<br />';
         echo $msg;
         $counter++;
     }
 }  */
?>
</div>
 </div>  <!-- end of row -->
  </div>  <!-- end of row -->
 </div>  <!-- end of container-fluid --> 
</div>
