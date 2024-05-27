using System.Collections.Generic;
using Audio;
using States;
using UnityEngine;

namespace Enemy
{
    public class EnemyStateDeath : ActorState
    {
        [SerializeField] private List<Collider> collidersToDisable;

        public override void StateEnter(IActorState fromState)
        {
            base.StateEnter(fromState);
            
            DisableColliders();
        }
        
        private void DisableColliders()
        {
            foreach (Collider collider in collidersToDisable)
            {
                if (collider != null)
                {
                    collider.enabled = false;
                    
                    AudioManager.Instance.PlayEnemyDeath(AudioManager.Instance.GetEnemyTypeFromActorType(Actor.type),
                        transform.position);
                }
            }
        }
    }
}