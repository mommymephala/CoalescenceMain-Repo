using FMODUnity;
using UnityEngine;

namespace Audio
{
    public class AudioZoneSettings : MonoBehaviour
    {
        public enum Action
        {
            None,
            Play,
            Stop,
            SetParameter
        }

        [System.Serializable]
        public class AudioSettings
        {
            [HideInInspector]
            public StudioEventEmitter emitter;
            public string tag = "";
            public Action action = Action.None;
            public string parameter = "";
            public float targetValue;
        }

        [NonReorderable] public AudioSettings[] audioSettings;

        private void Start()
        {
            if (audioSettings.Length != 0)
            {
                int number = 1;
                foreach (AudioSettings i in audioSettings)
                {
                    if (i.tag == "" || (i.parameter == "" && i.action == Action.SetParameter) || i.action == Action.None)
                    {
                        Debug.Log("You have unfinished fields in AudioZoneSettings number " + number++);
                    
                    }
                    else
                    {
                        switch (i.action)
                        {
                            case Action.None:
                                Debug.Log("You had an AudioZoneSetting set to 'None'");
                                break;
                            case Action.Play:
                                i.emitter = GameObject.FindGameObjectWithTag(i.tag).GetComponent<StudioEventEmitter>();
                                if (!i.emitter.EventInstance.isValid())
                                    i.emitter.Play();
                                break;
                            case Action.Stop:
                                i.emitter = GameObject.FindGameObjectWithTag(i.tag).GetComponent<StudioEventEmitter>();
                                if (i.emitter.EventInstance.isValid())
                                    i.emitter.Stop();
                                break;
                            case Action.SetParameter:
                                i.emitter = GameObject.FindGameObjectWithTag(i.tag).GetComponent<StudioEventEmitter>();
                                i.emitter.SetParameter(i.parameter, i.targetValue);
                                break;
                        }

                        Debug.Log("AudioZoneSettings number " + number++ + " done");
                    }
                }
            }
            else
                Debug.Log("AudioZoneSettings was NULL");
        }
    }
}