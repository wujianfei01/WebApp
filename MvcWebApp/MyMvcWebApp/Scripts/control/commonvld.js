/**
 * form 表单校验器
 */
(function ($) {
    /**
     * 表单校验添加jquery插件方法，方法简介如下：
     * 1、validateInit    校验初始化
     * 2、disableValidate    禁用校验    
     * 3、validate    校验
     * 4、isFormElement    是否为表单元素
     * 5、vattr    校验属性的获取和设置
     * 6、getVresult 获取校验结果
     * 7、saveVresult 保存校验结果
     */
    $.fn.extend({
        /*********************************************************************************
         * 初始化校验 绑定校验事件
         * validityObj: 校验选项，可选，如果设置了该选项，往后的校验就以该选项的值为主
         * validityObj = {
         *         vtype:    校验类型
         *         pattern: 校验正则表达式
         *         max: vtype为number是的最大值
         *         min: vtype为number是的最小值
         *         maxLength:非number是字符串的最大长度
         *         minLength:非number是字符串的最小长度
         *         step:vtype为number是的step
         *         onEvent:
         *         beforeFunc: func1,    //校验前的执行方法，该方法接受对象本身参数$(this)。
         *         afterFunc: func2,    //校验后的执行方法，该方法接受对象本身参数$(this)。
         * }
         *********************************************************************************/
        validateInit: function (validityObj) {
            if ($(this).isFormElement()) {
                $(this).data('vresult', '');
                event.onEvent($(this), validityObj);
            }
            else {
                $(this).find(constants.formElements.join(',')).each(function () {
                    $(this).data('vresult', '');
                    event.onEvent($(this), validityObj);
                });
            }
        },
        /******************************************
         * disabled校验功能
         * 取消本对象上的校验相关
         * 1、取消绑定校验事件。
         * 2、取消校验结果，设置不可用标识$(this).data('vresult','disabled');
         ******************************************/
        disableValidate: function () {
            if ($(this).isFormElement()) {
                event.offEvent($(this));
                $(this).data('vresult', 'disabled');
            }
            else {
                $(this).find(constants.formElements.join(',')).each(function () {
                    event.offEvent($(this));
                    $(this).data('vresult', 'disabled');
                });
            }
            return $(this);
        },
        /*********************************************************************************
         * 校验方法，开始校验
         * validityObj: 校验选项，可选，如果设置了该选项，往后的校验就以该选项的值为主
         * validityObj = {
         *         vtype:    校验类型
         *         pattern: 校验正则表达式
         *         max: vtype为number是的最大值
         *         min: vtype为number是的最小值
         *         maxLength:非number是字符串的最大长度
         *         minLength:非number是字符串的最小长度
         *         step:vtype为number是的step
         *         onEvent:
         *         beforeFunc: func1,    //校验前的执行方法，该方法接受对象本身参数$(this)。
         *         afterFunc: func2,    //校验后的执行方法，该方法接受对象本身参数$(this)。
         * }
         *********************************************************************************/
        validate: function (validityObj) {
            if ($(this).isFormElement()) {
                if (validityObj) {
                    for (var attr in validityObj) {
                        validityObj[attr] && source.vattr(attr, validityObj[attr]);
                    }
                }
                return event.validate('', $(this));
            }
            else {
                var result = true;
                $(this).find(constants.formElements.join(',')).each(function () {
                    var res = event.validate('', $(this));
                    if (res && res.valid == false) result = false;
                });
                return { valid: result };
            }
        },
        /*********************************************
         * 是否为表单元素
         * 判断当前对象的tagName是否为INPUT,TEXTEREA等。
         *********************************************/
        isFormElement: function () {
            var tag = $(this)[0].tagName;
            for (var i = 0; i < constants.formElements.length; i++) {
                if (tag == constants.formElements[i]) {
                    return true;
                }
            }
            return false;
        },
        /********************************************************************
         * 校验属性的获取和设置
         * 将属性取出放置到data('validator')中
         *******************************************************************/
        vattr: function (attr, val) {
            !$(this).data('validator') && $(this).data('validator', {});
            var data = $(this).data('validator');
            if (val && (val + '').trim() != '') {
                data[attr] = val;
                return $(this);
            }
            if (!data[attr]) {
                var attrVal = $(this).attr(attr);
                if (attrVal && attrVal.trim() != '') {
                    if (attr == 'beforeFunc' || attr == 'afterFunc') {
                        data[attr] = eval("(" + attrVal.trim() + ")");
                    } else {
                        data[attr] = attrVal.trim();
                    }
                }
            }
            return data[attr];
        },
        /*****************************************************************
         * 获取校验结果，从data('vresult')中
         * 校验完成时，结果存储在当前对象的 $(this).data('vresult')中；
         * 结果对象为htmL5校验结果对象，并添加几个其他
         * result = {
         *    badInput: false,
         *     customError: false,
         *    errorType: "typeMismatch",      //错误类型，记录值为true的属性到这里。
         *    patternMismatch: false,
         *    rangeOverflow: false,
         *    rangeUnderflow: false,
         *    stepMismatch: false,
         *    tooLong: false,
         *    typeMismatch: true,
         *    valid: false,
         *    valueMissing: false
         * }
         *****************************************************************/
        getVresult: function () {
            return $(this).data('vresult');
        },
        /*****************************************************************
         * 保存校验结果，到data('vresult')中
         * 校验完成时，结果存储在当前对象的 $(this).data('vresult')中；
         * 结果对象为htmL5校验结果对象，并添加几个其他
         * result = {
         *    badInput: false,
         *     customError: false,
         *    errorType: "typeMismatch",      //错误类型，记录值为true的属性到这里。
         *    patternMismatch: false,
         *    rangeOverflow: false,
         *    rangeUnderflow: false,
         *    stepMismatch: false,
         *    tooLong: false,
         *    typeMismatch: true,
         *    valid: false,
         *    valueMissing: false
         * }
         *****************************************************************/
        saveVresult: function (result) {
            var result = $.extend({}, $(this)[0].validity, result);
            for (var attr in result) {
                if (attr != "valid" && result[attr] == true) {
                    result.valid = false;
                    result.errorType = attr;
                    break;
                }
            }
            $(this).data('vresult', result);
            return $(this);
        }
    });

    /******************************************
     * jquery对象方法调用的事件函数
     * 主要函数有：
     * 1、validate    校验
     * 2、onEvent    添加事件
     * 3、offEvent    取消事件
     **************************************/
    var event = {
        validate: function (event, source) {
            source = !source ? event.data.source : source;
            if (source.data('vresult') == 'disabled') return;    //disabed used .disabedvalidate()
            //do beforeFunc
            source.vattr('beforeFunc') instanceof Function && source.vattr('beforeFunc')(source);
            //do validate
            source.saveVresult(validator.validate(source));
            //do afterFunc
            source.vattr('afterFunc') instanceof Function && source.vattr('afterFunc')(source);
            return source.getVresult();
        },
        onEvent: function (source, validityObj) {
            if (validityObj) {
                for (var attr in validityObj)
                    validityObj[attr] && source.vattr(attr, validityObj[attr]);
            }
            source.vattr('onEvent') && source.on(source.vattr('onEvent'), { source: source }, this.validate);
            source.on('blur', { source: source }, this.validate);
        },
        offEvent: function (source) {
            source.off(event, this.validate);
            source.off('blur', this.validate);
        }
    };

    //常量
    var constants = {
        formElements: ['INPUT', 'TEXTAREA']
    };

    var validator = {
        empty: function (value) {
            return !value || value.trim() == '';
        },
        number: function (value) {
            if (this.empty(value)) return true;
            return /^\-?[1-9]\d*(\.\d+)?$/.test(value);
        },
        min: function (value, min) {
            if (this.empty(value)) return true;
            return value * 1 >= min;
        },
        max: function (value, max) {
            if (this.empty(value)) return true;
            return value * 1 <= max;
        },
        step: function (value, step, min) {
            if (this.empty(value)) return true;
            min = this.number(min) ? min * 1 : 0;
            return (value * 1 - min) / step * 1 % 1 === 0;
        },
        tel: function (value) {
            if (this.empty(value)) return true;
            var reg = /^(0[0-9]{2,3}\-)?[2-9][0-9]{6,7}(\-[0-9]{1,4})?$/;
            return reg.test(value.trim());
        },
        mobile: function (value) {
            if (this.empty(value)) return true;
            var reg = /^1[3-8]\d{9}$/;
            return reg.test(value.trim());
        },
        email: function (value) {
            if (this.empty(value)) return true;
            var reg = /^.{2,}@.{2,}$/;
            return reg.test(value.trim());
        },
        pattern: function (value, regExp) {
            if (this.empty(value)) return true;
            return RegExp(regExp).test(value);
        },
        validate: function (source) {
            if (!source.vattr('vtype')
                    && !source.vattr('pattern')
                    && !source.vattr('minLength')
                    && !source.vattr('maxLength'))
                return;
            var vtype = source.vattr('vtype') ? source.vattr('vtype') : source.vattr('type');
            var val = source.val().trim();
            source.val(val);
            //required validate
            if (source.vattr("required") == 'required' && validator.empty(val)) {
                return { valueMissing: true };
            }
            //vtype validate
            if (vtype == 'number') {
                if (!validator.number(val)) return { typeMismatch: true };
                if (source.vattr('min') && !isNaN(source.vattr('min') * 1)) {
                    if (!validator.min(val, source.vattr('min') * 1)) return { rangeUnderflow: true };
                }
                if (source.vattr('max') && !isNaN(source.vattr('max') * 1)) {
                    if (!validator.max(val, source.vattr('max') * 1)) return { rangeOverflow: true };
                }
                if (source.vattr('step') && !isNaN(source.vattr('step') * 1)) {
                    if (!validator.step(val, source.vattr('step'), source.vattr('min'))) return { stepMismatch: true };
                }
            } else {
                if (vtype == 'email') {
                    if (!validator.email(val)) return { typeMismatch: true };
                } else if (vtype == 'tel') {
                    if (!validator.tel(val)) return { typeMismatch: true };
                } else if (vtype == 'url') {
                    if (!validator.url(val)) return { typeMismatch: true };
                }
                if (!validator.empty(val)) {
                    if (source.vattr('minLength') && !isNaN(source.vattr('minLength') * 1)) {
                        if (val.length < source.vattr('minLength') * 1) return { tooShort: true };
                    }
                    if (source.vattr('maxLength') && !isNaN(source.vattr('maxLength') * 1)) {
                        if (val.length > source.vattr('maxLength') * 1) return { tooLong: true };
                    }
                }
            }
            //pattern validate
            if (source.vattr('pattern') && source.vattr('pattern').trim() != '') {
                if (!validator.pattern(val, source.vattr('pattern'))) return { patternMismatch: true };
            }
        }
    };
    /**
     * String trim
     */
    String.prototype.trim = function () {
        return this.replace(/(^\s*)|(\s*$)/g, '');
    };
    String.prototype.leftTrim = function () {
        return this.replace(/^\s*/g, '');
    };
    String.prototype.rightTrim = function () {
        return this.replace(/\s*$/g, '');
    };
})(jQuery);


$(function () {
    $("#f_search").validateInit();
});

var modify = function (source) {
    source.val(source.val().replace(/[^0-9]+/g, ''));
};

var validate = function () {
    var res = $("#f_search").validate();
    alert(res.valid);
};

var showTip = function (e) {
    var res = e.getVresult();
    if (res && res.valid == false) {
        e.next().remove();
        //e.after('<span>' + res.errorType + '</span>');
        e.val(errorMsg(res.errorType));
    } else {
        e.next().remove();
    }
};

var disabd = function () {
    $("#f_search").disableValidate();
}

//中文化
var errorMsg = function (errorType)
{
    var cMsg="";
    switch (errorType) {
        case "valueMissing":
            cMsg = "必填項";
            break;
        default:
            cMsg = "未找到對應錯誤信息";
    }

    return cMsg;
}