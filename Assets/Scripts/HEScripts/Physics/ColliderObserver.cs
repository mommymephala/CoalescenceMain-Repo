using System;
using HEScripts.Misc;
using UnityEngine;
using UnityEngine.Events;

namespace HEScripts.Physics
{
    [Serializable]
    public class OnTriggerAction : UnityEvent<Collider> { }

    public class ColliderObserver : MonoBehaviour
    {
        public OnTriggerAction TriggerEnter;
        public OnTriggerAction TriggerExit;

        private Action<OnDisableNotifier> m_OnColliderDisabled;
        private ColliderObserverFilter[] m_Filters;

        private void Awake()
        {
            m_Filters = GetComponents<ColliderObserverFilter>();
            m_OnColliderDisabled = OnColliderDisabled;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!PassesFilter(other))
                return;

            other.GetComponentInParent<OnDisableNotifier>()?.AddCallback(m_OnColliderDisabled);
            TriggerEnter?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!PassesFilter(other))
                return;

            other.GetComponentInParent<OnDisableNotifier>()?.RemoveCallback(m_OnColliderDisabled);
            TriggerExit?.Invoke(other);
        }

        private bool PassesFilter(Collider other)
        {
            if (m_Filters.Length == 0)
                return true;

            foreach (var filter in m_Filters)
            {
                if (filter.Passes(other))
                {
                    return true;
                }
            }
            return false;
        }

        private void OnColliderDisabled(OnDisableNotifier notifier)
        {
            notifier.RemoveCallback(m_OnColliderDisabled);
            TriggerExit?.Invoke(notifier.GetComponent<Collider>());
        }
    }
}