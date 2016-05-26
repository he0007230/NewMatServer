using System;
using System.Collections.Generic;
using System.Text;

namespace transprot.dto
{
    [Serializable]
    public class TransDTO
    {
        public string appName { get; set; }
        public string ip { get; set; }
        public string codestr { get; set; }
        public int pflag { get; set; }
        public string stockNo { get; set; }
    }
}
