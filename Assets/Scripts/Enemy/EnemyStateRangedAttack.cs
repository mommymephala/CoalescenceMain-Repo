using States;
using UnityEngine;

namespace Enemy
{
    public class EnemyStateRangedAttack : EnemyStateAttack
    {
        public override void StateEnter(IActorState fromState)
        {
            base.StateEnter(fromState);
        }

        public override void StateUpdate()
        {
            base.StateUpdate();
        }

        public override void StateExit(IActorState intoState)
        {
            base.StateExit(intoState);
        }

        protected override void OnStateDurationEnd()
        {
            base.OnStateDurationEnd();
        }
    }
}