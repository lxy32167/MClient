using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
using FreeImageAPI;
using MClient.App_Code;
using System.Web.UI.DataVisualization.Charting;
using System.Web.Services;

namespace MClient
{
    public partial class Xzz : System.Web.UI.Page
    {
        //TODO:ClearInfo(4);
        [DllImport("kernel32")]
        static extern uint GetTickCount();
        TDM_Client DM_Client;
        TBeaconThreadClass TBeaconClass;
        TUDPListenedClass TUDPClass;
        TRcvPaperThreadClass TRcvPaperClass;
        TSaveScoreThreadClass TSaveScoreClass;
        TPreFetchThreadClass TPreFetchClass;
        int Review_RowCount;
        UCheck Check = new UCheck();
        UnitGlobalV GlobalV = new UnitGlobalV();
        protected void Page_Load(object sender, EventArgs e)
        {
            form1.DefaultButton = "ButtonSubmit";
            this.Buttonzhuxiao.Attributes.Add("onclick", "return confirm('确定返回登陆界面?');");

            DM_Client = (TDM_Client)Session["DM_Client"];
            TBeaconClass = (TBeaconThreadClass)Session["TBeaconClass"];
            TUDPClass = (TUDPListenedClass)Session["TUDPClass"];
            if (Session["DM_Client"] == null)
            {
                Response.Redirect("~/Login.aspx");
            }
            if (!IsPostBack)
            {
                LabelName.Text = "小组长" + new String(DM_Client.MyRecords.UserInfo.TrueName) + "已登陆";
                switch (DM_Client.MyRecords.UserInfo.Status)
                {
                    case UConstDefine.Trial:
                        LabelStatus.Text = "状态:试用";
                        break;
                    case UConstDefine.YangPing:
                        LabelStatus.Text = "状态:样评";
                        break;
                    case UConstDefine.CePing:
                        LabelStatus.Text = "状态:测评";
                        break;
                    case UConstDefine.ZhengPing:
                        LabelStatus.Text = "状态:正评";
                        break;
                }
                ButtonStart_Click(); //为了便于表单动态绑定 设定为开始直接点击阅卷
                //第一次启动该函数，会启动其他线程并保存状态
                FirstCheckData();  //初始化Check
            }
            Check = (UCheck)Session["Check"];
            TSaveScoreClass = (TSaveScoreThreadClass)Session["TSaveScoreClass"];
            TRcvPaperClass = (TRcvPaperThreadClass)Session["TRcvPaperClass"];
            TPreFetchClass = (TPreFetchThreadClass)Session["TPreFetchClass"];
            if (getPostBackControlName() == "ButtonSubmit")
            {
                SaveScoreString(); ////在Postback绑定之前，获取上次输入数据
            }
            //if (getPostBackControlName() == "ButtonBlank")
            //{
            //    SaveZeroString();
            //}
            LoadScoreData();   //原读取评分标准函数,decodeinfo移位 ,
            LockUserData();

            ((TextBox)(GridViewScore.Rows[0].FindControl("txtScore"))).Focus();
        }

        //private void SaveZeroString()
        //{
        //    int i, j, k, m, n;
        //    UMyRecords.stMyPaper PTmpPaper;
        //    Boolean flag = false;
        //    Check.ScoreInput = new string[DM_Client.tblScore.Rows.Count];
        //    j = 0;
        //    if (!Check.status_review) //正常阅卷
        //    {
        //        for (j = 0; j < DM_Client.MyRecords.CurBlockInfo.BlockInfo.BufSize; j++)
        //        {
        //            if (DM_Client.stCheck[j].PaperNo == -1)       //如果还有空位
        //            {
        //                flag = true;
        //                DM_Client.stCheck[j].PaperNo = DM_Client.MyVars.CurPaperID; //记录当前试卷号
        //                break;
        //            }
        //        }
        //        if (!flag)                   //没有空位
        //        {
        //            j = DM_Client.MyRecords.CurBlockInfo.BlockInfo.BufSize - 1;    //复查列表没找到，替换掉最后一个数据
        //            DM_Client.stCheck[j].PaperNo = DM_Client.MyVars.CurPaperID;
        //            for (m = 0; m < DM_Client.MyRecords.CurBlockInfo.BlockInfo.BufSize - 1; m++)   //分数前移
        //            {
        //                for (n = 0; n < DM_Client.tblScore.Rows.Count; n++)
        //                {
        //                    DM_Client.stCheck[m].DetailScore[n] = DM_Client.stCheck[m + 1].DetailScore[n];
        //                }
        //                DM_Client.stCheck[m].PaperNo = DM_Client.stCheck[m + 1].PaperNo;
        //            }
        //        }

        //    }
        //    else                      //复查状态,肯定会找到对应试卷
        //    {
        //        k = 0;
        //        if (DM_Client.OldPaperList.Count > 0)
        //        {
        //            for (i = 0; i < DM_Client.OldPaperList.Count; i++)
        //            {
        //                PTmpPaper = DM_Client.OldPaperList[i];
        //                if (PTmpPaper.Status)                 //复查列表寻找
        //                {
        //                    if (PTmpPaper.PaperInfo.PaperNo == DM_Client.MyVars.CurPaperID)  //找到试卷
        //                    {
        //                        j = k;       //赋值并退出循环
        //                        DM_Client.stCheck[j].PaperNo = PTmpPaper.PaperInfo.PaperNo;
        //                        break;
        //                    }
        //                    k++;             //试卷号不符合，复查列表下一个
        //                }
        //            }
        //        }
        //    }
        //    for (i = 0; i < DM_Client.tblScore.Rows.Count; i++)
        //    {
        //        if (i == DM_Client.MyVars.PositiveEndRow)
        //        {
        //            continue;
        //        }
        //        Check.ScoreInput[i] = "0";
        //        DM_Client.stCheck[j].DetailScore[i] = Check.ScoreInput[i];  //记录第m个试卷的每个题块分数
        //    }
        //    Session["Check"] = Check;
        //}

        private void SaveScoreString()
        {
            int i, j, k, m, n;
            UMyRecords.stMyPaper PTmpPaper;
            Boolean flag = false;
            Check.ScoreInput = new string[DM_Client.tblScore.Rows.Count];
            j = 0;
            Check.stCheckTemp = (DataModule.stCheck[])UMsgDefine.Clone(DM_Client.stCheck);
            if (!Check.status_review) //正常阅卷
            {
                for (j = 0; j < DM_Client.MyRecords.CurBlockInfo.BlockInfo.BufSize; j++)
                {
                    if (DM_Client.stCheck[j].PaperNo == -1)       //如果还有空位
                    {
                        flag = true;
                        DM_Client.stCheck[j].PaperNo = DM_Client.MyVars.CurPaperID; //记录当前试卷号
                        break;
                    }
                }
                if (!flag)                   //没有空位
                {
                    j = DM_Client.MyRecords.CurBlockInfo.BlockInfo.BufSize - 1;    //复查列表没找到，替换掉最后一个数据
                    DM_Client.stCheck[j].PaperNo = DM_Client.MyVars.CurPaperID;
                    for (m = 0; m < DM_Client.MyRecords.CurBlockInfo.BlockInfo.BufSize - 1; m++)   //分数前移
                    {
                        for (n = 0; n < DM_Client.tblScore.Rows.Count; n++)
                        {
                            DM_Client.stCheck[m].DetailScore[n] = DM_Client.stCheck[m + 1].DetailScore[n];
                        }
                        DM_Client.stCheck[m].PaperNo = DM_Client.stCheck[m + 1].PaperNo;
                    }
                }

            }
            else                      //复查状态,肯定会找到对应试卷
            {
                k = 0;
                if (DM_Client.OldPaperList.Count > 0)
                {
                    for (i = 0; i < DM_Client.OldPaperList.Count; i++)
                    {
                        PTmpPaper = DM_Client.OldPaperList[i];
                        if (PTmpPaper.Status)                 //复查列表寻找
                        {
                            if (PTmpPaper.PaperInfo.PaperNo == DM_Client.MyVars.CurPaperID)  //找到试卷
                            {
                                j = k;       //赋值并退出循环
                                DM_Client.stCheck[j].PaperNo = PTmpPaper.PaperInfo.PaperNo;
                                break;
                            }
                            k++;             //试卷号不符合，复查列表下一个
                        }
                    }
                }
            }
            for (i = 0; i < DM_Client.tblScore.Rows.Count; i++)
            {
                if (i == DM_Client.MyVars.PositiveEndRow)
                {
                    continue;
                }
                Check.ScoreInput[i] = ((TextBox)(GridViewScore.Rows[i].FindControl("txtScore"))).Text;
                DM_Client.stCheck[j].DetailScore[i] = Check.ScoreInput[i];  //记录第m个试卷的每个题块分数
            }
            Session["Check"] = Check;
        }
    
        /// <summary>
        /// 给锁定窗口的解锁用户名和密码赋值 
        /// </summary>
        protected void LockUserData()
        {
            string tmpstr1 = new String(DM_Client.MyRecords.UserInfo.LoginName);
            tmpstr1 = tmpstr1.Substring(0, tmpstr1.IndexOf('\0'));
            string username, userpwd;
            username = tmpstr1;
            userpwd = (string)Session["Password"];
            hidden.Value = username;
            hidden1.Value = userpwd;

        }
        public void FirstCheckData()
        {
            DataTable tbl = new DataTable("CheckScore");
            tbl.Columns.Add("序号", typeof(string));
            tbl.Columns.Add("试卷号", typeof(string));
            tbl.Columns.Add("分数", typeof(string));
            for (int i = 1; i <= DM_Client.MyRecords.CurBlockInfo.BlockInfo.BufSize; i++)
            {
                tbl.Rows.Add(i, "", "");
            }
            GridViewCheck.DataSource = tbl;
            GridViewCheck.DataBind();
        }
        protected void ButtonStart_Click()
        {
            int Len, i;
            string tmpstr, ErrorMsg;
            UMyRecords.stMyPaper PAddPaper;
            UMyRecords.stFullBlockInfo BlkInfo;
            uint ExitCode;
            ErrorMsg = "";

            CheckShow();
            if (!DM_Client.MyVars.BlockOK)      //第一次调用
            {
                DM_Client.MyRecords.BlockInfoList.Clear();
                if (DM_Client.MyRecords.AssignList.Count > 0)
                {
                    for (i = 0; i < DM_Client.MyRecords.AssignList.Count; i++)
                    {
                        tmpstr = DM_Client.MyRecords.AssignList[i];
                        Len = UMyFuncs.GetBlockInfo(DM_Client, 1, 0, tmpstr, ErrorMsg);
                        if (Len >= 0)
                        {
                            if (Len > 0)
                            {
                                UMyFuncs.GetBlockInfo(DM_Client, 2, Len, tmpstr, ErrorMsg);
                            }
                            DM_Client.MyVars.BlockOK = true;
                        }
                    }

                    if (DM_Client.MyVars.CurVolName.Equals(""))
                    {
                        UMyFuncs.GetNextVol(DM_Client);
                    }
                    if (DM_Client.MyVars.BlockOK)
                    {
                        TRcvPaperClass = new TRcvPaperThreadClass(DM_Client);
                        TSaveScoreClass = new TSaveScoreThreadClass(DM_Client);
                        TPreFetchClass = new TPreFetchThreadClass(DM_Client);
                        TPreFetchClass.picTemp = Server.MapPath(".");

                        TSaveScoreClass.TSaveScoreThread.Start();

                    }
                }
            }
            else
            {
                TSaveScoreClass.ToContinue = true;
                if (DM_Client.MyRecords.UserInfo.Role < UConstDefine.XiaoZuZhang)  //普通用户
                {

                }
                else //组长——区分仲裁试卷和异常试卷
                {
                    DM_Client.ToCreateImage = false;

                    if (RadioButtonList1.SelectedIndex == 0)
                    {
                        if (!TRcvPaperClass.ToContinue)
                        {
                            TRcvPaperClass.ToContinue = true;
                            Thread.Sleep(50);
                        }
                        DM_Client.ToCreateImage = ShowImage(0);
                    }
                    else if (RadioButtonList1.SelectedIndex == 1)
                    {
                        DM_Client.ToCreateImage = ShowImage(2);
                    }
                    if (DM_Client.ToCreateImage)
                    {
                        LabelName.Text = "正常阅卷状态";
                        UpdateLabelName.Update();
                    }

                }
            }
            //Thread.Sleep(1000);

            //hidden2.Value = GlobalV.WindowTime.ToString();

            //ShowImage(0);
            //Check.FirstQtyFlag = true;
            //Check.status_review = false;
            //lbTips.Text = "个人已阅/题组已阅";
            //BlkInfo = DM_Client.MyRecords.BlockInfoList[0];
            ////如果是多评题是多评题才显示个人有效阅卷数
            //if (BlkInfo.BlockInfo.nNEv > 1)
            //{
            //    lbTips.Text = "个人有效/" + LabelQty.Text;
            //}

            Session["TRcvPaperClass"] = TRcvPaperClass;
            Session["TSaveScoreClass"] = TSaveScoreClass;
            Session["TPreFetchClass"] = TPreFetchClass;
            Session["DM_Client"] = DM_Client;
            Session["Check"] = Check;
        }
        protected void CheckShow() //原客户端TF_Check.FormCreate函数
        {
            int i;
            UMyRecords.stExcpMap PTmpRsn;

            if (DM_Client.MyRecords.CurBlockInfo.BlockInfo.VolName.Length != 0)
            {
                DM_Client.MyVars.QuestionType = DM_Client.MyRecords.CurBlockInfo.BlockInfo.QuestionType;
                Review_RowCount = DM_Client.MyRecords.CurBlockInfo.BlockInfo.BufSize;

                if ((DM_Client.MyRecords.UserInfo.Role > UConstDefine.PuTong) && (DM_Client.MyRecords.CurBlockInfo.BlockInfo.DispScore == 1))
                {
                    DM_Client.MyVars.AllowDisplay = true;
                }
                else
                {
                    DM_Client.MyVars.AllowDisplay = false;
                }

                DM_Client.MyVars.CurFullScore = -1;    //初始化
                //      UMyFuncs.DecodeInfo(DM_Client, new string(DM_Client.MyRecords.CurBlockInfo.BlockInfo.CheckRule), 2);   //解析评分标准,分析各题块与满分 放在LoadScoreData()处理


                //查找当前题块信息并保存到PMyBlock中
                if (DM_Client.MyRecords.BlockInfoList.Count > 0)
                {
                    for (i = 0; i < DM_Client.MyRecords.BlockInfoList.Count; i++)
                    {
                        Check.PMyBlock = DM_Client.MyRecords.BlockInfoList[i];

                        if (new string(Check.PMyBlock.BlockInfo.VolName).TrimEnd('\0').Equals(DM_Client.MyVars.CurVolName))  //TODO:可能TrimEnd之后仍不相等
                        {
                            break;
                        }
                        else
                        {
                            //PMyBlock = null;
                        }
                    }
                }

                //获取套红框信息（需控件支持）
                //初始信息形如 5},(1,1,97,15),(1,18,97,30),(..),(..),(..)
                //StringRect:=strpas(PMyBlock^.BlockInfo.MarkModelList);
                //RectanglNum:=strtoint(copy(StringRect,1,pos('}',StringRect)-1));
                //StringRect:=copy(StringRect,pos(',',StringRect)+1,length(StringRect));  //形如(1,1,97,15),(1,18,97,30),(..),(..),(..)
                //tmpStringRect:=StringRect;

                switch (DM_Client.MyRecords.UserInfo.Status)
                {
                    case UConstDefine.YangPing:
                        Check.TestMin = DM_Client.MyRecords.CurBlockInfo.BlockInfo.SampleTestMin;
                        LabelStatus.Text = "状态:样评";
                        //目前不支持个人转换阅卷状态
                        if (DM_Client.MyVars.CurPaperNum >= Check.TestMin)   //若用户本次登录以来所改试卷数>=样评最小份数
                        {
                        }
                        else
                        {
                        }
                        if (DM_Client.MyRecords.UserInfo.SmpPaperTackled >= Check.TestMin)
                        {
                        }
                        break;
                    case UConstDefine.CePing:
                        Check.TestMin = DM_Client.MyRecords.CurBlockInfo.BlockInfo.TotalTestMin;
                        LabelStatus.Text = "状态:测评";
                        //目前不支持个人转换阅卷状态
                        if (DM_Client.MyVars.CurPaperNum >= Check.TestMin)   //若用户本次登录以来所改试卷数>=样评最小份数
                        {
                        }
                        else
                        {
                        }
                        if (DM_Client.MyRecords.UserInfo.TstPaperTackled >= Check.TestMin)
                        {
                        }
                        break;
                    case UConstDefine.Trial:
                        LabelStatus.Text = "状态:试用";
                        break;
                    case UConstDefine.ZhengPing:
                        LabelStatus.Text = "状态:正评";
                        break;
                }
                Check.tempstat3 = LabelStatus.Text;
            }
            string tmpstr1 = new String(DM_Client.MyRecords.UserInfo.LoginName);
            tmpstr1 = tmpstr1.Substring(0, tmpstr1.IndexOf('\0'));
            string tmpstr2 = new String(DM_Client.MyRecords.UserInfo.TrueName);
            tmpstr2 = tmpstr2.Substring(0, tmpstr2.IndexOf('\0'));
            LabelLoginName.Text = tmpstr1 + " " + tmpstr2;
            UpdatePanelLoginName.Update();
            Check.tempstat5 = LabelLoginName.Text;

            Check.YiJu = true;
            Check.LastPaper = false;
            Check.SampleOver = true;

            LabelName.Text = "正常阅卷状态";
            UpdatePanelStatus.Update();
            Check.tempstat1 = LabelName.Text;
            Check.tempstat2 = LabelPaperNo.Text;
            Check.tempstat3 = LabelStatus.Text;
            Check.tempstat4 = LabelPaperNum.Text;
            Check.tempstat5 = LabelLoginName.Text;

            if (true) //不可自动判分
            {
                if (DM_Client.MyRecords.ExcpRsnList.Count > 0) //加上异常原因
                {
                    for (i = 0; i <= 30; i++)
                        DM_Client.MyRecords.ArrRsnCode[i] = -1;     //赋予初值

                    for (i = 0; i < DM_Client.MyRecords.ExcpRsnList.Count; i++)
                    {
                        PTmpRsn = DM_Client.MyRecords.ExcpRsnList[i];
                        DM_Client.MyRecords.ArrRsnCode[i] = PTmpRsn.ReasonCode;
                    }

                    DM_Client.MyRecords.ArrRsnCode[i] = UConstDefine.SelfExcpCode;
                }
                else
                {
                    DM_Client.MyRecords.ArrRsnCode[0] = UConstDefine.SelfExcpCode;
                }
            }
        }
        /// <summary>
        /// 原CheckFormCreate里的解析评分标准
        /// </summary>
        protected void LoadScoreData()
        {
            String s, tmpStr, tmpDetail, Points;
            UMyRecords.stMyPaper PMyPaper;
            int i, j, k, code, row;
            Single TmpScore;
            string[] ScoreDetail;
            Boolean FirstFlag;
            DataTable tbl = new DataTable("InputScore");
            if (!IsPostBack) //第一次
            {
                s = new string(DM_Client.MyRecords.CurBlockInfo.BlockInfo.CheckRule);
                FirstFlag = true;
                DM_Client.MyVars.PositiveEndRow = -1;
                tmpStr = s;
                tmpStr = tmpStr.Substring(0, tmpStr.IndexOf('\0'));
                tmpStr = tmpStr.Substring(tmpStr.IndexOf('('));
                DM_Client.MyVars.CurFullScore = 0;
                switch (DM_Client.MyVars.QuestionType)
                {
                    case 1:
                    case 2:
                    case 3: //测试库都是3  格式为:{n},(str1,?,?,...Y/N(是否允许0.5分)),(str2,?,?,...),...(strn,?,?,...),Y/N(是否允许扣分)
                        tbl.Columns.Add("步骤", typeof(string));
                        tbl.Columns.Add("分数", typeof(string));
                        tbl.Columns.Add("满分", typeof(string));
                        tbl.Columns.Add("允许给半分", typeof(string));
                        j = 1;

                        while (tmpStr.IndexOf('(') >= 0)
                        {
                            Points = "";
                            Points = tmpStr.Substring(tmpStr.IndexOf('(') + 1, tmpStr.IndexOf(')') - tmpStr.IndexOf('(') - 1);
                            ScoreDetail = Points.Split(',');
                            if (ScoreDetail.Length != 3)
                            {
                                ScriptManager.RegisterStartupScript(this.UpdatePanel_submit, this.UpdatePanel_submit.GetType(), "msg", "<script>alert('解析得分要点错误');</script>", false);
                            }
                            if (!UMyFuncs.IsNum(ScoreDetail[1]))
                            {
                                ScriptManager.RegisterStartupScript(this.UpdatePanel_submit, this.UpdatePanel_submit.GetType(), "msg", "<script>alert('得分要点非法字符');</script>", false);
                            }
                            TmpScore = (float)Microsoft.VisualBasic.Conversion.Val(ScoreDetail[1]);
                            if (TmpScore > 0) //正分给分
                            {
                                DM_Client.MyVars.CurFullScore = DM_Client.MyVars.CurFullScore + TmpScore;
                            }
                            //else //负分给分点
                            //{
                            //    if (FirstFlag) //第一次出现负分
                            //    {
                            //        tbl.Rows.Add("负分给分区", "", "");
                            //        FirstFlag = false;
                            //        DM_Client.MyVars.PositiveEndRow = j - 1;
                            //        j++;
                            //    }
                            //}
                            tbl.Rows.Add(ScoreDetail[0], "", ScoreDetail[1], ScoreDetail[2]);
                            tmpStr = tmpStr.Substring(tmpStr.IndexOf(')') + 2);
                            j++;
                        }
                        //解析是否允许扣分
                        if (tmpStr.Substring(0, 1).Equals("Y"))
                        {
                            DM_Client.MyVars.AllowMinus = true;
                        }
                        else
                        {
                            DM_Client.MyVars.AllowMinus = false;
                        }
                        if (DM_Client.MyVars.PositiveEndRow == -1)
                        {
                            DM_Client.MyVars.PositiveEndRow = j - 1;
                        }
                        DM_Client.tblScore = tbl;
                        //复查用
                        DM_Client.stCheck = new DataModule.stCheck[DM_Client.MyRecords.CurBlockInfo.BlockInfo.BufSize];
                        for (int n = 0; n < DM_Client.MyRecords.CurBlockInfo.BlockInfo.BufSize; n++)
                        {
                            DM_Client.stCheck[n].PaperNo = -1;
                            DM_Client.stCheck[n].DetailScore = new string[DM_Client.tblScore.Rows.Count];  //初值
                        }
                        Session["DM_Client"] = DM_Client;  //第一次，更新session
                        break;
                    case 4:
                    case 5:  //测试库无数据 思路，做两个gridview 显示和隐藏
                        //for(j=0;j<UConstDefine.LvlCol;j++)
                        //{
                        //    for(k = 0;k<UConstDefine.LvlRow;k++)
                        //    {
                        //        DM_Client.MyRecords.ArrScoreLevel[j,k] = -1;
                        //    }
                        //}
                        //tbl.Columns.Add("步骤", typeof(string));
                        //tbl.Columns.Add("等级", typeof(string));
                        //tbl.Columns.Add("得分", typeof(string));
                        //tbl.Columns.Add("分数区间", typeof(string));
                        break;

                }

            }
            else              //直接读取session中的datatable
            {
                //for (int m = 0; m < DM_Client.tblScore.Rows.Count; m++)
                //{
                //    for (int n = 0; n < DM_Client.tblScore.Columns.Count; n++)
                //    {
                //        string temp = DM_Client.tblScore.Rows[m][n].ToString();
                //        temp = temp;
                //    }
                //}
                tbl = DM_Client.tblScore;
            }
            GridViewScore.DataSource = tbl;
            GridViewScore.DataBind();
        }
        public void SaveOne()
        {
            UMyRecords.stMyPaper PTmpPaper;
            int i;

            if (DM_Client.OldPaperList.Count > 0)
            {
                lock (DM_Client.CsOldPaper)
                {
                    lock (DM_Client.CsSavePaper)
                    {
                        for (i = DM_Client.OldPaperList.Count; i > 0; i--)
                        {
                            PTmpPaper = DM_Client.OldPaperList[0]; //已改分第一张
                            if (PTmpPaper.Status)
                            {
                                DM_Client.OldPaperList.RemoveAt(0);
                                DM_Client.SaveScoreList.Add(PTmpPaper);
                            }
                        }
                    }
                }
            }
            Session["DM_Client"] = DM_Client;
        }
        protected Boolean ShowImage(int OperationType, int ReviewRow = -1) //可选参数
        {
            UMyRecords.stMyPaper PTmpPaper, PAddPaper, PNewPaper;
            int i;
            Boolean ToChange;
            double v;
            UMyRecords.stMyUser PTmpUser;
            int b, a;
            string FileName;

            UMyRecords.stPaperInfo tmp;
            MemoryStream tmpPaper;
            UMyRecords.StExamRefScore PExamRefScore;
            uint t1, t2, t3, t4;
            int RcvResult;
            string Content;

            Boolean flag = false;

            ToChange = false;
            PTmpPaper = new UMyRecords.stMyPaper();
            t1 = GetTickCount();
            ShowTask();
            switch (OperationType)
            {
                case 0: //显示新的试卷——正常试卷
                    //先查看OldPaperList中最后一个元素是否为未改试卷（对于试卷复查后）
                    lock (DM_Client.CsOldPaper)
                    {
                        b = DM_Client.OldPaperList.Count;
                        if (DM_Client.OldPaperList.Count > 0)
                        {
                            PTmpPaper = DM_Client.OldPaperList.Last();
                            if (PTmpPaper.Status == true)
                            {
                                PTmpPaper = new UMyRecords.stMyPaper();
                                flag = true;
                            }
                        }
                        else
                        {
                            PTmpPaper = new UMyRecords.stMyPaper();
                            flag = true;
                        }
                        //从NewPaperList中取出一个元素
                        //OldPaperList中无元素或者其中最后一个元素已给分

                        if (flag)
                        {
                            lock (DM_Client.CsNewPaper)
                            {
                                if (DM_Client.NewPaperList.Count > 0)
                                {
                                    for (i = 0; i < DM_Client.NewPaperList.Count; i++)
                                    {
                                        PTmpPaper = DM_Client.NewPaperList[i];
                                        if (PTmpPaper.Flag)
                                        {
                                            //当前为有效试卷（该结构中保存有试卷）
                                            PTmpPaper.Flag = false; //该试卷未复查
                                            PTmpPaper.Status = false;//该试卷未打分

                                            DM_Client.NewPaperList.RemoveAt(i);
                                            DM_Client.OldPaperList.Add(PTmpPaper);

                                            a = DM_Client.OldPaperList.Count;

                                            if (a == b)
                                            {
                                                Thread.Sleep(50);
                                            }
                              //              DM_Client.MyVars.CurPaperNum++;
                                            //显示已改试卷数
                                            switch (DM_Client.MyRecords.UserInfo.Status)
                                            {
                                                case UConstDefine.Trial:
                                                case UConstDefine.ZhengPing:
                                                    //LabelPaperNum.Text = "本次阅卷数:" + DM_Client.MyVars.CurPaperNum.ToString();
                                                    //UpdatePanelPaperNum.Update();
                                                    //Check.tempstat4 = LabelPaperNum.Text;
                                                    break;
                                                case UConstDefine.YangPing:
                                                    LabelPaperNum.Text = DM_Client.MyRecords.UserInfo.SmpPaperTackled.ToString() + "/" + DM_Client.MyVars.CurPaperNum.ToString();
                                                    UpdatePanelPaperNum.Update();
                                                    Check.tempstat4 = LabelPaperNum.Text;
                                                    if (DM_Client.MyVars.CurPaperNum >= Check.TestMin)
                                                    {

                                                    }
                                                    break;
                                                case UConstDefine.CePing:
                                                    LabelPaperNum.Text = DM_Client.MyRecords.UserInfo.SmpPaperTackled.ToString() + "/" + DM_Client.MyVars.CurPaperNum.ToString();
                                                    UpdatePanelPaperNum.Update();
                                                    Check.tempstat4 = LabelPaperNum.Text;
                                                    if (DM_Client.MyVars.CurPaperNum >= Check.TestMin)
                                                    {

                                                    }
                                                    break;
                                            }
                                            break;
                                        }
                                        else//当前试卷不是有效试卷（该结构中未保存有试题）
                                        {
                                            PTmpPaper = new UMyRecords.stMyPaper();
                                            if (TRcvPaperClass.RcvResult != UConstDefine.RM_RSP_OK && TRcvPaperClass.RcvResult != UConstDefine.RM_ERR_WAIT)
                                            {
                                                RcvResult = TRcvPaperClass.RcvResult;
                                                if (TRcvPaperClass.TRcvPaperThread.ThreadState != ThreadState.Suspended)
                                                {
                                                    TRcvPaperClass.TRcvPaperThread.Suspend();
                                                }
                                                TRcvPaperClass.TRcvPaperThread.Resume();
                                                SaveOne();
                                                switch (RcvResult)
                                                {
                                                    case UConstDefine.RM_ERR_ROLE:
                                                        ScriptManager.RegisterStartupScript(this.ButtonOK, this.ButtonOK.GetType(), "msgAns", "<script>alert('当前没有需要评阅的试卷');</script>", false);
                                                        break;
                                                    case UConstDefine.RM_ERR_USER:
                                                    case UConstDefine.RM_ERR_ERR:
                                                    case UConstDefine.RM_ERR_PAPERTYPE:
                                                    case UConstDefine.RM_ERR_BLOCK:
                                                        ScriptManager.RegisterStartupScript(this.ButtonOK, this.ButtonOK.GetType(), "msgAns", "<script>alert('当前没有需要评阅的试卷');</script>", false);
                                                        break;
                                                    case UConstDefine.RM_ERR_WAIT:
                                                        SaveOne();
                                                        break;
                                                    case UConstDefine.RM_ERR_NOBLK:
                                                    case UConstDefine.RM_ERR_NOPAPER:
                                                        switch (DM_Client.MyRecords.UserInfo.Status)
                                                        {
                                                            case UConstDefine.YangPing:
                                                                ScriptManager.RegisterStartupScript(this.ButtonOK, this.ButtonOK.GetType(), "msgAns", "<script>alert('当前没有可用的样评试题!');</script>", false);
                                                                break;
                                                            case UConstDefine.CePing:
                                                                ScriptManager.RegisterStartupScript(this.ButtonOK, this.ButtonOK.GetType(), "msgAns", "<script>alert('当前没有可用的测评试题!');</script>", false);
                                                                break;
                                                            default:
                                                                if (DM_Client.MyRecords.UserInfo.Role < UConstDefine.XiaoZuZhang)
                                                                {
                                                                    ScriptManager.RegisterStartupScript(this.ButtonOK, this.ButtonOK.GetType(), "msgAns", "<script>alert('本块试题已全部取完，请注销或退出系统!');</script>", false);
                                                                    if (TRcvPaperClass.TRcvPaperThread.ThreadState != ThreadState.Suspended)
                                                                    {
                                                                        TRcvPaperClass.TRcvPaperThread.Suspend();
                                                                    }
                                                                    TRcvPaperClass.TRcvPaperThread.Resume();
                                                                    break; //TODO:
                                                                }
                                                                else
                                                                {
                                                                    Check.PaperCount = 0;
                                                                    ScriptManager.RegisterStartupScript(this.ButtonOK, this.ButtonOK.GetType(), "msgAns", "<script>alert('当前没有可用的仲裁试卷，请稍后再试!');</script>", false);
                                                                    if (DM_Client.OldPaperList.Count > 0)
                                                                    {
                                                                        PAddPaper = DM_Client.OldPaperList.First();
                                                                    }
                                                                    //if (PAddPaper.PaperImage.Length == 0)
                                                                    //{

                                                                    //}

                                                                    break; //TODO:
                                                                }

                                                        }
                                                        break;

                                                }
                                            }
                                            else //取题正常(RcvResult为RM_RSP_OK或RM_ERR_WAIT)
                                            {
                                                ButtonOK.Enabled = true;
                                                if (TRcvPaperClass.TRcvPaperThread.ThreadState != ThreadState.Suspended)
                                                {
                                                    TRcvPaperClass.TRcvPaperThread.Suspend();
                                                }
                                                TRcvPaperClass.TRcvPaperThread.Resume();
                                            }
                                        }
                                        //  TODO:样卷评阅完毕的处理机制
                                        if ((i == DM_Client.NewPaperList.Count - 1) && (Check.LastPaper = true))
                                        {

                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
                case 1: //显示复查试卷
                    lock (DM_Client.CsOldPaper)
                    {
                        if (ReviewRow + 1 <= DM_Client.OldPaperList.Count)
                        {
                            PTmpPaper = DM_Client.OldPaperList[ReviewRow];
                            if (PTmpPaper.Status)
                            {
                                PTmpPaper.Flag = true;
                            }
                        }
                    }
                    break;
                case 2: //异常试卷（问题试卷）---虽然与正常试卷处理过程大致相同，但有一些细节问题，所以还是单独处理
                    lock (DM_Client.CsOldPaper)
                    {
                        flag = false;
                        if (DM_Client.OldPaperList.Count > 0)
                        {
                            PTmpPaper = DM_Client.OldPaperList.Last();
                        }
                        else
                        {
                            flag = true;
                        }
                        lock (DM_Client.CsNextPaperNode)
                        {
                            if (flag == true || PTmpPaper.Status == true)
                            {
                                if (DM_Client.PNextPaper.PaperInfo.PaperNo > 0)
                                {
                                    PTmpPaper = DM_Client.PNextPaper;
                                    DM_Client.OldPaperList.Add(DM_Client.PNextPaper);
                                }
                                else
                                {
                                    GetExceptionPaper(DM_Client, ref PTmpPaper);
                                    if (PTmpPaper.PaperImage.Length != null)
                                    {
                                        DM_Client.OldPaperList.Add(PTmpPaper);
                                    }
                                }
                                t3 = GetTickCount();
                                if (PTmpPaper.PaperInfo.ImageFormat == UConstDefine.JP2)
                                {
                                    GetExceptionPaper(DM_Client, ref DM_Client.PNextPaper);
                                }
                                t4 = GetTickCount();
                            }
                        }
                    }
                    break;
            }
            //初始化并装载试卷图片
            PTmpPaper.PaperImage.Position = 0;
            switch (OperationType)
            {
                case 0:
                case 2:
                    lock (DM_Client.CsNextPaper)
                    {
                        string tmpstr1 = new String(DM_Client.MyRecords.UserInfo.LoginName);
                        tmpstr1 = tmpstr1.Substring(0, tmpstr1.IndexOf('\0'));
                        string picTemp = Server.MapPath(".");
                        if (DM_Client.NextPaper == true && (PTmpPaper.PaperInfo.PaperNo == DM_Client.NextPaperNo))
                        {
                            img.ImageUrl = "~/" + tmpstr1 + "/" + PTmpPaper.PaperInfo.PaperNo.ToString() + ".bmp";
                        }
                        else
                        {
                            if (PTmpPaper.PaperInfo.ImageFormat == UConstDefine.JP2)
                            {
                                UMyFuncs.LoadJP2(DM_Client, PTmpPaper, picTemp); //no matter the format is the FreeImageNET can deals with
                                img.ImageUrl = "~/" + tmpstr1 + "/" + PTmpPaper.PaperInfo.PaperNo.ToString() + ".bmp";
                            }
                            else
                            {
                                UMyFuncs.LoadJP2(DM_Client, PTmpPaper, picTemp);
                                img.ImageUrl = "~/" + tmpstr1 + "/" + PTmpPaper.PaperInfo.PaperNo.ToString() + ".bmp";
                            }
                        }
                    }
                    break;
                case 1: //check
                    lock (DM_Client.CsPreviewPaper)
                    {
                        Boolean exist = false;
                        string tmpstr1 = new String(DM_Client.MyRecords.UserInfo.LoginName);
                        tmpstr1 = tmpstr1.Substring(0, tmpstr1.IndexOf('\0'));
                        if (File.Exists(Server.MapPath("~/" + tmpstr1 + "/" + PTmpPaper.PaperInfo.PaperNo.ToString() + ".bmp")))
                        {
                            exist = true;
                        }
                        if ((PTmpPaper.PaperInfo.ImageFormat == UConstDefine.JP2) && exist)//TODO:
                        {
                            img.ImageUrl = "~/" + tmpstr1 + "/" + PTmpPaper.PaperInfo.PaperNo.ToString() + ".bmp";
                        }
                    }
                    break;
            }
            //TODO: 在图片上写异常原因

            //设置开始时间
            v = ((DateTime.Now - UnitGlobalV.javaTime).TotalDays + DM_Client.sysTimeDis) * 86400 - 8 * 60 * 60; //server

            PTmpPaper.RStartTime = GetTickCount();
            PTmpPaper.StartTime = v;


            //样评处理
            if (DM_Client.MyRecords.UserInfo.Status == UConstDefine.YangPing)
            {
                for (i = 0; i < DM_Client.MyRecords.SampleInfoList.Count; i++)
                {
                    PExamRefScore = DM_Client.MyRecords.SampleInfoList[i];
                    if (PExamRefScore.PaperNo == PTmpPaper.PaperInfo.PaperNo)
                    {
                        DM_Client.MyRecords.SampleInfoList.RemoveAt(i);
                        break;
                    }
                }
            }

            //if ((DM_Client.MyRecords.UserInfo.Interceder > 0) && ((DM_Client.MyRecords.UserInfo.Status == UConstDefine.Trial) || (DM_Client.MyRecords.UserInfo.Status == UConstDefine.ZhengPing)))
            //{
            //    //如果用户没有仲裁权且处于试用或者正评状态下，可以提交异常试卷
            //}
            //显示当前试卷的试卷号
            DM_Client.MyVars.CurPaperID = PTmpPaper.PaperInfo.PaperNo;

            LabelPaperNo.Text = "试卷号:" + DM_Client.MyVars.CurPaperID.ToString();
            UpdatePanelPaperNo.Update();
            Check.tempstat2 = LabelPaperNo.Text;
            PTmpUser = new UMyRecords.stMyUser();
            //对当前试卷，设置存分类型
            switch (PTmpPaper.PaperInfo.PaperType)
            {
                case UConstDefine.PM_PAPERTYPE_CEPING:
                    PTmpPaper.SaveType = UConstDefine.PM_SAVESCORE_CEPING;
                    break;
                case UConstDefine.PM_PAPERTYPE_CONFLICT:
                case UConstDefine.PM_PAPERTYPE_NORMAL: //仲裁试卷、正评试题
                    PTmpPaper.SaveType = UConstDefine.PM_SAVESCORE_NORMAL;
                    BindConflict(PTmpPaper, PTmpUser);
                    break;
                case UConstDefine.PM_PAPERTYPE_EXCEPTION:
                    PTmpPaper.SaveType = UConstDefine.PM_SAVESCORE_YICHANG;
                    BindException(PTmpPaper, PTmpUser);
                    break;
                case UConstDefine.PM_PAPERTYPE_FREESEL://抽调试卷
                    PTmpPaper.SaveType = UConstDefine.PM_SACESCORE_CHECKED; //已核查（零分或者满分）
                    break;
                case UConstDefine.PM_PAPERTYPE_RESAMPLE: //样卷重评题
                    PTmpPaper.SaveType = UConstDefine.PM_SAVESCORE_RESAMPLE;
                    break;
                case UConstDefine.PM_PAPERTYPE_SAMPLE://样卷试题
                    PTmpPaper.SaveType = UConstDefine.PM_SAVESCORE_SAMPLE;
                    ////说明样卷尚未改完
                    //Check.SampleOver = false;
                    break;
                case UConstDefine.PM_PAPERTYPE_SELF:
                    PTmpPaper.SaveType = UConstDefine.PM_SAVESCORE_SELF;
                    break;
                case UConstDefine.PM_PAPERTYPE_TRIAL:
                    PTmpPaper.SaveType = -1;
                    break;
            }

            if (OperationType != 1)
            {
                DM_Client.OldPaperList[DM_Client.OldPaperList.Count - 1] = PTmpPaper; // 不是引用 重新复制
            }
            else
            {
                DM_Client.OldPaperList[ReviewRow] = PTmpPaper;
            }

            //已改试卷数超过了设定的缓冲值，将OldPaperList队首元素移出->SaveScoreList
            lock (DM_Client.CsOldPaper)
            {
                if (DM_Client.OldPaperList.Count - 1 > DM_Client.MyRecords.CurBlockInfo.BlockInfo.BufSize)
                {
                    PAddPaper = DM_Client.OldPaperList.First();
                    if (PAddPaper.Status)//当前试卷已经打分
                    {
                        lock (DM_Client.CsSavePaper)
                        {
                            DM_Client.OldPaperList.RemoveAt(0);
                            DM_Client.SaveScoreList.Add(PAddPaper);
                        }
                    }
                    else//当前试卷尚未打分，直接放入JunkList中
                    {
                        lock (DM_Client.JunkList)
                        {
                            DM_Client.OldPaperList.RemoveAt(0);
                            DM_Client.JunkList.Add(PAddPaper);
                        }
                    }
                }
            }
            RefreshShow(1);
            TPreFetchClass.FetchType = 1;
           // TPreFetchClass.index = -1;
            lock (DM_Client.CsNewPaper)
            {
                if (OperationType == 0)
                {
                    for (i = 0; i < DM_Client.NewPaperList.Count; i++)
                    {
                        PNewPaper = DM_Client.NewPaperList[i];
                        if (PNewPaper.Flag)
                        {
                            DM_Client.PNextPaper = PNewPaper;
                            break;
                        }
                    }
                }
            }
            if (TPreFetchClass.TPreFetchThread.ThreadState == ThreadState.Unstarted)
                TPreFetchClass.TPreFetchThread.Start();

            if (TPreFetchClass.TPreFetchThread.ThreadState == ThreadState.Suspended)
            {
                TPreFetchClass.TPreFetchThread.Resume();
            }
            Session["Check"] = Check;
            Session["DM_Client"] = DM_Client;
            Session["TPreFetchClass"] = TPreFetchClass;
            return true;
        }
        protected void LinkButtonSelect_Click(object sender, EventArgs e)
        {
            int i, j, k;
            UMyRecords.stMyPaper PMyPaper;
            DataTable tbl;
            LinkButton b = (LinkButton)sender;
            GridViewRow row = (GridViewRow)b.Parent.Parent;
            int index = row.RowIndex;    //确定点击的行

            //if (DM_Client.OldPaperList.Count > 0)       //找复查试卷对应哪个结构体
            //{
            //    k = 0;
            //    for (i = 0; i < DM_Client.OldPaperList.Count; i++)
            //    {
            //        PMyPaper = DM_Client.OldPaperList[i];
            //        if (PMyPaper.Status)                 //复查列表寻找
            //        {
            //            if (PMyPaper.PaperInfo.PaperNo == DM_Client.OldPaperList[index].PaperInfo.PaperNo)  //找到试卷
            //            {
            //                j = k;       //赋值并退出循环
            //                DM_Client.stCheck[j].PaperNo = PMyPaper.PaperInfo.PaperNo;
            //                break;
            //            }
            //            k++;             //试卷号不符合，复查列表下一个
            //        }
            //    }
            //}
            tbl = DM_Client.tblScore.Copy();
            for (i = 0; i < DM_Client.tblScore.Rows.Count; i++)            //把分数绑定
            {
                tbl.Rows[i][1] = DM_Client.stCheck[index].DetailScore[i];
            }
            GridViewScore.DataSource = tbl;
            GridViewScore.DataBind();
            ShowImage(1, index);
            Check.status_review = true;
            LabelName.Text = "试卷复查";
            UpdateLabelName.Update();
            Session["Check"] = Check;
        }
        private void ShowTask()
        {
            string strCount;
            if (DM_Client.MyVars.CurPaperNum < 0)
                DM_Client.MyVars.CurPaperNum = 0;
            strCount = Check.PaperCount.ToString();
            if (Check.PaperCount < 0)
                strCount = "未知";
            if(RadioButtonList1.SelectedIndex == 0)
            {
                LabelPaperNum.Text = "待处理仲裁卷数:" + strCount + "/本次已阅:" + DM_Client.MyVars.CurPaperNum.ToString();
            }
            else if (RadioButtonList1.SelectedIndex == 1)
            {
                LabelPaperNum.Text = "待处理问题卷数:" + strCount + "/本次已阅:" + DM_Client.MyVars.CurPaperNum.ToString();
            }
            UpdatePanelPaperNum.Update();
        }
        public void RefreshShow(int Operation)
        {
            UMyRecords.stMyPaper PTmpPaper;
            int i, j, k;
            UMyRecords.stQueryResult PTmpResult;
            string Content;

            switch (Operation)
            {
                case 1:
                    DataTable tbl = new DataTable("CheckScore");
                    tbl.Columns.Add("序号", typeof(string));
                    tbl.Columns.Add("试卷号", typeof(string));
                    tbl.Columns.Add("分数", typeof(string));

                    Content = "复查区被清空，准备进入临界区CsOldPaper";
                    UMyFuncs.WriteLogFile(DM_Client, Content);
                    lock (DM_Client.CsOldPaper)
                    {
                        j = 1;
                        Content = "RefreshShow进入临界区CsOldPaper成功";
                        UMyFuncs.WriteLogFile(DM_Client, Content);
                        if (DM_Client.OldPaperList.Count > 0)
                        {
                            for (i = 0; i < DM_Client.OldPaperList.Count; i++)
                            {
                                PTmpPaper = DM_Client.OldPaperList[i];
                                if (PTmpPaper.Status)
                                {
                                    tbl.Rows.Add(j.ToString(), PTmpPaper.PaperInfo.PaperNo.ToString(), PTmpPaper.TotalScore.ToString());
                                    Content = "找到了可以复查的试卷" + PTmpPaper.PaperInfo.PaperNo.ToString();
                                    UMyFuncs.WriteLogFile(DM_Client, Content);
                                    j++;
                                }
                            }
                        }
                        for (k = j - 1; k < DM_Client.MyRecords.CurBlockInfo.BlockInfo.BufSize; k++)
                        {
                            tbl.Rows.Add(j.ToString(), "", "");
                            j++;
                        }
                    }
                    GridViewCheck.DataSource = tbl;
                    GridViewCheck.DataBind();
                    UpdatePanelCheck.Update();
                    break;
            }
        }
        protected void BindConflict(UMyRecords.stMyPaper PTmpPaper, UMyRecords.stMyUser PTmpUser)
        {
            DataTable conflicttbl = new DataTable("conflicttbl");
            conflicttbl.Columns.Add("阅卷人");
            conflicttbl.Columns.Add("分数");
            conflicttbl.Columns.Add("详细分数");
            conflicttbl.Columns.Add("异常原因");
            if (PTmpPaper.RefScore.UserID1 > 0)
            {
                lock (DM_Client.CsGrpUser)
                {
                    if (DM_Client.MyRecords.GrpUserList.Count > 0)
                    {
                        for (int i = 0; i < DM_Client.MyRecords.GrpUserList.Count; i++)
                        {
                            PTmpUser = DM_Client.MyRecords.GrpUserList[i];
                            if (PTmpUser.UserID == PTmpPaper.RefScore.UserID1)
                            {
                                string tmpstr1 = new String(DM_Client.MyRecords.UserInfo.LoginName);
                                tmpstr1 = tmpstr1.Substring(0, tmpstr1.IndexOf('\0'));
                                string tmpstr2 = new String(PTmpUser.UserTrueName);
                                tmpstr2 = tmpstr2.Substring(0, tmpstr2.IndexOf('\0'));
                                string tmpstr3 = new String(PTmpPaper.RefScore.DetailScore1);
                                tmpstr3 = tmpstr3.Substring(0, tmpstr3.IndexOf('\0'));
                                conflicttbl.Rows.Add(tmpstr1 + "(" + tmpstr2 + ")", PTmpPaper.RefScore.TotalScore1.ToString(), tmpstr3, "");
                            }
                        }
                    }
                }
            }
            if (PTmpPaper.RefScore.UserID2 > 0)
            {
                lock (DM_Client.CsGrpUser)
                {
                    if (DM_Client.MyRecords.GrpUserList.Count > 0)
                    {
                        for (int i = 0; i < DM_Client.MyRecords.GrpUserList.Count; i++)
                        {
                            PTmpUser = DM_Client.MyRecords.GrpUserList[i];
                            if (PTmpUser.UserID == PTmpPaper.RefScore.UserID2)
                            {
                                string tmpstr1 = new String(DM_Client.MyRecords.UserInfo.LoginName);
                                tmpstr1 = tmpstr1.Substring(0, tmpstr1.IndexOf('\0'));
                                string tmpstr2 = new String(PTmpUser.UserTrueName);
                                tmpstr2 = tmpstr2.Substring(0, tmpstr2.IndexOf('\0'));
                                string tmpstr3 = new String(PTmpPaper.RefScore.DetailScore2);
                                tmpstr3 = tmpstr3.Substring(0, tmpstr3.IndexOf('\0'));
                                conflicttbl.Rows.Add(tmpstr1 + "(" + tmpstr2 + ")", PTmpPaper.RefScore.TotalScore2.ToString(), tmpstr3, "");
                            }
                        }
                    }
                }
            }
            if (PTmpPaper.RefScore.UserID3 > 0)
            {
                lock (DM_Client.CsGrpUser)
                {
                    if (DM_Client.MyRecords.GrpUserList.Count > 0)
                    {
                        for (int i = 0; i < DM_Client.MyRecords.GrpUserList.Count; i++)
                        {
                            PTmpUser = DM_Client.MyRecords.GrpUserList[i];
                            if (PTmpUser.UserID == PTmpPaper.RefScore.UserID3)
                            {
                                string tmpstr1 = new String(DM_Client.MyRecords.UserInfo.LoginName);
                                tmpstr1 = tmpstr1.Substring(0, tmpstr1.IndexOf('\0'));
                                string tmpstr2 = new String(PTmpUser.UserTrueName);
                                tmpstr2 = tmpstr2.Substring(0, tmpstr2.IndexOf('\0'));
                                string tmpstr3 = new String(PTmpPaper.RefScore.DetailScore3);
                                tmpstr3 = tmpstr3.Substring(0, tmpstr3.IndexOf('\0'));
                                conflicttbl.Rows.Add(tmpstr1 + "(" + tmpstr2 + ")", PTmpPaper.RefScore.TotalScore3.ToString(), tmpstr3, "");
                            }
                        }
                    }
                }
            }
            if (PTmpPaper.RefScore.UserID4 > 0)
            {
                lock (DM_Client.CsGrpUser)
                {
                    if (DM_Client.MyRecords.GrpUserList.Count > 0)
                    {
                        for (int i = 0; i < DM_Client.MyRecords.GrpUserList.Count; i++)
                        {
                            PTmpUser = DM_Client.MyRecords.GrpUserList[i];
                            if (PTmpUser.UserID == PTmpPaper.RefScore.UserID4)
                            {
                                string tmpstr1 = new String(DM_Client.MyRecords.UserInfo.LoginName);
                                tmpstr1 = tmpstr1.Substring(0, tmpstr1.IndexOf('\0'));
                                string tmpstr2 = new String(PTmpUser.UserTrueName);
                                tmpstr2 = tmpstr2.Substring(0, tmpstr2.IndexOf('\0'));
                                string tmpstr3 = new String(PTmpPaper.RefScore.DetailScore4);
                                tmpstr3 = tmpstr3.Substring(0, tmpstr3.IndexOf('\0'));
                                conflicttbl.Rows.Add(tmpstr1 + "(" + tmpstr2 + ")", PTmpPaper.RefScore.TotalScore4.ToString(), tmpstr3, "");
                            }
                        }
                    }
                }
            }
            if (PTmpPaper.RefScore.UserID5 > 0)
            {
                lock (DM_Client.CsGrpUser)
                {
                    if (DM_Client.MyRecords.GrpUserList.Count > 0)
                    {
                        for (int i = 0; i < DM_Client.MyRecords.GrpUserList.Count; i++)
                        {
                            PTmpUser = DM_Client.MyRecords.GrpUserList[i];
                            if (PTmpUser.UserID == PTmpPaper.RefScore.UserID5)
                            {
                                string tmpstr1 = new String(DM_Client.MyRecords.UserInfo.LoginName);
                                tmpstr1 = tmpstr1.Substring(0, tmpstr1.IndexOf('\0'));
                                string tmpstr2 = new String(PTmpUser.UserTrueName);
                                tmpstr2 = tmpstr2.Substring(0, tmpstr2.IndexOf('\0'));
                                string tmpstr3 = new String(PTmpPaper.RefScore.DetailScore5);
                                tmpstr3 = tmpstr3.Substring(0, tmpstr3.IndexOf('\0'));
                                conflicttbl.Rows.Add(tmpstr1 + "(" + tmpstr2 + ")", PTmpPaper.RefScore.TotalScore5.ToString(), tmpstr3, "");
                            }
                        }
                    }
                }
            }
            GridViewDetail.DataSource = conflicttbl;
            GridViewDetail.DataBind();
        }
        protected void BindException(UMyRecords.stMyPaper PTmpPaper, UMyRecords.stMyUser PTmpUser)
        {
            DataTable exceptiontbl = new DataTable("exceptiontbl");
            exceptiontbl.Columns.Add("阅卷人");
            exceptiontbl.Columns.Add("分数");
            exceptiontbl.Columns.Add("详细分数"); 
            exceptiontbl.Columns.Add("异常原因");
            lock (DM_Client.CsGrpUser)
            {
                if (DM_Client.MyRecords.GrpUserList.Count > 0)
                {
                    for (int i = 0; i < DM_Client.MyRecords.GrpUserList.Count; i++)
                    {
                        PTmpUser = DM_Client.MyRecords.GrpUserList[i];
                        if (PTmpUser.UserID == PTmpPaper.ScoreInfo.UserIDOrPaperSeq)
                        {
                            string tmpstr1 = new String(DM_Client.MyRecords.UserInfo.LoginName);
                            tmpstr1 = tmpstr1.Substring(0, tmpstr1.IndexOf('\0'));
                            string tmpstr2 = new String(PTmpUser.UserTrueName);
                            tmpstr2 = tmpstr2.Substring(0, tmpstr2.IndexOf('\0'));
                            string tmpstr3 = new String(PTmpPaper.ScoreInfo.Txt);
                            tmpstr3 = tmpstr3.Substring(0, tmpstr3.IndexOf('\0'));
                            if (PTmpPaper.ScoreInfo.ExcpReason == UConstDefine.SelfExcpCode)
                            {
                                exceptiontbl.Rows.Add(tmpstr1 + "(" + tmpstr2 + ")", "", "", tmpstr3);
                                break;
                            }
                            else
                            {
                                for (int j = 0; j < 30; j++)
                                {
                                    if (PTmpPaper.ScoreInfo.ExcpReason == DM_Client.MyRecords.ArrRsnCode[j])
                                    {
                                        exceptiontbl.Rows.Add(tmpstr1 + "(" + tmpstr2 + ")", "", "", Select.Items[j + 1].Text);
                                        break; //TODO:
                                    }
                                }
                            }
                        }
                    }
                }
            }
            GridViewDetail.DataSource = exceptiontbl;
            GridViewDetail.DataBind();
        }
        protected void RadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TRcvPaperClass.ToContinue)
            {
                TRcvPaperClass.ToContinue = false;
            }
            if (!TSaveScoreClass.ToContinue)
            {
                TRcvPaperClass.ToContinue = true;
            }
            ClearNewPaperList();
            ClearOldPaperList();
            while (DM_Client.SaveScoreList.Count > 0)
                Thread.Sleep(50);
            RefreshShow(1);
            if (RadioButtonList1.SelectedIndex == 0)
            {
                Select.SelectedIndex = 0;
                Select.Enabled = false;

                ButtonOK.Enabled = true;
                GridViewDetail.Columns[1].Visible = true;
                GridViewDetail.Columns[2].Visible = true;
                GridViewDetail.Columns[3].Visible = false;

                Labelzhongcai.Text = "仲裁试卷区";

                LabelName.Text = "评阅仲裁试卷";
                UpdateLabelName.Update();

                DataTable conflicttbl = new DataTable("conflicttbl");
                conflicttbl.Columns.Add("阅卷人");
                conflicttbl.Columns.Add("分数");
                conflicttbl.Columns.Add("详细分数");
                conflicttbl.Rows.Add("", "", "");
                GridViewDetail.DataSource = conflicttbl;
                GridViewDetail.DataBind();
                UpdatePanelType.Update();
            }
            else if (RadioButtonList1.SelectedIndex == 1)
            {
                Select.Enabled = true;
                ButtonOK.Enabled = true;
                GridViewDetail.Columns[1].Visible = false;
                GridViewDetail.Columns[2].Visible = false;
                GridViewDetail.Columns[3].Visible = true;

                Labelzhongcai.Text = "问题试卷区";

                LabelName.Text = "评阅问题试卷";
                UpdateLabelName.Update();
                Check.ExcpRsn = UConstDefine.PM_GETYCPAPER_ALL;
                DataTable exceptiontbl = new DataTable("exceptiontbl");
                exceptiontbl.Columns.Add("阅卷人");
                exceptiontbl.Columns.Add("异常原因");
                exceptiontbl.Rows.Add("", "");
                GridViewDetail.DataSource = exceptiontbl;
                GridViewDetail.DataBind();
                UpdatePanelType.Update();
            }
            Session["DM_Client"] = DM_Client;
            Session["Check"] = Check;
        }

        private void ClearNewPaperList()
        {
            UMyRecords.stMyPaper PTmpPaper;
            int i;
            lock (DM_Client.CsNewPaper)
            {
                if (DM_Client.NewPaperList.Count > 0)
                {
                    for (i = 0; i < DM_Client.NewPaperList.Count; i++)
                    {
                        PTmpPaper = DM_Client.NewPaperList[i];
                        PTmpPaper.PaperInfo.PaperNo = 0;
                        PTmpPaper.PaperInfo.PaperType = 0;
                        PTmpPaper.SaveType = 0;
                        PTmpPaper.TotalScore = -1;
                        PTmpPaper.DetailScore = "";
                        PTmpPaper.Flag = false;
                        PTmpPaper.Status = false;
                        DM_Client.NewPaperList[i] = PTmpPaper;
                    }
                }
            }
        }

        private void ClearOldPaperList()
        {
            UMyRecords.stMyPaper PTmpPaper;
            lock (DM_Client.CsOldPaper)
            {
                if (DM_Client.OldPaperList.Count > 0)
                {
                    lock (DM_Client.CsSavePaper)
                    {
                        while (DM_Client.OldPaperList.Count > 0)
                        {
                            PTmpPaper = DM_Client.OldPaperList.First();
                            DM_Client.OldPaperList.RemoveAt(0);
                            if (PTmpPaper.Status && PTmpPaper.PaperInfo.PaperNo > 0)
                            {
                                DM_Client.SaveScoreList.Add(PTmpPaper);
                            }
                            else
                            {
                                PTmpPaper.PaperInfo.PaperNo = 0;
                                PTmpPaper.PaperImage.Flush();
                                DM_Client.JunkList.Add(PTmpPaper);
                            }
                        }

                        lock (DM_Client.CsJunk)
                        {
                            if (DM_Client.JunkList.Count == 0)
                            {
                                PTmpPaper = new UMyRecords.stMyPaper();
                                PTmpPaper.PaperImage = new MemoryStream();
                                DM_Client.JunkList.Add(PTmpPaper);

                            }
                            PTmpPaper = DM_Client.JunkList.First();
                            DM_Client.JunkList.Remove(PTmpPaper);

                            PTmpPaper.PaperInfo.PaperNo = 0;
                            PTmpPaper.TotalScore = -1;
                            PTmpPaper.PaperInfo.PaperType = -1;
                            PTmpPaper.Status = true;
                            PTmpPaper.SaveType = UConstDefine.PM_SAVESCORE_RECYCLE;
                            PTmpPaper.PaperInfo.VolumeName = DM_Client.MyVars.CurVolName.PadRight(32,'\0').ToCharArray();

                            DM_Client.SaveScoreList.Add(PTmpPaper);

                        }
                    }
                }
            }
        }
        protected void GetExceptionPaper(TDM_Client DM_Client, ref UMyRecords.stMyPaper PTmpPaper)
        {
            int i, nTmp;
      //      PTmpPaper = new UMyRecords.stMyPaper();
            lock (DM_Client.CsJunk)
            {
                if (DM_Client.JunkList.Count == 0)
                {
                    PTmpPaper.PaperImage = new MemoryStream();
                    DM_Client.JunkList.Add(PTmpPaper);
                }
                PTmpPaper = DM_Client.JunkList.First();

                //GetExcpPaper中的ExcpRsn是由TF_Check.Btn_TypeOKClick函数定义的
                nTmp = UMyFuncs.GetExcpPaper(DM_Client, ref PTmpPaper, Check.ExcpRsn, DM_Client.MyVars.CurVolName);
                if (nTmp != UConstDefine.RM_RSP_OK)
                {
                    switch (nTmp)
                    {
                        case UConstDefine.RM_ERR_WAIT:
                            i = 0;
                            PTmpPaper = DM_Client.JunkList.First();
                            while (nTmp == UConstDefine.RM_ERR_WAIT && i < 20)
                            {
                                Thread.Sleep(50);
                                nTmp = UMyFuncs.GetExcpPaper(DM_Client, ref PTmpPaper, Check.ExcpRsn, DM_Client.MyVars.CurVolName);
                                i++;
                            }
                            if (nTmp != UConstDefine.RM_RSP_OK)
                            {
                                switch (nTmp)                              //TODO:tobeadded
                                {
                                    case UConstDefine.RM_ERR_WAIT:        //取题中
                                        break;
                                    case UConstDefine.RM_ERR_NOPAPER:
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                if (PTmpPaper.Flag)
                                {
                                    DM_Client.JunkList.RemoveAt(0);
                                    PTmpPaper.Flag = false;
                                    PTmpPaper.Status = false;
                                }
                            }
                            break;
                        case UConstDefine.RM_ERR_NOPAPER:
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    if (PTmpPaper.Flag)
                    {
                        DM_Client.JunkList.Remove(PTmpPaper);
                        PTmpPaper.Flag = false;
                        PTmpPaper.Status = false;
                    }
                }
            }
        }

        protected void ButtonOK_Click(object sender, EventArgs e)
        {
            ButtonOK.Enabled = false;
            ButtonUp.Enabled = false;
            DM_Client.MyVars.CurPaperNum = 0;
            if (DM_Client.MyRecords.UserInfo.Role > UConstDefine.PuTong)
            {

            }
            Check.PaperCount = GetPaperCount();
            if (Check.PaperCount == -1)
            {
                ScriptManager.RegisterStartupScript(this.ButtonOK, this.ButtonOK.GetType(), "msgAns", "<script>alert('获取试卷数目失败');</script>", false);
                return;
            }
            if (RadioButtonList1.SelectedIndex == 0)
            {
                TRcvPaperClass.RcvResult = 0;
                TRcvPaperClass.ToContinue = true;
                TRcvPaperClass.TRcvPaperThread.Start();

                Thread.Sleep(100);

                if (!ShowImage(0))
                {
                    ButtonOK.Enabled = true;
                }
                LabelName.Text = "评阅仲裁试卷";
                UpdateLabelName.Update();
            }
            if (RadioButtonList1.SelectedIndex == 1)
            {
                Select.Enabled = true;

                if (!ShowImage(2))
                {
                    ButtonOK.Enabled = false;
                }
                LabelName.Text = "评阅问题试卷";
                UpdateLabelName.Update();
            }
            RadioButtonList1.Enabled = true;
            //TODO:定时刷新阅卷任务信息

        }

        private int GetPaperCount()
        {
            UMsgDefine.FM_GETPAPERCOUNT_REQ fmGetPaperCountReq;
            UMsgDefine.FM_GETPAPERCOUNT_RSP fmGetPaperCountRsp;

            fmGetPaperCountReq.MsgHead.MsgType = UConstDefine.TM_GETPAPERCOUNT_REQ;
            fmGetPaperCountReq.MsgHead.MsgLength = Marshal.SizeOf(typeof(UMsgDefine.FM_GETPAPERCOUNT_REQ));
            fmGetPaperCountReq.UserID = DM_Client.MyRecords.UserInfo.UserID;
            fmGetPaperCountReq.PaperType = RadioButtonList1.SelectedIndex;
            fmGetPaperCountReq.VolName = "".PadRight(32, '\0').ToCharArray();

            fmGetPaperCountReq.VolName = DM_Client.MyVars.CurVolName.PadRight(32, '\0').ToCharArray();
            lock (DM_Client.CsSocket)
            {
                byte[] Message = UMsgDefine.StructToBytes(fmGetPaperCountReq);
                DM_Client.TCPSocket.Send(Message, fmGetPaperCountReq.MsgHead.MsgLength, SocketFlags.None);

                byte[] RcvMessage = new byte[Marshal.SizeOf(typeof(UMsgDefine.FM_GETPAPERCOUNT_RSP))];
                DM_Client.TCPSocket.Receive(RcvMessage, Marshal.SizeOf(typeof(UMsgDefine.FM_GETPAPERCOUNT_RSP)), SocketFlags.None);
                fmGetPaperCountRsp = (UMsgDefine.FM_GETPAPERCOUNT_RSP)UMsgDefine.BytesToStruct(RcvMessage, typeof(UMsgDefine.FM_GETPAPERCOUNT_RSP));
                if (fmGetPaperCountRsp.MsgHead.MsgType == UConstDefine.TM_GETPAPERCOUNT_RSP)
                {
                    if (fmGetPaperCountRsp.RspCode == UConstDefine.RM_RSP_OK)
                    {
                        return fmGetPaperCountRsp.PaperCount;
                    }
                    else
                    {
                        return -1;//获取试卷数量失败
                    }
                }
            }
            return -1;
        }

        protected void ButtonUp_Click(object sender, EventArgs e)
        {
            UMyRecords.stMyPaper PTmpPaper;
            int i;
            UMsgDefine.FM_SaveScore_Req fmDelayReq;
            UMsgDefine.FM_SaveScore_Rsp fmDelayRsp;
            double d;
            Boolean RepOK = false;
            if (DM_Client.MyVars.CurPaperID > 0)
            {
                PTmpPaper = new UMyRecords.stMyPaper();
                lock (DM_Client.CsOldPaper)
                {
                    if (DM_Client.OldPaperList.Count > 0)
                    {
                        PTmpPaper = new UMyRecords.stMyPaper();
                        for (i = DM_Client.OldPaperList.Count - 1; i >= 0; i--)
                        {
                            PTmpPaper = DM_Client.OldPaperList[i];
                            if (PTmpPaper.PaperInfo.PaperNo == DM_Client.MyVars.CurPaperID)
                            {
                                DM_Client.OldPaperList.Remove(PTmpPaper);
                                break;
                            }
                            else
                            {
                                PTmpPaper = new UMyRecords.stMyPaper();
                            }
                        }
                    }
                }
                if (PTmpPaper.PaperImage.Length == 0)
                {
                    fmDelayReq.Score = new UMyRecords.stSaveScore[1];
                    fmDelayReq.MsgHead.MsgType = UConstDefine.TM_SAVESCORE_REQ;
                    fmDelayReq.MsgHead.MsgLength = Marshal.SizeOf(typeof(UMsgDefine.FM_SaveScore_Req));
                    fmDelayReq.RecCount = 1;
                    fmDelayReq.Score[0].UserID = DM_Client.MyRecords.UserInfo.UserID;
                    fmDelayReq.Score[0].PaperNo = DM_Client.MyVars.CurPaperID;
                    fmDelayReq.Score[0].VolumeName = PTmpPaper.PaperInfo.VolumeName;
                    fmDelayReq.Score[0].ScoreType = UConstDefine.PM_SAVESCORE_DELAY;
                    d = ((DateTime.Now - UnitGlobalV.javaTime).TotalDays + DM_Client.sysTimeDis) * 86400 - 8 * 60 * 60;
                    fmDelayReq.Score[0].TimeStamp = (uint)Math.Round(d);

                    lock (DM_Client.CsSocket)
                    {
                        byte[] Message = UMsgDefine.StructToBytes(fmDelayReq);
                        DM_Client.TCPSocket.Send(Message, fmDelayReq.MsgHead.MsgLength, SocketFlags.None);

                        byte[] RcvMessage = new byte[Marshal.SizeOf(typeof(UMsgDefine.FM_SaveScore_Rsp))];
                        DM_Client.TCPSocket.Receive(RcvMessage, Marshal.SizeOf(typeof(UMsgDefine.FM_SaveScore_Rsp)), SocketFlags.None);
                        fmDelayRsp = (UMsgDefine.FM_SaveScore_Rsp)UMsgDefine.BytesToStruct(RcvMessage, typeof(UMsgDefine.FM_SaveScore_Rsp));

                        if (fmDelayRsp.MsgHead.MsgType == UConstDefine.TM_SAVESCORE_RSP)
                        {
                            if (fmDelayRsp.RspCode == UConstDefine.RM_RSP_OK)
                            {
                                lock (DM_Client.CsJunk)
                                {
                                    PTmpPaper.PaperImage.Flush();
                                    PTmpPaper.TotalScore = -100;
                                    PTmpPaper.DetailScore = "";
                                    PTmpPaper.PaperInfo.PaperNo = 0;
                                    PTmpPaper.Status = false;
                                    PTmpPaper.Flag = false;
                                    DM_Client.JunkList.Add(PTmpPaper);
                                }
                            }
                            else
                            {
                                lock (DM_Client.CsOldPaper)
                                {
                                    DM_Client.OldPaperList.Add(PTmpPaper);
                                    ScriptManager.RegisterStartupScript(this.UpdatePanel_submit, this.UpdatePanel_submit.GetType(), "msg", "<script>alert('发送延后再判请求时发生错误');</script>", false);
                                }
                            }

                        }
                        else
                        {
                            lock (DM_Client.CsOldPaper)
                            {
                                DM_Client.OldPaperList.Add(PTmpPaper);
                                ScriptManager.RegisterStartupScript(this.UpdatePanel_submit, this.UpdatePanel_submit.GetType(), "msg", "<script>alert('收到错误的延后再判响应帧');</script>", false);
                            }
                        }
                    }
                    if (RepOK)
                    {
                        DM_Client.MyVars.CurPaperNum++;
                        if (RadioButtonList1.SelectedIndex == 0)
                        {
                            if (Check.status_review == false)
                                ShowImage(0);
                        }
                        else if (RadioButtonList1.SelectedIndex == 1)
                        {
                            if (Check.status_review == false)
                                ShowImage(2);
                        }
                        RefreshShow(1);
                    }
                }
            }
        }

        protected void ButtonSubmit_Click(object sender, EventArgs e)
        {
            if (this.Page.IsValid)
            {
                Check.ScoreInputOK = CheckScore();
                if (Check.ScoreInputOK)
                {
                    AfterGiveScore(Check.gDetailScore, Check.gTotalScore);

                }

            }
            else
            {

            }
        }
        public bool CheckScore()
        {
            //需要重新检查分数吗?

            //整个大题分数输入完后，根据已给分数为gDetailScore和gTotalScore赋值，并清空分数输入框
            int i, j, rate;
            Single Max, min, Score, CanGiveScore, ParamScore;
            Boolean ScoreIsVaild;
            int Code, MinusStart;
            int p;
            Boolean MinusFlag;
            string ScoreString, FullScoreString;
            Check.gDetailScore = "";
            Check.gTotalScore = 0;
            MinusStart = -1;
            p = 0;
            //给分普通情况，不考虑等级
            i = 1;
            foreach (GridViewRow gvrow in GridViewScore.Rows)
            {
                if (i == DM_Client.MyVars.PositiveEndRow + 1)
                {
                    i++;
                    continue;
                }
                ScoreString = Check.ScoreInput[i - 1];
                FullScoreString = DM_Client.tblScore.Rows[i - 1][2].ToString();
                Score = float.Parse(ScoreString);
                p = Check.gDetailScore.IndexOf('{');
                if (FullScoreString.IndexOf('-') >= 0)
                {
                    MinusFlag = true;
                }
                else
                {
                    MinusFlag = false;
                }
                if (!MinusFlag)
                {
                    Check.gDetailScore = Check.gDetailScore + ScoreString;
                    if (i < GridViewScore.Rows.Count)
                    {
                        Check.gDetailScore = Check.gDetailScore + ",";
                    }
                }
                else
                {
                    if (MinusStart < 0)
                    {
                        MinusStart = i;
                    }
                    if (p < 0) //如果还没有到负分给分点
                    {
                        Check.gDetailScore = Check.gDetailScore + "{";
                    }
                    Check.gDetailScore = Check.gDetailScore + ScoreString;
                    if (i < GridViewScore.Rows.Count)
                    {
                        Check.gDetailScore = Check.gDetailScore + "|";
                    }
                    else
                    {
                        Check.gDetailScore = Check.gDetailScore + "}";
                    }
                }
                Check.gTotalScore = Check.gTotalScore + Score;
                i++;
            }
            return true;
        }
        public void AfterGiveScore(string gDetailScore, float gTotalScore)
        {
            UMyRecords.stMyPaper PTmp = new UMyRecords.stMyPaper();
            int i = 0;
            double v, v1, v2;
            Boolean ImageFlag;
            int h1, h2;
            Boolean flag = false;
            lock (DM_Client.CsOldPaper)
            {
                if (DM_Client.OldPaperList.Count > 0)
                {
                    for (i = DM_Client.OldPaperList.Count - 1; i >= 0; i--)
                    {
                        PTmp = DM_Client.OldPaperList[i];
                        if (PTmp.PaperInfo.PaperNo == DM_Client.MyVars.CurPaperID)
                        {
                            flag = true;
                            break;
                        }
                        else
                        {
                            flag = false;
                        }
                    }
                }
                if (flag)  //在OldPaperList中找到了当前评阅试卷
                {
                    //填写给分时间，阅卷时长=给分时间-试卷显示时间
                    v = ((DateTime.Now - UnitGlobalV.javaTime).TotalDays + DM_Client.sysTimeDis) * 86400 - 8 * 60 * 60;
                    PTmp.TimeStamp = v;
                    PTmp.REndTime = GetTickCount();
                    //TODO:待删除
                    DateTime s = new DateTime(1899, 12, 30);
                    s.AddSeconds(v);
                    UMyFuncs.WriteLogFile(DM_Client, s.ToString("yyyyMMdd-HH:mm:ss"));
                    //
                    v1 = PTmp.RStartTime;
                    v2 = PTmp.TimeStamp;

                    //根据个人质量信息v1 v2判断是否给趋中分数和阅卷速度过快


                    //对于正在评阅的正常试卷，检查是否符合速度限制要求

                    //保存已有分数
                    PTmp.TotalScore = gTotalScore;
                    PTmp.DetailScore = gDetailScore;
                    PTmp.Status = true; //已给分 

                    DM_Client.OldPaperList[i] = PTmp; //saveback

                    if (PTmp.PaperInfo.ImageFormat == UConstDefine.JP2 && (!Check.status_review))
                    {
                        SavePicture(PTmp.PaperInfo.PaperNo);//保存当前试卷图片到已改试卷图片数组
                    }
                    if (DM_Client.MyRecords.UserInfo.Status == UConstDefine.YangPing)
                    {

                    }
                    else
                    {
                        RefreshShow(1);//显示更新复查试卷
                    }
                }

            }

            if (Check.status_review == true)
            {
                ExitReview();
            }
            if (DM_Client.MyRecords.UserInfo.Role == UConstDefine.PuTong)
                ShowImage(0);
            else
            {
                if (RadioButtonList1.SelectedIndex == 0)
                {
                    if (!Check.status_review)
                    {
                        DM_Client.MyVars.CurPaperNum++;
                    }
                    ImageFlag = ShowImage(0);
                    if (!Check.status_review)
                    {
                        //画套红框
                    }
                }
                else if (RadioButtonList1.SelectedIndex == 1)
                {
                    if (!Check.status_review)
                    {
                        DM_Client.MyVars.CurPaperNum++;
                    }
                    ImageFlag = ShowImage(2);
                    if (!Check.status_review)
                    {
                        //画套红框
                    }
                }
            }
            Session["DM_Client"] = DM_Client; //只需要保存这个
        }
        public void ExitReview()
        {
            LabelName.Text = "正常阅卷状态";
            UpdateLabelName.Update();
            Check.status_review = false;
            Session["Check"] = Check;
        }
        public void SavePicture(int PaperNo)  //保存已改试卷，处理多余试卷
        {
            int i;
            Boolean flag = false;
            lock (DM_Client.CsPreviewPaper)
            {
                for (i = 0; i < DM_Client.MyRecords.CurBlockInfo.BlockInfo.BufSize; i++)
                {
                    if (Check.previewpaper[i] == -1)
                    {
                        Check.previewpaper[i] = PaperNo;
                        flag = true;
                        break;
                    }
                }
                if (flag == false)      //存满，替换最早一张
                {
                    string tmpstr1 = new String(DM_Client.MyRecords.UserInfo.LoginName);
                    tmpstr1 = tmpstr1.Substring(0, tmpstr1.IndexOf('\0'));
                    if (File.Exists(Server.MapPath("~/" + tmpstr1 + "/" + Check.previewpaper[0].ToString() + ".bmp")))
                    {
                        File.Delete(Server.MapPath("~/" + tmpstr1 + "/" + Check.previewpaper[0].ToString() + ".bmp"));
                    }
                    for (i = 0; i < DM_Client.MyRecords.CurBlockInfo.BlockInfo.BufSize - 1; i++)
                    {
                        Check.previewpaper[i] = Check.previewpaper[i + 1];
                    }
                    Check.previewpaper[i] = PaperNo;
                }
            }
            Session["Check"] = Check;
        }
        protected void CustomValidator1_ServerValidate(object source, ServerValidateEventArgs args) //没什么需要保存状态的
        {
            Single score;            // 记录当前老师所给分
            Boolean ScoreIsValid;   // 当前得分点分数输入是否有效
            int Rate;           //分等级或者分项分等级给分题中所给等级数
            int curRow;         //记录焦点所在行
            int curCol;         //记录焦点所在列
            Single stateFull;
            string point, tmps, pattern;
            Single CanGiveScore;
            Single Min;
            Single Max;
            int strinpos;
            int i;
            string fullscore;
            Boolean MinusFlag; //是否是给负分
            Boolean HalfFlag;//是否允许给0.5分
            Boolean match = false;
            int Code;
            if (getPostBackControlName() != "ButtonSubmit")
            {
                return;
            }
            MinusFlag = false;
            ScoreIsValid = false;
            Check.ScoreInputOK = false;
            //这个函数会对每个验证控件起作用
            CustomValidator cv = (CustomValidator)source;
            GridViewRow dvr = (GridViewRow)cv.Parent.Parent;
            i = dvr.RowIndex; //验证到第几行
            if (DM_Client.tblScore.Rows[i][3].ToString() == "Y")
                HalfFlag = true;
            else
                HalfFlag = false;
            if (DM_Client.tblScore.Rows[i][2].ToString().IndexOf('-') >= 0)
            {
                MinusFlag = true;
            }
            point = Check.ScoreInput[i]; //获取分数字符串
            fullscore = DM_Client.tblScore.Rows[i][2].ToString();
            switch (DM_Client.MyVars.QuestionType)
            {
                case 3: //测试库里只有任意给分题
                    //用正则表达式，输入只能为正负整数，或者正负一位小数
                    if (HalfFlag == false && MinusFlag == false)  //只能为正整数
                    {
                        pattern = @"^(0|[1-9]\d*)$"; //非负
                        match = Regex.IsMatch(point, pattern);
                    }
                    if (HalfFlag == true && MinusFlag == false) //正整数或一位小数
                    {
                        pattern = @"^(0(\.5)?|[1-9]\d*(\.5)?)$";
                        match = Regex.IsMatch(point, pattern);
                    }
                    if (HalfFlag == false && MinusFlag == true)
                    {
                        pattern = @"^(0|-?[1-9]\d*)$";  //整数
                        match = Regex.IsMatch(point, pattern);
                    }
                    if (HalfFlag == true && MinusFlag == true)
                    {
                        pattern = @"^(-?[1-9][0-9]*(\.[1-9])?|0(\.5)?)$"; //整数或一位小数
                        match = Regex.IsMatch(point, pattern);
                    }
                    if (!match)
                    {
                        DM_Client.stCheck = (DataModule.stCheck[])UMsgDefine.Clone(Check.stCheckTemp);
                        ScriptManager.RegisterStartupScript(this.UpdatePanel_submit, this.UpdatePanel_submit.GetType(), "msg", "<script>alert('输入分数不合要求');</script>", false);
                        args.IsValid = false;
                    }
                    else //比较满分
                    {
                        if (Math.Abs(float.Parse(point)) > Math.Abs(float.Parse(fullscore)))
                        {
                            DM_Client.stCheck = (DataModule.stCheck[])UMsgDefine.Clone(Check.stCheckTemp);
                            ScriptManager.RegisterStartupScript(this.UpdatePanel_submit, this.UpdatePanel_submit.GetType(), "msg", "<script>alert('输入分数超过满分');</script>", false);
                            args.IsValid = false;
                        }
                        else
                        {
                            args.IsValid = true;
                        }
                    }
                    break;
            }
        }
        /// <summary>
        /// 获取引发Postback的控件名
        /// </summary>
        /// <returns></returns>
        private string getPostBackControlName()
        {
            Control control = null;
            string ctrlname = Page.Request.Params["__EVENTTARGET"];
            if (ctrlname != null && ctrlname != String.Empty)
            {
                control = Page.FindControl(ctrlname);
            }
            else
            {
                Control c;
                foreach (string ctl in Page.Request.Form)
                {
                    if (ctl.EndsWith(".x") || ctl.EndsWith(".y"))
                    {
                        c = Page.FindControl(ctl.Substring(0, ctl.Length - 2));
                    }
                    else
                    {
                        c = Page.FindControl(ctl);
                    }
                    if (c is System.Web.UI.WebControls.Button ||
                             c is System.Web.UI.WebControls.ImageButton)
                    {
                        control = c;
                        break;
                    }
                }
            }
            if (control != null)
                return control.ID;
            else
                return string.Empty;
        }

        protected void GridViewCheck_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridViewRow gvr = e.Row;
            if (gvr.RowType == DataControlRowType.DataRow)
            {
                if (IsBlankRow(gvr))
                {
                    int nCount = gvr.Cells.Count;
                    for (int i = 0; i < nCount; i++)
                    {
                        TableCell tc = gvr.Cells[i];
                        foreach (Control c in tc.Controls)
                        {
                            DisableCtrl(c);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 判断是不是空行
        /// </summary>
        /// <returns></returns>
        private static bool IsBlankRow(GridViewRow gvr)
        {
            int nCount = gvr.Cells.Count;
            for (int i = 1; i < nCount - 1; i++)
            {
                string strText = gvr.Cells[i].Text;
                if (strText != "" && strText != "&nbsp;")
                    return false;
            }
            return true;
        }
        private static void DisableCtrl(Control c)
        {
            if (c is WebControl)
            {
                WebControl wc = (WebControl)c;
                if (wc is LinkButton)
                    wc.Enabled = false;
            }

        }

        protected void Buttonzhuxiao_Click(object sender, EventArgs e)
        {
            UMsgDefine.FM_UserLogout fmLogout;
            int i;
            UMyRecords.stMyPaper PTmpPaper;

            string tmpstr1 = new String(DM_Client.MyRecords.UserInfo.LoginName);
            tmpstr1 = tmpstr1.Substring(0, tmpstr1.IndexOf('\0'));
            if (tmpstr1.Length != 0 && System.IO.Directory.Exists(Server.MapPath(".") + "/" + tmpstr1))
            {
                System.IO.Directory.Delete(Server.MapPath(".") + "/" + tmpstr1, true);
            }
            if (System.IO.Directory.Exists(Server.MapPath(".") + "/" + tmpstr1 + "DaAn"))
            {
                System.IO.Directory.Delete(Server.MapPath(".") + "/" + tmpstr1 + "DaAn", true);
            }
            if (System.IO.Directory.Exists(Server.MapPath(".") + "/" + tmpstr1 + "XiZe"))
            {
                System.IO.Directory.Delete(Server.MapPath(".") + "/" + tmpstr1 + "XiZe", true);
            }
            if (System.IO.Directory.Exists(Server.MapPath(".") + "/" + tmpstr1 + "BiaoZhun"))
            {
                System.IO.Directory.Delete(Server.MapPath(".") + "/" + tmpstr1 + "BiaoZhun", true);
            }
            if (System.IO.Directory.Exists("C:\\" + tmpstr1 + "mdb"))
            {
                System.IO.Directory.Delete("C:\\" + tmpstr1 + "mdb", true);
            }
            if (DM_Client.OldPaperList.Count > 0)
            {
                SaveOne();//将现有的分数存回服务器
            }
            while (DM_Client.SaveScoreList.Count != 0)
            {
                Thread.Sleep(3000);
            }
            if (DM_Client.MyRecords.ThdStatus.UDP)
            {
                TUDPClass.TUDPListenedThread.Abort();
            }
            fmLogout.MsgHead.MsgType = UConstDefine.TM_USER_LOGOUT;
            fmLogout.MsgHead.MsgLength = Marshal.SizeOf(typeof(UMsgDefine.FM_UserLogout));
            fmLogout.UserID = DM_Client.MyRecords.UserInfo.UserID;
            byte[] buf = UMsgDefine.StructToBytes(fmLogout);
            DM_Client.TCPSocket.Send(buf, buf.Length, SocketFlags.None);

            if (DM_Client.MyRecords.ThdStatus.Beacon)
            {
                TBeaconClass.TBeaconThread.Abort();
            }
            if (DM_Client.MyRecords.ThdStatus.Rcv)
            {
                TRcvPaperClass.TRcvPaperThread.Abort();
            }
            if (DM_Client.MyRecords.ThdStatus.Save)
            {
                TSaveScoreClass.TSaveScoreThread.Abort();
            }

            UMyFuncs.ClearUpInfo(DM_Client, 1);
            UMyFuncs.ClearUpInfo(DM_Client, 2);

            DM_Client.NewPaperList.Clear();
            DM_Client.SaveScoreList.Clear();
            DM_Client.OldPaperList.Clear();
            DM_Client.JunkList.Clear();
            DM_Client.StatusList.Clear();

            DM_Client.UDPSocket.Close();
            DM_Client.TCPSocket.Shutdown(SocketShutdown.Both);
            Thread.Sleep(10);
            DM_Client.TCPSocket.Close();
            DM_Client.MyRecords.ArrSocket.Close();
            Response.Redirect("~/Login.aspx");
        }
        protected void Select_SelectedIndexChanged(object sender, EventArgs e)
        {
            lock (DM_Client.CsExcpRsn)
            {
                Check.ExcpRsn = UConstDefine.PM_GETYCPAPER_ALL;
                if (Select.SelectedIndex == Select.Items.Count - 1)
                {
                    Check.ExcpRsn = UConstDefine.SelfExcpCode;
                }
                else
                {
                    Check.ExcpRsn = DM_Client.MyRecords.ArrRsnCode[Select.SelectedIndex - 1];
                }

            }
            Session["Check"] = Check;
        }
    }
}