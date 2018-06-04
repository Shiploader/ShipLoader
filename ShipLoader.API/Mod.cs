using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipLoader.API
{
	public class ModMetadata
	{
		[DisplayName("Enabled")]
		public bool IsEnabled { get; set; }

		[DisplayName("Name")]
		public string ModName { get; set; }

		[DisplayName("Version")]
		public string ModVersion { get; set; }

		[DisplayName("Description")]
		public string ModDescription { get; set; }

		[DisplayName("Author")]
		public string AuthorName { get; set; }
	}

    public class Mod
    {
		public virtual ModMetadata Metadata
			=> new ModMetadata()
			{
				AuthorName = "Invalid",
				IsEnabled = false,
				ModDescription = "Invalid",
				ModName = "Invalid",
				ModVersion = "0.0.0"
			};
	}
}
