using Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI_Codebase
{
#if ENABLE_INPUT_SYSTEM
    public class UIInputNew : MonoBehaviour, IUIInput
    {
        private Vector2 m_InputAxis;

        private InputActionProcessor m_CancelP = new InputActionProcessor();
        private InputActionProcessor m_ConfirmP = new InputActionProcessor();
        private InputActionProcessor m_ToggleInventoryP = new InputActionProcessor();
        private InputActionProcessor m_TogglePauseP = new InputActionProcessor();

        // ------------------------------------ SendMessages from PlayerInput component

        private void OnCancel(InputValue value)
        {
            m_CancelP.Process(value);
        }

        private void OnConfirm(InputValue value)
        {
            m_ConfirmP.Process(value);
        }

        private void OnToggleInventory(InputValue value)
        {
            m_ToggleInventoryP.Process(value);
        }

        private void OnTogglePause(InputValue value)
        {
            m_TogglePauseP.Process(value);
        }

        private void OnPrimaryAxis(InputValue value)
        {
            m_InputAxis = value.Get<Vector2>();
        }

        // ------------------------------------- IUIInput Implementation

        public bool IsCancelDown()
        {
            return m_CancelP.IsDown();
        }
        
        public bool IsConfirmDown()
        {
            return m_ConfirmP.IsDown();
        }

        public bool IsTogglePauseDown()
        {
            return m_TogglePauseP.IsDown();
        }

        public bool IsToggleInventoryDown()
        {
            return m_ToggleInventoryP.IsDown();
        }

        public Vector2 GetPrimaryAxis()
        {
            return m_InputAxis;
        }

        public void Flush()
        {
            m_CancelP.Clear();
            m_ConfirmP.Clear();
            m_ToggleInventoryP.Clear();
            m_TogglePauseP.Clear();
        }

    }
#else
    public class UIInputNew : MonoBehaviour { }
#endif
}