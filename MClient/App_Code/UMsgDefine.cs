using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace MClient.App_Code
{   
    /// <summary>
    /// 定义各种通信帧结构以及通用结构（组用户信息结构、质量控制文件单元节点结构等）及全局变量
    /// </summary>
    public class UMsgDefine
    {
        /// <summary>
        /// 帧头定义
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct stMsgHead
        {
            public int MsgType;
            public int MsgLength;
        }
        /// <summary>
        /// 各类通信帧定义
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_QueryUser_Req //探测帧
        {
            public stMsgHead MsgHead;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.NameLen)]
            public char[] UserName;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_QueryUser_Rsp  //响应帧
        {
            public stMsgHead MsgHead;
            public int RspCode;
            public int RecCount;
        }
        /// <summary>
        /// 认证登录类各帧
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_Auth_Req  //请求帧
        {
            public stMsgHead MsgHead;
            public int AuthType;
            public int UDPPort;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.NameLen)]
            public char[] UserName;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.PwdLen)]
            public char[] UserPwd;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_Auth_Rsp     //响应帧
        {
            public stMsgHead MsgHead;
            public int RspCode;
            public uint ServerTime;
        }
        /// <summary>
        /// 用户注销
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_UserLogout
        {
            public stMsgHead MsgHead;
            public Int64 UserID;
        }
        /// <summary>
        /// 更改密码
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_ChgPwd_Req
        {
            public stMsgHead MsgHead;
            public Int64 UserID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.PwdLen)]
            public char[] OldPwd;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.PwdLen)]
            public char[] NewPwd;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UConstDefine.NameLen)] //注意有汉字，必须采用ansi字符串与客户端相同
            public string TrueName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UConstDefine.ServeForLen)]
            public string ServeFor;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_ChgPwd_Rsp
        {
            public stMsgHead MsgHead;
            public int RspCode;
        }
        /// <summary>
        /// 阅卷周边流程类各帧
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GetBlkInfo_Req //试题信息获取
        {
            public stMsgHead MsgHead;
            public int InfoType;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.VolLen)]
            public char[] VolumeName;  //用户负责的卷名
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GetBlkInfo_Rsp_Login
        {
            public stMsgHead MsgHead;
            public int RspCode;
            public UMyRecords.stBlockInfoLogin BlockInfo;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GetBlkInfo_Rsp
        {
            public stMsgHead MsgHead;
            public int RspCode;
            public UMyRecords.stBlockInfo BlockInfo;
        }
        //考场字典请求响应帧
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GETCLASSROOMMAP_REQ
        {
            public stMsgHead MsgHead;
            public Int64 UserID;
        }
        
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GETCLASSROOMMAP_RSP
        {
            public stMsgHead MsgHead;
            public int RspCode;
            public int ClsMapLen;
        }
        //档次字典请求响应帧
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GETRATEMAP_REQ
        {
            public stMsgHead MsgHead;
            public Int64 UserID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.VolLen)]
            public char[] VolName;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GETRATEMAP_RSP
        {
            public stMsgHead MsgHead;
            public int RspCode;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
            public char[] RateMap;
        }
        /// <summary>
        /// 样卷相关操作 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_EXAMSAVESCORE_REQ   //专家存分请求响应帧
        {
            public stMsgHead MsgHead;
            public UMyRecords.stEXAMSAVESCORE Score; //最大32个试题缓冲区
            public int AdoptedID;  //分数被采纳的题组长ID
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_EXAMSAVESCORE_RSP
        {
            public stMsgHead MsgHead;
            public int RspCode;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_SUBMITSTANDARD_REQ //提交参考答案请求帧
        {
            public stMsgHead MsgHead;
            public Int64 UserID;
            public int PaperNo;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.VolLen)]
            public char[] VolName;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_SUBMITSTANDARD_RSP
        {
            public stMsgHead MsgHead;
            public int RspCode;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GETSAMBLKINFO_REQ //获取样卷题块信息请求帧
        {
            public stMsgHead MsgHead;
            public Int64 UserID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.VolLen)]
            public char[] VolumeName;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GETSAMBLKINFO_RSP
        {
            public stMsgHead MsgHead;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.BLKNUM)]
            public UMyRecords.stBlockInfo[] BLKINFO;
            public int RspCode;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_DELETESAM_REQ  //删除样卷请求帧
        {
            public stMsgHead MsgHead;
            public Int64 UserID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.VolLen)]
            public char[] VolumeName;
            public int PaperNo;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_DELETESAM_RSP
        {
            public stMsgHead MsgHead;
            public int RspCode;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_MODIFYSAM_REQ  //修改样卷请求帧
        {
            public stMsgHead MsgHead;
            public Int64 UserID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.VolLen)]
            public char[] VolumeName;
            public int PaperNo;
            public int Rate;  //专家所给档次
            public Single TotalScore;   //专家所给总分
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public char[] Reason;  //专家所给理由
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_MODIFYSAM_RSP
        {
            public stMsgHead MsgHead;
            public int RspCode;
        }
        //获取样卷分组信息请求帧
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GETSAMGROUPINFO_REQ
        {
            public stMsgHead MsgHead;
            public Int64 UserID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.VolLen)]
            public char[] VolumeName;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GETSAMGROUPINFO_RSP  
        {
            public stMsgHead MsgHead;
            public int RspCode;
            public int SamGroupNum;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.SAMGROUPNO)]
            public UMyRecords.stSamGroupInfo[] AllSamGroupInfo;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_SAMGROUP_REQ  //增加,修改样卷分组请求帧
        {
            public stMsgHead MsgHead;
            public Int64 UserID;
            public UMyRecords.stSamGroupInfo TmpSamGroupInfo;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.SAMGROUPNAMELEN)]
            public char[] OldSamGroupName;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_SAMGROUP_RSP
        {
            public stMsgHead MsgHead;
            public int RspCode;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_DELETESAMGROUP_REQ  //删除样卷分组请求帧
        {
            public stMsgHead MsgHead;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.SAMGROUPNAMELEN)]
            public char[] SamGroupName;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.VolLen)]
            public char[] VolumeName;
            public Int64 UserID;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_DELETESAMGROUP_RSP
        {
            public stMsgHead MsgHead;
            public int RspCode;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_ACTIVESAMGROUP_REQ  //激活样卷分组请求帧
        {
            public stMsgHead MsgHead;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.SAMGROUPNAMELEN)]
            public char[] SamGroupName;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.VolLen)]
            public char[] VolumeName;
            public int IsActive;
            public Int64 UserID;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_ACTIVESAMGROUP_RSP
        {
            public stMsgHead MsgHead;
            public int RspCode;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GETSAMGROUPPAPER_REQ  //获取分组样卷信息请求帧
        {
            public stMsgHead MsgHead;
            public Int64 UserID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.SAMGROUPNAMELEN)]
            public char[] SamGroupName;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.VolLen)]
            public char[] VolumeName;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GETSAMGROUPPAPER_RSP
        {
            public stMsgHead MsgHead;
            public int RspCode;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.SAMGROUPNAMELEN)]
            public char[] GroupName;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.VolLen)]
            public char[] VolumeName;

            public int SamGroupPaperNum;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.SamPaperNoLen)]
            public int[] SamGroupPaperNo;

            public int NoSamGroupPaperNum;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_SAVESAMGROUPPAPER_REQ  //保存样卷分组中样卷新信息请求帧
        {
            public stMsgHead MsgHead;
            public Int64 UserID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.SAMGROUPNAMELEN)]
            public char[] GroupName;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.VolLen)]
            public char[] VolumeName;
            public int AddSamGroupPaperNum;
            public int DelSamGroupPaperNum;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.SamPaperLen)]
            public int[] SamGroupPaper;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FM_SAVESAMGROUPPAPER_RSP
        {
            public stMsgHead MsgHead;
            public int RspCode;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GETSAMTOTAL_REQ  //获取样卷库信息请求帧
        {
            public stMsgHead MsgHead;
            public Int64 UserID;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GETSAMTOTAL_RSP
        {
            public stMsgHead MsgHead;
            public int RspCode;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.SamNum)]
            public int[] SamPaperNo;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GETSAMGRPNAME_REQ  //获取所有样卷分组名称请求帧
        {
            public stMsgHead MsgHead;
            public Int64 UserID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.VolLen)]
            public char[] VolumeName;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GETSAMGRPNAME_RSP
        {
            public stMsgHead MsgHead;
            public int RspCode;
            public int Count;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.ALLSAMGROUPNAMELEN)]
            public char[] SamGrpName;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GETSAMGRPPAPERNO_REQ  //获取分组所有样卷号的请求帧
        {
            public stMsgHead MsgHead;
            public Int64 UserID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.VolLen)]
            public char[] VolumeName;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.SAMGROUPNAMELEN)]
            public char[] GroupName;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GETSAMGRPPAPER_RSP
        {
            public stMsgHead MsgHead;
            public int RspCode;
            public int DataLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.SamPaperLen)]
            public int[] PaperNo;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_CHGSTAT_REQ  //状态转换请求帧
        {
            public stMsgHead MsgHead;
            public Int64 UserID;
            public int ReqType;
            public int TopTeam;
            public int MidTeam;
            public int BtmTeam;
            public int TargetID;
            public int OldStatus;
            public int NewStatus;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_CHGSTAT_RSP
        {
            public stMsgHead MsgHead;
            public int RspCode;
        }
        /// <summary>
        /// 取题存分类各帧
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GetPaper_Req  //取题请求帧
        {
            public stMsgHead MsgHead;
            public UMyRecords.stGetPaperTask GetPaperTask;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GetPaper_Rsp
        {
            public stMsgHead MsgHead;
            public int RspCode;
            public UMyRecords.stPaperInfo PaperData;
        }
        //请求帧1——针对非异常试题
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_SaveScore_Req
        {
            public stMsgHead MsgHead;
            public int RecCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
            public UMyRecords.stSaveScore[] Score;
        }
        [StructLayout(LayoutKind.Sequential)]
        //请求帧2——针对异常试题
        public struct FM_SaveExcp_Req
        {
            public stMsgHead MsgHead;
            public int RecCount;
            public UMyRecords.stSaveExcp Score;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_SaveScore_Rsp
        {
            public stMsgHead MsgHead;
            public int RspCode;
        }
        //提取雷同卷任务请求帧
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GetSMTask_Req
        {
            public stMsgHead MsgHead;
            public Int64 UserID;
            public int Role;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GetSMTask_Rsp
        {
            public stMsgHead MsgHead;
            public int RspCode;
            public int RecCount;
        }
        //雷同卷取题请求帧
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GetSimilarPaper_Req
        {
            public stMsgHead MsgHead;
            public Int64 UserID;
            public int Role;
            public int ClassRoomNo;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GetSimilarPaper_Rsp
        {
            public stMsgHead MsgHead;
            public int RspCode;
            public UMyRecords.stPaperInfo PaperInfo;
        }
        [StructLayout(LayoutKind.Sequential)]
        //雷同卷结果保存
        public struct FM_SaveSMPaper_Req
        {
            public stMsgHead MsgHead;
            public Int64 UserID;
            public int Role;
            public int ClassRoomNo;
            public int StringLen;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.BufferSize - 20)]
            public char[] StudentIDGrp;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_SaveSMPaper_Rsp
        {
            public stMsgHead MsgHead;
            public int RspCode;
        }
        /// <summary>
        /// 异常试卷处理类
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GetExcpPaper_Req  //异常试卷提取 
        {
            public stMsgHead MsgHead;
            public Int64 UserID;
            public int Role;
            public int BlockNo;
            public int Reason;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GetExcpPaper_Rsp
        {
            public stMsgHead MsgHead;
            public int RspCode;
            public UMyRecords.stExampleRec ExcpRec;
            public UMyRecords.stPaperInfo PaperInfo;
        }
        /// <summary>
        /// 质量控制类各帧
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GetGrpInfo_Req  //获取组用户信息
        {
            public stMsgHead MsgHead;
            public Int64 UserID;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GetGrpInfo_Rsp
        {
            public stMsgHead MsgHead;
            public int RspCode;
            public int BufLen;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GetQtyFile_Req //获取质量控制文件
        {
            public stMsgHead MsgHead;
            public Int64 UserID;
            public int FileType;
            public int Num;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GetQtyFile_Rsp
        {
            public stMsgHead MsgHead;
            public int RspCode;
            public int FileFormat;
            public int count;
            public int FileLen;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_QtyStat_Req  ////统计查询提交帧
        {
            public stMsgHead MsgHead;
            public int QueryType;
            public int LeaderID;
            public Int64 UserID;
            public int Num;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_QtyStat_Rsp
        {
            public stMsgHead MsgHead;
            public int RspCode;
            public int RecCount;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_SETRATIO_REQ  //重评率设置请求帧
        {
            public stMsgHead MsgHead;
            public Int64 UserID;
            public int RegType;
            public int RationType;
            public int TargetID;
            public int TopTeam;
            public int MidTeam;
            public int BtmTeam;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
            public char[] RationDate;
            public Single RejudgeRation;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.SAMGROUPNAMELEN)]
            public char[] SamGrpName;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_SetRatio_Rsp
        {
            public stMsgHead MsgHead;
            public int RspCode;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_SelectPaper_Req  //抽调试卷请求帧
        {
            public stMsgHead MsgHead;
            public Int64 UserID;
            public int Role;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.VolLen)]
            public char[] VolumeName;
            public int PaperNo;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_SelectPaper_Rsp
        {
            public stMsgHead MsgHead;
            public int RspCode;
            public UMyRecords.stPaperInfo PaperInfo;
            public UMyRecords.stRefScore RefScore;
        }
        /// <summary>
        /// 定时信标
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_Beacon_Client  //客户端发送的信标帧
        {
            public stMsgHead MsgHead;
            public int nFlag;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_Beacon_Server
        {
            public stMsgHead MsgHead;
            public uint ServerTime;
            public int Status;
            public int LogOut;
        }
        /// <summary>
        /// 广播类各帧
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_Beacon_Server_Assignment
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2000)]
            public char[] Assignment;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_ProveSrv_Req
        {
            public stMsgHead MsgHead;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_ProveSrv_Rsp
        {
            public stMsgHead MsgHead;
            public Int32 IPAddr;
            public int UserCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.SrvInfoLen)]
            public char[] ServerInfo;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_Broadcase_Srv //服务端发送的广播
        {
            public stMsgHead MsgHead;
            public UMyRecords.stBcMsgData BcMsgData;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_UDPMsg_Srv //服务端发送的单播消息帧
        {
            public stMsgHead MsgHead;
            public UMyRecords.stUcMsgData UcMsgData;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_UDPMsg_ACK //客户端的UDP消息ACK帧 ,服务器的UDP消息ACK帧也用此结构。
        {
            public stMsgHead MsgHead;
            public int MsgID;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_UDPMsg_IM //发送即时通信帧
        {
            public stMsgHead MsgHead;
            public int msgID;
            public int RecvNum;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
            public char[] msgData;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GetBlkInfoYc_Req //获取异常试卷块信息请求帧
        {
            public stMsgHead MsgHead;
            public Int64 UserID;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GetBlkInfoYcData_Req //获取异常试卷块信息数据请求帧
        {
            public stMsgHead MsgHead;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GetBlkInfoYc_Rsp //获取异常试卷块信息响应帧
        {
            public stMsgHead MsgHead;
            public int RspCode;
            public int BlkCount;
            public int nLen;
            public int JunkInfo;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GetYcBook_Req //获取异常试题本信息请求帧
        {
            public stMsgHead MsgHead;
            public int TopLevelTeam;
            public int BookNum;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GetYcBook_Rsp
        {
            public stMsgHead MsgHead;
            public int RspCode;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
            public char[] StuIdList;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_SaveScoreYc_Req //异常试题存分帧
        {
            public stMsgHead MsgHead;
            public UMyRecords.stSaveYcScore Score;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_SaveScoreYc_Rsp
        {
            public stMsgHead MsgHead;
            public int RspCode;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_SaveScoreYcDZ_Req //大组长处理异常试题存分帧
        {
            public stMsgHead MsgHead;
            public Boolean Ret;
            public UMyRecords.stSaveYcScore Score;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_SaveScoreYcDZ_Rsp
        {
            public stMsgHead MsgHead;
            public int RspCode;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GETSTANDARD_REQ //获取评分细则、参考答案请求响应帧
        {
            public stMsgHead MsgHead;
            public int RequestType;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.VolLen)]
            public char[] VolName;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GETSTANDARD_RSP
        {
            public stMsgHead MsgHead;
            public int RspCode;
            public int StandardLen;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GETEXCPStuINFO_REQ
        {
            public stMsgHead MsgHead;
            public Int64 UserID; //操作类型0表示提醒服务器回收自己的异常卷，1表示请求获得异常卷
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GETEXCPStuINFO_RSP
        {
            public stMsgHead MsgHead;
            public int RspCode;
            public int StuCount;
            public int StuIDLen;
            public int nLen; //字符串长度 为配合中间层LPSTR的定义 占位用
        }
        ///  add 2013-3-7
        ///  增加大组长根据某个题块试卷号查看全卷功能
        ///  根据题块试卷号获取试卷的学生ID和各题块的试题信息
        ///  添加请求帧和响应帧 1209--1228
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GETEXCPTINFOBYPAPERNO_REQ
        {
            public stMsgHead MsgHead;
            public Int64 UserID;
            public int BlkNo;
            public int PaperNo;
        }
        ///// RM_ERR_NOBLOCK表示有部分题块没有启动，
        ///RM_ERR_USER表示用户名错误，
        ///RM_RSP_OK表示正确，RM_ERR_PAPER表示没有与请求相对应试卷号的学生ID，
        ///RM_ERR_FAIL表操作失败，RM_RSP_PICKED表示已经被其他大组长取走，
        ///RM_RSP_SOLVED表示此学生的试卷已经被处理了
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GETEXCPTINFOBYPAPERNO_RSP
        {
            public stMsgHead MsgHead;
            public int RspCode;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public char[] StuID;
            public int ExcpPaperCnt;
            public int nLen; 
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GETEXPAPER_REQ //获取异常登分试卷的请求帧及响应帧
        {
            public stMsgHead MsgHead;
            public Int64 UserID;
            public int VolumeName;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public char[] StuID;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GETEXPAPER_RSP
        {
            public stMsgHead MsgHead;
            public int RspCode;
            public UMyRecords.stPaperInfo PaperData;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct getBlockInfo_Req
        {
            public stMsgHead MsgHead;
            public Int64 UserID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.VolLen)]
            public char[] VolumeName; //题块名称(以卷名形式给出);
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct getBlockInfo_Rsp
        {
            public stMsgHead MsgHead;
            public int RspCode;
            public UMyRecords.stBlockRateInfo paperRateInfo;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GETEXAMQTY_REQ
        {
            public stMsgHead MsgHead;
            public Int64 UserID;
            public int QuestionNo;
            public int BlkNo;
            public int Num;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GETEXAMQTY_RSP
        {
            public stMsgHead MsgHead;
            public int RspCode;
            public int FileFormat;
            public int FileLen;
        }
        //取某个题块全部样卷号 
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GETSAMPAPERNO_REQ
        {
            public stMsgHead MsgHead;
            public Int64 UserID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.VolLen)]
            public char[] VolName;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.SAMGROUPNAMELEN)]
            public char[] GroupName;
        }
        //普通用户正评时获取指定题块所有样卷分组下的样卷号
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GETSGPAPERNO_REQ
        {
            public stMsgHead MsgHead;
            public Int64 UserID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.VolLen)]
            public char[] VolName;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GETSGPAPERNO_RSP
        {
            public stMsgHead MsgHead;
            public int RspCode;
            public int DataLen;
            public int GroupNum;
        }
        //取某个题块某样卷成绩统计的通信帧
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GETSAMSCORESTATS_REQ
        {
            public stMsgHead MsgHead;
            public Int64 UserID;
            public int PaperNo;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.VolLen)]
            public char[] VolName;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GETSAMSCORESTATS_RSP
        {
            public stMsgHead MsgHead;
            public int RspCode;
            public int len;
        }
        //大组长确认样卷制作完成后发送此帧
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_MAKESAMPAPEROVER_REQ
        {
            public stMsgHead MsgHead;
            public Int64 UserID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.VolLen)]
            public char[] VolName;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_MAKESAMPAPEROVER_RSP
        {
            public stMsgHead MsgHead;
            public int RspCode;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct stxzstat
        {
            public uint time;
            public int total;
//            public List<> detaillist;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct sttzstat
        {
            public uint time;
            public int total;
     //     public list<> detaillist;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct stsampstatscore
        {
            public Single score;
            public Single tmpscore;
     //     public list<> tsampstatscorelist;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct stpyping
        {
            public Single score;
            public Single tmpscore;
  //          public List<> tsampstatscorelist;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GETSAMSCOREBYGROUP_REQ
        {
            public stMsgHead MsgHead;
            public Int64 UserID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.VolLen)]
            public char[] VolName;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.SAMGROUPNAMELEN)]
            public char[] GroupName;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GETSAMSCOREBYGROUP_RSP
        {
            public stMsgHead MsgHead;
            public Int64 RspCode;
            public int DataLen;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct stChkPaperCondition
        {
            public int PaperType; //试卷类型0表示正评试卷1表示测评试卷目前只支持正评试卷
            public int TopNo; //大组号
            public int MidNo; //题组号
            public Int64 TeacherID; //教师ID 0表示所有教师
            public int PaperNo; //试卷号 -1表示所有满足条件的试卷
            public int timeCondition;//表示时间条件，0表示抽取所有时间的试卷，1表示按下面两个参数设置的时间进行查询
            public uint Starttime; //起始时间
            public uint EndTime; //结束时间
            //试卷分数形如(分数类型，起始分数，结束分数)分数类型为0表示一评分数
            //分数类型为1表示二评分数 7表示最终分数，8表示仲裁分数
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
            public char[] Score;
            //两次给分差值形如(分数类型1，分数类型2，起始分数，结束分数)分数类型定义同上
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public char[] DiffScore;
            public int maxPaperNo; //本地收到的最大试卷号
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GETFINISHEDPAPERINFO_REQ
        {
            public stMsgHead MsgHead;
            public Int64 UserID;
            public stChkPaperCondition ChkPaperCondition;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GETFINISHEDPAPERINFO_RSP
        {
            public stMsgHead MsgHead;
            public int RspCode;
            public int nNEV; //该题块是几评题
            public int AvailableRecCount; //本次接收完成后客户端还可以获取的记录数
            public int GetCount;//本次服务器发送的记录条数
        }
        //获取需要处理的试卷数请求帧
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GETPAPERCOUNT_REQ
        {
            public stMsgHead MsgHead;
            public Int64 UserID;
            public int PaperType;  //0代表仲裁卷1代表问题卷
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.VolLen)]
            public char[] VolName;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FM_GETPAPERCOUNT_RSP
        {
            public stMsgHead MsgHead;
            public int RspCode;
            public int PaperCount; //需要处理的试卷数量
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct stDetailScore
        {
            public Int64 userid;
            public Single totalscore;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UConstDefine.DetailLen)]
            public char[] detailscore;
        }

        public static byte[] StructToBytes(object obj)
        {
            //得到结构体的大小
            int size = Marshal.SizeOf(obj);
            //创建byte数组
            byte[] bytes = new byte[size];
            //分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(obj, structPtr, false);
            //从内存空间拷到byte数组
            Marshal.Copy(structPtr, bytes, 0, size);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回byte数组
            return bytes;
        }
        public static object BytesToStruct(byte[] bytes, Type type)
        {
            //得到结构的大小
            int size = Marshal.SizeOf(type);
            //    Log(size.ToString(), 1);
            //byte数组长度小于结构的大小
            if (size > bytes.Length)
            {
                //返回空
                return null;
            }
            //分配结构大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将byte数组拷到分配好的内存空间
            Marshal.Copy(bytes, 0, structPtr, size);
            //将内存空间转换为目标结构
            object obj = Marshal.PtrToStructure(structPtr, type);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回结构
            return obj;
        }
        public static T Clone<T>(T RealObject)
        {
            using (Stream objectStream = new MemoryStream())
            {
                //利用 System.Runtime.Serialization序列化与反序列化完成引用对象的复制  
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, RealObject);
                objectStream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(objectStream);
            }
        }    
    }
}