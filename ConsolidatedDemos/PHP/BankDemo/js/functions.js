// js general functions

    function StrToHex(pStr) {
        tempstr = '';
        for (a = 0; a < pStr.length; a = a + 1) {
            tempstr = tempstr + pStr.charCodeAt(a).toString(16);
        }
        return tempstr;
    }
    
    function isEmail(text) {
        var status = /^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$/.test(text);
        return status;
    }

    function HideProcessingMessage() {

        $('#divPleaseWaitProcessing').hide();
    }

    function ShowProcessingMessage() {
        // Scroll window to top. If not, then if the browser is towards the bottom of the window, the dialog is out of view at the top
        window.scrollTo(0, 0);

        var screenHeight = $(window).innerHeight();
        var screenWidth = $(window).innerWidth();

        var loaderMargin = (screenHeight / 2) - 18;

        $('#divDialogContainer').css('margin-top', loaderMargin);

        $('#divPleaseWaitProcessing').css("height", screenHeight);
        $('#divPleaseWaitProcessing').css("width", screenWidth);
        $('#divPleaseWaitProcessing').show();
        $('#divDialogContainer').show();
        setTimeout(function () {    // wait 2 seconds
            return true;
        }, 20000);
    }
