using UnityEngine;

namespace HEScripts.Interaction
{
    [CreateAssetMenu(fileName = "InteractionData", menuName = "Horror Engine/Interaction")]
    public class InteractionData : ScriptableObject
    {
        public string Prompt;
        public Sprite Icon;
        public float InteractionDuration;
        public float InteractionDelay;

        private void OnValidate()
        {
            if (InteractionDuration < InteractionDelay)
                InteractionDuration = InteractionDelay;
        }
    }
}