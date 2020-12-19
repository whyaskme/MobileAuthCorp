
(function($) {
     // update time parameter of image - causes new load
     function refreshRandomImage() {
         var c_currentTime = new Date();
         var c_miliseconds = c_currentTime.getTime();

         document.getElementById('random-image').src = 'outImage.php?x='+ c_miliseconds;
     }


    $(document).ready( function() {
    // click event for random image
        $("#refresh-random-image").click(
             function(ev) {
                refreshRandomImage();
        });
        
    });
}(jQuery) );
    