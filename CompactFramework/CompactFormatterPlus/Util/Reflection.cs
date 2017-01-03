using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;

namespace CompactFormatter.Util
{
    public class Reflection
    {
#if PocketPC
        [DllImport("CoreDll.dll", SetLastError = true)]
        public static extern IntPtr GetModuleHandle(IntPtr ModuleName);
        [DllImport("CoreDll.dll", SetLastError = true)]
        public static extern Int32 GetModuleFileName(IntPtr hModule, StringBuilder ModuleName, Int32 cch);
#endif

        public static string GetEntryAssembly()
        {
#if PocketPC
            StringBuilder sb = null;
            IntPtr hModule = GetModuleHandle(IntPtr.Zero);
            if (IntPtr.Zero != hModule)
            {
                sb = new StringBuilder(255);
                if (0 == GetModuleFileName(hModule, sb, sb.Capacity))
                {
                    sb = null;
                }
            }
            return sb.ToString();
#else
            return Assembly.GetEntryAssembly().Location;
#endif
        } 
    }
}
