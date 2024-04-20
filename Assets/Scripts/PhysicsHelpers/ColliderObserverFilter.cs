using UnityEngine;

namespace PhysicsHelpers
{
    public abstract class ColliderObserverFilter : MonoBehaviour
    {
        public virtual bool Passes(Collider other) { return true; }
    }
}