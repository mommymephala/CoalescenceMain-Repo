using Singleton;
using UnityEngine;
using VInspector;

namespace Level_Events
{
    public class ActivationManager : SingletonBehaviour<ActivationManager>
    {
        [SerializeField]
        private SerializedDictionary<string, GameObject> objectDict = new SerializedDictionary<string, GameObject>();

        public void ActivateObject(string identifier)
        {
            if (objectDict.TryGetValue(identifier, out GameObject obj))
            {
                obj.SetActive(true);
            }
        }

        public void DeactivateObject(string identifier)
        {
            if (objectDict.TryGetValue(identifier, out GameObject obj))
            {
                obj.SetActive(false);
            }
        }
    }
}