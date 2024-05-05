using System.Collections.Generic;
using Audio;
using Combat;
using States;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class EnemyStateAttack : ActorStateWithDuration
    {
        [SerializeField] private List<AttackMontage> attackOptions;
        [SerializeField] EnemyStateAlerted m_AlertedState;
        [SerializeField] float m_FacingSpeed = 1f;
        [SerializeField] bool m_RotateTowardsTarget = true;
        public float AttackDistance = 1f;
        public float Cooldown = 3;

        private AttackMontage selectedAttack;
        private NavMeshAgent m_NavMeshAgent;
        private EnemySensesController m_EnemySenses;
        private float m_LastAttackTime;
        private NavMeshPath m_NavPath;
        
        // --------------------------------------------------------------------

        protected override void Awake()
        {
            base.Awake();

            m_NavPath = new NavMeshPath();
            m_NavMeshAgent = GetComponentInParent<NavMeshAgent>();
            m_EnemySenses = GetComponentInParent<EnemySensesController>();
        }

        // --------------------------------------------------------------------

        public override void StateEnter(IActorState fromState)
        {
            base.StateEnter(fromState);
            selectedAttack = SelectAttack();
            
            if (selectedAttack != null)
            {
                m_Duration = selectedAttack.Duration;
                selectedAttack.Play(Actor.MainAnimator);
            }
        }

        // --------------------------------------------------------------------

        public override void StateUpdate()
        {
            base.StateUpdate();

            // Face target
            if (m_RotateTowardsTarget)
            {
                Vector3 lookPos = m_EnemySenses.LastKnownPosition - transform.position;
                lookPos.y = 0;
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                Actor.transform.rotation = Quaternion.Slerp(Actor.transform.rotation, rotation, Time.deltaTime * m_FacingSpeed);
            }
        }

        // --------------------------------------------------------------------

        public override void StateExit(IActorState intoState)
        {
            m_AnimationState = null;
            m_LastAttackTime = Time.time;

            base.StateExit(intoState);
        }

        // --------------------------------------------------------------------

        protected override void OnStateDurationEnd()
        {
            base.OnStateDurationEnd();

            if (m_EnemySenses.IsPlayerDetected)
                SetState(m_AlertedState);
        }
        
        // --------------------------------------------------------------------

        private AttackMontage SelectAttack()
        {
            // Here you can implement any logic to select an attack based on current game state
            // For example, random, based on distance, enemy health, etc.
            return attackOptions[Random.Range(0, attackOptions.Count)];
        }

        // --------------------------------------------------------------------

        public virtual bool CanEnter()
        {
            m_NavPath.ClearCorners();
            m_NavMeshAgent.CalculatePath(m_EnemySenses.LastKnownPosition, m_NavPath);
            
            if ((m_NavPath.status != NavMeshPathStatus.PathInvalid) && (m_NavPath.corners.Length > 1))
            {
                float distToTarget = 0;
                for (var i = 1; i < m_NavPath.corners.Length; ++i)
                {
                    distToTarget += Vector3.Distance(m_NavPath.corners[i - 1], m_NavPath.corners[i]);
                    if (distToTarget > AttackDistance)
                        return false;
                }
            }
            
            else
            {
                return false;
            }

            return (Time.time - m_LastAttackTime) > Cooldown;
        }
    }
}
