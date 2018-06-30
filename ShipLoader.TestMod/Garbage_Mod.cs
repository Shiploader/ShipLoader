using Harpoon.Core;
using ShipLoader.API;
using System.ComponentModel;

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
            GameVersion = "1.03",
            Priority = 1
		};

		public override void Initialize()
        {

            Item plastic = Item.ByName("Raft.Plastic");

            garbage = AddItem(new Item("Garbage", "Garbage", "It's just garbage.", ItemCategory.Resources, ItemUse.None));
            AddRecipe(new Recipe(garbage, 1, true, plastic, 6));
            AddRecipe(new ConvertRecipe(garbage, 1, plastic, 5, ConvertType.smelter, 2.5f));

        }

        float timer = 0;

        public void UpdateGame(float dt)
        {

            if (timer >= 10)
            {
                PlayerHelper.Give(garbage, "", 1);
                System.Console.WriteLine("Testing garbage");
                timer = 0;
            }

            timer += dt;
        }

        public void UpdateMenu(float dt) { }
        public void Update(float dt) { }
        public void OnSceneUpdate(SceneUpdateType type) { }
        public void OnGameJoin() { }
        public void OnGameLeave() { }
        public void OnGameChange() { }
        public void OnMenuChange() { }
    }
}
