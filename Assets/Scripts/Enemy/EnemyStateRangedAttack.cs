using States;
using UnityEngine;

namespace Enemy
{
    public class EnemyStateRangedAttack : EnemyStateAttack
    {
        public override void StateEnter(IActorState fromState)
        {
            base.StateEnter(fromState);
            
            m_SelectedAttack = SelectAttack();
            
            if (m_SelectedAttack != null)
            {
                m_Duration = m_SelectedAttack.Duration;
                m_SelectedAttack.Play(Actor.MainAnimator);
            }
        }

        public override void StateUpdate()
        {
            base.StateUpdate();
            FaceTarget();
        }

        public override void StateExit(IActorState intoState)
        {
            m_AnimationState = null;
            m_LastAttackTime = Time.time;
            base.StateExit(intoState);
        }

        protected override void OnStateDurationEnd()
        {
            base.OnStateDurationEnd();
            if (m_EnemySenses.IsPlayerDetected)
                SetState(m_AlertedState);
        }
    }
}