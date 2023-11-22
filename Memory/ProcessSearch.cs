using System.Diagnostics;
using System.Threading;

namespace Memory
{
    public static class ProcessSearch
    {
        public static IntorProcc SearchMachineName(string MachineName)
        {
            while (true)
            {
                Process[] processes = Process.GetProcesses();
                for (int i = 0; i < processes.Length; i++)
                {
                    try
                    {
                        if ((int)processes[i].MainWindowHandle != 0 && processes[i].MainModule.ModuleName == MachineName)
                        {
                            return new IntorProcc(processes[i]);
                        }
                    }
                    catch
                    {
                    }
                }
                Thread.Sleep(10);
            }
        }
        public static IntorProcc SearchMachineName(string MachineName, string MachineName2)
        {
            while (true)
            {
                Process[] processes = Process.GetProcesses();
                for (int i = 0; i < processes.Length; i++)
                {
                    try
                    {
                        if ((int)processes[i].MainWindowHandle != 0 && processes[i].MainModule.ModuleName == MachineName)
                        {
                            for (int j = 0; j < processes[i].Modules.Count; j++)
                            {
                                if (processes[i].Modules[j].ModuleName == MachineName2)
                                {
                                    return new IntorProcc(processes[i], processes[i].Modules[j]);
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }
                Thread.Sleep(10);
            }
        }
        public static IntorProcc SearchProcessName(string ProcessName)
        {
            Process[] processes = Process.GetProcesses();
            for (int i = 0; i < processes.Length; i++)
            {
                try
                {
                    if ((int)processes[i].MainWindowHandle != 0 && processes[i].ProcessName == ProcessName)
                    {
                        return new IntorProcc(processes[i]);
                    }
                }
                catch
                {
                }
            }
            return null;
        }
        public static IntorProcc SearchProcessName(int ID)
        {
            Process[] processes = Process.GetProcesses();
            for (int i = 0; i < processes.Length; i++)
            {
                try
                {
                    if ((int)processes[i].MainWindowHandle != 0 && processes[i].Id == ID)
                    {
                        return new IntorProcc(processes[i]);
                    }
                }
                catch
                {
                }
            }
            return null;
        }
    }
}
