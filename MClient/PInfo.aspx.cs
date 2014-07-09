using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using MClient.App_Code;

namespace MClient
{
    public partial class PInfo : System.Web.UI.Page
    {
        TDM_Client DM_Client;
        protected void Page_Load(object sender, EventArgs e)
        {
           
            DM_Client = (TDM_Client)Session["DM_Client"];

        }
        protected void infocancel_Click(object sender, EventArgs e)
        {
            TextBoxName.Text = "";
            TextBoxOffice.Text = "";
        }
        protected void infosubmit_Click(object sender, EventArgs e)
        {
            
            UMsgDefine.FM_ChgPwd_Req fmChgReq;
            UMsgDefine.FM_ChgPwd_Rsp fmChgRsp;
            Boolean ToChangePwd;

            if (TextBoxName.Text.Length == 0)
            {
                ScriptManager.RegisterStartupScript(this.UpdatePanel1, this.UpdatePanel1.GetType(), "msgName", "<script>alert('输入姓名不能为空');</script>", false);
                return;
            }
            else if (TextBoxOffice.Text.Length == 0)
            {
                ScriptManager.RegisterStartupScript(this.UpdatePanel1, this.UpdatePanel1.GetType(), "msgName", "<script>alert('输入单位不能为空');</script>", false);
                return;
            }

            fmChgReq.NewPwd = "".PadRight(UConstDefine.PwdLen, '\0').ToCharArray();
            fmChgReq.OldPwd = "".PadRight(UConstDefine.PwdLen, '\0').ToCharArray();
            fmChgReq.UserID = 0;

            fmChgReq.MsgHead.MsgType = UConstDefine.TM_CHGPWD_REQ;
            fmChgReq.MsgHead.MsgLength = Marshal.SizeOf(typeof(UMsgDefine.FM_ChgPwd_Req));
            fmChgReq.UserID = DM_Client.MyRecords.UserInfo.UserID;
            fmChgReq.OldPwd = ((string)Session["Password"]).PadRight(UConstDefine.PwdLen, '\0').ToCharArray();
            fmChgReq.NewPwd = ((string)Session["Password"]).PadRight(UConstDefine.PwdLen, '\0').ToCharArray();
            fmChgReq.TrueName = TextBoxName.Text;//.PadRight(UConstDefine.NameLen, '\0').ToCharArray();
            fmChgReq.ServeFor = TextBoxOffice.Text;//.PadRight(UConstDefine.ServeForLen, '\0').ToCharArray();
            lock (DM_Client.CsSocket)
            {
                byte[] Message = UMsgDefine.StructToBytes(fmChgReq);
                DM_Client.TCPSocket.Send(Message, fmChgReq.MsgHead.MsgLength, SocketFlags.None);

                byte[] RcvMessage = new byte[Marshal.SizeOf(typeof(UMsgDefine.FM_ChgPwd_Rsp))];
                DM_Client.TCPSocket.Receive(RcvMessage, Marshal.SizeOf(typeof(UMsgDefine.FM_ChgPwd_Rsp)), SocketFlags.None);

                fmChgRsp = (UMsgDefine.FM_ChgPwd_Rsp)UMsgDefine.BytesToStruct(RcvMessage, typeof(UMsgDefine.FM_ChgPwd_Rsp));
                if (fmChgRsp.MsgHead.MsgType == UConstDefine.TM_CHGPWD_RSP)
                {
                    switch (fmChgRsp.RspCode)
                    {
                        case UConstDefine.RM_RSP_OK:
                            DM_Client.MyRecords.UserInfo.TrueName = TextBoxName.Text.PadRight(UConstDefine.NameLen, '\0').ToCharArray();
                            DM_Client.MyRecords.UserInfo.ServeFor = TextBoxOffice.Text.PadRight(UConstDefine.ServeForLen, '\0').ToCharArray();
                            Session["DM_Client"] = DM_Client;
                            string js = "window.opener.document.getElementById('Buttoninfo').click();window.close();";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ParentButtonClick", js, true);
                            break;
                        case UConstDefine.RM_ERR_OLDPWD:
                            this.Response.Write("密码错误");
                            break;
                        case UConstDefine.RM_ERR_NOCHG:
                            this.Response.Write("服务器不允许更改密码");
                            break;
                        case UConstDefine.RM_ERR_USER:
                            this.Response.Write("用户名错误");
                            break;
                        case UConstDefine.RM_ERR_LOCKED:
                            this.Response.Write("帐号被锁定");
                            break;
                        case UConstDefine.RM_ERR_ERR:
                            this.Response.Write("未知错误");
                            break;
                    }
                }

            }
        }

    }
}