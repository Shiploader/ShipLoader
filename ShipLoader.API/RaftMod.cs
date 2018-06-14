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
        public Item GetItem(Item_Base baseItem)
        {
            foreach (Item i in items.Values)
                if (i.baseItem == baseItem)
                    return i;
            
            return null;
        }

        //Adds item if it doesn't already exist in the mod
        //Returns null if it conflicts with an existing definition
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
            operations["items." + name] = i;

            if(!isOwned)
                ++itemOffset;

            Console.WriteLine(i.ToString());
            return i;
        }

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

        //Checks all queue objects against all mods
        public bool Conflicts(List<RaftMod> mods)
        {
            foreach (RaftMod mod in mods)
                if (mod != this)
                    foreach (IQueueObject obj0 in operations.Values)
                        foreach (IQueueObject obj1 in mod.operations.Values)
                            if (obj0.GetType() == obj1.GetType() && obj0.Conflicts(obj1))
                                return true;
            return false;
        }
        
        private Dictionary<string, Item> items = new Dictionary<string, Item>();
        private Dictionary<string, IQueueObject> operations = new Dictionary<string, IQueueObject>();
        private List<Recipe> recipes = new List<Recipe>();

        private static int itemOffset = 399;
    }
}
