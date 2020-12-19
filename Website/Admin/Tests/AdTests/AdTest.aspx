<%@ Page Title="MAC Otp System Administration"
    Language="C#"
    ValidateRequest="false"
    MasterPageFile="~/MasterPages/AdminConsole.master" 
    AutoEventWireup="true"
    CodeFile="AdTest.aspx.cs" Inherits="MACUserApps_Web_Tests_AdTest_AdTest" %>

<%@ Register Src="~/UserControls/Menu-Test.ascx" TagPrefix="uc1" TagName="MenuTest" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ScriptContainer" Runat="Server">
    <script>
        $().ready(function () {
            //On tab click, set hidden value 'panelFocus'
            $('#Tab1').click(function () {
                $('#panelFocusUsers').val('Tab1');
            });
            $('#Tab2').click(function () {
                $('#panelFocusUsers').val('Tab2');
            });
            $('#Tab3').click(function () {
                $('#panelFocusUsers').val('Tab3');
            });

            //Expand and position current tab on reload
            var currentTab = $('#panelFocusUsers').val();
            if (currentTab == 'Tab1') {               // Init Ad Provider in Client
                $('#' + currentTab + '').click();
                $('html, body').animate({
                    scrollTop: ($('#scroll2').offset().top - 44)
                }, 750, 'easeOutExpo');
            } else if (currentTab == 'Tab2') {               // Get Ads from Constants
                $('#' + currentTab + '').click();
                $('html, body').animate({
                    scrollTop: ($('#scroll2').offset().top)
                }, 750, 'easeOutExpo');
            } else if (currentTab == 'Tab3') {               // Get Ads from Secure Ads
                $('#' + currentTab + '').click();
                $('html, body').animate({
                    scrollTop: ($('#scroll2').offset().top + 46)
                }, 750, 'easeOutExpo');
            }
            $('#divKeywords').hide();
            $('#divProfile').hide();
        });

        function RequestUsageChange() {
            if ($('#rbUseKeywords').is(':checked')) {
                $('#divKeywords').show();
                $('#divProfile').hide();
            } else {
                $('#divKeywords').hide();
            }
            if ($('#rbUseProfileData').is(':checked')) {
                $('#divProfile').show();
                $('#divKeywords').hide();
            } else {
                $('#divProfile').hide();
            }
        }

   </script>
</asp:Content>

<asp:Content ID="BodyContainer" ContentPlaceHolderID="BodyContent" Runat="Server">
    <div class="row">
        <div class="large-3 medium-3 small-12 columns hide-for-small">
            &nbsp;
        </div>
        <div class="large-6 medium-6 small-12 columns">
            <uc1:MenuTest runat="server" ID="MenuTest" />
        </div>
        <div class="large-3 medium-3 small-12 columns hidden-for-medium hidden-for-small">
            &nbsp;
        </div>
    </div>

    <div style="padding: 0.75rem;"></div>
    <div class="row">
        <div class="large-12 medium-12 small-12 columns">
            <h3>AdPass</h3>
        </div>
    </div>

    <div style="padding: 0.125rem;"></div>
    <div class="row" ID="scroll2">
        <div class="large-12 columns">
            
            <dl class="accordion" data-accordion="">
                <dd>
                    <a href="#panel1" id="Tab1"><span>Init Ad Provider in Client</span></a>
                    <div id="panel1" class="content">
                                  
                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Client
                                    <asp:DropDownList CssClass="chosen-select" ID="ddlInitClient" runat="server" 
                                        AutoPostBack="true" OnSelectedIndexChanged="ddlInitClient_Selected" />
                                </label>
                            </div>
                            <div class="large-6 medium-6 small-12 columns">
                                <label><asp:Label ID="lbInitGroup" runat="server" Text="Groups" Font-Bold="true" Visible="false" />
                                    <asp:DropDownList CssClass="chosen-select" ID="ddlInitGroup" runat="server"  Visible="false" />
                                </label>
                            </div>
                        </div>
                        <div style="padding: 0.5rem;"></div>
                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Ad Client Id:
                                <asp:TextBox runat="server" ID="txtInitAdClientId" />
                                </label>
                            </div>
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Ad Campaign Id:
                                <asp:TextBox runat="server" ID="txtInitCampaignId" />
                                </label>
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-12 columns">
                                <asp:Button CssClass="button tiny radius" runat="server" ID="btnInitAdProvider" Text="Init Ad Provider" OnClick="btnInitAdProvider_Click"/>
                            </div>
                            </div>
                    </div>
                </dd>

                <dd>
                    <a href="#panel2" id="Tab2"><span>Get Ads from MAC Test Ad Service</span></a>
                    <div id="panel2" class="content">
                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Client
                                    <asp:DropDownList CssClass="chosen-select" ID="ddlClient2" runat="server" />
                                </label>
                            </div>
                            <div class="large-6 medium-6 small-12 columns">
                                <label><asp:Label ID="Label1" runat="server" Text="Groups" Font-Bold="true" Visible="false" />
                                    <asp:DropDownList CssClass="chosen-select" ID="ddlGroup2" runat="server"  Visible="false" />
                                </label>
                            </div>
                        </div>
                        <div style="padding: 0.5rem;"></div>
                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Ad Number
                                    <asp:DropDownList CssClass="chosen-select" ID="ddlAdNumber" runat="server" />
                                </label>
                            </div>
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Special Keywords
                                    <asp:DropDownList CssClass="chosen-select" ID="ddlSpeKeywords" runat="server" />
                                </label>
                            </div>
                        </div>
                        <div style="padding: 0.5rem;"></div>
                        <div class="row">
                            <div class="large-12 columns">
                                <asp:Button CssClass="button tiny radius" runat="server" 
                                    ID="btnGetTestAd" Text="Get Ad" OnClick="btnShowTestAd_Click"/>
                            </div>
                        </div>
                    </div>
                </dd>

                <dd>
                    <a href="#panel3" id="Tab3"><span>Get Ads from Secure Ads</span></a>
                    <div id="panel3" class="content">                        
                        <div class="row">
                            <div class="large-1 medium-1 small-1 columns">
                                <asp:label runat="server" ID="Label2" Text="Files:" />
                            </div>
                            <div class="large-5 medium-5 small-11 columns">
                                <asp:DropDownList CssClass="chosen-select" ID="ddlTestFiles" runat="server"
                                    AutoPostBack="true" OnSelectedIndexChanged="ddlTestFile_Selected" />
                            </div>
                            <div class="large-6 medium-6 small-12 columns hidden-for-small">
                                &nbsp;
                            </div>
                        </div>
                        <div style="padding: 0.5rem;"></div>
                        <div runat="server" title="Settings" id="divSettings" Visible="false">
                            <div class="row">
                                <div class="large-6 medium-6 small-12 columns">
                                    <label>Client Name:
                                        <asp:label runat="server" ID="lbClientName" Font-Underline="true" />
                                    </label>
                                </div>
                                <div class="large-6 medium-6 small-12 columns">
                                    <label>SA Client Id:
                                        <asp:label runat="server" ID="lbClientId" Font-Underline="true" />
                                    </label>
                                </div>
                            </div>
                            <div style="padding: 0.5rem;"></div>
                            <div class="row">
                                <div class="large-6 medium-6 small-12 columns">
                                    <label>URL:
                                        <asp:label runat="server" ID="lbUrl" Font-Underline="true" />
                                    </label>
                                </div>
                                <div class="large-6 medium-6 small-12 columns">
                                    <label>API Key:
                                        <asp:label runat="server" ID="lbAPIKey" Font-Underline="true" />
                                    </label>
                                </div>
                            </div>
                            <div style="padding: 0.5rem;"></div>
                            <div class="row">
                                <div class="large-6 medium-6 small-12 columns">
                                    <label>API UserName:
                                        <asp:label runat="server" ID="lbAPIUserName" Font-Underline="true" />
                                    </label>
                                </div>
                                <div class="large-6 medium-6 small-12 columns">
                                    <label>API Password:
                                        <asp:label runat="server" ID="lbAPIPassword"  Font-Underline="true" />
                                    </label>
                                </div>
                            </div>
                            <div style="padding: 0.5rem;"></div>
                            <div class="row">
                                <div class="large-6 medium-6 small-12 columns">
                                    <label>Campaigns:
                                        <asp:label runat="server" ID="lbCampaignName" Font-Underline="true" />
                                    </label>
                                </div>
                                <div class="large-6 medium-6 small-12 columns">
                                    <label>Campaign Id:
                                        <asp:label runat="server" ID="lbCampaignId" Font-Underline="true" />
                                    </label>
                                </div>
                            </div>
                            <div style="padding: 0.5rem;"></div>
                            <div class="row">
                                <div class="large-12 medium-12 small-12 columns">
                                    <label>Request Using Client Id and</label>
                                    <asp:RadioButton runat="server" ID="rbNone" GroupName="RequestUsing" Text="Only" Checked="True" />
                                        <asp:RadioButton runat="server" ID="rbUseCampaignId" GroupName="RequestUsing" Text="Campaign ID"/>
                                        <asp:RadioButton runat="server" ID="rbUseCampaignName" GroupName="RequestUsing" Text="Campaign Name" />
                                        <asp:RadioButton runat="server" ID="rbUseProfileData" GroupName="RequestUsing" Text="Profile Data"  />
                                        <asp:RadioButton runat="server" ID="rbUseKeywords" GroupName="RequestUsing" Text="Keywords" />
                                </div>
                            </div>
                        </div>

                        <div runat="server" ID="divKeywords" >
                            <div class="row">
                                <div class="large-6 medium-6 small-12 columns">
                                    <label>Possible Keywords:</label>
                                    <asp:TextBox runat="server" ID="txtPossibleKeywords" Width="400" />
                                </div>
                                <div runat="server" class="large-6 medium-6 small-12 columns">
                                    <label>Enter Keywords here:</label>
                                    <asp:TextBox runat="server" ID="txtKeywords" Width="400" />
                                </div>
                            </div>
                        </div>
                                   
                        <div runat="server" ID="divProfile" >              
                            <div class="row">
                                <div class="large-2 medium-2 small-12 columns">
                                    <label>Age
                                        <asp:TextBox ID="txtAge" runat="server" />
                                    </label>
                                </div>
                                <div class="large-3 medium-3 small-12 columns">
                                    <label>Ethnicity
                                        <asp:TextBox ID="txtEthnicity" runat="server" />
                                    </label>
                                </div>
                                <div class="large-3 medium-3 small-12 columns">
                                    <label>Gender</label>
                                        <asp:RadioButton runat="server" ID="rbMale" GroupName="Gender" Text="Male"/>
                                        <asp:RadioButton runat="server" ID="rbFemale" GroupName="Gender" Text="Female"/>     
                                </div>
                                <div class="large-4 medium-4 small-12 columns">
                                    <label>Marital Status</label>
                                        <asp:RadioButton runat="server" ID="rbSingle" GroupName="MaritalStatus" Text="Single"/>
                                        <asp:RadioButton runat="server" ID="rbMarried" GroupName="MaritalStatus" Text="Married"/>     
                                </div>
                            </div>
                            <div class="row">
                                <div class="large-2 medium-2 small-12 columns">
                                    <label>Home Owner</label>
                                    <asp:CheckBox ID="cbHomeowner" runat="server" Text="Yes" />
                                </div>
                                <div class="large-2 medium-2 small-12 columns">
                                    <label>Household Income
                                        <asp:TextBox ID="txtHouseholdIncome" runat="server" />
                                    </label>
                                </div>
                                <div class="large-4 medium-4 small-12 columns">
                                    <label>City
                                        <asp:TextBox ID="txtCity" runat="server" />
                                    </label>
                                </div>
                                <div class="large-2 medium-2 small-12 columns">
                                    <label>State
                                        <asp:TextBox ID="txtState" runat="server" />
                                    </label>
                                </div>                                    
                                <div class="large-2 medium-2 small-12 columns">
                                    <label>Ad Number</label>
                                    <asp:TextBox runat="server" ID="txtAdNumber" Width="50"/>
                                </div>
                            </div>
                        </div>
                        <div runat="server" title="Settings" id="divButton" Visible="false">     
                            <div class="row">
                                <div class="large-12 columns">
                                    <asp:Button CssClass="button tiny radius" 
                                        runat="server" ID="btnGetAd" Text="Get Ad(Direct)" OnClick="btnGetAdFromSA_Click"/>
                                </div>
                            </div>
                        </div>
                    </div>
                </dd>
            </dl>
                <div class="row">
                    <div class="large-6 medium-6 small-12 columns">
                        <label>Enter Otp Screen Ad</label>
                        <div style="margin-bottom: 1rem;width: 100%; " id="divAdEnterOtpScreenSent" runat="server">
                            goes here
                        </div>
                    </div>
                    <div class="large-6 medium-6 small-12 columns">
                        <label>Mobile Message Ad</label>
                        <asp:Label runat="server" ID="lbMesage" Text="Test"/>
                        &nbsp;
                        <asp:LinkButton runat="server" ID="lkMessage" OnClick="lkMessage_Clicked" />
                    </div>
                </div>
                <div class="row">
                    <div class="large-6 medium-6 small-12 columns">
                        <label>Verification Page Ad</label>
                        <div style="margin-bottom: 1rem;width: 100%; " id="divAdVerificationScreenSent" runat="server">
                            goes here
                        </div>
                    </div>
                    <div class="large-6 medium-6 small-12 columns hidden-for-small hidden-for-medium">
                        &nbsp;
                    </div>
                </div>
        </div>
    </div>
    <div class="row">
        <div class="large-12 columns">
            <asp:Button CssClass="button tiny radius" runat="server" ID="ClearLog" Text="Clear the log window" OnClick="btnClearLog_Click"/>
        </div>
    </div>
    <div class="row">
        <div class="large-12 columns">
            <asp:Label ID="lbError" runat="server" Font-Bold="false" ForeColor="Red" Text="" />
        </div>
    </div>
    <div class="row">
        <div class="large-12 columns">
            <asp:TextBox id="tbLog" runat="server" TextMode="MultiLine" Height="350" />
        </div>
    </div>
    <input id="panelFocusUsers" runat="server" type="hidden" value="" />
</asp:Content>
