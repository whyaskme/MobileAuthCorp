<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/AdminConsoleDocumentation.master" AutoEventWireup="true" CodeFile="Manage-Topics.aspx.cs" Inherits="Admin.Help.ManageTopics" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ScriptContainer" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BodyContent" Runat="Server">


    <link href="../../App_Themes/CSS/Admin.css" rel="stylesheet" />

    <script src="../../Javascript/jquery-1.10.2.js"></script>

    <script type="text/javascript" src="../../Javascript/ckeditor/ckeditor.js"></script>
    <script type="text/javascript" src="../../Javascript/ckeditor/adapters/jquery.js"></script>
    <script type="text/javascript">

        $(function () {

            CKEDITOR.replace('<%=CKEditor1.ClientID %>', { filebrowserImageUploadUrl: '/Admin/Help/FileUpload.ashx' });

            CKEDITOR.config.allowedContent = true;
            //CKEDITOR.config.autoParagraph = false;

            //toolbar: 'Full';
            //enterMode: CKEDITOR.ENTER_BR;
            //shiftEnterMode: CKEDITOR.ENTER_P;

            ResizeEditorToDisplayImages();
        });

        function togglePermissionsButton(IsChecked)
        {
            if (IsChecked)
                $('#btnAssignAccess').prop('disabled', true);
            else
                $('#btnAssignAccess').prop('disabled', false);

            return false;
        }

        function ResizeEditorToDisplayImages()
        {
            // Just this simple reference forces the control to render the images in the editor
            var htmlEditor = $('#cke_contents_CKEditor1').html;
        }

    </script>

    <div id="divHelpContentDetails">

        <div class="row" id="scroll2">
            <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.25rem;">
                <asp:DropDownList ID="dlTopics" runat="server" AutoPostBack="True" Enabled="true" CssClass="chosen-select" OnSelectedIndexChanged="dlTopics_SelectedIndexChanged"></asp:DropDownList>
            </div>
            <div class="large-6 medium-6 small-12 columns" style="margin-bottom: 0.50rem;">
                <asp:DropDownList ID="dlSubTopics" runat="server" AutoPostBack="True" Enabled="false" CssClass="chosen-select" OnSelectedIndexChanged="dlSubTopics_SelectedIndexChanged">
                    <asp:ListItem Value="0">Sub-Topics</asp:ListItem>
                    <asp:ListItem Value="/Admin/Help/Manage-Topics.aspx?action=newsubtopic">Create New Sub-Topic</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>

        <div class="row" id="divMessageContainer" runat="server">
            <div class="large-12 columns">
                <div style="padding:0.25rem;"></div>
                <div class="alert-box success radius" id="updateMessage" runat="server" style="cursor: pointer;" onclick="javascript: noDisplay();">
                    Update message...
                </div>
            </div>
        </div>

        <div style="padding:0.25rem;"></div>

        <div id="divEditorContainer" runat="server" visible="false">

            <div class="row" style="border: solid 0px #ff0000; vertical-align: middle; padding: 15px;">
                <div style="border: solid 1px #c0c0c0; border-radius: 3px; padding: 10px; height: 70px;">
                    <div id="divPermissions">Permissions</div>
                    <div class="large-6 medium-6 small-12 columns">
                        <div style="position: relative; top: 10px;">
                            <input id="chkIsPublic" type="checkbox" checked="checked" runat="server" onchange="javascript: togglePermissionsButton(this.checked);" style="margin-right: 10px; position: relative; top: 2px;" />Public Access?
                        </div>
                    </div>
                    <div class="large-6 medium-6 small-12 columns" style="position: relative; top: -10px;">
                        <input id="btnAssignAccess" type="button" runat="server" value="Assign User Access" disabled="disabled" class="button tiny radius" onclick="javascript: assignUserDocAccess(); return false;" />
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="large-12 medium-12 small-12 columns">
                    <label id="lblSummary"><span id="spanSummary">Summary (Used in Tooltips)</span>
                        <input id="txtSummary" lbl="Summary" type="text" runat="server" style="width: 100%; padding: 10px; height: 35px; border: solid 1px #c0c0c0;" />
                    </label>
                </div>
            </div>

            <div class="row">
                <div class="large-12 medium-12 small-12 columns" style="height: 600px; border: solid 0px #ff0000;">
                    <label id="lblDetails"><span id="spanDetails">Details</span>
                        <asp:TextBox ID="CKEditor1" TextMode="MultiLine" runat="server"></asp:TextBox>
                    </label>
                </div>
            </div>

            <div class="row" style="width: 100%; text-align: center;">
                <asp:Button ID="btnUpdateTopics" runat="server" Text="Save" Enabled="true" CssClass="button tiny radius" OnClick="SaveTopic" />     
                <asp:Button ID="btnCancelUpdate" runat="server" Text="Cancel" Enabled="true" CssClass="button tiny radius" OnClick="CancelUpdate" />   
            </div>

            <div style="padding:.25rem;"></div>

            <div class="row" style="position: relative; top: 0px; padding-bottom: 100px;">
                <div class="large-12 medium-12 small-12 columns" style="padding-top: 15px;">
                    <div id="divTopicId" runat="server" style="float: left;">Context Id: <span id="spanTopicId" runat="server"></span></div>
                    <div id="divTopicCreated" runat="server" style="float: right;">Created: <span id="spanTopicCreated" runat="server">DateTime</span></div>
                </div>
            </div>


        </div>

    </div>

    <input id="hiddenSelectedUserIds" runat="server" type="hidden" value="" />

</asp:Content>

