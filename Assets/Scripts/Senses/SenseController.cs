using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Utils;

namespace Senses
{
    public class SenseChangedEvent : UnityEvent<Sense, Transform> { }

    public class SenseController : MonoBehaviour
    {
        private SenseChangedEvent OnSenseChanged = new SenseChangedEvent();

        [SerializeField] List<Sense> m_Senses;

        private UnityAction<Sense, Transform> m_OnSenseChangedCallback;

        private void Awake()
        {
            m_OnSenseChangedCallback = OnSenseChangedCallback;
        }

        private void OnEnable()
        {
            foreach(Sense sense in m_Senses)
            {
                sense.Init(this);
                sense.OnChanged.AddListener(m_OnSenseChangedCallback);
                StartCoroutine(ScheduleSenseUpdate(sense));
            }
        }

        private void OnDisable()
        {
            StopAllCoroutines();

            foreach (Sense sense in m_Senses)
            {
                sense.OnChanged.RemoveListener(m_OnSenseChangedCallback);
            }
        }

        protected virtual void OnSenseChangedCallback(Sense sense, Transform detected)
        {
            OnSenseChanged?.Invoke(sense, detected);
        }

        private IEnumerator ScheduleSenseUpdate(Sense sense)
        {
            while (true)
            {
                sense.Tick();
                yield return Yielders.Time(sense.TickFrequency);
            }
        }
    }
}