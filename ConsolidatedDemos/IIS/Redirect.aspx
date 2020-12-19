<%@ Page Language="C#" 
    AutoEventWireup="true" 
    CodeFile="Redirect.aspx.cs" 
    Inherits="Redirect" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Invalid landing page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label runat="server" Text="Url: " />
        &nbsp;&nbsp;
        <asp:Label runat="server" ID="lbUrl" />
        <br />
        <asp:Label runat="server" Text="http: " />
        &nbsp;&nbsp;
        <asp:Label runat="server" ID="lbhttp" />
        <br />
        <asp:Label runat="server" Text="Page Requested: " />
        &nbsp;&nbsp;
        <asp:Label runat="server" ID="lbPageRequested" />
        <br />
        <asp:Label runat="server" Text="Target Url:" /> 
        &nbsp;&nbsp;
        <asp:Label runat="server" ID="lbTargetUrl" />
        <br />
        <asp:Label runat="server" Text="List from config: " />
        &nbsp;&nbsp;
        <asp:Label runat="server" ID="lbList" />
        <br />
        <asp:Label runat="server" Text="Key: " />
        &nbsp;&nbsp;
        <asp:Label runat="server" ID="lbKey" />
        <br />
        <asp:Label runat="server" Text="Client Name:" />
        &nbsp;&nbsp;
        <asp:Label runat="server" ID="lbClientName" />
        <br />
        <asp:Label runat="server" ID="lbError" ForeColor="Red" />
    </div>
    </form>
</body>
</html>
