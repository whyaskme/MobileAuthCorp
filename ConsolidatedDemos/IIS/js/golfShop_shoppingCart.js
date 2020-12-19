var itemCount1 = 0;
var itemCount2 = 0;
var itemCount3 = 0;
var itemCount4 = 0;
var itemCount5 = 0;
var itemCount6 = 0;
var itemCount7 = 0;
var itemCount8 = 0;

var subtotal1 = 0;
var subtotal2 = 0;
var subtotal3 = 0;
var subtotal4 = 0;
var subtotal5 = 0;
var subtotal6 = 0;
var subtotal7 = 0;
var subtotal8 = 0;

var displaySubtotal1 = 0;
var displaySubtotal2 = 0;
var displaySubtotal3 = 0;
var displaySubtotal4 = 0;
var displaySubtotal5 = 0;
var displaySubtotal6 = 0;
var displaySubtotal7 = 0;
var displaySubtotal8 = 0;

var total = 0;
var displayTotal = 0;

function btnItem1() {
    var itemNumber1 = $("#item_number1").html();
    var itemDescription1 = $("#item_decription1").html();
    var itemPrice1 = $("#item_price1").html().replace(/[^\d.]/g, '');

    itemCount1++;

    subtotal1 = itemCount1 * itemPrice1;
    displaySubtotal1 = "$" + subtotal1.toFixed(2);

    total = subtotal1 + subtotal2 + subtotal3 + subtotal4 + subtotal5 + subtotal6 + subtotal7 + subtotal8;
    displayTotal = total.toFixed(2);

    $("#totalPrice").html(displayTotal);

    $("#item_number1_display").html(itemNumber1);
    $("#item_decription1_display").html(itemDescription1);
    $("#item_quantity1").html(itemCount1);
    $("#item_subtotal1").html(displaySubtotal1);

    //var x = $("#item_display1").css("background-color");
    //alert(x);

    $("#item_display1").show();

    var h = $("#divDisclaimerOTP").height() + 25;

    $("html, body").animate({ scrollTop: $(document).height() - ($(window).height() + h) });

    $("#lbError2").hide();
    bgResize();
    CreateTrxDetail();

}
function btnItem2() {
    var itemNumber2 = $("#item_number2").html();
    var itemDescription2 = $("#item_decription2").html();
    var itemPrice2 = $("#item_price2").html().replace(/[^\d.]/g, '');

    itemCount2++;

    subtotal2 = itemCount2 * itemPrice2;
    displaySubtotal2 = "$" + subtotal2.toFixed(2);

    total = subtotal1 + subtotal2 + subtotal3 + subtotal4 + subtotal5 + subtotal6 + subtotal7 + subtotal8;
    displayTotal = total.toFixed(2);

    $("#totalPrice").html(displayTotal);

    $("#item_number2_display").html(itemNumber2);
    $("#item_decription2_display").html(itemDescription2);
    $("#item_quantity2").html(itemCount2);
    $("#item_subtotal2").html(displaySubtotal2);

    //var prevRow = $( row ).prev()[0];
    //var color1 = $(prevRow).css("background-color");
    //alert(color1);

    //var x = $("#item_display2").css("background-color");
    //if(x == "rgb(239, 239, 239)")
    //alert("color");

    $("#item_display2").show();

    var h = $("#divDisclaimerOTP").height() + 25;

    $("html, body").animate({ scrollTop: $(document).height() - ($(window).height() + h) });

    $("#lbError2").hide();
    bgResize();
    CreateTrxDetail();
}
function btnItem3() {
    var itemNumber3 = $("#item_number3").html();
    var itemDescription3 = $("#item_decription3").html();
    var itemPrice3 = $("#item_price3").html().replace(/[^\d.]/g, '');

    itemCount3++;

    subtotal3 = itemCount3 * itemPrice3;
    displaySubtotal3 = "$" + subtotal3.toFixed(2);

    total = subtotal1 + subtotal2 + subtotal3 + subtotal4 + subtotal5 + subtotal6 + subtotal7 + subtotal8;
    displayTotal = total.toFixed(2);

    $("#totalPrice").html(displayTotal);

    $("#item_number3_display").html(itemNumber3);
    $("#item_decription3_display").html(itemDescription3);
    $("#item_quantity3").html(itemCount3);
    $("#item_subtotal3").html(displaySubtotal3);

    $("#item_display3").show();

    var h = $("#divDisclaimerOTP").height() + 25;

    $("html, body").animate({ scrollTop: $(document).height() - ($(window).height() + h) });

    $("#lbError2").hide();
    bgResize();
    CreateTrxDetail();
}
function btnItem4() {
    var itemNumber4 = $("#item_number4").html();
    var itemDescription4 = $("#item_decription4").html();
    var itemPrice4 = $("#item_price4").html().replace(/[^\d.]/g, '');

    itemCount4++;

    subtotal4 = itemCount4 * itemPrice4;
    displaySubtotal4 = "$" + subtotal4.toFixed(2);

    total = subtotal1 + subtotal2 + subtotal3 + subtotal4 + subtotal5 + subtotal6 + subtotal7 + subtotal8;
    displayTotal = total.toFixed(2);

    $("#totalPrice").html(displayTotal);

    $("#item_number4_display").html(itemNumber4);
    $("#item_decription4_display").html(itemDescription4);
    $("#item_quantity4").html(itemCount4);
    $("#item_subtotal4").html(displaySubtotal4);

    $("#item_display4").show();

    var h = $("#divDisclaimerOTP").height() + 25;

    $("html, body").animate({ scrollTop: $(document).height() - ($(window).height() + h) });

    $("#lbError2").hide();
    bgResize();
    CreateTrxDetail();
}
function btnItem5() {
    var itemNumber5 = $("#item_number5").html();
    var itemDescription5 = $("#item_decription5").html();
    var itemPrice5 = $("#item_price5").html().replace(/[^\d.]/g, '');

    itemCount5++;

    subtotal5 = itemCount5 * itemPrice5;
    displaySubtotal5 = "$" + subtotal5.toFixed(2);

    total = subtotal1 + subtotal2 + subtotal3 + subtotal4 + subtotal5 + subtotal6 + subtotal7 + subtotal8;
    displayTotal = total.toFixed(2);

    $("#totalPrice").html(displayTotal);

    $("#item_number5_display").html(itemNumber5);
    $("#item_decription5_display").html(itemDescription5);
    $("#item_quantity5").html(itemCount5);
    $("#item_subtotal5").html(displaySubtotal5);

    $("#item_display5").show();

    var h = $("#divDisclaimerOTP").height() + 25;

    $("html, body").animate({ scrollTop: $(document).height() - ($(window).height() + h) });

    $("#lbError2").hide();
    bgResize();
    CreateTrxDetail();
}
function btnItem6() {
    var itemNumber6 = $("#item_number6").html();
    var itemDescription6 = $("#item_decription6").html();
    var itemPrice6 = $("#item_price6").html().replace(/[^\d.]/g, '');

    itemCount6++;

    subtotal6 = itemCount6 * itemPrice6;
    displaySubtotal6 = "$" + subtotal6.toFixed(2);

    total = subtotal1 + subtotal2 + subtotal3 + subtotal4 + subtotal5 + subtotal6 + subtotal7 + subtotal8;
    displayTotal = total.toFixed(2);

    $("#totalPrice").html(displayTotal);

    $("#item_number6_display").html(itemNumber6);
    $("#item_decription6_display").html(itemDescription6);
    $("#item_quantity6").html(itemCount6);
    $("#item_subtotal6").html(displaySubtotal6);

    $("#item_display6").show();

    var h = $("#divDisclaimerOTP").height() + 25;

    $("html, body").animate({ scrollTop: $(document).height() - ($(window).height() + h) });

    $("#lbError2").hide();
    bgResize();
    CreateTrxDetail();
}
function btnItem7() {
    var itemNumber7 = $("#item_number7").html();
    var itemDescription7 = $("#item_decription7").html();
    var itemPrice7 = $("#item_price7").html().replace(/[^\d.]/g, '');

    itemCount7++;

    subtotal7 = itemCount7 * itemPrice7;
    displaySubtotal7 = "$" + subtotal7.toFixed(2);

    total = subtotal1 + subtotal2 + subtotal3 + subtotal4 + subtotal5 + subtotal6 + subtotal7 + subtotal8;
    displayTotal = total.toFixed(2);

    $("#totalPrice").html(displayTotal);

    $("#item_number7_display").html(itemNumber7);
    $("#item_decription7_display").html(itemDescription7);
    $("#item_quantity7").html(itemCount7);
    $("#item_subtotal7").html(displaySubtotal7);

    $("#item_display7").show();

    var h = $("#divDisclaimerOTP").height() + 25;

    $("html, body").animate({ scrollTop: $(document).height() - ($(window).height() + h) });

    $("#lbError2").hide();
    bgResize();
    CreateTrxDetail();
}
function btnItem8() {
    var itemNumber8 = $("#item_number8").html();
    var itemDescription8 = $("#item_decription8").html();
    var itemPrice8 = $("#item_price8").html().replace(/[^\d.]/g, '');

    itemCount8++;

    subtotal8 = itemCount8 * itemPrice8;
    displaySubtotal8 = "$" + subtotal8.toFixed(2);

    total = subtotal1 + subtotal2 + subtotal3 + subtotal4 + subtotal5 + subtotal6 + subtotal7 + subtotal8;
    displayTotal = total.toFixed(2);

    $("#totalPrice").html(displayTotal);

    $("#item_number8_display").html(itemNumber8);
    $("#item_decription8_display").html(itemDescription8);
    $("#item_quantity8").html(itemCount8);
    $("#item_subtotal8").html(displaySubtotal8);

    $("#item_display8").show();

    var h = $("#divDisclaimerOTP").height() + 25;

    $("html, body").animate({ scrollTop: $(document).height() - ($(window).height() + h) });

    $("#lbError2").hide();
    bgResize();
    CreateTrxDetail();
}

function clearTotal() {
    itemCount1 = 0;
    itemCount2 = 0;
    itemCount3 = 0;
    itemCount4 = 0;
    itemCount5 = 0;
    itemCount6 = 0;
    itemCount7 = 0;
    itemCount8 = 0;

    subtotal1 = 0;
    subtotal2 = 0;
    subtotal3 = 0;
    subtotal4 = 0;
    subtotal5 = 0;
    subtotal6 = 0;
    subtotal7 = 0;
    subtotal8 = 0;

    displaySubtotal1 = 0;
    displaySubtotal2 = 0;
    displaySubtotal3 = 0;
    displaySubtotal4 = 0;
    displaySubtotal5 = 0;
    displaySubtotal6 = 0;
    displaySubtotal7 = 0;
    displaySubtotal8 = 0;

    total = 0;
    displayTotal = 0;

    $("#hiddenTrxDetails").val("");

    $("#item_display1").hide();
    $("#item_display2").hide();
    $("#item_display3").hide();
    $("#item_display4").hide();
    $("#item_display5").hide();
    $("#item_display6").hide();
    $("#item_display7").hide();
    $("#item_display8").hide();

    $("#loginMessage").hide();

    $("#totalPrice").html("0.00");

    $("#lbError2").hide();

    bgResize();

}

function bgResize() {

    var store = $("#myFieldset").is(":visible");

    var x = $("#divDisclaimerOTP").height();

    if (store == true) {
        var h = $("#myFieldset").innerHeight();
        var x = $("#divDisclaimerOTP").height();

        $('#bg').css('height', h + x + 45);

        var w = $('#bg').width() / 2;
        var xpos = $(window).innerWidth() / 2 - w;
        $('#bg').css('margin-left', xpos);
    } else {
        var h = $(document).height();
        $('#bg').css('height', h - 90);

        var w = $('#bg').width() / 2;
        var xpos = $(window).innerWidth() / 2 - w;
        $('#bg').css('margin-left', xpos);
    }

}

function RequestOTPResponse()
{
    $("#productDisplay").hide();
    $("#myFieldset legend").html("Purchase Summary");
    $("#buttonContainerPurchase").hide();
    $("#tableSpacer").hide();
    $("#adOTPContainer").show();
}

//function ShowProcessingMessage() {
//    // Scroll window to top. If not, then if the browser is towards the bottom of the window, the dialog is out of view at the top
//    window.scrollTo(0, 0);

//    var loaderMargin = (screenHeight / 2) - 18;

//    $('#divDialogContainer').css('margin-top', loaderMargin);

//    $('#divPleaseWaitProcessing').css("height", screenHeight);
//    $('#divPleaseWaitProcessing').css("width", screenWidth);
//    $('#divPleaseWaitProcessing').show();
//    $('#divDialogContainer').show();
//}

function CreateTrxDetail() {
    var trxdetails = "";
    var elementname = "";

    for (var x = 1; x < 9; ++x) {
        elementname = "#item_display" + x.toString();
        if ($(elementname).is(":visible")) {
            elementname = "#item_number" + x.toString() + "_display";
            var itemno = $(elementname).text();

            var itemdes = $("#item_decription" + x).html();

            elementname = "#item_quantity" + x.toString();
            var itemq = $(elementname).text();

            elementname = "#item_subtotal" + x.toString();
            var itemst = $(elementname).text();

            if (trxdetails.length == 0)
                trxdetails += x.toString() + ":" + itemno + ":" + itemq + ":" + itemst + ":" + itemdes;
            else
                trxdetails += "|" + x.toString() + ":" + itemno + ":" + itemq + ":" + itemst + ":" + itemdes;

        }
    }
    var totalprice = $("#totalPrice").text();
    trxdetails += "|Total " + totalprice;
    // set transaction detail in hiddenfield
    $("#hiddenTrxDetails").val(trxdetails);

}

function hidePanel() {
    $("#messageRow").hide();
}

function hideMessage() {
    $("#loginMessage").hide();
}

function pleaseLogin() {
    alert("Please login");
    $("#txtLoginName").focus();
}