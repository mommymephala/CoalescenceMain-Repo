using System.Collections.Generic;
using Audio;
using States;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Utils;

namespace Enemy
{
    public class EnemyStateAlerted : ActorStateWithDuration
    {
        [SerializeField] private EnemyStateIdle m_IdleState;
        [SerializeField] private EnemyStateAttack[] m_AttackStates;
        [SerializeField] private float m_InitialDelay = 1f;
        [SerializeField] private float m_MinTimeBetweenAttacks = 1f;
        [SerializeField] private float m_FacingSpeedBetweenAttacks = 1f;
        [SerializeField] private bool m_ShowDebug;
        
        private UnityAction m_OnPlayerUnreachable;
        
        private NavMeshAgent m_Agent;
        
        private EnemySensesController m_EnemySenses;
        
        private ActorState m_CurrentAttack;
        private List<ActorState> m_AttackCandidates = new List<ActorState>();
        
        private float m_Delay;
        
        [SerializeField] private float footstepInterval = 0.5f;
        private float _footstepTimer;

        // --------------------------------------------------------------------

        protected override void Awake()
        {
            base.Awake();

            m_Agent = GetComponentInParent<NavMeshAgent>();
            m_EnemySenses = GetComponentInParent<EnemySensesController>();
            m_OnPlayerUnreachable = OnPlayerUnreachable;
        }

        // --------------------------------------------------------------------

        public override void StateEnter(IActorState fromState)
        {
            base.StateEnter(fromState);

            AudioManager.Instance.PlayEnemyAlert(AudioManager.Instance.GetEnemyTypeFromActorType(Actor.type),
                transform.position);
            AudioManager.Instance.PlayEnemyFootstep(AudioManager.Instance.GetEnemyTypeFromActorType(Actor.type),
                transform.position);

            m_Delay = 0;
            m_CurrentAttack = null;
            m_EnemySenses.OnPlayerUnreachable.AddListener(m_OnPlayerUnreachable);
        }

        // --------------------------------------------------------------------

        public override void StateExit(IActorState intoState)
        {
            base.StateExit(intoState);

            m_Agent.isStopped = true;
            m_EnemySenses.OnPlayerUnreachable.RemoveListener(m_OnPlayerUnreachable);
        }

        // --------------------------------------------------------------------

        private void OnPlayerUnreachable()
        {
            SetState(m_IdleState);
        }

        // --------------------------------------------------------------------

        public override void StateUpdate()
        {
            base.StateUpdate();

            if (m_Delay < m_InitialDelay)
            {
                m_Delay += Time.deltaTime;

                FaceTarget();
                return;
            }

            if (m_ShowDebug)
                DebugUtils.DrawBox(m_EnemySenses.LastKnownPosition, Quaternion.identity, Vector3.one * 0.25f,
                    Color.white, 1f);

            // Footstep sound logic
            _footstepTimer += Time.deltaTime;
            if (_footstepTimer >= footstepInterval)
            {
                AudioManager.Instance.PlayEnemyFootstep(AudioManager.Instance.GetEnemyTypeFromActorType(Actor.type),
                    transform.position);
                _footstepTimer = 0;
            }

            m_Agent.SetDestination(m_EnemySenses.LastKnownPosition);
            m_Agent.isStopped = false;

            if (m_CurrentAttack == null)
            {
                m_CurrentAttack = PickAttack();
            }

            if (m_CurrentAttack != null)
            {
                if (m_EnemySenses.IsPlayerDetected)
                {
                    if (m_TimeInState > m_MinTimeBetweenAttacks)
                    {
                        SetState(m_CurrentAttack);
                    }
                    else
                    {
                        m_Agent.updateRotation = false;

                        if (m_ShowDebug)
                            Debug.DrawLine(Actor.transform.position, m_EnemySenses.LastKnownPosition, Color.magenta, 5);

                        FaceTarget();
                    }
                }
                else
                {
                    SetState(m_IdleState);
                }
            }
            else
            {
                m_Agent.updateRotation = true;
            }
        }

        // --------------------------------------------------------------------

        private void FaceTarget()
        {
            Vector3 lookPos = m_EnemySenses.LastKnownPosition - Actor.transform.position;
            lookPos.y = 0;
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            Actor.transform.rotation = Quaternion.Slerp(Actor.transform.rotation, rotation,
                Time.deltaTime * m_FacingSpeedBetweenAttacks);
        }

        // --------------------------------------------------------------------

        private ActorState PickAttack()
        {
            m_AttackCandidates.Clear();

            foreach (EnemyStateAttack attack in m_AttackStates)
            {
                if (attack.CanEnter())
                {
                    m_AttackCandidates.Add(attack);
                    break;
                }
            }

            if (m_AttackCandidates.Count == 0)
            {
                return null;
            }

            return m_AttackCandidates[Random.Range(0, m_AttackCandidates.Count)];
        }
    }
}