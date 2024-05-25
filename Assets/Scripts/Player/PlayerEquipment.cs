using System.Collections.Generic;
using Inventory;
using Items;
using Messages;
using Pooling;
using Systems;
using UnityEngine;

namespace Player
{
    public class PlayerEquipment : MonoBehaviour, IResetable
    {
        private struct EquipmentEntry
        {
            public GameObject Instance;
            public ItemData Data;
        }

        [SerializeField] private GameObject weaponHolder;
        private Dictionary<EquipmentSlot, EquipmentEntry> m_CurrentEquipment = new Dictionary<EquipmentSlot, EquipmentEntry>();

        // --------------------------------------------------------------------

        private void Awake()
        {
            MessageBuffer<EquippedItemChangedMessage>.Subscribe(OnEquippedItemChanged);
        }

        // --------------------------------------------------------------------

        private void Start()
        {
            SetupCurrentEquipment();
        }

        private void SetupCurrentEquipment()
        {
            Dictionary<EquipmentSlot, InventoryEntry> equipped = GameManager.Instance.Inventory.Equipped;
            foreach (var e in equipped)
            {
                var equipable = e.Value.Item as EquipableItemData;
                if (equipable.AttachOnEquipped)
                    Equip(equipable, EquipmentSlot.Weapon);
            }
        }

        // --------------------------------------------------------------------

        private void OnDestroy()
        {
            MessageBuffer<EquippedItemChangedMessage>.Unsubscribe(OnEquippedItemChanged);
        }

        // --------------------------------------------------------------------

        private void OnEquippedItemChanged(EquippedItemChangedMessage msg)
        {
            if (msg.InventoryEntry != null)
            {
                EquipableItemData equipable = msg.InventoryEntry.Item as EquipableItemData;
                if (equipable.AttachOnEquipped)
                    Equip(equipable, EquipmentSlot.Weapon);
            }
            else
            {
                Unequip(EquipmentSlot.Weapon);
            }
        }

        // --------------------------------------------------------------------

        public void Equip(EquipableItemData equipable, EquipmentSlot slot)
        {
            if (m_CurrentEquipment.ContainsKey(slot))
                Unequip(slot);

            GameObject instance = Instantiate(equipable.EquipPrefab, weaponHolder.transform);

            m_CurrentEquipment.Add(slot, new EquipmentEntry
            {
                Instance = instance,
                Data = equipable
            });

            // Activate the equipped weapon
            if (slot == EquipmentSlot.Weapon)
            {
                instance.SetActive(true);
            }
            else
            {
                instance.SetActive(false);
            }
        }

        private void Unequip(EquipmentSlot type, bool destroy = true)
        {
            if (m_CurrentEquipment.TryGetValue(type, out EquipmentEntry entry))
            {
                if (destroy && Application.isPlaying)
                    Destroy(entry.Instance);

                m_CurrentEquipment.Remove(type);
            }
        }

        // --------------------------------------------------------------------

        public void OnReset()
        {
            RemoveAllEquipment();
            SetupCurrentEquipment();
        }

        // --------------------------------------------------------------------

        private void RemoveAllEquipment()
        {
            foreach (var e in m_CurrentEquipment)
            {
                Destroy(e.Value.Instance);
            }
            m_CurrentEquipment.Clear();
        }
    }
}