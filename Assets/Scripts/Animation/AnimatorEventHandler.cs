using UnityEngine;
using UnityEngine.Events;

namespace Animation
{
    public class AnimatorEventHandler : MonoBehaviour
    {
        [HideInInspector]
        public UnityEvent<AnimationEvent> OnEvent;

        public void TriggerEvent(AnimationEvent e)
        {
            OnEvent?.Invoke(e);
        }
    }
}