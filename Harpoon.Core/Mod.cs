using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Harpoon.Core
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
		public virtual ModMetadata Metadata =>
			new ModMetadata {
				AuthorName = "Your Name",
				ModDescription = "A new beginning to a wonderful mod!",
				ModName = "New Mod",
				ModVersion = "0.0.0"
			};

        public virtual void Initialize()
		{

		}
	}
}
