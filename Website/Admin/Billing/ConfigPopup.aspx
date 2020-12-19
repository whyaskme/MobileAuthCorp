<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ConfigPopup.aspx.cs" Inherits="MACBilling.ConfigPopup" ValidateRequest="false" %>

<!DOCTYPE html>

<html lang="en">
<head runat="server">
    <title>Billing Configuration</title>

    <link href="../../App_Themes/CSS/jquery-ui-smoothness.css" rel="stylesheet" />
    
    <script src="../../JavaScript/jquery-1.10.2.js"></script>
    <script src="../../JavaScript/jquery-ui-1-11-0.js"></script>
    <script src="../../JavaScript/jquery.timer.js"></script>
    <script src="../../JavaScript/jquery.validate.js"></script>

    <link href="../../App_Themes/CSS/table-style.css" rel="stylesheet" />

    <link rel="stylesheet" href="../../App_Themes/CSS/style.css" />
    <link rel="stylesheet" href="../../App_Themes/CSS/prism.css" />
    <link rel="stylesheet" href="../../App_Themes/CSS/chosen.css" />
    <link rel="stylesheet" href="../../App_Themes/CSS/foundation_menu.css" />

    <link href="../../App_Themes/CSS/Admin.css" rel="stylesheet" />
    <link href="../../App_Themes/CSS/Billing.css" rel="stylesheet" />

    <script type="text/javascript" src="../../Javascript/Constants.js"></script>
    <script type="text/javascript" src="../../Javascript/MACSystemAdmin-Responsive.js"></script>
    <script type="text/javascript" src="../../Javascript/jquery.cookie.js"></script>

    <script type="text/javascript">

        function pageLoad()
        {
            if (window.location.toString().indexOf("focus=Pricing") > -1)
            {
                $('#configTab5').click(function () {
                    $('#panelFocusConfig').val('configTab5');
                });

                $('#configTab5').click();
            }
            else
            {
                $('#configTab1').click(function () {
                    $('#panelFocusConfig').val('configTab1');
                });

                $('#configTab1').click();
            }

            setPaymentMethod();

            $('#divGroupSelection').hide();

            var groupBilling  = $('#chkIncludeInGroupBill').is(':checked');

            //alert("groupBilling - " + groupBilling);

            if(groupBilling == true)
            {
                $('#divGroupSelection').show();

                $('#configTab1,#configTab2,#configTab3,#configTab4,#configTab5,#configTab6').prop('disabled', true);
                $('#configTab1,#configTab2,#configTab3,#configTab4,#configTab5,#configTab6,#txtExternallySettledBy,#txtMinimumOtpCharge,#txtMinimumAdCharge,#txtMinimumEndUserRegistrationCharge,#textMonthlyServiceCharge,#txtTaxRate').css('background', '#e3e3e3');
                $('#configTab1,#configTab2,#configTab3,#configTab4,#configTab5,#configTab6,#txtExternallySettledBy,#txtMinimumOtpCharge,#txtMinimumAdCharge,#txtMinimumEndUserRegistrationCharge,#textMonthlyServiceCharge,#txtTaxRate').css('border-color', '#e3e3e3');
                $('#configTab1,#configTab2,#configTab3,#configTab4,#configTab5,#configTab6,#txtExternallySettledBy,#txtMinimumOtpCharge,#txtMinimumAdCharge,#txtMinimumEndUserRegistrationCharge,#textMonthlyServiceCharge,#txtTaxRate').css('color', '#999');

                $('#dlBillingCycle,#dlBillingTerms').prop('disabled', true);
                $("#dlBillingCycle,#dlBillingTerms").trigger("chosen:updated");

                // Labels
                $('#spanExternallySettledBy,#spanBillingCycle,#spanPaymentTerms,#spanMinimumOtpCharge,#spanMinimumAdCharge,#spanMinimumEndUserRegistrationCharge,#spanMonthlyServiceCharge,#spanTaxRate').css('color', '#999');

                $('#configTab2,#configTab3,#configTab4,#configTab5,#configTab6').click(function ()
                {
                    alert('This has been disabled due to Group billing assignment');
                });

                $('#divSettingDisabledText').show();
            }
            else
            {
                $('#divSettingDisabledText').hide();
            }

            setAdBillingType();
        }

        function UpdateBillConfig()
        {
            //alert("UpdateBillConfig");
            document.getElementById("form1").submit();
        }

        function setAdBillingType()
        {
            var adBillingSingleSelected = document.getElementById("radioAdSimpleCharge").checked;

            if (adBillingSingleSelected)
            {
                document.getElementById("divAdSentOtpScreen").style.display = "none";
                document.getElementById("divAdSentVerifyOtpScreen").style.display = "none";
            }
            else
            {
                document.getElementById("divAdSentOtpScreen").style.display = "block";
                document.getElementById("divAdSentVerifyOtpScreen").style.display = "block";
            }
        }

        function callParentDocumentFunction() {
            window.parent.parent.hideJQueryDialog();
            var parentForm = window.parent.parent.document.getElementById("formMain");
            parentForm.submit();
        }

        function cancelChanges() {
            window.parent.parent.hideJQueryDialog();
        }

        function GetGroupMembership(IsChecked)
        {
            if(IsChecked)
            {
                $('#divGroupSelection').show();
                //$('#dlGroupList').prop('disabled', false);
            }
            else
            {
                $('#divGroupSelection').hide();
                //$('#dlGroupList').prop('disabled', true);
            }
        }

        function setPaymentMethod()
        {
            $('#divACHPayment').hide();
            $('#divCreditCardPayment').hide();
            $('#divManualCheck').hide();
            $('#divWireTransfer').hide();

            var dlPaymentMethod = document.getElementById("dlPaymentMethod");
            var selectedPaymentMethod = dlPaymentMethod.options[dlPaymentMethod.selectedIndex].value;

            switch (selectedPaymentMethod)
            {
                case "ACH":
                    $('#divACHPayment').show();
                    break;
                case "Credit Card":
                    $('#divCreditCardPayment').show();
                    break;
                case "Manual Check":
                    $('#divManualCheck').show();
                    break;
                case "Wire Transfer":
                    $('#divWireTransfer').show();
                    break;
            }
        }

    </script>

</head>
    <body style="overflow:auto !important;" onload="javascript: pageLoad();">

    <form id="form1" runat="server">

        <div class="row">
            <div class="large-12 medium-12 small-12 columns">
                <h3 style="font-size:1rem;font-weight:bold;margin-top:0.5rem; position: relative; top: 0px; margin-bottom: 15px; border: none;" id="spanBillingConfig" runat="server">Billing Config</h3>
            </div>
        </div>

        <div class="row" id="divConfigWarning_1" runat="server" style="margin-bottom: 15px;">
            <div id="divConfigWarningText_1" runat="server" class="large-12 medium-12 small-12 columns" style="width: 100%; text-align: center; color: #ff0000;">
                !!! Default (Client) configuration !!!
            </div>
        </div>

        <div class="row" id="scroll2">
            <div class="large-12 columns">
                <dl class="accordion" data-accordion>

                    <!-- General Settings -->
                    <dd>
                        <a id="configTab1" runat="server" href="#panel1"><span id="spanBilling" runat="server">General Settings</span></a>
                        <div id="panel1" class="content">

                            <div class="row" style="margin-bottom: 0.75rem; position: relative; top: -40px; margin-bottom: -40px;">
                                <div class="large-12 medium-12 small-12 columns" style="width: 100%; text-align: right;">
                                    <a href="javascript: NavigateTopicPopup('54985ae2ead6361ac8883c59');" id="link_help_generalSettings">Help?</a>
                                </div>
                            </div>

                            <asp:Panel ID="panelGeneralSettings" runat="server">

                                <div id="divSettingDisabledText" runat="server" class="row" style="margin-bottom: 0.75rem; position: relative; top: -10px; margin-bottom: 0px;">
                                    <div class="large-12 medium-12 small-12 columns" style="width: 100%; color: #ff0000; text-align: center;">
                                        Settings disabled by Group billing configuration
                                    </div>
                                </div>

                                <div class="row" style="margin-bottom: 0.75rem;">
                                    <div class="large-12 medium-12 small-12 columns">
                                        <label id="spanBillingCycle">Billing Cycle
                                            <asp:DropDownList ID="dlBillingCycle" runat="server" CssClass="chosen-select">
                                                <asp:ListItem Value="Weekly">Weekly</asp:ListItem>
                                                <asp:ListItem Selected="True" Value="Monthly">Monthly</asp:ListItem>
                                                <asp:ListItem Value="Quarterly">Quarterly</asp:ListItem>
                                                <asp:ListItem Value="Annually">Annually</asp:ListItem>
                                            </asp:DropDownList>
                                        </label>
                                    </div>
                                </div>

                                <div class="row" style="margin-bottom: 0.75rem;">
                                    <div class="large-12 medium-12 small-12 columns">
                                        <label id="spanPaymentTerms">Payment Terms
                                            <asp:DropDownList ID="dlBillingTerms" runat="server" CssClass="chosen-select">
                                                <asp:ListItem Value="DueUponReceipt">Due Upon Receipt</asp:ListItem>
                                                <asp:ListItem Value="Net10">Net 10</asp:ListItem>
                                                <asp:ListItem Value="Net15">Net 15</asp:ListItem>
                                                <asp:ListItem Value="Net30" Selected="True">Net 30</asp:ListItem>
                                                <asp:ListItem Value="2% 15, net 30">2% 15 - Net 30</asp:ListItem>
                                                <asp:ListItem Value="4% 7, net 30">4% 7 - Net 30</asp:ListItem>
                                            </asp:DropDownList>
                                        </label>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="large-12 medium-12 small-12 columns">
                                        <label id="spanExternallySettledBy">Externally Settled By
                                            <asp:TextBox ID="txtExternallySettledBy" runat="server"></asp:TextBox>
                                        </label>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="large-12 medium-12 small-12 columns">
                                        <label id="spanMonthlyServiceCharge">Monthly Service Charge
                                            <asp:TextBox ID="textMonthlyServiceCharge" runat="server"></asp:TextBox>
                                        </label>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="large-12 medium-12 small-12 columns">
                                        <label id="spanMinimumOtpCharge">Minimum OTP Charge
                                            <asp:TextBox ID="txtMinimumOtpCharge" runat="server"></asp:TextBox>
                                        </label>
                                    </div>
                                </div>

                                <div id="divMinimumAdCharge" runat="server" class="row">
                                    <div class="large-12 medium-12 small-12 columns">
                                        <label><span id="spanMinimumAdCharge" runat="server">Minimum Ad Charge</span>
                                            <asp:TextBox ID="txtMinimumAdCharge" runat="server"></asp:TextBox>
                                        </label>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="large-12 medium-12 small-12 columns">
                                        <label id="spanMinimumEndUserRegistrationCharge">Minimum End-User Registration Charge
                                            <asp:TextBox ID="txtMinimumEndUserRegistrationCharge" runat="server"></asp:TextBox>
                                        </label>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="large-12 medium-12 small-12 columns">
                                        <label id="spanTaxRate">Tax Rate
                                            <asp:TextBox ID="txtTaxRate" runat="server"></asp:TextBox>
                                        </label>
                                    </div>
                                </div>

                            </asp:Panel>

                            <div class="row" id="divIncludeInGroupBill" runat="server">
                                <div class="large-12 medium-12 small-12 columns" style="width: 100%; text-align: center; padding-top: 15px;">
                                    <label>
                                        <asp:CheckBox ID="chkIncludeInGroupBill" runat="server" Enabled="false" />
                                        <span id="spanIncludeInBillGroup" runat="server" style="color: #c0c0c0; position: relative; top: -2px; margin-left: 3px;">Include in Group Billing?</span>
                                    </label>
                                    <div id="divGroupSelection" class="large-12 medium-12 small-12 columns" runat="server" style="border: solid 0px #c0c0c0; width: 100%; text-align: center;">
                                        <div style="width: 100%; text-align: left; position: relative; top: 0px; margin-bottom: 0px;">Select only 1 Group to handle billing for this Client</div>
                                        <div style="width: 100%; text-align: center; position: relative; top: 0px; margin-bottom: 0px;">
                                            <asp:ListBox ID="dlGroupList" Height="150" SelectionMode="Multiple" runat="server"></asp:ListBox>
                                        </div>
                                    </div>
                                </div>

                            </div>

                        </div>
                    </dd>


                    <!-- Notification Users -->
                    <dd>
                        <a id="configTab2" runat="server" href="#panel2"><span id="spanBillUsers" runat="server">Notify Users</span></a>
                        <div id="panel2" class="content">

                            <div class="row" style="margin-bottom: 0.75rem; position: relative; top: -40px; margin-bottom: -40px;">
                                <div class="large-12 medium-12 small-12 columns" style="width: 100%; text-align: right;">
                                    <a href="javascript: NavigateTopicPopup('54cab7bab5655a41e03afc3b');" id="link_help_notifyUsers">Help?</a>
                                </div>
                            </div>

                            <div class="row">

                                <div class="large-12 medium-12 small-12 columns" style="width: 100%;">
                                    <iframe src='/Admin/Billing/UserPopupAssignment.aspx' id='billingAssignmentsFrame' name='assignmentContainer' scrolling='no' runat="server" style='display: block; background-color: transparent; width: 100%; height: 485px; overflow-y: auto;'></iframe>
                                </div>

                            </div>

                        </div>
                    </dd>


                    <!-- Organization Information -->
                    <dd>
                        <a id="configTab3" runat="server" href="#panel3"><span id="span2" runat="server">Organization Information</span></a>
                        <div id="panel3" class="content">

                            <div class="row" style="margin-bottom: 0.75rem; position: relative; top: -40px; margin-bottom: -40px;">
                                <div class="large-12 medium-12 small-12 columns" style="width: 100%; text-align: right;">
                                    <a href="javascript: NavigateTopicPopup('54985ae2ead6361ac8883c59');" id="link_help_organizationInformation">Help?</a>
                                </div>
                            </div>

                            <div class="row">

                                <div class="large-12 medium-12 small-12 columns">
                                    <label>Name (Cannot be changed here)
                                        <asp:TextBox ID="txtOrgName" Enabled="false" runat="server"></asp:TextBox>
                                    </label>
                                </div>

                                <div class="large-12 medium-12 small-12 columns">
                                    <label>TaxId
                                        <asp:TextBox ID="txtOrgTaxId" runat="server"></asp:TextBox>
                                    </label>
                                </div>

                                <div class="large-12 medium-12 small-12 columns">
                                    <label>Street 1
                                        <asp:TextBox ID="txtOrgStreet1" runat="server"></asp:TextBox>
                                    </label>
                                </div>

                                <div class="large-12 medium-12 small-12 columns">
                                    <label>Street 2
                                        <asp:TextBox ID="txtOrgStreet2" runat="server"></asp:TextBox>
                                    </label>
                                </div>

                                <div class="large-12 medium-12 small-12 columns">
                                    <label>City
                                        <asp:TextBox ID="txtOrgCity" runat="server"></asp:TextBox>
                                    </label>
                                </div>

                                <div class="large-12 medium-12 small-12 columns" style="margin-bottom: 15px;">
                                    <label>State
                                        <asp:DropDownList ID="dlOrgStates" runat="server" Visible="true" CssClass="chosen-select">
                                            <asp:ListItem Value="000000000000000000000000">Select State</asp:ListItem>
                                        </asp:DropDownList>
                                    </label>
                                </div>

                                <div class="large-12 medium-12 small-12 columns">
                                    <label>Zip
                                        <asp:TextBox ID="txtOrgZip" runat="server"></asp:TextBox>
                                    </label>
                                </div>

                                <div class="large-12 medium-12 small-12 columns">
                                    <label>Phone
                                        <asp:TextBox ID="txtOrgPhone" runat="server"></asp:TextBox>
                                    </label>
                                </div>

                            </div>

                        </div>
                    </dd>

                    <!-- Payment Gateway -->
                    <dd>
                        <a id="configTab4" runat="server" href="#panel4"><span id="spanAPI" runat="server">Payment Gateway</span></a>
                        <div id="panel4" class="content">

                            <div class="row" style="margin-bottom: 0.75rem; position: relative; top: -40px; margin-bottom: -40px;">
                                <div class="large-12 medium-12 small-12 columns" style="width: 100%; text-align: right;">
                                    <a href="javascript: NavigateTopicPopup('549857a6ead6361ac8883c51');" id="link_help_paymentGateway">Help?</a>
                                </div>
                            </div>

                            <div class="row">

                                <div class="large-12 medium-12 small-12 columns">
                                    <label>&nbsp;
                                        <asp:CheckBox ID="chkRequiresSsl" runat="server" Text="Requires SSL?" />
                                    </label>
                                    </div>

                                <div class="large-12 medium-12 small-12 columns">
                                    <label>Gateway Name
                                        <asp:TextBox ID="txtGatewayName" runat="server"></asp:TextBox>
                                    </label>
                                    </div>

                                <div class="large-12 medium-12 small-12 columns">
                                    <label>ApiVersion
                                        <asp:TextBox ID="txtApiVersion" runat="server"></asp:TextBox>
                                    </label>
                                    </div>

                                <div class="large-12 medium-12 small-12 columns">
                                    <label>ApiKey
                                        <asp:TextBox ID="txtApiKey" runat="server"></asp:TextBox>
                                    </label>
                                    </div>

                                <div class="large-12 medium-12 small-12 columns">
                                    <label>Username
                                        <asp:TextBox ID="txtLoginUsername" runat="server"></asp:TextBox>
                                    </label>
                                    </div>

                                <div class="large-12 medium-12 small-12 columns">
                                    <label>Password
                                        <asp:TextBox ID="txtLoginPassword" runat="server"></asp:TextBox>
                                    </label>
                                    </div>

                                <div class="large-12 medium-12 small-12 columns">
                                    <label>Protocol
                                        <asp:TextBox ID="txtProtocol" runat="server"></asp:TextBox>
                                    </label>
                                    </div>

                                <div class="large-12 medium-12 small-12 columns">
                                    <label>Url
                                        <asp:TextBox ID="txtUrl" runat="server"></asp:TextBox>
                                    </label>
                                    </div>
                            </div>

                        </div>
                    </dd>

                    <!-- Payment Method -->
                    <dd>
                        <a id="configTab5" runat="server" href="#panel5"><span id="spanPaymentMethod" runat="server">Payment Method</span></a>
                        <div id="panel5" class="content">

                            <div class="row" style="margin-bottom: 0.75rem; position: relative; top: -40px; margin-bottom: -40px;">
                                <div class="large-12 medium-12 small-12 columns" style="width: 100%; text-align: right;">
                                    <a href="javascript: NavigateTopicPopup('5499fd8dead6361da032e309');" id="link_help_paymentMethod">Help?</a>
                                </div>
                            </div>

                                <div class="row">
                                    <div class="large-12 medium-12 small-12 columns">
                                        <select id="dlPaymentMethod" class="chosen-select" runat="server" name="dlPaymentMethod" onchange="javascript: setPaymentMethod();">
                                            <option value="None">None</option>
                                            <option value="ACH" selected="selected">ACH</option>
                                            <option value="Credit Card">Credit Card</option>
                                            <option value="Manual Check">Manual Check</option>
                                            <option value="Wire Transfer">Wire Transfer</option>
                                        </select>
                                        </div>
                                </div>

                                <hr />

                                <div class="row" id="divACHPayment" runat="server">

                                    <div class="large-12 medium-12 small-12 columns">
                                        <label>Institution Name
                                            <asp:TextBox ID="txtACHInstitutionName" runat="server"></asp:TextBox>
                                        </label>
                                        </div>

                                    <div class="large-12 medium-12 small-12 columns">
                                        <label>Routing Number
                                            <asp:TextBox ID="txtACHRoutingNumber" runat="server"></asp:TextBox>
                                        </label>
                                        </div>

                                    <div class="large-12 medium-12 small-12 columns">
                                        <label>Account Number
                                            <asp:TextBox ID="txtACHAccountNumber" runat="server"></asp:TextBox>
                                        </label>
                                        </div>
                                </div>

                            <div class="row" id="divCreditCardPayment" runat="server">
                                <div class="large-12 medium-12 small-12 columns">
                                    <asp:DropDownList ID="dlCreditCardType" runat="server" CssClass="chosen-select">
                                        <asp:ListItem Value="-1">Select a Card Type</asp:ListItem>
                                        <asp:ListItem Value="0">American Express</asp:ListItem>
                                        <asp:ListItem Value="1">MasterCard</asp:ListItem>
                                        <asp:ListItem Value="2">Visa</asp:ListItem>
                                    </asp:DropDownList>
                                    </div>

                                <div style="padding:0.15rem;">&nbsp;</div>

                                <div class="large-12 medium-12 small-12 columns">
                                    <label>Cardholder Name (Name as appears on card)
                                        <asp:TextBox ID="txtCardHolderName" runat="server"></asp:TextBox>
                                    </label>
                                    </div>

                                <div class="large-12 medium-12 small-12 columns">
                                    <label>Card Number
                                        <asp:TextBox ID="txtCardNumber" runat="server"></asp:TextBox>
                                    </label>
                                    </div>
                                <div class="large-12 medium-12 small-12 columns">
                                    <label>CCV Number
                                        <asp:TextBox ID="txtCCVNumber" runat="server"></asp:TextBox>
                                    </label>
                                    </div>
                                <div class="large-12 medium-12 small-12 columns">
                                    <label>Expiration Date
                                        <asp:TextBox ID="txtCardExpires" runat="server"></asp:TextBox>
                                    </label>
                                    </div>

                                <!-- Billing Address -->
                                <div class="large-12 medium-12 small-12 columns">
                                    <label>Billing Street 1
                                        <asp:TextBox ID="txtBillingStreet1" runat="server"></asp:TextBox>
                                    </label>
                                    </div>

                                <div class="large-12 medium-12 small-12 columns">
                                    <label>Billing Street 2
                                        <asp:TextBox ID="txtBillingStreet2" runat="server"></asp:TextBox>
                                    </label>
                                    </div>

                                <div class="large-12 medium-12 small-12 columns">
                                    <label>Billing City
                                        <asp:TextBox ID="txtBillingCity" runat="server"></asp:TextBox>
                                    </label>
                                    </div>

                                <div class="large-12 medium-12 small-12 columns" style="margin-bottom: 15px;">
                                    <label>Billing State
                                        <asp:DropDownList ID="dlStates" runat="server" CssClass="chosen-select">
                                            <asp:ListItem Value="000000000000000000000000">Select State</asp:ListItem>
                                        </asp:DropDownList>
                                    </label>
                                    </div>

                                <div class="large-12 medium-12 small-12 columns">
                                    <label>Billing Zipcode
                                        <asp:TextBox ID="txtBillingZipcode" runat="server"></asp:TextBox>
                                    </label>
                                    </div>

                            </div>

                            <div class="row" id="divManualCheck" runat="server">
                                <div class="large-12 medium-12 small-12 columns" style="width: 100%; text-align: center; font-size: 12px;">
                                    Manual Check
                                    </div>
                            </div>

                            <div class="row" id="divWireTransfer" runat="server">

                                <div class="large-12 medium-12 small-12 columns">
                                    <label>Institution Name
                                        <asp:TextBox ID="txtWireInstitutionName" runat="server"></asp:TextBox>
                                    </label>
                                </div>

                                <div class="large-12 medium-12 small-12 columns">
                                    <label>Routing Number
                                        <asp:TextBox ID="txtWireRoutingNumber" runat="server"></asp:TextBox>
                                    </label>
                                </div>

                                <div class="large-12 medium-12 small-12 columns">
                                    <label>Account Number
                                        <asp:TextBox ID="txtAccountNumber" runat="server"></asp:TextBox>
                                    </label>
                                </div>
                            </div>

                        </div>
                    </dd>

                    <!-- Price Tiers -->
                    <dd>
                        <a id="configTab6" runat="server" href="#panel6"><span id="spanPricingTiers" runat="server">Pricing Tiers</span></a>
                        <div id="panel6" class="content">

                            <div class="row" style="margin-bottom: 0.75rem; position: relative; top: -40px; margin-bottom: -40px;">
                                <div class="large-12 medium-12 small-12 columns" style="width: 100%; text-align: right;">
                                    <a href="javascript: NavigateTopicPopup('5499fda4ead6361da032e30a');" id="link_help_pricingTiers">Help?</a>
                                </div>
                            </div>

                            <div id="divPricingTiersContainer" runat="server" class="row" style="padding: 10px; padding-top: 20px;">

                                <asp:Panel ID="panelOtpPricing" runat="server" GroupingText="OTP">

                                    <div class="large-12 medium-12 small-12 columns">
                                        <label>Sent via (Email)
                                            <asp:TextBox ID="OtpSentEmail" runat="server" Width="425"></asp:TextBox>
                                        </label>
                                    </div>

                                    <div class="large-12 medium-12 small-12 columns">
                                        <label>Sent via (Sms)
                                            <asp:TextBox ID="OtpSentSms" runat="server" Width="425"></asp:TextBox>
                                        </label>
                                    </div>

                                    <div class="large-12 medium-12 small-12 columns">
                                        <label>Sent via (Voice)
                                            <asp:TextBox ID="OtpSentVoice" runat="server" Width="425"></asp:TextBox>
                                        </label>
                                    </div>

                                </asp:Panel>

                                <div style="padding: 0.5rem;"></div>

                                <asp:Panel ID="panelAdPricing" runat="server" GroupingText="Advertising">

                                    <div class="large-12 medium-12 small-12 columns" style="width: 100%; text-align: center;">

                                        <input id="radioAdSimpleCharge" name="AdBillingTypeOption" onclick="javascript: setAdBillingType('Single');" runat="server" type="radio" value="Single" />
                                        <span style="position: relative; top: -2px;">Single Charge</span>

                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;

                                        <input id="radioAdItemizedCharges" name="AdBillingTypeOption" onclick="javascript: setAdBillingType('Itemized');" runat="server" type="radio" value="Itemized" />
                                        <span style="position: relative; top: -2px;">Itemized Charges</span>

                                    </div>

                                    <div class="large-12 medium-12 small-12 columns" style="border-top: solid 1px #d8d8d8; padding-top: 20px;">
                                        <label>Sent with (Otp Message)
                                            <asp:TextBox ID="AdMessageSent" runat="server" Width="425"></asp:TextBox>
                                        </label>
                                    </div>

                                    <div id="divAdSentOtpScreen" runat="server" class="large-12 medium-12 small-12 columns">
                                        <label>Sent with (Enter Otp Screen)
                                            <asp:TextBox ID="AdEnterOtpScreenSent" runat="server" Width="425"></asp:TextBox>
                                        </label>
                                    </div>

                                    <div id="divAdSentVerifyOtpScreen" runat="server" class="large-12 medium-12 small-12 columns">
                                        <label>Sent with (Verify Otp Screen)
                                            <asp:TextBox ID="AdVerificationScreenSent" runat="server" Width="425"></asp:TextBox>
                                        </label>
                                    </div>

                                </asp:Panel>

                                <div style="padding: 0.5rem;"></div>

                                <asp:Panel ID="panel" runat="server" GroupingText="End User">

                                    <div class="large-12 medium-12 small-12 columns">
                                        <label>Registration
                                            <asp:TextBox ID="EndUserRegister" runat="server" Width="425"></asp:TextBox>
                                        </label>
                                    </div>

                                    <div class="large-12 medium-12 small-12 columns">
                                        <label>Verification
                                            <asp:TextBox ID="EndUserVerify" runat="server" Width="425"></asp:TextBox>
                                        </label>
                                    </div>

                                </asp:Panel>

                            </div>

                        </div>
                    </dd>

                </dl>
            </div>
        </div>

        <div style="padding:0.25rem;"></div>

        <div class="row" id="divConfigWarning_2" runat="server" style="margin-bottom: 15px;">
            <div id="divConfigWarningText_2" runat="server" class="large-12 medium-12 small-12 columns" style="width: 100%; text-align: center; color: #ff0000;">
                !!! Default (Client) configuration !!!
            </div>
        </div>

        <div style="padding:0.25rem;"></div>
            
        <div class="row">
            <div class="large-12 columns" style="width: 100%; text-align: center; padding-bottom: 25px;">
                <%--<input id="btnSave" type="submit" value="Save" class="button tiny radius" />--%>
                <input id="btnSave" type="button" runat="server" value="Save" onclick="javascript: UpdateBillConfig();" class="button tiny radius" />
                <input id="btnCancel" type="button" onclick="javascript: cancelChanges();" value="Cancel" class="button tiny radius" />    
            </div>                      
        </div>

        <input id="panelFocusConfig" runat="server" type="hidden" value="" />

        <input id="hiddenOwnerId" runat="server" type="hidden" value="" />
        <input id="hiddenOwnerType" runat="server" type="hidden" value="" />

    <script src="../../JavaScript/foundation.min.js"></script>
    <script src="../../JavaScript/chosen.jquery.js" type="text/javascript"></script>
    <script src="../../JavaScript/prism.js" type="text/javascript" charset="utf-8"></script>
    <script src="../../Javascript/foundation.accordion.js"></script>


    <%--<script src="../../Javascript/foundation.min.js"></script>
    <script src="../../Javascript/chosen.jquery.js"></script>
    <script src="../../Javascript/prism.js"></script>--%>
    <script type="text/javascript">
        $(document).foundation();
        $('.chosen-select').chosen({ disable_search_threshold: 28 });
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
    </form>
</body>
</html>
