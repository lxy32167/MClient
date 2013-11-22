<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="MClient.Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>基于大数据中心的网上阅卷系统客户端</title>
    <link href="layout.css" rel="stylesheet" type="text/css" />

    <script type="text/javascript">
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
        function fullscreen() { //在ie下可行
            var wsh = new ActiveXObject("WScript.Shell");
            wsh.sendKeys("{F11}");
            
        }
    </script>
</head>
<body onload="getCurrentTime();">

    <form id="form1" runat="server">
        <div id="container">
            <div id="header" style="text-align: center; margin-left: auto; margin-right: auto">
                <asp:Label ID="Label1" runat="server" BorderColor="#666666" BorderStyle="Outset" BorderWidth="5px" Font-Bold="True" Font-Italic="True" Font-Names="华文仿宋" Font-Size="XX-Large" ForeColor="White" Text="网上阅卷系统客户端"></asp:Label>
            </div>
            <div id="mainContent">
                <div id="contentLogin" style="text-align: center; margin-left: auto; margin-right: auto; margin-top: auto;">
                    <asp:Login ID="LoginUser" runat="server" style="margin-top:150px" BackColor="#EEEEFF" DisplayRememberMe="false" InstructionText="输入用户名和密码登陆阅卷系统!" CssClass="loginbody" TitleTextStyle-CssClass="login_title" InstructionTextStyle-CssClass="login_instructions" LoginButtonStyle-CssClass="login_button" TitleText="网上阅卷系统客户端登录" OnAuthenticate="LoginUser_Authenticate" DestinationPageUrl = "~/Main.aspx">
                        <LayoutTemplate>
                            <table cellpadding="1" cellspacing="0" style="border-collapse: collapse;">
                                <tr>
                                    <td>
                                        <table cellpadding="0">
                                            <tr>
                                                <td align="center" class="login_title" colspan="2">网上阅卷系统客户端登录</td>
                                            </tr>
                                            <tr>
                                                <td align="center" class="login_instructions" colspan="2">输入用户名和密码登陆阅卷系统!</td>
                                            </tr>
                                            <tr>
                                                <td align="right">
                                                    <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">用户名:</asp:Label>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="UserName" runat="server"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName" ErrorMessage="必须填写“用户名”。" ToolTip="必须填写“用户名”。" ValidationGroup="LoginUser">*</asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" class="auto-style2">
                                                    <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">密码:</asp:Label>
                                                </td>
                                                <td class="auto-style2">
                                                    <asp:TextBox ID="Password" runat="server" TextMode="Password"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password" ErrorMessage="必须填写“密码”。" ToolTip="必须填写“密码”。" ValidationGroup="LoginUser">*</asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" class="auto-style2">
                                                    <asp:Label ID="ServerIPLabel" runat="server" AssociatedControlID="ServerIP">服务器IP:</asp:Label>
                                                </td>
                                                <td class="auto-style2">
                                                    <asp:TextBox ID="ServerIP" runat="server" Text="192.168.93.128"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="ServerIPRequired" runat="server" ControlToValidate="ServerIP" ErrorMessage="必须填写“服务器IP”。" ToolTip="必须填写“服务器IP”。" ValidationGroup="LoginUser">*</asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center" class="auto-style1" colspan="2" style="color:Red;">
                                                    <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" colspan="2">
                                                    <asp:Button ID="LoginButton" runat="server" CommandName="Login" CssClass="login_button" Text="登录" ValidationGroup="LoginUser" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </LayoutTemplate>
                        <LoginButtonStyle CssClass="login_button" />
                        <InstructionTextStyle CssClass="login_instructions" />
                        <TitleTextStyle CssClass="login_title" />
                    </asp:Login>
                </div>
                <div id="sidebar"></div>
            </div>
            <div id="div4" style="clear: both; height: 4px;"></div>
            <div id="footer">
                <b id="b_current_time" style="color: white" class="FootDate"></b>
                <asp:Label ID="Label2" runat="server" ForeColor="#FFFFFF" Text="您尚未登录！" Font-Bold="True"></asp:Label>

                
            </div>
        </div>
    </form>
</body>
</html>
