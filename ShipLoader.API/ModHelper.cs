using System.Linq;
using Harpoon.Core;
using System.Reflection;
using System.Collections.Generic;

namespace ShipLoader.API
{
    public class ModHelper
    {

        public static void Init()
        {

            if (mainMod != null)
                return;

            mainMod = new mod_Raft();

            //Initialize items
            foreach (Item_Base ib in ItemManager.GetAllItems())
                ConvertItem(mainMod, ib).Init();

            //Initialize recipes
            foreach(Item_Base ib in ItemManager.GetAllItems())
            {
                Item i = Item.ByHandle(ib);

                if (ib.settings_recipe.NewCost.Length != 0)
                    typeof(Item).GetProperty("recipe").SetValue(i, ConvertRecipe(mainMod, i, ib.settings_recipe), null);
            }
        }

        public static Item ConvertItem(RaftMod mod, Item_Base item)
        {
            Item i = new Item(item.UniqueName, item.settings_Inventory.DisplayName, item.settings_Inventory.Description, (ItemCategory) item.settings_recipe.CraftingCategory, item.MaxUses, item.settings_Inventory.StackSize, item.settings_recipe.SubCategory);
            typeof(Item).GetProperty("owner").SetValue(i, mod, null);
            typeof(Item).GetProperty("id").SetValue(i, item.UniqueIndex, null);
            typeof(Item).GetProperty("baseItem").SetValue(i, item, null);
            mod.AddItem(i);
            return i;
        }

        public static Recipe ConvertRecipe(RaftMod mod, Item it, ItemInstance_Recipe recipe)
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

            return mod.AddRecipe(r);
        }

        public static RaftMod GetMod(string modName)
        {
            if (modName == "" || modName.ToLower() == "raft")
                return mainMod;

            Mod mod = HarpoonCore.GetMod(modName);
            if (mod == null || !(mod is RaftMod))
                return null;

            return (RaftMod) mod;
        }
        
        private static mod_Raft mainMod = null;
    }
}
