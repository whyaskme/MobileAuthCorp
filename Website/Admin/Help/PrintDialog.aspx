<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PrintDialog.aspx.cs" Inherits="MACAdmin.Help.PrintDialog" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Client Advertising Settings</title>
    <link href="/App_Themes/CSS/Admin.css" rel="stylesheet" />
    <link href="/App_Themes/CSS/foundation_menu.css" rel="stylesheet" />
    <script src="/Javascript/MACSystemAdmin-Responsive.js"></script>
    <script src="/Javascript/jquery-1.10.2.js"></script>
    <script type="text/javascript">

        function cancelPrint() {
            window.parent.parent.hideJQueryDialog();
        }

        function updatePrintSelection()
        {
            // Pass user selection to parent document print output
            var optionSelected = $('input[name="PrintSelection"]:checked').val();

            //alert("updatePrintSelection - " + optionSelected);
            
            window.parent.parent.hideJQueryDialog();
            window.parent.parent.PrintDocumentation(optionSelected);
        }

    </script>
</head>
    <body style="overflow:auto !important;">

    <form id="form1" runat="server">

        <div class="row" style="text-align: center; padding-top: 50px;">
            <div class="large-12 medium-12 small-12 columns">
                Please select the content you want to print.
            </div>
            <div class="large-12 medium-12 small-12 columns" style="text-align: center; padding-top: 15px;">
                <table style="width: 100%; border: solid 0px #f00;">
                    <tr>
                        <td style="width: 50%;">
                            &nbsp;
                        </td>
                        <td style="white-space: nowrap; padding-right: 15px;">
                            <input id="rdTOC" name="PrintSelection" type="radio" value="TableOfContents" style="position: relative; top: 3px;" />
                            Table of Contents
                        </td>
                        <td style="white-space: nowrap; padding-right: 0px;">
                            <input id="rdCC" name="PrintSelection" type="radio" value="CompleteContent" checked="checked" style="position: relative; top: 3px;" />
                            Complete Topic Contents
                        </td>
                        <td style="width: 50%;">
                            &nbsp;
                        </td>
                    </tr>
                </table>
            </div>
        </div>

        <div class="row">
            <div class="large-12 columns" style="text-align: center;">
                <input id="btnSave" type="button" runat="server" class="button tiny radius" value="Print" onclick="javascript: updatePrintSelection();" />
                <input id="btnCancel" type="button" runat="server" class="button tiny radius" value="Cancel" onclick="javascript: cancelPrint();" />
            </div>
        </div>

        <input id="hiddenAA" type="hidden" runat="server" value="" />

    </form>

</body>
</html>
