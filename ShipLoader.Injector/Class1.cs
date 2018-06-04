using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShipLoader.Injector
{
	public class InjectedInstance
    {
		static InjectedInstance()
		{
			Application.Quit();
			Console.WriteLine("this is message");
			Process.Start("C:/");
		}

		public static void ExitApplication()
		{
			Console.WriteLine("this is message");
			Process.Start("C:/");
		}
    }
}
