using UnityEngine;

namespace UI_Codebase
{
    public interface IUIInput
    {
        Vector2 GetPrimaryAxis();
        bool IsConfirmDown();
        bool IsCancelDown();
        bool IsTogglePauseDown();
        bool IsToggleInventoryDown();
        void Flush();
    }
}