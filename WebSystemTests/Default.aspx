<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <link href="foundation/css/foundation.css" rel="stylesheet" />

    <link href="css/Admin.css" rel="stylesheet" />

    <script src="foundation/js/vendor/modernizr.js"></script>
    <script src="js/jquery-1.10.2.js"></script>
    <script src="js/jquery-ui-1-11-0.js"></script>

    <title>Web System Tests</title>

    <script type="text/javascript">

        var applicationAreasInitialized = false;

        var testErrorStopped = false;

        var calledApplicationName = "";
        var calledApplicationArea = "";
        var calledTestArea = "";

        var applicationContainerInitialized = false;

        var defaultCheckAllItems = "checked='checked'";
        defaultCheckAllItems = "";

        var ApplicationAreaCount = 0;
        var testCasesCount = 0;
        var testResultsCount = 0;

        var rowHeight = 35;

        var testResult = "";
        var systemUnderTest = "";
        var testScript = "";
        var testPassed = "";
        var testResultLabel = "Passed";
        var testResultClassName = "TestResultPassed";

        var currentUrl = window.location.toString();

        //alert(currentUrl.indexOf("localhost"));

        var localhostServicePrefix = "";
        if (currentUrl.indexOf("localhost") > -1)
            localhostServicePrefix = "/websystemtests";
        else
            localhostServicePrefix = "";

        $(document).ready(function () {
            //On tab click, set hidden value 'hiddenActiveAccordionTab'
            $('#OpsTestSystemConfigurationTab').click(function () {
                $('#hiddenActiveAccordionTab').val('OpsTestSystemConfigurationTab');
            });
            $('#SystemUnderTestConfigurationTab').click(function () {
                $('#hiddenActiveAccordionTab').val('SystemUnderTestConfigurationTab');
            });
            $('#RegressionTestTab').click(function () {
                $('#hiddenActiveAccordionTab').val('RegressionTestTab');
            });

            //Expand and position current tab on reload
            var currentTab = $('#hiddenActiveAccordionTab').val();

            if (currentTab == 'OpsTestSystemConfigurationTab') {
                $('#' + currentTab + '').click();

            } else if (currentTab == 'SystemUnderTestConfigurationTab') {
                $('#' + currentTab + '').click();

            } else if (currentTab == 'RegressionTestTab') {
                $('#' + currentTab + '').click();
            }

            GetTestRunEnvironmentsList();

            // Disable test selection controls until a test environment is selected.
            var testSelectionContainer = document.getElementById("divTestSelectionContainer");

            document.getElementById("divTestResultHistory").style.display = "block";
            document.getElementById("divRunControls").style.display = "block";
            document.getElementById("divProcessingStatus").style.display = "none";

        });

        function SelectAllApplications() {

            // Clear the functional areas and test cases boxes
            document.getElementById("ApplicationAreaList").innerHTML = "";
            document.getElementById("TestCaseList").innerHTML = "";

            document.getElementById("lblApplicationAreasLabel").innerHTML = "Select 1 or more Functional Areas";
            document.getElementById("lblTestCasesLabel").innerHTML = "Select 1 or more Test Cases";

            var allAppsSelected = false;

            var applicationsToTest = document.getElementById("div_ApplicationsToTest");

            var collectionAppsToTest = applicationsToTest.getElementsByTagName("input");

            for (var i = 0; i < collectionAppsToTest.length; i++) {

                var currentApplication = collectionAppsToTest[i];

                var applicationName = currentApplication.id.toString();
                applicationName = applicationName.replace(/chk_/g, '');
                applicationName = applicationName.replace(/ /g, '_');

                switch (applicationName) {
                    case "Select_All_Applications":
                        // Set this global that drives the setting for each app
                        if (currentApplication.checked)
                            allAppsSelected = true;
                        else
                            allAppsSelected = false;
                        break;
                    default:

                        currentApplication.checked = allAppsSelected;
                        if (currentApplication.checked) {
                            // Call web service to get functional areas for current app
                            GetApplicationAreas(applicationName);
                        }
                        else {
                            // Clear the functional areas and test cases boxes
                            document.getElementById("ApplicationAreaList").innerHTML = "";
                            document.getElementById("TestCaseList").innerHTML = "";
                        }
                        break;
                }
            }
        }

        function SelectAllApplicationTests(applicationToTest)
        {
            var appToTestContainer = document.getElementById("divApplicationContainer_" + applicationToTest);

            var applicationTests = appToTestContainer.getElementsByTagName("input");

            for (var i = 0; i < applicationTests.length; i++) {
                // Index 0 is select all
                applicationTests[i].checked = applicationTests[0].checked;
            }
        }

        function GetApplicationAreas(applicationName) {
            var currentSelectedItem = document.getElementById("chk_" + applicationName);

            // See if the current item is checked. If so, call the service. If not, let's clear the functional list
            if (currentSelectedItem.checked) {

                // Initialize the application test container
                if (!applicationContainerInitialized) {
                    
                    var testAreasParentContainer = "";

                    testAreasParentContainer = "<div id='divApplicationContainer_" + applicationName + "' class='ApplicationContainer'>";

                    testAreasParentContainer += "   <span>";
                    testAreasParentContainer += "       <input id='chk_Select_All_" + applicationName.replace(/ /g, '_') + "_Areas' " + defaultCheckAllItems + " onclick='javascript: SelectAllApplicationTests(&quot;" + applicationName + "&quot;);' testarea='All' applicationarea='All' application='" + applicationName + "' style='margin-right: 5px;' type='checkbox' />";
                    testAreasParentContainer += "   </span>";
                    testAreasParentContainer += "   <span>";
                    testAreasParentContainer += "   <span style='font-weight: bold; position: relative; top: -2px;'>";
                    testAreasParentContainer += "       Select All (" + applicationName.replace(/_/g, ' ') + ") Tests";
                    testAreasParentContainer += "   </span>";
                    testAreasParentContainer += "</div>";

                    var caseList = document.getElementById("TestCaseList");
                    caseList.innerHTML = testAreasParentContainer;

                    applicationContainerInitialized = true;
                }

                // Call web service to get functional areas for current app
                var webMethod = localhostServicePrefix + "/webservices/GetApplicationAreas.asmx/WsGetApplicationAreas";
                var parameters = "ApplicationName=" + applicationName.replace(/ /g, '_');

                $.post(webMethod, parameters, ProcessServiceResponseApplicationAreas);
            }
            else {
                // Remove the selected area container
                var appAreaContainerName = "div_" + applicationName + "_Areas";

                // Handle App Areas
                // Remove the selected applicationareas element
                var currentlySelectedAppAreas = document.getElementById(appAreaContainerName);

                //alert(currentlySelectedAppAreas.id);

                // Get number of children so we can decrement the item count count correctly
                var numberOfItemsToRemove = currentlySelectedAppAreas.getElementsByTagName("input").length - 1;

                // Remove this element from the DOM
                currentlySelectedAppAreas.parentNode.removeChild(currentlySelectedAppAreas);

                ApplicationAreaCount = ApplicationAreaCount - numberOfItemsToRemove;
                // Handle App Areas

                // Handle Test Areas
                var currentlySelectedTestAreas = document.getElementById("divApplicationContainer_" + applicationName);
                // Remove this element from the DOM
                currentlySelectedTestAreas.parentNode.removeChild(currentlySelectedTestAreas);

                // Get the current app item count
                var appAreaParentContainer = document.getElementById("ApplicationAreaList");
                var appAreas = appAreaParentContainer.getElementsByTagName("input");
                document.getElementById("spanApplicationAreaCount").innerHTML = appAreas.length;

                // Get the current test item count
                var testCasesParentContainer = document.getElementById("TestCaseList");
                var testCases = testCasesParentContainer.getElementsByTagName("input");
                document.getElementById("spanTestCasesCount").innerHTML = testCases.length;
            }
        }

        function ProcessServiceResponseApplicationAreas(serviceResponse) {

            var areaCount = 0;
            var areaHeight = 25;

            $(serviceResponse).find('serviceresponse ').each(function () {
                calledApplicationName = $(this).attr("application");
            });

            var applicationAreasContainer = "";
            var applicationAreas = "";

            applicationAreasContainer += "<div id='div_" + calledApplicationName + "_Areas' style='border-bottom: solid 1px #c0c0c0; height: SetMyHeightAfterProcessing; padding: 0; padding-left: 5px; padding-bottom: 0px; margin-bottom: 0px;'>";
            applicationAreasContainer += "    <div style='border: solid 0px #ff0000; padding: 0; padding-top: 5px; padding-left: 5px; height: " + areaHeight + "px;'>";
            applicationAreasContainer += "      <span>";
            applicationAreasContainer += "            <input id='chk_Select_All_" + calledApplicationName.replace(/ /g, '_') + "_Areas' " + defaultCheckAllItems + " onclick='javascript: SelectAllApplicationAreas(&quot;" + calledApplicationName + "&quot;);' style='margin-right: 5px;' type='checkbox' />";
            applicationAreasContainer += "      <span>";
            applicationAreasContainer += "      <span style='font-weight: bold; position: relative; top: -2px;'>Select all (" + calledApplicationName.replace(/_/g, ' ') + ")</span>";
            applicationAreasContainer += "    </div>";

            $(serviceResponse).find('area').each(function () {
                try {
                    areaCount++;
                    ApplicationAreaCount++;

                    // Reset for current iteration
                    applicationAreas += "<div style=' position: relative; top: 0px; border: solid 0px #ff0000; padding: 0; padding-left: 20px; height: " + areaHeight + "px;'>";
                    applicationAreas += "      <span>";
                    applicationAreas += "            <input id='chk_" + $(this).text().replace(/ /g, '_') + "' " + defaultCheckAllItems + " onclick='javascript: GetTestCases(&quot;" + calledApplicationName + "&quot;,&quot;" + $(this).text() + "&quot;);' style='margin-right: 5px; height: 25px; padding: 0;' type='checkbox' />";
                    applicationAreas += "      <span>";

                    applicationAreas += "      <span style='position: relative; top: -8px;'>" + areaCount + ". " + $(this).text() + "</span>";
                    applicationAreas += "</div>";

                    // Get test cases for current area
                    var currentTestArea = $(this).text().replace(/ /g, '_');

                    GetTestCases(calledApplicationName, currentTestArea);
                }
                catch (Exception) {
                    //alert(Exception + ": " + currentTestArea);
                }
            });

            // Increment to support the select all element
            areaCount = areaCount + 2;

            var containerHeight = (areaCount * areaHeight);
            applicationAreasContainer = applicationAreasContainer.replace("SetMyHeightAfterProcessing", containerHeight + "px");

            // Add the test cases to the container
            applicationAreasContainer += applicationAreas;

            // Close the application area
            applicationAreasContainer += "</div>";

            document.getElementById("ApplicationAreaList").innerHTML += applicationAreasContainer;

            document.getElementById("spanApplicationAreaCount").innerHTML = ApplicationAreaCount;
        }

        function GetTestCases(calledApplicationName, selectedApplicationArea) {

            //alert("calledApplicationName: " + calledApplicationName + " - selectedApplicationArea: " + selectedApplicationArea);

            // Get application parent container
            var parentContainerId = "div_" + calledApplicationName + "_Areas";

            var parentAppContainer = document.getElementById(parentContainerId);

            selectedApplicationArea = selectedApplicationArea.replace(/ /g, '_');

            var ApplicationAreaName = "chk_" + selectedApplicationArea;

            var currentSelectedItem = document.getElementById(ApplicationAreaName);

            if (currentSelectedItem != null) {

                // See if the current item is checked. If so, call the service. If not, let's clear the functional list
                if (currentSelectedItem.checked) {
                    // Call web service to get test cases for selected functional area
                    calledApplicationArea = selectedApplicationArea;

                    var webMethod = localhostServicePrefix + "/webservices/GetTestCases.asmx/WsGetTestCases";

                    var parameters = "ApplicationName=" + calledApplicationName.replace(/ /g, '_'); //.replace(" ", "_");
                    parameters += "&ApplicationAreaName=" + selectedApplicationArea.replace(/ /g, '_'); //.replace(" ", "_");

                    $.post(webMethod, parameters, ProcessServiceResponseTestCases);
                }
                else {
                    // Remove the selected areas
                    var testCasesContainerName = "div_" + selectedApplicationArea + "_Test_Area_Container";

                    // Remove the selected test cases element
                    var currentlySelectedTestCases = document.getElementById(testCasesContainerName);

                    // Get number of items to remove and decrement count
                    var itemsToRemoveCount = currentlySelectedTestCases.getElementsByTagName("input").length - 1;
                    testCasesCount = testCasesCount - itemsToRemoveCount;

                    currentlySelectedTestCases.parentNode.removeChild(currentlySelectedTestCases);
                }
            }
            else {
                //alert("ApplicationArea is Null: " + ApplicationAreaName);
            }

            document.getElementById("spanTestCasesCount").innerHTML = testCasesCount;
        }

        function ProcessServiceResponseTestCases(serviceResponse) {

            var applicationAreaInitialized = false;

            var caseList = document.getElementById("TestCaseList");

            var testCases = "";
            var itemCount = 0;

            $(serviceResponse).find('serviceresponse').each(function () {
                calledApplicationName = $(this).attr("application");
                calledApplicationArea = $(this).attr("applicationarea");
            });

            var testAreaContainer = "";
            testAreaContainer += "<div id='div_" + calledApplicationArea + "_Test_Area_Container' application='" + calledApplicationName + "' applicationarea='" + calledApplicationArea + "' testarea='" + calledApplicationArea + "' class='TestAreaContainer'>";

            if (!applicationAreaInitialized) {
                testAreaContainer += "<div id='div_" + calledApplicationArea + "_Application_Area' class='ApplicationAreaTitle'>";

                // Area title
                testCases += "<span style='position: relative; top: 0px; left: 0px;'>";
                testCases += "  <input id='chk_" + $(this).text().replace(/ /g, '_') + "' " + defaultCheckAllItems + " class='TestCase' application='" + calledApplicationName + "' applicationarea='" + calledApplicationArea + "' testarea='" + calledApplicationArea + "' onclick='javascript: SelectAllTestCases(&quot;" + calledApplicationArea + "&quot;);' type='checkbox' />";
                testCases += "<span>";

                testCases += "<span style='position: relative; top: -2px; left: 5px;'>";
                testCases += "  (" + calledApplicationArea.replace(/_/g, ' ') + ") Test Area</div>";
                testCases += "<span>";

                applicationAreaInitialized = true;
            }

            applicationAreaInitialized = false;

            $(serviceResponse).find('testarea').each(function () {

                calledTestArea = $(this).attr("name");

                // Get test cases
                $(this).children().each(function () {

                    testCasesCount++;
                    itemCount++;

                    var testCaseFileName = $(this).text().replace(/_/g, ' ').replace(/.txt/g, '');

                    testCases += "    <div id='testcase_" + testCaseFileName.replace(/ /g, '_') + "' class='TestCaseContainer'>";
                    testCases += "      <span>";
                    testCases += "            <input id='chk_" + $(this).text().replace(/ /g, '_') + "' " + defaultCheckAllItems + " class='TestCase' application='" + calledApplicationName + "' applicationarea='" + calledApplicationArea + "' testarea='" + calledTestArea + "' type='checkbox' />";
                    testCases += "      <span>";

                    testCases += "      <span class='TestCaseTitle'>" + itemCount + ". " + testCaseFileName + "</span>";
                    testCases += "    </div>";
                });
            });

            // Add test cases to container
            testAreaContainer += testCases;

            testAreaContainer += "</div>";

            var applicationContainer = document.getElementById("divApplicationContainer_" + calledApplicationName);
            applicationContainer.innerHTML += testAreaContainer;

            document.getElementById("spanTestCasesCount").innerHTML = testCasesCount;
        }

        function SelectAllApplicationAreas(calledApplication) {

            var selectedApplication = document.getElementById("div_" + calledApplication + "_Areas");
            var applicationAreas = selectedApplication.getElementsByTagName("input");

            for (var i = 0; i < applicationAreas.length; i++) {
                // Index 0 is select all
                applicationAreas[i].checked = applicationAreas[0].checked;

                if (applicationAreas[i].checked) {
                    // Get test cases
                    var selectedApplicationArea = applicationAreas[i].id.replace(/chk_/g, ''); //.replace("chk_", "");

                    // Do not call GetTestCases if Select All
                    if (selectedApplicationArea.indexOf("Select_All") < 0) {
                        GetTestCases(calledApplicationName, selectedApplicationArea);
                    }
                }
            }
        }

        function SelectAllTestCases(selectedTestArea) {

            var selectedTestAreaContainer = document.getElementById("div_" + selectedTestArea + "_Test_Area_Container");
            var testCases = selectedTestAreaContainer.getElementsByTagName("input");

            for (var i = 0; i < testCases.length; i++) {
                // Index 0 is select all
                testCases[i].checked = testCases[0].checked;
            }
        }

        function GetTestUsers() {

            var selectedSystemToTest = $('#dlTargetServers').find(':selected');
            var targetDatabaseServer = selectedSystemToTest.attr("targetdatabaseserver");
            var targetDatabaseName = selectedSystemToTest.attr("targetdatabasename");

            // Get web service test users for system
            var webMethod = localhostServicePrefix + "/webservices/GetUsers.asmx/WsGetUsers";
            var parameters = "systemUnderTest=" + selectedSystemToTest.val();

            $.post(webMethod, parameters, ProcessServiceResponseTestUsers);
        }

        function ProcessServiceResponseTestUsers(serviceResponse) {

            var adminCount = 0;

            var dlAdminList = $('#dlTestUsers');
            dlAdminList.empty();
            dlAdminList.append("<option value='Select a User to test'>Select a User to test</option>");

            $(serviceResponse).find('user').each(function () {
                adminCount++;
                try {
                    var adminName = $(this).attr("name");
                    var adminUserName = $(this).attr("username");
                    var adminRole = $(this).attr("role");
                    var adminNameAndRole = adminName + " (" + adminRole + ")";

                    var listItem = "<option id='" + adminUserName + "' value='" + adminUserName + "'>" + adminCount + ") " + adminNameAndRole + "</option>";
                    dlAdminList.append(listItem);
                }
                catch (Exception) {
                    alert(Exception);
                }
            });
        }

        function RunTests()
        {
            // System to test
            var systemList = document.getElementById("dlTargetServers");
            var selectedSystemToTest = systemList.options[systemList.selectedIndex].value;
            if (systemList.selectedIndex == 0) {
                alert("Please select a system to test");
                systemList.focus()
                return false;
            }

            // User to test
            var adminList = document.getElementById("dlTestUsers");
            var selectedSystemToTest = adminList.options[adminList.selectedIndex].value;
            if (adminList.selectedIndex == 0) {
                alert("Please select an Administrator to test");
                adminList.focus()
                return false;
            }

            document.getElementById("btnRunTests").disabled = true;

            var currentTestCount = 0;
            var currentTestIncrement = 0;

            var scriptPath = "";

            var applicationToTest = "";

            // Loop through all selected tests and call each one syncronously
            var testCasesContainer = document.getElementById("TestCaseList");
            var testCases = testCasesContainer.getElementsByTagName("input");

            for (var i = 0; i < testCases.length; i++) {

                // Stop processing if error stopped
                if (testErrorStopped)
                {
                    document.getElementById("btnRunTests").disabled = false;
                    return;
                }
                else
                {
                    var currentApplication = testCases[i].attributes["application"].value;
                    var currentApplicationArea = testCases[i].attributes["applicationarea"].value;

                    var currentTestArea = testCases[i].attributes["testarea"].value;
                    var currentTestCase = testCases[i].id.replace(/chk_/g, '');

                    if (currentTestCase.indexOf("Select_All") > -1) {
                        // Do not process this!
                    }
                    else {

                        if (currentTestCase.indexOf(".txt") > 0) {
                            if (testCases[i].checked) {

                                document.getElementById("divProcessingStatusDetails").innerHTML = "Processing test <span style='font-weight: bold; color: #ff0000;'>" + testResultsCount + "</span> > <span style='font-weight: bold; color: #ff0000;'>" + currentTestCase.replace(/_/g, ' ').replace(/.txt/g, '') + "</span>...";

                                scriptPath = "\\TestCases\\" + currentApplication + "\\" + currentApplicationArea + "\\" + currentTestArea + "\\" + currentTestCase;

                                var selectedAdminToTest = $('#dlTestUsers').find(':selected').val();
                                var selectedSystemToTest = $('#dlTargetServers').find(':selected').val();

                                var runId = "";
                                if (document.getElementById("txtRunId") != null)
                                    if (document.getElementById("txtRunId").innerHTML != "");
                                runId = document.getElementById("txtRunId").innerHTML;

                                var tmpRunId = runId.split(' ');
                                var datePart = tmpRunId[0].replace(/\//g, '-');
                                var timePart = tmpRunId[1].replace(/:/g, '~');
                                var AMPMPart = tmpRunId[2];

                                runId = datePart + " " + timePart + " " + AMPMPart;

                                // Call test service here...
                                var webMethod = localhostServicePrefix + "/webservices/RunTestService.asmx/WsRunTestService";

                                var parameters = "";
                                parameters += "URL:" + "http://" + selectedSystemToTest;

                                parameters += "|USER:" + selectedAdminToTest;

                                parameters += "|RUNID:" + runId;
                                parameters += "|PATH:" + scriptPath;

                                //alert("parameters - " + parameters);

                                parameters = StringToHex(parameters);

                                document.getElementById("divTestResultHistory").style.display = "none";
                                document.getElementById("divRunControls").style.display = "none";
                                document.getElementById("divProcessingStatus").style.display = "block";

                                jQuery.ajaxSetup({ async: false });
                                {
                                    $.get(webMethod, { data: parameters },
                                        function (serviceResponse, status) {

                                            testResultsCount++;

                                            $(serviceResponse).find('serviceresponse').each(function () {
                                                systemUnderTest = $(this).attr("systemundertest");
                                                testScript = $(this).attr("testscript");
                                                testPassed = $(this).attr("testpassed");
                                            });

                                            $(serviceResponse).find('testresult').each(function () {
                                                testResult = $(this).text();
                                            });

                                            var tmpScriptPath = decodeURIComponent(testScript);

                                            var tmpVal = tmpScriptPath.split('\\');
                                            var appName = tmpVal[2].replace(/_/g, ' ').replace(/\+/g, ' ');
                                            var appArea = tmpVal[3].replace(/_/g, ' ').replace(/\+/g, ' ');
                                            var testArea = tmpVal[4].replace(/_/g, ' ').replace(/\+/g, ' ');
                                            var testName = tmpVal[5].replace(/_/g, ' ').replace(/.txt/g, '').replace(/\+/g, ' '); //tmpVal[tmpVal.length - 1].replace(/_/g, ' ');

                                            if (testPassed.toLowerCase() == "false") {
                                                testResultLabel = "<span style='color: #ff0000;'>Failed!</span>";

                                                var stopOnError = document.getElementById("chkStopOnError").checked;
                                                if (stopOnError) {
                                                    alert("A test (" + testName + ") resulted in an error. Processing stopped!");
                                                    testErrorStopped = true;
                                                    document.getElementById("btnRunTests").disabled = false;

                                                    document.getElementById("divTestResultHistory").style.display = "block";
                                                    document.getElementById("divRunControls").style.display = "block";
                                                    document.getElementById("divProcessingStatus").style.display = "none";

                                                    return false;
                                                }
                                            }
                                            else {
                                                testResultLabel = "<span style=''>Passed</span>";
                                            }

                                            var testResultContainer = "";
                                            testResultContainer += "<div id='divTestResultContainer' class='" + testResultClassName + "' style='height: " + rowHeight + "px;'>";

                                            testResultContainer += "<div id='divTestNumber_' style='vertical-align: middle; text-align: right; padding-right: 10px; float: left; border: solid 0px #ff0000; width: 40px; height: " + rowHeight + "px;'>";
                                            testResultContainer += testResultsCount.toString() + ") ";
                                            testResultContainer += "</div>"

                                            testResultContainer += "<div id='divAppName_" + testResultsCount.toString() + "' style='vertical-align: middle; float: left; border: solid 0px #ff0000; width: 130px; height: " + rowHeight + "px;'>";
                                            testResultContainer += appName;
                                            testResultContainer += "</div>"

                                            testResultContainer += "<div id='divAppArea_" + testResultsCount.toString() + "' style='vertical-align: middle; float: left; border: solid 0px #ff0000; width: 155px; height: " + rowHeight + "px;'>";
                                            testResultContainer += appArea;
                                            testResultContainer += "</div>"

                                            testResultContainer += "<div id='divTestArea_" + testResultsCount.toString() + "' style='vertical-align: middle; float: left; border: solid 0px #ff0000; width: 180px; height: " + rowHeight + "px;'>";
                                            testResultContainer += testArea;
                                            testResultContainer += "</div>"

                                            testResultContainer += "<div id='divTestName_" + testResultsCount.toString() + "' style='vertical-align: middle; float: left; border: solid 0px #ff0000; width: 305px; height: " + rowHeight + "px;'>";
                                            testResultContainer += testName;
                                            testResultContainer += "</div>"

                                            testResultContainer += "<div id='divTestResult_" + testResultsCount.toString() + "' style='vertical-align: middle; float: left; border: solid 0px #ff0000; width: 50px; height: " + rowHeight + "px;'>";
                                            testResultContainer += testResultLabel;
                                            testResultContainer += "</div>"

                                            testResultContainer += "<div id='divTestView' style='vertical-align: middle; float: left; border: solid 0px #ff0000; width: 50px; height: " + rowHeight + "px;'>";
                                            testResultContainer += "<a href='javascript: ViewTestResult(&apos;" + testResult + "&apos;);'>View</a>";
                                            testResultContainer += "</div>"

                                            testResultContainer += "</div>";

                                            document.getElementById("divTestResults").innerHTML += testResultContainer;

                                            document.getElementById("btnRunTests").disabled = false;

                                            // Reset RunId
                                            var now = new Date();
                                            now.format("dd/mm/yyyy hh:mm:ss tt");

                                            document.getElementById("txtRunId").innerHTML = now.toString();

                                        });
                                }
                                jQuery.ajaxSetup({ async: true });
                            }
                        }
                    }
                }
            }

            GetTestRunEnvironmentsList();

            document.getElementById("divTestResultHistory").style.display = "block";
            document.getElementById("divRunControls").style.display = "block";
            document.getElementById("divProcessingStatus").style.display = "none";
            document.getElementById("divProcessingStatus").innerHTML = "Processing complete!";
        }

        function GetTestRunEnvironmentsList()
        {
            var webMethod = localhostServicePrefix + "/webservices/GetTestResults.asmx/WsGetTestRunEnvironments";

            var parameters = "";

            $.post(webMethod, parameters, ProcessServiceResponseTestEnvironmentList);
        }

        function ProcessServiceResponseTestEnvironmentList(serviceResponse) {

            var runCount = 0;
            var testRun = "";

            var dlTestRuns = $('#ddlTestEnvironments');

            dlTestRuns.empty();
            dlTestRuns.append("<option value='Select an environment to view previous results'>Select an environment to view previous results</option>");

            $(serviceResponse).find('environment').each(function () {

                runCount++;
                testRun = $(this).text();

                dlTestRuns.append("<option value='" + testRun + "'>" + testRun.replace(/_/g, ' ') + "</option>");
            });
        }

        function GetPreviousTestRunsList() {

            // Clear any results still showing
            document.getElementById("divTestResults").innerHTML = "";

            var selectedSystemToTest = $('#ddlTestEnvironments').find(':selected').text();

            var webMethod = localhostServicePrefix + "/webservices/GetTestResults.asmx/WsGetTestRuns";

            var parameters = "targetEnvironment=" + selectedSystemToTest;

            $.post(webMethod, parameters, ProcessServiceResponsePreviousTestRunList);
        }

        function ProcessServiceResponsePreviousTestRunList(serviceResponse) {

            var runCount = 0;
            var testRun = "";

            var dlTestRuns = $('#ddlTestRuns');

            dlTestRuns.empty();
            dlTestRuns.append("<option value='Select a test run to view results'>Select a test run to view results</option>");

            $(serviceResponse).find('testrun').each(function () {

                runCount++;
                testRun = $(this).text();

                dlTestRuns.append("<option value='" + testRun + "'>" + testRun.replace(/_/g, ' ').replace(/~/g, ':') + "</option>");
            });
        }

        function GetPreviousResultsDetails()
        {
            var testRunEnvironment = document.getElementById("ddlTestEnvironments");
            var selectedTestEnvironment = testRunEnvironment.options[testRunEnvironment.selectedIndex].value;

            var testRunList = document.getElementById("ddlTestRuns");
            var selectedTestRun = testRunList.options[testRunList.selectedIndex].value;

            var selectedRunPath = selectedTestEnvironment + "\\" + selectedTestRun;

            var webMethod = localhostServicePrefix + "/webservices/GetTestResults.asmx/WsGetTestRunDetails";

            var parameters = "RunToFetch=" + selectedRunPath;

            // Clear the log display
            document.getElementById("divTestResults").innerHTML = "";

            $.post(webMethod, parameters, ProcessServiceResponseTestRunDetails);
        }

        function ProcessServiceResponseTestRunDetails(serviceResponse) {

            var appName = "appName";
            var appArea = "appArea";
            var testArea = "testArea";
            var testName = "testName";
            var testResultLabel = "Passed";
            var testResult = "View";

            var currentAppArea = "";

            var responseData = "";
            var testResultsCount = 0;

            var borderDef = "border: solid 0px #ff0000";

            var reportLinks = "";

            $(serviceResponse).find('testrun ').each(function () {
                appName = $(this).attr("appname").replace(/_/g, ' ');
            });

            $(serviceResponse).find('testarea').each(function () {

                reportLinks = "";
                testResultsCount++;

                appArea = $(this).attr("apparea").replace(/_/g, ' ');
                testArea = $(this).attr("testarea").replace(/_/g, ' ');

                $(this).find('testfolder').each(function () {

                    testName = $(this).attr("name").replace(/_/g, ' ');

                    testResult = $(this).attr("results");
                    switch(testResult.toLowerCase())
                    {
                        case "failed":
                            testResult = "<span style='color: #ff0000;'>Failed!</span>";
                            break;

                        case "passed":
                            testResult = "<span style='color: #049115;'>Passed</span>";
                            break;
                    }

                    var testResultContainer = "";
                    testResultContainer += "<div id='divTestResultContainer' class='" + testResultClassName + "' style='height: " + rowHeight + "px;'>";

                    testResultContainer += "<div id='divTestNumber_' style='vertical-align: middle; text-align: right; padding-right: 10px; float: left; " + borderDef + "; width: 40px; height: " + rowHeight + "px;'>";
                    testResultContainer += testResultsCount.toString() + ") ";
                    testResultContainer += "</div>"

                    //testResultContainer += "<div id='divAppName_" + testResultsCount.toString() + "' style='vertical-align: middle; float: left; " + borderDef + "; width: 125px; height: " + rowHeight + "px;'>";
                    //testResultContainer += appName;
                    //testResultContainer += "</div>"

                    testResultContainer += "<div id='divAppArea_" + testResultsCount.toString() + "' style='vertical-align: middle; float: left; " + borderDef + "; width: 100px; height: " + rowHeight + "px;'>";
                    testResultContainer += appArea;
                    testResultContainer += "</div>"

                    testResultContainer += "<div id='divTestArea_" + testResultsCount.toString() + "' style='vertical-align: middle; float: left; " + borderDef + "; width: 190px; height: " + rowHeight + "px;'>";
                    testResultContainer += testArea;
                    testResultContainer += "</div>"

                    testResultContainer += "<div id='divTestName_" + testResultsCount.toString() + "' style='vertical-align: middle; float: left; " + borderDef + "; width: 290px; height: " + rowHeight + "px;'>";
                    testResultContainer += testName;
                    testResultContainer += "</div>"

                    testResultContainer += "<div id='divTestResult_" + testResultsCount.toString() + "' style='vertical-align: middle; float: left; " + borderDef + "; width: 65px; height: " + rowHeight + "px;'>";
                    testResultContainer += testResult;
                    testResultContainer += "</div>"

                    // Get report files and create links for UI
                    testResultContainer += "<div id='divTestView' style='vertical-align: middle; float: left; " + borderDef + "; width: 225px; height: " + rowHeight + "px;'>";
                    var linkCount = 0;
                    reportLinks = "";
                    $(this).find('file').each(function () {

                        var tmpLink = $(this).text().split('/');
                        var fileName = tmpLink[tmpLink.length - 1];

                        if (linkCount != 0)
                            reportLinks += "&nbsp;<span style='color: #c0c0c0;'>|</span>&nbsp;";

                        var linkPath = $(this).attr("path");

                        reportLinks += "<a href='javascript: ShowReportPopup(&quot;" + linkPath + "&quot;);'>";
                        reportLinks += fileName.replace(/html/g, '').replace(/xml/g, '').toTitleCase();
                        reportLinks += "</a>";

                        linkCount++;
                    });
                    testResultContainer += reportLinks;
                    testResultContainer += "</div>"

                    testResultContainer += "</div>";

                    document.getElementById("divTestResults").innerHTML += testResultContainer;
                });
            });
        }

        String.prototype.toTitleCase = function (n) {
            var s = this;
            if (1 !== n) s = s.toLowerCase();
            return s.replace(/\b[a-z]/g, function (f) { return f.toUpperCase() });
        }


        function ShowReportPopup(reportUrl)
        {
            //var targetUrl = encodeURI(reportUrl);

            //alert(reportUrl);

            var win = window.open(reportUrl, '_blank');
            win.focus();
        }

        function ViewTestResult(testResult) {
            testResult = decodeURIComponent(testResult);
            alert(testResult);
        }

        function ClearLogView()
        {
            document.getElementById("divTestResults").innerHTML = "";

            var now = new Date();
            now.format("dd/mm/yyyy hh:mm:ss tt");

            document.getElementById("txtRunId").innerHTML = now.toString();

            testResultsCount = 0;
        }

        function DeletePreviousTestResults()
        {
            alert("DeletePreviousTestResults");
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

    </script>
</head>
<body>
    <div id="scroll1" style="padding: 0.5rem;"></div>
    <form id="form1" runat="server">
        <div class="row">
            <div class="large-12 columns">
                <dl class="accordion" data-accordion>
                    <dd class="accordion-navigation">
                        <a id="OpsTestSystemConfigurationTab" runat="server" href="#panel1b">Ops Test System Configuration</a>
                        <div id="panel1b" class="content">
                            <div class="row">
                                <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.125rem;">
                                    <label>
                                        Name
                                    <input type="text" id="txtTestSystemName" runat="server" />
                                    </label>
                                </div>
                                <div class="large-3 medium-3 small-12 columns" style="margin-bottom: 0.125rem;">
                                    <label>
                                        IP
                                    <input type="text" id="txtTestSystemIP" runat="server" />
                                    </label>
                                </div>
                                <div class="large-3 medium-3 small-12 columns" style="margin-bottom: 0.125rem;">
                                    <label>
                                        Host
                                    <input type="text" id="txtTestSystemHost" runat="server" />
                                    </label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="large-3 medium-3 small-12 columns" style="margin-bottom: 0.125rem;">
                                    <label>
                                        Run Interval
                                    <input type="text" id="txtTestSystemRunInterval" runat="server"
                                        title="Interval in minutes" />
                                    </label>
                                </div>
                                <div class="large-3 medium-3 small-12 columns" style="margin-bottom: 0.125rem;">
                                    <label>
                                        Last Run
                                    <input type="text" id="txtTestSystemLastRun" runat="server" />
                                    </label>
                                </div>
                                <div class="large-3 medium-3 small-12 columns" style="margin-bottom: 0.125rem;">
                                    <label>
                                        Results Lookup (days)
                                    <input type="text" id="txtResultsLookupDays" runat="server" />
                                    </label>
                                </div>
                                <div class="large-3 medium-3 small-12 columns" style="margin-bottom: 0.125rem;">
                                    <label>
                                        Systems Under Test
                                    <input type="text" id="txtTestSystemCountSystemsUnderTest" runat="server" />
                                    </label>
                                </div>
                            </div>
                            <!-- ========= SMS Provider settings ======================== -->
                            <div class="row">
                                <div class="large-12 columns">
                                    <%--<div style="text-align:center;color:#d8d8d8;">
                                    &#9899;&nbsp;
                                    <asp:Label runat="server" ID="lbSMSProviderName" Text="..."/>
                                    (SMS) Settings&nbsp;&#9899;
                                </div>--%>
                                    <fieldset style="margin-top: 0.25rem !important;">
                                        <legend>
                                            <asp:Label runat="server" ID="lbSMSProviderName" Text="" />
                                            SMS Settings</legend>
                                        <div class="row">
                                            <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.125rem;">
                                                <label>
                                                    URL
                                                <input type="text" id="txtSMSURL" runat="server" />
                                                </label>
                                            </div>
                                            <div class="large-3 medium-3 small-12 columns" style="margin-bottom: 0.125rem;">
                                                <label>
                                                    Batch ID
                                                <input type="text" id="txtSMSBatchId" runat="server" />
                                                </label>
                                            </div>
                                            <div class="large-3 medium-3 small-12 columns" style="margin-bottom: 0.125rem;">
                                                <label>
                                                    AuthToken
                                                <input type="text" id="txtSMSAuthToken" runat="server" />
                                                </label>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.125rem;">
                                                <label>
                                                    Sid
                                                <input type="text" id="txtSMSSid" runat="server" />
                                                </label>
                                            </div>
                                            <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.125rem;">
                                                <label>
                                                    Text MessageTemplate
                                                <input type="text" id="txtSMSMessageTemplate" runat="server" />
                                                </label>
                                            </div>
                                        </div>
                                    </fieldset>
                                </div>
                            </div>

                            <!-- ========= Email Provider settings ======================== -->
                            <div class="row" id="div1" runat="server">
                                <div class="large-12 columns">
                                    <fieldset style="margin-top: 0.25rem !important;">
                                        <legend>Email Settings</legend>
                                        <div class="row">
                                            <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.125rem;">
                                                <label>
                                                    Server
                                                <input type="text" id="txtTestSystemEmailServer" runat="server" />
                                                </label>
                                            </div>
                                            <div class="large-2 medium-2 small-12 columns" style="margin-bottom: 0.125rem;">
                                                <label>
                                                    Port
                                                <input type="text" id="txtTestSystemEmailPort" runat="server" />
                                                </label>
                                            </div>
                                            <div class="large-2 medium-2 small-12 columns" style="margin-bottom: 0.125rem;">
                                                <label>Enable SSL</label>
                                                <asp:CheckBox runat="server" ID="cbTestSystemEmailEnableSsl" />
                                            </div>
                                            <div class="large-2 medium-2 small-12 columns" style="margin-bottom: 0.125rem;">
                                                <label>Use Def. Credentials</label>
                                                <asp:CheckBox runat="server" ID="cbTestSystemEmailUseDefaultCredentials" />
                                            </div>
                                        </div>
                                    </fieldset>
                                </div>
                            </div>
                            <div class="row">
                                <div class="large-3 medium-3 small-12 columns" style="margin-bottom: 0.125rem;">
                                    <label>
                                        Login
                                    <input type="text" id="txtTestSystemEmailLogin" runat="server" />
                                    </label>
                                </div>
                                <div class="large-3 medium-3 small-12 columns" style="margin-bottom: 0.125rem;">
                                    <label>
                                        PWD
                                    <input type="text" id="txtTestSystemEmailPWD" runat="server" />
                                    </label>
                                </div>
                                <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.125rem;">
                                    <label>
                                        From Address
                                    <input type="text" id="txtTestSystemEmailFromAddress" runat="server" />
                                    </label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.125rem;">
                                    <label>
                                        Email Subject
                                    <input type="text" id="txtTestSystemEmailSubject" runat="server" />
                                    </label>
                                </div>
                                <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.125rem;">
                                    <label>
                                        Email Body Template
                                    <input type="text" id="txtTestSystemEmailBodyTemplate" runat="server" />
                                    </label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="large-12 columns">
                                    <asp:Button runat="server" CssClass="button tiny radius" ID="btnUpdateTestSystemConfig" Width="100px" Text="Update" OnClick="btnUpdateTestSystemConfig_Click" />
                                    <asp:Button runat="server" CssClass="button tiny radius" ID="btnSystemRefresh" Width="100px" Text="System Refresh" OnClick="btnSystemRefresh_Click" />
                                </div>
                            </div>
                            <div class="row">
                                <div class="large-12 columns" style="margin-bottom: 1rem;">
                                    <asp:Label runat="server" ID="lbError" ForeColor="red" />
                                </div>
                            </div>
                        </div>
                    </dd>
                    <dd class="accordion-navigation">
                        <a id="SystemUnderTestConfigurationTab" runat="server" href="#panel2">System Under Test Configurations</a>
                        <div id="panel2" class="content">
                            <div class="row">
                                <div class="large-4 medium-4 small-12 columns">
                                    <label>
                                        Systems Under Test
                                   <asp:DropDownList ID="dlSystemsUnderTest" runat="server" AutoPostBack="True" OnSelectedIndexChanged="dlSystemsUnderTest_Changed">
                                       <asp:ListItem Value="None">None</asp:ListItem>
                                   </asp:DropDownList>

                                    </label>
                                </div>
                                <div class="large-8 medium-8 small-12 columns">
                                    <label>&nbsp;</label>
                                    <asp:Button runat="server" ID="btnSystemUnderTestSaveUpdate" Text="New" OnClick="btnSystemUnderTestSaveUpdate_Click" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:Button runat="server" ID="btnSystemUnderTestDelete" Text="Delete System Under Test" ForeColor="Red" OnClick="btnSystemUnderTestDelete_Click" />
                                </div>
                            </div>
                            <div class="hide-for-small" style="padding: 0.5rem;"></div>
                            <div class="panel" id="systemsDisplay" runat="server">
                                <div class="row">
                                    <div class="large-4 medium-4 small-12 columns">
                                        <label>
                                            System Under Test Name
                                        <input type="text" id="txtSystemUnderTestName" runat="server" />
                                        </label>
                                    </div>
                                    <div class="large-4 medium-4 small-12 columns">
                                        <label>
                                            Contact List

                                    <asp:DropDownList ID="dlSystemUnderTestContactList" runat="server" AutoPostBack="True" OnSelectedIndexChanged="dlSystemUnderTestContact_Changed">
                                        <asp:ListItem Value="one">None</asp:ListItem>
                                    </asp:DropDownList>
                                        </label>
                                    </div>
                                    <div class="large-4 medium-4 small-12 columns">
                                        <label>Selected Contact</label>
                                        <asp:Button runat="server" ID="btnAddContact" Text="Add" OnClick="btnAddUpdateContact_Click" />
                                        <asp:Button runat="server" ID="btnUpdateContact" Text="Update" OnClick="btnAddUpdateContact_Click" />
                                        &nbsp;&nbsp;&nbsp;&nbsp;
                                    <asp:Button runat="server" ID="btnDeleteContact" Text="Delete" ForeColor="Red" OnClick="btnDeleteContact_Click" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="large-2 medium-2 small-12 columns">
                                        <label>First Name</label>
                                        <input type="text" id="txtContactFirstName" runat="server" />
                                    </div>
                                    <div class="large-3 medium-3 small-12 columns">
                                        <label>Last Name</label>
                                        <input type="text" id="txtContactLastName" runat="server" />
                                    </div>
                                    <div class="large-2 medium-2 small-12 columns">
                                        <label>Mobile Phone</label>
                                        <input type="text" id="txtContactMobilePhone" runat="server" />
                                    </div>
                                    <div class="large-3 medium-3 small-12 columns">
                                        <label>
                                            Email address
                                    <input type="text" id="txtContactEmailaddress" runat="server" />
                                        </label>
                                    </div>
                                    <div class="large-2 medium-2 small-12 columns">
                                        <label>Contact Method</label>
                                        <asp:CheckBox runat="server" ID="cbContactSMS" Text="Text" />
                                        <asp:CheckBox runat="server" ID="cbContactEmail" Text="Email" />
                                    </div>
                                </div>
                                <div style="padding: 0.125rem;"></div>
                                <div class="row">
                                    <div class="large-12 columns">
                                        <asp:Label runat="server" ID="lbErrSUT" ForeColor="Red" />
                                    </div>
                                </div>
                            </div>
                            <div class="row" id="divConfigDotsRow" runat="server">
                                <div class="large-12 columns">
                                    <div style="text-align: center; color: #d8d8d8;">
                                        <%--<hr style="margin:0 auto;width:90%;" />--%>
                                    &#9899;&nbsp;&#9899;&nbsp;&#9899;
                                    </div>
                                </div>
                            </div>
                            <div class="row" id="divConfigTestsRow" runat="server">
                                <div class="large-6 medium-6 small-12 columns">
                                    <label>
                                        Test Names
                                    <asp:DropDownList ID="dlTests" runat="server" AutoPostBack="True" OnSelectedIndexChanged="dlTests_Changed">
                                        <asp:ListItem Value="one">None</asp:ListItem>
                                    </asp:DropDownList>

                                    </label>
                                </div>
                                <div class="large-6 medium-6 small-12 columns">
                                    <label>&nbsp;</label>
                                    <asp:Button runat="server" ID="btnSaveUpdateTest" Text="Save" OnClick="btnSaveUpdateTest_Click" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:Button runat="server" ID="btnDeleteTest" Text="Delete Test" ForeColor="Red" ToolTip="Delete selected Test Configuration" OnClick="btnDeleteTest_Click" />
                                </div>
                            </div>
                            <div class="hide-for-small" style="padding: 0.5rem;"></div>
                            <div class="panel" id="configurationsDisplay" runat="server">
                                <div class="row">
                                    <div class="large-4 medium-4 small-12 columns">
                                        <label>
                                            Test Name
                                        <input type="text" id="txtTestName" runat="server"
                                            title="Test Name must be unique." />
                                        </label>
                                    </div>
                                    <div class="large-4 medium-4 small-12 columns">
                                        <label>
                                            Test Script (Text File)
                                        <input type="text" id="txtTestScript" runat="server"
                                            title="File name of Test Script to run." />
                                        </label>
                                    </div>
                                    <div class="large-4 medium-4 small-12 columns hidden-for-small">
                                        &nbsp;
                                    </div>
                                </div>
                                <div class="panel" style="background: #e2e2e2 !important;">
                                    <div class="row">
                                        <div class="large-2 medium-2 small-12 columns">
                                            <label>
                                                Variable Name
                                            <input type="text" id="txtTestVariableName" runat="server" />
                                            </label>
                                        </div>
                                        <div class="large-6 medium-6 small-12 columns">
                                            <label>
                                                Variable Value
                                            <input type="text" id="txtTestVariableValue" runat="server" />
                                            </label>
                                        </div>
                                        <div class="large-4 medium-4 small-12 columns">
                                            <label>&nbsp;</label>
                                            <asp:Button runat="server" ID="btnVariableAdd" Text="Add" ToolTip="Add New Variable" OnClick="btnVariableAdd_Click" />
                                            &nbsp;&nbsp;&nbsp;&nbsp;
                                        <asp:Button runat="server" ID="btnVariableDelete" Text="Delete" ForeColor="Red" ToolTip="Delete Selected Variable" OnClick="btnVariableDelete_Click" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="large-12 columns">
                                            <asp:Label runat="server" ID="lbErrorConfigurationVariable" ForeColor="Red" />
                                            <asp:TextBox ID="txtTestVariableList" TextMode="multiline" runat="server" Rows="5" />
                                        </div>
                                    </div>
                                    <div id="TestVariableList" style="display: none;">
                                        <asp:DropDownList runat="server" ID="dlTextVariableList" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="large-12 columns">
                                        <asp:Label runat="server" ID="lbErrorTestConfiguration" ForeColor="Red" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </dd>
                    <dd class="accordion-navigation">
                        <a id="RegressionTestTab" runat="server" href="#panel3">Regression Tests</a>
                        <div id="panel3" class="content">
                            <div class="row">
                                <h1>Test Selection</h1>
                                <div class="large-4 medium-4 small-12 columns" style="">
                                    <select id="dlTargetServers" runat="server" class="" onchange="javascript: GetTestUsers();"></select>
                                </div>
                                <div class="large-4 medium-4 small-12 columns" style="">
<%--                                    <div id="divAdminPwd" style="">
                                        <div style="float: left; font-size: 12px; margin-right: 10px; position: relative; top: 10px;">
                                            Select a Login User
                                        </div>
                                        <div style="float: left;">
                                            <input id="txtAdminPwd" runat="server" value="" />
                                        </div>
                                    </div>--%>
                                    <select id="dlTestUsers" runat="server" class="" onchange=""></select>
                                </div>
                                <div class="large-4 medium-4 small-12 columns" style="text-align: right;">
                                    <span style="font-weight: bold; margin-right: 5px;">Run Id:</span><span id="txtRunId" style="margin-right: 15px;" runat="server"></span>
                                </div>
                            </div>

                            <div id="divTestSelectionContainer" class="row" style="border: solid 0px #c0c0c0; padding: 0px; margin-bottom: 0px;">
                                <div class="large-3 medium-3 small-12 columns" style="margin-bottom: 0.25rem;">
                                    <label>Select an Application to Test</label>
                                    <div id="divApplicationsToTest" runat="server" style="border: solid 1px #c0c0c0; width: 100%; height: 175px; overflow: auto;">
                                        Applications List
                                    </div>
                                </div>
                                <div class="large-3 medium-3 small-12 columns" style="margin-bottom: 0.25rem;">
                                    <label id="lblApplicationAreasLabel">Select from (<span id='spanApplicationAreaCount'>0</span>) Application Areas</label>
                                    <div id="ApplicationAreaList" runat="server" style="border: solid 1px #c0c0c0; width: 100%; height: 175px; overflow: auto;">
                                    </div>
                                </div>
                                <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.25rem;">
                                    <label id="lblTestCasesLabel">Select from (<span id='spanTestCasesCount'>0</span>) Test Cases</label>
                                    <div id="TestCaseList" runat="server" style="border: solid 1px #c0c0c0; padding-top: 5px; width: 100%; height: 175px; overflow: auto;">
                                    </div>
                                </div>
                            </div>

                            <div class="row" style="border-bottom: solid 1px #c0c0c0; margin-top: 15px; padding-bottom: 15px; padding-right: 0px;">
                                <div id="divRunControls" style="text-align: center;">
                                    <span style="font-size: 12px; margin-right: 15px;">
                                        <span>
                                            <input id="chkStopOnError" type="checkbox" />
                                        </span>
                                        <span>
                                            Stop on Error
                                        </span>
                                    </span>
                                    <span>
                                        <input id="btnRunTests" runat="server" type="button" class="tiny button radius" style="font-size: 12px; font-weight: bold;" onclick="javascript: RunTests();" value="Run Tests" />
                                    </span>
                                </div>
                                <div id="divProcessingStatus" runat="server" style="border: solid 0px #0000ff; width: 100%; padding-left: 150px; display: none; text-align: center; font-size: 12px;">
                                    <div  id="divProcessingStatusDetails" runat="server" style="border: solid 0px #ff0000; width: 500px; text-align: left; margin: 0 auto;">
                                        Processing status...
                                    </div>
                                </div>
                            </div>

                            <div id="divTestResultHistory" class="row" style="padding-top: 15px;">
                                
                                <div class="large-6 medium-6 small-12 columns" style="">
                                    <select id="ddlTestEnvironments" onchange="javascript: GetPreviousTestRunsList();"></select>
                                </div>
                                <div class="large-6 medium-6 small-12 columns" style="position: relative; top: 0px;">
                                    <select id="ddlTestRuns" onchange="javascript: GetPreviousResultsDetails();"></select>
                                </div>

                            </div>

                            <div style="padding: 0.5rem;"></div>
                            
                            
                            <asp:Label runat="server" ID="lbRTError" ForeColor="Red" />
                            <div class="row" style="padding: 10px;">
                                <h1>Test Results (<a href="javascript: ClearLogView();" style="text-decoration: underline;">Clear Results</a>)</h1>
                                <div id="divTestResults" runat="server" class="large-12 columns" style="padding: 0; border: solid 1px #c0c0c0; height: 310px; overflow: auto;">
                                </div>
                            </div>
                        </div>
                    </dd>
                </dl>
            </div>

            <div style="padding: 1.5rem;">&nbsp;</div>

        </div>
        <input id="hiddenRegressionTestEnabled" runat="server" type="hidden" value="false" />
        <input id="hiddenActiveAccordionTab" runat="server" type="hidden" value="" />
    </form>

    <script src="foundation/js/vendor/jquery.js"></script>
    <script src="foundation/js/foundation.min.js"></script>
    <script src="foundation/js/foundation/foundation.accordion.js"></script>

    <script>
        $(document).foundation();
    </script>

    <script src="chosen/chosen.jquery.js"></script>
    <script src="chosen/docsupport/prism.js"></script>

    <script type="text/javascript">
        $(".chosen-select").chosen();
        $(document).trigger("chosen:updated");
        //$('.chosen-select').chosen({ disable_search_threshold: 28 });
        //var config = {
        //    '.chosen-select': {},
        //    '.chosen-select-deselect': { allow_single_deselect: true },
        //    '.chosen-select-no-single': { disable_search_threshold: 10 },
        //    '.chosen-select-no-results': { no_results_text: 'Oops, nothing found!' },
        //    '.chosen-select-width': { width: "95%" }
        //}
        //for (var selector in config) {
        //    $(selector).chosen(config[selector]);
        //}
    </script>

</body>
</html>
