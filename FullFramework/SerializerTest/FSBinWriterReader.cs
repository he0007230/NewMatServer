using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DotNetRemoting
{
    public class FSBinWriter
    {
        BinaryWriter _bw;

        public FSBinWriter(Stream s)
        {
            _bw = new BinaryWriter(s);
        }

        public void Write(decimal value)
        {
            int[] answer = Decimal.GetBits(value);
            byte[] ans = new byte[16];
            Buffer.BlockCopy(answer, 0, ans, 0, 16);
            _bw.Write(ans, 0, 16);
        }

        public void Write(string value)
        {
            byte b = 0;
            if (value == null)
            {
                Write(b);
                return;
            }
            b = 1;
            _bw.Write(b);

            _bw.Write(value);
        }

        public void WriteGenericArray(Array Src, int StartPosInBytes, int BytesToWrite)
        {
            byte[] barr = new byte[BytesToWrite];
            Buffer.BlockCopy(Src, 0, barr, 0, BytesToWrite);
            _bw.Write(barr, 0, BytesToWrite);
        }

        public void Close()
        {
            _bw.Close();
        }

        public void Flush()
        {
            _bw.Flush();
        }

        public long Seek(int offset, SeekOrigin origin)
        {
            return _bw.Seek(offset, origin);
        }

        public void Write(bool value)
        {
            _bw.Write(value);
        }

        public void Write(byte value)
        {
            _bw.Write(value);
        }

        public void Write(byte[] buffer)
        {
            _bw.Write(buffer);
        }

        public void Write(char ch)
        {
            _bw.Write(ch);
        }

        public void Write(char[] chars)
        {
            _bw.Write(chars);
        }

        public void Write(double value)
        {
            _bw.Write(value);
        }

        public void Write(float value)
        {
            _bw.Write(value);
        }

        public void Write(int value)
        {
            _bw.Write(value);
        }

        public void Write(long value)
        {
            _bw.Write(value);
        }

        public void Write(sbyte value)
        {
            _bw.Write(value);
        }

        public void Write(short value)
        {
            _bw.Write(value);
        }

        public void Write(uint value)
        {
            _bw.Write(value);
        }

        public void Write(ulong value)
        {
            _bw.Write(value);
        }

        public void Write(ushort value)
        {
            _bw.Write(value);
        }

        public void Write(byte[] buffer, int index, int count)
        {
            _bw.Write(buffer, index, count);
        }

        public void Write(char[] chars, int index, int count)
        {
            _bw.Write(chars, index, count);
        }

        protected void Write7BitEncodedInt(int value)
        {
            _bw.Write(value);
        }
    }

    public class FSBinReader
    {
        BinaryReader _br;

        public FSBinReader(Stream s)
        {
            _br = new BinaryReader(s);
        }

        public void ReadGenericArray(Array ar, int BytesToRead)
        {
            byte[] barr = new byte[BytesToRead];
            _br.Read(barr, 0, BytesToRead);
            Buffer.BlockCopy(barr, 0, ar, 0, BytesToRead);
        }

        public string ReadString()
        {
            if (ReadByte() == 0)
                return null;

            return _br.ReadString();
        }

        public decimal ReadDecimal()
        {
            int[] array = new int[4];
            byte[] ans = new byte[16];
            _br.Read(ans, 0, 16);
            Buffer.BlockCopy(ans, 0, array, 0, 16);
            return new Decimal(array);
        }

        public void Close()
        {
            _br.Close();
        }

        public int PeekChar()
        {
            return _br.PeekChar();
        }

        public int Read()
        {
            return _br.Read();
        }

        public int Read(byte[] buffer, int index, int count)
        {
            return _br.Read(buffer, index, count);
        }

        public int Read(char[] buffer, int index, int count)
        {
            return _br.Read(buffer, index, count);
        }

        public bool ReadBoolean()
        {
            return _br.ReadBoolean();
        }

        public byte ReadByte()
        {
            return _br.ReadByte();
        }

        public byte[] ReadBytes(int count)
        {
            return _br.ReadBytes(count);
        }

        public char ReadChar()
        {
            return _br.ReadChar();
        }

        public char[] ReadChars(int count)
        {
            return _br.ReadChars(count);
        }

        public double ReadDouble()
        {
            return _br.ReadDouble();
        }

        public short ReadInt16()
        {
            return _br.ReadInt16();
        }

        public int ReadInt32()
        {
            return _br.ReadInt32();
        }

        public long ReadInt64()
        {
            return _br.ReadInt64();
        }

        public sbyte ReadSByte()
        {
            return _br.ReadSByte();
        }

        public float ReadSingle()
        {
            return _br.ReadSingle();
        }

        public ushort ReadUInt16()
        {
            return _br.ReadUInt16();
        }

        public uint ReadUInt32()
        {
            return _br.ReadUInt32();
        }

        public ulong ReadUInt64()
        {
            return _br.ReadUInt64();
        }
    }
}
