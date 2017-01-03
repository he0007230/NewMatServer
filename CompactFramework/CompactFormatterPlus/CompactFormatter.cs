#define OBJECT_TABLE
#define COMPACT
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
 * $Id: CompactFormatter.cs 14 2004-08-26 09:08:59Z Angelo $
 * */
#endregion

using System;
using System.Collections;
using System.Reflection;
using CompactFormatter.Interfaces;
using CompactFormatter.Exception;
using CF = CompactFormatter;
using System.Runtime.InteropServices;
using DotNetRemoting;
using CompactFormatter.Util;
using System.Data;
using CompactFormatter.Surrogate;

namespace CompactFormatter
{
    public enum PayloadType
    {
        // Null reference
        NULL,
        // Primitive Types
        BOOLEAN, BYTE, CHAR, DECIMAL, SINGLE, DOUBLE, INT16, INT32,
        INT64, SBYTE, UINT16, UINT32, UINT64, DATETIME, STRING,
        // Arrays of Primitive values
        ARRAYOFBOOLEAN, ARRAYOFBYTE, ARRAYOFCHAR, ARRAYOFDECIMAL, ARRAYOFSINGLE,
        ARRAYOFDOUBLE, ARRAYOFINT16, ARRAYOFINT32, ARRAYOFINT64, ARRAYOFSBYTE,
        ARRAYOFUINT16, ARRAYOFUINT32, ARRAYOFUINT64, ARRAYOFDATETIME, ARRAYOFSTRING,
        ARRAYOFOBJECTS,
        // Objects (runtime or not)
        OBJECT,

        FAST_SERIALIZATION,
        // Objects which requested explicit OverrideSerialization.
        OVERRIDESERIALIZATION,
        // Objects which requested explicit SurrogateSerialization.
        SURROGATESERIALIZATION,
        // Assembly Metadata.
        ASSEMBLY,
        // Type Metadata.
        TYPE,
        // Object which was already sent.
        ALREADYSENT,
        // Enum type.
        ENUM
    };

    /// <summary>
    /// CompactFormatter.
    /// </summary>
    public class CompactFormatter : IDNR_Formatter
    {
        private Assembly CurrentAssembly;

        bool _CheckSerializableAtribute = false;

        public bool CheckSerializableAtribute
        {
            get { return _CheckSerializableAtribute; }
            set { _CheckSerializableAtribute = value; }
        }

        private Type CurrentType;
        ArrayList Types_NOT_For_FastSerializer;
        // private CFormatterMode remoteMode;
        /// <summary>
        /// The Framework version on the other side of the serialization stream;
        /// It should be ignored when this object is serializing, in fact who 
        /// serialize data doesn't know who will receive it (consider, for example
        /// storing data in a filesystem)
        /// </summary>
        private FrameworkVersion remoteVersion;

        /// <summary>
        /// The Framework on which CompactFormatter is running.
        /// It's the first thing sent over the wire when starting serialization
        /// of an object.
        /// </summary>
        private FrameworkVersion localVersion;

        private IStreamParser[] registeredParsers;

        public IStreamParser[] RegisteredParsers
        {
            get
            {
                return registeredParsers;
            }
        }

        /// <summary>
        /// RegisterStreamParser is used to register a new StreamParser object
        /// to CompactFormatter.
        /// These objects are used to transform data stream before sending it on
        /// the wire or after receiving it from the wire,
        /// The list of StreamParsers used by CompactFormatter are stored in a simple
        /// array, this means that, when we add a new StreamParser we must create
        /// a bigger array and copy the content of old one to the newly create.
        /// This means that registering a new StreamParser is slow but, occupy less
        /// memory space and is more efficient to read (since it's a simple array
        /// and not a Collection object).
        /// The overhead in registering is not a problem since usually only one
        /// StreamParser should be registered and this is done once, before starting
        /// serialization.
        /// </summary>
        /// <param name="parser">The IStreamParser object to register.</param>
        public void RegisterStreamParser(Interfaces.IStreamParser parser)
        {
            IStreamParser[] temp = new IStreamParser[registeredParsers.Length + 1];
            Array.Copy(registeredParsers, 0, temp, 0, registeredParsers.Length);
            temp[registeredParsers.Length] = parser;
            registeredParsers = temp;
        }

        /// <summary>
        /// DeregisterStreamParser is used to remove a StreamParser object from
        /// CompactFormatter.
        /// As its twin RegisterStreamParser it uses simple arrays and so it's 
        /// unefficient, but, as its twin, this function is rarely used and repaid
        /// by the gain in efficiency at runtime.
        /// </summary>
        /// <param name="parser">The IStreamParser object to deregister.</param>
        public void DeregisterStreamParser(Interfaces.IStreamParser parser)
        {
            int index = -1;
            for (int i = 0; i < registeredParsers.Length; i++)
            {
                if (registeredParsers[i].Equals(parser))
                {
                    index = i;
                    break;
                }
            }
            if (index != -1)
            {
                IStreamParser[] temp = new IStreamParser[registeredParsers.Length - 1];
                Array.Copy(registeredParsers, 0, temp, 0, index);
                Array.Copy(registeredParsers, index + 1, temp, index,
                    registeredParsers.Length - index - 1);
                registeredParsers = temp;
            }
        }

        /// <summary>
        /// Used to flush away tables.
        /// This is useful to make room when tables are grown too much and
        /// when several streams are used from the same serializer.
        /// </summary>
        void Reset()
        {
            AssemblyHashtable.Clear();
            ClassInspector.Clear();
            CurrentAssembly = null;
            CurrentType = null;
        }

        /// <summary>
        /// the list of types that shoul not go to the fast serializer
        /// </summary>
        /// <returns></returns>
        ArrayList GetTypes()
        {
            ArrayList types = new ArrayList();
            types.Add(typeof(bool));
            types.Add(typeof(byte));
            types.Add(typeof(char));
            types.Add(typeof(decimal));
            types.Add(typeof(float));
            types.Add(typeof(double));
            types.Add(typeof(Int16));
            types.Add(typeof(Int32));
            types.Add(typeof(Int64));
            types.Add(typeof(sbyte));
            types.Add(typeof(UInt16));
            types.Add(typeof(UInt32));
            types.Add(typeof(UInt64));
            types.Add(typeof(DateTime));
            types.Add(typeof(string));
            types.Add(typeof(bool[]));
            types.Add(typeof(byte[]));
            types.Add(typeof(char[]));
            types.Add(typeof(decimal[]));
            types.Add(typeof(float[]));
            types.Add(typeof(double[]));
            types.Add(typeof(Int16[]));
            types.Add(typeof(Int32[]));
            types.Add(typeof(Int64[]));
            types.Add(typeof(sbyte[]));
            types.Add(typeof(UInt16[]));
            types.Add(typeof(UInt32[]));
            types.Add(typeof(UInt64[]));
            types.Add(typeof(DateTime[]));
            types.Add(typeof(string[]));
            return types;
        }

        public CompactFormatter()
        {
            Types_NOT_For_FastSerializer = GetTypes();

            localVersion = Framework.Detect();
            // Usually two StreamParsers are already too much...
            registeredParsers = new Interfaces.IStreamParser[0];
            AssemblyHashtable = new Hashtable();
            OverriderTable = new Hashtable();
            TypesHashtable = new Hashtable();

            // For sure i need to add Assembly mscorlib
            CurrentAssembly = Assembly.Load("mscorlib");
            AssemblyHashtable.Add("mscorlib", CurrentAssembly);
        }

        ArrayList FastSerializerTypes;

        bool SerializeFast(System.IO.Stream serializationStream, object graph)
        {
            try
            {
                if (FastSerializersHashTable == null)
                    return false;

                if (!FastSerializerTypes.Contains(graph.GetType()))
                    return false;

                IFastSerializer fser = (IFastSerializer)FastSerializersHashTable[graph.GetType()];

                serializationStream.WriteByte((byte)PayloadType.FAST_SERIALIZATION);

                string name = fser.Name;
                byte[] array = new byte[name.Length * 2 + 4];
                Buffer.BlockCopy(BitConverter.GetBytes(name.Length * 2), 0, array, 0, 4);
                Buffer.BlockCopy(System.Text.Encoding.Unicode.GetBytes(name),
                    0, array, 4, name.Length * 2);

                serializationStream.Write(array, 0, array.Length);
                fser.Serialize(serializationStream, graph);
            }
            catch (System.Exception ex)
            {
                throw new System.Exception("SerializeFast :" + ex.Message);
            }

            return true;
        }

        object DeSerializeFast(System.IO.Stream serializationStream)
        {
            if (FastSerializersHashTableByName == null)
                throw new System.Exception("DeSerializeFast : FastSerializer not set");

            try
            {
                byte[] integer = new byte[4];
                serializationStream.Read(integer, 0, 4);
                int len = BitConverter.ToInt32(integer, 0);

                byte[] array = new byte[len];
                serializationStream.Read(array, 0, len);
                string FastSerializerName = System.Text.Encoding.Unicode.GetString(array, 0, array.Length);

                IFastSerializer fser = (IFastSerializer)FastSerializersHashTableByName[FastSerializerName];
                return fser.Deserialize(serializationStream);
            }
            catch (System.Exception ex)
            {
                throw new System.Exception("DeSerializeFast :" + ex.Message);
            }
        }

        private void innerSerialize(System.IO.Stream serializationStream, object graph)
        {
            // If object is null serialize it
            if (graph == null)
            {
                serializationStream.WriteByte((byte)PayloadType.NULL);
                return;
            }

            // can be DNR fastserialized? 
            if (SerializeFast(serializationStream, graph))
                return;

            Type t = graph.GetType();

            // If object is of primitive type simply serialize it.
            if (t.IsPrimitive || t.Equals(typeof(String)) ||
                t.Equals(typeof(DateTime)) || t.Equals(typeof(Decimal)))
            {
                #region Serialization of Primitive Types
                switch (t.ToString())
                {
                    case "System.Int32":
                        {
                            PrimitiveSerializer.Serialize(
                                (int)graph, serializationStream);
                            return;
                        }
                    case "System.String":
                        {
                            PrimitiveSerializer.Serialize(
                                (string)graph, serializationStream);
                            return;
                        }
                    case "System.Boolean":
                        {
                            PrimitiveSerializer.Serialize(
                                (bool)graph, serializationStream);
                            return;
                        }
                    case "System.SByte":
                        {
                            PrimitiveSerializer.Serialize(
                                (sbyte)graph, serializationStream);
                            return;
                        }
                    case "System.Byte":
                        {
                            PrimitiveSerializer.Serialize(
                                (byte)graph, serializationStream);
                            return;
                        }
                    case "System.Char":
                        {
                            PrimitiveSerializer.Serialize(
                                (char)graph, serializationStream);
                            return;
                        }
                    case "System.Int16":
                        {
                            PrimitiveSerializer.Serialize(
                                (short)graph, serializationStream);
                            return;
                        }
                    case "System.UInt16":
                        {
                            PrimitiveSerializer.Serialize(
                                (ushort)graph, serializationStream);
                            return;
                        }
                    case "System.UInt32":
                        {
                            PrimitiveSerializer.Serialize(
                                (uint)graph, serializationStream);
                            return;
                        }
                    case "System.Int64":
                        {
                            PrimitiveSerializer.Serialize(
                                (long)graph, serializationStream);
                            return;
                        }
                    case "System.UInt64":
                        {
                            PrimitiveSerializer.Serialize(
                                (ulong)graph, serializationStream);
                            return;
                        }
                    case "System.Single":
                        {
                            PrimitiveSerializer.Serialize(
                                (float)graph, serializationStream);
                            return;
                        }
                    case "System.Double":
                        {
                            PrimitiveSerializer.Serialize(
                                (double)graph, serializationStream);
                            return;
                        }
                    case "System.Decimal":
                        {
                            PrimitiveSerializer.Serialize(
                                (decimal)graph, serializationStream);
                            return;
                        }
                    case "System.DateTime":
                        {
                            PrimitiveSerializer.Serialize(
                                (DateTime)graph, serializationStream);
                            return;
                        }
                }
                #endregion
            }
            // If it's an array of objects
            if (t.IsArray)
            {
                #region Serialization of Arrays of primitive types
                switch (t.GetElementType().ToString())
                {
                    case "System.Byte":
                        {
                            ArraySerializer.SerializeArrayBytes((byte[])graph,
                                serializationStream);
                            return;
                        }
                    case "System.Boolean":
                        {
                            ArraySerializer.SerializeArrayBoolean((bool[])graph,
                                serializationStream);
                            return;
                        }
                    case "System.Char":
                        {
                            ArraySerializer.SerializeArrayChar((char[])graph,
                                serializationStream);
                            return;
                        }
                    case "System.Decimal":
                        {
                            ArraySerializer.SerializeArrayDecimal((decimal[])graph,
                                serializationStream);
                            return;
                        }
                    case "System.Single":
                        {
                            ArraySerializer.SerializeArraySingle((Single[])graph,
                                serializationStream);
                            return;
                        }
                    case "System.Double":
                        {
                            ArraySerializer.SerializeArrayDouble((Double[])graph,
                                serializationStream);
                            return;
                        }
                    case "System.Int16":
                        {
                            ArraySerializer.SerializeArrayShort((Int16[])graph,
                                serializationStream);
                            return;
                        }
                    case "System.Int32":
                        {
                            ArraySerializer.SerializeArrayInteger((Int32[])graph,
                                serializationStream);
                            return;
                        }
                    case "System.Int64":
                        {
                            ArraySerializer.SerializeArrayLong((Int64[])graph,
                                serializationStream);
                            return;
                        }
                    case "System.SByte":
                        {
                            ArraySerializer.SerializeArraySByte((SByte[])graph,
                                serializationStream);
                            return;
                        }
                    case "System.UInt16":
                        {
                            ArraySerializer.SerializeArrayUInt16((UInt16[])graph,
                                serializationStream);
                            return;
                        }
                    case "System.UInt32":
                        {
                            ArraySerializer.SerializeArrayUInt32((UInt32[])graph,
                                serializationStream);
                            return;
                        }
                    case "System.UInt64":
                        {
                            ArraySerializer.SerializeArrayUInt64((UInt64[])graph,
                                serializationStream);
                            return;
                        }
                    case "System.String":
                        {
                            ArraySerializer.SerializeArrayString((String[])graph,
                                serializationStream);
                            return;
                        }
                    case "System.DateTime":
                        {
                            ArraySerializer.SerializeArrayDateTime((DateTime[])graph,
                                serializationStream);
                            return;
                        }
                    default:
                        {
                            this.SerializeArrayObjects((Array)graph,
                                serializationStream);
                            return;
                        }
                }
                #endregion
            }

            if (t.IsEnum)
            {
                SerializeEnum(graph, serializationStream);
                return;
            }

            GetAllOverriders(t);

            // The object is not marked with Serializable attribute,
            // we must check if we've an overrider or a surrogate.
            // First let's check if we've a Overrider who can handle it.
            //Console.WriteLine(OverriderTable[t]);
            IOverrider overrider = (IOverrider)OverriderTable[t];
            if (overrider != null)
            {
                // We've an overrider registered for it!
                // Check if Type was already sent
                //int index = SerializedTypesList.IndexOf(graph.GetType());

                WriteAssemblyMetadata(serializationStream, graph.GetType().Assembly);
                WriteTypeMetadata(serializationStream, graph.GetType());
                serializationStream.WriteByte((byte)PayloadType.OBJECT);
                overrider.Serialize(this, serializationStream, graph);
                return;
            }

            //if (IsSerializable(t))
            {
                SerializeObject(serializationStream, graph);
                return;
            }
        }

        private void GetAllOverriders(Type t)
        {
            foreach (FieldInfo field in ClassInspector.InspectClass(t, _CheckSerializableAtribute))
            {
                object[] attributes = field.GetCustomAttributes(typeof(CF.Attributes.OverriderAttribute), false);

                foreach (CF.Attributes.OverriderAttribute attribute in attributes)
                {
                    if (!OverriderTable.Contains(field.FieldType))
                        AddOverrider(attribute.CustomSerializer, field.FieldType);
                }

                if (field.FieldType.IsSubclassOf(typeof(DataSet)) && (string.Compare(field.FieldType.FullName, "System.Data.DataSet") != 0))
                {
                    if (!OverriderTable.Contains(field.FieldType))
                    {
                        Type overrider = typeof(DataSetOverrider<>).MakeGenericType(field.FieldType);

                        AddOverrider(overrider, field.FieldType);
                    }
                }
            }
        }

        private bool IsSerializable(Type t)
        {
            return ((t.Attributes & TypeAttributes.Serializable) > 0);
        }

        private object innerDeserialize(System.IO.Stream serializationStream)
        {
            PayloadType objType = (PayloadType)serializationStream.ReadByte();

            switch (objType)
            {
                #region Deserialization of null reference
                case (PayloadType.NULL):
                    return null;
                #endregion

                case (PayloadType.FAST_SERIALIZATION):
                    return DeSerializeFast(serializationStream);

                #region Deserialization of AlreadySent objects

                #endregion
                #region Deserialization of Primitive Types
                case (PayloadType.BOOLEAN):
                    return PrimitiveSerializer.
                        DeserializeBoolean(serializationStream);
                case (PayloadType.BYTE):
                    return PrimitiveSerializer.
                        DeserializeByte(serializationStream);
                case (PayloadType.CHAR):
                    return PrimitiveSerializer.
                        DeserializeChar(serializationStream);
                case (PayloadType.DATETIME):
                    return PrimitiveSerializer.
                        DeserializeDateTime(serializationStream);
                case (PayloadType.DECIMAL):
                    return PrimitiveSerializer.
                        DeserializeDecimal(serializationStream);
                case (PayloadType.DOUBLE):
                    return PrimitiveSerializer.
                        DeserializeDouble(serializationStream);
                case (PayloadType.INT16):
                    return PrimitiveSerializer.
                        DeserializeInt16(serializationStream);
                case (PayloadType.INT32):
                    return PrimitiveSerializer.
                        DeserializeInt32(serializationStream);
                case (PayloadType.INT64):
                    return PrimitiveSerializer.
                        DeserializeInt64(serializationStream);
                case (PayloadType.SBYTE):
                    return PrimitiveSerializer.
                        DeserializeSByte(serializationStream);
                case (PayloadType.SINGLE):
                    return PrimitiveSerializer.
                        DeserializeSingle(serializationStream);
                case (PayloadType.STRING):
                    return PrimitiveSerializer.
                        DeserializeString(serializationStream);
                case (PayloadType.UINT16):
                    return PrimitiveSerializer.
                        DeserializeUInt16(serializationStream);
                case (PayloadType.UINT32):
                    return PrimitiveSerializer.
                        DeserializeUInt32(serializationStream);
                case (PayloadType.UINT64):
                    return PrimitiveSerializer.
                        DeserializeUInt64(serializationStream);
                #endregion

                #region Deserialization of Assembly
                case (PayloadType.ASSEMBLY):
                    {
                        //This is tricky: I can't return an Assembly object because
                        //this info simply means that this assembly should be stored
                        //in assembly list table, so, after reading this one, let's call
                        //again innerDeserialize and return it's answer.
                        ReadAssemblyMetadata(serializationStream);
                        return innerDeserialize(serializationStream);
                    }
                #endregion
                #region Deserialization of Type
                case (PayloadType.TYPE):
                    {
                        ReadTypeMetadata(serializationStream);
                        return innerDeserialize(serializationStream);
                    }
                #endregion
                #region Deserialization of Object
                case (PayloadType.OBJECT):
                    {
                        object answer = DeserializeObject(serializationStream);
                        return answer;
                    }
                #endregion
                #region Deserialization of Arrays
                case (PayloadType.ARRAYOFBYTE):
                    {
                        object answer = ArraySerializer.DeserializeArrayByte(serializationStream);
                        return answer;
                    }
                case (PayloadType.ARRAYOFBOOLEAN):
                    {
                        object answer = ArraySerializer.DeserializeArrayBoolean(serializationStream);
                        return answer;
                    }
                case (PayloadType.ARRAYOFCHAR):
                    {
                        object answer = ArraySerializer.DeserializeArrayChar(serializationStream);
                        return answer;
                    }
                case (PayloadType.ARRAYOFDECIMAL):
                    {
                        object answer = ArraySerializer.DeserializeArrayDecimal(serializationStream);
                        return answer;
                    }
                case (PayloadType.ARRAYOFSINGLE):
                    {
                        object answer = ArraySerializer.DeserializeArraySingle(serializationStream);
                        return answer;
                    }
                case (PayloadType.ARRAYOFDOUBLE):
                    {
                        object answer = ArraySerializer.DeserializeArrayDouble(serializationStream);
                        return answer;
                    }
                case (PayloadType.ARRAYOFINT16):
                    {
                        object answer = ArraySerializer.DeserializeArrayShort(serializationStream);
                        return answer;
                    }
                case (PayloadType.ARRAYOFINT32):
                    {
                        object answer = ArraySerializer.DeserializeArrayInteger(serializationStream);
                        return answer;
                    }
                case (PayloadType.ARRAYOFINT64):
                    {
                        object answer = ArraySerializer.DeserializeArrayLong(serializationStream);
                        return answer;
                    }
                case (PayloadType.ARRAYOFSBYTE):
                    {
                        object answer = ArraySerializer.DeserializeArraySByte(serializationStream);
                        return answer;
                    }
                case (PayloadType.ARRAYOFUINT16):
                    {
                        object answer = ArraySerializer.DeserializeArrayUInt16(serializationStream);
                        return answer;
                    }
                case (PayloadType.ARRAYOFUINT32):
                    {
                        object answer = ArraySerializer.DeserializeArrayUInt32(serializationStream);
                        return answer;
                    }
                case (PayloadType.ARRAYOFUINT64):
                    {
                        object answer = ArraySerializer.DeserializeArrayUInt64(serializationStream);
                        return answer;
                    }
                case (PayloadType.ARRAYOFSTRING):
                    {
                        object answer = ArraySerializer.DeserializeArrayString(serializationStream);
                        return answer;
                    }
                case (PayloadType.ARRAYOFDATETIME):
                    {
                        object answer = ArraySerializer.DeserializeArrayDateTime(serializationStream);
                        return answer;
                    }
                case (PayloadType.ARRAYOFOBJECTS):
                    {
                        object answer = this.DeserializeArrayObject(serializationStream);
                        return answer;
                    }
                #endregion
                #region Deserialization of Enums
                case (PayloadType.ENUM):
                    {
                        object answer = DeserializeEnum(serializationStream);
                        return answer;
                    }

                #endregion
            }
            return null;
        }

        #region ICFormatter Members

        /// <summary>
        /// This method is called by external users to serialize an object on 
        /// the stream.
        /// First of all it serialize the header and then call innerSerialize 
        /// methods.
        /// Before returning from this function, the serializer must empty all 
        /// tables.
        /// </summary>
        /// <param name="serializationStream"></param>
        /// <param name="graph"></param>
        public void Serialize(System.IO.Stream serializationStream, object graph)
        {
            serializationStream.WriteByte((byte)localVersion);
            System.IO.Stream stream = serializationStream;
            for (int i = 0; i < registeredParsers.Length; i++)
            {
                registeredParsers[i].InnerStream = stream;
                stream = registeredParsers[i];
            }

            innerSerialize(stream, graph);
            stream.Flush();
            Reset();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializationStream"></param>
        /// <returns></returns>
        public object Deserialize(System.IO.Stream serializationStream)
        {
            remoteVersion = (FrameworkVersion)serializationStream.ReadByte();
            System.IO.Stream stream = serializationStream;
            for (int i = registeredParsers.Length - 1; i >= 0; i--)
            {
                registeredParsers[i].InnerStream = stream;
                stream = registeredParsers[i];
            }

            object obj = innerDeserialize(stream);
            Reset();
            return obj;
        }

        #endregion

        #region Serialization Tables
        /// <summary>
        /// An ArrayList containing all previously serialized assemblies.
        /// NOTICE: We don't serialize assemblies, just their metadata!
        /// </summary>
        Hashtable AssemblyHashtable;

        Hashtable TypesHashtable;

        /// <summary>
        /// An hashtable containing all currently registered overrider.
        /// </summary>
        Hashtable OverriderTable;

        Hashtable FastSerializersHashTable;
        Hashtable FastSerializersHashTableByName;

        #endregion
        /// <summary>
        /// Add FastSerializer generated by "Serialization Studio"
        /// www.dotnetremoting.com
        /// </summary>
        /// <param name="fser"></param>
        public void AddFastSerializer(IFastSerializer fser)
        {
            fser.GenericFormatter = this;

            if (FastSerializerTypes == null)
            {
                FastSerializerTypes = new ArrayList();
            }

            if (FastSerializersHashTableByName == null)
            {
                FastSerializersHashTableByName = new Hashtable();
            }

            if (FastSerializersHashTable == null)
            {
                FastSerializersHashTable = new Hashtable();
            }

            if (FastSerializersHashTableByName.ContainsKey(fser.Name))
                throw new System.Exception("CompactFormatter.AddFastSerializer : the serializer " + fser.Name + " is already registered");

            foreach (Type t in fser.GetTypes())
            {
                if (Types_NOT_For_FastSerializer.Contains(t))
                    continue;

                if (!FastSerializerTypes.Contains(t))
                    FastSerializerTypes.Add(t);

                FastSerializersHashTable[t] = fser;

                FastSerializersHashTableByName[fser.Name] = fser;
            }
        }

        #region Table of Overriders

        public void AddOverrider(Type overrider)
        {
            // First of all check if it's an overrider (if it's marked with 
            // OverriderAttribute)
            object[] attributes = overrider.GetCustomAttributes(typeof(Attributes.OverriderAttribute), false);
            if (attributes.Length != 0)
            {
                // If it is, i need to register it.
                Attributes.OverriderAttribute attribute = (Attributes.OverriderAttribute)overrider.
                    GetCustomAttributes(typeof(Attributes.OverriderAttribute),
                    false)[0];

                AddOverrider(overrider, attribute.CustomSerializer);
            }
            else throw new RegisterOverriderException(overrider);
        }

        private void AddOverrider(Type overrider, Type customSerializer)
        {
            Object formatter = Activator.CreateInstance(overrider);

            // Let's add not the type, but an instance of it
            // INFO: This requires Overrider to have parameterless constructor
            // INFO: obviously
            OverriderTable.Add(customSerializer, formatter);
        }

        #endregion

        #region Type Serialization

        internal void WriteTypeMetadata(System.IO.Stream stream, Type type)
        {
            // Now the assembly has been sent.
            stream.WriteByte((byte)PayloadType.TYPE);
            // now i need to send class FullName
            String classname = type.FullName;
            byte[] array = new byte[classname.Length * 2 + 4];

            Buffer.BlockCopy(BitConverter.GetBytes(classname.Length * 2), 0, array, 0, 4);
            Buffer.BlockCopy(System.Text.Encoding.Unicode.GetBytes(classname),
                0, array, 4, classname.Length * 2);

            stream.Write(array, 0, array.Length);
        }

        public void ReadTypeMetadata(System.IO.Stream serializationStream)
        {
            string typename = null;

            byte[] integer = new byte[4];
            serializationStream.Read(integer, 0, 4);
            int len = BitConverter.ToInt32(integer, 0);

            byte[] array = new byte[len];
            serializationStream.Read(array, 0, len);
            typename = System.Text.Encoding.Unicode.GetString(array, 0, array.Length);

            try
            {
                Type t = (Type)TypesHashtable[typename];

                if (t == null)
                {
                    t = CurrentAssembly.GetType(typename);
                    if (t == null)
                        throw new System.Exception("ReadTypeMetadata : CurrentAssembly.GetType() failed for " + typename);
                }

                TypesHashtable[typename] = t;
                CurrentType = t;
            }
            catch (System.Exception)
            {
                CurrentType = null;
                throw new TypeSerializationException(
                    "ReadTypeMetadata() : Unable to load type " + typename);
            }
        }

        #endregion

        #region Assembly Serialization

        public void ReadAssemblyMetadata(System.IO.Stream serializationStream)
        {
            byte[] integer = new byte[4];
            serializationStream.Read(integer, 0, 4);
            int len = BitConverter.ToInt32(integer, 0);

            byte[] array = new byte[len];
            serializationStream.Read(array, 0, len);
            string AssemblyShortName = System.Text.Encoding.Unicode.GetString(array, 0, array.Length);

            try
            {
                foreach (Interfaces.IOverrider ob in OverriderTable.Values)
                {
                    object[] atrr = ob.GetType().GetCustomAttributes(typeof(Attributes.OverriderAttribute), false);

                    if (atrr == null)
                        continue;

                    Attributes.OverriderAttribute over_attrib = (Attributes.OverriderAttribute)atrr[0];
                    Type AttribType = over_attrib.CustomSerializer;
                    string name = AttribType.Assembly.GetName().Name;
                    if (AssemblyShortName.IndexOf(name) != -1)
                    {
                        string Name = AttribType.Assembly.FullName;
                        CurrentAssembly = Assembly.Load(Name);
                        if (!AssemblyHashtable.ContainsKey(name))
                            AssemblyHashtable.Add(name, CurrentAssembly);
                        return;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("ReadAssemblyMetadata:" + ex.Message);
                throw new AssemblySerializationException(
                    "ReadAssemblyMetadata()_1: Unable to load assembly " + ex.Message);
            }

            try
            {
                CurrentAssembly = GetAssembly(AssemblyShortName);
                AssemblyHashtable[AssemblyShortName] = CurrentAssembly;
                return;
            }
            catch (System.IO.FileNotFoundException err)
            {
                Console.WriteLine(err);
                throw new AssemblySerializationException(
                    "ReadAssemblyMetadata()_2:Unable to load assembly " + AssemblyShortName + " file not found!");
            }
        }

        private Assembly GetAssembly(string assemblyshortname)
        {
            Assembly asm = null;

            try
            {
                asm = Assembly.Load(assemblyshortname);
                if (asm != null)
                    return asm;
            }
            catch
            { }

#if !PocketPC
            try
            {
                asm = Assembly.LoadWithPartialName(assemblyshortname);
                if (asm != null)
                    return asm;
            }
            catch
            { }
#endif

            string location = Reflection.GetEntryAssembly();
            string path = System.IO.Path.GetDirectoryName(location);

            foreach (string filename in System.IO.Directory.GetFiles(path, "*.dll"))
            {
                try
                {
                    string str_temp = System.IO.Path.GetFileNameWithoutExtension(filename);

                    if (assemblyshortname == str_temp)
                    {
                        asm = Assembly.LoadFrom(filename);
                        if (asm != null)
                            return asm;
                    }
                }
                catch
                {
                }
            }

            return null;
        }

        public string WriteAssemblyMetadata(System.IO.Stream stream, Assembly assembly)
        {
            string name = assembly.GetName().Name;

            stream.WriteByte((byte)PayloadType.ASSEMBLY);
            byte[] array = new byte[name.Length * 2 + 4];
            Buffer.BlockCopy(BitConverter.GetBytes(name.Length * 2), 0, array, 0, 4);
            Buffer.BlockCopy(System.Text.Encoding.Unicode.GetBytes(name),
                0, array, 4, name.Length * 2);

            stream.Write(array, 0, array.Length);
            return name;
        }

        #endregion

        #region Object Serialization
        internal Object DeserializeObject(System.IO.Stream stream)
        {
            Type t = CurrentType;

            GetAllOverriders(t);

            // First of all check if we've a Overrider who can handle it.
            IOverrider overrider = (IOverrider)OverriderTable[t];
            if (overrider != null)
            {
                Object obj = overrider.Deserialize(this, stream);
                return obj;
            }

            // The first thing to check is if the type is marked as serializable:
            //if (IsSerializable(t))
            {
                // It is marked, obviously it can't request Overriders or
                // Surrogates because otherwise the PayloadType couldn't be OBJECT
                // Deserialize it automatically
                Object obj = Activator.CreateInstance(t);
                return populateObject(stream, obj);
            }

            //throw new Exception.SerializationException("Unable to deserialize (23) " + t.Name);
        }

        /// <summary>
        /// This method is called wherever an object, marked with serializable
        /// attribute, is serialized.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="obj"></param>
        internal void SerializeObject(System.IO.Stream stream, Object obj)
        {
            WriteAssemblyMetadata(stream, obj.GetType().Assembly);
            WriteTypeMetadata(stream, obj.GetType());
            stream.WriteByte((byte)PayloadType.OBJECT);
            // CFormatter is in portable mode, i need to declare number of
            // fields and name for each field (Is GetHashCode on name enough?).
            FieldInfo[] list = ClassInspector.InspectClass(obj.GetType(), _CheckSerializableAtribute);
            innerSerialize(stream, list.Length);
            for (int i = 0; i < list.Length; i++)
            {
                // Here the field value is set.
                innerSerialize(stream, list[i].Name);
                innerSerialize(stream, list[i].GetValue(obj));
            }
        }

        #endregion

        /// <summary>
        /// This inner method is used during deserialization phase to populate
        /// a previously instantiated object (through Activator or a surrogate) 
        /// </summary>
        /// <param name="graph">The object instantiated but still uninitialized
        /// </param>
        /// <returns>graph object with all fields correctly set</returns>
        private Object populateObject(System.IO.Stream stream, Object graph)
        {
            // CFormatter is in portable mode, i need to declare number of
            // fields and name for each field (Is GetHashCode on name enough?).
            FieldInfo[] list = ClassInspector.InspectClass(graph.GetType(), _CheckSerializableAtribute);
            ArrayList a = new ArrayList(list);
            int length = (int)innerDeserialize(stream);
            for (int i = 0; i < length; i++)
            {
                // Here the field value is set.
                String name = (String)innerDeserialize(stream);
                for (int j = 0; j < a.Count; j++)
                {
                    if (((FieldInfo)a[j]).Name.Equals(name))
                    {
                        object obj = innerDeserialize(stream);
                        list[i].SetValue(graph, obj);
                        a.RemoveAt(j);
                        break;
                    }
                }
            }
            return graph;
        }

        private Object DeserializeEnum(System.IO.Stream stream)
        {
            stream.ReadByte();
            ReadAssemblyMetadata(stream);

            stream.ReadByte();
            ReadTypeMetadata(stream);
            Type t = CurrentType;

            byte[] buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            long l = (long)BitConverter.ToInt32(buffer, 0);
            return Enum.ToObject(t, l);
        }

        private void SerializeEnum(Object value, System.IO.Stream stream)
        {
            // At this point Assembly and the type are already sent, 
            // i just have to send the index.
            stream.WriteByte((byte)PayloadType.ENUM);

            WriteAssemblyMetadata(stream, value.GetType().Assembly);
            WriteTypeMetadata(stream, value.GetType());

            byte[] buffer = new byte[4];
            Buffer.BlockCopy(BitConverter.GetBytes((int)value), 0, buffer, 0, 4);
            stream.Write(buffer, 0, 4);
        }

        private void SerializeArrayObjects(Array array, System.IO.Stream serializationStream)
        {
            serializationStream.WriteByte((byte)PayloadType.ARRAYOFOBJECTS);

            WriteAssemblyMetadata(serializationStream, array.GetType().Assembly);
            WriteTypeMetadata(serializationStream, array.GetType().GetElementType());

            int length = array.Length;
            // Writing array length as Integer (in bytes)
            byte[] buffer = new byte[4];
            Buffer.BlockCopy(BitConverter.GetBytes(length), 0, buffer, 0, 4);
            serializationStream.Write(buffer, 0, 4);
            //Writing sequence of chars
            for (int i = 0; i < array.Length; i++)
            {
                this.innerSerialize(serializationStream, array.GetValue(i));
            }
        }

        private Array DeserializeArrayObject(System.IO.Stream serializationStream)
        {
            serializationStream.ReadByte();
            ReadAssemblyMetadata(serializationStream);

            serializationStream.ReadByte();
            ReadTypeMetadata(serializationStream);
            Type t = CurrentType;

            byte[] buffer = new byte[4];
            serializationStream.Read(buffer, 0, 4);
            int length = BitConverter.ToInt32(buffer, 0);
            // Now we've the size in bytes.
            Array answer = Array.CreateInstance(t, length);
            for (int i = 0; i < length; i++)
            {
                answer.SetValue(innerDeserialize(serializationStream), i);
            }
            return answer;
        }

        public string Name
        {
            get { return "CompactFormatterPlus "; }
        }
    }
}
