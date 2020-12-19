
var DefaultClientId = "530f6e8e675c9b1854a6970b";

var Localhost = "http://localhost";
var Demo = "http://Demo.otp-ap.us";
var ProductionStaging = "http://ProductionStaging.otp-ap.us";
var Production = "http://Production.otp-ap.us";
var QA = "http://qa.otp-ap.us";
var Test = "http://test.otp-ap.us";

// OTP Services -->
var RequestOtpWebService = "/MacServices/Otp/RequestOtp.asmx/WsRequestOtp";
var VerifyOtpWebService = "/MacServices/Otp/ValidateOtp.asmx/WsValidateOtp";
var MessageBroadcastReplyService = "/MacServices/Otp/MessageBroadcastReplyService.asmx/WsMessageBroadcastReplyService";
// Registration services -->
var SecureTraidingRegisterUserWebService = "/MacServices/User/StsEndUserRegistration.asmx/WsStsEndUserRegistration";
var RegisteredUserWebService = "/MacServices/User/EndUserRegistration.asmx/WsEndUserRegistration";
var EndUserManagementWebService = "/MacServices/User/EndUserManagement.asmx/WsEndUserManagement";
var EndUserFileRegistrationWebService = "/MacServices/User/EndUserFileRegistration.asmx/WsEndUserFileRegistration";
var EndUserCompleteRegistrationWebService = "/User/EndUserCompleteRegistration.asmx/WsEndUserCompleteRegistration";
// Open Access Services -->
var MacOpenEndUserServices = "/MacServices/OAS/OpenEndUserServices.asmx/WsOpenEndUserServices";
var MacOpenClientServices = "/MacServices/OAS/OpenClientServices.asmx/WsOpenClientServices";
// Admin Services -->
var MacUsageBilling = "/MacServices/AdminServices/UsageBilling.asmx/WsUsageBilling";
var MacSystemStats = "/MacServices/AdminServices/SystemStats.asmx/WsSystemStats";
var MacManageTypeDefsService = "/MacServices/AdminServices/ManageTypeDefsService.asmx/WsManageTypeDefsService";
var MacGroupInfo = "/MacServices/AdminServices/GroupInfo.asmx/WsGroupInfo";
var MacClientServices = "/MacServices/AdminServices/ClientServices.asmx/WsClientServices";
// Test Services
var MacTestAdService = "/MacServices/Test/TestAdService.asmx/WsTestAdService";
var GetTestClientsInfoUrl = "/MacServices/Test/GetClients.asmx/WsGetClients";
var MacTestBankUrl = "/MacServices/Test/MacTestBank.asmx/WsMacTestBank";
var ClientServicesUrl = "/MacServices/AdminServices/ClientServices.asmx/WsClientServices";
var MacTestReplyUrl = "/MacServices/Test/GetReply.asmx/WsGetReply";

var TwilioAPIService = "/MacServices/Otp/TwilioAPIService.asmx/WsTwilioAPIService";
var MessageBroadcastAPIService = "/MacServices/Otp/MessageBroadcastAPIService.asmx/WsMessageBroadcastAPIService";

// Demo merchant's name list
var Shop = "Scottsdale Golf Shop";
var Store = "Scottsdale Golf Store";
var ElecCo = "The Electric Company";
var WaterCo = "The Water Company";

// ---------------------------------------------------------------------------
// ----------- Service Tester page Functions ---------------------------------
//----------------------------------------------------------------------------
function execute_click() {
    // get system under test name from web page
    ClearLog1();
    $("#lbSUTNameError").html("");
    var sutname = $('#txtSUTName').val();
    if (sutname.length == 0) {
        $("#lbPassFail").html("fail");
        $("#lbSUTNameError").html("<-");
        $("#lbResults").html("Error: no SUT Name");
        return;
    }
    var baseUri = getBaseUriUsingSUTName(sutname);
    if (baseUri.length == 0) {
        $("#lbPassFail").html("fail");
        $("#lbSUTNameError").html("<-");
        $("#lbResults").html("Error: invalid SUT Name[" + sutname + "]");
        return;
    }
    $("#lbSUTNameError").html("");
    $('#lbUrl').html(baseUri);
    // get service code from web page
    var servicecode = $('#txtServiceCode').val();
    if (servicecode.length == 0) {
        $("#lbPassFail").html("fail");
        $("#lbResults").html("Error: no service code");
        return;
    }
    var serviceUri = getServiceUriUsingServiceCode(servicecode);
    if (serviceUri.length == 0) {
        $("#lbPassFail").html("fail");
        $("#lbResults").html("Error: invalid service code[" + servicecode + "]");
        return;
    }
    $('#lbUrl').html(baseUri + serviceUri);

    var requestData = "Request:Ping";
    requestData += "|CID:" + DefaultClientId;
    requestData += "|API:OPS";
    var data = "Data=99" + DefaultClientId.length.toString() + DefaultClientId.toUpperCase() + StringToHex(requestData);
    $.post(baseUri + serviceUri, data, execute_Reply);
}

function execute_Reply(pResponse) {
    var result = "";
    var passfail = "";

    $(pResponse).find('Error').each(function () {
        result = $(this).text();
    });
    if (result.length != 0) { //error
        passfail = "Fail";
    } else {
        $(pResponse).find('Reply').each(function () {
            passfail = "Pass";
            result = $(this).text();
        });
    }
    $("#lbPassFail").html(passfail);
    $("#lbResults").html(result);
    Log($('#lbServiceName').html() + ":" + passfail);
    Log1($('#lbServiceName').html() + ":" + passfail);
}

function Log(txt) {
    var currenttest = $('#lbLog').html();
    if (currenttest.length == 0)
        $('#lbLog').html(txt);
    else
        $('#lbLog').html(currenttest + "," + txt);
}

function getServiceUriUsingServiceCode(servicecode) {
    switch(servicecode) {
        case "RO":        //RO = RequestOTP
            $('#lbServiceName').html("RequestOTP");
            return RequestOtpWebService;
        case "VO":        //VO = VerifyOTP
            $('#lbServiceName').html("VerifyOTP");
            return VerifyOtpWebService;
        case "UR":        //UR = EndUserRegistration
            $('#lbServiceName').val("EndUserRegistration");
            return RegisteredUserWebService;
        case "CR":        //CR = EndUserCompleteRegistration
            $('#lbServiceName').val("EndUserCompleteRegistration");
            return EndUserCompleteRegistrationWebService;
        case "FR":        //FR = EndUserFileRegistration
            $('#lbServiceName').val("EndUserFileRegistration");
            return EndUserFileRegistrationWebService;
        case "UM":        //UM = EndUserManagement
            $('#lbServiceName').val("EndUserManagement");
            return EndUserManagementWebService;
        case "SR":        //SR = StsEndUserRegistration
            $('#lbServiceName').val("StsEndUserRegistration");
            return SecureTraidingRegisterUserWebService;
        case "OU":        //OU = OpenEndUserServices
            $('#lbServiceName').val("OpenEndUserServices");
            return MacOpenEndUserServices;
        case "OC":        //OC = OpenClientServices
            $('#lbServiceName').val("OpenClientServices");
            return MacOpenClientServices;
        case "EH":        //EH = EventHistory
            $('#lbServiceName').val("EventHistory");
            return "";
        case "ES":        //ES = EventHistoryStats
            $('#lbServiceName').val("EventHistoryStats");
            return "";
        case "EL":        //EL = EventLog
            $('#lbServiceName').val("EventLog");
            return "";
        case "SS":        //SS = SystemStats
            $('#lbServiceName').val("SystemStats");
            return MacSystemStats;
        case "MT":        //MT = ManageTypeDefsService
            $('#lbServiceName').val("ManageTypeDefsService");
            return MacManageTypeDefsService;
        case "GI":        //GI = GroupInfo
            $('#lbServiceName').val("GroupInfo");
            return MacGroupInfo;
        case "CS":        //CS = ClientServices
            $('#lbServiceName').val("ClientServices");
            return MacClientServices;
        case "UB":        //UB = UsageBilling
            $('#lbServiceName').val("UsageBilling");
            return MacUsageBilling;
        case "RP":        //RP = RegisterProviders
            $('#lbServiceName').val("RegisterProviders");
            return "";
        case "MB":        //MB = MessageBroadcastAPIService
            $('#lbServiceName').val("MessageBroadcastAPIService");
            return MessageBroadcastAPIService;
        case "TW":        //TW = TwilioAPIService
            $('#lbServiceName').val("TwilioAPIService");
            return TwilioAPIService;
        case "WP":        //WP = UserVerificationWhitePagesPro
            $('#lbServiceName').val("UserVerificationWhitePagesPro");
            return "";
        case "LN":        //LN = UserVerificationLexisNexis
            $('#lbServiceName').val("UserVerificationLexisNexis");
            return "";
        case "TA":        //TA = TestAdService
            $('#lbServiceName').val("TestAdService");
            return MacTestAdService;
        case "ET":        //ET = EventLogTests
            $('#lbServiceName').val("EventLogTests");
            return "";
        case "GC":        //GC = GetClients
            $('#lbServiceName').val("GetClients");
            return "";
        case "LI":        //LI = GetTestLoginInfo
            $('#lbServiceName').val("GetTestLoginInfo");
            return "";
        case "TB":        //TB = MacTestBank
            $('#lbServiceName').val("MacTestBank");
            return MacTestBankUrl;
        default:
            $('#lbServiceName').val("Invalid");
    }
    return "";
}

function testcExists_click() {
    var lbShop = $("#lbShop").html();
    var lbStore = $("#lbStore").html();
    var lbElecCo = $("#lbElecCo").html();
    var lbWaterCo = $("#lbWaterCo").html();
    if (lbShop.length == 0) {
        getAccountFromMacTestBank(Shop);
    }
    else if (lbStore.length == 0) {
        getAccountFromMacTestBank(Store);
    } else if (lbElecCo.length == 0) {
        getAccountFromMacTestBank(ElecCo);
    } else if (lbWaterCo.length == 0) {
        getAccountFromMacTestBank(WaterCo);
    }
}
function testShopExists_click() {
    getAccountFromMacTestBank(Shop);
}
function testStoreExists_click() {
    getAccountFromMacTestBank(Store);
}
function testElecCoExists_click() {
    getAccountFromMacTestBank(ElecCo);
}
function testWaterCoExists_click() {
    getAccountFromMacTestBank(WaterCo);
}

function getAccountFromMacTestBank(accountholdername) {
    // get system under test name from web page
    var sutname = $('#txtSUTName').val();
    if (sutname.length == 0) {
        $("#lbSUTNameError").html("<-");
        Log1("Error: no SUT Name");
        return;
    }
    var baseUri = getBaseUriUsingSUTName(sutname);
    if (baseUri.length == 0) {
        $("#lbSUTNameError").html("<-");
        Log1("Error: invalid SUT Name[" + sutname + "]");
        return;
    }
    $("#lbSUTNameError").html("");
    //<!--Request:GetAccountDetails|AccountHolder:Kohl's|CID:53ed31bb74846912e08d5789--> 
    var requestData = "Request:GetAccountDetails";
    requestData += "|CID:" + DefaultClientId;
    requestData += "|AccountHolder:" + accountholdername;
    requestData += "|API:OPS";
    Log1(requestData);
    var data = "Data=99" + DefaultClientId.length.toString() + DefaultClientId.toUpperCase() + StringToHex(requestData);
    $.post(baseUri + MacTestBankUrl, data, getAccountFromMacTestBank_Reply);
}

function getAccountFromMacTestBank_Reply(pResponse) {
    Log1("getAccountFromMacTestBank_Reply");
    var result = "";
    var AccountHolder = "";
    $(pResponse).find('Error').each(function () {
        result = $(this).text();
    });
    if (result.length == 0) { //error
        $(pResponse).find('Reply').each(function () {
            result = $(this).text();
        });
        $(pResponse).find('AccountHolder').each(function () {
            AccountHolder = $(this).text();
        });
    }
    Log1(AccountHolder + ":" + result);
    if (AccountHolder == Shop) {
        $("#lbShop").html(result);
    } else if (AccountHolder == Store) {
        $("#lbStore").html(result);
    } else if (AccountHolder == ElecCo) {
        $("#lbElecCo").html(result);
    } else if (AccountHolder == WaterCo) {
        $("#lbWaterCo").html(result);
    }
}

// ---------------------------------------------------------------------------
// ----------- FI_Only_Client page Functions ---------------------------------
//----------------------------------------------------------------------------
function Fill() {
    $('#txtSUTName').val("localhost");
    $('#txtClientName').val("Kohl's");
    $('#txtEmail').val("tdavis@mobileauthcorp.com");
    $('#txtPhone').val("4802684076");
    $('#txtLastName').val("Davis");
    $('#txtUID').val("tdavis@mobileauthcorp.com");
}

function btnGetClientId_Clicked() {
    Log1("btnGetClientId");
    // get system under test name from web page
    var sutname = $('#txtSUTName').val();
    if (sutname.length == 0) {
        $("#lbResults").html("Error: no SUT Name");
        return;
    }
    var baseUri = getBaseUriUsingSUTName(sutname);
    if (baseUri.length == 0) {
        $("#lbResults").html("Error: invalid SUT Name[" + sutname + "]");
        return;
    }
    Log1(baseUri);
    // get ClientName
    var clientName = $('#txtClientName').val();
    if (clientName.length == 0) {
        $("#lbResults").html("Error: Client Name required!");
        return;
    }
    Log1(clientName);
    var requestData = "Request:GetClientId";
    requestData += "|ClientName:" + clientName;
    requestData += "|API:OPS";
    //Log1(requestData);
    var data = "Data=99" + DefaultClientId.length.toString() + DefaultClientId.toUpperCase() + StringToHex(requestData);
    //(data);
    $.post(baseUri + ClientServicesUrl, data, GetClientId_Reply);
}
function GetClientId_Reply(pResponse) {
    //Log1("GetClientId_Reply");
    var result = "";
    $(pResponse).find('Error').each(function () {
        result = $(this).text();
    });
    if (result.length == 0) { //error
        $(pResponse).find('Reply').each(function () {
            result = $(this).text();
            //Log1("CID:" + result);
            $("#txtClientId").val(result);
        });
    }
    Log1("GetClientId_Reply:" + result);
}

// Request OTP With Registered User
function RequestOTPCR() {
    Log1("RequestOTPCR");
    $("#btnRequestOTPCM").prop('disabled', true);
    $("#btnRequestOTPCR").prop('disabled', true);
    $("#lbResults").html("");
    // get system under test name from web page
    var sutname = $('#txtSUTName').val();
    if (sutname.length == 0) {
        $("#lbResults").html("Error: no SUT Name");
        return;
    }
    var baseUri = getBaseUriUsingSUTName(sutname);
    if (baseUri.length == 0) {
        $("#lbResults").html("Error: invalid SUT Name[" + sutname + "]");
        return;
    }
    var cid = $('#txtClientId').val();
    if (cid.length == 0) {
        $("#lbResults").html("Error: CID required!");
        return;
    }
    var lastname = $('#txtLastName').val();
    if (lastname.length == 0) {
        $("#lbResults").html("Error: Last name required!");
        return;
    }
    var userid = $('#txtUID').val();
    if (cid.length == 0) {
        $("#lbResults").html("Error: UserId required!");
        return;
    }
    var requestData = "Request:SendOtp";
    requestData += "|CID:" + cid;
    requestData += "|UserId:" + HashUserId(lastname, userid);
    requestData += "|TrxType:2";
    requestData += "|TrxDetails:" + StringToHex("total: $19.95");
    requestData += "|ReplyUri:" + StringToHex(baseUri + GetTestClientsInfoUrl);
    requestData += "|API:OPS";
    var data = "Data=99" + cid.length.toString() + cid.toUpperCase() + StringToHex(requestData);
    $.post(baseUri + RequestOtpWebService, data, RequestOTP_Reply);
}
// Request OTP with client managed user (email and Phone supplied)
function RequestOTPCM() {
    Log1("RequestOTPCM");
    $("#btnRequestOTPCM").prop('disabled', true);
    $("#btnRequestOTPCR").prop('disabled', true);
    $("#lbResults").html("");
    // get system under test name from web page
    var sutname = $('#txtSUTName').val();
    if (sutname.length == 0) {
        $("#lbResults").html("Error: no SUT Name");
        return;
    }
    var baseUri = getBaseUriUsingSUTName(sutname);
    if (baseUri.length == 0) {
        $("#lbResults").html("Error: invalid SUT Name[" + sutname + "]");
        return;
    }
    var cid = $('#txtClientId').val();
    if (cid.length == 0) {
        $("#lbResults").html("Error: CID required!");
        return;
    }
    var email = $('#txtEmail').val();
    if (email.length == 0) {
        $("#lbResults").html("Error: email address required!");
        return;
    }
    var phone = $('#txtPhone').val();
    if (phone.length == 0) {
        $("#lbResults").html("Error: UserId required!");
        return;
    }
    var requestData = "Request:SendOtp";
    requestData += "|CID:" + cid;
    requestData += "|EmailAddress:" + email;
    requestData += "|PhoneNumber:" + phone;
    requestData += "|TrxType:2";
    requestData += "|TrxDetails:" + StringToHex("total: $19.95");
    requestData += "|ReplyUri:" + StringToHex(baseUri  + GetTestClientsInfoUrl);
    requestData += "|API:OPS";
    Log1(requestData);
    var data = "Data=99" + cid.length.toString() + cid.toUpperCase() + StringToHex(requestData);
    $.post(baseUri + RequestOtpWebService, data, RequestOTP_Reply);
}
function RequestOTP_Reply(pResponse) {
    Log1("RequestOTP_Reply");
    $("#btnRequestOTPCM").prop('disabled', false);
    $("#btnRequestOTPCR").prop('disabled', false);
    var result = "";
    $(pResponse).find('Error').each(function () {
        result = $(this).text();
    });
    if (result.length == 0) { //error
        $(pResponse).find('Reply').each(function () {
            result = $(this).text();
        });
        $(pResponse).find('RequestId').each(function () {
            $("#txtRID").val($(this).text());
        });
        $(pResponse).find('Debug').each(function () {
            $("#txtOTP").val($(this).text());
        });
    }
    $("#lbResults").html(result);
}

function GetReply() {
    Log1("GetReply");
    $("#btnGetReply").prop('disabled', true);
    $("#btnSendOTPToReplyService").prop('disabled', true);
    $("#lbResults").html("");
    var sutname = $('#txtSUTName').val();
    if (sutname.length == 0) {
        $("#lbResults").html("Error: no SUT Name");
        return;
    }
    var baseUri = getBaseUriUsingSUTName(sutname);
    if (baseUri.length == 0) {
        $("#lbResults").html("Error: invalid SUT Name[" + sutname + "]");
        return;
    }
    var cid = $('#txtClientId').val();
    if (cid.length == 0) {
        $("#lbResults").html("Error: CID required!");
        return;
    }
    var rid = $("#txtRID").val();
    if (rid.length == 0) {
        $("#lbResults").html("Error: request Id required!");
        return;
    }
    var requestData = "Request:CheckForReply";
    requestData += "|CID:" + cid;
    requestData += "|RequestId:" + rid;
    requestData += "|API:OPS";
    Log1(requestData);
    var data = "Data=99" + cid.length.toString() + cid.toUpperCase() + StringToHex(requestData);
    $.post(baseUri + MacTestReplyUrl, data, GetReply_Reply);
}
function GetReply_Reply(pResponse) {
    Log1("GetReply_Reply");
    $("#btnGetReply").prop('disabled', false);
    $("#btnSendOTPToReplyService").prop('disabled', false);
    var result = "";
    $(pResponse).find('Error').each(function () {
        result = $(this).text();
    });
    if (result.length == 0) { //error
        $(pResponse).find('Reply').each(function () {
            result = $(this).text();
        });
    }
    $("#lbResults").html(result);
}

function SendOTPToReplyService_Click() {
    Log1("SendOTPToReplyService");
    $("#lbResults").html("");
    var sutname = $('#txtSUTName').val();
    if (sutname.length == 0) {
        $("#lbResults").html("Error: no SUT Name");
        return;
    }
    var baseUri = getBaseUriUsingSUTName(sutname);
    if (baseUri.length == 0) {
        $("#lbResults").html("Error: invalid SUT Name[" + sutname + "]");
        return;
    }
    var cid = $('#txtClientId').val();
    if (cid.length == 0) {
        $("#lbResults").html("Error: CID required!");
        return;
    }
    var rid = $("#txtRID").val();
    if (rid.length == 0) {
        $("#lbResults").html("Error: request Id required!");
        return;
    }
    var otp = $("#txtOTP").val();
    if (otp.length == 0) {
        $("#lbResults").html("Error: otp required!");
        return;
    }
    var requestData = "Request:VerifyOtp";
    requestData += "|CID:" + cid;
    requestData += "|RequestId:" + rid;
    requestData += "|OTP:" + otp.replace("OTP:","");
    requestData += "|API:OPS";
    Log1(requestData);
    var data = "Data=" + StringToHex(requestData);
    Log1("[" + baseUri + MessageBroadcastReplyService + "]");
    $.post(baseUri + MessageBroadcastReplyService, data, SendOTPToReplyService_Reply);
    Log1("...");
}
function SendOTPToReplyService_Reply(pResponse) {
    Log1("SendOTPToReplyService_Reply");
    var result = "";
    $(pResponse).find('Error').each(function () {
        result = $(this).text();
    });
    if (result.length == 0) { //error
        $(pResponse).find('Reply').each(function () {
            result = $(this).text();
        });
    }
    $("#lbResults").html(result);
}

//----------------------------------------------------------------------------
//----------- Helper Functions -----------------------------------------------
//----------------------------------------------------------------------------

function ClearLog1()
{
    $('#txtLog').val("");
}

function Log1(txt) {
    var currenttest = $('#txtLog').val();
    if (currenttest.length == 0)
        $('#txtLog').val(txt);
    else
        $('#txtLog').val(currenttest + "," + txt);
}

function getBaseUriUsingSUTName(pSUTName) {
    var SUTName = pSUTName.toLowerCase();
    if (SUTName == "localhost")
        return Localhost;
    if (SUTName == "test")
        return Test;
    if (SUTName == "qa")
        return QA;
    if (SUTName == "demo")
        return Demo;
    if (SUTName == "productionstaging")
        return ProductionStaging;
    if (SUTName == "production")
        return Production;
    return "";
}

function selectoptionbyid(ddl_name, optionid) {
    $("#" + ddl_name + " option[id=" + optionid + "]").attr("selected", true);
    $("#" + ddl_name).trigger("chosen:updated");
    // return the selected option's id to insure the option exists
    return $("#" + ddl_name + " option:selected").id();
}

function selectoptionbyvalue(ddl_name, optionvalue) {
    $('#' + ddl_name + ' option[value="' + optionvalue + '"]').attr('selected', true);
    $('#' + ddl_name).trigger('chosen:updated');
}

function selectoptionbytext(formid, ddl_name, text) {
    // find and select the client in the dropdown
    $("#" + ddl_name + " option").filter(function () {
        return ($(this).text() == text);
    }).prop('selected', true);
    // change the chosen client
    $("#" + ddl_name).trigger("chosen:updated");
    // need to submit the form in order to get the client information populated
    $("#" + formid).submit();
    // return the selected option to insure the option exists
    return $("#" + ddl_name + " option:selected").text();
}

function TestDecrypt() {
    var mEvalue = $("#txtEncResult").val();
    var mKey = $("#txtKey").val();
    var mValue = Decrypt(mEvalue, mKey);
    $("#txtClrResult").val(mValue);
}

function Decrypt(value, key) {
    var RequestValue = StringToHex(value);
    var RequestKey = StringToHex(key);
    var result = "";
    var serviceurl = "/MACServices/AdminServices/EncryptDecrypt.asmx/WsDecrypt";
    var mdata = "value:" + RequestValue + "|key:" + RequestKey;
    jQuery.ajaxSetup({ async: false });
    {
        $.get(serviceurl, { data: mdata },
            function (pResponse, status) {
                $(pResponse).find("Reply").each(function () {
                    result = $(this).text();
                });
            });
    }
    jQuery.ajaxSetup({ async: true });
    return HexToString(result);
}

function TestEncrypt() {
    var mValue = $("#txtData").val();
    var mKey = $("#txtKey").val();
    var mEvalue = "";
    mEvalue = Encrypt(mValue, mKey);
    $("#txtEncResult").val(mEvalue);
}

function Encrypt(value, key) {
    var result = "";
    var RequestValue = StringToHex(value);
    var RequestKey = StringToHex(key);
    var serviceurl = "/MACServices/AdminServices/EncryptDecrypt.asmx/WsEncrypt";
    var mdata = "value:" + RequestValue + "|key:" + RequestKey;
    jQuery.ajaxSetup({ async: false });
    {
        $.get(serviceurl, { data: mdata },
            function (pResponse, status) {
                $(pResponse).find("Reply").each(function () {
                    result = $(this).text();
                });
            });
    }
    jQuery.ajaxSetup({ async: true });
    return result;
}

function Hash(pStringToHash) {
    var result = "";
    var mValue = StringToHex(pStringToHash);
    var serviceurl = "/MACServices/AdminServices/EncryptDecrypt.asmx/WsHash";
    var mdata = "value:" + mValue;
    jQuery.ajaxSetup({ async: false });
    {
        $.get(serviceurl, { data: mdata },
            function (pResponse, status) {
                $(pResponse).find("Reply").each(function () {
                    result = $(this).text();
                });
            });
    }
    jQuery.ajaxSetup({ async: true });
    return result;
}


function StringToHex(pInString) {
    var outHex = '';
    for (var i = 0; i < pInString.length; i++) {
        outHex += '' + pInString.charCodeAt(i).toString(16);
    }
    return outHex;
}

function HexToString(pInHex) {
    var outString = '';
    for (var i = 0; i < pInHex.length; i += 2) {
        outString += String.fromCharCode(parseInt(pInHex.substr(i, 2), 16));
    }
    return outString;
}

function HashUserId(pLastName, pUniqueIdentifier) {
    return hex_md5(pLastName.toLowerCase() + pUniqueIdentifier.toLowerCase()).toUpperCase();
}
//-------------- hash functions ---------------------------
/*
CryptoJS v3.0.2
code.google.com/p/crypto-js
(c) 2009-2012 by Jeff Mott. All rights reserved.
code.google.com/p/crypto-js/wiki/License
*/
//alert("hash");
//var hash = hex_md5("Message");
//alert(hash);
/*
 * A JavaScript implementation of the RSA Data Security, Inc. MD5 Message
 * Digest Algorithm, as defined in RFC 1321.
 * Version 2.2 Copyright (C) Paul Johnston 1999 - 2009
 * Other contributors: Greg Holt, Andrew Kepert, Ydnar, Lostinet
 * Distributed under the BSD License
 * See http://pajhome.org.uk/crypt/md5 for more info.
 */

/*
 * Configurable variables. You may need to tweak these to be compatible with
 * the server-side, but the defaults work in most cases.
 */
var hexcase = 0;   /* hex output format. 0 - lowercase; 1 - uppercase        */
var b64pad = "";  /* base-64 pad character. "=" for strict RFC compliance   */

/*
 * These are the functions you'll usually want to call
 * They take string arguments and return either hex or base-64 encoded strings
 */
function hex_md5(s) { return rstr2hex(rstr_md5(str2rstr_utf8(s))); }
function b64_md5(s) { return rstr2b64(rstr_md5(str2rstr_utf8(s))); }
function any_md5(s, e) { return rstr2any(rstr_md5(str2rstr_utf8(s)), e); }
function hex_hmac_md5(k, d)
{ return rstr2hex(rstr_hmac_md5(str2rstr_utf8(k), str2rstr_utf8(d))); }
function b64_hmac_md5(k, d)
{ return rstr2b64(rstr_hmac_md5(str2rstr_utf8(k), str2rstr_utf8(d))); }
function any_hmac_md5(k, d, e)
{ return rstr2any(rstr_hmac_md5(str2rstr_utf8(k), str2rstr_utf8(d)), e); }

/*
 * Perform a simple self-test to see if the VM is working
 */
function md5_vm_test() {
    return hex_md5("abc").toLowerCase() == "900150983cd24fb0d6963f7d28e17f72";
}

/*
 * Calculate the MD5 of a raw string
 */
function rstr_md5(s) {
    return binl2rstr(binl_md5(rstr2binl(s), s.length * 8));
}

/*
 * Calculate the HMAC-MD5, of a key and some data (raw strings)
 */
function rstr_hmac_md5(key, data) {
    var bkey = rstr2binl(key);
    if (bkey.length > 16) bkey = binl_md5(bkey, key.length * 8);

    var ipad = Array(16), opad = Array(16);
    for (var i = 0; i < 16; i++) {
        ipad[i] = bkey[i] ^ 0x36363636;
        opad[i] = bkey[i] ^ 0x5C5C5C5C;
    }

    var hash = binl_md5(ipad.concat(rstr2binl(data)), 512 + data.length * 8);
    return binl2rstr(binl_md5(opad.concat(hash), 512 + 128));
}

/*
 * Convert a raw string to a hex string
 */
function rstr2hex(input) {
    try {
        hexcase = -1;
    } catch (e) { hexcase = 0; }
    var hexTab = hexcase ? "0123456789ABCDEF" : "0123456789abcdef";
    var output = "";
    var x;
    for (var i = 0; i < input.length; i++) {
        x = input.charCodeAt(i);
        output += hexTab.charAt((x >>> 4) & 0x0F)
               + hexTab.charAt(x & 0x0F);
    }
    return output;
}

/*
 * Convert a raw string to a base-64 string
 */
function rstr2b64(input) {
    try {
        b64pad = '';
    } catch (e) { b64pad = ''; }
    var tab = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
    var output = "";
    var len = input.length;
    for (var i = 0; i < len; i += 3) {
        var triplet = (input.charCodeAt(i) << 16)
                    | (i + 1 < len ? input.charCodeAt(i + 1) << 8 : 0)
                    | (i + 2 < len ? input.charCodeAt(i + 2) : 0);
        for (var j = 0; j < 4; j++) {
            if (i * 8 + j * 6 > input.length * 8) output += b64pad;
            else output += tab.charAt((triplet >>> 6 * (3 - j)) & 0x3F);
        }
    }
    return output;
}

/*
 * Convert a raw string to an arbitrary string encoding
 */
function rstr2any(input, encoding) {
    var divisor = encoding.length;
    var i, j, q, x, quotient;

    /* Convert to an array of 16-bit big-endian values, forming the dividend */
    var dividend = Array(Math.ceil(input.length / 2));
    for (i = 0; i < dividend.length; i++) {
        dividend[i] = (input.charCodeAt(i * 2) << 8) | input.charCodeAt(i * 2 + 1);
    }

    /*
     * Repeatedly perform a long division. The binary array forms the dividend,
     * the length of the encoding is the divisor. Once computed, the quotient
     * forms the dividend for the next step. All remainders are stored for later
     * use.
     */
    var fullLength = Math.ceil(input.length * 8 /
                                      (Math.log(encoding.length) / Math.log(2)));
    var remainders = Array(fullLength);
    for (j = 0; j < fullLength; j++) {
        quotient = Array();
        x = 0;
        for (i = 0; i < dividend.length; i++) {
            x = (x << 16) + dividend[i];
            q = Math.floor(x / divisor);
            x -= q * divisor;
            if (quotient.length > 0 || q > 0)
                quotient[quotient.length] = q;
        }
        remainders[j] = x;
        dividend = quotient;
    }

    /* Convert the remainders to the output string */
    var output = "";
    for (i = remainders.length - 1; i >= 0; i--)
        output += encoding.charAt(remainders[i]);

    return output;
}

/*
 * Encode a string as utf-8.
 * For efficiency, this assumes the input is valid utf-16.
 */
function str2rstr_utf8(input) {
    var output = "";
    var i = -1;
    var x, y;

    while (++i < input.length) {
        /* Decode utf-16 surrogate pairs */
        x = input.charCodeAt(i);
        y = i + 1 < input.length ? input.charCodeAt(i + 1) : 0;
        if (0xD800 <= x && x <= 0xDBFF && 0xDC00 <= y && y <= 0xDFFF) {
            x = 0x10000 + ((x & 0x03FF) << 10) + (y & 0x03FF);
            i++;
        }

        /* Encode output as utf-8 */
        if (x <= 0x7F)
            output += String.fromCharCode(x);
        else if (x <= 0x7FF)
            output += String.fromCharCode(0xC0 | ((x >>> 6) & 0x1F),
                                          0x80 | (x & 0x3F));
        else if (x <= 0xFFFF)
            output += String.fromCharCode(0xE0 | ((x >>> 12) & 0x0F),
                                          0x80 | ((x >>> 6) & 0x3F),
                                          0x80 | (x & 0x3F));
        else if (x <= 0x1FFFFF)
            output += String.fromCharCode(0xF0 | ((x >>> 18) & 0x07),
                                          0x80 | ((x >>> 12) & 0x3F),
                                          0x80 | ((x >>> 6) & 0x3F),
                                          0x80 | (x & 0x3F));
    }
    return output;
}

/*
 * Encode a string as utf-16
 */
function str2rstr_utf16le(input) {
    var output = "";
    for (var i = 0; i < input.length; i++)
        output += String.fromCharCode(input.charCodeAt(i) & 0xFF,
                                      (input.charCodeAt(i) >>> 8) & 0xFF);
    return output;
}

function str2rstr_utf16be(input) {
    var output = "";
    for (var i = 0; i < input.length; i++)
        output += String.fromCharCode((input.charCodeAt(i) >>> 8) & 0xFF,
                                       input.charCodeAt(i) & 0xFF);
    return output;
}

/*
 * Convert a raw string to an array of little-endian words
 * Characters >255 have their high-byte silently ignored.
 */
function rstr2binl(input) {
    var output = Array(input.length >> 2);
    var i;
    for (i = 0; i < output.length; i++)
        output[i] = 0;
    for (i = 0; i < input.length * 8; i += 8)
        output[i >> 5] |= (input.charCodeAt(i / 8) & 0xFF) << (i % 32);
    return output;
}

/*
 * Convert an array of little-endian words to a string
 */
function binl2rstr(input) {
    var output = "";
    for (var i = 0; i < input.length * 32; i += 8)
        output += String.fromCharCode((input[i >> 5] >>> (i % 32)) & 0xFF);
    return output;
}

/*
 * Calculate the MD5 of an array of little-endian words, and a bit length.
 */
function binl_md5(x, len) {
    /* append padding */
    x[len >> 5] |= 0x80 << ((len) % 32);
    x[(((len + 64) >>> 9) << 4) + 14] = len;

    var a = 1732584193;
    var b = -271733879;
    var c = -1732584194;
    var d = 271733878;

    for (var i = 0; i < x.length; i += 16) {
        var olda = a;
        var oldb = b;
        var oldc = c;
        var oldd = d;

        a = md5_ff(a, b, c, d, x[i + 0], 7, -680876936);
        d = md5_ff(d, a, b, c, x[i + 1], 12, -389564586);
        c = md5_ff(c, d, a, b, x[i + 2], 17, 606105819);
        b = md5_ff(b, c, d, a, x[i + 3], 22, -1044525330);
        a = md5_ff(a, b, c, d, x[i + 4], 7, -176418897);
        d = md5_ff(d, a, b, c, x[i + 5], 12, 1200080426);
        c = md5_ff(c, d, a, b, x[i + 6], 17, -1473231341);
        b = md5_ff(b, c, d, a, x[i + 7], 22, -45705983);
        a = md5_ff(a, b, c, d, x[i + 8], 7, 1770035416);
        d = md5_ff(d, a, b, c, x[i + 9], 12, -1958414417);
        c = md5_ff(c, d, a, b, x[i + 10], 17, -42063);
        b = md5_ff(b, c, d, a, x[i + 11], 22, -1990404162);
        a = md5_ff(a, b, c, d, x[i + 12], 7, 1804603682);
        d = md5_ff(d, a, b, c, x[i + 13], 12, -40341101);
        c = md5_ff(c, d, a, b, x[i + 14], 17, -1502002290);
        b = md5_ff(b, c, d, a, x[i + 15], 22, 1236535329);

        a = md5_gg(a, b, c, d, x[i + 1], 5, -165796510);
        d = md5_gg(d, a, b, c, x[i + 6], 9, -1069501632);
        c = md5_gg(c, d, a, b, x[i + 11], 14, 643717713);
        b = md5_gg(b, c, d, a, x[i + 0], 20, -373897302);
        a = md5_gg(a, b, c, d, x[i + 5], 5, -701558691);
        d = md5_gg(d, a, b, c, x[i + 10], 9, 38016083);
        c = md5_gg(c, d, a, b, x[i + 15], 14, -660478335);
        b = md5_gg(b, c, d, a, x[i + 4], 20, -405537848);
        a = md5_gg(a, b, c, d, x[i + 9], 5, 568446438);
        d = md5_gg(d, a, b, c, x[i + 14], 9, -1019803690);
        c = md5_gg(c, d, a, b, x[i + 3], 14, -187363961);
        b = md5_gg(b, c, d, a, x[i + 8], 20, 1163531501);
        a = md5_gg(a, b, c, d, x[i + 13], 5, -1444681467);
        d = md5_gg(d, a, b, c, x[i + 2], 9, -51403784);
        c = md5_gg(c, d, a, b, x[i + 7], 14, 1735328473);
        b = md5_gg(b, c, d, a, x[i + 12], 20, -1926607734);

        a = md5_hh(a, b, c, d, x[i + 5], 4, -378558);
        d = md5_hh(d, a, b, c, x[i + 8], 11, -2022574463);
        c = md5_hh(c, d, a, b, x[i + 11], 16, 1839030562);
        b = md5_hh(b, c, d, a, x[i + 14], 23, -35309556);
        a = md5_hh(a, b, c, d, x[i + 1], 4, -1530992060);
        d = md5_hh(d, a, b, c, x[i + 4], 11, 1272893353);
        c = md5_hh(c, d, a, b, x[i + 7], 16, -155497632);
        b = md5_hh(b, c, d, a, x[i + 10], 23, -1094730640);
        a = md5_hh(a, b, c, d, x[i + 13], 4, 681279174);
        d = md5_hh(d, a, b, c, x[i + 0], 11, -358537222);
        c = md5_hh(c, d, a, b, x[i + 3], 16, -722521979);
        b = md5_hh(b, c, d, a, x[i + 6], 23, 76029189);
        a = md5_hh(a, b, c, d, x[i + 9], 4, -640364487);
        d = md5_hh(d, a, b, c, x[i + 12], 11, -421815835);
        c = md5_hh(c, d, a, b, x[i + 15], 16, 530742520);
        b = md5_hh(b, c, d, a, x[i + 2], 23, -995338651);

        a = md5_ii(a, b, c, d, x[i + 0], 6, -198630844);
        d = md5_ii(d, a, b, c, x[i + 7], 10, 1126891415);
        c = md5_ii(c, d, a, b, x[i + 14], 15, -1416354905);
        b = md5_ii(b, c, d, a, x[i + 5], 21, -57434055);
        a = md5_ii(a, b, c, d, x[i + 12], 6, 1700485571);
        d = md5_ii(d, a, b, c, x[i + 3], 10, -1894986606);
        c = md5_ii(c, d, a, b, x[i + 10], 15, -1051523);
        b = md5_ii(b, c, d, a, x[i + 1], 21, -2054922799);
        a = md5_ii(a, b, c, d, x[i + 8], 6, 1873313359);
        d = md5_ii(d, a, b, c, x[i + 15], 10, -30611744);
        c = md5_ii(c, d, a, b, x[i + 6], 15, -1560198380);
        b = md5_ii(b, c, d, a, x[i + 13], 21, 1309151649);
        a = md5_ii(a, b, c, d, x[i + 4], 6, -145523070);
        d = md5_ii(d, a, b, c, x[i + 11], 10, -1120210379);
        c = md5_ii(c, d, a, b, x[i + 2], 15, 718787259);
        b = md5_ii(b, c, d, a, x[i + 9], 21, -343485551);

        a = safe_add(a, olda);
        b = safe_add(b, oldb);
        c = safe_add(c, oldc);
        d = safe_add(d, oldd);
    }
    return Array(a, b, c, d);
}

/*
 * These functions implement the four basic operations the algorithm uses.
 */
function md5_cmn(q, a, b, x, s, t) {
    return safe_add(bit_rol(safe_add(safe_add(a, q), safe_add(x, t)), s), b);
}
function md5_ff(a, b, c, d, x, s, t) {
    return md5_cmn((b & c) | ((~b) & d), a, b, x, s, t);
}
function md5_gg(a, b, c, d, x, s, t) {
    return md5_cmn((b & d) | (c & (~d)), a, b, x, s, t);
}
function md5_hh(a, b, c, d, x, s, t) {
    return md5_cmn(b ^ c ^ d, a, b, x, s, t);
}
function md5_ii(a, b, c, d, x, s, t) {
    return md5_cmn(c ^ (b | (~d)), a, b, x, s, t);
}

/*
 * Add integers, wrapping at 2^32. This uses 16-bit operations internally
 * to work around bugs in some JS interpreters.
 */
function safe_add(x, y) {
    var lsw = (x & 0xFFFF) + (y & 0xFFFF);
    var msw = (x >> 16) + (y >> 16) + (lsw >> 16);
    return (msw << 16) | (lsw & 0xFFFF);
}

/*
 * Bitwise rotate a 32-bit number to the left.
 */
function bit_rol(num, cnt) {
    return (num << cnt) | (num >>> (32 - cnt));
}
