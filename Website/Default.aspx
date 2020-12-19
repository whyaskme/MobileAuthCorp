<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/AdminConsole.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ScriptContainer" runat="Server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="BodyContent" runat="Server">

    <!--show background-->
    <div class="hide-for-small">
        <div style="background: #ccc url(/Images/bg_wave10.jpg) top center repeat-x; height: 575px;">
            <div class="row hide-for-small" style="margin-top: 0;">
                <div class="large-12 columns">
                    <div style="font: bold 1.75rem 'Helvetica Neue', 'Helvetica', Helvetica, Arial, 'Open Sans', sans-serif; color: #fff; text-align: center; padding: 2.5rem 0;">One-Time Password (OTP)</div>
                </div>
            </div>
        </div>
    </div>
    <!--end show background-->

    <!--show background mobile-->
    <div class="show-for-small">
        <div style="background: #ccc url(/Images/bg_mobile.jpg) top center no-repeat; height: 575px;">
            <div class="row show-for-small" style="margin-top: 5px;">
                <div class="large-12 columns">
                    <div style="font: bold 1.5rem 'Helvetica Neue', 'Helvetica', Helvetica, Arial, 'Open Sans', sans-serif; color: #fff; text-align: center; padding: 2.5rem 0 1.35rem;">One-Time Password (OTP)</div>
                </div>
            </div>
        </div>
    </div>
    <!--end show background mobile-->

</asp:Content>

