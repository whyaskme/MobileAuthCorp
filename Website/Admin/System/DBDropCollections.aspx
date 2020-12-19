<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DBDropCollections.aspx.cs" Inherits="MACAdmin_System_DBDropCollections" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Database Collections to Drop</title>
    <%--<link href="/App_Themes/CSS/Admin.css" rel="stylesheet" />--%>
    <link href="../../App_Themes/CSS/Admin.css" rel="stylesheet" />
    <link href="../../App_Themes/CSS/foundation_menu.css" rel="stylesheet" />

    <script src="/Javascript/MACSystemAdmin-Responsive.js"></script>
    <script type="text/javascript">
        function callParentDocumentFunction() {
            window.parent.parent.hideJQueryDialog();
            window.parent.parent.location = "/Admin/Security/Logoff.aspx?msg=You must log back in after a system reset!";
        }
    </script>
</head>
<body>

    <form id="form1" runat="server" style="position: relative; top: 20px;">

        <%--<div id="spanProviderName" runat="server" style="position: relative; padding-left: 15px;"></div>--%>

        <div class="row" style="margin-bottom: 0.75rem; position: relative; top: 0px; margin-bottom: 10px;">
            <div class="large-12 medium-12 small-12 columns" style="width: 100%; text-align: right;">
                <a href="javascript: NavigateTopicPopup('54ee66a2b5655a1fd4adc17a');" id="link_help">Help?</a>
            </div>
        </div>

        <div id="divDBCollectionContainer" runat="server" style="width: 100%; height: auto; text-align: center; border: solid 0 #0000ff;">

            <div id="divMandatoryDrops" runat="server" style="width: 100%; height: 100%; font-size: 13px; font-style: normal; padding-left: 15px; text-align: center; border-bottom: solid 1px #a99f4e; margin-bottom: 25px; padding-bottom: 15px;">&nbsp;</div>

            <div style="width: 100%; font-size: 13px; margin-bottom: 15px;">Check each additional collection you want removed.</div>

            <div id="divDBCollectionsToDrop" runat="server" style="width: 100%; height: 100%; padding-left: 15px; text-align: center; border: solid 0 #ffff00; margin-bottom: 15px;">&nbsp;</div>

            <div style="color: #ffffff;"><hr /></div>

            <div style="width: 100%; text-align: center;  position: relative; top: 15px; padding-top: 15px; margin-bottom: 25px;">
                <asp:Button ID="btnDropDBCollections" runat="server" Text="Reset System Database" CssClass="button tiny radius" />
                &nbsp;
                <input type="button" id="btn_cancel" value="Cancel" onclick="javascript: window.parent.parent.hideJQueryDialog();" class="button tiny radius" />
            </div>

        </div>

        <asp:HiddenField ID="hiddenItemsToDropIds" runat="server" Value="" />

    </form>

</body>
</html>
