using System.Collections.Generic;
using UnityEngine;

namespace States
{
    public class Actor : MonoBehaviour
    {
        public enum ActorType
        {
            TarSpawn,
            ExperimentalMan,
            Player
        }

        public ActorType type;
        
        public ActorStateController StateController { get; private set; }
        public Animator MainAnimator;

        private HashSet<Object> m_DisableContext = new HashSet<Object>();

        private IDeactivateWithActor[] m_DeactivableCompontents;

        public bool IsDisabled => m_DisableContext.Count > 0;

        // --------------------------------------------------------------------

        protected void Awake()
        {
            StateController = GetComponent<ActorStateController>();

            m_DeactivableCompontents = GetComponentsInChildren<IDeactivateWithActor>();
        }

        // --------------------------------------------------------------------

        public void Disable(Object context)
        {
            var wasDisabled = IsDisabled;
            if (!m_DisableContext.Contains(context))
                m_DisableContext.Add(context);

            if (!wasDisabled && IsDisabled)
            {
                foreach (IDeactivateWithActor deactivable in m_DeactivableCompontents)
                {
                    ((MonoBehaviour)deactivable).enabled = false;
                }
            }
        }

        // --------------------------------------------------------------------

        public void Enable(Object context)
        {
            var wasDisabled = IsDisabled;
            m_DisableContext.Remove(context);

            if (wasDisabled && !IsDisabled)
            {
                foreach (IDeactivateWithActor deactivable in m_DeactivableCompontents)
                {
                    ((MonoBehaviour)deactivable).enabled = true;
                }
            }
        }
    }
}