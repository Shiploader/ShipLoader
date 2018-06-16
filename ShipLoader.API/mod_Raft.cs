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
            ModVersion = "1.02 (2851750)",
            Priority = 0
        };

        public override void Initialize()
        {
            //Initialize items
            foreach (Item_Base ib in ItemManager.GetAllItems())
                ConvertItem(ib).Init();

            //Initialize recipes
            foreach (Item_Base ib in ItemManager.GetAllItems())
            {
                Item i = Item.ByHandle(ib);

                if (ib.settings_recipe.NewCost.Length != 0)
                    typeof(Item).GetProperty("recipe").SetValue(i, ConvertRecipe(i, ib.settings_recipe), null);
            }
        }

        private Item ConvertItem(Item_Base item)
        {
            Item i = new Item(item.UniqueName, item.settings_Inventory.DisplayName, item.settings_Inventory.Description, (ItemCategory)item.settings_recipe.CraftingCategory, item.MaxUses, item.settings_Inventory.StackSize, item.settings_recipe.SubCategory);
            typeof(Item).GetProperty("owner").SetValue(i, this, null);
            typeof(Item).GetProperty("id").SetValue(i, item.UniqueIndex, null);
            typeof(Item).GetProperty("baseItem").SetValue(i, item, null);
            AddItem(i);
            return i;
        }

        private Recipe ConvertRecipe(Item it, ItemInstance_Recipe recipe)
        {
            Recipe r = new Recipe(it, recipe.AmountToCraft, recipe.LearnedFromBeginning, null);

            if (recipe.BlueprintItem != null)
                typeof(Recipe).GetProperty("blueprint").SetValue(r, Item.ByHandle(recipe.BlueprintItem), null);

            RecipeShape[] shape = new RecipeShape[recipe.NewCost.Length];

            int i = 0;
            foreach (CostMultiple cm in recipe.NewCost)
            {
                Item[] items = new Item[cm.items.Length];

                for (int j = 0; j < cm.items.Length; ++j)
                    items[j] = Item.ByHandle(cm.items[j]);

                shape[i] = new RecipeShape(cm.amount, items);
                ++i;
            }

            typeof(Recipe).GetProperty("recipe").SetValue(r, shape, null);

            return AddRecipe(r);
        }

    }
}
