using System;
using FMODUnity;
using SaveSystem;
using UI_Codebase;
using UnityEngine;
using UnityEngine.Events;

namespace Doors
{
    public class DoorLock : MonoBehaviour, ISavableObjectStateExtra
    {
        [SerializeField] private bool m_Locked = true;
     
        [SerializeField] protected DialogData m_OnLockedDialog;

        public bool IsLocked 
        {
            get => m_Locked;

            protected set
            {
                m_Locked = value;
                SaveState(); // Save the object state manually so it's reflected on the map
            } 
        }

        public UnityEvent OnUnlock;

        protected DoorBase Door;

        private SavableObjectState m_Savable;

        public void SetLocked(bool locked)
        {
            IsLocked = locked;
        }

        protected virtual void Awake()
        {
            Door = GetComponent<DoorBase>();
            m_Savable = GetComponent<SavableObjectState>();
        }

        public virtual void OnTryToUnlock(out bool openImmediately)
        {
            UIManager.Get<UIDialog>().Show(m_OnLockedDialog);

            openImmediately = false;
        }

        protected void SaveState()
        {
            m_Savable.SaveState();
        }

        //-----------------------------------------------------
        // ISavable implementation
        //-----------------------------------------------------

        public string GetSavableData()
        {
            return IsLocked.ToString();
        }

        public void SetFromSavedData(string savedData)
        {
            IsLocked = Convert.ToBoolean(savedData);
        }
    }
}