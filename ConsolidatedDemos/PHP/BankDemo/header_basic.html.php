<!DOCTYPE html>
<html lang="en">
  <head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="description" content="">
    <meta name="author" content="">

    <title>MAC Bank</title>

    <!-- Bootstrap core CSS -->
    <link href="bootstrap/css/testbootstrap_white_bank.css" rel="stylesheet">

    <!-- Custom styles for this template -->
    <link href="css/override_bootstrap.css" rel="stylesheet">
    <link href="css/mac.css" rel="stylesheet">
    <link href="css/table-style.css" rel="stylesheet">
  <!--   <link href="css/navbar_grey.css" rel="stylesheet">  -->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.0/jquery.min.js"></script>
    <script src="bootstrap/js/bootstrap.min.js"></script>
    <script src="js/functions.js" ></script>
    <script src="js/macEvents.js"></script>
    <script src="js/jquery.validate.min.js"></script>
    <script src="js/formatter.js"></script>
   <!-- Just for debugging purposes. Don't actually copy this line! -->
    <!--[if lt IE 9]><script src="../../assets/js/ie8-responsive-file-warning.js"></script><![endif]-->

    <!-- HTML5 shim and Respond.js IE8 support of HTML5 elements and media queries -->
    <!--[if lt IE 9]>
      <script src="https://oss.maxcdn.com/libs/html5shiv/3.7.0/html5shiv.js"></script>
      <script src="https://oss.maxcdn.com/libs/respond.js/1.4.2/respond.min.js"></script>
    <![endif]-->
  </head>

  <body>

    <div class="row980">
        <!--<div style="background: url('images/MAC-Logo-150.jpg') no-repeat left center;height:81px;text-align:right;padding-top: 2rem;margin-left:0.75rem;">-->
            <!--<img src="images/mac-logo_new.png" width="203" height="81" alt="mac">-->
            <!--<img src="images/mac-logo_new.png" style="width:155px;" alt="mac" />-->
            <!--<h1 style="margin:0;font-weight:normal;font-size:3.5rem;color:#272a2f;opacity:0.25;" class="banner-visibility">Online Banking Demo</h1>
        </div>-->
        <div style="height:36px;text-align:right;">
            <?php 
	          $welcomeText = '';           // default
	          $loginButtonText = 'My Account';  // default
	          $login_class = "login-class";
	          if(isset($_SESSION['mac-bank']['loggedIn']) && $_SESSION['mac-bank']['loggedIn'] == true) {
		          $welcomeText = 'Welcome <strong>'.$_SESSION['mac-bank']['fname'].'</strong>';
		          $loginButtonText = 'logout';
		          $login_class = "logout-class";
                  echo  '<div id="login-container" class="login-box1">'.$welcomeText.'&nbsp;&nbsp;<div class="login-button1 text-bg-profile '.$login_class.'"><span style="margin-left: 18px;">'.$loginButtonText.'</span></div></div>';
	          }
	          ?>
        </div>
    </div>
    <div class="cboth"></div>

    <div class="navbar navbar-default" role="navigation" style="background: #fff url('images/bg_navbar.png') repeat-x center center;height:79px;z-index:1000000 !important;margin-bottom:0;">
      <div class="container-nav">
        <div class="navbar-header">
          <button type="button" id="mobileMenu" class="navbar-toggle" data-toggle="collapse" data-target=".navbar-collapse" title="Menu">
            <span class="sr-only">Toggle navigation</span>
            <span class="icon-bar"></span>
            <span class="icon-bar"></span>
            <span class="icon-bar"></span>
          </button>
        </div>
        <div class="collapse navbar-collapse" style="">
          <ul class="nav navbar-nav" style="float: right;list-style-position: inside;">
          <?php
          if(isset($_SESSION['mac-bank']['loggedIn']) && $_SESSION['mac-bank']['loggedIn'] == true) {
            echo '
             <li><a href="index.php?action=main"><span id="menu_home">HOME</span></a></li>
             <li><a href="index.php?action=accounts"><span id="menu_accounts">ACCOUNTS</span></a></li>
             <li><a href="index.php?action=log"><span id="menu_log">LOG</span></a></li>
             <li><a href="index.php?action=transfer"><span id="menu_transfers">TRANSFERS</span></a></li>
             <li><a href="index.php?action=funds"><span id="menu_transactions">TRANSACTIONS</span></a></li>
             <li><a href="index.php?action=bills"><span id="menu_payments">PAYMENTS</span></a></li>';
          }
           else { 
               //echo '<li><a href="index.php?action=main">HOME</a></li>';
               echo '<li></li>';
          }

          ?>  
          </ul>
        </div><!--/.nav-collapse -->
      </div>
    </div>
    <div id="shadow" style="display: block;width: 100%;height: 4px;background: url(images/shadow.png) repeat-x top left;"></div>

    <div class="container-body">

