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
 * $Id: SerializableAttribute.cs 1 2004-08-13 18:29:52Z Angelo $
 * */
#endregion

using System;

namespace CompactFormatter.Attributes
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct 
		 | AttributeTargets.Delegate)]
	public class CustomSerializableAttribute : System.Attribute
	{

		private bool customSerializable;
		private Type surrogate;
		private Type overrideSerialization;

		public Type OverrideSerialization
		{
			get{return overrideSerialization;}
			set{overrideSerialization = value;}
		}

		public Type Surrogate
		{
			get{return surrogate;}
			set{surrogate = value;}
		}

		/// <summary>
		/// This property is a named parameter used to select custom serialization:
		/// If it's set to true, then the object is self-serializable and must
		/// use custom serialization.
		/// It's false by default.
		/// </summary>
		public bool Custom
		{
			get{return customSerializable;}
			set{customSerializable = value;	}
		}

		/// <summary>
		/// 
		/// </summary>
		public CustomSerializableAttribute()
		{
			this.surrogate = null;
			this.overrideSerialization = null;
			this.Custom = false;
		}
	}
}
