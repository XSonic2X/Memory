using System.Collections.Generic;
using System.Diagnostics;

namespace Memory;

public static class ProcessSearch
{
    /// <summary>
    /// Name = mspaint.exe
    /// </summary>
    /// <param name="MachineName"></param>
    /// <returns></returns>
    public static IEnumerator<IntorProcc> SearchMachineName(string MachineName)
    {
        Process[] processes = Process.GetProcesses();
        for (int i = 0; i < processes.Length; i++)
            if (SearchMachineName(processes[i], MachineName))
                yield return new IntorProcc(processes[i]);
        yield break;
    }

    public static IEnumerator<IntorProcc> SearchMachineName(string MachineName, string ModuleMachineName)
    {
        Process[] processes = Process.GetProcesses();
        for (int i = 0; i < processes.Length; i++)
            if (SearchMachineName(processes[i], MachineName) && SearchMachineName(processes[i], ModuleMachineName, out IntorProcc? intorProcc))
                yield return intorProcc.Value;
        yield break;
    }

    /// <summary>
    /// Name = mspaint
    /// </summary>
    /// <param name="ProcessName"></param>
    /// <returns></returns>
    public static IEnumerator<IntorProcc> SearchProcessName(string ProcessName)
    {
        Process[] processes = Process.GetProcesses();
        for (int i = 0; i < processes.Length; i++)
            if (SearchProcessName(processes[i], ProcessName))
                yield return new IntorProcc(processes[i]);
        yield break;
    }

    private static bool SearchMachineName(Process p, string MachineName)
    {
        try
        {
            return p.MainWindowHandle is not 0 && p.MainModule.ModuleName == MachineName;
        }
        catch { return false; }
    }

    private static bool SearchProcessName(Process p, string ProcessName)
    {
        try
        {
            return p.MainWindowHandle is not 0 && p.ProcessName == ProcessName;
        }
        catch { return false; }
    }

    private static bool SearchMachineName(Process p, string ModuleMachineName, out IntorProcc? intorProcc)
    {
        for (int i = 0; i < p.Modules.Count; i++)
            if (p.Modules[i].ModuleName == ModuleMachineName)
            {
                intorProcc = new IntorProcc(p, p.Modules[i]);
                return true;
            }

        intorProcc = null;
        return false;
    }

}
