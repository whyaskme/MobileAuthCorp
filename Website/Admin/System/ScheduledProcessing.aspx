<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ScheduledProcessing.aspx.cs" Inherits="Admin_System_ScheduledProcessing" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Database Collections to Drop</title>

    <link href="/App_Themes/CSS/jquery-ui-smoothness.css" rel="stylesheet" />
    
    <script src="/JavaScript/jquery-1.10.2.js"></script>
    <script src="/JavaScript/jquery-ui-1-11-0.js"></script>
    <script src="/JavaScript/jquery.timer.js"></script>
    <script src="/JavaScript/jquery.validate.js"></script>

    <link href="/App_Themes/CSS/Admin.css" rel="stylesheet" />
    <link href="/App_Themes/CSS/table-style.css" rel="stylesheet" />    

    <link rel="stylesheet" href="/App_Themes/CSS/style.css" />
    <link rel="stylesheet" href="/App_Themes/CSS/prism.css" />
    <link rel="stylesheet" href="/App_Themes/CSS/chosen.css" />

    <link rel="shortcut icon" href="/Images/favicon.ico" />
    <link rel="stylesheet" href="/App_Themes/CSS/foundation_menu.css" />

    <script type="text/javascript" src="/Javascript/Constants.js"></script>
    <script type="text/javascript" src="/Javascript/MACSystemAdmin-Responsive.js"></script>
    <script type="text/javascript" src="/Javascript/jquery.cookie.js"></script>
    <script lang="" type="text/javascript">

        //alert("Here");

        function callParentDocumentFunction(updateMsg) {

            //alert(updateMsg);

            window.parent.parent.hideJQueryDialog();
        }

        function cancelChanges() {
            window.parent.parent.hideJQueryDialog();
        }
    </script>
</head>
<body>

    <form id="form1" runat="server" style="position: relative; top: 20px;">

        <div id="spanProviderName" runat="server" style="position: relative; padding-left: 15px;"></div>

        <div id="divDBCollectionContainer" runat="server" style="width: 100%; height: auto; text-align: center; border: solid 0 #0000ff;">
            <div class="row" id="scroll3">
                <div class="large-12 medium-12 small-12 columns">
                    <asp:DropDownList ID="dlClients" runat="server" AutoPostBack="True" CssClass="chosen-select">
                        <asp:ListItem Value="000000000000000000000000">Select a Client</asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>
            <div id="divUpdateStatus" runat="server" style="float: left; width: 100%; height: 100%; padding-left: 15px; text-align: center; border: solid 0 #ffff00; margin-bottom: 15px;">&nbsp;</div>

            <div id="divButtons" runat="server" style="float: left; width: 100%; text-align: center;  position: relative; top: 15px; padding-top: 15px; margin-bottom: 25px;">
                <input id="btnCancel" type="button" value="Close" onclick="javascript: window.parent.parent.hideJQueryDialog();" class="button tiny radius" />
            </div>

        </div>
    </form>

</body>
</html>
