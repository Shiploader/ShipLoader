using ShipLoader.API;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
	}
}
