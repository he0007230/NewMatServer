using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace AceNetFrameWork.ace
{
   public class UserToken
    {
       public SocketAsyncEventArgs receiveEvent;
       public SocketAsyncEventArgs sendEvent;
       public Socket connectSocket;
       public delegate void SendProcess(SocketAsyncEventArgs e);
       public SendProcess sendProcess;
       public delegate void CloseProcess(UserToken token,string error);
       public CloseProcess closeProcess;
       public List<byte> cache = new List<byte>();
       public Queue<byte[]> writeQueue = new Queue<byte[]>();
       #region 编码解码器委托
       public delegate void MessageReceive(UserToken token, object message);

       public LengthEncode lEnCode;
       public LengthDecode lDecode;
       public SerEncode sEncode;
       public SerDecode sDecode;
       public MessageReceive messageReceive;
       #endregion

       bool isReading = false;
       bool isWriteing = false;

       public UserToken(int buffSize) {
            receiveEvent = new SocketAsyncEventArgs();
            receiveEvent.UserToken = this;
            sendEvent = new SocketAsyncEventArgs();
            sendEvent.UserToken = this;
            byte[] buff = new byte[buffSize];
            receiveEvent.SetBuffer(buff, 0, buff.Length);
        }

        public void receive(byte[] buff) {
            cache.AddRange(buff);
            if (!isReading)
            {
                onData();
            }
        }

        public void onData() {
            byte[] buff=null;
            if (lDecode != null)
            {
                buff = lDecode(ref cache);
                if (buff == null) { isReading = false; return; }
            }
            else {
                if (cache.Count == 0) return;
                buff = cache.ToArray();
                cache.Clear();
            }
            if (sDecode == null) throw new Exception("message decode process is null");
            if (messageReceive == null) throw new Exception("messageReceive process is null");
            object message = sDecode(buff);
            messageReceive(this, message);
            onData();
        }

        public void write(object value) {
            if (sEncode == null) { throw new Exception("message encode process is null"); }
            byte[] dcodes= sEncode(value);
            if (lEnCode != null) {
               dcodes= lEnCode(dcodes);
            }
            write(dcodes);
        }

        public void onWrite()
        {
            if (writeQueue.Count == 0) { isWriteing = false; return; }
            byte[] buff = writeQueue.Dequeue();
                sendEvent.SetBuffer(buff, 0, buff.Length);
                bool result = connectSocket.SendAsync(sendEvent);
                if (!result)
                {
                    sendProcess(sendEvent);
                }
        }

        public void sendEnd()
        {
            onWrite();
        }

       public void write(byte[] buff){
           if (connectSocket == null) {
               closeProcess(this,"发送消息给已断开的连接");
               return;
           }
           writeQueue.Enqueue(buff);
           if (!isWriteing)
           {
               isWriteing = true;
               onWrite();
           }
           
       }

        public void close() {
            if (connectSocket == null) {
                Console.WriteLine("keng");
            }
            try
            {
                writeQueue.Clear();
                cache.Clear();
                isReading = false;
                isWriteing = false;
                connectSocket.Shutdown(SocketShutdown.Both);
                connectSocket.Close();
                connectSocket = null;   
            }
            catch (Exception e){
                Console.WriteLine(e.Message);
            }
                     
        }
    }
}
