using System;
using System.Collections;
using Audio;
using States;
using Systems;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class EnemyStateChase : ActorStateWithDuration
    {
        [SerializeField] private EnemyStateIdle m_IdleState;
        [SerializeField] private EnemyStateAlerted m_AlertState;

        private NavMeshAgent _agent;
        private EnemySensesController m_EnemySenses;

        private Coroutine _idleSoundCoroutine;

        private float _footstepInterval = 0.5f;
        private float _footstepTimer;

        protected override void Awake()
        {
            base.Awake();

            _agent = GetComponentInParent<NavMeshAgent>();
            m_EnemySenses = GetComponentInParent<EnemySensesController>();
        }

        public override void StateEnter(IActorState fromState)
        {
            base.StateEnter(fromState);

            m_TimeInState = 0f;
            _footstepTimer = 0f;

            _agent.isStopped = false;

            _idleSoundCoroutine = StartCoroutine(PlayIdleSoundLoop());
        }

        public override void StateUpdate()
        {
            base.StateUpdate();

            if (m_EnemySenses.IsPlayerInReach)
            {
                _agent.SetDestination(GameManager.Instance.Player.transform.position);

                _footstepTimer += Time.deltaTime;
                if (_footstepTimer >= _footstepInterval)
                {
                    PlayFootstepSound();
                    _footstepTimer = 0;
                }

                // Check for player detection
                if (m_EnemySenses.IsPlayerDetected)
                {
                    SetState(m_AlertState);
                }
            }

            else
            {
                SetState(m_IdleState);
            }
        }

        public override void StateExit(IActorState intoState)
        {
            base.StateExit(intoState);

            StopCoroutine(_idleSoundCoroutine);

            _agent.isStopped = true;
        }

        protected override void OnStateDurationEnd()
        {
            base.OnStateDurationEnd();

            _agent.isStopped = true;

            if (m_GoToStateAfterDuration != null)
            {
                StopCoroutine(_idleSoundCoroutine);

                SetState(m_GoToStateAfterDuration);
            }
        }

        private void PlayFootstepSound()
        {
            AudioManager.Instance.PlayEnemyFootstep(AudioManager.Instance.GetEnemyTypeFromActorType(Actor.type), transform.position);
        }

        private IEnumerator PlayIdleSoundLoop()
        {
            while (true)
            {
                AudioManager.Instance.PlayEnemyFootstep(AudioManager.Instance.GetEnemyTypeFromActorType(Actor.type), transform.position);

                yield return new WaitForSeconds(3);
            }

            // ReSharper disable once IteratorNeverReturns
        }
    }
}