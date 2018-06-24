using Harpoon.Core;
using ShipLoader.API;
using System.ComponentModel;
using UnityEngine;

namespace ShipLoader.TestMod
{
    
    public class Garbage_Mod : RaftMod, ISceneListener
	{

        protected Item garbage;

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

            garbage = AddItem(new Item("Garbage", "Garbage", "It's just garbage.", ItemCategory.Resources, ItemUse.None));
            AddRecipe(new Recipe(garbage, 1, true, plastic, 6));
            AddRecipe(new ConvertRecipe(garbage, 1, plastic, 5, ConvertType.smelter, 2.5f));

            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sphere.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 3 + Camera.main.transform.right * 0.5f;
            GameObject.DontDestroyOnLoad(sphere);
        }

        float timer = 0;

        public void UpdateGame()
        {

            if (timer >= 10)
            {
                PlayerHelper.Give(garbage, "", 1);
                System.Console.WriteLine("Testing garbage");
                timer = 0;
            }

            timer += Time.deltaTime;
        }

        public void UpdateMenu() { }
        public void Update() { }
        public void OnSceneUpdate(SceneUpdateType type) { }
        public void OnGameJoin() { }
        public void OnGameLeave() { }
        public void OnGameChange() { }
        public void OnMenuChange() { }
    }
}
