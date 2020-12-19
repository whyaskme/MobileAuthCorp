<%@ Page Title="MAC Otp System Administration" Language="C#"
    ValidateRequest="false"
    MasterPageFile="~/MasterPages/AdminConsole.master" 
    AutoEventWireup="true" 
    CodeFile="TestData.aspx.cs" 
    Inherits="MACUserApps.Web.Tests.TestData.MACUserApps_Web_Tests_TestData_TestData" %>

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
    <div style="padding: 0.825rem;"></div>
    <div class="row">
        <div class="large-12 columns">
            <fieldset><legend>Test Data</legend>
                <div class="row">
                    <div class="large-6 medium-6 small-12 columns">
                        <label>Files
                            <asp:DropDownList CssClass="chosen-select" runat="server" ID="ddlTestFiles" />
                        </label>
                    </div>
                    <div class="large-6 medium-6 small-12 columns">
                        &nbsp;
                    </div>
                </div>
                <div style="padding: 0.5rem;"></div>
                <div class="row">
                    <div class="large-12 columns">
                        <asp:Button CssClass="button tiny radius" ID="btnProcessFile" runat="server" Text="Process File" OnClick="btnProcessFile_Click" />
                        <asp:Button CssClass="button tiny radius" ID="btnNext" runat="server" Text="Next Line" OnClick="btnNext_Click" Enabled="false"/>
                    </div>
                </div>
            </fieldset>
        </div>
    </div>
    <div style="padding: 0.25rem;"></div>
    <div class="row">
        <div class="large-12 columns">
            <asp:Label ID="lbLineNumber" runat="server" Font-Bold="false" />
        </div>
    </div>
    <div style="padding: 0.25rem;"></div>
    <div class="row">
        <div class="large-12 columns">
            <asp:Label ID="lbError" runat="server" Font-Bold="false" ForeColor="Red" Text="Test" />
        </div>
    </div>
    <div style="padding: 0.25rem;"></div>
    <div class="row">
        <div class="large-12 columns">
            <asp:TextBox id="tbLog" runat="server" TextMode="MultiLine" Height="350" />
        </div>
    </div>
</asp:Content>