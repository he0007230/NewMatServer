using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AceNetFrameWork.ace
{
   public abstract class AbsHandlerCenter
    {
       /// <summary>
       /// 消息到达 用户链接 收到消息
       /// </summary>
       /// <param name="token"></param>
       /// <param name="message"></param>
       public abstract void MessageReceive(UserToken token, object message);
       /// <summary>
       /// 客户端连接
       /// </summary>
       /// <param name="token"></param>
       public abstract void ClientConnect(UserToken token);
       /// <summary>
       /// 客户端断开
       /// </summary>
       /// <param name="token"></param>
       /// /// <param name="error">断开错误代码</param>
       public abstract void ClientClose(UserToken token,string error);
    }
}
