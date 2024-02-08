using Memory;
using System;

namespace MemoryTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IntorProcc intorProcc = ProcessSearch.SearchMachineName("test.exe", "test.dll");
            ProcessMemory processMemory = new ProcessMemory(intorProcc);
            processMemory.log += Console.WriteLine;
            Console.WriteLine(processMemory.baseAddress.ToString("x2"));
            Console.ReadLine();
            Signatures signatures = new Signatures("60 96 01 61 01 ?? ?? ?? ?? CF AB D0 85 ?? ?? ?? ?? 96 78 1B 0F69 03");
            IntPtr  iP = processMemory.SignaturesSearch(signatures, (long)processMemory.baseAddress, processMemory.Size);
            Console.WriteLine(iP.ToString("x2"));
            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
