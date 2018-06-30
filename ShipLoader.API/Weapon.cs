using System.Reflection;

namespace ShipLoader.API
{
    
    public class Weapon : Item
    {

        public int damage { get; private set; }
        public float range { get; private set; }
        public float cooldown { get; private set; }

        /// <summary>
        /// Automatically detects sprite in default asset bundle of mod. (item.{name}) <para />
        /// If you want to manually initialize it, use InitSprite on this item.
        /// </summary>
        /// <param name="name">Name you access the item by</param>
        /// <param name="displayName">Name you see ingame</param>
        /// <param name="description">Description you see ingame</param>
        /// <param name="durability">The max uses, aka durability of an item</param>
        /// <param name="damage">The damage this weapon does</param>
        /// <param name="cooldown">The cooldown of this weapon</param>
        /// <param name="range">The range of this weapon (3 units by default)</param>
        /// <param name="subcategory">The subcategory for this item; for example, fishing</param>
        public Weapon(string name, string displayName, string description, int durability, int damage, float cooldown, float range = 3, string subcategory = ""):
            base(name, displayName, description, ItemCategory.Tools, ItemUse.Tool, durability, 1, subcategory)
        {
            this.damage = damage;
            this.range = range;
            this.cooldown = cooldown;
        }

        protected override bool Init()
        {
            
            if (!base.Init())
                return false;

            baseItem.settings_usable = new ItemInstance_Usable("LMD", cooldown, 0, true, true, PlayerAnimation.Index_6_Spear, PlayerAnimation.Trigger_ItemHitTrigger, true, true, false, "");
            return true;
        }

        protected override void InitPlayer(Network_Player player)
        {

            FieldInfo eventRef_swingv = typeof(Spear).GetField("eventRef_swing", BindingFlags.NonPublic | BindingFlags.Instance);

            Spear spear = player.gameObject.AddComponent<Spear>();
            spear.name = fullName;
            spear.attackMask = player.GetComponentsInChildren<Spear>(true)[0].attackMask;
            eventRef_swingv.SetValue(spear, eventRef_swingv.GetValue(player.GetComponentsInChildren<Spear>(true)[0]));
            spear.enabled = false;

            FieldInfo attackRangev = typeof(Spear).GetField("attackRange", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo damagev = typeof(Spear).GetField("damage", BindingFlags.NonPublic | BindingFlags.Instance);

            attackRangev.SetValue(spear, range);
            damagev.SetValue(spear, damage);

            System.Console.WriteLine(owner.Metadata.ModName + ": Added weapon " + fullName + "; damage=" + damage + ", range=" + range + ", cooldown=" + cooldown);

            //The spear probably stores a parented object that represents the actual mesh

            //foreach (Spear sp in player.GetComponentsInChildren<Spear>())
            //{
            //    System.Console.WriteLine(sp.GetType().ToString());
                


            //}

            //TODO: Still not useable, find out how it works
            //TODO: Also things that are already weapons should be added here

        }

    }
}
