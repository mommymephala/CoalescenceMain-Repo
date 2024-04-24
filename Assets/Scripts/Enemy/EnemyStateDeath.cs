using System.Collections.Generic;
using Audio;
using Combat;
using FMODUnity;
using States;
using UnityEngine;

namespace Enemy
{
    [System.Serializable]
    public class DamageableAnimEntry
    {
        public Damageable Damageable;
        public AnimatorStateHandle AnimationState;
    }

    public class EnemyStateDeath : ActorState
    {
        [SerializeField] private List<DamageableAnimEntry> m_DamageableSpecificAnimation;

        private Health m_Health;

        protected override void Awake()
        {
            base.Awake();

            m_Health = GetComponentInParent<Health>();
        }

        public override void StateEnter(IActorState fromState)
        {
            Damageable lastDamageable = m_Health.LastDamageableHit;
            if (lastDamageable) 
            {
                foreach (var entry in m_DamageableSpecificAnimation)
                {
                    if (entry.Damageable == lastDamageable)
                    {
                        AudioManager.Instance.PlayEnemyDeath(gameObject,AudioManager.EnemyType.BaseEnemy);
                       Debug.Log("ölüyoz amk");
                        m_AnimationState = entry.AnimationState;
                    }
                }
            }

            base.StateEnter(fromState);
        }
    }
}