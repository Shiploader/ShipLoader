using ShipLoader.API;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ShipLoader.TestMod
{
    public class TestMod : Mod
    {
		[DisplayName("Metadata")]
		public override ModMetadata Metadata => new ModMetadata()
		{
			AuthorName = "Veld",
			ModName = "Test Mod",
			ModDescription = "This is a ShipLoader test mod!",
			ModVersion = "0.1"
		};

		public override void Initialize()
		{
			Console.WriteLine("creating cube...");
			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube.transform.position = Camera.main.transform.position + (Camera.main.transform.forward * 2);
			GameObject.DontDestroyOnLoad(cube);
		}
	}
}
