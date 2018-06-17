using System.ComponentModel;
using Harpoon.Core;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;

namespace ShipLoader.API
{
	/// <summary>
	/// A class for handling all mod behaviour that occurs run-time or pre-init
	/// </summary>
	public class ModHelper : MonoBehaviour
	{
		void Start()
		{
			MethodInfo regRecf = typeof(RaftMod).GetMethod("RegisterRecipe", BindingFlags.NonPublic | BindingFlags.Instance);
			PropertyInfo recipep = typeof(Item).GetProperty("recipe");
			PropertyInfo convertRecipep = typeof(Item).GetProperty("convertRecipe");
			MethodInfo initf = typeof(Item).GetMethod("Init", BindingFlags.NonPublic | BindingFlags.Instance);

			foreach (RaftMod mod in RaftMod.Get())
			{
				if (mod.Metadata.ModName != "Raft")
				{
					//Register all recipes into items
					//Which is how it works... for now
					foreach (Recipe recipe in mod.GetRecipes())
					{
						Item i = recipe.result;

						if (recipep.GetValue(i, null) != null)
							Console.WriteLine("Couldn't register recipe; \"" + recipe.ToString() + "\", it was already occupied. This is a Raft Game Architecture problem.");
						else
							recipep.SetValue(i, recipe, null);
					}

					//Register all convert recipes into items
					//Which is how it works... for now
					foreach (ConvertRecipe recipe in mod.GetConversions())
					{

						Item i = recipe.input;

						if (convertRecipep.GetValue(i, null) != null)
							Console.WriteLine("Couldn't register convert recipe; \"" + recipe.ToString() + "\", it was already occupied. This is a Raft Game Architecture problem.");
						else
							convertRecipep.SetValue(i, recipe, null);

					}

					foreach (Item i in mod.GetItems())
						initf.Invoke(i, null);

					//Register recipes
					foreach (ConvertRecipe recipe in mod.GetConversions())
						regRecf.Invoke(mod, new object[] { recipe });
				}
			}
		}
	}

    /// <summary>
    /// A mod that both encapsulates Raft's items as well as providing the ModHelper to simplify item creation.
    /// </summary>
    public class Raft_Mod : RaftMod
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

            MethodInfo initf = typeof(Item).GetMethod("Init", BindingFlags.NonPublic | BindingFlags.Instance);
            
            foreach (Item_Base ib in ItemManager.GetAllItems())
                initf.Invoke(ConvertItem(ib), null);

            //Get all conversion blocks so they can be scanned for recipes later

            AddConverter(ConvertType.grill, 
				new Item[] { Item.ByName("Raft.Placeable_CookingStand_Food_One"), Item.ByName("Raft.Placeable_CookingStand_Food_Two") });
            AddConverter(ConvertType.purifier, 
				new Item[] { Item.ByName("Raft.Placeable_CookingStand_Purifier_One"), Item.ByName("Raft.Placeable_CookingStand_Purifier_Two") });
            AddConverter(ConvertType.smelter, 
				new Item[] { Item.ByName("Raft.Placeable_CookingStand_Smelter") });
            AddConverter(ConvertType.paintMill, 
				new Item[] { Item.ByName("Raft.Placeable_PaintMill") });

            //Initialize recipes
            foreach (Item_Base ib in ItemManager.GetAllItems())
            {
                Item i = Item.ByHandle(ib);

                if (ib.settings_recipe.NewCost.Length != 0)
                    typeof(Item).GetProperty("recipe").SetValue(i, ConvertRecipe(i, ib.settings_recipe), null);

                if (ib.settings_cookable.CookingResult != null && ib.settings_cookable.CookingResult.item != null)
                    typeof(Item).GetProperty("convertRecipe").SetValue(i, ConvertRecipe(i, ib.settings_cookable), null);
            }

            //For pre-initialization tasks;
            //Such as registering recipes, conversion recipes and items

            GameObject modHelper = GameObject.Instantiate(new GameObject());
            modHelper.AddComponent<ModHelper>();
            GameObject.DontDestroyOnLoad(modHelper);
        }

        /// <summary>
        /// Convert an item from legacy to mod, as well as registering it
        /// </summary>
        /// <param name="item">The legacy item to convert</param>
        /// <returns>mod item</returns>
        private Item ConvertItem(Item_Base item)
        {
            Item i = new Item(item.UniqueName, item.settings_Inventory.DisplayName, item.settings_Inventory.Description, (ItemCategory)item.settings_recipe.CraftingCategory, (ItemUse) item.GetType(), item.MaxUses, item.settings_Inventory.StackSize, item.settings_recipe.SubCategory);
            typeof(Item).GetProperty("owner").SetValue(i, this, null);
            typeof(Item).GetProperty("id").SetValue(i, item.UniqueIndex, null);
            typeof(Item).GetProperty("baseItem").SetValue(i, item, null);
            AddItem(i);
            return i;
        }

        /// <summary>
        /// Convert a recipe from legacy to mod, as well as registering it
        /// </summary>
        /// <param name="it">The item this recipe is for</param>
        /// <param name="recipe">The legacy way of declaring a recipe</param>
        /// <returns>mod recipe</returns>
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

        /// <summary>
        /// Converting a conversion recipe from legacy to mod, as well as registering it.
        /// This requires lookup into 'CookingStand', which means that this should be used sparingly
        /// </summary>
        /// <param name="it">The item this recipe belongs to</param>
        /// <param name="recipe">The legacy way of declaring a recipe</param>
        /// <returns>mod recipe</returns>
        private ConvertRecipe ConvertRecipe(Item it, ItemInstance_Cookable recipe)
        {
            string type = "";

			foreach (string t in GetConverterTypes())
			{
				foreach (Item i in GetConverters(t))
				{
					Block block;

					if (i.baseItem == null || (block = i.baseItem.settings_buildable.GetBlockPrefab(DPS.Default)) == null || !(block is CookingStand))
						continue;

					CookingStand stand = (CookingStand)block;

					CookingSlot[] slots = block.GetComponentsInChildren<CookingSlot>();

					if (slots.Length < 1)
						continue;

					CookingSlot slot = slots[0];

					FieldInfo itemConnections = typeof(CookingSlot).GetField("itemConnections", BindingFlags.NonPublic | BindingFlags.Instance);

					if (slot == null || itemConnections == null)
						continue;

					object connectionso = itemConnections.GetValue(slot);

					if (connectionso == null)
						continue;

					List<CookItemConnection> connections = (List<CookItemConnection>)connectionso;

					if (connections == null)
						continue;

					foreach (CookItemConnection connection in connections)
					{
						if (connection.rawItem == null)
							continue;

						if (connection.cookableItem == it.baseItem)
						{
							type = t;
							goto end;
						}
					}
				}
			}

            end:

            if (type == "") return null;

            ConvertRecipe r = new ConvertRecipe(it, 1, Item.ByHandle(recipe.CookingResult.item), recipe.CookingResult.amount, type, recipe.CookingTime, recipe.CookingSlotsRequired);
            typeof(ConvertRecipe).GetProperty("owner").SetValue(r, this, null);
            return AddRecipe(r);
        }
    }
}
