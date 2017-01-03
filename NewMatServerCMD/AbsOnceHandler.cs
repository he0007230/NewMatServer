using AceNetFrameWork.ace;
using AceNetFrameWork.ace.auto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogHelper;

namespace NewMatServerCMD
{
   public class AbsOnceHandler
    {
       private int type;
       private int area;

       public void setArea(int area) {
           this.area = area;
       }

       public void setType(int type) {
           this.type = type;
       }

       public int getArea() {
           return area;
       }

       public virtual int getType() {
           return type;
       }

       public void write(UserToken token, int command)
       {
           write(token, getType(), getArea(), command, null);
       }

       public void write(UserToken token, int command, object message) {
           write(token, getType(), getArea(), command, message);
       }

       public void write(UserToken token, int area, int command, object message)
       {
           write(token, getType(), area, command, message);
       }      

       public void write(UserToken token, int type, int area, int command, object message) {
           byte[] bs= MessageEncoding.Encode(createSocketModel(type, area, command, message));
           bs=LengthEncoding.encode(bs);
           LoggerHelper.Info(token.connectSocket.RemoteEndPoint.ToString()+"  --返回处理结果");
           token.write(bs);
       }

       public SocketModel createSocketModel(int type, int area, int command, object message) {
           return new SocketModel(type, area, command, message);
       }
    }
}
