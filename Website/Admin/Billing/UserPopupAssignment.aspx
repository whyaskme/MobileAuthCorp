<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UserPopupAssignment.aspx.cs" Inherits="MACAdmin.Administrators.UserPopupAssignment" %>

<%@ Register Src="~/UserControls/BillingUserAssignment.ascx" TagPrefix="uc1" TagName="AdminAssignment" %>
<%@ Reference Page="~/Admin/Clients/ClientAssignmentPopup.aspx" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Assign Administrator(s) to Client</title>
</head>
<body style="overflow:auto !important;">
    <uc1:AdminAssignment runat="server" id="AdminAssignment" />
</body>
</html>
