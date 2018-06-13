using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using System.Threading;
using Harpoon.Core;
using System.IO;

namespace ShipLoader.UI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
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
				string folder = Launcher.Default.RaftExePath.Substring(0, Launcher.Default.RaftExePath.LastIndexOf("\\"));

				ProcessStartInfo info = new ProcessStartInfo(Launcher.Default.RaftExePath);
				info.RedirectStandardError = true;
				info.RedirectStandardInput = true;
				info.RedirectStandardOutput = true;
				info.WorkingDirectory = folder;
				info.UseShellExecute = false;

				Process raftProcess = Process.Start(info);

				PollForMono(raftProcess);

				ProcessStartInfo harpoonInfo = new ProcessStartInfo(Directory.GetCurrentDirectory() + "\\Harpoon.exe", $"-hook {raftProcess.Id} HarpoonLoader.dll");
				harpoonInfo.WorkingDirectory = folder;

				//harpoonInfo.RedirectStandardError = true;
				//harpoonInfo.RedirectStandardInput = true;
				//harpoonInfo.RedirectStandardOutput = true;

				var injectedProcess = Process.Start(harpoonInfo);

				injectedProcess.OutputDataReceived += (sendr, msg) => {
					Console.WriteLine(msg.Data);
				};
			}
		}

		void PollForMono(Process process)
		{
			Thread.Sleep(10000);

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