<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Menu-Test.ascx.cs" Inherits="UserControls_Menu_Test" %>

<script>
    $(document).ready(function () {

        //var x = $("#OpenRegisterClient").attr("ID");
        //alert(x);

        var testList = document.getElementById("testList");

        $(".chosen-select").chosen({ disable_search_threshold: 100 });
        //alert($(window).innerHeight());

        //$('#menuTests').addClass('active');
        $('#panelFocusUsers').val() == "";

        // Set selected listitem by quierystring if present
        if (window.location.toString().indexOf("?i=") > -1)
        {
            var tmpVal = window.location.toString().split('=');
            var selectedIndex = parseInt(tmpVal[1]);
            testList.selectedIndex = selectedIndex;
            $("#testList").trigger("chosen:updated");
        }

        $(function pageRedirect() {
            //ShowProcessingMessage();
            // bind change event to select
            $('#testList').on('change', function () {
                var url = $(this).val(); // get selected value
                if (url.indexOf("/AWS/") > -1) {
                    window.open(url);
                }
                else if (url)
                {
                    var myForm = document.getElementById("formMain");
                    myForm.action = url; // redirect
                    myForm.submit();
                }

                return false;
            });
        });

        var defaultMessage = $('#testList_chosen a span').html();
        if (defaultMessage == 'Select an Option') {
            $('#testList_chosen a span').html('Test Menu');
        }
        //alert();
    });
</script>

<select name="testSelect" id="testList" class="chosen-select" onchange="javascript: pageRedirect();">
    <option value="Please Choose"></option>     
    <!-- Test numbering shall start at 1 and add 1 for each page after that_-->
    <optgroup label="Admin Tests">
        <option id="OpenRegisterClient" value="/Admin/Tests/RegisterOasClient/RegisterOasClient.aspx?i=1">Open Register Client</option>
        <option id="EncryptEncode" value="/Admin/Tests/Encrypt/Encrypt.aspx?i=2">AES Encrypt/Decrypt &amp; Hex Encode/Decode</option>
        <option id="EndUser" value="/Admin/Tests/EndUserTests/EndUserTests.aspx?i=3">End User</option>
        <option id="EventLog" value="/Admin/Tests/EventLogTests/EventLogTests.aspx?i=4">Event Log</option>
        <option id="ManageTypeDefinitions" value="/Admin/Tests/TypeDefs/ManageTypeDefs.aspx?i=5">Manage Type Definitions</option>
        <option id="ClientServiceTests" value="/Admin/Tests/Client/ClientTests.aspx?i=6">Client Service Tests</option>
    </optgroup>
    <optgroup label="Amazon Web Services">
        <option id="AWSHealthCheck" value="/Admin/Tests/AWS/AWS-HealthCheck.aspx?i=7">AWS Health Check</option>
        <option id="RegistrationOTPRequestValidate" value="/Admin/Tests/AWS/EndUser-Register.aspx?i=8&autorequest=true">Combined - Registration/Request/Validate</option>
        <option id="EndUserRegistration" value="/Admin/Tests/AWS/EndUser-Register.aspx?i=9">End User Registration</option>
        <option id="OTPRequestValidate" value="/Admin/Tests/AWS/OTP-Request.aspx?i=10">OTP Request/Validate</option>
    </optgroup>
    <optgroup label="API Tests">
        <option id="AdPassTests" value="/Admin/Tests/AdTests/AdTest.aspx?i=11">AdPass Tests</option>
        <option id="MessageBroadcast" value="/Admin/Tests/MessageBroadcastTests/MBTests.aspx?i=12">Message Broadcast</option>
        <option id="IssueRequest" value="/Admin/Tests/IssueRequest/IssueRequest.aspx?i=13">Issue Request</option>
    </optgroup>
    <optgroup label="Communications">
        <option value="/Admin/Tests/EmailTemplates/Default.aspx?i=14">Email Templates</option>
    </optgroup>
    <optgroup label="End User Registration">
        <option value="/Admin/Tests/EndUserRegistration/Register.aspx?i=15">UI Registration (email completion)</option>
        <option value="/Admin/Tests/EndUserRegistration/RegisterUsersInFile.aspx?i=16">File Registration (uploads file)</option>
        <option value="/Admin/Tests/SecureTradingService/STSRegister.aspx?i=17">Registration (Pre-verification.. STS)</option>
    </optgroup>
    <optgroup label="End User">
        <option value="/Admin/Tests/Authentication/Auth.aspx?i=18">Login Authentication</option>
        <option value="/Admin/Tests/TrxVerification/TrxVerification.aspx?i=19">Transaction Verification & Authorization</option>
        <option value="/Admin/Tests/SendMessage/SendMessage.aspx?i=20">Send Message</option>
        <option value="/Admin/Tests/EndUserManagement/EndUserManagement.aspx?i=21">End User Management</option>
    </optgroup>
    <optgroup label="MAC Test Bank">
        <option value="/Admin/Tests/MACTestBank/AssignAccount.aspx?i=22">Account Management</option>
        <option value="/Admin/Tests/MACTestBank/MACTestBank.aspx?i=23">Bank Management</option>
        <option value="/Admin/Tests/MACTestBank/BillPay.aspx?i=24">Billing, Payments & Purchase</option>
        <option value="/Admin/Tests/MACTestBank/IssueCards.aspx?i=25">Issue Cards</option>
    </optgroup>
</select>