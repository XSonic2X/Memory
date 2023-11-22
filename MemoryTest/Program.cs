using Memory;
using System;
using System.Collections.Generic;

namespace MemoryTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IntorProcc intorProcc = ProcessSearch.SearchMachineName("Test.exe");
            ProcessMemory processMemory = new ProcessMemory(intorProcc);
            processMemory.log += Console.WriteLine;
            Signatures signatures = new Signatures("01 ?? ?? ?? ?? 05 ?? ?? ?? ?? ?? ?? 84 50 34 64 01 ??");
            List<IntPtr> iP = processMemory.SignaturesSearch(signatures, (int)intorProcc.process.MainModule.BaseAddress, (int)intorProcc.process.MainModule.BaseAddress + intorProcc.Size);
            foreach (IntPtr p in iP)
            {
                Console.WriteLine(p.ToString("x2"));
            }
            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
