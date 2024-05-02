using Audio;
using States;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class EnemyStateWanderAroundTarget : ActorStateWithDuration
    {
        [SerializeField] EnemyStateAlerted m_AlertedState;
        [SerializeField] float m_Radius = 10f;

        private NavMeshAgent m_Agent;
        private EnemySensesController m_EnemySenses;
        private Vector3 m_Destination;
        private Vector3 m_InitialPosition;
        
        private float _footstepTimer;
        private float _footstepInterval = 0.5f;

        // --------------------------------------------------------------------

        protected override void Awake()
        {
            base.Awake();
            m_Agent = GetComponentInParent<NavMeshAgent>();
            m_EnemySenses = GetComponentInParent<EnemySensesController>();
            m_InitialPosition = transform.position;
        }

        // --------------------------------------------------------------------

        private bool FindDestination()
        {
            // Check if the enemy is far from the initial position
            if (Vector3.Distance(transform.position, m_InitialPosition) > m_Radius)
            {
                m_Destination = m_InitialPosition;
                return true;
            }

            Vector3 randomDirection = Random.insideUnitSphere * m_Radius;
            randomDirection += m_EnemySenses.LastKnownPosition;
            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, m_Radius, 1))
            {
                m_Destination = hit.position;
                return true;
            }

            return false;
        }

        // --------------------------------------------------------------------

        public override void StateEnter(IActorState fromState)
        {
            base.StateEnter(fromState);

            m_Agent.isStopped = false;
            m_Agent.updateRotation = true;

            if (FindDestination())
                m_Agent.SetDestination(m_Destination);
            else
                SetState(m_AlertedState);
        }

        // --------------------------------------------------------------------

        public override void StateExit(IActorState intoState)
        {
            base.StateExit(intoState);
            m_Agent.isStopped = true;
        }

        // --------------------------------------------------------------------

        public override void StateUpdate()
        {
            base.StateUpdate();

            // Footstep sound logic
            _footstepTimer += Time.deltaTime;
            if (_footstepTimer >= _footstepInterval)
            {
                AudioManager.Instance.PlayEnemyFootstep(AudioManager.Instance.GetEnemyTypeFromActorType(Actor.type));
                _footstepTimer = 0;
            }

            var reachedTarget = m_Agent.remainingDistance <= m_Agent.stoppingDistance;
            if (reachedTarget)
            {
                if (FindDestination())
                    m_Agent.SetDestination(m_Destination);
                else
                    SetState(m_AlertedState);
            }
        }
    }
}