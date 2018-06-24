using System.Reflection;

namespace ShipLoader.API
{
    public class PlayerHelper
    {

        //Get the local player
        //If the player doesn't exist (like if you're not ingame), it returns ""
        public static string GetLocalPlayer()
        {
            Semih_Network network = UnityEngine.Object.FindObjectOfType<Semih_Network>();

            if (network == null)
                return "";

            Network_Player player = network.GetLocalPlayer();

            return player == null ? "" : player.name;
        }

        //Get the players
        //If there is no active game, or no players; it returns []
        public static string[] GetPlayers()
        {

            Semih_Network network = UnityEngine.Object.FindObjectOfType<Semih_Network>();

            if (network == null)
                return new string[] { };

            string[] names = new string[network.PlayerCount];

            int i = 0;
            foreach (var type in network.remoteUsers)
                names[i++] = type.Value.name;

            return names;
        }

        //Interface for getting a player (requires Assembly-CSharp)
        public static Network_Player GetPlayer(string name)
        {

            Semih_Network network = UnityEngine.Object.FindObjectOfType<Semih_Network>();

            if (network == null)
                return null;

            if (name == "")
                return network.GetLocalPlayer() == null ? null : network.GetLocalPlayer();

            foreach (var user in network.remoteUsers)
                if (user.Value != null && user.Value.name == name)
                    return user.Value;

            return null;
        }

        //Give a player an item
        //If you don't specify player, it picks local player
        //If you don't specify count, it assumes full stack
        public static bool Give(Item item, string player = "", int count = 0)
        {

            Network_Player p = GetPlayer(player);

            if (p == null || item == null || item.baseItem == null)
                return false;

            PlayerInventory playerInventory = (PlayerInventory)typeof(Network_Player).GetField("inventory", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(p);

            if (playerInventory == null)
                return false;

            playerInventory.AddItem(item.baseItem.UniqueName, count);
            return true;
        }

    }
}
