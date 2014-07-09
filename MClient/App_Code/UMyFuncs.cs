using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.Sockets;
using System.IO;
using FreeImageAPI;
using Microsoft.VisualBasic;
namespace MClient.App_Code
{
    public class UMyFuncs
    {
        public string sql;
        public UMsgDefine.FM_GetQtyFile_Req fmGetQtyReq; // 获取质量文件请求帧
        public UMsgDefine.FM_GetQtyFile_Rsp fmGetQtyRsp; // 响应帧
        public MemoryStream SocketStream;
        public MemoryStream RcvdStream;
        public byte[] Buf;
        public int Len;
        public DateTime tmpTime, tmpTime2;
        public Boolean FirstFlag; // 是否为第一次获取质量控制文件
        public static void ClearPaperNode(UMyRecords.stMyPaper PPaper)
        {
            PPaper.PaperInfo.PaperNo = 0;
            PPaper.PaperInfo.ImageFormat = -1;
            PPaper.PaperInfo.VolumeName = "".PadRight(UConstDefine.VolLen, '\0').ToCharArray();
            PPaper.PaperInfo.PaperType = -1;
            PPaper.PaperInfo.ImageLen = -1;

            PPaper.PaperImage.Flush();

            PPaper.SaveType = -1;

            PPaper.ScoreInfo.BlkNo = -1;
            PPaper.ScoreInfo.UserIDOrPaperSeq = -1;
            PPaper.ScoreInfo.ExcpReason = -1;
            PPaper.ScoreInfo.TotalScore = -1;
            PPaper.ScoreInfo.Txt = "".PadRight(UConstDefine.DetailLen, '\0').ToCharArray();

            PPaper.RefScore.UserID1 = -1;
            PPaper.RefScore.UserID2 = -1;
            PPaper.RefScore.UserID3 = -1;
            PPaper.RefScore.UserID4 = -1;

            PPaper.TotalScore = -1;
            PPaper.DetailScore = "";
            PPaper.StartTime = 0;
            PPaper.TimeStamp = 0;
            PPaper.RStartTime = 0;
            PPaper.REndTime = 0;
            PPaper.Status = false;
            PPaper.Flag = false;
        }
        public static void WriteLogFile(TDM_Client DM_Client, string Content)
        {
            Content = DateTime.Now.ToString("yyyy-MM-dd HH：mm：ss") + " " + Content;
            lock (DM_Client.CsLogFile)
            {
                string tmpstr1 = new String(DM_Client.MyRecords.UserInfo.LoginName);
                tmpstr1 = tmpstr1.Substring(0, tmpstr1.IndexOf('\0'));
                FileStream ds = File.Open(@"C:/Debug_" + tmpstr1 + @".txt", FileMode.Append, FileAccess.Write, FileShare.None);
                StreamWriter sw = new StreamWriter(ds);
                sw.WriteLine(Content);
                sw.Dispose();
                sw.Close();
                ds.Dispose();
                ds.Close();
            }
        }
        public static void GetNextVol(TDM_Client DM_Client)
        {
            int index;
            UMyRecords.stFullBlockInfo PTmpBlk;

            DM_Client.MyVars.CurVolName = "";
            if (DM_Client.MyRecords.AssignList.Count > 0)
            {
                if (DM_Client.MyRecords.AssignList.Count == 1)
                {
                    DM_Client.MyVars.CurVolName = DM_Client.MyRecords.AssignList.ElementAt(0);
                }
                else
                {
                    index = DM_Client.MyRecords.AssignList.IndexOf(new string(DM_Client.MyRecords.CurBlockInfo.BlockInfo.VolName));
                    if (index < 0)
                    {
                        index = 0;
                    }
                    else
                    {
                        index = (index + 1) % DM_Client.MyRecords.AssignList.Count;
                    }
                    if ((index >= 0) && (index < DM_Client.MyRecords.AssignList.Count))
                    {
                        DM_Client.MyVars.CurVolName = DM_Client.MyRecords.AssignList.ElementAt(index);
                    }
                }

                if (!(DM_Client.MyVars.CurVolName.Equals("")) && (!new String(DM_Client.MyRecords.CurBlockInfo.BlockInfo.VolName).TrimEnd('\0').Equals(DM_Client.MyVars.CurVolName)))
                {
                    for (index = 0; index < DM_Client.MyRecords.BlockInfoList.Count; index++)
                    {
                        PTmpBlk = DM_Client.MyRecords.BlockInfoList.ElementAt(index);
                        if (new string(PTmpBlk.BlockInfo.VolName).TrimEnd('\0').Equals(DM_Client.MyVars.CurVolName))
                        {
                            DM_Client.MyRecords.CurBlockInfo.BlockInfo.VolName = PTmpBlk.BlockInfo.VolName;
                            DM_Client.MyRecords.CurBlockInfo.BlockInfo.QuestionNo = PTmpBlk.BlockInfo.QuestionNo;
                            DM_Client.MyRecords.CurBlockInfo.BlockInfo.BlkSeqNo = PTmpBlk.BlockInfo.BlkSeqNo;
                            DM_Client.MyRecords.CurBlockInfo.BlockInfo.nNEv = PTmpBlk.BlockInfo.nNEv;
                            DM_Client.MyRecords.CurBlockInfo.BlockInfo.CheckRule = PTmpBlk.BlockInfo.CheckRule;
                            DM_Client.MyRecords.CurBlockInfo.BlockInfo.CheckScore = PTmpBlk.BlockInfo.CheckScore;
                            DM_Client.MyRecords.CurBlockInfo.BlockInfo.DispScore = PTmpBlk.BlockInfo.DispScore;
                            DM_Client.MyRecords.CurBlockInfo.BlockInfo.BufSize = PTmpBlk.BlockInfo.BufSize;
                            DM_Client.MyRecords.CurBlockInfo.BlockInfo.TimeOut = PTmpBlk.BlockInfo.TimeOut;
                            DM_Client.MyRecords.CurBlockInfo.BlockInfo.QuestionType = PTmpBlk.BlockInfo.QuestionType;
                            DM_Client.MyRecords.CurBlockInfo.BlockInfo.TotalTestMin = PTmpBlk.BlockInfo.TotalTestMin;
                            DM_Client.MyRecords.CurBlockInfo.BlockInfo.SampleTestMin = PTmpBlk.BlockInfo.SampleTestMin;
                            DM_Client.MyRecords.CurBlockInfo.BlockInfo.MaxSize = PTmpBlk.BlockInfo.MaxSize;
                            DM_Client.MyRecords.CurBlockInfo.BlockInfo.MinSize = PTmpBlk.BlockInfo.MinSize;
                            DM_Client.MyRecords.CurBlockInfo.BlockInfo.AvgSize = PTmpBlk.BlockInfo.AvgSize;
                            DM_Client.MyRecords.CurBlockInfo.BlockInfo.MinTime = PTmpBlk.BlockInfo.MinTime;
                            DM_Client.MyRecords.CurBlockInfo.BlockInfo.SampleAmount = PTmpBlk.BlockInfo.SampleAmount;
                            DM_Client.MyRecords.CurBlockInfo.BlockInfo.ServerID = PTmpBlk.BlockInfo.ServerID;
                            DM_Client.MyRecords.CurBlockInfo.BlockInfo.TipsLen = PTmpBlk.BlockInfo.TipsLen;
                            DM_Client.MyRecords.CurBlockInfo.BlockInfo.JunkInfo = PTmpBlk.BlockInfo.JunkInfo;
                            break;
                        }
                    }
                }
            }
        }
        public static int GetBlkNo(string VolStr)
        {
            int i;
            for (i = 0; i < 2; i++)
            {
                VolStr = VolStr.Substring(VolStr.IndexOf('_') + 1);
            }
            VolStr = VolStr.Substring(0, VolStr.IndexOf('_'));


            i = (int)Microsoft.VisualBasic.Conversion.Val(VolStr);
            if (IsNum(VolStr))
                return i;
            else
                return -1;
        }
        public static bool IsNum(String str)
        {
            if (str.ElementAt(0) == '-')
            {
                for (int i = 1; i < str.Length; i++)
                {
                    if (!Char.IsNumber(str, i))
                        return false;
                }
            }
            else
            {
                for (int i = 0; i < str.Length; i++)
                {
                    if (!Char.IsNumber(str, i))
                        return false;
                }
            }         
            return true;
        }
        public static void GetPicFromServer(TDM_Client DM_Client, MemoryStream stream, char[] URI)
        {
            //HTTP

            //SHARE
            
            WebClient webClient = new WebClient();
            byte[] buf = webClient.DownloadData(new string(URI).TrimEnd('\0'));
            stream.Write(buf, 0, buf.Length);
            //TFTP
        }
        public static int CheckDetail(TDM_Client DM_Client, int MidNo, string Detail)
        {
            UMyRecords.stFullBlockInfo PTmpBlk;
            int i, j, k, Count, Code, Pos1, Pos2;
            string CheckRule, Points, score1, score2;
            List<Single> ArrFull;
            string[] strPt, strPt1, strPt2;
            Boolean AllowMinus;
            Single sumScore, Score;
            CheckRule = "";
            if (Detail.Equals(""))
            {
                return 1;
            }
            for (i = 0; i < DM_Client.MyRecords.BlockInfoList.Count; i++)
            {
                PTmpBlk = DM_Client.MyRecords.BlockInfoList[i];
                if (PTmpBlk.BlockInfo.BlkSeqNo == MidNo)
                {
                    CheckRule = new String(PTmpBlk.BlockInfo.CheckRule);
                    break;
                }
            }

            i = 0;


            Count = Int32.Parse(CheckRule.Substring(1, CheckRule.IndexOf('}') - 1));
            ArrFull = new List<float>(Count);
            CheckRule = CheckRule.Substring(CheckRule.IndexOf('}') + 2);
            while (CheckRule.IndexOf('(') >= 0)
            {
                Points = CheckRule.Substring(1, CheckRule.IndexOf(')') - 1);
                strPt = Points.Split(',');
                ArrFull[i] = float.Parse(strPt[1]);
                i++;
                CheckRule = CheckRule.Substring(CheckRule.IndexOf(')') + 2);
            }

            AllowMinus = false;
            if (CheckRule[1] == 'Y')
            {
                AllowMinus = true;
            }

            score1 = Detail;
            score2 = "";
            if (AllowMinus)
            {
                Pos1 = Detail.IndexOf('{');
                Pos2 = Detail.IndexOf('}');
                if (Pos1 < 0 || Pos2 < 0)
                {
                    return 1;
                }
                score1 = Detail.Substring(0, Detail.IndexOf('{') - 2);
                score2 = Detail.Substring(Pos1 + 1, Pos2 - Pos1 - 1);

            }
            strPt1 = score1.Split(',');
            j = 0;
            sumScore = 0;
            for (i = 0; i < strPt1.Length; i++)
            {
                Score = (float)Conversion.Val(strPt1[i]);
                if (!IsNum(strPt1[i]))
                    return 1;
                if ((Score < 0) || (Score > ArrFull[i]))
                    return 2;
                j++;
                sumScore = sumScore + Score;
            }

            strPt2 = score2.Split('|');
            k = j;
            for (i = 0; i < strPt2.Length; i++)
            {
                Score = (float)Microsoft.VisualBasic.Conversion.Val(strPt2[i]);
                if (!IsNum(strPt2[i]))
                    return 1;
                if (Score > 0 || Score < ArrFull[j + k])
                    return 2;
                j++;
                sumScore = sumScore + Score;
            }
            if (j != Count)
                return 1;
            if (sumScore < 0)
                return 3;
            return 0;
        }
        public static void ClearUpInfo(TDM_Client DM_Client, int InfoType)
        {
            int i;
            UMyRecords.stFullBlockInfo PTmpBlk;
            switch (InfoType)
            {
                case 1:
                    DM_Client.MyRecords.UserInfo.UserID = 0;
                    DM_Client.MyRecords.UserInfo.LoginName = "".PadRight(UConstDefine.NameLen, '\0').ToCharArray();
                    DM_Client.MyRecords.UserInfo.Role = 0;
                    DM_Client.MyRecords.UserInfo.Status = 0;
                    DM_Client.MyRecords.UserInfo.Interceder = 0;
                    DM_Client.MyRecords.UserInfo.TopLevelTeam = 0;
                    DM_Client.MyRecords.UserInfo.MidLevelTeam = 0;
                    DM_Client.MyRecords.UserInfo.BtmLevelTeam = 0;
                    DM_Client.MyRecords.UserInfo.XiaoZuZhangID = 0;
                    DM_Client.MyRecords.UserInfo.TiZuZhangID = 0;
                    DM_Client.MyRecords.UserInfo.DaZuZhangID = 0;
                    DM_Client.MyRecords.UserInfo.SmpPaperTackled = 0;
                    DM_Client.MyRecords.UserInfo.TstPaperTackled = 0;
                    DM_Client.MyRecords.UserInfo.PaperTackled = 0;
                    DM_Client.MyVars.CurPaperNum = -1;
                    DM_Client.MyRecords.UserInfo.TrueName = "".PadRight(UConstDefine.NameLen, '\0').ToCharArray();
                    DM_Client.MyRecords.UserInfo.ServeFor = "".PadRight(UConstDefine.ServeForLen, '\0').ToCharArray();
                    DM_Client.MyRecords.UserInfo.AssignLen = 0;
                    //     DM_Client.MyRecords.UserInfo.Assignment = "".PadRight(2000, '\0').ToCharArray();
                    DM_Client.MyRecords.AssignList.Clear();
                    break;
                case 2:
                    DM_Client.MyVars.BlockOK = false;
                    if (DM_Client.MyRecords.BlockInfoList.Count > 0)
                    {
                        for (i = 0; i < DM_Client.MyRecords.BlockInfoList.Count; i++)
                        {
                            PTmpBlk = DM_Client.MyRecords.BlockInfoList[i];
                            PTmpBlk.TipsList.Clear();
                            DM_Client.MyRecords.BlockInfoList[i] = PTmpBlk;
                        }
                        DM_Client.MyRecords.BlockInfoList.Clear();
                    }
                    break;

            }
        }
        /// <summary>
        /// 获取本大组所有题块的题块号
        /// </summary>
        public static void GetAllBlkNo(TDM_Client DM_Client)
        {
            int i, j, k;
            string tz;
            Boolean same;
            int[] MidNoArray = new int[DM_Client.MyRecords.AllBlkList.Count];
            if (DM_Client.MyRecords.AllBlkList.Count > 0)
            {
   
                k = 0;
                for (i = 0; i < MidNoArray.Length; i++)
                {
                    MidNoArray[i] = -1;
                }
                for (i = 0; i < MidNoArray.Length; i++)
                {
                    tz = DM_Client.MyRecords.AllBlkList[i];
                    same = false;
                    //避免以下情况出现：题块5有2个分卷5-5-1和5-5-2，则下拉框中出现两个5
                    char[] Delimiter = new char[] { '_' };
                    tz = tz.Split(Delimiter)[1];
                    for (j = 0; j < MidNoArray.Length; j++)
                    {
                        if (MidNoArray[j] == Int32.Parse(tz))
                        {
                            same = true;
                            break;
                        }
                    }
                    if (!same)
                    {
                        MidNoArray[k] = Int32.Parse(tz);
                        k++;
                    }
                }
            }
            //采用冒泡排序法对题组号进行顺序排序
            for (i = 1; i <= DM_Client.MyRecords.AllBlkList.Count; i++)
            {
                for (j = 0; j < DM_Client.MyRecords.AllBlkList.Count - i; j++)
                {
                    if (MidNoArray[j] > MidNoArray[j + 1])
                    {
                        k = MidNoArray[j];
                        MidNoArray[j] = MidNoArray[j + 1];
                        MidNoArray[j + 1] = k;
                    }
                }
            }
            //先清空题块号列表
            DM_Client.MyRecords.AllBlkNo = new List<string>();
            DM_Client.MyRecords.AllBlkNo.Clear();
            //再添加题块号
            for (i = 0; i < DM_Client.MyRecords.AllBlkList.Count; i++)
            {
                if (MidNoArray[i] > 0)
                {
                    DM_Client.MyRecords.AllBlkNo.Add(MidNoArray[i].ToString());
                }
            }
        }
        public static void DecodeInfo_chen(TDM_Client DM_Client, string Info, int OperationType)
        {
            String s, tmpStr, tmpDetail, Points;
            UMyRecords.stMyPaper PMyPaper;
            int i, code;
            Single TmpScore;
            List<string> ScoreDetail;
            switch (OperationType)
            {
                case 1: // 从用户信息中解析改用户的责任试题，保存在DM_Cilent.MyRecords.AssignList中
                    DM_Client.MyRecords.AssignList.Clear();
                    s = Info;
                    while (s.Length > 0)
                    {
                        if (s.IndexOf(',') > 0)
                        {
                            tmpStr = s.Substring(0, s.IndexOf(','));
                        }
                        else
                        {
                            tmpStr = s;
                            DM_Client.MyRecords.AssignList.Add(tmpStr);
                            break;
                        }
                        DM_Client.MyRecords.AssignList.Add(tmpStr);
                        s = s.Substring(s.IndexOf(',') + 1);
                    }
                    break;
            }
        }
        public static void GetGrpUserInfo(TDM_Client DM_Client)
        {
            UMsgDefine.FM_GetGrpInfo_Req fmGetUserReq;
            UMsgDefine.FM_GetGrpInfo_Rsp fmGetUserRsp;
            UMyRecords.stGrpUserInfo TmpUser;
            UMyRecords.stMyUser PTmpUser;
            string Content;
            string sql;
            UMsgDefine.FM_GetQtyFile_Req fmGetQtyReq; // 获取质量文件请求帧
            UMsgDefine.FM_GetQtyFile_Rsp fmGetQtyRsp; // 响应帧
            MemoryStream SocketStream;
            MemoryStream RcvdStream;
            byte[] Buf;
            int Len;
            DateTime tmpTime, tmpTime2;
            

            fmGetUserReq.MsgHead.MsgType = UConstDefine.TM_GETGRPINFO_REQ;
            fmGetUserReq.MsgHead.MsgLength = Marshal.SizeOf(typeof(UMsgDefine.FM_GetGrpInfo_Req));
            fmGetUserReq.UserID = DM_Client.MyRecords.UserInfo.UserID;

            fmGetQtyRsp.count = 0;
            fmGetQtyRsp.FileLen = 0;

            lock (DM_Client.CsGrpUser)
            {
                lock (DM_Client.CsSocket)
                {
                    byte[] Message = UMsgDefine.StructToBytes(fmGetUserReq);
                    DM_Client.TCPSocket.Send(Message, fmGetUserReq.MsgHead.MsgLength, SocketFlags.None);

                    byte[] RcvMessage = new byte[Marshal.SizeOf(typeof(UMsgDefine.FM_GetGrpInfo_Rsp))];
                    DM_Client.TCPSocket.Receive(RcvMessage, RcvMessage.Length, SocketFlags.None);
                    fmGetUserRsp = (UMsgDefine.FM_GetGrpInfo_Rsp)UMsgDefine.BytesToStruct(RcvMessage, typeof(UMsgDefine.FM_GetGrpInfo_Rsp));
                    if (fmGetUserRsp.MsgHead.MsgType == UConstDefine.TM_GETGRPINFO_RSP)
                    {
                        if (fmGetUserRsp.RspCode == UConstDefine.RM_RSP_OK)
                        {
                            RcvdStream = new MemoryStream();
                            Buf = new byte[8192];
                            while (RcvdStream.Length < fmGetUserRsp.BufLen)
                            {
                                DM_Client.TCPSocket.Receive(Buf, 8192, SocketFlags.None);
                                RcvdStream.Write(Buf, 0, 8192);
                            }
                            Len = Marshal.SizeOf(typeof(UMyRecords.stGrpUserInfo));
                            if (fmGetUserRsp.BufLen % Len == 0)
                            {
                                RcvdStream.Position = 0;
                                sql = "create table GrpUser(" +
                  "UserId Varchar(16),UserRole integer,UserName varchar(16),UserTrueName varchar(16),VolumeName varchar(32),TopGrpNo integer,"
                  +
                  "MidGrpNo integer,BtmGrpNo integer,RejudgeSmpRatio float,SelfRejudgeRatio float," + "TaskAmount integer,SmpAmount integer,Flag Logical)";
                                DM_Client.DAO.ADOConnection1.Open();
                                DM_Client.DAO.ExecuteQuery(sql);
                                DM_Client.DAO.ADOConnection1.Close();

                                DM_Client.MyRecords.GrpUserList = new List<UMyRecords.stMyUser>();
                                DM_Client.MyRecords.GrpUserList.Clear();

                                while (RcvdStream.Position < RcvdStream.Length)
                                {
                                    byte[] buff = new byte[Len];
                                    RcvdStream.Read(buff, 0, Len);
                                    TmpUser = (UMyRecords.stGrpUserInfo)UMsgDefine.BytesToStruct(buff, typeof(UMyRecords.stGrpUserInfo));
                                    //将用户信息写入数据库
                                    sql = "insert into GrpUser values(" + "'" +
                                      TmpUser.UserID.ToString() + "'" + "," + TmpUser.UserRole.ToString()
                                      + "," + "'" + new string(TmpUser.UserName).TrimEnd('\0') + "'"
                                      + "," + "'" + new string(TmpUser.UserTrueName).TrimEnd('\0') + "'"
                                      + "," + "'" + new string(TmpUser.VolumeName).TrimEnd('\0') + "'"
                                      + "," + TmpUser.TopGrpNo.ToString() + "," +
                                      TmpUser.MidGrpNo.ToString() + "," +
                                      TmpUser.BtmGrpNo.ToString() + "," +
                                      TmpUser.RejudgeSmpRatio.ToString() + "," +
                                      TmpUser.SelfRejudgeRatio.ToString() + "," +
                                      TmpUser.TaskAmount.ToString() + "," + TmpUser.SmpAmount.ToString()
                                      + ",0)";
                                    DM_Client.DAO.ADOConnection1.Open();
                                    DM_Client.DAO.ExecuteQuery(sql);
                                    DM_Client.DAO.ADOConnection1.Close();


                                    PTmpUser = new UMyRecords.stMyUser();
                                    PTmpUser.UserID = TmpUser.UserID;
                                    PTmpUser.UserRole = TmpUser.UserRole;
                                    PTmpUser.UserName = TmpUser.UserName;
                                    PTmpUser.UserTrueName = TmpUser.UserTrueName;
                                    PTmpUser.VolumeName = TmpUser.VolumeName;
                                    PTmpUser.TopGrpNo = TmpUser.TopGrpNo;
                                    PTmpUser.BtmGrpNo = TmpUser.BtmGrpNo;
                                    PTmpUser.MidGrpNo = TmpUser.MidGrpNo;
                                    PTmpUser.RejudgeSmpRatio = TmpUser.RejudgeSmpRatio;
                                    PTmpUser.SelfRejudgeRatio = TmpUser.SelfRejudgeRatio;
                                    PTmpUser.TaskAmount = TmpUser.TaskAmount;
                                    PTmpUser.SmpAmount = TmpUser.SmpAmount;
                                    PTmpUser.Flag = false;

                                    DM_Client.MyRecords.GrpUserList.Add(PTmpUser);

                                }
                            }
                            else
                            {
                                Content = "组用户信息长度不合法，您需要重新获取组用户信息!";
                                UMyFuncs.WriteLogFile(DM_Client, Content);
                            }

                        }
                        else
                        {
                            Content = "获取组用户信息时出现错误!";
                            UMyFuncs.WriteLogFile(DM_Client, Content);
                        }

                    }
                    else
                    {
                        Content = "获取组用户信息时收到错误的响应帧!";
                        UMyFuncs.WriteLogFile(DM_Client, Content);
                    }
                    
                }
            }
            DM_Client.MyRecords.GrpUserList.Sort(new SortUserName());
            DM_Client.FirstFlag = true;
        }
        public static int GetBlockInfo(TDM_Client DM_Client, int BlockType, int InfoLen, string VolStr, string ErrorMsg)
        {
            UMsgDefine.FM_GetBlkInfo_Req fmGetBlkReq;
            UMsgDefine.FM_GetBlkInfo_Rsp fmGetBlkRsp;
            UMsgDefine.FM_GetBlkInfo_Rsp_Login fmGetBlkRspTemp;
            UMyRecords.stFullBlockInfo PAddBlk;
            UMyRecords.stTipsInfo PAddTips;
            int Len, PicFormat;
            DateTime tmpTime;
            MemoryStream RcvdStream = new MemoryStream();
            switch (BlockType)
            {
                case 1:
                    fmGetBlkReq.MsgHead.MsgType = UConstDefine.TM_GETBLKINFO_REQ;
                    fmGetBlkReq.MsgHead.MsgLength = Marshal.SizeOf(typeof(UMsgDefine.FM_GetBlkInfo_Req));
                    fmGetBlkReq.InfoType = UConstDefine.PM_GETBLKINFO_NORMAL;
                    fmGetBlkReq.VolumeName = VolStr.PadRight(UConstDefine.VolLen, '\0').ToCharArray();
                    lock (DM_Client.CsSocket)
                    {
                        byte[] Message = UMsgDefine.StructToBytes(fmGetBlkReq);
                        DM_Client.TCPSocket.Send(Message, fmGetBlkReq.MsgHead.MsgLength, SocketFlags.None);
                        DM_Client.TCPSocket.SendTimeout = UConstDefine.TimeOutSeconds;

                        fmGetBlkRsp.BlockInfo.CheckRule = "".PadRight(256, '\0').ToCharArray();
                        fmGetBlkRsp.BlockInfo.VolName = "".PadRight(UConstDefine.VolLen, '\0').ToCharArray();
                        fmGetBlkRsp.BlockInfo.RevaluateRule = "".PadRight(24, '\0').ToCharArray();

                        byte[] RcvMessage = new byte[Marshal.SizeOf(typeof(UMsgDefine.FM_GetBlkInfo_Rsp_Login))];
                        DM_Client.TCPSocket.Receive(RcvMessage, Marshal.SizeOf(typeof(UMsgDefine.FM_GetBlkInfo_Rsp_Login)), SocketFlags.None);
                        fmGetBlkRspTemp = (UMsgDefine.FM_GetBlkInfo_Rsp_Login)UMsgDefine.BytesToStruct(RcvMessage, typeof(UMsgDefine.FM_GetBlkInfo_Rsp_Login));

                        if (fmGetBlkRspTemp.MsgHead.MsgType == UConstDefine.TM_GETBLKINFO_RSP)
                        {
                            if (fmGetBlkRspTemp.RspCode == UConstDefine.RM_RSP_OK)
                            {
                                PAddBlk = new UMyRecords.stFullBlockInfo();
                                PAddBlk.BlockInfo.VolName = fmGetBlkRspTemp.BlockInfo.VolName;
                                PAddBlk.BlockInfo.QuestionNo = fmGetBlkRspTemp.BlockInfo.QuestionNo;
                                PAddBlk.BlockInfo.BlkSeqNo = fmGetBlkRspTemp.BlockInfo.BlkSeqNo;
                                PAddBlk.BlockInfo.nNEv = fmGetBlkRspTemp.BlockInfo.nNEv;
                                PAddBlk.BlockInfo.CheckRule = fmGetBlkRspTemp.BlockInfo.CheckRule;
                                PAddBlk.BlockInfo.RevaluateRule = fmGetBlkRspTemp.BlockInfo.RevaluateRule;
                                PAddBlk.BlockInfo.CheckScore = fmGetBlkRspTemp.BlockInfo.CheckScore;
                                PAddBlk.BlockInfo.DispScore = fmGetBlkRspTemp.BlockInfo.DispScore;
                                PAddBlk.BlockInfo.BufSize = fmGetBlkRspTemp.BlockInfo.BufSize;
                                PAddBlk.BlockInfo.TimeOut = fmGetBlkRspTemp.BlockInfo.TimeOut;
                                PAddBlk.BlockInfo.QuestionType = fmGetBlkRspTemp.BlockInfo.QuestionType;
                                PAddBlk.BlockInfo.SampleTestMin = fmGetBlkRspTemp.BlockInfo.SampleTestMin;
                                PAddBlk.BlockInfo.MaxSize = fmGetBlkRspTemp.BlockInfo.MaxSize;
                                PAddBlk.BlockInfo.MinSize = fmGetBlkRspTemp.BlockInfo.MinSize;
                                PAddBlk.BlockInfo.AvgSize = fmGetBlkRspTemp.BlockInfo.AvgSize;
                                PAddBlk.BlockInfo.MinTime = fmGetBlkRspTemp.BlockInfo.MinTime;
                                DM_Client.MyVars.CurImageLimit.MaxSize = fmGetBlkRspTemp.BlockInfo.MaxSize;
                                DM_Client.MyVars.CurImageLimit.MinSize = fmGetBlkRspTemp.BlockInfo.MinSize;
                                DM_Client.MyVars.CurImageLimit.AvgSize = fmGetBlkRspTemp.BlockInfo.AvgSize;
                                DM_Client.MyVars.CurImageLimit.SpeedLimit = fmGetBlkRspTemp.BlockInfo.MinTime;
                                PAddBlk.BlockInfo.TotalTestMin = fmGetBlkRspTemp.BlockInfo.TotalTestMin;
                                PAddBlk.BlockInfo.ServerID = fmGetBlkRspTemp.BlockInfo.ServerID;
                                PAddBlk.BlockInfo.TipsLen = fmGetBlkRspTemp.BlockInfo.TipsLen;
                                PAddBlk.BlockInfo.JunkInfo = fmGetBlkRspTemp.BlockInfo.JunkInfo;
                                PAddBlk.BlockInfo.nMarkModelLen = fmGetBlkRspTemp.BlockInfo.nMarkModelLen;
                                PAddBlk.BlockInfo.MarkModelInfo = fmGetBlkRspTemp.BlockInfo.MarkModelInfo;

                                PAddBlk.TipsList = new List<UMyRecords.stTipsInfo>();
                                PAddBlk.TipsList.Clear();

                                PAddBlk.PingFenXiZeList = new List<UMyRecords.stPingFenXiZe>();
                                PAddBlk.PingFenXiZeList.Clear();

                                PAddBlk.DaAnList = new List<UMyRecords.stDaAn>();
                                PAddBlk.DaAnList.Clear();

                                RcvMessage = new byte[PAddBlk.BlockInfo.nMarkModelLen];
                                DM_Client.TCPSocket.Receive(RcvMessage, PAddBlk.BlockInfo.nMarkModelLen, SocketFlags.None);
                                char[] buf = new char[PAddBlk.BlockInfo.nMarkModelLen];
                                buf = System.Text.Encoding.Default.GetChars(RcvMessage);
                                PAddBlk.BlockInfo.MarkModelList = (char[])buf.Clone();

                                DM_Client.MyRecords.BlockInfoList.Add(PAddBlk);



                                return PAddBlk.BlockInfo.TipsLen;
                            }
                            else
                            {
                                ErrorMsg = "获取题块信息时出现错误!";
                                return -1;
                            }
                        }
                        else
                        {
                            ErrorMsg = "获取题块信息时收到错误的响应帧";
                            return -1;
                        }
                    }
                case 2:
                    fmGetBlkReq.MsgHead.MsgType = UConstDefine.TM_GETBLKINFO_REQ;
                    fmGetBlkReq.MsgHead.MsgLength = fmGetBlkReq.MsgHead.MsgLength = Marshal.SizeOf(typeof(UMsgDefine.FM_GetBlkInfo_Req));
                    fmGetBlkReq.InfoType = UConstDefine.PM_GETBLKINFO_TIPS;
                    fmGetBlkReq.VolumeName = VolStr.PadRight(UConstDefine.VolLen, '\0').ToCharArray();


                    lock (DM_Client.CsSocket)
                    {
                        byte[] Message = UMsgDefine.StructToBytes(fmGetBlkReq);
                        DM_Client.TCPSocket.Send(Message, fmGetBlkReq.MsgHead.MsgLength, SocketFlags.None);
                        DM_Client.TCPSocket.SendTimeout = UConstDefine.TimeOutSeconds;

                        //for adding --network stopping
                        byte[] buff = new byte[UConstDefine.BufferSize];
                        while (RcvdStream.Length < InfoLen)
                        {
                            Len = DM_Client.TCPSocket.Receive(buff, UConstDefine.BufferSize, SocketFlags.None);
                            RcvdStream.Write(buff, 0, Len);
                        }
                        if (RcvdStream.Length == InfoLen)
                        {
                            RcvdStream.Position = 0;
                            if (DM_Client.MyRecords.BlockInfoList.Count > 0)
                            {
                                PAddBlk = DM_Client.MyRecords.BlockInfoList.Last();
                                PAddBlk.TipsList.Clear();

                                while (RcvdStream.Position < RcvdStream.Length)
                                {
                                    byte[] RcvMessage = new byte[Marshal.SizeOf(typeof(int))];
                                    RcvdStream.Read(RcvMessage, 0, Marshal.SizeOf(typeof(int)));
                                    Len = BitConverter.ToInt32(RcvMessage, 0);
                                    RcvdStream.Read(RcvMessage, 0, Marshal.SizeOf(typeof(int)));
                                    PicFormat = BitConverter.ToInt32(RcvMessage, 0);

                                    PAddTips = new UMyRecords.stTipsInfo();

                                    PAddTips.TipsImage = new MemoryStream();

                                    RcvMessage = new byte[Len];
                                    RcvdStream.Read(RcvMessage, 0, Len);
                                    PAddTips.TipsImage.Write(RcvMessage, 0, Len);

                                    PAddBlk.TipsList.Add(PAddTips);
                                }
                            }
                        }
                        RcvdStream.Close();
                        return -1;
                    }
            }
            return -1;
        }
        public static void LoadJP2(TDM_Client DM_Client, UMyRecords.stMyPaper PTmpPaper, string picTemp)                     // 复查时调用
        {
            string tmpstr1 = new String(DM_Client.MyRecords.UserInfo.LoginName);
            tmpstr1 = tmpstr1.Substring(0, tmpstr1.IndexOf('\0'));
            if (!System.IO.Directory.Exists(picTemp + "/" + tmpstr1))
            {
                System.IO.Directory.CreateDirectory(picTemp + "/" + tmpstr1);
            }
            FreeImageBitmap fbm = FreeImageBitmap.FromStream(PTmpPaper.PaperImage);
            fbm.Save(picTemp + "/" + tmpstr1 + "/" + PTmpPaper.PaperInfo.PaperNo.ToString() + ".bmp", FREE_IMAGE_FORMAT.FIF_BMP);
            string Content = "普通取卷请求成功";
            UMyFuncs.WriteLogFile(DM_Client, Content);
        }
        public static void LoadJP2(TDM_Client DM_Client, UMyRecords.stMyPaper PTmpPaper, string picTemp, Boolean NextPaper)  //重载,正常预取函数
        {
            string tmpstr1 = new String(DM_Client.MyRecords.UserInfo.LoginName);
            tmpstr1 = tmpstr1.Substring(0, tmpstr1.IndexOf('\0'));
            if (!System.IO.Directory.Exists(picTemp + "/" + tmpstr1))
            {
                System.IO.Directory.CreateDirectory(picTemp + "/" + tmpstr1);
            }
            
            FreeImageBitmap fbm = FreeImageBitmap.FromStream(PTmpPaper.PaperImage);
            fbm.Save(picTemp + "/" + tmpstr1 + "/" + PTmpPaper.PaperInfo.PaperNo.ToString() + ".bmp", FREE_IMAGE_FORMAT.FIF_BMP);
            DM_Client.NextPaper = true;     //已预取
            DM_Client.NextPaperNo = PTmpPaper.PaperInfo.PaperNo;
            string Content = "预取请求成功";
            UMyFuncs.WriteLogFile(DM_Client, Content);
        }
        
        public static int GetExcpPaper(TDM_Client DM_Client, ref UMyRecords.stMyPaper PAddPaper,int ExcpRsn,string VolStr)
        {
            int Len;
            MemoryStream RcvMS = new MemoryStream();
            byte[] Buf = new byte[8192];
            UMsgDefine.FM_GetPaper_Req fmGetPaperReq;
            UMsgDefine.FM_GetPaper_Rsp fmGetPaperRsp;
            UMyRecords.stScoreForExcp TmpScoreExcp;
            DateTime tmpTime;
            int result = -2;


            PAddPaper = DM_Client.JunkList.First();

            fmGetPaperReq.MsgHead.MsgType = UConstDefine.TM_GETPAPER_REQ;
            fmGetPaperReq.MsgHead.MsgLength = Marshal.SizeOf(typeof(UMsgDefine.FM_GetPaper_Req));
            fmGetPaperReq.GetPaperTask.PaperType = UConstDefine.PM_PAPERTYPE_EXCEPTION;
            fmGetPaperReq.GetPaperTask.UserID = DM_Client.MyRecords.UserInfo.UserID;
            fmGetPaperReq.GetPaperTask.DispOrBlkNo = GetBlkNo(VolStr);
            fmGetPaperReq.GetPaperTask.PaperNoOrReason = ExcpRsn;
            fmGetPaperReq.GetPaperTask.VolName = VolStr.PadRight(UConstDefine.VolLen, '\0').ToCharArray();
            fmGetPaperReq.GetPaperTask.nAllPaper = 0;
            lock (DM_Client.CsSocket)
            {
                byte[] Message = UMsgDefine.StructToBytes(fmGetPaperReq);
                DM_Client.TCPSocket.Send(Message, fmGetPaperReq.MsgHead.MsgLength, SocketFlags.None);

                byte[] RcvMessage = new byte[Marshal.SizeOf(typeof(UMsgDefine.FM_GetPaper_Rsp))];
                DM_Client.TCPSocket.Receive(RcvMessage, Marshal.SizeOf(typeof(UMsgDefine.FM_GetPaper_Rsp)), SocketFlags.None);
                fmGetPaperRsp = (UMsgDefine.FM_GetPaper_Rsp)UMsgDefine.BytesToStruct(RcvMessage, typeof(UMsgDefine.FM_GetPaper_Rsp));
                if (fmGetPaperRsp.MsgHead.MsgType == UConstDefine.TM_GETPAPER_RSP)
                {
                    result = fmGetPaperRsp.RspCode;
                    if (fmGetPaperRsp.RspCode == UConstDefine.RM_RSP_OK)
                    {
                        PAddPaper.PaperInfo = fmGetPaperRsp.PaperData;

                        RcvMessage = new byte[Marshal.SizeOf(typeof(UMyRecords.stScoreForExcp))];
                        DM_Client.TCPSocket.Receive(RcvMessage, Marshal.SizeOf(typeof(UMyRecords.stScoreForExcp)), SocketFlags.None);
                        TmpScoreExcp = (UMyRecords.stScoreForExcp)UMsgDefine.BytesToStruct(RcvMessage, typeof(UMyRecords.stScoreForExcp));

                        PAddPaper.ScoreInfo.BlkNo = TmpScoreExcp.BlkNo;
                        PAddPaper.ScoreInfo.UserIDOrPaperSeq = TmpScoreExcp.UserID;
                        PAddPaper.ScoreInfo.ExcpReason = TmpScoreExcp.ExcpReason;
                        PAddPaper.ScoreInfo.TotalScore = -100;
                        PAddPaper.ScoreInfo.Txt = TmpScoreExcp.ExcpTxt;
                        //接收试题图片  第一种为从文件服务器读取
                        if (PAddPaper.PaperInfo.ImageLen == 0)  //TODO:tobeadded
                        {
                          
                            UMyFuncs.GetPicFromServer(DM_Client, PAddPaper.PaperImage, PAddPaper.PaperInfo.PicPath);
                            PAddPaper.Flag = false;
                            if (PAddPaper.PaperImage.Length > 0)
                            {
                                PAddPaper.Flag = true;
                                PAddPaper.Status = false;
                            }
                            else
                            {
                                string Content = "获取异常试卷图片失败!";
                                UMyFuncs.WriteLogFile(DM_Client, Content);
                            }
                        }
                        else
                        {
                            RcvMS.Flush();
                            while (RcvMS.Length <  PAddPaper.PaperInfo.ImageLen)
                            {
                                Len = DM_Client.TCPSocket.Receive(Buf, 8192, SocketFlags.None);
                                if (Len <= 0)
                                {
                                    if (RcvMS.Length < PAddPaper.PaperInfo.ImageLen) //TODO:tobeadded
                                    {

                                    }
                                }
                                RcvMS.Write(Buf, 0, 8192);
                            }
                            if (RcvMS.Length == PAddPaper.PaperInfo.ImageLen)
                            {
                                RcvMS.Position = 0;

                                PAddPaper.PaperImage.Flush();
                                RcvMS.CopyTo(PAddPaper.PaperImage, 0);
                                PAddPaper.Flag = true;
                                PAddPaper.Status = false;
                                RcvMS.Flush();

                            }
                            RcvMS.Flush();
                        }
                        PAddPaper.SaveType = -2;
                    }                 
                    return result;
                }
            }
            return result;

        }
    }
}