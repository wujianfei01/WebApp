//綁定event
$(document).ready(function () {
    var myqueryaction = $("#v_q_action").val();
    var myeditaction = $("#v_e_action").val();
    var mydeleteaction = $('#v_d_action').val();
    var myaddaction = $('#v_a_action').val();
    InitGrid(myqueryaction, myaddaction);
    InitGridPager(myqueryaction);
    //LoadDataforGrid(myqueryaction);
    $("#bt_add").click(function () {
        var url = myaddaction + '?EORA=a';
        window.location.href = url;
    });
    $("#bt_del").click(function () {
        var dataGrid = $('#datagrid');
        var row = dataGrid.datagrid('getSelected');
        var jbox = $.jBox;
        if (row) {
            if (row.KEYS) {
                var submit = function (v, h, f) {
                    if (v == 'ok') {
                        keys = row.KEYS;
                        $.post(mydeleteaction + '?keys=' + keys + "&time=" + Math.random(),
                           function (data, status) {
                               if (status == "success") {
                                   var vData = $.parseJSON(data);
                                   if (vData.code == 0) {
                                       jbox.tip("操作成功!");
                                       LoadDataforGrid($('#v_q_action').val());//刷新頁面
                                   }
                                   else {
                                       jbox.tip("操作失敗:" + vData.msg + "!");
                                   }
                               }
                               else
                                   jbox.tip("操作失敗,請重試!");
                           });
                    }
                    return true; //close
                };

                jbox.confirm("確定要刪除嗎?", "提示", submit);
            }
            else
                jbox.tip("Error:Index的DataGrid没有key值!");
        }
        else
            jbox.tip("請先選中一行數據!");
    });

    $("#bt_search").click(
   function () {
       var jbox = $.jBox;
       var form = $('#f_search');
       var dtOptions = $('#datagrid').datagrid('getPager').data("pagination").options;
       //檢測form的校驗規則
       var res = form.form('validate');
       if (res) {
           jbox.tip("正在查詢......", 'loading');
           $.post(myqueryaction + "?pageindex=" + dtOptions.pageNumber + "&pagesize=" + dtOptions.pageSize + "&time=" + Math.random(),
               form.serialize(),
               function (data, status) {
                   if (status == "success") {
                       var vData = $.parseJSON(data);
                       var dataGrid = $('#datagrid');
                       dataGrid.datagrid('loadData', vData);
                       dataGrid.datagrid('resize');
                       dataGrid.datagrid('unselectAll');
                       if (vData.total == 0)
                           jbox.tip("沒有查詢到數據!");
                       else
                           $.jBox.tip("查詢成功!");
                   }
                   else
                       jbox.tip("查詢失敗,請重試!");
               });
       }
   }
 );
});

//1.初始化grid以及個人設置
function InitGrid(myqueryaction, myaddaction) {
    $('#datagrid').datagrid({
        idField: 'id',
        striped: true,
        singleSelect: true,
        title: '數據列表',
        fitColumns: true,
        collapsible: true,
        fit: false,
        rownumbers: true,
        pagination: true,
        nowrap: false,
        pageNumber: 1,
        pageSize: 20,
        pageList: [10, 20, 50],
        showFooter: true,
        onDblClickRow: function (rowIndex, rowData) {
            var url;
            var params = "";
            $.each(rowData, function (key, value) {
                params += "&" + key + "=" + value;
            });
            url = encodeURI(myaddaction + '?EORA=' + 'e&ROWINDEX=' + rowIndex + params);
            window.location.href = url;
        }
    });
    $('#datagrid').datagrid('hideColumn', 'KEYS');
};

//2.初始化分頁器
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
                data: $("#f_search").serialize(),
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

//3.加載初始數據
function LoadDataforGrid(myaction) {
    var page = $('#datagrid').datagrid('getPager').data("pagination").options.pageNumber;
    var rows = $('#datagrid').datagrid('getPager').data("pagination").options.pageSize;
    $.ajax({
        type: "POST",
        url: myaction + "?pageindex=" + page + "&pagesize=" + rows + "&time=" + Math.random(),
        data: $("#f_search").serialize(),
        success: function (data) {
            var vData = $.parseJSON(data);
            var dataGrid = $('#datagrid');
            dataGrid.datagrid('loadData', vData);
            dataGrid.datagrid('resize');
            dataGrid.datagrid('unselectAll');
            //if (vData.total == 0)
            //    $.jBox.tip("沒有查詢到數據!");
            //else
            //    $.jBox.tip("查詢成功!");
        }
    });
};
