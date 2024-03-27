using System;
using HEScripts.Inventory;
using HEScripts.SaveSystem;
using UnityEngine;

namespace HEScripts.Items
{
    [Flags]
    public enum InventoryMainAction
    {
        None,
        Use,
        Equip
    }

    [Flags]
    public enum ItemFlags
    {
        Bulkable            = 1 << 1,
        ConsumeOnUse        = 1 << 2,
        Combinable          = 1 << 3,
        Examinable          = 1 << 4,
        Droppable           = 1 << 5,
        CreatePickupOnDrop  = 1 << 6,
        UseOnInteractive    = 1 << 7,
        Depletable          = 1 << 8,
        Movable            = 1 << 9,
    }

    [CreateAssetMenu(menuName = "Horror Engine/Items/Item")]
    public class ItemData : Register
    {
        public Sprite image;
        public GameObject examineModel;
        public string Name;
        public string description;
        public InventoryMainAction inventoryAction = InventoryMainAction.Use;
        public ItemFlags flags;
        public SpawnableSavable dropPrefab;

        public virtual void OnUse(InventoryEntry entry) { }
    }
}