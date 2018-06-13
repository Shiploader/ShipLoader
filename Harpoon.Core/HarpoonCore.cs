using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace Harpoon.Core
{
    
    public class HarpoonCore
    {

        public static void Initialize()
        {
            try
            {
                Console.WriteLine("Loading assemblies...");

                //These are just 'reflective' assemblies. They aren't executed.
                List<Assembly> mods = new List<Assembly>();

                //Check for dlls
                Console.WriteLine("Scanning " + Directory.GetCurrentDirectory() + "\\mods");

                string[] dirs = Directory.GetDirectories(Directory.GetCurrentDirectory() + "\\mods");

                //Categorize the dlls on if they have an Initializable
                foreach (string modDir in dirs)
                {

                    //List<Assembly> deps = new List<Assembly>();

                    ////Load all dependencies (reflection only)

                    //string[] depsPath = Directory.GetFiles(modDir + "\\deps");

                    //Console.WriteLine("Found mod(s) at " + modDir + " with dependencies:");

                    //foreach (string dep in depsPath)
                    //{
                    //    deps.Add(Assembly.LoadFile(dep));
                    //    Console.WriteLine(dep);
                    //}

                    //Load mods themselves

                    string[] modsPath = Directory.GetFiles(modDir, "*.dll");

                    Console.WriteLine("And mods: ");

                    foreach (string mod in modsPath)
                    {
                        Console.WriteLine("Checking " + mod);

                        try
                        {
                            //Check what category this belongs into

                            Assembly asm = Assembly.LoadFile(mod);

                            foreach (Type t in asm.GetTypes().Where(x => typeof(IMod).IsAssignableFrom(x)))
                                Console.WriteLine("Garbage: " + t.FullName);

                            if (asm.GetTypes().Count(x => typeof(IMod).IsAssignableFrom(x)) > 0)
                            {
                                mods.Add(asm);
                                Console.WriteLine("Found mod " + mod);
                            }
                        }
                        catch (ReflectionTypeLoadException e)
                        {
                            Console.WriteLine("Couldn't load dll (" + mod + "): ");

                            foreach (Exception ex in e.LoaderExceptions)
                                Console.WriteLine(ex.ToString());
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Couldn't load dll (" + mod + "): " + e.ToString());
                        }
                    }
                }

                //Right now it just loads all mods in alphabetical order
                //But the better way would be to sort them on dependency
                foreach (Assembly a in mods)
                {
                    Type[] types = a.GetTypes()
                        .Where(x => typeof(IMod).IsAssignableFrom(x))
                        .ToArray();

                    foreach (Type t in types)
                    {
                        Console.WriteLine("Initializing initializable '" + t.ToString() + "'...");

                        IMod m = (IMod) Activator.CreateInstance(t);
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
            catch (Exception e)
            {
                Console.WriteLine("ERROR: internal exception!\nCALL STACK:\n" + e.ToString());
            }
        }

    }

}