<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AdminPopupGroupAssignment.aspx.cs" Inherits="MACAdmin.Administrators.PopupGroupAssignment" %>

<%@ Register Src="~/UserControls/AdminAssignmentGroup.ascx" TagPrefix="uc1" TagName="AdminAssignment" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Assign Administrator(s) to Group</title>
</head>
<body>
    <uc1:AdminAssignment runat="server" id="AdminAssignment" />
</body>
</html>
