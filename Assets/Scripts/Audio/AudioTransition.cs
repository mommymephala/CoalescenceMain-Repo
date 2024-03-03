using FMODUnity;
using UnityEngine;

namespace Audio
{
    public class AudioTransition : MonoBehaviour
    {
        [System.Serializable]
        public class AudioSettings
        {
            [HideInInspector]
            public StudioEventEmitter emitter;
            public string tag = "";
            public string parameter = "";
            public float targetValue;
        }

        public AudioSettings[] audioSettings;
    
        private void OnTriggerEnter()
        {
            if (audioSettings != null)
            {
                foreach (AudioSettings i in audioSettings)
                {
                    if (i.tag == "" || i.parameter == "")
                    {
                        Debug.Log("You have empty fields in an AudioTransition.");
                    
                    }
                    else
                    {
                        i.emitter = GameObject.FindGameObjectWithTag(i.tag).GetComponent<StudioEventEmitter>();
                        i.emitter.SetParameter(i.parameter, i.targetValue);
                        Debug.Log("AudioSettings done");
                    }
                }
            }
            else
                Debug.Log("AudioSettings was NULL");
        }
    }
}
