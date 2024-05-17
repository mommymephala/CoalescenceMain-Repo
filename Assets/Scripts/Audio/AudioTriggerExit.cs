using System.Collections;
using FMODUnity;
using UnityEngine;

namespace Audio
{
    public class AudioTriggerExit : MonoBehaviour
    {
        public bool destroyAfterUse = true;

        public enum Action
        {
            None,
            Play,
            Stop,
            Pause,
            Unpause,
            SetParameter
        }

        [System.Serializable]
        public class AudioTriggerSettings
        {
            public StudioEventEmitter emitter;
            public Action action = Action.None;
            public string parameter = "";
            public float targetValue;
            public float fadeInDuration = 1f; // Default to 1 second for fade in
            public float fadeOutDuration = 1f;
        }

        public AudioTriggerSettings[] audioTriggerSettings;

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                foreach (var settings in audioTriggerSettings)
                {
                    if (settings.emitter == null) continue;

                    switch (settings.action)
                    {
                        case Action.None:
                            break;
                        case Action.Play:
                            StartCoroutine(FadeIn(settings.emitter, 1f, settings.fadeInDuration));
                            break;
                        case Action.Stop:
                            StartCoroutine(FadeOut(settings.emitter, settings.fadeOutDuration, false));
                            break;
                        case Action.Pause:
                            StartCoroutine(FadeOut(settings.emitter, settings.fadeOutDuration, true)); // Fade out and pause
                            break;
                        case Action.Unpause:
                            settings.emitter.EventInstance.setPaused(false);
                            StartCoroutine(FadeIn(settings.emitter, 1f, settings.fadeInDuration));
                            break;
                        case Action.SetParameter:
                            settings.emitter.SetParameter(settings.parameter, settings.targetValue);
                            break;
                    }
                }

                if (destroyAfterUse)
                {
                    Destroy(gameObject);
                }
            }
        }

        private IEnumerator FadeIn(StudioEventEmitter emitter, float finalVolume, float duration)
        {
            emitter.EventInstance.setVolume(0); // Start at volume 0
            emitter.EventInstance.setPaused(false);
            float currentTime = 0;
            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                float newVolume = Mathf.Lerp(0, finalVolume, currentTime / duration);
                emitter.EventInstance.setVolume(newVolume);
                yield return null;
            }
            emitter.EventInstance.setVolume(finalVolume);
        }

        private IEnumerator FadeOut(StudioEventEmitter emitter, float duration, bool shouldPause)
        {
            emitter.EventInstance.getVolume(out float startVolume);
            float currentTime = 0;
            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                float newVolume = Mathf.Lerp(startVolume, 0, currentTime / duration);
                emitter.EventInstance.setVolume(newVolume);
                yield return null;
            }
            emitter.EventInstance.setVolume(0);
            if (shouldPause)
            {
                emitter.EventInstance.setPaused(true);
            }
            else
            {
                emitter.Stop();
            }
        }
    }
}
