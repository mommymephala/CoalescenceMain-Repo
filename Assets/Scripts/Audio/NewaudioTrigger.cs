using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
public class NewaudioTrigger : MonoBehaviour
{
    public bool destroyAfterUse = true;

        public enum Action
        {
            None,
            Play,
            Stop,
            Pause,
            SetParameter
        }

        [System.Serializable]
        public class AudioTriggerSettings
        {
            public StudioEventEmitter emitter;
            public string tag = "";
            public Action action = Action.None;
            public string parameter = "";
            public float targetValue;

            public float fadeInDuration = 1f; // Default to 1 second for fade in
            public float fadeOutDuration = 1f; 
            // public bool shouldPauseInsteadOfStop = false;
        }

        public AudioTriggerSettings[] audioTriggerSettings;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                foreach (var settings in audioTriggerSettings)
                {
                    settings.emitter = GameObject.FindGameObjectWithTag(settings.tag).GetComponent<StudioEventEmitter>();

                    switch (settings.action)
                    {
                        case Action.None:
                            break;
                        case Action.Play:
                            StartCoroutine(FadeIn(settings.emitter, 1f, settings.fadeInDuration)); // Fade in over 1 second
                            break;
                        case Action.Stop:
                            StartCoroutine(FadeOut(settings.emitter, settings.fadeOutDuration, false)); // Fade out and stop
                            break;
                        case Action.Pause:
                            StartCoroutine(FadeOut(settings.emitter, settings.fadeOutDuration, true)); // Fade out and pause
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
            emitter.Play();
            emitter.EventInstance.setVolume(0); // Start at volume 0
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

