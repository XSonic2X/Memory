using MemoryCore;

namespace TestMem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ProcessMemory proMem = new ProcessMemory(ProcessSearch.SearchMachineName("New.exe"));
            while (true)
            {
                Console.WriteLine(proMem.ReadByte(proMem.FinalAddress64Bass([0x124EF50, 0x68, 0x5d8, 0x28])));

                Console.WriteLine("End");
                Console.ReadLine();
            }
        }
    }
}
