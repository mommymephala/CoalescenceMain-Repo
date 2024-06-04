using UnityEngine;

namespace Level_Events.Lights
{
    public class LightControl : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                // RenderSettings.ambientIntensity = 0f;
                RenderSettings.reflectionIntensity = 0f;
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                // RenderSettings.ambientIntensity = 0f;
                RenderSettings.reflectionIntensity = 0.5f;
            }
        }
    }
}