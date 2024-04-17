using HorrorEngine;
using UnityEngine;

namespace HEScripts.Senses
{
    public class SenseProximity : Sense
    {
        [SerializeField] SenseTarget m_Target;
        [SerializeField] float m_DetectionRadius = 10;
        [SerializeField] float m_UndetectionRadius = 15;

        private bool m_InProximity;

        public override bool SuccessfullySensed()
        {
            return m_InProximity;
        }

        public override void Tick()
        {
            Transform target = m_Target.GetTransform();
            var wasInProximity = m_InProximity;

            if (target)
            {
                if (!m_InProximity)
                    m_InProximity = Vector3.Distance(target.position, transform.position) < m_DetectionRadius;
                else
                    m_InProximity = Vector3.Distance(target.position, transform.position) < m_UndetectionRadius;
            }
            else
            {
                m_InProximity = false;
            }

            if (wasInProximity != m_InProximity)
                OnChanged?.Invoke(this, target);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, m_DetectionRadius);
            Gizmos.DrawWireSphere(transform.position, m_UndetectionRadius);
        }
    }
}