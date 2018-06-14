using System.ComponentModel;
using Harpoon.Core;

namespace ShipLoader.API
{
    public class mod_Raft : RaftMod
    {
        [DisplayName("Metadata")]
        public override ModMetadata Metadata => new ModMetadata()
        {
            AuthorName = "RedBeet Interactive",
            ModName = "Raft",
            ModDescription = "The mod that contains all items, blocks, etc. made by Raft",
            ModVersion = "1.02 (2851750)"
        };

        public override void Initialize()
        {
        }

    }
}
