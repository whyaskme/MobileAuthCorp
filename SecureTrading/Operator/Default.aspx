<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Operator Test Client</title>

    <link href="css/foundation.css" rel="stylesheet" />
    <link href="css/main.css" rel="stylesheet" />

    <script src="js/jquery-1.10.2.js"></script>
    <script src="js/STIntegration.js"></script>

</head>
<body onload="javascript: InitDisplay();">
    <form id="form1" runat="server">
    <div class="container">

        <div id="divServerInfo" runat="server" style="width: 100%; text-align: center; padding-bottom: 15px; border-bottom: solid 1px #e4e4e4;">
            Server Info: 
        </div>

        <div id="divDeleteStateObjects" style="padding:0.25rem 0 1rem; height: auto; border-bottom: solid 1px #e4e4e4; padding: 15px; margin-bottom: 15px; text-align: center; vertical-align: middle !important;">
            <asp:Button ID="Button1" runat="server" Text="Reset" OnClick="btnReset_Click" />
            <asp:Button ID="Button2" runat="server" Text="Delete States" OnClick="btnDeleteStateObjects_Click"
                ToolTip="Deletes all 'Wrapper State Documents' saved in the database" />
            <asp:Button ID="Button3" runat="server" Text="Clear Log File" OnClick="btnClearLogFile_Click" 
                ToolTip="Deletes today's log file." />
            <b>&nbsp;-&nbsp;File Logging:</b>
            <asp:CheckBox runat="server" ID="cbLogToFile" Text="Enabled" ToolTip="Logging to file, Checked enables logging, Unchecked disables logging."/>
            <b>&nbsp;-&nbsp;Loopback:&nbsp;Disabled</b>
            <input type="radio" id="rbst" runat="server" name="TestCase" value="0" onchange="javascript: SetLoopbackOption('0');" />
            <b>&nbsp;&nbsp;Text Case:</b>
            &nbsp;
            <b>1</b><input type="radio" id="rblt1" runat="server" name="TestCase" value="1" onclick="javascript: SetLoopbackOption('1');" />
            <b>2</b><input type="radio" id="rblt2" runat="server" name="TestCase" value="2" onchange="javascript: SetLoopbackOption('2');" />
            <b>3</b><input type="radio" id="rblt3" runat="server" name="TestCase" value="3" onchange="javascript: SetLoopbackOption('3');" />
            <b>4</b><input type="radio" id="rblt4" runat="server" name="TestCase" value="4" onchange="javascript: SetLoopbackOption('4');" />
            &nbsp;&nbsp
            <asp:Button ID="Button5" runat="server" Text="New Player" />
            <br />
            <div id="divDeleteResult" runat="server" style="border: solid 0 #ff0000;">Delete result...</div>
        </div>

        <div id="divOperatorContainer" runat="server" style="margin-bottom:1rem;border:1px solid #e4e4e4; padding-bottom: 0.5rem;">
            <div style="font-weight: bold; margin-bottom: 10px; background:#e4e4e4;padding:0.25rem;">Operator Info</div>

            <div style="width: 100%; height: 25px; border: solid 0 #ff0000; text-align: center;">
                <div style="width: 100%;">
                    <select style="width: 100%;" id="dlOperatorSites" runat="server" onchange="javascript: SetOperatorSiteInfo();"></select>
                </div>
            </div>
            
            <div id="divOperatorInfo" runat="server" >
                <div style="width: 100%; height: 25px;">                    
                    <div style="float: left; width: 50%; height: 25px;">
                        <div style="white-space: nowrap;">
                            <div style="float: left; width: 150px; text-align: right;">Operator Id&nbsp;</div>
                            <asp:Label runat="server" ID="lbOperatorId" />
                        </div>
                    </div>
                    <div style="float: left; width: 50%; height: 25px;">
                        <div style="white-space: nowrap;">
                            <div style="float: left; width: 150px; text-align: right;">Site Id&nbsp;</div>
                            <asp:Label runat="server" ID="lbSiteId" />
                        </div>
                    </div>
                </div>
                <div style="width: 100%; height: 25px;">  
                    <div style="float: left; width: 50%; height: 25px;">
                        <div style="white-space: nowrap;">
                            <div style="float: left; width: 150px; text-align: right;">Site Username&nbsp;</div>
                            <input type="text" id="txtSiteUsername" runat="server" value="" />
                        </div>
                    </div>
                    <div style="float: left; width: 50%; height: 25px;">
                        <div style="white-space: nowrap;">
                            <div style="float: left; width: 150px; text-align: right;">Site Password&nbsp;</div>
                            <input type="text" id="txtSiteUserPassword" runat="server" value="" />
                        </div>
                    </div>
                </div>
                <div style="width: 100%; height: 25px;">    
                    <div style="float: left; width: 50%; height: 25px;">
                        <div style="white-space: nowrap;">
                            <div style="float: left; width: 150px; text-align: right;">Geo Info&nbsp;</div>
                            <input type="text" id="txtGeoInfo" runat="server" value="" />
                        </div>
                    </div>
                    <div style="float: left; width: 30%; height: 25px;">
                        <div style="white-space: nowrap;">
                            <div style="float: left; width: 150px; text-align: right;">Authorized IP&nbsp;</div>
                            <input style="width: 110px;" type="text" id="txtAuthorizedIP" runat="server" value="" />
                        </div>
                    </div>
                                
                    <div style="width: 20%; height: 25px;">
                        <div style="white-space: nowrap;">
                            <asp:Button ID="btnUpdateOperatorSettings" runat="server" Text="Update Settings" OnClick="btnUpdateOperatorSettings_Click" />
                        </div>
                    </div>
                </div>
            </div> <!-- DivOperatorInfo -->
        </div> <!-- divOperatorContainer -->

        <!-- divServiceOperationsContainer -->
        <div id="divServiceOperationsContainer" runat="server" style="margin-bottom:1rem;border:1px solid #e4e4e4; padding-bottom: 0.5rem;">
            <div style="font-weight: bold; margin-bottom: 10px; background:#e4e4e4;padding:0.25rem;">Service Operations</div>

            <div style="width: 100%; height: 50px;">
                <div id="div11" runat="server" style="margin:0.5rem 0;text-align:center;">
                    <asp:Label runat="server" ID="lbError" ForeColor="Red" />
                </div>
                <div id="div12" runat="server" style="margin:0.5rem 0;text-align:center;">
                    <div id="divStatus" runat="server" style="font-weight: normal; color: #808080; margin-bottom: 5px;">Status</div>
                </div>
                <div style="float: left; width: 100%; height: 25px;">
                    <div style="float: left; width: 148px; text-align: right;">&nbsp;</div>

                    <div id="divWorkflows" runat="server" style="float: left; border: solid 0 #ff0000; margin-right: 15px;">

                        <asp:DropDownList ID="dlWorkflows" runat="server" CssClass="" AutoPostBack="True" OnSelectedIndexChanged="dlWorkflows_SelectedIndexChanged">
                            <asp:ListItem Value="Select">Select a Workflow</asp:ListItem>
                                <asp:ListItem Value="RegisterPlayer">1 Player Registration - OTP</asp:ListItem>
                                <asp:ListItem Value="PlayerLogin">2 Player Login - OTP</asp:ListItem>
                                <asp:ListItem Value="PreCheckSiteValidatePlayerRequest">3 Site Validation and Player Validation - Passthru</asp:ListItem>
                                <asp:ListItem Value="PreCheckSiteCardRequest">4 Card Registration - Passthru</asp:ListItem>
                                <asp:ListItem Value="DeleteRegisteredCard">5 Delete Registered Card - Passthru</asp:ListItem>
                                <asp:ListItem Value="RegisterPrePaidAccount">6 Register Prepaid Account - Passthru</asp:ListItem>
                                <asp:ListItem Value="LoadPrePaidFunds">7 Load Prepaid Funds - OTP</asp:ListItem>
                                <asp:ListItem Value="SubmitPrePaidDeposit">8 Submit Prepaid Deposit - OTP</asp:ListItem>
                                <asp:ListItem Value="SubmitDepositAccountRequest">9 Submit Deposit Account Request - OTP</asp:ListItem>                         
                                <asp:ListItem Value="SubmitPrePaidWithdrawal">10 Submit Prepaid Withdrawal - OTP</asp:ListItem>
                                <asp:ListItem Value="GetTransactionDetails">11 GetTransactionDetails - Passthru</asp:ListItem>
                                <asp:ListItem Value="UpdateTransaction">12 UpdateTransaction - Passthru</asp:ListItem>
                                <asp:ListItem Value="GetRegisteredAccounts">13 GetRegisteredAccounts - Passthru</asp:ListItem>                             
                                <asp:ListItem Value="GetRegisteredCards">14 GetRegisteredCards - Passthru</asp:ListItem>
                                <asp:ListItem Value="GetPlayerSSN">15 GetPlayerSSN - Passthru</asp:ListItem>
                                <asp:ListItem Value="UpdatePlayerSSN">16 UpdatePlayerSSN - Passthru</asp:ListItem>
                                <asp:ListItem Value="GetPrePaidAccountHolderInfo">17 GetPrePaidAccountHolderInfo - Passthru</asp:ListItem>
                                <asp:ListItem Value="GetPrePaidAccountBalance">18 GetPrePaidAccountBalance - Passthru</asp:ListItem>                         
                                <asp:ListItem Value="GetPrePaidAccountCVV2">19 GetPrePaidAccountCVV2 - Passthru</asp:ListItem>
                                <asp:ListItem Value="PreCheckSiteAccountRequest">20 PreCheckSiteAccountRequest - Passthru</asp:ListItem>
                                <asp:ListItem Value="PrePaidRegisterAndLoad">21 PrePaidRegisterAndLoad - OTP</asp:ListItem> 
                                <asp:ListItem Value="UpdatePrePaidAccountStatus">22 UpdatePrePaidAccountStatus</asp:ListItem> 
                                <asp:ListItem Value="SubmitDepositRequest">23 SubmitDepositRequest - OTP</asp:ListItem>
                                <asp:ListItem Value="AccountWithdrawal">24 AccountWithdrawal - OTP</asp:ListItem>
                                <asp:ListItem Value="SelfExcludePlayer">25 stapiSelfExcludePlayer</asp:ListItem>
                                <asp:ListItem Value="ModifyPlayerRequest">26 stapiModifyPlayerRequest</asp:ListItem>
                                <asp:ListItem Value="AuthDepositRequest">27 stapiAuthDepositRequest</asp:ListItem>
                                <asp:ListItem Value="SubmitRefund">28 stapiSubmitRefund</asp:ListItem>
                                <asp:ListItem Value="AddSelfExcludePlayer">29 stapiAddSelfExcludePlayer</asp:ListItem>              
                                <asp:ListItem Value="Withdrawal">30 stapiWithdrawal - OTP</asp:ListItem>
                                <asp:ListItem Value="SubmitReversal">31 stapiSubmitReversal</asp:ListItem>                             
                            <asp:ListItem Value="Select">--------- -------------</asp:ListItem>                            
                                <asp:ListItem Value="CombinedPlayerRegistrationPrePaidDeposit">Combined Player Registration & Pre Paid Deposit - OTP</asp:ListItem>
                        </asp:DropDownList>
                        <asp:DropDownList ID="dlPlayer" runat="server" CssClass="" AutoPostBack="True" OnSelectedIndexChanged="dlPlayer_SelectedIndexChanged">
                            <asp:ListItem Value="Select a Player">Select a Player</asp:ListItem>
                        </asp:DropDownList>

                        <div id="divActionButtons" runat="server" style="float: left; border: solid 0 #ff0000; position: relative; top: -2px; padding-left: 15px;">
                            <asp:Button ID="btnSubmitSTDirect" runat="server" Text="ST Direct" OnClick="btnSubmitSTDirect_Click" />
                            <asp:Button ID="btnSubmitWCFService" runat="server" Text="Wrapper Service" OnClick="btnSubmitToWrapperService_Click" />
                        </div>
                        
                    </div>
                </div>
            </div>

        </div> 

        <div style="padding:0.625rem;"></div>

        <div id="divPlayerContainer" runat="server" style="margin-bottom:1rem;padding-bottom:0.5rem;border:1px solid #e4e4e4;">
            <div style="font-weight: bold;margin-bottom: 10px;background:#e4e4e4;padding:0.25rem;">Player Details</div>

            <div id="div4" runat="server" style="margin-bottom:0.5rem;padding-left: 15px; font-weight: normal; color: #808080;"><h2>Player</h2></div>

            <div style=" width: 100%; height: 25px;">
                <div style="float: left; width: 50%; height: 25px;">
                    <div style="white-space: nowrap;">
                        <div style="float: left; width: 150px; text-align: right;">First Name&nbsp;</div>
                        <input type="text" id="txtPlayerFirstName" runat="server" value="" />
                    </div>
                </div>
                <div style="float: left; width: 50%; height: 25px;">
                    <div style="white-space: nowrap;">
                        <div style="float: left; width: 150px; text-align: right;">Middle Initial&nbsp;</div>
                        <input type="text" id="txtPlayerMiddleInitial" runat="server" value="" />
                    </div>
                </div>
            </div>
            <div style="width: 100%; height: 25px;margin-bottom:0.5rem;">
                <div style="float: left; width: 50%; height: 25px;">
                    <div style="white-space: nowrap;">
                        <div style="float: left; width: 150px; text-align: right;">Last Name&nbsp;</div>
                        <input type="text" id="txtPlayerLastName" runat="server" value="" />
                    </div>
                </div>
                <div style="float: left; width: 50%; height: 25px;">
                    <div style="white-space: nowrap;">
                        <div style="float: left; width: 150px; text-align: right;">Gender&nbsp;</div>
                        <input type="text" id="txtPlayerGender" runat="server" value="" />
                    </div>
                </div>
            </div>

            <div style=" width: 100%; height: 25px;">                           
                <div style="float: left; width: 50%; height: 25px;">
                    <div style="white-space: nowrap;">
                        <div style="float: left; width: 150px; text-align: right;">SSN&nbsp;</div>
                        <input type="text" id="txtPlayerSSN" runat="server" value="" />
                    </div>
                </div>
                <div style="float: left; width: 50%; height: 25px;">
                    <div style="white-space: nowrap;">
                        <div style="float: left; width: 150px; text-align: right;">DoB&nbsp;</div>
                        <input type="text" id="txtPlayerDoB" runat="server" value="" />
                    </div>
                </div>
            </div>
            <div style=" width: 100%; height: 25px;">                           
                <div style="float: left; width: 50%; height: 25px;">
                    <div style="white-space: nowrap;">
                        <div style="float: left; width: 150px; text-align: right;">Drivers License&nbsp;</div>
                        <input type="text" id="txtPlayerDriversLicenseNumber" runat="server" value="" />
                    </div>
                </div>
                <div style="float: left; width: 50%; height: 25px;">
                    <div style="white-space: nowrap;">
                        <div style="float: left; width: 150px; text-align: right;">State Issued&nbsp;</div>
                        <input type="text" id="txtPlayerDriversLicenseIssueState" runat="server" value="" />
                    </div>
                </div>
            </div>
            <div style=" width: 100%; height: 25px;margin-bottom:0.5rem;">                           
                <div style="width: 25%; height: 25px;">
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="Button4" runat="server" Text="Update Player from Form" OnClick="btnUpdatePlayerFromForm_Click" />
                </div>
            </div>

            <div id="divPlayerAddress" runat="server" style="border-top: solid 0 #e4e4e4; padding-top: 5px;margin-bottom:0.5rem;">
                <div id="divPlayerAddressHeading" runat="server" style="border-top: solid 0 #e4e4e4;margin-bottom:0.5rem; padding-left: 15px; font-weight: normal; color: #808080;"><h3>Address</h3></div>
                <div style=" width: 100%; height: 25px;">
                    <div style="float: left; width: 50%; height: 25px;">
                        <div style="white-space: nowrap;">
                            <div style="float: left; width: 150px; text-align: right;">Address 1&nbsp;</div>
                            <input type="text" id="txtPlayerAddress1" runat="server" value="" />
                        </div>
                    </div>
                    <div style="float: left; width: 50%; height: 25px;">
                        <div style="white-space: nowrap;">
                            <div style="float: left; width: 150px; text-align: right;">Address 2&nbsp;</div>
                            <input type="text" id="txtPlayerAddress2" runat="server" value="" />
                        </div>
                    </div>
                </div>
                <div style=" width: 100%; height: 25px;">
                    <div style="float: left; width: 50%; height: 25px;">
                        <div style="white-space: nowrap;">
                            <div style="float: left; width: 150px; text-align: right;">City&nbsp;</div>
                            <input type="text" id="txtPlayerCity" runat="server" value="" />
                        </div>
                    </div>
                    <div style="float: left; width: 50%; height: 25px;">
                        <div style="white-space: nowrap;">
                            <div style="float: left; width: 150px; text-align: right;">County&nbsp;</div>
                            <input type="text" id="txtPlayerCounty" runat="server" value="" />
                        </div>
                    </div>
                </div>
                <div style=" width: 100%; height: 25px;">                           
                    <div style="float: left; width: 50%; height: 25px;">
                        <div style="white-space: nowrap;">
                            <div style="float: left; width: 150px; text-align: right;">State&nbsp;</div>
                            <input type="text" id="txtPlayerState" runat="server" value="" />
                        </div>
                    </div>
                    <div style="float: left; width: 50%; height: 25px;">
                        <div style="white-space: nowrap;">
                            <div style="float: left; width: 150px; text-align: right;">Zip&nbsp;</div> 
                            <input type="text" id="txtPlayerZip" runat="server" value="" />
                        </div>
                    </div>
                </div>
                <div style=" width: 100%; height: 25px;">                           
                    <div style="float: left; width: 50%; height: 25px;margin-bottom:0.5rem;">
                        <div style="white-space: nowrap;">
                            <div style="float: left; width: 150px; text-align: right;">Country&nbsp;</div>
                            <input type="text" id="txtPlayerCountry" runat="server" value="" />
                        </div>
                    </div>
                    <div style="float: left; width: 50%; height: 25px;margin-bottom:0.5rem;">
                        <div style="white-space: nowrap;">
                            &nbsp;
                        </div>
                    </div>
                </div>
            </div> <!-- divPlayerAddress -->

            <div id="divPlayerContact" runat="server" style="border-top: solid 0 #e4e4e4; padding-top: 5px;margin-bottom:0.5rem;">
                <div id="divPlayerContactHeading" runat="server" style="border-top: solid 0 #e4e4e4;margin-bottom:0.5rem; padding-left: 15px; font-weight: normal; color: #808080;"><h3>Contact</h3></div>
                <div style=" width: 100%; height: 25px;">                           
                    <div style="float: left; width: 50%; height: 25px;">
                        <div style="white-space: nowrap;">
                            <div style="float: left; width: 150px; text-align: right;">Home Phone&nbsp;</div>
                            <input type="text" id="txtPlayerHomePhone" runat="server" value="" />
                        </div>
                    </div>
                    <div style="float: left; width: 50%; height: 25px;">
                        <div style="white-space: nowrap;">
                            <div style="float: left; width: 150px; text-align: right;">Mobile Phone&nbsp;</div>
                            <input type="text" id="txtPlayerMobilePhone" runat="server" value="" />
                        </div>
                    </div>
                </div>
                <div style=" width: 100%; height: 25px;">                           
                    <div style="float: left; width: 50%; height: 25px;">
                        <div style="white-space: nowrap;">
                            <div style="float: left; width: 150px; text-align: right;">Email&nbsp;</div>
                            <input type="text" id="txtPlayerEmail" runat="server" value="" />
                        </div>
                    </div>
                    <div style="float: left; width: 50%; height: 25px;">
                        <div style="white-space: nowrap;">
                            &nbsp;
                        </div>
                    </div>
                </div>
            </div> <!-- divPlayerContact -->

            <div id="divPlayerReference" runat="server" style="border-top: solid 0 #e4e4e4; padding-top: 5px;margin-bottom:0.5rem;">
                <div style="width: 100%; height: 25px; margin-bottom: 25px;">    
                    <div id="divPlayerReferenceInfo" runat="server" style="margin-bottom:0.5rem;padding-left: 15px; font-weight: normal; color: #808080;"><h3>Reference</h3></div>
                    
                    <div style=" width: 100%; height: 25px;">                           
                        <div style="float: left; width: 50%; height: 25px;">
                            <div style="white-space: nowrap;">
                                <div style="float: left; width: 150px; text-align: right;">Username&nbsp;</div>
                                <input type="text" id="txtPlayerUsername" runat="server" value="" />
                            </div>
                        </div>
                        <asp:Button ID="btnUpdatePlayerUserName" runat="server" Text="Update Player's User Name" OnClick="btnUpdatePlayerUserName_Click" />
                    </div>
                           
                    <div style="float: left; width: 50%; height: 25px;">
                        <div style="white-space: nowrap;">
                            <div style="float: left; width: 150px; text-align: right;">Player ID&nbsp;</div>
                            <input type="text" id="txtPlayerPlayerId" runat="server" value="" />
                        </div>
                    </div>             
                    <div style="float: left; width: 50%; height: 25px;">
                        <div style="white-space: nowrap;">
                            <div style="float: left; width: 150px; text-align: right;">Reference Number&nbsp;</div>
                            <input type="text" id="txtPlayerReferenceNumber" runat="server" value="" />
                        </div>
                    </div>
                      
                    <div style="float: left; width: 50%; height: 25px;">
                        <div style="white-space: nowrap;">
                            <div style="float: left; width: 150px; text-align: right;">Connection Token&nbsp;</div>
                            <input type="text" id="txtPlayerConnectionToken" runat="server" value="" />
                        </div>
                    </div>             
                    <div style="float: left; width: 50%; height: 25px;">
                        <div style="white-space: nowrap;">
                            <div style="float: left; width: 150px; text-align: right;">Session Token&nbsp;</div>
                            <input type="text" id="txtPlayerSessionToken" runat="server" value="" />
                        </div>
                    </div>

                </div>
            </div> <!-- divPlayerReference -->

            <div style="clear:both;"></div>

            <div id="div1" runat="server" style="border-top: solid 0 #e4e4e4; padding-top: 5px;margin-bottom:0.5rem;">
                <div style="width: 100%; height: 25px; margin-bottom: 25px;">    
                    <div id="div2" runat="server" style="margin-bottom:0.5rem;padding-left: 15px; font-weight: normal; color: #808080;"><h3>Card Selection</h3></div>

                        <div style="width: 100%; height: 25px;">                    
                            <div style="float: left; width: 50%; height: 25px;">
                                <div style="white-space: nowrap;">
                                    <div style="float: left; width: 150px; text-align: right;">Card Token&nbsp;</div>
                                    <input type="text" id="txtPCICardToken" runat="server" value="" />
                                </div>
                            </div>

                            <div style="float: left; width: 50%; height: 25px;">
                                <div style="white-space: nowrap;">
                                    <div style="float: left; width: 150px; text-align: right;">Account Type&nbsp;</div>
                                    <input style="" type="text" id="txtPCIAccountType" runat="server" value="" />
                                </div>
                            </div>
                        </div>

                        <div style=" width: 100%; height: 25px;">
                            <div style="float: left; width: 50%; height: 25px;">
                                <div style="white-space: nowrap;">
                                    <div style="float: left; width: 150px; text-align: right;">Name On Card&nbsp;</div>
                                    <input type="text" id="txtPCINameOnCard" runat="server" value="" />
                                </div>
                            </div>
                            <div style="float: left; width: 50%; height: 25px;">
                                <div style="white-space: nowrap;">
                                    <div style="float: left; width: 150px; text-align: right;">Card Type&nbsp;</div>
                                    <input type="text" id="txtPCICardType" runat="server" value="" />
                                </div>
                            </div>
                        </div> 
                        <div style=" width: 100%; height: 25px;">
                            <div style="float: left; width: 50%; height: 25px;">
                                <div style="white-space: nowrap;">
                                    <div style="float: left; width: 150px; text-align: right;">Card Number&nbsp;</div>
                                    <input type="text" style="" id="txtPCICardNumber" runat="server" value="" />
                                </div>
                            </div>

                            <div style="float: left; width: 50%; height: 25px;">
                                <div style="white-space: nowrap;">
                                    <div style="float: left; width: 150px; text-align: right;">CVV&nbsp;</div>
                                    <input type="text" id="txtPCICVV" runat="server" value="" />
                                </div>
                            </div>
                        </div>
            
                        <div style=" width: 100%; height: 25px;">
                            <div style="float: left; width: 50%; height: 25px;">
                                <div style="white-space: nowrap;">
                                    <div style="float: left; width: 150px; text-align: right;">Start Date(MM/YYYY)&nbsp;</div>
                                    <input type="text" id="txtPCIStartDate" runat="server" value="" />
                                </div>
                            </div>
                            <div style="float: left; width: 50%; height: 25px;">
                                <div style="white-space: nowrap;">
                                    <div style="float: left; width: 150px; text-align: right;">Expiration Data&nbsp;</div>
                                    <input type="text" id="txtPCIExpiryDate" runat="server" value="" />
                                </div>
                            </div>
                        </div> 

                        <div style=" width: 100%; height: 25px;">
                            <div style="float: left; width: 50%; height: 25px;">
                                <div style="white-space: nowrap;">
                                    <div style="float: left; width: 150px; text-align: right;">Issue Number&nbsp;</div>
                                    <input type="text" id="txtPCIIssueNumber" runat="server" value="" />
                                </div>
                            </div>
                            <div style="float: left; width: 50%; height: 25px;">
                                <div style="white-space: nowrap;">
                                    <div style="float: left; width: 150px; text-align: right;">Default Card(Y/N)&nbsp;</div>
                                    <input type="text" id="txtPCIDefaultCard" runat="server" value="" />
                                </div>
                            </div>
                        </div>

                        <div id="div3" runat="server" style="margin-bottom:0.5rem;padding-left: 15px; font-weight: normal; color: #808080;"><h4>Billing Address</h4></div>

                        <div style=" width: 100%; height: 25px;">
                            <div style="float: left; width: 50%; height: 25px;">
                                <div style="white-space: nowrap;">
                                    <div style="float: left; width: 150px; text-align: right;">Address1&nbsp;</div>
                                    <input type="text" id="txtPCIAddress1" runat="server" value="" />
                                </div>
                            </div>
                            <div style="float: left; width: 50%; height: 25px;">
                                <div style="white-space: nowrap;">
                                    <div style="float: left; width: 150px; text-align: right;">Address 2&nbsp;</div>
                                    <input type="text" id="txtPCIAddress2" runat="server" value="" />
                                </div>
                            </div>
                            <div style=" width: 100%; height: 25px;">
                                <div style="float: left; width: 50%; height: 25px;">
                                    <div style="white-space: nowrap;">
                                        <div style="float: left; width: 150px; text-align: right;">City&nbsp;</div>
                                        <input type="text" id="txtPCICity" runat="server" value="" />
                                    </div>
                                </div>
                                <div style="float: left; width: 50%; height: 25px;">
                                    <div style="white-space: nowrap;">
                                        <div style="float: left; width: 150px; text-align: right;">County&nbsp;</div>
                                        <input type="text" id="txtPCICounty" runat="server" value="" />
                                    </div>
                                </div>
                            </div>
            
                            <div style=" width: 100%; height: 25px;">
                                <div style="float: left; width: 50%; height: 25px;">
                                    <div style="white-space: nowrap;">
                                        <div style="float: left; width: 150px; text-align: right;">State&nbsp;</div>
                                        <input type="text" id="txtPCIState" runat="server" value="" />
                                    </div>
                                </div>
                                <div style="float: left; width: 50%; height: 25px;">
                                    <div style="white-space: nowrap;">
                                        <div style="float: left; width: 150px; text-align: right;">Zip Code&nbsp;</div>
                                        <input type="text" id="txtPCIZipCode" runat="server" value="" />
                                    </div>
                                </div>
                            </div>
            
                            <div style=" width: 100%; height: 25px;">
                                <div style="float: left; width: 50%; height: 25px;">
                                    <div style="white-space: nowrap;">
                                        <div style="float: left; width: 150px; text-align: right;">Country&nbsp;</div>
                                        <input type="text" id="txtPCICountry" runat="server" value="UNITED STATES" />
                                    </div>
                                </div>
                                <div style="float: left; width: 50%; height: 25px;">
                                    <div style="white-space: nowrap;">
                                        &nbsp;
                                    </div>
                                </div>
                            </div>
                        </div>

                </div>
            </div>

            <div style="clear:both;"></div>
            <!-- PrePaid Account -->
            <div id="divPrePaidAccount" runat="server" style="border-top: solid 0 #e4e4e4; padding-top: 5px;margin-bottom:0.5rem;">
                <div style="width: 100%; height: 25px; margin-bottom: 25px;">    
                    <div id="div6" runat="server" style="margin-bottom:0.5rem;padding-left: 15px; font-weight: normal; color: #808080;"><h3>Pre-Paid Account</h3></div>

                        <div style=" width: 100%; height: 25px;">
                            <div style="float: left; width: 50%; height: 25px;">
                                <div style="white-space: nowrap;">
                                    <div style="float: left; width: 150px; text-align: right;">Account Token&nbsp;</div>
                                    <input type="text" id="txtPPAToken" runat="server" value="" />
                                </div>
                            </div>
                            <div style="float: left; width: 50%; height: 25px;">
                                <div style="white-space: nowrap;">
                                    <div style="float: left; width: 150px; text-align: right;">Balance</div>
                                    <input type="text" id="txtPPABalance" runat="server" value="" />
                                </div>
                            </div>
                        </div>
            
                        <div style=" width: 100%; height: 25px;">
                            <div style="float: left; width: 50%; height: 25px;">
                                <div style="white-space: nowrap;">
                                    <div style="float: left; width: 150px; text-align: right;">Account Number&nbsp;</div>
                                    <input type="text" id="txtPPANumber" runat="server" value="" />
                                </div>
                            </div>
                            <div style="float: left; width: 50%; height: 25px;">
                                <div style="white-space: nowrap;">
                                    <div style="float: left; width: 150px; text-align: right;">Virtual Card Number&nbsp;</div>
                                    <input type="text" id="txtPPAVirtualCardNumber" runat="server" value="" />
                                </div>
                            </div>
                        </div>
            
                        <div style=" width: 100%; height: 25px;">
                            <div style="float: left; width: 50%; height: 25px;">
                                <div style="white-space: nowrap;">
                                    <div style="float: left; width: 150px; text-align: right;">T&amp;C Accepted&nbsp;</div>
                                    <input type="text" id="txtPPAtandcAccepted" runat="server" value="" />
                                </div>
                            </div>
                            <div style="float: left; width: 50%; height: 25px;">
                                <div style="white-space: nowrap;">
                                    <div style="float: left; width: 150px; text-align: right;">T&amp;C Accept Timestamp&nbsp;</div>
                                    <input type="text" id="txtPPAtandcAcceptedTimestampUTC" runat="server" value="" />
                                </div>
                            </div>
                        </div>
            </div>
            </div>

            <div style="clear:both;"></div>

            <div id="div7" runat="server" style="border-top: solid 0 #e4e4e4; padding-top: 5px;margin-bottom:0.5rem;">
                <div style="width: 100%; height: 25px; margin-bottom: 25px;">    
                    <div id="div8" runat="server" style="margin-bottom:0.5rem;padding-left: 15px; font-weight: normal; color: #808080;"><h3>Bank Account</h3></div>

                        <div style=" width: 100%; height: 25px;">
                            <div style="float: left; width: 50%; height: 25px;">
                                <div style="white-space: nowrap;">
                                    <div style="float: left; width: 150px; text-align: right;">Bank Name&nbsp;</div>
                                    <input type="text" id="txtBankName" runat="server" value="" />
                                </div>
                            </div>
                            <div style="float: left; width: 50%; height: 25px;">
                                <div style="white-space: nowrap;">
                                    <div style="float: left; width: 150px; text-align: right;">Account Type&nbsp;</div>
                                    <input type="text" id="txtBAAccountType" runat="server" value="" />
                                </div>
                            </div>
                        </div>
            
                        <div style=" width: 100%; height: 25px;">
                            <div style="float: left; width: 50%; height: 25px;">
                                <div style="white-space: nowrap;">
                                    <div style="float: left; width: 150px; text-align: right;">ABA Number&nbsp;</div>
                                    <input type="text" id="txtabaNumber" runat="server" value="" />
                                </div>
                            </div>
                            <div style="float: left; width: 50%; height: 25px;">
                                <div style="white-space: nowrap;">
                                    <div style="float: left; width: 150px; text-align: right;">Account Number&nbsp;</div>
                                    <input type="text" id="txtBAAccountNumber" runat="server" value="" />
                                </div>
                            </div>
                        </div>

                </div>
            </div>

            <div style="clear:both;"></div>

            <div id="div9" runat="server" style="border-top: solid 0 #e4e4e4; padding-top: 5px;margin-bottom:0.5rem;">
                <div style="width: 100%; height: 25px; margin-bottom: 25px;">    
                    <div id="div10" runat="server" style="margin-bottom:0.5rem;padding-left: 15px; font-weight: normal; color: #808080;"><h3>Transactions</h3></div>

                        <div style=" width: 100%; height: 25px;">
                            <div style="float: left; width: 50%; height: 25px;">
                                <div style="white-space: nowrap;">
                                    <div style="float: left; width: 150px; text-align: right;">Bank Name&nbsp;</div>
                                    <input type="text" id="Text1" runat="server" value="" />
                                </div>
                            </div>
                            <div style="float: left; width: 50%; height: 25px;">
                                <div style="white-space: nowrap;">
                                    <div style="float: left; width: 150px; text-align: right;">Account Type&nbsp;</div>
                                    <input type="text" id="Text2" runat="server" value="" />
                                </div>
                            </div>
                        </div>
            
                        <div style=" width: 100%; height: 25px;">
                            <div style="float: left; width: 50%; height: 25px;">
                                <div style="white-space: nowrap;">
                                    <div style="float: left; width: 150px; text-align: right;">ABA Number&nbsp;</div>
                                    <input type="text" id="Text3" runat="server" value="" />
                                </div>
                            </div>
                            <div style="float: left; width: 50%; height: 25px;">
                                <div style="white-space: nowrap;">
                                    <div style="float: left; width: 150px; text-align: right;">Account Number&nbsp;</div>
                                    <input type="text" id="Text4" runat="server" value="" />
                                </div>
                            </div>
                        </div>

                </div>
            </div>

            <div style="clear:both;"></div>

        </div>
  

        <div id="divPlayerOperationsContainer" runat="server" style="margin-bottom:1rem;padding-bottom:0.5rem;border:1px solid #e4e4e4;">
            <div style="font-weight: bold;margin-bottom: 10px;background:#e4e4e4;padding:0.25rem;">Player Operations</div>

            <!-- Otp Validate -->
            <div id="divOtpValidate" runat="server" style="padding: 15px; width: 100%; height: 150px; text-align: center; vertical-align: middle;">
                <div style="float: left; width: 50%; height: 25px;">
                    <div style="white-space: nowrap;">
                        <div class="row">
                            <div class="large-12 columns">
                                <div id="divServiceResponse"></div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-12 columns">
                                <div id="spanOtpMessage" runat="server">returnMsg</div>
                            </div>
                        </div>
                        <div style="padding: 0.125rem;"></div>
                        <div id="divOtpControls" runat="server">
                            <div class="row">
                                <div class="large-12 columns">
                                    <input type="text" style="border:1px solid #E4E4E4;" id="txtOtp" runat="server" />
                                </div>
                            </div>
                            <div class="row" style="margin-top: 15px;">
                                <div class="large-12 columns">
                                    <asp:button ID="btnValidate" runat="server" text="Validate Otp" CssClass="tiny button radius" OnClick="btnValidateOtp_Click" />
                                    <asp:button ID="btnResend" runat="server" text="Resend Otp" CssClass="tiny button radius" OnClick="btnResend_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div style="float: left; width: 50%; height: 25px;">
                    <div style="white-space: nowrap;">
                        <div id="divImageAdContainer" runat="server" style="float: left; border: solid 0 #ff0000; width: 365px; height: 165px;" ></div>
                    </div>
                </div>
            </div>
            <!-- Otp Operations -->        
            <div id="divQuestionsContainer"  runat="server" style="border: solid 0 #ff0000;">
                <div id="divQuestions" runat="server" ></div>
                <asp:button ID="btnSubmitAnswers" runat="server" text="Submit Answers" CssClass="tiny button radius" OnClick="btnSubmitAnswers_Click" />
            </div>
        </div> <!-- divPlayerOperationsContainer -->

        <div id="divServiceLogContainer" style="border: solid 0 #ff0000; position: relative; top: 25px;">
            <div style="font-weight: bold; color: #808080;">Service Log</div>
            <div id="divJsonResponseData" runat="server" style="width: 98%; height: 200px; overflow: auto; padding: 10px; border: solid 1px #e4e4e4;"></div>
        </div>

    </div>
    <input type="hidden" id="hiddenSelectedWorkFlow" runat="server" value="" />
    <input type="hidden" id="hiddenOperationToExecute" runat="server" value="" />
    <input type="hidden" id="hiddenPageAction" runat="server" value="" />
    <input type="hidden" id="hiddenLoopbackEnabled" runat="server" value="" />
    <input type="hidden" id="hiddenService" runat="server" value="Wrapper" />
    <input type="hidden" id="hiddenFlow" runat="server" value="none" />
    <input type="hidden" id="hiddenRequestToken" runat="server" value="" />
    <input type="hidden" id="hiddenOperatorId" runat="server" value="0" />
    <input type="hidden" id="hiddenSiteId" runat="server" value="0" />
    <input type="hidden" id="hiddenQuestionsResponseToken" runat="server" value="" />
    <input type="hidden" id="hiddenQuestion1Answer" runat="server" value="1" />
    <input type="hidden" id="hiddenQuestion2Answer" runat="server" value="1" />
    <input type="hidden" id="hiddenQuestion3Answer" runat="server" value="1" />
    <input type="hidden" id="hiddenQuestion4Answer" runat="server" value="1" />
    <input type="hidden" id="hiddenQuestion5Answer" runat="server" value="1" />
    </form>
</body>
</html>
