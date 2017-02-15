/**
 * framework.basic.all.js
 * 不依赖jquery
 * Copyright (c) 2016 Eric Wu
 *
 */
/*************************************************************************************************************************************************************************/
//define cAjaxForm constructor.
function cAjaxForm(frmObj) {
    this._frm = frmObj;
    this._para = "";
    this.NonBlockPost = true;
    this.httRet = new httpRetObj();
    this.hasServerRet = false;
    this.contentType = 'urlencoded';
};

cAjaxForm.prototype.SetPostContent = function (content) {
    this._para = content;
};
cAjaxForm.prototype.SetBlock = function (isblock) {
    this.NonBlockPost = !isblock;		//true = no-block,  false=block
};

cAjaxForm.prototype.SetPostContentType = function (type) {
    this.contentTypet = type;		//true = no-block,  false=block
};

cAjaxForm.prototype.IE_AjaxHttpGet = function (url) {
    var xmlhttp = GetXMLHttpRequest();
    if (xmlhttp == null) {
        return;
    }
    url += TmTick(url);
    xmlhttp.open('GET', url, false);//no-block, false=block, true= no-block
    xmlhttp.onreadystatechange = function () {
        if (xmlhttp.readyState == 4) {
            this.httpRet.value = xmlhttp.responseText;
            this.hasServerRet = true;
            return this.httpRet.value
        }
    }
    xmlhttp.send(null);
};

cAjaxForm.prototype.Get = function (url, para, szCallbackFunName) {
    var xmlhttp = GetXMLHttpRequest();
    if (xmlhttp == null)
        return;
    xmlhttp.onreadystatechange = function () {
        if (xmlhttp.readyState == 4) {
            this.httRet = xmlhttp.responseText;
            this.hasServerRet = true;
            if (szCallbackFunName.length > 0) {
                var cb_func = window[szCallbackFunName];
                if (!IsNullObj(cb_func) && typeof (cb_func) == 'function')
                    cb_func(this.httRet)
            }
        }
    }
    url += TmTick(url);
    xmlhttp.open("GET", url, this.NonBlockPost);//no-block, false=block, true= no-block
    xmlhttp.send(para);
};

cAjaxForm.prototype.CreatePostParement = function () {
    for (i = 0; i < this._frm.length ; i++) {
        this._para += this._frm.elements[i].name + "=" + encodeURIComponent(this._frm.elements[i].value) + "&";
    }
    this._para += "charset=UTF8";
    return this._para;
    //alert(this._para)
};

cAjaxForm.prototype.CreatePostMultiPart = function () {
    for (i = 0; i < this._frm.length ; i++) {
        this._para += "-----------------------------7db1be8f1260\r\n";
        this._para += "Content-Disposition: form-data; name=\"" + this._frm.elements[i].name + "\"\r\n\r\n" + this._frm.elements[i].value + "\r\n";
    }
    this._para += "-----------------------------7db1be8f1260--\r\n\r\n";
    return this._para;
};

cAjaxForm.prototype.Post = function (url, szCallbackFunName) {
    var xmlhttp = GetXMLHttpRequest();
    url += TmTick(url);
    xmlhttp.open("POST", url, this.NonBlockPost);//no-block, false=block, true= no-block
    if (this._para == "") {
        if (this.contentType == 'multipart') {
            this.CreatePostMultiPart();
            xmlhttp.setRequestHeader("Content-type", "multipart/form-data; boundary=---------------------------7db1be8f1260");
        }
        else {
            this.CreatePostParement();
        }
    }
    xmlhttp.onreadystatechange = function () {
        if (xmlhttp.readyState == 4) {
            this.httRet = xmlhttp.responseText;
            window[szCallbackFunName](this.httRet);
        }
    }
    //xmlhttp.setRequestHeader("Content-length", this._para.length);
    xmlhttp.send(this._para);
};

cAjaxForm.prototype.PostFilter = function (url, szCallbackFunName) {
    var xmlhttp = GetXMLHttpRequest();
    url += TmTick(url);
    xmlhttp.open("POST", url, this.NonBlockPost);//no-block, false=block, true= no-block
    if (this._para == "") {
        if (this.contentType == 'multipart') {
            this.CreatePostMultiPart();
            xmlhttp.setRequestHeader("Content-type", "multipart/form-data; boundary=---------------------------7db1be8f1260");
        }
        else {
            for (i = 0; i < this._frm.length ; i++) {
                this._para += this._frm.elements[i].name + "=" + encodeURIComponent(this._frm.elements[i].value) + "&";
            }
            this._para += "charset=UTF8";
        }
    }
    xmlhttp.onreadystatechange = function () {
        if (xmlhttp.readyState == 4) {
            this.httRet = xmlhttp.responseText;
            window[szCallbackFunName](this.httRet);
        }
    }
    this._para = this._para.replaceAll("'", "''");
    //xmlhttp.setRequestHeader("Content-length", this._para.length);
    xmlhttp.send(this._para);
};

/*************************************************************************************************************************************************************************/
//define cTime constructor.
function cTime() {
    this._objDate = new Date();
};
cTime.prototype.Set = function (millsecs) {
    if (millsecs == 0)
        this._objDate = new Date();
    else
        this._objDate = new Date(millsecs);
};

cTime.prototype.Substract = function (objTime, fmt) {
    timediff = (this._objDate.getTime() - objTime.getTime());
    if (fmt == 'hh')
        return parseInt(timediff / (1000 * 60 * 60));
    else if (fm == "mm")
        return parseInt(timediff / (1000 * 60));
    else
        return parseInt(timediff / 1000);
};
cTime.prototype.Add = function (secs) {
    var dt = this._objDate.getTime() + secs * 1000;
    this._objDate = new Date(dt);
};
cTime.prototype.SetDateStr = function (dt_str) {
    var yyyy = 0, mth = 0, dd = 0, hh = 0, mm = 0, ss = 0;
    if (typeof dt_str == 'undefined') return;
    if (dt_str.length == 0) return;
    var tmp = dt_str.split(' ');

    if (tmp[0] == null && tmp[0].length == 0) return;

    var arr = tmp[0].split('-');
    yyyy = parseInt(arr[0]);
    if (arr[1].charAt(0) == '0') arr[1] = arr[1].substr(1);
    mth = parseInt(arr[1]) - 1;
    if (arr[2].charAt(0) == '0') arr[2] = arr[2].substr(1);
    dd = parseInt(arr[2]);

    if (tmp[1] != null && tmp[1] != 'undefined' && tmp[1].length > 0) {
        var arrTm = tmp[1].split(':');
        if (arrTm[0].charAt(0) == '0') arrTm[0] = arrTm[0].substr(1);
        hh = parseInt(arrTm[0]);
        if (arrTm[1].charAt(0) == '0') arrTm[1] = arrTm[1].substr(1);
        mm = parseInt(arrTm[1]);
        if (arrTm[2].charAt(0) == '0') arrTm[2] = arrTm[2].substr(1);
        ss = parseInt(arrTm[2]);
    }
    this._objDate = new Date(yyyy, mth, dd, hh, mm, ss);
};
function fmt2D(val) {
    var str = val.toString();
    if (str.length == 1)
        str = '0' + str;
    return str;
};
cTime.prototype.ToStr = function (fmt) {
    var strMM = fmt2D(this._objDate.getMonth() + 1);
    var strDD = fmt2D(this._objDate.getDate());
    var strHH = fmt2D(this._objDate.getHours());
    var strMN = fmt2D(this._objDate.getMinutes());
    var strSS = fmt2D(this._objDate.getSeconds());

    var ret = "";
    var arr = fmt.split(' ');
    if (arr[0] == "YYYY-MM-DD")
        ret = this._objDate.getFullYear() + '-' + strMM + '-' + strDD;
    if (arr[0] == "MM-DD")
        ret = strMM + '-' + strDD;
    if (arr[0] == "YY-MM-DD") {
        var stryy = this._objDate.getFullYear().toString();
        stryy = stryy.substr(2)
        ret = stryy + '-' + strMM + '-' + strDD;
    }
    if (arr[1] != null && arr[1] == "HH:MM")
        ret += " " + strHH + ":" + strMN;
    if (arr[1] != null && arr[1] == "HH:MM:SS")
        ret += " " + strHH + ":" + strMN + ":" + strSS;
    return ret;
};
cTime.prototype.GetYear = function () {
    return this._objDate.getFullYear();
};
cTime.prototype.GetMonth = function () {
    return this._objDate.getMonth() + 1;
};
cTime.prototype.GetDay = function () {
    return this._objDate.getDate();
};
cTime.prototype.GetHours = function () {
    return this._objDate.getHours();
};
cTime.prototype.GetMinutes = function () {
    return this._objDate.getMinutes();
};
cTime.prototype.GetSeconds = function () {
    return this._objDate.getSeconds();
};
cTime.prototype.GetLastDayOfMonth = function () {
    var month = this.GetMonth();
    var year = this._objDate.getFullYear();
    return this.GetLastDay(month, year);
};
cTime.prototype.GetLastDay = function (month, year) {
    if (month < 1 || month > 12) { // check month range
        var msg = (g_lang == 'cn') ? "月份必须介于1和12之间" : "Month must be between 1 and 12.";
        alert(msg);
        return 1;
    }
    if (month == 4 || month == 6 || month == 9 || month == 11)
        return 30;
    if (month == 2) { // check for february 29th
        var isleap = (year % 4 == 0 && (year % 100 != 0 || year % 400 == 0));
        if (isleap)
            return 29
        else
            return 28
    } else
        return 31;
};

/*************************************************************************************************************************************************************************/
//javascript String实例扩展
String.prototype.replaceAll = function (s1, s2) {
    return this.replace(new RegExp(s1, "gm"), s2);
};
/*Trim(string):去除字符串两边的空格*/
String.prototype.Trim = function (str) {
    return RTrim(LTrim(str));
};

/*RTrim(string):去除右边的空格*/
String.prototype.RTrim = function (str) {
    var whitespace = new String(" \t\n\r");
    var s = new String(str);

    if (whitespace.indexOf(s.charAt(s.length - 1)) != -1) {
        var i = s.length - 1;
        while (i >= 0 && whitespace.indexOf(s.charAt(i)) != -1) {
            i--;
        }
        s = s.substring(0, i + 1);
    }
    return s;
};

/*LTrim(string):去除左边的空格*/
String.prototype.LTrim = function (str) {
    var whitespace = new String(" \t\n\r");
    var s = new String(str);

    if (whitespace.indexOf(s.charAt(0)) != -1) {
        var j = 0, i = s.length;
        while (j < i && whitespace.indexOf(s.charAt(j)) != -1) {
            j++;
        }
        s = s.substring(j, i);
    }
    return s;
};

//SetQueryString for href
String.prototype.SetQueryString =
function (strName, uVal) {
    var strHref = this;
    var firStr = strHref.substring(0, strHref.lastIndexOf('?'));
    var intPos = strHref.indexOf("?");
    var strRight = strHref.substr(intPos + 1);
    var hasSt = new RegExp(strName, "g").test(strRight);//reg test str;
    var arrTmp = strRight.split("&");
    if (hasSt) {
        for (var i = 0; i < arrTmp.length; i++) {
            var arrTemp = arrTmp[i].split("=");
            if (arrTemp[0].toUpperCase() == strName.toUpperCase())
                arrTemp[1] = uVal;
            arrTmp[i] = arrTemp[0] + "=" + arrTemp[1];//restore url;
        }
    }

    var lastStr = "?";
    for (var i = 0; i < arrTmp.length; i++) {
        if (i > 0)
            lastStr += "&";
        lastStr += arrTmp[i];
    }

    return hasSt ? (firStr + lastStr) : (firStr + lastStr + "&" + strName + "=" + uVal);
};

/*************************************************************************************************************************************************************************/
function cTimer() {
    this.obj = (arguments.length) ? arguments[0] : window;
    return this;
};

// The set functions should be called with:
// - The name of the object method (as a string) (required)
// - The millisecond delay (required)
// - Any number of extra arguments, which will all be
//   passed to the method when it is evaluated.

cTimer.prototype.setInterval = function (func, msec) {
    var i = Timer.getNew();
    var t = Timer.buildCall(this.obj, i, arguments);
    Timer.set[i].timer = window.setInterval(t, msec);
    return i;
};
cTimer.prototype.setTimeout = function (func, msec) {
    var i = Timer.getNew();
    Timer.buildCall(this.obj, i, arguments);
    Timer.set[i].timer = window.setTimeout("Timer.callOnce(" + i + ");", msec);
    return i;
};

// The clear functions should be called with
// the return value from the equivalent set function.

cTimer.prototype.clearInterval = function (i) {
    if (!Timer.set[i]) return;
    window.clearInterval(Timer.set[i].timer);
    Timer.set[i] = null;
};
cTimer.prototype.clearTimeout = function (i) {
    if (!Timer.set[i]) return;
    window.clearTimeout(Timer.set[i].timer);
    Timer.set[i] = null;
};

// Private data

cTimer.set = new Array();
cTimer.buildCall = function (obj, i, args) {
    var t = "";
    Timer.set[i] = new Array();
    if (obj != window) {
        Timer.set[i].obj = obj;
        t = "Timer.set[" + i + "].obj.";
    }
    t += args[0] + "(";
    if (args.length > 2) {
        Timer.set[i][0] = args[2];
        t += "Timer.set[" + i + "][0]";
        for (var j = 1; (j + 2) < args.length; j++) {
            Timer.set[i][j] = args[j + 2];
            t += ", Timer.set[" + i + "][" + j + "]";
        }
    }
    t += ");";
    Timer.set[i].call = t;
    return t;
};
cTimer.callOnce = function (i) {
    if (!Timer.set[i]) return;
    eval(Timer.set[i].call);
    Timer.set[i] = null;
};
cTimer.getNew = function () {
    var i = 0;
    while (Timer.set[i]) i++;
    return i;
};

/*************************************************************************************************************************************************************************/
function Xparse_getValue(xmlRoot, idx) {
    var elem = Xparse_getElement(xmlRoot, idx);
    if (elem == null) return '';
    if (typeof elem.contents != "undefined" && elem.contents[0] != null)
        elem = elem.contents[0];
    if (typeof elem.value == "undefined" || elem.value == null)
        return '';
    return elem.value;
};

function Xparse_getElement(xmlRoot, idx) {
    if (typeof xmlRoot == "undefined" || xmlRoot == null)
        return null;
    tag = xmlRoot.contents;
    if (typeof tag == "undefined" || tag == null)
        return null;
    if (tag.length <= idx) return null;
    var elem = tag[idx];
    if (typeof elem == "undefined" || elem == null)
        return null;
    return elem;
};

function _element() {
    this.type = "element";
    this.name = new String();
    this.attributes = new Array();
    this.contents = new Array();
    this.uid = _Xparse_count++;
    _Xparse_index[this.uid] = this;
    this.currElmIdx = -1;
};
//add on function. ZQ------------------
_element.prototype.getHead = function (tagname) {
    this.currElmIdx = -1;
    return this.getNext(tagname);
};
_element.prototype.getNext = function (tagname) {
    if (typeof this.contents == "undefined" || this.contents == null)
        return null;
    tagname = tagname.toUpperCase();
    while (this.currElmIdx < this.contents.length - 1) {
        this.currElmIdx++;
        if (this.contents[this.currElmIdx].name != null) {
            if (this.contents[this.currElmIdx].name.toUpperCase() == tagname)
                return this.contents[this.currElmIdx];
        }
    }
    return null;
};
_element.prototype.Value = function () {
    if (this == null) return '';
    if (typeof this.contents == "undefined" || this.contents[0] == null)
        return '';
    if (typeof this.contents[0].value == "undefined" || this.contents[0].value == null)
        return '';
    return this.contents[0].value;
};
_element.prototype.getValueByName = function (tagname) {
    var elem = this.getElementByName(tagname);
    return elem.Value();
};
_element.prototype.getElementByName = function (tagname) {
    return this.getHead(tagname);
};
_element.prototype.getValueByName = function (tagname) {
    var elem = this.getElementByName(tagname);
    if (elem != null) return elem.Value();
};
_element.prototype.getElement = function (idx) {
    tag = this.contents;
    if (typeof tag == "undefined" || tag == null)
        return null;
    if (tag.length <= idx) return null;

    var elem = tag[idx];
    if (typeof elem == "undefined" || elem == null)
        return null;
    return elem;
};
_element.prototype.getValue = function (idx) {
    var elem = this.getElement(idx);
    return elem.Value();
};


function _chardata() {
    this.type = "chardata";
    this.value = new String();
};

function _pi() {
    this.type = "pi";
    this.value = new String();
};

function _comment() {
    this.type = "comment";
    this.value = new String();
};

// an internal fragment that is passed between functions
function _frag() {
    this.str = new String();
    this.ary = new Array();
    this.end = new String();
};

// global vars to track element UID's for the index
var _Xparse_count = 0;
var _Xparse_index = new Array();

//// Main public function that is called to 
//// parse the XML string and return a root element object

function cXparse(src) {
    var frag = new _frag();

    // remove bad \r characters and the prolog
    frag.str = _prolog(src);

    // create a root element to contain the document
    var root = new _element();
    root.name = "ROOT";

    // main recursive function to process the xml
    frag = _compile(frag);

    // all done, lets return the root element + index + document
    root.contents = frag.ary;
    root.index = _Xparse_index;
    _Xparse_index = new Array();
    return root;
};

//// transforms raw text input into a multilevel array
function _compile(frag) {
    // keep circling and eating the str
    while (1) {
        // when the str is empty, return the fragment
        if (frag.str.length == 0) {
            return frag;
        }

        var TagStart = frag.str.indexOf("<");

        if (TagStart != 0) {
            // theres a chunk of characters here, store it and go on
            var thisary = frag.ary.length;
            frag.ary[thisary] = new _chardata();
            if (TagStart == -1) {
                frag.ary[thisary].value = _entity(frag.str);
                frag.str = "";
            }
            else {
                frag.ary[thisary].value = _entity(frag.str.substring(0, TagStart));
                frag.str = frag.str.substring(TagStart, frag.str.length);
            }
        }
        else {
            // determine what the next section is, and process it

            if (frag.str.substring(1, 2) == "?") {
                frag = _tag_pi(frag);
            }
            else {
                if (frag.str.substring(1, 4) == "!--") {
                    frag = _tag_comment(frag);
                }
                else {
                    if (frag.str.substring(1, 9) == "![CDATA[") {
                        frag = _tag_cdata(frag);
                    }
                    else {
                        if (frag.str.substring(1, frag.end.length + 3) == "/" + frag.end + ">" || _strip(frag.str.substring(1, frag.end.length + 3)) == "/" + frag.end) {
                            // found the end of the current tag, end the recursive process and return
                            frag.str = frag.str.substring(frag.end.length + 3, frag.str.length);
                            frag.end = "";
                            return frag;
                        }
                        else {
                            frag = _tag_element(frag);
                        }
                    }
                }
            }

        }
    }
    return "";
};

//// functions to process different tags

function _tag_element(frag) {
    // initialize some temporary variables for manipulating the tag
    var close = frag.str.indexOf(">");
    var empty = (frag.str.substring(close - 1, close) == "/");
    if (empty) {
        close -= 1;
    }

    // split up the name and attributes
    var starttag = _normalize(frag.str.substring(1, close));
    var nextspace = starttag.indexOf(" ");
    var attribs = new String();
    var name = new String();
    if (nextspace != -1) {
        name = starttag.substring(0, nextspace);
        attribs = starttag.substring(nextspace + 1, starttag.length);
    }
    else {
        name = starttag;
    }

    var thisary = frag.ary.length;
    frag.ary[thisary] = new _element();
    frag.ary[thisary].name = _strip(name);
    if (attribs.length > 0) {
        frag.ary[thisary].attributes = _attribution(attribs);
    }
    if (!empty) {
        // !!!! important, 
        // take the contents of the tag and parse them
        var contents = new _frag();
        contents.str = frag.str.substring(close + 1, frag.str.length);
        contents.end = name;
        contents = _compile(contents);
        frag.ary[thisary].contents = contents.ary;
        frag.str = contents.str;
    }
    else {
        frag.str = frag.str.substring(close + 2, frag.str.length);
    }
    return frag;
};

function _tag_pi(frag) {
    var close = frag.str.indexOf("?>");
    var val = frag.str.substring(2, close);
    var thisary = frag.ary.length;
    frag.ary[thisary] = new _pi();
    frag.ary[thisary].value = val;
    frag.str = frag.str.substring(close + 2, frag.str.length);
    return frag;
};

function _tag_comment(frag) {
    var close = frag.str.indexOf("-->");
    var val = frag.str.substring(4, close);
    var thisary = frag.ary.length;
    frag.ary[thisary] = new _comment();
    frag.ary[thisary].value = val;
    frag.str = frag.str.substring(close + 3, frag.str.length);
    return frag;
};

function _tag_cdata(frag) {
    var close = frag.str.indexOf("]]>");
    var val = frag.str.substring(9, close);
    var thisary = frag.ary.length;
    frag.ary[thisary] = new _chardata();
    frag.ary[thisary].value = val;
    frag.str = frag.str.substring(close + 3, frag.str.length);
    return frag;
};

//// util for element attribute parsing
//// returns an array of all of the keys = values
function _attribution(str) {
    var all = new Array();
    while (1) {
        var eq = str.indexOf("=");
        if (str.length == 0 || eq == -1) {
            return all;
        }

        var id1 = str.indexOf("\'");
        var id2 = str.indexOf("\"");
        var ids = new Number();
        var id = new String();
        if ((id1 < id2 && id1 != -1) || id2 == -1) {
            ids = id1;
            id = "\'";
        }
        if ((id2 < id1 || id1 == -1) && id2 != -1) {
            ids = id2;
            id = "\"";
        }
        var nextid = str.indexOf(id, ids + 1);
        var val = str.substring(ids + 1, nextid);

        var name = _strip(str.substring(0, eq));
        all[name] = _entity(val);
        str = str.substring(nextid + 1, str.length);
    }
    return "";
};

//// util to remove \r characters from input string
//// and return xml string without a prolog
function _prolog(str) {
    var A = new Array();

    A = str.split("\r\n");
    str = A.join("\n");
    A = str.split("\r");
    str = A.join("\n");

    var start = str.indexOf("<");
    if (str.substring(start, start + 3) == "<?x" || str.substring(start, start + 3) == "<?X") {
        var close = str.indexOf("?>");
        str = str.substring(close + 2, str.length);
    }
    var start = str.indexOf("<!DOCTYPE");
    if (start != -1) {
        var close = str.indexOf(">", start) + 1;
        var dp = str.indexOf("[", start);
        if (dp < close && dp != -1) {
            close = str.indexOf("]>", start) + 2;
        }
        str = str.substring(close, str.length);
    }
    return str;
};

//// util to remove white characters from input string
function _strip(str) {
    var A = new Array();

    A = str.split("\n");
    str = A.join("");
    A = str.split(" ");
    str = A.join("");
    A = str.split("\t");
    str = A.join("");

    return str;
};

//// util to replace white characters in input string
function _normalize(str) {
    var A = new Array();

    A = str.split("\n");
    str = A.join(" ");
    A = str.split("\t");
    str = A.join(" ");

    return str;
};

//// util to replace internal entities in input string
function _entity(str) {
    var A = new Array();

    A = str.split("&lt;");
    str = A.join("<");
    A = str.split("&gt;");
    str = A.join(">");
    A = str.split("&quot;");
    str = A.join("\"");
    A = str.split("&apos;");
    str = A.join("\'");
    A = str.split("&amp;");
    str = A.join("&");

    return str;
};
/*************************************************************************************************************************************************************************/
function GetXMLHttpRequest() {

    var xmlHttp = null;
    if (window.XMLHttpRequest) {
        try { xmlHttp = new XMLHttpRequest(); } catch (e) { xmlHttp = null; }
    }
    else if (window.createRequest) {
        try { xmlHttp = new window.createRequest(); } catch (e) { xmlHttp = null; }
    }

    if (xmlHttp == null && window.ActiveXObject) {
        try {
            xmlHttp = new ActiveXObject("Msxml2.XMLHTTP");
        }
        catch (e) {
            try { xmlHttp = new ActiveXObject("Microsoft.XMLHTTP"); } catch (e) { xmlHttp = null; }
        }
    }
    if (xmlHttp == null) {
        alert("You browser doesn't suppor Ajax.");
    }
    return xmlHttp;

};
function httpRetObj() {
    this.value = '';
};
function HttpGetValue(url, obj) {
    url += TmTick(url);
    var xmlhttp = GetXMLHttpRequest();
    xmlhttp.onreadystatechange = function () {
        if (xmlhttp.readyState == 1) {

        }
        if (xmlhttp.readyState == 4) {
            obj.value = xmlhttp.responseText;
        }
    }
    xmlhttp.open("GET", url, false);//block
    xmlhttp.send(null);
};

function AjaxHttpGet(url) {
    var httpret = new httpRetObj();
    HttpGetValue(url, httpret);
    return httpret.value;
};

function TmTick(url) {
    var tmstr = "";
    if (url.search(/&/) > 0)
        tmstr = '&_tm=' + (new Date()).getTime();
    else if (url.charAt(url.length) == '?')
        tmstr = '_tm=' + (new Date()).getTime();
    else if (url.charAt(url.length) != '?')
        tmstr = '?_tm=' + (new Date()).getTime();
    return tmstr;
};

function IsNullObj(obj) {
    return (obj == null || obj == 'undefined');
};
function IsValidObj(obj) {
    return !(IsNullObj(obj));
};
function JsonToUri(jsnObj) {
    var content = "";
    for (i = 0; i < jsnObj.length; i++) {
        for (var name in jsnObj[i]) {
            content += "&" + name + "=" + encodeURIComponent(jsnObj[i][name]);
        }
    }
    return content;
};

function StringReplaceAll(org, old, newstr) {
    while (org.indexOf(old) >= 0)
        org = org.replace(old, newstr);
    return org;
};
function getDiv_H(divobj) {
    var isIE6 = (window.document.documentElement.clientHeight) ? true : false;
    wnd_H = (isIE6) ? divobj.clientHeight : ((window.document.all) ? window.document.body.clientHeight : window.innerHeight);
    return wnd_H;
};
function getDiv_W(divobj) {
    var isIE6 = (window.document.documentElement.clientHeight) ? true : false;
    wnd_W = (isIE6) ? divobj.clientWidth : ((window.document.all) ? window.document.body.clientWidth : window.innerWidth);
    return wnd_W;
};

function GetPos(w, h, otherstr) {
    var winleft = ((screen.width - w) / 2);
    var wintop = ((screen.height - h) / 2) - 40;
    if (wintop < 0) wintop = 10
    if (otherstr.length == 0)
        otherstr = 'location=no,scrollbars=yes,status=yes,resizable=yes';
    return ('height=' + h + ',width=' + w + ',top=' + wintop + ',left=' + winleft + ',' + otherstr);
};
function Pop(htmPage, wndName, w, h, style) {
    var today = new Date();
    htmPage += "&tm=" + today.getTime();
    var obj = window.open(htmPage, wndName, GetPos(w, h, style));
    obj.focus();
    return obj;
};
function PopModal(htmPage, w, h, style) {
    var str = 'dialogWidth:' + w + 'px;dialogHeight:' + h + 'px;center:yes;help:no;status:no;';
    window.showModalDialog(htmPage, window, str);
    return false;
};
function CloseReload() {
    if (window.opener && !window.opener.closed)
        window.opener.location.reload();
    window.close();
};
function CloseRefreshOpener() {
    if (window.opener && !window.opener.closed) {
        window.opener.focus();
        window.opener.frm.submit();
    }
    window.close();
};
function IsChecked(frm, nm, msg) {
    for (i = 0 ; i < frm.length ; i++) {
        if (frm.elements[i].name == nm && frm.elements[i].checked) {
            return true;
        }
    }
    if (msg.length > 0)
        alert(msg);
    return false;
};
function MarkAllCheckBox(frm, nm, flag) {
    for (i = 0 ; i < frm.length ; i++) {
        if (frm.elements[i].name == nm && frm.elements[i].disabled == false) {
            frm.elements[i].checked = flag;
        }
    }
};
function UncheckAll(frm, nm) {
    for (i = 0 ; i < frm.length ; i++) {
        if (frm.elements[i].name == nm) {
            frm.elements[i].checked = false;
        }
    }
};
function GetRadioBoxValue(objradio) {
    for (i = 0 ; i < objradio.length ; i++) {
        if (objradio[i].checked) {
            return objradio[i].value;
        }
    }
    return '';
};

function ToInt(str) {
    if (str == null)
        return 0.0;
    var n = parseInt(str);
    if (isNaN(n))
        return 0;
    else
        return n;
};
function ToDouble(str) {
    if (str == null)
        return 0.0;
    var n = parseFloat(str);
    if (isNaN(n))
        return 0.0;
    else
        return n;
};
function IsNumerical(field, num) {
    if (field.value.length > 0) {
        for (i = 0 ; i < field.value.length; i++) {
            if (field.value[i] < '0' || field.value[i] > '9')
                return false;
        }
        if (num > 0 && field.value.length > num) {
            return false;
        }
    } else if (num > 0) return false;
    return true;
};
function IsDecimal(field, num) {
    if (field.value.length > 0) {
        for (i = 0 ; i < field.value.length; i++) {
            if (field.value[i] < '0' || field.value[i] > '9' || field.value[i] != '.')
                return false;
        }
        if (field.value.length > num) {
            return false;
        }
    } else if (num > 0) return false;
    return true;
};
