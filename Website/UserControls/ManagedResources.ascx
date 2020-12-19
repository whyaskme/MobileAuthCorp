<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ManagedResources.ascx.cs" Inherits="UserControls.ManagedResources" %>

<link href="/App_Themes/CSS/Admin.css" rel="stylesheet" />
<link href="/App_Themes/CSS/foundation_menu.css" rel="stylesheet" />
<script src="/Javascript/MACSystemAdmin-Responsive.js"></script>

<%--        <div>
            <h4 id="h3AdminManagedResources" runat="server" style="display:none;">&nbsp;</h4>

            <div style="position: relative; margin-bottom: 20px; width: 100%; text-align: center; position: relative; top: 5px;">
                Click to view or edit an item
            </div>--%>
            <div class="large-6 medium-6 small-12 columns">
                <div id="divMyGroups" runat="server" class="MyResourcesContainer" style=""></div>
                <div class="show-for-small" style="padding-bottom: 1rem;"></div>
            </div>
            <div class="large-6 medium-6 small-12 columns">
                <div id="divMyClients" runat="server">
                    <asp:ListBox ID="lstMyClients" runat="server" CssClass="MyResourcesContainer word_wrap">
                        <asp:ListItem>My Clients</asp:ListItem>
                    </asp:ListBox>
                </div>
            </div>            

            <%--<div id="divMyLast5Events" runat="server">
                <asp:ListBox ID="lstMyLast5Events" runat="server" Width="100%" Height="155">
                    <asp:ListItem>My Last (5) Events - Show Local Time</asp:ListItem>
                </asp:ListBox>
            </div>--%>
        <%--</div>--%>



<%--<div>

    <asp:Panel ID="pnlManagedResources" CssClass="manRes" runat="server">
        <div>
            <h4 id="h3AdminManagedResources" runat="server" style="display:none;">&nbsp;</h4>

            <div style="position: relative; margin-bottom: 20px; width: 100%; text-align: center; position: relative; top: 5px;">
                Click to view or edit an item
            </div>

            <div id="divMyGroups" runat="server" class="MyResourcesContainer">
            </div>

            <div id="divMyClients" runat="server" style="margin-bottom: 10px;">
                <asp:ListBox ID="lstMyClients" CssClass="noBg" runat="server" Width="100%" Height="100%">
                    <asp:ListItem>My Clients</asp:ListItem>
                </asp:ListBox>
            </div>

            <div id="divMyLast5Events" runat="server" style="margin-bottom: 10px;">
                <asp:ListBox ID="lstMyLast5Events" CssClass="noBg" runat="server" Width="100%" Height="100%">
                    <asp:ListItem>My Last (5) Events - Show Local Time</asp:ListItem>
                </asp:ListBox>
            </div>
        </div>

    </asp:Panel>

</div>--%>

<%--<div class="divFormControlsContainer" style="position: relative; top: 0;  width: 350px; min-height: 100px;">

    <asp:Panel ID="pnlManagedResources" CssClass="" runat="server">

        <h3 id="h3AdminManagedResources" runat="server">Managed Resources</h3>

        <div style="position: relative; margin-bottom: 20px; width: 100%; text-align: center; position: relative; top: 5px;">
            Click to view or edit an item
        </div>

        <div id="divMyGroups" runat="server" class="MyResourcesContainer">
        </div>

        <div id="divMyClients" runat="server" style="border-bottom: solid 0 #00ff00; width: 100%; height: 98px; padding: 0; margin-bottom: 10px;">
            <asp:ListBox ID="lstMyClients" runat="server" Width="100%" Height="100%">
                <asp:ListItem>My Clients</asp:ListItem>
            </asp:ListBox>
        </div>

        <div id="divMyLast5Events" runat="server" style="border-bottom: solid 0 #00ff00; width: 100%; height: 98px; padding: 0; margin-bottom: 10px;">
            <asp:ListBox ID="lstMyLast5Events" runat="server" Width="100%" Height="100%">
                <asp:ListItem>My Last (5) Events - Show Local Time</asp:ListItem>
            </asp:ListBox>
        </div>

    </asp:Panel>

</div>--%>