using Harpoon.Core;
using ShipLoader.API;
using System.ComponentModel;

namespace ShipLoader.Balance
{
    public class Balance_Mod : RaftMod
    {
        [DisplayName("Metadata")]
        public override ModMetadata Metadata => new ModMetadata()
        {
            AuthorName = "MankeNelus",
            ModName = "Balance",
            ModDescription = "This mod changes durability, stack sizes and recipes to reduce the feel of Raft being a farming simulator. \"Perfectly balanced. As all things should be.\"",
            ModVersion = "0.1",
            GameVersion = "1.03",
            Priority = 1
        };

        public override void Initialize()
        {
            Item plastic = Item.ByName("Raft.Plastic");
            Item stone = Item.ByName("Raft.Stone");
            Item plank = Item.ByName("Raft.Plank");
            Item arrow = Item.ByName("Raft.Arrow_Stone");

            ModifyItem(Item.ByName("Raft.AirBottle"), ItemField.Durability, 800);
            ModifyItem(Item.ByName("Raft.Flipper"), ItemField.Durability, 800);
            ModifyItem(Item.ByName("Raft.Battery"), ItemField.Durability, 75);
            ModifyItem(Item.ByName("Raft.Axe_Stone"), ItemField.Durability, 75);
            ModifyItem(Item.ByName("Raft.Axe"), ItemField.DisplayName, "Scrap Axe");
            ModifyItem(Item.ByName("Raft.Axe"), ItemField.Durability, 160);
            ModifyItem(Item.ByName("Raft.Bow"), ItemField.Durability, 80);
            ModifyItem(Item.ByName("Raft.Spear_Plank"), ItemField.Durability, 30);
            ModifyItem(Item.ByName("Raft.Spear_Scrap"), ItemField.Durability, 80);
            ModifyItem(Item.ByName("Raft.FishingRod"), ItemField.Durability, 20);
            ModifyItem(Item.ByName("Raft.FishingRod_Metal"), ItemField.Durability, 45);
            ModifyItem(Item.ByName("Raft.Hook_Plastic"), ItemField.Durability, 45);
            ModifyItem(Item.ByName("Raft.Hook_Plastic"), ItemField.Durability, 50);
            ModifyItem(Item.ByName("Raft.Hook_Scrap"), ItemField.Durability, 125);

            ModifyItem(Item.ByName("Raft.Egg"), ItemField.StackSize, 30);
            ModifyItem(Item.ByName("Raft.SeaVine"), ItemField.StackSize, 30);
            ModifyItem(Item.ByName("Raft.Raw_Beet"), ItemField.StackSize, 40);
            ModifyItem(Item.ByName("Raft.Raw_Potato"), ItemField.StackSize, 40);
            ModifyItem(Item.ByName("Raft.Feather"), ItemField.StackSize, 40);
            ModifyItem(plank, ItemField.StackSize, 40);
            ModifyItem(plastic, ItemField.StackSize, 40);
            ModifyItem(Item.ByName("Raft.Rope"), ItemField.StackSize, 40);
            ModifyItem(Item.ByName("Raft.Thatch"), ItemField.StackSize, 40);
            ModifyItem(stone, ItemField.StackSize, 40);
            ModifyItem(Item.ByName("Raft.Scrap"), ItemField.StackSize, 40);
            ModifyItem(Item.ByName("Raft.Coconut"), ItemField.StackSize, 40);
            ModifyItem(Item.ByName("Raft.Mango"), ItemField.StackSize, 40);
            ModifyItem(Item.ByName("Raft.Seed_Palm"), ItemField.StackSize, 40);
            ModifyItem(Item.ByName("Raft.SharkHead"), ItemField.StackSize, 10);

            ModifyItem(arrow, ItemField.Recipe, AddRecipe(new Recipe(arrow, 6, false, plank, 1, stone, 3, plastic, 3)));
        }

    }
}
