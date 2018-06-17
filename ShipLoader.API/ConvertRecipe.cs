using System.Collections.Generic;

namespace ShipLoader.API
{
    public class ConvertType
    {
        public const string grill = "Food";
        public const string purifier = "Purifier";
        public const string smelter = "Smelter";
        public const string paintMill = "PaintMill";

        public static List<string> types = new List<string> { grill, purifier, smelter, paintMill };
    }

    public class ConvertRecipe
    {
        
        public ConvertRecipe(Item input, Item output, int amount, float time, string type, int slots = 1)
        {
            this.input = input;
            this.output = output;
            this.amount = amount;
            this.time = time;
            this.type = type;
            this.slots = slots;
        }

        public override string ToString()
        {
            return owner.Metadata.ModName + ": " + input.fullName + " -> " + output.fullName + " x" + amount + " in a " + type + " within " + time + "s taking up " + slots + " slots";
        }

        public RaftMod owner { get; private set; }
        public Item input { get; private set; }
        public int amount { get; private set; }
        public Item output { get; private set; }
        public float time { get; private set; }
        public string type { get; private set; }
        public int slots { get; private set; }

    }
}
