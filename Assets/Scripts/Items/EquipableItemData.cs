using HorrorEngine;
using Inventory;
using Player;
using Systems;
using UnityEngine;

namespace Items
{
    public class EquipableItemData : ItemData
    {
        public GameObject EquipPrefab;
        public EquipmentSlot Slot = EquipmentSlot.Weapon;
        [Tooltip("If true, this item will be attached to the character when used from the inventory selecting the Equip option. In some cases, items might not need to be attached until a action happens")]
        public bool AttachOnEquipped = true;
        [Tooltip("If true, this item will be removed from the current inventory slot when equipped")]
        public bool MoveOutOfInventoryOnEquip;

        public void Equip(InventoryEntry entry)
        {
            var equipment = GameManager.Instance.Player.GetComponent<PlayerEquipment>();
            Inventory.Inventory inventory = GameManager.Instance.Inventory;
            if (entry != null)
            {
                EquipmentSlot slot = inventory.GetOccupyingEquipmentSlot(entry);
                if (slot != EquipmentSlot.None)
                    inventory.Unequip(Slot);
                else
                    inventory.Equip(entry);
            }
            else
            {
                equipment.Equip(this, this.Slot);
            }
        }

    }
}