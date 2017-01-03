using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NewMatServerCMD.tool
{
   public delegate void ExecuteDelegate();

   public class ExecutorPool
    {
       Mutex tex = new Mutex();
       private static ExecutorPool pool;

       public static ExecutorPool Instance { get { if (pool == null)pool=new ExecutorPool(); return pool; } }

       public void execute(ExecuteDelegate d) {
           lock (this)
           {
               tex.WaitOne();
               d();
               tex.ReleaseMutex();
           }
           
       }
    }
}
