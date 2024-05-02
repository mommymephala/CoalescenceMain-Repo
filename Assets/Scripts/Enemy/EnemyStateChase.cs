using System;
using System.Collections;
using Audio;
using States;
using Systems;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Enemy
{
    public class EnemyStateChase : ActorStateWithDuration
    {
        [SerializeField] EnemyStateIdle m_IdleState;
        
        private NavMeshAgent _agent;
        private EnemySensesController m_EnemySenses;
        
        private Coroutine _idleSoundCoroutine;
        
        private float _footstepInterval = 0.5f;
        private float _footstepTimer;
        
        private UnityAction m_OnPlayerUnreachable;

        protected override void Awake()
        {
            base.Awake();
            
            _agent = GetComponentInParent<NavMeshAgent>();
            m_EnemySenses = GetComponentInParent<EnemySensesController>();
            m_OnPlayerUnreachable = OnPlayerUnreachable;
        }

        public override void StateEnter(IActorState fromState)
        {
            base.StateEnter(fromState);
            
            m_TimeInState = 0f;
            _footstepTimer = 0f;
            
            _agent.isStopped = false;
            
            m_EnemySenses.OnPlayerUnreachable.AddListener(m_OnPlayerUnreachable);
            _idleSoundCoroutine = StartCoroutine(PlayIdleSoundLoop());
        }
        
        public override void StateExit(IActorState intoState)
        {
            base.StateExit(intoState);
            
            StopCoroutine(_idleSoundCoroutine);
            
            _agent.isStopped = true;
            m_EnemySenses.OnPlayerUnreachable.RemoveListener(m_OnPlayerUnreachable);
        }
        
        private void OnPlayerUnreachable()
        {
            SetState(m_IdleState);
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

        public override void StateUpdate()
        {
            base.StateUpdate();

            _agent.SetDestination(GameManager.Instance.Player.transform.position);

            _footstepTimer += Time.deltaTime;
            if (_footstepTimer >= _footstepInterval)
            {
                PlayFootstepSound();
                _footstepTimer = 0;
            }
        }

        private void PlayFootstepSound()
        {
            AudioManager.Instance.PlayEnemyFootstep(AudioManager.Instance.GetEnemyTypeFromActorType(Actor.type));
        }
        
        private IEnumerator PlayIdleSoundLoop()
        {
            while (true)
            {
                AudioManager.Instance.PlayEnemyIdle(AudioManager.Instance.GetEnemyTypeFromActorType(Actor.type));
                
                yield return new WaitForSeconds(3);
            }
            
            // ReSharper disable once IteratorNeverReturns
        }
    }
}