using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.InteropServices;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace MClient.App_Code
{
    public class TBeaconThreadClass                            //所有的线程所使用的参数在页面SUBMIT之后都会被释放，因此要使用SESSION
    {
        public Thread TBeaconThread;
        public Boolean ToContinue;
        public UMsgDefine.FM_Beacon_Server BeaconRsp;
        public UMsgDefine.FM_Beacon_Server_Assignment BeaconRspAssign;
        public TBeaconThreadClass(TDM_Client DM_Client)
        {
            ToContinue = true;
            DM_Client.MyRecords.ThdStatus.Beacon = true;
            TBeaconThread = new Thread(new ThreadStart(delegate() { Execute(DM_Client); }));
        }
        public void Execute(TDM_Client DM_Client)
        {
            UMsgDefine.FM_Beacon_Client BeaconReq;
            int LogOutFlag, len;
            double v, v1;
            MemoryStream RcvStream = new MemoryStream();
            LogOutFlag = 0;

            byte[] bufbytes = new byte[2000];

            while (ToContinue)
            {
                BeaconReq.MsgHead.MsgType = UConstDefine.TM_BEACON_CLIENT;
                BeaconReq.MsgHead.MsgLength = Marshal.SizeOf(typeof(UMsgDefine.FM_Beacon_Client));
                BeaconReq.nFlag = 0;//not used
                lock (DM_Client.CsSocket)
                {
                    BeaconRsp.MsgHead.MsgType = 0;
                    byte[] Message = UMsgDefine.StructToBytes(BeaconReq);
                    DM_Client.MyRecords.ArrSocket.Send(Message, BeaconReq.MsgHead.MsgLength, SocketFlags.None);
                    if (ToContinue)
                    {
                        byte[] RcvMessage = new byte[Marshal.SizeOf(typeof(UMsgDefine.FM_Beacon_Server))];
                        DM_Client.MyRecords.ArrSocket.Receive(RcvMessage, 20, SocketFlags.None);
                        BeaconRsp = (UMsgDefine.FM_Beacon_Server)UMsgDefine.BytesToStruct(RcvMessage, typeof(UMsgDefine.FM_Beacon_Server));

                        if (BeaconRsp.MsgHead.MsgType == UConstDefine.TM_BEACON_SERVER)
                        {
                            //while (RcvStream.Length < Marshal.SizeOf(typeof(UMsgDefine.FM_Beacon_Server_Assignment)))
                            //{
                            //    len = DM_Client.MyRecords.ArrSocket.Receive(bufbytes, 2000, SocketFlags.None);
                            //    RcvStream.Write(bufbytes, 0, len);
                            //}
                            //LogOutFlag = BeaconRsp.LogOut;
                            //RcvStream.Position = 0;

                            //char[] buf = new char[RcvStream.Length];
                            //RcvMessage = new byte[RcvStream.Length];
                            //RcvStream.Read(RcvMessage, 0, RcvMessage.Length);
                            //buf = System.Text.Encoding.Default.GetChars(RcvMessage);
                            //Array.Copy(buf, 0, BeaconRspAssign.Assignment, 0, buf.Length);

                            v = UnitGlobalV.ts.TotalDays + (double)(BeaconRsp.ServerTime + 8 * 60 * 60) / 86400; //ServerTime
                            v1 = (DateTime.Now - UnitGlobalV.delphiTime).TotalDays;  //LocalTime
                            if (Math.Abs((v - v1) * 86400) > 1)
                            {
                                FileStream ds = File.Open(@"C:\1.txt", FileMode.Append, FileAccess.Write, FileShare.None); ;
                                StreamWriter sw = new StreamWriter(ds);
                                DM_Client.sysTimeDis = Math.Round((v - v1), 6);
                                sw.WriteLine("{0}\t{1}", DM_Client.sysTimeDis, DateTime.Now);
                                sw.Dispose();
                                sw.Close();
                                ds.Dispose();
                                ds.Close();
                            }
                        }
                        else
                        {
                        }
                    }
                    RcvStream.Close();

                }
                BeaconReq.nFlag = -1;
                if (DM_Client.MyRecords.UserInfo.Status != BeaconRsp.Status)
                {
                    if (BeaconRsp.Status == UConstDefine.STAT_DEL)
                    {
                        BeaconReq.nFlag = 1;
                    }
                    if (BeaconRsp.Status == UConstDefine.STAT_ADD)
                    {
                        BeaconReq.nFlag = 2;
                    }
                    if (DM_Client.MyRecords.UserInfo.Role < UConstDefine.DaZuZhang)
                    {
                        while (RcvStream.Length < Marshal.SizeOf(typeof(UMsgDefine.FM_Beacon_Server_Assignment)))
                        {
                            len = DM_Client.MyRecords.ArrSocket.Receive(bufbytes, 2000, SocketFlags.None);
                            RcvStream.Write(bufbytes, 0, len);
                        }
                        LogOutFlag = BeaconRsp.LogOut;
                        RcvStream.Position = 0;

                        char[] buf = new char[RcvStream.Length];
                        byte[] RcvMessage = new byte[RcvStream.Length];
                        RcvStream.Read(RcvMessage, 0, RcvMessage.Length);
                        buf = System.Text.Encoding.Default.GetChars(RcvMessage);
                        Array.Copy(buf, 0, BeaconRspAssign.Assignment, 0, buf.Length);
                    }
                    ChgStat(DM_Client);           //How to send message to browser?
                }
                if (LogOutFlag == 1)
                {
                    HandleOffline(DM_Client);
                }
                Thread.Sleep(15000);
            }

        }
        public void ChgStat(TDM_Client DM_Client)
        {

        }
        public void HandleOffline(TDM_Client DM_Client)
        {

        }
    }
    public class TUDPListenedClass
    {
        public Thread TUDPListenedThread;
        public Boolean ToCloseUDP;
        public TUDPListenedClass(TDM_Client DM_Client)
        {
            ToCloseUDP = false;
            DM_Client.MyRecords.ThdStatus.UDP = true;
            TUDPListenedThread = new Thread(new ThreadStart(delegate() { Execute(DM_Client); }));
        }
        public void Execute(TDM_Client DM_Client)
        {
            int TmpMsgType;
            UMyRecords.stServerInfo PSrvInfo;
            UMsgDefine.FM_UDPMsg_Srv RcvdMsg;
            UMsgDefine.FM_UDPMsg_ACK AckMsg;
            UMsgDefine.FM_CHGSTAT_REQ StatusReq;
            UMyRecords.stStatusInfo PAddStatus;
            int ret, i;
            Boolean ToAdd;
            MemoryStream RcvdStream = new MemoryStream();
            PSrvInfo = DM_Client.ServerInfoList.First();
            while (!ToCloseUDP)
            {
                byte[] RcvMessage = new byte[Marshal.SizeOf(typeof(UMsgDefine.FM_UDPMsg_Srv))];
                try
                {
                    DM_Client.UDPSocket.Blocking = false;
                    if (DM_Client.UDPSocket.Available > 0)
                    {
                        lock (DM_Client.CsSocket)
                        {
                            ret = DM_Client.UDPSocket.Receive(RcvMessage, Marshal.SizeOf(typeof(UMsgDefine.FM_UDPMsg_Srv)), SocketFlags.None);
                            RcvdMsg = (UMsgDefine.FM_UDPMsg_Srv)UMsgDefine.BytesToStruct(RcvMessage, typeof(UMsgDefine.FM_UDPMsg_Srv));
                        }
                        switch (RcvdMsg.MsgHead.MsgType)
                        {
                            case UConstDefine.TM_UDPMSG_SRV:
                                AckMsg.MsgHead.MsgType = UConstDefine.TM_UDPMSG_ACK;
                                AckMsg.MsgHead.MsgLength = Marshal.SizeOf(typeof(UMsgDefine.FM_UDPMsg_ACK));
                                AckMsg.MsgID = RcvdMsg.UcMsgData.MsgID;
                                lock (DM_Client.CsSocket)
                                {
                                    if (RcvdMsg.UcMsgData.MsgType == UConstDefine.TM_CHGSTAT_REQ)
                                    {
                                        byte[] buf = System.Text.Encoding.Default.GetBytes(RcvdMsg.UcMsgData.MsgData);
                                        StatusReq = (UMsgDefine.FM_CHGSTAT_REQ)UMsgDefine.BytesToStruct(buf, typeof(UMsgDefine.FM_CHGSTAT_REQ));
                                        lock (DM_Client.CsStatusInfo)
                                        {
                                            ToAdd = true;
                                            if (DM_Client.StatusList.Count > 0)
                                            {
                                                for (i = 0; i < DM_Client.StatusList.Count; i++)
                                                {
                                                    PAddStatus = DM_Client.StatusList.ElementAt(i);
                                                    if (PAddStatus.UserID == StatusReq.UserID)
                                                    {
                                                        if (PAddStatus.ReqTime < RcvdMsg.UcMsgData.TagTime)
                                                        {
                                                            PAddStatus.OldStatus = StatusReq.OldStatus;
                                                            PAddStatus.NewStatus = StatusReq.NewStatus;
                                                            if (PAddStatus.ValidStatus < PAddStatus.NewStatus)
                                                            {
                                                                PAddStatus.ValidStatus = 0;
                                                            }
                                                            if (PAddStatus.Accept == UConstDefine.PM_CHGSTAT_REFUSE)
                                                            {
                                                                PAddStatus.Accept = 0;
                                                            }
                                                            PAddStatus.ReqTime = (uint)RcvdMsg.UcMsgData.TagTime;
                                                        }
                                                        ToAdd = false;
                                                        break;
                                                    }
                                                }
                                            }
                                            if (ToAdd)
                                            {
                                                PAddStatus = new UMyRecords.stStatusInfo();
                                                PAddStatus.UserID = StatusReq.UserID;
                                                PAddStatus.ReqTime = (uint)RcvdMsg.UcMsgData.TagTime;     //int -> uint?
                                                PAddStatus.OldStatus = StatusReq.OldStatus;
                                                PAddStatus.NewStatus = StatusReq.NewStatus;
                                                PAddStatus.ValidStatus = 0;
                                                PAddStatus.Flag = false;
                                                DM_Client.StatusList.Add(PAddStatus);
                                            }
                                        }
                                        if (DM_Client.MyRecords.UserInfo.Role > UConstDefine.PuTong)
                                        {
                                            RefreshStatus();
                                        }
                                        ////else(DM_Client.StatusList.Count > 0 && 1)
                                        //{
                                        //}
                                    }
                                }
                                break;
                            case UConstDefine.TM_BROADCAST_SRV:
                                break;
                        }
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
                catch (SocketException)
                {
                    Thread.Sleep(1000);
                }
                catch (ObjectDisposedException)
                {
                    Thread.Sleep(1000);
                }

            }
        }
        public void RefreshStatus()
        {

        }
    }
    /// <summary>
    /// 取抽查试卷处理
    /// </summary>
    public class TRcvPaperThreadClass
    {
        public Thread TRcvPaperThread;
        public int RcvResult, WaitTimes;
        public Boolean ToContinue;
        public MemoryStream RcvMS;
        public TRcvPaperThreadClass(TDM_Client DM_Client)
        {
            RcvMS = new MemoryStream();
            ToContinue = true;
            WaitTimes = 0;
            DM_Client.MyRecords.ThdStatus.Rcv = true;
            TRcvPaperThread = new Thread(new ThreadStart(delegate() { Execute(DM_Client); }));
        }
        public void Execute(TDM_Client DM_Client)
        {
            int i, a;
            UMyRecords.stMyPaper PNewPaper;
            string Content;
            Boolean waitflag = true;

            while (true)
            {
                a = 1;
                if (ToContinue)
                {
                    lock (DM_Client.CsNewPaper)
                    {
                        if (DM_Client.NewPaperList.Count < UConstDefine.PaperCount + 1)
                        {
                            while (DM_Client.NewPaperList.Count < UConstDefine.PaperCount)
                            {
                                lock (DM_Client.CsJunk)
                                {
                                    if (DM_Client.JunkList.Count == 0)
                                    {
                                        PNewPaper = new UMyRecords.stMyPaper();
                                        PNewPaper.PaperImage = new MemoryStream();
                                        UMyFuncs.ClearPaperNode(PNewPaper);
                                        DM_Client.JunkList.Add(PNewPaper);
                                    }
                                    PNewPaper = DM_Client.JunkList.First();
                                    UMyFuncs.ClearPaperNode(PNewPaper);
                                    DM_Client.JunkList.RemoveAt(0);

                                    PNewPaper.Flag = false;
                                    DM_Client.NewPaperList.Add(PNewPaper);
                                }
                            }
                            if (DM_Client.NewPaperList.Count > 0)
                            {
                                for (i = 0; i < DM_Client.NewPaperList.Count; i++)
                                {
                                    PNewPaper = DM_Client.NewPaperList[i];
                                    if (!PNewPaper.Flag)
                                    {
                                        switch (DM_Client.MyRecords.UserInfo.Status)
                                        {
                                            case UConstDefine.Trial:
                                                GetNewPaper(DM_Client, ref PNewPaper, UConstDefine.PM_PAPERTYPE_NORMAL, 0, DM_Client.MyVars.CurVolName);
                                                if (PNewPaper.Flag)
                                                {
                                                    PNewPaper.SaveType = -2;
                                                }
                                                DM_Client.NewPaperList[i] = PNewPaper;
                                                break;
                                            case UConstDefine.YangPing:
                                                GetNewPaper(DM_Client, ref PNewPaper, UConstDefine.PM_PAPERTYPE_SAMPLE, 1, DM_Client.MyVars.CurVolName);
                                                if (PNewPaper.Flag)
                                                {
                                                    PNewPaper.SaveType = UConstDefine.PM_SAVESCORE_SAMPLE;
                                                }
                                                DM_Client.NewPaperList[i] = PNewPaper;
                                                break;
                                            case UConstDefine.CePing:
                                                GetNewPaper(DM_Client, ref PNewPaper, UConstDefine.PM_PAPERTYPE_CEPING, 0, DM_Client.MyVars.CurVolName);
                                                if (PNewPaper.Flag)
                                                {
                                                    PNewPaper.SaveType = UConstDefine.PM_SAVESCORE_CEPING;
                                                }
                                                DM_Client.NewPaperList[i] = PNewPaper;
                                                break;
                                            case UConstDefine.ZhengPing:

                                                //TODO:如果是取抽查试卷

                                                //正常取卷
                                                GetNewPaper(DM_Client, ref PNewPaper, UConstDefine.PM_PAPERTYPE_NORMAL, 0, DM_Client.MyVars.CurVolName);
                                                if (PNewPaper.Flag)
                                                {
                                                    switch (PNewPaper.PaperInfo.PaperType)
                                                    {
                                                        case UConstDefine.PM_PAPERTYPE_NORMAL:
                                                        case UConstDefine.PM_PAPERTYPE_CONFLICT:
                                                            PNewPaper.SaveType = UConstDefine.PM_SAVESCORE_NORMAL;
                                                            break;
                                                        case UConstDefine.PM_PAPERTYPE_RESAMPLE:
                                                            PNewPaper.SaveType = UConstDefine.PM_SAVESCORE_RESAMPLE;
                                                            break;
                                                        case UConstDefine.PM_SAVESCORE_SELF:
                                                            PNewPaper.SaveType = UConstDefine.PM_SAVESCORE_SELF;
                                                            break;
                                                    }
                                                }
                                                DM_Client.NewPaperList[i] = PNewPaper;
                                                break;
                                        }
                                        if (!PNewPaper.Flag)
                                        {
                                            if ((RcvResult != UConstDefine.RM_ERR_WAIT))
                                            {
                                          //      if(!CheckFlag)
                                                if (DM_Client.MyRecords.UserInfo.Role < UConstDefine.TiZuZhang)
                                                {
                                                    ToContinue = false;
                                                    WaitTimes = 0;
                               
                                                    if (RcvResult == UConstDefine.RM_ERR_NOACTIVE)
                                                    {
                                                        //实时消息可能要用AJAX轮询
                                                        Content = "当前没有样卷分组处于激活状态";
                                                        UMyFuncs.WriteLogFile(DM_Client, Content);
                                                        break;
                                                    }
                                                    if (RcvResult == UConstDefine.RM_ERR_GETEXAMOVER)
                                                    {
                                                        DM_Client.LastPaper = true;
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    if (RcvResult == UConstDefine.RM_ERR_NOBLK || RcvResult == UConstDefine.RM_ERR_NOPAPER)
                                                    {
                                                        UMyFuncs.GetNextVol(DM_Client);
                                                    }
                                                    else
                                                    {
                                                        ToContinue = false;
                                                        WaitTimes = 0;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //如果是在取抽查试卷（待添加）

                                                //非抽查
                                                if (DM_Client.MyRecords.UserInfo.Role > UConstDefine.PuTong)
                                                {
                                                    UMyFuncs.GetNextVol(DM_Client);
                                                }
                                                else  //没有等待控件
                                                {
                                                    WaitTimes++;
                                                    Content = "取卷 RcvResult = 0x" + RcvResult.ToString("X8") + " " + "WaitTimes = " + WaitTimes.ToString() + " ToContinue = True 取题线程正常工作";

                                                    UMyFuncs.WriteLogFile(DM_Client, Content);
                                                    if (WaitTimes >= 10)
                                                    {
                                                        RcvResult = UConstDefine.RM_ERR_NOPAPER;

                                                        ToContinue = false;
                                                        WaitTimes = 0;
                                                        Content = "WaitTimes = 10 取卷线程暂停工作";

                                                        UMyFuncs.WriteLogFile(DM_Client, Content);
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        else //如果当前结构里面有试题，则无需等待
                                        //如果没有题可等待取，则取消等待消息
                                        {
                                            waitflag = false;//如果所有的结构里面都没有试题，则赋值为True，否则，只要有一个结构有试题，则赋值为false；
                                        }
                                        //如果是在取抽查试卷（待添加）
                                    }
                                    else//如果当前缓冲区有题可阅，则取消等待状态
                                    {
                                        //如果没有题可等待取，则取消等待消息
                                    }
                                }
                            }
                            else
                            {
                                while (DM_Client.NewPaperList.Count < UConstDefine.PaperCount)
                                {
                                    lock (DM_Client.CsJunk)
                                    {
                                        if (DM_Client.JunkList.Count == 0)
                                        {
                                            PNewPaper = new UMyRecords.stMyPaper();
                                            PNewPaper.PaperImage = new MemoryStream();
                                            UMyFuncs.ClearPaperNode(PNewPaper);
                                            DM_Client.JunkList.Add(PNewPaper);
                                        }
                                        PNewPaper = DM_Client.JunkList.First();
                                        UMyFuncs.ClearPaperNode(PNewPaper);
                                        DM_Client.JunkList.RemoveAt(0);

                                        PNewPaper.Flag = false;
                                        DM_Client.NewPaperList.Add(PNewPaper);
                                    }
                                }
                            }
                        }
                    }
                }
                Thread.Sleep(2000);
            }
        }
        public void GetNewPaper(TDM_Client DM_Client, ref UMyRecords.stMyPaper PAddPaper, int PaperType, int DispOrBlkNo, string VolStr)
        {
            int Len;
            string Content;
            UMsgDefine.FM_GetPaper_Req fmGetPaperReq;
            UMsgDefine.FM_GetPaper_Rsp fmGetPaperRsp;
            UMyRecords.stFullBlockInfo PTmpBlkInfo;
            UMyRecords.stScoreForScore TmpScoreScore;
            Boolean ToGetRef;
            byte[] buf = new byte[10000];
            UMyRecords.StExamRefScore tmpExamRefScore, ptmpExamRefScore;
            DateTime tmpTime;

            UMyFuncs.ClearPaperNode(PAddPaper);
            ToGetRef = false;

            fmGetPaperReq.MsgHead.MsgType = UConstDefine.TM_GETPAPER_REQ;
            fmGetPaperReq.MsgHead.MsgLength = Marshal.SizeOf(typeof(UMsgDefine.FM_GetPaper_Req));
            fmGetPaperReq.GetPaperTask.PaperType = PaperType;
            fmGetPaperReq.GetPaperTask.UserID = DM_Client.MyRecords.UserInfo.UserID;
            fmGetPaperReq.GetPaperTask.nAllPaper = 0;
            if (DM_Client.MyRecords.BlockInfoList.Count > 0)
            {
                PTmpBlkInfo = DM_Client.MyRecords.BlockInfoList.First();

                switch (DispOrBlkNo)
                {
                    case 0:
                        fmGetPaperReq.GetPaperTask.DispOrBlkNo = UConstDefine.PM_GETPAPER_NORMAL;
                        ToGetRef = false;
                        if ((DM_Client.MyRecords.UserInfo.Role >= UConstDefine.XiaoZuZhang) && (PTmpBlkInfo.BlockInfo.DispScore == 1))
                        {
                            fmGetPaperReq.GetPaperTask.DispOrBlkNo = UConstDefine.PM_GETPAPER_DISPSCORE;
                            ToGetRef = true;
                        }
                        break;
                    case 1:
                        fmGetPaperReq.GetPaperTask.DispOrBlkNo = UMyFuncs.GetBlkNo(VolStr);
                        break;
                    default:
                        return;
                    //出错
                }
            }
            else //出错
            {
                return;
            }
            fmGetPaperReq.GetPaperTask.PaperNoOrReason = 0;
            fmGetPaperReq.GetPaperTask.VolName = VolStr.PadRight(UConstDefine.VolLen, '\0').ToCharArray();
         
            //Content = "获取试卷准备进入CSSOCKET";
            //UMyFuncs.WriteLogFile(DM_Client, Content);
            lock (DM_Client.CsSocket)
            {
                //Content = "获取试卷进入CSSOCKET成功,准备发送请求帧";
                //UMyFuncs.WriteLogFile(DM_Client, Content);
                byte[] Message = UMsgDefine.StructToBytes(fmGetPaperReq);
                DM_Client.TCPSocket.Send(Message, fmGetPaperReq.MsgHead.MsgLength, SocketFlags.None);
                //Content = "发送获取试卷请求成功";
                //UMyFuncs.WriteLogFile(DM_Client, Content);
                //获取响应 有超时
                //Content = "等待获取试卷响应帧";
                //UMyFuncs.WriteLogFile(DM_Client, Content);
                byte[] RcvMessage = new byte[Marshal.SizeOf(typeof(UMsgDefine.FM_GetPaper_Rsp))];
                DM_Client.TCPSocket.Receive(RcvMessage, Marshal.SizeOf(typeof(UMsgDefine.FM_GetPaper_Rsp)), SocketFlags.None);
                fmGetPaperRsp = (UMsgDefine.FM_GetPaper_Rsp)UMsgDefine.BytesToStruct(RcvMessage, typeof(UMsgDefine.FM_GetPaper_Rsp));

                //Content = "收到获取试卷响应帧";
                //UMyFuncs.WriteLogFile(DM_Client, Content);
                if (fmGetPaperRsp.MsgHead.MsgType == UConstDefine.TM_GETPAPER_RSP)
                {
                    if (fmGetPaperRsp.RspCode == UConstDefine.RM_RSP_OK)
                    {
                        PAddPaper.PaperInfo = fmGetPaperRsp.PaperData;
                        switch (PAddPaper.PaperInfo.PaperType)
                        {   //针对仲裁试题和自重评进行操作
                            case UConstDefine.PM_PAPERTYPE_NORMAL:
                            case UConstDefine.PM_PAPERTYPE_SELF:
                            case UConstDefine.PM_PAPERTYPE_CEPING:
                                //对于测评、自重评，需要接收参考分结构
                                if (PAddPaper.PaperInfo.PaperType != UConstDefine.PM_PAPERTYPE_NORMAL)
                                {
                                    ToGetRef = true;
                                }
                                if (ToGetRef)
                                {
                                    RcvMessage = new byte[Marshal.SizeOf(typeof(UMyRecords.stRefScore))];
                                    DM_Client.TCPSocket.Receive(RcvMessage, Marshal.SizeOf(typeof(UMyRecords.stRefScore)), SocketFlags.None);
                                    PAddPaper.RefScore = (UMyRecords.stRefScore)UMsgDefine.BytesToStruct(RcvMessage, typeof(UMyRecords.stRefScore));
                                }
                                break;
                            case UConstDefine.PM_PAPERTYPE_RESAMPLE: //对于样卷重评试题，继续接收TmpScoreScore结构
                                RcvMessage = new byte[Marshal.SizeOf(typeof(UMyRecords.stScoreForScore))];
                                DM_Client.TCPSocket.Receive(RcvMessage, Marshal.SizeOf(typeof(UMyRecords.stScoreForScore)), SocketFlags.None);

                                TmpScoreScore = (UMyRecords.stScoreForScore)UMsgDefine.BytesToStruct(RcvMessage, typeof(UMyRecords.stScoreForScore));
                                PAddPaper.ScoreInfo.BlkNo = TmpScoreScore.BlkNo;
                                PAddPaper.ScoreInfo.UserIDOrPaperSeq = TmpScoreScore.PaperSeq;
                                PAddPaper.ScoreInfo.ExcpReason = -100; //对于样卷试题，该位无效
                                PAddPaper.ScoreInfo.TotalScore = TmpScoreScore.TotalScore;
                                PAddPaper.ScoreInfo.Txt = TmpScoreScore.DetailScore;

                                break;
                            case UConstDefine.PM_PAPERTYPE_SAMPLE: //对于样卷试题，继续接收tmpExamRefScore结构
                                RcvMessage = new byte[Marshal.SizeOf(typeof(UMyRecords.StExamRefScore))];
                                DM_Client.TCPSocket.Receive(RcvMessage, Marshal.SizeOf(typeof(UMyRecords.StExamRefScore)), SocketFlags.None);
                                //读取题组长或大组长给分详细信息,并保存到SampleInfoList；
                                tmpExamRefScore = (UMyRecords.StExamRefScore)UMsgDefine.BytesToStruct(RcvMessage, typeof(UMyRecords.StExamRefScore));

                                PAddPaper.ScoreInfo.TotalScore = tmpExamRefScore.TotalScore;
                                ptmpExamRefScore = new UMyRecords.StExamRefScore();
                                ptmpExamRefScore.UserID = tmpExamRefScore.UserID;
                                ptmpExamRefScore.PaperNo = tmpExamRefScore.PaperNo;
                                ptmpExamRefScore.TotalScore = tmpExamRefScore.TotalScore;
                                ptmpExamRefScore.DetailScore = tmpExamRefScore.DetailScore;
                                ptmpExamRefScore.Reason = tmpExamRefScore.Reason;
                                ptmpExamRefScore.Rate = tmpExamRefScore.Rate;
                                ptmpExamRefScore.GroupName = tmpExamRefScore.GroupName;
                                ptmpExamRefScore.RecCount = tmpExamRefScore.RecCount;

                                DM_Client.MyRecords.SampleInfoList.Add(ptmpExamRefScore);
                                break;
                        }
                        //Content = "获取试卷时获取试卷信息成功，等待接收试卷图片...";
                        //UMyFuncs.WriteLogFile(DM_Client, Content);
                        //接收试题图片  第一种为从文件服务器读取
                        if (PAddPaper.PaperInfo.ImageLen == 0)
                        {
                            RcvResult = UConstDefine.RM_ERR_PAPERNOTEXIST;

                            UMyFuncs.GetPicFromServer(DM_Client, PAddPaper.PaperImage, PAddPaper.PaperInfo.PicPath);

                            PAddPaper.Flag = false;
                            PAddPaper.Status = false;

                            if (PAddPaper.PaperImage.Length > 0)
                            {
                                PAddPaper.Flag = true;
                                RcvResult = UConstDefine.RM_RSP_OK;
                                Content = "成功读取共享试卷图片" + new string(PAddPaper.PaperInfo.PicPath);
                                UMyFuncs.WriteLogFile(DM_Client, Content);
                            }
                            else
                            {
                                RcvResult = UConstDefine.RM_ERR_ERR;
                                Content = "读取共享试卷图片失败" + PAddPaper.PaperInfo.PaperNo.ToString() + new string(PAddPaper.PaperInfo.PicPath);
                                UMyFuncs.WriteLogFile(DM_Client, Content);
                            }
                        }
                        else
                        {
                            RcvMS.Flush();
                            while (RcvMS.Length < PAddPaper.PaperInfo.ImageLen)
                            {
                                //对网络中断的控制
                                Len = DM_Client.TCPSocket.Receive(buf, 8192, SocketFlags.None);

                                if (Len <= 0)
                                {
                                    if (RcvMS.Length < PAddPaper.PaperInfo.ImageLen)
                                    {
                                        RcvResult = UConstDefine.RM_ERR_ERR;
                                        //错误提示
                                        break;
                                    }
                                }
                                RcvMS.Write(buf, 0, Len);
                            }
                            if (RcvMS.Length == PAddPaper.PaperInfo.ImageLen)
                            {
                                RcvMS.Position = 0;

                                PAddPaper.PaperImage.Flush();
                                PAddPaper.PaperImage = RcvMS; //?

                                PAddPaper.Flag = true;
                                PAddPaper.Status = false;

                                RcvMS.Flush();
                                RcvResult = UConstDefine.RM_RSP_OK;
                                Content = "成功读取试卷图片";
                                UMyFuncs.WriteLogFile(DM_Client, Content);
                            }
                            RcvMS.Flush();
                        }
                    }
                    else
                    {
                        RcvResult = fmGetPaperRsp.RspCode;
                        Content = "获取试卷响应帧返回不为RM_RSP_OK";
                        UMyFuncs.WriteLogFile(DM_Client, Content);
                    }
                }
                else
                {
                    RcvResult = UConstDefine.RM_ERR_ERR;
                    Content = "获取试卷时收到错误响应帧";
                    UMyFuncs.WriteLogFile(DM_Client, Content);
                }
            }
        }
    }
    public class TSaveScoreThreadClass
    {
        public Thread TSaveScoreThread;
        public uint LastSaveTime;
        public int TimeThreshold;
        public Boolean ToContinue;
        public TSaveScoreThreadClass(TDM_Client DM_Client)
        {
            LastSaveTime = 0;
            ToContinue = true;
            DM_Client.MyRecords.ThdStatus.Save = true;
            TSaveScoreThread = new Thread(new ThreadStart(delegate() { Execute(DM_Client); }));
        }
        public void Execute(TDM_Client DM_Client)
        {
            uint CurTime;
            int i, n, nTmpGap, j;
            double v;
            UMyRecords.stMyPaper PTmpPaper;

            while (true)
            {
                if (ToContinue)
                {
                    v = ((DateTime.Now - UnitGlobalV.delphiTime).TotalDays + DM_Client.sysTimeDis) * 86400;
                    CurTime = (uint)Math.Round(v);
                    if (LastSaveTime == 0)
                        LastSaveTime = CurTime;
                    //超过服务器设定的值，则将所有OldPaperList中已有分数移到SaveScoreList中
                    else
                    {
                        nTmpGap = (int)(CurTime - LastSaveTime);
                        //如果服务器端没有设置存分时间则自己设置为5分钟
                        if (DM_Client.MyRecords.CurBlockInfo.BlockInfo.TimeOut < 5)
                        {
                            DM_Client.MyRecords.CurBlockInfo.BlockInfo.TimeOut = 5;
                        }
                        if (Math.Abs(nTmpGap) > DM_Client.MyRecords.CurBlockInfo.BlockInfo.TimeOut * 60)
                        {
                            n = DM_Client.OldPaperList.Count;
                            lock (DM_Client.CsOldPaper)
                            {
                                if (DM_Client.OldPaperList.Count > 0)
                                {
                                    lock (DM_Client.CsSavePaper)
                                    {
                                        for (i = DM_Client.OldPaperList.Count - 1; i >= 0; i--)
                                        {

                                            PTmpPaper = DM_Client.OldPaperList[i];
                                            //对已判分数进行操作
                                            if (PTmpPaper.Status)
                                            {
                                                //从OldPaperList中取出
                                                DM_Client.OldPaperList.RemoveAt(i);

                                                //设置存分类型!!!!!!!!!!
                                                //存分类型已于显示试卷图片时进行了设置
                                                DM_Client.SaveScoreList.Add(PTmpPaper);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    LastSaveTime = CurTime;
                                }
                            }
                            //如果有存分操作，则需要重新设置存分类型，并做相应操作
                            if (DM_Client.OldPaperList.Count != n)
                            {
                                //RefreshShow(1); hatethis -> JS refresh the check list
                            }
                        }
                    }

                    while (DM_Client.SaveScoreList.Count > 0)
                    {
                        if (DM_Client.SaveScoreList.Count >= UConstDefine.SavePaperCount)//一次能凑足SavePaperCount份试卷存分
                        {
                            SavePaper(DM_Client, UConstDefine.SavePaperCount);
                        }
                        else
                        {
                            SavePaper(DM_Client, DM_Client.SaveScoreList.Count);
                        }
                    }
                }
                Thread.Sleep(3000);
            }
        }
        public void SavePaper(TDM_Client DM_Client, int SaveCount)
        {
            UMsgDefine.FM_SaveScore_Req fmSaveReq;
            UMsgDefine.FM_SaveScore_Rsp fmSaveRsp;
            MemoryStream SocketStream;
            UMyRecords.stMyPaper PTmpPaper;
            double v, v2;
            int i, j, k, orderPaperNum;
            UMyRecords.StExamRefScore ptmpExamRefScore;
            string Content;
            int CheckResult;
            fmSaveReq.Score = new UMyRecords.stSaveScore[5];
            v2 = 1;
            lock (DM_Client.CsSavePaper)
            {
                if (DM_Client.MyRecords.UserInfo.Status > UConstDefine.Trial)
                {

                    fmSaveReq.MsgHead.MsgType = UConstDefine.TM_SAVESCORE_REQ;
                    fmSaveReq.MsgHead.MsgLength = Marshal.SizeOf(typeof(UMsgDefine.FM_SaveScore_Req));
                    fmSaveReq.RecCount = SaveCount;

                    for (i = 0; i < SaveCount; i++)
                    {
                        PTmpPaper = DM_Client.SaveScoreList[i];
                        if (PTmpPaper.SaveType == UConstDefine.PM_SAVESCORE_SAMPLE)
                        {
                            for (j = 0; j < DM_Client.MyRecords.SampleInfoList.Count; j++)
                            {
                                ptmpExamRefScore = DM_Client.MyRecords.SampleInfoList[j];
                                if (ptmpExamRefScore.PaperNo == PTmpPaper.PaperInfo.PaperNo)
                                {
                                    DM_Client.MyRecords.SampleInfoList.RemoveAt(j);
                                    break;
                                }
                            }
                            orderPaperNum = 0;

                            //略
                        }
                        if ((PTmpPaper.Status) && (PTmpPaper.SaveType > 0))
                        {
                            fmSaveReq.Score[i].UserID = DM_Client.MyRecords.UserInfo.UserID;
                            fmSaveReq.Score[i].PaperNo = PTmpPaper.PaperInfo.PaperNo;
                            fmSaveReq.Score[i].VolumeName = PTmpPaper.PaperInfo.VolumeName;
                            fmSaveReq.Score[i].ScoreType = PTmpPaper.SaveType;
                            fmSaveReq.Score[i].TotalScore = PTmpPaper.TotalScore;
                            ////检查详细分数合法性
                            //CheckResult = UMyFuncs.CheckDetail(DM_Client, DM_Client.MyRecords.CurBlockInfo.BlockInfo.BlkSeqNo, PTmpPaper.DetailScore);
                            //if ((PTmpPaper.PaperInfo.PaperNo > 0) && (CheckResult > 0))
                            //{
                            //    Content = "存分时发现分数错误,ErrCode:" + CheckResult.ToString() + "试卷号:" + PTmpPaper.PaperInfo.PaperNo.ToString() + "分数:" + PTmpPaper.DetailScore;

                            //    UMyFuncs.WriteLogFile(DM_Client, Content);
                            //}
                            fmSaveReq.Score[i].DetailScore = PTmpPaper.DetailScore.PadRight(64, '\0').ToCharArray();
                            fmSaveReq.Score[i].TimeStamp = (uint)Math.Round(PTmpPaper.TimeStamp);
                            //有可能想减的结果大于0却小于0.5 取整后为0 2009.6.8 故不使用Round取整
                            fmSaveReq.Score[i].TimeCost = (int)Math.Ceiling(PTmpPaper.TimeStamp - PTmpPaper.StartTime);//计算是以秒为单位
                            if (fmSaveReq.Score[i].TimeCost == 0)
                            {
                                fmSaveReq.Score[i].TimeCost = 1; //为防止非常意外的情况发生，强制为1； //修改for timecost
                            }
                            //Content = "阅卷人:" + fmSaveReq.Score[i].UserID.ToString() +
                            //"| 试卷号:" + fmSaveReq.Score[i].PaperNo.ToString() +
                            //"| TimeStamp:" + fmSaveReq.Score[i].TimeStamp.ToString() +
                            //"| StartTime:" + PTmpPaper.StartTime.ToString() +
                            //"| TimeCost:" + fmSaveReq.Score[i].TimeCost.ToString();
                            //UMyFuncs.WriteLogFile(DM_Client, Content);
                        }
                    }
                    for (i = 0; i < SaveCount; i++)
                    {
                        PTmpPaper = DM_Client.SaveScoreList.First();
                        DM_Client.SaveScoreList.RemoveAt(0);
                        lock (DM_Client.CsJunk)
                        {
                            DM_Client.JunkList.Add(PTmpPaper);
                        }
                    }

                    v = ((DateTime.Now - UnitGlobalV.delphiTime).TotalDays + DM_Client.sysTimeDis) * 86400;
                    LastSaveTime = (uint)Math.Round(v);

                    lock (DM_Client.CsSocket)
                    {
                        //Content = "发送存分请求帧";
                        //UMyFuncs.WriteLogFile(DM_Client, Content);
                        byte[] Message = UMsgDefine.StructToBytes(fmSaveReq);
                        DM_Client.TCPSocket.Send(Message, fmSaveReq.MsgHead.MsgLength, SocketFlags.None);
                        //Content = "发送存分请求成功";
                        //UMyFuncs.WriteLogFile(DM_Client, Content);
                        //等待响应超时
                        //Content = "等待存分响应帧";
                        //UMyFuncs.WriteLogFile(DM_Client, Content);

                        //Content = "成功收到存分响应帧";
                        //UMyFuncs.WriteLogFile(DM_Client, Content);
                        byte[] RcvMessage = new byte[Marshal.SizeOf(typeof(UMsgDefine.FM_SaveScore_Rsp))];
                        DM_Client.TCPSocket.Receive(RcvMessage, Marshal.SizeOf(typeof(UMsgDefine.FM_SaveScore_Rsp)), SocketFlags.None);
                        fmSaveRsp = (UMsgDefine.FM_SaveScore_Rsp)UMsgDefine.BytesToStruct(RcvMessage, typeof(UMsgDefine.FM_SaveScore_Rsp));

                    }
                }
                else
                {
                    for (i = 0; i < SaveCount; i++)
                    {
                        PTmpPaper = DM_Client.SaveScoreList.First();
                        DM_Client.SaveScoreList.RemoveAt(0);

                        lock (DM_Client.CsJunk)
                        {
                            DM_Client.JunkList.Add(PTmpPaper);
                        }
                    }
                    v = ((DateTime.Now - UnitGlobalV.delphiTime).TotalDays + DM_Client.sysTimeDis) * 86400;
                    LastSaveTime = (uint)Math.Round(v);
                }
            }
        }

        public void DealError()
        {
        }
    }
    public class TPreFetchThreadClass
    {
        public Thread TPreFetchThread;
        public int FetchType;
        public int index;
        public Boolean ToContinue;
        public string picTemp;
        public TPreFetchThreadClass(TDM_Client DM_Client)
        {
            ToContinue = true;
            TPreFetchThread = new Thread(new ThreadStart(delegate() { Execute(DM_Client); }));
        }
        public void Execute(TDM_Client DM_Client)
        {
            String Mark;
            UMyRecords.stMyPaper PNextPaper = new UMyRecords.stMyPaper();
            Boolean NextPaper = false;
            while (ToContinue)
            {
                while (FetchType == -1)
                {
                    Thread.Sleep(200);
                }
                lock (DM_Client.CsNextPaperNode)
                {
                    //TODO:
                    if (DM_Client.PNextPaper.PaperInfo.PaperNo != 0)// 判断：下一份试卷已经取到
                    {
                        if (FetchType == 1)
                        {
                            Mark = DM_Client.PNextPaper.PaperInfo.PaperNo.ToString();//1为普通试卷（包括小组长处理的试卷），采用试卷号来唯一标识试卷
                        }
                        else //2为大组长异处理常试卷，采用题块号唯一标识试卷
                        {
                            Mark = UMyFuncs.GetBlkNo(DM_Client.PNextPaper.PaperInfo.VolumeName.ToString()).ToString();
                        }
                        lock (DM_Client.CsNextPaper)
                        {
                            DM_Client.PNextPaper.PaperImage.Position = 0;
                            UMyFuncs.LoadJP2(DM_Client, DM_Client.PNextPaper, picTemp, NextPaper);
                        }
                    }

                }
       //         UMyFuncs.FetchNextPaper(DM_Client, index, FetchType, picTemp);
                FetchType = -1;
            }
        }
    }
}