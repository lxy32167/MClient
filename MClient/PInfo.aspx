<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PInfo.aspx.cs" Inherits="MClient.PInfo" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <base target="_self">
    <script type="text/javascript" src="jquery-1.9.1.js"></script>
    <script type="text/javascript" src="jquery.blockUI.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="InfoDiv" style="cursor: default">
        <h3 style="text-align:center">
            请输入个人信息
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
        </h3>
        <table style="width: 350px; text-align: center">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <tr>
                        <td>
                            个人姓名
                        </td>
                        <td>
                            <asp:TextBox ID="TextBoxName" runat="server" Width="163px" />
                        </td>
                    </tr>
                    <tr>
                        <td class="style1">
                            工作单位
                        </td>
                        <td class="style1">
                            <asp:TextBox ID="TextBoxOffice" runat="server" Width="163px" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="center">
                            <asp:Button ID="infosubmit" runat="server" Text="提交" OnClick="infosubmit_Click" Style="margin-right: 40px" />
                            <asp:Button ID="infocancel" runat="server" Text="清空" OnClick="infocancel_Click" />
                        </td>
                    </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
