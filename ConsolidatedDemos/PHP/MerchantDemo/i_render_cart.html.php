<div id="columnMain">
    <p class="page-header"><span>MAC Test - Shopping Cart</span><span class="align-right">
    <input name="clear_cart" type="button" value="Clear" /></span></p>
    <table id="cart-content" class="cart-content cart">
     <tr class=" ">
       <th class="image-thumb-container foto">&nbsp;</th><th class="foto">Product Number</th><th class="removable-column foto">Description</th><th class="foto">Unit Price</th><th class="foto">Qty</th><th class="foto">Total Price</th><th class="foto">Del</th>
     </tr>
    <?php
        if(!empty($products)) {
            foreach($products as $index => $product) {
                echo  '
    <tr id="cart'.$product['product_number'].'" class="cart-row">
     <td class="cart-cell image-thumb-container foto">
      <img src="pictures/'.$product['image_name'].'" class="cart-cell image-thumb" />
      </td>
     <td class="foto">'.
      $product['product_number'].
     '</td>
     <td class="removable-column foto">'.
      $product['description'].
     '</td>
     <td class="foto" > $<span class="cart-product-price">'.
      $product['price'].'</span>
     </td>
     <td class="foto">
      <input type="text" name="order_qty" value="'.$product['qty'].'"  maxlength="3" size="3" style="width: 80%; max-width: 40px; padding: 1px 1px 1px 2px;" class="form-control squish-form ">
     </td> 
	 <td class="foto">
	  <span class="cart-total-product-price align-right">'.$product['total_price'].'</span>
	 </td>
     <td class="foto">
       <img class="delete-cart-button text-bg" src="images/deletetrash.png" width="16" height="16"/>
     </td>  
      
    </tr>';
      } 
    }
    else  {
       echo '<tr class="cart-row"><td>cart is empty</td></tr>';
    }
    // output totals
    echo '
      <tr><td colspan="5"><span id="empty-message-cell"></span></td></tr>
      <tr>
       <td colspan="2" class="">&nbsp;</td>
	   <td class="removable-column">&nbsp;</td>
       <td colspan="2" class="total-amt-container"  style="border-top-style: solid; border-top-width: thin;">Total:</td>
       <td class="total-amt-container" style="border-top-style: solid; border-top-width: thin;"><span class="cart-order-total align-right">$</span></td>
     </tr>';

    ?>
</table>
<div id="errorMessage" class="error-msg-red">
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
<div style="margin-top: 10px;"><span class="align-right"><input name="check_out" type="button" value="Checkout" /></span>
   <input type="hidden" name="ClientId" id="ClientId" value="53a9a3d8faba33196cbabdf5"/> <!-- was 538ecc2c1c86333658d0806c -->
</div>
</div>