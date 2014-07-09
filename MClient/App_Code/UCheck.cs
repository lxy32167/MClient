using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MClient.App_Code
{
    public class UCheck     //原check里定义的变量，页面无法保存，故定义类，大部分用不着
    {
        public string tempstat1, tempstat2, tempstat3, tempstat4, tempstat5;              //保存状态栏信息
        public string gDetailScore;      //整个题目的全部详细分 形如翻译题的'1,1.5,2,1,1.5'
        public Single gTotalScore;//整个题目的总分 接上例  为7
        public Single CurImageScale;          //试卷图片的显示比例
        public Boolean status_gist;         //判断窗体是处于依据-试卷-评分栏布局(true)还是试卷-评分栏布局
        public int ScrollPosionFlag;      //标记图片的位置 0:顶头显示 1：顶底显示 使用空格键控制图片上下滚动时使用
        public int TestMin;
        public int ExcpRsn;
        public Boolean status_review;        //判断是否处于试卷复查状态（处于试卷复查状态是为true）
        public Boolean LastPaper;           //查看样卷是否分发完
        public Boolean SampleOver;            //查看样卷是否改完（有时候已分发完，但是未改完，就不能浏览样卷）

        ////********************************套红框信息*******************************
        //    Throw_Rectangl:boolean;      //是否去除套红框 TRUE表示去除
        //    k:integer;                   //表示是第几个套红框，从1开始，不是0
        //    RectanglNum:integer;         //表示本题块有几个套红框
        //    StringRect:string;           //装有题块原始套红框信息的字符串
        //    tmpStringRect:string;        //装有题块变化中的套红框信息的字符串
        //    tmpRtg:string;               //暂时记录每个给分点的套红框数据

        //    ColorPicPath:string;         //存放彩色图片的路径
        //    DRect:TRect;
        //    SRect:TRect;

        ////***********************评分依据显示系列变量及函数************************
        public UMyRecords.stFullBlockInfo PMyBlock;

        public int BiaoZhunNum;   //评分标准的总数
        public int XiZeNum;       //评分细则的总数
        public int DaAnNum;       //参考答案的总数
        public Boolean YiJu;          //判断是否有评分依据，true表示有，false表示没有

        public int BiaoZhunNo;    //当前显示的评分标准号
        public int XiZeNo;        //当前显示的评分细则号
        public int DaAnNo;        //当前显示的参考答案号

        public int BiaoZhunWidth;
        public int BiaoZhunHeight;
        public int XiZeWidth;
        public int XiZeHeight;
        public int DaAnWidth;
        public int DaAnHeight;

        public Single CurImageScale_BiaoZhun;     //评分标准的显示比例
        public Single CurImageScale_XiZe;         //评分细则的显示比例
        public Single CurImageScale_DaAn;         //参考答案的显示比例
        //    ThdPreFetchNewPaper:TPreFetchThread;
        public UMyRecords.stMyPaper PExpPaperNode;

        public int[] previewpaper = { -1, -1, -1 };
        public string[] ScoreInput; //在Postback绑定之前，获取上次输入数据
        public Boolean ScoreChecked;//输入分数是否曾经检查过
        public Boolean ScoreInputOK;
        public Boolean Qty = false;
        //   public int CheckNum = 1; //记录当前Check序号

        //保存阅卷速度和平均分时间曲线横坐标的值，横坐标为时间如08:15
        public DateTime[] TimeArray = new DateTime[UnitGlobalV.WindowCount];
        //保存个人在WindowTime时间内阅卷平均分数据，只保存最近WindowCount个检测窗口的数据
        public Single[] PerAvgScoreArray = new Single[UnitGlobalV.WindowCount];
        //保存个人在WindowTime时间段内阅卷速度数据
        public Single[] PerSpeedArray = new Single[UnitGlobalV.WindowCount];
        //保存题组在WindowTime时间内阅卷平均分数据，只保存最近WindowCount个检测窗口的数据
        public Single[] BlkAvgScoreArray = new Single[UnitGlobalV.WindowCount];
        //保存题组在WindowTime时间段内阅卷速度数据
        public Single[] BlkSpeedArray = new Single[UnitGlobalV.WindowCount];
        ////定时刷新个人质量信息线程
        //ThdRefreshQty:TRefreshQty;
        ////记录某个教师已经有连续几份试卷给了趋中分数
        //nMiddleScore:Integer;
        ////记录某个教师已经有连续几份试卷阅卷速度达到了题组的两倍及以上
        //nFastSpeed:Integer;
        ////记录水平和垂直滚动条的位置
        //HPosition:Integer;
        //VPosition:Integer;
        public Boolean FirstQtyFlag = true;
        public int PaperCount;
        public DataModule.stCheck[] stCheckTemp;//输入错误分数时，保存元分数
    }
}