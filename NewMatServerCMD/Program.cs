using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AceNetFrameWork;
using AceNetFrameWork.ace;
using LogHelper;
using AceNetFrameWork.ace.auto;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace NewMatServerCMD
{

    class Program
    {
        delegate void ShowMessageCallBack(string msg);
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]   //找子窗体   
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        [DllImport("User32.dll", EntryPoint = "SendMessage")]   //用于发送信息给窗体   
        private static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, string lParam);
        [DllImport("User32.dll", EntryPoint = "ShowWindow")]   //
        private static extern bool ShowWindow(IntPtr hWnd, int type);
        static void Main(string[] args)
        {

            Console.Title = "无线服务器";
            IntPtr ParenthWnd = new IntPtr(0);
            IntPtr et = new IntPtr(0);
            ParenthWnd = FindWindow(null, "无线服务器");
            ShowWindow(ParenthWnd, 2);//隐藏本dos窗体, 0: 后台执行；1:正常启动；2:最小化到任务栏；3:最大化
            //LoggerHelper.ShowMessage += ShowMessage;
            try
            {
                LoggerHelper.ShowMessage += ShowMessage;
                IOCPServ server = new IOCPServ(Config.Instance.sysInfo.maxListener);
                server.lengthEncode = LengthEncoding.encode;
                server.lengthDecode = LengthEncoding.decode;
                server.serEncode = MessageEncoding.Encode;
                server.serDecode = MessageEncoding.Decode;
                server.center = new HandlerCenter();
                server.init();
                server.Start(Config.Instance.sysInfo.server_port);

                LoggerHelper.Info("无线服务器启动成功...", false);

            }
            catch (Exception err)
            {
                LoggerHelper.Info("服务器启动出错:" + err.Message);
                Process proc = null;
                try
                {
                    proc = new Process();
                    proc.StartInfo.FileName = @"D:\MatServer\resetCMD.bat";
                    proc.StartInfo.CreateNoWindow = false;
                    proc.Start();
                }
                catch
                {
                }
                return;
            }
            for (; ; ) ;
            //while (true) { }
            //Console.Read();
        }
        public static void ShowMessage(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
