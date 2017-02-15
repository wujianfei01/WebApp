function ClearAllInput() {
    var txts = document.getElementsByTagName("input");
    for (var i = 0; i < txts.length; i++) {
        if ((txts[i].type == "text" || txts[i].type == "checkbox") && txts[i].className != "pagination-num") {
            txts[i].value = "";
        }
    }
};

$("#bt_clearall").click(
                function () {
                    $.jBox.tip("正在清空...", 'loading');
                    ClearAllInput();
                    $.jBox.tip('清空已完成!', 'success');
                }
              );

document.getElementById('f_search').onkeydown = keyDownSearch;

function keyDownSearch(e) {
    // 兼容FF和IE和Opera    
    var theEvent = e || window.event;
    var code = theEvent.keyCode || theEvent.which || theEvent.charCode;
    if (code == 13) {
        $("#bt_search").click();
        return false;
    }
    return true;
};

$(window).resize(function () {
    $('#datagrid').datagrid('resize');
});

