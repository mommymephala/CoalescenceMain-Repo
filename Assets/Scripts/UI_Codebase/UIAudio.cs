using UnityEngine;
using FMODUnity;

namespace UI_Codebase
{
    public class UIAudio : MonoBehaviour
    {
        public void Play(EventReference soundEvent)
        {
            if (!soundEvent.IsNull)
            {
                RuntimeManager.PlayOneShot(soundEvent);
            }
            else
            {
                Debug.LogWarning("FMOD event reference is not set or is invalid", this);
            }
        }
    }
}