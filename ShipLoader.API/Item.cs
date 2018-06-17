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

    public enum ItemUse
    {
        None,                   //Can't be used in inventory
        Inventory,              //Just generic, it's meant for inventory use
        Buildable,
        Equipable,
        Drinkable,              //An empty bottle is seen as 'drinkable' but FoodType is None
        Recipe,
        Tool
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
        public ItemUse use { get; private set; }
        public string subcategory { get; private set; }

        public string fullName
        {
            get
            {
                return owner.Metadata.ModName + "." + name;
            }
        }

        public Recipe recipe { get; private set; } = null;
        public ConvertRecipe convertRecipe { get; private set; } = null;

        private static Dictionary<Item_Base, Item> items = new Dictionary<Item_Base, Item>();
        private static Dictionary<string, Item> itemsByName = new Dictionary<string, Item>();

        public Item(string name, string displayName, string description, ItemCategory category, ItemUse use, int durability = 1, int stackSize = 20, string subcategory = "")
        {
            this.name = name;
            this.displayName = displayName;
            this.durability = durability;
            this.stackSize = stackSize;
            this.description = description;
            this.category = category;
            this.use = use;
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
                item.Initialize(id, fullName, (ItemType) use, durability);
                item.name = fullName;

                //Inventory

                item.settings_Inventory = new ItemInstance_Inventory(null, "Item/" + fullName, stackSize);
                item.settings_Inventory.Description = description;
                item.settings_Inventory.DisplayName = displayName;

                //Stuff required to make items work; as they are used by ItemInstance constructor

                item.settings_buildable = new ItemInstance_Buildable(null, false, false);
                item.settings_consumeable = new ItemInstance_Consumeable(0, 0, false, null, FoodType.None);
                item.settings_equipment = new ItemInstance_Equipment(EquipSlotType.None);
                item.settings_usable = new ItemInstance_Usable("", 0, 0, false, false, PlayerAnimation.None, PlayerAnimation.None, false, false, false, "");

                //'Cooking' Recipe (conversion recipe)

                if(convertRecipe != null)
                    item.settings_cookable = new ItemInstance_Cookable(convertRecipe.slots, convertRecipe.time, new Cost(convertRecipe.output.baseItem, convertRecipe.amount));
                else
                    item.settings_cookable = new ItemInstance_Cookable(0, 0, null);

                //Crafting recipe

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
            itemsByName[fullName] = this;
            return true;
        }

        //public void ApplyPatch(ItemField field, object o)
        //{

        //}

        public override string ToString()
        {
            return fullName + " #" + id + " (" + displayName + "): \"" + description + "\", stackSize=" + stackSize + ", durability=" + durability + ", category=" + category.ToString() + ", use=" + use.ToString();
        }

        public void InitRecipe(Recipe recipe)
        {
            if (baseItem == null)
                this.recipe = recipe;
        }

        public void InitConvertRecipe(ConvertRecipe recipe)
        {
            if (baseItem == null)
                convertRecipe = recipe;
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
