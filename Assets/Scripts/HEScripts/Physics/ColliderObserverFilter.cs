using UnityEngine;

namespace HEScripts.Physics
{
    public abstract class ColliderObserverFilter : MonoBehaviour
    {
        public virtual bool Passes(Collider other) { return true; }
    }
}