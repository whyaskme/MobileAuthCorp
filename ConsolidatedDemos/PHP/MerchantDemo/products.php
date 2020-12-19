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
  $products = $pd->getProducts();
  
  include_once 'header_basic.html.php';   // uses $cart_count
  include 'i_render_products.html.php';
  include 'footer_basic.html';
?> 
<script>
  $(document).ready(function() {
      // click events
      
      // add to cart
      $('.item-thumb-button').on('click', function(ev) {
            var id = ev.currentTarget.id;
            var desc = $(ev.currentTarget.parentNode.parentNode).find('.item-thumb-description').html();
            $('#adding-to-cart').remove();
            $('body').append('<div id="adding-to-cart" class="fading-popup"><strong>confirmation</strong> -<br />adding <strong>"'+desc.substr(0, 15)+'..."</strong> to cart</div>');
            $('#adding-to-cart').fadeOut(3500)            
            macCart.addToCart(id);
      });
  });

</script>
