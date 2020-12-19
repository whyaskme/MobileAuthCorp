<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GroupSelectionPopup.aspx.cs" Inherits="MACAdmin.Groups.GroupSelectionPopup" %>

<%@ Register Src="~/UserControls/GroupSelection.ascx" TagPrefix="uc1" TagName="GroupSelection" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Select a Group to manage</title>
</head>
<body>
    <uc1:GroupSelection runat="server" ID="GroupSelection" />
</body>
</html>
