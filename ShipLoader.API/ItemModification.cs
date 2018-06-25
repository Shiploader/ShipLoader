namespace ShipLoader.API
{
    
    /// <summary>
    /// A property of an Item that can be edited by mods
    /// </summary>
    public enum ItemField
    {
        DisplayName,
        Description,
        Durability,
        StackSize,
        Category,
        Sprite,
        Subcategory,
        Recipe,
        Use
    }

    public class ItemModification
    {

        public RaftMod owner { get; private set; }
        public Item item { get; private set; }
        public ItemField field { get; private set; }
        public object newValue { get; private set; }

        public ItemModification(RaftMod owner, Item item, ItemField field, object newValue)
        {
            this.owner = owner;
            this.item = item;
            this.field = field;
            this.newValue = newValue;
        }

        public bool IsValid()
        {
            switch (field)
            {
                case ItemField.DisplayName:
                case ItemField.Description:
                case ItemField.Subcategory:
                    return newValue is string;

                case ItemField.Durability:
                case ItemField.StackSize:
                    return newValue is int;

                case ItemField.Sprite:
                    return newValue is UnityEngine.Sprite;

                case ItemField.Use:
                    return newValue is ItemUse;

                case ItemField.Category:
                    return newValue is ItemCategory;

                case ItemField.Recipe:
                    return newValue is Recipe;

                default:
                    return false;
            }
        }

    }
}
