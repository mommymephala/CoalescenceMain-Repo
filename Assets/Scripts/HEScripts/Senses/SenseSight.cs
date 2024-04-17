using HEScripts.Physics;
using UnityEngine;

namespace HEScripts.Senses
{
    public class SenseSight : Sense
    {
        [SerializeField] SenseTarget m_Target;
        
        private SightCheck m_Sight;
        private bool m_InSight;

        // --------------------------------------------------------------------

        public override void Init(SenseController controller)
        {
            base.Init(controller);
            
            m_Sight = controller.GetComponentInParent<SightCheck>();
            Debug.Assert(m_Sight, "Sense needs a SightCheck component on this object or any of the parents", gameObject);
        }

        // --------------------------------------------------------------------

        public override bool SuccessfullySensed()
        {
            return m_InSight;
        }

        // --------------------------------------------------------------------

        public override void Tick()
        {
            Transform target = m_Target.GetTransform();
            var wasInsight = m_InSight;

            m_InSight = target && m_Sight.IsInSight(target);

            if (wasInsight != m_InSight)
                OnChanged?.Invoke(this, target);
        }
    }
}