// mac events

$(document).ready(function() {
    
    $('#login-container .login-class').click(function(ev) {
        window.location = 'index.php?action=loginform';
    });
    
    $('#login-container .logout-class').click(function(ev) {
        window.location = 'index.php?action=logout';
    })
    
    $('#refresh-otp').click(function(ev) {
        window.location = 'index.php?action=otpform&refresh=yes';
    });
    
    function closeMessage() {
        $(this).hide();
    }

});

