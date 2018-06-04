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

			Log.InitializeLogger(ConsoleTextBox);

			modLoader.LoadMods();

			ModMetadataTable.DataContext = typeof(Mod);

			foreach (Mod m in modLoader.LoadedMods)
			{
				ModMetadataTable.Items.Add(m);
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if(string.IsNullOrEmpty(Launcher.Default.RaftExePath))
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

				Thread.Sleep(5000);

				// geting the handle of the process - with required privileges
				IntPtr procHandle = OpenProcess(PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ, false, raftProcess.Id);

				// searching for the address of LoadLibraryA and storing it in a pointer
				IntPtr loadLibraryAddr = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");

				// name of the dll we want to inject
				string dllName = "ShipLoader.Injector.dll";

				// alocating some memory on the target process - enough to store the name of the dll
				// and storing its address in a pointer
				IntPtr allocMemAddress = VirtualAllocEx(procHandle, IntPtr.Zero, (uint)((dllName.Length + 1) * Marshal.SizeOf(typeof(char))), MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);

				// writing the name of the dll there
				UIntPtr bytesWritten;
				WriteProcessMemory(procHandle, allocMemAddress, Encoding.Default.GetBytes(dllName), (uint)((dllName.Length + 1) * Marshal.SizeOf(typeof(char))), out bytesWritten);

				// creating a thread that will call LoadLibraryA with allocMemAddress as argument
				CreateRemoteThread(procHandle, IntPtr.Zero, 0, loadLibraryAddr, allocMemAddress, 0, IntPtr.Zero);

				for(int i = 0; i < raftProcess.Modules.Count; i++) {
					Log.PrintLine("loaded module '{0}' correctly!", raftProcess.Modules[i].ModuleName);
				}

				loadLibraryAddr = GetProcAddress(GetModuleHandle("ShipLoader.Injector.dll"), "ShipLoader.Injector.InjectedInstance.ExitApplication");

				CreateRemoteThread(procHandle, IntPtr.Zero, 0, loadLibraryAddr, IntPtr.Zero, 0, IntPtr.Zero);

			}
		}
	}
}
