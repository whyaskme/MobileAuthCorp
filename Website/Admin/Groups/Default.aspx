<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/AdminConsole.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Admin.Groups.Default" %>

<%@ Register Src="~/UserControls/GroupManagement.ascx" TagPrefix="uc1" TagName="GroupManagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ScriptContainer" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BodyContent" Runat="Server">
    <div class="row">
            <uc1:GroupManagement runat="server" ID="GroupManagement" />
    </div>
</asp:Content>

