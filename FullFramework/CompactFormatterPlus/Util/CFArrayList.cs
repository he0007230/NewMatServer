using System;
using System.Collections;
using System.Text;

namespace CompactFormatter
{
    public  class CFArrayList : ArrayList
    {
        public override int Add(object value)
        {
            if (this.Contains(value))
                return - 1;
            return base.Add(value);
        }
    }
}
