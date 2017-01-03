#region LGPL License
/* 
 * CompactFormatter: A generic formatter for the .NET Compact Framework
 * Copyright (C) 2004  Angelo Scotto (scotto_a@hotmail.com)
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 * 
 * $Id$
 * */
#endregion

using System;
using System.Collections;
using System.Data;
using CompactFormatter;
using DotNetRemoting;

namespace CompactFormatter.Surrogate
{
    [Attributes.Overrider(typeof(System.Data.DataTable))]
    public class DataTableOverrider : Interfaces.IOverrider
    {
        public void Serialize(CompactFormatter parent, System.IO.Stream serializationStream, object graph)
        {
            DataTable dt = (DataTable)graph;
            byte[] barr = AdoNetHelper.SerializeDataTable(dt);
            byte[] bar_len_arr = BitConverter.GetBytes((Int32)barr.Length);
            serializationStream.Write(bar_len_arr, 0, bar_len_arr.Length);
            serializationStream.Write(barr, 0, barr.Length);
        }

        public object Deserialize(CompactFormatter parent, System.IO.Stream serializationStream)
        {
            byte[] barr_len = new byte[4];
            serializationStream.Read(barr_len, 0, 4);
            Int32 Len = BitConverter.ToInt32(barr_len, 0);
            byte[] barr = new byte[Len];
            serializationStream.Read(barr, 0, Len);
            return AdoNetHelper.DeserializeDataTable(barr);
        }
    }
}
