using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AceNetFrameWork.ace;
using AceNetFrameWork.ace.auto;
using LogHelper;
using NewMatServerForm.tool;
using NewMatServerForm.Model;
using transprot.dto;

namespace NewMatServerForm
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
            LoggerHelper.Info(token.connectSocket.RemoteEndPoint.ToString() + "  --有消息到达:" + (message as SocketModel).ts());
            SocketModel model = message as SocketModel;
            ExecutorPool.Instance.execute(delegate()
            {
                analysis(token, model);
            });
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
            TransDTO transDTO = socketModel.message as TransDTO;
            RedirectModel redirectModel = new RedirectModel();
            if (Config.Instance.redirectDict.TryGetValue(transDTO.pflag, out redirectModel))
            {
                //跳转到对应的处理
                Config.Instance.mainList[redirectModel.funcName](transDTO, redirectModel);
            }
            else
            {
                Config.Instance.outStr = "找不到对应的处理模块!";
                LoggerHelper.Info(token.connectSocket.RemoteEndPoint.ToString() + "  --找不到对应的处理模块!");
                redirectModel.area = 1;
                redirectModel.type = 1;
                redirectModel.command = 1;
            }
            sendHandler.write(token, redirectModel.type, redirectModel.area, redirectModel.command, Config.Instance.outStr);
        }
    }
}
