using System;
using System.Linq;
using Harpoon.Core;

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
                mainMod.AddItem(ConvertItem(mainMod, ib));

            //Initialize recipes
            foreach(Item_Base ib in ItemManager.GetAllItems())
            {
                Item i = mainMod.GetItem(ib);

                if (ib.settings_recipe.NewCost.Length != 0)
                {
                    Recipe rec;
                    typeof(Item).GetProperty("recipe").SetValue(i, rec = mainMod.AddRecipe(new Recipe(i, ib.settings_recipe, mainMod)), null);
                }
            }
        }

        public static Item ConvertItem(RaftMod mod, Item_Base item)
        {
            Item i = new Item(item.UniqueName, item.settings_Inventory.DisplayName, item.settings_Inventory.Description, item.settings_recipe.CraftingCategory, item.MaxUses, item.settings_Inventory.StackSize);
            typeof(Item).GetProperty("owner").SetValue(i, mod, null);
            typeof(Item).GetProperty("id").SetValue(i, item.UniqueIndex, null);
            typeof(Item).GetProperty("baseItem").SetValue(i, item, null);
            return i;
        }

        //modObjectName;
        //Scrap = .Scrap = Raft.Scrap (The scrap item)
        //Test.Tin (The tin ingot from the test mod)
        public static Item GetModItem(string item)
        {
            RaftMod owner = mainMod;

            if (!item.Contains('.'))
                return owner.GetItem(item);

            owner = GetMod(item);
            item = item.Substring(item.IndexOf('.'));

            if (owner == null)
                return mainMod.GetItem("Undefined");

            return owner.GetItem(item);
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
