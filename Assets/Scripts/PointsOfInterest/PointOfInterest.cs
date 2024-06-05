using UI_Codebase;
using UnityEngine;
using UnityEngine.Events;

namespace PointsOfInterest
{
    public class PointOfInterest : MonoBehaviour
    {
        [SerializeField] private DialogData m_Dialog;
        public UnityEvent OnCheckStart;
        public UnityEvent OnCheckEnd;

        public void Check()
        {
            OnCheckStart?.Invoke();
            if (m_Dialog.IsValid())
            {
                UIManager.PushAction(new UIStackedAction
                {
                    Action = () =>
                    {
                        OnCheckEnd?.Invoke();
                    },
                    
                    Name = "PointOfInterest.Check (OnCheckEnd)"
                });
                
                UIManager.Get<UIDialog>().Show(m_Dialog);
            }
        }
    }
}