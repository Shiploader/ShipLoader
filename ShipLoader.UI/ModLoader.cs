using ShipLoader.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ShipLoader.UI
{
	public class ModLoader
	{
		public List<Mod> LoadedMods { get; } = new List<Mod>();

		public void LoadMods()
		{
			Assembly[] asm = GetModAssemblies();

			foreach(Assembly a in asm)
			{
				Type[] types = a.GetTypes()
					.Where(x => x.IsSubclassOf(typeof(Mod)))
					.ToArray();

				foreach (Type t in types)
				{
					Mod m = (Mod)Activator.CreateInstance(t);
					if(m != null)
					{
						LoadedMods.Add(m);
					}
				}
			}
		}
		
		private Assembly[] GetModAssemblies()
		{
			List<Assembly> assembliesFound = new List<Assembly>();

			string[] modFiles = Directory.GetFiles(Directory.GetCurrentDirectory() + "/mods");

			foreach(string modLoc in modFiles)
			{
				Assembly asm = Assembly.LoadFile(modLoc);
				
				if(asm.GetTypes().Count(x => x.IsSubclassOf(typeof(Mod))) > 0)
				{
					assembliesFound.Add(asm);
				}
			}

			return assembliesFound.ToArray();
		}
	}
}
