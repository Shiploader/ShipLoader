using Microsoft.Win32;
using ShipLoader.API;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using mscoree;
using System.Threading.Tasks;
using System.Threading;
using NLog.Config;

namespace ShipLoader.UI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		[DllImport("kernel32.dll")]
		public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr GetModuleHandle(string lpModuleName);

		[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

		[DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
		static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
			uint dwSize, uint flAllocationType, uint flProtect);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);

		[DllImport("kernel32.dll")]
		static extern IntPtr CreateRemoteThread(IntPtr hProcess,
			IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

		// privileges
		const int PROCESS_CREATE_THREAD = 0x0002;
		const int PROCESS_QUERY_INFORMATION = 0x0400;
		const int PROCESS_VM_OPERATION = 0x0008;
		const int PROCESS_VM_WRITE = 0x0020;
		const int PROCESS_VM_READ = 0x0010;

		// used for memory allocation
		const uint MEM_COMMIT = 0x00001000;
		const uint MEM_RESERVE = 0x00002000;
		const uint PAGE_READWRITE = 4;

		private ModLoader modLoader = new ModLoader();

		public MainWindow()
		{
			InitializeComponent();

			modLoader.LoadMods();

			ModMetadataTable.DataContext = typeof(Mod);

			foreach (Mod m in modLoader.LoadedMods)
			{
				ModMetadataTable.Items.Add(m);
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(Launcher.Default.RaftExePath))
			{
				OpenFileDialog openFileDialog = new OpenFileDialog();
				if (openFileDialog.ShowDialog() == true)
					Launcher.Default.RaftExePath = openFileDialog.FileName;
			}
			else
			{
				ProcessStartInfo info = new ProcessStartInfo(Launcher.Default.RaftExePath);
				info.RedirectStandardError = true;
				info.RedirectStandardInput = true;
				info.RedirectStandardOutput = true;

				info.UseShellExecute = false;

				Process raftProcess = Process.Start(info);
				PollForMono(raftProcess);

				ProcessStartInfo harpoonInfo = new ProcessStartInfo("Harpoon.exe", $"-penetrate {raftProcess.Id} \"Harpoon.Dll.dll\"");
				harpoonInfo.RedirectStandardError = true;
				harpoonInfo.RedirectStandardInput = true;
				harpoonInfo.RedirectStandardOutput = true;
				harpoonInfo.UseShellExecute = false;

				var injectedProcess = Process.Start(harpoonInfo);

				injectedProcess.OutputDataReceived += (sendr, msg) => {
					NLog.LogManager.GetLogger("harpoon").Log(NLog.LogLevel.Info, msg.Data);
				};


				injectedProcess.Exited += (jawatjonge, ex) =>
				{
					raftProcess.Close();
					MessageBox.Show("Mod injector has not succeeded in fucking us over. thanks Nelus");
				};
			}
		}

		void PollForMono(Process process)
		{
			Thread.Sleep(1000);

			do
			{
				for (int i = 0; i < process.Modules.Count; i++)
				{
					if (process.Modules[i].ModuleName == "mono.dll")
					{
						return;
					}
				}

				Thread.Sleep(100);
			} while (true);
		}
	}
}