﻿using System.Collections;
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.IO;

namespace CCC
{

    public class NetWorkScript
    {
        public delegate void FinRecvHandler();
        public event FinRecvHandler FinRecvEvent;
        /// <summary>
        /// 全局唯一的连接对象实例
        /// </summary>
        private static NetWorkScript instance;

        private byte[] readBuff = new byte[1024];
        private bool isRead = false;
        public List<SocketModel> messageList = new List<SocketModel>();
        private Socket socket;

        private List<byte> cache = new List<byte>();

        public static NetWorkScript Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new NetWorkScript();
                }
                return instance;
            }
        }

        public void release()
        {
            if (socket != null)
            {
                socket.Close();
            }
            cache.Clear();
            messageList.Clear();
        }

        public int init(string ip, int port)
        {
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(ip, port);
                socket.BeginReceive(readBuff, 0, 1024, SocketFlags.None, ReceiveCallBack, readBuff);
                //Console.WriteLine("连接服务器成功");
                return 0;
            }
            catch (Exception e)
            {
                //Console.WriteLine("连接服务器失败" + e.Message);
                return -1;
            }
        }

        public int write(int type, int area, int command, object message)
        {
            ByteArray arr = new ByteArray();
            arr.write(type);
            arr.write(area);
            arr.write(command);
            if (message != null)
            {
                byte[] bs = SerializeUtil.encode(message);
                arr.write(bs);
            }
            ByteArray arr1 = new ByteArray();
            arr1.write(arr.Length);
            arr1.write(arr.getBuff());
            try
            {
                socket.Send(arr1.getBuff());
            }
            catch (Exception e)
            {
                //Console.WriteLine("网络错误，请重新登录" + e.Message);
                return -1;
            }
            return 0;
        }

        private void ReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                //结束异步消息读取 并获取消息长度
                int readCount = socket.EndReceive(ar);
                Console.WriteLine(readCount);
                byte[] bytes = new byte[readCount];
                //将接收缓冲池的内容复制到临时消息存储数组
                Buffer.BlockCopy(readBuff, 0, bytes, 0, readCount);
                cache.AddRange(bytes);
                if (!isRead)
                {
                    isRead = true;
                    onData();
                }
            }
            catch (Exception e)
            {
                //Console.WriteLine("远程服务器主动断开连接" + e.Message);
                socket.Close();
                return;
            }

            socket.BeginReceive(readBuff, 0, 1024, SocketFlags.None, ReceiveCallBack, readBuff);
        }

        private void onData()
        {
            //消息体长度为一个4字节数值 长度不足的时候 说明消息未接收完成 或者是废弃消息
            if (cache.Count < 4)
            {
                isRead = false;
                return;
            }

            byte[] result = ldecode(ref cache);

            if (result == null)
            {
                isRead = false;
                return;
            }
            //转换为传输模型用于使用
            SocketModel model = mDecode(result);
            //将消息存储进消息列表 等待读取
            messageList.Add(model);
            FinRecvEvent();
            onData();
        }


        public static byte[] ldecode(ref List<byte> cache)
        {
            if (cache.Count < 4) return null;
            MemoryStream ms = new MemoryStream(cache.ToArray());
            BinaryReader br = new BinaryReader(ms);
            int length = br.ReadInt32();
            if (length > ms.Length - ms.Position)
            {
                return null;
            }

            byte[] result = br.ReadBytes(length);
            cache.Clear();
            cache.AddRange(br.ReadBytes((int)(ms.Length - ms.Position)));
            br.Close();
            ms.Close();
            return result;
        }

        public static SocketModel mDecode(byte[] value)
        {
            ByteArray ba = new ByteArray(value);
            SocketModel sm = new SocketModel();
            int type;
            int area;
            int command;
            ba.read(out type);
            ba.read(out area);
            ba.read(out command);
            sm.type = type;
            sm.area = area;
            sm.command = command;
            if (ba.Readnable)
            {
                byte[] message;
                ba.read(out message, ba.Length - ba.Position);
                sm.message = SerializeUtil.decoder(message);
            }
            ba.Close();
            return sm;
        }
    }
}