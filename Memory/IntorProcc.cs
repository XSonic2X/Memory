using System.Diagnostics;

namespace Memory;

public struct IntorProcc
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
    public bool isEnd { get => process.HasExited; }
    public int Size
    {
        get => process.MainModule?.ModuleMemorySize ?? 0;
    }
    public int ModuleSize
    {
        get => processModule is null ? -1 : processModule.ModuleMemorySize;
    }
    public override string ToString()
        => $"[{process.ProcessName}][{process.Id.ToString("x2")}]";

}

