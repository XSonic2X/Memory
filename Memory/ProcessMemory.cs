using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Memory
{
    public delegate void Log(string log);
    public class ProcessMemory
    {
        public ProcessMemory(IntorProcc iP)
        {
            intorProcc = iP;
            if (intorProcc.processModule == null)
            { baseAddress = intorProcc.process.MainModule.BaseAddress; }
            else { baseAddress = intorProcc.processModule.BaseAddress; }
            CheckProcess();
        }
        public event Log log;
        public IntorProcc intorProcc;
        public IntPtr baseAddress { get; set; }
        public IntPtr processHandle { get; set; }
        /// <summary>
        /// Финальный адрес
        /// </summary>
        /// <param name="BaseAddress"></param>
        /// <param name="BiasAddress"></param>
        /// <returns></returns>
        public IntPtr FinalAddress(IntPtr BaseAddress, int[] BiasAddress)
        {
#if x86
            for (int i = 0; i < BiasAddress.Count() - 1; i++)
            {
                BaseAddress = (IntPtr)ReadInt32(IntPtr.Add(BaseAddress, BiasAddress[i]));
            }
            return BaseAddress + BiasAddress[Convert.ToInt32(BiasAddress.Count() - 1)];
#else
            for (int i = 0; i < BiasAddress.Count() - 1; i++)
            {
                BaseAddress = (IntPtr)ReadInt64(IntPtr.Add(BaseAddress, BiasAddress[i]));
            }
            return BaseAddress + BiasAddress[Convert.ToInt32(BiasAddress.Count() - 1)];
#endif
        }
        public IntPtr FinalAddress(int[] BiasAddress) => FinalAddress(baseAddress, BiasAddress);
        public List<IntPtr> SignaturesSearch(Signatures signatures,int start , int Size)
        {
            List <IntPtr> iP = new List <IntPtr>();
            for (int i = start, j = 0 ; i < Size; i++)
            {
                if (signatures.Test(ReadByte((IntPtr)i),j))
                {
                    j++;
                    if (j >= signatures.sig.Length)
                    { 
                        iP.Add((IntPtr)i-j);
                        j = 0;
                    }
                    continue;
                }
                if (j > 0)
                {
                    i -= j;
                    j = 0;
                }
            }
            return iP;
        }
        public bool CheckProcess()
        {
            if (intorProcc.End) 
            {
                log?.Invoke("Процесс не работает.");
                return false; 
            }
            processHandle = OpenProcess(2035711U, false, intorProcc.ID);
            if (processHandle == IntPtr.Zero) 
            { 
                log?.Invoke("Процесс не был найден. Пожалуйста, проверьте и повторите попытку"); 
                return false;
            }
            return true;
        }
        /// <summary>
        /// Основа чтения
        /// </summary>
        /// <param name="pOffset"></param>
        /// <param name="pSize"></param>
        /// <returns></returns>
        public byte[] ReadByteArray(IntPtr pOffset, uint pSize)
        {
            if (intorProcc.End) { return null; }
            try
            {
                uint flNewProtect;
                VirtualProtectEx(processHandle, pOffset, (UIntPtr)pSize, 4U, out flNewProtect);
                byte[] array = new byte[pSize];
                ReadProcessMemory(processHandle, pOffset, array, pSize, 0U);
                VirtualProtectEx(processHandle, pOffset, (UIntPtr)pSize, flNewProtect, out flNewProtect);
                return array;
            }
            catch (Exception ex)
            {
                log?.Invoke("Error: ReadByteArray" + ex.ToString());
                return null;
            }
        }
        public string ReadStringUnicode(IntPtr pOffset, uint pSize)
        {
            if (intorProcc.End) { return ""; }
            try
            {
                byte[] array  = ReadByteArray(pOffset, pSize);
                if (array == null) { return ""; }
                return Encoding.Unicode.GetString(array, 0, (int)pSize);
            }
            catch (Exception ex)
            {
                log?.Invoke("Error: ReadStringUnicode" + ex.ToString());
                return "";
            }
        }
        public string ReadStringASCII(IntPtr pOffset, uint pSize)
        {
            if (intorProcc.End) { return ""; }
            try
            {
                byte[] array = ReadByteArray(pOffset, pSize);
                if (array == null) { return ""; }
                return Encoding.ASCII.GetString(array, 0, (int)pSize);
            }
            catch (Exception ex)
            {
                log?.Invoke("Error: ReadStringASCII" + ex.ToString());
                return "";
            }
        }
        public char ReadChar(IntPtr pOffset)
        {
            if (intorProcc.End) { return ' '; }
            try
            {
                byte[] array = ReadByteArray(pOffset, 1U);
                if (array == null) { return ' '; }
                return BitConverter.ToChar(array, 0);
            }
            catch (Exception ex)
            {
                log?.Invoke("Error: ReadChar" + ex.ToString());
                return ' ';
            }
        }
        public bool ReadBoolean(IntPtr pOffset)
        {
            if (intorProcc.End) { return false; }
            try
            {
                byte[] array = ReadByteArray(pOffset, 1U);
                if (array == null) { return false; }
                return BitConverter.ToBoolean(array, 0);
            }
            catch (Exception ex)
            {
                log?.Invoke("Error: ReadByte" + ex.ToString());
                return false;
            }
        }
        public byte ReadByte(IntPtr pOffset)
        {
            if (intorProcc.End) { return 0; }
            try
            {
                byte[] array = ReadByteArray(pOffset, 1U);
                if (array == null) { return 0; }
                return array[0];
            }
            catch (Exception ex)
            {
                log?.Invoke("Error: ReadByte" + ex.ToString());
                return 0;
            }
        }
        public short ReadInt16(IntPtr pOffset)
        {
            if (intorProcc.End) { return 0; }
            try
            {
                byte[] array = ReadByteArray(pOffset, 2U);
                if (array == null) { return 0; }
                return BitConverter.ToInt16(array, 0);
            }
            catch (Exception ex)
            {
                log?.Invoke("Error: ReadInt16" + ex.ToString());
                return 0;
            }
        }
        public short ReadShort(IntPtr pOffset)
        {
            if (intorProcc.End) { return 0; }
            try
            {
                byte[] array = ReadByteArray(pOffset, 2U);
                if (array == null) { return 0; }
                return BitConverter.ToInt16(array, 0);
            }
            catch (Exception ex)
            {
                log?.Invoke("Error: ReadShort" + ex.ToString());
                return 0;
            }
        }
        public int ReadInt32(IntPtr pOffset)
        {
            if (intorProcc.End) { return 0; }
            try
            {
                byte[] array = ReadByteArray(pOffset, 4U);
                if (array == null) { return 0; }
                return BitConverter.ToInt32(array, 0);
            }
            catch (Exception ex)
            {
                log?.Invoke("Error: ReadInt32" + ex.ToString());
                return 0;
            }
        }
        public long ReadInt64(IntPtr pOffset)
        {
            if (intorProcc.End) { return 0l; }
            try
            {
                byte[] array = ReadByteArray(pOffset, 8U);
                if (array == null) { return 0; }
                return BitConverter.ToInt64(array, 0);
            }
            catch (Exception ex)
            {
                log?.Invoke("Error: ReadInt64" + ex.ToString());
                return 0l;
            }
        }
        public long ReadLong(IntPtr pOffset)
        {
            if (intorProcc.End) { return 0l; }
            try
            {
               byte[] array = ReadByteArray(pOffset, 8U);
                if (array == null) { return 0; }
                return BitConverter.ToInt64(array, 0);
            }
            catch (Exception ex)
            {
                log?.Invoke("Error: ReadLong" + ex.ToString());
                return 0l;
            }
        }
        public ushort ReadUInt16(IntPtr pOffset)
        {
            if (intorProcc.End) { return 0; }
            try
            {
                byte[] array = ReadByteArray(pOffset, 2U);
                if (array == null) { return 0; }
                return BitConverter.ToUInt16(array, 0);
            }
            catch (Exception ex)
            {
                log?.Invoke("Error: ReadUInt16" + ex.ToString());
                return 0;
            }
        }
        public ushort ReadUShort(IntPtr pOffset)
        {
            if (intorProcc.End) { return 0; }
            try
            {
                byte[] array = ReadByteArray(pOffset, 2U);
                if (array == null) { return 0; }
                return BitConverter.ToUInt16(array, 0);
            }
            catch (Exception ex)
            {
                log?.Invoke("Error: ReadUShort" + ex.ToString());
                return 0;
            }
        }
        public uint ReadUInt32(IntPtr pOffset)
        {
            if (intorProcc.End) { return 0u; }
            try
            {
                byte[] array = ReadByteArray(pOffset, 4U);
                if (array == null) { return 0; }
                return BitConverter.ToUInt32(array, 0);
            }
            catch (Exception ex)
            {
                log?.Invoke("Error: ReadUInt32" + ex.ToString());
                return 0u;
            }
        }
        public uint ReadUInteger(IntPtr pOffset)
        {
            if (intorProcc.End) { return 0u; }
            try
            {
                byte[] array = ReadByteArray(pOffset, 4U);
                if (array == null) { return 0; }
                return BitConverter.ToUInt32(array, 0);
            }
            catch (Exception ex)
            {
                log?.Invoke("Error: ReadUInteger" + ex.ToString());
                return 0u;
            }
        }
        public ulong ReadUInt64(IntPtr pOffset)
        {
            if (intorProcc.End) { return 0ul; }
            try
            {
                byte[] array = ReadByteArray(pOffset, 8U);
                if (array == null) { return 0; }
                return BitConverter.ToUInt64(array, 0);
            }
            catch (Exception ex)
            {
                log?.Invoke("Error: ReadUInt64" + ex.ToString());
                return 0ul;
            }
        }
        public ulong ReadULong(IntPtr pOffset)
        {
            if (intorProcc.End) { return 0ul; }
            try
            {
                byte[] array = ReadByteArray(pOffset, 8U);
                if (array == null) { return 0; }
                return BitConverter.ToUInt64(array, 0);
            }
            catch (Exception ex)
            {
                log?.Invoke("Error: ReadULong" + ex.ToString());
                return 0ul;
            }
        }
        public float ReadFloat(IntPtr pOffset)
        {
            if (intorProcc.End) { return 0f; }
            try
            {
                byte[] array = ReadByteArray(pOffset, 4U);
                if (array == null) { return 0f; }
                return BitConverter.ToSingle(array, 0);
            }
            catch (Exception ex)
            {
                log?.Invoke("Error: ReadFloat" + ex.ToString());
                return 0f;
            }
        }
        public double ReadDouble(IntPtr pOffset)
        {
            if (intorProcc.End) { return 0d; }
            try
            {
                byte[] array = ReadByteArray(pOffset, 8U);
                if (array == null) { return 0d; }
                return BitConverter.ToDouble(array, 0);
            }
            catch (Exception ex)
            {
                log?.Invoke("Error: ReadDouble" + ex.ToString());
                return 0d;
            }
        }

        /// <summary>
        /// Основа записи
        /// </summary>
        /// <param name="pOffset"></param>
        /// <param name="pBytes"></param>
        /// <returns></returns>
        public bool WriteByteArray(IntPtr pOffset, byte[] pBytes)
        {
            if (intorProcc.End) { return false; }
            bool result;
            try
            {
                uint flNewProtect;
                VirtualProtectEx(processHandle, pOffset, (UIntPtr)((ulong)((long)pBytes.Length)), 4U, out flNewProtect);
                result = WriteProcessMemory(processHandle, pOffset, pBytes, (uint)pBytes.Length, 0U);
                VirtualProtectEx(processHandle, pOffset, (UIntPtr)((ulong)((long)pBytes.Length)), flNewProtect, out flNewProtect);
            }
            catch (Exception ex)
            {
                log?.Invoke("Error: WriteByteArray" + ex.ToString());
                result = false;
            }
            return result;
        }
        public bool WriteStringUnicode(IntPtr pOffset, string pData)
        {
            if (intorProcc.End) { return false; }
            try
            {
                return WriteByteArray(pOffset, Encoding.Unicode.GetBytes(pData));
            }
            catch (Exception ex)
            {
                log?.Invoke("Error: WriteStringUnicode" + ex.ToString());
                return false;
            }
        }
        public bool WriteStringASCII(IntPtr pOffset, string pData)
        {
            if (intorProcc.End) { return false; }
            try
            {
                return WriteByteArray(pOffset, Encoding.ASCII.GetBytes(pData));
            }
            catch (Exception ex)
            {
                log?.Invoke("Error: WriteStringASCII" + ex.ToString());
                return false;
            }
        }
        public bool WriteBoolean(IntPtr pOffset, bool pData)
        {
            if (intorProcc.End) { return false; }
            try
            {
                return WriteByteArray(pOffset, BitConverter.GetBytes(pData));
            }
            catch (Exception ex)
            {
                log?.Invoke("Error: WriteBoolean" + ex.ToString());
                return false;
            }
        }
        public bool WriteChar(IntPtr pOffset, char pData)
        {
            if (intorProcc.End) { return false; }
            try
            {
                return WriteByteArray(pOffset, BitConverter.GetBytes(pData));
            }
            catch (Exception ex)
            {
                log?.Invoke("Error: WriteChar" + ex.ToString());
                return false;
            }
        }
        public bool WriteByte(IntPtr pOffset, byte pData)
        {
            if (intorProcc.End) { return false; }
            try
            {
                return WriteByteArray(pOffset, BitConverter.GetBytes((short)pData));
            }
            catch (Exception ex)
            {
                log?.Invoke("Error: WriteByte" + ex.ToString());
                return false;
            }
        }
        public bool WriteInt16(IntPtr pOffset, short pData)
        {
            if (intorProcc.End) { return false; }
            try
            {
                return WriteByteArray(pOffset, BitConverter.GetBytes(pData));
            }
            catch (Exception ex)
            {
                log?.Invoke("Error: WriteInt16" + ex.ToString());
                return false;
            }
        }
        public bool WriteShort(IntPtr pOffset, short pData)
        {
            if (intorProcc.End) { return false; }
            try
            {
                return WriteByteArray(pOffset, BitConverter.GetBytes(pData));
            }
            catch (Exception ex)
            {
                log?.Invoke("Error: WriteShort" + ex.ToString());
                return false;
            }
        }
        public bool WriteInt32(IntPtr pOffset, int pData)
        {
            if (intorProcc.End) { return false; }
            try
            {
                return WriteByteArray(pOffset, BitConverter.GetBytes(pData));
            }
            catch (Exception ex)
            {
                log?.Invoke("Error: WriteInt32" + ex.ToString());
                return false;
            }
        }
        public bool WriteInt64(IntPtr pOffset, long pData)
        {
            if (intorProcc.End) { return false; }
            try
            {
                return WriteByteArray(pOffset, BitConverter.GetBytes(pData));
            }
            catch (Exception ex)
            {
                log?.Invoke("Error: WriteInt64" + ex.ToString());
                return false;
            }
        }
        public bool WriteLong(IntPtr pOffset, long pData)
        {
            if (intorProcc.End) { return false; }
            try
            {
                return WriteByteArray(pOffset, BitConverter.GetBytes(pData));
            }
            catch (Exception ex)
            {
                log?.Invoke("Error: WriteLong" + ex.ToString());
                return false;
            }
        }
        public bool WriteUInt16(IntPtr pOffset, ushort pData)
        {
            if (intorProcc.End) { return false; }
            try
            {
                return WriteByteArray(pOffset, BitConverter.GetBytes(pData));
            }
            catch (Exception ex)
            {
                log?.Invoke("Error: WriteUInt16" + ex.ToString());
                return false;
            }
        }
        public bool WriteUShort(IntPtr pOffset, ushort pData)
        {
            if (intorProcc.End) { return false; }
            try
            {
                return WriteByteArray(pOffset, BitConverter.GetBytes(pData));
            }
            catch (Exception ex)
            {
                log?.Invoke("Error: WriteShort" + ex.ToString());
                return false;
            }
        }
        public bool WriteUInt32(IntPtr pOffset, uint pData)
        {
            if (intorProcc.End) { return false; }
            try
            {
                return WriteByteArray(pOffset, BitConverter.GetBytes(pData));
            }
            catch (Exception ex)
            {
                log?.Invoke("Error: WriteUInt32" + ex.ToString());
                return false;
            }
        }
        public bool WriteUInteger(IntPtr pOffset, uint pData)
        {
            if (intorProcc.End) { return false; }
            try
            {
                return WriteByteArray(pOffset, BitConverter.GetBytes(pData));
            }
            catch (Exception ex)
            {
                log?.Invoke("Error: WriteUInt" + ex.ToString());
                return false;
            }
        }
        public bool WriteUInt64(IntPtr pOffset, ulong pData)
        {
            if (intorProcc.End) { return false; }
            try
            {
                return WriteByteArray(pOffset, BitConverter.GetBytes(pData));
            }
            catch (Exception ex)
            {
                log?.Invoke("Error: WriteUInt64" + ex.ToString());
                return false;
            }
        }
        public bool WriteULong(IntPtr pOffset, ulong pData)
        {
            if (intorProcc.End) { return false; }
            try
            {
                return WriteByteArray(pOffset, BitConverter.GetBytes(pData));
            }
            catch (Exception ex)
            {
                log?.Invoke("Error: WriteULong" + ex.ToString());
                return false;
            }
        }
        public bool WriteFloat(IntPtr pOffset, float pData)
        {
            if (intorProcc.End) { return false; }
            try
            {
                return WriteByteArray(pOffset, BitConverter.GetBytes(pData));
            }
            catch (Exception ex)
            {
                log?.Invoke("Error: WriteFloat" + ex.ToString());
                return false;
            }
        }
        public bool WriteDouble(IntPtr pOffset, double pData)
        {
            if (intorProcc.End) { return false; }
            try
            {
                return WriteByteArray(pOffset, BitConverter.GetBytes(pData));
            }
            catch (Exception ex)
            {
                log?.Invoke("Error: WriteDouble" + ex.ToString());
                return false;
            }
        }
        public override string ToString()
        {
            return intorProcc.ToString();
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint dwSize, uint lpNumberOfBytesRead);
        [DllImport("kernel32.dll")]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, uint lpNumberOfBytesWritten);
        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32.dll")]
        static extern bool CloseHandle(IntPtr hObject);
        [DllImport("kernel32.dll")]
        static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);
    }
}
