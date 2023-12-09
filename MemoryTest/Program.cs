using Memory;
using System;
using System.Collections.Generic;

namespace MemoryTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IntorProcc intorProcc = ProcessSearch.SearchMachineName("cs2.exe", "client.dll");
            ProcessMemory processMemory = new ProcessMemory(intorProcc);
            processMemory.log += Console.WriteLine;

            IntPtr intPtr = (IntPtr)processMemory.ReadInt64(processMemory.baseAddress + 0x16C2D98);
            Console.ReadLine();
            Signatures signatures = new Signatures("01 ?? ?? ?? ?? 05 ?? ?? ?? ?? ?? ?? 84 50 34 64 01 ??");
            List<IntPtr> iP = processMemory.SignaturesSearch(signatures, (int)intorProcc.process.MainModule.BaseAddress, (int)intorProcc.process.MainModule.BaseAddress + intorProcc.Size);
            foreach (IntPtr p in iP)
            {
                Console.WriteLine(p.ToString("x2"));
            }
            Console.WriteLine(processMemory.ReadInt32(processMemory.FinalAddress(new int[] { 0x15, 0x4 })));
            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
