using Combat;
using UnityEngine;

namespace Senses
{
    public class SenseVitality : Sense
    {
        [SerializeField] SenseTarget m_Target;

        private bool m_IsAlive;

        public override bool SuccessfullySensed()
        {
            return m_IsAlive;
        }

        public override void Tick()
        {
            Transform target = m_Target.GetTransform();
            var wasAlive = m_IsAlive;

            m_IsAlive = target && target.GetComponent<Health>().Value > 0;

            if (wasAlive != m_IsAlive)
                OnChanged?.Invoke(this, target);
        }
    }
}