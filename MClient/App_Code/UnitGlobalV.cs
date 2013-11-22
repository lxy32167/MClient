using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MClient.App_Code
{
    public class UnitGlobalV
    {
        public Boolean TestFlag;
        public Boolean RefreshQtyFlag = true;
        public const int WindowCount = 5;
        public string Version = "V 4.0";
        public int LockTime = 10 * 60;
        static public DateTime javaTime = new DateTime(1970, 1, 1);
        static public DateTime delphiTime = new DateTime(1899, 12, 30);
        static public TimeSpan ts = javaTime - delphiTime;

        public int Num = 0; //本地已有的记录数
        public int TotalRecCount = 0;//某一次质量信息刷新时总的记录条数
        public int RcvdRecCount = 0; //某一次质量信息刷新时已经接受的质量信息条数
        public int TimeSpann = 15;//最大时间跨度设为15天
        public int XYCount = 360; //需要计算的最大坐标个数
        public Boolean RcvFinished = false;
        public uint rcvtime, uncomtime, wtime, waittime;

    } 
}