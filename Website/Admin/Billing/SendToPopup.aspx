<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SendToPopup.aspx.cs" Inherits="MACBilling.SendToPopup" ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Email Provider Config</title>

    <link href="/App_Themes/CSS/foundation_menu.css" rel="stylesheet" />
    <link href="../../App_Themes/CSS/Billing.css" rel="stylesheet" />

    <script src="/Javascript/MACSystemAdmin-Responsive.js"></script>
    <script src="/Javascript/jquery-1.10.2.js"></script>
    <script type="text/javascript">
        function callParentDocumentFunction(billId)
        {
            //alert("billId - " + billId);

            window.parent.parent.hideJQueryDialog();

            //window.parent.parent.document.getElementById("hiddenV").value = billId;

            var parentForm = window.parent.parent.document.getElementById("formMain");
            parentForm.submit();
        }

        function cancelChanges()
        {
            window.parent.parent.hideJQueryDialog();
        }
    </script>
</head>
    <body style="overflow:auto !important;">

    <form id="form1" runat="server">

        <div class="row">
            <div class="large-12 columns">
                <h3 style="font-size:1rem;font-weight:bold;margin-top:0.5rem;" id="spanSendTo" runat="server">Addendum</h3>
            </div>
        </div>

        <div class="row">
            <div class="small-12 columns">
                <hr style="margin-top:0;margin-bottom:1rem;" />
            </div>
        </div>

        <div style="padding:.25rem;"></div>

        <div id="panel3" class="content">
            <div class="row">

                <div class="large-6 medium-6 small-12 columns">
                    <label id="lblAmount"><span id="spanEmailAddresses">Email Addresses (comma delimited list if more than one)</span>
                        <input id="txtEmailAdddresses" runat="server" lbl="EmailAddresses" isrequired="true" isvalid="false" min-length="1" max-length="75" matchpattern="^\$-?\d+((,\d{3})+)?(\.\d+)?$" patterndescription="$, dash, digits, commas and periods are allowed" type="text" runat="server" 
                            onblur="javascript: validateFormFields(this);" 
                            onkeyup="javascript: validateFormFields(this);"
                            onchange="javascript: validateFormFields(this);" />
                    </label>
                </div>
                <div class="large-6 medium-6 small-12 columns">
                    <label>Notes
                        <textarea id="Notes" runat="server" cols="20" rows="10">Thank you for your business, we certainly appreciate it!</textarea>
                    </label>
                </div>
            </div>

            <div style="padding:.25rem;"></div>
            <div class="row" style="width: 100%; text-align: center;">
                <input id="btnSave" type="submit" onclick="javascript: saveAddendum();" value="Send" class="button tiny radius" />
                <input id="btnCancel" type="button" onclick="javascript: cancelChanges();" value="Cancel" class="button tiny radius" />          
            </div>

        </div>   
        
        <asp:HiddenField ID="hiddenV" runat="server" Value="" />  

    </form>
</body>
</html>
