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
        Recipe,
        Subcategory
    }

    public enum ItemCategory
    {
        Tools = 0,
        Other = 1,
        Resources = 2,
        Decorations = 3,
        Equipment = 4,
        Nothing = 5,
        FoodWater = 6,
        Navigation = 7,
        CreativeMode = 8
    }

    public class Item
    {

        public RaftMod owner { get; private set; } = null;
        public string name { get; private set; }
        public int id { get; private set; } = 0;
        public string displayName { get; private set; }
        public int durability { get; private set; }
        public int stackSize { get; private set; }
        public string description { get; private set; }
        public Item_Base baseItem { get; private set; } = null;
        public ItemCategory category { get; private set; }
        public string subcategory { get; private set; }

        public Recipe recipe { get; private set; } = null;

        private static Dictionary<Item_Base, Item> items = new Dictionary<Item_Base, Item>();
        private static Dictionary<string, Item> itemsByName = new Dictionary<string, Item>();

        public Item(string name, string displayName, string description, ItemCategory category, int durability = 1, int stackSize = 20, string subcategory = "")
        {
            this.name = name;
            this.displayName = displayName;
            this.durability = durability;
            this.stackSize = stackSize;
            this.description = description;
            this.category = category;
            this.subcategory = subcategory;
        }

        public bool Init()
        {
            if (id == 0 || owner == null)
            {
                Console.WriteLine("Couldn't initialize item; it has no owner and/or id");
                return false;
            }

            Item_Base item = baseItem;

            if (item == null)
            {

                //Item itself

                item = baseItem = ScriptableObject.CreateInstance<Item_Base>();
                item.Initialize(id, name, ItemType.Inventory, durability);
                item.name = displayName;

                //Inventory

                item.settings_Inventory = new ItemInstance_Inventory(null, "Item/" + item.UniqueName, stackSize);
                item.settings_Inventory.Description = description;
                item.settings_Inventory.DisplayName = displayName;

                //Recipe (TODO: maybe split this up? like with SetRecipe or something?)

                item.settings_recipe = new ItemInstance_Recipe((CraftingCategory)category, false, recipe == null ? false : recipe.discoveredByDefault, subcategory, 0);

                if (recipe != null)
                {

                    if (recipe.blueprint != null)
                        typeof(Item_Base).GetField("BlueprintItem", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(item.settings_recipe, recipe.blueprint.baseItem);

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

            }
            else if (Item.items.ContainsKey(item))
                return true;

            Item.items[item] = this;
            Item.itemsByName[owner.Metadata.ModName + "." + name] = this;
            return true;
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

        public static Item ByHandle(Item_Base item)
        {
            if (!items.ContainsKey(item))
                return null;

            return items[item];
        }

        public static Item ByName(string name)
        {
            if (!itemsByName.ContainsKey(name))
                return null;

            return itemsByName[name];
        }

    }
}
