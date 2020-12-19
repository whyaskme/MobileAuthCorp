var webMethod = "";
var parameters = "";
var totalRecords = 0;
var totalPages = 0;
var recordsPerPage = 8;
var currentPageNumber = 1;
var lastRecordNumber = 0;
var numberGroupsSelected = 0;
var eventRowCount = 0;

var userIsReadOnly = false;
var userId = false;

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
var screenWidth = $(window).innerWidth();


var left;
var top;

var popupWindowWidth = 0;
var popupWindowHeight = 0;
var eventHistoryTimerEnabled = false;

var paginationEnabled = false;

var windowLocation = window.location.toString().toLowerCase();

$(document).ready(function ()
{
    // Bust any iFrames that don't originate from our domain. This prevents click-jacking.
    //if (self == top) {
    //    var pageBody = document.getElementById("pageBody")[0];
    //    pageBody.style.display = "block";
    //}
    //else
    //    top.location = self.location;

    detectBrowser();

    userIsReadOnly = document.getElementById("hiddenF").value.toString().toLowerCase();
    userId = document.getElementById("hiddenE").value.toString().toLowerCase();

    var tmpVal = windowLocation.split('/');
    currentHost = "http://" + tmpVal[2];

    DisplayMode = "Desktop";

    docBody = $('#pageBody')[0];
    hiddenU = $('#hiddenU')[0];

    if (parseInt(screenWidth) < 641)
        DisplayMode = "Mobile";

    hiddenU.value = DisplayMode;

    $("#pageBody").bind("keydown", function (event) {
        switch (event.keyCode) {
            case 13: // Enter key
                if (windowLocation.indexOf("?") > -1) {
                    var tmpVal = windowLocation.split('?');
                    windowLocation = tmpVal[0];
                }

                switch(windowLocation)
                {
                    case "http://localhost:8080/admin/":
                        submitOtpRequest(hiddenU.value);
                        return false;
                    case "http://localhost:8080/admin/default":
                        submitOtpRequest(hiddenU.value);
                    case "http://localhost:8080/admin/security/login":
                        submitOtpLogon(hiddenU.value);
                        return false;
                }
                return false;
        }
        return true;
    });
});

function updateGroupBillConfig(groupId, configType)
{
    var loggedInAdminId;
    if ($('#hiddenE') !== null)
        loggedInAdminId = $('#hiddenE').val();

    var hiddenF;
    if ($('#hiddenF') !== null)
        hiddenF = $('#hiddenF').val();

    popupWindowWidth = 425;
    popupWindowHeight = 350;

    var targetUrl = "/Admin/Billing/ConfigPopup?loggedInAdminId=" + loggedInAdminId + "&userisreadonly=" + hiddenF + "&ownerId=" + groupId + "&configType=Group";
    showJQueryDialog(targetUrl, popupWindowWidth, popupWindowHeight);
}

function PrintDocument(contentId)
{
    alert("PrintDocument - " + contentId);
}

function UploadGroupLogo()
{
    var userId = document.getElementById("hiddenE").value;
    var ownerId = document.getElementById("spanGroupID").innerHTML;

    if (ownerId != "" && ownerId != undefined) {
        var targetUrl = "/Admin/System/UploadFile?userId=" + userId + "&ownerId=" + ownerId + "&ownerType=Group";

        showJQueryDialog(targetUrl, popupWindowWidth, popupWindowHeight);
    }
    else
    {
        alert("Please select a Group first!");
    }
}

function UploadClientLogo()
{
    var userId = document.getElementById("hiddenE").value;
    var ownerId = document.getElementById("txtClientID").value;

    if (ownerId != "" && ownerId != undefined) {
        var targetUrl = "/Admin/System/UploadFile?userId=" + userId + "&ownerId=" + ownerId + "&ownerType=Client";

        showJQueryDialog(targetUrl, popupWindowWidth, popupWindowHeight);
    }
    else
    {
        alert("Please select a Client first!");
    }
}

function NavigateTopic(topicId) {

    var formDoc = document.getElementById("formMain");

    var formAction = "";

    // Reset hidden field
    document.getElementById("hiddenX").value = "";

    var parentUrlLocation = window.location.toString();

    if (parentUrlLocation.indexOf('?') > -1) {
        parentUrlLocation = parentUrlLocation.split('?');

        var params = parentUrlLocation[1].split('&');
        for (var i = 0; i < params.length; i++)
        {
            var param = params[i].split('=');
            switch (param[0])
            {
                case "userId":
                    userId = param[1];
                    break;
            }
        }
        formAction = "/Admin/Help/Default?u=" + userId + "&tid=" + topicId;
    }
    else
        formAction = "/Admin/Help/Default?u=" + userId + "&tid=" + topicId;

   ShowProcessingMessage();

   formDoc.action = formAction;
   formDoc.submit();
}

function NavigateTopicPopup(topicId) {

    var parentUrlLocation = window.location.toString();

    var formAction = "";

    if (parentUrlLocation.indexOf('?') > -1) {
        parentUrlLocation = parentUrlLocation.split('?');

        var params = parentUrlLocation[1].split('&');
        for (var i = 0; i < params.length; i++) {
            var param = params[i].split('=');
            switch (param[0]) {
                case "userId":
                    userId = param[1];
                    break;
            }
        }
        formAction = "/Admin/Help/Default?u=" + userId + "&tid=" + topicId;
    }
    else
        formAction = "/Admin/Help/Default?u=" + userId + "&tid=" + topicId;

    window.open(formAction);
}

function MenuNavigation(urlToNavigate)
{
    //alert(urlToNavigate);

    var formDoc = document.getElementById("formMain");

    if (urlToNavigate == "/Admin/Help/Default.aspx") {
        urlToNavigate += "?tid=" + document.getElementById("hiddenW").value;
        urlToNavigate += "&u=" + document.getElementById("hiddenE").value;
        window.open(urlToNavigate);
        return false;
    }
    else {
        formDoc.action = urlToNavigate;
        formDoc.submit();
        return false;
    }
}

function detectBrowser()
{
    if (navigator.userAgent.indexOf("Chrome") > -1) {
        if (windowLocation.indexOf("/404") < 1)
            window.location = "/404?errmsg=<p>We apologize for any inconvenience. We don't support Google Chrome at this point in time.</p><p>Please use another web browser</p>";
    }
}

function showBilling()
{
    var forwardingUrl = "/Admin/Billing/Client/Default?cid=" + $('#txtClientID').val();
    document.getElementById("formMain").action = forwardingUrl;
    document.getElementById("formMain").submit();
}

function validateFormFields(element) {
    var minLength = element.attributes["min-length"].value;
    var maxLength = element.attributes["max-length"].value;

    var matchPattern = new RegExp(element.attributes["matchpattern"].value, "i"); // "i" - ignoreCase
    var matchPatternLabel = element.attributes["patterndescription"].value;

    var fieldLabel = $("#" + element.id.replace("txt", "span"));
    var fieldName = element.attributes["lbl"].value;
    var fieldValue = element.value;

    if (fieldName == "Client Name") {
        if (!checkDuplicateClientName(element)) {
            if (fieldValue == "New Client Name?")
                fieldLabel.html("<span style='color: #ff0000;'>" + fieldValue + " is invalid, please enter valid name</span>");
            else
                fieldLabel.html("<span style='color: #ff0000;'>" +  fieldValue + " already exists, please try another name</span>");
            element.style = "border-color: #8b0300;background: url('/Images/warning-symbol.png') no-repeat right center;";
            element.focus();
            return false;
        }
    }

    //alert("matchPattern - " + matchPattern);
    if(parseInt(fieldValue.length) < parseInt(minLength))
    {
        //alert(parseInt(fieldValue.length));
        fieldLabel.html("<span style='color: #ff0000;'>" + fieldName + " must be at least " + minLength + " characters </span>");
        element.style = "border-color: #8b0300;background: url('/Images/warning-symbol.png') no-repeat right center;";
        element.focus();
        return false;
    }
    else if (parseInt(fieldValue.length) > parseInt(maxLength))
    {
        //alert(parseInt(fieldValue.length));
        fieldLabel.html("<span style='color: #ff0000;'>" + fieldName + " must be shorter than " + maxLength + " characters </span>");
        element.style = "border-color: #8b0300;background: url('/Images/warning-symbol.png') no-repeat right center;";
        element.focus();
        return false;
    }
    else
    {
        if (matchPattern.test(fieldValue)) { // This is a valid format
            fieldLabel.html(fieldName);
            element.style = "background: #ffffff url('/Images/green-checkmark-symbol.png') no-repeat right center;";
            element.attributes["isvalid"].value = "true";
            return true;
        }
        else {
            //alert('no match');
            fieldLabel.html("<span style='color: #ff0000;'>" + fieldName + " only (" + matchPatternLabel + ") </span>");
            element.style = "border-color: #8b0300;background: url('/Images/warning-symbol.png') no-repeat right center;";
            element.focus();
            return false;
        }
    }
}

function checkDuplicateClientName(currentClientElement) {
    var currentClientName = currentClientElement.value.trim();
    var clientNames;

    if ($('#hiddenClientList').val() != "") {
        clientNames = $('#hiddenClientList').val().split('|');
        for (var i = 0; i < clientNames.length; i++) {
            var existingClientName = clientNames[i].trim();

            if (currentClientName.toLowerCase() == existingClientName.toLowerCase()
                || currentClientName == "New Client Name?") {
                return false;
            }
        }
    }
    return true;
}

function CheckMessageProviders() {
    // Check to make sure at least 1 provider is checked and at least 1 provider in retry list
    var divProvidersContainer = document.getElementById("divProvidersContainer");

    var selectedProviderCount = 0;
    var selectedProviders = divProvidersContainer.getElementsByTagName("input");
    for (var i = 0; i < selectedProviders.length; i++) {
        if (selectedProviders[i].checked)
            selectedProviderCount++;
    }
    if (selectedProviderCount < 1)
        return false;
    return true;
}

function CheckMessageProviderList() {
    var selectProviderList = document.getElementById("selectProviderList");
    var providersInRetryListCount = selectProviderList.options.length;
    if (providersInRetryListCount < 1)
        return false;
    return true;
}

function validateClientForm() {

    var formIsValid = false;

    var currentAction = $('#btnClientActions').val();
    var currentMode = $('#hiddenT').val();

    //alert(currentAction + " - " + currentMode);

    if (currentAction == "Create New") {
        document.getElementById("hiddenT").value = "CreateNewClient";

        ShowProcessingMessage();
        document.getElementById("formMain").submit();
        return true;
    }

    if (currentAction != "Save") {
        if (currentMode != "CreateNewClient") {
            // Check to make sure at least 1 provider is checked
            if (CheckMessageProviders() == false) {
                alert("You must select at least 1 Message Provider! " + currentAction);
                return false;
            }

            // Check to make sure at least 1 provider in retry list
            if (CheckMessageProviderList() == false) {
                alert("You must add at least 1 Message Provider to the Client Retry List! " + currentAction);
                return false;
            }
        }
    }
    
    $('#panelFocusClients').val('clientTab1');

    $("#newClientPanelFocus").val("true");

    if ($('#btnClientActions').val() != "Create New") {
        var formFields = $("input");

        var currentField = null;
        for (i = 0; i < formFields.length - 1; i++)
        {
            currentField = formFields[i];

            if (currentField != undefined)
            {
                if ($(currentField).attr("isrequired") != null) {
                    
                    if ($(currentField).attr("isrequired") == "true") {
                        formIsValid = validateFormFields(currentField);

                        if (!formIsValid) {
                            //alert($(currentField).attr["lbl"] + " is invalid, please correct your input");
                            $(currentField).focus();
                            return false;
                        }
                    }
                }
            }
        }

        if (formIsValid) {
            var xmlDocumentTemplates = "";

            var hiddenTemplatesXml = $("#hiddenTemplatesXml")[0];

            $(hiddenTemplatesXml).val("<documenttemplates>");

            // Get the templates list and process into xml then add to hiddenTemplatesXml
            var templateItems = $('#dlMessageTemplates')[0];
            
            for (var i = 0; i < templateItems.options.length; i++)
            {
                var currentTemplate = templateItems.options[i];

                if ($(currentTemplate).attr("isroottype") != null) {

                    if ($(currentTemplate).attr("isroottype") == "false") {

                        // Reset for the current template item
                        xmlDocumentTemplates = "<documenttemplate>";

                        for (var j = 0; j < currentTemplate.attributes.length; j++)
                        {
                            if (currentTemplate.attributes[j].name != "style" && currentTemplate.attributes[j].name != "value" && currentTemplate.attributes[j].name != "isroottype")
                            {
                                if (currentTemplate.attributes[j].name != "id")
                                    xmlDocumentTemplates += "<" + currentTemplate.attributes[j].name + ">" + currentTemplate.attributes[j].value + "</" + currentTemplate.attributes[j].name + ">";
                            }
                        }
                        xmlDocumentTemplates += "</documenttemplate>";
                        
                        // Add to hiddenTemplatesXml
                        hiddenTemplatesXml.value += xmlDocumentTemplates;
                    }
                }
            }

            hiddenTemplatesXml.value += "</documenttemplates>";

            switch (currentAction) {
                case "Save":
                    //alert("Save");
                    document.getElementById("hiddenT").value = "SaveNewClient";

                    ShowProcessingMessage();
                    document.getElementById("formMain").submit();
                    break;

                case "Update":
                    //alert("Update");
                    document.getElementById("hiddenT").value = "UpdateExistingClient";

                    if (ensureMinimumProviderSelected()) {
                        ShowProcessingMessage();
                        document.getElementById("formMain").submit();
                    }
                    else {
                        alert("You must select at least 1 Message Delivery Provider");
                        return false;
                    }
                    break;
            }
        }
        //("Please correct your input");
        return false;
    }
    return true;
}

function ensureMinimumProviderSelected()
{
    var providersSelected = 0;

    // --------------- Email ---------------
        // GMail
        var emailGMailProviderSelected = document.getElementById("Gmail (Email)").checked;
        if (emailGMailProviderSelected)
            providersSelected++;

        // MAC
        var emailMACProviderSelected = document.getElementById("MobileAuthCorp (Email)").checked;
        if (emailMACProviderSelected)
            providersSelected++;

        // Yahoo
        var emailYahooProviderSelected = document.getElementById("Yahoo (Email)").checked;
        if (emailYahooProviderSelected)
            providersSelected++;
    // --------------- Email ---------------


    // --------------- Sms ---------------
        // MessageBroadcast
            var smsMBProviderSelected = document.getElementById("MessageBroadcast (Sms)").checked;
            if (smsMBProviderSelected)
                providersSelected++;

        // Twilio
            var smsTwilioProviderSelected = document.getElementById("Twilio (Sms)").checked;
            if (smsTwilioProviderSelected)
                providersSelected++;
    // --------------- Sms ---------------


    // --------------- Voice ---------------
        // MessageBroadcast
            var voiceMBProviderSelected = document.getElementById("MessageBroadcast (Voice)").checked;
            if (voiceMBProviderSelected)
                providersSelected++;

        // RingCentral
            var voiceRingCentralProviderSelected = document.getElementById("RingCentral (Voice)").checked;
            if (voiceRingCentralProviderSelected)
                providersSelected++;
    // --------------- Voice ---------------

        if (providersSelected > 0)
        return true;
    else
        return false;
}

function autoUpdateTemplateItemAttributes(updatedItem)
{
    hasUnsavedChanges = true;

    var templateList = document.getElementById("dlMessageTemplates");

    var selectedTemplateListItem = templateList.options[templateList.selectedIndex];
    var selectedTemplateListItemId = updatedItem.id;

    var fieldLabelId = "lbl_" + updatedItem.id;
    var fieldLabelValue = document.getElementById(fieldLabelId).innerHTML;

    var oldValue = updatedItem.attributes["originalvalue"].value;
    var newValue = updatedItem.value;

    //alert("oldValue - " + oldValue);

    selectedTemplateListItem.attributes[selectedTemplateListItemId].value = updatedItem.value;

    //alert("newValue - " + selectedTemplateListItem.attributes[selectedTemplateListItemId].value);
}

function setTemplateDisplay(templateList)
{
    var inititalMsgDisplayed = false;
    var selectedTemplate = templateList.options[templateList.selectedIndex];

    var selectedTemplateText = selectedTemplate.text;
    var selectedTemplateId = selectedTemplate.id;

    var templateData = "";
    var templateContent = "";

    for (var i = 0; i < selectedTemplate.attributes.length; i++)
    {
        var itemAttributeName = selectedTemplate.attributes[i].name;
        var itemAttributeValue = selectedTemplate.attributes[i].value;

        if (itemAttributeName.indexOf("message") > -1) {

            if (itemAttributeName != "messagedesc" && itemAttributeName != "messageclass") {

                var fieldLabel = itemAttributeName.replace("message", "");
                switch(fieldLabel)
                {
                    case "format":
                        fieldLabel = "Message Body";

                        if (itemAttributeValue.indexOf("~") > -1) {
                            // This an email template
                            var tmpEmailValues = itemAttributeValue.split('~');
                            templateContent = "<textarea id='" + itemAttributeName + "' value='" + itemAttributeValue + "' originalvalue='" + itemAttributeValue + "' class='MessageTemplateBody' onchange='javascript: autoUpdateTemplateItemAttributes(this);'>" + itemAttributeValue + "</textarea>";
                        }
                        else {
                            templateContent = "<textarea id='" + itemAttributeName + "' value='" + itemAttributeValue + "' originalvalue='" + itemAttributeValue + "' class='MessageTemplateBody' onchange='javascript: autoUpdateTemplateItemAttributes(this);'>" + itemAttributeValue + "</textarea>";
                        }
                        break;
                    case "fromaddress":
                        fieldLabel = "From Address";
                        templateContent = "<input id='" + itemAttributeName + "' type='text' value='" + itemAttributeValue + "' originalvalue='" + itemAttributeValue + "' onchange='javascript: autoUpdateTemplateItemAttributes(this);' />";
                        break;
                    case "fromname":
                        fieldLabel = "From Name";
                        templateContent = "<input id='" + itemAttributeName + "' type='text' value='" + itemAttributeValue + "' originalvalue='" + itemAttributeValue + "' onchange='javascript: autoUpdateTemplateItemAttributes(this);' />";
                        break;
                    default:
                        fieldLabel = fieldLabel;
                        templateContent = "<input id='" + itemAttributeName + "' type='text' value='" + itemAttributeValue + "' originalvalue='" + itemAttributeValue + "' onchange='javascript: autoUpdateTemplateItemAttributes(this);' />";
                        break;
                }

                if (itemAttributeValue != "") {
                    templateData += "<div class='row'>";
                    templateData += "   <div class='large-12 columns'>";
                    templateData += "       <label><span id='lbl_" + itemAttributeName + "'>" + fieldLabel + "</span>";
                    templateData += templateContent;
                    templateData += "       </label>";
                    templateData += "   </div>";
                    templateData += "</div>";
                }
            }
        }
        $('#divTemplateContent').html(templateData);
    }
}

function setHiddenRefreshEventTypeList(bRefresh) {
    $('#hiddenRefreshEventTypeList').val(bRefresh);
}

function toggleGroupDisplay() {
    $('#GroupDisplay').slideToggle('slow');
    var currentDisplay = $('#displayGroupSettings').html();
    $('html, body').animate({
        scrollTop: ($('#scroll2').offset().top -55)
    }, 750, 'easeOutExpo');
    if (currentDisplay == '[+] Show Group Settings') {
        $('#displayGroupSettings').html('[<span style="font-size: 1rem;">-</span>] Hide Group Settings');
    } else {
        $('#displayGroupSettings').html('[+] Show Group Settings');
    }
}

function toggleSearchDisplay() {
    $('#searchSettingsDisplay').slideToggle('slow');
    var currentDisplay = $('#displaySearchSettings').html();
    $('html, body').animate({
        scrollTop: ($('#scroll2').offset().top)
    }, 750, 'easeOutExpo');
    if (currentDisplay == '[+] Show Settings') {
        $('#displaySearchSettings').html('[<span style="font-size: 1rem;">-</span>] Hide Settings');
    } else {
        $('#displaySearchSettings').html('[+] Show Settings');
    }
}

function toggleClientDisplay() {
    $('#clientSettingsDisplay').slideToggle('slow');
    var currentDisplay = $('#displayClientSettings').html();
    $('html, body').animate({
        scrollTop: ($('#scroll2').offset().top)
    }, 750, 'easeOutExpo');
    if (currentDisplay == '[+] Show Settings') {
        $('#displayClientSettings').html('[<span style="font-size: 1rem;">-</span>] Hide Settings');
    } else {
        $('#displayClientSettings').html('[+] Show Settings');
    }
}

function refreshMessageIndicator() {
    $('#refreshIndicator').show();
    if ($("#eventHistoryTBody tr td").html() == 1) {
        $('#refreshIndicator').delay(550).fadeOut(1500);
    }
}

function startEventTimer() {

    eventTimer = $.timer(function() {
        getEventHistory();
    });

    var recordsCheckboxes = $(".recordsCheckbox");
    for (var i = 0; i < recordsCheckboxes.length; i++) {
        $(recordsCheckboxes[i]).prop("disabled", true);
    }

    eventTimer.once(intervalTime);

    //eventTimer.set({ time: intervalTime, autostart: true });

    //eventTimer.set(options);
    //eventTimer.play(reset);  // Boolean. Defaults to false.
    //eventTimer.pause();
    //eventTimer.stop();  // Pause and resets
    //eventTimer.toggle(reset);  // Boolean. Defaults to false.
    //eventTimer.once(time);  // Number. Defaults to 0.
    //eventTimer.isActive  // Returns true if timer is running
    //eventTimer.remaining // Remaining time when paused
}

function switchClient() {
    var btnPauseValue = $('#btnPause').val();
    if (btnPauseValue == 'Resume') {
        pauseRefresh(false);
    }

    var selectedClient = document.getElementById("dlClients");

    if (selectedClient.value != "000000000000000000000000")
        $('#btnEditClient').show();
    else
        $('#btnEditClient').hide();

    document.getElementById("refreshMessage").innerHTML = "Loading...";

        $('#hiddenRefreshEventTypeList').val("true");

        $('#btnDetails').prop('disabled', true);
        $('#save').prop('disabled', true);

        $('#btnNext').prop('disabled', true);
        $('#btnLast').prop('disabled', true);

    startEventTimer();
}

function pauseRefresh(bPause) {
    var pauseButton = $('#btnPause').val();
    var currentPage = $('#CurrentPageNumber').html();
    var totalPPages = $('#TotalPageNumbers').html();

    if (pauseButton == "Pause") {
        // Event refresh is running. so pause it
        eventTimer.stop();

        $('#btnPause').val('Resume'); // change button to resume

        paginationEnabled = true;

        $('#refreshMessage').html('<span style="color: #222;">Paused!</span>');

        $('#btnDetails').prop('disabled', false);
        $('#save').prop('disabled', false);

        if (currentPage == '1') {
            $('#btnFirst').prop('disabled', true);
            $('#btnPrevious').prop('disabled', true);
            $('#btnNext').prop('disabled', false);
            $('#btnLast').prop('disabled', false);
        } else if (currentPage != totalPPages) {
            $('#btnFirst').prop('disabled', false);
            $('#btnPrevious').prop('disabled', false);
            $('#btnNext').prop('disabled', false);
            $('#btnLast').prop('disabled', false);
        } else {
            $('#btnFirst').prop('disabled', false);
            $('#btnPrevious').prop('disabled', false);
            $('#btnNext').prop('disabled', true);
            $('#btnLast').prop('disabled', true);
        }
    }
    else {
        // Event refresh is paused, so resume it

        $('#btnPause').val('Pause');
        paginationEnabled = false;
        $('#hiddenSelectedEventIds').val("");
        $('#btnDetails').prop('disabled', true);
        $('#save').prop('disabled', true);

        $('#btnFirst').prop('disabled', true);
        $('#btnPrevious').prop('disabled', true);
        $('#btnNext').prop('disabled', true);
        $('#btnLast').prop('disabled', true);

        getEventHistory();

        $('#hiddenTimerID').val(intervalId);
    }

    if (bPause) {
        $('#btnPause').val('Resume');
        $('#refreshMessage').html('<span style="color: #222;">Paused!</span>');
    }
}

function navigatePages(direction) {

    paginationEnabled = true;

    var recordsDiplayed = $("#dlRecordsPerPage").val().replace(/[^\d.]/g, '');
    
    switch (direction) {
        case "first":
            currentPageNumber = 1;
            eventRowCount = 0;
            break;

        case "previous":
            currentPageNumber--;
            if (eventRowCount <= 0)
                eventRowCount = 0;
            else
                eventRowCount = eventRowCount - (recordsDiplayed * 2);
            break;

        case "next":
            currentPageNumber++;
            break;

        case "last":
            getString = $('#TotalPageNumbers').html();
            clearString = getString.replace(/[^\d.]/g, '');
            currentPageNumber = parseInt(clearString);
            eventRowCount = lastRecordNumber;
            break;
    }

    if (currentPageNumber < 1)
        currentPageNumber = 1;

    $('#CurrentPageNumber').html(numberWithCommas(currentPageNumber));

    if (currentPageNumber > 1) {
        $("#btnFirst").prop("disabled", false);
        $("#btnPrevious").prop("disabled", false);
        $('#btnNext').prop('disabled', false);
        $('#btnLast').prop('disabled', false);
    }

    if (currentPageNumber == parseInt(clearString)) {
        $("#btnFirst").prop("disabled", false);
        $("#btnPrevious").prop("disabled", false);
        $("#btnNext").prop("disabled", true);
        $("#btnLast").prop("disabled", true);
    }

    if (currentPageNumber < parseInt(clearString)) {
        $("#btnFirst").prop("disabled", false);
        $("#btnPrevious").prop("disabled", false);
        $("#btnNext").prop("disabled", false);
        $("#btnLast").prop("disabled", false);
    }

    if (currentPageNumber == 1) {
        $("#btnFirst").prop("disabled", true);
        $("#btnPrevious").prop("disabled", true);
        $("#btnNext").prop("disabled", false);
        $("#btnLast").prop("disabled", false);
    }

    lastRecordNumber = (currentPageNumber * recordsPerPage);

    getEventHistory();
}

function changeEventTypes() {
    var btnPauseValue = $('#btnPause').val();
    if (btnPauseValue == 'Resume') {
    pauseRefresh(false);
    }

    var dlEventTypes = document.getElementById("dlEventTypes");
    document.getElementById("refreshMessage").innerHTML = "Loading...";

    $('#btnDetails').removeClass('resumeButton');
    $('#btnDetails').prop('disabled', true);
    $('#save').prop('disabled', true);

    startEventTimer();
}

function getEventHistory() {

    var selectedClient = "";
    var clientId = "";
    var eventTypeList = "";
    var eventTypeSelected = "";
    var sortFieldList = "";
    var sortField = "";
    var startDate = "";
    var endDate = "";

    if (document.getElementById("dlClients") != null) {
        selectedClient = document.getElementById("dlClients");
        clientId = selectedClient.options[selectedClient.selectedIndex].value;
    }

    if (clientId == "")
        clientId = dk_DefaultEmptyObjectId;

    if (document.getElementById("dlEventTypes") != null) {
        eventTypeList = document.getElementById("dlEventTypes");
        if (eventTypeList.selectedIndex != "-1")
            eventTypeSelected = eventTypeList.options[eventTypeList.selectedIndex].value;
    }

    if (eventTypeSelected == "All Events")
        eventTypeSelected = "";

    if (document.getElementById("dlSortField") != null) {
        sortFieldList = document.getElementById("dlSortField");
        sortField = sortFieldList.options[sortFieldList.selectedIndex].value;
    }

    if (sortField == "Sort By")
        sortField = "";

    var sortDirectionList = "";
    var sortDirection = "Descending";

    if (document.getElementById("dlSortOrder") != null) {
        sortDirectionList = document.getElementById("dlSortOrder");
        sortDirection = sortDirectionList.options[sortDirectionList.selectedIndex].value;
    }

    var dlRecordsPerPage = document.getElementById("dlRecordsPerPage");
    if (dlRecordsPerPage != null)
        recordsPerPage = dlRecordsPerPage.options[dlRecordsPerPage.selectedIndex].value;

    if (document.getElementById("popupDatepickerStartDate") != null)
        startDate = document.getElementById("popupDatepickerStartDate").value;
    else
        startDate = "";

    if (document.getElementById("popupDatepickerEndDate") != null)
        endDate = document.getElementById("popupDatepickerEndDate").value;
    else
        endDate = "";

    webMethod = "";
    webMethod += "/MACServices/Event/EventHistory1.asmx/WsEventHistory1";

    parameters = "Data=Request=GetHistory";
    parameters += "|clientId=" + clientId;
    parameters += "|ObjectType=" + eventTypeSelected;
    parameters += "|StartRecordNumber=" + lastRecordNumber; //
    parameters += "|NumberOfRecords=" + recordsPerPage;
    parameters += "|SortField=" + sortField;
    parameters += "|SortDirection=" + sortDirection;
    parameters += "|StartDate=" + startDate;
    parameters += "|EndDate=" + endDate;
    //alert("2.." + $('#hiddenRefreshEventTypeList').val());
    parameters += "|RefreshEventTypeListBox=" + $('#hiddenRefreshEventTypeList').val();

    //alert(webMethod + parameters);

    if (eventHistoryTimerEnabled == true) {

        // If New Client Name, don't call history. Instead, display empty message
        var clientName = $('#clientTab1 span').html();

        if (clientName != "New Client Name") {

            if (document.getElementById("refreshMessage") != null) {
                refreshMessageIndicator();
                $('#EventHistoryDisabledMessage').hide();
                $('#eventHistory').show();
                $('#displaySearchSettings').show();
                $('#eventButtonsContainer').show();
                $('#paginationButtonsContainer').show();
            }

            $.post(webMethod, parameters, EventHistoryResult);
            $('#EventHistoryDisabledMessage').hide();
            $('#eventHistory').show();
            $('#displaySearchSettings').show();
            $('#eventButtonsContainer').show();
            $('#paginationButtonsContainer').show();
        }
    }
}

function EventHistoryResult(eventHistoryResponse) {

    var recordsThisPage = 0;

    var pauseButton = document.getElementById("btnPause");

    if ($('#hiddenRefreshEventTypeList').val() == "true") {

        var dlEventTypes = $('#dlEventTypes');

        dlEventTypes.empty();
        dlEventTypes.append("<option value='All Events'>All Events</option>");

        $(eventHistoryResponse).find('eventtypes').each(function () {
            $(this).children().each(function () {
                var listItem = "<option id='" + $(this).text() + "' value='" + $(this).text() + "'>" + $(this).text() + "</option>";
                dlEventTypes.append(listItem);
            });
        });

        var listItem = "<option value='ClientEvents'>------------------------Client and Group Events------------------------</option>";
        dlEventTypes.append(listItem);

        listItem = "<option value='AdsSent'>Ads Sent</option>";
        dlEventTypes.append(listItem);

        listItem = "<option value='EndUsers'>End Users</option>";
        dlEventTypes.append(listItem);

        listItem = "<option value='OtpSent'>Otp Sent</option>";
        dlEventTypes.append(listItem);

        listItem = "<option value='OtpValid'>Otp Valid</option>";
        dlEventTypes.append(listItem);

        listItem = "<option value='OtpInvalid'>Otp Invalid</option>";
        dlEventTypes.append(listItem);

        $('#hiddenRefreshEventTypeList').val("false");

        // Update the eventype list display
        $("#dlEventTypes").trigger("chosen:updated");
    }

    var eventTable = "";

    var top = "0";
    var left = "0";

    //alert(eventHistoryResponse.toString());

    var totalRecords = $(eventHistoryResponse).find('events').attr('totalrecords');

    //alert("totalRecords - " + totalRecords);

    var totalPages = Math.round(totalRecords / recordsPerPage) - 1;
    if (totalPages.toString() == "NaN") {
        totalPages = 0;
        currentPageNumber = 0;

        if (currentPageNumber == 0)
            currentPageNumber = 1;

        if (currentDocUrl.indexOf("/Admin/Reporting/Default") > -1) {
            top = "0";
            left = "0";
        }

        if (document.getElementById("refreshMessage") != null)
            document.getElementById("refreshMessage").innerHTML = "<span>No records found!</span>";
    }

    if (totalPages < 1)
        totalPages = 1;

    if (document.getElementById("TotalPageNumbers") != null)
        document.getElementById("TotalPageNumbers").innerHTML = numberWithCommas(totalPages);

    if (document.getElementById("CurrentPageNumber") != null)
        document.getElementById("CurrentPageNumber").innerHTML = numberWithCommas(currentPageNumber);

    var rowCount = 2;

    $(eventHistoryResponse).find('event').each(function () {
        var eventId = $(this).attr('id');

        recordsThisPage++;

        eventTable += "<tr title='EventId: " + eventId + "'>";

        eventRowCount++;

        // Row count column
        eventTable += "<td style='width: 30px;vertical-align: top;'>";
        eventTable += eventRowCount + " ";
        eventTable += "</td>";

        $(this).children().each(function () {

            var val = $(this).text();

            var eventDetails = val.replace("<br />", "&nbsp;");

            var truncateDescriptionLength = 200;
            if (eventDetails.length >= truncateDescriptionLength)
                eventDetails = eventDetails.substring(0, truncateDescriptionLength - 3) + "...";

            if (this.tagName != "id") {
                switch (this.tagName) {
                    case "date":

                        eventDetails = convertUTCDateTimeToLocal(val);

                        eventTable += "<td style='width: 190px; vertical-align: top;'>";
                        eventTable += "<input id='chk_" + eventId + "' type='checkbox' class='recordsCheckbox' style='position: relative;top: .125rem !important;margin-right:0.25rem !important;' value='" + eventId + "' onclick='javascript: setEventDetailIds(this);' /> ";
                        eventTable += eventDetails.trim();
                        eventTable += "<br /><a href='javascript: showSingleEventDetails(&quot;" + eventId + "&quot;);'><span style='font-size: 12px;' id='link_showSingleEventDetails'>" + eventId + "</span></a>";
                        eventTable += "</td>";
                        break;

                    case "details":
                        if (val.indexOf("Exception") > -1)
                            eventDetails = "<span style='color: #f00;padding-top: -10px !important;'>" + eventDetails.trim() + "</span>";

                        if (val.indexOf("exception") > -1)
                            eventDetails = "<span style='color: #f00;padding-top: -10px !important;'>" + eventDetails.trim() + "</span>";

                        if (val.indexOf("Invalid") > -1)
                            eventDetails = "<span style='color: #f00;padding-top: -10px !important;'>" + eventDetails.trim() + "</span>";

                        if (val.indexOf("invalid") > -1)
                            eventDetails = "<span style='color: #f00;padding-top: -10px !important;'>" + eventDetails.trim() + "</span>";

                        if (val.indexOf("Otp (Invalidated)") > -1)
                            eventDetails = "<span style='color: #222;padding-top: -10px !important;'>" + eventDetails.trim() + "</span>";

                        if (val.indexOf("Otp (Validated)") > -1)
                            eventDetails = "<span style='color: #0f0;padding-top: -10px !important;'>" + eventDetails.trim() + "</span>";

                        eventTable += "<td style='border:width: 100%;min-height: 55px;line-height: 1.125rem; vertical-align: top; font-size: 12px;'>";
                        eventTable += eventDetails.trim();
                        eventTable += "</td>";
                        break;

                    default:
                        break;
                }
            }
        });

        eventTable += "</tr>";
    });


    var eventHistory = document.getElementById("eventHistoryTBody");
    if (eventHistory != null) {
        eventHistory.innerHTML = eventTable;
    }

    if (totalRecords == "undefined")
        totalRecords = 0;

    if (document.getElementById("refreshMessage") != null)
        document.getElementById("refreshMessage").innerHTML = "<span style='color: #222;'>Events: " + numberWithCommas(totalRecords) + "</span>&nbsp;";

    if (paginationEnabled == false) {
        eventRowCount = 0;
        
        // This is for infinite scroll
        //lastRecordNumber += recordsThisPage;
        //eventRowCount = 0;

        startEventTimer();
    }
};

/*get group info*/
function getGroupInfo() {

    var groupId = "?";

    $.ajax
    ({
        type: "POST",
        url: "/MACServices/AdminServices/GroupInfo.asmx/WsGetGroupInfo?groupId=" + groupId,
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {

            var serviceResponse = $.parseJSON(response.d);

            // Core
            document.getElementById("legendCoreStats").innerHTML = "Core";

            $('#spanGroupCount').html(numberWithCommas(serviceResponse.Groups));
            $('#spanClientCount').html(numberWithCommas(serviceResponse.Clients));
            $('#spanEventCount').html(numberWithCommas(serviceResponse.Events));
            $('#spanExceptionCount').html(numberWithCommas(serviceResponse.Exceptions));

            // Users
            document.getElementById("legendUserStats").innerHTML = "User";
            $('#spanSystemAdminCount').html(numberWithCommas(serviceResponse.System_Admins));
            $('#spanGroupAdminCount').html(numberWithCommas(serviceResponse.Group_Admins));
            $('#spanClientAdminCount').html(numberWithCommas(serviceResponse.Client_Admins));
            $('#spanEndUserCount').html(numberWithCommas(serviceResponse.End_Users));

            // OTPs
            document.getElementById("legendOtpStats").innerHTML = "OTP";
            $('#spanOtpRequestsCount').html(numberWithCommas(serviceResponse.Requests));
            $('#spanOtpValidCount').html(numberWithCommas(serviceResponse.Valid));
            $('#spanOtpInvalidCount').html(numberWithCommas(serviceResponse.Invalid));
            $('#spanOtpExpiredCount').html(numberWithCommas(serviceResponse.Expired));

            //startSystemEventTimer();
        },

        failure: function (response) {
            alert(response.d);
        }
    });
}

function assignUserDocAccess() {

    popupWindowWidth = 550;
    popupWindowHeight = 200;

    var topicId = document.getElementById("spanTopicId").innerHTML;

    //alert("topicId - " + topicId);

    var targetUrl = "/Admin/Help/UsersPopup?topicId=" + topicId;

    showJQueryDialog(targetUrl, popupWindowWidth, popupWindowHeight);
}

function changeEnvironmentSettings()
{
    var userId = document.getElementById("hiddenE").value;
    var targetUrl = "/Admin/System/ChangeEnvironmentSettings?userId=" + userId;

    showJQueryDialog(targetUrl, popupWindowWidth, popupWindowHeight);
}

function createEventStats()
{
    var userId = document.getElementById("hiddenE").value;
    
    //var targetUrl = "/Admin/System/CreateEventStats?showupdates=true&userId=" + userId;
    var targetUrl = "/Admin/System/CreateEventStats?su=true&userId=" + userId;

    //alert(targetUrl);

    showJQueryDialog(targetUrl, popupWindowWidth, popupWindowHeight);
}

function createMessageTemplate()
{
    var userId = document.getElementById("hiddenE").value;

    var targetUrl = "/Admin/System/CreateMessageTemplate?userId=" + userId;

    //alert(targetUrl);

    showJQueryDialog(targetUrl, popupWindowWidth, popupWindowHeight);
}

function resetSystemData() {

    var userId = document.getElementById("hiddenE").value;
    var targetUrl = "/Admin/System/DBDropCollections?userId=" + userId;

    showJQueryDialog(targetUrl, popupWindowWidth, popupWindowHeight);
}

function deleteTestData()
{
    var userId = document.getElementById("hiddenE").value;
    var targetUrl = "/Admin/System/DeleteTestData?userId=" + userId;

    showJQueryDialog(targetUrl, popupWindowWidth, popupWindowHeight);
}

function resetIPs()
{
    var userId = document.getElementById("hiddenE").value;
    var targetUrl = "/Admin/System/ResetIPs?userId=" + userId;

    showJQueryDialog(targetUrl, popupWindowWidth, popupWindowHeight);
}

function resetAdProviders() {
    var userId = document.getElementById("hiddenE").value;
    var targetUrl = "/Admin/System/ResetAdProviders?userId=" + userId;

    showJQueryDialog(targetUrl, popupWindowWidth, popupWindowHeight);
}

function resetMessageProviders() {
    var userId = document.getElementById("hiddenE").value;
    var targetUrl = "/Admin/System/ResetMessageProviders?userId=" + userId;

    showJQueryDialog(targetUrl, popupWindowWidth, popupWindowHeight);
}

function updateStatsAndLists()
{
    popupWindowWidth = 550;
    popupWindowHeight = 200;

    var targetUrl = "/Admin/System/ScheduledProcessing?billing=false&lists=false&stats=true";

    showJQueryDialog(targetUrl, popupWindowWidth, popupWindowHeight);
}

function setCollectionIdsToDrop(selectedItem) {

    var hiddenItemsToDropIds = $("#hiddenItemsToDropIds")[0];

    switch (selectedItem.checked)
    {
        case true:
            hiddenItemsToDropIds.value += selectedItem.id + "|";
            break;

        case false:
            hiddenItemsToDropIds.value = hiddenItemsToDropIds.value.replace(selectedItem.id + "|","");
            break;
    }
}

function navigateCreateNewAdmin(adminType)
{
    window.opener.ShowProcessingMessage();
    var adminUrl = "/Admin/Membership/AdminUsers?AdminType=" + adminType;
    window.opener.location = adminUrl;
    window.close();
}

function submitOtpRequest(requestType) {

    var mainForm = document.getElementById("formMain");
    var pageAction = document.getElementById("hiddenAA");
    var userLoginName = document.getElementById("txtUsername_" + requestType);
    var hiddenAB = document.getElementById("hiddenAB");

    if (userLoginName != null)
        hiddenAB.value = userLoginName.value;

    pageAction.value = "requestotp";

    mainForm.action += "?DisplyMode=" + requestType;

    if (userLoginName != null) {
        if (userLoginName.value == "") {
            document.getElementById("AdminLoginResult_Desktop").innerHTML = "<span style='color: #ff0000;'>Enter a Login Name!</span>";
            userLoginName.focus();
            return false;
        }
        else {
            ShowProcessingMessage();
            mainForm.submit();
            return true;
        }
    }
}

function submitOtpLogon(requestType) {

    var userPassword = document.getElementById("txtPassword_" + requestType);

    if (userPassword != null) {
        if (userPassword.value == "") {
            document.getElementById("AdminLoginResult_Desktop").innerHTML = "<span style='color: #ff0000;'>Please enter your OTP/Password!</span>";
            userPassword.focus();
            return false;
        }
        else {
            var hiddenAC= document.getElementById("hiddenAC");

            hiddenAC.value = userPassword.value;

            document.getElementById("hiddenAA").value = "login";

            ShowProcessingMessage();
            document.getElementById("formMain").submit();
            return true;
        }
    }
}

function submitLogonRequest() {

    var submitButton = document.getElementById("AdminLoginButton").value;
    var pageAction = document.getElementById("hiddenAA");

    switch (submitButton) {
        case "Login":
            pageAction.value = "login";
            break;

        case "Request OTP":
            pageAction.value = "requestotp";
            break;

        default:
            pageAction.value = "login";
            break;
    }

    var userName = document.getElementById("txtUsername");
    if (userName != null) {
        if (userName.value == "") {
            alert("Please enter a Login Name!");
            document.getElementById("txtUsername").focus();
        }
        else {
            ShowProcessingMessage();
            document.getElementById("formMain").submit();
        }
    }
    else {
        var userPassword = document.getElementById("txtPassword");
        if (userPassword != null) {
            if (userPassword.value == "") {
                alert("Please enter the " + document.getElementById("spanLoginResultDetails").innerHTML);
                document.getElementById("txtPassword").focus();
            }
            else {
                ShowProcessingMessage();
                document.getElementById("formMain").submit();
            }
        }
    }
}

function redirectLogin()
{
    var mainForm = document.getElementById("formMain");

    var hiddenAE = document.getElementById("hiddenAE");

    //alert(mainForm.action);

    //mainForm.action = "/Admin/Security/Login";
    mainForm.submit();
}

function useExistingCredentials(requestType) {

    var requestButton = document.getElementById("btnAdminOtpRequest_" + requestType);
    var userName = document.getElementById("txtUsername_" + requestType);
    var billId = document.getElementById("hiddenV").value;

    //alert("billId - " + billId);

    if (userName.value == "")
        alert("Please enter a Login Name to continue!");
    else {

        //var myParams = document.getElementById("hiddenAE");
        //myParams.value = "Action=UseExistingCredentials&UName=" + userName.value + "&DisplayMode=" + DisplayMode + "&bid=" + billId;

        //alert(myParams.value);

        var mainForm = document.getElementById("formMain");

        mainForm.action = "/Admin/Security/Login?Action=UseExistingCredentials&UName=" + userName.value + "&DisplayMode=" + DisplayMode + "&bid=" + billId;
        mainForm.submit();
    }
}

function updateUserLoginNameHiddenField_Desktop(input) {
    document.getElementById("hiddenAB").value = input.value;
}

function updateUserPasswordHiddenField_Desktop(input) {
    document.getElementById("hiddenAC").value = input.value;
}

function updateUserLoginNameHiddenField_Mobile(input) {
    document.getElementById("hiddenAB").value = input.value;
}

function updateUserPasswordHiddenField_Mobile(input) {
    document.getElementById("hiddenAC").value = input.value;
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

function setMessageTemplates(selectedMessageClass) {
    $("#dlEmailTemplates").hide();
    $("#dlHtmlTemplates").hide();
    $("#dlSmsTemplates").hide();
    $("#dlVoiceTemplates").hide();

    $("#dlEmailTemplates_chosen").hide();
    $("#dlHtmlTemplates_chosen").hide();
    $("#dlSmsTemplates_chosen").hide();
    $("#dlVoiceTemplates_chosen").hide();

    $("#tempSelect_chosen").hide();

    $("#divButtonActions").hide();

    $("#divTemplateContent").html("");
    $("#divCurrentXPath").html("");

    $("#dlEmailTemplates").get(0).selectedIndex = 0;
    $("#dlHtmlTemplates").get(0).selectedIndex = 0;
    $("#dlSmsTemplates").get(0).selectedIndex = 0;
    $("#dlVoiceTemplates").get(0).selectedIndex = 0;

    switch (selectedMessageClass.value.toLowerCase()) {
        case "please choose":
            $("#tempSelect_chosen").show();
            break;
        case "email":
            $("#dlEmailTemplates_chosen").show();
            break;
        case "html":
            $("#dlHtmlTemplates_chosen").show();
            break;
        case "sms":
            $("#dlSmsTemplates_chosen").show();
            break;
        case "voice":
            $("#dlVoiceTemplates_chosen").show();
            break;
    }
}

function displaySelectedTemplateInfo(selectedMessageTypeTemplate)
{
    if (selectedMessageTypeTemplate.indexOf("Templates") > -1) {
        messageComponents = "";
        $("#divButtonActions").css('visibility', 'hidden');
    }
    else {
        $("#divButtonActions").css('visibility', 'visible');

        hilightCommittButton(false);

        var messageSubject = "";
        var messageBody = "";
        var messageFromAddress = "";
        var messageFromName = "";

        var regexPattern = /[|]+/g;
        var replacementCharacter = "\n";

        $("#divButtonActions").show();

        var messageComponents = "";
        var xmlData = $("#hiddenXmlTemplates").val();

        var xPath = "documenttemplates[messageclass=\"" + selectedMessageTypeTemplate + "\"]";
        $("#divCurrentXPath").html(xPath);

        var documentTemplate = $(xmlData).find(xPath);
        var templateData = documentTemplate[0];

        if (templateData.innerHTML != "") {
            var nodeName = "Message";

            // Replace tokens where possible using client info (subject, body, from address and from name
            var clientName = $("#txtClientName")[0];
            if (clientName != null)
                clientName = $("#txtClientName").val();

            var fromDomain = $("#txtAuthorizedDomain")[0];
            if (fromDomain != null)
                fromDomain = $("#txtAuthorizedDomain").val();

            var tmpData = $(templateData).html();
            messageFormat = tmpData.replace("[ClientName]", clientName);

            //<messageformat><![CDATA[Login~OTP:[OTP]||[AD]]]></messageformat><messagefromaddress>admin@[ClientDomain]</messagefromaddress><messagefromname>[ClientName]</messagefromname>
            var tmpMessageFormat = tmpData.split("</messageformat>");
            var messageFormat = tmpMessageFormat[0].replace("<messageformat><!--[CDATA[", "");
            messageFormat = messageFormat.replace("]]-->", "");

            if (messageFormat.indexOf('~') > -1) {

                if (tmpMessageFormat[1] != null) {
                    var tmpMessageFromAddress = tmpMessageFormat[1].split("</messagefromaddress>");
                    messageFromAddress = tmpMessageFromAddress[0].replace("<messagefromaddress>", "");
                    messageFromAddress = messageFromAddress.replace("[ClientDomain]", fromDomain);
                }

                if (templateData.innerHTML.contains("<messagefromname>")) {
                    var tmpMessageFromName = templateData.innerHTML.split("<messagefromname>");
                    messageFromName = tmpMessageFromName[1].replace("</messagefromname>", "");
                    messageFromName = messageFromName.replace("[ClientName]", clientName);
                }

                // This is an email template. We need to split on the tilde. Index 0 is the message subject line
                if (messageFormat.contains("~")) {
                    var tmpVal = messageFormat.split('~');

                    messageSubject = tmpVal[0].replace("[ClientName]", clientName);
                    messageBody = tmpVal[1];
                }

                // Replace dk_ItemSep with carriage return and new line feed
                messageBody = messageBody.replace(regexPattern, replacementCharacter);

                messageComponents += "<div class='row'>";
                messageComponents += "  <div class='large-2 medium-2 small-12 columns talign'>";
                messageComponents += "      <span class='font-875rem'>Subject</span>";
                messageComponents += "  </div>";
                messageComponents += "  <div class='large-10 medium-10 small-12 columns talign'>";
                messageComponents += "      <input id='textArea_MessageSubject' type='text' onchange='javascript: hilightCommittButton(true);' value='" + messageSubject + "' />";
                messageComponents += "  </div>";
                messageComponents += "</div>";
                messageComponents += "<div class='row'>";
                messageComponents += "  <div class='large-2 medium-2 small-12 columns talign'>";
                messageComponents += "      <span class='font-875rem'>" + nodeName + "</span>";
                messageComponents += "  </div>";
                messageComponents += "  <div class='large-10 medium-10 small-12 columns talign'>";
                messageComponents += "      <textarea id='textArea_MessageBody' class='MessageTemplateBody'>" + messageBody + "</textarea>";
                messageComponents += "  </div>";
                messageComponents += "</div>";

                if (messageFromAddress != null && messageFromAddress != "") {
                    messageComponents += "<div class='row'>";
                    messageComponents += "  <div class='large-2 medium-2 small-12 columns talign'>";
                    messageComponents += "      <span class='font-875rem no-wrap'>From Address</span>";
                    messageComponents += "  </div>";
                    messageComponents += "  <div class='large-10 medium-10 small-12 columns talign'>";
                    messageComponents += "      <input id='textArea_MessageFromAddress' type='text' onchange='javascript: hilightCommittButton(true);' value='" + messageFromAddress + "' />";
                    messageComponents += "  </div>";
                    messageComponents += "</div>";
                }

                if (messageFromName != null && messageFromName != "") {
                    messageComponents += "<div class='row'>";
                    messageComponents += "  <div class='large-2 medium-2 small-12 columns talign'>";
                    messageComponents += "      <span class='font-875rem'>From Name</span>";
                    messageComponents += "  </div>";
                    messageComponents += "  <div class='large-10 medium-10 small-12 columns talign'>";
                    messageComponents += "      <input id='textArea_MessageFromName' type='text' onchange='javascript: hilightCommittButton(true);' value='" + messageFromName + "' />";
                    messageComponents += "  </div>";
                    messageComponents += "</div>";
                }
            }
            else {

                // Replace dk_ItemSep with carriage return and new line feed
                messageBody = messageFormat.replace("[ClientName]", clientName);
                messageBody = messageBody.replace(regexPattern, replacementCharacter);

                messageComponents += "<div class='row'>";
                messageComponents += "  <div class='large-2 medium-2 small-12 columns talign'>";
                messageComponents += "      <span class='font-875rem'>" + nodeName + "</span>";
                messageComponents += "  </div>";
                messageComponents += "  <div class='large-10 medium-10 small-12 columns talign'>";
                messageComponents += "      <textarea id='textArea_MessageBody' class='MessageTemplateBody' style='height: 150px;'>" + messageBody + "</textarea>";
                messageComponents += "  </div>";
                messageComponents += "</div>";
            }
        }
    }
    $("#divTemplateContent").html(messageComponents);
}

function updateClientXml() {

    var xmlData = $("#hiddenXmlTemplates").val();    
    var xPath = $("#divCurrentXPath").html();
    
    var originalStringToReplace = null;
    var newReplacementValues = null;

    var updateSubject = "";
    
    if ($("#textArea_MessageSubject")[0] != null)
        updateSubject = $("#textArea_MessageSubject").val();
    

    var updateBody = "";

    if ($("#textArea_MessageBody")[0] != null)
        updateBody = $("#textArea_MessageBody").val();
    

    // Concatenate these values if we're dealing with an email template
    if (updateSubject != null && updateSubject != "")
        updateBody = updateSubject + "~" + updateBody;

    //<messageformat><![CDATA[Login~OTP:[OTP]||[AD]]]></messageformat><messagefromaddress>admin@[ClientDomain]</messagefromaddress><messagefromname>[ClientName]</messagefromname>
    var replacementString = "";
    replacementString += "<messageformat>";
    replacementString += "<![CDATA[";

    replacementString += sanitizeXml(updateBody);

    // This only dk_KVSep since we are conditio ning the string
    replacementString = replaceCarriageReturnsAndLinebreaks(replacementString);

    replacementString += "]]>";
    replacementString += "</messageformat>";

    var updateFromAddress = "";
    if ($("#textArea_MessageFromAddress") != null) {
        updateFromAddress = $("#textArea_MessageFromAddress").val();
        replacementString += "<messagefromaddress>";
        replacementString += updateFromAddress;
        replacementString += "</messagefromaddress>";
    }

    var updateFromName = "";
    if ($("#textArea_MessageFromName") != null) {
        updateFromName = $("#textArea_MessageFromName").val();
        replacementString += "<messagefromname>";
        replacementString += updateFromName;
        replacementString += "</messagefromname>";
    }

    // Locate the string we want to replace and clean it
    var documentTemplate = $(xmlData).find(xPath);
    if (documentTemplate != null)
        originalStringToReplace = documentTemplate[0].innerHTML.replace("<!--[", "<![").replace("]]-->", "]]>");

    var updateXmlData = xmlData.toString().replace(originalStringToReplace, replacementString);

    $("#hiddenXmlTemplates").value = updateXmlData;

    hilightUpdateClientButton(true);
    
    $("#hiddenXmlTemplatesHaveBeenUpdated").value = "true";

    window.scrollTo(0, 0);
}

function replaceCarriageReturnsAndLinebreaks(stringToConvert)
{
    var regexPattern = /\r?\n|\r/g;
    var replacementCharacter = "|";
    stringToConvert = stringToConvert.replace(regexPattern, replacementCharacter);

    return stringToConvert;
}

function sanitizeXml(stringToClean)
{
    stringToClean = stringToClean.replace("#", "&#35;");
    stringToClean = stringToClean.replace("'", "&#39;");

    return stringToClean;
}

function decodeXmlValue(stringToDecode)
{
    stringToDecode = stringToDecode.replace("&#39;", "'");
    stringToDecode = stringToDecode.replace("&#35;", "#");

    return stringToDecode;
}

function hilightUpdateClientButton(bShowButtonFocus) {
    if (bShowButtonFocus) {
        $("#btnClientActions").css('background-color', '#e78800');
        $("#btnClientActions").css('color', '#fff');
        $('#btnShowAPIClientWindow').prop("disabled", true);
    }
    else {
        $("#btnClientActions").css('background-color', '#e78800');
        $("#btnClientActions").css('color', '#fff');
        $('#btnShowAPIClientWindow').prop("disabled", true);
    }
}

String.prototype.toProperCase = function () {
    return this.replace(/\w\S*/g, function (txt) { return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase(); });
};

function setSelectedVerificationProviders(selectedItem) {

    if (selectedItem.checked)
    {
        document.getElementById("hiddenSelectedProviderIds").value += selectedItem.id + dk_ItemSep;
    }
    else
    {
        var arrProviderIds;
        if (document.getElementById("hiddenSelectedProviderIds").value.indexOf(dk_ItemSep) > -1) {
            arrProviderIds = document.getElementById("hiddenSelectedProviderIds").value.split(dk_ItemSep);
            for(var i = 0; i < arrProviderIds.length; i++)
            {
                if (arrProviderIds[i] == selectedItem.id)
                    document.getElementById("hiddenSelectedProviderIds").value = document.getElementById("hiddenSelectedProviderIds").value.replace(arrProviderIds[i] + dk_ItemSep, "");
            }
        }
    }
}

function showEventsLocalTime() {
    alert("showEventsLocalTime - ");
}

function navigateToGroup(selectedItem) {

    ShowProcessingMessage();

    var selectedItemGroupId = selectedItem.id.replace("spanGroupName_", "");

    var urlParams = "gid=" + selectedItemGroupId;
    urlParams += "&uid=" + document.getElementById("hiddenE").value;
    urlParams += "&fname=" + document.getElementById("hiddenH").value;
    urlParams += "&lname=" + document.getElementById("hiddenI").value;

    var parentContainer = document.getElementById(selectedItemGroupId);

    urlParams += "&indexnumber=" + parentContainer.attributes["indexnumber"].value;

    document.getElementById("formMain").action = "/Admin/Groups?" + urlParams;
    document.getElementById("formMain").submit();
}

function navigateToClientFromPopup(selectedItemId) {

    ShowProcessingMessage();

    var urlParams = "/Admin/Clients/default";
    urlParams += "?cid=" + selectedItemId;
    urlParams += "&uid=" + document.getElementById("hiddenE").value;
    urlParams += "&fname=" + document.getElementById("hiddenH").value;
    urlParams += "&lname=" + document.getElementById("hiddenI").value;
    urlParams += "&panelFocus=clientTab1";

    //document.getElementById("panelFocusClients").value == "clientTab1";

    window.location = urlParams;

    //$('#panelFocusClients').val('clientTab1');
}

function navigateToClient(selectedItemId) {

    ShowProcessingMessage();

    var urlParams = "cid=" + selectedItemId.toString().replace("spanGroupName_", "");
    urlParams += "&uid=" + document.getElementById("hiddenE").value;
    urlParams += "&fname=" + document.getElementById("hiddenH").value;
    urlParams += "&lname=" + document.getElementById("hiddenI").value;

    document.getElementById("formMain").action = "/Admin/Clients?" + urlParams;
    document.getElementById("formMain").submit();
}

function navigateToEvent(selectedItemId) {

    ShowProcessingMessage();

    var urlParams = "eid=" + selectedItemId.toString().replace("spanGroupName_", "");
    urlParams += "&uid=" + document.getElementById("hiddenE").value;
    urlParams += "&fname=" + document.getElementById("hiddenH").value;
    urlParams += "&lname=" + document.getElementById("hiddenI").value;

    document.getElementById("formMain").action = "/Admin/Reports?" + urlParams;
    document.getElementById("formMain").submit();
}

function HideProcessingMessage() {

    $('#divPleaseWaitProcessing').hide();
}

function ShowProcessingMessage() {
    // Scroll window to top. If not, then if the browser is towards the bottom of the window, the dialog is out of view at the top
    window.scrollTo(0, 0);

    var screenHeight = $(window).innerHeight();
    var screenWidth = $(window).innerWidth();

    var loaderMargin = (screenHeight / 2) - 18;

    $('#divDialogContainer').css('margin-top', loaderMargin);

    $('#divPleaseWaitProcessing').css("height", screenHeight);
    $('#divPleaseWaitProcessing').css("width", screenWidth);
    $('#divPleaseWaitProcessing').show();
    $('#divDialogContainer').show();
}

function setChartType()
{
    alert("setChartType");
}

function updateAdminNotificationLink(providerName)
{
    var linkElement = window.opener.document.getElementById("spanManageAdminNotificationProvider");
    linkElement.innerHTML = providerName;
}

function navigateToClientApp(clientName)
{
    window.location = clientName;
}

function cancelClientUpdate()
{
    ShowProcessingMessage();

    document.getElementById("hiddenT").value = "CancelUpdateClient";
    document.getElementById("formMain").submit();
}

function submitForm()
{
    var myForm = document.getElementById("formMain");
    alert("submitForm - " + myForm.action);
}

function toggleReportView(btnClicked) {
    document.getElementById("btnBillingView").className = "button";
    document.getElementById("btnChartView").className = "button";
    document.getElementById("btnGridView").className = "button";

    switch (btnClicked.id)
    {
        case "btnBillingView":
            alert("Show Billing");
            break;

        case "btnChartView":
            alert("Show Chart");
            break;

        case "btnGridView":
            alert("Show Grid");
            break;
    }

    btnClicked.className = "button_selected";
}

function clearGroups()
{
    alert("clearGroups");

    var groupButton = window.opener.document.getElementById("divSelectedGroups");

    groupButton.innerHTML = "0 Groups Assigned";

    popupWindow.close();
}

function closePopupAndUpdateAdministratorCount(popupWindow, selectedAdminCount) {

    var adminsAssigned = popupWindow.parent.document.getElementById("adminCount");
    adminsAssigned.innerHTML = selectedAdminCount;
    hideJQueryDialog();
}

function closePopupAndUpdateGroupButtonCount(popupWindow, selectedGroupCount)
{
    var groupsAssigned = window.opener.document.getElementById("groupCount");
    groupsAssigned.innerHTML = selectedGroupCount;
    hideJQueryDialog();
}

function TypeDef_onchangeHandler(formItem)
{
    var field = formItem.id;
    var currentchanges = document.getElementById("HiddenChangeList").value;
    if (currentchanges)
    {
        // is this field already in changed list
        if (currentchanges.indexOf(field) == -1) {
            //no, add to change list
            document.getElementById("HiddenChangeList").value += field + dk_ItemSep;
        }
    } else {
        document.getElementById("HiddenChangeList").value = field + dk_ItemSep;
    }

    var divaction = document.getElementById("divaction");
    divaction.style.visibility = "visible";
}

function toHex(str) {
    var hex = '';
    for (var i = 0; i < str.length; i++) {
        hex += '' + str.charCodeAt(i).toString(16);
    }
    return hex;
}

function TypeDefs_UpdateChangeList()
{
    var update = "";
    var changedfieldlist = document.getElementById("HiddenChangeList").value;
    if (changedfieldlist) {
        var changedfields = changedfieldlist.split(dk_ItemSep);
        var arrayLength = changedfields.length;
        for (var i = 0; i < arrayLength; i++) {
            if (changedfields[i]) {
                var fieldvalue = document.getElementById(changedfields[i]).value;
                update += changedfields[i].replace("txt" , "") + ":" + toHex(fieldvalue) + dk_ItemSep;
            }
        }
        document.getElementById("HiddenChangeList").value = update;
    }
}

function TypeDefs_btnUpdate_onclickHandler() {
    TypeDefs_UpdateChangeList();
    document.getElementById("hiddenT").value = "UpdateTypeByName";
    var form = document.getElementById("formMain");
    form.action = "";
    form.action = form.action + "?action=update";
    var divaction = document.getElementById("divaction");
    divaction.style.visibility = "hidden";
}

function TypeDefs_btnCreateNew_onclickHandler() {
    TypeDefs_UpdateChangeList();
    document.getElementById("hiddenT").value = "NewTypeByName";
    var form = document.getElementById("formMain");
    form.action = "";
    form.action = form.action + "?action=addnew";
    var divaction = document.getElementById("divaction");
    divaction.style.visibility = "hidden";
}

function TypeDefs_btnCancel_onclickHandler() {
    document.getElementById("HiddenChangeList").value = "";
    document.getElementById("hiddenT").value = "Cancel";
    var form = document.getElementById("formMain");
    form.action = "";
    form.action = form.action + "?action=cancel";
}

function submitForm()
{
    document.getElementById("formMain").submit();
}

function navigateToGroups(selectedItem)
{
    if (selectedItem == dk_DefaultEmptyObjectId) {
        var formMain = document.getElementById("formMain");
        formMain.action = "/Admin/Groups/Default?gid=" + selectedItem;
        formMain.submit();
    }
    else
    {
        alert("Show popup for " + selectedItem);
    }
}

function navigateToClients(selectedItem)
{
    if (selectedItem == dk_DefaultEmptyObjectId) {
        var formMain = document.getElementById("formMain");
        formMain.action = "/Admin/Clients/Default?cid=" + selectedItem;
        formMain.submit();
    }
    else
    {
        alert("Show popup for " + selectedItem);
    }
}

function navigateToAdminUsers(selectedItem)
{
    if (selectedItem == dk_DefaultEmptyObjectId) {
        var formMain = document.getElementById("formMain");
        formMain.action = "/Admin/Membership/AdminUsers?auid=" + selectedItem;
        formMain.submit();
    }
    else
    {
        alert("Show popup for " + selectedItem);
    }
}

function navigateToEndUsers(selectedItem)
{
    if (selectedItem == dk_DefaultEmptyObjectId) {
        var formMain = document.getElementById("formMain");
        formMain.action = "/Admin/Membership/EndUsers?euid=" + selectedItem;
        formMain.submit();
    }
    else
    {
        alert("Show popup for " + selectedItem);
    }
}

function setPageAction()
{

    var updateButton = document.getElementById("btnSaveGroup");
    var selectedGroupName = document.getElementById("txtGroupName").value;

    var parentGroupId = $('#hiddenSelectedParentGroup').val();

    var confirmAction = confirm("Are you sure you want to save " + selectedGroupName + "?");

    if (confirmAction) {
        if (updateButton.value.indexOf("Create") > -1)
            document.getElementById("hiddenAA").value = "CreateGroup";
        else
            document.getElementById("hiddenAA").value = "UpdateGroup";

        ShowProcessingMessage();
        document.getElementById("formMain").submit();
    }
}

function assignAdministrators(selectedAdminCount) {

    var selectedGroupName = document.getElementById("txtGroupName").value;

    if (selectedAdminCount == 0) {
        alert("Please select an administrator");
        return;
    } else if (selectedAdminCount == 1) {
        var confirmAction = confirm("Assign " + selectedAdminCount + " administrator to " + selectedGroupName + "?");
        $("#adminsAssigned").val(selectedAdminCount + " administrator assigned to " + selectedGroupName + ".");
    } else {
        var confirmAction = confirm("Assign " + selectedAdminCount + " administrators to " + selectedGroupName + "?");
        $("#adminsAssigned").val(selectedAdminCount + " administrators assigned to " + selectedGroupName + ".");
    }

    //alert(confirmAction);

    ShowProcessingMessage();
    document.getElementById("formMain").submit();

    //$("#divAssignedAdminsMessage").html(alertMessage);
    //$("#divAssignedAdminsMessage").show();
}

function exportEvents()
{
    alert("exportEvents()");
}

function numberWithCommas(x) {
    if (x != null && x != "")
        return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    else
        return "0";
}

function CloseWindow() {
    window.close();
}

function showEditClientStylesPopup()
{
    alert("showEditClientStylesPopup");
}

function showRestoreDataPopup() {

    popupWindowWidth = 475;
    popupWindowHeight = 350;

    var targetUrl = "/Admin/System/RestoreDataPopup.aspx?userId=" + userId;

    showJQueryDialog(targetUrl, popupWindowWidth, popupWindowHeight);
}

function setDirectoryToRestore(itemSelected)
{
    document.getElementById("hiddenSelectedDatabaseBackup").value = itemSelected.id;
    document.getElementById("formMain").submit();
}

function finishRestoreCleanup()
{
    window.opener.HideProcessingMessage();
    window.close();

    window.opener.location = "/Logoff?msg=You must login again after a system data restoration!";
}

function showAdminNotificationProviderPopup() {

    var loggedInAdminId = document.getElementById("hiddenE").value;

    var selectedClient = $('#dlClients')[0];
    var clientId = selectedClient.options[selectedClient.selectedIndex].value;
    var clientName = selectedClient.options[selectedClient.selectedIndex].text;
    
    popupWindowWidth = 575;
    popupWindowHeight = 375;

    var phoneToSend = $('#hiddenK').val();
    var emailToSend = $('#hiddenJ').val();

    var targetUrl = "/Admin/Providers/Messaging/Email/ConfigPopup-AdminNotification?userisreadonly=" + userIsReadOnly + "&loggedInAdminId=" + loggedInAdminId + "&clientId=" + clientId + "&clientName=" + clientName + "&phoneToSend=" + phoneToSend + "&emailToSend=" + emailToSend;

    showJQueryDialog(targetUrl, popupWindowWidth, popupWindowHeight);
}

function setEventDetailIds(selectedCheckbox) {
    
    var eventId = selectedCheckbox.id.replace("chk_", "");
    var selectedEventIds = document.getElementById("hiddenSelectedEventIds");
    if (selectedCheckbox.checked) {
        selectedEventIds.value += eventId + "|";
    }
    else {
        selectedEventIds.value = selectedEventIds.value.replace(eventId + "|", "");
    }
}

function showSingleEventDetails(eventId)
{
    //alert(eventId);

    document.getElementById("hiddenSelectedEventIds").value = eventId + "|";
    showEventDetailsPopup();
}

function showEventDetailsPopup() {

    var selectedEventIds = document.getElementById("hiddenSelectedEventIds").value;
    if (selectedEventIds.indexOf("|") > -1) {
        var width = 550;
        var height = 375;

        var left = (screen.width / 2) - (width / 2);
        var top = (screen.height / 2) - (height / 2);

        var targetUrl = "/Admin/Reports/EventPopup?eventIds=" + selectedEventIds;

        showJQueryDialog(targetUrl, width, height);
    }
    else {
        alert("Please check one or more events to view details.");
        return false;
    }
}

function showGroupClientsPopup() {

    var selectedGroupId = document.getElementById("hiddenSelectedParentGroup").value;

    popupWindowWidth = 500;
    popupWindowHeight = 200;

    var targetUrl = "/Admin/Clients/GroupClientsPopup?gid=" + selectedGroupId;

    showJQueryDialog(targetUrl, popupWindowWidth, popupWindowHeight);
}

function requestOtp() {
    var selectedClient = $('#dlClients')[0];
    
    var clientId = selectedClient.options[selectedClient.selectedIndex].value;
    var clientName = selectedClient.options[selectedClient.selectedIndex].text;

    var loggedInAdminId;
    if ($('#hiddenE') !== null)
        loggedInAdminId = $('#hiddenE').val();

    var userName = $('#hiddenG').val();

    popupWindowWidth = 500;
    popupWindowHeight = 200;

    var phoneToSend = $('#hiddenK').val();
    var emailToSend = $('#hiddenJ').val();

    //var targetUrl = "/Admin/Otp/RequestPopup?loggedInAdminId=" + loggedInAdminId + "&clientId=" + clientId + "&clientName=" + clientName + "&userName=" + userName + "&phoneToSend=" + phoneToSend + "&emailToSend=" + emailToSend;
    var targetUrl = "/Admin/Otp/RequestPopup?loggedInAdminId=" + loggedInAdminId + "&clientId=" + clientId;

    showJQueryDialog(targetUrl, popupWindowWidth, popupWindowHeight);
}

function validateOtp() {
    alert("validateOtp");
}

function showAdProvidersPopup() {

    alert("showAdProvidersPopup");

    //var selectedClient = document.getElementById("dlClients");

    //var clientId = "";
    //if (selectedClient != null)
    //    clientId = selectedClient.options[selectedClient.selectedIndex].value;

    //popupWindowWidth = 275;
    //popupWindowHeight = 215;

    //var targetUrl = "/Admin/Providers/UserVerification/VerificationSelectionPopup?clientId=" + clientId;

    //showJQueryDialog(targetUrl, popupWindowWidth, popupWindowHeight);
}

function showVerificationProvidersPopup() {

    var selectedClient = document.getElementById("dlClients");

    var clientId = "";
    if (selectedClient != null)
        clientId = selectedClient.options[selectedClient.selectedIndex].value;

    popupWindowWidth = 275;
    popupWindowHeight = 215;

    var targetUrl = "/Admin/Providers/UserVerification/VerificationSelectionPopup?clientId=" + clientId;

    showJQueryDialog(targetUrl, popupWindowWidth, popupWindowHeight);
}

function editUserVerificationProvider()
{
    var selectedClient = document.getElementById("dlClients");
    var clientId = selectedClient.options[selectedClient.selectedIndex].value;

    if (clientId == dk_DefaultEmptyObjectId)
        clientId = "";

    var selectedProvider = document.getElementById("dlVerificationProviders");
    var providerId = selectedProvider.options[selectedProvider.selectedIndex].value;

    var providerName = selectedProvider.options[selectedProvider.selectedIndex].text;
    if (providerId != dk_DefaultEmptyObjectId)
    {
        popupWindowWidth = 550;
        popupWindowHeight = 400;

        popupSettings = "toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=no, resizable=no, copyhistory=no, width=" + popupWindowWidth + ", height=" + popupWindowHeight + ", top=" + top + ", left=" + left;
        popupWindow = window.open("/Admin/Providers/UserVerification/ConfigPopup?providerID=" + providerId + "&providername=" + providerName + "&clientId=" + clientId, "Provider Settings", popupSettings);
    }
}

function popupGroupManagement() {

    popupWindowWidth = 510;
    popupWindowHeight = 300;

    popupSettings = "toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=no, resizable=no, copyhistory=no, width=" + popupWindowWidth + ", height=" + popupWindowHeight + ", top=" + top + ", left=" + left;
    popupWindow = window.open("/Admin/Groups/GroupSelectionPopup", "Group Management", popupSettings);
}

function clearGroupAssignments()
{
    document.getElementById("hiddenAA").value = "ClearGroupAssignments";
    document.getElementById("formMain").submit();
}

function popupGroupAssignment() {

    popupWindowWidth = 510;
    popupWindowHeight = 350;

    var loggedInAdminId;
    if ($('#hiddenE') !== null)
        loggedInAdminId = $('#hiddenE').val();

    var clientId;
    if ($('#txtClientID') !== null)
        clientId = $('#txtClientID').val();

    if (clientId == null || clientId == "")
        clientId = dk_DefaultEmptyObjectId;

    var tmpVal = $('#btnSelectedGroups').val().split(' ');

    var groupAssignmentCount = tmpVal[0];

    var targetUrl = "/Admin/Groups/GroupAssignmentPopup?loggedInAdminId=" + loggedInAdminId + "&clientId=" + clientId;

    showJQueryDialog(targetUrl, popupWindowWidth, popupWindowHeight);
}

function popupGroupAdministrators()
{
    var groupId = document.getElementById("spanGroupID").innerHTML;

    var loggedInAdminId;
    if ($('#hiddenE') !== null)
        loggedInAdminId = $('#hiddenE').val();

    var targetUrl = "/Admin/Groups/AdminPopupGroupAssignment?loggedInAdminId=" + loggedInAdminId + "&groupID=" + groupId;

    showJQueryDialog(targetUrl, popupWindowWidth, popupWindowHeight); 
}

function popupGroupBillingConfig() {

    var groupId = document.getElementById("spanGroupID").innerHTML;

    //alert("popupGroupBillingConfig - " + groupId);

    var loggedInAdminId;
    if ($('#hiddenE') !== null)
        loggedInAdminId = $('#hiddenE').val();

    popupWindowWidth = 425;
    popupWindowHeight = 350;

    var targetUrl = "/Admin/Billing/ConfigPopup?loggedInAdminId=" + loggedInAdminId + "&ownerId=" + groupId + "&configType=Group";
    showJQueryDialog(targetUrl, popupWindowWidth, popupWindowHeight);
}

function popupClientAdministrators() {

    popupWindowWidth = 425;
    popupWindowHeight = 425;

    var clientId = "";
    if (document.getElementById("txtClientID") != null)
        clientId = document.getElementById("txtClientID").value;

    var loggedInAdminId;
    if ($('#hiddenE') !== null)
        loggedInAdminId = $('#hiddenE').val();

    var targetUrl = "/Admin/Clients/AdminPopupClientAssignment?clientId=" + clientId + "&loggedInAdminId=" + loggedInAdminId;

    showJQueryDialog(targetUrl, popupWindowWidth, popupWindowHeight);
}

function popupClientAssignment(bShowUpdateButton) {

    popupWindowWidth = 510;
    popupWindowHeight = 350;

    alert("popupClientAssignment");

    popupSettings = "toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=no, resizable=no, copyhistory=no, width=" + popupWindowWidth + ", height=" + popupWindowHeight + ", top=" + top + ", left=" + left;
    popupWindow = window.open("/Admin/Clients/ClientAssignmentPopup?ub=" + bShowUpdateButton, "Group Assignment", popupSettings);
}

function setGroupAssignment()
{
    var groupAssignmentCount = 0;
    var selectedGroups = "Group Membership: ";
    var masterGroups = document.getElementById("dlMasterGroups");
    for (var i = 0; i <= masterGroups.options.length-1; i++)
    {
        if (masterGroups[i].selected) {
            selectedGroups += masterGroups[i].text.trim() + ", ";
            groupAssignmentCount++;
        }
    }

    if (groupAssignmentCount <= 1)
        document.getElementById("btnSaveGroup").value = "Save";
    else
        document.getElementById("btnSaveGroup").value = "Save";
}

function clearGroupUpdate() {
    clearGroupListSelections();

    $('#divDisabledGroupMessageText,#divGroupManagementMessage').hide();

    $('#divGroupId,#divGroupName,#divMacOasUrl,#divMembership,#txtGroupName,#btnSelectedAdministrators,#btnSaveGroup').show();

    $('#spanGroupID').html('');
    $('#spanMembershipStats').html('Administrators: 0 - Clients: 0');

    $('#btnSelectedAdministrators').val('Administrators');

    $('#btnSaveGroup').val('Create');
    $('#btnSaveGroup').prop('disabled', false);
    $('#hiddenAA').val('CreateGroup');

    $('#txtGroupName').prop('disabled', false);
    $('#txtMACOASServicesUrl').prop('disabled', false);
    $('#btnCreateRootGroup').prop('disabled', true);

    $('#btnCancelGroup').prop('disabled', false);
    $('#txtGroupName').focus();
    $('#lblGroupName').html('Please Enter Root Group Name');

    $('#btnEnableGroup').val('Disable');
    $('#hiddenSelectedParentGroup').val('');

    // This should be dynamic based on host from server variables
    var hostName = "localhost";
    $('#txtMACOASServicesUrl').val("http://" + hostName + "/macservices/");
}

function resetGroupNameLabel() {
    $('#lblGroupName').html('Group Name');
}

function cancelPageAction() {
    clearGroupListSelections();

    $('#hiddenSelectedParentGroup').val('');
    $('#spanGroupID').html('');

    $('#btnCreateRootGroup').prop('disabled', false);

    $('#btnCreateSubRootGroup').prop('disabled', true);
    $('#btnGroupBillConfig').prop('disabled', true);

    $('#btnEnableGroup').prop('disabled', true);

    $('#btnSelectedAdministrators').prop('disabled', true);
    

    $('#btnSaveGroup').prop('disabled', true);
    $('#btnCancelGroup').prop('disabled', true);
    $('#txtGroupName').val('');
    $('#txtMACOASServicesUrl').val('');
    $('#clearSelectionControl').hide();
    //$('#txtGroupName').prop('disabled', false);
    $('#txtMACOASServicesUrl').prop('disabled', false);
    $('#divDisabledGroupMessageText,#divGroupManagementMessage').hide();

    $('#txtGroupName').prop('disabled', true);
    $('#txtMACOASServicesUrl').prop('disabled', true);
    $('#btnCreateRootGroup').prop('disabled', false);

    $("#txtGroupName").css("background", "#ddd");
    $("#txtGroupName").css("border-color", "#b3b3b3");
}

function clearGroupListSelections() {

    // Loop through all controls and set default presentation before hilighting the selected item.
    var groupControlsContainer = document.getElementById("GroupControlsContainer");
    var groupItems = groupControlsContainer.getElementsByTagName("div");

    for (var i = 0; i < groupItems.length; i++) {
        
        groupItems[i].style.backgroundColor = "transparent";

        if (groupItems[i].attributes["enabled"].value == "true")
            groupItems[i].style.color = "";
        else
            groupItems[i].style.color = "#f00";
    }

    return false;
}

function addSubGroup(parentGroupId) {

    clearGroupListSelections();

    document.getElementById(parentGroupId).style.backgroundColor = "#accee5";
    document.getElementById(parentGroupId).style.color = "#222222";

    document.getElementById("spanGroupID").innerHTML = "";

    document.getElementById("txtGroupName").value = "";
    document.getElementById("txtGroupName").focus();

    $('#btnCreateRootGroup').prop('disabled', true);

    $('#btnCreateSubRootGroup').prop('disabled', true);
    $('#btnGroupBillConfig').prop('disabled', true);

    $('#btnEnableGroup').prop('disabled', true);
    $('#btnSelectedAdministrators').prop('disabled', true);

    $('#lblGroupName').html('Please Enter Sub Group Name');

    var parentGroupName = document.getElementById("spanGroupName_" + parentGroupId).innerHTML;
    if (parentGroupName.indexOf(")") > -1) {
        var tmpVal = parentGroupName.split(")");
        parentGroupName = tmpVal[1];
    }

    document.getElementById("spanMembershipStats").innerHTML = "Administrators: 0 - Clients: 0";

    document.getElementById("btnSelectedAdministrators").value = "Administrators";
    document.getElementById("hiddenSelectedParentGroup").value = parentGroupId;

    $('#txtGroupName').val('');
    $('#txtGroupName').prop('disabled', false);

    $('#txtMACOASServicesUrl').val(currentHost + '/macservices/');
    $('#txtMACOASServicesUrl').prop('disabled', false);

    $('#btnSaveGroup').val('Create');
    $('#btnSaveGroup').prop('disabled', false);
    $('#hiddenAA').val('CreateGroup');
}

function selectGroupToList(selectedItem) {

    var hiddenYs = document.getElementById("hiddenYs");

    var itemIsSelected = document.getElementById(selectedItem.id).attributes["isselected"].value;
    if (itemIsSelected == "false") {
        document.getElementById(selectedItem.id).attributes["isselected"].value = "true";
        selectedItem.style.backgroundColor = "#accee5";
        selectedItem.style.color = "#222222";
        numberGroupsSelected++;

        // Add the select item to the list
        hiddenYs.value += selectedItem.id + ",";

    } else {
        document.getElementById(selectedItem.id).attributes["isselected"].value = "false";
        selectedItem.style.backgroundColor = "";
        selectedItem.style.color = "";
        numberGroupsSelected--;

        // Add the select item to the list
        hiddenYs.value = hiddenYs.value.replace(selectedItem.id, "");
    }

    // Need to clean this as there are 2 commas when an item has been replaced
    hiddenYs.value = hiddenYs.value.replace(",,", ",");

    if (parseInt(numberGroupsSelected) == 0)
        document.getElementById("btnSaveGroup").value = "Save";
    else if (parseInt(numberGroupsSelected) == 1)
        document.getElementById("btnSaveGroup").value = "Save";
    else if (parseInt(numberGroupsSelected) > 1)
        document.getElementById("btnSaveGroup").value = "Save";
}

function findPos(id) {

    var currentTopPosition;

    var node = document.getElementById(id);
    var curtop = 0;
    var curtopscroll = 0;
    if (node.offsetParent) {
        do {
            curtop += node.offsetTop;
            curtopscroll += node.offsetParent ? node.offsetParent.scrollTop : 0;
        } while (node = node.offsetParent);

        currentTopPosition = curtop - curtopscroll;
        return currentTopPosition;
    }
    return false;
}

function noDisplay() {
    $('#divGroupManagementMessage,#divDisabledGroupMessageText,#updateMessage').hide();
    $('#clientVerificationMessage,#assignAdministratorsMessage').hide();
}

function userVerificationMessage() {
    $('#clientVerificationMessage').css('display', 'block');
}

function adminAsssignedMessage() {
    $('#assignAdministratorsMessage').css('display', 'block');
    $('#assignAdministratorsMessage').html('Administrators have been successfully assigned!');
}

function setActiveGroup(selectedItem) {

    var UserIsReadOnly = document.getElementById("hiddenF").value.toString();

    //alert("UserIsReadOnly - " + UserIsReadOnly);

    clearGroupListSelections();

    $('#clearSelectionControl').show();

    selectedItem.style.backgroundColor = "#accee5";
    selectedItem.style.color = "#222";

    var currentPosition = findPos(selectedItem.id);
    document.getElementById("hiddenSelectedItemTopPosition").value = currentPosition;

    var selectedGroupName = selectedItem.attributes["groupname"].value;
    document.getElementById("hiddenC").value = selectedGroupName;
    document.getElementById("hiddenB").value = selectedItem.id;

    var groupName;
    var groupId;

    var selectedItemEnabled = selectedItem.attributes["enabled"].value.toLowerCase();

    var parentGroupId = document.getElementById(selectedItem.id).attributes["parentid"].value;

    var groupLogo = selectedItem.attributes["ownerlogourl"].value;

    if (UserIsReadOnly != "True")
    {
        $('#btnCreateRootGroup').prop('disabled', true);

        $('#btnCreateSubRootGroup').prop('disabled', false);
        $('#btnGroupBillConfig').prop('disabled', false);

        $('#btnEnableGroup').prop('disabled', true);
        $('#btnSelectedAdministrators').prop('disabled', true);
        $('#btnCancelGroup').prop('disabled', false);
        $('#btnCreateSubRootGroup').attr('onClick', 'javascript: addSubGroup("' + selectedItem.id + '");');

        document.getElementById("btnSelectedAdministrators").style.visibility = "visible";

        var adminButton = document.getElementById("btnSelectedAdministrators");
        adminButton.value = "Admins";
        adminButton.attributes["onclick"].value = "javascript: popupGroupAdministrators('" + groupId + "')";
    }

    if (parentGroupId != "parentId???")
    {
        var parentGroupContainer = document.getElementById(parentGroupId);
        var parentGroupEnabled = parentGroupContainer.attributes["enabled"].value;
        var parentGroupName = parentGroupContainer.attributes["groupname"].value;

        if (parentGroupId != null) {
            document.getElementById("hiddenSelectedParentGroup").value = parentGroupId;
        }

        if (document.getElementById(selectedItem.id).attributes["id"] != null) {
            groupId = document.getElementById(selectedItem.id).attributes["id"].value;
        }

        if (document.getElementById(selectedItem.id).attributes["groupname"] != null) {
            groupName = document.getElementById(selectedItem.id).attributes["groupname"].value;
        }

        if (selectedItemEnabled == "true")
            setGroupInfoControls(true, parentGroupEnabled, parentGroupName);
        else {
            setGroupInfoControls(false, parentGroupEnabled, parentGroupName);

            if (UserIsReadOnly != "True") {
                $('#btnCreateSubRootGroup').prop('disabled', true);
                $('#btnGroupBillConfig').prop('disabled', true);
            }
        }

        document.getElementById("spanGroupID").innerHTML = groupId;

        //alert(selectedItem.attributes["administratorcount"].value);

        var membershipInfo = "<span style='color: #808080;'>Administrators:</span> <span style='text-decoration: none;'>" + selectedItem.attributes["administratorcount"].value + "</span>";

        if (parseInt(selectedItem.attributes["administratorcount"].value) > 0) {
            membershipInfo = "<span style='color: #808080;'>Administrators:</span> <a href='javascript: popupGroupAdministrators(&quot;" + groupId + "&quot;);' id='link_popupGroupAdministrators'><span style='color: #ff0000; text-decoration: underline;'>" + selectedItem.attributes["administratorcount"].value + "</span></a>";
        }

        membershipInfo += "&nbsp;&nbsp;&nbsp;";
        membershipInfo += "<span style='color: #808080;'>Clients:</span> ";
        if (parseInt(selectedItem.attributes["clientcount"].value) > 0) {
            membershipInfo += "<a href='javascript: showGroupClientsPopup();' id='link_showGroupClientsPopup'>";
            membershipInfo += "<span style='color: #ff0000; text-decoration: underline;'>";
        }
        else
            membershipInfo += "<span style='font-weight: normal; text-decoration: none;'>";

        membershipInfo += selectedItem.attributes["clientcount"].value;
        membershipInfo += "</span>";

        if (parseInt(selectedItem.attributes["clientcount"].value) > 0)
            membershipInfo += "</a>";

        document.getElementById("spanMembershipStats").innerHTML = membershipInfo;
        document.getElementById("txtGroupName").value = groupName;
        document.getElementById("txtGroupName").focus();

        //var adminButton = document.getElementById("btnSelectedAdministrators");
        //adminButton.value = "Admins";
        //adminButton.attributes["onclick"].value = "javascript: popupGroupAdministrators('" + groupId + "')";

        document.getElementById("txtMACOASServicesUrl").value = selectedItem.getAttribute("MacOasServicesUrl");
        if (document.getElementById("txtMACOASServicesUrl").value == "")
            document.getElementById("txtMACOASServicesUrl").value = currentHost + "/macservices/";

        document.getElementById("hiddenSelectedParentGroup").value = groupId;
        document.getElementById("hiddenSelectedIndexNumber").value = selectedItem.attributes["indexnumber"].value;

        // This allows us to re-enable a root group if needed
        if (selectedItem.id == parentGroupId) {
            if (selectedItemEnabled != "true")
                setGroupInfoControls(false, "true", parentGroupName);
        }
    }
    else
    {
        document.getElementById("hiddenSelectedParentGroup").value = selectedItem.id;

        if (document.getElementById(selectedItem.id).attributes["groupname"] != null) {
            groupName = document.getElementById(selectedItem.id).attributes["groupname"].value;
        }

        document.getElementById("txtGroupName").value = groupName;
        document.getElementById("txtMACOASServicesUrl").value = selectedItem.getAttribute("MacOasServicesUrl");

        var membershipInfo = "Administrators: <span style='color: #000000; text-decoration: none;'>" + selectedItem.attributes["administratorcount"].value + "</span>";

        if (parseInt(selectedItem.attributes["administratorcount"].value) > 0) {
            membershipInfo = "Administrators: <span style='color: #ff0000; text-decoration: underline;'>" + selectedItem.attributes["administratorcount"].value + "</span>";
        }

        membershipInfo += "&nbsp;&nbsp;&nbsp;Clients: ";
        if (parseInt(selectedItem.attributes["clientcount"].value) > 0) {
            membershipInfo += "<a href='javascript: showGroupClientsPopup();' id='link_showGroupClientsPopup'>";
            membershipInfo += "<span style='font-weight: normal; color: #ff0000; text-decoration: underline;'>";
        }
        else
            membershipInfo += "<span style='font-weight: normal; text-decoration: none;'>";

        membershipInfo += selectedItem.attributes["clientcount"].value;
        membershipInfo += "</span>";

        if (parseInt(selectedItem.attributes["clientcount"].value) > 0)
            membershipInfo += "</a>";

        document.getElementById("spanMembershipStats").innerHTML = membershipInfo;

        if (UserIsReadOnly != "True") {
            $('#btnCreateRootGroup').prop('disabled', true);

            $('#btnCreateSubRootGroup').prop('disabled', false);
            $('#btnGroupBillConfig').prop('disabled', false);

            $('#btnEnableGroup').prop('disabled', false);
            $('#btnSelectedAdministrators').prop('disabled', false);
            $('#btnCancelGroup').prop('disabled', false);

            document.getElementById("btnSaveGroup").value = "Save";
        }
    }

    // Process group logo
    if (groupLogo == "")
        groupLogo = "/Images/OwnerLogos/!Empty-Placeholder.png";

    document.getElementById("imgOwnerLogo").src = groupLogo;
}

function setSelectedItemFocus() {

    if (document.getElementById("GroupControlsContainer") != null) {
        
        var selectedItem = getElementByAttributeValue("isselected", "true");

        if (selectedItem != undefined && selectedItem != false)
        {
            selectedItem.style.backgroundColor = "#accee5";
            selectedItem.style.color = "#222";

            var itemHeight = parseInt(selectedItem.clientHeight);
            var selectedItemTopPosition = parseInt(selectedItem.attributes["indexnumber"].value) * itemHeight;

            // Now we need to position elements inside the container accordingly.
            $("#GroupControlsContainer").each(function () {
                this.scrollTop = parseInt(selectedItemTopPosition);
            });
        }
        if(selectedItem != false)
            setActiveGroup(selectedItem);
    }
}

function resetGroupControlsContainer(selectedItem) {

    document.getElementById("link_popupGroupAdministrators").innerHTML = "0";
    document.getElementById("link_showGroupClientsPopup").innerHTML = "0";
    document.getElementById("imgOwnerLogo").src = "/Images/OwnerLogos/!Empty-Placeholder.png";

    clearGroupListSelections();
    cancelPageAction();
    return false;
}

function getElementByAttributeValue(attribute, value)
{
    var allElements = document.getElementsByTagName('*');
    for (var i = 0; i < allElements.length; i++)
    {
        if (allElements[i].getAttribute(attribute) == value)
        {
            return allElements[i];
        }
    }
    return false;
}

function setGroupInfoControls(bShowControls, parentGroupEnabled, parentGroupName) {

    if (bShowControls) {

        $('#divDisabledGroupMessageText').hide();

        $("#txtGroupName,#txtMACOASServicesUrl,#btnSelectedAdministrators,#btnSaveGroup").prop("disabled", false);

        $('#btnEnableGroup').val('Disable');
    } else {

        $('#divDisabledGroupMessageText').show();

        $("#txtGroupName,#txtMACOASServicesUrl,#btnSelectedAdministrators,#btnSaveGroup").prop("disabled", true);

        $('#btnEnableGroup').val('Enable');
    }

    if (parentGroupEnabled == "true") {
        $('#divDisabledGroupMessageText').html('This parent-group and any sub-groups have been disabled.');
        $('#btnEnableGroup').attr("disabled", false);
    } else {
        $('#divDisabledGroupMessageText').html('This sub-group has been disabled. It' + "'" +'s parent-group <span style="font-weight: bold;">' + parentGroupName + '</span> must be re-enabled first.');
        $('#btnEnableGroup').attr("disabled", true);
    }
}

function enableGroup(btnEnableGroup) {

    switch (btnEnableGroup.value)
    {
        case "Enable":
            $('#btnCreateSubRootGroup').prop('disabled', true);
            $('#btnGroupBillConfig').prop('disabled', true);

            $('#btnSelectedAdministrators').prop('disabled', true);
            $('#btnSaveGroup').prop('disabled', true);
            $('#btnCancelGroup').prop('disabled', true);

            if (confirm("Enable Group?") == true) {
                $('#btnEnableGroup').val('Disable');
                $('#btnEnableGroup').prop('disabled', true);
                $('#hiddenAA').val('EnableGroup');

                ShowProcessingMessage();
                $('#formMain').submit();
            } else {
                $('#btnCreateSubRootGroup').prop('disabled', false);
                $('#btnGroupBillConfig').prop('disabled', false);

                $('#btnSelectedAdministrators').prop('disabled', false);
                $('#btnSaveGroup').prop('disabled', false);
                $('#btnCancelGroup').prop('disabled', false);
                return;
            }            
            break;

        case "Disable":
            $('#btnCreateSubRootGroup').prop('disabled', true);
            $('#btnGroupBillConfig').prop('disabled', true);

            $('#btnSelectedAdministrators').prop('disabled', true);
            $('#btnSaveGroup').prop('disabled', true);
            $('#btnCancelGroup').prop('disabled', true);

            if (confirm("Disable Group?") == true) {
                $('#btnEnableGroup').val('Enable');
                $('#btnEnableGroup').prop('disabled', true);
                $('#hiddenAA').val('DisableGroup');

                ShowProcessingMessage();
                $('#formMain').submit();
            } else {
                $('#btnCreateSubRootGroup').prop('disabled', false);
                $('#btnGroupBillConfig').prop('disabled', false);

                $('#btnSelectedAdministrators').prop('disabled', false);
                $('#btnSaveGroup').prop('disabled', false);
                $('#btnCancelGroup').prop('disabled', false);
                return;
            }
            break;
    }
    
}

function popupHandleSelectedGroup(groupName)
{
    window.opener.document.getElementById("divSelectedGroup").innerHTML = "&nbsp;&nbsp;" + groupName + " group selected&nbsp;&nbsp;";
    window.close();
}

function toggleProviderSelected(selectedCheckbox, providerType, providerId) {

    var selectedProvidersCount = 0;

    var divProvidersContainer = document.getElementById("divProvidersContainer");
    var selectedProviders = divProvidersContainer.getElementsByTagName("input");

    for (var i = 0; i < selectedProviders.length; i++)
    {
        if(selectedProviders[i].checked)
            selectedProvidersCount++;
    }

    document.getElementById("btnClientActions").disabled = false;

    popupWindowWidth = 510;
    popupWindowHeight = 350;

    var selectedProviderName = selectedCheckbox.id;

    var tmpProviderName = selectedProviderName;
    tmpProviderName = tmpProviderName.replace(" (Email)", "");
    tmpProviderName = tmpProviderName.replace(" (Sms)", "");
    tmpProviderName = tmpProviderName.replace(" (Verification)", "");
    tmpProviderName = tmpProviderName.replace(" (Voice)", "");

    var providerChecked = selectedCheckbox.checked;

    var retryListBox = document.getElementById("selectProviderList");
    var hiddenRetryList = document.getElementById("hiddenRetryList");
    var hiddenProvidersToDelete = document.getElementById("hiddenProvidersToDelete");

    var editIcon = document.getElementById("img_" + selectedProviderName);
    var providerNameLinkSpan = document.getElementById("span_" + selectedProviderName);

    if (selectedCheckbox.checked) {
        // Set to false. It will be set from the provider popup. If the popup is cancelled, then we do nothing.
        selectedCheckbox.checked = false;

        var targetUrl = "/Admin/Providers/Messaging/" + providerType + "/ConfigPopup?userisreadonly=" + userIsReadOnly + "&loggedInAdminId=" + $('#hiddenE').val() + "&providerID=" + providerId + "&clientId=" + $('#txtClientID').val();

        showJQueryDialog(targetUrl, popupWindowWidth, popupWindowHeight);
    }
    else {
        if (selectedProvidersCount < 1) {
            alert("You must6666666666666666666 select at least 1 other message provider before removing this one!");

            selectedCheckbox.checked = true;

            document.getElementById("btnClientActions").disabled = true;
            return false;
        }
        else {
            selectedCheckbox.checked = false;

            // Remove the hyperlink from the provider name (disabled)
            providerNameLinkSpan.innerHTML = "<span id='span_" + selectedProviderName + "' style='color: #c0c0c0; font-size: 13px;'>" + tmpProviderName + "</span>";

            // Disable the edit icon
            editIcon.src = "../../Images/icon-edit-disabled.png";

            hiddenRetryList.value = hiddenRetryList.value.replace(selectedProviderName, "");

            // Cleanup extra pipes after remove of a provider
            hiddenRetryList.value = hiddenRetryList.value.replace("||", "|");

            // Add selected provider to delete list
            hiddenProvidersToDelete.value += "|" + selectedProviderName;

            //alert(hiddenProvidersToDelete.value);

            if (hiddenRetryList.value == "|") {
                // Delete this straggler
                hiddenRetryList.value = "";
            }

            // Remove the selected provider from the listbox
            for (var i = 0; i < retryListBox.options.length; i++) {
                if (retryListBox.options[i].value.toLowerCase() == selectedProviderName.toLowerCase()) {
                    retryListBox.remove(i);
                }
            }
        }
    }
}

function editMessageProvider(providerId, providerType)
{
    //alert(providerId + ", " + providerType);

    popupWindowWidth = 510;
    popupWindowHeight = 350;

    var targetUrl = "/Admin/Providers/Messaging/" + providerType + "/ConfigPopup?userisreadonly=" + userIsReadOnly + "&loggedInAdminId=" + $('#hiddenE').val() + "&providerID=" + providerId + "&clientId=" + $('#txtClientID').val();

    showJQueryDialog(targetUrl, popupWindowWidth, popupWindowHeight);
}

//begin Client assignments tab functions **********
function clientVerificationAssignment() {

    $("#userVerificationContainer").html("");

    var selectedClient = document.getElementById("dlClients");

    var clientId = "";
    if (selectedClient != null)
        clientId = selectedClient.options[selectedClient.selectedIndex].value;

    var targetUrl = "/Admin/Providers/UserVerification/VerificationSelectionPopup?userisreadonly=" + userIsReadOnly + "&clientId=" + clientId;

    $("#userVerificationContainer").html("<iframe src='" + targetUrl + "' id='userVerificationFrame' name='assignmentContainer' style='display: block;width: 100%;height: 550px;overflow-y: auto;'></iframe>");
}

function clientIPAddressAssignment()
{
    $("#userVerificationContainer").html("");

    var selectedClient = document.getElementById("dlClients");

    var clientId = "";
    if (selectedClient != null)
        clientId = selectedClient.options[selectedClient.selectedIndex].value;

    var targetUrl = "/Admin/Clients/IPAssignmentPopup?userisreadonly=" + userIsReadOnly + "&userid=" + userId + "&clientId=" + clientId;

    //alert(targetUrl);

    $("#clientAssignmentsContainer").html("<iframe src='" + targetUrl + "' id='allowedIpAddressesFrame' name='assignmentContainer' style='display: block;width: 100%;height: 550px;overflow: none;'></iframe>");
}

function advertisingAssignment() {

    $("#AdPassConfigurationContainer").html("");

    var selectedClient = document.getElementById("dlClients");

    var clientId = "";
    if (selectedClient != null)
        clientId = selectedClient.options[selectedClient.selectedIndex].value;

    var targetUrl = "/Admin/Providers/Advertising/ConfigPopup?userisreadonly=" + userIsReadOnly + "&userid=" + userId + "&clientId=" + clientId;

    $("#AdPassConfigurationContainer").html("<iframe src='" + targetUrl + "' id='AdPassConfigurationFrame' name='assignmentContainer' style='display: block;width: 100%;height: 550px;overflow-y: auto;'></iframe>");
}

function userAssignment()
{
    $("#clientAssignmentsContainer").html("");

    var clientId = "";
    if (document.getElementById("txtClientID") != null)
        clientId = document.getElementById("txtClientID").value;

    var adminId = "";
    if (document.getElementById("hiddenE") != null)
        adminId = document.getElementById("hiddenE").value;

    var targetUrl = "/Admin/Users/ClientUsers?userisreadonly=" + userIsReadOnly + "&clientId=" + clientId + "&loggedInAdminId=" + adminId;

    showClientAssignments(targetUrl);
}

function clientAdminAssignment() {

    $("#clientAssignmentsContainer").html("");

    var clientId = "";
    if (document.getElementById("txtClientID") != null)
        clientId = document.getElementById("txtClientID").value;

    var adminId = "";
    if (document.getElementById("hiddenE") != null)
        adminId = document.getElementById("hiddenE").value;

    var targetUrl = "/Admin/Clients/AdminPopupClientAssignment?userisreadonly=" + userIsReadOnly + "&clientId=" + clientId + "&loggedInAdminId=" + adminId;

    showClientAssignments(targetUrl);
}

function clientGroupAssignment() {

    $("#clientAssignmentsContainer").html("");

    var loggedInUserId = $('#hiddenE').val();
    var clientId;

    if ($('#txtClientID') !== null)
        clientId = $('#txtClientID').val();

    if (clientId == null || clientId == "")
        clientId = dk_DefaultEmptyObjectId;

    var tmpVal = $("#groupCount").html();

    var groupAssignmentCount = tmpVal[0];

    var targetUrl = "/Admin/Groups/GroupAssignmentPopup?userisreadonly=" + userIsReadOnly + "&loggedInUserId=" + loggedInUserId + "&clientId=" + clientId;

    showClientAssignments(targetUrl);
}

function securetradingConfig()
{

    var loggedInUserId = $('#hiddenE').val();
    var clientId;

    if ($('#txtClientID') !== null)
        clientId = $('#txtClientID').val();

    if (clientId == null || clientId == "")
        clientId = dk_DefaultEmptyObjectId;

    var targetUrl = "/Admin/Providers/SecureTrading/ConfigPopup?userisreadonly=" + userIsReadOnly + "&loggedInUserId=" + loggedInUserId + "&clientId=" + clientId;

    showClientAssignments(targetUrl);
}

function showClientAssignments(targetUrl) {
    $("#clientAssignmentsContainer").html("<iframe src='" + targetUrl + "' id='clientAssignmentsFrame' name='assignmentContainer' scrolling='no' style='display: block; width: 100%; height: 485px; overflow-y: auto;'></iframe>");
}

function assignmentSelection() {

    //var targetUrl = "/Admin/Clients/IPAssignmentPopup.aspx?userisreadonly=" + userIsReadOnly + "&userid=" + userId + "&clientId=" + clientId;

    var selection = $("#assignmentMenu").val();

    if (selection == "Please Select") {
        $("#clientAssignmentsContainer").hide();
        return;
    }        
    else if (selection == "AdPass Configuration") {
        $("#clientAssignmentsContainer").show();
        advertisingAssignment();
    }
    else if (selection == "User Verification") {
        $("#clientAssignmentsContainer").show();
        clientVerificationAssignment();
    }
    else if (selection == "Allowed IP Adresses") {
        $("#clientAssignmentsContainer").show();
        clientIPAddressAssignment();
    }
    else if (selection == "Administrator Assignments") {
        $("#clientAssignmentsContainer").show();
        clientAdminAssignment();
    }
    else if (selection == "Group Assignments") {
        $("#clientAssignmentsContainer").show();
        clientGroupAssignment();
    }
    else if (selection == "Secure Trading Site Config") {
        $("#clientAssignmentsContainer").show();
        securetradingConfig();
    }
    else if (selection == "Service Pricing") {
        $("#clientAssignmentsContainer").show();
        clientPricing();
    }
    else if (selection == "User Assignments") {
        $("#clientAssignmentsContainer").show();
        userAssignment();
    }
}
//end Client assignments tab functions **********

function clientPricing()
{
    //alert("clientPricing");

    $("#clientAssignmentsContainer").html("");

    var clientId = "";
    if (document.getElementById("txtClientID") != null)
        clientId = document.getElementById("txtClientID").value;

    var adminId = "";
    if (document.getElementById("hiddenE") != null)
        adminId = document.getElementById("hiddenE").value;

    var targetUrl = "/Admin/Pricing/ConfigPopup?clientId=" + clientId + "&loggedInAdminId=" + adminId;

    showClientAssignments(targetUrl);
}

//system Management Functions **********
function systemManagementSelection() {
    var selection = $("#systemManagementFunctions").val();
    if (selection == "Please Select") {
        $("#clientAssignmentsContainer").hide();
        return;
    }
    else if (selection == "Backup") {
        document.getElementById("formMain").submit();
    }
    else if (selection == "Change Environment Settings") {
        changeEnvironmentSettings();
    }
    else if (selection == "Create EventStats") {
        createEventStats();
    }
    else if (selection == "Create Message Template") {
        createMessageTemplate();
    }
    else if (selection == "Reset") {
        resetSystemData();
    }
    else if (selection == "Restore") {
        showRestoreDataPopup();
    }
    else if (selection == "Delete Relationships") {
        document.getElementById("formMain").submit();
    }
    else if (selection == "Delete Test Data") {
        deleteTestData();
    }
    else if (selection == "Create Indexes") {
        document.getElementById("formMain").submit();
    }
    else if (selection == "Update Stats") {
        updateStatsAndLists();
    }
    else if (selection == "Reset IPs") {
        resetIPs();
    }
    else if (selection == "Reset Ad Providers") {
        resetAdProviders();
    }
    else if (selection == "Reset Message Providers") {
        resetMessageProviders();
    }
}
//end system Management Functions **********

function showJQueryDialog(targetUrl, popupWindowWidth, popupWindowHeight) {
    // Scroll window to top. If not, then if the browser is towards the bottom of the window, the dialog is out of view at the top
    window.scrollTo(0, 0);

    //determine lightbox background dimensions
    var contentHeight = $(window).height() - 50;

    //determine lightbox content dimensions
    popupWindowWidth = 550;
    popupWindowHeight = contentHeight;

    if (window.innerWidth < 550) {
        popupWindowWidth = window.innerWidth - 20;
    }

    //determine lightbox content position
    var moveTop = 10;
    var moveLeft = (window.innerWidth / 2) - 275;

    if (window.innerWidth < 550) {
        moveLeft = 0;
        moveTop = 10;
    }

    //determine lightbox background dimensions
    var documentHeight = $(document).height();
    var documentWidth = $(document).width();

    //assign dimensions and position to lightboxes
    $('#divPleaseWaitProcessing_popup').css("height", documentHeight);
    $('#divPleaseWaitProcessing_popup').css("width", documentWidth);

    $('#divDialogContainer_popup').css("top", moveTop);
    $('#divDialogContainer_popup').css("left", moveLeft);
    $('#divDialogContainer_popup').css("overflow", "hidden");

    $('#divPleaseWaitProcessing_popup').show();

    $('#divDialogContainer_popup').html("<iframe id='frameDialog' src='" + targetUrl + "' frameborder='no' align='center' width='" + popupWindowWidth + "' height='" + popupWindowHeight + "' style='opacity: 1.0 !important;overflow-x: hidden;overflow-y: auto !important;'></iframe>");

    $('#divDialogContainer_popup').show();

    $(window).resize(function () {
        // Scroll window to top. If not, then if the browser is towards the bottom of the window, the dialog is out of view at the top
        window.scrollTo(0, 0);

        //determine lightbox background dimensions
        var contentHeight = $(window).height() - 50;

        //determine lightbox content dimensions
        popupWindowWidth = 550;
        popupWindowHeight = contentHeight;

        if (window.innerWidth < 550) {
            popupWindowWidth = window.innerWidth - 20;
        }

        //determine lightbox content position
        var moveTop = 10;
        var moveLeft = (window.innerWidth / 2) - 275;

        if (window.innerWidth < 550) {
            moveLeft = 0;
            moveTop = 10;
        }

        //determine lightbox background dimensions
        var documentHeight = $(document).height();
        var documentWidth = $(document).width();

        //assign dimensions and position to lightboxes
        $('#divPleaseWaitProcessing_popup').css("height", documentHeight);
        $('#divPleaseWaitProcessing_popup').css("width", documentWidth);

        $('#divDialogContainer_popup').css("top", moveTop);
        $('#divDialogContainer_popup').css("left", moveLeft);
        $('#divDialogContainer_popup').css("overflow", "hidden");

        $('#divDialogContainer_popup').html("<iframe id='frameDialog' src='" + targetUrl + "' scrolling='no' frameborder='no' align='center' width='" + popupWindowWidth + "' height='" + popupWindowHeight + "' style='opacity: 1.0 !important;overflow-x: hidden;overflow-y: auto;'></iframe>");
    });
}

function findPopupDocumentHeight() {
    $("#frameDialog").load(function () {
        var h = $(this).height($(this).contents().find("body").height());
    });

    var b = $("#frameDialog").contentWindow.document.body.offsetHeight;

    var iframeHeight = $("#frameDialog").height();

    alert(b);
}

function hideJQueryDialog() {

    var iFrameDoc = $('#frameDialog')[0];
    var originalDialogContent = "";
    originalDialogContent += "<img id='imgPleaseWaitImage' alt='' src='/Images/Please-Wait-2.gif' />";

    $('#divDialogContainer2').html(originalDialogContent);

    $('#divPleaseWaitProcessing_popup,#divDialogContainer_popup').hide();
}

function resubmitFormFromChild() {

    //alert("resubmitFormFromChild");

    var parentForm = document.getElementById('formMain');
    parentForm.action = "/Admin/Default?config=changed";
    parentForm.submit();
}

function addToProvidersList(providerToAddToRetryList)
{
    //alert(providerToAddToRetryList);

    var retryListString = "";

    var hiddenRetryList = document.getElementById("hiddenRetryList");
    var listOptions = document.getElementById("selectProviderList");

    var i;
    var listInsertIndex;
    var option;

    // Add selected provider to Retry List
    listInsertIndex = listOptions.length;
    option = document.createElement("option");
    option.text = providerToAddToRetryList;
    option.value = option.text;
    option.setAttribute("onclick", "deleteFromRetryList(this)");
    listOptions.add(option, listInsertIndex);

    hiddenRetryList.value += "|" + providerToAddToRetryList;

    document.getElementById("btnClientActions").disabled = false;
}

function resetRetryList() {
    document.getElementById("selectProviderList").options.length = 0;
    document.getElementById("hiddenProvidersToDelete").value = document.getElementById("hiddenRetryList").value;
    document.getElementById("hiddenRetryList").value = "";
}

function deleteFromRetryList(itemSelected) {
    alert("deleteFromRetryList" + itemSelected);
    var listOptions = document.getElementById("selectProviderList");

    // Check to see if at least 1 one provider is defined in the retry list.
    // If not, warn and stop update until at least 1 is selected
    if (listOptions.length <= 1)
    {
        alert("You must3333333333333 select at least 1 provider in the retry list");

        document.getElementById("btnClientActions").disabled = true;

        return false;
    }
    else {
        document.getElementById("btnClientActions").disabled = false;

        listOptions.remove(listOptions.selectedIndex);

        var retryList = document.getElementById("hiddenRetryList");
        var currentHiddenRetry = retryList.value.replace(itemSelected.text + dk_ItemSep, "");

        retryList.value = currentHiddenRetry.replace(dk_ItemSep + dk_ItemSep, "");
    }
    return true;
}

function convertUTCDateTimeToLocal(timeToConvert)
{
    var d = new Date(timeToConvert);
    var date = d.toLocaleDateString();
    var time = d.toLocaleTimeString();

    var convertedDate = date + " " + time;

    // No builtin JS methods for short conversion
    convertedDate = convertedDate.replace("Sunday", "Sun");
    convertedDate = convertedDate.replace("Monday", "Mon");
    convertedDate = convertedDate.replace("Tuesday", "Tue");
    convertedDate = convertedDate.replace("Wednesday", "Wed");
    convertedDate = convertedDate.replace("Thursday", "Thur");
    convertedDate = convertedDate.replace("Friday", "Fri");
    convertedDate = convertedDate.replace("Saturday", "Sat");

    convertedDate = convertedDate.replace("January", "Jan");
    convertedDate = convertedDate.replace("February", "Feb");
    convertedDate = convertedDate.replace("March", "Mar");
    convertedDate = convertedDate.replace("April", "Apr");
    convertedDate = convertedDate.replace("May", "May");
    convertedDate = convertedDate.replace("June", "Jun");
    convertedDate = convertedDate.replace("July", "Jul");
    convertedDate = convertedDate.replace("August", "Aug");
    convertedDate = convertedDate.replace("September", "Sep");
    convertedDate = convertedDate.replace("October", "Oct");
    convertedDate = convertedDate.replace("November", "Nov");
    convertedDate = convertedDate.replace("December", "Dec");

    return convertedDate.toString();
}

function clearForm()
{
    $("#divExistingAccounts").hide();
    $("#divServiceResponseContainer").html("");
}

function clearDefaultValue(currentField) {
    var fieldLabel = $('label[for="' + currentField.id + '"]').html();
    var fieldValue = $("#" + currentField.id).val();

    if (fieldValue == fieldLabel) {
        $("#" + currentField.id).val("");
    }
}

function ClearEventDates() {
    document.getElementById("datepickSD").value = "";
    document.getElementById("datepickED").value = "";
};

// Reports > Operations
function toggleTestSystemDisplay() {
    $('#TestSystemSettingsDisplay').slideToggle(250);
    var currentDisplay = $('#linkTestSystemSettings').html();
    $('html, body').animate({
        scrollTop: ($('#scroll2').offset().top)
    }, 750, 'easeOutExpo');
    if (currentDisplay == '[-] Test System Settings') {
        $('#linkTestSystemSettings').html('[<span style="font-size: 1rem;">+</span>] Test System Settings');
        $('#linkTestSystemSettings').attr('title', 'Show Settings');
    } else {
        $('#linkTestSystemSettings').html('[-] Test System Settings');
        $('#linkTestSystemSettings').attr('title', 'Hide Settings');
    }
}
function toggleSystemsDisplay() {
    var x = $("#systemsDisplay").css("display");
    if (x == "none") {
        $('#systemsDisplay').slideToggle(250);
        $('html, body').animate({
            scrollTop: ($('#scroll2').offset().top)
        }, 750, 'easeOutExpo');
    }
    else {
        return;
    }
}
function cancelSystemsDisplay() {
        $('#systemsDisplay').slideToggle(250);
        $('html, body').animate({
            scrollTop: ($('#scroll2').offset().top)
        }, 750, 'easeOutExpo');
}
function toggleConfigurationsDisplay() {
    var x = $("#configurationsDisplay").css("display");
    if (x == "none") {
        $('#configurationsDisplay').slideToggle(250);
        $('html, body').animate({
            scrollTop: ($('#scroll2').offset().top)
        }, 750, 'easeOutExpo');
    }
    else {
        return;
    }
}
function cancelConfigurationsDisplay() {
    $('#configurationsDisplay').slideToggle(250);
    $('html, body').animate({
        scrollTop: ($('#scroll2').offset().top)
    }, 750, 'easeOutExpo');
}
function testSystemUnderTestSelection() {
    var selection = $("#dlSystemsUnderTest").val();
    alert(selection);
    if (selection == "Please Select") {
        $("#btnSystemNew").val("New");
        $("#btnSystemDelete").attr("disabled", "disabled");
        $("#systemsDisplay").hide();
        return;
    }
    else {
        $("#btnSystemNew").val("Edit");
        $("#btnSystemDelete").removeAttr("disabled");
        $("#systemsDisplay").hide();
        return;
    }
}
function testConfigurationsSelection() {
    var selection = $("#dlConfigTabConfigurations").val();

    if (selection == "Please Select") {
        $("#btnConfigurationsNew").val("New");
        $("#btnConfigurationsDelete").attr("disabled", "disabled");
        $("#configurationsDisplay").hide();
        return;
    }
    else {
        $("#btnConfigurationsNew").val("Edit");
        $("#btnConfigurationsDelete").removeAttr("disabled");
        $("#configurationsDisplay").hide();
        return;
    }
}
function btnAddToContactList_Click() {
    var mDlContactList = $('#dlSystemUnderTestContactList');
    var firstItem = mDlContactList.val();
    var firstItemText = firstItem.trim(' ').toLowerCase();

    if (firstItemText == "none") {
        alert("remove");
        mDlContactList.empty();
        mDlContactList.val("sdfsdf");
        alert(mDlContactList.val());
    }
    return;
    var mText = $("#txtContactFirstName").val() + " " + $("#txtContactLastName").val();
    //alert(mText);
    var mValue = "~"
    + "FirstName" + $("#txtContactFirstName").val()
    + "|LastName" + $("#txtContactLastName").val()
    + "|PhoneNumber" + $("#txtContactMobilePhone").val()
    + "|EmailAddress" + $("#txtContactEmailaddress").val();
    //alert(mValue);

    var listItem = "<option value='" + mValue + "'>" + mText + "</option>";
    alert("append");
    mDlContactList.append(listItem);
    alert("appended");

    $("#txtContactFirstName").val = "";
    $("#txtContactLastName").val = "";
    $("#txtContactMobilePhone").val = "";
    $("#txtContactEmailaddress").val = "";

    // Update the list display
    alert("Trigger");
    $("#dlSystemUnderTestContactList").trigger("chosen:updated");
    alert("focus");
    $("#btnSaveSystemUnderTest").focus();
    alert("done");
}
function deleteSystem() {
    confirm("Delete the selected System?");
    return;
}
function deleteConfiguration() {
    confirm("Delete the selected Configuration?");
    return;
}
// Test Results Accordian Tab
function resultsTabSystemsSelection() {
    var selection = $("#dlResultsTabSystems").val();
    alert(selection);
    if (selection == "Please Select") {
        //$("#btnConfigurationsNew").val("New");
        //$("#btnConfigurationsDelete").attr("disabled", "disabled");
        //$("#ConfigurationsDisplay").hide();
        return;
    }
    else {
        //$("#btnConfigurationsNew").val("Edit");
        //$("#btnConfigurationsDelete").removeAttr("disabled");
        //$("#ConfigurationsDisplay").hide();
        return;
    }
}

function resultsTabTestRunsSelection() {
    var selection = $("#dlResultsTabTestRuns").val();

    if (selection == "Please Select") {
        //$("#btnConfigurationsNew").val("New");
        //$("#btnConfigurationsDelete").attr("disabled", "disabled");
        //$("#ConfigurationsDisplay").hide();
        return;
    }
    else {
        //$("#btnConfigurationsNew").val("Edit");
        //$("#btnConfigurationsDelete").removeAttr("disabled");
        //$("#ConfigurationsDisplay").hide();
        return;
    }
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

// ----------- test functions ---------------
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
        return ($(this).text() == text );
    }).prop('selected', true);
    // change the chosen client
    $("#" + ddl_name).trigger("chosen:updated");
    // need to submit the form in order to get the client information populated
    $("#" + formid).submit();
    // return the selected option to insure the option exists
    return $("#" + ddl_name + " option:selected").text();
}

function getBackupNameFromMsg(message) {
    var BackupName = message.replace("Data successfully Backed up to ", "").replace("!", "");
    return BackupName;
}

function jsTextInput(fieldid, text) {
    $("#" + fieldid).val(text);
}

function TestDecrypt() {
    //alert("TestDecrypt");
    var mEvalue = $("#txtEncryptedData").val();
    var mKey = $("#txtKey").val();
    var mValue = Decrypt(mEvalue, mKey);
    $("#txtDecryptedData").val(mValue);
}

function TestEncrypt() {
    var mValue = $("#txtClearData").val();
    var mKey = $("#txtKey").val();
    var mEvalue = "";
    mEvalue = Encrypt(mValue, mKey);
    $("#txtEncryptedData").val(mEvalue);
}