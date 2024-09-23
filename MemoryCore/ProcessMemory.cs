using System.Runtime.InteropServices;
using System.Text;

namespace MemoryCore;

public delegate void Log(string log);
public class ProcessMemory
{
    public ProcessMemory(IntorProcc iP)
    {
        intorProcc = iP;
        if (intorProcc.processModule == null)
        {
            baseAddress = intorProcc.process.MainModule.BaseAddress;
            Size = intorProcc.Size - 1;
        }
        else
        {
            baseAddress = intorProcc.processModule.BaseAddress;
            Size = intorProcc.MSize - 1;
        }
        CheckProcess();
    }

    public event Log log;
    public IntorProcc intorProcc;
    public long Size = 0;

    public IntPtr baseAddress;
    public IntPtr processHandle;

    /// <summary>
    /// Финальный адрес
    /// </summary>
    /// <param name="BaseAddress"></param>
    /// <param name="BiasAddress"></param>
    /// <returns></returns>
    public IntPtr FinalAddress32(params IntPtr[] offset)
    {
        IntPtr baseAddress = offset[0];
        int i = 1, count = offset.Length - 1;
        for (; i < count; i++)
            baseAddress = ReadInt32(baseAddress + offset[i]);
        return baseAddress + offset[count];
    }

    public IntPtr FinalAddress64(params IntPtr[] offset)
    {
        IntPtr baseAddress = offset[0];
        int i = 1, count = offset.Length - 1;
        for (; i < count; i++)
            baseAddress = (IntPtr)ReadInt64(baseAddress + offset[i]);
        return baseAddress + offset[count];
    }

    public IntPtr FinalAddress64Bass(params IntPtr[] offset)
    {
        IntPtr baseAddress = this.baseAddress;
        int i = 0, count = offset.Length - 1;
        for (; i < count; i++)
            baseAddress = (IntPtr)ReadInt64(baseAddress + offset[i]);
        return baseAddress + offset[count];
    }

    public IntPtr FinalAddress32Bass(params IntPtr[] offset)
    {
        IntPtr baseAddress = this.baseAddress;
        int i = 0, count = offset.Length - 1;
        for (; i < count; i++)
            baseAddress = ReadInt32(baseAddress + offset[i]);
        return baseAddress + offset[count];
    }


    //public IntPtr SignaturesSearch(Signatures signatures, long start, long Size)
    //{
    //    return 0;
    //}

    public bool CheckProcess()
    {
        if (!intorProcc.End)
        {
            processHandle = OpenProcess(2035711U, false, intorProcc.ID);
            if (processHandle != IntPtr.Zero) 
                return true;
            else 
                log?.Invoke("Процесс не был найден. Пожалуйста, проверьте и повторите попытку");
        }
        else
            log?.Invoke("Процесс не работает.");
        return false;
    }
    /// <summary>
    /// Основа чтения
    /// </summary>
    /// <param name="pOffset"></param>
    /// <param name="pSize"></param>
    /// <returns></returns>
    public byte[] ReadByteArray(IntPtr pOffset, uint pSize)
    {
        if (!intorProcc.End) 
        {
            try
            {
                VirtualProtectEx(processHandle, pOffset, (UIntPtr)pSize, 4U, out uint flNewProtect);
                byte[] array = new byte[pSize];
                ReadProcessMemory(processHandle, pOffset, array, pSize, 0U);
                VirtualProtectEx(processHandle, pOffset, (UIntPtr)pSize, flNewProtect, out flNewProtect);
                return array;
            }
            catch (Exception ex)
            {
                log?.Invoke("Error: ReadByteArray" + ex.ToString());
            }
        }
        return null;
    }
    public string ReadStringUnicode(IntPtr pOffset, uint pSize)
    {
        byte[] array = ReadByteArray(pOffset, pSize);
        if (array is null) return string.Empty;
        return Encoding.Unicode.GetString(array, 0, (int)pSize);
    }
    public string ReadStringASCII(IntPtr pOffset, uint pSize)
    {
        byte[] array = ReadByteArray(pOffset, pSize);
        if (array is null) return string.Empty;
        return Encoding.ASCII.GetString(array, 0, (int)pSize);
    }
    public char ReadChar(IntPtr pOffset)
    {
        byte[] array = ReadByteArray(pOffset, 1U);
        if (array is null) return default;
        return BitConverter.ToChar(array, 0);
    }
    public bool ReadBoolean(IntPtr pOffset)
    {
        byte[] array = ReadByteArray(pOffset, 1U);
        if (array is null) return default;
        return BitConverter.ToBoolean(array, 0);
    }
    public byte ReadByte(IntPtr pOffset)
    {
        byte[] array = ReadByteArray(pOffset, 1U);
        if (array is null) return default;
        return array[0];
    }
    public short ReadInt16(IntPtr pOffset)
    {
        byte[] array = ReadByteArray(pOffset, 2U);
        if (array is null) return default;
        return BitConverter.ToInt16(array, 0);
    }
    public short ReadShort(IntPtr pOffset)
    {
        byte[] array = ReadByteArray(pOffset, 2U);
        if (array is null) return default;
        return BitConverter.ToInt16(array, 0);
    }
    public int ReadInt32(IntPtr pOffset)
    {
        byte[] array = ReadByteArray(pOffset, 4U);
        if (array is null) return default;
        return BitConverter.ToInt32(array, 0);
    }
    public long ReadInt64(IntPtr pOffset)
    {
        byte[] array = ReadByteArray(pOffset, 8U);
        if (array is null) return default;
        return BitConverter.ToInt64(array, 0);
    }
    public long ReadLong(IntPtr pOffset)
    {
        byte[] array = ReadByteArray(pOffset, 8U);
        if (array is null) return default;
        return BitConverter.ToInt64(array, 0);
    }
    public ushort ReadUInt16(IntPtr pOffset)
    {
        byte[] array = ReadByteArray(pOffset, 2U);
        if (array != null) return default;
        return BitConverter.ToUInt16(array, 0);
    }
    public ushort ReadUShort(IntPtr pOffset)
    {
        byte[] array = ReadByteArray(pOffset, 2U);
        if (array is null) return default;
        return BitConverter.ToUInt16(array, 0);
    }
    public uint ReadUInt32(IntPtr pOffset)
    {
        byte[] array = ReadByteArray(pOffset, 4U);
        if (array is null) return default;
        return BitConverter.ToUInt32(array, 0);
    }
    public uint ReadUInteger(IntPtr pOffset)
    {
        byte[] array = ReadByteArray(pOffset, 4U);
        if (array is null) return default;
        return BitConverter.ToUInt32(array, 0);
    }
    public ulong ReadUInt64(IntPtr pOffset)
    {
        byte[] array = ReadByteArray(pOffset, 8U);
        if (array is null) return default;
        return BitConverter.ToUInt64(array, 0);
    }
    public ulong ReadULong(IntPtr pOffset)
    {
        byte[] array = ReadByteArray(pOffset, 8U);
        if (array is null) return default;
        return BitConverter.ToUInt64(array, 0);
    }
    public float ReadFloat(IntPtr pOffset)
    {
        byte[] array = ReadByteArray(pOffset, 4U);
        if (array is null) return default;
        return BitConverter.ToSingle(array, 0);
    }
    public double ReadDouble(IntPtr pOffset)
    {
        byte[] array = ReadByteArray(pOffset, 8U);
        if (array is null) return default;
        return BitConverter.ToDouble(array, 0);
    }

    /// <summary>
    /// Основа записи
    /// </summary>
    /// <param name="pOffset"></param>
    /// <param name="pBytes"></param>
    /// <returns></returns>
    public bool WriteByteArray(IntPtr pOffset, byte[] pBytes)
    {
        if (!intorProcc.End) 
        {
            try
            {
                VirtualProtectEx(processHandle, pOffset, (UIntPtr)pBytes.Length, 4U, out uint flNewProtect);
                bool result = WriteProcessMemory(processHandle, pOffset, pBytes, (uint)pBytes.Length, 0U);
                VirtualProtectEx(processHandle, pOffset, (UIntPtr)pBytes.Length, flNewProtect, out flNewProtect);
                return result;
            }
            catch (Exception ex)
            {
                log?.Invoke("Error: WriteByteArray" + ex.ToString());
            }
        }
        return false;
    }
    public bool WriteStringUnicode(IntPtr pOffset, string pData)
        => WriteByteArray(pOffset, Encoding.Unicode.GetBytes(pData));
    public bool WriteStringASCII(IntPtr pOffset, string pData)
        => WriteByteArray(pOffset, Encoding.ASCII.GetBytes(pData));
    public bool WriteBoolean(IntPtr pOffset, bool pData)
        => WriteByteArray(pOffset, BitConverter.GetBytes(pData));
    public bool WriteChar(IntPtr pOffset, char pData)
        => WriteByteArray(pOffset, BitConverter.GetBytes(pData));
    public bool WriteByte(IntPtr pOffset, byte pData)
        => WriteByteArray(pOffset, BitConverter.GetBytes((short)pData));
    public bool WriteInt16(IntPtr pOffset, short pData)
        => WriteByteArray(pOffset, BitConverter.GetBytes(pData));
    public bool WriteShort(IntPtr pOffset, short pData)
        => WriteByteArray(pOffset, BitConverter.GetBytes(pData));
    public bool WriteInt32(IntPtr pOffset, int pData)
        => WriteByteArray(pOffset, BitConverter.GetBytes(pData));
    public bool WriteInt64(IntPtr pOffset, long pData)
        => WriteByteArray(pOffset, BitConverter.GetBytes(pData));
    public bool WriteLong(IntPtr pOffset, long pData)
        => WriteByteArray(pOffset, BitConverter.GetBytes(pData));
    public bool WriteUInt16(IntPtr pOffset, ushort pData)
        => WriteByteArray(pOffset, BitConverter.GetBytes(pData));
    public bool WriteUShort(IntPtr pOffset, ushort pData)
        => WriteByteArray(pOffset, BitConverter.GetBytes(pData));
    public bool WriteUInt32(IntPtr pOffset, uint pData)
        => WriteByteArray(pOffset, BitConverter.GetBytes(pData));
    public bool WriteUInteger(IntPtr pOffset, uint pData)
        => WriteByteArray(pOffset, BitConverter.GetBytes(pData));
    public bool WriteUInt64(IntPtr pOffset, ulong pData)
        => WriteByteArray(pOffset, BitConverter.GetBytes(pData));
    public bool WriteULong(IntPtr pOffset, ulong pData)
        => WriteByteArray(pOffset, BitConverter.GetBytes(pData));
    public bool WriteFloat(IntPtr pOffset, float pData)
        => WriteByteArray(pOffset, BitConverter.GetBytes(pData));
    public bool WriteDouble(IntPtr pOffset, double pData)
        => WriteByteArray(pOffset, BitConverter.GetBytes(pData));
    public override string ToString()
        => intorProcc.ToString();

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint dwSize, uint lpNumberOfBytesRead);
    
    [DllImport("kernel32.dll")]
    static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, uint lpNumberOfBytesWritten);
    
    [DllImport("kernel32.dll")]
    static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

    [DllImport("kernel32.dll")]
    static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

    [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
    static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint flAllocationType, uint flProtect);
}
