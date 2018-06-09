using Microsoft.Win32;
using ShipLoader.API;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using System.Threading;

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
				ProcessStartInfo info = new ProcessStartInfo(Launcher.Default.RaftExePath);
				info.RedirectStandardError = true;
				info.RedirectStandardInput = true;
				info.RedirectStandardOutput = true;

				info.UseShellExecute = false;

				Process raftProcess = Process.Start(info);
				PollForMono(raftProcess);

				ProcessStartInfo harpoonInfo = new ProcessStartInfo("Harpoon.exe", $"-hook {raftProcess.Id} \"Harpoon.Dll.dll\"");
				harpoonInfo.RedirectStandardError = true;
				harpoonInfo.RedirectStandardInput = true;
				harpoonInfo.RedirectStandardOutput = true;
				harpoonInfo.UseShellExecute = false;

				var injectedProcess = Process.Start(harpoonInfo);

				injectedProcess.OutputDataReceived += (sendr, msg) => {
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