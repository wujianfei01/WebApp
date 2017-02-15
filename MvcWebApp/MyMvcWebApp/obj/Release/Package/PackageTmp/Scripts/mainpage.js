$($('#tree').tree({
    url: '/Home/GetMenuTree',
    checkbox: false,
    animate: true,
    onClick: function (node) {
        //alert(node.text + ";" + node.attributes.url);
        if ($('#mainTab').tabs('exists', node.text)) {
            $('#mainTab').tabs('select', node.text);
        } else {
            var content = '<iframe scrolling="auto" frameborder="0"  src="' + node.attributes.url + '" style="width:100%;height:100%;"></iframe>';
            //debugger;
            if (node.attributes.url != '0') {
                $('#mainTab').tabs('add', {
                    title: node.text,
                    iconCls: 'icon-ok',
                    content: content,
                    closable: true
                });
            }
        }
    },
    onLoadSuccess: function (node, data) { $("#LoadingDiv").hide(); }
}));

$(setInterval("getTime()", 1000));


function getTime() {
    var date = new Date();
    var day = date.getDay();

    switch (day) {
        case 0: day = "日"; break;
        case 1: day = "一"; break;
        case 2: day = "二"; break;
        case 3: day = "三"; break;
        case 4: day = "四"; break;
        case 5: day = "五"; break;
        case 6: day = "六"; break;

        default: day = ""; break;
    }


    var Localtime = date.getFullYear() + "年" + "  " + (date.getMonth()+1)
            + "月   " + date.getDate() + "日   " + "星期" + day + "   "
            + date.getHours() + ":" + date.getMinutes() + ":"
            + date.getSeconds() + "<br>";
    document.getElementById("time").innerHTML = Localtime;


}