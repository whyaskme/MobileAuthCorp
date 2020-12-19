<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DeleteTestData.aspx.cs" Inherits="MACAdmin_System_DeleteTestData" %>

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
        <div class="row" style="position: relative; top: 10px; margin-bottom: 0px;">
            <div class="large-12 medium-12 small-12 columns" style="width: 100%; text-align: right;">
                <a href="javascript: NavigateTopicPopup('54ee66d8b5655a1fd4adc17d');" id="link_help">Help?</a>
            </div>
        </div>

        <div id="divDBCollectionContainer" runat="server" style="width: 100%; height: auto; text-align: center; border: solid 0 #0000ff;">

            <div id="divMandatoryDrops" runat="server" style="width: 100%; height: 100%; font-style: normal; padding-left: 15px; text-align: center; border-bottom: solid 1px #a99f4e; margin-bottom: 25px; padding-bottom: 15px;">
                Check the data collections you wish to delete.
            </div>

            <div id="divDBCollectionsToDrop" runat="server" style="float: left; width: 100%; height: 100%; padding-left: 15px; text-align: center; border: solid 0 #ffff00; margin-bottom: 15px;">&nbsp;</div>

            <div style="float: left; width: 100%; text-align: center;  position: relative; top: 5px; padding-top: 5px; margin-bottom: 5px;">
                <asp:CheckBox ID="chkBackUpDB" runat="server" Text="Backup database before deleting selected data?" />
            </div>

            <div style="float: left; width: 100%; text-align: center;  position: relative; top: 5px; padding-top: 5px; margin-bottom: 5px;">
                <asp:Button ID="btnDropDBCollections" runat="server" Text="Delete" CssClass="button tiny radius" />
                <input id="btnCancel" type="button" value="Cancel" onclick="javascript: window.parent.parent.hideJQueryDialog();" class="button tiny radius" />
            </div>

        </div>

        <asp:HiddenField ID="hiddenItemsToDropIds" runat="server" Value="" />

    </form>

</body>
</html>
