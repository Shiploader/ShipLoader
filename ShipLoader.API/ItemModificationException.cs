using System;

namespace ShipLoader.API.Exceptions
{
    public class ItemModificationException : Exception
    {
        public override string Message => $"Item modification on item '{Data["itemName"]}' has failed, due to invalid object type or field.";

        public ItemModificationException(string itemName) : base()
        {
            Data.Add("itemName", itemName);
        }
    }
}
