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
                if (DM_Client.MyRecords.UserInfo.Status != BeaconRsp.Status)
                {
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
}