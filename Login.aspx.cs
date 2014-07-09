using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Runtime.InteropServices;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using MClient.App_Code;

namespace MClient
{
    public partial class Login : System.Web.UI.Page
    {
        IPAddress ServerAddr;
        int Len, i, j, ret;
        Boolean LogOK;
        TDM_Client DM_Client;
        int BtnStatus;
        UMyRecords.stServerInfo PAddSrvInfo, tmpSrvInfo;
        string ErrorMsg;
        [DllImportAttribute("ws2_32.dll")]
        private static extern int inet_addr(string cp);
        [DllImportAttribute("ws2_32.dll")]
        private static extern ushort ntohs(ushort netshort);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                Session.Clear();
        }
   
        protected void LoginUser_Authenticate(object sender, AuthenticateEventArgs e)
        {
            DM_Client = new TDM_Client();
            TBeaconThreadClass TBeaconClass = new TBeaconThreadClass(DM_Client);
            TUDPListenedClass TUDPClass = new TUDPListenedClass(DM_Client);
            UMsgDefine.FM_QueryUser_Req fmQueryUserReq;
            UMsgDefine.FM_QueryUser_Rsp fmQueryUserRsp;
            string TmpStr;
            UMyRecords.stFullBlockInfo tmpPBlkInfo;
            TextBox ServerIP = LoginUser.FindControl("ServerIP") as TextBox;
            string tempIP = ServerIP.Text;
            UMyFuncs.ClearUpInfo(DM_Client, 1);
            UMyFuncs.ClearUpInfo(DM_Client, 2);
            if (tempIP.IndexOf('.') == -1)
            {
                LoginUser.FailureText = "服务器IP不合法!";
                e.Authenticated = false;
                return;
            }
            else
            {
                tempIP = tempIP.Substring(tempIP.IndexOf('.') + 1);
                if (tempIP.IndexOf('.') == -1)
                {
                    LoginUser.FailureText = "服务器IP不合法!";
                    e.Authenticated = false;
                    return;
                }
                else
                {
                    tempIP = tempIP.Substring(tempIP.IndexOf('.') + 1);
                    if (tempIP.IndexOf('.') == -1)
                    {
                        LoginUser.FailureText = "服务器IP不合法!";
                        e.Authenticated = false;
                        return;
                    }
                }
            }
            if (inet_addr(ServerIP.Text) == -1)
            {
                LoginUser.FailureText = "服务器IP不合法!";
                e.Authenticated = false;
                return;
            }
            ServerAddr = IPAddress.Parse(ServerIP.Text);

            char[] UserName = LoginUser.UserName.ToCharArray();
            char[] a = { 'x', 't', 'd', 'e' };
            if (!a.Contains(UserName[0]))
            {
                LoginUser.FailureText = "此为组长使用的客户端!";
                e.Authenticated = false;
                return;
            }

            UWini.WriteUserInfo(ServerIP.Text, LoginUser.UserName, LoginUser.Password);

            if ((DM_Client.MyVars.UdpCreated) && !(DM_Client.MyVars.UdpBinded))
            {
                IPHostEntry iph = Dns.GetHostEntry(Dns.GetHostName());
                string svrAddress = string.Empty;
                for (i = 0; i < iph.AddressList.Length; i++)
                {
                    if (iph.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        svrAddress = iph.AddressList[i].ToString();
                        break;
                    }
                }
                if (svrAddress.Length != 0)
                {
                    IPAddress localAddr = IPAddress.Parse(svrAddress);

                    //               DM_Client.UDPSocket.Bind(new IPEndPoint(localAddr, UConstDefine.BrdRecvPort));

                    DM_Client.MyVars.UdpPort = UConstDefine.BrdRecvPort;
                    DM_Client.MyVars.UdpBinded = true;
                    DM_Client.MyVars.UdpAvailable = true;
                }

            }

            DM_Client.UDPSocket.Connect(ServerAddr, UConstDefine.MainSrvPort);

            MemoryStream RcvdStream = new MemoryStream();

         //   DetectServer(RcvdStream);
            fmQueryUserReq.MsgHead.MsgType = UConstDefine.TM_QUERYUSER_REQ;
            fmQueryUserReq.MsgHead.MsgLength = Marshal.SizeOf(typeof(UMsgDefine.FM_QueryUser_Req));
            fmQueryUserReq.UserName = LoginUser.UserName.PadRight(UConstDefine.NameLen, '\0').ToCharArray();
            byte[] Message = UMsgDefine.StructToBytes(fmQueryUserReq);
            DM_Client.UDPSocket.Send(Message, fmQueryUserReq.MsgHead.MsgLength, SocketFlags.None);


            i = 0;
            byte[] buff = new byte[8192];

            byte[] RcvMessage = new byte[Marshal.SizeOf(typeof(UMsgDefine.FM_QueryUser_Rsp))];
            while ((RcvdStream.Length == 0) && (i < 100))
            {
                ret = DM_Client.UDPSocket.Receive(buff, buff.Length, SocketFlags.None);
                if (ret > 0)
                {
                    RcvdStream.Write(buff, 0, ret);
                }
                i++;
                Thread.Sleep(10);
            }

            //     DM_Client.UDPSocket.Receive(RcvMessage, Marshal.SizeOf(typeof(UMsgDefine.FM_QueryUser_Rsp)), SocketFlags.None);
            if (RcvdStream.Length != 0)
            {
                RcvdStream.Position = 0;
                RcvdStream.Read(RcvMessage, 0, Marshal.SizeOf(typeof(UMsgDefine.FM_QueryUser_Rsp)));
                fmQueryUserRsp = (UMsgDefine.FM_QueryUser_Rsp)UMsgDefine.BytesToStruct(RcvMessage, typeof(UMsgDefine.FM_QueryUser_Rsp));

                //if(DM_Client.MyRecords.ArrSocket.Connected 
                DM_Client.ServerInfoList.Clear();
                if (fmQueryUserRsp.MsgHead.MsgType == UConstDefine.TM_QUERYUSER_RSP)
                {
                    if (fmQueryUserRsp.RspCode == UConstDefine.RM_RSP_OK)
                    {
                        if (fmQueryUserRsp.RecCount > 0)
                        {
                            Len = Marshal.SizeOf(typeof(UMyRecords.stServerInfo));
                            for (i = 0; i < fmQueryUserRsp.RecCount; i++)
                            {
                                RcvMessage = new byte[Marshal.SizeOf(typeof(UMyRecords.stServerInfo))];
                                RcvdStream.Read(RcvMessage, 0, Marshal.SizeOf(typeof(UMyRecords.stServerInfo)));
                                tmpSrvInfo = (UMyRecords.stServerInfo)UMsgDefine.BytesToStruct(RcvMessage, typeof(UMyRecords.stServerInfo));

                                PAddSrvInfo = new UMyRecords.stServerInfo();
                                PAddSrvInfo.IPAddr = tmpSrvInfo.IPAddr;
                                PAddSrvInfo.TCPPort = ntohs((ushort)tmpSrvInfo.TCPPort);
                                PAddSrvInfo.UDPCBrdPort = ntohs((ushort)tmpSrvInfo.UDPCBrdPort);
                                PAddSrvInfo.UDPSBrdPort = ntohs((ushort)tmpSrvInfo.UDPSBrdPort);
                                PAddSrvInfo.UDPUniPort = ntohs((ushort)tmpSrvInfo.UDPUniPort);

                                DM_Client.ServerInfoList.Add(PAddSrvInfo);
                                string test = new IPAddress(BitConverter.GetBytes(PAddSrvInfo.IPAddr)).ToString();
                            }
                            DM_Client.MyRecords.ArrSocket.Connect(new IPAddress(BitConverter.GetBytes(PAddSrvInfo.IPAddr)), PAddSrvInfo.TCPPort);
                        }
                        else
                        {
                            Response.Write("<script type='text/javascript'>alert('没有找到服务器信息');window.location.href='login.aspx';</script>");
                        }
                    }
                    else
                    {
                        if (fmQueryUserRsp.RspCode == UConstDefine.RM_ERR_USER)
                        {
                            Response.Write("<script type='text/javascript'>alert('用户名错误');window.location.href='login.aspx';</script>");
                        }
                        else if (fmQueryUserRsp.RspCode == UConstDefine.RM_ERR_ERR)
                        {
                            Response.Write("<script type='text/javascript'>alert('获取服务器产生未知错误');window.location.href='login.aspx';</script>");
                        }
                        else if (fmQueryUserRsp.RspCode == UConstDefine.RM_ERR_BLKNOSTART)
                        {
                            Response.Write("<script type='text/javascript'>alert('您所属的题块还未启动');window.location.href='login.aspx';</script>");
                        }
                    }
                }
                else
                {
                    if (fmQueryUserRsp.MsgHead.MsgType != UConstDefine.TM_PROBESRV_RSP)
                    {
                        Response.Write("<script type='text/javascript'>alert('收到错误的用户信息探测帧');window.location.href='login.aspx';</script>");
                    }
                }
                LogOK = true;
                ret = 0;

                ret++;
                LogOK = LogOK && LoginServer(DM_Client.MyRecords.ArrSocket, LoginUser.UserName, LoginUser.Password, true);

                if (!LogOK)
                {
                    //       UMyFuncs.ClearUpInfo(DM_Client, 1);   MClient cancel
                }

                if ((ret > 0) && LogOK)
                {
                    string tmpstr1 = new String(DM_Client.MyRecords.UserInfo.LoginName);
                    tmpstr1 = tmpstr1.Substring(0, tmpstr1.IndexOf('\0'));
                    FileStream ds = File.Open(@"C:/Debug_" + tmpstr1 + @".txt", FileMode.Append, FileAccess.Write, FileShare.None);
                    StreamWriter sw = new StreamWriter(ds);
                    sw.Dispose();
                    sw.Close();
                    ds.Dispose();
                    ds.Close();


                    if (DM_Client.MyRecords.UserInfo.Role > UConstDefine.PuTong)
                    {
                        while (!File.Exists("C:\\" + tmpstr1 + "mdb\\Qty.mdb"))
                        {
                            TDM_Client.CreateDataBase(DM_Client);
                            DM_Client.DAO = new TDAO(DM_Client);
                        }
                        UMyFuncs.GetGrpUserInfo(DM_Client);   //依次获取组成员信息、质量控制文件的原始文件和增量文件
                    }


                    TBeaconClass.TBeaconThread.Start();


                    DM_Client.MyRecords.BlockInfoList.Clear();
                    for (i = 0; i < DM_Client.MyRecords.AssignList.Count; i++)
                    {
                        TmpStr = DM_Client.MyRecords.AssignList.ElementAt(i);
                        Len = UMyFuncs.GetBlockInfo(DM_Client, 1, 0, TmpStr, ErrorMsg);
                        if (ErrorMsg != null && Len == -1)
                        {
                            LoginUser.FailureText = ErrorMsg;
                            e.Authenticated = false;
                            return;
                        }
                        if (Len > 0)
                        {
                            UMyFuncs.GetBlockInfo(DM_Client, 2, Len, TmpStr, ErrorMsg);
                        }
                    }

                    if (DM_Client.MyRecords.UserInfo.Role == UConstDefine.TiZuZhang || DM_Client.MyRecords.UserInfo.Role == UConstDefine.DaZuZhang)
                    {
                        getClassRoomMap(DM_Client);
                        if (DM_Client.MyRecords.BlockInfoList.Count > 0)
                        {
                            for (j = 0; j < DM_Client.MyRecords.BlockInfoList.Count; j++)
                            {
                                tmpPBlkInfo = DM_Client.MyRecords.BlockInfoList[j];
                                getRateMap(DM_Client, ref tmpPBlkInfo);
                            }
                        }
                    }
                    UMyFuncs.GetNextVol(DM_Client);

                    TUDPClass.TUDPListenedThread.Start();

                    e.Authenticated = true;

                }
                else
                {
                    //              UMyFuncs.ClearUpInfo(DM_Client, 1);
                }
            }
            else
            {
                Response.Write("<script type='text/javascript'>alert('没有收到服务器的响应信息');window.location.href='login.aspx';</script>");
            }
            DM_Client.MyVars.MaxTime = 2400000000;
            DM_Client.MyVars.MinTime = 0;
            Session["Password"] = LoginUser.Password;
            Session["DM_Client"] = DM_Client;
            Session["TBeaconClass"] = TBeaconClass;
            Session["TUDPClass"] = TUDPClass;
            if (e.Authenticated == true)
            {
                if (DM_Client.MyRecords.UserInfo.Role == UConstDefine.XiaoZuZhang)
                {
                    Response.Redirect("~/Xzz.aspx");
                }
            }
        }
 
        protected Boolean LoginServer(Socket LogSocket, string UserName, string Password, Boolean Reset)
        {
            UMsgDefine.FM_Auth_Req fmLogReq;
            UMsgDefine.FM_Auth_Rsp fmLogRsp;
            UMyRecords.stExcpMap TmpRsn, PTmpRsn;
            UMyRecords.stUserInfo TmpUserInfo;
            UMyRecords.stUserInfoAssignment TmpUserInfoAssignment;
            int RsnCount, Len, ret, AllAssignLen, l;
            double v, v1, sysTimeDis;
            MemoryStream RcvdStream = new MemoryStream();
            char[] buf = new char[8192];
            byte[] tmpbuf = new byte[8192];
            char[] Assignbuf, Assignment;
            fmLogReq.UserName = UserName.PadRight(UConstDefine.NameLen, '\0').ToCharArray();
            fmLogReq.UserPwd = Password.PadRight(UConstDefine.PwdLen, '\0').ToCharArray();
            fmLogReq.MsgHead.MsgType = UConstDefine.TM_AUTH_REQ;
            fmLogReq.MsgHead.MsgLength = Marshal.SizeOf(typeof(UMsgDefine.FM_Auth_Req));
            fmLogReq.AuthType = UConstDefine.PM_AUTH_NORMAL;
            fmLogReq.UDPPort = 0;
            lock (DM_Client.CsSocket)
            {
                byte[] Message = UMsgDefine.StructToBytes(fmLogReq);
                LogSocket.Send(Message, fmLogReq.MsgHead.MsgLength, SocketFlags.None);
                LogSocket.SendTimeout = UConstDefine.TimeOutSeconds;

                byte[] RcvMessage = new byte[Marshal.SizeOf(typeof(UMsgDefine.FM_Auth_Rsp))];
                LogSocket.Receive(RcvMessage, Marshal.SizeOf(typeof(UMsgDefine.FM_Auth_Rsp)), SocketFlags.None);
                fmLogRsp = (UMsgDefine.FM_Auth_Rsp)UMsgDefine.BytesToStruct(RcvMessage, typeof(UMsgDefine.FM_Auth_Rsp));

                if (fmLogRsp.MsgHead.MsgType == UConstDefine.TM_AUTH_RSP)
                {
                    if (fmLogRsp.RspCode == UConstDefine.RM_RSP_OK)
                    {
                        RsnCount = 0;
                        RcvMessage = new byte[Marshal.SizeOf(typeof(int))];
                        LogSocket.Receive(RcvMessage, Marshal.SizeOf(typeof(int)), SocketFlags.None);
                        RsnCount = BitConverter.ToInt32(RcvMessage, 0);

                        Len = RsnCount * Marshal.SizeOf(typeof(UMyRecords.stExcpMap));
                        if (RsnCount > 0)
                        {
                            ret = LogSocket.Receive(tmpbuf, Len, SocketFlags.None);
                        }
                        TmpUserInfo.TrueName = "".PadRight(UConstDefine.NameLen, '\0').ToCharArray();
                        TmpUserInfo.ServeFor = "".PadRight(UConstDefine.ServeForLen, '\0').ToCharArray();
                        TmpUserInfoAssignment.Assignment = "".PadRight(2000, '\0').ToCharArray();
                        //收取用户信息——读取至多8192字节
                        RcvMessage = new byte[Marshal.SizeOf(typeof(UMyRecords.stUserInfo))];
                        LogSocket.Receive(RcvMessage, Marshal.SizeOf(typeof(UMyRecords.stUserInfo)), SocketFlags.None);  //wrong
                        TmpUserInfo = (UMyRecords.stUserInfo)UMsgDefine.BytesToStruct(RcvMessage, typeof(UMyRecords.stUserInfo));
                        //收取用户责任试题卷名信息
                        RcvMessage = new byte[TmpUserInfo.AssignLen];
                        LogSocket.Receive(RcvMessage, TmpUserInfo.AssignLen, SocketFlags.None);
                        Assignbuf = new char[TmpUserInfo.AssignLen];
                        Assignbuf = System.Text.Encoding.Default.GetChars(RcvMessage);       //waiting for transforming to stUserInfoAssignment
                        Array.Copy(Assignbuf, 0, TmpUserInfoAssignment.Assignment, 0, Assignbuf.Length);
                        //读取所有责任试题卷名信息先获取长度
                        RcvMessage = new byte[Marshal.SizeOf(typeof(int))];
                        LogSocket.Receive(RcvMessage, Marshal.SizeOf(typeof(int)), SocketFlags.None);    //marshal.sizeof(char) = Ansi,sizeof(char) = unicode
                        AllAssignLen = BitConverter.ToInt32(RcvMessage, 0);
                        //再读取所有题块分卷
                        RcvMessage = new byte[AllAssignLen];
                        l = LogSocket.Receive(RcvMessage, AllAssignLen, SocketFlags.None);
                        if (l < AllAssignLen)
                        {
                            Response.Write("<script type='text/javascript'>alert('收到错误的任务长度信息');window.location.href='login.aspx';</script>");
                        }
                        else
                        {
                            Assignment = new char[AllAssignLen];
                            Assignment = System.Text.Encoding.Default.GetChars(RcvMessage);
                            string tempAssignment = new string(Assignment);
                            char[] Delimiter = new char[] { ',' };
                            string[] CommaText = tempAssignment.Substring(0, tempAssignment.IndexOf('\0')).Split(Delimiter, StringSplitOptions.RemoveEmptyEntries);
                            for (int i = 0; i < CommaText.Length; i++)
                            {
                                DM_Client.MyRecords.AllBlkList.Add(CommaText[i]);
                            }
                        }


                        UMyFuncs.GetAllBlkNo(DM_Client);
                        if (Reset)
                        {
                            DM_Client.MyRecords.UserInfo = TmpUserInfo;
                            v = UnitGlobalV.ts.TotalDays + (double)(fmLogRsp.ServerTime + 8 * 60 * 60) / 86400;
                            v1 = (DateTime.Now - UnitGlobalV.delphiTime).TotalDays;
                            DM_Client.sysTimeDis = Math.Round((v - v1), 6);

                            if (DM_Client.MyRecords.UserInfo.Role <= UConstDefine.PuTong)
                            {
                                Response.Write("<script type='text/javascript'>alert('普通用户无法进行质量监控');window.location.href='login.aspx';</script>");
                            }
                            if (DM_Client.MyRecords.UserInfo.Role > UConstDefine.PuTong)
                            {
                                UMyFuncs.DecodeInfo_chen(DM_Client, new string(Assignbuf), 1);
                                DM_Client.MyRecords.ExcpRsnList.Clear();
                                if (RsnCount > 0)
                                {
                                    RcvdStream.Flush();

                                    RcvdStream.Write(tmpbuf, 0, Len);
                                    RcvdStream.Position = 0;

                                    for (int i = 0; i < RsnCount; i++)
                                    {
                                        RcvMessage = new byte[Marshal.SizeOf(typeof(UMyRecords.stExcpMap))];

                                        RcvdStream.Read(RcvMessage, 0, Marshal.SizeOf(typeof(UMyRecords.stExcpMap)));  //wrong

                                        TmpRsn = (UMyRecords.stExcpMap)UMsgDefine.BytesToStruct(RcvMessage, typeof(UMyRecords.stExcpMap));
                                        if (TmpRsn.ReasonCode != UConstDefine.SelfExcpCode)
                                        {
                                            PTmpRsn = new UMyRecords.stExcpMap();
                                            PTmpRsn.ReasonCode = TmpRsn.ReasonCode;
                                            PTmpRsn.Reason = (char[])TmpRsn.Reason.Clone();
                                            DM_Client.MyRecords.ExcpRsnList.Add(PTmpRsn);
                                        }
                                    }
                                }
                            }
                        }
                        return true;
                    }
                    else
                    {
                        switch (fmLogRsp.RspCode)
                        {
                            case UConstDefine.RM_ERR_USER:
                                LoginUser.FailureText = "密码错误!";
                                break;
                            case UConstDefine.RM_ERR_LOGED:
                                LoginUser.FailureText = "用户已登陆!";
                                break;
                            case UConstDefine.RM_ERR_LOCKED:
                                LoginUser.FailureText = "该用户已锁定!";
                                break;
                            case UConstDefine.RM_ERR_ERR:
                                LoginUser.FailureText = "未知错误!";
                                break;
                        }
                        return false;
                    }
                }
                else
                {
                    LoginUser.FailureText = "收到错误的认证响应帧!";
                    return false;
                }
            }
        }
        /// <summary>
        /// 探测服务器地址，检测用户所在题块是否启动
        /// </summary>
        /// <param name="RcvdStream"></param>
        protected void DetectServer(MemoryStream RcvdStream)
        {
            UMsgDefine.FM_QueryUser_Req fmQueryUserReq;

            fmQueryUserReq.MsgHead.MsgType = UConstDefine.TM_QUERYUSER_REQ;
            fmQueryUserReq.MsgHead.MsgLength = Marshal.SizeOf(typeof(UMsgDefine.FM_QueryUser_Req));
            fmQueryUserReq.UserName = LoginUser.UserName.PadRight(UConstDefine.NameLen, '\0').ToCharArray();
            byte[] Message = UMsgDefine.StructToBytes(fmQueryUserReq);
            DM_Client.UDPSocket.Send(Message, fmQueryUserReq.MsgHead.MsgLength, SocketFlags.None);
     

            i = 0;
            byte[] buff = new byte[8192];
            //MemoryStream RcvdStream = new MemoryStream();
            byte[] RcvMessage = new byte[Marshal.SizeOf(typeof(UMsgDefine.FM_QueryUser_Rsp))];
            while ((RcvdStream.Length == 0) && (i < 100))
            {
                ret = DM_Client.UDPSocket.Receive(buff, buff.Length, SocketFlags.None);
                
                if (ret > 0)
                {
                    RcvdStream.Write(buff, 0, ret);
                }
                i++;
                Thread.Sleep(10);
            }
        }
        protected void getClassRoomMap(TDM_Client DM_Client)
        {
            UMsgDefine.FM_GETCLASSROOMMAP_REQ fmGetClassRoomMapReq;
            UMsgDefine.FM_GETCLASSROOMMAP_RSP fmGetClassRoomMapRsp;
            string tmpStr, tmpclass, tmpClassNo, tmpClassName;
            int i, len, total, code, classNo;
            byte[] tmpbuf;
            char[] bufstr;
            UMyRecords.stClassRoom tmpPClassRoom;

            fmGetClassRoomMapReq.MsgHead.MsgType = UConstDefine.TM_GETCLASSROOMMAP_REQ;
            fmGetClassRoomMapReq.MsgHead.MsgLength = Marshal.SizeOf(typeof(UMsgDefine.FM_GETCLASSROOMMAP_REQ));
            fmGetClassRoomMapReq.UserID = DM_Client.MyRecords.UserInfo.UserID;
            lock (DM_Client.CsSocket)
            {
                byte[] Message = UMsgDefine.StructToBytes(fmGetClassRoomMapReq);
                DM_Client.UDPSocket.Send(Message, fmGetClassRoomMapReq.MsgHead.MsgLength, SocketFlags.None);

                byte[] RcvMessage = new byte[Marshal.SizeOf(typeof(UMsgDefine.FM_GETCLASSROOMMAP_RSP))];
                DM_Client.UDPSocket.Receive(RcvMessage, RcvMessage.Length, SocketFlags.None);
                fmGetClassRoomMapRsp = (UMsgDefine.FM_GETCLASSROOMMAP_RSP)UMsgDefine.BytesToStruct(RcvMessage, typeof(UMsgDefine.FM_GETCLASSROOMMAP_RSP));
                if (fmGetClassRoomMapRsp.MsgHead.MsgType == UConstDefine.TM_GETCLASSROOMMAP_RSP)
                {
                    if (fmGetClassRoomMapRsp.RspCode == UConstDefine.RM_RSP_OK)
                    {
                        len = 0;
                        total = 0;
                        tmpStr = "";
                        tmpbuf = new byte[1024];
                        while (total < fmGetClassRoomMapRsp.ClsMapLen)
                        {
                            len = DM_Client.UDPSocket.Receive(tmpbuf, 1024, SocketFlags.None);
                            if (len > 0)
                            {
                                bufstr = System.Text.Encoding.Default.GetChars(tmpbuf);
                                tmpStr = tmpStr + bufstr;
                                total = total + len;
                            }
                        }
                        DM_Client.MyRecords.ClassRoomList = new List<UMyRecords.stClassRoom>();
                        DM_Client.MyRecords.ClassRoomList.Clear();
                        while (tmpStr.Length > 0)
                        {
                            tmpclass = tmpStr.Substring(tmpStr.IndexOf('(') + 1, tmpStr.IndexOf(')') - tmpStr.IndexOf('(') - 1);
                            tmpStr = tmpStr.Substring(tmpStr.IndexOf(')') + 1);
                            tmpClassNo = tmpclass.Substring(0, tmpclass.IndexOf(','));
                            tmpClassName = tmpclass.Substring(tmpclass.IndexOf(',') + 1);
                            tmpPClassRoom = new UMyRecords.stClassRoom();
                            classNo = Int32.Parse(tmpClassNo);

                            tmpPClassRoom.ClassNo = classNo;
                            tmpPClassRoom.ClassName = tmpClassName.PadRight(32, '\0').ToCharArray();
                            DM_Client.MyRecords.ClassRoomList.Add(tmpPClassRoom);
                        }
                    }
                    Response.Write("<script type='text/javascript'>alert('获取响应信息出错');window.location.href='login.aspx';</script>");
                }
                Response.Write("<script type='text/javascript'>alert('获取响应帧类型错误');window.location.href='login.aspx';</script>");
            }
        }
        protected void getRateMap(TDM_Client DM_Client, ref UMyRecords.stFullBlockInfo tmpPBlkInfo)
        {
            UMsgDefine.FM_GETRATEMAP_REQ fmGetRateMapReq;
            UMsgDefine.FM_GETRATEMAP_RSP fmGetRateMapRsp;
            UMyRecords.stBlkRate tmpPBlkRate;
            UMyRecords.stRate tmpPRate;
            string tmpStr, tmpRateRec, tmpRateNo, tmpRateDes;
            int i, code, rateNo;
            Boolean toCreate;
            byte[] tmpBuf;
            tmpPBlkRate = new UMyRecords.stBlkRate();
            tmpPBlkRate.RateList = new List<UMyRecords.stRate>();
            toCreate = true;
            fmGetRateMapReq.MsgHead.MsgType = UConstDefine.TM_GETRATEMAP_REQ;
            fmGetRateMapReq.MsgHead.MsgLength = Marshal.SizeOf(typeof(UMsgDefine.FM_GETRATEMAP_REQ));
            fmGetRateMapReq.UserID = DM_Client.MyRecords.UserInfo.UserID;

            fmGetRateMapReq.VolName = "".PadRight(UConstDefine.VolLen, '\0').ToCharArray();
            fmGetRateMapReq.VolName = tmpPBlkInfo.BlockInfo.VolName;
            lock (DM_Client.CsSocket)
            {
                byte[] Message = UMsgDefine.StructToBytes(fmGetRateMapReq);
                DM_Client.UDPSocket.Send(Message, fmGetRateMapReq.MsgHead.MsgLength, SocketFlags.None);

                byte[] RcvMessage = new byte[Marshal.SizeOf(typeof(UMsgDefine.FM_GETRATEMAP_RSP))];
                DM_Client.UDPSocket.Receive(RcvMessage, RcvMessage.Length, SocketFlags.None);
                fmGetRateMapRsp = (UMsgDefine.FM_GETRATEMAP_RSP)UMsgDefine.BytesToStruct(RcvMessage, typeof(UMsgDefine.FM_GETRATEMAP_RSP));
                if (fmGetRateMapRsp.MsgHead.MsgType == UConstDefine.TM_GETRATEMAP_RSP)
                {
                    if (fmGetRateMapRsp.RspCode == UConstDefine.RM_RSP_OK)
                    {
                        DM_Client.MyRecords.BlkRateList = new List<UMyRecords.stBlkRate>();
                        DM_Client.MyRecords.BlkRateList.Clear();

                        if (DM_Client.MyRecords.BlkRateList.Count > 0)
                        {

                            for (i = 0; i < DM_Client.MyRecords.BlkRateList.Count; i++)
                            {
                                tmpPBlkRate = DM_Client.MyRecords.BlkRateList[i];
                                if (new string(tmpPBlkRate.VolName).Equals(tmpPBlkInfo.BlockInfo.VolName))
                                {
                                    tmpPBlkRate.RateList = new List<UMyRecords.stRate>();
                                    tmpPBlkRate.RateList.Clear();
                                    toCreate = false;
                                    break;
                                }
                                tmpPBlkRate = new UMyRecords.stBlkRate();
                            }
                        }
                        if (toCreate)
                        {
                            tmpPBlkRate = new UMyRecords.stBlkRate();
                            tmpPBlkRate.RateList = new List<UMyRecords.stRate>();
                            tmpPBlkRate.RateList.Clear();
                            tmpPBlkRate.VolName = tmpPBlkInfo.BlockInfo.VolName;
                            DM_Client.MyRecords.BlkRateList.Add(tmpPBlkRate);
                        }
                        tmpStr = new string(fmGetRateMapRsp.RateMap);
                        while (tmpStr.Length > 0)
                        {
                            tmpRateRec = tmpStr.Substring(tmpStr.IndexOf('(') + 1, tmpStr.IndexOf(')') - tmpStr.IndexOf('(') - 1);
                            tmpStr = tmpStr.Substring(tmpStr.IndexOf(')') + 1);
                            tmpRateNo = tmpRateRec.Substring(0, tmpRateRec.IndexOf(','));
                            tmpRateDes = tmpRateRec.Substring(tmpRateRec.IndexOf(',') + 1);
                            rateNo = Int32.Parse(tmpRateNo);
                            tmpPRate = new UMyRecords.stRate();

                            tmpPRate.rate = rateNo;
                            tmpPRate.rateDes = tmpRateDes.PadRight(128, '\0').ToCharArray();
                            tmpPBlkRate.RateList.Add(tmpPRate);
                        }
                    }
                    Response.Write("<script type='text/javascript'>alert('获取档次字典响应帧出错');window.location.href='login.aspx';</script>");
                }
                Response.Write("<script type='text/javascript'>alert('获取响应帧类型错误');window.location.href='login.aspx';</script>");
            }
        }
    }
}

