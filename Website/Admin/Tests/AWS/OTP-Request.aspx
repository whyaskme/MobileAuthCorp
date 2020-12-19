<%@ Page Language="C#" AutoEventWireup="true" 
    CodeFile="OTP-Request.aspx.cs" 
    Inherits="Admin.Tests.AWS.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>AWS Request OTP</title>
</head>
<body>
    <form id="form1" runat="server">
        <div id="divOTPRequestResult" runat="server" style="color: #ff0000;">
            <asp:DropDownList runat="server" ID="ddlType" AutoPostBack="True" OnSelectedIndexChanged="ddlType_Changed"/>
            <asp:Label runat="server" ID="lbSelectedUser"></asp:Label>
            <asp:Label runat="server" ID="lbError" />
            <br />
            <asp:Label runat="server" ID="lbState" ForeColor="Pink" />
            <div id="divUsers" runat="server" style="line-height:1.0em; font-size: 8pt;"></div>
        </div>
        <input id="hiddenT" runat="server" type="hidden" value="" />
        <input id="hiddenE" runat="server" type="hidden" value="" />
        <input id="hiddenCID" runat="server" type="hidden" value="" />
    </form>
</body>
</html>
