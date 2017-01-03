using System;
using System.Collections.Generic;
using System.Text;

namespace CompactFormatter
{
    [Attributes.CustomSerializableAttribute]
    public class TransDTO
    {
        private string appname;

        public string AppName
        {
            get { return appname; }
            set { appname = value; }
        }
        private string ip;

        public string IP
        {
            get { return ip; }
            set { ip = value; }
        }
        private string codestr;

        public string CodeStr
        {
            get { return codestr; }
            set { codestr = value; }
        }
        private int pflag;

        public int pFlag
        {
            get { return pflag; }
            set { pflag = value; }
        }
        private string stockno;

        public string StockNo
        {
            get { return stockno; }
            set { stockno = value; }
        }
        private string remark;

        public string Remark
        {
            get { return remark; }
            set { remark = value; }
        }
    }
}
