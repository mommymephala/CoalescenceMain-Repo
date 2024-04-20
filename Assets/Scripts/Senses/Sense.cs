using UnityEngine;

namespace Senses
{
    public abstract class Sense : MonoBehaviour
    {
        public SenseChangedEvent OnChanged = new SenseChangedEvent();

        public float TickFrequency;
        
        protected SenseController m_Controller;

        public virtual void Init(SenseController controller)
        {
            m_Controller = GetComponentInParent<SenseController>();
        }

        public abstract void Tick();

        public abstract bool SuccessfullySensed();
    }
}