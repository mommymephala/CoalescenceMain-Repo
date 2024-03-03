using UnityEngine;
using UnityEngine.Events;

namespace HEScripts.Pickups
{
    public abstract class Pickup : MonoBehaviour
    {
        public UnityEvent OnPickup;
        
        public virtual void Take() 
        { 
            OnPickup?.Invoke();
        }
    }
}