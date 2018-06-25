using System;

namespace ShipLoader.API.Exceptions
{
	public class DuplicateModException : Exception
	{
		public override string Message => $"Duplicate mods with the name '{Data["modname"]}' has been detected.";

		public DuplicateModException(string modName) : base()
		{
			Data.Add("modname", modName);
		}
	}
}
