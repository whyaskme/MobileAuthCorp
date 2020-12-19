<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GroupAssignmentPopup.aspx.cs" Inherits="MACAdmin.Groups.AssignmentPopup" %>

<%@ Register Src="~/UserControls/GroupAssignment.ascx" TagPrefix="uc1" TagName="GroupAssignment" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Assign Client to Group(s)</title>
</head>
<body style="overflow:auto !important;">
    <uc1:GroupAssignment runat="server" ID="GroupAssignment" />
</body>
</html>
