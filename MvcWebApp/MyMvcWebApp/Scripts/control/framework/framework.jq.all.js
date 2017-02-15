/**
 * framework.jq.all.js
 * 依赖jquery
 * Copyright (c) 2016 Eric Wu
 *
 */
function IsEmpty(obj_id, errmsg) {
    var val = $('#' + obj_id).val();
    if (val == '' || val == 0 || val == 0.0) {
        alert(errmsg);
        $('#' + obj_id).focus();
        return true;
    }
    return false;
};

/***********************************************************1.前端校验区 start ***************************************************************************/
//<input type="input" class="easyui-validatebox" validType="cCheckExist['/controller/action?key=${usertable.userid}']" required="true"/> 后端可以拿到key为name的值,即为控件上面的输入的值
$.extend($.fn.validatebox.defaults.rules, {
       cCheckExist: {
            validator: function (value, param) {
                var tmp=$.ajax({
                                    async: false,
                                    cache: false,
                                    type: 'post',
                                    url: param + "?time=" + Math.random(),
                                    data: {
                                        name: value
                                    }
                                }).responseText;
                return tmp == "unexist" ? true : false;
            },
            message: '此项已经在数据库中存在!'
       },
        cCheckNum: {  
            validator: function(value, param) {  
                return /^([0-9]+)$/.test(value);  
            },  
            message: '请输入整数!'  
        },  
        cCheckFloat: {  
            validator: function(value, param) {  
                return /^[+|-]?([0-9]+\.[0-9]+)|[0-9]+$/.test(value);  
            },  
            message: '请输入合法数字!'  
        }
});
/***********************************************************1.前端校验区 end ***************************************************************************/

/***********************************************************2.jquery实例对象功能扩展区 start ***************************************************************************/
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
};
/***********************************************************2.jquery实例对象功能扩展区 end ***************************************************************************/

/***********************************************************3.控件扩展区 start ***************************************************************************/
//disValFlag:0-显示key,1->显示value,3或者不传->显示key|value
$.fn.ajaxTextBox = function (url,disValFlag) {
    var obj = this;
    obj.autocomplete(url + "?time=" + Math.random(), {
        httpMethod: "POST",
        dataType: 'json',
        minChars: 1,
        selectFirst: false,
        matchCase: false,
        max: 50,
        scrollHeight: 280,

        formatItem: function (row, i, max) {
            var item;
            if (disValFlag == 0)
            {
                item = "<div><span style='display:block;float:right;color:green;font-style:italic;'></span>" + row[1] + "</div>";
            }
            else if (disValFlag == 1)
            {
                item = "<div><span style='display:block;float:right;color:green;font-style:italic;'></span>" + row[0] + "</div>";
            }
            else
            {
                item = "<div><span style='display:block;float:right;color:green;font-style:italic;'></span>" + row[1] + "|" + row[0] + "</div>";
            }
              
            return item;
        }
    });
};
/***********************************************************3.控件扩展区 end ***************************************************************************/
/***********************************************************4.js对象扩展区(原生或者自定义都写这里) start ***************************************************************************/
//js数组的数字全部保留2位小数,做前端报表时候方便数据处理(如baidu echarts等)
Array.prototype.AllToFixed = function (t) {
    var tmpArray = [];
    $.each(this, function (i, n) {
        var f = parseFloat(n);
        if (isNaN(f)) {
            return true;
        }
        tmpArray[i] = f.toFixed(t);
    });

    return tmpArray;
};
/***********************************************************4.控件扩展区 end ***************************************************************************/