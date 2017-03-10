//绑定event
$(document).ready(function () {
    var myqueryaction = $("#v_q_action").val();
    var myeditaction = $("#v_e_action").val();
    var mydeleteaction = $('#v_d_action').val();
    var myaddaction = $('#v_a_action').val();
    InitGrid(myqueryaction, myaddaction);//编辑页的表体grid
    InitGridPager(myqueryaction);//编辑页的表体grid
    //LoadDataforGrid(myqueryaction);//编辑页的表体grid
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
                                       LoadDataforGrid($('#v_q_action').val());//刷新页面
                                   }
                                   else {
                                       jbox.tip("操作失败:" + vData.msg + "!");
                                   }
                               }
                               else
                                   jbox.tip("操作失败,请重试!");
                           });
                    }
                    return true; //close
                };

                jbox.confirm("确定要删除吗?", "提示", submit);
            }
            else
                jbox.tip("Error:Index的DataGrid没有key值!");
        }
        else
            jbox.tip("请先选中一行数据!");
    });

    $("#bt_search").click(
   function () {
       var jbox = $.jBox;
       var form = $('#f_search');
       var dtOptions = $('#datagrid').datagrid('getPager').data("pagination").options;
       //检测form的检验规则
       var res = form.form('validate');
       if (res) {
           jbox.tip("正在查询......", 'loading');
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
                           jbox.tip("没有查询到数据!");
                       else
                           $.jBox.tip("查询成功!");
                   }
                   else
                       jbox.tip("查询失败,请重试!");
               });
       }
   }
 );
});

//1.初始化grid以及个人设置
function InitGrid(myqueryaction, myaddaction) {
    $('#datagrid').datagrid({
        idField: 'id',
        striped: true,
        singleSelect: true,
        title: '数据列表',
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

//2.初始化分页器
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
                        $.jBox.tip("没有查询到数据!");
                }
            });
        }
    });
};

//3.加载初始数据
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
            //    $.jBox.tip("没有查询到数据!");
            //else
            //    $.jBox.tip("查询成功!");
        }
    });
};
