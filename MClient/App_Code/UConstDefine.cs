using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MClient.App_Code
{
    public class UConstDefine
    {
        /// <summary>
    /// 统一定义程序中使用的各种常量数值
    /// </summary>

        //以下各长度均比服务端小1 
        public const int BufferSize = 8191; //8KB
        public const int ServeForLen = 64;  //工作单位最大长度
        public const int NameLen = 16; //用户名长度
        public const int PwdLen = 12; //密码长度
        public const int ExcpLen = 64; //自定义异常原因长度
        public const int IpLen = 15;  //IP地址字符串长度
        public const int SrvInfoLen = 32; //服务器名长度
        public const int VolLen = 32; //卷名长度
        public const int DetailLen = 64;//详细分数长度
        public const int ReasonLen = 255; //输入样卷理由最大长度
        public const int BlockCount = 32;//题块的最大个数
        // 将值增大用于样卷分组
        public const int SamPaperLen = 128 * 2;//一次通信获取的样卷试卷号的最大数目
        public const int SamPaperNoLen = 100;

        public const int PaperCount = 5;//用在TRcvPaperThread.Execute中,NewPaperList节点个数
        public const int SavePaperCount = 3;//用在TSaveScoreThread.Execute中，批量存分，一次存分的个数

        public const int SAMSOCRELEN = 256;
        public const int GrantLen = 10;//权限字符串长度
        public const int SAMGROUPNAMELEN = 64;//样卷分组名称长度
        public const int TRAINTARGETLEN = 128;//样卷分组培训目标程度
        public const int SAMGROUPNO = 34;//样卷分组号长度

        public const int AddSamGroupPaperNum = 32;//添加到分组的样卷试卷号数组长度
        public const int DeleteSamGroupPaperNum = 32;//从分组中删除的样卷试卷号数组长度
        public const int SamNum = 32;//获取样卷库时样卷试卷号数组长度
        public const int BLKNUM = 10;// 获取样卷题块信息时题块信息数组长度
        public const int ALLSAMGROUPNAMELEN = 2048; // 分组名称长度上限

        public const int LvlCol = 10;//指示分项分等题保存得分要点的二维数组的行、列
        public const int LvlRow = 10;
        //自定义异常原因
        public const int SelfExcpCode = 21;//自定义异常原因代码
        //主控服务器监听端口
        public const int MainSrvPort = 27520;
        //广播探测端口
        public const int BrdRecvPort = 27501; //用于客户端主机广播监听端口
        public const int BrdSendPort = 27500; //用于服务器广播监听端口        

        public const int UDPBasePort = 20000;
        //Socket超时等待
        public const int TimeOutSeconds = 120000;
        //评卷状态
        public const int Trial = 1;
        public const int YangPing = 2;
        public const int CePing = 3;
        public const int ZhengPing = 4;
        public const int Similar = 5;
        //用户身份
        public const int PuTong = 1;
        public const int XiaoZuZhang = 2;
        public const int TiZuZhang = 3;
        public const int DaZuZhang = 4;
        public const int NotSpecified = 5;
        public const int yichang = 7;
        public const int Special = 6;


        //帧类型常量

        public const int TM_AUTH_REQ = 0x01010001;//认证请求
        public const int TM_AUTH_RSP = 0x01010002;//认证响应
        public const int TM_BEACON_CLIENT = 0x01010003;//定时信标
        public const int TM_BEACON_SERVER = 0x01010004;//信标返回
        public const int TM_CHGPWD_REQ = 0x01010005;//更改密码请求
        public const int TM_CHGPWD_RSP = 0x01010006;//密码更改确认
        public const int TM_CHGSTAT_REQ = 0x01010007; //状态切换请求
        public const int TM_CHGSTAT_RSP = 0x01010008; //状态切换响应
        public const int TM_DBC_REQ = 0x01010009;// 更新通知
        public const int TM_DBC_RSP = 0x0101000A;// 更新确认

        public const int TM_GETBLKINFO_REQ = 0x0101000D;//题块信息请求
        public const int TM_GETBLKINFO_RSP = 0x0101000E;//题块信息响应

        public const int TM_GETGRPINFO_REQ = 0x01010011;//获取组用户信息
        public const int TM_GETGRPINFO_RSP = 0x01010012;//组用户信息响应
        public const int TM_GETPAPER_REQ = 0x01010013; //请求试题
        public const int TM_GETPAPER_RSP = 0x01010014;//取题确认
        public const int TM_GETQUAFILE_REQ = 0x01010015;//获取质量文件请求
        public const int PM_GETQUAFILE_NUM = 0x01030025; //增量文件类型
        public const int TM_GETQUAFILE_RSP = 0x01010016;//质量文件请求响应

        public const int TM_GETSMTASK_REQ = 0x01010019;//获取雷同卷任务
        public const int TM_GETSMTASK_RSP = 0x0101001A;//雷同卷任务响应

        public const int TM_BROADCAST_SRV = 0x0101001F;//广播消息
        public const int TM_PROBESRV_REQ = 0x01010020;//客户端探测
        public const int TM_PROBESRV_RSP = 0x01010021;//服务端响应
        public const int TM_QTYSTAT_REQ = 0x01010022; //提交查询请求
        public const int TM_QTYSTAT_RSP = 0x01010023; //返回查询响应
        public const int TM_SAVESCORE_REQ = 0x01010024;//试题存分
        public const int TM_SAVESCORE_RSP = 0x01010025;//存分确认

        public const int TM_SETRATIO_REQ = 0x01010028;//设置重评率请求
        public const int TM_SETRATIO_RSP = 0x01010029;//设置重评率响应
        public const int TM_UDPMSG_ACK = 0x0101002A;//单播回复
        public const int TM_UDPMSG_SRV = 0x0101002B; //单播消息
        public const int TM_USER_LOGOUT = 0x0101002C;//用户注销
        public const int TM_QUERYUSER_REQ = 0x0101002D; //探测请求
        public const int TM_QUERYUSER_RSP = 0x0101002E;//探测响应

        public const int TM_SAVESMPAPER_REQ = 0x0101002F;//雷同试卷结果保存
        public const int TM_SAVESMPAPER_RSP = 0x01010030; //雷同试卷结果响应

        //专家存分请求响应帧
        public const int TM_EXAMSAVESCORE_REQ = 0x01040007;
        public const int TM_EXAMSAVESCORE_RSP = 0x01040008;

        //提交参考答案请求响应帧
        public const int TM_SUBMITSTANDARD_REQ = 0x01040009;
        public const int TM_SUBMITSTANDARD_RSP = 0x0104000A;

        //考场字典请求响应帧
        public const int TM_GETCLASSROOMMAP_REQ = 0x01040003;
        public const int TM_GETCLASSROOMMAP_RSP = 0x01040004;

        //档次字典请求响应帧
        public const int TM_GETRATEMAP_REQ = 0x01040005;
        public const int TM_GETRATEMAP_RSP = 0x01040006;

        public const int PM_SAVESCORE_PRO = 0x01040019;  //专家存分类型

        public const int TM_MAKESAMPAPEROVER_REQ = 0x0101004A; //大组长确认样卷制作完成后发送此帧
        public const int TM_MAKESAMPAPEROVER_RSP = 0x0101004B;
        public const int RM_ERR_NOACTIVE = 0x0102002E;      //没有样卷分组处于激活状态

        //用于样卷库浏览
        public const int TM_GETSAMBLKINFO_REQ = 0x01010031;   //获取样卷题块信息请求
        public const int TM_GETSAMBLKINFO_RSP = 0x01010032;   //获取样卷题块信息响应
        public const int TM_GETSAMTOTAL_REQ = 0x01010045;   //获取样卷库信息请求
        public const int TM_GETSAMTOTAL_RSP = 0x01010046;   //获取样卷库信息响应
        public const int TM_DELSAMPAPER_REQ = 0x01050017;   //删除样卷请求
        public const int TM_DELSAMPAPER_RSP = 0x01050018;   //删除样卷响应
        public const int TM_MODIFYSAM_REQ = 0x01050019;    //修改样卷请求
        public const int TM_MODIFYSAM_RSP = 0x0105001A;    //修改样卷响应

        //用于样卷分组管理
        public const int TM_GETSAMGROUPINFO_REQ = 0x01050001; //获取样卷分组信息请求
        public const int TM_GETSAMGROUPINFO_RSP = 0x01050002; //获取样卷分组信息响应
        public const int TM_ADDSAMGROUP_REQ = 0x01050003; //增加样卷分组请求
        public const int TM_ADDSAMGROUP_RSP = 0x01050004; //增加样卷分组响应
        public const int TM_MODIFYSAMGROUP_REQ = 0x01050005; //修改样卷分组信息请求
        public const int TM_MODIFYSAMGROUP_RSP = 0x01050006; //修改样卷分组信息响应
        public const int TM_ACTIVESAMGROUP_REQ = 0x01050007; //激活样卷分组请求
        public const int TM_ACTIVESAMGROUP_RSP = 0x01050008; //激活样卷分组响应
        public const int TM_DELETESAMGROUP_REQ = 0x01050009; //删除样卷分组请求
        public const int TM_DELETESAMGROUP_RSP = 0x0105000A; //删除样卷分组响应
        public const int TM_GETSAMGROUPPAPER_REQ = 0x0105000B; //获取分组样卷信息请求
        public const int TM_GETSAMGROUPPAPER_RSP = 0x0105000C; //获取分组样卷信息响应
        public const int TM_SAVESAMGROUPPAPER_REQ = 0x0105000D; //保存样卷分组中样卷新信息请求
        public const int TM_SAVESAMGROUPPAPER_RSP = 0x0105000E; //保存样卷分组中样卷新信息响应

        //用于样评
        public const int TM_GETSAMGRPPAPERNO_REQ = 0x01050013;  //获取指定样卷分组中的所有样卷号请求
        public const int TM_GETSAMGRPPAPERNO_RSP = 0x01050014;  //获取指定样卷分组中的所有样卷号响应
        //TM_GETSAMGRPPAPERNO_REQ=0x0104000F;  //获取指定样卷分组中的所有样卷号请求
        //TM_GETSAMGRPPAPERNO_RSP=0x01040010;  //获取指定样卷分组中的所有样卷号响应
        public const int TM_GETSAMGRPNAME_REQ = 0x01050015;    //获取样卷分组名称请求
        public const int TM_GETSAMGRPNAME_RSP = 0x01050016;    //获取样卷分组名称响应


        public const int TP_ACTIVESAMGROUP = 0x01060001;  //激活样卷分组操作类型
        public const int TP_NOACTIVESAMGROUP = 0x01060002;  //取消激活样卷分组操作类型

        //取某个题块各个样卷分组下的样卷号
        public const int TM_GETSGPAPERNO_REQ = 0x01041100;
        public const int TM_GETSGPAPERNO_RSP = 0x01041101;

        public const int PM_AUTH_AGAIN = 0x01030001;  //重复登录
        public const int PM_AUTH_NORMAL = 0x01030002;  //正常登录
        public const int PM_BCMSG_ALL = 0x01030003;  //所有用户
        public const int PM_BCMSG_BTMGRP = 0x01030004;  //小组用户
        public const int PM_BCMSG_DESTROY = 0x01030005;  //销毁命令
        public const int PM_BCMSG_HALT = 0x01030006;  //自动关机
        public const int PM_BCMSG_LEADER = 0x01030007;  //组长文本通知
        public const int PM_BCMSG_MIDGRP = 0x01030008;  //题组用户
        public const int PM_BCMSG_NOTRIAL = 0x01030009;  //结束系统试用
        public const int PM_BCMSG_SINGLE = 0x0103000A;  //单个用户更新
        public const int PM_BCMSG_SYSTEM = 0x0103000B;  //系统广播消息
        public const int PM_BCMSG_TOPGRP = 0x0103000C;  //大组用户
        public const int PM_BCMSG_USER = 0x0103000D;  //用户资料更新
        public const int PM_CHGSTAT_OK = 0x0103000E;  //同意
        public const int PM_CHGSTAT_REFUSE = 0x0103000F;  //拒绝
        public const int PM_DBC_BLOCK = 0x01030010;  // 块更新
        public const int PM_DBC_USER = 0x01030011;  // 用户更新

        public const int PM_CHGSTAT_ALL = 0x01040015; //设置全部
        public const int PM_CHGSTAT_TEAM = 0x01040016;//设置小组
        public const int PM_CHGSTAT_USER = 0x01040017;//设置单个用户
        public const int PM_CHGSTAT_GROUP = 0x01040018;//设置题组



        public const int PM_GETBLKINFO_NORMAL = 0x01030014;  //第一次请求
        public const int PM_GETBLKINFO_TIPS = 0x01030015;  //请求Tips数据
        public const int PM_GETQUAFILE_FILE = 0x01030016;  //获取原始文件

        public const int PM_GETQUAFILE_INC = 0x01030018;  //获取增量文件
        public const int PM_GETQUAFILE_MEM = 0x01030019;  //使用内存缓冲区
        public const int PM_GETQUAFILE_SOCKET = 0x0103001A; //使用Socket缓冲区

        public const int PM_PAPERTYPE_CEPING = 0x0103001B;  //测评试题
        public const int PM_PAPERTYPE_CONFLICT = 0x0103001C;  //仲裁试卷
        public const int PM_PAPERTYPE_EXCEPTION = 0x0103001D;  //异常试题
        public const int PM_PAPERTYPE_FREESEL = 0x0103001E;  //抽调试卷
        public const int PM_PAPERTYPE_NORMAL = 0x0103001F;  //正评试题
        public const int PM_PAPERTYPE_RESAMPLE = 0x01030020;  //样卷重评题
        public const int PM_PAPERTYPE_SAMPLE = 0x01030021;  //样卷试题
        public const int PM_PAPERTYPE_SELF = 0x01030022;  //自重评题
        public const int PM_PAPERTYPE_SIMILAR = 0x01030023;  //雷同卷
        public const int PM_PAPERTYPE_TRIAL = 0x01030024;  //试用取题

        public const int PM_QTYSTAT_CEPING = 0x01030025;  // 查询测评统计信息
        public const int PM_QTYSTAT_YANGPING = 0x01030026;  // 查询样卷统计信息
        public const int PM_QTYSTAT_RESAMPLE = 0x01030027;  // 查询样卷重评统计信息
        public const int PM_QTYSTAT_RESELF = 0x01030028;  // 查询自重评统计信息

        public const int PM_SACESCORE_CHECKED = 0x01030029;  //已核查（零分或者满分）
        public const int PM_SAVESCORE_CEPING = 0x0103002A;  //测评存分
        public const int PM_SAVESCORE_DELAY = 0x0103002B;  //延后再判
        public const int PM_SAVESCORE_NORMAL = 0x0103002C;  //正常存分
        public const int PM_SAVESCORE_RECYCLE = 0x0103002D;  //回收试题
        public const int PM_SAVESCORE_RESAMPLE = 0x0103002E;  //样卷重评
        public const int PM_SAVESCORE_SAMPLE = 0x0103002F;  //样卷存分
        public const int PM_SAVESCORE_SELF = 0x01030030;  //自重评
        public const int PM_SAVESCORE_YICHANG = 0x01030031;  //异常存分
        public const int PM_SAVESCORE_YCREPORT = 0x01030037;  //异常提交

        public const int PM_GETPAPER_NORMAL = 0x01030032;  //不显示分数
        public const int PM_GETPAPER_DISPSCORE = 0x01030033;  //显示以前的分数

        public const int PM_SETRATIO_RESAMPLE = 0x01030034;  //样卷重评率
        public const int PM_SETRATIO_RESELF = 0x01030035;  //自重评率

        public const int PM_GETYCPAPER_ALL = 0x01030036;  //获取所有异常试卷



        public const int RM_RSP_OK = 0x01020001;  //操作成功

        public const int RM_ERR_CURSTAT = 0x01020002;  //当前状态错误
        public const int RM_ERR_ERR = 0x01020003;  //未知错误
        public const int RM_ERR_EXCP = 0x01020004;  //异常类型错误
        public const int RM_ERR_FILE = 0x01020005;  //无此文件（客户端不用）
        public const int RM_ERR_GROUP = 0x01020006;  //组号错误
        public const int RM_ERR_INVAILD = 0x01020007;  //非法存分
        public const int RM_ERR_LOCKED = 0x01020008;  //帐号已锁定
        public const int RM_ERR_LOGED = 0x01020009;  //该用户已登录
        public const int RM_ERR_MEM = 0x0102000A;  //缓冲区长度不足（客户端不用）
        public const int RM_ERR_NEXTSTAT = 0x0102000B;  // 转换状态错误
        public const int RM_ERR_NOBLK = 0x0102000C;  //题已取完
        public const int RM_ERR_NOCHG = 0x0102000D;  //不允许更改
        public const int RM_ERR_NOPAPER = 0x0102000F;  //该题已取完


        public const int RM_ERR_SAMPAPER = 0x0102002F;	//该试卷已制作为样卷
        public const int RM_ERR_EXISTREC = 0x01020030;	//该用户已取过该试卷


        public const int RM_ERR_NOSAMNO = 0x01020025;   //该题块没有样卷号 
        public const int RM_ERR_NOSAMSCORE = 0x01020026;    //没有样卷成绩 


        public const int RM_ERR_NOUPDATE = 0x01020011;  //无更新
        public const int RM_ERR_OLDPWD = 0x01020012;  //原密码错误
        public const int RM_ERR_PAPER = 0x01020013;  //试卷号错误
        public const int RM_ERR_PAPERTYPE = 0x01020014;  //取题类型错误
        public const int RM_ERR_QTYTYPE = 0x01020015;  //质量文件类型错误
        public const int RM_ERR_RATIO = 0x01020016;  //比率错误
        public const int RM_ERR_ROLE = 0x01020017;  //角色错误（权限不足）
        public const int RM_ERR_SCOPE = 0x01020018;  // 范围错误
        public const int RM_ERR_TYPE = 0x01020019;  // 类型错误
        public const int RM_ERR_USER = 0x0102001A;  //用户名错误
        public const int RM_ERR_WAIT = 0x0102001B;  //等待取题
        public const int RM_ERR_BLOCK = 0x0102001C;  //块号错误
        public const int RM_ERR_BUSY = 0x0102001D;  //服务器忙
        public const int RM_ERR_STATUS = 0x0102001E; //用户状态错误


        public const int RM_ERR_FAIL = 0x01020027;  //操作失败
        public const int RM_ERR_SAMGRPNAME = 0x01020028;  //样卷分组名已存在
        public const int RM_ERR_NOSAMGRP = 0x01020029;  //样卷还未被分组
        public const int RM_ERR_NOTEXIST = 0x0102002A;  //数据库中找不到指定的样卷分组
        public const int RM_ERR_ACTIVE = 0x0102002B;  //样卷分组处于激活状态
        public const int RM_ERR_REFUSE = 0x01020031;  //已经样评过的分组不允许删除
        public const int RM_ERR_NOSAMPAPER = 0x0102002C;	//样卷不存在
        public const int RM_ERR_PAPERNOTEXIST = 0x00000001;  //a

        public const int RM_ERR_BLKNOSTART = 0x01020032; //题块尚未启动


        //客户端自定义——服务器正在瞎忙
        public const int RM_ERR_CrazyBusy = 0x0102FFFF;


        public const int TM_GETEXCPTStuINFO_REQ = 0x01060001;  //异常题块学生信息请求
        public const int TM_GETEXCPTStuINFO_RSP = 0x01060002;  //异常题块学生信息响应
        public const int TM_GETEXPAPER_REQ = 0x01060003;        //异常题块图片信息请求
        public const int TM_GETEXPAPER_RSP = 0x01060004;        //异常题块图片信息相应

        public const int TM_GETBLKINFOYC_REQ = 0x0101000F;  //异常题块信息请求
        public const int TM_GETBLKINFOYC_RSP = 0x01010010;  //异常题块信息响应

        public const int TM_GETYCBOOK_REQ = 0x01010031;   //获取异常试题本信息请求
        public const int TM_GETYCBOOK_RSP = 0x01010032;   //获取异常试题本信息响应

        public const int PM_SAVESCOREYC_REQ = 0x01010038;      //异常试题存分
        public const int PM_SAVESCOREYC_RSP = 0x01010039;      //异常存分确认

        public const int PM_SAVESCOREYCDZ_REQ = 0x01060005;   //大组长异常试题存分
        public const int PM_SAVESCOREYCDZ_RSP = 0x01060006;   //大组长异常存分确认


        public const int TM_GETBLKDATA_REQ = 0x01010033;


        public const int RM_ERR_PAGENO = 0x0102001F;  //错误的本信息

        // 用于即时通信
        public const int TM_UDPMSG_IM_Srv = 0x0101003B;   //从服务器来的消息
        public const int TM_UDPMSG_IM_Client = 0x0101003A;  //客户发出去的消息

        //异常登分时同一个用户登第二次时接受的响应信息
        public const int RM_ERR_SAMEUSERID = 0x01020020;

        //获取评分细则、参考答案请求响应帧头
        public const int TM_GETSTANDARD_REQ = 0x01040001;
        public const int FM_GETPINGFENXIZE = 1;
        public const int FM_GETCANKAODAAN = 2;
        public const int TM_GETSTANDARD_RSP = 0x01040002;

        //获取改卷情况的请求帧和响应帧 
        public const int TM_GETPAPERRATE_REQ = 0x0104000D;
        public const int TM_GETPAPERRATE_RSP = 0x0104000E;
        //Stream超时等待（一分钟）  
        public const double TimeoutStream = 6e4;

        public const int RM_ERR_GETEXAMOVER = 0x01020023;

        //样卷浏览请求响应帧沿用以前的帧头
        //获取样评统计信息请求帧响应帧
        public const int TM_GETEXAMQTY_REQ = 0x0104000B;
        public const int TM_GETEXAMQTY_RSP = 0x0104000C;

        //样卷质量监控通信帧  
        public const int TM_GETSAMPAPERNO_REQ = 0x0104000F; //取某个题块全部样卷号
        public const int TM_GETSAMPAPERNO_RSP = 0x01040010;
        //取某个题块某样卷成绩统计 
        public const int TM_GETSAMSCORESTATS_REQ = 0x01040013;
        public const int TM_GETSAMSCORESTATS_RSP = 0x01040014;



        //专家取卷请求响应帧头沿用以前的，取卷类型如下
        public const int PM_PAPERTYPE_PRO = 0x01030038;  //专家取卷类型定义

        //重评率设置请求响应帧 沿用以前的
        public const int PM_SETRATIO_ALL = 0x01040012;    //设置全部
        public const int PM_SETRATIO_GROUP = 0x0104001C;  //设置题组 修改题组的重评率信息
        public const int PM_SETRATIO_TEAM = 0x01040010;   //设置小组
        public const int PM_SETRATIO_PERSON = 0x01040011; //设置个人

        public const int PM_PAPERTYPE_PAPERNO = 0x01040017; //专家根据试卷号取卷库中的试卷
        public const int PM_PAPERTYPE_CLASSROOM = 0x01040018;//专家根据考场号取卷库中的试卷



        public const int TM_GETSAMSCOREBYGROUP_REQ = 0x0104001A;
        public const int TM_GETSAMSCOREBYGROUP_RSP = 0x0104001B;

        public const int PM_GETSTATISTICS_XZ = 0x01030026;	//获取指定num记录的小组长质量统计文件
        public const int PM_GETSTATISTICS_TZ = 0x01030027;	//获取指定num记录的题组长质量统计文件

        public const int PM_PAPERTYPE_SELFPAPERNO = 0x01040019; //业务组长根据试卷号取试题图片（自重评） 


        public const int RM_ERR_STUNO = 0x0102001F;  //学号错误 pzg
        //获取个人质量信息帧类型 
        public const int TM_GETPERQTY_REQ = 0x0106000A;
        public const int TM_GETPERQTY_RSP = 0x0106000B;

        //定义图片格式
        public const int TIF = 1;
        public const int JPG = 3;
        public const int BMP = 5;
        public const int JP2 = 13;
        //图片路径长度
        public const int PICPATHLEN = 256;     // 图片路径最大长度
        public const int SERVERIPLEN = 16;     // 图片所在服务器IP地址长度
        public const int SHAREDPICPATHLEN = 280;     // 加上IP地址后的图像路径长度，这里是网络共享路径

    }
}