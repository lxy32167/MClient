using System;
using System.Collections.Generic;
//using System.Linq;
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

namespace MClient.App_Code
{
    public class UWini
    {
        public static void WriteUserInfo(string IP, string UserName, string Password)
        {
            FileStream ds = File.Open(@"C:/info.ini", FileMode.Create, FileAccess.Write, FileShare.None);
            StreamWriter sw = new StreamWriter(ds);
            sw.WriteLine("[UserInfo]");
            sw.WriteLine("ServerIP={0}\n", IP);
            sw.WriteLine("UserID={0}\n", UserName);
            sw.WriteLine("PassWord={0}\n", Password);
            sw.Dispose();
            sw.Close();
            ds.Dispose();
            ds.Close();
        }


    }
}