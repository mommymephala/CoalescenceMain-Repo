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
        private Coroutine idleSoundCoroutine;
        
        protected override void Awake()
        {
            base.Awake();
            _agent = GetComponentInParent<NavMeshAgent>();
        }

        public override void StateEnter(IActorState fromState)
        {
            base.StateEnter(fromState);
            _agent.isStopped = false;
            
            if (idleSoundCoroutine == null)
            {
                idleSoundCoroutine = StartCoroutine(AudioManager.Instance.PlayIdleSoundLoop());
            }
            
            m_TimeInState = 0f;
        }

        public override void StateUpdate()
        {
            base.StateUpdate();
            if (idleSoundCoroutine != null)
            {
                StopCoroutine(idleSoundCoroutine);
                idleSoundCoroutine = null;
            }
            _agent.SetDestination(GameManager.Instance.Player.transform.position);
        }

        protected override void OnStateDurationEnd()
        {
            base.OnStateDurationEnd();
            _agent.isStopped = true;
            if (m_GoToStateAfterDuration)
                SetState(m_GoToStateAfterDuration);
        }
        
        // public virtual void PlayFootstepSound()
        // {
        //     AudioManager.Instance.PlayEnemyFootStep(gameObject, AudioManager.EnemyType.BaseEnemy);
        // }
        
        public override void StateExit(IActorState intoState)
        {
            base.StateExit(intoState);
            _agent.isStopped = true;
        }
    }
}