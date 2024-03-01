using System.Collections.Generic;
using UnityEngine;

namespace Level_Events.Lights
{
    public class LightFlicker : MonoBehaviour 
    {
        [Tooltip("External light to flicker; you can leave this null if you attach script to a light")]
        public new Light light;
        [Tooltip("Minimum random light intensity")]
        public float minIntensity;
        [Tooltip("Maximum random light intensity")]
        public float maxIntensity = 1f;
        [Tooltip("How much to smooth out the randomness; lower values = sparks, higher = lantern")]
        [Range(1, 50)]
        public int smoothing = 5;

        private Queue<float> _smoothQueue;
        private float _lastSum;
    
        public void Reset() 
        {
            _smoothQueue.Clear();
            _lastSum = 0;
        }

        private void Start() 
        {
            _smoothQueue = new Queue<float>(smoothing);
            if (light == null) {
                light = GetComponent<Light>();
            }
        }

        private void Update() 
        {
            if (light == null)
                return;

            // pop off an item if too big
            while (_smoothQueue.Count >= smoothing) 
            {
                _lastSum -= _smoothQueue.Dequeue();
            }

            // Generate random new item, calculate new average
            var newVal = Random.Range(minIntensity, maxIntensity);
            _smoothQueue.Enqueue(newVal);
            _lastSum += newVal;

            // Calculate new smoothed average
            light.intensity = _lastSum / _smoothQueue.Count;
        }

    }
}