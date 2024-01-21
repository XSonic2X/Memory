using Memory;
using System;

namespace MemoryTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IntorProcc intorProcc = ProcessSearch.SearchMachineName("cs2.exe", "client.dll");
            ProcessMemory processMemory = new ProcessMemory(intorProcc);
            processMemory.log += Console.WriteLine;
            Console.WriteLine(processMemory.baseAddress.ToString("x2"));
            Console.ReadLine();
            Signatures signatures = new Signatures("01 ?? ?? ?? ?? 05 ?? ?? ?? ?? ?? ?? 84 50 34 64 01 ??");
            IntPtr  iP = processMemory.SignaturesSearch(signatures, (long)processMemory.baseAddress, (long)processMemory.baseAddress + processMemory.Size);
            Console.WriteLine(iP.ToString("x2"));
            Console.WriteLine(processMemory.ReadInt32(processMemory.FinalAddress(new int[] { 0x15, 0x4 })));
            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
