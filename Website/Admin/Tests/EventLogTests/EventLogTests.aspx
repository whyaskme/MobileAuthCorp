<%@ Page Title="MAC Otp System Administration"
    Language="C#"
    ValidateRequest="false"
    MasterPageFile="~/MasterPages/AdminConsole.master" 
    AutoEventWireup="true" 
    CodeFile="EventLogTests.aspx.cs"
    Inherits="MACUserApps_Web_Tests_LogTesting_EventLogTests" %>

<%@ Register Src="~/UserControls/Menu-Test.ascx" TagPrefix="uc1" TagName="MenuTest" %>

<asp:Content ID="BodyContainer" ContentPlaceHolderID="BodyContent" Runat="Server">

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

    <div style="padding: 0.75rem;"></div>
    <div class="row">
        <div class="large-12 medium-12 small-12 columns">
            <h3>Event Log</h3>
        </div>
    </div>

    <div class="row">
        <div class="large-12 columns">
            <fieldset><legend></legend>
                <div class="row">
                    <div class="large-6 medium-6 small-12 columns">
                        <label>
                            <asp:DropDownList CssClass="chosen-select" runat="server" ID="ddlExceptions"/>
                        </label>
                    </div>
                    <div class="large-6 medium-6 small-12 columns">
                        &nbsp;
                    </div>
                </div>
                <div style="padding: 0.5rem;"></div>
                <div class="row">
                    <div class="large-12 columns">
                        <asp:Button CssClass="button tiny radius" ID="btnExecute" runat="server" Text="Execute" toolTip="Send Exception test request to test service." OnClick="btnExecute_Click" />   
                    </div>
                </div>
            </fieldset>
        </div>
    </div>   
    <div style="padding: 0.75rem;"></div> 
    <div class="row">
        <div class="large-12 columns">
            <label><asp:Label ID="lbError" runat="server" Font-Bold="false" ForeColor="Red" Text="Test" /></label>
        </div>
    </div>    
    <div class="row">
        <div class="large-12 columns">
            <asp:TextBox id="tbLog" runat="server" TextMode="MultiLine" Height="350" />
        </div>
    </div>

</asp:Content>