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
                                        //form.form('reset');//操作成功后清空表单
                                        //LoadDataforGrid($("#v_q_action_4_e").val());//表头无datagrid
                                    }
                                    else {
                                        jbox.tip("操作失败:" + vData.msg + "!");
                                    }
                                }
                                else
                                    jbox.tip("操作失败,请重试!");
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
                                        //form.form('reset');//操作成功后清空表单
                                        LoadDataforGrid($("#v_q_action_4_e2_line").val());
                                    }
                                    else {
                                        jbox.tip("操作失败:" + vData.msg + "!");
                                    }
                                }
                                else
                                    jbox.tip("操作失败,请重试!");
                            });
                    }
                }
                else
                    jbox.tip("Error:e2编辑模式Line中v_s_action_4_e_line未定义,请添加input片段!");

            });

$(document).ready(
    function () {
        $('#tab_e2').tabs({ fit: true });
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
        title: '数据列表',
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
    $('#datagrid').datagrid('hideColumn', 'KEYS');//不显示keys字段,但可以使用
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
                        $.jBox.tip("没有查询到数据!");
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