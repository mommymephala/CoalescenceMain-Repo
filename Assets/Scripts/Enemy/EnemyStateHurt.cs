using System;
using Audio;
using States;
using UnityEngine;

namespace Enemy
{
    public class EnemyStateHurt : ActorStateWithDuration
    {
        public override void StateEnter(IActorState fromState)
        {
            base.StateEnter(fromState);

            switch (Actor.type)
            {
                case Actor.ActorType.TarSpawn:
                    AudioManager.Instance.PlayEnemyHurt(AudioManager.EnemyType.TarSpawn, transform.position);
                    break;
                case Actor.ActorType.ExperimentalMan:
                    AudioManager.Instance.PlayEnemyHurt(AudioManager.EnemyType.ExperimentalMan, transform.position);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}