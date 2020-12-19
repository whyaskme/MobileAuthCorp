<%@ Page Title="MAC Otp System Administration"
    Language="C#" 
    ValidateRequest="false"
    MasterPageFile="~/MasterPages/AdminConsole.master"
    AutoEventWireup="true" 
    CodeFile="MBTests.aspx.cs"
    Inherits="Admin_Tests_MessageBroadcastTests_MBTests" %>

<%@ Register Src="~/UserControls/Menu-Test.ascx" TagPrefix="uc1" TagName="MenuTest" %>


<asp:Content ID="BodyContainer" ContentPlaceHolderID="BodyContent" Runat="Server">
    
    <script>
    $().ready(function() {
        $('#cbDeliverSms43458').click(function () {
            $('#cbDeliverSms62667').prop('checked', false);
            $('#cbDeliverVoice').prop('checked', false);
        });
        $('#cbDeliverSms62667').click(function () {
            $('#cbDeliverSms43458').prop('checked', false);
            $('#cbDeliverVoice').prop('checked', false);
        });
        $('#cbDeliverVoice').click(function () {
            $('#cbDeliverSms62667').prop('checked', false);
            $('#cbDeliverSms43458').prop('checked', false);
        });

        $('#cbNoReply').click(function () {
            $('#cbSendAndWaitforReply').prop('checked', false);
            $('#cbLoopbackReplyatAPI').prop('checked', false);
            $('#LoopbackReplyatMB').prop('checked', false);
        });
        $('#cbLoopbackReplyatAPI').click(function () {
            $('#cbNoReply').prop('checked', false);
            $('#LoopbackReplyatMB').prop('checked', false);
            $('#cbSendAndWaitforReply').prop('checked', false);
        });
        $('#LoopbackReplyatMB').click(function () {
            $('#cbNoReply').prop('checked', false);
            $('#cbLoopbackReplyatAPI').prop('checked', false);
            $('#cbSendAndWaitforReply').prop('checked', false);
        });
        $('#cbSendAndWaitforReply').click(function () {
            $('#cbNoReply').prop('checked', false);
            $('#cbLoopbackReplyatAPI').prop('checked', false);
            $('#LoopbackReplyatMB').prop('checked', false);
        });
    });
    </script>

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
            <h3>Message Broadcast</h3>
        </div>
    </div>
    
    <div class="row">
        <div class="large-6 medium-6 small-12 columns">
            <div style="float:left; margin-right: 150px;">
                <label>Phone Number</label>
                <asp:TextBox id="txtPhoneNumber" runat="server" />
            </div>
            
            <div style="float:left;">
                <label>Delivery Method</label>
                <asp:CheckBox id="cbDeliverSms43458" runat="server" Text="Sms(43458)" Checked="True" />
                <asp:CheckBox id="cbDeliverSms62667" runat="server" Text="Sms(62667)" />
                <asp:CheckBox id="cbDeliverVoice" runat="server" Text="Voice(not Implemented)" />
            </div>
        </div>
    </div>
    <div class="row">
        <div class="large-12 medium-12 small-12 columns">
            <label>Test Type</label>
                <asp:CheckBox id="cbNoReply" runat="server" Text="No Reply" Checked="True" />
                <asp:CheckBox id="cbLoopbackReplyatAPI" runat="server" Text="Loopback Reply at API" />
                <asp:CheckBox id="LoopbackReplyatMB" runat="server" Text="Loopback Reply at MB" />
                <asp:CheckBox id="cbSendAndWaitforReply" runat="server" Text="Send and wait for Reply" />
        </div>
    </div>
    <div class="row">
        <div class="large-12 columns">
            <label>Test Message Text
                <asp:TextBox id="txtMessage" runat="server" TextMode="MultiLine" Height="125" />
            </label>
        </div>
    </div>
    <div class="row">
        <div class="large-12 columns">
            <label>
                <asp:Button CssClass="button tiny radius" ID="btnUseAPI" runat="server" Text="Use API Call" OnClick="btnUseAPI_Click" />
<%--                <asp:Button CssClass="button tiny radius" ID="btnIssueWebRequest" runat="server" Text="Issue Web Request" OnClick="btnIssueWebRequest_Click" />
                <asp:Button CssClass="button tiny radius" ID="btnIssueWebRequestXML" runat="server" Text="Issue Web Request(XML)" OnClick="btnIssueWebRequestXML_Click" />
                <asp:Button CssClass="button tiny radius" ID="btnDecode1" runat="server" Text="Decode JSON (COLUMNS/DATA)" OnClick="btndecodeJSON_Click" />
                <asp:Button CssClass="button tiny radius" ID="btnDecode2" runat="server" Text="Decode JSON (ERROR)" OnClick="btndecodeJSON_Click" />
                <asp:Button CssClass="button tiny radius" ID="btnDecodeXML" runat="server" Text="Decode XML" OnClick="btndecodeXML_Click" />--%>
            </label>
        </div>
    </div>
    <div class="row">
    <div class="large-12 columns">
        <label>
            <asp:Label ID="lbError" runat="server" Font-Bold="false" ForeColor="Red" Text="" />
        </label>
    </div>
    </div>
    <div style="padding: 0.5rem;"></div>
    <div class="row">
        <div class="large-12 columns" style="margin-bottom: 0.25rem;">
                <asp:Button CssClass="button tiny radius" ID="btnClearLog" runat="server" Text="Clear Log" OnClick="btnClearLog_Click"/>
        </div>
    </div>
    <div class="row">
        <div class="large-12 columns">
                <asp:TextBox id="tbLog" runat="server" TextMode="MultiLine" Height="350" />
        </div>
    </div>
</asp:Content>
