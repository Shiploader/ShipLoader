using Harpoon.Core;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;
using ShipLoader.API.Exceptions;
using System.IO;
using ShipLoader.API.AssetBundles;

namespace ShipLoader.API
{
    public class RaftMod : Mod
    {
		private List<AssetBundleReference> _assetBundles = new List<AssetBundleReference>();

		public RaftMod()
		{
			if (mods.ContainsKey(Metadata.ModName))
			{
				throw new DuplicateModException(Metadata.ModName);
			}

			mods[Metadata.ModName] = this;
		}				

		public void RegisterAssetBundle(AssetBundleReference reference)
		{
			_assetBundles.Add(reference);
		}

        //Get an item; returns null if it doesn't exist
        public Item GetItem(string name)
        {
            if (!items.ContainsKey(name))
                return null;

            return items[name];
        }

        //Adds item if it doesn't already exist in the mod
        protected Item AddItem(Item i)
        {
            string name = i.name;

            if (items.ContainsKey(name))
                return items[name];

            bool isOwned = i.owner != null;

            if (i.owner == null)
            {
                typeof(Item).GetProperty("id").SetValue(i, itemOffset, null);
                typeof(Item).GetProperty("owner").SetValue(i, this, null);
            }

            items[name] = i;

            if (!isOwned)
                ++itemOffset;

            Console.WriteLine(i.ToString());
            return i;
        }

        //Adds recipe if it doesn't already exist
        protected Recipe AddRecipe(Recipe r)
        {
            if (recipes.Contains(r))
                return r;

            if (r.owner == null)
                typeof(Recipe).GetProperty("owner").SetValue(r, this, null);

            recipes.Add(r);
            Console.WriteLine(r.ToString());
            return r;
        }

        //Adds 'use' recipe if it doesn't already exist
        protected ConvertRecipe AddRecipe(ConvertRecipe r)
        {
            if (convertRecipes.Contains(r))
                return r;

            if (r.owner == null)
                typeof(ConvertRecipe).GetProperty("owner").SetValue(r, this, null);

            convertRecipes.Add(r);
            Console.WriteLine(r.ToString());
            return r;
        }

        //Registers the recipe into the game
        protected bool RegisterRecipe(ConvertRecipe r)
        {

            FieldInfo itemConnections = typeof(CookingSlot).GetField("itemConnections", BindingFlags.NonPublic | BindingFlags.Instance);

            if (itemConnections == null) return false;

            int count = 0;

            //The recipe/item has to be registered into the converters
            foreach (Item c in converter[r.type])
            {
                Block buildable = c.baseItem.settings_buildable.GetBlockPrefab(DPS.Default);

                if (buildable == null || !(buildable is CookingStand))
                    continue;

                CookingStand stand = (CookingStand)buildable;

                CookingSlot[] slots = stand.GetComponentsInChildren<CookingSlot>();

                foreach (CookingSlot slot in slots)
                {

                    if (slot == null)
                        continue;

                    object connectionso = itemConnections.GetValue(slot);

                    if (connectionso == null)
                        continue;

                    List<CookItemConnection> connections = (List<CookItemConnection>)connectionso;

                    if (connections == null)
                        continue;

                    CookItemConnection recipe = new CookItemConnection();
                    recipe.cookableItem = r.input.baseItem;
                    recipe.rawItem = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    recipe.cookedItem = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    connections.Add(recipe);

                    GameObject.DontDestroyOnLoad(recipe.rawItem);
                    GameObject.DontDestroyOnLoad(recipe.cookedItem);

                    itemConnections.SetValue(slot, connections);
                    ++count;
                }
            }

            return count != 0;
        }

        //Add a 'transformer'; something that transforms items into other items
        //Examples are; smelters, grills, purifiers and paint mills.
        protected void AddConverter(string type, Item[] interfaces)
        {
            if (!converter.ContainsKey(type))
                converter.Add(type, new List<Item>(interfaces));
            else
                converter[type].InsertRange(converter[type].Count, interfaces);
        }

        public static Item[] GetConverters(string type)
        {
            if (!converter.ContainsKey(type))
                return new Item[] { };

            return converter[type].ToArray();
        }

        public static string[] GetConverterTypes()
        {
            string[] names = new string[converter.Count];
            converter.Keys.CopyTo(names, 0);
            return names;
        }

        public Item[] GetItems()
        {
            Item[] items = new Item[this.items.Count];
            this.items.Values.CopyTo(items, 0);
            return items;
        }

        public Recipe[] GetRecipes()
        {
            return recipes.ToArray();
        }

        public ConvertRecipe[] GetConversions()
        {
            return convertRecipes.ToArray();
        }

        public static RaftMod Get(string id)
        {
            if (!mods.ContainsKey(id))
                return null;

            return mods[id];
        }

        public static RaftMod[] Get()
        {
            RaftMod[] mods = new RaftMod[RaftMod.mods.Count];
            RaftMod.mods.Values.CopyTo(mods, 0);
            return mods;
        }
        
        private static Dictionary<string, RaftMod> mods = new Dictionary<string, RaftMod>();
        private static Dictionary<string, List<Item>> converter = new Dictionary<string, List<Item>>();

        private static int itemOffset = 399;

        private Dictionary<string, Item> items = new Dictionary<string, Item>();
        private List<Recipe> recipes = new List<Recipe>();
        private List<ConvertRecipe> convertRecipes = new List<ConvertRecipe>();
    }
}
