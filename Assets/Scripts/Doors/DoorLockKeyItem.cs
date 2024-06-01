using FMODUnity;
using Items;
using Systems;
using UI_Codebase;
using UnityEngine;

namespace Doors
{
    public class DoorLockKeyItem : DoorLock
    {
        [SerializeField] private DialogData m_OnUnlockedDialog;
        
        [Space]
        [SerializeField] private ItemData m_Key;
        [SerializeField] private bool m_ConsumesItem = true;
        [SerializeField] private bool m_UseObjectAutomatically = true;

        protected override void Awake()
        {
            base.Awake();

            Debug.Assert(m_Key, "DoorLockKeyItem requires an item to work", gameObject);
        }

        public override void OnTryToUnlock(out bool openImmediately)
        {
            if (!TryUnlock())
            {
                if (m_OnLockedDialog.IsValid())
                    UIManager.Get<UIDialog>().Show(m_OnLockedDialog);
            }

            openImmediately = false;
        }

        public bool TryUnlock()
        {
            Debug.Assert(m_Key, "DoorLockKeyItem requires an item to work", gameObject);
            if (!m_Key)
                return false;

            if (m_UseObjectAutomatically)
            {
                if (GameManager.Instance.Inventory.Contains(m_Key))
                {
                    if (m_ConsumesItem)
                    {
                        GameManager.Instance.Inventory.Remove(m_Key);
                        IsLocked = false;
                        OnUnlock?.Invoke();
                    }
                }

                if (m_OnUnlockedDialog.IsValid())
                    UIManager.Get<UIDialog>().Show(m_OnUnlockedDialog);


                return true;
            }
            
            return false;
        }
    }
}