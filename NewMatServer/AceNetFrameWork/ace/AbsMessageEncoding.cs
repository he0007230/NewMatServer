using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AceNetFrameWork.ace
{
   public abstract class AbsMessageEncoding
   {
      public abstract byte[] Encode(object value);
      public abstract object Decode(byte[] value);

    }
}
