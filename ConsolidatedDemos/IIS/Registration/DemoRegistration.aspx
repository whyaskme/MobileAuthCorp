<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DemoRegistration.aspx.cs" Inherits="OtpApDemos.DemoRegistration" %>

<%--<%@ Register TagPrefix="recaptcha" Namespace="Recaptcha" Assembly="Recaptcha" %>--%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>MAC Registration</title>
    <link href="/css/jquery-ui-smoothness.css" rel="stylesheet" />
    <link href="/css/jquery-ui-smoothness.css" rel="stylesheet" />
    <script src="/js/jquery-1.10.2.js"></script>
    <script src="/js/jquery-ui-1-11-0.js"></script>
    <script src="/js/jquery.timer.js"></script>
    <script src="/js/jquery.validate.js"></script>

    <link href="/css/Admin.css" rel="stylesheet" />
    <link href="/css/table-style.css" rel="stylesheet" />    

    <link rel="stylesheet" href="/css/style.css" />
    <link rel="stylesheet" href="/css/prism.css" />
    <link rel="stylesheet" href="/css/chosen.css" />

    <%--<link rel="shortcut icon" href="../../Images/favicon.ico" />--%>
    <link rel="stylesheet" href="/css/foundation_menu.css" />

    <script type="text/javascript" src="/js/Constants.js"></script>
    <script type="text/javascript" src="/js/MACSystemAdmin-Responsive.js"></script>
    <script type="text/javascript" src="/js/jquery.cookie.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div id="divRegForm" runat="server">
            <div class="row">
                <div class="large-12 columns">
                    <img src="/Images/mac-logo_80.png" alt="Mobile Authentication Corporation" style="margin: 0.5rem 0;" />
                </div>
            </div>
            <div class="row" id="scroll2">
                <div class="large-12 columns">
                    <fieldset><legend><span class="label_875">Registration</span></legend>
                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns">
                                <label>First Name
                                    <asp:TextBox ID="txtFirstName" runat="server" />
                                </label>
                            </div>
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Last Name
                                    <asp:TextBox ID="txtLastName" runat="server" />
                                </label>
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Company
                                    <asp:TextBox ID="Company" runat="server" />
                                </label>
                            </div>
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Job Title
                                    <asp:TextBox ID="JobTitle" runat="server" />
                                </label>
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Mobile Phone
                                    <asp:TextBox ID="txtMPhoneNo" runat="server" />
                                </label>
                            </div>
                            <div class="large-6 medium-6 small-12 columns">
                                <label>Email
                                    <asp:TextBox ID="txtEmailAdr" runat="server" />
                                </label>
                            </div>
                        </div>

                        <%--<div style="padding: 0.25rem;"></div>
                        <div class="row">
                            <div class="large-12 columns">
                                <img src="http://www.dynapass.com/captcha/cimg/23.jpg" alt="" />
                            </div>
                        </div>--%>

                        <div style="padding: 0.25rem;"></div>
                        <div class="row">
                            <div class="large-3 medium-3 small-12 columns">
                                   <%-- <recaptcha:RecaptchaControl ID="recaptcha" runat="server" PublicKey="6LeuI_sSAAAAAEDqV58nkVj6jZltkgQfOoIlLRTL" PrivateKey="6LeuI_sSAAAAAHx8CdPfWDkpU0-iWQ_RUsbkpEkF" />--%>
                            </div>
                            <div class="large-9 medium-9 small-12 columns hide-for-small">
                                &nbsp;                       
                            </div>
                        </div>

                        <%--<div style="padding: 0.25rem;"></div>
                        <div class="row">
                            <div class="large-12 columns">
                                <span id="myId" runat="server"></span><br />
                                <input id="myText" type="text" runat="server" value="" />
                            </div>
                        </div>--%>

                        <div style="padding: 0.25rem;"></div>
                        <div class="row">
                            <div class="large-12 columns">
                                <asp:Button CssClass="button tiny radius" ID="btnRegisterEndUser" runat="server" Text="Register" OnClick="btnRegisterEndUser_Click" tooltip="Click to register for demo."/>
                                <asp:Button CssClass="button tiny radius" ID="btnTestNavBack" runat="server" Text="Back" OnClick="btnTestNavBack_Click" />

                                <%--<asp:Button CssClass="button tiny radius" ID="buttonTest" runat="server" Text="Test" OnClick="btnTest_Click" />--%>
                            </div>
                        </div>
                    </fieldset>
                </div>
            </div>
        </div>
                
        <div id="divCfgForm" runat="server">  
            <div class="row">
                <div class="large-12 columns">
                    <img src="/Images/mac-logo_80.png" alt="Mobile Authentication Corporation" style="margin: 0.5rem 0;" />
                </div>
            </div>
            <div class="row" id="scroll4">
                <div class="large-12 columns">
                    <fieldset><legend><span class="label_875">Client Administration</span></legend>
                        <div class="row">
                            <div class="large-6 medium-6 small-12 columns">
                                <label>MAC Test Bank URL
                                    <asp:TextBox ID="txtMacBankUrl" runat="server" />
                                </label>
                            </div>
                            <div class="large-6 medium-6 small-12 columns hidden-for-small">
                                <label>RegistrationType
                                    <asp:TextBox ID="txtRegType" runat="server" />
                                </label>
                            </div>
                        </div>
                        <div style="padding: 0.25rem;"></div>
                        <div class="row">
                            <div class="large-12 columns">
                                <asp:Button CssClass="button tiny radius" ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" tooltip=""/>
                                <asp:Button CssClass="button tiny radius" ID="Button1" runat="server" Text="Back" OnClick="btnTestNavBack_Click" /> 
                            </div>
                        </div>
                    </fieldset>
                </div>
            </div>
        </div>
        <div style="padding: 0.5rem;"></div>
        <div class="row">
            <div class="large-12 columns">
                <label><asp:Label ID="lbError" runat="server" ForeColor="Red" Text="" /></label>
            </div>
        </div>
        <div class="row hide-for-small">
            <div class="large-12 columns">
                <div class="copyright">
                    <script type="text/javascript">
                        <!--
                        var currentDate = new Date();
                        var year = currentDate.getFullYear();
                        document.write("&copy; " + year + " Mobile Authentication Corporation. All rights reserved.");
                        //-->
                    </script>
                </div>
            </div>
        </div>
        <div class="row show-for-small">
            <div class="large-12 columns">
                <div class="copyright">
                    <script type="text/javascript">
                        <!--
                        var currentDate = new Date();
                        var year = currentDate.getFullYear();
                        document.write("&copy; " + year + " Mobile Authentication Corporation." + "<br />" + "All rights reserved.");
                        //-->
                    </script>
                </div>
            </div>
        </div>

        <input id="panelFocusClients" runat="server" type="hidden" value="" />
        <input id="hiddenCID" runat="server" type="hidden" value="" />
        <input id="hiddenDemo" runat="server" type="hidden" value="" />
        <input id="hiddenT" runat="server" type="hidden" value="" />
        <input id="hiddenCallerURL" runat="server" type="hidden" value="" />
        <input id="hiddenMACBankUrl" runat="server" type="hidden" value="" />
        <input id="hiddenRegistrationType" runat="server" type="hidden" value="" />
        </div>
    </form>
    <script src="/js/jquery-1.10.2.js"></script>
    
</body>
</html>
