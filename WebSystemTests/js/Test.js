function writelog(entry) {
    alert("writelog");
    var logContent = $("#tbLog").val();
    alert("logContent" + logContent);
    $("#tbLog").val(logContent + entry);
}

function selectoptionbyid(ddl_name, optionid) {
    $("#" + ddl_name + " option[id=" + optionid + "]").attr("selected", true);
    $("#" + ddl_name).trigger("chosen:updated");
    // return the selected option's id to insure the option exists
    return $("#" + ddl_name + " option:selected").id();
}

function selectoptionbyvalue(ddl_name, optionvalue) {
    $('#' + ddl_name + ' option[value="' + optionvalue + '"]').attr('selected', true);
    $('#' + ddl_name).trigger('chosen:updated');
}

function selectoptionbytext(formid, ddl_name, text) {
    // find and select the client in the dropdown
    $("#" + ddl_name + " option").filter(function () {
        return ($(this).text() == text);
    }).prop('selected', true);
    // change the chosen client
    $("#" + ddl_name).trigger("chosen:updated");
    // need to submit the form in order to get the client information populated
    $("#" + formid).submit();
    // return the selected option to insure the option exists
    return $("#" + ddl_name + " option:selected").text();
}
