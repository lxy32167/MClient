<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Xzz.aspx.cs" Inherits="MClient.Xzz" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>基于大数据中心的网上阅卷系统客户端</title>
    <script type="text/javascript" src="jquery-1.9.1.js"></script>
    <script type="text/javascript" src="jquery.blockUI.js"></script>
    <link href="layout.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
    
    .mainTitle
    {
        font-size: 12pt;
        font-weight: bold;
        font-family: 宋体;
    }

    .commonText
    {
        font-size: 11pt;
        font-family: 宋体;
    }

    .littleMainTitle
    {
        font-size: 10pt;
        font-weight: bold;
        font-family: 宋体;
    }

    .TopTitle
    {
        border: 0px;
        font-size: 10pt;
        font-weight: bold;
        text-decoration: none;
        color: Black;
        display: inline-block;
        width: 100%;
    }

    .SelectedTopTitle
    {
        border: 0px;
        font-size: 10pt;
        text-decoration: none;
        color: Black;
        display: inline-block;
        width: 100%;
        background-color: White;
    }

    .ContentView
    {
        border: 0px;
        padding: 3px 3px 3px 3px;
        background-color: White;
        display: inline-block;
        width: 390px;
    }

    .SepBorder
    {
        border-top-width: 0px;
        border-left-width: 0px;
        font-size: 1px;
        border-bottom: Gray 1px solid;
        border-right-width: 0px;
    }

    .TopBorder
    {
        border-right: Gray 1px solid;
        border-top: Gray 1px solid;
        background: #DCDCDC;
        border-left: Gray 1px solid;
        color: black;
        border-bottom: Gray 1px solid;
    }

    .ContentBorder
    {
        border-right: Gray 1px solid;
        border-top: Gray 0px solid;
        border-left: Gray 1px solid;
        border-bottom: Gray 1px solid;
        height: 100%;
        width: 100%;
    }

    .SelectedTopBorder
    {
        border-right: Gray 1px solid;
        border-top: Gray 1px solid;
        background: none transparent scroll repeat 0% 0%;
        border-left: Gray 1px solid;
        color: black;
        border-bottom: Gray 0px solid;
    }

    .Button
    {
        position: relative;
    }

    .Title
    {
        text-align: center;
    }

    .FootName
    {
        position: relative;
        left: 100px;
    }

    .FootDate
    {
        float: right;
    }

    .hidden
    {
        display: none;
    }

    menu.main
    {
        padding: 0px;
        padding-bottom: 2px;
        margin: 0px;
        margin-left: auto;
        margin-right: auto;
        background-color: #EEE;
        background-image: linear-gradient(bottom, rgb(238,238,255) 36%, rgb(244,244,244) 68%);
        background-image: -o-linear-gradient(bottom, rgb(238,238,255) 36%, rgb(244,244,244) 68%);
        background-image: -moz-linear-gradient(bottom, rgb(238,238,255) 36%, rgb(244,244,244) 68%);
        background-image: -webkit-linear-gradient(bottom, rgb(238,238,255) 36%, rgb(244,244,244) 68%);
        background-image: -ms-linear-gradient(bottom, rgb(238,238,255) 36%, rgb(244,244,244) 68%);
        background-image: -webkit-gradient( linear, left bottom, left top, color-stop(0.36, rgb(238,238,255)), color-stop(0.68, rgb(244,244,244)) );
    }

        menu.main ul.main_ul
        {
            display: inline-block;
            margin-right: auto;
            text-align: center;
            margin-left: auto;
            margin: 0px;
            padding: 0px;
        }

        menu.main ul li
        {
            display: inline-block;
            margin-left: auto;
            padding-right: 5px;
            padding-left: 5px;
            border: 1px solid #CCCCCC;
            margin: 0px;
            padding: 0px;
        }

    menu.sub
    {
        padding: 0px;
        padding-top: 3px;
        margin: 0px;
        margin-left: auto;
        position: relative;
    }

    menu.main ul li menu.sub ul
    {
        display: list-item;
    }

    menu.main ul li menu.sub input.button:hover
    {
        background-color: Gray;
    }

    menu.main ul li a
    {
        font: 10px/10px Arial, Helvetica, sans-serif;
        color: #6F6F6F;
        text-align: left;
    }

    menu.main ul li menu.sub input
    {
        margin: 0px;
        padding: 0px;
    }

    #upload-file-container
    {
        width: 24px;
        position: relative;
        height: 24px;
        float: left; /*    display: inline-block;*/
    }


    menu.main ul.main_ul li menu.sub input#FileToUpload.input
    {
        position: absolute;
        overflow: hidden;
        width: 24px;
        height: 24px;
        -ms-filter: 'progid:DXImageTransform.Microsoft.Alpha(Opacity=0)';
        opacity: 0;
        left: 0px;
    }

    /*This is a horrible terrible hack to get FireFox to render our single-click upload button properly.*/
    @-moz-document url-prefix()
    {
        menu .main ul.main_ul li menu.sub input#FileToUpload.input;

    {
        position: absolute;
        overflow: hidden;
        width: 24px;
        height: 24px;
        opacity: 0;
        left: -193px;
    }

    }


    #Insert-File-Container
    {
        width: 24px;
        position: relative;
        height: 24px;
        display: inline-block;
    }

    menu.main ul.main_ul li menu.sub input#FileToInsert.input
    {
        position: absolute;
        overflow: hidden;
        width: 24px;
        height: 24px;
        -ms-filter: 'progid:DXImageTransform.Microsoft.Alpha(Opacity=0)';
        opacity: 0;
    }

    menu.main ul li menu.sub ul li
    {
    }

    .nav
    {
    }
    #content
    {
        position: relative;
        overflow: auto;
        cursor: move;
    }
    .dragAble
    {
        position:absolute;
        cursor: move;
    }
</style>
    <script type="text/javascript">
        //图片放大和缩小（兼容IE和火狐，谷歌）
        var divId;
        var v_left;
        var v_top;

        window.onload = function () {
            var images1 = document.getElementById("img");
            divId = document.getElementById("content");
            var height1 = images1.height; //图片的高度
            var width1 = images1.width; //图片的宽度
            v_left = (document.body.clientWidth - width1) / 2;
            v_top = (document.body.clientHeight - height1) / 2;
            divId.style.left = v_left;
            divId.style.top = v_top;

        }
        drag = 0;
        move = 0;
        // 拖拽对象
        var ie = document.all;
        var nn6 = document.getElementById && !document.all;
        var isdrag = false;
        var y, x;
        var oDragObj;

        function moveMouse(e) {
            divId = document.getElementById("content");
            var clientWidth = divId.clientWidth;
            var clientHeight = divId.clientHeight;
            var zoom = parseInt(oDragObj.style.zoom, 10) || 100;
            var iLeft;
            var iTop;
            if (isdrag && (oDragObj.offsetWidth * zoom / 100 > clientWidth || oDragObj.offsetHeight * zoom / 100 > clientHeight)) {
                if (oDragObj.offsetWidth * zoom / 100 > clientWidth) {
                    iLeft = Math.max(Math.min(nn6 ? nTX + e.clientX - x : nTX + event.clientX - x, 0), (divId.clientWidth - oDragObj.offsetWidth * zoom / 100));
                    oDragObj.style.left = iLeft + "px";
                }
                if (oDragObj.offsetHeight * zoom / 100 > divId.clientHeight) {
                    iTop = Math.max(Math.min(nn6 ? nTY + e.clientY - y : nTY + event.clientY - y, 0), (divId.clientHeight - oDragObj.offsetHeight * zoom / 100));
                    oDragObj.style.top = iTop + "px";
                }
            }



            return false;
        }
        // 拖拽方法
        function initDrag(e) {
            var oDragHandle = nn6 ? e.target : event.srcElement;
            var topElement = "HTML";
            while (oDragHandle.tagName != topElement && oDragHandle.className != "dragAble") {
                oDragHandle = nn6 ? oDragHandle.parentNode : oDragHandle.parentElement;
            }
            if (oDragHandle.className == "dragAble") {
                isdrag = true;
                oDragObj = oDragHandle;
                nTY = parseInt(oDragObj.style.top + 0);
                y = nn6 ? e.clientY : event.clientY;
                nTX = parseInt(oDragObj.style.left + 0);
                x = nn6 ? e.clientX : event.clientX;
                document.onmousemove = moveMouse;
                //document.onmouseup=MUp;// 事件会在鼠标按键被松开时发生
                return false;
            }
        }
        document.onmousedown = initDrag;
        document.onmouseup = new Function("isdrag=false");
        //上下左右移动
        function clickMove(s) {
            if (s == "up") {
                dragObj.style.top = parseInt(dragObj.style.top) + 100;
            } else {
                if (s == "down") {
                    dragObj.style.top = parseInt(dragObj.style.top) - 100;
                } else {
                    if (s == "left") {
                        dragObj.style.left = parseInt(dragObj.style.left) + 100;
                    } else {
                        if (s == "right") {
                            dragObj.style.left = parseInt(dragObj.style.left) - 100;
                        }
                    }
                }
            }
        }
        //缩小倍数
        function smallit() {
            //将图片缩小，失去热点
            height1 = images1.height;
            width1 = images1.width;
            images1.height = height1 / 1.1;
            images1.width = width1 / 1.1;
        }
        //放大倍数
        function bigit() {

            //将图片放大，失去热点
            height1 = images1.height;
            width1 = images1.width;
            images1.height = height1 * 1.1;
            images1.width = width1 * 1.1;
        }
        //还原
        function realsize() {
            images1.style.zoom = 100 + "%";
            images1.height = images2.height;
            images1.width = images2.width;
            divId.style.left = v_left;
            divId.style.top = v_top;
        }
        function featsize() {
            var width1 = images2.width;
            var height1 = images2.height;
            var width2 = 360;
            var height2 = 200;
            var h = height1 / height2;
            var w = width1 / width2;
            if (height1 < height2 && width1 < width2) {
                images1.height = height1;
                images1.width = width1;
            } else {
                if (h > w) {
                    images1.height = height2;
                    images1.width = width1 * height2 / height1;
                } else {
                    images1.width = width2;
                    images1.height = height1 * width2 / width1;
                }
            }
            block1.style.left = 0;
            block1.style.top = 0;
        }
        //鼠标滚轮放大缩小
        function bbimg(o) {
            var zoom = parseInt(o.style.zoom, 10) || 100;
            zoom += event.wheelDelta / 12;
            if (zoom > 0) {
                o.style.zoom = zoom + "%";

            }
            return false;
        }

        $.blockUI.defaults.fadeIn = 500;
        $.blockUI.defaults.fadeOut = 500;
        function userChgPwd() {
            $.blockUI({ message: $('#ChgPwdDiv'), css: { width: '275px'} });
        }
        function chgpwd() {
            debugger;
            var oldpwd = $("#OldPwd").val();
            var newpwd = $("#NewPwd").val();
            var newpwd2 = $("#NewPwd2").val();

            if (oldpwd.length == 0) {
                $.blockUI({ message: "<h1>原密码未输入!</h1>" });
                $.unblockUI();
                return false;
            }
            else if (newpwd.length == 0 || newpwd2.length == 0) {
                $.blockUI({ message: "<h1>请输入新密码!</h1>" });
                $.unblockUI();
                return false;
            }
            else if (newpwd != newpwd2) {
                $.blockUI({ message: "<h1>密码不一致!</h1>" });
                $.unblockUI();
                return false;
            }
            else if (newpwd.length < 4) {
                $.blockUI({ message: "<h1>密码至少需要4位!</h1>" });
                $.unblockUI();
                return false;
            }
            else if (newpwd.length > 16) {
                $.blockUI({ message: "<h1>密码不能超过16位!</h1>" });
                $.unblockUI();
                return false;
            }
            else if (oldpwd != null && newpwd != null) {
                $.unblockUI();
                if (window.XMLHttpRequest) {
                    xmlHttp = new XMLHttpRequest();
                }
                else {
                    xmlHttp = new ActiveXObject("Microsoft.XMLHTTP");
                }
                if (xmlHttp) {
                    xmlHttp.open("GET", "validatePwd.aspx?oldpwd=" + oldpwd + "&newpwd=" + newpwd, true);
                    xmlHttp.onreadystatechange = getdata;
                    xmlHttp.send();
                }
            }
        }
        function getdata() {
            debugger;
            if (xmlHttp.readyState == 4 && xmlHttp.status == 200) {
                alert(xmlHttp.responseText);
            }
        } function userChgPwd() {
            $.blockUI({ message: $('#ChgPwdDiv'), css: { width: '275px'} });
        }
        function chgpwd() {
            debugger;
            var oldpwd = $("#OldPwd").val();
            var newpwd = $("#NewPwd").val();
            var newpwd2 = $("#NewPwd2").val();

            if (oldpwd.length == 0) {
                $.blockUI({ message: "<h1>原密码未输入!</h1>" });
                $.unblockUI();
                return false;
            }
            else if (newpwd.length == 0 || newpwd2.length == 0) {
                $.blockUI({ message: "<h1>请输入新密码!</h1>" });
                $.unblockUI();
                return false;
            }
            else if (newpwd != newpwd2) {
                $.blockUI({ message: "<h1>密码不一致!</h1>" });
                $.unblockUI();
                return false;
            }
            else if (newpwd.length < 4) {
                $.blockUI({ message: "<h1>密码至少需要4位!</h1>" });
                $.unblockUI();
                return false;
            }
            else if (newpwd.length > 16) {
                $.blockUI({ message: "<h1>密码不能超过16位!</h1>" });
                $.unblockUI();
                return false;
            }
            else if (oldpwd != null && newpwd != null) {
                $.unblockUI();
                if (window.XMLHttpRequest) {
                    xmlHttp = new XMLHttpRequest();
                }
                else {
                    xmlHttp = new ActiveXObject("Microsoft.XMLHTTP");
                }
                if (xmlHttp) {
                    xmlHttp.open("GET", "validatePwd.aspx?oldpwd=" + oldpwd + "&newpwd=" + newpwd, true);
                    xmlHttp.onreadystatechange = getpwddata;
                    xmlHttp.send();
                }
            }
        }
        function getpwddata() {
            debugger;
            if (xmlHttp.readyState == 4 && xmlHttp.status == 200) {
                alert(xmlHttp.responseText);
            }
        }
        function showInfo() {
            $.blockUI({ message: $('#DivInfo'), css: { width: '275px'} });
        }
        function submitInfo() {
            var truename = $("#TrueName").val();
            var workplace = $("#WorkPlace").val();
            truename = escape(truename);
            workplace = escape(workplace);
            if (truename.length == 0) {
                alert("请输入真实姓名!");
                $.unblockUI();
                return false;
            }
            if (workplace.length == 0) {
                alert("请输入工作单位!");
                $.unblockUI();
                return false;
            }
            else if (truename.length != 0 && workplace.length != 0) {
                $.unblockUI();
                if (window.XMLHttpRequest) {
                    xmlHttp = new XMLHttpRequest();
                }
                else {
                    xmlHttp = new ActiveXObject("Microsoft.XMLHTTP");
                }
                if (xmlHttp) {
                    xmlHttp.open("GET", "info.aspx?truename=" + truename + "&workplace=" + workplace, true);
                    xmlHttp.onreadystatechange = getdata;
                    xmlHttp.send();
                }
            }

        }
        function getdata() {
            debugger;
            if (xmlHttp.readyState == 4 && xmlHttp.status == 200) {
                document.getElementById("LabelLoginName").innerText = xmlHttp.responseText;
            }
        }
        Date.prototype.DatePart = function (interval) {
            var myDate = new Date();
            var partStr = '';
            var Week = ['日', '一', '二', '三', '四', '五', '六'];
            switch (interval) {
                case 'y': partStr = myDate.getFullYear(); break;
                case 'm': partStr = myDate.getMonth() + 1; break;
                case 'd': partStr = myDate.getDate(); break;
                case 'w': partStr = Week[myDate.getDay()]; break;
                case 'ww': partStr = myDate.WeekNumOfYear(); break;
                case 'h': partStr = checkTime(myDate.getHours()); break;
                case 'n': partStr = checkTime(myDate.getMinutes()); break;
                case 's': partStr = checkTime(myDate.getSeconds()); break;
            }
            return partStr;
        }
        function checkTime(i) {
            if (i < 10) {
                i = "0" + i;
            }
            return i;
        }
        function getCurrentTime() {
            var cur_time = "";
            cur_time += Date.prototype.DatePart("y") + "年";
            cur_time += Date.prototype.DatePart("m") + "月";
            cur_time += Date.prototype.DatePart("d") + "日 ";
            cur_time += "星期" + Date.prototype.DatePart("w") + " ";
            cur_time += Date.prototype.DatePart("h") + ":";
            cur_time += Date.prototype.DatePart("n") + ":";
            cur_time += Date.prototype.DatePart("s");
            document.getElementById("b_current_time").innerHTML = cur_time;
            setTimeout(getCurrentTime, 1000);
        }
        //        function zhongcaiclick() {
        //            document.getElementById("select").disabled = true;
        //        }
        //        function wenticlick() {
        //            document.getElementById("select").disabled = false;
        //        }
        function regInput(obj, inputStr) {
            var reg = /^\d*\.?\d{0,1}$/;
            var docSel = document.selection.createRange()
            if (docSel.parentElement().tagName != "INPUT") return false
            oSel = docSel.duplicate()
            oSel.text = ""
            var srcRange = obj.createTextRange()
            oSel.setEndPoint("StartToStart", srcRange)
            var str = oSel.text + inputStr + srcRange.text.substr(oSel.text.length)
            return reg.test(str)
        }  
         // <input onkeypress="return regInput(this,String.fromCharCode(event.keyCode))"
          //  onpaste="return regInput(this,window.clipboardData.getData('Text'))"
         //   ondrop="return regInput(this,event.dataTransfer.getData('Text'))"> 
        //处理键盘事件 禁止后退键（Backspace）密码或单行、多行文本框除外
        function banBackSpace(e) {
            var ev = e || window.event; //获取event对象
            var obj = ev.target || ev.srcElement; //获取事件源
            var t = obj.type || obj.getAttribute('type'); //获取事件源类型
            //获取作为判断条件的事件类型
            var vReadOnly = obj.readOnly;
            var vDisabled = obj.disabled;
            //处理undefined值情况
            vReadOnly = (vReadOnly == undefined) ? false : vReadOnly;
            vDisabled = (vDisabled == undefined) ? true : vDisabled;
            //当敲Backspace键时，事件源类型为密码或单行、多行文本的，
            //并且readOnly属性为true或disabled属性为true的，则退格键失效
            var flag1 = ev.keyCode == 8 && (t == "password" || t == "text" || t == "textarea") && (vReadOnly == true || vDisabled == true);
            //当敲Backspace键时，事件源类型非密码或单行、多行文本的，则退格键失效
            var flag2 = ev.keyCode == 8 && t != "password" && t != "text" && t != "textarea";
            //判断
            if (flag2 || flag1) return false;
        }
        //禁止退格键 作用于Firefox、Opera
        document.onkeypress = banBackSpace;
        //禁止退格键 作用于IE、Chrome
        document.onkeydown = banBackSpace;

        var isIe = (document.all) ? true : false;
        function keydown(e) {
            var currentForm = document.getElementById('form1');
            var table = document.getElementById('GridViewScore');

            var e = e || event;
            var currKey = e.keyCode || e.which || e.charCode;
            if (currKey == 13) {
                var el = e.srcElement || e.target;
                if (el.tagName.toLowerCase() == "input" && el.type != "submit") {
                    if (isIe) {
                        currKey = 9;
                    }
                    else {
                        if (el.parentNode.parentNode.rowIndex == table.rows.length - 1) {

                        }
                        else {
                            nextCtl(el).focus();
                            e.preventDefault();
                        }
                    }
                }
            }
        }
        function nextCtl(ctl) {
            var form = ctl.form;
            for (var i = 0; i < form.elements.length - 1; i++) {
                if (ctl == form.elements[i]) {
                    return form.elements[i + 1];
                }
            }
            return ctl;
        }
    </script>
</head>
<body onload="getCurrentTime();">
    <form id="form1" runat="server">
    <div id="ChgPwdDiv" style="display: none; cursor: default">
        <h3>
            请输入原密码和新密码!</h3>
        <table style="width: 300px; text-align: center">
            <tr>
                <td>
                    原密码
                </td>
                <td>
                    <input type="password" id="OldPwd" autocomplete="off" />
                </td>
            </tr>
            <tr>
                <td>
                    新密码
                </td>
                <td>
                    <input type="password" id="NewPwd" autocomplete="off" />
                </td>
            </tr>
            <tr>
                <td>
                    确认密码
                </td>
                <td>
                    <input type="password" id="NewPwd2" autocomplete="off" />
                </td>
            </tr>
            <tr>
                <td colspan="2" align="center">
                    <input id="Button2" onclick="chgpwd()" type="button" value="确认" />
                </td>
            </tr>
        </table>
    </div>
    <div id="DivInfo" style="display: none; cursor: default">
        <h3>
            请输入个人信息！</h3>
        <table style="width: 300px; text-align: center">
            <tr>
                <td>
                    真实姓名
                </td>
                <td>
                    <input type="text" id="TrueName" autocomplete="off" />
                </td>
            </tr>
            <tr>
                <td>
                    工作单位
                </td>
                <td>
                    <input type="text" id="WorkPlace" autocomplete="off" />
                </td>
            </tr>
            <tr>
                <td colspan="2" align="center">
                    <input type="button" id="ButtonInfo" onclick="submitInfo()" value="提交" />
                </td>
            </tr>
        </table>
    </div>
    <div id="container">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <label id="lblMsg" runat="server">
        </label>
        <input type="hidden" name="hidden" id="hidden" runat="server" />
        <input type="hidden" name="hidden1" id="hidden1" runat="server" />
        <div id="header_two">
            <fieldset>
                <legend>系统菜单</legend>
                <asp:UpdatePanel ID="UpdateSystemMenu" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <table cellpadding="0" cellspacing="0" width="100%" border="0">
                            <tr>
                                <td>
                                    <table class="ContentBorder" cellpadding="0" cellspacing="0" width="100%">
                                        <tr>
                                            <td valign="top">
                                                <asp:MultiView ID="mvCompany" runat="server" ActiveViewIndex="0">
                                                    <asp:View ID="View1" runat="server">
                                                        <asp:Button ID="ButtonStart" runat="server" Text="评阅试卷" Style="margin-right: 10px" />
                                                        <input id="Buttonyichang" type="button" value="质量监控" style="margin-right: 10px" />
                                                        <asp:Button ID="Button_up" runat="server" Text="申请抽调纸卷" OnClick="ButtonUp_Click" style="margin-right: 10px" />
                                                        <asp:Button ID="Buttonyangjuan" CssClass="Button" runat="server" Text="样卷库浏览" Style="margin-right: 10px" />
                                                        <!--               <asp:Button ID="Buttonhongkuang" CssClass="Button" runat="server" Text="去除套红框" Style="margin-right: 10px" /> -->
                                                        <input id="Buttonxinxi" type="button" value="用户信息登记" style="margin-right: 10px" onclick="showInfo()" />
                                                        <input id="Buttonmima" type="button" value="修改用户密码" style="margin-right: 10px" onclick="userChgPwd();" />
                                                        <asp:Button ID="Buttonzhuxiao" CssClass="Button" runat="server" Text="用户注销" 
                                                            Style="margin-right: 10px" onclick="Buttonzhuxiao_Click" />
                                                    </asp:View>
                                                </asp:MultiView>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>

                </asp:UpdatePanel>
            </fieldset>
        </div>
        <div id="mainContent">
            <asp:UpdatePanel ID="UpdateImage" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div id="content" style="margin-left: auto; margin-right: auto;background-color:White">
                        <asp:Image ID="img" runat="server" CssClass="dragAble" onmousewheel="return bbimg(this)"/>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="ButtonSubmit" />
                    <asp:AsyncPostBackTrigger ControlID="ButtonOK" />
                    <asp:AsyncPostBackTrigger ControlID="GridViewCheck" />
                    <asp:AsyncPostBackTrigger ControlID="ButtonUp" />
                    <asp:AsyncPostBackTrigger ControlID="Button_up" />
                    <asp:AsyncPostBackTrigger ControlID="RadioButtonList1" />
                </Triggers>
            </asp:UpdatePanel>
            <div id="sidebar" style="text-align: center;">
                <div id="givescore" class="Title">
                    <asp:Label ID="LabelScore" runat="server" ForeColor="#FFFFFF" Text="试卷给分区" Font-Bold="True"></asp:Label>
                </div>
                <asp:UpdatePanel ID="UpdateScore" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div id="sidebarup" style="overflow: auto; width: 100%">
                            <asp:GridView ID="GridViewScore" runat="server" EnableModelValidation="True" AllowPaging="True"
                                AutoGenerateColumns="False" DataKeyNames="步骤" CellPadding="4" ForeColor="#333333"
                                Width="100%" GridLines="None">
                                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                <Columns>
                                    <asp:ButtonField CommandName="SingleClick" Text="SingleClick" Visible="False" />
                                    <asp:BoundField DataField="步骤" HeaderText="步骤" InsertVisible="False" ReadOnly="True"
                                        SortExpression="步骤" ItemStyle-HorizontalAlign="Center">
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:TemplateField HeaderText="分数" SortExpression="分数">
                                        <ItemTemplate>
                                             <asp:TextBox ID="txtScore" runat="server" Text='<%# Eval("分数") %>' Height="16px"
                                                Width="30px" AutoCompleteType="Disabled" onkeydown="keydown()"></asp:TextBox>
                                            <%--<cc1:CustomValidator ID="CustomValidator1" runat="server" ControlToValidate="txtScore" Display="None" ValidateEmptyText="true" OnServerValidate="CustomValidator1_ServerValidate"></cc1:CustomValidator>--%>
                                                <asp:CustomValidator ID="CustomValidator1" runat="server" ControlToValidate="txtScore"
                                                Display="None" ValidateEmptyText="True" OnServerValidate="CustomValidator1_ServerValidate"></asp:CustomValidator>
                                            
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="满分" HeaderText="满分" InsertVisible="False" ReadOnly="True"
                                        SortExpression="满分" ItemStyle-HorizontalAlign="Center">
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="允许给半分" HeaderText="允许给半分" InsertVisible="False" Visible="false"
                                        ReadOnly="True" ItemStyle-HorizontalAlign="Center">
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:BoundField>
                                </Columns>
                                <EditRowStyle BackColor="#999999" />
                                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                                <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                            </asp:GridView>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ButtonSubmit" />
                        <asp:AsyncPostBackTrigger ControlID="ButtonOK" />
                        <asp:AsyncPostBackTrigger ControlID="GridViewCheck" />
                        <asp:AsyncPostBackTrigger ControlID="ButtonUp" />
                        <asp:AsyncPostBackTrigger ControlID="Button_up" />
                    </Triggers>
                </asp:UpdatePanel>
                <asp:UpdatePanel ID="UpdatePanel_submit" runat="server">
                    <ContentTemplate>
                        <div id="submitbutton" style="text-align: center">
                            <asp:Button ID="ButtonSubmit" runat="server" Style="margin-right: 10px; margin-top: 3px"
                                Text="分数提交" onclick="ButtonSubmit_Click" />
                            <%--<asp:Button ID="ButtonBlank" runat="server" Style="margin-left: 10px; margin-top: 3px"
                                Text="空白卷"/>--%>
                        </div>
                        <div id="yichangbutton" style="text-align: center">
                              
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <div id="checkname" class="Title">
                    <asp:Label ID="LabelCheck" runat="server" Text="试卷重查区" ForeColor="#FFFFFF" Font-Bold="True"></asp:Label>
                </div>
                <asp:UpdatePanel ID="UpdatePanelCheck" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="sidebardown" style="overflow: auto;">
                            <asp:GridView ID="GridViewCheck" runat="server" EnableModelValidation="True" OnRowDataBound="GridViewCheck_RowDataBound"
                                ForeColor="#333333" AutoGenerateColumns="False" DataKeyNames="试卷号" AllowPaging="True"
                                PageSize="3" Width="100%">
                                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                <Columns>
                                    <asp:BoundField HeaderText="序号" DataField="序号" InsertVisible="False" ReadOnly="True">
                                        <HeaderStyle Wrap="False" />
                                        <ItemStyle Wrap="False" />
                                    </asp:BoundField>
                                    <asp:TemplateField HeaderText="试卷号" InsertVisible="False" SortExpression="试卷号">
                                        <HeaderStyle Wrap="false" />
                                        <ItemStyle Wrap="false" />
                                        <ItemTemplate>
                                            <asp:Label ID="GridPaperNo" runat="server" Text='<%# Bind("试卷号") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="分数" DataField="分数" InsertVisible="False" ReadOnly="True">
                                        <HeaderStyle Wrap="False" />
                                        <ItemStyle Wrap="False" />
                                    </asp:BoundField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:LinkButton ID="LinkButtonSelect" runat="server" OnClick="LinkButtonSelect_Click" CausesValidation="False" CommandName="Select"
                                                Text="选择"></asp:LinkButton>
                                        </ItemTemplate>
                                        <HeaderStyle Wrap="False" />
                                        <ItemStyle Wrap="False" />
                                    </asp:TemplateField>
                                </Columns>
                                <EditRowStyle BackColor="#999999" />
                                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                            </asp:GridView>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ButtonSubmit" />
                        <asp:AsyncPostBackTrigger ControlID="ButtonOK" />
                        <asp:AsyncPostBackTrigger ControlID="GridViewCheck" />
                    </Triggers>
                </asp:UpdatePanel>
                <asp:UpdatePanel runat="server" ID="UpdatePanelType" UpdateMode="Conditional">
                <ContentTemplate>
                <div id="papertype" class="Title">
                    <asp:Label ID="LabelQty" runat="server" Text="试卷类型选择" ForeColor="#FFFFFF" Font-Bold="True"></asp:Label>
                </div>
                <div id="typeselect" class="Title" style="background-color: #8DEEEE;">
                    <asp:RadioButtonList runat="server" ID="RadioButtonList1" BorderStyle="None" RepeatLayout="Flow"
                        RepeatDirection="Horizontal" AutoPostBack="true" 
                        onselectedindexchanged="RadioButtonList1_SelectedIndexChanged">
                        <asp:ListItem Value="0">仲裁卷</asp:ListItem>
                        <asp:ListItem Value="1">问题卷</asp:ListItem>
                    </asp:RadioButtonList>
                    <br />
                    <asp:DropDownList ID="Select" runat="server" Enabled="false" 
                        onselectedindexchanged="Select_SelectedIndexChanged" AutoPostBack="true">
                        <asp:ListItem Value="0">提取所有问题试卷</asp:ListItem>
                        <asp:ListItem Value="1">试卷不清晰</asp:ListItem>
                        <asp:ListItem Value="2">试卷残缺</asp:ListItem>
                        <asp:ListItem Value="3">答题位置错误</asp:ListItem>
                        <asp:ListItem Value="4">乱写乱画</asp:ListItem>
                        <asp:ListItem Value="5">自定义异常原因</asp:ListItem>
                    </asp:DropDownList>
                    <br />
                    <asp:Button ID="ButtonOK" runat="server" Style="margin-top: 3px"
                        Text="确定" onclick="ButtonOK_Click" Enabled="False" />
                    <asp:Button ID="ButtonUp" runat="server" Style="margin-left: 10px; margin-top: 3px"
                        Text="申请抽调纸卷" onclick="ButtonUp_Click" Enabled="False" />
                </div>
                <div id="detail" class="Title">
                    <asp:Label ID="Labelzhongcai" runat="server" Text="仲裁试卷区" ForeColor="#FFFFFF" Font-Bold="True"></asp:Label>
                    <asp:Label ID="Labelwenti" runat="server" Text="问题试卷区" ForeColor="#FFFFFF" Font-Bold="True"
                        Visible="false"></asp:Label>
                </div>
                <div id="detailgw" style="overflow: auto;">
                    <asp:GridView ID="GridViewDetail" runat="server" Width="100%" CellPadding="4" ForeColor="#333333"
                        AutoGenerateColumns="False" DataKeyNames="阅卷人" PageSize="5">
                        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                        <Columns>
                            <asp:BoundField HeaderText="阅卷人" DataField="阅卷人" InsertVisible="False" ReadOnly="True">
                                <HeaderStyle Wrap="False" />   
                                <ItemStyle Wrap="False" />
                            </asp:BoundField>
                            <asp:BoundField HeaderText="分数" DataField="分数" InsertVisible="False" ReadOnly="True" Visible="true">
                                <HeaderStyle Wrap="False" />
                                <ItemStyle Wrap="False" />
                            </asp:BoundField>
                            <asp:BoundField HeaderText="详细分数" DataField="详细分数" InsertVisible="False" ReadOnly="True" Visible="true">
                                <HeaderStyle Wrap="False" />
                                <ItemStyle Wrap="False" />
                            </asp:BoundField>
                            <asp:BoundField HeaderText="异常原因" DataField="异常原因" InsertVisible="False" ReadOnly="True" Visible="false">
                                <HeaderStyle Wrap="False" />
                                <ItemStyle Wrap="False" />
                            </asp:BoundField>
                        </Columns>
                        <EditRowStyle BackColor="#999999" />
                        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                      
                    </asp:GridView>
                </div>
                </ContentTemplate>
          
                </asp:UpdatePanel>
            </div>
        </div>
        <div id="div4" style="clear: both; height: 4px;">
        </div>
        <div id="footer">
            <b id="b_current_time" style="color: white" class="FootDate"></b>
            <table>
                <tr>
                    <td>
                        <asp:UpdatePanel ID="UpdateLabelName" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Label ID="LabelName" runat="server" ForeColor="#FFFFFF" Font-Bold="True"></asp:Label>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                    
                    <td>
                        <asp:UpdatePanel ID="UpdatePanelPaperNo" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Label ID="LabelPaperNo" Style="margin-left: 10px" runat="server" ForeColor="#FFFFFF"
                                    Font-Bold="True"></asp:Label>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                    <td>
                        <asp:UpdatePanel ID="UpdatePanelStatus" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Label ID="LabelStatus" Style="margin-left: 10px" runat="server" ForeColor="#FFFFFF"
                                    Font-Bold="True"></asp:Label>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                    <td>
                    
                        <asp:UpdatePanel ID="UpdatePanelPaperNum" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Label ID="LabelPaperNum" Style="margin-left: 10px" runat="server" ForeColor="#FFFFFF"
                                    Font-Bold="True"></asp:Label>
                           <%-- <asp:Timer ID="TimerTask" runat="server"></asp:Timer>--%>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                    <td>
                        <asp:UpdatePanel ID="UpdatePanelLoginName" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Label ID="LabelLoginName" Style="margin-left: 40px" runat="server" ForeColor="#FFFFFF"
                                    Font-Bold="True"></asp:Label>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    </form>
</body>
</html>
