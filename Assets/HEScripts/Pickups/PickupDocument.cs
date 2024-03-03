using HEScripts.Documents;
using HEScripts.Systems;
using HEScripts.UI;
using HorrorEngine;
using UnityEngine;
using UnityEngine.Serialization;

namespace HEScripts.Pickups
{
    public class PickupDocument : Pickup
    {
        [FormerlySerializedAs("Data")]
        [SerializeField] private DocumentData m_Data;
        [SerializeField] private bool m_ReadOnPickup = true;

        public override void Take()
        {
            GameManager.Instance.Inventory.Documents.Add(m_Data);

            if (m_ReadOnPickup)
                Read();

            gameObject.SetActive(false);

            base.Take();
        }

        public void Read()
        {
            UIManager.Get<UIDocument>().Show(m_Data);
        }
    }
}