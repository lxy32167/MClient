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
    public partial class validatePwd : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            TDM_Client DM_Client;
            UMsgDefine.FM_ChgPwd_Req fmChgReq;
            UMsgDefine.FM_ChgPwd_Rsp fmChgRsp;
            string oldpwd = Request.QueryString["oldpwd"];
            string newpwd = Request.QueryString["newpwd"];
    
            DM_Client = (TDM_Client)Session["DM_Client"];
            fmChgReq.MsgHead.MsgType = UConstDefine.TM_CHGPWD_REQ;
            fmChgReq.MsgHead.MsgLength = Marshal.SizeOf(typeof(UMsgDefine.FM_ChgPwd_Req));
            fmChgReq.UserID = DM_Client.MyRecords.UserInfo.UserID;
            fmChgReq.OldPwd = oldpwd.PadRight(UConstDefine.PwdLen, '\0').ToCharArray();
            fmChgReq.NewPwd = newpwd.PadRight(UConstDefine.PwdLen, '\0').ToCharArray();
            fmChgReq.ServeFor = new String(DM_Client.MyRecords.UserInfo.ServeFor);
            fmChgReq.TrueName = new String(DM_Client.MyRecords.UserInfo.TrueName);

            lock (DM_Client.CsSocket)           //C#的socketsend其实不需要加锁，系统会自动对缓冲区上锁
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
                            this.Response.Write("修改成功");
                            //               //更改本地缓存的用户信息
                            //LStrCpy(DM_Client.MyRecords.UserInfo.TrueName,DM_Client.MyRecords.UserInfo.TrueName); 不知道要干嘛
                            //LStrCpy(DM_Client.MyRecords.UserInfo.ServeFor,DM_Client.MyRecords.UserInfo.ServeFor); 

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
                else
                {
                    this.Response.Write("错误的密码响应");
                }
            }
        }
    }
}