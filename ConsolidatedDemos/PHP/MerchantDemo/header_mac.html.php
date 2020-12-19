<!DOCTYPE html>
<html lang="en">
  <head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="description" content="">
    <meta name="author" content="">

    <title>MAC Test Shopping Cart</title>

    <!-- Bootstrap core CSS -->
    <link href="bootstrap/css/testbootstrap.css" rel="stylesheet">

    <!-- Custom styles for this template -->
    <link href="css/override_bootstrap.css" rel="stylesheet">
    <link href="css/mac.css" rel="stylesheet">
  <!--   <link href="css/navbar_grey.css" rel="stylesheet">  -->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.0/jquery.min.js"></script>
    <script src="bootstrap/js/bootstrap.min.js"></script>
    <script src="js/functions.js"></script>
    <script src="js/macCart.js"></script>
    <script src="js/macCartEvents.js"></script>
    <script src="js/jquery.validate.min.js"></script>
    <!-- Just for debugging purposes. Don't actually copy this line! -->
    <!--[if lt IE 9]><script src="../../assets/js/ie8-responsive-file-warning.js"></script><![endif]-->

    <!-- HTML5 shim and Respond.js IE8 support of HTML5 elements and media queries -->
    <!--[if lt IE 9]>
      <script src="https://oss.maxcdn.com/libs/html5shiv/3.7.0/html5shiv.js"></script>
      <script src="https://oss.maxcdn.com/libs/respond.js/1.4.2/respond.min.js"></script>
    <![endif]-->
</head>

<body>



<div class="hero-wrap hero-wrap-lg">
	<div id="hero">
    	<h3>Discover</h3>
        <h1>Best Golf Equipment</h1>
		<h2>at Scottsdale Golf Store</h2>
        <p>&nbsp;</p>
    </div>  
    <img class="hero_img" src="images/hero.jpg" />
    <div class="cboth"></div>     
</div>
<div class="navbar navbar-default" role="navigation">
      <div class="container-nav">
  <div class="cart-count" id="cart-count" style="float: right; top: 94px; width: 81px;"><span class="text-bg-cart"><span style="margin-left:18px;">Cart:</span></span><span class="cart-count-number"> <?= $cart_count ?></span></div>
        <div class="navbar-header">
          <button type="button" class="navbar-toggle" data-toggle="collapse" data-target=".navbar-collapse">
            <span class="sr-only">Toggle navigation</span>
            <span class="icon-bar"></span>
            <span class="icon-bar"></span>
            <span class="icon-bar"></span>
          </button>
          <a href="index.php?action=main" class="navbar-brand mac-navbar-logo"></a>
        </div>
        <div class="collapse navbar-collapse">
          <ul class="nav navbar-nav">
            <li><a href="index.php?action=products">Shopping</a></li>
            <li><a href="index.php?action=cart">View Cart</a></li>
          </ul>
        </div><!--/.nav-collapse -->
      </div>
    </div>
    <?php 
	  $welcomeText = '';           // default
	  $loginButtonText = 'My Account';  // default
	  $login_class = "login-class";
	  if(isset($_SESSION['mac-cart']['loggedIn']) && $_SESSION['mac-cart']['loggedIn'] == true) {
		  $welcomeText = 'welcome <strong>'.$_SESSION['mac-cart']['fname'].'</strong>';
		  $loginButtonText = 'logout';
		  $login_class = "logout-class";
	  }
	  echo  '<div id="login-container" class="login-box">'.$welcomeText.'&nbsp;&nbsp;<div class="login-button text-bg-profile '.$login_class.'"><span style="margin-left: 18px;">'.$loginButtonText.'</span></div></div>';
	  ?>

    <div class="container-body">

