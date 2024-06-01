using Player;
using UnityEngine;
using UnityEngine.Events;

namespace Doors
{
    public abstract class DoorBase : MonoBehaviour
    {
        public UnityEvent OnLocked;
        public UnityEvent OnOpened;

        public abstract void Use(IInteractor interactor);

        public abstract bool IsLocked();
    }

    public class SceneDoor : DoorBase
    {
        private DoorLock m_Lock;
        private Transform m_Interactor;

        // --------------------------------------------------------------------

        private void Awake()
        {
            m_Lock = GetComponent<DoorLock>();
        }

        // --------------------------------------------------------------------

        public override bool IsLocked()
        {
            return (m_Lock && m_Lock.IsLocked);
        }

        // --------------------------------------------------------------------

        public override void Use(IInteractor interactor)
        {
            if (m_Lock && m_Lock.IsLocked)
            {
                m_Lock.OnTryToUnlock(out var open);
                if (!open)
                {
                    if (IsLocked())
                        OnLocked?.Invoke();
                    return;
                }
            }
            
            OnOpened?.Invoke();
        }
    }
}