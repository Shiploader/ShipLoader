using System.Collections.Generic;
using UnityEngine;

namespace ShipLoader.API
{

    /// <summary>
    /// A RecipeShape is a combination of items that can be used 'amount' of times in a recipe.
    /// Normally, this is just 1 item, but shark bait is an example where you can use either a herring or a pomfret (you need 2 of either or 1 of either)
    /// </summary>
    public class RecipeShape
    {

        public Item[] items { get; private set; }
        public int amount { get; private set; }

        public RecipeShape(int amount, params Item[] items)
        {
            this.amount = amount;
            this.items = items;
        }

    }

    /// <summary>
    /// A recipe is the representation of a crafting recipe in the game
    /// </summary>
    public class Recipe
    {

        public RaftMod owner { get; private set; } = null;

        public Item result { get; private set; }
        public int amount { get; private set; }

        public RecipeShape[] recipe { get; private set; }
        public RecipeItem recipeItem { get; private set; } = null;

        public Item blueprint { get; private set; } = null;
        public bool discoveredByDefault { get; private set; } = false;

        /// <summary>
        /// Insert a number to the right of the item(s) you need for a RecipeShape.
        /// Example:<para />
        /// new Recipe(garbage, 1, true, plastic, 6)<para />
        /// new Recipe(garbage, 1, true, plastic, 6, scrap, 8)<para />
        /// new Recipe(garbage, 1, true, plastic, scrap, 6)<para />
        /// The first line requires 6 plastic, the second 6 plastic and 8 scrap and the third requires 6 scrap or plastic (either is fine).
        /// </summary>
        /// <param name="result">The result of the recipe</param>
        /// <param name="amount">The amount the recipe returns</param>
        /// <param name="discoveredByDefault">Whether or not the recipe is discovered by default</param>
        public Recipe(Item result, int amount, bool discoveredByDefault, Item item0, params object[] param)
        {
            this.result = result;
            this.amount = amount;
            this.discoveredByDefault = discoveredByDefault;

            List<Item> items = new List<Item>();
            List<RecipeShape> shapes = new List<RecipeShape>();
            items.Add(item0);

            foreach(object o in param)
            {

                if (o is int)
                {
                    if(items.Count == 0 || (int)o < 1)
                        throw new System.InvalidOperationException("Invalid Recipe; there has to be something left of the amount");

                    shapes.Add(new RecipeShape((int)o, items.ToArray()));
                    items.Clear();

                }
                else if (o is Item)
                    items.Add((Item)o);
                else
                    throw new System.InvalidOperationException("Invalid Recipe; it only allows an Item or int as parameter");

            }

            foreach (Item i in items)
                shapes.Add(new RecipeShape(1, i));

            recipe = shapes.ToArray();
        }

        /// <summary>
        /// Insert a number to the right of the item(s) you need for a RecipeShape.
        /// Example:<para />
        /// new Recipe(garbage, 1, true, plastic, 6)<para />
        /// new Recipe(garbage, 1, true, plastic, 6, scrap, 8)<para />
        /// new Recipe(garbage, 1, true, plastic, scrap, 6)<para />
        /// The first line requires 6 plastic, the second 6 plastic and 8 scrap and the third requires 6 scrap or plastic (either is fine).
        /// </summary>
        /// <param name="blueprint">The blueprint that unlocks this recipe</param>
        /// <param name="result">The result of the recipe</param>
        /// <param name="amount">The amount the recipe returns</param>
        /// <param name="discoveredByDefault">Whether or not the recipe is discovered by default</param>
        public Recipe(Item blueprint, Item result, int amount, Item item0, params object[] param): this(result, amount, false, item0, param)
        {
            this.blueprint = blueprint;
        }

        public override string ToString()
        {
            string res = owner.Metadata.ModName + ": " + result.fullName + " x" + amount + ", shape=[";

            bool first0 = true;

            foreach (RecipeShape shape in recipe)
            {

                res += (first0 ? (!(first0 = false) ? "" : "") : ", ") + "(x" + shape.amount + ", ";

                bool first1 = true;

                foreach (Item i in shape.items)
                    res += (first1 ? (!(first1 = false) ? "" : "") : ", ") + i.fullName;

                res += ")";
            }

            return res + "]";
        }
    }
}