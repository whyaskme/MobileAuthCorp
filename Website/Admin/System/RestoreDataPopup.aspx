<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RestoreDataPopup.aspx.cs" Inherits="Backups.RestoreDataPopup" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Restore System Data</title>
    <link href="/App_Themes/CSS/foundation_menu.css" rel="stylesheet" />
    <script src="/Javascript/MACSystemAdmin-Responsive.js"></script>
    <script type="text/javascript">
        function finishRestoreCleanup(backupDatabase) {

            alert("Successfully restored database from (" + backupDatabase + ")");
            window.parent.parent.hideJQueryDialog();
        }

        function cancel() {
            window.parent.parent.hideJQueryDialog();
        }
    </script>
</head>
<body>
    
    <form id="formMain" runat="server">

        <div class="row" style="position: relative; top: 10px; margin-bottom: 0px;">
            <div class="large-12 medium-12 small-12 columns" style="width: 100%; text-align: right;">
                <a href="javascript: NavigateTopicPopup('54f098b7b5655a2ba43ee424');" id="link_help">Help?</a>
            </div>
        </div>

        <div class="row">
            <div class="large-12 columns">
                <h3 id="spanBackups" runat="server" style="font-size:1rem;font-weight:bold;margin-top:0.5rem;">Restore System Data</h3>
                <span style="font-size: 0.875rem;">Please select a backup to restore from. Hover to view stats.</span>
            </div>
        </div>
        <div class="row">
            <div class="large-12 columns">
                <hr />
            </div>
        </div>
        <div class="row">
            <div class="large-12 columns">
                <div id="divBackupDirectoryContainer" runat="server"></div>
            </div>
        </div>
        <div style="padding: 0.5rem;"></div>
        <div class="row">
            <div class="large-12 columns" style="width: 100%; text-align: center;">
                <input class="tiny button radius" id="btn_cancel" type="button" value="Cancel" onclick="javascript: cancel()" />
            </div>
        </div>

        <input id="hiddenSelectedDatabaseBackup" type="hidden" runat="server" value="" />

    </form>
</body>
</html>
