<?php
//session_start();

  include_once 'includes/functions.php';
  include_once 'includes/product_class.php';
  
  
  // save for re-print
  $products = $_SESSION['mac-cart']['save-order']['products'];
  $product_totals = $_SESSION['mac-cart']['save-order']['product_totals'];
  $billingArray = $_SESSION['mac-cart']['save-order']['billingArray'];
  $errorArray =  $_SESSION['mac-cart']['save-order']['errorArray'];
  $messageArray = array();
  
  // i_render_verify.html.php has two sections - one for 'client managed', one for 'registered' user
  include 'header_none.php';
  include 'i_render_order_print.html.php';       // uses $products, product_totals, billingArray, errorArray, messageArray
  
  //unset($_SESSION['mac-cart']['save-order']);
?> 
<script>
  $(document).ready(function() {
          
      $('#print-me').click(function(ev) {
          window.print();
      });
  });


</script>

