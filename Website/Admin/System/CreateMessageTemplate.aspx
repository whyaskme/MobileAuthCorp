<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CreateMessageTemplate.aspx.cs" Inherits="System.CreateMessageTemplate" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Reset Event Stats</title>

    <link href="../../App_Themes/CSS/foundation_menu.css" rel="stylesheet" />
    <link href="../../App_Themes/CSS/Admin.css" rel="stylesheet" />

    <script src="/JavaScript/jquery-1.10.2.js"></script>

    <script src="/Javascript/MACSystemAdmin-Responsive.js"></script>

    <script type="text/javascript">
        function callParentDocumentFunction() {
            window.parent.parent.hideJQueryDialog();
        }

        function getLastTemplateNumber() {
            //alert("getLastTemplateNumber");
            document.getElementById("hiddenAA").value = "GetLastTemplateNumber";
            document.getElementById("formMain").submit();
        }

        function saveTemplate()
        {
            //alert("saveTemplate");

            //$('#dlMessageType,#txtDesc,#txtDetails,#txtFromAddress,#txtFromName,#btnCreate,#btnCancel').prop('disabled', true);
            $('#btnCreate,#btnCancel').prop('disabled', true);

            document.getElementById("hiddenAA").value = "SaveTemplate";
            document.getElementById("formMain").submit();
        }
    </script>
</head>
<body>
    <form id="formMain" runat="server" method="post">
        <div style="padding: 0.625rem;"></div>
        <div class="row" style="margin-bottom: 0.75rem; position: relative; top: -5px; margin-bottom: -20px;">
            <div class="large-12 medium-12 small-12 columns" style="width: 100%; text-align: right;">
                <a href="javascript: NavigateTopicPopup('553ff491a6e10b006c0cd085');" runat="server" id="link_help">Help?</a>
            </div>
        </div>
        <div class="row">
            <div class="large-6 medium-6 small-12 columns">
                <select id="dlMessageType" runat="server" onchange="javascript: getLastTemplateNumber();">
                    <option selected="selected" value="Select Message Type">Select Message Type</option>
                    <option value="Email">Email</option>
                    <option value="Sms">Sms</option>
                    <option value="Voice">Voice</option>
                </select>
            </div>
            <div class="large-6 medium-6 small-12 columns" style="padding-bottom: 15px;">
                <span id="spanClassName" runat="server" style="font-weight: bold;" />
            </div>
        </div>
        <div id="divDetailInfo" class="row" runat="server">
            <div class="large-6 medium-6 small-12 columns">
                <label>Description
                    <asp:TextBox ID="txtDesc" runat="server"></asp:TextBox>
                </label>
            </div>
            <div class="large-6 medium-6 small-12 columns">
                <label>Message Details
                    <textarea id="txtDetails" runat="server" cols="20" rows="5"></textarea>
                </label>
            </div>
        </div>

        <div class="row" runat="server" id="divFromInfo">
            <div class="large-6 medium-6 small-12 columns">
                <label>From Address
                    <asp:TextBox ID="txtFromAddress" runat="server"></asp:TextBox>
                </label>
            </div>
            <div class="large-6 medium-6 small-12 columns">
                <label>From Name
                    <asp:TextBox ID="txtFromName" runat="server"></asp:TextBox>
                </label>
            </div>
        </div>

        <div class="row">
            <div class="large-12 columns" style="width: 100%; text-align: center;">
                <input id="btnCreate" type="button" runat="server" value="Create" class="button tiny radius" onclick="javascript: saveTemplate();" />
                <input id="btnCancel" type="button" runat="server" value="Cancel" class="button tiny radius" onclick="javascript: callParentDocumentFunction();" />
            </div>
        </div>

        <input type="hidden" id="hiddenAA" runat="server" value="" />

    </form>
</body>
</html>
