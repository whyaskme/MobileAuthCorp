<div id="columnMain">
    <p class="page-header">MAC Test Golf Store</p>
<div id="cart-content" class="cart-content">
    <?php
        if(!empty($products)) {
            foreach($products as $index => $product) {
                echo  '
    <div class="item-thumb">
     <div class="image-thumb-container">
      <img src="pictures/'.$product['image_name'].'" class="image-thumb" />
      </div>
     <div class="item-thumb-number">'.
      $product['product_number'].
     '</div>
     <div class="item-thumb-description">'.
      $product['description'].
     '</div>
     <div class="item-thumb-price"> $'.
      $product['price'].
     '</div>
     <div class="item-thumb-button-container">
      <button id="add'.$product['product_number'].'" class="item-thumb-button">add to cart</button>
     </div>    
      
    </div>';
     }
    }
    ?>
</div>
</div>
