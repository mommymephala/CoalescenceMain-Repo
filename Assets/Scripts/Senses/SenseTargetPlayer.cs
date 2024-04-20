using Systems;
using UnityEngine;

namespace Senses
{
    public class SenseTargetPlayer : SenseTarget
    {
        public override Transform GetTransform()
        {
            if (GameManager.Exists && GameManager.Instance.Player)
                return GameManager.Instance.Player.transform;
            else
                return null;
        }
    }
}