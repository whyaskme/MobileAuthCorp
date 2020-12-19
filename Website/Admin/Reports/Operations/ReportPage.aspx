<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ReportPage.aspx.cs" Inherits="Admin_Reports_Operations_ReportPage" %>

<%--<!DOCTYPE html>--%>
<%--<html xmlns="http://www.w3.org/1999/xhtml">--%>

<?xml version="1.0" encoding="utf-8"?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
<head runat="server">
    <title>Report</title>
</head>
<body>
    <form id="form1" runat="server">
        <div id="Contentdiv" runat="server">
            Problem retriving document from database;
        </div>
        <div id="xmldiv" runat="server">
            <asp:Literal ID="lit1" runat="server" />
        </div>
        <div id="pngdiv" runat="server">
                <asp:Literal ID="Literal1" runat="server" />
        </div>
    </form>
</body>
</html>
