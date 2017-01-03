using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AceNetFrameWork.ace.auto
{
   public class SocketModel
    {
       public int type { get; set; }
       public int area { get; set; }
       public int command { get; set; }
       public object message { get; set; }

       public SocketModel() { }

       public SocketModel(int t, int a, int c, object o) {
           this.type = t;
           this.area = a;
           this.command = c;
           this.message = o;
       }

       public T getMessage<T>(){
        return(T)message;
       }

       public string ts() {
           return type + "-----" + area +"------"+ command+"----" + message;
       }
    }
}
