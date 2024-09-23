using System;
using System.Diagnostics;

namespace Memory
{
    public class IntorProcc
    {
        public IntorProcc(Process process)
        {
            ID = process.Id;
            this.process = process;
        }
        public IntorProcc(Process process, ProcessModule processModule)
        {
            ID = process.Id;
            this.process = process;
            this.processModule = processModule;
        }

        public readonly Process process;

        public readonly ProcessModule processModule;

        public readonly int ID;
        public bool End { get => process.HasExited; }
        public int Size
        {
            get => process is null ? -1 : process.MainModule.ModuleMemorySize;
        }
        public int MSize
        {
            get => processModule is null ? -1 : processModule.ModuleMemorySize;
        }

        public override string ToString()
        {
            try
            {
                return processModule is null ? process.ProcessName :
                    $"{process.MainModule.ModuleName}=>{processModule.ModuleName}";
            }
            catch (Exception ex) { return ex.Message; }
        }
    }
}
