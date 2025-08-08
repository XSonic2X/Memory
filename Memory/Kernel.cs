using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Memory;

public static partial class Kernel
{


    public static IntPtr OpenProcess(uint processId, uint flags)
        => OpenProcess(flags, false, processId);

    public static IntPtr OpenThread(uint threadId)
        => OpenThread(ThreadAccess.SUSPEND_RESUME, false, threadId);



    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern int SuspendThread(IntPtr hThread);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern uint ResumeThread(IntPtr hThread);

    [DllImport("kernel32.dll")]
    public static extern bool CloseHandle(IntPtr hObject);

    [DllImport("kernel32.dll")]
    public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint dwSize, uint lpNumberOfBytesRead);

    [DllImport("kernel32.dll")]
    public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, uint lpNumberOfBytesWritten);

    [DllImport("kernel32.dll")]
    public static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

    [DllImport("kernel32.dll")]
    private static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, uint processId);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

}

partial class Kernel
{
    public enum ProcessFlags : uint
    {
        All = 0x1F0FFF,
        Terminate = 0x1,
        CreateThread = 0x2,
        /// <summary>
        /// Virtual Memory Operation
        /// </summary>
        VMOperation = 0x8,
        /// <summary>
        /// Virtual Memory Read
        /// </summary>
        VMRead = 0x10,
        /// <summary>
        /// Virtual Memory Write
        /// </summary>
        VMWrite = 0x20,
        /// <summary>
        /// Duplicate Handle
        /// </summary>
        DupHandle = 0x40,
        CreateProcess = 0x80,
        SetQuota = 0x100,
        SetInformation = 0x200,
        QueryInformation = 0x400,
        QueryLimitedInformation = 0x1000,
        Synchronize = 0x100000
    }

    [Flags]
    public enum ThreadAccess : int
    {
        TERMINATE = 0x1,
        SUSPEND_RESUME = 0x2,
        GET_CONTEXT = 0x8,
        SET_CONTEXT = 0x10,
        SET_INFORMATION = 0x20,
        QUERY_INFORMATION = 0x40,
        SET_THREAD_TOKEN = 0x80,
        IMPERSONATE = 0x100,
        DIRECT_IMPERSONATION = 0x200,
        ALL_ACCESS = 0x1F03FF
    }

}
