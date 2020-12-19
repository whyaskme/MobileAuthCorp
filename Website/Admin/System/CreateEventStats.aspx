<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CreateEventStats.aspx.cs" Inherits="System.CreateEventStats" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Reset Event Stats</title>

    <link href="../../App_Themes/CSS/foundation_menu.css" rel="stylesheet" />
    <link href="../../App_Themes/CSS/Admin.css" rel="stylesheet" />

    <script src="/Javascript/MACSystemAdmin-Responsive.js"></script>
    <script type="text/javascript">
        function callParentDocumentFunction() {
            window.parent.parent.hideJQueryDialog();
        }
    </script>
</head>
<body>
    <%--<form id="formMain" runat="server" method="post" onsubmit="javascript: ShowProcessingMessage();">--%>
    <form id="formMain" runat="server" method="post">
        <div style="padding: 0.625rem;"></div>

        <div class="row" style="margin-bottom: 0.75rem; position: relative; top: -5px; margin-bottom: -30px;">
            <div class="large-12 medium-12 small-12 columns" style="width: 100%; text-align: right;">
                <a href="javascript: NavigateTopicPopup('54ee666db5655a1fd4adc177');" id="link_help">Help?</a>
            </div>
        </div>

        <div class="row">
            <div class="large-6 medium-6 small-12 columns">
                <label>Start Date
                    <asp:TextBox ID="txtStartDate" runat="server"></asp:TextBox>
                </label>
            </div>
            <div class="large-6 medium-6 small-12 columns">
                <label>End Date
                    <asp:TextBox ID="txtEndDate" runat="server"></asp:TextBox>
                </label>
            </div>
        </div>
        <div class="row">
<%--            <div class="large-4 medium-4 small-12 columns">
                <label>Number of Days to Generate
                    <asp:TextBox ID="txtNumberOfDaysToGenerate" runat="server"></asp:TextBox>
                </label>
            </div>--%>
            <div class="large-6 medium-6 small-12 columns">
                <label>Lower Bound
                    <asp:TextBox ID="txtLowerBound" runat="server"></asp:TextBox>
                </label>
            </div>
            <div class="large-6 medium-6 small-12 columns">
                <label>Upper Bound
                    <asp:TextBox ID="txtUpperBound" runat="server"></asp:TextBox>
                </label>
            </div>
        </div>

        <div class="row">
            <hr />
            <div class="large-12 medium-12 small-12 columns">
                <table id="statsTable" runat="server" style="width: 100%; border: none; padding: 0px;">
                    <tr>
                        <td colspan="3" style="width: 100%; padding: 0px; font-size: 14px; padding-bottom: 15px;">
                            We will generate numbers for the following events for every Client in the system.
                        </td>
                    </tr>

                    <tr>
                        <td colspan="3" style="width: 100%; padding: 5px;">
                            &nbsp;
                        </td>
                    </tr>

                    <tr>
                        <td colspan="3" style="width: 100%; padding: 0px; color: #808080;">
                            End User Events (<span id="spanUserEventCount" runat="server">0</span>)
                        </td>
                    </tr>

                    <tr>
                        <td style="width: 33%; padding: 10px;">
                            <span>EndUserRegister</span>
                            <span id="EndUserRegisterCount" runat="server" style="padding-left: 5px; font-weight: bold;">0</span>
                        </td>
                        <td style="width: 33%; padding: 10px;">
                            <span>EndUserVerify</span>
                            <span id="EndUserVerifyCount" runat="server" style="padding-left: 5px; font-weight: bold;">0</span>
                        </td>
                        <td style="width: 33%; padding: 10px;">
                            &nbsp;
                        </td>
                    </tr>

                    <tr>
                        <td colspan="3" style="width: 100%; padding: 5px;">
                            &nbsp;
                        </td>
                    </tr>

                    <tr>
                        <td colspan="3" style="width: 100%; padding: 0px; color: #808080;">
                            OTP Sent Events (<span id="spanOtpSentEventCount" runat="server">0</span>)
                        </td>
                    </tr>

                    <tr>
                        <td style="width: 33%; padding: 10px;">
                            <span>OtpSentSms</span>
                            <span id="OtpSentSmsCount" runat="server" style="padding-left: 5px; font-weight: bold;">0</span>
                        </td>
                        <td style="width: 33%; padding: 10px;">
                            <span>OtpSentEmail</span>
                            <span id="OtpSentEmailCount" runat="server" style="padding-left: 5px; font-weight: bold;">0</span>
                        </td>
                        <td style="width: 33%; padding: 10px;">
                            <span>OtpSentVoice</span>
                            <span id="OtpSentVoiceCount" runat="server" style="padding-left: 5px; font-weight: bold;">0</span>
                        </td>
                    </tr>

                    <tr>
                        <td colspan="3" style="width: 100%; padding: 5px;">
                            &nbsp;
                        </td>
                    </tr>

                    <tr>
                        <td colspan="3" style="width: 100%; padding: 0px; color: #808080;">
                            OTP Validation Events (<span id="spanOtpValidationEventCount" runat="server">0</span>)
                        </td>
                    </tr>

                    <tr>
                        <td style="width: 33%; padding: 10px;">
                            <span>OtpValid</span>
                            <span id="OtpValidCount" runat="server" style="padding-left: 5px; font-weight: bold;">0</span>
                        </td>
                        <td style="width: 33%; padding: 10px;">
                            <span>OtpInvalid</span>
                            <span id="OtpInvalidCount" runat="server" style="padding-left: 5px; font-weight: bold;">0</span>
                        </td>
                        <td style="width: 33%; padding: 10px;">
                            <span>OtpExpired</span>
                            <span id="OtpExpiredCount" runat="server" style="padding-left: 5px; font-weight: bold;">0</span>
                        </td>
                    </tr>


                    <tr>
                        <td colspan="3" style="width: 100%; padding: 5px;">
                            &nbsp;
                        </td>
                    </tr>

                    <tr>
                        <td colspan="3" style="width: 100%; padding: 0px; color: #808080;">
                            Advertising Events (<span id="spanAdEventCount" runat="server">0</span>)
                        </td>
                    </tr>

                    <tr>
                        <td style="width: 33%; padding: 10px;">
                            <span>AdMessageSent</span>
                            <span id="AdMessageSentCount" runat="server" style="padding-left: 5px; font-weight: bold;">0</span>
                        </td>
                        <td style="width: 33%; padding: 10px;">
                            <span>AdEnterOtpScreenSent</span>
                            <span id="AdEnterOtpScreenSentCount" runat="server" style="padding-left: 5px; font-weight: bold;">0</span>
                        </td>
                        <td style="width: 33%; padding: 10px;">
                            <span>AdVerificationScreenSent</span>
                            <span id="AdVerificationScreenSentCount" runat="server" style="padding-left: 5px; font-weight: bold;">0</span>
                        </td>
                    </tr>

                    <tr>
                        <td colspan="3" style="width: 100%; padding: 5px;">
                            &nbsp;
                        </td>
                    </tr>

                    <tr>
                        <td colspan="3" style="width: 100%; padding: 0px; color: #808080;">
                            Event History
                        </td>
                    </tr>

                    <tr>
                        <td style="width: 33%; padding: 10px;">
                            <span>Events</span>
                            <span id="EventsCount" runat="server" style="padding-left: 5px; font-weight: bold;">0</span>
                        </td>
                        <td style="width: 33%; padding: 10px;">
                            &nbsp;
                        </td>
                        <td style="width: 33%; padding: 10px;">
                            &nbsp;
                        </td>
                    </tr>

                </table>
            </div>
        </div>


        <div class="row">
            <div class="large-12 columns" style="width: 100%; text-align: center;">
                <input id="btnCreate" type="submit" runat="server" value="Create" class="button tiny radius" onclick="" />
                <input id="btnCancel" type="button" runat="server" value="Cancel" class="button tiny radius" onclick="javascript: callParentDocumentFunction();" />
            </div>
        </div>

        <div style="padding: 0.5rem;"></div>
        <div class="row">
            <div id="divClientsProcessed" runat="server" class="large-12 columns" style="overflow: auto;"></div>
        </div>

    </form>
</body>
</html>
