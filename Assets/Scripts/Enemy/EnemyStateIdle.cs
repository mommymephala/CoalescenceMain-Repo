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

        protected override void Awake()
        {
            base.Awake();

            m_EnemySenses = GetComponentInParent<EnemySensesController>();
        }

        public override void StateEnter(IActorState fromState)
        {
            base.StateEnter(fromState);
            m_StateTime = 0;
        }

        public override void StateUpdate()
        {
            base.StateUpdate();
            AudioManager.Instance.PlayEnemyIdle(gameObject,AudioManager.EnemyType.BaseEnemy);

            if (m_EnemySenses.IsPlayerDetected && m_EnemySenses.IsPlayerInReach)
            {
                SetState(m_AlertedState);
                return; 
            }

            m_StateTime += Time.deltaTime;
            if (m_WanderState && m_StateTime > TimeBetweenWander)
            {
                SetState(m_WanderState);
            }
        }
    }
}