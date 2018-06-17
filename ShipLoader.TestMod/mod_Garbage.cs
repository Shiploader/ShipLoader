using Harpoon.Core;
using ShipLoader.API;
using System.ComponentModel;
using UnityEngine;

namespace ShipLoader.TestMod
{

    public class TestModInterface : MonoBehaviour
    {

        int tick = 0;

        public void Update()
        {

            if (tick == 800)
            {
                PlayerHelper.Give(Item.ByName("Garbage.Garbage"), "", 1);
                System.Console.WriteLine("Testing garbage");
                tick = 0;
            }

            ++tick;
        }

    }

    public class mod_Garbage : RaftMod
	{
		[DisplayName("Metadata")]
		public override ModMetadata Metadata => new ModMetadata()
		{
			AuthorName = "Nelus",
			ModName = "Garbage",
			ModDescription = "It's just garbage.",
			ModVersion = "0.1",
            Priority = 1
		};

		public override void Initialize()
        {

            Item plastic = Item.ByName("Raft.Plastic");

            Item garbage = AddItem(new Item("Garbage", "Garbage", "It's just garbage.", ItemCategory.Resources, ItemUse.None));
            AddRecipe(new Recipe(garbage, 1, true, plastic, 6));
            AddRecipe(new ConvertRecipe(garbage, plastic, 5, 2.5f, ConvertType.smelter));

            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sphere.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 3 + Camera.main.transform.right * 0.5f;
            sphere.AddComponent<TestModInterface>();
            GameObject.DontDestroyOnLoad(sphere);
        }

    }
}
