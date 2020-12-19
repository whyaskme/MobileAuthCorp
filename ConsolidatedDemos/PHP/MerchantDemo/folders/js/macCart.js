// mac cart javascrip

var macCart = {
    
    cartCount: 0,
        
    addToCartSuccess: function(data, statusText, request){

        try {
            if(data.status == true) {  
               if(data.message == 'added') {  
                   // update cart item count if added new item, not if added duplicate
                   macCart.cartCount = $('#cart-count .cart-count-number').text();
                   macCart.cartCount++;
                   $('#cart-count .cart-count-number').text(' '+ macCart.cartCount);  
               }
            }
            else if(data.status == false) {  
           }

        }
        catch(ex) {
           alert("status: " + status, '');
        }
        return false;
    },
    
    // add new item to cart
    addToCart: function(id) {
       
       if(id.substr(0, 3) != 'add')  {
          alert('invalid item number');
          return false;
       } 
       var product_number = id.substr(3); 
        
       var request = {type: 'add_to_cart',   // set request data
                      product_number: product_number
                     }

        var status = jQuery.ajax({
                       type: 'POST',
                       url: 'cartAjax.php',
                       data: request,
                       dataType: 'json',
                       success: this.addToCartSuccess,
                       error: this._error,
                       timeout: 10000
                       });
    },
    
    // save qty for item if updated by user
    saveCart: function(id, qty) {
        if(id.substr(0, 4) != 'cart')
             return false;
       var product_number = id.substr(4); 
       var request = {type: 'save_cart',
                      product_number: product_number,
                      qty: qty
                     }
                     
       var status = jQuery.ajax({
                     type: 'POST',
                     url: 'cartAjax.php',
                     data: request, 
                     dataType: 'json',
                     success: this.saveCartSuccess,
                     error: this._error,
                     timeout: 10000
                    });
    },                      

   _success: function(data, statusText, request){

        try {
            if(data.status == true) {    // found a user
            }
            else if(data.status == false) {  // no user, so allow to create one
               alert(data.message);
           }

        }
        catch(ex) {
           alert("status: " + status, '');
        }
        return false;
    },
    
    removeFromCart: function(id) {
       var product_number = '';           // initialize
       if(id == undefined || id == null)
           id = null;
       else {
           if(id.substr(0, 4) != 'cart')  {
              alert('invalid item number');
              return false;
           } 
       }
        
       var request = {type: 'remove_from_cart',   // get event data
                      id: id        // 'cart'+product_number
                     }

       var status = jQuery.ajax({
                       type: 'POST',
                       url: 'cartAjax.php',
                       data: request,
                       dataType: 'json',
                       success: this.removeFromCartSuccess,
                       error: this._error,
                       timeout: 10000
                       });
    },
    
    removeFromCartSuccess: function(data, statusText, request){

        try {
            if(data.status == true  
                  && data.data.id != undefined
                  && data.data.id.length > 0) {    // deleting a single cart product
                macCart.cartCount = $('#cart-count .cart-count-number').text();
                macCart.cartCount--;
                var productTotal = parseFloat($('#'+data.data.id+' .cart-total-product-price').text());
                var oldTotal = $('.cart-order-total').text();
                oldTotal = oldTotal.replace('$', '');
                var newTotal = parseFloat(oldTotal) - parseFloat(productTotal);
                $('.cart-order-total').text('$'+newTotal.toFixed(2));
                
                $('#cart-count .cart-count-number').text(' '+ macCart.cartCount); 
                $('#'+data.data.id).remove(); 
            }
            else if(data.status == true) {                    // delete whole cart
                macCart.cartCount = 0;
                $('#cart-count .cart-count-number').text(' '+ macCart.cartCount); 
                $('tr.cart-row').remove(); 
                $('#empty-message-cell').text('cart is empty');
                var zero = 0;
                $('.cart-order-total').text('$'+zero.toFixed(2));
            }           
            else if(data.status == false) {  // no user, so allow to create one
                alert('problem removing cart');
            }

        }
        catch(ex) {
           alert("status: " + status, '');
        }
        return false;
    },
    
    calcOrderTotal: function(ID, newqty) {
        // ID is element id - 'cart12222', etc
        // calc total for this item
        var price = $('#'+ID+' span.cart-product-price').text();  // get price 
        var total = price * newqty;                                     // new total for item
        $('#'+ID+' span.cart-total-product-price').text(total.toFixed(2)); // put back in dom
                            
        // add up whole cart 
         var theCart = $('table.cart-content tr.cart-row');
         var total = 0;
     
         $.each(theCart, function(index, obj) {
            var amt = parseFloat($(obj).find('.cart-total-product-price').text());
            total = total + amt;
            
         }); 
         return total;
    },

    validateOrder: function() {
      // get input data
      var bill_fname = $('input[name="bill_fname"]').val();
      var bill_lname = $('input[name="bill_lname"]').val();
      var bill_address = $('input[name="bill_address"]').val();
      var bill_city = $('input[name="bill_city"]').val();
      var bill_state = $('input[name="bill_state"]').val(); 
      var bill_zip = $('input[name="bill_state"]').val();
      var bill_email = $('input[name="bill_email"]').val();
      var bill_phone = $('input[name="bill_phone"]').val();
      var payment_type = $('#payment-type').val();
      var product_total = $('.total-amt-container span.cart-order-product-total').text();
      var shipping_total = $('.total-amt-container span.cart-order-shipping-total').text();
      var order_total = $('.total-amt-container span.cart-order-total').text();
      
      // save input data to SESSION
      var request = {type: 'save_temp_validation_data',   // get event data
                      bill_fname: bill_fname,
                      bill_lname: bill_lname,
                      bill_address: bill_address,
                      bill_city: bill_city,
                      bill_state: bill_state,
                      bill_zip: bill_zip,
                      bill_email: bill_email,
                      bill_phone: bill_phone,
                      payment_type: payment_type,
                      product_total: product_total,
                      shipping_total: shipping_total,
                      order_total: order_total        // 
                     }

       var status = jQuery.ajax({
                       type: 'POST',
                       url: 'cartAjax.php',
                       data: $('#billing_information').serialize(),
                       dataType: 'json',
                       success: this.saveValidationDataSuccess,
                       error: this._error,
                       timeout: 10000
                       });
     },
      
   saveValidationDataSuccess: function(data, statusText, request){

        try {
            if(data.status == true && data.data != undefined) {    // saved OK
            // now validate at OTP
            }
            else if(data.status == false) {  // no user, so allow to create one
               alert(data.message);
           }

        }
        catch(ex) {
           alert("status: " + status, '');
        }
        return false;
    },
      
      
      ///////
      
      prepareAuthenticate: function (ev) {
          
          var client_id = "53a9a3d8faba33196cbabdf5";
          var bill_phone = $('input[name="bill_phone"]').val();
          var bill_email = $('input[name="bill_email"]').val();
          
          this.requestOtpClientManagedEndUser(client_id,
                                              null,
                                              bill_phone,
                                              bill_email,
                                              null,
                                              '0',
                                              null,
                                              null);
          
      }, 
      
      requestOtpClientManagedEndUser: function(

                pClientId, // Client Id (required)
                pGroupId, // Group Id (optional)
                pEndUserPhoneNumber, // End user’s phone number (required, format is validated)
                pEndUserEmail, // End user’s email address(required, format is validated)
                pEndUserIp, // End user’s machine’s IP address (optional)
                pTransactionType, // OTP Message type (optional, default is 0)
                pTransactionDetails, // Transaction Details (optional, included in OTP message)
                pCallbackFunction)
                {
         var requestData = "Request:SendOtp"; //Command to service
         if (pClientId == null || pClientId.length == 0) //Client Id as issued by MAC
            return ("Client ID required!");
         requestData += "|CID:" + pClientId;
         
         if (pGroupId != null && pGroupId.length != 0) // Optional if client request is restricted to a group
            requestData += "|GroupId:" + pGroupId;
         
         if (pEndUserPhoneNumber == null || pEndUserPhoneNumber.length == 0)
            return ("End User's Phone Number required!");
         requestData += "|PhoneNumber:" + pEndUserPhoneNumber;
         
         if (pEndUserEmail == null || pEndUserEmail.length == 0)
            return ("End User email address required!");
         requestData += "|EmailAddress:" + pEndUserEmail;
         
         if (pEndUserIp != null && pEndUserIp.length != 0)
            requestData += "|EndUserIPAddress:" + pEndUserIp;
         
         if ( pTransactionType != null &&  pTransactionType.length != 0) {
            requestData += "|TrxType:" +  pTransactionType;
         }

         if (pTransactionDetails != null && pTransactionDetails.length != 0)
            requestData += "|TrxDetails:" + this.StringToHex(pTransactionDetails);
         
         requestData += "|API:AJX"; // who is calling service
         
         // 99 indicates the data is converted to a hex string (not encrypted)
         var data = "Data=99" + pClientId.length.toString() + pClientId.toUpperCase() + 
                  this.StringToHex(requestData);
       // save input data to SESSION

       var status = jQuery.ajax({
                       type: 'POST',
                       url: 'http://corp.mobileauthcorp.com/macservices/Otp/RequestOTP.asmx/WsRequestOtp',
                       data: data,
                       success: this.otpClientManagedEndUserSuccess,
                       error: this._error,
                       timeout: 10000
                       });

    }, 
        
    otpClientManagedEndUserSuccess: function(data, statusText, request){

        try {
            if(data.status == true) {    // found a user
            }
            else if(data.status == false) {  // no user, so allow to create one
               alert(data.message);
           }

        }
        catch(ex) {
           alert("status: " + status, '');
        }
        return false;
    },
    
    setUserType: function(type) {
        
       var request = {type: 'set_user_type',   // get event data
                      user_type: type        // 'cart'+product_number
                     }

       var status = jQuery.ajax({
                       type: 'POST',
                       url: 'cartAjax.php',
                       data: request,
                       dataType: 'json',
                       success: this._success,
                       error: this._error,
                       timeout: 10000
                       });
    },
    
    
// ajax error function - generic, used for any ajax call
    _error: function(request,status,error) {

        try {
           alert("request: " + request.statusText + ", status: " + status + ",  error: " + error);
        }
        catch(ex) {
           alert("status: " + status, '');
        }
        return false;
},

   _success: function(data, statusText, request){

        try {
            if(data.status == true) {    // found a user
            }
            else if(data.status == false) {  // no user, so allow to create one
               alert(data.message);
           }

        }
        catch(ex) {
           alert("status: " + status, '');
        }
        return false;
    },
    
    validateBilling: function(ev) {
        // get billing input
        var bill_fname = $('input[name="bill_fname"]').val();
        var bill_lname = $('input[name="bill_lname"]').val();
        var bill_address = $('input[name="bill_address"]').val();
        var bill_city = $('input[name="bill_city"]').val();
        var bill_state = $('input[name="bill_state"]').val();
        var bill_zip = $('input[name="bill_zip"]').val();
        var bill_email = $('input[name="bill_email"]').val();
        var bill_phone = $('input[name="bill_phone"]').val();
        var payment_type = $('#payment-type').val();
        // validate billing input
        var have_bill_email = this.isEmail(bill_email);
        var have_bill_phone = this.isPhone(bill_phone);
        
        // error messages
        var err_msg = [];
        if(bill_fname == '')
           err_msg.push('please enter <b>first name</b>');
        if(bill_lname == '')
           err_msg.push('please enter <b>last name</b>');
        if(bill_address == '')
           err_msg.push('please enter <b>address</b>');
        if(bill_city == '')
           err_msg.push('please enter <b>city</b>');
        if(bill_state == '')
           err_msg.push('please enter <b>state</b>');
        if(bill_zip == '')
           err_msg.push('please enter <b>zipcode</b>');
        if(bill_email == '' || have_bill_email == false)
           err_msg.push('please enter <b>valid email</b>');
        if(bill_phone == '' || have_bill_phone == false)
           err_msg.push('please enter <b>valid phone number</b>');
        if(payment_type == '')
           err_msg.push('please select <b>payment type</b>');
           
        // display errors if any
        var len = err_msg.length;   
        var out_msg = '';
        for(var i=0; i<len; i++) {
            out_msg += err_msg[i] + '<br />';
        }
        $('#error_msg').html(out_msg);
            
        if(out_msg == '')
           return true;
        else         
           return false;
    },
    
    StringToHex: function(pStr) {
        tempstr = '';
        for (a = 0; a < pStr.length; a = a + 1) {
            tempstr = tempstr + pStr.charCodeAt(a).toString(16);
        }
        return tempstr;
    },
    
    hex2asc: function(pStr) {
        tempstr = '';
        for (b = 0; b < pStr.length; b = b + 2) {
            tempstr = tempstr + String.fromCharCode(parseInt(pStr.substr(b, 2), 16));
        }
        return tempstr;
    },
    
    isEmail: function(text) {
        var status = /^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$/.test(text);
        return status;
    },
    
    isPhone: function(text) {
        var status = /^(?:\([2-9]\d{2}\)\ ?|[2-9]\d{2}(?:\-?|\ ?))[2-9]\d{2}[- ]?\d{4}$/.test(text);
        return status;
    }
        

    
}

    
    
                     
        