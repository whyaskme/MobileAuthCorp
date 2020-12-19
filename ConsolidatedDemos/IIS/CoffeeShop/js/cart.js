var item1Count = 0;
var item2Count = 0;
var item3Count = 0;
var item4Count = 0;

var totalItemCount = 0;

var subtotal1Price = 0;
var subtotal2Price = 0;
var subtotal3Price = 0;
var subtotal4Price = 0;

var totalPrice = 0;

//screen dimensions for use with cart item popup centering and processing display
var screenHeight = $(window).innerHeight();
var screenWidth = $(window).innerWidth();

var itemAddedisplayWidth = $("#divItemAddedToCartDisplay").width();

var leftMargin = (screenWidth / 2) - (itemAddedisplayWidth / 2);

if (screenWidth <= 475)
    leftMargin = 0;

$("#divItemAddedToCartDisplay").css("margin-left", leftMargin);

function resize() {
    var screenHeight = $(window).innerHeight();
    var screenWidth = $(window).innerWidth();

    var itemAddedisplayWidth = $("#divItemAddedToCartDisplay").width();

    var leftMargin = (screenWidth / 2) - (itemAddedisplayWidth / 2);

    if (screenWidth <= 475)
        leftMargin = 0;
    
    $("#divItemAddedToCartDisplay").css("margin-left", leftMargin);
}

function itemAddedToCartDisplay() {
    var screenHeight = $(window).innerHeight();
    var screenWidth = $(window).innerWidth();

    var itemAddedisplayWidth = $("#divItemAddedToCartDisplay").width();
    
    if (screenWidth >= 475)
        itemAddedisplayWidth = 439;

    var leftMargin = (screenWidth / 2) - (itemAddedisplayWidth / 2);

    if (screenWidth <= 475)
        leftMargin = 0;

    $("#divItemAddedToCartDisplay").css("margin-left", leftMargin);
    $("#divItemAddedToCartDisplay").show();
}

function hideAddedToCartDisplay() {
    $("#divItemAddedToCartDisplay").hide();
}

function hideItemAddedToCartDisplay() {
    $("#divItemAddedToCartDisplay").hide();
}

function hideError() {
    $("#divErrorMessage").hide();
}

function item1AddedToCart() {

    item1Count++;
    totalItemCount++;

    //show cart header and row
    $("#cartDisplayHeader").show();
    $("#item1_cartDisplay").show();

    //cart qty value
    $("#item1_cartQty").html(item1Count);

    //cart subtotal value
    subtotal1Price = $("#item1_cartPrice").html().replace(/[^\d.]/g, '') * item1Count;
    $("#item1_cartSubtotal").html(subtotal1Price.toFixed(2));

    //cart total value
    totalPrice = subtotal1Price + subtotal2Price + subtotal3Price + subtotal4Price;
    $("#totalPriceDisplay").html(totalPrice.toFixed(2));

    //total cart items display (desktop and mobile)
    $("#spanAddItemCartTotal").html(totalItemCount);
    $("#spanCartItemsDesktop").html(totalItemCount);
    $("#spanCartItemsMobile").html(totalItemCount);

    //confirmation values
    $("#hiddenTotalQty").val(totalItemCount);
    $("#hiddenTotalPrice").val(totalPrice.toFixed(2));

    var addItem1Title = $("#spanItem1Title").html();
    var addItem1Price = $("#spanItem1Price").html();
    $("#spanAddItemTitle").html(addItem1Title);
    $("#spanAddItemPrice").html(addItem1Price);

    $("#imgItemAddedToCartDisplay").attr("src", "Images/coffee1small.jpg");

    //add item popup qty and subtotal
    $("#spanAddItemQty").html(item1Count);
    $("#spanAddItemPrice").html(subtotal1Price.toFixed(2));

    CreateTrxDetail();
    itemAddedToCartDisplay();

}
function item2AddedToCart() {

    item2Count++;
    totalItemCount++;

    //show cart header and row
    $("#cartDisplayHeader").show();
    $("#item2_cartDisplay").show();

    //cart qty value
    $("#item2_cartQty").html(item2Count);

    //cart subtotal value
    subtotal2Price = $("#item2_cartPrice").html().replace(/[^\d.]/g, '') * item2Count;
    $("#item2_cartSubtotal").html(subtotal2Price.toFixed(2));

    //cart total value
    totalPrice = subtotal1Price + subtotal2Price + subtotal3Price + subtotal4Price;
    $("#totalPriceDisplay").html(totalPrice.toFixed(2));

    //total cart items display (desktop and mobile)
    $("#spanAddItemCartTotal").html(totalItemCount);
    $("#spanCartItemsDesktop").html(totalItemCount);
    $("#spanCartItemsMobile").html(totalItemCount);

    //confirmation values
    $("#hiddenTotalQty").html(totalItemCount);
    $("#hiddenTotalPrice").html(totalPrice.toFixed(2));

    var addItem2Title = $("#spanItem2Title").html();
    var addItem2Price = $("#spanItem2Price").html();
    $("#spanAddItemTitle").html(addItem2Title);
    $("#spanAddItemPrice").html(addItem2Price);

    $("#imgItemAddedToCartDisplay").attr("src", "Images/coffee2small.jpg");

    //add item popup qty and subtotal
    $("#spanAddItemQty").html(item2Count);
    $("#spanAddItemPrice").html(subtotal2Price.toFixed(2));

    CreateTrxDetail();
    itemAddedToCartDisplay();

}
function item3AddedToCart() {

    item3Count++;
    totalItemCount++;

    //show cart header and row
    $("#cartDisplayHeader").show();
    $("#item3_cartDisplay").show();

    //cart qty value
    $("#item3_cartQty").html(item3Count);

    //cart subtotal value
    subtotal3Price = $("#item3_cartPrice").html().replace(/[^\d.]/g, '') * item3Count;
    $("#item3_cartSubtotal").html(subtotal3Price.toFixed(2));

    //cart total value
    totalPrice = subtotal1Price + subtotal2Price + subtotal3Price + subtotal4Price;
    $("#totalPriceDisplay").html(totalPrice.toFixed(2));

    //total cart items display (desktop and mobile)
    $("#spanAddItemCartTotal").html(totalItemCount);
    $("#spanCartItemsDesktop").html(totalItemCount);
    $("#spanCartItemsMobile").html(totalItemCount);

    //confirmation values
    $("#hiddenTotalQty").html(totalItemCount);
    $("#hiddenTotalPrice").html(totalPrice.toFixed(2));

    var addItem3Title = $("#spanItem3Title").html();
    var addItem3Price = $("#spanItem3Price").html();
    $("#spanAddItemTitle").html(addItem3Title);
    $("#spanAddItemPrice").html(addItem3Price);

    $("#imgItemAddedToCartDisplay").attr("src", "Images/coffee3small.jpg");

    //add item popup qty and subtotal
    $("#spanAddItemQty").html(item3Count);
    $("#spanAddItemPrice").html(subtotal3Price.toFixed(2));

    CreateTrxDetail();
    itemAddedToCartDisplay();

}
function item4AddedToCart() {

    item4Count++;
    totalItemCount++;

    //show cart header and row
    $("#cartDisplayHeader").show();
    $("#item4_cartDisplay").show();

    //cart qty value
    $("#item4_cartQty").html(item4Count);

    //cart subtotal value
    subtotal4Price = $("#item4_cartPrice").html().replace(/[^\d.]/g, '') * item4Count;
    $("#item4_cartSubtotal").html(subtotal4Price.toFixed(2));

    //cart total value
    totalPrice = subtotal1Price + subtotal2Price + subtotal3Price + subtotal4Price;
    $("#totalPriceDisplay").html(totalPrice.toFixed(2));

    //total cart items display (desktop and mobile)
    $("#spanAddItemCartTotal").html(totalItemCount);
    $("#spanCartItemsDesktop").html(totalItemCount);
    $("#spanCartItemsMobile").html(totalItemCount);

    //confirmation values
    $("#hiddenTotalQty").html(totalItemCount);
    $("#hiddenTotalPrice").html(totalPrice.toFixed(2));

    var addItem4Title = $("#spanItem4Title").html();
    var addItem4Price = $("#spanItem4Price").html();
    $("#spanAddItemTitle").html(addItem4Title);
    $("#spanAddItemPrice").html(addItem4Price);

    $("#imgItemAddedToCartDisplay").attr("src", "Images/coffee4small.jpg");

    //add item popup qty and subtotal
    $("#spanAddItemQty").html(item4Count);
    $("#spanAddItemPrice").html(subtotal4Price.toFixed(2));

    CreateTrxDetail();
    itemAddedToCartDisplay();

}

function item1DeletedFromCart() {

    var c = confirm("Remove item from shopping cart?");
    if (c == false)
        return;

    totalItemCount = totalItemCount - item1Count;
    item1Count = 0;

    $("#item1_cartDisplay").hide();

    //hide header if there are no other rows to display
    var x = $("#item2_cartDisplay").css("display");
    var y = $("#item3_cartDisplay").css("display");
    var z = $("#item4_cartDisplay").css("display");
    
    if (x == "none" && y == "none" && z == "none")
        $("#cartDisplayHeader").hide();

    //cart qty value
    $("#item1_cartQty").html("0");

    //cart subtotal value
    subtotal1Price = 0;
    $("#item1_cartSubtotal").html(subtotal1Price.toFixed(2));

    //cart total value
    totalPrice = subtotal1Price + subtotal2Price + subtotal3Price + subtotal4Price;
    $("#totalPriceDisplay").html(totalPrice.toFixed(2));

    //total cart items display (desktop and mobile)
    $("#spanAddItemCartTotal").html(totalItemCount);
    $("#spanCartItemsDesktop").html(totalItemCount);
    $("#spanCartItemsMobile").html(totalItemCount);

    //confirmation values
    $("#hiddenTotalQty").html(totalItemCount);
    $("#hiddenTotalPrice").html(totalPrice.toFixed(2));
    
}
function item2DeletedFromCart() {

    var c = confirm("Remove item from shopping cart?");
    if (c == false)
        return;

    totalItemCount = totalItemCount - item2Count;
    item2Count = 0;

    $("#item2_cartDisplay").hide();

    //hide header if there are no other rows to display
    var x = $("#item1_cartDisplay").css("display");
    var y = $("#item3_cartDisplay").css("display");
    var z = $("#item4_cartDisplay").css("display");

    if (x == "none" && y == "none" && z == "none")
        $("#cartDisplayHeader").hide();

    //cart qty value
    $("#item2_cartQty").html("0");

    //cart subtotal value
    subtotal2Price = 0;
    $("#item2_cartSubtotal").html(subtotal2Price.toFixed(2));

    //cart total value
    totalPrice = subtotal1Price + subtotal2Price + subtotal3Price + subtotal4Price;
    $("#totalPriceDisplay").html(totalPrice.toFixed(2));

    //total cart items display (desktop and mobile)
    $("#spanAddItemCartTotal").html(totalItemCount);
    $("#spanCartItemsDesktop").html(totalItemCount);
    $("#spanCartItemsMobile").html(totalItemCount);

    //confirmation values
    $("#hiddenTotalQty").html(totalItemCount);
    $("#hiddenTotalPrice").html(totalPrice.toFixed(2));

}
function item3DeletedFromCart() {

    var c = confirm("Remove item from shopping cart?");
    if (c == false)
        return;

    totalItemCount = totalItemCount - item3Count;
    item3Count = 0;

    $("#item3_cartDisplay").hide();

    //hide header if there are no other rows to display
    var x = $("#item1_cartDisplay").css("display");
    var y = $("#item2_cartDisplay").css("display");
    var z = $("#item4_cartDisplay").css("display");

    if (x == "none" && y == "none" && z == "none")
        $("#cartDisplayHeader").hide();

    //cart qty value
    $("#item3_cartQty").html("0");

    //cart subtotal value
    subtotal3Price = 0;
    $("#item3_cartSubtotal").html(subtotal3Price.toFixed(2));

    //cart total value
    totalPrice = subtotal1Price + subtotal2Price + subtotal3Price + subtotal4Price;
    $("#totalPriceDisplay").html(totalPrice.toFixed(2));

    //total cart items display (desktop and mobile)
    $("#spanAddItemCartTotal").html(totalItemCount);
    $("#spanCartItemsDesktop").html(totalItemCount);
    $("#spanCartItemsMobile").html(totalItemCount);

    //confirmation values
    $("#hiddenTotalQty").html(totalItemCount);
    $("#hiddenTotalPrice").html(totalPrice.toFixed(2));

}
function item4DeletedFromCart() {

    var c = confirm("Remove item from shopping cart?");
    if (c == false)
        return;

    totalItemCount = totalItemCount - item4Count;
    item4Count = 0;

    $("#item4_cartDisplay").hide();

    //hide header if there are no other rows to display
    var x = $("#item1_cartDisplay").css("display");
    var y = $("#item2_cartDisplay").css("display");
    var z = $("#item3_cartDisplay").css("display");

    if (x == "none" && y == "none" && z == "none")
        $("#cartDisplayHeader").hide();

    //cart qty value
    $("#item4_cartQty").html("0");

    //cart subtotal value
    subtotal4Price = 0;
    $("#item4_cartSubtotal").html(subtotal4Price.toFixed(2));

    //cart total value
    totalPrice = subtotal1Price + subtotal2Price + subtotal3Price + subtotal4Price;
    $("#totalPriceDisplay").html(totalPrice.toFixed(2));

    //total cart items display (desktop and mobile)
    $("#spanAddItemCartTotal").html(totalItemCount);
    $("#spanCartItemsDesktop").html(totalItemCount);
    $("#spanCartItemsMobile").html(totalItemCount);

    //confirmation values
    $("#hiddenTotalQty").html(totalItemCount);
    $("#hiddenTotalPrice").html(totalPrice.toFixed(2));

}

function showCart() {
    $("#liMenuHome").removeClass("active");
    $("#liMenuCart").addClass("active");
    $("#divHome").hide();
    $("#divShopping").hide();
    if (totalPrice == 0) {
        $("#totalPriceBorder").css("border-top", "none");
    } else {
        $("#totalPriceBorder").css("border-top", "1px solid #e3d4c1");
    }
    $("#divCart").show();
    $("#divProcessing").hide();
    $("#divThankYou").hide();
    $("#divPurchaseForm").hide();
    $("#divTooManyRetries").hide();
    $("#hiddenTooManyRetries").val("false");
}

function showHomePage() {
    $("#liMenuHome").addClass("active");
    $("#liMenuCart").removeClass("active");
    $("#divHome").show();
    $("#divShopping").show();
    $("#divErrorMessage").hide();
    $("#divTimeout").hide();
    $("#divTooManyRetries").hide();
    $("#divItemAddedToCartDisplay").hide();
    $("#divCart").hide();
    $("#divPurchaseForm").hide();
    $("#divProcessing").hide();
    $("#divThankYou").hide();
    $("#hiddenThankYou").val("false");
    $("#hiddenTimeout").val("false");
    $("#hiddenTooManyRetries").val("false");
}

function showPurchaseForm() {
    if (totalPrice == 0) {
        alert("There are no items to purchase");
        return;
    }

    //assign shopping cart values to hidden fields
    var d1 = $("#item1_cartDiscription").html();
    var d2 = $("#item2_cartDiscription").html();
    var d3 = $("#item3_cartDiscription").html();
    var d4 = $("#item4_cartDiscription").html();
    $("#hiddenDescription1").val(d1);
    $("#hiddenDescription2").val(d2);
    $("#hiddenDescription3").val(d3);
    $("#hiddenDescription4").val(d4);

    var q1 = $("#item1_cartQty").html();
    var q2 = $("#item2_cartQty").html();
    var q3 = $("#item3_cartQty").html();
    var q4 = $("#item4_cartQty").html();
    $("#hiddenQty1").val(q1);
    $("#hiddenQty2").val(q2);
    $("#hiddenQty3").val(q3);
    $("#hiddenQty4").val(q4);

    var st1 = $("#item1_cartSubtotal").html();
    var st2 = $("#item2_cartSubtotal").html();
    var st3 = $("#item3_cartSubtotal").html();
    var st4 = $("#item4_cartSubtotal").html();
    $("#hiddenSubtotal1").val(st1);
    $("#hiddenSubtotal2").val(st2);
    $("#hiddenSubtotal3").val(st3);
    $("#hiddenSubtotal4").val(st4);

    $("#hiddenTotalQty").val(totalItemCount);
    $("#hiddenTotalPrice").val(totalPrice.toFixed(2));    

    $("#divHome").hide();
    $("#divShopping").hide();
    $("#divCart").hide();
    $("#divProcessing").hide();
    $("#divThankYou").hide();
    $("#divPurchaseForm").show();
}

function ShowProcessing() {
    // Scroll window to top. If not, then if the browser is towards the bottom of the window, the dialog is out of view at the top
    window.scrollTo(0, 0);

    var loaderMargin = (screenHeight / 2) - 18;

    $('#divDialogContainer').css('margin-top', loaderMargin);

    $('#divPleaseWaitProcessing').css("height", screenHeight);
    $('#divPleaseWaitProcessing').css("width", screenWidth);
    $('#divPleaseWaitProcessing').show();
    $('#divDialogContainer').show();
}

function CreateTrxDetail() {
    var trxDetails = "";
    var cartItem = "";

    for (var x = 1; x < 5; x++) {

        var itemQ = $("#item" + x + "_cartQty").html();

        if (itemQ != 0) {

            var itemDesc = $("#item" + x + "_cartDiscription").html();
            var itemQ = $("#item" + x + "_cartQty").html();
            var itemSubtotal = $("#item" + x + "_cartSubtotal").html();

            if (trxDetails.length == 0)
                trxDetails += x.toString() + ":" + itemDesc + ":" + itemQ + ":" + itemSubtotal;
            else
                trxDetails += "|" + x.toString() + ":" + itemDesc + ":" + itemQ + ":" + itemSubtotal;

        }
    }
    var totalprice = $("#totalPriceDisplay").text();
    trxDetails += "|Total " + totalprice;
    // set transaction detail in hiddenfield
    $("#hiddenTrxDetails").val(trxDetails);
}

function validatePurchaseForm() {
    var firstName = $("#txtFirstName").val();
    var lastName = $("#txtLastName").val();

    var mobilePhone = $("#txtMobilePhone").val();
    var mobilePhoneRegEx = /^\s*(?:\+?(\d{1,3}))?[-. (]*(\d{3})[-. )]*(\d{3})[-. ]*(\d{4})(?: *x(\d+))?\s*$/;

    var email = $("#txtEmail").val();
    var emailRegEx = /^([A-Za-z0-9_\.-]+)@([\da-z\.-]+)\.([A-Za-z\.]{2,6})$/;

    var creditCard = $("#txtCreditCard").val();
    //var creditCardRegex = /\b(?:4[0-9]{12}(?:[0-9]{3})?|5[12345][0-9]{14}|3[47][0-9]{13}|3(?:0[012345]|[68][0-9])[0-9]{11}|6(?:011|5[0-9]{2})[0-9]{12}|(?:2131|1800|35[0-9]{3})[0-9]{11})\b/; // all major credit cards
    var creditCardRegex = /\b(?:\d[ -]*?){13,16}\b/; // basic 16-digit validation

    var expDate = $("#txtExpDate").val();
    var expDateRegex = /^(0[1-9]|1[0-2])\/?([0-9]{4}|[0-9]{2})$/;

    if (firstName != "" && lastName != ""
        && mobilePhone != "" && mobilePhoneRegEx.test(mobilePhone) != false
        && email != "" && emailRegEx.test(email) != false
        && creditCard != "" && creditCardRegex.test(creditCard) != false
        && expDate != "" && expDateRegex.test(expDate) != false)
    {
        $("#btnPurchase").attr("disabled", false);
    }
    else {
        $("#btnPurchase").attr("disabled", true);
        return false;
    }
    return;

}


$(document).ready(function () {
    $(document).mouseover(function () {
        validatePurchaseForm();
    });

    //create a unique confirmation number
    var chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXTZ";
    var string_length = 16;
    var myrnd = [], pos;

    while (string_length--) {
        
        pos = Math.floor(Math.random() * chars.length);        
        myrnd.push(chars.substr(pos, 1));

    }
    myrnd.join(''); // e.g "6DMIG9SP1KDEFB4JK5KWMNSI3UMQSSNT"
    var ConfirmationNumber = myrnd;

    //
    var x = $("#hiddenThankYou").val();
    if (x == "false")
    {
        $("#divHome").show();
        $("#divShopping").show();
        $("#divErrorMessage").hide();
        $("#divTimeout").hide();
        $("#divTooManyRetries").hide();
        $("#divItemAddedToCartDisplay").hide();
        $("#divCart").hide();
        $("#divPurchaseForm").hide();
        $("#divProcessing").hide();
        $("#divThankYou").hide();
    } else {
        $("#divHome").hide();
        $("#divShopping").hide();
        $("#divErrorMessage").hide();
        $("#divItemAddedToCartDisplay").hide();
        $("#divCart").hide();
        $("#divPurchaseForm").hide();
        $("#divProcessing").hide();
        $("#divTimeout").hide();
        $("#divTooManyRetries").hide();

        $("#spanConfirmationNumber").html(ConfirmationNumber);
        $("#divThankYou").show();
    }
    //request timeout
    var timeout = $("#hiddenTimeout").val();
    if (timeout == "true")
    {
        $("#divHome").hide();
        $("#divShopping").hide();
        $("#divErrorMessage").hide();
        $("#divItemAddedToCartDisplay").hide();
        $("#divCart").hide();
        $("#divPurchaseForm").hide();
        $("#divProcessing").hide();
        $("#divThankYou").hide();
        $("#divTimeout").show();
        $("#divTooManyRetries").hide();
    }
    //request retries exceeded
    var tooManyRetries = $("#hiddenTooManyRetries").val();
    if (tooManyRetries == "true") {
        $("#divHome").hide();
        $("#divShopping").hide();
        $("#divErrorMessage").hide();
        $("#divItemAddedToCartDisplay").hide();
        $("#divCart").hide();
        $("#divPurchaseForm").hide();
        $("#divProcessing").hide();
        $("#divThankYou").hide();
        $("#divTimeout").hide();
        $("#divTooManyRetries").show();
    }
});