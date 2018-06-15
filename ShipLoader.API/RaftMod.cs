using Harpoon.Core;
using System.Collections.Generic;
using System;

namespace ShipLoader.API
{

    public class RaftMod : Mod
    {

        //Get an item; returns null if it doesn't exist
        public Item GetItem(string name)
        {

            if (!items.ContainsKey(name))
                return null;

            return items[name];
        }

        //Adds item if it doesn't already exist in the mod
        public Item AddItem(Item i)
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

            if(!isOwned)
                ++itemOffset;
            
            Console.WriteLine(i.ToString());
            return i;
        }

        //Adds recipe if it doesn't already exist in the mod
        public Recipe AddRecipe(Recipe r)
        {

            if (recipes.Contains(r))
                return r;

            if (r.owner == null)
                typeof(Recipe).GetProperty("owner").SetValue(r, this, null);

            recipes.Add(r);
            Console.WriteLine(r.ToString());
            return r;
        }
        
        private Dictionary<string, Item> items = new Dictionary<string, Item>();
        private List<Recipe> recipes = new List<Recipe>();

        private static int itemOffset = 399;
    }
}
