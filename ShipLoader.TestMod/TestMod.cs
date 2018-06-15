using Harpoon.Core;
using ShipLoader.API;
using System.ComponentModel;
using UnityEngine;
using System;

namespace ShipLoader.TestMod
{

    public class TestModInterface : MonoBehaviour
    {

        public void Start() {

        }

    }

    public class TestMod : RaftMod
	{
		[DisplayName("Metadata")]
		public override ModMetadata Metadata => new ModMetadata()
		{
			AuthorName = "Nelus",
			ModName = "Garbage",
			ModDescription = "It's just garbage.",
			ModVersion = "0.1"
		};

		public override void Initialize()
        {
            Console.WriteLine("Creating plane");
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sphere.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 3 + Camera.main.transform.right * 0.5f;

            Console.WriteLine("Creating component");
            sphere.AddComponent<TestModInterface>();

            ModHelper.Init();

            Item garbage = AddItem(new Item("Garbage", "Garbage", "It's just garbage.", ItemCategory.Resources));
            garbage.InitRecipe(AddRecipe(new Recipe(garbage, 1, true, Item.ByName("Raft.Plastic"), 6)));
            garbage.Init();

            GameObject.DontDestroyOnLoad(sphere);
        }
	}
}
