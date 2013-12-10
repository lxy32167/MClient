using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Microsoft.VisualBasic;
namespace MClient.App_Code
{
    public class UMyFuncs
    {
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
            for (int i = 0; i < str.Length; i++)
            {
                if (!Char.IsNumber(str, i))
                    return false;
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
    }
}