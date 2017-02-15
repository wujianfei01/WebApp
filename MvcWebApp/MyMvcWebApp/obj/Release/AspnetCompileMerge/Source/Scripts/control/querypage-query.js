//綁定event
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
function InitGrid() {
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
        showFooter: true
    });
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
        }
    });
};