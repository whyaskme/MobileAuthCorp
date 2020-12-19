<?php
// redisplay because of errors
if(isset($_SESSION['m-notes']['validateMsg'])) {
    $errorArray = $_SESSION['m-notes']['validateMsg'];
}
if(isset($_SESSION['m-notes']['forgotuserid']))
   $useridSaved = $_SESSION['m-notes']['forgotuserid'];
include "mainBanner.html.php";

if(isset($_SESSION['m-notes']['loggedIn']) && $_SESSION['m-notes']['loggedIn'] == true && $_SESSION['m-notes']['shared_userid'] == 1) {
    include 'menuSharedUser.html.php';
    include 'forgot_shared.html.php';
 }
 else { 
    include 'menuMain.html.php';
    include "forgot_passwordForm.html.php";
 }
unset( $_SESSION['m-notes']['validateMsg']);
unset( $_SESSION['m-notes']['forgotuserid']);
$useridSaved = null;
?>
<script type="text/javascript">

  $(document).ready(function() {

     // focus to input field
     $('#userid').focus();
     
 });
</script>