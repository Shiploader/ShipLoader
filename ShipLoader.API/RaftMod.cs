using Harpoon.Core;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;
using ShipLoader.API.Exceptions;
//using System.IO;
using ShipLoader.API.AssetBundles;
using System.ComponentModel;

namespace ShipLoader.API
{

    public class RaftMod : Mod
    {

        public RaftMod()
		{
			if (mods.ContainsKey(Metadata.ModName))
			{
				throw new DuplicateModException(Metadata.ModName);
			}

			mods[Metadata.ModName] = this;

            if(defaultAssetBundle == null)
                defaultAssetBundle = "./mods/" + Metadata.ModName + "/" + Metadata.ModName + "." + Metadata.ModVersion;

            RegisterAssetBundle(assetBundle = new AssetBundleReference(defaultAssetBundle));
		}				

		public int RegisterAssetBundle(AssetBundleReference reference)
		{
            int id = assetBundles.Count;
			assetBundles.Add(reference);
            return id;
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
                    recipe.rawItem = GameObject.CreatePrimitive(PrimitiveType.Sphere);
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
		protected void AddConverter(string type, params Item[] interfaces)
		{
			if (!converter.ContainsKey(type))
			{
				converter.Add(type, new List<Item>(interfaces));
			}

			else
			{
				converter[type].InsertRange(converter[type].Count, interfaces);
			}
		}

        public static IEnumerable<Item> GetConverters(string type)
        {
			if (!converter.ContainsKey(type))
				return null;

			return converter[type];
        }

        public static IEnumerable<string> GetConverterTypes()
        {
            return converter.Keys;
        }

        public IEnumerable<Item> GetItems()
        {
            return items.Values;
        }

        public IEnumerable<Recipe> GetRecipes()
        {
            return recipes;
        }

        public IEnumerable<ConvertRecipe> GetConversions()
        {
            return convertRecipes;
        }

        /// <summary>
        /// Get an asset from an asset bundle
        /// </summary>
        /// <param name="assetName">The asset to look up</param>
        /// <param name="id">Optional, used for getting assets from different asset bundles (besides the default 0th asset bundle)</param>
        /// <returns></returns>
        public T GetAsset<T>(string assetName, int id = 0) where T : UnityEngine.Object
        {
            return assetBundles[id].assetBundle.LoadAsset<T>(assetName);
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

        private List<AssetBundleReference> assetBundles = new List<AssetBundleReference>();

        /// <summary>
        /// The default asset bundle it should load <para />
        /// If this is not set, it will use ModName.ModVersion by default <para />
        /// This means that Garbage mod version 0.1 would use asset bundle Garbage.0.1 in the mod directory <para />
        /// If you want to load multiple asset bundles, you can use RegisterAssetBundle yourself.
        /// </summary>
        protected string defaultAssetBundle = null;

        public AssetBundleReference assetBundle { get; private set; } = null;
    }
}
