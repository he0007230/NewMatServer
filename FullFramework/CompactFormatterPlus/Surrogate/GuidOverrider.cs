using System;
using System.Collections.Generic;
using System.Text;

namespace CompactFormatter.Surrogate
{
    [Attributes.Overrider(typeof(System.Guid))]
    public class GuidOverrider : Interfaces.IOverrider
    {
        public void Serialize(CompactFormatter parent, System.IO.Stream serializationStream, object graph)
        {
            Guid g = (Guid)graph;
            byte[] ByteBuff = g.ToByteArray();
            byte[] bar_len_arr = BitConverter.GetBytes((Int32)ByteBuff.Length);
            serializationStream.Write(bar_len_arr, 0, bar_len_arr.Length);
            serializationStream.Write(ByteBuff, 0, ByteBuff.Length);
        }

        public object Deserialize(CompactFormatter parent, System.IO.Stream serializationStream)
        {
            byte[] barr_len = new byte[4];
            serializationStream.Read(barr_len, 0, 4);
            Int32 Len = BitConverter.ToInt32(barr_len, 0);
            byte[] ByteBuff = new byte[Len];
            serializationStream.Read(ByteBuff, 0, Len);
            return new Guid(ByteBuff);
        }
    }
}
