using UnityEngine;
using VInspector;

namespace Level_Events
{
    public class ActivationManager : MonoBehaviour
    {
        public static ActivationManager Instance { get; private set; }
        
        [SerializeField]
        private SerializedDictionary<string, GameObject> objectDict = new SerializedDictionary<string, GameObject>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

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