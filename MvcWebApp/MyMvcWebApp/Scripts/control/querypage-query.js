
$(document).ready(function () {
    var myqueryaction = $("#v_q_action").val();
    InitGrid();
    InitGridPager(myqueryaction);
    //LoadDataforGrid(myqueryaction);

    $("#bt_search").click(
   function () {
       var jbox = $.jBox;
       var form = $('#f_search');
       var dtOptions = $('#datagrid').datagrid('getPager').data("pagination").options;

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

function InitGrid() {
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
        showFooter: true
    });
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
        }
    });
};