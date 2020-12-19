<?php
  if(isset($_SESSION['mac-bank']['cart'])) 
      $cart_count = count($_SESSION['mac-bank']['cart']);
  else
      $cart_count = 0;     
  $path = pathinfo(__FILE__);    
  $_SESSION['mac-bank']['page-name'] = $path['basename'];

  include_once 'includes/functions.php';
  
  // set up Registration URL if needed
  $requestUrl = $_SESSION['mac-bank']['config']['registerUrl'];  //  build url if not logged in
  $params = '?action=reg&demo='.urlencode($_SESSION['mac-bank']['config']['clientName']);
  $params .= '&cid='.$_SESSION['mac-bank']['config']['clientId'];
  $registerUrl .= $requestUrl.$params;

  //$registerUrl =  $_SESSION['mac-bank']['config']['registerUrl'];   // use on render pages

  if(isset($_SESSION['mac-bank']['loggedIn']) && $_SESSION['mac-bank']['loggedIn'] == true) {
      
     include_once 'header_basic.html.php';   // uses $_SESSION to determine logged in state
     include 'i_render_main.html.php';
     include 'footer_mac.html.php';
  }
  else { // not logged in yet
     include_once 'header_basic.html.php';   
     include 'i_login_form.html.php';    // show login form
     include 'footer_mac.html.php';
  }
  
?> 
<script>
$(document).ready(function() {
      // on load
      $('input[name="user_id"]').focus();

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
      
      // click event for Registration
      $('#register-button').click(function (ev) {
            var ln = $('#user_id').val();     // get requested id
            // is it an email address
            var status = isEmail(ln);
            if(status != true) {
                $('#errorMessage').text('please enter a valid email address');
                return false;
            }
            ln = StrToHex(ln);
            var registerUrl = "<?php echo $registerUrl; ?>";
            window.location = registerUrl + '&ln=' + ln;
      });
    var c1 = $("#col1").height();
    $("#col2").height(c1);
    $("#col3").height(c1);
    $(window).resize(function () {
        var c1 = $("#col1").height();
        $("#col2").height(c1);
        $("#col3").height(c1);
    });
    
});

</script>