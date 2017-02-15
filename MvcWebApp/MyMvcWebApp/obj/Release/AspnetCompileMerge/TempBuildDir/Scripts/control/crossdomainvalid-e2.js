$("#bt_save").click(
            function () {
                var myaction = $("#v_s_action").val();
                var jbox = $.jBox;
                if (myaction) {
                    var form = $("#f_edit");
                    var res = form.form('validate');
                    if (res) {
                        jbox.tip("正在操作......", 'loading');
                        $.post(myaction + "?time=" + Math.random(),
                            form.serialize(),
                            function (data, status) {
                                if (status == "success") {
                                    var vData = $.parseJSON(data);
                                    if (vData.code == 0) {
                                        jbox.tip("操作成功!");
                                        //form.form('reset');//操作成功后清空表單
                                        //LoadDataforGrid($("#v_q_action_4_e").val());//表头无datagrid
                                    }
                                    else {
                                        jbox.tip("操作失敗:" + vData.msg + "!");
                                    }
                                }
                                else
                                    jbox.tip("操作失敗,請重試!");
                            });
                    }
                }
                else
                    jbox.tip("框架Error:第二种编辑模式页面中v_s_action undefined!");

            });

$("#bt_save_4_e_line").click(
            function () {
                var myaction = $("#v_s_action_4_e2_line").val();
                var jbox = $.jBox;
                if (myaction) {
                    var form = $("#f_edit_4_e_line");
                    var res = form.form('validate');
                    if (res) {
                        jbox.tip("正在操作......", 'loading');
                        $.post(myaction + "?time=" + Math.random(),
                            form.serialize(),
                            function (data, status) {
                                if (status == "success") {
                                    var vData = $.parseJSON(data);
                                    if (vData.code == 0) {
                                        jbox.tip("操作成功!");
                                        //form.form('reset');//操作成功后清空表單
                                        LoadDataforGrid($("#v_q_action_4_e2_line").val());
                                    }
                                    else {
                                        jbox.tip("操作失敗:" + vData.msg + "!");
                                    }
                                }
                                else
                                    jbox.tip("操作失敗,請重試!");
                            });
                    }
                }
                else
                    jbox.tip("Error:e2编辑模式Line中v_s_action_4_e_line未定义,请添加input片段!");

            });

$(document).ready(
    function ()
    {
        $('#tab_e2').tabs({fit:true});
        var myqueryaction4edit = $("#v_q_action_4_e2_line").val();
        InitGrid(myqueryaction4edit);
        InitGridPager(myqueryaction4edit);
        LoadDataforGrid(myqueryaction4edit);
    });

function InitGrid(myqueryaction) {
    $('#datagrid').datagrid({
        idField: 'id',
        striped: true,
        singleSelect: true,
        title: '數據列表',
        fitColumns: true,
        collapsible: false,
        fit: false,
        rownumbers: true,
        pagination: true,
        nowrap: false,
        pageNumber: 1,
        pageSize: 20,
        pageList: [10, 20, 50],
        showFooter: true,
        onDblClickRow: function (rowIndex, rowData) {
            $('#f_edit_4_e_line').setForm(rowData);
        }
    });
    $('#datagrid').datagrid('hideColumn', 'KEYS');
};

function InitGridPager(myaction) {
    var dg = $("#datagrid");
    var opts = dg.datagrid('options');
    var pager = dg.datagrid("getPager");
    pager.pagination({
        onSelectPage: function (pageNo, pageSize) {
            opts.pageNumber = pageNo;
            opts.pageSize = pageSize;
            var start = (pageNo - 1) * pageSize;
            var end = start + pageSize;
            $.ajax({
                type: "POST",
                data: $("#f_edit_4_e_line").serialize(),
                url: myaction + "?pageindex=" + pageNo + "&pagesize=" + pageSize + "&time=" + Math.random(),
                success: function (data) {
                    var vData = $.parseJSON(data);
                    var dataGrid = $('#datagrid');
                    dataGrid.datagrid('loadData', vData);
                    dataGrid.datagrid('resize');
                    dataGrid.datagrid('unselectAll');
                    if (vData.total == 0)
                        $.jBox.tip("沒有查詢到數據!");
                }
            });
        }
    });
};

function LoadDataforGrid(myaction) {
    var page = $('#datagrid').datagrid('getPager').data("pagination").options.pageNumber;
    var rows = $('#datagrid').datagrid('getPager').data("pagination").options.pageSize;
    $.ajax({
        type: "POST",
        url: myaction + "?pageindex=" + page + "&pagesize=" + rows + "&time=" + Math.random(),
        data: $("#f_edit_4_e_line").serialize(),
        success: function (data) {
            var vData = $.parseJSON(data);
            var dataGrid = $('#datagrid');
            dataGrid.datagrid('loadData', vData);
            dataGrid.datagrid('resize');
            dataGrid.datagrid('unselectAll');
        }
    });
};

$('#tab_e2').resize(function () {
    $('#tab_e2').tabs('resize');
    $('#datagrid').datagrid('resize');   
});

$.fn.serializeJson = function (otherString) {
    var serializeObj = {},
        array = this.serializeArray();
    $(array).each(function () {
        if (serializeObj[this.name]) {
            serializeObj[this.name] += ';' + this.value;
        } else {
            serializeObj[this.name] = this.value;
        }
    });

    if (otherString != undefined) {
        var otherArray = otherString.split(';');
        $(otherArray).each(function () {
            var otherSplitArray = this.split(':');
            serializeObj[otherSplitArray[0]] = otherSplitArray[1];
        });
    }
    return serializeObj;
};

$.fn.setForm = function (jsonValue) {
    var obj = this;
    $.each(jsonValue, function (name, ival) {
        var $oinput = obj.find("input[name=" + name + "]");
        if ($oinput.attr("type") == "checkbox") {
            if (ival !== null) {
                var checkboxObj = $("[name=" + name + "]");
                var checkArray = ival.split(";");
                for (var i = 0; i < checkboxObj.length; i++) {
                    for (var j = 0; j < checkArray.length; j++) {
                        if (checkboxObj[i].value == checkArray[j]) {
                            checkboxObj[i].click();
                        }
                    }
                }
            }
        }
        else if ($oinput.attr("type") == "radio") {
            $oinput.each(function () {
                var radioObj = $("[name=" + name + "]");
                for (var i = 0; i < radioObj.length; i++) {
                    if (radioObj[i].value == ival) {
                        radioObj[i].click();
                    }
                }
            });
        }
        else if ($oinput.attr("type") == "textarea") {
            obj.find("[name=" + name + "]").html(ival);
        }
        else {
            obj.find("[name=" + name + "]").val(ival);
        }
    })
}