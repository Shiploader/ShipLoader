using Harpoon.Core;
using System.ComponentModel;
using UnityEngine;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.IO;

namespace ShipLoader.TestMod
{

    public class TestModInterface : MonoBehaviour
    {

        public void Start() {
            
            FieldInfo allAvailableItemsf = typeof(ItemManager).GetField("allAvailableItems", BindingFlags.NonPublic | BindingFlags.Static);

            Console.WriteLine("Getting 'allAvailableItems' FieldInfo");
            if (allAvailableItemsf == null)
                return;

            object allAvailableItems = allAvailableItemsf.GetValue(null);

            Console.WriteLine("Getting 'allAvailableItems' value");
            if (allAvailableItems == null)
                return;

            List<String> lines = new List<String>();
            List<Item_Base> items = (List<Item_Base>) allAvailableItems;

            Console.WriteLine("Getting 'items'");

            foreach (Item_Base ib in items)
                lines.Add(ib.UniqueIndex + " " + ib.name  + ", durability=" + ib.MaxUses + ", stacksize=" + ib.settings_Inventory.StackSize);

            Console.WriteLine("Writing 'items'");

            File.WriteAllLines("Items.txt", lines.ToArray());
        }

    }

    public class TestMod : Mod
	{
		[DisplayName("Metadata")]
		public override ModMetadata Metadata => new ModMetadata()
		{
			AuthorName = "Veld",
			ModName = "Test Mod",
			ModDescription = "This is a ShipLoader test mod!",
			ModVersion = "0.1"
		};

		public override void Initialize()
        {
            Console.WriteLine("Creating plane");
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sphere.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 3 + Camera.main.transform.right * 0.5f;

            Console.WriteLine("Creating component");
            sphere.AddComponent<TestModInterface>();

            GameObject.DontDestroyOnLoad(sphere);
        }
	}
}
