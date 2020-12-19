<?php
  include 'includes/random_image2.php';
  
  session_start();
  
  //  get new randmom string, output image
  $randomImage = new RandomImage();
  $randomImage->setImageText();
  $randomImage->outImage();
  
?>
