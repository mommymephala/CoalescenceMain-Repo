using UnityEngine;

namespace HEScripts.UI
{
    public interface IUIInput
    {
        Vector2 GetPrimaryAxis();
        bool IsConfirmDown();
        bool IsCancelDown();
        bool IsTogglePauseDown();
        bool IsToggleInventoryDown();
        bool IsToggleMapDown();
        bool IsToggleMapListDown();
        bool IsPrevSubmapDown();
        bool IsNextSubmapDown();
        void Flush();
    }
}