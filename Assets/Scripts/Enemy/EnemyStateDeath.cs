using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using Combat;
using States;
using UnityEngine;

namespace Enemy
{
    [Serializable]
    public class DamageableAnimEntry
    {
        public Damageable Damageable;
        public AnimatorStateHandle AnimationState;
    }

    public class EnemyStateDeath : ActorState
    {
        [SerializeField] private List<DamageableAnimEntry> m_DamageableSpecificAnimation;
        [SerializeField] private List<Collider> collidersToDisable;

        private Health m_Health;

        protected override void Awake()
        {
            base.Awake();

            m_Health = GetComponentInParent<Health>();
        }

        public override void StateEnter(IActorState fromState)
        {
            Damageable lastDamageable = m_Health.LastDamageableHit;

            if (lastDamageable && Actor) 
            {
                foreach (DamageableAnimEntry entry in m_DamageableSpecificAnimation)
                {
                    if (entry.Damageable != lastDamageable) continue;
                    
                    switch (Actor.type)
                    {
                        case Actor.ActorType.TarSpawn:
                            AudioManager.Instance.PlayEnemyDeath(AudioManager.EnemyType.TarSpawn, transform.position);
                            break;
                        case Actor.ActorType.ExperimentalMan:
                            AudioManager.Instance.PlayEnemyDeath(AudioManager.EnemyType.ExperimentalMan, transform.position);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    m_AnimationState = entry.AnimationState;
                    break;
                }
            }
            
            DisableColliders();
            base.StateEnter(fromState);
        }
        
        private void DisableColliders()
        {
            foreach (Collider collider in collidersToDisable)
            {
                if (collider != null)
                    collider.enabled = false;
            }
        }
    }
}