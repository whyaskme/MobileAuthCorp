<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UserAssignmentPopup.aspx.cs" Inherits="MACAdmin.Clients.UserAssignmentPopup" %>
<%@ Register Src="~/UserControls/UserAssignmentClient.ascx" TagPrefix="uc1" TagName="UserAssignmentClient" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Assign User to Client</title>
</head>
<body>
    <uc1:UserAssignmentClient runat="server" ID="UserAssignmentClient" />
</body>
</html>
