
<%@ Page Title="Admin Tiered Model Tests"
    Language="C#"
    ValidateRequest="false"
    MasterPageFile="~/MasterPages/AdminConsole.master"
    AutoEventWireup="true" 
    CodeFile="TieredTest.aspx.cs"
    Inherits="Admin_Tests_TieredModelTests_TieredTest" %>

<%@ Register Src="~/UserControls/Menu-Test.ascx" TagPrefix="uc1" TagName="MenuTest" %>


<asp:Content ID="BodyContainer" ContentPlaceHolderID="BodyContent" Runat="Server">
    <div style="padding: 0.25rem;"></div>
    <div class="row">
        <div class="large-3 medium-3 small-12 columns hide-for-small">
            &nbsp;
        </div>
        <div class="large-6 medium-6 small-12 columns">
            <uc1:MenuTest runat="server" ID="MenuTest" />
        </div>
        <div class="large-3 medium-3 small-12 columns hide-for-small">
            &nbsp;
        </div>
    </div>
    <div style="padding: 0.25rem;"></div>
    <div class="row">
        <fieldset><legend>OTP Charges</legend>
        <div class="large-2  medium-2 small-12 columns">
            <label>Tier 1 (count:Value)
                <asp:TextBox ID="OTPT1" runat="server" Text="9:0.09" />
            </label>
        </div>
        <div class="large-2 medium-2 small-12 columns">
            <label>Tier 2 (count:Value)
                <asp:TextBox ID="OTPT2" runat="server" Text="19:0.08" />
            </label>
        </div>
        <div class="large-2 medium-2 small-12 columns">
            <label>Tier 3 (count:Value)
                <asp:TextBox ID="OTPT3" runat="server" Text="29:0.07" />
            </label>
        </div>
        <div class="large-2 medium-2 small-12 columns">
            <label>Tier 4 (count:Value)
                <asp:TextBox ID="OTPT4" runat="server" />
            </label>
        </div>
        <div class="large-2 medium-2 small-12 columns">
            <label>Tier 5 (count:Value)
                <asp:TextBox ID="OTPT5" runat="server" />
            </label>
        </div>
        <div class="large-2 medium-2 small-12 columns">
            <label>Tier 6 (count:Value)
                <asp:TextBox ID="OTPT6" runat="server" />
            </label>
        </div>
        </fieldset>
    </div>
    <div class="row">
        <fieldset><legend>End User Registration Charges</legend>
        <div class="large-2  medium-2 small-12 columns">
            <label>Tier 1 (count:Value)
                <asp:TextBox ID="EUR1" runat="server" Text="9:1.50" />
            </label>
        </div>
        <div class="large-2 medium-2 small-12 columns">
            <label>Tier 2 (count:Value)
                <asp:TextBox ID="EUR2" runat="server" Text="19:1.00" />
            </label>
        </div>
        <div class="large-2 medium-2 small-12 columns">
            <label>Tier 3 (count:Value)
                <asp:TextBox ID="EUR3" runat="server" Text="29:.50"/>
            </label>
        </div>
        <div class="large-2 medium-2 small-12 columns">
            <label>Tier 4 (count:Value)
                <asp:TextBox ID="EUR4" runat="server" />
            </label>
        </div>
        <div class="large-2 medium-2 small-12 columns">
            <label>Tier 5 (count:Value)
                <asp:TextBox ID="EUR5" runat="server" />
            </label>
        </div>
        <div class="large-2 medium-2 small-12 columns">
            <label>Tier 6 (count:Value)
                <asp:TextBox ID="EUR6" runat="server" />
            </label>
        </div>
        </fieldset>
    </div>
    <div class="row">
        <fieldset><legend>Ads Sent Charges</legend>
        <div class="large-2  medium-2 small-12 columns">
            <label>Tier 1 (count:Value)
                <asp:TextBox ID="AS1" runat="server" Text="9:2.50"/>
            </label>
        </div>
        <div class="large-2 medium-2 small-12 columns">
            <label>Tier 2 (count:Value)
                <asp:TextBox ID="AS2" runat="server" />
            </label>
        </div>
        <div class="large-2 medium-2 small-12 columns">
            <label>Tier 3 (count:Value)
                <asp:TextBox ID="AS3" runat="server" />
            </label>
        </div>
        <div class="large-2 medium-2 small-12 columns">
            <label>Tier 4 (count:Value)
                <asp:TextBox ID="AS4" runat="server" />
            </label>
        </div>
        <div class="large-2 medium-2 small-12 columns">
            <label>Tier 5 (count:Value)
                <asp:TextBox ID="AS5" runat="server" />
            </label>
        </div>
        <div class="large-2 medium-2 small-12 columns">
            <label>Tier 6 (count:Value)
                <asp:TextBox ID="AS6" runat="server" />
            </label>
        </div>
        </fieldset>
  
    </div>    
    <div class="row">
        <div class="large-12  medium-12 small-12 columns">
            <label>Serialized
                <asp:TextBox ID="txtSer" runat="server" TextMode="MultiLine" Height="25" />
            </label>
        </div>
    </div>
    <div class="row">
        <div class="large-12 columns">
            <asp:Button CssClass="button tiny radius" runat="server" ID="btnSerialize" Text="Serialize" OnClick="btnSerialize_Click"/>
            <asp:Button CssClass="button tiny radius" runat="server" ID="btnClearLog" Text="Clear" OnClick="btnClearLog_Click"/>
        </div>
    </div>
    <asp:Label ID="lbError" runat="server" Font-Bold="true" ForeColor="Red" />
    <div class="row">
        <div class="large-12 columns">
            <asp:TextBox id="tbLog" runat="server" TextMode="MultiLine" Height="350" />
        </div>
    </div>
</asp:Content>