using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

[ComVisible(true)]
public interface IInitializable
{
    void Initialize();
}

[ComVisible(true)]
[ClassInterface(ClassInterfaceType.None)]
public class HarpoonCore : IInitializable
{
    public HarpoonCore()
    {

    }

    public void Initialize()
    {
        ProcessStartInfo ProcessInfo;
        Process Process;

        ProcessInfo = new ProcessStartInfo("cmd.exe", "/K msg * Here's Johnny");
        ProcessInfo.CreateNoWindow = true;
        ProcessInfo.UseShellExecute = true;

        Process = Process.Start(ProcessInfo);
    }

}