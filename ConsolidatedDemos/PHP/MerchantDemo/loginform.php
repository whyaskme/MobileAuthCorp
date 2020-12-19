<?php
  if(isset($_SESSION['mac-cart']['cart'])) 
      $cart_count = count($_SESSION['mac-cart']['cart']);
  else
      $cart_count = 0;     
  
  // load login page
  
  include_once 'header_basic.html.php';   // uses $cart_count
  include 'i_login_form.html.php';
  include 'footer_basic.html';
    
  ?> 
<script>
$(document).ready(function() {
      // on load
      $('input[name="user_id"]').focus();
     
      // initialize variables for submit
      var requestUrl = "<?php echo $requestUrl; ?>";

      $('#login-submit-button').click(function(ev) {
          var user_id = $('#user_id').val();   // get entered user id
          $('#form-status').show();
          $('#login-submit-button').prop('disabled', true);
          setTimeout(function() {    // wait 2 seconds
              return true;
              }, 2000);
          window.location = "index.php?action=login&user_id="+user_id;    
      });  
      
});

</script>
