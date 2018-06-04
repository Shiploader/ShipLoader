using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ShipLoader.UI
{
	public static class Log
	{
		private static TextBox ConsoleTextBox;

		public static void InitializeLogger(TextBox outputSource)
		{
			ConsoleTextBox = outputSource;
		}

		public static void PrintLine(string msg, params object[] args)
			=> Print(msg + "\n", args);

		public static void Print(string msg, params object[] args)
			=> ConsoleTextBox.AppendText(string.Format(msg, args));
	}
}
