using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCC;
using transprot.dto;

namespace CCCTest
{
    class Program
    {
        static void Main(string[] args)
        {
            NetWorkScript.Instance.init("192.168.3.119",10005);
            NetWorkScript.Instance.FinRecvEvent += FinRecv;
            TransDTO transDTO = new TransDTO();
            transDTO.appName = "";
            transDTO.codestr = "100704|111111|";
            transDTO.ip = "192.168.13.85";
            transDTO.pflag = 9;
            transDTO.stockNo = "13";
            NetWorkScript.Instance.write(1,1,1,transDTO);
            Console.ReadKey();
        }
        public static void FinRecv()
        {
            Console.WriteLine("FinRecv!");
            Console.WriteLine(NetWorkScript.Instance.messageList[0].message.ToString());
        }
    }
}
