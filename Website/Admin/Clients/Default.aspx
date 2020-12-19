<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/AdminConsole.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Admin.Clients.Default" %>

<%@ Register Src="~/UserControls/EventTypes.ascx" TagPrefix="uc1" TagName="EventTypes" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ScriptContainer" Runat="Server">
    <script>
        $(document).ready(function () {

            //var y = $("#divMenuBar").offset();
            //alert(y.top);

            // This is all being converted to ajax
            $("#clientAssignmentsContainer").hide();

            $('#spanTabName').html('');
            $('#EventHistoryDisabledMessage').hide();

            //On tab click, set hidden value 'panelFocus'
            $('#Tab_Organization').click(function () {
                $('#panelFocusClients').val('Tab_Organization');
                eventHistoryTimerEnabled = false;
            });
            $('#Tab_EventReporting').click(function () {
                $('#panelFocusClients').val('Tab_EventReporting');

                //alert("Tab2");

                eventHistoryTimerEnabled = true;
                getEventHistory();
            });
            $('#Tab_Assignments').click(function () {
                $('#panelFocusClients').val('Tab_Assignments');
                eventHistoryTimerEnabled = false;
            });
            $('#Tab_MessageProviders').click(function () {
                $('#panelFocusClients').val('Tab_MessageProviders');
                eventHistoryTimerEnabled = false;
            });
            $('#Tab_OTPSettings').click(function () {
                $('#panelFocusClients').val('Tab_OTPSettings');
                eventHistoryTimerEnabled = false;
            });
            $('#Tab_Advertising').click(function () {
                $('#panelFocusClients').val('Tab_Advertising');
                eventHistoryTimerEnabled = false;
                advertisingAssignment();
            });
            // Comment
            //Disable accordian tabs if no client is selected. Reset 'panelFocusClients' to ''
            if ($('#dlClients').val() == 000000000000000000000000) {
                $('#Tab_Organization,#Tab_EventReporting,#Tab_Assignments,#Tab_MessageProviders,#Tab_OTPSettings,#Tab_Advertising').prop('disabled', true);
                $('#Tab_Organization,#Tab_EventReporting,#Tab_Assignments,#Tab_MessageProviders,#Tab_OTPSettings,#Tab_Advertising').css('background', '#e3e3e3');
                $('#Tab_Organization,#Tab_EventReporting,#Tab_Assignments,#Tab_MessageProviders,#Tab_OTPSettings,#Tab_Advertising').css('color', '#999');
                $('#panelFocusClients').val('');
                $('#btnClientActions').val('Create New');
                $('#btnCancelClientUpdate').prop('disabled', true);
            } else if ($('#dlClients').val() == null) { //if create new, enable only 1st tab
                $('#Tab_EventReporting,#Tab_Assignments,#Tab_MessageProviders,#Tab_OTPSettings,#Tab_Advertising').prop('disabled', true);
                $('#Tab_EventReporting,#Tab_Assignments,#Tab_MessageProviders,#Tab_OTPSettings,#Tab_Advertising').prop('href', '#');
                $('#Tab_EventReporting,#Tab_Assignments,#Tab_MessageProviders,#Tab_OTPSettings,#Tab_Advertising').css('cursor', 'default');
                $('#Tab_EventReporting,#Tab_Assignments,#Tab_MessageProviders,#Tab_OTPSettings,#Tab_Advertising').css('background', '#e3e3e3');
                $('#Tab_EventReporting,#Tab_Assignments,#Tab_MessageProviders,#Tab_OTPSettings,#Tab_Advertising').css('color', '#999');
                $('#panelFocusClients').val('Tab_Organization');

                $('#Tab_EventReporting,#Tab_Assignments,#Tab_MessageProviders,#Tab_OTPSettings,#Tab_Advertising').click(function () {
                    alert('Please Create a New Client');
                    $('html, body').animate({
                        scrollTop: ($('#scroll2').offset().top - 46)
                    }, 750, 'easeOutExpo');
                });
            }

            $('#Tab_Organization,#Tab_EventReporting,#Tab_Assignments,#Tab_MessageProviders,#Tab_OTPSettings,#Tab_Advertising').click(function () {
                if ($('#dlClients').val() == 000000000000000000000000) {
                    alert('Please Select a Client');
                    $('#Tab_Organization,#Tab_EventReporting,#Tab_Assignments,#Tab_MessageProviders,#Tab_OTPSettings,#Tab_Advertising').prop('disabled', true);
                    $('#Tab_Organization,#Tab_EventReporting,#Tab_Assignments,#Tab_MessageProviders,#Tab_OTPSettings,#Tab_Advertising').css('background', '#e3e3e3');
                    $('#Tab_Organization,#Tab_EventReporting,#Tab_Assignments,#Tab_MessageProviders,#Tab_OTPSettings,#Tab_Advertising').css('color', '#999');
                    $('#panelFocusClients').val('');
                    $('#btnClientActions').val('Create New');
                    $('#btnCancelClientUpdate').prop('disabled', true);
                } else {
                    $('#Tab_Organization,#Tab_EventReporting,#Tab_Assignments,#Tab_MessageProviders,#Tab_OTPSettings,#Tab_Advertising').prop('disabled', false);
                }
            });

            $('#dlClients').change(function () {
                if ($('#dlClients').val() == 000000000000000000000000) {
                    $('#panelFocusClients').val('');
                    $('#btnClientActions').val('Create New 2');
                    $('#btnCancelClientUpdate').prop('disabled', true);
                } else {
                    $('#Tab_Organization,#Tab_EventReporting,#Tab_Assignments,#Tab_MessageProviders,#Tab_OTPSettings,#Tab_Advertising').prop('disabled', false);
                    $('#Tab_Organization,#Tab_EventReporting,#Tab_Assignments,#Tab_MessageProviders,#Tab_OTPSettings,#Tab_Advertising').css('background', '#efefef');
                    $('#Tab_Organization,#Tab_EventReporting,#Tab_Assignments,#Tab_MessageProviders,#Tab_OTPSettings,#Tab_Advertising').css('color', '#222');
                    $('#Tab_Organization').click();
                }
            });

            // scroll page so client menu is positioned below menu bar
            var currentTab = $('#panelFocusClients').val();

            //alert("currentTab: " + currentTab);

            if (currentTab == '') {
                eventHistoryTimerEnabled = false;
            } else if (currentTab == 'Tab_EventReporting') {
                eventHistoryTimerEnabled = true;
                $('#' + currentTab + '').click();
                $('html, body').animate({ scrollTop: ($("#scroll3").offset().top - 48) }, 750, 'easeOutExpo');
            } else {
                eventHistoryTimerEnabled = false;
                $('#' + currentTab + '').click();
                $('html, body').animate({ scrollTop: ($("#scroll3").offset().top - 48) }, 750, 'easeOutExpo');
            }

            //Hide message options select menus onload
            $("#dlEmailTemplates_chosen").hide();
            $("#dlHtmlTemplates_chosen").hide();
            $("#dlSmsTemplates_chosen").hide();
            $("#dlVoiceTemplates_chosen").hide();
            $("#tempSelect").hide();

            //Message Template alerts on OTP Settings tab
            $('#dlMessageTemplates_chosen a').click(function () {
                if ($('#dlClients').val() == 000000000000000000000000) {
                    alert('Please select a client');
                    $('#dlClients_chosen a').focus();
                }
            });
            $('#tempSelect_chosen a').click(function () {
                if ($('#dlClients').val() == 000000000000000000000000) {
                    alert('Please select a client');
                    $('#dlClients_chosen a').focus();
                }
                else if ($('#dlMessageTemplates_chosen a span').html() == 'Please Choose') {
                    alert('Please choose a Message Template');
                }
            });
            function scrollTabs() {
                $('html, body').animate({ scrollTop: ($("#scroll3").offset().top - 48) }, 750, 'easeOutExpo');
            }

        });
   </script>
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="BodyContent" Runat="Server">

    <script type="text/javascript">
        $(function () {
            $('#popupDatepickerStartDate').datepicker();
            $('#popupDatepickerEndDate').datepicker();
        });
    </script>

    <div class="row" id="scroll3">
        <div class="large-6 medium-6 small-12 columns">
            <asp:DropDownList ID="dlClients" runat="server" AutoPostBack="True" OnSelectedIndexChanged="dlClients_SelectedIndexChanged" CssClass="chosen-select">
                <asp:ListItem Value="000000000000000000000000">Select a Client</asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="large-6 medium-6 small-12 columns">

        </div>
    </div>
    <div style="padding: 0.5rem;"></div>
    <div class="row">
        <div class="large-12 columns">
            <div class="alert-box success radius" id="clientVerificationMessage" style="display: none; cursor: pointer;" onclick="javascript: noDisplay();">
                User Verification Provider settings have been saved!
            </div>
            <div class="alert-box success radius" id="assignAdministratorsMessage" style="display: none; cursor: pointer;" onclick="javascript: noDisplay();">
                Admins assigned!
            </div>
            <div class="alert-box success radius" id="updateMessage" runat="server" style="cursor: pointer;" onclick="javascript: noDisplay();">
                Update message...
            </div>
        </div>
    </div>
    <div class="row" id="scroll2">
      <div class="large-12 columns">
        <dl class="accordion" data-accordion="">
          <dd>
            <a id="Tab_Organization" runat="server" href="#panel_Organization" title=""><span id="spanOrganization" runat="server">Organization</span></a>
            <div id="panel_Organization" class="content">

                <div class="row" style="margin-bottom: 0.75rem; position: relative; top: -40px; margin-bottom: -40px;">
                    <div class="large-12 medium-12 small-12 columns" style="width: 100%; text-align: right;">
                        <a href="javascript: NavigateTopicPopup('5491ed80ead6361c58f5ce03');" id="link_help_organization">Help?</a>
                    </div>
                </div>

                <div style="padding: 0.75rem;"></div>
                <div class="row" style="border-bottom: solid 1px #c0c0c0; margin-bottom: 25px;">   
                    <div class="large-4 medium-4 small-12 columns" style="margin-bottom: 0.75rem; text-align: center; color: #808080;">
                        <a href="javascript: UploadClientLogo();" id="link_ownerLogo">
                            <img id="imgOwnerLogo" runat="server" src="/Images/OwnerLogos/!Empty-Placeholder.png" class="OwnerLogo" />
                        </a>
                        <br />
                        Max width allowed: 175px
                    </div>                     
                    <div class="large-4 medium-4 small-12 columns" style="margin-bottom: 0.75rem;">
                        <span style="font-weight: normal; color: #808080;">
                            ClientId 
                            <input class="transparentTextBox" type="text" runat="server" id="txtClientID" style="border-bottom: solid 1px #ff0000; width: 13rem;height: 1.375rem;padding: 0; position: relative; top: -8px;" />
                        </span>
                        <label runat="server" id="clientCreatedDate">Created:</label>
                        <label runat="server" id="clientLastModifiedDate">Modified:</label>
                    </div>
                    <div class="large-4 medium-4 small-12 columns" style="margin-bottom: 0.25rem;">
                        <div style="display:block;height:25px;position: relative;top: -3px;"><asp:CheckBox CssClass="noMargin" ID="chkClientEnabled" runat="server" Text="Enable Account" /></div>
                        <div style="display:block;height:25px;position: relative;top: -7px; margin-bottom: 18px;"><asp:CheckBox CssClass="noMargin" ID="chkOpenAccessServicesEnabled" runat="server" Text="Enable Open Access"  ToolTip="If checked this client can access the Open Access Service for OTP operations." /></div>
                        <label>Administrators: <span id="adminCount" runat="server">0</span></label>
                        <label>Groups: <span id="groupCount" runat="server">0</span></label>
                    </div>             
                </div>

                <div class="show-for-small" style="padding: 0.125rem;"></div>
                <div class="row">
                  <div class="large-6 medium-6 small-12 columns">
                    <label id="lblClientName"><span id="spanClientName">Name</span>
                        <input id="txtClientName" lbl="Client Name" isrequired="true" isvalid="false" min-length="3" max-length="75" 
                            matchpattern="^([A-Za-z\d\s.,!-_#()]+)$" patterndescription="A-Z, a-z, 0-9, space, .,!-_#()" type="text" runat="server" 
                            onblur="javascript: validateFormFields(this);" 
                            onkeyup="javascript: validateFormFields(this);"
                            onchange="javascript: validateFormFields(this);" />
                    </label>
                  </div>
                  <div class="large-6 medium-6 small-12 columns">
                    <label>Tax ID
                        <asp:TextBox ID="txtTaxId" runat="server"></asp:TextBox>
                    </label>
                  </div>
                </div>
                <div class="row">
                  <div class="large-6 medium-6 small-12 columns">
                    <label>Street 1
                        <asp:TextBox ID="txtStreet1" runat="server"></asp:TextBox>
                    </label>
                  </div>
                  <div class="large-6 medium-6 small-12 columns">
                    <label>Street 2
                        <asp:TextBox ID="txtStreet2" runat="server"></asp:TextBox>
                    </label>
                  </div>
                </div>
                <div class="row">
                  <div class="large-6 medium-6 small-12 columns">
                    <label>Unit
                        <asp:TextBox ID="txtUnit" runat="server"></asp:TextBox>
                    </label>
                  </div>
                  <div class="large-6 medium-6 small-12 columns">
                    <label>City
                        <asp:TextBox ID="txtCity" runat="server"></asp:TextBox>
                    </label>
                  </div>
                </div>
                <div class="row">
                  <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 1rem;">
                    <label id="lblStates"><span id="spanState">State</span>
                        <asp:DropDownList ID="dlStates" runat="server" CssClass="chosen-select">
                            <asp:ListItem Value="000000000000000000000000">Select State</asp:ListItem>
                        </asp:DropDownList>
                    </label>
                  </div>
                  <div class="large-6 medium-6 small-12 columns">
                    <label id="lblZipcode"><span id="spanZipCode">Zip Code</span>
                        <input id="txtZipCode" lbl="Zip Code" isrequired="true" isvalid="false" min-length="5" max-length="10" 
                            matchpattern="^\d{5}((-|\s)?\d{4})?$" patterndescription="#####) or (#####-####" type="text" runat="server" 
                            onblur="javascript: validateFormFields(this);" 
                            onkeyup="javascript: validateFormFields(this);"
                            onchange="javascript: validateFormFields(this);" />
                    </label>
                  </div>
                </div>
                <div style="padding: 0.5rem;"></div>
                <div class="row">
                  <div class="large-6 medium-6 small-12 columns">
                    <label id="lblEmail"><span id="spanEmail">Email Address</span>
                        <input id="txtEmail" lbl="Email Address" isrequired="true" isvalid="false" min-length="7" max-length="50" 
                            matchpattern="^([a-z0-9_\.-]+)@([\da-z\.-]+)\.([a-z\.]{2,6})$" patterndescription="a valid email address" type="text" runat="server" 
                            onblur="javascript: validateFormFields(this);" 
                            onkeyup="javascript: validateFormFields(this);"
                            onchange="javascript: validateFormFields(this);" />
                    </label>
                  </div>
                  <div class="large-6 medium-6 small-12 columns">
                    <label id="lblPhone"><span id="spanPhone">Phone</span>
                        <input id="txtPhone" lbl="Phone Number" isrequired="true" isvalid="false" min-length="10" max-length="12" 
                            matchpattern="^\s*(?:\+?(\d{1,3}))?[-. (]*(\d{3})[-. )]*(\d{3})[-. ]*(\d{4})(?: *x(\d+))?\s*$" patterndescription="###-###-####" type="text" runat="server" 
                            onblur="javascript: validateFormFields(this);" 
                            onkeyup="javascript: validateFormFields(this);"
                            onchange="javascript: validateFormFields(this);" />
                    </label>
                  </div>
                </div>
              </div>
            </dd>

            <dd>
                <a id="Tab_Advertising" runat="server" href="#panel_Advertising">Advertising</a>
                <div id="panel_Advertising" class="content">

                    <div class="row" style="margin-bottom: 0.75rem; position: relative; top: -40px; margin-bottom: -40px;">
                        <div class="large-12 medium-12 small-12 columns" style="width: 100%; text-align: right;">
                            <a href="javascript: NavigateTopicPopup('5491edabead6361c58f5ce05');" id="link_help_advertising">Help?</a>
                        </div>
                    </div>

                    <div class="row">
                        <div class="large-12 columns">
                            <div id="AdPassConfigurationContainer" style="border: 1px solid #b8b8b8;">                                    
                                
                            </div>
                        </div>
                    </div>
                </div>
            </dd>

            <dd>
            <a id="Tab_Assignments" runat="server" href="#panel_Assignments"><span id="span1" runat="server">Assignments</span></a>
                <div id="panel_Assignments" class="content">

                    <div class="row" style="margin-bottom: 0.75rem; position: relative; top: -40px; margin-bottom: -40px;">
                        <div class="large-12 medium-12 small-12 columns" style="width: 100%; text-align: right;">
                            <a href="javascript: NavigateTopicPopup('5491edcbead6361c58f5ce07');" id="link_help_assignments">Help?</a>
                        </div>
                    </div>

                    <div id="clientSettingsDisplay">
                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns">
                                <select id="assignmentMenu" class="chosen-select" runat="server" name="assignments" onchange="javascript: assignmentSelection();">
                                    <option value="Please Select"></option>
                                    <option value="Administrator Assignments"></option>
                                    <option value="Allowed IP Adresses"></option>
                                    <option value="Group Assignments"></option>
                                    <option value="Secure Trading Site Config"></option>
                                    <option value="User Assignments"></option>
                                </select>
                            </div>
                            <div class="large-6 medium-6 small-12 columns">
                                &nbsp;
                            </div>
                        </div>
                        <div style="padding: 0.25rem;"></div>
                        <div class="row">
                            <div class="large-12 columns">
                                <div id="clientAssignmentsContainer" style="border: 1px solid #b8b8b8;display: none;">                                    
                                    <input id="groupsAssigned" runat="server" type="hidden" value="" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </dd>

            <dd>
                <a id="Tab_EventReporting" runat="server" onclick="javascript:scrollTabs();" href="#panel_EventReporting">Event Reporting</a>
                <div id="panel_EventReporting" class="content">

                    <div class="row" style="margin-bottom: 0.75rem; position: relative; top: -40px; margin-bottom: -40px;">
                        <div class="large-12 medium-12 small-12 columns" style="width: 100%; text-align: right;">
                            <a href="javascript: NavigateTopicPopup('5491ee32ead6361c58f5ce08');" id="link_help_eventReporting">Help?</a>
                        </div>
                    </div>

                    <div class="row">
                        <div class="large-12 columns">
                            <span><a href="#" id="displaySearchSettings" onclick="javascript: toggleSearchDisplay();">[+] Show Settings</a></span>
                        </div>
                    </div>
                    <div style="padding: 0.25rem;"></div>
                    <div class="row">
                        <div class="large-12 columns">
                            <div id="searchSettingsDisplay" style="display: none;">
                                <fieldset style="margin-top: 0;">                     
                                <div class="row">
                                    <div class="large-4 medium-4 small-12 columns" style="margin-bottom: 0.25rem;">
                                        <label>
                                            <uc1:EventTypes runat="server" ID="EventTypes" />
                                        </label>
                                    </div>
                                    <div class="large-4 medium-4 small-12 columns" style="margin-bottom: 0.25rem;">
                                        <asp:DropDownList ID="dlRecordsPerPage" class="chosen-select" runat="server" AutoPostBack="True">
                                            <asp:ListItem value="5">5 Records Per Page</asp:ListItem>
                                            <asp:ListItem value="10">10 Records Per Page</asp:ListItem>
                                            <asp:ListItem value="25">25 Records Per Page</asp:ListItem>
                                            <asp:ListItem value="50">50 Records Per Page</asp:ListItem>
                                            <asp:ListItem value="100">100 Records Per Page</asp:ListItem>
                                            <asp:ListItem value="250">250 Records Per Page</asp:ListItem>
                                            <asp:ListItem value="500">500 Records Per Page</asp:ListItem>
                                            <asp:ListItem value="750">750 Records Per Page</asp:ListItem>
                                            <asp:ListItem value="1000">1000 Records Per Page</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="large-4 medium-4 small-12 columns" style="margin-bottom: 0.25rem;">
                                        <label>
                                        <select name="mySelect" id="clientList" class="chosen-select">
                                            <option value="client 1">Chart</option>
                                            <option value="client 1">Grid</option>
                                            <option value="client 1">Billing</option>
                                        </select>
                                        </label>
                                    </div>
                                    </div>
                                <div class="row">
                                    <div class="large-4 medium-4 small-12 columns" style="margin-bottom: 0.25rem;">
                                        <asp:DropDownList ID="dlSortField" class="chosen-select" runat="server" AutoPostBack="False">
                                            <asp:ListItem>Sort By</asp:ListItem>
                                            <asp:ListItem>Date</asp:ListItem>
                                            <asp:ListItem>Details</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="large-4 medium-4 small-12 columns" style="margin-bottom: 0.25rem;">
                                        <asp:DropDownList ID="dlSortOrder" class="chosen-select" runat="server" AutoPostBack="False">
                                            <asp:ListItem Value="Ascending">Ascending</asp:ListItem>
                                            <asp:ListItem Value="Descending" Selected="True">Descending</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="large-4 medium-4 small-12 columns" style="margin-bottom: 0.25rem;">
                                        <select id="dlChartTypes" onselect="javascript: setChartType();" class="chosen-select">
                                            <option value="Annotation">Annotation</option>
                                            <option value="Area">Area</option>
                                            <option value="Guage">Guage</option>
                                        </select>
                                    </div>
                                </div>
                                <div style="padding-top:0.75rem;"></div>
                                <div class="row">
                                    <div class="large-6 medium-6 small-12 columns">
                                        <label>Start Date
                                          <input type="text" id="popupDatepickerStartDate" runat="server" />
                                        </label>
                                    </div>
                                    <div class="large-6 medium-6 small-12 columns">
                                        <label>End Date
                                          <input type="text" id="popupDatepickerEndDate" runat="server" />
                                        </label>
                                    </div>
                                </div>
                                </fieldset>
                            </div>
                        </div>
                    </div>
                    <div style="padding: 0.25rem;"></div>
                    <div class="row" id="eventButtonsContainer">
                        <div class="large-6 medium-6 small-12 columns">
                            <div class="hide-for-small" style="display: block; height: 26px;"></div>
                            <span class="title_875rem" id="refreshMessage" style="color: #222;">Loading...</span>&nbsp;<span class="title_875rem" style="color: #222;display: none;" id="refreshIndicator"><img src="/Images/progress-bar-round.gif" style="height: 16px; width: 16px;" /></span>
                            <div class="show-for-small" style="display: block; height: 0.75rem;"></div>
                        </div>
                        <div class="large-6 medium-6 small-12 columns" style="text-align: right;">
                            <input class="button tiny radius" style="width: 80px;margin:0 0 0.75rem;" id="btnPause" type="button" value="Pause" onclick="javascript: pauseRefresh(false);" />
                            <input class="button tiny radius" disabled style="width: 80px;margin:0 0 0.75rem;" id="btnDetails" type="button" value="Details" onclick="javascript: showEventDetailsPopup(false);" />
                            <input class="button tiny radius" disabled style="width: 80px;margin:0 0 0.75rem;" id="save" type="button" value="Export" onclick="javascript: exportEvents();" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="large-12 columns">
                            <table style='width: 100%;border-collapse: collapse;margin-bottom:0;'><thead><tr><th>Row</th><th>Date</th><th>Details</th></thead>
                                <tbody id='eventHistoryTBody'>
                                </tbody>
                            </table>
                            <div id="EventHistoryDisabledMessage" style="display: block;height: 125px;line-height: 125px;text-align:center;"><span class="title_875rem;">You must first create a new client in order to view Event Reporting.</span></div>
                        </div>
                    </div>
                    <div class="row" id="paginationButtonsContainer">
                        <div class="large-6 medium-6 small-12 columns">
                            <div style="font-size:0.875rem;color:#4d4d4d;margin-top: 0.25rem;">
                                Page
                                <span id="CurrentPageNumber">0</span>
                                of
                                <span id="TotalPageNumbers">0</span>
                            </div>
                        </div>
                        <div class="large-6 medium-6 small-12 columns">
                            <div style="margin-top: 0.5rem;text-align: right;">
                                <input class="button tiny radius" disabled style="width: 80px;" id="btnFirst" type="button" value="First" onclick="javascript: navigatePages('first');" />
                                <input class="button tiny radius" disabled style="width: 80px;" id="btnPrevious" type="button" value="Previous" onclick="javascript: navigatePages('previous');" />
                                <input class="button tiny radius" disabled style="width: 80px;" id="btnNext" type="button" value="Next" onclick="javascript: navigatePages('next');" />
                                <input class="button tiny radius" disabled style="width: 80px;" id="btnLast" type="button" value="Last" onclick="javascript: navigatePages('last');" />
                            </div>
                        </div>
                    </div>
                    <div style="padding: 0.25rem;"></div>
                </div>
            </dd>

            <dd>
              <a id="Tab_MessageProviders" runat="server" href="#panel_MessageProviders">Messaging Providers</a>
              <div id="panel_MessageProviders" class="content">   
                  
                <div class="row" style="margin-bottom: 0.75rem; position: relative; top: -40px; margin-bottom: -40px;">
                    <div class="large-12 medium-12 small-12 columns" style="width: 100%; text-align: right;">
                        <a href="javascript: NavigateTopicPopup('5491ee58ead6361c58f5ce09');" id="link_help_messagingProviders">Help?</a>
                    </div>
                </div>
                               
                <div class="row">
                    <div class="large-12 columns">
                        <div id="divAdminNotificationProvider" runat="server">
                           <span style="font-size: 0.8125rem;font-weight: normal;">Admin Notifications (Email):</span> <a href="javascript: showAdminNotificationProviderPopup();" id="link_showAdminNotificationProviderPopup"><span id="spanManageAdminNotificationProvider" runat="server">Manage</span></a>
                        </div>
                        <div style="padding: 0.50rem;"></div>
                        <div style="margin-top:0.5rem;" id="div2" runat="server">
                            <span style="font-size: 0.8125rem;font-weight: normal;">Available Otp Message Delivery Providers</span>
                        </div>
                        <hr style="margin-top: 0.125rem;margin-bottom: 0;" />
                    </div>
                </div>
                <div id="divProvidersContainer" class="row" style="border: solid 0px #ff0000;">
                    <div class="large-4 medium-4 small-12 columns">
                        <div style="padding:.5rem 1.25rem 0;">
                            <span style="font-size: 0.8125rem;font-weight: bold;">Email</span>
                            <div id="divEmailProviders" runat="server" class="checkList"></div>
                        </div>
                    </div>
                    <div class="large-4 medium-4 small-12 columns">
                        <div style="padding:.5rem 1.25rem 0;">
                            <span style="font-size: 0.8125rem;font-weight: bold;">SMS</span>
                            <div id="divSmsProviders" runat="server" class="checkList"></div>
                        </div>
                    </div>
                    <div class="large-4 medium-4 small-12 columns">
                        <div style="padding:.5rem 1.25rem 0;">
                            <span style="font-size: 0.8125rem;font-weight: bold;">Voice</span>
                            <div id="divVoiceProviders" runat="server" class="checkList"></div>
                        </div>
                    </div>
                </div>
                <div style="padding: 0.25rem;"></div>
                <div class="row">
                    <div class="large-12 columns">
                        <span style="font-size: 0.8125rem;font-weight: bold;">Client Retry List -</span> <a href="javascript: resetRetryList();" id="link_resetRetryList" runat="server">Reset</a>
                        <asp:ListBox ID="selectProviderList" CssClass="clientRetryList" runat="server"></asp:ListBox>
                        <div class="hiddenRetryList">
                            <input id="hiddenRetryList" type="text" runat="server" style="width: 100%;" />
                            <input id="hiddenProvidersToDelete" type="text" runat="server" style="width: 100%;" />
                        </div>
                    </div>
                </div>
              </div>
            </dd>
            <dd>
              <a id="Tab_OTPSettings" runat="server" href="#panel_OTPSettings">OTP Settings</a>
              <div id="panel_OTPSettings" class="content">  
                  
                <div class="row" style="margin-bottom: 0.75rem; position: relative; top: -40px; margin-bottom: -40px;">
                    <div class="large-12 medium-12 small-12 columns" style="width: 100%; text-align: right;">
                        <a href="javascript: NavigateTopicPopup('5491ee7dead6361c58f5ce0a');" id="link_help_otpSettings">Help?</a>
                    </div>
                </div>
                                                
                <div class="row">
                  <div class="large-3 medium-6 small-12 columns" style="padding-top: 0.5rem;">
                    <label>Length
                        <asp:DropDownList ID="dlOtpLength" CssClass="chosen-select" runat="server">
                            <asp:listitem>4</asp:listitem>
                            <asp:listitem>5</asp:listitem>
                            <asp:listitem>6</asp:listitem>
                            <asp:listitem>7</asp:listitem>
                            <asp:listitem>8</asp:listitem>
                            <asp:listitem>9</asp:listitem>
                            <asp:listitem>10</asp:listitem>
                        </asp:DropDownList>
                    </label>
                  </div>
                  <div class="large-3 medium-6 small-12 columns" style="padding-top: 0.5rem;">
                    <label>Max Retries
                        <asp:DropDownList ID="dlMaxRetries" CssClass="chosen-select" runat="server">
                            <asp:listitem>1</asp:listitem>
                            <asp:listitem>2</asp:listitem>
                            <asp:listitem>3</asp:listitem>
                            <asp:listitem>4</asp:listitem>
                            <asp:listitem>5</asp:listitem>
                        </asp:DropDownList>
                    </label>
                  </div>
                  <div class="large-3 medium-6 small-12 columns" style="padding-top: 0.5rem;">
                    <label>Timeout
                        <asp:DropDownList ID="dlTimeout" CssClass="chosen-select" runat="server">
                            <asp:listitem>1</asp:listitem>
                            <asp:listitem>2</asp:listitem>
                            <asp:listitem>3</asp:listitem>
                            <asp:listitem>4</asp:listitem>
                            <asp:listitem>5</asp:listitem>
                            <asp:listitem>10</asp:listitem>
                            <asp:listitem>15</asp:listitem>
                            <asp:listitem>30</asp:listitem>
                            <asp:listitem>45</asp:listitem>
                            <asp:listitem>60</asp:listitem>
                        </asp:DropDownList>
                    </label>
                  </div>                  
                  <div class="large-3 medium-6 small-12 columns" style="padding-top: 0.5rem;">
                    <label>Charset
                        <select id="dlCharacterset" class="chosen-select" runat="server" name="dlCharacterSets">
                            <option value="abcdefghjkmnprstuvwxyz">Alpha Lowercase</option>
                            <option value="ABCDEFHIJKMNPRSTUVWXYZ">Alpha Uppercase</option>
                            <option value="0123456789">Numeric</option>
                        </select>
                    </label>
                  </div>
                </div>
                <div style="padding: 0.75rem;"></div>
                <div class="row">
                  <div class="large-12 columns">
                    <hr style="margin:0 0 0.625rem;">
                  </div>
                </div>
                <div class="row">
                  <div class="large-12 columns">
                    Message Templates
                  </div>
                </div>
                <div id="divTemplateContainer" runat="server" class="">
                    <div class="row">
                        <div id="divTemplateSelection" runat="server">
                            <div class="large-6 medium-6 small-12 columns" style="padding: 0.25rem;">
                                <select id="dlMessageTemplates" class="chosen-select" runat="server" onchange="javascript: setTemplateDisplay(this);"></select>
                            </div>
                        </div>
                    </div>
                    <div style="padding: 0.5rem;"></div>
                    <div id="divTemplateContent"></div>
                </div>
                <div class="row">
                    <div class="large-12 columns">
                        <hr />
                    </div>
                </div>
                <div class="row">
                    <div class="large-12 medium-12 small-12 columns talign">
                        <input id="btnRequestOtp" class="button tiny radius" type="button" runat="server" visible="true" value="Request OTP" onclick="javascript: requestOtp();" />
                    </div>
                </div>
              </div>
            </dd>
          </dl>
      </div>
    </div>
    <div class="row">
        <div class="large-12 medium-12 small-12 columns" style="margin: 0.75rem 0">
            <input id="btnClientActions" type="button" runat="server" value="Create New" onclick="javascript: validateClientForm();" class="button tiny radius" />
            <input id="btnCancelClientUpdate" type="button" runat="server" value="Cancel" onclick="javascript: cancelClientUpdate();" class="button tiny radius" />
        </div>
    </div>
    <div id="base" class="clearfix"></div>

    <input id="hiddenTemplatesXml" runat="server" type="hidden" value="" />

    <input id="hiddenCancelValue" runat="server" type="hidden" value="false" />

    <input id="panelFocusClients" runat="server" type="hidden" value="" />
    <input id="newClientPanelFocus" runat="server" type="hidden" value="false" />

    <input id="hiddenRefreshTimerPaused" type="hidden" value="false" />
    <input id="hiddenRefreshEventTypeList" type="hidden" value="true" />
    <input id="hiddenSelectedEventIds" type="hidden" />
    <input id="hiddenChkOpenRegistrationStatus" runat="server" type="hidden" value="" />
    <input id="hiddenClientList" runat="server" type="hidden" value="" />

</asp:Content>
