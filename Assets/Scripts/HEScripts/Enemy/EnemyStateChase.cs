using HEScripts.States;
using HEScripts.Systems;
using UnityEngine;
using UnityEngine.AI;

namespace HEScripts.Enemy
{
    public class EnemyStateChase : ActorStateWithDuration
    {
        private NavMeshAgent _agent;
        
        protected override void Awake()
        {
            base.Awake();
            _agent = GetComponentInParent<NavMeshAgent>();
        }

        public override void StateEnter(IActorState fromState)
        {
            base.StateEnter(fromState);
            _agent.isStopped = false;
            m_TimeInState = 0f;
        }

        public override void StateUpdate()
        {
            base.StateUpdate();
            _agent.SetDestination(GameManager.Instance.Player.transform.position);
        }

        protected override void OnStateDurationEnd()
        {
            base.OnStateDurationEnd();
            _agent.isStopped = true;
            if (m_GoToStateAfterDuration)
                SetState(m_GoToStateAfterDuration);
        }

        public override void StateExit(IActorState intoState)
        {
            base.StateExit(intoState);
            _agent.isStopped = true;
        }
    }
}