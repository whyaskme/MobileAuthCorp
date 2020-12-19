<?php
  if(isset($_SESSION['mac-cart']['cart'])) 
      $cart_count = count($_SESSION['mac-cart']['cart']);
  else
      $cart_count = 0;     
  $path = pathinfo(__FILE__);    
  $_SESSION['mac-cart']['page-name'] = $path['basename'];
  
  include_once 'includes/product_class.php';
  
  // get product data
  $pd = new ProductClass();
  $products = '';
 
  if(isset($_SESSION['mac-cart']['cart'])) {
      foreach($_SESSION['mac-cart']['cart'] as $index =>$productInfo) {
          $condition['product_number'][] = $productInfo['product_number'];
      }
      $products = $pd->getProducts($condition);
      foreach($products as $pindex => $product) {
          $prodNum = $product['product_number'];
          $qty = 0;
          foreach($_SESSION['mac-cart']['cart'] as $index => $cartProduct) {
              if($cartProduct['product_number'] == $prodNum)
                 $qty = $cartProduct['qty'];
          }
          $products[$pindex]['qty'] = $qty;
          $total = ($products[$pindex]['price']*100) * $qty;
          $products[$pindex]['total_price'] = number_format(($total/100), 2, '.', ',');
      }
   }
   
 
  include_once 'header_basic.html.php';     // uses $cart_count
  include 'i_render_cart.html.php';
  include 'footer_basic.html';
?> 
<script>
  $(document).ready(function() {
      
    // add up cart for total  on page load
     var theCart = $('table.cart-content tr.cart-row');
     var total = 0;
     $.each(theCart, function(index, obj) {
        var amt = parseFloat($(obj).find('.cart-total-product-price').text().replace(/[^\d\.]/g,''));
        total = total + amt;
        
     }); 
     if(isNaN(total))
        total = 0;
     $('.cart-order-total').text('$'+parseFloat(total).toFixed(2));
      
      // click events
      $('input[name="order_qty"]').on('change',function(ev) {
            var ID = ev.currentTarget.parentNode.parentNode.id; // ID is whole 'id'
            if(ID.substr(0, 4) != 'cart')
               return false;  
            else
               var id = ID.substr(4);                    // id is product number
            var newval = $('#cart'+id + ' input').val();    // get new qty
            if(newval == '0' || newval == '00' || newval == '') { // delete item with qty zero?
            var r = confirm('exclude this product from order?');
            if(r == true) {
                newval = 0;
            }
            else {
                $('#'+ID + ' input').val('1')
                newval = 1;
            }
        }
            // add up total on page   
            var total = macCart.calcOrderTotal(ID, newval);     
            $('.cart-order-total').text('$'+parseFloat(total).toFixed(2));
            
             // save updated qty in SESSION
            macCart.saveCart(ID, newval);   
                             
     });
     
     $('input[name="order_qty"]').on('click',function(ev) {
         $(this).select();
         this.setSelectionRange(0, 9999);
         return false;
     });
     
     // remove product from cart
     $('.delete-cart-button').on('click', function(ev) {
            var ID = ev.currentTarget.parentNode.parentNode.id;
            if(ID.substr(0, 4) != 'cart')
               return false; 
            else {
                // call remove function
                macCart.removeFromCart(ID);
                return false;
            }
     });
     
     // clear cart
     $('input[name="clear_cart"]').on('click', function(ev) {
           macCart.removeFromCart();  // no parameters = remove all
           return false;
     });
     
     // check out
     $('input[name="check_out"]').click(function(ev) {
           window.location = 'index.php?action=checkout';
     });       
      
  });


</script>
