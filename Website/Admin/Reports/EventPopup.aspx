<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EventPopup.aspx.cs" Inherits="MACAdmin.Reporting.EventPopup" ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Event Report</title>
    <link href="/App_Themes/CSS/Admin.css" rel="stylesheet" />
    <link href="/App_Themes/CSS/foundation_menu.css" rel="stylesheet" />
    <script src="/Javascript/MACSystemAdmin-Responsive.js"></script>
    <script src="../../Javascript/jquery-1.10.2.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $('#divEmailForm').hide();
            $('#divBtnEmailClose').show();
        });
        function callParentDocumentFunction() {
            window.parent.parent.hideJQueryDialog();
        }
        function btnEmailDetails_Click() {
            $('#divEmailForm').show();
            $('#divBtnEmailClose').hide();
        }
        function btnCancelSendEmail_Click() {
            $('#divEmailForm').hide();
            $('#divBtnEmailClose').show();
        }
    </script>
</head>

<body style="overflow:auto !important;">
    <form id="form1" runat="server">
        <div runat="server" id="divDetails">
            <div class="row">
                <div class="large-12 columns" style="padding: 0.5rem 0;">
                    <span class="title" id="spanProviderName" runat="server">Event Details</span>
                </div>
            </div>
            <div class="row">
                <div class="large-12 columns">
                    <div id="divEvents" runat="server" style="overflow-y: auto;"></div>
                </div>
            </div>
            <div class="row">
                <div class="large-12 columns">
                    <asp:Label runat="server" ID="lbError" ForeColor="Red" />
                </div>
            </div>
        </div>
        <div ID="divBtnEmailClose" runat="server" >
            <div class="row">
                <div class="large-12 columns">
                    <input class="tiny button radius" id="btn_emailDetails" type="button" value="Email Details" onclick="javascript: btnEmailDetails_Click();" />
                    <input class="tiny button radius" id="btn_close" type="button" value="Close" onclick="javascript: callParentDocumentFunction();" />
                </div>
            </div>
        </div>
        <div  ID="divEmailForm" runat="server" >
            <div class="row">
                <div class="large-12 columns">
                    <label id="lbSubject"><span id="spanSubject">Subject (Optional)</span>
                        <input id="txtSubject" lbl="lbSubject" isvalid="false"  min-length="0" max-length="50" 
                            matchpattern="^[a-zA-Z0-9_\s]*$" patterndescription="a valid subject" type="text" runat="server" 
                            onblur="javascript: validateFormFields(this);" 
                            onkeyup="javascript: validateFormFields(this);"
                            onchange="javascript: validateFormFields(this);" />
                    </label>
                </div>
            </div>
            <div class="row">
                <div class="large-12 columns">
                    <label id="lblEmail"><span id="spanEmail">Email Address</span>
                        <input id="txtEmail" lbl="Email Address" isrequired="true" isvalid="false" min-length="7" max-length="50" 
                            matchpattern="^([a-z0-9_\.-]+)@([\da-z\.-]+)\.([a-z\.]{2,6})$" patterndescription="a valid email address" type="text" runat="server" 
                            onblur="javascript: validateFormFields(this);" 
                            onkeyup="javascript: validateFormFields(this);"
                            onchange="javascript: validateFormFields(this);" />
                    </label>
                </div>
            </div>
            <div class="row">
                <div class="large-12 columns">
                    <asp:Button runat="server" ID="btnSendEmail" class="tiny button radius" Text="Send Email" onclick="btnSendEmail_Click" />
                    <input class="tiny button radius" id="btn_cancel" type="button" value="Cancel" onclick="javascript: btnCancelSendEmail_Click();" />
                </div>
            </div>
        </div>

        <div class="row">
            <div class="columns">
                <asp:HiddenField ID="hiddenEventId" runat="server" />
                <asp:HiddenField ID="hiddenCallerURL" runat="server" />               
            </div>
        </div>
    </form>

</body>
</html>
