using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace ShipLoader.API
{
    public enum ItemField
    {
        DisplayName,
        Description,
        Durability,
        StackSize,
        Category,
        Sprite,
        Recipe
    }

    public class Item : IQueueObject
    {

        public RaftMod owner { get; private set; } = null;
        public string name { get; private set; }
        public int id { get; private set; } = 0;
        public string displayName { get; private set; }
        public int durability { get; private set; }
        public int stackSize { get; private set; }
        public string description { get; private set; }
        public CraftingCategory category { get; private set; }
        public Item_Base baseItem { get; private set; } = null;

        public Recipe recipe { get; private set; } = null;

        public Item(string name, string displayName, string description, CraftingCategory category, int durability = 1, int stackSize = 20)
        {
            this.name = name;
            this.displayName = displayName;
            this.durability = durability;
            this.stackSize = stackSize;
            this.description = description;
            this.category = category;
        }

        public bool Init()
        {
            if (id == 0 || owner == null)
            {
                Console.WriteLine("Couldn't initialize item; it has no owner and/or id");
                return false;
            }

            if (baseItem != null)
                return true;

            //Item itself

            Item_Base item = baseItem = ScriptableObject.CreateInstance<Item_Base>();
            item.Initialize(id, name, ItemType.Inventory, durability);
            item.name = displayName;

            //Inventory

            item.settings_Inventory = new ItemInstance_Inventory(null, "Item/" + item.UniqueName, stackSize);
            item.settings_Inventory.Description = description;
            item.settings_Inventory.DisplayName = displayName;

            //Recipe

            item.settings_recipe = new ItemInstance_Recipe(category, false, /* TODO: */ true, /* TODO: */ "", /* TODO: */ 0);
            //TODO: item.settings_recipe.BlueprintItem;

            if (recipe != null)
            {

                typeof(ItemInstance_Recipe).GetField("amountToCraft", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(item.settings_recipe, recipe.amount);

                List<CostMultiple> costs = new List<CostMultiple>();

                foreach (RecipeShape shape in recipe.recipe)
                {
                    Item_Base[] itemBase = new Item_Base[shape.items.Length];

                    int j = 0;

                    foreach (Item i in shape.items)
                        itemBase[j++] = i.baseItem;
                    
                    costs.Add(new CostMultiple(itemBase, shape.number));
                }
                
                item.settings_recipe.NewCost = costs.ToArray();
            }

            //Insertion

            List<Item_Base> items = ItemManager.GetAllItems();

            FieldInfo allAvailableItemsf = typeof(ItemManager).GetField("allAvailableItems", BindingFlags.NonPublic | BindingFlags.Static);

            if (allAvailableItemsf == null)
            {
                Console.WriteLine("Couldn't find items in ItemManager");
                return false;
            }

            items.Add(item);
            allAvailableItemsf.SetValue(null, items);
            return true;
        }

        public bool Conflicts(IQueueObject obj)
        {
            return id == ((Item)obj).id || name == ((Item)obj).name;
        }

        //public void ApplyPatch(ItemField field, object o)
        //{

        //}

        public override string ToString()
        {
            return owner.Metadata.ModName + "." + name + " #" + id + " (" + displayName + "): \"" + description + "\", stackSize=" + stackSize + ", durability=" + durability + ", category=" + category.ToString();
        }

        public void InitRecipe(Recipe recipe)
        {
            if (this.recipe == null)
                this.recipe = recipe;
        }

    }
}
