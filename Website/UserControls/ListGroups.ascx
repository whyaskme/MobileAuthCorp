<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ListGroups.ascx.cs" Inherits="UserControls_ListGroups" %>

<%--<asp:DropDownList ID="dlGroups" runat="server"></asp:DropDownList>--%>

<select id="dlGroups" runat="server" onchange="javascript: alert('Get group info from service');"></select>