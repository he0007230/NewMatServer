using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AceNetFrameWork.ace
{
   public abstract class AbsLengthEncoding
    {
       /// <summary>
       /// 消息通过网络传输前调用
       /// </summary>
       /// <param name="buff">需要添加长度的数据</param>
       /// <returns>返回增加字节长度后的byte数组</returns>
       public abstract byte[] encode(byte[] buff);
       /// <summary>
       /// 网络消息到达调用
       /// </summary>
       /// <param name="cache">从网络中收到的消息数据</param>
       /// <returns>长度未达到则返回空，否则从缓存中取出数据并返回</returns>
       public abstract byte[] decode(ref List<byte> cache);

    }
}
