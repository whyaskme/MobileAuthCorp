// mac events

$(document).ready(function() {
    
    $('#login-container .login-class').click(function(ev) {
        window.location = 'index.php?action=loginform';
    });
    
    $('#login-container .logout-class').click(function(ev) {
        window.location = 'index.php?action=logout';
    })
    
    $('#cart-count span.text-bg-cart').click(function(ev) {
        window.location = 'index.php?action=cart';
    });
    
    $('#refresh-otp').click(function(ev) {
        window.location = 'index.php?action=otpform&refresh=yes';
    });
    
    $('input[name="submit_email"]').click(function(ev) {
        var email = $('input[name="email_list"]').val();
        if(email == '') {
            alert('please enter email');
        }
        else {
            alert('thank you: '+email);
        }
        return false;
    });
    
    $('#privacy-policy').click(function(ev) {
        alert('thank you');
    });
    
    
});