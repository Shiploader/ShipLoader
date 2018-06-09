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
			Console.WriteLine("Loading assemblies...");
            
            //These are just 'reflective' assemblies. They aren't executed.
			List<Assembly> mods = new List<Assembly>();
            List<Assembly> deps = new List<Assembly>();

            //Check for dlls
			Console.WriteLine("Scanning " + Directory.GetCurrentDirectory() + "\\mods");
			string[] dlls = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\mods", "*.dll");

            //Categorize the dlls on if they have an Initializable
			foreach (string dllLoc in dlls)
			{

                try
                {
                    //Check what category this belongs into

                    Assembly asm = Assembly.ReflectionOnlyLoad(dllLoc);

                    if (asm.GetTypes().Count(x => x.IsSubclassOf(typeof(IInitializable))) > 0)
                    {
                        mods.Add(asm);
                        Console.WriteLine("Found mod " + dllLoc);
                    }
                    else
                    {
                        deps.Add(asm);
                        Console.WriteLine("Found dependency " + dllLoc);
                    }
                }
                catch (ReflectionTypeLoadException e)
                {
                    Console.WriteLine("Couldn't load dll (" + dllLoc + "): ");

                    foreach (Exception ex in e.LoaderExceptions)
                        Console.WriteLine(ex.ToString());
                }
                catch(Exception e)
                {
                    Console.WriteLine("Couldn't load dll (" + dllLoc + "): " + e.ToString());
                }
			}

            //Load dependencies

            //Right now it just loads all mods in alphabetical order
            //But the better way would be to sort them on dependency
			foreach (Assembly a in mods)
			{
				Type[] types = a.GetTypes()
					.Where(x => x.IsSubclassOf(typeof(IInitializable)))
					.ToArray();

				foreach (Type t in types)
				{
					Console.WriteLine("Initializing initializable '" + t.ToString() + "'...");

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