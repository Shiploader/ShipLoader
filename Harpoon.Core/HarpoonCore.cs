using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

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
		try
		{
			Console.WriteLine("loading assemblies...");

			List<Assembly> assembliesFound = new List<Assembly>();

			Console.WriteLine("getting folder...");
			string[] modFiles = Directory.GetFiles(Directory.GetCurrentDirectory() + "/mods");

			foreach (string modLoc in modFiles)
			{
				try
				{
					if (modLoc.EndsWith(".dll"))
					{
						Assembly asm = Assembly.LoadFile(modLoc);

						if (asm.GetTypes().Count(x => x.IsSubclassOf(typeof(IInitializable))) > 0)
						{
							assembliesFound.Add(asm);
						}
						Console.WriteLine("loaded mod '" + modLoc + "'...");
					}
				}
				catch
				{

				}
			}


			foreach (Assembly a in assembliesFound)
			{
				Type[] types = a.GetTypes()
					.Where(x => x.IsSubclassOf(typeof(IInitializable)))
					.ToArray();

				foreach (Type t in types)
				{
					Console.WriteLine("initializing initializable '" + t.ToString() + "'...");

					IInitializable m = (IInitializable)Activator.CreateInstance(t);
					if (m != null)
					{
						try
						{
							m.Initialize();
						}
						catch (Exception e)
						{
							Console.WriteLine("ERROR: mod of type " + m.ToString() + " failed loading!\nCALL STACK:\n" + e.ToString());
						}
					}
				}
			}
		}
		catch(Exception e)
		{
			Console.WriteLine("ERROR: internal exception!\nCALL STACK:\n" + e.ToString());
		}
	}

}