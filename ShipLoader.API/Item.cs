using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using ShipLoader.API.Exceptions;

namespace ShipLoader.API
{

    /// <summary>
    /// A category for items; this also tells where to put them into the crafting menu
    /// </summary>
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

    /// <summary>
    /// What an item is used for
    /// </summary>
    public enum ItemUse
    {
        /// <summary>
        /// Can't be used in inventory
        /// </summary>
        None,

        /// <summary>
        /// Generic inventory item
        /// </summary>
        Inventory,

        /// <summary>
        /// There's a block attached that should be placed
        /// </summary>
        Buildable,

        /// <summary>
        /// It can be worn into your equipment slots
        /// </summary>
        Equipable,

        /// <summary>
        /// This determines how a purifier looks at it; if it isn't food but it is 'drinkable' then it will be 'cooked' into the recipe item.
        /// Please only use this for water or things affected by the water in a purifier
        /// </summary>
        Drinkable,

        /// <summary>
        /// This isn't used in the game, since blueprint items are Inventory items
        /// </summary>
        Recipe,

        /// <summary>
        /// A tool; something that can be used and damaged.
        /// Something that can be damaged isn't necessarily a tool
        /// </summary>
        Tool
    }

    /// <summary>
    /// A wrapper around Raft's Item_Base, just to make it more mod-friendly
    /// and to support features we would like to add.
    /// </summary>
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
        public Sprite sprite { get; private set; } = null;

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

        /// <summary>
        /// Automatically detects sprite in default asset bundle of mod. (item.{name}) <para />
        /// If you want to manually initialize it, use InitSprite on this item.
        /// </summary>
        /// <param name="name">Name you access the item by</param>
        /// <param name="displayName">Name you see ingame</param>
        /// <param name="description">Description you see ingame</param>
        /// <param name="category">The category this item belongs into (for example; crafting menu)</param>
        /// <param name="use">The use of the item</param>
        /// <param name="durability">The max uses, aka durability of an item</param>
        /// <param name="stackSize">The max stack size</param>
        /// <param name="subcategory">The subcategory for this item; for example, fishing</param>
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

        /// <summary>
        /// Convert this item to an Item_Base
        /// </summary>
        /// <returns>bool success</returns>
        protected bool Init()
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
                item.Initialize(id, fullName, (ItemType)use, durability);
                item.name = fullName;

                //Inventory

                if (sprite == null)      //Look for default sprite
                    sprite = owner.GetAsset<Sprite>("item." + name);

                item.settings_Inventory = new ItemInstance_Inventory(sprite, "Item/" + fullName, stackSize);
                item.settings_Inventory.Description = description;
                item.settings_Inventory.DisplayName = displayName;

                //Stuff required to make items work; as they are used by ItemInstance constructor

                item.settings_buildable = new ItemInstance_Buildable(null, false, false);
                item.settings_consumeable = new ItemInstance_Consumeable(0, 0, false, new Cost(null, 0), FoodType.None);
                item.settings_equipment = new ItemInstance_Equipment(EquipSlotType.None);
                item.settings_usable = new ItemInstance_Usable("", 0, 0, false, false, PlayerAnimation.None, PlayerAnimation.None, false, false, false, "");

                //'Cooking' Recipe (conversion recipe)

                if (convertRecipe != null)
                    item.settings_cookable = new ItemInstance_Cookable(convertRecipe.slots, convertRecipe.time, new Cost(convertRecipe.output.baseItem, convertRecipe.outputAmount));
                else
                    item.settings_cookable = new ItemInstance_Cookable(0, 0, null);

                //Crafting recipe
                RefreshRecipe();

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

        private void RefreshRecipe()
        {
            baseItem.settings_recipe = new ItemInstance_Recipe((CraftingCategory)category, false, recipe == null ? false : recipe.discoveredByDefault, subcategory, 0);

            if (recipe != null)
            {

                if (recipe.blueprint != null)
                    typeof(Item_Base).GetField("BlueprintItem", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(baseItem.settings_recipe, recipe.blueprint.baseItem);

                typeof(ItemInstance_Recipe).GetField("amountToCraft", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(baseItem.settings_recipe, recipe.amount);

                List<CostMultiple> costs = new List<CostMultiple>();

                foreach (RecipeShape shape in recipe.recipe)
                {
                    Item_Base[] itemBase = new Item_Base[shape.items.Length];

                    int j = 0;

                    foreach (Item i in shape.items)
                        itemBase[j++] = i.baseItem;

                    costs.Add(new CostMultiple(itemBase, shape.amount));
                }

                baseItem.settings_recipe.NewCost = costs.ToArray();
            }
        }

        public override string ToString()
        {
            return fullName + " #" + id + " (" + displayName + "): \"" + description + "\", stackSize=" + stackSize + ", durability=" + durability + ", category=" + category.ToString() + ", use=" + use.ToString();
        }

        /// <summary>
        /// Get an item by its handle
        /// </summary>
        /// <param name="item">Handle; the legacy item</param>
        /// <returns>item</returns>
        public static Item ByHandle(Item_Base item)
        {
            if (!items.ContainsKey(item))
                return null;

            return items[item];
        }

        /// <summary>
        /// Get an item by its (fully qualified) name
        /// </summary>
        /// <param name="name">The fully qualified name (Mod.Item)</param>
        /// <returns>item</returns>
        public static Item ByName(string name)
        {
            if (!itemsByName.ContainsKey(name))
                return null;

            return itemsByName[name];
        }

        /// <summary>
        /// Inits the sprite of this Item. <para />
        /// Normally it is done automatically (by looking for item.{name} in the default asset bundle).
        /// </summary>
        /// <param name="sprite">Sprite loaded from your own AssetBundle</param>
        public void InitSprite(Sprite sprite)
        {
            if (this.sprite == null && this.baseItem == null)
                this.sprite = sprite;
        }

        private void Apply(ItemModification mod)
        {

            if (!mod.IsValid())
                throw new ItemModificationException(fullName);

            switch (mod.field)
            {
                case ItemField.DisplayName:

                    displayName = (string) mod.newValue;

                    if (baseItem != null)
                        baseItem.settings_Inventory.DisplayName = displayName;

                    break;

                case ItemField.Description:
                    
                    description = (string) mod.newValue;

                    if (baseItem != null)
                        baseItem.settings_Inventory.Description = description;

                    break;

                case ItemField.Subcategory:

                    subcategory = (string)mod.newValue;

                    if (baseItem != null)
                        typeof(ItemInstance_Recipe).GetProperty("SubCategory").SetValue(baseItem.settings_recipe, subcategory, null);

                    break;

                case ItemField.Durability:
                    
                    durability = (int) mod.newValue;

                    if (baseItem != null)
                        typeof(Item_Base).GetField("maxUses", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(baseItem, durability);
                    
                    break;

                case ItemField.StackSize:

                    stackSize = (int) mod.newValue;

                    if (baseItem != null)
                        typeof(ItemInstance_Inventory).GetField("stackSize", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(baseItem.settings_Inventory, stackSize);

                    break;

                case ItemField.Sprite:

                    sprite = (Sprite) mod.newValue;

                    if (baseItem != null)
                        baseItem.settings_Inventory.Sprite = sprite;

                    break;

                case ItemField.Use:

                    use = (ItemUse) mod.newValue;

                    if (baseItem != null)
                        typeof(Item_Base).GetField("type", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(baseItem, (ItemType) use);

                    break;

                case ItemField.Category:

                    category = (ItemCategory) mod.newValue;

                    if (baseItem != null)
                        typeof(ItemInstance_Recipe).GetProperty("Category", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(baseItem.settings_recipe, (CraftingCategory) category, null);

                    break;

                case ItemField.Recipe:
                    
                    typeof(RaftMod).GetMethod("RemoveRecipe", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(recipe.owner, new object[] { recipe });

                    recipe = (Recipe) mod.newValue;

                    if (baseItem != null)
                        RefreshRecipe();

                    break;
                    
                default:
                    return;
            }
        }

    }
}
