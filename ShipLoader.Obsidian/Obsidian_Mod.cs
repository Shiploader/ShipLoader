using Harpoon.Core;
using ShipLoader.API;
using UnityEngine;

namespace ShipLoader.TestMod
{

    public class Obsidian_Mod : RaftMod, ISceneListener
    {

        protected Item obsidian;
        protected Item obsidianSword;

        protected GameObject swordInstance;
        
        public override ModMetadata Metadata => new ModMetadata()
        {
            AuthorName = "Nelus",
            ModName = "Obsidian",
            ModDescription = "Add a new material called 'obsidian' to the game.",
            ModVersion = "0.1",
            GameVersion = "1.03",
            Priority = 1
        };

        public override void Initialize()
        {

            Item plastic = Item.ByName("Raft.Plastic");

            obsidian = AddItem(new Item("Obsidian", "Obsidian", "A piece of volcanic glass, used to make sharp and durable weapons and tools.", ItemCategory.Resources, ItemUse.None));
            AddRecipe(new Recipe(obsidian, 1, true, plastic, 6));
            //AddRecipe(new ConvertRecipe(garbage, 1, plastic, 5, ConvertType.smelter, 2.5f));

            obsidianSword = AddItem(new Weapon("Obsidian_dagger", "Obsidian Dagger", "A sharp weapon but also very durable.", 250, 1, 0.2f, 1));
            AddRecipe(new Recipe(obsidianSword, 1, true, Item.ByName("Raft.Plank"), 3, obsidian, 5));
            
        }

        public void OnGameChange() { }
        public void OnGameJoin() { }
        public void OnGameLeave() { }

        public void OnMenuChange()
        {
            swordInstance = GameObject.Instantiate(GetAsset<GameObject>("object.Obsidian_dagger"));
            swordInstance.transform.SetPositionAndRotation(Camera.main.transform.position + Camera.main.transform.forward * 7, Quaternion.Euler(-90, 90, 0));
        }

        public void OnSceneUpdate(SceneUpdateType type) { }
        public void Update(float deltaTime) { }

        public void UpdateGame(float deltaTime) { }

        public void UpdateMenu(float deltaTime) { }
    }
}
