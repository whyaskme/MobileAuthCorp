<%@ Page Title="MAC Otp System Administration"
    Language="C#"
    ValidateRequest="false" 
    AutoEventWireup="true" 
    CodeFile="EndUser-Register.aspx.cs"
    Inherits="Admin.Tests.AWS.EndUserRegister" %>


<head xmlns="http://www.w3.org/1999/xhtml" runat="server">  
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <title>Automated End-User Registration, Redirect to Request OTP</title>

    <link href="/App_Themes/CSS/Admin.css" rel="stylesheet" />
    <link href="/App_Themes/CSS/style.css" rel="stylesheet" />
    <link href="/App_Themes/CSS/prism.css" rel="stylesheet" />
    <link href="/App_Themes/CSS/chosen.css" rel="stylesheet" />
    <link href="/App_Themes/CSS/foundation_menu.css" rel="stylesheet" />

    <script type="text/javascript" src="/Javascript/Constants.js"></script>
    <script type="text/javascript" src="/Javascript/MACSystemAdmin-Responsive.js"></script>
    <script type="text/javascript" src="/Javascript/jquery-1.10.2.js"></script>
    
    <script type="text/javascript">
        function submitRegistration() {

            var formSubmitted = document.getElementById("hiddenFormSubmitted").value;
            //alert("submitRegistration");

            if (formSubmitted == "false") {
                document.getElementById("btnRegisterEndUser").click();
                formSubmitted = "true";
            }
        }
    </script>
</head>
    
<body onload="submitRegistration();">
    <form id="formMain" runat="server" method="post" onsubmit="">
    <input id="hiddenFormSubmitted" type="hidden" runat="server" value="false" />
    <input id="hiddenE" runat="server" type="hidden" value="" />
    <input id="hiddenlastname" runat="server" type="hidden" value="" />
    <input id="hiddenCID" runat="server" type="hidden" value="" />
    <input id="hiddenB" runat="server" type="hidden" value="" />
    <div style="padding: 0.25rem;"></div>      
    <div class="row">
        <div class="large-6 medium-6 small-12 columns">
            <asp:DropDownList CssClass="chosen-select" ID="ddlClient" runat="server" AutoPostBack="true" />
        </div>
        <div class="large-6 medium-6 small-12 columns">
            <asp:DropDownList CssClass="chosen-select" ID="ddlGroups" runat="server"  />
        </div>
    </div>        
    <div class="row">
        <hr />
    </div>        
    <div style="padding: 0.25rem;"></div>
    <div class="row" style="text-align: center;">
        <span style="font-style: normal; font-size: 14px; margin-right: 15px;">Register End User as:</span>
        <asp:RadioButton id="rbClientRestricted" Text="&nbsp;Restricted to Client" Checked="True" GroupName="type" runat="server" />
        <asp:RadioButton id="rbGroupRestricted" Text="&nbsp;Restricted to Group" GroupName="type" runat="server" />
        <asp:RadioButton id="rbOpen" Text="&nbsp;Open (any client)" GroupName="type" runat="server" />
    </div>
    <div style="padding: 0.5rem;"></div>
    <div class="row">
        <div class="large-6 medium-6 small-12 columns">
            <label>First Name</label>
            <asp:TextBox ID="txtFirstName" runat="server" />
        </div>
        <div class="large-6 medium-6 small-12 columns">
            <label>Last Name</label>
            <asp:TextBox ID="txtLastName" runat="server" />
        </div>
    </div>
    <div class="row">
        <div class="large-6 medium-6 small-12 columns">
            <label>Mobile Phone</label>
            <asp:TextBox ID="txtMPhoneNo" runat="server" />
        </div>
        <div class="large-6 medium-6 small-12 columns">
            <label>Email</label>
            <asp:TextBox ID="txtEmailAdr" runat="server" />
        </div>
    </div>
    <div class="row" style="text-align: center;">
        <hr />
        <asp:Button CssClass="button tiny radius" ID="btnRegisterEndUser" runat="server" Text="Register" OnClick="btnRegisterEndUser_Click" tooltip="Click to register the selected end user."/>
        <asp:Button CssClass="button tiny radius" runat="server" ID="btnClearLog" Text="Clear the log window" OnClick="btnClearLog_Click"/>
        <asp:Button CssClass="button tiny radius" runat="server" ID="btnSendOTP" Text="SendOTP" OnClick="btnSendOTP_Click"/>
    </div>        
    <div class="row">
        <div class="large-12 columns">
            <div class="alert-box alert radius" style="cursor: pointer; display: none;" title="Close" onclick="javascript: $(this).hide();">
                <asp:Label ID="lbError" runat="server" Text="" />
            </div>
        </div>
    </div>        
    <div class="row">
        <div id="tbLog" runat="server" class="large-12 columns"></div>
    </div>        
    <div class="row">
        <div class="large-12 columns">

        </div>
    </div>       
    </form>

    <script src="../../../Javascript/foundation.min.js"></script>
    <script src="../../../Javascript/chosen.jquery.js" type="text/javascript"></script>
    <script src="../../../Javascript/prism.js" type="text/javascript" charset="utf-8"></script>
    <script type="text/javascript">
        $('.chosen-select').chosen({ disable_search_threshold: 10 });
        var config = {
            '.chosen-select': {},
            '.chosen-select-deselect': { allow_single_deselect: true },
            '.chosen-select-no-single': { disable_search_threshold: 10 },
            '.chosen-select-no-results': { no_results_text: 'Oops, nothing found!' },
            '.chosen-select-width': { width: "95%" }
        }
        for (var selector in config) {
            $(selector).chosen(config[selector]);
        }
    </script>
</body>