using System.Diagnostics;

namespace MemoryCore
{
    public class IntorProcc
    {
        public int ID { get; private set; } = -1;
        public bool End { get => process.HasExited; }
        public Process process { get; set; }
        public int Size
        {
            get
            {
                if (process == null) { return -1; }
                return process.MainModule.ModuleMemorySize;
            }
        }
        public ProcessModule processModule { get; set; }
        public int MSize
        {
            get
            {
                if (processModule == null) { return -1; }
                return processModule.ModuleMemorySize;
            }
        }
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
        public override string ToString()
        {
            if (processModule == null) { return process.ProcessName; }
            try { return $"{process.MainModule.ModuleName}=>{processModule.ModuleName}"; }
            catch (Exception ex) { return ex.Message; }
        }
    }
}
