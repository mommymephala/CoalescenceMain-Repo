using UnityEngine;

namespace HEScripts.Pooling 
{
    public class ObjectInstantiator : MonoBehaviour
    {
        [SerializeField] private GameObject Prefab;
        [SerializeField] private ObjectInstantiationSettings Settings;

        public GameObject Instatiate(GameObject prefab = null)
        {
            if (prefab)
                Prefab = prefab;

            return Prefab ? Settings.Instantiate(Prefab) : null;
        }
    }
}