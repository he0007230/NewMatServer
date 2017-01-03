using System;
using CompactFormatter.Surrogate;

namespace CompactFormatter
{
    public class CompactFormatterPlus : CompactFormatter
    {
        public CompactFormatterPlus()
            : base()
        {
            AddOverrider(typeof(DataSetOverrider<System.Data.DataSet>));
            AddOverrider(typeof(DataTableOverrider));
            AddOverrider(typeof(GuidOverrider));
        }
    }
}
