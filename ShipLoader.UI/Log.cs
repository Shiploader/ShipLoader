using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ShipLoader.UI
{
	public enum LogLevel
	{
		DEBUG, MESSAGE, WARNING, ERROR
	}

	public class LogEntry
	{
		public string Message { get; private set; }
		public DateTime TimeStamp { get; private set; }
		public LogLevel LogLevel { get; private set; }

		public LogEntry(string message, LogLevel level)
		{
			Message = message;
			LogLevel = level;
			TimeStamp = DateTime.Now;
		}
	}

	public static class Log
	{
		private static TextBox ConsoleTextBox;
		private static ObservableCollection<LogEntry> logEntries = new ObservableCollection<LogEntry>(); 

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
