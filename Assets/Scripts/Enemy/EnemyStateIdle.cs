using System;
using System.Collections;
using Audio;
using States;
using UnityEngine;

namespace Enemy
{
    public class EnemyStateIdle : ActorState
    {
        [SerializeField] private EnemyStateAlerted m_AlertedState;
        [SerializeField] private EnemyStateWanderAroundTarget m_WanderState;
        [SerializeField] private float TimeBetweenWander = 3;
        private float m_StateTime;
        private EnemySensesController m_EnemySenses;
        
        private Coroutine _idleSoundCoroutine;

        protected override void Awake()
        {
            base.Awake();
            m_EnemySenses = GetComponentInParent<EnemySensesController>();
        }

        public override void StateEnter(IActorState fromState)
        {
            base.StateEnter(fromState);
            m_StateTime = 0;
            _idleSoundCoroutine = StartCoroutine(PlayIdleSoundLoop());
        }

        private IEnumerator PlayIdleSoundLoop()
        {
            while (true)
            {
                AudioManager.Instance.PlayEnemyIdle(gameObject, Actor.type);
                yield return new WaitForSeconds(3);
            }
            // ReSharper disable once IteratorNeverReturns
        }

        public override void StateUpdate()
        {
            base.StateUpdate();

            if (m_EnemySenses.IsPlayerDetected && m_EnemySenses.IsPlayerInReach)
            {
                StopCoroutine(_idleSoundCoroutine);
                SetState(m_AlertedState);
                return;
            }

            m_StateTime += Time.deltaTime;
            if (m_WanderState && m_StateTime > TimeBetweenWander)
            {
                StopCoroutine(_idleSoundCoroutine);
                SetState(m_WanderState);
            }
        }
    }
}