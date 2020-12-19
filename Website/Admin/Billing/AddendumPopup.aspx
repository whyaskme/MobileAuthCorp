<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AddendumPopup.aspx.cs" Inherits="MACBilling.AddendumPopup" ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Add Addendum</title>

    <link href="../../App_Themes/CSS/foundation_menu.css" rel="stylesheet" />
    <link href="../../App_Themes/CSS/Billing.css" rel="stylesheet" />
    <link href="../../App_Themes/CSS/chosen.css" rel="stylesheet" />

    <script src="/Javascript/MACSystemAdmin-Responsive.js"></script>
    <script src="/Javascript/jquery-1.10.2.js"></script>
    <script type="text/javascript">
        function callParentDocumentFunction()
        {
            window.parent.parent.hideJQueryDialog();

            var parentForm = window.parent.parent.document.getElementById("formMain");
            parentForm.submit();
        }

        function saveAddendum()
        {
            var btnText = document.getElementById("btnSave").value;

            document.getElementById("hiddenAA").value = btnText;
            document.getElementById("form1").submit();
        }

        function deleteAddendum() {
            document.getElementById("hiddenAA").value = "Delete";
            document.getElementById("form1").submit();
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
                <h3 style="font-size:1rem;font-weight:bold;margin-top:0.5rem;" id="spanAddendum" runat="server">Miscellaneous Charges</h3>
            </div>
        </div>

        <div class="row" style="margin-bottom: 0.75rem; position: relative; top: -25px; margin-bottom: -20px;">
            <div class="large-12 medium-12 small-12 columns" style="width: 100%; text-align: right;">
                <a href="javascript: NavigateTopicPopup('54ac90347904fb17c0dacffc');" id="link_help_miscCharges">Help?</a>
            </div>
        </div>

        <div id="panel3" class="content">

            <div id="divAddendumCollection" runat="server" class="row" style="width: 100%; border: solid 0px #ff0000; margin-bottom: 15px;">
                <div class="large-12 medium-12 small-12 columns">
                    <asp:DropDownList ID="dlAddendums" AutoPostBack="true" runat="server" CssClass="chosen-select" OnSelectedIndexChanged="dlAddendums_SelectedIndexChanged"></asp:DropDownList>
                </div>
                <hr style="margin-top:0;margin-bottom:1rem; position: relative; top: 15px;" />
            </div>

            <div class="row">
                <div class="large-6 medium-6 small-12 columns">
                    <label id="lblAmount"><span id="spanAmount">Amount</span>
                        <input id="txtAmount" lbl="Amount" isrequired="true" isvalid="false" min-length="1" max-length="75" matchpattern="^\$-?\d+((,\d{3})+)?(\.\d+)?$" patterndescription="$, dash, digits, commas and periods are allowed" type="text" runat="server" 
                            onblur="javascript: validateFormFields(this);" 
                            onkeyup="javascript: validateFormFields(this);"
                            onchange="javascript: validateFormFields(this);" />
                    </label>
                </div>
                <div class="large-6 medium-6 small-12 columns">
                    <label>Notes
                        <textarea id="txtNotes" runat="server" cols="20" rows="10"></textarea>
                    </label>
                </div>
            </div>

            <div style="padding:.25rem;"></div>
            <div class="row" style="width: 100%; text-align: center;">
                <input id="btnDelete" type="button" runat="server" disabled="disabled" onclick="javascript: deleteAddendum();" value="Delete" class="button tiny radius" />
                <input id="btnSave" type="button" runat="server" onclick="javascript: saveAddendum();" value="Create" class="button tiny radius" />
                <input id="btnCancel" type="button" onclick="javascript: cancelChanges();" value="Cancel" class="button tiny radius" />          
            </div>

            <input id="hiddenAA" type="hidden" runat="server" value="" />

        </div>     

    </form>
</body>
</html>
