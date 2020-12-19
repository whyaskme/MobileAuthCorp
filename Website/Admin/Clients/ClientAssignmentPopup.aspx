<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ClientAssignmentPopup.aspx.cs" Inherits="MACAdmin.Clients.ClientAssignmentPopup" %>
<%@ Register Src="~/UserControls/AdminAssignmentClient.ascx" TagPrefix="uc1" TagName="AdminAssignmentClient" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Assign Client to Group(s)</title>
</head>
<body>
    <uc1:AdminAssignmentClient runat="server" ID="AdminAssignmentClient" />
</body>
</html>
