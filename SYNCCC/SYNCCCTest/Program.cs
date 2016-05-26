using System;
using System.Collections.Generic;
using System.Text;
using SYNCCC;
using transprot.dto;

namespace SYNCCCTest
{
    class Program
    {
        static void Main(string[] args)
        {
            NetWorkScript.Instance.init("192.168.3.119",10005);
            TransDTO transDTO = new TransDTO();
            transDTO.appName = "Test";
            transDTO.codestr = "100704|13|";
            transDTO.ip = "192.168.13.85";
            transDTO.pflag = 9;
            transDTO.stockNo = "13";
            NetWorkScript.Instance.write(1, 1, 1, transDTO);
            Console.WriteLine(NetWorkScript.Instance.messageList[0].message.ToString());
            Console.ReadKey();
        }
    }
}
