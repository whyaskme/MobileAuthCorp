var webMethod = "";
var parameters = "";
var totalRecords = 0;
var totalPages = 0;
var recordsPerPage = 8;
var currentPageNumber = 1;
var lastRecordNumber = 0;
var numberGroupsSelected = 0;
var eventRowCount = 0;

var eventTimer;

var hasUnsavedChanges = false;

var intervalId;
var intervalTime = 2000;

var popupSettings;
var popupWindow;

var currentHost = "";
var currentDocUrl = "";
var arrLocation;

var dk_ItemSep = "|";
var dk_DefaultEmptyObjectId = "000000000000000000000000";
var dk_DefaultStaticObjectId = "111111111111111111111111";

var getString;
var clearString;

var DisplayMode;
var docBody;
var hiddenU;

var screenHeight = $(window).innerHeight();
var screenWidth = screen.width;

var left;
var top;

var popupWindowWidth = 0;
var popupWindowHeight = 0;
var eventHistoryTimerEnabled = false;

var paginationEnabled = false;

var windowLocation = window.location.toString().toLowerCase();

$(document).ready(function ()
{
    $("#error_firstName").hide();

    var tmpVal = windowLocation.split('/');
    currentHost = "http://" + tmpVal[2];

});

// golf shop login validation
function validateFormFields(element) {

    var minLength = $("#txtLoginName").attr("min-length");
    var maxLength = $("#txtLoginName").attr("max-length");

    var matchPattern = new RegExp($("#txtLoginName").attr("matchpattern"));

    var matchPatternLabel = $("#txtLoginName").attr("patterndescription");

    var fieldLabel = $("#" + element.id.replace("txt", "span"));
    var fieldName = $("#txtLoginName").attr("lbl");
    var fieldValue = $("#txtLoginName").val();

    if(parseInt(fieldValue.length) < parseInt(minLength))
    {
        fieldLabel.html(fieldName + " must be longer than " + minLength + " characters");
        //element.style = "border-color: #8b0300;background: url('../GolfShop/img/warning-symbol.png') no-repeat right center;";
        $("#txtLoginName").css("border-color", "#8b0300");
        $("#txtLoginName").css("background", "url('../GolfShop/img/warning-symbol.png') no-repeat right center");
        //$("#error_" + element.attributes['id'].value).show();
        element.focus();
        $("#hiddenValidation").val("false");
        if ($("#btnUnsubscribe").is(":visible"))
            $("#btnUnsubscribe").attr("disabled", true);
        return false;
    }
    else if (parseInt(fieldValue.length) > parseInt(maxLength))
    {
        fieldLabel.html(fieldName + " must be shorter than " + maxLength + " characters");
        //element.style = "border-color: #8b0300;background: url('../GolfShop/img/warning-symbol.png') no-repeat right center;";
        $("#txtLoginName").css("border-color", "#8b0300");
        $("#txtLoginName").css("background", "url('../GolfShop/img/warning-symbol.png') no-repeat right center");
        element.focus();
        $("#hiddenValidation").val("false");
        if ($("#btnUnsubscribe").is(":visible"))
            $("#btnUnsubscribe").attr("disabled", true);
        return false;
    }
    else
    {
        if (matchPattern.test(fieldValue)) { // This is a valid format
            fieldLabel.html(fieldName);
            //element.style = "background-color: #fff;background: url('../GolfShop/img/green-checkmark-symbol.png') no-repeat right center;";
            $("#txtLoginName").css("border-color", "#fff");
            $("#txtLoginName").css("background", "url('../GolfShop/img/green-checkmark-symbol.png') no-repeat right center");
            $("#txtLoginName").attr("isvalid", "true");
            $("#hiddenValidation").val("true");
            if ($("#btnUnsubscribe").is(":visible"))
                $("#btnUnsubscribe").attr("disabled", false);
            return true;
        }
        else {
            fieldLabel.html(fieldName + " only (" + matchPatternLabel + ")");
            //element.style = "border-color: #8b0300;background: url('../GolfShop/img/warning-symbol.png') no-repeat right center;";
            $("#txtLoginName").css("border-color", "#8b0300");
            $("#txtLoginName").css("background", "url('../GolfShop/img/warning-symbol.png') no-repeat right center");
            element.focus();
            $("#hiddenValidation").val("false");
            if ($("#btnUnsubscribe").is(":visible"))
                $("#btnUnsubscribe").attr("disabled", true);
            return false;
        }
    }
    acceptTermsAndConditions();
    return false;
}

// unsubscribe form validation
function validateUnsubscribeFormFields(element) {

    var minLength = $("#txtUnsubscribe").attr("min-length");
    var maxLength = $("#txtUnsubscribe").attr("max-length");

    var matchPattern = new RegExp($("#txtUnsubscribe").attr("matchpattern"));

    var matchPatternLabel = $("#txtUnsubscribe").attr("patterndescription");

    var fieldLabel = $("#" + element.id.replace("txt", "span"));
    var fieldName = $("#txtUnsubscribe").attr("lbl");
    var fieldValue = $("#txtUnsubscribe").val();

    if (parseInt(fieldValue.length) < parseInt(minLength)) {
        fieldLabel.html(fieldName + " must be longer than " + minLength + " characters");
        //element.style = "border-color: #8b0300;background: url('../GolfShop/img/warning-symbol.png') no-repeat right center;";
        $("#txtUnsubscribe").css("border-color", "#8b0300");
        $("#txtUnsubscribe").css("background", "url('../GolfShop/img/warning-symbol.png') no-repeat right center");
        //$("#error_" + element.attributes['id'].value).show();
        element.focus();
        $("#hiddenValidation").val("false");
        if ($("#btnUnsubscribe").is(":visible"))
            $("#btnUnsubscribe").attr("disabled", true);
        return false;
    }
    else if (parseInt(fieldValue.length) > parseInt(maxLength)) {
        fieldLabel.html(fieldName + " must be shorter than " + maxLength + " characters");
        //element.style = "border-color: #8b0300;background: url('../GolfShop/img/warning-symbol.png') no-repeat right center;";
        $("#txtUnsubscribe").css("border-color", "#8b0300");
        $("#txtUnsubscribe").css("background", "url('../GolfShop/img/warning-symbol.png') no-repeat right center");
        element.focus();
        $("#hiddenValidation").val("false");
        if ($("#btnUnsubscribe").is(":visible"))
            $("#btnUnsubscribe").attr("disabled", true);
        return false;
    }
    else {
        if (matchPattern.test(fieldValue)) { // This is a valid format
            fieldLabel.html(fieldName);
            //element.style = "background-color: #fff;background: url('../GolfShop/img/green-checkmark-symbol.png') no-repeat right center;";
            $("#txtUnsubscribe").css("border-color", "#b3b3b3");
            $("#txtUnsubscribe").css("background", "url('../GolfShop/img/green-checkmark-symbol.png') no-repeat right center");
            $("#txtUnsubscribe").attr("isvalid", "true");
            $("#hiddenValidation").val("true");
            if ($("#btnUnsubscribe").is(":visible"))
                $("#btnUnsubscribe").attr("disabled", false);
            return true;
        }
        else {
            fieldLabel.html(fieldName + " only (" + matchPatternLabel + ")");
            //element.style = "border-color: #8b0300;background: url('../GolfShop/img/warning-symbol.png') no-repeat right center;";
            $("#txtUnsubscribe").css("border-color", "#8b0300");
            $("#txtUnsubscribe").css("background", "url('../GolfShop/img/warning-symbol.png') no-repeat right center");
            element.focus();
            $("#hiddenValidation").val("false");
            if ($("#btnUnsubscribe").is(":visible"))
                $("#btnUnsubscribe").attr("disabled", true);
            return false;
        }
    }
    acceptTermsAndConditions();
    return false;
}

function validateRegistrationFormFields(element) {

    var minLength = $("#txtEmailAdr").attr("min-length");
    var maxLength = $("#txtEmailAdr").attr("max-length");

    var matchPattern = new RegExp($("#txtEmailAdr").attr("matchpattern"));

    var matchPatternLabel = $("#txtEmailAdr").attr("patterndescription");

    var fieldLabel = $("#" + element.id.replace("txt", "span"));
    var fieldName = $("#txtEmailAdr").attr("lbl");
    var fieldValue = $("#txtEmailAdr").val();

    if (parseInt(fieldValue.length) < parseInt(minLength)) {
        fieldLabel.html(fieldName + " must be longer than " + minLength + " characters");
        //element.style = "border-color: #8b0300;background: url('../GolfShop/img/warning-symbol.png') no-repeat right center;";
        $("#txtEmailAdr").css("border-color", "#8b0300");
        $("#txtEmailAdr").css("background", "url('../GolfShop/img/warning-symbol.png') no-repeat right center");
        //$("#error_" + element.attributes['id'].value).show();
        element.focus();
        $("#hiddenValidation").val("false");
        if ($("#btnUnsubscribe").is(":visible"))
            $("#btnUnsubscribe").attr("disabled", true);
        return false;
    }
    else if (parseInt(fieldValue.length) > parseInt(maxLength)) {
        fieldLabel.html(fieldName + " must be shorter than " + maxLength + " characters");
        //element.style = "border-color: #8b0300;background: url('../GolfShop/img/warning-symbol.png') no-repeat right center;";
        $("#txtEmailAdr").css("border-color", "#8b0300");
        $("#txtEmailAdr").css("background", "url('../GolfShop/img/warning-symbol.png') no-repeat right center");
        element.focus();
        $("#hiddenValidation").val("false");
        if ($("#btnUnsubscribe").is(":visible"))
            $("#btnUnsubscribe").attr("disabled", true);
        return false;
    }
    else {
        if (matchPattern.test(fieldValue)) { // This is a valid format
            fieldLabel.html(fieldName);
            //element.style = "background-color: #fff;background: url('../GolfShop/img/green-checkmark-symbol.png') no-repeat right center;";
            $("#txtEmailAdr").css("border-color", "#b3b3b3");
            $("#txtEmailAdr").css("background", "url('../GolfShop/img/green-checkmark-symbol.png') no-repeat right center");
            $("#txtEmailAdr").attr("isvalid", "true");
            $("#hiddenValidation").val("true");
            if ($("#btnUnsubscribe").is(":visible"))
                $("#btnUnsubscribe").attr("disabled", false);
            return true;
        }
        else {
            fieldLabel.html(fieldName + " only (" + matchPatternLabel + ")");
            //element.style = "border-color: #8b0300;background: url('../GolfShop/img/warning-symbol.png') no-repeat right center;";
            $("#txtEmailAdr").css("border-color", "#8b0300");
            $("#txtEmailAdr").css("background", "url('../GolfShop/img/warning-symbol.png') no-repeat right center");
            element.focus();
            $("#hiddenValidation").val("false");
            if ($("#btnUnsubscribe").is(":visible"))
                $("#btnUnsubscribe").attr("disabled", true);
            return false;
        }
    }
    acceptTermsAndConditions();
    return false;
}

// Validate all registration form fields
function validateRegistrationForm() {

    var formFields = document.getElementsByTagName("input");
    var currentField = null;
    var invalidField = false;
    var errorMessage = "Please check the following input values: \n \n";

    for (i = 0; i < formFields.length - 1; i++) {
        currentField = formFields[i];
        if (currentField != undefined) {
            if (currentField.attributes["isrequired"] != null) {

                if (currentField.attributes["isrequired"].value == "true") {

                    formIsValid = validateRegistrationFormFields(currentField);

                    if (!formIsValid) {
                        invalidField = true;
                        errorMessage += currentField.attributes["lbl"].value + "\n";
                        currentField.focus();
                        acceptTermsAndConditions();
                    }
                }
            }
        }
    }

    if (invalidField == true) {
        alert(errorMessage);
        return false;
    }

    $("#hiddenAA").val("registerUser");
    acceptTermsAndConditions();
    PageMethods.btnRegisterEndUser_Click();

}

function acceptTermsAndConditions() {
    // validate form fields
    var formFields = document.getElementsByTagName("input");
    var currentField = null;
    var invalidField = false;
    var errorMessage = "Please check the following input values: \n \n";

    for (i = 0; i < formFields.length - 1; i++) {
        currentField = formFields[i];
        
        if (currentField != undefined) {
            if (currentField.attributes["isrequired"] != null) {
                
                if (currentField.attributes["isrequired"].value == "true") {
                    formIsValid = validateRegistrationFormFields(currentField);

                    if (!formIsValid) {
                        invalidField = true;
                        //errorMessage += currentField.attributes["lbl"].value + "\n";
                        //currentField.focus();
                    }
                }
            }
        }
    }

    if (invalidField == true) {
        $("#btnRegisterEndUser").attr("disabled", true);
        return false;
    }

    // get captcha value
    var captcha = $("#txtimgcode").val();

    // get terms and conditions checkbox
    var currentState = $("#chkTermsAndConditions").prop("checked");

    if (currentState == true && invalidField == false && captcha != "") {
        $("#btnRegisterEndUser").attr("disabled", false);
    }
    else {
        $("#btnRegisterEndUser").attr("disabled", true);
    }
    return;
}

//not used?
//function submitOtpRequest(requestType) {

//    var mainForm = document.getElementById("formMain");
//    var pageAction = document.getElementById("hiddenAA");
//    var userLoginName = document.getElementById("txtUsername_" + requestType);
//    var hiddenAB = document.getElementById("hiddenAB");

//    if (userLoginName != null)
//        hiddenAB.value = userLoginName.value;

//    pageAction.value = "requestotp";

//    mainForm.action += "?DisplyMode=" + requestType;

//    if (userLoginName != null) {
//        if (userLoginName.value == "") {
//            document.getElementById("AdminLoginResult_Desktop").innerHTML = "<span style='color: #ff0000;'>Enter a Login Name!</span>";
//            userLoginName.focus();
//            return false;
//        }
//        else {
//            ShowProcessingMessage();
//            mainForm.submit();
//            return true;
//        }
//    }
//}

//not used?
//function submitOtpLogon(requestType) {

//    var userPassword = document.getElementById("txtPassword_" + requestType);

//    if (userPassword != null) {
//        if (userPassword.value == "") {
//            document.getElementById("AdminLoginResult_Desktop").innerHTML = "<span style='color: #ff0000;'>Please enter your OTP/Password!</span>";
//            userPassword.focus();
//            return false;
//        }
//        else {
//            var hiddenAC= document.getElementById("hiddenAC");

//            hiddenAC.value = userPassword.value;

//            document.getElementById("hiddenAA").value = "login";

//            ShowProcessingMessage();
//            document.getElementById("formMain").submit();
//            return true;
//        }
//    }
//}

//not used?
//function submitLogonRequest() {

//    var submitButton = document.getElementById("AdminLoginButton").value;
//    var pageAction = document.getElementById("hiddenAA");

//    switch (submitButton) {
//        case "Login":
//            pageAction.value = "login";
//            break;

//        case "Request OTP":
//            pageAction.value = "requestotp";
//            break;

//        default:
//            pageAction.value = "login";
//            break;
//    }

//    var userName = document.getElementById("txtUsername");
//    if (userName != null) {
//        if (userName.value == "") {
//            alert("Please enter a Login Name!");
//            document.getElementById("txtUsername").focus();
//        }
//        else {
//            ShowProcessingMessage();
//            document.getElementById("formMain").submit();
//        }
//    }
//    else {
//        var userPassword = document.getElementById("txtPassword");
//        if (userPassword != null) {
//            if (userPassword.value == "") {
//                alert("Please enter the " + document.getElementById("spanLoginResultDetails").innerHTML);
//                document.getElementById("txtPassword").focus();
//            }
//            else {
//                ShowProcessingMessage();
//                document.getElementById("formMain").submit();
//            }
//        }
//    }
//}

//not used?
function StringToHex(pInString) {
    var outHex = '';
    for (var i = 0; i < pInString.length; i++) {
        outHex += '' + pInString.charCodeAt(i).toString(16);
    }
    return outHex;
}

//not used?
function HexToString(pInHex) {
    var outString = '';
    for (var i = 0; i < pInHex.length; i += 2) {
        outString += String.fromCharCode(parseInt(pInHex.substr(i, 2), 16));
    }
    return outString;
}

//not used?
//function replaceCarriageReturnsAndLinebreaks(stringToConvert)
//{
//    var regexPattern = /\r?\n|\r/g;
//    var replacementCharacter = "|";
//    stringToConvert = stringToConvert.replace(regexPattern, replacementCharacter);

//    return stringToConvert;
//}

function ShowProcessingMessage() {
    // Scroll window to top. If not, then if the browser is towards the bottom of the window, the dialog is out of view at the top
    window.scrollTo(0, 0);

    var loaderMargin = (screenHeight / 2) - 18;

    $('#divDialogContainer').css('margin-top', loaderMargin);

    $('#divPleaseWaitProcessing').css("height", screenHeight);
    $('#divPleaseWaitProcessing').css("width", screenWidth);
    $('#divPleaseWaitProcessing').show();
    $('#divDialogContainer').show();
}

//not used?
function toHex(str) {
    var hex = '';
    for (var i = 0; i < str.length; i++) {
        hex += '' + str.charCodeAt(i).toString(16);
    }
    return hex;
}

function numberWithCommas(x) {
    if (x != null && x != "")
        return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    else
        return "0";
}
//not used?
function CloseWindow() {
    window.close();
}
