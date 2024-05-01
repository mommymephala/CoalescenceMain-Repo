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
        private NavMeshAgent _agent;
        
        private Coroutine _idleSoundCoroutine;
        
        protected override void Awake()
        {
            base.Awake();
            _agent = GetComponentInParent<NavMeshAgent>();
        }

        public override void StateEnter(IActorState fromState)
        {
            base.StateEnter(fromState);
            _agent.isStopped = false;
            
            _idleSoundCoroutine = StartCoroutine(PlayIdleSoundLoop());
            
            m_TimeInState = 0f;
        }
        
        private IEnumerator PlayIdleSoundLoop()
        {
            while (true)
            {
                switch (Actor.type)
                {
                    case Actor.ActorType.TarSpawn:
                        AudioManager.Instance.PlayEnemyIdle(AudioManager.EnemyType.TarSpawn);
                        break;
                    case Actor.ActorType.ExperimentalMan:
                        AudioManager.Instance.PlayEnemyIdle(AudioManager.EnemyType.ExperimentalMan);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                yield return new WaitForSeconds(3);
            }
            
            // ReSharper disable once IteratorNeverReturns
        }

        public override void StateUpdate()
        {
            base.StateUpdate();
            
            StopCoroutine(_idleSoundCoroutine);
                
            _agent.SetDestination(GameManager.Instance.Player.transform.position);
        }

        protected override void OnStateDurationEnd()
        {
            base.OnStateDurationEnd();
            _agent.isStopped = true;
            
            if (!m_GoToStateAfterDuration) return;
            
            StopCoroutine(_idleSoundCoroutine);
            SetState(m_GoToStateAfterDuration);
        }
        
        // public virtual void PlayFootstepSound()
        // {
        //     AudioManager.Instance.PlayEnemyFootStep(gameObject, AudioManager.EnemyType.BaseEnemy);
        // }
        
        public override void StateExit(IActorState intoState)
        {
            base.StateExit(intoState);
            StopCoroutine(_idleSoundCoroutine);

            _agent.isStopped = true;
        }
    }
}