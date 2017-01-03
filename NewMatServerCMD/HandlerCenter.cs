using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AceNetFrameWork.ace;
using AceNetFrameWork.ace.auto;
using LogHelper;
using NewMatServerCMD.tool;
using NewMatServerCMD.Model;
using System.Diagnostics;

namespace NewMatServerCMD
{

    public class HandlerCenter:AbsHandlerCenter
    {
        private AbsOnceHandler sendHandler;

        public HandlerCenter()
        {
            sendHandler = new AbsOnceHandler();
        }


        public override void MessageReceive(UserToken token, object message)
        {
            LoggerHelper.Info(token.connectSocket.RemoteEndPoint.ToString() + "  --接收数据");
            SocketModel model = message as SocketModel;
            TimeoutSetting timeoutSetting = new TimeoutSetting();
            timeoutSetting.Do += analysis;
            bool isTimeout = timeoutSetting.DoWithTimeout(token, model, new TimeSpan(0, 0, 0, 8));
            if (isTimeout)
            {
                Config.Instance.timeoutCount++;
                LoggerHelper.Info(token.connectSocket.RemoteEndPoint.ToString() + "  --处理数据超时" + Config.Instance.timeoutCount);
                if (Config.Instance.timeoutCount > 2)
                {
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
                }
            }
            else
            {
                Config.Instance.timeoutCount = 0;
            }
            //analysis(token, model);
            /*
            ExecutorPool.Instance.execute(delegate()
            {
                analysis(token, model);
            });
             * */
        }

        public override void ClientConnect(UserToken token)
        {
            LoggerHelper.Info(token.connectSocket.RemoteEndPoint.ToString() + "  --客户端连接");
        }
        public override void ClientClose(UserToken token, string error)
        {
            LoggerHelper.Info(token.connectSocket.RemoteEndPoint.ToString() + "  --客户端断开:" + error);
        }
        private void analysis(UserToken token, SocketModel socketModel)
        {
            CompactFormatter.TransDTO transDTO = socketModel.message as CompactFormatter.TransDTO;
            RedirectModel redirectModel;
            object objmsg;
            if (Config.Instance.redirectDict.TryGetValue(transDTO.pFlag, out redirectModel))
            {
                //跳转到对应的处理
                Config.Instance.mainList[redirectModel.funcName](transDTO, redirectModel,out objmsg);
            }
            else
            {
                redirectModel = new RedirectModel();
                objmsg = "找不到对应的处理模块!";
                LoggerHelper.Info(token.connectSocket.RemoteEndPoint.ToString() + "  --找不到对应的处理模块!");
                redirectModel.area = 1;
                redirectModel.type = 1;
                redirectModel.command = 1;
            }
            sendHandler.write(token, redirectModel.type, redirectModel.area, redirectModel.command, objmsg);
        }
    }
}
