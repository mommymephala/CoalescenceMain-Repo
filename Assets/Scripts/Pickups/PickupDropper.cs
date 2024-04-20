using Inventory;
using Items;
using Pooling;
using SaveSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pickups
{
    [RequireComponent(typeof(ObjectInstantiator))]
    public class PickupDropper : MonoBehaviour
    {
        [SerializeField] GameObject m_DefaultDropPrefab;

        [SerializeField] InventoryEntry m_Entry;

        ObjectInstantiator m_Instantiator;

        private void Awake()
        {
            m_Instantiator = GetComponent<ObjectInstantiator>();
        }

        public void Drop()
        {
            Drop(m_Entry);
        }

        public void Drop(InventoryEntry entry)
        {
            ItemData item = entry.Item;

            GameObject prefab = item.dropPrefab ? item.dropPrefab.gameObject : m_DefaultDropPrefab;
            GameObject instance = m_Instantiator.Instatiate(prefab);

            instance.transform.SetParent(null);
            SceneManager.MoveGameObjectToScene(instance, SceneManager.GetActiveScene());

            var objectId = instance.GetComponent<ObjectUniqueId>();
            objectId.RegenerateId();

            PickupItem pickup = instance.GetComponent<PickupItem>();
            if (pickup)
            {
                pickup.Entry.Copy(entry);
            }

            SpawnableSavable savable = instance.GetComponent<SpawnableSavable>();
            if (savable)
            {
                savable.SetPrefab(prefab);
                savable.AddToManager();
            }
   
        }
    }
}
