using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Memory;

public delegate void Log(string log);
public abstract partial class ProcessMemory : IDisposable
{

    public ProcessMemory(in IntorProcc iP)
    {
        intorProcc = iP;
        if (intorProcc.processModule is null)
        {
            baseAddress = intorProcc.process.MainModule.BaseAddress;
            Size = (uint)intorProcc.Size - 1;
        }
        else
        {
            baseAddress = intorProcc.processModule.BaseAddress;
            Size = (uint)intorProcc.ModuleSize - 1;
        }
        mainThread = intorProcc.process.Threads.OfType<ProcessThread>().OrderBy(t => t.StartTime).FirstOrDefault();
        mainThreadHandle = Kernel.OpenThread((uint)mainThread.Id);
        if (mainThreadHandle is 0)
            throw new Exception("Null mth");
        CheckProcess();
    }

    public event Log log;
    public readonly IntorProcc intorProcc;
    private readonly ProcessThread mainThread;
    public uint Size = 0;

    public readonly IntPtr baseAddress;
    public IntPtr processHandle;
    public IntPtr mainThreadHandle;

    private object lockT = new();

    private bool End = false;

    private static readonly bool IsBit = IntPtr.Size is 4;

    public static ProcessMemory Create(Type type, in IntorProcc iP)
        => type switch
        {
            Type.Basic => IsBit ? new ProcessMemory32(iP) : new ProcessMemory64(iP),
            Type.Virtual => IsBit ? new VirtualProtect32(iP) : new VirtualProtect64(iP),
            _ => throw new NotImplementedException(),
        };

    /// <summary>
    /// Финальный адрес
    /// </summary>
    /// <param name="BaseAddress"></param>
    /// <param name="BiasAddress"></param>
    /// <returns></returns>
    public IntPtr FinalAddress(params IntPtr[] offset)
    {
        IntPtr baseAddress = offset[0];
        int i = 1, count = offset.Length - 1;
        for (; i < count; i++)
            baseAddress = ReadIntPtr(baseAddress + offset[i]);
        return baseAddress + offset[count];
    }

    public IntPtr FinalAddressBass(params IntPtr[] offset)
    {
        IntPtr baseAddress = this.baseAddress;
        int i = 0, count = offset.Length - 1;
        for (; i < count; i++)
            baseAddress = ReadIntPtr(baseAddress + offset[i]);
        return baseAddress + offset[count];
    }

    public IntPtr SearchAddress(IMask[] masks)
        => SearchAddress(masks, ReadByteArray(baseAddress, Size));

    public IntPtr[] SearchAddress(IMask[][] masks)
    {
        List<IntPtr> nints = new List<IntPtr>();
        byte[] bytes = ReadByteArray(baseAddress, Size);
        for (int i = 0; i < masks.Length; i++)
            nints.Add(SearchAddress(masks[i], bytes));
        return nints.ToArray();
    }

    #region Чтения
    /// <summary>
    /// Основа чтения
    /// </summary>
    /// <param name="pOffset"></param>
    /// <param name="pSize"></param>
    /// <returns></returns>
    public virtual byte[] ReadByteArray(IntPtr pOffset, uint pSize)
    {
        lock (lockT)
        {
            byte[] array = new byte[pSize];
            if (!intorProcc.isEnd && !Kernel.ReadProcessMemory(processHandle, pOffset, array, pSize, 0U))
                log?.Invoke("Error: Not enough rights to read");
            return array;
        }
    }

    public T ReadStruct<T>(IntPtr pOffset) where T : struct
    {
        unsafe
        {
            fixed (byte* p = ReadByteArray(pOffset, (uint)Marshal.SizeOf<T>()))
                return *(T*)p;
        }
    }
    public string ReadStringUnicode(IntPtr pOffset, uint pSize)
        => Encoding.Unicode.GetString(ReadByteArray(pOffset, pSize), 0, (int)pSize);
    public string ReadStringASCII(IntPtr pOffset, uint pSize)
        => Encoding.ASCII.GetString(ReadByteArray(pOffset, pSize), 0, (int)pSize);

    public char ReadChar(IntPtr pOffset)
        => BitConverter.ToChar(ReadByteArray(pOffset, 1U), 0);

    public bool ReadBoolean(IntPtr pOffset)
        => BitConverter.ToBoolean(ReadByteArray(pOffset, 1U), 0);
    public byte ReadByte(IntPtr pOffset)
        => ReadByteArray(pOffset, 1U)[0];
    public short ReadInt16(IntPtr pOffset)
        => BitConverter.ToInt16(ReadByteArray(pOffset, 2U), 0);
    public short ReadShort(IntPtr pOffset)
        => BitConverter.ToInt16(ReadByteArray(pOffset, 2U), 0);
    public int ReadInt32(IntPtr pOffset)
        => BitConverter.ToInt32(ReadByteArray(pOffset, 4U), 0);
    public long ReadInt64(IntPtr pOffset)
        => BitConverter.ToInt64(ReadByteArray(pOffset, 8U), 0);

    public abstract IntPtr ReadIntPtr(IntPtr pOffset);

    public long ReadLong(IntPtr pOffset)
        => BitConverter.ToInt64(ReadByteArray(pOffset, 8U), 0);
    public ushort ReadUInt16(IntPtr pOffset)
        => BitConverter.ToUInt16(ReadByteArray(pOffset, 2U), 0);
    public ushort ReadUShort(IntPtr pOffset)
        => BitConverter.ToUInt16(ReadByteArray(pOffset, 2U), 0);
    public uint ReadUInt32(IntPtr pOffset)
        => BitConverter.ToUInt32(ReadByteArray(pOffset, 4U), 0);
    public uint ReadUInteger(IntPtr pOffset)
        => BitConverter.ToUInt32(ReadByteArray(pOffset, 4U), 0);
    public ulong ReadUInt64(IntPtr pOffset)
        => BitConverter.ToUInt64(ReadByteArray(pOffset, 8U), 0);
    public ulong ReadULong(IntPtr pOffset)
        => BitConverter.ToUInt64(ReadByteArray(pOffset, 8U), 0);
    public float ReadFloat(IntPtr pOffset)
        => BitConverter.ToSingle(ReadByteArray(pOffset, 4U), 0);
    public double ReadDouble(IntPtr pOffset)
        => BitConverter.ToDouble(ReadByteArray(pOffset, 8U), 0);
    #endregion

    #region Записи
    /// <summary>
    /// Основа записи
    /// </summary>
    /// <param name="pOffset"></param>
    /// <param name="pBytes"></param>
    /// <returns></returns>
    public virtual bool WriteByteArray(IntPtr pOffset, byte[] pBytes)
    {
        lock (lockT)
        {
            if (intorProcc.isEnd) return false;
            bool result = Kernel.WriteProcessMemory(processHandle, pOffset, pBytes, (uint)pBytes.Length, 0U);
            if (!result) log?.Invoke("Error: There are insufficient recording rights.");
            return result;
        }
    }

    public bool WriteStructCorrector<T>(IntPtr pOffset, T t) where T : struct
    {
        FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        int[,] offse = new int[fields.Length, 2];
        for (int i = 0; i < fields.Length; i++)
        {
            offse[i, 0] = fields[i].GetCustomAttribute<FieldOffsetAttribute>().Value;
            offse[i, 1] = Marshal.SizeOf(fields[i].FieldType);
        }
        int size = Marshal.SizeOf<T>();
        byte[] oldBytes = new byte[size];
        byte[] newBytes;
        unsafe
        {
            fixed (byte* p = oldBytes)
                *(T*)p = t;
        }
        if (Kernel.SuspendThread(mainThreadHandle) is -1)
        {
            log($"Erro:{Marshal.GetLastWin32Error()}");
            return false;
        }
        newBytes = ReadByteArray(pOffset, (uint)size);
        for (int i = 0, offs; i < fields.Length; i++)
        {
            offs = offse[i, 0];
            Buffer.BlockCopy(oldBytes, offs, newBytes, offs, offse[i, 1]);
        }
        bool a = WriteByteArray(pOffset, newBytes);
        Kernel.ResumeThread(mainThreadHandle);
        return a;
    }

    public bool WriteStruc<T>(IntPtr pOffset, T t) where T : struct
    {
        byte[] bytes = new byte[Marshal.SizeOf<T>()];
        unsafe
        {
            fixed (byte* p = bytes)
                *(T*)p = t;
        }
        return WriteByteArray(pOffset, bytes);
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
    #endregion

    public void ReadToWrite<T>(IntPtr read, IntPtr write) where T : struct
        => WriteByteArray(write, ReadByteArray(read, (uint)Marshal.SizeOf<T>()));
    public void Dispose()
    {
        if (End) return;
        End = true;
        Kernel.CloseHandle(mainThreadHandle);
        Kernel.CloseHandle(processHandle);
    }

    protected virtual bool CheckProcess()
    {
        if (intorProcc.isEnd)
            log?.Invoke("Процесс не работает.");
        else if ((processHandle = Kernel.OpenProcess((uint)intorProcc.ID, (uint)Kernel.ProcessFlags.VMWrite | (uint)Kernel.ProcessFlags.VMRead)) == IntPtr.Zero)
            log?.Invoke("Процесс не был найден. Пожалуйста, проверьте и повторите попытку");
        else return true;
        return false;
    }

    private static IntPtr SearchAddress(IMask[] masks, byte[] bytes)
    {
        long end = bytes.Length - masks.Length, i = 0;
        do
        {
            long j = 0;
            do if (j < masks.Length)
                    return (IntPtr)i;
            while (masks[j].IsMask(bytes[i + j++]));
        } while (i++ <= end);
        return IntPtr.Zero;
    }

    public override string ToString()
        => intorProcc.ToString();


}
partial class ProcessMemory
{

    public enum Type
    {
        Basic,
        Virtual
    }

    private abstract class VirtualProtect : ProcessMemory
    {

        public VirtualProtect(in IntorProcc iP) : base(iP)
        {
        }

        public override bool WriteByteArray(IntPtr pOffset, byte[] pBytes)
        {
            lock (lockT)
            {
                bool result = false;
                if (intorProcc.isEnd) goto End;
                try
                {
                    result = Kernel.VirtualProtectEx(processHandle, pOffset, (UIntPtr)pBytes.Length, 4U, out uint flNewProtect);
                    if (!result) goto End;
                    result = Kernel.WriteProcessMemory(processHandle, pOffset, pBytes, (uint)pBytes.Length, 0U);
                    Kernel.VirtualProtectEx(processHandle, pOffset, (UIntPtr)pBytes.Length, flNewProtect, out flNewProtect);
                }
                catch (Exception ex)
                {
                    log?.Invoke("Error: WriteByteArray" + ex.ToString());
                }
            End:
                return result;
            }
        }

        public override byte[] ReadByteArray(IntPtr pOffset, uint pSize)
        {
            lock (lockT)
            {
                byte[] array = new byte[pSize];
                if (intorProcc.isEnd) goto End;
                try
                {
                    if (!Kernel.VirtualProtectEx(processHandle, pOffset, pSize, 4U, out uint flNewProtect)) goto End;
                    Kernel.ReadProcessMemory(processHandle, pOffset, array, pSize, 0U);
                    Kernel.VirtualProtectEx(processHandle, pOffset, pSize, flNewProtect, out flNewProtect);
                }
                catch (Exception ex)
                {
                    log?.Invoke("Error: ReadByteArray" + ex.ToString());
                }
            End:
                return array;
            }
        }

        protected override bool CheckProcess()
        {
            if (intorProcc.isEnd)
                log?.Invoke("Процесс не работает.");
            else if ((processHandle = Kernel.OpenProcess((uint)intorProcc.ID, (uint)Kernel.ProcessFlags.All)) == IntPtr.Zero)
                log?.Invoke("Процесс не был найден. Пожалуйста, проверьте и повторите попытку");
            else return true;
            return false;
        }


    }

    private class ProcessMemory32 : ProcessMemory
    {
        public ProcessMemory32(in IntorProcc iP) : base(iP)
        {
        }
        public override nint ReadIntPtr(nint pOffset)
            => (IntPtr)BitConverter.ToInt32(ReadByteArray(pOffset, 4U), 0);
    }
    private class ProcessMemory64 : ProcessMemory
    {
        public ProcessMemory64(in IntorProcc iP) : base(iP)
        {
        }
        public override nint ReadIntPtr(nint pOffset)
            => (IntPtr)BitConverter.ToInt64(ReadByteArray(pOffset, 8U), 0);
    }

    private class VirtualProtect32 : VirtualProtect
    {
        public VirtualProtect32(in IntorProcc iP) : base(iP)
        {
        }
        public override nint ReadIntPtr(nint pOffset)
            => (IntPtr)BitConverter.ToInt32(ReadByteArray(pOffset, 4U), 0);
    }
    private class VirtualProtect64 : VirtualProtect
    {
        public VirtualProtect64(in IntorProcc iP) : base(iP)
        {
        }
        public override nint ReadIntPtr(nint pOffset)
            => (IntPtr)BitConverter.ToInt64(ReadByteArray(pOffset, 8U), 0);
    }

}
