<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UploadFile.aspx.cs" Inherits="System.UploadFile" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Upload File</title>

    <link href="../../App_Themes/CSS/foundation_menu.css" rel="stylesheet" />
    <link href="../../App_Themes/CSS/Admin.css" rel="stylesheet" />

    <script src="/Javascript/MACSystemAdmin-Responsive.js"></script>
    <script type="text/javascript">
        function callParentDocumentFunction() {
            window.parent.parent.document.getElementById("formMain").submit();
            //window.parent.parent.hideJQueryDialog();
        }

        function cancelDocumentFunction() {
            window.parent.parent.hideJQueryDialog();
        }
    </script>
</head>
<body>
    <%--<form id="formMain" runat="server" method="post" onsubmit="javascript: ShowProcessingMessage();">--%>
    <div id="divUpload" runat="server">
        <form id="formMain" runat="server" method="post">
            <div style="padding: 0.625rem;"></div>
            <div class="row">
                <div class="large-12 medium-12 small-12 columns">
                    <label>Please select a file to upload
                        <br /><br />
                        <asp:FileUpload ID="FileUpload1" runat="server" />
                    </label>
                </div>
            </div>

            <div style="padding: 0.5rem;"></div>
            <div class="row">
                <div class="large-12 columns">
                    <input id="btnUpload" type="submit" runat="server" value="Upload" class="button tiny radius" onclick="" />
                    <input id="btnCancel" type="button" runat="server" value="Cancel" class="button tiny radius" onclick="javascript: cancelDocumentFunction();" />
                </div>
            </div>

            <input id="hiddenOwnerId" type="hidden" runat="server" value="" />
            <input id="hiddenOwnerType" type="hidden" runat="server" value="" />

        </form>
    </div>
</body>
</html>
