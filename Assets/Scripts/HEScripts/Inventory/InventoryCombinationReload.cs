using HEScripts.Items;
using HEScripts.Systems;
using HorrorEngine;
using UnityEngine;

namespace HEScripts.Inventory
{

    [CreateAssetMenu(menuName = "Horror Engine/Combinations/Reload")]
    public class InventoryCombinationReload : InventoryItemCombination
    {
        public override InventoryEntry OnCombine(InventoryEntry entry1, InventoryEntry entry2)
        {
            WeaponData reloadable1 = entry1.Item as WeaponData;
            WeaponData reloadable2 = entry2.Item as WeaponData;
            if (reloadable1 || reloadable2)
            {
                InventoryEntry reloadableEntry = reloadable1 ? entry1 : entry2;
                InventoryEntry ammoEntry = reloadable1 ? entry2 : entry1;

                WeaponData reloadable = reloadableEntry.Item as WeaponData;
                if (reloadable.ammoItem == ammoEntry.Item)
                {
                    return GameManager.Instance.Inventory.ReloadWeapon(reloadableEntry, ammoEntry);
                }
            }

            return entry1;
        }
    }
}