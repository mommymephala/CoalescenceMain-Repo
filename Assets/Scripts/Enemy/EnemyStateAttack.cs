using System.Collections.Generic;
using Combat;
using States;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class EnemyStateAttack : ActorStateWithDuration
    {
        [SerializeField] protected List<AttackMontage> m_AttackOptions;
        [SerializeField] protected EnemyStateAlerted m_AlertedState;
        [SerializeField] protected float m_FacingSpeed = 1f;
        [SerializeField] protected bool m_RotateTowardsTarget = true;
        
        public float AttackDistance = 1f;
        public float Cooldown = 3;

        protected AttackMontage m_SelectedAttack;
        protected NavMeshAgent m_NavMeshAgent;
        protected EnemySensesController m_EnemySenses;
        protected float m_LastAttackTime;
        protected NavMeshPath m_NavPath;

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
            m_SelectedAttack = SelectAttack();
            
            if (m_SelectedAttack != null)
            {
                m_Duration = m_SelectedAttack.Duration;
                m_SelectedAttack.Play(Actor.MainAnimator);
            }
        }

        // --------------------------------------------------------------------

        public override void StateUpdate()
        {
            base.StateUpdate();
            FaceTarget();
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

            if (m_EnemySenses.IsPlayerDetected && CanEnter())
            {
                SetState(this);
            }
            else
            {
                SetState(m_AlertedState);
            }
        }

        // --------------------------------------------------------------------
        
        protected void FaceTarget()
        {
            if (m_RotateTowardsTarget)
            {
                Vector3 lookPos = m_EnemySenses.LastKnownPosition - transform.position;
                lookPos.y = 0;
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                Actor.transform.rotation = Quaternion.Slerp(Actor.transform.rotation, rotation, Time.deltaTime * m_FacingSpeed);
            }
        }
        
        // --------------------------------------------------------------------

        protected AttackMontage SelectAttack()
        {
            // Here we can implement any logic to select an attack based on current game state
            return m_AttackOptions[Random.Range(0, m_AttackOptions.Count)];
        }

        // --------------------------------------------------------------------

        public virtual bool CanEnter()
        {
            m_NavPath.ClearCorners();
            m_NavMeshAgent.CalculatePath(m_EnemySenses.LastKnownPosition, m_NavPath);
            
            if (m_NavPath.status != NavMeshPathStatus.PathInvalid && m_NavPath.corners.Length > 1)
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

            return Time.time - m_LastAttackTime > Cooldown;
        }
    }
}