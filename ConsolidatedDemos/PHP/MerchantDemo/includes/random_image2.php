<?php
if(!class_exists('RandomImage')) {
    
class RandomImage{
    
    
     // Length of challenge string 
  private $CHALLENGE_STRING_LENGTH = 5;

    // Characters that will be used in challenge string
  private $CHALLENGE_STRING_LETTERS = 'ABCDEFGHKLMNPQRTUWXY3678abdefghmnrt#$&+?';

    // Name of session variable that will be used by the script.
    // You shouldn't need to change this unless it collides with a
    // session variable you are using.  
  private $CHALLENGE_STRING_SESSION_VAR_NAME = 'image_random_value';

    // Font size of challenge string in image
  private $CHALLENGE_STRING_FONT_SIZE = 5;

    // Whether background pattern is enabled
  private $CHALLENGE_BACKGROUND_PATTERN_ENABLED = TRUE;

    // Font size of characters in background pattern
  private $CHALLENGE_BACKGROUND_STRING_FONT_SIZE = 1;

    // Whether image should alternate between dark-on-light and 
    // light-on-dark
  private $CHALLENGE_ALTERNATE_COLORS = TRUE;

    // How much padding there should be between the edge of the image
    // and the challenge string bounds
  private $CHALLENGE_STRING_PADDING = 4;    // in pixels

    // Whether the entered verification code should be converted to upper-case.
    // In effect, this makes the verification code case-insensitive.
  private $CHALLENGE_CONVERT_TO_UPPER = TRUE;
  
  private $challeng_string;
  
  function __construct() {
     
     $challenge_string = "";
          
 }
 
  public function setImageText() {
      // Create string from random characters in list of valid characters
     for($i = 0; $i < $this->CHALLENGE_STRING_LENGTH; $i++) {
         $this->challenge_string .= $this->pickNextChar(); 
     }
     
     // Store challenge string in session
     $_SESSION[$this->CHALLENGE_STRING_SESSION_VAR_NAME] = md5($this->challenge_string);
  }
  
  public function compareImageText($text) {
     if($_SESSION[$this->CHALLENGE_STRING_SESSION_VAR_NAME] == md5(trim($text)))  
         return true;
     else
         return false;    
      
  }
     
  public function pickNextChar() {
      return substr($this->CHALLENGE_STRING_LETTERS, (rand() % strlen($this->CHALLENGE_STRING_LETTERS)), 1);
 }
 
  public function outImage() {
     
     // Create a challenge string
     if($_SESSION[$this->CHALLENGE_STRING_SESSION_VAR_NAME] === FALSE) { return FALSE; }
     
     // Set content type
     header("Content-type: image/png");

     // Get character sizes and string sizes
    $char_width = imagefontwidth($this->CHALLENGE_STRING_FONT_SIZE);
    $char_height = imagefontheight($this->CHALLENGE_STRING_FONT_SIZE);
    $string_width = $this->CHALLENGE_STRING_LENGTH * $char_width;
    $string_height = 1 * $char_height;
         
    // Create image and get color
    $img_width = $string_width + $this->CHALLENGE_STRING_PADDING * 2;
    $img_height = $string_height + $this->CHALLENGE_STRING_PADDING * 2;     
     $img = @imagecreatetruecolor($img_width, $img_height)
       or die("imagecreatetruecolor failed");

    // Pick colors
    if($this->CHALLENGE_ALTERNATE_COLORS === FALSE || rand(0, 1) == 0) {
         $background_color = imagecolorallocate($img, 15, 15, 15);
         $text_color = imagecolorallocate($img, 238, 238, 238);
         $bg_text_color = imagecolorallocate($img, 95, 95, 95);
    } else {
         $background_color = imagecolorallocate($img, 238, 238, 238);
         $text_color = imagecolorallocate($img, 15, 15, 15);
         $bg_text_color = imagecolorallocate($img, 191, 191, 191);
    }

     // Fill background
     imagefill($img ,0, 0, $background_color);
     
     // Draw background text pattern
     if($this->CHALLENGE_BACKGROUND_PATTERN_ENABLED === TRUE) {
        $bg_char_width = imagefontwidth($this->CHALLENGE_BACKGROUND_STRING_FONT_SIZE);
        $bg_char_height = imagefontheight($this->CHALLENGE_BACKGROUND_STRING_FONT_SIZE);
         for($x = rand(-2, 2); $x < $img_width; $x += $bg_char_width + 1) {
             for($y = rand(-2, 2); $y <  $img_height; $y += $bg_char_height + 1) {
                 imagestring($img, $this->CHALLENGE_BACKGROUND_STRING_FONT_SIZE, $x, 
                    $y, $this->pickNextChar(), $bg_text_color);
             }
         }
     }

     // Draw text
     $x = $this->CHALLENGE_STRING_PADDING + rand(-2, 2);
     $y = $this->CHALLENGE_STRING_PADDING + rand(-2, 2);
     for($i = 0; $i < strlen($this->challenge_string); $i++) {
        imagestring($img, $this->CHALLENGE_STRING_FONT_SIZE, $x, 
            $y  + rand(-2, 2), substr($this->challenge_string, $i, 1), $text_color);
        $x += $char_width;
     }
      
    // Output image
    imagepng($img);
    
    // Release image resources
    imagedestroy($img);
     
  }
 }  // end of class
}  // end of class_exists


?>
