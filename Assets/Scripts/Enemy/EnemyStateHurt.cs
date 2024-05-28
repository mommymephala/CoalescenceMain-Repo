using Audio;
using States;

namespace Enemy
{
    public class EnemyStateHurt : ActorStateWithDuration
    {
        public override void StateEnter(IActorState fromState)
        {
            base.StateEnter(fromState);

            AudioManager.Instance.PlayEnemyHurt(AudioManager.Instance.GetEnemyTypeFromActorType(Actor.type),
                transform.position);
        }
    }
}