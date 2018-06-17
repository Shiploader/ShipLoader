using System.Collections.Generic;

namespace ShipLoader.API
{
    /// <summary>
    /// The type of a resource conversion;
    /// This can be grill, purify, smelt, paint mill, etc.
    /// </summary>
    public class ConvertType
    {
        public const string grill = "Food";
        public const string purifier = "Purifier";
        public const string smelter = "Smelter";
        public const string paintMill = "PaintMill";
        
        public static List<string> types = new List<string> { grill, purifier, smelter, paintMill };
    }

    /// <summary>
    /// A recipe that turns one resource into another (or multiple) using a block.
    /// This excludes crafting recipes; since they are done through a menu.
    /// </summary>
    public class ConvertRecipe
    {
        
        /// <param name="input">The input of a recipe</param>
        /// <param name="inputAmount">The input amount</param>
        /// <param name="output">The output of a recipe</param>
        /// <param name="outputAmount">The output amount</param>
        /// <param name="type">The type for this resource (See ConvertType)</param>
        /// <param name="time">The amount of time this operation takes (if applicable)</param>
        /// <param name="slots">The number of slots this operation takes (if applicable)</param>
        public ConvertRecipe(Item input, int inputAmount, Item output, int outputAmount, string type, float time = 0, int slots = 1)
        {
            this.input = input;
            this.output = output;
            this.outputAmount = outputAmount;
            this.inputAmount = inputAmount;
            this.time = time;
            this.type = type;
            this.slots = slots;
        }

        public override string ToString()
        {
            return owner.Metadata.ModName + ": " + input.fullName + " x" + inputAmount + " -> " + output.fullName + " x" + outputAmount + " in a " + type + " within " + time + "s taking up " + slots + " slots";
        }

        public RaftMod owner { get; private set; }
        public Item input { get; private set; }
        public int inputAmount { get; private set; }
        public Item output { get; private set; }
        public int outputAmount { get; private set; }
        public float time { get; private set; }
        public string type { get; private set; }
        public int slots { get; private set; }

    }
}
