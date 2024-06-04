using System.Collections;
using UnityEngine;

namespace Level_Events.Lights
{
    public class LightControl : MonoBehaviour
    {
        [SerializeField] private float transitionDuration = 1f;
        private float _targetIntensity;
        private Coroutine _transitionCoroutine;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _targetIntensity = 0f;
                StartTransition();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _targetIntensity = 0.5f;
                StartTransition();
            }
        }

        private void StartTransition()
        {
            if (_transitionCoroutine != null)
            {
                StopCoroutine(_transitionCoroutine);
            }
            
            _transitionCoroutine = StartCoroutine(TransitionIntensity());
        }

        private IEnumerator TransitionIntensity()
        {
            var elapsedTime = 0f;
            var startIntensity = RenderSettings.reflectionIntensity;
            while (elapsedTime < transitionDuration)
            {
                elapsedTime += Time.deltaTime;
                var t = Mathf.Clamp01(elapsedTime / transitionDuration);
                RenderSettings.reflectionIntensity = Mathf.Lerp(startIntensity, _targetIntensity, t);
                yield return null;
            }
            
            RenderSettings.reflectionIntensity = _targetIntensity;
            _transitionCoroutine = null;
        }
    }
}