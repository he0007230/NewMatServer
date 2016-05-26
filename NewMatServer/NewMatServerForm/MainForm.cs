using AceNetFrameWork.ace;
using AceNetFrameWork.ace.auto;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LogHelper;
using NewMatServerForm.Model;

namespace NewMatServerForm
{
    public partial class MainForm : Form
    {
        delegate void ShowMessageCallBack(string msg);

        public MainForm()
        {
            InitializeComponent();
            LoggerHelper.ShowMessage += ShowMessage;
        }

        private void StartBtn_Click(object sender, EventArgs e)
        {
            Initialize();
        }
        private void Initialize()
        {
            try
            {
                IOCPServ server = new IOCPServ(Config.Instance.xmlData.System.maxListener);
                server.lengthEncode = LengthEncoding.encode;
                server.lengthDecode = LengthEncoding.decode;
                server.serEncode = MessageEncoding.Encode;
                server.serDecode = MessageEncoding.Decode;
                server.center = new HandlerCenter();
                server.init();
                server.Start(Config.Instance.xmlData.System.server_port);
                //Console.WriteLine("无线服务器启动成功...");
                //ShowMessage("无线服务器启动成功..." + Config.Instance.xmlData.System.maxListener);


                LoggerHelper.Info("无线服务器启动成功...",false);

            }
            catch (Exception err)
            {
                LoggerHelper.Error("服务器启动出错:" + err.TargetSite);
                //Console.WriteLine("服务器启动出错:" + err.TargetSite);
                //Console.WriteLine(err.Source);
                //Console.WriteLine(err.Message);
            }
        }
        public void ShowMessage(string msg)
        {
            if (this.msgListBox.InvokeRequired)
            {
                ShowMessageCallBack smcb = new ShowMessageCallBack(ShowMessage);
                this.Invoke(smcb, new object[] { msg });
            }
            else
            {
                if (msgListBox.Items.Count > 15)
                {
                    msgListBox.Items.Clear();
                }
                msgListBox.Items.Add(msg);
            }
        }

        private void StopBtn_Click(object sender, EventArgs e)
        {
            LoggerHelper.Info("无线服务器关闭...",false);
        }
    }
}
