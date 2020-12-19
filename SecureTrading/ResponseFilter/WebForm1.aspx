<%@ Page language="c#" Codebehind="WebForm1.aspx.cs" AutoEventWireup="false" Inherits="RemovingWhiteSpacesAspNet.WebForm1" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>WebForm1</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<asp:RadioButtonList id="rblFirst" style="Z-INDEX: 101; LEFT: 27px; POSITION: absolute; TOP: 23px" runat="server">
				<asp:ListItem Value="1">1</asp:ListItem>
				<asp:ListItem Value="2">2</asp:ListItem>
				<asp:ListItem Value="3">3</asp:ListItem>
				<asp:ListItem Value="4">4</asp:ListItem>
				<asp:ListItem Value="5">5</asp:ListItem>
			</asp:RadioButtonList>
			<asp:RadioButtonList id="rblSecond" style="Z-INDEX: 102; LEFT: 105px; POSITION: absolute; TOP: 21px"
				runat="server">
				<asp:ListItem Value="1">1</asp:ListItem>
				<asp:ListItem Value="2">2</asp:ListItem>
				<asp:ListItem Value="3">3</asp:ListItem>
				<asp:ListItem Value="4">4</asp:ListItem>
				<asp:ListItem Value="5">5</asp:ListItem>
			</asp:RadioButtonList>
			<asp:Label id="Label1" style="Z-INDEX: 103; LEFT: 78px; POSITION: absolute; TOP: 74px" runat="server">+</asp:Label>
			<asp:Label id="Label2" style="Z-INDEX: 104; LEFT: 154px; POSITION: absolute; TOP: 70px" runat="server">=</asp:Label>
			<asp:Label id="lblSum" style="Z-INDEX: 105; LEFT: 184px; POSITION: absolute; TOP: 70px" runat="server">[Sum]</asp:Label>
			<asp:Button id="btCalc" style="Z-INDEX: 106; LEFT: 244px; POSITION: absolute; TOP: 27px" runat="server"
				Text="Calculate!"></asp:Button>
			<asp:RequiredFieldValidator id="RequiredFieldValidator1" style="Z-INDEX: 107; LEFT: 244px; POSITION: absolute; TOP: 67px"
				runat="server" ErrorMessage="Select first number" Display="Dynamic" ControlToValidate="rblFirst"></asp:RequiredFieldValidator>
			<asp:RequiredFieldValidator id="RequiredFieldValidator2" style="Z-INDEX: 108; LEFT: 246px; POSITION: absolute; TOP: 95px"
				runat="server" ErrorMessage="Select second number" Display="Dynamic" ControlToValidate="rblSecond"></asp:RequiredFieldValidator>
		</form>
	</body>
</HTML>
