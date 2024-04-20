using Combat;
using Inventory;
using Systems;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(menuName = "Items/HealthKit")]
    public class HealthKitData : ItemData
    {
        [SerializeField] private float m_Regeneration;
        [SerializeField] private bool m_CompleteRegeneration;

        public override void OnUse(InventoryEntry entry)
        {
            base.OnUse(entry);

            if (m_CompleteRegeneration)
                GameManager.Instance.Player.GetComponent<Health>().RegenerateAll();
            else
                GameManager.Instance.Player.GetComponent<Health>().Regenerate(m_Regeneration);
        }
    }
}