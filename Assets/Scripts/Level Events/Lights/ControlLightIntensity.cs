using UnityEngine;

namespace Level_Events.Lights
{
    public class ControlLightIntensity : MonoBehaviour
    {
        private void Update()
        {
            RenderSettings.ambientIntensity = 0.35f;
        }
    }
}
