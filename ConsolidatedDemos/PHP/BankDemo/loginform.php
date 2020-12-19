<?php
   include_once 'header_basic.html.php';   
   include 'i_login_form.html.php';
   include 'footer_mac.html.php';
?> 
<script>
$(document).ready(function() {
    // on load
      $('input[name="user_id"]').focus();

      // initialize variables for submit
      var requestUrl = "<?php echo $requestUrl; ?>";

      $('#login-submit-button').click(function(ev) {
          var user_id = $('#user_id').val();   // get entered user id
          //$('#form-status').show();
          ShowProcessingMessage();
          $('#login-submit-button').prop('disabled', true);
          setTimeout(function() {    // wait 2 seconds
              return true;
              }, 2000);
          window.location = "index.php?action=login&user_id="+user_id;    
      });  

});

</script>
