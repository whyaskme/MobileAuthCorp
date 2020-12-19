var y1 = $('#col1').height();
var y2 = $('#col2').height();
var y3 = $('#col3').height();
var ym = Math.max(y1, y2, y3);
var ympx = ym + 'px';
$('#col1').height(ympx);
$('#col2').height(ympx);
$('#col3').height(ympx);

$(window).resize(function () {
    $('#col1').css('height', '');
    $('#col2').css('height', '');
    $('#col3').css('height', '');
    var y1 = $('#col1').height();
    var y2 = $('#col2').height();
    var y3 = $('#col3').height();
    var ym = Math.max(y1, y2, y3);
    var ympx = ym + 'px';
    $('#col1').height(ympx);
    $('#col2').height(ympx);
    $('#col3').height(ympx);
});