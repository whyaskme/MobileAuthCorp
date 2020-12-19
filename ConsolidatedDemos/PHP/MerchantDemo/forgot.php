<?php

 $returnvalue = 0;
 if(!empty($formVars["userid"]))  { 
   $_SESSION['m-notes']['forgotuserid'] = $formVars['userid'];  
   if($formVars['imagetext'] != '' && $_SESSION['image_random_value'] == md5($formVars['imagetext']) ) {
 
       // connect to database
     $user = new User_DB();
      // validate user in database
     if($user->usermatch($formVars['userid'])) {
               $user->set_userid($formVars['userid']);
               $user->set_sessionid($_COOKIE['PHPSESSID']);
               $theEmail = $user->get_email();
               // generate random verification key and update db
               $password_reset_code = random_string(20);
               $user->updateuserfield('password_reset_code', $password_reset_code);
               $user->updateuserfield('password_reset_status', 1);
               
			   unset($_SESSION['m-notes']);  // clear out session and start over
               include_once 'forgotThanks.php';
               exit();
	 }
     else
	    $_SESSION['m-notes']['validateMsg'][] = "User id does not exist!";
  }
  else
    $_SESSION['m-notes']['validateMsg'][] = "Your entry did not match Image";
 }
 else
      $_SESSION['m-notes']['validateMsg'][] = "Please enter User id!";    
    
 include 'forgot_passwordForm.php';

?>