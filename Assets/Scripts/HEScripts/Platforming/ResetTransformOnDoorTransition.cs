using System;
using HEScripts.Doors;
using HEScripts.Messages;
using HEScripts.SaveSystem;
using HorrorEngine;
using UnityEngine;

namespace HEScripts.Platforming
{
    public class ResetTransformOnDoorTransition : MonoBehaviour, ISavableObjectStateExtra
    {
        private Vector3 m_OriginalPosition;
        private Quaternion m_OriginalRotation;

        // --------------------------------------------------------------------

        private void Awake()
        {
            MessageBuffer<DoorTransitionMidWayMessage>.Subscribe(OnDoorTransition);
        }


        // --------------------------------------------------------------------

        private void Start()
        {
            m_OriginalPosition = transform.position;
            m_OriginalRotation = transform.rotation;
        }

        // --------------------------------------------------------------------

        private void OnDestroy()
        {
            MessageBuffer<DoorTransitionMidWayMessage>.Unsubscribe(OnDoorTransition);
        }

        // --------------------------------------------------------------------

        private void OnDoorTransition(DoorTransitionMidWayMessage msg)
        {
            if (enabled)
            {
                transform.position = m_OriginalPosition;
                transform.rotation = m_OriginalRotation;
            }
        }


        // --------------------------------------------------------------------
        // ISavable implementation
        // --------------------------------------------------------------------

        public string GetSavableData()
        {
            return enabled.ToString();
        }

        public void SetFromSavedData(string savedData)
        {
            enabled = Convert.ToBoolean(savedData);
        }
    }
}