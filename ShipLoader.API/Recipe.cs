using System.Collections.Generic;
using UnityEngine;

namespace ShipLoader.API
{

    public class RecipeShape
    {

        public Item[] items { get; private set; }
        public int number { get; private set; }

        public RecipeShape(int number, params Item[] items)
        {
            this.number = number;
            this.items = items;
        }

    }

    public class Recipe
    {

        public RaftMod owner { get; private set; } = null;
        public Item result { get; private set; }
        public int amount { get; private set; }
        public RecipeShape[] recipe { get; private set; }
        public RecipeItem recipeItem { get; private set; } = null;

        //Recipe syntax is as following;
        //scrap, plank, plastic
        //scrap, scrap, scrap, plank, plank, plank, plastic, plastic, plastic
        //Item names and amount per item (On the right)
        //scrap, 3, plank, 3, plastic, 3
        //Allowing multiple items as an input
        //scrap, plank, plastic, 3
        //3 Scrap, Plank or Plastic (This is used with shark bait for example)
        //SharkBait: rope, 2, rawPomfret, rawHerring, 2
        public Recipe(Item result, int amount, Item item0, params object[] param)
        {
            this.result = result;
            this.amount = amount;

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

        //Convert existing recipe
        public Recipe(Item it, ItemInstance_Recipe recipe, RaftMod owner)
        {

            amount = recipe.AmountToCraft;
            result = it;
            //recipe.BlueprintItem;

            this.recipe = new RecipeShape[recipe.NewCost.Length];

            int i = 0;
            foreach (CostMultiple cm in recipe.NewCost)
            {
                Item[] items = new Item[cm.items.Length];

                for (int j = 0; j < cm.items.Length; ++j)
                    items[j] = owner.GetItem(cm.items[j]);

                this.recipe[i] = new RecipeShape(cm.amount, items);
                ++i;
            }

        }

        public override string ToString()
        {
            string res = owner.Metadata.ModName + ": " + result.owner.Metadata.ModName + "." + result.name + " x" + amount + ", shape=[";

            bool first0 = true;

            foreach (RecipeShape shape in recipe)
            {

                res += (first0 ? (!(first0 = false) ? "" : "") : ", ") + "(x" + shape.number + ", ";

                bool first1 = true;

                foreach (Item i in shape.items)
                    res += (first1 ? (!(first1 = false) ? "" : "") : ", ") + i.owner.Metadata.ModName + "." + i.name;

                res += ")";
            }

            return res + "]";
        }
    }
}