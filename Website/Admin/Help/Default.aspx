<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/AdminConsoleDocumentation.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Admin.Help.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ScriptContainer" Runat="Server">
    <script>

</script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BodyContent" Runat="Server">

    <script type="text/javascript">

        //$(document).ready(function () {

            //alert("WTF???");

            SizeAndWatermarkImages();

            // position print content div offscreen
            var y = $("#divContentDetails").height() + 1500;

            $("#divContentDetails").css("position", "absolute");
            $("#divContentDetails").css("top", -y);

            $("#divContentDetails").css("visibility", "hidden");

        //});

        function SizeAndWatermarkImages() {
            //alert("Here...1");

            var arr = [];
            var imageIndex = 0;

            $("div.watermark > img").each(function () {
                var x = $(this).width();
                arr[imageIndex] = x;
                imageIndex++;
            });

            var int = arr.length - 1;

            $($("div.watermark").get().reverse()).each(function () {
                $(this).width(arr[int]);
                int--;
            });
        }

        var selectedTopic = "What topic do I want?";

        function ManageTopics() {
            window.location = "/Admin/Help/Manage-Topics.aspx";
        }

        function cancelPrint() {
            window.parent.parent.hideJQueryDialog();
        }

        function PrintTopicContent()
        {
            $("#divHelpDetails img").css("opacity", "1.0");

            // return print content to original position
            var y = $("#divContentDetails").height() + 1500;
            $("#divContentDetails").css("position", "relative");
            $("#divContentDetails").css("top", "50px");
            $("#divContentDetails").css("visibility", "visible");
            
            $('#PrintedPageContainer').printElement
                (
                    {
                        pageTitle: 'MAC System Documentation ',
                        printMode: 'iframe', //'iframe', 'popup'
                        overrideElementCSS: ['/App_Themes/CSS/Admin.css', { href: '/App_Themes/CSS/Admin.css', media: 'print' }, '/App_Themes/CSS/print.css', { href: '/App_Themes/CSS/print.css', media: 'print' }],

                        iframeElementOptions:
                        {
                            styleToAdd: 'position:absolute; bottom:0px;visibility:hidden;'
                        },

                        printBodyOptions:
                        {
                            styleToAdd: 'padding:0px; margin:0px; color:#fff !important;'
                        }
                    }
                );
            // reposition print content div offscreen
            var y = $("#divContentDetails").height() + 1500;
            $("#divContentDetails").css("position", "absolute");
            $("#divContentDetails").css("top", -y);
            $("#divContentDetails").css("visibility", "hidden");
        }

        function FormatTopicTitle(topicTitle)
        {
            //alert(topicTitle);
            topicTitle = topicTitle.replace("1) ", "");
            topicTitle = topicTitle.replace("2) ", "");
            topicTitle = topicTitle.replace("3) ", "");
            topicTitle = topicTitle.replace("4) ", "");
            topicTitle = topicTitle.replace("5) ", "");
            topicTitle = topicTitle.replace("6) ", "");
            topicTitle = topicTitle.replace("7) ", "");
            topicTitle = topicTitle.replace("8) ", "");
            topicTitle = topicTitle.replace("9) ", "");
            topicTitle = topicTitle.replace("10) ", "");

            return topicTitle;
        }

    </script>

    <link href="/App_Themes/CSS/Admin.css" rel="stylesheet" />
    <link href="/App_Themes/CSS/Documentation.css" rel="stylesheet" />

    <script src="/Javascript/jquery-1.10.2.js"></script>
    <script src="/Javascript/jquery-migrate-1.0.0.js"></script>
    <script src="/Javascript/jquery.printElement.js"></script>

    <div id="divHelpContentDetails">

        <div class="row" id="scroll2">
            <div class="large-4 medium-4 small-12 columns" style="margin-bottom: 0.25rem;overflow:">
                <asp:DropDownList ID="dlTopics" runat="server" AutoPostBack="True" Enabled="true" CssClass="chosen-select" OnSelectedIndexChanged="dlTopics_SelectedIndexChanged"></asp:DropDownList>
            </div>
            <div class="large-4 medium-4 small-12 columns" style="margin-bottom: 0.50rem;">
                <asp:DropDownList ID="dlSubTopics" runat="server" AutoPostBack="True" Enabled="false" CssClass="chosen-select" OnSelectedIndexChanged="dlSubTopics_SelectedIndexChanged">
                    <asp:ListItem Value="0">Sub-Topics</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="large-4 medium-4 small-12 columns" style="margin-bottom: 0.25rem;">
                <input id="btnManageTopics" runat="server" type="button" class="button tiny radius" value="Manage" onclick="javascript: ManageTopics();" />
                <input id="btnPrintManual" runat="server" type="button" class="button tiny radius" value="Print" onclick="javascript: PrintTopicContent();" />
            </div>
        </div>

        <div style="padding: 1.0rem;"></div>

        <div id="divEditorContainer" runat="server" style="border: solid 0px #0000ff; margin: 0 auto; min-height: 792pt; width: 612pt;">
            <div class="row">

                <div id="PrintedPageContainer" class="PrintedPageContainer" runat="server">
                    <div id="divPrintPageWithMargin" class="divPrintPageWithMargin">

                        <!-- Help Content -->
                        <div class="row" style="padding-top: 0px; padding-bottom: 15px;">
                            <table style="border: solid 0px #0000ff;">
                                <tr>
                                    <td style="border: solid 0px #ff0000; text-align: right; vertical-align: middle;">
                                        <img src="../../Images/OwnerLogos/!MAC-Logo.png" />
                                    </td>
                                    <td style="border: solid 0px #ff0000; width: 100%; text-align: right; vertical-align: middle;">
                                        <div id="divHelpDescription" runat="server" style="font-size: 18px; z-index: 1000;"> </div>
                                    </td>
                                </tr>
                            </table>
                        </div>

                        <div class="row" style="min-height: 450px; border-top: solid 1px #c0c0c0; padding-top: 25px; margin-bottom: 50px;">
                            <div class="large-12 medium-12 small-12 columns" style="font-size: 14px; word-wrap:break-word;">
                                <div id="divHelpDetails" runat="server" style="font-size: 12px;"></div>
                            </div>
                        </div>
                        <!-- Help Content -->

                    </div>

                    <div class="row" style="position: relative; bottom: 0px; padding: 25px;">
                        <div class="large-12 medium-12 small-12 columns" style="border-top: solid 1px #c0c0c0; padding-top: 15px;">
                            <div id="divTopicId" runat="server" style="float: left;">Context Id: <span id="spanTopicId" runat="server"></span></div>
                            <div id="divTopicCreated" runat="server" style="float: right;">Created: <span id="spanTopicCreated" runat="server"></span></div>
                        </div>
                    </div>

                </div>
            </div>

            <div style="padding: 1.0rem;"></div>

        </div>
    </div>

    <input id="hiddenX" type="hidden" runat="server" value="" />

</asp:Content>

